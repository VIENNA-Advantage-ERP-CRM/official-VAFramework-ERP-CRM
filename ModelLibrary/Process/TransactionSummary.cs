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

namespace VAdvantage.Process
{
    class TransactionSummary : SvrProcess
    {
        private StringBuilder sql = new StringBuilder();
        string Qry = "";
        private DataSet dsTransaction = null;
        private int productId = 0;
        private string orgId = "";
        private int _M_Product_ID = 0;
        private int _M_Locator_ID = 0;
        private int _M_AttributeSetInstance_ID = 0;
        private int _Ad_Org_ID = 0;
        private decimal _currentQty = 0;
        private decimal OpeningStock = 0, ClosingStock = 0;
        private decimal _moveQty = 0;
        private string _moveType = "";
        private DateTime? _moveDate = null;
        MTransactionSummary Trs = null;

        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("M_Product_ID"))
                {
                    productId = Util.GetValueOfInt(para[i].GetParameter());
                }

                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }

        protected override string DoIt()
        {
            Qry = "TRUNCATE TABLE M_TransactionSummary";
            int no = DB.ExecuteQuery(Qry);
            sql.Append(@" SELECT TR.M_PRODUCT_ID ,
                      TR.M_LOCATOR_ID ,
                      TR.M_ATTRIBUTESETINSTANCE_ID ,
                      TR.M_TRANSACTION_ID ,
                      TR.CURRENTQTY ,
                      TR.MOVEMENTQTY ,
                      TR.MOVEMENTTYPE ,
                      TR.MOVEMENTDATE ,
                      TR.AD_ORG_ID
                    FROM M_TRANSACTION TR
                    INNER JOIN M_PRODUCT PR
                    ON PR.M_PRODUCT_ID  =TR.M_PRODUCT_ID
                    WHERE TR.ISACTIVE   = 'Y'
                    AND PR.ISACTIVE     ='Y'");
            if (productId > 0)
            {
                sql.Append(@" AND PR.M_PRODUCT_ID  IN ( " + productId + " )");
            }
            sql.Append(@" ORDER BY TR.M_PRODUCT_ID ,
                      TR.M_LOCATOR_ID ,
                      TR.M_ATTRIBUTESETINSTANCE_ID ,
                      TR.MOVEMENTDATE ,
                      TR.M_TRANSACTION_ID ASC ");
            dsTransaction = new DataSet();
            try
            {
                dsTransaction = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                if (dsTransaction != null)
                {
                    if (dsTransaction.Tables.Count > 0)
                    {
                        if (dsTransaction.Tables[0].Rows.Count > 0)
                        {
                            int i = 0;
                            log.Info(" =====> Transaction Summary Entering Started at " + DateTime.Now.ToString() + " , Total Transactions To Correct = " + dsTransaction.Tables[0].Rows.Count + " <===== ");
                            for (i = 0; i < dsTransaction.Tables[0].Rows.Count; i++)
                            {
                                if (i % 10000 == 0)
                                {
                                    log.Info(" =====> " + i + " Transactions Updated till " + DateTime.Now.ToString() + "<===== ");
                                }
                                _M_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Product_ID"]);
                                _M_Locator_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_Locator_ID"]);
                                _M_AttributeSetInstance_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
                                _moveType = Util.GetValueOfString(dsTransaction.Tables[0].Rows[i]["MovementType"]);
                                _moveDate = Util.GetValueOfDateTime(dsTransaction.Tables[0].Rows[i]["MovementDate"]);
                                _moveQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["MovementQty"]);
                                _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
                                sql.Clear();
                                
                                sql.Append(@"Select M_TransactionSummary_ID From M_TransactionSummary Where M_Product_ID=" + _M_Product_ID + @" AND M_AttributeSetInstance_ID=" + _M_AttributeSetInstance_ID + @"
                                        AND M_Locator_ID=" + _M_Locator_ID + @" AND MovementDate=" + GlobalVariable.TO_DATE(_moveDate, true));
                                int M_TransactionSummary_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString()));
                                if (M_TransactionSummary_ID > 0)
                                {
                                    Trs = new MTransactionSummary(GetCtx(), M_TransactionSummary_ID, Get_TrxName());
                                    Trs.SetClosingStock(_currentQty);
                                }
                                else
                                {
                                    Qry = "SELECT Count(*) FROM M_TransactionSummary WHERE IsActive = 'Y' AND  M_Product_ID = " + _M_Product_ID +
                                     " AND M_Locator_ID = " + _M_Locator_ID + " AND M_AttributeSetInstance_ID = " + _M_AttributeSetInstance_ID +
                                     " AND movementdate < " + GlobalVariable.TO_DATE(_moveDate, true);
                                    int existOld = Util.GetValueOfInt(DB.ExecuteScalar(Qry));
                                    if (existOld > 0)
                                    {
                                        Qry = "SELECT ClosingStock FROM M_TransactionSummary WHERE IsActive = 'Y' AND  M_Product_ID = " + _M_Product_ID +
                                                        " AND M_Locator_ID = " + _M_Locator_ID + " AND M_AttributeSetInstance_ID = " + _M_AttributeSetInstance_ID +
                                                        " AND movementdate < " + GlobalVariable.TO_DATE(_moveDate, true) + " ORDER BY MovementDate DESC";
                                    }
                                    else
                                    {
                                        Qry = "SELECT NVL(GetStockofWarehouse(" + _M_Product_ID + "," + _M_Locator_ID + ",0," + _M_AttributeSetInstance_ID + ","
                                        + GlobalVariable.TO_DATE(Convert.ToDateTime(_moveDate).AddDays(-1), true) + "," + GetAD_Client_ID() + "," + GetAD_Org_ID() + "),0) AS Stock FROM DUAL";
                                    }
                                    OpeningStock = Util.GetValueOfDecimal(DB.ExecuteScalar(Qry));
                                    MLocator loc = new MLocator(GetCtx(), _M_Locator_ID, Get_TrxName());
                                    Trs = new MTransactionSummary(GetCtx(), loc.GetAD_Org_ID(), _M_Locator_ID, _M_Product_ID, _M_AttributeSetInstance_ID,
                                            OpeningStock, _currentQty, _moveDate, Get_TrxName());
                                }
                                if (_moveType == MTransaction.MOVEMENTTYPE_CustomerReturns)
                                {
                                    Trs.SetQtyCustReturn(Trs.GetQtyCustReturn() + _moveQty);
                                }
                                else if (_moveType == MTransaction.MOVEMENTTYPE_CustomerShipment)
                                {
                                    Trs.SetQtyCustShipment(Trs.GetQtyCustShipment() + _moveQty);
                                }
                                else if (_moveType == MTransaction.MOVEMENTTYPE_InventoryIn)
                                {
                                    Trs.SetQtyInventoryIn(Trs.GetQtyInventoryIn() + _moveQty);
                                }
                                else if (_moveType == MTransaction.MOVEMENTTYPE_InventoryOut)
                                {
                                    Trs.SetQtyInventoryOut(Trs.GetQtyInventoryOut() + _moveQty);
                                }
                                else if (_moveType == MTransaction.MOVEMENTTYPE_MovementFrom)
                                {
                                    Trs.SetQtyMoveOut(Trs.GetQtyMoveOut() + _moveQty);
                                }
                                else if (_moveType == MTransaction.MOVEMENTTYPE_MovementTo)
                                {
                                    Trs.SetQtyMoveTo(Trs.GetQtyMoveTo() + _moveQty);
                                }
                                else if (_moveType == MTransaction.MOVEMENTTYPE_Production_)
                                {
                                    Trs.SetQtyProductionOut(Trs.GetQtyProductionOut() + _moveQty);
                                }
                                else if (_moveType == MTransaction.MOVEMENTTYPE_ProductionPlus)
                                {
                                    Trs.SetQtyProductionIn(Trs.GetQtyProductionIn() + _moveQty);
                                }
                                else if (_moveType == MTransaction.MOVEMENTTYPE_VendorReceipts)
                                {
                                    Trs.SetQtyMaterialIn(Trs.GetQtyMaterialIn() + _moveQty);
                                }
                                else if (_moveType == MTransaction.MOVEMENTTYPE_VendorReturns)
                                {
                                    Trs.SetQtyMaterialOut(Trs.GetQtyMaterialOut() + _moveQty);
                                }
                                else if (_moveType == MTransaction.MOVEMENTTYPE_WorkOrderPlus)
                                {
                                    Trs.SetQtyWorkOrderIn(Trs.GetQtyWorkOrderIn() + _moveQty);
                                }
                                else if (_moveType == MTransaction.MOVEMENTTYPE_WorkOrder_)
                                {
                                    Trs.SetQtyWorkOrderOut(Trs.GetQtyWorkOrderOut() + _moveQty);
                                }
                                if (!Trs.Save())
                                {
                                    log.Info(Msg.GetMsg(GetCtx(), "TrxSummaryNotSaved"));
                                    Rollback();
                                }
                                else
                                {
                                    log.Info(Msg.GetMsg(GetCtx(), "TrxSummarySaved"));
                                    Commit();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
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
            log.Info(" =====> Transaction Correction End at " + DateTime.Now.ToString() + " <===== ");
            return Msg.GetMsg(GetCtx(), "SucessfullyCompleted");
        }
    }
}
