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
/** Generated Model for VAB_RegionState
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_RegionState : PO
{
public X_VAB_RegionState (Context ctx, int VAB_RegionState_ID, Trx trxName) : base (ctx, VAB_RegionState_ID, trxName)
{
/** if (VAB_RegionState_ID == 0)
{
SetVAB_Country_ID (0);
SetVAB_RegionState_ID (0);
SetName (null);
}
 */
}
public X_VAB_RegionState (Ctx ctx, int VAB_RegionState_ID, Trx trxName) : base (ctx, VAB_RegionState_ID, trxName)
{
/** if (VAB_RegionState_ID == 0)
{
SetVAB_Country_ID (0);
SetVAB_RegionState_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_RegionState (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_RegionState (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_RegionState (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_RegionState()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514374538L;
/** Last Updated Timestamp 7/29/2010 1:07:37 PM */
public static long updatedMS = 1280389057749L;
/** VAF_TableView_ID=164 */
public static int Table_ID;
 // =164;

/** TableName=VAB_RegionState */
public static String Table_Name="VAB_RegionState";

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
StringBuilder sb = new StringBuilder ("X_VAB_RegionState[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Country.
@param VAB_Country_ID Country */
public void SetVAB_Country_ID (int VAB_Country_ID)
{
if (VAB_Country_ID < 1) throw new ArgumentException ("VAB_Country_ID is mandatory.");
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
/** Set Region.
@param VAB_RegionState_ID Identifies a geographical Region */
public void SetVAB_RegionState_ID (int VAB_RegionState_ID)
{
if (VAB_RegionState_ID < 1) throw new ArgumentException ("VAB_RegionState_ID is mandatory.");
Set_ValueNoCheck ("VAB_RegionState_ID", VAB_RegionState_ID);
}
/** Get Region.
@return Identifies a geographical Region */
public int GetVAB_RegionState_ID() 
{
Object ii = Get_Value("VAB_RegionState_ID");
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
/** Set Default.
@param IsDefault Default value */
public void SetIsDefault (Boolean IsDefault)
{
Set_Value ("IsDefault", IsDefault);
}
/** Get Default.
@return Default value */
public Boolean IsDefault() 
{
Object oo = Get_Value("IsDefault");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
}

}
