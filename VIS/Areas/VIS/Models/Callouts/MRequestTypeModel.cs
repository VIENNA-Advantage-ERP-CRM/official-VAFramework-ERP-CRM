using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MRequestTypeModel
    {
        /// <summary>
        /// GetDefault R_Status_ID
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public int GetDefaultR_Status_ID(Ctx ctx,string fields)
        {            
            string[] paramValue = fields.Split(',');
            int R_RequestType_ID;

            //Assign parameter value
            R_RequestType_ID = Util.GetValueOfInt(paramValue[0].ToString());
            //End Assign parameter value

            MRequestType rt = MRequestType.Get(ctx, R_RequestType_ID);
            int R_Status_ID = rt.GetDefaultR_Status_ID();
            return R_Status_ID;
        }
     
    }
}