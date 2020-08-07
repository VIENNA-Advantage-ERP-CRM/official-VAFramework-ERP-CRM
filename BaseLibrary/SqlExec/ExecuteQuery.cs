/********************************************************
 * Module Name    : DatabaseExecute
 * Purpose        : Executes the query                  
 * Author         : Jagmohan Bhatt
 * Date           : 20-Nov-2008
 * Class Used     : MySqlHelper.cs
 *                  SqlHelper.cs
 *                  OracleHelper.cs
 *                  PostgreSqlHelper.cs
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Npgsql;
using MySql.Data.MySqlClient;
using System.Collections;
//using System.Data.OracleClient;
using VAdvantage.DataBase;
using Oracle.ManagedDataAccess.Client;
using System.Data.Common;
using CoreLibrary.DataBase;

namespace VAdvantage.SqlExec
{
#pragma warning disable 612, 618
    /// <summary>
    /// Executes the SQL Query
    /// </summary>
    public static class ExecuteQuery
    {
        #region Execute Dataset
        /// <summary>
        /// Executes the SQL Query and returns the data in the form of DataSet
        /// </summary>
        /// <param name="sql">SQL Query which is to be executed</param>
        /// <returns>Executed SQL Query in the form of Dataset</returns>
        public static DataSet ExecuteDataset(string sql)
        {
            if (DatabaseType.IsPostgre)
                return PostgreSql.PostgreHelper.ExecuteDataset(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql));
            else if (DatabaseType.IsMSSql)
                return SqlExec.MSSql.SqlHelper.ExecuteDataset(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql));
            else if (DatabaseType.IsMySql)
                return MySql.MySqlHelper.ExecuteDataset(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql));
            else if (DatabaseType.IsOracle)
                return Oracle.OracleHelper.ExecuteDataset(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql));


            return PostgreSql.PostgreHelper.ExecuteDataset(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql));
        }

        /// <summary>
        /// Executes the SQL Query and returns the data in the form of DataSet
        /// </summary>
        /// <param name="sql">SQL Query which is to be executed</param>
        /// <param name="arrParam">Parameters to be passed in SQL Query</param>
        /// <returns>Executed SQL Query in the form of Dataset</returns>
        public static DataSet ExecuteDataset(string sql, SqlParameter[] arrParam)
        {
            if (DatabaseType.IsPostgre)
            {
                NpgsqlParameter[] param = GetPostgreParameter(arrParam);
                return PostgreSql.PostgreHelper.ExecuteDataset(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql), param);
            }
            else if (DatabaseType.IsMSSql)
            {
                return MSSql.SqlHelper.ExecuteDataset(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql), arrParam);
            }
            else if (DatabaseType.IsMySql)
            {
                MySqlParameter[] param = GetMySqlParameter(arrParam);
                return MySql.MySqlHelper.ExecuteDataset(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql), param);
            }
            else if (DatabaseType.IsOracle)
            {
                OracleParameter[] param = GetOracleParameter(arrParam);
                return Oracle.OracleHelper.ExecuteDataset(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql), param);
            }

            NpgsqlParameter[] param_final = GetPostgreParameter(arrParam);
            return PostgreSql.PostgreHelper.ExecuteDataset(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql), param_final);
        }
        #endregion

        #region Execute Scalar
        /// <summary>
        /// Executes the SQL Query and returns the data in the form of Scalar
        /// </summary>
        /// <param name="sql">SQL Query which is to be executed</param>
        /// <returns>Executed SQL Query in the form of Scalar</returns>
        public static string ExecuteScalar(string sql)
        {
            if (DatabaseType.IsMySql)
                return PostgreSql.PostgreHelper.ExecuteScalar(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql)).ToString();
            else if (DatabaseType.IsMSSql)
                return MSSql.SqlHelper.ExecuteScalar(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql)).ToString();
            else if (DatabaseType.IsMySql)
                return MySql.MySqlHelper.ExecuteScalar(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql)).ToString();
            else if (DatabaseType.IsOracle)
                return Oracle.OracleHelper.ExecuteScalar(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql)).ToString();

            return PostgreSql.PostgreHelper.ExecuteScalar(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql)).ToString();
        }

        /// <summary>
        /// Executes the SQL Query and returns the data in the form of Scalar
        /// </summary>
        /// <param name="sql">SQL Query which is to be executed</param>
        /// <param name="arrParam">Parameters to be passed in SQL Query</param>
        /// <returns>Executed SQL Query in the form of Scalar</returns>
        public static object ExecuteScalar(string sql, SqlParameter[] arrParam)
        {
            if (DatabaseType.IsPostgre)
            {
                NpgsqlParameter[] param = GetPostgreParameter(arrParam);
                return PostgreSql.PostgreHelper.ExecuteScalar(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql), param);
            }
            else if (DatabaseType.IsMSSql)
            {
                return MSSql.SqlHelper.ExecuteScalar(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql), arrParam);
            }
            else if (DatabaseType.IsMySql)
            {
                MySqlParameter[] param = GetMySqlParameter(arrParam);
                return MySql.MySqlHelper.ExecuteScalar(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql), param);
            }
            else if (DatabaseType.IsOracle)
            {
                OracleParameter[] param = GetOracleParameter(arrParam);
                return Oracle.OracleHelper.ExecuteScalar(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql), param);
            }

            NpgsqlParameter[] param_final = GetPostgreParameter(arrParam);
            return PostgreSql.PostgreHelper.ExecuteScalar(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql), param_final);
        }
        #endregion

        #region Execute Reader
        /// <summary>
        /// Executes the SQL Query and returns the data in the form of IDataReader
        /// </summary>
        /// <param name="sql">SQL Query which is to be executed</param>
        /// <returns>Executed SQL Query in the form of IDataReader</returns>
        public static IDataReader ExecuteReader(string sql)
        {
            IDataReader dr_return = null;
            if (DatabaseType.IsMySql)
            {
                dr_return = (IDataReader)MySql.MySqlHelper.ExecuteReader(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql));
            }
            else if (DatabaseType.IsMSSql)
            {
                dr_return = (IDataReader)MSSql.SqlHelper.ExecuteReader(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql));
            }
            else if (DatabaseType.IsOracle)
            {
                dr_return = (IDataReader)Oracle.OracleHelper.ExecuteReader(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql));
            }
            else if (DatabaseType.IsPostgre)
            {
                dr_return = (IDataReader)PostgreSql.PostgreHelper.ExecuteReader(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql));
            }

            return dr_return;
        }

        /// <summary>
        /// Executes the SQL Query and returns the data in the form of IDataReader
        /// </summary>
        /// <param name="sql">SQL Query which is to be executed</param>
        /// <param name="arrParam">Parameters to be passed in SQL Query</param>
        /// <returns>Executed SQL Query in the form of IDataReader</returns>
        public static IDataReader ExecuteReader(string sql, SqlParameter[] arrParam)
        {
            IDataReader dr_return = null;
            if (DatabaseType.IsMySql)   //check if mysql
            {
                MySqlParameter[] param = ExecuteQuery.GetMySqlParameter(arrParam);  //Get Param values
                //finally execute the query
                dr_return = (IDataReader)MySql.MySqlHelper.ExecuteReader(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql), param);
            }
            else if (DatabaseType.IsMSSql)
            {
                dr_return = (IDataReader)MSSql.SqlHelper.ExecuteReader(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql), arrParam);
            }
            else if (DatabaseType.IsOracle)
            {
                OracleParameter[] param = ExecuteQuery.GetOracleParameter(arrParam);
                dr_return = (IDataReader)Oracle.OracleHelper.ExecuteReader(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql), param);

            }
            else if (DatabaseType.IsPostgre)
            {
                NpgsqlParameter[] param = GetPostgreParameter(arrParam);
                dr_return = (IDataReader)PostgreSql.PostgreHelper.ExecuteReader(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql), param);
            }
            //else if (DatabaseType.IsPostgrePlus)
            //{
            //    NpgsqlParameter[] param = GetPostgreParameter(arrParam);
            //    dr_return = (IDataReader)PostgreSql.PostgreHelper.ExecuteReader(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql), param);
            //}

            return dr_return;
        }
        #endregion

        #region Execute Non Query
        /// <summary>
        /// Executes the SQL Query and returns number of rows affected
        /// </summary>
        /// <param name="sql">SQL Query which is to be executed</param>
        /// <returns>Executed SQL Query in the form of Scalar</returns>
        public static int ExecuteNonQuery(string sql)
        {
            if (DatabaseType.IsMySql)
                return int.Parse(PostgreSql.PostgreHelper.ExecuteNonQuery(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql)).ToString());
            else if (DatabaseType.IsMSSql)
                return int.Parse(MSSql.SqlHelper.ExecuteNonQuery(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql)).ToString());
            else if (DatabaseType.IsMySql)
                return int.Parse(MySql.MySqlHelper.ExecuteNonQuery(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql)).ToString());
            else if (DatabaseType.IsOracle)
                return int.Parse(Oracle.OracleHelper.ExecuteNonQuery(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql)).ToString());

            return int.Parse(PostgreSql.PostgreHelper.ExecuteNonQuery(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql)).ToString());
        }
        #endregion


        #region Execute Non Query
        /// <summary>
        /// Executes the SQL Query and returns number of rows affected
        /// </summary>
        /// <param name="sql">SQL Query which is to be executed</param>
        /// <param name="blnPO">true if called from PO</param>
        /// <returns>Executed SQL Query in the form of Scalar</returns>
        public static int ExecuteNonQuery(string sql, bool blnPO)
        {
            //IDbConnection conn = GetConnection();
            //conn.Open();

            if (DatabaseType.IsMySql)
                return int.Parse(PostgreSql.PostgreHelper.ExecuteNonQuery(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql)).ToString());
            else if (DatabaseType.IsMSSql)
                return int.Parse(MSSql.SqlHelper.ExecuteNonQuery(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql)).ToString());
            else if (DatabaseType.IsMySql)
                return int.Parse(MySql.MySqlHelper.ExecuteNonQuery(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql)).ToString());
            else if (DatabaseType.IsOracle)
                return int.Parse(Oracle.OracleHelper.ExecuteNonQuery(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql)).ToString());

            return int.Parse(PostgreSql.PostgreHelper.ExecuteNonQuery(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql)).ToString());
        }


        /// <summary>
        /// Executes the SQL Query and returns number of rows affected
        /// </summary>
        /// <param name="sql">SQL Query which is to be executed</param>
        /// <param name="arrParam">Parameters to be passed in SQL Query</param>
        /// <returns>Executed SQL Query in the form of Scalar</returns>
        public static int ExecuteNonQuery(string sql, SqlParameter[] arrParam)
        {
            if (DatabaseType.IsPostgre)
            {
                NpgsqlParameter[] param = GetPostgreParameter(arrParam);
                return int.Parse(PostgreSql.PostgreHelper.ExecuteNonQuery(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql), param).ToString());
            }
            else if (DatabaseType.IsMSSql)
            {
                return int.Parse(MSSql.SqlHelper.ExecuteNonQuery(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql), arrParam).ToString());
            }
            else if (DatabaseType.IsMySql)
            {
                MySqlParameter[] param = GetMySqlParameter(arrParam);
                return int.Parse(MySql.MySqlHelper.ExecuteNonQuery(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql), param).ToString());
            }
            else if (DatabaseType.IsOracle)
            {
                OracleParameter[] param = GetOracleParameter(arrParam);
                return int.Parse(Oracle.OracleHelper.ExecuteNonQuery(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql), param).ToString());
            }

            NpgsqlParameter[] param_final = GetPostgreParameter(arrParam);
            return int.Parse(PostgreSql.PostgreHelper.ExecuteNonQuery(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql), param_final).ToString());
        }
        #endregion


        #region Execute Procedure

        /// <summary>
        /// Executes the Procedure and returns the data in the form of Sql Parameteres
        /// </summary>
        /// <param name="sql">SQL Query which is to be executed</param>
        /// <param name="arrParam">Parameters to be passed in SQL Query</param>
        /// <returns>Executed SQL Query in the form of Scalar</returns>
        public static SqlParameter[] ExecuteProcedure(string sql, SqlParameter[] arrParam)
        {
            DbParameter[] param = null;
            if (DatabaseType.IsPostgre)
            {
                param = GetPostgreProcedureParameter(arrParam);
            }

            else if (DatabaseType.IsOracle)
            {
                param = GetOracleProcedureParameter(arrParam);
            }

            return DB.GetDatabase().ExecuteProcedure(null, sql, param, null);
        }

        #endregion

        #region Process and Convert SQLParameter
        /// <summary>
        /// Creates the MySqlParameter class from passed in SQLParameter
        /// </summary>
        /// <param name="arrParam">Array of SQLParameter</param>
        /// <returns>MySqlParameter</returns>
        public static MySqlParameter[] GetMySqlParameter(SqlParameter[] arrParam)
        {
            if (arrParam == null)
                return null;

            //create and instance of MySqlParameter and initialize the length with the length of arrParam
            MySqlParameter[] param = new MySqlParameter[arrParam.Length];
            //loop through all the values of arrParam
            for (int i = 0; i <= arrParam.Length - 1; i++)
            {
                //set one by one all the values to the MySqlParameter
                //replace @ with ? for use in MySql
                param[i] = new MySqlParameter(arrParam[i].ParameterName.Replace("@", "?"), arrParam[i].Value);
            }
            return param;   //return the parameter
        }

        /// <summary>
        /// Creates the OracleParameter class from passed in SQLParameter
        /// </summary>
        /// <param name="arrParam">Array of SQLParameter</param>
        /// <returns>OracleParameter</returns>
        public static OracleParameter[] GetOracleParameter(SqlParameter[] arrParam)
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
        public static NpgsqlParameter[] GetPostgreParameter(SqlParameter[] arrParam)
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
        /// Creates the OracleParameter class from passed in SQLParameter
        /// </summary>
        /// <param name="arrParam">Array of SQLParameter</param>
        /// <returns>OracleParameter</returns>
        public static OracleParameter[] GetOracleProcedureParameter(SqlParameter[] arrParam)
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
        public static NpgsqlParameter[] GetPostgreProcedureParameter(SqlParameter[] arrParam)
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
        #endregion

        //#region Execute Non Query with Transaction
        ///// <summary>
        ///// Executes the SQL Query and returns number of rows affected
        ///// </summary>
        ///// <param name="sql">SQL Query which is to be executed</param>
        ///// <returns>Executed SQL Query in the form of Scalar</returns>
        //static OracleTransaction trans = null;
        //static NpgsqlTransaction transPostgre = null;

        //public static string ExecuteNonQuery(string sql, bool isTrans, bool isAutoComiit)
        //{


        //    if (DatabaseType.IsPostgre)
        //    {
        //        //return PostgreSql.PostgreHelper.ExecuteNonQuery(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql)).ToString();
        //        try
        //        {
        //            NpgsqlConnection conn = new NpgsqlConnection(DB.GetConnectionString());
        //            if (conn.State != ConnectionState.Open)
        //            {

        //                conn.Open();
        //            }

        //            if (isTrans)
        //            {
        //                transPostgre = conn.BeginTransaction();
        //            }
        //            string s = PostgreSql.PostgreHelper.ExecuteNonQuery(transPostgre, CommandType.Text, DB.ConvertSqlQuery(sql)).ToString();
        //            if (isAutoComiit)
        //            {
        //                transPostgre.Commit();
        //                conn.Close();
        //            }

        //            return "1";

        //        }
        //        catch (Exception ex)
        //        {
        //            try
        //            {
        //                transPostgre.Rollback();
        //                return "Error: Transaction failed" + ex.Message;
        //            }
        //            catch (Exception ex2)
        //            {
        //                return "Error: " + ex2.Message;
        //            }
        //        }
        //    }
        //    else if (DatabaseType.IsMSSql)
        //        return MSSql.SqlHelper.ExecuteNonQuery(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql)).ToString();
        //    else if (DatabaseType.IsMySql)
        //        return MySql.MySqlHelper.ExecuteNonQuery(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql)).ToString();
        //    else if (DatabaseType.IsOracle)
        //    {
        //        try
        //        {
        //            OracleConnection conn = new OracleConnection(DB.GetConnectionString());
        //            conn.Open();
        //            if (isTrans)
        //            {
        //                trans = conn.BeginTransaction();
        //            }
        //            string s = Oracle.OracleHelper.ExecuteNonQuery(trans, CommandType.Text, DB.ConvertSqlQuery(sql)).ToString();
        //            if (isAutoComiit)
        //            {
        //                trans.Commit();
        //                //conn.Close();
        //            }

        //            return "1";
        //        }
        //        catch (Exception ex)
        //        {
        //            try
        //            {
        //                trans.Rollback();
        //                return "Error: Transaction failed" + ex.Message;
        //            }
        //            catch (Exception ex2)
        //            {
        //                return "Error: " + ex2.Message;
        //            }
        //        }


        //    }

        //    return "Not Executed";
        //}

        /////// <summary>
        /////// Executes the SQL Query and returns number of rows affected
        /////// </summary>
        /////// <param name="sql">SQL Query which is to be executed</param>
        /////// <param name="arrParam">Parameters to be passed in SQL Query</param>
        /////// <returns>Executed SQL Query in the form of Scalar</returns>
        ////public static string ExecuteNonQuery(string sql, SqlParameter[] arrParam, bool isTrans, bool isAutoComiit)
        ////{
        ////    if (DatabaseType.IsPostgre)
        ////    {
        ////        NpgsqlParameter[] param = GetPostgreParameter(arrParam);
        ////        return PostgreSql.PostgreHelper.ExecuteNonQuery(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql), param).ToString();
        ////    }
        ////    else if (DatabaseType.IsMSSql)
        ////    {
        ////        return MSSql.SqlHelper.ExecuteNonQuery(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql), arrParam).ToString();
        ////    }
        ////    else if (DatabaseType.IsMySql)
        ////    {
        ////        MySqlParameter[] param = GetMySqlParameter(arrParam);
        ////        return MySql.MySqlHelper.ExecuteNonQuery(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql), param).ToString();
        ////    }
        ////    else if (DatabaseType.IsOracle)
        ////    {
        ////        OracleParameter[] param = GetOracleParameter(arrParam);

        ////        OracleTransaction trans = null;
        ////        try
        ////        {
        ////            OracleConnection conn = new OracleConnection(DB.GetConnectionString());
        ////            conn.Open();
        ////            if (isTrans)
        ////            {
        ////                trans = conn.BeginTransaction();
        ////            }
        ////            string s = Oracle.OracleHelper.ExecuteNonQuery(trans, CommandType.Text, DB.ConvertSqlQuery(sql), param).ToString();
        ////            if (isAutoComiit)
        ////            {
        ////                trans.Commit();
        ////            }
        ////            return s;
        ////        }
        ////        catch (Exception ex)
        ////        {
        ////            try
        ////            {
        ////                trans.Rollback();
        ////                return "Error: Transaction failed" + ex.Message;
        ////            }
        ////            catch (Exception ex2)
        ////            {
        ////                return "Error: " + ex2.Message;
        ////            }
        ////        }
        ////    }

        ////    return "Not Executed";
        ////}
        //#endregion

        #region Execute Scalar with stored procedure
        /// <summary>
        /// Executes the stored procedure and returns the data in the form of Scalar
        /// </summary>
        /// <param name="spName">Stored procedure name which is to be executed</param>
        /// <param name="blnVal">true</param>
        /// <returns>Executed stored procedure in the form of Scalar</returns>
        public static string ExecuteScalar(string spName, bool blnVal)
        {
            if (DatabaseType.IsMySql)
                return PostgreSql.PostgreHelper.ExecuteScalar(DB.GetConnectionString(), CommandType.StoredProcedure, spName).ToString();
            else if (DatabaseType.IsMSSql)
                return MSSql.SqlHelper.ExecuteScalar(DB.GetConnectionString(), CommandType.StoredProcedure, spName).ToString();
            else if (DatabaseType.IsMySql)
                return MySql.MySqlHelper.ExecuteScalar(DB.GetConnectionString(), CommandType.StoredProcedure, spName).ToString();
            else if (DatabaseType.IsOracle)
                return Oracle.OracleHelper.ExecuteScalar(DB.GetConnectionString(), CommandType.StoredProcedure, spName).ToString();

            return PostgreSql.PostgreHelper.ExecuteScalar(DB.GetConnectionString(), CommandType.StoredProcedure, spName).ToString();
        }



        /// <summary>
        /// Execute query with passing transaction
        /// </summary>
        /// <param name="iTrx">IDbTransaction onject</param>
        /// <param name="sql">sql queyr</param>
        /// <param name="isTrans">true or false if transaction</param>
        /// <param name="isAutoComiit">wheather to autocommit or not</param>
        /// <returns></returns>
        //public static string ExecuteScalar(IDbTransaction iTrx, string sql, bool isTrans, bool isAutoComiit)
        //{
        //    if (DatabaseType.IsPostgre)
        //    {
        //        //return PostgreSql.PostgreHelper.ExecuteNonQuery(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql)).ToString();
        //        try
        //        {
        //            nTrx = (NpgsqlTransaction)iTrx;
        //            NpgsqlConnection myconn = nTrx.Connection;
        //            if (myconn.State != ConnectionState.Open)
        //            {
        //                myconn.Open();
        //            }

        //            string s = PostgreSql.PostgreHelper.ExecuteScalar(nTrx, CommandType.Text, DB.ConvertSqlQuery(sql)).ToString();
        //            if (isAutoComiit)
        //            {
        //                nTrx.Commit();
        //                myconn.Close();
        //            }

        //            return "1";

        //        }
        //        catch (Exception ex)
        //        {
        //            try
        //            {
        //                nTrx.Rollback();
        //                return "Error: Transaction failed" + ex.Message;
        //            }
        //            catch (Exception ex2)
        //            {
        //                return "Error: " + ex2.Message;
        //            }
        //        }
        //    }
        //    else if (DatabaseType.IsMSSql)
        //        return MSSql.SqlHelper.ExecuteScalar(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql)).ToString();
        //    else if (DatabaseType.IsMySql)
        //        return MySql.MySqlHelper.ExecuteScalar(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql)).ToString();
        //    else if (DatabaseType.IsOracle)
        //    {
        //        try
        //        {
        //            oTrx = (OracleTransaction)iTrx;
        //            OracleConnection myconn = oTrx.Connection;

        //            if (myconn.State != ConnectionState.Open)
        //            {
        //                myconn.Open();
        //            }

        //            string s = Oracle.OracleHelper.ExecuteScalar(oTrx, CommandType.Text, DB.ConvertSqlQuery(sql)).ToString();
        //            if (isAutoComiit)
        //            {
        //                oTrx.Commit();
        //                //conn.Close();
        //            }

        //            return "1";
        //        }
        //        catch (Exception ex)
        //        {
        //            try
        //            {
        //                oTrx.Rollback();
        //                return "Error: Transaction failed" + ex.Message;
        //            }
        //            catch (Exception ex2)
        //            {
        //                return "Error: " + ex2.Message;
        //            }
        //        }


        //    }

        //    return "Not Executed";
        //}


        /// <summary>
        /// Executes the stored procedure and returns the data in the form of Scalar
        /// </summary>
        /// <param name="spName">Stored procedure which is to be executed</param>
        /// <param name="arrParam">Parameters to be passed in stored procedure</param>
        /// <param name="blnVal">true</param>
        /// <returns>Executed stored procedure in the form of Scalar</returns>
        public static string ExecuteScalar(string spName, SqlParameter[] arrParam, bool blnVal)
        {
            if (DatabaseType.IsPostgre)
            {
                NpgsqlParameter[] param = GetPostgreParameter(arrParam);
                return PostgreSql.PostgreHelper.ExecuteScalar(DB.GetConnectionString(), CommandType.StoredProcedure, spName, param).ToString();
            }
            else if (DatabaseType.IsMSSql)
            {
                return MSSql.SqlHelper.ExecuteScalar(DB.GetConnectionString(), CommandType.StoredProcedure, spName, arrParam).ToString();
            }
            else if (DatabaseType.IsMySql)
            {
                MySqlParameter[] param = GetMySqlParameter(arrParam);
                return MySql.MySqlHelper.ExecuteScalar(DB.GetConnectionString(), CommandType.StoredProcedure, spName, param).ToString();
            }
            else if (DatabaseType.IsOracle)
            {
                OracleParameter[] param = GetOracleParameter(arrParam);
                return Oracle.OracleHelper.ExecuteScalar(DB.GetConnectionString(), CommandType.StoredProcedure, spName, param).ToString();
            }

            NpgsqlParameter[] param_final = GetPostgreParameter(arrParam);
            return PostgreSql.PostgreHelper.ExecuteScalar(DB.GetConnectionString(), CommandType.StoredProcedure, spName, param_final).ToString();
        }
        #endregion

        //#region "Execute query with passing transaction"
        //private static NpgsqlTransaction nTrx;
        //private static OracleTransaction oTrx;

        ///// <summary>
        ///// Execute query with passing transaction
        ///// </summary>
        ///// <param name="iTrx">IDbTransaction onject</param>
        ///// <param name="sql">sql queyr</param>
        ///// <param name="isTrans">true or false if transaction</param>
        ///// <param name="isAutoComiit">wheather to autocommit or not</param>
        ///// <returns></returns>
        //public static string ExecuteNonQuery(IDbTransaction iTrx, string sql, bool isTrans, bool isAutoComiit)
        //{
        //    if (DatabaseType.IsPostgre)
        //    {
        //        //return PostgreSql.PostgreHelper.ExecuteNonQuery(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql)).ToString();
        //        try
        //        {
        //            nTrx = (NpgsqlTransaction)iTrx;
        //            NpgsqlConnection myconn = nTrx.Connection;
        //            if (myconn.State != ConnectionState.Open)
        //            {
        //                myconn.Open();
        //            }

        //            string s = PostgreSql.PostgreHelper.ExecuteNonQuery(nTrx, CommandType.Text, DB.ConvertSqlQuery(sql)).ToString();
        //            if (isAutoComiit)
        //            {
        //                nTrx.Commit();
        //                myconn.Close();
        //            }

        //            return "1";

        //        }
        //        catch (Exception ex)
        //        {
        //            try
        //            {
        //                nTrx.Rollback();
        //                return "Error: Transaction failed" + ex.Message;
        //            }
        //            catch (Exception ex2)
        //            {
        //                return "Error: " + ex2.Message;
        //            }
        //        }
        //    }
        //    else if (DatabaseType.IsMSSql)
        //        return MSSql.SqlHelper.ExecuteNonQuery(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql)).ToString();
        //    else if (DatabaseType.IsMySql)
        //        return MySql.MySqlHelper.ExecuteNonQuery(DB.GetConnectionString(), CommandType.Text, DB.ConvertSqlQuery(sql)).ToString();
        //    else if (DatabaseType.IsOracle)
        //    {
        //        try
        //        {
        //            oTrx = (OracleTransaction)iTrx;
        //            OracleConnection myconn = oTrx.Connection;

        //            if (myconn.State != ConnectionState.Open)
        //            {
        //                myconn.Open();
        //            }

        //            string s = Oracle.OracleHelper.ExecuteNonQuery(oTrx, CommandType.Text, DB.ConvertSqlQuery(sql)).ToString();
        //            if (isAutoComiit)
        //            {
        //                oTrx.Commit();
        //                //conn.Close();
        //            }

        //            return "1";
        //        }
        //        catch (Exception ex)
        //        {
        //            try
        //            {
        //                oTrx.Rollback();
        //                return "Error: Transaction failed" + ex.Message;
        //            }
        //            catch (Exception ex2)
        //            {
        //                return "Error: " + ex2.Message;
        //            }
        //        }


        //    }

        //    return "Not Executed";
        //}

        //#endregion

        //#region Gets the server transaction
        ///// <summary>
        ///// Get the sever transaction
        ///// </summary>
        ///// <returns>IDBTransaction</returns>
        //public static IDbTransaction GerServerTransaction()
        //{
        //    OracleTransaction  orclTrx = null;
        //    if (DatabaseType.IsOracle)
        //    {
        //        OracleConnection conn = new OracleConnection(DB.GetConnectionString());
        //        conn.Open();
        //        orclTrx = conn.BeginTransaction(IsolationLevel.ReadCommitted);
        //        return orclTrx;
        //    }
        //    else if (DatabaseType.IsPostgre)
        //    {
        //        NpgsqlConnection npgcon = new NpgsqlConnection(DB.GetConnectionString());
        //        npgcon.Open();

        //        string[] restrictions = new string[4];
        //        restrictions[2] = "ad_table";
        //        DataTable dt = npgcon.GetSchema("Columns", restrictions);
        //        NpgsqlTransaction psgrTrx = npgcon.BeginTransaction(IsolationLevel.ReadCommitted);

        //        return psgrTrx;
        //    }
        //    else if(DatabaseType.IsMSSql)
        //    {
        //        SqlConnection sqlcon = new SqlConnection(DB.GetConnectionString());

        //        sqlcon.Open();
        //        SqlTransaction mssqlTrx = sqlcon.BeginTransaction(IsolationLevel.ReadCommitted);

        //        return mssqlTrx;
        //    }
        //    else if (DatabaseType.IsMySql)
        //    {
        //        MySqlConnection mysqlcon = new MySqlConnection(DB.GetConnectionString());
        //        mysqlcon.Open();
        //        MySqlTransaction mysqlTrx = mysqlcon.BeginTransaction(IsolationLevel.ReadCommitted);

        //        return mysqlTrx;
        //    }

        //    return orclTrx;

        //}
        //#endregion

        #region Get Columns"
        /// <summary>
        /// Gets the column from db
        /// </summary>
        /// <param name="tableName">name of the table</param>
        /// <returns>DataTable</returns>
        public static DataTable GetColumns(string tableName)
        {
            DataTable dt = null;
            if (DatabaseType.IsOracle)
            {
                OracleConnection conn = new OracleConnection(DB.GetConnectionString());
                string[] restrictions = new string[2];
                restrictions[1] = tableName.ToUpper();
                conn.Open();
                dt = conn.GetSchema("Columns", restrictions);
                // dt = conn.GetSchema();
                conn.Close();
                return dt;
            }
            else if (DatabaseType.IsPostgre)
            {
                NpgsqlConnection npgcon = new NpgsqlConnection(DB.GetConnectionString());
                string[] restrictions = new string[4];
                restrictions[2] = tableName;
                dt = npgcon.GetSchema("Columns", restrictions);
                return dt;
            }
            else if (DatabaseType.IsMSSql)
            {
                SqlConnection sqlcon = new SqlConnection(DB.GetConnectionString());
                string[] restrictions = new string[4];
                restrictions[2] = tableName;
                dt = sqlcon.GetSchema("Columns", restrictions);
                return dt;
            }
            else if (DatabaseType.IsMySql)
            {
                MySqlConnection mysqlcon = new MySqlConnection(DB.GetConnectionString());
                string[] restrictions = new string[4];
                restrictions[2] = tableName;
                // dt = mysqlcon.GetSchema("Columns", restrictions);
                return dt;
            }

            return dt;

        }
        #endregion

    }


#pragma warning restore 612, 618
}
