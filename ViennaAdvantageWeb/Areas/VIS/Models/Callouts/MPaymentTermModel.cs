using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MPaymentTermModel
    {
        /// <summary>
        /// GetPaymentTerm
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetPaymentTerm(Ctx ctx,string fields)
        {
            string[] paramValue = fields.Split(',');
            int C_PaymentTerm_ID;
            int C_Invoice_ID;

            //Assign parameter value
            C_PaymentTerm_ID = Util.GetValueOfInt(paramValue[0].ToString());
            C_Invoice_ID = Util.GetValueOfInt(paramValue[1].ToString());
            //End Assign parameter value

            MPaymentTerm pt = new MPaymentTerm(ctx, C_PaymentTerm_ID, null);
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            retVal["Apply"] = pt.Apply(C_Invoice_ID).ToString();
            retVal["Get_ID"] = pt.Get_ID().ToString();
            return retVal;
                
        }

    }
}