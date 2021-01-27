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
/** Generated Model for VAR_Request
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAR_Request : PO
{
public X_VAR_Request (Context ctx, int VAR_Request_ID, Trx trxName) : base (ctx, VAR_Request_ID, trxName)
{
/** if (VAR_Request_ID == 0)
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
SetVAR_Req_Type_ID (0);
SetVAR_Request_ID (0);
SetRequestAmt (0.0);
SetSummary (null);
}
 */
}
public X_VAR_Request (Ctx ctx, int VAR_Request_ID, Trx trxName) : base (ctx, VAR_Request_ID, trxName)
{
/** if (VAR_Request_ID == 0)
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
SetVAR_Req_Type_ID (0);
SetVAR_Request_ID (0);
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
public X_VAR_Request (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAR_Request (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAR_Request (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAR_Request()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514383048L;
/** Last Updated Timestamp 7/29/2010 1:07:46 PM */
public static long updatedMS = 1280389066259L;
/** VAF_TableView_ID=417 */
public static int Table_ID;
 // =417;

/** TableName=VAR_Request */
public static String Table_Name="VAR_Request";

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
StringBuilder sb = new StringBuilder ("X_VAR_Request[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Role.
@param VAF_Role_ID Responsibility Role */
public void SetVAF_Role_ID (int VAF_Role_ID)
{
if (VAF_Role_ID <= 0) Set_Value ("VAF_Role_ID", null);
else
Set_Value ("VAF_Role_ID", VAF_Role_ID);
}
/** Get Role.
@return Responsibility Role */
public int GetVAF_Role_ID() 
{
Object ii = Get_Value("VAF_Role_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Table.
@param VAF_TableView_ID Database Table information */
public void SetVAF_TableView_ID (int VAF_TableView_ID)
{
if (VAF_TableView_ID <= 0) Set_ValueNoCheck ("VAF_TableView_ID", null);
else
Set_ValueNoCheck ("VAF_TableView_ID", VAF_TableView_ID);
}
/** Get Table.
@return Database Table information */
public int GetVAF_TableView_ID() 
{
Object ii = Get_Value("VAF_TableView_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User/Contact.
@param VAF_UserContact_ID User within the system - Internal or Business Partner Contact */
public void SetVAF_UserContact_ID (int VAF_UserContact_ID)
{
if (VAF_UserContact_ID <= 0) Set_Value ("VAF_UserContact_ID", null);
else
Set_Value ("VAF_UserContact_ID", VAF_UserContact_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetVAF_UserContact_ID() 
{
Object ii = Get_Value("VAF_UserContact_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Asset.
@param VAA_Asset_ID Asset used internally or by customers */
public void SetA_Asset_ID (int VAA_Asset_ID)
{
if (VAA_Asset_ID <= 0) Set_Value ("VAA_Asset_ID", null);
else
Set_Value ("VAA_Asset_ID", VAA_Asset_ID);
}
/** Get Asset.
@return Asset used internally or by customers */
public int GetA_Asset_ID() 
{
Object ii = Get_Value("VAA_Asset_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Activity.
@param VAB_BillingCode_ID Business Activity */
public void SetVAB_BillingCode_ID (int VAB_BillingCode_ID)
{
if (VAB_BillingCode_ID <= 0) Set_Value ("VAB_BillingCode_ID", null);
else
Set_Value ("VAB_BillingCode_ID", VAB_BillingCode_ID);
}
/** Get Activity.
@return Business Activity */
public int GetVAB_BillingCode_ID() 
{
Object ii = Get_Value("VAB_BillingCode_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAB_BusinessPartnerSR_ID VAF_Control_Ref_ID=353 */
public static int VAB_BUSINESSPARTNERSR_ID_VAF_Control_Ref_ID=353;
/** Set BPartner (Agent).
@param VAB_BusinessPartnerSR_ID Business Partner (Agent or Sales Rep) */
public void SetVAB_BusinessPartnerSR_ID (int VAB_BusinessPartnerSR_ID)
{
if (VAB_BusinessPartnerSR_ID <= 0) Set_Value ("VAB_BusinessPartnerSR_ID", null);
else
Set_Value ("VAB_BusinessPartnerSR_ID", VAB_BusinessPartnerSR_ID);
}
/** Get BPartner (Agent).
@return Business Partner (Agent or Sales Rep) */
public int GetVAB_BusinessPartnerSR_ID() 
{
Object ii = Get_Value("VAB_BusinessPartnerSR_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Business Partner.
@param VAB_BusinessPartner_ID Identifies a Business Partner */
public void SetVAB_BusinessPartner_ID (int VAB_BusinessPartner_ID)
{
if (VAB_BusinessPartner_ID <= 0) Set_Value ("VAB_BusinessPartner_ID", null);
else
Set_Value ("VAB_BusinessPartner_ID", VAB_BusinessPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetVAB_BusinessPartner_ID() 
{
Object ii = Get_Value("VAB_BusinessPartner_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Campaign.
@param VAB_Promotion_ID Marketing Campaign */
public void SetVAB_Promotion_ID (int VAB_Promotion_ID)
{
if (VAB_Promotion_ID <= 0) Set_Value ("VAB_Promotion_ID", null);
else
Set_Value ("VAB_Promotion_ID", VAB_Promotion_ID);
}
/** Get Campaign.
@return Marketing Campaign */
public int GetVAB_Promotion_ID() 
{
Object ii = Get_Value("VAB_Promotion_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAB_InvoiceRequest_ID VAF_Control_Ref_ID=336 */
public static int VAB_INVOICEREQUEST_ID_VAF_Control_Ref_ID=336;
/** Set Request Invoice.
@param VAB_InvoiceRequest_ID The generated invoice for this request */
public void SetVAB_InvoiceRequest_ID (int VAB_InvoiceRequest_ID)
{
if (VAB_InvoiceRequest_ID <= 0) Set_ValueNoCheck ("VAB_InvoiceRequest_ID", null);
else
Set_ValueNoCheck ("VAB_InvoiceRequest_ID", VAB_InvoiceRequest_ID);
}
/** Get Request Invoice.
@return The generated invoice for this request */
public int GetVAB_InvoiceRequest_ID() 
{
Object ii = Get_Value("VAB_InvoiceRequest_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Invoice.
@param VAB_Invoice_ID Invoice Identifier */
public void SetVAB_Invoice_ID (int VAB_Invoice_ID)
{
if (VAB_Invoice_ID <= 0) Set_Value ("VAB_Invoice_ID", null);
else
Set_Value ("VAB_Invoice_ID", VAB_Invoice_ID);
}
/** Get Invoice.
@return Invoice Identifier */
public int GetVAB_Invoice_ID() 
{
Object ii = Get_Value("VAB_Invoice_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Lead.
@param VAB_Lead_ID Business Lead */
public void SetVAB_Lead_ID (int VAB_Lead_ID)
{
if (VAB_Lead_ID <= 0) Set_Value ("VAB_Lead_ID", null);
else
Set_Value ("VAB_Lead_ID", VAB_Lead_ID);
}
/** Get Lead.
@return Business Lead */
public int GetVAB_Lead_ID() 
{
Object ii = Get_Value("VAB_Lead_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Order.
@param VAB_Order_ID Order */
public void SetVAB_Order_ID (int VAB_Order_ID)
{
if (VAB_Order_ID <= 0) Set_Value ("VAB_Order_ID", null);
else
Set_Value ("VAB_Order_ID", VAB_Order_ID);
}
/** Get Order.
@return Order */
public int GetVAB_Order_ID() 
{
Object ii = Get_Value("VAB_Order_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Payment.
@param VAB_Payment_ID Payment identifier */
public void SetVAB_Payment_ID (int VAB_Payment_ID)
{
if (VAB_Payment_ID <= 0) Set_Value ("VAB_Payment_ID", null);
else
Set_Value ("VAB_Payment_ID", VAB_Payment_ID);
}
/** Get Payment.
@return Payment identifier */
public int GetVAB_Payment_ID() 
{
Object ii = Get_Value("VAB_Payment_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project.
@param VAB_Project_ID Financial Project */
public void SetVAB_Project_ID (int VAB_Project_ID)
{
if (VAB_Project_ID <= 0) Set_Value ("VAB_Project_ID", null);
else
Set_Value ("VAB_Project_ID", VAB_Project_ID);
}
/** Get Project.
@return Financial Project */
public int GetVAB_Project_ID() 
{
Object ii = Get_Value("VAB_Project_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Sales Region.
@param VAB_SalesRegionState_ID Sales coverage region */
public void SetVAB_SalesRegionState_ID (int VAB_SalesRegionState_ID)
{
if (VAB_SalesRegionState_ID <= 0) Set_Value ("VAB_SalesRegionState_ID", null);
else
Set_Value ("VAB_SalesRegionState_ID", VAB_SalesRegionState_ID);
}
/** Get Sales Region.
@return Sales coverage region */
public int GetVAB_SalesRegionState_ID() 
{
Object ii = Get_Value("VAB_SalesRegionState_ID");
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

/** ConfidentialType VAF_Control_Ref_ID=340 */
public static int CONFIDENTIALTYPE_VAF_Control_Ref_ID=340;
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

/** ConfidentialTypeEntry VAF_Control_Ref_ID=340 */
public static int CONFIDENTIALTYPEENTRY_VAF_Control_Ref_ID=340;
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

/** DueType VAF_Control_Ref_ID=222 */
public static int DUETYPE_VAF_Control_Ref_ID=222;
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

/** M_FixChangeNotice_ID VAF_Control_Ref_ID=351 */
public static int M_FIXCHANGENOTICE_ID_VAF_Control_Ref_ID=351;
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

/** M_ProductSpent_ID VAF_Control_Ref_ID=162 */
public static int M_PRODUCTSPENT_ID_VAF_Control_Ref_ID=162;
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

/** NextAction VAF_Control_Ref_ID=219 */
public static int NEXTACTION_VAF_Control_Ref_ID=219;
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

/** Priority VAF_Control_Ref_ID=154 */
public static int PRIORITY_VAF_Control_Ref_ID=154;
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

/** PriorityUser VAF_Control_Ref_ID=154 */
public static int PRIORITYUSER_VAF_Control_Ref_ID=154;
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
@param VAR_Category_ID Request Category */
public void SetVAR_Category_ID (int VAR_Category_ID)
{
if (VAR_Category_ID <= 0) Set_Value ("VAR_Category_ID", null);
else
Set_Value ("VAR_Category_ID", VAR_Category_ID);
}
/** Get Category.
@return Request Category */
public int GetVAR_Category_ID() 
{
Object ii = Get_Value("VAR_Category_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Group.
@param VAR_Group_ID Request Group */
public void SetR_Group_ID (int VAR_Group_ID)
{
if (VAR_Group_ID <= 0) Set_Value ("VAR_Group_ID", null);
else
Set_Value ("VAR_Group_ID", VAR_Group_ID);
}
/** Get Group.
@return Request Group */
public int GetR_Group_ID() 
{
Object ii = Get_Value("VAR_Group_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Mail Template.
@param VAR_MailTemplate_ID Text templates for mailings */
public void SetVAR_MailTemplate_ID (int VAR_MailTemplate_ID)
{
if (VAR_MailTemplate_ID <= 0) Set_Value ("VAR_MailTemplate_ID", null);
else
Set_Value ("VAR_MailTemplate_ID", VAR_MailTemplate_ID);
}
/** Get Mail Template.
@return Text templates for mailings */
public int GetVAR_MailTemplate_ID() 
{
Object ii = Get_Value("VAR_MailTemplate_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAR_RequestRelated_ID VAF_Control_Ref_ID=341 */
public static int VAR_REQUESTRELATED_ID_VAF_Control_Ref_ID=341;
/** Set Related Request.
@param VAR_RequestRelated_ID Related Request (Master Issue, ..) */
public void SetVAR_RequestRelated_ID (int VAR_RequestRelated_ID)
{
if (VAR_RequestRelated_ID <= 0) Set_Value ("VAR_RequestRelated_ID", null);
else
Set_Value ("VAR_RequestRelated_ID", VAR_RequestRelated_ID);
}
/** Get Related Request.
@return Related Request (Master Issue, ..) */
public int GetVAR_RequestRelated_ID() 
{
Object ii = Get_Value("VAR_RequestRelated_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Request Type.
@param VAR_Req_Type_ID Type of request (e.g. Inquiry, Complaint, ..) */
public void SetVAR_Req_Type_ID (int VAR_Req_Type_ID)
{
if (VAR_Req_Type_ID < 1) throw new ArgumentException ("VAR_Req_Type_ID is mandatory.");
Set_Value ("VAR_Req_Type_ID", VAR_Req_Type_ID);
}
/** Get Request Type.
@return Type of request (e.g. Inquiry, Complaint, ..) */
public int GetVAR_Req_Type_ID() 
{
Object ii = Get_Value("VAR_Req_Type_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Request.
@param VAR_Request_ID Request from a Business Partner or Prospect */
public void SetVAR_Request_ID (int VAR_Request_ID)
{
if (VAR_Request_ID < 1) throw new ArgumentException ("VAR_Request_ID is mandatory.");
Set_ValueNoCheck ("VAR_Request_ID", VAR_Request_ID);
}
/** Get Request.
@return Request from a Business Partner or Prospect */
public int GetVAR_Request_ID() 
{
Object ii = Get_Value("VAR_Request_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Resolution.
@param VAR_Resolution_ID Request Resolution */
public void SetR_Resolution_ID (int VAR_Resolution_ID)
{
if (VAR_Resolution_ID <= 0) Set_Value ("VAR_Resolution_ID", null);
else
Set_Value ("VAR_Resolution_ID", VAR_Resolution_ID);
}
/** Get Resolution.
@return Request Resolution */
public int GetR_Resolution_ID() 
{
Object ii = Get_Value("VAR_Resolution_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Source.
@param VAR_Source_ID Source for the Lead or Request */
public void SetVAR_Source_ID (int VAR_Source_ID)
{
if (VAR_Source_ID <= 0) Set_Value ("VAR_Source_ID", null);
else
Set_Value ("VAR_Source_ID", VAR_Source_ID);
}
/** Get Source.
@return Source for the Lead or Request */
public int GetVAR_Source_ID() 
{
Object ii = Get_Value("VAR_Source_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Standard Response.
@param VAR_Req_StandardReply_ID Request Standard Response */
public void SetVAR_Req_StandardReply_ID (int VAR_Req_StandardReply_ID)
{
if (VAR_Req_StandardReply_ID <= 0) Set_Value ("VAR_Req_StandardReply_ID", null);
else
Set_Value ("VAR_Req_StandardReply_ID", VAR_Req_StandardReply_ID);
}
/** Get Standard Response.
@return Request Standard Response */
public int GetVAR_Req_StandardReply_ID() 
{
Object ii = Get_Value("VAR_Req_StandardReply_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Status.
@param VAR_Req_Status_ID Request Status */
public void SetVAR_Req_Status_ID (int VAR_Req_Status_ID)
{
if (VAR_Req_Status_ID <= 0) Set_Value ("VAR_Req_Status_ID", null);
else
Set_Value ("VAR_Req_Status_ID", VAR_Req_Status_ID);
}
/** Get Status.
@return Request Status */
public int GetVAR_Req_Status_ID() 
{
Object ii = Get_Value("VAR_Req_Status_ID");
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

/** SalesRep_ID VAF_Control_Ref_ID=286 */
public static int SALESREP_ID_VAF_Control_Ref_ID=286;
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

/** TaskStatus VAF_Control_Ref_ID=366 */
public static int TASKSTATUS_VAF_Control_Ref_ID=366;
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
