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
/** Generated Model for AD_UserDef_Tab
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_UserDef_Tab : PO
{
public X_AD_UserDef_Tab (Context ctx, int AD_UserDef_Tab_ID, Trx trxName) : base (ctx, AD_UserDef_Tab_ID, trxName)
{
/** if (AD_UserDef_Tab_ID == 0)
{
SetAD_Tab_ID (0);
SetAD_UserDef_Tab_ID (0);
SetAD_UserDef_Win_ID (0);
}
 */
}
public X_AD_UserDef_Tab (Ctx ctx, int AD_UserDef_Tab_ID, Trx trxName) : base (ctx, AD_UserDef_Tab_ID, trxName)
{
/** if (AD_UserDef_Tab_ID == 0)
{
SetAD_Tab_ID (0);
SetAD_UserDef_Tab_ID (0);
SetAD_UserDef_Win_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_UserDef_Tab (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_UserDef_Tab (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_UserDef_Tab (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_UserDef_Tab()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514365338L;
/** Last Updated Timestamp 7/29/2010 1:07:28 PM */
public static long updatedMS = 1280389048549L;
/** AD_Table_ID=466 */
public static int Table_ID;
 // =466;

/** TableName=AD_UserDef_Tab */
public static String Table_Name="AD_UserDef_Tab";

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
StringBuilder sb = new StringBuilder ("X_AD_UserDef_Tab[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Tab.
@param AD_Tab_ID Tab within a Window */
public void SetAD_Tab_ID (int AD_Tab_ID)
{
if (AD_Tab_ID < 1) throw new ArgumentException ("AD_Tab_ID is mandatory.");
Set_Value ("AD_Tab_ID", AD_Tab_ID);
}
/** Get Tab.
@return Tab within a Window */
public int GetAD_Tab_ID() 
{
Object ii = Get_Value("AD_Tab_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_Tab_ID().ToString());
}
/** Set User defined Tab.
@param AD_UserDef_Tab_ID User defined Tab */
public void SetAD_UserDef_Tab_ID (int AD_UserDef_Tab_ID)
{
if (AD_UserDef_Tab_ID < 1) throw new ArgumentException ("AD_UserDef_Tab_ID is mandatory.");
Set_ValueNoCheck ("AD_UserDef_Tab_ID", AD_UserDef_Tab_ID);
}
/** Get User defined Tab.
@return User defined Tab */
public int GetAD_UserDef_Tab_ID() 
{
Object ii = Get_Value("AD_UserDef_Tab_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User defined Window.
@param AD_UserDef_Win_ID User defined Window */
public void SetAD_UserDef_Win_ID (int AD_UserDef_Win_ID)
{
if (AD_UserDef_Win_ID < 1) throw new ArgumentException ("AD_UserDef_Win_ID is mandatory.");
Set_ValueNoCheck ("AD_UserDef_Win_ID", AD_UserDef_Win_ID);
}
/** Get User defined Window.
@return User defined Window */
public int GetAD_UserDef_Win_ID() 
{
Object ii = Get_Value("AD_UserDef_Win_ID");
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
/** Set Display Logic.
@param DisplayLogic If the Field is displayed, the result determines if the field is actually displayed */
public void SetDisplayLogic (String DisplayLogic)
{
if (DisplayLogic != null && DisplayLogic.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
DisplayLogic = DisplayLogic.Substring(0,2000);
}
Set_Value ("DisplayLogic", DisplayLogic);
}
/** Get Display Logic.
@return If the Field is displayed, the result determines if the field is actually displayed */
public String GetDisplayLogic() 
{
return (String)Get_Value("DisplayLogic");
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

/** IsDisplayed AD_Reference_ID=319 */
public static int ISDISPLAYED_AD_Reference_ID=319;
/** No = N */
public static String ISDISPLAYED_No = "N";
/** Yes = Y */
public static String ISDISPLAYED_Yes = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsIsDisplayedValid (String test)
{
return test == null || test.Equals("N") || test.Equals("Y");
}
/** Set Displayed.
@param IsDisplayed Determines, if this field is displayed */
public void SetIsDisplayed (String IsDisplayed)
{
if (!IsIsDisplayedValid(IsDisplayed))
throw new ArgumentException ("IsDisplayed Invalid value - " + IsDisplayed + " - Reference_ID=319 - N - Y");
if (IsDisplayed != null && IsDisplayed.Length > 1)
{
log.Warning("Length > 1 - truncated");
IsDisplayed = IsDisplayed.Substring(0,1);
}
Set_Value ("IsDisplayed", IsDisplayed);
}
/** Get Displayed.
@return Determines, if this field is displayed */
public String GetIsDisplayed() 
{
return (String)Get_Value("IsDisplayed");
}

/** IsInsertRecord AD_Reference_ID=319 */
public static int ISINSERTRECORD_AD_Reference_ID=319;
/** No = N */
public static String ISINSERTRECORD_No = "N";
/** Yes = Y */
public static String ISINSERTRECORD_Yes = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsIsInsertRecordValid (String test)
{
return test == null || test.Equals("N") || test.Equals("Y");
}
/** Set Insert Record.
@param IsInsertRecord The user can insert a new Record */
public void SetIsInsertRecord (String IsInsertRecord)
{
if (!IsIsInsertRecordValid(IsInsertRecord))
throw new ArgumentException ("IsInsertRecord Invalid value - " + IsInsertRecord + " - Reference_ID=319 - N - Y");
if (IsInsertRecord != null && IsInsertRecord.Length > 1)
{
log.Warning("Length > 1 - truncated");
IsInsertRecord = IsInsertRecord.Substring(0,1);
}
Set_Value ("IsInsertRecord", IsInsertRecord);
}
/** Get Insert Record.
@return The user can insert a new Record */
public String GetIsInsertRecord() 
{
return (String)Get_Value("IsInsertRecord");
}

/** IsMultiRowOnly AD_Reference_ID=319 */
public static int ISMULTIROWONLY_AD_Reference_ID=319;
/** No = N */
public static String ISMULTIROWONLY_No = "N";
/** Yes = Y */
public static String ISMULTIROWONLY_Yes = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsIsMultiRowOnlyValid (String test)
{
return test == null || test.Equals("N") || test.Equals("Y");
}
/** Set Multi Row Only.
@param IsMultiRowOnly This applies to Multi-Row view only */
public void SetIsMultiRowOnly (String IsMultiRowOnly)
{
if (!IsIsMultiRowOnlyValid(IsMultiRowOnly))
throw new ArgumentException ("IsMultiRowOnly Invalid value - " + IsMultiRowOnly + " - Reference_ID=319 - N - Y");
if (IsMultiRowOnly != null && IsMultiRowOnly.Length > 1)
{
log.Warning("Length > 1 - truncated");
IsMultiRowOnly = IsMultiRowOnly.Substring(0,1);
}
Set_Value ("IsMultiRowOnly", IsMultiRowOnly);
}
/** Get Multi Row Only.
@return This applies to Multi-Row view only */
public String GetIsMultiRowOnly() 
{
return (String)Get_Value("IsMultiRowOnly");
}

/** IsReadOnly AD_Reference_ID=319 */
public static int ISREADONLY_AD_Reference_ID=319;
/** No = N */
public static String ISREADONLY_No = "N";
/** Yes = Y */
public static String ISREADONLY_Yes = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsIsReadOnlyValid (String test)
{
return test == null || test.Equals("N") || test.Equals("Y");
}
/** Set Read Only.
@param IsReadOnly Field is read only */
public void SetIsReadOnly (String IsReadOnly)
{
if (!IsIsReadOnlyValid(IsReadOnly))
throw new ArgumentException ("IsReadOnly Invalid value - " + IsReadOnly + " - Reference_ID=319 - N - Y");
if (IsReadOnly != null && IsReadOnly.Length > 1)
{
log.Warning("Length > 1 - truncated");
IsReadOnly = IsReadOnly.Substring(0,1);
}
Set_Value ("IsReadOnly", IsReadOnly);
}
/** Get Read Only.
@return Field is read only */
public String GetIsReadOnly() 
{
return (String)Get_Value("IsReadOnly");
}

/** IsSingleRow AD_Reference_ID=319 */
public static int ISSINGLEROW_AD_Reference_ID=319;
/** No = N */
public static String ISSINGLEROW_No = "N";
/** Yes = Y */
public static String ISSINGLEROW_Yes = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsIsSingleRowValid (String test)
{
return test == null || test.Equals("N") || test.Equals("Y");
}
/** Set Single Row Layout.
@param IsSingleRow Default for toggle between Single- and Multi-Row (Grid) Layout */
public void SetIsSingleRow (String IsSingleRow)
{
if (!IsIsSingleRowValid(IsSingleRow))
throw new ArgumentException ("IsSingleRow Invalid value - " + IsSingleRow + " - Reference_ID=319 - N - Y");
if (IsSingleRow != null && IsSingleRow.Length > 1)
{
log.Warning("Length > 1 - truncated");
IsSingleRow = IsSingleRow.Substring(0,1);
}
Set_Value ("IsSingleRow", IsSingleRow);
}
/** Get Single Row Layout.
@return Default for toggle between Single- and Multi-Row (Grid) Layout */
public String GetIsSingleRow() 
{
return (String)Get_Value("IsSingleRow");
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name != null && Name.Length > 60)
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
/** Set Read Only Logic.
@param ReadOnlyLogic Logic to determine if field is read only (applies only when field is read-write) */
public void SetReadOnlyLogic (String ReadOnlyLogic)
{
if (ReadOnlyLogic != null && ReadOnlyLogic.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
ReadOnlyLogic = ReadOnlyLogic.Substring(0,2000);
}
Set_Value ("ReadOnlyLogic", ReadOnlyLogic);
}
/** Get Read Only Logic.
@return Logic to determine if field is read only (applies only when field is read-write) */
public String GetReadOnlyLogic() 
{
return (String)Get_Value("ReadOnlyLogic");
}
/** Set Sequence.
@param SeqNo Method of ordering elements;
 lowest number comes first */
public void SetSeqNo (int SeqNo)
{
Set_Value ("SeqNo", SeqNo);
}
/** Get Sequence.
@return Method of ordering elements;
 lowest number comes first */
public int GetSeqNo() 
{
Object ii = Get_Value("SeqNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
