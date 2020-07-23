/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : DocAction
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan      30-April-2009 
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Common;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using VAdvantage.SqlExec;
using System.Windows.Forms;


namespace VAdvantage.Process
{
    public class DocActionVariables
    {
        //Complete = CO
        public const string ACTION_COMPLETE = "CO";
        //Wait Complete = WC 
        public const string ACTION_WAITCOMPLETE = "WC";
        // Approve = AP 
        public const string ACTION_APPROVE = "AP";
        // Reject = RJ 
        public const string ACTION_REJECT = "RJ";
        // Post = PO 
        public const string ACTION_POST = "PO";
        // Void = VO 
        public const string ACTION_VOID = "VO";
        // Close = CL
        public const string ACTION_CLOSE = "CL";
        // Reverse - Correct = RC 
        public const string ACTION_REVERSE_CORRECT = "RC";
        // Reverse - Accrual = RA 
        public const string ACTION_REVERSE_ACCRUAL = "RA";
        // ReActivate = RE 
        public const string ACTION_REACTIVATE = "RE";
        // <None> = -- 
        public const string ACTION_NONE = "--";
        // Prepare = PR
        public const string ACTION_PREPARE = "PR";
        // Unlock = XL 
        public const string ACTION_UNLOCK = "XL";
        // Invalidate = IN 
        public const string ACTION_INVALIDATE = "IN";
        // ReOpen = OP 
        public const string ACTION_REOPEN = "OP";

        // Drafted = DR
        public const string STATUS_DRAFTED = "DR";
        // Completed = CO 
        public const string STATUS_COMPLETED = "CO";
        // Approved = AP 
        public const string STATUS_APPROVED = "AP";
        // Invalid = IN 
        public const string STATUS_INVALID = "IN";
        //Not Approved = NA 
        public const string STATUS_NOTAPPROVED = "NA";
        //Voided = VO 
        public const string STATUS_VOIDED = "VO";
        // Reversed = RE 
        public const string STATUS_REVERSED = "RE";
        // Closed = CL 
        public const string STATUS_CLOSED = "CL";
        // Unknown = ??
        public const string STATUS_UNKNOWN = "??";
        //In Progress = IP 
        public const string STATUS_INPROGRESS = "IP";
        // Waiting Payment = WP 
        public const string STATUS_WAITINGPAYMENT = "WP";
        //Waiting Confirmation = WC 
        public const string STATUS_WAITINGCONFIRMATION = "WC";
    }
    public interface DocAction
    {

        ////Complete = CO
        //public const string ACTION_COMPLETE = "CO";
        ////Wait Complete = WC 
        //public const string ACTION_WAITCOMPLETE = "WC";
        //// Approve = AP 
        //public const string ACTION_APPROVE = "AP";
        //// Reject = RJ 
        //public const string ACTION_REJECT = "RJ";
        //// Post = PO 
        //public const string ACTION_POST = "PO";
        //// Void = VO 
        //public const string ACTION_VOID = "VO";
        //// Close = CL
        //public const string ACTION_CLOSE = "CL";
        //// Reverse - Correct = RC 
        //public const string ACTION_REVERSE_CORRECT = "RC";
        //// Reverse - Accrual = RA 
        //public const string ACTION_REVERSE_ACCRUAL = "RA";
        //// ReActivate = RE 
        //public const string ACTION_REACTIVATE = "RE";
        //// <None> = -- 
        //public const string ACTION_NONE = "--";
        //// Prepare = PR
        //public const string ACTION_PREPARE = "PR";
        //// Unlock = XL 
        //public const string ACTION_UNLOCK = "XL";
        //// Invalidate = IN 
        //public const string ACTION_INVALIDATE = "IN";
        //// ReOpen = OP 
        //public const string ACTION_REOPEN = "OP";

        //// Drafted = DR
        //public const string STATUS_DRAFTED = "DR";
        //// Completed = CO 
        //public const string STATUS_COMPLETED = "CO";
        //// Approved = AP 
        //public const string STATUS_APPROVED = "AP";
        //// Invalid = IN 
        //public const string STATUS_INVALID = "IN";
        ////Not Approved = NA 
        //public const string STATUS_NOTAPPROVED = "NA";
        ////Voided = VO 
        //public const string STATUS_VOIDED = "VO";
        //// Reversed = RE 
        //public const string STATUS_REVERSED = "RE";
        //// Closed = CL 
        //public const string STATUS_CLOSED = "CL";
        //// Unknown = ??
        //public const string STATUS_UNKNOWN = "??";
        ////In Progress = IP 
        //public const string STATUS_INPROGRESS = "IP";
        //// Waiting Payment = WP 
        //public const string STATUS_WAITINGPAYMENT = "WP";
        ////Waiting Confirmation = WC 
        //public const string STATUS_WAITINGCONFIRMATION = "WC";


        /// <summary>
        /// Set Doc Status
        /// </summary>
        /// <param name="newStatus">new Status</param>
        void SetDocStatus(string newStatus);

        /// <summary>
        /// Get Doc Status
        /// </summary>
        /// <returns>Document Status</returns>
        String GetDocStatus();

        /// <summary>
        /// Process document
        /// </summary>
        /// <param name="action">document action</param>
        /// <returns>true if performed</returns>
        bool ProcessIt(String action);

        /// <summary>
        ///	Unlock Document.
        /// </summary>
        /// <returns>true if success </returns>
        bool UnlockIt();

        /// <summary>
        ///Invalidate Document
        /// </summary>
        /// <returns>true if success </returns>
        bool InvalidateIt();

        /// <summary>
        ///	Prepare Document
        /// </summary>
        /// <returns>new status (In Progress or Invalid) </returns>
        String PrepareIt();

        /// <summary>
        ///	Approve Document
        /// </summary>
        /// <returns>true if success </returns>
        bool ApproveIt();

        /// <summary>
        ///	Reject Approval
        /// </summary>
        /// <returns>true if success </returns>
        bool RejectIt();

        /// <summary>
        /// Complete Document
        /// </summary>
        /// <returns>new status (Complete, In Progress, Invalid, Waiting ..)</returns>
        String CompleteIt();

        /// <summary>
        ///	Void Document
        /// </summary>
        /// <returns>true if success </returns>
        bool VoidIt();

        /// <summary>
        /// Close Document
        /// </summary>
        /// <returns>true if success </returns>
        bool CloseIt();

        /// <summary>
        ///Reverse Correction
        /// </summary>
        /// <returns>true if success </returns>
        bool ReverseCorrectIt();

        /// <summary>
        /// Reverse Accrual
        /// </summary>
        /// <returns>true if success </returns>
        bool ReverseAccrualIt();

        /// <summary>
        /// Re-activate
        /// </summary>
        /// <returns>true if success </returns>
        bool ReActivateIt();

        /// <summary>
        ///Get Summary
        /// </summary>
        /// <returns>Summary of Document</returns>
        String GetSummary();

        /// <summary>
        /// Get Document No
        /// </summary>
        /// <returns>Document No</returns>
        String GetDocumentNo();

        /// <summary>
        ///	Get Document Info
        /// </summary>
        /// <returns>Type and Document No</returns>
        String GetDocumentInfo();

        /// <summary>
        /// Create PDF
        /// </summary>
        /// <returns>file</returns>
        FileInfo CreatePDF ();

        /// <summary>
        /// Get Process Message
        /// </summary>
        /// <returns>clear text message</returns>
        String GetProcessMsg();

        /// <summary>
        /// Get Document Owner
        /// </summary>
        /// <returns>AD_User_ID</returns>
        int GetDoc_User_ID();

        /// <summary>
        /// Get Document Currency
        /// </summary>
        /// <returns>C_Currency_ID</returns>
        int GetC_Currency_ID();

        /// <summary>
        /// Get Document Approval Amount
        /// </summary>
        /// <returns>amount</returns>
        Decimal GetApprovalAmt();

        /// <summary>
        /// Get Document Client
        /// </summary>
        /// <returns>AD_Client_ID</returns>
        int GetAD_Client_ID();

        /// <summary>
        /// Get Document Organization
        /// </summary>
        /// <returns>AD_Org_ID</returns>
        int GetAD_Org_ID();

        /// <summary>
        /// Get Doc Action
        /// </summary>
        /// <returns>Document Action</returns>
        String GetDocAction();

        /// <summary>
        /// Save Document
        /// </summary>
        /// <returns>true if saved</returns>
        bool Save();

        /// <summary>
        /// Get Context
        /// </summary>
        /// <returns>context</returns>
        Utility.Ctx GetCtx();

        /// <summary>
        /// Get ID of record
        /// </summary>
        /// <returns>ID</returns>
        int Get_ID();

        /// <summary>
        /// Get AD_Table_ID
        /// </summary>
        /// <returns>AD_Table_ID</returns>
        int Get_Table_ID();

        /// <summary>
        /// Get Logger
        /// </summary>
        /// <returns>logger</returns>
        //public CLogger get_Logger();..............

        /// <summary>
        /// Get Transaction
        /// </summary>
        /// <returns>trx name</returns>
        Trx Get_Trx();


        /**
        * Return the SQL to use to retrieve the document line organizations. This
        * is used to check whether the periods are open.
        * 
        * @return list of AD_Org_ID.  null if the lines do not need to be checked.
        */
        VAdvantage.Utility.Env.QueryParams GetLineOrgsQueryInfo();


        /**
         * Return the date to use for checking whether the periods are open.
         * 
         * @return
         */
        DateTime? GetDocumentDate();

        /**
         * Return the document base type for checking whether the periods are open.
         * 
         * @return
         */
        String GetDocBaseType();
    }
}
