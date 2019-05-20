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
/** Generated Model for AD_Tab
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Tab : PO
{
public X_AD_Tab (Context ctx, int AD_Tab_ID, Trx trxName) : base (ctx, AD_Tab_ID, trxName)
{
/** if (AD_Tab_ID == 0)
{
SetAD_Tab_ID (0);
SetAD_Table_ID (0);
SetAD_Window_ID (0);
SetEntityType (null);	// U
SetHasTree (false);
SetIsAdvancedTab (false);	// N
SetIsDisplayed (true);	// Y
SetIsInsertRecord (true);	// Y
SetIsReadOnly (false);
SetIsSingleRow (false);
SetIsSortTab (false);	// N
SetIsTranslationTab (false);
SetName (null);
SetSeqNo (0);	// @SQL=SELECT COALESCE(MAX(SeqNo),0)+10 AS DefaultValue FROM AD_Tab WHERE AD_Window_ID=@AD_Window_ID@
SetTabLevel (0);
}
 */
}
public X_AD_Tab (Ctx ctx, int AD_Tab_ID, Trx trxName) : base (ctx, AD_Tab_ID, trxName)
{
/** if (AD_Tab_ID == 0)
{
SetAD_Tab_ID (0);
SetAD_Table_ID (0);
SetAD_Window_ID (0);
SetEntityType (null);	// U
SetHasTree (false);
SetIsAdvancedTab (false);	// N
SetIsDisplayed (true);	// Y
SetIsInsertRecord (true);	// Y
SetIsReadOnly (false);
SetIsSingleRow (false);
SetIsSortTab (false);	// N
SetIsTranslationTab (false);
SetName (null);
SetSeqNo (0);	// @SQL=SELECT COALESCE(MAX(SeqNo),0)+10 AS DefaultValue FROM AD_Tab WHERE AD_Window_ID=@AD_Window_ID@
SetTabLevel (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Tab (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Tab (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Tab (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Tab()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514364273L;
/** Last Updated Timestamp 7/29/2010 1:07:27 PM */
public static long updatedMS = 1280389047484L;
/** AD_Table_ID=106 */
public static int Table_ID;
 // =106;

/** TableName=AD_Tab */
public static String Table_Name="AD_Tab";

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
StringBuilder sb = new StringBuilder ("X_AD_Tab[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** AD_ColumnSortOrder_ID AD_Reference_ID=257 */
public static int AD_COLUMNSORTORDER_ID_AD_Reference_ID=257;
/** Set Order Column.
@param AD_ColumnSortOrder_ID Column determining the order */
public void SetAD_ColumnSortOrder_ID (int AD_ColumnSortOrder_ID)
{
if (AD_ColumnSortOrder_ID <= 0) Set_Value ("AD_ColumnSortOrder_ID", null);
else
Set_Value ("AD_ColumnSortOrder_ID", AD_ColumnSortOrder_ID);
}
/** Get Order Column.
@return Column determining the order */
public int GetAD_ColumnSortOrder_ID() 
{
Object ii = Get_Value("AD_ColumnSortOrder_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** AD_ColumnSortYesNo_ID AD_Reference_ID=258 */
public static int AD_COLUMNSORTYESNO_ID_AD_Reference_ID=258;
/** Set Included Column.
@param AD_ColumnSortYesNo_ID Column determining if a Table Column is included in Ordering */
public void SetAD_ColumnSortYesNo_ID (int AD_ColumnSortYesNo_ID)
{
if (AD_ColumnSortYesNo_ID <= 0) Set_Value ("AD_ColumnSortYesNo_ID", null);
else
Set_Value ("AD_ColumnSortYesNo_ID", AD_ColumnSortYesNo_ID);
}
/** Get Included Column.
@return Column determining if a Table Column is included in Ordering */
public int GetAD_ColumnSortYesNo_ID() 
{
Object ii = Get_Value("AD_ColumnSortYesNo_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Column.
@param AD_Column_ID Column in the table */
public void SetAD_Column_ID (int AD_Column_ID)
{
if (AD_Column_ID <= 0) Set_Value ("AD_Column_ID", null);
else
Set_Value ("AD_Column_ID", AD_Column_ID);
}
/** Get Column.
@return Column in the table */
public int GetAD_Column_ID() 
{
Object ii = Get_Value("AD_Column_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Context Area.
@param AD_CtxArea_ID Business Domain Area Terminology */
public void SetAD_CtxArea_ID (int AD_CtxArea_ID)
{
if (AD_CtxArea_ID <= 0) Set_Value ("AD_CtxArea_ID", null);
else
Set_Value ("AD_CtxArea_ID", AD_CtxArea_ID);
}
/** Get Context Area.
@return Business Domain Area Terminology */
public int GetAD_CtxArea_ID() 
{
Object ii = Get_Value("AD_CtxArea_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Image.
@param AD_Image_ID Image or Icon */
public void SetAD_Image_ID (int AD_Image_ID)
{
if (AD_Image_ID <= 0) Set_Value ("AD_Image_ID", null);
else
Set_Value ("AD_Image_ID", AD_Image_ID);
}
/** Get Image.
@return Image or Icon */
public int GetAD_Image_ID() 
{
Object ii = Get_Value("AD_Image_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Process.
@param AD_Process_ID Process or Report */
public void SetAD_Process_ID (int AD_Process_ID)
{
if (AD_Process_ID <= 0) Set_Value ("AD_Process_ID", null);
else
Set_Value ("AD_Process_ID", AD_Process_ID);
}
/** Get Process.
@return Process or Report */
public int GetAD_Process_ID() 
{
Object ii = Get_Value("AD_Process_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Tab.
@param AD_Tab_ID Tab within a Window */
public void SetAD_Tab_ID (int AD_Tab_ID)
{
if (AD_Tab_ID < 1) throw new ArgumentException ("AD_Tab_ID is mandatory.");
Set_ValueNoCheck ("AD_Tab_ID", AD_Tab_ID);
}
/** Get Tab.
@return Tab within a Window */
public int GetAD_Tab_ID() 
{
Object ii = Get_Value("AD_Tab_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Table.
@param AD_Table_ID Database Table information */
public void SetAD_Table_ID (int AD_Table_ID)
{
if (AD_Table_ID < 1) throw new ArgumentException ("AD_Table_ID is mandatory.");
Set_Value ("AD_Table_ID", AD_Table_ID);
}
/** Get Table.
@return Database Table information */
public int GetAD_Table_ID() 
{
Object ii = Get_Value("AD_Table_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Window.
@param AD_Window_ID Data entry or display window */
public void SetAD_Window_ID (int AD_Window_ID)
{
if (AD_Window_ID < 1) throw new ArgumentException ("AD_Window_ID is mandatory.");
Set_ValueNoCheck ("AD_Window_ID", AD_Window_ID);
}
/** Get Window.
@return Data entry or display window */
public int GetAD_Window_ID() 
{
Object ii = Get_Value("AD_Window_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_Window_ID().ToString());
}
/** Set Commit Warning.
@param CommitWarning Warning displayed when saving */
public void SetCommitWarning (String CommitWarning)
{
if (CommitWarning != null && CommitWarning.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
CommitWarning = CommitWarning.Substring(0,2000);
}
Set_Value ("CommitWarning", CommitWarning);
}
/** Get Commit Warning.
@return Warning displayed when saving */
public String GetCommitWarning() 
{
return (String)Get_Value("CommitWarning");
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

/** EntityType AD_Reference_ID=389 */
public static int ENTITYTYPE_AD_Reference_ID=389;
/** Set Entity Type.
@param EntityType Dictionary Entity Type;
 Determines ownership and synchronization */
public void SetEntityType (String EntityType)
{
if (EntityType.Length > 4)
{
log.Warning("Length > 4 - truncated");
EntityType = EntityType.Substring(0,4);
}
Set_Value ("EntityType", EntityType);
}
/** Get Entity Type.
@return Dictionary Entity Type;
 Determines ownership and synchronization */
public String GetEntityType() 
{
return (String)Get_Value("EntityType");
}
/** Set Has Tree.
@param HasTree Window has Tree Graph */
public void SetHasTree (Boolean HasTree)
{
Set_Value ("HasTree", HasTree);
}
/** Get Has Tree.
@return Window has Tree Graph */
public Boolean IsHasTree() 
{
Object oo = Get_Value("HasTree");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
/** Set Import Fields.
@param ImportFields Create Fields from Table Columns */
public void SetImportFields (String ImportFields)
{
if (ImportFields != null && ImportFields.Length > 1)
{
log.Warning("Length > 1 - truncated");
ImportFields = ImportFields.Substring(0,1);
}
Set_Value ("ImportFields", ImportFields);
}
/** Get Import Fields.
@return Create Fields from Table Columns */
public String GetImportFields() 
{
return (String)Get_Value("ImportFields");
}

/** Included_Tab_ID AD_Reference_ID=278 */
public static int INCLUDED_TAB_ID_AD_Reference_ID=278;
/** Set Included Tab.
@param Included_Tab_ID Included Tab in this Tab (Master Detail) */
public void SetIncluded_Tab_ID (int Included_Tab_ID)
{
if (Included_Tab_ID <= 0) Set_Value ("Included_Tab_ID", null);
else
Set_Value ("Included_Tab_ID", Included_Tab_ID);
}
/** Get Included Tab.
@return Included Tab in this Tab (Master Detail) */
public int GetIncluded_Tab_ID() 
{
Object ii = Get_Value("Included_Tab_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Advanced Tab.
@param IsAdvancedTab This Tab contains advanced Functionality */
public void SetIsAdvancedTab (Boolean IsAdvancedTab)
{
Set_Value ("IsAdvancedTab", IsAdvancedTab);
}
/** Get Advanced Tab.
@return This Tab contains advanced Functionality */
public Boolean IsAdvancedTab() 
{
Object oo = Get_Value("IsAdvancedTab");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set IsAllowLetter.
@param IsAllowLetter IsAllowLetter */
public void SetIsAllowLetter (Boolean IsAllowLetter)
{
Set_Value ("IsAllowLetter", IsAllowLetter);
}
/** Get IsAllowLetter.
@return IsAllowLetter */
public Boolean IsAllowLetter() 
{
Object oo = Get_Value("IsAllowLetter");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Displayed.
@param IsDisplayed Determines, if this field is displayed */
public void SetIsDisplayed (Boolean IsDisplayed)
{
Set_Value ("IsDisplayed", IsDisplayed);
}
/** Get Displayed.
@return Determines, if this field is displayed */
public Boolean IsDisplayed() 
{
Object oo = Get_Value("IsDisplayed");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Accounting Tab.
@param IsInfoTab This Tab contains accounting information */
public void SetIsInfoTab (Boolean IsInfoTab)
{
Set_Value ("IsInfoTab", IsInfoTab);
}
/** Get Accounting Tab.
@return This Tab contains accounting information */
public Boolean IsInfoTab() 
{
Object oo = Get_Value("IsInfoTab");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Insert Record.
@param IsInsertRecord The user can insert a new Record */
public void SetIsInsertRecord (Boolean IsInsertRecord)
{
Set_Value ("IsInsertRecord", IsInsertRecord);
}
/** Get Insert Record.
@return The user can insert a new Record */
public Boolean IsInsertRecord() 
{
Object oo = Get_Value("IsInsertRecord");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Read Only.
@param IsReadOnly Field is read only */
public void SetIsReadOnly (Boolean IsReadOnly)
{
Set_Value ("IsReadOnly", IsReadOnly);
}
/** Get Read Only.
@return Field is read only */
public Boolean IsReadOnly() 
{
Object oo = Get_Value("IsReadOnly");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Single Row Layout.
@param IsSingleRow Default for toggle between Single- and Multi-Row (Grid) Layout */
public void SetIsSingleRow (Boolean IsSingleRow)
{
Set_Value ("IsSingleRow", IsSingleRow);
}
/** Get Single Row Layout.
@return Default for toggle between Single- and Multi-Row (Grid) Layout */
public Boolean IsSingleRow() 
{
Object oo = Get_Value("IsSingleRow");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Order Tab.
@param IsSortTab The Tab determines the Order */
public void SetIsSortTab (Boolean IsSortTab)
{
Set_Value ("IsSortTab", IsSortTab);
}
/** Get Order Tab.
@return The Tab determines the Order */
public Boolean IsSortTab() 
{
Object oo = Get_Value("IsSortTab");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set TranslationTab.
@param IsTranslationTab This Tab contains translation information */
public void SetIsTranslationTab (Boolean IsTranslationTab)
{
Set_Value ("IsTranslationTab", IsTranslationTab);
}
/** Get TranslationTab.
@return This Tab contains translation information */
public Boolean IsTranslationTab() 
{
Object oo = Get_Value("IsTranslationTab");
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
/** Set Sql ORDER BY.
@param OrderByClause Fully qualified ORDER BY clause */
public void SetOrderByClause (String OrderByClause)
{
if (OrderByClause != null && OrderByClause.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
OrderByClause = OrderByClause.Substring(0,2000);
}
Set_Value ("OrderByClause", OrderByClause);
}
/** Get Sql ORDER BY.
@return Fully qualified ORDER BY clause */
public String GetOrderByClause() 
{
return (String)Get_Value("OrderByClause");
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

/** Referenced_Tab_ID AD_Reference_ID=278 */
public static int REFERENCED_TAB_ID_AD_Reference_ID=278;
/** Set Referenced Tab.
@param Referenced_Tab_ID Referenced Tab */
public void SetReferenced_Tab_ID (int Referenced_Tab_ID)
{
if (Referenced_Tab_ID <= 0) Set_Value ("Referenced_Tab_ID", null);
else
Set_Value ("Referenced_Tab_ID", Referenced_Tab_ID);
}
/** Get Referenced Tab.
@return Referenced Tab */
public int GetReferenced_Tab_ID() 
{
Object ii = Get_Value("Referenced_Tab_ID");
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
/** Set Tab Level.
@param TabLevel Hierarchical Tab Level (0 = top) */
public void SetTabLevel (int TabLevel)
{
Set_Value ("TabLevel", TabLevel);
}
/** Get Tab Level.
@return Hierarchical Tab Level (0 = top) */
public int GetTabLevel() 
{
Object ii = Get_Value("TabLevel");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Sql WHERE.
@param WhereClause Fully qualified SQL WHERE clause */
public void SetWhereClause (String WhereClause)
{
if (WhereClause != null && WhereClause.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
WhereClause = WhereClause.Substring(0,2000);
}
Set_Value ("WhereClause", WhereClause);
}
/** Get Sql WHERE.
@return Fully qualified SQL WHERE clause */
public String GetWhereClause() 
{
return (String)Get_Value("WhereClause");
}
}

}
