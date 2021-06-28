using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MUOMModel
    {
        /// <summary>
        /// Get Precision
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public int GetPrecision(Ctx ctx,string fields)
        {            
            string[] paramValue = fields.ToString().Split(',');

            //Assign parameter value
            int C_UOM_To_ID = Util.GetValueOfInt(paramValue[0].ToString());
            //End Assign parameter value 

            return MUOM.GetPrecision(ctx, C_UOM_To_ID);          
        }
        
    }
}