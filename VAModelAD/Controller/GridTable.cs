/********************************************************
// Module Name    : Run Time Show Window
// Purpose        : Grid Table default model
                    *  <pre>
                    *		The following data types are handled
                    *		Integer		for all IDs
                    *			Decimal	for all Numbers
                    *			DateTime	for all Dates
                    *			String		for all others
                    *  The data is read  from database and cached in Datatable. Writes/updates
                    *  are via dynamically constructed SQL INSERT/UPDATE statements. 
                    *  </pre>
                    *  The model maintains and fires the requires TableModelEvent changes,
                    *  the DataChanged events (loading, changed, etc.)
// Class Used     : GlobalVariable.cs, CommonFunctions.cs,Ctx.cs,Vlogger.cs
// Created By     : Harwinder 
// Date           : 20 jan 2009
**********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using System.Windows.Forms;
using System.Data;
using VAdvantage.SqlExec;
using System.Collections;
using VAdvantage.Model;
using System.Threading;
using VAdvantage.Common;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.Controller;

namespace VAdvantage.Model
{
    /* Event Handler For GridTable Class' datachanged and other event
     * this event attatched on "GridController.cs" and call "Vform" Setstsusline metod 
     *  to show stsusbar message and error dialog window*/
    public delegate void DataStatusEEventHandler(Object sender, DataStatusEvent e);

    public class GridTable
    {
        #region delaration
        public int _AD_Table_ID;
        public string _tableName = "";
        public bool _withAccessControl;
        public bool _readOnly = true;
        public bool _deleteable = true;
        //
        public List<GridField> m_fields = new List<GridField>(30);
        List<String> m_fieldName = new List<string>(30);

        /**	Row count                    */
        private int _rowCount = 0;
        /**	Has Data changed?           */
        private bool _changed = false;
        /** Index of changed row via SetValueAt */
        private int _rowChanged = -1;
        /** Insert mode active          */
        private bool _inserting = false;
        /** Inserted Row number         */
        private int _newRow = -1;
        /**	Is the Resultset open?      */
        private bool _open = false;
        /**	Compare to DB before save	*/
        private bool _compareDB = true;		//	set to true after every save
        /**	The buffer for all data		*/
        //volatile List<Object[]> _buffer = new List<Object[]>(100);
        volatile DataSet _buffer;
        volatile DataTable _bufferTable;
        private DataTable defaultModel = new DataTable();

        /** Original row data               */
        private object[] _rowData = null;
        //private Object[] _rowData = null;
        /** Original data [row,col,data]    */
        //private DataRow _oldValue = null;
        //private Object[] _oldValue = null;

        /**	Columns                 		*/

        List<Object> _parameterSELECT = new List<Object>(5);
        List<Object> _parameterWHERE = new List<Object>(5);

        /** Complete SQL statement          */
        string _SQL;

        public string SQL
        {
            get { return _SQL; }
            set { _SQL = value; }
        }
        /** SQL Statement for Row Count     */
        string _SQL_Count;
        /** The SELECT clause with FROM     */
        private string _SQL_Select;
        /** The static where clause         */
        public string _whereClause = "";
        /** Static ORDER BY clause          */
        public string _orderClause = "";
        /** Max Rows to query or 0 for all	*/
        private int _maxRows = 0;
        /** Index of Key Column                 */
        public int _indexKeyColumn = -1;
        /** Index of Color Column               */
        public int _indexColorColumn = -1;
        /** Index of Processed Column           */
        public int _indexProcessedColumn = -1;
        /** Index of IsActive Column            */
        public int _indexActiveColumn = -1;
        /** Index of AD_Client_ID Column        */
        public int _indexClientColumn = -1;
        /** Index of AD_Org_ID Column           */
        public int _indexOrgColumn = -1;
        public int _windowNo;
        /** Tab No 0..				*/
        public int _tabNo;
        Ctx _ctx;
        private Thread loader;
        /** Save OK - O		*/
        public const char SAVE_OK = 'O';			//	the only OK condition
        /** Save Error - E	*/
        public const char SAVE_ERROR = 'E';
        /** Save Access Error - A	*/
        public const char SAVE_ACCESS = 'A';
        /** Save Mandatory Error - M	*/
        public const char SAVE_MANDATORY = 'M';
        /** Save Abort Error - U	*/
        public const char SAVE_ABORT = 'U';

        /** Saved for Future - F	*/
        public const char SAVE_FUTURE = 'F';

        /** Sent for WF Approval - W	*/
        public const char SAVE_WFAPPROVAL = 'W';

        /** Back Date Version - B	*/
        public const char SAVE_BACKDATEVER = 'B';

        /* Paging */
        int _currentPage = 1;

        public void SetCurrentPage(int cPage)
        {
            _currentPage = cPage;
        }

        /// <summary>
        /// return Current page[Paging]
        /// </summary>
        /// <returns></returns>
        public int GetCurrentPage()
        {
            return _currentPage;
        }
        /*error message event*/

        public event DataStatusEEventHandler FireDataStatusEvent;

        public event EventHandler VetoableChange;
        Ctx ctx = Utility.Env.GetCtx();

        public bool isMultipleProduct = false;

        /** Logging					*/
        static VLogger log = VLogger.GetVLogger(typeof(GridTable).FullName);

        private int _currentRow = -1;
        public int CurrentRow
        {
            set
            {
                _currentRow = value;
            }

        }

        #endregion

        /// <summary>
        /// Return Key Column Index
        /// </summary>
        /// <returns></returns>
        public int GetKeyIndex()
        {
            return _indexKeyColumn;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tctx"></param>
        /// <param name="AD_Table_ID"></param>
        /// <param name="TableName"></param>
        /// <param name="winNo"></param>
        /// <param name="tabNo"></param>
        /// <param name="withAccessControl"></param>
        public GridTable(Ctx tctx, int AD_Table_ID, string TableName, int winNo, int tabNo, bool withAccessControl, GridTab tabObj)
        {
            _AD_Table_ID = AD_Table_ID;
            _ctx = tctx;
            SetTableName(TableName);
            _tabNo = tabNo;
            _windowNo = winNo;
            _withAccessControl = withAccessControl;
            //_bufferTable = new DataTable(TableName);
            //_bufferTable.sets
        }

        public GridTable()
        {
        }

        /// <summary>
        ///	Add Field to Field Array
        /// </summary>
        /// <param name="field"></param>
        public void AddField(GridField field)
        {
            log.Fine("(" + _tableName + ") - " + field.GetColumnName());
            if (_open)
            {
                log.Log(Level.SEVERE, "Table already open - ignored: " + field.GetColumnName());
                return;
            }
            if (!MRole.GetDefault(_ctx, false).IsColumnAccess(_AD_Table_ID, field.GetAD_Column_ID(), true))
            {
                log.Fine("No Column Access " + field.GetColumnName());
                return;
            }
            //  Set Index for Key column
            if (field.IsKey())
                _indexKeyColumn = m_fields.Count;
            else if (field.GetColumnName().Equals("IsActive"))
                _indexActiveColumn = m_fields.Count;
            else if (field.GetColumnName().Equals("Processed"))
                _indexProcessedColumn = m_fields.Count;
            else if (field.GetColumnName().Equals("AD_Client_ID"))
                _indexClientColumn = m_fields.Count;
            else if (field.GetColumnName().Equals("AD_Org_ID"))
                _indexOrgColumn = m_fields.Count;
            //
            m_fields.Insert(m_fields.Count, field);
            m_fieldName.Insert(m_fieldName.Count, field.GetColumnName().ToLower());
            UpdateTableModel(field);

            //_bufferTable.Columns.Add(field.GetColumnName());
        }

        /// <summary>
        /// Create Default Table Model 
        /// </summary>
        /// <param name="field"></param>
        private void UpdateTableModel(GridField field)
        {
            string columnName = field.GetColumnName();
            int displayType = field.GetDisplayType();
            DataColumn dc = null;
            //	Integer, ID, Lookup (UpdatedBy is a numeric column)
            if (displayType == DisplayType.Integer
                || (DisplayType.IsID(displayType) // JJ: don't touch!
                    && (columnName.EndsWith("_ID") || columnName.EndsWith("_Acct") || columnName.EndsWith("_ID_1") || columnName.EndsWith("_ID_2") || columnName.EndsWith("_ID_3")))
                || columnName.EndsWith("atedBy"))
            {
                dc = new DataColumn(columnName, typeof(Int32));
            }
            //	Number
            else if (DisplayType.IsNumeric(displayType))
                dc = new DataColumn(columnName, typeof(decimal));			//	BigDecimal
            //	Date
            else if (DisplayType.IsDate(displayType))
                dc = new DataColumn(columnName, typeof(DateTime));	//	Timestamp
            //	RowID or Key (and Selection)
            else if (displayType == DisplayType.RowID)
                dc = new DataColumn(columnName, typeof(object));
            //	YesNo
            else if (displayType == DisplayType.YesNo)
            {
                //if (field.isEncryptedColumn())
                //	str = (String)decrypt(str);
                dc = new DataColumn(columnName, typeof(string));	//	Boolean
            }
            //	LOB
            else if (DisplayType.IsLOB(displayType))
            {
                if (DisplayType.Binary == displayType)
                {
                    dc = new DataColumn(columnName, typeof(Byte[]));
                }
                else
                {
                    dc = new DataColumn(columnName, typeof(String));
                }
            }
            // For EnterpriseDB (Vienna Type Long Text is stored as Text in EDB)
            //	String
            else
                dc = new DataColumn(columnName, typeof(String));

            defaultModel.Columns.Add(dc);
        }


        /// <summary>
        /// Set TAble Name
        /// </summary>
        /// <param name="newTableName"></param>
        public void SetTableName(string newTableName)
        {
            if (_open)
            {
                log.Fine("Table already open - ignored");
                return;
            }
            if (newTableName == null || newTableName.Length == 0)
                return;
            _tableName = newTableName;
        }	//	setTableName

        /// <summary>
        /// Set Read Only
        /// </summary>
        /// <param name="value"></param>
        public void SetReadOnly(bool value)
        {
            _readOnly = value;
        }	//	setReadOnly

        public bool GetReadOnly()
        {
            return false;
        }	//

        /// <summary>
        /// Set Is Deleteable
        /// </summary>
        /// <param name="value"></param>
        public void SetDeleteable(bool value)
        {
            _deleteable = value;
        }

        /// <summary>
        /// Get GridField By index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public GridField GetField(int index)
        {
            if (index < 0 || index >= m_fields.Count)
                return null;
            return m_fields[index];
        }

        /// <summary>
        /// Get GridField By Name
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        //public GridField GetField(string identifier)
        //{
        //    if (identifier == null || identifier.Length == 0)
        //        return null;
        //    int cols = m_fieldName.IndexOf(identifier.ToLower());

        //    if (cols > -1)
        //    {
        //        return m_fields[cols];
        //    }

        //    //for (int i = 0; i < cols; i++)
        //    //{
        //    //    GridField field = m_fields[i];
        //    //    if (identifier.ToLower().Equals(field.GetColumnName().ToLower()))
        //    //        return field;
        //    //}
        //    //	log.log(Level.WARNING, "Not found: '" + identifier + "'");
        //    return null;
        //}	//	GetField

        public GridField GetField(String identifier)
        {
            if (identifier == null || identifier.Length == 0)
                return null;
            int cols = m_fields.Count;
            for (int i = 0; i < cols; i++)
            {
                GridField field = m_fields[i];
                if (StringComparer.OrdinalIgnoreCase.Equals(identifier, field.GetColumnName()))
                {
                    return field;
                }
            }
            log.Log(Level.WARNING, "Not found: '" + identifier + "'");
            return null;
        }	//	getField


        /// <summary>
        /// Is inserting
        /// </summary>
        /// <returns></returns>
        public bool IsInserting()
        {
            return _inserting;
        }   //  isInserting


        /// <summary>
        /// Get Field Count
        /// </summary>
        /// <returns></returns>
        public int GetColumnCount()
        {
            return m_fields.Count;
        }	//	getColumnCount

        /// <summary>
        /// Set Order Clause of query 
        /// </summary>
        /// <param name="newOrderClause"></param>
        public void SetOrderClause(string newOrderClause)
        {
            _orderClause = newOrderClause;
            if (_orderClause == null)
                _orderClause = "";
        }

        /// <summary>
        /// Get GridField Array
        /// </summary>
        /// <returns></returns>
        public GridField[] GetFields()
        {
            GridField[] retValue = new GridField[m_fields.Count];
            //m_fields.ToArray(retValue);
            retValue = m_fields.ToArray();
            return retValue;
        }

        /// <summary>
        /// dont ' use this function 
        /// *this is for Visual Editor form only
        /// </summary>
        /// <returns></returns>
        public List<GridField> GetFieldList()
        {
            return m_fields;
        }
        /// <summary>
        ///Returns a column index by column name.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public int FindColumn(string columnName)
        {
            for (int i = 0; i < m_fields.Count; i++)
            {
                GridField field = m_fields[i];
                if (columnName.Equals(field.GetColumnName()))
                    return i;
            }
            return -1;
        }   //  

        /// <summary>
        ///Is it open?
        /// </summary>
        /// <returns></returns>
        public bool IsOpen()
        {
            return _open;
        }	//

        /// <summary>
        ///Requery with new whereClause
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public bool DataRequery(string whereClause)
        {
            log.Info(whereClause);
            Close(false);
            SetSelectWhereClause(whereClause);
            Open(_maxRows);
            //  Info
            _rowData = null;
            _changed = false;
            _rowChanged = -1;
            _inserting = false;
            //fireTableDataChanged();
            FireDataStatusIEvent("Refreshed", "");
            return true;
        }	//	dataRequery

        /// <summary>
        /// Clear Buffer
        /// </summary>
        /// <param name="finalCall"></param>
        public void Close(bool finalCall)
        {
            if (!_open)
                return;
            log.Fine("final=" + finalCall);

            //  remove listeners
            if (finalCall)
            {

            }

            //	Stop loader
            while (loader != null && loader.IsAlive)
            {
                //log.fine("Interrupting Loader ...");
                loader.Interrupt();
                try
                {
                    Thread.Sleep(200);		//	.2 second
                }
                catch (Exception)
                { }
            }

            if (!_inserting)
            {
                //DataSave(false);	//	not manual
            }

            if (_buffer != null)
            {
                _buffer.Clear();
            }
            _buffer = null;

            if (_bufferTable != null)
            {

                _bufferTable.Clear();
                _bufferTable = null;
            }
            //if (m_sort != null)
            //    m_sort.clear();
            //m_sort = null;

            if (finalCall)
            {
                Dispose();
            }

            //  Fields are disposed from MTab
            log.Fine("");
            _open = false;

        }	//	close

        /// <summary>
        /// Dispose Object members
        /// </summary>
        private void Dispose()
        {
            //  MFields
            for (int i = 0; i < m_fields.Count; i++)
            {
                m_fields[i].Dispose();
            }
            m_fields.Clear();
            m_fields = null;

            _parameterSELECT.Clear();
            _parameterSELECT = null;
            _parameterWHERE.Clear();
            _parameterWHERE = null;
            //  clear data arrays
            _buffer = null;
            _bufferTable = null;
            _rowData = null;
           // _oldValue = null;
            //_tabObj = null;
            loader = null;
        }   //  dispose

        /// <summary>
        ///Set Where Clause (w/o the WHERE and w/o History).
        /// </summary>
        /// <param name="newWhereClause"></param>
        /// <returns></returns>
        public bool SetSelectWhereClause(string newWhereClause)
        {
            if (_open)
            {
                log.Log(Level.SEVERE, "Table already open - ignored");
                return false;
            }
            //
            _whereClause = newWhereClause;
            if (_whereClause == null)
                _whereClause = "";
            return true;
        }

        ///// <summary>
        ///// Get Records (call from Vfom Class)
        ///// </summary>
        ///// <returns></returns>
        public DataTable GetRecords()
        {
            return _bufferTable;
        }

        public int GetTableID()
        {
            return _AD_Table_ID;
        }

        /// <summary>
        ///Return Default Table Model
        /// </summary>
        /// <returns></returns>
        public DataTable GetTableModel()
        {
            return defaultModel;
        }

        //public DataTable GetDataTable()
        //{
        //    if(loader != null && loader.IsAlive) 
        //    {
        //        try
        //        {
        //            loader.Join();
        //        }
        //        catch
        //        {
        //            Common.ErrorLog.FillErrorLog("GridTable.GetRecords()", "", "thread join error", VAdvantage.Framework.Message.MessageType.ERROR);
        //        }
        //    }
        //    return _buffer.Tables[0];
        //}

        /// <summary>
        /// is Alive
        /// </summary>
        /// <returns></returns>
        public bool IsAlive()
        {
            return (loader != null && loader.IsAlive);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //public int GetRowCount()
        //{
        //    if (_buffer !=null && _buffer.Tables.Count > 0)
        //    {
        //        return _buffer.Tables[0].Rows.Count;
        //    }
        //    return 0;
        //}

        /// <summary>
        /// Refresh All record 
        /// </summary>
        public void DataRefreshAll()
        {
            log.Info("");
            _inserting = false;	//	should not happen
            DataIgnore();
            Close(false);
            Open(_maxRows);
            //	Info
            _rowData = null;
            _changed = false;
            _rowChanged = -1;
            _inserting = false;
            //fireTableDataChanged();
            FireDataStatusIEvent("Refreshed", "");
        }


        //public bool IsVisualEditor
        //{
        //    get;
        //    set;
        //}


        /// <summary>
        /// create Sql query and intilize thread to get data
        /// </summary>
        /// <param name="maxRows"></param>
        /// <returns></returns>
        public bool Open(int maxRows)
        {
            log.Info("MaxRows=" + maxRows);
            _maxRows = maxRows;
            if (_open)
            {
                log.Fine("already open");
                DataRefreshAll();
                return true;
            }

            //	create _SQL and m_countSQL
            CreateSelectSql();
            if (_SQL == null || _SQL.Equals(""))
            {
                log.Log(Level.SEVERE, "No SQL");
                return false;
            }

            //	Start Loading
            // System.Threading.Thread loader = new System.Threading.Thread();

            _bufferTable = new DataTable();
            _bufferTable = defaultModel.Clone();

            //if (!IsVisualEditor)
            //{

            //    loader = new Thread(new ThreadStart(FillData));
            //    //loader.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            //    //loader.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentUICulture;
            //    loader.CurrentCulture = Utility.Env.GetLanguage(Utility.GetCtx()).GetCulture(Utility.Env.GetLoginLanguage(Utility.GetCtx()).GetAD_Language());
            //    loader.CurrentUICulture = Utility.Env.GetLanguage(Utility.GetCtx()).GetCulture(Utility.Env.GetLoginLanguage(Utility.GetCtx()).GetAD_Language());

            //}
            _rowCount = CountRecords(maxRows);
            if (_rowCount > 0)
            {
                //if (!IsVisualEditor)
                //{
                //    loader.Start();
                //}
                //else
                //{
                FillData();
                //}
            }
            else
            {
                // loader = null;
            }

            _open = true;
            //
            _changed = false;
            _rowChanged = -1;

            //fireTableDataChanged();
            //	Audit
            if (_rowCount > 0)
            {
                MSession session = MSession.Get(_ctx, true);
                session.QueryLog(_ctx.GetAD_Client_ID(), _ctx.GetAD_Org_ID(), _AD_Table_ID,
                    _SQL_Count, _rowCount);
            }
            return true;
        }

        /// <summary>
        /// Get record count and fill Dataset
        /// </summary>
        /// <param name="maxRows"></param>
        /// <returns></returns>
        private int CountRecords(int maxRows)
        {
            int rows = 0;
            try
            {
                object o = DataBase.DB.ExecuteScalar(_SQL_Count);

                if (o != null)
                {
                    rows = Convert.ToInt32(o);
                }
                o = null;
            }
            catch (System.Data.Common.DbException e0)
            {
                //	Zoom Query may have invalid where clause
                if (e0.ErrorCode == 904) 	//	ORA-00904: "C_x_ID": invalid identifier
                {
                    //Common.ErrorLog.FillErrorLog("GridTable.CountRecords()",  "Count - " + e0.Message + "\nSQL=" + _SQL_Count, e0.Message, VAdvantage.Framework.Message.MessageType.ERROR);
                    log.Warning("Count - " + e0.Message + "\nSQL=" + _SQL_Count);
                }
                else
                {
                    //Common.ErrorLog.FillErrorLog("GridTable.CountRecords()",  "Count - " + e0.Message + "\nSQL=" + _SQL_Count, e0.Message, VAdvantage.Framework.Message.MessageType.ERROR);
                    log.Log(Level.SEVERE, "Count SQL=" + _SQL_Count, e0);
                }
                return 0;
            }
            //log.Fine(info.toString());
            return rows;
        }


        /// <summary>
        ///Wait until async loader of Table and Lookup Fields is complete
        ///  
        /// </summary>
        public void LoadComplete()
        {
            //  Wait for loader
            if (loader != null)
            {
                if (loader.IsAlive)
                {
                    try
                    {
                        loader.Join();
                    }
                    catch (Exception ie)
                    {
                        //Common.ErrorLog.FillErrorLog("GridTable.LoadComplete()", "", ie.Message, VAdvantage.Framework.Message.MessageType.ERROR);
                        log.Log(Level.SEVERE, "Join interrupted", ie);
                    }
                }
            }
            //  wait for field lookup loaders
            // for (int i = 0; i < m_fields.size(); i++)
            // {
            //    GridField field = (GridField)m_fields.get(i);
            //   field.lookupLoadComplete();
            // }
        }   //  loadComplete





        ///// <summary>
        ///// get Data and fill  dataset
        ///// </summary>
        //protected void FillData()
        //{
        //    try
        //    {
        //        //_buffer.Locale = Utility.Env.GetLanguage(Utility.GetCtx()).GetCulture();
        //        //IDataReader   dr = DataBase.DB.ExecuteDataset(_SQL);
        //        _buffer = DataBase.DB.ExecuteDataset(_SQL);
        //        _bufferTable = _buffer.Tables[0];
        //    }
        //    catch( Exception e)
        //    {
        //        _buffer = null;
        //        _bufferTable = null;
        //        Common.ErrorLog.FillErrorLog("GridTable.FillData()", _SQL, e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
        //    }
        //    if (_bufferTable != null && _bufferTable.Rows.Count > 0)
        //    {
        //        _rowCount = _bufferTable.Rows.Count;
        //        MSession session = MSession.Get(_ctx, true);
        //        session.queryLog(_ctx.GetAD_Client_ID(), _ctx.GetAD_Org_ID(), _AD_Table_ID,
        //              "Row Count", _rowCount);
        //    }	//	open
        //}
        //

        /// <summary>
        /// Asyncronous fill data
        /// </summary>
        protected void FillData()
        {
            //log.info("");

            StringBuilder info = new StringBuilder("Rows=");
            info.Append(_rowCount);
            if (_rowCount == 0)
                info.Append(" - ").Append(_SQL_Count);
            //	open Statement (closed by Loader.close)
            try
            {
                if (_maxRows > 0 && _rowCount > _maxRows)
                {
                    /************************ Set Max Rows ***********************************/
                    // m_pstmt.setMaxRows(maxRows);
                    info.Append(" - MaxRows=").Append(_maxRows);
                    _rowChanged = _maxRows;
                }
                _buffer = DataBase.DB.ExecuteDataset(_SQL);
            }
            catch (System.Data.Common.DbException e)
            {
                // Common.ErrorLog.FillErrorLog("GridTable.CountRecords()", _SQL, e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
                log.Log(Level.SEVERE, _SQL, e);
                return;
            }
            //Common.ErrorLog.FillErrorLog("GridTable.CountRecords()", "", info.ToString(), VAdvantage.Framework.Message.MessageType.INFORMATION);
            log.Info(info.ToString());
            if (_buffer == null)
                return;
            int count = 0;
            try
            {
                count = _buffer.Tables[0].Rows.Count;
                // _bufferTable.BeginLoadData();
                _bufferTable.Clear();
                //_bufferTable.Merge(_buffer.Tables[0], false, MissingSchemaAction.Ignore);
                //_bufferTable.EndLoadData();

                //while (m_rs.next())
                for (int i = 0; i < count; i++)
                {
                    //if (this.isInterrupted())
                    //    //{
                    //    //    log.fine("Interrupted");
                    //    //    close();
                    //    //    return;
                    //    //}
                    //    //  Get Data
                    DataRow dr = _bufferTable.NewRow();

                    ReadData(dr, _buffer.Tables[0].Rows[i]);

                    _bufferTable.Rows.Add(dr);
                    //    //	add Data
                    //    MSort sort = new MSort(m_buffer.size(), null);	//	index
                    //    m_buffer.add(rowData);
                    //    m_sort.add(sort);

                    //    //	Statement all 250 rows & sleep
                    //    if (m_buffer.size() % 250 == 0)
                    //    {
                    //        //	give the other processes a chance
                    //        try
                    //        {
                    //            yield();
                    //            sleep(10);		//	.01 second
                    //        }
                    //        catch (InterruptedException ie)
                    //        {
                    //            log.fine("Interrupted while sleeping");
                    //            close();
                    //            return;
                    //        }
                    //        DataStatusEvent evt = createDSE();
                    //        evt.setLoading(m_buffer.size());
                    //        fireDataStatusChanged(evt);
                    //    }
                }	//	while(rs.next())
                _buffer.Clear();
                _buffer = null;
            }
            catch (System.Data.Common.DbException e)
            {
                //Common.ErrorLog.FillErrorLog("GridTable.FillData()", " Datatatble Merge Error", e.Message, VAdvantage.Framework.Message.MessageType.ERROR); 
                log.Log(Level.SEVERE, "Datatatble Merge Error", e);
            }

            catch (ArgumentException e)
            {
                log.Log(Level.SEVERE, "Merge Error " + e.InnerException, e);
            }

            //catch (Exception e)
            //{
            //    log.Log(Level.SEVERE, "Merge Error " + e.InnerException, e);
            //}

            //close();
            // FireDataStatusIEvent("", "");
        }

        /// <summary>
        /// Create Select Sql String
        /// </summary>
        /// <returns></returns>
        private string CreateSelectSql()
        {
            if (m_fields.Count == 0 || _tableName == null || _tableName.Equals(""))
                return "";

            //	Create SELECT Part
            StringBuilder select = new StringBuilder("SELECT ");
            string selectSql = null;
            for (int i = 0; i < m_fields.Count; i++)
            {
                if (i > 0)
                    select.Append(",");
                GridField field = m_fields[i];
                selectSql = field.GetColumnSQL(true);
                if (selectSql.IndexOf("@") == -1)
                {
                    select.Append(selectSql);	//	ColumnName or Virtual Column
                }
                else
                {
                    select.Append(Utility.Env.ParseContext(_ctx, _windowNo, selectSql, false));
                }
            }
            selectSql = null;
            //
            select.Append(" FROM ").Append(_tableName);
            _SQL_Select = select.ToString();
            _SQL_Count = "SELECT COUNT(*) FROM " + _tableName;
            //

            StringBuilder m_SQL_Where = new StringBuilder("");
            //	WHERE
            if (_whereClause.Length > 0)
            {
                m_SQL_Where.Append(" WHERE ");
                if (_whereClause.IndexOf("@") == -1)
                {
                    m_SQL_Where.Append(_whereClause);
                }
                else    //  replace variables
                    m_SQL_Where.Append(Utility.Env.ParseContext(_ctx, _windowNo, _whereClause, false));
                //
                if (_whereClause.ToUpper().IndexOf("=NULL") > 0)
                {
                    log.Severe("Invalid NULL - " + _tableName + "=" + _whereClause);
                }
            }
            //List<GridField> jj = new List<GridField>();

            //	RO/RW Access
            _SQL = _SQL_Select + m_SQL_Where.ToString();
            _SQL_Count += m_SQL_Where.ToString();
            if (_withAccessControl)
            {
                //	boolean ro = MRole.SQL_RO;
                //	if (!_readOnly)
                //		ro = MRole.SQL_RW;
                _SQL = MRole.GetDefault(_ctx, false).AddAccessSQL(_SQL,
                    _tableName, MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
                _SQL_Count = MRole.GetDefault(_ctx, false).AddAccessSQL(_SQL_Count,
                    _tableName, MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
            }

            //	ORDER BY
            if (!_orderClause.Equals(""))
                _SQL += " ORDER BY " + _orderClause;
            //
            //log.fine(_SQL_Count);
            _ctx.SetContext(_windowNo, _tabNo, "SQL", _SQL);
            return _SQL;
        }	//	createSelectSql

        /// <summary>
        ///	Check if the row needs to be saved.
        /// </summary>
        /// <param name="newRow"></param>
        /// <param name="onlyRealChange"></param>
        /// <returns></returns>
        public bool NeedSave(int newRow, bool onlyRealChange)
        {
            //if (!grdTable.IsCurrentRowDirty)
            //{
            //     return false;
            // }
            //if (!m_changed && m_rowChanged == -1)
            //    return false;
            ////  E.g. New unchanged records
            //if (m_changed && m_rowChanged == -1 && onlyRealChange)
            //    return false;
            ////  same row
            //if (newRow == m_rowChanged)
            //    return false;
            return true;
        }	//	needSa

        /// <summary>
        /// Store unchanged Row Data
        /// </summary>
        /// <param name="drv"></param>
        public bool SetOriginalData(DataRowView drv)
        {
            _rowData = new object[m_fields.Count];
            bool retVal = false;
            try
            {
                for (int col = 0; col < m_fields.Count; col++)
                {
                    _rowData[col] = drv[col];
                }
                retVal = true;
            }
            catch
            {
                _rowData = null;
                log.Info("DataRow maview is Null");
            }
            return retVal;
        }

        /// <summary>
        /// Save unconditional.
        /// OK Or Error condition
        ///  Error info (Access*, FillMandatory, SaveErrorNotUnique,
        /// SaveErrorRowNotFound, SaveErrorDataChanged) is saved in the log
        /// </summary>
        /// <param name="manualCmd"></param>
        /// <returns> OK Or Error condition</returns>
        public char DataSave(bool manualCmd, DataRowView drv)
        {
            //	cannot save
            if (!_open)
            {
                log.Warning("Error - Open=" + _open);
                return SAVE_ERROR;
            }

            //  Value not changed
            if (drv == null)
            {
                log.Fine("No Changes");
                return SAVE_ERROR;
            }
            if (_readOnly)
            //	If Processed - not editable (Find always editable)  -> ok for changing payment terms, etc.
            {
                log.Warning("IsReadOnly - ignored");
                DataIgnore();
                return SAVE_ACCESS;
            }

            int[] co = GetClientOrg(drv);
            int AD_Client_ID = co[0];
            int AD_Org_ID = co[1];
            bool createError = true;
            if (!MRole.GetDefault(_ctx, false)
                 .CanUpdate(AD_Client_ID, AD_Org_ID, _AD_Table_ID, 0, createError))
            {
                ShowInfoMessage("", "can-not Update records(access)");
                FireDataStatusEEvent("can-not Update records(access)", "", true);//CLogger.retrieveError());
                DataIgnore();
                return SAVE_ACCESS;
            }

            try
            {
                if (!manualCmd)
                {
                    VetoableChange(this, EventArgs.Empty);
                }
            }
            catch (Exception pve)
            {
                log.Warning(pve.Message);
                //log.Config(Level.INFO, "GridTable.DataSave()"  "cancel by user", pve.Message);
                if (!isMultipleProduct)
                {
                    DataIgnore();
                }
                return SAVE_ABORT;
            }
            //	Check Mandatory
            string missingColumns = GetMandatory(drv);
            if (missingColumns.Length != 0)
            {
                //	Trace.printStack(false, false);
                FireDataStatusEEvent("FillMandatory", missingColumns + "\n", true);
                drv.EndEdit();
                //ShowInfoMessage("FillMandatory", missingColumns + "\n");
                return SAVE_MANDATORY;
            }


            //	Check miscellaneous errors
            string errorColumns = GetErrorColumns();
            if (errorColumns.Length != 0)
            {
                //	Trace.printStack(false, false);
                FireDataStatusEEvent("Error", errorColumns + "\n", true);
                //ShowInfoMessage("Error", errorColumns + "\n");
                return SAVE_ERROR;
            }
            /*** Update row ***/

            /**
         *	Update row *****
         */
            int recordID = 0;
            try
            {
                if (!_inserting)
                    recordID = Convert.ToInt32(drv[_indexKeyColumn]);
            }
            catch
            {
                recordID = -1;
            }
            try
            {
                if (!_tableName.EndsWith("_Trl"))	//	translation tables have no model
                    return DataSavePO(recordID, drv);
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(NullReferenceException) && e.Message.Contains("No Persistent "))
                {
                    log.Warning(_tableName + " - " + e.Message);
                    //ErrorLog.FillErrorLog("SaveORUpdatePO", "PO Null", e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
                }
                else
                {
                    log.Log(Level.SEVERE, "Persistency Issue - "
                        + _tableName + ": " + e.Message, e);
                    //ErrorLog.FillErrorLog("Save Table", "DataSave by PO", e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
                    return SAVE_ERROR;
                }
            }

            /**	Manual Update of Row (i.e. not via PO class)	**/
            log.Info("NonPO");
            //ErrorLog.FillErrorLog("NonPO", "Save not using PO", "", VAdvantage.Framework.Message.MessageType.ERROR);

            bool error = false;
            LobReset();
            //
            String message = null;
            const String ERROR = "ERROR: ";
            const String INFO = "Info: ";


            //	Update SQL with specific where clause
            StringBuilder select = new StringBuilder("SELECT ");
            for (int i = 0; i < m_fields.Count; i++)
            {
                GridField field = m_fields[i];
                if (_inserting && field.IsVirtualColumn())
                    continue;

                if (i > 0)
                    select.Append(",");

                if (field.GetColumnSQL(true).IndexOf("@") != -1)
                {
                    //****Utility.Env.ParseContext used to parse @,,@ and get value from context from column sql
                    select.Append(Utility.Env.ParseContext(_ctx, _windowNo, field.GetColumnSQL(true), false));
                    continue;
                }
                select.Append(field.GetColumnSQL(true));	//	ColumnName or Virtual Column
            }

            select.Append(" FROM ").Append(_tableName);
            StringBuilder singleRowWHERE = new StringBuilder();
            StringBuilder multiRowWHERE = new StringBuilder();
            //	Create SQL	& RowID
            if (_inserting)
            {
                select.Append(" WHERE 1=2");
            }
            else	//  FOR UPDATE causes  -  ORA-01002 fetch out of sequence
            {
                select.Append(" WHERE ").Append(GetWhereClause(drv));
            }

            IDataReader drRef = null;
            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(select.ToString());
                //	only one row
                if (!(_inserting || ((System.Data.Common.DbDataReader)dr).HasRows))
                {
                    //  dt.Dispose();
                    dr.Close();
                    //ShowInfoMessage("SaveErrorRowNotFound", "");
                    FireDataStatusEEvent("SaveErrorRowNotFound", "", true);
                    // dataRefresh(m_rowChanged);
                    return SAVE_ERROR;
                }

                bool manualUpdate = true;
                //if (DataBase.isRemoteObjects())
                //    manualUpdate = true;

                object[] rowDataDB = null;
                //Prepare

                if (((System.Data.Common.DbDataReader)dr).HasRows)
                {
                    while (dr.Read())
                    {
                        rowDataDB = ReadData(dr);
                        break;
                    }
                }
                else
                {
                    int size = m_fields.Count;
                    rowDataDB = new object[size];
                }
                dr.Close();
                dr = null;

                /**	Data:
             *		_rowData	= original Data
             *		 drv 	    = updated Data
             *		rowDataDB	= current Data in DB
             *	1) Difference between original & updated Data?	N:next
             *	2) Difference between original & current Data?	Y:don't update
             *	3) Update current Data
             *	4) Refresh to get last Data (changed by trigger, ...)
             */
                //	Constants for Created/Updated(By)
                DateTime now = DateTime.Now;
                int user = _ctx.GetAD_User_ID();

                /**
                 *	for every column
                 */
                int size1 = m_fields.Count;
                int colRs = 1;

                //System.Threading.Thread.CurrentThread.CurrentCulture = Utility.Env.GetLanguage(Utility.GetCtx()).GetCulture(Utility.Env.GetBaseAD_Language());
                //System.Threading.Thread.CurrentThread.CurrentUICulture = Utility.Env.GetLanguage(Utility.GetCtx()).GetCulture(Utility.Env.GetBaseAD_Language());


                for (int col = 0; col < size1; col++)
                {
                    GridField field = m_fields[col];
                    if (field.IsVirtualColumn())
                    {
                        if (!_inserting)
                            colRs++;
                        continue;
                    }
                    String columnName = field.GetColumnName();
                    if (field.GetDisplayType() == DisplayType.RowID
                    || field.IsVirtualColumn())
                    {
                        ; //	ignore
                    }
                    //	New Key
                    else if (field.IsKey() && _inserting)
                    {
                        if (columnName.EndsWith("_ID") || columnName.ToUpper().EndsWith("_ID"))
                        {
                            int insertID = DB.GetNextID(_ctx, _tableName, null);	//	no trx
                            if (manualUpdate)
                            {
                                CreateUpdateSql(columnName, insertID.ToString());
                            }
                            else
                            {
                                //rs.updateInt (colRs, insertID); 						// ***
                            }
                            singleRowWHERE.Append(columnName).Append("=").Append(insertID);
                            //
                            message = INFO + columnName + " -> " + insertID.ToString() + " (Key)";
                        }

                        else //	Key with String value
                        {
                            String str = drv[col].ToString();
                            if (manualUpdate)
                                CreateUpdateSql(columnName, DataBase.GlobalVariable.TO_STRING(str));
                            else
                            {
                                //	rs.updateString (colRs, str); 						// ***
                            }
                            singleRowWHERE = new StringBuilder();	//	overwrite
                            singleRowWHERE.Append(columnName).Append("=").Append(DataBase.GlobalVariable.TO_STRING(str));
                            //
                            message = INFO + columnName + " -> " + str + " (StringKey)";
                        }

                        log.Fine(message);
                    }
                    // CAse For Documnet Number
                    //  case New Value(key) 
                    //	New DocumentNo
                    else if (columnName.Equals("DocumentNo"))
                    {
                        bool newDocNo = false;
                        String docNo = drv[col].ToString();
                        //  we need to have a doc number
                        if (docNo == null || docNo.Length == 0)
                            newDocNo = true;
                        //  Preliminary ID from CalloutSystem
                        else if (docNo.StartsWith("<") && docNo.EndsWith(">"))
                            newDocNo = true;

                        if (newDocNo || _inserting)
                        {
                            String insertDoc = null;
                            //  always overwrite if insering with mandatory DocType DocNo
                            /****************************************Check******************************************************/
                            if (_inserting)
                                insertDoc = null; // DataBase.getDocumentNo (m_ctx, m_WindowNo, 
                            //m_tableName, true, null);	//	only doc type - no trx
                            log.Fine("DocumentNo entered=" + docNo + ", DocTypeInsert=" + insertDoc + ", newDocNo=" + newDocNo);
                            // can we use entered DocNo?
                            if (insertDoc == null || insertDoc.Length == 0)
                            {
                                if (!newDocNo && docNo != null && docNo.Length > 0)
                                    insertDoc = docNo;
                                else //  get a number from DocType or Table
                                    insertDoc = MSequence.GetDocumentNo(AD_Client_ID, _tableName, null,_ctx);// DataBase.getDocumentNo(m_ctx, m_WindowNo, 
                                //m_tableName, false, null);	//	no trx
                                /****************************************Check********************************************************/
                            }
                            //	There might not be an automatic document no for this document
                            if (insertDoc == null || insertDoc.Length == 0)
                            {
                                //  in case DB function did not return a value
                                if (docNo != null && docNo.Length != 0)
                                    insertDoc = drv[col].ToString();
                                else
                                {
                                    error = true;
                                    message = ERROR + field.GetColumnName() + "= " + drv[col] + " NO DocumentNo";
                                    log.Fine(message);
                                    //Common.ErrorLog.FillErrorLog("SaveWithutPO","",message,VAdvantage.Framework.Message.MessageType.ERROR);
                                    break;
                                }
                            }
                            //
                            if (manualUpdate)
                                CreateUpdateSql(columnName, DataBase.DB.TO_STRING(insertDoc));
                            else
                                //rs.updateString (colRs, insertDoc);					//	***
                                //
                                message = INFO + columnName + " -> " + insertDoc + " (DocNo)";
                            // Common.ErrorLog.FillErrorLog("SaveWithutPO","DocumnetNo.",message,VAdvantage.Framework.Message.MessageType.ERROR);
                            log.Fine(message);
                        }
                    }	//	New DocumentNo

                //  New Value(key)
                    else if (columnName.Equals("Value") && _inserting)
                    {
                        String value = drv[col].ToString();
                        //  Get from Sequence, if not entered
                        if (value == null || value.Length == 0)
                        {
                            /***************************************Check**************************************************/
                            value = MSequence.GetDocumentNo(AD_Client_ID, _tableName, null,_ctx);// null;// DataBase.getDocumentNo(_ctx, _windowNo, _tableName, false, null);
                            //  No Value
                            if (value == null || value.Length == 0)
                            {
                                error = true;
                                message = ERROR + field.GetColumnName() + "= " + drv[col]
                                     + " No Value";
                                //Common.ErrorLog.FillErrorLog("SaveWithutPO", "Value", message, VAdvantage.Framework.Message.MessageType.ERROR);
                                log.Fine(message);
                                break;
                            }
                        }
                        if (manualUpdate)
                            CreateUpdateSql(columnName, DataBase.DB.TO_STRING(value));
                        else
                            //rs.updateString (colRs, value); 							//	***
                            //
                            message = INFO + columnName + " -> " + value + " (Value)";
                        //Common.ErrorLog.FillErrorLog("SaveWithutPO", "Value", message, VAdvantage.Framework.Message.MessageType.ERROR);
                        log.Fine(message);
                    }	//	New Value(key)

                    else if (columnName.Equals("Updated"))
                    {
                        if (_compareDB && !_inserting && !Utility.Util.IsEqual(_rowData[col], rowDataDB[col]))	//	changed
                        {
                            error = true;
                            message = ERROR + field.GetColumnName() + "= " + _rowData[col]
                                 + " != DB: " + rowDataDB[col];
                            log.Fine(message);
                            //ErrorLog.FillErrorLog("GridTable", "Save direct", message, VAdvantage.Framework.Message.MessageType.ERROR);
                            break;
                        }
                        if (manualUpdate)
                        {
                            CreateUpdateSql(columnName, DataBase.GlobalVariable.TO_DATE(DateTime.Now, false));
                        }
                        else
                        {
                            //	rs.updateTimestamp (colRs, now); 							//	***
                        }
                        //
                        message = INFO + "Updated/By -> " + now + " - " + user;
                        log.Fine(message);
                        //ErrorLog.FillErrorLog("GridTable", "Save direct", message, VAdvantage.Framework.Message.MessageType.INFORMATION);
                    } //	Updated

                    //	UpdatedBy	- update
                    else if (columnName.Equals("UpdatedBy"))
                    {
                        if (manualUpdate)
                            CreateUpdateSql(columnName, user.ToString());
                        else
                        {
                            //	rs.updateInt (colRs, user); 								//	***
                        }
                    } //	UpdatedBy

            //	Created
                    else if (_inserting && columnName.Equals("Created"))
                    {
                        if (manualUpdate)
                            CreateUpdateSql(columnName, DataBase.GlobalVariable.TO_DATE(DateTime.Now, false));
                        else
                        {
                            //	rs.updateTimestamp (colRs, now); 							//	***
                        }
                    } //	Created

                    //	CreatedBy
                    else if (_inserting && columnName.Equals("CreatedBy"))
                    {
                        if (manualUpdate)
                            CreateUpdateSql(columnName, user.ToString());
                        else
                        {
                            //rs.updateInt (colRs, user); 								//	***
                        }
                    } //	CreatedBy

                    //	Nothing changed & null
                    else if ((_rowData[col] == null || _rowData[col].Equals(typeof(DBNull))) && (drv[col] == null || drv[col].Equals(typeof(DBNull))))
                    {
                        if (_inserting)
                        {
                            if (manualUpdate)
                            {
                                CreateUpdateSql(columnName, "NULL");
                            }
                            else
                            {
                                //	rs.updateNull (colRs); 								//	***
                            }
                            message = INFO + columnName + "= NULL";
                            log.Fine(message);
                            //ErrorLog.FillErrorLog("GridTable", "Direct save", message, VAdvantage.Framework.Message.MessageType.ERROR);
                        }
                    }

                        //	***	Data changed ***
                    else if (_inserting
                                    || !drv[col].Equals(_rowData[col])) 			//	changed
                    {
                        //	Original == DB
                        bool isClientOrgId = field.GetColumnName() == "AD_Client_ID" || field.GetColumnName() == "AD_Org_ID";
                        object columnValue = GetValueAccordingPO(_rowData[col], field.GetDisplayType(), isClientOrgId);
                        object colNewValue = GetValueAccordingPO(drv[col], field.GetDisplayType(), isClientOrgId);


                        if (_inserting || !_compareDB
                          || (columnValue == null || columnValue.Equals(rowDataDB[col])))
                        {
                            if (Logging.VLogMgt.IsLevelFinest())
                                log.Fine(columnName + "=" + _rowData[col]
                                    + " " + ((_rowData[col] == null || _rowData[col] == DBNull.Value) ? "" : _rowData[col].GetType().Name));
                            bool encrypted = field.IsEncryptedColumn();
                            //
                            String type = "String";
                            if (colNewValue == null)
                            {
                                if (manualUpdate)
                                    CreateUpdateSql(columnName, "NULL");
                                else
                                {
                                    //	rs.updateNull (colRs); 							//	***
                                }
                            }
                            else if (DisplayType.IsID(field.GetDisplayType())
                            || field.GetDisplayType() == DisplayType.Integer)
                            {
                                try
                                {
                                    Object dd = colNewValue;
                                    int val = 0;
                                    if (dd.GetType().Equals(typeof(int)))
                                        val = (int)dd;
                                    else
                                        val = Convert.ToInt32(dd.ToString());
                                    if (encrypted)
                                        val = (int)Encrypt(val);
                                    if (manualUpdate)
                                        CreateUpdateSql(columnName, val.ToString());
                                    else
                                    {
                                        //	rs.updateInt (colRs, iii.intValue()); 		// 	***
                                    }
                                }
                                catch  //  could also be a String (AD_Language, AD_Message)
                                {
                                    if (manualUpdate)
                                        CreateUpdateSql(columnName, DataBase.GlobalVariable.TO_STRING(colNewValue.ToString()));
                                    else
                                    {
                                        //	rs.updateString (colRs, rowData[col].toString ()); //	***
                                    }
                                }
                                type = "Int";
                            }

                            //	Numeric - BigDecimal
                            else if (DisplayType.IsNumeric(field.GetDisplayType()))
                            {
                                decimal bd = (decimal)colNewValue;
                                if (encrypted)
                                    bd = (Decimal)Encrypt(bd);
                                if (manualUpdate)
                                    CreateUpdateSql(columnName, bd.ToString());
                                else
                                {
                                    //	rs.updateBigDecimal (colRs, bd); 				//	***
                                }
                                type = "Number";
                            }
                            //	Date - Timestamp
                            else if (DisplayType.IsDate(field.GetDisplayType()))
                            {
                                DateTime ts = (DateTime)colNewValue;
                                if (encrypted)
                                    ts = (DateTime)Encrypt(ts);
                                if (manualUpdate)
                                    CreateUpdateSql(columnName, DataBase.GlobalVariable.TO_DATE(ts, false));
                                else
                                {
                                    //	rs.updateTimestamp (colRs, ts); 				//	***
                                }
                                type = "Date";
                            }

                            else if (field.GetDisplayType() == DisplayType.TextLong)
                            {
                                PO_LOB lob = new PO_LOB(_tableName, columnName, null,
                                                                field.GetDisplayType(), colNewValue);

                                LobAdd(lob);
                                type = "CLOB";
                            }

                            else if (field.GetDisplayType() == DisplayType.Binary
                                || field.GetDisplayType() == DisplayType.Image)
                            {
                                PO_LOB lob = new PO_LOB(_tableName, columnName, null,
                                                            field.GetDisplayType(), colNewValue);
                                LobAdd(lob);
                                type = "BLOB";
                            }

                            else if (field.GetDisplayType() == DisplayType.YesNo)
                            {
                                String yn = null;
                                if (colNewValue.GetType().Equals(typeof(bool)))
                                {
                                    Boolean bb = (Boolean)colNewValue;
                                    yn = bb ? "Y" : "N";
                                }
                                else
                                    yn = "Y".Equals(colNewValue) ? "Y" : "N";
                                if (encrypted)
                                {
                                 //   yn = (String)yn;
                                }
                                if (manualUpdate)
                                    CreateUpdateSql(columnName, DataBase.GlobalVariable.TO_STRING(yn));
                                else
                                {
                                    //	rs.updateString (colRs, yn); 					//	***
                                }
                            }
                            //	String and others
                            else
                            {
                                String str = colNewValue.ToString();
                                if (encrypted)
                                    str = (String)Encrypt(str);
                                if (manualUpdate)
                                    CreateUpdateSql(columnName, DataBase.GlobalVariable.TO_STRING(str));
                                else
                                {
                                    //	rs.updateString (colRs, str); 					//	***
                                }
                            }
                            //
                            message = INFO + columnName + "= " + ((_rowData[col].ToString() == "") ? "null" : _rowData[col].ToString()).ToString()
                                 + " -> " + drv[col] + " (" + type + ")";
                            if (encrypted)
                                message += " encrypted";
                            log.Fine(message);
                            //ErrorLog.FillErrorLog("GridTable", "Info", message, VAdvantage.Framework.Message.MessageType.INFORMATION);
                        }

                            //	Original != DB
                        else
                        {
                            error = true;
                            message = ERROR + field.GetColumnName() + "= " + _rowData[col]
                                    + " != DB: " + rowDataDB[col] + " -> " + drv[col];
                           // ErrorLog.FillErrorLog("GridTable", "", message, VAdvantage.Framework.Message.MessageType.INFORMATION);
                            //log.fine(is);
                            Object o1 = _rowData[col];
                            Object o2 = rowDataDB[col];
                            bool eq = o1.Equals(o2);
                            log.Fine((o1 == o2) + "  " + eq);
                            //ErrorLog.FillErrorLog("GridTable", "", (o1 == o2).ToString() + "  " + eq, VAdvantage.Framework.Message.MessageType.INFORMATION);
                        }
                    } //Data Changed

                    if (field.IsKey() && !_inserting)
                    {
                        if (drv[col] == null || drv[col].Equals(typeof(DBNull)))
                        {
                            throw new Exception("Key is NULL - " + columnName);
                        }
                        if (columnName.EndsWith("_ID"))
                            singleRowWHERE.Append(columnName).Append("=").Append(drv[col]);
                        else
                        {
                            singleRowWHERE = new StringBuilder();	//	overwrite
                            singleRowWHERE.Append(columnName).Append("=").Append(DataBase.GlobalVariable.TO_STRING(drv[col].ToString()));
                        }
                    }

                    //	MultiKey Inserting - retrieval sql
                    if (field.IsParentColumn())
                    {
                        if (drv[col] == null || drv[col].Equals(typeof(DBNull)))
                            throw new Exception("MultiKey Parent is NULL - " + columnName);
                        if (multiRowWHERE.Length != 0)
                            multiRowWHERE.Append(" AND ");
                        if (columnName.EndsWith("_ID"))
                            multiRowWHERE.Append(columnName).Append("=").Append(drv[col]);
                        else
                            multiRowWHERE.Append(columnName).Append("=").Append(DataBase.GlobalVariable.TO_STRING(drv[col].ToString()));
                    }
                    //
                    colRs++;

                }

                if (error)
                {
                    if (manualUpdate)
                        CreateUpdateSqlReset();
                    //else
                    //  rs.cancelRowUpdates();
                    //rs.close();
                    // ShowInfoMessage("SaveErrorDataChanged", "");
                    FireDataStatusEEvent("SaveErrorDataChanged", "", true);
                    //dataRefresh(m_rowChanged);
                    return SAVE_ERROR;
                }

                /**
			 *	Save to Database
			 */
                //
                System.Threading.Thread.CurrentThread.CurrentCulture = Utility.Env.GetLanguage(Utility.Env.GetContext()).GetCulture(Utility.Env.GetLoginLanguage(Utility.Env.GetContext()).GetAD_Language());
                System.Threading.Thread.CurrentThread.CurrentUICulture = Utility.Env.GetLanguage(Utility.Env.GetContext()).GetCulture(Utility.Env.GetLoginLanguage(Utility.Env.GetContext()).GetAD_Language());

                String whereClause = singleRowWHERE.ToString();
                if (whereClause.Length == 0)
                    whereClause = multiRowWHERE.ToString();
                if (_inserting)
                {
                    //log.fine("Inserting ...");
                    if (manualUpdate)
                    {
                        String sql = CreateUpdateSql(true, null);
                        int no = DataBase.DB.ExecuteQuery(sql, null, null);	//	no Trx
                        if (no != 1)
                        {
                            //ErrorLog.FillErrorLog("GridTable", sql, " ---Update #=" + no + " - ", VAdvantage.Framework.Message.MessageType.ERROR);
                            log.Log(Level.SEVERE, "Insert #=" + no + " - " + sql);
                        }
                    }
                    else
                    {
                        //	rs.insertRow();
                    }
                }
                else
                {
                    log.Fine("Updating ... " + whereClause);
                    if (manualUpdate)
                    {
                        String sql = CreateUpdateSql(false, whereClause);
                        int no = SqlExec.ExecuteQuery.ExecuteNonQuery(sql);	//	no Trx
                        if (no != 1)
                        {
                            //ErrorLog.FillErrorLog("GridTable", sql, "Update #=" + no + " - ", VAdvantage.Framework.Message.MessageType.ERROR);
                            log.Log(Level.SEVERE, "Update #=" + no + " - " + sql);
                        }
                    }
                    else
                    {
                        //rs.updateRow();
                    }
                }
                LobSave(whereClause);

                //	Need to re-read row to get ROWID, Key, DocumentNo, Trigger, virtual columns
                log.Fine("Reading ... " + whereClause);
                StringBuilder refreshSQL = new StringBuilder(_SQL_Select)
                    .Append(" WHERE ").Append(whereClause);
                drRef = DataBase.DB.ExecuteReader(refreshSQL.ToString());

                if (((System.Data.Common.DbDataReader)drRef).HasRows)
                {
                    while (drRef.Read())
                    {
                        rowDataDB = ReadData(drRef);
                        RefreshData(drv, rowDataDB);
                        //	update buffer
                        break;
                    }
                }
                else
                {
                    //ErrorLog.FillErrorLog("GridTable", refreshSQL.ToString(), "Error", VAdvantage.Framework.Message.MessageType.ERROR);
                    log.Log(Level.SEVERE, "Inserted row not found");
                }
                drRef.Close();
                drRef = null;
            }
            catch (System.Data.Common.DbException e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                if (drRef != null)
                {
                    drRef.Close();
                    drRef = null;
                }


                String msg = "SaveError";
                if (e.ErrorCode == 1)		//	Unique Constraint
                {
                    log.Log(Level.SEVERE, "Key Not Unique", e);
                    msg = "SaveErrorNotUnique";
                }
                else
                    log.Log(Level.SEVERE, select.ToString(), e);

                FireDataStatusEEvent(msg, e.Message, true);
                return SAVE_ERROR;
            }
            catch
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                if (drRef != null)
                {
                    drRef.Close();
                    drRef = null;
                }
            }

            _rowData = null;
            _changed = false;
            _compareDB = true;
            _rowChanged = -1;
            _newRow = -1;
            _inserting = false;

            FireDataStatusIEvent("Saved", "");
            //
            log.Info("fini");
            return SAVE_OK;
        }

        public void SetChanged()
        {
            _changed = true;
            FireDataStatusIEvent("", "");
        }

        /// <summary>
        /// Ignore changes
        /// </summary>
        public void DataIgnore()
        {
            if (!_inserting && !_changed)//&& m_rowChanged < 0)
            {
                log.Fine("Nothing to ignore");
                return;
            }
            log.Info("Inserting=" + _inserting);

            if (_inserting)
            {
                //	Get Sort
                _changed = false;
                _rowChanged = -1;
                _rowData = null;
                _inserting = false;
            }
            else
            {
                _changed = false;
                _rowChanged = -1;
                _rowData = null;
                _inserting = false;
            }
            FireDataStatusIEvent("Ignored", "");
        }


        /// <summary>
        /// read data from DataReader
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private void ReadData(DataRow rowData, DataRow dr)
        {
            int size = m_fields.Count;
            //Object[] rowData = new Object[size];
            String columnName = null;
            int displayType = 0;

            //	Types see also MField.createDefault
            try
            {
                //	get row data
                for (int j = 0; j < size; j++)
                {

                    //	NULL
                    if (dr.IsNull(j))
                    {
                        rowData[j] = dr[j];
                    }
                    else
                    {
                        //	Column Info
                        GridField field = (GridField)m_fields[j];
                        columnName = field.GetColumnName();
                        displayType = field.GetDisplayType();
                        //	Integer, ID, Lookup (UpdatedBy is a numeric column)
                        if ((DisplayType.IsID(displayType) // JJ: don't touch!
                                && (columnName.EndsWith("_ID") || columnName.EndsWith("_Acct") || columnName.EndsWith("_ID_1") || columnName.EndsWith("_ID_2") || columnName.EndsWith("_ID_3")))
                            || columnName.EndsWith("atedBy") || displayType == DisplayType.Integer)
                        {
                            rowData[j] = Utility.Util.GetValueOfInt(dr[j].ToString());	//	Integer
                        }
                        //	Number
                        else if (DisplayType.IsNumeric(displayType))
                        {
                            rowData[j] = Utility.Util.GetValueOfDecimal(dr[j].ToString());			//	BigDecimal
                        }
                        //	Date
                        else if (DisplayType.IsDate(displayType))
                        {
                            DateTime time = new DateTime();
                            DateTime.TryParse(dr[j].ToString(), out time);
                            rowData[j] = time;
                        }
                        //	RowID or Key (and Selection)
                        else if (displayType == DisplayType.RowID)
                            rowData[j] = null;
                        //	YesNo
                        else if (displayType == DisplayType.YesNo)
                        {
                            String str = dr[j].ToString();
                            if (field.IsEncryptedColumn())
                                str = (String)Decrypt(str);
                            rowData[j] = "True".Equals(str);	//	Boolean
                        }
                        //	LOB
                        else if (DisplayType.IsLOB(displayType))
                        {
                            Object value = dr[j];
                            if (dr[j] == null)
                                rowData[j] = null;
                            //else if (value instanceof Clob) 
                            //{
                            //    Clob lob = (Clob)value;
                            //    long length = lob.length();
                            //    rowData[j] = lob.getSubString(1, (int)length);
                            //}
                            //else if (value instanceof Blob)
                            //{
                            //    Blob lob = (Blob)value;
                            //    long length = lob.length();
                            //    rowData[j] = lob.getBytes(1, (int)length);
                            //}
                            //// For EnterpriseDB (Vienna Type Long Text is stored as Text in EDB)
                            //else if (value instanceof java.lang.String) {
                            //    rowData[j] = value.toString();
                            //}
                            if (value.GetType().Equals(typeof(string)))
                            {
                                rowData[j] = value.ToString();
                            }
                            else
                            {
                                rowData[j] = value;
                            }
                        }
                        //	String
                        else
                            rowData[j] = dr[j].ToString();//string
                        //	Encrypted
                        if (field.IsEncryptedColumn() && displayType != DisplayType.YesNo)
                            rowData[j] = Decrypt(rowData[j]);
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, columnName + ", DT=" + displayType, e);
                //ErrorLog.FillErrorLog("GridTable", "columnName  DT=" + displayType, e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
            }
            //return rowData;

        }



        /// <summary>
        /// read data from DataReader
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private object[] ReadData(IDataReader dr)
        {
            int size = m_fields.Count;
            Object[] rowData = new Object[size];
            String columnName = null;
            int displayType = 0;

            //	Types see also MField.createDefault
            try
            {
                //	get row data
                for (int j = 0; j < size; j++)
                {

                    //	NULL
                    if (dr.IsDBNull(j))
                    {
                        rowData[j] = null;
                    }
                    else
                    {
                        //	Column Info
                        GridField field = (GridField)m_fields[j];
                        columnName = field.GetColumnName();
                        displayType = field.GetDisplayType();
                        //	Integer, ID, Lookup (UpdatedBy is a numeric column)
                        if ((DisplayType.IsID(displayType) // JJ: don't touch!
                                && (columnName.EndsWith("_ID") || columnName.EndsWith("_Acct") || columnName.EndsWith("_ID_1") || columnName.EndsWith("_ID_2") || columnName.EndsWith("_ID_3")))
                            || columnName.EndsWith("atedBy") || displayType == DisplayType.Integer)
                        {
                            rowData[j] = Utility.Util.GetValueOfInt(dr[j].ToString());	//	Integer
                        }
                        //	Number
                        else if (DisplayType.IsNumeric(displayType))
                        {
                            rowData[j] = Utility.Util.GetValueOfDecimal(dr[j].ToString());			//	BigDecimal
                        }
                        //	Date
                        else if (DisplayType.IsDate(displayType))
                        {
                            DateTime time = new DateTime();
                            DateTime.TryParse(dr[j].ToString(), out time);
                            rowData[j] = time;
                        }
                        //	RowID or Key (and Selection)
                        else if (displayType == DisplayType.RowID)
                            rowData[j] = null;
                        //	YesNo
                        else if (displayType == DisplayType.YesNo)
                        {
                            String str = dr[j].ToString();
                            if (field.IsEncryptedColumn())
                                str = (String)Decrypt(str);
                            rowData[j] = "True".Equals(str);	//	Boolean
                        }
                        //	LOB
                        else if (DisplayType.IsLOB(displayType))
                        {
                            Object value = dr[j];
                            if (dr[j] == null)
                                rowData[j] = null;
                            //else if (value instanceof Clob) 
                            //{
                            //    Clob lob = (Clob)value;
                            //    long length = lob.length();
                            //    rowData[j] = lob.getSubString(1, (int)length);
                            //}
                            //else if (value instanceof Blob)
                            //{
                            //    Blob lob = (Blob)value;
                            //    long length = lob.length();
                            //    rowData[j] = lob.getBytes(1, (int)length);
                            //}
                            //// For EnterpriseDB (Vienna Type Long Text is stored as Text in EDB)
                            //else if (value instanceof java.lang.String) {
                            //    rowData[j] = value.toString();
                            //}
                            if (value.GetType().Equals(typeof(string)))
                            {
                                rowData[j] = value.ToString();
                            }
                            else
                            {
                                rowData[j] = value;
                            }
                        }
                        //	String
                        else
                            rowData[j] = dr[j].ToString();//string
                        //	Encrypted
                        if (field.IsEncryptedColumn() && displayType != DisplayType.YesNo)
                            rowData[j] = Decrypt(rowData[j]);
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, columnName + ", DT=" + displayType, e);
                //ErrorLog.FillErrorLog("GridTable", "columnName  DT=" + displayType, e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
            }
            return rowData;

        }

        /// <summary>
        ///Prepare LOB save
        /// </summary>
        /// <param name="lob"></param>
        private void LobAdd(PO_LOB lob)
        {
            //		log.fine("LOB=" + lob);
            if (_lobInfo == null)
                _lobInfo = new List<PO_LOB>();
            _lobInfo.Add(lob);
        }	//	lobAdd

        /// <summary>
        ///Save LOB
        /// </summary>
        /// <param name="whereClause"></param>
        private void LobSave(String whereClause)
        {
            if (_lobInfo == null)
                return;
            for (int i = 0; i < _lobInfo.Count; i++)
            {
                PO_LOB lob = (PO_LOB)_lobInfo[i];
                lob.Save(whereClause, null);		//	no trx
            }	//	for all LOBs
            LobReset();
        }	//	

        /// <summary>
        ///	Get Client Org for row
        /// </summary>
        /// <param name="row"></param>
        /// <returns>array [0] = Client [1] = Org - a value of -1 is not defined/found</returns>
        private int[] GetClientOrg(DataRowView row)
        {
            int AD_Client_ID = -1;
            if (_indexClientColumn != -1)
            {
                object ii = row[_indexClientColumn];
                if (ii != null && ii != DBNull.Value)
                    AD_Client_ID = Convert.ToInt32(ii);
            }
            int AD_Org_ID = 0;
            if (_indexOrgColumn != -1)
            {
                object ii = row[_indexOrgColumn];
                if (ii != null && ii != DBNull.Value)
                    AD_Org_ID = Convert.ToInt32(ii);
            }
            return new int[] { AD_Client_ID, AD_Org_ID };
        }	//	getClientOrg

        /*************************************************************************/

        private List<String> _createSqlColumn = new List<String>();
        private List<String> _createSqlValue = new List<String>();

        /// <summary>
        ///	Prepare SQL creation
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        private void CreateUpdateSql(String columnName, String value)
        {
            _createSqlColumn.Add(columnName);
            _createSqlValue.Add(value);
            log.Finest("#" + _createSqlColumn.Count.ToString()
                + " - " + columnName + "=" + value);
            //ErrorLog.FillErrorLog("Tableobj", "size Info", _createSqlColumn.Count.ToString() + " - " + columnName + "=" + value, VAdvantage.Framework.Message.MessageType.INFORMATION);
        }

        /// <summary>
        ///Create update/insert SQL
        /// </summary>
        /// <param name="insert">true if insert - update otherwise</param>
        /// <param name="whereClause">where clause for update</param>
        /// <returns></returns>
        private String CreateUpdateSql(bool insert, String whereClause)
        {
            StringBuilder sb = new StringBuilder();
            if (insert)
            {
                sb.Append("INSERT INTO ").Append(_tableName).Append(" (");
                for (int i = 0; i < _createSqlColumn.Count; i++)
                {
                    if (i != 0)
                        sb.Append(",");
                    sb.Append(_createSqlColumn[i]);
                }
                sb.Append(") VALUES ( ");
                for (int i = 0; i < _createSqlValue.Count; i++)
                {
                    if (i != 0)
                        sb.Append(",");
                    sb.Append(_createSqlValue[i]);
                }
                sb.Append(")");
            }
            else
            {
                sb.Append("UPDATE ").Append(_tableName).Append(" SET ");
                for (int i = 0; i < _createSqlColumn.Count; i++)
                {
                    if (i != 0)
                        sb.Append(",");
                    sb.Append(_createSqlColumn[i]).Append("=").Append(_createSqlValue[i]);
                }
                sb.Append(" WHERE ").Append(whereClause);
            }
            log.Fine(sb.ToString());
            //ErrorLog.FillErrorLog("GridTable", "Info", sb.ToString(), VAdvantage.Framework.Message.MessageType.INFORMATION);
            //	reset
            CreateUpdateSqlReset();
            return sb.ToString();
        }

        /// <summary>
        ///Reset Update -Insert generic lists
        /// </summary>
        private void CreateUpdateSqlReset()
        {
            _createSqlColumn = new List<String>();
            _createSqlValue = new List<String>();
        }	//	createUpdateSqlReset

        /// <summary>
        ///Get Mandatory empty columns
        /// </summary>
        /// <param name="rowData"></param>
        /// <returns>String with missing column headers/labels</returns>
        private String GetMandatory(DataRowView rowData)
        {
            //  see also => ProcessParameter.saveParameter
            StringBuilder sb = new StringBuilder();

            //	Check all columns
            int size = m_fields.Count();
            for (int i = 0; i < size; i++)
            {
                GridField field = m_fields[i];
                if (field.IsMandatory(true))        //  check context
                {
                    if (rowData[i] == null || rowData[i] == DBNull.Value || rowData[i].ToString().Length == 0)
                    {
                        field.SetInserting(true);  //  set editable otherwise deadlock
                        field.SetError(true);
                        if (sb.Length > 0)
                            sb.Append(", ");
                        sb.Append(field.GetHeader());
                    }
                }
            }

            if (sb.Length == 0)
                return "";
            return sb.ToString();
        }	//	getMandatory

        /// <summary>
        ///Get columns in Error status
        /// </summary>
        /// <returns>String with missing column headers/labels</returns>
        private String GetErrorColumns()
        {
            //  see also => ProcessParameter.saveParameter
            StringBuilder sb = new StringBuilder();

            //	Check all columns
            int size = m_fields.Count;
            for (int i = 0; i < size; i++)
            {
                GridField field = m_fields[i];
                if (field.IsError())        //  check context
                {
                    if (sb.Length > 0)
                        sb.Append(", ");
                    sb.Append(field.GetHeader());
                }
            }

            if (sb.Length == 0)
                return "";
            return sb.ToString();
        }	//	getErrorColumns

        /// <summary>
        /// Show Message
        /// </summary>
        /// <param name="key"></param>
        /// <param name="additionalText"></param>
        private void ShowInfoMessage(string key, string additionalText)
        {
           // ShowMessage.Error(key, null, additionalText);
        }

        /// <summary>
        /// Save Data by Model Class
        /// </summary>
        /// <param name="Record_ID"></param>
        /// <param name="drv"></param>
        /// <returns></returns>
        private char DataSavePO(int Record_ID, DataRowView rowData)
        {
            log.Fine("ID=" + Record_ID);

            MTable table = MTable.Get(_ctx, _AD_Table_ID);
            PO po = null;
            if (table.IsSingleKey() || Record_ID == 0)
            {
                po = table.GetPO(_ctx, Record_ID, null);
            }
            else	//	Multi - Key
            {
                po = table.GetPO(_ctx, GetWhereClause(rowData), null);
            }
            //	No Persistent Object
            if (po == null)
            {
                throw new NullReferenceException("No Persistent Obj");
            }

            int size = m_fields.Count;
            for (int col = 0; col < size; col++)
            {
                GridField field = m_fields[col];
                if (field.IsVirtualColumn())
                    continue;
                String columnName = field.GetColumnName();
                bool isClientOrgId = columnName == "AD_Client_ID" || columnName == "AD_Org_ID";

                Object value = GetValueAccordingPO(rowData[col], field.GetDisplayType(), isClientOrgId);
                Object oldValue = GetValueAccordingPO(_rowData[col], field.GetDisplayType(), isClientOrgId);
                //	RowID
                if (field.GetDisplayType() == DisplayType.RowID)
                {
                    ;   //ignore
                }

                //	Nothing changed & null
                else if (oldValue == null && value == null)
                {
                    ;	//	ignore
                }
                /*data changed*/
                else if (_inserting || !Utility.Util.IsEqual(value, oldValue))
                {
                    //	Check existence
                    int poIndex = po.Get_ColumnIndex(columnName);
                    if (poIndex < 0)
                    {
                        //	Custom Fields not in PO
                        po.Set_CustomColumn(columnName, value);
                        continue;
                    }

                    Object dbValue = po.Get_Value(poIndex);

                    if (_inserting
                        || !_compareDB
                        //	Original == DB
                        || oldValue == dbValue
                        || Utility.Util.IsEqual(oldValue, dbValue)
                        //	Target == DB (changed by trigger to new value already)
                        || Utility.Util.IsEqual(value, dbValue))
                    {
                        po.Set_ValueNoCheck(columnName, value);
                    }
                    //	Original != DB
                    else
                    {
                        String msg = columnName
                            + "= " + oldValue
                                + (oldValue == null ? "" : "(" + oldValue.GetType().FullName + ")")
                            + " != DB: " + dbValue
                                + (dbValue == null ? "" : "(" + dbValue.GetType().FullName + ")")
                            + " -> New: " + value
                                + (value == null ? "" : "(" + value.GetType().FullName + ")");
                        //	CLogMgt.setLevel(Level.FINEST);
                        FireDataStatusEEvent("SaveErrorDataChanged", msg, true);
                        //ShowInfoMessage("SaveErrorDataChanged", msg);
                        //dataRefresh(m_rowChanged);
                        return SAVE_ERROR;
                    }
                }
            }

            if (!po.Save())
            {
                String msg = "SaveError";
                String info = "";
                ValueNamePair ppE = VLogger.RetrieveError();
                if (ppE != null)
                {
                    msg = ppE.GetValue();
                    info = ppE.GetName();
                    //	Unique Constraint
                    Exception ex = VLogger.RetrieveException();
                    if (ex != null
                        && ex is System.Data.Common.DbException
                        && ((System.Data.Common.DbException)ex).ErrorCode == -2146232008)
                        msg = "SaveErrorNotUnique";
                }
                //ShowInfoMessage("", msg);
                FireDataStatusEEvent(msg, info, true);
                return SAVE_ERROR;
            }

            //	Refresh - update buffer
            String whereClause = po.Get_WhereClause(true);
            log.Fine("Reading...." + whereClause);
            //ErrorLog.FillErrorLog("Table Object", whereClause, "information", VAdvantage.Framework.Message.MessageType.INFORMATION);

            StringBuilder refreshSQL = new StringBuilder(_SQL_Select)
            .Append(" WHERE ").Append(whereClause);
            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(refreshSQL.ToString());
                while (dr.Read())
                {
                    object[] drRow = ReadData(dr);
                    RefreshData(rowData, drRow);
                    break;
                }

            }
            catch (Exception e)
            {
                String msg = "SaveError";
                log.Log(Level.SEVERE, refreshSQL.ToString(), e);
                //ErrorLog.FillErrorLog("DataSavePo", refreshSQL.ToString(), e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
                FireDataStatusEEvent(msg, e.Message, true);
                return SAVE_ERROR;

            }
            finally { try { dr.Close(); dr = null; } catch { } }

            //	everything ok
            _rowData = null;
            _changed = false;
            _compareDB = true;
            _rowChanged = -1;
            _newRow = -1;
            _inserting = false;

            ValueNamePair pp = VLogger.RetrieveWarning();
            if (pp != null)
            {
                String msg = pp.GetValue();
                String info = pp.GetName();
                FireDataStatusEEvent(msg, info, false);
            }
            else
            {
                pp = VLogger.RetrieveInfo();
                String msg = "Saved";
                String info = "";
                if (pp != null)
                {
                    msg = pp.GetValue();
                    info = pp.GetName();
                }
                FireDataStatusIEvent("Record Saved", "");
            }
            //finish
            log.Config("fini");
            return SAVE_OK;
        }

        /// <summary>
        /// Return DataType According PO Class DataType
        /// </summary>
        /// <param name="value">Object</param>
        /// <returns>Object Value</returns>
        public Object GetValueAccordingPO(object value, int displayType, bool isOrgClientId)
        {
            if (value == null || value == DBNull.Value || "".Equals(value.ToString().Trim()))
            {
                return null;
            }

            if (DisplayType.IsLookup(displayType) || DisplayType.IsID(displayType)) // In Case Of Combobox Value
            {
                if (value.ToString() == "-1")
                {
                    return null;
                }
            }

            //else if (DisplayType.IsID(displayType) && (value.ToString() == "0" || value.ToString() == "-1"))
            //{
            //    return null;
            //}

            //else if (value is decimal)
            //{
            //    if (!DisplayType.IsNumeric(displayType) || displayType == DisplayType.Integer)
            //    {
            //        return Convert.ToInt32(value.ToString());
            //    }
            //    else
            //    {
            //        return Convert.ToDecimal(value.ToString());
            //    }
            //}

            else if (value.ToString() == "True" || value.ToString() == "False")
            {
                return Convert.ToBoolean(value.ToString());
            }

            //else if (value is DBNull || "".Equals(value.ToString()))
            //{
            //    return null;
            //}

            return value;
        }

        private void RefreshData(DataRowView rowData, Object[] dr)
        {
            int size = m_fields.Count;
            rowData.BeginEdit();
            for (int i = 0; i < size; i++)
            {
                string colName = m_fields[i].GetColumnName();
                rowData[i] = dr.GetValue(i);// [colName];

            }
            rowData.EndEdit();
        }


        private void RefreshData(DataRowView rowData, IDataReader dr)
        {
            int size = m_fields.Count;
            rowData.BeginEdit();
            for (int i = 0; i < size; i++)
            {
                string colName = m_fields[i].GetColumnName();
                rowData[colName] = dr[colName];

            }
            rowData.EndEdit();
        }

        /// <summary>
        /// Get Record Where Clause from data (single key or multi-parent)
        /// </summary>
        /// <param name="rowData"></param>
        /// <returns></returns>
        private String GetWhereClause(DataRowView rowData)
        {
            int size = m_fields.Count;
            StringBuilder singleRowWHERE = null;
            StringBuilder multiRowWHERE = null;
            for (int col = 0; col < size; col++)
            {
                GridField field = m_fields[col];
                if (field.IsKey())
                {
                    String columnName = field.GetColumnName();
                    Object value = rowData[col];
                    if (value == null || value == DBNull.Value || value.ToString().Trim() == "")
                    {
                        log.Log(Level.WARNING, "PK data is null - " + columnName);
                        return null;
                    }
                    if (columnName.EndsWith("_ID"))
                        singleRowWHERE = new StringBuilder(columnName)
                            .Append("=").Append(value);
                    else
                        singleRowWHERE = new StringBuilder(columnName)
                            .Append("=").Append(DataBase.GlobalVariable.TO_STRING(value.ToString()));
                }
                else if (field.IsParentColumn())
                {
                    String columnName = field.GetColumnName();
                    Object value = rowData[col];
                    if (value == null || value == DBNull.Value || value.ToString().Trim() == "")
                    {
                        log.Log(Level.INFO, "FK data is null - " + columnName);
                        continue;
                    }
                    if (multiRowWHERE == null)
                        multiRowWHERE = new StringBuilder();
                    else
                        multiRowWHERE.Append(" AND ");
                    if (columnName.EndsWith("_ID"))
                        multiRowWHERE.Append(columnName)
                            .Append("=").Append(value);
                    else
                        multiRowWHERE.Append(columnName)
                            .Append("=").Append(DataBase.GlobalVariable.TO_STRING(value.ToString()));
                }
            }	//	for all columns
            if (singleRowWHERE != null)
                return singleRowWHERE.ToString();
            if (multiRowWHERE != null)
                return multiRowWHERE.ToString();
            log.Log(Level.WARNING, "No key Found");
            return null;
        }

        /// <summary>
        /// Get Record Where Clause from data (single key or multi-parent)
        /// </summary>
        /// <param name="rowData"></param>
        /// <returns></returns>
        private String GetWhereClause(DataRow rowData)
        {
            int size = m_fields.Count;
            StringBuilder singleRowWHERE = null;
            StringBuilder multiRowWHERE = null;
            for (int col = 0; col < size; col++)
            {
                GridField field = m_fields[col];
                if (field.IsKey())
                {
                    String columnName = field.GetColumnName();
                    Object value = rowData[col];
                    if (value == null)
                    {
                        //log.log(Level.WARNING, "PK data is null - " + columnName);
                        return null;
                    }
                    if (columnName.EndsWith("_ID"))
                        singleRowWHERE = new StringBuilder(columnName)
                            .Append("=").Append(value);
                    else
                        singleRowWHERE = new StringBuilder(columnName)
                            .Append("=").Append(DataBase.GlobalVariable.TO_STRING(value.ToString()));
                }
                else if (field.IsParentColumn())
                {
                    String columnName = field.GetColumnName();
                    Object value = rowData[col];
                    if (value == null)
                    {
                        //    log.log(Level.INFO, "FK data is null - " + columnName);
                        continue;
                    }
                    if (multiRowWHERE == null)
                        multiRowWHERE = new StringBuilder();
                    else
                        multiRowWHERE.Append(" AND ");
                    if (columnName.EndsWith("_ID"))
                        multiRowWHERE.Append(columnName)
                            .Append("=").Append(value);
                    else
                        multiRowWHERE.Append(columnName)
                            .Append("=").Append(DataBase.GlobalVariable.TO_STRING(value.ToString()));
                }
            }	//	for all columns
            if (singleRowWHERE != null)
                return singleRowWHERE.ToString();
            if (multiRowWHERE != null)
                return multiRowWHERE.ToString();
            //log.log(Level.WARNING, "No key Found");
            return null;
        }

        /// <summary>
        /// Get Key ID or -1 of none
        /// </summary>
        /// <param name="row">row</param>
        /// <returns>ID or -1</returns>
        public int GetKeyID(int row)
        {
            //	Log.info("MTable.GetKeyID - row=" + row + ", keyColIdx=" + m_indexKeyColumn);
            if (_indexKeyColumn != -1)
            {

                if (_rowData != null)
                {
                    try
                    {
                        int ii = Utility.Util.GetValueOfInt(_rowData[_indexKeyColumn].ToString());

                        if (ii == 0)
                            return -1;
                        return ii;
                    }
                    catch
                    {
                        //Common.ErrorLog.FillErrorLog("GridTable.Delete()", "Could not get Key Id", ex.Message, VAdvantage.Framework.Message.MessageType.ERROR);
                        return -1;
                    }

                }

                try
                {
                    int ii = Utility.Util.GetValueOfInt(GetValueAt(row, _indexKeyColumn).ToString());
                    if (ii == 0)
                        return -1;
                    return ii;
                }
                catch      //  Alpha Key
                {
                    //Common.ErrorLog.FillErrorLog("GridTable.Delete()", "Could not get Key Id", ex.Message, VAdvantage.Framework.Message.MessageType.ERROR);
                    return -1;
                }
            }
            return -1;
        }

        /// <summary>
        /// Get Value in Resultset
        /// </summary>
        /// <param name="row">row</param>
        /// <param name="col">column</param>
        /// <returns>Object of that row/column</returns>
        public object GetValueAt(int row, int col)
        {
            //	log.config( "MTable.getValueAt r=" + row + " c=" + col);
            if (!_open || row < 0 || col < 0 || row >= _bufferTable.Rows.Count)// _buffer.Tables[0].Rows.Count)
            {
                //	log.fine( "Out of bounds - Open=" + m_open + ", RowCount=" + m_rowCount);
                return null;
            }

            //	need to wait for data read into buffer
            int loops = 0;
            //while (row >= _buffer.Tables[0].Rows.Count && loader.IsAlive && loops < 15)
            while (row >= _bufferTable.Rows.Count && loader.IsAlive && loops < 15)
            {
                //log.fine("Waiting for loader row=" + row + ", size=" + m_buffer.size());
                try
                {
                    Thread.Sleep(500);		//	1/2 second
                }
                catch
                { }
                loops++;
            }

            //	empty buffer
            if (row >= _bufferTable.Rows.Count)
            {
                //	log.fine( "Empty buffer");
                return null;
            }

            //	return Data item
            //MSort sort = (MSort)_sort.get(row);
            //Object[] rowData = (Object[])_buffer.Tables[0].Rows[sort.index];
            object rowData = (object)_bufferTable.Rows[row][col];
            //	out of bounds
            if (rowData == null)
            {
                //	log.fine( "No data or Column out of bounds");
                return null;
            }
            return rowData;
        }

        /**	LOB Info				*/
        private List<PO_LOB> _lobInfo = null;

        /// <summary>
        /// Reset Lob Info
        /// </summary>
        private void LobReset()
        {
            _lobInfo = null;
        }	//	resetLOB

        /// <summary>
        /// Add new row 
        /// </summary>
        /// <param name="copy"></param>
        /// <param name="drv"></param>
        /// <returns></returns>
        public bool DataNew(bool copy, DataRowView drv, DataRowView oldRow)
        {
            //  Read only
            log.Info("Current= row  , Copy=" + copy);
            if (_readOnly)
            {
                FireDataStatusEEvent("AccessCannotInsert", "", true);
                //ShowInfoMessage("AccessCannotInsert", "");
                return false;
            }

            _inserting = true;
            //	Create default data
            int size = m_fields.Count;
            // _rowData = new Object[size];	//	"original" data
            //Object[] rowData = new Object[size];
            //	fill data
            if (copy) //copy
            {
                if (oldRow == null)
                {
                    return false;
                }
                for (int i = 0; i < size; i++)
                {
                    GridField field = m_fields[i];
                    String columnName = field.GetColumnName();
                    if (field.IsVirtualColumn())
                    { continue; }
                    else if (field.IsKey()
                        || columnName.Equals("AD_Client_ID")
                        //
                        || columnName.StartsWith("Created") || columnName.StartsWith("Updated")
                        || columnName.Equals("EntityType") || columnName.Equals("DocumentNo")
                        || columnName.Equals("Processed") || columnName.Equals("IsSelfService")
                        || columnName.Equals("DocAction") || columnName.Equals("DocStatus")
                        || columnName.StartsWith("Ref_")
                        || columnName.Equals("Posted")
                        //	Order/Invoice
                        || columnName.Equals("GrandTotal") || columnName.Equals("TotalLines")
                        || columnName.Equals("C_CashLine_ID") || columnName.Equals("C_Payment_ID")
                        || columnName.Equals("IsPaid") || columnName.Equals("IsAllocated")
                    )
                    {
                        drv[i] = field.GetDefault();
                        field.SetValue(drv[i]);
                    }
                    else
                    {
                        drv[i] = oldRow[i];
                        field.SetValue(drv[i]);
                    }
                }
            }
            else	//	new
            {
                if (drv != null)
                {
                    for (int i = 0; i < size; i++)
                    {
                        GridField field = m_fields[i];
                        drv[i] = field.GetDefault();
                        field.SetValue(drv[i]);
                        //drv.EndEdit();
                    }
                }
                else
                {
                    log.Warning("currentDataRowView is null");
                    //Common.ErrorLog.FillErrorLog("GetDefault", "", "datarow is null", VAdvantage.Framework.Message.MessageType.ERROR);
                }
            }
            _changed = true;
            _compareDB = true;
            _rowChanged = -1;  //  only changed in setValueAt
            //_newRow = currentRow + 1;
            FireDataStatusIEvent(copy ? "UpdateCopied" : "Inserted", "");
            log.Fine("Current= DatarowView  ,  New=" + _newRow + " - complete");
            return true;

        }

        /// <summary>
        ///  Delete Data
        ///  Error info (Access*, AccessNotDeleteable, DeleteErrorDependent,
        ///  DeleteError) is saved in the log
        /// </summary>
        /// <param name="row"></param>
        /// <returns>true if success</returns>
        public bool DataDelete(int row)
        {
            if (row < 0)
                return false;

            //	Tab R/O
            if (_readOnly)
            {
                //ShowInfoMessage("AccessCannotDelete", "");	//	previleges
                FireDataStatusEEvent("AccessCannotDelete", "", true);	//	previleges
                return false;
            }
            //	Is this record deletable?
            if (!_deleteable)
            {
                //ShowInfoMessage("AccessNotDeleteable", "");	//	previleges
                FireDataStatusEEvent("AccessNotDeleteable", "", true);	//	audit
                return false;
            }
            //	Processed Column and not an Import Table
            if (_indexProcessedColumn > 0 && !_tableName.StartsWith("I_"))
            {
                bool processed = false;
                bool.TryParse(_rowData[_indexProcessedColumn].ToString(), out processed);//  GetValueAt(row, _indexProcessedColumn).ToString(),out processed);
                if (processed)
                {
                    //ShowInfoMessage("CannotDeleteTrx", "");
                    FireDataStatusEEvent("CannotDeleteTrx", "", true);
                    return false;
                }
            }


            DataRow rowData = _bufferTable.Rows[row];// _buffer.Tables[0].Rows[row];
            //
            MTable table = MTable.Get(_ctx, _AD_Table_ID);
            PO po = null;
            int recordId = GetKeyID(row);
            if (recordId != -1)
                po = table.GetPO(_ctx, recordId, null);
            else	//	Multi - Key
                po = table.GetPO(_ctx, GetWhereClause(rowData), null);
            //	Delete via PO 
            if (po != null)
            {
                bool ok = false;
                try
                {
                    ok = po.Delete(false);
                }
                catch (Exception ex)
                {
                    log.Log(Level.SEVERE, "Delete", ex);
                    //ErrorLog.FillErrorLog("GridTable", "Delete by PO", ex.Message, VAdvantage.Framework.Message.MessageType.ERROR);
                }
                if (!ok)
                {
                    //Get errro from Log
                    ValueNamePair vp = VLogger.RetrieveError();// new ValueNamePair("error occurred during delete process", "get error from logger class");// CLogger.retrieveError();
                    if (vp != null)
                        FireDataStatusEEvent(vp);
                    else
                    {
                        FireDataStatusEEvent("DeleteError", "", true);
                        //ShowInfoMessage("DeleteError", "");
                    }
                    return false;
                }
            }
            else //Delete By Sql
            {
                StringBuilder sql = new StringBuilder("DELETE FROM ");
                sql.Append(_tableName).Append(" WHERE ").Append(GetWhereClause(rowData));
                int no = 0;
                try
                {
                    no = SqlExec.ExecuteQuery.ExecuteNonQuery(sql.ToString());
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    //ErrorLog.FillErrorLog("GridTable", sql.ToString(), e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
                    log.Log(Level.SEVERE, sql.ToString(), e);
                    String msg = "DeleteError";
                    if (e.ErrorCode == 2292)	//	Child Record Found
                        msg = "DeleteErrorDependent";
                    FireDataStatusEEvent(msg, e.InnerException.Message, true);
                    return false;
                }
                if (no != 1)
                {
                    log.Log(Level.SEVERE, "Number of deleted rows = " + no);
                    //ErrorLog.FillErrorLog("GridTable.Delete()", sql.ToString(), "Number of deleted rows = " + no, VAdvantage.Framework.Message.MessageType.INFORMATION);
                    return false;
                }
            }
            //	inform
            _changed = false;
            _rowChanged = -1;
            FireDataStatusIEvent("Deleted", "");
            log.Fine("Row=" + row + " complete");
            return true;
        }

        /// <summary>
        ///  Delete Data
        ///  Error info (Access*, AccessNotDeleteable, DeleteErrorDependent,
        ///  DeleteError) is saved in the log
        /// </summary>
        /// <param name="row"></param>
        /// <returns>true if success</returns>
        public bool DataDelete(DataRowView rowData)
        {
            if (rowData == null)
                return false;

            //	Tab R/O
            if (_readOnly)
            {
                //ShowInfoMessage("AccessCannotDelete", "");	//	previleges
                FireDataStatusEEvent("AccessCannotDelete", "", true);	//	previleges
                return false;
            }
            //	Is this record deletable?
            if (!_deleteable)
            {
                //ShowInfoMessage("AccessNotDeleteable", "");	//	previleges
                FireDataStatusEEvent("AccessNotDeleteable", "", true);	//	audit
                return false;
            }
            //	Processed Column and not an Import Table
            if (_indexProcessedColumn > 0 && !_tableName.StartsWith("I_"))
            {
                bool processed = false;

                bool.TryParse(rowData[_indexProcessedColumn].ToString(), out processed);
                if (processed)
                {
                    //ShowInfoMessage("CannotDeleteTrx", "");
                    FireDataStatusEEvent("CannotDeleteTrx", "", true);
                    return false;
                }
            }


            // DataRow rowData = _bufferTable.Rows[row];// _buffer.Tables[0].Rows[row];
            //
            MTable table = MTable.Get(_ctx, _AD_Table_ID);
            PO po = null;
            int recordId = GetKeyID(-1);
            if (recordId != -1)
                po = table.GetPO(_ctx, recordId, null);
            else	//	Multi - Key
                po = table.GetPO(_ctx, GetWhereClause(rowData), null);
            //	Delete via PO 
            if (po != null)
            {
                bool ok = false;
                try
                {
                    ok = po.Delete(false);
                }
                catch (Exception ex)
                {
                    log.Log(Level.SEVERE, "Delete", ex);
                    //ErrorLog.FillErrorLog("GridTable", "Delete by PO", ex.Message, VAdvantage.Framework.Message.MessageType.ERROR);
                }
                if (!ok)
                {
                    ValueNamePair vp = VLogger.RetrieveError();
                    if (vp != null)
                        FireDataStatusEEvent(vp);
                    else
                        FireDataStatusEEvent("DeleteError", "", true);
                    return false;
                }
            }
            else //Delete By Sql
            {

                //String trxName = Trx.CreateTrxName("del_");
                Trx trx = Trx.Get("del_");

                PORecord.DeleteCascade(_AD_Table_ID, recordId, trx);

                StringBuilder sql = new StringBuilder("DELETE FROM ");
                sql.Append(_tableName).Append(" WHERE ").Append(GetWhereClause(rowData));
                int no = 0;
                try
                {
                    no = DataBase.DB.ExecuteQuery(sql.ToString(), null, trx);
                    trx.Commit();
                    trx.Close();
                    trx = null;
                }
                catch (System.Data.Common.DbException e)
                {
                    trx.Rollback();
                    trx.Close();
                    trx = null;


                    log.Log(Level.SEVERE, sql.ToString(), e);
                    //ErrorLog.FillErrorLog("GridTable", sql.ToString(), e.Message, VAdvantage.Framework.Message.MessageType.ERROR);

                    String msg = "DeleteError";
                    if (e.ErrorCode == -2146232008)	//	Child Record Found
                        msg = "DeleteErrorDependent";
                    //ShowInfoMessage(msg, "");
                    FireDataStatusEEvent(msg, e.Message, true);

                    //trx = null;
                    return false;
                }

                catch (Exception e)
                {
                    String msg = "DeleteError";
                    //if (e.ErrorCode == 2292)	//	Child Record Found
                    //    msg = "DeleteErrorDependent";
                    //ShowInfoMessage(msg, "");
                    FireDataStatusEEvent(msg, e.InnerException.Message, true);
                    trx.Rollback();
                    trx.Close();
                    trx = null;
                    return false;

                }
                if (no != 1)
                {
                    log.Log(Level.SEVERE, "Number of deleted rows = " + no);
                    //ErrorLog.FillErrorLog("GridTable.Delete()", sql.ToString(), "Number of deleted rows = " + no, VAdvantage.Framework.Message.MessageType.INFORMATION);
                    return false;
                }
            }
            //	inform
            _changed = false;
            _rowChanged = -1;
            FireDataStatusIEvent("Deleted", "");
            log.Fine("Row= delete complete");
            return true;
        }

        /// <summary>
        /// Create and fire Data Status Event (from Error Log)
        /// </summary>
        /// <param name="errorLog"></param>
        protected void FireDataStatusEEvent(ValueNamePair errorLog)
        {
            if (errorLog != null)
                FireDataStatusEEvent(errorLog.GetValue(), errorLog.GetName(), true);
        }

        /// <summary>
        /// Create and fire Data Status Error Event
        /// </summary>
        /// <param name="AD_Message"></param>
        /// <param name="info"></param>
        /// <param name="isError"></param>
        protected void FireDataStatusEEvent(String AD_Message, String info, bool isError)
        {
            DataStatusEvent e = CreateDSE();
            e.SetInfo(AD_Message, info, isError, !isError);
            if (isError)
            {
                log.SaveWarning(AD_Message, info);
            }
            if (FireDataStatusEvent != null)
            {
                FireDataStatusEvent(this, e);
            }
        }

        /// <summary>
        ///  Create and fire Data Status Info Event
        /// </summary>
        /// <param name="AD_Message"></param>
        /// <param name="info"></param>
        protected void FireDataStatusIEvent(String AD_Message, String info)
        {
            DataStatusEvent e = CreateDSE();
            e.SetInfo(AD_Message, info, false, false);
            //e.SetCurrentRow(_currentRow);
            if (FireDataStatusEvent != null)
            {
                FireDataStatusEvent(this, e);
            }
        }



        /// <summary>
        /// Create Data Status Event
        /// </summary>
        /// <returns></returns>
        private DataStatusEvent CreateDSE()
        {
            //bool changed = m_changed;
            //if (m_rowChanged != -1)
            //   changed = true;
            DataStatusEvent dse = new DataStatusEvent(this, (_bufferTable != null) ? _bufferTable.Rows.Count : 0, _changed,
               _ctx.IsAutoCommit(_windowNo), _inserting);
            dse.AD_Table_ID = _AD_Table_ID;
            dse.Record_ID = null;
            return dse;
        }

        #region "Commented"

        //public void EarlyDataBinding()
        //{
        //    //essageBox.Show("grid enter");

        //    for (int i = 0; i < m_fields.Count; i++)
        //    {
        //        GridField field = m_fields[i];
        //        if (!field.IsDisplayed() || !DisplayType.IsLookup(field.GetDisplayType()) || !field.IsParentColumn())
        //        {
        //            continue;
        //        }
        //        try
        //        {
        //            List<KeyValuePair<string, Object>> _data ;//= field.GetLookup().Get(_buffer.Tables[0].Rows[0][i]);
        //            DataGridViewComboBoxColumn combo = (DataGridViewComboBoxColumn)grdTable.Columns[field.GetHeader()];

        //            //c/ombo.DataSource = GetTabPage.GetLookupDataTable(field.GetColumnName(), _data);

        //            //    //((DataGridViewComboBoxColumn)grdTable.Columns[field.GetHeader()]).DisplayMember = "Name";
        //            //    //((DataGridViewComboBoxColumn)grdTable.Columns[field.GetHeader()]).ValueMember = field.GetColumnName();
        //            //    ////grdLoc.DataPropertyName = columnName;
        //            //    //((DataGridViewComboBoxColumn)grdTable.Columns[field.GetHeader()]).DataPropertyName = field.GetColumnName();
        //            //    // grdTable.InvalidateColumn(i);
        //            //}
        //        }
        //        catch
        //        {
        //        }
        //    }
        //    //grdTable.Invalidate();

        //}

        //public void SetCurrentRow(int row)
        //{

        //    if (row < 0)
        //    {
        //        return;
        //    }
        //    for (int i = 0; i < grdTable.ColumnCount; i++)
        //    {
        //        if (!grdTable.Columns[i].Visible)
        //        {
        //            continue;
        //        }
        //        GridField field = m_fields[i];
        //        int displayType = field.GetDisplayType();
        //        Lookup lookup = field.GetLookup();
        //        if (DisplayType.IsLookup(displayType) && displayType != DisplayType.Search)
        //        {
        //            if (lookup == null || (lookup.IsValidated()))
        //            {
        //                continue;
        //            }
        //            object value = grdTable.Rows[row].Cells[i].Value;
        //            string retValue = lookup.GetDisplay(value);

        //            DataGridViewComboBoxCell cell = grdTable.Rows[row].Cells[i] as DataGridViewComboBoxCell;
        //            List<KeyValuePair<string, Object>> _data = new List<KeyValuePair<string, Object>>();
        //            _data.Insert(0, new KeyValuePair<string, Object>(retValue, value));
        //            cell.DataSource = _data;
        //            cell.DisplayMember = "Key";
        //            cell.ValueMember = "Value";
        //        }
        //    }
        //}

        /// <summary>
        /// Set Value in data and update MField.
        /// </summary>
        /// <param name="value">value to assign to cell</param>
        /// <param name="row">row index of cell</param>
        /// <param name="col">column index of cell</param>
        //public void SetValueAt(Object value, int row, int col)
        //{
        //    SetValueAt(value, row, col, false);
        //}

        /// <summary>
        /// Set Value in data and update MField.
        /// </summary>
        /// <param name="value">value to assign to cell</param>
        /// <param name="row">row index of cell</param>
        /// <param name="col">column index of cell</param>
        /// <param name="force">force setting new value</param>
        //public void SetValueAt(Object value, int row, int col, bool force)
        //{
        //    //	Can we edit?
        //    if (!_open || _readOnly       //  not accessible
        //            || row < 0 || col < 0   //  invalid index
        //            || _buffer.Tables.Count ==0|| _buffer.Tables[0].Rows.Count ==0)     //  no rows
        //    {
        //        //log.finest("r=" + row + " c=" + col + " - R/O=" + _readOnly + ", Rows=" + grdTable.RowCount + " - Ignored");
        //        return;
        //    }

        //    //DataSave(false, row); // datarowview instead of row index

        //    //	Has anything changed?
        //    Object oldValue = GetValueAt(row, col);
        //    if (!force && (
        //        (oldValue == null && value == null)
        //        || (oldValue != null && oldValue.Equals(value))
        //        || (oldValue != null && value != null && oldValue.ToString().Equals(value.ToString()))
        //        || (oldValue == null && "".Equals(value))
        //        ))
        //    {
        //        //log.finest("r=" + row + " c=" + col + " - New=" + value + "==Old=" + oldValue + " - Ignored");
        //        return;
        //    }

        //    //log.fine("r=" + row + " c=" + col + " = " + value + " (" + oldValue + ")");

        //    //  Save old value
        //    //_oldValue = new Object[3];
        //    //_oldValue[0] = row;
        //    //_oldValue[1] = col;
        //    //_oldValue[2] = oldValue;

        //    //	Set Data item
        //    DataRow rowData = _buffer.Tables[0].Rows[row];

        //    _rowChanged = row;

        //    /**	Selection
        //    if (col == 0)
        //    {
        //        rowData[col] = value;
        //        m_buffer.set(sort.index, rowData);
        //        return;
        //    }	**/

        //    //	save original value - shallow copy
        //    if (_rowData == null)
        //    {
        //        int size = m_fields.Count;
        //        _rowData = new Object[size];
        //        for (int i = 0; i < size; i++)
        //            _rowData[i] = rowData[i];
        //    }

        //    //	save & update
        //    // rowData[col] = value;

        //    //DataGridViewRow  drRow = grdTable.CurrentRow;

        //   // grdTable.Rows[row].Cells[col].Value = value;


        //    //m_buffer.set(sort.index, rowData);
        //  //  grdTable.NotifyCurrentCellDirty(true);
        //    // grdTable.EndEdit();
        //    //  update Table
        //    //fireTableCellUpdated(row, col);
        //    //  update MField
        //    GridField field = GetField(col);
        //    field.SetValue(value, _inserting);


        //    //  inform
        //    //DataStatusEvent evt = createDSE();
        //    //evt.setChangedColumn(col, field.GetColumnName());
        //    //fireDataStatusChanged(evt);
        //}
        #endregion

        /// <summary>
        ///Refresh Row - ignore changes
        /// </summary>
        /// <param name="row"></param>
        public void DataRefresh(DataRowView rowData)
        {
            log.Info("Row= Refresh");

            if (rowData == null || _bufferTable.Rows.Count < 1 || _inserting)
                return;

            //MSort sort = (MSort)m_sort.get(row);
            //Object[] rowData = (Object[])m_buffer.get(sort.index);

            //  ignore
            DataIgnore();

            //	Create SQL
            String where = GetWhereClause(rowData);
            if (where == null || where.Length == 0)
                where = "1=2";
            String sql = _SQL_Select + " WHERE " + where;
            //sort = (MSort)m_sort.get(row);
           // Object[] rowDataDB = null;
            IDataReader dr = null;
            DataTable dt = null;
            int size = rowData.Row.ItemArray.Length;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql, null);
                dt = new DataTable();
                dt.Load(dr);


                //	only one row
                if (dt.Rows.Count > 0)
                {
                    if (dt.Columns.Count == size)
                    {
                        for (int i = 0; i < size; i++)
                        {
                            rowData[i] = dt.Rows[0][i];
                        }
                    }
                    else
                    {
                        dr.Close();
                        dr = null;
                        dt = null;
                        return;
                    }
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
                dt = null;
                log.Log(Level.SEVERE, sql, e);
                //fireTableRowsUpdated(row, row);
                FireDataStatusEEvent("RefreshError", sql, true);

                rowData.CancelEdit();
                return;
            }

            rowData.Row.AcceptChanges();
            rowData.EndEdit();

            //	update buffer
            //m_buffer.set(sort.index, rowDataDB);
            //	info
            _rowData = null;
            _changed = false;
            _rowChanged = -1;
            _inserting = false;
            //fireTableRowsUpdated(row, row);
            FireDataStatusIEvent("Refreshed", "");
        }	//	dataRefresh

        int m_rowCount = 0;
        /// <summary>
        /// Return number of rows
        /// </summary>
        /// <returns>Number of rows or 0 if not opened</returns>
        public int GetRowCount()
        {
            return m_rowCount;
        }

        /// <summary>
        /// Is Loading
        /// </summary>
        /// <returns>true if loading</returns>
        public bool IsLoading()
        {
            //if (m_loader != null && m_loader.isAlive())
            if (loader != null && loader.IsAlive)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        ///	Encrypt
        /// </summary>
        /// <param name="xx"></param>
        /// <returns></returns>
        private Object Encrypt(Object xx)
        {
            if (xx == null)
                return null;
            return SecureEngine.Encrypt(xx);
        }	//	encrypt

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="yy"></param>
        /// <returns></returns>
        private Object Decrypt(Object yy)
        {
            if (yy == null)
                return null;
            return SecureEngine.Decrypt(yy);
        }	//	d

    }


    public class DataStatusEvent : EventArgs
    {
        private int _totalRows;
        private bool _changed;
        private bool _autoSave;
        private bool _inserting;
        //
        private String _AD_Message = null;
        private String _info = null;
        public bool _isError = false;
        private bool _isWarning = false;
        private bool _confirmed = false;
        //
        private bool _allLoaded = true;
        private int _loadedRows = -1;
        private int _currentRow = -1;
        //
       // private int _changedColumn = 0;
       // private String _columnName = null;

        /** Created 				*/
        public DateTime? Created = null;
        /** Created By				*/
        public int CreatedBy;
        /** Updated					*/
        public DateTime? Updated = null;
        /** Updated By				*/
        public int UpdatedBy;

        /** Info					*/
        public String Info = null;
        /** Table ID				*/
        public int AD_Table_ID = 0;
        /** Record ID				*/
        public Object Record_ID = null;
        private Object _source;


        public DataStatusEvent()
        {
        }

        /// <summary>
        /// Set Loaded Info
        /// </summary>
        /// <param name="loadedRows"></param>
        public void SetLoading(int loadedRows)
        {
            _allLoaded = false;
            _loadedRows = loadedRows;
        }	//

        public DataStatusEvent(string msg, string information)
        {
            _AD_Message = msg;
            _info = information;
        }

        /// <summary>
        ///Constructor
        /// </summary>
        /// <param name="source1">source</param>
        /// <param name="totalRows">total rows</param>
        /// <param name="changed">changed</param>
        /// <param name="autoSave">auto save</param>
        /// <param name="inserting">inserting</param>
        public DataStatusEvent(Object source1, int totalRows, bool changed,
            bool autoSave, bool inserting)
        {
            _source = source1;
            _totalRows = totalRows;
            _changed = changed;
            _autoSave = autoSave;
            _inserting = inserting;
        }

        public void SetTotalRows(int row)
        {
            _totalRows = row;
        }

        /// <summary>
        ///Set Message Info
        /// </summary>
        /// <param name="AD_Message"></param>
        /// <param name="info"></param>
        /// <param name="isError"></param>
        /// <param name="isWarning"></param>
        public void SetInfo(String AD_Message, String info, bool isError, bool isWarning)
        {
            _AD_Message = AD_Message;
            _info = info;
            _isError = isError;
            _isWarning = isWarning;
        }

        /// <summary>
        ///Get Message Info
        /// </summary>
        /// <returns></returns>
        public String GetAD_Message()
        {
            return _AD_Message;
        }

        public bool IsChanged()
        {
            return _changed;
        }

        public bool Isinserting()
        {
            return _inserting;
        }

        public int GetTotalRows()
        {
            return _totalRows;
        }

        /// <summary>
        ///Get Message Info
        /// </summary>
        /// <returns></returns>
        public void SetAD_Message()
        {
            _AD_Message = null;
        }

        /// <summary>
        ///Set current Row (zero based)
        /// </summary>
        /// <param name="currentRow"></param>
        public void SetCurrentRow(int currentRow)
        {
            _currentRow = currentRow;
        }

        /// <summary>
        ///Get current row (zero based)
        /// </summary>
        /// <returns></returns>
        public int GetCurrentRow()
        {
            return _currentRow;
        }

        public void SetInserting(bool val)
        {
            _inserting = val;
        }

        /// <summary>
        ///Get Message Info
        /// </summary>
        /// <returns></returns>
        public String GetInfo()
        {
            return _info;
        }	//	getInfo

        /// <summary>
        ///	String representation of Status.
        /*  <pre>
        *		*1/20 		Change - automatic commit
        *		?1/20		Change - manual confirm
        *		 1/56->200	Loading
        *		 1/20		Normal
        *     +*1/20       Inserting, changed - automatic commit
        *  The row number is converted from zero based representation
        *  </pre>
        *  @return Status info
        */
        /// </summary>
        /// <returns></returns>
        public String GetMessage()
        {
            StringBuilder retValue = new StringBuilder();
            if (_inserting)
                retValue.Append("+");
            retValue.Append(_changed ? (_autoSave ? "*" : "?") : " ");
            //  current row
            if (_totalRows == 0)
                retValue.Append(_currentRow);
            else
                retValue.Append(_currentRow + 1);
            //  of
            retValue.Append("/");
            if (_allLoaded)
                retValue.Append(_totalRows);
            else
                retValue.Append(_loadedRows).Append("->").Append(_totalRows);
            //
            return retValue.ToString();
        }

        /// <summary>
        ///Is this an error
        /// </summary>
        /// <returns></returns>
        public bool IsError()
        {
            return _isError;
        }

        /// <summary>
        ///Is Confirmed (e.g. user has seen it)
        /// </summary>
        /// <returns></returns>
        public bool IsConfirmed()
        {
            return _confirmed;
        }

        /// <summary>
        ///Set Confirmed toggle
        /// </summary>
        /// <param name="confirmed"></param>
        public void SetConfirmed(bool confirmed)
        {
            _confirmed = confirmed;
        }

        /// <summary>
        ///Is this a warning
        /// </summary>
        /// <returns>true if warning</returns>
        public bool IsWarning()
        {
            return _isWarning;
        }



    }
}
