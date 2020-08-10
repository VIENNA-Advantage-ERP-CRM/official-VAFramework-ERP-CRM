using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MChargeModel
    {
        /// <summary>
        ///Get charge Amount
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Decimal GetCharge(Ctx ctx, string fields)
        {              
            string[] paramValue = fields.Split(',');
            int Cid;
            //Assign parameter value
            Cid = Util.GetValueOfInt(paramValue[0].ToString());
            //End Assign parameter
            X_C_Charge charge = new X_C_Charge(ctx, Cid, null);
            return charge.GetChargeAmt();                  
        }
    
    }
}