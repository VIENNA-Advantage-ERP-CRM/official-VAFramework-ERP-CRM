namespace VAdvantage.Model
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
    using System.Data;
    /** Generated Model for AD_WF_Activity
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_AD_WF_Activity : PO
    {
        public X_AD_WF_Activity(Context ctx, int AD_WF_Activity_ID, Trx trxName)
            : base(ctx, AD_WF_Activity_ID, trxName)
        {
            /** if (AD_WF_Activity_ID == 0)
            {
            SetAD_Table_ID (0);
            SetAD_WF_Activity_ID (0);
            SetAD_WF_Node_ID (0);
            SetAD_WF_Process_ID (0);
            SetAD_Workflow_ID (0);
            SetProcessed (false);	// N
            SetRecord_ID (0);
            SetWFState (null);
            }
             */
        }
        public X_AD_WF_Activity(Ctx ctx, int AD_WF_Activity_ID, Trx trxName)
            : base(ctx, AD_WF_Activity_ID, trxName)
        {
            /** if (AD_WF_Activity_ID == 0)
            {
            SetAD_Table_ID (0);
            SetAD_WF_Activity_ID (0);
            SetAD_WF_Node_ID (0);
            SetAD_WF_Process_ID (0);
            SetAD_Workflow_ID (0);
            SetProcessed (false);	// N
            SetRecord_ID (0);
            SetWFState (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_WF_Activity(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_WF_Activity(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_WF_Activity(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_AD_WF_Activity()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514365965L;
        /** Last Updated Timestamp 7/29/2010 1:07:29 PM */
        public static long updatedMS = 1280389049176L;
        /** AD_Table_ID=644 */
        public static int Table_ID;
        // =644;

        /** TableName=AD_WF_Activity */
        public static String Table_Name = "AD_WF_Activity";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(7);
        /** AccessLevel
        @return 7 - System - Client - Org 
        */
        protected override int Get_AccessLevel()
        {
            return Convert.ToInt32(accessLevel.ToString());
        }
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Ctx ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Context ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Info
        @return info
        */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_AD_WF_Activity[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Message.
        @param AD_Message_ID System Message */
        public void SetAD_Message_ID(int AD_Message_ID)
        {
            if (AD_Message_ID <= 0) Set_Value("AD_Message_ID", null);
            else
                Set_Value("AD_Message_ID", AD_Message_ID);
        }
        /** Get Message.
        @return System Message */
        public int GetAD_Message_ID()
        {
            Object ii = Get_Value("AD_Message_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Table.
        @param AD_Table_ID Database Table information */
        public void SetAD_Table_ID(int AD_Table_ID)
        {
            if (AD_Table_ID < 1) throw new ArgumentException("AD_Table_ID is mandatory.");
            Set_Value("AD_Table_ID", AD_Table_ID);
        }
        /** Get Table.
        @return Database Table information */
        public int GetAD_Table_ID()
        {
            Object ii = Get_Value("AD_Table_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** AD_User_ID AD_Reference_ID=286 */
        public static int AD_USER_ID_AD_Reference_ID = 286;
        /** Set User/Contact.
        @param AD_User_ID User within the system - Internal or Business Partner Contact */
        public void SetAD_User_ID(int AD_User_ID)
        {
            if (AD_User_ID <= 0) Set_Value("AD_User_ID", null);
            else
                Set_Value("AD_User_ID", AD_User_ID);
        }
        /** Get User/Contact.
        @return User within the system - Internal or Business Partner Contact */
        public int GetAD_User_ID()
        {
            Object ii = Get_Value("AD_User_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Workflow Activity.
        @param AD_WF_Activity_ID Workflow Activity */
        public void SetAD_WF_Activity_ID(int AD_WF_Activity_ID)
        {
            if (AD_WF_Activity_ID < 1) throw new ArgumentException("AD_WF_Activity_ID is mandatory.");
            Set_ValueNoCheck("AD_WF_Activity_ID", AD_WF_Activity_ID);
        }
        /** Get Workflow Activity.
        @return Workflow Activity */
        public int GetAD_WF_Activity_ID()
        {
            Object ii = Get_Value("AD_WF_Activity_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Node.
        @param AD_WF_Node_ID Workflow Node (activity), step or process */
        public void SetAD_WF_Node_ID(int AD_WF_Node_ID)
        {
            if (AD_WF_Node_ID < 1) throw new ArgumentException("AD_WF_Node_ID is mandatory.");
            Set_Value("AD_WF_Node_ID", AD_WF_Node_ID);
        }
        /** Get Node.
        @return Workflow Node (activity), step or process */
        public int GetAD_WF_Node_ID()
        {
            Object ii = Get_Value("AD_WF_Node_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetAD_WF_Node_ID().ToString());
        }
        /** Set Workflow Process.
        @param AD_WF_Process_ID Actual Workflow Process Instance */
        public void SetAD_WF_Process_ID(int AD_WF_Process_ID)
        {
            if (AD_WF_Process_ID < 1) throw new ArgumentException("AD_WF_Process_ID is mandatory.");
            Set_ValueNoCheck("AD_WF_Process_ID", AD_WF_Process_ID);
        }
        /** Get Workflow Process.
        @return Actual Workflow Process Instance */
        public int GetAD_WF_Process_ID()
        {
            Object ii = Get_Value("AD_WF_Process_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Workflow Responsible.
        @param AD_WF_Responsible_ID Responsible for Workflow Execution */
        public void SetAD_WF_Responsible_ID(int AD_WF_Responsible_ID)
        {
            if (AD_WF_Responsible_ID <= 0) Set_Value("AD_WF_Responsible_ID", null);
            else
                Set_Value("AD_WF_Responsible_ID", AD_WF_Responsible_ID);
        }
        /** Get Workflow Responsible.
        @return Responsible for Workflow Execution */
        public int GetAD_WF_Responsible_ID()
        {
            Object ii = Get_Value("AD_WF_Responsible_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Workflow.
        @param AD_Workflow_ID Workflow or combination of tasks */
        public void SetAD_Workflow_ID(int AD_Workflow_ID)
        {
            if (AD_Workflow_ID < 1) throw new ArgumentException("AD_Workflow_ID is mandatory.");
            Set_Value("AD_Workflow_ID", AD_Workflow_ID);
        }
        /** Get Workflow.
        @return Workflow or combination of tasks */
        public int GetAD_Workflow_ID()
        {
            Object ii = Get_Value("AD_Workflow_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Last Alert.
        @param DateLastAlert Date when last alert were sent */
        public void SetDateLastAlert(DateTime? DateLastAlert)
        {
            Set_Value("DateLastAlert", (DateTime?)DateLastAlert);
        }
        /** Get Last Alert.
        @return Date when last alert were sent */
        public DateTime? GetDateLastAlert()
        {
            return (DateTime?)Get_Value("DateLastAlert");
        }
        /** Set Dyn Priority Start.
        @param DynPriorityStart Starting priority before changed dynamically */
        public void SetDynPriorityStart(int DynPriorityStart)
        {
            Set_Value("DynPriorityStart", DynPriorityStart);
        }
        /** Get Dyn Priority Start.
        @return Starting priority before changed dynamically */
        public int GetDynPriorityStart()
        {
            Object ii = Get_Value("DynPriorityStart");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set End Wait.
        @param EndWaitTime End of sleep time */
        public void SetEndWaitTime(DateTime? EndWaitTime)
        {
            Set_Value("EndWaitTime", (DateTime?)EndWaitTime);
        }
        /** Get End Wait.
        @return End of sleep time */
        public DateTime? GetEndWaitTime()
        {
            return (DateTime?)Get_Value("EndWaitTime");
        }
        /** Set Priority.
        @param Priority Indicates if this request is of a high, medium or low priority. */
        public void SetPriority(int Priority)
        {
            Set_Value("Priority", Priority);
        }
        /** Get Priority.
        @return Indicates if this request is of a high, medium or low priority. */
        public int GetPriority()
        {
            Object ii = Get_Value("Priority");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Processed.
        @param Processed The document has been processed */
        public void SetProcessed(Boolean Processed)
        {
            Set_Value("Processed", Processed);
        }
        /** Get Processed.
        @return The document has been processed */
        public Boolean IsProcessed()
        {
            Object oo = Get_Value("Processed");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Process Now.
        @param Processing Process Now */
        public void SetProcessing(Boolean Processing)
        {
            Set_Value("Processing", Processing);
        }
        /** Get Process Now.
        @return Process Now */
        public Boolean IsProcessing()
        {
            Object oo = Get_Value("Processing");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Record ID.
        @param Record_ID Direct internal record ID */
        public void SetRecord_ID(int Record_ID)
        {
            if (Record_ID < 0) throw new ArgumentException("Record_ID is mandatory.");
            Set_Value("Record_ID", Record_ID);
        }
        /** Get Record ID.
        @return Direct internal record ID */
        public int GetRecord_ID()
        {
            Object ii = Get_Value("Record_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Text Message.
        @param TextMsg Text Message */
        public void SetTextMsg(String TextMsg)
        {
            if (TextMsg != null && TextMsg.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                TextMsg = TextMsg.Substring(0, 2000);
            }
            Set_Value("TextMsg", TextMsg);
        }
        /** Get Text Message.
        @return Text Message */
        public String GetTextMsg()
        {
            return (String)Get_Value("TextMsg");
        }

        /** WFState AD_Reference_ID=305 */
        public static int WFSTATE_AD_Reference_ID = 305;
        /** Aborted = CA */
        public static String WFSTATE_Aborted = "CA";
        /** Completed = CC */
        public static String WFSTATE_Completed = "CC";
        /** Terminated = CT */
        public static String WFSTATE_Terminated = "CT";
        /** Not Started = ON */
        public static String WFSTATE_NotStarted = "ON";
        /** Running = OR */
        public static String WFSTATE_Running = "OR";
        /** Suspended = OS */
        public static String WFSTATE_Suspended = "OS";
        /** Background = BK */
        public static String WFSTATE_Background = "BK";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsWFStateValid(String test)
        {
            return test.Equals("CA") || test.Equals("CC") || test.Equals("CT") || test.Equals("ON") || test.Equals("OR") || test.Equals("OS") || test.Equals("BK");
        }
        /** Set Workflow State.
        @param WFState State of the execution of the workflow */
        public void SetWFState(String WFState)
        {
            if (WFState == null) throw new ArgumentException("WFState is mandatory");
            if (!IsWFStateValid(WFState))
                throw new ArgumentException("WFState Invalid value - " + WFState + " - Reference_ID=305 - CA - CC - CT - ON - OR - OS - BK");
            if (WFState.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                WFState = WFState.Substring(0, 2);
            }
            Set_Value("WFState", WFState);
        }
        /** Get Workflow State.
        @return State of the execution of the workflow */
        public String GetWFState()
        {
            return (String)Get_Value("WFState");
        }

        /** vinay bhatt Set Window.
        @param AD_Window_ID Data entry or display window */
        public void SetAD_Window_ID(int AD_Window_ID)
        {
            if (AD_Window_ID <= 0) Set_Value("AD_Window_ID", null);
            else
                Set_Value("AD_Window_ID", AD_Window_ID);
        }/** Get Window.
@return Data entry or display window */
        public int GetAD_Window_ID() { Object ii = Get_Value("AD_Window_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }

        ///<summary>
        /// SetIs Background
        ///</summary>
        ///<param name="IsBackground">If true then this runs at background</param>
        public void SetIsBackground(Boolean IsBackground)
        {
            Set_Value("IsBackground", IsBackground);
        }

        ///<summary>
        /// GetIs Background
        ///</summary>
        ///<returns> If true then this runs at background</returns>
        public Boolean IsBackground()
        {
            Object oo = Get_Value("IsBackground"); if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Summary.
        @param Summary */
        public void SetSummary(String Summary)
        {
            if (Summary != null && Summary.Length > 200)
            {
                log.Warning("Length > 200 - truncated");
                Summary = Summary.Substring(0, 200);
            }
            Set_Value("Summary", Summary);
        }
        /** Get Summary.
        @return Summary */
        public String GetSummary()
        {
            return (String)Get_Value("Summary");
        }

        /** ResponsibleOrg_ID AD_Reference_ID=276 */
        public static int RESPONSIBLEORG_ID_AD_Reference_ID = 276;
        /** Set Responsible Organization.
        @param ResponsibleOrg_ID Responsible Organization */
        public void SetResponsibleOrg_ID(int ResponsibleOrg_ID)
        {
            if (ResponsibleOrg_ID <= 0) 
                Set_Value("ResponsibleOrg_ID", null);
            else
                Set_Value("ResponsibleOrg_ID", ResponsibleOrg_ID);
        }
        /** Get Responsible Organization.
        @return Responsible Organization */
        public int GetResponsibleOrg_ID() 
        { 
            Object ii = Get_Value("ResponsibleOrg_ID"); 
            if (ii == null) 
                return 0; 
            return Convert.ToInt32(ii); 
        }
    }

}
