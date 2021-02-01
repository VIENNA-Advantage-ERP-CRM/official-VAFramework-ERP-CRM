/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAB_DocAllocationLine
 * Chronological Development
 * Veena Pandey     23-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Common;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVABDocAllocationLine : X_VAB_DocAllocationLine
    {
        /**	Invoice info			*/
        private MInvoice _invoice = null;
        /** Allocation Header		*/
        private MVABDocAllocation _parent = null;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_DocAllocationLine_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVABDocAllocationLine(Ctx ctx, int VAB_DocAllocationLine_ID, Trx trxName)
            : base(ctx, VAB_DocAllocationLine_ID, trxName)
        {
            if (VAB_DocAllocationLine_ID == 0)
            {
                //	setVAB_DocAllocation_ID (0);
                SetAmount(Env.ZERO);
                SetDiscountAmt(Env.ZERO);
                SetWriteOffAmt(Env.ZERO);
                SetOverUnderAmt(Env.ZERO);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MVABDocAllocationLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        public MVABDocAllocationLine(MVABDocAllocation parent)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetVAB_DocAllocation_ID(parent.GetVAB_DocAllocation_ID());
            _parent = parent;
            Set_TrxName(parent.Get_TrxName());
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        /// <param name="amount">amount</param>
        /// <param name="discountAmt">optional discount</param>
        /// <param name="writeOffAmt">optional write off</param>
        /// <param name="overUnderAmt">over/underpayment</param>
        public MVABDocAllocationLine(MVABDocAllocation parent, Decimal amount,
            Decimal? discountAmt, Decimal? writeOffAmt, Decimal? overUnderAmt)
            : this(parent)
        {
            SetAmount(amount);
            SetDiscountAmt(discountAmt == null ? Env.ZERO : (Decimal)discountAmt);
            SetWriteOffAmt(writeOffAmt == null ? Env.ZERO : (Decimal)writeOffAmt);
            SetOverUnderAmt(overUnderAmt == null ? Env.ZERO : (Decimal)overUnderAmt);
        }

        /// <summary>
        /// Get Parent
        /// </summary>
        /// <returns>parent</returns>
        public MVABDocAllocation GetParent()
        {
            if (_parent == null)
                _parent = new MVABDocAllocation(GetCtx(), GetVAB_DocAllocation_ID(), Get_TrxName());
            return _parent;
        }

        /// <summary>
        /// Set Parent
        /// </summary>
        /// <param name="parent">parent</param>
        public void SetParent(MVABDocAllocation parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// Get Parent Trx Date
        /// </summary>
        /// <returns>date trx</returns>
        public new DateTime? GetDateTrx()
        {
            return GetParent().GetDateTrx();
        }

        /// <summary>
        /// Set Document Info
        /// </summary>
        /// <param name="VAB_BusinessPartner_ID">partner</param>
        /// <param name="VAB_Order_ID">order</param>
        /// <param name="VAB_Invoice_ID">invoice</param>
        public void SetDocInfo(int VAB_BusinessPartner_ID, int VAB_Order_ID, int VAB_Invoice_ID)
        {
            SetVAB_BusinessPartner_ID(VAB_BusinessPartner_ID);
            SetVAB_Order_ID(VAB_Order_ID);
            SetVAB_Invoice_ID(VAB_Invoice_ID);
        }

        /// <summary>
        /// Set Payment Info
        /// </summary>
        /// <param name="VAB_Payment_ID">payment</param>
        /// <param name="VAB_CashJRNLLine_ID">cash line</param>
        public void SetPaymentInfo(int VAB_Payment_ID, int VAB_CashJRNLLine_ID)
        {
            if (VAB_Payment_ID != 0)
                SetVAB_Payment_ID(VAB_Payment_ID);
            if (VAB_CashJRNLLine_ID != 0)
                SetVAB_CashJRNLLine_ID(VAB_CashJRNLLine_ID);
        }

        /// <summary>
        /// Get Invoice
        /// </summary>
        /// <returns>invoice or null</returns>
        public MInvoice GetInvoice()
        {
            if (_invoice == null && GetVAB_Invoice_ID() != 0)
                _invoice = new MInvoice(GetCtx(), GetVAB_Invoice_ID(), Get_TrxName());
            return _invoice;
        }


        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true if success</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (!newRecord
                && (Is_ValueChanged("VAB_BusinessPartner_ID") || Is_ValueChanged("VAB_Invoice_ID")))
            {
                log.Severe("Cannot Change Business Partner or Invoice");
                return false;
            }

            //	Set BPartner/Order from Invoice
            if (GetVAB_BusinessPartner_ID() == 0 && GetInvoice() != null)
                SetVAB_BusinessPartner_ID(GetInvoice().GetVAB_BusinessPartner_ID());
            if (GetVAB_Order_ID() == 0 && GetInvoice() != null)
                SetVAB_Order_ID(GetInvoice().GetVAB_Order_ID());
            //
            return true;
        }


        /// <summary>
        /// Before Delete
        /// </summary>
        /// <returns>true if reversed</returns>
        protected override bool BeforeDelete()
        {
            SetIsActive(false);
            ProcessIt(true);
            return true;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MAllocationLine[");
            sb.Append(Get_ID());
            if (GetVAB_Payment_ID() != 0)
                sb.Append(",VAB_Payment_ID=").Append(GetVAB_Payment_ID());
            if (GetVAB_CashJRNLLine_ID() != 0)
                sb.Append(",VAB_CashJRNLLine_ID=").Append(GetVAB_CashJRNLLine_ID());
            if (GetVAB_Invoice_ID() != 0)
                sb.Append(",VAB_Invoice_ID=").Append(GetVAB_Invoice_ID());
            if (GetVAB_BusinessPartner_ID() != 0)
                sb.Append(",VAB_BusinessPartner_ID=").Append(GetVAB_BusinessPartner_ID());
            sb.Append(", Amount=").Append(GetAmount())
                .Append(",Discount=").Append(GetDiscountAmt())
                .Append(",WriteOff=").Append(GetWriteOffAmt())
                .Append(",OverUnder=").Append(GetOverUnderAmt());
            sb.Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Process Allocation (does not update line).
        /// - Update and Link Invoice/Payment/Cash
        /// </summary>
        /// <param name="reverse">reverse if true allocation is reversed</param>
        /// <returns>VAB_BusinessPartner_ID</returns>
        public int ProcessIt(bool reverse)
        {
            log.Fine("Reverse=" + reverse + " - " + ToString());
            int VAB_Invoice_ID = GetVAB_Invoice_ID();
            MInvoicePaySchedule invoiceSchedule = null;
            MPayment payment = null;
            MVABCashJRNLLine cashLine = null;
            MInvoice invoice = GetInvoice();
            if (invoice != null
                && GetVAB_BusinessPartner_ID() != invoice.GetVAB_BusinessPartner_ID())
                SetVAB_BusinessPartner_ID(invoice.GetVAB_BusinessPartner_ID());
            //
            int VAB_Payment_ID = GetVAB_Payment_ID();
            int VAB_CashJRNLLine_ID = GetVAB_CashJRNLLine_ID();

            //	Update Payment
            if (VAB_Payment_ID != 0)
            {
                payment = new MPayment(GetCtx(), VAB_Payment_ID, Get_TrxName());
                if (GetVAB_BusinessPartner_ID() != payment.GetVAB_BusinessPartner_ID())
                {
                    log.Warning("VAB_BusinessPartner_ID different - Invoice=" + GetVAB_BusinessPartner_ID() + " - Payment=" + payment.GetVAB_BusinessPartner_ID());
                }
                if (reverse)
                {
                    if (!payment.IsCashTrx())
                    {
                        payment.SetIsAllocated(false);
                        payment.Save();
                    }
                }
                else
                {
                    if (payment.TestAllocation())
                        payment.Save();
                }
            }

            //	Update Cash Journal
            if (VAB_CashJRNLLine_ID != 0)
            {
                cashLine = new MVABCashJRNLLine(GetCtx(), VAB_CashJRNLLine_ID, Get_TrxName());
                if (GetVAB_BusinessPartner_ID() != cashLine.GetVAB_BusinessPartner_ID())
                {
                    log.Warning("VAB_BusinessPartner_ID different - Invoice=" + GetVAB_BusinessPartner_ID() + " - CashJournal=" + cashLine.GetVAB_BusinessPartner_ID());
                }
                if (reverse)
                {
                    cashLine.SetIsAllocated(false);
                    if (!cashLine.Save(Get_Trx()))
                    {
                        ValueNamePair pp = VLogger.RetrieveError();
                        log.Log(Level.SEVERE, "Error found for updating cashLine  for  this Line ID = " + cashLine.GetVAB_CashJRNLLine_ID() +
                                   " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                    }
                }
            }

            //	Payment - Invoice
            if (VAB_Payment_ID != 0 && invoice != null)
            {
                //	Link to Invoice
                if (reverse)
                {
                    invoice.SetVAB_Payment_ID(0);
                    log.Fine("VAB_Payment_ID=" + VAB_Payment_ID
                       + " Unlinked from VAB_Invoice_ID=" + VAB_Invoice_ID);
                }
                else if (invoice.IsPaid())
                {
                    invoice.SetVAB_Payment_ID(VAB_Payment_ID);
                    log.Fine("VAB_Payment_ID=" + VAB_Payment_ID
                        + " Linked to VAB_Invoice_ID=" + VAB_Invoice_ID);
                }

                //	Link to Order
                String update = "UPDATE VAB_Order o "
                    + "SET VAB_Payment_ID="
                        + (reverse ? "NULL " : "(SELECT VAB_Payment_ID FROM VAB_Invoice WHERE VAB_Invoice_ID=" + VAB_Invoice_ID + ") ")
                    + "WHERE EXISTS (SELECT * FROM VAB_Invoice i "
                        + "WHERE o.VAB_Order_ID=i.VAB_Order_ID AND i.VAB_Invoice_ID=" + VAB_Invoice_ID + ")";
                if (DataBase.DB.ExecuteQuery(update, null, Get_TrxName()) > 0)
                {
                    log.Fine("VAB_Payment_ID=" + VAB_Payment_ID
                        + (reverse ? " UnLinked from" : " Linked to")
                        + " order of VAB_Invoice_ID=" + VAB_Invoice_ID);
                }
            }

            //	Cash - Invoice
            if (VAB_CashJRNLLine_ID != 0 && invoice != null)
            {
                //	Link to Invoice
                if (reverse)
                {
                    invoice.SetVAB_CashJRNLLine_ID(0);
                    log.Fine("VAB_CashJRNLLine_ID=" + VAB_CashJRNLLine_ID
                        + " Unlinked from VAB_Invoice_ID=" + VAB_Invoice_ID);
                    // Set isallocated false on cashline while allocation gets deallocated assigned by Mukesh sir on 27/12/2017
                    //MCashLine cashline = new MCashLine(GetCtx(), GetVAB_CashJRNLLine_ID(), Get_TrxName());
                    //cashline.SetIsAllocated(false);
                    //cashline.Save();
                }
                else
                {
                    invoice.SetVAB_CashJRNLLine_ID(VAB_CashJRNLLine_ID);
                    log.Fine("VAB_CashJRNLLine_ID=" + VAB_CashJRNLLine_ID
                        + " Linked to VAB_Invoice_ID=" + VAB_Invoice_ID);
                }

                //	Link to Order
                String update = "UPDATE VAB_Order o "
                    + "SET VAB_CashJRNLLine_ID="
                        + (reverse ? "NULL " : "(SELECT VAB_CashJRNLLine_ID FROM VAB_Invoice WHERE VAB_Invoice_ID=" + VAB_Invoice_ID + ") ")
                    + "WHERE EXISTS (SELECT * FROM VAB_Invoice i "
                        + "WHERE o.VAB_Order_ID=i.VAB_Order_ID AND i.VAB_Invoice_ID=" + VAB_Invoice_ID + ")";
                if (DataBase.DB.ExecuteQuery(update, null, Get_TrxName()) > 0)
                {
                    log.Fine("VAB_CashJRNLLine_ID=" + VAB_CashJRNLLine_ID
                        + (reverse ? " UnLinked from" : " Linked to")
                        + " order of VAB_Invoice_ID=" + VAB_Invoice_ID);
                }
            }

            if (GetVAGL_JRNLLine_ID() != 0 && reverse)
            {
                // set allocation as false on View Allocation reversal
                DB.ExecuteQuery(@" UPDATE VAGL_JRNLLINE SET isAllocated ='N' WHERE VAGL_JRNLLINE_ID =" + GetVAGL_JRNLLine_ID(), null, Get_TrxName());
            }
            // Added by Bharat- Update Discrepancy amount on Invoice.

            if (VAB_Payment_ID == 0 && VAB_CashJRNLLine_ID == 0 && invoice != null)
            {
                if (invoice.Get_ColumnIndex("DiscrepancyAmt") >= 0)
                {
                    decimal desAmt = invoice.GetDiscrepancyAmt();
                    decimal desAjusted = invoice.GetDiscrepancyAdjusted();
                    decimal allocAmt = Math.Abs(GetAmount());	//	absolute
                    if (reverse)
                    {
                        if (allocAmt > desAjusted)
                        {
                            desAmt = Decimal.Add(desAjusted, desAmt);
                            desAjusted = 0;
                        }
                        else
                        {
                            desAmt = Decimal.Add(desAmt, allocAmt);
                            desAjusted = Decimal.Subtract(desAjusted, allocAmt);
                        }

                        invoice.SetDiscrepancyAmt(desAmt);
                        invoice.SetDiscrepancyAdjusted(desAjusted);
                        if (desAmt > 0)
                        {
                            invoice.SetIsInDispute(true);
                        }
                    }
                    else
                    {
                        if (allocAmt > desAmt)
                        {
                            desAjusted = Decimal.Add(desAjusted, desAmt);
                            desAmt = 0;
                        }
                        else
                        {
                            desAjusted = Decimal.Add(desAjusted, allocAmt);
                            desAmt = Decimal.Subtract(desAmt, allocAmt);
                        }
                        invoice.SetDiscrepancyAmt(desAmt);
                        invoice.SetDiscrepancyAdjusted(desAjusted);
                        if (desAmt == 0)
                        {
                            invoice.SetIsInDispute(false);
                        }
                    }
                    if (!invoice.Save())
                    {
                        log.Log(Level.SEVERE, "Invoice not updated - " + invoice);
                    }
                }
            }

            //	Update Balance / Credit used - Counterpart of MInvoice.completeIt
            if (invoice != null)
            {
                if (invoice.TestAllocation() && !invoice.Save())
                {
                    log.Log(Level.SEVERE, "Invoice not updated - " + invoice);
                }
                else if (reverse)
                {
                    // added by Amit
                    // if payment Management module downloaded and Invoice Schedule id available on Allocation then mark ispaid on schedule as false
                    if (Env.IsModuleInstalled("VA009_"))
                    {
                        MVABDocAllocation allocHdr = new MVABDocAllocation(GetCtx(), GetVAB_DocAllocation_ID(), Get_Trx());
                        decimal payAmt = 0;
                        MDocType doctype = null;
                        MVABCurrency currency = new MVABCurrency(GetCtx(), invoice.GetVAB_Currency_ID(), null);
                        if (GetVAB_sched_InvoicePayment_ID() != 0 && !invoice.IsPaid())
                        {
                            invoiceSchedule = new MInvoicePaySchedule(GetCtx(), GetVAB_sched_InvoicePayment_ID(), Get_TrxName());
                            invoiceSchedule.SetVA009_IsPaid(false);
                            // when we update schedule paid as False, then update payment method and related fields on schedule as on Invoice Header
                            if (reverse)
                            {
                                invoiceSchedule.SetVA009_PaymentMethod_ID(invoice.GetVA009_PaymentMethod_ID());
                                DataSet dsPaymentMethod = DB.ExecuteDataset(@"SELECT VA009_PaymentMode, VA009_PaymentType, VA009_PaymentTrigger FROM VA009_PaymentMethod
                                          WHERE IsActive = 'Y' AND  VA009_PaymentMethod_ID = " + invoice.GetVA009_PaymentMethod_ID(), null, Get_Trx());
                                if (dsPaymentMethod != null && dsPaymentMethod.Tables.Count > 0 && dsPaymentMethod.Tables[0].Rows.Count > 0)
                                {
                                    if (!String.IsNullOrEmpty(Util.GetValueOfString(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentMode"])))
                                        invoiceSchedule.SetVA009_PaymentMode(Util.GetValueOfString(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentMode"]));
                                    if (!String.IsNullOrEmpty(Util.GetValueOfString(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentType"])))
                                        invoiceSchedule.SetVA009_PaymentType(Util.GetValueOfString(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentType"]));
                                    invoiceSchedule.SetVA009_PaymentTrigger(Util.GetValueOfString(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentTrigger"]));
                                }
                                dsPaymentMethod = null;
                            }
                            if (reverse && payment != null)
                            {
                                #region Handle for Payment & Invoice Allocation
                                doctype = new MDocType(GetCtx(), invoice.GetVAB_DocTypes_ID(), null);

                                // convert (payment amount / Amount from View Allocation) to invoice currency amount then subtract Paid invoice amount to calculated amount
                                if (doctype.GetDocBaseType() == "ARC" || doctype.GetDocBaseType() == "APC")
                                {
                                    if (payment.GetVAB_Invoice_ID() != 0)
                                    {
                                        // when payment created with invoice refernce direct
                                        // convert payment amount in invoice amt with payment date and payment conversion type
                                        payAmt = MVABExchangeRate.Convert(GetCtx(), Decimal.Negate(Decimal.Add(Decimal.Add((payment.GetPayAmt() +
                                            (payment.Get_ColumnIndex("BackupWithholdingAmount") >= 0 ? (payment.GetWithholdingAmt() + payment.GetBackupWithholdingAmount()) : 0)), payment.GetDiscountAmt()),
                                            payment.GetWriteOffAmt())), payment.GetVAB_Currency_ID(), invoice.GetVAB_Currency_ID(), payment.GetDateAcct(),
                                            payment.GetVAB_CurrencyType_ID(), GetVAF_Client_ID(), GetVAF_Org_ID());
                                    }
                                    else
                                    {
                                        // when payment created with Payment Allocate entry OR
                                        // when we match payment with invoice through Payment Allocation form
                                        // convert payment amount in invoice amt with view allocation date 
                                        payAmt = MVABExchangeRate.Convert(GetCtx(), Decimal.Negate(Decimal.Add(Decimal.Add(GetAmount(), GetDiscountAmt()),
                                            GetWriteOffAmt())), allocHdr.GetVAB_Currency_ID(), invoice.GetVAB_Currency_ID(), allocHdr.GetDateAcct(),
                                            invoice.GetVAB_CurrencyType_ID(), GetVAF_Client_ID(), GetVAF_Org_ID());
                                        if (doctype.GetDocBaseType() == "APC")
                                        {
                                            payAmt = Decimal.Negate(payAmt);
                                        }
                                    }
                                }
                                else
                                {
                                    if (payment.GetVAB_Invoice_ID() != 0)
                                    {
                                        // when we create payment with invoice reference direct
                                        // convert payment amount in invoice amt with payment date and payment conversion type
                                        payAmt = MVABExchangeRate.Convert(GetCtx(), Decimal.Add(Decimal.Add((payment.GetPayAmt() +
                                            (payment.Get_ColumnIndex("BackupWithholdingAmount") >= 0 ? (payment.GetWithholdingAmt() + payment.GetBackupWithholdingAmount()) : 0)), payment.GetDiscountAmt()),
                                            payment.GetWriteOffAmt()), payment.GetVAB_Currency_ID(), invoice.GetVAB_Currency_ID(), payment.GetDateAcct(),
                                            payment.GetVAB_CurrencyType_ID(), GetVAF_Client_ID(), GetVAF_Org_ID());
                                    }
                                    else
                                    {
                                        // when payment created with Payment Allocate entry Or
                                        // when we match payment with invoice through Payment Allocation form
                                        // convert payment amount in invoice amt with view allocation date 
                                        payAmt = MVABExchangeRate.Convert(GetCtx(), Decimal.Add(Decimal.Add(GetAmount(), GetDiscountAmt()), GetWriteOffAmt()),
                                        allocHdr.GetVAB_Currency_ID(), invoice.GetVAB_Currency_ID(), allocHdr.GetDateAcct(), invoice.GetVAB_CurrencyType_ID(),
                                        GetVAF_Client_ID(), GetVAF_Org_ID());
                                        if (doctype.GetDocBaseType() == "API")
                                        {
                                            payAmt = Decimal.Negate(payAmt);
                                        }
                                    }
                                }
                                invoiceSchedule.SetVA009_PaidAmntInvce(Decimal.Round(Decimal.Subtract(invoiceSchedule.GetVA009_PaidAmntInvce(), payAmt),
                                    currency.GetStdPrecision()));

                                // during reversal, if Invoice paid amount <> 0 then reduce that amount from next schedule
                                try
                                {
                                    if (invoiceSchedule.GetVA009_PaidAmntInvce() != 0)
                                    {
                                        int no = Util.GetValueOfInt(DB.ExecuteQuery(@"UPDATE VAB_sched_InvoicePayment 
                                                                 SET VA009_PaidAmntInvce =   NVL(VA009_PaidAmntInvce , 0) + " + Decimal.Round(invoiceSchedule.GetVA009_PaidAmntInvce(), currency.GetStdPrecision()) +
                                         @" , VA009_PaidAmnt =  NVL(VA009_PaidAmnt , 0) + " + Decimal.Round(MVABExchangeRate.ConvertBase(GetCtx(), invoiceSchedule.GetVA009_PaidAmntInvce(), invoice.GetVAB_Currency_ID(), invoice.GetDateAcct(), invoice.GetVAB_CurrencyType_ID(), GetVAF_Client_ID(), GetVAF_Org_ID()), currency.GetStdPrecision()) +
                                         @" WHERE VAB_sched_InvoicePayment_ID = ( SELECT MIN(VAB_sched_InvoicePayment_ID) FROM VAB_sched_InvoicePayment WHERE IsActive = 'Y'
                                                                 AND VA009_IsPaid = 'N' AND VAB_Invoice_ID = " + invoice.GetVAB_Invoice_ID() +
                                         @" AND VAB_sched_InvoicePayment_ID <> " + GetVAB_sched_InvoicePayment_ID() + " ) ", null, Get_Trx()));

                                        // set paid invoice amount = 0, no > 0 bcz this is not last schedule
                                        if (no > 0)
                                        {
                                            invoiceSchedule.SetVA009_PaidAmntInvce(0);
                                        }
                                    }
                                }
                                catch { }

                                // convert invoice paid amount to base currency amount
                                invoiceSchedule.SetVA009_PaidAmnt(Decimal.Round(MVABExchangeRate.ConvertBase(GetCtx(), invoiceSchedule.GetVA009_PaidAmntInvce(),
                                 invoice.GetVAB_Currency_ID(), invoice.GetDateAcct(), invoice.GetVAB_CurrencyType_ID(), GetVAF_Client_ID(), GetVAF_Org_ID()),
                                 currency.GetStdPrecision()));

                                // set Currency Variance amount as 0, when we reverse paymnet/ cash journalor allocation against this schedule
                                invoiceSchedule.SetVA009_Variance(0);

                                // remove linking of Payment from schedule
                                invoiceSchedule.SetVAB_Payment_ID(0);

                                #endregion
                            }
                            else if (reverse && VAB_CashJRNLLine_ID > 0)
                            {
                                #region Handle fo Cash Journal & Invoice Allocation

                                doctype = new MDocType(GetCtx(), invoice.GetVAB_DocTypes_ID(), null);
                                cashLine = new MVABCashJRNLLine(GetCtx(), VAB_CashJRNLLine_ID, Get_Trx());

                                // convert cash amount to invoice currency amount with allocation date then subtract Paid invoice amount to calculated amount
                                if (doctype.GetDocBaseType() == "ARC" || doctype.GetDocBaseType() == "API")
                                {
                                    payAmt = Decimal.Negate(Decimal.Add(Decimal.Add(GetAmount(), GetDiscountAmt()), GetWriteOffAmt()));
                                    payAmt = MVABExchangeRate.Convert(GetCtx(), payAmt, allocHdr.GetVAB_Currency_ID(), invoice.GetVAB_Currency_ID(), allocHdr.GetDateAcct(),
                                        invoice.GetVAB_CurrencyType_ID(), GetVAF_Client_ID(), GetVAF_Org_ID());
                                    invoiceSchedule.SetVA009_PaidAmntInvce(Decimal.Round(Decimal.Subtract(invoiceSchedule.GetVA009_PaidAmntInvce(), payAmt),
                                        currency.GetStdPrecision()));
                                }
                                else
                                {
                                    payAmt = Decimal.Add(Decimal.Add(GetAmount(), GetDiscountAmt()), GetWriteOffAmt());
                                    payAmt = MVABExchangeRate.Convert(GetCtx(), payAmt, allocHdr.GetVAB_Currency_ID(), invoice.GetVAB_Currency_ID(), allocHdr.GetDateAcct(),
                                        invoice.GetVAB_CurrencyType_ID(), GetVAF_Client_ID(), GetVAF_Org_ID());
                                    invoiceSchedule.SetVA009_PaidAmntInvce(Decimal.Round(Decimal.Subtract(invoiceSchedule.GetVA009_PaidAmntInvce(), payAmt),
                                        currency.GetStdPrecision()));
                                }

                                // during reversal, if Invoice paid amount <> 0 then reduce that amount from next schedule
                                try
                                {
                                    if (invoiceSchedule.GetVA009_PaidAmntInvce() != 0)
                                    {
                                        int no = Util.GetValueOfInt(DB.ExecuteQuery(@"UPDATE VAB_sched_InvoicePayment 
                                                                 SET VA009_PaidAmntInvce =   NVL(VA009_PaidAmntInvce , 0) + " + Decimal.Round(invoiceSchedule.GetVA009_PaidAmntInvce(), currency.GetStdPrecision()) +
                                         @" , VA009_PaidAmnt =  NVL(VA009_PaidAmnt , 0) + " + Decimal.Round(MVABExchangeRate.ConvertBase(GetCtx(), invoiceSchedule.GetVA009_PaidAmntInvce(), invoice.GetVAB_Currency_ID(), invoice.GetDateAcct(), invoice.GetVAB_CurrencyType_ID(), GetVAF_Client_ID(), GetVAF_Org_ID()), currency.GetStdPrecision()) +
                                         @" WHERE VAB_sched_InvoicePayment_ID = ( SELECT MIN(VAB_sched_InvoicePayment_ID) FROM VAB_sched_InvoicePayment WHERE IsActive = 'Y'
                                                                 AND VA009_IsPaid = 'N' AND VAB_Invoice_ID = " + invoice.GetVAB_Invoice_ID() +
                                         @" AND VAB_sched_InvoicePayment_ID <> " + GetVAB_sched_InvoicePayment_ID() + " ) ", null, Get_Trx()));

                                        // set paid invoice amount = 0, no > 0 bcz this is not last schedule
                                        if (no > 0)
                                        {
                                            invoiceSchedule.SetVA009_PaidAmntInvce(0);
                                        }
                                    }
                                }
                                catch { }

                                // convert invoice paid amount to base currency amount
                                invoiceSchedule.SetVA009_PaidAmnt(Decimal.Round(MVABExchangeRate.ConvertBase(GetCtx(), invoiceSchedule.GetVA009_PaidAmntInvce(),
                                 invoice.GetVAB_Currency_ID(), invoice.GetDateAcct(), invoice.GetVAB_CurrencyType_ID(), GetVAF_Client_ID(), GetVAF_Org_ID()),
                                 currency.GetStdPrecision()));

                                // set Currency Variance amount as 0, when we reverse paymnet/ cash journalor allocation against this schedule
                                invoiceSchedule.SetVA009_Variance(0);

                                // remove linking of cash line from schedule
                                invoiceSchedule.SetVAB_CashJRNLLine_ID(0);

                                #endregion
                            }
                            else
                            {
                                invoiceSchedule.SetVA009_PaidAmntInvce(0);
                                invoiceSchedule.SetVA009_PaidAmnt(0);
                                // set Currency Variance amount as 0, when we reverse paymnet/ cash journalor allocation against this schedule
                                invoiceSchedule.SetVA009_Variance(0);
                            }
                            if (!invoiceSchedule.Save(Get_TrxName()))
                            {
                                log.Log(Level.SEVERE, "Invoice Pay Schedule not updated - " + invoice);
                            }
                        }
                    }
                }
            }

            return GetVAB_BusinessPartner_ID();
        }

    }
}
