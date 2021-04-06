using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using VAdvantage.SqlExec;

using VAdvantage.Logging;
using VAdvantage.Utility;
using BaseLibrary.Model;

namespace VAdvantage.Model
{
    public class MVAFTableView : X_VAF_TableView
    {
        private static VLogger s_log = VLogger.GetVLogger(typeof(MVAFTableView).FullName);
        private MViewComponent[] m_vcs = null;
        private MVAFColumn[] m_columns = null;
        
        /**
	 * 	Load Constructor
	 *	@param ctx context
	 *	@param rs result set
	 *	@param trxName transaction
	 */
   
        public MVAFTableView(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        public MVAFTableView(Ctx ctx, IDataReader rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /**************************************************************************
      * 	Standard Constructor
      *	@param ctx context
      *	@param VAF_TableView_ID id
      *	@param trxName transaction
      */
        public MVAFTableView(Ctx ctx, int VAF_TableView_ID, Trx trxName)
            : base(ctx, VAF_TableView_ID, trxName)
        {
            //super(ctx, VAF_TableView_ID, trxName);
            if (VAF_TableView_ID == 0)
            {
                //	setName (null);
                //	setTableName (null);
                SetAccessLevel(ACCESSLEVEL_SystemOnly);	// 4
                SetRecordType(RecordType_UserMaintained);	// U
                SetIsChangeLog(false);
                SetIsDeleteable(false);
                SetIsHighVolume(false);
                SetIsSecurityEnabled(false);
                SetIsView(false);	// N
                SetReplicationType(REPLICATIONTYPE_Local);
            }
        }	//	MVAFTableView

        /**
	 * 	Get SQL Create statement
	 *	@return sql statement
	 */
        
        private static CCache<int, MVAFTableView> s_cache = new CCache<int, MVAFTableView>("VAF_TableView", 20);

        private static CCache<int, bool> s_cacheHasKey = new CCache<int, bool>("VAF_TableView_key", 20);

        public String GetSQLCreate()
        {
            return GetSQLCreate(true);
        }	// getSQLCrete
        // getSQLCrete

        /**
      * 	Get SQL Create
      * 	@param requery refresh columns
      *	@return create table DDL
      */
        public String GetSQLCreate(bool requery)
        {
            StringBuilder sb = new StringBuilder("CREATE TABLE ")
                .Append(GetTableName()).Append(" (");
            //
            bool hasPK = false;
            bool hasParents = false;
            bool firstColumn = true;
            StringBuilder constraints = new StringBuilder();
            StringBuilder unqConstraints = new StringBuilder();
            GetColumns(requery);
            for (int i = 0; i < m_columns.Length; i++)
            {
                MVAFColumn column = m_columns[i];
                if (column.IsVirtualColumn())
                {
                    continue;
                }
                if (firstColumn)
                {
                    firstColumn = false;
                }
                else
                {
                    sb.Append(", ");
                }
                sb.Append(column.GetSQLDDL());
                //
                if (column.IsKey())
                {
                    constraints.Append(", CONSTRAINT PK").Append(GetVAF_TableView_ID())
                        .Append(" PRIMARY KEY (").Append(column.GetColumnName()).Append(")");
                    hasPK = true;
                }
                if (column.IsParent())
                    hasParents = true;

                if (column.IsUnique())
                {
                    if (unqConstraints.Length == 0)
                    {
                        unqConstraints.Append(", CONSTRAINT UK").Append(GetVAF_TableView_ID())
                        .Append(" UNIQUE (").Append(column.GetColumnName()); 
                    }
                    else
                        unqConstraints.Append(",").Append(column.GetColumnName());
                }
            }
            //	Multi Column PK 
            if (!hasPK && hasParents)
            {
                StringBuilder cols = new StringBuilder();
                for (int i = 0; i < m_columns.Length; i++)
                {
                    MVAFColumn column = m_columns[i];
                    if (!column.IsParent())
                        continue;
                    if (cols.Length > 0)
                        cols.Append(", ");
                    cols.Append(column.GetColumnName());
                }
                sb.Append(", CONSTRAINT PK").Append(GetVAF_TableView_ID())
                    .Append(" PRIMARY KEY (").Append(cols).Append(")");
            }
            if (unqConstraints.Length > 0)
            {
                unqConstraints.Append(")");
            }

            sb.Append(constraints);
            sb.Append(unqConstraints);
            sb.Append(")");
            return sb.ToString();
        }	//	getSQLCreate

        /**
         * 	Get Columns
         *	@param requery requery
         *	@return array of columns
         */
        public MVAFColumn[] GetColumns(bool requery)
        {
            if (m_columns != null && !requery)
                return m_columns;
            String sql = "SELECT * FROM VAF_Column WHERE VAF_TableView_ID=@VAF_TableView_Id ORDER BY ColumnName";

            List<MVAFColumn> list = GetCols(sql);
            //
            m_columns = new MVAFColumn[list.Count];
            m_columns = list.ToArray();
            return m_columns;
        }	//	getColumns

        /**
      * Get the MColumn
      * @param sql 
      * @return list of columns
      */
        private List<MVAFColumn> GetCols(string sql)
        {
            List<MVAFColumn> list = new List<MVAFColumn>();

            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@VAF_TableView_Id", GetVAF_TableView_ID());
                //DataSet ds = ExecuteQuery.ExecuteDataset(sql, param);
                DataSet ds = DB.ExecuteDataset(sql, param, Get_Trx());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    list.Add(new MVAFColumn(GetCtx(), dr, Get_TrxName()));
                }
                ds = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            //try
            //{
            //    if (pstmt != null)
            //        pstmt.close();
            //    pstmt = null;
            //}
            //catch (Exception e)
            //{
            //    pstmt = null;
            //}

            return list;
        }  // getColumns()

        /**
     * 	Get Table from Cache
     *	@param ctx context
     *	@param VAF_TableView_ID id
     *	@return MVAFTableView
     */
        public static MVAFTableView Get(Ctx ctx, int VAF_TableView_ID)
        {
            int key = VAF_TableView_ID;
            MVAFTableView retValue = null;
            s_cache.TryGetValue(key, out retValue);
            if (retValue != null)
                return retValue;
            retValue = new MVAFTableView(ctx, VAF_TableView_ID, null);
            if (retValue.Get_ID() != 0)
                s_cache.Add(key, retValue);
            return retValue;

        }

        /// <summary>
        /// Get Po Object of table
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="tableName"></param>
        /// <param name="Record_ID"></param>
        /// <param name="trxName"></param>
        /// <returns></returns>
        public static PO GetPO(Ctx ctx, string tableName, int Record_ID, Trx trxName)
        {
            var tableId = MVAFTableView.Get_Table_ID(tableName);
            MVAFTableView tbl = new MVAFTableView(ctx, tableId, trxName);
            String TableName = tbl.GetTableName();
            if (TableName == null)
            {
                return null;
            }
            var locpo = tbl.GetPO(ctx, Record_ID, trxName);
            return locpo;
        }

        /// <summary>
        /// Get Table from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="tableName">case insensitive table name</param>
        /// <returns>able or null</returns>
        /// <author>Raghunandan</author>
        public static MVAFTableView Get(Ctx ctx, string tableName)
        {
            //Uesd in translationTable Class 
            if (tableName == null)
                return null;
            //	Check cache
            //iterator<MVAFTableView> it = s_cache.Values().iterator();
            IEnumerator<MVAFTableView> it = s_cache.Values.GetEnumerator();
            it.Reset();
            while (it.MoveNext())
            //foreach(MVAFTableView mtable in it)
            {
                //MVAFTableView retValue = (MVAFTableView)it.next();
                MVAFTableView retValue1 = (MVAFTableView)it.Current;
                //if (tableName.equalsIgnoreCase(retValue.getTableName()))
                if (tableName.ToLower().Equals(retValue1.GetTableName().ToLower()))
                    return retValue1;
            }
            //	Get direct
            MVAFTableView retValue = null;
            String sql = "SELECT * FROM VAF_TableView WHERE UPPER(TableName)=UPPER('" + tableName + "')";
            DataSet ds = null;
            try
            {
                ds = CoreLibrary.DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = ds.Tables[0].Rows[i];
                    retValue = new MVAFTableView(ctx, rs, null);
                }
            }
            catch (Exception e)
            {
                // 
                s_log.Log(Level.SEVERE, sql, e);

            }
            ds = null;
            if (retValue != null)
            {
                int key = retValue.GetVAF_TableView_ID();
                s_cache.Add(key, retValue);
            }
            return retValue;
        }

        /// <summary>
        /// Get Table Name
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_TableView_ID">table id</param>
        /// <returns>table name</returns>
        public static String GetTableName(Ctx ctx, int VAF_TableView_ID)
        {
            return MVAFTableView.Get(ctx, VAF_TableView_ID).GetTableName();
        }

        public bool Haskey(int VAF_TableView_ID)
        {
            if (s_cacheHasKey.Count == 0)
            {
                LoadHasKeyData();
            }
            if (s_cacheHasKey.ContainsKey(VAF_TableView_ID))
            {
                return s_cacheHasKey[VAF_TableView_ID];
            }
            return false;
        }

        private void LoadHasKeyData()
        {
            string sql = "SELECT t.VAF_TableView_ID , (SELECT COUNT(VAF_Column_ID) FROM VAF_Column WHERE UPPER(ColumnName) = UPPER(t.TableName)||'_ID' AND VAF_TableView_ID=t.VAF_TableView_ID AND IsActive='Y' ) as hasKey FROM VAF_TableView t";
            IDataReader dr = null;
            try
            {
                dr = DB.ExecuteReader(sql);
                s_cacheHasKey.Clear();
                while (dr.Read())
                {
                    s_cacheHasKey[dr.GetInt32(0)] = dr.GetInt32(1) > 0;
                }
                dr.Close();
            }
            catch
            {
                if (dr != null)
                    dr.Close();
            }
        }

        /******  Created By Harwinder*******************/

        /// <summary>
        /// Get PO Class Instance
        /// </summary>
        /// <param name="ctx">context for PO</param>
        /// <param name="whereClause">where clause resulting in single record</param>
        /// <param name="trxName">trxName transaction</param>
        /// <returns>PO for Record or null</returns>
        public PO GetPO(Ctx ctx, String whereClause, Trx trxName)
        {
            if (whereClause == null || whereClause.Length == 0)
                return null;
            //
            PO po = null;
            StringBuilder sql = new StringBuilder("SELECT ")
                .Append(GetSelectColumns())
                .Append(" FROM ").Append(GetTableName())
                .Append(" WHERE ").Append(whereClause);

            DataTable dt = null;
            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql.ToString(), null, trxName);
                dt = new DataTable();
                dt.Load(dr);
                dr.Close();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    po = GetPO(ctx, dt.Rows[i], trxName);
                }
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                log.Severe(e.ToString());

                // lof error
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
                dt = null;
            }
            if (po == null)
                return GetPO(ctx, 0, trxName);
            return po;
        }

        /// <summary>
        ///Get PO Class Instance
        /// </summary>
        /// <param name="ctx">context for PO</param>
        /// <param name="rs">Datarow</param>
        /// <param name="trxName">trxName transaction</param>
        /// <returns>PO for Record or null</returns>
        public PO GetPO(Ctx ctx, DataRow dr, Trx trxName)
        {
            String tableName = GetTableName();

            PO po = null;
            List<IModelFactory> factoryList = VAModelAD.Classes.ModelFactoryLoader.GetList();

            if (factoryList != null)
            {
                foreach (IModelFactory factory in factoryList)
                {
                    po = factory.GetPO(tableName, ctx, dr, trxName);
                    if (po != null)
                    {
                            break;
                    }
                }
            }





            //Type clazz = GetClass(tableName);
            if (po == null)
            {
                //log.log(Level.SEVERE, "(rs) - Class not found for " + tableName);
                //ErrorLog.FillErrorLog("MVAFTableView.GetPO", "(rs) - Class not found for " + tableName, "", VAdvantage.Framework.Message.MessageType.ERROR);
                //return null;

                //Updateby--Raghu
                //to run work flow with virtual M_ or X_ classes
                log.Log(Level.INFO, "Using GenericPO for " + tableName);
                 po = new GenericPO(tableName, ctx, dr, trxName);
                return po;
            }
            
            return po;
        }

        /// <summary>
        /// Get PO Class Instance
        /// </summary>
        /// <param name="ctx">context for PO</param>
        /// <param name="Record_ID">Record_ID record - 0 = new</param>
        /// <param name="trxName">transaction name</param>
        /// <returns>PO for Record or null</returns>
        public PO GetPO(Ctx ctx, int Record_ID, Trx trxName)
        {
            return GetPO(ctx, Record_ID, trxName, true);
        }

        /// <summary>
        /// Get PO Class Instance
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="Record_ID"> record id</param>
        /// <param name="trxName">trx name</param>
        /// <param name="isNew">is new Record</param>
        /// <returns>PO or null</returns>
        public PO GetPO(Ctx ctx, int Record_ID, Trx trxName, bool isNew)
        {
            string tableName = GetTableName();
            if (Record_ID != 0 && !IsSingleKey())
            {
                log.Log(Level.WARNING, "(id) - Multi-Key " + tableName);
                return null;
            }
            PO po = null;
            List<IModelFactory> factoryList = VAModelAD.Classes.ModelFactoryLoader.GetList();
            if (factoryList != null)
            {
                foreach (IModelFactory factory in factoryList)
                {
                    po = factory.GetPO(tableName, ctx, Record_ID, trxName);
                    if (po != null)
                    {
                        if (!isNew && Record_ID == 0)
                            po.Load(trxName);
                        if (po.Get_ID() != Record_ID && Record_ID > 0)
                        {
                            po = null;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            if (po == null)
            {
                log.Log(Level.INFO, "Using GenericPO for " + tableName);
                po = new GenericPO(tableName, ctx, Record_ID, trxName);
                return po;
            }
           
            
            return po;
        }
        
        /**
	 * 	String Representation
	 *	@return info
	 */
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("MVAFTableView[");
            sb.Append(Get_ID()).Append("-").Append(GetTableName()).Append("]");
            return sb.ToString();
        }	//	
                     
        /// <summary>
        ///Get list of columns for SELECT statement.
        ///	Handles virtual columns
        /// </summary>
        /// <returns>select columns</returns>
        public String GetSelectColumns()
        {
            GetColumns(false);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < m_columns.Length; i++)
            {
                MVAFColumn col = m_columns[i];
                if (i > 0)
                    sb.Append(",");
                if (col.IsVirtualColumn())
                    sb.Append(col.GetColumnSQL()).Append(" AS ");
                sb.Append(col.GetColumnName());
            }
            return sb.ToString();
        }	//	getSelectColum
               
        /// <summary>
        ///Table has a single Key
        /// </summary>
        /// <returns>true if table has single key column</returns>
        public bool IsSingleKey()
        {
            String[] keys = GetKeyColumns();
            return keys.Length == 1;
        }	//	isSingleKey
        
        /// <summary>
        ///Get Key Columns of Table
        /// </summary>
        /// <returns>key columns</returns>
        public String[] GetKeyColumns()
        {
            List<String> list = new List<String>();
            GetColumns(false);
            for (int i = 0; i < m_columns.Length; i++)
            {
                MVAFColumn column = m_columns[i];
                if (column.IsKey())
                    return new String[] { column.GetColumnName() };
                if (column.IsParent())
                    list.Add(column.GetColumnName());
            }
            String[] retValue;
            retValue = list.ToArray();
            return retValue;
        }	//	getKeyColumns
        //	getKeyColumns

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true</returns>
        /// 
        protected override bool BeforeSave(bool newRecord)
        {
            if (IsView() && IsDeleteable())
                SetIsDeleteable(false);

            // check applied for maintain Versions 
            // if there is change in maintain Versions checkbox, then before unchecking need to check if
            // any other column is not marked as maintainversions and data is there in Version table then do not allow to uncheck maintain version on Table
            // else allow to change
            if (Is_ValueChanged("IsMaintainVersions"))
            {
                if (!IsMaintainVersions())
                {
                    string verTblName = GetTableName() + "_Ver";
                    if (HasVersionData(verTblName))
                    {
                        int countMtnVerCol = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_Column_ID) FROM VAF_Column WHERE VAF_TableView_ID = " + GetVAF_TableView_ID() + " AND IsMaintainVersions = 'Y'"));
                        if (countMtnVerCol == 0)
                            return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// if there is data in version table passed in parameter then return true else false
        /// </summary>
        /// <param name="TblName"></param>
        /// <returns>True/False</returns>
        public bool HasVersionData(string TblName)
        {
            if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_TableView_ID) FROM VAF_TableView WHERE TableName = '" + TblName + "'", null, Get_Trx())) > 0)
            {
                int countRec = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_Client_ID) FROM " + TblName, null, Get_TrxName()));
                if (countRec > 0)
                {
                    log.SaveError("VersionDataExists", Utility.Msg.GetElement(GetCtx(), "VersionDataExists"));
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">newRecord new</param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success)
                return false;
            //	Sync Table ID
            if (newRecord)
                MVAFRecordSeq.CreateTableSequence(GetCtx(), GetTableName(), Get_TrxName());
            else
            {
                MVAFRecordSeq seq = MVAFRecordSeq.Get(GetCtx(), GetTableName(), Get_TrxName());
                if (seq == null || seq.Get_ID() == 0)
                    MVAFRecordSeq.CreateTableSequence(GetCtx(), GetTableName(), Get_TrxName());
                else if (!seq.GetName().Equals(GetTableName()))
                {
                    seq.SetName(GetTableName());
                    seq.Save();
                }
            }
            // checked if value is changed in Maintain Version
            if (Is_ValueChanged("IsMaintainVersions"))
            {
                if (!newRecord && IsMaintainVersions())
                {
                    int ColID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT VAF_Column_ID FROM VAF_Column WHERE IsActive = 'Y' AND VAF_TableView_ID = " + GetVAF_TableView_ID() + " ORDER BY ColumnName", null, Get_Trx()));
                    if (ColID > 0)
                    {
                        MVAFColumn column = new MVAFColumn(GetCtx(), ColID, Get_Trx());
                        MasterVersions mv = new MasterVersions();
                        string versionMsg = mv.CreateVersionInfo(column.GetVAF_Column_ID(), column.GetVAF_TableView_ID(), Get_Trx());
                    }
                }
            }
            return success;
        }	//	afterSave

        /// <summary>
        /// 	After Delete
        /// </summary>
        /// <param name="success">success</param>
        /// <returns>true if success</returns>
        protected override Boolean AfterDelete(Boolean success)
        {
            if (success)
                MVAFRecordSeq.DeleteTableSequence(GetCtx(), GetTableName(), Get_TrxName());
            return success;
        }	//	afterDelete
        /**********************END**********************************/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public MVAFColumn GetColumn(String columnName)
        {
            if (string.IsNullOrEmpty(columnName))
                return null;
            GetColumns(false);
            //
            foreach (MVAFColumn element in m_columns)
            {
                if (columnName.Equals(element.GetColumnName(), StringComparison.OrdinalIgnoreCase))
                    return element;
            }
            return null;
        }	//	getColumn

        public MVAFColumn GetIdentifierCol()
        {
            GetColumns(false);

            foreach (MVAFColumn element in m_columns)
            {
                if (element.IsIdentifier())
                    return element;
            }

            return null;
        }
        /// <summary>
        /// Get all active Tables with input sql	 
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="sql">query to get tables</param>
        /// <returns>array of tables</returns>

        public static List<MVAFTableView> GetTablesByQuery(Ctx ctx, String sql)
        {
            IDataReader idr = null;
            List<MVAFTableView> list = new List<MVAFTableView>();
            try
            {
                idr = CoreLibrary.DataBase.DB.ExecuteReader(sql);
                while (idr.Read())
                {
                    MVAFTableView table = new MVAFTableView(ctx, idr, null);
                    /**
                    String s = table.getSQLCreate();
                    HashMap hmt = table.get_HashMap();
                    MColumn[] columns = table.getColumns(false);
                    HashMap hmc = columns[0].get_HashMap();
                     **/
                    list.Add(table);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                idr.Close();
                s_log.Log(Level.SEVERE, sql, e);
            }
            return list;
        }
        /// <summary>
        ///Get SQL Create View 
        /// </summary>
        /// <param name="requery">refresh columns</param>
        /// <returns>create view DDL</returns>
        public String GetViewCreate(bool requery)
        {
            if (!IsView() || !IsActive())
            {
                return null;
            }
            StringBuilder sb = new StringBuilder("CREATE OR REPLACE VIEW ")
            .Append(GetTableName());
            //
            this.GetViewComponent(requery);
            if (m_vcs == null || m_vcs.Length == 0)
                return null;

            MViewColumn[] vCols = null;
            for (int i = 0; i < m_vcs.Length; i++)
            {
                MViewComponent vc = m_vcs[i];
                if (i > 0)
                    sb.Append(" UNION ");
                else
                {
                    vCols = vc.GetColumns(requery);
                    if (vCols == null || vCols.Length == 0)
                        return null;
                    bool right = false;
                    for (int j = 0; j < vCols.Length; j++)
                    {
                        if (vCols[j].GetColumnName().Equals("*"))
                            break;
                        if (j == 0)
                        {
                            sb.Append("(");
                            right = true;
                        }
                        else
                            sb.Append(", ");
                        sb.Append(vCols[j].GetColumnName());
                    }

                    if (right)
                        sb.Append(")");
                    sb.Append(" AS ");
                }

                sb.Append(vc.GetSelect(false, vCols));
            }
            return sb.ToString();
        }

        /// <summary>
        ///Get MViewComponent Class Instances 
        /// </summary>
        /// <param name="reload">reload boolean if it need to reload </param>
        /// <returns>Array of MViewComponent or null</returns>
        public MViewComponent[] GetViewComponent(bool reload)
        {
            if (!IsView() || !IsActive())
                return null;

            if (m_vcs != null && !reload)
                return m_vcs;
            //
            List<MViewComponent> list = new List<MViewComponent>();
            String sql = "SELECT * FROM VAF_DBViewElement WHERE VAF_TableView_ID = " + this.GetVAF_TableView_ID() + " AND IsActive = 'Y'";
            IDataReader idr = null;
            try
            {
                idr = CoreLibrary.DataBase.DB.ExecuteReader(sql, null, Get_Trx());
                while (idr.Read())
                {
                    list.Add(new MViewComponent(GetCtx(), idr, Get_Trx()));
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            //
            m_vcs = new MViewComponent[list.Count];
            m_vcs = list.ToArray();
            return m_vcs;
        }

    }
}
