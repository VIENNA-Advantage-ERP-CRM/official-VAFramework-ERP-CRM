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
    /** Generated Model for AD_WF_EventAudit
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_AD_WF_EventAudit : PO
    {
        public X_AD_WF_EventAudit(Context ctx, int AD_WF_EventAudit_ID, Trx trxName) : base(ctx, AD_WF_EventAudit_ID, trxName)
        {
            /** if (AD_WF_EventAudit_ID == 0)
{
SetAD_Table_ID (0);
SetAD_WF_EventAudit_ID (0);
SetAD_WF_Node_ID (0);
SetAD_WF_Process_ID (0);
SetAD_WF_Responsible_ID (0);
SetElapsedTimeMS (0.0);
SetEventType (null);
SetRecord_ID (0);
SetWFState (null);
}
             */
        }
        public X_AD_WF_EventAudit(Ctx ctx, int AD_WF_EventAudit_ID, Trx trxName) : base(ctx, AD_WF_EventAudit_ID, trxName)
        {
            /** if (AD_WF_EventAudit_ID == 0)
{
SetAD_Table_ID (0);
SetAD_WF_EventAudit_ID (0);
SetAD_WF_Node_ID (0);
SetAD_WF_Process_ID (0);
SetAD_WF_Responsible_ID (0);
SetElapsedTimeMS (0.0);
SetEventType (null);
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
        public X_AD_WF_EventAudit(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_AD_WF_EventAudit(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_AD_WF_EventAudit(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_AD_WF_EventAudit()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514366075L;
        /** Last Updated Timestamp 7/29/2010 1:07:29 PM */
        public static long updatedMS = 1280389049286L;
        /** AD_Table_ID=649 */
        public static int Table_ID;
        // =649;

        /** TableName=AD_WF_EventAudit */
        public static String Table_Name = "AD_WF_EventAudit";

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
            StringBuilder sb = new StringBuilder("X_AD_WF_EventAudit[").Append(Get_ID()).Append("]");
            return sb.ToString();
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

        /** AD_User_ID AD_Reference_ID=110 */
        public static int AD_USER_ID_AD_Reference_ID = 110;
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
        /** Set Workflow Event Audit.
@param AD_WF_EventAudit_ID Workflow Process Activity Event Audit Information */
        public void SetAD_WF_EventAudit_ID(int AD_WF_EventAudit_ID)
        {
            if (AD_WF_EventAudit_ID < 1) throw new ArgumentException("AD_WF_EventAudit_ID is mandatory.");
            Set_ValueNoCheck("AD_WF_EventAudit_ID", AD_WF_EventAudit_ID);
        }
        /** Get Workflow Event Audit.
@return Workflow Process Activity Event Audit Information */
        public int GetAD_WF_EventAudit_ID()
        {
            Object ii = Get_Value("AD_WF_EventAudit_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Get Record ID/ColumnName
@return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetAD_WF_EventAudit_ID().ToString());
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
        /** Set Workflow Process.
@param AD_WF_Process_ID Actual Workflow Process Instance */
        public void SetAD_WF_Process_ID(int AD_WF_Process_ID)
        {
            if (AD_WF_Process_ID < 1) throw new ArgumentException("AD_WF_Process_ID is mandatory.");
            Set_Value("AD_WF_Process_ID", AD_WF_Process_ID);
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
            if (AD_WF_Responsible_ID < 1) throw new ArgumentException("AD_WF_Responsible_ID is mandatory.");
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
        /** Set Attribute Name.
@param AttributeName Name of the Attribute */
        public void SetAttributeName(String AttributeName)
        {
            if (AttributeName != null && AttributeName.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                AttributeName = AttributeName.Substring(0, 60);
            }
            Set_Value("AttributeName", AttributeName);
        }
        /** Get Attribute Name.
@return Name of the Attribute */
        public String GetAttributeName()
        {
            return (String)Get_Value("AttributeName");
        }
        /** Set Description.
@param Description Optional short description of the record */
        public void SetDescription(String Description)
        {
            if (Description != null && Description.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                Description = Description.Substring(0, 255);
            }
            Set_Value("Description", Description);
        }
        /** Get Description.
@return Optional short description of the record */
        public String GetDescription()
        {
            return (String)Get_Value("Description");
        }
        /** Set Elapsed Time ms.
@param ElapsedTimeMS Elapsed Time in milliseconds */
        public void SetElapsedTimeMS(Decimal? ElapsedTimeMS)
        {
            if (ElapsedTimeMS == null) throw new ArgumentException("ElapsedTimeMS is mandatory.");
            Set_Value("ElapsedTimeMS", (Decimal?)ElapsedTimeMS);
        }
        /** Get Elapsed Time ms.
@return Elapsed Time in milliseconds */
        public Decimal GetElapsedTimeMS()
        {
            Object bd = Get_Value("ElapsedTimeMS");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** EventType AD_Reference_ID=306 */
        public static int EVENTTYPE_AD_Reference_ID = 306;
        /** Process Created = PC */
        public static String EVENTTYPE_ProcessCreated = "PC";
        /** Process Completed = PX */
        public static String EVENTTYPE_ProcessCompleted = "PX";
        /** State Changed = SC */
        public static String EVENTTYPE_StateChanged = "SC";
        /** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsEventTypeValid(String test)
        {
            return test.Equals("PC") || test.Equals("PX") || test.Equals("SC");
        }
        /** Set Event Type.
@param EventType Type of Event */
        public void SetEventType(String EventType)
        {
            if (EventType == null) throw new ArgumentException("EventType is mandatory");
            if (!IsEventTypeValid(EventType))
                throw new ArgumentException("EventType Invalid value - " + EventType + " - Reference_ID=306 - PC - PX - SC");
            if (EventType.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                EventType = EventType.Substring(0, 2);
            }
            Set_Value("EventType", EventType);
        }
        /** Get Event Type.
@return Type of Event */
        public String GetEventType()
        {
            return (String)Get_Value("EventType");
        }
        /** Set New Value.
@param NewValue New field value */
        public void SetNewValue(String NewValue)
        {
            if (NewValue != null && NewValue.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                NewValue = NewValue.Substring(0, 2000);
            }
            Set_Value("NewValue", NewValue);
        }
        /** Get New Value.
@return New field value */
        public String GetNewValue()
        {
            return (String)Get_Value("NewValue");
        }
        /** Set Old Value.
@param OldValue The old file data */
        public void SetOldValue(String OldValue)
        {
            if (OldValue != null && OldValue.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                OldValue = OldValue.Substring(0, 2000);
            }
            Set_Value("OldValue", OldValue);
        }
        /** Get Old Value.
@return The old file data */
        public String GetOldValue()
        {
            return (String)Get_Value("OldValue");
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
