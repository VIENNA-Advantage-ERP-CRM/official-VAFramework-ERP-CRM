namespace VAdvantage.Model
{

/** Generated Model - DO NOT CHANGE */
using System;
using System.Text;
using VAdvantage.DataBase;
//using VAdvantage.Common;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Utility;
using System.Data;
/** Generated Model for C_InviteeList
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_InviteeList : PO
{
public X_C_InviteeList (Context ctx, int C_InviteeList_ID, Trx trxName) : base (ctx, C_InviteeList_ID, trxName)
{
/** if (C_InviteeList_ID == 0)
{
SetC_Campaign_ID (0);
SetC_InviteeList_ID (0);
}
 */
}
public X_C_InviteeList (Ctx ctx, int C_InviteeList_ID, Trx trxName) : base (ctx, C_InviteeList_ID, trxName)
{
/** if (C_InviteeList_ID == 0)
{
SetC_Campaign_ID (0);
SetC_InviteeList_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_InviteeList (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_InviteeList (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_InviteeList (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_InviteeList()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27610657762556L;
/** Last Updated Timestamp 2/6/2012 6:17:26 PM */
public static long updatedMS = 1328532445767L;
/** AD_Table_ID=1000241 */
public static int Table_ID;
 // =1000241;

/** TableName=C_InviteeList */
public static String Table_Name="C_InviteeList";

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
StringBuilder sb = new StringBuilder ("X_C_InviteeList[").Append(Get_ID()).Append("]");
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
if (Address1 != null && Address1.Length > 50)
{
log.Warning("Length > 50 - truncated");
Address1 = Address1.Substring(0,50);
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
if (Address2 != null && Address2.Length > 50)
{
log.Warning("Length > 50 - truncated");
Address2 = Address2.Substring(0,50);
}
Set_Value ("Address2", Address2);
}
/** Get Address 2.
@return Address line 2 for this location */
public String GetAddress2() 
{
return (String)Get_Value("Address2");
}
/** Set Campaign.
@param C_Campaign_ID Marketing Campaign */
public void SetC_Campaign_ID (int C_Campaign_ID)
{
if (C_Campaign_ID < 1) throw new ArgumentException ("C_Campaign_ID is mandatory.");
Set_ValueNoCheck ("C_Campaign_ID", C_Campaign_ID);
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
@param C_Country_ID Country  */
public void SetC_Country_ID (int C_Country_ID)
{
if (C_Country_ID <= 0) Set_Value ("C_Country_ID", null);
else
Set_Value ("C_Country_ID", C_Country_ID);
}
/** Get Country.
@return Country  */
public int GetC_Country_ID() 
{
Object ii = Get_Value("C_Country_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set C_InviteeList_ID.
@param C_InviteeList_ID C_InviteeList_ID */
public void SetC_InviteeList_ID (int C_InviteeList_ID)
{
if (C_InviteeList_ID < 1) throw new ArgumentException ("C_InviteeList_ID is mandatory.");
Set_ValueNoCheck ("C_InviteeList_ID", C_InviteeList_ID);
}
/** Get C_InviteeList_ID.
@return C_InviteeList_ID */
public int GetC_InviteeList_ID() 
{
Object ii = Get_Value("C_InviteeList_ID");
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
/** Set Address.
@param C_Location_ID Location or Address */
public void SetC_Location_ID (int C_Location_ID)
{
if (C_Location_ID <= 0) Set_Value ("C_Location_ID", null);
else
Set_Value ("C_Location_ID", C_Location_ID);
}
/** Get Address.
@return Location or Address */
public int GetC_Location_ID() 
{
Object ii = Get_Value("C_Location_ID");
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
if (City != null && City.Length > 50)
{
log.Warning("Length > 50 - truncated");
City = City.Substring(0,50);
}
Set_Value ("City", City);
}
/** Get City.
@return Identifies a City */
public String GetCity() 
{
return (String)Get_Value("City");
}
/** Set EMail Address.
@param EMail Electronic Mail Address */
public void SetEMail (String EMail)
{
if (EMail != null && EMail.Length > 50)
{
log.Warning("Length > 50 - truncated");
EMail = EMail.Substring(0,50);
}
Set_Value ("EMail", EMail);
}
/** Get EMail Address.
@return Electronic Mail Address */
public String GetEMail() 
{
return (String)Get_Value("EMail");
}
/** Set Invitee Response.
@param InviteeResponse Invitee Response */
public void SetInviteeResponse (String InviteeResponse)
{
if (InviteeResponse != null && InviteeResponse.Length > 1)
{
log.Warning("Length > 1 - truncated");
InviteeResponse = InviteeResponse.Substring(0,1);
}
Set_Value ("InviteeResponse", InviteeResponse);
}
/** Get Invitee Response.
@return Invitee Response */
public String GetInviteeResponse() 
{
return (String)Get_Value("InviteeResponse");
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name != null && Name.Length > 50)
{
log.Warning("Length > 50 - truncated");
Name = Name.Substring(0,50);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Set Phone.
@param Phone Identifies a telephone number */
public void SetPhone (String Phone)
{
if (Phone != null && Phone.Length > 50)
{
log.Warning("Length > 50 - truncated");
Phone = Phone.Substring(0,50);
}
Set_Value ("Phone", Phone);
}
/** Get Phone.
@return Identifies a telephone number */
public String GetPhone() 
{
return (String)Get_Value("Phone");
}
/** Set ZIP.
@param Postal Postal code */
public void SetPostal (String Postal)
{
if (Postal != null && Postal.Length > 50)
{
log.Warning("Length > 50 - truncated");
Postal = Postal.Substring(0,50);
}
Set_Value ("Postal", Postal);
}
/** Get ZIP.
@return Postal code */
public String GetPostal() 
{
return (String)Get_Value("Postal");
}
/** Set Region.
@param RegionName Name of the Region */
public void SetRegionName (String RegionName)
{
if (RegionName != null && RegionName.Length > 50)
{
log.Warning("Length > 50 - truncated");
RegionName = RegionName.Substring(0,50);
}
Set_Value ("RegionName", RegionName);
}
/** Get Region.
@return Name of the Region */
public String GetRegionName() 
{
return (String)Get_Value("RegionName");
}
/** Set Remark.
@param Remark Remark */
public void SetRemark (String Remark)
{
Set_Value ("Remark", Remark);
}
/** Get Remark.
@return Remark */
public String GetRemark() 
{
return (String)Get_Value("Remark");
}
/** Set Response Date.
@param ResponseDate Response Date */
public void SetResponseDate (DateTime? ResponseDate)
{
Set_Value ("ResponseDate", (DateTime?)ResponseDate);
}
/** Get Response Date.
@return Response Date */
public DateTime? GetResponseDate() 
{
return (DateTime?)Get_Value("ResponseDate");
}
/** Set URL.
@param URL Full URL address - e.g. http://www.viennasolutions.com */
public void SetURL (String URL)
{
if (URL != null && URL.Length > 50)
{
log.Warning("Length > 50 - truncated");
URL = URL.Substring(0,50);
}
Set_Value ("URL", URL);
}
/** Get URL.
@return Full URL address - e.g. http://www.viennasolutions.com */
public String GetURL() 
{
return (String)Get_Value("URL");
}
}

}
