namespace VAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for C_AllocationHdr
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_AllocationHdr : PO{public X_C_AllocationHdr (Context ctx, int C_AllocationHdr_ID, Trx trxName) : base (ctx, C_AllocationHdr_ID, trxName){/** if (C_AllocationHdr_ID == 0){SetApprovalAmt (0.0);SetC_AllocationHdr_ID (0);SetC_Currency_ID (0);SetDateAcct (DateTime.Now);SetDateTrx (DateTime.Now);SetDocAction (null);// CO
SetDocStatus (null);// DR
SetDocumentNo (null);SetIsApproved (false);SetIsManual (false);SetPosted (false);SetProcessed (false);// N
} */
}public X_C_AllocationHdr (Ctx ctx, int C_AllocationHdr_ID, Trx trxName) : base (ctx, C_AllocationHdr_ID, trxName){/** if (C_AllocationHdr_ID == 0){SetApprovalAmt (0.0);SetC_AllocationHdr_ID (0);SetC_Currency_ID (0);SetDateAcct (DateTime.Now);SetDateTrx (DateTime.Now);SetDocAction (null);// CO
SetDocStatus (null);// DR
SetDocumentNo (null);SetIsApproved (false);SetIsManual (false);SetPosted (false);SetProcessed (false);// N
} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_AllocationHdr (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_AllocationHdr (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_AllocationHdr (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_AllocationHdr(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27884379466138L;/** Last Updated Timestamp 10/9/2020 2:35:49 PM */
public static long updatedMS = 1602254149349L;/** AD_Table_ID=735 */
public static int Table_ID; // =735;
/** TableName=C_AllocationHdr */
public static String Table_Name="C_AllocationHdr";
protected static KeyNamePair model;protected Decimal accessLevel = new Decimal(1);/** AccessLevel
@return 1 - Org 
*/
protected override int Get_AccessLevel(){return Convert.ToInt32(accessLevel.ToString());}/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Context ctx){POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);return poi;}/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Ctx ctx){POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);return poi;}/** Info
@return info
*/
public override String ToString(){StringBuilder sb = new StringBuilder ("X_C_AllocationHdr[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set Approval Amount.
@param ApprovalAmt Document Approval Amount */
public void SetApprovalAmt (Decimal? ApprovalAmt){if (ApprovalAmt == null) throw new ArgumentException ("ApprovalAmt is mandatory.");Set_Value ("ApprovalAmt", (Decimal?)ApprovalAmt);}/** Get Approval Amount.
@return Document Approval Amount */
public Decimal GetApprovalAmt() {Object bd =Get_Value("ApprovalAmt");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set Approved.
@param CMS01_IsApproved Approved */
public void SetCMS01_IsApproved (Boolean CMS01_IsApproved){Set_Value ("CMS01_IsApproved", CMS01_IsApproved);}/** Get Approved.
@return Approved */
public Boolean IsCMS01_IsApproved() {Object oo = Get_Value("CMS01_IsApproved");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Allocation.
@param C_AllocationHdr_ID Payment allocation */
public void SetC_AllocationHdr_ID (int C_AllocationHdr_ID){if (C_AllocationHdr_ID < 1) throw new ArgumentException ("C_AllocationHdr_ID is mandatory.");Set_ValueNoCheck ("C_AllocationHdr_ID", C_AllocationHdr_ID);}/** Get Allocation.
@return Payment allocation */
public int GetC_AllocationHdr_ID() {Object ii = Get_Value("C_AllocationHdr_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Business Partner.
@param C_BPartner_ID Identifies a Customer/Prospect */
public void SetC_BPartner_ID (int C_BPartner_ID){throw new ArgumentException ("C_BPartner_ID Is virtual column");}/** Get Business Partner.
@return Identifies a Customer/Prospect */
public int GetC_BPartner_ID() {Object ii = Get_Value("C_BPartner_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Currency Type.
@param C_ConversionType_ID Currency Conversion Rate Type */
public void SetC_ConversionType_ID (int C_ConversionType_ID){if (C_ConversionType_ID <= 0) Set_Value ("C_ConversionType_ID", null);else
Set_Value ("C_ConversionType_ID", C_ConversionType_ID);}/** Get Currency Type.
@return Currency Conversion Rate Type */
public int GetC_ConversionType_ID() {Object ii = Get_Value("C_ConversionType_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Currency.
@param C_Currency_ID The Currency for this record */
public void SetC_Currency_ID (int C_Currency_ID){if (C_Currency_ID < 1) throw new ArgumentException ("C_Currency_ID is mandatory.");Set_Value ("C_Currency_ID", C_Currency_ID);}/** Get Currency.
@return The Currency for this record */
public int GetC_Currency_ID() {Object ii = Get_Value("C_Currency_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Conversion Date.
@param ConversionDate Date for selecting conversion rate */
public void SetConversionDate (DateTime? ConversionDate){Set_Value ("ConversionDate", (DateTime?)ConversionDate);}/** Get Conversion Date.
@return Date for selecting conversion rate */
public DateTime? GetConversionDate() {return (DateTime?)Get_Value("ConversionDate");}/** Set Account Date.
@param DateAcct General Ledger Date */
public void SetDateAcct (DateTime? DateAcct){if (DateAcct == null) throw new ArgumentException ("DateAcct is mandatory.");Set_Value ("DateAcct", (DateTime?)DateAcct);}/** Get Account Date.
@return General Ledger Date */
public DateTime? GetDateAcct() {return (DateTime?)Get_Value("DateAcct");}/** Set Transaction Date.
@param DateTrx Transaction Date */
public void SetDateTrx (DateTime? DateTrx){if (DateTrx == null) throw new ArgumentException ("DateTrx is mandatory.");Set_Value ("DateTrx", (DateTime?)DateTrx);}/** Get Transaction Date.
@return Transaction Date */
public DateTime? GetDateTrx() {return (DateTime?)Get_Value("DateTrx");}/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description){if (Description != null && Description.Length > 255){log.Warning("Length > 255 - truncated");Description = Description.Substring(0,255);}Set_Value ("Description", Description);}/** Get Description.
@return Optional short description of the record */
public String GetDescription() {return (String)Get_Value("Description");}
/** DocAction AD_Reference_ID=135 */
public static int DOCACTION_AD_Reference_ID=135;/** <None> = -- */
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
public bool IsDocActionValid (String test){return test.Equals("--") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("IN") || test.Equals("PO") || test.Equals("PR") || test.Equals("RA") || test.Equals("RC") || test.Equals("RE") || test.Equals("RJ") || test.Equals("VO") || test.Equals("WC") || test.Equals("XL");}/** Set Document Action.
@param DocAction The targeted status of the document */
public void SetDocAction (String DocAction){if (DocAction == null) throw new ArgumentException ("DocAction is mandatory");if (!IsDocActionValid(DocAction))
throw new ArgumentException ("DocAction Invalid value - " + DocAction + " - Reference_ID=135 - -- - AP - CL - CO - IN - PO - PR - RA - RC - RE - RJ - VO - WC - XL");if (DocAction.Length > 2){log.Warning("Length > 2 - truncated");DocAction = DocAction.Substring(0,2);}Set_Value ("DocAction", DocAction);}/** Get Document Action.
@return The targeted status of the document */
public String GetDocAction() {return (String)Get_Value("DocAction");}
/** DocStatus AD_Reference_ID=131 */
public static int DOCSTATUS_AD_Reference_ID=131;/** Unknown = ?? */
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
public bool IsDocStatusValid (String test){return test.Equals("??") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("DR") || test.Equals("IN") || test.Equals("IP") || test.Equals("NA") || test.Equals("RE") || test.Equals("VO") || test.Equals("WC") || test.Equals("WP");}/** Set Document Status.
@param DocStatus The current status of the document */
public void SetDocStatus (String DocStatus){if (DocStatus == null) throw new ArgumentException ("DocStatus is mandatory");if (!IsDocStatusValid(DocStatus))
throw new ArgumentException ("DocStatus Invalid value - " + DocStatus + " - Reference_ID=131 - ?? - AP - CL - CO - DR - IN - IP - NA - RE - VO - WC - WP");if (DocStatus.Length > 2){log.Warning("Length > 2 - truncated");DocStatus = DocStatus.Substring(0,2);}Set_Value ("DocStatus", DocStatus);}/** Get Document Status.
@return The current status of the document */
public String GetDocStatus() {return (String)Get_Value("DocStatus");}/** Set Document No..
@param DocumentNo Document sequence number of the document */
public void SetDocumentNo (String DocumentNo){if (DocumentNo == null) throw new ArgumentException ("DocumentNo is mandatory.");if (DocumentNo.Length > 30){log.Warning("Length > 30 - truncated");DocumentNo = DocumentNo.Substring(0,30);}Set_Value ("DocumentNo", DocumentNo);}/** Get Document No..
@return Document sequence number of the document */
public String GetDocumentNo() {return (String)Get_Value("DocumentNo");}/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() {return new KeyNamePair(Get_ID(), GetDocumentNo());}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_ValueNoCheck ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Approved.
@param IsApproved Indicates if this document requires approval */
public void SetIsApproved (Boolean IsApproved){Set_Value ("IsApproved", IsApproved);}/** Get Approved.
@return Indicates if this document requires approval */
public Boolean IsApproved() {Object oo = Get_Value("IsApproved");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Manual.
@param IsManual This is a manual process */
public void SetIsManual (Boolean IsManual){Set_Value ("IsManual", IsManual);}/** Get Manual.
@return This is a manual process */
public Boolean IsManual() {Object oo = Get_Value("IsManual");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Posted.
@param Posted Posting status */
public void SetPosted (Boolean Posted){Set_Value ("Posted", Posted);}/** Get Posted.
@return Posting status */
public Boolean IsPosted() {Object oo = Get_Value("Posted");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Processed.
@param Processed The document has been processed */
public void SetProcessed (Boolean Processed){Set_Value ("Processed", Processed);}/** Get Processed.
@return The document has been processed */
public Boolean IsProcessed() {Object oo = Get_Value("Processed");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Process Now.
@param Processing Process Now */
public void SetProcessing (Boolean Processing){Set_Value ("Processing", Processing);}/** Get Process Now.
@return Process Now */
public Boolean IsProcessing() {Object oo = Get_Value("Processing");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set VACMS_IsApproved.
@param VACMS_IsApproved VACMS_IsApproved */
public void SetVACMS_IsApproved (Boolean VACMS_IsApproved){Set_Value ("VACMS_IsApproved", VACMS_IsApproved);}/** Get VACMS_IsApproved.
@return VACMS_IsApproved */
public Boolean IsVACMS_IsApproved() {Object oo = Get_Value("VACMS_IsApproved");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}}
}