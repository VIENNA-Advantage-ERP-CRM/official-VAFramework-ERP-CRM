/********************************************************
// Module Name    : Run Time Show Window
// Purpose        : Model Field Value Object(get and set field attributes) (columns of table)
// Class Used     : GlobalVariable.cs, CommonFunctions.cs
// Created By     : Harwinder 
// Date           : 13 jan 2009
**********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using VAdvantage.Classes;

namespace VAdvantage.Controller
{
    public class FieldVObj
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="name"></param>
        /// <param name="displayType"></param>
        public FieldVObj(String columnName, String name, int displayType)
        {
            this.ColumnName = columnName;
            this.name = name.Replace("&", "");
            this.displayType = displayType;
            this.IsDisplayedf = true;
        }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="columnName">name of column</param>
        /// <param name="name">header</param>
        /// <param name="displayType">display type</param>
        /// <param name="isMandatoryUI">mandatory</param>
        public FieldVObj(String columnName, String name, int displayType, bool isMandatoryUI)
            : this(columnName, name, displayType)
        {
            this.IsMandatoryUI = isMandatoryUI;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="columnName">name of column</param>
        /// <param name="name">header</param>
        /// <param name="colSQL">column sql</param>
        /// <param name="displayType">display type</param>
        /// <param name="readOnly">is readonly</param>
        public FieldVObj(String columnName, String name, String colSQL, int displayType, bool readOnly)
            : this(columnName, name, colSQL, displayType, readOnly, true, false)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="columnName">name of column</param>
        /// <param name="name">headr name</param>
        /// <param name="colSQL">column sql</param>
        /// <param name="displayType">display type</param>
        /// <param name="readOnly">is readonly</param>
        /// <param name="isDisplayed">is displayed</param>
        /// <param name="isKey">is key column</param>
        public FieldVObj(String columnName, String name, String colSQL, int displayType, bool readOnly, bool isDisplayed, bool isKey)
            : this(columnName, name, displayType)
        {
            this.ColumnSQL = colSQL;
            this.selectClause = colSQL;
            this.IsReadOnly = readOnly;
            this.IsDisplayedf = isDisplayed;
            this.IsKey = isKey;
        }

        /// <summary>
        /// stnd Constructor
        /// </summary>
        public FieldVObj()
        {

        }

        //static long serialVersionUID = 4385061125114436797L;

        /*field Length*/
        public const int MAX_FIELD_LENGTH = 20;
        /*DisplayLength*/
        public const int MAX_DISPLAY_LENGTH = 20;

        /** Window No                   */
        public int windowNo;
        /** Tab No                      */
        public int tabNo;
        /** AD_Winmdow_ID               */
        public int AD_Window_ID;
        /** AD_Tab_ID					*/
        public int AD_Tab_ID;
        /** Is the Tab Read Only        */
        public bool tabReadOnly = false;

        /** Is Process Parameter        */
        public bool isProcess = false;

        /**	Column name		*/
        public String ColumnName = "";
        /**	Column sql		*/
        public String ColumnSQL;
        /**	Label			*/
        public String Header = "";
        /**	DisplayType		*/
        public int displayType = 0;
        /**	Table ID		*/
        public int AD_Table_ID = 0;
        /**	Clumn ID		*/
        public int AD_Column_ID = 0;
        //
        public int AD_InfoWindow_ID = 0;
        /**FieldID   */
        public int AD_Field_ID = 0;
        /**	Display Length	*/
        public int DisplayLength = 0;
        /**	Same Line		*/
        public bool IsSameLine = false;
        /**	Displayed		*/
        public bool IsDisplayedf = false;
        /**	Displayed		*/
        public bool IsDisplayedMR = false;

        /**	Dislay Logic	*/
        public String DisplayLogic = "";
        /**	Default Value	*/
        public String DefaultValue = "";
        /**	Mandatory Entry		*/
        public bool IsMandatoryUI = false;
        /**	Read Only		*/
        public bool IsReadOnly = false;
        /**	Updateable		*/
        public bool IsUpdateable = false;
        /**	Always Updateable	*/
        public bool IsAlwaysUpdateable = false;
        /**	Heading Only	*/
        public bool IsHeading = false;
        /**	Field Only		*/
        public bool IsFieldOnly = false;
        /**	Display Encryption	*/
        public bool IsEncryptedField = false;
        /**	Storage Encryption	*/
        public bool IsEncryptedColumn = false;
        /**	Find Selection		*/
        public bool IsSelectionColumn = false;
        //public bool IsIncludedColumn = false;
        /** Selection Seq */
        public int SelectionSeqNo = 0;
        /**	Order By		*/
        public int SortNo = 0;
        /**	Field Length		*/
        public int FieldLength = 0;
        /**	Format enforcement		*/
        public String VFormat = "";
        /**	Format Error Message		*/
        public String VFormatError = "";
        /**	Min. Value		*/
        public String ValueMin = "";
        /**	Max. Value		*/
        public String ValueMax = "";
        /**	Field Group		*/
        public String FieldGroup = "";
        /**	PK				*/
        public bool IsKey = false;
        /**	FK				*/
        public bool IsParent = false;
        /**	Process			*/
        public int AD_Process_ID = 0;
        /**	Process			*/
        public int AD_Form_ID = 0;
        /**	Description		*/
        public String Description = "";
        /**	Help			*/
        public String Help = "";
        /**	Read Only Logic	*/
        public String ReadOnlyLogic = "";
        /**	Display Obscure	*/
        public String ObscureType = null;
        /** Default Focus	*/
        public bool IsDefaultFocus = false;

        /**	Lookup Validation code	*/
        public String ValidationCode = "";
        /**	Reference Value			*/
        public int AD_Reference_Value_ID = 0;

        /**	Process Parameter Range		*/
        public bool isRange = false;
        /**	Process Parameter Value2	*/
        public String DefaultValue2 = "";

        /* display Type Id */
        public int AD_Reference_ID;
        /* Val_rule_ID */
        public int AD_Val_Rule_ID;

        /**	Label			*/
        public String name = "";
        public String label = "";

        /* Squence Number */
        public int seqNo;

        public bool IsIdentifier = false;
        public bool IsTranslated = false;
        //*Mandaotyr logic*/
        public String mandatoryLogic;
        public bool IsVirtualColumn = false;
        /**	Table name	*/
        public String tableName = null;

        public int hotKey = 1;
        public int mrSeqNo = 9999;

        //public ListBoxVO listBoxVO=null;

        public bool isImpactsValue = false;
        public bool isImpactsUITab = false;
        public bool isDependentValue = false;
        public bool isImpactsUI = false;

        // Used in process Parameters for fetching report data from tree hierarchy
        public bool LoadRecursiveData = false;
        public bool ShowChildOfSelected = false;

        public String MobileListingFormat = String.Empty;

        public bool isLink = false;
        public bool isRightPaneLink = false;
        /** zoom window **/
        public int ZoomWindow_ID = -1;

        public bool IsCopy = true;
        public int ColumnWidth = 0;

        //for checking process linked to btton is background process or not
        public bool IsBackgroundProcess = false;
        // If administrator wants to ask the end user about wheather to run process in background or not
        public bool AskUserBGProcess = false;


        public bool IsHeaderPanelitem = false;
        public int HeaderOverrideReference = 0;
        public string HeaderStyle = null;
        public bool HeaderHeadingOnly = false;
        public decimal HeaderSeqno = 0;
        public bool HeaderIconOnly = false;
        public string HtmlStyle = null;
        public bool ShowIcon = false;
        public int AD_Image_ID = 0;
        public string FontClass = "";
        public string PlaceHolder = "";
        public string ImageName = "";
        // for checking whether Maintain Versions is marked on Column
        public bool IsMaintainVersions = false;
        public int CellSpace = 0;
        public int FieldBreadth = 0;
        public bool LineBreak= false;
        public bool FieldGroupDefault = false;
        public bool ShowFilterOption = false;

        public bool IsUnique = false;
        public bool IsSwitch = false;

        /* style logic stirng { expression [,]}*/
        public string StyleLogic = "";
        public string GridImageStyle = null;



        /// <summary>
        /// calaculate MaxDisplayLength
        /// </summary>
        /// <returns>display length</returns>
        public int MaxDisplayLength()
        {
            //well, if no DisplayLenght defined, use MAX
            if (DisplayLength <= 0)
                return MAX_DISPLAY_LENGTH;
            return MAX_DISPLAY_LENGTH > DisplayLength ? DisplayLength : MAX_DISPLAY_LENGTH;
        }

        /// <summary>
        /// Field Has Display Logic
        /// </summary>
        /// <returns>true if has</returns>
        public bool HasDisplayLogic()
        {
            return (DisplayLogic != null && !DisplayLogic.Trim().Equals(""));
        }

        /// <summary>
        /// Field can display
        /// </summary>
        /// <returns>no display if false</returns>
        public bool CanDisplay()
        {
            return (HasDisplayLogic() || IsDisplayedf);
        }

        /// <summary>
        /// Has Read-Only logic
        /// </summary>
        /// <returns>true if has</returns>
        public bool HasReadOnlyLogic()
        {
            return (ReadOnlyLogic != null && !ReadOnlyLogic.Trim().Equals(""));
        }

        /// <summary>
        ///Is it Editable - checks IsActive, IsUpdateable, and isDisplayed
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="eval">context value</param>
        /// <param name="tabReadOnly">is tab readonly</param>
        /// <param name="rowReadOnly">data roe read only</param>
        /// <param name="tabLinkColumn">link column</param>
        /// <param name="inserting">is new record</param>
        /// <returns>if readonly then false</returns>
        public bool IsEditable(Ctx ctx, Evaluatee eval, bool tabReadOnly, bool rowReadOnly, String tabLinkColumn, bool inserting)
        {

            if (IsVirtualColumn)
                return false;
            //  Fields always enabled (are usually not updateable)
            if (ColumnName.Equals("Posted")
                || (ColumnName.Equals("Record_ID") && displayType == FieldType.Button))	//  Zoom
                return true;

            //  Fields always updareable
            if (IsAlwaysUpdateable)      //  Zoom
                return true;

            //  Tab or field is R/O
            if (tabReadOnly || IsReadOnly)
            {
                //if(Build.isVerbose())
                //log.finest(ColumnName + " NO - TabRO=" + tabReadOnly + ", FieldRO=" + IsReadOnly);
                return false;
            }

            //	Not Updateable - only editable if new updateable row
            if (!IsUpdateable && !inserting)
            {
                //if(Build.isVerbose())
                //log.finest(ColumnName + " NO - FieldUpdateable=" + IsUpdateable);
                return false;
            }

            //	Field is the Link Column of the tab
            String linkColumn = tabLinkColumn;
            if (ColumnName.Equals(linkColumn))
            {
                //if(Build.isVerbose())
                //log.finest(ColumnName + " NO - LinkColumn");
                return false;
            }

            //FIXME comment this out need to go back server for this information
            //	Role Access & Column Access			
            //	if (ctx != null)
            //	{
            //		int AD_Client_ID = ctx.getAD_Client_ID(windowNo);
            //		int AD_Org_ID = ctx.getAD_Org_ID(windowNo);
            //		String keyColumn = tabKeyColumn;
            //		if ("EntityType".Equals(keyColumn))
            //			keyColumn = "AD_EntityType_ID";
            //		if (!keyColumn.endsWith("_ID"))
            //			keyColumn += "_ID";			//	AD_Language_ID
            //		int Record_ID = ctx.getContextAsInt(keyColumn);
            //	MRole role = MRole.getDefault(ctx, false); 
            //if (!role.canUpdate(AD_Client_ID, AD_Org_ID, AD_Table_ID, Record_ID, false))
            //return false;
            //if (!role.isColumnAccess(AD_Table_ID, AD_Column_ID, false))
            //		return false;
            //	}
            if (rowReadOnly)
                return false;

            if (!isColumnAccess)
                return false;
            //  Do we have a readonly rule
            String logic = ReadOnlyLogic;
            if (logic != null && logic.Length > 0)
            {
                bool retValue = !Evaluator.EvaluateLogic(eval, logic);
                //if(Build.isVerbose())
                //log.finest(ColumnName + " R/O(" + logic + ") => R/W-" + retValue);
                if (!retValue)
                    return false;
            }

            //  Always editable if Active
            if (ColumnName.Equals("Processing")
                    || ColumnName.Equals("DocAction")
                    || ColumnName.Equals("GenerateTo"))
                return true;

            //  Record is Processed	***	
            if (ctx != null
                && (ctx.IsProcessed(windowNo)
                    || ctx.IsProcessing(windowNo)))
                return false;

            //  IsActive field is editable, if record not processed
            if (ColumnName.Equals("IsActive"))
                return true;

            //  Record is not Active
            if (ctx != null
                && !ctx.IsActive(windowNo))
            {
                //if(Build.isVerbose())
                //	log.finest(ColumnName + " Record not active");
                return false;
            }


            if (inserting)
                return true;
            //  ultimately visibily decides
            return IsDisplayed(eval);
        }	//	isEditable

        /// <summary>
        ///Is field is mandatory 
        /// </summary>
        /// <param name="eval">context value</param>
        /// <returns>true if mandatory</returns>
        public bool IsMandatory(Evaluatee eval)
        {
            if (IsVirtualColumn)
                return false;
            //	Not Mandatory
            String logic = mandatoryLogic;
            if (!IsMandatoryUI && (logic == null || logic.Length == 0))
                return false;

            //  Numeric Keys and Created/Updated as well as 
            //	DocumentNo/Value/ASI ars not mandatory (persistency layer manages them)
            if ((IsKey && ColumnName.EndsWith("_ID"))
                    || ColumnName.StartsWith("Created") || ColumnName.StartsWith("Updated")
                    || ColumnName.Equals("Value")
                    || ColumnName.Equals("DocumentNo")
                    || ColumnName.Equals("M_AttributeSetInstance_ID"))	//	0 is valid
                return false;

            //	Mandatory Logic
            if (logic != null && logic.Length > 0)
            {
                if (Evaluator.EvaluateLogic(eval, logic))
                    return true;
            }

            //  Mandatory only if displayed
            return IsDisplayed(eval);
        }	//	isMandatory

        /// <summary>
        ///Field is Displayed
        /// </summary>
        /// <param name="eval"></param>
        /// <returns></returns>
        public bool IsDisplayed(Evaluatee eval)
        {
            //  ** static content **
            //  not displayed
            if (!IsDisplayedf)
                return false;
            //  no restrictions
            String logic = DisplayLogic;
            if (logic == null || logic.Equals(""))
                return true;

            //  ** dynamic content **
            if (eval != null)
            {
                bool retValue = Evaluator.EvaluateLogic(eval, logic);
                //if(Build.isVerbose())
                //log.finest(ColumnName 
                //	+ " (" + logic + ") => " + retValue);
                return retValue;
            }
            return true;
        }

        public bool isQueryCriteria = false;
        public String selectClause = null;

        //public FieldVObj copySearch()
        //{
        //    FieldVO f = new FieldVO();

        //    f.AD_Column_ID = this.AD_Column_ID;
        //    f.AD_Field_ID = this.AD_Field_ID;
        //    f.AD_Tab_ID = this.AD_Tab_ID;
        //    f.AD_Table_ID = this.AD_Table_ID;
        //    f.ColumnName = this.ColumnName;
        //    f.tableName = this.tableName;
        //    if(this.displayType == FieldType.Text)
        //        f.displayType = FieldType.String;
        //    else
        //        f.displayType = this.displayType;

        //    f.DisplayLength = this.DisplayLength;
        //    f.FieldLength = this.FieldLength;
        //    f.IsDisplayed = this.IsDisplayed;

        //    if(this.listBoxVO != null){
        //        ListBoxVO searchList = new ListBoxVO( this.listBoxVO.getOptions(), this.listBoxVO.getDefaultKey() );
        //        searchList.Column_ID = this.listBoxVO.Column_ID;
        //        searchList.TableName = this.listBoxVO.TableName;
        //        searchList.AD_Reference_Value_ID = this.listBoxVO.AD_Reference_Value_ID;
        //        searchList.IsCreadedUpdatedBy = this.listBoxVO.IsCreadedUpdatedBy;
        //        searchList.IsValidated = this.listBoxVO.IsValidated;
        //        searchList.IsParent = this.listBoxVO.IsParent;
        //        searchList.KeyColumn = this.listBoxVO.KeyColumn;
        //        searchList.QueryDirect = this.listBoxVO.QueryDirect;
        //        searchList.ValidationCode = this.listBoxVO.ValidationCode;
        //        searchList.zoomWindow = this.listBoxVO.zoomWindow;
        //        searchList.ZoomWindowPO = this.listBoxVO.ZoomWindowPO;
        //        f.listBoxVO = searchList;
        //    }

        //    // these are needed by InfoWindow
        //    f.name = this.name;
        //    f.Description = this.Description;
        //    f.Help = this.Help;
        //    f.isQueryCriteria = this.isQueryCriteria;
        //    f.AD_Reference_ID = this.AD_Reference_ID;
        //    f.isRange = this.isRange;
        //    f.selectClause = this.selectClause;
        //    f.DefaultValue = this.DefaultValue;
        //    f.isImpactsValue = this.isImpactsValue;		
        //    f.IsIdentifier = this.IsIdentifier;

        //    f.windowNo = this.windowNo;
        //    f.TabNo = ComponentVO.SEARCH_TAB_NO;

        //    return f;
        //}
        //public FieldVO copySearchAdvanced()
        //{
        //    FieldVO f = new FieldVO();

        //    f.AD_Column_ID = this.AD_Column_ID;
        //    f.AD_Field_ID = this.AD_Field_ID;
        //    f.AD_Tab_ID = this.AD_Tab_ID;
        //    f.AD_Table_ID = this.AD_Table_ID;
        //    f.ColumnName = this.ColumnName;
        //    f.tableName = this.tableName;
        //    if(this.displayType == FieldType.Text)
        //        f.displayType = FieldType.String;
        //    else
        //        f.displayType = this.displayType;

        //    f.DisplayLength = this.DisplayLength;
        //    f.FieldLength = this.FieldLength;
        //    f.IsDisplayed = true;
        //    f.IsReadOnly = false;
        //    f.IsAlwaysUpdateable = this.IsAlwaysUpdateable;
        //    f.IsMandatoryUI = false;
        //    f.isDependentValue = this.isDependentValue;
        //    f.AD_Reference_Value_ID = this.AD_Reference_Value_ID;
        //    f.AD_Window_ID = this.AD_Window_ID;


        //    if(this.listBoxVO != null){
        //        ListBoxVO searchList = new ListBoxVO(false);
        //        searchList.Column_ID = this.listBoxVO.Column_ID;
        //        searchList.TableName = this.listBoxVO.TableName;
        //        searchList.AD_Reference_Value_ID = this.listBoxVO.AD_Reference_Value_ID;
        //        searchList.IsCreadedUpdatedBy = this.listBoxVO.IsCreadedUpdatedBy;
        //        searchList.IsValidated = this.listBoxVO.IsValidated;
        //        searchList.IsParent = this.listBoxVO.IsParent;
        //        searchList.KeyColumn = this.listBoxVO.KeyColumn;
        //        searchList.QueryDirect = this.listBoxVO.QueryDirect;
        //        searchList.ValidationCode = this.listBoxVO.ValidationCode;
        //        searchList.zoomWindow = this.listBoxVO.zoomWindow;
        //        searchList.ZoomWindowPO = this.listBoxVO.ZoomWindowPO;
        //        /*
        //        List searchOptions = this.listBoxVO.getOptions();
        //        if(searchOptions != null)
        //            for(int i=0; i < searchOptions.size(); i++){
        //                searchList.addOption((NamePair)searchOptions.get(i));
        //            }
        //            */
        //        f.listBoxVO = searchList;
        //    }

        //    // these are needed by InfoWindow
        //    f.name = this.name;
        //    f.Description = this.Description;
        //    f.Help = this.Help;
        //    f.isQueryCriteria = this.isQueryCriteria;
        //    f.AD_Reference_ID = this.AD_Reference_ID;
        //    f.isRange = this.isRange;
        //    f.selectClause = this.selectClause;
        //    f.DefaultValue = this.DefaultValue;
        //    f.isImpactsValue = this.isImpactsValue;		
        //    f.IsIdentifier = this.IsIdentifier;

        //    f.windowNo = this.windowNo;
        //    f.TabNo = ComponentVO.SEARCH_TAB_NO;
        //    return f;
        //}

        public static String GetToColumnName(String columnName)
        {
            return columnName + "_2";
        }

        public String numberFormatPattern = "###,###,###,###,###,###.######";
        public String dateFormatPattern = "yyyy-MM-dd";
        public bool isColumnAccess = true;

        /// <summary>
        /// String representation of class
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return ColumnName;
        }
    }
}
