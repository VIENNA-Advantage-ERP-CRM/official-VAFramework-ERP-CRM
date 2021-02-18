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
/** Generated Model for VAB_InviteeList
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_InviteeList : PO
{
public X_VAB_InviteeList (Context ctx, int VAB_InviteeList_ID, Trx trxName) : base (ctx, VAB_InviteeList_ID, trxName)
{
/** if (VAB_InviteeList_ID == 0)
{
SetVAB_Promotion_ID (0);
SetVAB_InviteeList_ID (0);
}
 */
}
public X_VAB_InviteeList (Ctx ctx, int VAB_InviteeList_ID, Trx trxName) : base (ctx, VAB_InviteeList_ID, trxName)
{
/** if (VAB_InviteeList_ID == 0)
{
SetVAB_Promotion_ID (0);
SetVAB_InviteeList_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_InviteeList (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_InviteeList (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_InviteeList (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_InviteeList()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27610657762556L;
/** Last Updated Timestamp 2/6/2012 6:17:26 PM */
public static long updatedMS = 1328532445767L;
/** VAF_TableView_ID=1000241 */
public static int Table_ID;
 // =1000241;

/** TableName=VAB_InviteeList */
public static String Table_Name="VAB_InviteeList";

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
StringBuilder sb = new StringBuilder ("X_VAB_InviteeList[").Append(Get_ID()).Append("]");
return sb.ToString();
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
@param VAB_Promotion_ID Marketing Campaign */
public void SetVAB_Promotion_ID (int VAB_Promotion_ID)
{
if (VAB_Promotion_ID < 1) throw new ArgumentException ("VAB_Promotion_ID is mandatory.");
Set_ValueNoCheck ("VAB_Promotion_ID", VAB_Promotion_ID);
}
/** Get Campaign.
@return Marketing Campaign */
public int GetVAB_Promotion_ID() 
{
Object ii = Get_Value("VAB_Promotion_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set City.
@param VAB_City_ID City */
public void SetVAB_City_ID (int VAB_City_ID)
{
if (VAB_City_ID <= 0) Set_Value ("VAB_City_ID", null);
else
Set_Value ("VAB_City_ID", VAB_City_ID);
}
/** Get City.
@return City */
public int GetVAB_City_ID() 
{
Object ii = Get_Value("VAB_City_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Country.
@param VAB_Country_ID Country  */
public void SetVAB_Country_ID (int VAB_Country_ID)
{
if (VAB_Country_ID <= 0) Set_Value ("VAB_Country_ID", null);
else
Set_Value ("VAB_Country_ID", VAB_Country_ID);
}
/** Get Country.
@return Country  */
public int GetVAB_Country_ID() 
{
Object ii = Get_Value("VAB_Country_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set VAB_InviteeList_ID.
@param VAB_InviteeList_ID VAB_InviteeList_ID */
public void SetVAB_InviteeList_ID (int VAB_InviteeList_ID)
{
if (VAB_InviteeList_ID < 1) throw new ArgumentException ("VAB_InviteeList_ID is mandatory.");
Set_ValueNoCheck ("VAB_InviteeList_ID", VAB_InviteeList_ID);
}
/** Get VAB_InviteeList_ID.
@return VAB_InviteeList_ID */
public int GetVAB_InviteeList_ID() 
{
Object ii = Get_Value("VAB_InviteeList_ID");
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
/** Set Address.
@param VAB_Address_ID Location or Address */
public void SetVAB_Address_ID (int VAB_Address_ID)
{
if (VAB_Address_ID <= 0) Set_Value ("VAB_Address_ID", null);
else
Set_Value ("VAB_Address_ID", VAB_Address_ID);
}
/** Get Address.
@return Location or Address */
public int GetVAB_Address_ID() 
{
Object ii = Get_Value("VAB_Address_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Region.
@param VAB_RegionState_ID Identifies a geographical Region */
public void SetVAB_RegionState_ID (int VAB_RegionState_ID)
{
if (VAB_RegionState_ID <= 0) Set_Value ("VAB_RegionState_ID", null);
else
Set_Value ("VAB_RegionState_ID", VAB_RegionState_ID);
}
/** Get Region.
@return Identifies a geographical Region */
public int GetVAB_RegionState_ID() 
{
Object ii = Get_Value("VAB_RegionState_ID");
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
