﻿/********************************************************
 * Module Name    : Process
 * Purpose        : synchronise column (update columns information phisically in database)
 * Chronological Development
 * Kiran Sangwan     20-March-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Common;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.SqlExec;
using VAdvantage.Utility;

using VAdvantage.ProcessEngine;
using VAdvantage.Logging;
namespace VAdvantage.Process
{
    public class ColumnSync : ProcessEngine.SvrProcess
    {
        // The Column				
        private int p_AD_Column_ID = 0;

        /// <summary>
        /// function to et parameters
        /// </summary>
        /// <returns>int</returns>
        /// 
        override protected void Prepare()
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
            p_AD_Column_ID = GetRecord_ID();
        }//	prepare


        /// <summary>
        /// Alert table 
        /// </summary>
        /// <returns>int</returns>
        /// 
        override protected string DoIt()
        {

            string exception = "";
            log.Info("C_Column_ID=" + p_AD_Column_ID);
            if (p_AD_Column_ID == 0)
            {
                //    return "";
                throw new Exception("@No@ @AD_Column_ID@");
            }
            //IDbTransaction trx = ExecuteQuery.GerServerTransaction();
            MColumn column = new MColumn(GetCtx(), p_AD_Column_ID, Get_Trx());
            if (column.Get_ID() == 0)
            {
                throw new Exception("@NotFound@ @AD_Column_ID@" + p_AD_Column_ID);
            }

            MTable table = MTable.Get(GetCtx(), column.GetAD_Table_ID());
            if (table.Get_ID() == 0)
            {
                throw new Exception("@NotFound@ @AD_Table_ID@" + column.GetAD_Table_ID());
            }
            //	Find Column in Database

            DatabaseMetaData md = new DatabaseMetaData();
            String catalog = "";
            String schema = DataBase.DB.GetSchema();

            //get table name
            string tableName = table.GetTableName();


            int noColumns;
            string sql = null;
            //get columns of a table
            DataSet dt = md.GetColumns(catalog, schema, tableName);
            md.Dispose();
            //for each column
            for (noColumns = 0; noColumns < dt.Tables[0].Rows.Count; noColumns++)
            {
                string columnName = dt.Tables[0].Rows[noColumns]["COLUMN_NAME"].ToString().ToLower();
                if (!columnName.Equals(column.GetColumnName().ToLower()))
                    continue;

                //check if column is null or not

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
                if (dt.Tables[0].Rows[noColumns][dtColumnName].ToString() == value)
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
                    sql = column.GetSQLModify(table, column.IsMandatory() != notNull);
                    noColumns++;
                    break;
                }

            }
            dt = null;

            //while (rs.next())
            //{
            //    noColumns++;
            //    String columnName = rs.getString ("COLUMN_NAME");
            //    if (!columnName.equalsIgnoreCase(column.getColumnName()))
            //        continue;

            //    //	update existing column
            //    boolean notNull = DatabaseMetaData.columnNoNulls == rs.getInt("NULLABLE");
            //    if (column.isVirtualColumn())
            //        sql = "ALTER TABLE " + table.getTableName() 
            //            + " DROP COLUMN " + columnName;
            //    else
            //        sql = column.getSQLModify(table, column.isMandatory() != notNull);
            //    break;
            //}
            //rs.close();
            //rs = null;

            //	No Table
            if (noColumns == 0)
            {
                sql = table.GetSQLCreate();
            }
            //	No existing column
            else if (sql == null)
            {
                if (column.IsVirtualColumn())
                {
                    return "@IsVirtualColumn@";
                }
                sql = column.GetSQLAdd(table);
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

            if (no == -1)
            {
                string msg = "@Error@ ";
                ValueNamePair pp = VAdvantage.Logging.VLogger.RetrieveError();
                if (pp != null)
                    msg += exception + " - ";
                msg += sql;
                throw new Exception(msg);
            }
            string r = createFK();
            return sql + "; " + r;
        }	//	doIt

        public void SetAD_Column_ID(int AD_Column_ID)
        {
            p_AD_Column_ID = AD_Column_ID;
        }

        DataSet dsMetaData = null;
        public String createFK()
        {
            String returnMessage = "";
            if (p_AD_Column_ID == 0)
                throw new Exception("@No@ @AD_Column_ID@");
            MColumn column = new MColumn(GetCtx(), p_AD_Column_ID, Get_Trx());
            if (column.Get_ID() == 0)
                throw new Exception("@NotFound@ @AD_Column_ID@ " + p_AD_Column_ID);

            MTable table = MTable.Get(GetCtx(), column.GetAD_Table_ID());
            if (table.Get_ID() == 0)
                throw new Exception("@NotFound@ @AD_Table_ID@ " + column.GetAD_Table_ID());

            String fk;
            if ((column.GetAD_Reference_ID() == DisplayType.Account)
                && !(column.GetColumnName().Equals("C_ValidCombination_ID", StringComparison.OrdinalIgnoreCase)))
            {
                fk = "SELECT t.TableName, c.ColumnName, c.AD_Column_ID,"
                    + " cPK.AD_Column_ID, tPK.TableName, cPK.ColumnName, c.ConstraintType,"
                    + " 'FK' || t.AD_Table_ID || '_' || c.AD_Column_ID AS ConstraintName "
                    + "FROM AD_Table t"
                    + " INNER JOIN AD_Column c ON (t.AD_Table_ID=c.AD_Table_ID)"
                    + " INNER JOIN AD_Column cPK ON (cPK.AD_Column_ID=1014)"
                    + " INNER JOIN AD_Table tPK ON (cPK.AD_Table_ID=tPK.AD_Table_ID) "
                    + "WHERE c.IsKey='N' AND c.AD_Reference_ID=25 AND C.AD_Column_ID= @param"	//	Acct
                    + " AND c.ColumnName<>'C_ValidCombination_ID'"
                    + " AND t.IsView='N' "
                    + " ORDER BY t.TableName, c.ColumnName";
            }
            else if ((column.GetAD_Reference_ID() == DisplayType.PAttribute)
                && !(column.GetColumnName().Equals("C_ValidCombination_ID", StringComparison.OrdinalIgnoreCase)))
            {
                fk = "SELECT t.TableName, c.ColumnName, c.AD_Column_ID,"
                    + " cPK.AD_Column_ID, tPK.TableName, cPK.ColumnName, c.ConstraintType,"
                    + " 'FK' || t.AD_Table_ID || '_' || c.AD_Column_ID AS ConstraintName "
                    + "FROM AD_Table t"
                    + " INNER JOIN AD_Column c ON (t.AD_Table_ID=c.AD_Table_ID)"
                    + " INNER JOIN AD_Column cPK ON (cPK.AD_Column_ID=8472)"
                    + " INNER JOIN AD_Table tPK ON (cPK.AD_Table_ID=tPK.AD_Table_ID) "
                    + "WHERE c.IsKey='N' AND c.AD_Reference_ID=35 AND C.AD_Column_ID=@param"	//	Product Attribute
                    + " AND c.ColumnName<>'C_ValidCombination_ID'"
                    + " AND t.IsView='N' "
                    + " ORDER BY t.TableName, c.ColumnName";
            }
            else if (((column.GetAD_Reference_ID() == DisplayType.TableDir) || (column.GetAD_Reference_ID() == DisplayType.Search))
                && (column.GetAD_Reference_Value_ID() == 0))
            {
                fk = "SELECT t.TableName, c.ColumnName, c.AD_Column_ID,"
                    + " cPK.AD_Column_ID, tPK.TableName, cPK.ColumnName, c.ConstraintType,"
                    + " 'FK' || t.AD_Table_ID || '_' || c.AD_Column_ID AS ConstraintName "
                    + "FROM AD_Table t"
                    + " INNER JOIN AD_Column c ON (t.AD_Table_ID=c.AD_Table_ID)"
                    + " INNER JOIN AD_Column cPK ON (c.AD_Element_ID=cPK.AD_Element_ID AND cPK.IsKey='Y')"
                    + " INNER JOIN AD_Table tPK ON (cPK.AD_Table_ID=tPK.AD_Table_ID AND tPK.IsView='N') "
                    + "WHERE c.IsKey='N' AND c.AD_Reference_Value_ID IS NULL AND C.AD_Column_ID=@param"
                    + " AND t.IsView='N' AND c.ColumnSQL IS NULL "
                    + " ORDER BY t.TableName, c.ColumnName";
            }
            else //	Table
            {
                fk = "SELECT t.TableName, c.ColumnName, c.AD_Column_ID,"
                    + " cPK.AD_Column_ID, tPK.TableName, cPK.ColumnName, c.ConstraintType,"
                    + " 'FK' || t.AD_Table_ID || '_' || c.AD_Column_ID AS ConstraintName "
                    + "FROM AD_Table t"
                    + " INNER JOIN AD_Column c ON (t.AD_Table_ID=c.AD_Table_ID AND c.AD_Reference_Value_ID IS NOT NULL)"
                    + " INNER JOIN AD_Ref_Table rt ON (c.AD_Reference_Value_ID=rt.AD_Reference_ID)"
                    + " INNER JOIN AD_Column cPK ON (rt.Column_Key_ID=cPK.AD_Column_ID)"
                    + " INNER JOIN AD_Table tPK ON (cPK.AD_Table_ID=tPK.AD_Table_ID) "
                    + "WHERE c.IsKey='N'"
                    + " AND t.IsView='N' AND c.ColumnSQL IS NULL AND C.AD_Column_ID=@param"
                    + " ORDER BY t.TableName, c.ColumnName";
            }

            SqlParameter[] pstmt = null;

            try
            {
                /*Find foreign key relation in Database
                 * */
                //Trx trx = Trx.Get("getDatabaseMetaData");
                DatabaseMetaData md = new DatabaseMetaData();
                String catalog = "";// DB.getCatalog();
                String schema = DB.GetSchema();
                String tableName = table.GetTableName();




                String dropsql = null;
                int no = 0;

                String constraintNameDB = null;
                String PKTableNameDB = null;
                String PKColumnNameDB = null;
                int constraintTypeDB = 0;

                /* Get foreign key information from DatabaseMetadata
                 * */
                if (dsMetaData == null)
                {
                    dsMetaData = md.GetForeignKeys(catalog, schema, tableName);
                }
                if (dsMetaData != null && dsMetaData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsMetaData.Tables[0].Rows.Count; i++)
                    {

                        Dictionary<String, String> fkcolumnDetail = md.GetForeignColumnDetail(dsMetaData.Tables[0].Rows[i]);
                        // string sql = "SELECT column_name FROM user_cons_columns WHERE constraint_name='" + ds.Tables[0].Rows[i]["FOREIGN_KEY_CONSTRAINT_NAME"].ToString() + "'";
                        //string fkcolumnName = Util.GetValueOfString(DB.ExecuteScalar(sql));

                        if (fkcolumnDetail["FK_Column_Name"].Equals(column.GetColumnName(), StringComparison.OrdinalIgnoreCase))
                        {
                            constraintNameDB = fkcolumnDetail["ConstraintNameDB"];
                            PKTableNameDB = fkcolumnDetail["PK_Table_Name"]; //rs.GetString("PKTABLE_NAME");
                            PKColumnNameDB = fkcolumnDetail["PK_Column_Name"]; //rs.GetString("PKCOLUMN_NAME");
                            constraintTypeDB = md.GetConstraintTypeDB(fkcolumnDetail["Delete_Rule"]); //rs.getShort("DELETE_RULE");
                            break;
                        }
                    }
                }


                pstmt = new SqlParameter[1];
                pstmt[0] = new SqlParameter("@param", column.Get_ID());

                DataSet ds = DB.ExecuteDataset(fk, pstmt, Get_Trx());

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    String TableName = ds.Tables[0].Rows[0]["TableName"].ToString();
                    String ColumnName = ds.Tables[0].Rows[0]["ColumnName"].ToString();
                    //	int AD_Column_ID = rs.getInt (3);
                    //	int PK_Column_ID = rs.getInt (4);
                    String PKTableName = ds.Tables[0].Rows[0]["TableName1"].ToString();
                    String PKColumnName = ds.Tables[0].Rows[0]["ColumnName1"].ToString();
                    String ConstraintType = ds.Tables[0].Rows[0]["ConstraintType"].ToString();
                    String ConstraintName = ds.Tables[0].Rows[0]["ConstraintName"].ToString();

                    /* verify if the constraint in DB is different than the one to be created */
                    Boolean modified = true;
                    if (constraintNameDB != null)
                    {
                        if (((constraintNameDB.Equals(ConstraintName, StringComparison.OrdinalIgnoreCase))
                                && (PKTableNameDB != null) && (PKTableNameDB.Equals(PKTableName, StringComparison.OrdinalIgnoreCase))
                                && (PKColumnNameDB != null) && (PKColumnNameDB.Equals(PKColumnName, StringComparison.OrdinalIgnoreCase))
                                && ((constraintTypeDB == DatabaseMetaData.importedKeyRestrict) &&
                                        (X_AD_Column.CONSTRAINTTYPE_Restrict.Equals(ConstraintType)
                                                || X_AD_Column.CONSTRAINTTYPE_RistrictTrigger.Equals(ConstraintType))))
                                                ||
                                                ((constraintTypeDB == DatabaseMetaData.importedKeyCascade) &&
                                                        (X_AD_Column.CONSTRAINTTYPE_Cascade.Equals(ConstraintType)
                                                                || X_AD_Column.CONSTRAINTTYPE_CascadeTrigger.Equals(ConstraintType)))
                                                                ||
                                                                ((constraintTypeDB == DatabaseMetaData.importedKeySetNull) &&
                                                                        (X_AD_Column.CONSTRAINTTYPE_Null.Equals(ConstraintType)))

                        )
                        {
                            modified = false;
                        }

                        else
                        {
                            dropsql = "ALTER TABLE " + table.GetTableName()
                            + " DROP CONSTRAINT " + constraintNameDB;
                        }
                    }
                    if (modified)
                    {
                        StringBuilder sql = null;
                        try
                        {
                            if (dropsql != null)
                            {
                                /* Drop the existing constraint */
                                //no = DB.executeUpdate(Get_Trx(), dropsql);

                                no = DB.ExecuteQuery(dropsql, null, Get_Trx());
                                AddLog(0, null, Decimal.Parse(no.ToString()), dropsql);
                            }
                            /* Now create the sql foreign key constraint */
                            sql = new StringBuilder("ALTER TABLE ")
                                .Append(TableName)
                                .Append(" ADD CONSTRAINT ").Append(ConstraintName)
                                .Append(" FOREIGN KEY (").Append(ColumnName)
                                .Append(") REFERENCES ").Append(PKTableName)
                                .Append(" (").Append(PKColumnName).Append(")");
                            Boolean createfk = true;
                            if (!String.IsNullOrEmpty(ConstraintType))
                            {
                                if (X_AD_Column.CONSTRAINTTYPE_DoNOTCreate.Equals(ConstraintType))
                                    createfk = false;
                                else if (X_AD_Column.CONSTRAINTTYPE_Restrict.Equals(ConstraintType)
                                    || X_AD_Column.CONSTRAINTTYPE_RistrictTrigger.Equals(ConstraintType))
                                {
                                    ;
                                }
                                else if (X_AD_Column.CONSTRAINTTYPE_Cascade.Equals(ConstraintType)
                                    || X_AD_Column.CONSTRAINTTYPE_CascadeTrigger.Equals(ConstraintType))
                                    sql.Append(" ON DELETE CASCADE");
                                else if (X_AD_Column.CONSTRAINTTYPE_Null.Equals(ConstraintType)
                                    || X_AD_Column.CONSTRAINTTYPE_NullTrigger.Equals(ConstraintType))
                                    sql.Append(" ON DELETE SET NULL");
                            }
                            else
                            {
                                createfk = false;
                            }
                            /* Create the constraint */
                            if (createfk)
                            {
                                // no = DB.ExecuteQuery(sql.ToString(), null, Get_Trx());
                                no = DB.ExecuteQuery(sql.ToString(), null, Get_Trx(), false, true);
                                AddLog(0, null, Decimal.Parse(no.ToString()), sql.ToString());
                                if (no != -1)
                                {
                                    log.Finer(ConstraintName + " - " + TableName + "." + ColumnName);
                                    returnMessage = sql.ToString();
                                }
                                else
                                {
                                    log.Info(ConstraintName + " - " + TableName + "." + ColumnName
                                        + " - ReturnCode=" + no);
                                    returnMessage = "FAILED:" + sql.ToString();
                                }
                            } //if createfk

                        }
                        catch (Exception e)
                        {
                            if (returnMessage.Length > 0)
                            {
                                returnMessage = returnMessage + Msg.GetMsg(GetCtx(), "VIS_ConstraintsError") + e.Message;
                                AddLog(0, null, Decimal.Parse(no.ToString()), ". " + Msg.GetMsg(GetCtx(), "VIS_ConstraintsError") + e.Message);

                            }
                            else
                            {
                                returnMessage = Msg.GetMsg(GetCtx(), "VIS_ConstraintsError") + e.Message;
                                AddLog(0, null, Decimal.Parse(no.ToString()), Msg.GetMsg(GetCtx(), "VIS_ConstraintsError") + e.Message);
                            }


                            log.Log(Level.SEVERE, sql.ToString() + " - " + e.ToString());

                        }
                    } // modified
                }	//	rs.next
                else
                {
                    if (constraintNameDB != null && constraintNameDB.Equals("FK" + column.GetAD_Table_ID() + "_" + p_AD_Column_ID, StringComparison.OrdinalIgnoreCase))
                    {
                        dropsql = "ALTER TABLE " + table.GetTableName()
                        + " DROP CONSTRAINT " + constraintNameDB;

                        /* Drop the existing constraint */
                        no = DB.ExecuteQuery(dropsql, null, Get_Trx());
                        AddLog(0, null, Decimal.Parse(no.ToString()), dropsql);
                        returnMessage = dropsql.ToString();
                    }
                }
            }
            catch (Exception e)
            {
                if (returnMessage.Length > 0)
                {
                    returnMessage = returnMessage + ". " + Msg.GetMsg(GetCtx(), "VIS_ConstraintsError") + e.Message;
                    AddLog(0, null, Decimal.Parse("0"), ". " + Msg.GetMsg(GetCtx(), "VIS_ConstraintsError") + e.Message);
                }
                else
                {
                    returnMessage = Msg.GetMsg(GetCtx(), "VIS_ConstraintsError") + e.Message;
                    AddLog(0, null, Decimal.Parse("0"), Msg.GetMsg(GetCtx(), "VIS_ConstraintsError") + e.Message);
                }
                log.Log(Level.SEVERE, fk, e);
            }
            return returnMessage;



        }

   

    }
}
