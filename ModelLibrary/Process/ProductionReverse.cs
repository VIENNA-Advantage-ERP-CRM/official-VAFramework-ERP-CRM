/********************************************************
    * Project Name   : VAdvantage
    * Class Name     : ProductionReverse
    * Purpose        : Is used to do a reverse entry of "Production Record" into the system                    
    * Class Used     : ProcessEngine.SvrProcess
    * Chronological    Development
    * Amit Bansal     28-Feb-2019
******************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;

namespace VAdvantage.Process
{
    class ProductionReverse : SvrProcess
    {
        private StringBuilder sql = new StringBuilder();
        private static VLogger _log = VLogger.GetVLogger(typeof(ProductionReverse).FullName);

        private int VAM_Production_ID = 0;
        private DataSet dsProductionPlan = null;
        private DataSet dsProductionLine = null;
        private DataRow[] drProductionLine = null;

        private X_VAM_ProductionPlan fromProdPlan = null;
        private X_VAM_ProductionPlan toProdPlan = null;
        private X_VAM_ProductionLine fromProdline = null;
        private X_VAM_ProductionLine toProdline = null;

        private String result = string.Empty;


        protected override void Prepare()
        {
            // record id - to be reversed
            VAM_Production_ID = GetRecord_ID();
        }

        /// <summary>
        /// Is used to do a reverse entry of "Production Record" into the system
        /// </summary>
        /// <returns>message successfuly created or not</returns>
        protected override string DoIt()
        {
            if (VAM_Production_ID > 0)
            {
                //Copy Production Header
                X_VAM_Production production = new X_VAM_Production(GetCtx(), VAM_Production_ID, Get_Trx());

                //check production is Reversed or not, if Reversed then not to do anything
                if (production.IsReversed())
                {
                    return Msg.GetMsg(GetCtx(), "AlreadyReversed");
                }

                //Get data from Production Plan
                dsProductionPlan = DB.ExecuteDataset(@"SELECT VAF_CLIENT_ID , VAF_ORG_ID , DESCRIPTION , LINE , VAM_Locator_ID , 
                                       VAM_Product_ID , VAM_ProductionPlan_ID ,  VAM_Production_ID  ,  PROCESSED  , PRODUCTIONQTY  VAM_Warehouse_ID FROM VAM_ProductionPlan 
                                       WHERE IsActive = 'Y' AND VAM_Production_ID = " + VAM_Production_ID, null, Get_Trx());

                //get data from production Line
                dsProductionLine = DB.ExecuteDataset(@"SELECT VAF_CLIENT_ID , VAF_ORG_ID , DESCRIPTION , LINE , VAM_PFeature_SetInstance_ID , VAM_Locator_ID , 
                                       VAM_Product_ID ,  VAM_ProductionLine_ID, VAM_ProductionPlan_ID , VAM_Production_ID  , PROCESSED  , MOVEMENTQTY , 
                                       VAB_UOM_ID , PLANNEDQTY , VAM_Warehouse_ID FROM VAM_ProductionLine 
                                       WHERE IsActive = 'Y' AND VAM_Production_ID = " + VAM_Production_ID, null, Get_Trx());


                // Create New record of Production Header with Reverse Entry
                X_VAM_Production productionTo = new X_VAM_Production(production.GetCtx(), 0, production.Get_Trx());
                try
                {
                    productionTo.Set_TrxName(production.Get_Trx());
                    PO.CopyValues(production, productionTo, production.GetVAF_Client_ID(), production.GetVAF_Org_ID());
                    productionTo.SetName("{->" + productionTo.GetName() + ")");
                    if (production.Get_ColumnIndex("DocumentNo") > 0)
                    {
                        productionTo.Set_Value("DocumentNo", ("{->" + production.Get_Value("DocumentNo") + ")"));
                    }
                    productionTo.SetMovementDate(production.GetMovementDate()); //SI_0662 : not to create reverse record in current date, it should be created with the same date.
                    productionTo.SetProcessed(false);
                    if (!productionTo.Save(production.Get_Trx()))
                    {
                        production.Get_Trx().Rollback();
                        ValueNamePair pp = VLogger.RetrieveError();
                        _log.Log(Level.SEVERE, "Could Not create Production reverse entry. ERRor Value : " + pp.GetValue() + "ERROR NAME : " + pp.GetName());
                        throw new Exception("Could not create Production reverse entry");
                    }
                    else
                    {
                        #region create new record of Production Plan
                        if (dsProductionPlan != null && dsProductionPlan.Tables.Count > 0 && dsProductionPlan.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsProductionPlan.Tables[0].Rows.Count; i++)
                            {
                                //Original Line
                                fromProdPlan = new X_VAM_ProductionPlan(GetCtx(), Util.GetValueOfInt(dsProductionPlan.Tables[0].Rows[i]["VAM_ProductionPlan_ID"]), Get_Trx());

                                // Create New record of Production Plan with Reverse Entry
                                toProdPlan = new X_VAM_ProductionPlan(production.GetCtx(), 0, production.Get_Trx());
                                try
                                {
                                    toProdPlan.Set_TrxName(production.Get_Trx());
                                    PO.CopyValues(fromProdPlan, toProdPlan, fromProdPlan.GetVAF_Client_ID(), fromProdPlan.GetVAF_Org_ID());
                                    toProdPlan.SetProductionQty(Decimal.Negate(toProdPlan.GetProductionQty()));
                                    toProdPlan.SetVAM_Production_ID(productionTo.GetVAM_Production_ID());
                                    toProdPlan.SetProcessed(false);
                                    if (!toProdPlan.Save(production.Get_Trx()))
                                    {
                                        production.Get_Trx().Rollback();
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        _log.Log(Level.SEVERE, "Could Not create Production Plan reverse entry. ERRor Value : " + pp.GetValue() + "ERROR NAME : " + pp.GetName());
                                        throw new Exception("Could not create Production Plan reverse entry");
                                    }
                                    else
                                    {
                                        #region check record exist on production line
                                        if (dsProductionLine != null && dsProductionLine.Tables.Count > 0 && dsProductionLine.Tables[0].Rows.Count > 0)
                                        {
                                            //check record exist on production line against production plan
                                            drProductionLine = dsProductionLine.Tables[0].Select("VAM_ProductionPlan_ID  = " + fromProdPlan.GetVAM_ProductionPlan_ID());
                                            if (drProductionLine.Length > 0)
                                            {
                                                for (int j = 0; j < drProductionLine.Length; j++)
                                                {
                                                    //Original Line
                                                    fromProdline = new X_VAM_ProductionLine(GetCtx(), Util.GetValueOfInt(drProductionLine[j]["VAM_ProductionLine_ID"]), Get_Trx());

                                                    // Create New record of Production line with Reverse Entry
                                                    toProdline = new X_VAM_ProductionLine(production.GetCtx(), 0, production.Get_Trx());
                                                    try
                                                    {
                                                        toProdline.Set_TrxName(production.Get_Trx());
                                                        PO.CopyValues(fromProdline, toProdline, fromProdPlan.GetVAF_Client_ID(), fromProdPlan.GetVAF_Org_ID());
                                                        toProdline.SetMovementQty(Decimal.Negate(toProdline.GetMovementQty()));
                                                        toProdline.SetPlannedQty(Decimal.Negate(toProdline.GetPlannedQty()));
                                                        toProdline.SetVAM_Production_ID(productionTo.GetVAM_Production_ID());
                                                        toProdline.SetVAM_ProductionPlan_ID(toProdPlan.GetVAM_ProductionPlan_ID());
                                                        toProdline.SetVAM_ProductContainer_ID(fromProdline.GetVAM_ProductContainer_ID()); // bcz not a copy record
                                                        toProdline.SetReversalDoc_ID(fromProdline.GetVAM_ProductionLine_ID()); //maintain refernce of Orignal record on reversed record
                                                        toProdline.SetProcessed(false);
                                                        if (!CheckQtyAvailablity(GetCtx(), toProdline.GetVAM_Warehouse_ID(), toProdline.GetVAM_Locator_ID(), toProdline.GetVAM_ProductContainer_ID(), toProdline.GetVAM_Product_ID(), toProdline.GetVAM_PFeature_SetInstance_ID(), toProdline.GetMovementQty(), Get_Trx()))
                                                        {
                                                            production.Get_Trx().Rollback();
                                                            ValueNamePair pp = VLogger.RetrieveError();
                                                            if (!string.IsNullOrEmpty(pp.GetName()))
                                                                throw new Exception("Could not create Production line reverse entry, " + pp.GetName());
                                                            else
                                                                throw new Exception("Could not create Production line reverse entry");
                                                        }
                                                        if (!toProdline.Save(production.Get_Trx()))
                                                        {
                                                            production.Get_Trx().Rollback();
                                                            ValueNamePair pp = VLogger.RetrieveError();
                                                            _log.Log(Level.SEVERE, "Could Not create Production Line reverse entry. ERRor Value : " + pp.GetValue() + "ERROR NAME : " + pp.GetName());
                                                            throw new Exception("Could not create Production line reverse entry");
                                                        }
                                                        else
                                                        {
                                                            // Create New record of Production line Policy (Material Policy) with Reverse Entry
                                                            sql.Clear();
                                                            sql.Append(@"INSERT INTO VAM_ProductionLineMP 
                                                                  (  VAF_CLIENT_ID, VAF_ORG_ID , CREATED , CREATEDBY , ISACTIVE , UPDATED , UPDATEDBY ,
                                                                    VAM_ProductionLine_ID , VAM_PFeature_SetInstance_ID , MMPOLICYDATE , VAM_ProductContainer_ID, MOVEMENTQTY )
                                                                  (SELECT VAF_CLIENT_ID, VAF_ORG_ID , sysdate , CREATEDBY , ISACTIVE , sysdate , UPDATEDBY ,
                                                                      " + toProdline.GetVAM_ProductionLine_ID() + @" , VAM_PFeature_SetInstance_ID , MMPOLICYDATE , VAM_ProductContainer_ID,  -1 * MOVEMENTQTY
                                                                    FROM VAM_ProductionLineMP  WHERE VAM_ProductionLine_ID = " + fromProdline.GetVAM_ProductionLine_ID() + @" ) ");
                                                            int no = DB.ExecuteQuery(sql.ToString(), null, Get_Trx());
                                                            _log.Info("No of records saved on Meterial Policy against Production line ID : " + toProdline.GetVAM_ProductionLine_ID() + " are : " + no);

                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        _log.Info("Error Occured during Production Reverse " + ex.ToString());
                                                        if (dsProductionLine != null)
                                                            dsProductionLine.Dispose();
                                                        if (dsProductionPlan != null)
                                                            dsProductionPlan.Dispose();
                                                        return Msg.GetMsg(GetCtx(), "DocumentNotReversed" + result);
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _log.Info("Error Occured during Production Reverse " + ex.ToString());
                                    if (dsProductionLine != null)
                                        dsProductionLine.Dispose();
                                    if (dsProductionPlan != null)
                                        dsProductionPlan.Dispose();
                                    return Msg.GetMsg(GetCtx(), "DocumentNotReversed" + result);
                                }
                            }
                        }
                        #endregion

                        result = productionTo.GetName();
                    }

                    // To check weather future date records are available in Transaction window
                    // this check implement after "SetCompletedDocumentNo" function, because this function overwrit movement date
                    string _processMsg = MTransaction.CheckFutureDateRecord(productionTo.GetMovementDate(),
                                           productionTo.Get_TableName(), productionTo.GetVAM_Production_ID(), production.Get_Trx());
                    if (!string.IsNullOrEmpty(_processMsg))
                    {
                        production.Get_Trx().Rollback();
                        _log.Log(Level.SEVERE, "Could Not create Production reverse entry. Future Date record Exists - " + _processMsg);
                        throw new Exception("Could not create Production reverse entry");
                    }

                    //set Reversed as True
                    productionTo.SetIsReversed(true);
                    productionTo.SetIsCreated(true);
                    if (!productionTo.Save(production.Get_Trx()))
                    {
                        production.Get_Trx().Rollback();
                        ValueNamePair pp = VLogger.RetrieveError();
                        _log.Log(Level.SEVERE, "Could Not create Production reverse entry. ERRor Value : " + pp.GetValue() + "ERROR NAME : " + pp.GetName());
                        throw new Exception("Could not create Production reverse entry");
                    }

                    //Set reversed as true, Reverse Refernce on Orignal Document
                    production.SetIsReversed(true);
                    production.SetM_Ref_Production(productionTo.GetVAM_Production_ID());
                    if (!production.Save(production.Get_Trx()))
                    {
                        production.Get_Trx().Rollback();
                        ValueNamePair pp = VLogger.RetrieveError();
                        _log.Log(Level.SEVERE, "Could Not create Production reverse entry. ERRor Value : " + pp.GetValue() + "ERROR NAME : " + pp.GetName());
                        throw new Exception("Could not create Production reverse entry");
                    }
                }
                catch (Exception ex)
                {
                    _log.Info("Error Occured during Production Reverse " + ex.ToString());
                    if (dsProductionLine != null)
                        dsProductionLine.Dispose();
                    if (dsProductionPlan != null)
                        dsProductionPlan.Dispose();
                    return Msg.GetMsg(GetCtx(), "DocumentNotReversed" + result);
                }
            }
            return Msg.GetMsg(GetCtx(), "DocumentReversedSuccessfully" + result);
        }

        /// <summary>
        /// Check Qty Availablity in warehouse when Disallow Negative Inventory = true
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_Warehouse_ID">warehouse refernce - for checking disallow negative inventory or not</param>
        /// <param name="VAM_Locator_ID">locator ref - in which locator we have to check stock</param>
        /// <param name="VAM_Product_ID">product ref -- to which product, need to check stock</param>
        /// <param name="VAM_PFeature_SetInstance_ID">AttributeSetInstance -- to which ASI, need to check stock</param>
        /// <param name="MovementQty">qty to be impacted</param>
        /// <param name="trxName">system transaction</param>
        /// <returns>TRUE/False</returns>
        private bool CheckQtyAvailablity(Ctx ctx, int VAM_Warehouse_ID, int VAM_Locator_ID, int VAM_ProductContainer_ID, int VAM_Product_ID, int VAM_PFeature_SetInstance_ID, Decimal? MovementQty, Trx trxName)
        {
            MWarehouse wh = MWarehouse.Get(ctx, VAM_Warehouse_ID);
            MProduct product = null;
            if (wh.IsDisallowNegativeInv() && VAM_Product_ID > 0)
            {
                product = MProduct.Get(ctx, VAM_Product_ID);
                string qry = "SELECT NVL(SUM(NVL(QtyOnHand,0)),0) AS QtyOnHand FROM VAM_Storage where VAM_Locator_id=" + VAM_Locator_ID + @" and VAM_Product_id=" + VAM_Product_ID;
                qry += " AND NVL(VAM_PFeature_SetInstance_ID, 0) =" + VAM_PFeature_SetInstance_ID;
                if (VAM_ProductContainer_ID > 0)
                {
                    qry += " AND VAM_ProductContainer_ID = " + VAM_ProductContainer_ID;
                }
                Decimal? OnHandQty = Convert.ToDecimal(DB.ExecuteScalar(qry, null, trxName));
                if (OnHandQty + MovementQty < 0)
                {
                    log.SaveError("Info", product.GetName() + ", " + Msg.GetMsg(GetCtx(), "VIS_InsufficientQty") + OnHandQty);
                    return false;
                }
            }
            return true;
        }
    }
}
