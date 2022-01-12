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
        /// <param name="ctx">Context</param>
        /// <param name="fields">Tab fields</param>
        /// <returns>List of Payment Details</returns>
        public List<Dictionary<string, Object>> GetPayment(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            //Assign parameter value
            int c_payment_ID = Util.GetValueOfInt(paramValue[0]);
            int CurTo_ID = Util.GetValueOfInt(paramValue[1]);
            //Conversion based on Bank StatementLine Date
            DateTime? convDate = Util.GetValueOfDateTime(paramValue[2]);
            //Get the Org_ID from the StatementLine - Tab
            int ad_org_ID = Util.GetValueOfInt(paramValue[3]);
            Decimal rate = 0;
            //End Assign parameter value
            List<Dictionary<string, Object>> _paymentDetails = null;
            //required PaymentMethod Details so added PaymentMethod, CheckNo, CheckDate and TenderType
            string qry = "SELECT v.PayAmt, v.C_Currency_ID, v.C_ConversionType_ID, pay.VA009_PaymentMethod_ID, v.CheckDate, v.CheckNo, v.TenderType FROM C_Payment_V v INNER JOIN C_Payment pay ON pay.C_Payment_ID=v.C_Payment_ID WHERE v.C_Payment_ID=" + c_payment_ID;
            DataSet ds = DB.ExecuteDataset(qry, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                decimal payAmt = Util.GetValueOfDecimal(ds.Tables[0].Rows[0][0]);
                int c_currency_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0][1]);
                int c_conversionType_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0][2]);
                //PaymentMethod Details from payment window
                int paymentMethod = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VA009_PaymentMethod_ID"]);
                string checkNo = Util.GetValueOfString(ds.Tables[0].Rows[0]["CheckNo"]);
                DateTime? checkDate = Util.GetValueOfDateTime(ds.Tables[0].Rows[0]["CheckDate"]);
                string tenderType = Util.GetValueOfString(ds.Tables[0].Rows[0]["TenderType"]);
                //as per requirment should not want dateAcct value
                //DateTime? dateAcct = Util.GetValueOfDateTime(ds.Tables[0].Rows[0][3]);          // JID_0333: Currency conversion should be based on Payment Account Date and Currency type
                //rate = MConversionRate.Convert(ctx, payAmt, c_currency_ID, CurTo_ID, dateAcct, c_conversionType_ID, ctx.GetAD_Client_ID(), ctx.GetAD_Org_ID());
                // Conversion Rate should be based on StatementLine Date & StatementLine ConversionType_ID Requirement by Ranvir
                if (CurTo_ID != c_currency_ID)
                {
                    rate = MConversionRate.Convert(ctx, payAmt, c_currency_ID, CurTo_ID, convDate, c_conversionType_ID, ctx.GetAD_Client_ID(), ad_org_ID);
                }
                else
                {
                    rate = payAmt;
                }
                Dictionary<string, Object> _list = new Dictionary<string, object>();
                _list.Add("C_ConversionType_ID", c_conversionType_ID);
                //added variable values to the List
                _list.Add("VA009_PaymentMethod_ID", paymentMethod);
                _list.Add("EFTCheckNo", checkNo);
                _list.Add("EFTValutaDate", checkDate);
                _list.Add("TenderType", tenderType);
                //_list.Add("DateAcct", dateAcct);
                _list.Add("payAmt", rate);
                if (payAmt != 0 && rate == 0)
                {
                    _list.Add("ConvertedAmt", "ConversionNotFound");
                }
                else
                {
                    _list.Add("ConvertedAmt", "");
                }
                _paymentDetails = new List<Dictionary<string, object>>();
                _paymentDetails.Add(_list);
            }
            return _paymentDetails;
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