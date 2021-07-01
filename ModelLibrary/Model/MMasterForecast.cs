
/********************************************************
 * Module Name    :    VA Framework
 * Purpose        :    Master Forecast Model
 * Employee Code  :    209
 * Date           :    26-April-2021
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Process;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MMasterForecast : X_C_MasterForecast, DocAction
    {
        #region Private Variables
        private static VLogger _log = VLogger.GetVLogger(typeof(MMasterForecast).FullName);
        //	Just Prepared Flag
        private bool _justPrepared = false;
        // Process Message
        private string _processMsg = null;
        // Forecast Lines array
        private MMasterForecastLine[] _lines = null;
        //
        ValueNamePair pp = null;
        #endregion

        /// <summary>
        /// Master Forecast Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Forecast_ID">Team Forecast or 0 for new</param>
        /// <param name="trxName">trx name</param>
        public MMasterForecast(Ctx ctx, int C_MasterForecast_ID, Trx trxName)
     : base(ctx, C_MasterForecast_ID, trxName)
        {
            if (C_MasterForecast_ID == 0)
            {
                SetDocStatus(DOCSTATUS_Drafted);		//	Draft
                SetDocAction(DOCACTION_Complete);
                //
                SetTRXDATE(DateTime.Now);
                SetDateAcct(DateTime.Now);
                //
                base.SetProcessed(false);
                SetProcessing(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">transaction</param>
        public MMasterForecast(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }


        /// <summary>
        /// Implement beforesave logic
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns>true/false</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            //Trxdate cant be greater than acctdate
            if (GetTRXDATE() > GetDateAcct())
            {
                log.SaveError("TrxDateGreater", "");
                return false;
            }
            return true;
        }


        /// <summary>
        /// Approve Document
        /// </summary>
        /// <returns></returns>
        public bool ApproveIt()
        {
            _log.Info(ToString());
            SetIsApproved(true);
            return true;
        }

        /// <summary>
        /// Close Document
        /// </summary>
        /// <returns>true if success</returns>
        public bool CloseIt()
        {
            _log.Info(ToString());
            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            return true;
        }

        /// <summary>
        /// Prepare Document
        /// </summary>
        /// <returns>new status (In Progress or Invalid)</returns>
        public string PrepareIt()
        {
            _log.Info(ToString());
            _processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_BEFORE_PREPARE);
            if (_processMsg != null)
                return DocActionVariables.STATUS_INVALID;

            //	Std Period open?
            if (!MPeriod.IsOpen(GetCtx(), GetTRXDATE(), GetDocBaseType(), GetAD_Org_ID()))
            {
                _processMsg = "@PeriodClosed@";
                return DocActionVariables.STATUS_INVALID;
            }

            // is Non Business Day?
            if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetTRXDATE(), GetAD_Org_ID()))
            {
                _processMsg = VAdvantage.Common.Common.NONBUSINESSDAY;
                return DocActionVariables.STATUS_INVALID;
            }

            MMasterForecastLine[] lines = GetLines(false);
            if (lines.Length == 0)
            {
                _processMsg = "@NoLines@";
                return DocActionVariables.STATUS_INVALID;
            }

            _justPrepared = true;
            if (!DOCACTION_Complete.Equals(GetDocAction()))
                SetDocAction(DOCACTION_Complete);
            return DocActionVariables.STATUS_INPROGRESS;
        }

        /// <summary>
        /// Complete Document
        /// </summary>
        /// <returns>new status (Complete, In Progress, Invalid, Waiting ..)</returns>
        public string CompleteIt()
        {
            //	Re-Check
            if (!_justPrepared)
            {
                String status = PrepareIt();
                if (!DocActionVariables.STATUS_INPROGRESS.Equals(status))
                    return status;
            }

            SetCompletedDocumentNo();

            //	Implicit Approval
            if (!IsApproved())
                ApproveIt();

            String valid = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_AFTER_COMPLETE);
            if (valid != null)
            {
                _processMsg = valid;
                return DocActionVariables.STATUS_INVALID;
            }
            //set processed true for all child tabs
            DB.ExecuteQuery("UPDATE C_MasterForecastLine SET Processed = 'Y'  WHERE C_MasterForecast_ID = " + GetC_MasterForecast_ID(), null, Get_Trx());

            SetProcessed(true);
            SetDocAction(DOCACTION_Close);
            return DocActionVariables.STATUS_COMPLETED;
        }


        /// <summary>
        /// Set the document number from Completed Document Sequence after completed
        /// </summary>
        private void SetCompletedDocumentNo()
        {
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());

            // if Overwrite Date on Complete checkbox is true.
            if (dt.IsOverwriteDateOnComplete())
            {
                SetTRXDATE(DateTime.Now.Date);
                if (GetDateAcct().Value.Date < GetTRXDATE().Value.Date)
                {
                    SetDateAcct(GetTRXDATE());

                    //	Std Period open?
                    if (!MPeriod.IsOpen(GetCtx(), GetDateAcct(), dt.GetDocBaseType(), GetAD_Org_ID()))
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
        /// Create PDF
        /// </summary>
        /// <returns>File or null</returns>
        public FileInfo CreatePDF()
        {
            return null;
        }

        /// <summary>
        /// Approval Amount
        /// </summary>
        /// <returns>0</returns>
        public decimal GetApprovalAmt()
        {
            return 0;
        }

        /// <summary>
        /// Get Document's DocBaseType
        /// </summary>
        /// <returns>DocBaseType</returns>
        public string GetDocBaseType()
        {
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
            return dt.GetDocBaseType();
        }

        /// <summary>
        /// Get Document Date
        /// </summary>
        /// <returns>Trx Date</returns>
        public DateTime? GetDocumentDate()
        {
            return GetTRXDATE();
        }

        /// <summary>
        /// Get Document Info
        /// </summary>
        /// <returns>document info (untranslated)</returns>
        public string GetDocumentInfo()
        {
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
            return dt.GetName() + " " + GetDocumentNo();
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
        /// 
        /// </summary>
        /// <returns>null</returns>
        public Env.QueryParams GetLineOrgsQueryInfo()
        {
            return null;
        }

        /// <summary>
        /// Get Process Message
        /// </summary>
        /// <returns>clear text error message</returns>
        public string GetProcessMsg()
        {
            return _processMsg;
        }

        /// <summary>
        /// Get Summary
        /// </summary>
        /// <returns>Summary of Document</returns>
        public string GetSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetDocumentNo());
            if (GetDescription() != null && GetDescription().Length > 0)
                sb.Append(" - ").Append(GetDescription());
            return sb.ToString();
        }

        /// <summary>
        /// Invalidate Document
        /// </summary>
        /// <returns>true if success</returns>
        public bool InvalidateIt()
        {
            _log.Info(ToString());
            SetDocAction(DOCACTION_Prepare);
            return true;
        }

        /// <summary>
        /// Process Document
        /// </summary>
        /// <param name="processAction">document action</param>
        /// <returns>true if performed</returns>
        public bool ProcessIt(string processAction)
        {
            _processMsg = null;
            DocumentEngine engine = new DocumentEngine(this, GetDocStatus());
            return engine.ProcessIt(processAction, GetDocAction());
        }

        /// <summary>
        /// Re-activate
        /// </summary>
        /// <returns>false</returns>
        public bool ReActivateIt()
        {
            _log.Info(ToString());

            //set processed true for all child tabs
            int no = DB.ExecuteQuery("UPDATE C_MasterForecastLine SET Processed = 'N'  WHERE C_MasterForecast_ID = " + GetC_MasterForecast_ID(), null, Get_Trx());
            _log.Info("UnProccessed record from Master Forecast Line - " + no);

            // Set Default Action
            SetDocAction(DOCACTION_Complete);
            SetProcessed(false);

            return true;
        }

        /// <summary>
        /// Reject Approval
        /// </summary>
        /// <returns>true if success</returns>
        public bool RejectIt()
        {
            _log.Info(ToString());
            SetIsApproved(false);
            return true;
        }

        /// <summary>
        /// Reverse Accrual - none
        /// </summary>
        /// <returns>false</returns>
        public bool ReverseAccrualIt()
        {
            _log.Info(ToString());
            return false;
        }


        /// Reverse Correction
        /// </summary>
        /// <returns>true if success</returns>
        public bool ReverseCorrectIt()
        {
            _log.Info(ToString());
            return false;
        }


        public bool UnlockIt()
        {
            _log.Info(ToString());
            SetProcessing(false);
            return true;
        }

        /// <summary>
        /// Void Document.
        /// </summary>
        /// <returns>true if success</returns>
        public bool VoidIt()
        {
            _log.Info(ToString());
            if (DOCSTATUS_Closed.Equals(GetDocStatus())
                || DOCSTATUS_Reversed.Equals(GetDocStatus())
                || DOCSTATUS_Voided.Equals(GetDocStatus()))
            {
                _processMsg = "Document Closed: " + GetDocStatus();
                SetDocAction(DOCACTION_None);
                return false;
            }
            else
            {

                MMasterForecastLine[] lines = GetLines(false);
                for (int i = 0; i < lines.Length; i++)
                {
                    MMasterForecastLine line = lines[i];
                    line.IsVoid = true;
                    Decimal old = line.GetTotalQty();
                    if (old.CompareTo(Env.ZERO) != 0)
                    {
                        line.SetTotalQty(Env.ZERO);
                        line.AddDescription(Msg.GetMsg(GetCtx(), "Voided") + " (" + old + ")");
                    }
                    old = line.GetSalesOrderQty();
                    if (old.CompareTo(Env.ZERO) != 0)
                    {
                        line.SetSalesOrderQty(Env.ZERO);
                    }
                    old = line.GetForcastQty();
                    if (old.CompareTo(Env.ZERO) != 0)
                    {
                        line.SetForcastQty(Env.ZERO);
                    }
                    old = line.GetOppQty();
                    if (old.CompareTo(Env.ZERO) != 0)
                    {
                        line.SetOppQty(Env.ZERO);
                    }
                    line.SetPlannedRevenue(Env.ZERO);
                    line.SetProcessed(true);
                    if (!line.Save(Get_TrxName()))
                    {
                        pp = VLogger.RetrieveError();
                        string error = string.Empty;
                        if (pp != null)
                        {
                            error = pp.GetValue();
                            if (string.IsNullOrEmpty(error))
                            {
                                error = pp.GetName();
                                if (string.IsNullOrEmpty(error))
                                {
                                    error = Msg.GetMsg(GetCtx(), "MasterForecastnotSave");
                                }
                            }
                        }
                        _processMsg = error;
                        return false;
                    }
                }

                AddDescription(Msg.GetMsg(GetCtx(), "Voided"));
            }
            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            return true;
        }

        /// <summary>
        /// Add to Description
        /// </summary>
        /// <param name="description">text</param>
        public void AddDescription(String description)
        {
            String desc = GetDescription();
            if (desc == null)
                SetDescription(description);
            else
                SetDescription(desc + " | " + description);
        }

        /// <summary>
        /// Get Forecast lines
        /// </summary>
        /// <param name="requery"></param>
        /// <returns>lines</returns>
        public MMasterForecastLine[] GetLines(Boolean requery)
        {
            if (_lines != null && !requery)
                return _lines;
            //
            List<MMasterForecastLine> list = new List<MMasterForecastLine>();
            String sql = "SELECT * FROM C_MasterForecastLine WHERE C_MasterForecast_ID = @forecastid ORDER BY Line";
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@forecastid", GetC_MasterForecast_ID());

                DataSet ds = DB.ExecuteDataset(sql, param, Get_TrxName());
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new MMasterForecastLine(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, "GetLines", e.Message);
            }

            _lines = new MMasterForecastLine[list.Count];
            _lines = list.ToArray();
            return _lines;
        }

        /// <summary>
        /// Copy Master Forecast Lines
        /// </summary>
        /// <param name="FromForecast"></param>
        /// <returns>info</returns>
        public String CopyLinesFrom(MMasterForecast FromForecast)
        {
            int zeroPriceCount = 0;
            int count = 0;
            string FromCurrency = null;
            string ToCurrency = null;
            int lineNo = 0;
            MMasterForecastLine line = null;
            try
            {
                if (IsProcessed() || IsPosted() || FromForecast == null)
                {
                    return "";
                }
                MMasterForecastLine[] fromLines = FromForecast.GetLines(false);

                //get ISO CODE of Currency
                DataSet _dsCurrency = DB.ExecuteDataset("SELECT ISO_CODE,C_Currency_ID FROM C_Currency WHERE C_Currency_ID IN(" + FromForecast.GetC_Currency_ID() + "," + GetC_Currency_ID() + ")");
                if (_dsCurrency != null && _dsCurrency.Tables.Count > 0)
                {
                    for (int i = 0; i < _dsCurrency.Tables[0].Rows.Count; i++)
                    {
                        if (Util.GetValueOfInt(_dsCurrency.Tables[0].Rows[i]["C_Currency_ID"]) == GetC_Currency_ID())
                        {
                            ToCurrency = Util.GetValueOfString(_dsCurrency.Tables[0].Rows[i]["ISO_CODE"]);
                        }
                        else
                        {
                            FromCurrency = Util.GetValueOfString(_dsCurrency.Tables[0].Rows[i]["ISO_CODE"]);
                        }
                    }
                }

                lineNo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT NVL(MAX(Line), 0) + 10 FROM C_MasterForecastLine WHERE C_MasterForecast_ID = " + GetC_MasterForecast_ID()));

                for (int i = 0; i < fromLines.Length; i++)
                {

                    line = new MMasterForecastLine(GetCtx(), 0, Get_Trx());
                    PO.CopyValues(fromLines[i], line, GetAD_Client_ID(), GetAD_Org_ID());

                    //price conversion
                    line.SetPrice(MConversionRate.Convert(GetCtx(), line.GetPrice(), FromForecast.GetC_Currency_ID(), GetC_Currency_ID(), GetDateAcct(),GetC_ConversionType_ID(),
                    GetAD_Client_ID(), GetAD_Org_ID()));
                    if (line.GetPrice() == 0)
                    {
                        if (FromCurrency != null && ToCurrency != FromCurrency)
                        {
                            //if conversion not found then display message
                            _processMsg = Msg.GetMsg(GetCtx(), "ConversionNotFound") + " " + Msg.GetMsg(GetCtx(), "From") + " " + FromCurrency + Msg.GetMsg(GetCtx(), "To") + ToCurrency;
                            count = 0;
                            return count + " " + _processMsg;
                        }
                        else
                        {
                            zeroPriceCount++;
                            continue;
                        }
                    }
                    line.SetTotalQty(line.GetForcastQty());
                    line.SetSalesOrderQty(Env.ZERO);
                    line.SetOppQty(Env.ZERO);
                    line.SetPlannedRevenue(line.GetPrice() * line.GetTotalQty());
                    line.SetC_MasterForecast_ID(GetC_MasterForecast_ID());
                    line.SetLine(lineNo);
                    line.SetProcessed(false);
                    if (!line.Save())
                    {
                        ValueNamePair vp = VLogger.RetrieveError();
                        if (vp != null)
                        {
                            string val = vp.GetName();
                            if (string.IsNullOrEmpty(val))
                            {
                                val = vp.GetValue();
                            }
                            log.SaveWarning("", Msg.GetMsg(GetCtx(), "NotSaveMasterForecastLine") + val);
                        }
                        else
                        {
                            log.SaveWarning("", Msg.GetMsg(GetCtx(), "NotSaveMasterForecastLine"));
                        }
                    }
                    else
                    {
                        lineNo += 10;
                        count++;
                    }

                }
                if (fromLines.Length != count)
                {
                    log.Log(Level.SEVERE, "Lines difference - MasterForecast=" + fromLines.Length + " <> Saved=" + count);
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, e.Message);
            }
            if (zeroPriceCount > 0)
            {
                return count + " " + Msg.GetMsg(GetCtx(), "PriceNotFound") + "=" + zeroPriceCount + " " + Msg.GetMsg(GetCtx(), "NoOfLines");
            }
            return count.ToString();
        }

    }
}
