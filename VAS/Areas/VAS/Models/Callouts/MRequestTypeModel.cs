using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MVARRequestTypeModel
    {
        /// <summary>
        /// GetDefault VAR_Req_Status_ID
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public int GetDefaultVAR_Req_Status_ID(Ctx ctx,string fields)
        {            
            string[] paramValue = fields.Split(',');
            int VAR_Req_Type_ID;

            //Assign parameter value
            VAR_Req_Type_ID = Util.GetValueOfInt(paramValue[0].ToString());
            //End Assign parameter value

            MVARRequestType rt = MVARRequestType.Get(ctx, VAR_Req_Type_ID);
            int VAR_Req_Status_ID = rt.GetDefaultVAR_Req_Status_ID();
            return VAR_Req_Status_ID;
        }
     
    }
}