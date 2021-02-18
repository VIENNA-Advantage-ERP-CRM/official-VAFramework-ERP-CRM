/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAM_ProductCostQueue
 * Chronological Development
 * Veena Pandey     18-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using System.Runtime.CompilerServices;

namespace VAdvantage.Model
{
    public class MVAMProductCostQueue : X_VAM_ProductCostQueue
    {
        /**	Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAMProductCostQueue).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="ignored">multi-key</param>
        /// <param name="trxName">transaction</param>
        public MVAMProductCostQueue(Ctx ctx, int ignored, Trx trxName)
            : base(ctx, ignored, trxName)
        {
            if (ignored == 0)
            {
                //	setVAB_AccountBook_ID (0);
                //	setVAM_PFeature_SetInstance_ID (0);
                //	setVAM_ProductCostElement_ID (0);
                //	setVAM_ProductCostType_ID (0);
                //	setVAM_Product_ID (0);
                SetCurrentCostPrice(Env.ZERO);
                SetCurrentQty(Env.ZERO);
            }
            else
                throw new ArgumentException("Multi-Key");
        }

        /// <summary>
        /// Load Construor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MVAMProductCostQueue(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="product">product</param>
        /// <param name="VAM_PFeature_SetInstance_ID">Attribute Set Instance</param>
        /// <param name="mas">Acct Schema</param>
        /// <param name="VAF_Org_ID">org</param>
        /// <param name="VAM_ProductCostElement_ID">cost element</param>
        /// <param name="trxName">transaction</param>
        public MVAMProductCostQueue(MProduct product, int VAM_PFeature_SetInstance_ID,
            MVABAccountBook mas, int VAF_Org_ID, int VAM_ProductCostElement_ID, Trx trxName)
            : this(product.GetCtx(), 0, trxName)
        {
            SetClientOrg(product.GetVAF_Client_ID(), VAF_Org_ID);
            SetVAB_AccountBook_ID(mas.GetVAB_AccountBook_ID());
            SetVAM_ProductCostType_ID(mas.GetVAM_ProductCostType_ID());
            SetVAM_Product_ID(product.GetVAM_Product_ID());
            SetVAM_PFeature_SetInstance_ID(VAM_PFeature_SetInstance_ID);
            SetVAM_ProductCostElement_ID(VAM_ProductCostElement_ID);
        }

        /// <summary>
        /// Adjust Qty based on in Lifo/Fifo order
        /// </summary>
        /// <param name="product">product</param>
        /// <param name="M_ASI_ID">costing level ASI</param>
        /// <param name="mas">accounting schema</param>
        /// <param name="Org_ID">costing level org</param>
        /// <param name="ce">Cost Element</param>
        /// <param name="Qty">quantity to be reduced</param>
        /// <param name="trxName">transaction</param>
        /// <returns>cost price reduced or null of error</returns>
        public static Decimal? AdjustQty(MProduct product, int M_ASI_ID, MVABAccountBook mas,
                int Org_ID, MVAMProductCostElement ce, Decimal Qty, Trx trxName)
        {
            if (Env.Signum(Qty) == 0)
                return Env.ZERO;
            MVAMProductCostQueue[] costQ = GetQueue(product, M_ASI_ID, mas, Org_ID, ce, trxName);
            Decimal remainingQty = Qty;
            for (int i = 0; i < costQ.Length; i++)
            {
                MVAMProductCostQueue queue = costQ[i];
                //	Negative Qty i.e. add
                if (Env.Signum(remainingQty) < 0)
                {
                    Decimal oldQty = queue.GetCurrentQty();
                    Decimal newQty = Decimal.Subtract(oldQty, remainingQty);
                    queue.SetCurrentQty(newQty);
                    if (queue.Save())
                    {
                        _log.Fine("Qty=" + remainingQty
                            + "(!), ASI=" + queue.GetVAM_PFeature_SetInstance_ID()
                            + " - " + oldQty + " -> " + newQty);
                        return queue.GetCurrentCostPrice();
                    }
                    else
                        return null;
                }

                //	Positive queue
                if (Env.Signum(queue.GetCurrentQty()) > 0)
                {
                    Decimal reduction = remainingQty;
                    if (reduction.CompareTo(queue.GetCurrentQty()) > 0)
                        reduction = queue.GetCurrentQty();
                    Decimal oldQty = queue.GetCurrentQty();
                    Decimal newQty = Decimal.Subtract(oldQty, reduction);
                    queue.SetCurrentQty(newQty);
                    if (queue.Save())
                    {
                        _log.Fine("Qty=" + reduction
                            + ", ASI=" + queue.GetVAM_PFeature_SetInstance_ID()
                            + " - " + oldQty + " -> " + newQty);
                        remainingQty = Decimal.Subtract(remainingQty, reduction);
                    }
                    else
                        return null;
                    //
                    if (Env.Signum(remainingQty) == 0)
                    {
                        return queue.GetCurrentCostPrice();
                    }
                }
            }	//	for queue	

            _log.Fine("RemainingQty=" + remainingQty);
            return null;
        }

        /// <summary>
        /// Get/Create Cost Queue Record.
        ///	CostingLevel is not validated
        /// </summary>
        /// <param name="product">product</param>
        /// <param name="VAM_PFeature_SetInstance_ID">real asi</param>
        /// <param name="mas">accounting schema</param>
        /// <param name="VAF_Org_ID">real org</param>
        /// <param name="VAM_ProductCostElement_ID">element</param>
        /// <param name="trxName">transaction</param>
        /// <returns>cost queue or null</returns>
        public static MVAMProductCostQueue Get(MProduct product, int VAM_PFeature_SetInstance_ID,
            MVABAccountBook mas, int VAF_Org_ID, int VAM_ProductCostElement_ID, Trx trxName)
        {
            MVAMProductCostQueue costQ = null;
            String sql = "SELECT * FROM VAM_ProductCostQueue "
                + "WHERE VAF_Client_ID=@client AND VAF_Org_ID=@org"
                + " AND VAM_Product_ID=@pro"
                + " AND VAM_PFeature_SetInstance_ID=@asi"
                + " AND VAM_ProductCostType_ID=@ct AND VAB_AccountBook_ID=@accs"
                + " AND VAM_ProductCostElement_ID=@ce";
            try
            {
                SqlParameter[] param = new SqlParameter[7];
                param[0] = new SqlParameter("@client", product.GetVAF_Client_ID());
                param[1] = new SqlParameter("@org", VAF_Org_ID);
                param[2] = new SqlParameter("@pro", product.GetVAM_Product_ID());
                param[3] = new SqlParameter("@asi", VAM_PFeature_SetInstance_ID);
                param[4] = new SqlParameter("@ct", mas.GetVAM_ProductCostType_ID());
                param[5] = new SqlParameter("@accs", mas.GetVAB_AccountBook_ID());
                param[6] = new SqlParameter("@ce", VAM_ProductCostElement_ID);

                DataSet ds = DataBase.DB.ExecuteDataset(sql, param);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        costQ = new MVAMProductCostQueue(product.GetCtx(), dr, trxName);
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            //	New
            if (costQ == null)
                costQ = new MVAMProductCostQueue(product, VAM_PFeature_SetInstance_ID,
                    mas, VAF_Org_ID, VAM_ProductCostElement_ID, trxName);
            return costQ;
        }

        /// <summary>
        /// Calculate Cost based on Qty based on in Lifo/Fifo order
        /// </summary>
        /// <param name="product">product</param>
        /// <param name="M_ASI_ID">costing level ASI</param>
        /// <param name="mas">accounting schema</param>
        /// <param name="Org_ID">costing level org</param>
        /// <param name="ce">Cost Element</param>
        /// <param name="Qty">quantity to be reduced</param>
        /// <param name="trxName">transaction</param>
        /// <returns>cost for qty or null of error</returns>
        public static Decimal? GetCosts(MProduct product, int M_ASI_ID, MVABAccountBook mas,
            int Org_ID, MVAMProductCostElement ce, Decimal Qty, Trx trxName)
        {
            if (Env.Signum(Qty) == 0)
                return Env.ZERO;
            MVAMProductCostQueue[] costQ = GetQueue(product, M_ASI_ID,
                mas, Org_ID, ce, trxName);
            //
            Decimal cost = Env.ZERO;
            Decimal remainingQty = Qty;
            Decimal? firstPrice = null;
            Decimal? lastPrice = null;
            //
            for (int i = 0; i < costQ.Length; i++)
            {
                MVAMProductCostQueue queue = costQ[i];
                //	Negative Qty i.e. add
                if (Env.Signum(remainingQty) <= 0)
                {
                    Decimal oldQty = queue.GetCurrentQty();
                    lastPrice = queue.GetCurrentCostPrice();
                    Decimal costBatch = Decimal.Multiply((Decimal)lastPrice, remainingQty);
                    cost = Decimal.Add(cost, costBatch);
                    _log.Config("ASI=" + queue.GetVAM_PFeature_SetInstance_ID()
                        + " - Cost=" + lastPrice + " * Qty=" + remainingQty + "(!) = " + costBatch);
                    return cost;
                }

                //	Positive queue
                if (Env.Signum(queue.GetCurrentQty()) > 0)
                {
                    Decimal reduction = remainingQty;
                    if (reduction.CompareTo(queue.GetCurrentQty()) > 0)
                        reduction = queue.GetCurrentQty();
                    Decimal oldQty = queue.GetCurrentQty();
                    lastPrice = queue.GetCurrentCostPrice();
                    Decimal costBatch = Decimal.Multiply((Decimal)lastPrice, reduction);
                    cost = Decimal.Add(cost, costBatch);
                    _log.Fine("ASI=" + queue.GetVAM_PFeature_SetInstance_ID()
                      + " - Cost=" + lastPrice + " * Qty=" + reduction + " = " + costBatch);
                    remainingQty = Decimal.Subtract(remainingQty, reduction);
                    //	Done
                    if (Env.Signum(remainingQty) == 0)
                    {
                        _log.Config("Cost=" + cost);
                        return cost;
                    }
                    if (firstPrice == null)
                        firstPrice = lastPrice;
                }
            }	//	for queue

            if (lastPrice == null)
            {
                lastPrice = MVAMProductCost.GetSeedCosts(product, M_ASI_ID, mas, Org_ID,
                    ce.GetCostingMethod(), 0);
                if (lastPrice == null)
                {
                    _log.Info("No Price found");
                    return null;
                }
                _log.Info("No Cost Queue");
            }
            Decimal costBatch1 = Decimal.Multiply((Decimal)lastPrice, remainingQty);
            _log.Fine("RemainingQty=" + remainingQty + " * LastPrice=" + lastPrice + " = " + costBatch1);
            cost = Decimal.Add(cost, costBatch1);
            _log.Config("Cost=" + cost);
            return cost;
        }

        /// <summary>
        /// Get Cost Queue Records in Lifo/Fifo order
        /// </summary>
        /// <param name="product">product</param>
        /// <param name="M_ASI_ID">costing level ASI</param>
        /// <param name="mas">accounting schema</param>
        /// <param name="Org_ID">costing level org</param>
        /// <param name="ce">Cost Element</param>
        /// <param name="trxName">transaction</param>
        /// <returns>cost queue or null</returns>
        public static MVAMProductCostQueue[] GetQueue(MProduct product, int M_ASI_ID, MVABAccountBook mas,
            int Org_ID, MVAMProductCostElement ce, Trx trxName, int VAM_Warehouse_ID = 0)
        {
            List<MVAMProductCostQueue> list = new List<MVAMProductCostQueue>();
            String sql = "SELECT * FROM VAM_ProductCostQueue "
                + "WHERE VAF_Client_ID=@client "
                + " AND VAM_Product_ID=@prod"
                + " AND VAM_ProductCostType_ID=@ct AND VAB_AccountBook_ID=@accs"
                + " AND VAM_ProductCostElement_ID=@ce";
            if (Org_ID != 0)
                sql += " AND VAF_Org_ID=@org";
            if (VAM_Warehouse_ID != 0)
                sql += " AND NVL(VAM_Warehouse_ID, 0) = " + VAM_Warehouse_ID;
            if (M_ASI_ID != 0)
                sql += " AND VAM_PFeature_SetInstance_ID=@asi";
            sql += " AND CurrentQty<>0 ";
            sql += "ORDER BY queuedate ";
            if (!ce.IsFifo())
                sql += "DESC ";
            sql += " , VAM_PFeature_SetInstance_ID ";
            if (!ce.IsFifo())
                sql += "DESC";
            try
            {
                SqlParameter[] param = null;
                if (M_ASI_ID != 0 && Org_ID != 0)
                {
                    param = new SqlParameter[7];
                }
                else if (M_ASI_ID == 0 && Org_ID == 0)
                {
                    param = new SqlParameter[5];
                }
                else
                {
                    param = new SqlParameter[6];
                }
                param[0] = new SqlParameter("@client", product.GetVAF_Client_ID());
                param[1] = new SqlParameter("@prod", product.GetVAM_Product_ID());
                param[2] = new SqlParameter("@ct", mas.GetVAM_ProductCostType_ID());
                param[3] = new SqlParameter("@accs", mas.GetVAB_AccountBook_ID());
                param[4] = new SqlParameter("@ce", ce.GetVAM_ProductCostElement_ID());
                if (M_ASI_ID != 0 && Org_ID != 0)
                {
                    param[5] = new SqlParameter("@org", Org_ID);
                    param[6] = new SqlParameter("@asi", M_ASI_ID);
                }
                else if (Org_ID != 0)
                {
                    param[5] = new SqlParameter("@org", Org_ID);
                }
                else if (M_ASI_ID != 0)
                {
                    param[5] = new SqlParameter("@asi", M_ASI_ID);
                }
                DataSet ds = DataBase.DB.ExecuteDataset(sql, param, trxName);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MVAMProductCostQueue(product.GetCtx(), dr, trxName));
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            MVAMProductCostQueue[] costQ = new MVAMProductCostQueue[list.Count];
            costQ = list.ToArray();
            return costQ;
        }

        /// <summary>
        /// Update Record.
        ///	((OldAvg*OldQty)+(Price*Qty)) / (OldQty+Qty)
        /// </summary>
        /// <param name="amt">total Amount</param>
        /// <param name="qty">quantity</param>
        /// <param name="precision">costing precision</param>
        public void SetCosts(Decimal amt, Decimal qty, int precision)
        {
            Decimal oldSum = Decimal.Multiply(GetCurrentCostPrice(), GetCurrentQty());
            Decimal newSum = amt;	//	is total already
            Decimal sumAmt = Decimal.Add(oldSum, newSum);
            Decimal sumQty = Decimal.Add(GetCurrentQty(), qty);
            if (Env.Signum(sumQty) != 0)
            {
                Decimal cost = Decimal.Round(Decimal.Divide(sumAmt, sumQty), precision, MidpointRounding.AwayFromZero);
                SetCurrentCostPrice(cost);
            }
            //
            SetCurrentQty(Decimal.Add(GetCurrentQty(), qty));
        }

        //Created by amit
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static bool CreateProductCostsDetails(Ctx ctx, int VAF_Client_ID, int VAF_Org_ID, MProduct product, int M_ASI_ID,
                     string windowName, MVAMInventoryLine inventoryLine, MVAMInvInOutLine inoutline, MVAMInvTrfLine movementline,
                     MVABInvoiceLine invoiceline, PO po, Decimal Price, Decimal Qty, Trx trxName, out string conversionNotFound,
                     string optionalstr = "process")
        {
            MVABAccountBook acctSchema = null;
            dynamic pca = null;
            string costingMethodMatchPO = null;
            MVAMProductCostElement costElement = null;
            MProduct productLca = null;
            MVABOrderLine orderline = null;
            MVABOrder order = null;
            MVABInvoice invoice = null;
            MVAMInventoryTransfer movement = null;
            MVAMInventory inventory = null;
            MVAMInvInOutLine matchInoutLine = inoutline;
            MVAMProductCostDetail cd = null;
            MVAMProductCostDetail cdSourceWarehouse = null;
            MVAMProductCost cost = null;
            MVAMInvInOut inout = null;
            Decimal price = 0;
            Decimal plPrice = 0; // price list price
            Decimal cmPrice = 0; // costing method price
            Decimal receivedPrice = Price;
            Decimal receivedQty = Qty;
            String costingMethod = string.Empty;
            int costingElementId = 0;
            bool isLandedCostAllocation = false;
            int orderLineId = 0;
            int costDetailId = 0;
            bool isPriceFromProductPrice = false;
            int VAF_Org_ID2 = VAF_Org_ID;
            DataSet ds = null;
            string cl = "";
            string isMatchFromForm = "N";
            string handlingWindowName = windowName;
            bool result = true;
            int MatchPO_OrderLineId = 0;
            StringBuilder query = new StringBuilder();
            conversionNotFound = "";
            MVAFClient client = MVAFClient.Get(ctx, VAF_Client_ID);
            string costQueuseIds = null;
            int VAM_Warehouse_Id = 0; // is used to manage costing at warehouse level
            int SourceVAM_Warehouse_Id = 0; // is used to manage costing at warehouse level during Inventory Move
            MVAMLocator loc = null; // is used to get warehouse id to manager costing levelev - Warehouse + Batch 
            String costLevel = null; // is used to check costing level binded for calculation of costing
            try
            {
                if (product != null)
                {
                    _log.Info("costing Calculation Start for window = " + windowName + " and product = " + product.GetVAM_Product_ID() + " AND ASI " + M_ASI_ID);
                }
                else
                {
                    _log.Info("costing Calculation Start for window = " + windowName);
                }

                // when we match a record through form - then costing calculate only for defined costing method
                if (client.IsCostImmediate() && (handlingWindowName == "Match IV" || handlingWindowName == "Match PO"))
                {
                    optionalstr = "window";

                    if (handlingWindowName == "Match PO")
                    {
                        inout = new MVAMInvInOut(ctx, inoutline.GetVAM_Inv_InOut_ID(), trxName);
                    }
                }

                query.Clear();
                //query.Append("SELECT VAB_AccountBook_ID FROM VAB_AccountBook WHERE IsActive = 'Y' AND VAF_Client_ID = " + VAF_Client_ID);
                // first calculate cost for primary accounting schema then calculate for other 
                query.Append(@"Select VAB_AccountBook_Id From VAB_AccountBook
                                WHERE Isactive = 'Y' AND VAB_AccountBook_Id = (SELECT VAB_AccountBook1_Id FROM VAF_ClientDetail WHERE vaf_client_Id = " + VAF_Client_ID + @" )
                                Union
                                Select VAB_AccountBook_Id From VAB_AccountBook Where Isactive = 'Y' And vaf_client_Id = " + VAF_Client_ID + @"
                                AND VAB_AccountBook_Id != (SELECT VAB_AccountBook1_Id FROM VAF_ClientDetail WHERE vaf_client_Id = " + VAF_Client_ID + " )");
                ds = DB.ExecuteDataset(query.ToString(), null, null);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        cd = null;
                        Price = receivedPrice;
                        Qty = receivedQty;
                        VAF_Org_ID = VAF_Org_ID2;
                        acctSchema = new MVABAccountBook(ctx, Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_AccountBook_ID"]), trxName);
                        if (product != null)
                        {
                            #region Costing Level
                            pca = new MProductCategory(ctx, product.GetVAM_ProductCategory_ID(), null);
                            if (product.GetProductType() == "E")
                            {
                                // for expense type product, we didnt check costing level
                                // bcz if we define costing level as client and try to calculate landed cost whose costing level is Organization
                                // then system calculate its costing in (*) org but this product costing level is Org. 
                            }
                            else
                            {
                                // get costing level
                                costLevel = (MProductCategory.Get(product.GetCtx(), product.GetVAM_ProductCategory_ID())).GetCostingLevel();
                                if (String.IsNullOrEmpty(costLevel))
                                {
                                    costLevel = acctSchema.GetCostingLevel();
                                }

                                if (windowName != "Inventory Move")
                                {
                                    if (pca != null)
                                    {
                                        if (pca.GetCostingLevel() == "C" || pca.GetCostingLevel() == "B")
                                        {
                                            VAF_Org_ID = 0;
                                        }
                                        else if ((string.IsNullOrEmpty(pca.GetCostingLevel())) && (acctSchema.GetCostingLevel() == "C" || acctSchema.GetCostingLevel() == "B"))
                                        {
                                            VAF_Org_ID = 0;
                                        }
                                    }
                                    else if (acctSchema.GetCostingLevel() == "C" || acctSchema.GetCostingLevel() == "B")
                                    {
                                        VAF_Org_ID = 0;
                                    }
                                }
                            }
                            #endregion
                        }

                        // need to check qty available on product cost for reduction
                        if (optionalstr == "window" && Qty < 0 &&
                            (windowName == "Shipment" || windowName == "Return To Vendor" ||
                             windowName == "Physical Inventory" || windowName == "Internal Use Inventory" || windowName == "AssetDisposal"))
                        {
                            if (windowName != "AssetDisposal")
                            {
                                loc = MVAMLocator.Get(ctx, (inoutline != null ? inoutline.GetVAM_Locator_ID() : inventoryLine.GetVAM_Locator_ID()));
                            }
                            else
                            {
                                if (loc == null)
                                {
                                    loc = MVAMLocator.Get(ctx, Util.GetValueOfInt(po.Get_Value("VAM_Locator_ID")));
                                }
                            }
                            decimal productCostsQty = MVAMProductCostQueue.CheckQtyAvailablity(ctx, acctSchema, VAF_Client_ID, VAF_Org_ID, product, M_ASI_ID, loc.GetVAM_Warehouse_ID());
                            if (productCostsQty < Decimal.Negate(Qty))
                            {
                                return false;
                            }
                        }
                        else if (windowName == "Inventory Move" && VAF_Org_ID > 0)
                        {
                            // when costing level is other than Warehouse wise and "Organization" of "Source And To Warehouse" is same then not require to calculate cost
                            // when costing level is Warehouse wise and "From AND To" warehouse is same then not require to calculate cost
                            if (costLevel == MProductCategory.COSTINGLEVEL_Warehouse || costLevel == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                            {
                                if (MVAMLocator.Get(ctx, (movementline != null ? movementline.GetVAM_Locator_ID() : 0)).GetVAM_Warehouse_ID() ==
                                    MVAMLocator.Get(ctx, (movementline != null ? movementline.GetVAM_LocatorTo_ID() : 0)).GetVAM_Warehouse_ID())
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (MVAMLocator.Get(ctx, (movementline != null ? movementline.GetVAM_Locator_ID() : 0)).GetVAF_Org_ID() ==
                                       MVAMLocator.Get(ctx, (movementline != null ? movementline.GetVAM_LocatorTo_ID() : 0)).GetVAF_Org_ID())
                                {
                                    return true;
                                }
                            }

                            // when qty is positive -- we are reducing stock from "Source Warehouse"
                            if (Qty > 0)
                            {
                                loc = MVAMLocator.Get(ctx, (movementline != null ? movementline.GetVAM_Locator_ID() : 0));
                            }
                            // when qty is negative -- we are reducing stock from "To Warehouse"
                            else
                            {
                                loc = MVAMLocator.Get(ctx, (movementline != null ? movementline.GetVAM_LocatorTo_ID() : 0));
                            }
                            decimal productCostsQty = MVAMProductCostQueue.CheckQtyAvailablity(ctx, acctSchema, VAF_Client_ID, (Qty > 0 ? VAF_Org_ID : loc.GetVAF_Org_ID()), product, M_ASI_ID, loc.GetVAM_Warehouse_ID());
                            if (productCostsQty < Math.Abs(Qty))
                            {
                                return false;
                            }
                        }

                        if (windowName == "Material Receipt" || windowName == "Customer Return" || windowName == "Shipment" || windowName == "Return To Vendor")
                        {
                            #region Get Price with conversion if required
                            inout = new MVAMInvInOut(ctx, inoutline.GetVAM_Inv_InOut_ID(), trxName);
                            VAM_Warehouse_Id = inout.GetVAM_Warehouse_ID();
                            if (inoutline.GetVAB_OrderLine_ID() > 0)
                            {
                                orderline = new MVABOrderLine(ctx, inoutline.GetVAB_OrderLine_ID(), trxName);
                                order = new MVABOrder(ctx, orderline.GetVAB_Order_ID(), trxName);
                                if (order.GetVAB_Currency_ID() != acctSchema.GetVAB_Currency_ID())
                                {
                                    // convert amount on account date of VAM_Inv_InOut (discussed with Ashish, Suya, Mukesh sir)
                                    Price = MVABExchangeRate.Convert(ctx, Price, order.GetVAB_Currency_ID(), acctSchema.GetVAB_Currency_ID(),
                                                                     inout.GetDateAcct(), order.GetVAB_CurrencyType_ID(), VAF_Client_ID, VAF_Org_ID);
                                    if (Price == 0)
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        conversionNotFound = inout.GetDocumentNo();
                                        return false;
                                    }
                                }
                                else if (Price == 0) //when order created with ZERO price then not to calculate cost 
                                {
                                    if (optionalstr != "window")
                                    {
                                        trxName.Rollback();
                                    }
                                    conversionNotFound = inout.GetDocumentNo();
                                    return false;
                                }
                            }
                            else
                            {
                                //28-4-2016
                                //get price from product costs based on costing method if not found 
                                //then check price list based on trx org or (*) org 
                                price = MVAMProductCostQueue.CalculateCost(ctx, acctSchema, product, M_ASI_ID, VAF_Client_ID, VAF_Org_ID, VAM_Warehouse_Id);
                                Price = price * inoutline.GetMovementQty();
                                cmPrice = Price;
                                if (Price == 0)
                                {
                                    Price = GetPrice(inout, inoutline, acctSchema.GetVAB_Currency_ID());
                                }
                                if (Price == 0)
                                {
                                    // when we not find price on product cost and price list then we check invoice price
                                    // check any invoice is created with this MR if yes then consider first invoice price for this MR costing
                                    int invoiceLineId = 0;
                                    if (invoiceline == null)
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT MIN(il.VAB_InvoiceLine_ID) FROM VAB_InvoiceLine il INNER JOIN VAB_Invoice i 
                                                   ON i.VAB_Invoice_id  = il.VAB_Invoice_id
                                                   WHERE i.DocStatus IN ('CO' , 'CL') AND il.IsActive = 'Y'
                                                   AND il.VAM_Inv_InOutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID());
                                        invoiceLineId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));
                                    }
                                    else
                                    {
                                        invoiceLineId = invoiceline.GetVAB_InvoiceLine_ID();
                                    }
                                    if (invoiceLineId > 0)
                                    {
                                        invoiceline = new MVABInvoiceLine(ctx, invoiceLineId, trxName);
                                        invoice = new MVABInvoice(ctx, invoiceline.GetVAB_Invoice_ID(), trxName);

                                        Price = Decimal.Divide(invoiceline.GetProductLineCost(invoiceline), invoiceline.GetQtyInvoiced());
                                        Price = Price * inoutline.GetMovementQty();
                                        if (invoice.GetVAB_Currency_ID() != acctSchema.GetVAB_Currency_ID())
                                        {
                                            // convert amount on account date of VAM_Inv_InOut (discussed with Ashish, Suya, Mukesh sir)
                                            Price = MVABExchangeRate.Convert(ctx, Price, invoice.GetVAB_Currency_ID(), acctSchema.GetVAB_Currency_ID(),
                                                                inout.GetDateAcct(), invoice.GetVAB_CurrencyType_ID(), VAF_Client_ID, VAF_Org_ID);
                                        }
                                        invoice = null;
                                        invoiceline = null;
                                    }
                                }
                                if (Price != 0 && windowName == "Return To Vendor")
                                {
                                    Price = Decimal.Negate(Price);
                                    cmPrice = Price;
                                }
                                if (Price == 0)
                                {
                                    if (optionalstr != "window")
                                    {
                                        trxName.Rollback();
                                    }
                                    conversionNotFound = inout.GetDocumentNo();
                                    return false;
                                }
                            }
                            invoice = null;
                            invoiceline = null;
                            #endregion
                        }
                        else if (windowName == "Invoice(Vendor)" || windowName == "Invoice(Customer)" || windowName == "Match IV" || windowName == "Invoice(Vendor)-Return")
                        {
                            #region Get Price with conversion if required
                            invoice = new MVABInvoice(ctx, invoiceline.GetVAB_Invoice_ID(), trxName);
                            if (invoiceline.GetVAM_Inv_InOutLine_ID() > 0)
                            {
                                VAM_Warehouse_Id = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT VAM_Warehouse_id FROM VAM_Inv_InOut WHERE VAM_Inv_InOut_id = 
                                    ( SELECT VAM_Inv_InOut_id FROM VAM_Inv_InOutLine WHERE VAM_Inv_InOutLine_id = " + invoiceline.GetVAM_Inv_InOutLine_ID() + " )", null, trxName));
                            }
                            if (invoice.GetVAB_Currency_ID() != acctSchema.GetVAB_Currency_ID())
                            {
                                Price = MVABExchangeRate.Convert(ctx, Price, invoice.GetVAB_Currency_ID(), acctSchema.GetVAB_Currency_ID(),
                                                                         invoice.GetDateAcct(), invoice.GetVAB_CurrencyType_ID(), VAF_Client_ID, VAF_Org_ID);
                                if (Price == 0)
                                {
                                    if (optionalstr != "window")
                                    {
                                        trxName.Rollback();
                                    }
                                    conversionNotFound = invoice.GetDocumentNo();
                                    return false;
                                }
                            }
                            else if (Price == 0) //when invoice created with ZERO price then not to calculate cost 
                            {
                                if (optionalstr != "window")
                                {
                                    trxName.Rollback();
                                }
                                conversionNotFound = inout.GetDocumentNo();
                                return false;
                            }
                            #endregion
                        }

                        // check MR matached from PO through Form or not
                        // if yes, then call Match IV
                        if (windowName == "Invoice(Vendor)")
                        {
                            if (product != null)
                            {
                                #region when we match Invoice (vendor) with Material receipt through matching Form - then calculate costing through match IV region
                                query.Clear();
                                query.Append(@"SELECT IsMatchPOForm FROM VAM_MatchPO WHERE IsActive = 'Y'   AND VAM_Product_ID = " + product.GetVAM_Product_ID());
                                if (matchInoutLine != null && matchInoutLine.GetVAM_Inv_InOutLine_ID() > 0)
                                {
                                    query.Append(" AND VAM_Inv_InOutLine_ID = " + Util.GetValueOfInt(matchInoutLine.GetVAM_Inv_InOutLine_ID()));
                                }
                                else
                                {
                                    query.Append(" AND VAM_Inv_InOutLine_ID = " + Util.GetValueOfInt(invoiceline.GetVAM_Inv_InOutLine_ID()));
                                }
                                isMatchFromForm = Util.GetValueOfString(DB.ExecuteScalar(query.ToString(), null, trxName));
                                if (isMatchFromForm == "Y")
                                {
                                    if (matchInoutLine != null && matchInoutLine.GetVAM_Inv_InOutLine_ID() > 0)
                                    {
                                        inoutline = new MVAMInvInOutLine(ctx, matchInoutLine.GetVAM_Inv_InOutLine_ID(), trxName);
                                    }
                                    else
                                    {
                                        inoutline = new MVAMInvInOutLine(ctx, invoiceline.GetVAM_Inv_InOutLine_ID(), trxName);
                                    }
                                    if (inoutline.GetVAM_Inv_InOutLine_ID() > 0)
                                    {
                                        M_ASI_ID = inoutline.GetVAM_PFeature_SetInstance_ID();
                                        if (!client.IsCostImmediate())
                                        {
                                            windowName = "Match IV";
                                        }
                                    }
                                }
                                #endregion
                            }

                            if (invoiceline.GetVAB_OrderLine_ID() <= 0)
                            {
                                #region when invoice line not having orderline ref but contain inoutline ref then handlinf costing through Match IV region
                                if (matchInoutLine != null && matchInoutLine.GetVAM_Inv_InOutLine_ID() > 0)
                                {
                                    inoutline = new MVAMInvInOutLine(ctx, matchInoutLine.GetVAM_Inv_InOutLine_ID(), trxName);
                                }
                                else
                                {
                                    inoutline = new MVAMInvInOutLine(ctx, invoiceline.GetVAM_Inv_InOutLine_ID(), trxName);
                                }
                                if (inoutline.GetVAM_Inv_InOutLine_ID() > 0)
                                {
                                    M_ASI_ID = inoutline.GetVAM_PFeature_SetInstance_ID();
                                    windowName = "Match IV";
                                }
                                #endregion
                            }
                        }

                        if (windowName == "Match PO")
                        {
                            #region Match PO
                            decimal MRPriceAvPo = 0;
                            decimal MRPriceLastPO = 0;
                            bool isRecordFromForm = false;

                            #region get costing level and costing method
                            pca = MProductCategory.Get(product.GetCtx(), product.GetVAM_ProductCategory_ID());
                            if (pca != null)
                            {
                                cl = pca.GetCostingLevel();
                                costingMethodMatchPO = pca.GetCostingMethod();
                                if (costingMethodMatchPO == "C")
                                {
                                    query.Clear();
                                    query.Append(@" SELECT costingmethod FROM VAM_ProductCostElement WHERE VAM_ProductCostElement_id = (SELECT CAST(cel.M_Ref_CostElement AS INTEGER)
                                    FROM VAM_ProductCostElement ce INNER JOIN VAM_ProductCostElementLine cel ON ce.VAM_ProductCostElement_ID  = cel.VAM_ProductCostElement_ID
                                    WHERE ce.VAF_Client_ID   =" + product.GetVAF_Client_ID() + @" 
                                    AND ce.IsActive         ='Y' AND ce.CostElementType  ='C'
                                    AND cel.IsActive        ='Y' AND ce.VAM_ProductCostElement_ID = " + pca.GetVAM_ProductCostElement_ID() + @"
                                    AND CAST(cel.M_Ref_CostElement AS INTEGER) IN (SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE costingmethod IS NOT NULL  ) )
                                    ");
                                }
                            }
                            else
                            {
                                cl = acctSchema.GetCostingLevel();
                                costingMethodMatchPO = acctSchema.GetCostingMethod();

                                if (costingMethodMatchPO == "C")
                                {
                                    query.Clear();
                                    query.Append(@" SELECT costingmethod FROM VAM_ProductCostElement WHERE VAM_ProductCostElement_id = (SELECT CAST(cel.M_Ref_CostElement AS INTEGER)
                                    FROM VAM_ProductCostElement ce INNER JOIN VAM_ProductCostElementLine cel ON ce.VAM_ProductCostElement_ID  = cel.VAM_ProductCostElement_ID
                                    WHERE ce.VAF_Client_ID   =" + product.GetVAF_Client_ID() + @" 
                                    AND ce.IsActive         ='Y' AND ce.CostElementType  ='C'
                                    AND cel.IsActive        ='Y' AND ce.VAM_ProductCostElement_ID = " + pca.GetVAM_ProductCostElement_ID() + @"
                                    AND CAST(cel.M_Ref_CostElement AS INTEGER) IN (SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE costingmethod IS NOT NULL  ) )
                                    ");
                                }
                            }
                            if (costingMethodMatchPO == "C")
                            {
                                costingMethodMatchPO = Util.GetValueOfString(DB.ExecuteScalar(query.ToString(), null, trxName));
                            }
                            #endregion

                            inout = new MVAMInvInOut(ctx, inoutline.GetVAM_Inv_InOut_ID(), trxName);
                            orderLineId = Util.GetValueOfInt(Price); // here price parametr contain Orderline id from controller
                            orderline = new MVABOrderLine(ctx, orderLineId, trxName);

                            #region get price from purchase order
                            order = new MVABOrder(ctx, orderline.GetVAB_Order_ID(), trxName);
                            Decimal ProductOrderLineCost = orderline.GetProductLineCost(orderline);
                            if (order.GetVAB_Currency_ID() != acctSchema.GetVAB_Currency_ID())
                            {
                                Price = MVABExchangeRate.Convert(ctx, Decimal.Divide(ProductOrderLineCost, orderline.GetQtyOrdered()), order.GetVAB_Currency_ID(), acctSchema.GetVAB_Currency_ID(),
                                    (inout != null ? inout.GetDateAcct() : order.GetDateAcct()), order.GetVAB_CurrencyType_ID(), VAF_Client_ID, VAF_Org_ID);
                                if (Price == 0)
                                {
                                    if (optionalstr != "window")
                                    {
                                        trxName.Rollback();
                                    }
                                    conversionNotFound = order.GetDocumentNo();
                                    _log.Severe("Currency Conversion not found-> MatchPO. Order No is " + order.GetDocumentNo());
                                    return false;
                                }
                            }
                            else
                            {
                                Price = Decimal.Divide(ProductOrderLineCost, orderline.GetQtyOrdered());
                            }
                            #endregion

                            // when we are matching PO with MR // costing calculate on completion = true
                            // costing method is not Avregae PO / Last PO 
                            // then no need to calculate cost but we are creating cost detail with PO price and qty
                            if (client.IsCostImmediate())
                            {
                                #region cost detail creation or not
                                if (costingMethodMatchPO == "A" || costingMethodMatchPO == "p")
                                {
                                    // not to create cost detail by this section if costing method is either Average PO or Last PO
                                }
                                else if (inoutline.IsCostCalculated())
                                {
                                    //  need to calculate cost for Average PO and Last PO 
                                    // bcz we alreadyy calculate cost against this mr otherwise costing not to be calculated
                                }
                                else
                                {
                                    // created cost detail with orderline ref 
                                    cd = new MVAMProductCostDetail(acctSchema, VAF_Org_ID, product.GetVAM_Product_ID(), M_ASI_ID,
                                                          0, Decimal.Round(Decimal.Multiply(Qty, Price), acctSchema.GetCostingPrecision()), Qty, null, trxName);
                                    cd.SetVAM_Inv_InOutLine_ID(inoutline.GetVAM_Inv_InOutLine_ID());
                                    cd.SetVAB_OrderLine_ID(orderLineId);
                                    cd.SetVAM_Warehouse_ID(inout.GetVAM_Warehouse_ID());
                                    if (!cd.Save(trxName))
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        _log.Severe("Error occured during saving a record, VAM_Inv_InOutLine_Id = " + inoutline.GetVAM_Inv_InOutLine_ID() + " in Cost-Detail -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                        return false;
                                    }
                                    continue;
                                }
                                #endregion
                            }

                            costDetailId = 0;
                            if (!client.IsCostImmediate())
                            {
                                query.Clear();
                                query.Append("SELECT MIN(VAM_ProductCostDetail_ID) FROM VAM_ProductCostDetail WHERE VAM_Inv_InOutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID() +
                                     " AND VAB_InvoiceLine_id IS NULL  AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID());
                                costDetailId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));
                            }

                            cd = new MVAMProductCostDetail(ctx, costDetailId, trxName);
                            if (costDetailId == 0)
                            {
                                cd.SetClientOrg(VAF_Client_ID, VAF_Org_ID);
                                cd.SetVAM_Product_ID(product.GetVAM_Product_ID());
                                cd.SetVAM_Inv_InOutLine_ID(inoutline.GetVAM_Inv_InOutLine_ID());
                                cd.SetVAM_PFeature_SetInstance_ID(M_ASI_ID);
                                cd.SetVAB_AccountBook_ID(acctSchema.GetVAB_AccountBook_ID());
                                cd.SetVAM_Warehouse_ID(inout.GetVAM_Warehouse_ID());
                            }

                            // MR costing calculated before matching PO with MR
                            if (inoutline.IsCostCalculated() && inout.IsCostCalculated() && !client.IsCostImmediate())
                            {
                                #region Get MR price
                                query.Clear();
                                if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_Organization)
                                {
                                    query.Append(@"SELECT  ROUND(Amt/Qty , 4) as currentCostAmount  FROM VAM_ProductCostElementDetail ced 
                                                                where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @" 
                                                                 AND ced.VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() + @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) = 0" +
                                       " AND ced.VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) = 0 AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 and 
                                                                 ced.VAM_ProductCostElement_ID = (SELECT VAM_ProductCostElement_ID from VAM_ProductCostElement where costingMethod = 'A' AND IsActive = 'Y' AND VAF_Client_ID = " + inoutline.GetVAF_Client_ID() + " ) AND ced.VAF_Client_ID = " + inoutline.GetVAF_Client_ID());
                                }
                                else if (cl == MProductCategory.COSTINGLEVEL_BatchLot || cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch)
                                {
                                    query.Append(@"SELECT  ROUND(Amt/Qty , 4) as currentCostAmount  FROM VAM_ProductCostElementDetail ced 
                                                                where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @" 
                                                                 AND ced.VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() + @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) = " + M_ASI_ID + @" 
                                                                 AND ced.VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) = 0 AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 and 
                                                                 ced.VAM_ProductCostElement_ID = (SELECT VAM_ProductCostElement_ID from VAM_ProductCostElement where costingMethod = 'A' AND IsActive = 'Y' AND VAF_Client_ID = " + inoutline.GetVAF_Client_ID() + " ) AND ced.VAF_Client_ID = " + inoutline.GetVAF_Client_ID());
                                }
                                else if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                {
                                    query.Append(@"SELECT  ROUND(Amt/Qty , 4) as currentCostAmount  FROM VAM_ProductCostElementDetail ced 
                                                                where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @" AND  NVL(ced.VAM_Warehouse_ID , 0) = " + inout.GetVAM_Warehouse_ID() + @" 
                                                                 AND ced.VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                                              @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) = " + (cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch ? M_ASI_ID : 0) + @" 
                                                                 AND ced.VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) = 0 AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 and 
                                                                 ced.VAM_ProductCostElement_ID = (SELECT VAM_ProductCostElement_ID from VAM_ProductCostElement where costingMethod = 'A' AND IsActive = 'Y' AND VAF_Client_ID = " + inoutline.GetVAF_Client_ID() + " ) AND ced.VAF_Client_ID = " + inoutline.GetVAF_Client_ID());
                                }
                                MRPriceAvPo = Util.GetValueOfDecimal(DB.ExecuteScalar(query.ToString()));

                                query.Clear();
                                if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_Organization)
                                {
                                    query.Append(@"SELECT  ROUND(Amt/Qty , 4) as currentCostAmount  FROM VAM_ProductCostElementDetail ced 
                                                                where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @" 
                                                                 AND ced.VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() + @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) = 0 " +
                                                               " AND ced.VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) = 0 AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 AND 
                                                                 ced.VAM_ProductCostElement_ID = (SELECT VAM_ProductCostElement_ID from VAM_ProductCostElement where costingMethod = 'p' AND IsActive = 'Y' AND VAF_Client_ID = " + inoutline.GetVAF_Client_ID() + " ) AND ced.VAF_Client_ID = " + inoutline.GetVAF_Client_ID());
                                }
                                else if (cl == MProductCategory.COSTINGLEVEL_BatchLot || cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch)
                                {
                                    query.Append(@"SELECT  ROUND(Amt/Qty , 4) as currentCostAmount  FROM VAM_ProductCostElementDetail ced 
                                                                where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @" 
                                                                 AND ced.VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() + @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) = " + M_ASI_ID + @" 
                                                                 AND ced.VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) = 0 AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 and 
                                                                 ced.VAM_ProductCostElement_ID = (SELECT VAM_ProductCostElement_ID from VAM_ProductCostElement where costingMethod = 'p' AND IsActive = 'Y' AND VAF_Client_ID = " + inoutline.GetVAF_Client_ID() + " ) AND ced.VAF_Client_ID = " + inoutline.GetVAF_Client_ID());
                                }
                                else if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                {
                                    query.Append(@"SELECT  ROUND(Amt/Qty , 4) as currentCostAmount  FROM VAM_ProductCostElementDetail ced 
                                                                where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @"  AND  NVL(ced.VAM_Warehouse_ID , 0) = " + inout.GetVAM_Warehouse_ID() + @" 
                                                                 AND ced.VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                                              @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) = " + (cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch ? M_ASI_ID : 0) + @" 
                                                                 AND ced.VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) = 0 AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 and 
                                                                 ced.VAM_ProductCostElement_ID = (SELECT VAM_ProductCostElement_ID from VAM_ProductCostElement where costingMethod = 'p' AND IsActive = 'Y' AND VAF_Client_ID = " + inoutline.GetVAF_Client_ID() + " ) AND ced.VAF_Client_ID = " + inoutline.GetVAF_Client_ID());
                                }
                                MRPriceLastPO = Util.GetValueOfDecimal(DB.ExecuteScalar(query.ToString()));
                                if (MRPriceLastPO == 0)
                                {
                                    MRPriceLastPO = MRPriceAvPo;
                                }
                                #endregion
                            }

                            // match PO with MR before MR costing calculation
                            else if (!inoutline.IsCostCalculated())
                            {
                                #region when costing calculation is on completion no need to create cost queue
                                if (!client.IsCostImmediate())
                                {
                                    if (!CreateCostQueueForMatchPO(ctx, acctSchema, product, M_ASI_ID, VAF_Client_ID, VAF_Org_ID,
                                        Price, inoutline.GetMovementQty(), inoutline, trxName, inout.GetVAM_Warehouse_ID(), optionalStrPO: optionalstr))
                                    {
                                        _log.Severe("Error occured during craetion of cost queue -> MatchPO. VAM_Inv_InOutLine_ID is " + inoutline.GetVAM_Inv_InOutLine_ID());
                                        return false;
                                    }
                                    else
                                    {
                                        isRecordFromForm = true;
                                    }
                                }
                                #endregion
                            }

                            // match PO with MR but one already matched with that
                            else if (inoutline.IsCostCalculated() && !client.IsCostImmediate())
                            {
                                #region Get MR price from t_temp_CostDetail
                                MRPriceAvPo = 0;
                                MRPriceLastPO = 0;
                                query.Clear();
                                query.Append(@"SELECT amt FROM VAT_Temp_CostDetail WHERE IsActive = 'Y' AND VAF_Client_ID = " + VAF_Client_ID +
                                             " AND VAF_Org_ID = " + VAF_Org_ID + " AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                             " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND isRecordFromForm = 'Y' " +
                                             " AND VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID());
                                MRPriceAvPo = Util.GetValueOfDecimal(DB.ExecuteScalar(query.ToString(), null, trxName));
                                if (MRPriceAvPo == 0)
                                {
                                    query.Clear();
                                    query.Append(@"SELECT amt FROM VAT_Temp_CostDetail WHERE IsActive = 'Y' AND VAF_Client_ID = " + VAF_Client_ID +
                                                 " AND VAF_Org_ID = " + inoutline.GetVAF_Org_ID() + " AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                                 " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND isRecordFromForm = 'Y' " +
                                                 " AND VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID());
                                    MRPriceAvPo = Util.GetValueOfDecimal(DB.ExecuteScalar(query.ToString(), null, trxName));
                                }
                                MRPriceLastPO = MRPriceAvPo;
                                if (MRPriceAvPo != 0)
                                {
                                    isRecordFromForm = true;
                                }
                                #endregion
                            }

                            if (client.IsCostImmediate())
                            {
                                #region Get MR price from t_temp_CostDetail  costing calculation on completion
                                MRPriceAvPo = 0;
                                MRPriceLastPO = 0;
                                query.Clear();
                                query.Append(@"SELECT amt FROM VAT_Temp_CostDetail WHERE IsActive = 'Y' AND VAF_Client_ID = " + VAF_Client_ID +
                                             " AND VAF_Org_ID = " + VAF_Org_ID + " AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                             " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND NVL(VAB_OrderLine_ID , 0) = 0  " +
                                             " AND VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID());
                                MRPriceAvPo = Util.GetValueOfDecimal(DB.ExecuteScalar(query.ToString(), null, trxName));
                                if (MRPriceAvPo == 0)
                                {
                                    query.Clear();
                                    query.Append(@"SELECT amt FROM VAT_Temp_CostDetail WHERE IsActive = 'Y' AND VAF_Client_ID = " + VAF_Client_ID +
                                                 " AND VAF_Org_ID = " + inoutline.GetVAF_Org_ID() + " AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                                 " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND NVL(VAB_OrderLine_ID , 0) = 0  " +
                                                 " AND VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID());
                                    MRPriceAvPo = Util.GetValueOfDecimal(DB.ExecuteScalar(query.ToString(), null, trxName));
                                }
                                MRPriceLastPO = MRPriceAvPo;
                                #endregion
                            }

                            // update VAM_ProductCost Accumulative Amount to be reduced for Av. PO and Last PO
                            if (cd.GetVAB_OrderLine_ID() == 0)
                            {
                                int costElementId = 0;
                                // when iscostimmediate = true
                                // if mr line cost already calculated then during match PO with MR we have to calculate both Average PO and Last PO
                                if ((optionalstr == "window" && (costingMethodMatchPO == "A" || inoutline.IsCostCalculated())) || optionalstr == "process")
                                {
                                    query.Clear();
                                    query.Append(@"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE  VAF_Client_ID=" + VAF_Client_ID +
                                                 @" AND IsActive='Y' AND CostElementType='M'  AND CostingMethod = 'A'");
                                    costElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));

                                    costElement = new MVAMProductCostElement(ctx, costElementId, null);

                                    #region Av. PO
                                    if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_Organization) //(cl != "B")
                                    {
                                        cost = MVAMProductCost.Get(product, 0, acctSchema, VAF_Org_ID, costElementId);
                                    }
                                    else if (cl == MProductCategory.COSTINGLEVEL_BatchLot || cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch)
                                    {
                                        cost = MVAMProductCost.Get(product, M_ASI_ID, acctSchema, VAF_Org_ID, costElementId);
                                    }
                                    else if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                    {
                                        cost = MVAMProductCost.Get(product, (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID), acctSchema, VAF_Org_ID, costElementId, inout.GetVAM_Warehouse_ID());
                                    }
                                    if ((!isRecordFromForm && inoutline.IsCostCalculated()) || (client.IsCostImmediate()))
                                    {
                                        cost.SetCumulatedAmt(Decimal.Subtract(cost.GetCumulatedAmt(), (MRPriceAvPo * Qty)));
                                    }
                                    if (!cost.Save())
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        _log.Severe("Error occured during saving a record , VAM_Inv_InOutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID() + " in cost detail -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                        return false;
                                    }

                                    cd.SetVAB_OrderLine_ID(orderLineId);
                                    cd.SetQty(Qty);
                                    cd.SetAmt(Decimal.Round(Decimal.Multiply(Price, Qty), acctSchema.GetCostingPrecision()));
                                    if (!cd.Save(trxName))
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        _log.Severe("Error occured during saving a record, VAM_Inv_InOutLine_Id = " + inoutline.GetVAM_Inv_InOutLine_ID() + " in cost detail -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                        return false;
                                    }

                                    cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Add(cd.GetAmt(),
                                                           Decimal.Round(Decimal.Divide(Decimal.Multiply(cd.GetAmt(), costElement.GetSurchargePercentage()), 100), acctSchema.GetCostingPrecision()))));

                                    if ((!client.IsCostImmediate() && (isRecordFromForm || !inout.IsCostCalculated())) ||
                                        (client.IsCostImmediate() && costingMethodMatchPO != "A" && inoutline.IsCostCalculated()))
                                    {
                                        // if cost not calculated for MR then add quantity
                                        cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), cd.GetQty()));
                                        cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), cd.GetQty()));
                                    }

                                    if (cost.GetCumulatedQty() != 0)
                                    {
                                        cost.SetCurrentCostPrice(Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), acctSchema.GetCostingPrecision()));
                                    }
                                    else
                                    {
                                        cost.SetCurrentCostPrice(0);
                                    }
                                    if (!cost.Save())
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        _log.Severe("Error occured during saving a record, VAM_Inv_InOutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID() + " in M-Cost -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                        return false;
                                    }
                                    else
                                    {
                                        if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_Warehouse
                                            || cl == MProductCategory.COSTINGLEVEL_Organization)//(cl != "B")
                                        {
                                            MVAMProductCostElementDetail.CreateCostElementDetail(ctx, VAF_Client_ID, VAF_Org_ID, product, 0,
                                                                 acctSchema, costElementId, windowName, cd, Decimal.Round(Decimal.Multiply(cost.GetCurrentCostPrice(), Qty), acctSchema.GetCostingPrecision()), Qty);
                                        }
                                        else if (cl == MProductCategory.COSTINGLEVEL_BatchLot || cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch
                                            || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                        {
                                            MVAMProductCostElementDetail.CreateCostElementDetail(ctx, VAF_Client_ID, VAF_Org_ID, product, M_ASI_ID,
                                                                     acctSchema, costElementId, windowName, cd, Decimal.Round(Decimal.Multiply(cost.GetCurrentCostPrice(), Qty), acctSchema.GetCostingPrecision()), Qty);
                                        }
                                    }
                                    #endregion

                                }

                                if ((optionalstr == "window" && (costingMethodMatchPO == "p" || inoutline.IsCostCalculated())) || optionalstr == "process")
                                {
                                    #region last po
                                    query.Clear();
                                    query.Append("SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE  VAF_Client_ID=" + VAF_Client_ID + " AND IsActive='Y' AND CostElementType='M'  AND CostingMethod = 'p'");
                                    costElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));

                                    costElement = new MVAMProductCostElement(ctx, costElementId, null);

                                    if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_Organization) //(cl != "B")
                                    {
                                        cost = MVAMProductCost.Get(product, 0, acctSchema, VAF_Org_ID, costElementId);
                                    }
                                    else if (cl == MProductCategory.COSTINGLEVEL_BatchLot || cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch)
                                    {
                                        cost = MVAMProductCost.Get(product, M_ASI_ID, acctSchema, VAF_Org_ID, costElementId);
                                    }
                                    else if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                    {
                                        cost = MVAMProductCost.Get(product, (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID), acctSchema, VAF_Org_ID, costElementId, inout.GetVAM_Warehouse_ID());
                                    }
                                    // reduce 
                                    if ((!isRecordFromForm && inoutline.IsCostCalculated()) || (client.IsCostImmediate()))
                                        cost.SetCumulatedAmt(Decimal.Subtract(cost.GetCumulatedAmt(), (MRPriceLastPO * Qty))); // reduce amount of only MR

                                    //cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), cd.GetAmt())); // add amount of Match PO
                                    cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Add(cd.GetAmt(),
                                                           Decimal.Round(Decimal.Divide(Decimal.Multiply(cd.GetAmt(), costElement.GetSurchargePercentage()), 100), acctSchema.GetCostingPrecision()))));

                                    // if cost immediate = true, costing method not last PO and mr line cost calculation = true then add qty
                                    if ((!client.IsCostImmediate() && (isRecordFromForm || !inout.IsCostCalculated())) ||
                                        (client.IsCostImmediate() && costingMethodMatchPO != "p" && inoutline.IsCostCalculated()))
                                    {
                                        // if cost not calculated for MR then add quantity
                                        cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), cd.GetQty()));
                                        cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), cd.GetQty()));
                                    }

                                    //cost.SetCurrentCostPrice(Decimal.Round(Price, acctSchema.GetCostingPrecision()));
                                    cost.SetCurrentCostPrice(Decimal.Round(Decimal.Add(Price, Decimal.Divide(Decimal.Multiply(Price, costElement.GetSurchargePercentage()), 100)), acctSchema.GetCostingPrecision()));

                                    if (!cost.Save())
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        _log.Severe("Error occured during saving a record, N_InoutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID() + " in M-Cost -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                        return false;
                                    }
                                    else
                                    {
                                        if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_Warehouse
                                            || cl == MProductCategory.COSTINGLEVEL_Organization) //(cl != "B")
                                        {
                                            MVAMProductCostElementDetail.CreateCostElementDetail(ctx, VAF_Client_ID, VAF_Org_ID, product, 0,
                                                                 acctSchema, costElementId, windowName, cd, Decimal.Round(Decimal.Multiply(cost.GetCurrentCostPrice(), Qty), acctSchema.GetCostingPrecision()), Qty);
                                        }
                                        else if (cl == MProductCategory.COSTINGLEVEL_BatchLot || cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch
                                            || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                        {
                                            MVAMProductCostElementDetail.CreateCostElementDetail(ctx, VAF_Client_ID, VAF_Org_ID, product, M_ASI_ID,
                                                                 acctSchema, costElementId, windowName, cd, Decimal.Round(Decimal.Multiply(cost.GetCurrentCostPrice(), Qty), acctSchema.GetCostingPrecision()), Qty);
                                        }
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                cd = new MVAMProductCostDetail(acctSchema, VAF_Org_ID, product.GetVAM_Product_ID(), M_ASI_ID,
                                                      0, Decimal.Round(Decimal.Multiply(Qty, Price), acctSchema.GetCostingPrecision()), Qty, null, trxName);
                                cd.SetVAM_Inv_InOutLine_ID(inoutline.GetVAM_Inv_InOutLine_ID());
                                cd.SetVAB_OrderLine_ID(orderLineId);
                                cd.SetVAM_Warehouse_ID(inout.GetVAM_Warehouse_ID());
                                if (!cd.Save(trxName))
                                {
                                    if (optionalstr != "window")
                                    {
                                        trxName.Rollback();
                                    }
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    _log.Severe("Error occured during saving a record, VAM_Inv_InOutLine_Id = " + inoutline.GetVAM_Inv_InOutLine_ID() + " in Cost-Detail -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                    return false;
                                }

                                int costElementId = 0;
                                if ((optionalstr == "window" && (costingMethodMatchPO == "A" || inoutline.IsCostCalculated())) || optionalstr == "process")
                                {
                                    #region Av. PO
                                    query.Clear();
                                    query.Append("SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE  VAF_Client_ID=" + VAF_Client_ID + " AND IsActive='Y' AND CostElementType='M'  AND CostingMethod = 'A'");
                                    costElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));

                                    costElement = new MVAMProductCostElement(ctx, costElementId, null);

                                    if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_Organization) //(cl != "B")
                                    {
                                        cost = MVAMProductCost.Get(product, 0, acctSchema, VAF_Org_ID, costElementId);
                                    }
                                    else if (cl == MProductCategory.COSTINGLEVEL_BatchLot || cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch)
                                    {
                                        cost = MVAMProductCost.Get(product, M_ASI_ID, acctSchema, VAF_Org_ID, costElementId);
                                    }
                                    else if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                    {
                                        cost = MVAMProductCost.Get(product, (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID), acctSchema, VAF_Org_ID, costElementId, inout.GetVAM_Warehouse_ID());
                                    }
                                    if ((!isRecordFromForm && inoutline.IsCostCalculated()) || (client.IsCostImmediate()))
                                        cost.SetCumulatedAmt(Decimal.Subtract(cost.GetCumulatedAmt(), (MRPriceAvPo * Qty))); // reduce amount of only MR

                                    //cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), cd.GetAmt()));
                                    cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Add(cd.GetAmt(),
                                                          Decimal.Round(Decimal.Divide(Decimal.Multiply(cd.GetAmt(), costElement.GetSurchargePercentage()), 100), acctSchema.GetCostingPrecision()))));

                                    if ((!client.IsCostImmediate() && (isRecordFromForm || !inout.IsCostCalculated())) ||
                                        (client.IsCostImmediate() && costingMethodMatchPO != "A" && inoutline.IsCostCalculated()))
                                    {
                                        // if cost not calculated from MR then add quantity
                                        cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), cd.GetQty()));
                                        cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), cd.GetQty()));
                                    }
                                    if (cost.GetCumulatedQty() != 0)
                                    {
                                        cost.SetCurrentCostPrice(Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), acctSchema.GetCostingPrecision()));
                                    }
                                    else
                                    {
                                        cost.SetCurrentCostPrice(0);
                                    }
                                    if (!cost.Save())
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        _log.Severe("Error occured during saving a record, VAM_Inv_InOutLine_Id = " + inoutline.GetVAM_Inv_InOutLine_ID() + " in M-Cost -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                        return false;
                                    }
                                    else
                                    {
                                        MVAMProductCostElementDetail.CreateCostElementDetail(ctx, VAF_Client_ID, VAF_Org_ID, product, M_ASI_ID,
                                                             acctSchema, costElementId, windowName, cd, Decimal.Round(Decimal.Multiply(cost.GetCurrentCostPrice(), Qty), acctSchema.GetCostingPrecision()), Qty);
                                    }
                                    #endregion
                                }
                                if ((optionalstr == "window" && (costingMethodMatchPO == "p" || inoutline.IsCostCalculated())) || optionalstr == "process")
                                {
                                    #region last po
                                    query.Clear();
                                    query.Append("SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE  VAF_Client_ID=" + VAF_Client_ID + " AND IsActive='Y' AND CostElementType='M'  AND CostingMethod = 'p'");
                                    costElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));

                                    costElement = new MVAMProductCostElement(ctx, costElementId, null);

                                    if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_Organization) //(cl != "B")
                                    {
                                        cost = MVAMProductCost.Get(product, 0, acctSchema, VAF_Org_ID, costElementId);
                                    }
                                    else if (cl == MProductCategory.COSTINGLEVEL_BatchLot || cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch)
                                    {
                                        cost = MVAMProductCost.Get(product, M_ASI_ID, acctSchema, VAF_Org_ID, costElementId);
                                    }
                                    else if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                    {
                                        cost = MVAMProductCost.Get(product, (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID), acctSchema, VAF_Org_ID, costElementId, inout.GetVAM_Warehouse_ID());
                                    }
                                    if ((!isRecordFromForm && inoutline.IsCostCalculated()) || (client.IsCostImmediate()))
                                        cost.SetCumulatedAmt(Decimal.Subtract(cost.GetCumulatedAmt(), (MRPriceLastPO * Qty))); // reduce amount of only MR

                                    //cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), cd.GetAmt()));
                                    cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Add(cd.GetAmt(),
                                                           Decimal.Round(Decimal.Divide(Decimal.Multiply(cd.GetAmt(), costElement.GetSurchargePercentage()), 100), acctSchema.GetCostingPrecision()))));

                                    if ((!client.IsCostImmediate() && (isRecordFromForm || !inout.IsCostCalculated())) ||
                                        (client.IsCostImmediate() && costingMethodMatchPO != "p" && inoutline.IsCostCalculated()))
                                    {
                                        // if cost not calculated for MR then add quantity
                                        cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), cd.GetQty()));
                                        cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), cd.GetQty()));
                                    }

                                    //cost.SetCurrentCostPrice(Decimal.Round(Price, acctSchema.GetCostingPrecision()));
                                    cost.SetCurrentCostPrice(Decimal.Round(Decimal.Add(Price, Decimal.Divide(Decimal.Multiply(Price, costElement.GetSurchargePercentage()), 100)), acctSchema.GetCostingPrecision()));

                                    if (!cost.Save())
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        _log.Severe("Error occured during saving a record, VAM_Inv_InOutLine_Id = " + inoutline.GetVAM_Inv_InOutLine_ID() + " in M-Cost -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                        return false;
                                    }
                                    else
                                    {
                                        MVAMProductCostElementDetail.CreateCostElementDetail(ctx, VAF_Client_ID, VAF_Org_ID, product, M_ASI_ID,
                                                             acctSchema, costElementId, windowName, cd, Decimal.Round(Decimal.Multiply(cost.GetCurrentCostPrice(), Qty), acctSchema.GetCostingPrecision()), Qty);
                                    }
                                    #endregion
                                }
                            }

                            #endregion
                        }

                        if (windowName == "Match IV")
                        {
                            #region Match Invoice
                            decimal MRPrice = 0;
                            decimal MRPriceLifo = 0;
                            decimal MRPriceFifo = 0;
                            decimal matchInvoicePrice = 0;
                            int ceFifo = 0;
                            int ceLifo = 0;

                            pca = MProductCategory.Get(product.GetCtx(), product.GetVAM_ProductCategory_ID());
                            if (pca != null)
                            {
                                cl = pca.GetCostingLevel();
                            }
                            else
                            {
                                cl = acctSchema.GetCostingLevel();
                            }
                            query.Clear();
                            query.Append("SELECT MIN(VAM_ProductCostDetail_ID) FROM VAM_ProductCostDetail WHERE VAM_Inv_InOutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID() +
                                    " AND VAB_Orderline_id IS NULL  AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID());
                            costDetailId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));
                            cd = new MVAMProductCostDetail(ctx, costDetailId, trxName);

                            query.Clear();
                            query.Append(@"SELECT VAB_OrderLine_Id FROM VAM_MatchPO WHERE IsActive = 'Y' AND VAM_MatchPO_ID = 
                                     (SELECT MIN(VAM_MatchPO_ID) FROM VAM_MatchPO WHERE IsActive = 'Y' AND VAM_Inv_InOutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID() + ") ");
                            MatchPO_OrderLineId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString()));

                            ceFifo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAM_ProductCostElement_ID from VAM_ProductCostElement where costingMethod = 'F' AND IsActive = 'Y' AND VAF_Client_ID = " + inoutline.GetVAF_Client_ID()));
                            ceLifo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAM_ProductCostElement_ID from VAM_ProductCostElement where costingMethod = 'L' AND IsActive = 'Y' AND VAF_Client_ID = " + inoutline.GetVAF_Client_ID()));

                            #region MR Price against LIFO / FIFO
                            //                            if (cl != "B")
                            //                            {
                            //                                query.Clear();
                            //                                query.Append(@"SELECT  Amt/Qty as currentCostAmount  FROM VAM_ProductCostElementDetail ced 
                            //                                    where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @" 
                            //                                     AND ced.VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() + @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) = 0 " +
                            //                                     " AND ced.VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) = " + MatchPO_OrderLineId + 
                            //                                     @" AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 and 
                            //                                     ced.VAM_ProductCostElement_ID = (SELECT VAM_ProductCostElement_ID from VAM_ProductCostElement where costingMethod = 'L' AND IsActive = 'Y'
                            //                                     AND VAF_Client_ID = " + inoutline.GetVAF_Client_ID() + " ) AND ced.VAF_Client_ID = " + inoutline.GetVAF_Client_ID());
                            //                            }
                            //                            else
                            //                            {
                            //                                query.Clear();
                            //                                query.Append(@"SELECT  Amt/Qty as currentCostAmount  FROM VAM_ProductCostElementDetail ced 
                            //                                    where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @" 
                            //                                     AND ced.VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() + @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) = " + M_ASI_ID + @" 
                            //                                     AND ced.VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) = " + MatchPO_OrderLineId + 
                            //                                     @" AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 and 
                            //                                     ced.VAM_ProductCostElement_ID = (SELECT VAM_ProductCostElement_ID from VAM_ProductCostElement where costingMethod = 'L' AND IsActive = 'Y' 
                            //                                     AND VAF_Client_ID = " + inoutline.GetVAF_Client_ID() + " ) AND ced.VAF_Client_ID = " + inoutline.GetVAF_Client_ID());
                            //                            }
                            //                            MRPriceLifo = Util.GetValueOfDecimal(DB.ExecuteScalar(query.ToString()));
                            if (MRPriceLifo == 0)
                            {
                                if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_Warehouse
                                            || cl == MProductCategory.COSTINGLEVEL_Organization) //(cl != "B")
                                {
                                    query.Clear();
                                    query.Append(@"SELECT  Amt as currentCostAmount  FROM VAT_Temp_CostDetail ced INNER JOIN VAM_ProductCostQueue cq ON cq.VAM_ProductCostQueue_id = ced.VAM_ProductCostQueue_id 
                                    where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @" AND cq.VAM_ProductCostElement_ID = " + ceLifo + @" 
                                     AND ced.VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() + @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) = 0 " +
                                         " AND ced.VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) = " + MatchPO_OrderLineId +
                                         @" AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 AND ced.VAF_Client_ID = " + inoutline.GetVAF_Client_ID());
                                }
                                else if (cl == MProductCategory.COSTINGLEVEL_BatchLot || cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch
                                            || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                {
                                    query.Clear();
                                    query.Append(@"SELECT  Amt as currentCostAmount  FROM VAT_Temp_CostDetail ced INNER JOIN VAM_ProductCostQueue cq ON cq.VAM_ProductCostQueue_id = ced.VAM_ProductCostQueue_id 
                                    where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @" AND cq.VAM_ProductCostElement_ID = " + ceLifo + @" 
                                     AND ced.VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() + @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) = " + M_ASI_ID + @" 
                                     AND ced.VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) = " + MatchPO_OrderLineId +
                                     @" AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 AND ced.VAF_Client_ID = " + inoutline.GetVAF_Client_ID());
                                }
                                MRPriceLifo = Util.GetValueOfDecimal(DB.ExecuteScalar(query.ToString()));
                                if (MRPriceLifo == 0)
                                {
                                    // this case happen when 
                                    // if orderline id available on MR 
                                    // but temp table doesnt contain orderline id ref
                                    // when we match PO and MR through form before calculating cost of inv
                                    // costing method is other than Average PO and Last PO
                                    if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_Warehouse
                                            || cl == MProductCategory.COSTINGLEVEL_Organization) //(cl != "B")
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT  Amt as currentCostAmount  FROM VAT_Temp_CostDetail ced INNER JOIN VAM_ProductCostQueue cq ON cq.VAM_ProductCostQueue_id = ced.VAM_ProductCostQueue_id 
                                    where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @" AND cq.VAM_ProductCostElement_ID = " + ceLifo + @" 
                                     AND ced.VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() + @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) = 0 " +
                                             " AND ced.VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) = 0 " +
                                             @" AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 AND ced.VAF_Client_ID = " + inoutline.GetVAF_Client_ID());
                                    }
                                    else if (cl == MProductCategory.COSTINGLEVEL_BatchLot || cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch
                                            || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT  Amt as currentCostAmount  FROM VAT_Temp_CostDetail ced INNER JOIN VAM_ProductCostQueue cq ON cq.VAM_ProductCostQueue_id = ced.VAM_ProductCostQueue_id 
                                    where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @" AND cq.VAM_ProductCostElement_ID = " + ceLifo + @" 
                                     AND ced.VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() + @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) = " + M_ASI_ID + @" 
                                     AND ced.VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) = 0 " +
                                         @" AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 AND ced.VAF_Client_ID = " + inoutline.GetVAF_Client_ID());
                                    }
                                    MRPriceLifo = Util.GetValueOfDecimal(DB.ExecuteScalar(query.ToString()));
                                }
                            }


                            //                            if (cl != "B")
                            //                            {
                            //                                query.Clear();
                            //                                query.Append(@"SELECT  Amt/Qty as currentCostAmount  FROM VAM_ProductCostElementDetail ced 
                            //                                    where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @" 
                            //                                     AND ced.VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() + @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) = 0 " +
                            //                                     " AND ced.VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) = " + MatchPO_OrderLineId + @" AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 and 
                            //                                     ced.VAM_ProductCostElement_ID = (SELECT VAM_ProductCostElement_ID from VAM_ProductCostElement where costingMethod = 'F' AND IsActive = 'Y' AND VAF_Client_ID = " + inoutline.GetVAF_Client_ID() + " ) AND ced.VAF_Client_ID = " + inoutline.GetVAF_Client_ID());
                            //                            }
                            //                            else
                            //                            {
                            //                                query.Clear();
                            //                                query.Append(@"SELECT  Amt/Qty as currentCostAmount  FROM VAM_ProductCostElementDetail ced 
                            //                                    where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @" 
                            //                                     AND ced.VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() + @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) = " + M_ASI_ID + @" 
                            //                                     AND ced.VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) = " + MatchPO_OrderLineId + @" AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 and 
                            //                                     ced.VAM_ProductCostElement_ID = (SELECT VAM_ProductCostElement_ID from VAM_ProductCostElement where costingMethod = 'F' AND IsActive = 'Y' AND VAF_Client_ID = " + inoutline.GetVAF_Client_ID() + " ) AND ced.VAF_Client_ID = " + inoutline.GetVAF_Client_ID());
                            //                            }
                            //                            MRPriceFifo = Util.GetValueOfDecimal(DB.ExecuteScalar(query.ToString()));
                            if (MRPriceFifo == 0)
                            {
                                if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_Warehouse
                                            || cl == MProductCategory.COSTINGLEVEL_Organization) //(cl != "B")
                                {
                                    query.Clear();
                                    query.Append(@"SELECT  Amt as currentCostAmount  FROM VAT_Temp_CostDetail ced INNER JOIN VAM_ProductCostQueue cq ON cq.VAM_ProductCostQueue_id = ced.VAM_ProductCostQueue_id 
                                    where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @" AND cq.VAM_ProductCostElement_ID = " + ceFifo + @" 
                                     AND ced.VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() + @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) = 0 " +
                                         " AND ced.VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) = " + MatchPO_OrderLineId +
                                         @" AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 AND ced.VAF_Client_ID = " + inoutline.GetVAF_Client_ID());
                                }
                                else if (cl == MProductCategory.COSTINGLEVEL_BatchLot || cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch
                                            || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                {
                                    query.Clear();
                                    query.Append(@"SELECT  Amt as currentCostAmount  FROM VAT_Temp_CostDetail ced INNER JOIN VAM_ProductCostQueue cq ON cq.VAM_ProductCostQueue_id = ced.VAM_ProductCostQueue_id 
                                    where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @" AND cq.VAM_ProductCostElement_ID = " + ceFifo + @" 
                                     AND ced.VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() + @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) = " + M_ASI_ID + @" 
                                     AND ced.VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) = " + MatchPO_OrderLineId +
                                     @" AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 AND ced.VAF_Client_ID = " + inoutline.GetVAF_Client_ID());
                                }
                                MRPriceFifo = Util.GetValueOfDecimal(DB.ExecuteScalar(query.ToString()));
                                if (MRPriceFifo == 0)
                                {
                                    // this case happen when 
                                    // if orderline id available on MR 
                                    // but temp table doesnt contain orderline id ref
                                    // when we match PO and MR through form before calculating cost of inv
                                    // costing method is other than Average PO and Last PO
                                    if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_Warehouse
                                            || cl == MProductCategory.COSTINGLEVEL_Organization) //(cl != "B")
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT  Amt as currentCostAmount  FROM VAT_Temp_CostDetail ced INNER JOIN VAM_ProductCostQueue cq ON cq.VAM_ProductCostQueue_id = ced.VAM_ProductCostQueue_id 
                                    where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @" AND cq.VAM_ProductCostElement_ID = " + ceFifo + @" 
                                     AND ced.VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() + @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) = 0 " +
                                             " AND ced.VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) = 0 " +
                                             @" AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 AND ced.VAF_Client_ID = " + inoutline.GetVAF_Client_ID());
                                    }
                                    else if (cl == MProductCategory.COSTINGLEVEL_BatchLot || cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch
                                            || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT  Amt as currentCostAmount  FROM VAT_Temp_CostDetail ced INNER JOIN VAM_ProductCostQueue cq ON cq.VAM_ProductCostQueue_id = ced.VAM_ProductCostQueue_id 
                                    where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @" AND cq.VAM_ProductCostElement_ID = " + ceFifo + @" 
                                     AND ced.VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() + @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) = " + M_ASI_ID + @" 
                                     AND ced.VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) = 0 " +
                                         @" AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 AND ced.VAF_Client_ID = " + inoutline.GetVAF_Client_ID());
                                    }
                                    MRPriceFifo = Util.GetValueOfDecimal(DB.ExecuteScalar(query.ToString()));
                                }
                            }
                            #endregion

                            // not execute this if section
                            if (costDetailId > 0 && cd.GetVAB_InvoiceLine_ID() == 0 && handlingWindowName != "Match IV" && handlingWindowName != "Invoice(Vendor)")
                            {
                                // set amount and qty on cost detail from match receipt
                                cd.SetAmt(Price);
                                cd.SetQty(Qty);
                                cd.SetVAB_InvoiceLine_ID(invoiceline.GetVAB_InvoiceLine_ID());
                                cd.SetVAB_OrderLine_ID(invoiceline.GetVAB_OrderLine_ID());
                                if (!cd.Save(trxName))
                                {
                                    if (optionalstr != "window")
                                    {
                                        trxName.Rollback();
                                    }
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    _log.Severe("Error occured during saving a record in M-Cost -> Match IV. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                    return false;
                                }

                                //get Cost Queue Record based on condition
                                query.Clear();
                                query.Append(@"SELECT * FROM VAT_Temp_CostDetail WHERE  VAM_Inv_InOutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID() +
                                              " AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID());
                                DataSet ds2 = DB.ExecuteDataset(query.ToString(), null, null);

                                // reduce cummulative amount , qty for all element
                                cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, invoiceline.GetVAF_Org_ID(), optionalStrCd: optionalstr);

                                if (handlingWindowName == "Invoice(Vendor)")
                                {
                                    #region handlingWindowName = Invoice(Vendor)
                                    if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                    {
                                        for (int b = 0; b < ds2.Tables[0].Rows.Count; b++)
                                        {
                                            if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostQueue WHERE VAM_ProductCostQueue_ID = " + (Util.GetValueOfInt(ds2.Tables[0].Rows[b]["VAM_ProductCostQueue_ID"])))) == ceFifo)
                                            {
                                                MRPrice = MRPriceFifo;
                                                costElement = new MVAMProductCostElement(ctx, ceFifo, null);
                                            }
                                            else
                                            {
                                                MRPrice = MRPriceLifo;
                                                costElement = new MVAMProductCostElement(ctx, ceLifo, null);
                                            }
                                            price = MVAMProductCostQueue.CalculateCostQueuePrice(invoice, invoiceline, MRPrice, acctSchema, VAF_Client_ID, VAF_Org_ID, costElement, trxName, client.IsCostImmediate(), optionalstr, matchInoutLine);
                                            if (price == 0)
                                            {
                                                if (optionalstr != "window")
                                                {
                                                    trxName.Rollback();
                                                }
                                                conversionNotFound = invoice.GetDocumentNo();
                                                return false;
                                            }
                                            query.Clear();
                                            query.Append("UPDATE VAM_ProductCostQueue SET CurrentCostPrice = " + price + " WHERE VAM_ProductCostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[b]["VAM_ProductCostQueue_ID"]));
                                            //query.Append("UPDATE VAM_ProductCostQueue SET CurrentCostPrice = " + Decimal.Round(Decimal.Add(price, Decimal.Divide(Decimal.Multiply(price, costElement.GetSurchargePercentage()), 100)), acctSchema.GetCostingPrecision()) + " WHERE VAM_ProductCostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[b]["VAM_ProductCostQueue_ID"]));
                                            DB.ExecuteQuery(query.ToString(), null, trxName);
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region Calculate amount of matched Invoice
                                    query.Clear();
                                    query.Append(@"SELECT il.VAB_InvoiceLine_id , il.VAB_Invoice_id , ROUND(il.linenetamt/il.qtyinvoiced , 4) AS priceactual, 
                                                    CASE
                                                    WHEN NVL(C_SurChargeTax.IsIncludeInCost , 'N') = 'Y'
                                                    AND NVL(VAB_TaxRate.IsIncludeInCost , 'N')           = 'Y'
                                                    THEN ROUND((il.taxbaseamt + il.taxamt + il.surchargeamt) / il.qtyinvoiced , 4)
                                                    WHEN NVL(C_SurChargeTax.IsIncludeInCost , 'N') = 'N'
                                                    AND NVL(VAB_TaxRate.IsIncludeInCost , 'N')           = 'Y'
                                                    THEN ROUND((il.taxbaseamt + il.taxamt) / il.qtyinvoiced , 4)
                                                    WHEN NVL(C_SurChargeTax.IsIncludeInCost , 'N') = 'Y'
                                                    AND NVL(VAB_TaxRate.IsIncludeInCost , 'N')           = 'N'
                                                    THEN ROUND((il.taxbaseamt + il.surchargeamt) / il.qtyinvoiced, 4)
                                                    ELSE ROUND(il.taxbaseamt  / il.qtyinvoiced, 4)
                                                  END AS PriceActualIncludedTax
                                                    , mi.qty ,  SUM(mi.qty) AS matchedqty 
                                          FROM VAM_Inv_InOut io INNER JOIN VAM_Inv_InOutLine iol ON io.VAM_Inv_InOut_id = iol.VAM_Inv_InOut_id
                                          INNER JOIN VAM_MatchInvoice mi ON iol.VAM_Inv_InOutLine_id = mi.VAM_Inv_InOutLine_id
                                          INNER JOIN VAB_InvoiceLine il ON il.VAB_InvoiceLine_id = mi.VAB_InvoiceLine_id
                                          INNER JOIN VAB_TaxRate VAB_TaxRate ON VAB_TaxRate.VAB_TaxRate_ID = il.VAB_TaxRate_ID 
                                          LEFT JOIN VAB_TaxRate C_SurChargeTax ON VAB_TaxRate.Surcharge_Tax_ID = C_SurChargeTax.VAB_TaxRate_ID
                                          WHERE mi.isactive      = 'Y' AND io.VAM_Inv_InOut_ID      =" + inoutline.GetVAM_Inv_InOut_ID() +
                                          " AND iol.VAM_Inv_InOutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID() +
                                         @" GROUP BY  il.VAB_InvoiceLine_id , il.VAB_Invoice_id , ROUND(il.linenetamt/il.qtyinvoiced , 4) , mi.qty , 
                                            il.taxbaseamt ,  il.taxamt , il.surchargeamt , C_SurChargeTax.IsIncludeInCost , VAB_TaxRate.IsIncludeInCost, il.qtyinvoiced ");
                                    ds2 = DB.ExecuteDataset(query.ToString(), null, trxName);
                                    if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                    {
                                        Price = 0;
                                        Qty = 0;
                                        MVABInvoice inv = null;
                                        Decimal PriceActualIncludedTax = 0;
                                        for (int n = 0; n < ds2.Tables[0].Rows.Count; n++)
                                        {
                                            PriceActualIncludedTax = Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["PriceActualIncludedTax"]) != 0 ?
                                            Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["PriceActualIncludedTax"]) :
                                            Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["priceactual"]);

                                            Qty += Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["matchedqty"]);
                                            inv = new MVABInvoice(ctx, Util.GetValueOfInt(ds2.Tables[0].Rows[n]["VAB_Invoice_id"]), trxName);
                                            if (inv.GetVAB_Currency_ID() != acctSchema.GetVAB_Currency_ID())
                                            {
                                                Price += MVABExchangeRate.Convert(ctx, decimal.Multiply(PriceActualIncludedTax, Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["qty"])),
                                                                                 inv.GetVAB_Currency_ID(), acctSchema.GetVAB_Currency_ID(),
                                                                                 inv.GetDateAcct(), inv.GetVAB_CurrencyType_ID(), VAF_Client_ID, VAF_Org_ID);
                                                if (Price == 0)
                                                {
                                                    if (optionalstr != "window")
                                                    {
                                                        trxName.Rollback();
                                                    }
                                                    conversionNotFound = inv.GetDocumentNo();
                                                    return false;
                                                }
                                            }
                                            else
                                            {
                                                Price += decimal.Multiply(PriceActualIncludedTax, Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["qty"]));
                                            }
                                        }
                                    }
                                    // temp qty = those qty which are not matched
                                    decimal tempQty = Decimal.Subtract(inoutline.GetMovementQty(), Qty);
                                    if (Qty != inoutline.GetMovementQty())
                                    {
                                        Qty += (inoutline.GetMovementQty() - Qty);
                                    }

                                    // update cost queue with invoice amount
                                    query.Clear();
                                    query.Append(@"SELECT * FROM VAT_Temp_CostDetail WHERE  VAM_Inv_InOutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID() +
                                              " AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID());
                                    ds2 = DB.ExecuteDataset(query.ToString(), null, null);
                                    if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                    {
                                        matchInvoicePrice = Price;
                                        for (int k = 0; k < ds2.Tables[0].Rows.Count; k++)
                                        {
                                            Price = matchInvoicePrice;
                                            //change 4-5-2016
                                            if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostQueue WHERE VAM_ProductCostQueue_ID = " + (Util.GetValueOfInt(ds2.Tables[0].Rows[k]["VAM_ProductCostQueue_id"])))) == ceFifo)
                                            {
                                                MRPrice = MRPriceFifo;
                                                costElement = new MVAMProductCostElement(ctx, ceFifo, null);
                                            }
                                            else
                                            {
                                                MRPrice = MRPriceLifo;
                                                costElement = new MVAMProductCostElement(ctx, ceLifo, null);
                                            }
                                            //24-Aug-2016
                                            Price = Decimal.Round(Decimal.Add(Price, Decimal.Divide(Decimal.Multiply(Price, costElement.GetSurchargePercentage()), 100)), acctSchema.GetCostingPrecision());
                                            //end
                                            Price = Decimal.Add(Price, Decimal.Multiply(MRPrice, tempQty));
                                            //end
                                            query.Clear();
                                            query.Append("UPDATE VAM_ProductCostQueue SET CurrentCostPrice = " + Decimal.Round((Price / Qty), acctSchema.GetCostingPrecision()) + " WHERE VAM_ProductCostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[k]["VAM_ProductCostQueue_ID"]));
                                            //query.Append("UPDATE VAM_ProductCostQueue SET CurrentCostPrice = " + Decimal.Round(Decimal.Add((Price / Qty), (((Price / Qty) * costElement.GetSurchargePercentage()) / 100)), acctSchema.GetCostingPrecision()) + " WHERE VAM_ProductCostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[k]["VAM_ProductCostQueue_ID"]));
                                            DB.ExecuteQuery(query.ToString(), null, trxName);
                                        }
                                    }
                                    ds2.Dispose();
                                    #endregion
                                }

                                // update VAM_ProductCost with accumulation qty , amt , and current cost
                                if (isMatchFromForm == "Y" || handlingWindowName == "Match IV")
                                {
                                    result = cd.UpdateProductCost("Product Cost IV Form", cd, acctSchema, product, M_ASI_ID, invoiceline.GetVAF_Org_ID(), optionalStrCd: optionalstr);
                                }
                                else
                                {
                                    result = cd.UpdateProductCost("Product Cost IV", cd, acctSchema, product, M_ASI_ID, invoiceline.GetVAF_Org_ID(), optionalStrCd: optionalstr);
                                }
                                if (!result)
                                {
                                    if (optionalstr != "window")
                                    {
                                        trxName.Rollback();
                                    }
                                    _log.Severe("Error occured during saving a record in M-Cost -> Match IV");
                                    return false;
                                }
                            }
                            else
                            {
                                Decimal ProductLineCost = invoiceline.GetProductLineCost(invoiceline);

                                invoice = new MVABInvoice(ctx, invoiceline.GetVAB_Invoice_ID(), trxName);
                                if (invoice.GetVAB_Currency_ID() != acctSchema.GetVAB_Currency_ID())
                                {
                                    Price = MVABExchangeRate.Convert(ctx, Decimal.Multiply(Qty, Decimal.Round(Decimal.Divide(ProductLineCost, invoiceline.GetQtyInvoiced()), 2)), invoice.GetVAB_Currency_ID(), acctSchema.GetVAB_Currency_ID(),
                                                                             invoice.GetDateAcct(), invoice.GetVAB_CurrencyType_ID(), VAF_Client_ID, VAF_Org_ID);
                                    if (Price == 0)
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        conversionNotFound = invoice.GetDocumentNo();
                                        return false;
                                    }
                                }

                                cd = new MVAMProductCostDetail(acctSchema, VAF_Org_ID, product.GetVAM_Product_ID(), M_ASI_ID,
                                                         0, Price, Qty, null, trxName);
                                cd.SetVAB_InvoiceLine_ID(invoiceline.GetVAB_InvoiceLine_ID());
                                cd.SetVAB_OrderLine_ID(invoiceline.GetVAB_OrderLine_ID());
                                cd.SetVAM_Inv_InOutLine_ID(inoutline.GetVAM_Inv_InOutLine_ID());
                                cd.SetVAM_Warehouse_ID(inoutline.GetVAM_Warehouse_ID()); // get warehouse reference from header
                                if (!cd.Save(trxName))
                                {
                                    if (optionalstr != "window")
                                    {
                                        trxName.Rollback();
                                    }
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    _log.Severe("Error occured during saving a record, VAB_InvoiceLine_ID = " + invoiceline.GetVAB_InvoiceLine_ID() + " in cost detail -> Match IV. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                    return false;
                                }
                                else
                                {
                                    // reduce cummulative amount for all element
                                    result = cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, invoiceline.GetVAF_Org_ID(), optionalStrCd: optionalstr);
                                    if (!result)
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        _log.Severe("Error occured during saving a record , VAB_InvoiceLine_ID = " + invoiceline.GetVAB_InvoiceLine_ID() + " in M-Cost -> Match IV");
                                        return false;
                                    }

                                    // update cost queue with invoice amount
                                    query.Clear();
                                    query.Append(@"SELECT * FROM VAT_Temp_CostDetail WHERE  VAM_Inv_InOutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID() +
                                              " AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID());
                                    DataSet ds2 = DB.ExecuteDataset(query.ToString(), null, null);

                                    if (handlingWindowName == "Invoice(Vendor)")
                                    {
                                        #region handlingWindowName == Invoice(Vendor)
                                        price = 0;
                                        int costElementID = 0;
                                        if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                        {
                                            for (int b = 0; b < ds2.Tables[0].Rows.Count; b++)
                                            {
                                                //change 4-5-2016
                                                costElementID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostQueue WHERE VAM_ProductCostQueue_ID = " + (Util.GetValueOfInt(ds2.Tables[0].Rows[b]["VAM_ProductCostQueue_ID"]))));
                                                if (costElementID == ceFifo)
                                                {
                                                    MRPrice = MRPriceFifo;
                                                    costElement = new MVAMProductCostElement(ctx, ceFifo, null);
                                                }
                                                else
                                                {
                                                    MRPrice = MRPriceLifo;
                                                    costElement = new MVAMProductCostElement(ctx, ceLifo, null);
                                                }
                                                //end
                                                price = 0;
                                                price = MVAMProductCostQueue.CalculateCostQueuePrice(invoice, invoiceline, MRPrice, acctSchema, VAF_Client_ID, VAF_Org_ID, costElement, trxName, client.IsCostImmediate(), optionalstr, matchInoutLine);
                                                if (price == 0 && costElementID != 0)
                                                {
                                                    if (optionalstr != "window")
                                                    {
                                                        trxName.Rollback();
                                                    }
                                                    conversionNotFound = invoice.GetDocumentNo();
                                                    return false;
                                                }
                                                query.Clear();
                                                query.Append("UPDATE VAM_ProductCostQueue SET CurrentCostPrice = " + price + " WHERE VAM_ProductCostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[b]["VAM_ProductCostQueue_ID"]));
                                                //query.Append("UPDATE VAM_ProductCostQueue SET CurrentCostPrice = " + Decimal.Round(Decimal.Add(price, Decimal.Divide(Decimal.Multiply(price, costElement.GetSurchargePercentage()), 100)), acctSchema.GetCostingPrecision()) + " WHERE VAM_ProductCostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[b]["VAM_ProductCostQueue_ID"]));
                                                DB.ExecuteQuery(query.ToString(), null, trxName);
                                            }
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region Calculate amount of matched Invoice
                                        query.Clear();
                                        query.Append(@"SELECT il.VAB_InvoiceLine_id , il.VAB_Invoice_id , ROUND(il.linenetamt/il.qtyinvoiced , 4) AS priceactual , 
                                             CASE WHEN NVL(C_SurChargeTax.IsIncludeInCost , 'N') = 'Y'
                                                AND NVL(VAB_TaxRate.IsIncludeInCost , 'N')           = 'Y'
                                                THEN ROUND((il.taxbaseamt + il.taxamt + il.surchargeamt) / il.qtyinvoiced , 4)
                                                WHEN NVL(C_SurChargeTax.IsIncludeInCost , 'N') = 'N'
                                                AND NVL(VAB_TaxRate.IsIncludeInCost , 'N')           = 'Y'
                                                THEN ROUND((il.taxbaseamt + il.taxamt) / il.qtyinvoiced , 4)
                                                WHEN NVL(C_SurChargeTax.IsIncludeInCost , 'N') = 'Y'
                                                AND NVL(VAB_TaxRate.IsIncludeInCost , 'N')           = 'N'
                                                THEN ROUND((il.taxbaseamt + il.surchargeamt) / il.qtyinvoiced, 4)
                                                ELSE ROUND(il.taxbaseamt  / il.qtyinvoiced, 4)
                                              END AS PriceActualIncludedTax,    
                                            mi.qty ,  SUM(mi.qty) AS matchedqty 
                                          FROM VAM_Inv_InOut io INNER JOIN VAM_Inv_InOutLine iol ON io.VAM_Inv_InOut_id = iol.VAM_Inv_InOut_id
                                          INNER JOIN VAM_MatchInvoice mi ON iol.VAM_Inv_InOutLine_id = mi.VAM_Inv_InOutLine_id
                                          INNER JOIN VAB_InvoiceLine il ON il.VAB_InvoiceLine_id = mi.VAB_InvoiceLine_id
                                          INNER JOIN VAB_TaxRate VAB_TaxRate ON VAB_TaxRate.VAB_TaxRate_ID = il.VAB_TaxRate_ID 
                                          LEFT JOIN VAB_TaxRate C_SurChargeTax ON VAB_TaxRate.Surcharge_Tax_ID = C_SurChargeTax.VAB_TaxRate_ID
                                          WHERE mi.isactive      = 'Y' AND io.VAM_Inv_InOut_ID      =" + inoutline.GetVAM_Inv_InOut_ID() +
                                                     " AND iol.VAM_Inv_InOutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID() +
                                                      @" GROUP BY  il.VAB_InvoiceLine_id , il.VAB_Invoice_id , ROUND(il.linenetamt/il.qtyinvoiced , 4) , mi.qty , 
                                          il.taxbaseamt ,  il.taxamt , il.surchargeamt , C_SurChargeTax.IsIncludeInCost , VAB_TaxRate.IsIncludeInCost , il.qtyinvoiced ");
                                        ds2 = DB.ExecuteDataset(query.ToString(), null, trxName);
                                        if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                        {
                                            Price = 0;
                                            Qty = 0; ;
                                            MVABInvoice inv = null;
                                            Decimal PriceActualIncludedTax = 0;
                                            for (int n = 0; n < ds2.Tables[0].Rows.Count; n++)
                                            {
                                                PriceActualIncludedTax = Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["PriceActualIncludedTax"]) != 0 ?
                                                    Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["PriceActualIncludedTax"]) :
                                                    Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["priceactual"]);

                                                Qty += Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["matchedqty"]);
                                                inv = new MVABInvoice(ctx, Util.GetValueOfInt(ds2.Tables[0].Rows[n]["VAB_Invoice_id"]), trxName);
                                                if (inv.GetVAB_Currency_ID() != acctSchema.GetVAB_Currency_ID())
                                                {
                                                    Price += MVABExchangeRate.Convert(ctx, decimal.Multiply(PriceActualIncludedTax, Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["qty"])),
                                                                                     inv.GetVAB_Currency_ID(), acctSchema.GetVAB_Currency_ID(),
                                                                                     inv.GetDateAcct(), inv.GetVAB_CurrencyType_ID(), VAF_Client_ID, VAF_Org_ID);
                                                    if (Price == 0)
                                                    {
                                                        if (optionalstr != "window")
                                                        {
                                                            trxName.Rollback();
                                                        }
                                                        conversionNotFound = inv.GetDocumentNo();
                                                        return false;
                                                    }
                                                }
                                                else
                                                {
                                                    Price += decimal.Multiply(PriceActualIncludedTax, Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["qty"]));
                                                }
                                            }
                                        }
                                        // temp qty = those qty which are not matched
                                        decimal tempQty = Decimal.Subtract(inoutline.GetMovementQty(), Qty);
                                        if (Qty != inoutline.GetMovementQty())
                                        {
                                            Qty += (inoutline.GetMovementQty() - Qty);
                                        }

                                        // update cost queue with invoice amount
                                        query.Clear();
                                        query.Append(@"SELECT * FROM VAT_Temp_CostDetail WHERE  VAM_Inv_InOutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID() +
                                                  " AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID());

                                        ds2 = DB.ExecuteDataset(query.ToString(), null, null);
                                        if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                        {
                                            matchInvoicePrice = Price;
                                            for (int k = 0; k < ds2.Tables[0].Rows.Count; k++)
                                            {
                                                Price = matchInvoicePrice;
                                                //change 4-5-2016
                                                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostQueue WHERE VAM_ProductCostQueue_ID = " + (Util.GetValueOfInt(ds2.Tables[0].Rows[k]["VAM_ProductCostQueue_ID"])))) == ceFifo)
                                                {
                                                    MRPrice = MRPriceFifo;
                                                    costElement = new MVAMProductCostElement(ctx, ceFifo, null);
                                                }
                                                else
                                                {
                                                    MRPrice = MRPriceLifo;
                                                    costElement = new MVAMProductCostElement(ctx, ceLifo, null);
                                                }
                                                //24-Aug-2016
                                                Price = Decimal.Round(Decimal.Add(Price, Decimal.Divide(Decimal.Multiply(Price, costElement.GetSurchargePercentage()), 100)), acctSchema.GetCostingPrecision());
                                                //end
                                                Price = Decimal.Add(Price, Decimal.Multiply(MRPrice, tempQty));
                                                //end
                                                query.Clear();
                                                query.Append("UPDATE VAM_ProductCostQueue SET CurrentCostPrice = " + Decimal.Round(Price / Qty, acctSchema.GetCostingPrecision()) + " WHERE VAM_ProductCostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[k]["VAM_ProductCostQueue_ID"]));
                                                //query.Append("UPDATE VAM_ProductCostQueue SET CurrentCostPrice = " + Decimal.Round(Decimal.Add((Price / Qty), (((Price / Qty) * costElement.GetSurchargePercentage()) / 100)), acctSchema.GetCostingPrecision()) + " WHERE VAM_ProductCostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[k]["VAM_ProductCostQueue_ID"]));
                                                DB.ExecuteQuery(query.ToString(), null, trxName);
                                            }
                                        }
                                        ds2.Dispose();
                                        #endregion
                                    }

                                    // update VAM_ProductCost with accumulation qty , amt , and current cost
                                    if (isMatchFromForm == "Y" || handlingWindowName == "Match IV")
                                    {
                                        result = cd.UpdateProductCost("Product Cost IV Form", cd, acctSchema, product, M_ASI_ID, invoiceline.GetVAF_Org_ID(), optionalStrCd: optionalstr);
                                    }
                                    else
                                    {
                                        result = cd.UpdateProductCost("Product Cost IV", cd, acctSchema, product, M_ASI_ID, invoiceline.GetVAF_Org_ID(), optionalStrCd: optionalstr);
                                    }
                                    if (!result)
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        _log.Severe("Error occured during saving a record in M-Cost -> Product Cost IV");
                                        return false;
                                    }
                                }

                            }
                            if (inoutline != null && inoutline.GetVAM_Inv_InOutLine_ID() > 0 && optionalstr == "process")
                            {
                                inoutline.SetIsCostCalculated(true);
                                inoutline.Save();
                            }

                            // set is cost calculated true for matched invoice
                            // after using optionalStr - this region never be called
                            if (handlingWindowName == "Match IV" && invoice != null && invoice.GetVAB_Invoice_ID() > 0 && optionalstr == "process")
                            {
                                invoice.SetIsCostCalculated(true);
                                if (invoice.Save())
                                {
                                    if (invoiceline != null && invoiceline.GetVAB_InvoiceLine_ID() > 0)
                                    {
                                        invoiceline.SetIsCostCalculated(true);
                                        invoiceline.Save();
                                    }
                                }
                            }
                            #endregion
                        }

                        if (windowName == "Physical Inventory" || windowName == "Internal Use Inventory" || windowName == "Inventory Move" || windowName == "AssetDisposal")
                        {
                            #region Get Price
                            //Get Price from MVAMProductCost
                            if (windowName == "Physical Inventory" || windowName == "Internal Use Inventory")
                            {
                                VAM_Warehouse_Id = inventoryLine.GetParent().GetVAM_Warehouse_ID();
                            }
                            else if (windowName == "Inventory Move")
                            {
                                VAM_Warehouse_Id = movementline.GetParent().GetDTD001_MWarehouseSource_ID();
                            }
                            else if (windowName == "AssetDisposal")
                            {
                                VAM_Warehouse_Id = Util.GetValueOfInt(po.Get_Value("VAM_Warehouse_ID"));
                            }
                            price = MVAMProductCostQueue.CalculateCost(ctx, acctSchema, product, M_ASI_ID, VAF_Client_ID, VAF_Org_ID, VAM_Warehouse_Id);
                            cmPrice = price;
                            //price = 0;
                            //end
                            int VAB_Currency_ID = 0;
                            if (windowName == "Physical Inventory")
                            {
                                if (price == 0)
                                {
                                    isPriceFromProductPrice = true;
                                    if (windowName == "Inventory Move")
                                    {
                                        query.Clear();
                                        query.Append(@"(SELECT pl.VAF_Org_ID , pl.name , pl.VAM_PriceList_id  , plv.VAM_PriceListVersion_id , pp.pricestd , pl.VAB_Currency_id
                                           FROM VAM_PriceList pl  INNER JOIN VAM_PriceListVersion plv   ON pl.VAM_PriceList_id    = plv.VAM_PriceList_id
                                            inner join VAM_ProductPrice pp   on pp.VAM_PriceListVersion_id = plv.VAM_PriceListVersion_id
                                           WHERE pl.VAF_Org_ID = " + VAF_Org_ID + " AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.isactive = 'Y' and pp.VAM_Product_id = " + product.GetVAM_Product_ID() +
                                               " AND NVL(pp.VAM_PFeature_SetInstance_id, 0) = " + M_ASI_ID +
                                               " AND pp.VAB_UOM_id = " + product.GetVAB_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.VAM_PriceList_id asc");
                                    }
                                    else
                                    {
                                        query.Clear();
                                        query.Append(@"(SELECT pl.VAF_Org_ID , pl.name , pl.VAM_PriceList_id  , plv.VAM_PriceListVersion_id , pp.pricestd , pl.VAB_Currency_id
                                           FROM VAM_PriceList pl  INNER JOIN VAM_PriceListVersion plv   ON pl.VAM_PriceList_id    = plv.VAM_PriceList_id
                                            inner join VAM_ProductPrice pp   on pp.VAM_PriceListVersion_id = plv.VAM_PriceListVersion_id
                                           WHERE pl.VAF_Org_ID = " + inventoryLine.GetVAF_Org_ID() + " AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.isactive = 'Y' and pp.VAM_Product_id = " + product.GetVAM_Product_ID() +
                                                  " AND NVL(pp.VAM_PFeature_SetInstance_id, 0) = " + M_ASI_ID +
                                                  " AND pp.VAB_UOM_id = " + product.GetVAB_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.VAM_PriceList_id asc");
                                    }
                                    DataSet dsStdPrice = DB.ExecuteDataset(query.ToString(), null, null);
                                    if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                                    {
                                        price = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                                        VAB_Currency_ID = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["VAB_Currency_id"]);
                                    }
                                    else
                                    {
                                        query.Clear();
                                        query.Append(@"(SELECT pl.VAF_Org_ID , pl.name , pl.VAM_PriceList_id  , plv.VAM_PriceListVersion_id , pp.pricestd , pl.VAB_Currency_id
                                           FROM VAM_PriceList pl  INNER JOIN VAM_PriceListVersion plv   ON pl.VAM_PriceList_id    = plv.VAM_PriceList_id
                                            inner join VAM_ProductPrice pp   on pp.VAM_PriceListVersion_id = plv.VAM_PriceListVersion_id
                                           WHERE pl.VAF_Org_ID = 0 AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.isactive = 'Y' and pp.VAM_Product_id = " + product.GetVAM_Product_ID() +
                                               " AND NVL(pp.VAM_PFeature_SetInstance_id, 0) = " + M_ASI_ID +
                                               " AND pp.VAB_UOM_id = " + product.GetVAB_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.VAM_PriceList_id asc");
                                        dsStdPrice = DB.ExecuteDataset(query.ToString(), null, null);
                                        if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                                        {
                                            price = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                                            VAB_Currency_ID = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["VAB_Currency_id"]);
                                        }
                                    }
                                    if (price == 0)
                                    {
                                        if (windowName == "Inventory Move")
                                        {
                                            query.Clear();
                                            query.Append(@"(SELECT pl.VAF_Org_ID , pl.name , pl.VAM_PriceList_id  , plv.VAM_PriceListVersion_id , pp.pricestd , pl.VAB_Currency_id
                                           FROM VAM_PriceList pl  INNER JOIN VAM_PriceListVersion plv   ON pl.VAM_PriceList_id    = plv.VAM_PriceList_id
                                            inner join VAM_ProductPrice pp   on pp.VAM_PriceListVersion_id = plv.VAM_PriceListVersion_id
                                           WHERE pl.VAF_Org_ID = " + VAF_Org_ID + " AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.isactive = 'Y' and pp.VAM_Product_id = " + product.GetVAM_Product_ID() +
                                               " AND pp.VAB_UOM_id = " + product.GetVAB_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.VAM_PriceList_id asc");
                                        }
                                        else
                                        {
                                            query.Clear();
                                            query.Append(@"(SELECT pl.VAF_Org_ID , pl.name , pl.VAM_PriceList_id  , plv.VAM_PriceListVersion_id , pp.pricestd , pl.VAB_Currency_id
                                           FROM VAM_PriceList pl  INNER JOIN VAM_PriceListVersion plv   ON pl.VAM_PriceList_id    = plv.VAM_PriceList_id
                                            inner join VAM_ProductPrice pp   on pp.VAM_PriceListVersion_id = plv.VAM_PriceListVersion_id
                                           WHERE pl.VAF_Org_ID = " + inventoryLine.GetVAF_Org_ID() + " AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.isactive = 'Y' and pp.VAM_Product_id = " + product.GetVAM_Product_ID() +
                                                  " AND pp.VAB_UOM_id = " + product.GetVAB_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.VAM_PriceList_id asc");
                                        }
                                        dsStdPrice = DB.ExecuteDataset(query.ToString(), null, null);
                                        if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                                        {
                                            price = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                                            VAB_Currency_ID = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["VAB_Currency_id"]);
                                        }
                                        else
                                        {
                                            query.Clear();
                                            query.Append(@"(SELECT pl.VAF_Org_ID , pl.name , pl.VAM_PriceList_id  , plv.VAM_PriceListVersion_id , pp.pricestd , pl.VAB_Currency_id
                                           FROM VAM_PriceList pl  INNER JOIN VAM_PriceListVersion plv   ON pl.VAM_PriceList_id    = plv.VAM_PriceList_id
                                            inner join VAM_ProductPrice pp   on pp.VAM_PriceListVersion_id = plv.VAM_PriceListVersion_id
                                           WHERE pl.VAF_Org_ID = 0 AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.isactive = 'Y' and pp.VAM_Product_id = " + product.GetVAM_Product_ID() +
                                                   " AND pp.VAB_UOM_id = " + product.GetVAB_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.VAM_PriceList_id asc");
                                            dsStdPrice = DB.ExecuteDataset(query.ToString(), null, null);
                                            if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                                            {
                                                price = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                                                VAB_Currency_ID = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["VAB_Currency_id"]);
                                            }
                                        }
                                        dsStdPrice.Dispose();
                                        if (price == 0)
                                            price = 0;
                                    }
                                }
                            }
                            Price = price * Qty;
                            cmPrice = cmPrice * Qty;

                            if (Price == 0)
                            {
                                if (optionalstr != "window")
                                {
                                    trxName.Rollback();
                                }
                                if (windowName == "Physical Inventory" || windowName == "Internal Use Inventory")
                                {
                                    inventory = new MVAMInventory(ctx, inventoryLine.GetVAM_Inventory_ID(), trxName);
                                    conversionNotFound = inventory.GetDocumentNo();
                                }
                                else if (windowName == "Inventory Move")
                                {
                                    movement = new MVAMInventoryTransfer(ctx, movementline.GetVAM_InventoryTransfer_ID(), trxName);
                                    conversionNotFound = movement.GetDocumentNo();
                                }
                                else if (windowName.Equals("AssetDisposal"))
                                {
                                    conversionNotFound = Util.GetValueOfString(po.Get_Value("DocumentNo"));
                                }
                                return false;
                            }

                            // Currency Conversion
                            if (isPriceFromProductPrice && (windowName == "Physical Inventory" || windowName == "Internal Use Inventory"))
                            {
                                inventory = new MVAMInventory(ctx, inventoryLine.GetVAM_Inventory_ID(), trxName);

                                if (VAB_Currency_ID != 0 && VAB_Currency_ID != acctSchema.GetVAB_Currency_ID())
                                {
                                    Price = MVABExchangeRate.Convert(ctx, Price, VAB_Currency_ID, acctSchema.GetVAB_Currency_ID(),
                                                                            inventory.GetMovementDate(), 0, VAF_Client_ID, VAF_Org_ID);
                                    if (Price == 0)
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        conversionNotFound = inventory.GetDocumentNo();
                                        return false;
                                    }
                                }
                            }
                            //else if (isPriceFromProductPrice && windowName == "AssetDisposal")
                            //{
                            //    //todo
                            //    if (VAB_Currency_ID != 0 && VAB_Currency_ID != acctSchema.GetVAB_Currency_ID())
                            //    {
                            //        Price = MConversionRate.Convert(ctx, Price, VAB_Currency_ID, acctSchema.GetVAB_Currency_ID(),
                            //                                                Util.GetValueOfDateTime(po.Get_Value("DisposedDate")), 0, VAF_Client_ID, VAF_Org_ID);
                            //        if (Price == 0)
                            //        {
                            //            if (optionalstr != "window")
                            //            {
                            //                trxName.Rollback();
                            //            }
                            //            conversionNotFound = Util.GetValueOfString(po.Get_Value("DocumentNo"));
                            //            return false;
                            //        }
                            //    }
                            //}

                            //else if (isPriceFromProductPrice && windowName == "Inventory Move")
                            //{
                            //    movement = new MVAMInventoryTransfer(ctx, movementline.GetVAM_InventoryTransfer_ID(), trxName);
                            //    if (VAB_Currency_ID != 0 && VAB_Currency_ID != acctSchema.GetVAB_Currency_ID())
                            //    {
                            //        Price = MConversionRate.Convert(ctx, Price, VAB_Currency_ID, acctSchema.GetVAB_Currency_ID(),
                            //                                                movement.GetMovementDate(), 0, VAF_Client_ID, VAF_Org_ID);
                            //        if (Price == 0)
                            //        {
                            //            trxName.Rollback();
                            //            conversionNotFound = movement.GetDocumentNo();
                            //            return false;
                            //        }
                            //    }
                            //}
                            plPrice = Price;
                            #endregion
                        }

                        if (windowName == "Production Execution" || windowName.Equals("PE-FinishGood"))
                        {
                            #region Get Price
                            //VAM_Warehouse_Id = Util.GetValueOfInt(po.Get_Value("VAM_Warehouse_ID"));
                            DateTime? VAMFG_DateAcct = null;
                            DataSet dsProductDetail = DB.ExecuteDataset(@"SELECT VAMFG_DateAcct , VAM_Warehouse_ID FROM VAMFG_M_WrkOdrTransaction   
                                                        WHERE VAMFG_M_WrkOdrTransaction_ID = " + po.Get_ValueAsInt("VAMFG_M_WrkOdrTransaction_ID"), null, trxName);
                            if (dsProductDetail != null && dsProductDetail.Tables.Count > 0 && dsProductDetail.Tables[0].Rows.Count > 0)
                            {
                                VAM_Warehouse_Id = Util.GetValueOfInt(dsProductDetail.Tables[0].Rows[0]["VAM_Warehouse_ID"]);
                                VAMFG_DateAcct = Util.GetValueOfDateTime(dsProductDetail.Tables[0].Rows[0]["VAMFG_DateAcct"]);
                            }
                            if (windowName.Equals("PE-FinishGood"))
                            {
                                price = Price;
                                // checking price for Finished Good
                                if (acctSchema.GetVAB_Currency_ID() != Util.GetValueOfInt(ctx.GetContextAsInt("$VAB_Currency_ID")))
                                {
                                    price = MVABExchangeRate.Convert(ctx, price, Util.GetValueOfInt(ctx.GetContext("$VAB_Currency_ID")), acctSchema.GetVAB_Currency_ID(),
                                                                        VAMFG_DateAcct, 0, VAF_Client_ID, VAF_Org_ID);
                                }
                                if (price == 0)
                                {
                                    if (optionalstr == "process")
                                    {
                                        trxName.Rollback();
                                    }
                                    conversionNotFound = Util.GetValueOfString(DB.ExecuteScalar("SELECT DocumentNo from VAMFG_M_WrkOdrTransaction where VAMFG_M_WrkOdrTransaction_ID = " + Util.GetValueOfInt(po.Get_Value("VAMFG_M_WrkOdrTransaction_ID"))));
                                    return false;
                                }
                            }
                            else
                            {
                                price = MVAMProductCostQueue.CalculateCost(ctx, acctSchema, product, M_ASI_ID, VAF_Client_ID, VAF_Org_ID, VAM_Warehouse_Id);
                            }
                            cmPrice = price;
                            Price = price * Qty;
                            cmPrice = cmPrice * Qty;

                            if (Price == 0)
                            {
                                if (optionalstr == "process")
                                {
                                    trxName.Rollback();
                                }
                                if (windowName == "Production Execution")
                                {
                                    conversionNotFound = Util.GetValueOfString(DB.ExecuteScalar("SELECT DocumentNo from VAMFG_M_WrkOdrTransaction where VAMFG_M_WrkOdrTransaction_ID = " + Util.GetValueOfInt(po.Get_Value("VAMFG_M_WrkOdrTransaction_ID"))));
                                }
                                return false;
                            }
                            #endregion
                        }

                        if (windowName == "Inventory Move")
                        {
                            #region Special handling for Inventory move
                            int cdId = 0;
                            // check cost detail is created on completion or not
                            if (optionalstr == "process")
                            {
                                query.Clear();
                                query.Append("SELECT VAM_ProductCostDetail_ID FROM VAM_ProductCostDetail WHERE IsActive = 'Y' AND VAM_InvTrf_Line_ID = " + movementline.GetVAM_InvTrf_Line_ID() +
                                              @" AND VAF_Org_ID = " + VAF_Org_ID);
                                if (costLevel == MProductCategory.COSTINGLEVEL_Warehouse || costLevel == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                {
                                    query.Append(" AND VAM_Warehouse_ID = " + VAM_Warehouse_Id);
                                }
                                cdId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));
                                cdSourceWarehouse = new MVAMProductCostDetail(ctx, cdId, trxName);
                            }

                            // if cost detail not created on completion, need to create cost detail
                            if (cdId <= 0)
                            {
                                if (cmPrice > 0)
                                {
                                    cdSourceWarehouse = MVAMProductCostDetail.CreateCostDetail(acctSchema, VAF_Org_ID, product.GetVAM_Product_ID(), M_ASI_ID, windowName,
                                        inventoryLine, inoutline, movementline, invoiceline, po, 0, Decimal.Negate(cmPrice), Decimal.Negate(Qty), null, trxName, VAM_Warehouse_Id);
                                }
                                else
                                {
                                    cdSourceWarehouse = MVAMProductCostDetail.CreateCostDetail(acctSchema, VAF_Org_ID, product.GetVAM_Product_ID(), M_ASI_ID, windowName,
                                        inventoryLine, inoutline, movementline, invoiceline, po, 0, Decimal.Negate(Price), Decimal.Negate(Qty), null, trxName, VAM_Warehouse_Id);
                                }
                            }
                            SourceVAM_Warehouse_Id = VAM_Warehouse_Id; // maintain warehouse reference of Source Warehouse 
                            query.Clear();
                            query.Append("SELECT VAM_Warehouse_ID FROM VAM_Warehouse WHERE IsActive = 'Y' AND VAM_Warehouse_ID = (SELECT VAM_Warehouse_ID FROM VAM_InventoryTransfer WHERE IsActive = 'Y' AND VAM_InventoryTransfer_ID = " + movementline.GetVAM_InventoryTransfer_ID() + ")");
                            VAM_Warehouse_Id = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));
                            if (VAM_Warehouse_Id == 0)
                            {
                                VAM_Warehouse_Id = MVAMLocator.Get(ctx, movementline.GetVAM_LocatorTo_ID()).GetVAM_Warehouse_ID();
                            }
                            VAF_Org_ID = (MWarehouse.Get(ctx, VAM_Warehouse_Id)).GetVAF_Org_ID();
                            #endregion
                        }


                        // calculate expected landed cost 
                        if (windowName == "Material Receipt" && orderline != null && orderline.Get_ID() > 0
                            && order != null && order.Get_ID() > 0 && inoutline != null && inoutline.Get_ID() > 0 && !inoutline.IsCostImmediate())
                        {
                            #region calculate expected landed cost 
                            decimal expectedAmt = 0;
                            decimal expectedQty = 0;
                            query.Clear();
                            query.Append("SELECT VAB_ExpectedCost.VAM_ProductCostElement_ID , VAB_ExpectedCostdistribution.Amt , VAB_ExpectedCostdistribution.Qty, VAB_ExpectedCostdistribution.VAB_ExpectedCostdistribution_ID  " +
                                          " FROM VAB_ExpectedCostdistribution INNER JOIN VAB_ExpectedCost ON VAB_ExpectedCostdistribution.VAB_ExpectedCost_ID = VAB_ExpectedCost.VAB_ExpectedCost_ID " +
                                          " WHERE VAB_ExpectedCostdistribution.Amt != 0 AND VAB_ExpectedCostdistribution.VAB_OrderLine_ID = " + orderline.GetVAB_OrderLine_ID());
                            //if (inoutline.GetReversalDoc_ID() > 0)
                            //{
                            //    // during reversal of MR doc, system check expected landed cost is calculated ?
                            //    // if yes then need to reversed the impact
                            //    query.Append(" AND VAB_ExpectedCostdistribution.IsCostCalculated = 'Y' ");
                            //}
                            //else
                            //{
                            //    query.Append(" AND VAB_ExpectedCostdistribution.IsCostCalculated = 'N' ");
                            //}
                            DataSet dsExpectedLandedCostAllocation = DB.ExecuteDataset(query.ToString(), null, trxName);
                            if (dsExpectedLandedCostAllocation != null && dsExpectedLandedCostAllocation.Tables.Count > 0 && dsExpectedLandedCostAllocation.Tables[0].Rows.Count > 0)
                            {
                                for (int lca = 0; lca < dsExpectedLandedCostAllocation.Tables[0].Rows.Count; lca++)
                                {
                                    // total distributed amount against orderline
                                    expectedAmt = Util.GetValueOfDecimal(dsExpectedLandedCostAllocation.Tables[0].Rows[lca]["Amt"]);
                                    // Orderline qty
                                    expectedQty = Util.GetValueOfDecimal(dsExpectedLandedCostAllocation.Tables[0].Rows[lca]["Qty"]);

                                    // movement qty
                                    expectedQty = inoutline.GetMovementQty();

                                    // during cost adjustment - all amount to be distributed
                                    if (!product.IsCostAdjustmentOnLost())
                                    {
                                        // distributed amount of each qty
                                        expectedAmt = Decimal.Divide(expectedAmt, Util.GetValueOfDecimal(dsExpectedLandedCostAllocation.Tables[0].Rows[lca]["Qty"]));
                                        //total amount of movement qty
                                        expectedAmt = Decimal.Multiply(expectedAmt, inoutline.GetMovementQty());
                                    }
                                    else if (expectedQty < 0 && expectedAmt > 0)
                                    {
                                        // In case of normal loss, when qty is less than ZERO then nagate amount 
                                        expectedAmt = Decimal.Negate(expectedAmt);
                                    }

                                    if (order.GetVAB_Currency_ID() != acctSchema.GetVAB_Currency_ID())
                                    {
                                        expectedAmt = MVABExchangeRate.Convert(ctx, expectedAmt, order.GetVAB_Currency_ID(), acctSchema.GetVAB_Currency_ID(),
                                                                    inout.GetDateAcct(), order.GetVAB_CurrencyType_ID(), VAF_Client_ID, VAF_Org_ID);
                                        if (expectedAmt == 0)
                                        {
                                            if (optionalstr != "window")
                                            {
                                                trxName.Rollback();
                                            }
                                            conversionNotFound = inout.GetDocumentNo();
                                            return false;
                                        }
                                    }

                                    // check cost detail is created on completion or not
                                    if (optionalstr == "process")
                                    {
                                        query.Clear();
                                        query.Append("SELECT VAM_ProductCostDetail_ID FROM VAM_ProductCostDetail WHERE IsActive = 'Y' " +
                                                     " AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                                     " AND VAM_ProductCostElement_ID = " + Util.GetValueOfInt(dsExpectedLandedCostAllocation.Tables[0].Rows[lca]["VAM_ProductCostElement_ID"]) +
                                                     " AND VAB_Orderline_ID = " + inoutline.GetVAB_OrderLine_ID() +
                                                     " AND VAM_Inv_InOutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID() +
                                                     " AND VAM_Product_ID = " + inoutline.GetVAM_Product_ID() +
                                                     " AND  VAM_PFeature_SetInstance_ID = " + inoutline.GetVAM_PFeature_SetInstance_ID());
                                        int cdId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));
                                        cd = new MVAMProductCostDetail(ctx, cdId, trxName);
                                    }

                                    // if cost detail not created on completion, need to create cost detail
                                    if (cd == null || cd.GetVAM_ProductCostDetail_ID() <= 0)
                                    {
                                        // get warehouse org -- freight record to be created in warehouse org
                                        if (VAM_Warehouse_Id > 0)
                                        {
                                            VAF_Org_ID = MWarehouse.Get(ctx, VAM_Warehouse_Id).GetVAF_Org_ID();
                                        }
                                        cd = MVAMProductCostDetail.CreateCostDetail(acctSchema, VAF_Org_ID, inoutline.GetVAM_Product_ID(),
                                            inoutline.GetVAM_PFeature_SetInstance_ID(), windowName, inventoryLine, inoutline, movementline,
                                             invoiceline, po, Util.GetValueOfInt(dsExpectedLandedCostAllocation.Tables[0].Rows[lca]["VAM_ProductCostElement_ID"]), expectedAmt,
                                             expectedQty, null, trxName, VAM_Warehouse_Id);
                                    }
                                    if (cd != null)
                                    {
                                        result = cd.UpdateProductCost(windowName, cd, acctSchema, product, inoutline.GetVAM_PFeature_SetInstance_ID(),
                                                                      VAF_Org_ID, optionalStrCd: optionalstr);
                                        if (result)
                                        {
                                            // will mark IsCostCalculated as TRUE during last accounting schema cycle
                                            //if (i == ds.Tables[0].Rows.Count - 1)
                                            //{
                                            //    DB.ExecuteQuery(@"UPDATE VAB_ExpectedCostdistribution SET 
                                            //     IsCostCalculated = '" + (inoutline.GetReversalDoc_ID() > 0 ? 'N' : 'Y') + @"' 
                                            //    WHERE VAB_ExpectedCostdistribution_ID = " + Util.GetValueOfInt(dsExpectedLandedCostAllocation.Tables[0].Rows[lca]["VAB_ExpectedCostdistribution_ID"]), null, trxName);
                                            //}
                                        }
                                        else
                                        {
                                            if (optionalstr != "window")
                                            {
                                                trxName.Rollback();
                                            }
                                            DB.ExecuteQuery("DELETE FROM VAM_ProductCostDetail WHERE VAM_ProductCostDetail_ID = " + cd.GetVAM_ProductCostDetail_ID(), null, trxName);
                                            _log.Severe("Error occured during UpdateProductCost for VAB_ExpectedCostdistribution_ID = "
                                                + Util.GetValueOfInt(dsExpectedLandedCostAllocation.Tables[0].Rows[lca]["VAB_ExpectedCostdistribution_ID"]));
                                            return false;
                                        }

                                        // set cost detail reference as null, so that system will calculate MR costs also
                                        cd = null;
                                    }
                                }
                            }
                            #endregion
                        }

                        // landed cost calculated on completion, not through process
                        // Cost element detail not created against it.
                        if (((windowName == "Invoice(Vendor)" && product == null) || (windowName == "Invoice(Vendor)" && product != null && product.GetProductType() == "E") ||
                            (windowName == "LandedCost" && product != null)))
                        {
                            #region Landed Cost Allocation Handle
                            isLandedCostAllocation = true;
                            decimal amt = 0;
                            decimal qntity = 0;
                            bool isExpectedCostCalculated = false;
                            query.Clear();
                            query.Append("SELECT * FROM VAB_LCostDistribution WHERE IsCostCalculated = 'N' AND Amt != 0 AND VAB_InvoiceLine_ID = " + invoiceline.GetVAB_InvoiceLine_ID());
                            if (windowName == "LandedCost")
                            {
                                query.Append(" AND VAM_Product_ID = " + product.GetVAM_Product_ID());
                            }
                            //query.Append("SELECT * FROM VAB_LCostDistribution WHERE Amt != 0 AND VAB_InvoiceLine_ID = " + invoiceline.GetVAB_InvoiceLine_ID());
                            //
                            DataSet dsLandedCostAllocation = DB.ExecuteDataset(query.ToString(), null, trxName);
                            if (dsLandedCostAllocation != null && dsLandedCostAllocation.Tables.Count > 0 && dsLandedCostAllocation.Tables[0].Rows.Count > 0)
                            {
                                invoice = new MVABInvoice(ctx, invoiceline.GetVAB_Invoice_ID(), trxName);
                                for (int lca = 0; lca < dsLandedCostAllocation.Tables[0].Rows.Count; lca++)
                                {
                                    cd = null;
                                    isExpectedCostCalculated = Util.GetValueOfString(dsLandedCostAllocation.Tables[0].Rows[lca]["IsExpectedCostCalculated"]).Equals("Y");

                                    if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                    {
                                        if (Util.GetValueOfDecimal(dsLandedCostAllocation.Tables[0].Rows[lca]["Qty"]) > 0)
                                        {
                                            qntity = Decimal.Negate(Util.GetValueOfDecimal(dsLandedCostAllocation.Tables[0].Rows[lca]["Qty"]));
                                        }
                                        else
                                        {
                                            qntity = Util.GetValueOfDecimal(dsLandedCostAllocation.Tables[0].Rows[lca]["Qty"]);
                                        }
                                        if (Util.GetValueOfDecimal(dsLandedCostAllocation.Tables[0].Rows[lca][isExpectedCostCalculated ? "DifferenceAmt" : "Amt"]) > 0)
                                        {
                                            if (isExpectedCostCalculated)
                                            {
                                                amt = Util.GetValueOfDecimal(dsLandedCostAllocation.Tables[0].Rows[lca]["DifferenceAmt"]);
                                            }
                                            else
                                            {
                                                amt = Decimal.Negate(Util.GetValueOfDecimal(dsLandedCostAllocation.Tables[0].Rows[lca]["Amt"]));
                                            }
                                        }
                                        else
                                        {
                                            amt = Util.GetValueOfDecimal(dsLandedCostAllocation.Tables[0].Rows[lca][isExpectedCostCalculated ? "DifferenceAmt" : "Amt"]);
                                        }
                                    }
                                    else
                                    {
                                        qntity = Util.GetValueOfDecimal(dsLandedCostAllocation.Tables[0].Rows[lca]["Qty"]);
                                        amt = Util.GetValueOfDecimal(dsLandedCostAllocation.Tables[0].Rows[lca][isExpectedCostCalculated ? "DifferenceAmt" : "Amt"]);
                                    }
                                    // conversion required
                                    if (invoice.GetVAB_Currency_ID() != acctSchema.GetVAB_Currency_ID())
                                    {
                                        amt = MVABExchangeRate.Convert(ctx, amt, invoice.GetVAB_Currency_ID(), acctSchema.GetVAB_Currency_ID(),
                                                                             invoice.GetDateAcct(), invoice.GetVAB_CurrencyType_ID(), VAF_Client_ID, VAF_Org_ID);
                                        if (amt == 0)
                                        {
                                            if (optionalstr != "window")
                                            {
                                                trxName.Rollback();
                                            }
                                            conversionNotFound = invoice.GetDocumentNo();
                                            return false;
                                        }
                                    }

                                    // check cost detail is created on completion or not
                                    if (optionalstr == "process")
                                    {
                                        query.Clear();
                                        query.Append("SELECT VAM_ProductCostDetail_ID FROM VAM_ProductCostDetail WHERE IsActive = 'Y'  AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() + " AND VAB_InvoiceLine_ID = " + invoiceline.GetVAB_InvoiceLine_ID());
                                        if (invoiceline.GetVAB_OrderLine_ID() > 0)
                                        {
                                            query.Append(" AND VAB_Orderline_ID = " + invoiceline.GetVAB_OrderLine_ID());
                                        }
                                        if (invoiceline.GetVAM_Inv_InOutLine_ID() > 0)
                                        {
                                            query.Append(" AND VAM_Inv_InOutLine_ID = " + invoiceline.GetVAM_Inv_InOutLine_ID());
                                        }
                                        query.Append(" AND VAM_Product_ID = " + Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["VAM_Product_ID"]) +
                                                     " AND  VAM_PFeature_SetInstance_ID = " + Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["VAM_PFeature_SetInstance_ID"]));
                                        int cdId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));
                                        cd = new MVAMProductCostDetail(ctx, cdId, trxName);
                                    }

                                    // if cost detail not created on completion, need to create cost detail
                                    if (cd == null || cd.GetVAM_ProductCostDetail_ID() <= 0)
                                    {
                                        VAM_Warehouse_Id = Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["VAM_Warehouse_ID"]);
                                        // get warehouse org -- freight record to be created in warehouse org
                                        if (VAM_Warehouse_Id > 0)
                                        {
                                            VAF_Org_ID = MWarehouse.Get(ctx, VAM_Warehouse_Id).GetVAF_Org_ID();
                                        }
                                        cd = MVAMProductCostDetail.CreateCostDetail(acctSchema, VAF_Org_ID, Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["VAM_Product_ID"]),
                                            Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["VAM_PFeature_SetInstance_ID"]), windowName, inventoryLine, inoutline, movementline,
                                         invoiceline, po, Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["VAM_ProductCostElement_ID"]), amt,
                                         qntity, null, trxName, VAM_Warehouse_Id);
                                    }
                                    if (cd != null && ((optionalstr == "window" && client.IsCostImmediate()) || (optionalstr == "process")))
                                    {
                                        productLca = new MProduct(ctx, Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["VAM_Product_ID"]), trxName);
                                        M_ASI_ID = Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["VAM_PFeature_SetInstance_ID"]);
                                        // set dynamic property as expected landed cost 
                                        cd.SetExpectedCostCalculated(isExpectedCostCalculated);
                                        //if ((optionalstr == "window" && client.IsCostImmediate()) || (optionalstr == "process" && !client.IsCostImmediate()))
                                        //{
                                        result = cd.UpdateProductCost(windowName, cd, acctSchema, productLca, M_ASI_ID, invoiceline.GetVAF_Org_ID(), optionalStrCd: optionalstr);
                                        if (result)
                                        {
                                            // will mark IsCostCalculated as TRUE during last accounting schema cycle
                                            if (i == ds.Tables[0].Rows.Count - 1)
                                            {
                                                DB.ExecuteQuery(@"UPDATE VAB_LCostDistribution SET IsCostCalculated = 'Y' WHERE VAB_LCostDistribution_ID = "
                                                    + Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["VAB_LCostDistribution_ID"]), null, trxName);
                                            }
                                        }
                                        else
                                        {
                                            if (optionalstr != "window")
                                            {
                                                trxName.Rollback();
                                            }
                                            DB.ExecuteQuery("DELETE FROM VAM_ProductCostDetail WHERE VAM_ProductCostDetail_ID = " + cd.GetVAM_ProductCostDetail_ID(), null, trxName);
                                            _log.Severe("Error occured during UpdateProductCost for VAB_LCostDistribution = " + Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["VAB_LCostDistribution_ID"]));
                                            return false;
                                        }
                                        //}
                                        if (cd != null)
                                        {
                                            cd.CreateCostForCombination(cd, acctSchema, productLca, M_ASI_ID, 0, "LandedCostAllocation", optionalStrcc: optionalstr);
                                        }
                                    }
                                }
                            }
                            dsLandedCostAllocation.Dispose();
                            #endregion
                        }
                        else if (windowName != "Match PO" && windowName != "Match IV")
                        {
                            #region check cost detail is created on completion or not. if not then create cost detail
                            if (optionalstr == "process")
                            {
                                query.Clear();
                                query.Append("SELECT MIN(VAM_ProductCostDetail_ID) FROM VAM_ProductCostDetail WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID());
                                if (windowName == "Invoice(Vendor)" || windowName == "Invoice(Customer)" || windowName == "Invoice(Vendor)-Return")
                                {
                                    if (invoiceline.GetVAB_InvoiceLine_ID() > 0)
                                    {
                                        query.Append("  AND VAB_InvoiceLine_ID = " + invoiceline.GetVAB_InvoiceLine_ID());
                                    }
                                    if (invoiceline.GetVAB_OrderLine_ID() > 0)
                                    {
                                        query.Append(" AND VAB_Orderline_ID = " + invoiceline.GetVAB_OrderLine_ID());
                                    }
                                    if (invoiceline.GetVAM_Inv_InOutLine_ID() > 0)
                                    {
                                        if ((windowName == "Invoice(Vendor)" || windowName == "Invoice(Vendor)-Return") && matchInoutLine != null && matchInoutLine.GetVAM_Inv_InOutLine_ID() > 0)
                                        {
                                            // here inoutline contain ref from match invoice table
                                            query.Append(" AND VAM_Inv_InOutLine_ID = " + matchInoutLine.GetVAM_Inv_InOutLine_ID());
                                        }
                                        else
                                        {
                                            query.Append(" AND VAM_Inv_InOutLine_ID = " + invoiceline.GetVAM_Inv_InOutLine_ID());
                                        }
                                    }
                                    else if (invoiceline.GetVAB_OrderLine_ID() > 0 && windowName == "Invoice(Customer)")
                                    {
                                        query.Append(" AND VAM_Inv_InOutLine_ID = " + Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAM_Inv_InOutLine_id FROM VAM_Inv_InOutLine WHERE isactive = 'Y' AND VAB_Orderline_id = " + invoiceline.GetVAB_OrderLine_ID(), null, null)));
                                    }
                                }
                                else if (windowName == "Material Receipt" || windowName == "Shipment" || windowName == "Customer Return" || windowName == "Return To Vendor")
                                {
                                    if (inoutline.GetVAM_Inv_InOutLine_ID() > 0)
                                    {
                                        query.Append(" AND VAM_Inv_InOutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID());
                                    }
                                    if (inoutline.GetVAB_OrderLine_ID() > 0)
                                    {
                                        query.Append(" AND VAB_Orderline_ID = " + inoutline.GetVAB_OrderLine_ID());
                                    }
                                    query.Append(" AND NVL(VAB_InvoiceLine_ID , 0) = 0 AND NVL(VAM_ProductCostElement_ID , 0) = 0 ");
                                }
                                else if (windowName == "Physical Inventory" || windowName == "Internal Use Inventory")
                                {
                                    query.Append(" AND VAM_InventoryLine_ID = " + inventoryLine.GetVAM_InventoryLine_ID());
                                }
                                else if (windowName == "AssetDisposal")
                                {
                                    query.Append(" AND VAFAM_AssetDisposal_ID = " + Util.GetValueOfInt(po.Get_Value("VAFAM_AssetDisposal_ID")));
                                }
                                else if (windowName == "Production Execution" || windowName.Equals("PE-FinishGood"))
                                {
                                    query.Append(" AND VAMFG_M_WrkOdrTrnsctionLine_ID = " + Util.GetValueOfInt(po.Get_Value("VAMFG_M_WrkOdrTrnsctionLine_ID")));
                                }
                                else if (windowName == "Inventory Move")
                                {
                                    query.Append(" AND VAM_InvTrf_Line_ID = " + movementline.GetVAM_InvTrf_Line_ID() +
                                    @" AND VAF_Org_ID = " + VAF_Org_ID);
                                    if (costLevel == MProductCategory.COSTINGLEVEL_Warehouse || costLevel == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                    {
                                        query.Append(" AND VAM_Warehouse_ID = " + VAM_Warehouse_Id);
                                    }
                                }
                                int cdId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));
                                cd = new MVAMProductCostDetail(ctx, cdId, trxName);
                            }

                            if (cd == null || cd.GetVAM_ProductCostDetail_ID() <= 0)
                            {
                                if (cmPrice != 0)
                                {
                                    cd = MVAMProductCostDetail.CreateCostDetail(acctSchema, VAF_Org_ID, product.GetVAM_Product_ID(), M_ASI_ID, windowName,
                                          inventoryLine, inoutline, movementline, invoiceline, po, 0, cmPrice, Qty, null, trxName, VAM_Warehouse_Id);
                                }
                                else
                                {
                                    cd = MVAMProductCostDetail.CreateCostDetail(acctSchema, VAF_Org_ID, product.GetVAM_Product_ID(), M_ASI_ID, windowName,
                                         inventoryLine, inoutline, movementline, invoiceline, po, 0, Price, Qty, null, trxName, VAM_Warehouse_Id);
                                }
                            }
                            #endregion
                        }

                        #region Create / update "Cost Queue"
                        // Not to Create / Update / detele cost queue from process. impact comes here on Completion
                        if (cd != null && !isLandedCostAllocation &&
                            ((optionalstr != "process" && client.IsCostImmediate()) || (optionalstr == "process" && !client.IsCostImmediate())
                            || (optionalstr == "process" && client.IsCostImmediate() &&
                                inoutline != null && inoutline.GetVAM_Inv_InOutLine_ID() > 0 &&
                                !inoutline.IsCostImmediate()) //&& windowName == "Material Receipt" / shipment and its reverse
                             || (optionalstr == "process" && client.IsCostImmediate() &&
                                invoiceline != null && invoiceline.GetVAB_InvoiceLine_ID() > 0 &&
                                !invoiceline.IsCostImmediate()) //&& windowName == "Invoice(Vendor)"
                                || (optionalstr == "process" && client.IsCostImmediate() &&
                                inventoryLine != null && inventoryLine.GetVAM_InventoryLine_ID() > 0 &&
                                !inventoryLine.IsCostImmediate()) // window = physical inventory / internal use inventory
                                || (optionalstr == "process" && client.IsCostImmediate() &&
                                movementline != null && movementline.GetVAM_InvTrf_Line_ID() > 0 &&
                                !movementline.IsCostImmediate()) // window = movement 
                                || (optionalstr == "process" && client.IsCostImmediate() &&
                                po != null && (windowName == "Production Execution" || windowName.Equals("PE-FinishGood")) && Util.GetValueOfInt(po.Get_Value("VAMFG_M_WrkOdrTrnsctionLine_ID")) > 0 &&
                                !Util.GetValueOfBool(po.Get_Value("IsCostImmediate")))
                                || (optionalstr == "process" && client.IsCostImmediate() &&
                                po != null && windowName == "AssetDisposal" && Util.GetValueOfInt(po.Get_Value("VAFAM_AssetDisposal_ID")) > 0 &&
                                !Util.GetValueOfBool(po.Get_Value("IsCostImmediate")))
                                ))
                        {
                            query.Clear();
                            query.Append(@"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = " +
                                     " ( SELECT MMPolicy FROM VAM_ProductCategory WHERE IsActive = 'Y' AND VAM_ProductCategory_ID = " +
                                     " (SELECT VAM_ProductCategory_ID FROM VAM_Product WHERE IsActive = 'Y' AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " )) AND VAF_Client_ID = " + VAF_Client_ID);
                            costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                            costElement = new MVAMProductCostElement(ctx, costingElementId, null);

                            if (windowName == "Physical Inventory" || windowName == "Internal Use Inventory")
                            {
                                #region Phy. Inventory / Internal Use Inventory
                                if (Qty > 0)
                                {
                                    result = CreateCostQueue(ctx, acctSchema, product, M_ASI_ID, VAF_Client_ID, inventoryLine.GetVAF_Org_ID(), Price / Qty, Qty, windowName,
                                          inventoryLine, inoutline, movementline, invoiceline, cd, trxName, out costQueuseIds);
                                    if (!result)
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        else
                                        {
                                            DB.ExecuteQuery("DELETE FROM VAM_ProductCostDetail WHERE VAM_ProductCostDetail_ID = " + cd.GetVAM_ProductCostDetail_ID(), null, trxName);
                                            DB.ExecuteQuery("DELETE FROM VAM_ProductCostQueue WHERE VAM_ProductCostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                        }
                                        _log.Severe("Error occured during CreateCostQueue for VAM_Inventory_ID = " + inventoryLine.GetVAM_InventoryLine_ID());
                                        return false;
                                    }
                                }
                                else
                                {
                                    //1st entry either of FIFO of LIFO 
                                    updateCostQueue(product, M_ASI_ID, acctSchema, inventoryLine.GetVAF_Org_ID(), costElement, Decimal.Negate(Qty), VAM_Warehouse_Id);

                                    //2nd either for Fifo or lifo opposite of 1st entry
                                    if (costElement.GetCostingMethod() == "F")
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND VAF_Client_ID = " + VAF_Client_ID);
                                    }
                                    else
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND VAF_Client_ID = " + VAF_Client_ID);
                                    }
                                    costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                                    costElement = new MVAMProductCostElement(ctx, costingElementId, null);
                                    updateCostQueue(product, M_ASI_ID, acctSchema, inventoryLine.GetVAF_Org_ID(), costElement, Decimal.Negate(Qty), VAM_Warehouse_Id);
                                }
                                #endregion
                            }
                            if (windowName == "AssetDisposal")
                            {
                                #region AssetDisposal
                                if (Qty > 0)
                                {
                                    result = CreateCostQueue(ctx, acctSchema, product, M_ASI_ID, VAF_Client_ID, Util.GetValueOfInt(po.Get_Value("VAF_Org_ID")), Price / Qty, Qty, windowName,
                                          inventoryLine, inoutline, movementline, invoiceline, cd, trxName, out costQueuseIds);
                                    if (!result)
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        else
                                        {
                                            DB.ExecuteQuery("DELETE FROM VAM_ProductCostDetail WHERE VAM_ProductCostDetail_ID = " + cd.GetVAM_ProductCostDetail_ID(), null, trxName);
                                            DB.ExecuteQuery("DELETE FROM VAM_ProductCostQueue WHERE VAM_ProductCostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                        }
                                        _log.Severe("Error occured during CreateCostQueue for VAFAM_AssetDisposal_ID = " + Util.GetValueOfInt(po.Get_Value("VAFAM_AssetDisposal_ID")));
                                        return false;
                                    }
                                }
                                else
                                {
                                    //1st entry either of FIFO of LIFO 
                                    updateCostQueue(product, M_ASI_ID, acctSchema, Util.GetValueOfInt(po.Get_Value("VAF_Org_ID")), costElement, Decimal.Negate(Qty), VAM_Warehouse_Id);

                                    //2nd either for Fifo or lifo opposite of 1st entry
                                    if (costElement.GetCostingMethod() == "F")
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND VAF_Client_ID = " + VAF_Client_ID);
                                    }
                                    else
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND VAF_Client_ID = " + VAF_Client_ID);
                                    }
                                    costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                                    costElement = new MVAMProductCostElement(ctx, costingElementId, null);
                                    updateCostQueue(product, M_ASI_ID, acctSchema, Util.GetValueOfInt(po.Get_Value("VAF_Org_ID")), costElement, Decimal.Negate(Qty), VAM_Warehouse_Id);
                                }
                                #endregion
                            }
                            else if (windowName == "Production Execution" || windowName.Equals("PE-FinishGood"))
                            {
                                #region Production Execution
                                if (Qty > 0)
                                {
                                    result = CreateCostQueue(ctx, acctSchema, product, M_ASI_ID, VAF_Client_ID, Util.GetValueOfInt(po.Get_Value("VAF_Org_ID")),
                                        Price / Qty, Qty, windowName, inventoryLine, inoutline, movementline, invoiceline, cd, trxName, out costQueuseIds);
                                    if (!result)
                                    {
                                        if (optionalstr == "process")
                                        {
                                            trxName.Rollback();
                                        }
                                        else
                                        {
                                            DB.ExecuteQuery("DELETE FROM VAM_ProductCostDetail WHERE VAM_ProductCostDetail_ID = " + cd.GetVAM_ProductCostDetail_ID(), null, trxName);
                                            DB.ExecuteQuery("DELETE FROM VAM_ProductCostQueue WHERE VAM_ProductCostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                        }
                                        _log.Severe("Error occured during CreateCostQueue for Production Execution line = " + Util.GetValueOfInt(po.Get_Value("VAMFG_M_WrkOdrTrnsctionLine_ID")));
                                        return false;
                                    }
                                }
                                else
                                {
                                    //1st entry either of FIFO of LIFO 
                                    updateCostQueue(product, M_ASI_ID, acctSchema, Util.GetValueOfInt(po.Get_Value("VAF_Org_ID")), costElement, Decimal.Negate(Qty), VAM_Warehouse_Id);

                                    //2nd either for Fifo or lifo opposite of 1st entry
                                    if (costElement.GetCostingMethod() == "F")
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND VAF_Client_ID = " + VAF_Client_ID);
                                    }
                                    else
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND VAF_Client_ID = " + VAF_Client_ID);
                                    }
                                    costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                                    costElement = new MVAMProductCostElement(ctx, costingElementId, null);
                                    updateCostQueue(product, M_ASI_ID, acctSchema, Util.GetValueOfInt(po.Get_Value("VAF_Org_ID")), costElement, Decimal.Negate(Qty), VAM_Warehouse_Id);
                                }
                                #endregion
                            }

                            else if (windowName == "Material Receipt" || windowName == "Customer Return" || windowName == "Shipment" || windowName == "Return To Vendor")
                            {
                                #region Sales / Purchase / Return
                                if (Qty > 0)
                                {
                                    result = CreateCostQueue(ctx, acctSchema, product, M_ASI_ID, VAF_Client_ID, inoutline.GetVAF_Org_ID(), Price / Qty, Qty, windowName,
                                         inventoryLine, inoutline, movementline, invoiceline, cd, trxName, out costQueuseIds);
                                    if (!result)
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        else
                                        {
                                            DB.ExecuteQuery("DELETE FROM VAM_ProductCostDetail WHERE VAM_ProductCostDetail_ID = " + cd.GetVAM_ProductCostDetail_ID(), null, trxName);
                                            DB.ExecuteQuery("DELETE FROM VAM_ProductCostQueue WHERE VAM_ProductCostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                        }
                                        _log.Severe("Error occured during CreateCostQueue for VAM_Inv_InOut_ID = " + inoutline.GetVAM_Inv_InOutLine_ID());
                                        return false;
                                    }
                                }
                                else
                                {
                                    //1st entry either of FIFO of LIFO 
                                    updateCostQueue(product, M_ASI_ID, acctSchema, inoutline.GetVAF_Org_ID(), costElement, Decimal.Negate(Qty), VAM_Warehouse_Id);

                                    //2nd either for Fifo or lifo opposite of 1st entry
                                    if (costElement.GetCostingMethod() == "F")
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND VAF_Client_ID = " + VAF_Client_ID);
                                    }
                                    else
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND VAF_Client_ID = " + VAF_Client_ID);
                                    }
                                    costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                                    costElement = new MVAMProductCostElement(ctx, costingElementId, null);
                                    updateCostQueue(product, M_ASI_ID, acctSchema, inoutline.GetVAF_Org_ID(), costElement, Decimal.Negate(Qty), VAM_Warehouse_Id);
                                }
                                #endregion
                            }
                            else if (windowName == "Inventory Move")
                            {
                                #region Inventory Move
                                if (Qty > 0)
                                {
                                    result = CreateCostQueue(ctx, acctSchema, product, M_ASI_ID, VAF_Client_ID, VAF_Org_ID, Price / Qty, Qty, windowName,
                                         inventoryLine, inoutline, movementline, invoiceline, cd, trxName, out costQueuseIds);
                                    if (!result)
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        else
                                        {
                                            DB.ExecuteQuery("DELETE FROM VAM_ProductCostDetail WHERE VAM_ProductCostDetail_ID = " + cd.GetVAM_ProductCostDetail_ID(), null, trxName);
                                            DB.ExecuteQuery("DELETE FROM VAM_ProductCostQueue WHERE VAM_ProductCostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                        }
                                        _log.Severe("Error occured during CreateCostQueue for VAM_InventoryTransferLime_ID = " + movementline.GetVAM_InvTrf_Line_ID());
                                        return false;
                                    }

                                    //1st entry either of FIFO of LIFO 
                                    updateCostQueue(product, M_ASI_ID, acctSchema, movementline.GetVAF_Org_ID(), costElement, Qty, SourceVAM_Warehouse_Id);

                                    //2nd either for Fifo or lifo opposite of 1st entry
                                    if (costElement.GetCostingMethod() == "F")
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND VAF_Client_ID = " + VAF_Client_ID);
                                    }
                                    else
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND VAF_Client_ID = " + VAF_Client_ID);
                                    }
                                    costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                                    costElement = new MVAMProductCostElement(ctx, costingElementId, null);
                                    updateCostQueue(product, M_ASI_ID, acctSchema, movementline.GetVAF_Org_ID(), costElement, Qty, SourceVAM_Warehouse_Id);
                                }
                                else
                                {
                                    // change refsrence from cd to cdSourceWarehouse -- bcz we are incresing stock in "Source Warehouse"
                                    result = CreateCostQueue(ctx, acctSchema, product, M_ASI_ID, VAF_Client_ID, movementline.GetVAF_Org_ID(), Price / Qty, Decimal.Negate(Qty), windowName,
                                        inventoryLine, inoutline, movementline, invoiceline, cdSourceWarehouse, trxName, out costQueuseIds);
                                    if (!result)
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        else
                                        {
                                            DB.ExecuteQuery("DELETE FROM VAM_ProductCostDetail WHERE VAM_ProductCostDetail_ID = " + cd.GetVAM_ProductCostDetail_ID(), null, trxName);
                                            DB.ExecuteQuery("DELETE FROM VAM_ProductCostQueue WHERE VAM_ProductCostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                        }
                                        _log.Severe("Error occured during CreateCostQueue for VAM_InvTrf_Line_ID = " + movementline.GetVAM_InvTrf_Line_ID());
                                        return false;
                                    }

                                    // VAM_Warehouse_Id -- reduce stock from "To Warehouse"
                                    //1st entry either of FIFO of LIFO   
                                    updateCostQueue(product, M_ASI_ID, acctSchema, VAF_Org_ID, costElement, Decimal.Negate(Qty), VAM_Warehouse_Id);

                                    //2nd either for Fifo or lifo opposite of 1st entry
                                    if (costElement.GetCostingMethod() == "F")
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND VAF_Client_ID = " + VAF_Client_ID);
                                    }
                                    else
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND VAF_Client_ID = " + VAF_Client_ID);
                                    }
                                    costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                                    costElement = new MVAMProductCostElement(ctx, costingElementId, null);
                                    updateCostQueue(product, M_ASI_ID, acctSchema, VAF_Org_ID, costElement, Decimal.Negate(Qty), VAM_Warehouse_Id);
                                }
                                #endregion
                            }
                            else if (windowName == "Invoice(Vendor)")
                            {
                                #region update cost on cost Queue / VAM_ProductCost
                                if (invoiceline.GetVAB_OrderLine_ID() > 0 && matchInoutLine != null && matchInoutLine.GetVAM_Inv_InOutLine_ID() > 0)
                                {
                                    query.Clear();
                                    query.Append(@"SELECT * FROM VAT_Temp_CostDetail WHERE VAB_OrderLine_ID = " + invoiceline.GetVAB_OrderLine_ID() +
                                        " AND VAM_Inv_InOutLine_ID = " + matchInoutLine.GetVAM_Inv_InOutLine_ID() + " AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID());
                                    DataSet ds1 = DB.ExecuteDataset(query.ToString(), null, trxName);
                                    if (ds1 != null && ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                                    {
                                        for (int k = 0; k < ds1.Tables[0].Rows.Count; k++)
                                        {
                                            // change 9-5-2016
                                            // handle partial payment
                                            X_VAT_Temp_CostDetail tempCostDetail = new X_VAT_Temp_CostDetail(invoiceline.GetCtx(), Util.GetValueOfInt(ds1.Tables[0].Rows[k]["VAT_Temp_CostDetail_ID"]), trxName);
                                            query.Clear();
                                            query.Append("SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostQueue WHERE VAM_ProductCostQueue_ID = " + Util.GetValueOfInt(ds1.Tables[0].Rows[k]["VAM_ProductCostQueue_ID"]));
                                            costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));
                                            costElement = new MVAMProductCostElement(ctx, costingElementId, trxName);

                                            price = MVAMProductCostQueue.CalculateCostQueuePrice(invoice, invoiceline, tempCostDetail.GetAmt(), acctSchema, VAF_Client_ID, VAF_Org_ID, costElement, trxName, client.IsCostImmediate(), optionalstr, matchInoutLine);
                                            //sql = "UPDATE VAM_ProductCostQueue SET CurrentCostPrice = " + cd.GetAmt() / cd.GetQty() + " WHERE VAM_ProductCostQueue_ID = " + Util.GetValueOfInt(ds1.Tables[0].Rows[k]["VAM_ProductCostQueue_ID"]);
                                            query.Clear();
                                            query.Append("UPDATE VAM_ProductCostQueue SET CurrentCostPrice = " + price + " WHERE VAM_ProductCostQueue_ID = " + Util.GetValueOfInt(ds1.Tables[0].Rows[k]["VAM_ProductCostQueue_ID"]));
                                            DB.ExecuteQuery(query.ToString(), null, trxName);
                                        }
                                    }
                                    ds1.Dispose();
                                }
                                #endregion
                            }
                        }
                        #endregion

                        #region update "Product Cost"
                        if (cd != null)
                        {
                            if (windowName == "Physical Inventory" || windowName == "Internal Use Inventory")
                            {
                                result = cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, inventoryLine.GetVAF_Org_ID(), optionalStrCd: optionalstr);
                                if (!result)
                                {
                                    if (optionalstr != "window")
                                    {
                                        trxName.Rollback();
                                    }
                                    else
                                    {
                                        DB.ExecuteQuery("DELETE FROM VAM_ProductCostDetail WHERE VAM_ProductCostDetail_ID = " + cd.GetVAM_ProductCostDetail_ID(), null, trxName);
                                        DB.ExecuteQuery("DELETE FROM VAM_ProductCostQueue WHERE VAM_ProductCostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                    }
                                    _log.Severe("Error occured during UpdateProductCost for VAM_InventoryLine_ID = " + inventoryLine.GetVAM_InventoryLine_ID());
                                    return false;
                                }
                            }
                            else if (windowName == "AssetDisposal")
                            {
                                result = cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, Util.GetValueOfInt(po.Get_Value("VAF_Org_ID")), optionalStrCd: optionalstr);
                                if (!result)
                                {
                                    if (optionalstr != "window")
                                    {
                                        trxName.Rollback();
                                    }
                                    else
                                    {
                                        DB.ExecuteQuery("DELETE FROM VAM_ProductCostDetail WHERE VAM_ProductCostDetail_ID = " + cd.GetVAM_ProductCostDetail_ID(), null, trxName);
                                        DB.ExecuteQuery("DELETE FROM VAM_ProductCostQueue WHERE VAM_ProductCostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                    }
                                    _log.Severe("Error occured during UpdateProductCost for VAFAM_AssetDisposal_ID = " + Util.GetValueOfInt(po.Get_Value("VAFAM_AssetDisposal_ID")));
                                    return false;
                                }
                            }
                            else if (windowName == "Production Execution" || windowName.Equals("PE-FinishGood"))
                            {
                                result = cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, Util.GetValueOfInt(po.Get_Value("VAF_Org_ID")), optionalStrCd: optionalstr);
                                if (!result)
                                {
                                    if (optionalstr == "process")
                                    {
                                        trxName.Rollback();
                                    }
                                    else
                                    {
                                        DB.ExecuteQuery("DELETE FROM VAM_ProductCostDetail WHERE VAM_ProductCostDetail_ID = " + cd.GetVAM_ProductCostDetail_ID(), null, trxName);
                                        DB.ExecuteQuery("DELETE FROM VAM_ProductCostQueue WHERE VAM_ProductCostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                    }
                                    _log.Severe("Error occured during UpdateProductCost for VAM_InventoryLine_ID = " + inventoryLine.GetVAM_InventoryLine_ID());
                                    return false;
                                }
                            }
                            else if (windowName == "Material Receipt" || windowName == "Customer Return" || windowName == "Shipment" || windowName == "Return To Vendor")
                            {
                                result = cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, inoutline.GetVAF_Org_ID(), optionalStrCd: optionalstr);
                                if (!result)
                                {
                                    if (optionalstr != "window")
                                    {
                                        trxName.Rollback();
                                    }
                                    else
                                    {
                                        DB.ExecuteQuery("DELETE FROM VAM_ProductCostDetail WHERE VAM_ProductCostDetail_ID = " + cd.GetVAM_ProductCostDetail_ID(), null, trxName);
                                        DB.ExecuteQuery("DELETE FROM VAM_ProductCostQueue WHERE VAM_ProductCostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                    }
                                    _log.Severe("Error occured during UpdateProductCost for VAM_Inv_InOutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID());
                                    return false;
                                }
                            }
                            else if (windowName == "Inventory Move")
                            {
                                if (pca != null)
                                {
                                    if (pca.GetCostingLevel() == "C" || pca.GetCostingLevel() == "B")
                                    {
                                        VAF_Org_ID = 0;
                                    }
                                    else if ((string.IsNullOrEmpty(pca.GetCostingLevel())) && (acctSchema.GetCostingLevel() == "C" || acctSchema.GetCostingLevel() == "B"))
                                    {
                                        VAF_Org_ID = 0;
                                    }
                                }
                                else if (acctSchema.GetCostingLevel() == "C" || acctSchema.GetCostingLevel() == "B")
                                {
                                    VAF_Org_ID = 0;
                                }
                                if (VAF_Org_ID != 0)
                                {
                                    result = cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, VAF_Org_ID, optionalStrCd: optionalstr); // for destination warehouse
                                    if (!result)
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        _log.Severe("Error occured during UpdateProductCost for VAM_InvTrf_Line_ID = " + cd.GetVAM_InvTrf_Line_ID());
                                        return false;
                                    }

                                    result = cdSourceWarehouse.UpdateProductCost(windowName, cdSourceWarehouse, acctSchema, product, M_ASI_ID, movementline.GetVAF_Org_ID(), optionalStrCd: optionalstr); // for source warehouse org
                                    if (!result)
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        else
                                        {
                                            DB.ExecuteQuery("DELETE FROM VAM_ProductCostDetail WHERE VAM_ProductCostDetail_ID = " + cd.GetVAM_ProductCostDetail_ID(), null, trxName);
                                            DB.ExecuteQuery("DELETE FROM VAM_ProductCostQueue WHERE VAM_ProductCostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                        }
                                        _log.Severe("Error occured during UpdateProductCost fot VAM_InvTrf_Line_ID = " + cd.GetVAM_InvTrf_Line_ID());
                                        return false;
                                    }

                                    // change 7-4-2016
                                    // for handling and calculating Cost Combination
                                    cdSourceWarehouse.CreateCostForCombination(cdSourceWarehouse, acctSchema, product, M_ASI_ID, 0, windowName, optionalStrcc: optionalstr);
                                    //end
                                }
                            }
                            else if ((windowName == "Invoice(Vendor)" && !isLandedCostAllocation) || windowName == "Invoice(Customer)" || windowName == "Invoice(Vendor)-Return")
                            {
                                if ((windowName != "Invoice(Vendor)-Return" && invoiceline.GetVAB_OrderLine_ID() > 0) || (windowName == "Invoice(Vendor)-Return" && invoiceline.GetVAM_Inv_InOutLine_ID() > 0))
                                {
                                    result = cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, invoiceline.GetVAF_Org_ID(), optionalStrCd: optionalstr);
                                    if (!result)
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        else
                                        {
                                            DB.ExecuteQuery("DELETE FROM VAM_ProductCostDetail WHERE VAM_ProductCostDetail_ID = " + cd.GetVAM_ProductCostDetail_ID(), null, trxName);
                                            DB.ExecuteQuery("DELETE FROM VAM_ProductCostQueue WHERE VAM_ProductCostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                        }
                                        _log.Severe("Error occured during UpdateProductCost for VAB_InvoiceLine_ID = " + invoiceline.GetVAB_InvoiceLine_ID());
                                        return false;
                                    }
                                }
                                else if (invoiceline.GetVAB_OrderLine_ID() == 0 && invoiceline.GetVAM_Inv_InOutLine_ID() == 0)
                                {
                                    // get costing level
                                    if (String.IsNullOrEmpty(costLevel))
                                    {
                                        costLevel = (MProductCategory.Get(product.GetCtx(), product.GetVAM_ProductCategory_ID())).GetCostingLevel();
                                        if (String.IsNullOrEmpty(costLevel))
                                        {
                                            costLevel = acctSchema.GetCostingLevel();
                                        }
                                    }
                                    // when costing levele = warehouse or Warehouse + batch then not to calculate cost against indepenedent APC
                                    if (!(costLevel == MProductCategory.COSTINGLEVEL_Warehouse || costLevel == MProductCategory.COSTINGLEVEL_WarehousePlusBatch))
                                    {
                                        // in case of independent AP credit memo, accumulation amt reduce and current cost of AV. PO/Invoice will be calculated
                                        // discount is given only when document type having setting as "Treat As Discount" = True
                                        MVABDocTypes docType = new MVABDocTypes(ctx, invoice.GetVAB_DocTypesTarget_ID(), trxName);
                                        if (docType.GetDocBaseType() == "APC" && docType.IsTreatAsDiscount())
                                        {
                                            result = cd.UpdateProductCost("Invoice(APC)", cd, acctSchema, product, M_ASI_ID, invoiceline.GetVAF_Org_ID(), optionalStrCd: optionalstr);
                                            if (!result)
                                            {
                                                if (optionalstr != "window")
                                                {
                                                    trxName.Rollback();
                                                }
                                                else
                                                {
                                                    DB.ExecuteQuery("DELETE FROM VAM_ProductCostDetail WHERE VAM_ProductCostDetail_ID = " + cd.GetVAM_ProductCostDetail_ID(), null, trxName);
                                                    DB.ExecuteQuery("DELETE FROM VAM_ProductCostQueue WHERE VAM_ProductCostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                                }
                                                _log.Severe("Error occured during UpdateProductCost for VAB_InvoiceLine_ID = " + invoiceline.GetVAB_InvoiceLine_ID());
                                                return false;
                                            }
                                        }
                                    }
                                }
                            }

                            #endregion

                            // change 7-4-2016
                            // for handling and calculating Cost Combination
                            if (windowName == "Invoice(Vendor)" && invoiceline.GetVAB_OrderLine_ID() == 0 && invoiceline.GetVAM_Inv_InOutLine_ID() == 0)
                            {
                                // cost element detail not to be calculated when Invoice(Vendor) is alone.
                                // But in case of APC, we have to calculate Cost Element detail and cost combination both
                                //discount is given only when document type having setting as "Treat As Discount" = True
                                MVABDocTypes docType = new MVABDocTypes(ctx, invoice.GetVAB_DocTypesTarget_ID(), trxName);
                                if (docType.IsTreatAsDiscount() && docType.GetDocBaseType() == "APC" && cd != null)
                                {
                                    // get costing level
                                    if (String.IsNullOrEmpty(costLevel))
                                    {
                                        costLevel = (MProductCategory.Get(product.GetCtx(), product.GetVAM_ProductCategory_ID())).GetCostingLevel();
                                        if (String.IsNullOrEmpty(costLevel))
                                        {
                                            costLevel = acctSchema.GetCostingLevel();
                                        }
                                    }
                                    // when costing levele = warehouse or Warehouse + batch then not to calculate cost against indepenedent APC
                                    if (!(costLevel == MProductCategory.COSTINGLEVEL_Warehouse || costLevel == MProductCategory.COSTINGLEVEL_WarehousePlusBatch))
                                    {
                                        cd.CreateCostForCombination(cd, acctSchema, product, M_ASI_ID, 0, windowName, optionalStrcc: optionalstr);
                                    }
                                }
                            }
                            else
                                if (cd != null && windowName != "Invoice(Customer)" && !isLandedCostAllocation)
                            {
                                cd.CreateCostForCombination(cd, acctSchema, product, M_ASI_ID, 0, windowName, optionalStrcc: optionalstr);
                            }
                            //end
                        }
                    }
                    if (ds != null)
                        ds.Dispose();
                }
            }
            catch (Exception ex)
            {
                _log.Severe("Error Occured during costing " + ex.ToString());
                if (ds != null)
                {
                    ds.Dispose();
                }
                if (optionalstr != "window")
                {
                    trxName.Rollback();
                }
                return false;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static bool CreateProductCostsDetails(Ctx ctx, int VAF_Client_ID, int VAF_Org_ID, MProduct product, int M_ASI_ID,
                     string windowName, MVAMInventoryLine inventoryLine, MVAMInvInOutLine inoutline, MVAMInvTrfLine movementline,
                     MVABInvoiceLine invoiceline, Decimal Price, Decimal Qty, Trx trxName, int[] acctSchemaRecord, out string conversionNotFound)
        {
            MVABAccountBook acctSchema = null;
            dynamic pca = null;
            MVAMProductCostElement costElement = null;
            MProduct productLca = null;
            MVABOrderLine orderline = null;
            MVABOrder order = null;
            MVABInvoice invoice = null;
            MVAMInventoryTransfer movement = null;
            MVAMInventory inventory = null;
            MVAMProductCostDetail cd = null;
            MVAMProductCostDetail cdSourceWarehouse = null;
            MVAMProductCost cost = null;
            MVAMInvInOut inout = null;
            Decimal price = 0;
            Decimal plPrice = 0; // price list price
            Decimal cmPrice = 0; // costing method price
            Decimal receivedPrice = Price;
            Decimal receivedQty = Qty;
            String costingMethod = string.Empty;
            int costingElementId = 0;
            bool isLandedCostAllocation = false;
            int orderLineId = 0;
            int costDetailId = 0;
            bool isPriceFromProductPrice = false;
            int VAF_Org_ID2 = VAF_Org_ID;
            DataSet ds = null;
            string cl = "";
            string isMatchFromForm = "N";
            string handlingWindowName = windowName;
            bool result = true;
            int MatchPO_OrderLineId = 0;
            StringBuilder query = new StringBuilder();
            conversionNotFound = "";
            string costQueuseIds = null;
            try
            {
                if (product != null)
                {
                    _log.Info("costing Calculation Start for window = " + windowName + " and product = " + product.GetVAM_Product_ID() + " AND ASI " + M_ASI_ID);
                }
                else
                {
                    _log.Info("costing Calculation Start for window = " + windowName);
                }

                if (acctSchemaRecord.Length > 0)
                {
                    for (int i = 0; i < acctSchemaRecord.Length; i++)
                    {
                        Price = receivedPrice;
                        Qty = receivedQty;
                        VAF_Org_ID = VAF_Org_ID2;
                        acctSchema = new MVABAccountBook(ctx, acctSchemaRecord[i], trxName);
                        if (product != null)
                        {
                            #region Costing Level
                            pca = new MProductCategory(ctx, product.GetVAM_ProductCategory_ID(), null);
                            if (windowName != "Inventory Move")
                            {
                                if (product.GetProductType() == "E")
                                {
                                    // for expense type product, we didnt check costing level
                                    // bcz if we define costing level as client and try to calculate landed cost whose costing level is Organization
                                    // then system calculate its costing in (*) org but this product costing level is Org. 
                                }
                                else
                                {
                                    if (pca != null)
                                    {
                                        if (pca.GetCostingLevel() == "C" || pca.GetCostingLevel() == "B")
                                        {
                                            VAF_Org_ID = 0;
                                        }
                                        else if ((string.IsNullOrEmpty(pca.GetCostingLevel())) && (acctSchema.GetCostingLevel() == "C" || acctSchema.GetCostingLevel() == "B"))
                                        {
                                            VAF_Org_ID = 0;
                                        }
                                    }
                                    else if (acctSchema.GetCostingLevel() == "C" || acctSchema.GetCostingLevel() == "B")
                                    {
                                        VAF_Org_ID = 0;
                                    }
                                }
                            }
                            #endregion
                        }

                        if (windowName == "Material Receipt" || windowName == "Customer Return" || windowName == "Shipment" || windowName == "Return To Vendor")
                        {
                            inout = new MVAMInvInOut(ctx, inoutline.GetVAM_Inv_InOut_ID(), trxName);
                            if (inoutline.GetVAB_OrderLine_ID() > 0)
                            {
                                orderline = new MVABOrderLine(ctx, inoutline.GetVAB_OrderLine_ID(), trxName);
                                order = new MVABOrder(ctx, orderline.GetVAB_Order_ID(), trxName);
                                if (order.GetVAB_Currency_ID() != acctSchema.GetVAB_Currency_ID())
                                {
                                    Price = MVABExchangeRate.Convert(ctx, Price, order.GetVAB_Currency_ID(), acctSchema.GetVAB_Currency_ID(),
                                                                     order.GetDateAcct(), order.GetVAB_CurrencyType_ID(), VAF_Client_ID, VAF_Org_ID);
                                    if (Price == 0)
                                    {
                                        trxName.Rollback();
                                        conversionNotFound = inout.GetDocumentNo();
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                //28-4-2016
                                //get price based on costing method
                                // but VAM_ProductCost handled indivdually based on price
                                //Get Price from MVAMProductCost
                                price = MVAMProductCostQueue.CalculateCost(ctx, acctSchema, product, M_ASI_ID, VAF_Client_ID, VAF_Org_ID, 0);
                                Price = price * inoutline.GetMovementQty();
                                cmPrice = Price;
                                // if (Price != 0)
                                // {
                                Price = GetPrice(inoutline, acctSchema.GetVAB_Currency_ID());
                                //}
                                if (Price == 0)
                                {
                                    trxName.Rollback();
                                    conversionNotFound = inout.GetDocumentNo();
                                    return false;
                                }
                            }
                        }
                        else if (windowName == "Invoice(Vendor)" || windowName == "Invoice(Customer)" || windowName == "Match IV")
                        {
                            invoice = new MVABInvoice(ctx, invoiceline.GetVAB_Invoice_ID(), trxName);
                            if (invoice.GetVAB_Currency_ID() != acctSchema.GetVAB_Currency_ID())
                            {
                                Price = MVABExchangeRate.Convert(ctx, Price, invoice.GetVAB_Currency_ID(), acctSchema.GetVAB_Currency_ID(),
                                                                         invoice.GetDateAcct(), invoice.GetVAB_CurrencyType_ID(), VAF_Client_ID, VAF_Org_ID);

                                if (Price == 0)
                                {
                                    trxName.Rollback();
                                    conversionNotFound = invoice.GetDocumentNo();
                                    return false;
                                }
                            }
                        }

                        // check MR matached from PO through Form or not
                        // if yes, then call Match IV
                        if (windowName == "Invoice(Vendor)")
                        {
                            if (product != null)
                            {
                                query.Clear();
                                query.Append(@"SELECT IsMatchPOForm FROM VAM_MatchPO WHERE IsActive = 'Y' 
                                     AND VAM_Inv_InOutLine_ID = " + Util.GetValueOfInt(invoiceline.GetVAM_Inv_InOutLine_ID()) +
                                         " AND VAM_Product_ID = " + product.GetVAM_Product_ID());
                                isMatchFromForm = Util.GetValueOfString(DB.ExecuteScalar(query.ToString(), null, trxName));
                                if (isMatchFromForm == "Y")
                                {
                                    inoutline = new MVAMInvInOutLine(ctx, invoiceline.GetVAM_Inv_InOutLine_ID(), trxName);
                                    if (inoutline.GetVAM_Inv_InOutLine_ID() > 0)
                                    {
                                        M_ASI_ID = inoutline.GetVAM_PFeature_SetInstance_ID();
                                        windowName = "Match IV";
                                    }
                                }
                            }

                            if (invoiceline.GetVAB_OrderLine_ID() <= 0)
                            {
                                inoutline = new MVAMInvInOutLine(ctx, invoiceline.GetVAM_Inv_InOutLine_ID(), trxName);
                                if (inoutline.GetVAM_Inv_InOutLine_ID() > 0)
                                {
                                    M_ASI_ID = inoutline.GetVAM_PFeature_SetInstance_ID();
                                    windowName = "Match IV";
                                }
                            }
                        }

                        if (windowName == "Match PO")
                        {
                            #region Match PO
                            decimal MRPriceAvPo = 0;
                            decimal MRPriceLastPO = 0;
                            bool isRecordFromForm = false;

                            //if (countFRPT > 0 && acctSchema.GetFRPT_LocAcct_ID() > 0)
                            //{
                            pca = MProductCategory.Get(product.GetCtx(), product.GetVAM_ProductCategory_ID());
                            if (pca != null)
                            {
                                cl = pca.GetCostingLevel();
                            }
                            else
                            {
                                cl = acctSchema.GetCostingLevel();
                            }
                            //}
                            //else
                            //{
                            //    pca = MProductCategoryAcct.Get(product.GetCtx(),
                            //                  product.GetVAM_ProductCategory_ID(), acctSchema.GetVAB_AccountBook_ID(), product.Get_TrxName());
                            //    if (pca != null)
                            //    {
                            //        cl = pca.GetCostingLevel();
                            //    }
                            //    else
                            //    {
                            //        cl = acctSchema.GetCostingLevel();
                            //    }
                            //}

                            inout = new MVAMInvInOut(ctx, inoutline.GetVAM_Inv_InOut_ID(), trxName);
                            orderLineId = Util.GetValueOfInt(Price); // here price parametr contain Orderline id from controller
                            orderline = new MVABOrderLine(ctx, orderLineId, trxName);

                            order = new MVABOrder(ctx, orderline.GetVAB_Order_ID(), trxName);
                            if (order.GetVAB_Currency_ID() != acctSchema.GetVAB_Currency_ID())
                            {
                                Price = MVABExchangeRate.Convert(ctx, Decimal.Divide(orderline.GetLineNetAmt(), orderline.GetQtyOrdered()), order.GetVAB_Currency_ID(), acctSchema.GetVAB_Currency_ID(),
                                                                 order.GetDateAcct(), order.GetVAB_CurrencyType_ID(), VAF_Client_ID, VAF_Org_ID);
                                if (Price == 0)
                                {
                                    trxName.Rollback();
                                    conversionNotFound = order.GetDocumentNo();
                                    _log.Severe("Currency Conversion not found-> MatchPO. Order No is " + order.GetDocumentNo());
                                    return false;
                                }
                            }
                            else
                            {
                                Price = Decimal.Divide(orderline.GetLineNetAmt(), orderline.GetQtyOrdered());
                            }

                            query.Clear();
                            query.Append("SELECT VAM_ProductCostDetail_ID FROM VAM_ProductCostDetail WHERE VAM_Inv_InOutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID() +
                                 " AND VAB_InvoiceLine_id IS NULL  AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID());
                            costDetailId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));
                            cd = new MVAMProductCostDetail(ctx, costDetailId, trxName);

                            if (costDetailId == 0)
                            {
                                cd.SetClientOrg(VAF_Client_ID, VAF_Org_ID);
                                cd.SetVAM_Product_ID(product.GetVAM_Product_ID());
                                cd.SetVAM_Inv_InOutLine_ID(inoutline.GetVAM_Inv_InOutLine_ID());
                                cd.SetVAM_PFeature_SetInstance_ID(M_ASI_ID);
                                cd.SetVAB_AccountBook_ID(acctSchema.GetVAB_AccountBook_ID());
                            }

                            // MR complete before matching
                            if (inoutline.IsCostCalculated() && inout.IsCostCalculated())
                            {
                                //                                sql = @"SELECT VAB_OrderLine_Id FROM VAM_MatchPO WHERE IsActive = 'Y' AND VAM_MatchPO_ID = 
                                //                                                                 (SELECT MIN(VAM_MatchPO_ID) FROM VAM_MatchPO WHERE IsActive = 'Y' AND VAM_Inv_InOutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID() + ") ";
                                //                                MatchPO_OrderLineId = Util.GetValueOfInt(DB.ExecuteScalar(sql));
                                //change 4-5-2016
                                query.Clear();
                                if (cl != "B")
                                {
                                    query.Append(@"SELECT  ROUND(Amt/Qty , 4) as currentCostAmount  FROM VAM_ProductCostElementDetail ced 
                                                                where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @" 
                                                                 AND ced.VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() + @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) = 0" +
                                       " AND ced.VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) = 0 AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 and 
                                                                 ced.VAM_ProductCostElement_ID = (SELECT VAM_ProductCostElement_ID from VAM_ProductCostElement where costingMethod = 'A' AND IsActive = 'Y' AND VAF_Client_ID = " + inoutline.GetVAF_Client_ID() + " ) AND ced.VAF_Client_ID = " + inoutline.GetVAF_Client_ID());
                                }
                                else
                                {
                                    query.Append(@"SELECT  ROUND(Amt/Qty , 4) as currentCostAmount  FROM VAM_ProductCostElementDetail ced 
                                                                where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @" 
                                                                 AND ced.VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() + @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) = " + M_ASI_ID + @" 
                                                                 AND ced.VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) = 0 AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 and 
                                                                 ced.VAM_ProductCostElement_ID = (SELECT VAM_ProductCostElement_ID from VAM_ProductCostElement where costingMethod = 'A' AND IsActive = 'Y' AND VAF_Client_ID = " + inoutline.GetVAF_Client_ID() + " ) AND ced.VAF_Client_ID = " + inoutline.GetVAF_Client_ID());
                                }
                                MRPriceAvPo = Util.GetValueOfDecimal(DB.ExecuteScalar(query.ToString()));

                                query.Clear();
                                if (cl != "B")
                                {
                                    query.Append(@"SELECT  ROUND(Amt/Qty , 4) as currentCostAmount  FROM VAM_ProductCostElementDetail ced 
                                                                where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @" 
                                                                 AND ced.VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() + @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) = 0 " +
                                         " AND ced.VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) = 0 AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 and 
                                                                 ced.VAM_ProductCostElement_ID = (SELECT VAM_ProductCostElement_ID from VAM_ProductCostElement where costingMethod = 'p' AND IsActive = 'Y' AND VAF_Client_ID = " + inoutline.GetVAF_Client_ID() + " ) AND ced.VAF_Client_ID = " + inoutline.GetVAF_Client_ID());
                                }
                                else
                                {
                                    query.Append(@"SELECT  ROUND(Amt/Qty , 4) as currentCostAmount  FROM VAM_ProductCostElementDetail ced 
                                                                where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @" 
                                                                 AND ced.VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() + @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) = " + M_ASI_ID + @" 
                                                                 AND ced.VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) = 0 AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 and 
                                                                 ced.VAM_ProductCostElement_ID = (SELECT VAM_ProductCostElement_ID from VAM_ProductCostElement where costingMethod = 'p' AND IsActive = 'Y' AND VAF_Client_ID = " + inoutline.GetVAF_Client_ID() + " ) AND ced.VAF_Client_ID = " + inoutline.GetVAF_Client_ID());
                                }
                                MRPriceLastPO = Util.GetValueOfDecimal(DB.ExecuteScalar(query.ToString()));
                                if (MRPriceLastPO == 0)
                                {
                                    MRPriceLastPO = MRPriceAvPo;
                                }
                            }
                            //end
                            // match PO with MR before MR completeion 1st record
                            else if (!inoutline.IsCostCalculated())
                            {
                                if (!CreateCostQueueForMatchPO(ctx, acctSchema, product, M_ASI_ID, VAF_Client_ID, VAF_Org_ID,
                                                              Price, inoutline.GetMovementQty(), inoutline, trxName, 0))
                                {
                                    _log.Severe("Error occured during craetion of cost queue -> MatchPO. VAM_Inv_InOutLine_ID is " + inoutline.GetVAM_Inv_InOutLine_ID());
                                    return false;
                                }
                                else
                                {
                                    isRecordFromForm = true;
                                }
                            }
                            // match PO with MR but one already matched with that
                            else if (inoutline.IsCostCalculated())
                            {
                                MRPriceAvPo = 0;
                                MRPriceLastPO = 0;
                                query.Clear();
                                query.Append(@"SELECT amt FROM VAT_Temp_CostDetail WHERE IsActive = 'Y' AND VAF_Client_ID = " + VAF_Client_ID +
                                             " AND VAF_Org_ID = " + VAF_Org_ID + " AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                             " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND isRecordFromForm = 'Y' " +
                                             " AND VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID());
                                MRPriceAvPo = Util.GetValueOfDecimal(DB.ExecuteScalar(query.ToString(), null, trxName));
                                MRPriceLastPO = MRPriceAvPo;
                                if (MRPriceAvPo != 0)
                                {
                                    isRecordFromForm = true;
                                }
                            }

                            if (cd.GetVAB_OrderLine_ID() == 0)
                            {
                                // update VAM_ProductCost Accumulative Amount to be reduced for Av. PO and Last PO
                                query.Clear();
                                query.Append("SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE  VAF_Client_ID=" + VAF_Client_ID + " AND IsActive='Y' AND CostElementType='M'  AND CostingMethod = 'A'");
                                int costElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));

                                costElement = new MVAMProductCostElement(ctx, costElementId, null);

                                // Av. PO
                                if (cl != "B")
                                {
                                    cost = MVAMProductCost.Get(product, 0, acctSchema, VAF_Org_ID, costElementId);
                                }
                                else
                                {
                                    cost = MVAMProductCost.Get(product, M_ASI_ID, acctSchema, VAF_Org_ID, costElementId);
                                }
                                if (!isRecordFromForm && inoutline.IsCostCalculated())
                                    cost.SetCumulatedAmt(Decimal.Subtract(cost.GetCumulatedAmt(), (MRPriceAvPo * Qty)));
                                if (!cost.Save())
                                {
                                    trxName.Rollback();
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    _log.Severe("Error occured during saving a record , VAM_Inv_InOutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID() + " in cost detail -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                    return false;
                                }

                                cd.SetVAB_OrderLine_ID(orderLineId);
                                cd.SetQty(Qty);
                                cd.SetAmt(Decimal.Round(Decimal.Multiply(Price, Qty), acctSchema.GetCostingPrecision()));
                                if (!cd.Save(trxName))
                                {
                                    trxName.Rollback();
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    _log.Severe("Error occured during saving a record, VAM_Inv_InOutLine_Id = " + inoutline.GetVAM_Inv_InOutLine_ID() + " in cost detail -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                    return false;
                                }

                                //cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), cd.GetAmt()));
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Add(cd.GetAmt(),
                                                       Decimal.Round(Decimal.Divide(Decimal.Multiply(cd.GetAmt(), costElement.GetSurchargePercentage()), 100), acctSchema.GetCostingPrecision()))));

                                if (isRecordFromForm || !inout.IsCostCalculated())
                                {
                                    // if cost not calculated for MR then add quantity
                                    cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), cd.GetQty()));
                                    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), cd.GetQty()));
                                }

                                if (cost.GetCumulatedQty() != 0)
                                {
                                    cost.SetCurrentCostPrice(Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), acctSchema.GetCostingPrecision()));
                                }
                                else
                                {
                                    cost.SetCurrentCostPrice(0);
                                }
                                if (!cost.Save())
                                {
                                    trxName.Rollback();
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    _log.Severe("Error occured during saving a record, VAM_Inv_InOutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID() + " in M-Cost -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                    return false;
                                }
                                else
                                {
                                    if (cl != "B")
                                    {
                                        MVAMProductCostElementDetail.CreateCostElementDetail(ctx, VAF_Client_ID, VAF_Org_ID, product, 0,
                                                             acctSchema, costElementId, windowName, cd, Decimal.Round(Decimal.Multiply(cost.GetCurrentCostPrice(), Qty), acctSchema.GetCostingPrecision()), Qty);
                                    }
                                    else
                                    {
                                        MVAMProductCostElementDetail.CreateCostElementDetail(ctx, VAF_Client_ID, VAF_Org_ID, product, M_ASI_ID,
                                                                 acctSchema, costElementId, windowName, cd, Decimal.Round(Decimal.Multiply(cost.GetCurrentCostPrice(), Qty), acctSchema.GetCostingPrecision()), Qty);
                                    }
                                }

                                // last po
                                query.Clear();
                                query.Append("SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE  VAF_Client_ID=" + VAF_Client_ID + " AND IsActive='Y' AND CostElementType='M'  AND CostingMethod = 'p'");
                                costElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));

                                costElement = new MVAMProductCostElement(ctx, costElementId, null);

                                if (cl != "B")
                                {
                                    cost = MVAMProductCost.Get(product, 0, acctSchema, VAF_Org_ID, costElementId);
                                }
                                else
                                {
                                    cost = MVAMProductCost.Get(product, M_ASI_ID, acctSchema, VAF_Org_ID, costElementId);
                                }
                                // reduce 
                                if (!isRecordFromForm && inoutline.IsCostCalculated())
                                    cost.SetCumulatedAmt(Decimal.Subtract(cost.GetCumulatedAmt(), (MRPriceLastPO * Qty))); // reduce amount of only MR

                                //cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), cd.GetAmt())); // add amount of Match PO
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Add(cd.GetAmt(),
                                                       Decimal.Round(Decimal.Divide(Decimal.Multiply(cd.GetAmt(), costElement.GetSurchargePercentage()), 100), acctSchema.GetCostingPrecision()))));

                                if (isRecordFromForm || !inout.IsCostCalculated())
                                {
                                    // if cost not calculated for MR then add quantity
                                    cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), cd.GetQty()));
                                    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), cd.GetQty()));
                                }

                                //cost.SetCurrentCostPrice(Decimal.Round(Price, acctSchema.GetCostingPrecision()));
                                cost.SetCurrentCostPrice(Decimal.Round(Decimal.Add(Price, Decimal.Divide(Decimal.Multiply(Price, costElement.GetSurchargePercentage()), 100)), acctSchema.GetCostingPrecision()));

                                if (!cost.Save())
                                {
                                    trxName.Rollback();
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    _log.Severe("Error occured during saving a record, N_InoutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID() + " in M-Cost -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                    return false;
                                }
                                else
                                {
                                    if (cl != "B")
                                    {
                                        MVAMProductCostElementDetail.CreateCostElementDetail(ctx, VAF_Client_ID, VAF_Org_ID, product, 0,
                                                             acctSchema, costElementId, windowName, cd, Decimal.Round(Decimal.Multiply(cost.GetCurrentCostPrice(), Qty), acctSchema.GetCostingPrecision()), Qty);
                                    }
                                    else
                                    {
                                        MVAMProductCostElementDetail.CreateCostElementDetail(ctx, VAF_Client_ID, VAF_Org_ID, product, M_ASI_ID,
                                                             acctSchema, costElementId, windowName, cd, Decimal.Round(Decimal.Multiply(cost.GetCurrentCostPrice(), Qty), acctSchema.GetCostingPrecision()), Qty);
                                    }
                                }
                            }
                            else
                            {
                                cd = new MVAMProductCostDetail(acctSchema, VAF_Org_ID, product.GetVAM_Product_ID(), M_ASI_ID,
                                                      0, Decimal.Round(Decimal.Multiply(Qty, Price), acctSchema.GetCostingPrecision()), Qty, null, trxName);
                                cd.SetVAM_Inv_InOutLine_ID(inoutline.GetVAM_Inv_InOutLine_ID());
                                cd.SetVAB_OrderLine_ID(orderLineId);
                                if (!cd.Save(trxName))
                                {
                                    trxName.Rollback();
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    _log.Severe("Error occured during saving a record, VAM_Inv_InOutLine_Id = " + inoutline.GetVAM_Inv_InOutLine_ID() + " in Cost-Detail -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                    return false;
                                }

                                // Av. PO
                                query.Clear();
                                query.Append("SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE  VAF_Client_ID=" + VAF_Client_ID + " AND IsActive='Y' AND CostElementType='M'  AND CostingMethod = 'A'");
                                int costElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));

                                costElement = new MVAMProductCostElement(ctx, costElementId, null);

                                if (cl != "B")
                                {
                                    cost = MVAMProductCost.Get(product, 0, acctSchema, VAF_Org_ID, costElementId);
                                }
                                else
                                {
                                    cost = MVAMProductCost.Get(product, M_ASI_ID, acctSchema, VAF_Org_ID, costElementId);
                                }
                                if (!isRecordFromForm && inoutline.IsCostCalculated())
                                    cost.SetCumulatedAmt(Decimal.Subtract(cost.GetCumulatedAmt(), (MRPriceAvPo * Qty))); // reduce amount of only MR

                                //cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), cd.GetAmt()));
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Add(cd.GetAmt(),
                                                      Decimal.Round(Decimal.Divide(Decimal.Multiply(cd.GetAmt(), costElement.GetSurchargePercentage()), 100), acctSchema.GetCostingPrecision()))));

                                if (isRecordFromForm || !inout.IsCostCalculated())
                                {
                                    // if cost not calculated from MR then add quantity
                                    cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), cd.GetQty()));
                                    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), cd.GetQty()));
                                }
                                if (cost.GetCumulatedQty() != 0)
                                {
                                    cost.SetCurrentCostPrice(Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), acctSchema.GetCostingPrecision()));
                                }
                                else
                                {
                                    cost.SetCurrentCostPrice(0);
                                }
                                if (!cost.Save())
                                {
                                    trxName.Rollback();
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    _log.Severe("Error occured during saving a record, VAM_Inv_InOutLine_Id = " + inoutline.GetVAM_Inv_InOutLine_ID() + " in M-Cost -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                    return false;
                                }
                                else
                                {
                                    MVAMProductCostElementDetail.CreateCostElementDetail(ctx, VAF_Client_ID, VAF_Org_ID, product, M_ASI_ID,
                                                         acctSchema, costElementId, windowName, cd, Decimal.Round(Decimal.Multiply(cost.GetCurrentCostPrice(), Qty), acctSchema.GetCostingPrecision()), Qty);
                                }

                                // last po
                                query.Clear();
                                query.Append("SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE  VAF_Client_ID=" + VAF_Client_ID + " AND IsActive='Y' AND CostElementType='M'  AND CostingMethod = 'p'");
                                costElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));

                                costElement = new MVAMProductCostElement(ctx, costElementId, null);

                                if (cl != "B")
                                {
                                    cost = MVAMProductCost.Get(product, 0, acctSchema, VAF_Org_ID, costElementId);
                                }
                                else
                                {
                                    cost = MVAMProductCost.Get(product, M_ASI_ID, acctSchema, VAF_Org_ID, costElementId);
                                }
                                if (!isRecordFromForm && inoutline.IsCostCalculated())
                                    cost.SetCumulatedAmt(Decimal.Subtract(cost.GetCumulatedAmt(), (MRPriceLastPO * Qty))); // reduce amount of only MR

                                //cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), cd.GetAmt()));
                                cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), Decimal.Add(cd.GetAmt(),
                                                       Decimal.Round(Decimal.Divide(Decimal.Multiply(cd.GetAmt(), costElement.GetSurchargePercentage()), 100), acctSchema.GetCostingPrecision()))));

                                if (isRecordFromForm || !inout.IsCostCalculated())
                                {
                                    // if cost not calculated for MR then add quantity
                                    cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), cd.GetQty()));
                                    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), cd.GetQty()));
                                }

                                //cost.SetCurrentCostPrice(Decimal.Round(Price, acctSchema.GetCostingPrecision()));
                                cost.SetCurrentCostPrice(Decimal.Round(Decimal.Add(Price, Decimal.Divide(Decimal.Multiply(Price, costElement.GetSurchargePercentage()), 100)), acctSchema.GetCostingPrecision()));

                                if (!cost.Save())
                                {
                                    trxName.Rollback();
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    _log.Severe("Error occured during saving a record, VAM_Inv_InOutLine_Id = " + inoutline.GetVAM_Inv_InOutLine_ID() + " in M-Cost -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                    return false;
                                }
                                else
                                {
                                    MVAMProductCostElementDetail.CreateCostElementDetail(ctx, VAF_Client_ID, VAF_Org_ID, product, M_ASI_ID,
                                                         acctSchema, costElementId, windowName, cd, Decimal.Round(Decimal.Multiply(cost.GetCurrentCostPrice(), Qty), acctSchema.GetCostingPrecision()), Qty);
                                }
                            }
                            #endregion
                        }

                        if (windowName == "Match IV")
                        {
                            #region Match Invoice
                            decimal MRPrice = 0;
                            decimal MRPriceLifo = 0;
                            decimal MRPriceFifo = 0;
                            decimal matchInvoicePrice = 0;
                            int ceFifo = 0;
                            int ceLifo = 0;

                            //if (countFRPT > 0 && acctSchema.GetFRPT_LocAcct_ID() > 0)
                            //{
                            pca = MProductCategory.Get(product.GetCtx(), product.GetVAM_ProductCategory_ID());
                            if (pca != null)
                            {
                                cl = pca.GetCostingLevel();
                            }
                            else
                            {
                                cl = acctSchema.GetCostingLevel();
                            }
                            //}
                            //else
                            //{
                            //    pca = MProductCategoryAcct.Get(product.GetCtx(),
                            //                  product.GetVAM_ProductCategory_ID(), acctSchema.GetVAB_AccountBook_ID(), product.Get_TrxName());
                            //    if (pca != null)
                            //    {
                            //        cl = pca.GetCostingLevel();
                            //    }
                            //    else
                            //    {
                            //        cl = acctSchema.GetCostingLevel();
                            //    }
                            //}
                            query.Clear();
                            query.Append("SELECT VAM_ProductCostDetail_ID FROM VAM_ProductCostDetail WHERE VAM_Inv_InOutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID() +
                                    " AND VAB_Orderline_id IS NULL  AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID());
                            costDetailId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));
                            cd = new MVAMProductCostDetail(ctx, costDetailId, trxName);

                            query.Clear();
                            query.Append(@"SELECT VAB_OrderLine_Id FROM VAM_MatchPO WHERE IsActive = 'Y' AND VAM_MatchPO_ID = 
                                     (SELECT MIN(VAM_MatchPO_ID) FROM VAM_MatchPO WHERE IsActive = 'Y' AND VAM_Inv_InOutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID() + ") ");
                            MatchPO_OrderLineId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString()));

                            if (cl != "B")
                            {
                                query.Clear();
                                query.Append(@"SELECT  Amt/Qty as currentCostAmount  FROM VAM_ProductCostElementDetail ced 
                                    where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @" 
                                     AND ced.VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() + @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) = 0 " +
                                     " AND ced.VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) = " + MatchPO_OrderLineId + @" AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 and 
                                     ced.VAM_ProductCostElement_ID = (SELECT VAM_ProductCostElement_ID from VAM_ProductCostElement where costingMethod = 'L' AND IsActive = 'Y' AND VAF_Client_ID = " + inoutline.GetVAF_Client_ID() + " ) AND ced.VAF_Client_ID = " + inoutline.GetVAF_Client_ID());
                            }
                            else
                            {
                                query.Clear();
                                query.Append(@"SELECT  Amt/Qty as currentCostAmount  FROM VAM_ProductCostElementDetail ced 
                                    where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @" 
                                     AND ced.VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() + @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) = " + M_ASI_ID + @" 
                                     AND ced.VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) = " + MatchPO_OrderLineId + @" AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 and 
                                     ced.VAM_ProductCostElement_ID = (SELECT VAM_ProductCostElement_ID from VAM_ProductCostElement where costingMethod = 'L' AND IsActive = 'Y' AND VAF_Client_ID = " + inoutline.GetVAF_Client_ID() + " ) AND ced.VAF_Client_ID = " + inoutline.GetVAF_Client_ID());
                            }
                            MRPriceLifo = Util.GetValueOfDecimal(DB.ExecuteScalar(query.ToString()));

                            if (cl != "B")
                            {
                                query.Clear();
                                query.Append(@"SELECT  Amt/Qty as currentCostAmount  FROM VAM_ProductCostElementDetail ced 
                                    where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @" 
                                     AND ced.VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() + @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) = 0 " +
                                     " AND ced.VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) = " + MatchPO_OrderLineId + @" AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 and 
                                     ced.VAM_ProductCostElement_ID = (SELECT VAM_ProductCostElement_ID from VAM_ProductCostElement where costingMethod = 'F' AND IsActive = 'Y' AND VAF_Client_ID = " + inoutline.GetVAF_Client_ID() + " ) AND ced.VAF_Client_ID = " + inoutline.GetVAF_Client_ID());
                            }
                            else
                            {
                                query.Clear();
                                query.Append(@"SELECT  Amt/Qty as currentCostAmount  FROM VAM_ProductCostElementDetail ced 
                                    where  ced.IsActive = 'Y' AND ced.VAM_Product_ID = " + product.GetVAM_Product_ID() + @" 
                                     AND ced.VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() + @" AND  NVL(ced.VAM_PFeature_SetInstance_ID , 0) = " + M_ASI_ID + @" 
                                     AND ced.VAM_Inv_InOutLine_ID =  " + inoutline.GetVAM_Inv_InOutLine_ID() + @" AND NVL(ced.VAB_OrderLIne_ID , 0) = " + MatchPO_OrderLineId + @" AND NVL(ced.VAB_InvoiceLine_ID , 0) = 0 and 
                                     ced.VAM_ProductCostElement_ID = (SELECT VAM_ProductCostElement_ID from VAM_ProductCostElement where costingMethod = 'F' AND IsActive = 'Y' AND VAF_Client_ID = " + inoutline.GetVAF_Client_ID() + " ) AND ced.VAF_Client_ID = " + inoutline.GetVAF_Client_ID());
                            }
                            MRPriceFifo = Util.GetValueOfDecimal(DB.ExecuteScalar(query.ToString()));

                            ceFifo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAM_ProductCostElement_ID from VAM_ProductCostElement where costingMethod = 'F' AND IsActive = 'Y' AND VAF_Client_ID = " + inoutline.GetVAF_Client_ID()));
                            ceLifo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAM_ProductCostElement_ID from VAM_ProductCostElement where costingMethod = 'L' AND IsActive = 'Y' AND VAF_Client_ID = " + inoutline.GetVAF_Client_ID()));

                            if (costDetailId > 0 && cd.GetVAB_InvoiceLine_ID() == 0)
                            {
                                // set amount and qty on cost detail from match receipt
                                cd.SetAmt(Price);
                                cd.SetQty(Qty);
                                cd.SetVAB_InvoiceLine_ID(invoiceline.GetVAB_InvoiceLine_ID());
                                cd.SetVAB_OrderLine_ID(invoiceline.GetVAB_OrderLine_ID());
                                if (!cd.Save(trxName))
                                {
                                    trxName.Rollback();
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    _log.Severe("Error occured during saving a record in M-Cost -> Match IV. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                    return false;
                                }

                                //get Cost Queue Record based on condition
                                query.Clear();
                                query.Append(@"SELECT * FROM VAT_Temp_CostDetail WHERE  VAM_Inv_InOutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID() +
                                              " AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID());
                                DataSet ds2 = DB.ExecuteDataset(query.ToString(), null, null);

                                // reduce cummulative amount , qty for all element
                                cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, invoiceline.GetVAF_Org_ID());

                                if (handlingWindowName == "Invoice(Vendor)")
                                {
                                    if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                    {
                                        for (int b = 0; b < ds2.Tables[0].Rows.Count; b++)
                                        {
                                            if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostQueue WHERE VAM_ProductCostQueue_ID = " + (Util.GetValueOfInt(ds2.Tables[0].Rows[b]["VAM_ProductCostQueue_ID"])))) == ceFifo)
                                            {
                                                MRPrice = MRPriceFifo;
                                                costElement = new MVAMProductCostElement(ctx, ceFifo, null);
                                            }
                                            else
                                            {
                                                MRPrice = MRPriceLifo;
                                                costElement = new MVAMProductCostElement(ctx, ceLifo, null);
                                            }
                                            price = MVAMProductCostQueue.CalculateCostQueuePrice(invoice, invoiceline, MRPrice, acctSchema, VAF_Client_ID, VAF_Org_ID, costElement, trxName, false, "process", null);
                                            if (price == 0)
                                            {
                                                trxName.Rollback();
                                                conversionNotFound = invoice.GetDocumentNo();
                                                return false;
                                            }
                                            query.Clear();
                                            query.Append("UPDATE VAM_ProductCostQueue SET CurrentCostPrice = " + price + " WHERE VAM_ProductCostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[b]["VAM_ProductCostQueue_ID"]));
                                            //query.Append("UPDATE VAM_ProductCostQueue SET CurrentCostPrice = " + Decimal.Round(Decimal.Add(price, Decimal.Divide(Decimal.Multiply(price, costElement.GetSurchargePercentage()), 100)), acctSchema.GetCostingPrecision()) + " WHERE VAM_ProductCostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[b]["VAM_ProductCostQueue_ID"]));
                                            DB.ExecuteQuery(query.ToString(), null, trxName);
                                        }
                                    }
                                }
                                else
                                {
                                    // Calculate amount of matched Invoice
                                    query.Clear();
                                    query.Append(@"SELECT il.VAB_InvoiceLine_id , il.VAB_Invoice_id , ROUND(il.linenetamt/il.qtyinvoiced , 4) AS priceactual , mi.qty ,  SUM(mi.qty) AS matchedqty 
                                          FROM VAM_Inv_InOut io INNER JOIN VAM_Inv_InOutLine iol ON io.VAM_Inv_InOut_id = iol.VAM_Inv_InOut_id
                                          INNER JOIN VAM_MatchInvoice mi ON iol.VAM_Inv_InOutLine_id = mi.VAM_Inv_InOutLine_id
                                          INNER JOIN VAB_InvoiceLine il ON il.VAB_InvoiceLine_id = mi.VAB_InvoiceLine_id
                                          WHERE mi.isactive      = 'Y' AND io.VAM_Inv_InOut_ID      =" + inoutline.GetVAM_Inv_InOut_ID() +
                                                    " AND iol.VAM_Inv_InOutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID() +
                                                     " GROUP BY  il.VAB_InvoiceLine_id , il.VAB_Invoice_id , ROUND(il.linenetamt/il.qtyinvoiced , 4) , mi.qty ");
                                    ds2 = DB.ExecuteDataset(query.ToString(), null, trxName);
                                    if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                    {
                                        Price = 0;
                                        Qty = 0;
                                        MVABInvoice inv = null;
                                        for (int n = 0; n < ds2.Tables[0].Rows.Count; n++)
                                        {
                                            Qty += Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["matchedqty"]);
                                            inv = new MVABInvoice(ctx, Util.GetValueOfInt(ds2.Tables[0].Rows[n]["VAB_Invoice_id"]), trxName);
                                            if (inv.GetVAB_Currency_ID() != acctSchema.GetVAB_Currency_ID())
                                            {
                                                Price += MVABExchangeRate.Convert(ctx, decimal.Multiply(Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["priceactual"]), Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["qty"])),
                                                                                 inv.GetVAB_Currency_ID(), acctSchema.GetVAB_Currency_ID(),
                                                                                 inv.GetDateAcct(), inv.GetVAB_CurrencyType_ID(), VAF_Client_ID, VAF_Org_ID);
                                                if (Price == 0)
                                                {
                                                    trxName.Rollback();
                                                    conversionNotFound = inv.GetDocumentNo();
                                                    return false;
                                                }
                                            }
                                            else
                                            {
                                                Price += decimal.Multiply(Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["priceactual"]), Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["qty"]));
                                            }
                                        }
                                    }
                                    // change 4-5-2016
                                    //Price = Decimal.Add(Price, Decimal.Multiply(MRPrice, Decimal.Subtract(inoutline.GetQtyEntered(), Qty)));
                                    decimal tempQty = Decimal.Subtract(inoutline.GetMovementQty(), Qty);
                                    if (Qty != inoutline.GetMovementQty())
                                    {
                                        Qty += (inoutline.GetMovementQty() - Qty);
                                    }

                                    // update cost queue with invoice amount
                                    query.Clear();
                                    query.Append(@"SELECT * FROM VAT_Temp_CostDetail WHERE  VAM_Inv_InOutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID() +
                                              " AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID());
                                    ds2 = DB.ExecuteDataset(query.ToString(), null, null);
                                    if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                    {
                                        matchInvoicePrice = Price;
                                        for (int k = 0; k < ds2.Tables[0].Rows.Count; k++)
                                        {
                                            Price = matchInvoicePrice;
                                            //change 4-5-2016
                                            if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostQueue WHERE VAM_ProductCostQueue_ID = " + (Util.GetValueOfInt(ds2.Tables[0].Rows[k]["VAM_ProductCostQueue_id"])))) == ceFifo)
                                            {
                                                MRPrice = MRPriceFifo;
                                                costElement = new MVAMProductCostElement(ctx, ceFifo, null);
                                            }
                                            else
                                            {
                                                MRPrice = MRPriceLifo;
                                                costElement = new MVAMProductCostElement(ctx, ceLifo, null);
                                            }
                                            //24-Aug-2016
                                            Price = Decimal.Round(Decimal.Add(Price, Decimal.Divide(Decimal.Multiply(Price, costElement.GetSurchargePercentage()), 100)), acctSchema.GetCostingPrecision());
                                            //end
                                            Price = Decimal.Add(Price, Decimal.Multiply(MRPrice, tempQty));
                                            //end
                                            query.Clear();
                                            query.Append("UPDATE VAM_ProductCostQueue SET CurrentCostPrice = " + Decimal.Round((Price / Qty), acctSchema.GetCostingPrecision()) + " WHERE VAM_ProductCostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[k]["VAM_ProductCostQueue_ID"]));
                                            //query.Append("UPDATE VAM_ProductCostQueue SET CurrentCostPrice = " + Decimal.Round(Decimal.Add((Price / Qty), (((Price / Qty) * costElement.GetSurchargePercentage()) / 100)), acctSchema.GetCostingPrecision()) + " WHERE VAM_ProductCostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[k]["VAM_ProductCostQueue_ID"]));
                                            DB.ExecuteQuery(query.ToString(), null, trxName);
                                        }
                                    }
                                    ds2.Dispose();
                                }

                                // update VAM_ProductCost with accumulation qty , amt , and current cost
                                result = cd.UpdateProductCost("Product Cost IV", cd, acctSchema, product, M_ASI_ID, invoiceline.GetVAF_Org_ID());
                                if (!result)
                                {
                                    trxName.Rollback();
                                    _log.Severe("Error occured during saving a record in M-Cost -> Match IV");
                                    return false;
                                }
                            }
                            else
                            {
                                invoice = new MVABInvoice(ctx, invoiceline.GetVAB_Invoice_ID(), trxName);
                                if (invoice.GetVAB_Currency_ID() != acctSchema.GetVAB_Currency_ID())
                                {
                                    Price = MVABExchangeRate.Convert(ctx, Decimal.Multiply(Qty, Decimal.Round(Decimal.Divide(invoiceline.GetLineNetAmt(), invoiceline.GetQtyInvoiced()), 2)), invoice.GetVAB_Currency_ID(), acctSchema.GetVAB_Currency_ID(),
                                                                             invoice.GetDateAcct(), invoice.GetVAB_CurrencyType_ID(), VAF_Client_ID, VAF_Org_ID);
                                    if (Price == 0)
                                    {
                                        trxName.Rollback();
                                        conversionNotFound = invoice.GetDocumentNo();
                                        return false;
                                    }
                                }

                                cd = new MVAMProductCostDetail(acctSchema, VAF_Org_ID, product.GetVAM_Product_ID(), M_ASI_ID,
                                                         0, Price, Qty, null, trxName);
                                cd.SetVAB_InvoiceLine_ID(invoiceline.GetVAB_InvoiceLine_ID());
                                cd.SetVAB_OrderLine_ID(invoiceline.GetVAB_OrderLine_ID());
                                cd.SetVAM_Inv_InOutLine_ID(inoutline.GetVAM_Inv_InOutLine_ID());
                                if (!cd.Save(trxName))
                                {
                                    trxName.Rollback();
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    _log.Severe("Error occured during saving a record, VAB_InvoiceLine_ID = " + invoiceline.GetVAB_InvoiceLine_ID() + " in cost detail -> Match IV. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                    return false;
                                }
                                else
                                {
                                    // reduce cummulative amount for all element
                                    result = cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, invoiceline.GetVAF_Org_ID());
                                    if (!result)
                                    {
                                        trxName.Rollback();
                                        _log.Severe("Error occured during saving a record , VAB_InvoiceLine_ID = " + invoiceline.GetVAB_InvoiceLine_ID() + " in M-Cost -> Match IV");
                                        return false;
                                    }

                                    // update cost queue with invoice amount
                                    query.Clear();
                                    query.Append(@"SELECT * FROM VAT_Temp_CostDetail WHERE  VAM_Inv_InOutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID() +
                                              " AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID());
                                    DataSet ds2 = DB.ExecuteDataset(query.ToString(), null, null);

                                    if (handlingWindowName == "Invoice(Vendor)")
                                    {
                                        price = 0;
                                        if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                        {
                                            for (int b = 0; b < ds2.Tables[0].Rows.Count; b++)
                                            {
                                                //change 4-5-2016
                                                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostQueue WHERE VAM_ProductCostQueue_ID = " + (Util.GetValueOfInt(ds2.Tables[0].Rows[b]["VAM_ProductCostQueue_ID"])))) == ceFifo)
                                                {
                                                    MRPrice = MRPriceFifo;
                                                    costElement = new MVAMProductCostElement(ctx, ceFifo, null);
                                                }
                                                else
                                                {
                                                    MRPrice = MRPriceLifo;
                                                    costElement = new MVAMProductCostElement(ctx, ceLifo, null);
                                                }
                                                //end
                                                price = 0;
                                                price = MVAMProductCostQueue.CalculateCostQueuePrice(invoice, invoiceline, MRPrice, acctSchema, VAF_Client_ID, VAF_Org_ID, costElement, trxName, false, "process", null);
                                                if (price == 0)
                                                {
                                                    trxName.Rollback();
                                                    conversionNotFound = invoice.GetDocumentNo();
                                                    return false;
                                                }
                                                query.Clear();
                                                query.Append("UPDATE VAM_ProductCostQueue SET CurrentCostPrice = " + price + " WHERE VAM_ProductCostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[b]["VAM_ProductCostQueue_ID"]));
                                                //query.Append("UPDATE VAM_ProductCostQueue SET CurrentCostPrice = " + Decimal.Round(Decimal.Add(price, Decimal.Divide(Decimal.Multiply(price, costElement.GetSurchargePercentage()), 100)), acctSchema.GetCostingPrecision()) + " WHERE VAM_ProductCostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[b]["VAM_ProductCostQueue_ID"]));
                                                DB.ExecuteQuery(query.ToString(), null, trxName);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // Calculate amount of matched Invoice
                                        query.Clear();
                                        query.Append(@"SELECT il.VAB_InvoiceLine_id , il.VAB_Invoice_id , ROUND(il.linenetamt/il.qtyinvoiced , 4) AS priceactual , mi.qty ,  SUM(mi.qty) AS matchedqty 
                                          FROM VAM_Inv_InOut io INNER JOIN VAM_Inv_InOutLine iol ON io.VAM_Inv_InOut_id = iol.VAM_Inv_InOut_id
                                          INNER JOIN VAM_MatchInvoice mi ON iol.VAM_Inv_InOutLine_id = mi.VAM_Inv_InOutLine_id
                                          INNER JOIN VAB_InvoiceLine il ON il.VAB_InvoiceLine_id = mi.VAB_InvoiceLine_id
                                          WHERE mi.isactive      = 'Y' AND io.VAM_Inv_InOut_ID      =" + inoutline.GetVAM_Inv_InOut_ID() +
                                                     " AND iol.VAM_Inv_InOutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID() +
                                                      " GROUP BY  il.VAB_InvoiceLine_id , il.VAB_Invoice_id , ROUND(il.linenetamt/il.qtyinvoiced , 4) , mi.qty ");
                                        ds2 = DB.ExecuteDataset(query.ToString(), null, trxName);
                                        if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                        {
                                            Price = 0;
                                            Qty = 0; ;
                                            MVABInvoice inv = null;
                                            for (int n = 0; n < ds2.Tables[0].Rows.Count; n++)
                                            {
                                                Qty += Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["matchedqty"]);
                                                inv = new MVABInvoice(ctx, Util.GetValueOfInt(ds2.Tables[0].Rows[n]["VAB_Invoice_id"]), trxName);
                                                if (inv.GetVAB_Currency_ID() != acctSchema.GetVAB_Currency_ID())
                                                {
                                                    Price += MVABExchangeRate.Convert(ctx, decimal.Multiply(Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["priceactual"]), Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["qty"])),
                                                                                     inv.GetVAB_Currency_ID(), acctSchema.GetVAB_Currency_ID(),
                                                                                     inv.GetDateAcct(), inv.GetVAB_CurrencyType_ID(), VAF_Client_ID, VAF_Org_ID);
                                                    if (Price == 0)
                                                    {
                                                        trxName.Rollback();
                                                        conversionNotFound = inv.GetDocumentNo();
                                                        return false;
                                                    }
                                                }
                                                else
                                                {
                                                    Price += decimal.Multiply(Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["priceactual"]), Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["qty"]));
                                                }
                                            }
                                        }
                                        //change 4-5-2016
                                        //Price = Decimal.Add(Price, Decimal.Multiply(MRPrice, Decimal.Subtract(inoutline.GetQtyEntered(), Qty)));
                                        decimal tempQty = Decimal.Subtract(inoutline.GetMovementQty(), Qty);
                                        if (Qty != inoutline.GetMovementQty())
                                        {
                                            Qty += (inoutline.GetMovementQty() - Qty);
                                        }

                                        // update cost queue with invoice amount
                                        query.Clear();
                                        query.Append(@"SELECT * FROM VAT_Temp_CostDetail WHERE  VAM_Inv_InOutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID() +
                                                  " AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID());

                                        ds2 = DB.ExecuteDataset(query.ToString(), null, null);
                                        if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                        {
                                            matchInvoicePrice = Price;
                                            for (int k = 0; k < ds2.Tables[0].Rows.Count; k++)
                                            {
                                                Price = matchInvoicePrice;
                                                //change 4-5-2016
                                                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostQueue WHERE VAM_ProductCostQueue_ID = " + (Util.GetValueOfInt(ds2.Tables[0].Rows[k]["VAM_ProductCostQueue_ID"])))) == ceFifo)
                                                {
                                                    MRPrice = MRPriceFifo;
                                                    costElement = new MVAMProductCostElement(ctx, ceFifo, null);
                                                }
                                                else
                                                {
                                                    MRPrice = MRPriceLifo;
                                                    costElement = new MVAMProductCostElement(ctx, ceLifo, null);
                                                }
                                                //24-Aug-2016
                                                Price = Decimal.Round(Decimal.Add(Price, Decimal.Divide(Decimal.Multiply(Price, costElement.GetSurchargePercentage()), 100)), acctSchema.GetCostingPrecision());
                                                //end
                                                Price = Decimal.Add(Price, Decimal.Multiply(MRPrice, tempQty));
                                                //end
                                                query.Clear();
                                                query.Append("UPDATE VAM_ProductCostQueue SET CurrentCostPrice = " + Decimal.Round(Price / Qty, acctSchema.GetCostingPrecision()) + " WHERE VAM_ProductCostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[k]["VAM_ProductCostQueue_ID"]));
                                                //query.Append("UPDATE VAM_ProductCostQueue SET CurrentCostPrice = " + Decimal.Round(Decimal.Add((Price / Qty), (((Price / Qty) * costElement.GetSurchargePercentage()) / 100)), acctSchema.GetCostingPrecision()) + " WHERE VAM_ProductCostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[k]["VAM_ProductCostQueue_ID"]));
                                                DB.ExecuteQuery(query.ToString(), null, trxName);
                                            }
                                        }
                                        ds2.Dispose();
                                    }

                                    // update VAM_ProductCost with accumulation qty , amt , and current cost
                                    result = cd.UpdateProductCost("Product Cost IV", cd, acctSchema, product, M_ASI_ID, invoiceline.GetVAF_Org_ID());
                                    if (!result)
                                    {
                                        trxName.Rollback();
                                        _log.Severe("Error occured during saving a record in M-Cost -> Product Cost IV");
                                        return false;
                                    }
                                }

                            }
                            if (inoutline != null && inoutline.GetVAM_Inv_InOutLine_ID() > 0)
                            {
                                inoutline.SetIsCostCalculated(true);
                                inoutline.Save();
                            }
                            #endregion
                        }

                        if (windowName == "Physical Inventory" || windowName == "Internal Use Inventory" || windowName == "Inventory Move")
                        {
                            #region Get Price
                            //Get Price from MVAMProductCost
                            // change 2-5-2016
                            price = MVAMProductCostQueue.CalculateCost(ctx, acctSchema, product, M_ASI_ID, VAF_Client_ID, VAF_Org_ID, 0);
                            cmPrice = price;
                            price = 0;
                            //end
                            int VAB_Currency_ID = 0;
                            if (price == 0 || plPrice == 0)
                            {
                                isPriceFromProductPrice = true;
                                if (windowName == "Inventory Move")
                                {
                                    query.Clear();
                                    query.Append(@"(SELECT pl.VAF_Org_ID , pl.name , pl.VAM_PriceList_id  , plv.VAM_PriceListVersion_id , pp.pricestd , pl.VAB_Currency_id
                                           FROM VAM_PriceList pl  INNER JOIN VAM_PriceListVersion plv   ON pl.VAM_PriceList_id    = plv.VAM_PriceList_id
                                            inner join VAM_ProductPrice pp   on pp.VAM_PriceListVersion_id = plv.VAM_PriceListVersion_id
                                           WHERE pl.VAF_Org_ID = " + VAF_Org_ID + " AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.VAM_Product_id = " + product.GetVAM_Product_ID() +
                                           " AND NVL(pp.VAM_PFeature_SetInstance_id, 0) = " + M_ASI_ID +
                                           " AND pp.VAB_UOM_id = " + product.GetVAB_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.VAM_PriceList_id asc");
                                }
                                else
                                {
                                    query.Clear();
                                    query.Append(@"(SELECT pl.VAF_Org_ID , pl.name , pl.VAM_PriceList_id  , plv.VAM_PriceListVersion_id , pp.pricestd , pl.VAB_Currency_id
                                           FROM VAM_PriceList pl  INNER JOIN VAM_PriceListVersion plv   ON pl.VAM_PriceList_id    = plv.VAM_PriceList_id
                                            inner join VAM_ProductPrice pp   on pp.VAM_PriceListVersion_id = plv.VAM_PriceListVersion_id
                                           WHERE pl.VAF_Org_ID = " + inventoryLine.GetVAF_Org_ID() + " AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.VAM_Product_id = " + product.GetVAM_Product_ID() +
                                              " AND NVL(pp.VAM_PFeature_SetInstance_id, 0) = " + M_ASI_ID +
                                              " AND pp.VAB_UOM_id = " + product.GetVAB_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.VAM_PriceList_id asc");
                                }
                                DataSet dsStdPrice = DB.ExecuteDataset(query.ToString(), null, null);
                                if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                                {
                                    price = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                                    VAB_Currency_ID = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["VAB_Currency_id"]);
                                }
                                else
                                {
                                    query.Clear();
                                    query.Append(@"(SELECT pl.VAF_Org_ID , pl.name , pl.VAM_PriceList_id  , plv.VAM_PriceListVersion_id , pp.pricestd , pl.VAB_Currency_id
                                           FROM VAM_PriceList pl  INNER JOIN VAM_PriceListVersion plv   ON pl.VAM_PriceList_id    = plv.VAM_PriceList_id
                                            inner join VAM_ProductPrice pp   on pp.VAM_PriceListVersion_id = plv.VAM_PriceListVersion_id
                                           WHERE pl.VAF_Org_ID = 0 AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.VAM_Product_id = " + product.GetVAM_Product_ID() +
                                           " AND NVL(pp.VAM_PFeature_SetInstance_id, 0) = " + M_ASI_ID +
                                           " AND pp.VAB_UOM_id = " + product.GetVAB_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.VAM_PriceList_id asc");
                                    dsStdPrice = DB.ExecuteDataset(query.ToString(), null, null);
                                    if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                                    {
                                        price = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                                        VAB_Currency_ID = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["VAB_Currency_id"]);
                                    }
                                }
                                if (price == 0)
                                {
                                    if (windowName == "Inventory Move")
                                    {
                                        query.Clear();
                                        query.Append(@"(SELECT pl.VAF_Org_ID , pl.name , pl.VAM_PriceList_id  , plv.VAM_PriceListVersion_id , pp.pricestd , pl.VAB_Currency_id
                                           FROM VAM_PriceList pl  INNER JOIN VAM_PriceListVersion plv   ON pl.VAM_PriceList_id    = plv.VAM_PriceList_id
                                            inner join VAM_ProductPrice pp   on pp.VAM_PriceListVersion_id = plv.VAM_PriceListVersion_id
                                           WHERE pl.VAF_Org_ID = " + VAF_Org_ID + " AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.VAM_Product_id = " + product.GetVAM_Product_ID() +
                                           " AND pp.VAB_UOM_id = " + product.GetVAB_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.VAM_PriceList_id asc");
                                    }
                                    else
                                    {
                                        query.Clear();
                                        query.Append(@"(SELECT pl.VAF_Org_ID , pl.name , pl.VAM_PriceList_id  , plv.VAM_PriceListVersion_id , pp.pricestd , pl.VAB_Currency_id
                                           FROM VAM_PriceList pl  INNER JOIN VAM_PriceListVersion plv   ON pl.VAM_PriceList_id    = plv.VAM_PriceList_id
                                            inner join VAM_ProductPrice pp   on pp.VAM_PriceListVersion_id = plv.VAM_PriceListVersion_id
                                           WHERE pl.VAF_Org_ID = " + inventoryLine.GetVAF_Org_ID() + " AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.VAM_Product_id = " + product.GetVAM_Product_ID() +
                                              " AND pp.VAB_UOM_id = " + product.GetVAB_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.VAM_PriceList_id asc");
                                    }
                                    dsStdPrice = DB.ExecuteDataset(query.ToString(), null, null);
                                    if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                                    {
                                        price = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                                        VAB_Currency_ID = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["VAB_Currency_id"]);
                                    }
                                    else
                                    {
                                        query.Clear();
                                        query.Append(@"(SELECT pl.VAF_Org_ID , pl.name , pl.VAM_PriceList_id  , plv.VAM_PriceListVersion_id , pp.pricestd , pl.VAB_Currency_id
                                           FROM VAM_PriceList pl  INNER JOIN VAM_PriceListVersion plv   ON pl.VAM_PriceList_id    = plv.VAM_PriceList_id
                                            inner join VAM_ProductPrice pp   on pp.VAM_PriceListVersion_id = plv.VAM_PriceListVersion_id
                                           WHERE pl.VAF_Org_ID = 0 AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.VAM_Product_id = " + product.GetVAM_Product_ID() +
                                               " AND pp.VAB_UOM_id = " + product.GetVAB_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.VAM_PriceList_id asc");
                                        dsStdPrice = DB.ExecuteDataset(query.ToString(), null, null);
                                        if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                                        {
                                            price = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                                            VAB_Currency_ID = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["VAB_Currency_id"]);
                                        }
                                    }
                                    dsStdPrice.Dispose();
                                    if (price == 0)
                                        price = 0;
                                }
                            }
                            Price = price * Qty;
                            cmPrice = cmPrice * Qty;

                            // Currency Conversion
                            if (isPriceFromProductPrice && (windowName == "Physical Inventory" || windowName == "Internal Use Inventory"))
                            {
                                inventory = new MVAMInventory(ctx, inventoryLine.GetVAM_Inventory_ID(), trxName);
                                if (VAB_Currency_ID != 0 && VAB_Currency_ID != acctSchema.GetVAB_Currency_ID())
                                {
                                    Price = MVABExchangeRate.Convert(ctx, Price, VAB_Currency_ID, acctSchema.GetVAB_Currency_ID(),
                                                                            inventory.GetMovementDate(), 0, VAF_Client_ID, VAF_Org_ID);
                                    if (Price == 0)
                                    {
                                        trxName.Rollback();
                                        conversionNotFound = inventory.GetDocumentNo();
                                        return false;
                                    }
                                }
                            }
                            else if (isPriceFromProductPrice && windowName == "Inventory Move")
                            {
                                movement = new MVAMInventoryTransfer(ctx, movementline.GetVAM_InventoryTransfer_ID(), trxName);
                                if (VAB_Currency_ID != 0 && VAB_Currency_ID != acctSchema.GetVAB_Currency_ID())
                                {
                                    Price = MVABExchangeRate.Convert(ctx, Price, VAB_Currency_ID, acctSchema.GetVAB_Currency_ID(),
                                                                            movement.GetMovementDate(), 0, VAF_Client_ID, VAF_Org_ID);
                                    if (Price == 0)
                                    {
                                        trxName.Rollback();
                                        conversionNotFound = movement.GetDocumentNo();
                                        return false;
                                    }
                                }
                            }
                            plPrice = Price;
                            #endregion
                        }
                        if (windowName == "Inventory Move")
                        {
                            #region Special handling for Inventory move
                            if (cmPrice > 0)
                            {
                                cdSourceWarehouse = MVAMProductCostDetail.CreateCostDetail(acctSchema, VAF_Org_ID, product.GetVAM_Product_ID(), M_ASI_ID, windowName,
                                    inventoryLine, inoutline, movementline, invoiceline, null, 0, Decimal.Negate(cmPrice), Decimal.Negate(Qty), null, trxName);
                            }
                            else
                            {
                                cdSourceWarehouse = MVAMProductCostDetail.CreateCostDetail(acctSchema, VAF_Org_ID, product.GetVAM_Product_ID(), M_ASI_ID, windowName,
                                    inventoryLine, inoutline, movementline, invoiceline, null, 0, Decimal.Negate(Price), Decimal.Negate(Qty), null, trxName);
                            }
                            query.Clear();
                            query.Append("SELECT VAF_Org_ID FROM VAM_Warehouse WHERE IsActive = 'Y' AND VAM_Warehouse_ID = (SELECT VAM_Warehouse_ID FROM VAM_InventoryTransfer WHERE IsActive = 'Y' AND VAM_InventoryTransfer_ID = " + movementline.GetVAM_InventoryTransfer_ID() + ")");
                            VAF_Org_ID = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));
                            #endregion
                        }

                        // create cost detail
                        if ((windowName == "Invoice(Vendor)" && product == null) || (windowName == "Invoice(Vendor)" && product != null && product.GetProductType() == "E"))
                        {
                            #region Landed Cost Allocation Handle
                            isLandedCostAllocation = true;
                            decimal amt = 0;
                            decimal qntity = 0;
                            query.Clear();
                            query.Append("SELECT * FROM VAB_LCostDistribution WHERE  VAB_InvoiceLine_ID = " + invoiceline.GetVAB_InvoiceLine_ID());
                            DataSet dsLandedCostAllocation = DB.ExecuteDataset(query.ToString(), null, trxName);
                            if (dsLandedCostAllocation != null && dsLandedCostAllocation.Tables.Count > 0 && dsLandedCostAllocation.Tables[0].Rows.Count > 0)
                            {
                                invoice = new MVABInvoice(ctx, invoiceline.GetVAB_Invoice_ID(), trxName);
                                for (int lca = 0; lca < dsLandedCostAllocation.Tables[0].Rows.Count; lca++)
                                {
                                    if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                    {
                                        if (Util.GetValueOfDecimal(dsLandedCostAllocation.Tables[0].Rows[lca]["Base"]) > 0)
                                        {
                                            qntity = Decimal.Negate(Util.GetValueOfDecimal(dsLandedCostAllocation.Tables[0].Rows[lca]["Base"]));
                                        }
                                        else
                                        {
                                            qntity = Util.GetValueOfDecimal(dsLandedCostAllocation.Tables[0].Rows[lca]["Base"]);
                                        }
                                        if (Util.GetValueOfDecimal(dsLandedCostAllocation.Tables[0].Rows[lca]["Amt"]) > 0)
                                        {
                                            amt = Decimal.Negate(Util.GetValueOfDecimal(dsLandedCostAllocation.Tables[0].Rows[lca]["Amt"]));
                                        }
                                        else
                                        {
                                            amt = Util.GetValueOfDecimal(dsLandedCostAllocation.Tables[0].Rows[lca]["Amt"]);
                                        }
                                    }
                                    else
                                    {
                                        qntity = Util.GetValueOfDecimal(dsLandedCostAllocation.Tables[0].Rows[lca]["Base"]);
                                        amt = Util.GetValueOfDecimal(dsLandedCostAllocation.Tables[0].Rows[lca]["Amt"]);
                                    }
                                    // conversion required
                                    if (invoice.GetVAB_Currency_ID() != acctSchema.GetVAB_Currency_ID())
                                    {
                                        amt = MVABExchangeRate.Convert(ctx, amt, invoice.GetVAB_Currency_ID(), acctSchema.GetVAB_Currency_ID(),
                                                                             invoice.GetDateAcct(), invoice.GetVAB_CurrencyType_ID(), VAF_Client_ID, VAF_Org_ID);
                                        if (amt == 0)
                                        {
                                            trxName.Rollback();
                                            conversionNotFound = invoice.GetDocumentNo();
                                            return false;
                                        }
                                    }
                                    cd = MVAMProductCostDetail.CreateCostDetail(acctSchema, VAF_Org_ID, Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["VAM_Product_ID"]),
                                        Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["VAM_PFeature_SetInstance_ID"]), windowName, inventoryLine, inoutline, movementline,
                                     invoiceline, null, Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["VAM_ProductCostElement_ID"]), amt,
                                     qntity, null, trxName);
                                    if (cd != null)
                                    {
                                        productLca = new MProduct(ctx, Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["VAM_Product_ID"]), trxName);
                                        M_ASI_ID = Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["VAM_PFeature_SetInstance_ID"]);
                                        cd.UpdateProductCost(windowName, cd, acctSchema, productLca, M_ASI_ID, invoiceline.GetVAF_Org_ID());
                                        if (cd != null)
                                        {
                                            cd.CreateCostForCombination(cd, acctSchema, productLca, M_ASI_ID, 0, "LandedCostAllocation");
                                        }
                                    }
                                }
                            }
                            dsLandedCostAllocation.Dispose();
                            #endregion
                        }
                        else if (windowName != "Match PO" && windowName != "Match IV")
                        {
                            //change 2-5-2016
                            if (cmPrice != 0)
                            {
                                cd = MVAMProductCostDetail.CreateCostDetail(acctSchema, VAF_Org_ID, product.GetVAM_Product_ID(), M_ASI_ID, windowName,
                                      inventoryLine, inoutline, movementline, invoiceline, null, 0, cmPrice, Qty, null, trxName);
                            }
                            else
                            {
                                cd = MVAMProductCostDetail.CreateCostDetail(acctSchema, VAF_Org_ID, product.GetVAM_Product_ID(), M_ASI_ID, windowName,
                                     inventoryLine, inoutline, movementline, invoiceline, null, 0, Price, Qty, null, trxName);
                            }
                        }

                        #region Create / update "Cost Queue"
                        if (cd != null && !isLandedCostAllocation)
                        {
                            query.Clear();
                            query.Append(@"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = " +
                                     " ( SELECT MMPolicy FROM VAM_ProductCategory WHERE IsActive = 'Y' AND VAM_ProductCategory_ID = " +
                                     " (SELECT VAM_ProductCategory_ID FROM VAM_Product WHERE IsActive = 'Y' AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " )) AND VAF_Client_ID = " + VAF_Client_ID);
                            costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                            costElement = new MVAMProductCostElement(ctx, costingElementId, null);

                            if (windowName == "Physical Inventory" || windowName == "Internal Use Inventory")
                            {
                                #region Phy. Inventory / Internal Use Inventory
                                if (Qty > 0)
                                {
                                    result = CreateCostQueue(ctx, acctSchema, product, M_ASI_ID, VAF_Client_ID, inventoryLine.GetVAF_Org_ID(), Price / Qty, Qty, windowName,
                                          inventoryLine, inoutline, movementline, invoiceline, cd, trxName, out costQueuseIds);
                                    if (!result)
                                    {
                                        trxName.Rollback();
                                        _log.Severe("Error occured during CreateCostQueue for VAM_Inventory_ID = " + inventoryLine.GetVAM_InventoryLine_ID());
                                        return false;
                                    }
                                }
                                else
                                {
                                    //1st entry either of FIFO of LIFO 
                                    updateCostQueue(product, M_ASI_ID, acctSchema, inventoryLine.GetVAF_Org_ID(), costElement, Decimal.Negate(Qty));

                                    //2nd either for Fifo or lifo opposite of 1st entry
                                    if (costElement.GetCostingMethod() == "F")
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND VAF_Client_ID = " + VAF_Client_ID);
                                    }
                                    else
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND VAF_Client_ID = " + VAF_Client_ID);
                                    }
                                    costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                                    costElement = new MVAMProductCostElement(ctx, costingElementId, null);
                                    updateCostQueue(product, M_ASI_ID, acctSchema, inventoryLine.GetVAF_Org_ID(), costElement, Decimal.Negate(Qty));
                                }
                                #endregion
                            }
                            else if (windowName == "Material Receipt" || windowName == "Customer Return" || windowName == "Shipment" || windowName == "Return To Vendor")
                            {
                                #region Sales / Purchase / Return
                                if (Qty > 0)
                                {
                                    result = CreateCostQueue(ctx, acctSchema, product, M_ASI_ID, VAF_Client_ID, inoutline.GetVAF_Org_ID(), Price / Qty, Qty, windowName,
                                         inventoryLine, inoutline, movementline, invoiceline, cd, trxName, out costQueuseIds);
                                    if (!result)
                                    {
                                        trxName.Rollback();
                                        _log.Severe("Error occured during CreateCostQueue for VAM_Inv_InOut_ID = " + inoutline.GetVAM_Inv_InOutLine_ID());
                                        return false;
                                    }
                                }
                                else
                                {
                                    //1st entry either of FIFO of LIFO 
                                    updateCostQueue(product, M_ASI_ID, acctSchema, inoutline.GetVAF_Org_ID(), costElement, Decimal.Negate(Qty));

                                    //2nd either for Fifo or lifo opposite of 1st entry
                                    if (costElement.GetCostingMethod() == "F")
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND VAF_Client_ID = " + VAF_Client_ID);
                                    }
                                    else
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND VAF_Client_ID = " + VAF_Client_ID);
                                    }
                                    costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                                    costElement = new MVAMProductCostElement(ctx, costingElementId, null);
                                    updateCostQueue(product, M_ASI_ID, acctSchema, inoutline.GetVAF_Org_ID(), costElement, Decimal.Negate(Qty));
                                }
                                #endregion
                            }
                            else if (windowName == "Inventory Move")
                            {
                                #region Inventory Move
                                if (Qty > 0)
                                {
                                    result = CreateCostQueue(ctx, acctSchema, product, M_ASI_ID, VAF_Client_ID, VAF_Org_ID, Price / Qty, Qty, windowName,
                                         inventoryLine, inoutline, movementline, invoiceline, cd, trxName, out costQueuseIds);
                                    if (!result)
                                    {
                                        trxName.Rollback();
                                        _log.Severe("Error occured during CreateCostQueue for VAM_InventoryTransferLime_ID = " + movementline.GetVAM_InvTrf_Line_ID());
                                        return false;
                                    }

                                    //1st entry either of FIFO of LIFO 
                                    updateCostQueue(product, M_ASI_ID, acctSchema, movementline.GetVAF_Org_ID(), costElement, Qty);

                                    //2nd either for Fifo or lifo opposite of 1st entry
                                    if (costElement.GetCostingMethod() == "F")
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND VAF_Client_ID = " + VAF_Client_ID);
                                    }
                                    else
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND VAF_Client_ID = " + VAF_Client_ID);
                                    }
                                    costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                                    costElement = new MVAMProductCostElement(ctx, costingElementId, null);
                                    updateCostQueue(product, M_ASI_ID, acctSchema, movementline.GetVAF_Org_ID(), costElement, Qty);
                                }
                                else
                                {
                                    result = CreateCostQueue(ctx, acctSchema, product, M_ASI_ID, VAF_Client_ID, movementline.GetVAF_Org_ID(), Price / Qty, Decimal.Negate(Qty), windowName,
                                        inventoryLine, inoutline, movementline, invoiceline, cd, trxName, out costQueuseIds);
                                    if (!result)
                                    {
                                        trxName.Rollback();
                                        _log.Severe("Error occured during CreateCostQueue for VAM_InvTrf_Line_ID = " + movementline.GetVAM_InvTrf_Line_ID());
                                        return false;
                                    }

                                    //1st entry either of FIFO of LIFO 
                                    updateCostQueue(product, M_ASI_ID, acctSchema, VAF_Org_ID, costElement, Decimal.Negate(Qty));

                                    //2nd either for Fifo or lifo opposite of 1st entry
                                    if (costElement.GetCostingMethod() == "F")
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND VAF_Client_ID = " + VAF_Client_ID);
                                    }
                                    else
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND VAF_Client_ID = " + VAF_Client_ID);
                                    }
                                    costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                                    costElement = new MVAMProductCostElement(ctx, costingElementId, null);
                                    updateCostQueue(product, M_ASI_ID, acctSchema, VAF_Org_ID, costElement, Decimal.Negate(Qty));
                                }
                                #endregion
                            }
                            else if (windowName == "Invoice(Vendor)")
                            {
                                #region update cost on cost Queue / VAM_ProductCost
                                if (invoiceline.GetVAB_OrderLine_ID() > 0 && invoiceline.GetVAM_Inv_InOutLine_ID() > 0)
                                {
                                    query.Clear();
                                    query.Append(@"SELECT * FROM VAT_Temp_CostDetail WHERE VAB_OrderLine_ID = " + invoiceline.GetVAB_OrderLine_ID() +
                                         " AND VAM_Inv_InOutLine_ID = " + invoiceline.GetVAM_Inv_InOutLine_ID() + " AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID());
                                    DataSet ds1 = DB.ExecuteDataset(query.ToString(), null, trxName);
                                    if (ds1 != null && ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                                    {
                                        for (int k = 0; k < ds1.Tables[0].Rows.Count; k++)
                                        {
                                            // change 9-5-2016
                                            // handle partial payment
                                            X_VAT_Temp_CostDetail tempCostDetail = new X_VAT_Temp_CostDetail(invoiceline.GetCtx(), Util.GetValueOfInt(ds1.Tables[0].Rows[k]["VAT_Temp_CostDetail_ID"]), trxName);
                                            query.Clear();
                                            query.Append("SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostQueue WHERE VAM_ProductCostQueue_ID = " + Util.GetValueOfInt(ds1.Tables[0].Rows[k]["VAM_ProductCostQueue_ID"]));
                                            costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));
                                            costElement = new MVAMProductCostElement(ctx, costingElementId, trxName);

                                            price = MVAMProductCostQueue.CalculateCostQueuePrice(invoice, invoiceline, tempCostDetail.GetAmt(), acctSchema, VAF_Client_ID, VAF_Org_ID, costElement, trxName, false, "process", null);
                                            //sql = "UPDATE VAM_ProductCostQueue SET CurrentCostPrice = " + cd.GetAmt() / cd.GetQty() + " WHERE VAM_ProductCostQueue_ID = " + Util.GetValueOfInt(ds1.Tables[0].Rows[k]["VAM_ProductCostQueue_ID"]);
                                            query.Clear();
                                            query.Append("UPDATE VAM_ProductCostQueue SET CurrentCostPrice = " + price + " WHERE VAM_ProductCostQueue_ID = " + Util.GetValueOfInt(ds1.Tables[0].Rows[k]["VAM_ProductCostQueue_ID"]));
                                            DB.ExecuteQuery(query.ToString(), null, trxName);
                                        }
                                    }
                                    ds1.Dispose();
                                }
                                #endregion
                            }
                        }
                        #endregion

                        #region update "Product Cost"
                        if (cd != null)
                        {
                            if (windowName == "Physical Inventory" || windowName == "Internal Use Inventory")
                            {
                                result = cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, inventoryLine.GetVAF_Org_ID());
                                if (!result)
                                {
                                    trxName.Rollback();
                                    _log.Severe("Error occured during UpdateProductCost for VAM_InventoryLine_ID = " + inventoryLine.GetVAM_InventoryLine_ID());
                                    return false;
                                }
                            }
                            else if (windowName == "Material Receipt" || windowName == "Customer Return" || windowName == "Shipment" || windowName == "Return To Vendor")
                            {
                                result = cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, inoutline.GetVAF_Org_ID());
                                if (!result)
                                {
                                    trxName.Rollback();
                                    _log.Severe("Error occured during UpdateProductCost for VAM_Inv_InOutLine_ID = " + inoutline.GetVAM_Inv_InOutLine_ID());
                                    return false;
                                }
                            }
                            else if (windowName == "Inventory Move")
                            {
                                if (pca != null)
                                {
                                    if (pca.GetCostingLevel() == "C" || pca.GetCostingLevel() == "B")
                                    {
                                        VAF_Org_ID = 0;
                                    }
                                    else if ((string.IsNullOrEmpty(pca.GetCostingLevel())) && (acctSchema.GetCostingLevel() == "C" || acctSchema.GetCostingLevel() == "B"))
                                    {
                                        VAF_Org_ID = 0;
                                    }
                                }
                                else if (acctSchema.GetCostingLevel() == "C" || acctSchema.GetCostingLevel() == "B")
                                {
                                    VAF_Org_ID = 0;
                                }
                                if (VAF_Org_ID != 0)
                                {
                                    result = cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, VAF_Org_ID); // for destination warehouse
                                    if (!result)
                                    {
                                        trxName.Rollback();
                                        _log.Severe("Error occured during UpdateProductCost for VAM_InvTrf_Line_ID = " + cd.GetVAM_InvTrf_Line_ID());
                                        return false;
                                    }

                                    result = cdSourceWarehouse.UpdateProductCost(windowName, cdSourceWarehouse, acctSchema, product, M_ASI_ID, movementline.GetVAF_Org_ID()); // for source warehouse org
                                    if (!result)
                                    {
                                        trxName.Rollback();
                                        _log.Severe("Error occured during UpdateProductCost fot VAM_InvTrf_Line_ID = " + cd.GetVAM_InvTrf_Line_ID());
                                        return false;
                                    }

                                    // change 7-4-2016
                                    // for handling and calculating Cost Combination
                                    cdSourceWarehouse.CreateCostForCombination(cdSourceWarehouse, acctSchema, product, M_ASI_ID, 0, windowName);
                                    //end
                                }
                            }
                            else if ((windowName == "Invoice(Vendor)" && !isLandedCostAllocation) || windowName == "Invoice(Customer)")
                            {
                                if (invoiceline.GetVAB_OrderLine_ID() > 0)
                                {
                                    result = cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, invoiceline.GetVAF_Org_ID());
                                    if (!result)
                                    {
                                        trxName.Rollback();
                                        _log.Severe("Error occured during UpdateProductCost for VAB_InvoiceLine_ID = " + invoiceline.GetVAB_InvoiceLine_ID());
                                        return false;
                                    }
                                }
                                else if (invoiceline.GetVAB_OrderLine_ID() == 0 && invoiceline.GetVAM_Inv_InOutLine_ID() == 0)
                                {
                                    // 20-4-2016
                                    // in case of independent AP credit memo, accumulation amt reduce and current cost of AV. PO/Invoice will be calculated
                                    MVABDocTypes docType = new MVABDocTypes(ctx, invoice.GetVAB_DocTypesTarget_ID(), trxName);
                                    if (docType.GetDocBaseType() == "APC")
                                    {
                                        result = cd.UpdateProductCost("Invoice(APC)", cd, acctSchema, product, M_ASI_ID, invoiceline.GetVAF_Org_ID());
                                        if (!result)
                                        {
                                            trxName.Rollback();
                                            _log.Severe("Error occured during UpdateProductCost for VAB_InvoiceLine_ID = " + invoiceline.GetVAB_InvoiceLine_ID());
                                            return false;
                                        }
                                    }
                                }
                            }
                        }
                        #endregion

                        // change 7-4-2016
                        // for handling and calculating Cost Combination
                        if (windowName == "Invoice(Vendor)" && invoiceline.GetVAB_OrderLine_ID() == 0 && invoiceline.GetVAM_Inv_InOutLine_ID() == 0)
                        {
                            // cost element detail not to be calculated when Invoice(Vendor) is alone.
                            // But in case of APC, we have to calculate Cost Element detail and cost combination both
                            MVABDocTypes docType = new MVABDocTypes(ctx, invoice.GetVAB_DocTypesTarget_ID(), trxName);
                            if (docType.GetDocBaseType() == "APC" && cd != null)
                            {
                                cd.CreateCostForCombination(cd, acctSchema, product, M_ASI_ID, 0, windowName);
                            }
                        }
                        else
                            if (cd != null && windowName != "Invoice(Customer)" && !isLandedCostAllocation)
                        {
                            cd.CreateCostForCombination(cd, acctSchema, product, M_ASI_ID, 0, windowName);
                        }
                        //end
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Severe("Error Occured during costing " + ex.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// Is used to get current qty from  product costs based on respective parameter
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="acctSchema">accounting schema</param>
        /// <param name="VAF_Client_ID">client</param>
        /// <param name="VAF_Org_ID">organization</param>
        /// <param name="product">product</param>
        /// <param name="M_ASI_ID">attributesetinstance</param>
        /// <param name="VAM_Warehouse_ID">VAM_Warehouse_ID</param>
        /// <returns>qty</returns>
        public static decimal CheckQtyAvailablity(Ctx ctx, MVABAccountBook acctSchema, int VAF_Client_ID, int VAF_Org_ID, MProduct product, int M_ASI_ID, int VAM_Warehouse_ID)
        {
            decimal qty = 0;
            try
            {
                string sql = @"SELECT ROUND(AVG(CST.CURRENTQTY),4 )   FROM VAM_Product P   INNER JOIN VAM_ProductCost CST   ON P.VAM_Product_ID=CST.VAM_Product_ID
                               LEFT JOIN VAM_ProductCategory PC   ON P.VAM_ProductCategory_ID=PC.VAM_ProductCategory_ID
                               INNER JOIN VAB_ACCOUNTBOOK ACC   ON CST.VAB_ACCOUNTBOOK_ID=ACC.VAB_ACCOUNTBOOK_ID
                               INNER JOIN VAM_ProductCostElement CE  ON CST.VAM_ProductCostElement_ID=CE.VAM_ProductCostElement_ID
                              WHERE (( CASE WHEN PC.COSTINGMETHOD IS NOT NULL  THEN PC.COSTINGMETHOD
                                            ELSE ACC.COSTINGMETHOD  END) = CE.COSTINGMETHOD )
                              AND ((   CASE WHEN PC.COSTINGMETHOD IS NOT NULL  AND PC.COSTINGMETHOD   = 'C'  THEN PC.VAM_ProductCostElement_id
                                            WHEN PC.COSTINGMETHOD IS NOT NULL  THEN (SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement 
                                             WHERE COSTINGMETHOD = pc.COSTINGMETHOD AND vaf_client_id    = " + VAF_Client_ID + @" )
                                            WHEN ACC.COSTINGMETHOD IS NOT NULL AND ACC.COSTINGMETHOD   = 'C' THEN ACC.VAM_ProductCostElement_id ELSE
                                             (SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE COSTINGMETHOD = acc.COSTINGMETHOD 
                                             AND vaf_client_id    = " + VAF_Client_ID + @" ) END) = ce.VAM_ProductCostElement_id)
                             AND ((    CASE WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('A' , 'O' , 'W' , 'D')  THEN " + VAF_Org_ID + @"
                                            WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('B' , 'C')  THEN 0 
                                            WHEN ACC.COSTINGLEVEL IS NOT NULL AND ACC.COSTINGLEVEL   IN ('A' , 'O' , 'W' , 'D') THEN " + VAF_Org_ID + @"
                                            ELSE 0  END) = CST.VAF_Org_ID)
                            AND ((     CASE WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('A' , 'B', 'D')  THEN " + M_ASI_ID + @"
                                            WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('C' , 'O', 'W')  THEN 0 
                                            WHEN ACC.COSTINGLEVEL IS NOT NULL AND ACC.COSTINGLEVEL   IN ('A' , 'B' , 'D') THEN " + M_ASI_ID + @"
                                            ELSE 0   END) = NVL(CST.VAM_PFeature_SetInstance_ID , 0))
                            AND ((     CASE WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('W' ,'D')  THEN " + VAM_Warehouse_ID + @"
                                             WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('A' ,'B' , 'C' ,'O')  THEN 0 
                                            WHEN ACC.COSTINGLEVEL IS NOT NULL AND ACC.COSTINGLEVEL   IN ('W' ,'D') THEN " + VAM_Warehouse_ID + @"
                                            ELSE 0   END) = NVL(CST.VAM_Warehouse_ID , 0))
                            AND P.VAM_Product_ID      =" + product.GetVAM_Product_ID() + @"
                            AND CST.VAB_ACCOUNTBOOK_ID = " + acctSchema.GetVAB_AccountBook_ID();
                qty = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
            }
            catch { }
            return qty;
        }

        public static Decimal CalculateCostQueuePrice(MVABInvoice invoice, MVABInvoiceLine invoiceLine, Decimal price, MVABAccountBook acctSchema, int VAF_Client_ID, int VAF_Org_ID, MVAMProductCostElement ce, Trx trxName, bool iscostImmdiate, string optionalStr, MVAMInvInOutLine matchInoutLine)
        {
            // if inoutline not available the return 0
            if (matchInoutLine == null)
            {
                matchInoutLine = new MVAMInvInOutLine(invoiceLine.GetCtx(), invoiceLine.GetVAM_Inv_InOutLine_ID(), trxName);
            }
            if (matchInoutLine == null || matchInoutLine.GetVAM_Inv_InOutLine_ID() <= 0)
            {
                return 0;
            }

            Decimal ProductLineCost = invoiceLine.GetProductLineCost(invoiceLine);
            Decimal ProductInvoicePriceActual = ProductLineCost / invoiceLine.GetQtyEntered();
            Decimal MRQtyConsumed = 0;
            Decimal PriceLifoOrFifo = 0;
            Decimal surchargeAmount = 0;
            string isCostAdjustmentOnLost = "N";
            try
            {
                string sql = @"SELECT COUNT(*) FROM VAF_Column WHERE IsActive = 'Y' AND 
                                       VAF_TableView_ID =  ( SELECT VAF_TableView_ID FROM VAF_TableView WHERE IsActive = 'Y' AND TableName LIKE 'VAM_Product' ) 
                                       AND ColumnName LIKE 'IsCostAdjustmentOnLost' ";
                int count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                if (count > 0)
                {
                    sql = @"SELECT IsCostAdjustmentOnLost FROM VAM_Product WHERE VAM_Product_ID =" + invoiceLine.GetVAM_Product_ID();
                    isCostAdjustmentOnLost = Util.GetValueOfString(DB.ExecuteScalar(sql, null, null));
                }


                sql = @"SELECT ROUND( il.linenetamt/il.qtyinvoiced , 4) as priceactual,
                            CASE
                                WHEN NVL(C_SurChargeTax.IsIncludeInCost , 'N') = 'Y'
                                AND NVL(VAB_TaxRate.IsIncludeInCost , 'N')           = 'Y'
                                THEN ROUND((il.taxbaseamt + il.taxamt + il.surchargeamt) / il.qtyinvoiced , 4)
                                WHEN NVL(C_SurChargeTax.IsIncludeInCost , 'N') = 'N'
                                AND NVL(VAB_TaxRate.IsIncludeInCost , 'N')           = 'Y'
                                THEN ROUND((il.taxbaseamt + il.taxamt) / il.qtyinvoiced , 4)
                                WHEN NVL(C_SurChargeTax.IsIncludeInCost , 'N') = 'Y'
                                AND NVL(VAB_TaxRate.IsIncludeInCost , 'N')           = 'N'
                                THEN ROUND((il.taxbaseamt + il.surchargeamt) / il.qtyinvoiced, 4)
                                ELSE ROUND(il.taxbaseamt  / il.qtyinvoiced, 4)
                              END AS PriceActualIncludedTax,
                                                   il.qtyinvoiced as qtyentered,  i.VAB_Currency_id ,  i.VAB_CurrencyType_id ,
                                                   il.VAB_InvoiceLine_id ,  iol.VAM_Inv_InOut_id , il.VAM_Inv_InOutLine_id  , i.dateacct , iol.movementqty AS mrqty
                                              FROM VAM_Inv_InOutLine iol INNER JOIN VAB_InvoiceLine il ON il.VAM_Inv_InOutLine_id = iol.VAM_Inv_InOutLine_id
                                                   INNER JOIN VAB_TaxRate VAB_TaxRate ON VAB_TaxRate.VAB_TaxRate_ID = il.VAB_TaxRate_ID
                                                   LEFT JOIN VAB_TaxRate C_SurChargeTax ON VAB_TaxRate.Surcharge_Tax_ID = C_SurChargeTax.VAB_TaxRate_ID
                                                   INNER JOIN VAB_Invoice i ON i.VAB_Invoice_id = il.VAB_Invoice_id
                                              WHERE il.IsActive = 'Y' AND i.DocStatus IN ('CO' , 'CL') AND i.issotrx = 'N' and i.isreturntrx = 'N'
                                                    AND iol.VAM_Inv_InOutLine_ID = " + matchInoutLine.GetVAM_Inv_InOutLine_ID();
                DataSet dsInvoiceRecord = new DataSet();
                dsInvoiceRecord = DB.ExecuteDataset(sql, null, trxName);
                if (dsInvoiceRecord != null && dsInvoiceRecord.Tables.Count > 0 && dsInvoiceRecord.Tables[0].Rows.Count > 0)
                {
                    Decimal PriceActual = 0;
                    for (int i = 0; i < dsInvoiceRecord.Tables[0].Rows.Count; i++)
                    {
                        surchargeAmount = 0;
                        if (i == 0)
                        {
                            PriceLifoOrFifo = price * Util.GetValueOfDecimal(dsInvoiceRecord.Tables[0].Rows[i]["mrqty"]);
                            MRQtyConsumed = Util.GetValueOfDecimal(dsInvoiceRecord.Tables[0].Rows[i]["mrqty"]);
                        }

                        PriceActual = Util.GetValueOfDecimal(dsInvoiceRecord.Tables[0].Rows[i]["PriceActualIncludedTax"]) != 0 ?
                                        Util.GetValueOfDecimal(dsInvoiceRecord.Tables[0].Rows[i]["PriceActualIncludedTax"]) :
                                        Util.GetValueOfDecimal(dsInvoiceRecord.Tables[0].Rows[i]["priceactual"]);

                        // for all partial invoice complated against singlr MR
                        if (MRQtyConsumed >= Util.GetValueOfDecimal(dsInvoiceRecord.Tables[0].Rows[i]["qtyentered"]) && isCostAdjustmentOnLost == "Y")
                        {
                            PriceLifoOrFifo -= (price * Util.GetValueOfDecimal(dsInvoiceRecord.Tables[0].Rows[i]["qtyentered"]));
                        }
                        else if (isCostAdjustmentOnLost == "Y")
                        {
                            PriceLifoOrFifo -= (price * MRQtyConsumed);
                        }
                        else
                        {
                            PriceLifoOrFifo -= (price * Util.GetValueOfDecimal(dsInvoiceRecord.Tables[0].Rows[i]["qtyentered"]));
                        }
                        if (Util.GetValueOfInt(dsInvoiceRecord.Tables[0].Rows[i]["VAB_Currency_id"]) != acctSchema.GetVAB_Currency_ID())
                        {
                            surchargeAmount = MVABExchangeRate.Convert(invoiceLine.GetCtx(),
                                isCostAdjustmentOnLost == "Y" && MRQtyConsumed < Util.GetValueOfDecimal(dsInvoiceRecord.Tables[0].Rows[i]["qtyentered"]) ? decimal.Multiply(PriceActual, MRQtyConsumed)
                                : decimal.Multiply(PriceActual, Util.GetValueOfDecimal(dsInvoiceRecord.Tables[0].Rows[i]["qtyentered"])),
                                                  Util.GetValueOfInt(dsInvoiceRecord.Tables[0].Rows[i]["VAB_Currency_id"]), acctSchema.GetVAB_Currency_ID(),
                                                  Util.GetValueOfDateTime(dsInvoiceRecord.Tables[0].Rows[i]["dateacct"]),
                                                  Util.GetValueOfInt(dsInvoiceRecord.Tables[0].Rows[i]["VAB_CurrencyType_id"]), VAF_Client_ID, VAF_Org_ID);
                            if (PriceActual > 0 && surchargeAmount == 0)
                            {
                                return 0;
                            }
                            PriceLifoOrFifo += Decimal.Round(Decimal.Add(surchargeAmount, Decimal.Divide(Decimal.Multiply(surchargeAmount, ce.GetSurchargePercentage()), 100)), acctSchema.GetCostingPrecision());
                        }
                        else
                        {
                            surchargeAmount = isCostAdjustmentOnLost == "Y" && MRQtyConsumed < Util.GetValueOfDecimal(dsInvoiceRecord.Tables[0].Rows[i]["qtyentered"]) ? decimal.Multiply(Util.GetValueOfDecimal(dsInvoiceRecord.Tables[0].Rows[i]["priceactual"]), MRQtyConsumed)
                                : decimal.Multiply(PriceActual, Util.GetValueOfDecimal(dsInvoiceRecord.Tables[0].Rows[i]["qtyentered"]));
                            PriceLifoOrFifo += Decimal.Round(Decimal.Add(surchargeAmount, Decimal.Divide(Decimal.Multiply(surchargeAmount, ce.GetSurchargePercentage()), 100)), acctSchema.GetCostingPrecision());
                        }
                        MRQtyConsumed -= Util.GetValueOfDecimal(dsInvoiceRecord.Tables[0].Rows[i]["qtyentered"]);
                        //end
                        if (i == dsInvoiceRecord.Tables[0].Rows.Count - 1)
                        {
                            // when we are calculating cost on completion / Optional Str = "window
                            if (iscostImmdiate && optionalStr == "window")
                            {
                                if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                {
                                    // 
                                }
                                else
                                {
                                    PriceLifoOrFifo -= price * invoiceLine.GetQtyEntered();
                                    if (invoice.GetVAB_Currency_ID() != acctSchema.GetVAB_Currency_ID())
                                    {
                                        PriceLifoOrFifo += MVABExchangeRate.Convert(invoiceLine.GetCtx(), (ProductInvoicePriceActual * invoiceLine.GetQtyEntered()), invoice.GetVAB_Currency_ID(), acctSchema.GetVAB_Currency_ID(),
                                                                                 invoice.GetDateAcct(), invoice.GetVAB_CurrencyType_ID(), VAF_Client_ID, VAF_Org_ID);
                                    }
                                    else
                                    {
                                        PriceLifoOrFifo += (ProductInvoicePriceActual * invoiceLine.GetQtyEntered());
                                    }
                                }
                            }
                            PriceLifoOrFifo = Decimal.Round(PriceLifoOrFifo / Util.GetValueOfDecimal(dsInvoiceRecord.Tables[0].Rows[i]["mrqty"]), acctSchema.GetCostingPrecision());
                        }
                    }
                }
                else
                {
                    MVAMInvInOutLine VOinoutline = new MVAMInvInOutLine(invoiceLine.GetCtx(), matchInoutLine.GetVAM_Inv_InOutLine_ID(), invoiceLine.Get_TrxName());
                    PriceLifoOrFifo = price * VOinoutline.GetMovementQty();

                    // change 23-aug-2016
                    if (invoice != null && invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                    {
                        //at this moment single invoice was created against MR, 
                        //we are rollback the impact of Invoice
                        PriceLifoOrFifo = Decimal.Round((PriceLifoOrFifo / VOinoutline.GetMovementQty()), acctSchema.GetCostingPrecision());
                    }
                    else
                    {
                        // when isCostAdjustableOnLost = true on product and movement qty on MR is less than invoice qty then consider MR qty else invoice qty
                        if (isCostAdjustmentOnLost == "Y" && VOinoutline.GetMovementQty() < invoiceLine.GetQtyInvoiced())
                        {
                            PriceLifoOrFifo -= price * VOinoutline.GetMovementQty();
                        }
                        else
                        {
                            PriceLifoOrFifo -= price * invoiceLine.GetQtyInvoiced();
                        }

                        if (invoice.GetVAB_Currency_ID() != acctSchema.GetVAB_Currency_ID())
                        {
                            surchargeAmount = MVABExchangeRate.Convert(invoiceLine.GetCtx(), (ProductLineCost), invoice.GetVAB_Currency_ID(), acctSchema.GetVAB_Currency_ID(),
                                                                     invoice.GetDateAcct(), invoice.GetVAB_CurrencyType_ID(), VAF_Client_ID, VAF_Org_ID);
                            if (ProductLineCost > 0 && surchargeAmount == 0)
                            {
                                return 0;
                            }
                            PriceLifoOrFifo += Decimal.Round(Decimal.Add(surchargeAmount, Decimal.Divide(Decimal.Multiply(surchargeAmount, ce.GetSurchargePercentage()), 100)), acctSchema.GetCostingPrecision());
                            PriceLifoOrFifo = Decimal.Round((PriceLifoOrFifo / VOinoutline.GetMovementQty()), acctSchema.GetCostingPrecision());
                        }
                        else
                        {
                            surchargeAmount = (ProductLineCost);
                            PriceLifoOrFifo += Decimal.Round(Decimal.Add(surchargeAmount, Decimal.Divide(Decimal.Multiply(surchargeAmount, ce.GetSurchargePercentage()), 100)), acctSchema.GetCostingPrecision());
                            PriceLifoOrFifo = Decimal.Round((PriceLifoOrFifo / VOinoutline.GetMovementQty()), acctSchema.GetCostingPrecision());
                        }
                    }
                }
                if (dsInvoiceRecord != null)
                {
                    dsInvoiceRecord.Dispose();
                }
            }
            catch (Exception ex)
            {
                ValueNamePair pp = VLogger.RetrieveError();
                _log.Info("Error occured during calculation of Cost Queue Price. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
            }
            return PriceLifoOrFifo;
        }

        /// <summary>
        /// is used to get price from price list and convert it into "currency to" with default "currency type" and "acct date" of Inout
        /// </summary>
        /// <param name="inoutline">Line reference</param>
        /// <param name="CurrencyTo">converted currency</param>
        /// <returns>amount</returns>
        public static Decimal GetPrice(MVAMInvInOutLine inoutline, int CurrencyTo)
        {
            return GetPrice(null, inoutline, CurrencyTo);
        }

        /// <summary>
        /// is used to get price from price list and convert it into "currency to" with default "currency type" and "acct date" of Inout
        /// </summary>
        /// <param name="inout">Inout reference</param>
        /// <param name="inoutline">InoutLine reference</param>
        /// <param name="CurrencyTo">converted currency</param>
        /// <returns>amount</returns>
        public static Decimal GetPrice(MVAMInvInOut inout, MVAMInvInOutLine inoutline, int CurrencyTo)
        {
            decimal amount = 0;
            int VAB_Currency_id = 0;
            DataSet dsStdPrice = new DataSet();
            if (inout == null)
            {
                inout = new MVAMInvInOut(inoutline.GetCtx(), inoutline.GetVAM_Inv_InOut_ID(), inoutline.Get_Trx());
            }
            MVABBusinessPartner bpartner = new MVABBusinessPartner(inout.GetCtx(), inout.GetVAB_BusinessPartner_ID(), inout.Get_Trx());
            if (bpartner.GetPO_PriceList_ID() > 0)
            {
                MPriceList pl = new MPriceList(bpartner.GetCtx(), bpartner.GetPO_PriceList_ID(), bpartner.Get_Trx());
                VAB_Currency_id = pl.GetVAB_Currency_ID();
            }
            string sql = "";
            try
            {
                if (bpartner.GetPO_PriceList_ID() > 0)
                {
                    #region when price list available at Customer/Vendor
                    sql = @"SELECT pricestd FROM VAM_ProductPrice WHERE isactive = 'Y' AND VAM_Product_id = " + inoutline.GetVAM_Product_ID() + " AND NVL(VAM_PFeature_SetInstance_id, 0) = " + inoutline.GetVAM_PFeature_SetInstance_ID() +
                        " AND VAB_UOM_id = " + inoutline.GetVAB_UOM_ID() + " AND VAM_PriceListVersion_id =   (SELECT MAX(plv.VAM_PriceListVersion_id)   FROM VAM_PriceList pl   INNER JOIN VAM_PriceListVersion plv " +
                                " ON pl.VAM_PriceList_id    = plv.VAM_PriceList_id   WHERE plv.IsActive = 'Y' AND  pl.VAM_PriceList_id = " + bpartner.GetPO_PriceList_ID() + ")";
                    dsStdPrice = DB.ExecuteDataset(sql, null, null);
                    if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                    {
                        amount = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                    }
                    if (amount == 0)
                    {
                        sql = @"(SELECT pl.VAF_Org_ID , pl.name , pl.VAM_PriceList_id  , plv.VAM_PriceListVersion_id , pp.pricestd , pl.VAB_Currency_id
                                           FROM VAM_PriceList pl  INNER JOIN VAM_PriceListVersion plv   ON pl.VAM_PriceList_id    = plv.VAM_PriceList_id
                                            inner join VAM_ProductPrice pp   on pp.VAM_PriceListVersion_id = plv.VAM_PriceListVersion_id
                                           WHERE pl.VAF_Org_ID = " + inoutline.GetVAF_Org_ID() + " AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.isactive = 'Y' and pp.VAM_Product_id = " + inoutline.GetVAM_Product_ID() +
                            " AND NVL(pp.VAM_PFeature_SetInstance_id, 0) = " + inoutline.GetVAM_PFeature_SetInstance_ID() +
                            " AND pp.VAB_UOM_id = " + inoutline.GetVAB_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.VAM_PriceList_id asc";
                        dsStdPrice = DB.ExecuteDataset(sql, null, null);
                        if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                        {
                            amount = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                            VAB_Currency_id = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["VAB_Currency_id"]);
                        }
                        else
                        {
                            sql = @"(SELECT pl.VAF_Org_ID , pl.name , pl.VAM_PriceList_id  , plv.VAM_PriceListVersion_id , pp.pricestd , pl.VAB_Currency_id
                                           FROM VAM_PriceList pl  INNER JOIN VAM_PriceListVersion plv   ON pl.VAM_PriceList_id    = plv.VAM_PriceList_id
                                            inner join VAM_ProductPrice pp   on pp.VAM_PriceListVersion_id = plv.VAM_PriceListVersion_id
                                           WHERE pl.VAF_Org_ID = 0 AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.isactive = 'Y' and pp.VAM_Product_id = " + inoutline.GetVAM_Product_ID() +
                                   " AND NVL(pp.VAM_PFeature_SetInstance_id, 0) = " + inoutline.GetVAM_PFeature_SetInstance_ID() +
                                   " AND pp.VAB_UOM_id = " + inoutline.GetVAB_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.VAM_PriceList_id asc , pl.VAF_Org_ID desc";
                            dsStdPrice = DB.ExecuteDataset(sql, null, null);
                            if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                            {
                                amount = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                                VAB_Currency_id = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["VAB_Currency_id"]);
                            }
                            else
                            {
                                sql = @"(SELECT pl.VAF_Org_ID , pl.name , pl.VAM_PriceList_id  , plv.VAM_PriceListVersion_id , pp.pricestd , pl.VAB_Currency_id
                                           FROM VAM_PriceList pl  INNER JOIN VAM_PriceListVersion plv   ON pl.VAM_PriceList_id    = plv.VAM_PriceList_id
                                            inner join VAM_ProductPrice pp   on pp.VAM_PriceListVersion_id = plv.VAM_PriceListVersion_id
                                           WHERE pl.VAF_Org_ID = " + inoutline.GetVAF_Org_ID() + " AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.isactive = 'Y' and pp.VAM_Product_id = " + inoutline.GetVAM_Product_ID() +
                               " AND pp.VAB_UOM_id = " + inoutline.GetVAB_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.VAM_PriceList_id asc";
                                dsStdPrice = DB.ExecuteDataset(sql, null, null);
                                if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                                {
                                    amount = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                                    VAB_Currency_id = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["VAB_Currency_id"]);
                                }
                                else
                                {
                                    sql = @"(SELECT pl.VAF_Org_ID , pl.name , pl.VAM_PriceList_id  , plv.VAM_PriceListVersion_id , pp.pricestd , pl.VAB_Currency_id
                                           FROM VAM_PriceList pl  INNER JOIN VAM_PriceListVersion plv   ON pl.VAM_PriceList_id    = plv.VAM_PriceList_id
                                            inner join VAM_ProductPrice pp   on pp.VAM_PriceListVersion_id = plv.VAM_PriceListVersion_id
                                           WHERE pl.VAF_Org_ID = 0 AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.isactive = 'Y' and pp.VAM_Product_id = " + inoutline.GetVAM_Product_ID() +
                                           " AND pp.VAB_UOM_id = " + inoutline.GetVAB_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.VAM_PriceList_id asc , pl.VAF_Org_ID desc";
                                    dsStdPrice = DB.ExecuteDataset(sql, null, null);
                                    if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                                    {
                                        amount = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                                        VAB_Currency_id = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["VAB_Currency_id"]);
                                    }
                                    else
                                    {
                                        amount = 0;
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region Get Price from price list
                    sql = @"(SELECT pl.VAF_Org_ID , pl.name , pl.VAM_PriceList_id  , plv.VAM_PriceListVersion_id , pp.pricestd , pl.VAB_Currency_id
                                           FROM VAM_PriceList pl  INNER JOIN VAM_PriceListVersion plv   ON pl.VAM_PriceList_id    = plv.VAM_PriceList_id
                                            inner join VAM_ProductPrice pp   on pp.VAM_PriceListVersion_id = plv.VAM_PriceListVersion_id
                                           WHERE pl.VAF_Org_ID = " + inoutline.GetVAF_Org_ID() + " AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.isactive = 'Y' and pp.VAM_Product_id = " + inoutline.GetVAM_Product_ID() +
                           " AND NVL(pp.VAM_PFeature_SetInstance_id, 0) = " + inoutline.GetVAM_PFeature_SetInstance_ID() +
                           " AND pp.VAB_UOM_id = " + inoutline.GetVAB_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.VAM_PriceList_id asc";
                    dsStdPrice = DB.ExecuteDataset(sql, null, null);
                    if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                    {
                        amount = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                        VAB_Currency_id = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["VAB_Currency_id"]);
                    }
                    else
                    {
                        sql = @"(SELECT pl.VAF_Org_ID , pl.name , pl.VAM_PriceList_id  , plv.VAM_PriceListVersion_id , pp.pricestd , pl.VAB_Currency_id
                                           FROM VAM_PriceList pl  INNER JOIN VAM_PriceListVersion plv   ON pl.VAM_PriceList_id    = plv.VAM_PriceList_id
                                            inner join VAM_ProductPrice pp   on pp.VAM_PriceListVersion_id = plv.VAM_PriceListVersion_id
                                           WHERE pl.VAF_Org_ID = 0 AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.isactive = 'Y' and pp.VAM_Product_id = " + inoutline.GetVAM_Product_ID() +
                               " AND NVL(pp.VAM_PFeature_SetInstance_id, 0) = " + inoutline.GetVAM_PFeature_SetInstance_ID() +
                               " AND pp.VAB_UOM_id = " + inoutline.GetVAB_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.VAM_PriceList_id asc , pl.VAF_Org_ID desc";
                        dsStdPrice = DB.ExecuteDataset(sql, null, null);
                        if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                        {
                            amount = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                            VAB_Currency_id = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["VAB_Currency_id"]);
                        }
                        else
                        {
                            sql = @"(SELECT pl.VAF_Org_ID , pl.name , pl.VAM_PriceList_id  , plv.VAM_PriceListVersion_id , pp.pricestd , pl.VAB_Currency_id
                                           FROM VAM_PriceList pl  INNER JOIN VAM_PriceListVersion plv   ON pl.VAM_PriceList_id    = plv.VAM_PriceList_id
                                            inner join VAM_ProductPrice pp   on pp.VAM_PriceListVersion_id = plv.VAM_PriceListVersion_id
                                           WHERE pl.VAF_Org_ID = " + inoutline.GetVAF_Org_ID() + " AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.isactive = 'Y' and pp.VAM_Product_id = " + inoutline.GetVAM_Product_ID() +
                           " AND pp.VAB_UOM_id = " + inoutline.GetVAB_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.VAM_PriceList_id asc";
                            dsStdPrice = DB.ExecuteDataset(sql, null, null);
                            if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                            {
                                amount = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                                VAB_Currency_id = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["VAB_Currency_id"]);
                            }
                            else
                            {
                                sql = @"(SELECT pl.VAF_Org_ID , pl.name , pl.VAM_PriceList_id  , plv.VAM_PriceListVersion_id , pp.pricestd , pl.VAB_Currency_id
                                           FROM VAM_PriceList pl  INNER JOIN VAM_PriceListVersion plv   ON pl.VAM_PriceList_id    = plv.VAM_PriceList_id
                                            inner join VAM_ProductPrice pp   on pp.VAM_PriceListVersion_id = plv.VAM_PriceListVersion_id
                                           WHERE pl.VAF_Org_ID = 0 AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.isactive = 'Y' and pp.VAM_Product_id = " + inoutline.GetVAM_Product_ID() +
                                       " AND pp.VAB_UOM_id = " + inoutline.GetVAB_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.VAM_PriceList_id asc , pl.VAF_Org_ID desc";
                                dsStdPrice = DB.ExecuteDataset(sql, null, null);
                                if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                                {
                                    amount = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                                    VAB_Currency_id = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["VAB_Currency_id"]);
                                }
                                else
                                {
                                    amount = 0;
                                }
                            }
                        }
                    }
                    #endregion
                }

                amount = amount * inoutline.GetQtyEntered();
                // conversion of amount
                if (VAB_Currency_id != 0 && VAB_Currency_id != CurrencyTo)
                {
                    amount = MVABExchangeRate.Convert(inout.GetCtx(), amount, VAB_Currency_id, CurrencyTo,
                                                                            inout.GetDateAcct(), 0, inout.GetVAF_Client_ID(), inout.GetVAF_Org_ID());
                }
            }
            catch (Exception ex)
            {
                if (dsStdPrice != null)
                {
                    dsStdPrice.Dispose();
                }
                ValueNamePair pp = VLogger.RetrieveError();
                _log.Info("Error occured during GetPrice Method. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName() +
                           " And Exception message : " + ex.Message.ToString());
            }
            finally
            {
                if (dsStdPrice != null)
                {
                    dsStdPrice.Dispose();
                }
            }
            return amount;
        }

        /// <summary>
        /// Is used to get price based on costing method either binded on Product Category or on Accounting schema
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="acctSchema">accounting schema</param>
        /// <param name="product">product</param>
        /// <param name="M_ASI_ID">accounting schema</param>
        /// <param name="VAF_Client_ID">client</param>
        /// <param name="VAF_Org_ID">org</param>
        /// <param name="VAM_Warehouse_ID">warehouse</param>
        /// <returns>PRICE</returns>
        private static Decimal CalculateCost(Ctx ctx, MVABAccountBook acctSchema, MProduct product, int M_ASI_ID, int VAF_Client_ID, int VAF_Org_ID, int VAM_Warehouse_ID)
        {
            Decimal price = 0;
            int costElementID = 0;
            string sql = "";
            dynamic pc = null;

            pc = new MProductCategory(ctx, product.GetVAM_ProductCategory_ID(), null);
            String costingLevel = "";
            try
            {
                if (pc != null && !String.IsNullOrEmpty(pc.GetCostingMethod()))
                {
                    //get cost element frpom product category in case of costing method
                    if (pc.GetCostingMethod() == "C")
                    {
                        costElementID = pc.GetVAM_ProductCostElement_ID();
                    }
                    else
                    {
                        sql = @"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = '" + pc.GetCostingMethod() + "' AND VAF_Client_ID = " + VAF_Client_ID;
                        costElementID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    }
                }
                else
                {
                    //get cost element frpom accouting Schema in case of costing method
                    if (acctSchema.GetCostingMethod() == "C")
                    {
                        costElementID = acctSchema.GetVAM_ProductCostElement_ID();
                    }
                    else
                    {
                        sql = @"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = '" + acctSchema.GetCostingMethod() + "' AND VAF_Client_ID = " + VAF_Client_ID;
                        costElementID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    }
                }


                if (pc != null && pc.GetCostingLevel() != null)
                {
                    costingLevel = pc.GetCostingLevel();
                }
                else
                {
                    costingLevel = acctSchema.GetCostingLevel();
                }
                if (costingLevel == MProductCategory.COSTINGLEVEL_BatchLot) // Batch/Lot
                {
                    sql = "SELECT CurrentCostPrice FROM VAM_ProductCost WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                           " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + M_ASI_ID + " AND  VAM_ProductCostElement_ID = " + costElementID +
                           " AND VAF_Client_ID = " + VAF_Client_ID + " AND VAF_Org_ID = 0";
                }
                else if (costingLevel == MProductCategory.COSTINGLEVEL_OrgPlusBatch) // batch + Org
                {
                    sql = "SELECT CurrentCostPrice FROM VAM_ProductCost WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                           " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + M_ASI_ID + " AND  VAM_ProductCostElement_ID = " + costElementID +
                           " AND VAF_Client_ID = " + VAF_Client_ID + " AND VAF_Org_ID = " + VAF_Org_ID;
                }
                else if (costingLevel == MProductCategory.COSTINGLEVEL_Client) // client
                {
                    sql = "SELECT CurrentCostPrice FROM VAM_ProductCost WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                              " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + "  AND VAM_ProductCostElement_ID = " + costElementID +
                              " AND VAF_Client_ID = " + VAF_Client_ID + " AND VAF_Org_ID = 0";
                }
                else if (costingLevel == MProductCategory.COSTINGLEVEL_Organization) // org
                {
                    sql = "SELECT CurrentCostPrice FROM VAM_ProductCost WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                              " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND VAM_ProductCostElement_ID = " + costElementID +
                              " AND VAF_Client_ID = " + VAF_Client_ID + " AND VAF_Org_ID = " + VAF_Org_ID;
                }
                else if (costingLevel == MProductCategory.COSTINGLEVEL_Warehouse) // Warehouse + Org
                {
                    sql = "SELECT CurrentCostPrice FROM VAM_ProductCost WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                              " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND VAM_ProductCostElement_ID = " + costElementID +
                              " AND VAF_Client_ID = " + VAF_Client_ID + " AND VAF_Org_ID = " + VAF_Org_ID + " AND VAM_Warehouse_ID = " + VAM_Warehouse_ID;
                }
                else if (costingLevel == MProductCategory.COSTINGLEVEL_WarehousePlusBatch) // Warehouse + batch + Org
                {
                    sql = "SELECT CurrentCostPrice FROM VAM_ProductCost WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                           " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + M_ASI_ID + " AND  VAM_ProductCostElement_ID = " + costElementID +
                           " AND VAF_Client_ID = " + VAF_Client_ID + " AND VAF_Org_ID = " + VAF_Org_ID + " AND VAM_Warehouse_ID = " + VAM_Warehouse_ID;
                }
                price = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
            }
            catch (Exception ex)
            {
                _log.Info("Error Occured during costing of this method : CalculateCost =>  " + ex.Message.ToString());
            }
            return price;
        }

        private static bool CreateCostQueue(Ctx ctx, MVABAccountBook acctSchema, MProduct product, int M_ASI_ID, int VAF_Client_ID, int VAF_Org_ID, Decimal Price, Decimal Qty,
                               string windowName, MVAMInventoryLine inventoryLine, MVAMInvInOutLine inoutline, MVAMInvTrfLine movementline,
                               MVABInvoiceLine invoiceline, MVAMProductCostDetail cd, Trx trxName, out string costQueueIds)
        {
            costQueueIds = null;
            if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM vaf_tableview WHERE lower(tablename) LIKE 't_temp_costdetail'")) <= 0)
            {
                return false;
            }
            String sql = "";
            dynamic pc = null;
            decimal priceLifo = 0;
            decimal priceFifo = 0;
            string policy = null;
            int VAM_ProductCostElement_ID = 0;
            MVAMProductCostElement ce = null;
            decimal amtWithSurcharge = 0;
            int M_SourceWarehouse_ID = 0;
            // change 2-5-2016
            try
            {
                if (windowName == "Physical Inventory" || windowName == "Internal Use Inventory" ||
                    windowName == "Material Receipt" || windowName == "Inventory Move" ||
                    windowName == "Customer Return" || windowName == "Return To Vendor" || windowName == "Shipment" ||
                    windowName == "Production Execution" || windowName == "AssetDisposal")
                {
                    String cl = null;

                    #region get Costing Level and Policy
                    if (product != null)
                    {
                        pc = MProductCategory.Get(product.GetCtx(), product.GetVAM_ProductCategory_ID());
                        if (pc != null)
                        {
                            cl = pc.GetCostingLevel();
                            policy = pc.GetMMPolicy();
                        }
                    }
                    if (cl == null)
                    {
                        cl = acctSchema.GetCostingLevel();
                        if (pc != null)
                            policy = pc.GetMMPolicy();
                        else
                        {
                            pc = MProductCategory.Get(product.GetCtx(), product.GetVAM_ProductCategory_ID());
                            if (pc != null)
                                policy = pc.GetMMPolicy();

                        }
                    }
                    #endregion

                    if (windowName == "Inventory Move")
                    {
                        // is used to get source warehouse, for picking cost from source warehouse
                        M_SourceWarehouse_ID = Convert.ToInt32(DB.ExecuteScalar("SELECT DTD001_MWarehouseSource_ID FROM VAM_InventoryTransfer WHERE VAM_InventoryTransfer_ID = " + movementline.GetVAM_InventoryTransfer_ID(), null, trxName));
                    }

                    #region fifo cost
                    if (cl == MProductCategory.COSTINGLEVEL_BatchLot)
                    {
                        sql = "SELECT CurrentCostPrice FROM VAM_ProductCost WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                               " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + M_ASI_ID + " AND  VAM_ProductCostElement_ID = " +
                               " ( SELECT MIN(VAM_ProductCostElement_ID) FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND VAF_Client_ID = " + VAF_Client_ID + ") " +
                               " AND VAF_Client_ID = " + VAF_Client_ID + " AND VAF_Org_ID = 0";
                    }
                    else if (cl == MProductCategory.COSTINGLEVEL_Client)
                    {
                        sql = "SELECT CurrentCostPrice FROM VAM_ProductCost WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                  " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + "  AND VAM_ProductCostElement_ID = " +
                                  " ( SELECT MIN(VAM_ProductCostElement_ID) FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND VAF_Client_ID = " + VAF_Client_ID + ") " +
                                  " AND VAF_Client_ID = " + VAF_Client_ID + " AND VAF_Org_ID = 0";
                    }
                    else if (cl == MProductCategory.COSTINGLEVEL_Organization)
                    {
                        if (windowName == "Inventory Move")
                        {
                            sql = "SELECT CurrentCostPrice FROM VAM_ProductCost WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                     " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND VAM_ProductCostElement_ID = " +
                                     " ( SELECT MIN(VAM_ProductCostElement_ID) FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND VAF_Client_ID = " + VAF_Client_ID + ") " +
                                     " AND VAF_Client_ID = " + VAF_Client_ID + " AND VAF_Org_ID = " + movementline.GetVAF_Org_ID();
                        }
                        else
                        {
                            sql = "SELECT CurrentCostPrice FROM VAM_ProductCost WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                      " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND VAM_ProductCostElement_ID = " +
                                      " ( SELECT MIN(VAM_ProductCostElement_ID) FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND VAF_Client_ID = " + VAF_Client_ID + ") " +
                                      " AND VAF_Client_ID = " + VAF_Client_ID + " AND VAF_Org_ID = " + VAF_Org_ID;
                        }
                    }
                    else if (cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch || cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                    {
                        if (windowName == "Inventory Move")
                        {
                            sql = "SELECT CurrentCostPrice FROM VAM_ProductCost WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID) +
                                " AND VAM_ProductCostElement_ID = " +
                                     " ( SELECT MIN(VAM_ProductCostElement_ID) FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND VAF_Client_ID = " + VAF_Client_ID + ") " +
                                     " AND VAF_Client_ID = " + VAF_Client_ID + " AND VAF_Org_ID = " + movementline.GetVAF_Org_ID();
                            if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                            {
                                sql += " AND VAM_Warehouse_ID = " + M_SourceWarehouse_ID;//cd.GetVAM_Warehouse_ID();
                            }
                        }
                        else
                        {
                            sql = "SELECT CurrentCostPrice FROM VAM_ProductCost WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                      " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID) + " AND VAM_ProductCostElement_ID = " +
                                      " ( SELECT MIN(VAM_ProductCostElement_ID) FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND VAF_Client_ID = " + VAF_Client_ID + ") " +
                                      " AND VAF_Client_ID = " + VAF_Client_ID + " AND VAF_Org_ID = " + VAF_Org_ID;
                            if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                            {
                                sql += " AND VAM_Warehouse_ID = " + cd.GetVAM_Warehouse_ID();
                            }
                        }
                    }
                    priceFifo = Util.GetValueOfDecimal(DB.ExecuteScalar(sql));

                    #region on fly calculation
                    if (priceFifo <= 0)
                    {
                        if (cl == MProductCategory.COSTINGLEVEL_BatchLot)
                        {
                            sql = "SELECT ROUND(sum(currentcostprice * currentqty)/ sum(currentqty) , " + acctSchema.GetCostingPrecision() + @") FROM VAM_ProductCostQueue WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                   " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + M_ASI_ID + " AND  VAM_ProductCostElement_ID = " +
                                   " ( SELECT MIN(VAM_ProductCostElement_ID) FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND VAF_Client_ID = " + VAF_Client_ID + ") " +
                                   " AND VAF_Client_ID = " + VAF_Client_ID;
                        }
                        else if (cl == MProductCategory.COSTINGLEVEL_Client)
                        {
                            sql = "SELECT ROUND(sum(currentcostprice * currentqty)/ sum(currentqty) , " + acctSchema.GetCostingPrecision() + @") FROM VAM_ProductCostQueue WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                      " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + "  AND VAM_ProductCostElement_ID = " +
                                      " ( SELECT MIN(VAM_ProductCostElement_ID) FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND VAF_Client_ID = " + VAF_Client_ID + ") " +
                                      " AND VAF_Client_ID = " + VAF_Client_ID;
                        }
                        else if (cl == MProductCategory.COSTINGLEVEL_Organization)
                        {
                            if (windowName == "Inventory Move")
                            {
                                sql = "SELECT ROUND(sum(currentcostprice * currentqty)/ sum(currentqty) , " + acctSchema.GetCostingPrecision() + @") FROM VAM_ProductCostQueue WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                         " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND VAM_ProductCostElement_ID = " +
                                         " ( SELECT MIN(VAM_ProductCostElement_ID) FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND VAF_Client_ID = " + VAF_Client_ID + ") " +
                                         " AND VAF_Client_ID = " + VAF_Client_ID + " AND VAF_Org_ID = " + movementline.GetVAF_Org_ID();
                            }
                            else
                            {
                                sql = "SELECT ROUND(sum(currentcostprice * currentqty)/ sum(currentqty) , " + acctSchema.GetCostingPrecision() + @") FROM VAM_ProductCostQueue WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                          " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND VAM_ProductCostElement_ID = " +
                                          " ( SELECT MIN(VAM_ProductCostElement_ID) FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND VAF_Client_ID = " + VAF_Client_ID + ") " +
                                          " AND VAF_Client_ID = " + VAF_Client_ID + " AND VAF_Org_ID = " + VAF_Org_ID;
                            }
                        }
                        else if (cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch || cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                        {
                            if (windowName == "Inventory Move")
                            {
                                sql = "SELECT ROUND(sum(currentcostprice * currentqty)/ sum(currentqty) , " + acctSchema.GetCostingPrecision() + @") FROM VAM_ProductCostQueue WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                         " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID) + " AND VAM_ProductCostElement_ID = " +
                                         " ( SELECT MIN(VAM_ProductCostElement_ID) FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND VAF_Client_ID = " + VAF_Client_ID + ") " +
                                         " AND VAF_Client_ID = " + VAF_Client_ID + " AND VAF_Org_ID = " + movementline.GetVAF_Org_ID();
                                if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                {
                                    sql += " AND VAM_Warehouse_ID = " + M_SourceWarehouse_ID;//cd.GetVAM_Warehouse_ID();
                                }
                            }
                            else
                            {
                                sql = "SELECT ROUND(sum(currentcostprice * currentqty)/ sum(currentqty) , " + acctSchema.GetCostingPrecision() + @") FROM VAM_ProductCostQueue WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                          " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID) + " AND VAM_ProductCostElement_ID = " +
                                          " ( SELECT MIN(VAM_ProductCostElement_ID) FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND VAF_Client_ID = " + VAF_Client_ID + ") " +
                                          " AND VAF_Client_ID = " + VAF_Client_ID + " AND VAF_Org_ID = " + VAF_Org_ID;
                                if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                {
                                    sql += " AND VAM_Warehouse_ID = " + cd.GetVAM_Warehouse_ID();
                                }
                            }
                        }
                        priceFifo = Util.GetValueOfDecimal(DB.ExecuteScalar(sql));
                        priceFifo = Decimal.Round(priceFifo, acctSchema.GetCostingPrecision());
                    }
                    #endregion
                    #endregion

                    #region lifo cost
                    if (cl == MProductCategory.COSTINGLEVEL_BatchLot)
                    {
                        sql = "SELECT CurrentCostPrice FROM VAM_ProductCost WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                               " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + M_ASI_ID + " AND  VAM_ProductCostElement_ID = " +
                               " ( SELECT MIN(VAM_ProductCostElement_ID) FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND VAF_Client_ID = " + VAF_Client_ID + ") " +
                               " AND VAF_Client_ID = " + VAF_Client_ID;
                    }
                    else if (cl == MProductCategory.COSTINGLEVEL_Client)
                    {
                        sql = "SELECT CurrentCostPrice FROM VAM_ProductCost WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                  " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + "  AND VAM_ProductCostElement_ID = " +
                                  " ( SELECT MIN(VAM_ProductCostElement_ID) FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND VAF_Client_ID = " + VAF_Client_ID + ") " +
                                  " AND VAF_Client_ID = " + VAF_Client_ID;
                    }
                    else if (cl == MProductCategory.COSTINGLEVEL_Organization)
                    {
                        if (windowName == "Inventory Move")
                        {
                            sql = "SELECT CurrentCostPrice FROM VAM_ProductCost WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                     " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND VAM_ProductCostElement_ID = " +
                                     " ( SELECT MIN(VAM_ProductCostElement_ID) FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND VAF_Client_ID = " + VAF_Client_ID + ") " +
                                     " AND VAF_Client_ID = " + VAF_Client_ID + " AND VAF_Org_ID = " + movementline.GetVAF_Org_ID();
                        }
                        else
                        {
                            sql = "SELECT CurrentCostPrice FROM VAM_ProductCost WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                      " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND VAM_ProductCostElement_ID = " +
                                      " ( SELECT MIN(VAM_ProductCostElement_ID) FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND VAF_Client_ID = " + VAF_Client_ID + ") " +
                                      " AND VAF_Client_ID = " + VAF_Client_ID + " AND VAF_Org_ID = " + VAF_Org_ID;
                        }
                    }
                    else if (cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch || cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                    {
                        if (windowName == "Inventory Move")
                        {
                            sql = "SELECT CurrentCostPrice FROM VAM_ProductCost WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                     " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID) + " AND VAM_ProductCostElement_ID = " +
                                     " ( SELECT MIN(VAM_ProductCostElement_ID) FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND VAF_Client_ID = " + VAF_Client_ID + ") " +
                                     " AND VAF_Client_ID = " + VAF_Client_ID + " AND VAF_Org_ID = " + movementline.GetVAF_Org_ID();
                            if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                            {
                                sql += " AND VAM_Warehouse_ID = " + M_SourceWarehouse_ID;//cd.GetVAM_Warehouse_ID();
                            }
                        }
                        else
                        {
                            sql = "SELECT CurrentCostPrice FROM VAM_ProductCost WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                      " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID) + " AND VAM_ProductCostElement_ID = " +
                                      " ( SELECT MIN(VAM_ProductCostElement_ID) FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND VAF_Client_ID = " + VAF_Client_ID + ") " +
                                      " AND VAF_Client_ID = " + VAF_Client_ID + " AND VAF_Org_ID = " + VAF_Org_ID;
                            if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                            {
                                sql += " AND VAM_Warehouse_ID = " + cd.GetVAM_Warehouse_ID();
                            }
                        }
                    }
                    priceLifo = Util.GetValueOfDecimal(DB.ExecuteScalar(sql));

                    #region on fly calculation from cost queue
                    if (priceLifo <= 0)
                    {
                        if (cl == MProductCategory.COSTINGLEVEL_BatchLot)
                        {
                            sql = "SELECT ROUND(sum(currentcostprice * currentqty)/ sum(currentqty) , " + acctSchema.GetCostingPrecision() + @") FROM VAM_ProductCostQueue WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                   " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + M_ASI_ID + " AND  VAM_ProductCostElement_ID = " +
                                   " ( SELECT MIN(VAM_ProductCostElement_ID) FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND VAF_Client_ID = " + VAF_Client_ID + ") " +
                                   " AND VAF_Client_ID = " + VAF_Client_ID;
                        }
                        else if (cl == MProductCategory.COSTINGLEVEL_Client)
                        {
                            sql = "SELECT ROUND(sum(currentcostprice * currentqty)/ sum(currentqty) , " + acctSchema.GetCostingPrecision() + @") FROM VAM_ProductCostQueue WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                      " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + "  AND VAM_ProductCostElement_ID = " +
                                      " ( SELECT MIN(VAM_ProductCostElement_ID) FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND VAF_Client_ID = " + VAF_Client_ID + ") " +
                                      " AND VAF_Client_ID = " + VAF_Client_ID;
                        }
                        else if (cl == MProductCategory.COSTINGLEVEL_Organization)
                        {
                            if (windowName == "Inventory Move")
                            {
                                sql = "SELECT ROUND(sum(currentcostprice * currentqty)/ sum(currentqty) , " + acctSchema.GetCostingPrecision() + @") FROM VAM_ProductCostQueue WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                         " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND VAM_ProductCostElement_ID = " +
                                         " ( SELECT MIN(VAM_ProductCostElement_ID) FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND VAF_Client_ID = " + VAF_Client_ID + ") " +
                                         " AND VAF_Client_ID = " + VAF_Client_ID + " AND VAF_Org_ID = " + movementline.GetVAF_Org_ID();
                            }
                            else
                            {
                                sql = "SELECT ROUND(sum(currentcostprice * currentqty)/ sum(currentqty) , " + acctSchema.GetCostingPrecision() + @") FROM VAM_ProductCostQueue WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                          " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND VAM_ProductCostElement_ID = " +
                                          " ( SELECT MIN(VAM_ProductCostElement_ID) FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND VAF_Client_ID = " + VAF_Client_ID + ") " +
                                          " AND VAF_Client_ID = " + VAF_Client_ID + " AND VAF_Org_ID = " + VAF_Org_ID;
                            }
                        }
                        else if (cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch || cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                        {
                            if (windowName == "Inventory Move")
                            {
                                sql = "SELECT ROUND(sum(currentcostprice * currentqty)/ sum(currentqty) , " + acctSchema.GetCostingPrecision() + @") FROM VAM_ProductCostQueue WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                         " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID) + " AND VAM_ProductCostElement_ID = " +
                                         " ( SELECT MIN(VAM_ProductCostElement_ID) FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND VAF_Client_ID = " + VAF_Client_ID + ") " +
                                         " AND VAF_Client_ID = " + VAF_Client_ID + " AND VAF_Org_ID = " + movementline.GetVAF_Org_ID();
                                if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                {
                                    sql += " AND VAM_Warehouse_ID = " + M_SourceWarehouse_ID;// cd.GetVAM_Warehouse_ID();
                                }
                            }
                            else
                            {
                                sql = "SELECT ROUND(sum(currentcostprice * currentqty)/ sum(currentqty) , " + acctSchema.GetCostingPrecision() + @") FROM VAM_ProductCostQueue WHERE IsActive = 'Y' AND VAB_AccountBook_ID = " + acctSchema.GetVAB_AccountBook_ID() +
                                          " AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID) + " AND VAM_ProductCostElement_ID = " +
                                          " ( SELECT MIN(VAM_ProductCostElement_ID) FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND VAF_Client_ID = " + VAF_Client_ID + ") " +
                                          " AND VAF_Client_ID = " + VAF_Client_ID + " AND VAF_Org_ID = " + VAF_Org_ID;
                                if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                {
                                    sql += " AND VAM_Warehouse_ID = " + cd.GetVAM_Warehouse_ID();
                                }
                            }
                        }
                        priceLifo = Util.GetValueOfDecimal(DB.ExecuteScalar(sql));
                        priceLifo = Decimal.Round(priceLifo, acctSchema.GetCostingPrecision());
                    }
                    #endregion

                    #endregion
                }
            }
            catch (Exception ex)
            {
                ValueNamePair pp = VLogger.RetrieveError();
                _log.Info("Error occured at CreateCostQueue method. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName() +
                           " And Exception message : " + ex.Message.ToString());
                return false;
            }
            //
            if (Price < 0)
                Price = Decimal.Negate(Price);
            Price = Decimal.Round(Price, acctSchema.GetCostingPrecision());
            MVAMProductCostElement costElement = null;
            X_VAT_Temp_CostDetail tempCostDetail = null;
            try
            {
                #region Ist Entry Either FIFO or LIFO
                MVAMProductCostQueue costQueue = new MVAMProductCostQueue(ctx, 0, trxName);
                costQueue.SetVAF_Client_ID(VAF_Client_ID);
                if (windowName == "Physical Inventory" || windowName == "Internal Use Inventory")
                {
                    VAF_Org_ID = inventoryLine.GetVAF_Org_ID();
                }
                //else if (windowName == "Inventory Move")
                //{
                //    VAF_Org_ID = movementline.GetVAF_Org_ID();
                //}
                else if (windowName == "Material Receipt" || windowName == "Shipment" || windowName == "Customer Return" || windowName == "Return To Vendor")
                {
                    VAF_Org_ID = inoutline.GetVAF_Org_ID();
                }
                else if (windowName == "Invoice(Vendor)")
                {
                    VAF_Org_ID = invoiceline.GetVAF_Org_ID();
                }
                costQueue.SetVAF_Org_ID(VAF_Org_ID);
                costQueue.SetVAB_AccountBook_ID(Util.GetValueOfInt(acctSchema.GetVAB_AccountBook_ID()));
                costQueue.SetVAM_ProductCostType_ID(acctSchema.GetVAM_ProductCostType_ID());
                costQueue.SetVAM_Product_ID(product.GetVAM_Product_ID());
                sql = @"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = " +
                    " ( SELECT MMPolicy FROM VAM_ProductCategory WHERE IsActive = 'Y' AND VAM_ProductCategory_ID = " +
                    " (SELECT VAM_ProductCategory_ID FROM VAM_Product WHERE IsActive = 'Y' AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " )) AND VAF_Client_ID = " + VAF_Client_ID;
                VAM_ProductCostElement_ID = (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null)));
                costQueue.SetVAM_ProductCostElement_ID(VAM_ProductCostElement_ID);
                costQueue.SetVAM_PFeature_SetInstance_ID(M_ASI_ID);
                costQueue.SetVAM_Warehouse_ID(cd.GetVAM_Warehouse_ID());
                costQueue.SetCurrentQty(Qty);
                // change 2-5-2016
                if (policy == "F" && priceFifo > 0)
                {
                    costQueue.SetCurrentCostPrice(priceFifo);
                }
                else if (policy == "L" && priceLifo > 0)
                {
                    costQueue.SetCurrentCostPrice(priceLifo);
                }
                else
                {
                    ce = MVAMProductCostElement.Get(ctx, VAM_ProductCostElement_ID);
                    amtWithSurcharge = Decimal.Add(Price, Decimal.Round(Decimal.Divide(Decimal.Multiply(Price, ce.GetSurchargePercentage()), 100), acctSchema.GetCostingPrecision()));
                    costQueue.SetCurrentCostPrice(amtWithSurcharge);
                }
                //end
                costQueue.SetQueueDate(System.DateTime.Now.ToLocalTime());
                if (!costQueue.Save())
                {
                    ValueNamePair pp = VLogger.RetrieveError();
                    _log.Info("Cost Queue not saved for  <===> " + product.GetVAM_Product_ID() + " Error Type is : " + pp.GetName());
                    return false;
                }
                else
                {
                    costQueueIds += costQueue.GetVAM_ProductCostQueue_ID();
                    tempCostDetail = new X_VAT_Temp_CostDetail(ctx, 0, null);
                    tempCostDetail.SetVAF_Client_ID(VAF_Client_ID);
                    tempCostDetail.SetVAF_Org_ID(VAF_Org_ID);
                    tempCostDetail.SetVAM_ProductCostDetail_ID(cd.GetVAM_ProductCostDetail_ID());
                    tempCostDetail.SetVAM_ProductCostQueue_ID(costQueue.GetVAM_ProductCostQueue_ID());
                    tempCostDetail.SetVAB_AccountBook_ID(Util.GetValueOfInt(acctSchema.GetVAB_AccountBook_ID()));
                    if (invoiceline != null && invoiceline.GetVAB_InvoiceLine_ID() > 0)
                        tempCostDetail.SetVAB_InvoiceLine_ID(invoiceline.GetVAB_InvoiceLine_ID());
                    if (inoutline != null && inoutline.GetVAB_OrderLine_ID() > 0)
                        tempCostDetail.SetVAB_OrderLine_ID(inoutline.GetVAB_OrderLine_ID());
                    if (inoutline != null && inoutline.GetVAM_Inv_InOutLine_ID() > 0)
                        tempCostDetail.SetVAM_Inv_InOutLine_ID(inoutline.GetVAM_Inv_InOutLine_ID());
                    if (inventoryLine != null && inventoryLine.GetVAM_InventoryLine_ID() > 0)
                        tempCostDetail.SetVAM_InventoryLine_ID(inventoryLine.GetVAM_InventoryLine_ID());
                    if (movementline != null && movementline.GetVAM_InvTrf_Line_ID() > 0)
                        tempCostDetail.SetVAM_InvTrf_Line_ID(movementline.GetVAM_InvTrf_Line_ID());
                    //if (inoutline != null && inoutline.GetVAB_OrderLine_ID() == 0)
                    //    tempCostDetail.SetisRecordFromForm(true);
                    tempCostDetail.SetVAM_Product_ID(product.GetVAM_Product_ID());
                    tempCostDetail.SetVAM_PFeature_SetInstance_ID(M_ASI_ID);
                    tempCostDetail.SetVAM_Warehouse_ID(cd.GetVAM_Warehouse_ID());
                    tempCostDetail.Set_Value("VAFAM_AssetDisposal_ID",cd.Get_Value("VAFAM_AssetDisposal_ID"));

                    // change 2-5-2016
                    if (policy == "F" && priceFifo > 0)
                    {
                        policy = "L";
                        tempCostDetail.SetAmt(priceFifo);
                    }
                    else if (policy == "L" && priceLifo > 0)
                    {
                        policy = "F";
                        tempCostDetail.SetAmt(priceLifo);
                    }
                    else
                    {
                        tempCostDetail.SetAmt(amtWithSurcharge);
                    }
                    //end
                    //tempCostDetail.SetVAB_Currency_ID(acctSchema.GetVAB_Currency_ID());
                    tempCostDetail.Save();
                }
                #endregion

                int queueRecordId = costQueue.GetVAM_ProductCostQueue_ID();

                #region 2nd Entry Either FIFO or LIFO opposite of 1st entry
                costElement = new MVAMProductCostElement(ctx, costQueue.GetVAM_ProductCostElement_ID(), null);
                if (costElement.GetCostingMethod() == "F")
                {
                    sql = @"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND VAF_Client_ID = " + VAF_Client_ID;
                }
                else
                {
                    sql = @"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND VAF_Client_ID = " + VAF_Client_ID;
                }
                costQueue = new MVAMProductCostQueue(ctx, 0, trxName);
                costQueue.SetVAF_Client_ID(VAF_Client_ID);
                costQueue.SetVAF_Org_ID(VAF_Org_ID);
                costQueue.SetVAB_AccountBook_ID(Util.GetValueOfInt(acctSchema.GetVAB_AccountBook_ID()));
                costQueue.SetVAM_ProductCostType_ID(acctSchema.GetVAM_ProductCostType_ID());
                costQueue.SetVAM_Product_ID(product.GetVAM_Product_ID());
                VAM_ProductCostElement_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                costQueue.SetVAM_ProductCostElement_ID(VAM_ProductCostElement_ID);
                costQueue.SetVAM_PFeature_SetInstance_ID(M_ASI_ID);
                costQueue.SetVAM_Warehouse_ID(cd.GetVAM_Warehouse_ID());
                costQueue.SetCurrentQty(Qty);
                // change 2-5-2016
                if (policy == "F" && priceFifo > 0)
                {
                    costQueue.SetCurrentCostPrice(priceFifo);
                }
                else if (policy == "L" && priceLifo > 0)
                {
                    costQueue.SetCurrentCostPrice(priceLifo);
                }
                else
                {
                    ce = MVAMProductCostElement.Get(ctx, VAM_ProductCostElement_ID);
                    amtWithSurcharge = Decimal.Add(Price, Decimal.Round(Decimal.Divide(Decimal.Multiply(Price, ce.GetSurchargePercentage()), 100), acctSchema.GetCostingPrecision()));
                    costQueue.SetCurrentCostPrice(amtWithSurcharge);
                    //costQueue.SetCurrentCostPrice(Price);
                }
                //end
                costQueue.SetQueueDate(System.DateTime.Now.ToLocalTime());
                if (!costQueue.Save())
                {
                    // delete record if created 
                    if (queueRecordId > 0)
                        DB.ExecuteQuery("DELETE FROM VAM_ProductCostQueue WHERE VAM_ProductCostQueue_ID = " + queueRecordId, null, trxName);
                    ValueNamePair pp = VLogger.RetrieveError();
                    _log.Info("Cost Queue not saved for  <===> " + product.GetVAM_Product_ID() + " Error Type is : " + pp.GetName());
                    return false;
                }
                else
                {
                    costQueueIds += " , " + costQueue.GetVAM_ProductCostQueue_ID();
                    tempCostDetail = new X_VAT_Temp_CostDetail(ctx, 0, null);
                    tempCostDetail.SetVAF_Client_ID(VAF_Client_ID);
                    tempCostDetail.SetVAF_Org_ID(VAF_Org_ID);
                    tempCostDetail.SetVAB_AccountBook_ID(Util.GetValueOfInt(acctSchema.GetVAB_AccountBook_ID()));
                    tempCostDetail.SetVAM_ProductCostDetail_ID(cd.GetVAM_ProductCostDetail_ID());
                    tempCostDetail.SetVAM_ProductCostQueue_ID(costQueue.GetVAM_ProductCostQueue_ID());
                    if (invoiceline != null && invoiceline.GetVAB_InvoiceLine_ID() > 0)
                        tempCostDetail.SetVAB_InvoiceLine_ID(invoiceline.GetVAB_InvoiceLine_ID());
                    if (inoutline != null && inoutline.GetVAB_OrderLine_ID() > 0)
                        tempCostDetail.SetVAB_OrderLine_ID(inoutline.GetVAB_OrderLine_ID());
                    if (inoutline != null && inoutline.GetVAM_Inv_InOutLine_ID() > 0)
                        tempCostDetail.SetVAM_Inv_InOutLine_ID(inoutline.GetVAM_Inv_InOutLine_ID());
                    if (inventoryLine != null && inventoryLine.GetVAM_InventoryLine_ID() > 0)
                        tempCostDetail.SetVAM_InventoryLine_ID(inventoryLine.GetVAM_InventoryLine_ID());
                    if (movementline != null && movementline.GetVAM_InvTrf_Line_ID() > 0)
                        tempCostDetail.SetVAM_InvTrf_Line_ID(movementline.GetVAM_InvTrf_Line_ID());
                    //if (inoutline != null && inoutline.GetVAB_OrderLine_ID() == 0)
                    //    tempCostDetail.SetisRecordFromForm(true);
                    tempCostDetail.SetVAM_Product_ID(product.GetVAM_Product_ID());
                    tempCostDetail.SetVAM_PFeature_SetInstance_ID(M_ASI_ID);
                    tempCostDetail.SetVAM_Warehouse_ID(cd.GetVAM_Warehouse_ID());
                    // change 2-5-2016
                    if (policy == "F" && priceFifo > 0)
                    {
                        tempCostDetail.SetAmt(priceFifo);
                    }
                    else if (policy == "L" && priceLifo > 0)
                    {
                        tempCostDetail.SetAmt(priceLifo);
                    }
                    else
                    {
                        tempCostDetail.SetAmt(amtWithSurcharge);
                        //tempCostDetail.SetAmt(Price);
                    }
                    //end
                    //tempCostDetail.SetVAB_Currency_ID(acctSchema.GetVAB_Currency_ID());
                    tempCostDetail.Save();
                }
                #endregion

            }
            catch (Exception ex)
            {
                ValueNamePair pp = VLogger.RetrieveError();
                _log.Info("Error occured Cost Queue not saved. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName() +
                           " And Exception message : " + ex.Message.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// is used to create entry for Cost Queue
        /// also we are doing entry in temp table "VAT_Temp_CostDetail" -- for re-updation of cost when we received invoice
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="acctSchema">accounting schema</param>
        /// <param name="product">product</param>
        /// <param name="M_ASI_ID">Attribute set instance</param>
        /// <param name="VAF_Client_ID">client</param>
        /// <param name="VAF_Org_ID">org</param>
        /// <param name="Price">receiving price</param>
        /// <param name="Qty">qty to be matched</param>
        /// <param name="inoutline">inoutline reference</param>
        /// <param name="trxName">trx</param>
        /// <param name="optionalStrPO">calling from process or manual</param>
        /// <param name="VAM_Warehouse_ID">warehouse</param>
        /// <returns>TRUE - if success</returns>
        private static bool CreateCostQueueForMatchPO(Ctx ctx, MVABAccountBook acctSchema, MProduct product, int M_ASI_ID, int VAF_Client_ID,
                                                      int VAF_Org_ID, Decimal Price, Decimal Qty, MVAMInvInOutLine inoutline, Trx trxName, int VAM_Warehouse_ID, string optionalStrPO = "process")
        {
            if (optionalStrPO == "window")
            {
                return true;
            }
            MVAMProductCostElement costElement = null;
            X_VAT_Temp_CostDetail tempCostDetail = null;
            int VAM_ProductCostElement_ID = 0;
            string sql = null;
            decimal amtWithSurcharge = 0;
            try
            {
                #region Ist Entry Either FIFO or LIFO
                MVAMProductCostQueue costQueue = new MVAMProductCostQueue(ctx, 0, trxName);
                costQueue.SetVAF_Client_ID(VAF_Client_ID);
                costQueue.SetVAF_Org_ID(VAF_Org_ID);
                costQueue.SetVAB_AccountBook_ID(Util.GetValueOfInt(acctSchema.GetVAB_AccountBook_ID()));
                costQueue.SetVAM_Warehouse_ID(VAM_Warehouse_ID);
                costQueue.SetVAM_ProductCostType_ID(acctSchema.GetVAM_ProductCostType_ID());
                costQueue.SetVAM_Product_ID(product.GetVAM_Product_ID());
                sql = @"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = " +
                    " ( SELECT MMPolicy FROM VAM_ProductCategory WHERE IsActive = 'Y' AND VAM_ProductCategory_ID = " +
                    " (SELECT VAM_ProductCategory_ID FROM VAM_Product WHERE IsActive = 'Y' AND VAM_Product_ID = " + product.GetVAM_Product_ID() + " )) AND VAF_Client_ID = " + VAF_Client_ID;
                VAM_ProductCostElement_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, trxName));
                costQueue.SetVAM_ProductCostElement_ID(VAM_ProductCostElement_ID);
                costQueue.SetVAM_PFeature_SetInstance_ID(M_ASI_ID);
                costQueue.SetCurrentQty(Qty);

                costElement = new MVAMProductCostElement(ctx, VAM_ProductCostElement_ID, trxName);
                amtWithSurcharge = Decimal.Add(Price, Decimal.Round(Decimal.Divide(Decimal.Multiply(Price, costElement.GetSurchargePercentage()), 100), acctSchema.GetCostingPrecision()));
                costQueue.SetCurrentCostPrice(amtWithSurcharge);

                //costQueue.SetCurrentCostPrice(Price);
                costQueue.SetQueueDate(System.DateTime.Now.ToLocalTime());
                if (!costQueue.Save())
                {
                    ValueNamePair pp = VLogger.RetrieveError();
                    _log.Severe("Cost Queue not saved by CreateCostQueueForMatchPO for this product <===> " + product.GetVAM_Product_ID() + " Error Type is : " + pp.GetName());
                    return false;
                }
                else
                {
                    tempCostDetail = new X_VAT_Temp_CostDetail(ctx, 0, null);
                    tempCostDetail.SetVAF_Client_ID(VAF_Client_ID);
                    tempCostDetail.SetVAF_Org_ID(VAF_Org_ID);
                    //tempCostDetail.SetVAM_ProductCostDetail_ID(cd.GetVAM_ProductCostDetail_ID());
                    tempCostDetail.SetVAM_Inv_InOutLine_ID(inoutline.GetVAM_Inv_InOutLine_ID());
                    tempCostDetail.SetVAM_ProductCostQueue_ID(costQueue.GetVAM_ProductCostQueue_ID());
                    tempCostDetail.SetVAB_AccountBook_ID(Util.GetValueOfInt(acctSchema.GetVAB_AccountBook_ID()));
                    tempCostDetail.SetVAM_Warehouse_ID(VAM_Warehouse_ID);
                    tempCostDetail.SetisRecordFromForm(true);
                    tempCostDetail.SetVAM_Product_ID(product.GetVAM_Product_ID());
                    tempCostDetail.SetVAM_PFeature_SetInstance_ID(M_ASI_ID);
                    //tempCostDetail.SetAmt(Price);
                    tempCostDetail.SetAmt(amtWithSurcharge);
                    tempCostDetail.Save();
                }
                #endregion

                #region 2nd Entry Either FIFO or LIFO opposite of 1st entry
                //costElement = new MVAMProductCostElement(ctx, costQueue.GetVAM_ProductCostElement_ID(), null);
                if (costElement.GetCostingMethod() == "F")
                {
                    sql = @"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND VAF_Client_ID = " + VAF_Client_ID;
                }
                else
                {
                    sql = @"SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND VAF_Client_ID = " + VAF_Client_ID;
                }
                costQueue = new MVAMProductCostQueue(ctx, 0, trxName);
                costQueue.SetVAF_Client_ID(VAF_Client_ID);
                costQueue.SetVAF_Org_ID(VAF_Org_ID);
                costQueue.SetVAB_AccountBook_ID(Util.GetValueOfInt(acctSchema.GetVAB_AccountBook_ID()));
                costQueue.SetVAM_Warehouse_ID(VAM_Warehouse_ID);
                costQueue.SetVAM_ProductCostType_ID(acctSchema.GetVAM_ProductCostType_ID());
                costQueue.SetVAM_Product_ID(product.GetVAM_Product_ID());
                VAM_ProductCostElement_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                costQueue.SetVAM_ProductCostElement_ID(VAM_ProductCostElement_ID);
                costQueue.SetVAM_PFeature_SetInstance_ID(M_ASI_ID);
                costQueue.SetCurrentQty(Qty);

                costElement = new MVAMProductCostElement(ctx, VAM_ProductCostElement_ID, trxName);
                amtWithSurcharge = Decimal.Add(Price, Decimal.Round(Decimal.Divide(Decimal.Multiply(Price, costElement.GetSurchargePercentage()), 100), acctSchema.GetCostingPrecision()));
                costQueue.SetCurrentCostPrice(amtWithSurcharge);
                //costQueue.SetCurrentCostPrice(Price);

                costQueue.SetQueueDate(System.DateTime.Now.ToLocalTime());
                if (!costQueue.Save())
                {
                    ValueNamePair pp = VLogger.RetrieveError();
                    _log.Severe("Cost Queue not saved by CreateCostQueueForMatchPO for product  <===> " + product.GetVAM_Product_ID() + " Error Type is : " + pp.GetName());
                    return false;
                }
                else
                {
                    tempCostDetail = new X_VAT_Temp_CostDetail(ctx, 0, null);
                    tempCostDetail.SetVAF_Client_ID(VAF_Client_ID);
                    tempCostDetail.SetVAF_Org_ID(VAF_Org_ID);
                    tempCostDetail.SetVAB_AccountBook_ID(Util.GetValueOfInt(acctSchema.GetVAB_AccountBook_ID()));
                    //tempCostDetail.SetVAM_ProductCostDetail_ID(cd.GetVAM_ProductCostDetail_ID());
                    tempCostDetail.SetVAM_Inv_InOutLine_ID(inoutline.GetVAM_Inv_InOutLine_ID());
                    tempCostDetail.SetVAM_ProductCostQueue_ID(costQueue.GetVAM_ProductCostQueue_ID());
                    tempCostDetail.SetVAM_Warehouse_ID(VAM_Warehouse_ID);
                    tempCostDetail.SetisRecordFromForm(true);
                    tempCostDetail.SetVAM_Product_ID(product.GetVAM_Product_ID());
                    tempCostDetail.SetVAM_PFeature_SetInstance_ID(M_ASI_ID);

                    //tempCostDetail.SetAmt(Price);
                    tempCostDetail.SetAmt(amtWithSurcharge);
                    tempCostDetail.Save();
                }
                #endregion

            }
            catch (Exception ex)
            {
                ValueNamePair pp = VLogger.RetrieveError();
                _log.Severe("Error occured Cost Queue not saved by CreateCostQueueForMatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName() +
                           " And Exception message : " + ex.Message.ToString());
                return false;
            }
            return true;
        }

        private static void updateCostQueue(MProduct product, int M_ASI_ID, MVABAccountBook mas,
         int Org_ID, MVAMProductCostElement ce, decimal movementQty, int VAM_Warehouse_ID = 0)
        {
            Decimal qty = movementQty;
            #region Org Specific

            // get Costing level
            String costingLevel = String.Empty;
            costingLevel = MProductCategory.Get(product.GetCtx(), product.GetVAM_ProductCategory_ID()).GetCostingLevel();
            if (String.IsNullOrEmpty(costingLevel))
            {
                costingLevel = mas.GetCostingLevel();
            }
            if (!(costingLevel == MProductCategory.COSTINGLEVEL_Warehouse || costingLevel == MProductCategory.COSTINGLEVEL_WarehousePlusBatch))
            {
                VAM_Warehouse_ID = 0;
            }
            MVAMProductCostQueue[] cQueue = MVAMProductCostQueue.GetQueue(product, M_ASI_ID, mas, Org_ID, ce, product.Get_Trx(), VAM_Warehouse_ID);
            if (cQueue != null && cQueue.Length > 0)
            {
                bool value = false;
                for (int cq = 0; cq < cQueue.Length; cq++)
                {
                    MVAMProductCostQueue queue = cQueue[cq];
                    if (queue.GetCurrentQty() < 0) continue;
                    if (queue.GetCurrentQty() > qty)
                    {
                        value = true;
                    }
                    else
                    {
                        value = false;
                    }
                    qty = MVAMProductCostQueue.Quantity(queue.GetCurrentQty(), qty);
                    if (qty <= 0)
                    {
                        queue.Delete(false);
                        qty = Decimal.Negate(qty);
                    }
                    else
                    {
                        queue.SetCurrentQty(qty);
                        qty = 0;
                        if (!queue.Save())
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            _log.Severe("Cost Queue not updated by updateCostQueue for product  <===> " + product.GetVAM_Product_ID() + " Error Type is : " + pp.GetName());
                        }
                    }
                    if (value)
                    {
                        break;
                    }
                }
            }
            #endregion
        }

        public static Decimal Quantity(Decimal cQueueQty, Decimal qty)
        {
            Decimal quantity = 0;
            quantity = Decimal.Subtract(cQueueQty, qty);
            return quantity;
        }

        //end
    }
}
