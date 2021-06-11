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

            // Get Precision
            sql.Append(@"SELECT StdPrecision FROM C_Currency WHERE C_Currency_ID =
                            (SELECT C_Currency_ID FROM C_MasterForecast WHERE C_MasterForecast_ID = " + GetRecord_ID() + ")");
            int precision = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString()));

            sql.Clear();
            sql.Append(@"SELECT C_MasterForecastLine_ID, TotalQty, AdjustedQty  FROM C_MasterForecastLine ");
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
                    quantity = Util.GetValueOfDecimal(dsMasterForecastLine.Tables[0].Rows[i]["TotalQty"]);

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
                }
            }

            return Msg.GetMsg(GetCtx(), "SuccessfullyAdjusted");
        }
    }
}
