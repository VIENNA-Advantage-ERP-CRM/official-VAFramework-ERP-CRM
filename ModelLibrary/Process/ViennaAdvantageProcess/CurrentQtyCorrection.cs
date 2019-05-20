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
        private int _M_Product_ID = 0;
        private int _M_Locator_ID = 0;
        private int _M_AttributeSetInstance_ID = 0;
        private decimal _currentQty = 0;
        VAdvantage.Model.MTransaction transaction = null;
        //ViennaAdvantage.Model.MInventoryLine inventoryLine = null;
        //ViennaAdvantage.Model.MStorage storage = null;
        //ViennaAdvantage.Model.MInventory inventory = null;
        MInventoryLine inventoryLine = null;
        MStorage storage = null;
        MInventory inventory = null;


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
                else if (name.Equals("M_Product_ID"))
                {
                    productId = (String)para[i].GetParameter();
                    //productCollection = productId.Split(',');
                }
                //else if (name.Equals("AD_Org_ID"))
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
            sql = @"SELECT   M_Product_ID ,   M_Locator_ID ,   M_AttributeSetInstance_ID ,  M_Transaction_ID , M_InventoryLine_ID , 
                           CurrentQty ,   MovementQty ,  MovementType , movementdate , TO_Char(Created, 'DD-MON-YY HH24:MI:SS')
                    FROM M_Transaction WHERE IsActive= 'Y' ";
            //if (orgId != null && !string.IsNullOrEmpty(orgId))
            //{
            //    sql += " AND AD_Org_ID  IN ( " + orgId + " )";
            //}
            if (productId != null && !string.IsNullOrEmpty(productId))
            {
                sql += " AND M_Product_ID IN ( " + productId + " )";
            }
            sql += " ORDER BY   M_Product_ID  ,  M_Locator_ID ,  M_AttributeSetInstance_ID , movementdate , M_Transaction_ID ASC ";
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
                                if (_M_Product_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]) &&
                                    _M_Locator_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]) &&
                                    _M_AttributeSetInstance_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]) &&
                                    Util.GetValueOfString(dsTransaction.Tables[0].Rows[i]["MovementType"]) == "I+" &&
                                    Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_InventoryLine_ID"]) > 0)
                                {
                                    //update Quantity Book at inventory line 
                                    inventoryLine = new MInventoryLine(GetCtx(), Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_InventoryLine_ID"]), Get_Trx());
                                    inventory = new MInventory(GetCtx(), Util.GetValueOfInt(inventoryLine.GetM_Inventory_ID()), null);
                                    if (!inventory.IsInternalUse())
                                    {
                                        inventoryLine.SetQtyBook(_currentQty);
                                        inventoryLine.SetOpeningStock(_currentQty);
                                        inventoryLine.SetDifferenceQty(Decimal.Subtract(_currentQty, Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"])));
                                        if (!inventoryLine.Save())
                                        {
                                            log.Info("Quantity Book Not Updated at Inventory Line Tab <===> " + Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_InventoryLine_ID"]));
                                            Rollback();
                                            continue;
                                        }
                                        else
                                        {
                                            Commit();
                                        }

                                        // update movement Qty at Transaction for the same record
                                        transaction = new VAdvantage.Model.MTransaction(GetCtx(), Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Transaction_ID"]), Get_Trx());
                                       // transaction.SetMovementQty(Decimal.Subtract(inventoryLine.GetQtyCount(), _currentQty));
                                        transaction.SetMovementQty(Decimal.Negate(Decimal.Subtract(_currentQty, Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]))));
                                        if (!transaction.Save())
                                        {
                                            log.Info("Current Quantity Not Updated at Transaction Tab <===> " + Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Transaction_ID"]));
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
                                        transaction = new VAdvantage.Model.MTransaction(GetCtx(), Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Transaction_ID"]), Get_Trx());
                                        transaction.SetCurrentQty(Decimal.Add(_currentQty, Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["MovementQty"])));
                                        if (!transaction.Save())
                                        {
                                            log.Info("Current Quantity Not Updated at Transaction Tab <===> " + Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Transaction_ID"]));
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
                                    Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_InventoryLine_ID"]) > 0)
                                {
                                    if (_M_Product_ID > 0)
                                    {
                                        UpdateStorage(_M_Product_ID, _M_Locator_ID, _M_AttributeSetInstance_ID, _currentQty);
                                    }
                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
                                    _M_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]);
                                    _M_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]);
                                    _M_AttributeSetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
                                    continue;
                                }

                                if (_M_Product_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]) &&
                                          _M_Locator_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]) &&
                                          _M_AttributeSetInstance_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]))
                                {
                                    transaction = new VAdvantage.Model.MTransaction(GetCtx(), Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Transaction_ID"]), Get_Trx());
                                    transaction.SetCurrentQty(Decimal.Add(_currentQty, Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["MovementQty"])));
                                    if (!transaction.Save())
                                    {
                                        log.Info("Current Quantity Not Updated at Transaction Tab <===> " + Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Transaction_ID"]));
                                    }
                                    else
                                    {
                                        Commit();
                                        log.Info("Current Quantity  Updated at Transaction Tab <===> " + Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Transaction_ID"]));
                                        _currentQty = Decimal.Add(_currentQty, Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["MovementQty"]));
                                    }
                                }
                                //when Attribute not matched
                                else if (_M_Product_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]) &&
                                          _M_Locator_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]) &&
                                          _M_AttributeSetInstance_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]))
                                {
                                    if (_M_Product_ID > 0)
                                    {
                                        UpdateStorage(_M_Product_ID, _M_Locator_ID, _M_AttributeSetInstance_ID, _currentQty);
                                    }
                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
                                    _M_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]);
                                    _M_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]);
                                    _M_AttributeSetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
                                    continue;
                                }
                                // when Locator not Matched
                                else if (_M_Product_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]) &&
                                         _M_Locator_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]) &&
                                         _M_AttributeSetInstance_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]))
                                {
                                    if (_M_Product_ID > 0)
                                    {
                                        UpdateStorage(_M_Product_ID, _M_Locator_ID, _M_AttributeSetInstance_ID, _currentQty);
                                    }
                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
                                    _M_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]);
                                    _M_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]);
                                    _M_AttributeSetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
                                    continue;
                                }
                                // when Product not Matched (Changed by Amit on 5-11-2015)
                                else if (_M_Product_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]) &&
                                         _M_Locator_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]) &&
                                         _M_AttributeSetInstance_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]))
                                {
                                    if (_M_Product_ID > 0)
                                    {
                                        UpdateStorage(_M_Product_ID, _M_Locator_ID, _M_AttributeSetInstance_ID, _currentQty);
                                    }
                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
                                    _M_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]);
                                    _M_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]);
                                    _M_AttributeSetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
                                    continue;
                                }
                                //when Product , Locator n Attribute both not matched (means First Record)
                                else if (_M_Product_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]) &&
                                        _M_Locator_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]) &&
                                        _M_AttributeSetInstance_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]))
                                {
                                    if (_M_Product_ID > 0)
                                    {
                                        UpdateStorage(_M_Product_ID, _M_Locator_ID, _M_AttributeSetInstance_ID, _currentQty);
                                    }
                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
                                    _M_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]);
                                    _M_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]);
                                    _M_AttributeSetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
                                    continue;
                                }
                                //when Locator n Attribute both not matched
                                else if (_M_Product_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]) &&
                                   _M_Locator_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]) &&
                                   _M_AttributeSetInstance_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]))
                                {
                                    if (_M_Product_ID > 0)
                                    {
                                        UpdateStorage(_M_Product_ID, _M_Locator_ID, _M_AttributeSetInstance_ID, _currentQty);
                                    }
                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
                                    _M_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]);
                                    _M_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]);
                                    _M_AttributeSetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
                                    continue;
                                }
                                // when product and Locator not Matched
                                else if (_M_Product_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]) &&
                                        _M_Locator_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]) &&
                                        _M_AttributeSetInstance_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]))
                                {
                                    if (_M_Product_ID > 0)
                                    {
                                        UpdateStorage(_M_Product_ID, _M_Locator_ID, _M_AttributeSetInstance_ID, _currentQty);
                                    }
                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
                                    _M_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]);
                                    _M_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]);
                                    _M_AttributeSetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
                                    continue;
                                }
                                // when product and Attribute not Matched
                                else if (_M_Product_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]) &&
                                        _M_Locator_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]) &&
                                        _M_AttributeSetInstance_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]))
                                {
                                    if (_M_Product_ID > 0)
                                    {
                                        UpdateStorage(_M_Product_ID, _M_Locator_ID, _M_AttributeSetInstance_ID, _currentQty);
                                    }
                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
                                    _M_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]);
                                    _M_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]);
                                    _M_AttributeSetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
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
            //            sql = @"SELECT   M_Product_ID ,   M_Locator_ID ,   M_AttributeSetInstance_ID ,  M_Transaction_ID , 
            //                           CurrentQty ,   MovementQty ,  MovementType , TO_Char(Created, 'DD-MON-YY HH24:MI:SS')
            //                    FROM M_Transaction WHERE IsActive= 'Y' ";
            //            //if (orgId != null && !string.IsNullOrEmpty(orgId))
            //            //{
            //            //    sql += " AND AD_Org_ID  IN ( " + orgId + " )";
            //            //}
            //            if (productId != null && !string.IsNullOrEmpty(productId))
            //            {
            //                sql += " AND M_Product_ID IN ( " + productId + " )";
            //            }
            //            sql += " ORDER BY   M_Product_ID  ,  M_Locator_ID ,  M_AttributeSetInstance_ID , Created ASC ";
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
            //                                    _M_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]);
            //                                    _M_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]);
            //                                    _M_AttributeSetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
            //                                    continue;
            //                                }
            //                                else if (_M_Product_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]) &&
            //                                          _M_Locator_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]) &&
            //                                          _M_AttributeSetInstance_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]))
            //                                {
            //                                    transaction = new VAdvantage.Model.MTransaction(GetCtx(), Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Transaction_ID"]), null);
            //                                    transaction.SetCurrentQty(Decimal.Add(_currentQty, Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["MovementQty"])));
            //                                    if (!transaction.Save())
            //                                    {
            //                                        log.Info("Current Quantity Not Updated at Transaction Tab <===> " + Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Transaction_ID"]));
            //                                    }
            //                                    else
            //                                    {
            //                                        Commit();
            //                                        _currentQty = Decimal.Add(_currentQty, Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["MovementQty"]));
            //                                    }
            //                                }
            //                                //when Attribute not matched
            //                                else if (_M_Product_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]) &&
            //                                          _M_Locator_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]) &&
            //                                          _M_AttributeSetInstance_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]))
            //                                {
            //                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
            //                                    _M_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]);
            //                                    _M_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]);
            //                                    _M_AttributeSetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
            //                                    continue;
            //                                }
            //                                // when Locator not Matched
            //                                else if (_M_Product_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]) &&
            //                                         _M_Locator_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]) &&
            //                                         _M_AttributeSetInstance_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]))
            //                                {
            //                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
            //                                    _M_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]);
            //                                    _M_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]);
            //                                    _M_AttributeSetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
            //                                    continue;
            //                                }
            //                                //when Product , Locator n Attribute both not matched (means First Record)
            //                                else if (_M_Product_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]) &&
            //                                        _M_Locator_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]) &&
            //                                        _M_AttributeSetInstance_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]))
            //                                {
            //                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
            //                                    _M_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]);
            //                                    _M_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]);
            //                                    _M_AttributeSetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
            //                                    continue;
            //                                }
            //                                //when Locator n Attribute both not matched
            //                                else if (_M_Product_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]) &&
            //                                   _M_Locator_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]) &&
            //                                   _M_AttributeSetInstance_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]))
            //                                {
            //                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
            //                                    _M_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]);
            //                                    _M_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]);
            //                                    _M_AttributeSetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
            //                                    continue;
            //                                }
            //                                // when product and Locator not Matched
            //                                else if (_M_Product_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]) &&
            //                                        _M_Locator_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]) &&
            //                                        _M_AttributeSetInstance_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]))
            //                                {
            //                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
            //                                    _M_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]);
            //                                    _M_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]);
            //                                    _M_AttributeSetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
            //                                    continue;
            //                                }
            //                                // when product and Attribute not Matched
            //                                else if (_M_Product_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]) &&
            //                                        _M_Locator_ID == Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]) &&
            //                                        _M_AttributeSetInstance_ID != Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]))
            //                                {
            //                                    _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
            //                                    _M_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]);
            //                                    _M_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]);
            //                                    _M_AttributeSetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
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

        private void UpdateStorage(int M_Product_ID, int M_Locator_ID, int M_AttributeInstance_ID, decimal QtyOnHand)
        {
            storage = MStorage.Get(GetCtx(), M_Locator_ID,
                               M_Product_ID, M_AttributeInstance_ID, Get_Trx());
            if (storage == null)
                storage = MStorage.GetCreate(GetCtx(), M_Locator_ID,
                   M_Product_ID, 0, Get_Trx());
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
