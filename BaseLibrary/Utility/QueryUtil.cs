using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.Classes;
using System.Collections;
using VAdvantage.Login;
using VAdvantage.Logging;
//using VAdvantage.Apps;
using System.Drawing;
using System.Resources;
using VAdvantage.SqlExec;
using VAdvantage.Process;
using VAdvantage.Common;
using VAdvantage.DataBase;
using VAdvantage.Controller;
using System.Data;
using System.Data.SqlClient;

namespace VAdvantage.Utility
{
    public class QueryUtil
    {
        #region PrivateVariables
        public static int MAX_ROWS = 1000000;
        private static VLogger log = VLogger.GetVLogger(typeof(QueryUtil).FullName);
        #endregion

        /// <summary>
        /// Implement this callback class to generate the record for
        /// each row.Typically used to cast row results
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        public interface Callback<T>
        {
            T Cast(Object[] row);
        }

        /// <summary>
        /// Execute sql query with provided parameters
        /// </summary>
        /// <param name="trx">sql transaction</param>
        /// <param name="SQL">Sql query</param>
        /// <param name="callback"></param>
        /// <param name="params1">Param</param>
        /// <returns></returns>
        public static List<Object> ExecuteQuery(Trx trx, String SQL, Callback<Object> callback,
            List<Object> qParams)
        {
            Object[][] rows = ExecuteQuery(trx, SQL, qParams);
            List<Object> result = new List<Object>();
            foreach (Object[] row in rows)
            {
                result.Add(callback.Cast(row));
            }
            return result;
        }

        /// <summary>
        /// Execute sql query with provided parameters
        /// </summary>
        /// <param name="trx">transaction</param>
        /// <param name="SQL">sql query</param>
        /// <param name="qParams">parameters</param>
        /// <returns></returns>
        public static Object[][] ExecuteQuery(Trx trx, String SQL, List<Object> qParams)
        {
            StringBuilder logBuffer = new StringBuilder();
            logBuffer.Append("SQL: " + SQL + "\n");

            Object[][] result = new Object[0][];
            try
            {
                //pstmt = DB.prepareStatement(SQL, trx);
                SqlParameter[] param = new SqlParameter[qParams.Count];

                if (qParams != null)
                {
                    int i = 1;
                    foreach (Object obj in qParams)
                    {
                        logBuffer.Append("  qParams[" + i + "]: " + qParams[i - 1]);
                        if (qParams[i - 1] != null)
                        {
                            //logBuffer.Append(" (" + qParams[i - 1].getClass().getSimpleName() + ")");
                            logBuffer.Append(" (" + qParams[i - 1].GetType().FullName + ")");
                        }
                        logBuffer.Append("\n ");

                        if (obj is int || obj is decimal)
                        {
                            Decimal n = Convert.ToDecimal(obj);
                            try
                            {
                                //pstmt.setInt(i, n.intValueExact());
                                param[i - 1] = new SqlParameter("@param" + i.ToString(),
                                    Utility.Util.GetValueOfInt(n));
                            }
                            catch 
                            {
                                
                                param[i - 1] = new SqlParameter("@param" + i.ToString(),
                                    Utility.Util.GetValueOfDecimal(n));
                            }
                        }
                        else if (obj is DateTime)
                        {
                            //pstmt.setTimestamp(i, new Timestamp(((Date) obj).getTime()));
                            param[i - 1] = new SqlParameter("@param" + i.ToString(),
                                     Utility.Util.GetValueOfDateTime(obj));
                        }
                        else if (obj is Boolean)
                        {
                            //pstmt.setString(i, ((Boolean) obj).booleanValue() ? "Y" : "N");
                            param[i - 1] = new SqlParameter("@param" + i.ToString(),
                                     Utility.Util.GetValueOfBool(obj) ? "Y" : "N");
                        }
                        else if (obj is String)
                        {
                            //pstmt.setString(i, (String) obj);
                            param[i - 1] = new SqlParameter("@param" + i.ToString(), Utility.Util.GetValueOfString(obj));

                        }
                        else
                        {
                            //pstmt.setObject(i, obj);
                            param[i - 1] = new SqlParameter("@param" + i.ToString(), obj);
                        }
                        ++i;
                    }
                }
                log.Log(Level.FINE, logBuffer.ToString());

                //result = ExecuteQuery(pstmt);
                result = ExecuteQuery(SQL, param, trx);
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, logBuffer.ToString());
                throw new Exception(e.Message);
            }
            //finally
            //{
            //    DB.closeStatement(pstmt);
            //}
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        private static Object[][] ExecuteQuery(String sql, SqlParameter[] param, Trx trx)
        //private static Object[][] executeQuery(PreparedStatement pstmt)
        {
            DataSet ds = new DataSet();
            List<Object[]> result = new List<Object[]>();
            try
            {
                ds = DB.ExecuteDataset(sql, param, trx);
                //while ( idr.Read())
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    List<Object> row = new List<Object>();
                    for (int j = 0; j < ds.Tables[0].Columns.Count; ++j)
                    {
                        Object obj = ds.Tables[0].Rows[i][j];// rs.getObject(j);
                        if (obj is int || obj is Decimal)
                        {
                            //row.add(rs.getBigDecimal(j));
                            row.Add(Utility.Util.GetValueOfDecimal(ds.Tables[0].Rows[i][j]));
                        }
                        else if (obj is DateTime)
                        {
                            //row.add(rs.getTimestamp(j));
                            row.Add(Utility.Util.GetValueOfDateTime(ds.Tables[0].Rows[i][j]));
                        }
                        else
                        {
                            //row.add(rs.getString(j));
                            row.Add(Utility.Util.GetValueOfString(ds.Tables[0].Rows[i][j]));
                        }
                    }
                    result.Add(row.ToArray());//adding array of object in the list
                }
            }
            finally
            {
                ds = null;
            }
            return result.ToArray();
        }

        /// <summary>
        /// Get Value from sql
        /// </summary>
        /// <param name="trx">transaction</param>
        /// <param name="sql">sql</param>
        /// <param name="qParams"></param>
        /// <returns>first value or -1</returns>
        public static int GetSQLValue(Trx trx, String sql, List<Object> qParams)
        {
            if ((sql == null) || (sql.Length == 0))
            {
                throw new ArgumentException("Required parameter missing - " + sql);
            }
            int retValue = -1;
            Object[][] rows = ExecuteQuery(trx, sql, qParams);
            if (rows.Length > 0 && rows[0][0] != null)
            {
                retValue = Utility.Util.GetValueOfInt(rows[0][0]);
            }
            return retValue;
        }

        /// <summary>
        /// Get String Value from sql
        /// </summary>
        /// <param name="trx">transaction</param>
        /// <param name="sql">sql</param>
        /// <param name="qParams">parameters</param>
        /// <returns>first value or null</returns>
        public static String GetSQLValueString(Trx trx, String sql, List<Object> qParams)
        {
            String retValue = null;
            Object[][] rows = ExecuteQuery(trx, sql, qParams);
            if (rows.Length > 0 && rows[0][0] != null)
            {
                retValue = Utility.Util.GetValueOfString(rows[0][0]);
            }
            return retValue;
        }

        /// <summary>
        /// Get Decimal Value from sql
        /// </summary>
        /// <param name="trx">Transaction</param>
        /// <param name="sql">sql</param>
        /// <param name="qParams">parameter 1</param>
        /// <returns>first value or null</returns>
        public static Decimal GetSQLValueBD(Trx trx, String sql, List<Object> qParams)
        {
            Decimal retValue = Env.ZERO;
            Object[][] rows = ExecuteQuery(trx, sql, qParams);
            if (rows.Length > 0 && rows[0][0] != null)
            {
                retValue = Utility.Util.GetValueOfDecimal(rows[0][0]);
            }
            return retValue;
        }

        /// <summary>
        /// Execute sql query with provided parameters - limit ResultSet to MAX_ROWS
        /// </summary>
        /// <param name="trx"></param>
        /// <param name="SQL"></param>
        /// <param name="callback"></param>
        /// <param name="qParams"></param>
        /// <returns></returns>
        public static List<Object> ExecuteQueryMaxRows(Trx trx, String SQL, Callback<Object> callback,
                Object[] qParams)
        {
            Object[][] rows = ExecuteQueryMaxRows(trx, SQL, qParams, 0, MAX_ROWS);
            List<Object> result = new List<Object>();
            foreach (Object[] row in rows)
            {
                result.Add(callback.Cast(row));
            }
            return result;
        }

        /// <summary>
        /// Execute sql query with provided parameters - limit ResultSet to MAX_ROWS
        /// </summary>
        /// <param name="trx"></param>
        /// <param name="SQL"></param>
        /// <param name="qParams"></param>
        /// <returns></returns>
        public static Object[][] ExecuteQueryMaxRows(Trx trx, String SQL, Object[] qParams)
        {
            return ExecuteQueryMaxRows(trx, SQL, qParams, 0, MAX_ROWS);
        }

        /// <summary>
        /// Execute sql query with provided parameters - limit ResultSet to MAX_ROWS
        /// </summary>
        /// <param name="SQL"></param>
        /// <param name="qParams"></param>
        /// <returns></returns>
        public static Object[][] ExecuteQueryMaxRows(String SQL, Object[] qParams)
        {
            return ExecuteQueryMaxRows(null, SQL, qParams);
        }

        /// <summary>
        ///  execute max rows
        /// </summary>
        /// <param name="trx"></param>
        /// <param name="SQL"></param>
        /// <param name="qParams"></param>
        /// <param name="startRow">The row (zero-based) to start with</param>
        /// <param name="rowCount">The number of rows to return</param>
        /// <returns></returns>
        public static Object[][] ExecuteQueryMaxRows(Trx trx, String SQL, Object[] qParams,
            int startRow, int rowCount)
        {
            StringBuilder logBuffer = new StringBuilder();
            logBuffer.Append("SQL: " + SQL + "\n");
            Object[][] result = new Object[0][];
            try
            {
                //pstmt = DB.prepareStatement(SQL, trx);
                //pstmt.setMaxRows(startRow + rowCount);//not set here open for future
                SqlParameter[] param = new SqlParameter[qParams.Length];
                if (qParams != null)
                {
                    int i = 1;
                    foreach (Object obj in qParams)
                    {
                        logBuffer.Append("  qParams[" + i + "]: " + qParams[i - 1]);
                        if (qParams[i - 1] != null)
                        {
                            //  logBuffer.Append(" (" + qParams[i - 1].getClass().getSimpleName() + ")");
                            logBuffer.Append(" (" + qParams[i - 1].GetType().FullName + ")");
                        }
                        logBuffer.Append("\n ");
                        if (obj is int || obj is decimal)
                        {
                            Decimal n = Util.GetValueOfDecimal(obj);
                            try
                            {
                                //pstmt.setInt(i, n.intValueExact());
                                param[i - 1] = new SqlParameter("@param" + i.ToString(),
                                   Utility.Util.GetValueOfInt(n));
                            }
                            catch 
                            {
                                //pstmt.setBigDecimal(i, n);
                                param[i - 1] = new SqlParameter("@param" + i.ToString(),
                                   Utility.Util.GetValueOfDecimal(n));
                            }
                        }
                        else if (obj is DateTime)
                        {
                            //pstmt.setTimestamp(i, new Timestamp(((Date)obj).getTime()));
                            param[i - 1] = new SqlParameter("@param" + i.ToString(),
                                   Utility.Util.GetValueOfDateTime(obj));
                        }
                        else if (obj is Boolean)
                        {
                            //pstmt.setString(i, ((Boolean)obj).booleanValue() ? "Y" : "N");
                            param[i - 1] = new SqlParameter("@param" + i.ToString(),
                                   Utility.Util.GetValueOfBool(obj) ? "Y" : "N");
                        }
                        else if (obj is String)
                        {
                            //pstmt.setString(i, (String)obj);
                            param[i - 1] = new SqlParameter("@param" + i.ToString(),
                                   Utility.Util.GetValueOfString(obj));
                        }
                        else
                        {
                            //  pstmt.setObject(i, obj);
                            param[i - 1] = new SqlParameter("@param" + i.ToString(), obj);
                        }
                        ++i;
                    }
                }
                log.Log(Level.FINE, logBuffer.ToString());
                result = ExecuteQueryMaxRows(SQL, param, trx, startRow, rowCount);
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, logBuffer.ToString());
                throw new Exception(e.Message);
            }
            //finally
            //{
            //    DB.closeStatement(pstmt);
            //}
            return result;

        }

        /// <summary>
        /// execute sql queries
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="trx"></param>
        /// <param name="startRow"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        private static Object[][] ExecuteQueryMaxRows(string sql, SqlParameter[] param, Trx trx,
            int startRow, int rowCount)
        {
            List<Object[]> result = new List<Object[]>();
            IDataReader idr = null;
            try
            {
                //rs = pstmt.executeQuery();
                //ResultSetMetaData rsmd = rs.getMetaData();
                idr = DB.ExecuteReader(sql, param, trx);
                int rowNum = 0;



                while (idr.Read() && rowNum > startRow + rowCount)
                {
                    //start read of recods from rowNum
                    if (rowNum < startRow)
                    {
                        ++rowNum;
                        continue;
                    }

                    List<Object> row = new List<Object>();
                    for (int i = 0; i < idr.FieldCount; ++i)
                    {
                        Object obj = idr[i];
                        if (obj is int || obj is Decimal)
                        {
                            row.Add(Util.GetValueOfDecimal(idr[i]));
                        }
                        else if (obj is DateTime)
                        {
                            row.Add(Util.GetValueOfDateTime(idr[i]));
                        }
                        else
                        {
                            row.Add(Util.GetValueOfString(idr[i]));
                        }
                    }
                    result.Add(row.ToArray());
                    ++rowNum;
                }

            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }
            return result.ToArray();
        }
    }

}
