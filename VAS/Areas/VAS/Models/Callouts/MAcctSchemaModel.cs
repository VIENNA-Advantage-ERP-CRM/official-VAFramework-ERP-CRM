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
            int VAB_AccountBook_ID;
            Dictionary<String, String> retDic = new Dictionary<string, string>();
            //Assign parameter value
            VAB_AccountBook_ID = Util.GetValueOfInt(paramValue[0].ToString());
            //End Assign parameter value
            MAcctSchema aas = MAcctSchema.Get(ctx, VAB_AccountBook_ID);
            retDic["StdPrecision"] = aas.GetStdPrecision().ToString();
            retDic["VAB_Currency_ID"] = aas.GetVAB_Currency_ID().ToString();
            return retDic;
              
        }
    }
}