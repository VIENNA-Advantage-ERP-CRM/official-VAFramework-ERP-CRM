using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MCurrencyModel
    {
        /// <summary>
        /// Get Currency Detail
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetCurrency(Ctx ctx, string fields)
        {         
            string[] paramValue = fields.Split(',');                
            //Assign parameter value
            int C_Currency_ID = Util.GetValueOfInt(paramValue[0].ToString());
            //End Assign parameter
            MCurrency currency = MCurrency.Get(ctx, C_Currency_ID);
            Dictionary<string, string> result = new Dictionary<string, string>();
            result["StdPrecision"] = currency.GetStdPrecision().ToString();
            return result;
        }
    }
}