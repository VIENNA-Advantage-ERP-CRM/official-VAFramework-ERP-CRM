using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;

namespace VAdvantage.Process
{
    public class TableColumnSync : SvrProcess
    {
        private int p_AD_Table_ID = 0;
        List<string> standardColumns = new List<string>();

        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                // if (para[i].getParameter() == null)
                {
                }
                // else
                {
                    // log.log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
            p_AD_Table_ID = GetRecord_ID();

            standardColumns.Add("AD_CLIENT_ID");
            standardColumns.Add("AD_ORG_ID");
            standardColumns.Add("CREATED");
            standardColumns.Add("CREATEDBY");
            standardColumns.Add("EXPORT_ID");
            standardColumns.Add("ISACTIVE");
            standardColumns.Add("UPDATED");
            standardColumns.Add("UPDATEDBY");

        }

        protected override string DoIt()
        {
            // string returnSql = "";
            string exception = "";
            log.Info("AD_Table_ID=" + p_AD_Table_ID);
            if (p_AD_Table_ID == 0)
            {
                //    return "";
                throw new Exception("@No@ @AD_Table_ID@");
            }


            MTable table = MTable.Get(GetCtx(), p_AD_Table_ID);
            if (table.Get_ID() == 0)
            {
                throw new Exception("@NotFound@ @AD_Table_ID@" + p_AD_Table_ID);
            }

            //	Find Column in Database

            DatabaseMetaData md = new DatabaseMetaData();
            String catalog = "";
            String schema = DataBase.DB.GetSchema();

            //get table name
            string tableName = table.GetTableName();
            int noColumns = 0;
            string sql = null;
            //get columns of a table
            DataSet dt = md.GetColumns(catalog, schema, tableName);

            MColumn[] columnsAD = table.GetColumns(true);

            md.Dispose();

            StringBuilder sqlAlter = new StringBuilder("ALTER TABLE " + tableName + " ");
            StringBuilder sqlAdd = new StringBuilder();
            StringBuilder sqlModify = new StringBuilder();
            StringBuilder sqlSetNull = new StringBuilder();
            StringBuilder sqlSetDefault = new StringBuilder();
            StringBuilder sqlUpdate = new StringBuilder();
            StringBuilder constraints = new StringBuilder();
            List<MColumn> cols = new List<MColumn>();
            //if (dt.Tables[0].Rows.Count > 0)
            //{
            //    for (int i = 0; i < columnsAD.Count(); i++)
            //    {
            //        var rows = from row in dt.Tables[0].AsEnumerable()
            //                   where row.Field<string>("COLUMN_NAME") == columnsAD[i].GetColumnName()
            //                   select row;
            //    }
            //}

            if (DatabaseType.IsOracle)
            {
                sqlAdd.Append(" ADD ( ");
                sqlModify.Append(" Modify ( ");
            }


            if (dt.Tables.Count == 0 || dt.Tables[0].Rows.Count == 0)
            {
                sql = table.GetSQLCreate();
            }
            else
            {
                for (int adCol = 0; adCol < columnsAD.Length; adCol++)
                {
                    MColumn column = columnsAD[adCol];

                    string columnName = column.GetColumnName();

                    DataRow[] dr = dt.Tables[0].Select("COLUMN_NAME='" + columnName.ToUpper() + "'");

                    if (dr == null || dr.Length == 0)
                    {
                        if (column.IsVirtualColumn())
                        {
                            continue;
                        }
                        if (DatabaseType.IsOracle)
                        {
                            if (sqlAdd.Length > 7)
                            {
                                sqlAdd.Append(" , " + column.GetSQLDDL());
                            }
                            else
                            {
                                sqlAdd.Append(column.GetSQLDDL());
                            }
                        }
                        else if (DatabaseType.IsPostgre)
                        {
                            if (sqlAdd.Length > 0)
                            {
                                sqlAdd.Append(" , ADD COLUMN " + column.GetSQLDDL());
                            }
                            else
                            {
                                sqlAdd.Append(" ADD COLUMN " + column.GetSQLDDL());
                            }
                        }

                        if (column.IsKey())
                        {
                            constraints.Append(", CONSTRAINT PK").Append(column.GetAD_Table_ID())
                                .Append(" PRIMARY KEY (").Append(column.GetColumnName()).Append(")");
                        }
                    }
                    else
                    {
                        if (standardColumns.IndexOf(columnName.ToUpper()) > -1 || column.IsKey())
                        {
                            continue;
                        }
                        string dtColumnName = "is_nullable";
                        string value = "YES";
                        //if database is oracle
                        if (DatabaseType.IsOracle)
                        {
                            dtColumnName = "NULLABLE";
                            value = "Y";
                        }
                        bool notNull = false;
                        //check if column is null
                        if (dr[0][dtColumnName].ToString() == value)
                            notNull = false;
                        else
                            notNull = true;

                        //............................

                        //if column is virtual column then alter table and drop this column
                        if (column.IsVirtualColumn())
                        {
                            sql = "ALTER TABLE " + table.GetTableName()
                           + " DROP COLUMN " + columnName;
                        }
                        else
                        {
                            if (DatabaseType.IsOracle)
                            {
                                if (sqlModify.Length > 10)
                                {
                                    sqlModify.Append(" , " + column.GetSQLModifyPerColumn(table, columnName, column.IsMandatory() != notNull));
                                }
                                else
                                {
                                    sqlModify.Append(column.GetSQLModifyPerColumn(table, columnName, column.IsMandatory() != notNull));
                                }
                                sqlModify.Append(column.SetDefaultValue(table, columnName));
                            }
                            else if (DatabaseType.IsPostgre)
                            {
                                if (sqlModify.Length > 0)
                                {
                                    sqlModify.Append(" , ALTER COLUMN " + column.GetSQLModifyPerColumn(table, columnName, column.IsMandatory() != notNull));
                                }
                                else
                                {
                                    sqlModify.Append(" ALTER COLUMN " + column.GetSQLModifyPerColumn(table, columnName, column.IsMandatory() != notNull));
                                }
                                sqlSetDefault.Append(column.SetDefaultValue(table, columnName));
                            }

                            string setNUll = column.SetNullOption(column.IsMandatory() != notNull, table, columnName);
                            if (setNUll.Length > 0)
                            {
                                sqlSetNull.Append(";").Append(setNUll);
                            }

                            string updateQuery = column.UpdateDefaultValue(table);
                            if (updateQuery.Length > 0)
                            {
                                if (sqlUpdate.Length == 0)
                                {
                                    sqlUpdate.Append(updateQuery);
                                }
                                else
                                {
                                    sqlUpdate.Append(" , " + updateQuery);
                                }
                            }
                            noColumns++;
                        }
                    }
                }



                if (sqlAdd.Length > 7)
                {
                    sqlAlter.Append(sqlAdd).Append(" ) ");
                }
                if (sqlModify.Length > 10)
                {
                    sqlAlter.Append(sqlModify);
                    if (DatabaseType.IsOracle)
                    {
                        sqlAlter.Append(" ) ");
                    }
                }
                if (sqlSetDefault.Length > 0)
                {
                    sqlAlter.Append(sqlSetDefault);
                }
                if (sqlSetNull.Length > 0)
                {
                    sqlAlter.Append(sqlSetNull);
                }
                if (constraints.Length > 0)
                {
                    sqlAlter.Append(" " + constraints + " ");
                }


                // returnSql = sqlAlter.ToString();


                if (sqlUpdate.Length > 0)
                {
                    sqlAlter.Append("; " + sqlUpdate);
                    //EXECUTE SQLUPDATE
                    // returnSql = returnSql + " " + sqlUpdate.ToString();

                }
                sql = sqlAlter.ToString();

            }

            int no = 0;
            if (sql.IndexOf(";") == -1)
            {
                //no = 
                //ExecuteQuery.ExecuteNonQuery(sql, false, Get_Trx());
                try
                {
                    no = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
                    AddLog(0, DateTime.MinValue, Decimal.Parse(no.ToString()), sql);
                }
                catch (Exception ex)
                {
                    exception = ex.Message;
                }
                //addLog (0, null, new BigDecimal(no), sql);
            }
            else
            {
                //string ss = "; ";
                string[] statements = sql.Split(';');
                for (int i = 0; i < statements.Length; i++)
                {
                    int count = DataBase.DB.ExecuteQuery(statements[i].ToString(), null, Get_Trx());
                    AddLog(0, DateTime.MinValue, Decimal.Parse(count.ToString()), statements[i]);
                    no += count;
                    //ExecuteQuery.ExecuteNonQuery(statements[i].ToString());
                }
            }



            if (dt.Tables.Count > 0 && dt.Tables[0].Rows.Count > 0)
            {
                StringBuilder sqlFK = new StringBuilder();
                ColumnSync sync = new ColumnSync();
                for (int adCol = 0; adCol < columnsAD.Length; adCol++)
                {
                    MColumn column = columnsAD[adCol];

                    sync.SetAD_Column_ID(column.GetAD_Column_ID());
                    sqlFK.Append(sync.createFK());
                }
                sql = sql + sqlFK;
            }

            dt = null;

            return sql;

        }
    }
}
