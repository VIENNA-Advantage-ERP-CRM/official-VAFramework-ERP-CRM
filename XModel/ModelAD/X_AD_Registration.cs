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
/** Generated Model for AD_Registration
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Registration : PO
{
public X_AD_Registration (Context ctx, int AD_Registration_ID, Trx trxName) : base (ctx, AD_Registration_ID, trxName)
{
/** if (AD_Registration_ID == 0)
{
SetAD_Registration_ID (0);
SetIsAllowPublish (true);	// Y
SetIsAllowStatistics (true);	// Y
SetIsInProduction (false);
SetIsRegistered (false);	// N
}
 */
}
public X_AD_Registration (Ctx ctx, int AD_Registration_ID, Trx trxName) : base (ctx, AD_Registration_ID, trxName)
{
/** if (AD_Registration_ID == 0)
{
SetAD_Registration_ID (0);
SetIsAllowPublish (true);	// Y
SetIsAllowStatistics (true);	// Y
SetIsInProduction (false);
SetIsRegistered (false);	// N
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Registration (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Registration (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Registration (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Registration()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514363458L;
/** Last Updated Timestamp 7/29/2010 1:07:26 PM */
public static long updatedMS = 1280389046669L;
/** AD_Table_ID=625 */
public static int Table_ID;
 // =625;

/** TableName=AD_Registration */
public static String Table_Name="AD_Registration";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(4);
/** AccessLevel
@return 4 - System 
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
StringBuilder sb = new StringBuilder ("X_AD_Registration[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set System Registration.
@param AD_Registration_ID System Registration */
public void SetAD_Registration_ID (int AD_Registration_ID)
{
if (AD_Registration_ID < 1) throw new ArgumentException ("AD_Registration_ID is mandatory.");
Set_ValueNoCheck ("AD_Registration_ID", AD_Registration_ID);
}
/** Get System Registration.
@return System Registration */
public int GetAD_Registration_ID() 
{
Object ii = Get_Value("AD_Registration_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Currency.
@param C_Currency_ID The Currency for this record */
public void SetC_Currency_ID (int C_Currency_ID)
{
if (C_Currency_ID <= 0) Set_Value ("C_Currency_ID", null);
else
Set_Value ("C_Currency_ID", C_Currency_ID);
}
/** Get Currency.
@return The Currency for this record */
public int GetC_Currency_ID() 
{
Object ii = Get_Value("C_Currency_ID");
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
/** Set Industry Info.
@param IndustryInfo Information of the industry (e.g. professional service, distribution of equipment, ..) */
public void SetIndustryInfo (String IndustryInfo)
{
if (IndustryInfo != null && IndustryInfo.Length > 255)
{
log.Warning("Length > 255 - truncated");
IndustryInfo = IndustryInfo.Substring(0,255);
}
Set_Value ("IndustryInfo", IndustryInfo);
}
/** Get Industry Info.
@return Information of the industry (e.g. professional service, distribution of equipment, ..) */
public String GetIndustryInfo() 
{
return (String)Get_Value("IndustryInfo");
}
/** Set Allowed to be Published.
@param IsAllowPublish You allow to publish the information, not just statistical summary info */
public void SetIsAllowPublish (Boolean IsAllowPublish)
{
Set_Value ("IsAllowPublish", IsAllowPublish);
}
/** Get Allowed to be Published.
@return You allow to publish the information, not just statistical summary info */
public Boolean IsAllowPublish() 
{
Object oo = Get_Value("IsAllowPublish");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Maintain Statistics.
@param IsAllowStatistics Maintain general statistics */
public void SetIsAllowStatistics (Boolean IsAllowStatistics)
{
Set_Value ("IsAllowStatistics", IsAllowStatistics);
}
/** Get Maintain Statistics.
@return Maintain general statistics */
public Boolean IsAllowStatistics() 
{
Object oo = Get_Value("IsAllowStatistics");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set In Production.
@param IsInProduction The system is in production */
public void SetIsInProduction (Boolean IsInProduction)
{
Set_Value ("IsInProduction", IsInProduction);
}
/** Get In Production.
@return The system is in production */
public Boolean IsInProduction() 
{
Object oo = Get_Value("IsInProduction");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Registered.
@param IsRegistered The application is registered. */
public void SetIsRegistered (Boolean IsRegistered)
{
Set_ValueNoCheck ("IsRegistered", IsRegistered);
}
/** Get Registered.
@return The application is registered. */
public Boolean IsRegistered() 
{
Object oo = Get_Value("IsRegistered");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
/** Set Platform Info.
@param PlatformInfo Information about Server and Client Platform */
public void SetPlatformInfo (String PlatformInfo)
{
if (PlatformInfo != null && PlatformInfo.Length > 255)
{
log.Warning("Length > 255 - truncated");
PlatformInfo = PlatformInfo.Substring(0,255);
}
Set_Value ("PlatformInfo", PlatformInfo);
}
/** Get Platform Info.
@return Information about Server and Client Platform */
public String GetPlatformInfo() 
{
return (String)Get_Value("PlatformInfo");
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
/** Set Remote Addr.
@param Remote_Addr Remote Address */
public void SetRemote_Addr (String Remote_Addr)
{
if (Remote_Addr != null && Remote_Addr.Length > 60)
{
log.Warning("Length > 60 - truncated");
Remote_Addr = Remote_Addr.Substring(0,60);
}
Set_ValueNoCheck ("Remote_Addr", Remote_Addr);
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
Set_ValueNoCheck ("Remote_Host", Remote_Host);
}
/** Get Remote Host.
@return Remote host Info */
public String GetRemote_Host() 
{
return (String)Get_Value("Remote_Host");
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
/** Set Start Implementation/Production.
@param StartProductionDate The day you started the implementation (if implementing) - or production (went life) with Vienna */
public void SetStartProductionDate (DateTime? StartProductionDate)
{
Set_Value ("StartProductionDate", (DateTime?)StartProductionDate);
}
/** Get Start Implementation/Production.
@return The day you started the implementation (if implementing) - or production (went life) with Vienna */
public DateTime? GetStartProductionDate() 
{
return (DateTime?)Get_Value("StartProductionDate");
}
}

}
