using System;
using System.Net;
using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Documents;
//using System.Windows.Ink;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Animation;
//using System.Windows.Shapes;
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
        public MMasterForecastLine(X_C_MasterForecast Parent, int M_Product_ID, int M_AttributeSetInstance_ID)
            : base(Parent.GetCtx(), 0, Parent.Get_Trx())
        {

            string sql = "SELECT C_UOM_ID FROM M_Product WHERE M_Product_ID = " + M_Product_ID;
            SetC_UOM_ID(Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null)));

            SetAD_Client_ID(Parent.GetAD_Client_ID());
            SetAD_Org_ID(Parent.GetAD_Org_ID());
            SetM_Product_ID(M_Product_ID);
            if (!Env.IsModuleInstalled("VA073_"))
            {
                SetProcessed(true);
            }
            else
            {
                Set_Value("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
            }
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
        /// <param name="ForeCast">MasterForecast</param>
        /// <param name="M_Product_ID">Product</param>
        /// <returns>Object</returns>
        public static MMasterForecastLine GetOrCreate(X_C_MasterForecast ForeCast, int M_Product_ID, int M_AttributeSetInstance_ID)
        {
            MMasterForecastLine retValue = null;
            String sql = "SELECT * FROM C_MasterForecastLine " +
                         " WHERE NVL(M_Product_ID,0) = " + M_Product_ID +
                         " AND NVL(C_MasterForecast_ID,0) =" + ForeCast.GetC_MasterForecast_ID();

            if (Env.IsModuleInstalled("VA073_"))
            {
                sql+= " AND NVL(M_AttributeSetInstance_ID,0)=" + M_AttributeSetInstance_ID;
            }
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, ForeCast.Get_Trx());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MMasterForecastLine(ForeCast.GetCtx(), dr, ForeCast.Get_Trx());
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
                retValue = new MMasterForecastLine(ForeCast, M_Product_ID, M_AttributeSetInstance_ID);

            return retValue;
        }

    }
}
