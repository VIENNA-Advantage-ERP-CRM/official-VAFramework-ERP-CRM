using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MDashboardModel
    {
        public int GroupByChecked(Ctx ctx, int RC_View_ID, int RC_ViewColumn_ID)
        {
            string sql = "UPDATE RC_ViewColumn SET IsGroupBy='N' WHERE RC_View_ID=" + RC_View_ID + " AND RC_ViewColumn_ID NOT IN(" + RC_ViewColumn_ID + ")";
            int count = DB.ExecuteQuery(sql);
            return count;
        }
    }
}