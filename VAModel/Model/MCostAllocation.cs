using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Process;
using VAdvantage.Utility;
using ViennaAdvantage.Model;

namespace VAdvantage.Model
{
    public class MCostAllocation : X_M_CostAllocation, DocAction
    {
        private String _processMsg = null;
        private MCostAllocationLine[] _lines = null;
        public const String REVERSE_INDICATOR = "^";
        public MCostAllocation(Ctx ctx, int M_CostAllocation_ID, Trx trx) : base(ctx, M_CostAllocation_ID, trx) { }
        public MCostAllocation(Ctx ctx, DataRow dr, Trx trx) : base(ctx, dr, trx) { }

        public virtual bool ProcessIt(String processAction)
        {
            _processMsg = null;
            DocumentEngine engine = new DocumentEngine(this, GetDocStatus());
            return engine.ProcessIt(processAction, GetDocAction());
        }
        public String PrepareIt()
        {
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
            if (!MPeriod.IsOpen(GetCtx(), GetDateAcct(), dt.GetDocBaseType()))
            {
                _processMsg = "@PeriodClosed@";
                return _processMsg;
            }

            // is Non Business Day?
            if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetDateAcct()))
            {
                _processMsg = VAdvantage.Common.Common.NONBUSINESSDAY;
                return _processMsg;
            }

            return DocActionVariables.STATUS_INPROGRESS;
        }
        public virtual String CompleteIt()
        {
            SetIsProcessed(true);
            SetDocAction(DOCACTION_Close);
            SetDocStatus(DOCACTION_Complete);
            return DocActionVariables.STATUS_COMPLETED;
        }
        public virtual bool CloseIt()
        {
            log.Info(ToString());
            SetIsProcessed(true);
            SetDocAction(DOCACTION_None);
            return true;
        }
        public virtual bool VoidIt()
        {
            log.Info(ToString());

            if (DOCSTATUS_Closed.Equals(GetDocStatus())
                || DOCSTATUS_Reversed.Equals(GetDocStatus())
                || DOCSTATUS_Voided.Equals(GetDocStatus()))
            {
                _processMsg = "Document Closed: " + GetDocStatus();
                return false;
            }

            //	Not Processed
            if (DOCSTATUS_Drafted.Equals(GetDocStatus())
                || DOCSTATUS_Invalid.Equals(GetDocStatus())
                || DOCSTATUS_InProgress.Equals(GetDocStatus())
                || DOCSTATUS_Approved.Equals(GetDocStatus())
                || DOCSTATUS_NotApproved.Equals(GetDocStatus()))
            {
                //	Set lines to 0
                MCostAllocationLine[] lines = GetLines(false);
                for (int i = 0; i < lines.Length; i++)
                {
                    MCostAllocationLine line = lines[i];
                    Decimal old = line.GetAmount();
                    if (old != 0)
                    {
                        line.SetAmount(Env.ZERO);
                        line.Save(Get_TrxName());
                    }
                }
            }
            else
            {
                return ReverseCorrectIt();
            }

            SetIsProcessed(true);
            SetDocAction(DOCACTION_None);
            return true;
        }
        public bool ReverseCorrectIt()
        {
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
            if (!MPeriod.IsOpen(GetCtx(), GetDateAcct(), dt.GetDocBaseType()))
            {
                _processMsg = "@PeriodClosed@";
                return false;
            }

            // is Non Business Day?
            if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetDateAcct()))
            {
                _processMsg = VAdvantage.Common.Common.NONBUSINESSDAY;
                return false;
            }

            //set delievered = true
            //int no = Util.GetValueOfInt(DB.ExecuteScalar("UPDATE VA024_t_ObsoleteInventory SET VA024_ISDELIVERED = 'Y' , VA024_REMAININGQTY=0 WHERE VA024_ObsoleteInventory_ID = " + GetVA024_ObsoleteInventory_ID(), null, Get_Trx()));

            // Create Reversal Line
            MCostAllocation reversal = new MCostAllocation(GetCtx(), 0, Get_Trx());
            CopyValues(this, reversal);
            reversal.SetClientOrg(this);
            //
            reversal.SetDocumentNo(GetDocumentNo() + REVERSE_INDICATOR);
            reversal.SetDocStatus(DOCSTATUS_Drafted);
            reversal.SetDocAction(DOCACTION_Complete);
            //
            reversal.SetIsProcessed(false);
            reversal.SetPosted(false);
            //reversal.SetProcessing(false);
            reversal.SetDescription(GetDescription());
            reversal.AddDescription("{->" + GetDocumentNo() + ")");
            reversal.SetAllocationAmt(Decimal.Negate(GetAllocationAmt()));
            reversal.Save(Get_Trx());

            // Copy lines
            MCostAllocationLine reversalLine = null;
            MCostAllocationLine[] lines = GetLines(false);
            for (int i = 0; i < lines.Length; i++)
            {
                MCostAllocationLine line = lines[i];
                reversalLine = new MCostAllocationLine(GetCtx(), 0, Get_Trx());
                CopyValues(line, reversalLine);
                reversalLine.SetClientOrg(this);
                reversalLine.SetM_CostAllocation_ID(reversal.GetM_CostAllocation_ID());
                reversalLine.SetIsProcessed(true);
                reversalLine.SetAmount(Decimal.Negate(line.GetAmount()));
                reversalLine.Save(Get_Trx());

            }

            reversal.CloseIt();
            _processMsg = reversal.GetDocumentNo();
            reversal.SetIsProcessed(true);
            reversal.SetDocStatus(DOCSTATUS_Reversed);
            reversal.SetDocAction(DOCACTION_None);
            reversal.Save(Get_Trx());

            this.AddDescription("(" + GetDocumentNo() + "<-)");
            SetDocAction(DOCACTION_None);
            SetDocStatus(DOCSTATUS_Reversed);
            Save(Get_Trx());
            return true;
        }

        public void AddDescription(String description)
        {
            String desc = GetDescription();
            if (desc == null)
                SetDescription(description);
            else
                SetDescription(desc + " | " + description);
        }

        public MCostAllocationLine[] GetLines(bool requery)
        {
            if (_lines != null && !requery)
                return _lines;
            List<MCostAllocationLine> list = new List<MCostAllocationLine>();
            String sql = "SELECT * FROM M_CostAllocationLine WHERE M_CostAllocation_ID=" + GetM_CostAllocation_ID() + "";
            DataSet ds = null;
            DataRow dr = null;
            try
            {
                ds = DB.ExecuteDataset(sql, null, Get_TrxName());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    dr = ds.Tables[0].Rows[i];
                    list.Add(new MCostAllocationLine(GetCtx(), dr, Get_TrxName()));
                }
                ds = null;
            }
            catch (Exception ex)
            {
                log.Log(Level.SEVERE, sql, ex);
                list = null;
            }
            ds = null;
            //
            if (list == null)
                return null;
            _lines = new MCostAllocationLine[list.Count];
            _lines = list.ToArray();
            return _lines;
        }

        public virtual bool InvalidateIt()
        {
            log.Info(ToString());
            SetDocAction(DOCACTION_Prepare);
            return true;
        }

        public virtual bool ApproveIt()
        {
            log.Info(ToString());
            return true;
        }

        public virtual bool RejectIt()
        {
            return true;
        }

        public virtual bool UnlockIt()
        {
            log.Info(ToString());
            return true;
        }

        public bool ReverseAccrualIt()
        {
            log.Info(ToString());
            return false;
        }

        public virtual bool ReActivateIt()
        {
            log.Info(ToString());
            return false;
        }

        public String GetDocumentInfo()
        {
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
            return dt.GetName() + " " + GetDocumentNo();
        }

        public String GetSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetDocumentNo());
            //	: Total Lines = 123.00 (#1)
            //sb.Append(":")
            //    .Append(" (#").Append(GetLines(false).Length).Append(")");
            //	 - Description
            if (GetDescription() != null && GetDescription().Length > 0)
                sb.Append(" - ").Append(GetDescription());
            return sb.ToString();
        }

        public FileInfo CreatePDF()
        {
            return null;
        }

        public String GetProcessMsg()
        {
            return _processMsg;
        }

        public int GetDoc_User_ID()
        {
            return 0;
        }

        public Decimal GetApprovalAmt()
        {
            return Env.ZERO;
        }

        //public int GetC_Currency_ID()
        //{
        //    return GetCtx().GetContextAsInt("$C_Currency_ID ");
        //}

        public bool IsComplete()
        {
            String ds = GetDocStatus();
            return DOCSTATUS_Completed.Equals(ds)
                || DOCSTATUS_Closed.Equals(ds)
                || DOCSTATUS_Reversed.Equals(ds);
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
            return null;
        }

        public void SetProcessMsg(string processMsg)
        {

        }
    }
}
