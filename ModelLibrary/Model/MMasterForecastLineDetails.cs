
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
    public class MMasterForecastLineDetails : X_C_MasterForecastLineDetails
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

        public MMasterForecastLineDetails(MMasterForecastLine Parent) :
           base(Parent.GetCtx(), 0, Parent.Get_Trx())
        {
            int LineNo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT NVL(MAX(LineNo), 0)  AS DefaultValue FROM C_MasterForecastLineDetails WHERE C_MasterForecastLine_ID=" + Parent.GetC_MasterForecastLine_ID(), null, Get_Trx()));
            SetAD_Client_ID(Parent.GetAD_Client_ID());
            SetAD_Org_ID(Parent.GetAD_Org_ID());
            SetC_MasterForecastLine_ID(Parent.GetC_MasterForecastLine_ID());
            SetLineNo(LineNo + 10);
            SetM_Product_ID(Parent.GetM_Product_ID());
            SetM_AttributeSetInstance_ID(Parent.GetM_AttributeSetInstance_ID());
            SetC_UOM_ID(Parent.GetC_UOM_ID());
            SetC_Charge_ID(Parent.GetC_Charge_ID());
            SetQtyEntered(Parent.GetForcastQty());
            SetPriceEntered(Parent.GetPrice());
            SetTotaAmt(GetQtyEntered() * GetPriceEntered());
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Product_ID">Product</param>
        /// <param name="ctx">Context</param>
        /// <param name="trx">Transaction</param>
        /// <param name="MasterForecastLine_ID">MasterForecastLine</param>
        public MMasterForecastLineDetails(Ctx ctx, Trx trx, int Product_ID, int MasterForecastLine_ID) :
           base(ctx, 0, trx)
        {
            int LineNo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT NVL(MAX(LineNo), 0)+10  AS DefaultValue FROM C_MasterForecastLineDetails WHERE" +
                       " C_MasterForecastLine_ID=" + MasterForecastLine_ID, null, Get_Trx()));
            SetC_MasterForecastLine_ID(MasterForecastLine_ID);
            SetLineNo(LineNo);
            SetM_Product_ID(Product_ID);

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
            if (!Util.GetValueOfBool(Get_Value("IsAdjusted")))
            {
                //delete adjusted line details if new line detail is created against the same product.
                DeleteAdjustedLineDetails();
               
                string _sql = "SELECT StdPrecision FROM C_Currency WHERE C_Currency_ID=(SELECT C_Currency_ID FROM C_MasterForecast WHERE C_MasterForecast_ID =" +
                "(SELECT C_MasterForecast_ID FROM C_MasterForecastLine WHERE C_MasterForecastLine_ID =" + GetC_MasterForecastLine_ID() + "))";
                int Precision = Util.GetValueOfInt(DB.ExecuteScalar(_sql, null, Get_Trx()));
              
                //update Amounts at master forecast line  
                _sql = "UPDATE C_MasterForecastLine SET " +
               "ForcastQty=(SELECT NVL(SUM(QtyEntered),0) FROM C_MasterForecastLineDetails WHERE (NVL(C_Forecast_ID,0)>0 OR (NVL(C_Order_ID,0)=0 AND NVL(C_Project_ID,0)=0)) AND C_MasterForecastLine_ID=" + GetC_MasterForecastLine_ID() + "), " +
               "SalesOrderQty =(SELECT NVL(SUM(QtyEntered),0) FROM C_MasterForecastLineDetails WHERE NVL(C_Order_ID,0)>0 AND NVL(C_Forecast_ID,0)=0  AND C_MasterForecastLine_ID=" + GetC_MasterForecastLine_ID() + "), " +
               "OppQty=(SELECT NVL(SUM(QtyEntered),0) FROM C_MasterForecastLineDetails WHERE NVL(C_Project_ID,0)>0 AND NVL(C_Forecast_ID,0)=0  AND C_MasterForecastLine_ID=" + GetC_MasterForecastLine_ID() + "), " +
               "TotalQty=(SELECT NVL(SUM(QtyEntered),0) FROM C_MasterForecastLineDetails WHERE  C_MasterForecastLine_ID=" + GetC_MasterForecastLine_ID() + ") , " +
               "Price= ROUND((SELECT NVL(SUM(TotaAmt),0)/ NVL(SUM(QtyEntered),0) FROM C_MasterForecastLineDetails WHERE C_MasterForecastLine_ID=" + GetC_MasterForecastLine_ID() + ")," + Precision + ")," +
               "PlannedRevenue =ROUND((SELECT SUM(TotaAmt) FROM C_MasterForecastLineDetails WHERE C_MasterForecastLine_ID=" + GetC_MasterForecastLine_ID() + ")," + Precision + ")" +
               " WHERE C_MasterForecastLine_ID=" + GetC_MasterForecastLine_ID();

                if (DB.ExecuteQuery(_sql, null, Get_Trx()) < 0)
                {
                    log.SaveError("ForecastLineNotUpdated", "");
                    return false;
                }
                //else
                //{
                //    //update price and totalamt on linedetails in case after adjustment user again creates the line.
                //    _sql = "UPDATE C_MasterForecastLineDetails SET PriceEntered=(SELECT Price FROM C_MasterForecastLine WHERE C_MasterForecastLine_ID=" + GetC_MasterForecastLine_ID() + "), " +
                //        "TotaAmt= ROUND((SELECT Price FROM C_MasterForecastLine WHERE C_MasterForecastLine_ID=" + GetC_MasterForecastLine_ID() + ")+" +
                //        "(SELECT QtyEntered FROM C_MasterForecastLineDetails WHERE C_MasterForecastLine_ID=" + GetC_MasterForecastLine_ID() + " AND IsAdjusted='Y')," + Precision + ") " +
                //        " WHERE C_MasterForecastLine_ID=" + GetC_MasterForecastLine_ID() + " AND IsAdjusted='Y' ";

                //    DB.ExecuteQuery(_sql, null, Get_Trx());
                //    if (DB.ExecuteQuery(_sql, null, Get_Trx()) < 0)
                //    {
                //        log.SaveError("LineDetailNotUpdated", "");
                //        return false;
                //    }
                //}
               
            }
            return true;
        }

        /// <summary>
        /// This function is used to delete the Adjusted linedetails if new 
        /// forecastline has added against the product which has already been adjusted
        /// </summary>
        /// <writer>209</writer>
        public void DeleteAdjustedLineDetails()
        {
            string sql = @"DELETE FROM C_MasterForecastLineDetails   WHERE 
            IsAdjusted = 'Y' AND M_Product_ID = " + GetM_Product_ID() + @" AND C_MasterForecastLine_ID IN
            (SELECT C_MasterForecastLine_ID FROM C_MasterForecastLine WHERE C_MasterForecast_ID = 
            (SELECT C_MasterForecast_ID FROM C_MasterForecastLine WHERE C_MasterForecastLine_ID=" + GetC_MasterForecastLine_ID() + " ))";


            if (DB.ExecuteQuery(sql, null, Get_Trx()) > 0)
            {
                sql = "UPDATE C_MasterForecastLine SET AdjustedQty=0 WHERE M_Product_ID =  " + GetM_Product_ID() + @" AND C_MasterForecastLine_ID IN
               (SELECT C_MasterForecastLine_ID FROM C_MasterForecastLine WHERE C_MasterForecast_ID = 
               (SELECT C_MasterForecast_ID FROM C_MasterForecastLine WHERE C_MasterForecastLine_ID=" + GetC_MasterForecastLine_ID() + " ))";
                DB.ExecuteQuery(sql, null, Get_Trx());
            }

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

        /// <summary>
        /// Is Used to Get or Create  Instance of MasterForecastLineDetails
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="trx">Transaction</param>
        /// <param name="MasterForecastLine_ID">MasterForecatLine</param>
        /// <param name="M_Product_ID">Product</param>
        /// <param name="IsAdjusted">Adjusted</param>
        /// <returns>Masterforecastlinedetails</returns>
        public static MMasterForecastLineDetails GetOrCreate(Ctx ctx, Trx trx, int MasterForecastLine_ID, int M_Product_ID, bool IsAdjusted)
        {
            MMasterForecastLineDetails retValue = null;

            String sql = "SELECT * FROM C_MasterForecastLineDetails WHERE NVL(M_Product_ID,0) = " + M_Product_ID +
             " AND C_ForecastLine_ID IS NULL AND C_OrderLine_ID IS NULL AND C_ProjectLine_ID IS NULL " +
             " AND IsAdjusted='Y' AND NVL(C_MasterForecastLine_ID, 0) = " + MasterForecastLine_ID;


            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, trx);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MMasterForecastLineDetails(ctx, dr, trx);
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

            if (retValue == null)
                retValue = new MMasterForecastLineDetails(ctx, trx, M_Product_ID, MasterForecastLine_ID);

            return retValue;
        }

    }
}
