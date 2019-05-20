/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : CostUpdate
 * Purpose        : Standard Cost Update 
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan     26-Oct-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;


using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class CostUpdate : ProcessEngine.SvrProcess
    {
        #region Private Variables
        //	Product Category	
        private int _M_Product_Category_ID = 0;
        // Future Costs			
        private String _SetFutureCostTo = null;
        // Standard Costs			
        private String _SetStandardCostTo = null;
        // PLV						
        private int _M_PriceList_Version_ID = 0;
        private static String TO_AveragePO = "A";
        private static String TO_AverageInvoiceHistory = "DI";
        private static String TO_AveragePOHistory = "DP";
        private static String TO_FiFo = "F";
        private static String TO_AverageInvoice = "I";
        private static String TO_LiFo = "L";
        private static String TO_PriceListLimit = "LL";
        private static String TO_StandardCost = "S";
        private static String TO_FutureStandardCost = "f";
        private static String TO_LastInvoicePrice = "i";
        private static String TO_LastPOPrice = "p";
        private static String TO_OldStandardCost = "x";
        //Standard Cost Element		
        private MCostElement _ce = null;
        // Client Accounting SChema	
        private MAcctSchema[] _ass = null;
        // Map of Cost Elements		
        private Dictionary<String, MCostElement> _ces = new Dictionary<String, MCostElement>();
        #endregion

        /// <summary>
        /// Prepare
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                //	log.fine("prepare - " + para[i]);
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("M_Product_Category_ID"))
                {
                    _M_Product_Category_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("SetFutureCostTo"))
                {
                    _SetFutureCostTo = (String)para[i].GetParameter();
                }
                else if (name.Equals("SetStandardCostTo"))
                {
                    _SetStandardCostTo = (String)para[i].GetParameter();
                }
                else if (name.Equals("M_PriceList_Version_ID"))
                {
                    _M_PriceList_Version_ID = para[i].GetParameterAsInt();
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
        /// <returns>info</returns>
        protected override String DoIt()
        {
            log.Info("M_Product_Category_ID=" + _M_Product_Category_ID
                + ", Future=" + _SetFutureCostTo
                + ", Standard=" + _SetStandardCostTo
                + "; M_PriceList_Version_ID=" + _M_PriceList_Version_ID);
            if (_SetFutureCostTo == null)
            {
                _SetFutureCostTo = "";
            }
            if (_SetStandardCostTo == null)
            {
                _SetStandardCostTo = "";
            }
            //	Nothing to Do
            if (_SetFutureCostTo.Length == 0 && _SetStandardCostTo.Length == 0)
            {
                return "-";
            }
            //	PLV required
            if (_M_PriceList_Version_ID == 0
                && (_SetFutureCostTo.Equals(TO_PriceListLimit) || _SetStandardCostTo.Equals(TO_PriceListLimit)))
            {
                throw new Exception("@FillMandatory@  @M_PriceList_Version_ID@");
            }

            //	Validate Source
            if (!IsValid(_SetFutureCostTo))
            {
                throw new Exception("@NotFound@ @M_CostElement_ID@ (Future) " + _SetFutureCostTo);
            }
            if (!IsValid(_SetStandardCostTo))
            {
                throw new Exception("@NotFound@ @M_CostElement_ID@ (Standard) " + _SetStandardCostTo);
            }

            //	Prepare
            MClient client = MClient.Get(GetCtx());
            _ce = MCostElement.GetMaterialCostElement(client, MAcctSchema.COSTINGMETHOD_StandardCosting);
            if (_ce.Get_ID() == 0)
            {
                throw new Exception("@NotFound@ @M_CostElement_ID@ (StdCost)");
            }
            log.Config(_ce.ToString());
            _ass = MAcctSchema.GetClientAcctSchema(GetCtx(), client.GetAD_Client_ID());
            for (int i = 0; i < _ass.Length; i++)
            {
                CreateNew(_ass[i]);
            }
            Commit();

            //	Update Cost
            int counter = Update();

            return "#" + counter;
        }

        /// <summary>
        /// Costing Method must exist
        /// </summary>
        /// <param name="to">test</param>
        /// <returns>true valid</returns>
        private bool IsValid(String to)
        {
            if (_SetFutureCostTo.Length == 0)
            {
                return true;
            }

            String toTarget = to;
            if (to.Equals(TO_AverageInvoiceHistory))
            {
                to = TO_AverageInvoice;
            }
            if (to.Equals(TO_AveragePOHistory))
            {
                to = TO_AveragePO;
            }
            if (to.Equals(TO_FutureStandardCost))
            {
                to = TO_StandardCost;
            }
            //
            if (to.Equals(TO_AverageInvoice)
                || to.Equals(TO_AveragePO)
                || to.Equals(TO_FiFo)
                || to.Equals(TO_LiFo)
                || to.Equals(TO_StandardCost))
            {
                MCostElement ce = GetCostElement(_SetFutureCostTo);
                return ce != null;
            }
            return true;
        }

        /// <summary>
        /// Create New Standard Costs
        /// </summary>
        /// <param name="as1">accounting schema</param>
        private void CreateNew(MAcctSchema as1)
        {
            if (!as1.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Client))
            {
                String txt = "Costing Level prevents creating new Costing records for " + as1.GetName();
                log.Warning(txt);
                AddLog(0, null, null, txt);
                return;
            }
            String sql = "SELECT * FROM M_Product p "
                + "WHERE NOT EXISTS (SELECT * FROM M_Cost c WHERE c.M_Product_ID=p.M_Product_ID"
                + " AND c.M_CostType_ID=" + as1.GetM_CostType_ID() + " AND c.C_AcctSchema_ID=" + as1.GetC_AcctSchema_ID() + " AND c.M_CostElement_ID=" + _ce.GetM_CostElement_ID()
                + " AND c.M_AttributeSetInstance_ID=0) "
                + "AND AD_Client_ID=" + as1.GetAD_Client_ID();
            if (_M_Product_Category_ID != 0)
            {
                sql += " AND M_Product_Category_ID=" + _M_Product_Category_ID;
            }
            int counter = 0;
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    if (CreateNew(new MProduct(GetCtx(), dr, null), as1))
                    {
                        counter++;
                    }
                }
               
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
                if (idr != null)
                {
                    idr.Close();
                }
            }

            log.Info("#" + counter);
            AddLog(0, null, new Decimal(counter), "Created for " + as1.GetName());
        }

        /// <summary>
        /// Create New Client level Costing Record
        /// </summary>
        /// <param name="product">product</param>
        /// <param name="?">acct schema</param>
        /// <returns>true if created</returns>
        private bool CreateNew(MProduct product, MAcctSchema as1)
        {
            MCost cost = MCost.Get(product, 0, as1, 0, _ce.GetM_CostElement_ID());
            if (cost.Is_New())
            {
                return cost.Save();
            }
            return false;
        }

        /// <summary>
        /// Update Cost Records
        /// </summary>
        /// <returns>no updated</returns>
        private int Update()
        {
            int counter = 0;
            String sql = "SELECT * FROM M_Cost c WHERE M_CostElement_ID=" + _ce.GetM_CostElement_ID();
            if (_M_Product_Category_ID != 0)
            {
                sql += " AND EXISTS (SELECT * FROM M_Product p "
                    + "WHERE c.M_Product_ID=p.M_Product_ID AND p.M_Product_Category_ID=" + _M_Product_Category_ID + ")";
            }
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                //pstmt.setInt (1, _ce.GetM_CostElement_ID());
                //if (_M_Product_Category_ID != 0)
                //    pstmt.setInt (2, _M_Product_Category_ID);
                //ResultSet dr = pstmt.executeQuery ();
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    MCost cost = new MCost(GetCtx(), dr, Get_Trx());
                    for (int i = 0; i < _ass.Length; i++)
                    {
                        //	Update Costs only for default Cost Type
                        if (_ass[i].GetC_AcctSchema_ID() == cost.GetC_AcctSchema_ID()
                            && _ass[i].GetM_CostType_ID() == cost.GetM_CostType_ID())
                        {
                            if (Update(cost))
                            {
                                counter++;
                            }
                        }
                    }
                }
                
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
                if (idr != null)
                {
                    idr.Close();
                }
            }

            log.Info("#" + counter);
            AddLog(0, null, new Decimal(counter), "@Updated@");
            return counter;
        }

        /// <summary>
        /// Update Cost Records
        /// </summary>
        /// <param name="cost">cost</param>
        /// <returns>true if updated</returns>
        private bool Update(MCost cost)
        {
            bool updated = false;
            if (_SetFutureCostTo.Equals(_SetStandardCostTo))
            {
                Decimal costs = Utility.Util.GetValueOfDecimal(GetCosts(cost, _SetFutureCostTo));
                if ( Env.Signum(costs) != 0)
                {
                    cost.SetFutureCostPrice(costs);
                    cost.SetCurrentCostPrice(costs);
                    updated = true;
                }
            }
            else
            {
                if (_SetStandardCostTo.Length > 0)
                {
                    Decimal costs = Utility.Util.GetValueOfDecimal(GetCosts(cost, _SetStandardCostTo));
                    if ( Env.Signum(costs) != 0)
                    {
                        cost.SetCurrentCostPrice(costs);
                        updated = true;
                    }
                }
                if (_SetFutureCostTo.Length > 0)
                {
                    Decimal costs = Utility.Util.GetValueOfDecimal(GetCosts(cost, _SetFutureCostTo));
                    if ( Env.Signum(costs) != 0)
                    {
                        cost.SetFutureCostPrice(costs);
                        updated = true;
                    }
                }
            }
            if (updated)
            {
                updated = cost.Save();
            }
            return updated;
        }

        /// <summary>
        /// Get Costs
        /// </summary>
        /// <param name="cost">Cost</param>
        /// <param name="to">where to get costs from </param>
        /// <returns>costs (could be 0) or null if not found</returns>
        private Decimal? GetCosts(MCost cost, String to)
        {
            Decimal? retValue = null;

            //	Average Invoice
            if (to.Equals(TO_AverageInvoice))
            {
                MCostElement ce = GetCostElement(TO_AverageInvoice);
                if (ce == null)
                {
                    throw new Exception("CostElement not found: " + TO_AverageInvoice);
                }
                MCost xCost = MCost.Get(GetCtx(), cost.GetAD_Client_ID(), cost.GetAD_Org_ID(), cost.GetM_Product_ID(), cost.GetM_CostType_ID(), cost.GetC_AcctSchema_ID(), ce.GetM_CostElement_ID(), cost.GetM_AttributeSetInstance_ID());
                if (xCost != null)
                {
                    retValue = xCost.GetCurrentCostPrice();
                }
            }
            //	Average Invoice History
            else if (to.Equals(TO_AverageInvoiceHistory))
            {
                MCostElement ce = GetCostElement(TO_AverageInvoice);
                if (ce == null)
                {
                    throw new Exception("CostElement not found: " + TO_AverageInvoice);
                }
                MCost xCost = MCost.Get(GetCtx(), cost.GetAD_Client_ID(), cost.GetAD_Org_ID(), cost.GetM_Product_ID(), cost.GetM_CostType_ID(), cost.GetC_AcctSchema_ID(), ce.GetM_CostElement_ID(), cost.GetM_AttributeSetInstance_ID());
                if (xCost != null)
                {
                    retValue = xCost.GetHistoryAverage();
                }
            }

            //	Average PO
            else if (to.Equals(TO_AveragePO))
            {
                MCostElement ce = GetCostElement(TO_AveragePO);
                if (ce == null)
                {
                    throw new Exception("CostElement not found: " + TO_AveragePO);
                }
                MCost xCost = MCost.Get(GetCtx(), cost.GetAD_Client_ID(), cost.GetAD_Org_ID(), cost.GetM_Product_ID(), cost.GetM_CostType_ID(), cost.GetC_AcctSchema_ID(), ce.GetM_CostElement_ID(), cost.GetM_AttributeSetInstance_ID());
                if (xCost != null)
                {
                    retValue = xCost.GetCurrentCostPrice();
                }
            }
            //	Average PO History
            else if (to.Equals(TO_AveragePOHistory))
            {
                MCostElement ce = GetCostElement(TO_AveragePO);
                if (ce == null)
                {
                    throw new Exception("CostElement not found: " + TO_AveragePO);
                }
                MCost xCost = MCost.Get(GetCtx(), cost.GetAD_Client_ID(), cost.GetAD_Org_ID(), cost.GetM_Product_ID(), cost.GetM_CostType_ID(), cost.GetC_AcctSchema_ID(), ce.GetM_CostElement_ID(), cost.GetM_AttributeSetInstance_ID());
                if (xCost != null)
                {
                    retValue = xCost.GetHistoryAverage();
                }
            }

            //	FiFo
            else if (to.Equals(TO_FiFo))
            {
                MCostElement ce = GetCostElement(TO_FiFo);
                if (ce == null)
                {
                    throw new Exception("CostElement not found: " + TO_FiFo);
                }
                MCost xCost = MCost.Get(GetCtx(), cost.GetAD_Client_ID(), cost.GetAD_Org_ID(), cost.GetM_Product_ID(), cost.GetM_CostType_ID(), cost.GetC_AcctSchema_ID(), ce.GetM_CostElement_ID(), cost.GetM_AttributeSetInstance_ID());
                if (xCost != null)
                {
                    retValue = xCost.GetCurrentCostPrice();
                }
            }

            //	Future Std Costs
            else if (to.Equals(TO_FutureStandardCost))
            {
                retValue = cost.GetFutureCostPrice();
            }

            //	Last Inv Price
            else if (to.Equals(TO_LastInvoicePrice))
            {
                MCostElement ce = GetCostElement(TO_LastInvoicePrice);
                if (ce != null)
                {
                    MCost xCost = MCost.Get(GetCtx(), cost.GetAD_Client_ID(), cost.GetAD_Org_ID(), cost.GetM_Product_ID(), cost.GetM_CostType_ID(), cost.GetC_AcctSchema_ID(), ce.GetM_CostElement_ID(), cost.GetM_AttributeSetInstance_ID());
                    if (xCost != null)
                    {
                        retValue = xCost.GetCurrentCostPrice();
                    }
                }
                if (retValue == null)
                {
                    MProduct product = MProduct.Get(GetCtx(), cost.GetM_Product_ID());
                    MAcctSchema as1 = MAcctSchema.Get(GetCtx(), cost.GetC_AcctSchema_ID());
                    retValue = MCost.GetLastInvoicePrice(product,
                        cost.GetM_AttributeSetInstance_ID(), cost.GetAD_Org_ID(), as1.GetC_Currency_ID());
                }
            }

            //	Last PO Price
            else if (to.Equals(TO_LastPOPrice))
            {
                MCostElement ce = GetCostElement(TO_LastPOPrice);
                if (ce != null)
                {
                    MCost xCost = MCost.Get(GetCtx(), cost.GetAD_Client_ID(), cost.GetAD_Org_ID(), cost.GetM_Product_ID(), cost.GetM_CostType_ID(), cost.GetC_AcctSchema_ID(), ce.GetM_CostElement_ID(), cost.GetM_AttributeSetInstance_ID());
                    if (xCost != null)
                    {
                        retValue = xCost.GetCurrentCostPrice();
                    }
                }
                if (retValue == null)
                {
                    MProduct product = MProduct.Get(GetCtx(), cost.GetM_Product_ID());
                    MAcctSchema as1 = MAcctSchema.Get(GetCtx(), cost.GetC_AcctSchema_ID());
                    retValue = MCost.GetLastPOPrice(product,
                        cost.GetM_AttributeSetInstance_ID(), cost.GetAD_Org_ID(), as1.GetC_Currency_ID());
                }
            }

            //	FiFo
            else if (to.Equals(TO_LiFo))
            {
                MCostElement ce = GetCostElement(TO_LiFo);
                if (ce == null)
                {
                    throw new Exception("CostElement not found: " + TO_LiFo);
                }
                MCost xCost = MCost.Get(GetCtx(), cost.GetAD_Client_ID(), cost.GetAD_Org_ID(), cost.GetM_Product_ID(), cost.GetM_CostType_ID(), cost.GetC_AcctSchema_ID(), ce.GetM_CostElement_ID(), cost.GetM_AttributeSetInstance_ID());
                if (xCost != null)
                {
                    retValue = xCost.GetCurrentCostPrice();
                }
            }

            //	Old Std Costs
            else if (to.Equals(TO_OldStandardCost))
            {
                retValue = GetOldCurrentCostPrice(cost);
            }

            //	Price List
            else if (to.Equals(TO_PriceListLimit))
            {
                retValue = GetPrice(cost);
            }

            //	Standard Costs
            else if (to.Equals(TO_StandardCost))
            {
                retValue = cost.GetCurrentCostPrice();
            }

            return retValue;
        }

        /// <summary>
        /// Get Cost Element
        /// </summary>
        /// <param name="CostingMethod">method</param>
        /// <returns>costing element or null</returns>
        private MCostElement GetCostElement(String CostingMethod)
        {
            MCostElement ce = null;
            //if (_ces.Count == 0)
            //{
            //    ce = null;
            //}
            //else
            //{
            //    ce = _ces[CostingMethod];// _ces.get(CostingMethod);
            //}
            if (_ces.ContainsKey(CostingMethod))
            {
                ce = _ces[CostingMethod];// _ces.get(CostingMethod);
            }
            if (ce == null)
            {
                ce = MCostElement.GetMaterialCostElement(GetCtx(), CostingMethod);
                //_ces.put(CostingMethod, ce);
                _ces.Add(CostingMethod, ce);
            }
            return ce;
        }

        /// <summary>
        /// Get Old Current Cost Price
        /// </summary>
        /// <param name="cost">costs</param>
        /// <returns>price if found</returns>
        private Decimal? GetOldCurrentCostPrice(MCost cost)
        {
            Decimal? retValue = null;
            String sql = "SELECT CostStandard, CurrentCostPrice "
                + "FROM M_Product_Costing "
                + "WHERE M_Product_ID=" + cost.GetM_Product_ID() + " AND C_AcctSchema_ID=" + cost.GetC_AcctSchema_ID();
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                //pstmt.setInt (1, cost.GetM_Product_ID());
                //pstmt.setInt (2, cost.GetC_AcctSchema_ID());
                //ResultSet dr = pstmt.executeQuery ();
                if (idr.Read())
                {
                    retValue = Utility.Util.GetValueOfDecimal(idr[0]);//.getBigDecimal(1);
                    if (retValue == null || Env.Signum(Utility.Util.GetValueOfDecimal(retValue)) == 0)
                    {
                        retValue = Utility.Util.GetValueOfDecimal(idr[1]);
                    }
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
           

            return retValue;
        }

        /// <summary>
        /// Get Price from Price List
        /// </summary>
        /// <param name="cost">cost record</param>
        /// <returns>price or null</returns>
        private Decimal? GetPrice(MCost cost)
        {
            Decimal? retValue = null;
            String sql = "SELECT PriceLimit "
                + "FROM M_ProductPrice "
                + "WHERE M_Product_ID=" + cost.GetM_Product_ID() + " AND M_PriceList_Version_ID=" + _M_PriceList_Version_ID;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                //pstmt.setInt (1, cost.GetM_Product_ID());
                //pstmt.setInt (2, _M_PriceList_Version_ID);
                //ResultSet dr = pstmt.executeQuery ();
                if (idr.Read())
                {
                    retValue = Utility.Util.GetValueOfDecimal(idr[0]);//.getBigDecimal(1);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            

            return retValue;
        }
    }
}
