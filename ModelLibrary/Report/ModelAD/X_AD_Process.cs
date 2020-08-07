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
    /** Generated Model for AD_Process
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_AD_Process : PO
    {
        public X_AD_Process(Context ctx, int AD_Process_ID, Trx trxName)
            : base(ctx, AD_Process_ID, trxName)
        {
            /** if (AD_Process_ID == 0)
            {
            SetAD_Process_ID (0);
            SetAccessLevel (null);
            SetEntityType (null);	// U
            SetIsBetaFunctionality (false);
            SetIsReport (false);
            SetIsServerProcess (false);
            SetName (null);
            SetValue (null);
            }
             */
        }
        public X_AD_Process(Ctx ctx, int AD_Process_ID, Trx trxName)
            : base(ctx, AD_Process_ID, trxName)
        {
            /** if (AD_Process_ID == 0)
            {
            SetAD_Process_ID (0);
            SetAccessLevel (null);
            SetEntityType (null);	// U
            SetIsBetaFunctionality (false);
            SetIsReport (false);
            SetIsServerProcess (false);
            SetName (null);
            SetValue (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_Process(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_Process(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_Process(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_AD_Process()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID = 27562514363175L;
        /** Last Updated Timestamp 7/29/2010 1:07:26 PM */
        public static long updatedMS = 1280389046386L;
        /** AD_Table_ID=284 */
        public static int Table_ID;
        // =284;

        /** TableName=AD_Process */
        public static String Table_Name = "AD_Process";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(4);
        /** AccessLevel
        @return 4 - System 
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
            StringBuilder sb = new StringBuilder("X_AD_Process[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Context Area.
        @param AD_CtxArea_ID Business Domain Area Terminology */
        public void SetAD_CtxArea_ID(int AD_CtxArea_ID)
        {
            if (AD_CtxArea_ID <= 0) Set_Value("AD_CtxArea_ID", null);
            else
                Set_Value("AD_CtxArea_ID", AD_CtxArea_ID);
        }
        /** Get Context Area.
        @return Business Domain Area Terminology */
        public int GetAD_CtxArea_ID()
        {
            Object ii = Get_Value("AD_CtxArea_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Print Format.
        @param AD_PrintFormat_ID Data Print Format */
        public void SetAD_PrintFormat_ID(int AD_PrintFormat_ID)
        {
            if (AD_PrintFormat_ID <= 0) Set_Value("AD_PrintFormat_ID", null);
            else
                Set_Value("AD_PrintFormat_ID", AD_PrintFormat_ID);
        }
        /** Get Print Format.
        @return Data Print Format */
        public int GetAD_PrintFormat_ID()
        {
            Object ii = Get_Value("AD_PrintFormat_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Process.
        @param AD_Process_ID Process or Report */
        public void SetAD_Process_ID(int AD_Process_ID)
        {
            if (AD_Process_ID < 1) throw new ArgumentException("AD_Process_ID is mandatory.");
            Set_ValueNoCheck("AD_Process_ID", AD_Process_ID);
        }
        /** Get Process.
        @return Process or Report */
        public int GetAD_Process_ID()
        {
            Object ii = Get_Value("AD_Process_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Report View.
        @param AD_ReportView_ID View used to generate this report */
        public void SetAD_ReportView_ID(int AD_ReportView_ID)
        {
            if (AD_ReportView_ID <= 0) Set_Value("AD_ReportView_ID", null);
            else
                Set_Value("AD_ReportView_ID", AD_ReportView_ID);
        }
        /** Get Report View.
        @return View used to generate this report */
        public int GetAD_ReportView_ID()
        {
            Object ii = Get_Value("AD_ReportView_ID");
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

        /** AccessLevel AD_Reference_ID=5 */
        public static int ACCESSLEVEL_AD_Reference_ID = 5;
        /** Organization = 1 */
        public static String ACCESSLEVEL_Organization = "1";
        /** Client only = 2 */
        public static String ACCESSLEVEL_ClientOnly = "2";
        /** Client+Organization = 3 */
        public static String ACCESSLEVEL_ClientPlusOrganization = "3";
        /** System only = 4 */
        public static String ACCESSLEVEL_SystemOnly = "4";
        /** System+Client = 6 */
        public static String ACCESSLEVEL_SystemPlusClient = "6";
        /** All = 7 */
        public static String ACCESSLEVEL_All = "7";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsAccessLevelValid(String test)
        {
            return test.Equals("1") || test.Equals("2") || test.Equals("3") || test.Equals("4") || test.Equals("6") || test.Equals("7");
        }
        /** Set Data Access Level.
        @param AccessLevel Access Level required */
        public void SetAccessLevel(String AccessLevel)
        {
            if (AccessLevel == null) throw new ArgumentException("AccessLevel is mandatory");
            if (!IsAccessLevelValid(AccessLevel))
                throw new ArgumentException("AccessLevel Invalid value - " + AccessLevel + " - Reference_ID=5 - 1 - 2 - 3 - 4 - 6 - 7");
            if (AccessLevel.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                AccessLevel = AccessLevel.Substring(0, 1);
            }
            Set_Value("AccessLevel", AccessLevel);
        }
        /** Get Data Access Level.
        @return Access Level required */
        public String GetAccessLevel()
        {
            return (String)Get_Value("AccessLevel");
        }
        /** Set Classname.
        @param Classname Java Classname */
        public void SetClassname(String Classname)
        {
            if (Classname != null && Classname.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                Classname = Classname.Substring(0, 60);
            }
            Set_Value("Classname", Classname);
        }
        /** Get Classname.
        @return Java Classname */
        public String GetClassname()
        {
            return (String)Get_Value("Classname");
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
        /** Set Comment.
        @param Help Comment, Help or Hint */
        public void SetHelp(String Help)
        {
            if (Help != null && Help.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                Help = Help.Substring(0, 2000);
            }
            Set_Value("Help", Help);
        }
        /** Get Comment.
        @return Comment, Help or Hint */
        public String GetHelp()
        {
            return (String)Get_Value("Help");
        }
        /** Set Beta Functionality.
        @param IsBetaFunctionality This functionality is considered Beta */
        public void SetIsBetaFunctionality(Boolean IsBetaFunctionality)
        {
            Set_Value("IsBetaFunctionality", IsBetaFunctionality);
        }
        /** Get Beta Functionality.
        @return This functionality is considered Beta */
        public Boolean IsBetaFunctionality()
        {
            Object oo = Get_Value("IsBetaFunctionality");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Direct print.
        @param IsDirectPrint Print without dialog */
        public void SetIsDirectPrint(Boolean IsDirectPrint)
        {
            Set_Value("IsDirectPrint", IsDirectPrint);
        }
        /** Get Direct print.
        @return Print without dialog */
        public Boolean IsDirectPrint()
        {
            Object oo = Get_Value("IsDirectPrint");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Report.
        @param IsReport Indicates a Report record */
        public void SetIsReport(Boolean IsReport)
        {
            Set_Value("IsReport", IsReport);
        }
        /** Get Report.
        @return Indicates a Report record */
        public Boolean IsReport()
        {
            Object oo = Get_Value("IsReport");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        ///** Set Report.
        //@param IsReport Indicates a Report record */
        //public void SetIsCrystalReport(Boolean IsReport)
        //{
        //    Set_Value("IsCrystalReport", IsReport);
        //}
        ///** Get Report.
        //@return Indicates a Report record */
        //public Boolean IsCrystalReport()
        //{
        //    Object oo = Get_Value("IsCrystalReport");
        //    if (oo != null)
        //    {
        //        if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
        //        return "Y".Equals(oo);
        //    }
        //    return false;
        //}



        /** IsCrystalReport AD_Reference_ID=1000193 */
        public static int ISCRYSTALREPORT_AD_Reference_ID = 1000193;/** BI Report = B */
        public static String ISCRYSTALREPORT_BIReport = "B";/** None = N */
        public static String ISCRYSTALREPORT_None = "N";/** Telerik Report = T */
        public static String ISCRYSTALREPORT_TelerikReport = "T";/** Crystal Report = Y */
        public static String ISCRYSTALREPORT_CrystalReport = "Y";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsIsCrystalReportValid(String test) { return test == null || test.Equals("B") || test.Equals("N") || test.Equals("T") || test.Equals("Y"); }/** Set Crystal Report.
@param IsCrystalReport Crystal Report */
        public void SetIsCrystalReport(String IsCrystalReport)
        {
            if (!IsIsCrystalReportValid(IsCrystalReport))
                throw new ArgumentException("IsCrystalReport Invalid value - " + IsCrystalReport + " - Reference_ID=1000193 - B - N - T - Y"); if (IsCrystalReport != null && IsCrystalReport.Length > 1) { log.Warning("Length > 1 - truncated"); IsCrystalReport = IsCrystalReport.Substring(0, 1); } Set_Value("IsCrystalReport", IsCrystalReport);
        }/** Get Crystal Report.
@return Crystal Report */
        public String GetIsCrystalReport() { return (String)Get_Value("IsCrystalReport"); }



        /** Set Server Process.
        @param IsServerProcess Run this Process on Server only */
        public void SetIsServerProcess(Boolean IsServerProcess)
        {
            Set_Value("IsServerProcess", IsServerProcess);
        }
        /** Get Server Process.
        @return Run this Process on Server only */
        public Boolean IsServerProcess()
        {
            Object oo = Get_Value("IsServerProcess");
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
        /** Set Procedure.
        @param ProcedureName Name of the Database Procedure */
        public void SetProcedureName(String ProcedureName)
        {
            if (ProcedureName != null && ProcedureName.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                ProcedureName = ProcedureName.Substring(0, 60);
            }
            Set_Value("ProcedureName", ProcedureName);
        }
        /** Get Procedure.
        @return Name of the Database Procedure */
        public String GetProcedureName()
        {
            return (String)Get_Value("ProcedureName");
        }
        /** Set Statistic Count.
        @param Statistic_Count Internal statistics how often the entity was used */
        public void SetStatistic_Count(int Statistic_Count)
        {
            Set_Value("Statistic_Count", Statistic_Count);
        }
        /** Get Statistic Count.
        @return Internal statistics how often the entity was used */
        public int GetStatistic_Count()
        {
            Object ii = Get_Value("Statistic_Count");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Statistic Seconds.
        @param Statistic_Seconds Internal statistics how many seconds a process took */
        public void SetStatistic_Seconds(Decimal? Statistic_Seconds)
        {
            Set_Value("Statistic_Seconds", (Decimal?)Statistic_Seconds);
        }
        /** Get Statistic Seconds.
        @return Internal statistics how many seconds a process took */
        public Decimal GetStatistic_Seconds()
        {
            Object bd = Get_Value("Statistic_Seconds");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Search Key.
        @param Value Search key for the record in the format required - must be unique */
        public void SetValue(String Value)
        {
            if (Value == null) throw new ArgumentException("Value is mandatory.");
            if (Value.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                Value = Value.Substring(0, 40);
            }
            Set_Value("Value", Value);
        }
        /** Get Search Key.
        @return Search key for the record in the format required - must be unique */
        public String GetValue()
        {
            return (String)Get_Value("Value");
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetValue());
        }
        /** Set Workflow Key.
        @param WorkflowValue Key of the Workflow to start */
        public void SetWorkflowValue(String WorkflowValue)
        {
            if (WorkflowValue != null && WorkflowValue.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                WorkflowValue = WorkflowValue.Substring(0, 40);
            }
            Set_Value("WorkflowValue", WorkflowValue);
        }
        /** Get Workflow Key.
        @return Key of the Workflow to start */
        public String GetWorkflowValue()
        {
            return (String)Get_Value("WorkflowValue");
        }

        /** Set AD_ReportFormat_ID.
        @param AD_ReportFormat AD_ReportFormat_ID */
        public void SetAD_ReportFormat_ID(int AD_ReportFormat_ID)
        {
            if (AD_ReportFormat_ID <= 0) Set_Value("AD_ReportFormat_ID", null);
            else
                Set_Value("AD_ReportFormat_ID", AD_ReportFormat_ID);
        }
        /** Get VARCOM_Report_ID.
        @return VARCOM_Report_ID */
        public int GetAD_ReportFormat_ID()
        {
            Object ii = Get_Value("AD_ReportFormat_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set BI Report ID.
@param VA039_BIReportID BI Report ID */
        public void SetVA039_BIReportID(String VA039_BIReportID) { if (VA039_BIReportID != null && VA039_BIReportID.Length > 250) { log.Warning("Length > 250 - truncated"); VA039_BIReportID = VA039_BIReportID.Substring(0, 250); } Set_Value("VA039_BIReportID", VA039_BIReportID); }/** Get BI Report ID.
@return BI Report ID */
        public String GetVA039_BIReportID() { return (String)Get_Value("VA039_BIReportID"); }
        /** Set Jasper Report Path.
        @param VA039_JasperReportPath Jasper Report Path */
        public void SetVA039_JasperReportPath(String VA039_JasperReportPath) { if (VA039_JasperReportPath != null && VA039_JasperReportPath.Length > 500) { log.Warning("Length > 500 - truncated"); VA039_JasperReportPath = VA039_JasperReportPath.Substring(0, 500); } Set_Value("VA039_JasperReportPath", VA039_JasperReportPath); }/** Get Jasper Report Path.
@return Jasper Report Path */
        public String GetVA039_JasperReportPath() { return (String)Get_Value("VA039_JasperReportPath"); }



        /** Set AD_ReportFormat_ID.
       @param AD_ReportFormat AD_ReportFormat_ID */
        public void SetAD_ReportMaster_ID(int AD_ReportMaster_ID)
        {
            if (AD_ReportMaster_ID <= 0) Set_Value("AD_ReportMaster_ID", null);
            else
                Set_Value("AD_ReportMaster_ID", AD_ReportMaster_ID);
        }
        /** Get VARCOM_Report_ID.
        @return VARCOM_Report_ID */
        public int GetAD_ReportMaster_ID()
        {
            Object ii = Get_Value("AD_ReportMaster_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

    }

}
