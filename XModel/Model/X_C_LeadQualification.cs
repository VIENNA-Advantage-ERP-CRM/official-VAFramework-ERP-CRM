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
/** Generated Model for VAB_Lead_Qua
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_Lead_Qua : PO
{
public X_VAB_Lead_Qua (Context ctx, int VAB_Lead_Qua_ID, Trx trxName) : base (ctx, VAB_Lead_Qua_ID, trxName)
{
/** if (VAB_Lead_Qua_ID == 0)
{
SetVAB_Lead_Qua_ID (0);
SetIsQualified (false);
SetName (null);
SetValue (null);
}
 */
}
public X_VAB_Lead_Qua (Ctx ctx, int VAB_Lead_Qua_ID, Trx trxName) : base (ctx, VAB_Lead_Qua_ID, trxName)
{
/** if (VAB_Lead_Qua_ID == 0)
{
SetVAB_Lead_Qua_ID (0);
SetIsQualified (false);
SetName (null);
SetValue (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_Lead_Qua (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_Lead_Qua (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_Lead_Qua (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_Lead_Qua()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514373034L;
/** Last Updated Timestamp 7/29/2010 1:07:36 PM */
public static long updatedMS = 1280389056245L;
/** VAF_TableView_ID=925 */
public static int Table_ID;
 // =925;

/** TableName=VAB_Lead_Qua */
public static String Table_Name="VAB_Lead_Qua";

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
StringBuilder sb = new StringBuilder ("X_VAB_Lead_Qua[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Lead Qualification.
@param VAB_Lead_Qua_ID Lead Qualification evaluation */
public void SetVAB_Lead_Qua_ID (int VAB_Lead_Qua_ID)
{
if (VAB_Lead_Qua_ID < 1) throw new ArgumentException ("VAB_Lead_Qua_ID is mandatory.");
Set_ValueNoCheck ("VAB_Lead_Qua_ID", VAB_Lead_Qua_ID);
}
/** Get Lead Qualification.
@return Lead Qualification evaluation */
public int GetVAB_Lead_Qua_ID() 
{
Object ii = Get_Value("VAB_Lead_Qua_ID");
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
/** Set Qualified.
@param IsQualified Is the Lead Qualified */
public void SetIsQualified (Boolean IsQualified)
{
Set_Value ("IsQualified", IsQualified);
}
/** Get Qualified.
@return Is the Lead Qualified */
public Boolean IsQualified() 
{
Object oo = Get_Value("IsQualified");
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
/** Set Search Key.
@param Value Search key for the record in the format required - must be unique */
public void SetValue (String Value)
{
if (Value == null) throw new ArgumentException ("Value is mandatory.");
if (Value.Length > 40)
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
}

}
