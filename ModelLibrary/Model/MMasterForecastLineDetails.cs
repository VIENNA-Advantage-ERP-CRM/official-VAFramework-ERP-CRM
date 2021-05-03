
/********************************************************
 * Module Name    :    VA Framework
 * Purpose        :    MasterForecast LineDetails
 * Employee Code  :    209
 * Date           :    26-April-2021
  ******************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MMasterForecastLineDetails:X_C_MasterForecastLineDetails
    {
        private static VLogger log = VLogger.GetVLogger(typeof(MMasterForecastLineDetails).FullName);

        public MMasterForecastLineDetails(Ctx ctx, int C_MasterForecastLineDetails_ID, Trx trxName) :
           base(ctx, C_MasterForecastLineDetails_ID, null)
        {

        }

        public MMasterForecastLineDetails(Ctx ctx, DataRow dr, Trx trxName) :
           base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Create new MasterForeastLineDetails
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="trx">Transaction</param>
        /// <param name="C_MasterForecast_ID">Master Forecast</param>
        /// <param name="M_Product_ID">Product</param>
        /// <param name="M_AttributeSetInstance_ID">Attribute set Insatnce</param>
        public MMasterForecastLineDetails(MMasterForecastLine Parent, int M_Product_ID)
            : base(Parent.GetCtx(), 0, Parent.Get_Trx())
        {          
            //SetC_UOM_ID(Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null)));          
            SetM_Product_ID(M_Product_ID);          
        }
        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">New record</param>
        /// <param name="success">Success</param>
        /// <returns>true/false</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            //update Amounts at master forecast line  
            string _sql = "UPDATE C_MasterForecastLine SET " +
            "ForcastQty=(SELECT NVL(SUM(QtyEntered),0) FROM C_MasterForecastLineDetails WHERE (NVL(C_Forecast_ID,0)>0 OR (NVL(C_Order_ID,0)=0 AND NVL(C_Project_ID,0)=0)) AND C_MasterForecastLine_ID=" + GetC_MasterForecastLine_ID() + "), " +
            "SalesOrderQty =(SELECT NVL(SUM(QtyEntered),0) FROM C_MasterForecastLineDetails WHERE NVL(C_Order_ID,0)>0 AND C_MasterForecastLine_ID=" + GetC_MasterForecastLine_ID() + "), " +
            "OppQty=(SELECT NVL(SUM(QtyEntered),0) FROM C_MasterForecastLineDetails WHERE NVL(C_Project_ID,0)>0 AND C_MasterForecastLine_ID=" + GetC_MasterForecastLine_ID() + "), " +
            "TotalQty=(SELECT NVL(SUM(QtyEntered),0) FROM C_MasterForecastLineDetails WHERE  C_MasterForecastLine_ID=" + GetC_MasterForecastLine_ID() + ") , " +
            "Price= (SELECT NVL(SUM(TotaAmt),0)/ NVL(SUM(QtyEntered),0) FROM C_MasterForecastLineDetails WHERE C_MasterForecastLine_ID=" + GetC_MasterForecastLine_ID() + ")," +
            "PlannedRevenue =(SELECT SUM(TotaAmt) FROM C_MasterForecastLineDetails WHERE C_MasterForecastLine_ID=" + GetC_MasterForecastLine_ID() + ")" +
            " WHERE C_MasterForecastLine_ID=" + GetC_MasterForecastLine_ID();
        
            if (DB.ExecuteQuery(_sql, null, Get_Trx()) < 0)
            {
                log.SaveError("ForecastLineNotUpdated", "");
                return false;
            }
            return true;
        }

        /// <summary>
        ///  Is Used to Get or Create  Instance of MasterForecastLineDetails
        /// </summary>
        /// <param name="Line">MMasterForecastLine</param>
        /// <param name="M_Product_ID">Product</param>
        /// <param name="ProductCategories">Product category</param>
        /// <returns> Masterforecastlinedetails</returns>
        public static MMasterForecastLineDetails GetOrCreate(MMasterForecastLine Line, int M_Product_ID, string ProductCategories)
        {
            MMasterForecastLineDetails retValue = null;
            if (!String.IsNullOrEmpty(ProductCategories))
            {
                String sql = "SELECT * FROM C_MasterForecastLineDetails WHERE ";
                sql += "NVL(M_Product_ID,0) = " + M_Product_ID + " AND C_ForecastLine_ID IS NULL AND C_OrderLine_ID IS NULL AND C_ProjectLine_ID IS NULL AND " +
                    " NVL(C_MasterForecastLine_ID,0) =" + Line.GetC_MasterForecastLine_ID();

                DataTable dt = null;
                IDataReader idr = null;
                try
                {
                    idr = DB.ExecuteReader(sql, null, Line.Get_Trx());
                    dt = new DataTable();
                    dt.Load(idr);
                    idr.Close();
                    foreach (DataRow dr in dt.Rows)
                    {
                        retValue = new MMasterForecastLineDetails(Line.GetCtx(), dr, Line.Get_Trx());
                    }
                }
                catch (Exception ex)
                {
                    if (idr != null)
                    {
                        idr.Close();
                    }
                    log.Log(Level.SEVERE, sql, ex);
                }
                finally
                {
                    if (idr != null)
                    {
                        idr.Close();
                    }
                    dt = null;
                }
            }
            if (retValue == null)
                retValue = new MMasterForecastLineDetails(Line, M_Product_ID);

            return retValue;
        }

    }
}
