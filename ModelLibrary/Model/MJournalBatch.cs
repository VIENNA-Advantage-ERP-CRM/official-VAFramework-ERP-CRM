/********************************************************
 * Class Name     : MJournalBatch
 * Purpose        :  Journal Batch Model
 * Class Used     : X_GL_JournalBatch,DocAction
 * Chronological    Development
 * Deepak           15-JAN-2010
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
using System.Data.SqlClient;
using System.IO;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MJournalBatch : X_GL_JournalBatch, DocAction
    {
        // Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MJournalBatch).FullName);

        /// <summary>
        ///	Create new Journal Batch by copying
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="GL_JournalBatch_ID">journal batch</param>
        /// <param name="dateDoc">date of the document date</param>
        /// <param name="trxName">transaction</param>
        /// <returns>Journal Batch</returns>

        public static MJournalBatch CopyFrom(Ctx ctx, int GL_JournalBatch_ID,
            DateTime? dateDoc, Trx trxName)
        {
            MJournalBatch from = new MJournalBatch(ctx, GL_JournalBatch_ID, trxName);
            if (from.GetGL_JournalBatch_ID() == 0)
            {
                throw new ArgumentException("From Journal Batch not found GL_JournalBatch_ID=" + GL_JournalBatch_ID);
            }
            //
            MJournalBatch to = new MJournalBatch(ctx, 0, trxName);
            PO.CopyValues(from, to, from.GetAD_Client_ID(), from.GetAD_Org_ID());
            to.Set_ValueNoCheck("DocumentNo", null);
            to.Set_ValueNoCheck("C_Period_ID", null);
            to.SetDateAcct(dateDoc);
            to.SetDateDoc(dateDoc);
            to.SetDocStatus(DOCSTATUS_Drafted);
            to.SetDocAction(DOCACTION_Complete);
            to.SetIsApproved(false);
            to.SetProcessed(false);
            //
            if (!to.Save())
            {
                String error = "";
                ValueNamePair pp = VLogger.RetrieveError();
                if (pp != null)
                {
                    error = pp.GetName();
                    if (String.IsNullOrEmpty(error))
                    {
                        error = pp.GetValue();
                    }
                    _log.Log(Level.SEVERE, String.IsNullOrEmpty(error) ? "Could not create Journal Batch" : Msg.GetMsg(to.GetCtx(), error));
                }
                to.SetProcessMsg(String.IsNullOrEmpty(error) ? "Could not create Journal Batch" : Msg.GetMsg(to.GetCtx(), error));
                throw new Exception("Could not create Journal Batch");
            }

            if (to.CopyDetailsFrom(from) == 0)
            {
                throw new Exception("Could not create Journal Batch Details");
            }

            return to;
        }	//	copyFrom


        /// <summary>
        /// Standard Construvtore
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="GL_JournalBatch_ID">id if 0 - create actual batch</param>
        /// <param name="trxName">transaction</param>
        public MJournalBatch(Ctx ctx, int GL_JournalBatch_ID, Trx trxName)
            : base(ctx, GL_JournalBatch_ID, trxName)
        {
            //super (ctx, GL_JournalBatch_ID, trxName);
            if (GL_JournalBatch_ID == 0)
            {
                //	setGL_JournalBatch_ID (0);	PK
                //	setDescription (null);
                //	setDocumentNo (null);
                //	setC_DocType_ID (0);
                SetPostingType(POSTINGTYPE_Actual);
                SetDocAction(DOCACTION_Complete);
                SetDocStatus(DOCSTATUS_Drafted);
                SetTotalCr(Env.ZERO);
                SetTotalDr(Env.ZERO);
                SetProcessed(false);
                SetProcessing(false);
                SetIsApproved(false);
            }
        }	//	MJournalBatch

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">transaction</param>
        public MJournalBatch(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            //super(ctx, dr, trxName);
        }	//	MJournalBatch

        /// <summary>
        /// Copy Constructor.Dos not copy: Dates/Period
        /// </summary>
        /// <param name="original">original</param>
        public MJournalBatch(MJournalBatch original)
            : this(original.GetCtx(), 0, original.Get_TrxName())
        {
            //this (original.getCtx(), 0, original.Get_TrxName());
            SetClientOrg(original);
            SetGL_JournalBatch_ID(original.GetGL_JournalBatch_ID());
            //
            //	setC_AcctSchema_ID(original.getC_AcctSchema_ID());
            //	setGL_Budget_ID(original.getGL_Budget_ID());
            SetGL_Category_ID(original.GetGL_Category_ID());
            SetPostingType(original.GetPostingType());
            SetDescription(original.GetDescription());
            SetC_DocType_ID(original.GetC_DocType_ID());
            SetControlAmt(original.GetControlAmt());
            //
            SetC_Currency_ID(original.GetC_Currency_ID());
            //	setC_ConversionType_ID(original.getC_ConversionType_ID());
            //	setCurrencyRate(original.getCurrencyRate());

            //	SetDateDoc(original.getDateDoc());
            //	setDateAcct(original.getDateAcct());
            //	setC_Period_ID(original.getC_Period_ID());
        }	//	MJournal



        /// <summary>
        ///	Overwrite Client/Org if required
        /// </summary>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="AD_Org_ID">org</param>
        public new void SetClientOrg(int AD_Client_ID, int AD_Org_ID)
        {
            //super.setClientOrg(AD_Client_ID, AD_Org_ID);
            base.SetClientOrg(AD_Client_ID, AD_Org_ID);
        }	//	setClientOrg


        /// <summary>
        /// Get Journal Lines
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns> Array of lines</returns>
        public MJournal[] GetJournals(Boolean requery)
        {
            List<MJournal> list = new List<MJournal>();
            String sql = "SELECT * FROM GL_Journal WHERE GL_JournalBatch_ID=@param ORDER BY DocumentNo";
            IDataReader idr = null;
            SqlParameter[] param = new SqlParameter[1];
            try
            {
                //pstmt = DataBase.prepareStatement(sql, get_TrxName());
                //pstmt.setInt(1, getGL_JournalBatch_ID());
                param[0] = new SqlParameter("@param", GetGL_JournalBatch_ID());
                idr = DataBase.DB.ExecuteReader(sql, param, Get_TrxName());
                while (idr.Read())
                {
                    list.Add(new MJournal(GetCtx(), idr, Get_TrxName()));
                }
                idr.Close();
            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                log.Log(Level.SEVERE, sql, ex);
            }
            //
            MJournal[] retValue = new MJournal[list.Count];
            retValue = list.ToArray();
            return retValue;
        }	//	getJournals

        /// <summary>
        /// Copy Journal/Lines from other Journal Batch
        /// </summary>
        /// <param name="jb">Journal Batch</param>
        /// <returns>number of journals + lines copied</returns>
        public int CopyDetailsFrom(MJournalBatch jb)
        {
            if (IsProcessed() || jb == null)
            {
                return 0;
            }
            int count = 0;
            int lineCount = 0;
            MJournal[] fromJournals = jb.GetJournals(false);
            for (int i = 0; i < fromJournals.Length; i++)
            {
                MJournal toJournal = new MJournal(GetCtx(), 0, jb.Get_TrxName());
                PO.CopyValues(fromJournals[i], toJournal, GetAD_Client_ID(), GetAD_Org_ID());
                toJournal.SetGL_JournalBatch_ID(GetGL_JournalBatch_ID());
                toJournal.Set_ValueNoCheck("DocumentNo", null);	//	create new

                //Manish 18/7/2016. C_Period_ID was Null.. But column is mandatory in database so value can't be null.
                toJournal.Set_ValueNoCheck("C_Period_ID", fromJournals[i].GetC_Period_ID());
                // end
                toJournal.SetDateDoc(GetDateDoc());		//	dates from this Batch
                toJournal.SetDateAcct(GetDateAcct());
                toJournal.SetDocStatus(MJournal.DOCSTATUS_Drafted);
                toJournal.SetDocAction(MJournal.DOCACTION_Complete);
                toJournal.SetTotalCr(Env.ZERO);
                toJournal.SetTotalDr(Env.ZERO);
                toJournal.SetIsApproved(false);
                toJournal.SetIsPrinted(false);
                toJournal.SetPosted(false);
                toJournal.SetProcessed(false);
                if (!toJournal.Save())
                {
                    String error = "";
                    ValueNamePair pp = VLogger.RetrieveError();
                    if (pp != null)
                    {
                        error = pp.GetName();
                        if (String.IsNullOrEmpty(error))
                        {
                            error = pp.GetValue();
                        }
                        _log.Log(Level.SEVERE, String.IsNullOrEmpty(error) ? "Could not create Batch Journal" : Msg.GetMsg(toJournal.GetCtx(), error));
                    }
                    SetProcessMsg(String.IsNullOrEmpty(error) ? "Could not create Batch Journal" : Msg.GetMsg(toJournal.GetCtx(), error));
                }
                else
                {
                    count++;
                    lineCount += toJournal.CopyLinesFrom(fromJournals[i], GetDateAcct(), 'x');
                }
            }
            if (fromJournals.Length != count)
            {
                log.Log(Level.SEVERE, "Line difference - Journals=" + fromJournals.Length + " <> Saved=" + count);
            }

            return count + lineCount;
        }	//	copyLinesFrom

        /// <summary>
        /// Get Period
        /// </summary>
        /// <returns>period or null</returns>
        public MPeriod GetPeriod()
        {
            int C_Period_ID = GetC_Period_ID();
            if (C_Period_ID != 0)
            {
                return MPeriod.Get(GetCtx(), C_Period_ID);
            }
            return null;
        }	//	getPeriod

        /// <summary>
        /// Set Doc Date - Callout.Sets also acct date and period
        /// </summary>
        /// <param name="oldDateDoc">old</param>
        /// <param name="newDateDoc">new</param>
        /// <param name="windowNo">window no</param>
        public void SetDateDoc(String oldDateDoc,
                String newDateDoc, int windowNo)
        {
            if (newDateDoc == null || newDateDoc.Length == 0)
            {
                return;
            }
            DateTime? dateDoc = Utility.Util.GetValueOfDateTime(PO.ConvertToTimestamp(newDateDoc));
            if (dateDoc == null)
            {
                return;
            }
            SetDateDoc(dateDoc);
            SetDateAcct(dateDoc);
        }	//	SetDateDoc

        /// <summary>
        /// Set Acct Date - Callout.	Sets Period
        /// </summary>
        /// <param name="oldDateAcct">old</param>
        /// <param name="newDateAcct">new</param>
        /// <param name="windowNo">window no</param>
        public void SetDateAcct(String oldDateAcct,
               String newDateAcct, int windowNo)
        {
            if (newDateAcct == null || newDateAcct.Length == 0)
            {
                return;
            }
            DateTime? dateAcct = Utility.Util.GetValueOfDateTime(PO.ConvertToTimestamp(newDateAcct));
            if (dateAcct == null)
            {
                return;
            }
            SetDateAcct(dateAcct);
        }	//	setDateAcct

        /// <summary>
        /// Set Period - Callout.	Set Acct Date if required
        /// </summary>
        /// <param name="oldC_Period_ID">old</param>
        /// <param name="newC_Period_ID">new</param>
        /// <param name="windowNo">window no</param>
        public void SetC_Period_ID(String oldC_Period_ID,
                String newC_Period_ID, int windowNo)
        {
            if (newC_Period_ID == null || newC_Period_ID.Length == 0)
            {
                return;
            }
            //int C_Period_ID = Integer.parseInt(newC_Period_ID);
            int C_Period_ID = Utility.Util.GetValueOfInt(newC_Period_ID);
            if (C_Period_ID == 0)
            {
                return;
            }
            SetC_Period_ID(C_Period_ID);
        }	//	setC_Period_ID

        /// <summary>
        /// Set Accounting Date.Set also Period if not set earlier
        /// </summary>
        /// <param name="DateAcct">date</param>
        public new void SetDateAcct(DateTime? DateAcct)
        {
            //super.setDateAcct(DateAcct);
            base.SetDateAcct(DateAcct);
            if (DateAcct == null)
            {
                return;
            }
            if (GetC_Period_ID() != 0)
            {
                return;
            }
            int C_Period_ID = MPeriod.GetC_Period_ID(GetCtx(), DateAcct);
            if (C_Period_ID == 0)
            {
                log.Warning("Period not found");
            }
            else
            {
                base.SetC_Period_ID(C_Period_ID);
            }
        }	//	setDateAcct

        /// <summary>
        /// Set Period
        /// </summary>
        /// <param name="C_Period_ID">period</param>
        public new void SetC_Period_ID(int C_Period_ID)
        {
            base.SetC_Period_ID(C_Period_ID);
            if (C_Period_ID == 0)
            {
                return;
            }
            DateTime? dateAcct = GetDateAcct();
            //
            MPeriod period = GetPeriod();
            if (period != null && period.IsStandardPeriod())
            {
                if (!period.IsInPeriod(dateAcct))
                {
                    base.SetDateAcct(period.GetEndDate());
                }
            }
        }	//	setC_Period_ID

        /// <summary>
        /// Process document
        /// </summary>
        /// <param name="processAction">document action</param>
        /// <returns> true if performed</returns>
        public Boolean ProcessIt(String processAction)
        {
            m_processMsg = null;
            DocumentEngine engine = new DocumentEngine(this, GetDocStatus());
            return engine.ProcessIt(processAction, GetDocAction());
        }	//	process

        /**	Process Message 			*/
        private String m_processMsg = null;
        /**	Just Prepared Flag			*/
        private Boolean m_justPrepared = false;

        /// <summary>
        /// Unlock Document.
        /// </summary>
        /// <returns>true if success </returns>
        public Boolean UnlockIt()
        {
            log.Info("unlockIt - " + ToString());
            SetProcessing(false);
            return true;
        }	//	unlockIt

        /// <summary>
        ///	Invalidate Document
        /// </summary>
        /// <returns>true if success </returns>
        public Boolean InvalidateIt()
        {
            log.Info("invalidateIt - " + ToString());
            return true;
        }	//	invalidateIt

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
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());

            //	Std Period open?
            if (!MPeriod.IsOpen(GetCtx(), GetDateAcct(), dt.GetDocBaseType(), GetAD_Org_ID()))
            {
                m_processMsg = "@PeriodClosed@";
                return DocActionVariables.STATUS_INVALID;
            }

            // is Non Business Day?
            // JID_1205: At the trx, need to check any non business day in that org. if not fund then check * org.
            if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetDateAcct(), GetAD_Org_ID()))
            {
                m_processMsg = Common.Common.NONBUSINESSDAY;
                return DocActionVariables.STATUS_INVALID;
            }

            // JID_0521 - Restrict if debit and credit amount is not equal.-Mohit-12-jun-2019.
            if (GetTotalCr() != GetTotalDr())
            {
                m_processMsg = Msg.GetMsg(GetCtx(), "DBAndCRAmtNotEqual");
                return DocActionVariables.STATUS_INVALID;
            }

            //	Add up Amounts & prepare them
            MJournal[] journals = GetJournals(false);
            if (journals.Length == 0)
            {
                m_processMsg = "@NoLines@";
                return DocActionVariables.STATUS_INVALID;
            }

            Decimal TotalDr = Env.ZERO;
            Decimal TotalCr = Env.ZERO;
            for (int i = 0; i < journals.Length; i++)
            {
                MJournal journal = journals[i];
                if (!journal.IsActive())
                {
                    continue;
                }
                //	Prepare if not closed
                if (DOCSTATUS_Closed.Equals(journal.GetDocStatus())
                    || DOCSTATUS_Voided.Equals(journal.GetDocStatus())
                    || DOCSTATUS_Reversed.Equals(journal.GetDocStatus())
                    || DOCSTATUS_Completed.Equals(journal.GetDocStatus()))
                {
                    ;
                }
                else
                {
                    String status = journal.PrepareIt();
                    if (!DocActionVariables.STATUS_INPROGRESS.Equals(status))
                    {
                        journal.SetDocStatus(status);
                        journal.Save();
                        m_processMsg = journal.GetProcessMsg();
                        return status;
                    }
                    journal.SetDocStatus(DOCSTATUS_InProgress);
                    journal.Save();
                }
                //
                //TotalDr = TotalDr.add(journal.getTotalDr());
                TotalDr = Decimal.Add(TotalDr, journal.GetTotalDr());
                TotalCr = Decimal.Add(TotalCr, journal.GetTotalCr());
            }
            SetTotalDr(TotalDr);
            SetTotalCr(TotalCr);

            //	Control Amount
            if (Env.ZERO.CompareTo(GetControlAmt()) != 0
                && GetControlAmt().CompareTo(GetTotalDr()) != 0)
            {
                m_processMsg = "@ControlAmtError@";
                return DocActionVariables.STATUS_INVALID;
            }

            //	Add up Amounts
            m_justPrepared = true;
            return DocActionVariables.STATUS_INPROGRESS;
        }	//	prepareIt

        /// <summary>
        ///	Approve Document
        /// </summary>
        /// <returns>true if success </returns>
        public Boolean ApproveIt()
        {
            log.Info("approveIt - " + ToString());
            SetIsApproved(true);
            return true;
        }	//	approveIt

        /// <summary>
        /// Reject Approval
        /// </summary>
        /// <returns> true if success </returns>
        public Boolean RejectIt()
        {
            log.Info("rejectIt - " + ToString());
            SetIsApproved(false);
            return true;
        }	//	rejectIt

        /// <summary>
        /// Complete Document
        /// </summary>
        /// <returns>new status (Complete, In Progress, Invalid, Waiting ..)</returns>
        public String CompleteIt()
        {
            log.Info("completeIt - " + ToString());
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
            ApproveIt();

            //	Add up Amounts & complete them
            MJournal[] journals = GetJournals(true);
            Decimal? TotalDr = Env.ZERO;
            Decimal? TotalCr = Env.ZERO;
            for (int i = 0; i < journals.Length; i++)
            {
                MJournal journal = journals[i];
                if (!journal.IsActive())
                {
                    journal.SetProcessed(true);
                    journal.SetDocStatus(DOCSTATUS_Voided);
                    journal.SetDocAction(DOCACTION_None);
                    journal.Save();
                    continue;
                }
                //	Complete if not closed
                if (DOCSTATUS_Closed.Equals(journal.GetDocStatus())
                    || DOCSTATUS_Voided.Equals(journal.GetDocStatus())
                    || DOCSTATUS_Reversed.Equals(journal.GetDocStatus())
                    || DOCSTATUS_Completed.Equals(journal.GetDocStatus()))
                {
                    ;
                }
                else
                {
                    String status = journal.CompleteIt();
                    if (!DocActionVariables.STATUS_COMPLETED.Equals(status))
                    {
                        journal.SetDocStatus(status);
                        journal.Save();
                        m_processMsg = journal.GetProcessMsg();
                        return status;
                    }
                    journal.SetDocStatus(DOCSTATUS_Completed);
                    journal.Save();
                }
                //
                //TotalDr = TotalDr.add(journal.getTotalDr());
                TotalDr = Decimal.Add(TotalDr.Value, journal.GetTotalDr());
                TotalCr = Decimal.Add(TotalCr.Value, journal.GetTotalCr());
            }
            SetTotalDr(TotalDr.Value);
            SetTotalCr(TotalCr.Value);
            //	User Validation
            String valid = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_AFTER_COMPLETE);
            if (valid != null)
            {
                m_processMsg = valid;
                return DocActionVariables.STATUS_INVALID;
            }
            //

            // JID_1290: Set the document number from completed document sequence after completed (if needed)
            SetCompletedDocumentNo();

            SetProcessed(true);
            SetDocAction(DOCACTION_Close);
            return DocActionVariables.STATUS_COMPLETED;
        }	//	completeIt

        /// <summary>
        /// Set the document number from Completed Document Sequence after completed
        /// </summary>
        private void SetCompletedDocumentNo()
        {
            // if Reversal document then no need to get Document no from Completed sequence
            if (Get_ColumnIndex("IsReversal") > 0 && IsReversal())
            {
                return;
            }

            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());

            // if Overwrite Date on Complete checkbox is true.
            if (dt.IsOverwriteDateOnComplete())
            {
                SetDateDoc(DateTime.Now.Date);
                if (GetDateAcct().Value.Date < GetDateDoc().Value.Date)
                {
                    SetDateAcct(GetDateDoc());

                    //	Std Period open?
                    if (!MPeriod.IsOpen(GetCtx(), GetDateDoc(), dt.GetDocBaseType(), GetAD_Org_ID()))
                    {
                        throw new Exception("@PeriodClosed@");
                    }
                }
            }

            // if Overwrite Sequence on Complete checkbox is true.
            if (dt.IsOverwriteSeqOnComplete())
            {
                // Set Drafted Document No into Temp Document No.
                if (Get_ColumnIndex("TempDocumentNo") > 0)
                {
                    SetTempDocumentNo(GetDocumentNo());
                }

                // Get current next from Completed document sequence defined on Document type
                String value = MSequence.GetDocumentNo(GetC_DocType_ID(), Get_TrxName(), GetCtx(), true, this);
                if (value != null)
                {
                    SetDocumentNo(value);
                }
            }
        }

        /// <summary>
        /// Void Document.
        /// </summary>
        /// <returns>false </returns>
        public Boolean VoidIt()
        {
            log.Info("voidIt - " + ToString());
            return false;
        }	//	voidIt

        /// <summary>
        /// Close Document.
        /// </summary>
        /// <returns> true if success </returns>
        public Boolean CloseIt()
        {
            log.Info("closeIt - " + ToString());
            MJournal[] journals = GetJournals(true);
            for (int i = 0; i < journals.Length; i++)
            {
                MJournal journal = journals[i];
                if (!journal.IsActive() && !journal.IsProcessed())
                {
                    journal.SetProcessed(true);
                    journal.SetDocStatus(DOCSTATUS_Voided);
                    journal.SetDocAction(DOCACTION_None);
                    journal.Save();
                    continue;
                }
                if (DOCSTATUS_Drafted.Equals(journal.GetDocStatus())
                    || DOCSTATUS_InProgress.Equals(journal.GetDocStatus())
                    || DOCSTATUS_Invalid.Equals(journal.GetDocStatus()))
                {
                    m_processMsg = "Journal not Completed: " + journal.GetSummary();
                    return false;
                }

                //	Close if not closed
                if (DOCSTATUS_Closed.Equals(journal.GetDocStatus())
                    || DOCSTATUS_Voided.Equals(journal.GetDocStatus())
                    || DOCSTATUS_Reversed.Equals(journal.GetDocStatus()))
                {
                    ;
                }
                else
                {
                    if (!journal.CloseIt())
                    {
                        m_processMsg = "Cannot close: " + journal.GetSummary();
                        return false;
                    }
                    journal.Save();
                }
            }
            return true;
        }	//	closeIt

        /// <summary>
        /// Reverse Correction.As if nothing happened - same date
        /// </summary>
        /// <returns>true if success </returns>
        public Boolean ReverseCorrectIt()
        {
            log.Info("reverseCorrectIt - " + ToString());
            MJournal[] journals = GetJournals(true);
            //	check prerequisites
            for (int i = 0; i < journals.Length; i++)
            {
                MJournal journal = journals[i];
                if (!journal.IsActive())
                {
                    continue;
                }
                //	All need to be closed/Completed
                if (DOCSTATUS_Completed.Equals(journal.GetDocStatus()))
                {
                    ;
                }
                else
                {
                    m_processMsg = "All Journals need to be Compleded: " + journal.GetSummary();
                    return false;
                }
            }

            //	Reverse it
            MJournalBatch reverse = new MJournalBatch(this);
            reverse.SetDateDoc(GetDateDoc());
            reverse.SetC_Period_ID(GetC_Period_ID());
            reverse.SetDateAcct(GetDateAcct());
            reverse.SetC_Year_ID(GetC_Year_ID());
            //	Reverse indicator

            if (reverse.Get_ColumnIndex("ReversalDoc_ID") > 0 && reverse.Get_ColumnIndex("IsReversal") > 0)
            {
                // set Reversal property for identifying, record is reversal or not during saving or for other actions
                reverse.SetIsReversal(true);
                // Set Orignal Document Reference
                reverse.SetReversalDoc_ID(GetGL_JournalBatch_ID());
            }

            // for reversal document set Temp Document No to empty
            if (reverse.Get_ColumnIndex("TempDocumentNo") > 0)
            {
                reverse.SetTempDocumentNo("");
            }

            String description = reverse.GetDescription();
            if (description == null)
            {
                description = "** " + GetDocumentNo() + " **";
            }
            else
            {
                description += " ** " + GetDocumentNo() + " **";
                reverse.SetDescription(description);
            }
            if (!reverse.Save())
            {
                ValueNamePair pp = VLogger.RetrieveError();
                if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                {
                    m_processMsg = pp.GetName() + " - " + "Could not reverse " + this;
                }
                else
                {
                    m_processMsg = "Could not reverse " + this;
                }
                return false;
            }
            //

            //	Reverse Journals
            for (int i = 0; i < journals.Length; i++)
            {
                MJournal journal = journals[i];
                if (!journal.IsActive())
                {
                    continue;
                }
                if (journal.ReverseCorrectIt(reverse.GetGL_JournalBatch_ID()) == null)
                {
                    m_processMsg = "Could not reverse " + journal;
                    return false;
                }
                journal.Save();
            }
            return true;
        }	//	reverseCorrectionIt

        /// <summary>
        /// Reverse Accrual.	Flip Dr/Cr - Use Today's date
        /// </summary>
        /// <returns>true if success </returns>
        public Boolean ReverseAccrualIt()
        {
            log.Info("ReverseCorrectIt - " + ToString());
            MJournal[] journals = GetJournals(true);
            //	check prerequisites
            for (int i = 0; i < journals.Length; i++)
            {
                MJournal journal = journals[i];
                if (!journal.IsActive())
                {
                    continue;
                }
                //	All need to be closed/Completed
                if (DOCSTATUS_Completed.Equals(journal.GetDocStatus()))
                {
                    ;
                }
                else
                {
                    m_processMsg = "All Journals need to be Compleded: " + journal.GetSummary();
                    return false;
                }
            }
            //	Reverse it
            MJournalBatch reverse = new MJournalBatch(this);
            reverse.SetC_Period_ID(0);
            //reverse.SetDateDoc(new Timestamp(System.currentTimeMillis()));
            reverse.SetDateDoc(DateTime.Now);
            reverse.SetDateAcct(reverse.GetDateDoc());
            //	Reverse indicator
            String description = reverse.GetDescription();
            if (description == null)
            {
                description = "** " + GetDocumentNo() + " **";
            }
            else
            {
                description += " ** " + GetDocumentNo() + " **";
            }
            reverse.SetDescription(description);
            reverse.Save();

            //	Reverse Journals
            for (int i = 0; i < journals.Length; i++)
            {
                MJournal journal = journals[i];
                if (!journal.IsActive())
                {
                    continue;
                }
                if (journal.ReverseCorrectIt(reverse.GetGL_JournalBatch_ID()) == null)
                {
                    m_processMsg = "Could not reverse " + journal;
                    return false;
                }
                journal.Save();
            }
            return true;
        }	//	ReverseCorrectIt

        /// <summary>
        /// Re-activate - same as reverse correct
        /// </summary>
        /// <returns>true if success </returns>
        public Boolean ReActivateIt()
        {
            log.Info("reActivateIt - " + ToString());

            //	setProcessed(false);
            if (ReverseCorrectIt())
            {
                return true;
            }
            return false;
        }	//	reActivateIt


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
            .Append(Msg.Translate(GetCtx(), "TotalDr")).Append("=").Append(GetTotalDr())
            .Append(" ")
            .Append(Msg.Translate(GetCtx(), "TotalCR")).Append("=").Append(GetTotalCr())
            .Append(" (#").Append(GetJournals(false).Length).Append(")");
            //	 - Description
            if (GetDescription() != null && GetDescription().Length > 0)
            {
                sb.Append(" - ").Append(GetDescription());
            }
            return sb.ToString();
        }	//	GetSummary

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns> info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MJournalBatch[");
            sb.Append(Get_ID()).Append(",").Append(GetDescription())
                .Append(",DR=").Append(GetTotalDr())
                .Append(",CR=").Append(GetTotalCr())
                .Append("]");
            return sb.ToString();
        }	//	toString

        /// <summary>
        ///	Get Document Info
        /// </summary>
        /// <returns>document info (untranslated)</returns>
        public String GetDocumentInfo()
        {
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
            return dt.GetName() + " " + GetDocumentNo();
        }	//	getDocumentInfo

        /// <summary>
        /// Create PDF
        /// </summary>
        /// <returns>File or null</returns>
        //public FileInfo createPDF ()
        //{
        //    try
        //    {
        //        File temp = File.createTempFile(get_TableName()+get_ID()+"_", ".pdf");
        //        return createPDF (temp);
        //    }
        //    catch (Exception e)
        //    {
        //        log.severe("Could not create PDF - " + e.getMessage());
        //    }
        //    return null;
        //}	//	getPDF
        public FileInfo CreatePDF()
        {
            try
            {
                string fileName = Get_TableName() + Get_ID() + "_" + CommonFunctions.GenerateRandomNo()
                                    + ".pdf"; //.pdf
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
        /// <param name="file">file output file</param>
        /// <returns>file if success</returns>
        public FileInfo CreatePDF(FileInfo file)
        {
            //	ReportEngine re = ReportEngine.get (getCtx(), ReportEngine.INVOICE, getC_Invoice_ID());
            //	if (re == null)
            return null;
            //	return re.getPDF(file);
        }	//	createPDF


        /// <summary>
        ///	Get Process Message
        /// </summary>
        /// <returns>clear text error message</returns>
        public String GetProcessMsg()
        {
            return m_processMsg;
        }	//	getProcessMsg

        /// <summary>
        /// Get Document Owner (Responsible)
        /// </summary>
        /// <returns>AD_User_ID (Created By)</returns>
        public int GetDoc_User_ID()
        {
            return GetCreatedBy();
        }	//	getDoc_User_ID

        /// <summary>
        /// Get Document Approval Amount
        /// </summary>
        /// <returns>DR amount</returns>
        public Decimal GetApprovalAmt()
        {
            return GetTotalDr();
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
            m_processMsg = processMsg;
        }



        #endregion

    }

}
