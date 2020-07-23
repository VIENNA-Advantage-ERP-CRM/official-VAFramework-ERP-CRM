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
/** Generated Model for AD_UserDef_Field
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_UserDef_Field : PO
{
public X_AD_UserDef_Field (Context ctx, int AD_UserDef_Field_ID, Trx trxName) : base (ctx, AD_UserDef_Field_ID, trxName)
{
/** if (AD_UserDef_Field_ID == 0)
{
SetAD_Field_ID (0);
SetAD_UserDef_Field_ID (0);
SetAD_UserDef_Tab_ID (0);
}
 */
}
public X_AD_UserDef_Field (Ctx ctx, int AD_UserDef_Field_ID, Trx trxName) : base (ctx, AD_UserDef_Field_ID, trxName)
{
/** if (AD_UserDef_Field_ID == 0)
{
SetAD_Field_ID (0);
SetAD_UserDef_Field_ID (0);
SetAD_UserDef_Tab_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_UserDef_Field (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_UserDef_Field (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_UserDef_Field (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_UserDef_Field()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514365307L;
/** Last Updated Timestamp 7/29/2010 1:07:28 PM */
public static long updatedMS = 1280389048518L;
/** AD_Table_ID=464 */
public static int Table_ID;
 // =464;

/** TableName=AD_UserDef_Field */
public static String Table_Name="AD_UserDef_Field";

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
StringBuilder sb = new StringBuilder ("X_AD_UserDef_Field[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Field.
@param AD_Field_ID Field on a tab in a window */
public void SetAD_Field_ID (int AD_Field_ID)
{
if (AD_Field_ID < 1) throw new ArgumentException ("AD_Field_ID is mandatory.");
Set_Value ("AD_Field_ID", AD_Field_ID);
}
/** Get Field.
@return Field on a tab in a window */
public int GetAD_Field_ID() 
{
Object ii = Get_Value("AD_Field_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_Field_ID().ToString());
}
/** Set User defined Field.
@param AD_UserDef_Field_ID User defined Field */
public void SetAD_UserDef_Field_ID (int AD_UserDef_Field_ID)
{
if (AD_UserDef_Field_ID < 1) throw new ArgumentException ("AD_UserDef_Field_ID is mandatory.");
Set_ValueNoCheck ("AD_UserDef_Field_ID", AD_UserDef_Field_ID);
}
/** Get User defined Field.
@return User defined Field */
public int GetAD_UserDef_Field_ID() 
{
Object ii = Get_Value("AD_UserDef_Field_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Default Logic.
@param DefaultValue Default value hierarchy, separated by ;
 */
public void SetDefaultValue (String DefaultValue)
{
if (DefaultValue != null && DefaultValue.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
DefaultValue = DefaultValue.Substring(0,2000);
}
Set_Value ("DefaultValue", DefaultValue);
}
/** Get Default Logic.
@return Default value hierarchy, separated by ;
 */
public String GetDefaultValue() 
{
return (String)Get_Value("DefaultValue");
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
/** Set Display Length.
@param DisplayLength Length of the display in characters */
public void SetDisplayLength (int DisplayLength)
{
Set_Value ("DisplayLength", DisplayLength);
}
/** Get Display Length.
@return Length of the display in characters */
public int GetDisplayLength() 
{
Object ii = Get_Value("DisplayLength");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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

/** IsMandatoryUI AD_Reference_ID=319 */
public static int ISMANDATORYUI_AD_Reference_ID=319;
/** No = N */
public static String ISMANDATORYUI_No = "N";
/** Yes = Y */
public static String ISMANDATORYUI_Yes = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsIsMandatoryUIValid (String test)
{
return test == null || test.Equals("N") || test.Equals("Y");
}
/** Set Mandatory UI.
@param IsMandatoryUI Data entry is required for data entry in the field */
public void SetIsMandatoryUI (String IsMandatoryUI)
{
if (!IsIsMandatoryUIValid(IsMandatoryUI))
throw new ArgumentException ("IsMandatoryUI Invalid value - " + IsMandatoryUI + " - Reference_ID=319 - N - Y");
if (IsMandatoryUI != null && IsMandatoryUI.Length > 1)
{
log.Warning("Length > 1 - truncated");
IsMandatoryUI = IsMandatoryUI.Substring(0,1);
}
Set_Value ("IsMandatoryUI", IsMandatoryUI);
}
/** Get Mandatory UI.
@return Data entry is required for data entry in the field */
public String GetIsMandatoryUI() 
{
return (String)Get_Value("IsMandatoryUI");
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

/** IsSameLine AD_Reference_ID=319 */
public static int ISSAMELINE_AD_Reference_ID=319;
/** No = N */
public static String ISSAMELINE_No = "N";
/** Yes = Y */
public static String ISSAMELINE_Yes = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsIsSameLineValid (String test)
{
return test == null || test.Equals("N") || test.Equals("Y");
}
/** Set Same Line.
@param IsSameLine Displayed on same line as previous field */
public void SetIsSameLine (String IsSameLine)
{
if (!IsIsSameLineValid(IsSameLine))
throw new ArgumentException ("IsSameLine Invalid value - " + IsSameLine + " - Reference_ID=319 - N - Y");
if (IsSameLine != null && IsSameLine.Length > 1)
{
log.Warning("Length > 1 - truncated");
IsSameLine = IsSameLine.Substring(0,1);
}
Set_Value ("IsSameLine", IsSameLine);
}
/** Get Same Line.
@return Displayed on same line as previous field */
public String GetIsSameLine() 
{
return (String)Get_Value("IsSameLine");
}

/** IsSelectionColumn AD_Reference_ID=319 */
public static int ISSELECTIONCOLUMN_AD_Reference_ID=319;
/** No = N */
public static String ISSELECTIONCOLUMN_No = "N";
/** Yes = Y */
public static String ISSELECTIONCOLUMN_Yes = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsIsSelectionColumnValid (String test)
{
return test == null || test.Equals("N") || test.Equals("Y");
}
/** Set Selection Column.
@param IsSelectionColumn Is this column used for finding rows in windows */
public void SetIsSelectionColumn (String IsSelectionColumn)
{
if (!IsIsSelectionColumnValid(IsSelectionColumn))
throw new ArgumentException ("IsSelectionColumn Invalid value - " + IsSelectionColumn + " - Reference_ID=319 - N - Y");
if (IsSelectionColumn != null && IsSelectionColumn.Length > 1)
{
log.Warning("Length > 1 - truncated");
IsSelectionColumn = IsSelectionColumn.Substring(0,1);
}
Set_Value ("IsSelectionColumn", IsSelectionColumn);
}
/** Get Selection Column.
@return Is this column used for finding rows in windows */
public String GetIsSelectionColumn() 
{
return (String)Get_Value("IsSelectionColumn");
}

/** IsSummaryColumn AD_Reference_ID=319 */
public static int ISSUMMARYCOLUMN_AD_Reference_ID=319;
/** No = N */
public static String ISSUMMARYCOLUMN_No = "N";
/** Yes = Y */
public static String ISSUMMARYCOLUMN_Yes = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsIsSummaryColumnValid (String test)
{
return test == null || test.Equals("N") || test.Equals("Y");
}
/** Set Summary Column.
@param IsSummaryColumn Summary Info Column */
public void SetIsSummaryColumn (String IsSummaryColumn)
{
if (!IsIsSummaryColumnValid(IsSummaryColumn))
throw new ArgumentException ("IsSummaryColumn Invalid value - " + IsSummaryColumn + " - Reference_ID=319 - N - Y");
if (IsSummaryColumn != null && IsSummaryColumn.Length > 1)
{
log.Warning("Length > 1 - truncated");
IsSummaryColumn = IsSummaryColumn.Substring(0,1);
}
Set_Value ("IsSummaryColumn", IsSummaryColumn);
}
/** Get Summary Column.
@return Summary Info Column */
public String GetIsSummaryColumn() 
{
return (String)Get_Value("IsSummaryColumn");
}

/** IsUpdateable AD_Reference_ID=319 */
public static int ISUPDATEABLE_AD_Reference_ID=319;
/** No = N */
public static String ISUPDATEABLE_No = "N";
/** Yes = Y */
public static String ISUPDATEABLE_Yes = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsIsUpdateableValid (String test)
{
return test == null || test.Equals("N") || test.Equals("Y");
}
/** Set Updateable.
@param IsUpdateable Determines, if the field can be updated */
public void SetIsUpdateable (String IsUpdateable)
{
if (!IsIsUpdateableValid(IsUpdateable))
throw new ArgumentException ("IsUpdateable Invalid value - " + IsUpdateable + " - Reference_ID=319 - N - Y");
if (IsUpdateable != null && IsUpdateable.Length > 1)
{
log.Warning("Length > 1 - truncated");
IsUpdateable = IsUpdateable.Substring(0,1);
}
Set_Value ("IsUpdateable", IsUpdateable);
}
/** Get Updateable.
@return Determines, if the field can be updated */
public String GetIsUpdateable() 
{
return (String)Get_Value("IsUpdateable");
}
/** Set Multi-Row Sequence.
@param MRSeqNo Method of ordering fields in Multi-Row (Grid) View;
 lowest number comes first */
public void SetMRSeqNo (int MRSeqNo)
{
Set_Value ("MRSeqNo", MRSeqNo);
}
/** Get Multi-Row Sequence.
@return Method of ordering fields in Multi-Row (Grid) View;
 lowest number comes first */
public int GetMRSeqNo() 
{
Object ii = Get_Value("MRSeqNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Selection Sequence.
@param SelectionSeqNo Sequence in Selection */
public void SetSelectionSeqNo (int SelectionSeqNo)
{
Set_Value ("SelectionSeqNo", SelectionSeqNo);
}
/** Get Selection Sequence.
@return Sequence in Selection */
public int GetSelectionSeqNo() 
{
Object ii = Get_Value("SelectionSeqNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Record Sort No.
@param SortNo Determines in what order the records are displayed */
public void SetSortNo (int SortNo)
{
Set_Value ("SortNo", SortNo);
}
/** Get Record Sort No.
@return Determines in what order the records are displayed */
public int GetSortNo() 
{
Object ii = Get_Value("SortNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Summary Sequence.
@param SummarySeqNo Sequence in Summary */
public void SetSummarySeqNo (int SummarySeqNo)
{
Set_Value ("SummarySeqNo", SummarySeqNo);
}
/** Get Summary Sequence.
@return Sequence in Summary */
public int GetSummarySeqNo() 
{
Object ii = Get_Value("SummarySeqNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Max. Value.
@param ValueMax Maximum Value for a field */
public void SetValueMax (String ValueMax)
{
if (ValueMax != null && ValueMax.Length > 20)
{
log.Warning("Length > 20 - truncated");
ValueMax = ValueMax.Substring(0,20);
}
Set_Value ("ValueMax", ValueMax);
}
/** Get Max. Value.
@return Maximum Value for a field */
public String GetValueMax() 
{
return (String)Get_Value("ValueMax");
}
/** Set Min. Value.
@param ValueMin Minimum Value for a field */
public void SetValueMin (String ValueMin)
{
if (ValueMin != null && ValueMin.Length > 20)
{
log.Warning("Length > 20 - truncated");
ValueMin = ValueMin.Substring(0,20);
}
Set_Value ("ValueMin", ValueMin);
}
/** Get Min. Value.
@return Minimum Value for a field */
public String GetValueMin() 
{
return (String)Get_Value("ValueMin");
}
}

}
