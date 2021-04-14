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
using VAdvantage.Process;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MForecast : X_C_Forecast
    {   
        #region Private Variables
        private static VLogger log = VLogger.GetVLogger(typeof(MForecast).FullName);

        private string _processMsg = null;
       
        private MForecastLine[] _lines = null;
        #endregion

       
        public MForecast(Ctx ctx, int C_Forecast_ID, Trx trxName)
     : base(ctx, C_Forecast_ID, trxName)
        {
        }

        public MForecast(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        public bool ApproveIt()
        {
            log.Info(ToString());
            return false;
        }

        /// <summary>
        /// Close Document
        /// </summary>
        /// <returns>true if success</returns>
        public bool CloseIt()
        {
            log.Info(ToString());
            return false;
        }

        /// <summary>
        /// Complete Document
        /// </summary>
        /// <returns>new status (Complete, In Progress, Invalid, Waiting ..)</returns>
        public string CompleteIt()
        {
            String valid = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_AFTER_COMPLETE);
            if (valid != null)
            {
                _processMsg = valid;
                return DocActionVariables.STATUS_INVALID;
            }
            //set processed true for all child tabs
            DB.ExecuteQuery("UPDATE C_ForecastLine SET Processed = 'Y'  WHERE C_Forecast_ID = " + GetC_Forecast_ID(), null, Get_Trx());

            SetProcessed(true);
            SetDocAction(DOCACTION_Complete);
            return DocActionVariables.STATUS_COMPLETED;
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
        /// Apprival Amount
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
            log.Info(ToString());
            SetDocAction(DOCACTION_Prepare);
            return true;
        }

        /// <summary>
        /// Prepare Document
        /// </summary>
        /// <returns>new status (In Progress or Invalid)</returns>
        public string PrepareIt()
        {
            log.Info(ToString());
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
            if (!DOCACTION_Complete.Equals(GetDocAction()))
                SetDocAction(DOCACTION_Complete);
            return DocActionVariables.STATUS_INPROGRESS;
        }

        /// <summary>
        /// Process Document
        /// </summary>
        /// <param name="processAction">document action</param>
        /// <returns>true if performed</returns>
        public bool ProcessIt(string processAction)
        {
            _processMsg = null;
            DocumentEngine engine = new DocumentEngine(processAction, GetDocStatus());
            return engine.ProcessIt(processAction, GetDocAction());
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
        /// Reject Approval
        /// </summary>
        /// <returns>true if success</returns>
        public bool RejectIt()
        {
            log.Info(ToString());
            return false;
        }

        /// <summary>
        /// Reverse Accrual - none
        /// </summary>
        /// <returns>false</returns>
        public bool ReverseAccrualIt()
        {
            log.Info(ToString());
            return false;
        }


        /// Reverse Correction
        /// </summary>
        /// <returns>true if success</returns>
        public bool ReverseCorrectIt()
        {
            log.Info(ToString());
            return false;
        }


        public bool UnlockIt()
        {
            log.Info(ToString());
            SetProcessing(false);
            return true;
        }

        /// <summary>
        /// Void Document.
        /// </summary>
        /// <returns>true if success</returns>
        public bool VoidIt()
        {
            log.Info(ToString());
            return false;
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
            String sql = "SELECT * FROM C_ForecastLine WHERE C_Forecast_ID = @forecastid ORDER BY LineNo";
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
                log.Log(Level.SEVERE, "GetLines", e.Message);
            }

            _lines = new MForecastLine[list.Count];
            _lines = list.ToArray();
            return _lines;
        }

    }
}
