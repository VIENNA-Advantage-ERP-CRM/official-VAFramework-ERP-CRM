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
/** Generated Model for C_Lead
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_Lead : PO
{
public X_C_Lead (Context ctx, int C_Lead_ID, Trx trxName) : base (ctx, C_Lead_ID, trxName)
{
/** if (C_Lead_ID == 0)
{
SetC_Lead_ID (0);
SetDocumentNo (null);
}
 */
}
public X_C_Lead (Ctx ctx, int C_Lead_ID, Trx trxName) : base (ctx, C_Lead_ID, trxName)
{
/** if (C_Lead_ID == 0)
{
SetC_Lead_ID (0);
SetDocumentNo (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Lead (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Lead (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Lead (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_Lead()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514372971L;
/** Last Updated Timestamp 7/29/2010 1:07:36 PM */
public static long updatedMS = 1280389056182L;
/** AD_Table_ID=923 */
public static int Table_ID;
 // =923;

/** TableName=C_Lead */
public static String Table_Name="C_Lead";

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
StringBuilder sb = new StringBuilder ("X_C_Lead[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Address 1.
@param Address1 Address line 1 for this location */
public void SetAddress1 (String Address1)
{
if (Address1 != null && Address1.Length > 60)
{
log.Warning("Length > 60 - truncated");
Address1 = Address1.Substring(0,60);
}
Set_Value ("Address1", Address1);
}
/** Get Address 1.
@return Address line 1 for this location */
public String GetAddress1() 
{
return (String)Get_Value("Address1");
}
/** Set Address 2.
@param Address2 Address line 2 for this location */
public void SetAddress2 (String Address2)
{
if (Address2 != null && Address2.Length > 60)
{
log.Warning("Length > 60 - truncated");
Address2 = Address2.Substring(0,60);
}
Set_Value ("Address2", Address2);
}
/** Get Address 2.
@return Address line 2 for this location */
public String GetAddress2() 
{
return (String)Get_Value("Address2");
}
/** Set Partner Name.
@param BPName Account or Business Partner Name */
public void SetBPName (String BPName)
{
if (BPName != null && BPName.Length > 60)
{
log.Warning("Length > 60 - truncated");
BPName = BPName.Substring(0,60);
}
Set_Value ("BPName", BPName);
}
/** Get Partner Name.
@return Account or Business Partner Name */
public String GetBPName() 
{
return (String)Get_Value("BPName");
}
/** Set Business Partner Group.
@param C_BP_Group_ID Business Partner Group */
public void SetC_BP_Group_ID (int C_BP_Group_ID)
{
if (C_BP_Group_ID <= 0) Set_Value ("C_BP_Group_ID", null);
else
Set_Value ("C_BP_Group_ID", C_BP_Group_ID);
}
/** Get Business Partner Group.
@return Business Partner Group */
public int GetC_BP_Group_ID() 
{
Object ii = Get_Value("C_BP_Group_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set BP Size.
@param C_BP_Size_ID Business Partner Size */
public void SetC_BP_Size_ID (int C_BP_Size_ID)
{
if (C_BP_Size_ID <= 0) Set_Value ("C_BP_Size_ID", null);
else
Set_Value ("C_BP_Size_ID", C_BP_Size_ID);
}
/** Get BP Size.
@return Business Partner Size */
public int GetC_BP_Size_ID() 
{
Object ii = Get_Value("C_BP_Size_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set BP Status.
@param C_BP_Status_ID Business Partner Status */
public void SetC_BP_Status_ID (int C_BP_Status_ID)
{
if (C_BP_Status_ID <= 0) Set_Value ("C_BP_Status_ID", null);
else
Set_Value ("C_BP_Status_ID", C_BP_Status_ID);
}
/** Get BP Status.
@return Business Partner Status */
public int GetC_BP_Status_ID() 
{
Object ii = Get_Value("C_BP_Status_ID");
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
/** Set Partner Location.
@param C_BPartner_Location_ID Identifies the (ship to) address for this Business Partner */
public void SetC_BPartner_Location_ID (int C_BPartner_Location_ID)
{
if (C_BPartner_Location_ID <= 0) Set_Value ("C_BPartner_Location_ID", null);
else
Set_Value ("C_BPartner_Location_ID", C_BPartner_Location_ID);
}
/** Get Partner Location.
@return Identifies the (ship to) address for this Business Partner */
public int GetC_BPartner_Location_ID() 
{
Object ii = Get_Value("C_BPartner_Location_ID");
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
/** Set City.
@param C_City_ID City */
public void SetC_City_ID (int C_City_ID)
{
if (C_City_ID <= 0) Set_Value ("C_City_ID", null);
else
Set_Value ("C_City_ID", C_City_ID);
}
/** Get City.
@return City */
public int GetC_City_ID() 
{
Object ii = Get_Value("C_City_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Country.
@param C_Country_ID Country */
public void SetC_Country_ID (int C_Country_ID)
{
if (C_Country_ID <= 0) Set_Value ("C_Country_ID", null);
else
Set_Value ("C_Country_ID", C_Country_ID);
}
/** Get Country.
@return Country */
public int GetC_Country_ID() 
{
Object ii = Get_Value("C_Country_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Greeting.
@param C_Greeting_ID Greeting to print on correspondence */
public void SetC_Greeting_ID (int C_Greeting_ID)
{
if (C_Greeting_ID <= 0) Set_Value ("C_Greeting_ID", null);
else
Set_Value ("C_Greeting_ID", C_Greeting_ID);
}
/** Get Greeting.
@return Greeting to print on correspondence */
public int GetC_Greeting_ID() 
{
Object ii = Get_Value("C_Greeting_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Industry Code.
@param C_IndustryCode_ID Business Partner Industry Classification */
public void SetC_IndustryCode_ID (int C_IndustryCode_ID)
{
if (C_IndustryCode_ID <= 0) Set_Value ("C_IndustryCode_ID", null);
else
Set_Value ("C_IndustryCode_ID", C_IndustryCode_ID);
}
/** Get Industry Code.
@return Business Partner Industry Classification */
public int GetC_IndustryCode_ID() 
{
Object ii = Get_Value("C_IndustryCode_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Position.
@param C_Job_ID Job Position */
public void SetC_Job_ID (int C_Job_ID)
{
if (C_Job_ID <= 0) Set_Value ("C_Job_ID", null);
else
Set_Value ("C_Job_ID", C_Job_ID);
}
/** Get Position.
@return Job Position */
public int GetC_Job_ID() 
{
Object ii = Get_Value("C_Job_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Lead Qualification.
@param C_LeadQualification_ID Lead Qualification evaluation */
public void SetC_LeadQualification_ID (int C_LeadQualification_ID)
{
if (C_LeadQualification_ID <= 0) Set_Value ("C_LeadQualification_ID", null);
else
Set_Value ("C_LeadQualification_ID", C_LeadQualification_ID);
}
/** Get Lead Qualification.
@return Lead Qualification evaluation */
public int GetC_LeadQualification_ID() 
{
Object ii = Get_Value("C_LeadQualification_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Lead.
@param C_Lead_ID Business Lead */
public void SetC_Lead_ID (int C_Lead_ID)
{
if (C_Lead_ID < 1) throw new ArgumentException ("C_Lead_ID is mandatory.");
Set_ValueNoCheck ("C_Lead_ID", C_Lead_ID);
}
/** Get Lead.
@return Business Lead */
public int GetC_Lead_ID() 
{
Object ii = Get_Value("C_Lead_ID");
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
/** Set Region.
@param C_Region_ID Identifies a geographical Region */
public void SetC_Region_ID (int C_Region_ID)
{
if (C_Region_ID <= 0) Set_Value ("C_Region_ID", null);
else
Set_Value ("C_Region_ID", C_Region_ID);
}
/** Get Region.
@return Identifies a geographical Region */
public int GetC_Region_ID() 
{
Object ii = Get_Value("C_Region_ID");
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
/** Set City.
@param City Identifies a City */
public void SetCity (String City)
{
if (City != null && City.Length > 60)
{
log.Warning("Length > 60 - truncated");
City = City.Substring(0,60);
}
Set_Value ("City", City);
}
/** Get City.
@return Identifies a City */
public String GetCity() 
{
return (String)Get_Value("City");
}
/** Set Contact Name.
@param ContactName Business Partner Contact Name */
public void SetContactName (String ContactName)
{
if (ContactName != null && ContactName.Length > 60)
{
log.Warning("Length > 60 - truncated");
ContactName = ContactName.Substring(0,60);
}
Set_Value ("ContactName", ContactName);
}
/** Get Contact Name.
@return Business Partner Contact Name */
public String GetContactName() 
{
return (String)Get_Value("ContactName");
}
/** Set Create BP.
@param CreateBP Create BP */
public void SetCreateBP (String CreateBP)
{
if (CreateBP != null && CreateBP.Length > 1)
{
log.Warning("Length > 1 - truncated");
CreateBP = CreateBP.Substring(0,1);
}
Set_Value ("CreateBP", CreateBP);
}
/** Get Create BP.
@return Create BP */
public String GetCreateBP() 
{
return (String)Get_Value("CreateBP");
}
/** Set Create Project.
@param CreateProject Create Project */
public void SetCreateProject (String CreateProject)
{
if (CreateProject != null && CreateProject.Length > 1)
{
log.Warning("Length > 1 - truncated");
CreateProject = CreateProject.Substring(0,1);
}
Set_Value ("CreateProject", CreateProject);
}
/** Get Create Project.
@return Create Project */
public String GetCreateProject() 
{
return (String)Get_Value("CreateProject");
}
/** Set Create Request.
@param CreateRequest Create Request */
public void SetCreateRequest (String CreateRequest)
{
if (CreateRequest != null && CreateRequest.Length > 1)
{
log.Warning("Length > 1 - truncated");
CreateRequest = CreateRequest.Substring(0,1);
}
Set_Value ("CreateRequest", CreateRequest);
}
/** Get Create Request.
@return Create Request */
public String GetCreateRequest() 
{
return (String)Get_Value("CreateRequest");
}
/** Set D-U-N-S.
@param DUNS Creditor Check (Dun & Bradstreet) Number */
public void SetDUNS (String DUNS)
{
if (DUNS != null && DUNS.Length > 11)
{
log.Warning("Length > 11 - truncated");
DUNS = DUNS.Substring(0,11);
}
Set_Value ("DUNS", DUNS);
}
/** Get D-U-N-S.
@return Creditor Check (Dun & Bradstreet) Number */
public String GetDUNS() 
{
return (String)Get_Value("DUNS");
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 255)
{
log.Warning("Length > 255 - truncated");
Description = Description.Substring(0,255);
}
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
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
/** Set EMail Address.
@param EMail Electronic Mail Address */
public void SetEMail (String EMail)
{
if (EMail != null && EMail.Length > 60)
{
log.Warning("Length > 60 - truncated");
EMail = EMail.Substring(0,60);
}
Set_Value ("EMail", EMail);
}
/** Get EMail Address.
@return Electronic Mail Address */
public String GetEMail() 
{
return (String)Get_Value("EMail");
}
/** Set Fax.
@param Fax Facsimile number */
public void SetFax (String Fax)
{
if (Fax != null && Fax.Length > 40)
{
log.Warning("Length > 40 - truncated");
Fax = Fax.Substring(0,40);
}
Set_Value ("Fax", Fax);
}
/** Get Fax.
@return Facsimile number */
public String GetFax() 
{
return (String)Get_Value("Fax");
}
/** Set Comment.
@param Help Comment, Help or Hint */
public void SetHelp (String Help)
{
if (Help != null && Help.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Help = Help.Substring(0,2000);
}
Set_Value ("Help", Help);
}
/** Get Comment.
@return Comment, Help or Hint */
public String GetHelp() 
{
return (String)Get_Value("Help");
}

/** LeadRating AD_Reference_ID=421 */
public static int LEADRATING_AD_Reference_ID=421;
/** Hot = 1 */
public static String LEADRATING_Hot = "1";
/** Warm = 4 */
public static String LEADRATING_Warm = "4";
/** Cold = 9 */
public static String LEADRATING_Cold = "9";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsLeadRatingValid (String test)
{
return test == null || test.Equals("1") || test.Equals("4") || test.Equals("9");
}
/** Set Lead Rating.
@param LeadRating Rating of the Lead */
public void SetLeadRating (String LeadRating)
{
if (!IsLeadRatingValid(LeadRating))
throw new ArgumentException ("LeadRating Invalid value - " + LeadRating + " - Reference_ID=421 - 1 - 4 - 9");
if (LeadRating != null && LeadRating.Length > 1)
{
log.Warning("Length > 1 - truncated");
LeadRating = LeadRating.Substring(0,1);
}
Set_Value ("LeadRating", LeadRating);
}
/** Get Lead Rating.
@return Rating of the Lead */
public String GetLeadRating() 
{
return (String)Get_Value("LeadRating");
}
/** Set NAICS/SIC.
@param NAICS Standard Industry Code or its successor NAIC - http://www.osha.gov/oshstats/sicser.html */
public void SetNAICS (String NAICS)
{
if (NAICS != null && NAICS.Length > 6)
{
log.Warning("Length > 6 - truncated");
NAICS = NAICS.Substring(0,6);
}
Set_Value ("NAICS", NAICS);
}
/** Get NAICS/SIC.
@return Standard Industry Code or its successor NAIC - http://www.osha.gov/oshstats/sicser.html */
public String GetNAICS() 
{
return (String)Get_Value("NAICS");
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name != null && Name.Length > 60)
{
log.Warning("Length > 60 - truncated");
Name = Name.Substring(0,60);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Set Employees.
@param NumberEmployees Number of employees */
public void SetNumberEmployees (int NumberEmployees)
{
Set_Value ("NumberEmployees", NumberEmployees);
}
/** Get Employees.
@return Number of employees */
public int GetNumberEmployees() 
{
Object ii = Get_Value("NumberEmployees");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Phone.
@param Phone Identifies a telephone number */
public void SetPhone (String Phone)
{
if (Phone != null && Phone.Length > 40)
{
log.Warning("Length > 40 - truncated");
Phone = Phone.Substring(0,40);
}
Set_Value ("Phone", Phone);
}
/** Get Phone.
@return Identifies a telephone number */
public String GetPhone() 
{
return (String)Get_Value("Phone");
}
/** Set 2nd Phone.
@param Phone2 Identifies an alternate telephone number. */
public void SetPhone2 (String Phone2)
{
if (Phone2 != null && Phone2.Length > 40)
{
log.Warning("Length > 40 - truncated");
Phone2 = Phone2.Substring(0,40);
}
Set_Value ("Phone2", Phone2);
}
/** Get 2nd Phone.
@return Identifies an alternate telephone number. */
public String GetPhone2() 
{
return (String)Get_Value("Phone2");
}
/** Set ZIP.
@param Postal Postal code */
public void SetPostal (String Postal)
{
if (Postal != null && Postal.Length > 10)
{
log.Warning("Length > 10 - truncated");
Postal = Postal.Substring(0,10);
}
Set_Value ("Postal", Postal);
}
/** Get ZIP.
@return Postal code */
public String GetPostal() 
{
return (String)Get_Value("Postal");
}
/** Set -.
@param Postal_Add Additional ZIP or Postal code */
public void SetPostal_Add (String Postal_Add)
{
if (Postal_Add != null && Postal_Add.Length > 10)
{
log.Warning("Length > 10 - truncated");
Postal_Add = Postal_Add.Substring(0,10);
}
Set_Value ("Postal_Add", Postal_Add);
}
/** Get -.
@return Additional ZIP or Postal code */
public String GetPostal_Add() 
{
return (String)Get_Value("Postal_Add");
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
/** Set Interest Area.
@param R_InterestArea_ID Interest Area or Topic */
public void SetR_InterestArea_ID (int R_InterestArea_ID)
{
if (R_InterestArea_ID <= 0) Set_Value ("R_InterestArea_ID", null);
else
Set_Value ("R_InterestArea_ID", R_InterestArea_ID);
}
/** Get Interest Area.
@return Interest Area or Topic */
public int GetR_InterestArea_ID() 
{
Object ii = Get_Value("R_InterestArea_ID");
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
/** Set Region.
@param RegionName Name of the Region */
public void SetRegionName (String RegionName)
{
if (RegionName != null && RegionName.Length > 40)
{
log.Warning("Length > 40 - truncated");
RegionName = RegionName.Substring(0,40);
}
Set_Value ("RegionName", RegionName);
}
/** Get Region.
@return Name of the Region */
public String GetRegionName() 
{
return (String)Get_Value("RegionName");
}
/** Set Remote Addr.
@param Remote_Addr Remote Address */
public void SetRemote_Addr (String Remote_Addr)
{
if (Remote_Addr != null && Remote_Addr.Length > 60)
{
log.Warning("Length > 60 - truncated");
Remote_Addr = Remote_Addr.Substring(0,60);
}
Set_Value ("Remote_Addr", Remote_Addr);
}
/** Get Remote Addr.
@return Remote Address */
public String GetRemote_Addr() 
{
return (String)Get_Value("Remote_Addr");
}
/** Set Remote Host.
@param Remote_Host Remote host Info */
public void SetRemote_Host (String Remote_Host)
{
if (Remote_Host != null && Remote_Host.Length > 120)
{
log.Warning("Length > 120 - truncated");
Remote_Host = Remote_Host.Substring(0,120);
}
Set_Value ("Remote_Host", Remote_Host);
}
/** Get Remote Host.
@return Remote host Info */
public String GetRemote_Host() 
{
return (String)Get_Value("Remote_Host");
}

/** SalesRep_ID AD_Reference_ID=190 */
public static int SALESREP_ID_AD_Reference_ID=190;
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
/** Set Sales Volume.
@param SalesVolume Total Volume of Sales in Thousands of Base Currency */
public void SetSalesVolume (int SalesVolume)
{
Set_Value ("SalesVolume", SalesVolume);
}
/** Get Sales Volume.
@return Total Volume of Sales in Thousands of Base Currency */
public int GetSalesVolume() 
{
Object ii = Get_Value("SalesVolume");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Send EMail to Contact.
@param SendNewEMail Send new EMail to Contact */
public void SetSendNewEMail (String SendNewEMail)
{
if (SendNewEMail != null && SendNewEMail.Length > 1)
{
log.Warning("Length > 1 - truncated");
SendNewEMail = SendNewEMail.Substring(0,1);
}
Set_Value ("SendNewEMail", SendNewEMail);
}
/** Get Send EMail to Contact.
@return Send new EMail to Contact */
public String GetSendNewEMail() 
{
return (String)Get_Value("SendNewEMail");
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
/** Set Title.
@param Title Title of the Contact */
public void SetTitle (String Title)
{
if (Title != null && Title.Length > 40)
{
log.Warning("Length > 40 - truncated");
Title = Title.Substring(0,40);
}
Set_Value ("Title", Title);
}
/** Get Title.
@return Title of the Contact */
public String GetTitle() 
{
return (String)Get_Value("Title");
}
/** Set URL.
@param URL Full URL address - e.g. http://www.viennaadvantage.com */
public void SetURL (String URL)
{
if (URL != null && URL.Length > 120)
{
log.Warning("Length > 120 - truncated");
URL = URL.Substring(0,120);
}
Set_Value ("URL", URL);
}
/** Get URL.
@return Full URL address - e.g. http://www.viennaadvantage.com */
public String GetURL() 
{
return (String)Get_Value("URL");
}

/** Ref_BPartner_ID AD_Reference_ID=138 */
public static int REF_BPARTNER_ID_AD_Reference_ID = 138;
/** Set Ref_BPartner_ID.
@param Ref_BPartner_ID Ref_BPartner_ID */
public void SetRef_BPartner_ID(int Ref_BPartner_ID)
{
    if (Ref_BPartner_ID <= 0) Set_Value("Ref_BPartner_ID", null);
    else
        Set_Value("Ref_BPartner_ID", Ref_BPartner_ID);
}
/** Get Ref_BPartner_ID.
@return Ref_BPartner_ID */
public int GetRef_BPartner_ID()
{
    Object ii = Get_Value("Ref_BPartner_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}

/** Status AD_Reference_ID=424 */
public static int STATUS_AD_Reference_ID = 424;
/** New = 10 */
public static String STATUS_New = "10";
/** Intro Mail Sent = 11 */
public static String STATUS_IntroMailSent = "11";
/** First Contact = 12 */
public static String STATUS_FirstContact = "12";
/** Follow up Contact = 14 */
public static String STATUS_FollowUpContact = "14";
/** Product Shown = 15 */
public static String STATUS_ProductShown = "15";
/** Price Quoted = 16 */
public static String STATUS_PriceQuoted = "16";
/** Negotiation = 17 */
public static String STATUS_Negotiation = "17";
/** No Interest = 18 */
public static String STATUS_NoInterest = "18";
/** Follw Up Later = 19 */
public static String STATUS_FollwUpLater = "19";
/** Converted = 20 */
public static String STATUS_Converted = "20";
/** Given Up = 21 */
public static String STATUS_GivenUp = "21";
/** Invalid Data = 22 */
public static String STATUS_InvalidData = "22";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsStatusValid(String test)
{
    return test == null || test.Equals("10") || test.Equals("11") || test.Equals("12") || test.Equals("14") || test.Equals("15") || test.Equals("16") || test.Equals("17") || test.Equals("18") || test.Equals("19") || test.Equals("20") || test.Equals("21") || test.Equals("22");
}
/** Set Status.
@param Status Status of the currently running check */
public void SetStatus(String Status)
{
    if (!IsStatusValid(Status))
        throw new ArgumentException("Status Invalid value - " + Status + " - Reference_ID=424 - 10 - 11 - 12 - 14 - 15 - 16 - 17 - 18 - 19 - 20 - 21 - 22");
    if (Status != null && Status.Length > 2)
    {
        log.Warning("Length > 2 - truncated");
        Status = Status.Substring(0, 2);
    }
    Set_Value("Status", Status);
}
/** Get Status.
@return Status of the currently running check */
public String GetStatus()
{
    return (String)Get_Value("Status");
}

/** Set Mobile.
@param Mobile Mobile */
public void SetMobile(String Mobile)
{
    if (Mobile != null && Mobile.Length > 50)
    {
        log.Warning("Length > 50 - truncated");
        Mobile = Mobile.Substring(0, 50);
    }
    Set_Value("Mobile", Mobile);
}
/** Get Mobile.
@return Mobile */
public String GetMobile()
{
    return (String)Get_Value("Mobile");
}
/** Set Enquiry Received @param C_EnquiryRdate Enquiry Received Date */
public void SetC_EnquiryRdate (DateTime? C_EnquiryRdate){Set_Value ("C_EnquiryRdate", (DateTime?)C_EnquiryRdate);}/** Get Enquiry Received Date.
@return Enquiry Received Date */
public DateTime? GetC_EnquiryRdate() {return (DateTime?)Get_Value("C_EnquiryRdate");}/** Set FollowUp Date.
@param C_Followupdate FollowUp Date */
public void SetC_Followupdate (DateTime? C_Followupdate){Set_Value ("C_Followupdate", (DateTime?)C_Followupdate);}/** Get FollowUp Date.
@return FollowUp Date */
public DateTime? GetC_Followupdate() {return (DateTime?)Get_Value("C_Followupdate");}

/** Set Proposal Due Date.
@param C_ProposalDdate Proposal Due Date */
public void SetC_ProposalDdate(DateTime? C_ProposalDdate) { Set_Value("C_ProposalDdate", (DateTime?)C_ProposalDdate); }/** Get Proposal Due Date.
@return Proposal Due Date */
public DateTime? GetC_ProposalDdate() { return (DateTime?)Get_Value("C_ProposalDdate"); }
/** Set First Name.
@param FirstName Alphanumeric identifier of the entity */
public void SetFirstName(String FirstName) { if (FirstName != null && FirstName.Length > 60) { log.Warning("Length > 60 - truncated"); FirstName = FirstName.Substring(0, 60); } Set_Value("FirstName", FirstName); }/** Get First Name.
@return Alphanumeric identifier of the entity */
 public String GetFirstName() { return (String)Get_Value("FirstName"); }

/** Set Last Name.
@param LastName Last Name */
public void SetLastName(String LastName) { if (LastName != null && LastName.Length > 60) { log.Warning("Length > 60 - truncated"); LastName = LastName.Substring(0, 60); } Set_Value("LastName", LastName); }/** Get Last Name.
@return Last Name */
public String GetLastName() { return (String)Get_Value("LastName"); }
}

}
