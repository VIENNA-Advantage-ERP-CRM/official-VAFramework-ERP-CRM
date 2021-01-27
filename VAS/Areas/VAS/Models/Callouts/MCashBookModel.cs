using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MCashBookModel
    {
        /// <summary>
        /// Get CashBook Detail
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetCashBook(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            //Assign parameter value
            int VAB_CashBook_ID = Util.GetValueOfInt(paramValue[0].ToString());
            //End Assign parameter value
            MCashBook cBook = new MCashBook(ctx, VAB_CashBook_ID, null);
            Dictionary<string, string> result = new Dictionary<string, string>();
            result["VAB_Currency_ID"] = cBook.GetVAB_Currency_ID().ToString();
            return result;
        }

        //Get Cash Journal Detail Added by Bharat 16/12/2016
        public Dictionary<string, string> GetCashJournal(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            //Assign parameter value
            int VAB_CashJRNLJournal_ID = Util.GetValueOfInt(paramValue[0].ToString());
            //End Assign parameter value
            MCash cash = new MCash(ctx, VAB_CashJRNLJournal_ID, null);
            Dictionary<string, string> result = new Dictionary<string, string>();
            result["VAB_Currency_ID"] = cash.GetVAB_Currency_ID().ToString();
            result["DateAcct"] = cash.GetDateAcct().ToString();
            return result;
        }

        // Added by Mohit to remove client side queries 10/05/2017
        /// <summary>
        /// Get CashBook Beginning Balance
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="Cashbook_ID"></param>
        /// <returns></returns>
        public decimal GetBeginBalance(Ctx ctx, int Cashbook_ID)
        {
            decimal TotalAmt = Util.GetValueOfDecimal(DB.ExecuteScalar(" select sum(nvl(CompletedBalance,0)) + sum(nvl(runningbalance,0)) as TotalBal from VAB_CashBook where VAB_CashBook_id=" + Cashbook_ID));
            return TotalAmt;
        }
        /// <summary>
        /// Get Tax Rate
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="Tax_ID"></param>
        /// <returns></returns>
        public decimal GetTaxRate(Ctx ctx, int Tax_ID)
        {
            decimal TaxRate = Util.GetValueOfDecimal(DB.ExecuteScalar("Select Rate From VAB_TaxRate Where VAB_TaxRate_ID=" + Tax_ID + " AND IsActive='Y'"));
            return TaxRate;
        }

        /// <summary>
        /// Get Converted Amount
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="Tax_ID"></param>
        /// <returns></returns>
        public CashConvertedAmt GetConvertedAmt(Ctx ctx, string fields)
        {
            CashConvertedAmt retValue = new CashConvertedAmt();
            try
            {
                string[] Data = fields.Split(',');
                decimal amt = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT CASHLINE.Amount FROM VAB_CashJRNLLine CASHLINE INNER JOIN VAB_CashBook CB ON (CB.VAB_CashBook_ID=CASHLINE.VAB_CashBook_ID) WHERE CASHLINE.VAB_CashJRNLLine_ID =" + Util.GetValueOfInt(Data[0])));
                int currTo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAB_Currency_ID FROM VAB_CashBook WHERE VAB_CashBook_ID=(SELECT VAB_CashBook_ID FROM VAB_CashJRNL WHERE VAB_CashJRNL_ID=" + Util.GetValueOfInt(Data[1]) + ")"));
                int CurrFrom = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAB_Currency_ID FROM VAB_CashBook WHERE VAB_CashBook_ID= (SELECT VAB_CashBook_ID FROM VAB_CashJRNL WHERE VAB_CashJRNL_ID=(SELECT VAB_CashJRNL_ID FROM VAB_CashJRNLLine WHERE VAB_CashJRNLLine_ID= " + Util.GetValueOfInt(Data[0]) + "))"));

                retValue.CurrFrom = CurrFrom;
                retValue.CurrTo = currTo;
                retValue.Amt = amt;
            }
            catch (Exception e)
            {

            }
            return retValue;
        }

        /// <summary>
        /// Get Invoice Pay Schedule Amount
        /// </summary>        
        /// <param name="VAB_Invoice_ID"></param>
        /// <returns></returns>
        public decimal GetPaySheduleAmount(string fields)
        {
            string _sql = "SELECT sum(ips.DueAmt)  FROM VAB_Invoice i INNER JOIN VAB_sched_InvoicePayment ips ON (i.VAB_Invoice_ID=ips.VAB_Invoice_ID) WHERE i.IsPayScheduleValid='Y' AND ips.IsValid ='Y' AND ips.isactive ='Y'" +
                    "AND i.VAB_Invoice_ID = " + Util.GetValueOfInt(fields) + " AND VAB_sched_InvoicePayment_ID NOT IN (SELECT NVL(VAB_sched_InvoicePayment_ID,0) FROM VAB_sched_InvoicePayment WHERE VAB_Payment_ID IN " +
                    "(SELECT NVL(VAB_Payment_ID,0) FROM VAB_sched_InvoicePayment) UNION SELECT NVL(VAB_sched_InvoicePayment_ID,0) FROM VAB_sched_InvoicePayment WHERE VAB_CashJRNLLine_ID IN (SELECT NVL(VAB_CashJRNLLine_ID,0) FROM VAB_sched_InvoicePayment))";
            return Util.GetValueOfDecimal(DB.ExecuteScalar(_sql, null, null));
        }

        /// <summary>
        /// Get Invoice Pay Schedule Data
        /// </summary>        
        /// <param name="VAB_sched_InvoicePayment_ID"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetPaySheduleData(string fields)
        {
            string[] paramValue = fields.Split(',');
            Dictionary<string, object> retValue = new Dictionary<string, object>();
            DataSet _ds = DB.ExecuteDataset(@"SELECT NVL(ips.DueAmt,0) - NVL(ips.va009_paidamntinvce , 0) AS DueAmt, ips.DiscountDate , ips.DiscountAmt , ips.DiscountDays2 , ips.Discount2, i.IsReturnTrx 
                FROM VAB_sched_InvoicePayment ips INNER JOIN VAB_Invoice i ON ips.VAB_Invoice_ID = i.VAB_Invoice_ID WHERE ips.VAB_sched_InvoicePayment_ID=" + Util.GetValueOfInt(paramValue[0]), null, null);
            try
            {
                if (_ds.Tables[0].Rows.Count > 0 && _ds != null)
                {
                    retValue["DueAmt"] = Util.GetValueOfDecimal(_ds.Tables[0].Rows[0]["DueAmt"]);
                    retValue["DiscountDate"] = Util.GetValueOfDateTime(_ds.Tables[0].Rows[0]["DiscountDate"]);
                    retValue["DiscountAmt"] = Util.GetValueOfDecimal(_ds.Tables[0].Rows[0]["DiscountAmt"]);
                    retValue["DiscountDays2"] = Util.GetValueOfDateTime(_ds.Tables[0].Rows[0]["DiscountDate"]);
                    retValue["Discount2"] = Util.GetValueOfDecimal(_ds.Tables[0].Rows[0]["Discount2"]);
                    retValue["IsReturnTrx"] = Util.GetValueOfString(_ds.Tables[0].Rows[0]["IsReturnTrx"]);
                }
                
                retValue["accountDate"] = Util.GetValueOfDateTime(DB.ExecuteScalar("SELECT DateAcct FROM VAB_CashJRNL WHERE IsActive = 'Y' AND VAB_CashJRNL_ID = " + Util.GetValueOfInt(paramValue[1])));                
                retValue["isSoTrx"] = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsSoTrx FROM VAB_Invoice WHERE VAB_Invoice_ID = " + Util.GetValueOfInt(paramValue[2])));
            }
            catch (Exception e)
            {

            }

            return retValue;
        }

        /// <summary>
        /// Get Invoice Pay Schedule Due Amount
        /// </summary>        
        /// <param name="VAB_sched_InvoicePayment_ID"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetInvSchedDueAmount(string fields)
        {
            Dictionary<string, object> retValue = null;
            string qry = @"SELECT NVL(DueAmt , 0) AS DueAmt, i.IsReturnTrx FROM VAB_sched_InvoicePayment ips INNER JOIN VAB_Invoice i ON ips.VAB_Invoice_ID = i.VAB_Invoice_ID"
                + " INNER JOIN VAB_DocTypes d ON i.VAB_DocTypesTarget_ID = d.VAB_DocTypes_ID WHERE ips.VAB_sched_InvoicePayment_ID = " + Util.GetValueOfInt(fields);
            DataSet _ds = DB.ExecuteDataset(qry);
            if (_ds.Tables[0].Rows.Count > 0 && _ds != null)
            {
                retValue = new Dictionary<string, object>();
                retValue["DueAmt"] = Util.GetValueOfDecimal(_ds.Tables[0].Rows[0]["DueAmt"]);
                retValue["IsReturnTrx"] = Util.GetValueOfString(_ds.Tables[0].Rows[0]["IsReturnTrx"]);
            }
            return retValue;
        }

        public int GetBankAcctCurrency(string fields)
        {
            int Currency_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAB_Currency_ID FROM VAB_Bank_Acct WHERE VAB_Bank_Acct_ID = " + Util.GetValueOfInt(fields)));
            return Currency_ID;
        }
        // End Changes Mohit

    }
    public class CashConvertedAmt
    {
        public int CurrTo { get; set; }
        public int CurrFrom { get; set; }
        public decimal Amt { get; set; }

    }
}