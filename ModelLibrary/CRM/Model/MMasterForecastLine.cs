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
    public class MMasterForecastLine : X_C_MasterForecastLine
    {
        public MMasterForecastLine(Ctx ctx, int C_MasterForecastLine_ID, Trx trxName) :
            base(ctx, C_MasterForecastLine_ID, null)
        {

        }

         public MMasterForecastLine(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {

        }

         protected override bool AfterSave(bool newRecord, bool success)
         {
             if (!success)
                 return success;
             string sql = "update C_MasterForecast set GrandTotal = (SELECT COALESCE(SUM(PlannedRevenue),0) FROM C_MasterForecastLine WHERE isactive = 'Y' and C_MasterForecast_ID= " + GetC_MasterForecast_ID() + ") where C_MasterForecast_ID = " + GetC_MasterForecast_ID();
             int count = DB.ExecuteQuery(sql,null, null);
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
    }
}
