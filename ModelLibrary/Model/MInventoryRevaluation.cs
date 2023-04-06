/********************************************************
 * Project Name   : VAdvantage
 * Module Name    : ModelLibrary
 * Class Name     : MInventoryRevaluation, X_M_InventoryRevaluation, DocAction
 * Purpose        : Inventory Revaluation
 * Class Used     : none
 * Chronological  : Development
 * VIS_0045       : 10-March-2023
  ******************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Process;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MInventoryRevaluation : X_M_InventoryRevaluation, DocAction
    {
        /**	Process Message 			*/
        private String _processMsg = null;
        /**	Just Prepared Flag			*/
        private bool _justPrepared = false;
        /** Inventory Revaluation Lines*/
        private MRevaluationLine[] _lines = null;
        /** Sql Query*/
        private StringBuilder sql = new StringBuilder();
        /** Rows Affected after execute query */
        int no = 0;
        /** Accounting Schema*/
        MAcctSchema acctSchema = null;
        /** Cost Element ID*/
        int M_CostElement_ID = 0;
        int Binded_CostElement_ID = 0;
        /** Get Product Cost Element */
        DataSet dsCostElement = null;
        DataRow[] drCostElement = null;
        /** Product Transaction Details*/
        DataSet dsProdutTransaction = null;
        DataRow[] drProdutTransaction = null;
        /* Product Cost**/
        decimal productCost = 0;
        decimal combCost = 0;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Payment_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MInventoryRevaluation(Ctx ctx, int M_InventoryRevaluation_ID, Trx trxName)
           : base(ctx, M_InventoryRevaluation_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MInventoryRevaluation(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Get Inventory Revaluation Lines
        /// </summary>
        /// <param name="requery">true/false</param>
        /// <returns>list of Inventory Revaluation Lines</returns>
        public MRevaluationLine[] GetLines(Boolean requery)
        {
            if (_lines != null && !requery)
                return _lines;
            //
            List<MRevaluationLine> list = new List<MRevaluationLine>();
            String sql = "SELECT * FROM M_RevaluationLine WHERE M_InventoryRevaluation_ID = @M_InventoryRevaluation_ID ORDER BY LineNo";
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@M_InventoryRevaluation_ID", GetM_InventoryRevaluation_ID());

                DataSet ds = DB.ExecuteDataset(sql, param, Get_TrxName());
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new MRevaluationLine(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "GetLines", e);
            }

            _lines = new MRevaluationLine[list.Count];
            _lines = list.ToArray();
            return _lines;
        }

        /// <summary>
        /// Implement Before Save Functionality
        /// </summary>
        /// <param name="newRecord">Is New Record</param>
        /// <returns>True, when Success</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (!newRecord)
            {
                // When Lines found, then following fields can't chnage
                int countLines = Util.GetValueOfInt(DB.ExecuteScalar($@"SELECT COUNT(M_RevaluationLine_ID) FROM M_RevaluationLine 
                             WHERE M_InventoryRevaluation_ID = {GetM_InventoryRevaluation_ID()}", null, Get_Trx()));
                if (countLines > 0)
                {
                    if (Is_ValueChanged("RevaluationType") || Is_ValueChanged("C_Period_ID") || Is_ValueChanged("C_AcctSchema_ID") ||
                        Is_ValueChanged("C_ConversionType_ID") || Is_ValueChanged("CostingLevel") || Is_ValueChanged("CostingMethod") ||
                        Is_ValueChanged("M_Warehouse_ID"))
                    {
                        log.SaveError("RevalLineFoundNotChanged", "");
                        return false;
                    }
                }
            }

            // Set Accounting Schema Currency
            if (GetC_AcctSchema_ID() > 0 && (GetC_Currency_ID() == 0 || Is_ValueChanged("C_AcctSchema_ID")))
            {
                SetC_Currency_ID(MAcctSchema.Get(GetCtx(), GetC_AcctSchema_ID()).GetC_Currency_ID());
            }

            // Document Date / Account Date should be equal to current date
            if ((newRecord || Is_ValueChanged("DateDoc") || Is_ValueChanged("DateAcct") || Is_ValueChanged("RevaluationType"))
                && GetRevaluationType().Equals(REVALUATIONTYPE_OnAvailableQuantity))
            {
                if (GetDateDoc().Value.Date != GetDateAcct().Value.Date)
                {
                    log.SaveError("RevaluationDateNotMatch", "");
                    return false;
                }
                if (GetDateAcct().Value.Date != DateTime.Now.Date)
                {
                    log.SaveError("RevalAcctDateNotCurrentDate", "");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Process Document
        /// </summary>
        /// <param name="processAction">document action</param>
        /// <returns>true if performed</returns>
        public bool ProcessIt(string action)
        {
            _processMsg = null;
            DocumentEngine engine = new DocumentEngine(this, GetDocStatus());
            return engine.ProcessIt(action, GetDocAction());
        }

        /// <summary>
        /// Prepare Document
        /// </summary>
        /// <returns>new status (In Progress or Invalid)</returns>
        public string PrepareIt()
        {
            log.Info(ToString());
            _processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_BEFORE_PREPARE);
            if (_processMsg != null)
                return DocActionVariables.STATUS_INVALID;

            //	Std Period open?
            if (!MPeriod.IsOpen(GetCtx(), GetDateAcct(), GetDocBaseType(), GetAD_Org_ID()))
            {
                _processMsg = "@PeriodClosed@";
                log.Info(_processMsg);
                return DocActionVariables.STATUS_INVALID;
            }

            // is Non Business Day?
            if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetDateAcct(), GetAD_Org_ID()))
            {
                _processMsg = VAdvantage.Common.Common.NONBUSINESSDAY;
                log.Info(_processMsg);
                return DocActionVariables.STATUS_INVALID;
            }

            // check Lines
            MRevaluationLine[] lines = GetLines(false);
            if (lines.Length == 0)
            {
                _processMsg = "@NoLines@";
                log.Info(_processMsg);
                return DocActionVariables.STATUS_INVALID;
            }

            //when newcostprice is ZERO then returm message, please define revaluated cost
            if (Util.GetValueOfInt(DB.ExecuteScalar($@"SELECT COUNT(M_RevaluationLine_ID) FROM M_RevaluationLine 
                WHERE M_InventoryRevaluation_ID = {GetM_InventoryRevaluation_ID()} AND NewCostPrice = 0 ")) > 0)
            {
                _processMsg = Msg.GetMsg(GetCtx(), "EnterRevaluatedCost");
                log.Info(_processMsg);
                return DocActionVariables.STATUS_INVALID;
            }

            //
            _justPrepared = true;

            if (!DOCACTION_Complete.Equals(GetDocAction()))
                SetDocAction(DOCACTION_Complete);
            return DocActionVariables.STATUS_INPROGRESS;
        }

        /// <summary>
        /// Complete Document
        /// </summary>
        /// <returns>new status (Complete, In Progress, Invalid, Waiting ..)</returns>
        public string CompleteIt()
        {
            //	Re-Check
            if (!_justPrepared)
            {
                String status = PrepareIt();
                if (!DocActionVariables.STATUS_INPROGRESS.Equals(status))
                    return status;
            }

            // Get Accounting Schema Cost Type
            acctSchema = MAcctSchema.Get(GetCtx(), GetC_AcctSchema_ID());
            int M_CostType_ID = acctSchema.GetM_CostType_ID();

            // Get Product Cost Element based on selected Costing Method
            M_CostElement_ID = MCostElement.GetMaterialCostElement(GetCtx(), GetCostingMethod()).GetM_CostElement_ID();

            // Get Cost Element ID's binded on product Category
            GetCostElements();

            // Get Product Transaction Details
            GetProductTransaction();

            MRevaluationLine[] lines = GetLines(true);
            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                RevaluateProductCost(lines[lineIndex], M_CostType_ID);
                if (!string.IsNullOrEmpty(_processMsg))
                {
                    return DocActionVariables.STATUS_INVALID;
                }
            }

            String valid = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_AFTER_COMPLETE);
            if (valid != null)
            {
                _processMsg = valid;
                return DocActionVariables.STATUS_INVALID;
            }

            SetProcessed(true);
            SetDocAction(DOCACTION_Close);
            return DocActionVariables.STATUS_COMPLETED;
        }

        /// <summary>
        /// Update Revaluated Cost
        /// DevOps Task - FEATURE 1995
        /// </summary>
        /// <param name="objRevaluationLine">Revaluation Line Object</param>
        /// <param name="M_CostType_ID">Cost Type</param>
        /// <returns>true, when updated lines</returns>
        private string RevaluateProductCost(MRevaluationLine objRevaluationLine, int M_CostType_ID)
        {
            combCost = 0;
            productCost = 0;
            // Update Cost Queue Cost
            if (UpdateCostQueue(objRevaluationLine))
            {
                // Update Product Costs - CurrentCostPrice and AccumulationCost
                if (UpdateproductCost(objRevaluationLine, M_CostType_ID, out productCost))
                {
                    // Update Cost Combination
                    if (UpdateCostCombination(objRevaluationLine, out combCost))
                    {
                        // Create Transaction
                        CreateRevaluationTransaction(objRevaluationLine, (combCost != 0 ? combCost : productCost));
                    }
                    else
                    {
                        _processMsg = Msg.GetMsg(GetCtx(), "IRCostCombNotUpdated");
                    }
                }
                else
                {
                    _processMsg = Msg.GetMsg(GetCtx(), "IRProdCostNotUpdated");
                }
            }
            else
            {
                _processMsg = Msg.GetMsg(GetCtx(), "IRQueueNotUpdated");
            }
            return "";
        }

        /// <summary>
        /// Update Product Costs line with Revaluated Cost
        /// </summary>
        /// <param name="objRevaluationLine">Revaluation Line Object</param>
        /// <param name="M_CostType_ID">Cost Type</param>
        /// <returns>true, when updated lines</returns>
        private bool UpdateproductCost(MRevaluationLine objRevaluationLine, int M_CostType_ID, out decimal productCost)
        {
            sql.Clear();
            productCost = Decimal.Round(GetProductCost(objRevaluationLine), acctSchema.GetCostingPrecision(), MidpointRounding.AwayFromZero);
            sql.Clear();
            sql.Append($@"UPDATE M_Cost SET CurrentCostPrice = {productCost}, 
                            CumulatedAmt  = NVL(CumulatedAmt, 0) + {objRevaluationLine.GetTotalDifference()}, 
                            Updated = {GlobalVariable.TO_DATE(DateTime.Now, false)},
                            UpdatedBy = {GetCtx().GetAD_User_ID()}
                           WHERE C_AcctSchema_ID = {GetC_AcctSchema_ID()} 
                            AND M_Product_ID = {objRevaluationLine.GetM_Product_ID()}
                            AND AD_Client_ID = {GetAD_Client_ID()}
                            AND M_CostType_ID = {M_CostType_ID}
                            AND M_CostElement_ID = {M_CostElement_ID}");
            if (GetCostingLevel().Equals(COSTINGLEVEL_Organization) ||
                GetCostingLevel().Equals(COSTINGLEVEL_OrgPlusBatch) ||
                GetCostingLevel().Equals(COSTINGLEVEL_Warehouse) ||
                GetCostingLevel().Equals(COSTINGLEVEL_WarehousePlusBatch))
            {
                sql.Append($" AND AD_Org_ID = {GetAD_Org_ID()}");
            }
            if (GetCostingLevel().Equals(COSTINGLEVEL_OrgPlusBatch) ||
                GetCostingLevel().Equals(COSTINGLEVEL_BatchLot) ||
                GetCostingLevel().Equals(COSTINGLEVEL_WarehousePlusBatch))
            {
                sql.Append($@" AND NVL(M_AttributeSetInstance_ID , 0) = {objRevaluationLine.GetM_AttributeSetInstance_ID()}");
            }
            if (GetCostingLevel().Equals(COSTINGLEVEL_Warehouse) ||
                GetCostingLevel().Equals(COSTINGLEVEL_WarehousePlusBatch))
            {
                sql.Append($@" AND NVL(M_Warehouse_ID , 0) = {GetM_Warehouse_ID()}");
            }
            no = DB.ExecuteQuery(sql.ToString(), null, Get_Trx());
            if (no < 0)
            {
                log.Log(Level.WARNING, $@"Product Costs not updated, 
                                          Inventory Revaluation ID = {GetM_InventoryRevaluation_ID()},  
                                          Revlaution Line ID = {objRevaluationLine.GetM_RevaluationLine_ID()},
                                          C_AcctSchema_ID = {GetC_AcctSchema_ID()},  
                                          M_Product_ID = {objRevaluationLine.GetM_Product_ID()}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get Product Costs from Cost Queue based on Costing Level or Material Policy defined on Product Category
        /// </summary>
        /// <param name="objRevaluationLine">Inventory Revaluation Line</param>
        /// <returns>Product Costs</returns>
        private decimal GetProductCost(MRevaluationLine objRevaluationLine)
        {
            decimal cost = 0;

            bool isMMPolicyFIFO = true;
            if (GetCostingMethod().Equals(MInventoryRevaluation.COSTINGMETHOD_Fifo))
            {
                isMMPolicyFIFO = true;
            }
            else if (GetCostingMethod().Equals(MInventoryRevaluation.COSTINGMETHOD_Lifo))
            {
                isMMPolicyFIFO = false;
            }
            else
            {
                MProductCategory pc = MProductCategory.Get(GetCtx(), objRevaluationLine.GetM_Product_Category_ID());
                if (pc.GetMMPolicy().Equals(MProductCategory.MMPOLICY_FiFo))
                {
                    isMMPolicyFIFO = true;
                }
                else
                {
                    isMMPolicyFIFO = false;
                }
            }

            sql.Clear();
            sql.Append(@"SELECT");
            if (GetCostingMethod().Equals(MInventoryRevaluation.COSTINGMETHOD_Fifo) ||
                GetCostingMethod().Equals(MInventoryRevaluation.COSTINGMETHOD_Lifo))
            {
                sql.Append(" cq.CurrentCostPrice");
            }
            else
            {
                sql.Append(@" CASE WHEN SUM(cq.CurrentQty) = 0
                                   THEN 0 ELSE ROUND(SUM(cq.CurrentQty * cq.currentcostprice) / SUM(cq.CurrentQty), 10)
                              END AS CurrentCostPrice ");
            }
            sql.Append(" FROM M_CostQueue cq ");

            // Where Clause
            sql.Append($@" WHERE cq.CurrentQty <> 0 AND cq.C_AcctSchema_ID = {GetC_AcctSchema_ID()} 
                            AND cq.M_Product_ID = {objRevaluationLine.GetM_Product_ID()}
                            AND cq.AD_Client_ID = {GetAD_Client_ID()}");

            if (GetCostingLevel().Equals(COSTINGLEVEL_Organization) ||
                GetCostingLevel().Equals(COSTINGLEVEL_OrgPlusBatch) ||
                GetCostingLevel().Equals(COSTINGLEVEL_Warehouse) ||
                GetCostingLevel().Equals(COSTINGLEVEL_WarehousePlusBatch))
            {
                sql.Append($" AND cq.AD_Org_ID = {GetAD_Org_ID()}");
            }

            if (GetCostingLevel().Equals(COSTINGLEVEL_OrgPlusBatch) ||
                GetCostingLevel().Equals(COSTINGLEVEL_BatchLot) ||
                GetCostingLevel().Equals(COSTINGLEVEL_WarehousePlusBatch))
            {
                sql.Append($@" AND NVL(cq.M_AttributeSetInstance_ID , 0) = {objRevaluationLine.GetM_AttributeSetInstance_ID()}");
            }

            if (GetCostingLevel().Equals(COSTINGLEVEL_Warehouse) ||
                GetCostingLevel().Equals(COSTINGLEVEL_WarehousePlusBatch))
            {
                sql.Append($@" AND NVL(cq.M_Warehouse_ID , 0) = {GetM_Warehouse_ID()}");
            }

            // Order BY
            if (GetCostingMethod().Equals(MInventoryRevaluation.COSTINGMETHOD_Fifo) ||
                GetCostingMethod().Equals(MInventoryRevaluation.COSTINGMETHOD_Lifo))
            {
                sql.Append(@" ORDER BY cq.QueueDate ");
                if (!isMMPolicyFIFO)
                {
                    sql.Append("DESC ");
                }
                sql.Append(@" , cq.M_AttributeSetInstance_ID ");
                if (!isMMPolicyFIFO)
                {
                    sql.Append("DESC ");
                }
            }

            cost = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));
            return cost;
        }

        /// <summary>
        /// Update Cost Queue line with Revaluated Cost
        /// </summary>
        /// <param name="objRevaluationLine">Revaluation Line Object</param>
        /// <returns>true, when updated lines</returns>
        private bool UpdateCostQueue(MRevaluationLine objRevaluationLine)
        {
            sql.Clear();
            #region Update Cost Queue
            sql.Append($@" UPDATE M_CostQueue SET CurrentCostPrice= {objRevaluationLine.GetNewCostPrice()}, 
                            Updated = {GlobalVariable.TO_DATE(DateTime.Now, false)},
                            UpdatedBy = {GetCtx().GetAD_User_ID()}
                           WHERE C_AcctSchema_ID = {GetC_AcctSchema_ID()} 
                            AND M_Product_ID = {objRevaluationLine.GetM_Product_ID()}
                            AND AD_Client_ID = {GetAD_Client_ID()}");
            sql.Append($@" AND CurrentQty <> 0 ");

            if (GetCostingLevel().Equals(COSTINGLEVEL_Organization) ||
                GetCostingLevel().Equals(COSTINGLEVEL_OrgPlusBatch) ||
                GetCostingLevel().Equals(COSTINGLEVEL_Warehouse) ||
                GetCostingLevel().Equals(COSTINGLEVEL_WarehousePlusBatch))
            {
                sql.Append($" AND AD_Org_ID = {GetAD_Org_ID()}");
            }
            if (GetCostingLevel().Equals(COSTINGLEVEL_OrgPlusBatch) ||
                GetCostingLevel().Equals(COSTINGLEVEL_BatchLot) ||
                GetCostingLevel().Equals(COSTINGLEVEL_WarehousePlusBatch))
            {
                sql.Append($@" AND NVL(M_AttributeSetInstance_ID , 0) = {objRevaluationLine.GetM_AttributeSetInstance_ID()}");
            }
            if (GetCostingLevel().Equals(COSTINGLEVEL_Warehouse) ||
                GetCostingLevel().Equals(COSTINGLEVEL_WarehousePlusBatch) ||
                GetM_Warehouse_ID() > 0)
            {
                sql.Append($@" AND NVL(M_Warehouse_ID , 0) = {GetM_Warehouse_ID()}");
            }
            no = DB.ExecuteQuery(sql.ToString(), null, Get_Trx());
            if (no < 0)
            {
                log.Log(Level.WARNING, $@"Cost Queue not updated, 
                                          Inventory Revaluation ID = {GetM_InventoryRevaluation_ID()},  
                                          Revlaution Line ID = {objRevaluationLine.GetM_RevaluationLine_ID()},
                                          C_AcctSchema_ID = {GetC_AcctSchema_ID()},  
                                          M_Product_ID = {objRevaluationLine.GetM_Product_ID()}");
                return false;
            }
            #endregion 

            // Update temp Table with the revaluated cost
            no = DB.ExecuteQuery(sql.ToString().Replace("M_CostQueue", "T_Temp_CostDetail"), null, Get_Trx());

            return true;
        }

        /// <summary>
        /// Calculate Cost Combination
        /// </summary>
        /// <param name="objRevaluationLine">Revaluation Line Object</param>
        /// <returns>true, when combination updated</returns>
        private bool UpdateCostCombination(MRevaluationLine objRevaluationLine, out Decimal ProductCost)
        {
            ProductCost = 0;
            if (dsCostElement != null && dsCostElement.Tables.Count > 0 && dsCostElement.Tables[0].Rows.Count > 0)
            {
                drCostElement = dsCostElement.Tables[0].Select($"M_Product_Category_ID = {objRevaluationLine.GetM_Product_Category_ID()}");
                if (drCostElement != null && drCostElement.Length > 0 &&
                    (Util.GetValueOfString(drCostElement[0]["costingmethod"])).Equals(MCostElement.COSTINGMETHOD_CostCombination))
                {
                    // Binded Cost Element either on Product Category or on Accounting Schema
                    Binded_CostElement_ID = Util.GetValueOfInt(drCostElement[0]["M_CostElement_ID"]);

                    // Get Cost element of Cost Combination type
                    sql.Clear();
                    sql.Append($@"SELECT ce.M_CostElement_ID ,  ce.Name ,  cel.lineno ,  cel.m_ref_costelement , 
                                 (SELECT CASE  WHEN costingmethod IS NOT NULL THEN 1  ELSE 0 END  FROM m_costelement 
                                  WHERE m_costelement_id = CAST(cel.M_Ref_CostElement AS INTEGER) ) AS iscostMethod 
                              FROM M_CostElement ce INNER JOIN m_costelementline cel ON ce.M_CostElement_ID = cel.M_CostElement_ID 
                              WHERE ce.AD_Client_ID = {GetAD_Client_ID()}
                                    AND ce.IsActive='Y' AND ce.CostElementType='C' AND cel.IsActive='Y' ");
                    sql.Append(" AND ce.M_CostElement_ID = " + Binded_CostElement_ID);
                    sql.Append("ORDER BY ce.M_CostElement_ID , iscostMethod DESC");
                    DataSet ds = new DataSet();
                    ds = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                    try
                    {
                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            MCost costCombination = null;
                            MCostElement ce = null;
                            MCost cost = null;
                            MProduct product = new MProduct(GetCtx(), objRevaluationLine.GetM_Product_ID(), Get_Trx());
                            MAcctSchema acctSchema = MAcctSchema.Get(GetCtx(), GetC_AcctSchema_ID());
                            int costElementId = 0;

                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                // Reset Cost Combination cost impacts
                                if (Util.GetValueOfInt(ds.Tables[0].Rows[i]["iscostMethod"]) == 1)
                                {
                                    costCombination = MCost.Get(product, objRevaluationLine.GetM_AttributeSetInstance_ID(), acctSchema,
                                    ((GetCostingLevel().Equals(COSTINGLEVEL_Client) ||
                                     GetCostingLevel().Equals(COSTINGLEVEL_BatchLot)) ? 0 : GetAD_Org_ID()),
                                    Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_CostElement_ID"]),
                                    ((GetCostingLevel().Equals(COSTINGLEVEL_Warehouse) ||
                                      GetCostingLevel().Equals(COSTINGLEVEL_WarehousePlusBatch)) ? GetM_Warehouse_ID() : 0));

                                    costCombination.SetCurrentCostPrice(0);
                                    costCombination.SetCurrentQty(0);
                                    costCombination.SetCumulatedAmt(0);
                                    costCombination.SetCumulatedQty(0);
                                    costCombination.Save();
                                }
                            }

                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                costElementId = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_CostElement_ID"]);

                                // created object of Cost element for checking iscalculated = true/ false
                                ce = MCostElement.Get(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_ref_costelement"]));

                                // if m_ref_costelement is of Freight type then current cost against this record is :: 
                                costCombination = MCost.Get(product, objRevaluationLine.GetM_AttributeSetInstance_ID(), acctSchema,
                                    ((GetCostingLevel() == MProductCategory.COSTINGLEVEL_Client ||
                                     GetCostingLevel() == MProductCategory.COSTINGLEVEL_BatchLot) ? 0 : GetAD_Org_ID()),
                                    Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_CostElement_ID"]),
                                    ((GetCostingLevel().Equals(COSTINGLEVEL_Warehouse) ||
                                      GetCostingLevel().Equals(COSTINGLEVEL_WarehousePlusBatch)) ? GetM_Warehouse_ID() : 0));

                                cost = MCost.Get(product, objRevaluationLine.GetM_AttributeSetInstance_ID(), acctSchema,
                                    ((GetCostingLevel() == MProductCategory.COSTINGLEVEL_Client ||
                                     GetCostingLevel() == MProductCategory.COSTINGLEVEL_BatchLot) ? 0 : GetAD_Org_ID()),
                                    Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_ref_costelement"]),
                                    ((GetCostingLevel().Equals(COSTINGLEVEL_Warehouse) ||
                                      GetCostingLevel().Equals(COSTINGLEVEL_WarehousePlusBatch)) ? GetM_Warehouse_ID() : 0));

                                // if m_ref_costelement is of Freight type then current cost against this record is :: 
                                // Formula : (Freight Current Cost * Freight Current Qty) / Current Qty (whose costing method is available)
                                costCombination.SetCurrentCostPrice(Decimal.Round(Decimal.Add(costCombination.GetCurrentCostPrice(),
                                                                    cost.GetCurrentCostPrice()), acctSchema.GetCostingPrecision()));

                                costCombination.SetCumulatedAmt(Decimal.Add(costCombination.GetCumulatedAmt(), cost.GetCumulatedAmt()));

                                // if calculated = true then we added qty else not and costing method is Standard Costing
                                if (ce.IsCalculated() || ce.GetCostingMethod() == MCostElement.COSTINGMETHOD_StandardCosting)
                                {
                                    costCombination.SetCurrentQty(Decimal.Add(costCombination.GetCurrentQty(), cost.GetCurrentQty()));
                                    costCombination.SetCumulatedQty(Decimal.Add(costCombination.GetCumulatedQty(), cost.GetCumulatedQty()));
                                }
                                costCombination.Save();
                            }
                            ProductCost = costCombination.GetCurrentCostPrice();
                        }
                    }
                    catch { }
                }
            }
            return true;
        }

        /// <summary>
        /// Get Binded Cost Element Details on Product Category or Accounting Schema
        /// </summary>
        /// <returns></returns>
        private DataSet GetCostElements()
        {
            sql.Clear();
            sql.Append($@" (SELECT DISTINCT 
                            CASE WHEN (pc.costingmethod IS NOT NULL AND pc.costingmethod = 'C') THEN pc.M_CostElement_ID
                                 WHEN (pc.costingmethod IS NOT NULL AND pc.costingmethod  <> 'C') THEN 
                                 (SELECT M_CostElement_ID FROM M_CostElement WHERE CostingMethod = pc.CostingMethod AND AD_Client_ID = {GetAD_Client_ID()})
                                 WHEN ( acct.costingmethod IS NOT NULL AND  acct.costingmethod = 'C') THEN acct.M_CostElement_ID
                                 ELSE (SELECT M_CostElement_ID FROM M_CostElement WHERE CostingMethod = acct.CostingMethod AND AD_Client_ID =  {GetAD_Client_ID()})
                                 END AS M_CostElement_ID, 
                            CASE WHEN (pc.costingmethod IS NOT NULL) THEN pc.costingmethod
                                 ELSE acct.costingmethod END AS costingmethod, 
                            pc.M_Product_Category_ID, 
                            acct.C_AcctSchema_ID
                            FROM M_Product_Category pc 
                            INNER JOIN C_AcctSchema acct ON (acct.AD_Client_ID = pc.AD_Client_ID AND {GetC_AcctSchema_ID()} = acct.C_AcctSchema_ID)
                            WHERE pc.IsActive = 'Y' and pc.Ad_Client_id = {GetAD_Client_ID()}");
            sql.Append($@" AND pc.M_Product_Category_ID IN 
                           (SELECT M_Product_Category_ID FROM M_RevaluationLine WHERE M_InventoryRevaluation_ID = {GetM_InventoryRevaluation_ID()})");
            sql.Append(")");
            dsCostElement = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
            return dsCostElement;
        }

        /// <summary>
        /// Create Tranaction Line for Inventory Revaluation
        /// </summary>
        /// <param name="objRevaluationLine">Revaluation Line</param>
        /// <param name="ProductCost">Product Costs</param>
        /// <returns>Error Message if any</returns>
        private string CreateRevaluationTransaction(MRevaluationLine objRevaluationLine, decimal ProductCost)
        {
            MTransaction objTransaction = null;
            if (dsProdutTransaction != null && dsProdutTransaction.Tables.Count > 0 && dsProdutTransaction.Tables[0].Rows.Count > 0)
            {
                if (!(GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Client) ||
                    GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Organization) ||
                   GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Warehouse)))
                {
                    // Get data Atribute wise when costing level belong to Batch
                    drProdutTransaction = dsProdutTransaction.Tables[0].Select($@"M_Product_ID = {objRevaluationLine.GetM_Product_ID()} AND 
                                            M_AttributeSetInstance_ID = {objRevaluationLine.GetM_AttributeSetInstance_ID()}");
                }
                else
                {
                    drProdutTransaction = dsProdutTransaction.Tables[0].Select($@"M_Product_ID = {objRevaluationLine.GetM_Product_ID()}");
                }
                if (drProdutTransaction != null && drProdutTransaction.Length > 0)
                {
                    for (int i = 0; i < drProdutTransaction.Length; i++)
                    {
                        objTransaction = new MTransaction(GetCtx(), 0, Get_Trx());
                        objTransaction.SetAD_Client_ID(Util.GetValueOfInt(drProdutTransaction[i]["AD_Client_ID"]));
                        objTransaction.SetAD_Org_ID(Util.GetValueOfInt(drProdutTransaction[i]["AD_Org_ID"]));
                        objTransaction.SetM_Locator_ID(Util.GetValueOfInt(drProdutTransaction[i]["M_Locator_ID"]));
                        objTransaction.SetM_ProductContainer_ID(Util.GetValueOfInt(drProdutTransaction[i]["M_ProductContainer_ID"]));
                        objTransaction.SetM_Product_ID(Util.GetValueOfInt(drProdutTransaction[i]["M_Product_ID"]));
                        objTransaction.SetM_AttributeSetInstance_ID(Util.GetValueOfInt(drProdutTransaction[i]["M_AttributeSetInstance_ID"]));
                        objTransaction.SetCurrentQty(Util.GetValueOfDecimal(drProdutTransaction[i]["CurrentQty"]));
                        objTransaction.SetContainerCurrentQty(Util.GetValueOfDecimal(drProdutTransaction[i]["ContainerCurrentQty"]));
                        objTransaction.Set_Value("CostingLevel", Util.GetValueOfString(drProdutTransaction[i]["CostingLevel"]));
                        objTransaction.Set_Value("M_CostElement_ID", Util.GetValueOfInt(drProdutTransaction[i]["M_CostElement_ID"]));
                        objTransaction.Set_Value("ProductCost", ProductCost);
                        objTransaction.Set_Value("M_RevaluationLine_ID", objRevaluationLine.GetM_RevaluationLine_ID());
                        objTransaction.Set_ValueNoCheck("MovementType", "IR");
                        if (!objTransaction.Save())
                        {
                            ValueNamePair vp = VLogger.RetrieveError();
                            string val = "";
                            if (vp != null)
                            {
                                val = vp.GetName();
                                if (String.IsNullOrEmpty(val))
                                {
                                    val = vp.GetValue();
                                }
                            }
                            _processMsg = Msg.GetMsg(GetCtx(), "IRTrxNotSaved") + " " + (String.IsNullOrEmpty(val) ? "" : val);
                            log.Log(Level.SEVERE, "Transaction not created for Inventory Revaluation" + val);
                            return _processMsg;
                        }
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// Get Product Transaction Details
        /// </summary>
        /// <returns>DataSet Transaction Details</returns>
        private DataSet GetProductTransaction()
        {
            sql.Clear();
            sql.Append($@"SELECT AD_Client_ID, AD_Org_ID , M_Locator_ID, M_ProductContainer_ID, 
                                 M_Product_ID, M_AttributeSetInstance_ID, CurrentQty, ContainerCurrentQty,
                                 CostingLevel, M_CostElement_ID, ProductApproxCost, ProductCost
                          FROM M_Transaction WHERE M_Transaction_ID IN (
                            SELECT DISTINCT First_VALUE(t.M_Transaction_ID) OVER 
                                 (PARTITION BY t.M_Product_ID, t.M_AttributeSetInstance_ID, t.M_Locator_ID, NVL(t.M_ProductContainer_ID, 0) 
                                  ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS M_Transaction_ID 
                            FROM M_Transaction t 
                            INNER JOIN M_Locator l ON (t.M_Locator_ID = l.M_Locator_ID) 
                            INNER JOIN M_Warehouse w ON (w.M_Warehouse_ID = l.M_Warehouse_ID)
                            WHERE t.AD_Client_ID = {GetAD_Client_ID()} 
                            AND t.M_Product_ID IN (SELECT M_Product_ID FROM M_RevaluationLine WHERE M_InventoryRevaluation_ID = {GetM_InventoryRevaluation_ID()})
                            AND w.M_Warehouse_ID = {GetM_Warehouse_ID()}");

            if (!(GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Client) ||
                  GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Organization) ||
                 GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Warehouse)))
            {
                sql.Append($@" AND NVL(M_AttributeSetInstance_ID, 0) IN (SELECT NVL(M_AttributeSetInstance_ID, 0) FROM M_RevaluationLine 
                                WHERE M_InventoryRevaluation_ID = {GetM_InventoryRevaluation_ID()}) ");
            }
            sql.Append(")");
            dsProdutTransaction = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
            return dsProdutTransaction;
        }

        /// <summary>
        /// Set Processed value (Own Document and Inventory Revaluation Line)
        /// </summary>
        /// <param name="processed">true/false</param>
        public new void SetProcessed(bool processed)
        {
            base.SetProcessed(processed);
            if (Get_ID() == 0)
                return;

            no = DB.ExecuteQuery($@"UPDATE M_RevaluationLine SET Processed = 
                         {GlobalVariable.TO_STRING(processed ? "Y" : "N")} 
                         WHERE M_InventoryRevaluation_ID = {GetM_InventoryRevaluation_ID()}", null, Get_Trx());
            _lines = null;
            log.Fine(processed + " - Inventory Revaluation Lines=" + no);
        }

        /// <summary>
        /// Create PDF
        /// </summary>
        /// <returns>File or null</returns>
        public FileInfo CreatePDF()
        {
            return null;
        }

        /// <summary>
        /// Approval Amount
        /// </summary>
        /// <returns>0</returns>
        public decimal GetApprovalAmt()
        {
            return 0;
        }

        /// <summary>
        /// Get Document's DocBaseType
        /// </summary>
        /// <returns>DocBaseType</returns>
        public string GetDocBaseType()
        {
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
            return dt.GetDocBaseType();
        }

        /// <summary>
        /// Get Document Date
        /// </summary>
        /// <returns>Document Date</returns>
        public DateTime? GetDocumentDate()
        {
            return GetDateDoc();
        }

        /// <summary>
        /// Get Document Info
        /// </summary>
        /// <returns>document info (untranslated)</returns>
        public string GetDocumentInfo()
        {
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
            return dt.GetName() + " " + GetDocumentNo();
        }

        /// <summary>
        /// Get Document Owner (Responsible)
        /// </summary>
        /// <returns>AD_User_ID</returns>
        public int GetDoc_User_ID()
        {
            return GetUpdatedBy();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>null</returns>
        public Env.QueryParams GetLineOrgsQueryInfo()
        {
            return null;
        }

        /// <summary>
        /// Get Process Message
        /// </summary>
        /// <returns>clear text error message</returns>
        public string GetProcessMsg()
        {
            return _processMsg;
        }

        /// <summary>
        /// Get Summary
        /// </summary>
        /// <returns>Summary of Document</returns>
        public string GetSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetDocumentNo());
            if (GetDescription() != null && GetDescription().Length > 0)
                sb.Append(" - ").Append(GetDescription());
            return sb.ToString();
        }

        /// <summary>
        /// Approve Document
        /// </summary>
        /// <returns>true if success</returns>
        public bool ApproveIt()
        {
            log.Info(ToString());
            SetIsApproved(true);
            return true;
        }

        /// <summary>
        /// Close Document
        /// </summary>
        /// <returns>true if success</returns>
        public bool CloseIt()
        {
            log.Info(ToString());
            SetDocAction(DOCACTION_None);
            return true;
        }

        /// <summary>
        /// Invalidate Document
        /// </summary>
        /// <returns>true if success</returns>
        public bool InvalidateIt()
        {
            log.Info(ToString());
            SetDocAction(DOCACTION_Prepare);
            return true;
        }

        /// <summary>
        /// Re-activate
        /// </summary>
        /// <returns>false</returns>
        public bool ReActivateIt()
        {
            log.Info(ToString());
            return false;
        }

        /// <summary>
        /// Reject Approval
        /// </summary>
        /// <returns>true if success</returns>
        public bool RejectIt()
        {
            log.Info(ToString());
            SetIsApproved(false);
            return true;
        }

        /// <summary>
        /// Reverse Accrual - none
        /// </summary>
        /// <returns>false</returns>
        public bool ReverseAccrualIt()
        {
            log.Info(ToString());
            return false;
        }

        /// <summary>
        /// Reverse Correction
        /// </summary>
        /// <returns>true if success</returns>
        public bool ReverseCorrectIt()
        {
            _processMsg = Msg.GetMsg(GetCtx(), "ReversalNotAllow");
            log.Info(_processMsg);
            return false;
        }

        /// <summary>
        /// Unlock  document
        /// </summary>
        /// <returns>true</returns>
        public bool UnlockIt()
        {
            log.Info(ToString());
            SetProcessing(false);
            return true;
        }

        /// <summary>
        /// Void Document.
        /// </summary>
        /// <returns>true if success</returns>
        public bool VoidIt()
        {
            log.Info(ToString());
            if (DOCSTATUS_Closed.Equals(GetDocStatus())
                || DOCSTATUS_Reversed.Equals(GetDocStatus())
                || DOCSTATUS_Voided.Equals(GetDocStatus()))
            {
                _processMsg = "Document Closed: " + GetDocStatus();
                log.Info(_processMsg);
                return false;
            }
            else if (DOCSTATUS_Completed.Equals(GetDocStatus()))
            {
                _processMsg = Msg.GetMsg(GetCtx(), "ReversalNotAllow") + GetDocStatus();
                log.Info(_processMsg);
                return false;
            }
            else if (DOCSTATUS_Drafted.Equals(GetDocStatus())
                || DOCSTATUS_Invalid.Equals(GetDocStatus())
                || DOCSTATUS_InProgress.Equals(GetDocStatus())
                || DOCSTATUS_Approved.Equals(GetDocStatus())
                || DOCSTATUS_NotApproved.Equals(GetDocStatus()))
            {
                //todo
                _processMsg = "VoidedSuccessfully";
            }
            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            return true;
        }

        /// <summary>
        /// Add to Description
        /// </summary>
        /// <param name="description">text</param>
        public void AddDescription(String description)
        {
            String desc = GetDescription();
            if (desc == null)
                SetDescription(description);
            else
                SetDescription(desc + " | " + description);
        }
    }
}
