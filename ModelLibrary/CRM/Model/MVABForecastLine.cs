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
    public class MVABForecastLine : X_VAB_ForecastLine
    {
         public MVABForecastLine(Ctx ctx, int VAB_ForecastLine_ID, Trx trxName) :
            base(ctx, VAB_ForecastLine_ID, null)
        {

        }

         public MVABForecastLine(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {

        }

         protected override bool AfterDelete(bool success)
         {
             if (!success)
                 return success;
             string sql = "update VAB_Forecast set GrandTotal = (SELECT COALESCE(SUM(PriceStd),0) FROM VAB_ForecastLine WHERE isactive = 'Y' and VAB_Forecast_ID= " + GetVAB_Forecast_ID() + ") where VAB_Forecast_ID = " + GetVAB_Forecast_ID();
             int count = DB.ExecuteQuery(sql, null, null);
             return true;
         }

         protected override bool AfterSave(bool newRecord, bool success)
         {
             if (!success)
                 return success;
             string sql = "update VAB_Forecast set GrandTotal = (SELECT COALESCE(SUM(PriceStd),0) FROM VAB_ForecastLine WHERE isactive = 'Y' and VAB_Forecast_ID= " + GetVAB_Forecast_ID() + ") where VAB_Forecast_ID = " + GetVAB_Forecast_ID();
             int count = DB.ExecuteQuery(sql, null, null);
             return true;
         }
    }
}
