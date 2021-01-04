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

namespace VAdvantage.Model
{
    public class MForecastLine : X_C_ForecastLine
    {
        public MForecastLine(Ctx ctx, int C_ForecastLine_ID, Trx trxName) :
           base(ctx, C_ForecastLine_ID, null)
        {

        }

        public MForecastLine(Ctx ctx, DataRow dr, Trx trxName) :
           base(ctx, dr, trxName)
        {

        }

        protected override bool AfterDelete(bool success)
        {
            if (!success)
                return success;
            string sql = "update C_Forecast set GrandTotal = (SELECT COALESCE(SUM(PriceStd),0) FROM C_ForecastLine WHERE isactive = 'Y' and C_Forecast_ID= " + GetC_Forecast_ID() + ") where C_Forecast_ID = " + GetC_Forecast_ID();
            int count = DB.ExecuteQuery(sql, null, null);
            return true;
        }

        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success)
                return success;
            // Develop By Deekshant For margin amount and total amount
            if (VAdvantage.Utility.Env.IsModuleInstalled("VA077_"))
            {
                double amount = Util.GetValueOfDouble(Get_Value("VA077_PurchasePrice"));
                double qty = Util.GetValueOfDouble(GetQtyEntered());
                double total = amount * qty;
                double margin = Util.GetValueOfDouble(GetPriceStd()) - Util.GetValueOfDouble(total);
                string query = "UPDATE C_ForeCastLine SET VA077_PurchaseAmount=" + total + " , VA077_MarginAmt=" + margin + " WHERE C_ForeCastLine_ID=" + GetC_ForecastLine_ID();
                int res = Util.GetValueOfInt(DB.ExecuteQuery(query, null, Get_Trx()));
                if (res <= 0)
                {
                    Get_Trx().Rollback();
                    return false;
                }
            }
            string sql = "update C_Forecast set GrandTotal = (SELECT COALESCE(SUM(PriceStd),0) FROM C_ForecastLine WHERE isactive = 'Y' and C_Forecast_ID= " + GetC_Forecast_ID() + ") where C_Forecast_ID = " + GetC_Forecast_ID();
            int count = DB.ExecuteQuery(sql, null, null);
            return true;
        }
    }
}
