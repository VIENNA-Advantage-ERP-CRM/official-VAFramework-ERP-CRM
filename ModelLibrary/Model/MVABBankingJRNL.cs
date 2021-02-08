/********************************************************
    * Project Name   : VAdvantage
    * Class Name     : MBankStatement
    * Purpose        : Bank Statement Model
    * Class Used     : X_VAB_BankingJRNL and DocAction
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
//using System.Windows.Forms;
//using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using VAdvantage.Logging;


namespace VAdvantage.Model
{
    public class MVABBankingJRNL : X_VAB_BankingJRNL, DocAction
    {
        //Lines							
        private MVABBankingJRNLLine[] m_lines = null;
        //	Process Message 			
        private String m_processMsg = null;
        //	Just Prepared Flag			
        private bool m_justPrepared = false;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="VAB_BankingJRNL_ID"></param>
        /// <param name="trxName">Transaction</param>
        public MVABBankingJRNL(Ctx ctx, int VAB_BankingJRNL_ID, Trx trxName)
            : base(ctx, VAB_BankingJRNL_ID, trxName)
        {
            if (VAB_BankingJRNL_ID == 0)
            {
                //	setVAB_Bank_Acct_ID (0);	//	parent
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
        public MVABBankingJRNL(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="account">Bank Account</param>
        /// <param name="isManual">Manual statement</param>
        public MVABBankingJRNL(MVABBankAcct account, bool isManual)
            : this(account.GetCtx(), 0, account.Get_TrxName())
        {
            SetClientOrg(account);
            SetVAB_Bank_Acct_ID(account.GetVAB_Bank_Acct_ID());
            SetStatementDate(DateTime.Today.Date);//new DateTime(System.currentTimeMillis()));
            SetBeginningBalance(account.GetCurrentBalance());
            SetName(GetStatementDate().ToString());
            SetIsManual(isManual);
        }

        /// <summary>
        /// Create a new Bank Statement
        /// </summary>
        /// <param name="account">Bank account</param>
        public MVABBankingJRNL(MVABBankAcct account)
            : this(account, false)
        {

        }

        /// <summary>
        /// Get Bank Statement Lines
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>line array</returns>
        public MVABBankingJRNLLine[] GetLines(bool requery)
        {
            if (m_lines != null && !requery)
            {
                return m_lines;
            }
            List<MVABBankingJRNLLine> list = new List<MVABBankingJRNLLine>();
            String sql = "SELECT * FROM VAB_BankingJRNLLine"
                + " WHERE VAB_BankingJRNL_ID=@VAB_BankingJRNL_ID"
                + " ORDER BY Line";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@VAB_BankingJRNL_ID", GetVAB_BankingJRNL_ID());
                idr = DataBase.DB.ExecuteReader(sql, param, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)// while (dr.next())
                {
                    list.Add(new MVABBankingJRNLLine(GetCtx(), dr, Get_TrxName()));
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

            MVABBankingJRNLLine[] retValue = new MVABBankingJRNLLine[list.Count];
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
            String sql = "UPDATE VAB_BankingJRNLLine SET Processed='"
                + (processed ? "Y" : "N")
                + "' WHERE VAB_BankingJRNL_ID=" + GetVAB_BankingJRNL_ID();

            int noLine = Convert.ToInt32(DataBase.DB.ExecuteQuery(sql, null, Get_TrxName()));
            m_lines = null;
            log.Fine("setProcessed - " + processed + " - Lines=" + noLine);
        }

        /// <summary>
        /// Get Bank Account
        /// </summary>
        /// <returns>Bank Account</returns>
        public MVABBankAcct GetBankAccount()
        {
            return MVABBankAcct.Get(GetCtx(), GetVAB_Bank_Acct_ID());
        }

        /// <summary>
        /// Set Bank Account
        /// </summary>
        /// <param name="VAB_Bank_Acct_ID">acc/id</param>
        public new void SetVAB_Bank_Acct_ID(int VAB_Bank_Acct_ID)
        {
            base.SetVAB_Bank_Acct_ID(VAB_Bank_Acct_ID);
        }

        /// <summary>
        /// Set Bank Account - Callout
        /// </summary>
        /// <param name="oldVAB_Bank_Acct_ID">Oldbank</param>
        /// <param name="newVAB_Bank_Acct_ID">new bank</param>
        /// <param name="windowNo">window no</param>
        /// @UICallout
        public void SetVAB_Bank_Acct_ID(String oldVAB_Bank_Acct_ID, String newVAB_Bank_Acct_ID, int windowNo)
        {
            if (newVAB_Bank_Acct_ID == null || newVAB_Bank_Acct_ID.Length == 0)
            {
                return;
            }
            int VAB_Bank_Acct_ID = int.Parse(newVAB_Bank_Acct_ID);
            if (VAB_Bank_Acct_ID == 0)
            {
                return;
            }
            SetVAB_Bank_Acct_ID(VAB_Bank_Acct_ID);
            MVABBankAcct ba = GetBankAccount();
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
            //	ReportEngine re = ReportEngine.get (getCtx(), ReportEngine.INVOICE, getVAB_Invoice_ID());
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
            if (newRecord || Is_ValueChanged("VAB_Bank_Acct_ID"))
            {
                no = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(VAB_BankingJRNL_ID) FROM VAB_BankingJRNL WHERE IsActive = 'Y' AND DocStatus NOT IN ('CO' , 'CL', 'VO')  
                                                           AND VAB_Bank_Acct_ID = " + GetVAB_Bank_Acct_ID(), null, Get_Trx()));
                if (no > 0)
                {
                    log.SaveError("VIS_CantCreateNewStatement", "");
                    return false;
                }
            }

            //JID_1325: System should not allow to save bank statment with previous date, statement date should be equal or greater than previous created bank statment record with same bank account. 
            no = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(VAB_BankingJRNL_ID) FROM VAB_BankingJRNL WHERE IsActive = 'Y' AND DocStatus != 'VO' AND StatementDate > "
                + GlobalVariable.TO_DATE(GetStatementDate(), true) + " AND VAB_Bank_Acct_ID = " + GetVAB_Bank_Acct_ID() + " AND VAB_BankingJRNL_ID != " + Get_ID(), null, Get_Trx()));
            if (no > 0)
            {
                log.SaveError("VIS_BankStatementDate", "");
                return false;
            }

            // JID_0331: Once user add the lines system should not allow to change the bank account on header
            if (!newRecord && Is_ValueChanged("VAB_Bank_Acct_ID"))
            {
                MVABBankingJRNLLine[] lines = GetLines(false);
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
            if (!MPeriod.IsOpen(GetCtx(), GetStatementDate(), MDocBaseType.DOCBASETYPE_BANKSTATEMENT, GetVAF_Org_ID()))
            {
                m_processMsg = "@PeriodClosed@";
                return DocActionVariables.STATUS_INVALID;
            }

            // is Non Business Day?
            // JID_1205: At the trx, need to check any non business day in that org. if not fund then check * org.
            if (MVABNonBusinessDay.IsNonBusinessDay(GetCtx(), GetStatementDate(), GetVAF_Org_ID()))
            {
                m_processMsg = Common.Common.NONBUSINESSDAY;
                return DocActionVariables.STATUS_INVALID;
            }

            MVABBankingJRNLLine[] lines = GetLines(true);
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
                MVABBankingJRNLLine line = lines[i];
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
            if (!MPeriod.IsOpen(GetCtx(), minDate, MDocBaseType.DOCBASETYPE_BANKSTATEMENT, GetVAF_Org_ID())
                || !MPeriod.IsOpen(GetCtx(), maxDate, MDocBaseType.DOCBASETYPE_BANKSTATEMENT, GetVAF_Org_ID()))
            {
                m_processMsg = "@PeriodClosed@";
                return DocActionVariables.STATUS_INVALID;
            }

            // is Non Business Day?
            // JID_1205: At the trx, need to check any non business day in that org. if not fund then check * org.
            if (MVABNonBusinessDay.IsNonBusinessDay(GetCtx(), GetStatementDate(), GetVAF_Org_ID()))
            {
                m_processMsg = Common.Common.NONBUSINESSDAY;
                return DocActionVariables.STATUS_INVALID;
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
            int docStatus = Util.GetValueOfInt(DB.ExecuteScalar("SELECT count(VAB_Payment_id) FROM VAB_Payment WHERE VAB_Payment_id in ((SELECT VAB_Payment_id from VAB_BankingJRNLline WHERE VAB_BankingJRNL_id =" + GetVAB_BankingJRNL_ID() + " AND VAB_Payment_id > 0)) AND docstatus NOT IN ('CO' , 'CL')", null, Get_Trx()));
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

            int _CountVA034 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_MODULEINFO_ID) FROM VAF_MODULEINFO WHERE PREFIX='VA034_'  AND IsActive = 'Y'"));

            //	Set Payment reconciled
            MVABBankingJRNLLine[] lines = GetLines(false);

            //Changes by SUkhwinder on 20 April, if all lines are not matched then dont allow complete.
            foreach (MVABBankingJRNLLine line in lines)
            {
                // if Transaction amount exist but no payment reference or Charge amount exist with no Charge then give message for Unmatched lines
                if ((line.GetTrxAmt() != Env.ZERO && line.GetVAB_Payment_ID() == 0) || (line.GetChargeAmt() != Env.ZERO && line.GetVAB_Charge_ID() == 0))
                {
                    m_processMsg = Msg.GetMsg(Env.GetCtx(), "LinesNotMatchedYet");
                    return DocActionVariables.STATUS_INVALID;
                }
            }
            //Changes by SUkhwinder on 20 April, if all lines are not matched then dont allow complete.
            Decimal transactionAmt = 0; //Arpit to update only transaction amount in Bank Account UnMatched Balance asked by Ashish Gandhi
            for (int i = 0; i < lines.Length; i++)
            {
                MVABBankingJRNLLine line = lines[i];
                transactionAmt += line.GetTrxAmt();
                if (line.GetVAB_Payment_ID() != 0)
                {
                    MPayment payment = new MPayment(GetCtx(), line.GetVAB_Payment_ID(), Get_TrxName());
                    payment.SetIsReconciled(true);
                    if (_CountVA034 > 0)
                        payment.SetVA034_DepositSlipNo(line.GetVA012_VoucherNo());
                    payment.Save(Get_TrxName());
                }

                //Pratap 1-2-16
                /////	Set Cash Line reconciled
                int _CountVA012 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_MODULEINFO_ID) FROM VAF_MODULEINFO WHERE PREFIX='VA012_'  AND IsActive = 'Y'"));
                if (_CountVA012 > 0)
                {
                    if (line.GetVAB_CashJRNLLine_ID() != 0)
                    {
                        MVABCashJRNLLine cashLine = new MVABCashJRNLLine(GetCtx(), line.GetVAB_CashJRNLLine_ID(), Get_TrxName());
                        cashLine.SetVA012_IsReconciled(true);
                        cashLine.Save(Get_TrxName());
                    }
                }
                ////
            }
            //	Update Bank Account
            MVABBankAcct ba = MVABBankAcct.Get(GetCtx(), GetVAB_Bank_Acct_ID());
            ba.SetCurrentBalance(GetEndingBalance());
            ba.SetUnMatchedBalance(Decimal.Subtract(ba.GetUnMatchedBalance(), transactionAmt));//Arpit
            ba.Save(Get_TrxName());



            //VA009----------------------------------Anuj----------------------
            //int _CountVA009 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_MODULEINFO_ID) FROM VAF_MODULEINFO WHERE PREFIX='VA009_'  AND IsActive = 'Y'"));
            if (Env.IsModuleInstalled("VA009_"))
            {
                MVABBankingJRNLLine[] STlines = GetLines(false);
                for (int i = 0; i < STlines.Length; i++)
                {
                    MVABBankingJRNLLine line = STlines[i];
                    if (line.GetVAB_Payment_ID() != 0)
                    {
                        MPayment payment = new MPayment(GetCtx(), line.GetVAB_Payment_ID(), Get_TrxName());
                        payment.SetVA009_ExecutionStatus("R");
                        if (_CountVA034 > 0)
                            payment.SetVA034_DepositSlipNo(line.GetVA012_VoucherNo());
                        payment.Save(Get_TrxName());

                        //MInvoicePaySchedule inp = new MInvoicePaySchedule(GetCtx(), payment.GetVAB_sched_InvoicePayment_ID(), Get_TrxName());
                        //inp.SetVA009_ExecutionStatus("R");
                        //inp.Save(Get_TrxName());

                        // update execution status as received on Invoice Schedule -  for those payment which are completed or closed
                        if (payment.GetDocStatus() == DOCSTATUS_Closed || payment.GetDocStatus() == DOCSTATUS_Completed)
                        {
                            int no = Util.GetValueOfInt(DB.ExecuteQuery(@"UPDATE VAB_sched_InvoicePayment SET VA009_ExecutionStatus = 'R' WHERE VAB_Payment_ID = " + line.GetVAB_Payment_ID(), null, Get_Trx()));
                        }
                    }
                }
            }

            //END----------------------------------Anuj----------------------

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
                if (!MPeriod.IsOpen(GetCtx(), GetStatementDate(), MDocBaseType.DOCBASETYPE_BANKSTATEMENT, GetVAF_Org_ID()))
                {
                    m_processMsg = "@PeriodClosed@";
                    return false;
                }

                // is Non Business Day?
                // JID_1205: At the trx, need to check any non business day in that org. if not fund then check * org.
                if (MVABNonBusinessDay.IsNonBusinessDay(GetCtx(), GetStatementDate(), GetVAF_Org_ID()))
                {
                    m_processMsg = Common.Common.NONBUSINESSDAY;
                    return false;
                }


                if (MFactAcct.Delete(Table_ID, GetVAB_BankingJRNL_ID(), Get_TrxName()) < 0)
                {
                    return false;	//	could not delete
                }
            }

            //	Set lines to 0
            Decimal transactionAmt = 0; //To update transaction amount in unMatched Balance in case of void 
            MVABBankingJRNLLine[] lines = GetLines(true);
            for (int i = 0; i < lines.Length; i++)
            {
                MVABBankingJRNLLine line = lines[i];
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
                    if (line.GetVAB_Payment_ID() != 0)
                    {
                        MPayment payment = new MPayment(GetCtx(), line.GetVAB_Payment_ID(), Get_TrxName());
                        payment.SetIsReconciled(false);
                        payment.Save(Get_TrxName());
                    }
                }
            }
            AddDescription(Msg.Translate(GetCtx(), "Voided"));
            Decimal voidedDifference = GetStatementDifference();
            SetStatementDifference(Env.ZERO);

            //VA009----------------------------------Anuj----------------------
            //int _CountVA009 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_MODULEINFO_ID) FROM VAF_MODULEINFO WHERE PREFIX='VA009_'  AND IsActive = 'Y'"));
            if (Env.IsModuleInstalled("VA009_"))
            {
                MVABBankingJRNLLine[] STlines = GetLines(false);
                string status = "R"; // Received
                for (int i = 0; i < STlines.Length; i++)
                {
                    MVABBankingJRNLLine line = STlines[i];
                    if (line.GetVAB_Payment_ID() != 0)
                    {
                        MPayment payment = new MPayment(GetCtx(), line.GetVAB_Payment_ID(), Get_TrxName());
                        string _paymentMethod = Util.GetValueOfString(DB.ExecuteScalar("Select va009_paymentbaseType from va009_paymentmethod where va009_paymentmethod_id=" + payment.GetVA009_PaymentMethod_ID() + " And IsActive = 'Y' AND VAF_Client_ID = " + GetVAF_Client_ID()));
                        if (_paymentMethod == "S") // Check
                            status = "B"; // Bounced
                        else
                            status = "C"; // Rejected

                        payment.SetVA009_ExecutionStatus(status);
                        payment.Save(Get_TrxName());

                        //MInvoicePaySchedule inp = new MInvoicePaySchedule(GetCtx(), payment.GetVAB_sched_InvoicePayment_ID(), Get_TrxName());
                        //inp.SetVA009_ExecutionStatus(status);
                        //inp.Save(Get_TrxName());

                        // update execution status as set on Payment on Invoice Schedule -  for those payment which are completed or closed
                        if (payment.GetDocStatus() == DOCSTATUS_Closed || payment.GetDocStatus() == DOCSTATUS_Completed)
                        {
                            int no = Util.GetValueOfInt(DB.ExecuteQuery(@"UPDATE VAB_sched_InvoicePayment
                                                                          SET VA009_ExecutionStatus = '" + payment.GetVA009_ExecutionStatus() + @"'  
                                                                          WHERE VAB_Payment_ID = " + line.GetVAB_Payment_ID(), null, Get_Trx()));
                        }
                    }
                }
            }
            //END----------------------------------Anuj----------------------

            //	Update Bank Account
            MVABBankAcct ba = MVABBankAcct.Get(GetCtx(), GetVAB_Bank_Acct_ID());
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
        /// <returns>VAF_UserContact_ID</returns>
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
        /// <returns>VAB_Currency_id</returns>
        public int GetVAB_Currency_ID()
        {
            /*/	MPriceList pl = MPriceList.get(getCtx(), getM_PriceList_ID());
            //	return pl.getVAB_Currency_ID();*/
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
