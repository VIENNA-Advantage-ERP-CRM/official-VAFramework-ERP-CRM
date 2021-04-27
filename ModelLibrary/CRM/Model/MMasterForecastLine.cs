
/********************************************************
 * Module Name    :    VA Framework
 * Purpose        :    MasterForecast Line Model
 * Employee Code  :    209
 * Date           :    26-April-2021
  ******************************************************/
using System;
using System.Net;
using System.Windows;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MMasterForecastLine : X_C_MasterForecastLine
    {
        private static VLogger log = VLogger.GetVLogger(typeof(MMasterForecastLine).FullName);

        public MMasterForecastLine(Ctx ctx, int C_MasterForecastLine_ID, Trx trxName) :
            base(ctx, C_MasterForecastLine_ID, null)
        {

        }

        public MMasterForecastLine(Ctx ctx, DataRow dr, Trx trxName) :
           base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Create new Master Forecast Line
        /// </summary>
        /// <param name="Parent">Master Forecast</param>
        /// <param name="M_Product_ID">Product</param>
        public MMasterForecastLine(Ctx ctx ,Trx trx ,int C_MasterForecast_ID ,int M_Product_ID, int M_AttributeSetInstance_ID)
            : base(ctx, 0, trx)
        {
            int LineNo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT NVL(MAX(Line), 0)+10  FROM C_MasterForecastLine WHERE " +
               "C_MasterForecast_ID=" + C_MasterForecast_ID, null, trx));
            string sql = "SELECT C_UOM_ID FROM M_Product WHERE M_Product_ID = " + M_Product_ID;
            SetLine(LineNo);
            SetC_UOM_ID(Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null)));          
            SetM_Product_ID(M_Product_ID);
            SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
          
        }

        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success)
                return success;
            string sql = "update C_MasterForecast set GrandTotal = (SELECT COALESCE(SUM(PlannedRevenue),0) FROM C_MasterForecastLine WHERE isactive = 'Y' and C_MasterForecast_ID= " + GetC_MasterForecast_ID() + ") where C_MasterForecast_ID = " + GetC_MasterForecast_ID();
            int count = DB.ExecuteQuery(sql, null, null);
            return true;
            //return base.AfterSave(newRecord, success);
        }

        protected override bool AfterDelete(bool success)
        {
            if (!success)
                return success;
            string sql = "update C_MasterForecast set GrandTotal = (SELECT COALESCE(SUM(PlannedRevenue),0) FROM C_MasterForecastLine WHERE isactive = 'Y' and C_MasterForecast_ID= " + GetC_MasterForecast_ID() + ") where C_MasterForecast_ID = " + GetC_MasterForecast_ID();
            int count = DB.ExecuteQuery(sql, null, null);
            return true;
            // return base.AfterDelete(success);
        }


        /// <summary>
        /// Is Used to Get or Create  Instance of Master ForecastLine
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="trx">Transaction</param>
        /// <param name="MasterForeCast_ID">Master Forecast</param>
        /// <param name="M_Product_ID">Product</param>
        /// <param name="M_AttributeSetInstance_ID">AttributesetInsatnce</param>
        /// <param name="ProductCategories">Product Categories</param>
        /// <returns></returns>
        public static MMasterForecastLine GetOrCreate(Ctx ctx,Trx trx,int MasterForeCast_ID, int M_Product_ID, int M_AttributeSetInstance_ID,string ProductCategories)
        {
            MMasterForecastLine retValue = null;
            String sql = "SELECT * FROM C_MasterForecastLine WHERE ";
            if (!String.IsNullOrEmpty(ProductCategories))
            {
                sql += "NVL(M_Product_ID,0) = " + M_Product_ID + " AND C_ForecastLine_ID IS NULL AND C_OrderLine_ID IS NULL AND C_ProjectLine_ID IS NULL AND ";
            }
            else
            {
                if (M_AttributeSetInstance_ID > 0)
                {
                    sql += " NVL(M_AttributeSetInstance_ID,0)=" + M_AttributeSetInstance_ID+" AND ";
                }
                if (M_Product_ID > 0)
                {
                    sql += " NVL(M_Product_ID,0) = " + M_Product_ID +" AND";
                }
              
            }
            sql+= " NVL(C_MasterForecast_ID,0) =" + MasterForeCast_ID;
             
            
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
                    retValue = new MMasterForecastLine(ctx, dr, trx);
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
                retValue = new MMasterForecastLine(ctx,trx , MasterForeCast_ID, M_Product_ID, M_AttributeSetInstance_ID);

            return retValue;
        }

        /// <summary>
        /// Add to Description
        /// </summary>
        /// <param name="description">text</param>
        public void AddDescription(String description)
        {
            String desc = GetDescription();
            if (desc == null)
            {
                SetDescription(description);
            }
            else
            {
                SetDescription(desc + " | " + description);
            }
        }
    }
}
