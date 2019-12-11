using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Common;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using VAdvantage.SqlExec;
using System.Reflection;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MTable : X_AD_Table
    {

        private static VLogger s_log = VLogger.GetVLogger(typeof(MTable).FullName);


        private MViewComponent[] m_vcs = null;
        /**	Special Classes				*/
        private static String[] _special = new String[] {
		"AD_Element", "VAdvantage.Model.M_Element",
		"AD_Registration", "VAdvantage.Model.M_Registration",
		"AD_Tree", "VAdvantage.Model.MTree_Base",
		"R_Category", "VAdvantage.Model.MRequestCategory",
		"GL_Category", "VAdvantage.Model.MGLCategory",
		"K_Category", "VAdvantage.Model.MKCategory",
		"C_ValidCombination", "VAdvantage.Model.MAccount",
		"C_Phase", "VAdvantage.Model.MProjectTypePhase",
		"C_Task", "VAdvantage.Model.MProjectTypeTask",
		"K_Source", "VAdvantage.Model.X_K_Source",
        "RC_ViewColumn","VAdvantage.Model.X_RC_ViewColumn",
	//	AD_Attribute_Value, AD_TreeNode
	    };

        private static String[] _projectClasses = new String[]{
            "ViennaAdvantage.Model.M",
            "ViennaAdvantage.Process.M",
            "ViennaAdvantage.CMFG.Model.M",
            "ViennaAdvantage.CMRP.Model.M",
            "ViennaAdvantage.CWMS.Model.M",
            "ViennaAdvantage.Model.X_"

        };

        private static String[] _productClasses = new String[]{
            "VAdvantage.Model.M",
            "VAdvantage.Model.X_",
            "VAdvantage.Process.M",
            "VAdvantage.WF.M",
            "VAdvantage.Report.M",
            " VAdvantage.ProcessEngine.M",
            "VAdvantage.Print.M"

        };


        private static String[] _moduleClasses = new String[]{
            ".Model.M",
            ".Process.M",
            ".WF.M",
            ".Report.M",
            ".Print.M",
            ".CMFG.Model.M",
            ".CMRP.Model.M",
            ".CWMS.Model.M",
            ".Model.X_",

        };



        private MColumn[] m_columns = null;
        /**
	 * 	Load Constructor
	 *	@param ctx context
	 *	@param rs result set
	 *	@param trxName transaction
	 */
        public MTable(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        public MTable(Ctx ctx, IDataReader rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /**************************************************************************
      * 	Standard Constructor
      *	@param ctx context
      *	@param AD_Table_ID id
      *	@param trxName transaction
      */
        public MTable(Ctx ctx, int AD_Table_ID, Trx trxName)
            : base(ctx, AD_Table_ID, trxName)
        {
            //super(ctx, AD_Table_ID, trxName);
            if (AD_Table_ID == 0)
            {
                //	setName (null);
                //	setTableName (null);
                SetAccessLevel(ACCESSLEVEL_SystemOnly);	// 4
                SetEntityType(ENTITYTYPE_UserMaintained);	// U
                SetIsChangeLog(false);
                SetIsDeleteable(false);
                SetIsHighVolume(false);
                SetIsSecurityEnabled(false);
                SetIsView(false);	// N
                SetReplicationType(REPLICATIONTYPE_Local);
            }
        }	//	MTable

        /**
	 * 	Get SQL Create statement
	 *	@return sql statement
	 */

        //    /**	Cache						*/
        private static CCache<int, MTable> s_cache = new CCache<int, MTable>("AD_Table", 20);

        private static CCache<int, bool> s_cacheHasKey = new CCache<int, bool>("AD_Table_key", 20);

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
            GetColumns(requery);
            for (int i = 0; i < m_columns.Length; i++)
            {
                MColumn column = m_columns[i];
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
                    constraints.Append(", CONSTRAINT PK").Append(GetAD_Table_ID())
                        .Append(" PRIMARY KEY (").Append(column.GetColumnName()).Append(")");
                    hasPK = true;
                }
                if (column.IsParent())
                    hasParents = true;
            }
            //	Multi Column PK 
            if (!hasPK && hasParents)
            {
                StringBuilder cols = new StringBuilder();
                for (int i = 0; i < m_columns.Length; i++)
                {
                    MColumn column = m_columns[i];
                    if (!column.IsParent())
                        continue;
                    if (cols.Length > 0)
                        cols.Append(", ");
                    cols.Append(column.GetColumnName());
                }
                sb.Append(", CONSTRAINT PK").Append(GetAD_Table_ID())
                    .Append(" PRIMARY KEY (").Append(cols).Append(")");
            }

            sb.Append(constraints)
                .Append(")");
            return sb.ToString();
        }	//	getSQLCreate



        /**
         * 	Get Columns
         *	@param requery requery
         *	@return array of columns
         */
        public MColumn[] GetColumns(bool requery)
        {
            if (m_columns != null && !requery)
                return m_columns;
            String sql = "SELECT * FROM AD_Column WHERE AD_Table_ID=@AD_Table_Id ORDER BY ColumnName";

            List<MColumn> list = GetCols(sql);
            //
            m_columns = new MColumn[list.Count];
            m_columns = list.ToArray();
            return m_columns;
        }	//	getColumns
        /**
      * Get the MColumn
      * @param sql 
      * @return list of columns
      */
        private List<MColumn> GetCols(string sql)
        {
            List<MColumn> list = new List<MColumn>();

            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@AD_Table_Id", GetAD_Table_ID());
                //DataSet ds = ExecuteQuery.ExecuteDataset(sql, param);
                DataSet ds = DB.ExecuteDataset(sql, param, Get_Trx());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    list.Add(new MColumn(GetCtx(), dr, Get_TrxName()));
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
        // getColumns()


        /**
     * 	Get Table from Cache
     *	@param ctx context
     *	@param AD_Table_ID id
     *	@return MTable
     */
        public static MTable Get(Ctx ctx, int AD_Table_ID)
        {
            int key = AD_Table_ID;
            MTable retValue = null;
            s_cache.TryGetValue(key, out retValue);
            if (retValue != null)
                return retValue;
            retValue = new MTable(ctx, AD_Table_ID, null);
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
            var tableId = MTable.Get_Table_ID(tableName);
            MTable tbl = new MTable(ctx, tableId, trxName);
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
        public static MTable Get(Ctx ctx, string tableName)
        {
            //Uesd in translationTable Class 
            if (tableName == null)
                return null;
            //	Check cache
            //iterator<MTable> it = s_cache.Values().iterator();
            IEnumerator<MTable> it = s_cache.Values.GetEnumerator();
            it.Reset();
            while (it.MoveNext())
            //foreach(MTable mtable in it)
            {
                //MTable retValue = (MTable)it.next();
                MTable retValue1 = (MTable)it.Current;
                //if (tableName.equalsIgnoreCase(retValue.getTableName()))
                if (tableName.ToLower().Equals(retValue1.GetTableName().ToLower()))
                    return retValue1;
            }
            //	Get direct
            MTable retValue = null;
            String sql = "SELECT * FROM AD_Table WHERE UPPER(TableName)=UPPER('" + tableName + "')";
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = ds.Tables[0].Rows[i];
                    retValue = new MTable(ctx, rs, null);
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
                int key = retValue.GetAD_Table_ID();
                s_cache.Add(key, retValue);
            }
            return retValue;
        }

        /// <summary>
        /// Get Table Name
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Table_ID">table id</param>
        /// <returns>table name</returns>
        public static String GetTableName(Ctx ctx, int AD_Table_ID)
        {
            return MTable.Get(ctx, AD_Table_ID).GetTableName();
        }

        public bool Haskey(int AD_Table_ID)
        {
            if (s_cacheHasKey.Count == 0)
            {
                LoadHasKeyData();
            }
            if (s_cacheHasKey.ContainsKey(AD_Table_ID))
            {
                return s_cacheHasKey[AD_Table_ID];
            }
            return false;
        }

        private void LoadHasKeyData()
        {
            string sql = "SELECT t.AD_Table_ID , (SELECT COUNT(AD_Column_ID) FROM AD_Column WHERE UPPER(ColumnName) = UPPER(t.TableName)||'_ID' AND AD_Table_ID=t.AD_Table_ID AND IsActive='Y' ) as hasKey FROM AD_Table t";
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
        public PO GetPO(Ctx ctx, DataRow rs, Trx trxName)
        {
            String tableName = GetTableName();
            Type clazz = GetClass(tableName);
            if (clazz == null)
            {
                //log.log(Level.SEVERE, "(rs) - Class not found for " + tableName);
                //ErrorLog.FillErrorLog("MTable.GetPO", "(rs) - Class not found for " + tableName, "", VAdvantage.Framework.Message.MessageType.ERROR);
                //return null;

                //Updateby--Raghu
                //to run work flow with virtual M_ or X_ classes
                log.Log(Level.INFO, "Using GenericPO for " + tableName);
                GenericPO po = new GenericPO(tableName, ctx, rs, trxName);
                return po;
            }
            bool errorLogged = false;
            try
            {
                ConstructorInfo constructor = clazz.GetConstructor(new Type[] { typeof(Ctx), typeof(DataRow), typeof(Trx) });
                PO po = (PO)constructor.Invoke(new object[] { ctx, rs, trxName });
                return po;
            }
            catch (Exception e)
            {
                ////ErrorLog.FillErrorLog("MTable.GetPO", "(rs) - Table=" + tableName + ",Class=" + clazz, e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
                log.Log(Level.SEVERE, "(rs) - Table=" + tableName + ",Class=" + clazz, e);
                errorLogged = true;
                log.SaveError("Error", "Table=" + tableName + ",Class=" + clazz);
            }
            if (!errorLogged)
            {
                ////ErrorLog.FillErrorLog("Mtable", "(rs) - Not found - Table=" + tableName, "", VAdvantage.Framework.Message.MessageType.INFORMATION);
                log.Log(Level.SEVERE, "(rs) - Not found - Table=" + tableName);
            }
            return null;
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


        public PO GetPO(Ctx ctx, int Record_ID, Trx trxName, bool isNew)
        {
            //return GetPO(ctx, Record_ID, trxName, true);
            string tableName = GetTableName();
            if (Record_ID != 0 && !IsSingleKey())
            {
                log.Log(Level.WARNING, "(id) - Multi-Key " + tableName);
                return null;

                //Updateby--Raghu
                //to run work flow with virtual M_ or X_ classes
                //log.Log(Level.INFO, "Using GenericPO for " + tableName);
                //GenericPO po = new GenericPO(tableName, ctx, Record_ID, trxName);
                //return po;
            }

            Type className = GetClass(tableName);

            if (className == null)
            {
                //log.log(Level.WARNING, "(id) - Class not found for " + tableName);
                //to run work flow with virtual M_ or X_ classes
                log.Log(Level.INFO, "Using GenericPO for " + tableName);
                GenericPO po = new GenericPO(tableName, ctx, Record_ID, trxName);
                return po;
            }
            bool errorLogged = false;
            try
            {
                ConstructorInfo constructor = null;
                try
                {
                    constructor = className.GetConstructor(new Type[] { typeof(Ctx), typeof(int), typeof(Trx) });
                }
                catch (Exception e)
                {
                    log.Warning("No transaction Constructor for " + className.FullName + " (" + e.ToString() + ")");
                }

                if (constructor != null)
                {
                    PO po = (PO)constructor.Invoke(new object[] { ctx, Record_ID, trxName });
                    //	Load record 0 - valid for System/Client/Org/Role/User
                    if (!isNew && Record_ID == 0)
                        po.Load(trxName);
                    //	Check if loaded correctly
                    if (po != null && po.Get_ID() != Record_ID && IsSingleKey())
                    {
                        // Common.//ErrorLog.FillErrorLog("MTable", "", po.Get_TableName() + "_ID=" + po.Get_ID() + " <> requested=" + Record_ID, VAdvantage.Framework.Message.MessageType.INFORMATION);
                        return null;
                    }
                    return po;
                }
                else
                {
                    throw new Exception("No Std Constructor");
                }
            }
            catch (Exception ex1)
            {
                log.Severe(ex1.ToString());
                //exception handling
            }
            if (!errorLogged)
            {
                //log.log(Level.SEVERE, "(id) - Not found - Table=" + tableName
                //    + ", Record_ID=" + Record_ID);
            }
            return null;
        }


        /**
	 * 	String Representation
	 *	@return info
	 */
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("MTable[");
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
                MColumn col = m_columns[i];
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
                MColumn column = m_columns[i];
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
        /// Get Persistency Class for table
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public Type GetClass(string tableName)
        {
            //	Not supported
            if (tableName == null || tableName.EndsWith("_Trl"))
                return null;

            //	Import Tables (Name conflict)
            if (tableName.StartsWith("I_"))
            {
                Type className = GetPOclass("VAdvantage.Process.X_" + tableName);
                if (className != null)
                {
                    return className;
                }
                log.Warning("No class for table: " + tableName);
                return null;
            }

            //Special Naming
            for (int i = 0; i < _special.Length; i++)
            {
                if (_special[i++].Equals(tableName))
                {
                    Type clazzsn = GetPOclass(_special[i]);
                    if (clazzsn != null)
                        return clazzsn;
                    break;
                }
            }

            //	Strip table name prefix (e.g. AD_) Customizations are 3/4
            String classNm = tableName;
            int index = classNm.IndexOf('_');
            if (index > 0)
            {
                if (index < 3)		//	AD_, A_
                    classNm = classNm.Substring(index + 1);
                else
                {
                    String prefix = classNm.Substring(0, index);
                    if (prefix.Equals("Fact"))		//	keep custom prefix
                        classNm = classNm.Substring(index + 1);
                }
            }
            //	Remove underlines
            classNm = classNm.Replace("_", "");

            //	Search packages
            //String[] packages = getPackages(GetCtx());
            //for (int i = 0; i < packages.length; i++)
            //{

            string namspace = "";

            /*********** Module Section  **************/

            Tuple<String, String, String> moduleInfo;
            Assembly asm = null;



            //////Check MClasses through list
            for (int i = 0; i < _projectClasses.Length; i++)
            {
                namspace = _projectClasses[i] + classNm;
                if (_projectClasses.Contains("X_"))
                {
                    namspace = _projectClasses[i] + tableName;
                }

                Type clazzsn = GetFromCustomizationPOclass(namspace);
                if (clazzsn != null)
                    return clazzsn;


            }



            //Modules
            if (Env.HasModulePrefix(tableName, out  moduleInfo))
            {
                asm = GetAssembly(moduleInfo.Item1);
                if (asm != null)
                {
                    for (int i = 0; i < _moduleClasses.Length; i++)
                    {
                        namspace = moduleInfo.Item2 + _moduleClasses[i] + classNm;
                        if (_moduleClasses.Contains("X_"))
                        {
                            namspace = moduleInfo.Item2 + _moduleClasses[i] + tableName;
                        }

                        Type clazzsn = GetClassFromAsembly(asm, namspace);
                        if (clazzsn != null)
                            return clazzsn;
                    }
                }
            }


            /********  End  **************/


            for (int i = 0; i < _productClasses.Length; i++)
            {
                namspace = _productClasses[i] + classNm;
                if (_productClasses.Contains("X_"))
                {
                    namspace = _productClasses[i] + tableName;
                }

                Type clazzsn = GetPOclass(namspace);
                if (clazzsn != null)
                    return clazzsn;
            }

            return null;
        }


        /// <summary>
        /// Get Assembly 
        /// </summary>
        /// <param name="AssemblyInfo"> Assembly name And Version</param>
        /// <returns></returns>
        private Assembly GetAssembly(string AssemblyInfo)
        {
            Assembly asm = null;
            try
            {
                asm = Assembly.Load(AssemblyInfo);
            }
            catch (Exception e)
            {
                log.Info(e.Message);
                asm = null;
            }
            return asm;
        }

        /// <summary>
        /// Get Class From Assembly
        /// </summary>
        /// <param name="asm">Assembly</param>
        /// <param name="className">Fully Qulified Class Name</param>
        /// <returns>Class Object</returns>
        private Type GetClassFromAsembly(Assembly asm, string className)
        {
            Type type = null;
            try
            {
                type = asm.GetType(className);
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, e.Message);
            }

            if (type == null)
            {
                return null;
            }

            Type baseClass = type.BaseType;

            while (baseClass != null)
            {
                if (baseClass == typeof(PO))
                {
                    return type;
                }
                baseClass = baseClass.BaseType;
            }
            return null;
        }




        private Type GetFromCustomizationPOclass(string className)
        {

            try
            {
                Assembly asm = null;
                Type type = null;
                try
                {
                    asm = Assembly.Load(GlobalVariable.PRODUCT_NAME);
                    type = asm.GetType(className);
                }
                catch
                {
                    // asm = Assembly.Load(GlobalVariable.ASSEMBLY_NAME);
                }

                /*EndCustomization*/

                if (type == null)
                {
                    type = Type.GetType(className);
                }

                Type baseClass = type.BaseType;

                while (baseClass != null)
                {
                    if (baseClass == typeof(PO))
                    {
                        return type;
                    }
                    baseClass = baseClass.BaseType;
                }
            }
            catch
            {
                log.Finest("Not found: " + className);
            }

            return null;

        }

        /// <summary>
        ///  Get PO class
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        private Type GetPOclass(string className)
        {
            try
            {

                Type classObject = Type.GetType(className);

                Type baseClass = classObject.BaseType;

                while (baseClass != null)
                {
                    if (baseClass == typeof(PO))
                    {
                        return classObject;
                    }
                    baseClass = baseClass.BaseType;
                }
            }
            catch
            {
                log.Finest("Not found: " + className);
            }

            return null;
        }	//	getPOclass


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
            return true;
        }



        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">newRecord new</param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            //	Sync Table ID
            if (newRecord)
                MSequence.CreateTableSequence(GetCtx(), GetTableName(), Get_TrxName());
            else
            {
                MSequence seq = MSequence.Get(GetCtx(), GetTableName(), Get_TrxName());
                if (seq == null || seq.Get_ID() == 0)
                    MSequence.CreateTableSequence(GetCtx(), GetTableName(), Get_TrxName());
                else if (!seq.GetName().Equals(GetTableName()))
                {
                    seq.SetName(GetTableName());
                    seq.Save();
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
                MSequence.DeleteTableSequence(GetCtx(), GetTableName(), Get_TrxName());
            return success;
        }	//	afterDelete
        /**********************END**********************************/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public MColumn GetColumn(String columnName)
        {
            if (string.IsNullOrEmpty(columnName))
                return null;
            GetColumns(false);
            //
            foreach (MColumn element in m_columns)
            {
                if (columnName.Equals(element.GetColumnName(), StringComparison.OrdinalIgnoreCase))
                    return element;
            }
            return null;
        }	//	getColumn


        public MColumn GetIdentifierCol()
        {
            GetColumns(false);

            foreach (MColumn element in m_columns)
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

        public static List<MTable> GetTablesByQuery(Ctx ctx, String sql)
        {
            IDataReader idr = null;
            List<MTable> list = new List<MTable>();
            try
            {
                idr = DataBase.DB.ExecuteReader(sql);
                while (idr.Read())
                {
                    MTable table = new MTable(ctx, idr, null);
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
            String sql = "SELECT * FROM AD_ViewComponent WHERE AD_Table_ID = " + this.GetAD_Table_ID() + " AND IsActive = 'Y'";
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_Trx());
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
