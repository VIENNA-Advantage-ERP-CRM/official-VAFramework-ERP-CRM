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
/** Generated Model for I_Request
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_I_Request : PO
{
public X_I_Request (Context ctx, int I_Request_ID, Trx trxName) : base (ctx, I_Request_ID, trxName)
{
/** if (I_Request_ID == 0)
{
SetI_IsImported (null);	// N
SetI_Request_ID (0);
}
 */
}
public X_I_Request (Ctx ctx, int I_Request_ID, Trx trxName) : base (ctx, I_Request_ID, trxName)
{
/** if (I_Request_ID == 0)
{
SetI_IsImported (null);	// N
SetI_Request_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_Request (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_Request (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_Request (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_I_Request()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514377657L;
/** Last Updated Timestamp 7/29/2010 1:07:40 PM */
public static long updatedMS = 1280389060868L;
/** AD_Table_ID=940 */
public static int Table_ID;
 // =940;

/** TableName=I_Request */
public static String Table_Name="I_Request";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(2);
/** AccessLevel
@return 2 - Client 
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
StringBuilder sb = new StringBuilder ("X_I_Request[").Append(Get_ID()).Append("]");
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
if (AD_Table_ID <= 0) Set_Value ("AD_Table_ID", null);
else
Set_Value ("AD_Table_ID", AD_Table_ID);
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
/** Set Activity Name.
@param ActivityName Activity Name */
public void SetActivityName (String ActivityName)
{
if (ActivityName != null && ActivityName.Length > 60)
{
log.Warning("Length > 60 - truncated");
ActivityName = ActivityName.Substring(0,60);
}
Set_Value ("ActivityName", ActivityName);
}
/** Get Activity Name.
@return Activity Name */
public String GetActivityName() 
{
return (String)Get_Value("ActivityName");
}
/** Set Activity Key.
@param ActivityValue Activity Key */
public void SetActivityValue (String ActivityValue)
{
if (ActivityValue != null && ActivityValue.Length > 40)
{
log.Warning("Length > 40 - truncated");
ActivityValue = ActivityValue.Substring(0,40);
}
Set_Value ("ActivityValue", ActivityValue);
}
/** Get Activity Key.
@return Activity Key */
public String GetActivityValue() 
{
return (String)Get_Value("ActivityValue");
}
/** Set Asset Name.
@param AssetName Asset Name */
public void SetAssetName (String AssetName)
{
if (AssetName != null && AssetName.Length > 60)
{
log.Warning("Length > 60 - truncated");
AssetName = AssetName.Substring(0,60);
}
Set_Value ("AssetName", AssetName);
}
/** Get Asset Name.
@return Asset Name */
public String GetAssetName() 
{
return (String)Get_Value("AssetName");
}
/** Set Asset Key.
@param AssetValue Asset Key */
public void SetAssetValue (String AssetValue)
{
if (AssetValue != null && AssetValue.Length > 40)
{
log.Warning("Length > 40 - truncated");
AssetValue = AssetValue.Substring(0,40);
}
Set_Value ("AssetValue", AssetValue);
}
/** Get Asset Key.
@return Asset Key */
public String GetAssetValue() 
{
return (String)Get_Value("AssetValue");
}
/** Set Business Partner Name.
@param BPartnerName Business Partner Name */
public void SetBPartnerName (String BPartnerName)
{
if (BPartnerName != null && BPartnerName.Length > 120)
{
log.Warning("Length > 120 - truncated");
BPartnerName = BPartnerName.Substring(0,120);
}
Set_Value ("BPartnerName", BPartnerName);
}
/** Get Business Partner Name.
@return Business Partner Name */
public String GetBPartnerName() 
{
return (String)Get_Value("BPartnerName");
}
/** Set BPartner (Agent) Name.
@param BPartnerSRName BPartner (Agent) Name */
public void SetBPartnerSRName (String BPartnerSRName)
{
if (BPartnerSRName != null && BPartnerSRName.Length > 120)
{
log.Warning("Length > 120 - truncated");
BPartnerSRName = BPartnerSRName.Substring(0,120);
}
Set_Value ("BPartnerSRName", BPartnerSRName);
}
/** Get BPartner (Agent) Name.
@return BPartner (Agent) Name */
public String GetBPartnerSRName() 
{
return (String)Get_Value("BPartnerSRName");
}
/** Set BPartner (Agent) Key.
@param BPartnerSRValue BPartner (Agent) Key */
public void SetBPartnerSRValue (String BPartnerSRValue)
{
if (BPartnerSRValue != null && BPartnerSRValue.Length > 80)
{
log.Warning("Length > 80 - truncated");
BPartnerSRValue = BPartnerSRValue.Substring(0,80);
}
Set_Value ("BPartnerSRValue", BPartnerSRValue);
}
/** Get BPartner (Agent) Key.
@return BPartner (Agent) Key */
public String GetBPartnerSRValue() 
{
return (String)Get_Value("BPartnerSRValue");
}
/** Set Business Partner Key.
@param BPartnerValue Key of the Business Partner */
public void SetBPartnerValue (String BPartnerValue)
{
if (BPartnerValue != null && BPartnerValue.Length > 80)
{
log.Warning("Length > 80 - truncated");
BPartnerValue = BPartnerValue.Substring(0,80);
}
Set_Value ("BPartnerValue", BPartnerValue);
}
/** Get Business Partner Key.
@return Key of the Business Partner */
public String GetBPartnerValue() 
{
return (String)Get_Value("BPartnerValue");
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
/** Set Campaign Name.
@param CampaignName Campaign Name */
public void SetCampaignName (String CampaignName)
{
if (CampaignName != null && CampaignName.Length > 60)
{
log.Warning("Length > 60 - truncated");
CampaignName = CampaignName.Substring(0,60);
}
Set_Value ("CampaignName", CampaignName);
}
/** Get Campaign Name.
@return Campaign Name */
public String GetCampaignName() 
{
return (String)Get_Value("CampaignName");
}
/** Set Campaign Key.
@param CampaignValue Campaign Key */
public void SetCampaignValue (String CampaignValue)
{
if (CampaignValue != null && CampaignValue.Length > 40)
{
log.Warning("Length > 40 - truncated");
CampaignValue = CampaignValue.Substring(0,40);
}
Set_Value ("CampaignValue", CampaignValue);
}
/** Get Campaign Key.
@return Campaign Key */
public String GetCampaignValue() 
{
return (String)Get_Value("CampaignValue");
}
/** Set Category Name.
@param CategoryName Name of the Category */
public void SetCategoryName (String CategoryName)
{
if (CategoryName != null && CategoryName.Length > 60)
{
log.Warning("Length > 60 - truncated");
CategoryName = CategoryName.Substring(0,60);
}
Set_Value ("CategoryName", CategoryName);
}
/** Get Category Name.
@return Name of the Category */
public String GetCategoryName() 
{
return (String)Get_Value("CategoryName");
}
/** Set Change Request Name.
@param ChangeRequestName Change Request Name */
public void SetChangeRequestName (String ChangeRequestName)
{
if (ChangeRequestName != null && ChangeRequestName.Length > 60)
{
log.Warning("Length > 60 - truncated");
ChangeRequestName = ChangeRequestName.Substring(0,60);
}
Set_Value ("ChangeRequestName", ChangeRequestName);
}
/** Get Change Request Name.
@return Change Request Name */
public String GetChangeRequestName() 
{
return (String)Get_Value("ChangeRequestName");
}
/** Set Tenant Name.
@param ClientName Tenant Name */
public void SetClientName (String ClientName)
{
if (ClientName != null && ClientName.Length > 60)
{
log.Warning("Length > 60 - truncated");
ClientName = ClientName.Substring(0,60);
}
Set_Value ("ClientName", ClientName);
}
/** Get Tenant Name.
@return Tenant Name */
public String GetClientName() 
{
return (String)Get_Value("ClientName");
}
/** Set Tenant Key.
@param ClientValue Key of the Tenant */
public void SetClientValue (String ClientValue)
{
if (ClientValue != null && ClientValue.Length > 40)
{
log.Warning("Length > 40 - truncated");
ClientValue = ClientValue.Substring(0,40);
}
Set_Value ("ClientValue", ClientValue);
}
/** Get Tenant Key.
@return Key of the Tenant */
public String GetClientValue() 
{
return (String)Get_Value("ClientValue");
}
/** Set Close Date.
@param CloseDate Close Date */
public void SetCloseDate (DateTime? CloseDate)
{
Set_ValueNoCheck ("CloseDate", (DateTime?)CloseDate);
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
return test == null || test.Equals("A") || test.Equals("C") || test.Equals("I") || test.Equals("P");
}
/** Set Entry Access Level.
@param ConfidentialTypeEntry Confidentiality of the individual entry */
public void SetConfidentialTypeEntry (String ConfidentialTypeEntry)
{
if (!IsConfidentialTypeEntryValid(ConfidentialTypeEntry))
throw new ArgumentException ("ConfidentialTypeEntry Invalid value - " + ConfidentialTypeEntry + " - Reference_ID=340 - A - C - I - P");
if (ConfidentialTypeEntry != null && ConfidentialTypeEntry.Length > 1)
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
/** Set Contact Name.
@param ContactName Business Partner Contact Name */
public void SetContactName (String ContactName)
{
if (ContactName != null && ContactName.Length > 120)
{
log.Warning("Length > 120 - truncated");
ContactName = ContactName.Substring(0,120);
}
Set_Value ("ContactName", ContactName);
}
/** Get Contact Name.
@return Business Partner Contact Name */
public String GetContactName() 
{
return (String)Get_Value("ContactName");
}
/** Set Contact Key.
@param ContactValue Key of the Contact */
public void SetContactValue (String ContactValue)
{
if (ContactValue != null && ContactValue.Length > 80)
{
log.Warning("Length > 80 - truncated");
ContactValue = ContactValue.Substring(0,80);
}
Set_Value ("ContactValue", ContactValue);
}
/** Get Contact Key.
@return Key of the Contact */
public String GetContactValue() 
{
return (String)Get_Value("ContactValue");
}
/** Set Complete Plan.
@param DateCompletePlan Planned Completion Date */
public void SetDateCompletePlan (DateTime? DateCompletePlan)
{
Set_ValueNoCheck ("DateCompletePlan", (DateTime?)DateCompletePlan);
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
Set_ValueNoCheck ("DateLastAlert", (DateTime?)DateLastAlert);
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
Set_ValueNoCheck ("DateStartPlan", (DateTime?)DateStartPlan);
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
if (DocumentNo != null && DocumentNo.Length > 30)
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
return test == null || test.Equals("3") || test.Equals("5") || test.Equals("7");
}
/** Set Aging Status.
@param DueType Status of the next action for this Request */
public void SetDueType (String DueType)
{
if (!IsDueTypeValid(DueType))
throw new ArgumentException ("DueType Invalid value - " + DueType + " - Reference_ID=222 - 3 - 5 - 7");
if (DueType != null && DueType.Length > 1)
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
Set_ValueNoCheck ("EndTime", (DateTime?)EndTime);
}
/** Get End Time.
@return End of the time span */
public DateTime? GetEndTime() 
{
return (DateTime?)Get_Value("EndTime");
}
/** Set Group Name.
@param GroupName Group Name */
public void SetGroupName (String GroupName)
{
if (GroupName != null && GroupName.Length > 60)
{
log.Warning("Length > 60 - truncated");
GroupName = GroupName.Substring(0,60);
}
Set_Value ("GroupName", GroupName);
}
/** Get Group Name.
@return Group Name */
public String GetGroupName() 
{
return (String)Get_Value("GroupName");
}
/** Set Import Error Message.
@param I_ErrorMsg Messages generated from import process */
public void SetI_ErrorMsg (String I_ErrorMsg)
{
if (I_ErrorMsg != null && I_ErrorMsg.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
I_ErrorMsg = I_ErrorMsg.Substring(0,2000);
}
Set_Value ("I_ErrorMsg", I_ErrorMsg);
}
/** Get Import Error Message.
@return Messages generated from import process */
public String GetI_ErrorMsg() 
{
return (String)Get_Value("I_ErrorMsg");
}

/** I_IsImported AD_Reference_ID=420 */
public static int I_ISIMPORTED_AD_Reference_ID=420;
/** Error = E */
public static String I_ISIMPORTED_Error = "E";
/** No = N */
public static String I_ISIMPORTED_No = "N";
/** Yes = Y */
public static String I_ISIMPORTED_Yes = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsI_IsImportedValid (String test)
{
return test.Equals("E") || test.Equals("N") || test.Equals("Y");
}
/** Set Imported.
@param I_IsImported Has this import been processed */
public void SetI_IsImported (String I_IsImported)
{
if (I_IsImported == null) throw new ArgumentException ("I_IsImported is mandatory");
if (!IsI_IsImportedValid(I_IsImported))
throw new ArgumentException ("I_IsImported Invalid value - " + I_IsImported + " - Reference_ID=420 - E - N - Y");
if (I_IsImported.Length > 1)
{
log.Warning("Length > 1 - truncated");
I_IsImported = I_IsImported.Substring(0,1);
}
Set_Value ("I_IsImported", I_IsImported);
}
/** Get Imported.
@return Has this import been processed */
public String GetI_IsImported() 
{
return (String)Get_Value("I_IsImported");
}
/** Set I_Request_ID.
@param I_Request_ID I_Request_ID */
public void SetI_Request_ID (int I_Request_ID)
{
if (I_Request_ID < 1) throw new ArgumentException ("I_Request_ID is mandatory.");
Set_ValueNoCheck ("I_Request_ID", I_Request_ID);
}
/** Get I_Request_ID.
@return I_Request_ID */
public int GetI_Request_ID() 
{
Object ii = Get_Value("I_Request_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Shipment/Receipt Document No.
@param InOutDocumentNo Shipment/Receipt Document No */
public void SetInOutDocumentNo (String InOutDocumentNo)
{
if (InOutDocumentNo != null && InOutDocumentNo.Length > 30)
{
log.Warning("Length > 30 - truncated");
InOutDocumentNo = InOutDocumentNo.Substring(0,30);
}
Set_Value ("InOutDocumentNo", InOutDocumentNo);
}
/** Get Shipment/Receipt Document No.
@return Shipment/Receipt Document No */
public String GetInOutDocumentNo() 
{
return (String)Get_Value("InOutDocumentNo");
}
/** Set Invoice Document No.
@param InvoiceDocumentNo Document Number of the Invoice */
public void SetInvoiceDocumentNo (String InvoiceDocumentNo)
{
if (InvoiceDocumentNo != null && InvoiceDocumentNo.Length > 30)
{
log.Warning("Length > 30 - truncated");
InvoiceDocumentNo = InvoiceDocumentNo.Substring(0,30);
}
Set_Value ("InvoiceDocumentNo", InvoiceDocumentNo);
}
/** Get Invoice Document No.
@return Document Number of the Invoice */
public String GetInvoiceDocumentNo() 
{
return (String)Get_Value("InvoiceDocumentNo");
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
Set_Value ("IsSelfService", IsSelfService);
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
/** Set Lead Document No.
@param LeadDocumentNo Lead Document No */
public void SetLeadDocumentNo (String LeadDocumentNo)
{
if (LeadDocumentNo != null && LeadDocumentNo.Length > 30)
{
log.Warning("Length > 30 - truncated");
LeadDocumentNo = LeadDocumentNo.Substring(0,30);
}
Set_Value ("LeadDocumentNo", LeadDocumentNo);
}
/** Get Lead Document No.
@return Lead Document No */
public String GetLeadDocumentNo() 
{
return (String)Get_Value("LeadDocumentNo");
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
/** Set Order Document No.
@param OrderDocumentNo Order Document No */
public void SetOrderDocumentNo (String OrderDocumentNo)
{
if (OrderDocumentNo != null && OrderDocumentNo.Length > 30)
{
log.Warning("Length > 30 - truncated");
OrderDocumentNo = OrderDocumentNo.Substring(0,30);
}
Set_Value ("OrderDocumentNo", OrderDocumentNo);
}
/** Get Order Document No.
@return Order Document No */
public String GetOrderDocumentNo() 
{
return (String)Get_Value("OrderDocumentNo");
}
/** Set Organization Name.
@param OrgName Name of the Organization */
public void SetOrgName (String OrgName)
{
if (OrgName != null && OrgName.Length > 60)
{
log.Warning("Length > 60 - truncated");
OrgName = OrgName.Substring(0,60);
}
Set_Value ("OrgName", OrgName);
}
/** Get Organization Name.
@return Name of the Organization */
public String GetOrgName() 
{
return (String)Get_Value("OrgName");
}
/** Set Organization Key.
@param OrgValue Key of the Organization */
public void SetOrgValue (String OrgValue)
{
if (OrgValue != null && OrgValue.Length > 40)
{
log.Warning("Length > 40 - truncated");
OrgValue = OrgValue.Substring(0,40);
}
Set_Value ("OrgValue", OrgValue);
}
/** Get Organization Key.
@return Key of the Organization */
public String GetOrgValue() 
{
return (String)Get_Value("OrgValue");
}
/** Set Payment Document No.
@param PaymentDocumentNo Document number of the Payment */
public void SetPaymentDocumentNo (String PaymentDocumentNo)
{
if (PaymentDocumentNo != null && PaymentDocumentNo.Length > 30)
{
log.Warning("Length > 30 - truncated");
PaymentDocumentNo = PaymentDocumentNo.Substring(0,30);
}
Set_Value ("PaymentDocumentNo", PaymentDocumentNo);
}
/** Get Payment Document No.
@return Document number of the Payment */
public String GetPaymentDocumentNo() 
{
return (String)Get_Value("PaymentDocumentNo");
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
/** Set Process Now.
@param Processing Process Now */
public void SetProcessing (Boolean Processing)
{
Set_Value ("Processing", Processing);
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
/** Set Product Name.
@param ProductName Name of the Product */
public void SetProductName (String ProductName)
{
if (ProductName != null && ProductName.Length > 60)
{
log.Warning("Length > 60 - truncated");
ProductName = ProductName.Substring(0,60);
}
Set_Value ("ProductName", ProductName);
}
/** Get Product Name.
@return Name of the Product */
public String GetProductName() 
{
return (String)Get_Value("ProductName");
}
/** Set Product Used Name.
@param ProductSpentName Product Used Name */
public void SetProductSpentName (String ProductSpentName)
{
if (ProductSpentName != null && ProductSpentName.Length > 60)
{
log.Warning("Length > 60 - truncated");
ProductSpentName = ProductSpentName.Substring(0,60);
}
Set_Value ("ProductSpentName", ProductSpentName);
}
/** Get Product Used Name.
@return Product Used Name */
public String GetProductSpentName() 
{
return (String)Get_Value("ProductSpentName");
}
/** Set Product Used Key.
@param ProductSpentValue Product Used Key */
public void SetProductSpentValue (String ProductSpentValue)
{
if (ProductSpentValue != null && ProductSpentValue.Length > 40)
{
log.Warning("Length > 40 - truncated");
ProductSpentValue = ProductSpentValue.Substring(0,40);
}
Set_Value ("ProductSpentValue", ProductSpentValue);
}
/** Get Product Used Key.
@return Product Used Key */
public String GetProductSpentValue() 
{
return (String)Get_Value("ProductSpentValue");
}
/** Set Product Key.
@param ProductValue Key of the Product */
public void SetProductValue (String ProductValue)
{
if (ProductValue != null && ProductValue.Length > 40)
{
log.Warning("Length > 40 - truncated");
ProductValue = ProductValue.Substring(0,40);
}
Set_Value ("ProductValue", ProductValue);
}
/** Get Product Key.
@return Key of the Product */
public String GetProductValue() 
{
return (String)Get_Value("ProductValue");
}
/** Set Project Name.
@param ProjectName Name of the Project */
public void SetProjectName (String ProjectName)
{
if (ProjectName != null && ProjectName.Length > 60)
{
log.Warning("Length > 60 - truncated");
ProjectName = ProjectName.Substring(0,60);
}
Set_Value ("ProjectName", ProjectName);
}
/** Get Project Name.
@return Name of the Project */
public String GetProjectName() 
{
return (String)Get_Value("ProjectName");
}
/** Set Project Key.
@param ProjectValue Key of the Project */
public void SetProjectValue (String ProjectValue)
{
if (ProjectValue != null && ProjectValue.Length > 40)
{
log.Warning("Length > 40 - truncated");
ProjectValue = ProjectValue.Substring(0,40);
}
Set_Value ("ProjectValue", ProjectValue);
}
/** Get Project Key.
@return Key of the Project */
public String GetProjectValue() 
{
return (String)Get_Value("ProjectValue");
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
if (R_RequestType_ID <= 0) Set_Value ("R_RequestType_ID", null);
else
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
if (R_Request_ID <= 0) Set_Value ("R_Request_ID", null);
else
Set_Value ("R_Request_ID", R_Request_ID);
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
/** Set Request Type Name.
@param ReqTypeName Request Type Name */
public void SetReqTypeName (String ReqTypeName)
{
if (ReqTypeName != null && ReqTypeName.Length > 120)
{
log.Warning("Length > 120 - truncated");
ReqTypeName = ReqTypeName.Substring(0,120);
}
Set_Value ("ReqTypeName", ReqTypeName);
}
/** Get Request Type Name.
@return Request Type Name */
public String GetReqTypeName() 
{
return (String)Get_Value("ReqTypeName");
}
/** Set Request Amount.
@param RequestAmt Amount associated with this request */
public void SetRequestAmt (Decimal? RequestAmt)
{
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
/** Set Request Related Doc No.
@param RequestRelatedDocNo Request Related Doc No */
public void SetRequestRelatedDocNo (String RequestRelatedDocNo)
{
if (RequestRelatedDocNo != null && RequestRelatedDocNo.Length > 30)
{
log.Warning("Length > 30 - truncated");
RequestRelatedDocNo = RequestRelatedDocNo.Substring(0,30);
}
Set_Value ("RequestRelatedDocNo", RequestRelatedDocNo);
}
/** Get Request Related Doc No.
@return Request Related Doc No */
public String GetRequestRelatedDocNo() 
{
return (String)Get_Value("RequestRelatedDocNo");
}
/** Set Resolution Name.
@param ResolutionName Resolution Name */
public void SetResolutionName (String ResolutionName)
{
if (ResolutionName != null && ResolutionName.Length > 60)
{
log.Warning("Length > 60 - truncated");
ResolutionName = ResolutionName.Substring(0,60);
}
Set_Value ("ResolutionName", ResolutionName);
}
/** Get Resolution Name.
@return Resolution Name */
public String GetResolutionName() 
{
return (String)Get_Value("ResolutionName");
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
/** Set Role Name.
@param RoleName Role Name */
public void SetRoleName (String RoleName)
{
if (RoleName != null && RoleName.Length > 60)
{
log.Warning("Length > 60 - truncated");
RoleName = RoleName.Substring(0,60);
}
Set_Value ("RoleName", RoleName);
}
/** Get Role Name.
@return Role Name */
public String GetRoleName() 
{
return (String)Get_Value("RoleName");
}
/** Set Sales Region Name.
@param SalesRegionName Sales Region Name */
public void SetSalesRegionName (String SalesRegionName)
{
if (SalesRegionName != null && SalesRegionName.Length > 60)
{
log.Warning("Length > 60 - truncated");
SalesRegionName = SalesRegionName.Substring(0,60);
}
Set_Value ("SalesRegionName", SalesRegionName);
}
/** Get Sales Region Name.
@return Sales Region Name */
public String GetSalesRegionName() 
{
return (String)Get_Value("SalesRegionName");
}
/** Set Sales Region Key.
@param SalesRegionValue Sales Region Key */
public void SetSalesRegionValue (String SalesRegionValue)
{
if (SalesRegionValue != null && SalesRegionValue.Length > 40)
{
log.Warning("Length > 40 - truncated");
SalesRegionValue = SalesRegionValue.Substring(0,40);
}
Set_Value ("SalesRegionValue", SalesRegionValue);
}
/** Get Sales Region Key.
@return Sales Region Key */
public String GetSalesRegionValue() 
{
return (String)Get_Value("SalesRegionValue");
}
/** Set Representative Name.
@param SalesRepName Representative Name */
public void SetSalesRepName (String SalesRepName)
{
if (SalesRepName != null && SalesRepName.Length > 120)
{
log.Warning("Length > 120 - truncated");
SalesRepName = SalesRepName.Substring(0,120);
}
Set_Value ("SalesRepName", SalesRepName);
}
/** Get Representative Name.
@return Representative Name */
public String GetSalesRepName() 
{
return (String)Get_Value("SalesRepName");
}
/** Set Representative Key.
@param SalesRepValue Representative Key */
public void SetSalesRepValue (String SalesRepValue)
{
if (SalesRepValue != null && SalesRepValue.Length > 80)
{
log.Warning("Length > 80 - truncated");
SalesRepValue = SalesRepValue.Substring(0,80);
}
Set_Value ("SalesRepValue", SalesRepValue);
}
/** Get Representative Key.
@return Representative Key */
public String GetSalesRepValue() 
{
return (String)Get_Value("SalesRepValue");
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
/** Set Source Name.
@param SourceName Source Name */
public void SetSourceName (String SourceName)
{
if (SourceName != null && SourceName.Length > 120)
{
log.Warning("Length > 120 - truncated");
SourceName = SourceName.Substring(0,120);
}
Set_Value ("SourceName", SourceName);
}
/** Get Source Name.
@return Source Name */
public String GetSourceName() 
{
return (String)Get_Value("SourceName");
}
/** Set Source Key.
@param SourceValue Source Key */
public void SetSourceValue (String SourceValue)
{
if (SourceValue != null && SourceValue.Length > 40)
{
log.Warning("Length > 40 - truncated");
SourceValue = SourceValue.Substring(0,40);
}
Set_Value ("SourceValue", SourceValue);
}
/** Get Source Key.
@return Source Key */
public String GetSourceValue() 
{
return (String)Get_Value("SourceValue");
}
/** Set Start Date.
@param StartDate First effective day (inclusive) */
public void SetStartDate (DateTime? StartDate)
{
Set_ValueNoCheck ("StartDate", (DateTime?)StartDate);
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
Set_ValueNoCheck ("StartTime", (DateTime?)StartTime);
}
/** Get Start Time.
@return Time started */
public DateTime? GetStartTime() 
{
return (DateTime?)Get_Value("StartTime");
}
/** Set Status Name.
@param StatusName Status Name */
public void SetStatusName (String StatusName)
{
if (StatusName != null && StatusName.Length > 60)
{
log.Warning("Length > 60 - truncated");
StatusName = StatusName.Substring(0,60);
}
Set_Value ("StatusName", StatusName);
}
/** Get Status Name.
@return Status Name */
public String GetStatusName() 
{
return (String)Get_Value("StatusName");
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
Set_Value ("Summary", Summary);
}
/** Get Summary.
@return Textual summary of this request */
public String GetSummary() 
{
return (String)Get_Value("Summary");
}
/** Set DB Table Name.
@param TableName Name of the table in the database */
public void SetTableName (String TableName)
{
if (TableName != null && TableName.Length > 60)
{
log.Warning("Length > 60 - truncated");
TableName = TableName.Substring(0,60);
}
Set_Value ("TableName", TableName);
}
/** Get DB Table Name.
@return Name of the table in the database */
public String GetTableName() 
{
return (String)Get_Value("TableName");
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
