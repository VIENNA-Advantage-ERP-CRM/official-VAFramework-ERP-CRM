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
    using System.Data;/** Generated Model for C_Recurring
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_Recurring : PO
    {
        public X_C_Recurring(Context ctx, int C_Recurring_ID, Trx trxName)
            : base(ctx, C_Recurring_ID, trxName)
        {/** if (C_Recurring_ID == 0){SetC_Recurring_ID (0);SetFrequencyType (null);// M
SetName (null);SetRecurringType (null);SetRunsMax (0);SetRunsRemaining (0);} */
        }
        public X_C_Recurring(Ctx ctx, int C_Recurring_ID, Trx trxName)
            : base(ctx, C_Recurring_ID, trxName)
        {/** if (C_Recurring_ID == 0){SetC_Recurring_ID (0);SetFrequencyType (null);// M
SetName (null);SetRecurringType (null);SetRunsMax (0);SetRunsRemaining (0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_Recurring(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_Recurring(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_Recurring(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_C_Recurring() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27763848376125L;/** Last Updated Timestamp 12/14/2016 7:14:19 PM */
        public static long updatedMS = 1481723059336L;/** AD_Table_ID=574 */
        public static int Table_ID; // =574;
        /** TableName=C_Recurring */
        public static String Table_Name = "C_Recurring";
        protected static KeyNamePair model; protected Decimal accessLevel = new Decimal(3);/** AccessLevel
@return 3 - Client - Org 
*/
        protected override int Get_AccessLevel() { return Convert.ToInt32(accessLevel.ToString()); }/** Load Meta Data
@param ctx context
@return PO Info
*/
        protected override POInfo InitPO(Context ctx) { POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi; }/** Load Meta Data
@param ctx context
@return PO Info
*/
        protected override POInfo InitPO(Ctx ctx) { POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi; }/** Info
@return info
*/
        public override String ToString() { StringBuilder sb = new StringBuilder("X_C_Recurring[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Invoice.
@param C_Invoice_ID Invoice Identifier */
        public void SetC_Invoice_ID(int C_Invoice_ID)
        {
            if (C_Invoice_ID <= 0) Set_Value("C_Invoice_ID", null);
            else
                Set_Value("C_Invoice_ID", C_Invoice_ID);
        }/** Get Invoice.
@return Invoice Identifier */
        public int GetC_Invoice_ID() { Object ii = Get_Value("C_Invoice_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Order.
@param C_Order_ID Sales Order */
        public void SetC_Order_ID(int C_Order_ID)
        {
            if (C_Order_ID <= 0) Set_Value("C_Order_ID", null);
            else
                Set_Value("C_Order_ID", C_Order_ID);
        }/** Get Order.
@return Sales Order */
        public int GetC_Order_ID() { Object ii = Get_Value("C_Order_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Payment.
@param C_Payment_ID Payment identifier */
        public void SetC_Payment_ID(int C_Payment_ID)
        {
            if (C_Payment_ID <= 0) Set_Value("C_Payment_ID", null);
            else
                Set_Value("C_Payment_ID", C_Payment_ID);
        }/** Get Payment.
@return Payment identifier */
        public int GetC_Payment_ID() { Object ii = Get_Value("C_Payment_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Opportunity.
@param C_Project_ID Business Opportunity */
        public void SetC_Project_ID(int C_Project_ID)
        {
            if (C_Project_ID <= 0) Set_Value("C_Project_ID", null);
            else
                Set_Value("C_Project_ID", C_Project_ID);
        }/** Get Opportunity.
@return Business Opportunity */
        public int GetC_Project_ID() { Object ii = Get_Value("C_Project_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Recurring.
@param C_Recurring_ID Recurring Document */
        public void SetC_Recurring_ID(int C_Recurring_ID) { if (C_Recurring_ID < 1) throw new ArgumentException("C_Recurring_ID is mandatory."); Set_ValueNoCheck("C_Recurring_ID", C_Recurring_ID); }/** Get Recurring.
@return Recurring Document */
        public int GetC_Recurring_ID() { Object ii = Get_Value("C_Recurring_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Date Last Run.
@param DateLastRun Date the process was last run. */
        public void SetDateLastRun(DateTime? DateLastRun) { Set_ValueNoCheck("DateLastRun", (DateTime?)DateLastRun); }/** Get Date Last Run.
@return Date the process was last run. */
        public DateTime? GetDateLastRun() { return (DateTime?)Get_Value("DateLastRun"); }/** Set Date next run.
@param DateNextRun Date the process will run next */
        public void SetDateNextRun(DateTime? DateNextRun) { Set_Value("DateNextRun", (DateTime?)DateNextRun); }/** Get Date next run.
@return Date the process will run next */
        public DateTime? GetDateNextRun() { return (DateTime?)Get_Value("DateNextRun"); }/** Set Description.
@param Description Optional short description of the record */
        public void SetDescription(String Description) { if (Description != null && Description.Length > 255) { log.Warning("Length > 255 - truncated"); Description = Description.Substring(0, 255); } Set_Value("Description", Description); }/** Get Description.
@return Optional short description of the record */
        public String GetDescription() { return (String)Get_Value("Description"); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_ValueNoCheck("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Frequency.
@param Frequency Frequency of events */
        public void SetFrequency(int Frequency) { Set_Value("Frequency", Frequency); }/** Get Frequency.
@return Frequency of events */
        public int GetFrequency() { Object ii = Get_Value("Frequency"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        /** FrequencyType AD_Reference_ID=283 */
        public static int FREQUENCYTYPE_AD_Reference_ID = 283;/** Daily = D */
        public static String FREQUENCYTYPE_Daily = "D";/** Monthly = M */
        public static String FREQUENCYTYPE_Monthly = "M";/** Quarterly = Q */
        public static String FREQUENCYTYPE_Quarterly = "Q";/** Weekly = W */
        public static String FREQUENCYTYPE_Weekly = "W";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsFrequencyTypeValid(String test) { return test.Equals("D") || test.Equals("M") || test.Equals("Q") || test.Equals("W"); }/** Set Frequency Type.
@param FrequencyType Frequency of event */
        public void SetFrequencyType(String FrequencyType)
        {
            if (FrequencyType == null) throw new ArgumentException("FrequencyType is mandatory"); if (!IsFrequencyTypeValid(FrequencyType))
                throw new ArgumentException("FrequencyType Invalid value - " + FrequencyType + " - Reference_ID=283 - D - M - Q - W"); if (FrequencyType.Length > 1) { log.Warning("Length > 1 - truncated"); FrequencyType = FrequencyType.Substring(0, 1); } Set_Value("FrequencyType", FrequencyType);
        }/** Get Frequency Type.
@return Frequency of event */
        public String GetFrequencyType() { return (String)Get_Value("FrequencyType"); }/** Set Journal Batch.
@param GL_JournalBatch_ID General Ledger Journal Batch */
        public void SetGL_JournalBatch_ID(int GL_JournalBatch_ID)
        {
            if (GL_JournalBatch_ID <= 0) Set_Value("GL_JournalBatch_ID", null);
            else
                Set_Value("GL_JournalBatch_ID", GL_JournalBatch_ID);
        }/** Get Journal Batch.
@return General Ledger Journal Batch */
        public int GetGL_JournalBatch_ID() { Object ii = Get_Value("GL_JournalBatch_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Journal.
@param GL_Journal_ID General Ledger Journal */
        public void SetGL_Journal_ID(int GL_Journal_ID)
        {
            if (GL_Journal_ID <= 0) Set_Value("GL_Journal_ID", null);
            else
                Set_Value("GL_Journal_ID", GL_Journal_ID);
        }/** Get Journal.
@return General Ledger Journal */
        public int GetGL_Journal_ID() { Object ii = Get_Value("GL_Journal_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Comment.
@param Help Comment, Help or Hint */
        public void SetHelp(String Help) { if (Help != null && Help.Length > 2000) { log.Warning("Length > 2000 - truncated"); Help = Help.Substring(0, 2000); } Set_Value("Help", Help); }/** Get Comment.
@return Comment, Help or Hint */
        public String GetHelp() { return (String)Get_Value("Help"); }/** Set Name.
@param Name Alphanumeric identifier of the entity */
        public void SetName(String Name) { if (Name == null) throw new ArgumentException("Name is mandatory."); if (Name.Length > 60) { log.Warning("Length > 60 - truncated"); Name = Name.Substring(0, 60); } Set_Value("Name", Name); }/** Get Name.
@return Alphanumeric identifier of the entity */
        public String GetName() { return (String)Get_Value("Name"); }/** Get Record ID/ColumnName
@return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair() { return new KeyNamePair(Get_ID(), GetName()); }/** Set Process Now.
@param Processing Process Now */
        public void SetProcessing(Boolean Processing) { Set_Value("Processing", Processing); }/** Get Process Now.
@return Process Now */
        public Boolean IsProcessing() { Object oo = Get_Value("Processing"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }
        /** RecurringType AD_Reference_ID=282 */
        public static int RECURRINGTYPE_AD_Reference_ID = 282;/** GL Journal = B */
        public static String RECURRINGTYPE_GLJournal = "B";/** GL Journal Batch = G */
        public static String RECURRINGTYPE_GLJournalBatch = "G";/** Invoice = I */
        public static String RECURRINGTYPE_Invoice = "I";/** Project = J */
        public static String RECURRINGTYPE_Project = "J";/** Order = O */
        public static String RECURRINGTYPE_Order = "O";/** Payment = P */
        public static String RECURRINGTYPE_Payment = "P";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsRecurringTypeValid(String test) { return test.Equals("B") || test.Equals("G") || test.Equals("I") || test.Equals("J") || test.Equals("O") || test.Equals("P"); }/** Set Recurring Type.
@param RecurringType Type of Recurring Document */
        public void SetRecurringType(String RecurringType)
        {
            if (RecurringType == null) throw new ArgumentException("RecurringType is mandatory"); if (!IsRecurringTypeValid(RecurringType))
                throw new ArgumentException("RecurringType Invalid value - " + RecurringType + " - Reference_ID=282 - B - G - I - J - O - P"); if (RecurringType.Length > 1) { log.Warning("Length > 1 - truncated"); RecurringType = RecurringType.Substring(0, 1); } Set_Value("RecurringType", RecurringType);
        }/** Get Recurring Type.
@return Type of Recurring Document */
        public String GetRecurringType() { return (String)Get_Value("RecurringType"); }/** Set Maximum Runs.
@param RunsMax Number of recurring runs */
        public void SetRunsMax(int RunsMax) { Set_Value("RunsMax", RunsMax); }/** Get Maximum Runs.
@return Number of recurring runs */
        public int GetRunsMax() { Object ii = Get_Value("RunsMax"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Remaining Runs.
@param RunsRemaining Number of recurring runs remaining */
        public void SetRunsRemaining(int RunsRemaining) { Set_ValueNoCheck("RunsRemaining", RunsRemaining); }/** Get Remaining Runs.
@return Number of recurring runs remaining */
        public int GetRunsRemaining() { Object ii = Get_Value("RunsRemaining"); if (ii == null) return 0; return Convert.ToInt32(ii); }
    }
}