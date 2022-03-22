using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.Controller;
namespace VAdvantage.Model
{
    public class GridField : Classes.Evaluatee
    {
        #region "Declaration"

        /** Value Object                */
        public GridFieldVO _vo;
        /** The Mnemonic				*/
        private char _mnemonic = '0';
        /** New Row / inserting         */
        private bool _inserting = false;
        /** Max Display Length = 60		*/
        public const int MAXDISPLAY_LENGTH = 60;
        /** The current value                   */
        private Object _value = null;
        /** The old to force Property Change    */
        private static Object _sOldValue = new Object();
        /** The old/previous value              */
        private Object _oldValue = _sOldValue;
        /** Only fire Property Change if old value really changed   */
        //private bool _valueNoFire = true;
        /** Error Status                        */
        private bool _error = false;
        /** Parent Check						*/
        private bool? _parentValue = null;
        /** PropertyChange Name 				*/
        public static string PROPERTY = "FieldValue";
        /** Indicator for new Value				*/
        public static string INSERTING = "FieldValueInserting";
        /** Error Value for HTML interface          */
        // private string _errorValue = null;
        /** Error Value indicator for HTML interface    */
        // private bool _errorValueFlag = false;
        /** Lookup for this field, use for reference type table, table direct and list */
        //locator, location. contain the data to show runtime
        public Lookup _lookup = null;
        //private IControl _vControl = null;

        private VLogger log = VLogger.GetVLogger(typeof(GridField).FullName);

        #endregion

        //public IControl SetControl
        //{
        //    set { _vControl = value; }
        //}

        public GridField(GridFieldVO vo)
        {
            _vo = vo;
            LoadLookUp();
            SetError(false);
        }

        /// <summary>
        /// Set Lookup for columns with lookup
        /// </summary>
        public void LoadLookUp()
        {
            if (!IsLookup())
            {
                return;
            }

            if (_vo.GetCtx().SkipLookup) // do need in Visual Editor
            {
                return;
            }
            //if (!IsDisplayed() && !IsDisplayedMR())
            //{
            //    return;
            //}

            if (DisplayType.IsLookup(_vo.displayType))
            {
                MLookup ml = new MLookup(_vo.GetCtx(), _vo.windowNo, _vo.tabNo, _vo.displayType);
                ml.SetColumnName(_vo.ColumnName.ToLower());
                if (_vo.lookupInfo == null)
                {
                    _vo.lookupInfo = VLookUpFactory.GetLookUpInfo(ml, _vo.AD_Column_ID,
                    Env.GetLanguage(_vo.GetCtx()), _vo.ColumnName, _vo.AD_Reference_Value_ID,
                    _vo.IsParent, _vo.ValidationCode);

                    //_vo.lookupInfo = VLookUpFactory.GetLookUpInfo(_vo.GetContext(), _vo.GetWindowNum(), _vo.DISPLAYTYPE, _vo.AD_COLUMN_ID,
                    //     Utility.Env.GetLanguage(_vo.GetCtx()), _vo.COLUMNNAME, _vo.AD_REF_VAL_ID,
                    //    _vo.ISPARENT, _vo.VALIDATIONCODE);
                }
                //	Prevent loading of CreatedBy/UpdatedBy
                if (_vo.displayType == DisplayType.Table
                    && (_vo.ColumnName.Equals("CreatedBy") || _vo.ColumnName.Equals("UpdatedBy")))
                {
                    _vo.lookupInfo.isCreadedUpdatedBy = true;
                    ml.SetDisplayType(DisplayType.Search);
                }
                //
                if (_vo.lookupInfo == null)
                {
                    throw new Exception(Msg.GetMsg(_vo.GetCtx(), "Error") + " " + _vo.Header);
                }
                _vo.lookupInfo.isKey = _vo.IsKey;
                _lookup = ml.Initialize(_vo.lookupInfo);
            }
            //else if (_vo.displayType == DisplayType.Location)   //  not cached
            //{
            //    MLocationLookup ml = new MLocationLookup(_vo.GetCtx(), _vo.windowNo);
            //    _lookup = ml;
            //}
            //else if (_vo.displayType == DisplayType.Locator)
            //{
            //    MLocatorLookup ml = new MLocatorLookup(_vo.GetCtx(), _vo.windowNo);
            //    _lookup = ml;
            //}
            //else if (_vo.displayType == DisplayType.Account)    //  not cached
            //{
            //    MAccountLookup ma = new MAccountLookup(_vo.GetCtx(), _vo.windowNo);
            //    _lookup = ma;
            //}
            //else if (_vo.displayType == DisplayType.PAttribute)    //  not cached
            //{
            //    MPAttributeLookup pa = new MPAttributeLookup(_vo.GetCtx(), _vo.windowNo);
            //    _lookup = pa;
            //}
            //else if (_vo.displayType == DisplayType.AmtDimension)    //  not cached
            //{
            //    MAmtDimension pa = new MAmtDimension(_vo.GetCtx(), _vo.windowNo);
            //    _lookup = pa;
            //}
            //else if (_vo.displayType == DisplayType.ProductContainer)    //  not cached
            //{
            //    MProductContainerLoopup pa = new MProductContainerLoopup(_vo.GetCtx(), _vo.windowNo);
            //    _lookup = pa;
            //}
        }


        /// <summary>
        /// Wait until Load is complete
        /// </summary>
        public void LookupLoadComplete()
        {
            if (_lookup == null)
                return;
            //_lookup.loadComplete();
        }   // 

        /// <summary>
        /// IS Field Is lookup
        /// </summary>
        /// <returns></returns>
        public bool IsLookup()
        {
            bool retValue = false;
            if (_vo.IsKey)
                retValue = false;
            //	else if (_vo.COLUMNNAME.Equals("CreatedBy") || _vo.COLUMNNAME.Equals("UpdatedBy"))
            //		retValue = false;
            else if (DisplayType.IsLookup(_vo.displayType))
                retValue = true;
            else if (_vo.displayType == DisplayType.Location
                || _vo.displayType == DisplayType.Locator
                || _vo.displayType == DisplayType.Account
                || _vo.displayType == DisplayType.PAttribute
                || _vo.displayType == DisplayType.AmtDimension
                 || _vo.displayType == DisplayType.ProductContainer)
                retValue = true;

            return retValue;
        }   //  isLookup

        /// <summary>
        /// Get Column Name
        /// </summary>
        /// <returns></returns>
        public string GetColumnName()
        {
            if (_vo != null)
                return _vo.ColumnName;
            return null;
        }

        /// <summary>
        /// Field Window Id
        /// </summary>
        /// <returns></returns>
        public int GetAD_Window_ID()
        {
            return _vo.AD_Window_ID;
        }

        /// <summary>
        /// Name Of field
        /// </summary>
        /// <returns></returns>
        public string GetHeader()
        {
            return _vo.Header;
        }

        /// <summary>
        /// Name Of field
        /// </summary>
        /// <returns></returns>
        public int GetMRSeqNo()
        {
            return _vo.mrSeqNo;
        }




        /// <summary>
        ///  Get Value
        /// </summary>
        /// <returns>return current value</returns>
        public Object GetValue()
        {
            if (DisplayType.YesNo == _vo.displayType)
            {
                return Convert.ToBoolean(_value.ToString());
            }
            return _value;
        }

        /// <summary>
        /// Is Key
        /// </summary>
        /// <returns></returns>
        public bool IsKey()
        {
            return _vo.IsKey;
        }

        /// <summary>
        ///Parent Column
        /// </summary>
        /// <returns></returns>
        public bool IsParentColumn()
        {
            return _vo.IsParent;
        }

        /// <summary>
        /// Sort Number
        /// </summary>
        /// <returns></returns>
        public int GetSortNo()
        {
            return _vo.SortNo;
        }

        /// <summary>
        /// field is displayed or not
        /// </summary>
        /// <returns>true if displayed </returns>
        public bool IsDisplayed()
        {
            return _vo.IsDisplayedf;
        }


        /// <summary>
        /// field is displayed or not
        /// </summary>
        /// <returns>true if displayed </returns>
        public bool IsDisplayedMR()
        {
            return _vo.IsDisplayedMR;
        }

        /// <summary>
        /// Get group of field
        /// </summary>
        /// <returns></returns>
        public string GetFieldGroup()
        {
            return _vo.FieldGroup;
        }

        /// <summary>
        /// Get Dependenton  fields  list
        /// </summary>
        /// <returns></returns>
        public List<string> GetDependentOn()
        {
            List<string> list = new List<String>();
            //	Implicit Dependencies
            if (GetColumnName().Equals("M_AttributeSetInstance_ID"))
                list.Add("M_Product_ID");
            else if (GetColumnName().Equals("M_Locator_ID") || GetColumnName().Equals("M_LocatorTo_ID"))
            {
                list.Add("M_Product_ID");
                list.Add("M_Warehouse_ID");
            }
            //  Display dependent
            Evaluator.ParseDepends(list, _vo.DisplayLogic);
            Evaluator.ParseDepends(list, _vo.ReadOnlyLogic);
            //  Lookup
            if (_lookup != null)
            {
                Evaluator.ParseDepends(list, _lookup.GetValidation());// _lookup.getv .getValidation());
            }
            //
            if (list.Count > 0 && Logging.VLogMgt.IsLevelFiner())//  CLogMgt.isLevelFiner())
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < list.Count; i++)
                    sb.Append(list[i]).Append(" - ");
                log.Finer("(" + _vo.ColumnName + ") " + sb.ToString());
            }
            return list;
        }

        //protected void ParseDepends(List<string> list, string str)
        //{
        //    if (str == null || str == "")
        //        return;
        //    //	log.fine(parseString);
        //    string s = str;
        //    //  while we have variables
        //    while (s.IndexOf("@") != -1)
        //    {   
        //        int pos = s.IndexOf("@");
        //        s = s.Substring(pos + 1);
        //        pos = s.IndexOf("@");
        //        if (pos == -1)
        //            continue;	//	error number of @@ not correct
        //        string variable = s.Substring(0, pos);
        //        s = s.Substring(pos + 1);
        //        //	log.fine(variable);
        //        if (!list.Contains(variable))
        //            list.Add(variable);
        //    }
        //}

        /// <summary>
        /// Get Display Type Of field(eg. text,string,int etc)
        /// </summary>
        /// <returns></returns>
        public int GetDisplayType()
        {
            return _vo.displayType;
        }

        /// <summary>
        /// Is Field Only
        /// </summary>
        /// <returns></returns>
        public bool IsFieldOnly()
        {
            return _vo.IsFieldOnly;
        }

        /// <summary>
        /// Is this a long (string/text) field (over 60/2=30 characters)
        /// </summary>
        /// <returns>true if long field</returns>
        public bool IsLongField()
        {
            //	if (m_vo.displayType == DisplayType.String 
            //		|| m_vo.displayType == DisplayType.Text 
            //		|| m_vo.displayType == DisplayType.Memo
            //		|| m_vo.displayType == DisplayType.TextLong
            //		|| m_vo.displayType == DisplayType.Image)
            return (_vo.DisplayLength >= MAXDISPLAY_LENGTH / 2);
        }

        /// <summary>
        /// Is Default focus field
        /// </summary>
        /// <returns></returns>
        public bool IsDefaultFocus()
        {
            return _vo.IsDefaultFocus;
        }


        //public string GetColumnName()
        //{
        //    if (_vo != null)
        //        return _vo.COLUMNNAME;
        //    return null;
        //}	//	getColumnName


        /// <summary>
        /// Is Mandatory
        /// </summary>
        /// <param name="checkContext"></param>
        /// <returns></returns>
        public bool IsMandatory(bool checkContext)
        {
            //  Do we have mandatory logic
            if (checkContext && (_vo.mandatoryLogic != null) && (_vo.mandatoryLogic.Length > 0))
            {
                var retValue = Evaluator.EvaluateLogic(this, _vo.mandatoryLogic);
                log.Finest(_vo.ColumnName + " MandatoryLogic(" + _vo.mandatoryLogic + ") => " + retValue);
                if (retValue)
                    return true;
            }

            //  Not mandatory
            if (!_vo.IsMandatoryUI || IsVirtualColumn())
                return false;

            //  Numeric Keys and Created/Updated as well as 
            //	DocumentNo/Value/ASI ars not mandatory (persistency layer manages them)
            if ((_vo.IsKey && _vo.ColumnName.EndsWith("_ID"))
                    || _vo.ColumnName.StartsWith("Created") || _vo.ColumnName.StartsWith("Updated")
                    || _vo.ColumnName.Equals("Value")
                    || _vo.ColumnName.Equals("DocumentNo")
                    || _vo.ColumnName.Equals("M_AttributeSetInstance_ID"))	//	0 is valid
                return false;

            //  Mandatory if displayed
            return IsDisplayed(checkContext);
        }

        /// <summary>
        ///Is the Column Visible ?
        /// checkContext - check environment (requires correct row position)
        /// </summary>
        /// <param name="checkContext"></param>
        /// <returns></returns>
        public bool IsDisplayed(bool checkContext)
        {
            //  ** static content **
            //  not displayed
            if (!_vo.IsDisplayedf)
                return false;
            //  no restrictions
            if (_vo.DisplayLogic == null || _vo.DisplayLogic.Equals(""))
                return true;

            ////  ** dynamic content **
            if (checkContext)
            {
                bool retValue = false;
                List<bool> checkAll = new List<bool>(5);
                List<string> logical = new List<string>(5);
                //changes done by Jagmohan Bhatt on 5-May-2010
                //purpose: display logic defined by multiple field action
                //best way: [@fieldName1 = <someValue>] &/| [@fieldName2 = <someValue]
                //currently it will support condition from only two fields.
                StringTokenizer bracket = new StringTokenizer(_vo.DisplayLogic, "[]");
                while (bracket.HasMoreTokens())
                {
                    string currentToken = bracket.NextToken();
                    if (currentToken.Trim() != " & ".Trim())
                    {
                        retValue = Evaluator.EvaluateLogic(this, currentToken);
                        checkAll.Add(retValue);
                    }
                    else
                    {
                        logical.Add(currentToken);
                    }
                }

                if (checkAll.Count() > 1)
                {
                    if (logical[0].Trim().Equals("&", StringComparison.OrdinalIgnoreCase))
                    {
                        if (checkAll.Contains(false))
                            retValue = false;
                        else
                            retValue = true;
                    }
                    else if (logical[0].Trim().Equals("|", StringComparison.OrdinalIgnoreCase))
                    {
                        if (checkAll.Contains(true))
                            retValue = true;
                    }
                }
                //    log.finest(_vo.COLUMNNAME
                //        + " (" + _vo.DisplayLogic + ") => " + retValue);
                return retValue;
            }
            return true;
        }	//	isDisplaye

        /// <summary>
        ///	Add Display Dependencies to given List.
        /// </summary>
        /// <param name="list"></param>

        public void AddDependencies(List<String> list)
        {
            //	nothing to parse
            if (!_vo.IsDisplayedf || _vo.DisplayLogic.Equals(""))
                return;

            StringTokenizer logic = new StringTokenizer(_vo.DisplayLogic.Trim(), "&|", false);

            while (logic.HasMoreTokens())
            {
                StringTokenizer st = new StringTokenizer(logic.NextToken().Trim(), "!=^", false);
                while (st.HasMoreTokens())
                {
                    String tag = st.NextToken().Trim();					//	get '@tag@'
                    //	Do we have a @variable@ ?
                    if (tag.IndexOf('@') != -1)
                    {
                        tag = tag.Replace('@', ' ').Trim();				//	strip 'tag'
                        //	Add columns (they might not be a column, but then it is static)
                        if (!list.Contains(tag))
                            list.Add(tag);
                    }
                }
            }
        }






        /// <summary>
        /// is Column sql
        /// </summary>
        /// <returns></returns>
        public bool IsVirtualColumn()
        {
            if (_vo.ColumnSQL != null && _vo.ColumnSQL.Length > 0)
                return true;
            return false;
        }	//	isColumnVirtual

        /// <summary>
        /// Set Read only
        /// </summary>
        /// <param name="val"></param>
        public void SetReadOnly(bool val)
        {
            _vo.IsReadOnly = val;
        }

        /// <summary>
        /// Is Read only
        /// </summary>
        /// <returns></returns>
        public bool IsReadOnly()
        {
            if (IsVirtualColumn())
                return true;
            return _vo.IsReadOnly;
        }

        /// <summary>
        /// Is Heading
        /// </summary>
        /// <returns></returns>
        public bool IsHeading()
        {
            return _vo.IsHeading;
        }


        /// <summary>
        ///Is Encrypted Field (display)
        /// </summary>
        /// <returns></returns>
        public bool IsEncryptedField()
        {
            return _vo.IsEncryptedField;
        }

        /// <summary>
        /// Get Display Length
        /// </summary>
        /// <returns></returns>
        public int GetDisplayLength()
        {
            return _vo.DisplayLength;
        }

        /// <summary>
        /// Get Display Length
        /// </summary>
        /// <returns></returns>
        public string GetMobileListingFormat()
        {
            return _vo.MobileListingFormat;
        }

        /// <summary>
        /// Set Display Length
        /// </summary>
        /// <param name="length">int length</param>
        public void SetDisplayLength(int length)
        {
            _vo.DisplayLength = length;
        }

        /// <summary>
        /// Get Field Length
        /// </summary>
        /// <returns>int value</returns>
        public int GetFieldLength()
        {
            return _vo.FieldLength;
        }
        /// <summary>
        /// Is same line
        /// </summary>
        /// <returns></returns>
        public bool IsSameLine()
        {
            return _vo.IsSameLine;
        }

        /// <summary>
        /// Get Desc
        /// </summary>
        /// <returns></returns>
        public string GetDescription()
        {
            return _vo.Description;
        }
        /// <summary>
        /// Get help text
        /// </summary>
        /// <returns></returns>
        public string GetHelp()
        {
            return _vo.Help;
        }

        /// <summary>
        ///  Overwrite Displayed
        /// </summary>
        /// <param name="displayed"></param>
        public void SetDisplayed(bool displayed)
        {
            _vo.IsDisplayedf = displayed;
        }
        /// <summary>
        /// overwrite updatable
        /// </summary>
        /// <param name="updateable"></param>
        public void SetUpdateable(bool updateable)
        {
            _vo.IsUpdateable = updateable;
        }   //  setDisplayed

        /// <summary>
        /// Get Lookup, may return null
        /// </summary>
        /// <returns></returns>
        public Lookup GetLookup()
        {
            return _lookup;
        }   //  getLookup

        /// <summary>
        ///Is it Editable - checks IsActive, IsUpdateable, and isDisplayed
        /// checkContext if true checks Context for Active, IsProcessed, LinkColumn
        /// </summary>
        /// <param name="checkContext"></param>
        /// <returns></returns>
        public bool IsEditable(bool checkContext)
        {
            if (IsVirtualColumn())
                return false;
            //  Fields always enabled (are usually not updateable)
            if (_vo.ColumnName.Equals("Posted")
                || (_vo.ColumnName.Equals("Record_ID") && _vo.displayType == DisplayType.Button))	//  Zoom
                return true;

            //  Fields always updateable
            if (_vo.IsAlwaysUpdateable)      //  Zoom
                return true;

            //  Tab or field is R/O
            if (_vo.tabReadOnly || _vo.IsReadOnly)
            {
                log.Finest(_vo.ColumnName + " NO - TabRO=" + _vo.tabReadOnly + ", FieldRO=" + _vo.IsReadOnly);
                return false;
            }

            //	Not Updateable - only editable if new updateable row
            if (!_vo.IsUpdateable && !_inserting)
            {
                log.Finest(_vo.ColumnName + " NO - FieldUpdateable=" + _vo.IsUpdateable);
                return false;
            }

            ////	Field is the Link Column of the tab
            if (_vo.ColumnName.Equals(_vo.GetCtx().GetContext(_vo.windowNo, _vo.tabNo, "LinkColumnName")))
            {
                log.Finest(_vo.ColumnName + " NO - LinkColumn");
                return false;
            }

            //	Role Access & Column Access			
            if (checkContext)
            {
                int AD_Client_ID = _vo.GetCtx().GetContextAsInt(_vo.windowNo, _vo.tabNo, "AD_Client_ID");
                int AD_Org_ID = _vo.GetCtx().GetContextAsInt(_vo.windowNo, _vo.tabNo, "AD_Org_ID");
                String keyColumn = _vo.GetCtx().GetContext(_vo.windowNo, _vo.tabNo, "KeyColumnName");
                if ("EntityType".Equals(keyColumn))
                    keyColumn = "AD_EntityType_ID";
                if (!keyColumn.EndsWith("_ID"))
                    keyColumn += "_ID";			//	AD_Language_ID
                int Record_ID = _vo.GetCtx().GetContextAsInt(_vo.windowNo, _vo.tabNo, keyColumn);
                int AD_Table_ID = _vo.AD_Table_ID;
                if (!MRole.GetDefault((Context)_vo.GetCtx(), false).CanUpdate(
                    AD_Client_ID, AD_Org_ID, AD_Table_ID, Record_ID, false))
                {
                    return false;
                }
                if (!MRole.GetDefault((Context)_vo.GetCtx(), false).IsColumnAccess(AD_Table_ID, _vo.AD_Column_ID, false))
                {
                    return false;
                }
            }

            //  Do we have a readonly rule
            if (checkContext && _vo.ReadOnlyLogic.Length > 0)
            {
                bool retValue = !Evaluator.EvaluateLogic(this, _vo.ReadOnlyLogic);
                log.Finest(_vo.ColumnName + " R/O(" + _vo.ReadOnlyLogic + ") => R/W-" + retValue);
                if (!retValue)
                    return false;
            }

            //  Always editable if Active
            if (_vo.ColumnName.Equals("Processing")
                    || _vo.ColumnName.Equals("DocAction")
                    || _vo.ColumnName.Equals("GenerateTo"))
                return true;

            ////  Record is Processed	***	
            if (checkContext
                && (_vo.GetCtx().GetContext(_vo.windowNo, "Processed").Equals("Y")
                    || _vo.GetCtx().GetContext(_vo.windowNo, "Processing").Equals("Y")))
            {
                return false;
            }

            //  IsActive field is editable, if record not processed
            if (_vo.ColumnName.Equals("IsActive"))
            {
                return true;
            }

            //  Record is not Active
            if (checkContext && !(_vo.GetCtx().GetContext(_vo.windowNo, "IsActive").Equals("True") || _vo.GetCtx().GetContext(_vo.windowNo, "IsActive").Equals("Y")))
            {
                return false;
            }

            //  ultimately visibily decides
            return IsDisplayed(checkContext);
        }

        /// <summary>
        /// Is Editable
        /// </summary>
        /// <returns></returns>
        public bool IsEncrypted()
        {
            if (_vo.IsEncryptedField)
                return true;
            string ob = GetObscureType();
            if (ob != null && ob.Length > 0)
                return true;
            return _vo.ColumnName.Equals("Password");
        }

        /// <summary>
        /// is encrypted column
        /// </summary>
        /// <returns></returns>
        public bool IsEncryptedColumn()
        {
            return _vo.IsEncryptedColumn;
        }

        public bool IsSelectionColumn()
        {
            return _vo.IsSelectionColumn;
        }

        public string GetObscureType()
        {
            return _vo.ObscureType;
        }

        public int GetAD_Column_ID()
        {
            return _vo.AD_Column_ID;
        }

        public int GetAD_Field_ID()
        {
            return _vo.AD_Field_ID;
        }
        /// <summary>
        ///	Set Error.
        ///  Used by editors to set the color
        /// </summary>
        /// <param name="error"></param>
        public void SetError(bool error)
        {
            _error = error;
        }

        /// <summary>
        ///  Get Column Name or SQL .. with/without AS
        /// </summary>
        /// <param name="withAS">include AS ColumnName for virtual columns in select statements</param>
        /// <returns></returns>
        public string GetColumnSQL(bool withAS)
        {
            //if (_vo.COLUMNSQL != null && _vo.COLUMNSQL.Length > 0)
            //{
            //    if (withAS)
            //        return _vo.COLUMNSQL + " AS " + _vo.COLUMNNAME;
            //    else
            //        return _vo.COLUMNSQL;
            //}
            //return _vo.COLUMNNAME;

            //(case o.ISACTIVE when 'Y' then 'True' else 'False' end) as Active,
            if (_vo.ColumnSQL != null && _vo.ColumnSQL.Length > 0)
            {
                if (withAS)
                {
                    if (_vo.displayType == DisplayType.YesNo)
                    {
                        return " (case " + _vo.ColumnSQL + " when 'Y' then 'True' else 'False' end) " + " AS " + _vo.ColumnName;
                    }
                    return _vo.ColumnSQL + " AS " + _vo.ColumnName;
                }
                else
                {
                    if (_vo.displayType == DisplayType.YesNo)
                    {
                        return " (case " + _vo.ColumnSQL + " when 'Y' then 'True' else 'False' end) ";
                    }
                    return _vo.ColumnSQL;
                }
            }
            if (_vo.displayType == DisplayType.YesNo)
            {
                return " (case " + _vo.ColumnName + " when 'Y' then 'True' else 'False' end) AS " + _vo.ColumnName;
            }
            return _vo.ColumnName;

        }

        public string GetColumnSQL()
        {
            return _vo.ColumnName;
        }

        public int GetWindowNo()
        {

            return _vo.windowNo;
        }

        public int GetAD_InfoWindow_ID()
        {
            return _vo.AD_InfoWindow_ID;
        }


        public string GetCallout()
        {
            return _vo.Callout == null ? "" : _vo.Callout;
        }

        /// <summary>
        /// Get AD_Process_ID
        /// </summary>
        /// <returns></returns>
        public int GetAD_Process_ID()
        {
            return _vo.AD_Process_ID;
        }

        /// <summary>
        ///Get VFormat
        /// </summary>
        /// <returns></returns>
        public String GetVFormat()
        {
            return _vo.VFormat;
        }

        /// <summary>
        ///Get Value Min
        /// </summary>
        /// <returns></returns>
        public String GetValueMin()
        {
            return _vo.ValueMin;
        }

        /// <summary>
        ///Get Value Max
        /// </summary>
        /// <returns></returns>
        public String GetValueMax()
        {
            return _vo.ValueMax;
        }

        public int GetAD_Reference_Value_ID()
        {
            return _vo.AD_Reference_Value_ID;
        }

        public void SetDisplayType(int displayType)
        {
            _vo.displayType = displayType;
        }

        /// <summary>
        ///Set Inserting (allows to enter not updateable fields).
        /// Reset when setting the Field Value
        /// </summary>
        /// <param name="inserting"></param>
        public void SetInserting(bool inserting)
        {
            _inserting = inserting;
        }   //  setInserting

        /// <summary>
        /// Get Background Error.
        /// </summary>
        /// <returns></returns>
        public bool IsError()
        {
            return _error;
        }	//	isError


        public string GetDefaultValue()
        {
            if (_vo.DefaultValue == null)
            {
                return "";
            }
            return _vo.DefaultValue;

        }


        /// <summary>
        /// Get Default value of field
        /// </summary>
        /// <returns></returns>
        public Object GetDefault()
        {
            /**
             *  (a) Key/Parent/IsActive/SystemAccess*/
            //	No defaults for these fields

            if (_vo.IsKey || _vo.displayType == DisplayType.RowID
                || DisplayType.IsLOB(_vo.displayType))
                return DBNull.Value;// null;
            //	Set Parent to context if not explitly set
            if (IsParentValue()
                && (_vo.DefaultValue == null || _vo.DefaultValue.Length == 0))
            {
                String parent = _vo.GetCtx().GetContext(_vo.windowNo, _vo.ColumnName);
                log.Fine("[Parent] " + _vo.ColumnName + "=" + parent);
                return CreateDefault(_vo.ColumnName, parent);
            }

            //	Always Active
            if (_vo.ColumnName.Equals("IsActive"))
            {
                log.Fine("[IsActive] " + _vo.ColumnName + "=Y");
                return "True";
            }

            //	Set Client & Org to System, if System access
            if (X_AD_Table.ACCESSLEVEL_SystemOnly.Equals(_vo.GetCtx().GetContext(_vo.windowNo, _vo.tabNo, "AccessLevel"))
                && (_vo.ColumnName.Equals("AD_Client_ID") || _vo.ColumnName.Equals("AD_Org_ID")))
            {
                log.Fine("[SystemAccess] " + _vo.ColumnName + "=0");
                return 0;
            }
            //	Set Org to System, if Client access
            else if (X_AD_Table.ACCESSLEVEL_SystemPlusClient.Equals(_vo.GetCtx().GetContext(_vo.windowNo, _vo.tabNo, "AccessLevel"))
                && _vo.ColumnName.Equals("AD_Org_ID"))
            {
                log.Fine("[ClientAccess] " + _vo.ColumnName + "=0");
                return 0;
            }

            /**
		 *  (b) SQL Statement (for data integity & consistency)
		 */
            String defStr = "";
            if (_vo.DefaultValue.ToString().StartsWith("@SQL="))
            {
                String sql0 = _vo.DefaultValue.Substring(5);			//	w/o tag
                String sql = Utility.Env.ParseContext(_vo.GetCtx(), _vo.windowNo, sql0, false, true);	//	replace variables
                String sqlTest = sql.ToUpper();
                if (sqlTest.IndexOf("DELETE ") != -1 && sqlTest.IndexOf("UPDATE ") != -1 && sqlTest.IndexOf("DROP ") != -1)
                    sql = "";	//	Potential security issue
                if (sql.Equals(""))
                {
                    log.Log(Level.WARNING, "(" + _vo.ColumnName + ") - Default SQL variable parse failed: "
                        + sql0);
                }
                else
                {
                    System.Data.IDataReader dr = null;
                    try
                    {
                        dr = SqlExec.ExecuteQuery.ExecuteReader(sql);
                        if (dr.Read())
                        {
                            defStr = dr[0].ToString();//    rs.getString(1);
                        }
                        else
                        {
                            log.Log(Level.WARNING, "(" + _vo.ColumnName + ") - no Result: " + sql);
                        }
                        dr.Close();
                    }
                    catch (System.Data.Common.DbException e)
                    {
                        if (sql.EndsWith("="))	//	Variable Resolved empty
                            log.Log(Level.SEVERE, "(" + _vo.ColumnName + ") " + sql0, e);
                        else
                            log.Log(Level.WARNING, "(" + _vo.ColumnName + ") " + sql, e);
                    }
                    finally { try { dr.Close(); } catch { } };
                }
                if (defStr != null && defStr.Length > 0)
                {
                    log.Fine("[SQL] " + _vo.ColumnName + "=" + defStr);
                    return CreateDefault("", defStr);
                }
            }	//	SQL Statement

            /**
		 * 	(c) Field DefaultValue		=== similar code in AStartRPDialog.getDefault ===
		 */
            if (!_vo.DefaultValue.Equals("") && !_vo.DefaultValue.StartsWith("@SQL="))
            {
                defStr = "";		//	problem is with texts like 'sss;sss'
                //	It is one or more variables/constants
                StringTokenizer st = new StringTokenizer(_vo.DefaultValue, ",;", false);
                while (st.HasMoreElements())
                {
                    String variable = st.NextToken().Trim();
                    if (variable.Equals("@SysDate@"))				//	System Time
                        return new DateTime(CommonFunctions.CurrentTimeMillis());// Timestamp(System.currentTimeMillis());
                    else if (variable.IndexOf('@') != -1)			//	it is a variable
                        defStr = _vo.GetCtx().GetContext(_vo.windowNo, variable.Replace('@', ' ').Trim());
                    else if (defStr.IndexOf("'") != -1)			//	it is a 'String'
                        defStr = variable.Replace('\'', ' ').Trim();
                    else
                        defStr = variable;

                    if (defStr.Length > 0)
                    {
                        log.Fine("[DefaultValue] " + _vo.ColumnName + "=" + defStr);
                        return CreateDefault(variable, defStr);
                    }
                }	//	while more Tokens

            }	//	Default value

            /**
                     *	(d) Preference (user) - P|
                     */
            defStr = Utility.Env.GetPreference(_vo.GetCtx(), _vo.AD_Window_ID, _vo.ColumnName, false);
            if (!defStr.Equals(""))
            {
                log.Fine("[UserPreference] " + _vo.ColumnName + "=" + defStr);
                return CreateDefault("", defStr);
            }

            /**
             *	(e) Preference (System) - # $
             */
            defStr = Utility.Env.GetPreference(_vo.GetCtx(), _vo.AD_Window_ID, _vo.ColumnName, true);
            if (!defStr.Equals(""))
            {
                log.Fine("[SystemPreference] " + _vo.ColumnName + "=" + defStr);
                return CreateDefault("", defStr);
            }

            /**
		 *	(f) DataType defaults
		 */

            //	Button to N
            if (_vo.displayType == DisplayType.Button && !_vo.ColumnName.EndsWith("_ID"))
            {
                log.Fine("[Button=N] " + _vo.ColumnName);
                return "N";
            }
            //	CheckBoxes default to No
            if (_vo.displayType == DisplayType.YesNo)
            {
                log.Fine("[YesNo=N] " + _vo.ColumnName);
                return "False";
            }
            //  IDs remain null
            if (_vo.ColumnName.EndsWith("_ID"))
            {
                log.Fine("[ID=null] " + _vo.ColumnName);
                return DBNull.Value;
                //return null;
            }
            //  actual Numbers default to zero
            if (DisplayType.IsNumeric(_vo.displayType))
            {
                log.Fine("[Number=0] " + _vo.ColumnName);
                return CreateDefault("", "0");
            }
            //No Resolation
            log.Fine("[NONE] " + _vo.ColumnName);
            return DBNull.Value;
            //return null;
        }
        /// <summary>
        ///Parent Link Value
        ///	@return parent value
        /// </summary>
        /// <returns></returns>
        public bool IsParentValue()
        {
            if (_parentValue != null)
                return (bool)_parentValue;
            if (!DisplayType.IsID(_vo.displayType) || _vo.tabNo == 0)
                _parentValue = false;
            else
            {
                String linkColumnName = _vo.GetCtx().GetContext(_vo.windowNo, _vo.tabNo, "LinkColumnName");
                if (linkColumnName.Length == 0)
                    _parentValue = false;
                else
                    _parentValue = _vo.ColumnName.Equals(linkColumnName);
                if ((bool)_parentValue)
                {
                    log.Config(_parentValue
                        + " - Link(" + linkColumnName + ", W=" + _vo.windowNo + ",T=" + _vo.tabNo
                        + ") = " + _vo.ColumnName);
                }
            }
            return (bool)_parentValue;
        }	//	isParentValue

        /// <summary>
        ///	Create Default Object type.
        ///  <pre>
        ///		Integer 	(IDs, Integer)
        ///		BigDecimal 	(Numbers)
        ///		Timestamp	(Dates)
        ///		Boolean		(YesNo)
        ///		default: String
        ///  </pre>
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        /// <returns>type dependent converted object</returns>
        private Object CreateDefault(String variable, String value)
        {
            //	true NULL
            if (value == null || value.Length == 0)
                return DBNull.Value;
            //	see also MTable.readData
            try
            {
                //	IDs & Integer & CreatedBy/UpdatedBy
                if (_vo.ColumnName.EndsWith("atedBy")
                        || _vo.ColumnName.EndsWith("_ID"))
                {
                    try	//	defaults -1 => null
                    {
                        int ii = int.Parse(value);
                        if (ii < 0)
                            return DBNull.Value;
                        return ii;
                    }
                    catch (Exception e)
                    {
                        log.Warning("Cannot parse: " + value + " - " + e.Message);
                    }
                    return 0;
                }
                //	Integer
                if (_vo.displayType == DisplayType.Integer)
                {
                    int ii;
                    if (int.TryParse(value, out ii))
                    {
                        return ii;
                    }
                    return 0;
                }

                //	Number
                if (DisplayType.IsNumeric(_vo.displayType))
                {
                    decimal d;
                    if (decimal.TryParse(value, out d))
                    {
                        return d;
                    }
                    return new decimal(0);
                }

                //	Timestamps
                if (DisplayType.IsDate(_vo.displayType))
                {
                    //	Time
                    DateTime date = DateTime.Now;

                    try
                    {
                        //if (_vo.displayType == DisplayType.Date)
                        //{
                        //    //return CommonFunctions.CovertMilliToDate(CommonFunctions.CurrentTimeMillis());
                        //    value = Convert.ToDateTime(DataBase.GlobalVariable.SetDateFormat(Convert.ToDateTime(value), true)).ToString();
                        //}
                        //else
                        //{
                        //    value = Convert.ToDateTime(DataBase.GlobalVariable.SetDateFormat(Convert.ToDateTime(value), false)).ToString();
                        //}

                        date = CommonFunctions.CovertMilliToDate(System.Convert.ToInt64(value));


                        if (_vo.displayType == DisplayType.Date)
                        {
                            //return CommonFunctions.CovertMilliToDate(CommonFunctions.CurrentTimeMillis());
                            return Convert.ToDateTime(DataBase.GlobalVariable.SetDateFormat(date, true)).ToString();
                        }
                        else
                        {
                            return Convert.ToDateTime(DataBase.GlobalVariable.SetDateFormat(date, false)).ToString();
                        }
                    }
                    catch
                    {
                    }


                    //	Date yyyy-mm-dd hh:mm:ss.fffffffff
                    String tsString = value;
                    //+ "2007-01-01 00:00:00.00".Substring(value.Length);
                    try
                    {
                        date = Convert.ToDateTime(tsString);

                        if (_vo.displayType == DisplayType.Date)
                        {
                            //return CommonFunctions.CovertMilliToDate(CommonFunctions.CurrentTimeMillis());
                            return Convert.ToDateTime(DataBase.GlobalVariable.SetDateFormat(date, true));
                        }
                        else
                        {
                            return Convert.ToDateTime(DataBase.GlobalVariable.SetDateFormat(date, false));
                        }
                    }
                    catch (Exception e)
                    {
                        log.Warning("Cannot convert to Timestamp: " + tsString + e.Message);
                    }


                    if (_vo.displayType == DisplayType.Date)
                    {
                        //return CommonFunctions.CovertMilliToDate(CommonFunctions.CurrentTimeMillis());
                        return Convert.ToDateTime(DataBase.GlobalVariable.SetDateFormat(DateTime.Now, true));
                    }
                    else
                    {
                        return Convert.ToDateTime(DataBase.GlobalVariable.SetDateFormat(DateTime.Now, false));
                    }
                }

                //	Boolean
                if (_vo.displayType == DisplayType.YesNo)
                    return ("Y".Equals(value) || "True".Equals(value)).ToString();

                //	Default - String
                if (variable.Equals("@#Date@"))
                {
                    try
                    {	//	2007-06-27 00:00:00.0
                        long time = long.Parse(value);
                        value = CommonFunctions.CovertMilliToDate(time).ToString("yyyy-MM-dd");
                        //value = new DateTime(time).ToString();
                        //value = value.Substring(0, 10);
                    }
                    catch
                    {
                    }
                    return value;
                }

                if ((value.ToString() == "Y" || value.ToString() == "N") && _vo.displayType != DisplayType.Button && !DisplayType.IsLookup(_vo.displayType))
                {
                    return (value.ToString() == "Y") ? "True" : "False";
                }

                return value;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, _vo.ColumnName + " - " + e.Message);
            }
            return DBNull.Value;
        }

        /// <summary>
        /// Refresh Lookup if the lookup is unstable
        /// </summary>
        /// <returns>true if lookup is validated</returns>
        public bool RefreshLookup()
        {
            ////  if there is a validation string, the lookup is unstable
            if (_lookup == null && _lookup.GetValidation().Length.Equals(""))
                return true;
            ////
            log.Fine("(" + _vo.ColumnName + ")");
            _lookup.Refresh();
            return _lookup.IsValidated();
        }

        /// <summary>
        /// interface evaultee
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        public string GetValueAsString(string variableName)
        {
            return _vo.GetCtx().GetContext(_vo.windowNo, variableName, true);
        }

        /// <summary>
        /// Is Always Updateable
        /// </summary>
        /// <returns></returns>
        public bool IsAlwaysUpdateable()
        {
            if (IsVirtualColumn() || !_vo.IsUpdateable)
                return false;
            return _vo.IsAlwaysUpdateable;
        }

        /// <summary>
        /// Get old/previous Value. Called from MTab.ProcessCallout
        /// </summary>
        /// <returns>old value</returns>
        public Object GetOldValue()
        {
            return _oldValue;
        }

        /// <summary>
        /// Set Value to null.
        /// Do not update context - called from GridTab.SetCurrentRow
        /// Send Bean PropertyChange if there is a change
        /// </summary>
        //public void SetValue()
        //{
        //    //	log.fine(ColumnName + "=" + newValue);
        //    if (_valueNoFire)      //  set the old value
        //        _oldValue = _value;
        //    _value = null;
        //    _inserting = false;
        //    _error = false;        //  reset error

        //    //  Does not fire, if same value
        //    //m_propertyChangeListeners.firePropertyChange(PROPERTY, m_oldValue, m_value);
        //} 

        /// <summary>
        /// Set Value.
        /// Update context, if not text or RowID;
        /// Send Bean PropertyChange if there is a change
        /// </summary>
        /// <param name="newValue">new value</param>
        /// <param name="inserting">true if inserting</param>
        public void SetValue(Object newValue, bool inserting)
        {
            //	log.fine(ColumnName + "=" + newValue);
            if (!inserting)      //  set the old value
                _oldValue = _value;
            _value = newValue;
            _inserting = inserting;
            _error = false;        //  reset error

            //	Set Context
            if (_vo.displayType == DisplayType.Text
                || _vo.displayType == DisplayType.Memo
                || _vo.displayType == DisplayType.TextLong
                || _vo.displayType == DisplayType.Binary
                || _vo.displayType == DisplayType.RowID
                || IsEncrypted())
            {
            }
            else if (newValue is DBNull || newValue == null)
            {
                _vo.GetCtx().SetContext(_vo.windowNo, _vo.ColumnName, null);
            }
            else if (newValue.GetType() == typeof(Boolean) || _vo.displayType == DisplayType.YesNo)
            {
                if (newValue.ToString() == "Y" || newValue.ToString() == "N")
                {
                    _vo.GetCtx().SetContext(_vo.windowNo, _vo.ColumnName, newValue.ToString());
                }
                else
                    _vo.GetCtx().SetContext(_vo.windowNo, _vo.ColumnName, bool.Parse(newValue.ToString()) ? "Y" : "N");
            }
            else if (newValue.GetType() == typeof(DateTime) || DisplayType.IsDate(_vo.displayType))
            {
                _vo.GetCtx().SetContext(_vo.windowNo, _vo.ColumnName, Convert.ToDateTime(_value));
            }

            else if (_vo.displayType == DisplayType.Integer
                || (DisplayType.IsID(_vo.displayType) // JJ: don't touch!
                    && (_vo.ColumnName.EndsWith("_ID") || _vo.ColumnName.EndsWith("_Acct") || _vo.ColumnName.EndsWith("_ID_1")
                || _vo.ColumnName.EndsWith("_ID_2") || _vo.ColumnName.EndsWith("_ID_3")))
                || _vo.ColumnName.EndsWith("atedBy"))
            {
                _value = Convert.ToInt32(_value);
                _vo.GetCtx().SetContext(_vo.windowNo, _vo.ColumnName, Convert.ToInt32(_value));
            }
            else
            {
                _vo.GetCtx().SetContext(_vo.windowNo, _vo.ColumnName, newValue == null ? null : newValue.ToString());
            }

            //  Does not fire, if same value

            //Object oldValue = _oldValue;
            //if (inserting)
            //    _oldValue = INSERTING;
            //_propertyChangeListeners.firePropertyChange(PROPERTY, oldValue, _value);

            //if (newValue != null && _vControl != null)
            //{
            //    try
            //    {
            //        _vControl.SetValue(newValue);
            //    }
            //    catch (Exception ex)
            //    {
            //        try
            //        {
            //            _vControl.SetValue(newValue);
            //        }
            //        catch (Exception ex1)
            //        { 

            //        }
            //    }
            //}
        }

        /// <summary>
        /// Set Value called when cretae default for addes row
        /// </summary>
        /// <param name="newValue"></param>
        public void SetValue(Object newValue)
        {
            //	log.fine(ColumnName + "=" + newValue);
            //  set the old value
            SetValue(newValue, false);
            _oldValue = INSERTING;
            _inserting = true;
        }

        /// <summary>
        /// is Inserting
        /// </summary>
        /// <returns></returns>
        public bool IsInserting()
        {
            return _inserting;
        }

        /// <summary>
        /// Release Resource
        /// </summary>
        public void Dispose()
        {
            if (_lookup != null)
                _lookup.Dispose();
            _lookup = null;
            _vo.lookupInfo = null;
            _vo = null;
        }

        /// <summary>
        /// Create Mnemonic for field
        /// </summary>
        /// <returns></returns>
        public bool IsCreateMnemonic()
        {
            if (IsReadOnly()
                || _vo.ColumnName.Equals("AD_Client_ID")
                || _vo.ColumnName.Equals("AD_Org_ID")
                || _vo.ColumnName.Equals("DocumentNo"))
                return false;
            return true;
        }

        /// <summary>
        /// Get Label Mnemonic
        /// </summary>
        /// <returns></returns>
        public char GetMnemonic()
        {
            return _mnemonic;
        }	//	getMnemonic

        /**
         * 	Set Label Mnemonic
         *	@param mnemonic Mnemonic
         */
        public void SetMnemonic(char mnemonic)
        {
            _mnemonic = mnemonic;
        }	//	setMnemonic

        /// <summary>
        ///  String representation
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("GridField[")
                .Append(_vo.ColumnName).Append("=").Append(_value);
            if (IsKey())
                sb.Append("(Key)");
            if (IsParentColumn())
                sb.Append("(Parent)");
            sb.Append("]");
            return sb.ToString();
        }

        /**
         *  Extended String representation
         *  @return string representation
         */
        public String ToStringX()
        {
            StringBuilder sb = new StringBuilder("MField[");
            sb.Append(_vo.ColumnName).Append("=").Append(_value)
                .Append(",DisplayType=").Append(GetDisplayType())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        ///Create Fields.
        /// 	Used by APanel.cmd_find  and  Viewer.cmd_find
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="WindowNo"></param>
        /// <param name="TabNo"></param>
        /// <param name="AD_Tab_ID"></param>
        /// <param name="AD_UserDef_Win_ID"></param>
        /// <returns></returns>
        public static GridField[] CreateFields(Ctx ctx, int windowNo, int tabNo,
             int AD_Tab_ID, int AD_UserDef_Win_ID)
        {
            List<GridFieldVO> listVO = new List<GridFieldVO>();
            int AD_Window_ID = 0;
            bool readOnly = false;

            String[] stdFieldNames = new String[] { "Created", "CreatedBy", "Updated", "UpdatedBy" };
            bool[] stdFieldsFound = new bool[] { false, false, false, false };

            String sql = GridFieldVO.GetSQL(ctx, AD_UserDef_Win_ID);
            System.Data.IDataReader dr = null;
            System.Data.SqlClient.SqlParameter[] param = null;
            try
            {
                param = new System.Data.SqlClient.SqlParameter[1];
                param[0] = new System.Data.SqlClient.SqlParameter("@tabID", AD_Tab_ID);

                dr = DataBase.DB.ExecuteReader(sql, param);
                while (dr.Read())
                {
                    GridFieldVO vo = GridFieldVO.Create(ctx, windowNo, tabNo,
                        AD_Window_ID, AD_Tab_ID, readOnly, dr);
                    listVO.Add(vo);
                    String columnName = vo.ColumnName;
                    for (int i = 0; i < stdFieldsFound.Length; i++)
                    {
                        if (stdFieldNames[i].Equals(columnName))
                            stdFieldsFound[i] = true;
                    }
                }
                dr.Close();
                dr = null;
                param = null;
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                param = null;
                VLogger.Get().Log(Level.SEVERE, sql, e);
            }


            //	Standard Fields
            if (!stdFieldsFound[0])
                listVO.Add(GridFieldVO.CreateStdField(ctx, windowNo, tabNo, AD_Window_ID, AD_Tab_ID, false, true, true));
            if (!stdFieldsFound[1])
                listVO.Add(GridFieldVO.CreateStdField(ctx, windowNo, tabNo, AD_Window_ID, AD_Tab_ID, false, true, false));
            if (!stdFieldsFound[2])
                listVO.Add(GridFieldVO.CreateStdField(ctx, windowNo, tabNo, AD_Window_ID, AD_Tab_ID, false, false, true));
            if (!stdFieldsFound[3])
                listVO.Add(GridFieldVO.CreateStdField(ctx, windowNo, tabNo, AD_Window_ID, AD_Tab_ID, false, false, false));
            //
            GridField[] retValue = new GridField[listVO.Count];
            for (int i = 0; i < listVO.Count; i++)
                retValue[i] = new GridField(listVO[i]);
            return retValue;
        }	//	createFields

        /// <summary>
        /// Validate initial Field Value.
        /// Called from MTab.dataNew and MTab.setCurrentRow when inserting
        /// </summary>
        /// <returns>true if valid</returns>
        public bool ValidateValue()
        {
            //	Search not cached
            if (GetDisplayType() == DisplayType.Search && _lookup != null)
            {
                // need to re-set invalid values - OK BPartner in PO Line - not OK SalesRep in Invoice
                if (_lookup.GetDirect(_value, false, true) == null)
                {
                    log.Finest(_vo.ColumnName + " Search not valid - set to null");
                    SetValue(null, _inserting);
                }
            }

            //  null
            if (_value == null || _value.ToString().Length == 0)
            {
                if (IsMandatory(true))
                {
                    _error = true;
                    return false;
                }
                else
                {
                    return true;
                }
            }

            //  cannot be validated
            if (!IsLookup()
                || _lookup.Get(_value) != null)
            {
                return true;
            }
            //	it's not null, a lookup and does not have the key
            if (IsKey() || IsParentValue())		//	parents/key are not validated
            {
                return true;
            }

            log.Config(_vo.ColumnName + "=" + _value + " not found - set to null");
            SetValue(null, _inserting);
            _error = true;
            return false;
        }

        /// <summary>
        /// Get LookupInfo, may return null;
        /// </summary>
        /// <returns></returns>
        /// bu raghu
        public VLookUpInfo GetLookupInfo()
        {
            return _vo.lookupInfo;
        }


        public bool LoadRecursiveData()
        {
            return _vo.LoadRecursiveData;
        }
    }
}
