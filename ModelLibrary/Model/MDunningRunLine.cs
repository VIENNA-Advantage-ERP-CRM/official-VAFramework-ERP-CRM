/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MDunningRunLine
 * Purpose        : Dunning Run Line Model
 * Class Used     : X_C_DunningRunLine
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
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MDunningRunLine : X_C_DunningRunLine
    {
        #region   
        private MDunningRunEntry _parent = null;
        private MInvoice _invoice = null;
        private MPayment _payment = null;
        private int _C_CurrencyFrom_ID = 0;
        private int _C_CurrencyTo_ID = 0;

        #endregion

        /// <summary>
        /// Standarc Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="C_DunningRunLine_ID"></param>
        /// <param name="trxName"></param>
        public MDunningRunLine(Ctx ctx, int C_DunningRunLine_ID, Trx trxName)
            : base(ctx, C_DunningRunLine_ID, trxName)
        {
            if (C_DunningRunLine_ID == 0)
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
        public MDunningRunLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        public MDunningRunLine(MDunningRunEntry parent)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetC_DunningRunEntry_ID(parent.GetC_DunningRunEntry_ID());
            //
            _parent = parent;
            _C_CurrencyTo_ID = parent.GetC_Currency_ID();
        }

        /// <summary>
        /// Get Parent 
        /// </summary>
        /// <returns>parent</returns>
        public MDunningRunEntry GetParent()
        {
            if (_parent == null)
            {
                _parent = new MDunningRunEntry(GetCtx(), GetC_DunningRunEntry_ID(), Get_TrxName());
            }
            return _parent;
        }

        /// <summary>
        /// Get Invoice
        /// </summary>
        /// <returns>Returns the invoice.</returns>
        public MInvoice GetInvoice()
        {
            if (GetC_Invoice_ID() == 0)
            {
                _invoice = null;
            }
            else if (_invoice == null)
            {
                _invoice = new MInvoice(GetCtx(), GetC_Invoice_ID(), Get_TrxName());
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
                _C_CurrencyFrom_ID = invoice.GetC_Currency_ID();
                SetAmt(invoice.GetGrandTotal());
                SetOpenAmt(GetAmt());	//	not correct
                SetConvertedAmt(MConversionRate.Convert(GetCtx(), GetOpenAmt(),
                    GetC_CurrencyFrom_ID(), GetC_CurrencyTo_ID(), GetAD_Client_ID(), GetAD_Org_ID()));
            }
            else
            {
                _C_CurrencyFrom_ID = 0;
                SetAmt(Env.ZERO);
                SetOpenAmt(Env.ZERO);
                SetConvertedAmt(Env.ZERO);
            }
        }

        /// <summary>
        /// Set Invoice
        /// </summary>
        /// <param name="C_Invoice_ID">invoice</param>
        /// <param name="C_Currency_ID">currency</param>
        /// <param name="grandTotal"> total</param>
        /// <param name="open"> open amount</param>
        /// <param name="feeAmount">days due</param>
        /// <param name="daysDue">in dispute</param>
        /// <param name="isInDispute">bp</param>
        /// <param name="timesDunned">number of dunnings</param>
        /// <param name="daysAfterLast">not used</param>
        public void SetInvoice(int C_Invoice_ID, int C_Currency_ID,
            Decimal grandTotal, Decimal open,
            Decimal feeAmount,
            int daysDue, bool isInDispute,
            int timesDunned, int daysAfterLast)
        {
            SetC_Invoice_ID(C_Invoice_ID);
            _C_CurrencyFrom_ID = C_Currency_ID;
            SetAmt(grandTotal);
            SetOpenAmt(open);
            SetFeeAmt(feeAmount);
            SetConvertedAmt(MConversionRate.Convert(GetCtx(), GetOpenAmt(),
                C_Currency_ID, GetC_CurrencyTo_ID(), GetAD_Client_ID(), GetAD_Org_ID()));
            SetIsInDispute(isInDispute);
            SetDaysDue(daysDue);
            SetTimesDunned(timesDunned);
        }

        /// <summary>
        /// Set Fee
        /// </summary>
        /// <param name="C_Currency_ID"></param>
        /// <param name="feeAmount"></param>
        public void SetFee(int C_Currency_ID, Decimal feeAmount)
        {
            _C_CurrencyFrom_ID = C_Currency_ID;
            SetAmt(feeAmount);
            SetOpenAmt(feeAmount);
            SetFeeAmt(feeAmount);
            SetConvertedAmt(MConversionRate.Convert(GetCtx(), GetOpenAmt(),
                C_Currency_ID, GetC_CurrencyTo_ID(), GetAD_Client_ID(), GetAD_Org_ID()));
        }

        /// <summary>
        /// Get Payment
        /// </summary>
        /// <returns>Returns the payment.</returns>
        public MPayment GetPayment()
        {
            if (GetC_Payment_ID() == 0)
            {
                _payment = null;
            }
            else if (_payment == null)
            {
                _payment = new MPayment(GetCtx(), GetC_Payment_ID(), Get_TrxName());
            }
            return _payment;
        }

        /// <summary>
        /// Set Payment
        /// </summary>
        /// <param name="payment"></param>
        public void SetPayment(MPayment payment)
        {
            _payment = payment;
            if (payment != null)
            {
                _C_CurrencyFrom_ID = payment.GetC_Currency_ID();
                SetAmt(payment.GetPayAmt());	//	need to reverse
                SetOpenAmt(GetAmt());	//	not correct
                SetConvertedAmt(MConversionRate.Convert(GetCtx(), GetOpenAmt(),
                    GetC_CurrencyFrom_ID(), GetC_CurrencyTo_ID(), GetAD_Client_ID(), GetAD_Org_ID()));
            }
            else
            {
                _C_CurrencyFrom_ID = 0;
                SetAmt(Env.ZERO);
                SetConvertedAmt(Env.ZERO);
            }
        }

        /// <summary>
        /// Set Payment
        /// </summary>
        /// <param name="C_Payment_ID">payment</param>
        /// <param name="C_Currency_ID">currency</param>
        /// <param name="payAmt">amount</param>
        /// <param name="openAmt">open</param>
        public void SetPayment(int C_Payment_ID, int C_Currency_ID,
            Decimal payAmt, Decimal openAmt)
        {
            SetC_Payment_ID(C_Payment_ID);
            _C_CurrencyFrom_ID = C_Currency_ID;
            SetAmt(payAmt);
            SetOpenAmt(openAmt);
            SetConvertedAmt(MConversionRate.Convert(GetCtx(), GetOpenAmt(),
                C_Currency_ID, GetC_CurrencyTo_ID(), GetAD_Client_ID(), GetAD_Org_ID()));
        }

        /// <summary>
        /// Get Currency From (Invoice/Payment)
        /// </summary>
        /// <returns>Returns the Currency From</returns>
        public int GetC_CurrencyFrom_ID()
        {
            if (_C_CurrencyFrom_ID == 0)
            {
                if (GetC_Invoice_ID() != 0)
                {
                    _C_CurrencyFrom_ID = GetInvoice().GetC_Currency_ID();
                }
                else if (GetC_Payment_ID() != 0)
                {
                    _C_CurrencyFrom_ID = GetPayment().GetC_Currency_ID();
                }
            }
            return _C_CurrencyFrom_ID;
        }

        /// <summary>
        /// Get Currency To from Parent
        /// </summary>
        /// <returns>Returns the Currency To</returns>
        public int GetC_CurrencyTo_ID()
        {
            if (_C_CurrencyTo_ID == 0)
            {
                _C_CurrencyTo_ID = GetParent().GetC_Currency_ID();
            }
            return _C_CurrencyTo_ID;
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns></returns>
        protected override bool BeforeSave(bool newRecord)
        {
            //	Set Amt
            if (GetC_Invoice_ID() == 0 && GetC_Payment_ID() == 0)
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
                SetConvertedAmt(MConversionRate.Convert(GetCtx(), GetOpenAmt(),
                    GetC_CurrencyFrom_ID(), GetC_CurrencyTo_ID(), GetAD_Client_ID(), GetAD_Org_ID()));
            }
            //	Total
            SetTotalAmt(Decimal.Add(Decimal.Add(GetConvertedAmt(), GetFeeAmt()), GetInterestAmt()));
            // Reset Collection Status only if null
            if (GetInvoice() != null && GetInvoice().GetInvoiceCollectionType() == null)
            {
                if (_invoice != null)
                {
                    _invoice.SetInvoiceCollectionType(X_C_Invoice.INVOICECOLLECTIONTYPE_Dunning);
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
            String sql = "UPDATE C_DunningRunEntry e "
                + "SET Amt=(SELECT SUM(ConvertedAmt)+SUM(FeeAmt)"
                + " FROM C_DunningRunLine l "
                    + "WHERE e.C_DunningRunEntry_ID=l.C_DunningRunEntry_ID), "
                + "QTY=(SELECT COUNT(*)"
                + " FROM C_DunningRunLine l "
                    + "WHERE e.C_DunningRunEntry_ID=l.C_DunningRunEntry_ID "
                    + " AND (NOT C_Invoice_ID IS NULL OR NOT C_Payment_ID IS NULL))"
                + " WHERE C_DunningRunEntry_ID=" + GetC_DunningRunEntry_ID();

            DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
        }
    }
}
