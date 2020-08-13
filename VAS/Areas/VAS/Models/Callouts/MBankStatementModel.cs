using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.DBase;

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


        /// <summary>
        /// Get Payment Detail
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Decimal GetPayment(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            //Assign parameter value
            int c_payment_ID = Util.GetValueOfInt(paramValue[0]);
            int CurTo_ID = Util.GetValueOfInt(paramValue[1]);
            //DateTime? convDate = Util.GetValueOfDateTime(paramValue[2]);
            Decimal rate = 0;
            //End Assign parameter value

            string qry = "SELECT PayAmt, C_Currency_ID, C_ConversionType_ID, DateAcct FROM C_Payment_v WHERE C_Payment_ID=" + c_payment_ID;
            DataSet ds = DB.ExecuteDataset(qry, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                decimal payAmt = Util.GetValueOfDecimal(ds.Tables[0].Rows[0][0]);
                int c_currency_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0][1]);
                int c_conversionType_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0][2]);
                DateTime? dateAcct = Util.GetValueOfDateTime(ds.Tables[0].Rows[0][3]);          // JID_0333: Currency conversion should be based on Payment Account Date and Currency type
                rate = MConversionRate.Convert(ctx, payAmt, c_currency_ID, CurTo_ID, dateAcct, c_conversionType_ID, ctx.GetAD_Client_ID(), ctx.GetAD_Org_ID());
            }
            return rate;
        }


        /// <summary>
        /// Get Payment Detail
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="fields">String Fields</param>
        /// <returns>Decimal Converted Amount</returns>
        public Decimal GetConvertedAmt(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            //Assign parameter value
            int c_payment_ID = Util.GetValueOfInt(paramValue[0]);
            int CurTo_ID = Util.GetValueOfInt(paramValue[1]);
            DateTime? convDate = Util.GetValueOfDateTime(paramValue[2]);
            Decimal rate = 0;
            //End Assign parameter value

            string qry = "SELECT PayAmt, C_Currency_ID, C_ConversionType_ID FROM C_Payment_v WHERE C_Payment_ID=" + c_payment_ID;
            DataSet ds = DB.ExecuteDataset(qry, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                decimal payAmt = Util.GetValueOfDecimal(ds.Tables[0].Rows[0][0]);
                int c_currency_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0][1]);
                int c_conversionType_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0][2]);
                rate = MConversionRate.Convert(ctx, payAmt, c_currency_ID, CurTo_ID, convDate, c_conversionType_ID, ctx.GetAD_Client_ID(), ctx.GetAD_Org_ID());
            }
            return rate;
        }
    }
}