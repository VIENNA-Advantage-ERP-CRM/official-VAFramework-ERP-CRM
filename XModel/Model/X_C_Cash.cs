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
    /** Generated Model for C_Cash
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_Cash : PO
    {
        public X_C_Cash(Context ctx, int C_Cash_ID, Trx trxName)
            : base(ctx, C_Cash_ID, trxName)
        {
            /** if (C_Cash_ID == 0)
            {
            SetBeginningBalance (0.0); 
            SetC_CashBook_ID (0);
            SetC_Cash_ID (0);
            SetDateAcct (DateTime.Now);	// @#Date@
            SetDocAction (null);	// CO
            SetDocStatus (null);	// DR
            SetEndingBalance (0.0);
            SetIsApproved (false);
            SetName (null);	// @#Date@
            SetPosted (false);	// N
            SetProcessed (false);	// N
            SetStatementDate (DateTime.Now);	// @#Date@
            }
             */
        }
        public X_C_Cash(Ctx ctx, int C_Cash_ID, Trx trxName)
            : base(ctx, C_Cash_ID, trxName)
        {
            /** if (C_Cash_ID == 0)
            {
            SetBeginningBalance (0.0);
            SetC_CashBook_ID (0);
            SetC_Cash_ID (0);
            SetDateAcct (DateTime.Now);	// @#Date@
            SetDocAction (null);	// CO
            SetDocStatus (null);	// DR
            SetEndingBalance (0.0);
            SetIsApproved (false);
            SetName (null);	// @#Date@
            SetPosted (false);	// N
            SetProcessed (false);	// N
            SetStatementDate (DateTime.Now);	// @#Date@
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_Cash(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_Cash(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_Cash(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_C_Cash()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27682432707807L;
        /** Last Updated Timestamp 5/17/2014 11:46:31 AM */
        public static long updatedMS = 1400307391018L;
        /** AD_Table_ID=407 */
        public static int Table_ID;
        // =407;

        /** TableName=C_Cash */
        public static String Table_Name = "C_Cash";

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
            StringBuilder sb = new StringBuilder("X_C_Cash[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }

        /** AD_OrgTrx_ID AD_Reference_ID=130 */
        public static int AD_ORGTRX_ID_AD_Reference_ID = 130;
        /** Set Trx Organization.
        @param AD_OrgTrx_ID Performing or initiating organization */
        public void SetAD_OrgTrx_ID(int AD_OrgTrx_ID)
        {
            if (AD_OrgTrx_ID <= 0) Set_Value("AD_OrgTrx_ID", null);
            else
                Set_Value("AD_OrgTrx_ID", AD_OrgTrx_ID);
        }
        /** Get Trx Organization.
        @return Performing or initiating organization */
        public int GetAD_OrgTrx_ID()
        {
            Object ii = Get_Value("AD_OrgTrx_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Beginning Balance.
        @param BeginningBalance Balance prior to any transactions */
        public void SetBeginningBalance(Decimal? BeginningBalance)
        {
            if (BeginningBalance == null) throw new ArgumentException("BeginningBalance is mandatory.");
            Set_Value("BeginningBalance", (Decimal?)BeginningBalance);
        }
        /** Get Beginning Balance.
        @return Balance prior to any transactions */
        public Decimal GetBeginningBalance()
        {
            Object bd = Get_Value("BeginningBalance");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Activity.
        @param C_Activity_ID Business Activity */
        public void SetC_Activity_ID(int C_Activity_ID)
        {
            if (C_Activity_ID <= 0) Set_Value("C_Activity_ID", null);
            else
                Set_Value("C_Activity_ID", C_Activity_ID);
        }
        /** Get Activity.
        @return Business Activity */
        public int GetC_Activity_ID()
        {
            Object ii = Get_Value("C_Activity_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Campaign.
        @param C_Campaign_ID Marketing Campaign */
        public void SetC_Campaign_ID(int C_Campaign_ID)
        {
            if (C_Campaign_ID <= 0) Set_Value("C_Campaign_ID", null);
            else
                Set_Value("C_Campaign_ID", C_Campaign_ID);
        }
        /** Get Campaign.
        @return Marketing Campaign */
        public int GetC_Campaign_ID()
        {
            Object ii = Get_Value("C_Campaign_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Cash Book.
        @param C_CashBook_ID Cash Book for recording petty cash transactions */
        public void SetC_CashBook_ID(int C_CashBook_ID)
        {
            if (C_CashBook_ID < 1) throw new ArgumentException("C_CashBook_ID is mandatory.");
            Set_ValueNoCheck("C_CashBook_ID", C_CashBook_ID);
        }
        /** Get Cash Book.
        @return Cash Book for recording petty cash transactions */
        public int GetC_CashBook_ID()
        {
            Object ii = Get_Value("C_CashBook_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Cash Journal.
        @param C_Cash_ID Cash Journal */
        public void SetC_Cash_ID(int C_Cash_ID)
        {
            if (C_Cash_ID < 1) throw new ArgumentException("C_Cash_ID is mandatory.");
            Set_ValueNoCheck("C_Cash_ID", C_Cash_ID);
        }
        /** Get Cash Journal.
        @return Cash Journal */
        public int GetC_Cash_ID()
        {
            Object ii = Get_Value("C_Cash_ID");
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
        /** Set Opportunity.
        @param C_Project_ID Business Opportunity */
        public void SetC_Project_ID(int C_Project_ID)
        {
            if (C_Project_ID <= 0) Set_Value("C_Project_ID", null);
            else
                Set_Value("C_Project_ID", C_Project_ID);
        }
        /** Get Opportunity.
        @return Business Opportunity */
        public int GetC_Project_ID()
        {
            Object ii = Get_Value("C_Project_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
        /** Set Description.
        @param Description Optional short description of the record */
        public void SetDescription(String Description)
        {
            if (Description != null && Description.Length > 500)
            {
                log.Warning("Length > 500 - truncated");
                Description = Description.Substring(0, 500);
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
            if (DocumentNo != null && DocumentNo.Length > 10)
            {
                log.Warning("Length > 10 - truncated");
                DocumentNo = DocumentNo.Substring(0, 10);
            }
            Set_Value("DocumentNo", DocumentNo);
        }
        /** Get DocumentNo.
        @return Document sequence number of the document */
        public String GetDocumentNo()
        {
            return (String)Get_Value("DocumentNo");
        }
        /** Set Ending balance.
        @param EndingBalance Ending  or closing balance */
        public void SetEndingBalance(Decimal? EndingBalance)
        {
            if (EndingBalance == null) throw new ArgumentException("EndingBalance is mandatory.");
            Set_Value("EndingBalance", (Decimal?)EndingBalance);
        }
        /** Get Ending balance.
        @return Ending  or closing balance */
        public Decimal GetEndingBalance()
        {
            Object bd = Get_Value("EndingBalance");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
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
        /** Set Generate CashBook Transfer.
        @param GenerateCashBookTransfer Generate CashBook Transfer */
        public void SetGenerateCashBookTransfer(String GenerateCashBookTransfer)
        {
            if (GenerateCashBookTransfer != null && GenerateCashBookTransfer.Length > 10)
            {
                log.Warning("Length > 10 - truncated");
                GenerateCashBookTransfer = GenerateCashBookTransfer.Substring(0, 10);
            }
            Set_Value("GenerateCashBookTransfer", GenerateCashBookTransfer);
        }
        /** Get Generate CashBook Transfer.
        @return Generate CashBook Transfer */
        public String GetGenerateCashBookTransfer()
        {
            return (String)Get_Value("GenerateCashBookTransfer");
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
            if (Name == null) throw new ArgumentException("Name is mandatory.");
            if (Name.Length > 500)
            {
                log.Warning("Length > 500 - truncated");
                Name = Name.Substring(0, 500);
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
        /** Set Statement date.
        @param StatementDate Date of the statement */
        public void SetStatementDate(DateTime? StatementDate)
        {
            if (StatementDate == null) throw new ArgumentException("StatementDate is mandatory.");
            Set_Value("StatementDate", (DateTime?)StatementDate);
        }
        /** Get Statement date.
        @return Date of the statement */
        public DateTime? GetStatementDate()
        {
            return (DateTime?)Get_Value("StatementDate");
        }
        /** Set Statement difference.
        @param StatementDifference Difference between statement ending balance and actual ending balance */
        public void SetStatementDifference(Decimal? StatementDifference)
        {
            Set_Value("StatementDifference", (Decimal?)StatementDifference);
        }
        /** Get Statement difference.
        @return Difference between statement ending balance and actual ending balance */
        public Decimal GetStatementDifference()
        {
            Object bd = Get_Value("StatementDifference");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** User1_ID AD_Reference_ID=134 */
        public static int USER1_ID_AD_Reference_ID = 134;
        /** Set User List 1.
        @param User1_ID User defined list element #1 */
        public void SetUser1_ID(int User1_ID)
        {
            if (User1_ID <= 0) Set_Value("User1_ID", null);
            else
                Set_Value("User1_ID", User1_ID);
        }
        /** Get User List 1.
        @return User defined list element #1 */
        public int GetUser1_ID()
        {
            Object ii = Get_Value("User1_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** User2_ID AD_Reference_ID=137 */
        public static int USER2_ID_AD_Reference_ID = 137;
        /** Set User List 2.
        @param User2_ID User defined list element #2 */
        public void SetUser2_ID(int User2_ID)
        {
            if (User2_ID <= 0) Set_Value("User2_ID", null);
            else
                Set_Value("User2_ID", User2_ID);
        }
        /** Get User List 2.
        @return User defined list element #2 */
        public int GetUser2_ID()
        {
            Object ii = Get_Value("User2_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** @param VAPOS_ShiftDetails_ID Shift Details */
        public void SetVAPOS_ShiftDetails_ID(int VAPOS_ShiftDetails_ID)
        {
            if (VAPOS_ShiftDetails_ID <= 0) Set_Value("VAPOS_ShiftDetails_ID", null);
            else
                Set_Value("VAPOS_ShiftDetails_ID", VAPOS_ShiftDetails_ID);
        }
        /** Get Shift Details.
        @return Shift Details */
        public int GetVAPOS_ShiftDetails_ID()
        {
            Object ii = Get_Value("VAPOS_ShiftDetails_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Shift Date.
        @param VAPOS_ShiftDate Shift Date */
        public void SetVAPOS_ShiftDate(DateTime? VAPOS_ShiftDate)
        {
            Set_Value("VAPOS_ShiftDate", (DateTime?)VAPOS_ShiftDate);
        }
        /** Get Shift Date. @return Shift Date */
        public DateTime? GetVAPOS_ShiftDate()
        {
            return (DateTime?)Get_Value("VAPOS_ShiftDate");
        }

        /** Set Currency.
        @param C_Currency_ID The Currency for this record */
        public void SetC_Currency_ID(int C_Currency_ID)
        {
            if (C_Currency_ID <= 0) Set_Value("C_Currency_ID", null);
            else
                Set_Value("C_Currency_ID", C_Currency_ID);
        }/** Get Currency.
@return The Currency for this record */
        public int GetC_Currency_ID() { Object ii = Get_Value("C_Currency_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
    }

}
