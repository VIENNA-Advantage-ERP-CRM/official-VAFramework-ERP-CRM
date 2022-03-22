/********************************************************
 * Module Name    : PO
 * Purpose        : Pesistant Object (Super Class For Actual Implementation)
                    (also implemnet Save update Metohds   etc)
 * Class Used     : -----
 * Created By     : Harwinder /Jagmohan / Veena/Raghu
 * Date           : --------
**********************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using VAdvantage.Classes;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.WF;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VAdvantage.Logging;
using VAdvantage.Print;
using BaseLibrary.Engine;

namespace VAdvantage.Model
{
    /// <summary>
    /// </summary>
    /// 
    [Serializable]
    public abstract class PO : IComparer<PO>, IEquatable<Object>, Evaluatee
    {

        #region Declaration

        /* Context              */
        public Ctx p_ctx;
        /* Model Info              */
        protected volatile POInfo p_info = null;
        /**	Optional Transaction		*/
        //private string _mtrxName = null;

        //Optional Transaction		
        private Trx _trx = null;

        /** Original Values         */
        private Object[] _mOldValues = null;
        /** New Values              */
        private Object[] _mNewValues = null;
        /** Record_IDs          		*/
        private Object[] _mIDs = new Object[] { I_ZERO };
        private string[] _mKeyColumns;
        private bool _mCreateNew = false;

        /**	Logger	
         * */
        //[NonSerialized]
        protected VLogger log = null;
        /** Static Logger					*/
        private static VLogger s_log = VLogger.GetVLogger(typeof(PO).FullName);



        static protected String ENTITYTYPE_UserMaintained = "U";
        /** Dictionary Maintained Entity Type		*/
        static protected String ENTITYTYPE_Dictionary = "D";

        /** Accounting Columns			*/
        private List<String> acctColumns = null;

        //Deleted ID					
        private int _idOld = 0;

        /**	Attachment with entriess	*/
        private dynamic _attachment = null;
        /** Document Value Workflow Manager		*/
        private static DocWorkflowMgr s_docWFMgr = null;

        // Document Value Workflow message
        public string s_docWFMsg = "";

        private int parent_ID = 0;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public PO()
        {
            if (log == null)
                log = VLogger.GetVLogger(this.GetType().FullName);

            SqlConnection conn = new SqlConnection("");
        }

        /// <summary>
        /// Method Body Define in inheritee class
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        protected virtual POInfo InitPO(Context ctx)
        {
            return null;
        }

        protected abstract POInfo InitPO(Ctx ctx);
        //present
        /// <summary>
        /// Create & Load existing Persistent Object
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="ID">The unique ID of the object</param>
        /// <param name="trxName">transaction name</param>

        //public PO(Context ctx, int ID, Trx trxName)
        //{
        //    if (log == null)
        //        log = VLogger.GetVLogger(this.GetType().FullName);

        //    Init((Ctx)ctx, ID, null, trxName);
        //} //past

        //[Obsolete("use PO(Ctx ctx, int ID, Trx trx) instead")]
        //public PO(Ctx ctx, int ID, Trx trxName,bool dontUse = true)
        //{
        //    if (log == null)
        //        log = VLogger.GetVLogger(this.GetType().FullName);

        //    Init(ctx, ID, null, trxName);
        //} //present



        public PO(Ctx ctx, int ID, Trx trx)
        {
            if (log == null)
                log = VLogger.GetVLogger(this.GetType().FullName);

            Init(ctx, ID, null, trx);
        } //

        /// <summary>
        ///Fill map with data as Strings
        /// </summary>
        /// <param name="hmOut">map</param>
        public void FillMap(HashMap<String, String> hmOut)
        {
            int size = p_info.GetColumnCount();//  get_ColumnCount();
            for (int i = 0; i < size; i++)
            {
                Object value = Get_Value(i);
                //	Don't insert NULL values (allows Database defaults)
                if (value == null
                    || p_info.IsVirtualColumn(i))
                    continue;
                //	Display Type
                int dt = p_info.GetColumnDisplayType(i);
                //  Based on class of definition, not class of value
                //Class<?> c = p_info.getColumnClass(i);
                Type c = p_info.GetColumnClass(i);
                String stringValue = null;
                if (c == typeof(Object))
                {
                    ;	//	saveNewSpecial (value, i));
                }
                else if (value == null || value.Equals(Null.NULL))
                {
                    ;
                }
                else if (value is int || value is Decimal)
                {
                    stringValue = value.ToString();
                }
                else if (c == typeof(Boolean))
                {
                    Boolean bValue = false;
                    if (value is bool)
                    {
                        bValue = Util.GetValueOfBool((bool)value);
                    }
                    else
                        bValue = "Y".Equals(value);
                    stringValue = bValue ? "Y" : "N";
                }
                else if (value is DateTime)
                {
                    stringValue = value.ToString();
                }
                else if (c == typeof(String))
                {
                    stringValue = (String)value;
                }
                else if (dt == DisplayType.TextLong)
                {
                    stringValue = (String)value;
                }
                else if (dt == DisplayType.Binary)
                {
                    stringValue = Util.ToHexString((byte[])value);
                }
                else
                {
                    ;	//	saveNewSpecial (value, i));
                }
                //
                if (stringValue != null)
                {
                    //hmOut.put(p_info.getColumnName(i), stringValue);
                    hmOut.Add(p_info.GetColumnName(i), stringValue);
                }
            }
            //	Custom Columns
            if (m_custom != null)
            {
                //Iterator<String> it = m_custom.keySet().iterator();
                IEnumerator<String> it = m_custom.Keys.GetEnumerator();// GetEnumerator();
                while (it.MoveNext())
                {
                    String column = it.Current;
                    //	int index = p_info.getColumnIndex(column);
                    //String value = (String)m_custom.get(column);
                    String value = (String)m_custom[column];
                    if (value != null)
                    {
                        //hmOut.put(column, value);
                        hmOut.Add(column, value);
                    }
                }
                m_custom = null;
            }
        }   //  fillMap




        //void Init(Context ctx, int ID, DataRow rs, Trx trx)
        //{
        //    if (ctx == null)
        //        throw new ArgumentException("No Context");
        //    if (ID == 0 && rs == null && ctx.GetAD_Client_ID() < 0)
        //        throw new ArgumentException("R/O Context - no new instances allowed");
        //    p_ctx = ctx;
        //    p_info = InitPO(ctx);
        //    if (p_info == null || p_info.GetTableName() == null)
        //        throw new ArgumentException("Invalid PO Info - " + p_info);
        //    //
        //    int size = p_info.GetColumnCount();
        //    _mOldValues = new Object[size];
        //    _mNewValues = new Object[size];
        //    _mtrxName = trx;
        //    if (rs != null)
        //        Load(rs);		//	will not have virtual columns
        //    else
        //        Load(ID, trx);
        //}   // past

        //void Init(Ctx ctx, int ID, DataRow rs, string trx)
        //{
        //    if (ctx == null)
        //    {
        //        throw new ArgumentException("No Context");
        //    }
        //    if (ID == 0 && rs == null && ctx.GetAD_Client_ID() < 0)
        //        throw new ArgumentException("R/O Context - no new instances allowed");
        //    p_ctx = ctx;
        //    p_info = InitPO(ctx);
        //    if (p_info == null || p_info.GetTableName() == null)
        //        throw new ArgumentException("Invalid PO Info - " + p_info);
        //    //
        //    int size = p_info.GetColumnCount();
        //    _mOldValues = new Object[size];
        //    _mNewValues = new Object[size];
        //    _mtrxName = trx;
        //    if (rs != null)
        //    {
        //        Load(rs);		//	will not have virtual columns
        //    }
        //    else
        //        Load(ID, trx);
        //}   //  present

        void Init(Ctx ctx, int ID, DataRow rs, Trx trx) //future
        {
            if (ctx == null)
            {
                throw new ArgumentException("No Context");
            }
            if (ID == 0 && rs == null && ctx.GetAD_Client_ID() < 0)
                throw new ArgumentException("R/O Context - no new instances allowed");
            p_ctx = ctx;
            p_info = InitPO(ctx);
            if (p_info == null || p_info.GetTableName() == null)
                throw new ArgumentException("Invalid PO Info - " + p_info);
            //
            int size = p_info.GetColumnCount();
            _mOldValues = new Object[size];
            _mNewValues = new Object[size];
            _trx = trx;
            if (rs != null)
            {
                Load(rs);		//	will not have virtual columns
            }
            else
                Load(ID, trx);
        }   //  future

        void Init(Ctx ctx, int ID, IDataReader dr, Trx trx, bool extra)
        {
            if (ctx == null)
                throw new ArgumentException("No Context");
            if (ID == 0 && dr == null && ctx.GetAD_Client_ID() < 0)
                throw new ArgumentException("R/O Context - no new instances allowed");
            p_ctx = ctx;
            p_info = InitPO(ctx);
            if (p_info == null || p_info.GetTableName() == null)
                throw new ArgumentException("Invalid PO Info - " + p_info);
            //
            int size = p_info.GetColumnCount();
            _mOldValues = new Object[size];
            _mNewValues = new Object[size];
            _trx = trx;
            if (dr != null)
            {
                Load(dr);		//	will not have virtual columns
            }
            else
                Load(ID, trx);
        }   //future

        /// <summary>
        /// Create & Load existing Persistent Object.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="rs"></param>
        /// <param name="trxName"></param>
        ////public PO(Context ctx, DataRow rs, string trxName)
        ////{
        ////    if (log == null)
        ////        log = VLogger.GetVLogger(this.GetType().FullName);

        ////    Init(ctx, 0, rs, trxName);
        ////}	// past

        /// <summary>
        /// Create & Load existing Persistent Object.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="rs"></param>
        /// <param name="trxName"></param>

        //[Obsolete("use PO(Ctx ctx, DataRow dr, Trx trx)")]
        //public PO(Ctx ctx, DataRow dr, string trxName)
        //{
        //    if (log == null)
        //        log = VLogger.GetVLogger(this.GetType().FullName);

        //    Init(ctx, 0, dr, trxName);
        //}	// present

        /// <summary>
        /// Create & Load existing Persistent Object.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="rs"></param>
        /// <param name="trxName"></param>
        public PO(Ctx ctx, DataRow dr, Trx trx)
        {
            if (log == null)
                log = VLogger.GetVLogger(this.GetType().FullName);

            Init(ctx, 0, dr, trx);
        }	// present



        public PO(Ctx ctx, IDataReader dr, Trx trx)
        {
            if (log == null)
                log = VLogger.GetVLogger(this.GetType().FullName);

            Init(ctx, 0, dr, trx, false);
        }	// future

        ////Datarow rs
        //private PO(Context ctx, int ID, DataRow rs, string trxName)
        //{
        //    if (log == null)
        //        log = VLogger.GetVLogger(this.GetType().FullName);

        //    Init(ctx, 0, rs, trxName);
        //    //if (ctx == null)
        //    //    throw new ArgumentException("No Context");
        //    //p_ctx = ctx;
        //    //p_info = InitPO(ctx); //this is abstract Method
        //    //if (p_info == null || p_info.GetTableName() == "")
        //    //    throw new ArgumentException("Invalid PO Info - " + p_info);

        //    //int size = p_info.GetColumnCount();
        //    ////intilize object array 
        //    //_mOldValues = new Object[size];
        //    //_mNewValues = new Object[size];
        //    //_mtrxName = trxName;
        //    //if (rs != null) //id datarow
        //    //    Load(rs);		//	will not have virtual columns
        //    //else
        //    //    Load(ID, trxName);
        //}   // past


        /// <summary>
        /// Get a copy/clone of PO and set ctx
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="po"></param>
        /// <param name="trx"></param>
        /// <returns>po or null</returns>
        /// <date>03-march-2011</date>
        public static PO Copy(Ctx ctx, PO po, Trx trx)
        {

            if (ctx == null || po == null)
                return null;
            try
            {
                PO newPO = (PO)po.MemberwiseClone();
                newPO.p_ctx = ctx;
                newPO._trx = trx;
                return newPO;
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, po.ToString(), e);
            }
            return null;
        }


        private PO(Ctx ctx, int ID, DataRow rs, Trx trx)
        {
            if (log == null)
                log = VLogger.GetVLogger(this.GetType().FullName);

            Init(ctx, 0, rs, trx);
            //if (ctx == null)
            //    throw new ArgumentException("No Context");
            //p_ctx = ctx;
            //p_info = InitPO(ctx); //this is abstract Method
            //if (p_info == null || p_info.GetTableName() == "")
            //    throw new ArgumentException("Invalid PO Info - " + p_info);

            //int size = p_info.GetColumnCount();
            ////intilize object array 
            //_mOldValues = new Object[size];
            //_mNewValues = new Object[size];
            //_mtrxName = trxName;
            //if (rs != null) //id datarow
            //    Load(rs);		//	will not have virtual columns
            //else
            //    Load(ID, trxName);
        }   // present

        //private PO(Ctx ctx, int ID, IDataReader dr, string trx)
        //{
        //    if (log == null)
        //        log = VLogger.GetVLogger(this.GetType().FullName);

        //    Init(ctx, 0, dr, trx, false);
        //    //if (ctx == null)
        //    //    throw new ArgumentException("No Context");
        //    //p_ctx = ctx;
        //    //p_info = InitPO(ctx); //this is abstract Method
        //    //if (p_info == null || p_info.GetTableName() == "")
        //    //    throw new ArgumentException("Invalid PO Info - " + p_info);

        //    //int size = p_info.GetColumnCount();
        //    ////intilize object array 
        //    //_mOldValues = new Object[size];
        //    //_mNewValues = new Object[size];
        //    //_mtrxName = trxName;
        //    //if (rs != null) //id datarow
        //    //    Load(rs);		//	will not have virtual columns
        //    //else
        //    //    Load(ID, trxName);
        //}   //future


        /// <summary>
        ///Get ID of table
        /// </summary>
        /// <param name="tableName">Name of Table</param>
        /// <returns>Table_ID</returns>
        public static int Get_Table_ID(String tableName)
        {
            String sql = "SELECT AD_Table_ID FROM AD_Table WHERE TableName=@param1";
            int AD_Table_ID = DB.GetSQLValue(null, sql, tableName);
            return AD_Table_ID;
        }

        //protected int Get_ColumnCount()
        //{
        //    return p_info.GetColumnCount();
        //}   //  getColumnCount

        /// <summary>
        /// Compare based on DocumentNo, Value, Name, Description
        /// </summary>
        /// <param name="o1">Object 1</param>
        /// <param name="o2">Object 2</param>
        /// <returns>-1 if o1 < o2</returns>
        /// <author>raghu</author>
        public int Compare(PO o1, PO o2)
        {
            if (o1 == null)
            {
                return -1;
            }
            else if (o2 == null)
            {
                return 1;
            }
            //	same class
            if (o1.GetType().Equals(o2.GetType()))
            {
                int index = Get_ColumnIndex("DocumentNo");
                if (index == -1)
                {
                    index = Get_ColumnIndex("Value");
                }
                if (index == -1)
                {
                    index = Get_ColumnIndex("Name");
                }
                if (index == -1)
                {
                    index = Get_ColumnIndex("Description");
                }
                if (index != -1)
                {
                    PO po1 = (PO)o1;
                    Object comp1 = po1.Get_Value(index);
                    PO po2 = (PO)o2;
                    Object comp2 = po2.Get_Value(index);
                    if (comp1 == null)
                    {
                        return -1;
                    }
                    else if (comp2 == null)
                    {
                        return 1;
                    }
                    return comp1.ToString().CompareTo(comp2.ToString());
                }
            }
            return o1.ToString().CompareTo(o2.ToString());
        }


        /// <summary>
        /// Equals based on ID
        /// </summary>
        /// <param name="cmp">comparator</param>
        /// <returns>true if ID the same</returns>
        /// <author>Raghu</author>
        public override bool Equals(Object cmp)
        {
            if (cmp == null)
            {
                return false;
            }
            if (!(cmp is PO))
            {
                return false;
            }
            if (cmp.GetType().Equals(this.GetType()))
            {
                if (_mIDs.Length < 2)
                {
                    return ((PO)cmp).Get_ID() == Get_ID();
                }
                //	Multi-Key Compare
                String[] keyColumns = Get_KeyColumns();
                for (int i = 0; i < keyColumns.Length; i++)
                {
                    String keyColumn = keyColumns[i];
                    Object o1 = Get_Value(keyColumn);
                    Object o2 = ((PO)cmp).Get_Value(keyColumn);
                    if (!Util.IsEqual(o1, o2))
                    {
                        return false;
                    }
                }
                return true;
            }
            return base.Equals(cmp);
        }


        /// <summary>
        /// Return Deleted Single Key Record ID //used in translationtables class
        /// </summary>
        /// <returns>ID or 0</returns>
        /// <author>raghu</author>
        public int Get_IDOld()
        {
            return _idOld;
        }
        /// <summary>
        ///Set Document Value Workflow Manager
        /// </summary>
        /// <param name="docWFMgr">mgr</param>
        /// <author>raghu</author>
        public static void SetDocWorkflowMgr(DocWorkflowMgr docWFMgr)
        {
            s_docWFMgr = docWFMgr;
            s_log.Config(s_docWFMgr.ToString());
        }

        /// <summary>
        ///Copy old values of From to new values of To.
        ///Does not copy Keys and AD_Client_ID/AD_Org_ID
        /// do not copy those column values, whose IsCopy is False
        /// </summary>
        /// <param name="from">from old, existing & unchanged PO</param>
        /// <param name="to">to new, not saved PO</param>
        public static void CopyValues(PO from, PO to)
        {
            s_log.Fine("From ID=" + from.Get_ID() + " - To ID=" + to.Get_ID());
            //	Different Classes
            //if (from.getClass() != to.getClass())
            if (from.GetType() != to.GetType())
            {
                for (int i1 = 0; i1 < from._mOldValues.Length; i1++)
                {
                    if (from.p_info.IsVirtualColumn(i1) || from.p_info.IsKey(i1) || !from.p_info.IsCopy(i1))		//	KeyColumn
                        continue;
                    String colName = from.p_info.GetColumnName(i1);
                    //  Ignore Standard Values
                    if (colName.StartsWith("Created")
                        || colName.StartsWith("Updated")
                        || colName.Equals("IsActive")
                        || colName.Equals("AD_Client_ID")
                        || colName.Equals("AD_Org_ID"))
                    {
                        ;	//	ignore
                    }
                    else
                    {
                        for (int i2 = 0; i2 < to._mOldValues.Length; i2++)
                        {
                            if (to.p_info.GetColumnName(i2).Equals(colName))
                            {
                                to._mNewValues[i2] = from._mOldValues[i1];
                                break;
                            }
                        }
                    }
                }	//	from loop
            }
            else	//	same class
            {
                for (int i = 0; i < from._mOldValues.Length; i++)
                {
                    if (from.p_info.IsVirtualColumn(i)
                        || from.p_info.IsKey(i) || !from.p_info.IsCopy(i))		//	KeyColumn
                        continue;
                    String colName = from.p_info.GetColumnName(i);
                    //  Ignore Standard Values
                    if (colName.StartsWith("Created")
                        || colName.StartsWith("Updated")
                        || colName.Equals("IsActive")
                        || colName.Equals("AD_Client_ID")
                        || colName.Equals("AD_Org_ID"))
                    {
                        ;	//	ignore
                    }
                    else
                        to._mNewValues[i] = from._mOldValues[i];
                }
            }	//	same class
        }

        /**
        * 	Copy old values of From to new values of To.
        *  Does not copy Keys
        * 	@param from old, existing & unchanged PO
        *  @param to new, not saved PO
        * 	@param AD_Client_ID client
        * 	@param AD_Org_ID org
        */
        public static void CopyValues(PO from, PO to, int AD_Client_ID, int AD_Org_ID)
        {
            CopyValues(from, to);
            to.SetAD_Client_ID(AD_Client_ID);
            to.SetAD_Org_ID(AD_Org_ID);
        }

        /// <summary>
        ///Get Translation of column
        /// </summary>
        /// <param name="columnName">columnName</param>
        /// <param name="AD_Language">AD_Language</param>
        /// <returns>translation or null if not found</returns>
        ///<author>raghu</author>
        protected String Get_Translation(string columnName, string AD_Language)
        {
            if (columnName == null || AD_Language == null
                || _mIDs.Length > 1 || _mIDs[0].Equals(I_ZERO)
                //|| !(_mIDs[0] is Integer))
                || !(_mIDs[0] is int))
            {
                log.Severe("Invalid Argument: ColumnName" + columnName
                    + ", AD_Language=" + AD_Language
                    + ", ID.length=" + _mIDs.Length + ", ID=" + _mIDs[0]);
                return null;
            }
            int ID = (int)_mIDs[0];
            string retValue = null;
            StringBuilder sql = new StringBuilder("SELECT ").Append(columnName)
                .Append(" FROM ").Append(p_info.GetTableName()).Append("_Trl WHERE ")
                .Append(_mKeyColumns[0]).Append("=" + ID)
                .Append(" AND AD_Language=" + AD_Language);
            DataSet pstmt = null;
            try
            {
                pstmt = DB.ExecuteDataset(sql.ToString(), null, _trx);
                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = pstmt.Tables[0].Rows[i];
                    retValue = rs[1].ToString();//.getString(1);
                }
                pstmt = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql.ToString(), e);
            }
            return retValue;
        }

        //protected void Load(int ID, string trxName)
        //{
        //    log.Finest("ID=" + ID);
        //    if (ID > 0)  // for update
        //    {
        //        _mIDs = new Object[] { ID };
        //        string keyColumn = ""; //	p_info.getTableName() + "_ID";
        //        string parentColumn = "";
        //        //Get Key Column and parent column
        //        for (int i = 0; i < p_info.GetColumnCount(); i++)
        //        {
        //            if (p_info.IsKey(i))
        //            {
        //                keyColumn = p_info.GetColumnName(i);
        //                break;
        //            }
        //            if (p_info.IsColumnParent(i))
        //                parentColumn = p_info.GetColumnName(i);
        //        }
        //        if (keyColumn == "")
        //            keyColumn = parentColumn;
        //        if (keyColumn == "")
        //            throw new Exception("No PK for " + p_info.GetTableName());
        //        _mKeyColumns = new string[] { keyColumn };
        //        Load(trxName);
        //    }
        //    else	//	new row
        //    {
        //        LoadDefaults();
        //        _mCreateNew = true;
        //        SetKeyInfo();	//	sets _mIDs
        //        //loadComplete(true);
        //    }
        //}

        /// <summary>
        /// (re)Load record with _mIDs[*]
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="trx"></param>
        protected void Load(int ID, Trx trx)
        {
            log.Finest("ID=" + ID);
            if (ID > 0)  // for update
            {
                _mIDs = new Object[] { ID };
                string keyColumn = ""; //	p_info.getTableName() + "_ID";
                string parentColumn = "";
                //Get Key Column and parent column
                for (int i = 0; i < p_info.GetColumnCount(); i++)
                {
                    if (p_info.IsKey(i))
                    {
                        keyColumn = p_info.GetColumnName(i);
                        break;
                    }
                    if (p_info.IsColumnParent(i))
                        parentColumn = p_info.GetColumnName(i);
                }
                if (keyColumn == "")
                    keyColumn = parentColumn;
                if (keyColumn == "")
                    throw new Exception("No PK for " + p_info.GetTableName());
                _mKeyColumns = new string[] { keyColumn };
                Load(trx);
            }
            else	//	new row
            {
                LoadDefaults();
                _mCreateNew = true;
                SetKeyInfo();	//	sets _mIDs
                //loadComplete(true);
            }
        }

        /// <summary>
        /// (re)Load record with m_ID[*]
        /// trxName transaction
        /// return true if loaded
        /// </summary>
        /// <param name="trxName"></param>
        /// 
        //[Obsolete("Use Load(Trx trx) instead")]
        //public bool Load(string trxName,bool dontUser=true)
        //{
        //    _mtrxName = trxName; //set transaction name
        //    bool success = true;
        //    //create sql
        //    StringBuilder sql = new StringBuilder("SELECT ");
        //    int size = Get_ColumnCount();
        //    for (int i = 0; i < size; i++)
        //    {
        //        if (i != 0)
        //            sql.Append(",");
        //        sql.Append(p_info.GetColumnSQL(i));	//	Normal and Virtual(column has column-sql value) Column
        //    }
        //    sql.Append(" FROM ").Append(p_info.GetTableName())
        //   .Append(" WHERE ").Append(Get_WhereClause(false));

        //    //	int index = -1;
        //    if (VLogMgt.IsLevelFinest())
        //        log.Finest(Get_WhereClause(true));

        //    IDataReader dr = null;
        //    SqlParameter[] param = null;

        //    try
        //    {
        //        //Set Parameter (primary key ids)
        //        param = new SqlParameter[_mIDs.Length];

        //        for (int j = 0; j < _mIDs.Length; j++)
        //        {
        //            if (_mIDs[j] is int)
        //            {
        //                param[j] = new SqlParameter("@param" + j.ToString(), (int)_mIDs[j]);

        //            }
        //            else
        //            {
        //                param[j] = new SqlParameter("@param" + j.ToString(), _mIDs[j].ToString());
        //            }
        //        }
        //        DataSet ds = null;
        //        //dr = DB.ExecuteReader(sql.ToString(), param, _mtrxName);
        //        try
        //        {
        //            ds = DB.ExecuteDataset(sql.ToString(), param, _mtrxName);

        //        }
        //        catch
        //        {
        //            ds = DB.ExecuteDataset(sql.ToString(), param, null);        //for rare error from postgre (transaction errro)
        //        }
        //        //if (dr.Read())
        //        //{
        //        //    //Load Column Info
        //        //    success = Load(dr);
        //        //}
        //        if (ds != null && ds.Tables[0].Rows.Count > 0)
        //        {
        //            success = Load(ds.Tables[0].Rows[0]);
        //        }
        //        else
        //        {
        //            log.Log(Level.SEVERE, "NO Data found for " + Get_WhereClause(true));
        //            _mIDs = new object[] { 0 }; //set to zero
        //            success = false;
        //        }

        //        //dr.Close();
        //        //dr = null;
        //        ds = null;
        //        param = null;

        //        _mCreateNew = false;
        //        //	reset new values
        //        _mNewValues = new object[size];
        //    }
        //    catch (Exception ex)
        //    {
        //        if (dr != null)
        //        {
        //            dr.Close();
        //            dr = null;
        //        }
        //        param = null;

        //        string msg = "";
        //        if (_mtrxName != null)
        //            msg = "[" + _mtrxName + "] - ";
        //        msg += Get_WhereClause(true)
        //            + ", SQL=" + sql.ToString();
        //        //log error msg

        //        success = false;
        //        _mIDs = new object[] { 0 };
        //        log.Log(Level.SEVERE, msg + "==" + ex.Message);
        //    }
        //    SetKeyInfo();
        //    LoadComplete(success);
        //    return success;
        //}

        /// <summary>
        /// (re)Load record with m_ID[*]
        /// trxName transaction
        /// return true if loaded
        /// </summary>
        /// <param name="trxName"></param>
        public bool Load(Trx trx)
        {
            _trx = trx; //set transaction name
            bool success = true;
            //create sql
            StringBuilder sql = new StringBuilder("SELECT ");
            int size = Get_ColumnCount();
            for (int i = 0; i < size; i++)
            {
                if (i != 0)
                    sql.Append(",");
                sql.Append(p_info.GetColumnSQL(i));	//	Normal and Virtual(column has column-sql value) Column
            }
            sql.Append(" FROM ").Append(p_info.GetTableName())
           .Append(" WHERE ").Append(Get_WhereClause(false));

            //	int index = -1;
            if (VLogMgt.IsLevelFinest())
                log.Finest(Get_WhereClause(true));

            IDataReader dr = null;
            SqlParameter[] param = null;

            try
            {
                //Set Parameter (primary key ids and multi key values (DateTime and Boolean))
                param = new SqlParameter[_mIDs.Length];

                for (int j = 0; j < _mIDs.Length; j++)
                {
                    if (_mIDs[j] is int)
                    {
                        param[j] = new SqlParameter("@param" + j.ToString(), (int)_mIDs[j]);
                    }
                    else if (_mIDs[j] is DateTime)
                    {
                        param[j] = new SqlParameter("@param" + j.ToString(), (DateTime)_mIDs[j]);
                    }
                    else if (_mIDs[j] is Boolean)
                    {
                        param[j] = new SqlParameter("@param" + j.ToString(), ((Boolean)_mIDs[j] ? "Y" : "N"));
                    }
                    else
                    {
                        param[j] = new SqlParameter("@param" + j.ToString(), _mIDs[j].ToString());
                    }
                }
                DataSet ds = null;
                //dr = DB.ExecuteReader(sql.ToString(), param, _mtrxName);
                try
                {
                    ds = DB.ExecuteDataset(sql.ToString(), param, _trx);

                }
                catch
                {
                    ds = DB.ExecuteDataset(sql.ToString(), param, null);        //for rare error from postgre (transaction errro)
                }
                //if (dr.Read())
                //{
                //    //Load Column Info
                //    success = Load(dr);
                //}
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    success = Load(ds.Tables[0].Rows[0]);
                }
                else
                {
                    log.Log(Level.SEVERE, "NO Data found for " + Get_WhereClause(true));
                    _mIDs = new object[] { 0 }; //set to zero
                    success = false;
                }

                //dr.Close();
                //dr = null;
                ds = null;
                param = null;

                _mCreateNew = false;
                //	reset new values
                _mNewValues = new object[size];
            }
            catch (Exception ex)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                param = null;

                string msg = "";
                if (_trx != null)
                    msg = "[" + _trx + "] - ";
                msg += Get_WhereClause(true)
                    + ", SQL=" + sql.ToString();
                //log error msg

                success = false;
                _mIDs = new object[] { 0 };
                log.Log(Level.SEVERE, msg + "==" + ex.Message);
            }
            SetKeyInfo();
            LoadComplete(success);
            return success;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="success"></param>
        protected void LoadComplete(bool success)
        {
        }

        public int GetAccessLevel()
        {
            return Get_AccessLevel();
        }

        public VLogger GetLog()
        {
            return log;
        }

        public string GetTableName()
        {
            return p_info.GetTableName();
        }


        /// <summary>
        /// Load from the current position of a ResultSet
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        protected bool Load(DataRow rs)
        {
            int size = Get_ColumnCount();
            bool success = true;
            int index = 0;
            log.Finest("(rs)");
            //  load column values
            for (index = 0; index < size; index++)
            {
                string columnName = p_info.GetColumnName(index);
                if (DatabaseType.IsMSSql && columnName.Equals("[LineNo]"))
                    columnName = "LineNo";
                if (p_info.IsVirtualColumn(index))
                {
                    try
                    {
                        string ss = rs[columnName].ToString();
                    }
                    catch
                    {
                        continue;
                    }
                }

                int dt = p_info.GetColumnDisplayType(index);

                Type clazz = p_info.GetColumnClass(index);
                try
                {
                    //	NULL
                    if (rs.IsNull(columnName))
                    {
                        _mOldValues[index] = null;
                    }
                    else
                    {
                        //    if (clazz == typeof(System.Int32))
                        //        _mOldValues[index] = Decrypt(index, CommonFunctions.GetInt(rs[columnName]));
                        //    else if (clazz == typeof(decimal))
                        //        _mOldValues[index] = Decrypt(index, decimal.Parse(rs[columnName].ToString()));
                        //    else if (clazz == typeof(System.Boolean))
                        //        _mOldValues[index] = "Y".Equals(Decrypt(index, rs[columnName]));
                        //    else if (clazz == typeof(System.DateTime))
                        //        _mOldValues[index] = Decrypt(index, (DateTime)rs[columnName]);
                        //    else if (DisplayType.IsLOB(dt))
                        //        _mOldValues[index] = Get_LOB(rs[columnName]);
                        //    else if (clazz == typeof(System.String))
                        //        _mOldValues[index] = Decrypt(index, CommonFunctions.GetString(rs[columnName]));
                        //    else
                        //        _mOldValues[index] = rs[columnName];

                        if (clazz == typeof(System.Int32))
                            _mOldValues[index] = Decrypt(index, Util.GetValueOfInt(rs[columnName]));
                        else if (clazz == typeof(decimal))
                            _mOldValues[index] = Decrypt(index, Util.GetNullableDecimal(rs[columnName]));
                        else if (clazz == typeof(System.Boolean))
                            _mOldValues[index] = "Y".Equals(Decrypt(index, rs[columnName]));
                        else if (clazz == typeof(System.DateTime))
                            _mOldValues[index] = Decrypt(index, (DateTime)rs[columnName]);
                        else if (DisplayType.IsLOB(dt))
                            _mOldValues[index] = Get_LOB(rs[columnName]);
                        else if (clazz == typeof(System.String))
                            _mOldValues[index] = Decrypt(index, CommonFunctions.GetString(rs[columnName]));
                        else
                            _mOldValues[index] = rs[columnName];


                    }
                    //	NULL
                    if (rs.IsNull(columnName) && _mOldValues[index] != null)
                    {
                        _mOldValues[index] = null;
                    }

                    if (VLogMgt.IsLevelAll())
                        log.Finest(index.ToString() + ": " + p_info.GetColumnName(index) + "(" + clazz + ") = " + _mOldValues[index]);
                }
                catch
                {
                    if (p_info.IsVirtualColumn(index))	//	if rs constructor used
                    {
                        log.Log(Level.FINER, "Virtual Column not loaded: " + columnName);
                    }
                    else
                    {
                        log.Log(Level.WARNING, "(rs) - " + index.ToString() + ": " + p_info.GetTableName() + "." + p_info.GetColumnName(index) + " (" + clazz + ")");
                        success = false;
                    }
                }
            }
            _mCreateNew = false;
            SetKeyInfo();
            //loadComplete(success);
            return success;
        }	//	load

        /// <summary>
        /// Load from the current position of a ResultSet
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        protected bool Load(IDataReader rs)
        {
            int size = Get_ColumnCount();
            bool success = true;
            int index = 0;
            log.Finest("(rs)");
            //  load column values
            for (index = 0; index < size; index++)
            {
                string columnName = p_info.GetColumnName(index);
                if (DatabaseType.IsMSSql && columnName.Equals("[LineNo]"))
                    columnName = "LineNo";
                if (p_info.IsVirtualColumn(index))
                {
                    try
                    {
                        string ss = rs[columnName].ToString();
                    }
                    catch (Exception e)
                    {
                        s_log.Info("error in getting Virtual column name---->" + e.Message);
                        continue;
                    }
                }

                int dt = p_info.GetColumnDisplayType(index);

                Type clazz = p_info.GetColumnClass(index);
                try
                {
                    //	NULL
                    if (Util.IsNUll(rs[columnName]))
                    {
                        _mOldValues[index] = null;
                    }
                    else
                    {
                        if (clazz == typeof(System.Int32))
                            _mOldValues[index] = Decrypt(index, Util.GetValueOfInt(rs[columnName]));
                        else if (clazz == typeof(decimal))
                            _mOldValues[index] = Decrypt(index, Util.GetNullableDecimal(rs[columnName]));
                        else if (clazz == typeof(System.Boolean))
                            _mOldValues[index] = "Y".Equals(Decrypt(index, rs[columnName]));
                        else if (clazz == typeof(System.DateTime))
                            _mOldValues[index] = Decrypt(index, (DateTime)rs[columnName]);
                        else if (DisplayType.IsLOB(dt))
                            _mOldValues[index] = Get_LOB(rs[columnName]);
                        else if (clazz == typeof(System.String))
                            _mOldValues[index] = Decrypt(index, CommonFunctions.GetString(rs[columnName]));
                        else
                            _mOldValues[index] = rs[columnName];
                    }
                    //	NULL
                    if (Util.IsNUll(rs[columnName]) && _mOldValues[index] != null)
                    {
                        _mOldValues[index] = null;
                    }

                    if (VLogMgt.IsLevelAll())
                        log.Finest(index.ToString() + ": " + p_info.GetColumnName(index) + "(" + clazz + ") = " + _mOldValues[index]);
                }
                catch
                {
                    if (p_info.IsVirtualColumn(index))	//	if rs constructor used
                    {
                        log.Log(Level.FINER, "Virtual Column not loaded: " + columnName);
                    }
                    else
                    {
                        log.Log(Level.WARNING, "(rs) - " + index.ToString() + ": " + p_info.GetTableName() + "." + p_info.GetColumnName(index) + " (" + clazz + ")");
                        success = false;
                    }
                }
            }
            _mCreateNew = false;
            SetKeyInfo();
            //loadComplete(success);
            return success;
        }	//	load

        /// <summary>
        /// Get Column Count
        /// return column count
        /// </summary>
        /// <returns></returns>
        //protected int Get_ColumnCount()
        //{
        //    return p_info.GetColumnCount();
        //}

        /// <summary>
        /// Create Single/Multi Key (Included (DateTime and Boolean)) Where Clause
        /// </summary>
        /// <param name="blnWithValues">blnWithValues if true uses actual values otherwise ?</param>
        /// <returns>where clause</returns>
        public String Get_WhereClause(bool blnWithValues)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _mIDs.Length; i++)
            {
                if (i != 0)
                    sb.Append(" AND ");
                sb.Append(_mKeyColumns[i]).Append("=");
                if (blnWithValues)
                {
                    if (_mKeyColumns[i].EndsWith("_ID"))
                        sb.Append(_mIDs[i]);
                    else if (_mIDs[i] is DateTime)
                        sb.Append(DB.TO_DATE(((DateTime)_mIDs[i]).ToLocalTime(), false));
                    else
                    {
                        sb.Append("'");
                        if (_mIDs[i] is Boolean)
                        {
                            if ((Boolean)_mIDs[i])
                            {
                                sb.Append("Y");
                            }
                            else
                            {
                                sb.Append("N");
                            }
                        }
                        else
                        {
                            sb.Append(_mIDs[i]);
                        }
                        sb.Append("'");
                    }
                }
                else
                    sb.Append("@param" + i.ToString());
            }
            return sb.ToString();
        }

        public String GetUpdateWhereClause(bool blnWithValues, int count)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _mIDs.Length; i++)
            {
                if (i != 0)
                    sb.Append(" AND ");
                sb.Append(_mKeyColumns[i]).Append("=");
                if (blnWithValues)
                {
                    if (_mKeyColumns[i].EndsWith("_ID"))
                        sb.Append(_mIDs[i]);
                    else
                        sb.Append("'").Append(_mIDs[i]).Append("'");
                }
                else
                {
                    sb.Append("@param" + count);
                    count++;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AD_Client_ID"></param>
        public void SetAD_Client_ID(int AD_Client_ID)
        {
            Set_ValueNoCheck("AD_Client_ID", (int)AD_Client_ID);
        }	//	setAD_Client_ID

        public void SetAD_Org_ID(int AD_Org_ID)
        {
            Set_ValueNoCheck("AD_Org_ID", (int)AD_Org_ID);
        }
        public void SetClientOrg(PO po)
        {
            SetClientOrg(po.GetAD_Client_ID(), po.GetAD_Org_ID());
        }
        public void SetClientOrg(int AD_Client_ID, int AD_Org_ID)
        {
            if (AD_Client_ID != GetAD_Client_ID())
                SetAD_Client_ID(AD_Client_ID);
            if (AD_Org_ID != GetAD_Org_ID())
                SetAD_Org_ID(AD_Org_ID);


        }

        // vinay bhatt window id
        private int AD_Window_ID = 0;

        public void SetAD_Window_ID(int ADWindow_ID)
        {
            AD_Window_ID = ADWindow_ID;
        }
        public int GetAD_Window_ID()
        {
            return AD_Window_ID;
        }

        // Property for Master Data Versioning
        private dynamic MasterDetails = null;
        public void SetMasterDetails(dynamic MasterDet)
        {
            MasterDetails = MasterDet;
        }
        public dynamic GetMasterDetails()
        {
            return MasterDetails;
        }

        /// <summary>
        /// Load LOB(large object)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private Object Get_LOB(Object value)
        {
            if (value == null || value == DBNull.Value)
                return null;

            Object retValue = null;
            long length = -99;
            try
            {

                //if (value  is Clob)		//	returns String
                //{
                //    Clob clob = (Clob)value;
                //    length = clob.length();
                //    retValue = clob.getSubString(1, (int)length);
                //}
                //else if (value is Blob)	//	returns byte[]
                //{
                //    Blob blob = (Blob)value;
                //    length = blob.length();
                //    int index = 1;	//	correct
                //    if (blob.getClass().getName().equals("oracle.jdbc.rowset.OracleSerialBlob"))
                //        index = 0;	//	Oracle Bug Invalid Arguments
                //                    //	at oracle.jdbc.rowset.OracleSerialBlob.getBytes(OracleSerialBlob.java:130)
                //    retValue = blob.getBytes(index, (int)length);
                //}
                if (value is System.String)
                {
                    retValue = value;
                }
                else if (value is System.Byte[])// .GetType().FullName.Equals("System.Byte[]"))	//EDB returns byte[] for blob
                {
                    retValue = value;
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown: " + value);
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "Length=" + length, e);
            }
            return retValue;
        }

        /// <summary>
        /// Set Key Info (IDs and KeyColumns).
        /// </summary>
        private void SetKeyInfo()
        {
            //	Search for Primary Key
            for (int i = 0; i < p_info.GetColumnCount(); i++)
            {
                if (p_info.IsKey(i))
                {
                    string ColumnName = p_info.GetColumnName(i);
                    _mKeyColumns = new string[] { ColumnName };
                    if (p_info.GetColumnName(i).EndsWith("_ID"))
                    {
                        int ii = CommonFunctions.GetInt(Get_Value(i));
                        //if (ii == null)
                        //	_mIDs = new Object[] {I_ZERO};
                        //else
                        _mIDs = new Object[] { ii };
                        log.Finest("(PK) " + ColumnName + "=" + ii);
                    }
                    else
                    {
                        Object oo = Get_Value(i);
                        if (oo == null)
                            _mIDs = new Object[] { null };
                        else
                            _mIDs = new Object[] { oo };
                        log.Finest("(PK) " + ColumnName + "=" + oo);
                    }
                    return;
                }
            }	//	primary key search

            //	Search for Parents
            List<String> columnNames = new List<String>();
            for (int i = 0; i < p_info.GetColumnCount(); i++)
            {
                if (p_info.IsColumnParent(i))
                    columnNames.Add(p_info.GetColumnName(i));
            }
            //	Set FKs
            int size = columnNames.Count;
            if (size == 0)
                throw new ArgumentNullException("No PK nor FK - " + p_info.GetTableName());
            _mIDs = new Object[size];
            _mKeyColumns = new string[size];
            for (int i = 0; i < size; i++)
            {
                _mKeyColumns[i] = columnNames[i];
                if (_mKeyColumns[i].EndsWith("_ID"))
                {
                    int? ii = null;
                    try
                    {
                        ii = CommonFunctions.GetInt(Get_Value(_mKeyColumns[i]));
                    }
                    catch (Exception e)
                    {
                        log.Log(Level.SEVERE, e.Message);
                    }
                    if (ii != null)
                        _mIDs[i] = ii;
                }
                else
                    _mIDs[i] = Get_Value(_mKeyColumns[i]);
                log.Finest("(FK) " + _mKeyColumns[i] + "=" + _mIDs[i]);
            }
        }	//	setKeyInfo

        /// <summary>
        /// Get Value
        /// </summary>
        public Object Get_Value(int index)
        {
            if (index < 0 || index >= Get_ColumnCount())
            {
                log.Log(Level.WARNING, "Index invalid - " + index);
                return null;
            }
            if (_mNewValues[index] != null)
            {
                if (_mNewValues[index].Equals(Null.NULL))
                    return null;
                return _mNewValues[index];
            }
            //if (_mOldValues[index].ToString() == "True" || _mOldValues[index].ToString() == "False")
            //{
            //    return  Convert.ToBoolean(_mOldValues[index].ToString());
            //}
            return _mOldValues[index];
        }   //  get_Value


        /// <summary>
        /// Set Default values.
        /// Client, Org, Created/Updated, *By, IsActive
        /// </summary>
        protected void LoadDefaults()
        {
            int size = Get_ColumnCount();
            for (int i = 0; i < size; i++)
            {
                if (p_info.IsVirtualColumn(i))
                    continue;
                string colName = p_info.GetColumnName(i);
                //  Set Standard Values
                if (colName.EndsWith("tedBy"))
                    _mNewValues[i] = p_ctx.GetAD_User_ID();
                else if (colName.Equals("Created") || colName.Equals("Updated"))
                    _mNewValues[i] = DateTime.Now;
                else if (colName.Equals(p_info.GetTableName() + "_ID"))    //  KeyColumn
                    _mNewValues[i] = 0;
                else if (colName.Equals("IsActive"))
                    _mNewValues[i] = true;
                else if (colName.Equals("AD_Client_ID"))
                    _mNewValues[i] = p_ctx.GetAD_Client_ID();
                else if (colName.Equals("AD_Org_ID"))
                    _mNewValues[i] = p_ctx.GetAD_Org_ID();
                else if (colName.Equals("Processed"))
                    _mNewValues[i] = false;
                else if (colName.Equals("Processing"))
                    _mNewValues[i] = false;
                else if (colName.Equals("Posted"))
                    _mNewValues[i] = false;
            }
        }   //  setDefaults

        /// <summary>
        /// Set Value
        /// </summary>
        public bool Set_Value(string ColumnName, object value)
        {
            if (value is System.String
                && ColumnName.Equals("WhereClause")
                && value.ToString().ToUpper().IndexOf("=NULL") != -1)
            {
                log.Warning("Invalid Null Value - " + ColumnName + "=" + value);
            }
            int index = Get_ColumnIndex(ColumnName);
            if (index < 0)
            {
                log.Log(Level.SEVERE, "Column not found - " + ColumnName);
                return false;
            }

            if (!ColumnName.Equals("Export_ID", StringComparison.OrdinalIgnoreCase))
            {
                if (ColumnName.EndsWith("_ID") && value is System.String)
                {
                    log.Warning("String converted to Integer for " + ColumnName + "=" + value);
                    value = int.Parse(value.ToString());
                }
            }

            return Set_Value(index, value);
        }   //  setValue

        /// <summary>
        ///  Get Column Index
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public int Get_ColumnIndex(string columnName)
        {
            return p_info.GetColumnIndex(columnName);
        }   //  getColumnIndex

        /// <summary>
        /// Set Value if updateable and correct class.
        ///  (and to NULL if not mandatory)
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Set_Value(int index, object value)
        {
            if (index < 0 || index >= Get_ColumnCount())
            {
                log.Log(Level.WARNING, "Index invalid - " + index);
                return false;
            }
            string ColumnName = p_info.GetColumnName(index);
            string colInfo = " - " + ColumnName;
            //
            if (p_info.IsVirtualColumn(index))
            {
                log.Log(Level.WARNING, "Virtual Column" + colInfo);
                return false;
            }
            ////
            if (!p_info.IsColumnUpdateable(index))
            {
                if (!ColumnName.Equals("Export_ID"))
                {
                    colInfo += " - NewValue=" + value + " - OldValue=" + Get_Value(index);
                    log.Log(Level.WARNING, "Column not updateable" + colInfo);
                    return false;
                }
            }
            ////
            if (value == null)
            {
                if (p_info.IsColumnMandatory(index))
                {
                    log.Log(Level.WARNING, "Cannot set mandatory column to null " + colInfo);
                    //    //	Trace.printStack();
                    return false;
                }
                _mNewValues[index] = Null.NULL;          //  correct
                log.Finer(ColumnName + " = null");
            }
            else
            {
                Type clazz = p_info.GetColumnClass(index);
                //  matching class or generic object
                if (value.GetType().Equals(clazz)
                    || clazz is System.Object)
                    _mNewValues[index] = value;     //  correct
                //  Integer can be set as BigDecimal 
                //    else if (value.GetType == BigDecimal.class 
                ////        && clazz == Integer.class)
                ////        _mNewValues[index] = new Integer (((BigDecimal)value).intValue());
                //    //	Set Boolean
                else if (clazz == typeof(System.Boolean)
                    && ("Y".Equals(value) || "N".Equals(value)))
                    _mNewValues[index] = "Y".Equals(value);
                //	Button
                else if (p_info.GetColumnDisplayType(index) == DisplayType.Button)
                {
                    if (ColumnName.EndsWith("_ID"))
                    {
                        if (value is int || value is System.Decimal)
                            _mNewValues[index] = value;
                        else
                            _mNewValues[index] = int.Parse(value.ToString());
                    }
                    else
                        _mNewValues[index] = value.ToString();
                }
                else
                {
                    log.Log(Level.SEVERE, ColumnName + " - Class invalid: " + value.ToString() + ", Should be " + clazz.ToString() + ": " + value);
                    return false;
                }
                //	Validate (Min/Max)
                string error = p_info.Validate(index, value);
                if (error != null)
                {
                    log.Log(Level.WARNING, ColumnName + "=" + value + " - " + error);
                    return false;
                }
                //	Length for String
                if (clazz == typeof(System.String))
                {
                    string stringValue = value.ToString();
                    int length = p_info.GetFieldLength(index);
                    if (stringValue.Length > length && length > 0)
                    {
                        log.Warning(ColumnName + " - Value too long - truncated to length=" + length);
                        _mNewValues[index] = stringValue.Substring(0, length);
                    }
                }
                log.Finest(ColumnName + " = " + _mNewValues[index]);
            }
            Set_Keys(ColumnName, _mNewValues[index]);
            //if (p_changeVO != null)
            //    p_changeVO.addChangedValue(ColumnName, value);
            return true;
        }   //  setValue

        /// <summary>
        /// Set (numeric) Key Value
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="value"></param>
        private void Set_Keys(string ColumnName, object value)
        {
            //	Update if KeyColumn
            for (int i = 0; i < _mIDs.Length; i++)
            {
                if (ColumnName.Equals(_mKeyColumns[i]))
                    _mIDs[i] = value;
            }	//	for all key columns
        }
        /// <summary>
        ///Get Key Columns.
        /// </summary>
        /// <returns>table name</returns>
        public String[] Get_KeyColumns()
        {
            return _mKeyColumns;
        }

        public Boolean Load(Dictionary<String, String> hmIn)
        {

            int size = Get_ColumnCount();
            Boolean success = true;
            //
            Boolean keyHasNoValue = true;
            String[] keyColumns = Get_KeyColumns();
            if (keyColumns.Length > 0)
            {
                for (int i = 0; i < keyColumns.Length; i++)
                {
                    String keyColumn = keyColumns[i];
                    String value = hmIn[keyColumn];
                    if (value != null)
                    {
                        int index1 = p_info.GetColumnIndex(keyColumn);
                        _mOldValues[index1] = value;
                    }
                    keyHasNoValue = false;
                }
            }
            //
            log.Finest("(hm)");
            int index = 0;
            //  load column values
            for (index = 0; index < size; index++)
            {
                String columnName = p_info.GetColumnName(index);

                String value = hmIn[columnName];
                if (value == null || value.Length == 0)
                    continue;
                Type clazz = p_info.GetColumnClass(index);
                int dt = p_info.GetColumnDisplayType(index);
                try
                {
                    if (clazz == typeof(System.Int32))
                        _mNewValues[index] = CommonFunctions.GetInt(value);
                    else if (clazz == typeof(System.Decimal))
                        _mNewValues[index] = Decimal.Parse(value.ToString());
                    else if (clazz == typeof(System.Boolean))
                        _mNewValues[index] = "Y".Equals(value)
                            || "on".Equals(value) || "true".Equals(value);
                    else if (clazz == typeof(System.DateTime))
                    {
                        _mNewValues[index] = Convert.ToDateTime(value);
                    }
                    else if (DisplayType.IsLOB(dt))
                        _mNewValues[index] = null;	//	get_LOB (rs.getObject(columnName));
                    else if (clazz == typeof(System.String))
                    {
                        _mNewValues[index] = value;
                        int length = p_info.GetFieldLength(index);
                        if (value.Length > length)
                        {
                            _mNewValues[index] = value.Substring(0, length);
                            log.Warning(columnName + ": truncated to length=" + length + " from=" + value);
                        }
                    }
                    else
                        _mNewValues[index] = null;	// loadSpecial(rs, index);
                    //
                    if (VLogMgt.IsLevelAll())
                        log.Finest(index.ToString() + ": " + p_info.GetColumnName(index) + "(" + clazz + ") = " + _mOldValues[index]);
                }
                catch
                {
                    if (p_info.IsVirtualColumn(index))	//	if rs constructor used
                    {
                        log.Log(Level.FINER, "Virtual Column not loaded: " + columnName);
                    }
                    else
                    {
                        log.Log(Level.SEVERE, "(ht) - " + index.ToString() + ": " + p_info.GetTableName() + "." + p_info.GetColumnName(index) + " (" + clazz + ")");
                        success = false;
                    }
                }
            }
            //	Overwrite
            //	setStandardDefaults();
            SetKeyInfo();
            LoadComplete(success);
            //
            _mCreateNew = keyHasNoValue;
            return success;
        }	//	load


        /// <summary>
        /// Get Value
        /// </summary>
        public object Get_Value(string columnName)
        {
            int index = Get_ColumnIndex(columnName);
            if (index < 0)
            {
                log.Log(Level.WARNING, "Column not found - " + columnName);
                return null;
            }
            return Get_Value(index);
        }

        /// <summary>
        ///	Get Context
        /// </summary>
        /// <returns></returns>
        public Ctx GetCtx()
        {
            return p_ctx;
        }	//

        //public Context GetCtx()
        //{
        //    return (Context)p_ctx;
        //}	//

        /// <summary>
        ///Get Trx Object
        ///Obsolete("use Get_Trx() instead")
        /// </summary>
        /// <returns></returns>
        /// 
        public Trx Get_TrxName()
        {
            return _trx;
        }	//	getTrx

        public Trx Get_Trx()
        {
            return _trx;
        }	//	getTrx

        /// <summary>
        /// set Transaction Object
        /// [Obsolete ("use Set_Trx() instead")]
        /// </summary>
        /// <param name="trxName"></param>


        public void Set_TrxName(Trx trxName)
        {
            _trx = trxName;
        }

        public void Set_Trx(Trx trx)
        {
            _trx = trx;
        }

        /// <summary>
        /// Get AD_Client
        /// </summary>
        /// <returns></returns>
        public int GetAD_Client_ID()
        {
            object oo = Get_Value("AD_Client_ID");
            if (oo == null)
                return 0;
            return int.Parse(oo.ToString());
        }	//	getAD_C

        /// <summary>
        /// Get AD_Org
        /// </summary>
        /// <returns></returns>
        public int GetAD_Org_ID()
        {
            object oo = Get_Value("AD_Org_ID");
            if (oo == null)
                return 0;
            return int.Parse(oo.ToString());
        }	//	getAD_Org_ID

        public int Get_ID()
        {
            Object oo = _mIDs[0];
            if (oo != null && oo is int)
                return (int.Parse(oo.ToString()));
            return 0;
        }

        public void SetNodeParentID(int parent_ID)
        {
            this.parent_ID = parent_ID;
        }

        public int GetNodeParentID()
        {
            return parent_ID;
        }

        /// <summary>
        /// Set Document Value WF Message
        /// </summary>
        /// <param name="msg"></param>
        public void SetDocWFMsg(string msg)
        {
            this.s_docWFMsg = msg;
        }

        /// <summary>
        /// Get Document Value WF Message
        /// </summary>
        /// <returns></returns>
        public string GetDocWFMsg()
        {
            return s_docWFMsg;
        }

        public bool Set_ValueNoCheck(String ColumnName, Object value)
        {
            bool success = true;
            int index = Get_ColumnIndex(ColumnName);
            if (index < 0)
            {
                log.Log(Level.SEVERE, "Column not found - " + ColumnName);
                return false;
            }

            if (value == null)
            {
                _mNewValues[index] = Null.NULL;
            }
            // Case handled for DB null
            else if (value is DBNull)
            {
                _mNewValues[index] = null;
            }
            else
            {
                Type clazz = p_info.GetColumnClass(index);
                if (value.GetType().Equals(clazz) || clazz is System.Object)
                {
                    _mNewValues[index] = value;
                }
                else if (clazz == typeof(Boolean) && ("Y".Equals(value) || "N".Equals(value)))
                    _mNewValues[index] = "Y".Equals(value);
                else
                {
                    log.Warning(ColumnName + " - Class invalid: " + value.ToString() + ", Should be " + clazz.ToString() + ": " + value);
                    _mNewValues[index] = value;
                }

                string error = p_info.Validate(index, value);
                if (error != null)
                {
                    success = false;
                    log.Warning(ColumnName + "=" + value + " - " + error);
                }
                if (clazz == typeof(System.String))
                {
                    String stringValue = value.ToString();
                    int length = p_info.GetFieldLength(index);
                    if (stringValue.Length > length && length > 0)
                    {
                        log.Warning(ColumnName + " - Value too long - truncated to length=" + length);
                        _mNewValues[index] = stringValue.Substring(0, length);
                    }
                }
            }
            log.Finest(ColumnName + " = " + _mNewValues[index] + " (" + (_mNewValues[index] == null ? "-" : _mNewValues[index].ToString()) + ")");
            Set_Keys(ColumnName, _mNewValues[index]);
            return success;
        }

        protected static int I_ZERO = 0;



        /// <summary>
        /// Is new record
        /// check all value fo multikey table
        /// </summary>
        /// <returns>true if new</returns>
        public bool Is_New()
        {
            if (_mCreateNew)
                return true;

            for (int i = 0; i < _mIDs.Length; i++)
            {
                if (_mIDs[i].Equals(I_ZERO) || _mIDs[i] == null || _mIDs[i] == Null.NULL) //match non zero and non- null value
                    continue;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Is value changed of column
        /// </summary>
        /// <returns></returns>
        public bool Is_Changed()
        {
            int size = Get_ColumnCount();
            for (int i = 0; i < size; i++)
            {
                if (_mNewValues[i] != null)
                    return true;	//	something changed
            }
            return false;
        }

        /// <summary>
        /// Record is active or not
        /// </summary>
        /// <returns>tru if active</returns>
        public bool IsActive()
        {
            bool? bb = (bool?)Get_Value("IsActive");
            if (bb != null)
                return bb.Value;
            return false;
        }

        public int Get_Table_ID()
        {
            return p_info.getAD_Table_ID();
        }

        #region "SAVE Function"
        //*************Methods for Save*************************************************


        /// <summary>
        /// Save Special Data to be extended by sub-classes
        /// </summary>
        /// <param name="value">value value</param>
        /// <param name="index">index index</param>
        /// <returns>SQL code for INSERT VALUES clause</returns>
        protected String SaveNewSpecial(Object value, int index)
        {
            String colName = p_info.GetColumnName(index);
            String colClass = p_info.GetColumnClass(index).ToString();
            String colValue = value == null ? "null" : value.GetType().ToString();
            //	int dt = p_info.getColumnDisplayType(index);

            if (value == null)
                return "NULL";
            return value.ToString();
        }



        /// <summary>
        /// Get ID for new record during save. You can overwite this to explicitly set the ID
        /// </summary>
        /// <returns>ID to be used or 0 for fedault logic</returns>
        protected int SaveNew_GetID()
        {
            return 0;
        }


        public void Set_CustomColumn(String columnName, Object value)
        {
            if (m_custom == null)
                m_custom = new Dictionary<String, String>();
            String valueString = "NULL";

            if (value is int)
                valueString = value.ToString();
            else if (value is Boolean)
                valueString = ((Boolean)value) ? "'Y'" : "'N'";
            //else if (value == typeof(DateTime))
            //    valueString = DB.TO_DATE((Timestamp)value, false);
            else //	if (value is String)
                valueString = GlobalVariable.TO_STRING(value.ToString());
            //	Save it
            log.Log(Level.INFO, columnName + "=" + valueString);
            if (m_custom.ContainsKey(columnName))
                m_custom[columnName] = valueString;
            else
                m_custom.Add(columnName, valueString);
        }	//	set_CustomColumn


        private Dictionary<String, String> m_custom = null;
        private static Dictionary<int, StringBuilder> logbatch = new Dictionary<int, StringBuilder>();

        private const string TYPE_DATE_TIME = "System.DateTime";
        private const string TYPE_INT32 = "System.Int32";
        private const string TYPE_DECIMAL = "System.Decimal";
        private const string TYPE_BOOLEAN = "System.Boolean";
        private const string TYPE_OBJECT = "System.Object";

        public bool SaveNewInsertSQL()
        {
            //	Change Log
            //MSession session = MSession.Get(GetCtx(), false);
            int AD_ChangeLog_ID = 0;
            // Change Log
            MSession session = null;
            if (!(this is MSession))
                session = MSession.Get(p_ctx);

            bool logThis = session != null;
            if (logThis)
                logThis = session.IsLogged(p_info.getAD_Table_ID(),
                        Get_TableName(), X_AD_ChangeLog.CHANGELOGTYPE_Insert);

            //	SQL
            StringBuilder strSqlInsert = new StringBuilder("INSERT INTO ");
            strSqlInsert.Append(p_info.GetTableName()).Append(" (");
            StringBuilder strSqlValues = new StringBuilder(") VALUES (");
            int iSize = Get_ColumnCount();
            bool blnDoComma = false;


            System.Globalization.CultureInfo original = System.Threading.Thread.CurrentThread.CurrentCulture;

            System.Threading.Thread.CurrentThread.CurrentCulture = Env.GetLanguage(p_ctx).GetCulture(Env.GetBaseAD_Language());
            System.Threading.Thread.CurrentThread.CurrentUICulture = Env.GetLanguage(p_ctx).GetCulture(Env.GetBaseAD_Language());


            for (int i = 0; i <= iSize; i++)
            {
                if (p_info.IsVirtualColumn(i))
                    continue;

                Object value = Get_Value(i);
                //Dont Insert Null values(allow database defaults)
                if (value == null || value == Null.NULL)
                    continue;

                int dt = p_info.GetColumnDisplayType(i);

                if (DisplayType.IsLOB(dt))
                {
                    LOBAdd(value, i, dt);
                    continue;
                }
                if (blnDoComma)
                {
                    strSqlInsert.Append(",");
                    strSqlValues.Append(",");
                }
                else
                    blnDoComma = true;
                String strColumnName = p_info.GetColumnName(i);
                strSqlInsert.Append(strColumnName);

                //  Based on class of definition, not class of value
                Type type = p_info.GetColumnClass(i);
                try
                {
                    if (type == typeof(Object) && dt != DisplayType.Label) //  may have need to deal with null values differently
                    {
                        strSqlValues.Append(SaveNewSpecial(value, i));
                    }
                    else if (value == null || value == Null.NULL)
                        strSqlValues.Append("NULL");
                    else if (value.GetType().ToString() == TYPE_INT32 || value.GetType().ToString() == TYPE_DECIMAL)
                        strSqlValues.Append(Encrypt(i, value));
                    else if (type == typeof(bool))
                    {
                        bool bValue = false;
                        if (value.GetType().ToString() == TYPE_BOOLEAN)
                            bValue = ((bool)value);
                        else
                            bValue = "Y".Equals(value);
                        strSqlValues.Append(Encrypt(i, bValue ? "'Y'" : "'N'"));
                    }
                    else if (value.GetType().ToString() == TYPE_BOOLEAN)
                        strSqlValues.Append(Encrypt(i, ((bool)value) ? "'Y'" : "'N'"));
                    else if (value.GetType().ToString() == TYPE_DATE_TIME)
                        strSqlValues.Append(GlobalVariable.TO_DATE((DateTime?)Encrypt(i, value), p_info.GetColumnDisplayType(i) == DisplayType.Date));
                    else if (type == typeof(string) || dt == DisplayType.Label)
                        strSqlValues.Append(Encrypt(i, GlobalVariable.TO_STRING((string)value)));
                    else if (DisplayType.IsLOB(dt))
                        strSqlValues.Append("null");		//	no db dependent stuff here
                    else
                        strSqlValues.Append(SaveNewSpecial(value, i));


                    //strSqlValues.Append (encrypt(i,value));
                }
                catch
                {
                    String msg = "";
                    if (_trx != null)
                        msg = "[" + _trx.GetTrxName() + "] - ";
                    msg += p_info.ToString()
                        + " - Value=" + value
                        + "(" + (value == null ? "null" : value.ToString()) + ")";
                    log.Log(Level.SEVERE, msg);
                }

                //	Change Log
                if (_mIDs.Length == 1 && session != null && logThis
                        && !p_info.IsEncrypted(i)		//	not encrypted
                        && !p_info.IsVirtualColumn(i)	//	no virtual column
                        && !"Password".Equals(strColumnName)
                        && !"CreditCardNumber".Equals(strColumnName)
                )
                {
                    Object keyInfo = Get_ID();
                    if (_mIDs.Length != 1)
                        keyInfo = Get_WhereClause(true);
                    if ((this.Get_TableName() != "AD_PInstance_Para") && (this.Get_TableName() != "AD_PInstance"))
                    {
                        MChangeLog cLog = session.ChangeLog(
                                _trx, AD_ChangeLog_ID,
                                p_info.getAD_Table_ID(), p_info.GetColumn(i).AD_Column_ID,
                                keyInfo, GetAD_Client_ID(), GetAD_Org_ID(), null, value,
                                Get_TableName(), X_AD_ChangeLog.CHANGELOGTYPE_Insert);
                        if (cLog != null)
                            AD_ChangeLog_ID = cLog.GetAD_ChangeLog_ID();
                    }
                }

            }

            System.Threading.Thread.CurrentThread.CurrentCulture = original;// Env.GetLanguage(GetCtx()).GetCulture(Env.GetLoginLanguage(p_ctx).GetAD_Language());
            System.Threading.Thread.CurrentThread.CurrentUICulture = original;// Env.GetLanguage(GetCtx()).GetCulture(Env.GetLoginLanguage(p_ctx).GetAD_Language());
            //custom columns

            int index;
            if (m_custom != null)
            {

                foreach (KeyValuePair<string, string> kvp in m_custom)
                {
                    String columnName = kvp.Key;
                    index = p_info.GetColumnIndex(columnName);
                    String value = kvp.Value;
                    if (string.IsNullOrEmpty(value))
                        continue;
                    if (blnDoComma)
                    {
                        strSqlInsert.Append(",");
                        strSqlValues.Append(",");
                    }
                    else
                        blnDoComma = true;

                    strSqlInsert.Append(columnName);
                    String value2 = DB.TO_STRING((string)Encrypt(index, value));
                    strSqlValues.Append(value2);


                    if (_mIDs.Length == 1 && session != null
                        && !p_info.IsEncrypted(index)		//	not encrypted
                        && !p_info.IsVirtualColumn(index)	//	no virtual column
                        && !"Password".Equals(columnName)
                        )
                    {
                        MChangeLog cLog = session.ChangeLog(
                            _trx, AD_ChangeLog_ID,
                            p_info.getAD_Table_ID(), p_info.GetColumn(index).AD_Column_ID,
                            _mIDs, GetAD_Client_ID(), GetAD_Org_ID(), null, value,
                            Get_TableName(), MChangeLog.CHANGELOGTYPE_Insert);
                        if (cLog != null)
                            AD_ChangeLog_ID = cLog.GetAD_ChangeLog_ID();
                    }
                }
                m_custom = null;

            }
            strSqlInsert.Append(strSqlValues).Append(")");
            int no = -1;
            bool ok;
            try
            {
                StringBuilder adLog = null;
                //string adLog = p_ctx.GetContext("AD_ChangeLogBatch");
                if (session != null && logbatch.ContainsKey(session.GetAD_Session_ID()))
                {
                    adLog = logbatch[session.GetAD_Session_ID()];
                }
                else if (session != null)
                {
                    adLog = new StringBuilder();
                    logbatch[session.GetAD_Session_ID()] = adLog;
                }




                if (p_info.GetTableName() == "AD_ChangeLog")
                {
                    if ((adLog == null || adLog.Length <= 0) && !DB.IsPostgreSQL())
                    {
                        adLog.Append("BEGIN ");
                    }

                    if (DB.IsPostgreSQL())
                    {
                        //p_ctx.SetContext("AD_ChangeLogBatch", adLog + " SELECT ExecuteImmediate('" + strSqlInsert.Replace("'", "''") + "');");
                        adLog.Append(" SELECT ExecuteImmediate('" + strSqlInsert.Replace("'", "''") + "');");
                    }
                    else
                        //p_ctx.SetContext("AD_ChangeLogBatch", adLog + "execute immediate('" + strSqlInsert.ToString().Replace("'", "''") + "');");
                        adLog.Append("execute immediate('" + strSqlInsert.ToString().Replace("'", "''") + "');");
                    //no = 1;
                    return true;
                }
                else
                {
                    if (adLog != null && adLog.Length > 6)
                    {
                        if (!DB.IsPostgreSQL())
                        {
                            adLog.Append(" END; ");
                        }

                        //p_ctx.SetContext("AD_ChangeLogBatch", "");
                        adLog.Replace("TO_DATE", "TO_TIMESTAMP");
                        no = DB.ExecuteQuery(adLog.ToString(), null, _trx);
                        adLog.Clear();
                    }
                    else
                    {
                        no = 1;
                    }
                    //if (no == 1)
                    no = DB.ExecuteQuery(strSqlInsert.ToString(), null, _trx);
                }
                ok = (no == 1);
            }
            catch (Exception ex)
            {
                log.Info(strSqlInsert.ToString());
                log.SaveError("DBExecuteError", ex);
                //Common.//ErrorLog.FillErrorLog("POSaveNew", strSqlInsert.ToString(), ex.Message, VAdvantage.Framework.Message.MessageType.ERROR);
                ok = false;
            }
            if (ok)
            {
                ok = LobSave();
                if (!Load(_trx))
                {
                    if (_trx == null)
                        log.Log(Level.SEVERE, "reloading");
                    else
                        log.Log(Level.SEVERE, "[" + _trx.GetTrxName() + "] - reloading");
                    ok = false;
                }
            }
            else
            {
                string msg = "Not inserted";
                if (VLogMgt.IsLevelFiner())
                    msg += strSqlInsert.ToString();
                else
                    msg += Get_TableName();
                if (_trx == null)
                    log.Log(Level.WARNING, msg);
                else
                    log.Log(Level.WARNING, "[" + _trx.GetTrxName() + "]" + msg);
            }
            return ok;
        }

        //private bool m_hasKeyColumn = false;

        /// <summary>
        /// Update Value or create new record.
        /// To reload call load() - not updated
        /// </summary>
        /// <returns>true if saved</returns>
        private bool saveNew()
        {
            //trans = DB.GerServerTransaction();

            if (_mIDs.Length == 1 && p_info.hasKeyColumn() && _mKeyColumns[0].EndsWith("_ID"))	//	AD_Language, EntityType
            {
                int no = SaveNew_GetID();
                if (no <= 0)
                {
                    no = POActionEngine.Get().GetNextID(GetAD_Client_ID(), p_info.GetTableName(), Get_Trx());


                    //if (DatabaseType.IsOracle)
                    //    no = MSequence.GetNextIDOracle(GetAD_Client_ID(), p_info.GetTableName(), Get_Trx());
                    //else if (DatabaseType.IsPostgre)
                    //    no = MSequence.GetNextIDPostgre(GetAD_Client_ID(), p_info.GetTableName(), Get_Trx());//, Get_Trx());
                    //else if (DatabaseType.IsMySql)
                    //    no = MSequence.GetNextIDMySql(GetAD_Client_ID(), p_info.GetTableName());//, Get_Trx());
                    //else if (DatabaseType.IsMSSql)
                    //    no = MSequence.GetNextIDMSSql(GetAD_Client_ID(), p_info.GetTableName());//, Get_Trx());                }
                }
                if (no <= 0)
                {
                    log.Severe("No NextID (" + no + ")");
                    return SaveFinish(true, false);
                }
                _mIDs[0] = no;
                Set_ValueNoCheck(_mKeyColumns[0], _mIDs[0]);
            }

            if (_trx == null)
                log.Fine(p_info.GetTableName() + " - " + Get_WhereClause(true));
            else
                log.Fine("[" + _trx.GetTrxName() + "] - " + p_info.GetTableName() + " - " + Get_WhereClause(true));

            dynamic masDet = GetMasterDetails();

            //	Set new DocumentNo
            String columnName = "DocumentNo";
            int index = p_info.GetColumnIndex(columnName);
            if (index != -1)
            {
                // String value = (String)Get_Value(index);
                // Commented  As it will give error in case of Integer value

                String value = "";
                if ((Get_Value(index)) != null)
                {
                    value = Util.GetValueOfString(Get_Value(index));
                }
                else
                {
                    value = null;
                }
                if (value != null && value.StartsWith("<") && value.EndsWith(">"))
                    value = null;
                if (value == null || value.Length == 0)
                {
                    int docTypeId = -1;
                    int dt = p_info.GetColumnIndex("C_DocTypeTarget_ID");
                    if (dt == -1)
                        dt = p_info.GetColumnIndex("C_DocType_ID");
                    if (dt != -1)       //	get based on Doc Type (might return null)
                        
                            docTypeId = get_ValueAsInt(dt);


                    //value = MSequence.GetDocumentNo(get_ValueAsInt(dt), _trx, GetCtx());
                    //value = MSequence.GetDocumentNo(get_ValueAsInt(dt), _trx, GetCtx(), false, this);
                    value = POActionEngine.Get().GetDocumentNo(docTypeId, this);
                    //if (value == null)  //	not overwritten by DocType and not manually entered
                    //{
                    //    if (masDet != null && masDet.TableName != null && masDet.TableName != "")
                    //    {
                    //        // Handled to get DocumentNo based on Organization
                    //        //value = MSequence.GetDocumentNo(GetAD_Client_ID(), masDet.TableName, _trx, GetCtx());
                    //        value = MSequence.GetDocumentNo(masDet.TableName, _trx, GetCtx(), this);
                    //    }
                    //    else
                    //    {
                    //        // Handled to get DocumentNo based on Organization
                    //        //value = MSequence.GetDocumentNo(GetAD_Client_ID(), p_info.GetTableName(), _trx, GetCtx());
                    //        value = MSequence.GetDocumentNo(p_info.GetTableName(), _trx, GetCtx(), this);
                    //    }
                    //}
                    Set_ValueNoCheck(columnName, value);
                }
            }

            //	Set empty Value
            columnName = "Value";
            index = p_info.GetColumnIndex(columnName);
            if (index != -1)
            {
                String value = (String)Get_Value(index);
                if (value == null || value.Length == 0)
                {
                    //value = MSequence.GetDocumentNo(GetAD_Client_ID(), p_info.GetTableName(), _trx, GetCtx());
                    //if (masDet != null && masDet.TableName != null && masDet.TableName != "")
                    //    value = MSequence.GetDocumentNo(masDet.TableName, _trx, GetCtx(), this);
                    //else
                    //    // Handled to get Search Key based on Organization same as Document No.
                    //    value = MSequence.GetDocumentNo(p_info.GetTableName(), _trx, GetCtx(), this);
                    
                    value = POActionEngine.Get().GetDocumentNo(this);
                    Set_ValueNoCheck(columnName, value);
                }
            }

            LobReset();
            bool ok = SaveNewInsertSQL();
            return SaveFinish(true, ok);

        }

        /// <summary>
        /// Update Record directly
        /// </summary>
        /// <returns>true if updated</returns>
        protected bool SaveUpdate()
        {
            String strWhere = Get_WhereClause(true);
            bool blnChanges = false;
            StringBuilder sql = new StringBuilder("UPDATE ");
            sql.Append(p_info.GetTableName()).Append(" SET ");
            bool blnUpdated = false;
            bool blnUpdatedBy = false;
            LobReset();
            //Change Log
            // MSession session = MSession.Get(GetCtx(), true);

            // Change Log
            MSession session = null;
            if (!(this is MSession))
                session = MSession.Get(p_ctx);
            bool logThis = session != null;
            if (logThis)
                logThis = session.IsLogged(p_info.getAD_Table_ID(),
                        Get_TableName(), X_AD_ChangeLog.CHANGELOGTYPE_Update);

            int AD_ChangeLog_ID = 0;

            System.Globalization.CultureInfo original = System.Threading.Thread.CurrentThread.CurrentCulture;

            System.Threading.Thread.CurrentThread.CurrentCulture = Env.GetLanguage(p_ctx).GetCulture(Env.GetBaseAD_Language());
            System.Threading.Thread.CurrentThread.CurrentUICulture = Env.GetLanguage(p_ctx).GetCulture(Env.GetBaseAD_Language());

            int iSize = Get_ColumnCount();
            for (int i = 0; i < iSize; i++)
            {
                Object value = _mNewValues[i];
                if (value == null
                    || p_info.IsVirtualColumn(i))
                    continue;
                //we have a change
                Type type = p_info.GetColumnClass(i);
                int dt = p_info.GetColumnDisplayType(i);
                String columnName = p_info.GetColumnName(i);

                if (columnName.Equals("UpdatedBy"))
                {
                    if (blnUpdatedBy)	//	explicit
                        continue;
                    blnUpdatedBy = true;

                }
                else if (columnName.Equals("Updated"))
                {
                    if (blnUpdated)
                        continue;
                    blnUpdated = true;
                }
                if (DisplayType.IsLOB(dt))
                {
                    LOBAdd(value, i, dt);
                    if (!blnChanges & !blnUpdatedBy)
                    {
                        int AD_User_ID = p_ctx.GetAD_User_ID();
                        Set_ValueNoCheck("UpdatedBy", (int)AD_User_ID);
                        sql.Append("UpdatedBy=").Append(AD_User_ID);
                        blnChanges = true;
                        blnUpdatedBy = true;
                    }
                    continue;
                }

                //	Update Document No
                if (columnName.Equals("DocumentNo"))
                {
                    String strValue = Util.GetValueOfString(value);
                    if (strValue.StartsWith("<") && strValue.EndsWith(">"))
                    {
                        value = null;
                        int AD_Client_ID = GetAD_Client_ID();
                        int docId = -1;
                        int index = p_info.GetColumnIndex("C_DocTypeTarget_ID");
                        if (index == -1)
                            index = p_info.GetColumnIndex("C_DocType_ID");
                        if (index != -1)		//	get based on Doc Type (might return null)
                            docId = Get_ValueAsInt(index);
                        //if (index != -1)		//	get based on Doc Type (might return null)
                        //    //value = MSequence.GetDocumentNo(get_ValueAsInt(index), _trx, GetCtx());
                        //    value = MSequence.GetDocumentNo(get_ValueAsInt(dt), _trx, GetCtx(), false, this);
                        //if (value == null)	//	not overwritten by DocType and not manually entered
                        //    value = MSequence.GetDocumentNo(AD_Client_ID, p_info.GetTableName(), _trx, GetCtx());
                        value = POActionEngine.Get().GetDocumentNo(docId, this);
                    }
                    else
                        log.Warning("DocumentNo updated: " + _mOldValues[i] + " -> " + value);

                }

                if (blnChanges)
                    sql.Append(", ");
                blnChanges = true;
                sql.Append(columnName).Append("=");

                if (value == Utility.Null.NULL)
                {
                    sql.Append("NULL");
                }
                else if (value is int || value is decimal)
                    sql.Append(Encrypt(i, value));
                else if (type == typeof(System.Boolean))
                {
                    bool blnValue = false;
                    if (value is bool)
                        blnValue = (bool)value;
                    else
                        blnValue = "Y".Equals(value);

                    sql.Append(Encrypt(i, blnValue ? "'Y'" : "'N'"));
                }
                else if (value.GetType().ToString() == TYPE_DATE_TIME)
                    sql.Append(GlobalVariable.TO_DATE((DateTime?)Encrypt(i, value), p_info.GetColumnDisplayType(i) == DisplayType.Date));
                else if (type == typeof(string) || dt == DisplayType.Label)
                    sql.Append(Encrypt(i, GlobalVariable.TO_STRING(value.ToString())));
                else
                    sql.Append(SaveNewSpecial(value, i));



                //Change Log
                if (_mIDs.Length == 1 && (session != null) && logThis
                   && !p_info.IsEncrypted(i)		//	not encrypted
                   && !p_info.IsVirtualColumn(i)	//	no virtual column
                   && !"Password".Equals(columnName)
                   && !"CreditCardNumber".Equals(columnName)
                   )
                {
                    Object oldV = _mOldValues[i];
                    Object newV = value;
                    if ((oldV != null) && (oldV == Null.NULL))
                        oldV = null;
                    if ((newV != null) && (newV == Null.NULL))
                        newV = null;
                    //
                    Object keyInfo = Get_ID();
                    if (_mIDs.Length != 1)
                        keyInfo = Get_WhereClause(true);

                    if ((this.Get_TableName() != "AD_PInstance_Para") && (this.Get_TableName() != "AD_PInstance"))
                    {

                        MChangeLog cLog = session.ChangeLog(
                                _trx, AD_ChangeLog_ID,
                                p_info.getAD_Table_ID(), p_info.GetColumn(i).AD_Column_ID,
                                keyInfo, GetAD_Client_ID(), GetAD_Org_ID(), oldV, newV,
                                Get_TableName(), X_AD_ChangeLog.CHANGELOGTYPE_Update);
                        if (cLog != null)
                            AD_ChangeLog_ID = cLog.GetAD_ChangeLog_ID();
                    }
                    // System.Threading.Thread.CurrentThread.CurrentCulture = Env.GetLanguage(p_ctx).GetCulture(Env.GetBaseAD_Language());
                    //  System.Threading.Thread.CurrentThread.CurrentUICulture = Env.GetLanguage(p_ctx).GetCulture(Env.GetBaseAD_Language());
                }
            }

            //	Custom Columns (cannot be logged as no column)
            if (m_custom != null)
            {
                //to do 
            }

            // System.Threading.Thread.CurrentThread.CurrentCulture = Env.GetLanguage(GetCtx()).GetCulture(Env.GetLoginLanguage(p_ctx).GetAD_Language());
            // System.Threading.Thread.CurrentThread.CurrentUICulture = Env.GetLanguage(GetCtx()).GetCulture(Env.GetLoginLanguage(p_ctx).GetAD_Language());

            System.Threading.Thread.CurrentThread.CurrentCulture = original;
            System.Threading.Thread.CurrentThread.CurrentUICulture = original;

            //	Something changed
            if (blnChanges)
            {
                if (_trx == null)
                    log.Fine(p_info.GetTableName() + "." + strWhere);
                else
                    log.Fine("[" + _trx.GetTrxName() + "] - " + p_info.GetTableName() + "." + strWhere);

                if (!blnUpdated)	//	Updated not explicitly set
                {
                    DateTime now = DateTime.Now;
                    Set_ValueNoCheck("Updated", (DateTime)now);
                    sql.Append(",Updated=").Append(GlobalVariable.TO_DATE(now, false));
                }
                if (!blnUpdatedBy)	//	UpdatedBy not explicitly set
                {
                    int AD_User_ID = p_ctx.GetAD_User_ID();
                    Set_ValueNoCheck("UpdatedBy", (int)AD_User_ID);
                    sql.Append(",UpdatedBy=").Append(AD_User_ID);
                }
                sql.Append(" WHERE ").Append(strWhere);

                log.Finest(sql.ToString());

                int no = -1;
                bool ok;
                try
                {
                    //string adLog = p_ctx.GetContext("AD_ChangeLogBatch");
                    StringBuilder adLog = null;
                    if (session != null && logbatch.ContainsKey(session.GetAD_Session_ID()))
                    {
                        adLog = logbatch[session.GetAD_Session_ID()];
                    }

                    if (adLog != null && adLog.Length > 6)
                    {
                        if (!DB.IsPostgreSQL())
                        {
                            adLog.Append(" END; ");
                        }

                        //p_ctx.SetContext("AD_ChangeLogBatch", "");
                        //logbatch[session.GetAD_Session_ID()].Clear();
                        adLog.Replace("TO_DATE", "TO_TIMESTAMP");
                        no = DB.ExecuteQuery(adLog.ToString(), null, _trx);
                        adLog.Clear();
                    }
                    else
                    {
                        no = 1;
                    }
                    //if (no == 1)
                    no = DB.ExecuteQuery(sql.ToString(), null, _trx);
                    ok = (no == 1);
                }
                catch
                {
                    ok = false;
                }
                if (ok)
                    ok = LobSave();
                else
                {
                    if (_trx == null)
                        log.Log(Level.WARNING, "#" + "Records-" + no.ToString() + " - " + p_info.GetTableName() + "." + strWhere);
                    else
                        log.Log(Level.WARNING, "#" + "Records-" + no.ToString()
                            + " - [" + _trx.GetTrxName() + "] - " + p_info.GetTableName() + "." + strWhere);
                }

                return SaveFinish(false, ok);

            }
            return SaveFinish(false, true);
        }


        private bool SaveFinish(bool newRecord, bool success)
        {
            if (success)
            {
                if (newRecord)
                    InsertTranslations();
                else
                    UpdateTranslations();

                UpdatePreferences();
                // return true;
            }

            try
            {
                success = AfterSave(newRecord, success);
                POActionEngine.Get().AfterSave(newRecord, success, this);
                //if (success && newRecord)
                //    InsertTreeNode();
            }
            catch (Exception ex)
            {
                log.Log(Level.WARNING, "afterSave" + " " + ex.Message);
                log.SaveError("Error", ex.Message, false);
                success = false;
            }

            // Case for Master Data Versioning, check if the record being saved is in Version table
            //if (GetTable().ToLower().EndsWith("_ver"))
            //{
            //    // Get Master Data Properties
            //    var MasterDetails = GetMasterDetails();
            //    // check if Record has any Workflow (Value Type) linked, or Is Immediate save etc
            //    if (MasterDetails != null && MasterDetails.AD_Table_ID > 0 &&
            //        (MasterDetails.ImmediateSave || MasterDetails.IsLatestVersion) && !MasterDetails.HasDocValWF)
            //    {
            //        // create object of parent table
            //        MTable tbl = MTable.Get(p_ctx, MasterDetails.AD_Table_ID);
            //        PO po = null;
            //        bool updateMasID = false;
            //        // check if Master table has single key or multiple keys or single key
            //        // then create object 
            //        if (tbl.IsSingleKey())
            //        {
            //            po = tbl.GetPO(p_ctx, MasterDetails.Record_ID, _trx);
            //            if (po.Get_ID() <= 0)
            //                updateMasID = true;
            //        }
            //        else
            //        {
            //            // fetch key columns for parent table
            //            string[] keyCols = tbl.GetKeyColumns();
            //            StringBuilder whereCond = new StringBuilder("");
            //            for (int w = 0; w < keyCols.Length; w++)
            //            {
            //                if (w == 0)
            //                {
            //                    if (keyCols[w] != null)
            //                        whereCond.Append(keyCols[w] + " = " + Get_Value(keyCols[w]));
            //                    else
            //                        whereCond.Append(" COALESCE(" + keyCols[w] + ",0) = 0");
            //                }
            //                else
            //                {
            //                    if (keyCols[w] != null)
            //                        whereCond.Append(" AND " + keyCols[w] + " = " + Get_Value(keyCols[w]));
            //                    else
            //                        whereCond.Append(" AND COALESCE(" + keyCols[w] + ",0) = 0");
            //                }
            //            }
            //            po = tbl.GetPO(p_ctx, whereCond.ToString(), _trx);
            //        }
            //        if (po != null)
            //        {
            //            po.SetAD_Window_ID(MasterDetails.AD_Window_ID);
            //            // copy date from Version table to Master table
            //            bool saveSuccess = CopyVersionToMaster(po);
            //            if (!saveSuccess)
            //            {
            //                if (_trx != null)
            //                {
            //                    _trx.Rollback();
            //                    _trx.Close();
            //                    return false;
            //                }
            //            }
            //            else
            //            {
            //                if (updateMasID)
            //                {
            //                    MasterDetails.Record_ID = po.Get_ID();
            //                    // set new values in MaserDetails Object
            //                    SetMasterDetails(MasterDetails);
            //                    // Update key column in version table against Master table 
            //                    // only in case of single key column and in case of new record
            //                    string sqlQuery = "UPDATE " + GetTable() + " SET " + po.Get_TableName() + "_ID = " + po.Get_ID() + " WHERE " + GetTable() + "_ID = " + Get_ID();
            //                    int count = DB.ExecuteQuery(sqlQuery, null, _trx);
            //                }
            //            }
            //        }
            //    }
            //}

            if (success)
            {
                //if (s_docWFMgr == null)
                //{
                //    try
                //    {
                //        //s_docWFMgr = (DocWorkflowMgr)Activator.GetObject(typeof(DocWorkflowMgr), "VAdvantage.WF.DocWorkflowManager");
                //        System.Reflection.Assembly asm = System.Reflection.Assembly.Load("VAWorkflow");
                //        s_docWFMgr = (DocWorkflowMgr)asm.GetType("VAdvantage.WF.DocWorkflowManager").GetMethod("Get").Invoke(null, null);
                //        //s_docWFMgr = DocWorkflowManager.Get();
                //    }
                //    catch
                //    {
                //    }
                //}
                //if (s_docWFMgr != null)
                //    s_docWFMgr.Process(this, p_info.getAD_Table_ID());
                ExecuteWF();
                //	Copy to Old values
                int size = p_info.GetColumnCount();
                for (int i = 0; i < size; i++)
                {
                    if (_mNewValues[i] != null)
                    {
                        if (_mNewValues[i] == Null.NULL)
                            _mOldValues[i] = null;
                        else
                            _mOldValues[i] = _mNewValues[i];
                    }
                }
                _mNewValues = new Object[size];
            }


            _mCreateNew = false;
            if (!newRecord)
                CacheMgt.Get().Reset(p_info.GetTableName());
            //return false;
            return success;


        }

        /// <summary>
        /// Copy record from Version table to Master table
        /// </summary>
        /// <param name="po"></param>
        /// <returns></returns>
        private bool CopyVersionToMaster(PO po)
        {
            int count = Get_ColumnCount();
            for (int i = 0; i < count; i++)
            {
                string columnName = Get_ColumnName(i);
                // skip column if column name is either "Created" or "CreatedBy"
                if (columnName.Trim().ToLower() == "created" || columnName.Trim().ToLower() == "createdby")
                    continue;
                if (po.Get_ColumnIndex(columnName) < 0)
                    continue;

                if (po.Get_Value(columnName) != Get_ValueOld(columnName))
                {
                    po.Set_ValueNoCheck(columnName, Get_Value(columnName));
                }
            }

            if (!po.Save())
                return false;

            return true;
        }

        protected virtual bool AfterSave(bool newRecord, bool success)
        {
            return success;
        }

        private void UpdatePreferences()
        {
            if (!IsActive())
            {
                String keyColumn = p_info.GetTableName() + "_ID";
                String keyValue = Get_ID().ToString();
                //MPreference.Delete(keyColumn, keyValue);
                StringBuilder sql = new StringBuilder("DELETE FROM AD_ValuePreference WHERE Attribute='")
                   .Append(keyColumn).Append("' AND Value='").Append(keyValue).Append("'");
                DataBase.DB.ExecuteQuery(sql.ToString(), null, null);
                GetCtx().DeletePreference(keyColumn, keyValue);
            }
        }

        public bool Is_ValueChanged(String columnName)
        {
            int index = p_info.GetColumnIndex(columnName);
            if (index < 0)
            {
                log.Log(Level.WARNING, "Column not found - " + columnName);
                return false;
            }
            return Is_ValueChanged(index);
        }

        public bool Is_ValueChanged(int index)
        {
            if (index < 0 || index >= p_info.GetColumnCount())
            {
                log.Log(Level.WARNING, "Column not found - " + index.ToString());
                return false;
            }
            if (_mNewValues[index] == null)
                return false;
            return !_mNewValues[index].Equals(_mOldValues[index]);
        }

        /** Access Level S__ 100	4	System info			*/
        public const int ACCESSLEVEL_SYSTEM = 4;
        /** Access Level _C_ 010	2	Client info			*/
        public const int ACCESSLEVEL_CLIENT = 2;
        /** Access Level __O 001	1	Organization info	*/
        public const int ACCESSLEVEL_ORG = 1;
        /**	Access Level SCO 111	7	System shared info	*/
        public const int ACCESSLEVEL_ALL = 7;
        /** Access Level SC_ 110	6	System/Client info	*/
        public const int ACCESSLEVEL_SYSTEMCLIENT = 6;
        /** Access Level _CO 011	3	Client shared info	*/
        public const int ACCESSLEVEL_CLIENTORG = 3;


        /// <summary>
        /// Get Column Count
        /// return column count
        /// </summary>
        /// <returns></returns>
        public int Get_ColumnCount()
        {
            return p_info.GetColumnCount();
        }

        abstract protected int Get_AccessLevel();

        //IDbTransaction trans;
        public virtual bool Save()
        {
            VLogger.ResetLast();
            bool newRecord = Is_New();	//	save locally as load resets

            if (!newRecord && !Is_Changed())
            {
                log.Fine("Nothing changed - " + p_info.GetTableName());
                return true;
            }

            //log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "AD_Org_ID"));
            //log.SaveError("Other One", Msg.GetElement(GetCtx(), "AD_Org_ID"));

            //if (GetAD_Org_ID() == 0
            //    && (Get_AccessLevel() == ACCESSLEVEL_ORG
            //        || (Get_AccessLevel() == ACCESSLEVEL_CLIENTORG
            //            && MClientShare.IsOrgLevelOnly(GetAD_Client_ID(), Get_Table_ID()))))
            //{
            //    log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "AD_Org_ID"));
            //    return false;
            //}

            ////	Should be Org 0
            //if (GetAD_Org_ID() != 0)
            //{
            //    bool reset = Get_AccessLevel() == ACCESSLEVEL_SYSTEM;
            //    if (!reset && MClientShare.IsClientLevelOnly(GetAD_Client_ID(), Get_Table_ID()))
            //    {
            //        reset = Get_AccessLevel() == ACCESSLEVEL_CLIENT
            //            || Get_AccessLevel() == ACCESSLEVEL_SYSTEMCLIENT
            //            || Get_AccessLevel() == ACCESSLEVEL_CLIENTORG;
            //    }
            //    if (reset)
            //    {
            //        log.Warning("Set Org to 0");
            //        SetAD_Org_ID(0);
            //    }
            //}

            //	Before Save
           // MAssignSet.Execute(this, newRecord);    //	Automatic Assignment

            // Commented this function as Not required 
            // Check For Advance Document Type Module
            //if (this.Get_Value("DocAction") != null)
            //{
            //    if (!ModelLibrary.Classes.AdvanceDocumentType.GetAdvanceDocument(this))
            //    {
            //        return false;
            //    }
            //}
            if (!POActionEngine.Get().BeforeSave(this))
                return false;

            try
            {
                if (!BeforeSave(newRecord))
                {
                    log.Warning("beforeSave failed - " + ToString());
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.Log(Level.WARNING, "beforeSave - " + ToString());
                log.SaveError("Error", ex.Message, false);
                //	throw new DBException(e);
                return false;

            }

            String errorMsg = ModelValidationEngine.Get().FireModelChange
            (this, newRecord ? ModalValidatorVariables.CHANGETYPE_NEW : ModalValidatorVariables.CHANGETYPE_CHANGE);
            if (errorMsg != null)
            {
                s_log.Warning("Validation failed - " + errorMsg);
                s_log.SaveError("Error", errorMsg);
                return false;
            }

            if (newRecord)
                return saveNew();
            else
                return SaveUpdate();

            //return true;
        }

        /// <summary>
        /// overridded ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("PO[")
                .Append(Get_WhereClause(true)).Append("]");
            return sb.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        ///Update Value or create new record.
        ///To reload call load() - not updated
        /// </summary>
        /// <param name="trxName">transaction</param>
        /// <returns>true if saved</returns>
        /// 
        //[Obsolete("User Save(Trx trx) instead")]
        //public bool Save(String trxName,bool dontUse = true)
        //{
        //    Set_TrxName(trxName);
        //    return Save();
        //}

        /// <summary>
        ///Update Value or create new record.
        ///To reload call load() - not updated
        /// </summary>
        /// <param name="trxName">transaction</param>
        /// <returns>true if saved</returns>
        public bool Save(Trx trx)
        {
            Set_Trx(trx);
            return Save();
        }

        /// <summary>
        ///Set Active
        /// </summary>
        /// <param name="active">active</param>
        public void SetIsActive(bool active)
        {
            Set_Value("IsActive", active);
        }

        /// <summary>
        /// Insert (missing) Translation Records
        /// </summary>
        /// <returns>false if error (true if no translation or success)</returns>
        private bool InsertTranslations()
        {
            //	Not a translation table
            if (_mIDs.Length > 1
                || _mIDs[0].Equals(I_ZERO)
                || !p_info.IsTranslated()
                || !(_mIDs[0] is int))
                return true;
            //
            StringBuilder iColumns = new StringBuilder();
            StringBuilder sColumns = new StringBuilder();
            for (int i = 0; i < p_info.GetColumnCount(); i++)
            {
                if (p_info.IsColumnTranslated(i))
                {
                    iColumns.Append(p_info.GetColumnName(i))
                        .Append(",");
                    sColumns.Append("t.")
                        .Append(p_info.GetColumnName(i))
                        .Append(",");
                }
            }
            if (iColumns.Length == 0)
                return true;

            String tableName = p_info.GetTableName();
            String keyColumn = _mKeyColumns[0];
            StringBuilder sql = new StringBuilder("INSERT INTO ")
                .Append(tableName).Append("_Trl (AD_Language,")
                .Append(keyColumn).Append(", ")
                .Append(iColumns)
                .Append(" IsTranslated,AD_Client_ID,AD_Org_ID,Created,CreatedBy,Updated,UpdatedBy) ")
                .Append("SELECT l.AD_Language,t.")
                .Append(keyColumn).Append(", ")
                .Append(sColumns)
                .Append(" 'N',t.AD_Client_ID,t.AD_Org_ID,t.Created,t.CreatedBy,t.Updated,t.UpdatedBy ")
                .Append("FROM AD_Language l, ").Append(tableName).Append(" t ")
                .Append("WHERE l.IsActive='Y' AND l.IsSystemLanguage='Y' AND l.IsBaseLanguage='N' AND t.")
                .Append(keyColumn).Append("=").Append(Get_ID())
                .Append(" AND NOT EXISTS (SELECT * FROM ").Append(tableName)
                .Append("_Trl tt WHERE tt.AD_Language=l.AD_Language AND tt.")
                .Append(keyColumn).Append("=t.").Append(keyColumn).Append(")");
            /* 	Alternative *
            .append(" AND EXISTS (SELECT * FROM ").append(tableName)
            .append("_Trl tt WHERE tt.AD_Language!=l.AD_Language OR tt.")
            .append(keyColumn).append("!=t.").append(keyColumn).append(")");
            /** */
            int no = DB.ExecuteQuery(sql.ToString(), null, _trx);
            log.Fine("#" + no);
            return no > 0;
        }

        /// <summary>
        /// Update Translations.
        /// </summary>
        /// <returns>false if error (true if no translation or success)</returns>
        private bool UpdateTranslations()
        {
            //	Not a translation table
            if (_mIDs.Length > 1
                || _mIDs[0].Equals(I_ZERO)
                || !p_info.IsTranslated()
                || !(_mIDs[0] is int))
                return true;
            //
            bool trlColumnChanged = false;
            for (int i = 0; i < p_info.GetColumnCount(); i++)
            {
                if (p_info.IsColumnTranslated(i)
                    && Is_ValueChanged(p_info.GetColumnName(i)))
                {
                    trlColumnChanged = true;
                    break;
                }
            }
            if (!trlColumnChanged)
                return true;
            //
           // MClient client = MClient.Get(GetCtx());
            //
            String tableName = p_info.GetTableName();
            String keyColumn = _mKeyColumns[0];
            StringBuilder sql = new StringBuilder("UPDATE ")
                .Append(tableName).Append("_Trl SET ");
            //
            if (POActionEngine.Get().IsAutoUpdateTrl(GetCtx(), tableName))
            {
                for (int i = 0; i < p_info.GetColumnCount(); i++)
                {
                    if (p_info.IsColumnTranslated(i))
                    {
                        String columnName = p_info.GetColumnName(i);
                        sql.Append(columnName).Append("=");
                        Object value = Get_Value(columnName);
                        if (value == null)
                            sql.Append("NULL");
                        else if (value is String)
                            sql.Append(GlobalVariable.TO_STRING((String)value));
                        else if (value is Boolean)
                            sql.Append(((Boolean)value) ? "'Y'" : "'N'");
                        //else if (value == typeof(DateTime))
                        //    sql.Append(DB.TO_DATE((Timestamp)value));
                        else
                            sql.Append(value.ToString());
                        sql.Append(",");
                    }
                }
                sql.Append("IsTranslated='Y'");
            }
            else
                sql.Append("IsTranslated='N'");
            //
            sql.Append(" WHERE ")
                .Append(keyColumn).Append("=").Append(Get_ID());
            int no = DB.ExecuteQuery(sql.ToString(), null, _trx);
            log.Fine("#" + no.ToString());
            return no >= 0;
        }


        private List<PO_LOB> m_lobInfo = null;

        private void LOBAdd(Object value, int index, int displayType)
        {
            log.Finest("Value=" + value);
            PO_LOB lob = new PO_LOB(p_info.GetTableName(), p_info.GetColumnName(index),
                Get_WhereClause(true), displayType, value);
            if (m_lobInfo == null)
                m_lobInfo = new List<PO_LOB>();
            m_lobInfo.Add(lob);
        }

        private bool LobSave()
        {
            if (m_lobInfo == null)
                return true;
            bool retValue = true;
            for (int i = 0; i < m_lobInfo.Count; i++)
            {
                PO_LOB lob = (PO_LOB)m_lobInfo[i];
                if (!lob.Save(Get_Trx()))
                {
                    retValue = false;
                    break;
                }
            }	//	for all LOBs
            LobReset();
            return retValue;
        }

        private void LobReset()
        {
            m_lobInfo = null;
        }

        private Object Encrypt(int index, Object xx)
        {
            if (xx == null)
                return null;
            if (index != -1 && p_info.IsEncrypted(index))
                return SecureEngine.Encrypt(xx);
            return xx;
        }

        private Object Encrypt(int index, string xx)
        {
            if (xx == null)
                return null;
            if (index != -1 && p_info.IsEncrypted(index))
                return SecureEngine.Encrypt(xx);
            return xx;
        }

        /**
	 * 	Decrypt data
	 *	@param index index
	 *	@param yy data
	 *	@return yy
	 */
        private Object Decrypt(int index, Object yy)
        {
            if (yy == null)
                return null;
            //Check value is encrypted or not [ column may have value]
            if (index != -1 && p_info.IsEncrypted(index))
            {
                if (yy is String && SecureEngine.IsEncrypted((string)yy))
                {
                    return SecureEngine.Decrypt(yy);
                }
            }
            return yy;
        }	//


        #endregion

        #region "DELETE Function"
        //*************Methods for Delete*************************************************

        /// <summary>
        /// Before delete
        /// </summary>
        /// <returns>bool type</returns>
        /// <author>Veena Pandey</author>
        protected virtual bool BeforeDelete()
        {
            //	log.saveError("Error", Msg.getMsg(getCtx(), "CannotDelete"));
            return true;
        }

        /// <summary>
        /// After delete
        /// </summary>
        /// <param name="success">bool type</param>
        /// <returns>bool type</returns>
        /// <author>Veena Pandey</author>
        protected virtual bool AfterDelete(bool success)
        {
            return success;
        }

        /// <summary>
        /// Delete transaltions of a record
        /// </summary>
        /// <param name="trxName">transaction</param>
        /// <returns>bool type true if translations successfully deleted</returns>
        /// <author>Veena Pandey</author>
        private bool DeleteTranslations(Trx trxName)
        {
            //	Not a translation table
            if (_mIDs.Length > 1
                || _mIDs[0].Equals(I_ZERO)
                || !p_info.IsTranslated()
                || !(_mIDs[0] is int))
                return true;
            //
            String tableName = p_info.GetTableName();
            String keyColumn = _mKeyColumns[0];
            StringBuilder sql = new StringBuilder("DELETE FROM ")
                .Append(tableName).Append("_Trl WHERE ")
                .Append(keyColumn).Append("=").Append(Get_ID());
            int no = DB.ExecuteQuery(sql.ToString(), null, trxName);
            log.Fine("#" + no.ToString());
            return no >= 0;
        }

        /// <summary>
        /// Deletes a record
        /// </summary>
        /// <param name="force">bool type</param>
        /// <param name="trxName">transaction</param>
        /// <returns>bool type true if record successfully deleted</returns>
        public bool Delete(bool force, Trx trx)
        {
            Set_Trx(trx);
            return Delete(force);
        }

        /// <summary>
        /// Deletes a record
        /// </summary>
        /// <param name="force">bool type</param>
        /// <returns>bool type true if record successfully deleted</returns>
        /// <author>Veena Pandey</author>
        public bool Delete(bool force)
        {
            //CLogger.resetLast();
            if (Is_New())
                return true;
            if (GetCtx().GetAD_Client_ID() < 0)
                throw new ArgumentException("R/O Context - no new instances allowed");

            int AD_Table_ID = p_info.getAD_Table_ID();
            int Record_ID = Get_ID();

            if (!force)
            {
                int iProcessed = Get_ColumnIndex("Processed");
                if (iProcessed != -1)
                {
                    Boolean? processed = (Boolean?)Get_Value(iProcessed);
                    if (processed != null && processed.Value)
                    {
                        log.Warning("Record processed");	//	CannotDeleteTrx
                        log.SaveError("Processed", "Processed", false);
                        return false;
                    }
                }	//	processed
            }

            if (CheckActiveWF(AD_Table_ID, Record_ID, _trx))
            {
                log.Warning("Active Workflow Exists");	//	CannotDeleteTrx
                log.SaveError("ActiveWF", "ActiveWF Exists", false);
                return false;
            }


            try
            {
                if (!BeforeDelete())
                {
                    log.Warning("beforeDelete failed");
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.Log(Level.WARNING, "beforeDelete" + ex.Message);
                log.SaveError("Error", ex.Message, false);
                return false;
            }

            //	Delete Restrict AD_Table_ID/Record_ID (Requests, ..)
            string errorMsg = PORecord.Exists(AD_Table_ID, Record_ID, _trx);
            if (errorMsg != null)
            {
                log.SaveError("CannotDelete", errorMsg);
                return false;
            }
            Trx localTrx = null;
            bool isLocalTrx = false;

            bool success = false;
            try
            {
                errorMsg = ModelValidationEngine.Get().FireModelChange(this, ModalValidatorVariables.CHANGETYPE_DELETE);
                if (errorMsg != null)
                {
                    log.SaveError("Error", errorMsg);
                    return false;
                }

                localTrx = _trx;
                if (_trx == null)
                {
                    localTrx = Trx.Get("POdel");
                    isLocalTrx = true;
                }

                //// Delete translationa of the record
                DeleteTranslations(localTrx);

                //	Delete Cascade AD_Table_ID/Record_ID (Attachments, ..)
                PORecord.DeleteCascade(AD_Table_ID, Record_ID, localTrx);

                //	The Delete Statement
                StringBuilder sql = new StringBuilder("DELETE FROM ").Append(p_info.GetTableName())
                .Append(" WHERE ").Append(Get_WhereClause(true));
                int no = DB.ExecuteQuery(sql.ToString(), null, localTrx);
                success = no == 1;

                // commit the transaction
                _idOld = Get_ID();

                if (!success)
                {
                    log.Warning("Not deleted");
                    if (isLocalTrx)
                        localTrx.Rollback();
                }
                else
                {
                    if (isLocalTrx)
                        localTrx.Commit();
                    //	Change Log
                    MSession session = MSession.Get(GetCtx(), false);
                    if (session == null)
                    {
                        log.Fine("No Session found");
                    }
                    else if (session.IsWebStoreSession()
                        || MChangeLog.IsLogged(AD_Table_ID, MChangeLog.CHANGELOGTYPE_Delete))
                    {
                        int AD_ChangeLog_ID = 0;
                        int size = Get_ColumnCount();
                        for (int i = 0; i < size; i++)
                        {
                            Object value = _mOldValues[i];
                            if (_mIDs.Length == 1 && value != null
                               && !p_info.IsEncrypted(i)        //	not encrypted
                               && !p_info.IsVirtualColumn(i)    //	no virtual column
                               && !"Password".Equals(p_info.GetColumnName(i))
                               )
                            {
                                Object keyInfo = Get_ID();
                                if (_mIDs.Length != 1)
                                    keyInfo = Get_WhereClause(true);
                                MChangeLog cLog = session.ChangeLog(
                                    _trx, AD_ChangeLog_ID,
                                    AD_Table_ID, p_info.GetColumn(i).AD_Column_ID,
                                    keyInfo, GetAD_Client_ID(), GetAD_Org_ID(), value, null,
                                    Get_TableName(), MChangeLog.CHANGELOGTYPE_Delete);
                                if (cLog != null)
                                    AD_ChangeLog_ID = cLog.GetAD_ChangeLog_ID();
                            }
                        }   //   for all fields
                    }

                    // string adLog = p_ctx.GetContext("AD_ChangeLogBatch");
                    StringBuilder adLog = null;
                    if (session != null && logbatch.ContainsKey(session.GetAD_Session_ID()))
                    {
                        adLog = logbatch[session.GetAD_Session_ID()];
                    }
                    if (adLog != null && adLog.Length > 6)
                    {
                        if (!DB.IsPostgreSQL())
                        {
                            adLog.Append(" END; ");
                        }
                        adLog.Replace("TO_DATE", "TO_TIMESTAMP");
                        no = DB.ExecuteQuery(adLog.ToString(), null, null);
                        adLog.Clear();


                    }


                    //	Housekeeping
                    _mIDs[0] = I_ZERO;
                    if (_trx == null)
                    {
                        log.Fine("complete");
                    }
                    else
                    {
                        log.Fine("[" + _trx.GetTrxName() + "] - complete");
                    }
                    _attachment = null;
                }
            }
            catch (Exception ex)
            {

            }
            if (isLocalTrx)
                localTrx.Close();
            localTrx = null;
            UpdatePreferences();

            try
            {
                success = AfterDelete(success);
                POActionEngine.Get().AfterDelete(this, success);
                //if (success)
                //    DeleteTreeNode();
            }
            catch (Exception e)
            {
                //Common.//ErrorLog.FillErrorLog("PO afterDelete", "", e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
                log.Log(Level.WARNING, "afterDelete" + e.Message);
                log.SaveError("Error", e.Message, false);
                success = false;
                //	throw new DBException(e);
            }

            //	Reset
            if (success)
            {
                _idOld = 0;
                int size = p_info.GetColumnCount();
                _mOldValues = new Object[size];
                _mNewValues = new Object[size];
                CacheMgt.Get().Reset(p_info.GetTableName());
            }
            log.Info("" + success);
            return success;
        }

        private bool CheckActiveWF(int AD_Table_ID, int Record_ID, Trx trx)
        {
            String sql = "SELECT COUNT(AD_WF_ACTIVITY_ID) FROM AD_WF_ACTIVITY " +
            "WHERE AD_Table_ID=@param1 AND Record_ID=@param2 AND PROCESSED='N'";

            List<object> lst = new List<object>();
            lst.Add(AD_Table_ID);
            lst.Add(Record_ID);

            int no = QueryUtil.GetSQLValue(trx, sql, lst);
            if (no > 0)
                return true;
            else
                return false;
        }


        /// <summary>
        ///Insert id data into Tree
        /// </summary>
        /// <returns></returns>
        //private bool InsertTreeNode()
        //{
        //    int AD_Table_ID = Get_Table_ID();
        //    if (!MTree.HasTree(AD_Table_ID, GetCtx()))
        //        return false;
        //    int id = Get_ID();
        //    int AD_Client_ID = GetAD_Client_ID();
        //    String treeTableName = MTree.GetNodeTableName(AD_Table_ID, GetCtx());
        //    int C_Element_ID = 0;
        //    if (AD_Table_ID == X_C_ElementValue.Table_ID)
        //    {
        //        int? ii = (int?)Get_Value("C_Element_ID");
        //        if (ii != null)
        //            C_Element_ID = ii.Value;
        //    }
        //    //
        //    StringBuilder sb = new StringBuilder("INSERT INTO ")
        //        .Append(treeTableName)
        //        .Append(" (AD_Client_ID,AD_Org_ID, IsActive,Created,CreatedBy,Updated,UpdatedBy, ")
        //        .Append("AD_Tree_ID, Node_ID, Parent_ID, SeqNo) ")
        //        //
        //        .Append("SELECT t.AD_Client_ID,0, 'Y', SysDate, 0, SysDate, 0,")
        //        .Append("t.AD_Tree_ID, ").Append(id).Append(", 0, 999 ")
        //        .Append("FROM AD_Tree t ")
        //        .Append("WHERE t.AD_Client_ID=").Append(AD_Client_ID).Append(" AND t.IsActive='Y'");
        //    //	Account Element Value handling
        //    if (C_Element_ID != 0)
        //        sb.Append(" AND EXISTS (SELECT * FROM C_Element ae WHERE ae.C_Element_ID=")
        //            .Append(C_Element_ID).Append(" AND t.AD_Tree_ID=ae.AD_Tree_ID)");
        //    else	//	std trees
        //        sb.Append(" AND t.IsAllNodes='Y' AND t.AD_Table_ID=").Append(AD_Table_ID);
        //    //	Duplicate Check
        //    sb.Append(" AND NOT EXISTS (SELECT * FROM ").Append(treeTableName).Append(" e ")
        //        .Append("WHERE e.AD_Tree_ID=t.AD_Tree_ID AND Node_ID=").Append(id).Append(")");
        //    //
        //    // Check applied to insert the node in treenode from organization units window in only default tree - Changed by Mohit asked by mukesh sir and ashish
        //    if (AD_Table_ID == X_AD_Org.Table_ID)
        //    {
        //        MOrg Org = new MOrg(GetCtx(), id, null);
        //        if (Org.Get_ColumnIndex("IsOrgUnit") > -1)
        //        {
        //            if (Org.IsOrgUnit())
        //            {
        //                int DefaultTree_ID = MTree.GetDefaultAD_Tree_ID(GetAD_Client_ID(), AD_Table_ID);
        //                sb.Append(" AND t.AD_Tree_ID=").Append(DefaultTree_ID);
        //            }
        //        }
        //    }
        //    int no = DB.ExecuteQuery(sb.ToString(), null, Get_Trx());
        //    if (no > 0)
        //    {
        //        log.Fine("#" + no.ToString() + " - AD_Table_ID=" + AD_Table_ID);
        //    }
        //    else
        //    {
        //        log.Warning("#" + no.ToString() + " - AD_Table_ID=" + AD_Table_ID);
        //    }
        //    return no > 0;
        //}

        /// <summary>
        /// Delete ID Tree Nodes
        /// </summary>
        /// <returns>true if actually deleted (could be non existing)</returns>
        //private bool DeleteTreeNode()
        //{
        //    int id = Get_ID();
        //    if (id == 0)
        //        id = Get_IDOld();
        //    int AD_Table_ID = Get_Table_ID();
        //    if (!MTree.HasTree(AD_Table_ID, GetCtx()))
        //        return false;
        //    String treeTableName = MTree.GetNodeTableName(AD_Table_ID, GetCtx());
        //    if (treeTableName == null)
        //        return false;
        //    //
        //    StringBuilder sb = new StringBuilder("DELETE FROM ")
        //        .Append(treeTableName)
        //        .Append(" n WHERE Node_ID=").Append(id)
        //        .Append(" AND EXISTS (SELECT * FROM AD_Tree t ")
        //        .Append("WHERE t.AD_Tree_ID=n.AD_Tree_ID AND t.AD_Table_ID=")
        //        .Append(AD_Table_ID).Append(")");
        //    //
        //    int no = DB.ExecuteQuery(sb.ToString(), null, Get_Trx());
        //    if (no > 0)
        //        log.Fine("#" + no.ToString() + " - AD_Table_ID=" + AD_Table_ID);
        //    else
        //        log.Warning("#" + no.ToString() + " - AD_Table_ID=" + AD_Table_ID);
        //    return no > 0;
        //}



        #endregion

        protected int get_ValueAsInt(int index)
        {
            Object value = Get_Value(index);
            if (value == null)
                return 0;
            if (value is int)
                return ((int)value);
            try
            {
                return int.Parse(value.ToString());
            }
            catch (Exception ex)
            {
                log.Warning(p_info.GetColumnName(index) + " - " + ex.Message);
                return 0;
            }
        }

        protected virtual bool BeforeSave(bool newRecord)
        {
            /** Prevents saving
            log.saveError("Error", Msg.parseTranslation(getCtx(), "@C_Currency_ID@ = @C_Currency_ID@"));
            log.saveError("FillMandatory", Msg.getElement(getCtx(), "PriceEntered"));
            /** Issues message
            log.saveWarning(AD_Message, message);
            log.saveInfo (AD_Message, message);
            **/
            return true;
        }

        /// <summary>
        ///	Get Created
        /// </summary>
        /// <returns>return created date</returns>
        /// <author>Raghunandan Sharma</author>
        public DateTime GetCreated()
        {
            return Convert.ToDateTime(Get_Value("Created"));
        }

        /// <summary>
        /// Get TableName
        /// </summary>
        /// <returns>table name</returns>
        public String Get_TableName()
        {
            return p_info.GetTableName();
        }

        /// <summary>
        /// Get UpdatedBy
        /// </summary>
        /// <returns>AD_User_ID</returns>
        public int GetUpdatedBy()
        {
            object oo = Get_Value("UpdatedBy");
            if (oo == null)
                return 0;
            return int.Parse(oo.ToString());
        }

        /// <summary>
        /// Get Updated
        /// </summary>
        /// <returns>updated</returns>
        public DateTime GetUpdated()
        {
            return Convert.ToDateTime(Get_Value("Updated"));
        }

        /// <summary>
        /// Delete Accounting records.
        /// </summary>
        /// <param name="acctTable">accounting sub table</param>
        /// <returns>true</returns>
        protected bool Delete_Accounting(String acctTable)
        {
            return true;
        }

        protected bool Insert_Accounting(String acctTable, String acctBaseTable, String whereClause)
        {
            if (acctColumns == null	//	cannot cache C_BP_*_Acct as there are 3
                || acctTable.StartsWith("C_BP_"))
            {
                acctColumns = new List<String>();
                String sql = "SELECT c.ColumnName "
                    + "FROM AD_Column c INNER JOIN AD_Table t ON (c.AD_Table_ID=t.AD_Table_ID) "
                    + "WHERE t.TableName='" + acctTable + "' AND c.IsActive='Y' AND c.AD_Reference_ID=25 ORDER BY 1";
                DataSet ds = null;
                try
                {
                    ds = DB.ExecuteDataset(sql, null, _trx);
                    if (ds.Tables.Count > 0)
                    {
                        int totCount = ds.Tables[0].Rows.Count;
                        for (int i = 0; i < totCount; i++)
                        {
                            acctColumns.Add(ds.Tables[0].Rows[i]["ColumnName"].ToString());
                        }
                    }
                }
                catch (Exception e)
                {
                    log.Log(Level.SEVERE, acctTable + " " + e.Message);
                }

                if (acctColumns.Count == 0)
                {
                    log.Severe("No Columns for " + acctTable);
                    return false;
                }
            }

            //	Create SQL Statement - INSERT
            StringBuilder sb = new StringBuilder("INSERT INTO ")
                .Append(acctTable)
                .Append(" (").Append(Get_TableName())
                .Append("_ID, C_AcctSchema_ID, AD_Client_ID,AD_Org_ID,IsActive, Created,CreatedBy,Updated,UpdatedBy ");
            for (int i = 0; i < acctColumns.Count; i++)
                sb.Append(",").Append(acctColumns[i]);
            //	..	SELECT
            sb.Append(") SELECT ").Append(Get_ID())
                .Append(", p.C_AcctSchema_ID, p.AD_Client_ID,0,'Y', SysDate,")
                .Append(GetUpdatedBy()).Append(",SysDate,").Append(GetUpdatedBy());
            for (int i = 0; i < acctColumns.Count; i++)
                sb.Append(",p.").Append(acctColumns[i]);
            //	.. 	FROM
            sb.Append(" FROM ").Append(acctBaseTable)
                .Append(" p WHERE p.AD_Client_ID=").Append(GetAD_Client_ID());
            if (whereClause != null && whereClause.Length > 0)
                sb.Append(" AND ").Append(whereClause);
            sb.Append(" AND NOT EXISTS (SELECT * FROM ").Append(acctTable)
                .Append(" e WHERE e.C_AcctSchema_ID=p.C_AcctSchema_ID AND e.")
                .Append(Get_TableName()).Append("_ID=").Append(Get_ID()).Append(")");
            //
            int no = DB.ExecuteQuery(sb.ToString(), null, _trx);

            if (no > 0)
                log.Fine("#" + no.ToString());
            else
                log.Warning("#" + no.ToString() + " - Table=" + acctTable + " from " + acctBaseTable);
            return no > 0;
        }

        /// <summary>
        /// Set Value of Column
        /// </summary>
        /// <param name="AD_Column_ID">column</param>
        /// <param name="value">value</param>
        public void Set_ValueOfColumn(int AD_Column_ID, Object value)
        {
            int index = p_info.GetColumnIndex(AD_Column_ID);
            if (index < 0)
            {
                log.Log(Level.SEVERE, "Not found - AD_Column_ID=" + AD_Column_ID);
            }
            String ColumnName = p_info.GetColumnName(index);
            if (ColumnName.Equals("IsApproved"))
                Set_ValueNoCheck(ColumnName, value);
            else
                Set_Value(index, value);
        }

        /// <summary>
        /// Get Value of Column
        /// </summary>
        /// <param name="AD_Column_ID">column</param>
        /// <returns>value or null</returns>
        public Object Get_ValueOfColumn(int AD_Column_ID)
        {
            int index = p_info.GetColumnIndex(AD_Column_ID);
            if (index < 0)
            {
                log.Log(Level.WARNING, "Not found - AD_Column_ID=" + AD_Column_ID);
                return null;
            }
            return Get_Value(index);
        }

        /// <summary>
        /// Do we have a PDF Attachment
        /// </summary>
        /// <returns>true if there is a PDF attachment</returns>
        public bool IsPdfAttachment()
        {
            return IsAttachment(".pdf");
        }

        /// <summary>
        /// Do we have a Attachment of type
        /// </summary>
        /// <param name="extension">extension e.g. .pdf</param>
        /// <returns>true if there is a attachment of type</returns>
        public bool IsAttachment(String extension)
        {
            GetAttachment(false);
            if (_attachment == null)
                return false;
            for (int i = 0; i < _attachment.GetEntryCount(); i++)
            {
                if (_attachment.GetEntryName(i).ToLower().EndsWith(extension.ToLower()))
                {
                    log.Fine("#" + i.ToString() + ": " + _attachment.GetEntryName(i));
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///Get Attachments.	An attachment may have multiple entries
        /// </summary>
        /// <returns> Attachment or null</returns>
        public dynamic GetAttachment()
        {
            return GetAttachment(false);
        }
        /// <summary>
        /// Get Attachments
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>Attachment or null</returns>
        public dynamic   GetAttachment(bool requery)
        {
            if (_attachment == null || requery)
                _attachment = POActionEngine.Get().GetAttachment(GetCtx(), p_info.getAD_Table_ID(), Get_ID());
            return _attachment;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] GetPdfAttachment()
        {
            return GetAttachmentData(".pdf");
        }

        /// <summary>
        /// Get Attachment Data of type
        /// </summary>
        /// <param name="extension">extension e.g. .pdf</param>
        /// <returns>data or null</returns>
        public byte[] GetAttachmentData(String extension)
        {
            GetAttachment(false);
            if (_attachment == null)
                return null;
            for (int i = 0; i < _attachment.GetEntryCount(); i++)
            {
                if (_attachment.GetEntryName(i).ToLower().EndsWith(extension.ToLower()))
                {
                    log.Fine("#" + i.ToString() + ": " + _attachment.GetEntryName(i));
                    return _attachment.GetEntryData(i);
                }
            }
            return null;
        }

        /// <summary>
        /// Get the Column Processing index
        /// </summary>
        /// <returns>index or -1</returns>
        private int Get_ProcessingIndex()
        {
            return p_info.GetColumnIndex("Processing");
        }

        /// <summary>
        /// Lock it.
        /// </summary>
        /// <returns>true if locked</returns>
        public bool Lock()
        {
            int index = Get_ProcessingIndex();
            if (index != -1)
            {
                _mOldValues[index] = true;		//	direct
                String sql = "UPDATE " + p_info.GetTableName()
                    + " SET Processing='Y' WHERE (Processing='N' OR Processing IS NULL) AND "
                    + Get_WhereClause(true);
                bool success = DB.ExecuteQuery(sql, null, null) == 1;	//	outside trx
                if (success)
                    log.Fine("success");
                else
                    log.Log(Level.WARNING, "failed");
                return success;
            }
            return false;
        }

        /// <summary>
        /// UnLock it
        /// </summary>
        /// <param name="trxName">transaction</param>
        /// <returns>true if unlocked (false only if unlock fails)</returns>
        public bool Unlock(Trx trxName)
        {
            int index = Get_ProcessingIndex();
            if (index != -1)
            {
                _mNewValues[index] = false;		//	direct
                String sql = "UPDATE " + p_info.GetTableName()
                    + " SET Processing='N' WHERE " + Get_WhereClause(true);
                bool success = DB.ExecuteQuery(sql, null, trxName) == 1;

                if (success)
                    log.Fine("success" + (trxName == null ? "" : " [" + trxName + "]"));
                else
                    log.Log(Level.WARNING, "failed" + (trxName == null ? "" : " [" + trxName + "]"));
                return success;
            }
            return true;
        }

        /// <summary>
        /// Set Default values.
        /// Client, Org, Created/Updated, *By, IsActive
        /// </summary>
        protected void SetStandardDefaults()
        {
            int size = Get_ColumnCount();
            for (int i = 0; i < size; i++)
            {
                if (p_info.IsVirtualColumn(i))
                    continue;
                String colName = p_info.GetColumnName(i);
                //  Set Standard Values
                if (colName.EndsWith("tedBy"))
                    _mNewValues[i] = p_ctx.GetAD_User_ID();
                else if (colName.Equals("Created") || colName.Equals("Updated"))
                    _mNewValues[i] = DateTime.Now;
                else if (colName.Equals(p_info.GetTableName() + "_ID"))    //  KeyColumn
                    _mNewValues[i] = I_ZERO;
                else if (colName.Equals("IsActive"))
                    _mNewValues[i] = true;
                else if (colName.Equals("AD_Client_ID"))
                    _mNewValues[i] = p_ctx.GetAD_Client_ID();
                else if (colName.Equals("AD_Org_ID"))
                    _mNewValues[i] = p_ctx.GetAD_Org_ID();
                else if (colName.Equals("Processed"))
                    _mNewValues[i] = false;
                else if (colName.Equals("Processing"))
                    _mNewValues[i] = false;
                else if (colName.Equals("Posted"))
                    _mNewValues[i] = false;
            }


        }
        //protected Dictionary<String, String> Get_HashMap()
        //{
        //    Dictionary<String, String> hmOut = new Dictionary<String, String>();
        //    FillMap(hmOut);
        //    return hmOut;
        //}
        protected HashMap<String, String> Get_HashMap()
        {
            HashMap<String, String> hmOut = new HashMap<string, string>();
            FillMap(hmOut);
            return hmOut;
        }


        /// <summary>
        /// Used for HashTable
        /// </summary>
        /// <returns></returns>
        public int HashCode()
        {
            StringBuilder keyValue = new StringBuilder().Append(Get_Table_ID()).Append(7);
            for (int i = 0; i < _mIDs.Length; i++)
                keyValue.Append(_mIDs[i]);
            try
            {
                return int.Parse(keyValue.ToString());
            }
            catch (Exception e)
            {
                log.Severe(e.ToString());
            }
            return base.GetHashCode();
        }

        /**
        *  Get Old Value
        *  @param columnName column name
        *  @return value or null
        */
        public Object Get_ValueOld(String columnName)
        {
            int index = Get_ColumnIndex(columnName);
            if (index < 0)
            {
                log.Log(Level.WARNING, "Column not found - " + columnName);
                return null;
            }
            return Get_ValueOld(index);
        }

        /**
	     *  Get Old Value
	     *  @param index index
	     *  @return value
	     */
        public Object Get_ValueOld(int index)
        {
            if (index < 0 || index >= Get_ColumnCount())
            {
                log.Log(Level.WARNING, "Index invalid - " + index);
                return null;
            }
            return _mOldValues[index];
        }

        /**
     * 	Convert String To Timestamp.
     * 	Null is passed through
     *	@param stringValue value
     *	@return Timestamp
     */
        public static DateTime? ConvertToTimestamp(String stringValue)
        {
            if (stringValue == null || stringValue.Length == 0)
                return null;
            //	long Time format
            try
            {
                long time = long.Parse(stringValue);
                return Convert.ToDateTime(time);
            }
            catch (Exception e)
            {
                s_log.Severe(e.ToString());
            }
            //	JDBC yyyy-mm-dd hh:mm:ss.fffffffff
            try
            {
                //return DateTime.valueOf(stringValue);
                return Convert.ToDateTime(stringValue);
            }
            catch (Exception e)
            {
                s_log.Severe(e.ToString());
            }
            throw new ArgumentException("Cannot convert to Timestamp: " + stringValue);
        }

        /**
        * 	Get CreatedBy
        * 	@return AD_User_ID
        */
        public int GetCreatedBy()
        {
            int ii = (int)Get_Value("CreatedBy");
            if (ii < 0)
                return 0;
            return ii;
        }

        public void SetContext(int windowNo, string context, string value)
        {
            //// Charges - Set Context
            //if (p_changeVO != null)
            //p_changeVO.setContext(getCtx(), windowNo, columnName, value);
            p_ctx.SetContext(windowNo, context, value);
        }
        public void SetContext(int windowNo, string context, bool value)
        {
            //// Charges - Set Context
            //if (p_changeVO != null)
            //p_changeVO.setContext(getCtx(), windowNo, columnName, value);
            p_ctx.SetContext(windowNo, context, value);
        }

        /**
	 * 	Get Display Value of value
	 *	@param columnName columnName
	 *	@param currentValue current value
	 *	@return String value with "./." as null
	 */
        protected String Get_DisplayValue(String columnName, bool currentValue)
        {
            Object value = currentValue ? Get_Value(columnName) : Get_ValueOld(columnName);
            if (value == null)
            {
                return "./.";
            }
            String retValue = value.ToString();
            int index = Get_ColumnIndex(columnName);
            if (index < 0)
                return retValue;
            int dt = Get_ColumnDisplayType(index);
            if (DisplayType.IsText(dt) || DisplayType.YesNo == dt)
            {
                return retValue;
            }
            //	Lookup
            Lookup lookup = Get_ColumnLookup(index);
            if (lookup != null)
            {
                return lookup.GetDisplay(value);
            }
            //	Other
            return retValue;
        }

        /**
        *  Get Column DisplayType
        *  @param index index
        *  @return display type
        */
        protected int Get_ColumnDisplayType(int index)
        {
            return p_info.GetColumnDisplayType(index);
        }

        /**
     *  Get Lookup
     *  @param index index
     *  @return Lookup or null
     */
        protected Lookup Get_ColumnLookup(int index)
        {
            POInfoColumn col = p_info.GetColumnInfo(index);
            return POActionEngine.Get().GetLookup(GetCtx(), col);
        }

        /**
        * 	Set UpdatedBy
        * 	@param AD_User_ID user
        */
        protected void SetUpdatedBy(int AD_User_ID)
        {
            Set_ValueNoCheck("UpdatedBy", AD_User_ID);
        }

        /// <summary>
        /// Get Logger
        /// </summary>
        /// <returns></returns>
        public VLogger Get_Logger()
        {
            return log;
        }	//	getLogger

        public static Decimal? ConvertToBigDecimal(String stringValue)
        {
            if (stringValue == null || stringValue.Length == 0)
            {
                return null;
            }
            return new Decimal(long.Parse(stringValue));
        }

        /// <summary>
        /// Get Column Value
        /// </summary>
        /// <param name="columnName">column name</param>
        /// <returns>value or ""</returns>
        public String GetValueAsString(String columnName)
        {
            Object value = Get_Value(columnName);
            if (value == null)
                return "";
            if (value.GetType() == typeof(DateTime))
            {
                //long time = ((Timestamp)value).getTime();
                //return String.valueOf(time);
                return value.ToString();
            }
            else if (value.GetType() == typeof(Boolean))
            {
                if (((Boolean)value))
                    return "Y";
                else
                    return "N";
            }
            return value.ToString();
        }

        /// <summary>
        /// Get All IDs of Table.
        /// Used for listing all Entities
        /// </summary>
        /// <param name="tableName">table name (key column with _ID)</param>
        /// <param name="whereClause">optional where clause</param>
        /// <param name="trxName">transaction</param>
        /// <returns>array  of IDs or null</returns>
        public static int[] GetAllIDs(String tableName, String whereClause, Trx trxName)
        {
            List<int> list = new List<int>();
            StringBuilder sql = new StringBuilder("SELECT ");
            sql.Append(tableName).Append("_ID FROM ").Append(tableName);
            if (whereClause != null && whereClause.Length > 0)
            {
                sql.Append(" WHERE ").Append(whereClause);
            }
            IDataReader idr = null;
            try
            {

                idr = DB.ExecuteReader(sql.ToString(), null, trxName);
                while (idr.Read())
                {
                    list.Add(Util.GetValueOfInt(idr[0].ToString()));//.getInt(1)));
                }
                idr.Close();
                idr = null;
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                s_log.Log(Level.SEVERE, sql.ToString(), e);
                return null;
            }

            //// Convert to array
            int[] retValue = new int[list.Count];
            for (int i = 0; i < retValue.Length; i++)
            {
                retValue[i] = Util.GetValueOfInt(list[i]);//.intValue();
            }
            return retValue;
        }

        /// <summary>
        /// Get Column Name
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>ColumnName</returns>
        public String Get_ColumnName(int index)
        {
            return p_info.GetColumnName(index);
        }

        /// <summary>
        /// Handel New insert from mail Attachment form
        /// </summary>
        public bool CreateNewRecord
        {
            get
            {
                return _mCreateNew;
            }
            set
            {
                _mCreateNew = value;
            }
        }

        public bool CopyTo(PO dCopy)
        {

            //  this._mOldValues.CopyTo(dCopy._mNewValues, 0);



            int size = dCopy.Get_ColumnCount();
            for (int i = 0; i < size; i++)
            {
                // if (dCopy.p_info.IsVirtualColumn(i))
                //   continue;
                String colName = dCopy.p_info.GetColumnName(i);
                //  Set Standard Values
                if (colName.EndsWith("tedBy"))
                    dCopy._mNewValues[i] = p_ctx.GetAD_User_ID();
                else if (colName.Equals("Created") || colName.Equals("Updated"))
                {
                    dCopy._mNewValues[i] = (GlobalVariable.TO_DATE(DateTime.Now, dCopy.p_info.GetColumnDisplayType(i) == DisplayType.Date));
                }
                //dCopy._mNewValues[i] = GlobalVariable.TO_DATE(DateTime.Now,false) ;//new DateTime(CommonFunctions.CurrentTimeMillis());
                else if (colName.Equals(dCopy.p_info.GetTableName() + "_ID"))    //  KeyColumn
                    dCopy._mNewValues[i] = I_ZERO;
                else if (colName.Equals("Export_ID"))
                    continue;
                else
                {
                    dCopy.Set_Value(colName, this.Get_Value(colName));
                }
            }

            dCopy.SetAD_Client_ID(0);//
            dCopy.SetAD_Org_ID(0);//




            //if (colName.EndsWith("tedBy"))
            //    _mNewValues[i] = p_ctx.GetAD_User_ID();
            //else if (colName.Equals("Created") || colName.Equals("Updated"))
            //    _mNewValues[i] = new DateTime(CommonFunctions.CurrentTimeMillis());
            //else if (colName.Equals(p_info.GetTableName() + "_ID"))    //  KeyColumn
            //    _mNewValues[i] = I_ZERO;


            return true;
        }

        public bool CopyForUpdate(PO dCopy)
        {
            int size = dCopy.Get_ColumnCount();
            for (int i = 0; i < size; i++)
            {
                String colName = dCopy.p_info.GetColumnName(i);
                if (colName.Equals(dCopy.p_info.GetTableName() + "_ID"))    //  KeyColumn
                {
                    continue;
                }

                else if (dCopy.p_info.GetColumn(i).IsParent)
                {
                    continue;
                }
                // dCopy._mNewValues[i] = I_ZERO;
                //  Set Standard Values
                else if (colName.EndsWith("tedBy") || colName.Equals("AD_Client_ID") || colName.Equals("AD_Org_ID"))
                {
                    // dCopy._mNewValues[i] = p_ctx.GetAD_User_ID();
                    continue;
                }
                else if (colName.Equals("Created") || colName.Equals("Updated"))
                { }
                //  dCopy._mNewValues[i] =  GlobalVariable.TO_DATE(DateTime.Now,false);// new DateTime(CommonFunctions.CurrentTimeMillis());


                else if (Util.IsEqual(this._mOldValues[i], dCopy._mOldValues[i]))
                {
                    continue;
                }
                else
                    dCopy._mNewValues[i] = this._mOldValues[i];
            }
            return true;
        }

        /// <summary>
        /// Get Find parameter.
        /// Convert to upper case and add % at the end
        /// </summary>
        /// <param name="query">query in string</param>
        /// <returns>out string</returns>
        protected static String GetFindParameter(String query)
        {
            if (query == null)
                return null;
            if (query.Length == 0 || query.Equals("%"))
                return null;
            if (!query.EndsWith("%"))
                query += "%";
            return query.ToUpper();
        }

        //Manfacturing

        /// <summary>
        /// Update Value or create new record.
        /// To reload call load() - not updated
        /// </summary>
        /// <param name="trx"></param>
        /// <param name="extends"></param>
        /// <returns>true if saved</returns>
        /// <writer>Raghu</writer>
        /// <Date>03-March-2011</Date>
        public static Boolean SaveAll(Trx trx, System.Collections.ArrayList _objects)
        {
            VLogger.ResetLast();
            if (_objects.Count <= 0)
            {
                return true;//requested by harwinder g
            }
            // only process changed objects
            PO[] allobjects = (PO[])_objects.ToArray(typeof(PO));// new aList<PO>(_objects.Count);

            List<PO> objects = new List<PO>();
            foreach (PO po in allobjects)
            {
                if (po.Is_Changed())
                {
                    objects.Add(po);
                }
            }

            _objects = null;

            foreach (PO po in objects)
            {
                if (!ValidateBeforeSave(po))
                {
                    return false;
                }
            }

            // map of SQL statements to sets of PO objects 
            Dictionary<String, HashSet<PO>> sqls = new Dictionary<String, HashSet<PO>>();
            /** map of PO objects to QueryParams */
            Dictionary<PO, QueryParams> queries = new Dictionary<PO, QueryParams>();

            foreach (PO po in objects)
            {
                QueryParams param = null;
                try
                {
                    param = po.Is_New() ? po.GetSaveNewQueryInfo() :
                       po.GetSaveUpdateQueryInfo();

                    Ctx p_ctx = new Ctx();

                    string adLog = p_ctx.GetContext("AD_ChangeLogBatch");
                    if (adLog != null && adLog.Length > 6)
                    {
                        string adLogDB = "";
                        if (!DB.IsPostgreSQL())
                        {
                            adLogDB = adLog + " END; ";
                        }
                        else
                        {
                            adLogDB = adLog;
                        }
                        p_ctx.SetContext("AD_ChangeLogBatch", "");
                        adLogDB = adLogDB.Replace("TO_DATE", "TO_TIMESTAMP");
                        DB.ExecuteQuery(adLogDB, null, null);
                    }
                }
                catch (Exception ex)

                {
                }

                queries[po] = param;
                HashSet<PO> pos = null;
                if (sqls.Count > 0)
                {
                    pos = sqls[param.sql];
                }
                if (pos == null)
                {
                    pos = new HashSet<PO>();
                    sqls[param.sql] = pos;
                }
                pos.Add(po);
            }
            //for (Map.Entry<String, HashSet<PO>> sql : sqls.entrySet()) 
            foreach (var sql in sqls)
            {
                s_log.Fine("Bulk update " + sql.Value.Count + " records for: " + sql.Key);
                List<List<Object>> bulkParams = new List<List<Object>>();
                //for (PO po : sql.getValue())
                foreach (PO po in sql.Value)
                {
                    bulkParams.Add(queries[po].parameters);
                }

                int no = 0;
                try
                {
                    no = DB.ExecuteBulkUpdate(trx, sql.Key, bulkParams);
                }
                catch { }
                if (no != bulkParams.Count)
                {
                    return false;
                }
            }


            Boolean allOK = true;
            foreach (PO po in objects)
            {
                Boolean ok = true;

                Boolean isNew = po.Is_New();
                if (queries[po] != null)
                {
                    ok &= po.LobSave();
                    if (isNew)
                        ok &= po.Load(trx);
                    ok &= po.SaveFinish(isNew, ok);
                }
                else
                {
                    // if new record, null aParams is failure;
                    // if update, null aParams is success
                    ok = po.SaveFinish(isNew, !isNew);
                }
                allOK &= ok;
            }

            return allOK;
        }


        /// <summary>
        /// Perform validation before saving
        /// </summary>
        /// <param name="po"></param>
        /// <returns></returns>
        ///  /// <writer>Raghu</writer>
        /// <Date>03-March-2011</Date>
        private static Boolean ValidateBeforeSave(PO po)
        {
            if (po.GetCtx().GetAD_Client_ID() < 0)
            {
                throw new ArgumentException("R/O Context - no new instances allowed");
            }

            Boolean newRecord = po.Is_New();	//	save locally as load resets

            ////	Organization Check
            //String TableTrxType = po.p_info.GetTableTrxType();
            //Boolean orgRequired = X_AD_Table.TABLETRXTYPE_MandatoryOrganization.Equals(TableTrxType);
            //if (po.GetAD_Org_ID() == 0)
            //{
            //    if (!orgRequired)
            //    {
            //        Boolean? shared = MClientShare.IsShared(po.GetAD_Client_ID(), po.Get_Table_ID());
            //        orgRequired = shared != null && !Util.GetValueOfBool(shared);
            //    }
            //    if (orgRequired)
            //    {
            //        s_log.SaveError("FillMandatory", Msg.GetElement(po.GetCtx(), "AD_Org_ID"));
            //        return false;
            //    }
            //}
            //else if (!orgRequired)	//	Org <> 0
            //{
            //    Boolean reset = X_AD_Table.TABLETRXTYPE_NoOrganization.Equals(TableTrxType);
            //    if (!reset)
            //    {
            //        Boolean? shared = MClientShare.IsShared(po.GetAD_Client_ID(), po.Get_Table_ID());
            //        reset = shared != null && Util.GetValueOfBool(shared);
            //    }
            //    if (reset)
            //    {
            //        s_log.Warning("Set Org to 0");
            //        po.SetAD_Org_ID(0);
            //    }
            //}

            ////	Before Save
            //MAssignSet.Execute(po, newRecord);  //	Auto Value Assignment

            if (!POActionEngine.Get().BeforeSave(po))
                return false;
            try
            {
                if (!po.BeforeSave(newRecord))
                {
                    s_log.Warning("beforeSave failed - " + po.ToString());
                    // the subclasses of PO that return false in beforeDelete()
                    // should have already called CLogger.SaveError()
                    //if (!CLogger.hasError())
                    //{
                    //    log.SaveError("Error", "BeforeSave failed", false);
                    //}
                    return false;
                }
            }
            catch (Exception e)
            {
                s_log.Log(Level.WARNING, "beforeSave - " + po.ToString(), e);
                s_log.SaveError("Error", e.ToString(), false);
                return false;
            }
            //	Model Validator
            String errorMsg = ModelValidationEngine.Get().FireModelChange
            (po, newRecord ? ModalValidatorVariables.CHANGETYPE_NEW : ModalValidatorVariables.CHANGETYPE_CHANGE);
            if (errorMsg != null)
            {
                s_log.Warning("Validation failed - " + errorMsg);
                s_log.SaveError("Error", errorMsg);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Create New Record
        /// </summary>
        /// <returns>true if new record inserted</returns>
        ///  ///  /// <writer>Raghu</writer>
        /// <Date>03-March-2011</Date>
        private VAdvantage.Utility.QueryParams GetSaveNewQueryInfo()
        {
            //String[] keyColumns = p_info.GetKeyColumns();
            String[] keyColumns = Get_KeyColumns();
            //  Set ID for single key - Multi-Key values need explicitly be set previously
            if (_mIDs.Length == 1 && p_info.hasKeyColumn()
                    && keyColumns[0].ToUpper().EndsWith("_ID"))	//	AD_Language, EntityType
            {
                int no = SaveNew_GetID();
                if (no <= 0)
                    no = DB.GetNextID(GetAD_Client_ID(), p_info.GetTableName(), Get_Trx());
                if (no <= 0)
                {
                    log.Severe("No NextID (" + no + ")");
                    return null;
                }
                _mIDs[0] = Util.GetValueOfInt(no);
                Set_ValueNoCheck(keyColumns[0], _mIDs[0]);
            }

            //	Set new DocumentNo for non-import tables
            String columnName = "DocumentNo";
            int index = p_info.GetColumnIndex(columnName);
            if (index != -1 && !p_info.GetTableName().StartsWith("I_"))
            {
                String value = (String)Get_Value(index);
                if (value != null && value.StartsWith("<") && value.EndsWith(">"))
                    value = null;
                if (value == null || value.Length == 0)
                {
                    int dtIndex = p_info.GetColumnIndex("C_DocTypeTarget_ID");
                    if (dtIndex == -1)
                        dtIndex = p_info.GetColumnIndex("C_DocType_ID");
                    //if (dtIndex != -1)		//	get based on Doc Type (might return null)
                    //    value = POActionEngine.Get().GetDocumentNo(Get_ValueAsInt(dtIndex), Get_Trx(), GetCtx());
                    //if (value == null)	//	not overwritten by DocType and not manually entered
                    //    value = DB.GetDocumentNo(GetAD_Client_ID(), p_info.GetTableName(), Get_Trx(), GetCtx());
                    value = POActionEngine.Get().GetDocumentNo(dtIndex, this);
                    Set_ValueNoCheck(columnName, value);
                }
            }
            //	Set empty Value
            columnName = "Value";
            index = p_info.GetColumnIndex(columnName);
            if (index != -1)
            {
                String value = (String)Get_Value(index);
                if (value == null || value.Length == 0)
                {
                    // If import table, get the underlying data table to get the "Value"
                    String tableName = p_info.GetTableName();
                    //if (p_info.GetTableName().StartsWith("I_"))
                     //   tableName = GetTable();

                    value = POActionEngine.Get().GetDocumentNo(-1, this);
                    Set_ValueNoCheck(columnName, value);
                }
            }

            LobReset();
            //	Change Log
            MSession session = null;
            if (!(this is MSession))	//	initial session
                session = MSession.Get(p_ctx);
            int AD_ChangeLog_ID = 0;
            Boolean logThis = session != null;
            if (logThis)
                logThis = session.IsLogged(p_info.GetAD_Table_ID(),
                        Get_TableName(), X_AD_ChangeLog.CHANGELOGTYPE_Insert);

            //	SQL
            StringBuilder sqlInsert = new StringBuilder("INSERT INTO ");
            sqlInsert.Append(p_info.GetTableName()).Append(" (");
            StringBuilder sqlValues = new StringBuilder(") VALUES (");

            List<Object> paramst = new List<Object>();

            int size = Get_ColumnCount();
            Boolean doComma = false;
            int count = 0;
            for (int i = 0; i < size; i++)
            {
                String columnName1 = p_info.GetColumnName(i);
                if (p_info.IsVirtualColumn(i))
                    continue;
                Object value = Get_Value(i);
                //	Don't insert NULL values (allows Database defaults)
                if (value == null || value == Null.NULL)
                    continue;

                //	Display Type
                int dt = p_info.GetColumnDisplayType(i);
                if (FieldType.IsLOB(dt))
                {
                    LOBAdd(value, i, dt);
                    continue;
                }

                //	** Add column **
                if (doComma)
                {
                    sqlInsert.Append(",");
                    sqlValues.Append(",");
                }
                else
                    doComma = true;
                sqlInsert.Append(columnName1);
                //
                //  Based on class of definition, not class of value
                Type c = p_info.GetColumnClass(i);
                try
                {
                    if (c == typeof(Object)) // may have need to deal with null
                    // values differently
                    {
                        sqlValues.Append(SaveNewSpecial(value, i));
                    }
                    else if (value == null || value.Equals(Null.NULL))
                    {
                        // sqlValues.Append ("NULL");
                        sqlValues.Append("@param" + count);
                        count++;

                        //string sqlType = Types.VARCHAR;

                        //if( Number.class.isAssignableFrom(c) )
                        //    sqlType = Types.NUMERIC;
                        //else if( Date.class.isAssignableFrom(c) )
                        //    sqlType = Types.TIMESTAMP;
                        //aParams.Add(new NullParameter(sqlType));
                    }
                    else if (value is int || value is Decimal)
                    {
                        // sqlValues.Append (Encrypt(i,value));
                        sqlValues.Append("@param" + count);
                        count++;
                        paramst.Add(Encrypt(i, value));
                    }
                    else if (c == typeof(Boolean))
                    {
                        Boolean bValue = false;
                        if (value is Boolean)
                        {
                            bValue = Util.GetValueOfBool(value);
                        }
                        else
                            bValue = "Y".Equals(value);
                        // sqlValues.Append (Encrypt(i,bValue ? "'Y'" : "'N'"));
                        sqlValues.Append("@param" + count);
                        count++;
                        paramst.Add(Encrypt(i, bValue ? "Y" : "N"));
                    }
                    else if (value is Boolean)
                    {
                        // sqlValues.Append (Encrypt(i,(Boolean)value ? "'Y'" : "'N'"));
                        sqlValues.Append('?');
                        paramst.Add(Encrypt(i, (Boolean)value ? "Y" : "N"));
                    }
                    else if (value is DateTime)
                    {
                        // sqlValues.Append (DB.TO_DATE ((DateTime)Encrypt(i,value), p_info.getColumnDisplayType (i) == DisplayTypeConstants.Date));
                        if (p_info.GetColumnDisplayType(i) == DisplayType.Date)
                        {
                            //If condition added to store time component for the end date in C_Period table 
                            if (!Get_TableName().Equals("C_Period"))
                            {
                                // value = truncDate((DateTime)value);
                                value = Convert.ToDateTime(value);
                            }
                        }
                        sqlValues.Append("@param" + count);
                        count++;
                        paramst.Add(Encrypt(i, value));
                    }
                    else if (c == typeof(String))
                    {
                        // sqlValues.Append (Encrypt(i,DB.TO_STRING ((String)value)));
                        sqlValues.Append("@param" + count);
                        count++;
                        paramst.Add(Encrypt(i, value));
                    }
                    else if (FieldType.IsLOB(dt))
                    {
                        sqlValues.Append("NULL");		//	no db dependent stuff here
                    }
                    else
                    {
                        sqlValues.Append(SaveNewSpecial(value, i));
                    }
                }
                catch (Exception e)
                {
                    String msg = "";
                    if (_trx != null)
                        msg = "[" + _trx + "] - ";
                    msg += p_info.ToString()
                    + " - Value=" + value
                    + "(" + (value == null ? "null" : value.ToString()) + ")";
                    log.Log(Level.SEVERE, msg, e);
                    throw new Exception(e.Message, e);	//	fini
                }

                //	Change Log
                if (session != null && logThis
                        && !p_info.IsEncrypted(i)		//	not encrypted
                        && !p_info.IsVirtualColumn(i)	//	no virtual column
                        && !"Password".Equals(columnName1)
                        && !"CreditCardNumber".Equals(columnName1)
                )
                {
                    Object keyInfo = Get_ID();
                    if (_mIDs.Length != 1)
                        keyInfo = Get_WhereClause(true);
                    MChangeLog cLog = session.ChangeLog(_trx, AD_ChangeLog_ID,
                            p_info.GetAD_Table_ID(), p_info.GetColumn(i).AD_Column_ID,
                            keyInfo, GetAD_Client_ID(), GetAD_Org_ID(), null, value,
                            Get_TableName(), X_AD_ChangeLog.CHANGELOGTYPE_Insert);
                    if (cLog != null)
                        AD_ChangeLog_ID = cLog.GetAD_ChangeLog_ID();
                }
            }
            //	Custom Columns
            int index1;
            if (m_custom != null)
            {
                //Iterator<String> it = m_custom.keySet().iterator();
                IEnumerator<String> it = m_custom.Keys.GetEnumerator();
                while (it.MoveNext())
                {
                    String columnName2 = it.Current;
                    index1 = p_info.GetColumnIndex(columnName2);
                    String value = m_custom[columnName2];
                    if (value == null || value.Equals(Null.NULLString))
                        continue;
                    if (doComma)
                    {
                        sqlInsert.Append(",");
                        sqlValues.Append(",");
                    }
                    else
                        doComma = true;
                    sqlInsert.Append(columnName2);
                    // String value2 = DB.TO_STRING(Encrypt(index, value));
                    // sqlValues.Append(value2);
                    sqlValues.Append("@param" + count);
                    count++;
                    paramst.Add(Encrypt(index1, value));

                    //	Change Log
                    if (session != null
                            && !p_info.IsEncrypted(index1)		//	not encrypted
                            && !p_info.IsVirtualColumn(index1)	//	no virtual column
                            && !"Password".Equals(columnName2)
                            && !"CreditCardNumber".Equals(columnName2)
                    )
                    {
                        MChangeLog cLog = session.ChangeLog(
                                _trx, AD_ChangeLog_ID,
                                p_info.GetAD_Table_ID(), p_info.GetColumn(index1).AD_Column_ID,
                                _mIDs, GetAD_Client_ID(), GetAD_Org_ID(), null, value,
                                Get_TableName(), X_AD_ChangeLog.CHANGELOGTYPE_Insert);
                        if (cLog != null)
                            AD_ChangeLog_ID = cLog.GetAD_ChangeLog_ID();
                    }
                }
                m_custom = null;
            }
            sqlInsert.Append(sqlValues).Append(")");
            //
            return new VAdvantage.Utility.QueryParams(sqlInsert.ToString(), paramst.ToArray());
        }

        private string ReplaceCharWithChar(ref string text, int index, char charToUse)
        {
            char[] tmpBuffer = text.ToCharArray();
            tmpBuffer[index] = charToUse;
            return text = new string(tmpBuffer);
        }

        /// <summary>
        /// Update Record directly
        /// </summary>
        /// <returns> true if updated</returns>
        ///   ///  ///  /// <writer>Raghu</writer>
        /// <Date>04-March-2011</Date>
        protected VAdvantage.Utility.QueryParams GetSaveUpdateQueryInfo()
        {
            //
            Boolean changes = false;
            StringBuilder sql = new StringBuilder("UPDATE ");
            sql.Append(p_info.GetTableName()).Append(" SET ");
            Boolean updated = false;
            Boolean updatedBy = false;
            LobReset();

            //	Change Log
            MSession session = null;
            if (!(this is MSession))
                session = MSession.Get(p_ctx);
            int AD_ChangeLog_ID = 0;
            Boolean logThis = session != null;
            if (logThis)
                logThis = session.IsLogged(p_info.GetAD_Table_ID(),
                        Get_TableName(), X_AD_ChangeLog.CHANGELOGTYPE_Update);

            List<Object> aParams = new List<Object>();

            int size = Get_ColumnCount();
            int count = 0;
            for (int i = 0; i < size; i++)
            {
                Object value = _mNewValues[i];
                if (value == null
                        || p_info.IsVirtualColumn(i))
                    continue;
                //  we have a change
                Type c = p_info.GetColumnClass(i);
                int dt = p_info.GetColumnDisplayType(i);
                String columnName = p_info.GetColumnName(i);
                //
                //	updated/by
                if (columnName.Equals("UpdatedBy"))
                {
                    if (updatedBy)	//	explicit
                        continue;
                    updatedBy = true;
                }
                else if (columnName.Equals("Updated"))
                {
                    if (updated)
                        continue;
                    updated = true;
                }
                if (FieldType.IsLOB(dt))
                {
                    LOBAdd(value, i, dt);
                    //	If no changes set UpdatedBy explicitly to ensure commit of lob
                    if (!changes & !updatedBy)
                    {
                        int AD_User_ID = p_ctx.GetAD_User_ID();
                        Set_ValueNoCheck("UpdatedBy", Util.GetValueOfInt(AD_User_ID));
                        sql.Append("UpdatedBy=@param" + count);
                        count++;
                        aParams.Add(AD_User_ID);
                        changes = true;
                        updatedBy = true;
                    }
                    continue;
                }
                //	Update Document No
                if (columnName.Equals("DocumentNo") && !p_info.GetTableName().StartsWith("I_"))
                {
                    String strValue = (String)value;
                    if (strValue.StartsWith("<") && strValue.EndsWith(">"))
                    {
                        value = null;
                        int AD_Client_ID = GetAD_Client_ID();
                        int docId = -1;
                        int index = p_info.GetColumnIndex("C_DocTypeTarget_ID");
                        if (index == -1)
                            index = p_info.GetColumnIndex("C_DocType_ID");
                        if (index != -1)		//	get based on Doc Type (might return null)
                            docId = Get_ValueAsInt(index);
                        //if (index != -1)		//	get based on Doc Type (might return null)
                        //    value = DB.GetDocumentNo(Get_ValueAsInt(index), _trx, GetCtx());
                        //if (value == null)	//	not overwritten by DocType and not manually entered
                        //    value = DB.GetDocumentNo(AD_Client_ID, p_info.GetTableName(), _trx, GetCtx());
                        value = POActionEngine.Get().GetDocumentNo(docId, this);
                    }
                    else if (!Util.IsEqual(_mOldValues[i], value))
                        log.Info("DocumentNo updated: " + _mOldValues[i] + " -> " + value);
                }

                if (changes)
                    sql.Append(", ");
                changes = true;
                sql.Append(columnName).Append("=");

                //  values
                if (value == Null.NULL)
                {
                    // sql.Append("NULL");
                    // sqlValues.Append ("NULL");
                    sql.Append("@param" + count);
                    count++;

                    //string sqlType = Types.VARCHAR;
                    //if( Number.class.isAssignableFrom(c) )
                    //    sqlType = Types.NUMERIC;
                    //else if( Date.class.isAssignableFrom(c) )
                    //    sqlType = Types.TIMESTAMP;
                    //aParams.Add(new NullParameter(sqlType));
                }
                else if (value is int || value is Decimal)
                {
                    // for Quantity columns, use the incremental form of
                    // UPDATE SET Qty=Qty+? to avoid lost quantity updates due to
                    // concurrent access
                    if (dt == DisplayType.Quantity)
                    {
                        if (value is Decimal)
                        {
                            Decimal oldValue = (Decimal)Get_ValueOld(i);
                            //if (oldValue == null)
                            //    oldValue = Decimal.Zero;
                            Decimal diff = Decimal.Subtract((Decimal)value, oldValue);
                            sql.Append(columnName).Append("+@param" + count);
                            count++;
                            aParams.Add(diff);
                        }
                        else
                        {
                            int? oldValue = (int?)Get_ValueOld(i);
                            if (oldValue == null)
                            {
                                oldValue = 0;
                            }
                            int? diff = Util.GetValueOfInt(value) - oldValue;
                            sql.Append(columnName).Append("+@param" + count);
                            count++;
                            aParams.Add(diff);
                        }
                    }
                    else
                    {
                        // sql.Append(Encrypt(i,value));
                        sql.Append("@param" + count);
                        count++;
                        aParams.Add(Encrypt(i, value));
                    }
                }
                else if (c == typeof(Boolean))
                {
                    Boolean bValue = false;
                    if (value is Boolean)
                    {
                        bValue = Util.GetValueOfBool(value);
                    }
                    else
                        bValue = "Y".Equals(value);
                    // sql.Append(Encrypt(i,bValue ? "'Y'" : "'N'"));
                    sql.Append("@param" + count);
                    count++;
                    aParams.Add(Encrypt(i, bValue ? "Y" : "N"));
                }
                else if (value is DateTime)
                {
                    // sql.Append(DB.TO_DATE((DateTime)Encrypt(i,value),p_info.getColumnDisplayType(i) == DisplayTypeConstants.Date));
                    if (p_info.GetColumnDisplayType(i) == DisplayType.Date)
                    {
                        //If condition added to store time component for the end date in C_Period table 
                        if (!Get_TableName().Equals("C_Period"))
                        {
                            //value = truncDate((DateTime)value);
                            value = Convert.ToDateTime(value);
                        }
                    }
                    sql.Append("@param" + count);
                    count++;
                    aParams.Add(Encrypt(i, value));
                }
                else
                {
                    // sql.Append(Encrypt(i,DB.TO_STRING(value.toString())));
                    sql.Append("@param" + count);
                    count++;
                    aParams.Add(Encrypt(i, value.ToString()));
                }

                //	Change Log	- Only
                if (session != null && logThis
                        && !p_info.IsEncrypted(i)		//	not encrypted
                        && !p_info.IsVirtualColumn(i)	//	no virtual column
                        && !"Password".Equals(columnName)
                        && !"CreditCardNumber".Equals(columnName)
                )
                {
                    Object oldV = _mOldValues[i];
                    Object newV = value;
                    if (oldV != null && oldV == Null.NULL)
                        oldV = null;
                    if (newV != null && newV == Null.NULL)
                        newV = null;
                    //
                    Object keyInfo = Get_ID();
                    if (_mIDs.Length != 1)
                        keyInfo = Get_WhereClause(true);
                    MChangeLog cLog = session.ChangeLog(
                            _trx, AD_ChangeLog_ID,
                            p_info.GetAD_Table_ID(), p_info.GetColumn(i).AD_Column_ID,
                            keyInfo, GetAD_Client_ID(), GetAD_Org_ID(), oldV, newV,
                            Get_TableName(), X_AD_ChangeLog.CHANGELOGTYPE_Update);
                    if (cLog != null)
                        AD_ChangeLog_ID = cLog.GetAD_ChangeLog_ID();
                }
            }	//   for all fields

            //	Custom Columns (cannot be logged as no column)
            if (m_custom != null)
            {
                IEnumerator<String> it = m_custom.Keys.GetEnumerator();
                while (it.MoveNext())
                {
                    if (changes)
                        sql.Append(", ");
                    changes = true;
                    //
                    String columnName = it.Current;
                    int index = p_info.GetColumnIndex(columnName);
                    String value = m_custom[columnName];
                    // sql.Append(columnName).Append("=").Append(value);	//	no encryption
                    sql.Append("@param" + count);
                    count++;
                    if (value != null && value.Equals(Null.NULLString))
                    {
                        aParams.Add(Types.VARCHAR);
                    }
                    else
                    {
                        aParams.Add(Encrypt(index, value));
                    }


                    //	Change Log
                    if (session != null
                            && !p_info.IsEncrypted(index)		//	not encrypted
                            && !p_info.IsVirtualColumn(index)	//	no virtual column
                            && !"Password".Equals(columnName)
                            && !"CreditCardNumber".Equals(columnName)
                    )
                    {
                        Object oldV = _mOldValues[index];
                        String newV = value;
                        if (oldV != null && oldV == Null.NULL)
                            oldV = null;
                        //
                        MChangeLog cLog = session.ChangeLog(
                                _trx, AD_ChangeLog_ID,
                                p_info.GetAD_Table_ID(), p_info.GetColumn(index).AD_Column_ID,
                                _mIDs, GetAD_Client_ID(), GetAD_Org_ID(), oldV, newV,
                                Get_TableName(), X_AD_ChangeLog.CHANGELOGTYPE_Update);
                        if (cLog != null)
                            AD_ChangeLog_ID = cLog.GetAD_ChangeLog_ID();
                    }
                }
                m_custom = null;
            }

            //	Something changed
            if (changes)
            {
                if (!updated)	//	Updated not explicitly set
                {
                    DateTime now = DateTime.Now.Date;
                    Set_ValueNoCheck("Updated", now);
                    sql.Append(",Updated=@param" + count);
                    count++;
                    aParams.Add(now);
                }
                if (!updatedBy)	//	UpdatedBy not explicitly set
                {
                    int AD_User_ID = p_ctx.GetAD_User_ID();
                    Set_ValueNoCheck("UpdatedBy", Util.GetValueOfInt(AD_User_ID));
                    sql.Append(",UpdatedBy=@param" + count);
                    count++;
                    aParams.Add(AD_User_ID);
                }
                sql.Append(" WHERE ").Append(GetUpdateWhereClause(false, count));
                aParams.AddRange(Get_WhereClauseParams());
                //
                log.Finest(sql.ToString());
                return new VAdvantage.Utility.QueryParams(sql.ToString(), aParams.ToArray());
            }

            //  nothing changed, so OK
            return null;
        }

        /// <summary>
        /// Where house paramert
        /// </summary>
        /// <returns></returns>
        /// <date>08-March-2011</date>
        /// <writer>Raghu</writer>
        public List<Object> Get_WhereClauseParams()
        {
            List<Object> param = new List<Object>();
            foreach (Object mID in _mIDs)
            {
                if (mID is int)
                {
                    param.Add(mID);
                }
                else
                {
                    param.Add(mID.ToString());
                }
            }
            return param;
        }

        public int Get_ValueAsInt(int index)
        {
            Object value = Get_Value(index);
            if (value == null)
                return 0;
            if (value is int)
                return ((int)value);
            try
            {
                return int.Parse(value.ToString());
            }
            catch (Exception ex)
            {
                log.Warning(p_info.GetColumnName(index) + " - " + ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// Get Value as Integer
        /// </summary>
        /// <param name="columnName">column name</param>
        /// <returns>value or 0 if null</returns>
        /// <date>15-March-2011</date>
        /// <writer>Raghu</writer>
        public int Get_ValueAsInt(String columnName)
        {
            int index = Get_ColumnIndex(columnName);
            if (index < 0)
            {
                log.Log(Level.WARNING, "Column not found - "
                        + Get_TableName() + "." + columnName);

                return 0;
            }
            return Get_ValueAsInt(index);
        }

        //private String GetTable()
        //{
        //    String tableName = p_info.GetTableName();
        //    if (!tableName.StartsWith("I_"))
        //    {
        //        return p_info.GetTableName();
        //    }

        //    tableName = p_info.GetTableName().Replace("I_", "AD_");
        //    int tableID = MTable.Get_Table_ID(tableName);
        //    if (tableID > 0)
        //        return tableName;

        //    tableName = p_info.GetTableName().Replace("I_", "M_");
        //    tableID = MTable.Get_Table_ID(tableName);
        //    if (tableID > 0)
        //        return tableName;

        //    tableName = p_info.GetTableName().Replace("I_", "C_");
        //    tableID = MTable.Get_Table_ID(tableName);
        //    if (tableID > 0)
        //        return tableName;

        //    tableName = p_info.GetTableName().Replace("I_", "R_");
        //    tableID = MTable.Get_Table_ID(tableName);
        //    if (tableID > 0)
        //        return tableName;

        //    tableName = p_info.GetTableName().Replace("I_", "MRP_");
        //    tableID = MTable.Get_Table_ID(tableName);
        //    if (tableID > 0)
        //        return tableName;

        //    tableName = p_info.GetTableName().Replace("I_", "A_");
        //    tableID = MTable.Get_Table_ID(tableName);
        //    if (tableID > 0)
        //        return tableName;

        //    tableName = p_info.GetTableName().Replace("I_", "B_");
        //    tableID = MTable.Get_Table_ID(tableName);
        //    if (tableID > 0)
        //        return tableName;

        //    tableName = p_info.GetTableName().Replace("I_", "CM_");
        //    tableID = MTable.Get_Table_ID(tableName);
        //    if (tableID > 0)
        //        return tableName;

        //    tableName = p_info.GetTableName().Replace("I_", "GL_");
        //    tableID = MTable.Get_Table_ID(tableName);
        //    if (tableID > 0)
        //        return tableName;

        //    tableName = p_info.GetTableName().Replace("I_", "K_");
        //    tableID = MTable.Get_Table_ID(tableName);
        //    if (tableID > 0)
        //        return tableName;

        //    tableName = p_info.GetTableName().Replace("I_", "PA_");
        //    tableID = MTable.Get_Table_ID(tableName);
        //    if (tableID > 0)
        //        return tableName;

        //    tableName = p_info.GetTableName().Replace("I_", "S_");
        //    tableID = MTable.Get_Table_ID(tableName);
        //    if (tableID > 0)
        //        return tableName;

        //    tableName = p_info.GetTableName().Replace("I_", "T_");
        //    tableID = MTable.Get_Table_ID(tableName);
        //    if (tableID > 0)
        //        return tableName;

        //    tableName = p_info.GetTableName().Replace("I_", "W_");
        //    tableID = MTable.Get_Table_ID(tableName);
        //    if (tableID > 0)
        //        return tableName;

        //    return p_info.GetTableName();

        //}

        /// <summary>
        /// Create/return Attachment for PO.
        /// If not exist, create new
        /// </summary>
        /// <returns>attachment</returns>
        /// <date>07-march-2011</date>
        /// <writer>raghu</writer>
        public dynamic CreateAttachment()
        {
            GetAttachment(false);
            if (_attachment == null)
                _attachment = POActionEngine.Get().CreateAttachment(GetCtx(), p_info.GetAD_Table_ID(), Get_ID(), null);
            return _attachment;
        }

        public void ExecuteWF()
        {
            if (s_docWFMgr == null)
            {
                try
                {
                    //s_docWFMgr = (DocWorkflowMgr)Activator.GetObject(typeof(DocWorkflowMgr), "VAdvantage.WF.DocWorkflowManager");
                    System.Reflection.Assembly asm = System.Reflection.Assembly.Load("VAWorkflow");
                    s_docWFMgr = (DocWorkflowMgr)asm.GetType("VAdvantage.WF.DocWorkflowManager").GetMethod("Get").Invoke(null, null);
                    //s_docWFMgr = DocWorkflowManager.Get();
                }
                catch
                {
                }
            }
            if (s_docWFMgr != null)
            {
                s_docWFMgr.Process(this, p_info.getAD_Table_ID());
            }

        }
    }



}