/********************************************************
 * Project Name   : ViennaAdvantage
 * Class Name     : MasterVersionSchedules
 * Class Used     : SvrProcess
 * Chronological    Development
 * Lokesh Chauhan   13-Nov-2019
  ******************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;

namespace VAdvantage.Process
{
    public class MasterVersionSchedules : SvrProcess
    {
        #region Private Variables
        StringBuilder sbSQL = new StringBuilder("");
        StringBuilder msg = new StringBuilder("");
        DataSet ds = null;
        List<string> defcolNames = new List<string>();
        List<string> _tblKeysProcessed = new List<string>();
        StringBuilder whereCond = new StringBuilder("");
        int count = 0;
        #endregion Private Variables

        /// <summary>
        /// Process Parameters
        /// </summary>
        protected override void Prepare()
        {
            GetSkipColumns();
        }

        /// <summary>
        /// Process Logic
        /// </summary>
        /// <returns>String Message</returns>
        protected override string DoIt()
        {
            sbSQL.Clear();
            // Query to fetch all tables which ends with "_Ver"
            sbSQL.Append("SELECT AD_Table_ID, TableName FROM AD_Table WHERE LOWER(TableName) LIKE '%_ver'");
            ds = DB.ExecuteDataset(sbSQL.ToString(), null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                StringBuilder sbTblName = new StringBuilder("");
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    sbTblName.Clear();
                    // Table Name of Version Table
                    sbTblName.Append(Util.GetValueOfString(ds.Tables[0].Rows[i]["TableName"]));
                    // Table ID of Version Table
                    int VerTableID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Table_ID"]);

                    // Check if table name ending with "_Ver" has Column "VersionValidFrom" else move to next table
                    int columnCount = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT AD_Column_ID FROM AD_Column WHERE AD_Table_ID = (SELECT AD_Table_ID FROM AD_Table WHERE 
                                                                            LOWER(TableName) = '" + sbTblName.ToString().Trim().ToLower() + "') AND ColumnName = 'VersionValidFrom'", null, null));
                    if (columnCount > 0)
                    {
                        log.Info("Processing for " + sbTblName.ToString());
                        // Select all data from "_Version" Table which needs to be updated on Master Table
                        // only those records which are approved and Not Processed by Schedular and Valid from or effective from Today or before Today's date
                        DataSet dsApprovedData = DB.ExecuteDataset(@"SELECT * FROM " + sbTblName.ToString() +
                            " WHERE IsVersionApproved = 'Y' AND ProcessedVersion = 'N' " +
                            "AND VersionValidFrom <= SYSDATE AND VersionLog IS NULL ORDER BY VersionValidFrom DESC, RecordVersion DESC");
                        if (dsApprovedData != null && dsApprovedData.Tables[0].Rows.Count > 0)
                        {
                            log.Info("Processing for " + sbTblName.ToString() + " ==>> Found unprocessed records " + dsApprovedData.Tables[0].Rows.Count);
                            bool retRes = UpdateRecords(dsApprovedData, sbTblName.ToString(), VerTableID);
                        }
                    }
                }
            }

            if (msg.ToString() == "")
                msg.Append(Msg.GetMsg(GetCtx(), "ProcessedSuccess"));

            return msg.ToString();
        }

        /// <summary>
        /// Function to update data in Master Table based on the versions saved
        /// </summary>
        /// <param name="dsRec">All Records which need to be updated</param>
        /// <param name="TableName">Version Table Name</param>
        /// <param name="VerTableID">Version Table ID</param>
        /// <returns>True/False based on the Updation of data in parent table</returns>
        private bool UpdateRecords(DataSet dsRec, string TableName, int VerTableID)
        {
            // Get Master table name from Version table
            string BaseTblName = TableName;
            if (TableName.EndsWith("_Ver"))
            {
                BaseTblName = BaseTblName.Substring(0, TableName.Length - 4);
            }
            // Master Table ID from Table name
            int BaseTableID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_Table_ID FROM AD_Table WHERE TableName = '" + BaseTblName + "'"));

            // Get Column information of Master Table
            DataSet dsDBColNames = DB.ExecuteDataset("SELECT ColumnName, AD_Reference_ID, IsUpdateable, IsAlwaysUpdateable FROM AD_Column WHERE AD_Table_ID = " + BaseTableID);

            if (dsDBColNames != null && dsDBColNames.Tables[0].Rows.Count > 0)
            {
                log.Info("Processing for " + TableName + " :: ");

                StringBuilder sqlSB = new StringBuilder("");
                bool recordProcessed = false;
                // create object of Master Table
                MTable tbl = new MTable(GetCtx(), BaseTableID, null);
                // create object of Version table
                MTable tblVer = new MTable(GetCtx(), VerTableID, null);

                // check whether master table has Single key
                bool isSingleKey = tbl.IsSingleKey();

                // List of records which were not processed by process in case of any error
                List<string> keys = new List<string>();
                StringBuilder sbKey = new StringBuilder("");
                // Loop through the records which need to be updated on Master Table
                for (int i = 0; i < dsRec.Tables[0].Rows.Count; i++)
                {
                    whereCond.Clear();
                    DataRow dr = dsRec.Tables[0].Rows[i];
                    sbKey.Clear();
                    PO poDest = null;
                    PO poSource = tblVer.GetPO(GetCtx(), dr, null);
                    // if table has single key
                    if (isSingleKey)
                    {
                        // Create object of PO Class from TableName and Record ID
                        poDest = MTable.GetPO(GetCtx(), BaseTblName, Util.GetValueOfInt(dr[BaseTblName + "_ID"]), null);
                        sbKey.Append(Util.GetValueOfInt(dr[BaseTblName + "_ID"]));
                        whereCond.Append(BaseTblName + "_ID = " + Util.GetValueOfInt(dr[BaseTblName + "_ID"]));
                    }
                    else
                    {
                        // Create object of PO Class from combination of key columns
                        string[] keyCols = tbl.GetKeyColumns();
                        for (int w = 0; w < keyCols.Length; w++)
                        {
                            if (w == 0)
                            {
                                if (keyCols[w] != null)
                                    whereCond.Append(keyCols[w] + " = " + poSource.Get_Value(keyCols[w]));
                                else
                                    whereCond.Append(" NVL(" + keyCols[w] + ",0) = 0");
                            }
                            else
                            {
                                if (keyCols[w] != null)
                                    whereCond.Append(" AND " + keyCols[w] + " = " + poSource.Get_Value(keyCols[w]));
                                else
                                    whereCond.Append(" AND NVL(" + keyCols[w] + ",0) = 0");
                            }
                        }
                        poDest = tbl.GetPO(GetCtx(), whereCond.ToString(), null);
                        sbKey.Append(whereCond);
                    }

                    if (_tblKeysProcessed.Contains(sbKey.ToString()))
                        continue;

                    // check if there is any error in processing record, then continue and do not process next versions
                    if (keys.Contains(sbKey.ToString()))
                        continue;

                    // Check whether Master Table contains "Processing" Column
                    if (poDest.Get_ColumnIndex("Processing") >= 0)
                    {
                        // if "Processing" column found then return, do not update in Master Table, 
                        // because transaction might be in approval process
                        if (Util.GetValueOfString(poSource.Get_Value("Processing")) == "Y")
                        {
                            keys.Add(sbKey.ToString());
                            continue;
                        }
                    }

                    // Check whether Master Table contains "Processed" Column
                    if (poDest.Get_ColumnIndex("Processed") >= 0)
                    {
                        if (Util.GetValueOfString(poSource.Get_Value("Processed")) == "Y")
                            recordProcessed = true;
                    }

                    // Change done to check for back date versions
                    sqlSB.Clear().Append("SELECT COUNT(*) FROM " + TableName + " WHERE IsVersionApproved = 'Y'" +
                            " AND TRUNC(VersionValidFrom) > " + GlobalVariable.TO_DATE(Util.GetValueOfDateTime(dr["VersionValidFrom"]), true)
                            + " AND " + GlobalVariable.TO_DATE(Util.GetValueOfDateTime(dr["VersionValidFrom"]), true) + " <= TRUNC(SysDate)"
                            + " AND " + whereCond);

                    int CountRecs = Util.GetValueOfInt(DB.ExecuteScalar(sqlSB.ToString(), null, null));
                    if (CountRecs > 0)
                    {
                        sqlSB.Clear().Append("UPDATE " + TableName + " SET ProcessedVersion = 'Y' WHERE ProcessedVersion = 'N' AND IsVersionApproved = 'Y'" +
                            " AND TRUNC(VersionValidFrom) <= " + GlobalVariable.TO_DATE(Util.GetValueOfDateTime(dr["VersionValidFrom"]), true) + " AND " + whereCond);
                        count = Util.GetValueOfInt(DB.ExecuteQuery(sqlSB.ToString(), null, null));
                        if (!(isSingleKey && Util.GetValueOfInt(sbKey.ToString()) <= 0))
                            _tblKeysProcessed.Add(sbKey.ToString());
                        continue;
                    }

                    // set client and Organization ID from Version table to Master
                    // as copy PO set these ID's as 0
                    poDest.SetAD_Client_ID(poSource.GetAD_Client_ID());
                    poDest.SetAD_Org_ID(poSource.GetAD_Org_ID());

                    StringBuilder sbColName = new StringBuilder("");
                    // Loop through all the columns in Master Table
                    for (int j = 0; j < dsDBColNames.Tables[0].Rows.Count; j++)
                    {
                        sbColName.Clear();
                        // Get Name of Column
                        sbColName.Append(dsDBColNames.Tables[0].Rows[j]["ColumnName"]);

                        // check if column exist in Default columns list, in that case do not update and continue to next column
                        if (defcolNames.Contains(sbColName.ToString()))
                            continue;
                        // No need to update Primary key column, continue to next column
                        if (sbColName.ToString().Equals(BaseTblName + "_ID"))
                            continue;
                        // if column is of "Yes-No" type i.e. Reference ID is 20 (Fixed) then set True/False
                        if (Util.GetValueOfInt(dsDBColNames.Tables[0].Rows[j]["AD_Reference_ID"]) == 20)
                        {
                            Object val = false;
                            if (poSource.Get_Value(sbColName.ToString()) != null)
                                val = poSource.Get_Value(sbColName.ToString());
                            poDest.Set_Value(sbColName.ToString(), val);
                        }
                        else
                            poDest.Set_ValueNoCheck(sbColName.ToString(), poSource.Get_Value(sbColName.ToString()));

                        // Check if Master record is Processed and Always Updatable is false then check whether any value updated in such column
                        // if value updated then return false, can't change data in Processed record if it's not Always Updatable
                        if (recordProcessed && Util.GetValueOfString(dsDBColNames.Tables[0].Rows[j]["IsAlwaysUpdateable"]) == "N")
                        {
                            bool upd = poDest.Is_ValueChanged(sbColName.ToString());
                            if (upd)
                            {
                                msg.Append("Can't update  " + sbColName.ToString() + " in " + TableName);
                                log.SaveError("ERROR", "Can not update processed record for column ==>> " + sbColName.ToString());
                                // Add record to the list of unprocessed records
                                keys.Add(sbKey.ToString());
                                continue;
                            }
                        }
                    }

                    // Save Master Record
                    if (!poDest.Save())
                    {
                        // Add record to the list of unprocessed records
                        keys.Add(sbKey.ToString());
                        // Check for Errors
                        ValueNamePair vnp = VLogger.RetrieveError();
                        string error = "";
                        if (vnp != null)
                        {
                            error = vnp.GetName();
                            if (error == "" && vnp.GetValue() != null)
                                error = vnp.GetValue();
                        }
                        if (error == "")
                            error = "Error in updating Version";

                        msg.Append("Save error in " + TableName + " ==>> " + error);
                        log.SaveError("ERROR", error);

                        sqlSB.Clear();

                        sqlSB.Append("UPDATE " + TableName + " SET VersionLog = '" + error + "' WHERE " + TableName + "_ID = " + dr[TableName + "_ID"]);
                        count = DB.ExecuteQuery(sqlSB.ToString(), null, null);

                        continue;
                    }
                    else
                    {
                        // Update Version table, Set "ProcessedVersion" to "Y", so that it don't consider when process runs next time
                        sqlSB.Clear();
                        // Update against record id in case of Single key, and update Key column in version table as well in case of new record
                        //if (isSingleKey)
                        //    sqlSB.Append("UPDATE " + TableName + " SET ProcessedVersion = 'Y', " + BaseTblName + "_ID  = " + poDest.Get_ID() + " WHERE " + TableName + "_ID = " + dr[TableName + "_ID"]);
                        //// else 
                        //else
                        //    sqlSB.Append("UPDATE " + TableName + " SET ProcessedVersion = 'Y' WHERE " + TableName + "_ID = " + dr[TableName + "_ID"]);

                        //int count = DB.ExecuteQuery(sqlSB.ToString(), null, null);
                        //if (count <= 0)
                        //    log.Info(TableName + " not updated ==>> " + sqlSB.ToString());

                        if (isSingleKey && Util.GetValueOfInt(sbKey.ToString()) <= 0)
                        {
                            sqlSB.Append("UPDATE " + TableName + " SET ProcessedVersion = 'Y', " + BaseTblName + "_ID  = " + poDest.Get_ID() + " WHERE " + TableName + "_ID = " + dr[TableName + "_ID"]);
                        }
                        else
                        {
                            sqlSB.Clear().Append("UPDATE " + TableName + " SET ProcessedVersion = 'Y' WHERE ProcessedVersion = 'N' AND IsVersionApproved = 'Y'" +
                                " AND TRUNC(VersionValidFrom) <= " + GlobalVariable.TO_DATE(Util.GetValueOfDateTime(dr["VersionValidFrom"]), true) + " AND " + whereCond);
                            _tblKeysProcessed.Add(sbKey.ToString());
                        }
                        count = Util.GetValueOfInt(DB.ExecuteQuery(sqlSB.ToString(), null, null));
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Create List of Columns which will not get updated in Master Table
        /// </summary>
        public void GetSkipColumns()
        {
            defcolNames.Add("AD_Client_ID");
            defcolNames.Add("AD_Org_ID");
            defcolNames.Add("Created");
            defcolNames.Add("CreatedBy");
            defcolNames.Add("UpdatedBy");
            defcolNames.Add("Updated");
            defcolNames.Add("Export_ID");
            defcolNames.Add("Processed");
            defcolNames.Add("Processing");
            defcolNames.Add("IsVersionApproved");
            defcolNames.Add("ProcessedVersion");
            defcolNames.Add("RecordVersion");
            defcolNames.Add("VersionValidFrom");
        }
    }
}
