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
using System.IO;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Print;
namespace VAdvantage.Model
{
    public class MProfitLoss : X_C_ProfitLoss, DocAction
    {
        #region Variables
        /**	Process Message 			*/
        private String _processMsg = null;
        /**	Just Prepared Flag			*/
        private bool _justPrepared = false;
        private bool _forceCreation = false;
        private MProfitLossLines[] _lines = null;

        #endregion
        public MProfitLoss(Ctx ctx, int C_ProfitLoss_ID, Trx trxName)
            : base(ctx, C_ProfitLoss_ID, trxName)
        {
            if (C_ProfitLoss_ID == 0)
            {
                SetDocStatus(DOCSTATUS_Drafted);
                SetDocAction(DOCACTION_Prepare);
                SetIsApproved(false);
                base.SetProcessed(false);
                SetProcessing(false);
                SetPosted(false);
                //SetDateAcct(Convert.ToDateTime(DateTime.Now));
                //SetTransactionDate(Convert.ToDateTime(DateTime.Now));
            }
        }

        public MProfitLoss(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Set DocAction
        /// </summary>
        /// <param name="docAction">doc action</param>
        public new void SetDocAction(String docAction)
        {
            SetDocAction(docAction, false);
        }

        /// <summary>
        /// Set DocAction
        /// </summary>
        /// <param name="docAction">doc action</param>
        /// <param name="forceCreation">force creation</param>
        public void SetDocAction(String docAction, bool forceCreation)
        {
            base.SetDocAction(docAction);
            _forceCreation = forceCreation;
        }

        public bool ProcessIt(string processAction)
        {
            _processMsg = null;
            DocumentEngine engine = new DocumentEngine(this, GetDocStatus());
            return engine.ProcessIt(processAction, GetDocAction());
        }

        public bool UnlockIt()
        {
            log.Info("unlockIt - " + ToString());
            SetProcessing(false);
            return true;
        }

        public bool InvalidateIt()
        {
            log.Info(ToString());
            SetDocAction(DOCACTION_Prepare);
            return true;
        }

        public string PrepareIt()
        {
            log.Info(ToString());
            _processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_BEFORE_PREPARE);
            if (_processMsg != null)
                return DocActionVariables.STATUS_INVALID;
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());

            //	Std Period open?
            if (!MPeriod.IsOpen(GetCtx(), GetDateAcct(), dt.GetDocBaseType(), GetAD_Org_ID()))
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

            //	Lines
            MProfitLossLines[] lines = GetLines(true);
            if (lines.Length == 0)
            {
                _processMsg = "@NoLines@";
                return DocActionVariables.STATUS_INVALID;
            }

            _justPrepared = true;
            return DocActionVariables.STATUS_INPROGRESS;
        }

        public MProfitLossLines[] GetLines(bool requery)
        {
            try
            {
                if (_lines != null && !requery)
                {
                    return _lines;
                }

                _lines = GetLines(null);

            }
            catch
            {

                //ShowMessage.Error("MOrder", null, "GetLines");
            }
            return _lines;
        }

        public MProfitLossLines[] GetLines(String whereClause)
        {
            List<MProfitLossLines> list = new List<MProfitLossLines>();
            StringBuilder sql = new StringBuilder("SELECT * FROM C_ProfitLossLines WHERE C_ProfitLoss_ID=" + GetC_ProfitLoss_ID() + "");
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql.ToString(), null, Get_TrxName());
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        MProfitLossLines ol = new MProfitLossLines(GetCtx(), dr, Get_TrxName());
                        //ol.SetHeaderInfo(this);
                        list.Add(ol);
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql.ToString(), e);
            }
            //
            MProfitLossLines[] lines = new MProfitLossLines[list.Count];
            lines = list.ToArray();
            return lines;
        }

        public bool ApproveIt()
        {
            log.Info("approveIt - " + ToString());
            SetIsApproved(true);
            return true;
        }

        public bool RejectIt()
        {
            log.Info("rejectIt - " + ToString());
            SetIsApproved(false);
            return true;
        }

        public string CompleteIt()
        {
            try
            {
                //	Just prepare
                if (DOCACTION_Prepare.Equals(GetDocAction()))
                {
                    SetProcessed(false);
                    return DocActionVariables.STATUS_INPROGRESS;
                }

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


                StringBuilder Info = new StringBuilder();

                bool msg = CheckForPreviousCompeletedRecords();

                if (!msg)
                {
                    return DocActionVariables.STATUS_INVALID;
                }

                SetProcessed(true);
                _processMsg = Info.ToString();

                SetDocAction(DOCACTION_Close);
            }
            catch (Exception ex)
            {
                log.Severe("Exception on Profit & Loss Clossing Completion : " + ex);
                return DocActionVariables.STATUS_INVALID;
            }
            return DocActionVariables.STATUS_COMPLETED;
        }

        private bool CheckForPreviousCompeletedRecords()
        {
            MProfitLoss PL = new MProfitLoss(GetCtx(), GetC_ProfitLoss_ID(), null);
            MProfitLoss prof = new MProfitLoss(GetCtx(), GetC_ProfitLoss_ID(), Get_TrxName());
            StringBuilder sql = new StringBuilder();

            sql.Append("SELECT distinct CP.* FROM C_ProfitLoss CP INNER JOIN Fact_Acct ft ON ft.C_AcctSchema_ID = Cp.C_AcctSchema_ID                             "
                       + " INNER JOIN c_elementvalue ev ON ft.account_id         = ev.c_elementvalue_id                                                           "
                       + " WHERE CP.ad_client_id    = " + +GetAD_Client_ID());

            if (prof.Get_Value("PostingType") != null)
            {
                sql.Append(" and CP.PostingType = '" + prof.Get_Value("PostingType") + "' ");
            }
            sql.Append(" AND (( " + GlobalVariable.TO_DATE(prof.GetDateFrom(), true) + " >= CP.DateFrom "
                     + " AND " + GlobalVariable.TO_DATE(prof.GetDateFrom(), true) + " <= CP.DateTo "
                     + " OR " + GlobalVariable.TO_DATE(prof.GetDateTo(), true) + " <= CP.DateFrom "
                     + " AND " + GlobalVariable.TO_DATE(prof.GetDateTo(), true) + " <= CP.DateTo ))  "
                     + " AND (ev.accounttype      ='E' OR ev.accounttype        ='R')     "
                     + " AND ev.isintermediatecode='N' AND CP.AD_Org_ID        IN (    (SELECT Ad_Org_ID   FROM AD_Org   WHERE isactive      = 'Y'             "
                     + " AND (" + DBFunctionCollection.TypecastColumnAsInt("legalentityorg") + " =" + PL.GetAD_Org_ID() + "  OR Ad_Org_ID = " + PL.GetAD_Org_ID() + ")  )) AND DOCstatus in ('CO', 'CL') ");

            if (Util.GetValueOfInt(PL.Get_Value("C_AcctSchema_ID")) > 0)
            {
                sql.Append(" AND Cp.C_AcctSchema_ID=" + Util.GetValueOfInt(PL.Get_Value("C_AcctSchema_ID")));
            }

            DataSet ds1 = DB.ExecuteDataset(sql.ToString(), null, Get_TrxName());

            if (ds1 != null && ds1.Tables[0].Rows.Count > 0)
            {
                /// _processMsg =  "Record(s) for defined criteria has already been generated";
                _processMsg = Msg.GetMsg(GetCtx(), "VIS_RecordsAlreadyGenerated", true);
                return false;
            }
            return true;
        }

        public bool VoidIt()
        {
            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            return true;
        }

        public bool CloseIt()
        {
            log.Info(ToString());
            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            return true;
        }

        public bool ReverseCorrectIt()
        {
            log.Info(ToString());
            return VoidIt();
        }

        public bool ReverseAccrualIt()
        {
            log.Info(ToString());
            return false;
        }

        public bool ReActivateIt()
        {
            log.Info(ToString());
            SetDocAction(DOCACTION_Complete);
            SetProcessed(false);
            return true;
        }

        public string GetSummary()
        {
            return "";
        }

        public string GetDocumentNo()
        {
            return "";
        }

        public string GetDocumentInfo()
        {
            return "";
        }

        public FileInfo CreatePDF()
        {
            return null;
        }

        public string GetProcessMsg()
        {
            return _processMsg;
        }

        public int GetDoc_User_ID()
        {
            return 0;
        }

        public new int GetC_Currency_ID()
        {
            return 0;
        }

        public decimal GetApprovalAmt()
        {
            return 0;
        }

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
            return "";
        }

        private bool CreateReversals()
        {
            return true;
        }

        /// <summary>
        /// Added by SUkhwinder on 5 Dec, 2017, for validating DateTo and DateFrom with Year.
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns></returns>
        protected override bool BeforeSave(bool newRecord)
        {
            try
            {
                DateTime? stDate, eDate;
                stDate = Util.GetValueOfDateTime(DB.ExecuteScalar(" SELECT P.STARTDATE AS STARTDATE    "
                                                                 + "    FROM C_PERIOD P  "
                                                                 + "    INNER JOIN C_YEAR Y                "
                                                                 + "    ON P.C_YEAR_ID    =Y.C_YEAR_ID     "
                                                                 + "    WHERE P.PERIODNO  ='1'             "
                                                                 + "    AND P.C_YEAR_ID   = " + GetC_Year_ID()
                                                                 + "    AND Y.AD_CLIENT_ID= " + GetAD_Client_ID()));

                eDate = Util.GetValueOfDateTime(DB.ExecuteScalar(" SELECT MAX(P.ENDDATE) AS ENDDATE    "
                                                                 + "    FROM C_PERIOD P  "
                                                                 + "    INNER JOIN C_YEAR Y                "
                                                                 + "    ON P.C_YEAR_ID    =Y.C_YEAR_ID     "
                                                                 + "    WHERE P.IsActive  ='Y'             "
                                                                 + "    AND P.C_YEAR_ID   = " + GetC_Year_ID()
                                                                 + "    AND Y.AD_CLIENT_ID= " + GetAD_Client_ID()));

                if (GetDateFrom() != null && GetDateTo() != null)
                {
                    if (GetDateFrom().Value.Date > Convert.ToDateTime(eDate).Date || GetDateFrom() < Convert.ToDateTime(stDate).Date)
                    {
                        log.SaveError(Msg.Translate(GetCtx(), "VIS_SelectedDateRangeDoesNotMatch"), "");
                        return false;
                    }
                    if (GetDateTo().Value.Date > Convert.ToDateTime(eDate).Date || GetDateTo() < Convert.ToDateTime(stDate).Date)
                    {
                        log.SaveError(Msg.Translate(GetCtx(), "VIS_SelectedDateRangeDoesNotMatch"), "");
                        return false;
                    }

                    if (GetDateFrom().Value.Date > GetDateTo().Value.Date)
                    {
                        log.SaveError(Msg.Translate(GetCtx(), "VIS_DateToShouldBeGrtr"), "");
                        return false;
                    }

                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
