/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_M_CostQueue
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
using ModelLibrary.Classes;
using System.Threading;

namespace VAdvantage.Model
{
    public class MCostQueue : X_M_CostQueue
    {
        /**	Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MCostQueue).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="ignored">multi-key</param>
        /// <param name="trxName">transaction</param>
        public MCostQueue(Ctx ctx, int ignored, Trx trxName)
            : base(ctx, ignored, trxName)
        {
            if (ignored == 0)
            {
                //	setC_AcctSchema_ID (0);
                //	setM_AttributeSetInstance_ID (0);
                //	setM_CostElement_ID (0);
                //	setM_CostType_ID (0);
                //	setM_Product_ID (0);
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
        public MCostQueue(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="product">product</param>
        /// <param name="M_AttributeSetInstance_ID">Attribute Set Instance</param>
        /// <param name="mas">Acct Schema</param>
        /// <param name="AD_Org_ID">org</param>
        /// <param name="M_CostElement_ID">cost element</param>
        /// <param name="trxName">transaction</param>
        public MCostQueue(MProduct product, int M_AttributeSetInstance_ID,
            MAcctSchema mas, int AD_Org_ID, int M_CostElement_ID, Trx trxName)
            : this(product.GetCtx(), 0, trxName)
        {
            SetClientOrg(product.GetAD_Client_ID(), AD_Org_ID);
            SetC_AcctSchema_ID(mas.GetC_AcctSchema_ID());
            SetM_CostType_ID(mas.GetM_CostType_ID());
            SetM_Product_ID(product.GetM_Product_ID());
            SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            SetM_CostElement_ID(M_CostElement_ID);
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
        public static Decimal? AdjustQty(MProduct product, int M_ASI_ID, MAcctSchema mas,
                int Org_ID, MCostElement ce, Decimal Qty, Trx trxName)
        {
            if (Env.Signum(Qty) == 0)
                return Env.ZERO;
            MCostQueue[] costQ = GetQueue(product, M_ASI_ID, mas, Org_ID, ce, trxName);
            Decimal remainingQty = Qty;
            for (int i = 0; i < costQ.Length; i++)
            {
                MCostQueue queue = costQ[i];
                //	Negative Qty i.e. add
                if (Env.Signum(remainingQty) < 0)
                {
                    Decimal oldQty = queue.GetCurrentQty();
                    Decimal newQty = Decimal.Subtract(oldQty, remainingQty);
                    queue.SetCurrentQty(newQty);
                    if (queue.Save())
                    {
                        _log.Fine("Qty=" + remainingQty
                            + "(!), ASI=" + queue.GetM_AttributeSetInstance_ID()
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
                            + ", ASI=" + queue.GetM_AttributeSetInstance_ID()
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
        /// <param name="M_AttributeSetInstance_ID">real asi</param>
        /// <param name="mas">accounting schema</param>
        /// <param name="AD_Org_ID">real org</param>
        /// <param name="M_CostElement_ID">element</param>
        /// <param name="trxName">transaction</param>
        /// <returns>cost queue or null</returns>
        public static MCostQueue Get(MProduct product, int M_AttributeSetInstance_ID,
            MAcctSchema mas, int AD_Org_ID, int M_CostElement_ID, Trx trxName)
        {
            MCostQueue costQ = null;
            String sql = "SELECT * FROM M_CostQueue "
                + "WHERE AD_Client_ID=@client AND AD_Org_ID=@org"
                + " AND M_Product_ID=@pro"
                + " AND M_AttributeSetInstance_ID=@asi"
                + " AND M_CostType_ID=@ct AND C_AcctSchema_ID=@accs"
                + " AND M_CostElement_ID=@ce";
            try
            {
                SqlParameter[] param = new SqlParameter[7];
                param[0] = new SqlParameter("@client", product.GetAD_Client_ID());
                param[1] = new SqlParameter("@org", AD_Org_ID);
                param[2] = new SqlParameter("@pro", product.GetM_Product_ID());
                param[3] = new SqlParameter("@asi", M_AttributeSetInstance_ID);
                param[4] = new SqlParameter("@ct", mas.GetM_CostType_ID());
                param[5] = new SqlParameter("@accs", mas.GetC_AcctSchema_ID());
                param[6] = new SqlParameter("@ce", M_CostElement_ID);

                DataSet ds = DataBase.DB.ExecuteDataset(sql, param);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        costQ = new MCostQueue(product.GetCtx(), dr, trxName);
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            //	New
            if (costQ == null)
                costQ = new MCostQueue(product, M_AttributeSetInstance_ID,
                    mas, AD_Org_ID, M_CostElement_ID, trxName);
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
        public static Decimal? GetCosts(MProduct product, int M_ASI_ID, MAcctSchema mas,
            int Org_ID, MCostElement ce, Decimal Qty, Trx trxName)
        {
            if (Env.Signum(Qty) == 0)
                return Env.ZERO;
            MCostQueue[] costQ = GetQueue(product, M_ASI_ID,
                mas, Org_ID, ce, trxName);
            //
            Decimal cost = Env.ZERO;
            Decimal remainingQty = Qty;
            Decimal? firstPrice = null;
            Decimal? lastPrice = null;
            //
            for (int i = 0; i < costQ.Length; i++)
            {
                MCostQueue queue = costQ[i];
                //	Negative Qty i.e. add
                if (Env.Signum(remainingQty) <= 0)
                {
                    Decimal oldQty = queue.GetCurrentQty();
                    lastPrice = queue.GetCurrentCostPrice();
                    Decimal costBatch = Decimal.Multiply((Decimal)lastPrice, remainingQty);
                    cost = Decimal.Add(cost, costBatch);
                    _log.Config("ASI=" + queue.GetM_AttributeSetInstance_ID()
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
                    _log.Fine("ASI=" + queue.GetM_AttributeSetInstance_ID()
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
                lastPrice = MCost.GetSeedCosts(product, M_ASI_ID, mas, Org_ID,
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
        public static MCostQueue[] GetQueue(MProduct product, int M_ASI_ID, MAcctSchema mas,
            int Org_ID, MCostElement ce, Trx trxName, int M_Warehouse_ID = 0)
        {
            string costingLevel = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT 
                        CASE WHEN M_Product_Category.CostingLevel IS NOT NULL THEN M_Product_Category.CostingLevel
                             WHEN C_AcctSchema.CostingLevel IS NOT NULL THEN C_AcctSchema.CostingLevel END AS CostingLevel
                        FROM M_Product
                              INNER JOIN M_Product_Category ON M_Product_Category.M_Product_Category_ID = M_Product.M_Product_Category_ID
                              INNER JOIN C_AcctSchema ON C_AcctSchema.C_AcctSchema_ID = " + mas.GetC_AcctSchema_ID() + @"
                        WHERE M_Product.M_Product_ID = " + product.GetM_Product_ID()));
            bool attributeApplicable = (costingLevel == X_C_AcctSchema.COSTINGLEVEL_BatchLot
                                        || costingLevel == X_C_AcctSchema.COSTINGLEVEL_OrgPlusBatch
                                        || costingLevel == X_C_AcctSchema.COSTINGLEVEL_WarehousePlusBatch);

            List<MCostQueue> list = new List<MCostQueue>();
            String sql = "SELECT * FROM M_CostQueue "
                + "WHERE AD_Client_ID=@client "
                + " AND M_Product_ID=@prod"
                + " AND M_CostType_ID=@ct AND C_AcctSchema_ID=@accs"
                + " AND M_CostElement_ID=@ce";
            if (Org_ID != 0)
                sql += " AND AD_Org_ID=@org";
            if (M_Warehouse_ID != 0)
                sql += " AND NVL(M_Warehouse_ID, 0) = " + M_Warehouse_ID;
            if (attributeApplicable)//M_ASI_ID != 0 && 
            {
                sql += " AND NVL(M_AttributeSetInstance_ID, 0)=@asi";
            }
            sql += " AND CurrentQty<>0 ";
            sql += "ORDER BY queuedate ";
            if (!ce.IsFifo())
                sql += "DESC ";
            sql += " , M_AttributeSetInstance_ID ";
            if (!ce.IsFifo())
                sql += "DESC";
            try
            {
                SqlParameter[] param = null;
                if (attributeApplicable && Org_ID != 0)
                {
                    param = new SqlParameter[7];
                }
                else if (M_ASI_ID == 0 && Org_ID == 0 && attributeApplicable)
                {
                    param = new SqlParameter[6];
                }
                else if (M_ASI_ID == 0 && Org_ID == 0 && !attributeApplicable)
                {
                    param = new SqlParameter[5];
                }
                else
                {
                    param = new SqlParameter[6];
                }
                param[0] = new SqlParameter("@client", product.GetAD_Client_ID());
                param[1] = new SqlParameter("@prod", product.GetM_Product_ID());
                param[2] = new SqlParameter("@ct", mas.GetM_CostType_ID());
                param[3] = new SqlParameter("@accs", mas.GetC_AcctSchema_ID());
                param[4] = new SqlParameter("@ce", ce.GetM_CostElement_ID());
                if (attributeApplicable && Org_ID != 0)
                {
                    param[5] = new SqlParameter("@org", Org_ID);
                    param[6] = new SqlParameter("@asi", M_ASI_ID);
                }
                else if (Org_ID != 0)
                {
                    param[5] = new SqlParameter("@org", Org_ID);
                }
                else if (attributeApplicable)
                {
                    param[5] = new SqlParameter("@asi", M_ASI_ID);
                }
                DataSet ds = DataBase.DB.ExecuteDataset(sql, param, trxName);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MCostQueue(product.GetCtx(), dr, trxName));
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            MCostQueue[] costQ = new MCostQueue[list.Count];
            costQ = list.ToArray();
            return costQ;
        }

        /// <summary>
        /// This function is used to get the Current Cost from Cost Queue
        /// </summary>
        /// <param name="AD_Client_ID">client id</param>
        /// <param name="Org_ID">organization id</param>
        /// <param name="M_Product">product id</param>
        /// <param name="M_ASI_ID">Attribute Set Instance ID</param>
        /// <param name="mas">accounting Schema Reference</param>
        /// <param name="M_Warehouse_ID">warehouse id</param>
        /// <param name="isFifo">isFifo</param>
        /// <param name="trxName">transaction</param>
        /// <returns>Current Cost</returns>
        public static Decimal GetLifoAndFifoCurrentCostFromCostQueue(int AD_Client_ID, int Org_ID, int M_Product, int M_ASI_ID, MAcctSchema mas,
             int M_Warehouse_ID, bool isFifo, Trx trxName)
        {
            // check costing is raleted to batch/lot or not
            string costingLevel = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT 
                        CASE WHEN M_Product_Category.CostingLevel IS NOT NULL THEN M_Product_Category.CostingLevel
                             WHEN C_AcctSchema.CostingLevel IS NOT NULL THEN C_AcctSchema.CostingLevel END AS CostingLevel
                        FROM M_Product
                              INNER JOIN M_Product_Category ON M_Product_Category.M_Product_Category_ID = M_Product.M_Product_Category_ID
                              INNER JOIN C_AcctSchema ON C_AcctSchema.C_AcctSchema_ID = " + mas.GetC_AcctSchema_ID() + @"
                        WHERE M_Product.M_Product_ID = " + M_Product));
            bool attributeApplicable = (costingLevel == X_C_AcctSchema.COSTINGLEVEL_BatchLot
                                        || costingLevel == X_C_AcctSchema.COSTINGLEVEL_OrgPlusBatch
                                        || costingLevel == X_C_AcctSchema.COSTINGLEVEL_WarehousePlusBatch);

            Decimal currentCost = 0;
            StringBuilder sql = new StringBuilder("SELECT CurrentCostPrice FROM M_CostQueue "
                + "WHERE AD_Client_ID=@client "
                + " AND M_Product_ID=@prod"
                + " AND M_CostType_ID=@ct AND C_AcctSchema_ID=@accs"
                + " AND M_CostElement_ID=( SELECT MIN(M_CostElement_ID) FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = '" + (isFifo ? "F" : " L") + "' AND AD_Client_ID = " + AD_Client_ID + ")");
            if (Org_ID != 0)
                sql.Append(" AND AD_Org_ID=@org");
            if (M_Warehouse_ID != 0)
                sql.Append(" AND NVL(M_Warehouse_ID, 0) = " + M_Warehouse_ID);
            if (attributeApplicable)
                sql.Append(" AND NVL(M_AttributeSetInstance_ID, 0)=@asi");
            sql.Append(" AND CurrentQty<>0 ");
            sql.Append("ORDER BY queuedate ");
            if (!isFifo)
                sql.Append("DESC ");
            sql.Append(" , M_AttributeSetInstance_ID ");
            if (!isFifo)
                sql.Append("DESC");
            try
            {
                SqlParameter[] param = null;
                if (attributeApplicable && Org_ID != 0)
                {
                    param = new SqlParameter[6];
                }
                else if (Org_ID == 0 && attributeApplicable)
                {
                    param = new SqlParameter[5];
                }
                else if (M_ASI_ID == 0 && Org_ID == 0 && !attributeApplicable)
                {
                    param = new SqlParameter[4];
                }
                else
                {
                    param = new SqlParameter[5];
                }
                param[0] = new SqlParameter("@client", AD_Client_ID);
                param[1] = new SqlParameter("@prod", M_Product);
                param[2] = new SqlParameter("@ct", mas.GetM_CostType_ID());
                param[3] = new SqlParameter("@accs", mas.GetC_AcctSchema_ID());
                if (attributeApplicable && Org_ID != 0)
                {
                    param[4] = new SqlParameter("@org", Org_ID);
                    param[5] = new SqlParameter("@asi", M_ASI_ID);
                }
                else if (Org_ID != 0)
                {
                    param[4] = new SqlParameter("@org", Org_ID);
                }
                else if (attributeApplicable)
                {
                    param[4] = new SqlParameter("@asi", M_ASI_ID);
                }
                currentCost = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), param, trxName));
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql.ToString(), e);
            }
            return currentCost;
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
        public static bool CreateProductCostsDetails(Ctx ctx, int AD_Client_ID, int AD_Org_ID, MProduct product, int M_ASI_ID,
                     string windowName, MInventoryLine inventoryLine, MInOutLine inoutline, MMovementLine movementline,
                     MInvoiceLine invoiceline, PO po, Decimal Price, Decimal Qty, Trx trxName, out string conversionNotFound,
                     string optionalstr = "process")
        {
            conversionNotFound = "";
            return CreateProductCostsDetails(ctx, AD_Client_ID, AD_Org_ID, product, M_ASI_ID, windowName, inventoryLine, inoutline, movementline,
                 invoiceline, po, Price, Qty, trxName, null, out conversionNotFound, optionalstr);
        }

        /// <summary>
        /// Calculate Product Cost
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Client_ID">Client</param>
        /// <param name="AD_Org_ID">Organization</param>
        /// <param name="product">Product</param>
        /// <param name="M_ASI_ID">ASI</param>
        /// <param name="windowName">window Name</param>
        /// <param name="inventoryLine">Inventory Line</param>
        /// <param name="inoutline">InOut Line</param>
        /// <param name="movementline">Movement Line</param>
        /// <param name="invoiceline">Invoice Line</param>
        /// <param name="po">PO Object</param>
        /// <param name="Price">Price</param>
        /// <param name="Qty">Qty</param>
        /// <param name="trxName">Transaction</param>
        /// <param name="costingCheck">Costing Check Class object</param>
        /// <param name="conversionNotFound">out Parameter</param>
        /// <param name="optionalstr">calling from window / Process</param>
        /// <writer>VIS_0046</writer>
        /// <returns>true, when success</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static bool CreateProductCostsDetails(Ctx ctx, int AD_Client_ID, int AD_Org_ID, MProduct product, int M_ASI_ID,
                     string windowName, MInventoryLine inventoryLine, MInOutLine inoutline, MMovementLine movementline,
                     MInvoiceLine invoiceline, PO po, Decimal Price, Decimal Qty, Trx trxName, CostingCheck costingCheck, out string conversionNotFound,
                     string optionalstr = "process")
        {
            Thread.Sleep(50);
            MAcctSchema acctSchema = null;
            dynamic pca = null;
            string costingMethodMatchPO = null;
            MCostElement costElement = null;
            MProduct productLca = null;
            MOrderLine orderline = null;
            MOrder order = null;
            MInvoice invoice = null;
            MMovement movement = null;
            MInventory inventory = null;
            MInOutLine matchInoutLine = inoutline;
            MCostDetail cd = null;
            MCostDetail cdSourceWarehouse = null;
            MCost cost = null;
            MInOut inout = null;
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
            int AD_Org_ID2 = AD_Org_ID;
            DataSet ds = null;
            string cl = "";
            string isMatchFromForm = "N";
            string handlingWindowName = windowName;
            bool result = true;
            int MatchPO_OrderLineId = 0;
            StringBuilder query = new StringBuilder();
            conversionNotFound = "";
            MClient client = MClient.Get(ctx, AD_Client_ID);
            string costQueuseIds = null;
            int M_Warehouse_Id = 0; // is used to manage costing at warehouse level
            int SourceM_Warehouse_Id = 0; // is used to manage costing at warehouse level during Inventory Move
            MLocator loc = null; // is used to get warehouse id to manager costing levelev - Warehouse + Batch 
            String costLevel = null; // is used to check costing level binded for calculation of costing
            bool backwardCompatabilitySupport = false;
            bool IsPOCostingMethod = product != null ? MCostElement.IsPOCostingmethod(ctx, AD_Client_ID, product.GetM_Product_ID(), trxName) : false;
            try
            {
                if (product != null)
                {
                    if (costingCheck != null && costingCheck.product == null)
                    {
                        costingCheck.product = product;
                    }
                    else if (costingCheck == null)
                    {
                        costingCheck = new CostingCheck(ctx);
                        costingCheck.product = product;
                    }

                    _log.Info("costing Calculation Start for window = " + windowName + " and product = " + product.GetM_Product_ID() + " AND ASI " + M_ASI_ID);
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
                        inout = new MInOut(ctx, inoutline.GetM_InOut_ID(), trxName);
                    }
                }

                query.Clear();
                //query.Append("SELECT C_AcctSchema_ID FROM C_AcctSchema WHERE IsActive = 'Y' AND AD_Client_ID = " + AD_Client_ID);
                // first calculate cost for primary accounting schema then calculate for other 
                query.Append(@"Select C_Acctschema_Id From C_Acctschema
                                WHERE Isactive = 'Y' AND C_Acctschema_Id = (SELECT C_Acctschema1_Id FROM Ad_Clientinfo WHERE Ad_Client_Id = " + AD_Client_ID + @" )
                                Union
                                Select C_Acctschema_Id From C_Acctschema Where Isactive = 'Y' And Ad_Client_Id = " + AD_Client_ID + @"
                                AND C_Acctschema_Id != (SELECT C_Acctschema1_Id FROM Ad_Clientinfo WHERE Ad_Client_Id = " + AD_Client_ID + " )");
                if (costingCheck != null && costingCheck.dsAccountingSchema != null)
                {
                    ds = costingCheck.dsAccountingSchema;
                }
                else
                {
                    ds = DB.ExecuteDataset(query.ToString(), null, null);
                }
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        cd = null;
                        Price = receivedPrice;
                        Qty = receivedQty;
                        AD_Org_ID = AD_Org_ID2;
                        acctSchema = MAcctSchema.Get(ctx, Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_AcctSchema_ID"]), trxName);
                        if (product != null)
                        {
                            #region Costing Level
                            pca = new MProductCategory(ctx, product.GetM_Product_Category_ID(), null);
                            if (product.GetProductType() == "E")
                            {
                                // for expense type product, we didnt check costing level
                                // bcz if we define costing level as client and try to calculate landed cost whose costing level is Organization
                                // then system calculate its costing in (*) org but this product costing level is Org. 
                            }
                            else
                            {
                                // get costing level
                                costLevel = (MProductCategory.Get(product.GetCtx(), product.GetM_Product_Category_ID())).GetCostingLevel();
                                if (String.IsNullOrEmpty(costLevel))
                                {
                                    costLevel = acctSchema.GetCostingLevel();
                                }

                                // maintain costing level, Costing method and Costing element 
                                if (costingCheck != null)
                                {
                                    costingCheck.costinglevel = costLevel;
                                    costingCheck.costingMethod = pca.GetCostingMethod();
                                    if (String.IsNullOrEmpty(costingCheck.costingMethod))
                                    {
                                        costingCheck.costingMethod = acctSchema.GetCostingMethod();
                                    }
                                    if (!String.IsNullOrEmpty(costingCheck.costingMethod) && costingCheck.costingMethod.Equals("C"))
                                    {
                                        costingCheck.costingElement = pca.GetM_CostElement_ID();
                                        if (costingCheck.costingElement <= 0)
                                        {
                                            costingCheck.costingElement = acctSchema.GetM_CostElement_ID();
                                        }
                                    }
                                }

                                if (windowName != "Inventory Move")
                                {
                                    if (pca != null)
                                    {
                                        if (pca.GetCostingLevel() == "C" || pca.GetCostingLevel() == "B")
                                        {
                                            AD_Org_ID = 0;
                                        }
                                        else if ((string.IsNullOrEmpty(pca.GetCostingLevel())) && (acctSchema.GetCostingLevel() == "C" || acctSchema.GetCostingLevel() == "B"))
                                        {
                                            AD_Org_ID = 0;
                                        }
                                    }
                                    else if (acctSchema.GetCostingLevel() == "C" || acctSchema.GetCostingLevel() == "B")
                                    {
                                        AD_Org_ID = 0;
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
                                loc = MLocator.Get(ctx, (inoutline != null ? inoutline.GetM_Locator_ID() : inventoryLine.GetM_Locator_ID()));
                            }
                            else
                            {
                                if (loc == null)
                                {
                                    loc = MLocator.Get(ctx, Util.GetValueOfInt(po.Get_Value("M_Locator_ID")));
                                }
                            }
                            decimal productCostsQty = MCostQueue.CheckQtyAvailablity(ctx, acctSchema, AD_Client_ID, AD_Org_ID, product, M_ASI_ID, loc.GetM_Warehouse_ID());
                            if (productCostsQty < Decimal.Negate(Qty))
                            {
                                return false;
                            }
                        }
                        else if (windowName == "Inventory Move" && AD_Org_ID > 0)
                        {
                            // when costing level is other than Warehouse wise and "Organization" of "Source And To Warehouse" is same then not require to calculate cost
                            // when costing level is Warehouse wise and "From AND To" warehouse is same then not require to calculate cost
                            if (costLevel == MProductCategory.COSTINGLEVEL_Warehouse || costLevel == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                            {
                                if (MLocator.Get(ctx, (movementline != null ? movementline.GetM_Locator_ID() : 0)).GetM_Warehouse_ID() ==
                                    MLocator.Get(ctx, (movementline != null ? movementline.GetM_LocatorTo_ID() : 0)).GetM_Warehouse_ID())
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (MLocator.Get(ctx, (movementline != null ? movementline.GetM_Locator_ID() : 0)).GetAD_Org_ID() ==
                                       MLocator.Get(ctx, (movementline != null ? movementline.GetM_LocatorTo_ID() : 0)).GetAD_Org_ID())
                                {
                                    return true;
                                }
                            }

                            // when qty is positive -- we are reducing stock from "Source Warehouse"
                            if (Qty > 0)
                            {
                                loc = MLocator.Get(ctx, (movementline != null ? movementline.GetM_Locator_ID() : 0));
                            }
                            // when qty is negative -- we are reducing stock from "To Warehouse"
                            else
                            {
                                loc = MLocator.Get(ctx, (movementline != null ? movementline.GetM_LocatorTo_ID() : 0));
                            }
                            decimal productCostsQty = MCostQueue.CheckQtyAvailablity(ctx, acctSchema, AD_Client_ID, (Qty > 0 ? AD_Org_ID : loc.GetAD_Org_ID()), product, M_ASI_ID, loc.GetM_Warehouse_ID());
                            if (productCostsQty < Math.Abs(Qty))
                            {
                                return false;
                            }
                        }

                        if (windowName == "Material Receipt" || windowName == "Customer Return" || windowName == "Shipment" || windowName == "Return To Vendor")
                        {
                            #region Get Price with conversion if required
                            if (costingCheck != null)
                            {
                                inout = costingCheck.inout;
                            }
                            if (inout == null || inout.Get_ID() <= 0 || inout.Get_ID() != inoutline.GetM_InOut_ID())
                            {
                                inout = inoutline.GetParent(); //new MInOut(ctx, inoutline.GetM_InOut_ID(), trxName);
                                if (costingCheck != null && costingCheck.inout == null)
                                {
                                    costingCheck.inout = inout;
                                }
                            }
                            M_Warehouse_Id = inout.GetM_Warehouse_ID();
                            if (inoutline.GetC_OrderLine_ID() > 0)
                            {
                                if (costingCheck != null)
                                {
                                    orderline = costingCheck.orderline;
                                }
                                if (orderline == null || orderline.Get_ID() <= 0 || orderline.Get_ID() != inoutline.GetC_OrderLine_ID())
                                {
                                    orderline = new MOrderLine(ctx, inoutline.GetC_OrderLine_ID(), trxName);
                                    if (costingCheck != null && costingCheck.orderline == null)
                                    {
                                        costingCheck.orderline = orderline;
                                    }
                                }

                                if (costingCheck != null)
                                {
                                    order = costingCheck.order;
                                }
                                if (order == null || order.Get_ID() <= 0 || order.Get_ID() != orderline.GetC_Order_ID())
                                {
                                    order = new MOrder(ctx, orderline.GetC_Order_ID(), trxName);
                                    if (costingCheck != null && costingCheck.order == null)
                                    {
                                        costingCheck.order = order;
                                    }
                                }

                                if (order.GetC_Currency_ID() != acctSchema.GetC_Currency_ID())
                                {
                                    // convert amount on account date of M_Inout (discussed with Ashish, Suya, Mukesh sir)
                                    Price = MConversionRate.Convert(ctx, Price, order.GetC_Currency_ID(), acctSchema.GetC_Currency_ID(),
                                                                     inout.GetDateAcct(), order.GetC_ConversionType_ID(), AD_Client_ID, AD_Org_ID);
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
                                price = MCostQueue.CalculateCost(ctx, acctSchema, product, M_ASI_ID, AD_Client_ID, AD_Org_ID, M_Warehouse_Id);
                                Price = price * inoutline.GetMovementQty();
                                cmPrice = Price;
                                if (Price == 0)
                                {
                                    Price = GetPrice(inout, inoutline, acctSchema.GetC_Currency_ID());
                                }
                                if (Price == 0)
                                {
                                    // when we not find price on product cost and price list then we check invoice price
                                    // check any invoice is created with this MR if yes then consider first invoice price for this MR costing
                                    int invoiceLineId = 0;
                                    if (invoiceline == null)
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT MIN(il.C_InvoiceLine_ID) FROM C_InvoiceLine il INNER JOIN c_invoice i 
                                                   ON i.c_invoice_id  = il.c_invoice_id
                                                   WHERE i.DocStatus IN ('CO' , 'CL') AND il.IsActive = 'Y'
                                                   AND il.M_InoutLine_ID = " + inoutline.GetM_InOutLine_ID());
                                        invoiceLineId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));
                                    }
                                    else
                                    {
                                        invoiceLineId = invoiceline.GetC_InvoiceLine_ID();
                                    }
                                    if (invoiceLineId > 0)
                                    {
                                        //invoiceline = new MInvoiceLine(ctx, invoiceLineId, trxName);
                                        //invoice = new MInvoice(ctx, invoiceline.GetC_Invoice_ID(), trxName);
                                        DataSet dsInv = DB.ExecuteDataset(@"SELECT C_Currency_ID , C_ConversionType_ID FROM C_Invoice 
                                                         WHERE C_Invoice_ID = (SELECT C_Invoice_ID FROM C_InvoiceLine 
                                                         WHERE C_InvoiceLine_ID = " + invoiceLineId + ")", null, trxName);

                                        Price = Decimal.Divide(invoiceline.GetProductLineCost(invoiceline), invoiceline.GetQtyInvoiced());
                                        Price = Price * inoutline.GetMovementQty();
                                        if (Util.GetValueOfInt(dsInv.Tables[0].Rows[0]["C_Currency_ID"]) != acctSchema.GetC_Currency_ID())
                                        {
                                            // convert amount on account date of M_Inout (discussed with Ashish, Suya, Mukesh sir)
                                            Price = MConversionRate.Convert(ctx, Price, Util.GetValueOfInt(dsInv.Tables[0].Rows[0]["C_Currency_ID"]), acctSchema.GetC_Currency_ID(),
                                                                inout.GetDateAcct(), Util.GetValueOfInt(dsInv.Tables[0].Rows[0]["C_ConversionType_ID"]), AD_Client_ID, AD_Org_ID);
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
                            if (costingCheck != null)
                            {
                                invoice = costingCheck.invoice;
                            }
                            if (invoice == null || invoice.Get_ID() <= 0 || invoice.Get_ID() != invoiceline.GetC_Invoice_ID())
                            {
                                invoice = new MInvoice(ctx, invoiceline.GetC_Invoice_ID(), trxName);
                                costingCheck.invoice = invoice;
                            }

                            if (invoiceline.GetM_InOutLine_ID() > 0)
                            {
                                M_Warehouse_Id = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT m_warehouse_id FROM m_inout WHERE m_inout_id = 
                                    ( SELECT m_inout_id FROM m_inoutline WHERE m_inoutline_id = " + invoiceline.GetM_InOutLine_ID() + " )", null, trxName));
                            }
                            else if (invoiceline.Get_ColumnIndex("M_Warehouse_ID") >= 0 && invoice.Get_ColumnIndex("M_Warehouse_ID") >= 0
                                && invoiceline.GetM_InOutLine_ID() <= 0 && invoiceline.GetC_OrderLine_ID() <= 0
                                && !invoice.IsSOTrx() && invoice.IsReturnTrx() && invoice.IsTreatAsDiscount())
                            {
                                M_Warehouse_Id = invoiceline.GetM_Warehouse_ID();
                                if (M_Warehouse_Id <= 0)
                                {
                                    M_Warehouse_Id = invoice.GetM_Warehouse_ID();
                                }
                            }
                            if (invoice.GetC_Currency_ID() != acctSchema.GetC_Currency_ID())
                            {
                                Price = MConversionRate.Convert(ctx, Price, invoice.GetC_Currency_ID(), acctSchema.GetC_Currency_ID(),
                                                                         invoice.GetDateAcct(), invoice.GetC_ConversionType_ID(), AD_Client_ID, AD_Org_ID);
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
                                conversionNotFound = invoice.GetDocumentNo();
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
                                query.Append(@"SELECT IsMatchPOForm FROM M_MatchPO WHERE IsActive = 'Y'   AND M_Product_ID = " + product.GetM_Product_ID());
                                if (matchInoutLine != null && matchInoutLine.GetM_InOutLine_ID() > 0)
                                {
                                    query.Append(" AND M_InoutLine_ID = " + Util.GetValueOfInt(matchInoutLine.GetM_InOutLine_ID()));
                                }
                                else
                                {
                                    query.Append(" AND M_InoutLine_ID = " + Util.GetValueOfInt(invoiceline.GetM_InOutLine_ID()));
                                }
                                isMatchFromForm = Util.GetValueOfString(DB.ExecuteScalar(query.ToString(), null, trxName));
                                if (isMatchFromForm == "Y")
                                {
                                    if (matchInoutLine != null && matchInoutLine.GetM_InOutLine_ID() > 0)
                                    {
                                        if (costingCheck != null)
                                        {
                                            inoutline = costingCheck.inoutline;
                                        }
                                        if (inoutline == null || inoutline.Get_ID() <= 0 || inoutline.Get_ID() != matchInoutLine.GetM_InOutLine_ID())
                                        {
                                            inoutline = new MInOutLine(ctx, matchInoutLine.GetM_InOutLine_ID(), trxName);
                                            if (costingCheck != null && costingCheck.inoutline == null)
                                            {
                                                costingCheck.inoutline = inoutline;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (costingCheck != null)
                                        {
                                            inoutline = costingCheck.inoutline;
                                        }
                                        if (inoutline == null || inoutline.Get_ID() <= 0 || inoutline.Get_ID() != invoiceline.GetM_InOutLine_ID())
                                        {
                                            inoutline = new MInOutLine(ctx, invoiceline.GetM_InOutLine_ID(), trxName);
                                            if (costingCheck != null && costingCheck.inoutline == null)
                                            {
                                                costingCheck.inoutline = inoutline;
                                            }
                                        }
                                    }
                                    if (inoutline.GetM_InOutLine_ID() > 0)
                                    {
                                        M_ASI_ID = inoutline.GetM_AttributeSetInstance_ID();
                                        if (!client.IsCostImmediate())
                                        {
                                            windowName = "Match IV";
                                        }
                                    }
                                }
                                #endregion
                            }

                            if (invoiceline.GetC_OrderLine_ID() <= 0)
                            {
                                #region when invoice line not having orderline ref but contain inoutline ref then handlinf costing through Match IV region
                                if (matchInoutLine != null && matchInoutLine.GetM_InOutLine_ID() > 0)
                                {
                                    if (costingCheck != null)
                                    {
                                        inoutline = costingCheck.inoutline;
                                    }
                                    if (inoutline == null || inoutline.Get_ID() <= 0 || inoutline.Get_ID() != matchInoutLine.GetM_InOutLine_ID())
                                    {
                                        inoutline = new MInOutLine(ctx, matchInoutLine.GetM_InOutLine_ID(), trxName);
                                        if (costingCheck != null && costingCheck.inoutline == null)
                                        {
                                            costingCheck.inoutline = inoutline;
                                        }
                                    }
                                }
                                else
                                {
                                    if (costingCheck != null)
                                    {
                                        inoutline = costingCheck.inoutline;
                                    }
                                    if (inoutline == null || inoutline.Get_ID() <= 0 || inoutline.Get_ID() != invoiceline.GetM_InOutLine_ID())
                                    {
                                        inoutline = new MInOutLine(ctx, invoiceline.GetM_InOutLine_ID(), trxName);
                                        if (costingCheck != null && costingCheck.inoutline == null)
                                        {
                                            costingCheck.inoutline = inoutline;
                                        }
                                    }
                                }
                                if (inoutline.GetM_InOutLine_ID() > 0)
                                {
                                    M_ASI_ID = inoutline.GetM_AttributeSetInstance_ID();
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
                            pca = MProductCategory.Get(product.GetCtx(), product.GetM_Product_Category_ID());
                            if (pca != null)
                            {
                                cl = pca.GetCostingLevel();
                                costingMethodMatchPO = pca.GetCostingMethod();
                                if (costingMethodMatchPO == "C")
                                {
                                    query.Clear();
                                    query.Append(@" SELECT costingmethod FROM m_costelement WHERE m_costelement_id = (SELECT CAST(cel.M_Ref_CostElement AS INTEGER)
                                    FROM M_CostElement ce INNER JOIN m_costelementline cel ON ce.M_CostElement_ID  = cel.M_CostElement_ID
                                    WHERE ce.AD_Client_ID   =" + product.GetAD_Client_ID() + @" 
                                    AND ce.IsActive         ='Y' AND ce.CostElementType  ='C'
                                    AND cel.IsActive        ='Y' AND ce.M_CostElement_ID = " + pca.GetM_CostElement_ID() + @"
                                    AND CAST(cel.M_Ref_CostElement AS INTEGER) IN (SELECT M_CostElement_ID FROM M_CostELEMENT WHERE costingmethod IS NOT NULL  ) )
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
                                    query.Append(@" SELECT costingmethod FROM m_costelement WHERE m_costelement_id = (SELECT CAST(cel.M_Ref_CostElement AS INTEGER)
                                    FROM M_CostElement ce INNER JOIN m_costelementline cel ON ce.M_CostElement_ID  = cel.M_CostElement_ID
                                    WHERE ce.AD_Client_ID   =" + product.GetAD_Client_ID() + @" 
                                    AND ce.IsActive         ='Y' AND ce.CostElementType  ='C'
                                    AND cel.IsActive        ='Y' AND ce.M_CostElement_ID = " + pca.GetM_CostElement_ID() + @"
                                    AND CAST(cel.M_Ref_CostElement AS INTEGER) IN (SELECT M_CostElement_ID FROM M_CostELEMENT WHERE costingmethod IS NOT NULL  ) )
                                    ");
                                }
                            }
                            if (costingMethodMatchPO == "C")
                            {
                                costingMethodMatchPO = Util.GetValueOfString(DB.ExecuteScalar(query.ToString(), null, trxName));
                            }
                            #endregion

                            if (costingCheck != null)
                            {
                                inout = costingCheck.inout;
                            }
                            if (inout == null || inout.Get_ID() <= 0 || inout.Get_ID() != inoutline.GetM_InOut_ID())
                            {
                                inout = new MInOut(ctx, inoutline.GetM_InOut_ID(), trxName);
                                if (costingCheck != null && costingCheck.inout == null)
                                {
                                    costingCheck.inout = inout;
                                }
                            }

                            orderLineId = Util.GetValueOfInt(Price); // here price parametr contain Orderline id from controller
                            if (costingCheck != null)
                            {
                                orderline = costingCheck.orderline;
                            }
                            if (orderline == null || orderline.Get_ID() <= 0 || orderline.Get_ID() != orderLineId)
                            {
                                orderline = new MOrderLine(ctx, orderLineId, trxName);
                                if (costingCheck != null && costingCheck.orderline == null)
                                {
                                    costingCheck.orderline = orderline;
                                }
                            }

                            #region get price from purchase order
                            if (costingCheck != null)
                            {
                                order = costingCheck.order;
                            }
                            if (order == null || order.Get_ID() <= 0 || order.Get_ID() != orderline.GetC_Order_ID())
                            {
                                order = new MOrder(ctx, orderline.GetC_Order_ID(), trxName);
                                if (costingCheck != null && costingCheck.order == null)
                                {
                                    costingCheck.order = order;
                                }
                            }
                            Decimal ProductOrderLineCost = orderline.GetProductLineCost(orderline);
                            if (order.GetC_Currency_ID() != acctSchema.GetC_Currency_ID())
                            {
                                Price = MConversionRate.Convert(ctx, Decimal.Divide(ProductOrderLineCost, orderline.GetQtyOrdered()), order.GetC_Currency_ID(), acctSchema.GetC_Currency_ID(),
                                    (inout != null ? inout.GetDateAcct() : order.GetDateAcct()), order.GetC_ConversionType_ID(), AD_Client_ID, AD_Org_ID);
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
                            // costing method is not Avregae PO / Last PO / We Av PO
                            // then no need to calculate cost but we are creating cost detail with PO price and qty
                            if (client.IsCostImmediate())
                            {
                                #region cost detail creation or not
                                if (costingMethodMatchPO == "A" || costingMethodMatchPO == "p" ||
                                    costingMethodMatchPO == MCostElement.COSTINGMETHOD_WeightedAveragePO)
                                {
                                    // not to create cost detail by this section if costing method is either Average PO or Last PO oe We AV PO
                                }
                                else if (inoutline.IsCostCalculated())
                                {
                                    //  need to calculate cost for Average PO and Last PO / We Av PO
                                    // bcz we alreadyy calculate cost against this mr otherwise costing not to be calculated
                                }
                                else
                                {
                                    // created cost detail with orderline ref 
                                    cd = new MCostDetail(acctSchema, AD_Org_ID, product.GetM_Product_ID(), M_ASI_ID,
                                                          0, Decimal.Round(Decimal.Multiply(Qty, Price), acctSchema.GetCostingPrecision()), Qty, null, trxName);
                                    cd.SetM_InOutLine_ID(inoutline.GetM_InOutLine_ID());
                                    cd.SetC_OrderLine_ID(orderLineId);
                                    cd.SetM_Warehouse_ID(inout.GetM_Warehouse_ID());
                                    if (!cd.Save(trxName))
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        _log.Severe("Error occured during saving a record, M_InoutLine_Id = " + inoutline.GetM_InOutLine_ID() + " in Cost-Detail -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
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
                                query.Append("SELECT MIN(M_CostDetail_ID) FROM M_CostDetail WHERE M_InOutLine_ID = " + inoutline.GetM_InOutLine_ID() +
                                     " AND c_invoiceline_id IS NULL  AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID());
                                costDetailId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));
                            }

                            cd = new MCostDetail(ctx, costDetailId, trxName);
                            if (costDetailId == 0)
                            {
                                cd.SetClientOrg(AD_Client_ID, AD_Org_ID);
                                cd.SetM_Product_ID(product.GetM_Product_ID());
                                cd.SetM_InOutLine_ID(inoutline.GetM_InOutLine_ID());
                                cd.SetM_AttributeSetInstance_ID(M_ASI_ID);
                                cd.SetC_AcctSchema_ID(acctSchema.GetC_AcctSchema_ID());
                                cd.SetM_Warehouse_ID(inout.GetM_Warehouse_ID());
                            }

                            // MR costing calculated before matching PO with MR
                            if (inoutline.IsCostCalculated() && inout.IsCostCalculated() && !client.IsCostImmediate())
                            {
                                #region Get MR price
                                query.Clear();
                                if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_Organization)
                                {
                                    query.Append(@"SELECT  ROUND(Amt/Qty , 4) as currentCostAmount  FROM m_costelementdetail ced 
                                                                where  ced.IsActive = 'Y' AND ced.M_Product_ID = " + product.GetM_Product_ID() + @" 
                                                                 AND ced.C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() + @" AND  NVL(ced.M_AttributeSetInstance_ID , 0) = 0" +
                                       " AND ced.M_InOutLine_ID =  " + inoutline.GetM_InOutLine_ID() + @" AND NVL(ced.C_OrderLIne_ID , 0) = 0 AND NVL(ced.C_InvoiceLine_ID , 0) = 0 and 
                                                                 ced.M_CostElement_ID = (SELECT M_CostElement_ID from M_costElement where costingMethod = 'A' AND IsActive = 'Y' AND AD_Client_ID = " + inoutline.GetAD_Client_ID() + " ) AND ced.AD_Client_ID = " + inoutline.GetAD_Client_ID());
                                }
                                else if (cl == MProductCategory.COSTINGLEVEL_BatchLot || cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch)
                                {
                                    query.Append(@"SELECT  ROUND(Amt/Qty , 4) as currentCostAmount  FROM m_costelementdetail ced 
                                                                where  ced.IsActive = 'Y' AND ced.M_Product_ID = " + product.GetM_Product_ID() + @" 
                                                                 AND ced.C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() + @" AND  NVL(ced.M_AttributeSetInstance_ID , 0) = " + M_ASI_ID + @" 
                                                                 AND ced.M_InOutLine_ID =  " + inoutline.GetM_InOutLine_ID() + @" AND NVL(ced.C_OrderLIne_ID , 0) = 0 AND NVL(ced.C_InvoiceLine_ID , 0) = 0 and 
                                                                 ced.M_CostElement_ID = (SELECT M_CostElement_ID from M_costElement where costingMethod = 'A' AND IsActive = 'Y' AND AD_Client_ID = " + inoutline.GetAD_Client_ID() + " ) AND ced.AD_Client_ID = " + inoutline.GetAD_Client_ID());
                                }
                                else if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                {
                                    query.Append(@"SELECT  ROUND(Amt/Qty , 4) as currentCostAmount  FROM m_costelementdetail ced 
                                                                where  ced.IsActive = 'Y' AND ced.M_Product_ID = " + product.GetM_Product_ID() + @" AND  NVL(ced.M_Warehouse_ID , 0) = " + inout.GetM_Warehouse_ID() + @" 
                                                                 AND ced.C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                                              @" AND  NVL(ced.M_AttributeSetInstance_ID , 0) = " + (cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch ? M_ASI_ID : 0) + @" 
                                                                 AND ced.M_InOutLine_ID =  " + inoutline.GetM_InOutLine_ID() + @" AND NVL(ced.C_OrderLIne_ID , 0) = 0 AND NVL(ced.C_InvoiceLine_ID , 0) = 0 and 
                                                                 ced.M_CostElement_ID = (SELECT M_CostElement_ID from M_costElement where costingMethod = 'A' AND IsActive = 'Y' AND AD_Client_ID = " + inoutline.GetAD_Client_ID() + " ) AND ced.AD_Client_ID = " + inoutline.GetAD_Client_ID());
                                }
                                MRPriceAvPo = Util.GetValueOfDecimal(DB.ExecuteScalar(query.ToString()));

                                query.Clear();
                                if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_Organization)
                                {
                                    query.Append(@"SELECT  ROUND(Amt/Qty , 4) as currentCostAmount  FROM m_costelementdetail ced 
                                                                where  ced.IsActive = 'Y' AND ced.M_Product_ID = " + product.GetM_Product_ID() + @" 
                                                                 AND ced.C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() + @" AND  NVL(ced.M_AttributeSetInstance_ID , 0) = 0 " +
                                                               " AND ced.M_InOutLine_ID =  " + inoutline.GetM_InOutLine_ID() + @" AND NVL(ced.C_OrderLIne_ID , 0) = 0 AND NVL(ced.C_InvoiceLine_ID , 0) = 0 AND 
                                                                 ced.M_CostElement_ID = (SELECT M_CostElement_ID from M_costElement where costingMethod = 'p' AND IsActive = 'Y' AND AD_Client_ID = " + inoutline.GetAD_Client_ID() + " ) AND ced.AD_Client_ID = " + inoutline.GetAD_Client_ID());
                                }
                                else if (cl == MProductCategory.COSTINGLEVEL_BatchLot || cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch)
                                {
                                    query.Append(@"SELECT  ROUND(Amt/Qty , 4) as currentCostAmount  FROM m_costelementdetail ced 
                                                                where  ced.IsActive = 'Y' AND ced.M_Product_ID = " + product.GetM_Product_ID() + @" 
                                                                 AND ced.C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() + @" AND  NVL(ced.M_AttributeSetInstance_ID , 0) = " + M_ASI_ID + @" 
                                                                 AND ced.M_InOutLine_ID =  " + inoutline.GetM_InOutLine_ID() + @" AND NVL(ced.C_OrderLIne_ID , 0) = 0 AND NVL(ced.C_InvoiceLine_ID , 0) = 0 and 
                                                                 ced.M_CostElement_ID = (SELECT M_CostElement_ID from M_costElement where costingMethod = 'p' AND IsActive = 'Y' AND AD_Client_ID = " + inoutline.GetAD_Client_ID() + " ) AND ced.AD_Client_ID = " + inoutline.GetAD_Client_ID());
                                }
                                else if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                {
                                    query.Append(@"SELECT  ROUND(Amt/Qty , 4) as currentCostAmount  FROM m_costelementdetail ced 
                                                                where  ced.IsActive = 'Y' AND ced.M_Product_ID = " + product.GetM_Product_ID() + @"  AND  NVL(ced.M_Warehouse_ID , 0) = " + inout.GetM_Warehouse_ID() + @" 
                                                                 AND ced.C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                                              @" AND  NVL(ced.M_AttributeSetInstance_ID , 0) = " + (cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch ? M_ASI_ID : 0) + @" 
                                                                 AND ced.M_InOutLine_ID =  " + inoutline.GetM_InOutLine_ID() + @" AND NVL(ced.C_OrderLIne_ID , 0) = 0 AND NVL(ced.C_InvoiceLine_ID , 0) = 0 and 
                                                                 ced.M_CostElement_ID = (SELECT M_CostElement_ID from M_costElement where costingMethod = 'p' AND IsActive = 'Y' AND AD_Client_ID = " + inoutline.GetAD_Client_ID() + " ) AND ced.AD_Client_ID = " + inoutline.GetAD_Client_ID());
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
                                    if (!CreateCostQueueForMatchPO(ctx, acctSchema, product, M_ASI_ID, AD_Client_ID, AD_Org_ID,
                                        Price, inoutline.GetMovementQty(), inoutline, trxName, inout.GetM_Warehouse_ID(), optionalStrPO: optionalstr))
                                    {
                                        _log.Severe("Error occured during craetion of cost queue -> MatchPO. M_Inoutline_ID is " + inoutline.GetM_InOutLine_ID());
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
                                query.Append(@"SELECT amt FROM T_Temp_CostDetail WHERE IsActive = 'Y' AND AD_Client_ID = " + AD_Client_ID +
                                             " AND AD_Org_ID = " + AD_Org_ID + " AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                             " AND M_Product_ID = " + product.GetM_Product_ID() + " AND isRecordFromForm = 'Y' " +
                                             " AND M_InOutLine_ID =  " + inoutline.GetM_InOutLine_ID());
                                MRPriceAvPo = Util.GetValueOfDecimal(DB.ExecuteScalar(query.ToString(), null, trxName));
                                if (MRPriceAvPo == 0)
                                {
                                    query.Clear();
                                    query.Append(@"SELECT amt FROM T_Temp_CostDetail WHERE IsActive = 'Y' AND AD_Client_ID = " + AD_Client_ID +
                                                 " AND AD_Org_ID = " + inoutline.GetAD_Org_ID() + " AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                                 " AND M_Product_ID = " + product.GetM_Product_ID() + " AND isRecordFromForm = 'Y' " +
                                                 " AND M_InOutLine_ID =  " + inoutline.GetM_InOutLine_ID());
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
                                query.Append(@"SELECT amt FROM T_Temp_CostDetail WHERE IsActive = 'Y' AND AD_Client_ID = " + AD_Client_ID +
                                             " AND AD_Org_ID = " + AD_Org_ID + " AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                             " AND M_Product_ID = " + product.GetM_Product_ID() + " AND NVL(C_OrderLine_ID , 0) = 0  " +
                                             " AND M_InOutLine_ID =  " + inoutline.GetM_InOutLine_ID());
                                MRPriceAvPo = Util.GetValueOfDecimal(DB.ExecuteScalar(query.ToString(), null, trxName));
                                if (MRPriceAvPo == 0)
                                {
                                    query.Clear();
                                    query.Append(@"SELECT amt FROM T_Temp_CostDetail WHERE IsActive = 'Y' AND AD_Client_ID = " + AD_Client_ID +
                                                 " AND AD_Org_ID = " + inoutline.GetAD_Org_ID() + " AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                                 " AND M_Product_ID = " + product.GetM_Product_ID() + " AND NVL(C_OrderLine_ID , 0) = 0  " +
                                                 " AND M_InOutLine_ID =  " + inoutline.GetM_InOutLine_ID());
                                    MRPriceAvPo = Util.GetValueOfDecimal(DB.ExecuteScalar(query.ToString(), null, trxName));
                                }
                                MRPriceLastPO = MRPriceAvPo;
                                #endregion
                            }

                            // update m_cost Accumulative Amount to be reduced for Av. PO and Last PO
                            if (cd.GetC_OrderLine_ID() == 0)
                            {
                                int costElementId = 0;
                                // when iscostimmediate = true
                                // if mr line cost already calculated then during match PO with MR we have to calculate both Average PO and Last PO
                                if ((optionalstr == "window" && (costingMethodMatchPO == "A")) || optionalstr == "process")
                                {
                                    query.Clear();
                                    query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE  AD_Client_ID=" + AD_Client_ID +
                                                 @" AND IsActive='Y' AND CostElementType='M'  AND CostingMethod = 'A'");
                                    costElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));

                                    costElement = MCostElement.Get(ctx, costElementId);

                                    #region Av. PO
                                    if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_Organization) //(cl != "B")
                                    {
                                        cost = MCost.Get(product, 0, acctSchema, AD_Org_ID, costElementId);
                                    }
                                    else if (cl == MProductCategory.COSTINGLEVEL_BatchLot || cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch)
                                    {
                                        cost = MCost.Get(product, M_ASI_ID, acctSchema, AD_Org_ID, costElementId);
                                    }
                                    else if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                    {
                                        cost = MCost.Get(product, (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID), acctSchema, AD_Org_ID, costElementId, inout.GetM_Warehouse_ID());
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
                                        _log.Severe("Error occured during saving a record , M_inoutLine_ID = " + inoutline.GetM_InOutLine_ID() + " in cost detail -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                        return false;
                                    }

                                    cd.SetC_OrderLine_ID(orderLineId);
                                    cd.SetQty(Qty);
                                    cd.SetAmt(Decimal.Round(Decimal.Multiply(Price, Qty), acctSchema.GetCostingPrecision()));
                                    if (!cd.Save(trxName))
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        _log.Severe("Error occured during saving a record, M_InoutLine_Id = " + inoutline.GetM_InOutLine_ID() + " in cost detail -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
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
                                        _log.Severe("Error occured during saving a record, M_InoutLine_ID = " + inoutline.GetM_InOutLine_ID() + " in M-Cost -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                        return false;
                                    }
                                    else
                                    {
                                        if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_Warehouse
                                            || cl == MProductCategory.COSTINGLEVEL_Organization)//(cl != "B")
                                        {
                                            MCostElementDetail.CreateCostElementDetail(ctx, AD_Client_ID, AD_Org_ID, product, 0,
                                                                 acctSchema, costElementId, windowName, cd, Decimal.Round(Decimal.Multiply(cost.GetCurrentCostPrice(), Qty), acctSchema.GetCostingPrecision()), Qty);
                                        }
                                        else if (cl == MProductCategory.COSTINGLEVEL_BatchLot || cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch
                                            || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                        {
                                            MCostElementDetail.CreateCostElementDetail(ctx, AD_Client_ID, AD_Org_ID, product, M_ASI_ID,
                                                                     acctSchema, costElementId, windowName, cd, Decimal.Round(Decimal.Multiply(cost.GetCurrentCostPrice(), Qty), acctSchema.GetCostingPrecision()), Qty);
                                        }
                                    }
                                    #endregion

                                }

                                if ((optionalstr == "window" && (costingMethodMatchPO == "p")) || optionalstr == "process")
                                {
                                    #region last po
                                    query.Clear();
                                    query.Append("SELECT M_CostElement_ID FROM M_CostElement WHERE  AD_Client_ID=" + AD_Client_ID + " AND IsActive='Y' AND CostElementType='M'  AND CostingMethod = 'p'");
                                    costElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));

                                    costElement = MCostElement.Get(ctx, costElementId);

                                    if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_Organization) //(cl != "B")
                                    {
                                        cost = MCost.Get(product, 0, acctSchema, AD_Org_ID, costElementId);
                                    }
                                    else if (cl == MProductCategory.COSTINGLEVEL_BatchLot || cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch)
                                    {
                                        cost = MCost.Get(product, M_ASI_ID, acctSchema, AD_Org_ID, costElementId);
                                    }
                                    else if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                    {
                                        cost = MCost.Get(product, (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID), acctSchema, AD_Org_ID, costElementId, inout.GetM_Warehouse_ID());
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
                                        _log.Severe("Error occured during saving a record, N_InoutLine_ID = " + inoutline.GetM_InOutLine_ID() + " in M-Cost -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                        return false;
                                    }
                                    else
                                    {
                                        if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_Warehouse
                                            || cl == MProductCategory.COSTINGLEVEL_Organization) //(cl != "B")
                                        {
                                            MCostElementDetail.CreateCostElementDetail(ctx, AD_Client_ID, AD_Org_ID, product, 0,
                                                                 acctSchema, costElementId, windowName, cd, Decimal.Round(Decimal.Multiply(cost.GetCurrentCostPrice(), Qty), acctSchema.GetCostingPrecision()), Qty);
                                        }
                                        else if (cl == MProductCategory.COSTINGLEVEL_BatchLot || cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch
                                            || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                        {
                                            MCostElementDetail.CreateCostElementDetail(ctx, AD_Client_ID, AD_Org_ID, product, M_ASI_ID,
                                                                 acctSchema, costElementId, windowName, cd, Decimal.Round(Decimal.Multiply(cost.GetCurrentCostPrice(), Qty), acctSchema.GetCostingPrecision()), Qty);
                                        }
                                    }
                                    #endregion
                                }

                                if ((optionalstr == "window" && (costingMethodMatchPO == MCostElement.COSTINGMETHOD_WeightedAveragePO)) || optionalstr == "process")
                                {
                                    query.Clear();
                                    query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE  AD_Client_ID=" + AD_Client_ID +
                                                 @" AND IsActive='Y' AND CostElementType='M'  AND CostingMethod = 'O'");
                                    costElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));

                                    costElement = MCostElement.Get(ctx, costElementId);

                                    #region Weighted Av. PO
                                    if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_Organization)
                                    {
                                        cost = MCost.Get(product, 0, acctSchema, AD_Org_ID, costElementId);
                                    }
                                    else if (cl == MProductCategory.COSTINGLEVEL_BatchLot || cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch)
                                    {
                                        cost = MCost.Get(product, M_ASI_ID, acctSchema, AD_Org_ID, costElementId);
                                    }
                                    else if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                    {
                                        cost = MCost.Get(product, (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID), acctSchema, AD_Org_ID, costElementId, inout.GetM_Warehouse_ID());
                                    }

                                    cd.SetC_OrderLine_ID(orderLineId);
                                    cd.SetQty(Qty);
                                    cd.SetAmt(Decimal.Round(Decimal.Multiply(Price, Qty), acctSchema.GetCostingPrecision()));
                                    if (!cd.Save(trxName))
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        _log.Severe("Error occured during saving a record, M_InoutLine_Id = " + inoutline.GetM_InOutLine_ID() + " in cost detail -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                        return false;
                                    }

                                    Decimal WACcost = Decimal.Add(cd.GetAmt(),
                                                           Decimal.Round(Decimal.Divide(Decimal.Multiply(cd.GetAmt(), costElement.GetSurchargePercentage()), 100), acctSchema.GetCostingPrecision()));

                                    if (Decimal.Add(cost.GetCurrentQty(), cd.GetQty()) != 0)
                                    {
                                        cost.SetCurrentCostPrice(Decimal.Round(Decimal.Divide(
                                               Decimal.Add(
                                               Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()), WACcost),
                                               Decimal.Add(cost.GetCurrentQty(), cd.GetQty()))
                                               , acctSchema.GetCostingPrecision(), MidpointRounding.AwayFromZero));
                                    }

                                    cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), WACcost));
                                    cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), cd.GetQty()));
                                    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), cd.GetQty()));

                                    if (!cost.Save())
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        _log.Severe("Error occured during saving a record, M_InoutLine_ID = " + inoutline.GetM_InOutLine_ID() + " in M-Cost -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                        return false;
                                    }
                                    else
                                    {
                                        if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_Warehouse
                                            || cl == MProductCategory.COSTINGLEVEL_Organization)
                                        {
                                            MCostElementDetail.CreateCostElementDetail(ctx, AD_Client_ID, AD_Org_ID, product, 0,
                                                                 acctSchema, costElementId, windowName, cd, Decimal.Round(Decimal.Multiply(cost.GetCurrentCostPrice(), Qty), acctSchema.GetCostingPrecision()), Qty);
                                        }
                                        else if (cl == MProductCategory.COSTINGLEVEL_BatchLot || cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch
                                            || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                        {
                                            MCostElementDetail.CreateCostElementDetail(ctx, AD_Client_ID, AD_Org_ID, product, M_ASI_ID,
                                                                     acctSchema, costElementId, windowName, cd, Decimal.Round(Decimal.Multiply(cost.GetCurrentCostPrice(), Qty), acctSchema.GetCostingPrecision()), Qty);
                                        }
                                    }
                                    #endregion

                                }
                            }
                            else
                            {
                                cd = new MCostDetail(acctSchema, AD_Org_ID, product.GetM_Product_ID(), M_ASI_ID,
                                                      0, Decimal.Round(Decimal.Multiply(Qty, Price), acctSchema.GetCostingPrecision()), Qty, null, trxName);
                                cd.SetM_InOutLine_ID(inoutline.GetM_InOutLine_ID());
                                cd.SetC_OrderLine_ID(orderLineId);
                                cd.SetM_Warehouse_ID(inout.GetM_Warehouse_ID());
                                if (!cd.Save(trxName))
                                {
                                    if (optionalstr != "window")
                                    {
                                        trxName.Rollback();
                                    }
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    _log.Severe("Error occured during saving a record, M_InoutLine_Id = " + inoutline.GetM_InOutLine_ID() + " in Cost-Detail -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                    return false;
                                }

                                int costElementId = 0;
                                if ((optionalstr == "window" && (costingMethodMatchPO == "A")) || optionalstr == "process")
                                {
                                    #region Av. PO
                                    query.Clear();
                                    query.Append("SELECT M_CostElement_ID FROM M_CostElement WHERE  AD_Client_ID=" + AD_Client_ID + " AND IsActive='Y' AND CostElementType='M'  AND CostingMethod = 'A'");
                                    costElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));

                                    costElement = MCostElement.Get(ctx, costElementId);

                                    if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_Organization) //(cl != "B")
                                    {
                                        cost = MCost.Get(product, 0, acctSchema, AD_Org_ID, costElementId);
                                    }
                                    else if (cl == MProductCategory.COSTINGLEVEL_BatchLot || cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch)
                                    {
                                        cost = MCost.Get(product, M_ASI_ID, acctSchema, AD_Org_ID, costElementId);
                                    }
                                    else if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                    {
                                        cost = MCost.Get(product, (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID), acctSchema, AD_Org_ID, costElementId, inout.GetM_Warehouse_ID());
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
                                        _log.Severe("Error occured during saving a record, M_inoutLine_Id = " + inoutline.GetM_InOutLine_ID() + " in M-Cost -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                        return false;
                                    }
                                    else
                                    {
                                        MCostElementDetail.CreateCostElementDetail(ctx, AD_Client_ID, AD_Org_ID, product, M_ASI_ID,
                                                             acctSchema, costElementId, windowName, cd, Decimal.Round(Decimal.Multiply(cost.GetCurrentCostPrice(), Qty), acctSchema.GetCostingPrecision()), Qty);
                                    }
                                    #endregion
                                }
                                if ((optionalstr == "window" && (costingMethodMatchPO == "p")) || optionalstr == "process")
                                {
                                    #region last po
                                    query.Clear();
                                    query.Append("SELECT M_CostElement_ID FROM M_CostElement WHERE  AD_Client_ID=" + AD_Client_ID + " AND IsActive='Y' AND CostElementType='M'  AND CostingMethod = 'p'");
                                    costElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));

                                    costElement = MCostElement.Get(ctx, costElementId);

                                    if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_Organization) //(cl != "B")
                                    {
                                        cost = MCost.Get(product, 0, acctSchema, AD_Org_ID, costElementId);
                                    }
                                    else if (cl == MProductCategory.COSTINGLEVEL_BatchLot || cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch)
                                    {
                                        cost = MCost.Get(product, M_ASI_ID, acctSchema, AD_Org_ID, costElementId);
                                    }
                                    else if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                    {
                                        cost = MCost.Get(product, (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID), acctSchema, AD_Org_ID, costElementId, inout.GetM_Warehouse_ID());
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
                                        _log.Severe("Error occured during saving a record, M_inoutLine_Id = " + inoutline.GetM_InOutLine_ID() + " in M-Cost -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                        return false;
                                    }
                                    else
                                    {
                                        MCostElementDetail.CreateCostElementDetail(ctx, AD_Client_ID, AD_Org_ID, product, M_ASI_ID,
                                                             acctSchema, costElementId, windowName, cd, Decimal.Round(Decimal.Multiply(cost.GetCurrentCostPrice(), Qty), acctSchema.GetCostingPrecision()), Qty);
                                    }
                                    #endregion
                                }
                                if ((optionalstr == "window" && (costingMethodMatchPO == MCostElement.COSTINGMETHOD_WeightedAveragePO)) || optionalstr == "process")
                                {
                                    #region We Av. PO
                                    query.Clear();
                                    query.Append("SELECT M_CostElement_ID FROM M_CostElement WHERE  AD_Client_ID=" + AD_Client_ID + " AND IsActive='Y' AND CostElementType='M'  AND CostingMethod = 'O'");
                                    costElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));

                                    costElement = MCostElement.Get(ctx, costElementId);

                                    if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_Organization) //(cl != "B")
                                    {
                                        cost = MCost.Get(product, 0, acctSchema, AD_Org_ID, costElementId);
                                    }
                                    else if (cl == MProductCategory.COSTINGLEVEL_BatchLot || cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch)
                                    {
                                        cost = MCost.Get(product, M_ASI_ID, acctSchema, AD_Org_ID, costElementId);
                                    }
                                    else if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                    {
                                        cost = MCost.Get(product, (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID), acctSchema, AD_Org_ID, costElementId, inout.GetM_Warehouse_ID());
                                    }

                                    Decimal WACcost = Decimal.Add(cd.GetAmt(),
                                                           Decimal.Round(Decimal.Divide(Decimal.Multiply(cd.GetAmt(), costElement.GetSurchargePercentage()), 100), acctSchema.GetCostingPrecision()));

                                    if (Decimal.Add(cost.GetCurrentQty(), cd.GetQty()) != 0)
                                    {
                                        cost.SetCurrentCostPrice(Decimal.Round(Decimal.Divide(
                                               Decimal.Add(
                                               Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()), WACcost),
                                               Decimal.Add(cost.GetCurrentQty(), cd.GetQty()))
                                               , acctSchema.GetCostingPrecision(), MidpointRounding.AwayFromZero));
                                    }

                                    cost.SetCumulatedAmt(Decimal.Add(cost.GetCumulatedAmt(), WACcost));
                                    cost.SetCumulatedQty(Decimal.Add(cost.GetCumulatedQty(), cd.GetQty()));
                                    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), cd.GetQty()));

                                    if (!cost.Save())
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        _log.Severe("Error occured during saving a record, M_inoutLine_Id = " + inoutline.GetM_InOutLine_ID() + " in M-Cost -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                        return false;
                                    }
                                    else
                                    {
                                        MCostElementDetail.CreateCostElementDetail(ctx, AD_Client_ID, AD_Org_ID, product, M_ASI_ID,
                                                             acctSchema, costElementId, windowName, cd, Decimal.Round(Decimal.Multiply(cost.GetCurrentCostPrice(), Qty), acctSchema.GetCostingPrecision()), Qty);
                                    }
                                    #endregion
                                }
                            }

                            // Update Cost Queue Price
                            if (!UpdateCostQueuePrice(0, MRPriceAvPo, Qty, Price, inoutline.GetM_InOutLine_ID(), acctSchema.GetCostingPrecision()))
                            {
                                _log.Severe("Cost Queue Cost not updated for GRN Line= " + inoutline.GetM_InOutLine_ID());
                                return false;
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

                            if (costingCheck == null || String.IsNullOrEmpty(costingCheck.costinglevel))
                            {
                                pca = MProductCategory.Get(product.GetCtx(), product.GetM_Product_Category_ID());
                                if (pca != null)
                                {
                                    cl = pca.GetCostingLevel();
                                }
                                else
                                {
                                    cl = acctSchema.GetCostingLevel();
                                }
                            }
                            else if (costingCheck != null)
                            {
                                cl = costingCheck.costinglevel;
                            }
                            query.Clear();
                            query.Append("SELECT MIN(M_CostDetail_ID) FROM M_CostDetail WHERE M_InOutLine_ID = " + inoutline.GetM_InOutLine_ID() +
                                    " AND c_orderline_id IS NULL  AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID());
                            costDetailId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));
                            cd = new MCostDetail(ctx, costDetailId, trxName);

                            query.Clear();
                            query.Append(@"SELECT C_OrderLine_Id FROM M_MatchPO WHERE IsActive = 'Y' AND M_MatchPO_ID = 
                                     (SELECT MIN(M_MatchPO_ID) FROM M_MatchPO WHERE IsActive = 'Y' AND M_InOutLine_ID = " + inoutline.GetM_InOutLine_ID() + ") ");
                            MatchPO_OrderLineId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString()));

                            ceFifo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_CostElement_ID from M_costElement where costingMethod = 'F' AND IsActive = 'Y' AND AD_Client_ID = " + inoutline.GetAD_Client_ID()));
                            ceLifo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_CostElement_ID from M_costElement where costingMethod = 'L' AND IsActive = 'Y' AND AD_Client_ID = " + inoutline.GetAD_Client_ID()));

                            #region MR Price against LIFO / FIFO
                            if (MRPriceLifo == 0)
                            {
                                if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_Warehouse
                                            || cl == MProductCategory.COSTINGLEVEL_Organization) //(cl != "B")
                                {
                                    query.Clear();
                                    query.Append(@"SELECT  Amt as currentCostAmount  FROM T_Temp_CostDetail ced INNER JOIN m_costqueue cq ON cq.m_costqueue_id = ced.m_costqueue_id 
                                    where  ced.IsActive = 'Y' AND ced.M_Product_ID = " + product.GetM_Product_ID() + @" AND cq.M_CostElement_ID = " + ceLifo + @" 
                                     AND ced.C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() + @" AND  NVL(ced.M_AttributeSetInstance_ID , 0) IN (0, " + M_ASI_ID + ") " +
                                         " AND ced.M_InOutLine_ID =  " + inoutline.GetM_InOutLine_ID() + @" AND NVL(ced.C_OrderLIne_ID , 0) = " + MatchPO_OrderLineId +
                                         @" AND NVL(ced.C_InvoiceLine_ID , 0) = 0 AND ced.AD_Client_ID = " + inoutline.GetAD_Client_ID());
                                }
                                else if (cl == MProductCategory.COSTINGLEVEL_BatchLot || cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch
                                            || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                {
                                    query.Clear();
                                    query.Append(@"SELECT  Amt as currentCostAmount  FROM T_Temp_CostDetail ced INNER JOIN m_costqueue cq ON cq.m_costqueue_id = ced.m_costqueue_id 
                                    where  ced.IsActive = 'Y' AND ced.M_Product_ID = " + product.GetM_Product_ID() + @" AND cq.M_CostElement_ID = " + ceLifo + @" 
                                     AND ced.C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() + @" AND  NVL(ced.M_AttributeSetInstance_ID , 0) = " + M_ASI_ID + @" 
                                     AND ced.M_InOutLine_ID =  " + inoutline.GetM_InOutLine_ID() + @" AND NVL(ced.C_OrderLIne_ID , 0) = " + MatchPO_OrderLineId +
                                     @" AND NVL(ced.C_InvoiceLine_ID , 0) = 0 AND ced.AD_Client_ID = " + inoutline.GetAD_Client_ID());
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
                                        query.Append(@"SELECT  Amt as currentCostAmount  FROM T_Temp_CostDetail ced INNER JOIN m_costqueue cq ON cq.m_costqueue_id = ced.m_costqueue_id 
                                    where  ced.IsActive = 'Y' AND ced.M_Product_ID = " + product.GetM_Product_ID() + @" AND cq.M_CostElement_ID = " + ceLifo + @" 
                                     AND ced.C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() + @" AND  NVL(ced.M_AttributeSetInstance_ID , 0)  IN (0, " + M_ASI_ID + ") " +
                                             " AND ced.M_InOutLine_ID =  " + inoutline.GetM_InOutLine_ID() + @" AND NVL(ced.C_OrderLIne_ID , 0) = 0 " +
                                             @" AND NVL(ced.C_InvoiceLine_ID , 0) = 0 AND ced.AD_Client_ID = " + inoutline.GetAD_Client_ID());
                                    }
                                    else if (cl == MProductCategory.COSTINGLEVEL_BatchLot || cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch
                                            || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT  Amt as currentCostAmount  FROM T_Temp_CostDetail ced INNER JOIN m_costqueue cq ON cq.m_costqueue_id = ced.m_costqueue_id 
                                    where  ced.IsActive = 'Y' AND ced.M_Product_ID = " + product.GetM_Product_ID() + @" AND cq.M_CostElement_ID = " + ceLifo + @" 
                                     AND ced.C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() + @" AND  NVL(ced.M_AttributeSetInstance_ID , 0) = " + M_ASI_ID + @" 
                                     AND ced.M_InOutLine_ID =  " + inoutline.GetM_InOutLine_ID() + @" AND NVL(ced.C_OrderLIne_ID , 0) = 0 " +
                                         @" AND NVL(ced.C_InvoiceLine_ID , 0) = 0 AND ced.AD_Client_ID = " + inoutline.GetAD_Client_ID());
                                    }
                                    MRPriceLifo = Util.GetValueOfDecimal(DB.ExecuteScalar(query.ToString()));
                                }
                            }

                            if (MRPriceFifo == 0)
                            {
                                if (cl == MProductCategory.COSTINGLEVEL_Client || cl == MProductCategory.COSTINGLEVEL_Warehouse
                                            || cl == MProductCategory.COSTINGLEVEL_Organization) //(cl != "B")
                                {
                                    query.Clear();
                                    query.Append(@"SELECT  Amt as currentCostAmount  FROM T_Temp_CostDetail ced INNER JOIN m_costqueue cq ON cq.m_costqueue_id = ced.m_costqueue_id 
                                    where  ced.IsActive = 'Y' AND ced.M_Product_ID = " + product.GetM_Product_ID() + @" AND cq.M_CostElement_ID = " + ceFifo + @" 
                                     AND ced.C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() + @" AND  NVL(ced.M_AttributeSetInstance_ID , 0)  IN (0, " + M_ASI_ID + ") " +
                                         " AND ced.M_InOutLine_ID =  " + inoutline.GetM_InOutLine_ID() + @" AND NVL(ced.C_OrderLIne_ID , 0) = " + MatchPO_OrderLineId +
                                         @" AND NVL(ced.C_InvoiceLine_ID , 0) = 0 AND ced.AD_Client_ID = " + inoutline.GetAD_Client_ID());
                                }
                                else if (cl == MProductCategory.COSTINGLEVEL_BatchLot || cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch
                                            || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                {
                                    query.Clear();
                                    query.Append(@"SELECT  Amt as currentCostAmount  FROM T_Temp_CostDetail ced INNER JOIN m_costqueue cq ON cq.m_costqueue_id = ced.m_costqueue_id 
                                    where  ced.IsActive = 'Y' AND ced.M_Product_ID = " + product.GetM_Product_ID() + @" AND cq.M_CostElement_ID = " + ceFifo + @" 
                                     AND ced.C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() + @" AND  NVL(ced.M_AttributeSetInstance_ID , 0) = " + M_ASI_ID + @" 
                                     AND ced.M_InOutLine_ID =  " + inoutline.GetM_InOutLine_ID() + @" AND NVL(ced.C_OrderLIne_ID , 0) = " + MatchPO_OrderLineId +
                                     @" AND NVL(ced.C_InvoiceLine_ID , 0) = 0 AND ced.AD_Client_ID = " + inoutline.GetAD_Client_ID());
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
                                        query.Append(@"SELECT  Amt as currentCostAmount  FROM T_Temp_CostDetail ced INNER JOIN m_costqueue cq ON cq.m_costqueue_id = ced.m_costqueue_id 
                                    where  ced.IsActive = 'Y' AND ced.M_Product_ID = " + product.GetM_Product_ID() + @" AND cq.M_CostElement_ID = " + ceFifo + @" 
                                     AND ced.C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() + @" AND  NVL(ced.M_AttributeSetInstance_ID , 0)  IN (0, " + M_ASI_ID + ") " +
                                             " AND ced.M_InOutLine_ID =  " + inoutline.GetM_InOutLine_ID() + @" AND NVL(ced.C_OrderLIne_ID , 0) = 0 " +
                                             @" AND NVL(ced.C_InvoiceLine_ID , 0) = 0 AND ced.AD_Client_ID = " + inoutline.GetAD_Client_ID());
                                    }
                                    else if (cl == MProductCategory.COSTINGLEVEL_BatchLot || cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch
                                            || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT  Amt as currentCostAmount  FROM T_Temp_CostDetail ced INNER JOIN m_costqueue cq ON cq.m_costqueue_id = ced.m_costqueue_id 
                                    where  ced.IsActive = 'Y' AND ced.M_Product_ID = " + product.GetM_Product_ID() + @" AND cq.M_CostElement_ID = " + ceFifo + @" 
                                     AND ced.C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() + @" AND  NVL(ced.M_AttributeSetInstance_ID , 0) = " + M_ASI_ID + @" 
                                     AND ced.M_InOutLine_ID =  " + inoutline.GetM_InOutLine_ID() + @" AND NVL(ced.C_OrderLIne_ID , 0) = 0 " +
                                         @" AND NVL(ced.C_InvoiceLine_ID , 0) = 0 AND ced.AD_Client_ID = " + inoutline.GetAD_Client_ID());
                                    }
                                    MRPriceFifo = Util.GetValueOfDecimal(DB.ExecuteScalar(query.ToString()));
                                }
                            }
                            #endregion

                            // not execute this if section
                            if (costDetailId > 0 && cd.GetC_InvoiceLine_ID() == 0 && handlingWindowName != "Match IV" && handlingWindowName != "Invoice(Vendor)")
                            {
                                // set amount and qty on cost detail from match receipt
                                cd.SetAmt(Price);
                                cd.SetQty(Qty);
                                cd.SetC_InvoiceLine_ID(invoiceline.GetC_InvoiceLine_ID());
                                cd.SetC_OrderLine_ID(invoiceline.GetC_OrderLine_ID());
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
                                query.Append(@"SELECT * FROM T_Temp_CostDetail WHERE  M_InOutLine_ID = " + inoutline.GetM_InOutLine_ID() +
                                              " AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID());
                                DataSet ds2 = DB.ExecuteDataset(query.ToString(), null, null);

                                // reduce cummulative amount , qty for all element
                                cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, invoiceline.GetAD_Org_ID(), costingCheck, optionalStrCd: optionalstr);

                                if (handlingWindowName == "Invoice(Vendor)")
                                {
                                    #region handlingWindowName = Invoice(Vendor)
                                    if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                    {
                                        for (int b = 0; b < ds2.Tables[0].Rows.Count; b++)
                                        {
                                            if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_CostElement_ID FROM M_CostQueue WHERE M_CostQueue_ID = " + (Util.GetValueOfInt(ds2.Tables[0].Rows[b]["M_CostQueue_ID"])))) == ceFifo)
                                            {
                                                MRPrice = MRPriceFifo;
                                                costElement = MCostElement.Get(ctx, ceFifo);
                                            }
                                            else
                                            {
                                                MRPrice = MRPriceLifo;
                                                costElement = MCostElement.Get(ctx, ceLifo);
                                            }
                                            price = MCostQueue.CalculateCostQueuePrice(invoice, invoiceline, MRPrice, acctSchema, AD_Client_ID, AD_Org_ID, costElement, trxName, client.IsCostImmediate(), optionalstr, matchInoutLine, costingCheck);
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
                                            query.Append("UPDATE m_costqueue SET CurrentCostPrice = " + price + " WHERE M_CostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[b]["M_CostQueue_ID"]));
                                            //query.Append("UPDATE m_costqueue SET CurrentCostPrice = " + Decimal.Round(Decimal.Add(price, Decimal.Divide(Decimal.Multiply(price, costElement.GetSurchargePercentage()), 100)), acctSchema.GetCostingPrecision()) + " WHERE M_CostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[b]["M_CostQueue_ID"]));
                                            DB.ExecuteQuery(query.ToString(), null, trxName);
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region Calculate amount of matched Invoice
                                    query.Clear();
                                    query.Append(@"SELECT il.c_invoiceline_id , il.c_invoice_id , ROUND(il.linenetamt/il.qtyinvoiced , 4) AS priceactual, 
                                                    CASE
                                                    WHEN NVL(C_SurChargeTax.IsIncludeInCost , 'N') = 'Y'
                                                    AND NVL(C_Tax.IsIncludeInCost , 'N')           = 'Y'
                                                    THEN ROUND((il.taxbaseamt + il.taxamt + il.surchargeamt) / il.qtyinvoiced , 4)
                                                    WHEN NVL(C_SurChargeTax.IsIncludeInCost , 'N') = 'N'
                                                    AND NVL(C_Tax.IsIncludeInCost , 'N')           = 'Y'
                                                    THEN ROUND((il.taxbaseamt + il.taxamt) / il.qtyinvoiced , 4)
                                                    WHEN NVL(C_SurChargeTax.IsIncludeInCost , 'N') = 'Y'
                                                    AND NVL(C_Tax.IsIncludeInCost , 'N')           = 'N'
                                                    THEN ROUND((il.taxbaseamt + il.surchargeamt) / il.qtyinvoiced, 4)
                                                    ELSE ROUND(il.taxbaseamt  / il.qtyinvoiced, 4)
                                                  END AS PriceActualIncludedTax
                                                    , mi.qty ,  SUM(mi.qty) AS matchedqty 
                                          FROM m_inout io INNER JOIN m_inoutline iol ON io.m_inout_id = iol.m_inout_id
                                          INNER JOIN m_matchInv mi ON iol.m_inoutline_id = mi.m_inoutline_id
                                          INNER JOIN c_invoiceline il ON il.c_invoiceline_id = mi.c_invoiceline_id
                                          INNER JOIN c_tax C_Tax ON C_Tax.C_Tax_ID = il.C_Tax_ID 
                                          LEFT JOIN C_Tax C_SurChargeTax ON C_Tax.Surcharge_Tax_ID = C_SurChargeTax.C_Tax_ID
                                          WHERE mi.isactive      = 'Y' AND io.M_InOut_ID      =" + inoutline.GetM_InOut_ID() +
                                          " AND iol.m_inoutline_ID = " + inoutline.GetM_InOutLine_ID() +
                                         @" GROUP BY  il.c_invoiceline_id , il.c_invoice_id , ROUND(il.linenetamt/il.qtyinvoiced , 4) , mi.qty , 
                                            il.taxbaseamt ,  il.taxamt , il.surchargeamt , C_SurChargeTax.IsIncludeInCost , C_Tax.IsIncludeInCost, il.qtyinvoiced ");
                                    ds2 = DB.ExecuteDataset(query.ToString(), null, trxName);
                                    if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                    {
                                        Price = 0;
                                        Qty = 0;
                                        MInvoice inv = null;
                                        Decimal PriceActualIncludedTax = 0;
                                        for (int n = 0; n < ds2.Tables[0].Rows.Count; n++)
                                        {
                                            PriceActualIncludedTax = Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["PriceActualIncludedTax"]) != 0 ?
                                            Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["PriceActualIncludedTax"]) :
                                            Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["priceactual"]);

                                            Qty += Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["matchedqty"]);
                                            inv = new MInvoice(ctx, Util.GetValueOfInt(ds2.Tables[0].Rows[n]["c_invoice_id"]), trxName);
                                            if (inv.GetC_Currency_ID() != acctSchema.GetC_Currency_ID())
                                            {
                                                Price += MConversionRate.Convert(ctx, decimal.Multiply(PriceActualIncludedTax, Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["qty"])),
                                                                                 inv.GetC_Currency_ID(), acctSchema.GetC_Currency_ID(),
                                                                                 inv.GetDateAcct(), inv.GetC_ConversionType_ID(), AD_Client_ID, AD_Org_ID);
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
                                    query.Append(@"SELECT * FROM T_Temp_CostDetail WHERE  M_InOutLine_ID = " + inoutline.GetM_InOutLine_ID() +
                                              " AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID());
                                    ds2 = DB.ExecuteDataset(query.ToString(), null, null);
                                    if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                    {
                                        matchInvoicePrice = Price;
                                        for (int k = 0; k < ds2.Tables[0].Rows.Count; k++)
                                        {
                                            Price = matchInvoicePrice;
                                            //change 4-5-2016
                                            if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_CostElement_ID FROM M_CostQueue WHERE M_CostQueue_ID = " + (Util.GetValueOfInt(ds2.Tables[0].Rows[k]["m_costqueue_id"])))) == ceFifo)
                                            {
                                                MRPrice = MRPriceFifo;
                                                costElement = MCostElement.Get(ctx, ceFifo);
                                            }
                                            else
                                            {
                                                MRPrice = MRPriceLifo;
                                                costElement = MCostElement.Get(ctx, ceLifo);
                                            }
                                            //24-Aug-2016
                                            Price = Decimal.Round(Decimal.Add(Price, Decimal.Divide(Decimal.Multiply(Price, costElement.GetSurchargePercentage()), 100)), acctSchema.GetCostingPrecision());
                                            //end
                                            Price = Decimal.Add(Price, Decimal.Multiply(MRPrice, tempQty));
                                            //end
                                            query.Clear();
                                            query.Append("UPDATE m_costqueue SET CurrentCostPrice = " + Decimal.Round((Price / Qty), acctSchema.GetCostingPrecision()) + " WHERE M_CostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[k]["M_CostQueue_ID"]));
                                            //query.Append("UPDATE m_costqueue SET CurrentCostPrice = " + Decimal.Round(Decimal.Add((Price / Qty), (((Price / Qty) * costElement.GetSurchargePercentage()) / 100)), acctSchema.GetCostingPrecision()) + " WHERE M_CostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[k]["M_CostQueue_ID"]));
                                            DB.ExecuteQuery(query.ToString(), null, trxName);
                                        }
                                    }
                                    ds2.Dispose();
                                    #endregion
                                }

                                // update m_cost with accumulation qty , amt , and current cost
                                if (isMatchFromForm == "Y" || handlingWindowName == "Match IV")
                                {
                                    result = cd.UpdateProductCost("Product Cost IV Form", cd, acctSchema, product, M_ASI_ID, invoiceline.GetAD_Org_ID(), optionalStrCd: optionalstr);
                                }
                                else
                                {
                                    result = cd.UpdateProductCost("Product Cost IV", cd, acctSchema, product, M_ASI_ID, invoiceline.GetAD_Org_ID(), costingCheck, optionalStrCd: optionalstr);
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

                                if (costingCheck != null)
                                {
                                    invoice = costingCheck.invoice;
                                }
                                if (invoice == null || invoice.Get_ID() <= 0 || invoice.Get_ID() != invoiceline.GetC_Invoice_ID())
                                {
                                    invoice = new MInvoice(ctx, invoiceline.GetC_Invoice_ID(), trxName);
                                    if (costingCheck != null && costingCheck.invoice == null)
                                    {
                                        costingCheck.invoice = invoice;
                                    }
                                }
                                if (invoice.GetC_Currency_ID() != acctSchema.GetC_Currency_ID())
                                {
                                    Price = MConversionRate.Convert(ctx, Decimal.Multiply(Qty, Decimal.Round(Decimal.Divide(ProductLineCost, invoiceline.GetQtyInvoiced()), 2)), invoice.GetC_Currency_ID(), acctSchema.GetC_Currency_ID(),
                                                                             invoice.GetDateAcct(), invoice.GetC_ConversionType_ID(), AD_Client_ID, AD_Org_ID);
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

                                cd = new MCostDetail(acctSchema, AD_Org_ID, product.GetM_Product_ID(), M_ASI_ID,
                                                         0, Price, Qty, null, trxName);
                                cd.SetC_InvoiceLine_ID(invoiceline.GetC_InvoiceLine_ID());
                                cd.SetC_OrderLine_ID(invoiceline.GetC_OrderLine_ID());
                                cd.SetM_InOutLine_ID(inoutline.GetM_InOutLine_ID());
                                cd.SetM_Warehouse_ID(inoutline.GetM_Warehouse_ID()); // get warehouse reference from header
                                if (!cd.Save(trxName))
                                {
                                    if (optionalstr != "window")
                                    {
                                        trxName.Rollback();
                                    }
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    _log.Severe("Error occured during saving a record, C_InvoiceLine_ID = " + invoiceline.GetC_InvoiceLine_ID() + " in cost detail -> Match IV. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                    return false;
                                }
                                else
                                {
                                    // reduce cummulative amount for all element
                                    result = cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, invoiceline.GetAD_Org_ID(), costingCheck, optionalStrCd: optionalstr);
                                    if (!result)
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        _log.Severe("Error occured during saving a record , C_InvoiceLine_ID = " + invoiceline.GetC_InvoiceLine_ID() + " in M-Cost -> Match IV");
                                        return false;
                                    }

                                    // update cost queue with invoice amount
                                    query.Clear();
                                    query.Append(@"SELECT * FROM T_Temp_CostDetail WHERE  M_InOutLine_ID = " + inoutline.GetM_InOutLine_ID() +
                                              " AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID());
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
                                                costElementID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_CostElement_ID FROM M_CostQueue WHERE M_CostQueue_ID = " + (Util.GetValueOfInt(ds2.Tables[0].Rows[b]["M_CostQueue_ID"]))));
                                                if (costElementID == ceFifo)
                                                {
                                                    MRPrice = MRPriceFifo;
                                                    costElement = MCostElement.Get(ctx, ceFifo);
                                                }
                                                else
                                                {
                                                    MRPrice = MRPriceLifo;
                                                    costElement = MCostElement.Get(ctx, ceLifo);
                                                }
                                                //end
                                                price = 0;
                                                price = MCostQueue.CalculateCostQueuePrice(invoice, invoiceline, MRPrice, acctSchema, AD_Client_ID, AD_Org_ID, costElement, trxName, client.IsCostImmediate(), optionalstr, matchInoutLine, costingCheck);
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
                                                query.Append("UPDATE m_costqueue SET CurrentCostPrice = " + price + " WHERE M_CostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[b]["M_CostQueue_ID"]));
                                                //query.Append("UPDATE m_costqueue SET CurrentCostPrice = " + Decimal.Round(Decimal.Add(price, Decimal.Divide(Decimal.Multiply(price, costElement.GetSurchargePercentage()), 100)), acctSchema.GetCostingPrecision()) + " WHERE M_CostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[b]["M_CostQueue_ID"]));
                                                DB.ExecuteQuery(query.ToString(), null, trxName);
                                            }
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region Calculate amount of matched Invoice
                                        query.Clear();
                                        query.Append(@"SELECT il.c_invoiceline_id , il.c_invoice_id , ROUND(il.linenetamt/il.qtyinvoiced , 4) AS priceactual , 
                                             CASE WHEN NVL(C_SurChargeTax.IsIncludeInCost , 'N') = 'Y'
                                                AND NVL(C_Tax.IsIncludeInCost , 'N')           = 'Y'
                                                THEN ROUND((il.taxbaseamt + il.taxamt + il.surchargeamt) / il.qtyinvoiced , 4)
                                                WHEN NVL(C_SurChargeTax.IsIncludeInCost , 'N') = 'N'
                                                AND NVL(C_Tax.IsIncludeInCost , 'N')           = 'Y'
                                                THEN ROUND((il.taxbaseamt + il.taxamt) / il.qtyinvoiced , 4)
                                                WHEN NVL(C_SurChargeTax.IsIncludeInCost , 'N') = 'Y'
                                                AND NVL(C_Tax.IsIncludeInCost , 'N')           = 'N'
                                                THEN ROUND((il.taxbaseamt + il.surchargeamt) / il.qtyinvoiced, 4)
                                                ELSE ROUND(il.taxbaseamt  / il.qtyinvoiced, 4)
                                              END AS PriceActualIncludedTax,    
                                            mi.qty ,  SUM(mi.qty) AS matchedqty 
                                          FROM m_inout io INNER JOIN m_inoutline iol ON io.m_inout_id = iol.m_inout_id
                                          INNER JOIN m_matchInv mi ON iol.m_inoutline_id = mi.m_inoutline_id
                                          INNER JOIN c_invoiceline il ON il.c_invoiceline_id = mi.c_invoiceline_id
                                          INNER JOIN c_tax C_Tax ON C_Tax.C_Tax_ID = il.C_Tax_ID 
                                          LEFT JOIN C_Tax C_SurChargeTax ON C_Tax.Surcharge_Tax_ID = C_SurChargeTax.C_Tax_ID
                                          WHERE mi.isactive      = 'Y' AND io.M_InOut_ID      =" + inoutline.GetM_InOut_ID() +
                                                     " AND iol.m_inoutline_ID = " + inoutline.GetM_InOutLine_ID() +
                                                      @" GROUP BY  il.c_invoiceline_id , il.c_invoice_id , ROUND(il.linenetamt/il.qtyinvoiced , 4) , mi.qty , 
                                          il.taxbaseamt ,  il.taxamt , il.surchargeamt , C_SurChargeTax.IsIncludeInCost , C_Tax.IsIncludeInCost , il.qtyinvoiced ");
                                        ds2 = DB.ExecuteDataset(query.ToString(), null, trxName);
                                        if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                        {
                                            Price = 0;
                                            Qty = 0; ;
                                            MInvoice inv = null;
                                            Decimal PriceActualIncludedTax = 0;
                                            for (int n = 0; n < ds2.Tables[0].Rows.Count; n++)
                                            {
                                                PriceActualIncludedTax = Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["PriceActualIncludedTax"]) != 0 ?
                                                    Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["PriceActualIncludedTax"]) :
                                                    Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["priceactual"]);

                                                Qty += Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["matchedqty"]);
                                                inv = new MInvoice(ctx, Util.GetValueOfInt(ds2.Tables[0].Rows[n]["c_invoice_id"]), trxName);
                                                if (inv.GetC_Currency_ID() != acctSchema.GetC_Currency_ID())
                                                {
                                                    Price += MConversionRate.Convert(ctx, decimal.Multiply(PriceActualIncludedTax, Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["qty"])),
                                                                                     inv.GetC_Currency_ID(), acctSchema.GetC_Currency_ID(),
                                                                                     inv.GetDateAcct(), inv.GetC_ConversionType_ID(), AD_Client_ID, AD_Org_ID);
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
                                        query.Append(@"SELECT * FROM T_Temp_CostDetail WHERE  M_InOutLine_ID = " + inoutline.GetM_InOutLine_ID() +
                                                  " AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID());

                                        ds2 = DB.ExecuteDataset(query.ToString(), null, null);
                                        if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                        {
                                            matchInvoicePrice = Price;
                                            for (int k = 0; k < ds2.Tables[0].Rows.Count; k++)
                                            {
                                                Price = matchInvoicePrice;
                                                //change 4-5-2016
                                                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_CostElement_ID FROM M_CostQueue WHERE M_CostQueue_ID = " + (Util.GetValueOfInt(ds2.Tables[0].Rows[k]["M_CostQueue_ID"])))) == ceFifo)
                                                {
                                                    MRPrice = MRPriceFifo;
                                                    costElement = MCostElement.Get(ctx, ceFifo);
                                                }
                                                else
                                                {
                                                    MRPrice = MRPriceLifo;
                                                    costElement = MCostElement.Get(ctx, ceLifo);
                                                }
                                                //24-Aug-2016
                                                Price = Decimal.Round(Decimal.Add(Price, Decimal.Divide(Decimal.Multiply(Price, costElement.GetSurchargePercentage()), 100)), acctSchema.GetCostingPrecision());
                                                //end
                                                Price = Decimal.Add(Price, Decimal.Multiply(MRPrice, tempQty));
                                                //end
                                                query.Clear();
                                                query.Append("UPDATE m_costqueue SET CurrentCostPrice = " + Decimal.Round(Price / Qty, acctSchema.GetCostingPrecision()) + " WHERE M_CostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[k]["M_CostQueue_ID"]));
                                                //query.Append("UPDATE m_costqueue SET CurrentCostPrice = " + Decimal.Round(Decimal.Add((Price / Qty), (((Price / Qty) * costElement.GetSurchargePercentage()) / 100)), acctSchema.GetCostingPrecision()) + " WHERE M_CostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[k]["M_CostQueue_ID"]));
                                                DB.ExecuteQuery(query.ToString(), null, trxName);
                                            }
                                        }
                                        ds2.Dispose();
                                        #endregion
                                    }

                                    // update m_cost with accumulation qty , amt , and current cost
                                    if (isMatchFromForm == "Y" || handlingWindowName == "Match IV")
                                    {
                                        if (isMatchFromForm.Equals("N") && MatchPO_OrderLineId > 0)
                                        {
                                            // check PO is matched with GRN through matching form or direct linkage on Matched PO (first order)
                                            costingCheck.isMatchFromForm = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT IsMatchPOForm FROM M_MatchPO WHERE IsActive = 'Y'
                                            AND NVL(C_InvoiceLine_ID, 0) = 0 AND M_Product_ID =" + product.GetM_Product_ID() + @"
                                            AND M_InOutLine_ID = " + inoutline.GetM_InOutLine_ID()));
                                        }
                                        else
                                        {
                                            costingCheck.isMatchFromForm = isMatchFromForm;
                                        }
                                        result = cd.UpdateProductCost("Product Cost IV Form", cd, acctSchema, product, M_ASI_ID, invoiceline.GetAD_Org_ID(), costingCheck, optionalStrCd: optionalstr);
                                    }
                                    else
                                    {
                                        result = cd.UpdateProductCost("Product Cost IV", cd, acctSchema, product, M_ASI_ID, invoiceline.GetAD_Org_ID(), costingCheck, optionalStrCd: optionalstr);
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
                            if (inoutline != null && inoutline.GetM_InOutLine_ID() > 0 && optionalstr == "process")
                            {
                                inoutline.SetIsCostCalculated(true);
                                inoutline.Save();
                            }

                            // set is cost calculated true for matched invoice
                            // after using optionalStr - this region never be called
                            if (handlingWindowName == "Match IV" && invoice != null && invoice.GetC_Invoice_ID() > 0 && optionalstr == "process")
                            {
                                invoice.SetIsCostCalculated(true);
                                if (invoice.Save())
                                {
                                    if (invoiceline != null && invoiceline.GetC_InvoiceLine_ID() > 0)
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
                            //Get Price from MCost
                            if (windowName == "Physical Inventory" || windowName == "Internal Use Inventory")
                            {
                                M_Warehouse_Id = inventoryLine.GetParent().GetM_Warehouse_ID();
                            }
                            else if (windowName == "Inventory Move")
                            {
                                M_Warehouse_Id = movementline.GetParent().GetDTD001_MWarehouseSource_ID();
                            }
                            else if (windowName == "AssetDisposal")
                            {
                                M_Warehouse_Id = Util.GetValueOfInt(po.Get_Value("M_Warehouse_ID"));
                            }
                            price = MCostQueue.CalculateCost(ctx, acctSchema, product, M_ASI_ID, AD_Client_ID, AD_Org_ID, M_Warehouse_Id);
                            cmPrice = price;
                            //price = 0;
                            //end
                            int C_Currency_ID = 0;
                            if (windowName == "Physical Inventory")
                            {
                                if (price == 0)
                                {
                                    isPriceFromProductPrice = true;
                                    if (windowName == "Inventory Move")
                                    {
                                        query.Clear();
                                        query.Append(@"(SELECT pl.AD_Org_ID , pl.name , pl.m_pricelist_id  , plv.m_pricelist_version_id , pp.pricestd , pl.c_currency_id
                                           FROM m_pricelist pl  INNER JOIN m_pricelist_version plv   ON pl.m_pricelist_id    = plv.m_pricelist_id
                                            inner join m_productprice pp   on pp.m_pricelist_version_id = plv.m_pricelist_version_id
                                           WHERE pl.AD_Org_ID = " + AD_Org_ID + " AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.isactive = 'Y' and pp.m_product_id = " + product.GetM_Product_ID() +
                                               " AND NVL(pp.m_attributesetinstance_id, 0) = " + M_ASI_ID +
                                               " AND pp.c_uom_id = " + product.GetC_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.m_pricelist_id asc");
                                    }
                                    else
                                    {
                                        query.Clear();
                                        query.Append(@"(SELECT pl.AD_Org_ID , pl.name , pl.m_pricelist_id  , plv.m_pricelist_version_id , pp.pricestd , pl.c_currency_id
                                           FROM m_pricelist pl  INNER JOIN m_pricelist_version plv   ON pl.m_pricelist_id    = plv.m_pricelist_id
                                            inner join m_productprice pp   on pp.m_pricelist_version_id = plv.m_pricelist_version_id
                                           WHERE pl.AD_Org_ID = " + inventoryLine.GetAD_Org_ID() + " AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.isactive = 'Y' and pp.m_product_id = " + product.GetM_Product_ID() +
                                                  " AND NVL(pp.m_attributesetinstance_id, 0) = " + M_ASI_ID +
                                                  " AND pp.c_uom_id = " + product.GetC_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.m_pricelist_id asc");
                                    }
                                    DataSet dsStdPrice = DB.ExecuteDataset(query.ToString(), null, null);
                                    if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                                    {
                                        price = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                                        C_Currency_ID = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["c_currency_id"]);
                                    }
                                    else
                                    {
                                        query.Clear();
                                        query.Append(@"(SELECT pl.AD_Org_ID , pl.name , pl.m_pricelist_id  , plv.m_pricelist_version_id , pp.pricestd , pl.c_currency_id
                                           FROM m_pricelist pl  INNER JOIN m_pricelist_version plv   ON pl.m_pricelist_id    = plv.m_pricelist_id
                                            inner join m_productprice pp   on pp.m_pricelist_version_id = plv.m_pricelist_version_id
                                           WHERE pl.AD_Org_ID = 0 AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.isactive = 'Y' and pp.m_product_id = " + product.GetM_Product_ID() +
                                               " AND NVL(pp.m_attributesetinstance_id, 0) = " + M_ASI_ID +
                                               " AND pp.c_uom_id = " + product.GetC_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.m_pricelist_id asc");
                                        dsStdPrice = DB.ExecuteDataset(query.ToString(), null, null);
                                        if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                                        {
                                            price = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                                            C_Currency_ID = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["c_currency_id"]);
                                        }
                                    }
                                    if (price == 0)
                                    {
                                        if (windowName == "Inventory Move")
                                        {
                                            query.Clear();
                                            query.Append(@"(SELECT pl.AD_Org_ID , pl.name , pl.m_pricelist_id  , plv.m_pricelist_version_id , pp.pricestd , pl.c_currency_id
                                           FROM m_pricelist pl  INNER JOIN m_pricelist_version plv   ON pl.m_pricelist_id    = plv.m_pricelist_id
                                            inner join m_productprice pp   on pp.m_pricelist_version_id = plv.m_pricelist_version_id
                                           WHERE pl.AD_Org_ID = " + AD_Org_ID + " AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.isactive = 'Y' and pp.m_product_id = " + product.GetM_Product_ID() +
                                               " AND pp.c_uom_id = " + product.GetC_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.m_pricelist_id asc");
                                        }
                                        else
                                        {
                                            query.Clear();
                                            query.Append(@"(SELECT pl.AD_Org_ID , pl.name , pl.m_pricelist_id  , plv.m_pricelist_version_id , pp.pricestd , pl.c_currency_id
                                           FROM m_pricelist pl  INNER JOIN m_pricelist_version plv   ON pl.m_pricelist_id    = plv.m_pricelist_id
                                            inner join m_productprice pp   on pp.m_pricelist_version_id = plv.m_pricelist_version_id
                                           WHERE pl.AD_Org_ID = " + inventoryLine.GetAD_Org_ID() + " AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.isactive = 'Y' and pp.m_product_id = " + product.GetM_Product_ID() +
                                                  " AND pp.c_uom_id = " + product.GetC_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.m_pricelist_id asc");
                                        }
                                        dsStdPrice = DB.ExecuteDataset(query.ToString(), null, null);
                                        if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                                        {
                                            price = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                                            C_Currency_ID = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["c_currency_id"]);
                                        }
                                        else
                                        {
                                            query.Clear();
                                            query.Append(@"(SELECT pl.AD_Org_ID , pl.name , pl.m_pricelist_id  , plv.m_pricelist_version_id , pp.pricestd , pl.c_currency_id
                                           FROM m_pricelist pl  INNER JOIN m_pricelist_version plv   ON pl.m_pricelist_id    = plv.m_pricelist_id
                                            inner join m_productprice pp   on pp.m_pricelist_version_id = plv.m_pricelist_version_id
                                           WHERE pl.AD_Org_ID = 0 AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.isactive = 'Y' and pp.m_product_id = " + product.GetM_Product_ID() +
                                                   " AND pp.c_uom_id = " + product.GetC_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.m_pricelist_id asc");
                                            dsStdPrice = DB.ExecuteDataset(query.ToString(), null, null);
                                            if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                                            {
                                                price = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                                                C_Currency_ID = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["c_currency_id"]);
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
                                    inventory = new MInventory(ctx, inventoryLine.GetM_Inventory_ID(), trxName);
                                    conversionNotFound = inventory.GetDocumentNo();
                                }
                                else if (windowName == "Inventory Move")
                                {
                                    movement = new MMovement(ctx, movementline.GetM_Movement_ID(), trxName);
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
                                inventory = new MInventory(ctx, inventoryLine.GetM_Inventory_ID(), trxName);

                                if (C_Currency_ID != 0 && C_Currency_ID != acctSchema.GetC_Currency_ID())
                                {
                                    Price = MConversionRate.Convert(ctx, Price, C_Currency_ID, acctSchema.GetC_Currency_ID(),
                                                                            inventory.GetMovementDate(), 0, AD_Client_ID, AD_Org_ID);
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
                            //    if (C_Currency_ID != 0 && C_Currency_ID != acctSchema.GetC_Currency_ID())
                            //    {
                            //        Price = MConversionRate.Convert(ctx, Price, C_Currency_ID, acctSchema.GetC_Currency_ID(),
                            //                                                Util.GetValueOfDateTime(po.Get_Value("DisposedDate")), 0, AD_Client_ID, AD_Org_ID);
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
                            //    movement = new MMovement(ctx, movementline.GetM_Movement_ID(), trxName);
                            //    if (C_Currency_ID != 0 && C_Currency_ID != acctSchema.GetC_Currency_ID())
                            //    {
                            //        Price = MConversionRate.Convert(ctx, Price, C_Currency_ID, acctSchema.GetC_Currency_ID(),
                            //                                                movement.GetMovementDate(), 0, AD_Client_ID, AD_Org_ID);
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
                            //M_Warehouse_Id = Util.GetValueOfInt(po.Get_Value("M_Warehouse_ID"));
                            DateTime? VAMFG_DateAcct = null;
                            DataSet dsProductDetail = DB.ExecuteDataset(@"SELECT VAMFG_DateAcct , M_Warehouse_ID FROM VAMFG_M_WrkOdrTransaction   
                                                        WHERE VAMFG_M_WrkOdrTransaction_ID = " + po.Get_ValueAsInt("VAMFG_M_WrkOdrTransaction_ID"), null, trxName);
                            if (dsProductDetail != null && dsProductDetail.Tables.Count > 0 && dsProductDetail.Tables[0].Rows.Count > 0)
                            {
                                M_Warehouse_Id = Util.GetValueOfInt(dsProductDetail.Tables[0].Rows[0]["M_Warehouse_ID"]);
                                VAMFG_DateAcct = Util.GetValueOfDateTime(dsProductDetail.Tables[0].Rows[0]["VAMFG_DateAcct"]);
                            }
                            if (windowName.Equals("PE-FinishGood"))
                            {
                                price = Price;
                                // checking price for Finished Good
                                if (acctSchema.GetC_Currency_ID() != Util.GetValueOfInt(ctx.GetContextAsInt("$C_Currency_ID")))
                                {
                                    price = MConversionRate.Convert(ctx, price, Util.GetValueOfInt(ctx.GetContext("$C_Currency_ID")), acctSchema.GetC_Currency_ID(),
                                                                        VAMFG_DateAcct, 0, AD_Client_ID, AD_Org_ID);
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
                                price = MCostQueue.CalculateCost(ctx, acctSchema, product, M_ASI_ID, AD_Client_ID, AD_Org_ID, M_Warehouse_Id);
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

                        // Get the Provisional Invoice Account Date 
                        DateTime? dateAcct = DateTime.Now;
                        if (windowName.Equals("ProvisionalInvoice"))
                        {
                            if (po.Get_ValueAsInt("M_InOutLine_ID") > 0)
                            {
                                M_Warehouse_Id = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT M_Warehouse_ID FROM M_InOut WHERE M_InOut_ID = 
                                    ( SELECT M_InOut_ID FROM M_InOutLine WHERE M_InOutLine_ID = "
                                        + po.Get_ValueAsInt("M_InOutLine_ID") + " )", null, trxName));
                            }
                            // handle conversion 
                            DataSet dsProductDetail = DB.ExecuteDataset(@"SELECT C_ProvisionalInvoice.DateAcct,
                            C_ProvisionalInvoice.C_Currency_ID, C_ProvisionalInvoice.C_ConversionType_ID 
                            FROM C_ProvisionalInvoice 
                            INNER JOIN C_ProvisionalInvoiceLine ON C_ProvisionalInvoice.C_ProvisionalInvoice_ID = C_ProvisionalInvoiceLine.C_ProvisionalInvoice_ID 
                            WHERE C_ProvisionalInvoice.C_ProvisionalInvoice_ID = " + po.Get_ValueAsInt("C_ProvisionalInvoice_ID"), null, trxName);
                            if (dsProductDetail != null && dsProductDetail.Tables.Count > 0 && dsProductDetail.Tables[0].Rows.Count > 0)
                            {
                                dateAcct = Util.GetValueOfDateTime(dsProductDetail.Tables[0].Rows[0]["DateAcct"]);
                                if (acctSchema.GetC_Currency_ID() != Util.GetValueOfInt(dsProductDetail.Tables[0].Rows[0]["C_Currency_ID"]))
                                {
                                    Price = MConversionRate.Convert(ctx, Price,
                                        Util.GetValueOfInt(dsProductDetail.Tables[0].Rows[0]["C_Currency_ID"]),
                                        acctSchema.GetC_Currency_ID(),
                                        Util.GetValueOfDateTime(dsProductDetail.Tables[0].Rows[0]["DateAcct"]),
                                        Util.GetValueOfInt(dsProductDetail.Tables[0].Rows[0]["C_ConversionType_ID"]),
                                        AD_Client_ID, AD_Org_ID);
                                }
                            }

                            if (Price == 0)
                            {
                                if (optionalstr == "process")
                                {
                                    trxName.Rollback();
                                }
                                conversionNotFound = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT DocumentNo from 
                                C_ProvisionalInvoice where C_ProvisionalInvoice_ID = " + Util.GetValueOfInt(po.Get_Value("C_ProvisionalInvoice_ID"))));
                                return false;
                            }
                        }

                        if (windowName == "Inventory Move")
                        {
                            #region Special handling for Inventory move
                            int cdId = 0;
                            // check cost detail is created on completion or not
                            if (optionalstr == "process")
                            {
                                query.Clear();
                                query.Append("SELECT M_CostDetail_ID FROM M_CostDetail WHERE IsActive = 'Y' AND M_MovementLine_ID = " + movementline.GetM_MovementLine_ID() +
                                              @" AND AD_Org_ID = " + AD_Org_ID);
                                if (costLevel == MProductCategory.COSTINGLEVEL_Warehouse || costLevel == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                {
                                    query.Append(" AND M_Warehouse_ID = " + M_Warehouse_Id);
                                }
                                cdId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));
                                cdSourceWarehouse = new MCostDetail(ctx, cdId, trxName);
                            }

                            // if cost detail not created on completion, need to create cost detail
                            if (cdId <= 0)
                            {
                                if (cmPrice > 0)
                                {
                                    cdSourceWarehouse = MCostDetail.CreateCostDetail(acctSchema, AD_Org_ID, product.GetM_Product_ID(), M_ASI_ID, windowName,
                                        inventoryLine, inoutline, movementline, invoiceline, po, 0, Decimal.Negate(cmPrice), Decimal.Negate(Qty), null, trxName, M_Warehouse_Id);
                                }
                                else
                                {
                                    cdSourceWarehouse = MCostDetail.CreateCostDetail(acctSchema, AD_Org_ID, product.GetM_Product_ID(), M_ASI_ID, windowName,
                                        inventoryLine, inoutline, movementline, invoiceline, po, 0, Decimal.Negate(Price), Decimal.Negate(Qty), null, trxName, M_Warehouse_Id);
                                }
                            }
                            SourceM_Warehouse_Id = M_Warehouse_Id; // maintain warehouse reference of Source Warehouse 
                            query.Clear();
                            query.Append("SELECT M_Warehouse_ID FROM M_Warehouse WHERE IsActive = 'Y' AND M_Warehouse_ID = (SELECT M_Warehouse_ID FROM M_Movement WHERE IsActive = 'Y' AND M_Movement_ID = " + movementline.GetM_Movement_ID() + ")");
                            M_Warehouse_Id = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));
                            if (M_Warehouse_Id == 0)
                            {
                                M_Warehouse_Id = MLocator.Get(ctx, movementline.GetM_LocatorTo_ID()).GetM_Warehouse_ID();
                            }
                            AD_Org_ID = (MWarehouse.Get(ctx, M_Warehouse_Id)).GetAD_Org_ID();
                            #endregion
                        }

                        #region Calculate expected landed cost from Provisonal Invoice
                        if (!IsPOCostingMethod && windowName == "ProvisionalInvoice" && po.Get_ValueAsInt("M_InOutLine_ID") > 0
                            && !Util.GetValueOfBool(po.Get_Value("IsCostImmediate")))
                        {
                            #region calculate expected landed cost 
                            decimal expectedAmt = 0;
                            decimal expectedQty = 0;
                            query.Clear();
                            query.Append(@"SELECT C_ExpectedCost.M_CostElement_ID , C_Expectedcostdistribution.Amt , C_Expectedcostdistribution.Qty,
                                                 C_Expectedcostdistribution.C_Expectedcostdistribution_ID , C_Order.C_Currency_ID , C_Order.C_ConversionType_ID 
                                          FROM C_Expectedcostdistribution INNER JOIN C_ExpectedCost ON C_Expectedcostdistribution.C_ExpectedCost_ID = C_ExpectedCost.C_ExpectedCost_ID 
                                          INNER JOIN C_OrderLine ON C_OrderLine.C_OrderLine_ID = C_Expectedcostdistribution.C_OrderLine_ID
                                          INNER JOIN C_Order ON C_Order.C_Order_ID = C_OrderLine.C_Order_ID 
                                          WHERE C_Expectedcostdistribution.Amt != 0 AND C_Expectedcostdistribution.C_OrderLine_ID = " +
                                          po.Get_ValueAsInt("C_OrderLine_ID"));
                            DataSet dsExpectedLandedCostAllocation = DB.ExecuteDataset(query.ToString(), null, trxName);
                            if (dsExpectedLandedCostAllocation != null && dsExpectedLandedCostAllocation.Tables.Count > 0 && dsExpectedLandedCostAllocation.Tables[0].Rows.Count > 0)
                            {
                                int OrderCurrency_ID;
                                for (int lca = 0; lca < dsExpectedLandedCostAllocation.Tables[0].Rows.Count; lca++)
                                {
                                    OrderCurrency_ID = Util.GetValueOfInt(dsExpectedLandedCostAllocation.Tables[0].Rows[lca]["C_Currency_ID"]);
                                    // total distributed amount against orderline
                                    expectedAmt = Util.GetValueOfDecimal(dsExpectedLandedCostAllocation.Tables[0].Rows[lca]["Amt"]);
                                    // Orderline qty
                                    expectedQty = Util.GetValueOfDecimal(dsExpectedLandedCostAllocation.Tables[0].Rows[lca]["Qty"]);

                                    // movement qty
                                    expectedQty = Util.GetValueOfDecimal(po.Get_Value("QtyInvoiced"));

                                    // during cost adjustment - all amount to be distributed
                                    if (!product.IsCostAdjustmentOnLost())
                                    {
                                        // distributed amount of each qty
                                        expectedAmt = Decimal.Divide(expectedAmt, Util.GetValueOfDecimal(dsExpectedLandedCostAllocation.Tables[0].Rows[lca]["Qty"]));
                                        //total amount of movement qty
                                        expectedAmt = Decimal.Multiply(expectedAmt, expectedQty);
                                    }
                                    else if (expectedQty < 0 && expectedAmt > 0)
                                    {
                                        // In case of normal loss, when qty is less than ZERO then nagate amount 
                                        //expectedAmt = Decimal.Negate(expectedAmt);
                                    }

                                    if (OrderCurrency_ID != acctSchema.GetC_Currency_ID())
                                    {
                                        expectedAmt = MConversionRate.Convert(ctx, expectedAmt, OrderCurrency_ID, acctSchema.GetC_Currency_ID(),
                                                                    dateAcct, Util.GetValueOfInt(dsExpectedLandedCostAllocation.Tables[0].Rows[lca]["C_ConversionType_ID"]),
                                                                    AD_Client_ID, AD_Org_ID);
                                        if (expectedAmt == 0)
                                        {
                                            if (optionalstr != "window")
                                            {
                                                trxName.Rollback();
                                            }
                                            conversionNotFound = Util.GetValueOfString(po.Get_Value("DocumentNo"));
                                            return false;
                                        }
                                    }

                                    // check cost detail is created on completion or not
                                    if (optionalstr == "process")
                                    {
                                        query.Clear();
                                        query.Append("SELECT M_CostDetail_ID FROM M_CostDetail WHERE IsActive = 'Y' " +
                                                     " AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                                     " AND M_CostElement_ID = " + Util.GetValueOfInt(dsExpectedLandedCostAllocation.Tables[0].Rows[lca]["M_CostElement_ID"]) +
                                                     " AND C_Orderline_ID = " + po.Get_ValueAsInt("C_OrderLine_ID") +
                                                     " AND M_InoutLine_ID = " + po.Get_ValueAsInt("M_InOutLine_ID") +
                                                     " AND M_Product_ID = " + po.Get_ValueAsInt("M_Product_ID") +
                                                     " AND  M_AttributeSetInstance_ID = " + po.Get_ValueAsInt("M_AttributeSetInstance_ID"));
                                        int cdId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));
                                        cd = new MCostDetail(ctx, cdId, trxName);
                                    }

                                    // if cost detail not created on completion, need to create cost detail
                                    if (cd == null || cd.GetM_CostDetail_ID() <= 0)
                                    {
                                        // get warehouse org -- freight record to be created in warehouse org
                                        if (M_Warehouse_Id > 0)
                                        {
                                            AD_Org_ID = MWarehouse.Get(ctx, M_Warehouse_Id).GetAD_Org_ID();
                                        }
                                        cd = MCostDetail.CreateCostDetail(acctSchema, AD_Org_ID, po.Get_ValueAsInt("M_Product_ID"),
                                            po.Get_ValueAsInt("M_AttributeSetInstance_ID"), windowName, inventoryLine, inoutline, movementline,
                                             invoiceline, po, Util.GetValueOfInt(dsExpectedLandedCostAllocation.Tables[0].Rows[lca]["M_CostElement_ID"]),
                                             Decimal.Round(expectedAmt, acctSchema.GetCostingPrecision()),
                                             expectedQty, null, trxName, M_Warehouse_Id);
                                    }
                                    if (cd != null)
                                    {
                                        // when expected landed cost calculated from invoice then set this property
                                        if (!IsPOCostingMethod)
                                        {
                                            cd.SetCalculateExpectedLandedCostFromInvoice(true);
                                        }

                                        result = cd.UpdateProductCost(windowName, cd, acctSchema, product, po.Get_ValueAsInt("M_AttributeSetInstance_ID"),
                                                                      AD_Org_ID, optionalStrCd: optionalstr);
                                        if (result)
                                        {
                                        }
                                        else
                                        {
                                            if (optionalstr != "window")
                                            {
                                                trxName.Rollback();
                                            }
                                            DB.ExecuteQuery("DELETE FROM M_CostDetail WHERE M_CostDetail_ID = " + cd.GetM_CostDetail_ID(), null, trxName);
                                            _log.Severe("Error occured during UpdateProductCost for C_Expectedcostdistribution_ID = "
                                                + Util.GetValueOfInt(dsExpectedLandedCostAllocation.Tables[0].Rows[lca]["C_Expectedcostdistribution_ID"]));
                                            return false;
                                        }

                                        // set cost detail reference as null, so that system will calculate MR costs also
                                        cd = null;
                                    }
                                }
                            }
                            #endregion
                        }
                        #endregion

                        // calculate expected landed cost  
                        if (((IsPOCostingMethod && windowName == "Material Receipt" && orderline != null && orderline.Get_ID() > 0
                            && order != null && order.Get_ID() > 0) || (!IsPOCostingMethod && windowName == "Invoice(Vendor)" && invoiceline != null && invoiceline.GetC_OrderLine_ID() > 0
                            && ((invoiceline.Get_ColumnIndex("C_ProvisionalInvoiceLine_ID") >= 0 && invoiceline.Get_ValueAsInt("C_ProvisionalInvoiceLine_ID") <= 0) ||
                                 invoiceline.Get_ColumnIndex("C_ProvisionalInvoiceLine_ID") < 0)))
                            && inoutline != null && inoutline.Get_ID() > 0 && (IsPOCostingMethod ? !inoutline.IsCostImmediate() : !invoiceline.IsCostImmediate()))
                        {
                            #region calculate expected landed cost 
                            decimal expectedAmt = 0;
                            decimal expectedQty = 0;
                            query.Clear();
                            query.Append(@"SELECT C_ExpectedCost.M_CostElement_ID , C_Expectedcostdistribution.Amt , C_Expectedcostdistribution.Qty,
                                                 C_Expectedcostdistribution.C_Expectedcostdistribution_ID , C_Order.C_Currency_ID , C_Order.C_ConversionType_ID 
                                          FROM C_Expectedcostdistribution INNER JOIN C_ExpectedCost ON C_Expectedcostdistribution.C_ExpectedCost_ID = C_ExpectedCost.C_ExpectedCost_ID 
                                          INNER JOIN C_OrderLine ON C_OrderLine.C_OrderLine_ID = C_Expectedcostdistribution.C_OrderLine_ID
                                          INNER JOIN C_Order ON C_Order.C_Order_ID = C_OrderLine.C_Order_ID 
                                          WHERE C_Expectedcostdistribution.Amt != 0 AND C_Expectedcostdistribution.C_OrderLine_ID = " +
                                          (IsPOCostingMethod ? orderline.GetC_OrderLine_ID() : invoiceline.GetC_OrderLine_ID()));
                            //if (inoutline.GetReversalDoc_ID() > 0)
                            //{
                            //    // during reversal of MR doc, system check expected landed cost is calculated ?
                            //    // if yes then need to reversed the impact
                            //    query.Append(" AND C_Expectedcostdistribution.IsCostCalculated = 'Y' ");
                            //}
                            //else
                            //{
                            //    query.Append(" AND C_Expectedcostdistribution.IsCostCalculated = 'N' ");
                            //}
                            DataSet dsExpectedLandedCostAllocation = DB.ExecuteDataset(query.ToString(), null, trxName);
                            if (dsExpectedLandedCostAllocation != null && dsExpectedLandedCostAllocation.Tables.Count > 0 && dsExpectedLandedCostAllocation.Tables[0].Rows.Count > 0)
                            {
                                int OrderCurrency_ID;
                                for (int lca = 0; lca < dsExpectedLandedCostAllocation.Tables[0].Rows.Count; lca++)
                                {
                                    OrderCurrency_ID = IsPOCostingMethod ? order.GetC_Currency_ID() : Util.GetValueOfInt(dsExpectedLandedCostAllocation.Tables[0].Rows[lca]["C_Currency_ID"]);
                                    // total distributed amount against orderline
                                    expectedAmt = Util.GetValueOfDecimal(dsExpectedLandedCostAllocation.Tables[0].Rows[lca]["Amt"]);
                                    // Orderline qty
                                    expectedQty = Util.GetValueOfDecimal(dsExpectedLandedCostAllocation.Tables[0].Rows[lca]["Qty"]);

                                    // movement qty
                                    expectedQty = inoutline.GetMovementQty();
                                    // when we reverese invoice, expected lanede cost calcualetd on invoice
                                    // then make the qty Negate
                                    if (!IsPOCostingMethod && invoiceline.GetQtyInvoiced() < 0)
                                    {
                                        expectedQty = decimal.Negate(expectedQty);
                                    }

                                    // during cost adjustment - all amount to be distributed
                                    if (!product.IsCostAdjustmentOnLost())
                                    {
                                        // distributed amount of each qty
                                        expectedAmt = Decimal.Divide(expectedAmt, Util.GetValueOfDecimal(dsExpectedLandedCostAllocation.Tables[0].Rows[lca]["Qty"]));
                                        //total amount of movement qty
                                        expectedAmt = Decimal.Multiply(expectedAmt, expectedQty);
                                    }
                                    else if (expectedQty < 0 && expectedAmt > 0)
                                    {
                                        // In case of normal loss, when qty is less than ZERO then nagate amount 
                                        //expectedAmt = Decimal.Negate(expectedAmt);
                                    }

                                    if (OrderCurrency_ID != acctSchema.GetC_Currency_ID())
                                    {
                                        expectedAmt = MConversionRate.Convert(ctx, expectedAmt, OrderCurrency_ID, acctSchema.GetC_Currency_ID(),
                                                                    IsPOCostingMethod ? inout.GetDateAcct() : invoice.GetDateAcct(), IsPOCostingMethod ? order.GetC_ConversionType_ID() :
                                                                    Util.GetValueOfInt(dsExpectedLandedCostAllocation.Tables[0].Rows[lca]["C_ConversionType_ID"]), AD_Client_ID, AD_Org_ID);
                                        if (expectedAmt == 0)
                                        {
                                            if (optionalstr != "window")
                                            {
                                                trxName.Rollback();
                                            }
                                            conversionNotFound = IsPOCostingMethod ? inout.GetDocumentNo() : invoice.GetDocumentNo();
                                            return false;
                                        }
                                    }

                                    // check cost detail is created on completion or not
                                    if (optionalstr == "process")
                                    {
                                        query.Clear();
                                        query.Append("SELECT M_CostDetail_ID FROM M_CostDetail WHERE IsActive = 'Y' " +
                                                     " AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                                     " AND M_CostElement_ID = " + Util.GetValueOfInt(dsExpectedLandedCostAllocation.Tables[0].Rows[lca]["M_CostElement_ID"]) +
                                                     " AND C_Orderline_ID = " + inoutline.GetC_OrderLine_ID() +
                                                     " AND M_InoutLine_ID = " + inoutline.GetM_InOutLine_ID() +
                                                     " AND M_Product_ID = " + inoutline.GetM_Product_ID() +
                                                     " AND  M_AttributeSetInstance_ID = " + inoutline.GetM_AttributeSetInstance_ID());
                                        int cdId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));
                                        cd = new MCostDetail(ctx, cdId, trxName);
                                    }

                                    // if cost detail not created on completion, need to create cost detail
                                    if (cd == null || cd.GetM_CostDetail_ID() <= 0)
                                    {
                                        // get warehouse org -- freight record to be created in warehouse org
                                        if (M_Warehouse_Id > 0)
                                        {
                                            AD_Org_ID = MWarehouse.Get(ctx, M_Warehouse_Id).GetAD_Org_ID();
                                        }
                                        cd = MCostDetail.CreateCostDetail(acctSchema, AD_Org_ID, inoutline.GetM_Product_ID(),
                                            inoutline.GetM_AttributeSetInstance_ID(), windowName, inventoryLine, inoutline, movementline,
                                             invoiceline, po, Util.GetValueOfInt(dsExpectedLandedCostAllocation.Tables[0].Rows[lca]["M_CostElement_ID"]),
                                             Decimal.Round(expectedAmt, acctSchema.GetCostingPrecision()),
                                             expectedQty, null, trxName, M_Warehouse_Id);
                                    }
                                    if (cd != null)
                                    {
                                        // when expected landed cost calculated from invoice then set this property
                                        if (!IsPOCostingMethod)
                                        {
                                            cd.SetCalculateExpectedLandedCostFromInvoice(true);
                                        }

                                        result = cd.UpdateProductCost(windowName, cd, acctSchema, product, inoutline.GetM_AttributeSetInstance_ID(),
                                                                      AD_Org_ID, optionalStrCd: optionalstr);
                                        if (result)
                                        {
                                            // will mark IsCostCalculated as TRUE during last accounting schema cycle
                                            //if (i == ds.Tables[0].Rows.Count - 1)
                                            //{
                                            //    DB.ExecuteQuery(@"UPDATE C_Expectedcostdistribution SET 
                                            //     IsCostCalculated = '" + (inoutline.GetReversalDoc_ID() > 0 ? 'N' : 'Y') + @"' 
                                            //    WHERE C_Expectedcostdistribution_ID = " + Util.GetValueOfInt(dsExpectedLandedCostAllocation.Tables[0].Rows[lca]["C_Expectedcostdistribution_ID"]), null, trxName);
                                            //}
                                        }
                                        else
                                        {
                                            if (optionalstr != "window")
                                            {
                                                trxName.Rollback();
                                            }
                                            DB.ExecuteQuery("DELETE FROM M_CostDetail WHERE M_CostDetail_ID = " + cd.GetM_CostDetail_ID(), null, trxName);
                                            _log.Severe("Error occured during UpdateProductCost for C_Expectedcostdistribution_ID = "
                                                + Util.GetValueOfInt(dsExpectedLandedCostAllocation.Tables[0].Rows[lca]["C_Expectedcostdistribution_ID"]));
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
                            query.Append("SELECT * FROM C_LandedCostAllocation WHERE IsCostCalculated = 'N' AND Amt != 0 AND C_InvoiceLine_ID = " + invoiceline.GetC_InvoiceLine_ID());
                            if (windowName == "LandedCost")
                            {
                                query.Append(" AND M_product_ID = " + product.GetM_Product_ID());
                            }
                            //query.Append("SELECT * FROM C_LandedCostAllocation WHERE Amt != 0 AND C_InvoiceLine_ID = " + invoiceline.GetC_InvoiceLine_ID());
                            //
                            DataSet dsLandedCostAllocation = DB.ExecuteDataset(query.ToString(), null, trxName);
                            if (dsLandedCostAllocation != null && dsLandedCostAllocation.Tables.Count > 0 && dsLandedCostAllocation.Tables[0].Rows.Count > 0)
                            {
                                invoice = new MInvoice(ctx, invoiceline.GetC_Invoice_ID(), trxName);
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
                                    if (invoice.GetC_Currency_ID() != acctSchema.GetC_Currency_ID())
                                    {
                                        amt = MConversionRate.Convert(ctx, amt, invoice.GetC_Currency_ID(), acctSchema.GetC_Currency_ID(),
                                                                             invoice.GetDateAcct(), invoice.GetC_ConversionType_ID(), AD_Client_ID, AD_Org_ID);
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
                                        query.Append("SELECT M_CostDetail_ID FROM M_CostDetail WHERE IsActive = 'Y'  AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() + " AND C_InvoiceLine_ID = " + invoiceline.GetC_InvoiceLine_ID());
                                        if (invoiceline.GetC_OrderLine_ID() > 0)
                                        {
                                            query.Append(" AND C_Orderline_ID = " + invoiceline.GetC_OrderLine_ID());
                                        }
                                        if (invoiceline.GetM_InOutLine_ID() > 0)
                                        {
                                            query.Append(" AND M_InoutLine_ID = " + invoiceline.GetM_InOutLine_ID());
                                        }
                                        query.Append(" AND M_Product_ID = " + Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["M_Product_ID"]) +
                                                     " AND  M_AttributeSetInstance_ID = " + Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["M_AttributeSetInstance_ID"]));
                                        int cdId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));
                                        cd = new MCostDetail(ctx, cdId, trxName);
                                    }

                                    // if cost detail not created on completion, need to create cost detail
                                    if (cd == null || cd.GetM_CostDetail_ID() <= 0)
                                    {
                                        M_Warehouse_Id = Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["M_Warehouse_ID"]);
                                        // get warehouse org -- freight record to be created in warehouse org
                                        if (M_Warehouse_Id > 0)
                                        {
                                            AD_Org_ID = MWarehouse.Get(ctx, M_Warehouse_Id).GetAD_Org_ID();
                                        }
                                        cd = MCostDetail.CreateCostDetail(acctSchema, AD_Org_ID, Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["M_Product_ID"]),
                                            Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["M_AttributeSetInstance_ID"]), windowName, inventoryLine, inoutline, movementline,
                                         invoiceline, po, Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["M_CostElement_ID"]), amt,
                                         qntity, null, trxName, M_Warehouse_Id);
                                    }
                                    if (cd != null && ((optionalstr == "window" && client.IsCostImmediate()) || (optionalstr == "process")))
                                    {
                                        productLca = new MProduct(ctx, Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["M_Product_ID"]), trxName);
                                        M_ASI_ID = Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["M_AttributeSetInstance_ID"]);
                                        // set dynamic property as expected landed cost 
                                        cd.SetExpectedCostCalculated(isExpectedCostCalculated);
                                        //if ((optionalstr == "window" && client.IsCostImmediate()) || (optionalstr == "process" && !client.IsCostImmediate()))
                                        //{
                                        result = cd.UpdateProductCost(windowName, cd, acctSchema, productLca, M_ASI_ID, invoiceline.GetAD_Org_ID(), costingCheck, optionalStrCd: optionalstr);
                                        if (result)
                                        {
                                            // will mark IsCostCalculated as TRUE during last accounting schema cycle
                                            if (i == ds.Tables[0].Rows.Count - 1)
                                            {
                                                DB.ExecuteQuery(@"UPDATE C_LandedCostAllocation SET IsCostCalculated = 'Y' WHERE C_LandedCostAllocation_ID = "
                                                    + Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["C_LandedCostAllocation_ID"]), null, trxName);
                                            }
                                        }
                                        else
                                        {
                                            if (optionalstr != "window")
                                            {
                                                trxName.Rollback();
                                            }
                                            DB.ExecuteQuery("DELETE FROM M_CostDetail WHERE M_CostDetail_ID = " + cd.GetM_CostDetail_ID(), null, trxName);
                                            _log.Severe("Error occured during UpdateProductCost for C_LandedCostAllocation = " + Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["C_LandedCostAllocation_ID"]));
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
                                query.Append("SELECT MIN(M_CostDetail_ID) FROM M_CostDetail WHERE NVL(M_CostElement_ID , 0) = 0 AND IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID());
                                if (windowName == "Invoice(Vendor)" || windowName == "Invoice(Customer)" || windowName == "Invoice(Vendor)-Return")
                                {
                                    // get cost detail object where cost element is null 
                                    query.Append("  AND NVL(M_CostElement_ID , 0) = 0 ");
                                    if (invoiceline.GetC_InvoiceLine_ID() > 0)
                                    {
                                        query.Append("  AND C_InvoiceLine_ID = " + invoiceline.GetC_InvoiceLine_ID());
                                    }
                                    if (invoiceline.GetC_OrderLine_ID() > 0)
                                    {
                                        query.Append(" AND C_Orderline_ID = " + invoiceline.GetC_OrderLine_ID());
                                    }
                                    if (invoiceline.GetM_InOutLine_ID() > 0)
                                    {
                                        if ((windowName == "Invoice(Vendor)" || windowName == "Invoice(Vendor)-Return") && matchInoutLine != null && matchInoutLine.GetM_InOutLine_ID() > 0)
                                        {
                                            // here inoutline contain ref from match invoice table
                                            query.Append(" AND M_InoutLine_ID = " + matchInoutLine.GetM_InOutLine_ID());
                                        }
                                        else
                                        {
                                            query.Append(" AND M_InoutLine_ID = " + invoiceline.GetM_InOutLine_ID());
                                        }
                                    }
                                    else if (invoiceline.GetC_OrderLine_ID() > 0 && windowName == "Invoice(Customer)")
                                    {
                                        query.Append(" AND M_InoutLine_ID = " + Util.GetValueOfInt(DB.ExecuteScalar("SELECT m_inoutline_id FROM m_inoutline WHERE isactive = 'Y' AND c_orderline_id = " + invoiceline.GetC_OrderLine_ID(), null, null)));
                                    }
                                }
                                else if (windowName == "Material Receipt" || windowName == "Shipment" || windowName == "Customer Return" || windowName == "Return To Vendor")
                                {
                                    if (inoutline.GetM_InOutLine_ID() > 0)
                                    {
                                        query.Append(" AND M_InoutLine_ID = " + inoutline.GetM_InOutLine_ID());
                                    }
                                    if (inoutline.GetC_OrderLine_ID() > 0)
                                    {
                                        query.Append(" AND C_Orderline_ID = " + inoutline.GetC_OrderLine_ID());
                                    }
                                    query.Append(" AND NVL(C_Invoiceline_ID , 0) = 0 AND NVL(M_CostElement_ID , 0) = 0 ");
                                }
                                else if (windowName == "Physical Inventory" || windowName == "Internal Use Inventory")
                                {
                                    query.Append(" AND M_InventoryLine_ID = " + inventoryLine.GetM_InventoryLine_ID());
                                }
                                else if (windowName == "AssetDisposal")
                                {
                                    query.Append(" AND VAFAM_AssetDisposal_ID = " + Util.GetValueOfInt(po.Get_Value("VAFAM_AssetDisposal_ID")));
                                }
                                else if (windowName == "ProvisionalInvoice")
                                {
                                    query.Append(" AND C_ProvisionalInvoiceLine_ID = " + Util.GetValueOfInt(po.Get_Value("C_ProvisionalInvoiceLine_ID")));
                                }
                                else if (windowName == "Production Execution" || windowName.Equals("PE-FinishGood"))
                                {
                                    query.Append(" AND VAMFG_M_WrkOdrTrnsctionLine_ID = " + Util.GetValueOfInt(po.Get_Value("VAMFG_M_WrkOdrTrnsctionLine_ID")));
                                }
                                else if (windowName == "Inventory Move")
                                {
                                    query.Append(" AND M_MovementLine_ID = " + movementline.GetM_MovementLine_ID() +
                                    @" AND AD_Org_ID = " + AD_Org_ID);
                                    if (costLevel == MProductCategory.COSTINGLEVEL_Warehouse || costLevel == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                    {
                                        query.Append(" AND M_Warehouse_ID = " + M_Warehouse_Id);
                                    }
                                }
                                int cdId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));
                                cd = new MCostDetail(ctx, cdId, trxName);
                            }

                            if (cd == null || cd.GetM_CostDetail_ID() <= 0)
                            {
                                if (cmPrice != 0)
                                {
                                    cd = MCostDetail.CreateCostDetail(acctSchema, AD_Org_ID, product.GetM_Product_ID(), M_ASI_ID, windowName,
                                          inventoryLine, inoutline, movementline, invoiceline, po, 0, cmPrice, Qty, null, trxName, M_Warehouse_Id);
                                }
                                else
                                {
                                    cd = MCostDetail.CreateCostDetail(acctSchema, AD_Org_ID, product.GetM_Product_ID(), M_ASI_ID, windowName,
                                         inventoryLine, inoutline, movementline, invoiceline, po, 0, Price, Qty, null, trxName, M_Warehouse_Id);
                                }
                            }
                            #endregion
                        }

                        #region Create / update "Cost Queue"
                        // Not to Create / Update / detele cost queue from process. impact comes here on Completion
                        if (cd != null && !isLandedCostAllocation &&
                            ((optionalstr != "process" && client.IsCostImmediate()) || (optionalstr == "process" && !client.IsCostImmediate())
                            || (optionalstr == "process" && client.IsCostImmediate() &&
                                inoutline != null && inoutline.GetM_InOutLine_ID() > 0 &&
                                !inoutline.IsCostImmediate()) //&& windowName == "Material Receipt" / shipment and its reverse
                             || (optionalstr == "process" && client.IsCostImmediate() &&
                                invoiceline != null && invoiceline.GetC_InvoiceLine_ID() > 0 &&
                                !invoiceline.IsCostImmediate()) //&& windowName == "Invoice(Vendor)"
                                || (optionalstr == "process" && client.IsCostImmediate() &&
                                inventoryLine != null && inventoryLine.GetM_InventoryLine_ID() > 0 &&
                                !inventoryLine.IsCostImmediate()) // window = physical inventory / internal use inventory
                                || (optionalstr == "process" && client.IsCostImmediate() &&
                                movementline != null && movementline.GetM_MovementLine_ID() > 0 &&
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
                            query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = " +
                                     " ( SELECT MMPolicy FROM M_Product_Category WHERE IsActive = 'Y' AND M_Product_Category_ID = " +
                                     " (SELECT M_Product_Category_ID FROM M_Product WHERE IsActive = 'Y' AND M_Product_ID = " + product.GetM_Product_ID() + " )) AND AD_Client_ID = " + AD_Client_ID);
                            costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                            costElement = MCostElement.Get(ctx, costingElementId);
                        backwardInOut:
                            if (windowName == "Physical Inventory" || windowName == "Internal Use Inventory")
                            {
                                #region Phy. Inventory / Internal Use Inventory
                                if (Qty > 0)
                                {
                                    if (inventoryLine.GetReversalDoc_ID() > 0)
                                    {
                                        result = MCostQueue.ReturnStockReAssignedToCostQueue(cd, windowName, costElement, Qty, true, cd.GetM_Warehouse_ID(), out backwardCompatabilitySupport);
                                        if (backwardCompatabilitySupport)
                                        {
                                            result = CreateCostQueue(ctx, acctSchema, product, M_ASI_ID, AD_Client_ID, inventoryLine.GetAD_Org_ID(), Price / Qty, Qty, windowName,
                                                  inventoryLine, inoutline, movementline, invoiceline, cd, trxName, out costQueuseIds);
                                        }
                                    }
                                    else
                                    {
                                        result = CreateCostQueue(ctx, acctSchema, product, M_ASI_ID, AD_Client_ID, inventoryLine.GetAD_Org_ID(), Price / Qty, Qty, windowName,
                                              inventoryLine, inoutline, movementline, invoiceline, cd, trxName, out costQueuseIds);
                                    }
                                    if (!result)
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        else
                                        {
                                            DB.ExecuteQuery("DELETE FROM M_CostDetail WHERE M_CostDetail_ID = " + cd.GetM_CostDetail_ID(), null, trxName);
                                            DB.ExecuteQuery("DELETE FROM M_CostQueue WHERE M_CostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                        }
                                        _log.Severe("Error occured during CreateCostQueue for M_inventory_ID = " + inventoryLine.GetM_InventoryLine_ID());
                                        return false;
                                    }

                                    if (inventoryLine.GetReversalDoc_ID() > 0 && !backwardCompatabilitySupport)
                                    {
                                        //2nd either for Fifo or lifo opposite of 1st entry
                                        query.Clear();
                                        query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = '"
                                        + (costElement.GetCostingMethod().Equals(MCostElement.COSTINGMETHOD_Fifo) ? MCostElement.COSTINGMETHOD_Lifo :
                                            MCostElement.COSTINGMETHOD_Fifo) + "' AND AD_Client_ID = " + AD_Client_ID);
                                        costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                                        costElement = new MCostElement(ctx, costingElementId, null);

                                        result = MCostQueue.ReturnStockReAssignedToCostQueue(cd, windowName, costElement, Qty, true, cd.GetM_Warehouse_ID(), out backwardCompatabilitySupport);
                                        if (!result)
                                        {
                                            if (optionalstr != "window")
                                            {
                                                trxName.Rollback();
                                            }
                                            else
                                            {
                                                DB.ExecuteQuery("DELETE FROM M_CostDetail WHERE M_CostDetail_ID = " + cd.GetM_CostDetail_ID(), null, trxName);
                                                DB.ExecuteQuery("DELETE FROM M_CostQueue WHERE M_CostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                            }
                                            _log.Severe("Error occured during CreateCostQueue for M_inventory_ID = " + inventoryLine.GetM_InventoryLine_ID());
                                            return false;
                                        }
                                    }
                                }
                                else
                                {
                                    //1st entry either of FIFO of LIFO 
                                    updateCostQueue(product, M_ASI_ID, acctSchema, inventoryLine.GetAD_Org_ID(), costElement, Decimal.Negate(Qty), M_Warehouse_Id, cd);

                                    //2nd either for Fifo or lifo opposite of 1st entry
                                    if (costElement.GetCostingMethod() == "F")
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND AD_Client_ID = " + AD_Client_ID);
                                    }
                                    else
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND AD_Client_ID = " + AD_Client_ID);
                                    }
                                    costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                                    costElement = new MCostElement(ctx, costingElementId, null);
                                    updateCostQueue(product, M_ASI_ID, acctSchema, inventoryLine.GetAD_Org_ID(), costElement, Decimal.Negate(Qty), M_Warehouse_Id, cd);
                                }
                                #endregion
                            }

                            if (windowName == "AssetDisposal")
                            {
                                #region AssetDisposal
                                if (Qty > 0)
                                {
                                    if (po.Get_ValueAsInt("ReversalDoc_ID") > 0)
                                    {
                                        result = MCostQueue.ReturnStockReAssignedToCostQueue(cd, windowName, costElement, Qty, true, cd.GetM_Warehouse_ID(), out backwardCompatabilitySupport);
                                        if (backwardCompatabilitySupport)
                                        {
                                            result = CreateCostQueue(ctx, acctSchema, product, M_ASI_ID, AD_Client_ID, Util.GetValueOfInt(po.Get_Value("AD_Org_ID")), Price / Qty, Qty, windowName,
                                             inventoryLine, inoutline, movementline, invoiceline, cd, trxName, out costQueuseIds);
                                        }
                                    }
                                    else
                                    {
                                        result = CreateCostQueue(ctx, acctSchema, product, M_ASI_ID, AD_Client_ID, Util.GetValueOfInt(po.Get_Value("AD_Org_ID")), Price / Qty, Qty, windowName,
                                          inventoryLine, inoutline, movementline, invoiceline, cd, trxName, out costQueuseIds);
                                    }
                                    if (!result)
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        else
                                        {
                                            DB.ExecuteQuery("DELETE FROM M_CostDetail WHERE M_CostDetail_ID = " + cd.GetM_CostDetail_ID(), null, trxName);
                                            DB.ExecuteQuery("DELETE FROM M_CostQueue WHERE M_CostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                        }
                                        _log.Severe("Error occured during CreateCostQueue for VAFAM_AssetDisposal_ID = " + Util.GetValueOfInt(po.Get_Value("VAFAM_AssetDisposal_ID")));
                                        return false;
                                    }

                                    if (po.Get_ValueAsInt("ReversalDoc_ID") > 0 && !backwardCompatabilitySupport)
                                    {
                                        //2nd either for Fifo or lifo opposite of 1st entry
                                        query.Clear();
                                        query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = '"
                                        + (costElement.GetCostingMethod().Equals(MCostElement.COSTINGMETHOD_Fifo) ? MCostElement.COSTINGMETHOD_Lifo :
                                            MCostElement.COSTINGMETHOD_Fifo) + "' AND AD_Client_ID = " + AD_Client_ID);
                                        costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                                        costElement = new MCostElement(ctx, costingElementId, null);

                                        result = MCostQueue.ReturnStockReAssignedToCostQueue(cd, windowName, costElement, Qty, true, cd.GetM_Warehouse_ID(), out backwardCompatabilitySupport);
                                        if (!result)
                                        {
                                            if (optionalstr != "window")
                                            {
                                                trxName.Rollback();
                                            }
                                            else
                                            {
                                                DB.ExecuteQuery("DELETE FROM M_CostDetail WHERE M_CostDetail_ID = " + cd.GetM_CostDetail_ID(), null, trxName);
                                                DB.ExecuteQuery("DELETE FROM M_CostQueue WHERE M_CostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                            }
                                            _log.Severe("Error occured during CreateCostQueue for VAFAM_AssetDisposal_ID = " + Util.GetValueOfInt(po.Get_Value("VAFAM_AssetDisposal_ID")));
                                            return false;
                                        }
                                    }
                                }
                                else
                                {
                                    //1st entry either of FIFO of LIFO 
                                    updateCostQueue(product, M_ASI_ID, acctSchema, Util.GetValueOfInt(po.Get_Value("AD_Org_ID")), costElement, Decimal.Negate(Qty), M_Warehouse_Id, cd);

                                    //2nd either for Fifo or lifo opposite of 1st entry
                                    if (costElement.GetCostingMethod() == "F")
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND AD_Client_ID = " + AD_Client_ID);
                                    }
                                    else
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND AD_Client_ID = " + AD_Client_ID);
                                    }
                                    costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                                    costElement = new MCostElement(ctx, costingElementId, null);
                                    updateCostQueue(product, M_ASI_ID, acctSchema, Util.GetValueOfInt(po.Get_Value("AD_Org_ID")), costElement, Decimal.Negate(Qty), M_Warehouse_Id, cd);
                                }
                                #endregion
                            }

                            else if (windowName == "Production Execution" || windowName.Equals("PE-FinishGood"))
                            {
                                #region Production Execution
                                if (Qty > 0)
                                {
                                    if (po.Get_ValueAsInt("ReversalDoc_ID") > 0 || po.Get_ValueAsInt("VAMFG_OrigWrkOdrTrxLine_ID_1") > 0)
                                    {
                                        result = MCostQueue.ReturnStockReAssignedToCostQueue(cd, windowName, costElement, Qty, true, cd.GetM_Warehouse_ID(), out backwardCompatabilitySupport);
                                        if (backwardCompatabilitySupport)
                                        {
                                            result = CreateCostQueue(ctx, acctSchema, product, M_ASI_ID, AD_Client_ID, Util.GetValueOfInt(po.Get_Value("AD_Org_ID")),
                                        Price / Qty, Qty, windowName, inventoryLine, inoutline, movementline, invoiceline, cd, trxName, out costQueuseIds);
                                        }
                                    }
                                    else
                                    {
                                        result = CreateCostQueue(ctx, acctSchema, product, M_ASI_ID, AD_Client_ID, Util.GetValueOfInt(po.Get_Value("AD_Org_ID")),
                                        Price / Qty, Qty, windowName, inventoryLine, inoutline, movementline, invoiceline, cd, trxName, out costQueuseIds);
                                    }
                                    if (!result)
                                    {
                                        if (optionalstr == "process")
                                        {
                                            trxName.Rollback();
                                        }
                                        else
                                        {
                                            DB.ExecuteQuery("DELETE FROM M_CostDetail WHERE M_CostDetail_ID = " + cd.GetM_CostDetail_ID(), null, trxName);
                                            DB.ExecuteQuery("DELETE FROM M_CostQueue WHERE M_CostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                        }
                                        _log.Severe("Error occured during CreateCostQueue for Production Execution line = " + Util.GetValueOfInt(po.Get_Value("VAMFG_M_WrkOdrTrnsctionLine_ID")));
                                        return false;
                                    }

                                    if ((po.Get_ValueAsInt("ReversalDoc_ID") > 0 || po.Get_ValueAsInt("VAMFG_OrigWrkOdrTrxLine_ID_1") > 0) && !backwardCompatabilitySupport)
                                    {
                                        //2nd either for Fifo or lifo opposite of 1st entry
                                        query.Clear();
                                        query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = '"
                                        + (costElement.GetCostingMethod().Equals(MCostElement.COSTINGMETHOD_Fifo) ? MCostElement.COSTINGMETHOD_Lifo :
                                            MCostElement.COSTINGMETHOD_Fifo) + "' AND AD_Client_ID = " + AD_Client_ID);
                                        costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                                        costElement = new MCostElement(ctx, costingElementId, null);

                                        result = MCostQueue.ReturnStockReAssignedToCostQueue(cd, windowName, costElement, Qty, true, cd.GetM_Warehouse_ID(), out backwardCompatabilitySupport);
                                        if (!result)
                                        {
                                            if (optionalstr == "process")
                                            {
                                                trxName.Rollback();
                                            }
                                            else
                                            {
                                                DB.ExecuteQuery("DELETE FROM M_CostDetail WHERE M_CostDetail_ID = " + cd.GetM_CostDetail_ID(), null, trxName);
                                            }
                                            _log.Severe("Error occured during CreateCostQueue for Production Execution line = " + Util.GetValueOfInt(po.Get_Value("VAMFG_M_WrkOdrTrnsctionLine_ID")));
                                            return false;
                                        }
                                    }
                                }
                                else
                                {
                                backwardSupportPE:
                                    //1st entry either of FIFO of LIFO 
                                    if (po.Get_ValueAsInt("ReversalDoc_ID") > 0 && !backwardCompatabilitySupport)
                                    {
                                        result = MCostQueue.ReturnStockReduceFromCostQueue(cd, windowName, costElement, Decimal.Negate(Qty),
                                                                                  true, cd.GetM_Warehouse_ID(), out backwardCompatabilitySupport);
                                        if (backwardCompatabilitySupport)
                                        {
                                            goto backwardSupportPE;
                                        }
                                        if (!result)
                                        {
                                            if (optionalstr != "window")
                                            {
                                                trxName.Rollback();
                                            }
                                            else
                                            {
                                                DB.ExecuteQuery("DELETE FROM M_CostDetail WHERE M_CostDetail_ID = " + cd.GetM_CostDetail_ID(), null, trxName);
                                            }
                                            _log.Severe("Error occured during ReturnStockReduceFromCostQueue for Production Execution line = " + Util.GetValueOfInt(po.Get_Value("VAMFG_M_WrkOdrTrnsctionLine_ID")));
                                            return false;
                                        }

                                        //2nd either for Fifo or lifo opposite of 1st entry
                                        query.Clear();
                                        query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = '"
                                        + (costElement.GetCostingMethod().Equals(MCostElement.COSTINGMETHOD_Fifo) ? MCostElement.COSTINGMETHOD_Lifo :
                                            MCostElement.COSTINGMETHOD_Fifo) + "' AND AD_Client_ID = " + AD_Client_ID);
                                        costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                                        costElement = new MCostElement(ctx, costingElementId, null);

                                        result = MCostQueue.ReturnStockReduceFromCostQueue(cd, windowName, costElement, Decimal.Negate(Qty),
                                                                                   true, cd.GetM_Warehouse_ID(), out backwardCompatabilitySupport);
                                        if (!result)
                                        {
                                            if (optionalstr != "window")
                                            {
                                                trxName.Rollback();
                                            }
                                            else
                                            {
                                                DB.ExecuteQuery("DELETE FROM M_CostDetail WHERE M_CostDetail_ID = " + cd.GetM_CostDetail_ID(), null, trxName);
                                            }
                                            _log.Severe("Error occured during ReturnStockReduceFromCostQueue for Production Execution line = " + Util.GetValueOfInt(po.Get_Value("VAMFG_M_WrkOdrTrnsctionLine_ID")));
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        //1st entry either of FIFO of LIFO 
                                        updateCostQueue(product, M_ASI_ID, acctSchema, Util.GetValueOfInt(po.Get_Value("AD_Org_ID")), costElement, Decimal.Negate(Qty), M_Warehouse_Id, cd);

                                        //2nd either for Fifo or lifo opposite of 1st entry
                                        if (costElement.GetCostingMethod() == "F")
                                        {
                                            query.Clear();
                                            query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND AD_Client_ID = " + AD_Client_ID);
                                        }
                                        else
                                        {
                                            query.Clear();
                                            query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND AD_Client_ID = " + AD_Client_ID);
                                        }
                                        costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                                        costElement = new MCostElement(ctx, costingElementId, null);
                                        updateCostQueue(product, M_ASI_ID, acctSchema, Util.GetValueOfInt(po.Get_Value("AD_Org_ID")), costElement, Decimal.Negate(Qty), M_Warehouse_Id, cd);
                                    }
                                }
                                #endregion
                            }

                            else if (inoutline != null && ((inoutline.GetReversalDoc_ID() == 0 &&
                                ((windowName.Equals("Material Receipt") || windowName.Equals("Shipment")) ||
                                (inoutline.GetC_OrderLine_ID() == 0 && windowName.Equals("Return To Vendor")))) || backwardCompatabilitySupport)) // windowName == "Customer Return"
                            {
                                #region Sales / Purchase / Return
                                if (Qty > 0)
                                {
                                    result = CreateCostQueue(ctx, acctSchema, product, M_ASI_ID, AD_Client_ID, inoutline.GetAD_Org_ID(), Price / Qty, Qty, windowName,
                                         inventoryLine, inoutline, movementline, invoiceline, cd, trxName, out costQueuseIds);
                                    if (!result)
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        else
                                        {
                                            DB.ExecuteQuery("DELETE FROM M_CostDetail WHERE M_CostDetail_ID = " + cd.GetM_CostDetail_ID(), null, trxName);
                                            DB.ExecuteQuery("DELETE FROM M_CostQueue WHERE M_CostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                        }
                                        _log.Severe("Error occured during CreateCostQueue for M_Inout_ID = " + inoutline.GetM_InOutLine_ID());
                                        return false;
                                    }
                                }
                                else
                                {
                                    //1st entry either of FIFO of LIFO 
                                    updateCostQueue(product, M_ASI_ID, acctSchema, inoutline.GetAD_Org_ID(), costElement, Decimal.Negate(Qty), M_Warehouse_Id, cd);

                                    //2nd either for Fifo or lifo opposite of 1st entry
                                    if (costElement.GetCostingMethod() == "F")
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND AD_Client_ID = " + AD_Client_ID);
                                    }
                                    else
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND AD_Client_ID = " + AD_Client_ID);
                                    }
                                    costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                                    costElement = MCostElement.Get(ctx, costingElementId);
                                    updateCostQueue(product, M_ASI_ID, acctSchema, inoutline.GetAD_Org_ID(), costElement, Decimal.Negate(Qty), M_Warehouse_Id, cd);
                                }
                                #endregion
                            }

                            else if (!backwardCompatabilitySupport && (windowName == "Customer Return" ||
                                windowName == "Return To Vendor" ||
                                (inoutline != null && inoutline.GetReversalDoc_ID() != 0 &&
                                (windowName.Equals("Material Receipt") || windowName.Equals("Shipment")))))
                            {
                                #region  Returns
                                if (Qty > 0)
                                {
                                    result = MCostQueue.ReturnStockReAssignedToCostQueue(cd, windowName, costElement, Qty,
                                             inoutline.GetReversalDoc_ID() != 0 ? true : false, cd.GetM_Warehouse_ID(), out backwardCompatabilitySupport);
                                    if (backwardCompatabilitySupport)
                                    {
                                        goto backwardInOut;
                                    }
                                    if (!result)
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        else
                                        {
                                            DB.ExecuteQuery("DELETE FROM M_CostDetail WHERE M_CostDetail_ID = " + cd.GetM_CostDetail_ID(), null, trxName);
                                            DB.ExecuteQuery("DELETE FROM M_CostQueue WHERE M_CostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                        }
                                        _log.Severe("Error occured during CreateCostQueue for M_Inout_ID = " + inoutline.GetM_InOutLine_ID());
                                        return false;
                                    }

                                    // 2nd either for Fifo or lifo opposite of 1st entry
                                    query.Clear();
                                    query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = '"
                                    + (costElement.GetCostingMethod().Equals(MCostElement.COSTINGMETHOD_Fifo) ? MCostElement.COSTINGMETHOD_Lifo :
                                        MCostElement.COSTINGMETHOD_Fifo) + "' AND AD_Client_ID = " + AD_Client_ID);
                                    costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                                    costElement = MCostElement.Get(ctx, costingElementId);

                                    result = MCostQueue.ReturnStockReAssignedToCostQueue(cd, windowName, costElement, Qty,
                                                                                        inoutline.GetReversalDoc_ID() != 0 ? true : false, cd.GetM_Warehouse_ID(), out backwardCompatabilitySupport);
                                    if (!result)
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        else
                                        {
                                            DB.ExecuteQuery("DELETE FROM M_CostDetail WHERE M_CostDetail_ID = " + cd.GetM_CostDetail_ID(), null, trxName);
                                            DB.ExecuteQuery("DELETE FROM M_CostQueue WHERE M_CostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                        }
                                        _log.Severe("Error occured during CreateCostQueue for M_Inout_ID = " + inoutline.GetM_InOutLine_ID());
                                        return false;
                                    }
                                }
                                else
                                {
                                    //1st entry either of FIFO of LIFO 
                                    result = MCostQueue.ReturnStockReduceFromCostQueue(cd, windowName, costElement, Decimal.Negate(Qty),
                                                                                        inoutline.GetReversalDoc_ID() != 0 ? true : false, cd.GetM_Warehouse_ID(), out backwardCompatabilitySupport);
                                    if (backwardCompatabilitySupport)
                                    {
                                        goto backwardInOut;
                                    }
                                    if (!result)
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        else
                                        {
                                            DB.ExecuteQuery("DELETE FROM M_CostDetail WHERE M_CostDetail_ID = " + cd.GetM_CostDetail_ID(), null, trxName);
                                        }
                                        _log.Severe("Error occured during ReturnStockReduceFromCostQueue for M_Inout_ID = " + inoutline.GetM_InOutLine_ID());
                                        return false;
                                    }

                                    //2nd either for Fifo or lifo opposite of 1st entry
                                    query.Clear();
                                    query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = '"
                                    + (costElement.GetCostingMethod().Equals(MCostElement.COSTINGMETHOD_Fifo) ? MCostElement.COSTINGMETHOD_Lifo :
                                        MCostElement.COSTINGMETHOD_Fifo) + "' AND AD_Client_ID = " + AD_Client_ID);
                                    costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                                    costElement = MCostElement.Get(ctx, costingElementId);

                                    result = MCostQueue.ReturnStockReduceFromCostQueue(cd, windowName, costElement, Decimal.Negate(Qty),
                                                                                        inoutline.GetReversalDoc_ID() != 0 ? true : false, cd.GetM_Warehouse_ID(), out backwardCompatabilitySupport);
                                    if (!result)
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        else
                                        {
                                            DB.ExecuteQuery("DELETE FROM M_CostDetail WHERE M_CostDetail_ID = " + cd.GetM_CostDetail_ID(), null, trxName);
                                        }
                                        _log.Severe("Error occured during ReturnStockReduceFromCostQueue for M_Inout_ID = " + inoutline.GetM_InOutLine_ID());
                                        return false;
                                    }
                                }
                                #endregion
                            }

                            else if (windowName == "Inventory Move")
                            {
                                #region Inventory Move
                                if (Qty > 0)
                                {
                                    if (movementline.GetReversalDoc_ID() > 0)
                                    {
                                        result = MCostQueue.ReturnStockReAssignedToCostQueue(cd, windowName, costElement, Qty, true, cd.GetM_Warehouse_ID(), out backwardCompatabilitySupport);
                                        if (backwardCompatabilitySupport)
                                        {
                                            result = CreateCostQueue(ctx, acctSchema, product, M_ASI_ID, AD_Client_ID, AD_Org_ID, Price / Qty, Qty, windowName,
                                             inventoryLine, inoutline, movementline, invoiceline, cd, trxName, out costQueuseIds);
                                            backwardCompatabilitySupport = false;
                                        }
                                    }
                                    else
                                    {
                                        result = CreateCostQueue(ctx, acctSchema, product, M_ASI_ID, AD_Client_ID, AD_Org_ID, Price / Qty, Qty, windowName,
                                         inventoryLine, inoutline, movementline, invoiceline, cd, trxName, out costQueuseIds);
                                    }
                                    if (!result)
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        else
                                        {
                                            DB.ExecuteQuery("DELETE FROM M_CostDetail WHERE M_CostDetail_ID = " + cd.GetM_CostDetail_ID(), null, trxName);
                                            DB.ExecuteQuery("DELETE FROM M_CostQueue WHERE M_CostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                        }
                                        _log.Severe("Error occured during CreateCostQueue for m_MovementLime_ID = " + movementline.GetM_MovementLine_ID());
                                        return false;
                                    }

                                    //1st entry either of FIFO of LIFO 
                                    if (movementline.GetReversalDoc_ID() > 0)
                                    {
                                        result = MCostQueue.ReturnStockReduceFromCostQueue(cd, windowName, costElement, Qty, true, SourceM_Warehouse_Id, out backwardCompatabilitySupport);
                                        if (backwardCompatabilitySupport)
                                        {
                                            updateCostQueue(product, M_ASI_ID, acctSchema, movementline.GetAD_Org_ID(), costElement, Qty, SourceM_Warehouse_Id, cd);
                                        }
                                    }
                                    else
                                    {
                                        updateCostQueue(product, M_ASI_ID, acctSchema, movementline.GetAD_Org_ID(), costElement, Qty, SourceM_Warehouse_Id, cd);
                                    }

                                    //2nd either for Fifo or lifo opposite of 1st entry
                                    if (costElement.GetCostingMethod() == "F")
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND AD_Client_ID = " + AD_Client_ID);
                                    }
                                    else
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND AD_Client_ID = " + AD_Client_ID);
                                    }
                                    costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                                    costElement = MCostElement.Get(ctx, costingElementId);

                                    if (movementline.GetReversalDoc_ID() > 0)
                                    {
                                        result = MCostQueue.ReturnStockReduceFromCostQueue(cd, windowName, costElement, Qty, true, SourceM_Warehouse_Id, out backwardCompatabilitySupport);
                                        if (backwardCompatabilitySupport)
                                        {
                                            updateCostQueue(product, M_ASI_ID, acctSchema, movementline.GetAD_Org_ID(), costElement, Qty, SourceM_Warehouse_Id, cd);
                                        }

                                        result = MCostQueue.ReturnStockReAssignedToCostQueue(cd, windowName, costElement, Qty, true, cd.GetM_Warehouse_ID(), out backwardCompatabilitySupport);
                                        if (!result)
                                        {
                                            if (optionalstr != "window")
                                            {
                                                trxName.Rollback();
                                            }
                                            else
                                            {
                                                DB.ExecuteQuery("DELETE FROM M_CostDetail WHERE M_CostDetail_ID = " + cd.GetM_CostDetail_ID(), null, trxName);
                                                DB.ExecuteQuery("DELETE FROM M_CostQueue WHERE M_CostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                            }
                                            _log.Severe("Error occured during CreateCostQueue for m_MovementLime_ID = " + movementline.GetM_MovementLine_ID());
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        updateCostQueue(product, M_ASI_ID, acctSchema, movementline.GetAD_Org_ID(), costElement, Qty, SourceM_Warehouse_Id, cd);
                                    }
                                }
                                else
                                {
                                    // change refsrence from cd to cdSourceWarehouse -- bcz we are incresing stock in "Source Warehouse"
                                    if (movementline.GetReversalDoc_ID() > 0)
                                    {
                                        result = MCostQueue.ReturnStockReAssignedToCostQueue(cd, windowName, costElement, Decimal.Negate(Qty), true, cdSourceWarehouse.GetM_Warehouse_ID(), out backwardCompatabilitySupport);
                                        if (backwardCompatabilitySupport)
                                        {
                                            result = CreateCostQueue(ctx, acctSchema, product, M_ASI_ID, AD_Client_ID, movementline.GetAD_Org_ID(), Price / Qty, Decimal.Negate(Qty), windowName,
                                           inventoryLine, inoutline, movementline, invoiceline, cdSourceWarehouse, trxName, out costQueuseIds);
                                        }
                                    }
                                    else
                                    {
                                        result = CreateCostQueue(ctx, acctSchema, product, M_ASI_ID, AD_Client_ID, movementline.GetAD_Org_ID(), Price / Qty, Decimal.Negate(Qty), windowName,
                                        inventoryLine, inoutline, movementline, invoiceline, cdSourceWarehouse, trxName, out costQueuseIds);
                                    }
                                    if (!result)
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        else
                                        {
                                            DB.ExecuteQuery("DELETE FROM M_CostDetail WHERE M_CostDetail_ID = " + cd.GetM_CostDetail_ID(), null, trxName);
                                            DB.ExecuteQuery("DELETE FROM M_CostQueue WHERE M_CostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                        }
                                        _log.Severe("Error occured during CreateCostQueue for M_MovementLine_ID = " + movementline.GetM_MovementLine_ID());
                                        return false;
                                    }

                                    // M_Warehouse_Id -- reduce stock from "To Warehouse"
                                    //1st entry either of FIFO of LIFO   
                                    if (movementline.GetReversalDoc_ID() > 0)
                                    {
                                        result = MCostQueue.ReturnStockReduceFromCostQueue(cd, windowName, costElement, Decimal.Negate(Qty), true, M_Warehouse_Id, out backwardCompatabilitySupport);
                                        if (backwardCompatabilitySupport)
                                        {
                                            updateCostQueue(product, M_ASI_ID, acctSchema, AD_Org_ID, costElement, Decimal.Negate(Qty), M_Warehouse_Id, cd);
                                        }
                                    }
                                    else
                                    {
                                        updateCostQueue(product, M_ASI_ID, acctSchema, AD_Org_ID, costElement, Decimal.Negate(Qty), M_Warehouse_Id, cd);
                                    }

                                    //2nd either for Fifo or lifo opposite of 1st entry
                                    if (costElement.GetCostingMethod() == "F")
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND AD_Client_ID = " + AD_Client_ID);
                                    }
                                    else
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND AD_Client_ID = " + AD_Client_ID);
                                    }
                                    costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                                    costElement = MCostElement.Get(ctx, costingElementId);

                                    if (movementline.GetReversalDoc_ID() > 0)
                                    {
                                        result = MCostQueue.ReturnStockReduceFromCostQueue(cd, windowName, costElement, Decimal.Negate(Qty), true, M_Warehouse_Id, out backwardCompatabilitySupport);
                                        if (backwardCompatabilitySupport)
                                        {
                                            updateCostQueue(product, M_ASI_ID, acctSchema, AD_Org_ID, costElement, Decimal.Negate(Qty), M_Warehouse_Id, cd);
                                        }

                                        result = MCostQueue.ReturnStockReAssignedToCostQueue(cd, windowName, costElement, Decimal.Negate(Qty), true, cdSourceWarehouse.GetM_Warehouse_ID(), out backwardCompatabilitySupport);
                                        if (!result)
                                        {
                                            if (optionalstr != "window")
                                            {
                                                trxName.Rollback();
                                            }
                                            else
                                            {
                                                DB.ExecuteQuery("DELETE FROM M_CostDetail WHERE M_CostDetail_ID = " + cd.GetM_CostDetail_ID(), null, trxName);
                                                DB.ExecuteQuery("DELETE FROM M_CostQueue WHERE M_CostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                            }
                                            _log.Severe("Error occured during CreateCostQueue for M_MovementLine_ID = " + movementline.GetM_MovementLine_ID());
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        updateCostQueue(product, M_ASI_ID, acctSchema, AD_Org_ID, costElement, Decimal.Negate(Qty), M_Warehouse_Id, cd);
                                    }
                                }
                                #endregion
                            }

                            else if (windowName == "Invoice(Vendor)")
                            {
                                #region update cost on cost Queue / M_Cost
                                if (invoiceline.GetC_OrderLine_ID() > 0 && matchInoutLine != null && matchInoutLine.GetM_InOutLine_ID() > 0)
                                {
                                    query.Clear();
                                    query.Append(@"SELECT * FROM T_Temp_CostDetail WHERE C_OrderLine_ID = " + invoiceline.GetC_OrderLine_ID() +
                                        " AND M_InOutLine_ID = " + matchInoutLine.GetM_InOutLine_ID() + " AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID());
                                    DataSet ds1 = DB.ExecuteDataset(query.ToString(), null, trxName);
                                    if (ds1 != null && ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                                    {
                                        for (int k = 0; k < ds1.Tables[0].Rows.Count; k++)
                                        {
                                            // change 9-5-2016
                                            // handle partial payment
                                            X_T_Temp_CostDetail tempCostDetail = new X_T_Temp_CostDetail(invoiceline.GetCtx(), ds1.Tables[0].Rows[k], trxName);
                                            query.Clear();
                                            query.Append("SELECT M_CostElement_ID FROM M_CostQueue WHERE M_CostQueue_ID = " + Util.GetValueOfInt(ds1.Tables[0].Rows[k]["M_CostQueue_ID"]));
                                            costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));
                                            costElement = MCostElement.Get(ctx, costingElementId);

                                            price = MCostQueue.CalculateCostQueuePrice(invoice, invoiceline, tempCostDetail.GetAmt(), acctSchema, AD_Client_ID, AD_Org_ID, costElement, trxName, client.IsCostImmediate(), optionalstr, matchInoutLine, costingCheck);

                                            query.Clear();
                                            query.Append("UPDATE m_costqueue SET CurrentCostPrice = " + price + " WHERE M_CostQueue_ID = " + Util.GetValueOfInt(ds1.Tables[0].Rows[k]["M_CostQueue_ID"]));
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
                                result = cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, inventoryLine.GetAD_Org_ID(), costingCheck, optionalStrCd: optionalstr);
                                if (!result)
                                {
                                    if (optionalstr != "window")
                                    {
                                        trxName.Rollback();
                                    }
                                    else
                                    {
                                        DB.ExecuteQuery("DELETE FROM M_CostDetail WHERE M_CostDetail_ID = " + cd.GetM_CostDetail_ID(), null, trxName);
                                        DB.ExecuteQuery("DELETE FROM M_CostQueue WHERE M_CostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                    }
                                    _log.Severe("Error occured during UpdateProductCost for M_InventoryLine_ID = " + inventoryLine.GetM_InventoryLine_ID());
                                    return false;
                                }
                            }
                            else if (windowName == "AssetDisposal")
                            {
                                result = cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, Util.GetValueOfInt(po.Get_Value("AD_Org_ID")), costingCheck, optionalStrCd: optionalstr);
                                if (!result)
                                {
                                    if (optionalstr != "window")
                                    {
                                        trxName.Rollback();
                                    }
                                    else
                                    {
                                        DB.ExecuteQuery("DELETE FROM M_CostDetail WHERE M_CostDetail_ID = " + cd.GetM_CostDetail_ID(), null, trxName);
                                        DB.ExecuteQuery("DELETE FROM M_CostQueue WHERE M_CostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                    }
                                    _log.Severe("Error occured during UpdateProductCost for VAFAM_AssetDisposal_ID = " + Util.GetValueOfInt(po.Get_Value("VAFAM_AssetDisposal_ID")));
                                    return false;
                                }
                            }
                            else if (windowName == "Production Execution" || windowName.Equals("PE-FinishGood"))
                            {
                                result = cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, Util.GetValueOfInt(po.Get_Value("AD_Org_ID")), costingCheck, optionalStrCd: optionalstr);
                                if (!result)
                                {
                                    if (optionalstr == "process")
                                    {
                                        trxName.Rollback();
                                    }
                                    else
                                    {
                                        DB.ExecuteQuery("DELETE FROM M_CostDetail WHERE M_CostDetail_ID = " + cd.GetM_CostDetail_ID(), null, trxName);
                                        DB.ExecuteQuery("DELETE FROM M_CostQueue WHERE M_CostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                    }
                                    _log.Severe("Error occured during UpdateProductCost for M_InventoryLine_ID = " + inventoryLine.GetM_InventoryLine_ID());
                                    return false;
                                }
                            }
                            else if (windowName == "Material Receipt" || windowName == "Customer Return" || windowName == "Shipment" || windowName == "Return To Vendor")
                            {
                                result = cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, inoutline.GetAD_Org_ID(), costingCheck, optionalStrCd: optionalstr);
                                if (!result)
                                {
                                    if (optionalstr != "window")
                                    {
                                        trxName.Rollback();
                                    }
                                    else
                                    {
                                        DB.ExecuteQuery("DELETE FROM M_CostDetail WHERE M_CostDetail_ID = " + cd.GetM_CostDetail_ID(), null, trxName);
                                        DB.ExecuteQuery("DELETE FROM M_CostQueue WHERE M_CostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                    }
                                    _log.Severe("Error occured during UpdateProductCost for M_InoutLine_ID = " + inoutline.GetM_InOutLine_ID());
                                    return false;
                                }
                            }
                            else if (windowName == "Inventory Move")
                            {
                                if (pca != null)
                                {
                                    if (pca.GetCostingLevel() == "C" || pca.GetCostingLevel() == "B")
                                    {
                                        AD_Org_ID = 0;
                                    }
                                    else if ((string.IsNullOrEmpty(pca.GetCostingLevel())) && (acctSchema.GetCostingLevel() == "C" || acctSchema.GetCostingLevel() == "B"))
                                    {
                                        AD_Org_ID = 0;
                                    }
                                }
                                else if (acctSchema.GetCostingLevel() == "C" || acctSchema.GetCostingLevel() == "B")
                                {
                                    AD_Org_ID = 0;
                                }
                                if (AD_Org_ID != 0)
                                {
                                    result = cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, AD_Org_ID, costingCheck, optionalStrCd: optionalstr); // for destination warehouse
                                    if (!result)
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        _log.Severe("Error occured during UpdateProductCost for M_movementLine_ID = " + cd.GetM_MovementLine_ID());
                                        return false;
                                    }

                                    result = cdSourceWarehouse.UpdateProductCost(windowName, cdSourceWarehouse, acctSchema, product, M_ASI_ID, movementline.GetAD_Org_ID(), costingCheck, optionalStrCd: optionalstr); // for source warehouse org
                                    if (!result)
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        else
                                        {
                                            DB.ExecuteQuery("DELETE FROM M_CostDetail WHERE M_CostDetail_ID = " + cd.GetM_CostDetail_ID(), null, trxName);
                                            DB.ExecuteQuery("DELETE FROM M_CostQueue WHERE M_CostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                        }
                                        _log.Severe("Error occured during UpdateProductCost fot M_MovementLine_ID = " + cd.GetM_MovementLine_ID());
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
                                if ((windowName != "Invoice(Vendor)-Return" && invoiceline.GetC_OrderLine_ID() > 0) || (windowName == "Invoice(Vendor)-Return" && invoiceline.GetM_InOutLine_ID() > 0))
                                {
                                    result = cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, invoiceline.GetAD_Org_ID(), costingCheck, optionalStrCd: optionalstr);
                                    if (!result)
                                    {
                                        if (optionalstr != "window")
                                        {
                                            trxName.Rollback();
                                        }
                                        else
                                        {
                                            DB.ExecuteQuery("DELETE FROM M_CostDetail WHERE M_CostDetail_ID = " + cd.GetM_CostDetail_ID(), null, trxName);
                                            if (!string.IsNullOrEmpty(costQueuseIds))
                                            {
                                                DB.ExecuteQuery("DELETE FROM M_CostQueue WHERE M_CostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                            }
                                        }
                                        _log.Severe("Error occured during UpdateProductCost for C_invoiceLine_ID = " + invoiceline.GetC_InvoiceLine_ID());
                                        return false;
                                    }
                                }
                                else if (invoiceline.GetC_OrderLine_ID() == 0 && invoiceline.GetM_InOutLine_ID() == 0)
                                {
                                    // get costing level
                                    if (String.IsNullOrEmpty(costLevel))
                                    {
                                        costLevel = (MProductCategory.Get(product.GetCtx(), product.GetM_Product_Category_ID())).GetCostingLevel();
                                        if (String.IsNullOrEmpty(costLevel))
                                        {
                                            costLevel = acctSchema.GetCostingLevel();
                                        }
                                    }
                                    // when costing level = warehouse or Warehouse + batch and warehouse is not defined on Invoice/line then not to calculate cost against indepenedent APC
                                    if (!((costLevel == MProductCategory.COSTINGLEVEL_Warehouse || costLevel == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                        && M_Warehouse_Id <= 0))
                                    {
                                        // in case of independent AP credit memo, accumulation amt reduce and current cost of AV. PO/Invoice will be calculated
                                        // discount is given only when document type having setting as "Treat As Discount" = True
                                        MDocType docType = MDocType.Get(ctx, invoice.GetC_DocTypeTarget_ID());
                                        if (docType.GetDocBaseType() == "APC" && docType.IsTreatAsDiscount())
                                        {
                                            result = cd.UpdateProductCost("Invoice(APC)", cd, acctSchema, product, M_ASI_ID, invoiceline.GetAD_Org_ID(), costingCheck, optionalStrCd: optionalstr);
                                            if (!result)
                                            {
                                                if (optionalstr != "window")
                                                {
                                                    trxName.Rollback();
                                                }
                                                else
                                                {
                                                    DB.ExecuteQuery("DELETE FROM M_CostDetail WHERE M_CostDetail_ID = " + cd.GetM_CostDetail_ID(), null, trxName);
                                                    if (!string.IsNullOrEmpty(costQueuseIds))
                                                    {
                                                        DB.ExecuteQuery("DELETE FROM M_CostQueue WHERE M_CostQueue_ID IN ( " + costQueuseIds + " )", null, trxName);
                                                    }
                                                }
                                                _log.Severe("Error occured during UpdateProductCost for C_InvoiceLine_ID = " + invoiceline.GetC_InvoiceLine_ID());
                                                return false;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (windowName.Equals("ProvisionalInvoice"))
                            {
                                // Update Product Cost for Provisional Weighted Average
                                result = cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, Util.GetValueOfInt(po.Get_Value("AD_Org_ID")), optionalStrCd: optionalstr);
                                if (!result)
                                {
                                    if (optionalstr != "window")
                                    {
                                        trxName.Rollback();
                                    }
                                    else
                                    {
                                        DB.ExecuteQuery("DELETE FROM M_CostDetail WHERE M_CostDetail_ID = " + cd.GetM_CostDetail_ID(), null, trxName);
                                    }
                                    _log.Severe("Error occured during UpdateProductCost for C_ProvisonalInvoiceLine_ID = " + invoiceline.GetC_InvoiceLine_ID());
                                    return false;
                                }
                            }

                            #endregion

                            // change 7-4-2016
                            // for handling and calculating Cost Combination
                            if (windowName == "Invoice(Vendor)" && invoiceline.GetC_OrderLine_ID() == 0 && invoiceline.GetM_InOutLine_ID() == 0)
                            {
                                // cost element detail not to be calculated when Invoice(Vendor) is alone.
                                // But in case of APC, we have to calculate Cost Element detail and cost combination both
                                //discount is given only when document type having setting as "Treat As Discount" = True
                                MDocType docType = new MDocType(ctx, invoice.GetC_DocTypeTarget_ID(), trxName);
                                if (docType.IsTreatAsDiscount() && docType.GetDocBaseType() == "APC" && cd != null)
                                {
                                    // get costing level
                                    if (String.IsNullOrEmpty(costLevel))
                                    {
                                        costLevel = (MProductCategory.Get(product.GetCtx(), product.GetM_Product_Category_ID())).GetCostingLevel();
                                        if (String.IsNullOrEmpty(costLevel))
                                        {
                                            costLevel = acctSchema.GetCostingLevel();
                                        }
                                    }
                                    // when costing levele = warehouse or Warehouse + batch then not to calculate cost against indepenedent APC
                                    if (!((costLevel == MProductCategory.COSTINGLEVEL_Warehouse || costLevel == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                        && M_Warehouse_Id <= 0))
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
                _log.Severe("Error Occured during costing " + ex.Message);
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
        public static bool CreateProductCostsDetails(Ctx ctx, int AD_Client_ID, int AD_Org_ID, MProduct product, int M_ASI_ID,
                     string windowName, MInventoryLine inventoryLine, MInOutLine inoutline, MMovementLine movementline,
                     MInvoiceLine invoiceline, Decimal Price, Decimal Qty, Trx trxName, int[] acctSchemaRecord, out string conversionNotFound)
        {
            MAcctSchema acctSchema = null;
            dynamic pca = null;
            MCostElement costElement = null;
            MProduct productLca = null;
            MOrderLine orderline = null;
            MOrder order = null;
            MInvoice invoice = null;
            MMovement movement = null;
            MInventory inventory = null;
            MCostDetail cd = null;
            MCostDetail cdSourceWarehouse = null;
            MCost cost = null;
            MInOut inout = null;
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
            int AD_Org_ID2 = AD_Org_ID;
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
                    _log.Info("costing Calculation Start for window = " + windowName + " and product = " + product.GetM_Product_ID() + " AND ASI " + M_ASI_ID);
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
                        AD_Org_ID = AD_Org_ID2;
                        acctSchema = new MAcctSchema(ctx, acctSchemaRecord[i], trxName);
                        if (product != null)
                        {
                            #region Costing Level
                            pca = new MProductCategory(ctx, product.GetM_Product_Category_ID(), null);
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
                                            AD_Org_ID = 0;
                                        }
                                        else if ((string.IsNullOrEmpty(pca.GetCostingLevel())) && (acctSchema.GetCostingLevel() == "C" || acctSchema.GetCostingLevel() == "B"))
                                        {
                                            AD_Org_ID = 0;
                                        }
                                    }
                                    else if (acctSchema.GetCostingLevel() == "C" || acctSchema.GetCostingLevel() == "B")
                                    {
                                        AD_Org_ID = 0;
                                    }
                                }
                            }
                            #endregion
                        }

                        if (windowName == "Material Receipt" || windowName == "Customer Return" || windowName == "Shipment" || windowName == "Return To Vendor")
                        {
                            inout = new MInOut(ctx, inoutline.GetM_InOut_ID(), trxName);
                            if (inoutline.GetC_OrderLine_ID() > 0)
                            {
                                orderline = new MOrderLine(ctx, inoutline.GetC_OrderLine_ID(), trxName);
                                order = new MOrder(ctx, orderline.GetC_Order_ID(), trxName);
                                if (order.GetC_Currency_ID() != acctSchema.GetC_Currency_ID())
                                {
                                    Price = MConversionRate.Convert(ctx, Price, order.GetC_Currency_ID(), acctSchema.GetC_Currency_ID(),
                                                                     order.GetDateAcct(), order.GetC_ConversionType_ID(), AD_Client_ID, AD_Org_ID);
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
                                // but m_cost handled indivdually based on price
                                //Get Price from MCost
                                price = MCostQueue.CalculateCost(ctx, acctSchema, product, M_ASI_ID, AD_Client_ID, AD_Org_ID, 0);
                                Price = price * inoutline.GetMovementQty();
                                cmPrice = Price;
                                // if (Price != 0)
                                // {
                                Price = GetPrice(inoutline, acctSchema.GetC_Currency_ID());
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
                            invoice = new MInvoice(ctx, invoiceline.GetC_Invoice_ID(), trxName);
                            if (invoice.GetC_Currency_ID() != acctSchema.GetC_Currency_ID())
                            {
                                Price = MConversionRate.Convert(ctx, Price, invoice.GetC_Currency_ID(), acctSchema.GetC_Currency_ID(),
                                                                         invoice.GetDateAcct(), invoice.GetC_ConversionType_ID(), AD_Client_ID, AD_Org_ID);

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
                                query.Append(@"SELECT IsMatchPOForm FROM M_MatchPO WHERE IsActive = 'Y' 
                                     AND M_InoutLine_ID = " + Util.GetValueOfInt(invoiceline.GetM_InOutLine_ID()) +
                                         " AND M_Product_ID = " + product.GetM_Product_ID());
                                isMatchFromForm = Util.GetValueOfString(DB.ExecuteScalar(query.ToString(), null, trxName));
                                if (isMatchFromForm == "Y")
                                {
                                    inoutline = new MInOutLine(ctx, invoiceline.GetM_InOutLine_ID(), trxName);
                                    if (inoutline.GetM_InOutLine_ID() > 0)
                                    {
                                        M_ASI_ID = inoutline.GetM_AttributeSetInstance_ID();
                                        windowName = "Match IV";
                                    }
                                }
                            }

                            if (invoiceline.GetC_OrderLine_ID() <= 0)
                            {
                                inoutline = new MInOutLine(ctx, invoiceline.GetM_InOutLine_ID(), trxName);
                                if (inoutline.GetM_InOutLine_ID() > 0)
                                {
                                    M_ASI_ID = inoutline.GetM_AttributeSetInstance_ID();
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
                            pca = MProductCategory.Get(product.GetCtx(), product.GetM_Product_Category_ID());
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
                            //                  product.GetM_Product_Category_ID(), acctSchema.GetC_AcctSchema_ID(), product.Get_TrxName());
                            //    if (pca != null)
                            //    {
                            //        cl = pca.GetCostingLevel();
                            //    }
                            //    else
                            //    {
                            //        cl = acctSchema.GetCostingLevel();
                            //    }
                            //}

                            inout = new MInOut(ctx, inoutline.GetM_InOut_ID(), trxName);
                            orderLineId = Util.GetValueOfInt(Price); // here price parametr contain Orderline id from controller
                            orderline = new MOrderLine(ctx, orderLineId, trxName);

                            order = new MOrder(ctx, orderline.GetC_Order_ID(), trxName);
                            if (order.GetC_Currency_ID() != acctSchema.GetC_Currency_ID())
                            {
                                Price = MConversionRate.Convert(ctx, Decimal.Divide(orderline.GetLineNetAmt(), orderline.GetQtyOrdered()), order.GetC_Currency_ID(), acctSchema.GetC_Currency_ID(),
                                                                 order.GetDateAcct(), order.GetC_ConversionType_ID(), AD_Client_ID, AD_Org_ID);
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
                            query.Append("SELECT M_CostDetail_ID FROM M_CostDetail WHERE M_InOutLine_ID = " + inoutline.GetM_InOutLine_ID() +
                                 " AND c_invoiceline_id IS NULL  AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID());
                            costDetailId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));
                            cd = new MCostDetail(ctx, costDetailId, trxName);

                            if (costDetailId == 0)
                            {
                                cd.SetClientOrg(AD_Client_ID, AD_Org_ID);
                                cd.SetM_Product_ID(product.GetM_Product_ID());
                                cd.SetM_InOutLine_ID(inoutline.GetM_InOutLine_ID());
                                cd.SetM_AttributeSetInstance_ID(M_ASI_ID);
                                cd.SetC_AcctSchema_ID(acctSchema.GetC_AcctSchema_ID());
                            }

                            // MR complete before matching
                            if (inoutline.IsCostCalculated() && inout.IsCostCalculated())
                            {
                                //                                sql = @"SELECT C_OrderLine_Id FROM M_MatchPO WHERE IsActive = 'Y' AND M_MatchPO_ID = 
                                //                                                                 (SELECT MIN(M_MatchPO_ID) FROM M_MatchPO WHERE IsActive = 'Y' AND M_InOutLine_ID = " + inoutline.GetM_InOutLine_ID() + ") ";
                                //                                MatchPO_OrderLineId = Util.GetValueOfInt(DB.ExecuteScalar(sql));
                                //change 4-5-2016
                                query.Clear();
                                if (cl != "B")
                                {
                                    query.Append(@"SELECT  ROUND(Amt/Qty , 4) as currentCostAmount  FROM m_costelementdetail ced 
                                                                where  ced.IsActive = 'Y' AND ced.M_Product_ID = " + product.GetM_Product_ID() + @" 
                                                                 AND ced.C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() + @" AND  NVL(ced.M_AttributeSetInstance_ID , 0) = 0" +
                                       " AND ced.M_InOutLine_ID =  " + inoutline.GetM_InOutLine_ID() + @" AND NVL(ced.C_OrderLIne_ID , 0) = 0 AND NVL(ced.C_InvoiceLine_ID , 0) = 0 and 
                                                                 ced.M_CostElement_ID = (SELECT M_CostElement_ID from M_costElement where costingMethod = 'A' AND IsActive = 'Y' AND AD_Client_ID = " + inoutline.GetAD_Client_ID() + " ) AND ced.AD_Client_ID = " + inoutline.GetAD_Client_ID());
                                }
                                else
                                {
                                    query.Append(@"SELECT  ROUND(Amt/Qty , 4) as currentCostAmount  FROM m_costelementdetail ced 
                                                                where  ced.IsActive = 'Y' AND ced.M_Product_ID = " + product.GetM_Product_ID() + @" 
                                                                 AND ced.C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() + @" AND  NVL(ced.M_AttributeSetInstance_ID , 0) = " + M_ASI_ID + @" 
                                                                 AND ced.M_InOutLine_ID =  " + inoutline.GetM_InOutLine_ID() + @" AND NVL(ced.C_OrderLIne_ID , 0) = 0 AND NVL(ced.C_InvoiceLine_ID , 0) = 0 and 
                                                                 ced.M_CostElement_ID = (SELECT M_CostElement_ID from M_costElement where costingMethod = 'A' AND IsActive = 'Y' AND AD_Client_ID = " + inoutline.GetAD_Client_ID() + " ) AND ced.AD_Client_ID = " + inoutline.GetAD_Client_ID());
                                }
                                MRPriceAvPo = Util.GetValueOfDecimal(DB.ExecuteScalar(query.ToString()));

                                query.Clear();
                                if (cl != "B")
                                {
                                    query.Append(@"SELECT  ROUND(Amt/Qty , 4) as currentCostAmount  FROM m_costelementdetail ced 
                                                                where  ced.IsActive = 'Y' AND ced.M_Product_ID = " + product.GetM_Product_ID() + @" 
                                                                 AND ced.C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() + @" AND  NVL(ced.M_AttributeSetInstance_ID , 0) = 0 " +
                                         " AND ced.M_InOutLine_ID =  " + inoutline.GetM_InOutLine_ID() + @" AND NVL(ced.C_OrderLIne_ID , 0) = 0 AND NVL(ced.C_InvoiceLine_ID , 0) = 0 and 
                                                                 ced.M_CostElement_ID = (SELECT M_CostElement_ID from M_costElement where costingMethod = 'p' AND IsActive = 'Y' AND AD_Client_ID = " + inoutline.GetAD_Client_ID() + " ) AND ced.AD_Client_ID = " + inoutline.GetAD_Client_ID());
                                }
                                else
                                {
                                    query.Append(@"SELECT  ROUND(Amt/Qty , 4) as currentCostAmount  FROM m_costelementdetail ced 
                                                                where  ced.IsActive = 'Y' AND ced.M_Product_ID = " + product.GetM_Product_ID() + @" 
                                                                 AND ced.C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() + @" AND  NVL(ced.M_AttributeSetInstance_ID , 0) = " + M_ASI_ID + @" 
                                                                 AND ced.M_InOutLine_ID =  " + inoutline.GetM_InOutLine_ID() + @" AND NVL(ced.C_OrderLIne_ID , 0) = 0 AND NVL(ced.C_InvoiceLine_ID , 0) = 0 and 
                                                                 ced.M_CostElement_ID = (SELECT M_CostElement_ID from M_costElement where costingMethod = 'p' AND IsActive = 'Y' AND AD_Client_ID = " + inoutline.GetAD_Client_ID() + " ) AND ced.AD_Client_ID = " + inoutline.GetAD_Client_ID());
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
                                if (!CreateCostQueueForMatchPO(ctx, acctSchema, product, M_ASI_ID, AD_Client_ID, AD_Org_ID,
                                                              Price, inoutline.GetMovementQty(), inoutline, trxName, 0))
                                {
                                    _log.Severe("Error occured during craetion of cost queue -> MatchPO. M_Inoutline_ID is " + inoutline.GetM_InOutLine_ID());
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
                                query.Append(@"SELECT amt FROM T_Temp_CostDetail WHERE IsActive = 'Y' AND AD_Client_ID = " + AD_Client_ID +
                                             " AND AD_Org_ID = " + AD_Org_ID + " AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                             " AND M_Product_ID = " + product.GetM_Product_ID() + " AND isRecordFromForm = 'Y' " +
                                             " AND M_InOutLine_ID =  " + inoutline.GetM_InOutLine_ID());
                                MRPriceAvPo = Util.GetValueOfDecimal(DB.ExecuteScalar(query.ToString(), null, trxName));
                                MRPriceLastPO = MRPriceAvPo;
                                if (MRPriceAvPo != 0)
                                {
                                    isRecordFromForm = true;
                                }
                            }

                            if (cd.GetC_OrderLine_ID() == 0)
                            {
                                // update m_cost Accumulative Amount to be reduced for Av. PO and Last PO
                                query.Clear();
                                query.Append("SELECT M_CostElement_ID FROM M_CostElement WHERE  AD_Client_ID=" + AD_Client_ID + " AND IsActive='Y' AND CostElementType='M'  AND CostingMethod = 'A'");
                                int costElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));

                                costElement = new MCostElement(ctx, costElementId, null);

                                // Av. PO
                                if (cl != "B")
                                {
                                    cost = MCost.Get(product, 0, acctSchema, AD_Org_ID, costElementId);
                                }
                                else
                                {
                                    cost = MCost.Get(product, M_ASI_ID, acctSchema, AD_Org_ID, costElementId);
                                }
                                if (!isRecordFromForm && inoutline.IsCostCalculated())
                                    cost.SetCumulatedAmt(Decimal.Subtract(cost.GetCumulatedAmt(), (MRPriceAvPo * Qty)));
                                if (!cost.Save())
                                {
                                    trxName.Rollback();
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    _log.Severe("Error occured during saving a record , M_inoutLine_ID = " + inoutline.GetM_InOutLine_ID() + " in cost detail -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                    return false;
                                }

                                cd.SetC_OrderLine_ID(orderLineId);
                                cd.SetQty(Qty);
                                cd.SetAmt(Decimal.Round(Decimal.Multiply(Price, Qty), acctSchema.GetCostingPrecision()));
                                if (!cd.Save(trxName))
                                {
                                    trxName.Rollback();
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    _log.Severe("Error occured during saving a record, M_InoutLine_Id = " + inoutline.GetM_InOutLine_ID() + " in cost detail -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
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
                                    _log.Severe("Error occured during saving a record, M_InoutLine_ID = " + inoutline.GetM_InOutLine_ID() + " in M-Cost -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                    return false;
                                }
                                else
                                {
                                    if (cl != "B")
                                    {
                                        MCostElementDetail.CreateCostElementDetail(ctx, AD_Client_ID, AD_Org_ID, product, 0,
                                                             acctSchema, costElementId, windowName, cd, Decimal.Round(Decimal.Multiply(cost.GetCurrentCostPrice(), Qty), acctSchema.GetCostingPrecision()), Qty);
                                    }
                                    else
                                    {
                                        MCostElementDetail.CreateCostElementDetail(ctx, AD_Client_ID, AD_Org_ID, product, M_ASI_ID,
                                                                 acctSchema, costElementId, windowName, cd, Decimal.Round(Decimal.Multiply(cost.GetCurrentCostPrice(), Qty), acctSchema.GetCostingPrecision()), Qty);
                                    }
                                }

                                // last po
                                query.Clear();
                                query.Append("SELECT M_CostElement_ID FROM M_CostElement WHERE  AD_Client_ID=" + AD_Client_ID + " AND IsActive='Y' AND CostElementType='M'  AND CostingMethod = 'p'");
                                costElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));

                                costElement = new MCostElement(ctx, costElementId, null);

                                if (cl != "B")
                                {
                                    cost = MCost.Get(product, 0, acctSchema, AD_Org_ID, costElementId);
                                }
                                else
                                {
                                    cost = MCost.Get(product, M_ASI_ID, acctSchema, AD_Org_ID, costElementId);
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
                                    _log.Severe("Error occured during saving a record, N_InoutLine_ID = " + inoutline.GetM_InOutLine_ID() + " in M-Cost -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                    return false;
                                }
                                else
                                {
                                    if (cl != "B")
                                    {
                                        MCostElementDetail.CreateCostElementDetail(ctx, AD_Client_ID, AD_Org_ID, product, 0,
                                                             acctSchema, costElementId, windowName, cd, Decimal.Round(Decimal.Multiply(cost.GetCurrentCostPrice(), Qty), acctSchema.GetCostingPrecision()), Qty);
                                    }
                                    else
                                    {
                                        MCostElementDetail.CreateCostElementDetail(ctx, AD_Client_ID, AD_Org_ID, product, M_ASI_ID,
                                                             acctSchema, costElementId, windowName, cd, Decimal.Round(Decimal.Multiply(cost.GetCurrentCostPrice(), Qty), acctSchema.GetCostingPrecision()), Qty);
                                    }
                                }
                            }
                            else
                            {
                                cd = new MCostDetail(acctSchema, AD_Org_ID, product.GetM_Product_ID(), M_ASI_ID,
                                                      0, Decimal.Round(Decimal.Multiply(Qty, Price), acctSchema.GetCostingPrecision()), Qty, null, trxName);
                                cd.SetM_InOutLine_ID(inoutline.GetM_InOutLine_ID());
                                cd.SetC_OrderLine_ID(orderLineId);
                                if (!cd.Save(trxName))
                                {
                                    trxName.Rollback();
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    _log.Severe("Error occured during saving a record, M_InoutLine_Id = " + inoutline.GetM_InOutLine_ID() + " in Cost-Detail -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                    return false;
                                }

                                // Av. PO
                                query.Clear();
                                query.Append("SELECT M_CostElement_ID FROM M_CostElement WHERE  AD_Client_ID=" + AD_Client_ID + " AND IsActive='Y' AND CostElementType='M'  AND CostingMethod = 'A'");
                                int costElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));

                                costElement = new MCostElement(ctx, costElementId, null);

                                if (cl != "B")
                                {
                                    cost = MCost.Get(product, 0, acctSchema, AD_Org_ID, costElementId);
                                }
                                else
                                {
                                    cost = MCost.Get(product, M_ASI_ID, acctSchema, AD_Org_ID, costElementId);
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
                                    _log.Severe("Error occured during saving a record, M_inoutLine_Id = " + inoutline.GetM_InOutLine_ID() + " in M-Cost -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                    return false;
                                }
                                else
                                {
                                    MCostElementDetail.CreateCostElementDetail(ctx, AD_Client_ID, AD_Org_ID, product, M_ASI_ID,
                                                         acctSchema, costElementId, windowName, cd, Decimal.Round(Decimal.Multiply(cost.GetCurrentCostPrice(), Qty), acctSchema.GetCostingPrecision()), Qty);
                                }

                                // last po
                                query.Clear();
                                query.Append("SELECT M_CostElement_ID FROM M_CostElement WHERE  AD_Client_ID=" + AD_Client_ID + " AND IsActive='Y' AND CostElementType='M'  AND CostingMethod = 'p'");
                                costElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));

                                costElement = new MCostElement(ctx, costElementId, null);

                                if (cl != "B")
                                {
                                    cost = MCost.Get(product, 0, acctSchema, AD_Org_ID, costElementId);
                                }
                                else
                                {
                                    cost = MCost.Get(product, M_ASI_ID, acctSchema, AD_Org_ID, costElementId);
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
                                    _log.Severe("Error occured during saving a record, M_inoutLine_Id = " + inoutline.GetM_InOutLine_ID() + " in M-Cost -> MatchPO. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                    return false;
                                }
                                else
                                {
                                    MCostElementDetail.CreateCostElementDetail(ctx, AD_Client_ID, AD_Org_ID, product, M_ASI_ID,
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
                            pca = MProductCategory.Get(product.GetCtx(), product.GetM_Product_Category_ID());
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
                            //                  product.GetM_Product_Category_ID(), acctSchema.GetC_AcctSchema_ID(), product.Get_TrxName());
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
                            query.Append("SELECT M_CostDetail_ID FROM M_CostDetail WHERE M_InOutLine_ID = " + inoutline.GetM_InOutLine_ID() +
                                    " AND c_orderline_id IS NULL  AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID());
                            costDetailId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));
                            cd = new MCostDetail(ctx, costDetailId, trxName);

                            query.Clear();
                            query.Append(@"SELECT C_OrderLine_Id FROM M_MatchPO WHERE IsActive = 'Y' AND M_MatchPO_ID = 
                                     (SELECT MIN(M_MatchPO_ID) FROM M_MatchPO WHERE IsActive = 'Y' AND M_InOutLine_ID = " + inoutline.GetM_InOutLine_ID() + ") ");
                            MatchPO_OrderLineId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString()));

                            if (cl != "B")
                            {
                                query.Clear();
                                query.Append(@"SELECT  Amt/Qty as currentCostAmount  FROM m_costelementdetail ced 
                                    where  ced.IsActive = 'Y' AND ced.M_Product_ID = " + product.GetM_Product_ID() + @" 
                                     AND ced.C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() + @" AND  NVL(ced.M_AttributeSetInstance_ID , 0) = 0 " +
                                     " AND ced.M_InOutLine_ID =  " + inoutline.GetM_InOutLine_ID() + @" AND NVL(ced.C_OrderLIne_ID , 0) = " + MatchPO_OrderLineId + @" AND NVL(ced.C_InvoiceLine_ID , 0) = 0 and 
                                     ced.M_CostElement_ID = (SELECT M_CostElement_ID from M_costElement where costingMethod = 'L' AND IsActive = 'Y' AND AD_Client_ID = " + inoutline.GetAD_Client_ID() + " ) AND ced.AD_Client_ID = " + inoutline.GetAD_Client_ID());
                            }
                            else
                            {
                                query.Clear();
                                query.Append(@"SELECT  Amt/Qty as currentCostAmount  FROM m_costelementdetail ced 
                                    where  ced.IsActive = 'Y' AND ced.M_Product_ID = " + product.GetM_Product_ID() + @" 
                                     AND ced.C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() + @" AND  NVL(ced.M_AttributeSetInstance_ID , 0) = " + M_ASI_ID + @" 
                                     AND ced.M_InOutLine_ID =  " + inoutline.GetM_InOutLine_ID() + @" AND NVL(ced.C_OrderLIne_ID , 0) = " + MatchPO_OrderLineId + @" AND NVL(ced.C_InvoiceLine_ID , 0) = 0 and 
                                     ced.M_CostElement_ID = (SELECT M_CostElement_ID from M_costElement where costingMethod = 'L' AND IsActive = 'Y' AND AD_Client_ID = " + inoutline.GetAD_Client_ID() + " ) AND ced.AD_Client_ID = " + inoutline.GetAD_Client_ID());
                            }
                            MRPriceLifo = Util.GetValueOfDecimal(DB.ExecuteScalar(query.ToString()));

                            if (cl != "B")
                            {
                                query.Clear();
                                query.Append(@"SELECT  Amt/Qty as currentCostAmount  FROM m_costelementdetail ced 
                                    where  ced.IsActive = 'Y' AND ced.M_Product_ID = " + product.GetM_Product_ID() + @" 
                                     AND ced.C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() + @" AND  NVL(ced.M_AttributeSetInstance_ID , 0) = 0 " +
                                     " AND ced.M_InOutLine_ID =  " + inoutline.GetM_InOutLine_ID() + @" AND NVL(ced.C_OrderLIne_ID , 0) = " + MatchPO_OrderLineId + @" AND NVL(ced.C_InvoiceLine_ID , 0) = 0 and 
                                     ced.M_CostElement_ID = (SELECT M_CostElement_ID from M_costElement where costingMethod = 'F' AND IsActive = 'Y' AND AD_Client_ID = " + inoutline.GetAD_Client_ID() + " ) AND ced.AD_Client_ID = " + inoutline.GetAD_Client_ID());
                            }
                            else
                            {
                                query.Clear();
                                query.Append(@"SELECT  Amt/Qty as currentCostAmount  FROM m_costelementdetail ced 
                                    where  ced.IsActive = 'Y' AND ced.M_Product_ID = " + product.GetM_Product_ID() + @" 
                                     AND ced.C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() + @" AND  NVL(ced.M_AttributeSetInstance_ID , 0) = " + M_ASI_ID + @" 
                                     AND ced.M_InOutLine_ID =  " + inoutline.GetM_InOutLine_ID() + @" AND NVL(ced.C_OrderLIne_ID , 0) = " + MatchPO_OrderLineId + @" AND NVL(ced.C_InvoiceLine_ID , 0) = 0 and 
                                     ced.M_CostElement_ID = (SELECT M_CostElement_ID from M_costElement where costingMethod = 'F' AND IsActive = 'Y' AND AD_Client_ID = " + inoutline.GetAD_Client_ID() + " ) AND ced.AD_Client_ID = " + inoutline.GetAD_Client_ID());
                            }
                            MRPriceFifo = Util.GetValueOfDecimal(DB.ExecuteScalar(query.ToString()));

                            ceFifo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_CostElement_ID from M_costElement where costingMethod = 'F' AND IsActive = 'Y' AND AD_Client_ID = " + inoutline.GetAD_Client_ID()));
                            ceLifo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_CostElement_ID from M_costElement where costingMethod = 'L' AND IsActive = 'Y' AND AD_Client_ID = " + inoutline.GetAD_Client_ID()));

                            if (costDetailId > 0 && cd.GetC_InvoiceLine_ID() == 0)
                            {
                                // set amount and qty on cost detail from match receipt
                                cd.SetAmt(Price);
                                cd.SetQty(Qty);
                                cd.SetC_InvoiceLine_ID(invoiceline.GetC_InvoiceLine_ID());
                                cd.SetC_OrderLine_ID(invoiceline.GetC_OrderLine_ID());
                                if (!cd.Save(trxName))
                                {
                                    trxName.Rollback();
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    _log.Severe("Error occured during saving a record in M-Cost -> Match IV. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                    return false;
                                }

                                //get Cost Queue Record based on condition
                                query.Clear();
                                query.Append(@"SELECT * FROM T_Temp_CostDetail WHERE  M_InOutLine_ID = " + inoutline.GetM_InOutLine_ID() +
                                              " AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID());
                                DataSet ds2 = DB.ExecuteDataset(query.ToString(), null, null);

                                // reduce cummulative amount , qty for all element
                                cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, invoiceline.GetAD_Org_ID());

                                if (handlingWindowName == "Invoice(Vendor)")
                                {
                                    if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                    {
                                        for (int b = 0; b < ds2.Tables[0].Rows.Count; b++)
                                        {
                                            if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_CostElement_ID FROM M_CostQueue WHERE M_CostQueue_ID = " + (Util.GetValueOfInt(ds2.Tables[0].Rows[b]["M_CostQueue_ID"])))) == ceFifo)
                                            {
                                                MRPrice = MRPriceFifo;
                                                costElement = new MCostElement(ctx, ceFifo, null);
                                            }
                                            else
                                            {
                                                MRPrice = MRPriceLifo;
                                                costElement = new MCostElement(ctx, ceLifo, null);
                                            }
                                            price = MCostQueue.CalculateCostQueuePrice(invoice, invoiceline, MRPrice, acctSchema, AD_Client_ID, AD_Org_ID, costElement, trxName, false, "process", null);
                                            if (price == 0)
                                            {
                                                trxName.Rollback();
                                                conversionNotFound = invoice.GetDocumentNo();
                                                return false;
                                            }
                                            query.Clear();
                                            query.Append("UPDATE m_costqueue SET CurrentCostPrice = " + price + " WHERE M_CostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[b]["M_CostQueue_ID"]));
                                            //query.Append("UPDATE m_costqueue SET CurrentCostPrice = " + Decimal.Round(Decimal.Add(price, Decimal.Divide(Decimal.Multiply(price, costElement.GetSurchargePercentage()), 100)), acctSchema.GetCostingPrecision()) + " WHERE M_CostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[b]["M_CostQueue_ID"]));
                                            DB.ExecuteQuery(query.ToString(), null, trxName);
                                        }
                                    }
                                }
                                else
                                {
                                    // Calculate amount of matched Invoice
                                    query.Clear();
                                    query.Append(@"SELECT il.c_invoiceline_id , il.c_invoice_id , ROUND(il.linenetamt/il.qtyinvoiced , 4) AS priceactual , mi.qty ,  SUM(mi.qty) AS matchedqty 
                                          FROM m_inout io INNER JOIN m_inoutline iol ON io.m_inout_id = iol.m_inout_id
                                          INNER JOIN m_matchInv mi ON iol.m_inoutline_id = mi.m_inoutline_id
                                          INNER JOIN c_invoiceline il ON il.c_invoiceline_id = mi.c_invoiceline_id
                                          WHERE mi.isactive      = 'Y' AND io.M_InOut_ID      =" + inoutline.GetM_InOut_ID() +
                                                    " AND iol.m_inoutline_ID = " + inoutline.GetM_InOutLine_ID() +
                                                     " GROUP BY  il.c_invoiceline_id , il.c_invoice_id , ROUND(il.linenetamt/il.qtyinvoiced , 4) , mi.qty ");
                                    ds2 = DB.ExecuteDataset(query.ToString(), null, trxName);
                                    if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                    {
                                        Price = 0;
                                        Qty = 0;
                                        MInvoice inv = null;
                                        for (int n = 0; n < ds2.Tables[0].Rows.Count; n++)
                                        {
                                            Qty += Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["matchedqty"]);
                                            inv = new MInvoice(ctx, Util.GetValueOfInt(ds2.Tables[0].Rows[n]["c_invoice_id"]), trxName);
                                            if (inv.GetC_Currency_ID() != acctSchema.GetC_Currency_ID())
                                            {
                                                Price += MConversionRate.Convert(ctx, decimal.Multiply(Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["priceactual"]), Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["qty"])),
                                                                                 inv.GetC_Currency_ID(), acctSchema.GetC_Currency_ID(),
                                                                                 inv.GetDateAcct(), inv.GetC_ConversionType_ID(), AD_Client_ID, AD_Org_ID);
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
                                    query.Append(@"SELECT * FROM T_Temp_CostDetail WHERE  M_InOutLine_ID = " + inoutline.GetM_InOutLine_ID() +
                                              " AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID());
                                    ds2 = DB.ExecuteDataset(query.ToString(), null, null);
                                    if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                    {
                                        matchInvoicePrice = Price;
                                        for (int k = 0; k < ds2.Tables[0].Rows.Count; k++)
                                        {
                                            Price = matchInvoicePrice;
                                            //change 4-5-2016
                                            if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_CostElement_ID FROM M_CostQueue WHERE M_CostQueue_ID = " + (Util.GetValueOfInt(ds2.Tables[0].Rows[k]["m_costqueue_id"])))) == ceFifo)
                                            {
                                                MRPrice = MRPriceFifo;
                                                costElement = new MCostElement(ctx, ceFifo, null);
                                            }
                                            else
                                            {
                                                MRPrice = MRPriceLifo;
                                                costElement = new MCostElement(ctx, ceLifo, null);
                                            }
                                            //24-Aug-2016
                                            Price = Decimal.Round(Decimal.Add(Price, Decimal.Divide(Decimal.Multiply(Price, costElement.GetSurchargePercentage()), 100)), acctSchema.GetCostingPrecision());
                                            //end
                                            Price = Decimal.Add(Price, Decimal.Multiply(MRPrice, tempQty));
                                            //end
                                            query.Clear();
                                            query.Append("UPDATE m_costqueue SET CurrentCostPrice = " + Decimal.Round((Price / Qty), acctSchema.GetCostingPrecision()) + " WHERE M_CostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[k]["M_CostQueue_ID"]));
                                            //query.Append("UPDATE m_costqueue SET CurrentCostPrice = " + Decimal.Round(Decimal.Add((Price / Qty), (((Price / Qty) * costElement.GetSurchargePercentage()) / 100)), acctSchema.GetCostingPrecision()) + " WHERE M_CostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[k]["M_CostQueue_ID"]));
                                            DB.ExecuteQuery(query.ToString(), null, trxName);
                                        }
                                    }
                                    ds2.Dispose();
                                }

                                // update m_cost with accumulation qty , amt , and current cost
                                result = cd.UpdateProductCost("Product Cost IV", cd, acctSchema, product, M_ASI_ID, invoiceline.GetAD_Org_ID());
                                if (!result)
                                {
                                    trxName.Rollback();
                                    _log.Severe("Error occured during saving a record in M-Cost -> Match IV");
                                    return false;
                                }
                            }
                            else
                            {
                                invoice = new MInvoice(ctx, invoiceline.GetC_Invoice_ID(), trxName);
                                if (invoice.GetC_Currency_ID() != acctSchema.GetC_Currency_ID())
                                {
                                    Price = MConversionRate.Convert(ctx, Decimal.Multiply(Qty, Decimal.Round(Decimal.Divide(invoiceline.GetLineNetAmt(), invoiceline.GetQtyInvoiced()), 2)), invoice.GetC_Currency_ID(), acctSchema.GetC_Currency_ID(),
                                                                             invoice.GetDateAcct(), invoice.GetC_ConversionType_ID(), AD_Client_ID, AD_Org_ID);
                                    if (Price == 0)
                                    {
                                        trxName.Rollback();
                                        conversionNotFound = invoice.GetDocumentNo();
                                        return false;
                                    }
                                }

                                cd = new MCostDetail(acctSchema, AD_Org_ID, product.GetM_Product_ID(), M_ASI_ID,
                                                         0, Price, Qty, null, trxName);
                                cd.SetC_InvoiceLine_ID(invoiceline.GetC_InvoiceLine_ID());
                                cd.SetC_OrderLine_ID(invoiceline.GetC_OrderLine_ID());
                                cd.SetM_InOutLine_ID(inoutline.GetM_InOutLine_ID());
                                if (!cd.Save(trxName))
                                {
                                    trxName.Rollback();
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    _log.Severe("Error occured during saving a record, C_InvoiceLine_ID = " + invoiceline.GetC_InvoiceLine_ID() + " in cost detail -> Match IV. Error Value :  " + pp.GetValue() + " AND Error Name : " + pp.GetName());
                                    return false;
                                }
                                else
                                {
                                    // reduce cummulative amount for all element
                                    result = cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, invoiceline.GetAD_Org_ID());
                                    if (!result)
                                    {
                                        trxName.Rollback();
                                        _log.Severe("Error occured during saving a record , C_InvoiceLine_ID = " + invoiceline.GetC_InvoiceLine_ID() + " in M-Cost -> Match IV");
                                        return false;
                                    }

                                    // update cost queue with invoice amount
                                    query.Clear();
                                    query.Append(@"SELECT * FROM T_Temp_CostDetail WHERE  M_InOutLine_ID = " + inoutline.GetM_InOutLine_ID() +
                                              " AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID());
                                    DataSet ds2 = DB.ExecuteDataset(query.ToString(), null, null);

                                    if (handlingWindowName == "Invoice(Vendor)")
                                    {
                                        price = 0;
                                        if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                        {
                                            for (int b = 0; b < ds2.Tables[0].Rows.Count; b++)
                                            {
                                                //change 4-5-2016
                                                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_CostElement_ID FROM M_CostQueue WHERE M_CostQueue_ID = " + (Util.GetValueOfInt(ds2.Tables[0].Rows[b]["M_CostQueue_ID"])))) == ceFifo)
                                                {
                                                    MRPrice = MRPriceFifo;
                                                    costElement = new MCostElement(ctx, ceFifo, null);
                                                }
                                                else
                                                {
                                                    MRPrice = MRPriceLifo;
                                                    costElement = new MCostElement(ctx, ceLifo, null);
                                                }
                                                //end
                                                price = 0;
                                                price = MCostQueue.CalculateCostQueuePrice(invoice, invoiceline, MRPrice, acctSchema, AD_Client_ID, AD_Org_ID, costElement, trxName, false, "process", null);
                                                if (price == 0)
                                                {
                                                    trxName.Rollback();
                                                    conversionNotFound = invoice.GetDocumentNo();
                                                    return false;
                                                }
                                                query.Clear();
                                                query.Append("UPDATE m_costqueue SET CurrentCostPrice = " + price + " WHERE M_CostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[b]["M_CostQueue_ID"]));
                                                //query.Append("UPDATE m_costqueue SET CurrentCostPrice = " + Decimal.Round(Decimal.Add(price, Decimal.Divide(Decimal.Multiply(price, costElement.GetSurchargePercentage()), 100)), acctSchema.GetCostingPrecision()) + " WHERE M_CostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[b]["M_CostQueue_ID"]));
                                                DB.ExecuteQuery(query.ToString(), null, trxName);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // Calculate amount of matched Invoice
                                        query.Clear();
                                        query.Append(@"SELECT il.c_invoiceline_id , il.c_invoice_id , ROUND(il.linenetamt/il.qtyinvoiced , 4) AS priceactual , mi.qty ,  SUM(mi.qty) AS matchedqty 
                                          FROM m_inout io INNER JOIN m_inoutline iol ON io.m_inout_id = iol.m_inout_id
                                          INNER JOIN m_matchInv mi ON iol.m_inoutline_id = mi.m_inoutline_id
                                          INNER JOIN c_invoiceline il ON il.c_invoiceline_id = mi.c_invoiceline_id
                                          WHERE mi.isactive      = 'Y' AND io.M_InOut_ID      =" + inoutline.GetM_InOut_ID() +
                                                     " AND iol.m_inoutline_ID = " + inoutline.GetM_InOutLine_ID() +
                                                      " GROUP BY  il.c_invoiceline_id , il.c_invoice_id , ROUND(il.linenetamt/il.qtyinvoiced , 4) , mi.qty ");
                                        ds2 = DB.ExecuteDataset(query.ToString(), null, trxName);
                                        if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                        {
                                            Price = 0;
                                            Qty = 0; ;
                                            MInvoice inv = null;
                                            for (int n = 0; n < ds2.Tables[0].Rows.Count; n++)
                                            {
                                                Qty += Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["matchedqty"]);
                                                inv = new MInvoice(ctx, Util.GetValueOfInt(ds2.Tables[0].Rows[n]["c_invoice_id"]), trxName);
                                                if (inv.GetC_Currency_ID() != acctSchema.GetC_Currency_ID())
                                                {
                                                    Price += MConversionRate.Convert(ctx, decimal.Multiply(Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["priceactual"]), Util.GetValueOfDecimal(ds2.Tables[0].Rows[n]["qty"])),
                                                                                     inv.GetC_Currency_ID(), acctSchema.GetC_Currency_ID(),
                                                                                     inv.GetDateAcct(), inv.GetC_ConversionType_ID(), AD_Client_ID, AD_Org_ID);
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
                                        query.Append(@"SELECT * FROM T_Temp_CostDetail WHERE  M_InOutLine_ID = " + inoutline.GetM_InOutLine_ID() +
                                                  " AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID());

                                        ds2 = DB.ExecuteDataset(query.ToString(), null, null);
                                        if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                        {
                                            matchInvoicePrice = Price;
                                            for (int k = 0; k < ds2.Tables[0].Rows.Count; k++)
                                            {
                                                Price = matchInvoicePrice;
                                                //change 4-5-2016
                                                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_CostElement_ID FROM M_CostQueue WHERE M_CostQueue_ID = " + (Util.GetValueOfInt(ds2.Tables[0].Rows[k]["M_CostQueue_ID"])))) == ceFifo)
                                                {
                                                    MRPrice = MRPriceFifo;
                                                    costElement = new MCostElement(ctx, ceFifo, null);
                                                }
                                                else
                                                {
                                                    MRPrice = MRPriceLifo;
                                                    costElement = new MCostElement(ctx, ceLifo, null);
                                                }
                                                //24-Aug-2016
                                                Price = Decimal.Round(Decimal.Add(Price, Decimal.Divide(Decimal.Multiply(Price, costElement.GetSurchargePercentage()), 100)), acctSchema.GetCostingPrecision());
                                                //end
                                                Price = Decimal.Add(Price, Decimal.Multiply(MRPrice, tempQty));
                                                //end
                                                query.Clear();
                                                query.Append("UPDATE m_costqueue SET CurrentCostPrice = " + Decimal.Round(Price / Qty, acctSchema.GetCostingPrecision()) + " WHERE M_CostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[k]["M_CostQueue_ID"]));
                                                //query.Append("UPDATE m_costqueue SET CurrentCostPrice = " + Decimal.Round(Decimal.Add((Price / Qty), (((Price / Qty) * costElement.GetSurchargePercentage()) / 100)), acctSchema.GetCostingPrecision()) + " WHERE M_CostQueue_ID = " + Util.GetValueOfInt(ds2.Tables[0].Rows[k]["M_CostQueue_ID"]));
                                                DB.ExecuteQuery(query.ToString(), null, trxName);
                                            }
                                        }
                                        ds2.Dispose();
                                    }

                                    // update m_cost with accumulation qty , amt , and current cost
                                    result = cd.UpdateProductCost("Product Cost IV", cd, acctSchema, product, M_ASI_ID, invoiceline.GetAD_Org_ID());
                                    if (!result)
                                    {
                                        trxName.Rollback();
                                        _log.Severe("Error occured during saving a record in M-Cost -> Product Cost IV");
                                        return false;
                                    }
                                }

                            }
                            if (inoutline != null && inoutline.GetM_InOutLine_ID() > 0)
                            {
                                inoutline.SetIsCostCalculated(true);
                                inoutline.Save();
                            }
                            #endregion
                        }

                        if (windowName == "Physical Inventory" || windowName == "Internal Use Inventory" || windowName == "Inventory Move")
                        {
                            #region Get Price
                            //Get Price from MCost
                            // change 2-5-2016
                            price = MCostQueue.CalculateCost(ctx, acctSchema, product, M_ASI_ID, AD_Client_ID, AD_Org_ID, 0);
                            cmPrice = price;
                            price = 0;
                            //end
                            int C_Currency_ID = 0;
                            if (price == 0 || plPrice == 0)
                            {
                                isPriceFromProductPrice = true;
                                if (windowName == "Inventory Move")
                                {
                                    query.Clear();
                                    query.Append(@"(SELECT pl.AD_Org_ID , pl.name , pl.m_pricelist_id  , plv.m_pricelist_version_id , pp.pricestd , pl.c_currency_id
                                           FROM m_pricelist pl  INNER JOIN m_pricelist_version plv   ON pl.m_pricelist_id    = plv.m_pricelist_id
                                            inner join m_productprice pp   on pp.m_pricelist_version_id = plv.m_pricelist_version_id
                                           WHERE pl.AD_Org_ID = " + AD_Org_ID + " AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.m_product_id = " + product.GetM_Product_ID() +
                                           " AND NVL(pp.m_attributesetinstance_id, 0) = " + M_ASI_ID +
                                           " AND pp.c_uom_id = " + product.GetC_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.m_pricelist_id asc");
                                }
                                else
                                {
                                    query.Clear();
                                    query.Append(@"(SELECT pl.AD_Org_ID , pl.name , pl.m_pricelist_id  , plv.m_pricelist_version_id , pp.pricestd , pl.c_currency_id
                                           FROM m_pricelist pl  INNER JOIN m_pricelist_version plv   ON pl.m_pricelist_id    = plv.m_pricelist_id
                                            inner join m_productprice pp   on pp.m_pricelist_version_id = plv.m_pricelist_version_id
                                           WHERE pl.AD_Org_ID = " + inventoryLine.GetAD_Org_ID() + " AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.m_product_id = " + product.GetM_Product_ID() +
                                              " AND NVL(pp.m_attributesetinstance_id, 0) = " + M_ASI_ID +
                                              " AND pp.c_uom_id = " + product.GetC_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.m_pricelist_id asc");
                                }
                                DataSet dsStdPrice = DB.ExecuteDataset(query.ToString(), null, null);
                                if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                                {
                                    price = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                                    C_Currency_ID = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["c_currency_id"]);
                                }
                                else
                                {
                                    query.Clear();
                                    query.Append(@"(SELECT pl.AD_Org_ID , pl.name , pl.m_pricelist_id  , plv.m_pricelist_version_id , pp.pricestd , pl.c_currency_id
                                           FROM m_pricelist pl  INNER JOIN m_pricelist_version plv   ON pl.m_pricelist_id    = plv.m_pricelist_id
                                            inner join m_productprice pp   on pp.m_pricelist_version_id = plv.m_pricelist_version_id
                                           WHERE pl.AD_Org_ID = 0 AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.m_product_id = " + product.GetM_Product_ID() +
                                           " AND NVL(pp.m_attributesetinstance_id, 0) = " + M_ASI_ID +
                                           " AND pp.c_uom_id = " + product.GetC_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.m_pricelist_id asc");
                                    dsStdPrice = DB.ExecuteDataset(query.ToString(), null, null);
                                    if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                                    {
                                        price = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                                        C_Currency_ID = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["c_currency_id"]);
                                    }
                                }
                                if (price == 0)
                                {
                                    if (windowName == "Inventory Move")
                                    {
                                        query.Clear();
                                        query.Append(@"(SELECT pl.AD_Org_ID , pl.name , pl.m_pricelist_id  , plv.m_pricelist_version_id , pp.pricestd , pl.c_currency_id
                                           FROM m_pricelist pl  INNER JOIN m_pricelist_version plv   ON pl.m_pricelist_id    = plv.m_pricelist_id
                                            inner join m_productprice pp   on pp.m_pricelist_version_id = plv.m_pricelist_version_id
                                           WHERE pl.AD_Org_ID = " + AD_Org_ID + " AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.m_product_id = " + product.GetM_Product_ID() +
                                           " AND pp.c_uom_id = " + product.GetC_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.m_pricelist_id asc");
                                    }
                                    else
                                    {
                                        query.Clear();
                                        query.Append(@"(SELECT pl.AD_Org_ID , pl.name , pl.m_pricelist_id  , plv.m_pricelist_version_id , pp.pricestd , pl.c_currency_id
                                           FROM m_pricelist pl  INNER JOIN m_pricelist_version plv   ON pl.m_pricelist_id    = plv.m_pricelist_id
                                            inner join m_productprice pp   on pp.m_pricelist_version_id = plv.m_pricelist_version_id
                                           WHERE pl.AD_Org_ID = " + inventoryLine.GetAD_Org_ID() + " AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.m_product_id = " + product.GetM_Product_ID() +
                                              " AND pp.c_uom_id = " + product.GetC_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.m_pricelist_id asc");
                                    }
                                    dsStdPrice = DB.ExecuteDataset(query.ToString(), null, null);
                                    if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                                    {
                                        price = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                                        C_Currency_ID = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["c_currency_id"]);
                                    }
                                    else
                                    {
                                        query.Clear();
                                        query.Append(@"(SELECT pl.AD_Org_ID , pl.name , pl.m_pricelist_id  , plv.m_pricelist_version_id , pp.pricestd , pl.c_currency_id
                                           FROM m_pricelist pl  INNER JOIN m_pricelist_version plv   ON pl.m_pricelist_id    = plv.m_pricelist_id
                                            inner join m_productprice pp   on pp.m_pricelist_version_id = plv.m_pricelist_version_id
                                           WHERE pl.AD_Org_ID = 0 AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.m_product_id = " + product.GetM_Product_ID() +
                                               " AND pp.c_uom_id = " + product.GetC_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.m_pricelist_id asc");
                                        dsStdPrice = DB.ExecuteDataset(query.ToString(), null, null);
                                        if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                                        {
                                            price = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                                            C_Currency_ID = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["c_currency_id"]);
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
                                inventory = new MInventory(ctx, inventoryLine.GetM_Inventory_ID(), trxName);
                                if (C_Currency_ID != 0 && C_Currency_ID != acctSchema.GetC_Currency_ID())
                                {
                                    Price = MConversionRate.Convert(ctx, Price, C_Currency_ID, acctSchema.GetC_Currency_ID(),
                                                                            inventory.GetMovementDate(), 0, AD_Client_ID, AD_Org_ID);
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
                                movement = new MMovement(ctx, movementline.GetM_Movement_ID(), trxName);
                                if (C_Currency_ID != 0 && C_Currency_ID != acctSchema.GetC_Currency_ID())
                                {
                                    Price = MConversionRate.Convert(ctx, Price, C_Currency_ID, acctSchema.GetC_Currency_ID(),
                                                                            movement.GetMovementDate(), 0, AD_Client_ID, AD_Org_ID);
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
                                cdSourceWarehouse = MCostDetail.CreateCostDetail(acctSchema, AD_Org_ID, product.GetM_Product_ID(), M_ASI_ID, windowName,
                                    inventoryLine, inoutline, movementline, invoiceline, null, 0, Decimal.Negate(cmPrice), Decimal.Negate(Qty), null, trxName);
                            }
                            else
                            {
                                cdSourceWarehouse = MCostDetail.CreateCostDetail(acctSchema, AD_Org_ID, product.GetM_Product_ID(), M_ASI_ID, windowName,
                                    inventoryLine, inoutline, movementline, invoiceline, null, 0, Decimal.Negate(Price), Decimal.Negate(Qty), null, trxName);
                            }
                            query.Clear();
                            query.Append("SELECT AD_Org_ID FROM M_Warehouse WHERE IsActive = 'Y' AND M_Warehouse_ID = (SELECT M_Warehouse_ID FROM M_Movement WHERE IsActive = 'Y' AND M_Movement_ID = " + movementline.GetM_Movement_ID() + ")");
                            AD_Org_ID = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));
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
                            query.Append("SELECT * FROM C_LandedCostAllocation WHERE  C_InvoiceLine_ID = " + invoiceline.GetC_InvoiceLine_ID());
                            DataSet dsLandedCostAllocation = DB.ExecuteDataset(query.ToString(), null, trxName);
                            if (dsLandedCostAllocation != null && dsLandedCostAllocation.Tables.Count > 0 && dsLandedCostAllocation.Tables[0].Rows.Count > 0)
                            {
                                invoice = new MInvoice(ctx, invoiceline.GetC_Invoice_ID(), trxName);
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
                                    if (invoice.GetC_Currency_ID() != acctSchema.GetC_Currency_ID())
                                    {
                                        amt = MConversionRate.Convert(ctx, amt, invoice.GetC_Currency_ID(), acctSchema.GetC_Currency_ID(),
                                                                             invoice.GetDateAcct(), invoice.GetC_ConversionType_ID(), AD_Client_ID, AD_Org_ID);
                                        if (amt == 0)
                                        {
                                            trxName.Rollback();
                                            conversionNotFound = invoice.GetDocumentNo();
                                            return false;
                                        }
                                    }
                                    cd = MCostDetail.CreateCostDetail(acctSchema, AD_Org_ID, Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["M_Product_ID"]),
                                        Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["M_AttributeSetInstance_ID"]), windowName, inventoryLine, inoutline, movementline,
                                     invoiceline, null, Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["M_CostElement_ID"]), amt,
                                     qntity, null, trxName);
                                    if (cd != null)
                                    {
                                        productLca = new MProduct(ctx, Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["M_Product_ID"]), trxName);
                                        M_ASI_ID = Util.GetValueOfInt(dsLandedCostAllocation.Tables[0].Rows[lca]["M_AttributeSetInstance_ID"]);
                                        cd.UpdateProductCost(windowName, cd, acctSchema, productLca, M_ASI_ID, invoiceline.GetAD_Org_ID());
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
                                cd = MCostDetail.CreateCostDetail(acctSchema, AD_Org_ID, product.GetM_Product_ID(), M_ASI_ID, windowName,
                                      inventoryLine, inoutline, movementline, invoiceline, null, 0, cmPrice, Qty, null, trxName);
                            }
                            else
                            {
                                cd = MCostDetail.CreateCostDetail(acctSchema, AD_Org_ID, product.GetM_Product_ID(), M_ASI_ID, windowName,
                                     inventoryLine, inoutline, movementline, invoiceline, null, 0, Price, Qty, null, trxName);
                            }
                        }

                        #region Create / update "Cost Queue"
                        if (cd != null && !isLandedCostAllocation)
                        {
                            query.Clear();
                            query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = " +
                                     " ( SELECT MMPolicy FROM M_Product_Category WHERE IsActive = 'Y' AND M_Product_Category_ID = " +
                                     " (SELECT M_Product_Category_ID FROM M_Product WHERE IsActive = 'Y' AND M_Product_ID = " + product.GetM_Product_ID() + " )) AND AD_Client_ID = " + AD_Client_ID);
                            costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                            costElement = new MCostElement(ctx, costingElementId, null);

                            if (windowName == "Physical Inventory" || windowName == "Internal Use Inventory")
                            {
                                #region Phy. Inventory / Internal Use Inventory
                                if (Qty > 0)
                                {
                                    result = CreateCostQueue(ctx, acctSchema, product, M_ASI_ID, AD_Client_ID, inventoryLine.GetAD_Org_ID(), Price / Qty, Qty, windowName,
                                          inventoryLine, inoutline, movementline, invoiceline, cd, trxName, out costQueuseIds);
                                    if (!result)
                                    {
                                        trxName.Rollback();
                                        _log.Severe("Error occured during CreateCostQueue for M_inventory_ID = " + inventoryLine.GetM_InventoryLine_ID());
                                        return false;
                                    }
                                }
                                else
                                {
                                    //1st entry either of FIFO of LIFO 
                                    updateCostQueue(product, M_ASI_ID, acctSchema, inventoryLine.GetAD_Org_ID(), costElement, Decimal.Negate(Qty));

                                    //2nd either for Fifo or lifo opposite of 1st entry
                                    if (costElement.GetCostingMethod() == "F")
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND AD_Client_ID = " + AD_Client_ID);
                                    }
                                    else
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND AD_Client_ID = " + AD_Client_ID);
                                    }
                                    costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                                    costElement = new MCostElement(ctx, costingElementId, null);
                                    updateCostQueue(product, M_ASI_ID, acctSchema, inventoryLine.GetAD_Org_ID(), costElement, Decimal.Negate(Qty));
                                }
                                #endregion
                            }
                            else if (windowName == "Material Receipt" || windowName == "Customer Return" || windowName == "Shipment" || windowName == "Return To Vendor")
                            {
                                #region Sales / Purchase / Return
                                if (Qty > 0)
                                {
                                    result = CreateCostQueue(ctx, acctSchema, product, M_ASI_ID, AD_Client_ID, inoutline.GetAD_Org_ID(), Price / Qty, Qty, windowName,
                                         inventoryLine, inoutline, movementline, invoiceline, cd, trxName, out costQueuseIds);
                                    if (!result)
                                    {
                                        trxName.Rollback();
                                        _log.Severe("Error occured during CreateCostQueue for M_Inout_ID = " + inoutline.GetM_InOutLine_ID());
                                        return false;
                                    }
                                }
                                else
                                {
                                    //1st entry either of FIFO of LIFO 
                                    updateCostQueue(product, M_ASI_ID, acctSchema, inoutline.GetAD_Org_ID(), costElement, Decimal.Negate(Qty));

                                    //2nd either for Fifo or lifo opposite of 1st entry
                                    if (costElement.GetCostingMethod() == "F")
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND AD_Client_ID = " + AD_Client_ID);
                                    }
                                    else
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND AD_Client_ID = " + AD_Client_ID);
                                    }
                                    costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                                    costElement = new MCostElement(ctx, costingElementId, null);
                                    updateCostQueue(product, M_ASI_ID, acctSchema, inoutline.GetAD_Org_ID(), costElement, Decimal.Negate(Qty));
                                }
                                #endregion
                            }
                            else if (windowName == "Inventory Move")
                            {
                                #region Inventory Move
                                if (Qty > 0)
                                {
                                    result = CreateCostQueue(ctx, acctSchema, product, M_ASI_ID, AD_Client_ID, AD_Org_ID, Price / Qty, Qty, windowName,
                                         inventoryLine, inoutline, movementline, invoiceline, cd, trxName, out costQueuseIds);
                                    if (!result)
                                    {
                                        trxName.Rollback();
                                        _log.Severe("Error occured during CreateCostQueue for m_MovementLime_ID = " + movementline.GetM_MovementLine_ID());
                                        return false;
                                    }

                                    //1st entry either of FIFO of LIFO 
                                    updateCostQueue(product, M_ASI_ID, acctSchema, movementline.GetAD_Org_ID(), costElement, Qty);

                                    //2nd either for Fifo or lifo opposite of 1st entry
                                    if (costElement.GetCostingMethod() == "F")
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND AD_Client_ID = " + AD_Client_ID);
                                    }
                                    else
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND AD_Client_ID = " + AD_Client_ID);
                                    }
                                    costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                                    costElement = new MCostElement(ctx, costingElementId, null);
                                    updateCostQueue(product, M_ASI_ID, acctSchema, movementline.GetAD_Org_ID(), costElement, Qty);
                                }
                                else
                                {
                                    result = CreateCostQueue(ctx, acctSchema, product, M_ASI_ID, AD_Client_ID, movementline.GetAD_Org_ID(), Price / Qty, Decimal.Negate(Qty), windowName,
                                        inventoryLine, inoutline, movementline, invoiceline, cd, trxName, out costQueuseIds);
                                    if (!result)
                                    {
                                        trxName.Rollback();
                                        _log.Severe("Error occured during CreateCostQueue for M_MovementLine_ID = " + movementline.GetM_MovementLine_ID());
                                        return false;
                                    }

                                    //1st entry either of FIFO of LIFO 
                                    updateCostQueue(product, M_ASI_ID, acctSchema, AD_Org_ID, costElement, Decimal.Negate(Qty));

                                    //2nd either for Fifo or lifo opposite of 1st entry
                                    if (costElement.GetCostingMethod() == "F")
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND AD_Client_ID = " + AD_Client_ID);
                                    }
                                    else
                                    {
                                        query.Clear();
                                        query.Append(@"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND AD_Client_ID = " + AD_Client_ID);
                                    }
                                    costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
                                    costElement = new MCostElement(ctx, costingElementId, null);
                                    updateCostQueue(product, M_ASI_ID, acctSchema, AD_Org_ID, costElement, Decimal.Negate(Qty));
                                }
                                #endregion
                            }
                            else if (windowName == "Invoice(Vendor)")
                            {
                                #region update cost on cost Queue / M_Cost
                                if (invoiceline.GetC_OrderLine_ID() > 0 && invoiceline.GetM_InOutLine_ID() > 0)
                                {
                                    query.Clear();
                                    query.Append(@"SELECT * FROM T_Temp_CostDetail WHERE C_OrderLine_ID = " + invoiceline.GetC_OrderLine_ID() +
                                         " AND M_InOutLine_ID = " + invoiceline.GetM_InOutLine_ID() + " AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID());
                                    DataSet ds1 = DB.ExecuteDataset(query.ToString(), null, trxName);
                                    if (ds1 != null && ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                                    {
                                        for (int k = 0; k < ds1.Tables[0].Rows.Count; k++)
                                        {
                                            // change 9-5-2016
                                            // handle partial payment
                                            X_T_Temp_CostDetail tempCostDetail = new X_T_Temp_CostDetail(invoiceline.GetCtx(), Util.GetValueOfInt(ds1.Tables[0].Rows[k]["T_Temp_CostDetail_ID"]), trxName);
                                            query.Clear();
                                            query.Append("SELECT M_CostElement_ID FROM M_CostQueue WHERE M_CostQueue_ID = " + Util.GetValueOfInt(ds1.Tables[0].Rows[k]["M_CostQueue_ID"]));
                                            costingElementId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, trxName));
                                            costElement = new MCostElement(ctx, costingElementId, trxName);

                                            price = MCostQueue.CalculateCostQueuePrice(invoice, invoiceline, tempCostDetail.GetAmt(), acctSchema, AD_Client_ID, AD_Org_ID, costElement, trxName, false, "process", null);
                                            //sql = "UPDATE m_costqueue SET CurrentCostPrice = " + cd.GetAmt() / cd.GetQty() + " WHERE M_CostQueue_ID = " + Util.GetValueOfInt(ds1.Tables[0].Rows[k]["M_CostQueue_ID"]);
                                            query.Clear();
                                            query.Append("UPDATE m_costqueue SET CurrentCostPrice = " + price + " WHERE M_CostQueue_ID = " + Util.GetValueOfInt(ds1.Tables[0].Rows[k]["M_CostQueue_ID"]));
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
                                result = cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, inventoryLine.GetAD_Org_ID());
                                if (!result)
                                {
                                    trxName.Rollback();
                                    _log.Severe("Error occured during UpdateProductCost for M_InventoryLine_ID = " + inventoryLine.GetM_InventoryLine_ID());
                                    return false;
                                }
                            }
                            else if (windowName == "Material Receipt" || windowName == "Customer Return" || windowName == "Shipment" || windowName == "Return To Vendor")
                            {
                                result = cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, inoutline.GetAD_Org_ID());
                                if (!result)
                                {
                                    trxName.Rollback();
                                    _log.Severe("Error occured during UpdateProductCost for M_InoutLine_ID = " + inoutline.GetM_InOutLine_ID());
                                    return false;
                                }
                            }
                            else if (windowName == "Inventory Move")
                            {
                                if (pca != null)
                                {
                                    if (pca.GetCostingLevel() == "C" || pca.GetCostingLevel() == "B")
                                    {
                                        AD_Org_ID = 0;
                                    }
                                    else if ((string.IsNullOrEmpty(pca.GetCostingLevel())) && (acctSchema.GetCostingLevel() == "C" || acctSchema.GetCostingLevel() == "B"))
                                    {
                                        AD_Org_ID = 0;
                                    }
                                }
                                else if (acctSchema.GetCostingLevel() == "C" || acctSchema.GetCostingLevel() == "B")
                                {
                                    AD_Org_ID = 0;
                                }
                                if (AD_Org_ID != 0)
                                {
                                    result = cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, AD_Org_ID); // for destination warehouse
                                    if (!result)
                                    {
                                        trxName.Rollback();
                                        _log.Severe("Error occured during UpdateProductCost for M_movementLine_ID = " + cd.GetM_MovementLine_ID());
                                        return false;
                                    }

                                    result = cdSourceWarehouse.UpdateProductCost(windowName, cdSourceWarehouse, acctSchema, product, M_ASI_ID, movementline.GetAD_Org_ID()); // for source warehouse org
                                    if (!result)
                                    {
                                        trxName.Rollback();
                                        _log.Severe("Error occured during UpdateProductCost fot M_MovementLine_ID = " + cd.GetM_MovementLine_ID());
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
                                if (invoiceline.GetC_OrderLine_ID() > 0)
                                {
                                    result = cd.UpdateProductCost(windowName, cd, acctSchema, product, M_ASI_ID, invoiceline.GetAD_Org_ID());
                                    if (!result)
                                    {
                                        trxName.Rollback();
                                        _log.Severe("Error occured during UpdateProductCost for C_invoiceLine_ID = " + invoiceline.GetC_InvoiceLine_ID());
                                        return false;
                                    }
                                }
                                else if (invoiceline.GetC_OrderLine_ID() == 0 && invoiceline.GetM_InOutLine_ID() == 0)
                                {
                                    // 20-4-2016
                                    // in case of independent AP credit memo, accumulation amt reduce and current cost of AV. PO/Invoice will be calculated
                                    MDocType docType = new MDocType(ctx, invoice.GetC_DocTypeTarget_ID(), trxName);
                                    if (docType.GetDocBaseType() == "APC")
                                    {
                                        result = cd.UpdateProductCost("Invoice(APC)", cd, acctSchema, product, M_ASI_ID, invoiceline.GetAD_Org_ID());
                                        if (!result)
                                        {
                                            trxName.Rollback();
                                            _log.Severe("Error occured during UpdateProductCost for C_InvoiceLine_ID = " + invoiceline.GetC_InvoiceLine_ID());
                                            return false;
                                        }
                                    }
                                }
                            }
                        }
                        #endregion

                        // change 7-4-2016
                        // for handling and calculating Cost Combination
                        if (windowName == "Invoice(Vendor)" && invoiceline.GetC_OrderLine_ID() == 0 && invoiceline.GetM_InOutLine_ID() == 0)
                        {
                            // cost element detail not to be calculated when Invoice(Vendor) is alone.
                            // But in case of APC, we have to calculate Cost Element detail and cost combination both
                            MDocType docType = new MDocType(ctx, invoice.GetC_DocTypeTarget_ID(), trxName);
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
        /// <param name="AD_Client_ID">client</param>
        /// <param name="AD_Org_ID">organization</param>
        /// <param name="product">product</param>
        /// <param name="M_ASI_ID">attributesetinstance</param>
        /// <param name="M_Warehouse_ID">M_Warehouse_ID</param>
        /// <returns>qty</returns>
        public static decimal CheckQtyAvailablity(Ctx ctx, MAcctSchema acctSchema, int AD_Client_ID, int AD_Org_ID, MProduct product, int M_ASI_ID, int M_Warehouse_ID)
        {
            decimal qty = 0;
            try
            {
                string sql = @"SELECT ROUND(AVG(CST.CURRENTQTY),4 )   FROM M_PRODUCT P   INNER JOIN M_COST CST   ON P.M_PRODUCT_ID=CST.M_PRODUCT_ID
                               LEFT JOIN M_PRODUCT_CATEGORY PC   ON P.M_PRODUCT_CATEGORY_ID=PC.M_PRODUCT_CATEGORY_ID
                               INNER JOIN C_ACCTSCHEMA ACC   ON CST.C_ACCTSCHEMA_ID=ACC.C_ACCTSCHEMA_ID
                               INNER JOIN M_COSTELEMENT CE  ON CST.M_COSTELEMENT_ID=CE.M_COSTELEMENT_ID
                              WHERE (( CASE WHEN PC.COSTINGMETHOD IS NOT NULL  THEN PC.COSTINGMETHOD
                                            ELSE ACC.COSTINGMETHOD  END) = CE.COSTINGMETHOD )
                              AND ((   CASE WHEN PC.COSTINGMETHOD IS NOT NULL  AND PC.COSTINGMETHOD   = 'C'  THEN PC.M_costelement_id
                                            WHEN PC.COSTINGMETHOD IS NOT NULL  THEN (SELECT M_CostElement_ID FROM M_costelement 
                                             WHERE COSTINGMETHOD = pc.COSTINGMETHOD AND ad_client_id    = " + AD_Client_ID + @" )
                                            WHEN ACC.COSTINGMETHOD IS NOT NULL AND ACC.COSTINGMETHOD   = 'C' THEN ACC.M_costelement_id ELSE
                                             (SELECT M_CostElement_ID FROM M_costelement WHERE COSTINGMETHOD = acc.COSTINGMETHOD 
                                             AND ad_client_id    = " + AD_Client_ID + @" ) END) = ce.M_COSTELEMENT_id)
                             AND ((    CASE WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('A' , 'O' , 'W' , 'D')  THEN " + AD_Org_ID + @"
                                            WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('B' , 'C')  THEN 0 
                                            WHEN ACC.COSTINGLEVEL IS NOT NULL AND ACC.COSTINGLEVEL   IN ('A' , 'O' , 'W' , 'D') THEN " + AD_Org_ID + @"
                                            ELSE 0  END) = CST.AD_Org_ID)
                            AND ((     CASE WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('A' , 'B', 'D')  THEN " + M_ASI_ID + @"
                                            WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('C' , 'O', 'W')  THEN 0 
                                            WHEN ACC.COSTINGLEVEL IS NOT NULL AND ACC.COSTINGLEVEL   IN ('A' , 'B' , 'D') THEN " + M_ASI_ID + @"
                                            ELSE 0   END) = NVL(CST.M_AttributeSetInstance_ID , 0))
                            AND ((     CASE WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('W' ,'D')  THEN " + M_Warehouse_ID + @"
                                             WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('A' ,'B' , 'C' ,'O')  THEN 0 
                                            WHEN ACC.COSTINGLEVEL IS NOT NULL AND ACC.COSTINGLEVEL   IN ('W' ,'D') THEN " + M_Warehouse_ID + @"
                                            ELSE 0   END) = NVL(CST.M_Warehouse_ID , 0))
                            AND P.M_PRODUCT_ID      =" + product.GetM_Product_ID() + @"
                            AND CST.C_ACCTSCHEMA_ID = " + acctSchema.GetC_AcctSchema_ID();
                qty = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
            }
            catch { }
            return qty;
        }

        /// <summary>
        /// Calculate Price
        /// </summary>
        /// <param name="invoice">Invoice</param>
        /// <param name="invoiceLine">Invoice Line</param>
        /// <param name="price">Price</param>
        /// <param name="acctSchema">Accouting Schema</param>
        /// <param name="AD_Client_ID">Client ID</param>
        /// <param name="AD_Org_ID">Organization</param>
        /// <param name="ce">Cost Element</param>
        /// <param name="trxName">TrxName</param>
        /// <param name="iscostImmdiate">cost Immediatly</param>
        /// <param name="optionalStr">Optional String</param>
        /// <param name="matchInoutLine">InOut Line Object</param>
        /// <returns>price</returns>
        public static Decimal CalculateCostQueuePrice(MInvoice invoice, MInvoiceLine invoiceLine, Decimal price, MAcctSchema acctSchema,
            int AD_Client_ID, int AD_Org_ID, MCostElement ce, Trx trxName, bool iscostImmdiate,
            string optionalStr, MInOutLine matchInoutLine)
        {
            return CalculateCostQueuePrice(invoice, invoiceLine, price, acctSchema, AD_Client_ID, AD_Org_ID, ce, trxName, iscostImmdiate,
                 optionalStr, matchInoutLine, null);
        }

        /// <summary>
        /// Calculate Price
        /// </summary>
        /// <param name="invoice">Invoice</param>
        /// <param name="invoiceLine">Invoice Line</param>
        /// <param name="price">Price</param>
        /// <param name="acctSchema">Accouting Schema</param>
        /// <param name="AD_Client_ID">Client ID</param>
        /// <param name="AD_Org_ID">Organization</param>
        /// <param name="ce">Cost Element</param>
        /// <param name="trxName">TrxName</param>
        /// <param name="iscostImmdiate">cost Immediatly</param>
        /// <param name="optionalStr">Optional String</param>
        /// <param name="matchInoutLine">InOut Line Object</param>
        /// <param name="costingCheck">Costing Check</param>
        /// <returns>price</returns>
        public static Decimal CalculateCostQueuePrice(MInvoice invoice, MInvoiceLine invoiceLine, Decimal price, MAcctSchema acctSchema,
            int AD_Client_ID, int AD_Org_ID, MCostElement ce, Trx trxName, bool iscostImmdiate,
            string optionalStr, MInOutLine matchInoutLine, CostingCheck costingCheck)
        {
            // if inoutline not available the return 0
            if (matchInoutLine == null)
            {
                if (costingCheck != null)
                {
                    matchInoutLine = costingCheck.inoutline;
                }
                if (matchInoutLine == null || matchInoutLine.Get_ID() <= 0 || matchInoutLine.Get_ID() != invoiceLine.GetM_InOutLine_ID())
                {
                    matchInoutLine = new MInOutLine(invoiceLine.GetCtx(), invoiceLine.GetM_InOutLine_ID(), trxName);
                }
            }
            if (matchInoutLine == null || matchInoutLine.GetM_InOutLine_ID() <= 0)
            {
                return 0;
            }

            Decimal ProductLineCost = invoiceLine.GetProductLineCost(invoiceLine);
            Decimal ProductInvoicePriceActual = ProductLineCost / invoiceLine.GetQtyEntered();
            Decimal MRQtyConsumed = 0;
            Decimal PriceLifoOrFifo = 0;
            Decimal surchargeAmount = 0;
            string isCostAdjustmentOnLost = "N";
            string sql = "";
            try
            {
                if (costingCheck != null && costingCheck.product != null && costingCheck.product.Get_ColumnIndex("IsCostAdjustmentOnLost") >= 0)
                {
                    isCostAdjustmentOnLost = costingCheck.product.IsCostAdjustmentOnLost() ? "Y" : "N";
                }
                else
                {
                    sql = @"SELECT COUNT(*) FROM AD_Column WHERE IsActive = 'Y' AND 
                                           AD_Table_ID =  ( SELECT AD_Table_ID FROM AD_Table WHERE IsActive = 'Y' AND TableName LIKE 'M_Product' ) 
                                           AND ColumnName LIKE 'IsCostAdjustmentOnLost' ";
                    int count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                    if (count > 0)
                    {
                        sql = @"SELECT IsCostAdjustmentOnLost FROM M_Product WHERE M_Product_ID =" + invoiceLine.GetM_Product_ID();
                        isCostAdjustmentOnLost = Util.GetValueOfString(DB.ExecuteScalar(sql, null, null));
                    }
                }


                sql = @"SELECT ROUND( il.linenetamt/il.qtyinvoiced , 4) as priceactual,
                            CASE
                                WHEN NVL(C_SurChargeTax.IsIncludeInCost , 'N') = 'Y'
                                AND NVL(C_Tax.IsIncludeInCost , 'N')           = 'Y'
                                THEN ROUND((il.taxbaseamt + il.taxamt + il.surchargeamt) / il.qtyinvoiced , 4)
                                WHEN NVL(C_SurChargeTax.IsIncludeInCost , 'N') = 'N'
                                AND NVL(C_Tax.IsIncludeInCost , 'N')           = 'Y'
                                THEN ROUND((il.taxbaseamt + il.taxamt) / il.qtyinvoiced , 4)
                                WHEN NVL(C_SurChargeTax.IsIncludeInCost , 'N') = 'Y'
                                AND NVL(C_Tax.IsIncludeInCost , 'N')           = 'N'
                                THEN ROUND((il.taxbaseamt + il.surchargeamt) / il.qtyinvoiced, 4)
                                ELSE ROUND(il.taxbaseamt  / il.qtyinvoiced, 4)
                              END AS PriceActualIncludedTax,
                                                   il.qtyinvoiced as qtyentered,  i.c_currency_id ,  i.c_conversiontype_id ,
                                                   il.c_invoiceline_id ,  iol.m_inout_id , il.m_inoutline_id  , i.dateacct , iol.movementqty AS mrqty
                                              FROM m_inoutline iol INNER JOIN c_invoiceline il ON il.m_inoutline_id = iol.m_inoutline_id
                                                   INNER JOIN c_tax C_Tax ON C_Tax.C_Tax_ID = il.C_Tax_ID
                                                   LEFT JOIN C_Tax C_SurChargeTax ON C_Tax.Surcharge_Tax_ID = C_SurChargeTax.C_Tax_ID
                                                   INNER JOIN c_invoice i ON i.c_invoice_id = il.c_invoice_id
                                              WHERE il.IsActive = 'Y' AND i.DocStatus IN ('CO' , 'CL') AND i.issotrx = 'N' and i.isreturntrx = 'N'
                                                    AND iol.M_Inoutline_ID = " + matchInoutLine.GetM_InOutLine_ID();
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
                        if (Util.GetValueOfInt(dsInvoiceRecord.Tables[0].Rows[i]["c_currency_id"]) != acctSchema.GetC_Currency_ID())
                        {
                            surchargeAmount = MConversionRate.Convert(invoiceLine.GetCtx(),
                                isCostAdjustmentOnLost == "Y" && MRQtyConsumed < Util.GetValueOfDecimal(dsInvoiceRecord.Tables[0].Rows[i]["qtyentered"]) ? decimal.Multiply(PriceActual, MRQtyConsumed)
                                : decimal.Multiply(PriceActual, Util.GetValueOfDecimal(dsInvoiceRecord.Tables[0].Rows[i]["qtyentered"])),
                                                  Util.GetValueOfInt(dsInvoiceRecord.Tables[0].Rows[i]["c_currency_id"]), acctSchema.GetC_Currency_ID(),
                                                  Util.GetValueOfDateTime(dsInvoiceRecord.Tables[0].Rows[i]["dateacct"]),
                                                  Util.GetValueOfInt(dsInvoiceRecord.Tables[0].Rows[i]["c_conversiontype_id"]), AD_Client_ID, AD_Org_ID);
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
                                    // when we revrse the transaction, remove invoice amount and add mr amount
                                    if (invoice.GetC_Currency_ID() != acctSchema.GetC_Currency_ID())
                                    {
                                        PriceLifoOrFifo += MConversionRate.Convert(invoiceLine.GetCtx(), (ProductInvoicePriceActual * invoiceLine.GetQtyEntered()), invoice.GetC_Currency_ID(), acctSchema.GetC_Currency_ID(),
                                                                                 invoice.GetDateAcct(), invoice.GetC_ConversionType_ID(), AD_Client_ID, AD_Org_ID);
                                    }
                                    else
                                    {
                                        PriceLifoOrFifo += (ProductInvoicePriceActual * invoiceLine.GetQtyEntered());
                                    }
                                    PriceLifoOrFifo -= price * invoiceLine.GetQtyEntered();
                                }
                                else
                                {
                                    PriceLifoOrFifo -= price * invoiceLine.GetQtyEntered();
                                    if (invoice.GetC_Currency_ID() != acctSchema.GetC_Currency_ID())
                                    {
                                        PriceLifoOrFifo += MConversionRate.Convert(invoiceLine.GetCtx(), (ProductInvoicePriceActual * invoiceLine.GetQtyEntered()), invoice.GetC_Currency_ID(), acctSchema.GetC_Currency_ID(),
                                                                                 invoice.GetDateAcct(), invoice.GetC_ConversionType_ID(), AD_Client_ID, AD_Org_ID);
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
                    MInOutLine VOinoutline = null;
                    if (costingCheck != null)
                    {
                        VOinoutline = costingCheck.inoutline;
                    }
                    if (VOinoutline == null || VOinoutline.Get_ID() <= 0 || VOinoutline.Get_ID() != matchInoutLine.GetM_InOutLine_ID())
                    {
                        VOinoutline = new MInOutLine(invoiceLine.GetCtx(), matchInoutLine.GetM_InOutLine_ID(), invoiceLine.Get_TrxName());
                    }
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

                        if (invoice.GetC_Currency_ID() != acctSchema.GetC_Currency_ID())
                        {
                            surchargeAmount = MConversionRate.Convert(invoiceLine.GetCtx(), (ProductLineCost), invoice.GetC_Currency_ID(), acctSchema.GetC_Currency_ID(),
                                                                     invoice.GetDateAcct(), invoice.GetC_ConversionType_ID(), AD_Client_ID, AD_Org_ID);
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
        /// This Method is used to update urrent Cost Price on Cos Queue
        /// </summary>
        /// <param name="CostQueue_ID">Cost Queue ID</param>
        /// <param name="oldPrice">Old Price</param>
        /// <param name="qty">Quantity</param>
        /// <param name="newPrice">New Price</param>
        /// <param name="InOutLine_ID">GRN Lline</param>
        /// <param name="precision">Precision</param>
        /// <returns>true, when updated</returns>
        public static bool UpdateCostQueuePrice(int CostQueue_ID, Decimal oldPrice, Decimal qty, Decimal newPrice, int InOutLine_ID, int precision)
        {
            Decimal price = Decimal.Subtract(Decimal.Multiply(newPrice, qty), Decimal.Multiply(oldPrice, qty));
            int no = DB.ExecuteQuery("UPDATE M_CostQueue SET CurrentCostPrice =ROUND(((CurrentCostPrice * CurrentQty) + " + price +
                            @")/ (CASE WHEN CurrentQty = 0 THEN 1 ELSE CurrentQty END) ," + precision + @" )
                            WHERE M_CostQueue_ID IN (SELECT M_CostQueue_ID FROM M_CostQueueTransaction WHERE M_InOutLine_ID
                            = " + InOutLine_ID + ")");
            if (no < 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// is used to get price from price list and convert it into "currency to" with default "currency type" and "acct date" of Inout
        /// </summary>
        /// <param name="inoutline">Line reference</param>
        /// <param name="CurrencyTo">converted currency</param>
        /// <returns>amount</returns>
        public static Decimal GetPrice(MInOutLine inoutline, int CurrencyTo)
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
        public static Decimal GetPrice(MInOut inout, MInOutLine inoutline, int CurrencyTo)
        {
            decimal amount = 0;
            int c_currency_id = 0;
            DataSet dsStdPrice = new DataSet();
            if (inout == null)
            {
                inout = new MInOut(inoutline.GetCtx(), inoutline.GetM_InOut_ID(), inoutline.Get_Trx());
            }
            MBPartner bpartner = new MBPartner(inout.GetCtx(), inout.GetC_BPartner_ID(), inout.Get_Trx());
            if (bpartner.GetPO_PriceList_ID() > 0)
            {
                MPriceList pl = MPriceList.Get(bpartner.GetCtx(), bpartner.GetPO_PriceList_ID(), bpartner.Get_Trx());
                c_currency_id = pl.GetC_Currency_ID();
            }
            string sql = "";
            try
            {
                if (bpartner.GetPO_PriceList_ID() > 0)
                {
                    #region when price list available at Customer/Vendor
                    sql = @"SELECT pricestd FROM m_productprice WHERE isactive = 'Y' AND m_product_id = " + inoutline.GetM_Product_ID() + " AND NVL(m_attributesetinstance_id, 0) = " + inoutline.GetM_AttributeSetInstance_ID() +
                        " AND c_uom_id = " + inoutline.GetC_UOM_ID() + " AND m_pricelist_version_id =   (SELECT MAX(plv.m_pricelist_version_id)   FROM m_pricelist pl   INNER JOIN m_pricelist_version plv " +
                                " ON pl.m_pricelist_id    = plv.m_pricelist_id   WHERE plv.IsActive = 'Y' AND  pl.m_pricelist_id = " + bpartner.GetPO_PriceList_ID() + ")";
                    dsStdPrice = DB.ExecuteDataset(sql, null, null);
                    if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                    {
                        amount = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                    }
                    if (amount == 0)
                    {
                        sql = @"(SELECT pl.AD_Org_ID , pl.name , pl.m_pricelist_id  , plv.m_pricelist_version_id , pp.pricestd , pl.c_currency_id
                                           FROM m_pricelist pl  INNER JOIN m_pricelist_version plv   ON pl.m_pricelist_id    = plv.m_pricelist_id
                                            inner join m_productprice pp   on pp.m_pricelist_version_id = plv.m_pricelist_version_id
                                           WHERE pl.AD_Org_ID = " + inoutline.GetAD_Org_ID() + " AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.isactive = 'Y' and pp.m_product_id = " + inoutline.GetM_Product_ID() +
                            " AND NVL(pp.m_attributesetinstance_id, 0) = " + inoutline.GetM_AttributeSetInstance_ID() +
                            " AND pp.c_uom_id = " + inoutline.GetC_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.m_pricelist_id asc";
                        dsStdPrice = DB.ExecuteDataset(sql, null, null);
                        if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                        {
                            amount = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                            c_currency_id = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["c_currency_id"]);
                        }
                        else
                        {
                            sql = @"(SELECT pl.AD_Org_ID , pl.name , pl.m_pricelist_id  , plv.m_pricelist_version_id , pp.pricestd , pl.c_currency_id
                                           FROM m_pricelist pl  INNER JOIN m_pricelist_version plv   ON pl.m_pricelist_id    = plv.m_pricelist_id
                                            inner join m_productprice pp   on pp.m_pricelist_version_id = plv.m_pricelist_version_id
                                           WHERE pl.AD_Org_ID = 0 AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.isactive = 'Y' and pp.m_product_id = " + inoutline.GetM_Product_ID() +
                                   " AND NVL(pp.m_attributesetinstance_id, 0) = " + inoutline.GetM_AttributeSetInstance_ID() +
                                   " AND pp.c_uom_id = " + inoutline.GetC_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.m_pricelist_id asc , pl.AD_Org_ID desc";
                            dsStdPrice = DB.ExecuteDataset(sql, null, null);
                            if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                            {
                                amount = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                                c_currency_id = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["c_currency_id"]);
                            }
                            else
                            {
                                sql = @"(SELECT pl.AD_Org_ID , pl.name , pl.m_pricelist_id  , plv.m_pricelist_version_id , pp.pricestd , pl.c_currency_id
                                           FROM m_pricelist pl  INNER JOIN m_pricelist_version plv   ON pl.m_pricelist_id    = plv.m_pricelist_id
                                            inner join m_productprice pp   on pp.m_pricelist_version_id = plv.m_pricelist_version_id
                                           WHERE pl.AD_Org_ID = " + inoutline.GetAD_Org_ID() + " AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.isactive = 'Y' and pp.m_product_id = " + inoutline.GetM_Product_ID() +
                               " AND pp.c_uom_id = " + inoutline.GetC_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.m_pricelist_id asc";
                                dsStdPrice = DB.ExecuteDataset(sql, null, null);
                                if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                                {
                                    amount = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                                    c_currency_id = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["c_currency_id"]);
                                }
                                else
                                {
                                    sql = @"(SELECT pl.AD_Org_ID , pl.name , pl.m_pricelist_id  , plv.m_pricelist_version_id , pp.pricestd , pl.c_currency_id
                                           FROM m_pricelist pl  INNER JOIN m_pricelist_version plv   ON pl.m_pricelist_id    = plv.m_pricelist_id
                                            inner join m_productprice pp   on pp.m_pricelist_version_id = plv.m_pricelist_version_id
                                           WHERE pl.AD_Org_ID = 0 AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.isactive = 'Y' and pp.m_product_id = " + inoutline.GetM_Product_ID() +
                                           " AND pp.c_uom_id = " + inoutline.GetC_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.m_pricelist_id asc , pl.AD_Org_ID desc";
                                    dsStdPrice = DB.ExecuteDataset(sql, null, null);
                                    if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                                    {
                                        amount = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                                        c_currency_id = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["c_currency_id"]);
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
                    sql = @"(SELECT pl.AD_Org_ID , pl.name , pl.m_pricelist_id  , plv.m_pricelist_version_id , pp.pricestd , pl.c_currency_id
                                           FROM m_pricelist pl  INNER JOIN m_pricelist_version plv   ON pl.m_pricelist_id    = plv.m_pricelist_id
                                            inner join m_productprice pp   on pp.m_pricelist_version_id = plv.m_pricelist_version_id
                                           WHERE pl.AD_Org_ID = " + inoutline.GetAD_Org_ID() + " AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.isactive = 'Y' and pp.m_product_id = " + inoutline.GetM_Product_ID() +
                           " AND NVL(pp.m_attributesetinstance_id, 0) = " + inoutline.GetM_AttributeSetInstance_ID() +
                           " AND pp.c_uom_id = " + inoutline.GetC_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.m_pricelist_id asc";
                    dsStdPrice = DB.ExecuteDataset(sql, null, null);
                    if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                    {
                        amount = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                        c_currency_id = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["c_currency_id"]);
                    }
                    else
                    {
                        sql = @"(SELECT pl.AD_Org_ID , pl.name , pl.m_pricelist_id  , plv.m_pricelist_version_id , pp.pricestd , pl.c_currency_id
                                           FROM m_pricelist pl  INNER JOIN m_pricelist_version plv   ON pl.m_pricelist_id    = plv.m_pricelist_id
                                            inner join m_productprice pp   on pp.m_pricelist_version_id = plv.m_pricelist_version_id
                                           WHERE pl.AD_Org_ID = 0 AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.isactive = 'Y' and pp.m_product_id = " + inoutline.GetM_Product_ID() +
                               " AND NVL(pp.m_attributesetinstance_id, 0) = " + inoutline.GetM_AttributeSetInstance_ID() +
                               " AND pp.c_uom_id = " + inoutline.GetC_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.m_pricelist_id asc , pl.AD_Org_ID desc";
                        dsStdPrice = DB.ExecuteDataset(sql, null, null);
                        if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                        {
                            amount = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                            c_currency_id = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["c_currency_id"]);
                        }
                        else
                        {
                            sql = @"(SELECT pl.AD_Org_ID , pl.name , pl.m_pricelist_id  , plv.m_pricelist_version_id , pp.pricestd , pl.c_currency_id
                                           FROM m_pricelist pl  INNER JOIN m_pricelist_version plv   ON pl.m_pricelist_id    = plv.m_pricelist_id
                                            inner join m_productprice pp   on pp.m_pricelist_version_id = plv.m_pricelist_version_id
                                           WHERE pl.AD_Org_ID = " + inoutline.GetAD_Org_ID() + " AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.isactive = 'Y' and pp.m_product_id = " + inoutline.GetM_Product_ID() +
                           " AND pp.c_uom_id = " + inoutline.GetC_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.m_pricelist_id asc";
                            dsStdPrice = DB.ExecuteDataset(sql, null, null);
                            if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                            {
                                amount = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                                c_currency_id = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["c_currency_id"]);
                            }
                            else
                            {
                                sql = @"(SELECT pl.AD_Org_ID , pl.name , pl.m_pricelist_id  , plv.m_pricelist_version_id , pp.pricestd , pl.c_currency_id
                                           FROM m_pricelist pl  INNER JOIN m_pricelist_version plv   ON pl.m_pricelist_id    = plv.m_pricelist_id
                                            inner join m_productprice pp   on pp.m_pricelist_version_id = plv.m_pricelist_version_id
                                           WHERE pl.AD_Org_ID = 0 AND pp.pricestd > 0 AND pl.isactive = 'Y' and plv.isactive = 'Y' and pp.isactive = 'Y' and pp.m_product_id = " + inoutline.GetM_Product_ID() +
                                       " AND pp.c_uom_id = " + inoutline.GetC_UOM_ID() + " AND pl.issopricelist = 'N') ORDER BY pl.m_pricelist_id asc , pl.AD_Org_ID desc";
                                dsStdPrice = DB.ExecuteDataset(sql, null, null);
                                if (dsStdPrice != null && dsStdPrice.Tables.Count > 0 && dsStdPrice.Tables[0].Rows.Count > 0)
                                {
                                    amount = Util.GetValueOfDecimal(dsStdPrice.Tables[0].Rows[0]["pricestd"]);
                                    c_currency_id = Util.GetValueOfInt(dsStdPrice.Tables[0].Rows[0]["c_currency_id"]);
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
                if (c_currency_id != 0 && c_currency_id != CurrencyTo)
                {
                    amount = MConversionRate.Convert(inout.GetCtx(), amount, c_currency_id, CurrencyTo,
                                                                            inout.GetDateAcct(), 0, inout.GetAD_Client_ID(), inout.GetAD_Org_ID());
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
        /// <param name="AD_Client_ID">client</param>
        /// <param name="AD_Org_ID">org</param>
        /// <param name="M_Warehouse_ID">warehouse</param>
        /// <returns>PRICE</returns>
        private static Decimal CalculateCost(Ctx ctx, MAcctSchema acctSchema, MProduct product, int M_ASI_ID, int AD_Client_ID, int AD_Org_ID, int M_Warehouse_ID)
        {
            Decimal price = 0;
            int costElementID = 0;
            string sql = "";
            dynamic pc = null;

            pc = MProductCategory.Get(ctx, product.GetM_Product_Category_ID());
            String costingLevel = "";
            try
            {
                if (pc != null && !String.IsNullOrEmpty(pc.GetCostingMethod()))
                {
                    //get cost element frpom product category in case of costing method
                    if (pc.GetCostingMethod() == "C")
                    {
                        costElementID = pc.GetM_CostElement_ID();
                    }
                    else
                    {
                        sql = @"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = '" + pc.GetCostingMethod() + "' AND AD_Client_ID = " + AD_Client_ID;
                        costElementID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    }
                }
                else
                {
                    //get cost element frpom accouting Schema in case of costing method
                    if (acctSchema.GetCostingMethod() == "C")
                    {
                        costElementID = acctSchema.GetM_CostElement_ID();
                    }
                    else
                    {
                        sql = @"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = '" + acctSchema.GetCostingMethod() + "' AND AD_Client_ID = " + AD_Client_ID;
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
                    sql = "SELECT CurrentCostPrice FROM M_Cost WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                           " AND M_Product_ID = " + product.GetM_Product_ID() + " AND NVL(M_AttributeSetInstance_ID , 0) = " + M_ASI_ID + " AND  M_CostElement_ID = " + costElementID +
                           " AND AD_Client_ID = " + AD_Client_ID + " AND AD_Org_ID = 0";
                }
                else if (costingLevel == MProductCategory.COSTINGLEVEL_OrgPlusBatch) // batch + Org
                {
                    sql = "SELECT CurrentCostPrice FROM M_Cost WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                           " AND M_Product_ID = " + product.GetM_Product_ID() + " AND NVL(M_AttributeSetInstance_ID , 0) = " + M_ASI_ID + " AND  M_CostElement_ID = " + costElementID +
                           " AND AD_Client_ID = " + AD_Client_ID + " AND AD_Org_ID = " + AD_Org_ID;
                }
                else if (costingLevel == MProductCategory.COSTINGLEVEL_Client) // client
                {
                    sql = "SELECT CurrentCostPrice FROM M_Cost WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                              " AND M_Product_ID = " + product.GetM_Product_ID() + "  AND M_CostElement_ID = " + costElementID +
                              " AND AD_Client_ID = " + AD_Client_ID + " AND AD_Org_ID = 0";
                }
                else if (costingLevel == MProductCategory.COSTINGLEVEL_Organization) // org
                {
                    sql = "SELECT CurrentCostPrice FROM M_Cost WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                              " AND M_Product_ID = " + product.GetM_Product_ID() + " AND M_CostElement_ID = " + costElementID +
                              " AND AD_Client_ID = " + AD_Client_ID + " AND AD_Org_ID = " + AD_Org_ID;
                }
                else if (costingLevel == MProductCategory.COSTINGLEVEL_Warehouse) // Warehouse + Org
                {
                    sql = "SELECT CurrentCostPrice FROM M_Cost WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                              " AND M_Product_ID = " + product.GetM_Product_ID() + " AND M_CostElement_ID = " + costElementID +
                              " AND AD_Client_ID = " + AD_Client_ID + " AND AD_Org_ID = " + AD_Org_ID + " AND M_Warehouse_ID = " + M_Warehouse_ID;
                }
                else if (costingLevel == MProductCategory.COSTINGLEVEL_WarehousePlusBatch) // Warehouse + batch + Org
                {
                    sql = "SELECT CurrentCostPrice FROM M_Cost WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                           " AND M_Product_ID = " + product.GetM_Product_ID() + " AND NVL(M_AttributeSetInstance_ID , 0) = " + M_ASI_ID + " AND  M_CostElement_ID = " + costElementID +
                           " AND AD_Client_ID = " + AD_Client_ID + " AND AD_Org_ID = " + AD_Org_ID + " AND M_Warehouse_ID = " + M_Warehouse_ID;
                }
                price = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
            }
            catch (Exception ex)
            {
                _log.Info("Error Occured during costing of this method : CalculateCost =>  " + ex.Message.ToString());
            }
            return price;
        }

        private static bool CreateCostQueue(Ctx ctx, MAcctSchema acctSchema, MProduct product, int M_ASI_ID, int AD_Client_ID, int AD_Org_ID, Decimal Price, Decimal Qty,
                               string windowName, MInventoryLine inventoryLine, MInOutLine inoutline, MMovementLine movementline,
                               MInvoiceLine invoiceline, MCostDetail cd, Trx trxName, out string costQueueIds)
        {
            costQueueIds = null;
            //if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM ad_table WHERE lower(tablename) LIKE 't_temp_costdetail'")) <= 0)
            //{
            //    return false;
            //}
            String sql = "";
            dynamic pc = null;
            decimal priceLifo = 0;
            decimal priceFifo = 0;
            string policy = null;
            int M_CostElement_ID = 0;
            MCostElement ce = null;
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
                        pc = MProductCategory.Get(product.GetCtx(), product.GetM_Product_Category_ID());
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
                            pc = MProductCategory.Get(product.GetCtx(), product.GetM_Product_Category_ID());
                            if (pc != null)
                                policy = pc.GetMMPolicy();

                        }
                    }
                    #endregion

                    if (windowName == "Inventory Move")
                    {
                        // is used to get source warehouse, for picking cost from source warehouse
                        M_SourceWarehouse_ID = Convert.ToInt32(DB.ExecuteScalar("SELECT DTD001_MWarehouseSource_ID FROM M_Movement WHERE M_Movement_ID = " + movementline.GetM_Movement_ID(), null, trxName));
                    }

                    if (Price == 0)
                    {
                        #region fifo cost
                        if (cl == MProductCategory.COSTINGLEVEL_BatchLot)
                        {
                            sql = "SELECT CurrentCostPrice FROM M_Cost WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                   " AND M_Product_ID = " + product.GetM_Product_ID() + " AND NVL(M_AttributeSetInstance_ID , 0) = " + M_ASI_ID + " AND  M_CostElement_ID = " +
                                   " ( SELECT MIN(M_CostElement_ID) FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND AD_Client_ID = " + AD_Client_ID + ") " +
                                   " AND AD_Client_ID = " + AD_Client_ID + " AND AD_Org_ID = 0";
                        }
                        else if (cl == MProductCategory.COSTINGLEVEL_Client)
                        {
                            sql = "SELECT CurrentCostPrice FROM M_Cost WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                      " AND M_Product_ID = " + product.GetM_Product_ID() + "  AND M_CostElement_ID = " +
                                      " ( SELECT MIN(M_CostElement_ID) FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND AD_Client_ID = " + AD_Client_ID + ") " +
                                      " AND AD_Client_ID = " + AD_Client_ID + " AND AD_Org_ID = 0";
                        }
                        else if (cl == MProductCategory.COSTINGLEVEL_Organization)
                        {
                            if (windowName == "Inventory Move")
                            {
                                sql = "SELECT CurrentCostPrice FROM M_Cost WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                         " AND M_Product_ID = " + product.GetM_Product_ID() + " AND M_CostElement_ID = " +
                                         " ( SELECT MIN(M_CostElement_ID) FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND AD_Client_ID = " + AD_Client_ID + ") " +
                                         " AND AD_Client_ID = " + AD_Client_ID + " AND AD_Org_ID = " + movementline.GetAD_Org_ID();
                            }
                            else
                            {
                                sql = "SELECT CurrentCostPrice FROM M_Cost WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                          " AND M_Product_ID = " + product.GetM_Product_ID() + " AND M_CostElement_ID = " +
                                          " ( SELECT MIN(M_CostElement_ID) FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND AD_Client_ID = " + AD_Client_ID + ") " +
                                          " AND AD_Client_ID = " + AD_Client_ID + " AND AD_Org_ID = " + AD_Org_ID;
                            }
                        }
                        else if (cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch || cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                        {
                            if (windowName == "Inventory Move")
                            {
                                sql = "SELECT CurrentCostPrice FROM M_Cost WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                    " AND M_Product_ID = " + product.GetM_Product_ID() + " AND NVL(M_AttributeSetInstance_ID , 0) = " + (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID) +
                                    " AND M_CostElement_ID = " +
                                         " ( SELECT MIN(M_CostElement_ID) FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND AD_Client_ID = " + AD_Client_ID + ") " +
                                         " AND AD_Client_ID = " + AD_Client_ID + " AND AD_Org_ID = " + movementline.GetAD_Org_ID();
                                if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                {
                                    sql += " AND M_Warehouse_ID = " + M_SourceWarehouse_ID;//cd.GetM_Warehouse_ID();
                                }
                            }
                            else
                            {
                                sql = "SELECT CurrentCostPrice FROM M_Cost WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                          " AND M_Product_ID = " + product.GetM_Product_ID() + " AND NVL(M_AttributeSetInstance_ID , 0) = " + (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID) + " AND M_CostElement_ID = " +
                                          " ( SELECT MIN(M_CostElement_ID) FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND AD_Client_ID = " + AD_Client_ID + ") " +
                                          " AND AD_Client_ID = " + AD_Client_ID + " AND AD_Org_ID = " + AD_Org_ID;
                                if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                {
                                    sql += " AND M_Warehouse_ID = " + cd.GetM_Warehouse_ID();
                                }
                            }
                        }
                        priceFifo = Util.GetValueOfDecimal(DB.ExecuteScalar(sql));

                        #region on fly calculation
                        if (priceFifo <= 0)
                        {
                            if (cl == MProductCategory.COSTINGLEVEL_BatchLot)
                            {
                                //sql = "SELECT ROUND(sum(currentcostprice * currentqty)/ sum(currentqty) , " + acctSchema.GetCostingPrecision() + @") FROM m_costqueue WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                //       " AND M_Product_ID = " + product.GetM_Product_ID() + " AND NVL(M_AttributeSetInstance_ID , 0) = " + M_ASI_ID + " AND  M_CostElement_ID = " +
                                //       " ( SELECT MIN(M_CostElement_ID) FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND AD_Client_ID = " + AD_Client_ID + ") " +
                                //       " AND AD_Client_ID = " + AD_Client_ID;
                                priceFifo = GetLifoAndFifoCurrentCostFromCostQueue(AD_Client_ID, 0, product.GetM_Product_ID(), M_ASI_ID, acctSchema, 0, true, cd.Get_Trx());
                            }
                            else if (cl == MProductCategory.COSTINGLEVEL_Client)
                            {
                                //sql = "SELECT ROUND(sum(currentcostprice * currentqty)/ sum(currentqty) , " + acctSchema.GetCostingPrecision() + @") FROM m_costqueue WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                //          " AND M_Product_ID = " + product.GetM_Product_ID() + "  AND M_CostElement_ID = " +
                                //          " ( SELECT MIN(M_CostElement_ID) FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND AD_Client_ID = " + AD_Client_ID + ") " +
                                //          " AND AD_Client_ID = " + AD_Client_ID;
                                priceFifo = GetLifoAndFifoCurrentCostFromCostQueue(AD_Client_ID, 0, product.GetM_Product_ID(), 0, acctSchema, 0, true, cd.Get_Trx());
                            }
                            else if (cl == MProductCategory.COSTINGLEVEL_Organization)
                            {
                                if (windowName == "Inventory Move")
                                {
                                    //sql = "SELECT ROUND(sum(currentcostprice * currentqty)/ sum(currentqty) , " + acctSchema.GetCostingPrecision() + @") FROM m_costqueue WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                    //         " AND M_Product_ID = " + product.GetM_Product_ID() + " AND M_CostElement_ID = " +
                                    //         " ( SELECT MIN(M_CostElement_ID) FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND AD_Client_ID = " + AD_Client_ID + ") " +
                                    //         " AND AD_Client_ID = " + AD_Client_ID + " AND AD_Org_ID = " + movementline.GetAD_Org_ID();
                                    priceFifo = GetLifoAndFifoCurrentCostFromCostQueue(AD_Client_ID, movementline.GetAD_Org_ID(), product.GetM_Product_ID(), 0, acctSchema, 0, true, cd.Get_Trx());
                                }
                                else
                                {
                                    //sql = "SELECT ROUND(sum(currentcostprice * currentqty)/ sum(currentqty) , " + acctSchema.GetCostingPrecision() + @") FROM m_costqueue WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                    //          " AND M_Product_ID = " + product.GetM_Product_ID() + " AND M_CostElement_ID = " +
                                    //          " ( SELECT MIN(M_CostElement_ID) FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND AD_Client_ID = " + AD_Client_ID + ") " +
                                    //          " AND AD_Client_ID = " + AD_Client_ID + " AND AD_Org_ID = " + AD_Org_ID;
                                    priceFifo = GetLifoAndFifoCurrentCostFromCostQueue(AD_Client_ID, AD_Org_ID, product.GetM_Product_ID(), 0, acctSchema, 0, true, cd.Get_Trx());
                                }
                            }
                            else if (cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch || cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                            {
                                if (windowName == "Inventory Move")
                                {
                                    //sql = "SELECT ROUND(sum(currentcostprice * currentqty)/ sum(currentqty) , " + acctSchema.GetCostingPrecision() + @") FROM m_costqueue WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                    //         " AND M_Product_ID = " + product.GetM_Product_ID() + " AND NVL(M_AttributeSetInstance_ID , 0) = " + (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID) + " AND M_CostElement_ID = " +
                                    //         " ( SELECT MIN(M_CostElement_ID) FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND AD_Client_ID = " + AD_Client_ID + ") " +
                                    //         " AND AD_Client_ID = " + AD_Client_ID + " AND AD_Org_ID = " + movementline.GetAD_Org_ID();
                                    //if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                    //{
                                    //    sql += " AND M_Warehouse_ID = " + M_SourceWarehouse_ID;//cd.GetM_Warehouse_ID();
                                    //}
                                    priceFifo = GetLifoAndFifoCurrentCostFromCostQueue(AD_Client_ID, movementline.GetAD_Org_ID(),
                                        product.GetM_Product_ID(), (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID), acctSchema,
                                        ((cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch) ? M_SourceWarehouse_ID : 0), true, cd.Get_Trx());
                                }
                                else
                                {
                                    //sql = "SELECT ROUND(sum(currentcostprice * currentqty)/ sum(currentqty) , " + acctSchema.GetCostingPrecision() + @") FROM m_costqueue WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                    //          " AND M_Product_ID = " + product.GetM_Product_ID() + " AND NVL(M_AttributeSetInstance_ID , 0) = " + (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID) + " AND M_CostElement_ID = " +
                                    //          " ( SELECT MIN(M_CostElement_ID) FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND AD_Client_ID = " + AD_Client_ID + ") " +
                                    //          " AND AD_Client_ID = " + AD_Client_ID + " AND AD_Org_ID = " + AD_Org_ID;
                                    //if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                    //{
                                    //    sql += " AND M_Warehouse_ID = " + cd.GetM_Warehouse_ID();
                                    //}
                                    priceFifo = GetLifoAndFifoCurrentCostFromCostQueue(AD_Client_ID, AD_Org_ID,
                                       product.GetM_Product_ID(), (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID), acctSchema,
                                       ((cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch) ? cd.GetM_Warehouse_ID() : 0), true, cd.Get_Trx());
                                }
                            }
                            //priceFifo = Util.GetValueOfDecimal(DB.ExecuteScalar(sql));
                            priceFifo = Decimal.Round(priceFifo, acctSchema.GetCostingPrecision());
                        }
                        #endregion
                        #endregion

                        #region lifo cost
                        if (cl == MProductCategory.COSTINGLEVEL_BatchLot)
                        {
                            sql = "SELECT CurrentCostPrice FROM M_Cost WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                   " AND M_Product_ID = " + product.GetM_Product_ID() + " AND NVL(M_AttributeSetInstance_ID , 0) = " + M_ASI_ID + " AND  M_CostElement_ID = " +
                                   " ( SELECT MIN(M_CostElement_ID) FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND AD_Client_ID = " + AD_Client_ID + ") " +
                                   " AND AD_Client_ID = " + AD_Client_ID;
                        }
                        else if (cl == MProductCategory.COSTINGLEVEL_Client)
                        {
                            sql = "SELECT CurrentCostPrice FROM M_Cost WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                      " AND M_Product_ID = " + product.GetM_Product_ID() + "  AND M_CostElement_ID = " +
                                      " ( SELECT MIN(M_CostElement_ID) FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND AD_Client_ID = " + AD_Client_ID + ") " +
                                      " AND AD_Client_ID = " + AD_Client_ID;
                        }
                        else if (cl == MProductCategory.COSTINGLEVEL_Organization)
                        {
                            if (windowName == "Inventory Move")
                            {
                                sql = "SELECT CurrentCostPrice FROM M_Cost WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                         " AND M_Product_ID = " + product.GetM_Product_ID() + " AND M_CostElement_ID = " +
                                         " ( SELECT MIN(M_CostElement_ID) FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND AD_Client_ID = " + AD_Client_ID + ") " +
                                         " AND AD_Client_ID = " + AD_Client_ID + " AND AD_Org_ID = " + movementline.GetAD_Org_ID();
                            }
                            else
                            {
                                sql = "SELECT CurrentCostPrice FROM M_Cost WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                          " AND M_Product_ID = " + product.GetM_Product_ID() + " AND M_CostElement_ID = " +
                                          " ( SELECT MIN(M_CostElement_ID) FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND AD_Client_ID = " + AD_Client_ID + ") " +
                                          " AND AD_Client_ID = " + AD_Client_ID + " AND AD_Org_ID = " + AD_Org_ID;
                            }
                        }
                        else if (cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch || cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                        {
                            if (windowName == "Inventory Move")
                            {
                                sql = "SELECT CurrentCostPrice FROM M_Cost WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                         " AND M_Product_ID = " + product.GetM_Product_ID() + " AND NVL(M_AttributeSetInstance_ID , 0) = " + (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID) + " AND M_CostElement_ID = " +
                                         " ( SELECT MIN(M_CostElement_ID) FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND AD_Client_ID = " + AD_Client_ID + ") " +
                                         " AND AD_Client_ID = " + AD_Client_ID + " AND AD_Org_ID = " + movementline.GetAD_Org_ID();
                                if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                {
                                    sql += " AND M_Warehouse_ID = " + M_SourceWarehouse_ID;//cd.GetM_Warehouse_ID();
                                }
                            }
                            else
                            {
                                sql = "SELECT CurrentCostPrice FROM M_Cost WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                          " AND M_Product_ID = " + product.GetM_Product_ID() + " AND NVL(M_AttributeSetInstance_ID , 0) = " + (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID) + " AND M_CostElement_ID = " +
                                          " ( SELECT MIN(M_CostElement_ID) FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND AD_Client_ID = " + AD_Client_ID + ") " +
                                          " AND AD_Client_ID = " + AD_Client_ID + " AND AD_Org_ID = " + AD_Org_ID;
                                if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                {
                                    sql += " AND M_Warehouse_ID = " + cd.GetM_Warehouse_ID();
                                }
                            }
                        }
                        priceLifo = Util.GetValueOfDecimal(DB.ExecuteScalar(sql));

                        #region on fly calculation from cost queue
                        if (priceLifo <= 0)
                        {
                            if (cl == MProductCategory.COSTINGLEVEL_BatchLot)
                            {
                                //sql = "SELECT ROUND(sum(currentcostprice * currentqty)/ sum(currentqty) , " + acctSchema.GetCostingPrecision() + @") FROM M_CostQueue WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                //       " AND M_Product_ID = " + product.GetM_Product_ID() + " AND NVL(M_AttributeSetInstance_ID , 0) = " + M_ASI_ID + " AND  M_CostElement_ID = " +
                                //       " ( SELECT MIN(M_CostElement_ID) FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND AD_Client_ID = " + AD_Client_ID + ") " +
                                //       " AND AD_Client_ID = " + AD_Client_ID;
                                priceLifo = GetLifoAndFifoCurrentCostFromCostQueue(AD_Client_ID, 0, product.GetM_Product_ID(), M_ASI_ID, acctSchema, 0, false, cd.Get_Trx());
                            }
                            else if (cl == MProductCategory.COSTINGLEVEL_Client)
                            {
                                //sql = "SELECT ROUND(sum(currentcostprice * currentqty)/ sum(currentqty) , " + acctSchema.GetCostingPrecision() + @") FROM M_CostQueue WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                //          " AND M_Product_ID = " + product.GetM_Product_ID() + "  AND M_CostElement_ID = " +
                                //          " ( SELECT MIN(M_CostElement_ID) FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND AD_Client_ID = " + AD_Client_ID + ") " +
                                //          " AND AD_Client_ID = " + AD_Client_ID;
                                priceLifo = GetLifoAndFifoCurrentCostFromCostQueue(AD_Client_ID, 0, product.GetM_Product_ID(), 0, acctSchema, 0, false, cd.Get_Trx());
                            }
                            else if (cl == MProductCategory.COSTINGLEVEL_Organization)
                            {
                                if (windowName == "Inventory Move")
                                {
                                    //sql = "SELECT ROUND(sum(currentcostprice * currentqty)/ sum(currentqty) , " + acctSchema.GetCostingPrecision() + @") FROM M_CostQueue WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                    //         " AND M_Product_ID = " + product.GetM_Product_ID() + " AND M_CostElement_ID = " +
                                    //         " ( SELECT MIN(M_CostElement_ID) FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND AD_Client_ID = " + AD_Client_ID + ") " +
                                    //         " AND AD_Client_ID = " + AD_Client_ID + " AND AD_Org_ID = " + movementline.GetAD_Org_ID();
                                    priceLifo = GetLifoAndFifoCurrentCostFromCostQueue(AD_Client_ID, movementline.GetAD_Org_ID(), product.GetM_Product_ID(), 0, acctSchema, 0, false, cd.Get_Trx());
                                }
                                else
                                {
                                    //sql = "SELECT ROUND(sum(currentcostprice * currentqty)/ sum(currentqty) , " + acctSchema.GetCostingPrecision() + @") FROM M_CostQueue WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                    //          " AND M_Product_ID = " + product.GetM_Product_ID() + " AND M_CostElement_ID = " +
                                    //          " ( SELECT MIN(M_CostElement_ID) FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND AD_Client_ID = " + AD_Client_ID + ") " +
                                    //          " AND AD_Client_ID = " + AD_Client_ID + " AND AD_Org_ID = " + AD_Org_ID;
                                    priceLifo = GetLifoAndFifoCurrentCostFromCostQueue(AD_Client_ID, AD_Org_ID, product.GetM_Product_ID(), 0, acctSchema, 0, false, cd.Get_Trx());
                                }
                            }
                            else if (cl == MProductCategory.COSTINGLEVEL_OrgPlusBatch || cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                            {
                                if (windowName == "Inventory Move")
                                {
                                    //sql = "SELECT ROUND(sum(currentcostprice * currentqty)/ sum(currentqty) , " + acctSchema.GetCostingPrecision() + @") FROM M_CostQueue WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                    //         " AND M_Product_ID = " + product.GetM_Product_ID() + " AND NVL(M_AttributeSetInstance_ID , 0) = " + (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID) + " AND M_CostElement_ID = " +
                                    //         " ( SELECT MIN(M_CostElement_ID) FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND AD_Client_ID = " + AD_Client_ID + ") " +
                                    //         " AND AD_Client_ID = " + AD_Client_ID + " AND AD_Org_ID = " + movementline.GetAD_Org_ID();
                                    //if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                    //{
                                    //    sql += " AND M_Warehouse_ID = " + M_SourceWarehouse_ID;// cd.GetM_Warehouse_ID();
                                    //}
                                    priceLifo = GetLifoAndFifoCurrentCostFromCostQueue(AD_Client_ID, movementline.GetAD_Org_ID(),
                                      product.GetM_Product_ID(), (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID), acctSchema,
                                      ((cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch) ? M_SourceWarehouse_ID : 0), false, cd.Get_Trx());
                                }
                                else
                                {
                                    //sql = "SELECT ROUND(sum(currentcostprice * currentqty)/ sum(currentqty) , " + acctSchema.GetCostingPrecision() + @") FROM M_CostQueue WHERE IsActive = 'Y' AND C_AcctSchema_ID = " + acctSchema.GetC_AcctSchema_ID() +
                                    //          " AND M_Product_ID = " + product.GetM_Product_ID() + " AND NVL(M_AttributeSetInstance_ID , 0) = " + (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID) + " AND M_CostElement_ID = " +
                                    //          " ( SELECT MIN(M_CostElement_ID) FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND AD_Client_ID = " + AD_Client_ID + ") " +
                                    //          " AND AD_Client_ID = " + AD_Client_ID + " AND AD_Org_ID = " + AD_Org_ID;
                                    //if (cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch)
                                    //{
                                    //    sql += " AND M_Warehouse_ID = " + cd.GetM_Warehouse_ID();
                                    //}
                                    priceLifo = GetLifoAndFifoCurrentCostFromCostQueue(AD_Client_ID, AD_Org_ID,
                                      product.GetM_Product_ID(), (cl == MProductCategory.COSTINGLEVEL_Warehouse ? 0 : M_ASI_ID), acctSchema,
                                      ((cl == MProductCategory.COSTINGLEVEL_Warehouse || cl == MProductCategory.COSTINGLEVEL_WarehousePlusBatch) ? cd.GetM_Warehouse_ID() : 0), false, cd.Get_Trx());
                                }
                            }
                            //priceLifo = Util.GetValueOfDecimal(DB.ExecuteScalar(sql));
                            priceLifo = Decimal.Round(priceLifo, acctSchema.GetCostingPrecision());
                        }
                        #endregion

                        #endregion
                    }
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
            MCostElement costElement = null;
            X_T_Temp_CostDetail tempCostDetail = null;
            try
            {
                #region Ist Entry Either FIFO or LIFO
                MCostQueue costQueue = new MCostQueue(ctx, 0, trxName);
                costQueue.SetAD_Client_ID(AD_Client_ID);
                if (windowName == "Physical Inventory" || windowName == "Internal Use Inventory")
                {
                    AD_Org_ID = inventoryLine.GetAD_Org_ID();
                }
                //else if (windowName == "Inventory Move")
                //{
                //    AD_Org_ID = movementline.GetAD_Org_ID();
                //}
                else if (windowName == "Material Receipt" || windowName == "Shipment" || windowName == "Customer Return" || windowName == "Return To Vendor")
                {
                    AD_Org_ID = inoutline.GetAD_Org_ID();
                }
                else if (windowName == "Invoice(Vendor)")
                {
                    AD_Org_ID = invoiceline.GetAD_Org_ID();
                }
                costQueue.SetAD_Org_ID(AD_Org_ID);
                costQueue.SetC_AcctSchema_ID(Util.GetValueOfInt(acctSchema.GetC_AcctSchema_ID()));
                costQueue.SetM_CostType_ID(acctSchema.GetM_CostType_ID());
                costQueue.SetM_Product_ID(product.GetM_Product_ID());
                sql = @"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = " +
                    " ( SELECT MMPolicy FROM M_Product_Category WHERE IsActive = 'Y' AND M_Product_Category_ID = " +
                    " (SELECT M_Product_Category_ID FROM M_Product WHERE IsActive = 'Y' AND M_Product_ID = " + product.GetM_Product_ID() + " )) AND AD_Client_ID = " + AD_Client_ID;
                M_CostElement_ID = (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null)));
                if (M_CostElement_ID == 0)
                {
                    _log.Info("Cost Element missing");
                    return false;
                }
                costQueue.SetM_CostElement_ID(M_CostElement_ID);
                costQueue.SetM_AttributeSetInstance_ID(M_ASI_ID);
                costQueue.SetM_Warehouse_ID(cd.GetM_Warehouse_ID());
                costQueue.SetCurrentQty(Qty);
                costQueue.SetActualQty(Qty);
                // change 2-5-2016
                if (Price != 0)
                {
                    ce = MCostElement.Get(ctx, M_CostElement_ID);
                    amtWithSurcharge = Decimal.Add(Price, Decimal.Round(Decimal.Divide(Decimal.Multiply(Price, ce.GetSurchargePercentage()), 100), acctSchema.GetCostingPrecision()));
                    costQueue.SetCurrentCostPrice(amtWithSurcharge);
                }
                else if (policy == "F" && priceFifo > 0)
                {
                    costQueue.SetCurrentCostPrice(priceFifo);
                }
                else if (policy == "L" && priceLifo > 0)
                {
                    costQueue.SetCurrentCostPrice(priceLifo);
                }

                //end
                costQueue.SetQueueDate(System.DateTime.Now.ToLocalTime());
                if (!costQueue.Save())
                {
                    ValueNamePair pp = VLogger.RetrieveError();
                    _log.Info("Cost Queue not saved for  <===> " + product.GetM_Product_ID() + " Error Type is : " + (pp != null ? pp.GetName() : ""));
                    return false;
                }
                else
                {
                    // Create Cost Queue Transactional Record
                    if (!MCostQueueTransaction.CreateCostQueueTransaction(ctx, AD_Client_ID, AD_Org_ID, costQueue.GetM_CostQueue_ID(), cd, Qty))
                    {
                        return false;
                    }

                    costQueueIds += costQueue.GetM_CostQueue_ID();
                    tempCostDetail = new X_T_Temp_CostDetail(ctx, 0, null);
                    tempCostDetail.SetAD_Client_ID(AD_Client_ID);
                    tempCostDetail.SetAD_Org_ID(AD_Org_ID);
                    tempCostDetail.SetM_CostDetail_ID(cd.GetM_CostDetail_ID());
                    tempCostDetail.SetM_CostQueue_ID(costQueue.GetM_CostQueue_ID());
                    tempCostDetail.SetC_AcctSchema_ID(Util.GetValueOfInt(acctSchema.GetC_AcctSchema_ID()));
                    if (invoiceline != null && invoiceline.GetC_InvoiceLine_ID() > 0)
                        tempCostDetail.SetC_InvoiceLine_ID(invoiceline.GetC_InvoiceLine_ID());
                    if (inoutline != null && inoutline.GetC_OrderLine_ID() > 0)
                        tempCostDetail.SetC_OrderLine_ID(inoutline.GetC_OrderLine_ID());
                    if (inoutline != null && inoutline.GetM_InOutLine_ID() > 0)
                        tempCostDetail.SetM_InOutLine_ID(inoutline.GetM_InOutLine_ID());
                    if (inventoryLine != null && inventoryLine.GetM_InventoryLine_ID() > 0)
                        tempCostDetail.SetM_InventoryLine_ID(inventoryLine.GetM_InventoryLine_ID());
                    if (movementline != null && movementline.GetM_MovementLine_ID() > 0)
                        tempCostDetail.SetM_MovementLine_ID(movementline.GetM_MovementLine_ID());
                    //if (inoutline != null && inoutline.GetC_OrderLine_ID() == 0)
                    //    tempCostDetail.SetisRecordFromForm(true);
                    tempCostDetail.SetM_Product_ID(product.GetM_Product_ID());
                    tempCostDetail.SetM_AttributeSetInstance_ID(M_ASI_ID);
                    tempCostDetail.SetM_Warehouse_ID(cd.GetM_Warehouse_ID());
                    tempCostDetail.Set_Value("VAFAM_AssetDisposal_ID", cd.Get_Value("VAFAM_AssetDisposal_ID"));

                    // change 2-5-2016
                    if (amtWithSurcharge != 0)
                    {
                        tempCostDetail.SetAmt(amtWithSurcharge);
                    }
                    else if (policy == "F" && priceFifo > 0)
                    {
                        policy = "L";
                        tempCostDetail.SetAmt(priceFifo);
                    }
                    else if (policy == "L" && priceLifo > 0)
                    {
                        policy = "F";
                        tempCostDetail.SetAmt(priceLifo);
                    }

                    //end
                    //tempCostDetail.SetC_Currency_ID(acctSchema.GetC_Currency_ID());
                    tempCostDetail.Save();
                }
                #endregion

                int queueRecordId = costQueue.GetM_CostQueue_ID();

                #region 2nd Entry Either FIFO or LIFO opposite of 1st entry
                costElement = MCostElement.Get(ctx, costQueue.GetM_CostElement_ID());
                if (costElement.GetCostingMethod() == "F")
                {
                    sql = @"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND AD_Client_ID = " + AD_Client_ID;
                }
                else
                {
                    sql = @"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND AD_Client_ID = " + AD_Client_ID;
                }
                costQueue = new MCostQueue(ctx, 0, trxName);
                costQueue.SetAD_Client_ID(AD_Client_ID);
                costQueue.SetAD_Org_ID(AD_Org_ID);
                costQueue.SetC_AcctSchema_ID(Util.GetValueOfInt(acctSchema.GetC_AcctSchema_ID()));
                costQueue.SetM_CostType_ID(acctSchema.GetM_CostType_ID());
                costQueue.SetM_Product_ID(product.GetM_Product_ID());
                M_CostElement_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                if (M_CostElement_ID == 0)
                {
                    _log.Info("Cost Element missing");
                    return false;
                }
                costQueue.SetM_CostElement_ID(M_CostElement_ID);
                costQueue.SetM_AttributeSetInstance_ID(M_ASI_ID);
                costQueue.SetM_Warehouse_ID(cd.GetM_Warehouse_ID());
                costQueue.SetCurrentQty(Qty);
                costQueue.SetActualQty(Qty);
                // change 2-5-2016
                if (Price != 0)
                {
                    ce = MCostElement.Get(ctx, M_CostElement_ID);
                    amtWithSurcharge = Decimal.Add(Price, Decimal.Round(Decimal.Divide(Decimal.Multiply(Price, ce.GetSurchargePercentage()), 100), acctSchema.GetCostingPrecision()));
                    costQueue.SetCurrentCostPrice(amtWithSurcharge);
                }
                else if (policy == "F" && priceFifo > 0)
                {
                    costQueue.SetCurrentCostPrice(priceFifo);
                }
                else if (policy == "L" && priceLifo > 0)
                {
                    costQueue.SetCurrentCostPrice(priceLifo);
                }

                //end
                costQueue.SetQueueDate(System.DateTime.Now.ToLocalTime());
                if (!costQueue.Save())
                {
                    // delete record if created 
                    if (queueRecordId > 0)
                        DB.ExecuteQuery("DELETE FROM M_CostQueue WHERE M_CostQueue_ID = " + queueRecordId, null, trxName);
                    ValueNamePair pp = VLogger.RetrieveError();
                    _log.Info("Cost Queue not saved for  <===> " + product.GetM_Product_ID() + " Error Type is : " + (pp != null ? pp.GetName() : ""));
                    return false;
                }
                else
                {
                    // Create Cost Queue Transactional Record
                    if (!MCostQueueTransaction.CreateCostQueueTransaction(ctx, AD_Client_ID, AD_Org_ID, costQueue.GetM_CostQueue_ID(), cd, Qty))
                    {
                        return false;
                    }

                    costQueueIds += " , " + costQueue.GetM_CostQueue_ID();
                    tempCostDetail = new X_T_Temp_CostDetail(ctx, 0, null);
                    tempCostDetail.SetAD_Client_ID(AD_Client_ID);
                    tempCostDetail.SetAD_Org_ID(AD_Org_ID);
                    tempCostDetail.SetC_AcctSchema_ID(Util.GetValueOfInt(acctSchema.GetC_AcctSchema_ID()));
                    tempCostDetail.SetM_CostDetail_ID(cd.GetM_CostDetail_ID());
                    tempCostDetail.SetM_CostQueue_ID(costQueue.GetM_CostQueue_ID());
                    if (invoiceline != null && invoiceline.GetC_InvoiceLine_ID() > 0)
                        tempCostDetail.SetC_InvoiceLine_ID(invoiceline.GetC_InvoiceLine_ID());
                    if (inoutline != null && inoutline.GetC_OrderLine_ID() > 0)
                        tempCostDetail.SetC_OrderLine_ID(inoutline.GetC_OrderLine_ID());
                    if (inoutline != null && inoutline.GetM_InOutLine_ID() > 0)
                        tempCostDetail.SetM_InOutLine_ID(inoutline.GetM_InOutLine_ID());
                    if (inventoryLine != null && inventoryLine.GetM_InventoryLine_ID() > 0)
                        tempCostDetail.SetM_InventoryLine_ID(inventoryLine.GetM_InventoryLine_ID());
                    if (movementline != null && movementline.GetM_MovementLine_ID() > 0)
                        tempCostDetail.SetM_MovementLine_ID(movementline.GetM_MovementLine_ID());
                    //if (inoutline != null && inoutline.GetC_OrderLine_ID() == 0)
                    //    tempCostDetail.SetisRecordFromForm(true);
                    tempCostDetail.SetM_Product_ID(product.GetM_Product_ID());
                    tempCostDetail.SetM_AttributeSetInstance_ID(M_ASI_ID);
                    tempCostDetail.SetM_Warehouse_ID(cd.GetM_Warehouse_ID());
                    // change 2-5-2016
                    if (amtWithSurcharge != 0)
                    {
                        tempCostDetail.SetAmt(amtWithSurcharge);
                    }
                    else if (policy == "F" && priceFifo > 0)
                    {
                        tempCostDetail.SetAmt(priceFifo);
                    }
                    else if (policy == "L" && priceLifo > 0)
                    {
                        tempCostDetail.SetAmt(priceLifo);
                    }

                    //end
                    //tempCostDetail.SetC_Currency_ID(acctSchema.GetC_Currency_ID());
                    tempCostDetail.Save();
                }
                #endregion

            }
            catch (Exception ex)
            {
                ValueNamePair pp = VLogger.RetrieveError();
                _log.Info("Error occured Cost Queue not saved. Error Value :  " + (pp != null ? pp.GetValue() : "")
                    + " AND Error Name : " + (pp != null ? pp.GetName() : "") +
                           " And Exception message : " + ex.Message.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// is used to create entry for Cost Queue
        /// also we are doing entry in temp table "T_Temp_CostDetail" -- for re-updation of cost when we received invoice
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="acctSchema">accounting schema</param>
        /// <param name="product">product</param>
        /// <param name="M_ASI_ID">Attribute set instance</param>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="AD_Org_ID">org</param>
        /// <param name="Price">receiving price</param>
        /// <param name="Qty">qty to be matched</param>
        /// <param name="inoutline">inoutline reference</param>
        /// <param name="trxName">trx</param>
        /// <param name="optionalStrPO">calling from process or manual</param>
        /// <param name="M_Warehouse_ID">warehouse</param>
        /// <returns>TRUE - if success</returns>
        private static bool CreateCostQueueForMatchPO(Ctx ctx, MAcctSchema acctSchema, MProduct product, int M_ASI_ID, int AD_Client_ID,
                                                      int AD_Org_ID, Decimal Price, Decimal Qty, MInOutLine inoutline,
                                                      Trx trxName, int M_Warehouse_ID, string optionalStrPO = "process")
        {
            if (optionalStrPO == "window")
            {
                return true;
            }
            MCostElement costElement = null;
            X_T_Temp_CostDetail tempCostDetail = null;
            int M_CostElement_ID = 0;
            string sql = null;
            decimal amtWithSurcharge = 0;
            try
            {
                #region Ist Entry Either FIFO or LIFO
                MCostQueue costQueue = new MCostQueue(ctx, 0, trxName);
                costQueue.SetAD_Client_ID(AD_Client_ID);
                costQueue.SetAD_Org_ID(AD_Org_ID);
                costQueue.SetC_AcctSchema_ID(Util.GetValueOfInt(acctSchema.GetC_AcctSchema_ID()));
                costQueue.SetM_Warehouse_ID(M_Warehouse_ID);
                costQueue.SetM_CostType_ID(acctSchema.GetM_CostType_ID());
                costQueue.SetM_Product_ID(product.GetM_Product_ID());
                sql = @"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = " +
                    " ( SELECT MMPolicy FROM M_Product_Category WHERE IsActive = 'Y' AND M_Product_Category_ID = " +
                    " (SELECT M_Product_Category_ID FROM M_Product WHERE IsActive = 'Y' AND M_Product_ID = " + product.GetM_Product_ID() + " )) AND AD_Client_ID = " + AD_Client_ID;
                M_CostElement_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, trxName));
                costQueue.SetM_CostElement_ID(M_CostElement_ID);
                costQueue.SetM_AttributeSetInstance_ID(M_ASI_ID);
                costQueue.SetCurrentQty(Qty);

                costElement = MCostElement.Get(ctx, M_CostElement_ID);
                amtWithSurcharge = Decimal.Add(Price, Decimal.Round(Decimal.Divide(Decimal.Multiply(Price, costElement.GetSurchargePercentage()), 100), acctSchema.GetCostingPrecision()));
                costQueue.SetCurrentCostPrice(amtWithSurcharge);

                //costQueue.SetCurrentCostPrice(Price);
                costQueue.SetQueueDate(System.DateTime.Now.ToLocalTime());
                if (!costQueue.Save())
                {
                    ValueNamePair pp = VLogger.RetrieveError();
                    _log.Severe("Cost Queue not saved by CreateCostQueueForMatchPO for this product <===> " + product.GetM_Product_ID() + " Error Type is : " + pp.GetName());
                    return false;
                }
                else
                {
                    // Create Cost Queue Transactional Record
                    //if (!MCostQueueTransaction.CreateCostQueueTransaction(ctx, AD_Client_ID, AD_Org_ID, costQueue.GetM_CostQueue_ID(), cd, Qty))
                    //{
                    //    return false;
                    //}

                    tempCostDetail = new X_T_Temp_CostDetail(ctx, 0, null);
                    tempCostDetail.SetAD_Client_ID(AD_Client_ID);
                    tempCostDetail.SetAD_Org_ID(AD_Org_ID);
                    //tempCostDetail.SetM_CostDetail_ID(cd.GetM_CostDetail_ID());
                    tempCostDetail.SetM_InOutLine_ID(inoutline.GetM_InOutLine_ID());
                    tempCostDetail.SetM_CostQueue_ID(costQueue.GetM_CostQueue_ID());
                    tempCostDetail.SetC_AcctSchema_ID(Util.GetValueOfInt(acctSchema.GetC_AcctSchema_ID()));
                    tempCostDetail.SetM_Warehouse_ID(M_Warehouse_ID);
                    tempCostDetail.SetisRecordFromForm(true);
                    tempCostDetail.SetM_Product_ID(product.GetM_Product_ID());
                    tempCostDetail.SetM_AttributeSetInstance_ID(M_ASI_ID);
                    //tempCostDetail.SetAmt(Price);
                    tempCostDetail.SetAmt(amtWithSurcharge);
                    tempCostDetail.Save();
                }
                #endregion

                #region 2nd Entry Either FIFO or LIFO opposite of 1st entry
                //costElement = new MCostElement(ctx, costQueue.GetM_CostElement_ID(), null);
                if (costElement.GetCostingMethod() == "F")
                {
                    sql = @"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'L' AND AD_Client_ID = " + AD_Client_ID;
                }
                else
                {
                    sql = @"SELECT M_CostElement_ID FROM M_CostElement WHERE IsActive = 'Y' AND CostingMethod = 'F' AND AD_Client_ID = " + AD_Client_ID;
                }
                costQueue = new MCostQueue(ctx, 0, trxName);
                costQueue.SetAD_Client_ID(AD_Client_ID);
                costQueue.SetAD_Org_ID(AD_Org_ID);
                costQueue.SetC_AcctSchema_ID(Util.GetValueOfInt(acctSchema.GetC_AcctSchema_ID()));
                costQueue.SetM_Warehouse_ID(M_Warehouse_ID);
                costQueue.SetM_CostType_ID(acctSchema.GetM_CostType_ID());
                costQueue.SetM_Product_ID(product.GetM_Product_ID());
                M_CostElement_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                costQueue.SetM_CostElement_ID(M_CostElement_ID);
                costQueue.SetM_AttributeSetInstance_ID(M_ASI_ID);
                costQueue.SetCurrentQty(Qty);

                costElement = MCostElement.Get(ctx, M_CostElement_ID);
                amtWithSurcharge = Decimal.Add(Price, Decimal.Round(Decimal.Divide(Decimal.Multiply(Price, costElement.GetSurchargePercentage()), 100), acctSchema.GetCostingPrecision()));
                costQueue.SetCurrentCostPrice(amtWithSurcharge);
                //costQueue.SetCurrentCostPrice(Price);

                costQueue.SetQueueDate(System.DateTime.Now.ToLocalTime());
                if (!costQueue.Save())
                {
                    ValueNamePair pp = VLogger.RetrieveError();
                    _log.Severe("Cost Queue not saved by CreateCostQueueForMatchPO for product  <===> " + product.GetM_Product_ID() + " Error Type is : " + pp.GetName());
                    return false;
                }
                else
                {
                    tempCostDetail = new X_T_Temp_CostDetail(ctx, 0, null);
                    tempCostDetail.SetAD_Client_ID(AD_Client_ID);
                    tempCostDetail.SetAD_Org_ID(AD_Org_ID);
                    tempCostDetail.SetC_AcctSchema_ID(Util.GetValueOfInt(acctSchema.GetC_AcctSchema_ID()));
                    //tempCostDetail.SetM_CostDetail_ID(cd.GetM_CostDetail_ID());
                    tempCostDetail.SetM_InOutLine_ID(inoutline.GetM_InOutLine_ID());
                    tempCostDetail.SetM_CostQueue_ID(costQueue.GetM_CostQueue_ID());
                    tempCostDetail.SetM_Warehouse_ID(M_Warehouse_ID);
                    tempCostDetail.SetisRecordFromForm(true);
                    tempCostDetail.SetM_Product_ID(product.GetM_Product_ID());
                    tempCostDetail.SetM_AttributeSetInstance_ID(M_ASI_ID);

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

        private static void updateCostQueue(MProduct product, int M_ASI_ID, MAcctSchema mas,
         int Org_ID, MCostElement ce, decimal movementQty, int M_Warehouse_ID = 0, MCostDetail cd = null)
        {
            Decimal qty = movementQty;
            Decimal QueueQtyAffected = 0;
            #region Org Specific

            // get Costing level
            String costingLevel = String.Empty;
            costingLevel = MProductCategory.Get(product.GetCtx(), product.GetM_Product_Category_ID()).GetCostingLevel();
            if (String.IsNullOrEmpty(costingLevel))
            {
                costingLevel = mas.GetCostingLevel();
            }
            if (!(costingLevel == MProductCategory.COSTINGLEVEL_Warehouse || costingLevel == MProductCategory.COSTINGLEVEL_WarehousePlusBatch))
            {
                M_Warehouse_ID = 0;
            }
            MCostQueue[] cQueue = MCostQueue.GetQueue(product, M_ASI_ID, mas, Org_ID, ce, product.Get_Trx(), M_Warehouse_ID);
            if (cQueue != null && cQueue.Length > 0)
            {
                bool value = false;
                for (int cq = 0; cq < cQueue.Length; cq++)
                {
                    MCostQueue queue = cQueue[cq];
                    if (queue.GetCurrentQty() < 0) continue;
                    if (queue.GetCurrentQty() > qty)
                    {
                        value = true;
                    }
                    else
                    {
                        value = false;
                    }
                    QueueQtyAffected = queue.GetCurrentQty();
                    qty = MCostQueue.Quantity(queue.GetCurrentQty(), qty);
                    if (qty <= 0)
                    {
                        //queue.Delete(false);
                        queue.SetCurrentQty(0);
                        if (!queue.Save())
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            _log.Severe("Cost Queue not updated as current qty 0, by updateCostQueue for product  <===> " + product.GetM_Product_ID() + " Error Type is : " + (pp != null ? pp.GetName() : ""));
                        }
                        else if (cd != null && QueueQtyAffected != 0)
                        {
                            // Create Cost Queue Transactional Record
                            MCostQueueTransaction.CreateCostQueueTransaction(cd.GetCtx(), queue.GetAD_Client_ID(), queue.GetAD_Org_ID(), queue.GetM_CostQueue_ID(), cd, Decimal.Negate(QueueQtyAffected));
                        }
                        qty = Decimal.Negate(qty);
                    }
                    else
                    {
                        // affected qty which is to be reduce from cost queue
                        QueueQtyAffected -= qty;
                        queue.SetCurrentQty(qty);
                        qty = 0;
                        if (!queue.Save())
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            _log.Severe("Cost Queue not updated by updateCostQueue for product  <===> " + product.GetM_Product_ID() + " Error Type is : " + (pp != null ? pp.GetName() : ""));
                        }
                        else if (cd != null && QueueQtyAffected != 0)
                        {
                            // Create Cost Queue Transactional Record
                            MCostQueueTransaction.CreateCostQueueTransaction(cd.GetCtx(), queue.GetAD_Client_ID(), queue.GetAD_Org_ID(), queue.GetM_CostQueue_ID(), cd, Decimal.Negate(QueueQtyAffected));

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

        /// <summary>
        /// This function is used to add the qty on respective cost queue reference against which we reduce the stock 
        /// </summary>
        /// <param name="cd">cost detail</param>
        /// <param name="windowName">window name</param>
        /// <param name="costElement">cost element</param>
        /// <param name="Qty">qty to be added</param>
        /// <param name="IsReversedDocument">Is Reversal Document</param>
        /// <param name="M_Warehouse_ID">Warehouse ID</param>
        /// <returns>True when success</returns>
        public static bool ReturnStockReAssignedToCostQueue(MCostDetail cd, String windowName, MCostElement costElement, Decimal Qty, bool IsReversedDocument, int M_Warehouse_ID, out bool backwardCompatabilitySupport)
        {
            String sql = "";
            backwardCompatabilitySupport = false;
            sql = @"SELECT  M_CostQueueTransaction.M_CostQueueTransaction_ID, M_CostQueue.M_CostQueue_ID, M_CostQueueTransaction.MovementQty, M_CostQueue.AD_Org_ID
                            FROM M_CostQueue INNER JOIN M_CostQueueTransaction
                            ON M_CostQueue.M_CostQueue_ID = m_costQueuetransaction.M_CostQueue_ID
                            INNER JOIN M_CostElement ON M_CostQueue.M_CostElement_ID = M_CostElement.M_CostElement_ID
                            WHERE  M_CostQueueTransaction.MovementQty <> 0 AND M_CostQueue.M_CostElement_ID = " + costElement.GetM_CostElement_ID() + @"
                            AND M_CostQueue.C_ACCTSCHEMA_ID = " + cd.GetC_AcctSchema_ID();

            if (windowName.Equals("Customer Return") ||
                (windowName.Equals("Return To Vendor") && cd.GetC_OrderLine_ID() > 0 && !IsReversedDocument))
            {
                // get reference of Orignal Document
                sql += @" AND M_InoutLine_ID = (SELECT NVL(C_OrderLine.Orig_InOutLine_ID, 0) FROM M_InoutLine 
                                INNER JOIN C_OrderLine ON M_InoutLine.C_OrderLine_ID = C_OrderLine.C_OrderLine_ID 
                        WHERE  M_InoutLine.M_InoutLine_ID = " + cd.GetM_InOutLine_ID() + ")";
            }
            else if (windowName.Equals("Return To Vendor") && (cd.GetC_OrderLine_ID() == 0 || IsReversedDocument))
            {
                // get reference of Orignal Document
                sql += @" AND M_InoutLine_ID = (SELECT NVL(M_InoutLine.Reversaldoc_ID, 0) FROM M_InoutLine 
                          WHERE M_InoutLine.M_InoutLine_ID = " + cd.GetM_InOutLine_ID() + ")";
            }
            else if (windowName.Equals("Internal Use Inventory"))
            {
                sql += @" AND M_InventoryLine_ID = (SELECT NVL(M_InventoryLine.Reversaldoc_ID, 0) FROM M_InventoryLine  
                        WHERE  M_InventoryLine.M_InventoryLine_ID = " + cd.GetM_InventoryLine_ID() + ")";
            }
            else if (windowName.Equals("AssetDisposal"))
            {
                sql += @" AND VAFAM_AssetDisposal_ID = (SELECT NVL(VAFAM_AssetDisposal.Reversaldoc_ID, 0) FROM VAFAM_AssetDisposal  
                        WHERE  VAFAM_AssetDisposal.VAFAM_AssetDisposal_ID = " + cd.Get_ValueAsInt("VAFAM_AssetDisposal_ID") + ")";
            }
            else if (windowName.Equals("Material Receipt") || windowName.Equals("Shipment"))
            {
                sql += @" AND M_InoutLine_ID = (SELECT NVL(M_InoutLine.Reversaldoc_ID, " + cd.GetM_InOutLine_ID() + @") FROM M_InoutLine 
                          WHERE M_InoutLine.M_InoutLine_ID = " + cd.GetM_InOutLine_ID() + ")";
            }
            else if (windowName.Equals("Inventory Move"))
            {
                sql += @" AND M_CostQueue.M_Warehouse_ID = " + M_Warehouse_ID + @" AND M_CostQueueTransaction.M_MovementLine_ID = 
                          (SELECT NVL(M_MovementLine.Reversaldoc_ID, " + cd.GetM_MovementLine_ID() + @") FROM M_MovementLine 
                          WHERE M_MovementLine.M_MovementLine_ID = " + cd.GetM_MovementLine_ID() + ")";
            }
            else if (windowName.Equals("Production Execution"))
            {
                sql += @" AND VAMFG_M_WrkOdrTrnsctionLine_ID = (SELECT COALESCE(VAMFG_M_WrkOdrTrnsctionLine.Reversaldoc_ID ,
                            NVL(VAMFG_M_WrkOdrTrnsctionLine.VAMFG_OrigWrkOdrTrxLine_ID_1, 0)) FROM VAMFG_M_WrkOdrTrnsctionLine 
                          WHERE VAMFG_M_WrkOdrTrnsctionLine.VAMFG_M_WrkOdrTrnsctionLine_ID = " + cd.GetVAMFG_M_WrkOdrTrnsctionLine_ID() + ")";
            }

            sql += @" ORDER BY CASE WHEN M_CostElement.CostingMethod = 'F' THEN M_CostQueueTransaction.M_CostQueueTransaction_ID END Desc,
                               CASE WHEN M_CostElement.CostingMethod = 'L' THEN M_CostQueueTransaction.M_CostQueueTransaction_ID END ASC";
            DataSet dsCostQueue = DB.ExecuteDataset(sql, null, cd.Get_Trx());
            if (dsCostQueue != null && dsCostQueue.Tables.Count > 0 && dsCostQueue.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsCostQueue.Tables[0].Rows.Count; i++)
                {
                    // when qty on Queue Transaction is greaterthan or Equal to 
                    if (Math.Abs(Util.GetValueOfDecimal(dsCostQueue.Tables[0].Rows[i]["MovementQty"])) >= Qty)
                    {
                        DB.ExecuteQuery("UPDATE M_CostQueue SET CurrentQty = CurrentQty + " + Qty +
                            @" WHERE M_CostQueue_ID = " + Util.GetValueOfInt(dsCostQueue.Tables[0].Rows[i]["M_CostQueue_ID"]), null, cd.Get_Trx());

                        // Create Cost Queue Transactional Record
                        if (!MCostQueueTransaction.CreateCostQueueTransaction(cd.GetCtx(), cd.GetAD_Client_ID(),
                            Util.GetValueOfInt(dsCostQueue.Tables[0].Rows[i]["AD_Org_ID"]), Util.GetValueOfInt(dsCostQueue.Tables[0].Rows[i]["M_CostQueue_ID"]), cd, Qty))
                        {
                            return false;
                        }
                        break;
                    }
                    // when qty on Queue Transaction is less than 
                    else if (Math.Abs(Util.GetValueOfDecimal(dsCostQueue.Tables[0].Rows[i]["MovementQty"])) < Qty)
                    {
                        DB.ExecuteQuery("UPDATE M_CostQueue SET CurrentQty = CurrentQty + " + Math.Abs(Util.GetValueOfDecimal(dsCostQueue.Tables[0].Rows[i]["MovementQty"])) +
                            @" WHERE M_CostQueue_ID = " + Util.GetValueOfInt(dsCostQueue.Tables[0].Rows[i]["M_CostQueue_ID"]), null, cd.Get_Trx());
                        Qty -= Math.Abs(Util.GetValueOfDecimal(dsCostQueue.Tables[0].Rows[i]["MovementQty"]));

                        // Create Cost Queue Transactional Record
                        if (!MCostQueueTransaction.CreateCostQueueTransaction(cd.GetCtx(), cd.GetAD_Client_ID(),
                            Util.GetValueOfInt(dsCostQueue.Tables[0].Rows[i]["AD_Org_ID"]), Util.GetValueOfInt(dsCostQueue.Tables[0].Rows[i]["M_CostQueue_ID"]), cd,
                            Math.Abs(Util.GetValueOfDecimal(dsCostQueue.Tables[0].Rows[i]["MovementQty"]))))
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                // TO-DO when return orignal records not found
                _log.Info("Costing Engine: Record not found for ReAssigned Stock -> " + sql);
                backwardCompatabilitySupport = true;
                return false;
            }
            return true;
        }

        /// <summary>
        /// This function is reduce the stock from cost queue against Orginal transaction
        /// </summary>
        /// <param name="cd">Cost Detail</param>
        /// <param name="windowName">window Name</param>
        /// <param name="costElement">cost Element</param>
        /// <param name="Qty">qty to be reduced</param>
        /// <param name="IsReversedDocument">Is Reversed Document or not</param>
        /// <param name="M_Warehouse_ID">Warehouse Id</param>
        /// <returns>true when success</returns>
        public static bool ReturnStockReduceFromCostQueue(MCostDetail cd, String windowName, MCostElement costElement, Decimal Qty, bool IsReversedDocument, int M_Warehouse_ID, out bool backwardCompatabilitySupport)
        {
            String sql = "";
            backwardCompatabilitySupport = false;
            String selectStatement = @"SELECT  M_CostQueueTransaction.M_CostQueueTransaction_ID, M_CostQueue.M_CostQueue_ID, M_CostQueueTransaction.MovementQty,
                            M_CostQueue.CurrentQty, M_CostQueue.AD_Org_ID ";

            sql = @" FROM M_CostQueue INNER JOIN M_CostQueueTransaction
                            ON M_CostQueue.M_CostQueue_ID = m_costQueuetransaction.M_CostQueue_ID
                            INNER JOIN M_CostElement ON M_CostQueue.M_CostElement_ID = M_CostElement.M_CostElement_ID
                            WHERE  M_CostQueueTransaction.MovementQty <> 0 AND M_CostQueue.M_CostElement_ID = " + costElement.GetM_CostElement_ID() + @"
                            AND M_CostQueue.C_ACCTSCHEMA_ID = " + cd.GetC_AcctSchema_ID();

            if (windowName.Equals("Customer Return") ||
                windowName.Equals("Material Receipt") ||
                windowName.Equals("Shipment") ||
                (windowName.Equals("Return To Vendor") && (cd.GetC_OrderLine_ID() == 0 || IsReversedDocument)))
            {
                sql += @" AND M_InoutLine_ID = (SELECT NVL(M_InoutLine.Reversaldoc_ID, " + cd.GetM_InOutLine_ID() + @") FROM M_InoutLine 
                          WHERE M_InoutLine.M_InoutLine_ID = " + cd.GetM_InOutLine_ID() + ")";
            }
            else if (windowName.Equals("Return To Vendor") && cd.GetC_OrderLine_ID() > 0 && !IsReversedDocument)
            {
                // get reference of Orignal Document
                sql += @" AND M_InoutLine_ID = (SELECT NVL(C_OrderLine.Orig_InOutLine_ID, 0) FROM M_InoutLine 
                                INNER JOIN C_OrderLine ON M_InoutLine.C_OrderLine_ID = C_OrderLine.C_OrderLine_ID 
                        WHERE  M_InoutLine.M_InoutLine_ID = " + cd.GetM_InOutLine_ID() + ")";
            }
            else if (windowName.Equals("Inventory Move"))
            {
                sql += @" AND M_CostQueue.M_Warehouse_ID = " + M_Warehouse_ID + @" AND M_CostQueueTransaction.M_MovementLine_ID = 
                          (SELECT NVL(M_MovementLine.Reversaldoc_ID, " + cd.GetM_MovementLine_ID() + @") FROM M_MovementLine 
                          WHERE M_MovementLine.M_MovementLine_ID = " + cd.GetM_MovementLine_ID() + ")";
            }
            else if (windowName.Equals("Production Execution") || windowName.Equals("PE-FinishGood"))
            {
                sql += @" AND VAMFG_M_WrkOdrTrnsctionLine_ID = (SELECT COALESCE(VAMFG_M_WrkOdrTrnsctionLine.Reversaldoc_ID, 0) FROM VAMFG_M_WrkOdrTrnsctionLine 
                          WHERE VAMFG_M_WrkOdrTrnsctionLine.VAMFG_M_WrkOdrTrnsctionLine_ID = " + cd.GetVAMFG_M_WrkOdrTrnsctionLine_ID() + ")";
            }

            if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(M_CostQueue.M_CostQueue_ID) " + sql +
                @" AND  M_CostQueue.CurrentQty < CASE WHEN " + Math.Abs(Qty) + " <  M_CostQueueTransaction.MovementQty THEN " + Math.Abs(Qty) +
                " ELSE M_CostQueueTransaction.MovementQty END")) > 0)
            {
                _log.Info("SELECT COUNT(M_CostQueue.M_CostQueue_ID) " + sql + " AND  M_CostQueue.CurrentQty < CASE WHEN " + Math.Abs(Qty) + " <  M_CostQueueTransaction.MovementQty THEN " + Math.Abs(Qty) +
                " ELSE M_CostQueueTransaction.MovementQty END");
                _log.Info("Costing Engine : Current Qty is less than Movement Qty on cost Queue Transaction. window Name  = " + windowName);
                return false;
            }

            sql += @" ORDER BY CASE WHEN M_CostElement.CostingMethod = 'F' THEN M_CostQueueTransaction.M_CostQueueTransaction_ID END Desc,
                               CASE WHEN M_CostElement.CostingMethod = 'L' THEN M_CostQueueTransaction.M_CostQueueTransaction_ID END ASC";

            DataSet dsCostQueue = DB.ExecuteDataset(selectStatement + sql, null, cd.Get_Trx());
            if (dsCostQueue != null && dsCostQueue.Tables.Count > 0 && dsCostQueue.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsCostQueue.Tables[0].Rows.Count; i++)
                {
                    // when qty on Queue Transaction is greaterthan or Equal to 
                    if (Math.Abs(Util.GetValueOfDecimal(dsCostQueue.Tables[0].Rows[i]["MovementQty"])) >= Qty)
                    {
                        int no = DB.ExecuteQuery("UPDATE M_CostQueue SET CurrentQty = CurrentQty - " + Qty +
                             @" WHERE M_CostQueue_ID = " + Util.GetValueOfInt(dsCostQueue.Tables[0].Rows[i]["M_CostQueue_ID"]), null, cd.Get_Trx());

                        // Create Cost Queue Transactional Record
                        if (!MCostQueueTransaction.CreateCostQueueTransaction(cd.GetCtx(), cd.GetAD_Client_ID(),
                            Util.GetValueOfInt(dsCostQueue.Tables[0].Rows[i]["AD_Org_ID"]), Util.GetValueOfInt(dsCostQueue.Tables[0].Rows[i]["M_CostQueue_ID"]),
                            cd, Decimal.Negate(Qty)))
                        {
                            return false;
                        }
                        break;
                    }
                    // when qty on Queue Transaction is less than 
                    else if (Math.Abs(Util.GetValueOfDecimal(dsCostQueue.Tables[0].Rows[i]["MovementQty"])) < Qty)
                    {
                        int no = DB.ExecuteQuery("UPDATE M_CostQueue SET CurrentQty = CurrentQty - " + Math.Abs(Util.GetValueOfDecimal(dsCostQueue.Tables[0].Rows[i]["MovementQty"])) +
                            @" WHERE M_CostQueue_ID = " + Util.GetValueOfInt(dsCostQueue.Tables[0].Rows[i]["M_CostQueue_ID"]), null, cd.Get_Trx());
                        Qty -= Math.Abs(Util.GetValueOfDecimal(dsCostQueue.Tables[0].Rows[i]["MovementQty"]));

                        // Create Cost Queue Transactional Record
                        if (!MCostQueueTransaction.CreateCostQueueTransaction(cd.GetCtx(), cd.GetAD_Client_ID(),
                            Util.GetValueOfInt(dsCostQueue.Tables[0].Rows[i]["AD_Org_ID"]), Util.GetValueOfInt(dsCostQueue.Tables[0].Rows[i]["M_CostQueue_ID"]), cd,
                            Decimal.Negate(Math.Abs(Util.GetValueOfDecimal(dsCostQueue.Tables[0].Rows[i]["MovementQty"])))))
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                // TO-DO when return orignal records not found
                _log.Info("Costing Engine: Record not found for Reduce Stock -> " + (selectStatement + sql));
                backwardCompatabilitySupport = true;
                return false;
            }
            return true;
        }
        //end
    }
}
