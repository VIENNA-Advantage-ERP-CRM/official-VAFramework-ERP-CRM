using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OracleClient;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using System.Data.SqlClient;
using VIS.DataContracts;


namespace VIS.DBase
{
#pragma warning disable 612, 618
    public class DB
    {

        private static string connectionString = System.Configuration.ConfigurationManager.AppSettings["oracleConnectionString"];


        /// <summary>
        /// GetOracle Params by converting Data Contract sql Param
        /// </summary>
        /// <param name="arrParam">class sqlpaparam(data contract)</param>
        /// <returns> oracle parameter</returns>
        public static OracleParameter[] GetOracleParameter(SqlParams[] arrParam)
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
                param[i] = new OracleParameter(arrParam[i].name, arrParam[i].value);
            }
            return param;   //return the parameter
        }

        public static System.Data.SqlClient.SqlParameter[] GetSqlParameter(SqlParams[] arrParam)
        {
            if (arrParam == null)
                return null;
            //create and instance of OracleParameter and initialize the length with the length of arrParam
            System.Data.SqlClient.SqlParameter[] param = new System.Data.SqlClient.SqlParameter[arrParam.Length];
            //loop through all the values of arrParam
            for (int i = 0; i <= arrParam.Length - 1; i++)
            {
                //set one by one all the values to the OracleParameter
                //replace @ with ? for use in Oracle
                //string str = arrParam[i].SqlDbType.ToString();
                //string strVal = to_date(arrParam[i].Value.ToString(), "mm/dd/yyyy");

                if (arrParam[i].isDate)
                {
                    arrParam[i].value = Convert.ToDateTime(arrParam[i].value).ToUniversalTime().Date;
                }
                else if (arrParam[i].isDateTime)
                {
                    arrParam[i].value = Convert.ToDateTime(arrParam[i].value).ToUniversalTime();
                }


                param[i] = new System.Data.SqlClient.SqlParameter(arrParam[i].name, arrParam[i].value);
            }
            return param;   //return the parameter
        }

        public static string ConvertSqlQuery(string sql)
        {
            string final_query = sql;    //value which is to be returned back to the caller
            //string new_sql = Ini.RemoveInvalidSpaces(sql);    //use only if needed to avoid unnecessary calling overhead
            // final_query = new_sql.ToLower();
            final_query = sql;
            //if (DatabaseType.IsPostgre)
            //{
            //    string strDB = "vienna";
            //    if (!DataBase.DB.UseMigratedConnection)
            //    {
            //        strDB = VConnection.Get().Db_name;
            //    }
            //    else
            //    {
            //        strDB = WindowMigration.MDialog.GetMConnection().Db_name;
            //    }
            //    string strSearchPath = "set search_path to " + strDB + ", public;";
            //    final_query = DB_PostgreSQL.ConvertStatement(final_query);
            //    final_query = final_query.Insert(0, strSearchPath);
            //}
            //GlobalVariable.LAST_EXECUTED_QUERY = final_query.ToString().Trim();
            return final_query.ToString(); //return the converted sql query
        }

        /// <summary>
        /// Get Value from sql
        /// </summary>
        /// <param name="trxName">trx</param>
        /// <param name="sql">sql</param>
        /// <param name="int_param1">parameter 1</param>
        /// <returns>value or -1</returns>
        public static int ExecuteQuery(String sql, SqlParams[] param1, Trx trx, bool error)
        {

            //try
            //{

            System.Data.SqlClient.SqlParameter[] param = GetSqlParameter(param1);
            //Trx trx = trxName == null ? null : Trx.Get(trxName, true);
            if (trx != null)
            {
                return trx.ExecuteNonQuery(sql, param, trx);
            }
            else
            {
                return VAdvantage.SqlExec.ExecuteQuery.ExecuteNonQuery(sql, param);
            }
            //}
            //catch (System.Data.Common.DbException ex)
            //{
            //    if (ignoreError)
            //    {
            //        log.Log(Level.WARNING, trxName, ex.Message);
            //    }
            //    else
            //    {
            //        log.Log(Level.SEVERE, trxName, ex);
            //        log.SaveError("DBExecuteError", ex);
            //    }
            //    return -1;
            //}
            //catch (Exception ex)
            //{
            //    log.Severe(ex.ToString());
            //    //log.SaveError(ex.ToString());
            //    return -1;
            //}
        }

        //modified by Deepak
        //public static Decimal? GetSQLValueBD(String trxName, String sql, int int_param1)
        //{
        //    Decimal? retValue = null;
        //    SqlParameter[] param = new SqlParameter[1];
        //    IDataReader idr = null;
        //    try
        //    {
        //        //pstmt = prepareStatement(sql, trxName);
        //        //pstmt.setInt(1, int_param1);
        //        param[0] = new SqlParameter("@param1", int_param1);
        //        idr = DataBase.DB.ExecuteReader(sql, param, trxName);
        //        if (idr.Read())
        //        {
        //            retValue = Utility.Util.GetValueOfDecimal(idr[0]);// rs.getBigDecimal(1);
        //        }
        //        else
        //            log.Info("No Value " + sql + " - Param1=" + int_param1);
        //        idr.Close();
        //    }
        //    catch (Exception e)
        //    {
        //        log.Log(Level.SEVERE, sql + " - Param1=" + int_param1 + " [" + trxName + "]", e);
        //    }

        //    return retValue;
        //}
        //public static int GetSQLValue(String trxName, String sql)
        //{
        //    int retValue = -1;
        //    IDataReader idr = null;
        //    try
        //    {
        //        idr = DataBase.DB.ExecuteReader(sql, null, trxName);
        //        if (idr.Read())
        //            retValue = Utility.Util.GetValueOfInt(idr[0].ToString());
        //        else
        //        {
        //            //log.fine("No Value " + sql);
        //        }
        //        idr.Close();
        //    }
        //    catch (Exception e)
        //    {
        //        if (idr != null)
        //        {
        //            idr.Close();
        //        }
        //        log.Log(Level.SEVERE, sql, e);
        //        //ErrorLog.FillErrorLog("DataBase.DB.GetSQLValue", sql, e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
        //    }
        //    return retValue;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataset(string sql, SqlParams[] param)
        {
            return ExecuteDataset(sql, param, null);
        }

        /// <summary>
        /// Executes the query and fills the dataset
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="param"></param>
        /// <returns>dataset</returns>
        public static DataSet ExecuteDataset(string sql, SqlParams[] param, Trx trx)
        {

            System.Data.SqlClient.SqlParameter[] param1 = GetSqlParameter(param);

            //Trx trx = trxName == null ? null : Trx.Get(trxName, true);
            if (trx != null)
            {
                return trx.ExecuteDataset(sql, param1, trx);
            }
            else
            {
                return VAdvantage.SqlExec.ExecuteQuery.ExecuteDataset(sql, param1);
            }
        }

        /// <summary>
        /// Executes the query and fills the dataset
        /// </summary>
        /// <param name="sql">sql string</param>
        /// <param name="page">page </param>
        /// <param name="pageSize">size of page</param>
        /// <returns>return dataset</returns>
        public static DataSet ExecuteDatasetPaging(string sql, int page, int pageSize, int increment = 0)
        {
            return VAdvantage.DataBase.DB.GetDatabase().ExecuteDatasetPaging(VAdvantage.DataBase.DB.ConvertSqlQuery(sql), page, pageSize, increment);
        }

        /// <summary>
        /// Executes the query and fills the dataset
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns>dataset</returns>
        public static DataSet ExecuteDataset(string sql)
        {
            return ExecuteDataset(sql, null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static IDataReader ExecuteReader(string sql, SqlParams[] param)
        {
            return ExecuteReader(sql, param, null);
        }

        /// <summary>
        /// Executes the query and fills the dataset
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="param"></param>
        /// <returns>dataset</returns>
        public static IDataReader ExecuteReader(string sql, SqlParams[] param1, Trx trx)
        {
            System.Data.SqlClient.SqlParameter[] param = GetSqlParameter(param1);
            //Trx trx = trxName == null ? null : Trx.Get(trxName, true);
            if (trx != null)
            {
                return trx.ExecuteReader(sql, param, trx);
            }
            else
            {
                return VAdvantage.SqlExec.ExecuteQuery.ExecuteReader(sql, param);
            }
        }

        public static object ExecuteScalar(string sql, SqlParams[] param1, Trx trx)
        {
            System.Data.SqlClient.SqlParameter[] param = GetSqlParameter(param1);
            //Trx trx = trxName == null ? null : Trx.Get(trxName, true);
            if (trx != null)
            {
                return trx.ExecuteScalar(sql, param, trx);
            }
            else
            {
                return VAdvantage.SqlExec.ExecuteQuery.ExecuteScalar(sql, param);
            }
        }

        public static int ExecuteQuery(string sql, SqlParams[] param, Trx trxName)
        {
            return ExecuteQuery(sql, param, trxName, false);
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
    }
#pragma warning disable 612, 618
}
