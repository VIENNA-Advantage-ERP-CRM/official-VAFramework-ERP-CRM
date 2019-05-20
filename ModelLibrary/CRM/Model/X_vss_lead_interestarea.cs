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
/** Generated Model for vss_lead_interestarea
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_vss_lead_interestarea : PO
{
public X_vss_lead_interestarea (Context ctx, int vss_lead_interestarea_ID, Trx trxName) : base (ctx, vss_lead_interestarea_ID, trxName)
{
/** if (vss_lead_interestarea_ID == 0)
{
SetR_InterestArea_ID (0);
Setvss_lead_interestarea_ID (0);
}
 */
}
public X_vss_lead_interestarea (Ctx ctx, int vss_lead_interestarea_ID, Trx trxName) : base (ctx, vss_lead_interestarea_ID, trxName)
{
/** if (vss_lead_interestarea_ID == 0)
{
SetR_InterestArea_ID (0);
Setvss_lead_interestarea_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_vss_lead_interestarea (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_vss_lead_interestarea (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_vss_lead_interestarea (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_vss_lead_interestarea()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27607356283620L;
/** Last Updated Timestamp 12/30/2011 1:12:47 PM */
public static long updatedMS = 1325230966831L;
/** AD_Table_ID=1000224 */
public static int Table_ID;
 // =1000224;

/** TableName=vss_lead_interestarea */
public static String Table_Name="vss_lead_interestarea";

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
StringBuilder sb = new StringBuilder ("X_vss_lead_interestarea[").Append(Get_ID()).Append("]");
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
/** Set Partner Name.
@param BPName Account or Business Partner Name */
public void SetBPName (String BPName)
{
if (BPName != null && BPName.Length > 50)
{
log.Warning("Length > 50 - truncated");
BPName = BPName.Substring(0,50);
}
Set_Value ("BPName", BPName);
}
/** Get Partner Name.
@return Account or Business Partner Name */
public String GetBPName() 
{
return (String)Get_Value("BPName");
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
/** Set Fax.
@param Fax Facsimile number */
public void SetFax (String Fax)
{
if (Fax != null && Fax.Length > 50)
{
log.Warning("Length > 50 - truncated");
Fax = Fax.Substring(0,50);
}
Set_Value ("Fax", Fax);
}
/** Get Fax.
@return Facsimile number */
public String GetFax() 
{
return (String)Get_Value("Fax");
}
/** Set Opt-out Date.
@param OptOutDate Date the contact opted out */
public void SetOptOutDate (DateTime? OptOutDate)
{
Set_Value ("OptOutDate", (DateTime?)OptOutDate);
}
/** Get Opt-out Date.
@return Date the contact opted out */
public DateTime? GetOptOutDate() 
{
return (DateTime?)Get_Value("OptOutDate");
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
/** Set Interest Area.
@param R_InterestArea_ID Interest Area or Topic */
public void SetR_InterestArea_ID (int R_InterestArea_ID)
{
if (R_InterestArea_ID < 1) throw new ArgumentException ("R_InterestArea_ID is mandatory.");
Set_ValueNoCheck ("R_InterestArea_ID", R_InterestArea_ID);
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
/** Set Subscribe Date.
@param SubscribeDate Date the contact actively subscribed */
public void SetSubscribeDate (DateTime? SubscribeDate)
{
Set_Value ("SubscribeDate", (DateTime?)SubscribeDate);
}
/** Get Subscribe Date.
@return Date the contact actively subscribed */
public DateTime? GetSubscribeDate() 
{
return (DateTime?)Get_Value("SubscribeDate");
}
/** Set vss_lead_interestarea_ID.
@param vss_lead_interestarea_ID vss_lead_interestarea_ID */
public void Setvss_lead_interestarea_ID (int vss_lead_interestarea_ID)
{
if (vss_lead_interestarea_ID < 1) throw new ArgumentException ("vss_lead_interestarea_ID is mandatory.");
Set_ValueNoCheck ("vss_lead_interestarea_ID", vss_lead_interestarea_ID);
}
/** Get vss_lead_interestarea_ID.
@return vss_lead_interestarea_ID */
public int Getvss_lead_interestarea_ID() 
{
Object ii = Get_Value("vss_lead_interestarea_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
