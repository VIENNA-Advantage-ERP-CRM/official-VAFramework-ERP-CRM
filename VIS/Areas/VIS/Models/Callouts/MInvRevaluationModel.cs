/********************************************************
 * Project Name   : VAdvantage
 * Module Name    : VIS
 * Class Name     : MInvRevaluationModel
 * Purpose        : Callout Handling
 * Class Used     : none
 * Chronological  : Development
 * VIS_0045       : 10-March-2023
  ******************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MInvRevaluationModel
    {
        /// <summary>
        /// Get Inventory Revaluation Details
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="fields">Inventory Revaluation ID</param>
        /// <returns>Dictionary Object of Details</returns>
        public Dictionary<string, object> GetInvRevaluationDetails(Ctx ctx, string fields)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            string[] paramValue = fields.Split(',');

            //Assign parameter value
            int M_InventoryRevaluation_ID = Util.GetValueOfInt(paramValue[0].ToString());
            //End Assign parameter

            DataSet ds = DB.ExecuteDataset($@"SELECT c.CostingPrecision, c.StdPrecision, ir.RevaluationType FROM M_InventoryRevaluation ir 
                         INNER JOIN C_Currency c ON (c.C_Currency_ID = ir.C_Currency_ID)
                         WHERE ir.M_InventoryRevaluation_ID = {M_InventoryRevaluation_ID}");
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result["CostingPrecision"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["CostingPrecision"]);
                result["StdPrecision"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["StdPrecision"]);
                result["RevaluationType"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["RevaluationType"]);
            }
            else
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// Get Currency From Accounting Schema
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="fields">Accounting Schema ID</param>
        /// <returns>Currency ID</returns>
        public int GetAccountingSchemaCurrency(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');

            //Assign parameter value
            int C_AcctSchema_ID = Util.GetValueOfInt(paramValue[0].ToString());
            //End Assign parameter

            return MAcctSchema.Get(ctx, C_AcctSchema_ID).GetC_Currency_ID();
        }
    }
}