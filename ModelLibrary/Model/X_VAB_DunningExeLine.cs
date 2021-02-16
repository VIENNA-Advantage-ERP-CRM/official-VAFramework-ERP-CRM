/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVABDunningExeLine
 * Purpose        : Dunning Run Line Model
 * Class Used     : X_VAB_DunningExeLine
 * Chronological    Development
 * Raghunandan     10-Nov-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVABDunningExeLine : X_VAB_DunningExeLine
    {
        #region   
        private MVABDunningExeEntry _parent = null;
        private MInvoice _invoice = null;
        private MVABPayment _payment = null;
        private int _VAB_CurrencyFrom_ID = 0;
        private int _VAB_CurrencyTo_ID = 0;

        #endregion

        /// <summary>
        /// Standarc Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAB_DunningExeLine_ID"></param>
        /// <param name="trxName"></param>
        public MVABDunningExeLine(Ctx ctx, int VAB_DunningExeLine_ID, Trx trxName)
            : base(ctx, VAB_DunningExeLine_ID, trxName)
        {
            if (VAB_DunningExeLine_ID == 0)
            {
                SetAmt(Env.ZERO);
                SetOpenAmt(Env.ZERO);
                SetConvertedAmt(Env.ZERO);
                SetFeeAmt(Env.ZERO);
                SetInterestAmt(Env.ZERO);
                SetTotalAmt(Env.ZERO);
                SetDaysDue(0);
                SetTimesDunned(0);
                SetIsInDispute(false);
                SetProcessed(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MVABDunningExeLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        public MVABDunningExeLine(MVABDunningExeEntry parent)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetVAB_DunningExeEntry_ID(parent.GetVAB_DunningExeEntry_ID());
            //
            _parent = parent;
            _VAB_CurrencyTo_ID = parent.GetVAB_Currency_ID();
        }

        /// <summary>
        /// Get Parent 
        /// </summary>
        /// <returns>parent</returns>
        public MVABDunningExeEntry GetParent()
        {
            if (_parent == null)
            {
                _parent = new MVABDunningExeEntry(GetCtx(), GetVAB_DunningExeEntry_ID(), Get_TrxName());
            }
            return _parent;
        }

        /// <summary>
        /// Get Invoice
        /// </summary>
        /// <returns>Returns the invoice.</returns>
        public MInvoice GetInvoice()
        {
            if (GetVAB_Invoice_ID() == 0)
            {
                _invoice = null;
            }
            else if (_invoice == null)
            {
                _invoice = new MInvoice(GetCtx(), GetVAB_Invoice_ID(), Get_TrxName());
            }
            return _invoice;
        }

        /// <summary>
        /// Set Invoice
        /// </summary>
        /// <param name="invoice">The invoice to set.</param>
        public void SetInvoice(MInvoice invoice)
        {
            _invoice = invoice;
            if (invoice != null)
            {
                _VAB_CurrencyFrom_ID = invoice.GetVAB_Currency_ID();
                SetAmt(invoice.GetGrandTotal());
                SetOpenAmt(GetAmt());	//	not correct
                SetConvertedAmt(MVABExchangeRate.Convert(GetCtx(), GetOpenAmt(),
                    GetVAB_CurrencyFrom_ID(), GetVAB_CurrencyTo_ID(), GetVAF_Client_ID(), GetVAF_Org_ID()));
            }
            else
            {
                _VAB_CurrencyFrom_ID = 0;
                SetAmt(Env.ZERO);
                SetOpenAmt(Env.ZERO);
                SetConvertedAmt(Env.ZERO);
            }
        }

        /// <summary>
        /// Set Invoice
        /// </summary>
        /// <param name="VAB_Invoice_ID">invoice</param>
        /// <param name="VAB_Currency_ID">currency</param>
        /// <param name="grandTotal"> total</param>
        /// <param name="open"> open amount</param>
        /// <param name="feeAmount">days due</param>
        /// <param name="daysDue">in dispute</param>
        /// <param name="isInDispute">bp</param>
        /// <param name="timesDunned">number of dunnings</param>
        /// <param name="daysAfterLast">not used</param>
        public void SetInvoice(int VAB_Invoice_ID, int VAB_Currency_ID,
            Decimal grandTotal, Decimal open,
            Decimal feeAmount,
            int daysDue, bool isInDispute,
            int timesDunned, int daysAfterLast)
        {
            SetVAB_Invoice_ID(VAB_Invoice_ID);
            _VAB_CurrencyFrom_ID = VAB_Currency_ID;
            SetAmt(grandTotal);
            SetOpenAmt(open);
            SetFeeAmt(feeAmount);
            SetConvertedAmt(MVABExchangeRate.Convert(GetCtx(), GetOpenAmt(),
                VAB_Currency_ID, GetVAB_CurrencyTo_ID(), GetVAF_Client_ID(), GetVAF_Org_ID()));
            SetIsInDispute(isInDispute);
            SetDaysDue(daysDue);
            SetTimesDunned(timesDunned);
        }

        /// <summary>
        /// Set Fee
        /// </summary>
        /// <param name="VAB_Currency_ID"></param>
        /// <param name="feeAmount"></param>
        public void SetFee(int VAB_Currency_ID, Decimal feeAmount)
        {
            _VAB_CurrencyFrom_ID = VAB_Currency_ID;
            SetAmt(feeAmount);
            SetOpenAmt(feeAmount);
            SetFeeAmt(feeAmount);
            SetConvertedAmt(MVABExchangeRate.Convert(GetCtx(), GetOpenAmt(),
                VAB_Currency_ID, GetVAB_CurrencyTo_ID(), GetVAF_Client_ID(), GetVAF_Org_ID()));
        }

        /// <summary>
        /// Get Payment
        /// </summary>
        /// <returns>Returns the payment.</returns>
        public MVABPayment GetPayment()
        {
            if (GetVAB_Payment_ID() == 0)
            {
                _payment = null;
            }
            else if (_payment == null)
            {
                _payment = new MVABPayment(GetCtx(), GetVAB_Payment_ID(), Get_TrxName());
            }
            return _payment;
        }

        /// <summary>
        /// Set Payment
        /// </summary>
        /// <param name="payment"></param>
        public void SetPayment(MVABPayment payment)
        {
            _payment = payment;
            if (payment != null)
            {
                _VAB_CurrencyFrom_ID = payment.GetVAB_Currency_ID();
                SetAmt(payment.GetPayAmt());	//	need to reverse
                SetOpenAmt(GetAmt());	//	not correct
                SetConvertedAmt(MVABExchangeRate.Convert(GetCtx(), GetOpenAmt(),
                    GetVAB_CurrencyFrom_ID(), GetVAB_CurrencyTo_ID(), GetVAF_Client_ID(), GetVAF_Org_ID()));
            }
            else
            {
                _VAB_CurrencyFrom_ID = 0;
                SetAmt(Env.ZERO);
                SetConvertedAmt(Env.ZERO);
            }
        }

        /// <summary>
        /// Set Payment
        /// </summary>
        /// <param name="VAB_Payment_ID">payment</param>
        /// <param name="VAB_Currency_ID">currency</param>
        /// <param name="payAmt">amount</param>
        /// <param name="openAmt">open</param>
        public void SetPayment(int VAB_Payment_ID, int VAB_Currency_ID,
            Decimal payAmt, Decimal openAmt)
        {
            SetPayment(VAB_Payment_ID, VAB_Currency_ID, payAmt, openAmt, 0);
        }

        /// <summary>
        /// set payment from dunningRunCreate process
        /// </summary>
        /// <param name="VAB_Payment_ID">payment</param>
        /// <param name="VAB_Currency_ID">currency</param>
        /// <param name="payAmt">amount</param>
        /// <param name="openAmt">openamont</param>
        /// <param name="VA027_PostDatedCheck_ID">PostDatedCheque</param>
        public void SetPayment(int VAB_Payment_ID, int VAB_Currency_ID,
            Decimal payAmt, Decimal openAmt, int VA027_PostDatedCheck_ID)
        {
            SetVAB_Payment_ID(VAB_Payment_ID);
            _VAB_CurrencyFrom_ID = VAB_Currency_ID;
            SetAmt(payAmt);
            SetOpenAmt(openAmt);
            SetConvertedAmt(MVABExchangeRate.Convert(GetCtx(), GetOpenAmt(),
                VAB_Currency_ID, GetVAB_CurrencyTo_ID(), GetVAF_Client_ID(), GetVAF_Org_ID()));
            if (Env.IsModuleInstalled("VA027_") && VA027_PostDatedCheck_ID > 0)
            {
                Set_Value("VA027_PostDatedCheck_ID", VA027_PostDatedCheck_ID);
            }
        }


        /// <summary>
        /// Set CashJournalLine
        /// </summary>
        /// <param name="VAB_CashJRNLLine_ID">cash Jurnal Line</param>
        /// <param name="VAB_Currency_ID">currency</param>
        /// <param name="payAmt">amount</param>
        /// <param name="openAmt">openAmount</param>
        public void SetcashLine(int VAB_CashJRNLLine_ID,
            Decimal payAmt, Decimal openAmt, int VAB_Currency_ID)
        {
            Set_Value("VAB_CashJRNLLine_ID", VAB_CashJRNLLine_ID);
            _VAB_CurrencyFrom_ID = VAB_Currency_ID;
            SetAmt(payAmt);
            SetOpenAmt(openAmt);
            SetConvertedAmt(MVABExchangeRate.Convert(GetCtx(), GetOpenAmt(),
           VAB_Currency_ID, GetVAB_CurrencyTo_ID(), GetVAF_Client_ID(), GetVAF_Org_ID()));
        }
        /// <summary>
        /// Set GLJournalLine 
        /// </summary>
        /// <param name="VAGL_JRNLLine_ID">cash Jurnal Line</param>
        /// <param name="VAB_Currency_ID">currency</param>
        /// <param name="Amtount">amount</param>
        /// <param name="openAmt">openAmount</param>
        public void SetJournalLine(int VAGL_JRNLLine_ID, int VAB_Currency_ID, Decimal Amount, Decimal OpenAmt)
        {
            Set_Value("VAGL_JRNLLine_ID", VAGL_JRNLLine_ID);
            _VAB_CurrencyFrom_ID = VAB_Currency_ID;
            SetAmt(Amount);
            SetOpenAmt(Amount);
            SetConvertedAmt(MVABExchangeRate.Convert(GetCtx(), GetOpenAmt(),
           VAB_Currency_ID, GetVAB_CurrencyTo_ID(), GetVAF_Client_ID(), GetVAF_Org_ID()));
        }
        /// <summary>
        /// set PostDatedCheque
        /// </summary>
        /// <param name="VA027_PostDatedCheck_ID">PostDatedCheque</param>
        /// <param name="VAB_Currency_ID">Currency</param>
        /// <param name="Amount">Amount</param>
        /// <param name="openAmt">openAmount</param>
        public void SetPostDatedCheque(int VA027_PostDatedCheck_ID, int VAB_Currency_ID, Decimal Amount)
        {
            Set_Value("VA027_PostDatedCheck_ID", VA027_PostDatedCheck_ID);
            _VAB_CurrencyFrom_ID = VAB_Currency_ID;
            SetAmt(Amount);
            //openAmount same as Amount in PDC  whose payment is not generated
            SetOpenAmt(Amount);
            SetConvertedAmt(MVABExchangeRate.Convert(GetCtx(), GetOpenAmt(),
            VAB_Currency_ID, GetVAB_CurrencyTo_ID(), GetVAF_Client_ID(), GetVAF_Org_ID()));
        }

        /// <summary>
        /// Get Currency From (Invoice/Payment)
        /// </summary>
        /// <returns>Returns the Currency From</returns>
        public int GetVAB_CurrencyFrom_ID()
        {
            if (_VAB_CurrencyFrom_ID == 0)
            {
                if (GetVAB_Invoice_ID() != 0)
                {
                    _VAB_CurrencyFrom_ID = GetInvoice().GetVAB_Currency_ID();
                }
                else if (GetVAB_Payment_ID() != 0)
                {
                    _VAB_CurrencyFrom_ID = GetPayment().GetVAB_Currency_ID();
                }
            }
            return _VAB_CurrencyFrom_ID;
        }

        /// <summary>
        /// Get Currency To from Parent
        /// </summary>
        /// <returns>Returns the Currency To</returns>
        public int GetVAB_CurrencyTo_ID()
        {
            if (_VAB_CurrencyTo_ID == 0)
            {
                _VAB_CurrencyTo_ID = GetParent().GetVAB_Currency_ID();
            }
            return _VAB_CurrencyTo_ID;
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns></returns>
        protected override bool BeforeSave(bool newRecord)
        {
            //	Set Amt
            if (GetVAB_Invoice_ID() == 0 && GetVAB_Payment_ID() == 0 && Util.GetValueOfInt(Get_Value("VAB_CashJRNLLine_ID")) == 0 && Util.GetValueOfInt(Get_Value("VAGL_JRNLLine_ID")) == 0 && (!Env.IsModuleInstalled("VA027_") || (Get_ColumnIndex("VA027_PostDatedCheck_ID") > 0 && Util.GetValueOfInt(Get_Value("VA027_PostDatedCheck_ID")) == 0)))
            {
                SetAmt(Env.ZERO);
                SetOpenAmt(Env.ZERO);
            }
            //	Converted Amt
            if (Env.ZERO.CompareTo(GetOpenAmt()) == 0)
            {
                SetConvertedAmt(Env.ZERO);
            }
            else if (Env.ZERO.CompareTo(GetConvertedAmt()) == 0)
            {
                SetConvertedAmt(MVABExchangeRate.Convert(GetCtx(), GetOpenAmt(),
                    GetVAB_CurrencyFrom_ID(), GetVAB_CurrencyTo_ID(), GetVAF_Client_ID(), GetVAF_Org_ID()));
            }
            //	Total
            SetTotalAmt(Decimal.Add(Decimal.Add(GetConvertedAmt(), GetFeeAmt()), GetInterestAmt()));
            // Reset Collection Status only if null
            if (GetInvoice() != null && GetInvoice().GetInvoiceCollectionType() == null)
            {
                if (_invoice != null)
                {
                    _invoice.SetInvoiceCollectionType(X_VAB_Invoice.INVOICECOLLECTIONTYPE_Dunning);
                    _invoice.Save();
                }
            }
            //
            return true;
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord"></param>
        /// <param name="success"></param>
        /// <returns>Success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            UpdateEntry();
            return success;
        }

        /// <summary>
        /// After Delete
        /// </summary>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterDelete(bool success)
        {
            UpdateEntry();
            return success;
        }

        /// <summary>
        /// Update Entry.
        /// Calculate/update Amt/Qty
        /// </summary>
        private void UpdateEntry()
        {
            // we do not count the fee line as an item, but it sum it up.
            String sql = "UPDATE VAB_DunningExeEntry e "
                + "SET Amt=(SELECT SUM(ConvertedAmt)+SUM(FeeAmt)"
                + " FROM VAB_DunningExeLine l "
                    + "WHERE e.VAB_DunningExeEntry_ID=l.VAB_DunningExeEntry_ID), "
                + "QTY=(SELECT COUNT(*)"
                + " FROM VAB_DunningExeLine l "
                    + "WHERE e.VAB_DunningExeEntry_ID=l.VAB_DunningExeEntry_ID )"
                //+ " AND (NOT VAB_Invoice_ID IS NULL OR NOT VAB_Payment_ID IS NULL))"
                + " WHERE VAB_DunningExeEntry_ID=" + GetVAB_DunningExeEntry_ID();

            DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
        }
    }
}
