/********************************************************
 * Module Name    :     General DB
 * Purpose        :     Database function
 * Author         :     Jagmohan Bhatt
 * Date           :     8-dec-2008
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Model;
using System.Data.SqlClient;
using System.Data;
using VAdvantage.Common;
using VAdvantage.Utility;
//using System.Data.OracleClient;
using Oracle.ManagedDataAccess.Client;
using MySql.Data.MySqlClient;
using VAdvantage.Logging;
using Npgsql;
//using VAdvantage.Install;
using System.Data.Common;
using System.Drawing;
using System.IO;
using java.io;
using java.util.zip;

namespace VAdvantage.DataBase
{
#pragma warning disable 612, 618
    public class DB
    {
        /** Quote			*/
        private static char QUOTE = '\'';

        /** Connection Descriptor           */
        private static VConnection s_cc = null;
        /**	Logger							*/

        private static VAdvantage.Logging.VLogger log = VAdvantage.Logging.VLogger.GetVLogger(typeof(DB).FullName);

        /** SQL Statement Separator "; "	*/
        public static String SQLSTATEMENT_SEPARATOR = "; ";

        public static String P_VIENNASYS = "ViennaSys";	//	Save system records

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
            if (string.IsNullOrEmpty(txt))
                return "NULL";

            //  Length
            String text = txt;
            if (maxLength != 0 && text.Length > maxLength)
                text = txt.Substring(0, maxLength);

            //  copy characters		(we need to look through anyway)
            StringBuilder outt = new StringBuilder();
            outt.Append(QUOTE);		//	'
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == QUOTE)
                    outt.Append("''");
                else
                    outt.Append(c);
            }
            outt.Append(QUOTE);		//	'
            //
            return outt.ToString();
        }	//	TO_STRING

        public static String TO_DATE(DateTime? day)
        {
            return TO_DATE(day, true);
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
            if (columnName == null || columnName.Length == 0)
                throw new ArgumentException("Required parameter missing");
            return s_cc.GetDatabase().TO_CHAR(columnName, displayType, AD_Language);
        }   //  TO_CHAR

        /// <summary>
        ///Create SQL TO Date String from Timestamp


        /// </summary>
        /// <param name="time">Date to be converted</param>
        /// <param name="dayOnly">dayOnly true if time set to 00:00:00</param>
        /// <returns> TO_DATE('2001-01-30 18:10:20',''YYYY-MM-DD HH24:MI:SS')
        ///     or  TO_DATE('2001-01-30',''YYYY-MM-DD')</returns>
        public static String TO_DATE(DateTime? time, bool dayOnly)
        {
            if (!dayOnly)
            {
                time = GlobalVariable.SetDateTimeUTC(time);
            }

            return s_cc.GetDatabase().TO_DATE(time, dayOnly);
        }





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



        /// <summary>
        /// Get next number for Key column = 0 is Error.
        /// </summary>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="TableName">table name</param>
        /// <param name="trxName">optional Transaction Name</param>
        /// <returns>next no</returns>
        public static int GetNextID(int AD_Client_ID, String TableName, Trx trxName)
        {
            //if ((trxName == null || trxName.Length() == 0) && isRemoteObjects())
            //{
            //    Server server = CConnection.get().getServer();
            //    try
            //    {
            //        if (server != null)
            //        {	//	See ServerBean
            //            int id = server.getNextID(AD_Client_ID, TableName, null);
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
            int id = MSequence.GetNextID(AD_Client_ID, TableName, trxName);	//	tries 3 times
            //	if (id <= 0)
            //		throw new DBException("No NextID (" + id + ")");
            return id;
        }	//	getNextID

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
            //Trx trx = trxName == null ? null : Trx.Get(trxName, true);
            if (trx != null)
            {
                return trx.ExecuteDataset(sql, param, trx);
            }
            else
            {

                if (DB.IsPostgreSQL())
                {
                    NpgsqlParameter[] pparam = VAdvantage.SqlExec.ExecuteQuery.GetPostgreParameter(param);
                    return VAdvantage.SqlExec.PostgreSql.PostgreHelper.ExecuteDataset(GetConnectionString(), CommandType.Text, ConvertSqlQuery(sql), pageSize, pageNumber, pparam);
                }
                OracleParameter[] oparam = VAdvantage.SqlExec.ExecuteQuery.GetOracleParameter(param);
                return SqlExec.Oracle.OracleHelper.ExecuteDataset(GetConnectionString(), CommandType.Text, ConvertSqlQuery(sql), pageSize, pageNumber, oparam);

            }
        }

        /// <summary>
        /// Get Value from sql
        /// </summary>
        /// <param name="trxName">trx</param>
        /// <param name="sql">sql</param>
        /// <returns>first value or -1</returns>
        public static int GetSQLValue(Trx trxName, String sql)
        {
            int retValue = -1;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                if (idr.Read())
                    retValue = Utility.Util.GetValueOfInt(idr[0].ToString());
                else
                {
                    //log.fine("No Value " + sql);
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
                //////ErrorLog.FillErrorLog("DataBase.DB.GetSQLValue", sql, e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
            }
            return retValue;
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
            int retValue = -1;
            //PreparedStatement pstmt = null;
            string sqlQuery = sql;// +int_param1;
            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@param1", int_param1);
                dr = DataBase.DB.ExecuteReader(sql, param, trxName);
                if (dr.Read())
                {
                    retValue = Utility.Util.GetValueOfInt(dr[0].ToString());
                }
                else
                {
                    log.Config("No Value " + sql + " - Param1=" + int_param1);
                }
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                log.Log(Level.SEVERE, sql + " - Param1=" + int_param1 + " [" + trxName + "]", e);

            }
            return retValue;
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
            int retValue = -1;
            //PreparedStatement pstmt = null;
            string sqlQuery = sql;// +int_param1;
            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@param1", str_param1);
                dr = DataBase.DB.ExecuteReader(sql, param, trxName);
                if (dr.Read())
                {
                    retValue = Utility.Util.GetValueOfInt(dr[0].ToString());
                }
                else
                {
                    log.Config("No Value " + sql + " - Param1=" + str_param1);
                }
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                log.Log(Level.SEVERE, sql + " - Param1=" + str_param1 + " [" + trxName + "]", e);

            }
            return retValue;
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
            int retValue = -1;
            //PreparedStatement pstmt = null;
            string sqlQuery = sql;
            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@param1", param1);
                param[1] = new SqlParameter("@param2", param2);

                dr = DataBase.DB.ExecuteReader(sql, param, trxName);
                if (dr.Read())
                {
                    retValue = Utility.Util.GetValueOfInt(dr[0].ToString());
                }
                else
                {
                    log.Config("No Value " + sql + " - Param1=" + param1);
                }
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                log.Log(Level.SEVERE, sql + " - Param1=" + param1 + " [" + trxName + "]", e);

            }
            return retValue;
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
            //if (s_cc != null)
            //    return s_cc.GetDatabase().GetCachedConnection(true, 0);

            if (DatabaseType.IsOracle)
                return new OracleConnection(DB.GetConnectionString());
            else if (DatabaseType.IsPostgre)
                return new NpgsqlConnection(DB.GetConnectionString());
            else if (DatabaseType.IsMSSql)
                return new SqlConnection(DB.GetConnectionString());
            else if (DatabaseType.IsMySql)
                return new MySqlConnection(DB.GetConnectionString());

            return null;
        }





        /// <summary>
        /// Get Database

        /// </summary>
        /// <returns></returns>
        public static ViennaDatabase GetDatabase()
        {
            if (s_cc != null)
                return s_cc.GetDatabase();
            log.Severe("No Database Connection");
            return null;
        }   //  ge


        public static int ExecuteUpdateMultiple(String sql, bool ignoreError, Trx trx)
        {
            int no = 0;
            try
            {
                if ((sql == null) || (sql.Length == 0))
                    throw new ArgumentException("Required parameter missing - " + sql);
                int index = sql.IndexOf(SQLSTATEMENT_SEPARATOR);
                if (index == -1)
                    return ExecuteQuery(sql, null, trx);

                //
                String[] statements = sql.Split(';');
                foreach (String element in statements)
                {
                    //log.fine(element);
                    no += ExecuteQuery(element, null, trx);
                }

                return no;
            }
            catch (Exception ex)
            {
                log.Log(VAdvantage.Logging.Level.SEVERE, ex.Message);
            }
            return no;
        }	//	executeUpdareMultiple


        /// <summary>
        /// Gets the schema of the current database
        /// </summary>
        /// <returns></returns> 
        public static string GetSchema()
        {
            if (DatabaseType.IsOracle)
                return VConnection.Get().Db_uid.ToUpper();//  Ini.GetProperty("dbuserid").ToUpper(); //VConnection.Get().Db_uid;//  
            else
                return VConnection.Get().Db_name;
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
            //IDbConnection _cc = GetConnection();
            if (cc == null)
            {
                throw new ArgumentException("Connection is NULL");
            }

            if (s_cc != null && s_cc.Equals(cc))
            {
                return;
            }

            DataBase.DB.CloseTarget();
            //
            if (s_cc == null)
            {
                s_cc = cc;
            }

            //lock (cc)    //  use as mutex
            //{
            //    s_cc = cc;

            //}

            log.Config(s_cc + " - DS=" + s_cc.Name);
        }

        /// <summary>
        /// Get Array of Key Name Pairs
        /// </summary>
        /// <param name="sql">select with id / name as first / second column</param>
        /// <param name="optional">if true (-1,"") is added </param>
        /// <returns>array of key name pairs</returns>
        public static KeyNamePair[] GetKeyNamePairs(String sql, bool optional)
        {
            DataTable dt = null;
            IDataReader idr = null;
            List<KeyNamePair> list = new List<KeyNamePair>();
            if (optional)
            {
                list.Add(new KeyNamePair(-1, ""));
            }
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)// while (rs.next())
                {
                    list.Add(new KeyNamePair(Utility.Util.GetValueOfInt(dr[0]), dr[1].ToString()));//.getString(2)));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            KeyNamePair[] retValue = new KeyNamePair[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Do we have an Oracle DB ?
        /// </summary>
        /// <returns>true if connected to Oracle</returns>
        public static Boolean IsOracle()
        {
            return DatabaseType.IsOracle;
        }

        /// <summary>
        /// Do we have a PostgreSQL DB ?
        /// </summary>
        /// <returns>true if connected to PostgreSQL</returns>
        public static Boolean IsPostgreSQL()
        {
            return DatabaseType.IsPostgre;

        }
        /// <summary>
        ///Get a string representation of literal used in SQL clause
        /// </summary>
        /// <param name="sqlClause">sqlClause "S", "U", "I", "W"</param>
        /// <param name="dataType">sql Types</param>
        /// <returns>NULL or db2: nullif(x,x)</returns>

        public static String NULL(String sqlClause, String dataType)
        {
            if (DataBase.DB.IsConnected())
            {
                //return s_cc.getDatabase().nullValue(sqlClause, dataType);
                //return  DataBase.DB.GetConnection().Database .getDatabase().nullValue(sqlClause, dataType);
                return "NULL";
            }

            return "NULL";
            //return null;
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
            try
            {
                //Connection conn = null;
                IDbConnection conn = null;
                Trx trx = trxName;
                if (trx != null)
                {
                    conn = trx.GetConnection();
                    trx.Commit();
                }
                else
                {
                    //conn = DataBase.DB.GetConnection();
                }
                //	if (!conn.getAutoCommit())
                //conn.commit();
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "[" + trxName + "]", e);
                if (throwException)
                {
                    throw e;
                }
                return false;
            }
            return true;
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
            string trxName = "";
            try
            {
                //Connection conn = null;
                //IDbConnection conn = null;
                //Trx trx = trxName == null ? null : Trx.Get(trxName, true);

                if (trx != null)
                {
                    trxName = trx.GetTrxName();
                    //conn = trx.GetConnection();
                    trx.Rollback();
                }
                else
                {
                    //conn = DataBase.DB.GetConnection();
                }
                //	if (!conn.getAutoCommit())

            }
            catch (Exception e)
            {

                if (trx != null)

                    log.Log(Level.SEVERE, "[" + trxName + "]", e);
                if (throwException)
                {
                    throw e;
                }
                return false;
            }
            return true;
        }	//	commit


        public static int ExecuteQuery(String sql, SqlParameter[] param, Trx trx, bool ignoreError, bool throwError = false)
        {
            string trxName = "";
            try
            {
                //Trx trx = trxName == null ? null : Trx.Get(trxName, true);
                if (trx != null)
                {
                    trxName = trx.GetTrxName();
                    return trx.ExecuteNonQuery(sql, param, trx);
                }
                else
                {
                    return SqlExec.ExecuteQuery.ExecuteNonQuery(sql, param);
                }
            }
            catch (System.Data.Common.DbException ex)
            {
                if (ignoreError)
                {
                    log.Log(Level.WARNING, trxName, ex.Message);
                }
                else
                {
                    log.Log(Level.SEVERE, trxName, ex);
                    log.SaveError("DBExecuteError", ex);
                }
                if (throwError)
                {
                    throw new Exception(ex.Message);
                }
                return -1;
            }
            catch (Exception ex)
            {
                log.Severe(ex.ToString());
                //log.SaveError(ex.ToString());
                return -1;
            }
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
            if (trx != null)
            {
                return trx.ExecuteProcedure(sql, arrParam, trx);
            }
            else
            {
                return SqlExec.ExecuteQuery.ExecuteProcedure(sql, arrParam);
            }
        }

        //modified by Deepak
        public static Decimal? GetSQLValueBD(Trx trxName, String sql, int int_param1)
        {
            Decimal? retValue = null;
            SqlParameter[] param = new SqlParameter[1];
            IDataReader idr = null;
            try
            {
                //pstmt = prepareStatement(sql, trxName);
                //pstmt.setInt(1, int_param1);
                param[0] = new SqlParameter("@param1", int_param1);
                idr = DataBase.DB.ExecuteReader(sql, param, trxName);
                if (idr.Read())
                {
                    retValue = Utility.Util.GetValueOfDecimal(idr[0]);// rs.getBigDecimal(1);
                }
                else
                    log.Info("No Value " + sql + " - Param1=" + int_param1);
                idr.Close();
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql + " - Param1=" + int_param1 + " [" + trxName + "]", e);
            }

            return retValue;
        }



        public static bool UseMigratedConnection
        {
            get;
            set;
        }


        //by Karan
        public static string MigrationConnection
        {
            get;
            set;
        }


        //Manfacturing
        public static int ExecuteBulkUpdate(Trx trx, String key, List<List<Object>> bulkParams)
        {
            List<Object[]> bulkParamAsArray = new List<Object[]>(bulkParams.Count);
            foreach (List<Object> param in bulkParams)
            {
                bulkParamAsArray.Add(param.ToArray());
            }
            return ExecuteBulkUpdate(trx, key, bulkParamAsArray, false, true);
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
            //long time = System.currentTimeMillis();
            Trx trx = trx1;

            DateTime time = DateTime.Now.Date;
            if ((sql == null) || (sql.Length == 0))
            {
                throw new ArgumentException("Required parameter missing - " + sql);
            }

            //CPreparedStatement pstmt = new CPreparedStatement(ResultSet.TYPE_FORWARD_ONLY,
            //ResultSet.CONCUR_UPDATABLE, sql, trx);	//	converted in call

            int total = 0;
            int count = 0;
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[bulkParams.Count];
                foreach (Object[] param in bulkParams)
                {
                    count++;
                    // Set Parameter
                    if (param != null)
                    {
                        for (int i = 0; i < param.Length; i++)
                        {
                            //setParam(pstmt, param[i], i + 1);
                            sqlParam[i] = new SqlParameter("@param" + i, param[i]);
                        }
                    }
                    if (bulkSQL)
                    {
                        //This method is used to add a set of parameter to the batch of command 
                        //to this sql server prepared object.
                        // pstmt.addBatch();
                        if (count % batchSize == 0)
                        {
                            ////executeBatch()--This method is used to submit a set of command in sql query 
                            ////to the database, In case all the commands successfully, return you an 
                            ////array update count.
                            //int[] updateCounts = pstmt.executeBatch();
                            //foreach (int updateCount in updateCounts)
                            //{
                            //    if (updateCount >= 0) total += updateCount;
                            //    if (updateCount == Statement.SUCCESS_NO_INFO)
                            //    { total++; }
                            //}
                        }
                    }
                    else
                    {
                        // int no = ExecuteQuery(sql, sqlParam, trx, false);

                        //if (DB.isMSSQLServer() && (no == -1))
                        //{
                        //    no = 0; //
                        //}
                        // total += no;
                    }
                }

                if (bulkSQL)
                {
                    //int[] updateCounts = pstmt.executeBatch();
                    //foreach (int updateCount in updateCounts)
                    //{
                    //    if (updateCount >= 0) total += updateCount;
                    //    if (updateCount == Statement.SUCCESS_NO_INFO) total++;
                    //}
                }
                int no = ExecuteQuery(sql, sqlParam, trx, false);
                total += no;
                // No Transaction - Commit
                if (trx == null)
                {
                    trx.Commit(); // Local commit
                }
            }
            catch (Exception e)
            {
                log.Severe("ExecuteBulkUpdate =>" + e.Message);
            }
            return total;
        }

        /// <summary>
        /// Get Document No from table
        /// </summary>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="TableName">table name</param>
        /// <param name="trx">optional Transaction Name</param>
        /// <returns>document no or null</returns>
        public static String GetDocumentNo(int AD_Client_ID, String TableName, Trx trx, Ctx ctx)
        {
            //ServerBean s;
            //TcpChannel tcp = new TcpChannel();
            //IChannel[] chans = ChannelServices.RegisteredChannels;
            //if (chans.Count() <= 0)
            //    ChannelServices.RegisterChannel(tcp, true);
            //try
            //{
            //    Ctx ctx = Env.GetCtx();
            //    IDictionary<string, string> dic = ctx.GetMap();
            //    s = (ServerBean)Activator.GetObject(typeof(ServerBean), "tcp://" + VConnection.Get().Apps_host + ":" + VConnection.Get().Apps_port + "/" + "ViennaFramework");
            //    string sttr = "";
            //    string error = s.TryConnection(out sttr);
            //    if ((trx == null) && error == "OK")
            //    {
            //        error = s.GetDocumentNo(AD_Client_ID, TableName, null);
            //        log.Finest("Server => " + error);
            //        if (error != null)
            //        {
            //            return error;
            //        }
            //        log.Log(Level.SEVERE, "AppsServer not found - " + TableName);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    //ShowMessage.Error(ex.ToString(), true);
            //    log.SaveError("AppsServerNotFound", ex);
            //}

            String dn = MSequence.GetDocumentNo(AD_Client_ID, TableName, trx, ctx);
            if (dn == null)		//	try again
                dn = MSequence.GetDocumentNo(AD_Client_ID, TableName, trx, ctx);
            if (dn == null)
                throw new Exception("No DocumentNo");
            return dn;
        }	//	getDocumentNo

        /// <summary>
        /// Get Document No based on Document Type get doc numbr from application server
        /// </summary>
        /// <param name="C_DocType_ID">document type</param>
        /// <param name="trx">optional Transaction Name</param>
        /// <returns>document no or null</returns>
        /// <date>08-March-2011</date>


        public static String GetDocumentNo(int C_DocType_ID, Trx trx, Ctx ctx)
        {
            //ServerBean s;
            //TcpChannel tcp = new TcpChannel();
            //IChannel[] chans = ChannelServices.RegisteredChannels;
            //if (chans.Count() <= 0)
            //    ChannelServices.RegisterChannel(tcp, true);
            //try
            //{
            //    Ctx ctx = Env.GetCtx();
            //    IDictionary<string, string> dic = ctx.GetMap();
            //    s = (ServerBean)Activator.GetObject(typeof(ServerBean), "tcp://" + VConnection.Get().Apps_host + ":" + VConnection.Get().Apps_port + "/" + "ViennaFramework");
            //    string sttr = "";
            //    string error = s.TryConnection(out sttr);
            //    if ((trx == null) && error == "OK")
            //    {
            //        error = s.GetDocumentNo(C_DocType_ID, null);
            //        log.Finest("Server => " + error);
            //        if (error != null)
            //        {
            //            return error;
            //        }
            //        log.Log(Level.SEVERE, "AppsServer not found - " + C_DocType_ID);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    //ShowMessage.Error(ex.ToString(), true);
            //    log.SaveError("AppsServerNotFound", ex);
            //}

            String dn = MSequence.GetDocumentNo(C_DocType_ID, trx, ctx);
            if (dn == null)		//	try again
            {
                dn = MSequence.GetDocumentNo(C_DocType_ID, trx, ctx);
            }
            return dn;
        }


        private static Boolean _isLoggingUpdates;
        //private static Dictionary<String, UpdateStats> _updateStats = new Dictionary<string, UpdateStats>();

        /// <summary>
        /// Update Log
        /// </summary>
        /// <Date>14-March-2011</Date>
        /// <Writer>Raghu</Writer>
        public static void StartLoggingUpdates()
        {
            _isLoggingUpdates = true;
            if (_isLoggingUpdates)
            {
            }
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
            //DB._isLoggingUpdates = false;
            //StringBuilder s = new StringBuilder();

            //List<DB.UpdateStats> stats = new List<DB.UpdateStats>(DB._updateStats.Values);

            //collections.Sort(stats, new Comparator<DB.UpdateStats>()
            //{

            //    public int Compare(UpdateStats o1, UpdateStats o2)
            //    {
            //        if( o1.timeSpent != o2.timeSpent )
            //            return o1.timeSpent.compareTo(o2.timeSpent);
            //        if( o1.numRecords != o2.numRecords )
            //            return o1.numRecords.compareTo(o2.numRecords);
            //        if( o1.numExecutions != o2.numExecutions )
            //            return o1.numExecutions.compareTo(o2.numExecutions);
            //        return o1.sql.compareTo(o2.sql);
            //    }
            //});

            //Formatter f = new Formatter(s);
            //for (DB.UpdateStats stat : stats) {
            //    if(stat.numRecords > numRecordsThreshold){
            //        f.format("%8d", stat.timeSpent);
            //        s.append(" ms spent in ");
            //        f.format("%6d", stat.numExecutions);
            //        s.append(" updates of ");
            //        f.format("%8.1f", ((float) stat.numRecords) / stat.numExecutions);
            //        s.append(" records each for " + stat.sql);
            //        s.append("\n");
            //    }
            //}

            //DB._updateStats.clear();
            //return s.toString();
            return "";
        }

        /** Main Version String         */
        static public String MAIN_VERSION = "Release 3.0.0";
        /** Detail Version as date      Used for Client/Server		*/
        static public String DATE_VERSION = "2007-11-26";
        /** Database Version as date    Compared with AD_System		*/
        static public String DB_VERSION = "2007-11-26";

        /** Product Name            */
        static public String NAME = "ViennaFramework\u00AE";
        /** URL of Product          */
        static public String URL = "www.viennasolutions.com";

        /** Copyright Notice - Don't modify this - Program will someday fail unexpectedly
         *  it also violates the license and you'll be held liable for any damage claims */
        static public String COPYRIGHT = "\u00A9 1999-2009 Vienna \u00AE";

        //static private Image s_image16;
        static private Image s_image48x15;
        //static private Image s_imageLogo;

        /** 16*16 Product Image.
        /** Removing/modifying the Vienna logo is a violation of the license	*/
        //static private String s_File16x16 = "Images/C16.png";
        /** 32*32 Product Image.
        /** Removing/modifying the Vienna logo is a violation of the license	*/
        // static private String s_file32x32 = "Images/C32.png";
        /** 100*30 Product Image.
        /** Removing/modifying the Vienna logo is a violation of the license	*/
        //static private String s_fileMedium = "Images/Vienna120x30.png";
        /** 48*15 Product Image.
        /** Removing/modifying the Vienna logo is a violation of the license	*/
        //static private String s_fileSmall = "Images/vienna.png";
        /** Removing/modifying the Vienna logo is a violation of the license	*/
        // static private String s_fileHR = "Images\\vienna.png";
        /** Removing/modifying the Vienna logo is a violation of the license	*/
        // static private String s_fileHRJPG = "Images\\vienna.jpg";

        public static Image GetImageLogoSmall(bool hr)
        {
            if (s_image48x15 == null)
            {
                s_image48x15 = ModelLibrary.Properties.Resources.vienna;
            }

            return s_image48x15;

        }



        public static bool SaveZipEntries(List<KeyValuePair<String, byte[]>> list, string tableName, int Id)
        {

            ByteArrayOutputStream bOut = new ByteArrayOutputStream();
            // initialize zip
            ZipOutputStream zip = new ZipOutputStream(bOut);
            zip.setMethod(ZipOutputStream.DEFLATED);
            zip.setLevel(Deflater.BEST_COMPRESSION);

            try
            {
                int isize = list.Count;
                for (int i = 0; i < isize; i++)
                {
                    // get item
                    KeyValuePair<String, byte[]> item = list[i];
                    // make zip entry
                    ZipEntry entry = new ZipEntry(item.Key);
                    // set time
                    entry.setTime(long.Parse(System.DateTime.Now.Millisecond.ToString()));
                    entry.setMethod(ZipEntry.DEFLATED);
                    // start setting zip entry into zip file
                    zip.putNextEntry(entry);
                    byte[] data = item.Value;
                    object obj = (object)data;
                    // set data into zip
                    zip.write((byte[])obj, 0, data.Length);
                    // close zip entry
                    zip.closeEntry();
                }
                // close zip
                zip.close();

                byte[] sObjData = bOut.toByteArray();
                byte[] zipData = ConvertTobyte(sObjData);

                // Set binary data in PO and return
                //    SetBinaryData(zipData);


                System.Data.SqlClient.SqlParameter[] param = new System.Data.SqlClient.SqlParameter[1];

                param[0] = new System.Data.SqlClient.SqlParameter("@param1", zipData);

                // string sql = "Update AD_Attachment Set BinaryData = @param1 Where AD_Attachment_ID = " + AD_Attachment_ID;

                string sql = "Update " + tableName + " Set BinaryData = @param1 Where " + tableName + "_ID = " + Id;

                return VAdvantage.DataBase.DB.ExecuteQuery(sql, param, null) != -1;
            }

            catch
            {
                return false;
            }




        }

        public static List<KeyValuePair<string, byte[]>> GetZipEntries(byte[] sdata)
        {

            List<KeyValuePair<string, byte[]>> list = new List<KeyValuePair<string, byte[]>>();




            ByteArrayInputStream inBt = new ByteArrayInputStream(sdata);
            // initialize zip
            ZipInputStream zip = new ZipInputStream(inBt);
            // get next entry i.e. 1st entry in zip
            ZipEntry entry = zip.getNextEntry();
            // for every entry in zip
            while (entry != null)
            {
                // get file name
                string name = entry.getName();
                ByteArrayOutputStream outBt = new ByteArrayOutputStream();
                byte[] buffer = new byte[2048];
                int length = zip.read(buffer);
                while (length != -1)
                {
                    // get data
                    outBt.write(buffer, 0, length);
                    length = zip.read(buffer);
                }
                //
                byte[] sdataEntry = outBt.toByteArray();
                byte[] dataEntry = ConvertTobyte(sdataEntry);




                // add the entry into _items list

                KeyValuePair<string, byte[]> keyVal = new KeyValuePair<string, byte[]>(name, dataEntry);

                list.Add(keyVal);
                //info.AttachmentEntries.Add(new AttachmentEntry(name, dataEntry, info.AttachmentEntries.Count + 1));
                // get next entry in zip
                entry = zip.getNextEntry();
            }
            return list;

        }




        public static byte[] ConvertTobyte(byte[] byteVar)
        {
            if (byteVar != null)
            {
                //int len = byteVar.Length;
                //byte[] byteData = new byte[len];
                //for (int i = 0; i < len; i++)
                //{
                //    byteData[i] = (byte)byteVar[i];
                //}
                byte[] byteData = new byte[byteVar.Length];
                System.Buffer.BlockCopy(byteVar, 0, byteData, 0, byteVar.Length);

                return byteData;
            }
            else
            {
                return null;
            }
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

        /// <summary>
        /// Get Connection String 
        /// </summary>
        /// <returns></returns>
        internal static string GetConnectionString()
        {
            return DBConn.CreateConnectionString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static string ConvertSqlQuery(string sql)
        {
            if (s_cc != null)
                return s_cc.GetDatabase().ConvertStatement(sql);
            return sql;
        }
    }

#pragma warning restore 612, 618
}
