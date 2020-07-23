namespace ViennaAdvantage.Model
{
    /** Generated Model - DO NOT CHANGE */
    using System;
    using System.Text;
    using VAdvantage.DataBase;
    using VAdvantage.Common;
    using VAdvantage.Classes;
    using VAdvantage.Process;
    using VAdvantage.Model;
    using VAdvantage.Utility;
    using System.Data;/** Generated Model for AD_MailQueue
 *  @author Vienna Solutions 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_AD_MailQueue : PO
    {
        public X_AD_MailQueue(Context ctx, int AD_MailQueue_ID, Trx trxName)
            : base(ctx, AD_MailQueue_ID, trxName)
        {
            /** if (AD_MailQueue_ID == 0){SetAD_MailQueue_ID (0);SetToEMail (null);} */
        }
        public X_AD_MailQueue(Ctx ctx, int AD_MailQueue_ID, Trx trxName)
            : base(ctx, AD_MailQueue_ID, trxName)
        {
            /** if (AD_MailQueue_ID == 0){SetAD_MailQueue_ID (0);SetToEMail (null);} */
        }

        ///<summary>
        /// Load Constructor
        ///</summary>
        ///<param name="ctx"> Context </param>
        ///<param name="rs"> Result Set </param>
        ///<param name="trxName"> Transaction </param>
        public X_AD_MailQueue(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        ///<summary>
        /// Load Constructor
        ///</summary>
        ///<param name="ctx"> Context </param>
        ///<param name="rs"> Result Set </param>
        ///<param name="trxName"> Transaction </param>
        public X_AD_MailQueue(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        ///<summary>
        /// Load Constructor
        ///</summary>
        ///<param name="ctx"> Context </param>
        ///<param name="rs"> Result Set </param>
        ///<param name="trxName"> Transaction </param>
        public X_AD_MailQueue(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        ///<summary>
        /// Static Constructor 
        /// Set Table ID By Table Name
        ///</summary>
        static X_AD_MailQueue()
        {
            Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name);
        }

        ///<summary>
        /// Serial Version No.
        ///</summary>
        static long serialVersionUID = 27813001838034L;

        ///<summary>
        /// Last Updated Timestamp 7/6/2018 4:58:41 PM
        ///</summary>
        public static long updatedMS = 1530876521245L;

        ///<summary>
        /// AD_Table_ID=1000668
        ///</summary>
        public static int Table_ID; // =1000668;

        ///<summary>
        /// TableName=AD_MailQueue
        ///</summary>
        public static String Table_Name = "AD_MailQueue";
        protected static KeyNamePair model;

        protected Decimal accessLevel = new Decimal(7);
        ///<summary>
        /// AccessLevel 
        ///</summary>
        ///<returns>7 - System - Client - Org </returns>
        protected override int Get_AccessLevel()
        {
            return Convert.ToInt32(accessLevel.ToString());
        }

        ///<summary>
        /// Load Meta Data
        ///</summary>
        ///<param name="ctx"> Context </param>
        ///<returns> PO Info </returns>
        protected override POInfo InitPO(Context ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi;
        }

        ///<summary>
        /// Load Meta Data
        ///</summary>
        ///<param name="ctx"> Context </param>
        ///<returns> PO Info </returns>
        protected override POInfo InitPO(Ctx ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi;
        }

        ///<returns>Info </returns>
        ///<summary>
        /// Info 
        ///</summary>
        ///<returns> Info </returns>

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_AD_MailQueue[").Append(Get_ID()).Append("]"); return sb.ToString();
        }

        ///<summary>
        /// SetAD_MailQueue_ID
        ///</summary>
        ///<param name="AD_MailQueue_ID">AD_MailQueue_ID</param>
        public void SetAD_MailQueue_ID(int AD_MailQueue_ID)
        {
            if (AD_MailQueue_ID < 1) throw new ArgumentException("AD_MailQueue_ID is mandatory.");
            Set_ValueNoCheck("AD_MailQueue_ID", AD_MailQueue_ID);
        }

        ///<summary>
        /// GetAD_MailQueue_ID
        ///</summary>
        ///<returns> AD_MailQueue_ID</returns>
        public int GetAD_MailQueue_ID()
        {
            Object ii = Get_Value("AD_MailQueue_ID");
            if (ii == null) return 0; return Convert.ToInt32(ii);
        }

        ///<summary>
        /// SetRole
        ///</summary>
        ///<param name="AD_Role_ID">Responsibility Role</param>
        public void SetAD_Role_ID(int AD_Role_ID)
        {
            if (AD_Role_ID <= 0) Set_Value("AD_Role_ID", null);
            else
                Set_Value("AD_Role_ID", AD_Role_ID);
        }

        ///<summary>
        /// GetRole
        ///</summary>
        ///<returns> Responsibility Role</returns>
        public int GetAD_Role_ID()
        {
            Object ii = Get_Value("AD_Role_ID");
            if (ii == null) return 0; return Convert.ToInt32(ii);
        }

        ///<summary>
        /// SetTable
        ///</summary>
        ///<param name="AD_Table_ID">Database Table information</param>
        public void SetAD_Table_ID(int AD_Table_ID)
        {
            if (AD_Table_ID <= 0) Set_Value("AD_Table_ID", null);
            else
                Set_Value("AD_Table_ID", AD_Table_ID);
        }

        ///<summary>
        /// GetTable
        ///</summary>
        ///<returns> Database Table information</returns>
        public int GetAD_Table_ID()
        {
            Object ii = Get_Value("AD_Table_ID");
            if (ii == null) return 0; return Convert.ToInt32(ii);
        }

        ///<summary>
        /// SetWorkflow Activity
        ///</summary>
        ///<param name="AD_WF_Activity_ID">Workflow Activity</param>
        public void SetAD_WF_Activity_ID(int AD_WF_Activity_ID)
        {
            if (AD_WF_Activity_ID <= 0) Set_Value("AD_WF_Activity_ID", null);
            else
                Set_Value("AD_WF_Activity_ID", AD_WF_Activity_ID);
        }

        ///<summary>
        /// GetWorkflow Activity
        ///</summary>
        ///<returns> Workflow Activity</returns>
        public int GetAD_WF_Activity_ID()
        {
            Object ii = Get_Value("AD_WF_Activity_ID");
            if (ii == null) return 0; return Convert.ToInt32(ii);
        }

        ///<summary>
        /// SetWorkflow Event Audit
        ///</summary>
        ///<param name="AD_WF_EventAudit_ID">Workflow Process Activity Event Audit Information</param>
        public void SetAD_WF_EventAudit_ID(int AD_WF_EventAudit_ID)
        {
            if (AD_WF_EventAudit_ID <= 0) Set_Value("AD_WF_EventAudit_ID", null);
            else
                Set_Value("AD_WF_EventAudit_ID", AD_WF_EventAudit_ID);
        }

        ///<summary>
        /// GetWorkflow Event Audit
        ///</summary>
        ///<returns> Workflow Process Activity Event Audit Information</returns>
        public int GetAD_WF_EventAudit_ID()
        {
            Object ii = Get_Value("AD_WF_EventAudit_ID");
            if (ii == null) return 0; return Convert.ToInt32(ii);
        }

        ///<summary>
        /// SetWorkflow Process
        ///</summary>
        ///<param name="AD_WF_Process_ID">Actual Workflow Process Instance</param>
        public void SetAD_WF_Process_ID(int AD_WF_Process_ID)
        {
            if (AD_WF_Process_ID <= 0) Set_Value("AD_WF_Process_ID", null);
            else
                Set_Value("AD_WF_Process_ID", AD_WF_Process_ID);
        }

        ///<summary>
        /// GetWorkflow Process
        ///</summary>
        ///<returns> Actual Workflow Process Instance</returns>
        public int GetAD_WF_Process_ID()
        {
            Object ii = Get_Value("AD_WF_Process_ID");
            if (ii == null) return 0; return Convert.ToInt32(ii);
        }

        ///<summary>
        /// SetExport
        ///</summary>
        ///<param name="Export_ID">Export</param>
        public void SetExport_ID(String Export_ID)
        {
            if (Export_ID != null && Export_ID.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Export_ID = Export_ID.Substring(0, 50);
            }
            Set_Value("Export_ID", Export_ID);
        }

        ///<summary>
        /// GetExport
        ///</summary>
        ///<returns> Export</returns>
        public String GetExport_ID()
        {
            return (String)Get_Value("Export_ID");
        }

        ///<summary>
        /// SetIs Html Email
        ///</summary>
        ///<param name="IsHtmlEmail">Text has html tags</param>
        public void SetIsHtmlEmail(Boolean IsHtmlEmail)
        {
            Set_Value("IsHtmlEmail", IsHtmlEmail);
        }

        ///<summary>
        /// GetIs Html Email
        ///</summary>
        ///<returns> Text has html tags</returns>
        public Boolean IsHtmlEmail()
        {
            Object oo = Get_Value("IsHtmlEmail"); if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        ///<summary>
        /// SetMail Message
        ///</summary>
        ///<param name="MailMessage">Content of the mail</param>
        public void SetMailMessage(String MailMessage)
        {
            Set_Value("MailMessage", MailMessage);
        }

        ///<summary>
        /// GetMail Message
        ///</summary>
        ///<returns> Content of the mail</returns>
        public String GetMailMessage()
        {
            return (String)Get_Value("MailMessage");
        }

        ///<summary>
        /// MailStatus AD_Reference_ID=1000243
        ///</summary>
        public static int MAILSTATUS_AD_Reference_ID = 1000243;
        ///<summary>
        /// Failed = F
        ///</summary>
        public static String MAILSTATUS_Failed = "F";
        ///<summary>
        /// In Queue = Q
        ///</summary>
        public static String MAILSTATUS_InQueue = "Q";
        ///<summary>
        /// Sent = S
        ///</summary>
        public static String MAILSTATUS_Sent = "S";

        ///<summary>
        /// Is test a valid value.
        ///</summary>
        ///<param name="test"> TestValue </param>
        ///<returns> True If Valid </returns>
        public bool IsMailStatusValid(String test)
        {
            return test == null || test.Equals("F") || test.Equals("Q") || test.Equals("S");
        }

        ///<summary>
        /// SetMail Status
        ///</summary>
        ///<param name="MailStatus">Status of email: sent, failed, or in queue</param>
        public void SetMailStatus(String MailStatus)
        {
            if (!IsMailStatusValid(MailStatus))
                throw new ArgumentException("MailStatus Invalid value - " + MailStatus + " - Reference_ID=1000243 - F - Q - S");
            if (MailStatus != null && MailStatus.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                MailStatus = MailStatus.Substring(0, 1);
            }
            Set_Value("MailStatus", MailStatus);
        }

        ///<summary>
        /// GetMail Status
        ///</summary>
        ///<returns> Status of email: sent, failed, or in queue</returns>
        public String GetMailStatus()
        {
            return (String)Get_Value("MailStatus");
        }

        ///<summary>
        /// SetMail Subject
        ///</summary>
        ///<param name="MailSubject">Subject of the email</param>
        public void SetMailSubject(String MailSubject)
        {
            if (MailSubject != null && MailSubject.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                MailSubject = MailSubject.Substring(0, 2000);
            }
            Set_Value("MailSubject", MailSubject);
        }

        ///<summary>
        /// GetMail Subject
        ///</summary>
        ///<returns> Subject of the email</returns>
        public String GetMailSubject()
        {
            return (String)Get_Value("MailSubject");
        }

        ///<summary>
        /// SetRecord ID
        ///</summary>
        ///<param name="Record_ID">Direct internal record ID</param>
        public void SetRecord_ID(int Record_ID)
        {
            if (Record_ID <= 0) Set_Value("Record_ID", null);
            else
                Set_Value("Record_ID", Record_ID);
        }

        ///<summary>
        /// GetRecord ID
        ///</summary>
        ///<returns> Direct internal record ID</returns>
        public int GetRecord_ID()
        {
            Object ii = Get_Value("Record_ID");
            if (ii == null) return 0; return Convert.ToInt32(ii);
        }

        ///<summary>
        /// SetTo EMail
        ///</summary>
        ///<param name="ToEMail">Email of user to whom mail will be sent</param>
        public void SetToEMail(String ToEMail)
        {
            if (ToEMail == null) throw new ArgumentException("ToEMail is mandatory.");
            if (ToEMail.Length > 100)
            {
                log.Warning("Length > 100 - truncated");
                ToEMail = ToEMail.Substring(0, 100);
            }
            Set_Value("ToEMail", ToEMail);
        }

        ///<summary>
        /// GetTo EMail
        ///</summary>
        ///<returns> Email of user to whom mail will be sent</returns>
        public String GetToEMail()
        {
            return (String)Get_Value("ToEMail");
        }

        ///<summary>
        /// SetTo Name
        ///</summary>
        ///<param name="ToName">Name of the email receiving person</param>
        public void SetToName(String ToName)
        {
            if (ToName != null && ToName.Length > 100)
            {
                log.Warning("Length > 100 - truncated");
                ToName = ToName.Substring(0, 100);
            }
            Set_Value("ToName", ToName);
        }

        ///<summary>
        /// GetTo Name
        ///</summary>
        ///<returns> Name of the email receiving person</returns>
        public String GetToName()
        {
            return (String)Get_Value("ToName");
        }
    }
}