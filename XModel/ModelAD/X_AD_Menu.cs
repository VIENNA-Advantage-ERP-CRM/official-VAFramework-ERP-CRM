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
    /** Generated Model for VAF_MenuConfig
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAF_MenuConfig : PO
    {
        public X_VAF_MenuConfig(Context ctx, int VAF_MenuConfig_ID, Trx trxName)
            : base(ctx, VAF_MenuConfig_ID, trxName)
        {
            /** if (VAF_MenuConfig_ID == 0)
            {
            SetVAF_MenuConfig_ID (0);
            SetEntityType (null);	// U
            SetIsReadOnly (false);	// N
            SetIsSummary (false);
            SetName (null);
            }
             */
        }
        public X_VAF_MenuConfig(Ctx ctx, int VAF_MenuConfig_ID, Trx trxName)
            : base(ctx, VAF_MenuConfig_ID, trxName)
        {
            /** if (VAF_MenuConfig_ID == 0)
            {
            SetVAF_MenuConfig_ID (0);
            SetEntityType (null);	// U
            SetIsReadOnly (false);	// N
            SetIsSummary (false);
            SetName (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_MenuConfig(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_MenuConfig(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAF_MenuConfig(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAF_MenuConfig()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID = 27562514362141L;
        /** Last Updated Timestamp 7/29/2010 1:07:25 PM */
        public static long updatedMS = 1280389045352L;
        /** VAF_TableView_ID=116 */
        public static int Table_ID;
        // =116;

        /** TableName=VAF_MenuConfig */
        public static String Table_Name = "VAF_MenuConfig";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(6);
        /** AccessLevel
        @return 6 - System - Client 
        */
        protected override int Get_AccessLevel()
        {
            return Convert.ToInt32(accessLevel.ToString());
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
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Ctx ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Info
        @return info
        */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_VAF_MenuConfig[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Special Form.
        @param VAF_Page_ID Special Form */
        public void SetVAF_Page_ID(int VAF_Page_ID)
        {
            if (VAF_Page_ID <= 0) Set_Value("VAF_Page_ID", null);
            else
                Set_Value("VAF_Page_ID", VAF_Page_ID);
        }
        /** Get Special Form.
        @return Special Form */
        public int GetVAF_Page_ID()
        {
            Object ii = Get_Value("VAF_Page_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Menu.
        @param VAF_MenuConfig_ID Identifies a Menu */
        public void SetVAF_MenuConfig_ID(int VAF_MenuConfig_ID)
        {
            if (VAF_MenuConfig_ID < 1) throw new ArgumentException("VAF_MenuConfig_ID is mandatory.");
            Set_ValueNoCheck("VAF_MenuConfig_ID", VAF_MenuConfig_ID);
        }
        /** Get Menu.
        @return Identifies a Menu */
        public int GetVAF_MenuConfig_ID()
        {
            Object ii = Get_Value("VAF_MenuConfig_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Process.
        @param AD_Process_ID Process or Report */
        public void SetAD_Process_ID(int AD_Process_ID)
        {
            if (AD_Process_ID <= 0) Set_Value("AD_Process_ID", null);
            else
                Set_Value("AD_Process_ID", AD_Process_ID);
        }
        /** Get Process.
        @return Process or Report */
        public int GetAD_Process_ID()
        {
            Object ii = Get_Value("AD_Process_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set OS Task.
        @param AD_Task_ID Operation System Task */
        public void SetAD_Task_ID(int AD_Task_ID)
        {
            if (AD_Task_ID <= 0) Set_Value("AD_Task_ID", null);
            else
                Set_Value("AD_Task_ID", AD_Task_ID);
        }
        /** Get OS Task.
        @return Operation System Task */
        public int GetAD_Task_ID()
        {
            Object ii = Get_Value("AD_Task_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Window.
        @param AD_Window_ID Data entry or display window */
        public void SetAD_Window_ID(int AD_Window_ID)
        {
            if (AD_Window_ID <= 0) Set_Value("AD_Window_ID", null);
            else
                Set_Value("AD_Window_ID", AD_Window_ID);
        }
        /** Get Window.
        @return Data entry or display window */
        public int GetAD_Window_ID()
        {
            Object ii = Get_Value("AD_Window_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Workbench.
        @param AD_Workbench_ID Collection of windows, reports */
        public void SetAD_Workbench_ID(int AD_Workbench_ID)
        {
            if (AD_Workbench_ID <= 0) Set_Value("AD_Workbench_ID", null);
            else
                Set_Value("AD_Workbench_ID", AD_Workbench_ID);
        }
        /** Get Workbench.
        @return Collection of windows, reports */
        public int GetAD_Workbench_ID()
        {
            Object ii = Get_Value("AD_Workbench_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Workflow.
        @param AD_Workflow_ID Workflow or combination of tasks */
        public void SetAD_Workflow_ID(int AD_Workflow_ID)
        {
            if (AD_Workflow_ID <= 0) Set_Value("AD_Workflow_ID", null);
            else
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

        /** Action AD_Reference_ID=104 */
        public static int ACTION_AD_Reference_ID = 104;
        /** Workbench = B */
        public static String ACTION_Workbench = "B";
        /** WorkFlow = F */
        public static String ACTION_WorkFlow = "F";
        /** Process = P */
        public static String ACTION_Process = "P";
        /** Report = R */
        public static String ACTION_Report = "R";
        /** Task = T */
        public static String ACTION_Task = "T";
        /** Window = W */
        public static String ACTION_Window = "W";
        /** Form = X */
        public static String ACTION_Form = "X";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsActionValid(String test)
        {
            return test == null || test.Equals("B") || test.Equals("F") || test.Equals("P") || test.Equals("R") || test.Equals("T") || test.Equals("W") || test.Equals("X");
        }
        /** Set Action.
        @param Action Indicates the Action to be performed */
        public void SetAction(String Action)
        {
            if (!IsActionValid(Action))
                throw new ArgumentException("Action Invalid value - " + Action + " - Reference_ID=104 - B - F - P - R - T - W - X");
            if (Action != null && Action.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                Action = Action.Substring(0, 1);
            }
            Set_Value("Action", Action);
        }
        /** Get Action.
        @return Indicates the Action to be performed */
        public String GetAction()
        {
            return (String)Get_Value("Action");
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

        /** EntityType AD_Reference_ID=389 */
        public static int ENTITYTYPE_AD_Reference_ID = 389;
        /** Set Entity Type.
        @param EntityType Dictionary Entity Type;
         Determines ownership and synchronization */
        public void SetEntityType(String EntityType)
        {
            if (EntityType.Length > 4)
            {
                log.Warning("Length > 4 - truncated");
                EntityType = EntityType.Substring(0, 4);
            }
            Set_Value("EntityType", EntityType);
        }
        /** Get Entity Type.
        @return Dictionary Entity Type;
         Determines ownership and synchronization */
        public String GetEntityType()
        {
            return (String)Get_Value("EntityType");
        }
        /** Set Read Only.
        @param IsReadOnly Field is read only */
        public void SetIsReadOnly(Boolean IsReadOnly)
        {
            Set_Value("IsReadOnly", IsReadOnly);
        }
        /** Get Read Only.
        @return Field is read only */
        public Boolean IsReadOnly()
        {
            Object oo = Get_Value("IsReadOnly");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Summary Level.
        @param IsSummary This is a summary entity */
        public void SetIsSummary(Boolean IsSummary)
        {
            Set_Value("IsSummary", IsSummary);
        }
        /** Get Summary Level.
        @return This is a summary entity */
        public Boolean IsSummary()
        {
            Object oo = Get_Value("IsSummary");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Name.
        @param Name Alphanumeric identifier of the entity */
        public void SetName(String Name)
        {
            if (Name == null) throw new ArgumentException("Name is mandatory.");
            if (Name.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                Name = Name.Substring(0, 60);
            }
            Set_Value("Name", Name);
        }
        /** Get Name.
        @return Alphanumeric identifier of the entity */
        public String GetName()
        {
            return (String)Get_Value("Name");
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetName());
        }

        /** Set Name.
@param Name Alphanumeric identifier of the entity */
        public void SetExport_ID(String Export_ID)
        {
            if (Export_ID == null) throw new ArgumentException("Export_ID is mandatory.");
            if (Export_ID.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                Export_ID = Export_ID.Substring(0, 60);
            }
            Set_Value("Export_ID", Export_ID);
        }

        public String GetExport_ID()
        {
            return (String)Get_Value("Export_ID");
        }
    }

}
