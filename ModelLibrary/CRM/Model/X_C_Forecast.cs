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
    using System.Data;/** Generated Model for C_Forecast
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_Forecast : PO
    {
        public X_C_Forecast(Context ctx, int C_Forecast_ID, Trx trxName) : base(ctx, C_Forecast_ID, trxName)
        {/** if (C_Forecast_ID == 0){SetC_Forecast_ID (0);SetC_Period_ID (0);SetName (null);} */
        }
        public X_C_Forecast(Ctx ctx, int C_Forecast_ID, Trx trxName) : base(ctx, C_Forecast_ID, trxName)
        {/** if (C_Forecast_ID == 0){SetC_Forecast_ID (0);SetC_Period_ID (0);SetName (null);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_Forecast(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_Forecast(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_Forecast(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_C_Forecast() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27900682686290L;/** Last Updated Timestamp 4/16/2021 7:16:09 AM */
        public static long updatedMS = 1618557369501L;/** AD_Table_ID=1000244 */
        public static int Table_ID; // =1000244;
        /** TableName=C_Forecast */
        public static String Table_Name = "C_Forecast";
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_C_Forecast[").Append(Get_ID()).Append("]"); return sb.ToString(); }
        /** AD_User_ID AD_Reference_ID=110 */
        public static int AD_USER_ID_AD_Reference_ID = 110;/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Customer/Prospect Contact. */
        public void SetAD_User_ID(int AD_User_ID)
        {
            if (AD_User_ID <= 0) Set_Value("AD_User_ID", null);
            else
                Set_Value("AD_User_ID", AD_User_ID);
        }/** Get User/Contact.
@return User within the system - Internal or Customer/Prospect Contact. */
        public int GetAD_User_ID() { Object ii = Get_Value("AD_User_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Currency.
@param C_Currency_ID The Currency for this record */
        public void SetC_Currency_ID(int C_Currency_ID)
        {
            if (C_Currency_ID <= 0) Set_Value("C_Currency_ID", null);
            else
                Set_Value("C_Currency_ID", C_Currency_ID);
        }/** Get Currency.
@return The Currency for this record */
        public int GetC_Currency_ID() { Object ii = Get_Value("C_Currency_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Document Type.
@param C_DocType_ID Document type or rules */
        public void SetC_DocType_ID(int C_DocType_ID)
        {
            if (C_DocType_ID <= 0) Set_Value("C_DocType_ID", null);
            else
                Set_Value("C_DocType_ID", C_DocType_ID);
        }/** Get Document Type.
@return Document type or rules */
        public int GetC_DocType_ID() { Object ii = Get_Value("C_DocType_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Forecast.
@param C_Forecast_ID Forecast */
        public void SetC_Forecast_ID(int C_Forecast_ID) { if (C_Forecast_ID < 1) throw new ArgumentException("C_Forecast_ID is mandatory."); Set_ValueNoCheck("C_Forecast_ID", C_Forecast_ID); }/** Get Forecast.
@return Forecast */
        public int GetC_Forecast_ID() { Object ii = Get_Value("C_Forecast_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        /** C_Period_ID AD_Reference_ID=1000169 */
        public static int C_PERIOD_ID_AD_Reference_ID = 1000169;/** Set Period.
@param C_Period_ID Period of the Calendar */
        public void SetC_Period_ID(int C_Period_ID) { if (C_Period_ID < 1) throw new ArgumentException("C_Period_ID is mandatory."); Set_Value("C_Period_ID", C_Period_ID); }/** Get Period.
@return Period of the Calendar */
        public int GetC_Period_ID() { Object ii = Get_Value("C_Period_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Sales Region.
@param C_SalesRegion_ID Sales coverage region */
        public void SetC_SalesRegion_ID(int C_SalesRegion_ID)
        {
            if (C_SalesRegion_ID <= 0) Set_Value("C_SalesRegion_ID", null);
            else
                Set_Value("C_SalesRegion_ID", C_SalesRegion_ID);
        }/** Get Sales Region.
@return Sales coverage region */
        public int GetC_SalesRegion_ID() { Object ii = Get_Value("C_SalesRegion_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Team.
@param C_Team_ID Team */
        public void SetC_Team_ID(int C_Team_ID)
        {
            if (C_Team_ID <= 0) Set_Value("C_Team_ID", null);
            else
                Set_Value("C_Team_ID", C_Team_ID);
        }/** Get Team.
@return Team */
        public int GetC_Team_ID() { Object ii = Get_Value("C_Team_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Comments.
@param Comments Comments or additional information */
        public void SetComments(String Comments) { if (Comments != null && Comments.Length > 255) { log.Warning("Length > 255 - truncated"); Comments = Comments.Substring(0, 255); } Set_Value("Comments", Comments); }/** Get Comments.
@return Comments or additional information */
        public String GetComments() { return (String)Get_Value("Comments"); }/** Set Copy Lines.
@param CopyLines Copy Lines */
        public void SetCopyLines(String CopyLines) { if (CopyLines != null && CopyLines.Length > 10) { log.Warning("Length > 10 - truncated"); CopyLines = CopyLines.Substring(0, 10); } Set_Value("CopyLines", CopyLines); }/** Get Copy Lines.
@return Copy Lines */
        public String GetCopyLines() { return (String)Get_Value("CopyLines"); }/** Set Account Date.
@param DateAcct General Ledger Date */
        public void SetDateAcct(DateTime? DateAcct) { Set_Value("DateAcct", (DateTime?)DateAcct); }/** Get Account Date.
@return General Ledger Date */
        public DateTime? GetDateAcct() { return (DateTime?)Get_Value("DateAcct"); }/** Set Description.
@param Description Optional short description of the record */
        public void SetDescription(String Description) { if (Description != null && Description.Length > 255) { log.Warning("Length > 255 - truncated"); Description = Description.Substring(0, 255); } Set_Value("Description", Description); }/** Get Description.
@return Optional short description of the record */
        public String GetDescription() { return (String)Get_Value("Description"); }
        /** DocAction AD_Reference_ID=135 */
        public static int DOCACTION_AD_Reference_ID = 135;/** <None> = -- */
        public static String DOCACTION_None = "--";/** Approve = AP */
        public static String DOCACTION_Approve = "AP";/** Close = CL */
        public static String DOCACTION_Close = "CL";/** Complete = CO */
        public static String DOCACTION_Complete = "CO";/** Invalidate = IN */
        public static String DOCACTION_Invalidate = "IN";/** Post = PO */
        public static String DOCACTION_Post = "PO";/** Prepare = PR */
        public static String DOCACTION_Prepare = "PR";/** Reverse - Accrual = RA */
        public static String DOCACTION_Reverse_Accrual = "RA";/** Reverse - Correct = RC */
        public static String DOCACTION_Reverse_Correct = "RC";/** Re-activate = RE */
        public static String DOCACTION_Re_Activate = "RE";/** Reject = RJ */
        public static String DOCACTION_Reject = "RJ";/** Void = VO */
        public static String DOCACTION_Void = "VO";/** Wait Complete = WC */
        public static String DOCACTION_WaitComplete = "WC";/** Unlock = XL */
        public static String DOCACTION_Unlock = "XL";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsDocActionValid(String test) { return test == null || test.Equals("--") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("IN") || test.Equals("PO") || test.Equals("PR") || test.Equals("RA") || test.Equals("RC") || test.Equals("RE") || test.Equals("RJ") || test.Equals("VO") || test.Equals("WC") || test.Equals("XL"); }/** Set Document Action.
@param DocAction The targeted status of the document */
        public void SetDocAction(String DocAction)
        {
            if (!IsDocActionValid(DocAction))
                throw new ArgumentException("DocAction Invalid value - " + DocAction + " - Reference_ID=135 - -- - AP - CL - CO - IN - PO - PR - RA - RC - RE - RJ - VO - WC - XL"); if (DocAction != null && DocAction.Length > 2) { log.Warning("Length > 2 - truncated"); DocAction = DocAction.Substring(0, 2); }
            Set_Value("DocAction", DocAction);
        }/** Get Document Action.
@return The targeted status of the document */
        public String GetDocAction() { return (String)Get_Value("DocAction"); }
        /** DocStatus AD_Reference_ID=131 */
        public static int DOCSTATUS_AD_Reference_ID = 131;/** Unknown = ?? */
        public static String DOCSTATUS_Unknown = "??";/** Approved = AP */
        public static String DOCSTATUS_Approved = "AP";/** Closed = CL */
        public static String DOCSTATUS_Closed = "CL";/** Completed = CO */
        public static String DOCSTATUS_Completed = "CO";/** Drafted = DR */
        public static String DOCSTATUS_Drafted = "DR";/** Invalid = IN */
        public static String DOCSTATUS_Invalid = "IN";/** In Progress = IP */
        public static String DOCSTATUS_InProgress = "IP";/** Not Approved = NA */
        public static String DOCSTATUS_NotApproved = "NA";/** Reversed = RE */
        public static String DOCSTATUS_Reversed = "RE";/** Voided = VO */
        public static String DOCSTATUS_Voided = "VO";/** Waiting Confirmation = WC */
        public static String DOCSTATUS_WaitingConfirmation = "WC";/** Waiting Payment = WP */
        public static String DOCSTATUS_WaitingPayment = "WP";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsDocStatusValid(String test) { return test == null || test.Equals("??") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("DR") || test.Equals("IN") || test.Equals("IP") || test.Equals("NA") || test.Equals("RE") || test.Equals("VO") || test.Equals("WC") || test.Equals("WP"); }/** Set Document Status.
@param DocStatus The current status of the document */
        public void SetDocStatus(String DocStatus)
        {
            if (!IsDocStatusValid(DocStatus))
                throw new ArgumentException("DocStatus Invalid value - " + DocStatus + " - Reference_ID=131 - ?? - AP - CL - CO - DR - IN - IP - NA - RE - VO - WC - WP"); if (DocStatus != null && DocStatus.Length > 2) { log.Warning("Length > 2 - truncated"); DocStatus = DocStatus.Substring(0, 2); }
            Set_Value("DocStatus", DocStatus);
        }/** Get Document Status.
@return The current status of the document */
        public String GetDocStatus() { return (String)Get_Value("DocStatus"); }/** Set Document No..
@param DocumentNo Document sequence number of the document */
        public void SetDocumentNo(String DocumentNo) { if (DocumentNo != null && DocumentNo.Length > 30) { log.Warning("Length > 30 - truncated"); DocumentNo = DocumentNo.Substring(0, 30); } Set_Value("DocumentNo", DocumentNo); }/** Get Document No..
@return Document sequence number of the document */
        public String GetDocumentNo() { return (String)Get_Value("DocumentNo"); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_ValueNoCheck("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Create Component Txn Lines.
@param GenerateLines Generate component lines for Push supply type components  */
        public void SetGenerateLines(String GenerateLines) { if (GenerateLines != null && GenerateLines.Length > 10) { log.Warning("Length > 10 - truncated"); GenerateLines = GenerateLines.Substring(0, 10); } Set_Value("GenerateLines", GenerateLines); }/** Get Create Component Txn Lines.
@return Generate component lines for Push supply type components  */
        public String GetGenerateLines() { return (String)Get_Value("GenerateLines"); }/** Set Approved.
@param IsApproved Indicates if this document requires approval */
        public void SetIsApproved(Boolean IsApproved) { Set_Value("IsApproved", IsApproved); }/** Get Approved.
@return Indicates if this document requires approval */
        public Boolean IsApproved() { Object oo = Get_Value("IsApproved"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Price List.
@param M_PriceList_ID Unique identifier of a Price List */
        public void SetM_PriceList_ID(int M_PriceList_ID)
        {
            if (M_PriceList_ID <= 0) Set_Value("M_PriceList_ID", null);
            else
                Set_Value("M_PriceList_ID", M_PriceList_ID);
        }/** Get Price List.
@return Unique identifier of a Price List */
        public int GetM_PriceList_ID() { Object ii = Get_Value("M_PriceList_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Name.
@param Name Alphanumeric identifier of the entity */
        public void SetName(String Name) { if (Name == null) throw new ArgumentException("Name is mandatory."); if (Name.Length > 50) { log.Warning("Length > 50 - truncated"); Name = Name.Substring(0, 50); } Set_Value("Name", Name); }/** Get Name.
@return Alphanumeric identifier of the entity */
        public String GetName() { return (String)Get_Value("Name"); }/** Set Posted.
@param Posted Posting status */
        public void SetPosted(Boolean Posted) { Set_Value("Posted", Posted); }/** Get Posted.
@return Posting status */
        public Boolean IsPosted() { Object oo = Get_Value("Posted"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Processed.
@param Processed The document has been processed */
        public void SetProcessed(Boolean Processed) { Set_Value("Processed", Processed); }/** Get Processed.
@return The document has been processed */
        public Boolean IsProcessed() { Object oo = Get_Value("Processed"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Process Now.
@param Processing Process Now */
        public void SetProcessing(Boolean Processing) { Set_Value("Processing", Processing); }/** Get Process Now.
@return Process Now */
        public Boolean IsProcessing() { Object oo = Get_Value("Processing"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }
        /** Supervisor_ID AD_Reference_ID=316 */
        public static int SUPERVISOR_ID_AD_Reference_ID = 316;/** Set Supervisor.
@param Supervisor_ID Supervisor for this user/organization - used for escalation and approval */
        public void SetSupervisor_ID(int Supervisor_ID)
        {
            if (Supervisor_ID <= 0) Set_Value("Supervisor_ID", null);
            else
                Set_Value("Supervisor_ID", Supervisor_ID);
        }/** Get Supervisor.
@return Supervisor for this user/organization - used for escalation and approval */
        public int GetSupervisor_ID() { Object ii = Get_Value("Supervisor_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set TRXDATE.
@param TRXDATE TRXDATE */
        public void SetTRXDATE(DateTime? TRXDATE) { Set_Value("TRXDATE", (DateTime?)TRXDATE); }/** Get TRXDATE.
@return TRXDATE */
        public DateTime? GetTRXDATE() { return (DateTime?)Get_Value("TRXDATE"); }/** Set Temp Document No.
@param TempDocumentNo Temp Document No for this Document */
        public void SetTempDocumentNo(String TempDocumentNo) { if (TempDocumentNo != null && TempDocumentNo.Length > 30) { log.Warning("Length > 30 - truncated"); TempDocumentNo = TempDocumentNo.Substring(0, 30); } Set_Value("TempDocumentNo", TempDocumentNo); }/** Get Temp Document No.
@return Temp Document No for this Document */
        public String GetTempDocumentNo() { return (String)Get_Value("TempDocumentNo"); }
    }
}