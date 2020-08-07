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
/** Generated Model for C_TargetList
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_TargetList : PO
{
public X_C_TargetList (Context ctx, int C_TargetList_ID, Trx trxName) : base (ctx, C_TargetList_ID, trxName)
{
/** if (C_TargetList_ID == 0)
{
SetC_MasterTargetList_ID (0);
SetC_TargetList_ID (0);
}
 */
}
public X_C_TargetList (Ctx ctx, int C_TargetList_ID, Trx trxName) : base (ctx, C_TargetList_ID, trxName)
{
/** if (C_TargetList_ID == 0)
{
SetC_MasterTargetList_ID (0);
SetC_TargetList_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_TargetList (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_TargetList (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_TargetList (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_TargetList()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27610469706157L;
/** Last Updated Timestamp 2/4/2012 2:03:09 PM */
public static long updatedMS = 1328344389368L;
/** AD_Table_ID=1000240 */
public static int Table_ID;
 // =1000240;

/** TableName=C_TargetList */
public static String Table_Name="C_TargetList";

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
StringBuilder sb = new StringBuilder ("X_C_TargetList[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set C_MasterTargetList_ID.
@param C_MasterTargetList_ID C_MasterTargetList_ID */
public void SetC_MasterTargetList_ID (int C_MasterTargetList_ID)
{
if (C_MasterTargetList_ID < 1) throw new ArgumentException ("C_MasterTargetList_ID is mandatory.");
Set_ValueNoCheck ("C_MasterTargetList_ID", C_MasterTargetList_ID);
}
/** Get C_MasterTargetList_ID.
@return C_MasterTargetList_ID */
public int GetC_MasterTargetList_ID() 
{
Object ii = Get_Value("C_MasterTargetList_ID");
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
/** Set C_TargetList_ID.
@param C_TargetList_ID C_TargetList_ID */
public void SetC_TargetList_ID (int C_TargetList_ID)
{
if (C_TargetList_ID < 1) throw new ArgumentException ("C_TargetList_ID is mandatory.");
Set_ValueNoCheck ("C_TargetList_ID", C_TargetList_ID);
}
/** Get C_TargetList_ID.
@return C_TargetList_ID */
public int GetC_TargetList_ID() 
{
Object ii = Get_Value("C_TargetList_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set City.
@param City Identifies a City */
public void SetCity (String City)
{
if (City != null && City.Length > 10)
{
log.Warning("Length > 10 - truncated");
City = City.Substring(0,10);
}
Set_Value ("City", City);
}
/** Get City.
@return Identifies a City */
public String GetCity() 
{
return (String)Get_Value("City");
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

/** Ref_BPartner_ID AD_Reference_ID=138 */
public static int REF_BPARTNER_ID_AD_Reference_ID=138;
/** Set Ref_BPartner_ID.
@param Ref_BPartner_ID Ref_BPartner_ID */
public void SetRef_BPartner_ID (int Ref_BPartner_ID)
{
if (Ref_BPartner_ID <= 0) Set_Value ("Ref_BPartner_ID", null);
else
Set_Value ("Ref_BPartner_ID", Ref_BPartner_ID);
}
/** Get Ref_BPartner_ID.
@return Ref_BPartner_ID */
public int GetRef_BPartner_ID() 
{
Object ii = Get_Value("Ref_BPartner_ID");
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
}

}
