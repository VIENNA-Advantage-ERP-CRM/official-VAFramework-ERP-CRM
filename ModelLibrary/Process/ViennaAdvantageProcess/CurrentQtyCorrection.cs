/********************************************************
    * Project Name   : Payment Method (ED008)
    * Class Name     : CurrentQtyCorrection
    * Purpose        : correct Current Quantity
    * Class Used     : ProcessEngine.SvrProcess
    * Chronological    Development
    * Amit Bansal     05-March-2015
******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.ProcessEngine;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VAdvantage.Logging;
using VAdvantage.Model;

namespace ViennaAdvantageServer.Process
{
    public class CurrentQtyCorrection : SvrProcess
    {
        private string sql = "";
        private DataSet dsTransaction = null;
        private string productId = "";
        //private string[] productCollection;
        private string orgId = "";
        //private string[] OrgCollection;
        private int _VAM_Product_ID = 0;
        private int _VAM_Locator_ID = 0;
        private int _VAM_PFeature_SetInstance_ID = 0;
        private decimal _currentQty = 0;
        VAdvantage.Model.MTransaction transaction = null;
        //ViennaAdvantage.Model.MVAMInventoryLine inventoryLine = null;
        //ViennaAdvantage.Model.MStorage storage = null;
        //ViennaAdvantage.Model.MVAMInventory inventory = null;
        MVAMInventoryLine inventoryLine = null;
        MStorage storage = null;
        MVAMInventory inventory = null;


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
                else if (name.Equals("VAM_Product_ID"))
                {
                    productId = (String)para[i].GetParameter();
                    //productCollection = productId.Split(',');
                }
                //else if (name.Equals("VAF_Org_ID"))
                //{
                //    orgId = (String)para[i].GetParameter();
                //    //OrgCollection = orgId.Split(',');
                //}
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }
        protected override string DoIt()
        {
            sql = @"SELECT   VAM_Product_ID ,   VAM_Locator_ID ,   VAM_PFeature_SetInstance_ID ,  VAM_Inv_Trx_ID , VAM_InventoryLine_ID , 
                           CurrentQty ,   MovementQty ,  MovementType , movementdate , TO_Char(Created, 'DD-MON-YY HH24:MI:SS')
                    FROM VAM_Inv_Trx WHERE IsActive= 'Y' ";
            //if (orgId != null && !string.IsNullOrEmpty(orgId))
            //{
            //    sql += " AND VAF_Org_ID  IN ( " + orgId + " )";
            //}
            if (productId != null && !string.IsNullOrEmpty(productId))
            {
                sql += " AND VAM_Product_ID IN ( " + productId + " )";
            }
            sql += " ORDER BY   VAM_Product_ID  ,  VAM_Locator_ID ,  VAM_PFeature_SetInstance_ID , movementdate , VAM_Inv_Trx_ID ASC ";
            dsTransaction = new DataSet();
            try
            {
                dsTransaction = DB.ExecuteDataset(sql, null, Get_Trx());
                if (dsTransaction != null)
                {
                    if (dsTransaction.Tables.Count > 0)
                    {
                        if (dsTransaction.Tables[0].Rows.Count > 0)
                        {
                            int i = 0;
                            for (i = 0; i < dsTransaction.Tables[0].Rows.Count; i++)
                            {
                                if (_VAM_Product_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]) &&
                                    _VAM_Locator_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]) &&
                                    _VAM_PFeature_SetInstance_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]) &&
                                    Util.GetValueOfString(dsTransaction.Tables[0].Rows[i]["MovementType"]) == "I+" &&
                                    Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_InventoryLine_ID"]) > 0)
                                {
                                    //update Quantity Book at inventory line 
                                    inventoryLine = new MVAMInventoryLine(GetCtx(), Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_InventoryLine_ID"]), Get_Trx());
                                    inventory = new MVAMInventory(GetCtx(), Util.GetValueOfInt(inventoryLine.GetVAM_Inventory_ID()), null);
                                    if (!inventory.IsInternalUse())
                                    {
                                        inventoryLine.SetQtyBook(_currentQty);
                                        inventoryLine.SetOpeningStock(_currentQty);
                                        inventoryLine.SetDifferenceQty(Decimal.Subtract(_currentQty, Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"])));
                                        if (!inventoryLine.Save())
                                        {
                                            log.Info("Quantity Book Not Updated at Inventory Line Tab <===> " + Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_InventoryLine_ID"]));
                                            Rollback();
                                            continue;
                                        }
                                        else
                                        {
                                            Commit();
                                        }

                                        // update movement Qty at Transaction for the same record
                                        transaction = new VAdvantage.Model.MTransaction(GetCtx(), Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Inv_Trx_ID"]), Get_Trx());
                                       // transaction.SetMovementQty(Decimal.Subtract(inventoryLine.GetQtyCount(), _currentQty));
                                        transaction.SetMovementQty(Decimal.Negate(Decimal.Subtract(_currentQty, Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]))));
                                        if (!transaction.Save())
                                        {
                                            log.Info("Current Quantity Not Updated at Transaction Tab <===> " + Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Inv_Trx_ID"]));
                                        }
                                        else
                                        {
                                            Commit();
                                            _currentQty = Util.GetValueOfDecimal(transaction.GetCurrentQty());
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        transaction = new VAdvantage.Model.MTransaction(GetCtx(), Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Inv_Trx_ID"]), Get_Trx());
                                        transaction.SetCurrentQty(Decimal.Add(_currentQty, Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["MovementQty"])));
                                        if (!transaction.Save())
                                        {
                                            log.Info("Current Quantity Not Updated at Transaction Tab <===> " + Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Inv_Trx_ID"]));
                                        }
                                        else
                                        {
                                            Commit();
                                            _currentQty = Decimal.Add(_currentQty, Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["MovementQty"]));
                                            continue;
                                        }
                                    }
                                }
                                else if (Util.GetValueOfString(dsTransaction.Tables[0].Rows[i]["MovementType"]) == "I+" &&
                                    Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_InventoryLine_ID"]) > 0)
                                {
                                    if (_VAM_Product_ID > 0)
                                    {
                                        UpdateStorage(_VAM_Product_ID, _VAM_Locator_ID, _VAM_PFeature_SetInstance_ID, _currentQty);
                                    }
                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
                                    _VAM_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]);
                                    _VAM_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]);
                                    _VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]);
                                    continue;
                                }

                                if (_VAM_Product_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]) &&
                                          _VAM_Locator_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]) &&
                                          _VAM_PFeature_SetInstance_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]))
                                {
                                    transaction = new VAdvantage.Model.MTransaction(GetCtx(), Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Inv_Trx_ID"]), Get_Trx());
                                    transaction.SetCurrentQty(Decimal.Add(_currentQty, Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["MovementQty"])));
                                    if (!transaction.Save())
                                    {
                                        log.Info("Current Quantity Not Updated at Transaction Tab <===> " + Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Inv_Trx_ID"]));
                                    }
                                    else
                                    {
                                        Commit();
                                        log.Info("Current Quantity  Updated at Transaction Tab <===> " + Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Inv_Trx_ID"]));
                                        _currentQty = Decimal.Add(_currentQty, Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["MovementQty"]));
                                    }
                                }
                                //when Attribute not matched
                                else if (_VAM_Product_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]) &&
                                          _VAM_Locator_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]) &&
                                          _VAM_PFeature_SetInstance_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]))
                                {
                                    if (_VAM_Product_ID > 0)
                                    {
                                        UpdateStorage(_VAM_Product_ID, _VAM_Locator_ID, _VAM_PFeature_SetInstance_ID, _currentQty);
                                    }
                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
                                    _VAM_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]);
                                    _VAM_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]);
                                    _VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]);
                                    continue;
                                }
                                // when Locator not Matched
                                else if (_VAM_Product_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]) &&
                                         _VAM_Locator_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]) &&
                                         _VAM_PFeature_SetInstance_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]))
                                {
                                    if (_VAM_Product_ID > 0)
                                    {
                                        UpdateStorage(_VAM_Product_ID, _VAM_Locator_ID, _VAM_PFeature_SetInstance_ID, _currentQty);
                                    }
                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
                                    _VAM_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]);
                                    _VAM_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]);
                                    _VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]);
                                    continue;
                                }
                                // when Product not Matched (Changed by Amit on 5-11-2015)
                                else if (_VAM_Product_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]) &&
                                         _VAM_Locator_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]) &&
                                         _VAM_PFeature_SetInstance_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]))
                                {
                                    if (_VAM_Product_ID > 0)
                                    {
                                        UpdateStorage(_VAM_Product_ID, _VAM_Locator_ID, _VAM_PFeature_SetInstance_ID, _currentQty);
                                    }
                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
                                    _VAM_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]);
                                    _VAM_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]);
                                    _VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]);
                                    continue;
                                }
                                //when Product , Locator n Attribute both not matched (means First Record)
                                else if (_VAM_Product_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]) &&
                                        _VAM_Locator_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]) &&
                                        _VAM_PFeature_SetInstance_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]))
                                {
                                    if (_VAM_Product_ID > 0)
                                    {
                                        UpdateStorage(_VAM_Product_ID, _VAM_Locator_ID, _VAM_PFeature_SetInstance_ID, _currentQty);
                                    }
                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
                                    _VAM_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]);
                                    _VAM_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]);
                                    _VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]);
                                    continue;
                                }
                                //when Locator n Attribute both not matched
                                else if (_VAM_Product_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]) &&
                                   _VAM_Locator_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]) &&
                                   _VAM_PFeature_SetInstance_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]))
                                {
                                    if (_VAM_Product_ID > 0)
                                    {
                                        UpdateStorage(_VAM_Product_ID, _VAM_Locator_ID, _VAM_PFeature_SetInstance_ID, _currentQty);
                                    }
                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
                                    _VAM_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]);
                                    _VAM_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]);
                                    _VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]);
                                    continue;
                                }
                                // when product and Locator not Matched
                                else if (_VAM_Product_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]) &&
                                        _VAM_Locator_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]) &&
                                        _VAM_PFeature_SetInstance_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]))
                                {
                                    if (_VAM_Product_ID > 0)
                                    {
                                        UpdateStorage(_VAM_Product_ID, _VAM_Locator_ID, _VAM_PFeature_SetInstance_ID, _currentQty);
                                    }
                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
                                    _VAM_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]);
                                    _VAM_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]);
                                    _VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]);
                                    continue;
                                }
                                // when product and Attribute not Matched
                                else if (_VAM_Product_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]) &&
                                        _VAM_Locator_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]) &&
                                        _VAM_PFeature_SetInstance_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]))
                                {
                                    if (_VAM_Product_ID > 0)
                                    {
                                        UpdateStorage(_VAM_Product_ID, _VAM_Locator_ID, _VAM_PFeature_SetInstance_ID, _currentQty);
                                    }
                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
                                    _VAM_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]);
                                    _VAM_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]);
                                    _VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]);
                                    continue;
                                }
                            }
                        }
                    }
                }
                dsTransaction.Dispose();
            }
            catch
            {
                if (dsTransaction != null)
                {
                    dsTransaction.Dispose();
                }
                return Msg.GetMsg(GetCtx(), "NotCompleted");
            }
            finally
            {
                if (dsTransaction != null)
                {
                    dsTransaction.Dispose();
                }
            }
            return Msg.GetMsg(GetCtx(), "SucessfullyCompleted");
            #region
            //            sql = @"SELECT   VAM_Product_ID ,   VAM_Locator_ID ,   VAM_PFeature_SetInstance_ID ,  VAM_Inv_Trx_ID , 
            //                           CurrentQty ,   MovementQty ,  MovementType , TO_Char(Created, 'DD-MON-YY HH24:MI:SS')
            //                    FROM VAM_Inv_Trx WHERE IsActive= 'Y' ";
            //            //if (orgId != null && !string.IsNullOrEmpty(orgId))
            //            //{
            //            //    sql += " AND VAF_Org_ID  IN ( " + orgId + " )";
            //            //}
            //            if (productId != null && !string.IsNullOrEmpty(productId))
            //            {
            //                sql += " AND VAM_Product_ID IN ( " + productId + " )";
            //            }
            //            sql += " ORDER BY   VAM_Product_ID  ,  VAM_Locator_ID ,  VAM_PFeature_SetInstance_ID , Created ASC ";
            //            dsTransaction = new DataSet();
            //            try
            //            {
            //                dsTransaction = DB.ExecuteDataset(sql, null, null);
            //                if (dsTransaction != null)
            //                {
            //                    if (dsTransaction.Tables.Count > 0)
            //                    {
            //                        if (dsTransaction.Tables[0].Rows.Count > 0)
            //                        {
            //                            int i = 0;
            //                            for (i = 0; i < dsTransaction.Tables[0].Rows.Count; i++)
            //                            {
            //                                if (Util.GetValueOfString(dsTransaction.Tables[0].Rows[i]["MovementType"]) == "I+")
            //                                {
            //                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
            //                                    _VAM_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]);
            //                                    _VAM_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]);
            //                                    _VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]);
            //                                    continue;
            //                                }
            //                                else if (_VAM_Product_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]) &&
            //                                          _VAM_Locator_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]) &&
            //                                          _VAM_PFeature_SetInstance_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]))
            //                                {
            //                                    transaction = new VAdvantage.Model.MTransaction(GetCtx(), Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Inv_Trx_ID"]), null);
            //                                    transaction.SetCurrentQty(Decimal.Add(_currentQty, Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["MovementQty"])));
            //                                    if (!transaction.Save())
            //                                    {
            //                                        log.Info("Current Quantity Not Updated at Transaction Tab <===> " + Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Inv_Trx_ID"]));
            //                                    }
            //                                    else
            //                                    {
            //                                        Commit();
            //                                        _currentQty = Decimal.Add(_currentQty, Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["MovementQty"]));
            //                                    }
            //                                }
            //                                //when Attribute not matched
            //                                else if (_VAM_Product_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]) &&
            //                                          _VAM_Locator_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]) &&
            //                                          _VAM_PFeature_SetInstance_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]))
            //                                {
            //                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
            //                                    _VAM_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]);
            //                                    _VAM_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]);
            //                                    _VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]);
            //                                    continue;
            //                                }
            //                                // when Locator not Matched
            //                                else if (_VAM_Product_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]) &&
            //                                         _VAM_Locator_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]) &&
            //                                         _VAM_PFeature_SetInstance_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]))
            //                                {
            //                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
            //                                    _VAM_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]);
            //                                    _VAM_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]);
            //                                    _VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]);
            //                                    continue;
            //                                }
            //                                //when Product , Locator n Attribute both not matched (means First Record)
            //                                else if (_VAM_Product_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]) &&
            //                                        _VAM_Locator_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]) &&
            //                                        _VAM_PFeature_SetInstance_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]))
            //                                {
            //                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
            //                                    _VAM_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]);
            //                                    _VAM_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]);
            //                                    _VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]);
            //                                    continue;
            //                                }
            //                                //when Locator n Attribute both not matched
            //                                else if (_VAM_Product_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]) &&
            //                                   _VAM_Locator_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]) &&
            //                                   _VAM_PFeature_SetInstance_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]))
            //                                {
            //                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
            //                                    _VAM_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]);
            //                                    _VAM_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]);
            //                                    _VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]);
            //                                    continue;
            //                                }
            //                                // when product and Locator not Matched
            //                                else if (_VAM_Product_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]) &&
            //                                        _VAM_Locator_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]) &&
            //                                        _VAM_PFeature_SetInstance_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]))
            //                                {
            //                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
            //                                    _VAM_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]);
            //                                    _VAM_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]);
            //                                    _VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]);
            //                                    continue;
            //                                }
            //                                // when product and Attribute not Matched
            //                                else if (_VAM_Product_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]) &&
            //                                        _VAM_Locator_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]) &&
            //                                        _VAM_PFeature_SetInstance_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]))
            //                                {
            //                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
            //                                    _VAM_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]);
            //                                    _VAM_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Locator_ID"]);
            //                                    _VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]);
            //                                    continue;
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //                dsTransaction.Dispose();
            //            }
            //            catch
            //            {
            //                if (dsTransaction != null)
            //                {
            //                    dsTransaction.Dispose();
            //                }
            //                return Msg.GetMsg(GetCtx(), "NotCompleted");
            //            }
            //            finally
            //            {
            //                if (dsTransaction != null)
            //                {
            //                    dsTransaction.Dispose();
            //                }
            //            }
            //            return Msg.GetMsg(GetCtx(), "SucessfullyCompleted");
            #endregion
        }

        private void UpdateStorage(int VAM_Product_ID, int VAM_Locator_ID, int VAM_PFeatue_Instance_ID, decimal QtyOnHand)
        {
            storage = MStorage.Get(GetCtx(), VAM_Locator_ID,
                               VAM_Product_ID, VAM_PFeatue_Instance_ID, Get_Trx());
            if (storage == null)
                storage = MStorage.GetCreate(GetCtx(), VAM_Locator_ID,
                   VAM_Product_ID, 0, Get_Trx());
            storage.SetQtyOnHand(QtyOnHand);
            if (!storage.Save())
            {
                log.Info("Onhand Quantity Not Updated at Storage");
                Rollback();
            }
            else
            {
                log.Info("Onhand Quantity  Updated at Storage");
                Commit();
            }
        }
    }
}
