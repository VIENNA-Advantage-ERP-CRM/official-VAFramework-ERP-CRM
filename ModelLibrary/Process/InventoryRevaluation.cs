/********************************************************
 * Project Name   : VAdvantage
 * Module Name    : ModelLibrary
 * Class Name     : InventoryRevaluation
 * Purpose        : Revaluate Product Costs
 * Class Used     : none
 * Chronological  : Development
 * VIS_0045       : 10-March-2023
  ******************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;

namespace VAdvantage.Process
{
    public class InventoryRevaluation : SvrProcess
    {
        private MInventoryRevaluation objInventoryRevaluation = null;
        private MRevaluationLine objRevaluationLine = null;
        StringBuilder sql = new StringBuilder();
        private DataSet dsRevaluation = null;
        private int lineNo = 10;
        private int precision = 2;
        private StringBuilder errorMessage = new StringBuilder();

        /// <summary>
        /// Prepare Parameter
        /// </summary>
        protected override void Prepare()
        {
            ;
        }

        /// <summary>
        /// Implement functionality
        /// </summary>
        /// <returns>Process Message</returns>
        protected override string DoIt()
        {
            // Create header Object 
            objInventoryRevaluation = new MInventoryRevaluation(GetCtx(), GetRecord_ID(), Get_Trx());

            // Get Costing precision based on currency selected on Accounting Schema
            precision = MAcctSchema.Get(GetCtx(), objInventoryRevaluation.GetC_AcctSchema_ID()).GetCostingPrecision();

            // Remove Lines if exists
            DeleteRevaluationLines();

            // Create query for picking revaluation 
            CreateQueryForRevaluation();

            // Get Data for Revaluation
            dsRevaluation = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
            if (dsRevaluation != null && dsRevaluation.Tables.Count > 0 && dsRevaluation.Tables[0].Rows.Count > 0)
            {
                // Create Revaluation lines
                CreateRevaluationLine();
            }

            if (!string.IsNullOrEmpty(errorMessage.ToString()) && errorMessage.Length > 0)
            {
                return errorMessage.ToString();
            }
            return (lineNo == 10 ? 0 : ((lineNo - 10) / 10)) + Msg.GetMsg(GetCtx(), "RevaluationLineCreated");
        }

        /// <summary>
        /// Delete Revaluation Lines
        /// </summary>
        private void DeleteRevaluationLines()
        {
            DB.ExecuteQuery($"DELETE FROM M_RevaluationLine WHERE M_InventoryRevaluation_ID = {GetRecord_ID()}", null, Get_Trx());
        }

        /// <summary>
        /// Create Revaluation Lines
        /// </summary>
        /// <returns>Error Message if any</returns>
        private string CreateRevaluationLine()
        {
            for (int i = 0; i < dsRevaluation.Tables[0].Rows.Count; i++)
            {
                objRevaluationLine = new MRevaluationLine(GetCtx(), null, Get_Trx());
                objRevaluationLine.isUpdateHeader = false;
                objRevaluationLine.SetAD_Client_ID(objInventoryRevaluation.GetAD_Client_ID());
                objRevaluationLine.SetAD_Org_ID(objInventoryRevaluation.GetAD_Org_ID());
                objRevaluationLine.SetM_InventoryRevaluation_ID(objInventoryRevaluation.GetM_InventoryRevaluation_ID());
                objRevaluationLine.SetLineNo(lineNo);
                objRevaluationLine.SetM_Product_Category_ID(Util.GetValueOfInt(dsRevaluation.Tables[0].Rows[i]["M_Product_Category_ID"]));
                objRevaluationLine.SetM_Product_ID(Util.GetValueOfInt(dsRevaluation.Tables[0].Rows[i]["M_Product_ID"]));
                objRevaluationLine.SetC_UOM_ID(Util.GetValueOfInt(dsRevaluation.Tables[0].Rows[i]["C_UOM_ID"]));
                objRevaluationLine.SetM_AttributeSetInstance_ID(Util.GetValueOfInt(dsRevaluation.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]));
                objRevaluationLine.SetCostingMethod(objInventoryRevaluation.GetCostingMethod());
                objRevaluationLine.SetCostingLevel(objInventoryRevaluation.GetCostingLevel());
                objRevaluationLine.SetQtyOnHand(Util.GetValueOfDecimal(dsRevaluation.Tables[0].Rows[i]["TotalQty"]));
                objRevaluationLine.SetSalesPrice(Decimal.Round(Util.GetValueOfDecimal(dsRevaluation.Tables[0].Rows[i]["PriceStd"]),
                                    precision, MidpointRounding.AwayFromZero));
                objRevaluationLine.SetCurrentCostPrice(Decimal.Round(Util.GetValueOfDecimal(dsRevaluation.Tables[0].Rows[i]["CurrentCostPrice"]),
                                    precision, MidpointRounding.AwayFromZero));
                objRevaluationLine.SetNetRealizableValue(Decimal.Subtract(objRevaluationLine.GetSalesPrice(), objRevaluationLine.GetCurrentCostPrice()));
                if (objInventoryRevaluation.GetRevaluationType().Equals(X_M_InventoryRevaluation.REVALUATIONTYPE_OnSoldConsumedQuantity))
                {
                    objRevaluationLine.SetNewCostPrice(objRevaluationLine.GetCurrentCostPrice());
                    objRevaluationLine.SetDifferenceCostPrice(0);
                    objRevaluationLine.SetSoldQty(Decimal.Negate(Util.GetValueOfDecimal(dsRevaluation.Tables[0].Rows[i]["SoldQty"])));
                    objRevaluationLine.SetSoldValue(Decimal.Round(Decimal.Multiply(objRevaluationLine.GetSoldQty(), objRevaluationLine.GetCurrentCostPrice())
                                                      , precision, MidpointRounding.AwayFromZero));
                    objRevaluationLine.SetNewValue(Decimal.Round(Decimal.Multiply(objRevaluationLine.GetSoldQty(), objRevaluationLine.GetNewCostPrice())
                                                      , precision, MidpointRounding.AwayFromZero));
                    objRevaluationLine.SetDifferenceValue(Decimal.Subtract(objRevaluationLine.GetNewValue(), objRevaluationLine.GetSoldValue()));
                }
                if (!objRevaluationLine.Save())
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
                    ErrorMessage(dsRevaluation.Tables[0].Rows[i], val);
                    log.Log(Level.SEVERE, "Inventory Revaluation not saved " + errorMessage);
                }
                else
                {
                    lineNo += 10;
                }
            }

            // Update difference on Inventory revaluation Header
            if (objRevaluationLine != null && objRevaluationLine.Get_ID() > 0)
            {
                objRevaluationLine.UpdateHeader();
            }

            return "";
        }

        /// <summary>
        /// Error Message Display
        /// </summary>
        /// <param name="dr">Datarow</param>
        /// <param name="Reason">Error Message</param>
        private void ErrorMessage(DataRow dr, string Reason)
        {
            if (string.IsNullOrEmpty(errorMessage.ToString()) || errorMessage.Length <= 0)
            {
                errorMessage.Append(Msg.GetMsg(GetCtx(), "RevaluationLineNotSaved"));
            }
            else
            {
                errorMessage.Append(" , ");
            }
            if (objRevaluationLine.GetCostingLevel().Equals(X_M_InventoryRevaluation.COSTINGLEVEL_OrgPlusBatch) ||
                objRevaluationLine.GetCostingLevel().Equals(X_M_InventoryRevaluation.COSTINGLEVEL_BatchLot) ||
                objRevaluationLine.GetCostingLevel().Equals(X_M_InventoryRevaluation.COSTINGLEVEL_WarehousePlusBatch))
            {
                errorMessage.Append($@" M_Product_ID = {Util.GetValueOfInt(dr["M_Product_ID"])} - 
                                        M_AttributeSetInstance_ID = {Util.GetValueOfInt(dr["M_AttributeSetInstance_ID"])} ");
            }
            else
            {
                errorMessage.Append($" M_Product_ID = {Util.GetValueOfInt(dr["M_Product_ID"])}");
            }
            if (!string.IsNullOrEmpty(Reason))
            {
                errorMessage.Append($"({Reason})");
            }
        }

        /// <summary>
        /// Create query for Revaluation
        /// </summary>
        private void CreateQueryForRevaluation()
        {
            sql.Clear();

            #region Get Stock from Warehouse when On Available Quantity OR Trnsaction when On Sold Consumed Quantity
            sql.Append($@" With Stock AS ");
            if (objInventoryRevaluation.GetRevaluationType().Equals(MInventoryRevaluation.REVALUATIONTYPE_OnAvailableQuantity) ||
                objInventoryRevaluation.GetRevaluationType().Equals(MInventoryRevaluation.REVALUATIONTYPE_OnSoldConsumedQuantity))
            {
                sql.Append($@"(SELECT st.M_Product_ID ");

                // Organization
                if (objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Client) ||
                  objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_BatchLot))
                {
                    sql.Append(@" , 0 AS AD_Org_ID ");
                }
                else
                {
                    sql.Append(@" , st.AD_Org_ID ");
                }

                // Warehouse
                if (objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Warehouse) ||
                  objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_WarehousePlusBatch))
                {
                    sql.Append(@" , loc.M_Warehouse_ID ");
                }
                else
                {
                    sql.Append(@" , 0 AS M_Warehouse_ID ");
                }

                //Batch
                if (objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Client) ||
                    objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Organization) ||
                    objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Warehouse))
                {
                    sql.Append(@" , 0 AS M_AttributeSetInstance_ID ");
                }
                else
                {
                    sql.Append(@" , NVL(st.M_AttributeSetInstance_ID, 0) AS M_AttributeSetInstance_ID ");
                }

                sql.Append(" , SUM(st.QtyOnhand) as TotalQty ");

                sql.Append($@" FROM M_Warehouse w
                            INNER JOIN M_Locator loc ON (loc.M_Warehouse_ID = w.M_Warehouse_ID)
                            INNER JOIN M_Storage st ON (st.M_Locator_ID = loc.M_Locator_ID)
                            INNER JOIN M_Product p ON (p.M_Product_ID = st.M_Product_ID)
                            WHERE st.QtyOnhand 
                {(objInventoryRevaluation.GetRevaluationType().Equals(MInventoryRevaluation.REVALUATIONTYPE_OnAvailableQuantity) ? " > " : " >= ")} 0 ");

                sql.Append($@" AND st.AD_Client_ID = { objInventoryRevaluation.GetAD_Client_ID()}");
                if (objInventoryRevaluation.GetM_Product_ID() > 0)
                {
                    sql.Append($@" AND st.M_Product_ID = {objInventoryRevaluation.GetM_Product_ID()}");
                }
                if (objInventoryRevaluation.GetM_Product_Category_ID() > 0)
                {
                    sql.Append($@" AND p.M_Product_Category_ID = {objInventoryRevaluation.GetM_Product_Category_ID()}");
                }
                // when Costing Level is not Clinet OR Batch/Lot then check AD_Org Check 
                if (!(objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Client) ||
                   objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_BatchLot))
                   && objInventoryRevaluation.GetAD_Org_ID() > 0)
                {
                    sql.Append($@" AND st.AD_Org_ID = {objInventoryRevaluation.GetAD_Org_ID()}");
                }

                // when Costing Level is Warehouse or Warehouse+Batch then add warehouse id
                if (objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Warehouse) ||
                    objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_WarehousePlusBatch)
                   && objInventoryRevaluation.GetM_Warehouse_ID() > 0)
                {
                    sql.Append($@" AND loc.M_Warehouse_ID = {objInventoryRevaluation.GetM_Warehouse_ID()}");
                }

                // Group BY 
                sql.Append(@" GROUP BY st.M_Product_ID");

                // when Costing Level is not Clinet OR Batch/Lot then check AD_Org Check 
                if (!(objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Client) ||
                      objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_BatchLot)))
                {
                    sql.Append($@" , st.AD_Org_ID ");
                }

                // when Costing Level is not Clinet OR Org OR Warehouse then check AD_Org Check 
                if (!(objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Client) ||
                      objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Organization) ||
                      objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Warehouse)))
                {
                    sql.Append(@" , st.M_AttributeSetInstance_ID ");
                }

                // when Costing Level is Warehouse or Warehouse+Batch then add warehouse id
                if (objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Warehouse) ||
                   objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_WarehousePlusBatch))
                {
                    sql.Append($@" , loc.M_Warehouse_ID ");
                }
                sql.Append(" )");
            }

            if (objInventoryRevaluation.GetRevaluationType().Equals(MInventoryRevaluation.REVALUATIONTYPE_OnSoldConsumedQuantity))
            {
                sql.Append($@", Sales AS (
                            SELECT st.M_Product_ID ");

                // Organization
                if (objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Client) ||
                  objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_BatchLot))
                {
                    sql.Append(@" , 0 AS AD_Org_ID ");
                }
                else
                {
                    sql.Append(@" , st.AD_Org_ID ");
                }

                // Warehouse
                if (objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Warehouse) ||
                  objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_WarehousePlusBatch))
                {
                    sql.Append(@" , st.M_Warehouse_ID ");
                }
                else
                {
                    sql.Append(@" , 0 AS M_Warehouse_ID ");
                }

                //Batch
                if (objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Client) ||
                    objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Organization) ||
                    objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Warehouse))
                {
                    sql.Append(@" , 0 AS M_AttributeSetInstance_ID ");
                }
                else
                {
                    sql.Append(@" , NVL(st.M_AttributeSetInstance_ID, 0) AS M_AttributeSetInstance_ID ");
                }

                sql.Append(" , SUM(st.QtyOnhand) as TotalQty ");

                sql.Append($@" FROM (
                            SELECT st.AD_Client_ID, loc.AD_Org_ID, p.M_Product_Category_ID, st.M_Product_ID, 
                                   st.M_Attributesetinstance_ID, loc.M_Warehouse_ID, 
                                   SUM(CASE WHEN st.movementtype IN ('I+', 'I-') AND (inv.isinternaluse = 'N' OR inv.isinternaluse IS NULL) THEN 0 
                                            WHEN st.movementtype IN ('P-', 'P+') AND (pl.MaterialType = 'F' OR pl.MaterialType IS NULL) THEN 0 ");
                if (Env.IsModuleInstalled("VAMFG_"))
                {
                    sql.Append(@" WHEN st.movementtype IN ('W-', 'W+') AND 
                                  (wot.VAMFG_WorkOrderTxnType NOT IN ('CI' , 'CR') OR wot.VAMFG_WorkOrderTxnType IS NULL) THEN 0 ");
                }
                sql.Append($@" ELSE st.MovementQty END ) AS QtyOnhand
                               FROM M_Transaction st 
                               INNER JOIN M_Locator loc ON (loc.M_Locator_ID = st.M_Locator_ID)
                               INNER JOIN M_Product p ON (p.M_Product_ID = st.M_Product_ID)
                               INNER JOIN C_Period prd ON (prd.C_Period_ID = {objInventoryRevaluation.GetC_Period_ID()})
                               LEFT JOIN M_Inventoryline il ON (st.M_Inventoryline_ID = il.M_Inventoryline_ID and il.IsInternalUse = 'Y')
                               LEFT JOIN M_inventory inv ON (il.M_inventory_ID = inv.M_inventory_ID and inv.isinternaluse = 'Y')
                               LEFT JOIN M_ProductionLine pl ON (pl.M_ProductionLine_ID = st.M_ProductionLine_ID AND pl.MaterialType = 'C')");
                sql.Append(@" LEFT JOIN VAMFG_M_WrkOdrTrnsctionLine wotl ON (wotl.VAMFG_M_WrkOdrTrnsctionLine_ID = st.VAMFG_M_WrkOdrTrnsctionLine_ID)
                              LEFT JOIN VAMFG_M_WrkOdrTransaction wot ON (wot.VAMFG_M_WrkOdrTransaction_ID = wotl.VAMFG_M_WrkOdrTransaction_ID
                                                                          AND wot.VAMFG_WorkOrderTxnType IN ('CI' , 'CR'))");
                sql.Append($@" WHERE st.MovementType IN ('C-' , 'C+' , 'I+', 'I-', 'P-', 'P+' , 'W-', 'W+') 
                                     AND st.MovementDate BETWEEN prd.StartDate AND prd.EndDate ");
                sql.Append(@" GROUP BY st.AD_Client_ID, loc.AD_Org_ID, p.M_Product_Category_ID, st.M_Product_ID, 
                                   st.M_Attributesetinstance_ID, loc.M_Warehouse_ID ");
                sql.Append($@" )st WHERE st.QtyOnhand <> 0 ");
                sql.Append($@" AND st.AD_Client_ID = { objInventoryRevaluation.GetAD_Client_ID()}");
                if (objInventoryRevaluation.GetM_Product_ID() > 0)
                {
                    sql.Append($@" AND st.M_Product_ID = {objInventoryRevaluation.GetM_Product_ID()}");
                }
                if (objInventoryRevaluation.GetM_Product_Category_ID() > 0)
                {
                    sql.Append($@" AND st.M_Product_Category_ID = {objInventoryRevaluation.GetM_Product_Category_ID()}");
                }
                // when Costing Level is not Clinet OR Batch/Lot then check AD_Org Check 
                if (!(objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Client) ||
                   objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_BatchLot))
                   && objInventoryRevaluation.GetAD_Org_ID() > 0)
                {
                    sql.Append($@" AND st.AD_Org_ID = {objInventoryRevaluation.GetAD_Org_ID()}");
                }

                // when Costing Level is Warehouse or Warehouse+Batch then add warehouse id
                if (objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Warehouse) ||
                    objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_WarehousePlusBatch)
                   && objInventoryRevaluation.GetM_Warehouse_ID() > 0)
                {
                    sql.Append($@" AND st.M_Warehouse_ID = {objInventoryRevaluation.GetM_Warehouse_ID()}");
                }

                // Group BY 
                sql.Append(@" GROUP BY st.M_Product_ID");

                // when Costing Level is not Clinet OR Batch/Lot then check AD_Org Check 
                if (!(objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Client) ||
                      objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_BatchLot)))
                {
                    sql.Append($@" , st.AD_Org_ID ");
                }

                // when Costing Level is not Clinet OR Org OR Warehouse then check AD_Org Check 
                if (!(objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Client) ||
                      objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Organization) ||
                      objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Warehouse)))
                {
                    sql.Append(@" , st.M_AttributeSetInstance_ID ");
                }

                // when Costing Level is Warehouse or Warehouse+Batch then add warehouse id
                if (objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Warehouse) ||
                   objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_WarehousePlusBatch))
                {
                    sql.Append($@" , st.M_Warehouse_ID ");
                }
                sql.Append(" )");
            }
            #endregion

            #region Get Maximum Product Price from Price List of Base UOM 
            sql.Append($@", PriceList AS (
                            SELECT pp.M_Product_ID,");

            // ASI
            if (objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Client) ||
               objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Organization) ||
               objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Warehouse))
            {
                sql.Append(@" 0 AS M_AttributeSetInstance_ID, ");
            }
            else
            {
                sql.Append(@" NVL(pp.M_AttributeSetInstance_ID, 0) AS M_AttributeSetInstance_ID, ");
            }

            sql.Append($@" MAX(CASE WHEN pl.C_Currency_ID = {objInventoryRevaluation.GetC_Currency_ID()} THEN pp.PriceStd
                            ELSE NVL(CurrencyConvert(pp.PriceStd, pl.C_Currency_ID , {objInventoryRevaluation.GetC_Currency_ID()},
                            {GlobalVariable.TO_DATE(objInventoryRevaluation.GetDateAcct(), true)}, {objInventoryRevaluation.GetC_ConversionType_ID()},
                            {objInventoryRevaluation.GetAD_Client_ID()},{objInventoryRevaluation.GetAD_Org_ID()}) , 0) END) AS PriceStd
                            FROM M_PriceList pl
                            INNER JOIN M_PriceList_Version plv ON (pl.M_PriceList_ID = plv.M_PriceList_ID)
                            INNER JOIN M_ProductPrice pp ON (plv.M_PriceList_Version_ID = pp.M_PriceList_Version_ID)
                            INNER JOIN M_Product p ON (p.M_Product_ID = pp.M_Product_ID AND pp.C_UOM_ID = p.C_UOM_ID)
                            WHERE pl.IsSOPriceList = 'Y' AND pl.IsActive = 'Y'  AND plv.IsActive = 'Y'  AND pp.IsActive = 'Y' ");
            if (objInventoryRevaluation.GetM_Product_Category_ID() > 0)
            {
                sql.Append($@" AND p.M_Product_Category_ID = {objInventoryRevaluation.GetM_Product_Category_ID()}");
            }
            if (objInventoryRevaluation.GetM_Product_ID() > 0)
            {
                sql.Append($@" AND p.M_Product_ID = {objInventoryRevaluation.GetM_Product_ID()}");
            }
            sql.Append(" GROUP BY pp.M_Product_ID ");
            if (!(objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Client) ||
                  objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Organization) ||
                  objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Warehouse)))
            {
                sql.Append(@" , pp.M_AttributeSetInstance_ID ");
            }
            sql.Append(" )");
            #endregion

            #region Cost Element
            sql.Append($@" , CostElement AS 
                             (SELECT CASE WHEN t.CostingMethod = 'C' THEN comb.M_Costelement_Id ELSE t.M_CostElement_ID END AS M_CostElement_ID, 
                             t.M_Product_Category_ID , t.C_AcctSchema_ID, 
                             CASE WHEN t.CostingMethod = 'C' THEN comb.CostingMethod ELSE t.CostingMethod END AS costingMethod
                           FROM                                        
                            (SELECT DISTINCT 
                            CASE WHEN (pc.costingmethod IS NOT NULL AND pc.costingmethod = 'C') THEN pc.M_CostElement_ID
                                 WHEN (pc.costingmethod IS NOT NULL AND pc.costingmethod  <> 'C') THEN 
                                 (SELECT M_CostElement_ID FROM M_CostElement WHERE CostingMethod = pc.CostingMethod AND AD_Client_ID = {GetAD_Client_ID()})
                                 WHEN ( acct.costingmethod IS NOT NULL AND  acct.costingmethod = 'C') THEN acct.M_CostElement_ID
                                 ELSE (SELECT M_CostElement_ID FROM M_CostElement WHERE CostingMethod = acct.CostingMethod AND AD_Client_ID =  {GetAD_Client_ID()})
                                 END AS M_CostElement_ID, 
                            CASE WHEN ( pc.costingmethod IS NOT NULL ) THEN pc.costingmethod
                                 ELSE acct.costingmethod END AS costingmethod, 
                            pc.M_Product_Category_ID, acct.C_AcctSchema_ID
                            FROM M_Product_Category pc 
                            INNER JOIN C_AcctSchema acct ON (acct.AD_Client_ID = pc.AD_Client_ID 
                                        AND {objInventoryRevaluation.GetC_AcctSchema_ID()} = acct.C_AcctSchema_ID)
                            where pc.IsActive = 'Y' and pc.Ad_Client_id = {GetAD_Client_ID()})t
                            LEFT JOIN (
                                 (SELECT CAST(Cel.M_Ref_Costelement AS INTEGER) AS M_Costelement_Id, 
                                         cel.m_CostELement_ID AS CombinationID,  ced.CostingMethod 
                                  FROM M_CostElement ced  INNER JOIN M_Costelementline Cel ON (Ced.M_Costelement_Id = CAST(Cel.M_Ref_Costelement AS INTEGER))
                                  WHERE Ced.AD_Client_ID = {GetAD_Client_ID()} AND Ced.IsActive ='Y' AND ced.CostElementType ='M'
                                        AND Cel.IsActive ='Y'  AND ced.CostingMethod  IS NOT NULL )) comb ON (comb.CombinationID = t.M_CostElement_ID)
                            WHERE CASE WHEN t.CostingMethod = 'C' THEN comb.CostingMethod ELSE t.CostingMethod END
                                = {GlobalVariable.TO_STRING(objInventoryRevaluation.GetCostingMethod())}) ");
            #endregion

            sql.Append($@"SELECT P.M_Product_Category_ID, 
                                 P.M_Product_ID,
                                 P.C_UOM_ID,
                                 pl.PriceStd,
                                 stk.TotalQty");
            if (objInventoryRevaluation.GetCostingMethod().Equals(MInventoryRevaluation.COSTINGMETHOD_Fifo) ||
                objInventoryRevaluation.GetCostingMethod().Equals(MInventoryRevaluation.COSTINGMETHOD_Lifo))
            {
                sql.Append(" ,CASE WHEN SUM(cq.CurrentQty) = 0 THEN 0 ELSE ROUND(SUM(cq.CurrentQty * cq.currentcostprice) / SUM(cq.CurrentQty) , 10) END AS CurrentCostPrice");
            }
            else
            {
                sql.Append(@" ,cst.CurrentCostPrice ");
            }
            if (objInventoryRevaluation.GetRevaluationType().Equals(MInventoryRevaluation.REVALUATIONTYPE_OnSoldConsumedQuantity))
            {
                sql.Append(@"  ,s.TotalQty AS SoldQty");
            }

            //ASI
            if (objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Client) ||
                objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Organization) ||
                objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Warehouse))
            {
                sql.Append(@" ,0 AS M_AttributeSetInstance_ID ");
            }
            else
            {
                sql.Append(@" ,cst.M_AttributeSetInstance_ID ");
            }

            //Warehose
            if (objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Warehouse) ||
                objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_WarehousePlusBatch))
            {
                sql.Append(@" ,cst.M_Warehouse_ID ");
            }
            else
            {
                sql.Append(@" ,0 AS M_Warehouse_ID");
            }

            sql.Append($@" FROM M_Product P  
                           INNER JOIN M_Cost CST ON (P.M_Product_ID = CST.M_Product_ID)
                           INNER JOIN M_Product_Category PC ON (P.M_Product_Category_ID = PC.M_Product_Category_ID)
                           INNER JOIN C_AcctSchema ACC ON (CST.C_AcctSchema_ID = ACC.C_AcctSchema_ID)
                           INNER JOIN M_CostType ct ON (ct.M_CostType_ID = acc.M_CostType_ID AND ct.M_CostType_ID = cst.M_CostType_ID)
                           INNER JOIN CostElement CE ON (CST.M_CostElement_ID = CE.M_CostElement_ID AND CE.M_Product_Category_ID = PC.M_Product_Category_ID)");
            if (objInventoryRevaluation.GetCostingMethod().Equals(MInventoryRevaluation.COSTINGMETHOD_Fifo) ||
                objInventoryRevaluation.GetCostingMethod().Equals(MInventoryRevaluation.COSTINGMETHOD_Lifo))
            {
                sql.Append($@" INNER JOIN M_CostQueue cq ON (cq.M_Product_ID = CST.M_Product_ID 
                                AND cq.M_CostElement_ID = CE.M_CostElement_ID AND cq.C_AcctSchema_ID = {objInventoryRevaluation.GetC_AcctSchema_ID()}");
                if (!(objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Client) ||
                    objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Organization) ||
                    objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Warehouse)))
                {
                    sql.Append(" cq.M_AttributeSetInstance_ID = CST.M_AttributeSetInstance_ID ");
                }
                if (objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Warehouse) ||
                    objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_WarehousePlusBatch))
                {
                    sql.Append(@" cq.M_Warehouse_ID = CST.M_Warehouse_ID ");
                }
                if (objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Organization) ||
                    objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_OrgPlusBatch) ||
                    objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Warehouse) ||
                    objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_WarehousePlusBatch))
                {
                    sql.Append(@" cq.AD_Org_ID = CST.AD_Org_ID ");
                }
                sql.Append(" ) ");
            }
            sql.Append($@" LEFT JOIN PriceList pl ON (pl.M_Product_ID = cst.M_Product_ID AND pl.M_AttributeSetInstance_ID = cst.M_AttributeSetInstance_ID)
                           INNER JOIN Stock stk ON (stk.M_Product_ID = cst.M_Product_ID AND stk.M_AttributeSetInstance_ID = cst.M_AttributeSetInstance_ID
                                                    AND stk.M_Warehouse_ID = cst.M_Warehouse_ID)");

            if (objInventoryRevaluation.GetRevaluationType().Equals(MInventoryRevaluation.REVALUATIONTYPE_OnSoldConsumedQuantity))
            {
                sql.Append(@" INNER JOIN Sales s ON (s.M_Product_ID = cst.M_Product_ID AND s.M_AttributeSetInstance_ID = cst.M_AttributeSetInstance_ID
                                                    AND s.M_Warehouse_ID = cst.M_Warehouse_ID)");
            }

            sql.Append($@" WHERE acc.C_AcctSchema_ID = {objInventoryRevaluation.GetC_AcctSchema_ID()} 
                                 AND ce.CostingMethod = {GlobalVariable.TO_STRING(objInventoryRevaluation.GetCostingMethod())} 
                                 AND NVL(pc.CostingLevel, acc.CostingLevel) = {GlobalVariable.TO_STRING(objInventoryRevaluation.GetCostingLevel())} 
                           AND ((CASE WHEN {GlobalVariable.TO_STRING(objInventoryRevaluation.GetCostingLevel())} IN ('A' , 'O' , 'W' , 'D') 
                                      THEN {objInventoryRevaluation.GetAD_Org_ID()}
                                      ELSE 0 END) = CST.AD_Org_ID)");
            if (objInventoryRevaluation.GetM_Warehouse_ID() > 0)
            {
                sql.Append($@" AND ((CASE WHEN { GlobalVariable.TO_STRING(objInventoryRevaluation.GetCostingLevel())} IN ('W', 'D')
                                          THEN { objInventoryRevaluation.GetM_Warehouse_ID()}
                                          ELSE 0 END) = NVL(CST.M_Warehouse_ID, 0))");
            }
            if (objInventoryRevaluation.GetM_Product_Category_ID() > 0)
            {
                sql.Append($@" AND PC.M_Product_Category_ID = {objInventoryRevaluation.GetM_Product_Category_ID()}");
            }
            if (objInventoryRevaluation.GetM_Product_ID() > 0)
            {
                sql.Append($@" AND P.M_Product_ID = {objInventoryRevaluation.GetM_Product_ID()}");
            }
            if (objInventoryRevaluation.GetCostingMethod().Equals(MInventoryRevaluation.COSTINGMETHOD_Fifo) ||
                objInventoryRevaluation.GetCostingMethod().Equals(MInventoryRevaluation.COSTINGMETHOD_Lifo))
            {
                sql.Append(@" GROUP BY P.M_Product_Category_ID, 
                                 P.M_Product_ID,
                                 P.C_UOM_ID,
                                 pl.PriceStd,
                                 stk.TotalQty");
                if (objInventoryRevaluation.GetRevaluationType().Equals(MInventoryRevaluation.REVALUATIONTYPE_OnSoldConsumedQuantity))
                {
                    sql.Append(@"  ,s.TotalQty");
                }
                if (!(objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Client) ||
                    objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Organization) ||
                    objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Warehouse)))
                {
                    sql.Append(@" ,cst.M_AttributeSetInstance_ID ");
                }
                if (objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_Warehouse) ||
                   objInventoryRevaluation.GetCostingLevel().Equals(MAcctSchema.COSTINGLEVEL_WarehousePlusBatch))
                {
                    sql.Append(@" ,cst.M_Warehouse_ID ");
                }
            }
        }

    }
}
