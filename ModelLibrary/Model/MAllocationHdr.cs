/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_C_AllocationHdr
 * Chronological Development
 * Veena Pandey     23-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MAllocationHdr : X_C_AllocationHdr, DocAction
    {
        /**	Logger						*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MAllocationHdr).FullName);
        /**	Process Message 			*/
        private String _processMsg = null;
        /**	Just Prepared Flag			*/
        private bool _justPrepared = false;
        /**	Lines						*/
        private MAllocationLine[] _lines = null;
        /** PaidAmt                     */
        decimal PaidAmt = 0;
        /** variance                    */
        decimal Variance = 0;
        decimal varianceAmount = 0;
        decimal ShiftVarianceOnOther = 0;
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_AllocationHdr_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MAllocationHdr(Ctx ctx, int C_AllocationHdr_ID, Trx trxName)
            : base(ctx, C_AllocationHdr_ID, trxName)
        {
            if (C_AllocationHdr_ID == 0)
            {
                //	setDocumentNo (null);
                SetDateTrx(DateTime.Now);
                SetDateAcct(GetDateTrx());
                SetDocAction(DOCACTION_Complete);	// CO
                SetDocStatus(DOCSTATUS_Drafted);	// DR
                //	setC_Currency_ID (0);
                SetApprovalAmt(Env.ZERO);
                SetIsApproved(false);
                SetIsManual(false);
                //
                SetPosted(false);
                SetProcessed(false);
                SetProcessing(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MAllocationHdr(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Mandatory New Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="IsManual">manual trx</param>
        /// <param name="dateTrx">date (if null today)</param>
        /// <param name="C_Currency_ID">currency</param>
        /// <param name="description">description</param>
        /// <param name="trxName">transaction</param>
        public MAllocationHdr(Ctx ctx, bool IsManual, DateTime? dateTrx, int C_Currency_ID,
            String description, Trx trxName)
            : this(ctx, 0, trxName)
        {
            SetIsManual(IsManual);
            if (dateTrx != null)
            {
                SetDateTrx(dateTrx);
                SetDateAcct(dateTrx);
            }
            SetC_Currency_ID(C_Currency_ID);
            if (description != null)
                SetDescription(description);
        }

        /// <summary>
        /// Get Allocations of Payment
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Payment_ID">payment</param>
        /// <param name="trxName">transaction</param>
        /// <returns>allocations of payment</returns>
        public static MAllocationHdr[] GetOfPayment(Ctx ctx, int C_Payment_ID, Trx trxName)
        {
            String sql = "SELECT * FROM C_AllocationHdr h "
                + "WHERE IsActive='Y'"
                + " AND EXISTS (SELECT * FROM C_AllocationLine l "
                    + "WHERE h.C_AllocationHdr_ID=l.C_AllocationHdr_ID AND l.C_Payment_ID=" + C_Payment_ID + ")";
            List<MAllocationHdr> list = new List<MAllocationHdr>();
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MAllocationHdr(ctx, dr, trxName));
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            MAllocationHdr[] retValue = new MAllocationHdr[list.Count];
            retValue = list.ToArray();
            return retValue;
        }


        /// <summary>
        /// Get Allocations of Invoice
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Invoice_ID">invoice</param>
        /// <param name="trxName">transaction</param>
        /// <returns>allocations of invoice</returns>
        public static MAllocationHdr[] GetOfInvoice(Ctx ctx, int C_Invoice_ID, Trx trxName)
        {
            String sql = "SELECT * FROM C_AllocationHdr h "
                + "WHERE IsActive='Y'"
                + " AND EXISTS (SELECT * FROM C_AllocationLine l "
                    + "WHERE h.C_AllocationHdr_ID=l.C_AllocationHdr_ID AND l.C_Invoice_ID=" + C_Invoice_ID + ")";
            List<MAllocationHdr> list = new List<MAllocationHdr>();
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MAllocationHdr(ctx, dr, trxName));
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            MAllocationHdr[] retValue = new MAllocationHdr[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get Lines
        /// </summary>
        /// <param name="requery">if true requery</param>
        /// <returns>lines</returns>
        public MAllocationLine[] GetLines(bool requery)
        {
            if (_lines != null && _lines.Length != 0 && !requery)
                return _lines;
            //
            String sql = "SELECT * FROM C_AllocationLine WHERE C_AllocationHdr_ID=" + GetC_AllocationHdr_ID();
            List<MAllocationLine> list = new List<MAllocationLine>();
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        MAllocationLine line = new MAllocationLine(GetCtx(), dr, Get_TrxName());
                        line.SetParent(this);
                        list.Add(line);
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            //
            _lines = new MAllocationLine[list.Count];
            _lines = list.ToArray();
            return _lines;
        }

        /// <summary>
        /// Set Processed
        /// </summary>
        /// <param name="processed">processed</param>
        public new void SetProcessed(bool processed)
        {
            base.SetProcessed(processed);
            if (Get_ID() == 0)
                return;
            String sql = "UPDATE C_AllocationHdr SET Processed='"
                + (processed ? "Y" : "N")
                + "' WHERE C_AllocationHdr_ID=" + GetC_AllocationHdr_ID();
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            _lines = null;
            log.Fine(processed + " - #" + no);
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true if success</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            //	Changed from Not to Active
            if (!newRecord && Is_ValueChanged("IsActive") && IsActive())
            {
                log.Severe("Cannot Re-Activate deactivated Allocations");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Before Delete
        /// </summary>
        /// <returns>true if acct was deleted</returns>
        protected override bool BeforeDelete()
        {
            Trx trxName = Get_Trx();
            if (trxName == null)
            {
                log.Warning("No transaction");
            }
            if (IsPosted())
            {
                // Check Period Open
                if (!MPeriod.IsOpen(GetCtx(), GetDateTrx(), MDocBaseType.DOCBASETYPE_PAYMENTALLOCATION, GetAD_Org_ID()))
                {
                    log.Warning("Period Closed");
                    return false;
                }
                //// is Non Business Day?
                //if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetDateTrx()))
                //{
                //    log.Warning("DateIsInNonBusinessDay");
                //    return false;
                //}

                SetPosted(false);
                if (MFactAcct.Delete(Table_ID, Get_ID(), trxName) < 0)
                    return false;
            }
            //	Mark as Inactive
            SetIsActive(false);		//	updated DB for line delete/process
            String sql = "UPDATE C_AllocationHdr SET IsActive='N' WHERE C_AllocationHdr_ID=" + GetC_AllocationHdr_ID();
            DataBase.DB.ExecuteQuery(sql, null, trxName);

            //	Unlink
            GetLines(true);
            HashSet<int> bps = new HashSet<int>();
            for (int i = 0; i < _lines.Length; i++)
            {
                MAllocationLine line = _lines[i];
                bps.Add(line.GetC_BPartner_ID());
                if (!line.Delete(true, trxName))
                    return false;
            }
            UpdateBP(bps);
            return true;
        }

        /// <summary>
        /// After save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            return success;
        }

        /// <summary>
        /// Process document
        /// </summary>
        /// <param name="processAction">document action</param>
        /// <returns>true if performed</returns>
        public bool ProcessIt(String processAction)
        {
            _processMsg = null;
            DocumentEngine engine = new DocumentEngine(this, GetDocStatus());
            return engine.ProcessIt(processAction, GetDocAction());
        }

        /// <summary>
        /// Unlock Document.
        /// </summary>
        /// <returns>true if success</returns>
        public bool UnlockIt()
        {
            log.Info(ToString());
            SetProcessing(false);
            return true;
        }

        /// <summary>
        /// Invalidate Document
        /// </summary>
        /// <returns>true if success</returns>
        public bool InvalidateIt()
        {
            log.Info(ToString());
            SetDocAction(DOCACTION_Prepare);
            return true;
        }

        /// <summary>
        /// Prepare Document
        /// </summary>
        /// <returns>new status (In Progress or Invalid)</returns>
        public String PrepareIt()
        {
            log.Info(ToString());
            _processMsg = ModelValidationEngine.Get().FireDocValidate
                (this, ModalValidatorVariables.DOCTIMING_BEFORE_PREPARE);
            if (_processMsg != null)
                return DocActionVariables.STATUS_INVALID;

            //	Std Period open?
            if (!MPeriod.IsOpen(GetCtx(), GetDateAcct(), MDocBaseType.DOCBASETYPE_PAYMENTALLOCATION, GetAD_Org_ID()))
            {
                _processMsg = "@PeriodClosed@";
                return DocActionVariables.STATUS_INVALID;
            }
            // is Non Business Day?
            // JID_1205: At the trx, need to check any non business day in that org. if not fund then check * org.
            if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetDateAcct(), GetAD_Org_ID()))
            {
                _processMsg = Common.Common.NONBUSINESSDAY;
                return DocActionVariables.STATUS_INVALID;
            }

            GetLines(false);
            if (_lines.Length == 0)
            {
                _processMsg = "@NoLines@";
                return DocActionVariables.STATUS_INVALID;
            }
            //	Add up Amounts & validate
            Decimal approval = Env.ZERO;
            for (int i = 0; i < _lines.Length; i++)
            {
                MAllocationLine line = _lines[i];
                approval = Decimal.Add(Decimal.Add(approval, line.GetWriteOffAmt()), line.GetDiscountAmt());
                //	Make sure there is BP
                if (line.GetC_BPartner_ID() == 0)
                {
                    _processMsg = "No Business Partner";
                    return DocActionVariables.STATUS_INVALID;
                }
            }
            SetApprovalAmt(approval);
            //
            _justPrepared = true;
            if (!DOCACTION_Complete.Equals(GetDocAction()))
                SetDocAction(DOCACTION_Complete);
            return DocActionVariables.STATUS_INPROGRESS;
        }

        /// <summary>
        /// Approve Document
        /// </summary>
        /// <returns>true if success</returns>
        public bool ApproveIt()
        {
            log.Info(ToString());
            SetIsApproved(true);
            return true;
        }

        /// <summary>
        /// Reject Approval
        /// </summary>
        /// <returns>true if success</returns>
        public bool RejectIt()
        {
            log.Info(ToString());
            SetIsApproved(false);
            return true;
        }

        /// <summary>
        /// Complete Document
        /// </summary>
        /// <returns>new status (Complete, In Progress, Invalid, Waiting ..)</returns>
        public String CompleteIt()
        {
            //	Re-Check
            if (!_justPrepared)
            {
                String status = PrepareIt();
                if (!DocActionVariables.STATUS_INPROGRESS.Equals(status))
                    return status;
            }
            //	Implicit Approval
            if (!IsApproved())
                ApproveIt();
            log.Info(ToString());


            int _invoice_id = 0;
            int _invoicePaySchedule_ID = 0;
            List<int> invoiceIds = new List<int>();
            //	Link
            GetLines(false);
            HashSet<int> bps = new HashSet<int>();
            for (int i = 0; i < _lines.Length; i++)
            {
                MAllocationLine line = _lines[i];
                bps.Add(line.ProcessIt(false));	//	not reverse

                // change by Amit for Payment Management 5-11-2015
                //if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA009_'  AND IsActive = 'Y'")) > 0)
                if (Env.IsModuleInstalled("VA009_"))
                {
                    if (line.GetC_Invoice_ID() > 0 && line.GetC_InvoicePaySchedule_ID() > 0)
                    {
                        log.Info("Start setting value of Paid Amount on Invoice Schedule");
                        //VA228:Get invoice ids
                        if (!invoiceIds.Contains(Util.GetValueOfInt(line.GetC_Invoice_ID())))
                            invoiceIds.Add(Util.GetValueOfInt(line.GetC_Invoice_ID()));

                        //check no of schedule left for Payment for Currenct Invoice except this schedule
                        int countUnPaidSchedule = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(C_InvoicePaySchedule_ID) FROM C_InvoicePaySchedule WHERE IsActive = 'Y' AND 
                                                   VA009_IsPaid = 'N' AND C_Invoice_ID = " + line.GetC_Invoice_ID() +
                                                   @" AND C_InvoicePaySchedule_ID <> " + line.GetC_InvoicePaySchedule_ID(), null, Get_Trx()));

                        MInvoicePaySchedule paySch = new MInvoicePaySchedule(GetCtx(), line.GetC_InvoicePaySchedule_ID(), Get_Trx());
                        if (paySch.IsVA009_IsPaid())
                            continue;
                        //// Added by Bharat on 27 June 2017 to restrict multiple payment against same Invoice Pay Schedule.
                        //if (paySch.IsVA009_IsPaid())
                        //{
                        //    _processMsg = "Payment is already done for selected invoice Schedule";
                        //    return DocActionVariables.STATUS_INVALID;
                        //}
                        MInvoice invoice = new MInvoice(GetCtx(), line.GetC_Invoice_ID(), Get_Trx());
                        MCurrency currency = MCurrency.Get(GetCtx(), invoice.GetC_Currency_ID());
                        MDocType doctype = MDocType.Get(GetCtx(), invoice.GetC_DocType_ID());
                        //not getting DocType while creating payment from POS Order Tyepe because in get method transaction is not passed in parameter
                        if (doctype.GetC_DocType_ID() == 0)
                        {
                            doctype = MDocType.Get(GetCtx(), invoice.GetC_DocTypeTarget_ID());
                        }
                        StringBuilder _sql = new StringBuilder();
                        varianceAmount = 0;


                        // _sql.Clear();
                        //_sql.Append("SELECT VA009_PaidAmntInvce FROM c_invoicepayschedule WHERE c_invoicepayschedule_id = " + line.GetC_InvoicePaySchedule_ID());
                        //decimal paidInvoiceAmount = Util.GetValueOfDecimal(DB.ExecuteScalar(_sql.ToString(), null, null));
                        decimal paidInvoiceAmount = paySch.GetVA009_PaidAmntInvce();
                        ShiftVarianceOnOther = 0;

                        _sql.Clear();
                        //_sql.Append(@"SELECT DISTINCT asch.C_Currency_ID FROM c_acctschema asch INNER JOIN ad_clientinfo ci ON ci.c_acctschema1_id = asch.c_acctschema_id
                        //         INNER JOIN ad_client c ON c.ad_client_id = ci.ad_client_id INNER JOIN c_invoice i ON c.ad_client_id    = i.ad_client_id
                        //         WHERE i.ad_client_id = " + invoice.GetAD_Client_ID());
                        //int BaseCurrency = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString(), null, null));

                        int BaseCurrency = GetCtx().GetContextAsInt("$C_Currency_ID");

                        #region set Invoice Paid Amount
                        //added check for payment and cash if cash/payment exist than create object otherwise that will be null
                        MPayment payment = null;
                        if (line.GetC_Payment_ID() > 0)
                            payment = new MPayment(GetCtx(), line.GetC_Payment_ID(), Get_Trx());

                        MCashLine cashline = null;
                        if (line.GetC_CashLine_ID() > 0)
                            cashline = new MCashLine(GetCtx(), line.GetC_CashLine_ID(), Get_Trx());

                        // in case of GL Allocation if GL_JournalLine_ID is available on Allocation Line
                        MJournalLine journalline = null;
                        if (Util.GetValueOfInt(line.Get_Value("GL_JournalLine_ID")) > 0)
                        {
                            journalline = new MJournalLine(GetCtx(), Util.GetValueOfInt(line.Get_Value("GL_JournalLine_ID")), Get_Trx());
                        }

                        decimal currencymultiplyRate = 1;
                        if (payment != null && payment.GetC_Payment_ID() != 0)
                        {
                            // to get Multiply Rate 
                            currencymultiplyRate = GetCurrencyMultiplyRate(invoice, payment, cashline, journalline);
                        }
                        else if (cashline != null && cashline.GetC_CashLine_ID() != 0)
                        {
                            // to get Multiply Rate 
                            currencymultiplyRate = GetCurrencyMultiplyRate(invoice, payment, cashline, journalline);
                        }
                        // When allocation is against invoice - invoice and payment - payment
                        else
                        {
                            currencymultiplyRate = GetCurrencyMultiplyRate(invoice, payment, cashline, journalline);
                        }


                        // set paid amount after checking if invoice is duplicate in next line added by VIvek on 02/02/2018
                        bool _isDuplicateLine = false;
                        if (line.GetC_Invoice_ID() == _invoice_id && line.GetC_InvoicePaySchedule_ID() == _invoicePaySchedule_ID)
                        {
                            _isDuplicateLine = true;
                        }
                        _invoice_id = line.GetC_Invoice_ID();
                        _invoicePaySchedule_ID = line.GetC_InvoicePaySchedule_ID();
                        // new function to set variance amount added by Vivek on 02/02/2018
                        SetVariance(line, countUnPaidSchedule, paySch, currencymultiplyRate, doctype, currency, _isDuplicateLine);

                        // check varaince amount is less than 0 then apply negate on that
                        if (paySch.GetVA009_Variance() < 0)
                        {
                            //paySch.SetVA009_Variance(Decimal.Negate(paySch.GetVA009_Variance()));
                        }

                        // set Backup Withholding amount and withholding Amount
                        if (payment != null && payment.GetC_Payment_ID() != 0 && line.Get_ColumnIndex("WithholdingAmt") > 0)
                        {
                            if (doctype.GetDocBaseType().Equals(MDocBaseType.DOCBASETYPE_ARCREDITMEMO) || doctype.GetDocBaseType().Equals(MDocBaseType.DOCBASETYPE_APINVOICE))
                            {
                                paySch.SetWithholdingAmt(Decimal.Round(Decimal.Multiply(Decimal.Negate(line.GetWithholdingAmt()), currencymultiplyRate), currency.GetStdPrecision()));
                                paySch.SetBackupWithholdingAmount(Decimal.Round(Decimal.Multiply(Decimal.Negate(line.GetBackupWithholdingAmount()), currencymultiplyRate), currency.GetStdPrecision()));
                            }
                            else
                            {
                                paySch.SetWithholdingAmt(Decimal.Round(Decimal.Multiply(line.GetWithholdingAmt(), currencymultiplyRate), currency.GetStdPrecision()));
                                paySch.SetBackupWithholdingAmount(Decimal.Round(Decimal.Multiply(line.GetBackupWithholdingAmount(), currencymultiplyRate), currency.GetStdPrecision()));
                            }
                        }
                        #endregion

                        #region set Base Paid Amount
                        decimal multiplyRate = 0;
                        if (BaseCurrency != invoice.GetC_Currency_ID())
                        {
                            // get Conversion on the basis of Allocation Date Account because Paid Amount need to be convert on DateAcct of Allocation
                            multiplyRate = MConversionRate.GetRate(invoice.GetC_Currency_ID(), BaseCurrency, invoice.GetDateAcct(), invoice.GetC_ConversionType_ID(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID());
                            paySch.SetVA009_PaidAmnt(Decimal.Round(paySch.GetVA009_PaidAmntInvce() * multiplyRate, currency.GetStdPrecision()));
                        }
                        else
                        {
                            paySch.SetVA009_PaidAmnt(Decimal.Round(paySch.GetVA009_PaidAmntInvce(), currency.GetStdPrecision()));
                        }
                        #endregion

                        // reduce over under amount from the due amount if available
                        if (line.GetOverUnderAmt() > 0)
                        {
                            //when over under amount is +ve
                            paySch.SetDueAmt(decimal.Round(decimal.Add(decimal.Subtract(paySch.GetDueAmt(), decimal.Multiply(line.GetOverUnderAmt(), currencymultiplyRate)), varianceAmount), currency.GetStdPrecision()));
                        }
                        else if (line.GetOverUnderAmt() < 0)
                        {
                            //when over under amount is -ve
                            paySch.SetDueAmt(decimal.Round(decimal.Add(decimal.Add(paySch.GetDueAmt(), Decimal.Multiply(line.GetOverUnderAmt(), currencymultiplyRate)), varianceAmount), currency.GetStdPrecision()));
                        }


                        // when paid amount against invoice = due amount on schedule then make invoice schedule as Paid 
                        //or wjem last record and due amount = variance amount + paid invoice amount
                        if (((paySch.GetVA009_PaidAmntInvce()) >= paySch.GetDueAmt()) ||
                            (countUnPaidSchedule == 0 && line.GetOverUnderAmt() == 0 &&
                            Decimal.Add(paySch.GetVA009_PaidAmntInvce(), paySch.GetVA009_Variance()) == paySch.GetDueAmt()))
                        {
                            paySch.SetVA009_IsPaid(true);
                        }
                        else
                        {
                            paySch.SetVA009_IsPaid(false);
                        }
                        paySch.SetC_Payment_ID(line.GetC_Payment_ID());

                        // work done to set cashline ID on payschedule tab of invoice if allocation done by cash
                        paySch.SetC_CashLine_ID(line.GetC_CashLine_ID());

                        // when schedule is paid from cash journal then Payment Execution status = Received
                        // when schedule is paid from Payment then Payment Execution status = In-Progress
                        if (paySch.GetC_Payment_ID() > 0)
                        {
                            // update payment execution status from payment
                            // Reason : in case of independent payment -- bank statement already created - then status on payment become Received
                            // after that when we allocate the same payment with schedule then its show "In-Pogress". It should be "Received".
                            if (payment != null && payment.GetC_Payment_ID() > 0)
                            {
                                paySch.SetVA009_ExecutionStatus(payment.GetVA009_ExecutionStatus());
                            }
                            else
                                paySch.SetVA009_ExecutionStatus("I");
                        }
                        else
                            paySch.SetVA009_ExecutionStatus("R");

                        //back up of Original schedule
                        MInvoicePaySchedule backupNewPaySch = null;
                        if (line.GetOverUnderAmt() != 0 && !_isDuplicateLine)
                        {
                            backupNewPaySch = new MInvoicePaySchedule(GetCtx(), 0, Get_Trx());
                            backupNewPaySch.Set_TrxName(Get_Trx());
                            PO.CopyValues(paySch, backupNewPaySch, paySch.GetAD_Client_ID(), paySch.GetAD_Org_ID());
                        }

                        // update open amount in base / invoice currency when we splitted record
                        paySch.SetVA009_OpenAmnt(BaseCurrency != invoice.GetC_Currency_ID() ? (paySch.GetDueAmt() * multiplyRate) : paySch.GetDueAmt());
                        paySch.SetVA009_OpnAmntInvce(paySch.GetDueAmt());
                        paySch.ByPassValidatePayScheduleCondition(true);
                        if (!paySch.Save(Get_Trx()))
                        {
                            log.Info("Not Updated Paid Amount on Invoice Schedule for this schedule <==> " + line.GetC_InvoicePaySchedule_ID());
                        }

                        // Updating Next Schedule with Varaince amount
                        if (line.GetOverUnderAmt() == 0 && countUnPaidSchedule > 0 && (decimal.Add(varianceAmount, ShiftVarianceOnOther)) != 0)
                        {
                            int no = Util.GetValueOfInt(DB.ExecuteQuery(@"UPDATE C_InvoicePaySchedule 
                                                                 SET VA009_PaidAmntInvce =   NVL(VA009_PaidAmntInvce , 0) + " + Decimal.Round((decimal.Add(varianceAmount, ShiftVarianceOnOther)), currency.GetStdPrecision()) +
                                     @" , VA009_PaidAmnt =  NVL(VA009_PaidAmnt , 0) + " + (BaseCurrency != invoice.GetC_Currency_ID() ? Decimal.Round((decimal.Add(varianceAmount, ShiftVarianceOnOther)) * multiplyRate, currency.GetStdPrecision()) : Decimal.Round((decimal.Add(varianceAmount, ShiftVarianceOnOther)), currency.GetStdPrecision())) +
                                     @" WHERE C_InvoicePaySchedule_ID = ( SELECT MIN(C_InvoicePaySchedule_ID) FROM C_InvoicePaySchedule WHERE IsActive = 'Y'
                                                                 AND VA009_IsPaid = 'N' AND C_Invoice_ID = " + line.GetC_Invoice_ID() +
                                     @" AND C_InvoicePaySchedule_ID <> " + line.GetC_InvoicePaySchedule_ID() + " ) ", null, Get_Trx()));
                        }

                        // Create new invoice schedule with ( over under amount - varaince amount ) as Due Amount rest are same
                        if (line.GetOverUnderAmt() != 0 && !_isDuplicateLine)
                        {
                            //MInvoicePaySchedule newPaySch = new MInvoicePaySchedule(GetCtx(), 0, Get_Trx());
                            MInvoicePaySchedule newPaySch = backupNewPaySch;
                            newPaySch.Set_TrxName(Get_Trx());
                            //PO.CopyValues(paySch, newPaySch, paySch.GetAD_Client_ID(), paySch.GetAD_Org_ID());

                            // update payment method and its respective fields

                            /*************************************************************************************/
                            // need not update paymnet method id from invocie header in splited  schedule
                            /*
                            newPaySch.SetVA009_PaymentMethod_ID(invoice.GetVA009_PaymentMethod_ID());
                            DataSet dsPaymentMethod = DB.ExecuteDataset(@"SELECT VA009_PaymentMode, VA009_PaymentType, VA009_PaymentTrigger FROM VA009_PaymentMethod
                                          WHERE IsActive = 'Y' AND VA009_PaymentMethod_ID = " + invoice.GetVA009_PaymentMethod_ID(), null, Get_Trx());
                            if (dsPaymentMethod != null && dsPaymentMethod.Tables.Count > 0 && dsPaymentMethod.Tables[0].Rows.Count > 0)
                            {
                                if (!String.IsNullOrEmpty(Util.GetValueOfString(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentMode"])))
                                    newPaySch.SetVA009_PaymentMode(Util.GetValueOfString(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentMode"]));
                                if (!String.IsNullOrEmpty(Util.GetValueOfString(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentType"])))
                                    newPaySch.SetVA009_PaymentType(Util.GetValueOfString(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentType"]));
                                newPaySch.SetVA009_PaymentTrigger(Util.GetValueOfString(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentTrigger"]));
                            }
                            dsPaymentMethod = null;*/

                            /*************************************************************************************/

                            // update previous schedule variance amount here 
                            newPaySch.SetVA009_PaidAmnt((BaseCurrency != invoice.GetC_Currency_ID() ? ShiftVarianceOnOther * multiplyRate : ShiftVarianceOnOther));
                            newPaySch.SetVA009_PaidAmntInvce(ShiftVarianceOnOther);
                            newPaySch.SetVA009_Variance(0);
                            newPaySch.SetC_Payment_ID(0);
                            newPaySch.SetC_CashLine_ID(0);
                            newPaySch.SetVA009_ExecutionStatus("A");
                            newPaySch.SetIsValid(true);
                            newPaySch.SetVA009_IsPaid(false);
                            if (newPaySch.Get_ColumnIndex("WithholdingAmt") > 0)
                            {
                                newPaySch.SetBackupWithholdingAmount(0);
                                newPaySch.SetWithholdingAmt(0);
                            }
                            if (decimal.Subtract(line.GetOverUnderAmt(), paySch.GetVA009_Variance()) > 0)
                            {
                                //during AR Invoice / AP Credit Memo
                                newPaySch.SetDueAmt(decimal.Round(decimal.Subtract(decimal.Multiply(line.GetOverUnderAmt(), currencymultiplyRate), varianceAmount), currency.GetStdPrecision()));
                            }
                            else if (line.GetOverUnderAmt() < 0)
                            {
                                // during AR Credit Memo / AP Invoice
                                newPaySch.SetDueAmt(decimal.Round(decimal.Subtract(decimal.Negate(decimal.Multiply(line.GetOverUnderAmt(), currencymultiplyRate)), varianceAmount), currency.GetStdPrecision()));
                            }

                            //checking amount is match or not - if not then balance with the same du amount
                            decimal matchedAmount = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(NVL(DUEAMT, 0)) FROM C_InvoicePaySchedule WHERE ISACTIVE = 'Y' AND C_INVOICE_ID = " + invoice.GetC_Invoice_ID(), null, Get_Trx()));
                            matchedAmount += newPaySch.GetDueAmt();
                            if (matchedAmount != (invoice.Get_ColumnIndex("GrandTotalAfterWithholding") > 0
                                && invoice.GetGrandTotalAfterWithholding() != 0 ? invoice.GetGrandTotalAfterWithholding() : invoice.GetGrandTotal()))
                            {
                                newPaySch.SetDueAmt(Decimal.Add(newPaySch.GetDueAmt(),
                                    (Decimal.Subtract((invoice.Get_ColumnIndex("GrandTotalAfterWithholding") > 0
                                    && invoice.GetGrandTotalAfterWithholding() != 0 ? invoice.GetGrandTotalAfterWithholding() : invoice.GetGrandTotal()), matchedAmount))));
                            }

                            // convert due amount into Base Currency
                            newPaySch.SetVA009_OpenAmnt(BaseCurrency != invoice.GetC_Currency_ID() ? Decimal.Multiply(newPaySch.GetDueAmt(), multiplyRate) : newPaySch.GetDueAmt());
                            newPaySch.SetVA009_OpnAmntInvce(newPaySch.GetDueAmt());
                            newPaySch.ByPassValidatePayScheduleCondition(true);
                            if (!newPaySch.Save(Get_Trx()))
                            {
                                ValueNamePair pp = VLogger.RetrieveError();
                                _log.Info("Error found for splitting Invoice Schedule with Over under amount for  this Line ID = " + newPaySch.GetC_Invoice_ID() +
                                           " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                Get_Trx().Rollback();
                                // SI_0745 : message returning
                                SetProcessMsg(Msg.GetMsg(GetCtx(), "ScheduleNotSplitted") + "  " + (pp != null ? pp.GetName() : ""));
                                return DocActionVariables.STATUS_INVALID;
                            }
                        }

                        // this is used to validate invoice and its schedule
                        invoice.ValidatePaySchedule();
                        if (!invoice.Save(Get_Trx()))
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            _log.Info("Error found for updating ispayschedulevalid as true at Invoice  for  Invoice ID = " + invoice.GetC_Invoice_ID() +
                                       " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                            Get_Trx().Rollback();
                            //_processMsg = Msg.GetMsg(GetCtx(), "ScheduleNotSplitted");
                            SetProcessMsg(Msg.GetMsg(GetCtx(), "ScheduleNotSplitted") + "  " + (pp != null ? pp.GetName() : ""));
                            return DocActionVariables.STATUS_INVALID;
                        }

                        // update invoice if all schedule are paid then mark paid as true at invoice
                        if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(C_InvoicePaySchedule_ID) FROM C_InvoicePaySchedule WHERE va009_ispaid = 'N' AND C_Invoice_ID = " + Util.GetValueOfInt(line.GetC_Invoice_ID()), null, Get_Trx())) == 0)
                        {
                            //MInvoice inv = new MInvoice(GetCtx(), line.GetC_Invoice_ID(), Get_Trx());
                            //inv.SetIsPaid(true);
                            //inv.Save(Get_Trx());
                            DB.ExecuteQuery("UPDATE C_Invoice SET IsPaid = 'Y' WHERE C_Invoice_ID = " + line.GetC_Invoice_ID(), null, Get_Trx());
                        }
                        else
                        {
                            //MInvoice inv = new MInvoice(GetCtx(), line.GetC_Invoice_ID(), Get_Trx());
                            //inv.SetIsPaid(false);
                            //inv.Save(Get_Trx());
                            DB.ExecuteQuery("UPDATE C_Invoice SET IsPaid = 'N' WHERE C_Invoice_ID = " + line.GetC_Invoice_ID(), null, Get_Trx());
                        }
                    }
                }

                //End
            }
            //VA228:update amount paid on invoice,get total of paid schedule invoice amount from C_InvoicePaySchedule and update on invoice header
            if (invoiceIds.Count > 0)
            {
                string query = @"UPDATE C_Invoice INV SET VA009_PaidAmount=(SELECT VA009_PaidAmntInvce FROM (
                                SELECT SUM(VA009_PaidAmntInvce) AS VA009_PaidAmntInvce , inv.C_Invoice_id
                                FROM C_Invoice inv
                                INNER JOIN C_InvoicePaySchedule ps ON ps.C_Invoice_id=inv.C_Invoice_id
                                WHERE inv.C_Invoice_id IN(" + string.Join(",", invoiceIds) + @")
                                GROUP BY inv.C_Invoice_id)t WHERE INV.C_Invoice_id=t.C_Invoice_id) WHERE INV.C_Invoice_id IN(" + string.Join(",", invoiceIds) + @")";
                DB.ExecuteQuery(query, null, Get_Trx());

                // update Open Amount 
                query = @"UPDATE C_Invoice INV SET VA009_OpenAmount= (CASE WHEN GrandTotalAfterWithholding != 0 THEN GrandTotalAfterWithholding ELSE GrandTotal END)
                                - VA009_PaidAmount WHERE INV.C_Invoice_id IN(" + string.Join(",", invoiceIds) + @")";
                DB.ExecuteQuery(query, null, Get_Trx());
            }
            //UpdateBP(bps);

            //	User Validation
            String valid = ModelValidationEngine.Get().FireDocValidate
                (this, ModalValidatorVariables.DOCTIMING_AFTER_COMPLETE);
            if (valid != null)
            {
                _processMsg = valid;
                return DocActionVariables.STATUS_INVALID;
            }

            SetProcessed(true);
            SetDocAction(DOCACTION_Close);
            return DocActionVariables.STATUS_COMPLETED;
        }

        /// <summary>
        /// Void Document.
        ///	Same as Close.
        /// </summary>
        /// <returns>true if success</returns>
        public bool VoidIt()
        {
            log.Info(ToString());
            bool retValue = ReverseIt();
            SetDocAction(DOCACTION_None);
            if (retValue)
                _processMsg = Msg.GetMsg(GetCtx(), "Voided");
            else
                _processMsg = Msg.GetMsg(GetCtx(), "VoidError");
            return retValue;
        }

        /// <summary>
        /// Close Document.
        ///	Cancel not delivered Qunatities
        /// </summary>
        /// <returns>true if success</returns>
        public bool CloseIt()
        {
            log.Info(ToString());
            SetDocAction(DOCACTION_None);
            return true;
        }

        /// <summary>
        /// Reverse Correction
        /// </summary>
        /// <returns>true if success</returns>
        public bool ReverseCorrectIt()
        {
            log.Info(ToString());
            bool retValue = ReverseIt();
            SetDocAction(DOCACTION_None);
            if (retValue)
                _processMsg = Msg.GetMsg(GetCtx(), "Reversed");
            else
                _processMsg = Msg.GetMsg(GetCtx(), "ErrorReverse");
            return retValue;
        }

        /// <summary>
        /// Reverse Accrual - none
        /// </summary>
        /// <returns>false</returns>
        public bool ReverseAccrualIt()
        {
            log.Info(ToString());
            bool retValue = ReverseIt();
            SetDocAction(DOCACTION_None);
            return retValue;
        }

        /// <summary>
        /// Re-activate
        /// </summary>
        /// <returns>false</returns>
        public bool ReActivateIt()
        {
            log.Info(ToString());
            return false;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MAllocationHdr[");
            sb.Append(Get_ID()).Append("-").Append(GetSummary()).Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Get Document Info
        /// </summary>
        /// <returns>document info (untranslated)</returns>
        public String GetDocumentInfo()
        {
            return Msg.GetElement(GetCtx(), "C_AllocationHdr_ID") + " " + GetDocumentNo();
        }

        /// <summary>
        /// Create PDF
        /// </summary>
        /// <returns>file or null</returns>
        public FileInfo CreatePDF()
        {
            try
            {
                string fileName = Get_TableName() + Get_ID() + "_" + CommonFunctions.GenerateRandomNo()
                                    + ".txt"; //.pdf
                string filePath = Path.GetTempPath() + fileName;

                FileInfo temp = new FileInfo(filePath);
                if (!temp.Exists)
                {
                    return CreatePDF(temp);
                }
            }
            catch (Exception e)
            {
                log.Severe("Could not create PDF - " + e.Message);
            }
            return null;
        }

        /// <summary>
        /// Create PDF file
        /// </summary>
        /// <param name="file">output file</param>
        /// <returns>file if success</returns>
        public FileInfo CreatePDF(FileInfo file)
        {
            //	ReportEngine re = ReportEngine.get (getCtx(), ReportEngine.INVOICE, getC_Invoice_ID());
            //	if (re == null)
            return null;
            //	return re.getPDF(file);
        }

        /// <summary>
        /// Get Summary
        /// </summary>
        /// <returns>Summary of Document</returns>
        public String GetSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetDocumentNo());
            //	: Total Lines = 123.00 (#1)
            sb.Append(": ")
                .Append(Msg.Translate(GetCtx(), "ApprovalAmt")).Append("=").Append(GetApprovalAmt())
                .Append(" (#").Append(GetLines(false).Length).Append(")");
            //	 - Description
            if (GetDescription() != null && GetDescription().Length > 0)
                sb.Append(" - ").Append(GetDescription());
            return sb.ToString();
        }

        /// <summary>
        /// Get Process Message
        /// </summary>
        /// <returns>clear text error message</returns>
        public String GetProcessMsg()
        {
            return _processMsg;
        }

        /// <summary>
        /// Get Document Owner (Responsible)
        /// </summary>
        /// <returns>AD_User_ID</returns>
        public int GetDoc_User_ID()
        {
            return GetCreatedBy();
        }

        /// <summary>
        /// Reverse Allocation.
        /// Period needs to be open
        /// </summary>
        /// <returns>true if reversed</returns>
        private bool ReverseIt()
        {
            if (!IsActive())
                throw new Exception("Allocation already reversed (not active)");

            //	Can we delete posting
            if (!MPeriod.IsOpen(GetCtx(), GetDateTrx(), MDocBaseType.DOCBASETYPE_PAYMENTALLOCATION, GetAD_Org_ID()))
                throw new Exception("@PeriodClosed@");
            // is Non Business Day?
            // JID_1205: At the trx, need to check any non business day in that org. if not fund then check * org.
            if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetDateTrx(), GetAD_Org_ID()))
            {
                throw new Exception(Common.Common.NONBUSINESSDAY);
            }

            //	Set Inactive
            SetIsActive(false);
            SetDocumentNo(GetDocumentNo() + "^");
            SetApprovalAmt(Env.ZERO);
            SetDocStatus(DOCSTATUS_Reversed);	//	for direct calls
            if (!Save() || IsActive())
                throw new Exception("Cannot de-activate allocation");

            //	Delete Posting
            String sql = "DELETE FROM Fact_Acct WHERE AD_Table_ID=" + MAllocationHdr.Table_ID
                + " AND Record_ID=" + GetC_AllocationHdr_ID();
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            log.Fine("Fact_Acct deleted #" + no);

            //	Unlink Invoices
            GetLines(true);
            HashSet<int> bps = new HashSet<int>();
            for (int i = 0; i < _lines.Length; i++)
            {
                MAllocationLine line = _lines[i];

                bps.Add(line.ProcessIt(true));	//	reverse

                line.SetIsActive(false);
                // set Amount as ZERO on Reversal of Allocation
                line.SetAmount(Env.ZERO);
                line.SetDiscountAmt(Env.ZERO);
                line.SetWriteOffAmt(Env.ZERO);
                line.SetOverUnderAmt(Env.ZERO);
                line.SetWithholdingAmt(Env.ZERO);
                line.SetBackupWithholdingAmount(Env.ZERO);
                line.Save();


                // Added by Amit for Payment Management 5-11-2015   
                if (Env.IsModuleInstalled("VA009_"))
                {
                    MInvoicePaySchedule paySch = new MInvoicePaySchedule(GetCtx(), line.GetC_InvoicePaySchedule_ID(), Get_Trx());
                    paySch.SetVA009_IsPaid(false);
                    paySch.SetVA009_ExecutionStatus("A");
                    paySch.Save(Get_Trx());

                    MInvoice inv = new MInvoice(GetCtx(), line.GetC_Invoice_ID(), Get_Trx());
                    inv.SetIsPaid(false);
                    inv.Save(Get_Trx());
                }
                //End
            }
            // stopped updation of open balance for customer at reversal of allocation by Vivek on 24/11/2017
            //UpdateBP(bps);

            // By amit for Payment management 5-11-2015
            if (Env.IsModuleInstalled("VA009_"))
            {
                //update Payment Batch line Details set payment = null during reverse of this payment
                sql = "UPDATE va009_batchlinedetails SET  C_AllocationHdr_ID = null WHERE C_AllocationHdr_ID = " + GetC_AllocationHdr_ID();
                DB.ExecuteQuery(sql, null, null);
            }

            return true;
        }

        /// <summary>
        /// Update Open Balance of BP's
        /// </summary>
        /// <param name="bps">list of business partners</param>
        private void UpdateBP(HashSet<int> bps)
        {
            log.Info("#" + bps.Count);
            IEnumerator<int> it = bps.GetEnumerator();
            it.Reset();
            while (it.MoveNext())
            {
                int C_BPartner_ID = it.Current;
                MBPartner bp = new MBPartner(GetCtx(), C_BPartner_ID, Get_TrxName());
                if (bp.GetCreditStatusSettingOn() == "CH")
                {
                    bp.SetTotalOpenBalance();		//	recalculates from scratch
                    //	bp.setSOCreditStatus();			//	called automatically
                    if (bp.Save())
                    {
                        log.Fine(bp.ToString());
                    }
                    else
                    {
                        log.Log(Level.SEVERE, "BP not updated - " + bp);
                    }
                }
            }
        }

        #region DocAction Members


        public Env.QueryParams GetLineOrgsQueryInfo()
        {
            return null;
        }

        public DateTime? GetDocumentDate()
        {
            return null;
        }

        public string GetDocBaseType()
        {
            return null;
        }



        public void SetProcessMsg(string processMsg)
        {
            _processMsg = processMsg;
        }



        #endregion

        /// <summary>
        /// Get currency multiply rate according to selected invoice and payment/cashline currecnies
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="payment"></param>
        /// <param name="cashline"></param>
        /// <param name="journalline"></param>
        /// <returns></returns>
        private decimal GetCurrencyMultiplyRate(MInvoice invoice, MPayment payment, MCashLine cashline, MJournalLine journalline)
        {
            decimal currencymultiplyRate = 1;
            StringBuilder _sql = new StringBuilder();
            MCash cash = null; MJournal journal = null;
            int currencyTo_ID = 0, C_ConversionType_ID = 0, AD_Client_ID = 0, AD_Org_ID = 0;
            DateTime? DateAcct = null;
            DateTime? conversionDate = Get_ColumnIndex("ConversionDate") >= 0 && GetConversionDate() != null ? GetConversionDate() : GetDateAcct();
            if (GetC_Currency_ID() != invoice.GetC_Currency_ID())
            {
                // when we allocate invoice with invoice 
                if (payment == null && cashline == null && journalline == null) // Invoice to Invoice
                {
                    currencymultiplyRate = MConversionRate.GetRate(GetC_Currency_ID(), invoice.GetC_Currency_ID(), conversionDate, GetC_ConversionType_ID(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID());
                }
                else
                {
                    //In case of Journal
                    if (journalline != null)
                    {

                        if (journalline != null && invoice != null) // journal to invoice
                        {
                            DateAcct = conversionDate;
                            C_ConversionType_ID = GetC_ConversionType_ID();
                            currencyTo_ID = invoice.GetC_Currency_ID();
                            AD_Client_ID = invoice.GetAD_Client_ID();
                            AD_Org_ID = invoice.GetAD_Org_ID();
                        }
                        else if (journalline != null && payment != null) //Journal to payment
                        {
                            DateAcct = conversionDate;
                            C_ConversionType_ID = GetC_ConversionType_ID();
                            currencyTo_ID = payment.GetC_Currency_ID();
                            AD_Client_ID = payment.GetAD_Client_ID();
                            AD_Org_ID = payment.GetAD_Org_ID();
                        }
                        else if (journalline != null && cashline != null) // journal to cash
                        {
                            //cash = new MCash(cashline.GetCtx(), cashline.GetC_Cash_ID(), cashline.Get_Trx());
                            DateAcct = conversionDate;
                            C_ConversionType_ID = GetC_ConversionType_ID();
                            currencyTo_ID = cash.GetC_Currency_ID();
                            AD_Client_ID = cash.GetAD_Client_ID();
                            AD_Org_ID = cash.GetAD_Org_ID();
                        }
                        return currencymultiplyRate = MConversionRate.GetRate(GetC_Currency_ID(), currencyTo_ID, DateAcct, C_ConversionType_ID, AD_Client_ID, AD_Org_ID);
                    }

                    // in case of cash journal
                    if (cashline != null)
                    {
                        if (cashline != null && invoice != null) // Cash to invoice
                        {
                            DateAcct = conversionDate;
                            C_ConversionType_ID = GetC_ConversionType_ID();
                            currencyTo_ID = invoice.GetC_Currency_ID();
                            AD_Client_ID = invoice.GetAD_Client_ID();
                            AD_Org_ID = invoice.GetAD_Org_ID();
                        }
                        return currencymultiplyRate = MConversionRate.GetRate(GetC_Currency_ID(), currencyTo_ID, DateAcct, C_ConversionType_ID, AD_Client_ID, AD_Org_ID);
                    }

                    // in case of Payment
                    if (payment != null)
                    {
                        if (payment != null && invoice != null) // payment to invoice
                        {
                            DateAcct = conversionDate;
                            C_ConversionType_ID = GetC_ConversionType_ID();
                            currencyTo_ID = invoice.GetC_Currency_ID();
                            AD_Client_ID = invoice.GetAD_Client_ID();
                            AD_Org_ID = invoice.GetAD_Org_ID();
                        }
                        else if (payment != null && payment != null) //payment to payment
                        {
                            DateAcct = conversionDate;
                            C_ConversionType_ID = GetC_ConversionType_ID();
                            currencyTo_ID = payment.GetC_Currency_ID();
                            AD_Client_ID = payment.GetAD_Client_ID();
                            AD_Org_ID = payment.GetAD_Org_ID();
                        }
                        return currencymultiplyRate = MConversionRate.GetRate(GetC_Currency_ID(), currencyTo_ID, DateAcct, C_ConversionType_ID, AD_Client_ID, AD_Org_ID);
                    }
                }
            }
            return currencymultiplyRate;
        }

        private int C_InvoicePaySch_ID = 0;
        /// <summary>
        /// Set Variance on basis of Document type
        /// </summary>
        /// <param name="line"></param>
        /// <param name="countUnPaidSchedule"></param>
        /// <param name="paySch"></param>
        /// <param name="currencymultiplyRate"></param>
        /// <param name="doctype"></param>
        /// <param name="currency"></param>
        private void SetVariance(MAllocationLine line, int countUnPaidSchedule, MInvoicePaySchedule paySch, decimal currencymultiplyRate, MDocType doctype, MCurrency currency, bool _isDuplicate)
        {
            varianceAmount = 0;
            ShiftVarianceOnOther = 0;
            decimal paidInvoiceAmount = paySch.GetVA009_PaidAmntInvce();

            if (currencymultiplyRate != 1)
            {
                if (doctype.GetDocBaseType() == "ARC" || doctype.GetDocBaseType() == "API")
                {
                    varianceAmount = Decimal.Round(Decimal.Subtract((Decimal.Multiply(Decimal.Negate(line.GetAmount() + line.GetWriteOffAmt() +
                        line.GetDiscountAmt() + line.GetOverUnderAmt())
                        , currencymultiplyRate)), paySch.GetDueAmt()), currency.GetStdPrecision());
                }
                else
                {
                    varianceAmount = Decimal.Round(Decimal.Subtract((Decimal.Multiply((line.GetAmount() + line.GetWriteOffAmt() +
                        line.GetDiscountAmt() + line.GetOverUnderAmt())
                        , currencymultiplyRate)), paySch.GetDueAmt()), currency.GetStdPrecision());
                }
                if (_isDuplicate)
                {
                    varianceAmount += Variance;
                }
            }
            if (line.GetOverUnderAmt() == 0 && countUnPaidSchedule == 0)
            {
                // set varaince when there is no other schedule
                if (doctype.GetDocBaseType() == "ARC" || doctype.GetDocBaseType() == "API")
                {
                    if ((Decimal.Multiply(Decimal.Negate(line.GetAmount() + line.GetWriteOffAmt() +
                        line.GetDiscountAmt() + line.GetOverUnderAmt())
                        , currencymultiplyRate)) < paySch.GetDueAmt())
                    {
                        paySch.SetVA009_Variance(Decimal.Negate(varianceAmount));
                    }
                    else
                    {
                        paySch.SetVA009_Variance(varianceAmount);
                    }
                }
                else
                {
                    if (Decimal.Multiply((line.GetAmount() + line.GetWriteOffAmt() +
                        line.GetDiscountAmt() + line.GetOverUnderAmt())
                        , currencymultiplyRate) < paySch.GetDueAmt())
                    {
                        paySch.SetVA009_Variance(Decimal.Negate(varianceAmount));
                    }
                    else
                    {
                        paySch.SetVA009_Variance(varianceAmount);
                    }
                }
            }

            if (line.GetOverUnderAmt() != 0)
            {
                // check varinace available on current schedule and try to create other schedule then update other schedule with this variance amount
                ShiftVarianceOnOther = paidInvoiceAmount;
                if (doctype.GetDocBaseType() == "ARC" || doctype.GetDocBaseType() == "API")
                {
                    if (_isDuplicate)
                    {
                        paySch.SetVA009_PaidAmntInvce(Decimal.Round(Decimal.Add(Decimal.Multiply
                            (Decimal.Negate(line.GetAmount() + line.GetWriteOffAmt() +
                            line.GetDiscountAmt())
                            , currencymultiplyRate), PaidAmt), currency.GetStdPrecision()));
                    }
                    else
                    {
                        paySch.SetVA009_PaidAmntInvce(Decimal.Round(Decimal.Multiply
                            (Decimal.Negate(line.GetAmount() + line.GetWriteOffAmt() + line.GetDiscountAmt())
                            , currencymultiplyRate), currency.GetStdPrecision()));
                    }
                }
                else
                {
                    // check varinace available on current schedule and try to create other schedule then update other schedule with this variance amount
                    ShiftVarianceOnOther = paidInvoiceAmount;
                    if (_isDuplicate)
                    {
                        paySch.SetVA009_PaidAmntInvce(Decimal.Round(Decimal.Add(
                           Decimal.Multiply((line.GetAmount() + line.GetWriteOffAmt() + line.GetDiscountAmt())
                           , currencymultiplyRate), PaidAmt), currency.GetStdPrecision()));
                    }
                    else
                    {
                        paySch.SetVA009_PaidAmntInvce(Decimal.Round(
                            Decimal.Multiply((line.GetAmount() + line.GetWriteOffAmt() + line.GetDiscountAmt())
                            , currencymultiplyRate), currency.GetStdPrecision()));
                    }

                    // varaince must be 0 when we have other schedule to be paid
                    paySch.SetVA009_Variance(0);
                }
            }
            else if (countUnPaidSchedule != 0)
            {
                ShiftVarianceOnOther = paidInvoiceAmount;
                if (doctype.GetDocBaseType() == "ARC" || doctype.GetDocBaseType() == "API")
                {
                    if (_isDuplicate)
                    {
                        paySch.SetVA009_PaidAmntInvce(Decimal.Round(Decimal.Add(Decimal.Subtract(
                               Decimal.Multiply(Decimal.Negate(line.GetAmount() + line.GetWriteOffAmt() + line.GetDiscountAmt())
                               , currencymultiplyRate), varianceAmount), PaidAmt), currency.GetStdPrecision()));
                    }
                    else
                    {
                        paySch.SetVA009_PaidAmntInvce(Decimal.Round(Decimal.Subtract(
                               Decimal.Multiply(Decimal.Negate(line.GetAmount() + line.GetWriteOffAmt() + line.GetDiscountAmt())
                               , currencymultiplyRate), varianceAmount), currency.GetStdPrecision()));
                    }
                }
                else
                {
                    if (_isDuplicate)
                    {
                        paySch.SetVA009_PaidAmntInvce(Decimal.Round(Decimal.Add(Decimal.Subtract(
                                Decimal.Multiply((line.GetAmount() + line.GetWriteOffAmt() + line.GetDiscountAmt())
                                , currencymultiplyRate)
                                    , varianceAmount), PaidAmt), currency.GetStdPrecision()));
                    }
                    else
                    {
                        paySch.SetVA009_PaidAmntInvce(Decimal.Round(Decimal.Subtract(
                                Decimal.Multiply((line.GetAmount() + line.GetWriteOffAmt() + line.GetDiscountAmt())
                                , currencymultiplyRate), varianceAmount), currency.GetStdPrecision()));
                    }
                    // varaince must be 0 when we have other schedule to be paid
                    paySch.SetVA009_Variance(0);
                }
            }
            else if (countUnPaidSchedule == 0 && line.GetOverUnderAmt() == 0)
            {
                // against last schedule, we have to add Previous Variance amount and current paid amount
                ShiftVarianceOnOther = paidInvoiceAmount;
                if (doctype.GetDocBaseType() == "ARC" || doctype.GetDocBaseType() == "API")
                {
                    if (_isDuplicate)
                    {
                        paySch.SetVA009_PaidAmntInvce(Decimal.Add(Decimal.Add(ShiftVarianceOnOther,
                            Decimal.Round(Decimal.Multiply(Decimal.Negate(line.GetAmount() + line.GetWriteOffAmt() + line.GetDiscountAmt())
                            , currencymultiplyRate), currency.GetStdPrecision())), PaidAmt));
                    }
                    else
                    {
                        paySch.SetVA009_PaidAmntInvce(Decimal.Add(ShiftVarianceOnOther,
                            Decimal.Round(Decimal.Multiply(Decimal.Negate(line.GetAmount() + line.GetWriteOffAmt() + line.GetDiscountAmt())
                            , currencymultiplyRate), currency.GetStdPrecision())));
                    }
                }
                else
                {
                    if (_isDuplicate)
                    {
                        paySch.SetVA009_PaidAmntInvce(Decimal.Add(Decimal.Add(ShiftVarianceOnOther,
                            Decimal.Round(Decimal.Multiply((line.GetAmount() + line.GetWriteOffAmt() + line.GetDiscountAmt())
                            , currencymultiplyRate), currency.GetStdPrecision())), PaidAmt));
                    }
                    else
                    {
                        paySch.SetVA009_PaidAmntInvce(Decimal.Add(ShiftVarianceOnOther,
                         Decimal.Round(Decimal.Multiply((line.GetAmount() + line.GetWriteOffAmt() + line.GetDiscountAmt())
                         , currencymultiplyRate), currency.GetStdPrecision())));
                    }
                }
                // variance = Paid Invoice amount - Due Amount 
                //paySch.SetVA009_Variance(Decimal.Subtract(paySch.GetVA009_PaidAmntInvce(), paySch.GetDueAmt()));
                paySch.SetVA009_Variance(Decimal.Subtract(paySch.GetDueAmt(), paySch.GetVA009_PaidAmntInvce()));
            }
            if (C_InvoicePaySch_ID != paySch.GetC_InvoicePaySchedule_ID())
            {
                PaidAmt = 0;
                Variance = 0;
            }
            PaidAmt = paySch.GetVA009_PaidAmntInvce();
            Variance = paySch.GetVA009_Variance();
            C_InvoicePaySch_ID = paySch.GetC_InvoicePaySchedule_ID();
        }

    }
}