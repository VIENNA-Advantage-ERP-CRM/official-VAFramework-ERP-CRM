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
    class ProductStockSummary : SvrProcess
    {
        private StringBuilder sql = new StringBuilder();
        string Qry = "";
        private DataSet dsTransaction = null;
        private int productId = 0;
        private string orgId = "";
        private int _VAM_Product_ID = 0;
        private int _VAM_Locator_ID = 0;
        private int _VAM_PFeature_SetInstance_ID = 0;
        private int _vaf_org_ID = 0;
        private decimal _currentQty = 0;
        private decimal OpeningStock = 0, ClosingStock = 0;
        private decimal _moveQty = 0;
        private string _moveType = "";
        private int existOld = 0;
        private DateTime? _moveDate = null;
        MVAMProdStockSummary Trs = null;

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
                else if (name.Equals("VAM_Product_ID"))
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
            Qry = "TRUNCATE TABLE VAM_Prod_StockSummary";
            int no = DB.ExecuteQuery(Qry);
            sql.Append(@" SELECT TR.VAM_Product_ID ,                      
                      LC.VAF_ORG_ID ,
                      TR.VAM_Inv_Trx_ID ,
                      TR.CURRENTQTY ,
                      TR.MOVEMENTQTY ,
                      TR.MOVEMENTTYPE ,
                      TR.MOVEMENTDATE
                    FROM VAM_Inv_Trx TR
                    INNER JOIN VAM_Product PR
                    ON PR.VAM_Product_ID  =TR.VAM_Product_ID
                    INNER JOIN VAM_Locator LC
                    ON LC.VAM_Locator_ID  =TR.VAM_Locator_ID
                    WHERE TR.ISACTIVE   = 'Y'
                    AND PR.ISACTIVE     ='Y'");
            if (productId > 0)
            {
                sql.Append(@" AND PR.VAM_Product_ID  IN ( " + productId + " )");
            }
            sql.Append(@" ORDER BY TR.VAM_Product_ID ,
                      TR.MOVEMENTDATE ,
                      TR.VAM_Inv_Trx_ID ASC ");
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
                            log.Info(" =====> Product Stock Summary Entering Started at " + DateTime.Now.ToString() + " , Total Transactions To Correct = " + dsTransaction.Tables[0].Rows.Count + " <===== ");
                            for (i = 0; i < dsTransaction.Tables[0].Rows.Count; i++)
                            {
                                OpeningStock = 0;
                                ClosingStock = 0;
                                if (i % 10000 == 0)
                                {
                                    log.Info(" =====> " + i + " Transactions Updated till " + DateTime.Now.ToString() + "<===== ");
                                }
                                _VAM_Product_ID = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAM_Product_ID"]);
                                _vaf_org_ID  = Util.GetValueOfInt(dsTransaction.Tables[0].Rows[i]["VAF_Org_ID"]);                                
                                _moveDate = Util.GetValueOfDateTime(dsTransaction.Tables[0].Rows[i]["MovementDate"]);
                                _moveQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["MovementQty"]);
                                _currentQty = Util.GetValueOfDecimal(dsTransaction.Tables[0].Rows[i]["CurrentQty"]);
                                sql.Clear();

                                Qry= "SELECT VAM_Prod_StockSummary_ID FROM VAM_Prod_StockSummary WHERE IsActive = 'Y' AND VAM_Product_ID = " + _VAM_Product_ID +
                                        " AND VAF_Org_ID = " + _vaf_org_ID + " AND MovementFromDate = " + GlobalVariable.TO_DATE(_moveDate, true);
                                int VAM_Prod_StockSummary_ID = Util.GetValueOfInt(DB.ExecuteScalar(Qry.ToString(), null, Get_Trx()));
                                if (VAM_Prod_StockSummary_ID > 0)
                                {
                                    Trs = new MVAMProdStockSummary(GetCtx(), VAM_Prod_StockSummary_ID, Get_TrxName());
                                    Trs.SetQtyCloseStockOrg(Trs.GetQtyCloseStockOrg() + _moveQty);
                                }
                                else
                                {
                                    Qry = "SELECT Count(*) FROM VAM_Prod_StockSummary WHERE IsActive = 'Y' AND VAM_Product_ID = " + _VAM_Product_ID +
                                            " AND VAF_Org_ID = " + _vaf_org_ID + " AND MovementFromDate < " + GlobalVariable.TO_DATE(_moveDate, true);
                                    existOld = Util.GetValueOfInt(DB.ExecuteScalar(Qry, null, Get_Trx()));
                                    if (existOld > 0)
                                    {
                                        Qry = "SELECT QtyCloseStockOrg FROM VAM_Prod_StockSummary WHERE IsActive = 'Y' AND VAM_Product_ID = " + _VAM_Product_ID +
                                            " AND VAF_Org_ID = " + _vaf_org_ID + " AND MovementFromDate < " + GlobalVariable.TO_DATE(_moveDate, true) + " ORDER BY MovementFromDate DESC";
                                        OpeningStock = Util.GetValueOfDecimal(DB.ExecuteScalar(Qry, null, Get_Trx()));
                                    }
                                    Trs = new MVAMProdStockSummary(GetCtx(), _vaf_org_ID, _VAM_Product_ID,
                                            OpeningStock, OpeningStock + _moveQty, _moveDate, Get_TrxName());
                                    Trs.SetIsStockSummarized(true);
                                }                                
                                if (!Trs.Save())
                                {
                                    log.Info(Msg.GetMsg(GetCtx(), "StockSummaryNotSaved"));
                                    Rollback();
                                }
                                else
                                {
                                    if (existOld > 0)
                                    {
                                        Qry = "SELECT VAM_Prod_StockSummary_ID FROM VAM_Prod_StockSummary WHERE IsActive = 'Y' AND VAM_Product_ID = " + _VAM_Product_ID +
                                                    " AND VAF_Org_ID = " + _vaf_org_ID + " AND MovementFromDate < " + GlobalVariable.TO_DATE(_moveDate, true) + " ORDER BY MovementFromDate DESC";
                                        int oldSummary_ID = Util.GetValueOfInt(DB.ExecuteScalar(Qry.ToString(), null, Get_Trx()));
                                        MVAMProdStockSummary oldTrs = new MVAMProdStockSummary(GetCtx(), oldSummary_ID, Get_Trx());
                                        oldTrs.SetMovementToDate(Convert.ToDateTime(_moveDate).AddDays(-1));
                                        if (!oldTrs.Save())
                                        {
                                            log.Info(Msg.GetMsg(GetCtx(), "StockSummaryNotSaved"));
                                            Rollback();
                                        }
                                        else
                                        {
                                            log.Info(Msg.GetMsg(GetCtx(), "StockSummarySaved"));
                                            Commit();
                                        }
                                    }
                                    else
                                    {
                                        log.Info(Msg.GetMsg(GetCtx(), "StockSummarySaved"));
                                        Commit();
                                    }
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
