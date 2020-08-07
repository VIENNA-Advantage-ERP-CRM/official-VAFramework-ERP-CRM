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
/** Generated Model for AD_ChangeLog
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_ChangeLog : PO
{
public X_AD_ChangeLog (Context ctx, int AD_ChangeLog_ID, Trx trxName) : base (ctx, AD_ChangeLog_ID, trxName)
{
/** if (AD_ChangeLog_ID == 0)
{
SetAD_ChangeLog_ID (0);
SetAD_Column_ID (0);
SetAD_Session_ID (0);
SetAD_Table_ID (0);
SetIsCustomization (false);
SetRecord_ID (0);
}
 */
}
public X_AD_ChangeLog (Ctx ctx, int AD_ChangeLog_ID, Trx trxName) : base (ctx, AD_ChangeLog_ID, trxName)
{
/** if (AD_ChangeLog_ID == 0)
{
SetAD_ChangeLog_ID (0);
SetAD_Column_ID (0);
SetAD_Session_ID (0);
SetAD_Table_ID (0);
SetIsCustomization (false);
SetRecord_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ChangeLog (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ChangeLog (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ChangeLog (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_ChangeLog()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514360778L;
/** Last Updated Timestamp 7/29/2010 1:07:23 PM */
public static long updatedMS = 1280389043989L;
/** AD_Table_ID=580 */
public static int Table_ID;
 // =580;

/** TableName=AD_ChangeLog */
public static String Table_Name="AD_ChangeLog";

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
StringBuilder sb = new StringBuilder ("X_AD_ChangeLog[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Change Log.
@param AD_ChangeLog_ID Log of data changes */
public void SetAD_ChangeLog_ID (int AD_ChangeLog_ID)
{
if (AD_ChangeLog_ID < 1) throw new ArgumentException ("AD_ChangeLog_ID is mandatory.");
Set_ValueNoCheck ("AD_ChangeLog_ID", AD_ChangeLog_ID);
}
/** Get Change Log.
@return Log of data changes */
public int GetAD_ChangeLog_ID() 
{
Object ii = Get_Value("AD_ChangeLog_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Column.
@param AD_Column_ID Column in the table */
public void SetAD_Column_ID (int AD_Column_ID)
{
if (AD_Column_ID < 1) throw new ArgumentException ("AD_Column_ID is mandatory.");
Set_ValueNoCheck ("AD_Column_ID", AD_Column_ID);
}
/** Get Column.
@return Column in the table */
public int GetAD_Column_ID() 
{
Object ii = Get_Value("AD_Column_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Role.
@param AD_Role_ID Responsibility Role */
public void SetAD_Role_ID (int AD_Role_ID)
{
if (AD_Role_ID <= 0) Set_ValueNoCheck ("AD_Role_ID", null);
else
Set_ValueNoCheck ("AD_Role_ID", AD_Role_ID);
}
/** Get Role.
@return Responsibility Role */
public int GetAD_Role_ID() 
{
Object ii = Get_Value("AD_Role_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Session.
@param AD_Session_ID User Session Online or Web */
public void SetAD_Session_ID (int AD_Session_ID)
{
if (AD_Session_ID < 1) throw new ArgumentException ("AD_Session_ID is mandatory.");
Set_ValueNoCheck ("AD_Session_ID", AD_Session_ID);
}
/** Get Session.
@return User Session Online or Web */
public int GetAD_Session_ID() 
{
Object ii = Get_Value("AD_Session_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_Session_ID().ToString());
}
/** Set Table.
@param AD_Table_ID Database Table information */
public void SetAD_Table_ID (int AD_Table_ID)
{
if (AD_Table_ID < 1) throw new ArgumentException ("AD_Table_ID is mandatory.");
Set_ValueNoCheck ("AD_Table_ID", AD_Table_ID);
}
/** Get Table.
@return Database Table information */
public int GetAD_Table_ID() 
{
Object ii = Get_Value("AD_Table_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** ChangeLogType AD_Reference_ID=430 */
public static int CHANGELOGTYPE_AD_Reference_ID=430;
/** Delete = D */
public static String CHANGELOGTYPE_Delete = "D";
/** Insert = I */
public static String CHANGELOGTYPE_Insert = "I";
/** Update = U */
public static String CHANGELOGTYPE_Update = "U";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsChangeLogTypeValid (String test)
{
return test == null || test.Equals("D") || test.Equals("I") || test.Equals("U");
}
/** Set Change Log Type.
@param ChangeLogType Type of change */
public void SetChangeLogType (String ChangeLogType)
{
if (!IsChangeLogTypeValid(ChangeLogType))
throw new ArgumentException ("ChangeLogType Invalid value - " + ChangeLogType + " - Reference_ID=430 - D - I - U");
if (ChangeLogType != null && ChangeLogType.Length > 1)
{
log.Warning("Length > 1 - truncated");
ChangeLogType = ChangeLogType.Substring(0,1);
}
Set_ValueNoCheck ("ChangeLogType", ChangeLogType);
}
/** Get Change Log Type.
@return Type of change */
public String GetChangeLogType() 
{
return (String)Get_Value("ChangeLogType");
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
/** Set Customization.
@param IsCustomization The change is a customization of the data dictionary and can be applied after Migration */
public void SetIsCustomization (Boolean IsCustomization)
{
Set_Value ("IsCustomization", IsCustomization);
}
/** Get Customization.
@return The change is a customization of the data dictionary and can be applied after Migration */
public Boolean IsCustomization() 
{
Object oo = Get_Value("IsCustomization");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set New Value.
@param NewValue New field value */
public void SetNewValue (String NewValue)
{
if (NewValue != null && NewValue.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
NewValue = NewValue.Substring(0,2000);
}
Set_ValueNoCheck ("NewValue", NewValue);
}
/** Get New Value.
@return New field value */
public String GetNewValue() 
{
return (String)Get_Value("NewValue");
}
/** Set Old Value.
@param OldValue The old file data */
public void SetOldValue (String OldValue)
{
if (OldValue != null && OldValue.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
OldValue = OldValue.Substring(0,2000);
}
Set_ValueNoCheck ("OldValue", OldValue);
}
/** Get Old Value.
@return The old file data */
public String GetOldValue() 
{
return (String)Get_Value("OldValue");
}
/** Set Record Key.
@param Record2_ID Record Key (where clause) */
public void SetRecord2_ID (String Record2_ID)
{
if (Record2_ID != null && Record2_ID.Length > 255)
{
log.Warning("Length > 255 - truncated");
Record2_ID = Record2_ID.Substring(0,255);
}
Set_ValueNoCheck ("Record2_ID", Record2_ID);
}
/** Get Record Key.
@return Record Key (where clause) */
public String GetRecord2_ID() 
{
return (String)Get_Value("Record2_ID");
}
/** Set Record ID.
@param Record_ID Direct internal record ID */
public void SetRecord_ID (int Record_ID)
{
if (Record_ID < 0) throw new ArgumentException ("Record_ID is mandatory.");
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
/** Set Redo.
@param Redo Redo */
public void SetRedo (String Redo)
{
if (Redo != null && Redo.Length > 1)
{
log.Warning("Length > 1 - truncated");
Redo = Redo.Substring(0,1);
}
Set_Value ("Redo", Redo);
}
/** Get Redo.
@return Redo */
public String GetRedo() 
{
return (String)Get_Value("Redo");
}
/** Set Transaction.
@param TrxName Name of the transaction */
public void SetTrxName (String TrxName)
{
if (TrxName != null && TrxName.Length > 60)
{
log.Warning("Length > 60 - truncated");
TrxName = TrxName.Substring(0,60);
}
Set_ValueNoCheck ("TrxName", TrxName);
}
/** Get Transaction.
@return Name of the transaction */
public String GetTrxName() 
{
return (String)Get_Value("TrxName");
}
/** Set Undo.
@param Undo Undo */
public void SetUndo (String Undo)
{
if (Undo != null && Undo.Length > 1)
{
log.Warning("Length > 1 - truncated");
Undo = Undo.Substring(0,1);
}
Set_Value ("Undo", Undo);
}
/** Get Undo.
@return Undo */
public String GetUndo() 
{
return (String)Get_Value("Undo");
}
}

}
