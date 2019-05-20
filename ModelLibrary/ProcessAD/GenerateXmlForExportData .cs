using System;
using System.Net;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;
using VAdvantage.Logging;
using System.Data.SqlClient;
using System.Data;
using VAdvantage.DataBase;
using System.Collections.Generic;
using VAdvantage.Model;
using System.Text;
using System.Linq;
using VAdvantage.Classes;
using System.IO;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Web.Hosting;

namespace VAdvantage.Process
{
    /// <summary>
    /// Xml Generator process for records which are to be exported
    /// </summary>
    public class GenerateXmlForExportData : SvrProcess
    {
        //Replace Space with Underscore
        string Module_Name = "";
        string prefix = "";
       // string versionId = "1.0.0.0";
        string versionNo = "1000";
        //string Database_Schema = "DataBaseSchema";


        private int _AD_ModuleInfo_ID = 0;

        private String _FilePath = "";

        private int rowNum = 0;

        string msg = "";

        List<string> deleteSqlExp = new List<string>();

        //Checks for already exported record. If array contains a particular record, that record is excluded
        List<ExportDataRecords> _ExportRecordList = new List<ExportDataRecords>();
        List<ExportDataRecords> _ExecutedRecordList = new List<ExportDataRecords>();

        private DataSet ds; //Stores all the fetched records

        String[] _ExceptionTables = new String[] { "AD_Role", "AD_User" };   //Stores tables which are not be included

        /// <summary>
        /// Prepare any parameter to be passed
        /// </summary>
        protected override void Prepare()
        {
            //            int _AD_ExportData_ID = GetRecord_ID();
            //            IDataReader dr = DB.ExecuteReader(@" SELECT AD_ModuleInfo_ID from ad_ExportDAta 
            //                                                  WHERE AD_ExportData_ID =" + _AD_ExportData_ID);

            //            if (dr.Read())
            //                _AD_ModuleInfo_ID = Convert.ToInt32(dr[0].ToString());
            //            dr.Close();

            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("AD_ModuleInfo_ID"))
                {
                    _AD_ModuleInfo_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }

        /// <summary>
        /// Executes the process
        /// </summary>
        /// <returns></returns>
        protected override string DoIt()
        {
            File.AppendAllText(HostingEnvironment.ApplicationPhysicalPath + "\\log\\XMLLog.txt", "DoIT");

            _ExportRecordList = GetExportData();    //fetch all the records to be exported / marked by a user
            ds = new DataSet(); //init ds

            DataTable SeqTable = new DataTable();
            SeqTable.TableName = "ExportTableSequence";
            SeqTable.Columns.Add("RowNum");
            SeqTable.Columns.Add("TableName");
            SeqTable.Columns.Add("Record_ID");
            SeqTable.Columns.Add("AD_ColOne_ID");
            ds.Tables.Add(SeqTable);

            //Parse through the marked record one by one
            foreach (ExportDataRecords exportdata in _ExportRecordList)
            {
                MTable currentTable = MTable.Get(GetCtx(), exportdata.AD_Table_ID);
                if (currentTable == null)   //should not be null
                {
                    continue;   //skip the record and move further for next record
                }

                if (currentTable.Get_ID() == 0) //should not be 0
                {
                    log.Log(Level.SEVERE, "Table record not found. Continuing with next record");
                    continue;   //move next
                }
                File.AppendAllText(HostingEnvironment.ApplicationPhysicalPath + "\\log\\XMLLog.txt", "GetFData"+currentTable);
                msg = GetForeignData(currentTable, exportdata);

                if (msg.Length > 0)
                {
                    return msg;
                }

            }

            if (module == null)
                module = new X_AD_ModuleInfo(GetCtx(), _AD_ModuleInfo_ID, null);
            prefix = module.GetPrefix();
            Module_Name = module.GetName();
            versionNo = module.GetVersionNo();

            // versionId = module.GetVersionID();

            _FilePath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, prefix, Module_Name.Trim() + "_" + versionNo, "Data");

            if (!Directory.Exists(_FilePath))
                Directory.CreateDirectory(_FilePath);

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                try
                {

                    DataRow r = ds.Tables[0].Rows[i];

                    string str = "Record_ID='" + r["Record_ID"].ToString() + "' and TableName='" + r["TableName"].ToString() + "'";

                    File.AppendAllText(HostingEnvironment.ApplicationPhysicalPath + "\\log\\XMLLog.txt", str);

                    if (r["AD_ColOne_ID"] != DBNull.Value && r["AD_ColOne_ID"] != null && r["AD_ColOne_ID"].ToString() != "")
                    {
                        str += " and AD_ColOne_ID=" + Util.GetValueOfString(r["AD_ColOne_ID"]);
                    }
                    DataRow[] row = ds.Tables[0].Select(str);
                    int findmax = Convert.ToInt32(row[row.Count() - 1]["RowNum"]);
                    bool isDeleted = false;
                    foreach (DataRow a in row)
                    {
                        if (!Convert.ToInt32(a["RowNum"]).Equals(findmax))
                        {
                            a.Delete();
                            isDeleted = true;
                        }
                    }
                    if (isDeleted)
                    {
                        --i;
                    }
                }

                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

            // Delete Marking of records which were not found. 
            DeleteRecordsMarked();

            return ds.ToXml(_FilePath);
        }

        private void DeleteRecordsMarked()
        {
            for (int i = 0; i < deleteSqlExp.Count; i++)
            {
                DB.ExecuteQuery(deleteSqlExp[i]);
            }
        }

        String ModulePath = String.Empty;
        X_AD_ModuleInfo module = null;

        private String ManageExportID(int PrimaryKey, String tableName, String refTable)
        {
            try
            {
                if (String.IsNullOrEmpty(refTable))
                    refTable = tableName;

                if (module == null)
                    module = new X_AD_ModuleInfo(GetCtx(), _AD_ModuleInfo_ID, null);

                //String exportid = module.GetPrefix() + PrimaryKey;    //export id to be picked from msequence table

                String exportid = module.GetPrefix() + MSequence.GetNextExportID(GetCtx().GetAD_Client_ID(), tableName, null);

                String _updateSql = "Update " + refTable + " SET Export_ID = " + DB.TO_STRING(exportid) + " WHERE " + tableName + "_ID = " + PrimaryKey;
                int result = DB.ExecuteQuery(_updateSql);
                return exportid;
            }
            catch
            {
                return "";
            }
        }


        private String ManageExportID(int recordID, int AD_Colone_ID, String tableName, int _table_ID)
        {
            try
            {
                //if (String.IsNullOrEmpty(refTable))
                //    refTable = tableName;

                if (module == null)
                    module = new X_AD_ModuleInfo(GetCtx(), _AD_ModuleInfo_ID, null);

                //String exportid = module.GetPrefix() + PrimaryKey;    //export id to be picked from msequence table

                String exportid = module.GetPrefix() + MSequence.GetNextExportID(GetCtx().GetAD_Client_ID(), tableName, null);

                string[] ds = GetParentColumns(_table_ID);

                //String _updateSql = "Update " + tableName + " SET Export_ID = " + DB.TO_STRING(exportid) + " WHERE " + tableName + "_ID = " + recordID;

                String _updateSql = "Update " + tableName + " SET Export_ID = " + DB.TO_STRING(exportid) + " WHERE ";

                //  _updateSql += ds.Tables[0].Rows[0]["ColumnName"].ToString() + " =" + recordID;
                _updateSql += ds[0] + " =" + recordID;

                if (ds.Length > 1)
                {
                    //_updateSql += " and " + ds.Tables[0].Rows[1]["ColumnName"].ToString() + " =" + AD_Colone_ID;
                    _updateSql += " and " + ds[1] + " =" + AD_Colone_ID;
                }

                int result = DB.ExecuteQuery(_updateSql);
                return exportid;
            }
            catch
            {
                return "";
            }
        }


        /// <summary>
        /// Manage export id which is unique for exporting data. Generated only once
        /// </summary>
        /// <param name="PrimaryKey">Primary key of the row whose export is to be generated</param>
        /// <param name="tableName">Name of the table</param>
        /// <returns>true or false based on updation</returns>
        private String ManageExportID(int PrimaryKey, String tableName)
        {
            return ManageExportID(PrimaryKey, tableName, "");
        }

        /// <summary>
        /// Check for Reference List in database
        /// </summary>
        /// <param name="refVID">Ref value ID</param>
        /// <param name="tableName">Name of the table</param>
        MRefTable CheckReference(ExportDataRecords refRecord, String _tableName)
        {
            MRefTable refTable = null;
            int refVID = refRecord.Record_ID;
            string tableName = _tableName;

            //check if the record is already exported. 
            var res = _ExecutedRecordList.Where((a) => a.AD_Table_ID == refRecord.AD_Table_ID)
                                         .Where((a) => a.Record_ID == refRecord.Record_ID);

            //if (res.Count() <= 0)
            {
                _ExecutedRecordList.Add(refRecord);
                String chkRef = "SELECT * FROM AD_Reference WHERE IsActive = 'Y' AND AD_Reference_ID =" + refVID;
                String ValidationType = "";
                DataSet tmpDS = DB.ExecuteDataset(chkRef);
                if (tmpDS.Tables[0].Rows[0]["ValidationType"] != DBNull.Value)
                    ValidationType = tmpDS.Tables[0].Rows[0]["ValidationType"].ToString();
                tmpDS.Tables[0].TableName = tableName;

                if (tmpDS.Tables[0].Rows[0]["Export_ID"].Equals(DBNull.Value))
                {
                    tmpDS.Tables[0].Rows[0]["Export_ID"] = ManageExportID(refVID, tableName);
                }

                DataSet tempRefDS = tmpDS.Copy();

                if (!String.IsNullOrEmpty(ValidationType)) //if reference entry is found, chck furhter for ad_ref_list records
                {
                    String refTableName = ValidationType.Equals("L") ? "AD_Ref_List" : "AD_Ref_Table";

                    tmpDS = DB.ExecuteDataset(GetSql(refTableName, refVID, "AD_Reference_ID"));
                    tmpDS.Tables[0].TableName = refTableName;

                    for (int a = 0; a < tmpDS.Tables[0].Rows.Count; a++)
                    {
                        try
                        {
                            String tempStore = "";
                            int PrimaryID = 0;
                            if (ValidationType.Equals("T"))
                            {
                                tempStore = "AD_Reference_ID";
                                PrimaryID = int.Parse(tmpDS.Tables[0].Rows[a][tempStore].ToString());

                                refTable = new MRefTable(GetCtx(), tmpDS.Tables[0].Rows[a], null);

                                if (tmpDS.Tables[0].Rows[a]["Export_ID"].Equals(DBNull.Value))
                                {
                                    tmpDS.Tables[0].Rows[a]["Export_ID"] = ManageExportID(refVID, tableName, "AD_Ref_Table");
                                }
                            }
                            else
                            {
                                PrimaryID = int.Parse(tmpDS.Tables[0].Rows[a][refTableName + "_ID"].ToString());

                                if (tmpDS.Tables[0].Rows[a]["Export_ID"].Equals(DBNull.Value))
                                {
                                    tmpDS.Tables[0].Rows[a]["Export_ID"] = ManageExportID(PrimaryID, refTableName);
                                }
                                ds.AddOrCopy(tmpDS, refTableName, PrimaryID, 0, null, rowNum++);

                            }

                        }
                        catch
                        {
                            log.Log(Level.SEVERE, "PrimaryID Issue");
                        }
                    }

                    if (ValidationType.Equals("T"))
                        if (tmpDS != null && tmpDS.Tables[0].Rows.Count > 0)
                        {
                            ds.AddOrCopy(tmpDS, refTableName, refVID, 0, null, rowNum++);

                        }
                }

                ds.AddOrCopy(tempRefDS, tableName, refVID, 0, null, rowNum++);

                return refTable;
            }
        }

        /// <summary>
        /// Gets the parent record for every row (if exists)
        /// </summary>
        /// <param name="currentTable">Current table to be parsed</param>
        /// <param name="exportdata">Exportdata info</param>
        private string GetForeignData(MTable currentTable, ExportDataRecords exportdata)
        {
            try
            {

                //check if the record is already exported. 
                var res = _ExecutedRecordList.Where((a) => a.AD_Table_ID == exportdata.AD_Table_ID)
                                             .Where((a) => a.Record_ID == exportdata.Record_ID)
                                             .Where((a) => a.AD_ColOne_ID == exportdata.AD_ColOne_ID);

                //if (res.Count() <= 0)
                {
                    _ExecutedRecordList.Add(exportdata);

                    String tableName = currentTable.GetTableName();

                    if (!_ExceptionTables.Contains(tableName))
                    {

                        if (exportdata.AD_ColOne_ID == 0)
                        {
                            File.AppendAllText(HostingEnvironment.ApplicationPhysicalPath + "\\log\\XMLLog.txt", tableName + " : " + exportdata.Record_ID);
                            int found = 0;
                            if (ds.Tables[tableName] != null)
                                found = ds.Tables[tableName].Select(tableName + "_ID = " + exportdata.Record_ID).Count();
                            MColumn[] columns = currentTable.GetColumns(true); //Fetch column details
                            if (columns.Length > 0)
                            {

                                string sql = GetSql(currentTable.GetAD_Table_ID(), currentTable.GetTableName(), exportdata);

                                DataSet tmpDS = DB.ExecuteDataset(sql, null);

                                if (tmpDS == null || tmpDS.Tables[0].Rows.Count <= 0)
                                {
                                    //sql = sql.Substring(sql.IndexOf("WHERE"));
                                    if (tmpDS != null)
                                    {
                                        deleteSqlExp.Add("delete from AD_ExportData Where record_ID = " + exportdata.Record_ID + " and ad_table_id = " + exportdata.AD_Table_ID + " and ad_Moduleinfo_id = " + _AD_ModuleInfo_ID);
                                    }
                                    return "";
                                }

                                if (tmpDS.Tables[0].Rows[0]["Export_ID"].Equals(DBNull.Value))
                                {
                                    tmpDS.Tables[0].Rows[0]["Export_ID"] = ManageExportID(exportdata.Record_ID, tableName);
                                }

                                ds.AddOrCopy(tmpDS, tableName, exportdata.Record_ID, 0, null, rowNum++);     //add or copy 

                                for (int cols = 0; cols <= columns.Length - 1; cols++)
                                {
                                    string colName = columns[cols].GetColumnName();
                                    int refVID = columns[cols].GetAD_Reference_Value_ID();
                                    int refID = columns[cols].GetAD_Reference_ID();

                                    if (!columns[cols].IsStandardColumn() && !columns[cols].IsKey())
                                    {
                                        if (colName.EndsWith("_ID"))    //only columns ending with _ID to be processed (indicated Foreign Key )
                                        {
                                            if (!columns[cols].GetColumnName().Equals("Export_ID"))
                                            {
                                                Object colValue = tmpDS.Tables[0].Rows[0][colName];
                                                if (colValue != null)
                                                {
                                                    if (!String.IsNullOrEmpty(colValue.ToString()))
                                                    {
                                                        MTable fkTable = columns[cols].GetFKTable(); //Get the Parent table of the FK Column
                                                        if (fkTable != null)
                                                            GetForeignData(fkTable, new ExportDataRecords() { AD_Table_ID = fkTable.GetAD_Table_ID(), Record_ID = Convert.ToInt32(colValue) });
                                                    }
                                                }
                                            }
                                        }
                                        else if (refID == DisplayType.List || refID == DisplayType.Table)
                                        {
                                            Object colValue = tmpDS.Tables[0].Rows[0][colName];
                                            MRefTable refTable = CheckReference(new ExportDataRecords() { Record_ID = refVID, AD_Table_ID = MTable.Get_Table_ID("AD_Reference") }, "AD_Reference");

                                            if (refTable != null && colValue != null && colValue.ToString() != "")
                                            {

                                                try
                                                {
                                                    MTable tbl = MTable.Get(GetCtx(), refTable.GetAD_Table_ID());

                                                    //string tName  =  MTable.GetTableName(GetCtx(), refTable.GetAD_Table_ID());
                                                    string cName = MColumn.GetColumnName(GetCtx(), refTable.GetColumn_Key_ID());

                                                    int recordId;
                                                    if (int.TryParse(colValue.ToString(), out recordId)) //If Value is type of int
                                                    {
                                                        ;
                                                    }
                                                    else
                                                    {
                                                        recordId = Convert.ToInt32(DB.ExecuteScalar("SELECT " + tbl.GetTableName() + "_ID FROM " + tbl.GetTableName() + " WHERE " + cName + " = '" + colValue.ToString() + "'"));
                                                        cName = tbl.GetTableName() + "_ID";
                                                    }

                                                    DataSet temp = DB.ExecuteDataset("SELECT * FROM " + tbl.GetTableName() + " WHERE " + cName + " = " + recordId);

                                                    ds.AddOrCopy(temp, tbl.GetTableName(), recordId, 0, null, rowNum++);

                                                    GetForeignData(tbl, new ExportDataRecords() { AD_Table_ID = tbl.GetAD_Table_ID(), Record_ID = recordId });

                                                }
                                                catch (Exception ex)
                                                {
                                                    log.Severe("Table Reference =>" + ex.Message);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ;
                                        }
                                    }
                                }
                            }   //column length #if
                        }
                        else
                        {
                            MColumn[] columns = currentTable.GetColumns(true); //Fetch column details
                            if (columns.Length > 0)
                            {
                                string sql = GetSql(currentTable.GetAD_Table_ID(), currentTable.GetTableName(), exportdata);

                                DataSet tmpDS = DB.ExecuteDataset(sql, null);

                                if (tmpDS == null || tmpDS.Tables[0].Rows.Count <= 0)
                                {
                                    //sql = sql.Substring(sql.IndexOf("WHERE"));
                                    if (tmpDS != null)
                                    {
                                        deleteSqlExp.Add("delete from AD_ExportData Where record_ID = " + exportdata.Record_ID + " and ad_table_id = " + exportdata.AD_Table_ID + " and ad_Moduleinfo_id = " + _AD_ModuleInfo_ID);
                                    }
                                    return "";
                                }


                                if (tmpDS.Tables[0].Rows[0]["Export_ID"].Equals(DBNull.Value))
                                {
                                    tmpDS.Tables[0].Rows[0]["Export_ID"] = ManageExportID(exportdata.Record_ID, exportdata.AD_ColOne_ID, tableName, currentTable.GetAD_Table_ID());
                                }

                                ds.AddOrCopy(tmpDS, tableName, exportdata.Record_ID, exportdata.AD_ColOne_ID, GetParentColumns(currentTable.GetAD_Table_ID()), rowNum++);     //add or copy 

                                for (int cols = 0; cols <= columns.Length - 1; cols++)
                                {
                                    string colName = columns[cols].GetColumnName();
                                    int refVID = columns[cols].GetAD_Reference_Value_ID();
                                    int refID = columns[cols].GetAD_Reference_ID();

                                    if (!columns[cols].IsStandardColumn() && !columns[cols].IsKey())
                                    {
                                        if (colName.EndsWith("_ID"))    //only columns ending with _ID to be processed (indicated Foreign Key )
                                        {
                                            if (!columns[cols].GetColumnName().Equals("Export_ID"))
                                            {
                                                Object colValue = tmpDS.Tables[0].Rows[0][colName];
                                                if (colValue != null)
                                                {
                                                    if (!String.IsNullOrEmpty(colValue.ToString()))
                                                    {
                                                        MTable fkTable = columns[cols].GetFKTable(); //Get the Parent table of the FK Column
                                                        if (fkTable != null)
                                                            GetForeignData(fkTable, new ExportDataRecords() { AD_Table_ID = fkTable.GetAD_Table_ID(), Record_ID = Convert.ToInt32(colValue) });
                                                    }
                                                }
                                            }
                                        }
                                        else if (refID == DisplayType.List || refID == DisplayType.Table)
                                        {
                                            CheckReference(new ExportDataRecords() { Record_ID = refVID, AD_Table_ID = MTable.Get_Table_ID("AD_Reference") }, "AD_Reference");
                                        }
                                        else
                                        {
                                            ;
                                        }
                                    }
                                }
                            }   //c
                        }
                    }   //exception table #if
                }
                //else
                //{
                //}
            }
            catch (Exception ex)
            {
                log.Log(Level.SEVERE, ex.Message);
                return ex.Message.ToString();
            }
            return "";
        }


        /// <summary>
        /// Check colName has suffix end with (_1,_2_3)
        /// </summary>
        /// <param name="colName"></param>
        /// <example> AD_AccountSubGroup_ID_1 </example>
        /// <returns> true if contain</returns>
        private bool ColNameEndsWithID(string colName)
        {
            for (int i = 1; i < 4; i++)
            {
                if (colName.EndsWith("_ID") || colName.EndsWith("_ID_" + i))
                {
                    return true;
                }
            }
            return false;
        }

        String GetSql(String _tableName, int ID)
        {
            return GetSql(_tableName, ID, "");
        }

        String GetSql(String _tableName, int ID, String whereColumn)
        {
            String _Sql = "";
            if (!String.IsNullOrEmpty(whereColumn))
                _Sql = "SELECT * FROM " + _tableName + " WHERE IsActive = 'Y' AND " + whereColumn + " = " + ID;
            else
                _Sql = "SELECT * FROM " + _tableName + " WHERE IsActive = 'Y' AND " + _tableName + "_ID = " + ID;

            return _Sql;
        }


        string[] GetParentColumns(int _TableID)
        {
            //string _Sql = "select columnname from AD_field_V where ad_table_id=" + _TableID + " and isparent='Y' order by isdisplayed desc, seqno";
            //string _Sql = "SELECT * FROM AD_Column WHERE AD_Table_ID=" + _TableID + " ORDER BY ColumnName";
            //return DB.ExecuteDataset(_Sql);
            string[] keyColumns = new MTable(GetCtx(), _TableID, null).GetKeyColumns();
            return keyColumns;
        }

        String GetSql(int _TableID, string _tableName, ExportDataRecords export)
        {
            String _Sql = "";
            try
            {
                string[] ds = GetParentColumns(_TableID);
                // Code Commented by Lokesh Chauhan to get All Records for Marking those which are inactive too
               // _Sql = "SELECT * FROM " + _tableName + " WHERE IsActive = 'Y' AND ";
                _Sql = "SELECT * FROM " + _tableName + " WHERE ";

                //_Sql += ds.Tables[0].Rows[0]["ColumnName"].ToString() + " =" + export.Record_ID;
                _Sql += ds[0] + " =" + export.Record_ID;

                if (ds.Length > 1)
                {
                    // _Sql += " and " + ds.Tables[0].Rows[1]["ColumnName"].ToString() + " =" + export.AD_ColOne_ID;
                    _Sql += " and " + ds[1] + " =" + export.AD_ColOne_ID;
                }
            }
            catch
            {

            }
            return _Sql;
        }


        #region Fetch ExportData

        /// <summary>
        /// Gets the Records to be exported into Xml
        /// </summary>
        /// <returns>Records in list</returns>
        List<ExportDataRecords> GetExportData()
        {
            List<ExportDataRecords> list = new List<ExportDataRecords>();

            String _sql = "SELECT AD_Table_ID, Record_ID,AD_ColOne_ID FROM AD_ExportData WHERE IsActive = 'Y' AND AD_ModuleInfo_ID = @AD_ModuleInfo_ID";

            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@AD_ModuleInfo_ID", _AD_ModuleInfo_ID);

            IDataReader dr = DB.ExecuteReader(_sql, param);

            while (dr.Read())
            {
                ExportDataRecords records = new ExportDataRecords();
                records.AD_Table_ID = int.Parse(dr["AD_Table_ID"].ToString());
                records.Record_ID = int.Parse(dr["Record_ID"].ToString());
                if (dr["AD_ColOne_ID"] != DBNull.Value && dr["AD_ColOne_ID"] != null)
                {
                    records.AD_ColOne_ID = Convert.ToInt32(dr["AD_ColOne_ID"]);
                }
                else
                {
                    records.AD_ColOne_ID = 0;
                }

                list.Add(records);
            }
            dr.Close();


            return list;
        }

        #endregion

    }

    /// <summary>
    /// Stores record info which are to be exported into Xml
    /// </summary>
    public class ExportDataRecords
    {
        private int _AD_Table_ID = 0;

        /// <summary>
        /// Stores the table ID to be exported
        /// </summary>
        public int AD_Table_ID
        {
            get { return _AD_Table_ID; }
            set { _AD_Table_ID = value; }
        }

        private int _Record_ID = 0;

        /// <summary>
        /// Stores the Record ID (Primay Key) to be exported
        /// </summary>
        public int Record_ID
        {
            get { return _Record_ID; }
            set { _Record_ID = value; }
        }

        private int _AD_ColOne_ID = 0;

        public int AD_ColOne_ID
        {
            get { return _AD_ColOne_ID; }
            set { _AD_ColOne_ID = value; }
        }
    }
}

