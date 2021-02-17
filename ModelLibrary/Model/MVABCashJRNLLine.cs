/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MCashLine
 * Purpose        : Cash Line Model
 * Class Used     : X_VAB_CashJRNLLine
 * Chronological    Development
 * Raghunandan     23-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.ProcessEngine;
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using VAdvantage.Logging;


namespace VAdvantage.Model
{
    public class MVABCashJRNLLine : X_VAB_CashJRNLLine
    {
        #region variables
        // Parent				
        private MVABCashJRNL _parent = null;
        // Cash Book			
        private MVABCashBook _cashBook = null;
        // Bank Account			
        private MVABBankAcct _bankAccount = null;
        // Invoice				
        private MVABInvoice _invoice = null;
        // old Vlaues
        decimal old_sdAmt = 0, old_ebAmt = 0, new_sdAmt = 0, new_ebAmt = 0;

        Tuple<String, String, String> mInfo = null;

        private bool resetAmtDim = false;
        #endregion


        /* Standard Constructor
         * @param ctx context
	     *	@param VAB_CashJRNLLine_ID id
	     *	@param trxName transaction
	    */
        public MVABCashJRNLLine(Ctx ctx, int VAB_CashJRNLLine_ID, Trx trxName)
            : base(ctx, VAB_CashJRNLLine_ID, trxName)
        {
            if (VAB_CashJRNLLine_ID == 0)
            {
                //	setLine (0);
                //	setCashType (CASHTYPE_GeneralExpense);
                SetAmount(Env.ZERO);
                SetDiscountAmt(Env.ZERO);
                SetWriteOffAmt(Env.ZERO);
                // Added by Amit 31-7-2015 VAMRP
                //if (Env.HasModulePrefix("VAMRP_", out mInfo))
                //{
                //    SetRETURNLOANAMOUNT(Env.ZERO);
                //}
                //End 
                SetIsGenerated(false);
            }
        }

        /**
         * 	Load Cosntructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MVABCashJRNLLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /**
         * 	Parent Cosntructor
         *	@param cash parent
         */
        public MVABCashJRNLLine(MVABCashJRNL cash)
            : this(cash.GetCtx(), 0, cash.Get_TrxName())
        {
            SetClientOrg(cash);
            SetVAB_CashJRNL_ID(cash.GetVAB_CashJRNL_ID());
            _parent = cash;
            _cashBook = _parent.GetCashBook();
        }

        /**
         * 	Add to Description
         *	@param description text
         */
        public void AddDescription(String description)
        {
            String desc = GetDescription();
            if (desc == null)
                SetDescription(description);
            else
                SetDescription(desc + " | " + description);
        }

        /// <summary>
        /// Set Invoice - no discount
        /// </summary>
        /// <param name="invoice">Invoice</param>
        public void SetInvoice(MVABInvoice invoice)
        {
            Decimal amt = 0;
            SetVAB_Invoice_ID(invoice.GetVAB_Invoice_ID());
            SetCashType(CASHTYPE_Invoice);
            SetVAB_BusinessPartner_ID(invoice.GetVAB_BusinessPartner_ID());

            // JID_0687: System is not updating the Location on cash journal line in cases of POS order and payment method cash
            SetVAB_BPart_Location_ID(invoice.GetVAB_BPart_Location_ID());

            SetVAB_Currency_ID(invoice.GetVAB_Currency_ID());
            //	Amount
            MVABDocTypes dt = MVABDocTypes.Get(GetCtx(), invoice.GetVAB_DocTypes_ID());
            if (invoice.GetRef_VAB_Invoice_ID() > 0)
            {
                //amt = Decimal.Negate(Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(Al.Amount) FROM VAB_DocAllocationLine al INNER JOIN VAB_DocAllocation alhdr ON alhdr.VAB_DocAllocation_ID=al.VAB_DocAllocation_ID "
                //                                            + " WHERE alhdr.isactive        ='Y' AND Alhdr.Docstatus        IN ('CO','CL') and al.VAB_Invoice_id=" + invoice.GetRef_VAB_Invoice_ID())));
                // get amount against invoice wheether cash journal is completed or not
                // Done by Vivek on 01/03/2016
                amt = Decimal.Negate(Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(cl.Amount) FROM VAB_CashJRNLLine cl INNER JOIN VAB_CashJRNL cs ON cs.VAB_CashJRNL_ID=CL.VAB_CASHJRNL_ID WHERE cl.VAB_Invoice_ID=" + invoice.GetRef_VAB_Invoice_ID() + " AND cs.DocStatus NOT IN ('VO') ")));
            }
            else
            {
                amt = invoice.GetGrandTotal();
            }
            if (MVABMasterDocType.DOCBASETYPE_APINVOICE.Equals(dt.GetDocBaseType())
                || MVABMasterDocType.DOCBASETYPE_ARCREDITMEMO.Equals(dt.GetDocBaseType()))
            {
                amt = Decimal.Negate(amt);
                // set payment type according to invoice document type
                SetVSS_PAYMENTTYPE("P");
            }
            else
            {
                // set payment type according to invoice document type
                SetVSS_PAYMENTTYPE("R");
            }
            SetAmount(amt);
            //
            SetDiscountAmt(Env.ZERO);
            SetWriteOffAmt(Env.ZERO);
            SetIsGenerated(true);
            _invoice = invoice;
        }

        /// <summary>
        /// Create Cash Journal Line
        /// </summary>
        /// <param name="invoice">Invoice</param>
        /// <param name="VAB_sched_InvoicePayment_ID">Invoice Payemt Schedule</param>
        /// <param name="amt">Amount</param>
        public void CreateCashLine(MVABInvoice invoice, int VAB_sched_InvoicePayment_ID, decimal amt)
        {
            SetVAB_Invoice_ID(invoice.GetVAB_Invoice_ID());
            SetVAB_sched_InvoicePayment_ID(VAB_sched_InvoicePayment_ID);
            SetCashType(CASHTYPE_Invoice);
            SetVAB_BusinessPartner_ID(invoice.GetVAB_BusinessPartner_ID());

            // JID_0687: System is not updating the Location on cash journal line in cases of POS order and payment method cash
            SetVAB_BPart_Location_ID(invoice.GetVAB_BPart_Location_ID());

            SetVAB_Currency_ID(invoice.GetVAB_Currency_ID());
            //	Amount
            MVABDocTypes dt = MVABDocTypes.Get(GetCtx(), invoice.GetVAB_DocTypes_ID());
            if (MVABMasterDocType.DOCBASETYPE_APINVOICE.Equals(dt.GetDocBaseType())
                || MVABMasterDocType.DOCBASETYPE_ARCREDITMEMO.Equals(dt.GetDocBaseType()))
            {
                amt = Decimal.Negate(amt);
                SetVSS_PAYMENTTYPE("P");
            }
            else
            {
                SetVSS_PAYMENTTYPE("R");
            }
            SetAmount(amt);
            //
            SetDiscountAmt(Env.ZERO);
            SetWriteOffAmt(Env.ZERO);
            SetIsGenerated(true);
            _invoice = invoice;
        }

        /**
         * 	Set Invoice - Callout
         *	@param oldVAB_Invoice_ID old BP
         *	@param newVAB_Invoice_ID new BP
         *	@param windowNo window no
         */
        //@UICallout 
        public void SetVAB_Invoice_ID(String oldVAB_Invoice_ID, String newVAB_Invoice_ID, int windowNo)
        {
            if (newVAB_Invoice_ID == null || newVAB_Invoice_ID.Length == 0)
                return;
            int VAB_Invoice_ID = int.Parse(newVAB_Invoice_ID);
            if (VAB_Invoice_ID == 0)
                return;

            //  Date
            DateTime ts = new DateTime(GetCtx().GetContextAsTime(windowNo, "DateAcct"));     //  from VAB_CashJRNL
            String sql = "SELECT VAB_BusinessPartner_ID, VAB_Currency_ID,"		//	1..2
                + "invoiceOpen(VAB_Invoice_ID, 0), IsSOTrx, "			//	3..4
                + "paymentTermDiscount(invoiceOpen(VAB_Invoice_ID, 0),VAB_Currency_ID,VAB_PaymentTerm_ID,DateInvoiced," + DB.TO_DATE(ts, true) + ") "
                + "FROM VAB_Invoice WHERE VAB_Invoice_ID=" + VAB_Invoice_ID;
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    SetVAB_Currency_ID(Convert.ToInt32(dr[1]));//.getInt(2));
                    Decimal PayAmt = Convert.ToDecimal(dr[2]);//.getBigDecimal(3);
                    Decimal DiscountAmt = Convert.ToDecimal(dr[4]);
                    bool isSOTrx = "Y".Equals(dr[3].ToString());
                    if (!isSOTrx)
                    {
                        PayAmt = Decimal.Negate(PayAmt);
                        DiscountAmt = Decimal.Negate(DiscountAmt);
                    }
                    //
                    SetAmount(Decimal.Subtract(PayAmt, DiscountAmt));
                    SetDiscountAmt(DiscountAmt);
                    SetWriteOffAmt(Env.ZERO);
                    //p_changeVO.setContext(getCtx(), windowNo, "InvTotalAmt", PayAmt);
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally { dt = null; }
        }

        /**
         * 	Set Order - no discount
         *	@param order order
         *	@param trxName transaction
         */
        public void SetOrder(MVABOrder order, Trx trxName)
        {
            SetCashType(CASHTYPE_Invoice);
            SetVAB_Currency_ID(order.GetVAB_Currency_ID());
            //	Amount
            Decimal amt = order.GetGrandTotal();
            SetAmount(amt);
            SetDiscountAmt(Env.ZERO);
            SetWriteOffAmt(Env.ZERO);
            SetIsGenerated(true);
            //
            if (MVABOrder.DOCSTATUS_WaitingPayment.Equals(order.GetDocStatus()))
            {
                Save(trxName);
                order.SetVAB_CashJRNLLine_ID(GetVAB_CashJRNLLine_ID());
                //order.ProcessIt(MOrder.ACTION_WaitComplete);
                order.ProcessIt(DocActionVariables.ACTION_WAITCOMPLETE);
                order.Save(trxName);
                //	Set Invoice
                MVABInvoice[] invoices = order.GetInvoices(true);
                int length = invoices.Length;
                if (length > 0)		//	get last invoice
                {
                    _invoice = invoices[length - 1];
                    SetVAB_Invoice_ID(_invoice.GetVAB_Invoice_ID());
                }
            }
        }

        /**
         * 	Set Amount - Callout
         *	@param oldAmount old value
         *	@param newAmount new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout 
        public void SetAmount(String oldAmount, String newAmount, int windowNo)
        {
            if (newAmount == null || newAmount.Length == 0)
                return;
            Decimal Amount = Convert.ToDecimal(newAmount);
            base.SetAmount(Amount);
            SetAmt(windowNo, "Amount");
        }

        // Added by Amit 31-7-2015 VAMRP
        /// <summary>
        /// Return Amount
        /// </summary>
        /// <param name="RETURNLOANAMOUNT"></param>
        //public void SetRETURNLOANAMOUNT(Decimal? RETURNLOANAMOUNT)
        //{
        //    base.SetRETURNLOANAMOUNT(RETURNLOANAMOUNT == null ? Env.ZERO : (Decimal)RETURNLOANAMOUNT);
        //}

        //public void SetLOAN(bool LOAN)
        //{
        //    base.SetLOAN(LOAN == null ? true : LOAN);
        //}
        // End Amit

        /**
         * 	Set WriteOffAmt - Callout
         *	@param oldWriteOffAmt old value
         *	@param newWriteOffAmt new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout
        public void SetWriteOffAmt(String oldWriteOffAmt, String newWriteOffAmt, int windowNo)
        {
            if (newWriteOffAmt == null || newWriteOffAmt.Length == 0)
                return;
            Decimal WriteOffAmt = Convert.ToDecimal(newWriteOffAmt);
            base.SetWriteOffAmt(WriteOffAmt);
            SetAmt(windowNo, "WriteOffAmt");
        }

        /**
         * 	Set DiscountAmt - Callout
         *	@param oldDiscountAmt old value
         *	@param newDiscountAmt new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout
        public void SetDiscountAmt(String oldDiscountAmt, String newDiscountAmt, int windowNo)
        {
            if (newDiscountAmt == null || newDiscountAmt.Length == 0)
                return;
            Decimal DiscountAmt = Convert.ToDecimal(newDiscountAmt);
            base.SetDiscountAmt(DiscountAmt);
            SetAmt(windowNo, "DiscountAmt");
        }

        /**
         * 	Set Amount or WriteOffAmt for Invoices
         *	@param windowNo window
         *	@param columnName source column
         */
        private void SetAmt(int windowNo, String columnName)
        {
            //  Needs to be Invoice
            if (!CASHTYPE_Invoice.Equals(GetCashType()))
                return;
            //  Check, if InvTotalAmt exists
            String total = GetCtx().GetContext(windowNo, "InvTotalAmt");
            if (total == null || total.Length == 0)
                return;
            Decimal InvTotalAmt = Convert.ToDecimal(total);

            Decimal PayAmt = GetAmount();
            Decimal DiscountAmt = GetDiscountAmt();
            Decimal WriteOffAmt = GetWriteOffAmt();
            log.Fine(columnName + " - Invoice=" + InvTotalAmt
                + " - Amount=" + PayAmt + ", Discount=" + DiscountAmt + ", WriteOff=" + WriteOffAmt);

            //  Amount - calculate write off
            if (columnName.Equals("Amount"))
            {
                WriteOffAmt = Decimal.Subtract(InvTotalAmt, Decimal.Subtract(PayAmt, DiscountAmt));
                SetWriteOffAmt(WriteOffAmt);
            }
            else    //  calculate PayAmt
            {
                PayAmt = Decimal.Subtract(InvTotalAmt, Decimal.Subtract(DiscountAmt, WriteOffAmt));
                SetAmount(PayAmt);
            }
        }

        /**
         * 	Get Statement Date from header 
         *	@return date
         */
        public DateTime? GetStatementDate()
        {
            return GetParent().GetStatementDate();
        }

        /**
         * 	Create Line Reversal
         *	@return new reversed CashLine
         */
        public MVABCashJRNLLine CreateReversal()
        {
            MVABCashJRNL parent = GetParent();
            if (parent.IsProcessed())
            {	//	saved
                parent = MVABCashJRNL.Get(GetCtx(), parent.GetVAF_Org_ID(),
                    parent.GetStatementDate(), parent.GetVAB_Currency_ID(), Get_TrxName());
            }
            //
            MVABCashJRNLLine reversal = new MVABCashJRNLLine(parent);
            reversal.SetClientOrg(this);
            reversal.SetVAB_Bank_Acct_ID(GetVAB_Bank_Acct_ID());
            reversal.SetVAB_Charge_ID(GetVAB_Charge_ID());
            reversal.SetVAB_Currency_ID(GetVAB_Currency_ID());
            reversal.SetVAB_Invoice_ID(GetVAB_Invoice_ID());
            reversal.SetCashType(GetCashType());
            reversal.SetDescription(GetDescription());
            reversal.SetIsGenerated(true);
            //
            reversal.SetAmount(Decimal.Negate(GetAmount()));
            //if (GetDiscountAmt() == null)
            ////    SetDiscountAmt(Env.ZERO);
            //else
            reversal.SetDiscountAmt(Decimal.Negate(GetDiscountAmt()));
            //if (GetWriteOffAmt() == null)
            //    SetWriteOffAmt(Env.ZERO);
            //else
            reversal.SetWriteOffAmt(Decimal.Negate(GetWriteOffAmt()));
            reversal.AddDescription("(" + GetLine() + ")");
            return reversal;
        }


        /**
         * 	Get Cash (parent)
         *	@return cash
         */
        public MVABCashJRNL GetParent()
        {
            if (_parent == null)
                _parent = new MVABCashJRNL(GetCtx(), GetVAB_CashJRNL_ID(), Get_TrxName());
            return _parent;
        }

        /**
         * 	Get CashBook
         *	@return cash book
         */
        public MVABCashBook GetCashBook()
        {
            if (_cashBook == null)
                _cashBook = MVABCashBook.Get(GetCtx(), GetParent().GetVAB_CashBook_ID());
            return _cashBook;
        }

        /**
         * 	Get Bank Account
         *	@return bank account
         */
        public MVABBankAcct GetBankAccount()
        {
            if (_bankAccount == null && GetVAB_Bank_Acct_ID() != 0)
                _bankAccount = MVABBankAcct.Get(GetCtx(), GetVAB_Bank_Acct_ID());
            return _bankAccount;
        }

        /**
         * 	Get Invoice
         *	@return invoice
         */
        public MVABInvoice GetInvoice()
        {
            if (_invoice == null && GetVAB_Invoice_ID() != 0)
                _invoice = MVABInvoice.Get(GetCtx(), GetVAB_Invoice_ID());
            return _invoice;
        }

        /**
         * 	Before Delete
         *	@return true/false
         */
        protected override bool BeforeDelete()
        {
            //	Cannot Delete generated Invoices
            Boolean? generated = (Boolean?)Get_ValueOld("IsGenerated");
            if (generated != null && generated.Value)
            {
                if (Get_ValueOld("VAB_Invoice_ID") != null)
                {
                    log.Warning("Cannot delete line with generated Invoice");
                    return false;
                }
            }
            return true;
        }

        /**
         * 	After Delete
         *	@param success
         *	@return true/false
         */
        protected override bool AfterDelete(bool success)
        {
            if (!success)
                return success;
            return UpdateCbAndLine();
            //return UpdateHeader();
        }



        /**
         * 	Before Save
         *	@param newRecord
         *	@return true/false
         */
        protected override bool BeforeSave(bool newRecord)
        {
            // Added by Amit 1-8-2015 VAMRP
            //if (Env.HasModulePrefix("VAMRP_", out mInfo))
            //{
            //    //for kc
            //    //charge
            //    if (GetCashType() == "C")
            //    {
            //        SetVAB_Invoice_ID(0);
            //        SetDiscountAmt(0);
            //        SetWriteOffAmt(0);
            //        SetVAB_Bank_Acct_ID(0);
            //    }
            //    //invoice
            //    if (GetCashType() == "I")
            //    {
            //        SetVAB_BusinessPartner_ID(0);
            //        SetVAB_Charge_ID(0);
            //        SetVAB_Bank_Acct_ID(0);
            //    }
            //    //bank a/c transfer
            //    if (GetCashType() == "T")
            //    {
            //        SetVAB_Invoice_ID(0);
            //        SetDiscountAmt(0);
            //        SetWriteOffAmt(0);
            //        SetVAB_BusinessPartner_ID(0);
            //        SetVAB_Charge_ID(0);
            //    }
            //    //genral expense
            //    if (GetCashType() == "E")
            //    {
            //        SetVAB_Invoice_ID(0);
            //        SetDiscountAmt(0);
            //        SetWriteOffAmt(0);
            //        SetVAB_BusinessPartner_ID(0);
            //        SetVAB_Charge_ID(0);
            //        SetVAB_Bank_Acct_ID(0);
            //    }
            //    //genral receipt
            //    if (GetCashType() == "R")
            //    {
            //        SetVAB_Invoice_ID(0);
            //        SetDiscountAmt(0);
            //        SetWriteOffAmt(0);
            //        SetVAB_BusinessPartner_ID(0);
            //        SetVAB_Charge_ID(0);
            //        SetVAB_Bank_Acct_ID(0);
            //    }
            //    //differennce
            //    if (GetCashType() == "D")
            //    {
            //        SetVAB_Invoice_ID(0);
            //        SetDiscountAmt(0);
            //        SetWriteOffAmt(0);
            //        SetVAB_BusinessPartner_ID(0);
            //        SetVAB_Charge_ID(0);
            //        SetVAB_Bank_Acct_ID(0);
            //    }
            //}
            // End

            //	Cannot change generated Invoices
            if (Is_ValueChanged("VAB_Invoice_ID"))
            {
                Object generated = Get_ValueOld("IsGenerated");
                if (generated != null && ((Boolean)generated))
                {
                    log.Warning("Cannot change line with generated Invoice");
                    return false;
                }

               //JID_0615_1 prevent saving record if conversion not found
                if (Util.GetValueOfDecimal(Get_Value("ConvertedAmt")) == 0 ) { 
                    log.SaveError(Msg.GetMsg(GetCtx(), "NoConversion"), "");
                    return false;
                };
                
            }

            // during saving a new record, system will check same invoice schedule reference exist on same cash line or not
            if (newRecord && GetCashType() == CASHTYPE_Invoice && GetVAB_sched_InvoicePayment_ID() > 0)
            {
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM VAB_CashJRNLLine WHERE VAB_CashJRNL_ID = " + GetVAB_CashJRNL_ID() +
                          @" AND IsActive = 'Y' AND VAB_sched_InvoicePayment_ID = " + GetVAB_sched_InvoicePayment_ID(), null, Get_Trx())) > 0)
                {
                    log.SaveError("Error", Msg.GetMsg(GetCtx(), "VIS_NotSaveDuplicateRecord"));
                    return false;
                }

            }

            // check schedule is hold or not, if hold then no to save record
            if (Get_ColumnIndex("VAB_sched_InvoicePayment_ID") >= 0 && GetVAB_sched_InvoicePayment_ID() > 0)
            {
                if (IsHoldpaymentSchedule(GetVAB_sched_InvoicePayment_ID()))
                {
                    log.SaveError("", Msg.GetMsg(GetCtx(), "VIS_PaymentisHold"));
                    return false;
                }
            }

            //	Verify CashType
            if (CASHTYPE_Invoice.Equals(GetCashType()) && GetVAB_Invoice_ID() == 0)
                SetCashType(CASHTYPE_GeneralExpense);
            if (CASHTYPE_BankAccountTransfer.Equals(GetCashType()) && GetVAB_Bank_Acct_ID() == 0)
                SetCashType(CASHTYPE_GeneralExpense);
            if (CASHTYPE_Charge.Equals(GetCashType()) && GetVAB_Charge_ID() == 0)
                SetCashType(CASHTYPE_GeneralExpense);

            // JID_1244: On Save of record need to check cash journal account date with check date. If check date is greater than account data need to give error as on payment window.
            if (CASHTYPE_BankAccountTransfer.Equals(GetCashType()))
            {
                if (GetCheckDate() != null)
                {
                    if (GetCheckDate().Value.Date > GetParent().GetDateAcct().Value.Date)
                    {
                        log.SaveError("Error", Msg.GetMsg(GetCtx(), "VIS_CheckDateCantbeGreaterSys"));
                        return false;
                    }
                }
            }

            bool verify = newRecord
                || Is_ValueChanged("CashType")
                || Is_ValueChanged("VAB_Invoice_ID")
                || Is_ValueChanged("VAB_Bank_Acct_ID");
            if (verify)
            {
                //	Verify Currency
                if (CASHTYPE_BankAccountTransfer.Equals(GetCashType()))
                    SetVAB_Currency_ID(GetBankAccount().GetVAB_Currency_ID());
                else if (CASHTYPE_Invoice.Equals(GetCashType()))
                {
                    // Added by Amit 1-8-2015 VAMRP
                    if (Env.HasModulePrefix("VAMRP_", out mInfo))
                    {
                        // SetVAB_Currency_ID(GetInvoice().GetVAB_Currency_ID());
                    }
                    else
                    {
                        // Commented To Get the Invoice Open Amount Right in case of Diff Currencies
                        // SetVAB_Currency_ID(GetInvoice().GetVAB_Currency_ID());
                    }
                    //end
                }
                // Added by Bharat on 16/12/2016 handle the case of Multicurrency in Cash Book Transfer
                else if (CASHTYPE_CashBookTransfer.Equals(GetCashType()))
                {

                }
                else if (CASHTYPE_CashRecievedFrom.Equals(GetCashType()))
                {

                }
                else	//	Cash 
                    SetVAB_Currency_ID(GetCashBook().GetVAB_Currency_ID());

                //	Set Organization
                if (CASHTYPE_BankAccountTransfer.Equals(GetCashType()))
                    SetVAF_Org_ID(GetBankAccount().GetVAF_Org_ID());
                //	Cash Book
                else if (CASHTYPE_Invoice.Equals(GetCashType()))
                    SetVAF_Org_ID(GetCashBook().GetVAF_Org_ID());
                //	otherwise (charge) - leave it
                //	Enforce Org
                if (GetVAF_Org_ID() == 0)
                    SetVAF_Org_ID(GetParent().GetVAF_Org_ID());
            }

            /**	General fix of Currency 
            UPDATE VAB_CashJRNLLine cl SET VAB_Currency_ID = (SELECT VAB_Currency_ID FROM VAB_Invoice i WHERE i.VAB_Invoice_ID=cl.VAB_Invoice_ID) WHERE VAB_Currency_ID IS NULL AND VAB_Invoice_ID IS NOT NULL;
            UPDATE VAB_CashJRNLLine cl SET VAB_Currency_ID = (SELECT VAB_Currency_ID FROM VAB_Bank_Acct b WHERE b.VAB_Bank_Acct_ID=cl.VAB_Bank_Acct_ID) WHERE VAB_Currency_ID IS NULL AND VAB_Bank_Acct_ID IS NOT NULL;
            UPDATE VAB_CashJRNLLine cl SET VAB_Currency_ID = (SELECT b.VAB_Currency_ID FROM VAB_CashJRNL c, VAB_CashBook b WHERE c.VAB_CashJRNL_ID=cl.VAB_CashJRNL_ID AND c.VAB_CashBook_ID=b.VAB_CashBook_ID) WHERE VAB_Currency_ID IS NULL;
            **/

            //	Get Line No
            if (GetLine() == 0)
            {
                String sql = "SELECT COALESCE(MAX(Line),0)+10 FROM VAB_CashJRNLLine WHERE VAB_CashJRNL_ID=@param1";
                int ii = DB.GetSQLValue(Get_TrxName(), sql, GetVAB_CashJRNL_ID());
                SetLine(ii);
            }

            // JID_1326: Voucher number would be pick from document number from header Hyphen line number
            if (String.IsNullOrEmpty(GetVSS_RECEIPTNO()))
            {
                SetVSS_RECEIPTNO(Util.GetValueOfString(GetParent().Get_Value("DocumentNo")) + "-" + GetLine());
            }

            // Added by Amit 1-8-2015 VAMRP
            //if (Env.HasModulePrefix("VAMRP_", out mInfo))
            //{
            //    if (GetVSS_RECEIPTNO() == null || GetVSS_RECEIPTNO() == "")
            //    {
            //        MOrg mo = new MOrg(GetCtx(), GetVAF_Org_ID(), Get_TrxName());
            //        String org_name = mo.GetName();
            //        //modified by ashish.bisht on 04-feb-10
            //        String paymenttype = GetVSS_PAYMENTTYPE();
            //        String test_name = "DocNo_" + org_name + "_" + paymenttype;

            //        int[] s = MSequence.GetAllIDs("VAF_Record_Seq", "Name= '" + test_name + "'", Get_TrxName());

            //        if (s != null && s.Length != 0)
            //        {
            //            MSequence sqq = new MSequence(GetCtx(), s[0], Get_TrxName());
            //            String ss = sqq.GetName();

            //            if (ss.Equals(test_name))
            //            {
            //                int inc = sqq.GetIncrementNo();
            //                String pre = sqq.GetPrefix();
            //                String suff = sqq.GetSuffix();

            //                int curr = sqq.GetCurrentNext();
            //                curr = curr + inc;
            //                sqq.SetCurrentNext(curr);
            //                sqq.Save();
            //                String StrCurr = "" + curr;

            //                if (pre == null && suff == null)
            //                {
            //                    SetVSS_RECEIPTNO(StrCurr);
            //                }
            //                if (pre != null && suff == null)
            //                {
            //                    SetVSS_RECEIPTNO(pre + StrCurr);
            //                }
            //                if (pre == null && suff != null)
            //                {
            //                    SetVSS_RECEIPTNO(StrCurr + suff);
            //                }

            //                if (pre != null && suff != null)
            //                {
            //                    SetVSS_RECEIPTNO(pre + StrCurr + suff);
            //                }
            //            }
            //        }
            //    }
            //}
            //End
            if (CASHTYPE_BusinessPartner.Equals(GetCashType()) && VSS_PAYMENTTYPE_PaymentReturn.Equals(GetVSS_PAYMENTTYPE()) && GetAmount() < 0)
            {
                SetAmount(Math.Abs(GetAmount()));
            }
            else if (CASHTYPE_BusinessPartner.Equals(GetCashType()) && VSS_PAYMENTTYPE_ReceiptReturn.Equals(GetVSS_PAYMENTTYPE()) && GetAmount() > 0)
            {
                SetAmount(Decimal.Negate(GetAmount()));
            }

            // Reset Amount Dimension if Amount is different
            if (Util.GetValueOfInt(Get_Value("AmtDimAmount")) > 0)
            {
                string qry = "SELECT Amount FROM VAB_DimAmt WHERE VAB_DimAmt_ID=" + Util.GetValueOfInt(Get_Value("AmtDimAmount"));
                decimal amtdimAmt = Util.GetValueOfDecimal(DB.ExecuteScalar(qry, null, Get_TrxName()));

                if (amtdimAmt != GetAmount())
                {
                    resetAmtDim = true;
                    Set_Value("AmtDimAmount", null);
                }
            }

            return true;
        }

        /// <summary>
        /// Is used to get Invoice payment schedule is Hold payment or not
        /// </summary>
        /// <param name="VAB_sched_InvoicePayment_ID">Invoice payment schedule reference</param>
        /// <returns>TRUE, if hold payment</returns>
        public bool IsHoldpaymentSchedule(int VAB_sched_InvoicePayment_ID)
        {
            try
            {
                String sql = "SELECT IsHoldPayment FROM VAB_sched_InvoicePayment WHERE VAB_sched_InvoicePayment_ID = " + VAB_sched_InvoicePayment_ID;
                String IsHoldPayment = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_Trx()));
                return IsHoldPayment.Equals("Y");
            }
            catch
            {
                // when column not found, mean hold payment functionlity not in system
                return false;
            }
        }

        /**
       * 	After Save
       *	@param newRecord
       *	@param success
       *	@return success
       */
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success)
                return success;

            if (!UpdateCbAndLine())
                return false;

            if (GetVSS_PAYMENTTYPE() == X_VAB_CashJRNLLine.VSS_PAYMENTTYPE_Payment && GetVAB_BusinessPartner_ID() > 0)
            {
                MVABCashJRNL csh = new MVABCashJRNL(GetCtx(), GetVAB_CashJRNL_ID(), Get_TrxName());
                Decimal amt = MVABExchangeRate.ConvertBase(GetCtx(), GetAmount(),	//	CM adjusted 
                    GetVAB_Currency_ID(), csh.GetDateAcct(), 0, GetVAF_Client_ID(), GetVAF_Org_ID());

                MVABBusinessPartner bp = new MVABBusinessPartner(GetCtx(), GetVAB_BusinessPartner_ID(), Get_Trx());
                string retMsg = "";
                bool crdAll = bp.IsCreditAllowed(GetVAB_BPart_Location_ID(), Decimal.Subtract(0, amt), out retMsg);
                if (!crdAll)
                    log.SaveWarning("Warning", retMsg);
                else if (bp.IsCreditWatch(GetVAB_BPart_Location_ID()))
                    log.SaveWarning("Warning", Msg.GetMsg(GetCtx(), "VIS_BPCreditWatch"));
            }

            return true;
        }

        private bool UpdateCbAndLine()
        {
            // Update Cash Journal
            if (!UpdateHeader())
            {
                log.Warning("Cannot update cash journal.");
                return false;
            }

            // Update Cashbook and CashbookLine
            MVABCashJRNL parent = GetParent();
            MVABCashBook cashbook = new MVABCashBook(GetCtx(), parent.GetVAB_CashBook_ID(), Get_TrxName());
            if (cashbook.GetCompletedBalance() == 0)
            {
                cashbook.SetCompletedBalance(parent.GetBeginningBalance());
            }
            cashbook.SetRunningBalance(Decimal.Add(Decimal.Subtract(cashbook.GetRunningBalance(), old_ebAmt), new_ebAmt));
            //if (cashbook.GetRunningBalance() == 0)
            //{
            //    cashbook.SetRunningBalance
            //        (Decimal.Add(Decimal.Add(Decimal.Subtract(cashbook.GetRunningBalance(), old_ebAmt), new_ebAmt),cashbook.GetCompletedBalance()));
            //}
            //else
            //{
            //    cashbook.SetRunningBalance(Decimal.Add(Decimal.Subtract(cashbook.GetRunningBalance(), old_ebAmt), new_ebAmt));
            //}

            if (!cashbook.Save())
            {
                log.Warning("Cannot update running balance.");
                return false;
            }

            DataTable dtCashbookLine;
            int VAB_CASHBOOKLINE_ID = 0;

            string sql = "SELECT VAB_CASHBOOKLINE_ID FROM VAB_CASHBOOKLINE WHERE VAB_CASHBOOK_ID="
                            + cashbook.GetVAB_CashBook_ID() + " AND DATEACCT="
                            + DB.TO_DATE(parent.GetDateAcct()) + " AND VAF_ORG_ID=" + GetVAF_Org_ID();

            dtCashbookLine = DB.ExecuteDataset(sql, null, null).Tables[0];

            if (dtCashbookLine.Rows.Count > 0)
            {
                VAB_CASHBOOKLINE_ID = Util.GetValueOfInt(dtCashbookLine.Rows[0]
                    .ItemArray[0]);
            }

            MVABCashbookLine cashbookLine = new MVABCashbookLine(GetCtx(), VAB_CASHBOOKLINE_ID, Get_TrxName());

            if (VAB_CASHBOOKLINE_ID == 0)
            {
                cashbookLine.SetVAB_CashBook_ID(cashbook.GetVAB_CashBook_ID());
                // SI_0419 : Update org/client as on cash line
                cashbookLine.SetVAF_Org_ID(GetVAF_Org_ID());
                cashbookLine.SetVAF_Client_ID(GetVAF_Client_ID());
                cashbookLine.SetEndingBalance
                    (Decimal.Add(Decimal.Add(Decimal.Subtract(cashbookLine.GetEndingBalance(), old_ebAmt), new_ebAmt), cashbook.GetCompletedBalance()));
            }
            else
            {
                cashbookLine.SetEndingBalance(Decimal.Add(Decimal.Subtract(cashbookLine.GetEndingBalance(), old_ebAmt), new_ebAmt));
            }
            cashbookLine.SetDateAcct(parent.GetDateAcct());
            cashbookLine.SetStatementDifference(Decimal.Add(Decimal.Subtract(cashbookLine.GetStatementDifference(), old_sdAmt), new_sdAmt));


            if (!cashbookLine.Save())
            {
                log.Warning("Cannot create/update cashbook line.");
                return false;
            }

            return true;
        }

        /**
         * 	Update Cash Header.
         * 	Statement Difference, Ending Balance
         *	@return true if success
         */
        private bool UpdateHeader()
        {
            /* jz re-write this SQL because SQL Server doesn't like it
            String sql = "UPDATE VAB_CashJRNL c"
                + " SET StatementDifference="
                    + "(SELECT COALESCE(SUM(currencyConvert(cl.Amount, cl.VAB_Currency_ID, cb.VAB_Currency_ID, c.DateAcct, ";
                    //jz null  //TODO check if 0 is OK with application logic
                    //+ DB.NULL("S", Types.INTEGER)   DB2 function wouldn't take null value for int parameter
            if (DB.isDB2())
                sql += "0";
            else
                sql += "NULL";
            sql += ", c.VAF_Client_ID, c.VAF_Org_ID)),0) "
                    + "FROM VAB_CashJRNLLine cl, VAB_CashBook cb "
                    + "WHERE cb.VAB_CashBook_ID=c.VAB_CashBook_ID"
                    + " AND cl.VAB_CashJRNL_ID=c.VAB_CashJRNL_ID) "
                + "WHERE VAB_CashJRNL_ID=" + getVAB_CashJRNL_ID();
            int no = DB.executeUpdate(sql, get_TrxName());
            if (no != 1)
                //log.warning("Difference #" + no);
                */
            String sql = "";
            if (Get_ColumnIndex("VAB_CurrencyType_ID") > 0)
            {
                sql = "SELECT COALESCE(SUM(currencyConvert(cl.Amount, cl.VAB_Currency_ID, cb.VAB_Currency_ID, c.DateAcct, cl.VAB_CurrencyType_ID"
                        + ", c.VAF_Client_ID, c.VAF_Org_ID)),0) "
                    + "FROM VAB_CashJRNLLine cl, VAB_CashBook cb, VAB_CashJRNL c "
                    + "WHERE cb.VAB_CashBook_ID=c.VAB_CashBook_ID"
                    + " AND cl.VAB_CashJRNL_ID=c.VAB_CashJRNL_ID AND "
                    + "c.VAB_CashJRNL_ID=" + GetVAB_CashJRNL_ID();
            }
            else
            {
                sql = "SELECT COALESCE(SUM(currencyConvert(cl.Amount, cl.VAB_Currency_ID, cb.VAB_Currency_ID, c.DateAcct, 0"
                            + ", c.VAF_Client_ID, c.VAF_Org_ID)),0) "
                        + "FROM VAB_CashJRNLLine cl, VAB_CashBook cb, VAB_CashJRNL c "
                        + "WHERE cb.VAB_CashBook_ID=c.VAB_CashBook_ID"
                        + " AND cl.VAB_CashJRNL_ID=c.VAB_CashJRNL_ID AND "
                        + "c.VAB_CashJRNL_ID=" + GetVAB_CashJRNL_ID();
            }
            DataTable dt = null;
            IDataReader idr = DB.ExecuteReader(sql, null, Get_TrxName());
            Decimal sum = Env.ZERO;
            try
            {
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    sum = Convert.ToDecimal(dr[0]);
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Severe(e.Message.ToString());
                return false;
            }
            finally { dt = null; }


            // Statement difference and ending balance before update in cash journal.
            DataTable dtOldValues = GetCurrentAmounts();

            if (dtOldValues.Rows.Count > 0)
            {
                old_ebAmt = Util.GetValueOfDecimal(dtOldValues.Rows[0].ItemArray[0]);
                old_sdAmt = Util.GetValueOfDecimal(dtOldValues.Rows[0].ItemArray[1]);
            }

            //	Ending Balance
            sql = "UPDATE VAB_CashJRNL"
                + " SET EndingBalance = BeginningBalance + @sum ,"
                + " StatementDifference=@sum"
                + " WHERE VAB_CashJRNL_ID=" + GetVAB_CashJRNL_ID();

            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@sum", sum);
            //DataSet ds = DB.ExecuteDataset(sql, param);

            int no = DB.ExecuteQuery(sql, param, Get_TrxName());
            if (no != 1)
            {
                log.Warning("Balance #" + no);
            }

            // Statement difference and ending balance after update in cash journal.
            DataTable dtNewValues = GetCurrentAmounts();

            if (dtOldValues.Rows.Count > 0)
            {
                new_ebAmt = Util.GetValueOfDecimal(dtNewValues.Rows[0].ItemArray[0]);
                new_sdAmt = Util.GetValueOfDecimal(dtNewValues.Rows[0].ItemArray[1]);
            }

            return no == 1;
        }

        private DataTable GetCurrentAmounts()
        {
            string sql = "SELECT ENDINGBALANCE,STATEMENTDIFFERENCE FROM VAB_CASHJRNL "
                    + "WHERE VAB_CashJRNL_ID=" + GetVAB_CashJRNL_ID();

            return DB.ExecuteDataset(sql, null, Get_TrxName()).Tables[0];
        }


        /**
        * 	Set Invoice - no discount
        *	@param invoice invoice
        */
        public void SetInvoiceMultiCurrency(MVABInvoice invoice, Decimal Amt, int VAB_Currency_ID)
        {
            SetVAB_Invoice_ID(invoice.GetVAB_Invoice_ID());
            SetCashType(CASHTYPE_Invoice);
            SetVAB_Currency_ID(VAB_Currency_ID);
            //	Amount
            MVABDocTypes dt = MVABDocTypes.Get(GetCtx(), invoice.GetVAB_DocTypes_ID());
            Decimal amt = Amt;
            if (MVABMasterDocType.DOCBASETYPE_APINVOICE.Equals(dt.GetDocBaseType())
                || MVABMasterDocType.DOCBASETYPE_ARCREDITMEMO.Equals(dt.GetDocBaseType()))
                amt = Decimal.Negate(amt);
            SetAmount(amt);
            //
            SetDiscountAmt(Env.ZERO);
            SetWriteOffAmt(Env.ZERO);
            SetIsGenerated(true);
            _invoice = invoice;
        }




    }
}
