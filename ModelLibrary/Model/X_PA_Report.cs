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
    /** Generated Model for PA_Report
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_PA_Report : PO
    {
        public X_PA_Report(Context ctx, int PA_Report_ID, Trx trxName)
            : base(ctx, PA_Report_ID, trxName)
        {
            /** if (PA_Report_ID == 0)
            {
            SetListSources (false);
            SetListTrx (false);
            SetName (null);
            SetPA_Report_ID (0);
            SetProcessing (false);	// N
            }
             */
        }
        public X_PA_Report(Ctx ctx, int PA_Report_ID, Trx trxName)
            : base(ctx, PA_Report_ID, trxName)
        {
            /** if (PA_Report_ID == 0)
            {
            SetListSources (false);
            SetListTrx (false);
            SetName (null);
            SetPA_Report_ID (0);
            SetProcessing (false);	// N
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_PA_Report(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_PA_Report(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_PA_Report(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_PA_Report()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27721491778707L;
        /** Last Updated Timestamp 8/12/2015 1:31:03 PM */
        public static long updatedMS = 1439366461918L;
        /** AD_Table_ID=445 */
        public static int Table_ID;
        // =445;

        /** TableName=PA_Report */
        public static String Table_Name = "PA_Report";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(3);
        /** AccessLevel
        @return 3 - Client - Org 
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
            StringBuilder sb = new StringBuilder("X_PA_Report[").Append(Get_ID()).Append("]");
            return sb.ToString();
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
        /** Set Accounting Schema.
        @param C_AcctSchema_ID Rules for accounting */
        public void SetC_AcctSchema_ID(int C_AcctSchema_ID)
        {
            if (C_AcctSchema_ID <= 0) Set_Value("C_AcctSchema_ID", null);
            else
                Set_Value("C_AcctSchema_ID", C_AcctSchema_ID);
        }
        /** Get Accounting Schema.
        @return Rules for accounting */
        public int GetC_AcctSchema_ID()
        {
            Object ii = Get_Value("C_AcctSchema_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Calendar.
        @param C_Calendar_ID Accounting Calendar Name */
        public void SetC_Calendar_ID(int C_Calendar_ID)
        {
            if (C_Calendar_ID <= 0) Set_Value("C_Calendar_ID", null);
            else
                Set_Value("C_Calendar_ID", C_Calendar_ID);
        }
        /** Get Calendar.
        @return Accounting Calendar Name */
        public int GetC_Calendar_ID()
        {
            Object ii = Get_Value("C_Calendar_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
        /** Set Export.
        @param Export_ID Export */
        public void SetExport_ID(String Export_ID)
        {
            if (Export_ID != null && Export_ID.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Export_ID = Export_ID.Substring(0, 50);
            }
            Set_ValueNoCheck("Export_ID", Export_ID);
        }
        /** Get Export.
        @return Export */
        public String GetExport_ID()
        {
            return (String)Get_Value("Export_ID");
        }
        /** Set List Sources.
        @param ListSources List Report Line Sources */
        public void SetListSources(Boolean ListSources)
        {
            Set_Value("ListSources", ListSources);
        }
        /** Get List Sources.
        @return List Report Line Sources */
        public Boolean IsListSources()
        {
            Object oo = Get_Value("ListSources");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set List Transactions.
        @param ListTrx List the report transactions */
        public void SetListTrx(Boolean ListTrx)
        {
            Set_Value("ListTrx", ListTrx);
        }
        /** Get List Transactions.
        @return List the report transactions */
        public Boolean IsListTrx()
        {
            Object oo = Get_Value("ListTrx");
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
        /** Set Report Column Set.
        @param PA_ReportColumnSet_ID Collection of Columns for Report */
        public void SetPA_ReportColumnSet_ID(int PA_ReportColumnSet_ID)
        {
            if (PA_ReportColumnSet_ID <= 0) Set_Value("PA_ReportColumnSet_ID", null);
            else
                Set_Value("PA_ReportColumnSet_ID", PA_ReportColumnSet_ID);
        }
        /** Get Report Column Set.
        @return Collection of Columns for Report */
        public int GetPA_ReportColumnSet_ID()
        {
            Object ii = Get_Value("PA_ReportColumnSet_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Report Line Set.
        @param PA_ReportLineSet_ID Report Line Set */
        public void SetPA_ReportLineSet_ID(int PA_ReportLineSet_ID)
        {
            if (PA_ReportLineSet_ID <= 0) Set_Value("PA_ReportLineSet_ID", null);
            else
                Set_Value("PA_ReportLineSet_ID", PA_ReportLineSet_ID);
        }
        /** Get Report Line Set.
        @return Report Line Set */
        public int GetPA_ReportLineSet_ID()
        {
            Object ii = Get_Value("PA_ReportLineSet_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Financial Report.
        @param PA_Report_ID Financial Report */
        public void SetPA_Report_ID(int PA_Report_ID)
        {
            if (PA_Report_ID < 1) throw new ArgumentException("PA_Report_ID is mandatory.");
            Set_ValueNoCheck("PA_Report_ID", PA_Report_ID);
        }
        /** Get Financial Report.
        @return Financial Report */
        public int GetPA_Report_ID()
        {
            Object ii = Get_Value("PA_Report_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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

        /** Set List Subgroup.
        @param FRPT_ListSubgroup List Subgroup */
        public void SetFRPT_ListSubgroup(Boolean FRPT_ListSubgroup)
        {
            Set_Value("FRPT_ListSubgroup", FRPT_ListSubgroup);
        }
        /** Get List Subgroup.
        @return List Subgroup */
        public Boolean IsFRPT_ListSubgroup()
        {
            Object oo = Get_Value("FRPT_ListSubgroup");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

    }

}
