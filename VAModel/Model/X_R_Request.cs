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
/** Generated Model for R_Request
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_R_Request : PO
{
public X_R_Request (Context ctx, int R_Request_ID, Trx trxName) : base (ctx, R_Request_ID, trxName)
{
/** if (R_Request_ID == 0)
{
SetConfidentialType (null);	// C
SetConfidentialTypeEntry (null);	// C
SetDocumentNo (null);
SetDueType (null);	// 5
SetIsEscalated (false);
SetIsInvoiced (false);
SetIsSelfService (false);	// N
SetPriority (null);	// 5
SetProcessed (false);	// N
SetR_RequestType_ID (0);
SetR_Request_ID (0);
SetRequestAmt (0.0);
SetSummary (null);
}
 */
}
public X_R_Request (Ctx ctx, int R_Request_ID, Trx trxName) : base (ctx, R_Request_ID, trxName)
{
/** if (R_Request_ID == 0)
{
SetConfidentialType (null);	// C
SetConfidentialTypeEntry (null);	// C
SetDocumentNo (null);
SetDueType (null);	// 5
SetIsEscalated (false);
SetIsInvoiced (false);
SetIsSelfService (false);	// N
SetPriority (null);	// 5
SetProcessed (false);	// N
SetR_RequestType_ID (0);
SetR_Request_ID (0);
SetRequestAmt (0.0);
SetSummary (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_Request (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_Request (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_Request (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_R_Request()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514383048L;
/** Last Updated Timestamp 7/29/2010 1:07:46 PM */
public static long updatedMS = 1280389066259L;
/** AD_Table_ID=417 */
public static int Table_ID;
 // =417;

/** TableName=R_Request */
public static String Table_Name="R_Request";

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
protected override POInfo InitPO (Ctx ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO(Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_R_Request[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Role.
@param AD_Role_ID Responsibility Role */
public void SetAD_Role_ID (int AD_Role_ID)
{
if (AD_Role_ID <= 0) Set_Value ("AD_Role_ID", null);
else
Set_Value ("AD_Role_ID", AD_Role_ID);
}
/** Get Role.
@return Responsibility Role */
public int GetAD_Role_ID() 
{
Object ii = Get_Value("AD_Role_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Table.
@param AD_Table_ID Database Table information */
public void SetAD_Table_ID (int AD_Table_ID)
{
if (AD_Table_ID <= 0) Set_ValueNoCheck ("AD_Table_ID", null);
else
Set_ValueNoCheck ("AD_Table_ID", AD_Table_ID);
}
/** Get Table.
@return Database Table information */
public int GetAD_Table_ID() 
{
Object ii = Get_Value("AD_Table_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Business Partner Contact */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID <= 0) Set_Value ("AD_User_ID", null);
else
Set_Value ("AD_User_ID", AD_User_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetAD_User_ID() 
{
Object ii = Get_Value("AD_User_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Asset.
@param A_Asset_ID Asset used internally or by customers */
public void SetA_Asset_ID (int A_Asset_ID)
{
if (A_Asset_ID <= 0) Set_Value ("A_Asset_ID", null);
else
Set_Value ("A_Asset_ID", A_Asset_ID);
}
/** Get Asset.
@return Asset used internally or by customers */
public int GetA_Asset_ID() 
{
Object ii = Get_Value("A_Asset_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Activity.
@param C_Activity_ID Business Activity */
public void SetC_Activity_ID (int C_Activity_ID)
{
if (C_Activity_ID <= 0) Set_Value ("C_Activity_ID", null);
else
Set_Value ("C_Activity_ID", C_Activity_ID);
}
/** Get Activity.
@return Business Activity */
public int GetC_Activity_ID() 
{
Object ii = Get_Value("C_Activity_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** C_BPartnerSR_ID AD_Reference_ID=353 */
public static int C_BPARTNERSR_ID_AD_Reference_ID=353;
/** Set BPartner (Agent).
@param C_BPartnerSR_ID Business Partner (Agent or Sales Rep) */
public void SetC_BPartnerSR_ID (int C_BPartnerSR_ID)
{
if (C_BPartnerSR_ID <= 0) Set_Value ("C_BPartnerSR_ID", null);
else
Set_Value ("C_BPartnerSR_ID", C_BPartnerSR_ID);
}
/** Get BPartner (Agent).
@return Business Partner (Agent or Sales Rep) */
public int GetC_BPartnerSR_ID() 
{
Object ii = Get_Value("C_BPartnerSR_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Business Partner.
@param C_BPartner_ID Identifies a Business Partner */
public void SetC_BPartner_ID (int C_BPartner_ID)
{
if (C_BPartner_ID <= 0) Set_Value ("C_BPartner_ID", null);
else
Set_Value ("C_BPartner_ID", C_BPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetC_BPartner_ID() 
{
Object ii = Get_Value("C_BPartner_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Campaign.
@param C_Campaign_ID Marketing Campaign */
public void SetC_Campaign_ID (int C_Campaign_ID)
{
if (C_Campaign_ID <= 0) Set_Value ("C_Campaign_ID", null);
else
Set_Value ("C_Campaign_ID", C_Campaign_ID);
}
/** Get Campaign.
@return Marketing Campaign */
public int GetC_Campaign_ID() 
{
Object ii = Get_Value("C_Campaign_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** C_InvoiceRequest_ID AD_Reference_ID=336 */
public static int C_INVOICEREQUEST_ID_AD_Reference_ID=336;
/** Set Request Invoice.
@param C_InvoiceRequest_ID The generated invoice for this request */
public void SetC_InvoiceRequest_ID (int C_InvoiceRequest_ID)
{
if (C_InvoiceRequest_ID <= 0) Set_ValueNoCheck ("C_InvoiceRequest_ID", null);
else
Set_ValueNoCheck ("C_InvoiceRequest_ID", C_InvoiceRequest_ID);
}
/** Get Request Invoice.
@return The generated invoice for this request */
public int GetC_InvoiceRequest_ID() 
{
Object ii = Get_Value("C_InvoiceRequest_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Invoice.
@param C_Invoice_ID Invoice Identifier */
public void SetC_Invoice_ID (int C_Invoice_ID)
{
if (C_Invoice_ID <= 0) Set_Value ("C_Invoice_ID", null);
else
Set_Value ("C_Invoice_ID", C_Invoice_ID);
}
/** Get Invoice.
@return Invoice Identifier */
public int GetC_Invoice_ID() 
{
Object ii = Get_Value("C_Invoice_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Lead.
@param C_Lead_ID Business Lead */
public void SetC_Lead_ID (int C_Lead_ID)
{
if (C_Lead_ID <= 0) Set_Value ("C_Lead_ID", null);
else
Set_Value ("C_Lead_ID", C_Lead_ID);
}
/** Get Lead.
@return Business Lead */
public int GetC_Lead_ID() 
{
Object ii = Get_Value("C_Lead_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Order.
@param C_Order_ID Order */
public void SetC_Order_ID (int C_Order_ID)
{
if (C_Order_ID <= 0) Set_Value ("C_Order_ID", null);
else
Set_Value ("C_Order_ID", C_Order_ID);
}
/** Get Order.
@return Order */
public int GetC_Order_ID() 
{
Object ii = Get_Value("C_Order_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Payment.
@param C_Payment_ID Payment identifier */
public void SetC_Payment_ID (int C_Payment_ID)
{
if (C_Payment_ID <= 0) Set_Value ("C_Payment_ID", null);
else
Set_Value ("C_Payment_ID", C_Payment_ID);
}
/** Get Payment.
@return Payment identifier */
public int GetC_Payment_ID() 
{
Object ii = Get_Value("C_Payment_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project.
@param C_Project_ID Financial Project */
public void SetC_Project_ID (int C_Project_ID)
{
if (C_Project_ID <= 0) Set_Value ("C_Project_ID", null);
else
Set_Value ("C_Project_ID", C_Project_ID);
}
/** Get Project.
@return Financial Project */
public int GetC_Project_ID() 
{
Object ii = Get_Value("C_Project_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Sales Region.
@param C_SalesRegion_ID Sales coverage region */
public void SetC_SalesRegion_ID (int C_SalesRegion_ID)
{
if (C_SalesRegion_ID <= 0) Set_Value ("C_SalesRegion_ID", null);
else
Set_Value ("C_SalesRegion_ID", C_SalesRegion_ID);
}
/** Get Sales Region.
@return Sales coverage region */
public int GetC_SalesRegion_ID() 
{
Object ii = Get_Value("C_SalesRegion_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Close Date.
@param CloseDate Close Date */
public void SetCloseDate (DateTime? CloseDate)
{
Set_Value ("CloseDate", (DateTime?)CloseDate);
}
/** Get Close Date.
@return Close Date */
public DateTime? GetCloseDate() 
{
return (DateTime?)Get_Value("CloseDate");
}

/** ConfidentialType AD_Reference_ID=340 */
public static int CONFIDENTIALTYPE_AD_Reference_ID=340;
/** Public Information = A */
public static String CONFIDENTIALTYPE_PublicInformation = "A";
/** Partner Confidential = C */
public static String CONFIDENTIALTYPE_PartnerConfidential = "C";
/** Internal = I */
public static String CONFIDENTIALTYPE_Internal = "I";
/** Private Information = P */
public static String CONFIDENTIALTYPE_PrivateInformation = "P";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsConfidentialTypeValid (String test)
{
return test.Equals("A") || test.Equals("C") || test.Equals("I") || test.Equals("P");
}
/** Set Confidentiality.
@param ConfidentialType Type of Confidentiality */
public void SetConfidentialType (String ConfidentialType)
{
if (ConfidentialType == null) throw new ArgumentException ("ConfidentialType is mandatory");
if (!IsConfidentialTypeValid(ConfidentialType))
throw new ArgumentException ("ConfidentialType Invalid value - " + ConfidentialType + " - Reference_ID=340 - A - C - I - P");
if (ConfidentialType.Length > 1)
{
log.Warning("Length > 1 - truncated");
ConfidentialType = ConfidentialType.Substring(0,1);
}
Set_Value ("ConfidentialType", ConfidentialType);
}
/** Get Confidentiality.
@return Type of Confidentiality */
public String GetConfidentialType() 
{
return (String)Get_Value("ConfidentialType");
}

/** ConfidentialTypeEntry AD_Reference_ID=340 */
public static int CONFIDENTIALTYPEENTRY_AD_Reference_ID=340;
/** Public Information = A */
public static String CONFIDENTIALTYPEENTRY_PublicInformation = "A";
/** Partner Confidential = C */
public static String CONFIDENTIALTYPEENTRY_PartnerConfidential = "C";
/** Internal = I */
public static String CONFIDENTIALTYPEENTRY_Internal = "I";
/** Private Information = P */
public static String CONFIDENTIALTYPEENTRY_PrivateInformation = "P";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsConfidentialTypeEntryValid (String test)
{
return test.Equals("A") || test.Equals("C") || test.Equals("I") || test.Equals("P");
}
/** Set Entry Access Level.
@param ConfidentialTypeEntry Confidentiality of the individual entry */
public void SetConfidentialTypeEntry (String ConfidentialTypeEntry)
{
if (ConfidentialTypeEntry == null) throw new ArgumentException ("ConfidentialTypeEntry is mandatory");
if (!IsConfidentialTypeEntryValid(ConfidentialTypeEntry))
throw new ArgumentException ("ConfidentialTypeEntry Invalid value - " + ConfidentialTypeEntry + " - Reference_ID=340 - A - C - I - P");
if (ConfidentialTypeEntry.Length > 1)
{
log.Warning("Length > 1 - truncated");
ConfidentialTypeEntry = ConfidentialTypeEntry.Substring(0,1);
}
Set_Value ("ConfidentialTypeEntry", ConfidentialTypeEntry);
}
/** Get Entry Access Level.
@return Confidentiality of the individual entry */
public String GetConfidentialTypeEntry() 
{
return (String)Get_Value("ConfidentialTypeEntry");
}
/** Set Complete Plan.
@param DateCompletePlan Planned Completion Date */
public void SetDateCompletePlan (DateTime? DateCompletePlan)
{
Set_Value ("DateCompletePlan", (DateTime?)DateCompletePlan);
}
/** Get Complete Plan.
@return Planned Completion Date */
public DateTime? GetDateCompletePlan() 
{
return (DateTime?)Get_Value("DateCompletePlan");
}
/** Set Date last action.
@param DateLastAction Date this request was last acted on */
public void SetDateLastAction (DateTime? DateLastAction)
{
Set_ValueNoCheck ("DateLastAction", (DateTime?)DateLastAction);
}
/** Get Date last action.
@return Date this request was last acted on */
public DateTime? GetDateLastAction() 
{
return (DateTime?)Get_Value("DateLastAction");
}
/** Set Last Alert.
@param DateLastAlert Date when last alert were sent */
public void SetDateLastAlert (DateTime? DateLastAlert)
{
Set_Value ("DateLastAlert", (DateTime?)DateLastAlert);
}
/** Get Last Alert.
@return Date when last alert were sent */
public DateTime? GetDateLastAlert() 
{
return (DateTime?)Get_Value("DateLastAlert");
}
/** Set Date next action.
@param DateNextAction Date that this request should be acted on */
public void SetDateNextAction (DateTime? DateNextAction)
{
Set_Value ("DateNextAction", (DateTime?)DateNextAction);
}
/** Get Date next action.
@return Date that this request should be acted on */
public DateTime? GetDateNextAction() 
{
return (DateTime?)Get_Value("DateNextAction");
}
/** Set Start Plan.
@param DateStartPlan Planned Start Date */
public void SetDateStartPlan (DateTime? DateStartPlan)
{
Set_Value ("DateStartPlan", (DateTime?)DateStartPlan);
}
/** Get Start Plan.
@return Planned Start Date */
public DateTime? GetDateStartPlan() 
{
return (DateTime?)Get_Value("DateStartPlan");
}
/** Set Document No.
@param DocumentNo Document sequence number of the document */
public void SetDocumentNo (String DocumentNo)
{
if (DocumentNo == null) throw new ArgumentException ("DocumentNo is mandatory.");
if (DocumentNo.Length > 30)
{
log.Warning("Length > 30 - truncated");
DocumentNo = DocumentNo.Substring(0,30);
}
Set_Value ("DocumentNo", DocumentNo);
}
/** Get Document No.
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

/** DueType AD_Reference_ID=222 */
public static int DUETYPE_AD_Reference_ID=222;
/** Overdue = 3 */
public static String DUETYPE_Overdue = "3";
/** Due = 5 */
public static String DUETYPE_Due = "5";
/** Scheduled = 7 */
public static String DUETYPE_Scheduled = "7";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsDueTypeValid (String test)
{
return test.Equals("3") || test.Equals("5") || test.Equals("7");
}
/** Set Aging Status.
@param DueType Status of the next action for this Request */
public void SetDueType (String DueType)
{
if (DueType == null) throw new ArgumentException ("DueType is mandatory");
if (!IsDueTypeValid(DueType))
throw new ArgumentException ("DueType Invalid value - " + DueType + " - Reference_ID=222 - 3 - 5 - 7");
if (DueType.Length > 1)
{
log.Warning("Length > 1 - truncated");
DueType = DueType.Substring(0,1);
}
Set_Value ("DueType", DueType);
}
/** Get Aging Status.
@return Status of the next action for this Request */
public String GetDueType() 
{
return (String)Get_Value("DueType");
}
/** Set End Time.
@param EndTime End of the time span */
public void SetEndTime (DateTime? EndTime)
{
Set_Value ("EndTime", (DateTime?)EndTime);
}
/** Get End Time.
@return End of the time span */
public DateTime? GetEndTime() 
{
return (DateTime?)Get_Value("EndTime");
}
/** Set Escalated.
@param IsEscalated This request has been escalated */
public void SetIsEscalated (Boolean IsEscalated)
{
Set_Value ("IsEscalated", IsEscalated);
}
/** Get Escalated.
@return This request has been escalated */
public Boolean IsEscalated() 
{
Object oo = Get_Value("IsEscalated");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Invoiced.
@param IsInvoiced Is this invoiced? */
public void SetIsInvoiced (Boolean IsInvoiced)
{
Set_Value ("IsInvoiced", IsInvoiced);
}
/** Get Invoiced.
@return Is this invoiced? */
public Boolean IsInvoiced() 
{
Object oo = Get_Value("IsInvoiced");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Self-Service.
@param IsSelfService This is a Self-Service entry or this entry can be changed via Self-Service */
public void SetIsSelfService (Boolean IsSelfService)
{
Set_ValueNoCheck ("IsSelfService", IsSelfService);
}
/** Get Self-Service.
@return This is a Self-Service entry or this entry can be changed via Self-Service */
public Boolean IsSelfService() 
{
Object oo = Get_Value("IsSelfService");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Last Result.
@param LastResult Result of last contact */
public void SetLastResult (String LastResult)
{
if (LastResult != null && LastResult.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
LastResult = LastResult.Substring(0,2000);
}
Set_Value ("LastResult", LastResult);
}
/** Get Last Result.
@return Result of last contact */
public String GetLastResult() 
{
return (String)Get_Value("LastResult");
}
/** Set Change Request.
@param M_ChangeRequest_ID BOM (Engineering) Change Request */
public void SetM_ChangeRequest_ID (int M_ChangeRequest_ID)
{
if (M_ChangeRequest_ID <= 0) Set_Value ("M_ChangeRequest_ID", null);
else
Set_Value ("M_ChangeRequest_ID", M_ChangeRequest_ID);
}
/** Get Change Request.
@return BOM (Engineering) Change Request */
public int GetM_ChangeRequest_ID() 
{
Object ii = Get_Value("M_ChangeRequest_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** M_FixChangeNotice_ID AD_Reference_ID=351 */
public static int M_FIXCHANGENOTICE_ID_AD_Reference_ID=351;
/** Set Fixed in.
@param M_FixChangeNotice_ID Fixed in Change Notice */
public void SetM_FixChangeNotice_ID (int M_FixChangeNotice_ID)
{
if (M_FixChangeNotice_ID <= 0) Set_Value ("M_FixChangeNotice_ID", null);
else
Set_Value ("M_FixChangeNotice_ID", M_FixChangeNotice_ID);
}
/** Get Fixed in.
@return Fixed in Change Notice */
public int GetM_FixChangeNotice_ID() 
{
Object ii = Get_Value("M_FixChangeNotice_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Shipment/Receipt.
@param M_InOut_ID Material Shipment Document */
public void SetM_InOut_ID (int M_InOut_ID)
{
if (M_InOut_ID <= 0) Set_Value ("M_InOut_ID", null);
else
Set_Value ("M_InOut_ID", M_InOut_ID);
}
/** Get Shipment/Receipt.
@return Material Shipment Document */
public int GetM_InOut_ID() 
{
Object ii = Get_Value("M_InOut_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** M_ProductSpent_ID AD_Reference_ID=162 */
public static int M_PRODUCTSPENT_ID_AD_Reference_ID=162;
/** Set Product Used.
@param M_ProductSpent_ID Product/Resource/Service used in Request */
public void SetM_ProductSpent_ID (int M_ProductSpent_ID)
{
if (M_ProductSpent_ID <= 0) Set_Value ("M_ProductSpent_ID", null);
else
Set_Value ("M_ProductSpent_ID", M_ProductSpent_ID);
}
/** Get Product Used.
@return Product/Resource/Service used in Request */
public int GetM_ProductSpent_ID() 
{
Object ii = Get_Value("M_ProductSpent_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID)
{
if (M_Product_ID <= 0) Set_Value ("M_Product_ID", null);
else
Set_Value ("M_Product_ID", M_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() 
{
Object ii = Get_Value("M_Product_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** NextAction AD_Reference_ID=219 */
public static int NEXTACTION_AD_Reference_ID=219;
/** Follow up = F */
public static String NEXTACTION_FollowUp = "F";
/** None = N */
public static String NEXTACTION_None = "N";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsNextActionValid (String test)
{
return test == null || test.Equals("F") || test.Equals("N");
}
/** Set Next action.
@param NextAction Next Action to be taken */
public void SetNextAction (String NextAction)
{
if (!IsNextActionValid(NextAction))
throw new ArgumentException ("NextAction Invalid value - " + NextAction + " - Reference_ID=219 - F - N");
if (NextAction != null && NextAction.Length > 1)
{
log.Warning("Length > 1 - truncated");
NextAction = NextAction.Substring(0,1);
}
Set_Value ("NextAction", NextAction);
}
/** Get Next action.
@return Next Action to be taken */
public String GetNextAction() 
{
return (String)Get_Value("NextAction");
}

/** Priority AD_Reference_ID=154 */
public static int PRIORITY_AD_Reference_ID=154;
/** Urgent = 1 */
public static String PRIORITY_Urgent = "1";
/** High = 3 */
public static String PRIORITY_High = "3";
/** Medium = 5 */
public static String PRIORITY_Medium = "5";
/** Low = 7 */
public static String PRIORITY_Low = "7";
/** Minor = 9 */
public static String PRIORITY_Minor = "9";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsPriorityValid (String test)
{
return test.Equals("1") || test.Equals("3") || test.Equals("5") || test.Equals("7") || test.Equals("9");
}
/** Set Priority.
@param Priority Indicates if this request is of a high, medium or low priority. */
public void SetPriority (String Priority)
{
if (Priority == null) throw new ArgumentException ("Priority is mandatory");
if (!IsPriorityValid(Priority))
throw new ArgumentException ("Priority Invalid value - " + Priority + " - Reference_ID=154 - 1 - 3 - 5 - 7 - 9");
if (Priority.Length > 1)
{
log.Warning("Length > 1 - truncated");
Priority = Priority.Substring(0,1);
}
Set_Value ("Priority", Priority);
}
/** Get Priority.
@return Indicates if this request is of a high, medium or low priority. */
public String GetPriority() 
{
return (String)Get_Value("Priority");
}

/** PriorityUser AD_Reference_ID=154 */
public static int PRIORITYUSER_AD_Reference_ID=154;
/** Urgent = 1 */
public static String PRIORITYUSER_Urgent = "1";
/** High = 3 */
public static String PRIORITYUSER_High = "3";
/** Medium = 5 */
public static String PRIORITYUSER_Medium = "5";
/** Low = 7 */
public static String PRIORITYUSER_Low = "7";
/** Minor = 9 */
public static String PRIORITYUSER_Minor = "9";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsPriorityUserValid (String test)
{
return test == null || test.Equals("1") || test.Equals("3") || test.Equals("5") || test.Equals("7") || test.Equals("9");
}
/** Set User Priority.
@param PriorityUser Priority of the issue for the User */
public void SetPriorityUser (String PriorityUser)
{
if (!IsPriorityUserValid(PriorityUser))
throw new ArgumentException ("PriorityUser Invalid value - " + PriorityUser + " - Reference_ID=154 - 1 - 3 - 5 - 7 - 9");
if (PriorityUser != null && PriorityUser.Length > 1)
{
log.Warning("Length > 1 - truncated");
PriorityUser = PriorityUser.Substring(0,1);
}
Set_Value ("PriorityUser", PriorityUser);
}
/** Get User Priority.
@return Priority of the issue for the User */
public String GetPriorityUser() 
{
return (String)Get_Value("PriorityUser");
}
/** Set Processed.
@param Processed The document has been processed */
public void SetProcessed (Boolean Processed)
{
Set_Value ("Processed", Processed);
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
/** Set Quantity Invoiced.
@param QtyInvoiced Invoiced Quantity */
public void SetQtyInvoiced (Decimal? QtyInvoiced)
{
Set_Value ("QtyInvoiced", (Decimal?)QtyInvoiced);
}
/** Get Quantity Invoiced.
@return Invoiced Quantity */
public Decimal GetQtyInvoiced() 
{
Object bd =Get_Value("QtyInvoiced");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Quantity Plan.
@param QtyPlan Planned Quantity */
public void SetQtyPlan (Decimal? QtyPlan)
{
Set_Value ("QtyPlan", (Decimal?)QtyPlan);
}
/** Get Quantity Plan.
@return Planned Quantity */
public Decimal GetQtyPlan() 
{
Object bd =Get_Value("QtyPlan");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Quantity Used.
@param QtySpent Quantity used for this event */
public void SetQtySpent (Decimal? QtySpent)
{
Set_Value ("QtySpent", (Decimal?)QtySpent);
}
/** Get Quantity Used.
@return Quantity used for this event */
public Decimal GetQtySpent() 
{
Object bd =Get_Value("QtySpent");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Category.
@param R_Category_ID Request Category */
public void SetR_Category_ID (int R_Category_ID)
{
if (R_Category_ID <= 0) Set_Value ("R_Category_ID", null);
else
Set_Value ("R_Category_ID", R_Category_ID);
}
/** Get Category.
@return Request Category */
public int GetR_Category_ID() 
{
Object ii = Get_Value("R_Category_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Group.
@param R_Group_ID Request Group */
public void SetR_Group_ID (int R_Group_ID)
{
if (R_Group_ID <= 0) Set_Value ("R_Group_ID", null);
else
Set_Value ("R_Group_ID", R_Group_ID);
}
/** Get Group.
@return Request Group */
public int GetR_Group_ID() 
{
Object ii = Get_Value("R_Group_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Mail Template.
@param R_MailText_ID Text templates for mailings */
public void SetR_MailText_ID (int R_MailText_ID)
{
if (R_MailText_ID <= 0) Set_Value ("R_MailText_ID", null);
else
Set_Value ("R_MailText_ID", R_MailText_ID);
}
/** Get Mail Template.
@return Text templates for mailings */
public int GetR_MailText_ID() 
{
Object ii = Get_Value("R_MailText_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** R_RequestRelated_ID AD_Reference_ID=341 */
public static int R_REQUESTRELATED_ID_AD_Reference_ID=341;
/** Set Related Request.
@param R_RequestRelated_ID Related Request (Master Issue, ..) */
public void SetR_RequestRelated_ID (int R_RequestRelated_ID)
{
if (R_RequestRelated_ID <= 0) Set_Value ("R_RequestRelated_ID", null);
else
Set_Value ("R_RequestRelated_ID", R_RequestRelated_ID);
}
/** Get Related Request.
@return Related Request (Master Issue, ..) */
public int GetR_RequestRelated_ID() 
{
Object ii = Get_Value("R_RequestRelated_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Request Type.
@param R_RequestType_ID Type of request (e.g. Inquiry, Complaint, ..) */
public void SetR_RequestType_ID (int R_RequestType_ID)
{
if (R_RequestType_ID < 1) throw new ArgumentException ("R_RequestType_ID is mandatory.");
Set_Value ("R_RequestType_ID", R_RequestType_ID);
}
/** Get Request Type.
@return Type of request (e.g. Inquiry, Complaint, ..) */
public int GetR_RequestType_ID() 
{
Object ii = Get_Value("R_RequestType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Request.
@param R_Request_ID Request from a Business Partner or Prospect */
public void SetR_Request_ID (int R_Request_ID)
{
if (R_Request_ID < 1) throw new ArgumentException ("R_Request_ID is mandatory.");
Set_ValueNoCheck ("R_Request_ID", R_Request_ID);
}
/** Get Request.
@return Request from a Business Partner or Prospect */
public int GetR_Request_ID() 
{
Object ii = Get_Value("R_Request_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Resolution.
@param R_Resolution_ID Request Resolution */
public void SetR_Resolution_ID (int R_Resolution_ID)
{
if (R_Resolution_ID <= 0) Set_Value ("R_Resolution_ID", null);
else
Set_Value ("R_Resolution_ID", R_Resolution_ID);
}
/** Get Resolution.
@return Request Resolution */
public int GetR_Resolution_ID() 
{
Object ii = Get_Value("R_Resolution_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Source.
@param R_Source_ID Source for the Lead or Request */
public void SetR_Source_ID (int R_Source_ID)
{
if (R_Source_ID <= 0) Set_Value ("R_Source_ID", null);
else
Set_Value ("R_Source_ID", R_Source_ID);
}
/** Get Source.
@return Source for the Lead or Request */
public int GetR_Source_ID() 
{
Object ii = Get_Value("R_Source_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Standard Response.
@param R_StandardResponse_ID Request Standard Response */
public void SetR_StandardResponse_ID (int R_StandardResponse_ID)
{
if (R_StandardResponse_ID <= 0) Set_Value ("R_StandardResponse_ID", null);
else
Set_Value ("R_StandardResponse_ID", R_StandardResponse_ID);
}
/** Get Standard Response.
@return Request Standard Response */
public int GetR_StandardResponse_ID() 
{
Object ii = Get_Value("R_StandardResponse_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Status.
@param R_Status_ID Request Status */
public void SetR_Status_ID (int R_Status_ID)
{
if (R_Status_ID <= 0) Set_Value ("R_Status_ID", null);
else
Set_Value ("R_Status_ID", R_Status_ID);
}
/** Get Status.
@return Request Status */
public int GetR_Status_ID() 
{
Object ii = Get_Value("R_Status_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Record ID.
@param Record_ID Direct internal record ID */
public void SetRecord_ID (int Record_ID)
{
if (Record_ID <= 0) Set_ValueNoCheck ("Record_ID", null);
else
Set_ValueNoCheck ("Record_ID", Record_ID);
}
/** Get Record ID.
@return Direct internal record ID */
public int GetRecord_ID() 
{
Object ii = Get_Value("Record_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Request Amount.
@param RequestAmt Amount associated with this request */
public void SetRequestAmt (Decimal? RequestAmt)
{
if (RequestAmt == null) throw new ArgumentException ("RequestAmt is mandatory.");
Set_Value ("RequestAmt", (Decimal?)RequestAmt);
}
/** Get Request Amount.
@return Amount associated with this request */
public Decimal GetRequestAmt() 
{
Object bd =Get_Value("RequestAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Result.
@param Result Result of the action taken */
public void SetResult (String Result)
{
if (Result != null && Result.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Result = Result.Substring(0,2000);
}
Set_Value ("Result", Result);
}
/** Get Result.
@return Result of the action taken */
public String GetResult() 
{
return (String)Get_Value("Result");
}

/** SalesRep_ID AD_Reference_ID=286 */
public static int SALESREP_ID_AD_Reference_ID=286;
/** Set Representative.
@param SalesRep_ID Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
public void SetSalesRep_ID (int SalesRep_ID)
{
if (SalesRep_ID <= 0) Set_Value ("SalesRep_ID", null);
else
Set_Value ("SalesRep_ID", SalesRep_ID);
}
/** Get Representative.
@return Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
public int GetSalesRep_ID() 
{
Object ii = Get_Value("SalesRep_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Start Date.
@param StartDate First effective day (inclusive) */
public void SetStartDate (DateTime? StartDate)
{
Set_Value ("StartDate", (DateTime?)StartDate);
}
/** Get Start Date.
@return First effective day (inclusive) */
public DateTime? GetStartDate() 
{
return (DateTime?)Get_Value("StartDate");
}
/** Set Start Time.
@param StartTime Time started */
public void SetStartTime (DateTime? StartTime)
{
Set_Value ("StartTime", (DateTime?)StartTime);
}
/** Get Start Time.
@return Time started */
public DateTime? GetStartTime() 
{
return (DateTime?)Get_Value("StartTime");
}
/** Set Summary.
@param Summary Textual summary of this request */
public void SetSummary (String Summary)
{
if (Summary == null) throw new ArgumentException ("Summary is mandatory.");
if (Summary.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Summary = Summary.Substring(0,2000);
}
Set_Value ("Summary", Summary);
}
/** Get Summary.
@return Textual summary of this request */
public String GetSummary() 
{
return (String)Get_Value("Summary");
}

/** TaskStatus AD_Reference_ID=366 */
public static int TASKSTATUS_AD_Reference_ID=366;
/** 0% Not Started = 0 */
public static String TASKSTATUS_0NotStarted = "0";
/** 20% Started = 2 */
public static String TASKSTATUS_20Started = "2";
/** 40% Busy = 4 */
public static String TASKSTATUS_40Busy = "4";
/** 60% Good Progress = 6 */
public static String TASKSTATUS_60GoodProgress = "6";
/** 80% Nearly Done = 8 */
public static String TASKSTATUS_80NearlyDone = "8";
/** 90% Finishing = 9 */
public static String TASKSTATUS_90Finishing = "9";
/** 95% Almost Done = A */
public static String TASKSTATUS_95AlmostDone = "A";
/** 99% Cleaning up = C */
public static String TASKSTATUS_99CleaningUp = "C";
/** 100% Complete = D */
public static String TASKSTATUS_100Complete = "D";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsTaskStatusValid (String test)
{
return test == null || test.Equals("0") || test.Equals("2") || test.Equals("4") || test.Equals("6") || test.Equals("8") || test.Equals("9") || test.Equals("A") || test.Equals("C") || test.Equals("D");
}
/** Set Task Status.
@param TaskStatus Status of the Task */
public void SetTaskStatus (String TaskStatus)
{
if (!IsTaskStatusValid(TaskStatus))
throw new ArgumentException ("TaskStatus Invalid value - " + TaskStatus + " - Reference_ID=366 - 0 - 2 - 4 - 6 - 8 - 9 - A - C - D");
if (TaskStatus != null && TaskStatus.Length > 1)
{
log.Warning("Length > 1 - truncated");
TaskStatus = TaskStatus.Substring(0,1);
}
Set_Value ("TaskStatus", TaskStatus);
}
/** Get Task Status.
@return Status of the Task */
public String GetTaskStatus() 
{
return (String)Get_Value("TaskStatus");
}
}

}
