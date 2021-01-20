namespace VAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for VAF_Tab
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_Tab : PO{public X_VAF_Tab (Context ctx, int VAF_Tab_ID, Trx trxName) : base (ctx, VAF_Tab_ID, trxName){/** if (VAF_Tab_ID == 0){SetVAF_Tab_ID (0);SetVAF_TableView_ID (0);SetAD_Window_ID (0);SetEntityType (null);// U
SetHasTree (false);SetIsAdvancedTab (false);// N
SetIsDisplayed (true);// Y
SetIsInsertRecord (true);// Y
SetIsReadOnly (false);SetIsSingleRow (false);SetIsSortTab (false);// N
SetIsTranslationTab (false);SetName (null);SetSeqNo (0);// @SQL=SELECT COALESCE(MAX(SeqNo),0)+10 AS DefaultValue FROM VAF_Tab WHERE AD_Window_ID=@AD_Window_ID@
SetTabLevel (0);} */
}public X_VAF_Tab (Ctx ctx, int VAF_Tab_ID, Trx trxName) : base (ctx, VAF_Tab_ID, trxName){/** if (VAF_Tab_ID == 0){SetVAF_Tab_ID (0);SetVAF_TableView_ID (0);SetAD_Window_ID (0);SetEntityType (null);// U
SetHasTree (false);SetIsAdvancedTab (false);// N
SetIsDisplayed (true);// Y
SetIsInsertRecord (true);// Y
SetIsReadOnly (false);SetIsSingleRow (false);SetIsSortTab (false);// N
SetIsTranslationTab (false);SetName (null);SetSeqNo (0);// @SQL=SELECT COALESCE(MAX(SeqNo),0)+10 AS DefaultValue FROM VAF_Tab WHERE AD_Window_ID=@AD_Window_ID@
SetTabLevel (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_Tab (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_Tab (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_Tab (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_Tab(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27858186539576L;/** Last Updated Timestamp 12/11/2019 4:17:02 PM */
public static long updatedMS = 1576061222787L;/** VAF_TableView_ID=106 */
public static int Table_ID; // =106;
/** TableName=VAF_Tab */
public static String Table_Name="VAF_Tab";
protected static KeyNamePair model;protected Decimal accessLevel = new Decimal(4);/** AccessLevel
@return 4 - System 
*/
protected override int Get_AccessLevel(){return Convert.ToInt32(accessLevel.ToString());}/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Context ctx){POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);return poi;}/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Ctx ctx){POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);return poi;}/** Info
@return info
*/
public override String ToString(){StringBuilder sb = new StringBuilder ("X_VAF_Tab[").Append(Get_ID()).Append("]");return sb.ToString();}
/** VAF_ColumnSortOrder_ID VAF_Control_Ref_ID=257 */
public static int VAF_COLUMNSORTORDER_ID_VAF_Control_Ref_ID=257;/** Set Order Column.
@param VAF_ColumnSortOrder_ID Column determining the order */
public void SetVAF_ColumnSortOrder_ID (int VAF_ColumnSortOrder_ID){if (VAF_ColumnSortOrder_ID <= 0) Set_Value ("VAF_ColumnSortOrder_ID", null);else
Set_Value ("VAF_ColumnSortOrder_ID", VAF_ColumnSortOrder_ID);}/** Get Order Column.
@return Column determining the order */
public int GetVAF_ColumnSortOrder_ID() {Object ii = Get_Value("VAF_ColumnSortOrder_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}
/** VAF_ColumnSortYesNo_ID VAF_Control_Ref_ID=258 */
public static int VAF_COLUMNSORTYESNO_ID_VAF_Control_Ref_ID=258;/** Set Included Column.
@param VAF_ColumnSortYesNo_ID Column determining if a Table Column is included in Ordering */
public void SetVAF_ColumnSortYesNo_ID (int VAF_ColumnSortYesNo_ID){if (VAF_ColumnSortYesNo_ID <= 0) Set_Value ("VAF_ColumnSortYesNo_ID", null);else
Set_Value ("VAF_ColumnSortYesNo_ID", VAF_ColumnSortYesNo_ID);}/** Get Included Column.
@return Column determining if a Table Column is included in Ordering */
public int GetVAF_ColumnSortYesNo_ID() {Object ii = Get_Value("VAF_ColumnSortYesNo_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Column.
@param VAF_Column_ID Column in the table */
public void SetVAF_Column_ID (int VAF_Column_ID){if (VAF_Column_ID <= 0) Set_Value ("VAF_Column_ID", null);else
Set_Value ("VAF_Column_ID", VAF_Column_ID);}/** Get Column.
@return Column in the table */
public int GetVAF_Column_ID() {Object ii = Get_Value("VAF_Column_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Context Area.
@param VAF_ContextScope_ID Business Domain Area Terminology */
public void SetVAF_ContextScope_ID (int VAF_ContextScope_ID){if (VAF_ContextScope_ID <= 0) Set_Value ("VAF_ContextScope_ID", null);else
Set_Value ("VAF_ContextScope_ID", VAF_ContextScope_ID);}/** Get Context Area.
@return Business Domain Area Terminology */
public int GetVAF_ContextScope_ID() {Object ii = Get_Value("VAF_ContextScope_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Header Layout.
@param VAF_HeaderLayout_ID Header Layout */
public void SetVAF_HeaderLayout_ID (int VAF_HeaderLayout_ID){if (VAF_HeaderLayout_ID <= 0) Set_Value ("VAF_HeaderLayout_ID", null);else
Set_Value ("VAF_HeaderLayout_ID", VAF_HeaderLayout_ID);}/** Get Header Layout.
@return Header Layout */
public int GetVAF_HeaderLayout_ID() {Object ii = Get_Value("VAF_HeaderLayout_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Image.
@param VAF_Image_ID Image or Icon */
public void SetVAF_Image_ID (int VAF_Image_ID){if (VAF_Image_ID <= 0) Set_Value ("VAF_Image_ID", null);else
Set_Value ("VAF_Image_ID", VAF_Image_ID);}/** Get Image.
@return Image or Icon */
public int GetVAF_Image_ID() {Object ii = Get_Value("VAF_Image_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Process.
@param VAF_Job_ID Process or Report */
public void SetVAF_Job_ID (int VAF_Job_ID){if (VAF_Job_ID <= 0) Set_Value ("VAF_Job_ID", null);else
Set_Value ("VAF_Job_ID", VAF_Job_ID);}/** Get Process.
@return Process or Report */
public int GetVAF_Job_ID() {Object ii = Get_Value("VAF_Job_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Tab.
@param VAF_Tab_ID Tab within a Window */
public void SetVAF_Tab_ID (int VAF_Tab_ID){if (VAF_Tab_ID < 1) throw new ArgumentException ("VAF_Tab_ID is mandatory.");Set_ValueNoCheck ("VAF_Tab_ID", VAF_Tab_ID);}/** Get Tab.
@return Tab within a Window */
public int GetVAF_Tab_ID() {Object ii = Get_Value("VAF_Tab_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Table.
@param VAF_TableView_ID Database Table information */
public void SetVAF_TableView_ID (int VAF_TableView_ID){if (VAF_TableView_ID < 1) throw new ArgumentException ("VAF_TableView_ID is mandatory.");Set_Value ("VAF_TableView_ID", VAF_TableView_ID);}/** Get Table.
@return Database Table information */
public int GetVAF_TableView_ID() {Object ii = Get_Value("VAF_TableView_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Window.
@param AD_Window_ID Data entry or display window */
public void SetAD_Window_ID (int AD_Window_ID){if (AD_Window_ID < 1) throw new ArgumentException ("AD_Window_ID is mandatory.");Set_ValueNoCheck ("AD_Window_ID", AD_Window_ID);}/** Get Window.
@return Data entry or display window */
public int GetAD_Window_ID() {Object ii = Get_Value("AD_Window_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() {return new KeyNamePair(Get_ID(), GetAD_Window_ID().ToString());}/** Set Commit Warning.
@param CommitWarning Warning displayed when saving */
public void SetCommitWarning (String CommitWarning){if (CommitWarning != null && CommitWarning.Length > 2000){log.Warning("Length > 2000 - truncated");CommitWarning = CommitWarning.Substring(0,2000);}Set_Value ("CommitWarning", CommitWarning);}/** Get Commit Warning.
@return Warning displayed when saving */
public String GetCommitWarning() {return (String)Get_Value("CommitWarning");}/** Set Create Version Tab.
@param CreateMasterVersionTab Create Version Tab */
public void SetCreateMasterVersionTab (String CreateMasterVersionTab){if (CreateMasterVersionTab != null && CreateMasterVersionTab.Length > 1){log.Warning("Length > 1 - truncated");CreateMasterVersionTab = CreateMasterVersionTab.Substring(0,1);}Set_Value ("CreateMasterVersionTab", CreateMasterVersionTab);}/** Get Create Version Tab.
@return Create Version Tab */
public String GetCreateMasterVersionTab() {return (String)Get_Value("CreateMasterVersionTab");}/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description){if (Description != null && Description.Length > 255){log.Warning("Length > 255 - truncated");Description = Description.Substring(0,255);}Set_Value ("Description", Description);}/** Get Description.
@return Optional short description of the record */
public String GetDescription() {return (String)Get_Value("Description");}/** Set Display Logic.
@param DisplayLogic If the Field is displayed, the result determines if the field is actually displayed */
public void SetDisplayLogic (String DisplayLogic){if (DisplayLogic != null && DisplayLogic.Length > 2000){log.Warning("Length > 2000 - truncated");DisplayLogic = DisplayLogic.Substring(0,2000);}Set_Value ("DisplayLogic", DisplayLogic);}/** Get Display Logic.
@return If the Field is displayed, the result determines if the field is actually displayed */
public String GetDisplayLogic() {return (String)Get_Value("DisplayLogic");}
/** EntityType VAF_Control_Ref_ID=389 */
public static int ENTITYTYPE_VAF_Control_Ref_ID=389;/** Set Entity Type.
@param EntityType Dictionary Entity Type; Determines ownership and synchronization */
public void SetEntityType (String EntityType){if (EntityType.Length > 4){log.Warning("Length > 4 - truncated");EntityType = EntityType.Substring(0,4);}Set_Value ("EntityType", EntityType);}/** Get Entity Type.
@return Dictionary Entity Type; Determines ownership and synchronization */
public String GetEntityType() {return (String)Get_Value("EntityType");}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_ValueNoCheck ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Has Tree.
@param HasTree Window has Tree Graph */
public void SetHasTree (Boolean HasTree){Set_Value ("HasTree", HasTree);}/** Get Has Tree.
@return Window has Tree Graph */
public Boolean IsHasTree() {Object oo = Get_Value("HasTree");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Comment.
@param Help Comment, Help or Hint */
public void SetHelp (String Help){if (Help != null && Help.Length > 2000){log.Warning("Length > 2000 - truncated");Help = Help.Substring(0,2000);}Set_Value ("Help", Help);}/** Get Comment.
@return Comment, Help or Hint */
public String GetHelp() {return (String)Get_Value("Help");}/** Set Import Fields.
@param ImportFields Create Fields from Table Columns */
public void SetImportFields (String ImportFields){if (ImportFields != null && ImportFields.Length > 1){log.Warning("Length > 1 - truncated");ImportFields = ImportFields.Substring(0,1);}Set_Value ("ImportFields", ImportFields);}/** Get Import Fields.
@return Create Fields from Table Columns */
public String GetImportFields() {return (String)Get_Value("ImportFields");}
/** Included_Tab_ID VAF_Control_Ref_ID=278 */
public static int INCLUDED_TAB_ID_VAF_Control_Ref_ID=278;/** Set Included Tab.
@param Included_Tab_ID Included Tab in this Tab (Master Detail) */
public void SetIncluded_Tab_ID (int Included_Tab_ID){if (Included_Tab_ID <= 0) Set_Value ("Included_Tab_ID", null);else
Set_Value ("Included_Tab_ID", Included_Tab_ID);}/** Get Included Tab.
@return Included Tab in this Tab (Master Detail) */
public int GetIncluded_Tab_ID() {Object ii = Get_Value("Included_Tab_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Advanced Tab.
@param IsAdvancedTab This Tab contains advanced Functionality */
public void SetIsAdvancedTab (Boolean IsAdvancedTab){Set_Value ("IsAdvancedTab", IsAdvancedTab);}/** Get Advanced Tab.
@return This Tab contains advanced Functionality */
public Boolean IsAdvancedTab() {Object oo = Get_Value("IsAdvancedTab");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Displayed.
@param IsDisplayed Determines, if this field is displayed */
public void SetIsDisplayed (Boolean IsDisplayed){Set_Value ("IsDisplayed", IsDisplayed);}/** Get Displayed.
@return Determines, if this field is displayed */
public Boolean IsDisplayed() {Object oo = Get_Value("IsDisplayed");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Header Panel.
@param IsHeaderPanel If Checked, then header panel will be displayed for that tab. */
public void SetIsHeaderPanel (Boolean IsHeaderPanel){Set_Value ("IsHeaderPanel", IsHeaderPanel);}/** Get Header Panel.
@return If Checked, then header panel will be displayed for that tab. */
public Boolean IsHeaderPanel() {Object oo = Get_Value("IsHeaderPanel");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Accounting Tab.
@param IsInfoTab This Tab contains accounting information */
public void SetIsInfoTab (Boolean IsInfoTab){Set_Value ("IsInfoTab", IsInfoTab);}/** Get Accounting Tab.
@return This Tab contains accounting information */
public Boolean IsInfoTab() {Object oo = Get_Value("IsInfoTab");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Insert Record.
@param IsInsertRecord The user can insert a new Record */
public void SetIsInsertRecord (Boolean IsInsertRecord){Set_Value ("IsInsertRecord", IsInsertRecord);}/** Get Insert Record.
@return The user can insert a new Record */
public Boolean IsInsertRecord() {Object oo = Get_Value("IsInsertRecord");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Read Only.
@param IsReadOnly Field is read only */
public void SetIsReadOnly (Boolean IsReadOnly){Set_Value ("IsReadOnly", IsReadOnly);}/** Get Read Only.
@return Field is read only */
public Boolean IsReadOnly() {Object oo = Get_Value("IsReadOnly");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Single Row Layout.
@param IsSingleRow Default for toggle between Single- and Multi-Row (Grid) Layout */
public void SetIsSingleRow (Boolean IsSingleRow){Set_Value ("IsSingleRow", IsSingleRow);}/** Get Single Row Layout.
@return Default for toggle between Single- and Multi-Row (Grid) Layout */
public Boolean IsSingleRow() {Object oo = Get_Value("IsSingleRow");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Order Tab.
@param IsSortTab The Tab determines the Order */
public void SetIsSortTab (Boolean IsSortTab){Set_Value ("IsSortTab", IsSortTab);}/** Get Order Tab.
@return The Tab determines the Order */
public Boolean IsSortTab() {Object oo = Get_Value("IsSortTab");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set TranslationTab.
@param IsTranslationTab This Tab contains translation information */
public void SetIsTranslationTab (Boolean IsTranslationTab){Set_Value ("IsTranslationTab", IsTranslationTab);}/** Get TranslationTab.
@return This Tab contains translation information */
public Boolean IsTranslationTab() {Object oo = Get_Value("IsTranslationTab");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name){if (Name == null) throw new ArgumentException ("Name is mandatory.");if (Name.Length > 60){log.Warning("Length > 60 - truncated");Name = Name.Substring(0,60);}Set_Value ("Name", Name);}/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() {return (String)Get_Value("Name");}/** Set Sql ORDER BY.
@param OrderByClause Fully qualified ORDER BY clause */
public void SetOrderByClause (String OrderByClause){if (OrderByClause != null && OrderByClause.Length > 2000){log.Warning("Length > 2000 - truncated");OrderByClause = OrderByClause.Substring(0,2000);}Set_Value ("OrderByClause", OrderByClause);}/** Get Sql ORDER BY.
@return Fully qualified ORDER BY clause */
public String GetOrderByClause() {return (String)Get_Value("OrderByClause");}/** Set Process Now.
@param Processing Process Now */
public void SetProcessing (Boolean Processing){Set_Value ("Processing", Processing);}/** Get Process Now.
@return Process Now */
public Boolean IsProcessing() {Object oo = Get_Value("Processing");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Read Only Logic.
@param ReadOnlyLogic Logic to determine if field is read only (applies only when field is read-write) */
public void SetReadOnlyLogic (String ReadOnlyLogic){if (ReadOnlyLogic != null && ReadOnlyLogic.Length > 2000){log.Warning("Length > 2000 - truncated");ReadOnlyLogic = ReadOnlyLogic.Substring(0,2000);}Set_Value ("ReadOnlyLogic", ReadOnlyLogic);}/** Get Read Only Logic.
@return Logic to determine if field is read only (applies only when field is read-write) */
public String GetReadOnlyLogic() {return (String)Get_Value("ReadOnlyLogic");}
/** Referenced_Tab_ID VAF_Control_Ref_ID=278 */
public static int REFERENCED_TAB_ID_VAF_Control_Ref_ID=278;/** Set Referenced Tab.
@param Referenced_Tab_ID Referenced Tab */
public void SetReferenced_Tab_ID (int Referenced_Tab_ID){if (Referenced_Tab_ID <= 0) Set_Value ("Referenced_Tab_ID", null);else
Set_Value ("Referenced_Tab_ID", Referenced_Tab_ID);}/** Get Referenced Tab.
@return Referenced Tab */
public int GetReferenced_Tab_ID() {Object ii = Get_Value("Referenced_Tab_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Sequence.
@param SeqNo Method of ordering elements; lowest number comes first */
public void SetSeqNo (int SeqNo){Set_Value ("SeqNo", SeqNo);}/** Get Sequence.
@return Method of ordering elements; lowest number comes first */
public int GetSeqNo() {Object ii = Get_Value("SeqNo");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Show Summary Level Nodes.
@param ShowSummaryLevelNodes Show Summary Level Nodes */
public void SetShowSummaryLevelNodes (Boolean ShowSummaryLevelNodes){Set_Value ("ShowSummaryLevelNodes", ShowSummaryLevelNodes);}/** Get Show Summary Level Nodes.
@return Show Summary Level Nodes */
public Boolean IsShowSummaryLevelNodes() {Object oo = Get_Value("ShowSummaryLevelNodes");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Tab Level.
@param TabLevel Hierarchical Tab Level (0 = top) */
public void SetTabLevel (int TabLevel){Set_Value ("TabLevel", TabLevel);}/** Get Tab Level.
@return Hierarchical Tab Level (0 = top) */
public int GetTabLevel() {Object ii = Get_Value("TabLevel");if (ii == null) return 0;return Convert.ToInt32(ii);}
/** TabPanelAlignment VAF_Control_Ref_ID=1000223 */
public static int TABPANELALIGNMENT_VAF_Control_Ref_ID=1000223;/** Horizontal = H */
public static String TABPANELALIGNMENT_Horizontal = "H";/** Vertical = V */
public static String TABPANELALIGNMENT_Vertical = "V";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsTabPanelAlignmentValid (String test){return test == null || test.Equals("H") || test.Equals("V");}/** Set Tab Panel Alignment.
@param TabPanelAlignment Tab Panel Alignment */
public void SetTabPanelAlignment (String TabPanelAlignment){if (!IsTabPanelAlignmentValid(TabPanelAlignment))
throw new ArgumentException ("TabPanelAlignment Invalid value - " + TabPanelAlignment + " - Reference_ID=1000223 - H - V");if (TabPanelAlignment != null && TabPanelAlignment.Length > 1){log.Warning("Length > 1 - truncated");TabPanelAlignment = TabPanelAlignment.Substring(0,1);}Set_Value ("TabPanelAlignment", TabPanelAlignment);}/** Get Tab Panel Alignment.
@return Tab Panel Alignment */
public String GetTabPanelAlignment() {return (String)Get_Value("TabPanelAlignment");}/** Set Sql WHERE.
@param WhereClause Fully qualified SQL WHERE clause */
public void SetWhereClause (String WhereClause){if (WhereClause != null && WhereClause.Length > 2000){log.Warning("Length > 2000 - truncated");WhereClause = WhereClause.Substring(0,2000);}Set_Value ("WhereClause", WhereClause);}/** Get Sql WHERE.
@return Fully qualified SQL WHERE clause */
public String GetWhereClause() {return (String)Get_Value("WhereClause");}

        /** Set Maintain Versions on Approval.
@param MaintainVerOnApproval Maintain Versions on Approval */
        public void SetMaintainVerOnApproval(Boolean MaintainVerOnApproval) { Set_Value("MaintainVerOnApproval", MaintainVerOnApproval); }/** Get Maintain Versions on Approval.
@return Maintain Versions on Approval */
        public Boolean IsMaintainVerOnApproval() { Object oo = Get_Value("MaintainVerOnApproval"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }
    }
}