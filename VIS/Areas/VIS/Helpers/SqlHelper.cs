using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.DataContracts;

namespace VIS.Helpers
{
    /// <summary>
    /// help to fetch Data from dataServer and return json compatible class objects
    /// </summary>
    public class SqlHelper
    {
        Ctx _ctx;
        public void SetContext(Ctx ctx)
        {
            _ctx = ctx;
        }

        /// <summary>
        /// fetch Data from DataServer and create list of Jtable objects
        /// </summary>
        /// <param name="sqlIn">input parameter</param>
        /// <returns>null if error , other wise list of JTable object </returns>
        public List<JTable> ExecuteJDataSet(SqlParamsIn sqlIn)
        {
            try
            {
                List<JTable> outO = new List<JTable>();

                JTable obj = null;
                DataSet ds = null;

                string sql = sqlIn.sql;
                //string sql = sqlIn.sql;
                //if (!IsTableAccess(sql))
                //    return null;

                ds = ExecuteDataSet(sqlIn);

                if (ds == null || ds.Tables.Count < 1)
                    return null;



                // bool singleTable = ds.Tables.Count >= 0;

                //StringBuilder tables = new StringBuilder("[");
                // StringBuilder columns = new StringBuilder(!singleTable ? "[{" : "{");
                int MAX_ROWS = 10001;
                for (int table = 0; table < ds.Tables.Count; table++)
                {

                    obj = new JTable();

                    var dt = ds.Tables[table];


                    obj.name = dt.TableName;
                    obj.records = ds.Tables[table].Rows.Count;
                    obj.page = sqlIn.page;
                    obj.total = 1;

                    for (int column = 0; column < dt.Columns.Count; column++)
                    {
                        var cc = new JColumn();
                        cc.index = column;
                        cc.name = dt.Columns[column].ColumnName.ToLower();
                        cc.type = dt.Columns[column].DataType.ToString().ToLower();
                        obj.columns.Add(cc);
                    }

                    int count = dt.Rows.Count;
                    for (int row = 0; row < count; row++)
                    {
                        if (row > MAX_ROWS)
                        {
                            break;
                        }
                        JRow r = new JRow();
                        r.id = row;
                        for (int column = 0; column < dt.Columns.Count; column++)
                        {
                            //var c = new Dictionary<string,object>();
                            //c[dt.Columns[column].ColumnName.ToLower()] = dt.Rows[row][column];
                            r.cells[dt.Columns[column].ColumnName.ToLower()] = dt.Rows[row][column];
                            //rows.Append(dt.Columns[column].ColumnName).Append(":").Append(dt.Rows[row][column]);
                        }
                        obj.rows.Add(r);
                    }
                    outO.Add(obj);

                }
                ds.Tables.Clear();
                ds.Dispose();
                return outO;
            }
            catch
            {
                return null;
            }
        }



        //private bool isDataAccess(string sql)
        //{

        //}


        public DataSet ExecuteDataSet(SqlParamsIn sqlIn)
        {
            if (String.IsNullOrEmpty(sqlIn.sql))
            {
                return null;
            }
            string sql = sqlIn.sql;
            bool doPaging = sqlIn.pageSize > 0;
            SqlParams[] paramIn = sqlIn.param == null ? null : sqlIn.param.ToArray();
            Trx trxName = null;


            string[] tables = sql.Split('~');

            DataSet ds = new DataSet();

            int i = 0;
            foreach (string table in tables)
            {
                string tableName = "Table" + i;
                DataSet dsTemp = null;
                if (!doPaging)
                {
                    dsTemp = VIS.DBase.DB.ExecuteDataset(table, paramIn, trxName);

                    if (dsTemp != null && dsTemp.Tables.Count > 0)
                    {
                        DataTable data = dsTemp.Tables[0];
                        dsTemp.Tables.Remove(data);
                        data.TableName = tableName;
                        ds.Tables.Add(data);
                    }
                    i++;
                    // ds = VAdvantage.DataBase.DB.SetUtcDateTime(ds);
                }

                else //Paging
                {
                    ds = VIS.DBase.DB.ExecuteDatasetPaging(sql, sqlIn.page, sqlIn.pageSize);
                }

            }


            if (ds == null || ds.Tables.Count < 1)
                return null;




            ds = VAdvantage.DataBase.DB.SetUtcDateTime(ds);

            return ds;

        }

        public DataSet ExecuteDataSet(string sql)
        {

            //string trxName = null;

            DataSet ds = null;


            ds = VIS.DBase.DB.ExecuteDataset(sql, null);

            if (ds == null || ds.Tables.Count < 1)
                return null;

            ds = VAdvantage.DataBase.DB.SetUtcDateTime(ds);

            return ds;

        }


        public List<int> ExecuteNonQueries(string sql, List<List<SqlParams>> param)
        {
            List<int> result = new List<int>();

            string[] queries = sql.Split('/');



            for (int i = 0; i < queries.Count(); i++)
            {
                //if (!CanDeletingADValue(queries[i]))
                //{
                //    result.Add(-1);
                //    return result;
                //}

                //if (!IsTableAccessForExecute(queries[i]))
                //{
                //    result.Add(-1);
                //    return result;
                //}

                if (IsChangingSystemSettings(queries[i]))
                {
                    result.Add(-1);
                    return result;
                }

                SqlParams[] paramIn = (param == null || param.Count < (i + 1) || param[i] == null) ? null : param[i].ToArray();

                result.Add(VIS.DBase.DB.ExecuteQuery(queries[i], paramIn, null));
            }
            return result;
        }

        public int ExecuteNonQuery(SqlParamsIn sqlIn)
        {
            SqlParams[] paramIn = sqlIn.param == null ? null : sqlIn.param.ToArray();

            string sql = sqlIn.sql;

            //if (!CanDeletingADValue(sql))
            //{
            //    return -1;
            //}

            //string sql = sqlIn.sql;
            //if (!IsTableAccessForExecute(sql))
            //    return -1;

            if (IsChangingSystemSettings(sql))
                return -1;

            return VIS.DBase.DB.ExecuteQuery(sqlIn.sql, paramIn, null);
        }

        private bool CanDeletingADValue(string sql)
        {
            if (sql.ToUpper().IndexOf("DELETE") > -1)
            {
                sql = sql.Substring(sql.ToUpper().IndexOf("FROM") + 5).Split(' ')[0];
                if (sql.Trim().StartsWith("AD_"))
                {
                    return false;
                }

            }
            return true;
        }


        private bool IsChangingSystemSettings(string sql)
        {
            sql = sql.ToUpper();

            if (sql.IndexOf("ALTER") > -1 || sql.IndexOf("DROP") > -1 || sql.IndexOf("TRUNCATE") > -1)
            {
                return true;
            }

            return false;
        }

        List<string> tables = new List<string>();
        private List<string> parseQuery(string sql)
        {
            if (sql.IndexOf('~') > -1)
            {
                string[] sqls = sql.Split('~');
                for (int k = 0; k < sqls.Length; k++)
                {
                    parseQuery(sqls[k]);
                }
            }


            tables.Add(sql.Substring(sql.ToUpper().IndexOf(" FROM ") + 5).Split(' ')[0]);
            sql = sql.Substring(sql.ToUpper().IndexOf(" FROM ") + 5);
            if (sql.ToUpper().IndexOf("JOIN") > -1)
            {
                string[] stringSeparators = new string[] { "JOIN" };
                string[] splits = sql.Split(stringSeparators, StringSplitOptions.None);

                for (int i = 0; i < splits.Length; i++)
                {
                    var tab = splits[i].Trim();
                    if (tab.Length > 0)
                    {
                        if (tab.IndexOf(' ') > -1)
                        {
                            string table1 = tab.Substring(0, tab.IndexOf(' '));
                            if (tables.IndexOf(table1) == -1 && table1.Length > 0)
                            {
                                tables.Add(table1);
                            }
                        }
                        else
                        {
                            string table1 = tab.Trim();
                            if (tables.IndexOf(table1) == -1 && table1.Length > 0)
                            {
                                tables.Add(table1);
                            }
                        }
                    }
                }
            }


            if (sql.ToUpper().IndexOf(",") > -1)
            {
                string[] splits = sql.Split(',');

                for (int i = 0; i < splits.Length; i++)
                {
                    var tab = splits[i].Trim();
                    if (tab.Length > 0)
                    {
                        if (tab.IndexOf(' ') > -1)
                        {
                            string table1 = tab.Substring(0, tab.IndexOf(' '));
                            if (tables.IndexOf(table1) == -1 && table1.Length > 0)
                            {
                                tables.Add(table1);
                            }
                        }
                        else
                        {
                            string table1 = tab.Trim();
                            if (tables.IndexOf(table1) == -1 && table1.Length > 0)
                            {
                                tables.Add(table1);
                            }
                        }
                    }
                }
            }

            return tables;
        }

        private bool IsTableAccess(string sql)
        {
            if (sql.ToUpper().IndexOf("WHERE") > -1)
            {
                sql = sql.Substring(0, sql.ToUpper().IndexOf("WHERE"));
            }

            List<string> tables = parseQuery(sql);
            for (int i = 0; i < tables.Count; i++)
            {
                string tableName = tables[i];

                if (string.IsNullOrEmpty(tableName) || tableName.Trim().StartsWith("AD_"))
                {
                    continue;
                }
                int tableID = MTable.Get_Table_ID(tableName);
                if (tableID <= 0)
                {
                    return false;
                }
                if (!MRole.GetDefault(_ctx).IsTableAccess(tableID, false))
                    return false;
            }
            return true;
        }

        private bool IsTableAccessForExecute(string sql)
        {
            sql = sql.Trim();
            string[] sqlArray;
            if (sql.ToUpper().StartsWith("UPDATE") || sql.ToUpper().StartsWith("DELETE"))
            {
                sqlArray = sql.Split(' ');
            }
            else
            {
                sqlArray = new string[0];
            }

            if (sqlArray != null && sqlArray.Length > 0)
            {
                string tableName = sqlArray[1];
                if (!string.IsNullOrEmpty(tableName))
                {
                    int tableID = MTable.Get_Table_ID(tableName);
                    if (tableID <= 0)
                    {
                        return false;
                    }
                    if (!MRole.GetDefault(_ctx).IsTableAccess(tableID, false))
                        return false;
                }
            }

            return true;
        }

    }
}