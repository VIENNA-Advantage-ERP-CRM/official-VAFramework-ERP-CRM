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
/** Generated Model for I_BPartner
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_I_BPartner : PO
{
public X_I_BPartner (Context ctx, int I_BPartner_ID, Trx trxName) : base (ctx, I_BPartner_ID, trxName)
{
/** if (I_BPartner_ID == 0)
{
SetI_BPartner_ID (0);
SetI_IsImported (null);	// N
}
 */
}
public X_I_BPartner (Ctx ctx, int I_BPartner_ID, Trx trxName) : base (ctx, I_BPartner_ID, trxName)
{
/** if (I_BPartner_ID == 0)
{
SetI_BPartner_ID (0);
SetI_IsImported (null);	// N
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_BPartner (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_BPartner (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_BPartner (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_I_BPartner()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514376842L;
/** Last Updated Timestamp 7/29/2010 1:07:40 PM */
public static long updatedMS = 1280389060053L;
/** AD_Table_ID=533 */
public static int Table_ID;
 // =533;

/** TableName=I_BPartner */
public static String Table_Name="I_BPartner";

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
StringBuilder sb = new StringBuilder ("X_I_BPartner[").Append(Get_ID()).Append("]");
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
/** Set BP Contact Greeting.
@param BPContactGreeting Greeting for Business Partner Contact */
public void SetBPContactGreeting (String BPContactGreeting)
{
if (BPContactGreeting != null && BPContactGreeting.Length > 60)
{
log.Warning("Length > 60 - truncated");
BPContactGreeting = BPContactGreeting.Substring(0,60);
}
Set_Value ("BPContactGreeting", BPContactGreeting);
}
/** Get BP Contact Greeting.
@return Greeting for Business Partner Contact */
public String GetBPContactGreeting() 
{
return (String)Get_Value("BPContactGreeting");
}
/** Set Birthday.
@param Birthday Birthday or Anniversary day */
public void SetBirthday (DateTime? Birthday)
{
Set_Value ("Birthday", (DateTime?)Birthday);
}
/** Get Birthday.
@return Birthday or Anniversary day */
public DateTime? GetBirthday() 
{
return (DateTime?)Get_Value("Birthday");
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
/** Set Comments.
@param Comments Comments or additional information */
public void SetComments (String Comments)
{
if (Comments != null && Comments.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Comments = Comments.Substring(0,2000);
}
Set_Value ("Comments", Comments);
}
/** Get Comments.
@return Comments or additional information */
public String GetComments() 
{
return (String)Get_Value("Comments");
}
/** Set Contact Description.
@param ContactDescription Description of Contact */
public void SetContactDescription (String ContactDescription)
{
if (ContactDescription != null && ContactDescription.Length > 255)
{
log.Warning("Length > 255 - truncated");
ContactDescription = ContactDescription.Substring(0,255);
}
Set_Value ("ContactDescription", ContactDescription);
}
/** Get Contact Description.
@return Description of Contact */
public String GetContactDescription() 
{
return (String)Get_Value("ContactDescription");
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
/** Set ISO Country Code.
@param CountryCode Upper-case two-letter alphabetic ISO Country code according to ISO 3166-1 - http://www.chemie.fu-berlin.de/diverse/doc/ISO_3166.html */
public void SetCountryCode (String CountryCode)
{
if (CountryCode != null && CountryCode.Length > 2)
{
log.Warning("Length > 2 - truncated");
CountryCode = CountryCode.Substring(0,2);
}
Set_Value ("CountryCode", CountryCode);
}
/** Get ISO Country Code.
@return Upper-case two-letter alphabetic ISO Country code according to ISO 3166-1 - http://www.chemie.fu-berlin.de/diverse/doc/ISO_3166.html */
public String GetCountryCode() 
{
return (String)Get_Value("CountryCode");
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
/** Set Group Key.
@param GroupValue Business Partner Group Key */
public void SetGroupValue (String GroupValue)
{
if (GroupValue != null && GroupValue.Length > 40)
{
log.Warning("Length > 40 - truncated");
GroupValue = GroupValue.Substring(0,40);
}
Set_Value ("GroupValue", GroupValue);
}
/** Get Group Key.
@return Business Partner Group Key */
public String GetGroupValue() 
{
return (String)Get_Value("GroupValue");
}
/** Set Import Business Partner.
@param I_BPartner_ID Import Business Partner */
public void SetI_BPartner_ID (int I_BPartner_ID)
{
if (I_BPartner_ID < 1) throw new ArgumentException ("I_BPartner_ID is mandatory.");
Set_ValueNoCheck ("I_BPartner_ID", I_BPartner_ID);
}
/** Get Import Business Partner.
@return Import Business Partner */
public int GetI_BPartner_ID() 
{
Object ii = Get_Value("I_BPartner_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Interest Area.
@param InterestAreaName Name of the Interest Area */
public void SetInterestAreaName (String InterestAreaName)
{
if (InterestAreaName != null && InterestAreaName.Length > 40)
{
log.Warning("Length > 40 - truncated");
InterestAreaName = InterestAreaName.Substring(0,40);
}
Set_Value ("InterestAreaName", InterestAreaName);
}
/** Get Interest Area.
@return Name of the Interest Area */
public String GetInterestAreaName() 
{
return (String)Get_Value("InterestAreaName");
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
/** Set Name 2.
@param Name2 Additional Name */
public void SetName2 (String Name2)
{
if (Name2 != null && Name2.Length > 60)
{
log.Warning("Length > 60 - truncated");
Name2 = Name2.Substring(0,60);
}
Set_Value ("Name2", Name2);
}
/** Get Name 2.
@return Additional Name */
public String GetName2() 
{
return (String)Get_Value("Name2");
}
/** Set Password.
@param Password Password of any length (case sensitive) */
public void SetPassword (String Password)
{
if (Password != null && Password.Length > 20)
{
log.Warning("Length > 20 - truncated");
Password = Password.Substring(0,20);
}
Set_Value ("Password", Password);
}
/** Get Password.
@return Password of any length (case sensitive) */
public String GetPassword() 
{
return (String)Get_Value("Password");
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
Set_ValueNoCheck ("Processed", Processed);
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
/** Set Region.
@param RegionName Name of the Region */
public void SetRegionName (String RegionName)
{
if (RegionName != null && RegionName.Length > 60)
{
log.Warning("Length > 60 - truncated");
RegionName = RegionName.Substring(0,60);
}
Set_Value ("RegionName", RegionName);
}
/** Get Region.
@return Name of the Region */
public String GetRegionName() 
{
return (String)Get_Value("RegionName");
}
/** Set Tax ID.
@param TaxID Tax Identification */
public void SetTaxID (String TaxID)
{
if (TaxID != null && TaxID.Length > 20)
{
log.Warning("Length > 20 - truncated");
TaxID = TaxID.Substring(0,20);
}
Set_Value ("TaxID", TaxID);
}
/** Get Tax ID.
@return Tax Identification */
public String GetTaxID() 
{
return (String)Get_Value("TaxID");
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
/** Set Search Key.
@param Value Search key for the record in the format required - must be unique */
public void SetValue (String Value)
{
if (Value != null && Value.Length > 40)
{
log.Warning("Length > 40 - truncated");
Value = Value.Substring(0,40);
}
Set_Value ("Value", Value);
}
/** Get Search Key.
@return Search key for the record in the format required - must be unique */
public String GetValue() 
{
return (String)Get_Value("Value");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetValue());
}
/** Set Address 3.
@param Address3 Address Line 3 for the location */
public void SetAddress3(String Address3)
{
    if (Address3 != null && Address3.Length > 60)
    {
        log.Warning("Length > 60 - truncated");
        Address3 = Address3.Substring(0, 60);
    }
    Set_Value("Address3", Address3);
}
/** Get Address 3.
@return Address Line 3 for the location */
public String GetAddress3()
{
    return (String)Get_Value("Address3");
}
/** Set Address 4.
@param Address4 Address Line 4 for the location */
public void SetAddress4(String Address4)
{
    if (Address4 != null && Address4.Length > 60)
    {
        log.Warning("Length > 60 - truncated");
        Address4 = Address4.Substring(0, 60);
    }
    Set_Value("Address4", Address4);
}
/** Get Address 4.
@return Address Line 4 for the location */
public String GetAddress4()
{
    return (String)Get_Value("Address4");
}
/** Set BANKACCOUNT.
@param BANKACCOUNT BANKACCOUNT */
public void SetBANKACCOUNT(String BANKACCOUNT)
{
    if (BANKACCOUNT != null && BANKACCOUNT.Length > 60)
    {
        log.Warning("Length > 60 - truncated");
        BANKACCOUNT = BANKACCOUNT.Substring(0, 60);
    }
    Set_Value("BANKACCOUNT", BANKACCOUNT);
}
/** Get BANKACCOUNT.
@return BANKACCOUNT */
public String GetBANKACCOUNT()
{
    return (String)Get_Value("BANKACCOUNT");
}
/** Set Partner Bank Account.
@param C_BP_BankAccount_ID Bank Account of the Business Partner */
public void SetC_BP_BankAccount_ID(int C_BP_BankAccount_ID)
{
    if (C_BP_BankAccount_ID <= 0) Set_Value("C_BP_BankAccount_ID", null);
    else
        Set_Value("C_BP_BankAccount_ID", C_BP_BankAccount_ID);
}
/** Get Partner Bank Account.
@return Bank Account of the Business Partner */
public int GetC_BP_BankAccount_ID()
{
    Object ii = Get_Value("C_BP_BankAccount_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}

/** Set Customer.
@param IsCustomer Indicates if this Business Partner is a Customer */
public void SetIsCustomer(Boolean IsCustomer)
{
    Set_Value("IsCustomer", IsCustomer);
}
/** Get Customer.
@return Indicates if this Business Partner is a Customer */
public Boolean IsCustomer()
{
    Object oo = Get_Value("IsCustomer");
    if (oo != null)
    {
        if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
        return "Y".Equals(oo);
    }
    return false;
}
/** Set Employee.
@param IsEmployee Indicates if  this Business Partner is an employee */
public void SetIsEmployee(Boolean IsEmployee)
{
    Set_Value("IsEmployee", IsEmployee);
}
/** Get Employee.
@return Indicates if  this Business Partner is an employee */
public Boolean IsEmployee()
{
    Object oo = Get_Value("IsEmployee");
    if (oo != null)
    {
        if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
        return "Y".Equals(oo);
    }
    return false;
}
/** Set Representative.
@param IsSalesRep Indicates if  the business partner is a representative or company agent */
public void SetIsSalesRep(Boolean IsSalesRep)
{
    Set_Value("IsSalesRep", IsSalesRep);
}
/** Get Representative.
@return Indicates if  the business partner is a representative or company agent */
public Boolean IsSalesRep()
{
    Object oo = Get_Value("IsSalesRep");
    if (oo != null)
    {
        if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
        return "Y".Equals(oo);
    }
    return false;
}
/** Set Vendor.
@param IsVendor Indicates if this Business Partner is a Vendor */
public void SetIsVendor(Boolean IsVendor)
{
    Set_Value("IsVendor", IsVendor);
}
/** Get Vendor.
@return Indicates if this Business Partner is a Vendor */
public Boolean IsVendor()
{
    Object oo = Get_Value("IsVendor");
    if (oo != null)
    {
        if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
        return "Y".Equals(oo);
    }
    return false;
}
/** Set IsVendorRep.
@param IsVendorRep IsVendorRep */
public void SetIsVendorRep(Boolean IsVendorRep)
{
    Set_Value("IsVendorRep", IsVendorRep);
}
/** Get IsVendorRep.
@return IsVendorRep */
public Boolean IsVendorRep()
{
    Object oo = Get_Value("IsVendorRep");
    if (oo != null)
    {
        if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
        return "Y".Equals(oo);
    }
    return false;
}
/** Set Mobile.
@param Mobile Identifies the mobile number. */
public void SetMobile(String Mobile)
{
    if (Mobile != null && Mobile.Length > 40)
    {
        log.Warning("Length > 40 - truncated");
        Mobile = Mobile.Substring(0, 40);
    }
    Set_Value("Mobile", Mobile);
}
/** Get Mobile.
@return Identifies the mobile number. */
public String GetMobile()
{
    return (String)Get_Value("Mobile");
}

/** Set Open Balance Date.
@param OpenBalanceDate Open Balance Date */
public void SetOpenBalanceDate(DateTime? OpenBalanceDate)
{
    Set_Value("OpenBalanceDate", (DateTime?)OpenBalanceDate);
}
/** Get Open Balance Date.
@return Open Balance Date */
public DateTime? GetOpenBalanceDate()
{
    return (DateTime?)Get_Value("OpenBalanceDate");
}

/** Set Credit Limit.
@param SO_CreditLimit Total outstanding invoice amounts allowed */
public void SetSO_CreditLimit(Decimal? SO_CreditLimit)
{
    Set_Value("SO_CreditLimit", (Decimal?)SO_CreditLimit);
}
/** Get Credit Limit.
@return Total outstanding invoice amounts allowed */
public Decimal GetSO_CreditLimit()
{
    Object bd = Get_Value("SO_CreditLimit");
    if (bd == null) return Env.ZERO;
    return Convert.ToDecimal(bd);
}

/** Set UnReconciledPayment.
@param UnReconciledPayment UnReconciledPayment */
public void SetUnReconciledPayment(Decimal? UnReconciledPayment)
{
    Set_Value("UnReconciledPayment", (Decimal?)UnReconciledPayment);
}
/** Get UnReconciledPayment.
@return UnReconciledPayment */
public Decimal GetUnReconciledPayment()
{
    Object bd = Get_Value("UnReconciledPayment");
    if (bd == null) return Env.ZERO;
    return Convert.ToDecimal(bd);
}

}

}
