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
SetVAB_MasterTargetList_ID (0);
SetC_TargetList_ID (0);
}
 */
}
public X_C_TargetList (Ctx ctx, int C_TargetList_ID, Trx trxName) : base (ctx, C_TargetList_ID, trxName)
{
/** if (C_TargetList_ID == 0)
{
SetVAB_MasterTargetList_ID (0);
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
/** VAF_TableView_ID=1000240 */
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
/** Set VAB_MasterTargetList_ID.
@param VAB_MasterTargetList_ID VAB_MasterTargetList_ID */
public void SetVAB_MasterTargetList_ID (int VAB_MasterTargetList_ID)
{
if (VAB_MasterTargetList_ID < 1) throw new ArgumentException ("VAB_MasterTargetList_ID is mandatory.");
Set_ValueNoCheck ("VAB_MasterTargetList_ID", VAB_MasterTargetList_ID);
}
/** Get VAB_MasterTargetList_ID.
@return VAB_MasterTargetList_ID */
public int GetVAB_MasterTargetList_ID() 
{
Object ii = Get_Value("VAB_MasterTargetList_ID");
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
@param VAR_InterestArea_ID Interest Area or Topic */
public void SetR_InterestArea_ID (int VAR_InterestArea_ID)
{
if (VAR_InterestArea_ID <= 0) Set_Value ("VAR_InterestArea_ID", null);
else
Set_Value ("VAR_InterestArea_ID", VAR_InterestArea_ID);
}
/** Get Interest Area.
@return Interest Area or Topic */
public int GetR_InterestArea_ID() 
{
Object ii = Get_Value("VAR_InterestArea_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Ref_BPartner_ID VAF_Control_Ref_ID=138 */
public static int REF_BPARTNER_ID_VAF_Control_Ref_ID=138;
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
