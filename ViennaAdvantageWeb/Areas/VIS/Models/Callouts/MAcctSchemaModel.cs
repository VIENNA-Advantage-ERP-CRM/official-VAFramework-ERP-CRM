using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MAcctSchemaModel
    {
        /// <summary>
        /// GetAcctSchema Detail
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<String, String> GetAcctSchema(Ctx ctx, string fields)
        {            
            string[] paramValue = fields.Split(',');
            int C_AcctSchema_ID;
            Dictionary<String, String> retDic = new Dictionary<string, string>();
            //Assign parameter value
            C_AcctSchema_ID = Util.GetValueOfInt(paramValue[0].ToString());
            //End Assign parameter value
            MAcctSchema aas = MAcctSchema.Get(ctx, C_AcctSchema_ID);
            retDic["StdPrecision"] = aas.GetStdPrecision().ToString();
            retDic["C_Currency_ID"] = aas.GetC_Currency_ID().ToString();
            return retDic;
              
        }
    }
}