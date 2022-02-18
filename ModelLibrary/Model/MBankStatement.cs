/********************************************************
    * Project Name   : VAdvantage
    * Class Name     : MBankStatement
    * Purpose        : Bank Statement Model
    * Class Used     : X_C_BankStatement and DocAction
    * Chronological    Development
    * Raghunandan     01-Aug-2009
******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Windows.Forms;
//using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using VAdvantage.Logging;


namespace VAdvantage.Model
{
    public class MBankStatement : X_C_BankStatement, DocAction
    {
        //Lines							
        private MBankStatementLine[] m_lines = null;
        //	Process Message 			
        private String m_processMsg = null;
        //	Just Prepared Flag			
        private bool m_justPrepared = false;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="C_BankStatement_ID"></param>
        /// <param name="trxName">Transaction</param>
        public MBankStatement(Ctx ctx, int C_BankStatement_ID, Trx trxName)
            : base(ctx, C_BankStatement_ID, trxName)
        {
            if (C_BankStatement_ID == 0)
            {
                //	setC_BankAccount_ID (0);	//	parent
                SetStatementDate(DateTime.Today.Date);// new DateTime(System.currentTimeMillis()));	// @Date@
                SetDocAction(DOCACTION_Complete);	// CO
                SetDocStatus(DOCSTATUS_Drafted);	// DR
                SetBeginningBalance(Env.ZERO);
                SetStatementDifference(Env.ZERO);
                SetEndingBalance(Env.ZERO);
                SetIsApproved(false);	// N
                SetIsManual(true);	// Y
                SetPosted(false);	// N
                base.SetProcessed(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">Current context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">transaction</param>
        public MBankStatement(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="account">Bank Account</param>
        /// <param name="isManual">Manual statement</param>
        public MBankStatement(MBankAccount account, bool isManual)
            : this(account.GetCtx(), 0, account.Get_TrxName())
        {
            SetClientOrg(account);
            SetC_BankAccount_ID(account.GetC_BankAccount_ID());
            SetStatementDate(DateTime.Today.Date);//new DateTime(System.currentTimeMillis()));
            SetBeginningBalance(account.GetCurrentBalance());
            SetName(GetStatementDate().ToString());
            SetIsManual(isManual);
        }

        /// <summary>
        /// Create a new Bank Statement
        /// </summary>
        /// <param name="account">Bank account</param>
        public MBankStatement(MBankAccount account)
            : this(account, false)
        {

        }

        /// <summary>
        /// Get Bank Statement Lines
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>line array</returns>
        public MBankStatementLine[] GetLines(bool requery)
        {
            if (m_lines != null && !requery)
            {
                return m_lines;
            }
            List<MBankStatementLine> list = new List<MBankStatementLine>();
            String sql = "SELECT * FROM C_BankStatementLine"
                + " WHERE C_BankStatement_ID=@C_BankStatement_ID"
                + " ORDER BY Line";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@C_BankStatement_ID", GetC_BankStatement_ID());
                idr = DataBase.DB.ExecuteReader(sql, param, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)// while (dr.next())
                {
                    list.Add(new MBankStatementLine(GetCtx(), dr, Get_TrxName()));
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
            finally
            {
                dt = null;
                if (idr != null)
                {
                    idr.Close();
                }
            }

            MBankStatementLine[] retValue = new MBankStatementLine[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Add to Description
        /// </summary>
        /// <param name="description">text</param>
        public void AddDescription(String description)
        {
            String desc = GetDescription();
            if (desc == null)
            {
                SetDescription(description);
            }
            else
            {
                SetDescription(desc + " | " + description);
            }
        }

        /// <summary>
        /// Set Processed.
        /// Propagate to Lines/Taxes
        /// </summary>
        /// <param name="processed">processed</param>
        public new void SetProcessed(bool processed)
        {
            base.SetProcessed(processed);
            if (Get_ID() == 0)
            {
                return;
            }
            String sql = "UPDATE C_BankStatementLine SET Processed='"
                + (processed ? "Y" : "N")
                + "' WHERE C_BankStatement_ID=" + GetC_BankStatement_ID();

            int noLine = Convert.ToInt32(DataBase.DB.ExecuteQuery(sql, null, Get_TrxName()));
            m_lines = null;
            log.Fine("setProcessed - " + processed + " - Lines=" + noLine);
        }

        /// <summary>
        /// Get Bank Account
        /// </summary>
        /// <returns>Bank Account</returns>
        public MBankAccount GetBankAccount()
        {
            return MBankAccount.Get(GetCtx(), GetC_BankAccount_ID());
        }

        /// <summary>
        /// Set Bank Account
        /// </summary>
        /// <param name="C_BankAccount_ID">acc/id</param>
        public new void SetC_BankAccount_ID(int C_BankAccount_ID)
        {
            base.SetC_BankAccount_ID(C_BankAccount_ID);
        }

        /// <summary>
        /// Set Bank Account - Callout
        /// </summary>
        /// <param name="oldC_BankAccount_ID">Oldbank</param>
        /// <param name="newC_BankAccount_ID">new bank</param>
        /// <param name="windowNo">window no</param>
        /// @UICallout
        public void SetC_BankAccount_ID(String oldC_BankAccount_ID, String newC_BankAccount_ID, int windowNo)
        {
            if (newC_BankAccount_ID == null || newC_BankAccount_ID.Length == 0)
            {
                return;
            }
            int C_BankAccount_ID = int.Parse(newC_BankAccount_ID);
            if (C_BankAccount_ID == 0)
            {
                return;
            }
            SetC_BankAccount_ID(C_BankAccount_ID);
            MBankAccount ba = GetBankAccount();
            SetBeginningBalance(ba.GetCurrentBalance());
        }

        /// <summary>
        /// Get Document No 
        /// </summary>
        /// <returns>name</returns>
        public String GetDocumentNo()
        {
            return GetName();
        }

        /// <summary>
        /// Get Document Info
        /// </summary>
        /// <returns>document info (untranslated)</returns>
        public String GetDocumentInfo()
        {
            return GetBankAccount().GetName() + " " + GetDocumentNo();
        }

        /// <summary>
        /// Create PDF
        /// </summary>
        /// <returns>File or null</returns>
        public FileInfo CreatePDF()
        {
            //try
            //{
            //    File temp = File.createTempFile(get_TableName() + get_ID() + "_", ".pdf");
            //    return createPDF(temp);
            //}
            //catch (Exception e)
            //{
            //    log.severe("Could not create PDF - " + e.getMessage());
            //}
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
        /// Before Save
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            //JID_1325: if Bank Statement is open against same Bank Account, then not to save new record fo same Bank Account
            int no = 0;
            if (newRecord || Is_ValueChanged("C_BankAccount_ID"))
            {
                no = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(C_BankStatement_ID) FROM C_BankStatement WHERE IsActive = 'Y' AND DocStatus NOT IN ('CO' , 'CL', 'VO')  
                                                           AND C_BankAccount_ID = " + GetC_BankAccount_ID(), null, Get_Trx()));
                if (no > 0)
                {
                    log.SaveError("VIS_CantCreateNewStatement", "");
                    return false;
                }
            }

            // VIS0060: if no period found for statement date, then give message.
            MPeriod period = MPeriod.Get(GetCtx(), GetStatementDate(), GetAD_Org_ID());
            if (period == null)
            {
                log.SaveError("PeriodNotValid", "");
                return false;
            }

            //JID_1325: System should not allow to save bank statment with previous date, statement date should be equal or greater than previous created bank statment record with same bank account. 
            no = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(C_BankStatement_ID) FROM C_BankStatement WHERE IsActive = 'Y' AND DocStatus != 'VO' AND StatementDate > "
                + GlobalVariable.TO_DATE(GetStatementDate(), true) + " AND C_BankAccount_ID = " + GetC_BankAccount_ID() + " AND C_BankStatement_ID != " + Get_ID(), null, Get_Trx()));
            if (no > 0)
            {
                log.SaveError("VIS_BankStatementDate", "");
                return false;
            }

            // JID_0331: Once user add the lines system should not allow to change the bank account on header
            if (!newRecord && Is_ValueChanged("C_BankAccount_ID"))
            {
                MBankStatementLine[] lines = GetLines(false);
                if (lines.Length > 0)
                {
                    log.SaveError("pleaseDeleteLinesFirst", "");
                    return false;
                }
            }

            SetEndingBalance(Decimal.Add(GetBeginningBalance(), GetStatementDifference()));
            return true;
        }

        /// <summary>
        /// Process document
        /// </summary>
        /// <param name="processAction">document action</param>
        /// <returns>true if performed</returns>
        public bool ProcessIt(String processAction)
        {
            m_processMsg = null;
            DocumentEngine engine = new DocumentEngine(this, GetDocStatus());
            return engine.ProcessIt(processAction, GetDocAction());
        }


        /// <summary>
        /// Unlock Document.
        /// </summary>
        /// <returns>true if success </returns>
        public bool UnlockIt()
        {
            log.Info("unlockIt - " + ToString());
            SetProcessing(false);
            return true;
        }

        /// <summary>
        /// Invalidate Document
        /// </summary>
        /// <returns>true if success </returns>
        public bool InvalidateIt()
        {
            log.Info("invalidateIt - " + ToString());
            SetDocAction(DOCACTION_Prepare);
            return true;
        }

        /// <summary>
        /// Prepare Document
        /// </summary>
        /// <returns>new status (In Progress or Invalid) </returns>
        public String PrepareIt()
        {
            log.Info(ToString());
            m_processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_BEFORE_PREPARE);
            if (m_processMsg != null)
            {
                return DocActionVariables.STATUS_INVALID;
            }

            //	Std Period open?
            if (!MPeriod.IsOpen(GetCtx(), GetStatementDate(), MDocBaseType.DOCBASETYPE_BANKSTATEMENT, GetAD_Org_ID()))
            {
                m_processMsg = "@PeriodClosed@";
                return DocActionVariables.STATUS_INVALID;
            }

            // is Non Business Day?
            // JID_1205: At the trx, need to check any non business day in that org. if not fund then check * org.
            if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetStatementDate(), GetAD_Org_ID()))
            {
                m_processMsg = Common.Common.NONBUSINESSDAY;
                return DocActionVariables.STATUS_INVALID;
            }

            MBankStatementLine[] lines = GetLines(true);
            if (lines.Length == 0)
            {
                m_processMsg = "@NoLines@";
                return DocActionVariables.STATUS_INVALID;
            }
            //	Lines
            Decimal total = Env.ZERO;
            DateTime? minDate = GetStatementDate();
            DateTime? maxDate = minDate;
            for (int i = 0; i < lines.Length; i++)
            {
                MBankStatementLine line = lines[i];
                total = Decimal.Add(total, line.GetStmtAmt());
                if (line.GetDateAcct() < (minDate))//before
                {
                    minDate = line.GetDateAcct();
                }
                if (line.GetDateAcct() > maxDate)//after
                {
                    maxDate = line.GetDateAcct();
                }
            }
            SetStatementDifference(total);
            SetEndingBalance(Decimal.Add(GetBeginningBalance(), total));
            if (!MPeriod.IsOpen(GetCtx(), minDate, MDocBaseType.DOCBASETYPE_BANKSTATEMENT, GetAD_Org_ID())
                || !MPeriod.IsOpen(GetCtx(), maxDate, MDocBaseType.DOCBASETYPE_BANKSTATEMENT, GetAD_Org_ID()))
            {
                m_processMsg = "@PeriodClosed@";
                return DocActionVariables.STATUS_INVALID;
            }

            // is Non Business Day?
            // JID_1205: At the trx, need to check any non business day in that org. if not fund then check * org.
            if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetStatementDate(), GetAD_Org_ID()))
            {
                m_processMsg = Common.Common.NONBUSINESSDAY;
                return DocActionVariables.STATUS_INVALID;
            }

            // If all lines are not matched then give message.
            foreach (MBankStatementLine line in lines)
            {
                // if Transaction amount exist but no payment reference or Charge amount exist with no Charge then give message for Unmatched lines
                if ((line.GetTrxAmt() != Env.ZERO && line.GetC_Payment_ID() == 0 && line.GetC_CashLine_ID() == 0)
                    || (line.GetChargeAmt() != Env.ZERO && line.GetC_Charge_ID() == 0))
                {
                    m_processMsg = Msg.GetMsg(Env.GetCtx(), "LinesNotMatchedYet");
                    return DocActionVariables.STATUS_INVALID;
                }
            }


            m_justPrepared = true;
            if (!DOCACTION_Complete.Equals(GetDocAction()))
            {
                SetDocAction(DOCACTION_Complete);
            }
            return DocActionVariables.STATUS_INPROGRESS;
        }

        /// <summary>
        /// Approve Document
        /// </summary>
        /// <returns>true if success </returns>
        public bool ApproveIt()
        {
            log.Info("approveIt - " + ToString());
            SetIsApproved(true);
            return true;
        }

        /// <summary>
        /// Reject Approval
        /// </summary>
        /// <returns>true if success </returns>
        public bool RejectIt()
        {
            log.Info("rejectIt - " + ToString());
            SetIsApproved(false);
            return true;
        }

        /// <summary>
        /// Complete Document
        /// </summary>
        /// <returns>new status (Complete, In Progress, Invalid, Waiting ..)</returns>
        public String CompleteIt()
        {
            //added by shubham (JID_1472) To check payment is complete or close
            int docStatus = Util.GetValueOfInt(DB.ExecuteScalar("SELECT count(c_payment_id) FROM c_payment WHERE c_payment_id in ((SELECT c_payment_id from c_bankstatementline WHERE c_bankstatement_id =" + GetC_BankStatement_ID() + " AND c_payment_id > 0)) AND docstatus NOT IN ('CO' , 'CL')", null, Get_Trx()));
            if (docStatus != 0)
            {
                m_processMsg = Msg.GetMsg(GetCtx(), "paymentnotcompleted");
                return DocActionVariables.STATUS_INVALID;

            }
            //shubham
            //	Re-Check
            if (!m_justPrepared)
            {
                String status = PrepareIt();
                if (!DocActionVariables.STATUS_INPROGRESS.Equals(status))
                {
                    return status;
                }
            }
            //	Implicit Approval
            if (!IsApproved())
            {
                ApproveIt();
            }
            log.Info("completeIt - " + ToString());

            int _CountVA034 = Env.IsModuleInstalled("VA034_") ? 1 : 0;

            //	Set Payment reconciled
            MBankStatementLine[] lines = GetLines(false);

            // to update only transaction amount in Bank Account UnMatched Balance asked by Ashish Gandhi
            Decimal transactionAmt = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                MBankStatementLine line = lines[i];
                transactionAmt += line.GetTrxAmt();
                if (line.GetC_Payment_ID() != 0)
                {
                    MPayment payment = new MPayment(GetCtx(), line.GetC_Payment_ID(), Get_TrxName());
                    payment.SetIsReconciled(true);
                    if (_CountVA034 > 0)
                        payment.SetVA034_DepositSlipNo(line.GetVA012_VoucherNo());
                    payment.Save(Get_TrxName());
                }

                //	Set Cash Line reconciled
                int _CountVA012 = Env.IsModuleInstalled("VA012_") ? 1 : 0;
                if (_CountVA012 > 0)
                {
                    if (line.GetC_CashLine_ID() != 0)
                    {
                        MCashLine cashLine = new MCashLine(GetCtx(), line.GetC_CashLine_ID(), Get_TrxName());
                        cashLine.SetVA012_IsReconciled(true);
                        cashLine.Save(Get_TrxName());
                    }
                }
                ////
            }
            //	Update Bank Account
            MBankAccount ba = MBankAccount.Get(GetCtx(), GetC_BankAccount_ID());
            ba.SetCurrentBalance(GetEndingBalance());
            ba.SetUnMatchedBalance(Decimal.Subtract(ba.GetUnMatchedBalance(), transactionAmt));
            ba.Save(Get_TrxName());


            if (Env.IsModuleInstalled("VA009_"))
            {
                MBankStatementLine[] STlines = GetLines(false);
                for (int i = 0; i < STlines.Length; i++)
                {
                    MBankStatementLine line = STlines[i];
                    if (line.GetC_Payment_ID() != 0)
                    {
                        MPayment payment = new MPayment(GetCtx(), line.GetC_Payment_ID(), Get_TrxName());
                        payment.SetVA009_ExecutionStatus("R");
                        if (_CountVA034 > 0)
                            payment.SetVA034_DepositSlipNo(line.GetVA012_VoucherNo());
                        payment.Save(Get_TrxName());

                        if (payment.GetDocStatus() == DOCSTATUS_Closed || payment.GetDocStatus() == DOCSTATUS_Completed)
                        {
                            // update execution status as Received on Invoice Schedule -  for those payment which are completed or closed
                            int no = Util.GetValueOfInt(DB.ExecuteQuery(@"UPDATE C_InvoicePaySchedule SET VA009_ExecutionStatus = 'R' WHERE C_Payment_ID = " + line.GetC_Payment_ID(), null, Get_Trx()));
                            if (no == 0)
                            {
                                //(1052) update execution status as Received on Order Schedule -  for those payment which are completed or closed
                                no = Util.GetValueOfInt(DB.ExecuteQuery(@"UPDATE VA009_OrderPaySchedule SET VA009_ExecutionStatus = 'R' WHERE C_Payment_ID = " + line.GetC_Payment_ID(), null, Get_Trx()));
                            }
                        }
                    }
                }
            }

            //	User Validation
            String valid = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_AFTER_COMPLETE);
            if (valid != null)
            {
                m_processMsg = valid;
                return DocActionVariables.STATUS_INVALID;
            }
            SetProcessed(true);
            SetDocAction(DOCACTION_Close);
            return DocActionVariables.STATUS_COMPLETED;
        }

        /// <summary>
        /// Void Document.
        /// </summary>
        /// <returns>false</returns>
        public bool VoidIt()
        {
            log.Info(ToString());
            if (DOCSTATUS_Closed.Equals(GetDocStatus())
                || DOCSTATUS_Reversed.Equals(GetDocStatus())
                || DOCSTATUS_Voided.Equals(GetDocStatus()))
            {
                m_processMsg = "Document Closed: " + GetDocStatus();
                SetDocAction(DOCACTION_None);
                return false;
            }

            //	Not Processed
            if (DOCSTATUS_Drafted.Equals(GetDocStatus())
                || DOCSTATUS_Invalid.Equals(GetDocStatus())
                || DOCSTATUS_InProgress.Equals(GetDocStatus())
                || DOCSTATUS_Approved.Equals(GetDocStatus())
                || DOCSTATUS_NotApproved.Equals(GetDocStatus()))
            {
                ;
            }
            //	Std Period open?
            else
            {
                if (!MPeriod.IsOpen(GetCtx(), GetStatementDate(), MDocBaseType.DOCBASETYPE_BANKSTATEMENT, GetAD_Org_ID()))
                {
                    m_processMsg = "@PeriodClosed@";
                    return false;
                }

                // is Non Business Day?
                // JID_1205: At the trx, need to check any non business day in that org. if not fund then check * org.
                if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetStatementDate(), GetAD_Org_ID()))
                {
                    m_processMsg = Common.Common.NONBUSINESSDAY;
                    return false;
                }


                if (MFactAcct.Delete(Table_ID, GetC_BankStatement_ID(), Get_TrxName()) < 0)
                {
                    return false;	//	could not delete
                }
            }

            //	Set lines to 0
            Decimal transactionAmt = 0; //To update transaction amount in unMatched Balance in case of void 
            MBankStatementLine[] lines = GetLines(true);
            for (int i = 0; i < lines.Length; i++)
            {
                MBankStatementLine line = lines[i];
                transactionAmt += line.GetTrxAmt();
                if (line.GetStmtAmt().CompareTo(Env.ZERO) != 0)
                {
                    String description = Msg.Translate(GetCtx(), "Voided") + " ("
                        + Msg.Translate(GetCtx(), "StmtAmt") + "=" + line.GetStmtAmt();
                    if (line.GetTrxAmt().CompareTo(Env.ZERO) != 0)
                    {
                        description += ", " + Msg.Translate(GetCtx(), "TrxAmt") + "=" + line.GetTrxAmt();
                    }
                    if (line.GetChargeAmt().CompareTo(Env.ZERO) != 0)
                    {
                        description += ", " + Msg.Translate(GetCtx(), "ChargeAmt") + "=" + line.GetChargeAmt();
                    }
                    if (line.GetInterestAmt().CompareTo(Env.ZERO) != 0)
                    {
                        description += ", " + Msg.Translate(GetCtx(), "InterestAmt") + "=" + line.GetInterestAmt();
                    }
                    description += ")";
                    line.AddDescription(description);
                    line.SetStmtAmt(Env.ZERO);
                    line.SetTrxAmt(Env.ZERO);
                    line.SetChargeAmt(Env.ZERO);
                    line.SetInterestAmt(Env.ZERO);
                    line.Save(Get_TrxName());
                    if (line.GetC_Payment_ID() != 0)
                    {
                        MPayment payment = new MPayment(GetCtx(), line.GetC_Payment_ID(), Get_TrxName());
                        payment.SetIsReconciled(false);
                        payment.Save(Get_TrxName());
                    }
                }
            }
            AddDescription(Msg.Translate(GetCtx(), "Voided"));
            Decimal voidedDifference = GetStatementDifference();
            SetStatementDifference(Env.ZERO);

            //VA009----------------------------------Anuj----------------------
            //int _CountVA009 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA009_'  AND IsActive = 'Y'"));
            if (Env.IsModuleInstalled("VA009_"))
            {
                MBankStatementLine[] STlines = GetLines(false);
                string status = "R"; // Received
                for (int i = 0; i < STlines.Length; i++)
                {
                    MBankStatementLine line = STlines[i];
                    if (line.GetC_Payment_ID() != 0)
                    {
                        MPayment payment = new MPayment(GetCtx(), line.GetC_Payment_ID(), Get_TrxName());
                        string _paymentMethod = Util.GetValueOfString(DB.ExecuteScalar("Select va009_paymentbaseType from va009_paymentmethod where va009_paymentmethod_id=" + payment.GetVA009_PaymentMethod_ID() + " And IsActive = 'Y' AND AD_Client_ID = " + GetAD_Client_ID()));
                        if (_paymentMethod == "S") // Check
                            status = "B"; // Bounced
                        else
                            status = "C"; // Rejected

                        payment.SetVA009_ExecutionStatus(status);
                        payment.Save(Get_TrxName());

                        //MInvoicePaySchedule inp = new MInvoicePaySchedule(GetCtx(), payment.GetC_InvoicePaySchedule_ID(), Get_TrxName());
                        //inp.SetVA009_ExecutionStatus(status);
                        //inp.Save(Get_TrxName());

                        if (payment.GetDocStatus() == DOCSTATUS_Closed || payment.GetDocStatus() == DOCSTATUS_Completed)
                        {
                            // update execution status as InProgress on Invoice Schedule -  for those payment which are completed or closed
                            int no = Util.GetValueOfInt(DB.ExecuteQuery(@"UPDATE C_InvoicePaySchedule
                                                                          SET VA009_ExecutionStatus = '" + MPayment.VA009_EXECUTIONSTATUS_In_Progress + @"'  
                                                                          WHERE C_Payment_ID = " + line.GetC_Payment_ID(), null, Get_Trx()));
                            if (no == 0)
                            {
                                //(1052)update execution status as InProgress on Order Schedule -  for those payment which are completed or closed
                                no = Util.GetValueOfInt(DB.ExecuteQuery(@"UPDATE VA009_OrderPaySchedule
                                                                          SET VA009_ExecutionStatus = '" + MPayment.VA009_EXECUTIONSTATUS_In_Progress + @"'  
                                                                          WHERE C_Payment_ID = " + line.GetC_Payment_ID(), null, Get_Trx()));
                            }
                        }
                    }
                }
            }
            //END----------------------------------Anuj----------------------

            //	Update Bank Account
            MBankAccount ba = MBankAccount.Get(GetCtx(), GetC_BankAccount_ID());
            ba.SetCurrentBalance(Decimal.Subtract(ba.GetCurrentBalance(), voidedDifference));
            ba.SetUnMatchedBalance(Decimal.Add(ba.GetUnMatchedBalance(), transactionAmt));   //Arpit
            ba.Save(Get_TrxName());
            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            return true;
        }

        /// <summary>
        /// Close Document.
        /// </summary>
        /// <returns> true if success </returns>
        public bool CloseIt()
        {
            log.Info("closeIt - " + ToString());
            SetDocAction(DOCACTION_None);
            return true;
        }

        /// <summary>
        /// Reverse Correction
        /// </summary>
        /// <returns>false</returns>
        public bool ReverseCorrectIt()
        {
            log.Info("reverseCorrectIt - " + ToString());
            return false;
        }

        /// <summary>
        /// Reverse Accrual
        /// </summary>
        /// <returns>false</returns>
        public bool ReverseAccrualIt()
        {
            log.Info("reverseAccrualIt - " + ToString());
            return false;
        }

        /// <summary>
        /// Re-activate
        /// </summary>
        /// <returns>false</returns>
        public bool ReActivateIt()
        {
            log.Info("reActivateIt - " + ToString());
            return false;
        }


        /// <summary>
        /// Get Summary
        /// </summary>
        /// <returns>Summary of Document</returns>
        public String GetSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetName());
            //	: Total Lines = 123.00 (#1)
            sb.Append(": ")
                .Append(Msg.Translate(GetCtx(), "StatementDifference")).Append("=").Append(GetStatementDifference())
                .Append(" (#").Append(GetLines(false).Length).Append(")");
            //	 - Description
            if (GetDescription() != null && GetDescription().Length > 0)
            {
                sb.Append(" - ").Append(GetDescription());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Get Process Message
        /// </summary>
        /// <returns>clear text error message</returns>
        public String GetProcessMsg()
        {
            return m_processMsg;
        }

        /// <summary>
        /// Get Document Owner (Responsible)
        /// </summary>
        /// <returns>AD_User_ID</returns>
        public int GetDoc_User_ID()
        {
            return GetUpdatedBy();
        }

        /// <summary>
        /// Get Document Approval Amount.
        /// Statement Difference
        /// </summary>
        /// <returns>amount</returns>
        public Decimal GetApprovalAmt()
        {
            return GetStatementDifference();
        }

        /// <summary>
        /// Get Document Currency
        /// </summary>
        /// <returns>c_currency_id</returns>
        public int GetC_Currency_ID()
        {
            /*/	MPriceList pl = MPriceList.get(getCtx(), getM_PriceList_ID());
            //	return pl.getC_Currency_ID();*/
            return 0;
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

        }



        #endregion

        //Arpit Case Handled in case of Delete The Bank Statement from Header
        protected override bool BeforeDelete()
        {
            m_lines = GetLines(false);
            if (m_lines != null)
            {
                for (Int32 i = 0; i < m_lines.Length; i++)
                {
                    //Optional Parameter Set True For Deleting Header+Lines 
                    if (!m_lines[i].UpdateBSAndLine(true)) //Sending Deleting Lines true From bank Statement Header to reflect changes of each Line of Bank Statement
                    {
                        Get_TrxName().Rollback();
                        return false;
                    }
                }
            }
            return true;

        }
        //Arpit
    }
}
