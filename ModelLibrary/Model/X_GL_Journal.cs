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
    /** Generated Model for GL_Journal
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_GL_Journal : PO
    {
        public X_GL_Journal(Context ctx, int GL_Journal_ID, Trx trxName)
            : base(ctx, GL_Journal_ID, trxName)
        {
            /** if (GL_Journal_ID == 0)
            {
            SetC_AcctSchema_ID (0);	// @$C_AcctSchema_ID@
            SetC_ConversionType_ID (0);
            SetC_DocType_ID (0);	// @C_DocType_ID@
            SetC_Period_ID (0);	// @C_Period_ID@
            SetCurrencyRate (0.0);	// 1
            SetDateAcct (DateTime.Now);	// @DateAcct@
            SetDateDoc (DateTime.Now);	// @DateDoc@
            SetDescription (null);
            SetDocAction (null);	// CO
            SetDocStatus (null);	// DR
            SetDocumentNo (null);
            SetGL_Category_ID (0);	// @GL_Category_ID@
            SetGL_Journal_ID (0);
            SetIsApproved (true);	// Y
            SetIsPrinted (false);	// N
            SetPosted (false);	// N
            SetPostingType (null);	// @PostingType@
            SetTotalCr (0.0);	// 0
            SetTotalDr (0.0);	// 0
            }
             */
        }
        public X_GL_Journal(Ctx ctx, int GL_Journal_ID, Trx trxName)
            : base(ctx, GL_Journal_ID, trxName)
        {
            /** if (GL_Journal_ID == 0)
            {
            SetC_AcctSchema_ID (0);	// @$C_AcctSchema_ID@
            SetC_ConversionType_ID (0);
            SetC_DocType_ID (0);	// @C_DocType_ID@
            SetC_Period_ID (0);	// @C_Period_ID@
            SetCurrencyRate (0.0);	// 1
            SetDateAcct (DateTime.Now);	// @DateAcct@
            SetDateDoc (DateTime.Now);	// @DateDoc@
            SetDescription (null);
            SetDocAction (null);	// CO
            SetDocStatus (null);	// DR
            SetDocumentNo (null);
            SetGL_Category_ID (0);	// @GL_Category_ID@
            SetGL_Journal_ID (0);
            SetIsApproved (true);	// Y
            SetIsPrinted (false);	// N
            SetPosted (false);	// N
            SetPostingType (null);	// @PostingType@
            SetTotalCr (0.0);	// 0
            SetTotalDr (0.0);	// 0
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_GL_Journal(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_GL_Journal(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_GL_Journal(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_GL_Journal()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27636141469534L;
        /** Last Updated Timestamp 11/27/2012 5:05:52 PM */
        public static long updatedMS = 1354016152745L;
        /** AD_Table_ID=224 */
        public static int Table_ID;
        // =224;

        /** TableName=GL_Journal */
        public static String Table_Name = "GL_Journal";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(1);
        /** AccessLevel
        @return 1 - Org 
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
            StringBuilder sb = new StringBuilder("X_GL_Journal[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Accounting Schema.
        @param C_AcctSchema_ID Rules for accounting */
        public void SetC_AcctSchema_ID(int C_AcctSchema_ID)
        {
            if (C_AcctSchema_ID < 1) throw new ArgumentException("C_AcctSchema_ID is mandatory.");
            Set_ValueNoCheck("C_AcctSchema_ID", C_AcctSchema_ID);
        }
        /** Get Accounting Schema.
        @return Rules for accounting */
        public int GetC_AcctSchema_ID()
        {
            Object ii = Get_Value("C_AcctSchema_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Currency Type.
        @param C_ConversionType_ID Currency Conversion Rate Type */
        public void SetC_ConversionType_ID(int C_ConversionType_ID)
        {
            if (C_ConversionType_ID < 1) throw new ArgumentException("C_ConversionType_ID is mandatory.");
            Set_Value("C_ConversionType_ID", C_ConversionType_ID);
        }
        /** Get Currency Type.
        @return Currency Conversion Rate Type */
        public int GetC_ConversionType_ID()
        {
            Object ii = Get_Value("C_ConversionType_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
            if (C_DocType_ID < 0) throw new ArgumentException("C_DocType_ID is mandatory.");
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

        /** C_Period_ID AD_Reference_ID=275 */
        public static int C_PERIOD_ID_AD_Reference_ID = 275;
        /** Set Period.
        @param C_Period_ID Period of the Calendar */
        public void SetC_Period_ID(int C_Period_ID)
        {
            if (C_Period_ID < 1) throw new ArgumentException("C_Period_ID is mandatory.");
            Set_Value("C_Period_ID", C_Period_ID);
        }
        /** Get Period.
        @return Period of the Calendar */
        public int GetC_Period_ID()
        {
            Object ii = Get_Value("C_Period_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Control Amount.
        @param ControlAmt If not zero, the Debit amount of the document must be equal this amount */
        public void SetControlAmt(Decimal? ControlAmt)
        {
            Set_Value("ControlAmt", (Decimal?)ControlAmt);
        }
        /** Get Control Amount.
        @return If not zero, the Debit amount of the document must be equal this amount */
        public Decimal GetControlAmt()
        {
            Object bd = Get_Value("ControlAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Rate.
        @param CurrencyRate Currency Conversion Rate */
        public void SetCurrencyRate(Decimal? CurrencyRate)
        {
            if (CurrencyRate == null) throw new ArgumentException("CurrencyRate is mandatory.");
            Set_Value("CurrencyRate", (Decimal?)CurrencyRate);
        }
        /** Get Rate.
        @return Currency Conversion Rate */
        public Decimal GetCurrencyRate()
        {
            Object bd = Get_Value("CurrencyRate");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Account Date.
        @param DateAcct General Ledger Date */
        public void SetDateAcct(DateTime? DateAcct)
        {
            if (DateAcct == null) throw new ArgumentException("DateAcct is mandatory.");
            Set_Value("DateAcct", (DateTime?)DateAcct);
        }
        /** Get Account Date.
        @return General Ledger Date */
        public DateTime? GetDateAcct()
        {
            return (DateTime?)Get_Value("DateAcct");
        }
        /** Set Document Date.
        @param DateDoc Date of the Document */
        public void SetDateDoc(DateTime? DateDoc)
        {
            if (DateDoc == null) throw new ArgumentException("DateDoc is mandatory.");
            Set_Value("DateDoc", (DateTime?)DateDoc);
        }
        /** Get Document Date.
        @return Date of the Document */
        public DateTime? GetDateDoc()
        {
            return (DateTime?)Get_Value("DateDoc");
        }
        /** Set Description.
        @param Description Optional short description of the record */
        public void SetDescription(String Description)
        {
            if (Description == null) throw new ArgumentException("Description is mandatory.");
            if (Description.Length > 255)
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
            return test.Equals("--") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("IN") || test.Equals("PO") || test.Equals("PR") || test.Equals("RA") || test.Equals("RC") || test.Equals("RE") || test.Equals("RJ") || test.Equals("VO") || test.Equals("WC") || test.Equals("XL");
        }
        /** Set Document Action.
        @param DocAction The targeted status of the document */
        public void SetDocAction(String DocAction)
        {
            if (DocAction == null) throw new ArgumentException("DocAction is mandatory");
            if (!IsDocActionValid(DocAction))
                throw new ArgumentException("DocAction Invalid value - " + DocAction + " - Reference_ID=135 - -- - AP - CL - CO - IN - PO - PR - RA - RC - RE - RJ - VO - WC - XL");
            if (DocAction.Length > 2)
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
            return test.Equals("??") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("DR") || test.Equals("IN") || test.Equals("IP") || test.Equals("NA") || test.Equals("RE") || test.Equals("VO") || test.Equals("WC") || test.Equals("WP");
        }
        /** Set Document Status.
        @param DocStatus The current status of the document */
        public void SetDocStatus(String DocStatus)
        {
            if (DocStatus == null) throw new ArgumentException("DocStatus is mandatory");
            if (!IsDocStatusValid(DocStatus))
                throw new ArgumentException("DocStatus Invalid value - " + DocStatus + " - Reference_ID=131 - ?? - AP - CL - CO - DR - IN - IP - NA - RE - VO - WC - WP");
            if (DocStatus.Length > 2)
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
        /** Set DocumentNo.
        @param DocumentNo Document sequence number of the document */
        public void SetDocumentNo(String DocumentNo)
        {
            if (DocumentNo == null) throw new ArgumentException("DocumentNo is mandatory.");
            if (DocumentNo.Length > 30)
            {
                log.Warning("Length > 30 - truncated");
                DocumentNo = DocumentNo.Substring(0, 30);
            }
            Set_ValueNoCheck("DocumentNo", DocumentNo);
        }
        /** Get DocumentNo.
        @return Document sequence number of the document */
        public String GetDocumentNo()
        {
            return (String)Get_Value("DocumentNo");
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetDocumentNo());
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
        /** Set Budget.
        @param GL_Budget_ID General Ledger Budget */
        public void SetGL_Budget_ID(int GL_Budget_ID)
        {
            if (GL_Budget_ID <= 0) Set_Value("GL_Budget_ID", null);
            else
                Set_Value("GL_Budget_ID", GL_Budget_ID);
        }
        /** Get Budget.
        @return General Ledger Budget */
        public int GetGL_Budget_ID()
        {
            Object ii = Get_Value("GL_Budget_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set GL Category.
        @param GL_Category_ID General Ledger Category */
        public void SetGL_Category_ID(int GL_Category_ID)
        {
            if (GL_Category_ID < 1) throw new ArgumentException("GL_Category_ID is mandatory.");
            Set_Value("GL_Category_ID", GL_Category_ID);
        }
        /** Get GL Category.
        @return General Ledger Category */
        public int GetGL_Category_ID()
        {
            Object ii = Get_Value("GL_Category_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Journal Batch.
        @param GL_JournalBatch_ID General Ledger Journal Batch */
        public void SetGL_JournalBatch_ID(int GL_JournalBatch_ID)
        {
            if (GL_JournalBatch_ID <= 0) Set_ValueNoCheck("GL_JournalBatch_ID", null);
            else
                Set_ValueNoCheck("GL_JournalBatch_ID", GL_JournalBatch_ID);
        }
        /** Get Journal Batch.
        @return General Ledger Journal Batch */
        public int GetGL_JournalBatch_ID()
        {
            Object ii = Get_Value("GL_JournalBatch_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Journal.
        @param GL_Journal_ID General Ledger Journal */
        public void SetGL_Journal_ID(int GL_Journal_ID)
        {
            if (GL_Journal_ID < 1) throw new ArgumentException("GL_Journal_ID is mandatory.");
            Set_ValueNoCheck("GL_Journal_ID", GL_Journal_ID);
        }
        /** Get Journal.
        @return General Ledger Journal */
        public int GetGL_Journal_ID()
        {
            Object ii = Get_Value("GL_Journal_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Approved.
        @param IsApproved Indicates if this document requires approval */
        public void SetIsApproved(Boolean IsApproved)
        {
            Set_ValueNoCheck("IsApproved", IsApproved);
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
        /** Set Printed.
        @param IsPrinted Indicates if this document / line is printed */
        public void SetIsPrinted(Boolean IsPrinted)
        {
            Set_ValueNoCheck("IsPrinted", IsPrinted);
        }
        /** Get Printed.
        @return Indicates if this document / line is printed */
        public Boolean IsPrinted()
        {
            Object oo = Get_Value("IsPrinted");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Posted.
        @param Posted Posting status */
        public void SetPosted(Boolean Posted)
        {
            Set_ValueNoCheck("Posted", Posted);
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

        /** PostingType AD_Reference_ID=125 */
        public static int POSTINGTYPE_AD_Reference_ID = 125;
        /** Actual = A */
        public static String POSTINGTYPE_Actual = "A";
        /** Budget = B */
        public static String POSTINGTYPE_Budget = "B";
        /** Commitment = E */
        public static String POSTINGTYPE_Commitment = "E";
        /** Reservation = R */
        public static String POSTINGTYPE_Reservation = "R";
        /** Statistical = S */
        public static String POSTINGTYPE_Statistical = "S";
        /** Virtual = V */
        public static String POSTINGTYPE_Virtual = "V";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsPostingTypeValid(String test)
        {
            return test.Equals("A") || test.Equals("B") || test.Equals("E") || test.Equals("R") || test.Equals("S") || test.Equals("V");
        }
        /** Set PostingType.
        @param PostingType The type of posted amount for the transaction */
        public void SetPostingType(String PostingType)
        {
            if (PostingType == null) throw new ArgumentException("PostingType is mandatory");
            if (!IsPostingTypeValid(PostingType))
                throw new ArgumentException("PostingType Invalid value - " + PostingType + " - Reference_ID=125 - A - B - E - R - S - V");
            if (PostingType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                PostingType = PostingType.Substring(0, 1);
            }
            Set_Value("PostingType", PostingType);
        }
        /** Get PostingType.
        @return The type of posted amount for the transaction */
        public String GetPostingType()
        {
            return (String)Get_Value("PostingType");
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
        /** Set Total Credit.
        @param TotalCr Total Credit in document currency */
        public void SetTotalCr(Decimal? TotalCr)
        {
            if (TotalCr == null) throw new ArgumentException("TotalCr is mandatory.");
            Set_ValueNoCheck("TotalCr", (Decimal?)TotalCr);
        }
        /** Get Total Credit.
        @return Total Credit in document currency */
        public Decimal GetTotalCr()
        {
            Object bd = Get_Value("TotalCr");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Total Debit.
        @param TotalDr Total debit in document currency */
        public void SetTotalDr(Decimal? TotalDr)
        {
            if (TotalDr == null) throw new ArgumentException("TotalDr is mandatory.");
            Set_ValueNoCheck("TotalDr", (Decimal?)TotalDr);
        }
        /** Get Total Debit.
        @return Total debit in document currency */
        public Decimal GetTotalDr()
        {
            Object bd = Get_Value("TotalDr");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
    }

}
