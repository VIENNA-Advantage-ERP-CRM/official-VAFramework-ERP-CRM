/********************************************************
 * Module Name    :     General DB
 * Purpose        :     Database function
 * Author         :     Jagmohan Bhatt
 * Date           :     8-dec-2008
  ******************************************************/
using System;
using System.Collections.Generic;

using VAdvantage.Model;
using System.Data.SqlClient;
using System.Data;

using VAdvantage.Utility;

using BaseLibrary.Engine;

namespace VAdvantage.DataBase
{
#pragma warning disable 612, 618
    public class DB
    {


        /// <summary>
        ///Get next number for Key column = 0 is Error.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="TableName"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        public static int GetNextID(Ctx ctx, String TableName, Trx trx)
        {
            if (ctx == null)
                throw new ArgumentException("Context missing");
            if ((TableName == null) || (TableName.Length == 0))
                throw new ArgumentException("TableName missing");
            return GetNextID(ctx.GetAD_Client_ID(), TableName, trx);
        }	//	getNextID

        public static int GetNextID(int VAF_Client_ID, String TableName, Trx trxName)
        {
            //if ((trxName == null || trxName.Length() == 0) && isRemoteObjects())
            //{
            //    Server server = CConnection.get().getServer();
            //    try
            //    {
            //        if (server != null)
            //        {	//	See ServerBean
            //            int id = server.getNextID(VAF_Client_ID, TableName, null);
            //            log.finest("server => " + id);
            //            if (id < 0)
            //                throw new DBException("No NextID");
            //            return id;
            //        }
            //        log.log(Level.SEVERE, "AppsServer not found - " + TableName);
            //    }
            //    catch (RemoteException ex)
            //    {
            //        log.log(Level.SEVERE, "AppsServer error", ex);
            //    }
            //    //	Try locally
            //}
            ////jz let trxName = null so that the previous workflow node wouldn't block startnext  to invoke this method
            //   in the case trxName rollback, it's OK to keep the pumpup sequence.
            //TODO
            //if (isDB2())
            //    trxName = null;	//	tries 3 times
            int id = POActionEngine.Get().GetNextID(VAF_Client_ID, TableName, trxName);	//	tries 3 times
            //	if (id <= 0)
            //		throw new DBException("No NextID (" + id + ")");
            return id;
        }   //	getNextID

        /// <summary>
        /// Execute SQL Query
        /// </summary>
        /// <param name="sql">sql query to be executed</param>
        /// <param name="param">parameters to be passed to the query</param>
        /// <param name="trxName">optional transaction name</param>
        /// <returns>return number of rows affected. -1 if error occured</returns>
        public static int ExecuteQuery(string sql, SqlParameter[] param, Trx trx)
        {
            return ExecuteQuery(sql, param, trx, false);
            //try
            //{

            //    Trx trx = trxName == null ? null : Trx.Get(trxName, true);
            //    if (trx != null)
            //    {
            //        return trx.ExecuteNonQuery(sql, param, trxName);
            //    }
            //    else
            //    {
            //        return SqlExec.ExecuteQuery.ExecuteNonQuery(sql, param);
            //    }
            //}
            //catch
            //{
            //    return -1;
            //}
        }

        /// <summary>
        /// Execute SQL Query
        /// </summary>
        /// <param name="sql">sql query to be executed</param>
        /// <param name="trxName">optional transaction name</param>
        /// <returns>return number of rows affected. -1 if error occured</returns>
        public static int ExecuteQuery(string sql, SqlParameter[] param)
        {
            return ExecuteQuery(sql, param, null);
        }


        /// <summary>
        /// Execute SQL Query
        /// </summary>
        /// <param name="sql">sql query to be executed</param>
        /// <returns></returns>
        public static int ExecuteQuery(string sql)
        {
            return ExecuteQuery(sql, null, null);
        }

        /// <summary>
        /// Execute SQL Query
        /// </summary>
        /// <param name="sql">sql query to be executed</param>
        /// <param name="trxName">optional transaction name</param>
        /// <returns>return number of rows affected. -1 if error occured</returns>
        //public static int ExecuteQuery(string sql, Trx trx)
        //{
        //    return ExecuteQuery(sql, null, trxName);
        //}


        /// <summary>
        /// Execute SQL Query
        /// </summary>
        /// <param name="sql">sql query to be executed</param>
        /// <param name="param">parameters to be passed to the query</param>
        /// <param name="trxName">optional transaction name</param>
        /// <returns>return number of rows affected. -1 if error occured</returns>
        public static object ExecuteScalar(string sql, SqlParameter[] param, Trx trx)
        {
            //Trx trx = trxName == null ? null : Trx.Get(trxName, true);//
            if (trx != null)
            {
                return trx.ExecuteScalar(sql, param, trx);
            }
            else
            {
                return SqlExec.ExecuteQuery.ExecuteScalar(sql, param);
            }
        }

        /// <summary>
        /// Execute SQL Query
        /// </summary>
        /// <param name="sql">sql query to be executed</param>
        /// <param name="trxName">optional transaction name</param>
        /// <returns>return number of rows affected. -1 if error occured</returns>
        public static object ExecuteScalar(string sql)
        {
            return ExecuteScalar(sql, null, null);
        }

        /// <summary>
        /// Execute SQL Query
        /// </summary>
        /// <param name="sql">sql query to be executed</param>
        /// <param name="trxName">optional transaction name</param>
        /// <returns>return number of rows affected. -1 if error occured</returns>
        //public static object ExecuteScalar(string sql, string trxName)
        //{
        //    return ExecuteScalar(sql, null, trxName);
        //}

        ///// <summary>
        ///// ExecuteReader without parameter
        ///// </summary>
        ///// <param name="sql"></param>
        ///// <returns></returns>
        //public static System.Data.IDataReader ExecuteReader(string sql)
        //{
        //    return SqlExec.ExecuteQuery.ExecuteReader(sql, null);
        //}

        ///// <summary>
        ///// ExecuteReader with parameter
        ///// </summary>
        ///// <param name="sql"></param>
        ///// <param name="param"></param>
        ///// <returns></returns>
        //public static System.Data.IDataReader ExecuteReader(string sql, SqlParameter[] param)
        //{
        //    return SqlExec.ExecuteQuery.ExecuteReader(sql, param);
        //}


        /// <summary>
        /// Executes the query and fills the dataset
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns>dataset</returns>
        public static IDataReader ExecuteReader(string sql)
        {
            return ExecuteReader(sql, null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static IDataReader ExecuteReader(string sql, SqlParameter[] param)
        {
            return ExecuteReader(sql, param, null);
        }

        /// <summary>
        /// Executes the query and fills the dataset
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="param"></param>
        /// <returns>dataset</returns>
        /// [ob
        /// 
        //[Obsolete]
        //public static IDataReader ExecuteReader(string sql, SqlParameter[] param, string trxName,bool notUse = true)
        //{
        //    Trx trx = trxName == null ? null : Trx.Get(trxName, true);
        //    if (trx != null)
        //    {
        //        return trx.ExecuteReader(sql, param, trxName);
        //    }
        //    else
        //    {
        //        return SqlExec.ExecuteQuery.ExecuteReader(sql, param);
        //    }
        //}

        public static IDataReader ExecuteReader(string sql, SqlParameter[] param, Trx trx)
        {
            // Trx trx = trxName == null ? null : Trx.Get(trxName, true);
            if (trx != null)
            {
                return trx.ExecuteReader(sql, param, trx);
            }
            else
            {
                return SqlExec.ExecuteQuery.ExecuteReader(sql, param);
            }
        }


        //ends here

        /// <summary>
        /// Executes the query and fills the dataset
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns>dataset</returns>
        //public static DataSet ExecuteDataset(string sql)
        //{
        //    return SqlExec.ExecuteQuery.ExecuteDataset(sql, null);
        //}

        /// <summary>
        /// Executes the query and fills the dataset
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="param"></param>
        /// <returns>dataset</returns>
        //public static DataSet ExecuteDataset(string sql, SqlParameter[] param)
        //{
        //    return SqlExec.ExecuteQuery.ExecuteDataset(sql, param);
        //}


        /// <summary>
        /// Executes the query and fills the dataset
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns>dataset</returns>
        public static DataSet ExecuteDataset(string sql)
        {
            return ExecuteDataset(sql, null, (Trx)null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        //public static DataSet ExecuteDataset(string sql, string trx)
        //{
        //    return ExecuteDataset(sql, null, trx);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataset(string sql, SqlParameter[] param)
        {
            return ExecuteDataset(sql, param, (Trx)null);
        }

        /// <summary>
        /// Executes the query and fills the dataset
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="param"></param>
        /// <returns>dataset</returns>
        /// [
        /// 
        //[Obsolete]
        //public static DataSet ExecuteDataset(string sql, SqlParameter[] param, string trxName,bool dotUse = true)
        //{
        //    Trx trx = trxName == null ? null : Trx.Get(trxName, true);
        //    if (trx != null)
        //    {
        //        return trx.ExecuteDataset(sql, param, trxName);
        //    }
        //    else
        //    {
        //        return SqlExec.ExecuteQuery.ExecuteDataset(sql, param);
        //    }
        //}

        /// <summary>
        /// Get Connection String 
        /// </summary>
        /// <returns></returns>
        public static string GetConnectionString()
        {
            return DBConn.CreateConnectionString();
        }

        public static DataSet ExecuteDataset(string sql, SqlParameter[] param, Trx trx)
        {
            //Trx trx = trxName == null ? null : Trx.Get(trxName, true);
            if (trx != null)
            {
                return trx.ExecuteDataset(sql, param, trx);
            }
            else
            {
                return SqlExec.ExecuteQuery.ExecuteDataset(sql, param);
            }
        }


        public static DataSet ExecuteDatasetDoc(string sql, SqlParameter[] param, Trx trx)
        {
            //Trx trx = trxName == null ? null : Trx.Get(trxName, true);
            if (trx != null)
            {
                return trx.ExecuteDataset(sql, param, trx);
            }
            else
            {
                return SqlExec.ExecuteQuery.ExecuteDataset(sql, param);
            }
        }

        public static DataSet ExecuteDataset(string sql, SqlParameter[] param, Trx trx, int pageSize, int pageNumber)
        {
            return CoreLibrary.DataBase.DB.ExecuteDataset(sql, param, trx, pageSize, pageNumber);
        }

        /// <summary>
        /// Get Value from sql
        /// </summary>
        /// <param name="trxName">trx</param>
        /// <param name="sql">sql</param>
        /// <returns>first value or -1</returns>
        public static int GetSQLValue(Trx trxName, String sql)
        {

            return CoreLibrary.DataBase.DB.GetSQLValue(trxName, sql);

        }


        /// <summary>
        /// Get Value from sql
        /// </summary>
        /// <param name="trxName">trx</param>
        /// <param name="sql">sql</param>
        /// <param name="int_param1">parameter 1</param>
        /// <returns>value or -1</returns>
        public static int GetSQLValue(Trx trxName, String sql, int int_param1)
        {
            return CoreLibrary.DataBase.DB.GetSQLValue(trxName, sql, int_param1);
        }


        /// <summary>
        /// Get Value from sql
        /// </summary>
        /// <param name="trxName">trx</param>
        /// <param name="sql">sql</param>
        /// <param name="int_param1">parameter 1</param>
        /// <returns>value or -1</returns>
        public static int GetSQLValue(Trx trxName, String sql, string str_param1)
        {
            return CoreLibrary.DataBase.DB.GetSQLValue(trxName, sql, str_param1);
        }
        /// <summary>
        ///Get Value from sql
        /// </summary>
        /// <param name="trxName">trx</param>
        /// <param name="sql">sql</param>
        /// <param name="param1">parameter 1</param>
        /// <param name="param2"></param>
        /// <returns>value or -1</returns>
        public static int GetSQLValue(Trx trxName, String sql, int param1, String param2)
        {
            return CoreLibrary.DataBase.DB.GetSQLValue(trxName, sql, param1, param2);
        }

        /// <summary>
        /// Checks if database is available to be connected
        /// </summary>
        /// <returns></returns>
        public static bool IsConnected()
        {
            //Exception ex = VConnection.Get().Connect();
            //if (ex == null) return true;
            // return false;
            return true;
            //VConnection vconn = VConnection.Get();
            //IDbConnection conn = Ini.GetConnection(vconn);
            //try
            //{
            //    conn.Open();
            //    return true;
            //}
            //catch
            //{
            //    return false;
            //}
            //finally
            //{
            //    if (conn != null)
            //    {
            //        conn.Close();
            //    }
            //}

        }





        /// <summary>
        /// Gets the connection
        /// </summary>
        /// <returns>connection object</returns>
        public static IDbConnection GetConnection()
        {

            return CoreLibrary.DataBase.DB.GetConnection();

        }

        /// <summary>
        /// Get Database

        /// </summary>
        /// <returns></returns>
        public static ViennaDatabase GetDatabase()
        {
            return CoreLibrary.DataBase.DB.GetDatabase();

        }   //  ge


        public static int ExecuteUpdateMultiple(String sql, bool ignoreError, Trx trx)
        {
            return CoreLibrary.DataBase.DB.ExecuteUpdateMultiple(sql, ignoreError, trx);

        }	//	executeUpdareMultiple


        /// <summary>
        /// Gets the schema of the current database
        /// </summary>
        /// <returns></returns> 
        public static string GetSchema()
        {
            return CoreLibrary.DataBase.DB.GetSchema();

        }

        /// <summary>
        /// Close Target
        /// </summary>
        public static void CloseTarget()
        {

            //s_cc.SetDataSource(null)
            // s_cc = null;
        }

        /// <summary>
        ///  Set connection
        /// </summary>
        /// <param name="cc">connection</param>
        public static void SetDBTarget(VConnection cc)//setDBTarget (CConnection cc)
        {

            CoreLibrary.DataBase.DB.SetDBTarget(cc);

        }

        /// <summary>
        /// Get Array of Key Name Pairs
        /// </summary>
        /// <param name="sql">select with id / name as first / second column</param>
        /// <param name="optional">if true (-1,"") is added </param>
        /// <returns>array of key name pairs</returns>
        public static KeyNamePair[] GetKeyNamePairs(String sql, bool optional)
        {
            return CoreLibrary.DataBase.DB.GetKeyNamePairs(sql, optional);
        }

        /// <summary>
        /// Do we have an Oracle DB ?
        /// </summary>
        /// <returns>true if connected to Oracle</returns>
        public static Boolean IsOracle()
        {
            return CoreLibrary.DataBase.DB.IsOracle();
        }

        /// <summary>
        /// Do we have a PostgreSQL DB ?
        /// </summary>
        /// <returns>true if connected to PostgreSQL</returns>
        public static Boolean IsPostgreSQL()
        {
            return CoreLibrary.DataBase.DB.IsPostgreSQL();

        }


        /// <summary>
        ///Commit - commit on RW connection.
        //Is not required as RW connection is AutoCommit (exception: with transaction)
        /// </summary>
        /// <param name="throwException">if true, re-throws exception</param>
        /// <param name="trxName">trx name</param>
        /// <returns>true if not needed or success</returns>
        public static bool Commit(bool throwException, Trx trxName)
        {
            return CoreLibrary.DataBase.DB.Commit(throwException, trxName);

        }	//	commit


        /// <summary>
        ///	Rollback - rollback on RW connection.
        //Is has no effect as RW connection is AutoCommit (exception: with transaction)
        /// </summary>
        /// <param name="throwException">if true, re-throws exception</param>
        /// <param name="trxName">transaction name</param>
        /// <returns> true if not needed or success</returns>
        public static bool Rollback(bool throwException, Trx trx)
        {
            return CoreLibrary.DataBase.DB.Rollback(throwException, trx);

        }	//	commit


        public static int ExecuteQuery(String sql, SqlParameter[] param, Trx trx, bool ignoreError, bool throwError = false)
        {
            return CoreLibrary.DataBase.DB.ExecuteQuery(sql, param, trx, ignoreError, throwError);

        }

        /// <summary>
        /// Execute Procedure
        /// </summary>
        /// <param name="sql">Procedure Name</param>
        /// <param name="arrParam">Sql Parameters</param>
        /// <param name="trx">Transaction</param>
        /// <returns>Sql Parameters containing result</returns>
        public static SqlParameter[] ExecuteProcedure(string sql, SqlParameter[] arrParam, Trx trx)
        {
            return CoreLibrary.DataBase.DB.ExecuteProcedure(sql, arrParam, trx);
        }

        //modified by Deepak
        public static Decimal? GetSQLValueBD(Trx trxName, String sql, int int_param1)
        {
            return CoreLibrary.DataBase.DB.GetSQLValueBD(trxName, sql, int_param1);
        }



        public static bool UseMigratedConnection
        {
            get
            {
                return CoreLibrary.DataBase.DB.UseMigratedConnection;
            }
            set
            {
                CoreLibrary.DataBase.DB.UseMigratedConnection = value;
            }
        }


        //by Karan
        public static string MigrationConnection
        {
            get
            {
                return CoreLibrary.DataBase.DB.MigrationConnection;
            }
            set
            {
                CoreLibrary.DataBase.DB.MigrationConnection = value;
            }
        }

        public static String TO_DATE(DateTime? day)
        {
            return CoreLibrary.DataBase.DB.TO_DATE(day, true);
        }

        public static String TO_DATE(DateTime? time, bool dayOnly)
        {
            return CoreLibrary.DataBase.DB.TO_DATE(time, dayOnly);
        }

        //Manfacturing
        public static int ExecuteBulkUpdate(Trx trx, String key, List<List<Object>> bulkParams)
        {
            return CoreLibrary.DataBase.DB.ExecuteBulkUpdate(trx, key, bulkParams);

        }

        private static int batchSize = 5;// Util.GetValueOfInt(Ini.GetProperty(Ini._BATCH_SIZE));
        /// <summary>
        /// Execute Update.
        /// saves "DBExecuteError" in Log
        /// </summary>
        /// <param name="trx">optional transaction name</param>
        /// <param name="sql">sql</param>
        /// <param name="bulkParams"></param>
        /// <param name="ignoreError">if true, no execution error is reported</param>
        /// <param name="bulkSQL"></param>
        /// <returns>number of rows updated or -1 if error</returns>
        public static int ExecuteBulkUpdate(Trx trx1, String sql, List<Object[]> bulkParams, Boolean ignoreError, Boolean bulkSQL)
        {
            return CoreLibrary.DataBase.DB.ExecuteBulkUpdate(trx1, sql, bulkParams, ignoreError, bulkSQL);

        }

        /// <summary>
        /// Get Document No from table
        /// </summary>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="TableName">table name</param>
        /// <param name="trx">optional Transaction Name</param>
        ///// <returns>document no or null</returns>
        //public static String GetDocumentNo(int AD_Client_ID, String TableName, Trx trx, Ctx ctx)
        //{


        //    String dn = MSequence.GetDocumentNo(AD_Client_ID, TableName, trx, ctx);
        //    if (dn == null)		//	try again
        //        dn = MSequence.GetDocumentNo(AD_Client_ID, TableName, trx, ctx);
        //    if (dn == null)
        //        throw new Exception("No DocumentNo");
        //    return dn;
        //}	//	getDocumentNo

        ///// <summary>
        ///// Get Document No based on Document Type get doc numbr from application server
        ///// </summary>
        ///// <param name="C_DocType_ID">document type</param>
        ///// <param name="trx">optional Transaction Name</param>
        ///// <returns>document no or null</returns>
        ///// <date>08-March-2011</date>


        //public static String GetDocumentNo(int C_DocType_ID, Trx trx, Ctx ctx)
        //{


        //    String dn = MSequence.GetDocumentNo(C_DocType_ID, trx, ctx);
        //    if (dn == null)		//	try again
        //    {
        //        dn = MSequence.GetDocumentNo(C_DocType_ID, trx, ctx);
        //    }
        //    return dn;
        //}



        //private static Dictionary<String, UpdateStats> _updateStats = new Dictionary<string, UpdateStats>();

        /// <summary>
        /// Update Log
        /// </summary>
        /// <Date>14-March-2011</Date>
        /// <Writer>Raghu</Writer>
        public static void StartLoggingUpdates()
        {
            CoreLibrary.DataBase.DB.StartLoggingUpdates();
        }

        private static class UpdateStats
        {
            //static String sql;
            //static int numExecutions = 0;
            //static int numRecords = 0;
            //static long timeSpent = 0L;
        }


        /// <summary>
        /// numExecutionsThreshold
        /// only show the SQLs that have been executed for greater than this
        /// number of records
        /// </summary>
        /// <param name="numRecordsThreshold"></param>
        /// <returns>formatted log output</returns>
        public static String StopLoggingUpdates(int numRecordsThreshold)
        {
            return CoreLibrary.DataBase.DB.StopLoggingUpdates(numRecordsThreshold);

        }

     

        
        public static string ConvertSqlQuery(string sql)
        {
            return CoreLibrary.DataBase.DB.ConvertSqlQuery(sql);
        }


        public static String TO_STRING(String txt)
        {
            return TO_STRING(txt, 0);
        }   //  TO_STRING

        /**
         *	Package Strings for SQL command in quotes.
         *  <pre>
         *		-	include in ' (single quotes)
         *		-	replace ' with ''
         *  </pre>
         *  @param txt  String with text
         *  @param maxLength    Maximum Length of content or 0 to ignore
         *  @return escaped string for insert statement (NULL if null)
         */
        public static String TO_STRING(String txt, int maxLength)
        {
            return CoreLibrary.DataBase.DB.TO_STRING(txt, maxLength);
        }

        /// <summary>
        /// Create SQL for formatted Date, Number
        /// </summary>
        /// <param name="columnName">the column name in the SQL</param>
        /// <param name="displayType">displayType Display Type</param>
        /// <param name="AD_Language"></param>
        /// <returns>TRIM(TO_CHAR(columnName,'999G999G999G990D00','NLS_NUMERIC_CHARACTERS='',.'''))
        ///     or TRIM(TO_CHAR(columnName,'TM9')) depending on DisplayType and Language</returns>
        public static String TO_CHAR(String columnName, int displayType, String AD_Language)
        {
            return CoreLibrary.DataBase.DB.TO_CHAR(columnName, displayType, AD_Language);
            //if (columnName == null || columnName.Length == 0)
            //    throw new ArgumentException("Required parameter missing");
            //return s_cc.GetDatabase().TO_CHAR(columnName, displayType, AD_Language);
        }   //  TO

        public static String NULL(String sqlClause, String dataType)
        {
            return CoreLibrary.DataBase.DB.NULL(sqlClause, dataType);
        }


        public static DataSet SetUtcDateTime(DataSet dataSet)
        {
            var ds = new DataSet { Locale = System.Globalization.CultureInfo.InvariantCulture };
            foreach (DataTable source in dataSet.Tables)
            {
                bool containsDate = false;
                var target = source.Clone();

                foreach (DataColumn col in target.Columns)
                {
                    if (col.DataType == System.Type.GetType("System.DateTime"))
                    {
                        col.DateTimeMode = DataSetDateTime.Utc;
                        containsDate = true;
                    }
                }

                if (containsDate)
                {
                    foreach (DataRow row in source.Rows)
                        target.ImportRow(row);
                    ds.Tables.Add(target);
                }
                else
                {
                    ds.Tables.Add(source.Copy());
                }
            }
            dataSet.Tables.Clear();
            dataSet.Dispose();
            dataSet = ds;
            return dataSet;
        }

        public static DataTable SetUtcDateTime(DataTable source)
        {
            //var ds = new DataSet { Locale = CultureInfo.InvariantCulture };

            var target = source.Clone();

            //foreach (DataTable source in dataSet.Tables)
            //{
            bool containsDate = false;
            //    var target = source.Clone();

            foreach (DataColumn col in target.Columns)
            {
                if (col.DataType == System.Type.GetType("System.DateTime"))
                {
                    col.DateTimeMode = DataSetDateTime.Utc;
                    containsDate = true;
                }
            }

            if (containsDate)
            {
                foreach (DataRow row in source.Rows)
                    target.ImportRow(row);
                return target;
                //ds.Tables.Add(target);
            }
            //else
            //{
            //    ds.Tables.Add(source.Copy());
            //}
            //}

            return source;
        }
    }
#pragma warning restore 612, 618
}