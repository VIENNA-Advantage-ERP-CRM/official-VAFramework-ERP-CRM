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
/** Generated Model for VAB_City
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_City : PO
{
public X_VAB_City (Context ctx, int VAB_City_ID, Trx trxName) : base (ctx, VAB_City_ID, trxName)
{
/** if (VAB_City_ID == 0)
{
SetVAB_City_ID (0);
SetName (null);
}
 */
}
public X_VAB_City (Ctx ctx, int VAB_City_ID, Trx trxName) : base (ctx, VAB_City_ID, trxName)
{
/** if (VAB_City_ID == 0)
{
SetVAB_City_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_City (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_City (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_City (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_City()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514371247L;
/** Last Updated Timestamp 7/29/2010 1:07:34 PM */
public static long updatedMS = 1280389054458L;
/** VAF_TableView_ID=186 */
public static int Table_ID;
 // =186;

/** TableName=VAB_City */
public static String Table_Name="VAB_City";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(6);
/** AccessLevel
@return 6 - System - Client 
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
StringBuilder sb = new StringBuilder ("X_VAB_City[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Area Code.
@param AreaCode Phone Area Code */
public void SetAreaCode (String AreaCode)
{
if (AreaCode != null && AreaCode.Length > 10)
{
log.Warning("Length > 10 - truncated");
AreaCode = AreaCode.Substring(0,10);
}
Set_Value ("AreaCode", AreaCode);
}
/** Get Area Code.
@return Phone Area Code */
public String GetAreaCode() 
{
return (String)Get_Value("AreaCode");
}
/** Set City.
@param VAB_City_ID City */
public void SetVAB_City_ID (int VAB_City_ID)
{
if (VAB_City_ID < 1) throw new ArgumentException ("VAB_City_ID is mandatory.");
Set_ValueNoCheck ("VAB_City_ID", VAB_City_ID);
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
@param VAB_Country_ID Country */
public void SetVAB_Country_ID (int VAB_Country_ID)
{
if (VAB_Country_ID <= 0) Set_ValueNoCheck ("VAB_Country_ID", null);
else
Set_ValueNoCheck ("VAB_Country_ID", VAB_Country_ID);
}
/** Get Country.
@return Country */
public int GetVAB_Country_ID() 
{
Object ii = Get_Value("VAB_Country_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAB_RegionState_ID VAF_Control_Ref_ID=157 */
public static int VAB_REGIONSTATE_ID_VAF_Control_Ref_ID=157;
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
/** Set Coordinates.
@param Coordinates Location coordinate */
public void SetCoordinates (String Coordinates)
{
if (Coordinates != null && Coordinates.Length > 15)
{
log.Warning("Length > 15 - truncated");
Coordinates = Coordinates.Substring(0,15);
}
Set_Value ("Coordinates", Coordinates);
}
/** Get Coordinates.
@return Location coordinate */
public String GetCoordinates() 
{
return (String)Get_Value("Coordinates");
}
/** Set Locode.
@param Locode Location code - UN/LOCODE */
public void SetLocode (String Locode)
{
if (Locode != null && Locode.Length > 10)
{
log.Warning("Length > 10 - truncated");
Locode = Locode.Substring(0,10);
}
Set_Value ("Locode", Locode);
}
/** Get Locode.
@return Location code - UN/LOCODE */
public String GetLocode() 
{
return (String)Get_Value("Locode");
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name == null) throw new ArgumentException ("Name is mandatory.");
if (Name.Length > 60)
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
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetName());
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
}

}
