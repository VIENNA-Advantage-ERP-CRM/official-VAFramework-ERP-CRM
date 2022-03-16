/********************************************************
 * Module Name    :     General (Connection)
 * Purpose        :     Maintains the connection transaction
 * Author         :     Jagmohan Bhatt
 * Date           :     27-Apr-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using VAdvantage.Classes;
using System.Data;
//using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Data.Common;
using VAdvantage.DataBase;
using Npgsql;
using MySql.Data.MySqlClient;
using VAdvantage.Logging;
using Oracle.ManagedDataAccess.Client;
using CoreLibrary.DataBase;
using CoreLibrary.Classes;
//using VAdvantage.Install;

namespace VAdvantage.DataBase
{
#pragma warning disable 612, 618
    public class Trx
    {
        IDbTransaction _trx = null;
        IDbConnection _conn = null;

        private static readonly object _trxLock = new object();
        private static readonly object _trxCloseLock = new object();

        /** Logger					*/
        private VLogger log = null;
        bool useSameTrxForDocNo = true;
        /// <summary>
        /// if false then new transaction will be created
        /// Used in MSequence.GetDocumentNo.
        /// </summary>
        public bool UseSameTrxForDocNo
        {
            get
            {
                return useSameTrxForDocNo;
            }
            set
            {
                useSameTrxForDocNo = value;
            }
        }


        public int ExecuteQuery(string sql, SqlParameter[] arrparam)
        {
            if (!IsActive())
                Start();
            //using (OracleConnection cn = new OracleConnection(connection))
            //{
            Logging.VLogger.Get().Info("Trx non-query " + GetTrxName() + " " + System.Threading.Thread.CurrentThread.ManagedThreadId);
            int retval = 0;
            if (DatabaseType.IsOracle)
            {
                OracleParameter[] param = GetOracleParameter(arrparam);
                OracleCommand cmd1 = new OracleCommand();
                PrepareCommandForOracle(cmd1, (OracleConnection)_conn, (OracleTransaction)_trx, CommandType.Text, sql, param);


                try
                {
                    retval = cmd1.ExecuteNonQuery();
                    if (retval == -1)
                        retval = 1;
                    cmd1.Parameters.Clear();
                }
                catch (Exception e)
                {
                    cmd1.Parameters.Clear();
                    retval = -1;
                    VAdvantage.Logging.VLogger.Get().Severe(e.Message + " [Query NQTrx]" + sql);
                    throw e;
                }
            }
            else if (DatabaseType.IsPostgre)
            {
                NpgsqlParameter[] param = GetPostgreParameter(arrparam);
                NpgsqlCommand cmd1 = new NpgsqlCommand();
                PrepareCommandForPostgre(cmd1, (NpgsqlConnection)_conn, (NpgsqlTransaction)_trx, CommandType.Text, sql, param);


                try
                {
                    retval = cmd1.ExecuteNonQuery();
                    if (retval == -1)
                        retval = 1;
                    cmd1.Parameters.Clear();
                }
                catch (Exception e)
                {
                    cmd1.Parameters.Clear();
                    retval = -1;
                    VAdvantage.Logging.VLogger.Get().Severe(e.Message + " [Query NQTrx]" + sql);
                    throw e;
                }
            }

            return retval;

            //}
        }


        public DataSet ExecuteDataSet(string sql, SqlParameter[] arrparam)
        {
            if (!IsActive())
                Start();

            Logging.VLogger.Get().Info("Trx Dataset " + GetTrxName() + " " + System.Threading.Thread.CurrentThread.ManagedThreadId);
            DataSet ds = new DataSet();
            if (DatabaseType.IsOracle)
            {

                OracleParameter[] param = GetOracleParameter(arrparam);
                OracleCommand cmd = new OracleCommand();
                PrepareCommandForOracle(cmd, (OracleConnection)_conn, (OracleTransaction)_trx, CommandType.Text, sql, param);

                //create the DataAdapter & DataSet
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                //fill the DataSet using default values for DataTable names, etc.
                try
                {
                    da.Fill(ds);
                    cmd.Parameters.Clear();
                }
                catch (Exception ex)
                {
                    Logging.VLogger.Get().Severe(ex.Message + " [Query D] =>" + sql);
                    cmd.Parameters.Clear();
                    return null;
                }
            }
            else if (DatabaseType.IsPostgre)
            {
                NpgsqlParameter[] param = GetPostgreParameter(arrparam);
                NpgsqlCommand cmd = new NpgsqlCommand();
                PrepareCommandForPostgre(cmd, (NpgsqlConnection)_conn, (NpgsqlTransaction)_trx, CommandType.Text, sql, param);

                //create the DataAdapter & DataSet
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                //fill the DataSet using default values for DataTable names, etc.
                try
                {
                    da.Fill(ds);
                    cmd.Parameters.Clear();
                }
                catch (Exception ex)
                {
                    Logging.VLogger.Get().Severe(ex.Message + " [Query D] =>" + sql);
                    cmd.Parameters.Clear();
                    return null;
                }
            }
            return ds;
        }

        private void PrepareCommandForOracle(OracleCommand command, OracleConnection connection, OracleTransaction transaction, CommandType commandType, string commandText, OracleParameter[] commandParameters)
        {
            //if the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            //associate the connection with the command
            command.Connection = connection;

            //set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            //if we were provided a transaction, assign it.
            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            //set the command type
            command.CommandType = commandType;

            //attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParametersForOracle(command, commandParameters);
            }


            return;
        }

        private void PrepareCommandForPostgre(NpgsqlCommand command, NpgsqlConnection connection, NpgsqlTransaction transaction, CommandType commandType, string commandText, NpgsqlParameter[] commandParameters)
        {
            //if the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            //associate the connection with the command
            command.Connection = connection;

            //set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            //if we were provided a transaction, assign it.
            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            //set the command type
            command.CommandType = commandType;

            //attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParametersForPOstgre(command, commandParameters);
            }


            return;
        }

        private void AttachParametersForPOstgre(NpgsqlCommand command, NpgsqlParameter[] commandParameters)
        {

            foreach (NpgsqlParameter p in commandParameters)
            {
                //p.ParameterName = p.ParameterName.Replace("@", ":");
                p.ParameterName = p.ParameterName.Replace("@", "");
                //check for derived output value with no value assigned
                if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                {
                    p.Value = DBNull.Value;
                }
                command.Parameters.Add(p);
            }
        }

        private void AttachParametersForOracle(OracleCommand command, OracleParameter[] commandParameters)
        {
            foreach (OracleParameter p in commandParameters)
            {
                p.ParameterName = p.ParameterName.Replace("@", "");
                //check for derived output value with no value assigned
                if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                {
                    p.Value = DBNull.Value;
                }
                command.Parameters.Add(p);
            }
        }

        public OracleParameter[] GetOracleParameter(SqlParameter[] arrParam)
        {
            if (arrParam == null)
                return null;
            //create and instance of OracleParameter and initialize the length with the length of arrParam
            OracleParameter[] param = new OracleParameter[arrParam.Length];
            //loop through all the values of arrParam
            for (int i = 0; i <= arrParam.Length - 1; i++)
            {
                //set one by one all the values to the OracleParameter
                //replace @ with ? for use in Oracle
                //string str = arrParam[i].SqlDbType.ToString();
                //string strVal = to_date(arrParam[i].Value.ToString(), "mm/dd/yyyy");
                param[i] = new OracleParameter(arrParam[i].ParameterName, arrParam[i].Value);                
            }
            return param;   //return the parameter
        }


        /// <summary>
        /// Creates the NpgsqlParameter class from passed in SQLParameter
        /// </summary>
        /// <param name="arrParam">Array of SQLParameter</param>
        /// <returns>NpgsqlParameter</returns>
        public NpgsqlParameter[] GetPostgreParameter(SqlParameter[] arrParam)
        {
            if (arrParam == null)
                return null;

            //create and instance of NpgsqlParameter and initialize the length with the length of arrParam
            NpgsqlParameter[] param = new NpgsqlParameter[arrParam.Length];
            for (int i = 0; i <= arrParam.Length - 1; i++)
            {
                //set one by one all the values to the NpgsqlParameter
                //replace @ with ? for use in Postgre SQL
                // param[i] = new NpgsqlParameter(arrParam[i].ParameterName, String.IsNullOrEmpty(arrParam[i].Value.ToString()) ? "-1" : arrParam[i].Value);
                param[i] = new NpgsqlParameter(arrParam[i].ParameterName, arrParam[i].Value);                
            }
            return param;   //return the parameter
        }

        /// <summary>
        /// Get OracleParameters Class from passed SqlParameters
        /// </summary>
        /// <param name="arrParam">Array of SQLParameter</param>
        /// <returns>OracleParameters</returns>
        public OracleParameter[] GetOracleProcedureParameter(SqlParameter[] arrParam)
        {
            if (arrParam == null)
                return null;
            //create and instance of OracleParameter and initialize the length with the length of arrParam
            OracleParameter[] param = new OracleParameter[arrParam.Length];
            //loop through all the values of arrParam
            for (int i = 0; i <= arrParam.Length - 1; i++)
            {
                //set one by one all the values to the OracleParameter
                //replace @ with ? for use in Oracle
                //string str = arrParam[i].SqlDbType.ToString();
                //string strVal = to_date(arrParam[i].Value.ToString(), "mm/dd/yyyy");
                param[i] = new OracleParameter(arrParam[i].ParameterName, arrParam[i].Value);
                param[i].DbType = arrParam[i].DbType;

                if ((arrParam[i].Direction == ParameterDirection.InputOutput) && (arrParam[i].Value == null))
                {
                    param[i].Value = DBNull.Value;
                }

                if (arrParam[i].Direction == ParameterDirection.Output)
                {
                    param[i].Direction = arrParam[i].Direction;
                }
            }
            return param;   //return the parameter
        }


        /// <summary>
        /// Creates the NpgsqlParameter class from passed in SQLParameter
        /// </summary>
        /// <param name="arrParam">Array of SQLParameter</param>
        /// <returns>NpgsqlParameter</returns>
        public NpgsqlParameter[] GetPostgreProcedureParameter(SqlParameter[] arrParam)
        {
            if (arrParam == null)
                return null;

            //create and instance of NpgsqlParameter and initialize the length with the length of arrParam
            NpgsqlParameter[] param = new NpgsqlParameter[arrParam.Length];
            for (int i = 0; i <= arrParam.Length - 1; i++)
            {
                //set one by one all the values to the NpgsqlParameter
                //replace @ with ? for use in Postgre SQL
                // param[i] = new NpgsqlParameter(arrParam[i].ParameterName, String.IsNullOrEmpty(arrParam[i].Value.ToString()) ? "-1" : arrParam[i].Value);
                param[i] = new NpgsqlParameter(arrParam[i].ParameterName, arrParam[i].Value);
                param[i].DbType = arrParam[i].DbType;

                if ((arrParam[i].Direction == ParameterDirection.InputOutput) && (arrParam[i].Value == null))
                {
                    param[i].Value = DBNull.Value;
                }

                if (arrParam[i].Direction == ParameterDirection.Output)
                {
                    param[i].Direction = arrParam[i].Direction;
                }
            }
            return param;   //return the parameter
        }

        /**	Transaction Cache					*/
        private static Dictionary<String, Trx> _cache = null;	//	create change listener

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trxName"></param>
        private Trx(String trxName)
            : this(trxName, null)
        {
            if (log == null)
                log = VLogger.GetVLogger(this.GetType().FullName);
        }	//	Trx

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trxName"></param>
        /// <param name="con"></param>
        private Trx(String trxName, IDbConnection con)
        {
            //	log.info (trxName);

            if (log == null)
                log = VLogger.GetVLogger(this.GetType().FullName);
            SetTrxName(trxName);
            SetConnection(DB.GetConnection());

        }	//	Trx


        /// <summary>
        /// Execute SQL Query
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql)
        {
            return ExecuteNonQuery(sql, null, null);
        }

        /// <summary>
        /// Execute SQL Query
        /// </summary>
        /// <param name="sql">SQL Query to be executed</param>
        /// <param name="trxName">Transaction name</param>
        /// <returns>Number of rows affected</returns>
        public int ExecuteNonQuery(string sql, Trx trxName)
        {
            if (trxName == null)    //if trxName is null execute the query as it is
                return ExecuteNonQuery(sql, null, null);
            else
                return ExecuteNonQuery(sql, null, trxName);
        }

        /// <summary>
        /// Execute SQL Query
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, SqlParameter[] param)
        {
            return ExecuteNonQuery(sql, param, null);
        }

        /// <summary>
        /// Execute SQL Query
        /// </summary>
        /// <param name="sql">sql query to be executed</param>
        /// <param name="param">parameters to be passed to the query</param>
        /// <param name="trxName">optional transaction name</param>
        /// <returns>return number of rows affected. -1 if error occured</returns>
        public int ExecuteNonQuery(string sql, SqlParameter[] arrparam, Trx trx)
        {
            if ((trx == null))
                return SqlExec.ExecuteQuery.ExecuteNonQuery(sql, arrparam);


            sql = DB.ConvertSqlQuery(sql);
            if (!IsActive())
                Start();
            //log.Config("Executing query : " + sql);

            //Trx trx = trxName == null ? null : Trx.Get(trxName, true);

            if (DatabaseType.IsOracle)
            {
                OracleParameter[] oracleParam = SqlExec.ExecuteQuery.GetOracleParameter(arrparam);
                int val = Execute(_conn, CommandType.Text, sql, oracleParam);   //finally execute the query
                //log.Config("Query Result: " + val);
                return val;
            }
            else if (DatabaseType.IsPostgre)
            {
                NpgsqlParameter[] postgreParam = SqlExec.ExecuteQuery.GetPostgreParameter(arrparam);
                return Execute(_conn, CommandType.Text, sql, postgreParam);   //finally execute the query
            }
            else if (DatabaseType.IsMSSql)
            {
                return Execute(_conn, CommandType.Text, sql, arrparam);   //finally execute the query
            }
            else if (DatabaseType.IsMySql)
            {
                MySqlParameter[] mysqlParam = SqlExec.ExecuteQuery.GetMySqlParameter(arrparam);
                return Execute(_conn, CommandType.Text, sql, mysqlParam);   //finally execute the query
            }


            return 0;
        }


        /// <summary>
        /// Transaction is Active
        /// </summary>
        /// <returns>true if transaction active  </returns>
        public bool IsActive()
        {
            return _active;
        }

        ///// <summary>
        ///// Create unique Transaction Name
        ///// </summary>
        ///// <param name="prefix">prefix optional prefix</param>
        ///// <returns>unique name</returns>
        ///// 
        //[Obsolete("Method is deprecated, please use GetTrx Object instead.")]
        //[MethodImpl(MethodImplOptions.Synchronized)]
        public static String CreateTrxName(String prefix)
        {
            lock (_trxLock)
            {
                if (prefix == null || prefix.Length == 0)
                    prefix = "Trx";
                prefix += "_" + CommonFunctions.CurrentTimeMillis();
                return prefix;
            }
        }	//	CreateTrxName

        ///// <summary>
        ///// Create unique Transaction Name
        ///// </summary>
        ///// <returns>unique name</returns>
        ///// 
        //[Obsolete("Method is deprecated, please use GetTrx Object instead.")]
        //public static String CreateTrxName()
        //{
        //    return CreateTrxName(null);
        //}	//	CreateTrxName


        /// <summary>
        /// Get Transaction
        /// </summary>
        /// <param name="trxName">trx name</param>
        /// <returns>Transaction or null</returns>
        /// 
        //[Obsolete("Method is deprecated, please use GetTrx instead.")]
        //[MethodImpl(MethodImplOptions.Synchronized)]
        //public static Trx Get(String trxName)
        //{
        //    return Get(trxName, false);
        //}




        /// <summary>
        /// Get Transaction
        /// </summary>
        /// <param name="trxName">trx name</param>
        /// <param name="createNew">if false, null is returned if not found</param>
        /// <returns>Transaction or null</returns>
        /// 
        [Obsolete("Get is deprecated, please use GetTrx instead.")]
        // [MethodImpl(MethodImplOptions.Synchronized)]
        public static Trx Get(String trxName, bool createNew)
        {
            lock (_trxLock)
            {
                if (trxName == null || trxName.Length == 0)
                    throw new ArgumentException("No Transaction Name");

                if (_cache == null)
                {
                    _cache = new Dictionary<String, Trx>(10);	//	no expiration
                }

                Trx retValue = null;
                if (_cache.ContainsKey(trxName))
                {
                    retValue = _cache[trxName];
                }

                if (retValue == null && createNew)
                {
                    retValue = new Trx(trxName);
                    _cache.Add(trxName, retValue);

                }
                return retValue;
            }
        }	//	Get

        //[MethodImpl(MethodImplOptions.Synchronized)]
        public static Trx GetTrx(String trxName)
        {
            lock (_trxLock)
            {
                return new Trx(trxName, null);
            }
        }

        //  [MethodImpl(MethodImplOptions.Synchronized)]
        public static Trx Get(String trxName)
        {
            lock (_trxLock)
            {
                return new Trx(trxName, null);
            }
        }


        private String _trxName = null;
        private String _trxUniqueName = null; //WF Document Value Type

        private bool _active = false;

        /// <summary>
        /// Set Trx Name
        /// </summary>
        /// <param name="trxName">Transaction Name</param>
        private void SetTrxName(String trxName)
        {
            if (trxName == null || trxName.Length == 0)
                throw new ArgumentException("No Transaction Name");

            _trxName = trxName;
        }	//	setName

        public String SetUniqueTrxName(String trxName)
        {
            if (trxName == null || trxName.Length == 0)
                throw new ArgumentException("No Transaction Name");

            _trxUniqueName = trxName;
            return _trxUniqueName;
        }	//	setName

        /// <summary>
        /// Set Connection
        /// </summary>
        /// <param name="conn">connection</param>
        private void SetConnection(IDbConnection conn)
        {
            if (conn == null)
                return;
            _conn = conn;
            log.Finest("Connection=" + conn.ToString());
        }

        /// <summary>
        /// Get Name
        /// </summary>
        /// <returns>name</returns>
        public String GetTrxName()
        {
            return _trxName;
        }	//	getName

        /// <summary>
        /// Rollback
        /// </summary>
        /// <returns>true, if success</returns>
        public bool Rollback()
        {
            try
            {

                if (_conn != null && _trx != null && _trx.Connection != null)
                {
                    _trx.Rollback();
                    log.Info("**R** " + _trxName);
                    _active = false;

                    if (_trxUniqueName != null)
                        TrxActionNotifier.Get().OnRollBack(_trxUniqueName);
                    //ManageSkippedWF.Remove(_trxUniqueName);
                    return true;
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, _trxName, e);
            }

            _active = false;
            return false;
        }

        /// <summary>
        /// Commit
        /// </summary>
        /// <returns>true, if success</returns>
        public bool Commit()
        {
            try
            {
                if (_conn != null && _trx != null && _trx.Connection != null)
                {
                    _trx.Commit();
                    log.Info("**C** " + _trxName);
                    _active = false;

                    if (_trxUniqueName != null)
                        TrxActionNotifier.Get().OnCommit(_trxUniqueName);
                    //ManageSkippedWF.Execute(_trxUniqueName);
                    return true;
                }
            }
            catch (Exception sqlex)
            {
                log.Log(Level.SEVERE, _trxName, sqlex);
            }

            _active = false;
            return false;


        }

        /// <summary>
        /// Gets the Connection object to the caller
        /// </summary>
        /// <returns>Appropriate Connection Object</returns>
        public IDbConnection GetConnection()
        {
            if (_conn != null)
            {
                //log.Log(Level.ALL, "Active=" + IsActive() + ", Connection=" + _conn.ToString());
                return _conn;
            }

            return DB.GetConnection();

            //return null;
        }

        /// <summary>
        /// Close the connection
        /// </summary>
      //  [MethodImpl(MethodImplOptions.Synchronized)]
        public void Close()
        {
            lock (_trxCloseLock)
            {
                if (_cache != null)
                    _cache.Remove(GetTrxName());

                try
                {
                    if (_conn != null)
                        _conn.Close();
                    if (_trx != null)
                        _trx.Dispose();
                }
                catch (Exception sqlex)
                {
                    log.Log(Level.SEVERE, _trxName, sqlex);
                }
                _conn = null;
                _trx = null;
                _active = false;
                log.Config(_trxName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            _conn = GetConnection();    //Get the appropriate connection object
            if (_active)
            {
                log.Warning("Trx in progress " + _trxName + " - " + GetTrxName());
                return false;
            }

            _active = true;
            try
            {
                if (_trx != null)
                    _trx.Dispose();

                if (_conn != null)
                {
                    if (_conn.State == ConnectionState.Closed)
                    {
                        _conn.Open();
                    }
                    _trx = _conn.BeginTransaction(IsolationLevel.ReadCommitted);
                    log.Info("**** " + GetTrxName());
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, _trxName, e);
                _active = false;
                return false;
            }
            return true;
        }	//	startTrx


        /// <summary>
        /// Executes the SQL Query
        /// </summary>
        /// <param name="connection">current connection</param>
        /// <param name="commandType">command type => default: Text</param>
        /// <param name="commandText">SQL Query to be executed</param>
        /// <param name="commandParameters">Optional Parameter (If any)</param>
        /// <returns>Number of rows affected</returns>
        private int Execute(IDbConnection connection, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            return SqlExec.Oracle.OracleHelper.ExecuteNonQuery((OracleConnection)connection, CommandType.Text, commandText, (OracleTransaction)_trx, commandParameters);
        }

        /// <summary>
        /// Executes the SQL Query
        /// </summary>
        /// <param name="connection">current connection</param>
        /// <param name="commandType">command type => default: Text</param>
        /// <param name="commandText">SQL Query to be executed</param>
        /// <param name="commandParameters">Optional Parameter (If any)</param>
        /// <returns>Number of rows affected</returns>
        private int Execute(IDbConnection connection, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {
            return SqlExec.PostgreSql.PostgreHelper.ExecuteNonQuery((NpgsqlConnection)connection, CommandType.Text, commandText, (NpgsqlTransaction)_trx, commandParameters);
        }

        /// <summary>
        /// Executes the SQL Query
        /// </summary>
        /// <param name="connection">current connection</param>
        /// <param name="commandType">command type => default: Text</param>
        /// <param name="commandText">SQL Query to be executed</param>
        /// <param name="commandParameters">Optional Parameter (If any)</param>
        /// <returns>Number of rows affected</returns>
        private int Execute(IDbConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return SqlExec.MSSql.SqlHelper.ExecuteNonQuery((SqlConnection)connection, CommandType.Text, commandText, (SqlTransaction)_trx, commandParameters);
        }

        /// <summary>
        /// Executes the SQL Query
        /// </summary>
        /// <param name="connection">current connection</param>
        /// <param name="commandType">command type => default: Text</param>
        /// <param name="commandText">SQL Query to be executed</param>
        /// <param name="commandParameters">Optional Parameter (If any)</param>
        /// <returns>Number of rows affected</returns>
        private int Execute(IDbConnection connection, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            return SqlExec.MySql.MySqlHelper.ExecuteNonQuery((MySqlConnection)connection, CommandType.Text, commandText, (MySqlTransaction)_trx, commandParameters);
        }



        #region Execute DR


        /// <summary>
        /// Executes the SQL Query
        /// </summary>
        /// <param name="connection">current connection</param>
        /// <param name="commandType">command type => default: Text</param>
        /// <param name="commandText">SQL Query to be executed</param>
        /// <param name="commandParameters">Optional Parameter (If any)</param>
        /// <returns>Number of rows affected</returns>
        private IDataReader ExecuteDR(IDbConnection connection, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            return SqlExec.Oracle.OracleHelper.ExecuteReader((OracleConnection)connection, CommandType.Text, commandText, (OracleTransaction)_trx, commandParameters);
        }

        /// <summary>
        /// Executes the SQL Query
        /// </summary>
        /// <param name="connection">current connection</param>
        /// <param name="commandType">command type => default: Text</param>
        /// <param name="commandText">SQL Query to be executed</param>
        /// <param name="commandParameters">Optional Parameter (If any)</param>
        /// <returns>Number of rows affected</returns>
        private IDataReader ExecuteDR(IDbConnection connection, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {
            return SqlExec.PostgreSql.PostgreHelper.ExecuteReader((NpgsqlConnection)connection, CommandType.Text, commandText, (NpgsqlTransaction)_trx, commandParameters);
        }

        /// <summary>
        /// Executes the SQL Query
        /// </summary>
        /// <param name="connection">current connection</param>
        /// <param name="commandType">command type => default: Text</param>
        /// <param name="commandText">SQL Query to be executed</param>
        /// <param name="commandParameters">Optional Parameter (If any)</param>
        /// <returns>Number of rows affected</returns>
        private IDataReader ExecuteDR(IDbConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return SqlExec.MSSql.SqlHelper.ExecuteReader((SqlConnection)connection, CommandType.Text, commandText, (SqlTransaction)_trx, commandParameters);
        }

        /// <summary>
        /// Executes the SQL Query
        /// </summary>
        /// <param name="connection">current connection</param>
        /// <param name="commandType">command type => default: Text</param>
        /// <param name="commandText">SQL Query to be executed</param>
        /// <param name="commandParameters">Optional Parameter (If any)</param>
        /// <returns>Number of rows affected</returns>
        private IDataReader ExecuteDR(IDbConnection connection, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            return SqlExec.MySql.MySqlHelper.ExecuteReader((MySqlConnection)connection, CommandType.Text, commandText, (MySqlTransaction)_trx, commandParameters);
        }


        ///*************************

        public IDataReader ExecuteReader(string sql)
        {
            return ExecuteReader(sql, null, (Trx)null);
        }

        public IDataReader ExecuteReader(string sql, Trx trxName)
        {
            return ExecuteReader(sql, null, trxName);
        }

        //public IDataReader ExecuteReader(string sql, SqlParameter[] arrparam, Trx trx)
        //{
        //    if (trx == null )
        //        return SqlExec.ExecuteQuery.ExecuteReader(sql, arrparam);


        //        sql = Ini.ConvertSqlQuery(sql);
        //        if (!IsActive())
        //            Start();


        //            if (DatabaseType.IsOracle)
        //            {
        //                OracleParameter[] oracleParam = SqlExec.ExecuteQuery.GetOracleParameter(arrparam);
        //                return ExecuteDR(_conn, CommandType.Text, sql, oracleParam);   //finally execute the query
        //            }
        //            else if (DatabaseType.IsPostgre)
        //            {
        //                NpgsqlParameter[] postgreParam = SqlExec.ExecuteQuery.GetPostgreParameter(arrparam);
        //                return ExecuteDR(_conn, CommandType.Text, sql, postgreParam);   //finally execute the query
        //            }
        //            else if (DatabaseType.IsMSSql)
        //            {
        //                return ExecuteDR(_conn, CommandType.Text, sql, arrparam);   //finally execute the query
        //            }
        //            else if (DatabaseType.IsMySql)
        //            {
        //                MySqlParameter[] mysqlParam = SqlExec.ExecuteQuery.GetMySqlParameter(arrparam);
        //                return ExecuteDR(_conn, CommandType.Text, sql, mysqlParam);   //finally execute the query
        //            }

        //    return null;

        //}

        public IDataReader ExecuteReader(string sql, SqlParameter[] arrparam, Trx trx)
        {
            if (trx == null)
                return SqlExec.ExecuteQuery.ExecuteReader(sql, arrparam);

            sql = DB.ConvertSqlQuery(sql);
            if (!IsActive())
                Start();


            if (DatabaseType.IsOracle)
            {
                OracleParameter[] oracleParam = SqlExec.ExecuteQuery.GetOracleParameter(arrparam);
                return ExecuteDR(_conn, CommandType.Text, sql, oracleParam);   //finally execute the query
            }
            else if (DatabaseType.IsPostgre)
            {
                NpgsqlParameter[] postgreParam = SqlExec.ExecuteQuery.GetPostgreParameter(arrparam);
                return ExecuteDR(_conn, CommandType.Text, sql, postgreParam);   //finally execute the query
            }
            else if (DatabaseType.IsMSSql)
            {
                return ExecuteDR(_conn, CommandType.Text, sql, arrparam);   //finally execute the query
            }
            else if (DatabaseType.IsMySql)
            {
                MySqlParameter[] mysqlParam = SqlExec.ExecuteQuery.GetMySqlParameter(arrparam);
                return ExecuteDR(_conn, CommandType.Text, sql, mysqlParam);   //finally execute the query
            }


            return null;

        }


        #endregion


        #region ExecuteDS

        /// <summary>
        /// Executes the SQL Query
        /// </summary>
        /// <param name="connection">current connection</param>
        /// <param name="commandType">command type => default: Text</param>
        /// <param name="commandText">SQL Query to be executed</param>
        /// <param name="commandParameters">Optional Parameter (If any)</param>
        /// <returns>Number of rows affected</returns>
        private DataSet ExecuteDS(IDbConnection connection, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            return SqlExec.Oracle.OracleHelper.ExecuteDataset((OracleConnection)connection, CommandType.Text, commandText, (OracleTransaction)_trx, commandParameters);
        }

        /// <summary>
        /// Executes the SQL Query
        /// </summary>
        /// <param name="connection">current connection</param>
        /// <param name="commandType">command type => default: Text</param>
        /// <param name="commandText">SQL Query to be executed</param>
        /// <param name="commandParameters">Optional Parameter (If any)</param>
        /// <returns>Number of rows affected</returns>
        private DataSet ExecuteDS(IDbConnection connection, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {
            return SqlExec.PostgreSql.PostgreHelper.ExecuteDataset((NpgsqlConnection)connection, CommandType.Text, commandText, (NpgsqlTransaction)_trx, commandParameters);
        }

        /// <summary>
        /// Executes the SQL Query
        /// </summary>
        /// <param name="connection">current connection</param>
        /// <param name="commandType">command type => default: Text</param>
        /// <param name="commandText">SQL Query to be executed</param>
        /// <param name="commandParameters">Optional Parameter (If any)</param>
        /// <returns>Number of rows affected</returns>
        private DataSet ExecuteDS(IDbConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return SqlExec.MSSql.SqlHelper.ExecuteDataset((SqlConnection)connection, CommandType.Text, commandText, (SqlTransaction)_trx, commandParameters);
        }

        /// <summary>
        /// Executes the SQL Query
        /// </summary>
        /// <param name="connection">current connection</param>
        /// <param name="commandType">command type => default: Text</param>
        /// <param name="commandText">SQL Query to be executed</param>
        /// <param name="commandParameters">Optional Parameter (If any)</param>
        /// <returns>Number of rows affected</returns>
        private DataSet ExecuteDS(IDbConnection connection, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            return SqlExec.MySql.MySqlHelper.ExecuteDataset((MySqlConnection)connection, CommandType.Text, commandText, (MySqlTransaction)_trx, commandParameters);
        }

        #endregion


        #region "Execute Dataset

        public DataSet ExecuteDataset(string sql)
        {
            return ExecuteDataset(sql, null, (Trx)null);
        }

        public DataSet ExecuteDataset(string sql, Trx trxName)
        {
            return ExecuteDataset(sql, null, trxName);
        }

        //public DataSet ExecuteDataset(string sql, SqlParameter[] arrparam, Trx trx)
        //{
        //    if (trxName == null )
        //        return SqlExec.ExecuteQuery.ExecuteDataset(sql, arrparam);


        //        sql = Ini.ConvertSqlQuery(sql);
        //        if (!IsActive())
        //            Start();


        //            if (DatabaseType.IsOracle)
        //            {
        //                OracleParameter[] oracleParam = SqlExec.ExecuteQuery.GetOracleParameter(arrparam);
        //                return ExecuteDS(_conn, CommandType.Text, sql, oracleParam);   //finally execute the query
        //            }
        //            else if (DatabaseType.IsPostgre)
        //            {
        //                NpgsqlParameter[] postgreParam = SqlExec.ExecuteQuery.GetPostgreParameter(arrparam);
        //                return ExecuteDS(_conn, CommandType.Text, sql, postgreParam);   //finally execute the query
        //            }
        //            else if (DatabaseType.IsMSSql)
        //            {
        //                return ExecuteDS(_conn, CommandType.Text, sql, arrparam);   //finally execute the query
        //            }
        //            else if (DatabaseType.IsMySql)
        //            {
        //                MySqlParameter[] mysqlParam = SqlExec.ExecuteQuery.GetMySqlParameter(arrparam);
        //                return ExecuteDS(_conn, CommandType.Text, sql, mysqlParam);   //finally execute the query
        //            }


        //    return null;

        //}

        public DataSet ExecuteDataset(string sql, SqlParameter[] arrparam, Trx trx)
        {
            if (trx == null)
                return SqlExec.ExecuteQuery.ExecuteDataset(sql, arrparam);


            sql = DB.ConvertSqlQuery(sql);
            if (!IsActive())
                Start();

            if (trx != null)
            {
                if (DatabaseType.IsOracle)
                {
                    OracleParameter[] oracleParam = SqlExec.ExecuteQuery.GetOracleParameter(arrparam);
                    return ExecuteDS(_conn, CommandType.Text, sql, oracleParam);   //finally execute the query
                }
                else if (DatabaseType.IsPostgre)
                {
                    NpgsqlParameter[] postgreParam = SqlExec.ExecuteQuery.GetPostgreParameter(arrparam);
                    return ExecuteDS(_conn, CommandType.Text, sql, postgreParam);   //finally execute the query
                }
                else if (DatabaseType.IsMSSql)
                {
                    return ExecuteDS(_conn, CommandType.Text, sql, arrparam);   //finally execute the query
                }
                else if (DatabaseType.IsMySql)
                {
                    MySqlParameter[] mysqlParam = SqlExec.ExecuteQuery.GetMySqlParameter(arrparam);
                    return ExecuteDS(_conn, CommandType.Text, sql, mysqlParam);   //finally execute the query
                }
            }
            return null;
        }


        #endregion

        #region Execute Scalar
        /// <summary>
        /// Execute SQL Query
        /// </summary>
        /// <param name="sql">Sql Query to be executed</param>
        /// <returns>Scalar value in object type</returns>
        public object ExecuteScalar(string sql)
        {
            return ExecuteScalar(sql, null, null);
        }

        /// <summary>
        /// Execute SQL Query
        /// </summary>
        /// <param name="sql">SQL Query to be executed</param>
        /// <param name="trxName">Transaction name</param>
        /// <returns>Scalar value in object type</returns>
        public object ExecuteScalar(string sql, Trx trxName)
        {
            if (trxName == null)    //if trxName is null execute the query as it is
                return ExecuteScalar(sql, null, null);
            else
                return ExecuteScalar(sql, null, trxName);
        }

        /// <summary>
        /// Execute SQL Query
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, SqlParameter[] param)
        {
            return ExecuteScalar(sql, param, null);
        }

        /// <summary>
        /// Execute SQL Query
        /// </summary>
        /// <param name="sql">sql query to be executed</param>
        /// <param name="param">parameters to be passed to the query</param>
        /// <param name="trxName">optional transaction name</param>
        /// <returns>Scalar value in object type. -1 if error occured</returns>
        public object ExecuteScalar(string sql, SqlParameter[] arrparam, Trx trx)
        {
            if ((trx == null))
                return SqlExec.ExecuteQuery.ExecuteScalar(sql, arrparam);


            sql = DB.ConvertSqlQuery(sql);
            if (!IsActive())
                Start();

            //Trx trx = trxName == null ? null : Trx.Get(trxName, true);

            if (DatabaseType.IsOracle)
            {
                OracleParameter[] oracleParam = SqlExec.ExecuteQuery.GetOracleParameter(arrparam);
                return ExecuteScalar(_conn, CommandType.Text, sql, oracleParam);   //finally execute the query
            }
            else if (DatabaseType.IsPostgre)
            {
                NpgsqlParameter[] postgreParam = SqlExec.ExecuteQuery.GetPostgreParameter(arrparam);
                return ExecuteScalar(_conn, CommandType.Text, sql, postgreParam);   //finally execute the query
            }
            else if (DatabaseType.IsMSSql)
            {
                return ExecuteScalar(_conn, CommandType.Text, sql, arrparam);   //finally execute the query
            }
            else if (DatabaseType.IsMySql)
            {
                MySqlParameter[] mysqlParam = SqlExec.ExecuteQuery.GetMySqlParameter(arrparam);
                return ExecuteScalar(_conn, CommandType.Text, sql, mysqlParam);   //finally execute the query
            }


            return -1;
        }


        /// <summary>
        /// Executes the SQL Query
        /// </summary>
        /// <param name="connection">current connection</param>
        /// <param name="commandType">command type => default: Text</param>
        /// <param name="commandText">SQL Query to be executed</param>
        /// <param name="commandParameters">Optional Parameter (If any)</param>
        /// <returns>Scalar value in object type</returns>
        private object ExecuteScalar(IDbConnection connection, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            return SqlExec.Oracle.OracleHelper.ExecuteScalar((OracleConnection)connection, CommandType.Text, commandText, (OracleTransaction)_trx, commandParameters);
        }

        /// <summary>
        /// Executes the SQL Query
        /// </summary>
        /// <param name="connection">current connection</param>
        /// <param name="commandType">command type => default: Text</param>
        /// <param name="commandText">SQL Query to be executed</param>
        /// <param name="commandParameters">Optional Parameter (If any)</param>
        /// <returns>Scalar value in object type</returns>
        private object ExecuteScalar(IDbConnection connection, CommandType commandType, string commandText, params NpgsqlParameter[] commandParameters)
        {
            return SqlExec.PostgreSql.PostgreHelper.ExecuteScalar((NpgsqlConnection)connection, CommandType.Text, commandText, commandParameters);
        }

        /// <summary>
        /// Executes the SQL Query
        /// </summary>
        /// <param name="connection">current connection</param>
        /// <param name="commandType">command type => default: Text</param>
        /// <param name="commandText">SQL Query to be executed</param>
        /// <param name="commandParameters">Optional Parameter (If any)</param>
        /// <returns>Scalar value in object type</returns>
        private object ExecuteScalar(IDbConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return SqlExec.MSSql.SqlHelper.ExecuteScalar((SqlConnection)connection, CommandType.Text, commandText, commandParameters);
        }

        /// <summary>
        /// Executes the SQL Query
        /// </summary>
        /// <param name="connection">current connection</param>
        /// <param name="commandType">command type => default: Text</param>
        /// <param name="commandText">SQL Query to be executed</param>
        /// <param name="commandParameters">Optional Parameter (If any)</param>
        /// <returns>Scalar value in object type</returns>
        private object ExecuteScalar(IDbConnection connection, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            return SqlExec.MySql.MySqlHelper.ExecuteScalar((MySqlConnection)connection, CommandType.Text, commandText, commandParameters);
        }

        #endregion

        #region Execute Procedure
        /// <summary>
        /// Execute Procedure
        /// </summary>
        /// <param name="sql">sql query to be executed</param>
        /// <param name="arrparam">parameters to be passed to the query</param>
        /// <param name="trx">optional transaction name</param>
        /// <returns>Scalar value in object type. -1 if error occured</returns>
        public SqlParameter[] ExecuteProcedure(string sql, SqlParameter[] arrparam, Trx trx)
        {
            if ((trx == null))
                return SqlExec.ExecuteQuery.ExecuteProcedure(sql, arrparam);

            if (!IsActive())
                Start();

            DbParameter[] param = null;
            if (DatabaseType.IsOracle)
            {
                param = GetOracleProcedureParameter(arrparam);
            }
            else if (DatabaseType.IsPostgre)
            {
                param = GetPostgreProcedureParameter(arrparam);
            }

            return DB.GetDatabase().ExecuteProcedure(_conn, sql, param, (DbTransaction)_trx);
        }

        #endregion
    }
#pragma warning restore 612, 618
}