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

            //Get value from System Config for key SYSTEM_NATIVE_SEQUENCE
            String sql = "SELECT Value FROM AD_SysConfig"
                      + " WHERE Name='SYSTEM_NATIVE_SEQUENCE' AND AD_Client_ID IN (0) AND AD_Org_ID IN (0) AND IsActive='Y'"
                      + " ORDER BY AD_Client_ID DESC, AD_Org_ID DESC";

            object result= DB.ExecuteScalar(sql);
            if (result == null || result == DBNull.Value)
            {
                throw new Exception(Msg.GetMsg(GetCtx(), "KeyNotFound"));
            }


            // If already activated, then return.
            
            if (MSysConfig.IsNativeSequence(false))
            {
                throw new Exception(Msg.GetMsg(GetCtx(), "NativeSequenceActived"));
            }

            SetSystemNativeSequence(true);
            bool ok = false;
            try
            {
                CreateSequence("AD_Sequence", Get_TrxName());
                CreateSequence("AD_Issue", Get_TrxName());
                CreateSequence("AD_ChangeLog", Get_TrxName());
                
                sql = "SELECT AD_Table_ID FROM AD_Table WHERE TableName NOT IN ('AD_Sequence', 'AD_Issue', 'AD_ChangeLog') Order BY Upper(TableName)";
                DataSet ds = DB.ExecuteDataset(sql);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        MTable table = new MTable(GetCtx(), Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Table_ID"]), null);
                        CreateSequence(table, Get_TrxName());
                    }

                }

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


        /// <summary>
        /// Create Sequence for given table.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="trxName"></param>
        private void CreateSequence(MTable table, Trx trxName)
        {
            if (!table.IsView())
            {
                if (!MSequence.CreateTableSequence(GetCtx(), table.GetTableName(), trxName))
                {
                    //throw new Exception("Can not create Native Sequence for table " + table.GetTableName());
                    this.AddLog("Cannot create Native Sequence for table : " + table.GetTableName());
                }
                else
                {
                    this.AddLog("Created Native Sequence for : " + table.GetTableName());
                }
            }
        }
        private void CreateSequence(String tableName, Trx trxName)
        {
            CreateSequence(MTable.Get(GetCtx(), tableName), trxName);
        }

        /// <summary>
        /// Change value of -SYSTEM_NATIVE_SEQUENCE in System config to Y or N. and Update MSysConfig class
        /// </summary>
        /// <param name="value"></param>
        private void SetSystemNativeSequence(bool value)
        {
            if (value)
            {
                DB.ExecuteQuery("UPDATE AD_SysConfig SET Value='Y' WHERE Name='SYSTEM_NATIVE_SEQUENCE'");
                MSysConfig.IsNativeSequence(true);
            }
            else
            {
                DB.ExecuteQuery("UPDATE AD_SysConfig SET Value='N' WHERE Name='SYSTEM_NATIVE_SEQUENCE'");
                MSysConfig.IsNativeSequence(true);
            }
        }
    }
}
