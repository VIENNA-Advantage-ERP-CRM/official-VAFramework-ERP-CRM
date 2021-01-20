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
/** Generated Model for VAF_PageShortcut
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_PageShortcut : PO
{
public X_VAF_PageShortcut (Context ctx, int VAF_PageShortcut_ID, Trx trxName) : base (ctx, VAF_PageShortcut_ID, trxName)
{
/** if (VAF_PageShortcut_ID == 0)
{
SetVAF_PageShortcut_ID (0);
SetVAF_Page_ID (0);
SetName (null);
SetValue (null);
}
 */
}
public X_VAF_PageShortcut (Ctx ctx, int VAF_PageShortcut_ID, Trx trxName) : base (ctx, VAF_PageShortcut_ID, trxName)
{
/** if (VAF_PageShortcut_ID == 0)
{
SetVAF_PageShortcut_ID (0);
SetVAF_Page_ID (0);
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
public X_VAF_PageShortcut (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_PageShortcut (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_PageShortcut (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_PageShortcut()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514361389L;
/** Last Updated Timestamp 7/29/2010 1:07:24 PM */
public static long updatedMS = 1280389044600L;
/** VAF_TableView_ID=908 */
public static int Table_ID;
 // =908;

/** TableName=VAF_PageShortcut */
public static String Table_Name="VAF_PageShortcut";

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
StringBuilder sb = new StringBuilder ("X_VAF_PageShortcut[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Form Shortcut.
@param VAF_PageShortcut_ID Form Shortcut */
public void SetVAF_PageShortcut_ID (int VAF_PageShortcut_ID)
{
if (VAF_PageShortcut_ID < 1) throw new ArgumentException ("VAF_PageShortcut_ID is mandatory.");
Set_ValueNoCheck ("VAF_PageShortcut_ID", VAF_PageShortcut_ID);
}
/** Get Form Shortcut.
@return Form Shortcut */
public int GetVAF_PageShortcut_ID() 
{
Object ii = Get_Value("VAF_PageShortcut_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Special Form.
@param VAF_Page_ID Special Form */
public void SetVAF_Page_ID (int VAF_Page_ID)
{
if (VAF_Page_ID < 1) throw new ArgumentException ("VAF_Page_ID is mandatory.");
Set_Value ("VAF_Page_ID", VAF_Page_ID);
}
/** Get Special Form.
@return Special Form */
public int GetVAF_Page_ID() 
{
Object ii = Get_Value("VAF_Page_ID");
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
/** Set Comment.
@param Help Comment, Help or Hint */
public void SetHelp (String Help)
{
if (Help != null && Help.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Help = Help.Substring(0,2000);
}
Set_Value ("Help", Help);
}
/** Get Comment.
@return Comment, Help or Hint */
public String GetHelp() 
{
return (String)Get_Value("Help");
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
