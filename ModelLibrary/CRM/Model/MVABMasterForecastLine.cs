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
    public class MVABMasterForecastLine : X_VAB_MasterForecastLine
    {
        public MVABMasterForecastLine(Ctx ctx, int VAB_MasterForecastLine_ID, Trx trxName) :
            base(ctx, VAB_MasterForecastLine_ID, null)
        {

        }

         public MVABMasterForecastLine(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {

        }

         protected override bool AfterSave(bool newRecord, bool success)
         {
             if (!success)
                 return success;
             string sql = "update VAB_MasterForecast set GrandTotal = (SELECT COALESCE(SUM(PlannedRevenue),0) FROM VAB_MasterForecastLine WHERE isactive = 'Y' and VAB_MasterForecast_ID= " + GetVAB_MasterForecast_ID() + ") where VAB_MasterForecast_ID = " + GetVAB_MasterForecast_ID();
             int count = DB.ExecuteQuery(sql,null, null);
             return true;
             //return base.AfterSave(newRecord, success);
         }

         protected override bool AfterDelete(bool success)
         {
             if (!success)
                 return success;
             string sql = "update VAB_MasterForecast set GrandTotal = (SELECT COALESCE(SUM(PlannedRevenue),0) FROM VAB_MasterForecastLine WHERE isactive = 'Y' and VAB_MasterForecast_ID= " + GetVAB_MasterForecast_ID() + ") where VAB_MasterForecast_ID = " + GetVAB_MasterForecast_ID();
             int count = DB.ExecuteQuery(sql, null, null);
             return true;
            // return base.AfterDelete(success);
         }
    }
}
