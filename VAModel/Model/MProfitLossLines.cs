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
    public class MProfitLossLines:X_C_ProfitLossLines,DocAction
    {
         public MProfitLossLines(Ctx ctx, int C_ProfitLossLines_ID, Trx trxName)
            : base(ctx, C_ProfitLossLines_ID, trxName)
        {

        }

        public MProfitLossLines(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        public void SetDocStatus(string newStatus)
        {
           
        }

        public string GetDocStatus()
        {
            return "";
        }

        public bool ProcessIt(string action)
        {
            return true;
        }

        public bool UnlockIt()
        {
            return true;
        }

        public bool InvalidateIt()
        {
            return true;
        }

        public string PrepareIt()
        {
            return "";
        }

        public bool ApproveIt()
        {
            return true;
        }

        public bool RejectIt()
        {
            return true;
        }

        public string CompleteIt()
        {
            return "";
        }

        public bool VoidIt()
        {
            return true;
        }

        public bool CloseIt()
        {
            return true;
        }

        public bool ReverseCorrectIt()
        {
            return true;
        }

        public bool ReverseAccrualIt()
        {
            return true;
        }

        public bool ReActivateIt()
        {
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
            return "";
        }

        public int GetDoc_User_ID()
        {
            return 0;
        }

        public int GetC_Currency_ID()
        {
            return 0;
        }

        public decimal GetApprovalAmt()
        {
            return 0;
        }

        public string GetDocAction()
        {
            return "";
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
    }
}
