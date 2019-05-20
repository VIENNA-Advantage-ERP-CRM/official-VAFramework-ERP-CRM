using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MBankStatementModel
    {
        /// <summary>
        /// Get CurrentBalance
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Decimal GetCurrentBalance(Ctx ctx, string fields)
        { 
            string[] paramValue = fields.Split(',');          
            //Assign parameter value
            int C_BankAccount_ID = Util.GetValueOfInt(paramValue[0].ToString());
            //End Assign parameter value
           MBankAccount ba = MBankAccount.Get(ctx, C_BankAccount_ID);
           return ba.GetCurrentBalance();             
          
        }
    }
}