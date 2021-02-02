using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;

namespace VAdvantage.Process
{
    public class NativeSequence : SvrProcess
    {
        #region Private Variable

        // Table Name
        private object Name = "";

        // Currenct Primary Key Number
        private object CurrentNext = 0;

        // Currenct System ID
        private object CurrentNextSys = 0;

        // Increment By
        private object IncrementNo = 0;

        // Pcol (primary key column of table)
        private object Pcol = 0;

        #endregion

        // Prepare - e.g., get Parameters.
        protected override void Prepare()
        { }


        protected override String DoIt()
        {


            String sql = "SELECT Value FROM AD_SysConfig"
                      + " WHERE Name='SYSTEM_NATIVE_SEQUENCE' AND AD_Client_ID IN (0) AND AD_Org_ID IN (0) AND IsActive='Y'"
                      + " ORDER BY AD_Client_ID DESC, AD_Org_ID DESC";

            object result= DB.ExecuteScalar(sql);
            if (result == null || result == DBNull.Value)
            {
                throw new Exception("Key Not Found");
            }



            bool SYSTEM_NATIVE_SEQUENCE = MSysConfig.GetValue("SYSTEM_NATIVE_SEQUENCE") == "Y";
            if (SYSTEM_NATIVE_SEQUENCE)
            {
                throw new Exception("Native Sequence is Actived");
            }

            if (!DatabaseType.IsOracle)
            {
                throw new Exception("Native Sequence Supported for Oracle only.");
            }

            SetSystemNativeSequence(true);
            bool ok = false;
            try
            {
                CreateSequence("AD_Sequence", null);
                CreateSequence("AD_Issue", null);
                CreateSequence("AD_ChangeLog", null);
                //
                String whereClause = "TableName NOT IN ('AD_Sequence', 'AD_Issue', 'AD_ChangeLog')";
                //List<MTable> tables = new Query(GetCtx(),X_AD_Table.Table_Name, whereClause, Get_TrxName().GetTrxName())
                //    .SetOrderBy("TableName")
                //    .list();
                 sql = "SELECT AD_Table_ID FROM AD_Table WHERE TableName NOT IN ('AD_Sequence', 'AD_Issue', 'AD_ChangeLog') AND IsActive='Y'";
                DataSet ds = DB.ExecuteDataset(sql);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        MTable table = new MTable(GetCtx(), Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Table_ID"]), null);
                        CreateSequence(table, Get_TrxName());
                    }

                }

                //foreach(MTable table in tables)
                //{
                //    CreateSequence(table, Get_TrxName());
                //}
                ok = true;
            }
            finally
            {
                if (!ok)
                {
                    SetSystemNativeSequence(false);
                }
            }

            return "@OK@";
        }

        private void CreateSequence(MTable table, Trx trxName)
        {
            if (!table.IsView())
            {
                if (!MSequence.CreateTableSequence(GetCtx(), table.GetTableName(), trxName,true))
                {
                    //throw new Exception("Can not create Native Sequence for table " + table.GetTableName());
                    this.AddLog("Can not create Native Sequence for table : " + table.GetTableName());
                }
                else
                {
                    this.AddLog("Create Native Sequence for : " + table.GetTableName());
                }
            }
        }
        private void CreateSequence(String tableName, Trx trxName)
        {
            CreateSequence(MTable.Get(GetCtx(), tableName), trxName);
        }

        private void SetSystemNativeSequence(bool value)
        {
            if (value)
            {
                DB.ExecuteQuery("UPDATE AD_SysConfig SET Value='Y' WHERE Name='SYSTEM_NATIVE_SEQUENCE'");
            }
            else
            {
                DB.ExecuteQuery("UPDATE AD_SysConfig SET Value='N' WHERE Name='SYSTEM_NATIVE_SEQUENCE'");
            }
            //new Object[] { value ? "Y" : "N" },
            //null // trxName
            //);
           // MSysConfig.ResetCache();
        }



        //        // Do process
        //        protected override String DoIt()
        //        {
        //            bool tblresult;
        //            int totalTables;
        //            StringBuilder sqlDB;
        //            string msg = "";

        //            PrepareNativeSequenceSql(out tblresult, out totalTables, out sqlDB);
        //            if (tblresult)
        //            {
        //                try
        //                {
        //                    var sqlstring = sqlDB.ToString();
        //                    if (VAdvantage.Utility.Util.GetValueOfInt(DB.ExecuteQuery(sqlDB.ToString())) > -1)
        //                    {
        //                        msg = Msg.GetMsg(GetCtx(), "Done");
        //                    }
        //                }
        //                catch
        //                {
        //                    log.Log(Level.SEVERE, "Problem in process");
        //                    msg = Msg.GetMsg(GetCtx(), "Error");
        //                }
        //            }
        //            else
        //            {
        //                msg = Msg.GetMsg(GetCtx(), "Done");
        //            }
        //            return msg;
        //        }

        //        /// <summary>
        //        /// Get Tables to generate sequence and trigger for BEFORE INSERT
        //        /// </summary>
        //        /// <param name="tblresult">True/False for table to be updated</param>
        //        /// <param name="totalTables">Number of tables get updated</param>
        //        /// <param name="sqlDB">Returns sqldbscript string</param>
        //        public void PrepareNativeSequenceSql(out bool tblresult, out int totalTables, out StringBuilder sqlDB)
        //        {
        //            sqlDB = new StringBuilder();
        //            tblresult = false;
        //            totalTables = 0;
        //            string sql = @"select t1.ad_sequence_id, upper(t1.name) as NAME, t1.currentnext, t1.currentnextsys, t1.incrementno
        //,(SELECT upper(cols.column_name) FROM user_constraints cons, user_cons_columns cols 
        //WHERE cols.table_name = upper(t1.name) AND cons.constraint_type = 'P' AND cons.constraint_name = cols.constraint_name) as PCOL
        //from ad_sequence t1 inner join ad_table t2 on t1.name = t2.name 
        //where t2.isview = 'N' and t1.name not like 'DocumentNo_%' 
        //and not exists (select sequence_name from user_sequences where upper(sequence_name) = concat(upper(t1.name), '_ID_SEQ')) 
        //and upper(t1.name) in 
        //(SELECT upper(cols.table_name) FROM user_constraints cons join user_cons_columns cols 
        //on cons.constraint_name = cols.constraint_name where cons.constraint_type = 'P' and cols.table_name is not null 
        //group by cols.table_name HAVING COUNT(cols.table_name) = 1)";

        //            try
        //            {
        //                DataSet ds = DB.ExecuteDataset(sql);

        //                if (ds == null || ds.Tables[0].Rows.Count == 0)
        //                {
        //                    log.Log(Level.SEVERE, "No Table Found");
        //                    return;
        //                }

        //                sqlDB.Append("BEGIN ");
        //                sqlDB.Append("execute immediate('alter session set \"_optim_peek_user_binds\"=false');");

        //                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //                {
        //                    Name = ds.Tables[0].Rows[i]["NAME"];
        //                    CurrentNext = ds.Tables[0].Rows[i]["CURRENTNEXT"];
        //                    CurrentNextSys = ds.Tables[0].Rows[i]["CURRENTNEXTSYS"];
        //                    IncrementNo = ds.Tables[0].Rows[i]["INCREMENTNO"];
        //                    Pcol = ds.Tables[0].Rows[i]["PCOL"];


        //                    if (Name != DBNull.Value && CurrentNext != DBNull.Value && IncrementNo != DBNull.Value &&
        //                        Name != null && CurrentNext != null && IncrementNo != null)
        //                    {
        //                        sqlDB.Append("execute immediate('");
        //                        sqlDB.Append("CREATE SEQUENCE ");
        //                        sqlDB.Append(Convert.ToString(Name).Trim());
        //                        sqlDB.Append("_NS START WITH ");
        //                        sqlDB.Append(Convert.ToString(CurrentNext).Trim());
        //                        sqlDB.Append(" INCREMENT BY ");
        //                        sqlDB.Append(Convert.ToString(IncrementNo).Trim());
        //                        sqlDB.Append(" NOMAXVALUE");
        //                        sqlDB.Append("');");

        //                        sqlDB.Append("execute immediate('");
        //                        sqlDB.Append("CREATE OR REPLACE TRIGGER ");
        //                        sqlDB.Append(Convert.ToString(Name).Trim());
        //                        sqlDB.Append("_NT BEFORE INSERT ON ");
        //                        sqlDB.Append(Convert.ToString(Name).Trim());
        //                        sqlDB.Append(" FOR EACH ROW BEGIN :new.");
        //                        sqlDB.Append(Convert.ToString(Pcol).Trim());
        //                        sqlDB.Append(" := ");
        //                        sqlDB.Append(Convert.ToString(Name).Trim());
        //                        sqlDB.Append("_NS.NEXTVAL; END;');");

        //                        sqlDB.Append("execute immediate ('update ad_sequence set isnativesequence = ''Y'' where upper(name)=upper(''");
        //                        sqlDB.Append(Convert.ToString(Name).Trim());
        //                        sqlDB.Append("'')');");

        //                        totalTables += 1;
        //                    }
        //                    else
        //                    {
        //                        log.Log(Level.SEVERE, "Problem in Sequence table");
        //                    }
        //                }
        //                sqlDB.Append("execute immediate('alter session set \"_optim_peek_user_binds\"=true');");
        //                sqlDB.Append("END; ");
        //                tblresult = true;
        //            }
        //            catch
        //            {
        //                log.Log(Level.SEVERE, "Problem in process");
        //            }
        //        }
    }
}
