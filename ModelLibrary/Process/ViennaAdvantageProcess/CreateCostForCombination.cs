/********************************************************
    * Project Name   : 
    * Class Name     : CreateCostForCombination
    * Purpose        : Calculate Cost of Multiple costing element
    * Class Used     : ProcessEngine.SvrProcess
    * Chronological    Development
    * Amit Bansal     08-April-2016
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
    public class CreateCostForCombination : SvrProcess
    {
        private int costElement_ID = 0;
        private String sql = null;
        private DataSet dsCostCombination = null;
        private DataSet dsProductCost = null;
        MVAMVAMProductCost costcombination = null;
        private int _VAM_Product_ID = 0;
        private int _vaf_org_ID = 0;
        private int _VAM_PFeature_SetInstance_ID = 0;
        private int _VAB_AccountBook_ID = 0;
        List<int> costElement = new List<int>();
        MVAMVAMProductCostElement ce = null;

        protected override void Prepare()
        {
            costElement_ID = GetRecord_ID();
        }

        protected override string DoIt()
        {
            try
            {
                // Get Combination Record
                sql = @"SELECT ce.VAM_ProductCostElement_ID ,  ce.Name ,  cel.lineno ,  cel.m_ref_costelement
                            FROM VAM_ProductCostElement ce INNER JOIN VAM_ProductCostElementLine cel ON ce.VAM_ProductCostElement_ID = cel.VAM_ProductCostElement_ID "
                              + " WHERE ce.VAF_Client_ID=" + GetVAF_Client_ID() + " AND ce.VAM_ProductCostElement_ID = " + costElement_ID
                              + " AND ce.IsActive='Y'  AND cel.IsActive='Y'";
                dsCostCombination = DB.ExecuteDataset(sql, null, null);
                if (dsCostCombination != null && dsCostCombination.Tables.Count > 0 && dsCostCombination.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsCostCombination.Tables[0].Rows.Count; i++)
                    {
                        costElement.Add(Util.GetValueOfInt(dsCostCombination.Tables[0].Rows[i]["m_ref_costelement"]));
                    }
                }
                //var costElementRecord = dsCostCombination.Tables[0].AsEnumerable().Select(r => r.Field<int>("m_ref_costelement")).ToList();

                // Get All Product
                sql = @"SELECT vaf_client_id ,  vaf_org_id ,  VAM_Product_id ,  VAM_PFeature_SetInstance_id ,  VAB_AccountBook_id ,
                           VAM_ProductCostType_id ,   VAM_ProductCostElement_id ,  cumulatedamt ,  cumulatedqty ,  currentcostprice ,  currentqty
                      FROM VAM_ProductCost WHERE vaf_client_id = " + GetVAF_Client_ID() +
                          " ORDER BY VAM_Product_id ,   vaf_org_id ,  VAM_PFeature_SetInstance_id ,  VAB_AccountBook_id";
                dsProductCost = DB.ExecuteDataset(sql, null, null);

                if (dsProductCost != null && dsProductCost.Tables.Count > 0 && dsProductCost.Tables[0].Rows.Count > 0)
                {
                    // update all record of VAM_ProductCost having cost Element = costElement_ID
                    sql = "UPDATE VAM_ProductCost SET currentcostprice = 0 , currentqty = 0 , cumulatedamt = 0 , cumulatedqty = 0 WHERE VAM_ProductCostElement_ID = " + costElement_ID +
                         " AND VAF_Client_ID = " + GetVAF_Client_ID();
                    int no = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));

                    for (int i = 0; i < dsProductCost.Tables[0].Rows.Count; i++)
                    {
                        if (!costElement.Contains(Util.GetValueOfInt(dsProductCost.Tables[0].Rows[i]["VAM_ProductCostElement_id"])))
                            continue;
                        if (_VAM_Product_ID != Util.GetValueOfInt(dsProductCost.Tables[0].Rows[i]["VAM_Product_id"]) ||
                             _vaf_org_ID != Util.GetValueOfInt(dsProductCost.Tables[0].Rows[i]["vaf_org_id"]) ||
                             _VAM_PFeature_SetInstance_ID != Util.GetValueOfInt(dsProductCost.Tables[0].Rows[i]["VAM_PFeature_SetInstance_id"]) ||
                            _VAB_AccountBook_ID != Util.GetValueOfInt(dsProductCost.Tables[0].Rows[i]["VAB_AccountBook_id"]))
                        {
                            _VAM_Product_ID = Util.GetValueOfInt(dsProductCost.Tables[0].Rows[i]["VAM_Product_id"]);
                            _vaf_org_ID = Util.GetValueOfInt(dsProductCost.Tables[0].Rows[i]["vaf_org_id"]);
                            _VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(dsProductCost.Tables[0].Rows[i]["VAM_PFeature_SetInstance_id"]);
                            _VAB_AccountBook_ID = Util.GetValueOfInt(dsProductCost.Tables[0].Rows[i]["VAB_AccountBook_id"]);
                            MVAMProduct product = new MVAMProduct(GetCtx(), _VAM_Product_ID, Get_TrxName());
                            MVABAccountBook acctSchema = new MVABAccountBook(GetCtx(), _VAB_AccountBook_ID, Get_TrxName());
                            costcombination = MVAMVAMProductCost.Get(product, _VAM_PFeature_SetInstance_ID, acctSchema, _vaf_org_ID, Util.GetValueOfInt(dsCostCombination.Tables[0].Rows[0]["VAM_ProductCostElement_ID"]));
                        }

                        // created object of Cost elemnt for checking iscalculated = true/ false
                        ce = MVAMVAMProductCostElement.Get(GetCtx(), Util.GetValueOfInt(dsProductCost.Tables[0].Rows[i]["VAM_ProductCostElement_id"]));

                        costcombination.SetCurrentCostPrice(Decimal.Add(costcombination.GetCurrentCostPrice(), Util.GetValueOfDecimal(dsProductCost.Tables[0].Rows[i]["currentcostprice"])));
                        costcombination.SetCumulatedAmt(Decimal.Add(costcombination.GetCumulatedAmt(), Util.GetValueOfDecimal(dsProductCost.Tables[0].Rows[i]["cumulatedamt"])));
                        
                        // if calculated = true then we added qty else not and costing method is Standard Costing
                        if (ce.IsCalculated() || ce.GetCostingMethod() == MVAMVAMProductCostElement.COSTINGMETHOD_StandardCosting)
                        {
                            costcombination.SetCurrentQty(Decimal.Add(costcombination.GetCurrentQty(), Util.GetValueOfDecimal(dsProductCost.Tables[0].Rows[i]["currentqty"])));
                            costcombination.SetCumulatedQty(Decimal.Add(costcombination.GetCumulatedQty(), Util.GetValueOfDecimal(dsProductCost.Tables[0].Rows[i]["cumulatedqty"])));
                        }
                        if (costcombination.Save())
                        {
                            Commit();
                        }
                        else
                        {
                            log.Info("Cost Combination not updated for this product <===> " + Util.GetValueOfInt(dsProductCost.Tables[0].Rows[i]["VAM_Product_id"]));
                        }
                    }
                    dsProductCost.Dispose();
                }
            }
            catch
            {
                if (dsProductCost != null)
                {
                    dsProductCost.Dispose();
                }
                if (dsCostCombination != null)
                {
                    dsCostCombination.Dispose();
                }
            }
            return Msg.GetMsg(GetCtx(), "SucessfullyUpdated");
        }
    }
}
