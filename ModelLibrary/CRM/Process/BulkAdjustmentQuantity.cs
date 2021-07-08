/********************************************************
    * Project Name   : VAdvantage
    * Class Name     : BulkAdjustmentQuantity
    * Purpose        : The bulk adjustment process will be used to adjust the product quantities based on the following parameters:
                        a)	Product Category – Table Direct – Not mandatory
                        b)	Product	 - Multi-Select – Not Mandatory
                        c)	Percentage – Number - Mandatory
    * Class Used     : ProcessEngine.SvrProcess
    * Chronological  : Development
    * Neha ThakurAmit Bansal    : 19-April-2021
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
    public class BulkAdjustmentQuantity : SvrProcess
    {
        private static VLogger _log = VLogger.GetVLogger(typeof(BulkAdjustmentQuantity).FullName);
        private ValueNamePair pp = null;
        private Decimal _Percentage = 0;
        private int M_Product_Category_ID = 0;
        private String M_Product_IDs = string.Empty;

        /// <summary>
        /// Prepare Document filterration
        /// </summary>
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
                else if (name.Equals("M_Product_Category_ID"))
                {
                    M_Product_Category_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("M_Product_ID"))
                {
                    M_Product_IDs = Util.GetValueOfString(para[i].GetParameter());
                }
                else if (name.Equals("Percentage"))
                {
                    _Percentage = Util.GetValueOfDecimal(para[i].GetParameter());
                }
            }
        }

        /// <summary>
        /// Prepare Logic 
        /// </summary>
        /// <returns>successfull message</returns>
        protected override string DoIt()
        {
            Decimal quantity = 0;
            Decimal percentageBasedQty = 0;
            int no = 0;

            StringBuilder sql = new StringBuilder();

            MMasterForecastLineDetails lineDetails = null;

            // Get Precision
            sql.Append(@"SELECT StdPrecision FROM C_Currency WHERE C_Currency_ID =
                            (SELECT C_Currency_ID FROM C_MasterForecast WHERE C_MasterForecast_ID = " + GetRecord_ID() + ")");
            int precision = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString()));

            sql.Clear();
            sql.Append(@"SELECT C_MasterForecastLine.AD_Org_ID,C_MasterForecastLine_ID, C_MasterForecastLine.TotalQty, C_MasterForecastLine.AdjustedQty,C_MasterForecastLine.M_Product_ID,
            C_MasterForecastLine.C_UOM_ID,C_MasterForecastLine.M_AttributeSetInstance_ID,C_MasterForecastLine.Price,C_MasterForecastLine.ForcastQty,
            C_MasterForecastLine.SalesOrderQty,C_MasterForecastLine.OppQty
            FROM C_MasterForecastLine ");
            if (M_Product_Category_ID > 0 || !String.IsNullOrEmpty(M_Product_IDs))
            {
                sql.Append(@" INNER JOIN M_Product ON M_Product.M_Product_ID = C_MasterForecastLine.M_Product_ID");
                sql.Append(" WHERE ");
                if (!String.IsNullOrEmpty(M_Product_IDs))
                {
                    sql.Append(@" M_Product.M_Product_ID IN (" + M_Product_IDs + ")");
                }
                else if (M_Product_Category_ID > 0)
                {
                    sql.Append(@" M_Product.M_Product_Category_ID IN (" + M_Product_Category_ID + ")");
                }
                sql.Append(" AND C_MasterForecastLine.C_MasterForecast_ID = " + GetRecord_ID());
            }
            else
            {
                sql.Append(" WHERE C_MasterForecastLine.C_MasterForecast_ID = " + GetRecord_ID());
            }
            DataSet dsMasterForecastLine = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
            if (dsMasterForecastLine != null && dsMasterForecastLine.Tables.Count > 0 && dsMasterForecastLine.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsMasterForecastLine.Tables[0].Rows.Count; i++)
                {
                    // Quantity on which percentage to be applied
                    quantity = Util.GetValueOfDecimal(dsMasterForecastLine.Tables[0].Rows[i]["ForcastQty"]) + Util.GetValueOfDecimal(dsMasterForecastLine.Tables[0].Rows[i]["SalesOrderQty"]) + 
                    Util.GetValueOfDecimal(dsMasterForecastLine.Tables[0].Rows[i]["OppQty"]);

                    // quantity which is to be adjsuted
                    percentageBasedQty = Decimal.Divide(Decimal.Multiply(quantity, _Percentage), 100);

                    //
                    no = DB.ExecuteQuery("UPDATE C_MasterForecastLine SET TotalQty = " + Decimal.Add(quantity, percentageBasedQty) +
                                     " , AdjustedQty =  " + percentageBasedQty + 
                                     " , PlannedRevenue = Round((Price * " + Decimal.Add(quantity, percentageBasedQty) + "), " + precision + ")" +
                                     " WHERE C_MasterForecastLine_ID = "
                                     + Util.GetValueOfInt(dsMasterForecastLine.Tables[0].Rows[i]["C_MasterForecastLine_ID"]), null, Get_Trx());

                    _log.Info(no + "Update total qty (" + quantity + " -> " + (Decimal.Add(quantity, percentageBasedQty)) +
                                ") AND Adjustment Qty = " + percentageBasedQty +
                                " AND C_MasterForecastLine_ID = "
                                + Util.GetValueOfInt(dsMasterForecastLine.Tables[0].Rows[i]["C_MasterForecastLine_ID"]));
                    if (no > 0)
                    {
                       //create Master forecast linedetails for adjusted quantity
                        lineDetails = MMasterForecastLineDetails.GetOrCreate(GetCtx(),Get_Trx(),Util.GetValueOfInt(dsMasterForecastLine.Tables[0].Rows[i]["C_MasterForecastLine_ID"]), Util.GetValueOfInt(dsMasterForecastLine.Tables[0].Rows[i]["M_Product_ID"]), true);
                      
                        lineDetails.SetAD_Client_ID(GetAD_Client_ID());
                        lineDetails.SetAD_Org_ID(Util.GetValueOfInt(dsMasterForecastLine.Tables[0].Rows[i]["AD_Org_ID"]));   
                        lineDetails.Set_Value("IsAdjusted", true);
                        lineDetails.SetM_AttributeSetInstance_ID(Util.GetValueOfInt(dsMasterForecastLine.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]));
                        lineDetails.SetC_UOM_ID(Util.GetValueOfInt(dsMasterForecastLine.Tables[0].Rows[i]["C_UOM_ID"]));
                        lineDetails.SetQtyEntered(percentageBasedQty);
                        lineDetails.SetPriceEntered(Util.GetValueOfDecimal(dsMasterForecastLine.Tables[0].Rows[i]["Price"]));
                        lineDetails.SetTotaAmt(Math.Round(lineDetails.GetQtyEntered() * lineDetails.GetPriceEntered(),precision));

                        if (!lineDetails.Save(Get_Trx()))
                        {
                            pp = VLogger.RetrieveError();
                            if (pp != null)
                            {
                                string val = pp.GetName();
                                if (string.IsNullOrEmpty(val))
                                {
                                    val = pp.GetValue();
                                }

                                log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "MasterForecastlineDetailsNotSave") + " " + val);
                            }
                        }
                    }
        
                }
            }
            
            return Msg.GetMsg(GetCtx(), "SuccessfullyAdjusted");
        }
    }
}
