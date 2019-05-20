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
/** Generated Model for R_RequestAction
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_R_RequestAction : PO
{
public X_R_RequestAction (Context ctx, int R_RequestAction_ID, Trx trxName) : base (ctx, R_RequestAction_ID, trxName)
{
/** if (R_RequestAction_ID == 0)
{
SetR_RequestAction_ID (0);
SetR_Request_ID (0);
}
 */
}
public X_R_RequestAction (Ctx ctx, int R_RequestAction_ID, Trx trxName) : base (ctx, R_RequestAction_ID, trxName)
{
/** if (R_RequestAction_ID == 0)
{
SetR_RequestAction_ID (0);
SetR_Request_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_RequestAction (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_RequestAction (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_RequestAction (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_R_RequestAction()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514383158L;
/** Last Updated Timestamp 7/29/2010 1:07:46 PM */
public static long updatedMS = 1280389066369L;
/** AD_Table_ID=418 */
public static int Table_ID;
 // =418;

/** TableName=R_RequestAction */
public static String Table_Name="R_RequestAction";

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
StringBuilder sb = new StringBuilder ("X_R_RequestAction[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Role.
@param AD_Role_ID Responsibility Role */
public void SetAD_Role_ID (int AD_Role_ID)
{
if (AD_Role_ID <= 0) Set_ValueNoCheck ("AD_Role_ID", null);
else
Set_ValueNoCheck ("AD_Role_ID", AD_Role_ID);
}
/** Get Role.
@return Responsibility Role */
public int GetAD_Role_ID() 
{
Object ii = Get_Value("AD_Role_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Business Partner Contact */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID <= 0) Set_ValueNoCheck ("AD_User_ID", null);
else
Set_ValueNoCheck ("AD_User_ID", AD_User_ID);
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
if (A_Asset_ID <= 0) Set_ValueNoCheck ("A_Asset_ID", null);
else
Set_ValueNoCheck ("A_Asset_ID", A_Asset_ID);
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
if (C_Activity_ID <= 0) Set_ValueNoCheck ("C_Activity_ID", null);
else
Set_ValueNoCheck ("C_Activity_ID", C_Activity_ID);
}
/** Get Activity.
@return Business Activity */
public int GetC_Activity_ID() 
{
Object ii = Get_Value("C_Activity_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Business Partner.
@param C_BPartner_ID Identifies a Business Partner */
public void SetC_BPartner_ID (int C_BPartner_ID)
{
if (C_BPartner_ID <= 0) Set_ValueNoCheck ("C_BPartner_ID", null);
else
Set_ValueNoCheck ("C_BPartner_ID", C_BPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetC_BPartner_ID() 
{
Object ii = Get_Value("C_BPartner_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Invoice.
@param C_Invoice_ID Invoice Identifier */
public void SetC_Invoice_ID (int C_Invoice_ID)
{
if (C_Invoice_ID <= 0) Set_ValueNoCheck ("C_Invoice_ID", null);
else
Set_ValueNoCheck ("C_Invoice_ID", C_Invoice_ID);
}
/** Get Invoice.
@return Invoice Identifier */
public int GetC_Invoice_ID() 
{
Object ii = Get_Value("C_Invoice_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Order.
@param C_Order_ID Order */
public void SetC_Order_ID (int C_Order_ID)
{
if (C_Order_ID <= 0) Set_ValueNoCheck ("C_Order_ID", null);
else
Set_ValueNoCheck ("C_Order_ID", C_Order_ID);
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
if (C_Payment_ID <= 0) Set_ValueNoCheck ("C_Payment_ID", null);
else
Set_ValueNoCheck ("C_Payment_ID", C_Payment_ID);
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
if (C_Project_ID <= 0) Set_ValueNoCheck ("C_Project_ID", null);
else
Set_ValueNoCheck ("C_Project_ID", C_Project_ID);
}
/** Get Project.
@return Financial Project */
public int GetC_Project_ID() 
{
Object ii = Get_Value("C_Project_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
return test == null || test.Equals("A") || test.Equals("C") || test.Equals("I") || test.Equals("P");
}
/** Set Confidentiality.
@param ConfidentialType Type of Confidentiality */
public void SetConfidentialType (String ConfidentialType)
{
if (!IsConfidentialTypeValid(ConfidentialType))
throw new ArgumentException ("ConfidentialType Invalid value - " + ConfidentialType + " - Reference_ID=340 - A - C - I - P");
if (ConfidentialType != null && ConfidentialType.Length > 1)
{
log.Warning("Length > 1 - truncated");
ConfidentialType = ConfidentialType.Substring(0,1);
}
Set_ValueNoCheck ("ConfidentialType", ConfidentialType);
}
/** Get Confidentiality.
@return Type of Confidentiality */
public String GetConfidentialType() 
{
return (String)Get_Value("ConfidentialType");
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
/** Set Date next action.
@param DateNextAction Date that this request should be acted on */
public void SetDateNextAction (DateTime? DateNextAction)
{
Set_ValueNoCheck ("DateNextAction", (DateTime?)DateNextAction);
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
/** Set End Date.
@param EndDate Last effective date (inclusive) */
public void SetEndDate (DateTime? EndDate)
{
Set_Value ("EndDate", (DateTime?)EndDate);
}
/** Get End Date.
@return Last effective date (inclusive) */
public DateTime? GetEndDate() 
{
return (DateTime?)Get_Value("EndDate");
}

/** IsEscalated AD_Reference_ID=319 */
public static int ISESCALATED_AD_Reference_ID=319;
/** No = N */
public static String ISESCALATED_No = "N";
/** Yes = Y */
public static String ISESCALATED_Yes = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsIsEscalatedValid (String test)
{
return test == null || test.Equals("N") || test.Equals("Y");
}
/** Set Escalated.
@param IsEscalated This request has been escalated */
public void SetIsEscalated (String IsEscalated)
{
if (!IsIsEscalatedValid(IsEscalated))
throw new ArgumentException ("IsEscalated Invalid value - " + IsEscalated + " - Reference_ID=319 - N - Y");
if (IsEscalated != null && IsEscalated.Length > 1)
{
log.Warning("Length > 1 - truncated");
IsEscalated = IsEscalated.Substring(0,1);
}
Set_ValueNoCheck ("IsEscalated", IsEscalated);
}
/** Get Escalated.
@return This request has been escalated */
public String GetIsEscalated() 
{
return (String)Get_Value("IsEscalated");
}

/** IsInvoiced AD_Reference_ID=319 */
public static int ISINVOICED_AD_Reference_ID=319;
/** No = N */
public static String ISINVOICED_No = "N";
/** Yes = Y */
public static String ISINVOICED_Yes = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsIsInvoicedValid (String test)
{
return test == null || test.Equals("N") || test.Equals("Y");
}
/** Set Invoiced.
@param IsInvoiced Is this invoiced? */
public void SetIsInvoiced (String IsInvoiced)
{
if (!IsIsInvoicedValid(IsInvoiced))
throw new ArgumentException ("IsInvoiced Invalid value - " + IsInvoiced + " - Reference_ID=319 - N - Y");
if (IsInvoiced != null && IsInvoiced.Length > 1)
{
log.Warning("Length > 1 - truncated");
IsInvoiced = IsInvoiced.Substring(0,1);
}
Set_ValueNoCheck ("IsInvoiced", IsInvoiced);
}
/** Get Invoiced.
@return Is this invoiced? */
public String GetIsInvoiced() 
{
return (String)Get_Value("IsInvoiced");
}

/** IsSelfService AD_Reference_ID=319 */
public static int ISSELFSERVICE_AD_Reference_ID=319;
/** No = N */
public static String ISSELFSERVICE_No = "N";
/** Yes = Y */
public static String ISSELFSERVICE_Yes = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsIsSelfServiceValid (String test)
{
return test == null || test.Equals("N") || test.Equals("Y");
}
/** Set Self-Service.
@param IsSelfService This is a Self-Service entry or this entry can be changed via Self-Service */
public void SetIsSelfService (String IsSelfService)
{
if (!IsIsSelfServiceValid(IsSelfService))
throw new ArgumentException ("IsSelfService Invalid value - " + IsSelfService + " - Reference_ID=319 - N - Y");
if (IsSelfService != null && IsSelfService.Length > 1)
{
log.Warning("Length > 1 - truncated");
IsSelfService = IsSelfService.Substring(0,1);
}
Set_ValueNoCheck ("IsSelfService", IsSelfService);
}
/** Get Self-Service.
@return This is a Self-Service entry or this entry can be changed via Self-Service */
public String GetIsSelfService() 
{
return (String)Get_Value("IsSelfService");
}
/** Set Shipment/Receipt.
@param M_InOut_ID Material Shipment Document */
public void SetM_InOut_ID (int M_InOut_ID)
{
if (M_InOut_ID <= 0) Set_ValueNoCheck ("M_InOut_ID", null);
else
Set_ValueNoCheck ("M_InOut_ID", M_InOut_ID);
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
if (M_Product_ID <= 0) Set_ValueNoCheck ("M_Product_ID", null);
else
Set_ValueNoCheck ("M_Product_ID", M_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() 
{
Object ii = Get_Value("M_Product_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Null Columns.
@param NullColumns Columns with NULL value */
public void SetNullColumns (String NullColumns)
{
if (NullColumns != null && NullColumns.Length > 255)
{
log.Warning("Length > 255 - truncated");
NullColumns = NullColumns.Substring(0,255);
}
Set_ValueNoCheck ("NullColumns", NullColumns);
}
/** Get Null Columns.
@return Columns with NULL value */
public String GetNullColumns() 
{
return (String)Get_Value("NullColumns");
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
return test == null || test.Equals("1") || test.Equals("3") || test.Equals("5") || test.Equals("7") || test.Equals("9");
}
/** Set Priority.
@param Priority Indicates if this request is of a high, medium or low priority. */
public void SetPriority (String Priority)
{
if (!IsPriorityValid(Priority))
throw new ArgumentException ("Priority Invalid value - " + Priority + " - Reference_ID=154 - 1 - 3 - 5 - 7 - 9");
if (Priority != null && Priority.Length > 1)
{
log.Warning("Length > 1 - truncated");
Priority = Priority.Substring(0,1);
}
Set_ValueNoCheck ("Priority", Priority);
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
Set_ValueNoCheck ("PriorityUser", PriorityUser);
}
/** Get User Priority.
@return Priority of the issue for the User */
public String GetPriorityUser() 
{
return (String)Get_Value("PriorityUser");
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
if (R_Category_ID <= 0) Set_ValueNoCheck ("R_Category_ID", null);
else
Set_ValueNoCheck ("R_Category_ID", R_Category_ID);
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
if (R_Group_ID <= 0) Set_ValueNoCheck ("R_Group_ID", null);
else
Set_ValueNoCheck ("R_Group_ID", R_Group_ID);
}
/** Get Group.
@return Request Group */
public int GetR_Group_ID() 
{
Object ii = Get_Value("R_Group_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Request History.
@param R_RequestAction_ID Request has been changed */
public void SetR_RequestAction_ID (int R_RequestAction_ID)
{
if (R_RequestAction_ID < 1) throw new ArgumentException ("R_RequestAction_ID is mandatory.");
Set_ValueNoCheck ("R_RequestAction_ID", R_RequestAction_ID);
}
/** Get Request History.
@return Request has been changed */
public int GetR_RequestAction_ID() 
{
Object ii = Get_Value("R_RequestAction_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Request Type.
@param R_RequestType_ID Type of request (e.g. Inquiry, Complaint, ..) */
public void SetR_RequestType_ID (int R_RequestType_ID)
{
if (R_RequestType_ID <= 0) Set_ValueNoCheck ("R_RequestType_ID", null);
else
Set_ValueNoCheck ("R_RequestType_ID", R_RequestType_ID);
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
if (R_Resolution_ID <= 0) Set_ValueNoCheck ("R_Resolution_ID", null);
else
Set_ValueNoCheck ("R_Resolution_ID", R_Resolution_ID);
}
/** Get Resolution.
@return Request Resolution */
public int GetR_Resolution_ID() 
{
Object ii = Get_Value("R_Resolution_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Status.
@param R_Status_ID Request Status */
public void SetR_Status_ID (int R_Status_ID)
{
if (R_Status_ID <= 0) Set_ValueNoCheck ("R_Status_ID", null);
else
Set_ValueNoCheck ("R_Status_ID", R_Status_ID);
}
/** Get Status.
@return Request Status */
public int GetR_Status_ID() 
{
Object ii = Get_Value("R_Status_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** SalesRep_ID AD_Reference_ID=110 */
public static int SALESREP_ID_AD_Reference_ID=110;
/** Set Representative.
@param SalesRep_ID Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
public void SetSalesRep_ID (int SalesRep_ID)
{
if (SalesRep_ID <= 0) Set_ValueNoCheck ("SalesRep_ID", null);
else
Set_ValueNoCheck ("SalesRep_ID", SalesRep_ID);
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
/** Set Summary.
@param Summary Textual summary of this request */
public void SetSummary (String Summary)
{
if (Summary != null && Summary.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Summary = Summary.Substring(0,2000);
}
Set_ValueNoCheck ("Summary", Summary);
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
