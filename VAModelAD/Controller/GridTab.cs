/********************************************************
// Module Name    : Run Time Show Window
// Purpose        : Tab model.
                    a combination of AD_Tab (the display attributes) and AD_Table information.
                    and Set Field value & perform callout
// Class Used     : GlobalVariable.cs, CommonFunctions.cs,Context.cs
// Created By     : Harwinder 
// Date           : 20 jan 2009
**********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.Logging;
using System.Threading;
using VAdvantage.Model;
using System.Reflection;
using VAdvantage.Controller;
using VAdvantage.DataBase;
namespace VAdvantage.Model
{
    public class GridTab : Classes.Evaluatee
    {

        #region "Declaration"

        /** Value Object                    */
        public GridTabVO _vo;
        public GridTable _gridTable;
        public List<GridTabPanel> _panels;
        private GridWindowVO _wVo;

        private string _keyColumnName = "";
        public string _linkColumnName = "";
        public string _extendedWhere;

        private MultiMap<string, GridField> _depOnField = new MultiMap<string, GridField>();

        private volatile bool _loadComplete;
        /** Is Tab Included in other Tab  */
        private bool _included = false;

        private Query _query = new Query();
        private String _oldQuery = "0=9";
        private String _linkValue = "999999";

        /** Order By Array if SortNo 1..3   */
        private string[] _OrderBys = new string[3];
        /** List of Key Parents     */
        private List<string> _parents = new List<String>(2);

        /** Locks		        */
        private List<int> _Lock = null;
        // Attachments
        public List<AttachmentList> _attachments = null;
        // Attachments
        public List<AttachmentList> _documents = null;
        // Attachments
        public List<int> _pAccess = null;
        // Attachments
        public List<AttachmentList> _subscribe = null;

        //export
        public List<AttachmentList> _exports = null;

        // current row
        private int _currentRow = 0;

        private DataRowView _rowData = null;

        private bool _isCalloutExecute = false;

        private bool _isAllowLetter = false;

        private bool hasPanels = false;

        


        /**	Logger			*/
        protected VLogger log = VLogger.GetVLogger(typeof(GridTab).FullName);


        public bool IsRowDirty
        {
            get;
            set;
        }

        /// <summary>
        /// is Letter Tool Bar item is enabled
        /// </summary>
        public bool IsAllowLetter
        {
            get
            {
                return _isAllowLetter;
            }
        }

        /** Chats				*/
        public List<AttachmentList> _chats = null;

        #endregion

        /// <summary>
        /// Gets and Sets current row of vdatagridview
        /// </summary>
        public int CurrentRow
        {
            get
            {
                //if (GridTable().CurrentRow != null)
                //    return GridTable().CurrentRow.Index;
                //else
                //    return -1;
                return _currentRow;
            }
            set
            {
                _currentRow = value;
                if (_gridTable != null)
                {
                    _gridTable.CurrentRow = _currentRow;

                }
            }
        }

        /// <summary>
        /// current selected row data
        /// </summary>
        public DataRowView CurrentRowData
        {
            get
            {
                return _rowData;
            }
            set
            {
                _rowData = value;

                SetCurrentRow();
                // }
            }
        }
        public bool NewRow
        {
            get;
            set;
        }

        /// <summary>
        /// Set Current Row value in Field Obj
        /// </summary>
        /// <returns></returns>
        private int SetCurrentRow()
        {
            //int oldCurrentRow = _currentRow;
            //_currentRow = verifyRow(newCurrentRow);
            // log.fine("Row=" + _currentRow + " - fire=" + fireEvents);

            //  Update Field Values
            bool setContextDataRowView = true;
            if (!_gridTable.SetOriginalData(_rowData))
            {
                setContextDataRowView = false;
            }

            if (!NewRow)
            {
                int size = _gridTable.GetColumnCount();
                GridField mField = null;
                for (int i = 0; i < size; i++)
                {
                    mField = _gridTable.GetField(i);
                    //  get Value from Table
                    //Object value = null;
                    if (setContextDataRowView)
                    {
                        // value = _rowData[i];
                        mField.SetValue(_rowData[i], _gridTable.IsInserting());
                    }
                    else if (!mField.IsParentValue())
                    {

                        if (!mField.GetDefaultValue().StartsWith("@"))
                        {
                            mField.SetValue(DBNull.Value, _gridTable.IsInserting());
                        }

                        //mField.SetValue(DBNull.Value, _gridTable.IsInserting());
                    }

                    // gwu: now always validated, not just when inserting
                    //mField.validateValue();

                    // else
                    // {   //  no rows - set to a reasonable value - not updateable
                    //				Object value = null;
                    //				if (mField.isKey() || mField.isParent() || mField.getColumnName().equals(m_linkColumnName))
                    //					value = mField.getDefault();
                    //    mField.setValue();
                    // }
                }
            }

            // _gridTable.SetCurrentRow(_currentRow);
            LoadDependentInfo();
            return _currentRow;
        }   //  setCurrentR



        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="vo"></param>
        /// <param name="curDays"></param>
        public GridTab(GridTabVO vo, int curDays, GridWindowVO wVo)
        {
            _wVo = wVo;
            _vo = vo;
            _vo.onlyCurrentDays = curDays;
            _gridTable = new GridTable(_vo.GetCtx(), _vo.AD_Table_ID, _vo.TableName, _vo.windowNo, _vo.tabNo, true, this);
            _gridTable.SetReadOnly(_vo.IsReadOnly || _vo.IsView);
            _gridTable.SetDeleteable(_vo.IsDeleteable);
            IniTab();
            LoadData(_vo.GetCtx().GetAD_User_ID());
            InitIsAllowLetter();

        }

        /// <summary>
        /// Set Letter Flag
        /// Is Letter tool bar item is Enabled
        /// </summary>
        private void InitIsAllowLetter()
        {
            //try
            //{
            //    _isAllowLetter = DataBase.DB.ExecuteScalar("Select IsAllowLetter From Ad_Tab where AD_Tab_ID = " + _vo.AD_Tab_ID).ToString() == "Y" ? true : false;
            //}
            //catch (Exception e)
            //{
            //    log.Warning("InitisAllowLetter ----->" + e.Message);
            //    _isAllowLetter = false;
            //}
            _isAllowLetter = true;
        }

        /// <summary>
        ///Initialize Tab with record from AD_Tab_v
        /// </summary>
        /// <returns></returns>
        protected bool IniTab()
        {
            log.Fine("#" + _vo.tabNo + " - Async= false - Where=" + _vo.WhereClause);
            _extendedWhere = _vo.WhereClause;

            if (!LoadFields())
            {
                _loadComplete = true;
                return false;
            }

            if (_vo.HasPanels)
            {
                LoadPanels();
            }
            else
            {
                hasPanels = false;
            }
            //order by
            _gridTable.SetOrderClause(GetOrderByClause(_vo.onlyCurrentDays));
            _loadComplete = true;
            if (_loadComplete) { }
            return true;
        }

        /// <summary>
        /// Set Order By Clause
        /// </summary>
        /// <param name="onlyCurrentDays"></param>
        /// <returns></returns>
        private string GetOrderByClause(int onlyCurrentDays)
        {
            //	First Prio: Tab Order By
            if (_vo.OrderByClause.Length > 0)
                return _vo.OrderByClause;

            //	Second Prio: Fields (save it)
            _vo.OrderByClause = "";
            for (int i = 0; i < 3; i++)
            {
                string order = _OrderBys[i];
                if (order != null && order != "" && order.Length > 0)
                {
                    if (_vo.OrderByClause.Length > 0)
                        _vo.OrderByClause += ",";
                    _vo.OrderByClause += order;
                }
            }
            if (_vo.OrderByClause.Length > 0)
                return _vo.OrderByClause;

            //	Third Prio: onlyCurrentRows
            _vo.OrderByClause = "Created";
            if (onlyCurrentDays > 0)
                _vo.OrderByClause += " DESC";
            return _vo.OrderByClause;
        }	//	getOrderByClause

        /// <summary>
        /// Get Field data and add to MTable, if it's required or displayed.
        /// </summary>
        /// <returns></returns>
        protected bool LoadFields()
        {
            log.Fine("#" + _vo.tabNo);
            //  Add Fields
            for (int f = 0; f < _vo.GetFields().Count; f++)
            {
                GridFieldVO voF = (GridFieldVO)_vo.GetFields()[f];
                //	Add Fields to Table
                if (voF != null)
                {
                    GridField field = new GridField(voF);
                    String columnName = field.GetColumnName();
                    //	Record Info
                    if (field.IsKey())
                        _keyColumnName = columnName;
                    //	Parent Column(s)
                    if (field.IsParentColumn())
                        _parents.Add(columnName);
                    //	Order By
                    int sortNo = field.GetSortNo();
                    if (sortNo == 0)
                    { }
                    else if (Math.Abs(sortNo) == 1)
                    {
                        _OrderBys[0] = columnName;
                        if (sortNo < 0)
                            _OrderBys[0] += " DESC";
                    }
                    else if (Math.Abs(sortNo) == 2)
                    {
                        _OrderBys[1] = columnName;
                        if (sortNo < 0)
                            _OrderBys[1] += " DESC";
                    }
                    else if (Math.Abs(sortNo) == 3)
                    {
                        _OrderBys[2] = columnName;
                        if (sortNo < 0)
                            _OrderBys[2] += " DESC";
                    }
                    //  Add field
                    _gridTable.AddField(field);

                    //  List of ColumnNames, this field is dependent on
                    List<string> list = field.GetDependentOn();
                    for (int i = 0; i < list.Count; i++)
                        _depOnField.Insert(list[i], field);   //  ColumnName, Field
                    //  Add fields all fields are dependent on
                    if (columnName.Equals("IsActive")
                        || columnName.Equals("Processed")
                        || columnName.Equals("Processing"))
                        _depOnField.Insert(columnName, null);
                }
            }   //  for all fields

            //  Add Standard Fields
            if (_gridTable.GetField("Created") == null)
            {
                GridField created = new GridField(GridFieldVO.CreateStdField(_vo.GetCtx(),
                    _vo.windowNo, _vo.tabNo,
                    _vo.AD_Window_ID, _vo.AD_Tab_ID, false, true, true));
                _gridTable.AddField(created);
            }
            if (_gridTable.GetField("CreatedBy") == null)
            {
                GridField createdBy = new GridField(GridFieldVO.CreateStdField(_vo.GetCtx(),
                    _vo.windowNo, _vo.tabNo,
                    _vo.AD_Window_ID, _vo.AD_Tab_ID, false, true, false));
                _gridTable.AddField(createdBy);
            }
            if (_gridTable.GetField("Updated") == null)
            {
                GridField updated = new GridField(GridFieldVO.CreateStdField(_vo.GetCtx(),
                    _vo.windowNo, _vo.tabNo,
                    _vo.AD_Window_ID, _vo.AD_Tab_ID, false, false, true));
                _gridTable.AddField(updated);
            }
            if (_gridTable.GetField("UpdatedBy") == null)
            {
                GridField updatedBy = new GridField(GridFieldVO.CreateStdField(_vo.GetCtx(),
                    _vo.windowNo, _vo.tabNo,
                    _vo.AD_Window_ID, _vo.AD_Tab_ID, false, false, false));
                _gridTable.AddField(updatedBy);
            }
            return true;
        }

        protected void LoadPanels()
        {
            _panels = new List<GridTabPanel>();
            log.Fine("#" + _vo.tabNo);
            //  Add Fields
            for (int f = 0; f < _vo.GetPanels().Count; f++)
            {
                GridTabPanelVO voF = (GridTabPanelVO)_vo.GetPanels()[f];
                GridTabPanel panel = new GridTabPanel(voF);
                _panels.Add(panel);
            }
        }

        /// <summary>
        /// Get a list of variables, this tab is dependent on.
        /// </summary>
        /// <returns></returns>
        public List<String> GetDependentOn()
        {
            List<String> list = new List<String>();
            //  Display
            Evaluator.ParseDepends(list, _vo.DisplayLogic);
            //
            if (list.Count > 0 && VLogMgt.IsLevelFiner())
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < list.Count; i++)
                    sb.Append(list[i]).Append(" ");
                log.Finer("(" + _vo.Name + ") " + sb.ToString());
            }
            return list;
        }

        /// <summary>
        /// Get Link Col Name
        /// </summary>
        /// <returns></returns>
        public string GetLinkColumnName()
        {
            return _linkColumnName;
        }


        /// <summary>
        /// Get Link Col Name
        /// </summary>
        /// <returns></returns>
        public List<AttachmentList> GetAttachments()
        {
            return _attachments;
        }


        /// <summary>
        /// Get Link Col Name
        /// </summary>
        /// <returns></returns>
        public List<AttachmentList> GetChats()
        {
            return _chats;
        }

        /// <summary>
        /// Get Link Col Name
        /// </summary>
        /// <returns></returns>
        public List<AttachmentList> GetSubscribed()
        {
            return _subscribe;
        }

        /// <summary>
        /// Get Link Col Name
        /// </summary>
        /// <returns></returns>
        public List<int> GetPAccess()
        {
            return _pAccess;
        }

        /// <summary>
        /// Get Link Col Name
        /// </summary>
        /// <returns></returns>
        public List<AttachmentList> GetMarkToExport()
        {
            return _exports;
        }

        /// <summary>
        /// Get Link Col Name
        /// </summary>
        /// <returns></returns>
        public List<AttachmentList> GetDocuments()
        {
            return _documents;
        }


        /// <summary>
        ///Is Printed (Document can be printed)
        /// </summary>
        /// <returns></returns>
        public bool IsPrinted()
        {
            return _vo.AD_Process_ID != 0;
        }	//	isPrinted


        /// <summary>
        /// Get Parent COl Names
        /// </summary>
        /// <returns></returns>
        public List<string> GetParentColumnNames()
        {
            return _parents;
        }

        /// <summary>
        /// Set Link Col Name
        /// </summary>
        /// <param name="linkColumnName"></param>
        public void SetLinkColumnName(string linkColumnName)
        {
            if (linkColumnName != null)
                _linkColumnName = linkColumnName;
            else
            {
                if (_vo.AD_Column_ID == 0)
                    return;
                //	we have a link column identified (primary parent column)
                else
                {
                    string SQL = "SELECT ColumnName FROM AD_Column WHERE AD_Column_ID=" + _vo.AD_Column_ID;
                    try
                    {
                        _linkColumnName = DataBase.DB.ExecuteScalar(SQL).ToString();
                    }
                    catch (Exception e)
                    {
                        log.Log(Level.SEVERE, "", e);
                    }
                    log.Fine("AD_Column_ID=" + _vo.AD_Column_ID + " - " + _linkColumnName);
                }
            }
            _vo.GetCtx().SetContext(_vo.windowNo, _vo.tabNo, "LinkColumnName", _linkColumnName);
        }

        /// <summary>
        /// Is Tab Incluced in other Tab
        /// </summary>
        /// <returns></returns>
        public bool IsIncluded()
        {
            return _included;
        }

        /// <summary>
        /// Is Tab Incluced in other Tab
        /// </summary>
        /// <param name="isIncluded"></param>
        public void SetIncluded(bool isIncluded)
        {
            _included = isIncluded;
        }   //  setIncluded



        /// <summary>
        /// Is High Volume(show Find Window)
        /// </summary>
        /// <returns></returns>
        public bool IsHighVolume()
        {
            return _vo.IsHighVolume;
        }

        /// <summary>
        /// IS Sort Tab
        /// </summary>
        /// <returns></returns>
        public bool IsSortTab()
        {
            return _vo.IsSortTab;
        }
        /// <summary>
        /// Has Tree
        /// </summary>
        /// <returns>true if tree exists</returns>
        public bool IsTreeTab()
        {
            return _vo.HasTree;
        }
        /// <summary>
        /// Get Extended Where Clause
        /// </summary>
        /// <returns></returns>
        public string GetWhereExtended()
        {
            return _extendedWhere;
        }
        /// <summary>
        /// Get Tab Where Clause
        /// </summary>
        /// <returns></returns>
        public string GetWhereClause()
        {
            return _vo.WhereClause;
        }

        /// <summary>
        ///Get Table Name
        /// </summary>
        /// <returns></returns>
        public string GetTableName()
        {
            return _vo.TableName;
        }

        /// <summary>
        /// Table Id
        /// </summary>
        /// <returns></returns>
        public int GetAD_Table_ID()
        {
            return _vo.AD_Table_ID;
        }

        /// <summary>
        ///Return the name of the key column - may be ""
        /// </summary>
        /// <returns></returns>
        public string GetKeyColumnName()
        {
            return _keyColumnName;
        }

        /// <summary>
        /// return filed by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public GridField GetField(int index)
        {
            return _gridTable.GetField(index);
        }

        /// <summary>
        /// //return Total Fields in Tab
        /// </summary>
        /// <returns></returns>
        public int GetFieldCount()
        {
            return _gridTable.GetColumnCount();
        }

        /// <summary>
        /// Tab ID
        /// </summary>
        /// <returns></returns>
        public int GetTabID()
        {
            return _vo.AD_Tab_ID;
        }

        /// <summary>
        /// Tab Name
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return _vo.Name;
        }

        /// <summary>
        /// Description
        /// </summary>
        /// <returns></returns>
        public string GetDescription()
        {
            return _vo.Description;
        }

        /// <summary>
        /// //Help
        /// </summary>
        /// <returns></returns>
        public string GetHelp()
        {
            return _vo.Help;
        }

        /// <summary>
        /// Get Commit Warning
        /// </summary>
        /// <returns>commit warning</returns>
        public String GetCommitWarning()
        {
            return _vo.CommitWarning;
        }

        /// <summary>
        /// Get Order column for sort tab
        /// </summary>
        /// <returns></returns>
        public int GetAD_ColumnSortOrder_ID()
        {
            return _vo.AD_ColumnSortOrder_ID;
        }

        /// <summary>
        ///	Get Yes/No column for sort tab
        /// </summary>
        /// <returns></returns>
        public int GetAD_ColumnSortYesNo_ID()
        {
            return _vo.AD_ColumnSortYesNo_ID;
        }	//	getAD_ColumnSortYesNo_ID


        /*public int GetMaxFieldLenght()
        {
            return _vo.GetMaxLength();
        }*/

        /// <summary>
        /// Get Field by DB column name
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public GridField GetField(string columnName)
        {
            return _gridTable.GetField(columnName);
        }   //  GetField

        //public GridField getField(string columnName)
        //{
        //    return _gridTable.getField(columnName);
        //}   

        /// <summary>
        ///return fields Array
        /// </summary>
        /// <returns></returns>
        public GridField[] GetFields()
        {
            return _gridTable.GetFields();
        }

        /// <summary>
        ///return fields Array
        /// </summary>
        /// <returns></returns>
        public GridTabPanel[] GetPanels()
        {
            return _panels.ToArray();
        }

        public bool HasPanels
        {
            get
            {
                return hasPanels;
            }
        }


        /// <summary>
        /// Get Table mOdel Obj
        /// </summary>
        /// <returns></returns>
        public GridTable GetTableObj()
        {
            return _gridTable;
        }

        /// <summary>
        /// Tab Level
        /// </summary>
        /// <returns></returns>
        public int GetTabLevel()
        {
            return _vo.TabLevel;
        }


        /// <summary>
        ///	Is the Tab Visible.
        ///	Called when constructing the window.
        /// </summary>
        /// <param name="initialSetup">return false only if not to be displayed</param>
        /// <returns>true, if displayed</returns>
        public bool IsDisplayed(bool initialSetup)
        {
            //  no restrictions
            String dl = _vo.DisplayLogic;
            if (dl == null || dl.Equals(""))
                return true;

            if (initialSetup)
            {
                if (dl.IndexOf("@#") != -1)		//	global variable
                {
                    String parsed = Utility.Env.ParseContext(_vo.GetCtx(), 0, dl, false, false).Trim();
                    if (parsed.Length != 0)	//	variable defined
                        return Evaluator.EvaluateLogic(this, dl);
                }
                return true;
            }
            //
            bool retValue = Evaluator.EvaluateLogic(this, dl);
            log.Config(_vo.Name + " (" + dl + ") => " + retValue);
            return retValue;
        }	//	isDisplayed

        ///// <summary>
        ///// load the records and return true if success
        ///// </summary>
        ///// <param name="onlyCurrentDays"></param>
        ///// <param name="maxRows"></param>
        ///// <param name="created"></param>
        ///// <returns></returns>
        //public bool Query(int onlyCurrentDays,
        //int maxRows, bool created)
        //{
        //    bool success = false;
        //    //  start loading while building screen
        //    success = PrepareQuery(onlyCurrentDays, maxRows, created);
        //    ////  Update UI
        //    //if (!isSingleRow())
        //    //    vTable.autoSize(true);
        //    return success;
        //}   //  query

        String _ParentID = String.Empty;

        public String ParentID
        {
            get { return _ParentID; }
            set { _ParentID = value; }
        }

        /// <summary>
        /// Assemble whereClause and query .
        ///		Scenarios:
        ///	- Never opened 					(full query)
        ///		- query changed 				(full query)
        ///		- Detail link value changed		(full query)
        ///		- otherwise 					(refreshAll)
        /// </summary>
        /// <param name="onlyCurrentDays">if only current row, how many days back</param>
        /// <param name="maxRows">maxRows maximum rows or 0 for all</param>
        /// <param name="created">created query based on created if true otherwise updated</param>
        /// <returns>true if queried successfully</returns>
        public bool PrepareQuery(int onlyCurrentDays, int maxRows, bool created, bool isVisualEdtr)
        {

            //_gridTable.IsVisualEditor = isVisualEdtr;

            bool success = true;

            //	is it same query?
            bool refresh = _oldQuery.Equals(_query.GetWhereClause())
                && _vo.onlyCurrentDays == onlyCurrentDays;

            _oldQuery = _query.GetWhereClause();
            _vo.onlyCurrentDays = onlyCurrentDays;

            /**
           *	Set Where Clause
           */
            //	Tab Where Clause
            StringBuilder where = new StringBuilder("");
            if (!isVisualEdtr) //show all tab in visual editor
            {
                where.Append(_vo.WhereClause);
            }
            //    _vo.WhereClause);
            if (_vo.onlyCurrentDays > 0)
            {
                if (where.Length > 0)
                    where.Append(" AND ");

                bool showNotProcessed = FindColumn("Processed") != -1;
                //	Show only unprocessed or the one updated within x days
                if (showNotProcessed)
                    where.Append("(Processed='N' OR ");
                if (created)
                    where.Append("Created>=");
                else
                    where.Append("Updated>=");
                //	where.append("addDays(current_timestamp, -");
                where.Append("addDays(SysDate, -")
                    .Append(_vo.onlyCurrentDays).Append(")");
                if (showNotProcessed)
                    where.Append(")");
            }

            if (IsDetail())
            {
                string lc = GetLinkColumnName();
                if (lc.Equals(""))
                {
                    //log.warning("No link column");
                    if (where.Length > 0)
                        where.Append(" AND ");
                    where.Append(" 2=3");
                    success = false;
                }
                else
                {
                    string value = _vo.GetCtx().GetContext(_vo.windowNo, lc);

                    if (String.IsNullOrEmpty(value))
                    {
                        value = ParentID;
                    }
                    //	Same link value?
                    if (refresh)
                    {
                        refresh = _linkValue.Equals(value);
                    }
                    _linkValue = value;
                    //	Check validity
                    if (value.Length == 0)
                    {
                        //log.warning("No value for link column " + lc);
                        if (where.Length > 0)
                            where.Append(" AND ");
                        where.Append(" 2=4");
                        success = false;
                    }
                    else
                    {
                        //	we have column and value
                        if (where.Length > 0)
                            where.Append(" AND ");
                        if ("NULL".Equals(value.ToUpper()))
                        {
                            where.Append(lc).Append(" IS NULL ");
                            //log.severe("Null Value of link column " + lc);
                        }
                        else
                        {
                            where.Append(lc).Append("=");
                            if (lc.EndsWith("_ID"))
                                where.Append(value);
                            else
                                where.Append("'").Append(value).Append("'");
                        }
                    }
                }
            }	//	isDetail

            _extendedWhere = where.ToString();

            //	Final Query
            if (_query.IsActive())
            {
                string q = ValidateQuery(_query);
                if (q != null)
                {
                    if (where.Length > 0)
                        where.Append(" AND ");
                    where.Append(q);
                }
            }

            /**
		 *	Query
		 */
            //log.fine("#" + m_vo.tabNum + " - " + where);
            if (_gridTable.IsOpen())
            {
                if (refresh)
                {
                    _gridTable.DataRefreshAll();
                }
                else
                    _gridTable.DataRequery(where.ToString());
            }
            else
            {
                _gridTable.SetSelectWhereClause(where.ToString());
                _gridTable.Open(maxRows);
            }

            return success;
        }

        public bool PrepareQuery(int onlyCurrentDays, int maxRows, bool created)
        {

            return PrepareQuery(onlyCurrentDays, maxRows, created, false);


            //   bool success = true;
            //   //	is it same query?
            //   bool refresh = _oldQuery.Equals(_query.GetWhereClause())
            //       && _vo.onlyCurrentDays == onlyCurrentDays;

            //   _oldQuery = _query.GetWhereClause();
            //   _vo.onlyCurrentDays = onlyCurrentDays;

            //   /**
            //  *	Set Where Clause
            //  */
            //   //	Tab Where Clause
            //   StringBuilder where = new StringBuilder(_vo.WhereClause);
            //   if (_vo.onlyCurrentDays > 0)
            //   {
            //       if (where.Length > 0)
            //           where.Append(" AND ");

            //       bool showNotProcessed = FindColumn("Processed") != -1;
            //       //	Show only unprocessed or the one updated within x days
            //       if (showNotProcessed)
            //           where.Append("(Processed='N' OR ");
            //       if (created)
            //           where.Append("Created>=");
            //       else
            //           where.Append("Updated>=");
            //       //	where.append("addDays(current_timestamp, -");
            //       where.Append("addDays(SysDate, -")
            //           .Append(_vo.onlyCurrentDays).Append(")");
            //       if (showNotProcessed)
            //           where.Append(")");
            //   }

            //   if (IsDetail())
            //   {
            //       string lc = GetLinkColumnName();
            //       if (lc.Equals(""))
            //       {
            //           //log.warning("No link column");
            //           if (where.Length > 0)
            //               where.Append(" AND ");
            //           where.Append(" 2=3");
            //           success = false;
            //       }
            //       else
            //       {
            //           string value = _vo.ctx.GetContext(_vo.windowNo, lc);
            //           //	Same link value?
            //           if (refresh)
            //           {
            //               refresh = _linkValue.Equals(value);
            //           }
            //           _linkValue = value;
            //           //	Check validity
            //           if (value.Length == 0)
            //           {
            //               //log.warning("No value for link column " + lc);
            //               if (where.Length > 0)
            //                   where.Append(" AND ");
            //               where.Append(" 2=4");
            //               success = false;
            //           }
            //           else
            //           {
            //               //	we have column and value
            //               if (where.Length > 0)
            //                   where.Append(" AND ");
            //               if ("NULL".Equals(value.ToUpper()))
            //               {
            //                   where.Append(lc).Append(" IS NULL ");
            //                   //log.severe("Null Value of link column " + lc);
            //               }
            //               else
            //               {
            //                   where.Append(lc).Append("=");
            //                   if (lc.EndsWith("_ID"))
            //                       where.Append(value);
            //                   else
            //                       where.Append("'").Append(value).Append("'");
            //               }
            //           }
            //       }
            //   }	//	isDetail

            //   _extendedWhere = where.ToString();

            //   //	Final Query
            //   if (_query.IsActive())
            //   {
            //       string q = ValidateQuery(_query);
            //       if (q != null)
            //       {
            //           if (where.Length > 0)
            //               where.Append(" AND ");
            //           where.Append(q);
            //       }
            //   }

            //   /**
            //*	Query
            //*/
            //   //log.fine("#" + m_vo.tabNum + " - " + where);
            //   if (_gridTable.IsOpen())
            //   {
            //       if (refresh)
            //       {
            //           _gridTable.DataRefreshAll();
            //       }
            //       else
            //           _gridTable.DataRequery(where.ToString());
            //   }
            //   else
            //   {
            //       _gridTable.SetSelectWhereClause(where.ToString());
            //       _gridTable.Open(maxRows);
            //   }

            //   return success;
        }


        /// <summary>
        /// Refresh current row data
        /// </summary>
        public void DataRefresh()
        {
            DataRefresh(_currentRow);
        }

        /// <summary>
        ///Refresh row data
        /// </summary>
        /// <param name="row">row index</param>
        public void DataRefresh(int row)
        {
            //log.fine("#" + m_vo.TabNo + " - row=" + row);
            _gridTable.DataRefresh(_rowData);
            SetCurrentRow();
        }


        /// <summary>
        /// Get Dataset
        /// </summary>
        /// <returns></returns>
        public DataTable GetDataTable()
        {
            return _gridTable.GetRecords();
        }

        public DataTable GetDefaultTableModel()
        {

            return _gridTable.GetTableModel();
        }

        /// <summary>
        ///Is the tab/table currently open
        /// </summary>
        /// <returns></returns>
        public bool IsOpen()
        {
            //	Open?
            if (_gridTable != null)
                return _gridTable.IsOpen();
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public bool IsWaiting()
        //{
        //    if (_gridTable != null)
        //    {
        //        while (_gridTable.GetRowCount() <= _currentRow)
        //        {
        //          while (!_gridTable.IsAlive())
        //            {
        //                _currentRow = 0;
        //                return true;
        //            }
        //        }
        //        return true;
        //    }
        //    return false;
        //}

        public void LoadComplte()
        {
            _gridTable.LoadComplete();
        }


        ///// <summary>
        ///// total number of rows
        ///// </summary>
        ///// <returns></returns>
        //public int GetRows()
        //{
        //    if (_gridTable != null)
        //        return _gridTable.GetRowCount();
        //    return 0;
        //}
        /// <summary>
        /// Is Single Row
        /// </summary>
        /// <returns></returns>
        public bool IsSingleRow()
        {
            return _vo.IsSingleRow;
        }   //

        /// <summary>
        /// Find Column
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public int FindColumn(string columnName)
        {
            return _gridTable.FindColumn(columnName);
        }

        /// <summary>
        /// Returns true if this is a detail record
        /// </summary>
        /// <returns></returns>
        public bool IsDetail()
        {
            //	We have IsParent columns and/or a link column
            if (_parents.Count > 0 || _vo.AD_Column_ID != 0)
                return true;
            return false;
        }	//	IsDetail

        /// <summary>
        /* 
	     *	Is the tab current?.
	     *  <pre>
	     *	Yes 	- Table must be open
	     *			- Query String is the same
	     *			- Not Detail
	     *			- Old link column value is same as current one
	     *  </pre>
	     *  @return true if current
	     */
        /// </summary>
        /// <returns></returns>
        public bool IsCurrent()
        {
            //	Open?
            if (!_gridTable.IsOpen())
                return false;
            //	Same Query
            if (!_oldQuery.Equals(_query.GetWhereClause()))
                return false;
            //	Detail?
            if (!IsDetail())
                return true;
            //	Same link column value
            String value = _vo.GetCtx().GetContext(_vo.windowNo, GetLinkColumnName());
            return _linkValue.Equals(value);
        }	//	isCurrent

        /// <summary>
        /// Validate Query.
        /// If query column is not a tab column create EXISTS query
        /// </summary>
        /// <param name="query">query</param>
        /// <returns>where clause</returns>
        private String ValidateQuery(Query query)
        {
            if (query == null || query.GetRestrictionCount() == 0)
                return null;

            //	Check: only one restriction
            if (query.GetRestrictionCount() != 1)
            {
                //log.fine("Ignored(More than 1 Restriction): " + query);
                return query.GetWhereClause();
            }

            String colName = query.GetColumnName(0);
            if (colName == null)
            {
                //log.fine("Ignored(No Column): " + query);
                return query.GetWhereClause();
            }
            //	a '(' in the name = function - don't try to resolve
            if (colName.IndexOf('(') != -1)
            {
                //log.fine("Ignored(Function): " + colName);
                return query.GetWhereClause();
            }
            ////	OK - Query is valid 

            //	Zooms to the same Window (Parents, ..)
            String refColName = null;
            if (colName.Equals("R_RequestRelated_ID"))
            {
                refColName = "R_Request_ID";
            }
            else if (colName.StartsWith("C_DocType"))
            {
                refColName = "C_DocType_ID";
            }
            //Comment by raghu 18-Aug-2011
            //else if (colName.Equals("CreatedBy") || colName.Equals("UpdatedBy"))
            //{
            //    refColName = "AD_User_ID";
            //}
            // Updated by raghu 18-Aug-2011
            // resolved CreatedBy and UpdatedBy Search problem.i.e.Invoice(Vendor)
            else if (colName.Equals("CreatedBy"))
            {
                //refColName = "AD_User_ID";
                refColName = "CreatedBy";
            }
            else if (colName.Equals("UpdatedBy"))
            {
                refColName = "UpdatedBy";
            }
            /**********************************/
            else if (colName.Equals("Orig_Order_ID"))
            {
                refColName = "C_Order_ID";
            }
            else if (colName.Equals("Orig_InOut_ID"))
            {
                refColName = "M_InOut_ID";
            }

            if (refColName != null)
            {
                query.SetColumnName(0, refColName);
                if (GetField(refColName) != null)
                {
                    //log.fine("Column " + colName + " replaced with synonym " + refColName);
                    return query.GetWhereClause();
                }
                refColName = null;
            }

            ////	Simple Query. 
            if (GetField(colName) != null)
            {
                //log.fine("Field Found: " + colName);
                return query.GetWhereClause();
            }

            //	Find Refernce Column e.g. BillTo_ID -> C_BPartner_Location_ID
            string sql = "SELECT cc.ColumnName "
                + "FROM AD_Column c"
                + " INNER JOIN AD_Ref_Table r ON (c.AD_Reference_Value_ID=r.AD_Reference_ID)"
                + " INNER JOIN AD_Column cc ON (r.Column_Key_ID=cc.AD_Column_ID) "
                + "WHERE c.AD_Reference_ID IN (18,30)" 	//	Table/Search
                + " AND c.ColumnName='" + colName + "'";
            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql);
                if (dr.Read())
                    refColName = dr["ColumnName"].ToString();
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                log.Log(Level.SEVERE, "(ref) - Column=" + colName, e.Message);
                return query.GetWhereClause();
            }
            //	Reference Column found
            if (refColName != null)
            {
                query.SetColumnName(0, refColName);
                if (GetField(refColName) != null)
                {
                    //log.fine("Column " + colName + " replaced with " + refColName);
                    return query.GetWhereClause();
                }
                colName = refColName;
            }

            //	Column NOT in Tab - create EXISTS subquery
            String tableName = null;
            String tabKeyColumn = GetKeyColumnName();
            ////	Column=SalesRep_ID, Key=AD_User_ID, Query=SalesRep_ID=101

            sql = "SELECT t.TableName "
                + "FROM AD_Column c"
                + " INNER JOIN AD_Table t ON (c.AD_Table_ID=t.AD_Table_ID) "
                + "WHERE c.ColumnName='" + colName + "' AND IsKey='Y'"		//	#1 Link Column
                + " AND EXISTS (SELECT * FROM AD_Column cc"
                + " WHERE cc.AD_Table_ID=t.AD_Table_ID AND cc.ColumnName='" + tabKeyColumn + "')";	//	#2 Tab Key Column
            try
            {
                dr = DataBase.DB.ExecuteReader(sql);
                if (dr.Read())
                    tableName = dr["TableName"].ToString();
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                log.Log(Level.SEVERE, "Column=" + colName + ", Key=" + tabKeyColumn, e);
                return null;
            }

            //	Special Reference Handling
            if (tabKeyColumn.Equals("AD_Reference_ID"))
            {
                //	Column=AccessLevel, Key=AD_Reference_ID, Query=AccessLevel='6'
                sql = "SELECT AD_Reference_ID FROM AD_Column WHERE ColumnName='" + colName + "'";
                //int AD_Reference_ID = DataBase.getSQLValue(null, sql, colName);
                string AD_Reference_ID = DataBase.DB.ExecuteScalar(sql).ToString();
                return "AD_Reference_ID=" + AD_Reference_ID;
            }

            ////	Causes could be functions in query
            ////	e.g. Column=UPPER(Name), Key=AD_Element_ID, Query=UPPER(AD_Element.Name) LIKE '%CUSTOMER%'
            if (tableName == null)
            {
                log.Fine("Not successfull - Column=" + colName + ", Key=" + tabKeyColumn + ", Query=" + query);
                return query.GetWhereClause();
            }

            query.SetTableName("xx");
            StringBuilder result = new StringBuilder("EXISTS (SELECT * FROM ")
                .Append(tableName).Append(" xx WHERE ")
                .Append(query.GetWhereClause(true))
                .Append(" AND xx.").Append(tabKeyColumn).Append("=")
                .Append(GetTableName()).Append(".").Append(tabKeyColumn).Append(")");

            return result.ToString();
        }

        /// <summary>
        /// Get Only Current Days
        /// </summary>
        /// <returns></returns>
        public int GetOnlyCurrentDays()
        {
            return _vo.onlyCurrentDays;
        }

        //Set Query Object
        public void SetQuery(Query query)
        {
            if (query == null)
                _query = new Query();
            else
            {
                _query = query;
                _vo.onlyCurrentDays = 0;
            }
        }


        /// <summary>
        /// Evalutee
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        public String GetValueAsString(String variableName)
        {
            return _vo.GetCtx().GetContext(_vo.windowNo, variableName, true);
        }

        /// <summary>
        /// Get Key Index
        /// </summary>
        /// <returns></returns>
        public int GetKeyIndex()
        {
            return _gridTable.GetKeyIndex();
        }

        //Set Unchanged dataa row ..used when dynamic prepare query t sve or update
        public void SetOriginalData(DataRowView drv)
        {
            _gridTable.SetOriginalData(drv);
        }

        ///// <summary>
        ///// Do we need to Save?
        ///// </summary>
        ///// <param name="rowChange"></param>
        ///// <param name="onlyRealChange"></param>
        ///// <returns></returns>
        //public bool NeedSave(bool rowChange, bool onlyRealChange)
        //{
        //    if (rowChange)
        //    {
        //        return _gridTable.NeedSave(-2, onlyRealChange);
        //    }
        //    else
        //    {
        //        //if (onlyRealChange)
        //        //    return m_mTable.needSave();
        //        //else
        //        //    return m_mTable.needSave(onlyRealChange);
        //        return false;
        //    }
        //}

        /// <summary>
        ///Uncoditionally Save data
        /// </summary>
        /// <param name="manualCmd"></param>
        /// <returns></returns>
        public bool DataSave(bool manualCmd, DataRowView drv)
        {
            //log.fine("#" + m_vo.TabNo + " - row=" + _currentRow);
            try
            {
                bool retValue = (_gridTable.DataSave(manualCmd, drv) == GridTable.SAVE_OK);
                //if (manualCmd)
                //    setCurrentRow(_currentRow, false);
                return retValue;
            }
            catch (Exception e)
            {
                //Common.//ErrorLog.FillErrorLog("Save", "", e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
                log.Log(Level.SEVERE, "#" + _vo.tabNo + " - row=" + _currentRow, e);
            }
            return false;
        }   //  dataSave

        public bool DataSave(bool manualCmd)
        {
            //log.fine("#" + m_vo.TabNo + " - row=" + _currentRow);
            try
            {
                bool retValue = (_gridTable.DataSave(manualCmd, _rowData) == GridTable.SAVE_OK);
                if (retValue)
                {
                    _rowData.Row.AcceptChanges();
                    if (manualCmd)
                    {
                        SetCurrentRow();
                    }
                }
                else
                {
                    if (!manualCmd)
                    {
                        _rowData.CancelEdit();
                        _rowData.Row.Table.RejectChanges();
                    }
                }
                return retValue;
            }
            catch (Exception e)
            {
                //Common.//ErrorLog.FillErrorLog("TabObj.DataSave()", "", e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
                log.Log(Level.SEVERE, "#" + _vo.tabNo + " - row=" + _currentRow, e);
            }
            return false;
        }   //  da

        /// <summary>
        ///Ignore data changes
        /// </summary>
        public void DataIgnore()
        {
            _rowData.Row.Table.RejectChanges();
            _gridTable.DataIgnore();
        }


        /// <summary>
        ///Check User can Insert record
        /// </summary>
        /// <param name="copy">copy</param>
        /// <returns>true if copied/new</returns>
        public bool CheckDataNew()
        {
            //log.fine("#" + m_vo.TabNo);
            if (!IsInsertRecord())
            {
                log.Warning("Inset Not allowed in TabNo=" + _vo.tabNo);
                return false;
            }
            //	Prevent New Where Main Record is processed
            if (_vo.tabNo > 0)
            {
                bool processed = "Y".Equals(_vo.GetCtx().GetContext(_vo.windowNo, "Processed"));
                //	boolean active = "Y".equals(m_vo.ctx.getContext( m_vo.WindowNo, "IsActive"));
                if (processed)
                {
                    log.Warning("Not allowed in TabNo=" + _vo.tabNo + " -> Processed=" + processed);
                    return false;
                }
                log.Finest("Processed=" + processed);
            }
            return true;
        }

        /// <summary>
        /// Add new data in new row
        /// </summary>
        /// <param name="copy"></param>
        /// <param name="drv"></param>
        /// <returns></returns>
        public bool DataNew(bool copy)
        {
            if (!CheckDataNew())
                return false;

            return DataNew(copy, null);

            // bool retValue = _gridTable.DataNew(copy, null, null);
            // if (!retValue)
            // {
            //   return retValue;
            // }


            //    setCurrentRow(_currentRow + 1, true);
            //    //  process all Callouts (no dependency check - assumed that settings are valid)
            //for (int i = 0; i < GetFieldCount(); i++)
            //     ProcessCallout(GetField(i));
            //  check validity of defaults
            //for (int i = 0; i < GetFieldCount(); i++)
            //{
            //    ////GetField(i) . refreshLookup();
            //    //GetField(i) .validateValue();
            //    //getField(i).setError(false);
            //}
            ////    m_mTable.setChanged(false);
            //return retValue;
            //}   //  dataNew
        }

        /// <summary>
        /// Add new data in new row
        /// </summary>
        /// <param name="copy"></param>
        /// <param name="drv"></param>
        /// <returns></returns>
        public bool DataNew(bool copy, DataRowView oldRow)
        {
            bool retValue = _gridTable.DataNew(copy, _rowData, oldRow);
            if (!retValue)
            {
                return retValue;
            }

            //    setCurrentRow(_currentRow + 1, true);
            //  process all Callouts (no dependency check - assumed that settings are valid)
            _isCalloutExecute = true;
            for (int i = 0; i < GetFieldCount(); i++)
            {
                ProcessCallout(GetField(i));
            }
            //string ss = "ddd";
            _rowData.EndEdit();
            _isCalloutExecute = false;
            //  check validity of defaults
            for (int i = 0; i < GetFieldCount(); i++)
            {
                //    ////GetField(i) . refreshLookup();
                //    //GetField(i) .validateValue();
                GetField(i).SetError(false);
            }
            ////    m_mTable.setChanged(false);
            return retValue;
            //}   //  dataNew
        }




        /// <summary>
        /// Peform callout  
        /// </summary>
        public void PerformCallout()
        {
            for (int i = 0; i < GetFieldCount(); i++)
                ProcessCallout(GetField(i));
        }

        /// <summary>
        /// Can we Insert Records?
        /// </summary>
        /// <returns>true not read only and allowed</returns>
        public bool IsInsertRecord()
        {
            if (IsReadOnly())
                return false;
            return _vo.IsInsertRecord;
        }	//	

        /// <summary>
        ///Get Included Tab ID
        /// </summary>
        /// <returns></returns>
        public int GetIncluded_Tab_ID()
        {
            return _vo.Included_Tab_ID;
        }	//


        ///// <summary>
        ///// Delete current Row
        ///// </summary>
        ///// <returns>true if deleted</returns>
        //public bool DataDelete(int _currentRow)
        //{
        //    //log.fine("#" + m_vo.TabNo + " - row=" + _currentRow);
        //    bool retValue = _gridTable.DataDelete(_currentRow);
        //    //setCurrentRow(_currentRow, true);
        //    return retValue;
        //}   //  dataDelete

        /// <summary>
        /// Delete current Row
        /// </summary>
        /// <returns>true if deleted</returns>
        public bool DataDelete(int _currentRow)
        {
            //log.fine("#" + m_vo.TabNo + " - row=" + _currentRow);
            bool retValue = _gridTable.DataDelete(_rowData);
            //setCurrentRow(_currentRow, true);
            return retValue;
        }   //
        /// <summary>
        /// Has this field dependents ?
        /// </summary>
        /// <param name="columnName">column name</param>
        /// <returns>true if column has dependent</returns>
        public bool HasDependants(String columnName)
        {
            return _depOnField.ContainsKey(columnName);
        }   //  IsDependentOn

        /// <summary>
        ///Get dependents fields of columnName
        /// </summary>
        /// <param name="columnName">key</param>
        /// <returns>List with GridFields dependent on columnName</returns>
        public List<GridField> GetDependantFields(String columnName)
        {
            return _depOnField.GetValues(columnName);
        }   //  GetDependantFields

        /// <summary>
        /// Load Dependant Information
        /// </summary>
        public void LoadDependentInfo()
        {
            /**
             * Load Order Type from C_DocTypeTarget_ID
             */
            if (_vo.TableName.Equals("C_Order"))
            {
                int C_DocTyp_ID = 0;
                //int target = (Integer)GetValue("C_DocTypeTarget_ID");
                try
                {

                    int.TryParse(GetValue("C_DocTypeTarget_ID").ToString(), out C_DocTyp_ID);
                }
                catch
                { }
                if (C_DocTyp_ID == 0)
                    return;

                String sql = "SELECT DocSubTypeSO FROM C_DocType WHERE C_DocType_ID=" + C_DocTyp_ID;
                try
                {
                    string orderType = DataBase.DB.ExecuteScalar(sql).ToString();
                    _vo.GetCtx().SetContext(_vo.windowNo, "OrderType", orderType);
                }
                catch (Exception e)
                {
                    log.Log(Level.SEVERE, "LoadDependentInfo", e);
                }
            }   //  loadOrderInfo
        }   //  loadDependentInfo

        /// <summary>
        ///Is Read Only?
        /// </summary>
        /// <returns>true if read only</returns>
        public bool IsReadOnly()
        {
            if (_vo.IsReadOnly)
                return true;

            //  no restrictions
            if (_vo.ReadOnlyLogic == null || _vo.ReadOnlyLogic.Equals(""))
                return _vo.IsReadOnly;

            //  ** dynamic content **  uses get_ValueAsString Not Implement yet
            bool retValue = Evaluator.EvaluateLogic(this, _vo.ReadOnlyLogic);
            log.Finest(_vo.Name
                + " (" + _vo.ReadOnlyLogic + ") => " + retValue);
            return retValue;
        }

        /// <summary>
        ///	Tab contains Always Update Field
        /// </summary>
        /// <returns></returns>
        public bool IsAlwaysUpdateField()
        {
            int size = _gridTable.GetColumnCount();
            for (int i = 0; i < size; i++)
            {
                GridField field = _gridTable.GetField(i);
                if (field.IsAlwaysUpdateable())
                    return true;
            }
            return false;
        }	//


        /// <summary>
        /// Is Query Active
        /// </summary>
        /// <returns>true if query active</returns>
        public bool IsQueryActive()
        {
            if (_query != null)
                return _query.IsActive();
            return false;
        }

        /// <summary>
        ///    Is Query New Record
        /// </summary>
        /// <returns></returns>
        public bool IsQueryNewRecord()
        {
            if (_query != null)
                return _query.IsNewRecordQuery();
            return false;
        }	//	isQueryNewRecord



        /// <summary>
        /// Get Tab ID
        /// </summary>
        /// <returns>Tab ID</returns>
        public int GetAD_Tab_ID()
        {
            return _vo.AD_Tab_ID;
        }

        /// <summary>
        /// Get TabNo
        /// </summary>
        /// <returns>tab no</returns>
        public int GetTabNo()
        {
            return _vo.tabNo;
        }

        /// <summary>
        /// Get ProcessID
        /// </summary>
        /// <returns>ProcessID</returns>
        public int GetAD_Process_ID()
        {
            return _vo.AD_Process_ID;
        }

        /// <summary>
        /// Get Current Table Key ID
        /// </summary>
        /// <returns>Record_ID</returns>
        /// <author>Veena Pandey</author>
        public int GetRecord_ID()
        {
            int keyId = -1;
            if (_rowData != null)
            {
                try
                {
                    keyId = Convert.ToInt32(_rowData[_gridTable.GetKeyIndex()]);
                    //_rowData[_gridTable.GetKeyIndex()];// _gridTable.GetKeyID(_currentRow);
                }
                catch
                {
                    // Common.//ErrorLog.FillErrorLog("TabObj.GetRecordId()", "Error to get record id", e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
                }
            }
            return keyId;
        }

        /// <summary>
        /// Get Attachment_ID for current record.
        /// </summary>
        /// <returns>ID or 0, if not found</returns>
        /// <author>Veena Pandey</author>
        public int GetAD_AttachmentID()
        {
            //if (_attachments == null)
            //    LoadAttachments();
            //if (_attachments == null)
            //    return 0;
            ////
            //int key = GetRecord_ID();// _gridTable.GetKeyID(CurrentRow);
            ////int value = Utility.Util.GetValueOfInt(_attachments[key].ToString());
            //if (_attachments.ContainsKey(key))
            //{
            //    int value = Utility.Util.GetValueOfInt(_attachments[key].ToString());
            //    return value;
            //}
            return 0;
        }

        /// <summary>
        /// Returns true, if current row has an Attachment
        /// </summary>
        /// <returns>bool type true if record has attachment</returns>
        /// <author>Veena Pandey</author>
        public bool HasAttachment()
        {
            //if (_attachments == null)
            //    LoadAttachments();
            //if (_attachments == null)
            //    return false;
            ////
            //int key = GetRecord_ID();// _gridTable.GetKeyID(CurrentRow);
            //return _attachments[0].ContainsKey(key);
            return false;
        }

        /// <summary>
        /// Can this tab have Attachments?.
        /// </summary>
        /// <returns>bool type true if record can have attachment</returns>
        /// <author>Veena Pandey</author>
        public bool CanHaveAttachment()
        {
            if (GetKeyColumnName().EndsWith("_ID"))
                return true;
            return false;
        }

        /// <summary>
        /// Fetch Information like Chat_ID , Attachment_ID, PersonalAccess_ID, Export_ID etc for each record of current Tab.
        /// Will be fetched while opening window only.
        /// </summary>
        /// Karan.... 18 Dec 2018
        /// <param name="AD_User_ID"></param>
        public void LoadData(int AD_User_ID)
        {

            if (!CanHaveAttachment())
            {
                return;
            }

            string sql = "";
            DataSet ds = null;


            if (_wVo.IsMarkToExport)
            {
                sql = " SELECT AD_EXPORTDATA_ID,RECORD_ID, AD_ColOne_ID FROM AD_ExportData WHERE AD_Table_ID=" + _vo.AD_Table_ID;
                ds = DB.ExecuteDataset(sql);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    _exports = new List<AttachmentList>();
                    int key, value;
                    for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        key = Utility.Util.GetValueOfInt(ds.Tables[0].Rows[i]["Record_ID"]);
                        value = Utility.Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_ExportData_ID"]);
                        _exports.Add(new AttachmentList() { ID = key, value = value });
                    }
                }
            }
            if (_wVo.IsAttachment)
            {
                sql = " SELECT distinct att.AD_Attachment_ID, att.Record_ID FROM AD_Attachment att  INNER JOIN AD_AttachmentLine al ON (al.AD_Attachment_id=att.AD_Attachment_id)  WHERE att.AD_Table_ID=" + _vo.AD_Table_ID;
                ds = DB.ExecuteDataset(sql);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    _attachments = new List<AttachmentList>();
                    int key, value;
                    for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        key = Utility.Util.GetValueOfInt(ds.Tables[0].Rows[i]["Record_ID"]);
                        value = Utility.Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Attachment_ID"]);
                        _attachments.Add(new AttachmentList() { ID = key, value = value });
                    }

                }
            }
            if (_wVo.IsChat)
            {
                sql = (" SELECT CM_Chat_ID, Record_ID FROM CM_Chat WHERE AD_Table_ID=" + _vo.AD_Table_ID);
                ds = DB.ExecuteDataset(sql);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    _chats = new List<AttachmentList>();
                    int key, value;
                    for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        key = Utility.Util.GetValueOfInt(ds.Tables[0].Rows[i]["Record_ID"]);
                        value = Utility.Util.GetValueOfInt(ds.Tables[0].Rows[i]["CM_Chat_ID"]);
                        _chats.Add(new AttachmentList() { ID = key, value = value });
                    }

                }
            }
            if (_wVo.IsPrivateRecordLock)
            {
                sql = " SELECT Record_ID FROM AD_Private_Access WHERE AD_User_ID=" + AD_User_ID + " AND AD_Table_ID=" + _vo.AD_Table_ID + " AND IsActive='Y' ORDER BY Record_ID";
                ds = DB.ExecuteDataset(sql);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    _pAccess = new List<int>();
                    int key;
                    for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        key = Utility.Util.GetValueOfInt(ds.Tables[0].Rows[i]["Record_ID"]);
                        _pAccess.Add(key);
                    }
                }
            }
            if (_wVo.IsSubscribedRecord)
            {
                sql = " Select cm_Subscribe_ID,Record_ID from CM_Subscribe where AD_User_ID=" + AD_User_ID + " AND ad_Table_ID=" + +_vo.AD_Table_ID;
                ds = DB.ExecuteDataset(sql);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    _subscribe = new List<AttachmentList>();
                    int key, value;
                    for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        key = Utility.Util.GetValueOfInt(ds.Tables[0].Rows[i]["Record_ID"]);
                        value = Utility.Util.GetValueOfInt(ds.Tables[0].Rows[i]["CM_Subscribe_ID"]);
                        _subscribe.Add(new AttachmentList() { ID = key, value = value });
                    }

                }
            }
            if (_wVo.IsViewDocument)
            {
                sql = " SELECT vadms_windowdoclink_id, record_id FROM VADMS_WindowDocLink wdl JOIN vadms_document doc ON wdl.VADMS_Document_ID  =doc.VADMS_Document_ID WHERE doc.vadms_docstatus!='DD' AND AD_Table_ID =" + _vo.AD_Table_ID;
                ds = DB.ExecuteDataset(sql);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    _documents = new List<AttachmentList>();
                    int key, value;
                    for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        key = Utility.Util.GetValueOfInt(ds.Tables[0].Rows[i]["Record_ID"]);
                        value = Utility.Util.GetValueOfInt(ds.Tables[0].Rows[i]["VADMS_WindowDocLink_ID"]);
                        _documents.Add(new AttachmentList() { ID = key, value = value });
                    }

                }
            }
        }


        /// <summary>
        /// Load Attachments for this table
        /// </summary>
        /// <author>Veena Pandey</author>
        public void LoadAttachments()
        {
            //log.fine("#" + m_vo.TabNo);
            //if (!CanHaveAttachment())
            //    return;

            //string sqlQry = "SELECT AD_Attachment_ID, Record_ID FROM AD_Attachment "
            //    + "WHERE AD_Table_ID=" + _vo.AD_Table_ID;
            //IDataReader dr = null;
            //try
            //{
            //    if (_attachments == null)
            //        _attachments = new List<Dictionary<int, int>>();
            //    else
            //        _attachments.Clear();

            //    dr = DataBase.DB.ExecuteReader(sqlQry);
            //    int key, value;
            //    while (dr.Read())
            //    {
            //        key = Utility.Util.GetValueOfInt(dr["Record_ID"].ToString());
            //        value = Utility.Util.GetValueOfInt(dr["AD_Attachment_ID"].ToString());
            //        _attachments[0].Add(key, value);
            //    }
            //    dr.Close();
            //    dr = null;

            //}
            //catch (Exception ex)
            //{
            //    if (dr != null)
            //    {
            //        dr.Close();
            //        dr = null;
            //    }
            //    log.Log(Level.SEVERE, "loadAttachments", ex);
            //}
            //log.Config("#" + _attachments.Count);
        }

        /****************************************************************/
        #region Load Chat
        /// <summary>
        /// Load Chats for this table
        /// </summary>
        /// <author>Raghunandan Sharma</author>
        public void LoadChats()
        {
            //if doesn't have attachment
            if (!CanHaveAttachment())
                return;//return nothing
            //set query
            string sql = "SELECT CM_Chat_ID, Record_ID FROM CM_Chat "
                + "WHERE AD_Table_ID=" + _vo.AD_Table_ID;
            IDataReader dr = null;
            try
            {
                if (_chats == null)
                    //create new list for chat
                    _chats = new List<AttachmentList>();
                else
                    //if contain chat then clear list
                    _chats.Clear();
                //execute query
                dr = DataBase.DB.ExecuteReader(sql);



                int key, value;//for recordId and chatId
                while (dr.Read())
                {
                    key = Utility.Util.GetValueOfInt(dr["Record_ID"].ToString());
                    value = Utility.Util.GetValueOfInt(dr["CM_Chat_ID"].ToString());
                    _chats.Add(new AttachmentList() { ID = key, value = value });
                }

                dr.Close();
                dr = null;
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                // MessageBox.Show("No Chat");
                log.Log(Level.SEVERE, sql, e);
            }
        }
        #endregion

        #region Has Chat
        /// <summary>
        /// Returns true, if current row has a Chat
        /// </summary>
        /// <author>Raghunandan Sharma</author>
        /// <returns>Bool Type (return true if record has chat)</returns>
        public Boolean HasChat()
        {
            //if (_chats == null)
            //    LoadChats();//call load chat function
            //if (_chats == null || _chats == null)
            //    return false;
            ////get chat id
            //int key = GetRecord_ID();//ridTable.GetKeyID(CurrentRow);
            //return _chats[0].ContainsKey(key);//return chatId
            return false;
        }
        #endregion

        #region Get Chat ID
        /// <summary>
        ///Get Chat_ID for this record.
        /// </summary>
        /// <returns>return ID or 0, if not found</returns>
        /// <author>Raghunandan Sharma</author>
        public int GetCM_ChatID()
        {
            //if (_chats == null)
            //    LoadChats();//call chat function
            //if (_chats == null)
            //    return 0;
            ////get chatId
            //int key = GetRecord_ID();// _gridTable.GetKeyID(CurrentRow);
            ////The given key was not present in the dictionary. Error
            ////int value = Utility.Util.GetValueOfInt(_chats[key].ToString());
            //if (_chats[0].ContainsKey(key))
            //{
            //    //get chat key value
            //    int value = Utility.Util.GetValueOfInt(_chats[key].ToString());
            //    return value;
            //}
            return 0;
            //if (value.Equals(null))
            //    return 0;
            //else
            //    return value;
        }
        #endregion

        /****************************************************************/
        public void LoadExport()
        {
            //if doesn't have attachment
            if (!CanHaveAttachment())
                return;//return nothing
            //set query
            string sql = "SELECT AD_EXPORTDATA_ID,RECORD_ID FROM AD_EXPORTDATA"
                + " WHERE AD_Table_ID=" + _vo.AD_Table_ID;

            IDataReader dr = null;
            try
            {
                if (_exports == null)
                    //create new list for chat
                    _exports = new List<AttachmentList>();
                else
                    //if contain chat then clear list
                    _exports.Clear();
                //execute query
                dr = DataBase.DB.ExecuteReader(sql);
                int key, value;//for recordId and chatId

                while (dr.Read())
                {
                    key = Utility.Util.GetValueOfInt(dr["Record_ID"].ToString());
                    value = Utility.Util.GetValueOfInt(dr["AD_EXPORTDATA_ID"].ToString());
                    _exports.Add(new AttachmentList() { ID = key, value = value });
                }

                dr.Close();
                dr = null;
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                log.Log(Level.SEVERE, sql, e);
            }
        }
        public Boolean HasExportRecord()
        {
            //if (_exports == null)
            //    LoadExport();
            //if (_exports == null || _exports == null)
            //    return false;
            ////get chat id
            //int key = GetRecord_ID();//ridTable.GetKeyID(CurrentRow);
            //return _exports[0].ContainsKey(key);//return chatId
            return false;
        }
        public int ExportID()
        {
            //if (_exports == null)
            //    LoadExport();
            //if (_exports == null)
            //    return 0;
            ////get chatId
            //int key = GetRecord_ID();//
            ////The given key was not present in the dictionary. Error

            //if (_exports[0].ContainsKey(key))
            //{
            //    //get chat key value
            //    int value = Utility.Util.GetValueOfInt(_exports[key].ToString());
            //    return value;
            //}
            return 0;
        }



        /******************************************************/

        /// <summary>
        /// Get Value of Field with columnName
        /// </summary>
        /// <param name="columnName">column name</param>
        /// <returns>value</returns>
        public Object GetValue(String columnName)
        {
            if (columnName == null)
                return null;
            //int colInd = _gridTable.FindColumn(columnName);
            //return _gridTable.GetValueAt(CurrentRow, colInd);
            GridField field = _gridTable.GetField(columnName);
            return GetValue(field);
        }

        /// <summary>
        /// Get Value of Field
        /// </summary>
        /// <param name="field">field</param>
        /// <returns>value</returns>
        public Object GetValue(GridField field)
        {
            if (field == null)
                return null;
            //return GetValue(field.GetColumnName());           
            Object o = field.GetValue();
            if (o == DBNull.Value || o == null || o.ToString() == "")
                return null;
            return o;
        }

        /// <summary>
        /// Set New Value & call Callout
        /// </summary>
        /// <param name="columnName">database column name</param>
        /// <param name="value">value</param>
        /// <returns>error message or ""</returns>
        public String SetValue(String columnName, Object value)
        {
            if (columnName == null)
                return "NoColumn";
            if (value == null || value.ToString() == "")
            {
                value = DBNull.Value;
            }
            return SetValue(_gridTable.GetField(columnName), value);
        }

        /// <summary>
        /// Set New Value & call Callout
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <returns>error message or ""</returns>
        public String SetValue(GridField field, Object value)
        {
            if (field == null)
                return "NoField";

            log.Fine(field.GetColumnName() + "=" + value + " - Row=" + CurrentRow);
            try
            {
                if (value == null)
                {
                    //value != null ? value : DBNull.Value;
                    value = DBNull.Value;
                }
                _rowData[field.GetColumnName()] = value;
                //_rowData.Row[field.GetColumnName()] = value;
                field.SetValue(value, _gridTable.IsInserting());
                if (!_isCalloutExecute)
                {
                    _rowData.EndEdit();
                }
                IsRowDirty = true;

                //GridTable().NotifyCurrentCellDirty(true);
                //_gridTable.GetTableModal().NotifyCurrentCellDirty(true);
                // if (!CalloutEngine.IsCalloutActive())
                //{
                return ProcessFieldChange(field);
                //}
            }
            catch (Exception ex)
            {
                log.Fine(ex.Message);
            }

            //int col = _gridTable.FindColumn(field.GetColumnName());

            //_gridTable.SetValueAt(value, CurrentRow, col, false);

            //string coln = field.GetColumnName();
            //for (int i = 0; i < controlList.Count; i++)
            //{
            //    Control oo = controlList[i];
            //    if (oo.Name == coln)
            //    {
            //        Controls.IControl ic = (Controls.IControl)oo;
            //        ic.SetValue(value);
            //        break;
            //    }
            //}
            //
            return "";

        }

        /// <summary>
        /// Process Field Change - evaluate Dependencies and process Callouts.
        /// called from MTab.setValue or GridController.dataStatusChanged
        /// </summary>
        /// <param name="changedField">changed field</param>
        /// <returns>error message or ""</returns>
        public String ProcessFieldChange(GridField changedField)
        {
            ProcessDependencies(changedField);
            return ProcessCallout(changedField);
        }

        /// <summary>
        /// Evaluate Dependencies
        /// </summary>
        /// <param name="changedField">changed field</param>
        private void ProcessDependencies(GridField changedField)
        {
            String columnName = changedField.GetColumnName();
            //	log.trace(log.l4_Data, "Changed Column", columnName);

            //  when column name is not in list of DependentOn fields - fini
            if (!HasDependants(columnName))
                return;

            //  Get dependent MFields (may be because of display or dynamic lookup)
            List<GridField> list = GetDependantFields(columnName);
            for (int i = 0; i < list.Count; i++)
            {
                GridField dependentField = (GridField)list[i];
                //	log.trace(log.l5_DData, "Dependent Field", dependentField==null ? "null" : dependentField.getColumnName());
                //  if the field has a lookup

                if (dependentField.GetLookup() != null && dependentField != null && dependentField.GetLookup().GetType() == typeof(MLookup))
                {
                    MLookup mLookup = (MLookup)dependentField.GetLookup();
                    //	log.trace(log.l6_Database, "Lookup Validation", mLookup.getValidation());
                    //  if the lookup is dynamic (i.e. contains this columnName as variable)
                    if (mLookup.GetValidation().IndexOf("@" + columnName + "@") != -1)
                    {
                        log.Fine(columnName + " changed - " + dependentField.GetColumnName() + " set to null");
                        //  invalidate current selection
                        SetValue(dependentField, null);
                    }
                }
                if (dependentField != null && dependentField.GetLookup() is MLocatorLookup)
                {
                    // gwu: invalidate currently selected locator if any dependent fields changed
                    MLocatorLookup locLookup = (MLocatorLookup)dependentField.GetLookup();
                    int valueAsInt = 0;
                    if (changedField.GetValue() != null && changedField.GetValue() is int)// instanceof Number )
                    {
                        valueAsInt = (int)changedField.GetValue();
                    }
                    if (columnName.Equals("M_Warehouse_ID"))
                    {
                        locLookup.SetOnly_Warehouse_ID(valueAsInt);
                    }
                    if (columnName.Equals("M_Product_ID"))
                    {
                        locLookup.SetOnly_Product_ID(valueAsInt);
                    }
                    locLookup.SetOnly_Outgoing(Env.GetContext().IsSOTrx(_vo.windowNo));
                    locLookup.Refresh();
                    if (!locLookup.IsValid(dependentField.GetValue()))
                    {
                        SetValue(dependentField, null);
                    }
                }
            }
        }

        /// <summary>
        /// Is Processed
        /// </summary>
        /// <returns>true if current record is processed</returns>
        public bool IsProcessed()
        {
            int index = _gridTable.FindColumn("Processed");
            if (index != -1)
            {
                Object oo = _rowData[index];// _gridTable.GetValueAt(CurrentRow, index);
                if (oo.GetType() == typeof(String))
                    return "Y".Equals(oo) || "True".Equals(oo);
                if (oo.GetType() == typeof(Boolean))
                    return ((Boolean)oo);
            }
            return "Y".Equals(_vo.GetCtx().GetContext(_vo.windowNo, "Processed"));
        }

        /// <summary>
        /// Process Callout(s).
        /// The Callout is in the string of "class.method;class.method;"
        /// If there is no class name, i.e. only a method name, the class is regarded as CalloutSystem.
        /// The class needs to comply with the Interface Callout.
        /// </summary>
        /// <param name="field">field</param>
        /// <returns>error message or ""</returns>
        private String ProcessCallout(GridField field)
        {
            String callout = field.GetCallout();

            if (callout == null || callout.Length == 0)
            {
                return "";
            }

            if (IsProcessed())		//	only active records
                return "";			//	"DocProcessed";


            Object value = field.GetValue();
            Object oldValue = field.GetOldValue();






            log.Fine(field.GetColumnName() + "=" + value
            + " (" + callout + ") - old=" + oldValue);
            try
            {
                if (value.ToString() == oldValue.ToString())
                {
                    return "";
                }
                if (oldValue != null && oldValue.ToString() == "FieldValueInserting")
                {
                    if (value == null || value == DBNull.Value)
                    {
                        ///return "";
                    }
                }
                //_rowData.BeginEdit();
                _rowData[field.GetColumnName()] = value;
            }
            catch
            {
            }

            StringTokenizer st = new StringTokenizer(callout, ";,", false);
            while (st.HasMoreTokens())      //  for each callout
            {
                String cmd = st.NextToken().Trim();
                Callout call = null;
                String method = null;
                int methodStart = cmd.LastIndexOf(".");
                try
                {
                    if (methodStart != -1)      //  no class
                    {

                        Type type = Utility.ModuleTypeConatiner.GetClassType(cmd.Substring(0, methodStart));


                        //            {
                        //                log.Info(field.GetColumnName() + "=" + value
                        //+ " (" + callout + ") - old=" + oldValue + " -- From Customization Callout");
                        //            }
                        //            //if (type.IsClass)
                        //            {
                        //                call = (Callout)Activator.CreateInstance(type);
                        //            }
                        method = cmd.Substring(methodStart + 1);
                    }
                }
                catch (Exception e)
                {
                    log.Log(Level.SEVERE, "class=" + cmd, e);
                    return "Callout Invalid: " + cmd + " (" + e.ToString() + ")";
                }

                if (call == null || method == null || method.Length == 0)
                    return "Callout Invalid: " + method;

                String retValue = "";
                try
                {
                    retValue = call.Start(_vo.GetCtx(), method, _vo.windowNo, this, field, value, oldValue);
                }
                catch (Exception e)
                {
                    log.Log(Level.SEVERE, "start", e);
                    retValue = "Callout Invalid: " + e.ToString();
                    return retValue;
                }
                if (!retValue.Equals(""))		//	interrupt on first error
                {
                    log.Warning(retValue);
                    return retValue;
                }
            }
            return "";
        }

        /// <summary>
        /// check and process callout code
        /// </summary>
        /// <param name="mField"></param>
        /// <returns></returns>
        public bool ProcessDependentAndCallOut(GridField mField)
        {
            //  Process Callout
            _isCalloutExecute = true;
            bool ret = false;
            if (mField != null && mField.GetCallout().Length > 0)
            {

                String msg = ProcessFieldChange(mField);     //  Dependencies & Callout
                if (msg.Length > 0)
                {
                    //VAdvantage.Classes.ShowMessage.Error(msg, true);
                    ret = false;
                }
                else
                {
                    ret = true;
                }
            }
            _isCalloutExecute = false;
            //if (_rowData.IsEdit)
            //{
            //    _rowData.EndEdit();
            //}
            return ret;
        }

        /// <summary>
        ///Dispose - clean up resources
        /// </summary>
        public void Dispose()
        {
            //log.fine("#" + m_vo.TabNo);
            _OrderBys = null;
            //
            _parents.Clear();
            _parents = null;
            //
            _gridTable.Close(true);  //  also disposes Fields
            _gridTable = null;
            //
            _depOnField.Clear();
            _depOnField = null;
            if (_attachments != null)
                _attachments.Clear();
            _attachments = null;
            if (_chats != null)
                _chats.Clear();
            _chats = null;
            //
            //_vo.lstFields.Clear();
            //_vo.lstFields = null;

            _rowData = null;

            _vo = null;
        }


        /// <summary>
        ///Get Window ID
        /// </summary>
        /// <returns></returns>
        public int GetAD_Window_ID()
        {
            return _vo.AD_Window_ID;
        }	//	getAD_Window_ID



        /// <summary>
        /// Get WindowNo
        /// </summary>
        /// <returns>window no</returns>
        public int GetWindowNo()
        {
            return _vo.windowNo;
        }
        /****************************************************************/

        /**************************************************************************
	 * 	Load Locks for Table and User
	 */
        public void LoadLocks()
        {
            int AD_User_ID = Env.GetContext().GetAD_User_ID();
            log.Fine("#" + _vo.tabNo + " - AD_User_ID=" + AD_User_ID);
            if (!CanHaveAttachment())
                return;

            String sql = "SELECT Record_ID "
                + "FROM AD_Private_Access "
                + "WHERE AD_User_ID=" + AD_User_ID.ToString() + " AND AD_Table_ID=" + _vo.AD_Table_ID.ToString() + " AND IsActive='Y' "
                + "ORDER BY Record_ID";
            IDataReader dr = null;
            try
            {
                if (_Lock == null)
                    _Lock = new List<int>();
                else
                    _Lock.Clear();
                dr = DataBase.DB.ExecuteReader(sql, null);
                while (dr.Read())
                {
                    int key = Utility.Util.GetValueOfInt(dr[1]);
                    _Lock.Add(key);
                }
                dr.Close();
                dr = null;
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                log.Log(Level.SEVERE, sql, e);
            }
            log.Fine("#" + _Lock.Count);
        }

        /// <summary>
        ///Record Is Locked
        /// </summary>
        /// <returns></returns>
        public bool IsLocked()
        {
            if (!MRole.GetDefault((Context)_vo.GetCtx(), false).IsPersonalLock())
                return false;
            if (_Lock == null)
                LoadLocks();
            if (_Lock == null || (_Lock.Count == 0))
                return false;
            //
            int key = GetRecord_ID();// m_mTable.getKeyID (_currentRow));
            return _Lock.Contains(key);
        }

        /// <summary>
        /// Lock Record
        /// </summary>
        /// <param name="ctx">ctx context</param>
        /// <param name="Record_ID"></param>
        /// <param name="locks">true if lock, otherwise unlock</param>
        public void Locks(Ctx ctx, int Record_ID, bool locks)
        {
            int AD_User_ID = ctx.GetAD_User_ID();
            log.Finer("Lock=" + locks + ", AD_User_ID=" + AD_User_ID
              + ", AD_Table_ID=" + _vo.AD_Table_ID + ", Record_ID=" + Record_ID);
            MPrivateAccess access = MPrivateAccess.Get(ctx, AD_User_ID, _vo.AD_Table_ID, Record_ID);
            if (access == null)
            {
                access = new MPrivateAccess(ctx, AD_User_ID, _vo.AD_Table_ID, Record_ID);
            }
            access.SetIsActive(locks);
            access.Save();
            LoadLocks();
        }


        /// <summary>
        /// toString
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            String retValue = "MTab #" + _vo.tabNo;
            if (_vo != null)
            {
                retValue += " " + _vo.Name + " (" + _vo.AD_Tab_ID
                    + ") QueryActive=" + (_query != null && _query.IsActive())
                    + ", CurrentDays=" + _vo.onlyCurrentDays;
            }
            return retValue;
        }


        /// <summary>
        /// Assemble whereClause and query MTable and position to row 0.
        /// <pre>
        /// Scenarios:
        /// 	- Never opened 					(full query)
        /// 	- query changed 				(full query)
        /// 	- Detail link value changed		(full query)
        /// 	- otherwise 					(refreshAll)
        /// </pre>
        /// </summary>
        /// <param name="onlyCurrentDays">only current rows</param>
        public void Query(int onlyCurrentDays)
        {
            Query(onlyCurrentDays, 0, false);	//	updated
        }

        /// <summary>
        /// Assemble whereClause and query MTable and position to row 0.
        /// <pre>
        /// Scenarios:
        /// 	- Never opened 					(full query)
        /// 	- query changed 				(full query)
        /// 	- Detail link value changed		(full query)
        /// 	- otherwise 					(refreshAll)
        ///</pre>
        /// </summary>
        /// <param name="onlyCurrentDays">if only current row, how many days back</param>
        /// <param name="maxRows">maximum rows or 0 for all</param>
        /// <param name="created">query based on created if true otherwise updated</param>
        /// <returns>true if queried successfully</returns>
        public bool Query(int onlyCurrentDays, int maxRows, bool created)
        {
            log.Fine("#" + _vo.tabNo
                + " - OnlyCurrentDays=" + onlyCurrentDays + ", Detail=" + IsDetail());
            bool success = true;
            //	is it same query?
            bool refresh = _oldQuery.Equals(_query.GetWhereClause())
                && _vo.onlyCurrentDays == onlyCurrentDays;
            _oldQuery = _query.GetWhereClause();
            _vo.onlyCurrentDays = onlyCurrentDays;

            /**
             *	Set Where Clause
             */
            //	Tab Where Clause
            StringBuilder where = new StringBuilder(_vo.WhereClause);
            if (_vo.onlyCurrentDays > 0)
            {
                if (where.Length > 0)
                {
                    where.Append(" AND ");
                }

                bool showNotProcessed = FindColumn("Processed") != -1;
                //	Show only unprocessed or the one updated within x days
                if (showNotProcessed)
                {
                    where.Append("(Processed='N' OR ");
                }
                if (created)
                {
                    where.Append("Created>=");
                }
                else
                {
                    where.Append("Updated>=");
                }
                //	where.append("addDays(current_timestamp, -");
                where.Append("addDays(SysDate, -")
                    .Append(_vo.onlyCurrentDays).Append(")");
                if (showNotProcessed)
                {
                    where.Append(")");
                }
            }
            //	Detail Query
            if (IsDetail())
            {
                String lc = GetLinkColumnName();
                if (lc.Equals(""))
                {
                    log.Warning("No link column");
                    if (where.Length > 0)
                    {
                        where.Append(" AND ");
                    }
                    where.Append(" 2=3");
                    success = false;
                }
                else
                {
                    String value = _vo.GetCtx().GetContext(_vo.windowNo, lc);
                    //	Same link value?
                    if (refresh)
                    {
                        refresh = _linkValue.Equals(value);
                    }
                    _linkValue = value;
                    //	Check validity
                    if (value.Length == 0)
                    {
                        log.Warning("No value for link column " + lc);
                        if (where.Length > 0)
                        {
                            where.Append(" AND ");
                        }
                        where.Append(" 2=4");
                        success = false;
                    }
                    else
                    {
                        //	we have column and value
                        if (where.Length > 0)
                        {
                            where.Append(" AND ");
                        }
                        if ("NULL".Equals(value.ToUpper()))
                        {
                            where.Append(lc).Append(" IS NULL ");
                            log.Severe("Null Value of link column " + lc);
                        }
                        else
                        {
                            where.Append(lc).Append("=");
                            if (lc.EndsWith("_ID"))
                            {
                                where.Append(value);
                            }
                            else
                            {
                                where.Append("'").Append(value).Append("'");
                            }
                        }
                    }
                }
            }	//	isDetail

            _extendedWhere = where.ToString();
            //	Final Query
            if (_query.IsActive())
            {
                String q = ValidateQuery(_query);
                if (q != null)
                {
                    if (where.Length > 0)
                    {
                        where.Append(" AND ");
                    }
                    where.Append(q);
                }
            }

            /**
             *	Query
             */
            log.Fine("#" + _vo.tabNo + " - " + where);
            if (_gridTable.IsOpen())
            {
                if (refresh)
                {
                    _gridTable.DataRefreshAll();
                }
                else
                {
                    _gridTable.DataRequery(where.ToString());
                }
            }
            else
            {
                _gridTable.SetSelectWhereClause(where.ToString());
                _gridTable.Open(maxRows);
            }
            //  Go to Record 0
            SetCurrentRow(0, true);
            return success;
        }

        /// <summary>
        ///Transaction support.
        ///Depending on Table returns transaction info
        /// </summary>
        /// <returns>info</returns>
        public String GetTrxInfo()
        {
            //	InvoiceBatch
            if (_vo.TableName.StartsWith("C_InvoiceBatch"))
            {
                int Record_ID = _vo.GetCtx().GetContextAsInt(_vo.windowNo, "C_InvoiceBatch_ID");
                log.Fine(_vo.TableName + " - " + Record_ID);
                MessageFormat mf = null;
                try
                {
                    mf = new MessageFormat(Msg.GetMsg(Env.GetAD_Language(_vo.GetCtx()), "InvoiceBatchSummary"));
                }
                catch (Exception e)
                {
                    log.Log(Level.SEVERE, "InvoiceBatchSummary=" + Msg.GetMsg(Env.GetAD_Language(_vo.GetCtx()), "InvoiceBatchSummary"), e);
                }
                if (mf == null)
                    return " ";
                //    /**********************************************************************
                //     *	** Message: ExpenseSummary **
                //     *	{0} Line(s) {1,number,#,##0.00}  - Total: {2,number,#,##0.00}
                //     *
                //     *	{0} - Number of lines
                //     *	{1} - Toral
                //     *	{2} - Currency
                //     */
                Object[] arguments = new Object[3];
                bool filled = false;
                //    //
                String sql = "SELECT COUNT(*), NVL(SUM(LineNetAmt),0), NVL(SUM(LineTotalAmt),0) "
                    + "FROM C_InvoiceBatchLine "
                    + "WHERE C_InvoiceBatch_ID=" + Record_ID + " AND IsActive='Y'";
                //    //
                IDataReader dr = null;
                try
                {
                    dr = DataBase.DB.ExecuteReader(sql, null);
                    if (dr.Read())
                    {
                        //	{0} - Number of lines
                        int lines = Utility.Util.GetValueOfInt(dr[0]);
                        arguments[0] = lines;
                        //	{1} - Line net
                        Double net = Utility.Util.GetValueOfDouble(dr[1]);
                        arguments[1] = net;
                        //	{2} - Line net
                        Double total = Utility.Util.GetValueOfDouble(dr[2]);
                        arguments[2] = total;
                        filled = true;
                    }
                    dr.Close();
                    dr = null;
                }
                catch (System.Data.Common.DbException e)
                {
                    log.Log(Level.SEVERE, _vo.TableName + "\nSQL=" + sql, e);
                }
                finally
                {
                    if (dr != null)
                    {
                        dr.Close();
                        dr = null;
                    }
                }
                if (filled)
                    return mf.Format(arguments);
                return " ";
            }

            ////	Order || Invoice
            else if (_vo.TableName.StartsWith("C_Order") || _vo.TableName.StartsWith("C_Invoice"))
            {
                int Record_ID;
                bool isOrder = _vo.TableName.StartsWith("C_Order");
                //
                StringBuilder sql = new StringBuilder("SELECT COUNT(*) AS Lines,c.ISO_Code,o.TotalLines,o.GrandTotal,"
                    + "CURRENCYBASEWITHCONVERSIONTYPE(o.GrandTotal,o.C_Currency_ID,o.DateAcct, o.AD_Client_ID,o.AD_Org_ID, o.C_CONVERSIONTYPE_ID) AS ConvAmt ");
                if (isOrder)
                {
                    Record_ID = _vo.GetCtx().GetContextAsInt(_vo.windowNo, "C_Order_ID");
                    sql.Append("FROM C_Order o"
                        + " INNER JOIN C_Currency c ON (o.C_Currency_ID=c.C_Currency_ID)"
                        + " INNER JOIN C_OrderLine l ON (o.C_Order_ID=l.C_Order_ID) "
                        + "WHERE o.C_Order_ID=" + Record_ID);
                }
                else
                {
                    Record_ID = _vo.GetCtx().GetContextAsInt(_vo.windowNo, "C_Invoice_ID");
                    sql.Append("FROM C_Invoice o"
                        + " INNER JOIN C_Currency c ON (o.C_Currency_ID=c.C_Currency_ID)"
                        + " INNER JOIN C_InvoiceLine l ON (o.C_Invoice_ID=l.C_Invoice_ID) "
                        + "WHERE o.C_Invoice_ID= " + Record_ID);
                }
                sql.Append(" GROUP BY o.C_Currency_ID, c.ISO_Code, o.TotalLines, o.GrandTotal, o.DateAcct, o.AD_Client_ID, o.AD_Org_ID");

                log.Fine(_vo.TableName + " - " + Record_ID);

                MessageFormat mf = null;
                MessageFormat mfMC = null;
                try
                {
                    mf = new MessageFormat(Msg.GetMsg(Env.GetAD_Language(_vo.GetCtx()), "OrderSummary"));
                    mfMC = new MessageFormat(Msg.GetMsg(Env.GetAD_Language(_vo.GetCtx()), "OrderSummaryMC"));
                }
                catch (Exception e)
                {
                    log.Log(Level.SEVERE, "OrderSummary/MC", e);
                }
                if (mf == null || mfMC == null)
                    return " ";
                //    /**********************************************************************
                //     *	** Message: OrderSummary/MC **
                //     *	{0} Line(s) - {1,number,#,##0.00} - Total: {3}{2,number,#,##0.00} = {5}{4,number,#,##0.00}
                //     *	{0} Line(s) - {1,number,#,##0.00} - Total: {3}{2,number,#,##0.00}
                //     *
                //     *	{0} - Number of lines
                //     *	{1} - Line toral
                //     *	{2} - Grand total (including tax, etc.)
                //     *	{3} - Source Currency
                //     *	(4) - Grand total converted to local currency
                //     *	{5} - Base Currency
                //     */
                Object[] arguments = new Object[6];
                bool filled = false;
                IDataReader dr = null;

                try
                {

                    dr = DataBase.DB.ExecuteReader(sql.ToString());
                    if (dr.Read())
                    {
                        //	{0} - Number of lines
                        int lines = Utility.Util.GetValueOfInt(dr[0]);
                        arguments[0] = lines;
                        //	{1} - Line toral
                        decimal lineTotal = Utility.Util.GetValueOfDecimal(dr[2]);
                        arguments[1] = lineTotal;
                        //	{2} - Grand total (including tax, etc.)
                        decimal grandTotal = Utility.Util.GetValueOfDecimal(dr[3]);
                        //Double grandTotal = new Double(rs.getDouble(4));
                        arguments[2] = grandTotal;
                        //            //	{3} - Currency
                        String currency = dr[1].ToString();// rs.getString(2);
                        arguments[3] = currency;
                        //	(4) - Grand total converted to Base
                        decimal grandBase = Utility.Util.GetValueOfDecimal(dr[4]);
                        //  Double grandBase = new Double(rs.getDouble(5));
                        arguments[4] = grandBase;
                        arguments[5] = _vo.GetCtx().GetContext("$CurrencyISO");
                        filled = true;
                    }
                    dr.Close();
                }
                catch (System.Exception e)
                {

                    if (dr != null)
                    {
                        dr.Close();
                        dr = null;
                    }
                    log.Log(Level.SEVERE, _vo.TableName + "\nSQL=" + sql, e);
                }
                if (filled)
                {
                    if (arguments[2].Equals(arguments[4]))
                        return mf.Format(arguments);
                    else
                        return mfMC.Format(arguments);
                }
                return " ";
            }


            ////	Expense Report
            else if (_vo.TableName.StartsWith("S_TimeExpense") && _vo.tabNo == 0)
            {
                int Record_ID = _vo.GetCtx().GetContextAsInt(_vo.windowNo, "S_TimeExpense_ID");
                log.Fine(_vo.TableName + " - " + Record_ID);
                MessageFormat mf = null;
                try
                {
                    mf = new MessageFormat(Msg.GetMsg(Env.GetAD_Language(_vo.GetCtx()), "ExpenseSummary"));
                }
                catch (Exception e)
                {
                    log.Log(Level.SEVERE, "ExpenseSummary=" + Msg.GetMsg(Env.GetAD_Language(_vo.GetCtx()), "ExpenseSummary"), e);
                }
                if (mf == null)
                    return " ";
                //    /**********************************************************************
                //     *	** Message: ExpenseSummary **
                //     *	{0} Line(s) - Total: {1,number,#,##0.00} {2}
                //     *
                //     *	{0} - Number of lines
                //     *	{1} - Toral
                //     *	{2} - Currency
                //     */
                Object[] arguments = new Object[3];
                bool filled = false;
                //    //
                String SQL = "SELECT COUNT(*) AS Lines, SUM(ConvertedAmt*Qty) "
                     + "FROM S_TimeExpenseLine "
                     + "WHERE S_TimeExpense_ID=" + Record_ID;

                //    //
                IDataReader dr = null;
                try
                {
                    dr = DataBase.DB.ExecuteReader(SQL);

                    if (dr.Read())
                    {
                        //	{0} - Number of lines
                        int lines = Utility.Util.GetValueOfInt(dr[0]);
                        arguments[0] = lines;
                        //            //	{1} - Line toral
                        Double total = Utility.Util.GetValueOfDouble(dr[1]);
                        arguments[1] = total;
                        //            //	{3} - Currency
                        arguments[2] = " ";
                        filled = true;
                    }
                    dr.Close();
                    dr = null;
                }
                catch (System.Exception e)
                {

                    if (dr != null)
                    {
                        dr.Close();
                        dr = null;
                    }
                    log.Log(Level.SEVERE, _vo.TableName + "\nSQL=" + SQL, e);
                }
                if (filled)
                    return mf.Format(arguments);
                return " ";
            }
            ////	Default - No Trx Info
            return null;
        }





        //Current Data Status Event						
        private DataStatusEvent _DataStatusEvent = null;
        /// <summary>
        /// Set current row and load data into fields.
        /// If there is no row - load nulls
        /// </summary>
        /// <param name="newCurrentRow">new current row</param>
        /// <param name="fireEvents">fire events</param>
        /// <returns>current row</returns>
        private int SetCurrentRow(int newCurrentRow, bool fireEvents)
        {
            int oldCurrentRow = _currentRow;
            _currentRow = VerifyRow(newCurrentRow);
            log.Fine("Row=" + _currentRow + " - fire=" + fireEvents);

            //  Update Field Values
            int size = _gridTable.GetColumnCount();
            for (int i = 0; i < size; i++)
            {
                GridField mField = _gridTable.GetField(i);
                //  get Value from Table
                if (_currentRow >= 0)
                {
                    Object value = _gridTable.GetValueAt(_currentRow, i);
                    mField.SetValue(value, _gridTable.IsInserting());

                    // gwu: now always validated, not just when inserting
                    // mField.ValidateValue();
                }
                else
                {   //  no rows - set to a reasonable value - not updateable
                    //				Object value = null;
                    //				if (mField.isKey() || mField.isParent() || mField.getColumnName().equals(m_linkColumnName))
                    //					value = mField.getDefault();
                    // mField.SetValue();
                }
            }
            LoadDependentInfo();

            if (!fireEvents)    //  prevents informing twice
            {
                return _currentRow;
            }

            //  inform VTable/..    -> rowChanged
            ////m_propertyChangeSupport.firePropertyChange(PROPERTY, oldCurrentRow, _currentRow);

            //  inform APanel/..    -> dataStatus with row updated
            if (_DataStatusEvent == null)
            {
                _DataStatusEvent = new DataStatusEvent(this, GetRowCount(),
                    _gridTable.IsInserting(),		//	changed 
                    Env.GetCtx().IsAutoCommit(_vo.windowNo), _gridTable.IsInserting());
            }
            //
            _DataStatusEvent.SetCurrentRow(_currentRow);
            String status = _DataStatusEvent.GetAD_Message();
            if (status == null || status.Length == 0)
            {
                _DataStatusEvent.SetInfo("NavigateOrUpdate", null, false, false);
            }
            ////fireDataStatusChanged(_DataStatusEvent);
            return _currentRow;
        }

        /// <summary>
        /// Row Range check
        /// </summary>
        /// <param name="targetRow">target row</param>
        /// <returns>checked row</returns>
        private int VerifyRow(int targetRow)
        {
            int newRow = targetRow;
            //  Table Open?
            if (!_gridTable.IsOpen())
            {
                log.Severe("Table not open");
                return -1;
            }
            //  Row Count
            int rows = GetRowCount();
            if (rows == 0)
            {
                log.Fine("No Rows");
                return -1;
            }
            if (newRow >= rows)
            {
                newRow = rows - 1;
                log.Fine("Set to max Row: " + newRow);
            }
            else if (newRow < 0)
            {
                newRow = 0;
                log.Fine("Set to first Row");
            }
            return newRow;
        }

        /// <summary>
        /// Get RowCount
        /// </summary>
        /// <returns>row count</returns>
        public int GetRowCount()
        {
            int count = _gridTable.GetRowCount();
            //  Wait a bit if currently loading
            if (count == 0 && _gridTable.IsLoading())
            {
                try
                {
                    Thread.Sleep(100);      //  .1 sec
                }
                catch { }
                count = _gridTable.GetRowCount();
            }
            return count;
        }

    }

    public class AttachmentList
    {
        public int ID { get; set; }
        public int value { get; set; }
    }
}
