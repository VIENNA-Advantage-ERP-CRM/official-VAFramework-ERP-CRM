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
            int C_CashBook_ID = Util.GetValueOfInt(paramValue[0].ToString());
            //End Assign parameter value
            MCashBook cBook = new MCashBook(ctx, C_CashBook_ID, null);
            Dictionary<string, string> result = new Dictionary<string, string>();
            result["C_Currency_ID"] = cBook.GetC_Currency_ID().ToString();
            return result;
        }

        //Get Cash Journal Detail Added by Bharat 16/12/2016
        public Dictionary<string, string> GetCashJournal(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            //Assign parameter value
            int C_CashJournal_ID = Util.GetValueOfInt(paramValue[0].ToString());
            //End Assign parameter value
            MCash cash = new MCash(ctx, C_CashJournal_ID, null);
            Dictionary<string, string> result = new Dictionary<string, string>();
            result["C_Currency_ID"] = cash.GetC_Currency_ID().ToString();
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
            decimal TotalAmt = Util.GetValueOfDecimal(DB.ExecuteScalar(" select sum(nvl(CompletedBalance,0)) + sum(nvl(runningbalance,0)) as TotalBal from c_cashbook where c_cashbook_id=" + Cashbook_ID));
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
            decimal TaxRate = Util.GetValueOfDecimal(DB.ExecuteScalar("Select Rate From C_Tax Where C_Tax_ID=" + Tax_ID + " AND IsActive='Y'"));
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
                decimal amt = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT CASHLINE.Amount FROM C_CashLine CASHLINE INNER JOIN C_Cashbook CB ON (CB.C_Cashbook_ID=CASHLINE.C_Cashbook_ID) WHERE CASHLINE.C_CashLine_ID =" + Util.GetValueOfInt(Data[0])));
                int currTo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_Currency_ID FROM C_CashBook WHERE C_CashBook_ID=(SELECT C_CashBook_ID FROM C_Cash WHERE C_Cash_ID=" + Util.GetValueOfInt(Data[1]) + ")"));
                int CurrFrom = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_Currency_ID FROM C_Cashbook WHERE C_Cashbook_ID= (SELECT C_CashBook_ID FROM C_Cash WHERE C_Cash_ID=(SELECT C_Cash_ID FROM C_CashLine WHERE C_CashLine_ID= " + Util.GetValueOfInt(Data[0]) + "))"));

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
        /// <param name="C_Invoice_ID"></param>
        /// <returns></returns>
        public decimal GetPaySheduleAmount(string fields)
        {
            string _sql = "SELECT sum(ips.DueAmt)  FROM C_Invoice i INNER JOIN C_InvoicePaySchedule ips ON (i.C_Invoice_ID=ips.C_Invoice_ID) WHERE i.IsPayScheduleValid='Y' AND ips.IsValid ='Y' AND ips.isactive ='Y'" +
                    "AND i.C_Invoice_ID = " + Util.GetValueOfInt(fields) + " AND C_InvoicePaySchedule_ID NOT IN (SELECT NVL(C_InvoicePaySchedule_ID,0) FROM C_InvoicePaySchedule WHERE C_Payment_ID IN " +
                    "(SELECT NVL(C_Payment_ID,0) FROM C_InvoicePaySchedule) UNION SELECT NVL(C_InvoicePaySchedule_ID,0) FROM C_InvoicePaySchedule WHERE C_Cashline_ID IN (SELECT NVL(C_Cashline_ID,0) FROM C_InvoicePaySchedule))";
            return Util.GetValueOfDecimal(DB.ExecuteScalar(_sql, null, null));
        }

        /// <summary>
        /// Get Invoice Pay Schedule Data
        /// </summary>        
        /// <param name="C_InvoicePaySchedule_ID"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetPaySheduleData(string fields)
        {
            string[] paramValue = fields.Split(',');
            Dictionary<string, object> retValue = new Dictionary<string, object>();
            DataSet _ds = DB.ExecuteDataset(@"SELECT NVL(ips.DueAmt,0) - NVL(ips.va009_paidamntinvce , 0) AS DueAmt, ips.DiscountDate , ips.DiscountAmt , ips.DiscountDays2 , ips.Discount2, i.IsReturnTrx 
                FROM C_InvoicePaySchedule ips INNER JOIN C_Invoice i ON ips.C_Invoice_ID = i.C_Invoice_ID WHERE ips.C_InvoicePaySchedule_ID=" + Util.GetValueOfInt(paramValue[0]), null, null);
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
                
                retValue["accountDate"] = Util.GetValueOfDateTime(DB.ExecuteScalar("SELECT DateAcct FROM C_Cash WHERE IsActive = 'Y' AND C_Cash_ID = " + Util.GetValueOfInt(paramValue[1])));                
                retValue["isSoTrx"] = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsSoTrx FROM C_Invoice WHERE C_Invoice_ID = " + Util.GetValueOfInt(paramValue[2])));
            }
            catch (Exception e)
            {

            }

            return retValue;
        }

        /// <summary>
        /// Get Invoice Pay Schedule Due Amount
        /// </summary>        
        /// <param name="C_InvoicePaySchedule_ID"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetInvSchedDueAmount(string fields)
        {
            Dictionary<string, object> retValue = null;
            string qry = @"SELECT NVL(DueAmt , 0) AS DueAmt, i.IsReturnTrx FROM C_InvoicePaySchedule ips INNER JOIN C_Invoice i ON ips.C_Invoice_ID = i.C_Invoice_ID"
                + " INNER JOIN C_DocType d ON i.C_DocTypeTarget_ID = d.C_DocType_ID WHERE ips.C_InvoicePaySchedule_ID = " + Util.GetValueOfInt(fields);
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
            int Currency_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_Currency_ID FROM C_BankAccount WHERE C_BankAccount_ID = " + Util.GetValueOfInt(fields)));
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