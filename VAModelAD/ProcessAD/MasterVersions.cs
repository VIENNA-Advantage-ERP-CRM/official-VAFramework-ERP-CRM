/********************************************************
 * Project Name   : ViennaAdvantage
 * Class Name     : MasterVersions
 * Class Used     : SvrProcess
 * Chronological    Development
 * Lokesh Chauhan   14-Oct-2019
  ******************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;

namespace VAdvantage.Process
{
    public class MasterVersions : SvrProcess
    {
        #region Private Variables
        private int _AD_Table_ID = 0;
        private int _AD_Column_ID = 0;
        public List<String> listDefVerCols = new List<String> { "VersionValidFrom", "IsVersionApproved", "ProcessedVersion", "RecordVersion", "Processed", "Processing", "VersionLog", "OldVersion" };
        public List<int> listDefVerRef = new List<int> { 15, 20, 20, 11, 20, 20, 14, 11 };
        public List<String> listDefVerValues = new List<String> { "Created", "'Y'", "'Y'", "1", "'Y'", "'N'", "''", "''" };
        private List<int> _listDefVerElements = new List<int>();  //{ 617, 351, 1047, 524, 624 };
        private StringBuilder MKWhereClause = new StringBuilder("");
        private Trx _trx = null;
        int oldVerCol = 0;
        #endregion Private Variables

        /// <summary>
        /// fetch process parameters
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                }
                else if (name.Equals("AD_Table_ID"))
                    _AD_Table_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());
                else if (name.Equals("AD_Column_ID"))
                    _AD_Column_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
        }

        /// <summary>
        /// process logics (to create master versions)
        /// </summary>
        /// <returns></returns>
        protected override string DoIt()
        {
            _trx = Get_Trx();
            // case when neither Column ID nor Table ID is found to run the process, return in that case
            if (_AD_Column_ID <= 0 && _AD_Table_ID <= 0)
            {
                log.Log(Level.SEVERE, "Table and Column IDs not found");
                return "";
            }

            if (_AD_Table_ID <= 0)
                _AD_Table_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_Table_ID FROM AD_Column WHERE AD_Column_ID = " + _AD_Column_ID, null, _trx));

            // Return if Parent table not found
            if (_AD_Table_ID <= 0)
            {
                log.Log(Level.SEVERE, "Table not found");
                return "";
            }

            string msg = "";
            //try
            //{
            msg = CreateVersionInfo(_AD_Column_ID, _AD_Table_ID, _trx);
            //}
            //catch (Exception e)
            //{
            //    log.Log(Level.SEVERE, "Version table not created :: " + e.Message);
            //    Get_TrxName().Rollback();
            //    return Msg.GetMsg(GetCtx(), "VersionTblNotCreated") + " ==>> " + e.Message;
            //}
            return msg;
        }

        /// <summary>
        /// Create version table and window based on the parent table \
        /// where Maintain Version field is marked as true
        /// </summary>
        /// <returns> Message (String) </returns>
        public string CreateVersionInfo(int AD_Column_ID, int AD_Table_ID, Trx trx)
        {
            _trx = trx;
            _AD_Table_ID = AD_Table_ID;
            _AD_Column_ID = AD_Column_ID;
            bool hasMainVerCol = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_Column_ID FROM AD_Column WHERE AD_Table_ID = " + _AD_Table_ID + " AND IsActive ='Y' AND IsMaintainVersions = 'Y'", null, _trx)) > 0;
            if(!hasMainVerCol)
                hasMainVerCol = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsMaintainVersions FROM AD_Table WHERE AD_Table_ID = " + _AD_Table_ID, null, _trx)) == "Y";
            // check whether there are any columns in the table
            // marked as "Maintain Versions", then proceed else return
            if (hasMainVerCol)
            {
                MTable tbl = new MTable(GetCtx(), _AD_Table_ID, _trx);

                string VerTblName = tbl.GetTableName() + "_Ver";

                // Create/Get System Elements for Version Table Columns
                string retMsg = GetSystemElements(VerTblName);
                if (retMsg != "")
                    return retMsg;

                int Ver_AD_Table_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_Table_ID FROM AD_Table WHERE TableName = '" + VerTblName + "'", null, _trx));

                // check whether version table is already present in system
                // if not present then create table 
                MTable tblVer = null;
                if (Ver_AD_Table_ID <= 0)
                {
                    string tableName = tbl.GetTableName();
                    // create new Version table for parent table
                    tblVer = new MTable(GetCtx(), 0, _trx);
                    tbl.CopyTo(tblVer);
                    tblVer.SetTableName(tableName + "_Ver");
                    tblVer.SetName(tableName + " Ver");
                    tblVer.Set_Value("Export_ID", null);
                    tblVer.Set_Value("AD_Window_ID", null);
                    tblVer.SetIsDeleteable(true);
                    tblVer.SetDescription("Table for maintaining versions of " + tableName);
                    tblVer.SetHelp("Table for maintaining versions of " + tableName);
                    tblVer.SetIsMaintainVersions(false);
                    //tblVer.SetAD_Window_ID(Ver_AD_Window_ID);
                    if (!tblVer.Save())
                    {
                        ValueNamePair vnp = VLogger.RetrieveError();
                        string error = "";
                        if (vnp != null)
                        {
                            error = vnp.GetName();
                            if (error == "" && vnp.GetValue() != null)
                                error = vnp.GetValue();
                        }
                        if (error == "")
                            error = "Error in creating Version Table";
                        log.Log(Level.SEVERE, "Version table not created :: " + error);
                        if(_trx!=null)
                        _trx.Rollback();
                        return Msg.GetMsg(GetCtx(), "VersionTblNotCreated");
                    }
                    else
                    {
                        Ver_AD_Table_ID = tblVer.GetAD_Table_ID();
                        // Create Default Version Columns
                        retMsg = CreateDefaultVerCols(Ver_AD_Table_ID);
                        if (retMsg != "")
                            return retMsg;
                    }
                }
                else
                {
                    tblVer = new MTable(GetCtx(), Ver_AD_Table_ID, _trx);
                    // Create Default Version Columns
                    retMsg = CreateDefaultVerCols(Ver_AD_Table_ID);
                    if (retMsg != "")
                        return retMsg;
                }

                int VerTableColID = 0;
                // if Version table successfully created, then check columns, if not found then create new
                if (Ver_AD_Table_ID > 0)
                {
                    // Get all columns from Version Table
                    int[] ColIDs = MColumn.GetAllIDs("AD_Column", "AD_Table_ID = " + _AD_Table_ID, _trx);

                    bool hasCols = false;
                    DataSet dsDestCols = DB.ExecuteDataset("SELECT ColumnName, AD_Column_ID FROM AD_Column WHERE AD_Table_ID = " + Ver_AD_Table_ID, null, _trx);
                    if (dsDestCols != null && dsDestCols.Tables[0].Rows.Count > 0)
                        hasCols = true;

                    // loop through all columns
                    foreach (int columnID in ColIDs)
                    {
                        bool createNew = true;
                        // object of Column from source table (Master Table)
                        MColumn sCol = new MColumn(GetCtx(), columnID, _trx);
                        // check if source column is not Virtual Column, proceed in that case only
                        if (!sCol.IsVirtualColumn())
                        {
                            DataRow[] dr = null;
                            if (hasCols)
                            {
                                dr = dsDestCols.Tables[0].Select("ColumnName = '" + sCol.GetColumnName() + "'");
                                if (dr.Length > 0)
                                    createNew = false;
                            }
                            // Version Column object
                            MColumn colVer = null;
                            int AD_Col_ID = 0;
                            // if column not present in Version table then create new
                            if (createNew)
                            {
                                colVer = new MColumn(GetCtx(), AD_Col_ID, _trx);
                            }
                            // if column already present and user pressed sync button on same column of Master table
                            // then create object of existing column (in case of change in any column fields)
                            else if (!createNew && (_AD_Column_ID == columnID))
                            {
                                AD_Col_ID = Util.GetValueOfInt(dr[0]["AD_Column_ID"]);
                                colVer = new MColumn(GetCtx(), Util.GetValueOfInt(dr[0]["AD_Column_ID"]), _trx);
                            }
                            if (colVer != null)
                            {
                                sCol.CopyTo(colVer);
                                if (AD_Col_ID > 0)
                                    colVer.SetAD_Column_ID(AD_Col_ID);
                                colVer.SetExport_ID(null);
                                colVer.SetAD_Table_ID(Ver_AD_Table_ID);
                                // set key column to false
                                colVer.SetIsKey(false);
                                // check if source column is key column
                                // then set Restrict Constraint and set Reference as Table Direct
                                if (sCol.IsKey())
                                {
                                    colVer.SetConstraintType("R");
                                    colVer.SetAD_Reference_ID(19);
                                }
                                //if (sCol.IsKey())
                                //    colVer.SetIsParent(true);
                                //else
                                colVer.SetIsParent(false);
                                colVer.SetIsMaintainVersions(false);
                                colVer.SetIsMandatory(false);
                                colVer.SetIsMandatoryUI(false);
                                if (!colVer.Save())
                                {
                                    ValueNamePair vnp = VLogger.RetrieveError();
                                    string error = "";
                                    if (vnp != null)
                                    {
                                        error = vnp.GetName();
                                        if (error == "" && vnp.GetValue() != null)
                                            error = vnp.GetValue();
                                    }
                                    if (error == "")
                                        error = "Version Column not created";
                                    log.Log(Level.SEVERE, "Version Column not created :: " + sCol.GetColumnName() + " :: " + error);
                                    if (_trx != null)
                                        _trx.Rollback();
                                    return Msg.GetMsg(GetCtx(), "VersionColNotCreated");
                                }
                                else
                                    VerTableColID = colVer.GetAD_Column_ID();
                            }
                        }
                    }

                    // Get one column to sync table in database from Version Table
                    if (VerTableColID <= 0)
                        VerTableColID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_Column_ID FROM AD_Column WHERE AD_Table_ID = " + Ver_AD_Table_ID, null, _trx));

                    // Get newly Created Column
                    if (oldVerCol > 0)
                        VerTableColID = oldVerCol;

                    // Sync Version table in database
                    bool success = true;
                    retMsg = SyncVersionTable(tblVer, VerTableColID, out success);
                    // if any error and there is message in return then return and rollback transaction
                    if (!success && retMsg != "")
                    {
                        log.Log(Level.SEVERE, "Column not sync :: " + retMsg);
                        if (_trx != null)
                            _trx.Rollback();
                        return Msg.GetMsg(GetCtx(), "ColumnNotSync");
                    }
                    else
                    {
                        // if table has single key
                        if (tbl.IsSingleKey())
                        {
                            // get column names from parent table 
                            string colNameStrings = GetColumnNameString(tbl.GetAD_Table_ID());
                            // get default columns string from Version Table columns list
                            string defColString = GetDefaultColString(colNameStrings);
                            // Insert data in version table from Master table
                            InsertVersionData(colNameStrings, defColString, tblVer.GetTableName());
                        }
                        // Cases where single key is not present in Master table
                        else
                        {
                            // Insert data in version table against Master Table
                            retMsg = InsertMKVersionData(tbl, tbl.GetKeyColumns(), tblVer);
                            if (retMsg != "")
                            {
                                log.Log(Level.SEVERE, "Data not Inserted :: " + retMsg);
                                if (_trx != null)
                                    _trx.Rollback();
                                return Msg.GetMsg(GetCtx(), "DataInsertionErrorMultikey");
                            }
                        }
                    }
                }
            }
            return Msg.GetMsg(GetCtx(), "ProcessCompletedSuccessfully");
        }

        /// <summary>
        /// Get System Elements for Default Columns
        /// </summary>
        /// <param name="VerTblName">Table Name</param>
        public string GetSystemElements(string VerTblName)
        {
            // check if count in list is equal to default version columns
            if (_listDefVerElements.Count == listDefVerCols.Count)
                return "";

            // Clear values from list
            _listDefVerElements.Clear();

            // check if Primary key column is present in Columns list, if not present then add Primary Key column
            if (!listDefVerCols.Contains(VerTblName + "_ID"))
                listDefVerCols.Add(VerTblName + "_ID");

            // Create comma separated string of all default version columns
            string DefSysEle = string.Join(",", listDefVerCols
                                            .Select(x => string.Format("'{0}'", x)));

            // Get System Elements and Column Names for all Version table columns
            DataSet dsDefVerCols = DB.ExecuteDataset("SELECT AD_Element_ID, ColumnName FROM AD_Element WHERE ColumnName IN (" + DefSysEle + ")", null, _trx);

            if (dsDefVerCols != null && dsDefVerCols.Tables[0].Rows.Count > 0)
            {
                // loop through all columns of version table to get System Elements 
                // if not found then create new 
                for (int i = 0; i < listDefVerCols.Count; i++)
                {
                    DataRow[] drSysEle = dsDefVerCols.Tables[0].Select("ColumnName='" + listDefVerCols[i] + "'");
                    if (drSysEle.Length > 0)
                    {
                        if (!_listDefVerElements.Contains(Util.GetValueOfInt(drSysEle[0]["AD_Element_ID"])))
                            _listDefVerElements.Add(Util.GetValueOfInt(drSysEle[0]["AD_Element_ID"]));
                        if (listDefVerCols[i] == VerTblName + "_ID")
                        {
                            listDefVerRef.Add(13);
                        }
                    }
                    else
                    {
                        M_Element ele = new M_Element(GetCtx(), 0, _trx);
                        ele.SetAD_Client_ID(0);
                        ele.SetAD_Org_ID(0);
                        ele.SetName(listDefVerCols[i]);
                        ele.SetColumnName(listDefVerCols[i]);
                        ele.SetPrintName(listDefVerCols[i]);
                        if (!ele.Save())
                        {
                            ValueNamePair vnp = VLogger.RetrieveError();
                            string error = "";
                            if (vnp != null)
                            {
                                error = vnp.GetName();
                                if (error == "" && vnp.GetValue() != null)
                                    error = vnp.GetValue();
                            }
                            if (error == "")
                                error = "Error in creating System Element";
                            log.Log(Level.SEVERE, error);
                            if (_trx != null)
                                _trx.Rollback();
                            return Msg.GetMsg(GetCtx(), "ElementNotSaved");
                        }
                        else
                        {
                            _listDefVerElements.Add(ele.GetAD_Element_ID());
                            if (ele.GetColumnName() == VerTblName + "_ID")
                            {
                                listDefVerRef.Add(13);
                            }
                        }
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// Insert data in Version table against multikey Master table
        /// </summary>
        /// <param name="baseTbl"></param>
        /// <param name="keyCols"></param>
        /// <param name="tblVer"></param>
        /// <returns></returns>
        private string InsertMKVersionData(MTable baseTbl, string[] keyCols, MTable tblVer)
        {
            string retMsg = "";
            // Get data from Master table
            DataSet dsRecs = DB.ExecuteDataset("SELECT * FROM " + baseTbl.GetTableName(), null, _trx);

            // check if there are any records in master table
            if (dsRecs != null && dsRecs.Tables[0].Rows.Count > 0)
            {
                // loop through all records and insert in Version table
                for (int i = 0; i < dsRecs.Tables[0].Rows.Count; i++)
                {
                    // get where Clause for Master table against multiple key columns
                    GetMKWhereClause(dsRecs.Tables[0].Rows[i], keyCols);
                    if (MKWhereClause.Length > 0)
                    {
                        int count = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(" + tblVer.GetTableName() + "_ID) FROM " + tblVer.GetTableName() + " WHERE " + MKWhereClause.ToString(), null, _trx));
                        if (count > 0)
                            continue;
                    }

                    // create PO object of source table (Master table)
                    PO sPO = baseTbl.GetPO(GetCtx(), dsRecs.Tables[0].Rows[i], _trx);
                    // create PO object of destination table (Version table)
                    PO dPO = tblVer.GetPO(GetCtx(), 0, _trx);
                    sPO.CopyTo(dPO);
                    dPO.SetAD_Client_ID(sPO.GetAD_Client_ID());
                    dPO.SetAD_Org_ID(sPO.GetAD_Org_ID());
                    dPO.Set_Value("RecordVersion", "1");
                    dPO.Set_ValueNoCheck("VersionValidFrom", sPO.Get_Value("Created"));
                    dPO.Set_ValueNoCheck("IsVersionApproved", true);
                    dPO.Set_ValueNoCheck("Processed", true);
                    dPO.Set_ValueNoCheck("ProcessedVersion", true);
                    for (int j = 0; j < keyCols.Length; j++)
                    {
                        dPO.Set_ValueNoCheck(keyCols[j], sPO.Get_Value(keyCols[j]));
                    }
                    dPO.Set_Value("Export_ID", null);
                    if (!dPO.Save())
                    {
                        ValueNamePair vnp = VLogger.RetrieveError();
                        string error = "";
                        if (vnp != null)
                        {
                            error = vnp.GetName();
                            if (error == "" && vnp.GetValue() != null)
                                error = vnp.GetValue();
                        }
                        if (error == "")
                            error = "Error in saving data in Version table";
                        return retMsg;
                    }
                }
            }

            return retMsg;
        }

        /// <summary>
        /// Get where clause against Master table
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="keyCols"></param>
        private void GetMKWhereClause(DataRow dr, string[] keyCols)
        {
            MKWhereClause.Clear();
            int kLen = keyCols.Length;
            if (kLen > 0)
            {
                for (int i = 0; i < kLen; i++)
                {
                    if (i == 0)
                        MKWhereClause.Append(keyCols[i] + " = " + dr[keyCols[i]]);
                    else
                        MKWhereClause.Append(" AND " + keyCols[i] + " = " + dr[keyCols[i]]);
                }
            }
        }

        /// <summary>
        /// Create query to insert data in version table and insert data in Version table
        /// </summary>
        /// <param name="colNameStrings"></param>
        /// <param name="defColString"></param>
        /// <param name="TableName"></param>
        private void InsertVersionData(string colNameStrings, string defColString, string TableName)
        {
            //string baseTableName = TableName.Replace("_Ver", ""); //Check HERE
            string baseTableName = TableName;
            // check if table name ends with "_Ver" (stands for Version table)
            if (TableName.EndsWith("_Ver"))
            {
                baseTableName = baseTableName.Substring(0, TableName.Length - 4);
            }
            StringBuilder sbIns = new StringBuilder("INSERT INTO " + TableName + "(" + colNameStrings);
            StringBuilder sbVals = new StringBuilder("");
            string[] defColStringVal = defColString.Split(',');
            // loop through columns to create column name strings
            for (int i = 0; i < defColStringVal.Length; i++)
            {
                if (Util.GetValueOfString(defColStringVal[i]).Trim() == "")
                    continue;
                sbIns.Append("," + defColStringVal[i]);
                int indDefVerCol = listDefVerCols.IndexOf(defColStringVal[i]);
                if (indDefVerCol >= 0)
                {
                    if (defColStringVal[i].Trim().ToLower() == (TableName + "_ID").ToLower())
                        sbVals.Append("," + baseTableName + "_ID");
                    else
                        sbVals.Append("," + listDefVerValues[indDefVerCol]);
                }
            }

            // create select subquery to get data from Master table
            sbIns.Append(") ").Append(@"SELECT " + colNameStrings + sbVals + " FROM " + baseTableName
                                + " WHERE " + baseTableName + "_ID NOT IN (SELECT " + baseTableName + "_ID FROM " + TableName + ")");

            // fire query into database to insert data in version table
            int CountIns = DB.ExecuteQuery(sbIns.ToString(), null, _trx);

            log.Info("Master Version Insertion Query" + sbIns.ToString() + " :: Records Inserted ==>> " + CountIns);

            // Update AD_Sequence for Version table to set max + 1 against version table
            CountIns = DB.ExecuteQuery("UPDATE AD_Sequence SET CurrentNext = (SELECT (MAX(NVL(" + TableName + "_ID,0)) + 1) FROM " + TableName + ") WHERE LOWER(Name) = '" + TableName.Trim().ToLower() + "'", null, _trx);

            log.Info("Master Version Sequence Updated " + CountIns);
        }

        /// <summary>
        /// Sync Version table in database
        /// </summary>
        /// <param name="table"></param>
        /// <param name="AD_Column_ID"></param>
        /// <returns>Message (String)</returns>
        private string SyncVersionTable(MTable table, int AD_Column_ID, out bool Success)
        {
            // create object of Column passed in parameter
            Success = true;
            MColumn column = new MColumn(GetCtx(), AD_Column_ID, _trx);
            int noColumns = 0;
            // sync table in database
            string sql = VAdvantage.Common.Common.SyncColumn(table, column, out noColumns);
            string exception = "";
            int no = 0;
            if (sql.IndexOf(";") == -1)
            {
                try
                {
                    no = DataBase.DB.ExecuteQuery(sql, null, _trx);
                    AddLog(0, DateTime.MinValue, Decimal.Parse(no.ToString()), sql);
                }
                catch (Exception ex)
                {
                    Success = false;
                    exception = ex.Message;
                    return exception;
                }
                //addLog (0, null, new BigDecimal(no), sql);
            }
            else
            {
                //string ss = "; ";
                string[] statements = sql.Split(';');
                for (int i = 0; i < statements.Length; i++)
                {
                    int count = DataBase.DB.ExecuteQuery(statements[i].ToString(), null, _trx);
                    AddLog(0, DateTime.MinValue, Decimal.Parse(count.ToString()), statements[i]);
                    no += count;
                }
            }

            if (no == -1)
            {
                Success = false;
                string msg = "@Error@ ";
                ValueNamePair pp = VAdvantage.Logging.VLogger.RetrieveError();
                if (pp != null)
                    msg += exception + " - ";
                msg += sql;
                return msg;
            }

            // Apply constraints on columns for Version table
            ColumnSync colSync = new ColumnSync();
            colSync.SetAD_Column_ID(AD_Column_ID);
            string r = colSync.createFK(noColumns);

            return r;
        }

        /// <summary>
        /// Create default columns for Master Data Version Table
        /// e.g. Processed, Processing, IsApproved etc.
        /// </summary>
        /// <param name="Ver_AD_Table_ID"></param>
        /// <returns></returns>
        private string CreateDefaultVerCols(int Ver_AD_Table_ID)
        {
            DataSet dstblCols = DB.ExecuteDataset("SELECT ColumnName FROM AD_Column WHERE AD_Table_ID = " + Ver_AD_Table_ID, null, null);

            for (int i = 0; i < listDefVerCols.Count; i++)
            {
                bool hasCol = false;
                if (dstblCols != null && dstblCols.Tables[0].Rows.Count > 0)
                {
                    DataRow[] dr = dstblCols.Tables[0].Select("ColumnName = '" + listDefVerCols[i] + "'");
                    if (dr != null && dr.Length > 0)
                        hasCol = true;
                }
                if (hasCol)
                    continue;
                MColumn colVer = new MColumn(GetCtx(), 0, _trx);
                colVer.SetExport_ID(null);
                colVer.SetAD_Table_ID(Ver_AD_Table_ID);
                colVer.SetColumnName(listDefVerCols[i]);
                colVer.SetAD_Element_ID(_listDefVerElements[i]);
                colVer.SetAD_Reference_ID(listDefVerRef[i]);
                //if (listDefVerCols[i] == "VersionValidFrom")
                //    colVer.SetIsParent(true);
                if (listDefVerRef[i] == 10)
                    colVer.SetFieldLength(10);
                if (listDefVerRef[i] == 14)
                    colVer.SetFieldLength(2000);
                if (listDefVerRef[i] == 13)
                {
                    colVer.SetIsKey(true);
                    colVer.SetIsMandatory(true);
                    colVer.SetIsMandatoryUI(true);
                }
                if (!colVer.Save())
                {
                    ValueNamePair vnp = VLogger.RetrieveError();
                    string error = "";
                    if (vnp != null)
                    {
                        error = vnp.GetName();
                        if (error == "" && vnp.GetValue() != null)
                            error = vnp.GetValue();
                    }
                    if (error == "")
                        error = "Error in creating Version Column " + listDefVerCols[i];
                    log.Log(Level.SEVERE, "Version Column not created :: " + listDefVerCols[i] + " :: " + error);
                    if (_trx != null)
                        _trx.Rollback();
                    return Msg.GetMsg(GetCtx(), "VersionColNotCreated");
                }
                else
                    oldVerCol = colVer.GetAD_Column_ID();
            }
            return "";
        }

        /// <summary>
        /// Get column name string from Version table
        /// </summary>
        /// <param name="Ver_AD_Table_ID"></param>
        /// <returns>Comma separated string for column names</returns>
        private string GetColumnNameString(int Ver_AD_Table_ID)
        {
            StringBuilder colNameString = new StringBuilder("");
            // VIS0008 Ignored Virtual columns here
            DataSet dsCols = DB.ExecuteDataset("SELECT ColumnName FROM AD_Column WHERE AD_Table_ID = " + Ver_AD_Table_ID + " AND IsActive = 'Y' AND ColumnSQL IS NULL", null, _trx);
            if (dsCols != null && dsCols.Tables[0].Rows.Count > 0)
            {
                string[] colNames = dsCols.Tables[0].AsEnumerable().Select(r => Convert.ToString(r["ColumnName"])).ToArray();
                List<string> list = new List<string>(colNames);
                var ind = list.IndexOf("Processed");
                if (ind >= 0)
                {
                    list.Remove("Processed");
                }

                ind = list.IndexOf("Processing");
                if (ind >= 0)
                {
                    list.Remove("Processing");
                }
                colNames = list.ToArray();
                colNameString.Append(string.Join(",", colNames));
            }
            return colNameString.ToString();
        }

        /// <summary>
        /// Get Default COlumn names string for Version table Column
        /// </summary>
        /// <param name="colString"></param>
        /// <returns>Comma separated string for default column names for version table</returns>
        private string GetDefaultColString(string colString)
        {
            StringBuilder defColString = new StringBuilder("");

            string[] colNames = colString.ToLower().Split(',');
            for (int i = 0; i < listDefVerCols.Count; i++)
            {
                if (!colNames.Contains(listDefVerCols[i].ToLower()))
                    defColString.Append("," + listDefVerCols[i]);
            }
            return defColString.ToString();
        }
    }
}
