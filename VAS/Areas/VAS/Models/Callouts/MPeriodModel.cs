using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MPeriodModel
    {
        /// <summary>
        /// Get Period
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="fields">Parameter</param>
        /// <returns>Period ID</returns>
        public int GetPeriod(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int period_ID;

            int AD_Client_ID = Util.GetValueOfInt(paramValue[0]);
            DateTime? dateAcct = Util.GetValueOfDateTime(paramValue[1]);
            int AD_Org_ID = Util.GetValueOfInt(paramValue[2]);

            period_ID = MPeriod.GetC_Period_ID(ctx, dateAcct, AD_Org_ID);
            return period_ID;
        }

        /// <summary>
        /// Get Period Detail
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="fields">Parameter</param>
        /// <returns>Period ID</returns>
        public Dictionary<string, object> GetPeriodDetail(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            Dictionary<string, object> obj = null;
            int Period_ID = Util.GetValueOfInt(paramValue[0]);

            string sql = "SELECT PeriodType, StartDate, EndDate "
                + "FROM C_Period WHERE C_Period_ID=" + Period_ID;
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                obj = new Dictionary<string, object>();
                obj["PeriodType"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["PeriodType"]);
                obj["StartDate"] = Util.GetValueOfDateTime(ds.Tables[0].Rows[0]["StartDate"]);
                obj["EndDate"] = Util.GetValueOfDateTime(ds.Tables[0].Rows[0]["EndDate"]);
            }
            return obj;
        }
    }
}