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
    /** Generated Model for C_ProfitLoss
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_ProfitLoss : PO
    {
        public X_C_ProfitLoss(Context ctx, int C_ProfitLoss_ID, Trx trxName)
            : base(ctx, C_ProfitLoss_ID, trxName)
        {
            /** if (C_ProfitLoss_ID == 0)
            {
            SetC_ProfitLoss_ID (0);
            SetPosted (false);	// N
            }
             */
        }
        public X_C_ProfitLoss(Ctx ctx, int C_ProfitLoss_ID, Trx trxName)
            : base(ctx, C_ProfitLoss_ID, trxName)
        {
            /** if (C_ProfitLoss_ID == 0)
            {
            SetC_ProfitLoss_ID (0);
            SetPosted (false);	// N
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_ProfitLoss(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_ProfitLoss(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_ProfitLoss(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_C_ProfitLoss()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27682011782660L;
        /** Last Updated Timestamp 5/12/2014 2:51:06 PM */
        public static long updatedMS = 1399886465871L;
        /** AD_Table_ID=1000439 */
        public static int Table_ID;
        // =1000439;

        /** TableName=C_ProfitLoss */
        public static String Table_Name = "C_ProfitLoss";

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
            StringBuilder sb = new StringBuilder("X_C_ProfitLoss[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Currency.
        @param C_Currency_ID The Currency for this record */
        public void SetC_Currency_ID(int C_Currency_ID)
        {
            if (C_Currency_ID <= 0) Set_Value("C_Currency_ID", null);
            else
                Set_Value("C_Currency_ID", C_Currency_ID);
        }
        /** Get Currency.
        @return The Currency for this record */
        public int GetC_Currency_ID()
        {
            Object ii = Get_Value("C_Currency_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Document Type.
        @param C_DocType_ID Document type or rules */
        public void SetC_DocType_ID(int C_DocType_ID)
        {
            if (C_DocType_ID <= 0) Set_Value("C_DocType_ID", null);
            else
                Set_Value("C_DocType_ID", C_DocType_ID);
        }
        /** Get Document Type.
        @return Document type or rules */
        public int GetC_DocType_ID()
        {
            Object ii = Get_Value("C_DocType_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Profit Dimension.
        @param C_ProfitAndLoss_ID Profit Dimension */
        public void SetC_ProfitAndLoss_ID(int C_ProfitAndLoss_ID)
        {
            if (C_ProfitAndLoss_ID <= 0) Set_Value("C_ProfitAndLoss_ID", null);
            else
                Set_Value("C_ProfitAndLoss_ID", C_ProfitAndLoss_ID);
        }
        /** Get Profit Dimension.
        @return Profit Dimension */
        public int GetC_ProfitAndLoss_ID()
        {
            Object ii = Get_Value("C_ProfitAndLoss_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Profit Loss.
        @param C_ProfitLoss_ID Profit Loss */
        public void SetC_ProfitLoss_ID(int C_ProfitLoss_ID)
        {
            if (C_ProfitLoss_ID < 1) throw new ArgumentException("C_ProfitLoss_ID is mandatory.");
            Set_ValueNoCheck("C_ProfitLoss_ID", C_ProfitLoss_ID);
        }
        /** Get Profit Loss.
        @return Profit Loss */
        public int GetC_ProfitLoss_ID()
        {
            Object ii = Get_Value("C_ProfitLoss_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Year.
        @param C_Year_ID Calendar Year */
        public void SetC_Year_ID(int C_Year_ID)
        {
            if (C_Year_ID <= 0) Set_Value("C_Year_ID", null);
            else
                Set_Value("C_Year_ID", C_Year_ID);
        }
        /** Get Year.
        @return Calendar Year */
        public int GetC_Year_ID()
        {
            Object ii = Get_Value("C_Year_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Account Date.
        @param DateAcct General Ledger Date */
        public void SetDateAcct(DateTime? DateAcct)
        {
            Set_Value("DateAcct", (DateTime?)DateAcct);
        }
        /** Get Account Date.
        @return General Ledger Date */
        public DateTime? GetDateAcct()
        {
            return (DateTime?)Get_Value("DateAcct");
        }
        /** Set Transaction Date.
        @param DateTrx Transaction Date */
        public void SetDateTrx(DateTime? DateTrx)
        {
            Set_Value("DateTrx", (DateTime?)DateTrx);
        }
        /** Get Transaction Date.
        @return Transaction Date */
        public DateTime? GetDateTrx()
        {
            return (DateTime?)Get_Value("DateTrx");
        }

        /** DocAction AD_Reference_ID=135 */
        public static int DOCACTION_AD_Reference_ID = 135;
        /** <None> = -- */
        public static String DOCACTION_None = "--";
        /** Approve = AP */
        public static String DOCACTION_Approve = "AP";
        /** Close = CL */
        public static String DOCACTION_Close = "CL";
        /** Complete = CO */
        public static String DOCACTION_Complete = "CO";
        /** Invalidate = IN */
        public static String DOCACTION_Invalidate = "IN";
        /** Post = PO */
        public static String DOCACTION_Post = "PO";
        /** Prepare = PR */
        public static String DOCACTION_Prepare = "PR";
        /** Reverse - Accrual = RA */
        public static String DOCACTION_Reverse_Accrual = "RA";
        /** Reverse - Correct = RC */
        public static String DOCACTION_Reverse_Correct = "RC";
        /** Re-activate = RE */
        public static String DOCACTION_Re_Activate = "RE";
        /** Reject = RJ */
        public static String DOCACTION_Reject = "RJ";
        /** Void = VO */
        public static String DOCACTION_Void = "VO";
        /** Wait Complete = WC */
        public static String DOCACTION_WaitComplete = "WC";
        /** Unlock = XL */
        public static String DOCACTION_Unlock = "XL";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsDocActionValid(String test)
        {
            return test == null || test.Equals("--") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("IN") || test.Equals("PO") || test.Equals("PR") || test.Equals("RA") || test.Equals("RC") || test.Equals("RE") || test.Equals("RJ") || test.Equals("VO") || test.Equals("WC") || test.Equals("XL");
        }
        /** Set Document Action.
        @param DocAction The targeted status of the document */
        public void SetDocAction(String DocAction)
        {
            if (!IsDocActionValid(DocAction))
                throw new ArgumentException("DocAction Invalid value - " + DocAction + " - Reference_ID=135 - -- - AP - CL - CO - IN - PO - PR - RA - RC - RE - RJ - VO - WC - XL");
            if (DocAction != null && DocAction.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                DocAction = DocAction.Substring(0, 2);
            }
            Set_Value("DocAction", DocAction);
        }
        /** Get Document Action.
        @return The targeted status of the document */
        public String GetDocAction()
        {
            return (String)Get_Value("DocAction");
        }

        /** DocStatus AD_Reference_ID=131 */
        public static int DOCSTATUS_AD_Reference_ID = 131;
        /** Unknown = ?? */
        public static String DOCSTATUS_Unknown = "??";
        /** Approved = AP */
        public static String DOCSTATUS_Approved = "AP";
        /** Closed = CL */
        public static String DOCSTATUS_Closed = "CL";
        /** Completed = CO */
        public static String DOCSTATUS_Completed = "CO";
        /** Drafted = DR */
        public static String DOCSTATUS_Drafted = "DR";
        /** Invalid = IN */
        public static String DOCSTATUS_Invalid = "IN";
        /** In Progress = IP */
        public static String DOCSTATUS_InProgress = "IP";
        /** Not Approved = NA */
        public static String DOCSTATUS_NotApproved = "NA";
        /** Reversed = RE */
        public static String DOCSTATUS_Reversed = "RE";
        /** Voided = VO */
        public static String DOCSTATUS_Voided = "VO";
        /** Waiting Confirmation = WC */
        public static String DOCSTATUS_WaitingConfirmation = "WC";
        /** Waiting Payment = WP */
        public static String DOCSTATUS_WaitingPayment = "WP";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsDocStatusValid(String test)
        {
            return test == null || test.Equals("??") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("DR") || test.Equals("IN") || test.Equals("IP") || test.Equals("NA") || test.Equals("RE") || test.Equals("VO") || test.Equals("WC") || test.Equals("WP");
        }
        /** Set Document Status.
        @param DocStatus The current status of the document */
        public void SetDocStatus(String DocStatus)
        {
            if (!IsDocStatusValid(DocStatus))
                throw new ArgumentException("DocStatus Invalid value - " + DocStatus + " - Reference_ID=131 - ?? - AP - CL - CO - DR - IN - IP - NA - RE - VO - WC - WP");
            if (DocStatus != null && DocStatus.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                DocStatus = DocStatus.Substring(0, 2);
            }
            Set_Value("DocStatus", DocStatus);
        }
        /** Get Document Status.
        @return The current status of the document */
        public String GetDocStatus()
        {
            return (String)Get_Value("DocStatus");
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
            Set_Value("Export_ID", Export_ID);
        }
        /** Get Export.
        @return Export */
        public String GetExport_ID()
        {
            return (String)Get_Value("Export_ID");
        }
        /** Set Generate Lines.
        @param GenerateLines Generate component lines for Push supply type components  */
        public void SetGenerateLines(String GenerateLines)
        {
            if (GenerateLines != null && GenerateLines.Length > 10)
            {
                log.Warning("Length > 10 - truncated");
                GenerateLines = GenerateLines.Substring(0, 10);
            }
            Set_Value("GenerateLines", GenerateLines);
        }
        /** Get Generate Lines.
        @return Generate component lines for Push supply type components  */
        public String GetGenerateLines()
        {
            return (String)Get_Value("GenerateLines");
        }
        /** Set Approved.
        @param IsApproved Indicates if this document requires approval */
        public void SetIsApproved(Boolean IsApproved)
        {
            Set_Value("IsApproved", IsApproved);
        }
        /** Get Approved.
        @return Indicates if this document requires approval */
        public Boolean IsApproved()
        {
            Object oo = Get_Value("IsApproved");
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
            if (Name != null && Name.Length > 100)
            {
                log.Warning("Length > 100 - truncated");
                Name = Name.Substring(0, 100);
            }
            Set_Value("Name", Name);
        }
        /** Get Name.
        @return Alphanumeric identifier of the entity */
        public String GetName()
        {
            return (String)Get_Value("Name");
        }
        /** Set Posted.
        @param Posted Posting status */
        public void SetPosted(Boolean Posted)
        {
            Set_Value("Posted", Posted);
        }
        /** Get Posted.
        @return Posting status */
        public Boolean IsPosted()
        {
            Object oo = Get_Value("Posted");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
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
        /** Set Profit Before Tax.
        @param ProfitBeforeTax Profit Before Tax */
        public void SetProfitBeforeTax(Decimal? ProfitBeforeTax)
        {
            Set_Value("ProfitBeforeTax", (Decimal?)ProfitBeforeTax);
        }
        /** Get Profit Before Tax.
        @return Profit Before Tax */
        public Decimal GetProfitBeforeTax()
        {
            Object bd = Get_Value("ProfitBeforeTax");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Search Key.
        @param Value Search key for the record in the format required - must be unique */
        public void SetValue(String Value)
        {
            if (Value != null && Value.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Value = Value.Substring(0, 50);
            }
            Set_Value("Value", Value);
        }
        /** Get Search Key.
        @return Search key for the record in the format required - must be unique */
        public String GetValue()
        {
            return (String)Get_Value("Value");
        }


        /** Set Date From.
        @param DateFrom Starting date for a range */
        public void SetDateFrom(DateTime? DateFrom) 
        { 
            Set_Value("DateFrom", (DateTime?)DateFrom);
        }
        /** Get Date From.
        @return Starting date for a range */
        public DateTime? GetDateFrom() 
        { 
            return (DateTime?)Get_Value("DateFrom"); 
        }
        /** Set Date To.
        @param DateTo End date of a date range */
        public void SetDateTo(DateTime? DateTo) 
        { 
            Set_Value("DateTo", (DateTime?)DateTo); 
        }
        /** Get Date To.
        @return End date of a date range */
        public DateTime? GetDateTo() 
        { 
            return (DateTime?)Get_Value("DateTo"); 
        }
    }
}   
