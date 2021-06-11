
/********************************************************
 * Module Name    :    VA Framework
 * Purpose        :    Team Forecast Model 
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
using VAdvantage.Print;
using VAdvantage.Process;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MForecast : X_C_Forecast, DocAction
    {

        #region Private Variables
        private static VLogger _log = VLogger.GetVLogger(typeof(MForecast).FullName);
        //	Just Prepared Flag
        private bool _justPrepared = false;
        // Process Message
        private string _processMsg = null;
        // Forecast Lines array
        private MForecastLine[] _lines = null;
        //
        ValueNamePair pp = null;
        #endregion

        /// <summary>
        /// Team Forecast Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Forecast_ID">Team Forecast or 0 for new</param>
        /// <param name="trxName">trx name</param>
        public MForecast(Ctx ctx, int C_Forecast_ID, Trx trxName)
     : base(ctx, C_Forecast_ID, trxName)
        {
            if (C_Forecast_ID == 0)
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
        public MForecast(Ctx ctx, DataRow rs, Trx trxName)
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

            MForecastLine[] lines = GetLines(false);
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
            DB.ExecuteQuery("UPDATE C_ForecastLine SET Processed = 'Y'  WHERE C_Forecast_ID = " + GetC_Forecast_ID(), null, Get_Trx());

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
            int no = DB.ExecuteQuery("UPDATE C_ForecastLine SET Processed = 'N'  WHERE C_Forecast_ID = " + GetC_Forecast_ID(), null, Get_Trx());
            _log.Info("UnProccessed record from Team Forecast Line - " + no);

            // Set Value in Re-Activated when record is reactivated
            if (Get_ColumnIndex("IsReActivated") >= 0)
            {
                Set_Value("IsReActivated", true);
            }

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
                // If master forecast is generated against the team and the user is not able to void the team forecast.
                int no = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(C_MasterForecast.C_MasterForecast_ID) AS CountRecord
                            FROM C_MasterForecastLineDetails INNER JOIN C_MasterForecastLine ON 
                            C_MasterForecastLineDetails.C_MasterForecastLine_ID = C_MasterForecastLine.C_MasterForecastLine_ID 
                            INNER JOIN C_MasterForecast ON C_MasterForecastLine.C_MasterForecast_ID = C_MasterForecast.C_MasterForecast_ID
                            WHERE C_MasterForecast.DocStatus NOT IN ('RE', 'VO') AND 
                            C_MasterForecastLineDetails.C_Forecast_ID = " + GetC_Forecast_ID(), null, Get_Trx()));
                if (no > 0)
                {
                    _processMsg = Msg.GetMsg(GetCtx(), "DependentDocOnMasterForecast");
                    _log.Info("Team Foreacst Reference found on Master Forecast, TeamForecast Document : " + GetDocumentNo());
                    return false;
                }

                // When the user selects the Void option then document should move to In Voided stage. 
                // As a result of this event the system will update all the quantity to zero and 
                // line history will get generated. Enter the quantity value at the description.
                MForecastLine[] lines = GetLines(false);
                for (int i = 0; i < lines.Length; i++)
                {
                    MForecastLine line = lines[i];
                    Decimal old = line.GetBaseQty();
                    if (old.CompareTo(Env.ZERO) != 0)
                    {
                        line.SetBaseQty(Env.ZERO);
                        line.SetQtyEntered(Env.ZERO);
                        line.SetPriceStd(Env.ZERO);
                        line.SetTotalPrice(Env.ZERO);
                       // line.SetUnitPrice(Env.ZERO);
                        line.AddDescription(Msg.GetMsg(GetCtx(), "Voided") + " (" + old + ")");
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
                                        error = Msg.GetMsg(GetCtx(), "TeamForecastNotSave");
                                    }
                                }
                            }
                            _processMsg = error;
                            return false;
                        }
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
        public MForecastLine[] GetLines(Boolean requery)
        {
            if (_lines != null && !requery)
                return _lines;
            //
            List<MForecastLine> list = new List<MForecastLine>();
            String sql = "SELECT * FROM C_ForecastLine WHERE C_Forecast_ID = @forecastid ORDER BY Line";
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@forecastid", GetC_Forecast_ID());

                DataSet ds = VAdvantage.DataBase.DB.ExecuteDataset(sql, param, Get_TrxName());
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new MForecastLine(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, "GetLines", e.Message);
            }

            _lines = new MForecastLine[list.Count];
            _lines = list.ToArray();
            return _lines;
        }

        /// <summary>
        /// Copy Team Forecast Lines
        /// </summary>
        /// <param name="FromForecast"></param>
        /// <returns>info</returns>
        public String CopyLinesFrom(MForecast FromForecast)
        {
            int count = 0;
            int zeroPriceCount = 0;
            string FromCurrency = null;
            string ToCurrency = null;
            int lineNo = 0;
            MForecastLine line = null;
            try
            {
                if (IsProcessed() || IsPosted() || FromForecast == null)
                {
                    return "";
                }
                MForecastLine[] fromLines = FromForecast.GetLines(false);

                // Get Currency's ISO Code
                DataSet _dsCurrency = DB.ExecuteDataset("SELECT ISO_CODE,C_Currency_ID FROM C_Currency WHERE C_Currency_ID IN(" + FromForecast.GetC_Currency_ID()
                                      + "," + GetC_Currency_ID() + ")");

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

                lineNo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT NVL(MAX(Line), 0) + 10 FROM C_ForecastLine WHERE C_Forecast_ID = " + GetC_Forecast_ID()));

                for (int i = 0; i < fromLines.Length; i++)
                {
                    //donot copy the lines where order and project reference exists 
                    if (fromLines[i].GetC_Order_ID() > 0 || fromLines[i].GetC_Project_ID() > 0)
                    {
                        continue;
                    }
                    else
                    {
                        line = new MForecastLine(GetCtx(), 0, Get_Trx());
                        PO.CopyValues(fromLines[i], line, GetAD_Client_ID(), GetAD_Org_ID());

                        //price conversion
                        line.SetUnitPrice(MConversionRate.Convert(GetCtx(), line.GetUnitPrice(), FromForecast.GetC_Currency_ID(), GetC_Currency_ID(),
                            GetDateAcct(), Util.GetValueOfInt(Get_Value("C_ConversionType_ID")), GetAD_Client_ID(), GetAD_Org_ID()));
                        if (line.GetUnitPrice() == 0)
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

                        line.SetTotalPrice(line.GetUnitPrice() * line.GetBaseQty());
                        line.SetC_Forecast_ID(GetC_Forecast_ID());
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
                                log.SaveWarning("", Msg.GetMsg(GetCtx(), "NotSaveForecastLine") + val);
                            }
                            else
                            {
                                log.SaveWarning("", Msg.GetMsg(GetCtx(), "NotSaveForecastLine"));
                            }
                        }
                        else
                        {
                            lineNo += 10;
                            count++;
                        }

                    }
                }
                if (fromLines.Length != count)
                {
                    log.Log(Level.SEVERE, "Lines difference - TeamForecast=" + fromLines.Length + " <> Saved=" + count);
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, e.Message);
            }
            if (zeroPriceCount>0)
            {
                return count+" "+ Msg.GetMsg(GetCtx(), "PriceNotFound") + "=" + zeroPriceCount +" "+Msg.GetMsg(GetCtx(), "NoOfLines") ;
            }
            return count.ToString();
        }

    }
}
