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
    /** Generated Model for VAPA_FinancialReport
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAPA_FinancialReport : PO
    {
        public X_VAPA_FinancialReport(Context ctx, int VAPA_FinancialReport_ID, Trx trxName)
            : base(ctx, VAPA_FinancialReport_ID, trxName)
        {
            /** if (VAPA_FinancialReport_ID == 0)
            {
            SetListSources (false);
            SetListTrx (false);
            SetName (null);
            SetVAPA_FinancialReport_ID (0);
            SetProcessing (false);	// N
            }
             */
        }
        public X_VAPA_FinancialReport(Ctx ctx, int VAPA_FinancialReport_ID, Trx trxName)
            : base(ctx, VAPA_FinancialReport_ID, trxName)
        {
            /** if (VAPA_FinancialReport_ID == 0)
            {
            SetListSources (false);
            SetListTrx (false);
            SetName (null);
            SetVAPA_FinancialReport_ID (0);
            SetProcessing (false);	// N
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAPA_FinancialReport(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAPA_FinancialReport(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAPA_FinancialReport(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAPA_FinancialReport()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27721491778707L;
        /** Last Updated Timestamp 8/12/2015 1:31:03 PM */
        public static long updatedMS = 1439366461918L;
        /** VAF_TableView_ID=445 */
        public static int Table_ID;
        // =445;

        /** TableName=VAPA_FinancialReport */
        public static String Table_Name = "VAPA_FinancialReport";

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
            StringBuilder sb = new StringBuilder("X_VAPA_FinancialReport[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Print Format.
        @param VAF_Print_Rpt_Layout_ID Data Print Format */
        public void SetVAF_Print_Rpt_Layout_ID(int VAF_Print_Rpt_Layout_ID)
        {
            if (VAF_Print_Rpt_Layout_ID <= 0) Set_Value("VAF_Print_Rpt_Layout_ID", null);
            else
                Set_Value("VAF_Print_Rpt_Layout_ID", VAF_Print_Rpt_Layout_ID);
        }
        /** Get Print Format.
        @return Data Print Format */
        public int GetVAF_Print_Rpt_Layout_ID()
        {
            Object ii = Get_Value("VAF_Print_Rpt_Layout_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Accounting Schema.
        @param VAB_AccountBook_ID Rules for accounting */
        public void SetVAB_AccountBook_ID(int VAB_AccountBook_ID)
        {
            if (VAB_AccountBook_ID <= 0) Set_Value("VAB_AccountBook_ID", null);
            else
                Set_Value("VAB_AccountBook_ID", VAB_AccountBook_ID);
        }
        /** Get Accounting Schema.
        @return Rules for accounting */
        public int GetVAB_AccountBook_ID()
        {
            Object ii = Get_Value("VAB_AccountBook_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Calendar.
        @param VAB_Calender_ID Accounting Calendar Name */
        public void SetVAB_Calender_ID(int VAB_Calender_ID)
        {
            if (VAB_Calender_ID <= 0) Set_Value("VAB_Calender_ID", null);
            else
                Set_Value("VAB_Calender_ID", VAB_Calender_ID);
        }
        /** Get Calendar.
        @return Accounting Calendar Name */
        public int GetVAB_Calender_ID()
        {
            Object ii = Get_Value("VAB_Calender_ID");
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
        @param VAPA_FR_ColumnSet_ID Collection of Columns for Report */
        public void SetVAPA_FR_ColumnSet_ID(int VAPA_FR_ColumnSet_ID)
        {
            if (VAPA_FR_ColumnSet_ID <= 0) Set_Value("VAPA_FR_ColumnSet_ID", null);
            else
                Set_Value("VAPA_FR_ColumnSet_ID", VAPA_FR_ColumnSet_ID);
        }
        /** Get Report Column Set.
        @return Collection of Columns for Report */
        public int GetVAPA_FR_ColumnSet_ID()
        {
            Object ii = Get_Value("VAPA_FR_ColumnSet_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Report Line Set.
        @param VAPA_FR_RowSet_ID Report Line Set */
        public void SetVAPA_FR_RowSet_ID(int VAPA_FR_RowSet_ID)
        {
            if (VAPA_FR_RowSet_ID <= 0) Set_Value("VAPA_FR_RowSet_ID", null);
            else
                Set_Value("VAPA_FR_RowSet_ID", VAPA_FR_RowSet_ID);
        }
        /** Get Report Line Set.
        @return Report Line Set */
        public int GetVAPA_FR_RowSet_ID()
        {
            Object ii = Get_Value("VAPA_FR_RowSet_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Financial Report.
        @param VAPA_FinancialReport_ID Financial Report */
        public void SetVAPA_FinancialReport_ID(int VAPA_FinancialReport_ID)
        {
            if (VAPA_FinancialReport_ID < 1) throw new ArgumentException("VAPA_FinancialReport_ID is mandatory.");
            Set_ValueNoCheck("VAPA_FinancialReport_ID", VAPA_FinancialReport_ID);
        }
        /** Get Financial Report.
        @return Financial Report */
        public int GetVAPA_FinancialReport_ID()
        {
            Object ii = Get_Value("VAPA_FinancialReport_ID");
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
