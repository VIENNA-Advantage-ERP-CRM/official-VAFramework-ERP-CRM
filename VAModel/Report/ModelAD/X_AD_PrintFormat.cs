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
    /** Generated Model for AD_PrintFormat
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_AD_PrintFormat : PO
    {
        public X_AD_PrintFormat(Context ctx, int AD_PrintFormat_ID, Trx trxName)
            : base(ctx, AD_PrintFormat_ID, trxName)
        {
            /** if (AD_PrintFormat_ID == 0)
            {
            SetAD_PrintColor_ID (0);
            SetAD_PrintFont_ID (0);
            SetAD_PrintFormat_ID (0);
            SetAD_PrintPaper_ID (0);
            SetAD_Table_ID (0);
            SetFooterMargin (0);
            SetHeaderMargin (0);
            SetIsDefault (false);
            SetIsForm (false);
            SetIsStandardHeaderFooter (true);	// Y
            SetIsSuppressDupGroupBy (false);	// N
            SetIsTableBased (true);	// Y
            SetName (null);
            }
             */
        }
        public X_AD_PrintFormat(Ctx ctx, int AD_PrintFormat_ID, Trx trxName)
            : base(ctx, AD_PrintFormat_ID, trxName)
        {
            /** if (AD_PrintFormat_ID == 0)
            {
            SetAD_PrintColor_ID (0);
            SetAD_PrintFont_ID (0);
            SetAD_PrintFormat_ID (0);
            SetAD_PrintPaper_ID (0);
            SetAD_Table_ID (0);
            SetFooterMargin (0);
            SetHeaderMargin (0);
            SetIsDefault (false);
            SetIsForm (false);
            SetIsStandardHeaderFooter (true);	// Y
            SetIsSuppressDupGroupBy (false);	// N
            SetIsTableBased (true);	// Y
            SetName (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_PrintFormat(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_PrintFormat(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_PrintFormat(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_AD_PrintFormat()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID = 27673821570957L;
        /** Last Updated Timestamp 2/6/2014 7:47:34 PM */
        public static long updatedMS = 1391696254168L;
        /** AD_Table_ID=493 */
        public static int Table_ID;
        // =493;

        /** TableName=AD_PrintFormat */
        public static String Table_Name = "AD_PrintFormat";

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
            StringBuilder sb = new StringBuilder("X_AD_PrintFormat[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Business View.
        @param AD_BView_ID The logical subset of related data for the purposes of reporting */
        public void SetAD_BView_ID(int AD_BView_ID)
        {
            if (AD_BView_ID <= 0) Set_Value("AD_BView_ID", null);
            else
                Set_Value("AD_BView_ID", AD_BView_ID);
        }
        /** Get Business View.
        @return The logical subset of related data for the purposes of reporting */
        public int GetAD_BView_ID()
        {
            Object ii = Get_Value("AD_BView_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Print Color.
        @param AD_PrintColor_ID Color used for printing and display */
        public void SetAD_PrintColor_ID(int AD_PrintColor_ID)
        {
            if (AD_PrintColor_ID < 1) throw new ArgumentException("AD_PrintColor_ID is mandatory.");
            Set_Value("AD_PrintColor_ID", AD_PrintColor_ID);
        }
        /** Get Print Color.
        @return Color used for printing and display */
        public int GetAD_PrintColor_ID()
        {
            Object ii = Get_Value("AD_PrintColor_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Print Font.
        @param AD_PrintFont_ID Maintain Print Font */
        public void SetAD_PrintFont_ID(int AD_PrintFont_ID)
        {
            if (AD_PrintFont_ID < 1) throw new ArgumentException("AD_PrintFont_ID is mandatory.");
            Set_Value("AD_PrintFont_ID", AD_PrintFont_ID);
        }
        /** Get Print Font.
        @return Maintain Print Font */
        public int GetAD_PrintFont_ID()
        {
            Object ii = Get_Value("AD_PrintFont_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Print Format.
        @param AD_PrintFormat_ID Data Print Format */
        public void SetAD_PrintFormat_ID(int AD_PrintFormat_ID)
        {
            if (AD_PrintFormat_ID < 1) throw new ArgumentException("AD_PrintFormat_ID is mandatory.");
            Set_ValueNoCheck("AD_PrintFormat_ID", AD_PrintFormat_ID);
        }
        /** Get Print Format.
        @return Data Print Format */
        public int GetAD_PrintFormat_ID()
        {
            Object ii = Get_Value("AD_PrintFormat_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Print Paper.
        @param AD_PrintPaper_ID Printer paper definition */
        public void SetAD_PrintPaper_ID(int AD_PrintPaper_ID)
        {
            if (AD_PrintPaper_ID < 1) throw new ArgumentException("AD_PrintPaper_ID is mandatory.");
            Set_Value("AD_PrintPaper_ID", AD_PrintPaper_ID);
        }
        /** Get Print Paper.
        @return Printer paper definition */
        public int GetAD_PrintPaper_ID()
        {
            Object ii = Get_Value("AD_PrintPaper_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Print Table Format.
        @param AD_PrintTableFormat_ID Table Format in Reports */
        public void SetAD_PrintTableFormat_ID(int AD_PrintTableFormat_ID)
        {
            if (AD_PrintTableFormat_ID <= 0) Set_Value("AD_PrintTableFormat_ID", null);
            else
                Set_Value("AD_PrintTableFormat_ID", AD_PrintTableFormat_ID);
        }
        /** Get Print Table Format.
        @return Table Format in Reports */
        public int GetAD_PrintTableFormat_ID()
        {
            Object ii = Get_Value("AD_PrintTableFormat_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Report View.
        @param AD_ReportView_ID View used to generate this report */
        public void SetAD_ReportView_ID(int AD_ReportView_ID)
        {
            if (AD_ReportView_ID <= 0) Set_ValueNoCheck("AD_ReportView_ID", null);
            else
                Set_ValueNoCheck("AD_ReportView_ID", AD_ReportView_ID);
        }
        /** Get Report View.
        @return View used to generate this report */
        public int GetAD_ReportView_ID()
        {
            Object ii = Get_Value("AD_ReportView_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Tab.
        @param AD_Tab_ID Tab within a Window */
        public void SetAD_Tab_ID(int AD_Tab_ID)
        {
            if (AD_Tab_ID <= 0) Set_Value("AD_Tab_ID", null);
            else
                Set_Value("AD_Tab_ID", AD_Tab_ID);
        }
        /** Get Tab.
        @return Tab within a Window */
        public int GetAD_Tab_ID()
        {
            Object ii = Get_Value("AD_Tab_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Table.
        @param AD_Table_ID Database Table information */
        public void SetAD_Table_ID(int AD_Table_ID)
        {
            if (AD_Table_ID < 1) throw new ArgumentException("AD_Table_ID is mandatory.");
            Set_ValueNoCheck("AD_Table_ID", AD_Table_ID);
        }
        /** Get Table.
        @return Database Table information */
        public int GetAD_Table_ID()
        {
            Object ii = Get_Value("AD_Table_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Create Copy.
        @param CreateCopy Create Copy */
        public void SetCreateCopy(String CreateCopy)
        {
            if (CreateCopy != null && CreateCopy.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                CreateCopy = CreateCopy.Substring(0, 1);
            }
            Set_Value("CreateCopy", CreateCopy);
        }
        /** Get Create Copy.
        @return Create Copy */
        public String GetCreateCopy()
        {
            return (String)Get_Value("CreateCopy");
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
        /** Set Footer Margin.
        @param FooterMargin Margin of the Footer in 1/72 of an inch */
        public void SetFooterMargin(int FooterMargin)
        {
            Set_Value("FooterMargin", FooterMargin);
        }
        /** Get Footer Margin.
        @return Margin of the Footer in 1/72 of an inch */
        public int GetFooterMargin()
        {
            Object ii = Get_Value("FooterMargin");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Header Margin.
        @param HeaderMargin Margin of the Header in 1/72 of an inch */
        public void SetHeaderMargin(int HeaderMargin)
        {
            Set_Value("HeaderMargin", HeaderMargin);
        }
        /** Get Header Margin.
        @return Margin of the Header in 1/72 of an inch */
        public int GetHeaderMargin()
        {
            Object ii = Get_Value("HeaderMargin");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Check Print Format.
        @param IsCheckPrintFormat Check Print Format */
        public void SetIsCheckPrintFormat(Boolean IsCheckPrintFormat)
        {
            Set_Value("IsCheckPrintFormat", IsCheckPrintFormat);
        }
        /** Get Check Print Format.
        @return Check Print Format */
        public Boolean IsCheckPrintFormat()
        {
            Object oo = Get_Value("IsCheckPrintFormat");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Default.
        @param IsDefault Default value */
        public void SetIsDefault(Boolean IsDefault)
        {
            Set_Value("IsDefault", IsDefault);
        }
        /** Get Default.
        @return Default value */
        public Boolean IsDefault()
        {
            Object oo = Get_Value("IsDefault");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set For New Tenant.
        @param IsForNewTenant For New Tenant */
        public void SetIsForNewTenant(Boolean IsForNewTenant)
        {
            Set_Value("IsForNewTenant", IsForNewTenant);
        }
        /** Get For New Tenant.
        @return For New Tenant */
        public Boolean IsForNewTenant()
        {
            Object oo = Get_Value("IsForNewTenant");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Form.
        @param IsForm If Selected, a Form is printed, if not selected a columnar List report */
        public void SetIsForm(Boolean IsForm)
        {
            Set_Value("IsForm", IsForm);
        }
        /** Get Form.
        @return If Selected, a Form is printed, if not selected a columnar List report */
        public Boolean IsForm()
        {
            Object oo = Get_Value("IsForm");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Standard Header/Footer.
        @param IsStandardHeaderFooter The standard Header and Footer is used */
        public void SetIsStandardHeaderFooter(Boolean IsStandardHeaderFooter)
        {
            Set_Value("IsStandardHeaderFooter", IsStandardHeaderFooter);
        }
        /** Get Standard Header/Footer.
        @return The standard Header and Footer is used */
        public Boolean IsStandardHeaderFooter()
        {
            Object oo = Get_Value("IsStandardHeaderFooter");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Suppress Duplicate Group By.
        @param IsSuppressDupGroupBy Show Group By columns only in the first record for each unique combination */
        public void SetIsSuppressDupGroupBy(Boolean IsSuppressDupGroupBy)
        {
            Set_Value("IsSuppressDupGroupBy", IsSuppressDupGroupBy);
        }
        /** Get Suppress Duplicate Group By.
        @return Show Group By columns only in the first record for each unique combination */
        public Boolean IsSuppressDupGroupBy()
        {
            Object oo = Get_Value("IsSuppressDupGroupBy");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Table Based.
        @param IsTableBased Table based List Reporting */
        public void SetIsTableBased(Boolean IsTableBased)
        {
            Set_ValueNoCheck("IsTableBased", IsTableBased);
        }
        /** Get Table Based.
        @return Table based List Reporting */
        public Boolean IsTableBased()
        {
            Object oo = Get_Value("IsTableBased");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Totals Only.
        @param IsTotalsOnly Include only columns that represent totals in the print format */
        public void SetIsTotalsOnly(Boolean IsTotalsOnly)
        {
            Set_Value("IsTotalsOnly", IsTotalsOnly);
        }
        /** Get Totals Only.
        @return Include only columns that represent totals in the print format */
        public Boolean IsTotalsOnly()
        {
            Object oo = Get_Value("IsTotalsOnly");
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
        /** Set Printer Name.
        @param PrinterName Name of the Printer */
        public void SetPrinterName(String PrinterName)
        {
            if (PrinterName != null && PrinterName.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                PrinterName = PrinterName.Substring(0, 40);
            }
            Set_Value("PrinterName", PrinterName);
        }
        /** Get Printer Name.
        @return Name of the Printer */
        public String GetPrinterName()
        {
            return (String)Get_Value("PrinterName");
        }
        /** Set Ref PrintFormat.
        @param Ref_PrintFormat Ref PrintFormat */
        public void SetRef_PrintFormat(String Ref_PrintFormat)
        {
            if (Ref_PrintFormat != null && Ref_PrintFormat.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Ref_PrintFormat = Ref_PrintFormat.Substring(0, 50);
            }
            Set_Value("Ref_PrintFormat", Ref_PrintFormat);
        }
        /** Get Ref PrintFormat.
        @return Ref PrintFormat */
        public String GetRef_PrintFormat()
        {
            return (String)Get_Value("Ref_PrintFormat");
        }
    }

}
