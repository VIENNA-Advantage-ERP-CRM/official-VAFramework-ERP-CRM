using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
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
                SqlParams[] paramIn = (param == null || param.Count < (i + 1) || param[i] == null) ? null : param[i].ToArray();

                result.Add(VIS.DBase.DB.ExecuteQuery(queries[i], paramIn, null));
            }
            return result;
        }

        public int ExecuteNonQuery(SqlParamsIn sqlIn)
        {
            SqlParams[] paramIn = sqlIn.param == null ? null : sqlIn.param.ToArray();
            return VIS.DBase.DB.ExecuteQuery(sqlIn.sql, paramIn, null);
        }


    }
}