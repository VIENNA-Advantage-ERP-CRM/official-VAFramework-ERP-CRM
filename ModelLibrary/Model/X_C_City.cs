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
/** Generated Model for C_City
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_City : PO
{
public X_C_City (Context ctx, int C_City_ID, Trx trxName) : base (ctx, C_City_ID, trxName)
{
/** if (C_City_ID == 0)
{
SetC_City_ID (0);
SetName (null);
}
 */
}
public X_C_City (Ctx ctx, int C_City_ID, Trx trxName) : base (ctx, C_City_ID, trxName)
{
/** if (C_City_ID == 0)
{
SetC_City_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_City (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_City (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_City (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_City()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514371247L;
/** Last Updated Timestamp 7/29/2010 1:07:34 PM */
public static long updatedMS = 1280389054458L;
/** AD_Table_ID=186 */
public static int Table_ID;
 // =186;

/** TableName=C_City */
public static String Table_Name="C_City";

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
StringBuilder sb = new StringBuilder ("X_C_City[").Append(Get_ID()).Append("]");
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
@param C_City_ID City */
public void SetC_City_ID (int C_City_ID)
{
if (C_City_ID < 1) throw new ArgumentException ("C_City_ID is mandatory.");
Set_ValueNoCheck ("C_City_ID", C_City_ID);
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
if (C_Country_ID <= 0) Set_ValueNoCheck ("C_Country_ID", null);
else
Set_ValueNoCheck ("C_Country_ID", C_Country_ID);
}
/** Get Country.
@return Country */
public int GetC_Country_ID() 
{
Object ii = Get_Value("C_Country_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** C_Region_ID AD_Reference_ID=157 */
public static int C_REGION_ID_AD_Reference_ID=157;
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
