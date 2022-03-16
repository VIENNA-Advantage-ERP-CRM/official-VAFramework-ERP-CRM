/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_C_AllocationLine
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
using VAdvantage.Process;

namespace VAdvantage.Model
{
    public class MAllocationLine : X_C_AllocationLine
    {
        /**	Invoice info			*/
        private MInvoice _invoice = null;
        /** Allocation Header		*/
        private MAllocationHdr _parent = null;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_AllocationLine_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MAllocationLine(Ctx ctx, int C_AllocationLine_ID, Trx trxName)
            : base(ctx, C_AllocationLine_ID, trxName)
        {
            if (C_AllocationLine_ID == 0)
            {
                //	setC_AllocationHdr_ID (0);
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
        public MAllocationLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        public MAllocationLine(MAllocationHdr parent)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetC_AllocationHdr_ID(parent.GetC_AllocationHdr_ID());
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
        public MAllocationLine(MAllocationHdr parent, Decimal amount,
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
        public MAllocationHdr GetParent()
        {
            if (_parent == null)
                _parent = new MAllocationHdr(GetCtx(), GetC_AllocationHdr_ID(), Get_TrxName());
            return _parent;
        }

        /// <summary>
        /// Set Parent
        /// </summary>
        /// <param name="parent">parent</param>
        public void SetParent(MAllocationHdr parent)
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
        /// <param name="C_BPartner_ID">partner</param>
        /// <param name="C_Order_ID">order</param>
        /// <param name="C_Invoice_ID">invoice</param>
        public void SetDocInfo(int C_BPartner_ID, int C_Order_ID, int C_Invoice_ID)
        {
            SetC_BPartner_ID(C_BPartner_ID);
            SetC_Order_ID(C_Order_ID);
            SetC_Invoice_ID(C_Invoice_ID);
        }

        /// <summary>
        /// Set Payment Info
        /// </summary>
        /// <param name="C_Payment_ID">payment</param>
        /// <param name="C_CashLine_ID">cash line</param>
        public void SetPaymentInfo(int C_Payment_ID, int C_CashLine_ID)
        {
            if (C_Payment_ID != 0)
                SetC_Payment_ID(C_Payment_ID);
            if (C_CashLine_ID != 0)
                SetC_CashLine_ID(C_CashLine_ID);
        }

        /// <summary>
        /// Get Invoice
        /// </summary>
        /// <returns>invoice or null</returns>
        public MInvoice GetInvoice()
        {
            if (_invoice == null && GetC_Invoice_ID() != 0)
                _invoice = new MInvoice(GetCtx(), GetC_Invoice_ID(), Get_TrxName());
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
                && (Is_ValueChanged("C_BPartner_ID") || Is_ValueChanged("C_Invoice_ID")))
            {
                log.Severe("Cannot Change Business Partner or Invoice");
                return false;
            }

            //	Set BPartner/Order from Invoice
            if (GetC_BPartner_ID() == 0 && GetInvoice() != null)
                SetC_BPartner_ID(GetInvoice().GetC_BPartner_ID());
            if (GetC_Order_ID() == 0 && GetInvoice() != null)
                SetC_Order_ID(GetInvoice().GetC_Order_ID());
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
            if (GetC_Payment_ID() != 0)
                sb.Append(",C_Payment_ID=").Append(GetC_Payment_ID());
            if (GetC_CashLine_ID() != 0)
                sb.Append(",C_CashLine_ID=").Append(GetC_CashLine_ID());
            if (GetC_Invoice_ID() != 0)
                sb.Append(",C_Invoice_ID=").Append(GetC_Invoice_ID());
            if (GetC_BPartner_ID() != 0)
                sb.Append(",C_BPartner_ID=").Append(GetC_BPartner_ID());
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
        /// <returns>C_BPartner_ID</returns>
        public int ProcessIt(bool reverse)
        {
            log.Fine("Reverse=" + reverse + " - " + ToString());
            int C_Invoice_ID = GetC_Invoice_ID();
            MInvoicePaySchedule invoiceSchedule = null;
            MPayment payment = null;
            MCashLine cashLine = null;
            MInvoice invoice = GetInvoice();
            if (invoice != null
                && GetC_BPartner_ID() != invoice.GetC_BPartner_ID())
                SetC_BPartner_ID(invoice.GetC_BPartner_ID());
            //
            int C_Payment_ID = GetC_Payment_ID();
            int C_CashLine_ID = GetC_CashLine_ID();

            //	Update Payment
            if (C_Payment_ID != 0)
            {
                payment = new MPayment(GetCtx(), C_Payment_ID, Get_TrxName());
                if (GetC_BPartner_ID() != payment.GetC_BPartner_ID())
                {
                    log.Warning("C_BPartner_ID different - Invoice=" + GetC_BPartner_ID() + " - Payment=" + payment.GetC_BPartner_ID());
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
            if (C_CashLine_ID != 0)
            {
                cashLine = new MCashLine(GetCtx(), C_CashLine_ID, Get_TrxName());
                if (GetC_BPartner_ID() != cashLine.GetC_BPartner_ID())
                {
                    log.Warning("C_BPartner_ID different - Invoice=" + GetC_BPartner_ID() + " - CashJournal=" + cashLine.GetC_BPartner_ID());
                }
                if (reverse)
                {
                    cashLine.SetIsAllocated(false);
                    if (!cashLine.Save(Get_Trx()))
                    {
                        ValueNamePair pp = VLogger.RetrieveError();
                        log.Log(Level.SEVERE, "Error found for updating cashLine  for  this Line ID = " + cashLine.GetC_CashLine_ID() +
                                   " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                    }
                }
                else
                {
                    //VA230:Update Allocated status on cashline
                    if (Util.GetValueOfInt(DB.ExecuteQuery("UPDATE C_CashLine SET IsAllocated='" + (cashLine.TestAllocation() ? 'Y' : 'N') + "' WHERE C_CashLine_ID=" + C_CashLine_ID, null, Get_TrxName())) <= 0)
                    {
                        log.Log(Level.SEVERE, "Error found for updating cashLine  for  this Line ID = " + cashLine.GetC_CashLine_ID() + " ,VoucherNo: " + cashLine.GetVSS_RECEIPTNO());
                    }
                }
            }

            //	Payment - Invoice
            if (C_Payment_ID != 0 && invoice != null)
            {
                //	Link to Invoice
                if (reverse)
                {
                    invoice.SetC_Payment_ID(0);
                    log.Fine("C_Payment_ID=" + C_Payment_ID
                       + " Unlinked from C_Invoice_ID=" + C_Invoice_ID);
                }
                else if (invoice.IsPaid())
                {
                    invoice.SetC_Payment_ID(C_Payment_ID);
                    log.Fine("C_Payment_ID=" + C_Payment_ID
                        + " Linked to C_Invoice_ID=" + C_Invoice_ID);
                }

                //	Link to Order
                String update = "UPDATE C_Order o "
                    + "SET C_Payment_ID="
                        + (reverse ? "NULL " : "(SELECT C_Payment_ID FROM C_Invoice WHERE C_Invoice_ID=" + C_Invoice_ID + ") ")
                    + "WHERE EXISTS (SELECT * FROM C_Invoice i "
                        + "WHERE o.C_Order_ID=i.C_Order_ID AND i.C_Invoice_ID=" + C_Invoice_ID + ")";
                if (DataBase.DB.ExecuteQuery(update, null, Get_TrxName()) > 0)
                {
                    log.Fine("C_Payment_ID=" + C_Payment_ID
                        + (reverse ? " UnLinked from" : " Linked to")
                        + " order of C_Invoice_ID=" + C_Invoice_ID);
                }
            }

            //	Cash - Invoice
            if (C_CashLine_ID != 0 && invoice != null)
            {
                //	Link to Invoice
                if (reverse)
                {
                    invoice.SetC_CashLine_ID(0);
                    log.Fine("C_CashLine_ID=" + C_CashLine_ID
                        + " Unlinked from C_Invoice_ID=" + C_Invoice_ID);
                    // Set isallocated false on cashline while allocation gets deallocated assigned by Mukesh sir on 27/12/2017
                    //MCashLine cashline = new MCashLine(GetCtx(), GetC_CashLine_ID(), Get_TrxName());
                    //cashline.SetIsAllocated(false);
                    //cashline.Save();
                }
                else
                {
                    invoice.SetC_CashLine_ID(C_CashLine_ID);
                    log.Fine("C_CashLine_ID=" + C_CashLine_ID
                        + " Linked to C_Invoice_ID=" + C_Invoice_ID);
                }

                //	Link to Order
                String update = "UPDATE C_Order o "
                    + "SET C_CashLine_ID="
                        + (reverse ? "NULL " : "(SELECT C_CashLine_ID FROM C_Invoice WHERE C_Invoice_ID=" + C_Invoice_ID + ") ")
                    + "WHERE EXISTS (SELECT * FROM C_Invoice i "
                        + "WHERE o.C_Order_ID=i.C_Order_ID AND i.C_Invoice_ID=" + C_Invoice_ID + ")";
                if (DataBase.DB.ExecuteQuery(update, null, Get_TrxName()) > 0)
                {
                    log.Fine("C_CashLine_ID=" + C_CashLine_ID
                        + (reverse ? " UnLinked from" : " Linked to")
                        + " order of C_Invoice_ID=" + C_Invoice_ID);
                }
            }

            if (GetGL_JournalLine_ID() != 0 && reverse)
            {
                // set allocation as false on View Allocation reversal
                DB.ExecuteQuery(@" UPDATE GL_JOURNALLINE SET isAllocated ='N' WHERE GL_JOURNALLINE_ID =" + GetGL_JournalLine_ID(), null, Get_TrxName());
            }
            // Added by Bharat- Update Discrepancy amount on Invoice.

            if (C_Payment_ID == 0 && C_CashLine_ID == 0 && invoice != null)
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
                        MAllocationHdr allocHdr = new MAllocationHdr(GetCtx(), GetC_AllocationHdr_ID(), Get_Trx());
                        decimal payAmt = 0;
                        MDocType doctype = null;
                        MCurrency currency = MCurrency.Get(GetCtx(), invoice.GetC_Currency_ID());
                        if (GetC_InvoicePaySchedule_ID() != 0 && !invoice.IsPaid())
                        {
                            invoiceSchedule = new MInvoicePaySchedule(GetCtx(), GetC_InvoicePaySchedule_ID(), Get_TrxName());
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
                                doctype = MDocType.Get(GetCtx(), invoice.GetC_DocType_ID());

                                // convert (payment amount / Amount from View Allocation) to invoice currency amount then subtract Paid invoice amount to calculated amount
                                if (doctype.GetDocBaseType() == "ARC" || doctype.GetDocBaseType() == "APC")
                                {
                                    if (payment.GetC_Invoice_ID() != 0)
                                    {
                                        // when payment created with invoice refernce direct
                                        // convert payment amount in invoice amt with payment date and payment conversion type
                                        payAmt = MConversionRate.Convert(GetCtx(), Decimal.Negate(Decimal.Add(Decimal.Add((payment.GetPayAmt() +
                                            (payment.Get_ColumnIndex("BackupWithholdingAmount") >= 0 ? (payment.GetWithholdingAmt() + payment.GetBackupWithholdingAmount()) : 0)), payment.GetDiscountAmt()),
                                            payment.GetWriteOffAmt())), payment.GetC_Currency_ID(), invoice.GetC_Currency_ID(), payment.GetDateAcct(),
                                            payment.GetC_ConversionType_ID(), GetAD_Client_ID(), GetAD_Org_ID());
                                    }
                                    else
                                    {
                                        // when payment created with Payment Allocate entry OR
                                        // when we match payment with invoice through Payment Allocation form
                                        // convert payment amount in invoice amt with view allocation date 
                                        payAmt = MConversionRate.Convert(GetCtx(), Decimal.Negate(Decimal.Add(Decimal.Add(GetAmount(), GetDiscountAmt()),
                                            GetWriteOffAmt())), allocHdr.GetC_Currency_ID(), invoice.GetC_Currency_ID(), allocHdr.GetDateAcct(),
                                            invoice.GetC_ConversionType_ID(), GetAD_Client_ID(), GetAD_Org_ID());
                                        if (doctype.GetDocBaseType() == "APC")
                                        {
                                            payAmt = Decimal.Negate(payAmt);
                                        }
                                    }
                                }
                                else
                                {
                                    if (payment.GetC_Invoice_ID() != 0)
                                    {
                                        // when we create payment with invoice reference direct
                                        // convert payment amount in invoice amt with payment date and payment conversion type
                                        payAmt = MConversionRate.Convert(GetCtx(), Decimal.Add(Decimal.Add((payment.GetPayAmt() +
                                            (payment.Get_ColumnIndex("BackupWithholdingAmount") >= 0 ? (payment.GetWithholdingAmt() + payment.GetBackupWithholdingAmount()) : 0)), payment.GetDiscountAmt()),
                                            payment.GetWriteOffAmt()), payment.GetC_Currency_ID(), invoice.GetC_Currency_ID(), payment.GetDateAcct(),
                                            payment.GetC_ConversionType_ID(), GetAD_Client_ID(), GetAD_Org_ID());
                                    }
                                    else
                                    {
                                        // when payment created with Payment Allocate entry Or
                                        // when we match payment with invoice through Payment Allocation form
                                        // convert payment amount in invoice amt with view allocation date 
                                        payAmt = MConversionRate.Convert(GetCtx(), Decimal.Add(Decimal.Add(GetAmount(), GetDiscountAmt()), GetWriteOffAmt()),
                                        allocHdr.GetC_Currency_ID(), invoice.GetC_Currency_ID(), allocHdr.GetDateAcct(), invoice.GetC_ConversionType_ID(),
                                        GetAD_Client_ID(), GetAD_Org_ID());
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
                                        int no = Util.GetValueOfInt(DB.ExecuteQuery(@"UPDATE C_InvoicePaySchedule 
                                                                 SET VA009_PaidAmntInvce =   NVL(VA009_PaidAmntInvce , 0) + " + Decimal.Round(invoiceSchedule.GetVA009_PaidAmntInvce(), currency.GetStdPrecision()) +
                                         @" , VA009_PaidAmnt =  NVL(VA009_PaidAmnt , 0) + " + Decimal.Round(MConversionRate.ConvertBase(GetCtx(), invoiceSchedule.GetVA009_PaidAmntInvce(), invoice.GetC_Currency_ID(), invoice.GetDateAcct(), invoice.GetC_ConversionType_ID(), GetAD_Client_ID(), GetAD_Org_ID()), currency.GetStdPrecision()) +
                                         @" WHERE C_InvoicePaySchedule_ID = ( SELECT MIN(C_InvoicePaySchedule_ID) FROM C_InvoicePaySchedule WHERE IsActive = 'Y'
                                                                 AND VA009_IsPaid = 'N' AND C_Invoice_ID = " + invoice.GetC_Invoice_ID() +
                                         @" AND C_InvoicePaySchedule_ID <> " + GetC_InvoicePaySchedule_ID() + " ) ", null, Get_Trx()));

                                        // set paid invoice amount = 0, no > 0 bcz this is not last schedule
                                        if (no > 0)
                                        {
                                            invoiceSchedule.SetVA009_PaidAmntInvce(0);
                                        }
                                    }
                                }
                                catch { }

                                // convert invoice paid amount to base currency amount
                                invoiceSchedule.SetVA009_PaidAmnt(Decimal.Round(MConversionRate.ConvertBase(GetCtx(), invoiceSchedule.GetVA009_PaidAmntInvce(),
                                 invoice.GetC_Currency_ID(), invoice.GetDateAcct(), invoice.GetC_ConversionType_ID(), GetAD_Client_ID(), GetAD_Org_ID()),
                                 currency.GetStdPrecision()));

                                // set Currency Variance amount as 0, when we reverse paymnet/ cash journalor allocation against this schedule
                                invoiceSchedule.SetVA009_Variance(0);

                                // remove linking of Payment from schedule
                                invoiceSchedule.SetC_Payment_ID(0);

                                #endregion
                            }
                            else if (reverse && C_CashLine_ID > 0)
                            {
                                #region Handle fo Cash Journal & Invoice Allocation

                                doctype = MDocType.Get(GetCtx(), invoice.GetC_DocType_ID());
                                cashLine = new MCashLine(GetCtx(), C_CashLine_ID, Get_Trx());

                                // convert cash amount to invoice currency amount with allocation date then subtract Paid invoice amount to calculated amount
                                if (doctype.GetDocBaseType() == "ARC" || doctype.GetDocBaseType() == "API")
                                {
                                    payAmt = Decimal.Negate(Decimal.Add(Decimal.Add(GetAmount(), GetDiscountAmt()), GetWriteOffAmt()));
                                    payAmt = MConversionRate.Convert(GetCtx(), payAmt, allocHdr.GetC_Currency_ID(), invoice.GetC_Currency_ID(), allocHdr.GetDateAcct(),
                                        invoice.GetC_ConversionType_ID(), GetAD_Client_ID(), GetAD_Org_ID());
                                    invoiceSchedule.SetVA009_PaidAmntInvce(Decimal.Round(Decimal.Subtract(invoiceSchedule.GetVA009_PaidAmntInvce(), payAmt),
                                        currency.GetStdPrecision()));
                                }
                                else
                                {
                                    payAmt = Decimal.Add(Decimal.Add(GetAmount(), GetDiscountAmt()), GetWriteOffAmt());
                                    payAmt = MConversionRate.Convert(GetCtx(), payAmt, allocHdr.GetC_Currency_ID(), invoice.GetC_Currency_ID(), allocHdr.GetDateAcct(),
                                        invoice.GetC_ConversionType_ID(), GetAD_Client_ID(), GetAD_Org_ID());
                                    invoiceSchedule.SetVA009_PaidAmntInvce(Decimal.Round(Decimal.Subtract(invoiceSchedule.GetVA009_PaidAmntInvce(), payAmt),
                                        currency.GetStdPrecision()));
                                }

                                // during reversal, if Invoice paid amount <> 0 then reduce that amount from next schedule
                                try
                                {
                                    if (invoiceSchedule.GetVA009_PaidAmntInvce() != 0)
                                    {
                                        int no = Util.GetValueOfInt(DB.ExecuteQuery(@"UPDATE C_InvoicePaySchedule 
                                                                 SET VA009_PaidAmntInvce =   NVL(VA009_PaidAmntInvce , 0) + " + Decimal.Round(invoiceSchedule.GetVA009_PaidAmntInvce(), currency.GetStdPrecision()) +
                                         @" , VA009_PaidAmnt =  NVL(VA009_PaidAmnt , 0) + " + Decimal.Round(MConversionRate.ConvertBase(GetCtx(), invoiceSchedule.GetVA009_PaidAmntInvce(), invoice.GetC_Currency_ID(), invoice.GetDateAcct(), invoice.GetC_ConversionType_ID(), GetAD_Client_ID(), GetAD_Org_ID()), currency.GetStdPrecision()) +
                                         @" WHERE C_InvoicePaySchedule_ID = ( SELECT MIN(C_InvoicePaySchedule_ID) FROM C_InvoicePaySchedule WHERE IsActive = 'Y'
                                                                 AND VA009_IsPaid = 'N' AND C_Invoice_ID = " + invoice.GetC_Invoice_ID() +
                                         @" AND C_InvoicePaySchedule_ID <> " + GetC_InvoicePaySchedule_ID() + " ) ", null, Get_Trx()));

                                        // set paid invoice amount = 0, no > 0 bcz this is not last schedule
                                        if (no > 0)
                                        {
                                            invoiceSchedule.SetVA009_PaidAmntInvce(0);
                                        }
                                    }
                                }
                                catch { }

                                // convert invoice paid amount to base currency amount
                                invoiceSchedule.SetVA009_PaidAmnt(Decimal.Round(MConversionRate.ConvertBase(GetCtx(), invoiceSchedule.GetVA009_PaidAmntInvce(),
                                 invoice.GetC_Currency_ID(), invoice.GetDateAcct(), invoice.GetC_ConversionType_ID(), GetAD_Client_ID(), GetAD_Org_ID()),
                                 currency.GetStdPrecision()));

                                // set Currency Variance amount as 0, when we reverse paymnet/ cash journalor allocation against this schedule
                                invoiceSchedule.SetVA009_Variance(0);

                                // remove linking of cash line from schedule
                                invoiceSchedule.SetC_CashLine_ID(0);

                                #endregion
                            }
                            else
                            {
                                invoiceSchedule.SetVA009_PaidAmntInvce(0);
                                invoiceSchedule.SetVA009_PaidAmnt(0);
                                // set Currency Variance amount as 0, when we reverse paymnet/ cash journalor allocation against this schedule
                                invoiceSchedule.SetVA009_Variance(0);

                                // Clear GL Journal line reference also on Reversal 
                                if (invoiceSchedule.Get_ValueAsInt("GL_JournalLine_ID") > 0)
                                {
                                    invoiceSchedule.Set_Value("GL_JournalLine_ID", 0);
                                }
                            }
                            if (!invoiceSchedule.Save(Get_TrxName()))
                            {
                                log.Log(Level.SEVERE, "Invoice Pay Schedule not updated - " + invoice);
                            }
                        }
                    }
                }
            }

            return GetC_BPartner_ID();
        }

    }
}
