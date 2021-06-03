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
    /** Generated Model for VAF_WFlow_Task
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAF_WFlow_Task : PO
    {
        public X_VAF_WFlow_Task(Context ctx, int VAF_WFlow_Task_ID, Trx trxName)
            : base(ctx, VAF_WFlow_Task_ID, trxName)
        {
            /** if (VAF_WFlow_Task_ID == 0)
            {
            SetVAF_TableView_ID (0);
            SetVAF_WFlow_Task_ID (0);
            SetVAF_WFlow_Node_ID (0);
            SetVAF_WFlow_Job_ID (0);
            SetVAF_Workflow_ID (0);
            SetProcessed (false);	// N
            SetRecord_ID (0);
            SetWFState (null);
            }
             */
        }
        public X_VAF_WFlow_Task(Ctx ctx, int VAF_WFlow_Task_ID, Trx trxName)
            : base(ctx, VAF_WFlow_Task_ID, trxName)
        {
            /** if (VAF_WFlow_Task_ID == 0)
            {
            SetVAF_TableView_ID (0);
            SetVAF_WFlow_Task_ID (0);
            SetVAF_WFlow_Node_ID (0);
            SetVAF_WFlow_Job_ID (0);
            SetVAF_Workflow_ID (0);
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
        public X_VAF_WFlow_Task(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_WFlow_Task(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_WFlow_Task(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAF_WFlow_Task()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514365965L;
        /** Last Updated Timestamp 7/29/2010 1:07:29 PM */
        public static long updatedMS = 1280389049176L;
        /** VAF_TableView_ID=644 */
        public static int Table_ID;
        // =644;

        /** TableName=VAF_WFlow_Task */
        public static String Table_Name = "VAF_WFlow_Task";

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
            StringBuilder sb = new StringBuilder("X_VAF_WFlow_Task[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Message.
        @param VAF_Msg_Lable_ID System Message */
        public void SetVAF_Msg_Lable_ID(int VAF_Msg_Lable_ID)
        {
            if (VAF_Msg_Lable_ID <= 0) Set_Value("VAF_Msg_Lable_ID", null);
            else
                Set_Value("VAF_Msg_Lable_ID", VAF_Msg_Lable_ID);
        }
        /** Get Message.
        @return System Message */
        public int GetVAF_Msg_Lable_ID()
        {
            Object ii = Get_Value("VAF_Msg_Lable_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Table.
        @param VAF_TableView_ID Database Table information */
        public void SetVAF_TableView_ID(int VAF_TableView_ID)
        {
            if (VAF_TableView_ID < 1) throw new ArgumentException("VAF_TableView_ID is mandatory.");
            Set_Value("VAF_TableView_ID", VAF_TableView_ID);
        }
        /** Get Table.
        @return Database Table information */
        public int GetVAF_TableView_ID()
        {
            Object ii = Get_Value("VAF_TableView_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** VAF_UserContact_ID VAF_Control_Ref_ID=286 */
        public static int VAF_USERCONTACT_ID_VAF_Control_Ref_ID = 286;
        /** Set User/Contact.
        @param VAF_UserContact_ID User within the system - Internal or Business Partner Contact */
        public void SetVAF_UserContact_ID(int VAF_UserContact_ID)
        {
            if (VAF_UserContact_ID <= 0) Set_Value("VAF_UserContact_ID", null);
            else
                Set_Value("VAF_UserContact_ID", VAF_UserContact_ID);
        }
        /** Get User/Contact.
        @return User within the system - Internal or Business Partner Contact */
        public int GetVAF_UserContact_ID()
        {
            Object ii = Get_Value("VAF_UserContact_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Workflow Activity.
        @param VAF_WFlow_Task_ID Workflow Activity */
        public void SetVAF_WFlow_Task_ID(int VAF_WFlow_Task_ID)
        {
            if (VAF_WFlow_Task_ID < 1) throw new ArgumentException("VAF_WFlow_Task_ID is mandatory.");
            Set_ValueNoCheck("VAF_WFlow_Task_ID", VAF_WFlow_Task_ID);
        }
        /** Get Workflow Activity.
        @return Workflow Activity */
        public int GetVAF_WFlow_Task_ID()
        {
            Object ii = Get_Value("VAF_WFlow_Task_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Node.
        @param VAF_WFlow_Node_ID Workflow Node (activity), step or process */
        public void SetVAF_WFlow_Node_ID(int VAF_WFlow_Node_ID)
        {
            if (VAF_WFlow_Node_ID < 1) throw new ArgumentException("VAF_WFlow_Node_ID is mandatory.");
            Set_Value("VAF_WFlow_Node_ID", VAF_WFlow_Node_ID);
        }
        /** Get Node.
        @return Workflow Node (activity), step or process */
        public int GetVAF_WFlow_Node_ID()
        {
            Object ii = Get_Value("VAF_WFlow_Node_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetVAF_WFlow_Node_ID().ToString());
        }
        /** Set Workflow Process.
        @param VAF_WFlow_Job_ID Actual Workflow Process Instance */
        public void SetVAF_WFlow_Job_ID(int VAF_WFlow_Job_ID)
        {
            if (VAF_WFlow_Job_ID < 1) throw new ArgumentException("VAF_WFlow_Job_ID is mandatory.");
            Set_ValueNoCheck("VAF_WFlow_Job_ID", VAF_WFlow_Job_ID);
        }
        /** Get Workflow Process.
        @return Actual Workflow Process Instance */
        public int GetVAF_WFlow_Job_ID()
        {
            Object ii = Get_Value("VAF_WFlow_Job_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Workflow Responsible.
        @param VAF_WFlow_Incharge_ID Responsible for Workflow Execution */
        public void SetVAF_WFlow_Incharge_ID(int VAF_WFlow_Incharge_ID)
        {
            if (VAF_WFlow_Incharge_ID <= 0) Set_Value("VAF_WFlow_Incharge_ID", null);
            else
                Set_Value("VAF_WFlow_Incharge_ID", VAF_WFlow_Incharge_ID);
        }
        /** Get Workflow Responsible.
        @return Responsible for Workflow Execution */
        public int GetVAF_WFlow_Incharge_ID()
        {
            Object ii = Get_Value("VAF_WFlow_Incharge_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Workflow.
        @param VAF_Workflow_ID Workflow or combination of tasks */
        public void SetVAF_Workflow_ID(int VAF_Workflow_ID)
        {
            if (VAF_Workflow_ID < 1) throw new ArgumentException("VAF_Workflow_ID is mandatory.");
            Set_Value("VAF_Workflow_ID", VAF_Workflow_ID);
        }
        /** Get Workflow.
        @return Workflow or combination of tasks */
        public int GetVAF_Workflow_ID()
        {
            Object ii = Get_Value("VAF_Workflow_ID");
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

        /** WFState VAF_Control_Ref_ID=305 */
        public static int WFSTATE_VAF_Control_Ref_ID = 305;
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
        @param VAF_Screen_ID Data entry or display window */
        public void SetVAF_Screen_ID(int VAF_Screen_ID)
        {
            if (VAF_Screen_ID <= 0) Set_Value("VAF_Screen_ID", null);
            else
                Set_Value("VAF_Screen_ID", VAF_Screen_ID);
        }/** Get Window.
@return Data entry or display window */
        public int GetVAF_Screen_ID() { Object ii = Get_Value("VAF_Screen_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }

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

         /** ResponsibleOrg_ID VAF_Control_Ref_ID=276 */
        public static int RESPONSIBLEORG_ID_VAF_Control_Ref_ID = 276;
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
