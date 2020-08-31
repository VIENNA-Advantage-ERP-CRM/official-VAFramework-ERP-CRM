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
using VAdvantage.Process;
using VAdvantage.Print;
using System.Data.Common;
using System.Data.OleDb;

namespace MarketSvc
{

    /*CR_002       Don't update Other Customization Entity */
    /*            Check Export_ID and CUST Entity before updating record
                  [first,Check Table has Entity type column or not]
     
     * *CR_001    defer updatation of module version till the end of Module Installation
     
     */

    public class ImportModuleFromServer
    {
        public ImportModuleFromServer(int[] _clients)
        {
            clients = _clients;

        }

        int[] clients = null;

        /* module Id */
        private int _AD_ModuleInfo_ID = 0;

        /* Current Id */
        private int _AD_Client_ID = 0;

        /* Db Script shold run one time only in case migration is done for multuple clients*/
        private bool _isFirstClientRecordOfSchemaOrData = true;
        private bool _isLastClientRecordOfSchemaOrData = true;

        //File Path
        private String _XmlSourcePath = "";

        int NewPrimaryKey = 0;

        int OldPrimaryKey = 0;

        //Latest Dataset
        DataSet ds = null;
        DataSet dsOriginal = null;

        //Original Dataset
        DataSet dsTemp = null;

        Ctx ctx = null;

        //Log 
        StringBuilder _sbLog = null;

        //Unique table name in Migration data
        List<String> uniqueTableName = null;
        private bool _isDataMigration = true;

        /* Stores the Name of the Tables fetched from Vienna Service */
        private List<String> m_TableNameAsc = new List<string>();
        /* Stores the Name of the Tables fetched from Vienna Service */
        private List<String> m_TableName = new List<string>();
        /* Insert Query for different table */
        private List<String> m_InsertQueries = new List<string>();
        /* Update Query for different table */
        private List<String> m_UpdateQueries = new List<string>();

        /* Update Query for different table */
        private List<int> m_result = new List<int>();

        private List<String> imageCols = new List<string>();

        private List<Byte[]> mImages = new List<byte[]>();

        private List<String> accTableNames = new List<string>();

        int m_CountColumns = 0;

        private List<int> m_PrimaryKey = new List<int>();

        bool IsBinaryData = false;  //flag to check if the binary column data is there in a table

        int AD_Column_Table_ID = 0;

        //Name of the binary column in a table
        String BinaryColumnName = String.Empty;

        Object BinaryValue = null;

        List<string> _ImportInfo = new List<string>();

        String _trx = String.Empty;

        DataRow[] _tableList = null;

        List<String> _ErroLog = new List<string>();

        IMarketCallback __callback;

        object UpdatedDateInXMl = null;

        /* database MetaDataObject */
        MarketSvc.Classes.DatabaseMetaData mdObject = null;

        /* AD_Column contraints type list */
        Dictionary<int, string> _dicColumnCostarintType = null;

        Dictionary<String, Dictionary<String, List<string>>> _tableReferenceColumns = null;


        public void Init(int AD_ModuleInfo_ID, String XmlSourcePath, StringBuilder sbLog)
        {
            _sbLog = sbLog;
            _AD_ModuleInfo_ID = AD_ModuleInfo_ID;

            _XmlSourcePath = XmlSourcePath;

            //Get all the xml files into string array for parsing
            String[] XmlFiles = Directory.GetFiles(_XmlSourcePath, "*.xml", SearchOption.AllDirectories);

            dsOriginal = new DataSet();
            //  ds = new DataSet();
            uniqueTableName = new List<string>();
            _tableReferenceColumns = new Dictionary<string, Dictionary<string, List<string>>>();


            //read each xml one  by one
            foreach (String xmlfile in XmlFiles)
            {
                // Create new FileStream with which to read the schema.
                System.IO.FileStream fsReadXml = new System.IO.FileStream(xmlfile, System.IO.FileMode.Open);
                FileInfo info = new FileInfo(xmlfile);
                try
                {
                    DataTable dt = new DataTable();
                    dt.TableName = info.Name.Replace(".xml", "");

                    if (!(dt.TableName == "ExportTableSequence" || dt.TableName == "AD_Module_DB_Schema" || dt.TableName == "AD_Module_DBScript"))
                    {
                        uniqueTableName.Add(dt.TableName);
                        GetAndPrepareTableReferenceList(dt.TableName);
                    }
                    dt.ReadXml(fsReadXml);
                    dsOriginal.Tables.Add(dt);
                }
                catch (Exception ex)
                {
                    SendMessage("Fetching Schema:" + ex.Message);
                    VLogger.Get().Severe(ex.Message);
                }
                finally
                {
                    fsReadXml.Close();
                }
            }

            _dicColumnCostarintType = new Dictionary<int, string>();
        }

        private void GetAndPrepareTableReferenceList(string tableName)
        {

            List<string> displayColumnList = null;
            String keyColumnName = null;
            Dictionary<string, List<string>> displayColumnPair = null;

            DataSet ds = DB.ExecuteDataset(@" SELECT  * 
                                                FROM  
                                                    ( SELECT col.columnname, 
                                                        ( SELECT columnname FROM ad_column colmn WHERE ad_column_id =
                                                          ( SELECT reft.column_key_id FROM ad_ref_table refT
                                                            INNER JOIN ad_reference refA
                                                            ON 
                                                            ( refa.ad_reference_id = reft.ad_reference_id
                                                            ) 
                                                            WHERE refa.ad_reference_id  = col.ad_reference_value_id
                                                        )
                                                     ) AS ActualColumnName FROM ad_column col WHERE col.ad_table_id =
                                                        ( SELECT ad_table_id FROM ad_table WHERE lower(tablename) = '" + tableName.ToLower() + @"')
                                                    AND col.ad_reference_id in (18, 30) AND  lower(col.columnname) not in ('createdby', 'updatedby','entitytype')
                                                    ) tableref
                                                WHERE tableref.actualcolumnname is not null order by tableref.actualcolumnname ");

            if (ds.Tables[0].Rows.Count > 0)
            {
                displayColumnPair = new Dictionary<string, List<string>>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (dr[1].ToString() != keyColumnName)
                    {
                        keyColumnName = dr[1].ToString();

                        displayColumnList = new List<string>();

                        displayColumnList.Add(dr[0].ToString());

                        displayColumnPair.Add(keyColumnName, displayColumnList);

                    }
                    else
                    {
                        if (!displayColumnList.Contains(dr[0].ToString()))
                            displayColumnList.Add(dr[0].ToString());
                    }

                    if (displayColumnList.Contains(keyColumnName))
                    {
                        displayColumnList.Remove(keyColumnName);
                    }
                }

                if (displayColumnPair.Count > 0)
                {
                    _tableReferenceColumns.Add(tableName, displayColumnPair);
                }
            }
        }



        /// <summary>
        /// Prepare any parameter to be passed
        /// </summary>
        public void Prepare(int AD_Client_ID, bool isFirstClentID = true, bool isLastClientID = true)
        {
            _AD_Client_ID = AD_Client_ID;
            _isFirstClientRecordOfSchemaOrData = isFirstClentID;
            _isLastClientRecordOfSchemaOrData = isLastClientID;


            ds = dsOriginal.Copy();
        }

        public string CurrentModuleVersion
        {
            get;
            set;
        }


        public string DoIt(Ctx _ctx, IMarketCallback callback)
        {
            ctx = _ctx;
            //Get all the xml files into string array for parsing
            //String[] XmlFiles = Directory.GetFiles(_XmlSourcePath, "*.xml", SearchOption.AllDirectories);

            //ds = new DataSet();
            //uniqueTableName = new List<string>();


            ////read each xml one  by one
            //foreach (String xmlfile in XmlFiles)
            //{
            //    // Create new FileStream with which to read the schema.
            //    System.IO.FileStream fsReadXml = new System.IO.FileStream(xmlfile, System.IO.FileMode.Open);
            //    FileInfo info = new FileInfo(xmlfile);
            //    try
            //    {
            //        DataTable dt = new DataTable();
            //        dt.TableName = info.Name.Replace(".xml", "");

            //        if (!(dt.TableName == "ExportTableSequence" || dt.TableName == "AD_Module_DB_Schema" || dt.TableName == "AD_Module_DBScript"))
            //        {
            //            uniqueTableName.Add(dt.TableName);
            //        }
            //        dt.ReadXml(fsReadXml);
            //        ds.Tables.Add(dt);
            //    }
            //    catch (Exception ex)
            //    {
            //        VLogger.Get().Severe(ex.Message);
            //    }
            //    finally
            //    {
            //        fsReadXml.Close();
            //    }
            //}

            //Start the import process
            String _ImpStatus = ImportNow(ds, callback);

            if (dsTemp != null)
            {
                dsTemp.Clear();
                dsTemp = null;

            }

            ds.Clear();
            ds = null;
            return _ImpStatus;
        }

        #region Import Logic


        /// <summary>
        /// Creates the Insert Query
        /// </summary>
        /// <param name="tableName">Name of the Table</param>
        /// <returns>Insert Query</returns>
        private String CreateInsertQuery(String tableName)
        {
            try
            {
                MTable table = MTable.Get(GetCtx(), tableName); //get the table info
                MColumn[] columns = table.GetColumns(true);    //fetch all the columns available

                //Initials of insert query
                StringBuilder sbInsert = new StringBuilder("INSERT INTO ").Append(tableName)
                   .Append("(");


                bool first = false;
                StringBuilder sbValues = new StringBuilder();
                //loop through all columns and create insert query
                for (int cols = 0; cols <= columns.Length - 1; cols++)
                {
                    MColumn column = columns[cols];
                    if (!column.IsVirtualColumn())
                    {
                        String colName = column.GetColumnName();

                        // Changed By Lokesh Chauhan 
                        // Solved Virtual Column issue appending , at the end in case virtual column is the last column of the table
                        if (!first)
                        {
                            first = true;
                            sbInsert.Append(colName);
                            //String for column values in parameter way
                            sbValues.Append("@").Append(colName);
                            //.Append(", ");
                        }
                        else
                        {
                            sbInsert.Append(", ").Append(colName);
                            //String for column values in parameter way
                            sbValues.Append(", ").Append("@").Append(colName);

                        }
                        //if (cols == columns.Length - 1)
                        //{
                        //    //String for column values in parameter way
                        //    sbValues.Append("@").Append(colName)
                        //        .Append(")");
                        //    sbInsert.Append(colName).Append(") VALUES(")
                        //        .Append(sbValues);

                        //}
                        //else
                        //{
                        //    sbInsert.Append(colName).Append(", ");
                        //    //String for column values in parameter way
                        //    sbValues.Append("@").Append(colName)
                        //        .Append(", ");
                        //}
                    }
                }

                sbInsert.Append(") VALUES(")
                    .Append(sbValues).Append(")");

                return sbInsert.ToString();
            }
            catch (Exception ex)
            {
                SendMessage("Create Insert Query for " + tableName + "==>" + ex.Message);
                //log.Severe("ErrorCreatingSql");
            }

            return String.Empty;
        }

        private Ctx GetCtx()
        {
            return ctx;
        }


        /// <summary>
        /// Creates the Update Query
        /// </summary>
        /// <param name="tableName">Name of the Table</param>
        /// <returns>Update Query</returns>
        private String CreateUpdateQuery(String tableName)
        {
            try
            {
                int m_TotalColumns = 0;
                MTable table = MTable.Get(ctx, tableName); //get the table info
                MColumn[] columns = table.GetColumns(true);    //fetch all the columns available

                //Initials of insert query
                StringBuilder sbUpdate = new StringBuilder("UPDATE ").Append(tableName)
                   .Append(" SET ");
                bool first = false;
                StringBuilder sbValues = new StringBuilder();
                //loop through all columns and create insert query
                for (int cols = 0; cols <= columns.Length - 1; cols++)
                {

                    MColumn column = columns[cols];
                    if (!column.IsVirtualColumn())
                    {

                        //m_TotalColumns++;
                        //String colName = column.GetColumnName();

                        //sbUpdate.Append(colName).Append("=")
                        //    .Append("@").Append(colName);

                        //if (cols < columns.Length - 1)
                        //    sbUpdate.Append(", ");

                        // Changed By Lokesh Chauhan 
                        // Solved Virtual Column issue appending , at the end in case virtual column is the last column of the table
                        m_TotalColumns++;
                        String colName = column.GetColumnName();
                        if (!first)
                        {
                            first = true;
                            sbUpdate.Append(colName).Append("=")
                            .Append("@").Append(colName);
                        }
                        else
                        {
                            sbUpdate.Append(", ").Append(colName).Append("=")
                           .Append("@").Append(colName);
                        }
                    }
                }

                m_CountColumns = m_TotalColumns;

                return sbUpdate.ToString();
            }
            catch (Exception ex)
            {
                SendMessage("Create Update Query for " + tableName + "==>" + ex.Message);
                //log.Severe("ErrorCreatingSql");
            }

            return String.Empty;
        }

        System.Data.SqlClient.SqlParameter[] GetSqlParameter(MColumn[] columns, int paramcount, String tableName, int RecordID, Object result, String QueryForMultipleKeys)
        {
            System.Data.SqlClient.SqlParameter[] param = new System.Data.SqlClient.SqlParameter[paramcount];
            int pCounter = 0;

            try
            {
                Object newvalue = null;
                if (String.IsNullOrEmpty(QueryForMultipleKeys))
                {
                    newvalue = dsTemp.Get(tableName, "Export_ID", RecordID);
                }
                else
                {
                    DataRow[] ab = dsTemp.Tables[tableName].Select(QueryForMultipleKeys);
                    newvalue = ab[0]["Export_ID"];
                }

                DataRow[] Allvalue = ds.Tables[tableName].Select("Export_ID = " + DB.TO_STRING(newvalue.ToString()));

                for (int p = 0; p < columns.Length; p++)
                {
                    if (!columns[p].IsVirtualColumn())  //do not include virtual columns
                    {
                        String parameter = "@" + columns[p].GetColumnName().Replace(" ", "_");
                        Object value = DBNull.Value;

                        //try
                        //{
                        if (Allvalue[0].Table.Columns.Contains(columns[p].GetColumnName()))
                        {
                            value = Allvalue[0][columns[p].GetColumnName()];
                        }
                        else
                        {
                            SendMessage(columns[p].GetColumnName() + "Not Found");
                            //_ErroLog.Add(columns[p].GetColumnName() + "Not Found In :" + tableName);
                        }
                        //}
                        //catch (Exception ex)
                        //{
                        //    __callback.QueryExecuted(new CallBackDetail() { Status = columns[p].GetColumnName() + "Not Found" });
                        //    _ErroLog.Add(columns[p].GetColumnName() + "Not Found In :" + tableName);
                        //}

                        if (columns[p].IsDate() || columns[p].IsDateTime())
                        {
                            if (columns[p].GetColumnName() == "Updated")
                            {
                                param[pCounter] = new System.Data.SqlClient.SqlParameter(parameter, TimeUtil.GetDay((DateTime)DateTime.Now));
                                UpdatedDateInXMl = value;
                            }
                            else
                            {
                                param[pCounter] = new System.Data.SqlClient.SqlParameter(parameter, value);  //all other conditions
                            }
                            pCounter++;
                            continue;
                        }
                        else if (columns[p].IsAD_Client_ID())
                        {
                            //if (tableName.StartsWith("AD_"))        //all the tables of AD will be saved by default client and org (system)
                            //    value = 0;

                            param[pCounter] = new System.Data.SqlClient.SqlParameter(parameter, (Convert.ToInt32(value) == 0 ? 0 : _AD_Client_ID));
                            pCounter++;
                            continue;
                        }
                        else if (columns[p].IsAD_Org_ID())
                        {
                            //if (tableName.StartsWith("AD_"))        //all the tables of AD will be saved by default client and org (system)
                            //    value = 0;

                            param[pCounter] = new System.Data.SqlClient.SqlParameter(parameter, "0");
                            pCounter++;
                            continue;
                        }
                        /* change  insert super user id in destination db */
                        else if (columns[p].IsCreatedBy() || columns[p].IsUpdatedBy())
                        {
                            param[pCounter] = new System.Data.SqlClient.SqlParameter(parameter, "100");
                            pCounter++;
                            continue;
                        }

                        else if (columns[p].GetAD_Reference_ID() == DisplayType.Binary)
                        {
                            IsBinaryData = true;
                            BinaryColumnName = columns[p].GetColumnName();

                            BinaryValue = value;

                            //Binary data can not be inserted in the same process. 
                            //We will update the new inserted row after the insert process.
                            param[pCounter] = new System.Data.SqlClient.SqlParameter(parameter, DBNull.Value);
                            pCounter++;
                            continue;
                        }
                        else if (columns[p].GetColumnName() == tableName + "_ID")
                        {
                            //ad_client_id will be the one selected by user except for the AD Tables, which is 0 by default
                            var ad_client_value = ds.Get(tableName, "AD_Client_ID", RecordID);

                            if (Convert.ToInt32(ad_client_value) > 0)        //all the tables of AD will be saved by default client and org (system)
                                ad_client_value = 0;

                            NewPrimaryKey = Convert.ToInt32(result);  //for update mode, old value will be used

                            OldPrimaryKey = Convert.ToInt32(value);

                            if (columns[pCounter].IsKey())
                            {
                                if (Convert.ToInt32(result) <= 0)
                                {
                                    //Generate new primary key for the new record. (only for insert mode)
                                    NewPrimaryKey = MSequence.GetNextID(Convert.ToInt32(ad_client_value), tableName, null);
                                }
                            }

                            param[pCounter] = new System.Data.SqlClient.SqlParameter(parameter, NewPrimaryKey);
                            pCounter++;
                            continue;
                        }
                        else if (columns[p].IsYesNo() && value.Equals(DBNull.Value))
                        {
                            value = "N";
                            param[pCounter] = new System.Data.SqlClient.SqlParameter(parameter, value);  //all other conditions
                            pCounter++;
                            continue;
                        }
                        else
                        {
                            if (columns[pCounter].GetColumnName().Equals("AD_Table_ID", StringComparison.OrdinalIgnoreCase) && tableName.Equals("AD_Column", StringComparison.OrdinalIgnoreCase))
                            {
                                AD_Column_Table_ID = Convert.ToInt32(value);
                            }
                            param[pCounter] = new System.Data.SqlClient.SqlParameter(parameter, value);  //all other conditions
                        }
                    }
                    else
                    {
                        continue;
                    }

                    pCounter++;     //iterate the counter
                }
            }
            catch (Exception ex)
            {
                SendMessage(ex.Message);

                //_ErroLog.Add("Error creating sqlparameter: " + tableName);
            }


            return param;
        }

        System.Data.SqlClient.SqlParameter[] GetSqlParameterSeq(MColumn[] columns, int paramcount, String tableName, int RecordID, Object result, String QueryForMultipleKeys)
        {
            System.Data.SqlClient.SqlParameter[] param = new System.Data.SqlClient.SqlParameter[paramcount];
            int pCounter = 0;

            try
            {
                Object newvalue = null;
                if (String.IsNullOrEmpty(QueryForMultipleKeys))
                {
                    newvalue = dsTemp.Get(tableName, "Export_ID", RecordID);
                }
                else
                {
                    DataRow[] ab = dsTemp.Tables[tableName].Select(QueryForMultipleKeys);
                    newvalue = ab[0]["Export_ID"];
                }

                // DataRow[] Allvalue = ds.Tables[tableName].Select("Export_ID = " + DB.TO_STRING(newvalue.ToString()));

                DataRow[] Allvalue = null;
                if (tableName == "AD_Sequence")
                {
                    Allvalue = ds.Tables[tableName].Select("Name = " + DB.TO_STRING(result.ToString()));
                }
                else
                {
                    Allvalue = ds.Tables[tableName].Select("Export_ID = " + DB.TO_STRING(newvalue.ToString()));
                }

                for (int p = 0; p < columns.Length; p++)
                {
                    if (!columns[p].IsVirtualColumn())  //do not include virtual columns
                    {
                        String parameter = "@" + columns[p].GetColumnName().Replace(" ", "_");
                        Object value = DBNull.Value;

                        //try
                        //{
                        if (Allvalue[0].Table.Columns.Contains(columns[p].GetColumnName()))
                        {
                            value = Allvalue[0][columns[p].GetColumnName()];
                        }
                        else
                        {
                            SendMessage(columns[p].GetColumnName() + "Not Found");
                            //_ErroLog.Add(columns[p].GetColumnName() + "Not Found In :" + tableName);
                        }
                        //}
                        //catch (Exception ex)
                        //{
                        //    __callback.QueryExecuted(new CallBackDetail() { Status = columns[p].GetColumnName() + "Not Found" });
                        //    _ErroLog.Add(columns[p].GetColumnName() + "Not Found In :" + tableName);
                        //}

                        if (columns[p].IsDate() || columns[p].IsDateTime())
                        {
                            if (columns[p].GetColumnName() == "Updated")
                            {
                                param[pCounter] = new System.Data.SqlClient.SqlParameter(parameter, TimeUtil.GetDay((DateTime)DateTime.Now));
                                UpdatedDateInXMl = value;
                            }
                            else
                            {
                                param[pCounter] = new System.Data.SqlClient.SqlParameter(parameter, value);  //all other conditions
                            }
                            pCounter++;
                            continue;
                        }
                        else if (columns[p].IsAD_Client_ID())
                        {
                            //if (tableName.StartsWith("AD_"))        //all the tables of AD will be saved by default client and org (system)
                            //    value = 0;

                            param[pCounter] = new System.Data.SqlClient.SqlParameter(parameter, (Convert.ToInt32(value) == 0 ? 0 : _AD_Client_ID));
                            pCounter++;
                            continue;
                        }
                        else if (columns[p].IsAD_Org_ID())
                        {
                            //if (tableName.StartsWith("AD_"))        //all the tables of AD will be saved by default client and org (system)
                            //    value = 0;

                            param[pCounter] = new System.Data.SqlClient.SqlParameter(parameter, "0");
                            pCounter++;
                            continue;
                        }
                        /* change  insert super user id in destination db (createdby and updated by) */
                        else if (columns[p].IsCreatedBy() || columns[p].IsUpdatedBy())
                        {
                            param[pCounter] = new System.Data.SqlClient.SqlParameter(parameter, "100");
                            pCounter++;
                            continue;
                        }


                        else if (columns[p].GetAD_Reference_ID() == DisplayType.Binary)
                        {
                            IsBinaryData = true;
                            BinaryColumnName = columns[p].GetColumnName();

                            BinaryValue = value;

                            //Binary data can not be inserted in the same process. 
                            //We will update the new inserted row after the insert process.
                            param[pCounter] = new System.Data.SqlClient.SqlParameter(parameter, DBNull.Value);
                            pCounter++;
                            continue;
                        }
                        else if (columns[p].GetColumnName() == tableName + "_ID")
                        {
                            //ad_client_id will be the one selected by user except for the AD Tables, which is 0 by default
                            var ad_client_value = ds.Get(tableName, "AD_Client_ID", RecordID);

                            if (Convert.ToInt32(ad_client_value) > 0)        //all the tables of AD will be saved by default client and org (system)
                                ad_client_value = 0;

                            if (tableName == "AD_Sequence")
                            {
                                // In case of AD_Sequence Create New Record
                                NewPrimaryKey = Convert.ToInt32(0);  //for update mode, old value will be used
                            }
                            else
                            {
                                NewPrimaryKey = Convert.ToInt32(result);  //for update mode, old value will be used
                            }
                            OldPrimaryKey = Convert.ToInt32(value);

                            if (columns[pCounter].IsKey())
                            {
                                if (Convert.ToInt32(NewPrimaryKey) <= 0)
                                {
                                    //Generate new primary key for the new record. (only for insert mode)
                                    NewPrimaryKey = MSequence.GetNextID(Convert.ToInt32(ad_client_value), tableName, null);
                                }
                            }

                            param[pCounter] = new System.Data.SqlClient.SqlParameter(parameter, NewPrimaryKey);
                            pCounter++;
                            continue;
                        }
                        else if (columns[p].IsYesNo() && value.Equals(DBNull.Value))
                        {
                            value = "N";
                            param[pCounter] = new System.Data.SqlClient.SqlParameter(parameter, value);  //all other conditions
                            pCounter++;
                            continue;
                        }
                        else
                        {
                            if (columns[pCounter].GetColumnName().Equals("AD_Table_ID", StringComparison.OrdinalIgnoreCase) && tableName.Equals("AD_Column", StringComparison.OrdinalIgnoreCase))
                            {
                                AD_Column_Table_ID = Convert.ToInt32(value);
                            }
                            param[pCounter] = new System.Data.SqlClient.SqlParameter(parameter, value);  //all other conditions
                        }
                    }
                    else
                    {
                        continue;
                    }

                    pCounter++;     //iterate the counter
                }
            }
            catch (Exception ex)
            {
                SendMessage(ex.Message);

                //_ErroLog.Add("Error creating sqlparameter: " + tableName);
            }


            return param;
        }

        /// <summary>
        /// Executes the script
        /// </summary>
        /// <param name="script">Script to be executed</param>
        /// <returns></returns>
        int ExecuteScript(DataRow[] scripts)
        {
            try
            {
                foreach (DataRow row in scripts)
                {
                    // Code Changed to Execute Multiple Scripts and Delimiter set @SQL=
                    string[] queryToExecute = row["Script"].ToString().Split(new string[] { "@SQL=" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < queryToExecute.Length; i++)
                    {
                        int __query = DB.ExecuteQuery(queryToExecute[i]);

                        SendMessage("Script excecuted <===> " + __query);


                    }

                    //int __query = DB.ExecuteQuery(row["Script"].ToString());
                    //__callback.QueryExecuted(new CallBackDetail() { Status = "Script excecuted <===> " + __query });

                }
            }
            catch (Exception ex)
            {
                SendMessage(ex.Message);
                //_ErroLog.Add(ex.Message);
            }

            return 0;
        }





        String ModuleParentFolder = String.Empty;
        String ModuleParentExportID = String.Empty;
        int ModuleMenuParentID = 0;


        private void ProcessMenu(DataRow[] data, String tableName, String ExportID)
        {
            //Add item to the menu. (only for process, form and window)
            if (tableName.Equals("AD_ModuleWindow", StringComparison.OrdinalIgnoreCase) ||
                tableName.Equals("AD_ModuleProcess", StringComparison.OrdinalIgnoreCase) ||
                tableName.Equals("AD_ModuleForm", StringComparison.OrdinalIgnoreCase))
            {
                DataRow[] newvalue = dsTemp.Tables[tableName].Select("Export_ID = " + DB.TO_STRING(ExportID));

                DataRow[] Allvalue = ds.Tables[tableName].Select("Export_ID = " + DB.TO_STRING(newvalue[0]["Export_ID"].ToString()));
                //Object value = Allvalue[0][columns[p].GetColumnName()];
                String ParentExportID = String.Empty;
                if (Allvalue[0]["IsMenuItem"].ToString().Equals("Y"))
                {
                    SendMessage("Adding to menu");
                    String ParentName = String.Empty;
                    String SubParentName = String.Empty;
                    String MenuItem = String.Empty;
                    int seqNo = -1;
                    DataRow[] menurows = null;

                    if (Allvalue[0]["AD_ModuleMenuFolder_ID"] != DBNull.Value)
                    {
                        menurows = dsTemp.Tables["AD_ModuleMenuFolder"].Select("AD_ModuleMenuFolder_ID = " + Convert.ToInt32(newvalue[0]["AD_ModuleMenuFolder_ID"].ToString()));
                        ParentExportID = menurows[0]["Export_ID"].ToString();
                        SubParentName = menurows[0]["Name"].ToString();
                    }
                    //DataRow[] menurows = ds.Tables["AD_ModuleMenuFolder"].Select("Export_ID = " + DB.TO_STRING(Allvalue[0]["Export_ID"].ToString()));

                    ParentName = ModuleParentFolder;

                    MenuItem = Allvalue[0]["MenuItem"].ToString();


                    if (Allvalue[0].Table.Columns.Contains("SeqNo"))
                    {
                        seqNo = Util.GetValueOfInt(Allvalue[0]["SeqNo"].ToString());
                    }
                    else
                    {
                        seqNo = 9999;
                    }
                    //seqNo = Util.GetValueOfInt(Allvalue[0]["seqNo"].ToString());
                    int IDValue = 0;

                    if (tableName.Equals("AD_ModuleWindow"))
                        IDValue = Convert.ToInt32(Allvalue[0]["AD_Window_ID"].ToString());
                    else if (tableName.Equals("AD_ModuleProcess"))
                        IDValue = Convert.ToInt32(Allvalue[0]["AD_Process_ID"].ToString());
                    else if (tableName.Equals("AD_ModuleForm"))
                        IDValue = Convert.ToInt32(Allvalue[0]["AD_Form_ID"].ToString());

                    String menuitem_exportid = data[0]["MenuItemExport_ID"].ToString();
                    AddWindowToMenu(tableName, IDValue, ParentName, SubParentName, MenuItem, menuitem_exportid, ParentExportID, seqNo);

                }

            }

        }

        #region ImportNowOld
        //        String ImportNow(DataSet ds, IMarketCallback callback)
        //        {
        //            __callback = callback;
        //            //no import process will be run if dataset is empty
        //            if (ds.Tables.Count <= 0)
        //                return "XmlNotLoaded";

        //            //_trx = Get_Trx().GetTrxName();   // work with transaction

        //            String SeqTable = "ExportTableSequence";

        //            if (!ds.Tables.Contains(SeqTable))
        //            {
        //                _isDataMigration = false;
        //                SeqTable = "AD_Module_DB_Schema";
        //            }
        //            else
        //            {
        //                _isDataMigration = true;
        //            }

        //            _tableList = ds.Tables[SeqTable].Select();       //fetch all the tables from dataset having xml data



        //            // Changed Code to Execute Scripts based on the column IsPreExecuteScript
        //            // if Checked then execute checked Script before DB Schema
        //            if (ds.Tables.Contains("AD_Module_DBScript") && _isFirstClientRecordOfSchemaOrData) // run only one time in case of multiple clients are selected
        //            {
        //                if (ds.Tables["AD_Module_DBScript"].Columns["IsPreExecuteScript"] != null)
        //                {
        //                    DataRow[] scripts = ds.Tables["AD_Module_DBScript"].Select("IsPreExecuteScript" + " = 'Y' ");
        //                    int excecuted = ExecuteScript(scripts);
        //                }
        //            }

        //            //InsertOrUpdate function 
        //            List<int> m_ExecutedID = new List<int>();
        //            List<string> m_ExecutedTable = new List<string>();

        //            dsTemp = ds.Copy();

        //            Dictionary<String, String> __generatedInsertQueries = new Dictionary<string, string>();
        //            Dictionary<String, String> __generatedUpdateQueries = new Dictionary<string, string>();
        //            Dictionary<String, int> __ColumnCount = new Dictionary<String, int>();

        //            StringBuilder sbMsg = new StringBuilder("");


        //            for (int tables = _tableList.Count() - 1; tables >= 0; tables--)
        //            {
        //                sbMsg.Clear();

        //                String tableName = _tableList[tables]["TableName"].ToString();  //extract the table name to be processed

        //                if (tableName.Equals("AD_Ref_Table", StringComparison.OrdinalIgnoreCase))
        //                    continue;

        //                int RecordID = int.Parse(_tableList[tables]["Record_ID"].ToString());

        //                //sbMsg.Append("Processing table " + tableName + " <===> " + RecordID);
        //                //SendMessage("<br />Processing table " + tableName + " <===> " + RecordID);

        //                m_TableName.Add(tableName);

        //                String _insertQuery = String.Empty;
        //                String _updateQuery = String.Empty;

        //                if (__generatedUpdateQueries.ContainsKey(tableName))
        //                {
        //                    _insertQuery = __generatedInsertQueries[tableName];
        //                    _updateQuery = __generatedUpdateQueries[tableName];
        //                    m_CountColumns = __ColumnCount[tableName];
        //                }
        //                else
        //                {
        //                    _insertQuery = CreateInsertQuery(tableName);      //create insert query for the table
        //                    _updateQuery = CreateUpdateQuery(tableName);      //create update query for the table

        //                }

        //                String PrimaryColumn = tableName + "_ID";

        //                String Export_ID = "";
        //                try
        //                {
        //                    DataRow[] tempData = null;
        //                    DataRow[] data = null;
        //                    String QueryForMultipleParents = String.Empty;

        //                    /*CR_002*/
        //                    bool hasEntityTypeColumn = false;

        //                    bool multiplekeys = false;
        //                    if (!dsTemp.Tables[tableName].Columns.Contains(PrimaryColumn))
        //                    {
        //                        multiplekeys = true;
        //                        String[] pcolumns = MTable.Get(Env.GetCtx(), tableName).GetKeyColumns();
        //                        int AD_ColOne_ID = int.Parse(_tableList[tables]["AD_ColOne_ID"].ToString());

        //                        QueryForMultipleParents = pcolumns[0] + " = " + RecordID + " AND " + pcolumns[1] + " = " + AD_ColOne_ID;
        //                        tempData = dsTemp.Tables[tableName].Select(QueryForMultipleParents);
        //                    }
        //                    else
        //                    {
        //                        tempData = dsTemp.Tables[tableName].Select(PrimaryColumn + " = " + RecordID);
        //                    }

        //                    Export_ID = tempData[0]["Export_ID"].ToString();        //fetch export id from the data row available
        //                    //DataRow[] data = ds.Tables[tableName].Select(PrimaryColumn + " = " + RecordID);

        //                    hasEntityTypeColumn = dsTemp.Tables[tableName].Columns.Contains("EntityType");

        //                    if (multiplekeys)
        //                    {
        //                        data = dsTemp.Tables[tableName].Select(QueryForMultipleParents + " AND Export_ID = " + DB.TO_STRING(Export_ID));
        //                    }
        //                    else
        //                    {
        //                        data = ds.Tables[tableName].Select(PrimaryColumn + " = " + RecordID + " AND Export_ID = " + DB.TO_STRING(Export_ID));
        //                    }

        //                    if (data.Count() > 0)
        //                    {
        //                        Export_ID = data[0]["Export_ID"].ToString();        //fetch export id from the data row available
        //                        String TempAD_Client_ID = data[0]["AD_Client_ID"].ToString();

        //                        StringBuilder sbEntityTypeCheck = null;
        //                        StringBuilder sbColumnConstaintCheck = null;

        //                        if (hasEntityTypeColumn)
        //                        {
        //                            sbEntityTypeCheck = new StringBuilder("SELECT ")
        //                                /*OR_2*///.Append(multiplekeys ? "Count(1) as PrimaryColumn" : PrimaryColumn) //Karan
        //                            .Append(" EntityType ") // select Id Column to pass [null or DbNull.value] check
        //                                        .Append(" FROM ")
        //                                        .Append(tableName)
        //                                        .Append(" WHERE Export_ID = ")
        //                                        .Append(DB.TO_STRING(Export_ID));
        //                        }

        //                        //check if the record with the extracted export id already exisits
        //                        StringBuilder sbCheck = new StringBuilder("SELECT ")
        //                            /*OR_2*///.Append(multiplekeys ? "Count(1) as PrimaryColumn" : PrimaryColumn) //Karan
        //                            .Append(multiplekeys ? "AD_Client_ID as PrimaryColumn" : PrimaryColumn) // select Id Column to pass [null or DbNull.value] check
        //                                        .Append(" FROM ")
        //                                        .Append(tableName)
        //                                        .Append(" WHERE Export_ID = ")
        //                                        .Append(DB.TO_STRING(Export_ID));

        //                        if (SeqTable == "ExportTableSequence")
        //                        {
        //                            sbCheck.Append(" AND AD_Client_ID = ");

        //                            if (hasEntityTypeColumn)
        //                            {
        //                                sbEntityTypeCheck.Append(" AND AD_Client_ID = ");
        //                            }


        //                            //if current record client id is 0 then record insert in to client id 0
        //                            if (TempAD_Client_ID.Equals("0"))
        //                            {
        //                                sbCheck.Append("0");

        //                                if (hasEntityTypeColumn)
        //                                {
        //                                    sbEntityTypeCheck.Append(" 0 ");
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!TempAD_Client_ID.Equals("0"))
        //                                {
        //                                    if (_AD_Client_ID.Equals(0))
        //                                    {
        //                                        continue;
        //                                    }
        //                                }
        //                                sbCheck.Append(_AD_Client_ID);

        //                                if (hasEntityTypeColumn)
        //                                {
        //                                    sbEntityTypeCheck.Append(_AD_Client_ID);
        //                                }

        //                            }
        //                        }
        //                        else
        //                        {

        //                            if (!TempAD_Client_ID.Equals("0")) //in case schema if record client is not system
        //                            {
        //                                if (_AD_Client_ID.Equals(0)) //current client is system then skip the record 
        //                                {
        //                                    continue;
        //                                }
        //                            }

        //                            sbCheck.Append(" AND (AD_Client_ID = 0 OR AD_Client_ID = ").Append(_AD_Client_ID).Append(")");

        //                            if (hasEntityTypeColumn)
        //                            {
        //                                sbEntityTypeCheck.Append(" AND (AD_Client_ID = 0 OR AD_Client_ID = ").Append(_AD_Client_ID).Append(")");
        //                            }
        //                        }



        //                        //For AD_Column, we need to check it with its name 
        //                        //as standard columns would have already been created by now
        //                        if (tableName.Equals("AD_Column", StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            String __ColumnName = data[0]["ColumnName"].ToString();
        //                            String __TableID = data[0]["AD_Table_ID"].ToString();

        //                            Object r1 = DB.ExecuteScalar(sbCheck.ToString(), null, null);

        //                            if (r1 == null)
        //                            {
        //                                sbCheck = new StringBuilder("SELECT ")
        //                                            .Append(PrimaryColumn)
        //                                            .Append(" FROM ")
        //                                            .Append(tableName)
        //                                            .Append(" WHERE ColumnName = ")
        //                                            .Append(DB.TO_STRING(__ColumnName))
        //                                            .Append(" AND AD_Table_ID = ")
        //                                            .Append(__TableID);
        //                                //.Append(" AND Export_ID = ")
        //                                //.Append(DB.TO_STRING(Export_ID));

        //                            }


        //                            sbColumnConstaintCheck = new StringBuilder("SELECT AD_Column_ID, ConstraintType ").Append(sbCheck.ToString().Substring(sbCheck.ToString().IndexOf("FROM")));

        //                        }
        //                        else if (tableName.Equals("AD_Element", StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            String __ColumnName = data[0]["ColumnName"].ToString();

        //                            Object r1 = DB.ExecuteScalar(sbCheck.ToString(), null, null);

        //                            if (r1 == null)
        //                            {
        //                                sbCheck = new StringBuilder("SELECT ")
        //                                            .Append(PrimaryColumn)
        //                                            .Append(" FROM ")
        //                                            .Append(tableName)
        //                                            .Append(" WHERE ColumnName = ")
        //                                            .Append(DB.TO_STRING(__ColumnName));
        //                            }
        //                        }
        //                        else if (tableName.Equals("AD_Field", StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            String __ColumnID = data[0]["AD_Column_ID"].ToString();
        //                            String __TabID = data[0]["AD_Tab_ID"].ToString();

        //                            Object r1 = DB.ExecuteScalar(sbCheck.ToString(), null, null);

        //                            if (r1 == null)
        //                            {
        //                                sbCheck = new StringBuilder("SELECT ")
        //                                            .Append(PrimaryColumn)
        //                                            .Append(" FROM ")
        //                                            .Append(tableName)
        //                                            .Append(" WHERE AD_Column_ID = ")
        //                                            .Append(__ColumnID)
        //                                            .Append(" AND AD_Tab_ID = ")
        //                                            .Append(__TabID);
        //                            }
        //                        }

        //                        else if (tableName.Equals("AD_ModuleInfo", StringComparison.OrdinalIgnoreCase) && !_isDataMigration)
        //                        {
        //                            data[0]["VersionNo"] = CurrentModuleVersion;
        //                            data[0].EndEdit();
        //                        }


        //                        Object result = DB.ExecuteScalar(sbCheck.ToString(), null, null);

        //                        MTable mTable = MTable.Get(GetCtx(), tableName);    //Get table info
        //                        MColumn[] columns = mTable.GetColumns(true);    //Get all the columns of the selected table

        //                        //create sql parameters
        //                        System.Data.SqlClient.SqlParameter[] param = GetSqlParameter(columns, m_CountColumns, tableName, RecordID, result, QueryForMultipleParents);

        //                        if (result != null && result != DBNull.Value) //update
        //                        {

        //                            try
        //                            {

        //                                // Get Old Constraint type setting 
        //                                if (tableName.Equals("AD_Column", StringComparison.OrdinalIgnoreCase))
        //                                {
        //                                    IDataReader dr = DB.ExecuteReader(sbColumnConstaintCheck.ToString(), null);
        //                                    if (dr.Read())
        //                                    {
        //                                        int ad_col_id = VAdvantage.Utility.Util.GetValueOfInt(dr[0]);
        //                                        if (!_dicColumnCostarintType.Keys.Contains(ad_col_id))
        //                                        {
        //                                            _dicColumnCostarintType[ad_col_id] = VAdvantage.Utility.Util.GetValueOfString(dr[1]);
        //                                        }
        //                                    }
        //                                    dr.Close();
        //                                }

        //                                //SendMessage("Updating table " + tableName);
        //                                //sbMsg.Append("<br />").Append("Updating table " + tableName);
        //                                //Append where query for a particular record
        //                                String whereQuery = " WHERE " + tableName + "_ID = " + result;
        //                                if (multiplekeys)
        //                                {
        //                                    String[] pcolumns = MTable.Get(Env.GetCtx(), tableName).GetKeyColumns();
        //                                    String value_1 = param.First(s => s.ParameterName.Equals("@" + pcolumns[0])).Value.ToString();
        //                                    String value_2 = param.First(s => s.ParameterName.Equals("@" + pcolumns[1])).Value.ToString();
        //                                    whereQuery = " WHERE " + pcolumns[0].Replace("@", "") + " = " + value_1 + " AND " + pcolumns[1].Replace("@", "") + " = " + value_2;
        //                                }

        //                                _updateQuery += whereQuery;

        //                                if (tableName.Equals("AD_Workflow"))
        //                                    ManageWorkflowTables(tableName, ref param, false); //for AD_Workflow

        //                                //bool skipUpdate = false;
        //                                //if (hasEntityTypeColumn)
        //                                //{
        //                                //    Object eType = DB.ExecuteScalar(sbEntityTypeCheck.ToString(), null, null);
        //                                //    if (eType != null && eType.ToString().Equals("CUST"))
        //                                //    {
        //                                //        SendMessage("Skip updation, Customized record, table " + tableName);
        //                                //        skipUpdate = true;
        //                                //    }
        //                                //}

        //                                bool skipUpdate = false;

        //                                bool? updateRecentED = null; //if null then skip updateor insert in ad_RecentExportData
        //                                //if fale then Insert 
        //                                //if true then Update

        //                                //Check Content From AD_RecentExportData
        //                                //Is there any need to update 
        //                                List<DataRow> recentUpdated = (dsExportedData.Tables[0].Select("AD_Table_ID=" + mTable.GetAD_Table_ID() + " AND Record_ID=" + result + " AND AD_Client_ID=" + _AD_Client_ID)).ToList();
        //                                if (recentUpdated.Count > 0) //Check Last Updated Date
        //                                {
        //                                    if ((Convert.ToDateTime(UpdatedDateInXMl).ToString("MM/dd/yyyy HH:mm:ss")).Equals(Convert.ToDateTime(recentUpdated[0]["UPDATEDDATE"]).ToString("MM/dd/yyyy HH:mm:ss")))
        //                                    {
        //                                        SendMessage("Skip updation, Allready Updated, table " + tableName);
        //                                        skipUpdate = true;
        //                                        updateRecentED = null;
        //                                    }
        //                                    else
        //                                    {
        //                                        updateRecentED = true;
        //                                    }
        //                                }
        //                                else //insert in AD_RecentExportData
        //                                {
        //                                    updateRecentED = false;
        //                                }

        //                                if (hasEntityTypeColumn && !skipUpdate)
        //                                {
        //                                    Object eType = DB.ExecuteScalar(sbEntityTypeCheck.ToString(), null, null);
        //                                    if (eType != null && eType.ToString().Equals("CUST"))
        //                                    {
        //                                        SendMessage("Skip updation, Customized record, table " + tableName);
        //                                        skipUpdate = true;
        //                                    }
        //                                }


        //                                //int update = skipUpdate ? 0 : DB.ExecuteQuery(_updateQuery, param, null);
        //                                int update = 0;
        //                                if (!skipUpdate)
        //                                {
        //                                    sbMsg.Append("Processing table " + tableName + " <===> " + RecordID);
        //                                    SendMessage("<br />Processing table " + tableName + " <===> " + RecordID);

        //                                    sbMsg.Append("<br />").Append("Updating table " + tableName);
        //                                    SendMessage("Updating table " + tableName);
        //                                    update = DB.ExecuteQuery(_updateQuery, param, null);
        //                                }
        //                                //Insert Or Update AD_RecentExportDate
        //                                if (update > 0 && updateRecentED != null)
        //                                {
        //                                    if (updateRecentED == true)
        //                                    {
        //                                        DB.ExecuteQuery(@"UPDATE AD_RecentExportData SET UPDATEDDATE=to_date('" + Convert.ToDateTime(UpdatedDateInXMl).ToString("MM/dd/yyyy HH:mm:ss") + @"','MM/dd/yyyy HH24:MI:SS')
        //                                                                  WHERE AD_CLIENT_ID=" + _AD_Client_ID + " AND AD_TABLE_ID=" + mTable.GetAD_Table_ID() + " AND RECORD_ID =" + result);

        //                                    }
        //                                    else if (updateRecentED == false)
        //                                    {
        //                                        DB.ExecuteQuery("INSERT INTO AD_RecentExportData(AD_CLIENT_ID,AD_TABLE_ID,RECORD_ID,UPDATEDDATE) VALUES(" + _AD_Client_ID + "," + mTable.GetAD_Table_ID() + "," + result + ",to_date('" + Convert.ToDateTime(UpdatedDateInXMl).ToString("MM/dd/yyyy HH:mm:ss") + "','MM/dd/yyyy HH24:MI:SS'))");
        //                                    }
        //                                }

        //                                ValueNamePair error = VLogger.RetrieveError();
        //                                if (error == null || update > 0 || skipUpdate)
        //                                {
        //                                    if (!multiplekeys)
        //                                    {
        //                                        if (tableName.Equals("AD_ModuleMenuFolder", StringComparison.OrdinalIgnoreCase))
        //                                        {
        //                                            if (data[0]["IsParentFolder"].Equals("Y"))
        //                                            {
        //                                                ModuleParentFolder = data[0]["Name"].ToString();
        //                                                ModuleParentExportID = data[0]["Export_ID"].ToString();

        //                                                MMenu[] arrmenu = MMenu.Get(ctx, "Export_ID = " + DB.TO_STRING(ModuleParentExportID));
        //                                                MMenu menu = null;
        //                                                if (arrmenu.Length <= 0)
        //                                                {
        //                                                    menu = new MMenu(ctx, 0, null);
        //                                                }
        //                                                else
        //                                                {
        //                                                    menu = arrmenu[0];
        //                                                }
        //                                                menu.SetName(ModuleParentFolder);
        //                                                menu.SetIsSummary(true);
        //                                                menu.SetExport_ID(ModuleParentExportID);
        //                                                bool b = menu.Save();

        //                                                if (b)
        //                                                    ModuleMenuParentID = menu.Get_ID();
        //                                            }
        //                                        }

        //                                        if (tableName.Equals("AD_WF_Node"))
        //                                            ManageWorkflowTables(tableName, ref param, true); //for AD_Workflow
        //                                        //if (tableName.Equals("AD_WF_NodeNext"))
        //                                        //    ManageWorkflowTables(tableName, ref param, true); //for AD_Workflow


        //                                        //_ImportInfo.Add("Updated table " + tableName + ": Done");

        //                                        //Add item to the menu. (only for process, form and window)
        //                                        ProcessMenu(data, tableName, Export_ID);
        //                                    }


        //                                    if (update != 0)
        //                                    {
        //                                        sbMsg.Append("<br />").Append("Updated table " + tableName + ": Done");
        //                                        SendMessage("Updated table " + tableName + ": Done");
        //                                    }
        //                                    if (!multiplekeys)
        //                                        UpdateDependencies(PrimaryColumn, tableName, tables, false);
        //                                }
        //                                else
        //                                {
        //                                    //sbMsg.Clear();
        //                                    sbMsg.Append("<br />").Append("Error Updating " + tableName + ": " + error.GetName());
        //                                    SendMessage("Error Updating " + tableName + ": " + error.GetName());
        //                                    //_ImportInfo.Add("Error Updating " + tableName + ": " + error.GetName());

        //                                }

        //                                //update binay record
        //                                if (update > 0 && IsBinaryData)
        //                                {

        //                                    String paramName = "@" + BinaryColumnName;
        //                                    System.Data.SqlClient.SqlParameter[] prm = new System.Data.SqlClient.SqlParameter[1];
        //                                    prm[0] = new System.Data.SqlClient.SqlParameter(paramName, BinaryValue);

        //                                    _updateQuery = "UPDATE " + tableName + " SET " + BinaryColumnName + " = " + paramName + whereQuery;
        //                                    update = DB.ExecuteQuery(_updateQuery, prm, null);

        //                                    //if (update <= 0)
        //                                    //    log.Fine("BlobNotLoaded");

        //                                    IsBinaryData = false;
        //                                }







        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                sbMsg.Append("<br />").Append(ex.Message);
        //                                SendMessage(ex.Message);
        //                                //_ErroLog.Add("Error updating " + tableName + ": " + ex.Message);
        //                                //log.Severe(ex.Message);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            sbMsg.Append("Processing table " + tableName + " <===> " + RecordID);
        //                            SendMessage("<br />Processing table " + tableName + " <===> " + RecordID);

        //                            try
        //                            {
        //                                sbMsg.Append("<br />").Append("Inserting into table " + tableName + " <===> " + NewPrimaryKey);
        //                                SendMessage("Inserting into table " + tableName + " <===> " + NewPrimaryKey);
        //                                //AD_Workflow contains an ID AD_WF_Node_ID which has a circular dependency
        //                                //AD_Workflow ==> AD_WF_Node_ID;
        //                                //AD_WF_Node  ==> AD_Workflow_ID
        //                                //We can not insert data into ad_workflow with wf_node_id as wf_node_id is not created at 
        //                                //the time of insertion. So we will nullify the wf_node_id and update it later at the time
        //                                //of insertion into AD_WF_Node table.
        //                                if (tableName.Equals("AD_Workflow"))
        //                                    ManageWorkflowTables(tableName, ref param, false); //for AD_Workflow
        //                                //Insert Mode

        //                                int insert = 0;

        //                                if (tableName.Equals("AD_Table", StringComparison.OrdinalIgnoreCase)
        //                                    || tableName.Equals("AD_Tab", StringComparison.OrdinalIgnoreCase))
        //                                {
        //                                    insert = DB.ExecuteQuery(_insertQuery, param);

        //                                    ReportEngine_N re = null;
        //                                    byte[] report = null;
        //                                    ProcessCtl process = new ProcessCtl();
        //                                    if (tableName.Equals("AD_Table", StringComparison.OrdinalIgnoreCase))
        //                                    {



        //                                        //callback.QueryExecuted(new CallBackDetail() { Status = "Standard columns will be created" });
        //                                        //ProcessInfo info = new ProcessInfo("CreateTable", 173, 100, NewPrimaryKey);
        //                                        //process.Process(info, GetCtx(), out report, out re);
        //                                    }
        //                                    else if (tableName.Equals("AD_Tab", StringComparison.OrdinalIgnoreCase))
        //                                    {
        //                                        SendMessage("Standard fields will be created");
        //                                        sbMsg.Append("<br />").Append("Standard fields will be created");

        //                                        ProcessInfo info = new ProcessInfo("CreateFields", 174, 106, NewPrimaryKey);
        //                                        process.Process(info, GetCtx(), out report, out re);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    insert = DB.ExecuteQuery(_insertQuery, param, null);
        //                                }

        //                                if (insert > 0)
        //                                {

        //                                    if (tableName.Equals("AD_ModuleMenuFolder", StringComparison.OrdinalIgnoreCase))
        //                                    {
        //                                        if (data[0]["IsParentFolder"].Equals("Y"))
        //                                        {
        //                                            ModuleParentFolder = data[0]["Name"].ToString();
        //                                            ModuleParentExportID = data[0]["Export_ID"].ToString();

        //                                            MMenu menu = new MMenu(ctx, 0, null);
        //                                            menu.SetName(ModuleParentFolder);
        //                                            menu.SetIsSummary(true);
        //                                            menu.SetExport_ID(ModuleParentExportID);
        //                                            bool b = menu.Save();

        //                                            if (b)
        //                                                ModuleMenuParentID = menu.Get_ID();
        //                                        }
        //                                    }
        //                                    sbMsg.Append("<br />").Append("Inserted into " + tableName);
        //                                    SendMessage("Inserted into " + tableName);

        //                                    if (!multiplekeys)
        //                                    {
        //                                        UpdateDependencies(PrimaryColumn, tableName, tables, true); //update the dependency on other tables

        //                                        if (IsBinaryData)
        //                                        {
        //                                            String paramName = "@" + BinaryColumnName;
        //                                            String whereQuery = " WHERE " + tableName + "_ID = " + NewPrimaryKey;
        //                                            System.Data.SqlClient.SqlParameter[] prm = new System.Data.SqlClient.SqlParameter[1];
        //                                            prm[0] = new System.Data.SqlClient.SqlParameter(paramName, BinaryValue);

        //                                            _updateQuery = "UPDATE " + tableName + " SET " + BinaryColumnName + " = " + paramName + whereQuery;
        //                                            DB.ExecuteQuery(_updateQuery, prm, null);

        //                                            //if (update <= 0)
        //                                            //    log.Fine("BlobNotLoaded");

        //                                            IsBinaryData = false;
        //                                        }

        //                                        //if (tableName.Equals("AD_Module_DBScript"))
        //                                        //{
        //                                        //    int excecuted = ExecuteScript(data[0]["Script"].ToString());
        //                                        //    callback.QueryExecuted(new CallBackDetail() { Status = "Script excecuted <===> " + excecuted });
        //                                        //}

        //                                        if (tableName.Equals("AD_Table"))
        //                                            MSequence.CreateTableSequence(GetCtx(), data[0]["TableName"].ToString(), null);

        //                                        if (tableName.Equals("AD_WF_Node"))
        //                                            ManageWorkflowTables(tableName, ref param, true); //for AD_Workflow

        //                                        //_ImportInfo.Add("Inserted new record in " + tableName);
        //                                        //SendMessage("Inserted new record in " + tableName);
        //                                        //sbMsg.Append("<br />").Append("Inserted new record in " + tableName);

        //                                        //Add item to the menu. (only for process, form and window)
        //                                        ProcessMenu(tempData, tableName, Export_ID);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    ValueNamePair error = VLogger.RetrieveError();
        //                                    String errorText = "";
        //                                    if (error != null)
        //                                        errorText = error.GetName();

        //                                    sbMsg.Append("<br />").Append("Error in table " + tableName + " <===> " + errorText);
        //                                    SendMessage("Error in table " + tableName + " <===> " + errorText);
        //                                    //_ImportInfo.Add("Error Inserting in " + tableName + ": " + errorText);
        //                                }
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                sbMsg.Append("<br />").Append(ex.Message);
        //                                SendMessage(ex.Message);
        //                                //_ErroLog.Add("Error inserting " + tableName + ": " + ex.Message);
        //                            }
        //                        }

        //                        // IDataReader dr = null;

        //                        if (tableName.Equals("AD_Process", StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            //Assing role to the new menu created
        //                            IDataReader dr = GetRoles();
        //                            bool saved = false;
        //                            while (dr.Read())
        //                            {
        //                                X_AD_Process_Access access = new X_AD_Process_Access(GetCtx(), 0, null);
        //                                access.SetAD_Process_ID(NewPrimaryKey);
        //                                access.SetAD_Role_ID(int.Parse(dr[0].ToString()));
        //                                access.SetIsReadWrite(true);
        //                                saved = access.Save();
        //                            }
        //                            dr.Close();
        //                        }


        //                        if (tableName.Equals("AD_Form", StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            //Assing role to the new menu created

        //                            IDataReader dr = GetRoles();
        //                            bool saved = false;
        //                            while (dr.Read())
        //                            {
        //                                X_AD_Form_Access access = new X_AD_Form_Access(GetCtx(), 0, null);
        //                                access.SetAD_Form_ID(NewPrimaryKey);
        //                                access.SetAD_Role_ID(int.Parse(dr[0].ToString()));
        //                                access.SetIsReadWrite(true);
        //                                saved = access.Save();

        //                            }
        //                            dr.Close();
        //                        }

        //                        //assign role acces to workflow
        //                        if (tableName.Equals("AD_Workflow", StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            //Assing role to the new menu created

        //                            IDataReader dr = GetRoles();
        //                            bool saved = false;
        //                            while (dr.Read())
        //                            {
        //                                X_AD_Workflow_Access access = new X_AD_Workflow_Access(GetCtx(), 0, null);
        //                                access.SetAD_Workflow_ID(NewPrimaryKey);
        //                                access.SetAD_Role_ID(int.Parse(dr[0].ToString()));
        //                                access.SetIsReadWrite(true);
        //                                saved = access.Save();
        //                            }
        //                            dr.Close();
        //                        }



        //                        if (tableName.Equals("AD_Column", StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            // Commented Code
        //                            // As Primary key was not unique  if we sync column here

        //                            // DoColumnSync(NewPrimaryKey);
        //                        }

        //                        //Reset the primary values
        //                        NewPrimaryKey = 0;
        //                        OldPrimaryKey = 0;
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    sbMsg.Append("<br />").Append(ex.Message);
        //                    SendMessage(ex.Message);
        //                    //_ErroLog.Add("Error: " + ex.Message);
        //                    //log.Severe(ex.Message);
        //                }
        //                SendMessage(sbMsg.ToString(), true, true);
        //            }

        //            //update ad_ref_table
        //            if (ds.Tables.Contains("AD_Ref_Table"))
        //                Add_AD_Ref_Tables(ds.Tables["AD_Ref_Table"].Select());


        //            if (ds.Tables.Contains("AD_Workflow"))
        //            {
        //                DataTable dt = ds.Tables["AD_Workflow"];
        //                for (int x = 0; x < dt.Rows.Count; x++)
        //                {
        //                    String updateworkflow = "UPDATE AD_Workflow SET AD_WF_Node_ID = @AD_WF_Node_ID WHERE AD_Workflow_ID = @AD_Workflow_ID";
        //                    Object wf_node_id = dt.Rows[x]["AD_WF_Node_ID"];
        //                    Object workflow_id = dt.Rows[x]["AD_Workflow_ID"];

        //                    if (wf_node_id != null && wf_node_id != DBNull.Value)
        //                    {
        //                        System.Data.SqlClient.SqlParameter[] updateparam = new System.Data.SqlClient.SqlParameter[2];
        //                        updateparam[0] = new System.Data.SqlClient.SqlParameter("@AD_WF_Node_ID", wf_node_id);
        //                        updateparam[1] = new System.Data.SqlClient.SqlParameter("@AD_Workflow_ID", workflow_id);
        //                        int updatenode = DB.ExecuteQuery(updateworkflow, updateparam, null);
        //                        if (updatenode > 0)
        //                        {
        //                            //_ImportInfo.Add("**Updated AD_WF_Node in AD_Workflow");
        //                        }
        //                        else
        //                        {

        //                        }
        //                    }
        //                }
        //            }

        //            if (ds.Tables.Contains("AD_Column") && _isLastClientRecordOfSchemaOrData)
        //            {

        //                DataTable dt = ds.Tables["AD_Column"];
        //                SendMessage("Column Sync Start " + dt.Rows.Count, true, false);
        //                /* Create MetaDataObject Once */
        //                mdObject = new DatabaseMetaData();
        //                fkDataTable = new Dictionary<string, DataTable>(5);

        //                StringBuilder sb = new StringBuilder();

        //                for (int x = 0; x < dt.Rows.Count; x++)
        //                {
        //                    string msg = DoColumnSync(Util.GetValueOfInt(dt.Rows[x]["AD_Column_ID"]));
        //                    SendMessage(msg);
        //                    sb.Append(msg).Append("<br />");
        //                    if (x > 0 && (x % 50) == 0)
        //                    {
        //                        SendMessage(sb.ToString(), true, true);
        //                        sb.Clear();
        //                    }
        //                }

        //                if (sb.Length > 0)
        //                {
        //                    SendMessage(sb.ToString(), true, true);
        //                    sb.Clear();
        //                }

        //                mdObject.Dispose();
        //                mdObject = null;
        //                fkDataTable.Clear();
        //                fkDataTable = null;


        //                SendMessage("Column Sync END", true, false);

        //                System.Threading.Thread.Sleep(500); // 5 seconds sleep
        //            }



        //            //at last execute script 
        //            if (ds.Tables.Contains("AD_Module_DBScript") && _isLastClientRecordOfSchemaOrData)
        //            {
        //                SendMessage("Running Scripts", true, false);
        //                // // Changed Code to Execute Scripts based on the column IsPreExecuteScript
        //                // if UnChecked then execute unchecked Script here else execute all

        //                if (ds.Tables["AD_Module_DBScript"].Columns["IsPreExecuteScript"] != null)
        //                {
        //                    DataRow[] scripts = ds.Tables["AD_Module_DBScript"].Select("IsPreExecuteScript" + " = 'N' ");
        //                    int excecuted = ExecuteScript(scripts);
        //                }
        //                else
        //                {
        //                    DataRow[] scripts = ds.Tables["AD_Module_DBScript"].Select();
        //                    int excecuted = ExecuteScript(scripts);
        //                }
        //                SendMessage("End Scripts", true, false);
        //            }


        //            return "ImportDone";
        //        }
        #endregion


        #region ImportNow New


        private bool IsModuleTable(string tableName)
        {

            if (tableName.Equals("AD_ModuleInfo")
                || tableName.Equals("AD_ModuleRelatedInfo")
                || tableName.Equals("AD_ModuleDependencyInfo")
                //  || tableName.Equals("AD_ModuleMenuFolder")
                || tableName.Equals("AD_ModuleWindow")
                || tableName.Equals("AD_ModuleTab")
                || tableName.Equals("AD_ModuleField")
                || tableName.Equals("AD_ModuleProcess")
                || tableName.Equals("AD_ModuleForm")
                || tableName.Equals("AD_ModuleTable")
                || tableName.Equals("AD_Module_DBScript")
                || tableName.Equals("AD_Module_DB_Schema")
                )
            {
                return true;
            }
            return false;



        }

        ///before Menu Work
        //        String ImportNow(DataSet ds, IMarketCallback callback)
        //        {
        //            __callback = callback;
        //            //no import process will be run if dataset is empty
        //            if (ds.Tables.Count <= 0)
        //                return "XmlNotLoaded";

        //            //_trx = Get_Trx().GetTrxName();   // work with transaction
        //            DataSet dsExportedData = DB.ExecuteDataset("SELECT * FROM AD_LastUpdatedExportData");
        //            String SeqTable = "ExportTableSequence";

        //            if (!ds.Tables.Contains(SeqTable))
        //            {
        //                _isDataMigration = false;
        //                SeqTable = "AD_Module_DB_Schema";
        //            }
        //            else
        //            {
        //                _isDataMigration = true;
        //            }

        //            _tableList = ds.Tables[SeqTable].Select();       //fetch all the tables from dataset having xml data



        //            // Changed Code to Execute Scripts based on the column IsPreExecuteScript
        //            // if Checked then execute checked Script before DB Schema
        //            if (ds.Tables.Contains("AD_Module_DBScript") && _isFirstClientRecordOfSchemaOrData) // run only one time in case of multiple clients are selected
        //            {
        //                if (ds.Tables["AD_Module_DBScript"].Columns["IsPreExecuteScript"] != null)
        //                {
        //                    DataRow[] scripts = ds.Tables["AD_Module_DBScript"].Select("IsPreExecuteScript" + " = 'Y' ");
        //                    int excecuted = ExecuteScript(scripts);
        //                }
        //            }

        //            //InsertOrUpdate function 
        //            List<int> m_ExecutedID = new List<int>();
        //            List<string> m_ExecutedTable = new List<string>();

        //            dsTemp = ds.Copy();

        //            Dictionary<String, String> __generatedInsertQueries = new Dictionary<string, string>();
        //            Dictionary<String, String> __generatedUpdateQueries = new Dictionary<string, string>();
        //            Dictionary<String, int> __ColumnCount = new Dictionary<String, int>();

        //            StringBuilder sbMsg = new StringBuilder("");
        //            StringBuilder sbEntityTypeCheck = new StringBuilder("");
        //            StringBuilder sbColumnConstaintCheck = new StringBuilder("");
        //            StringBuilder sbCheck = new StringBuilder("");
        //            List<DataRow> recentUpdated = null;
        //            bool skipUpdate = false;
        //            bool? updateRecentED = null; //if null then skip updateor insert in ad_RecentExportData
        //            //if fale then Insert 
        //            //if true then Update

        //            String PrimaryColumn = string.Empty;
        //            DataRow[] tempData = null;
        //            DataRow[] data = null;
        //            String QueryForMultipleParents = String.Empty;
        //            bool multiplekeys = false;
        //            int RecordID = -1;
        //            MTable mTable = null;
        //            string Export_ID = string.Empty;

        //            for (int tables = _tableList.Count() - 1; tables >= 0; tables--)
        //            {

        //                String tableName = _tableList[tables]["TableName"].ToString();  //extract the table name to be processed
        //                if (tableName.Equals("AD_Ref_Table", StringComparison.OrdinalIgnoreCase))
        //                    continue;

        //                sbMsg.Clear();
        //                sbEntityTypeCheck.Clear();
        //                sbColumnConstaintCheck.Clear();
        //                sbCheck.Clear();
        //                skipUpdate = false;
        //                updateRecentED = null;
        //                tempData = null;
        //                data = null;
        //                QueryForMultipleParents = String.Empty;
        //                multiplekeys = false;


        //                try
        //                {
        //                    mTable = MTable.Get(GetCtx(), tableName);
        //                    PrimaryColumn = tableName + "_ID";


        //                    RecordID = int.Parse(_tableList[tables]["Record_ID"].ToString());
        //                    if (!dsTemp.Tables[tableName].Columns.Contains(PrimaryColumn))
        //                    {
        //                        multiplekeys = true;
        //                        String[] pcolumns = MTable.Get(ctx, tableName).GetKeyColumns();
        //                        int AD_ColOne_ID = int.Parse(_tableList[tables]["AD_ColOne_ID"].ToString());

        //                        QueryForMultipleParents = pcolumns[0] + " = " + RecordID + " AND " + pcolumns[1] + " = " + AD_ColOne_ID;
        //                        tempData = dsTemp.Tables[tableName].Select(QueryForMultipleParents);
        //                    }
        //                    else
        //                    {
        //                        tempData = dsTemp.Tables[tableName].Select(PrimaryColumn + " = " + RecordID);
        //                    }
        //                    Export_ID = tempData[0]["Export_ID"].ToString();
        //                    UpdatedDateInXMl = Util.GetValueOfString(tempData[0]["Updated"]);

        //                    if (multiplekeys)
        //                    {
        //                        data = dsTemp.Tables[tableName].Select(QueryForMultipleParents + " AND Export_ID = " + DB.TO_STRING(Export_ID));
        //                    }
        //                    else
        //                    {
        //                        data = ds.Tables[tableName].Select(PrimaryColumn + " = " + RecordID + " AND Export_ID = " + DB.TO_STRING(Export_ID));
        //                    }
        //                    if (data.Count() > 0 && !IsModuleTable(tableName))
        //                    {
        //                        recentUpdated = (dsExportedData.Tables[0].Select("Table_ID=" + mTable.GetAD_Table_ID() + " AND RecordExport_ID='" + Export_ID + "' AND AD_Client_ID=" + _AD_Client_ID)).ToList();
        //                        if (recentUpdated.Count > 0) //Check Last Updated Date
        //                        {
        //                            if ((Convert.ToDateTime(UpdatedDateInXMl).ToString("MM/dd/yyyy HH:mm:ss")).Equals(Convert.ToDateTime(recentUpdated[0]["UPDATEDDATE"]).ToString("MM/dd/yyyy HH:mm:ss")))
        //                            {
        //                                SendMessage("Skip updation, Allready Updated, table " + tableName);
        //                                skipUpdate = true;
        //                                updateRecentED = null;
        //                                NewPrimaryKey = Util.GetValueOfInt(recentUpdated[0]["Record_ID"]);
        //                                OldPrimaryKey = Util.GetValueOfInt(tempData[0][PrimaryColumn]);

        //                                if (!multiplekeys)
        //                                {
        //                                    if (tableName.Equals("AD_ModuleMenuFolder", StringComparison.OrdinalIgnoreCase))
        //                                    {
        //                                        if (data[0]["IsParentFolder"].Equals("Y"))
        //                                        {
        //                                            ModuleParentFolder = data[0]["Name"].ToString();
        //                                            ModuleParentExportID = data[0]["Export_ID"].ToString();

        //                                            MMenu[] arrmenu = MMenu.Get(ctx, "Export_ID = " + DB.TO_STRING(ModuleParentExportID));
        //                                            MMenu menu = null;
        //                                            if (arrmenu.Length <= 0)
        //                                            {
        //                                                menu = new MMenu(ctx, 0, null);
        //                                            }
        //                                            else
        //                                            {
        //                                                menu = arrmenu[0];
        //                                            }
        //                                            menu.SetName(ModuleParentFolder);
        //                                            menu.SetIsSummary(true);
        //                                            menu.SetExport_ID(ModuleParentExportID);
        //                                            bool b = menu.Save();

        //                                            if (b)
        //                                                ModuleMenuParentID = menu.Get_ID();
        //                                        }
        //                                    }

        //                                    //Add item to the menu. (only for process, form and window)
        //                                    ProcessMenu(data, tableName, Export_ID);


        //                                    UpdateDependencies(PrimaryColumn, tableName, tables, false);

        //                                }
        //                                continue;
        //                            }
        //                            else
        //                            {
        //                                updateRecentED = true;
        //                            }
        //                        }
        //                        else //insert in AD_RecentExportData
        //                        {
        //                            updateRecentED = false;
        //                        }
        //                    }


        //                    m_TableName.Add(tableName);

        //                    String _insertQuery = String.Empty;
        //                    String _updateQuery = String.Empty;

        //                    if (__generatedUpdateQueries.ContainsKey(tableName))
        //                    {
        //                        _insertQuery = __generatedInsertQueries[tableName];
        //                        _updateQuery = __generatedUpdateQueries[tableName];
        //                        m_CountColumns = __ColumnCount[tableName];
        //                    }
        //                    else
        //                    {
        //                        _insertQuery = CreateInsertQuery(tableName);      //create insert query for the table
        //                        _updateQuery = CreateUpdateQuery(tableName);      //create update query for the table

        //                    }

        //                    //String PrimaryColumn = tableName + "_ID";

        //                    //String Export_ID = "";
        //                    //try
        //                    //{
        //                    //    DataRow[] tempData = null;
        //                    //    DataRow[] data = null;
        //                    //    String QueryForMultipleParents = String.Empty;

        //                    /*CR_002*/
        //                    bool hasEntityTypeColumn = false;

        //                    //bool multiplekeys = false;
        //                    //if (!dsTemp.Tables[tableName].Columns.Contains(PrimaryColumn))
        //                    //{
        //                    //    multiplekeys = true;
        //                    //    String[] pcolumns = MTable.Get(Env.GetCtx(), tableName).GetKeyColumns();
        //                    //    int AD_ColOne_ID = int.Parse(_tableList[tables]["AD_ColOne_ID"].ToString());

        //                    //    QueryForMultipleParents = pcolumns[0] + " = " + RecordID + " AND " + pcolumns[1] + " = " + AD_ColOne_ID;
        //                    //    tempData = dsTemp.Tables[tableName].Select(QueryForMultipleParents);
        //                    //}
        //                    //else
        //                    //{
        //                    //    tempData = dsTemp.Tables[tableName].Select(PrimaryColumn + " = " + RecordID);
        //                    //}

        //                    //Export_ID = tempData[0]["Export_ID"].ToString();        //fetch export id from the data row available
        //                    //DataRow[] data = ds.Tables[tableName].Select(PrimaryColumn + " = " + RecordID);

        //                    hasEntityTypeColumn = dsTemp.Tables[tableName].Columns.Contains("EntityType");

        //                    //if (multiplekeys)
        //                    //{
        //                    //    data = dsTemp.Tables[tableName].Select(QueryForMultipleParents + " AND Export_ID = " + DB.TO_STRING(Export_ID));
        //                    //}
        //                    //else
        //                    //{
        //                    //    data = ds.Tables[tableName].Select(PrimaryColumn + " = " + RecordID + " AND Export_ID = " + DB.TO_STRING(Export_ID));
        //                    //}

        //                    if (data.Count() > 0)
        //                    {
        //                        Export_ID = data[0]["Export_ID"].ToString();        //fetch export id from the data row available
        //                        String TempAD_Client_ID = data[0]["AD_Client_ID"].ToString();

        //                        if (hasEntityTypeColumn)
        //                        {
        //                            sbEntityTypeCheck.Clear();
        //                            sbEntityTypeCheck.Append("SELECT ")
        //                                /*OR_2*///.Append(multiplekeys ? "Count(1) as PrimaryColumn" : PrimaryColumn) //Karan
        //                            .Append(" EntityType ") // select Id Column to pass [null or DbNull.value] check
        //                                        .Append(" FROM ")
        //                                        .Append(tableName)
        //                                        .Append(" WHERE Export_ID = ")
        //                                        .Append(DB.TO_STRING(Export_ID));
        //                        }

        //                        //check if the record with the extracted export id already exisits
        //                        sbCheck.Append("SELECT ")
        //                            /*OR_2*///.Append(multiplekeys ? "Count(1) as PrimaryColumn" : PrimaryColumn) //Karan
        //                           .Append(multiplekeys ? "AD_Client_ID as PrimaryColumn" : PrimaryColumn) // select Id Column to pass [null or DbNull.value] check
        //                                       .Append(" FROM ")
        //                                       .Append(tableName)
        //                                       .Append(" WHERE Export_ID = ")
        //                                       .Append(DB.TO_STRING(Export_ID));

        //                        if (SeqTable == "ExportTableSequence")
        //                        {
        //                            sbCheck.Append(" AND AD_Client_ID = ");

        //                            if (hasEntityTypeColumn)
        //                            {
        //                                sbEntityTypeCheck.Append(" AND AD_Client_ID = ");
        //                            }


        //                            //if current record client id is 0 then record insert in to client id 0
        //                            if (TempAD_Client_ID.Equals("0"))
        //                            {
        //                                sbCheck.Append("0");

        //                                if (hasEntityTypeColumn)
        //                                {
        //                                    sbEntityTypeCheck.Append(" 0 ");
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!TempAD_Client_ID.Equals("0"))
        //                                {
        //                                    if (_AD_Client_ID.Equals(0))
        //                                    {
        //                                        continue;
        //                                    }
        //                                }
        //                                sbCheck.Append(_AD_Client_ID);

        //                                if (hasEntityTypeColumn)
        //                                {
        //                                    sbEntityTypeCheck.Append(_AD_Client_ID);
        //                                }

        //                            }
        //                        }
        //                        else
        //                        {

        //                            if (!TempAD_Client_ID.Equals("0")) //in case schema if record client is not system
        //                            {
        //                                if (_AD_Client_ID.Equals(0)) //current client is system then skip the record 
        //                                {
        //                                    continue;
        //                                }
        //                            }

        //                            sbCheck.Append(" AND (AD_Client_ID = 0 OR AD_Client_ID = ").Append(_AD_Client_ID).Append(")");

        //                            if (hasEntityTypeColumn)
        //                            {
        //                                sbEntityTypeCheck.Append(" AND (AD_Client_ID = 0 OR AD_Client_ID = ").Append(_AD_Client_ID).Append(")");
        //                            }
        //                        }



        //                        //For AD_Column, we need to check it with its name 
        //                        //as standard columns would have already been created by now
        //                        if (tableName.Equals("AD_Column", StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            String __ColumnName = data[0]["ColumnName"].ToString();
        //                            String __TableID = data[0]["AD_Table_ID"].ToString();

        //                            Object r1 = DB.ExecuteScalar(sbCheck.ToString(), null, null);

        //                            if (r1 == null)
        //                            {
        //                                //sbCheck = new StringBuilder("SELECT ")
        //                                sbCheck.Clear();
        //                                sbCheck.Append("SELECT ")
        //                                            .Append(PrimaryColumn)
        //                                            .Append(" FROM ")
        //                                            .Append(tableName)
        //                                            .Append(" WHERE ColumnName = ")
        //                                            .Append(DB.TO_STRING(__ColumnName))
        //                                            .Append(" AND AD_Table_ID = ")
        //                                            .Append(__TableID);
        //                                //.Append(" AND Export_ID = ")
        //                                //.Append(DB.TO_STRING(Export_ID));

        //                            }


        //                            sbColumnConstaintCheck = new StringBuilder("SELECT AD_Column_ID, ConstraintType ").Append(sbCheck.ToString().Substring(sbCheck.ToString().IndexOf("FROM")));

        //                        }
        //                        else if (tableName.Equals("AD_Element", StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            String __ColumnName = data[0]["ColumnName"].ToString();

        //                            Object r1 = DB.ExecuteScalar(sbCheck.ToString(), null, null);

        //                            if (r1 == null)
        //                            {
        //                                sbCheck.Clear();
        //                                sbCheck.Append("SELECT ")
        //                                            .Append(PrimaryColumn)
        //                                            .Append(" FROM ")
        //                                            .Append(tableName)
        //                                            .Append(" WHERE ColumnName = ")
        //                                            .Append(DB.TO_STRING(__ColumnName));
        //                            }
        //                        }
        //                        else if (tableName.Equals("AD_Field", StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            String __ColumnID = data[0]["AD_Column_ID"].ToString();
        //                            String __TabID = data[0]["AD_Tab_ID"].ToString();

        //                            Object r1 = DB.ExecuteScalar(sbCheck.ToString(), null, null);

        //                            if (r1 == null)
        //                            {
        //                                //sbCheck = new StringBuilder("SELECT ")
        //                                sbCheck.Clear();
        //                                sbCheck.Append("SELECT ")
        //                                            .Append(PrimaryColumn)
        //                                            .Append(" FROM ")
        //                                            .Append(tableName)
        //                                            .Append(" WHERE AD_Column_ID = ")
        //                                            .Append(__ColumnID)
        //                                            .Append(" AND AD_Tab_ID = ")
        //                                            .Append(__TabID);
        //                            }
        //                        }
        //                        // Done to Check Whether table is AD_Sequence do not update
        //                        // Insert Based on Name
        //                        else if (tableName.Equals("AD_Sequence", StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            //String __ColumnName = data[0]["ColumnName"].ToString();

        //                            Object r1 = null;

        //                            if (r1 == null)
        //                            {
        //                                sbCheck.Clear();
        //                                sbCheck.Append("SELECT ")
        //                                            .Append(PrimaryColumn)
        //                                            .Append(" FROM ")
        //                                            .Append(tableName)
        //                                            .Append(" WHERE Name = ")
        //                                            .Append(DB.TO_STRING(tempData[0]["Name"].ToString()));
        //                            }
        //                        }

        //                        else if (tableName.Equals("AD_ModuleInfo", StringComparison.OrdinalIgnoreCase) && !_isDataMigration)
        //                        {
        //                            data[0]["VersionNo"] = CurrentModuleVersion;
        //                            data[0].EndEdit();
        //                        }

        //                        Object result = DB.ExecuteScalar(sbCheck.ToString(), null, null);

        //                        //MTable mTable = MTable.Get(GetCtx(), tableName);    //Get table info
        //                        MColumn[] columns = mTable.GetColumns(true);    //Get all the columns of the selected table

        //                        //create sql parameters
        //                        //System.Data.SqlClient.SqlParameter[] param = GetSqlParameter(columns, m_CountColumns, tableName, RecordID, result, QueryForMultipleParents);
        //                        // Changed in case of AD_Sequence

        //                        System.Data.SqlClient.SqlParameter[] param = null;

        //                        if (tableName == "AD_Sequence")
        //                        {
        //                            if (result != null)
        //                            {
        //                                param = GetSqlParameter(columns, m_CountColumns, tableName, RecordID, result, QueryForMultipleParents);
        //                            }
        //                            else
        //                            {
        //                                param = GetSqlParameterSeq(columns, m_CountColumns, tableName, RecordID, tempData[0]["Name"], QueryForMultipleParents);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            param = GetSqlParameter(columns, m_CountColumns, tableName, RecordID, result, QueryForMultipleParents);
        //                        }

        //                        if (result != null && result != DBNull.Value) //update
        //                        {

        //                            try
        //                            {

        //                                if (tableName.Equals("AD_Sequence", StringComparison.OrdinalIgnoreCase))
        //                                {
        //                                    skipUpdate = true;
        //                                }

        //                                // Get Old Constraint type setting 
        //                                if (tableName.Equals("AD_Column", StringComparison.OrdinalIgnoreCase))
        //                                {
        //                                    IDataReader dr = DB.ExecuteReader(sbColumnConstaintCheck.ToString(), null);
        //                                    if (dr.Read())
        //                                    {
        //                                        int ad_col_id = VAdvantage.Utility.Util.GetValueOfInt(dr[0]);
        //                                        if (!_dicColumnCostarintType.Keys.Contains(ad_col_id))
        //                                        {
        //                                            _dicColumnCostarintType[ad_col_id] = VAdvantage.Utility.Util.GetValueOfString(dr[1]);
        //                                        }
        //                                    }
        //                                    dr.Close();
        //                                }

        //                                //SendMessage("Updating table " + tableName);
        //                                //sbMsg.Append("<br />").Append("Updating table " + tableName);
        //                                //Append where query for a particular record
        //                                String whereQuery = " WHERE " + tableName + "_ID = " + result;
        //                                if (multiplekeys)
        //                                {
        //                                    String[] pcolumns = MTable.Get(ctx, tableName).GetKeyColumns();
        //                                    String value_1 = param.First(s => s.ParameterName.Equals("@" + pcolumns[0])).Value.ToString();
        //                                    String value_2 = param.First(s => s.ParameterName.Equals("@" + pcolumns[1])).Value.ToString();
        //                                    whereQuery = " WHERE " + pcolumns[0].Replace("@", "") + " = " + value_1 + " AND " + pcolumns[1].Replace("@", "") + " = " + value_2;
        //                                }

        //                                _updateQuery += whereQuery;

        //                                if (tableName.Equals("AD_Workflow"))
        //                                    ManageWorkflowTables(tableName, ref param, false); //for AD_Workflow

        //                                if (hasEntityTypeColumn && !skipUpdate)
        //                                {
        //                                    Object eType = DB.ExecuteScalar(sbEntityTypeCheck.ToString(), null, null);
        //                                    if (eType != null && eType.ToString().Equals("CUST"))
        //                                    {
        //                                        SendMessage("Skip updation, Customized record, table " + tableName);
        //                                        skipUpdate = true;
        //                                    }
        //                                }


        //                                //int update = skipUpdate ? 0 : DB.ExecuteQuery(_updateQuery, param, null);
        //                                int update = 0;
        //                                if (!skipUpdate)
        //                                {
        //                                    sbMsg.Append("Processing table " + tableName + " <===> " + RecordID);
        //                                    SendMessage("<br />Processing table " + tableName + " <===> " + RecordID);

        //                                    sbMsg.Append("<br />").Append("Updating table " + tableName);
        //                                    SendMessage("Updating table " + tableName);
        //                                    update = DB.ExecuteQuery(_updateQuery, param, null);
        //                                }
        //                                //Insert Or Update AD_RecentExportDate
        //                                if (update > 0 && updateRecentED != null)
        //                                {
        //                                    if (updateRecentED == true)
        //                                    {
        //                                        DB.ExecuteQuery(@"UPDATE AD_LastUpdatedExportData SET 
        //                                                                    UPDATEDDATE=to_date('" + Convert.ToDateTime(UpdatedDateInXMl).ToString("MM/dd/yyyy HH:mm:ss") + @"','MM/dd/yyyy HH24:MI:SS')
        //                                                          WHERE AD_CLIENT_ID=" + _AD_Client_ID + @"
        //                                                                AND TABLE_ID=" + mTable.GetAD_Table_ID() + @"
        //                                                                AND RecordExport_ID ='" + Export_ID + @"'
        //                                                                AND Record_ID=" + result);

        //                                    }
        //                                    else if (updateRecentED == false)
        //                                    {
        //                                        DB.ExecuteQuery(@"INSERT INTO AD_LastUpdatedExportData
        //                                                            (AD_CLIENT_ID,TABLE_ID,RecordExport_ID,RECORD_ID,UPDATEDDATE) 
        //                                                            VALUES(" + _AD_Client_ID + "," + mTable.GetAD_Table_ID() + @",
        //                                                                '" + Export_ID + @"'," + result + @",
        //                                                            to_date('" + Convert.ToDateTime(UpdatedDateInXMl).ToString("MM/dd/yyyy HH:mm:ss") + "','MM/dd/yyyy HH24:MI:SS') )");
        //                                    }
        //                                }

        //                                ValueNamePair error = VLogger.RetrieveError();
        //                                if (error == null || update > 0 || skipUpdate)
        //                                {
        //                                    if (!multiplekeys)
        //                                    {
        //                                        if (tableName.Equals("AD_ModuleMenuFolder", StringComparison.OrdinalIgnoreCase))
        //                                        {
        //                                            if (data[0]["IsParentFolder"].Equals("Y"))
        //                                            {
        //                                                ModuleParentFolder = data[0]["Name"].ToString();
        //                                                ModuleParentExportID = data[0]["Export_ID"].ToString();

        //                                                MMenu[] arrmenu = MMenu.Get(ctx, "Export_ID = " + DB.TO_STRING(ModuleParentExportID));
        //                                                MMenu menu = null;
        //                                                if (arrmenu.Length <= 0)
        //                                                {
        //                                                    menu = new MMenu(ctx, 0, null);
        //                                                }
        //                                                else
        //                                                {
        //                                                    menu = arrmenu[0];
        //                                                }
        //                                                menu.SetName(ModuleParentFolder);
        //                                                menu.SetIsSummary(true);
        //                                                menu.SetExport_ID(ModuleParentExportID);
        //                                                bool b = menu.Save();

        //                                                if (b)
        //                                                    ModuleMenuParentID = menu.Get_ID();
        //                                            }
        //                                        }

        //                                        if (tableName.Equals("AD_WF_Node"))
        //                                            ManageWorkflowTables(tableName, ref param, true); //for AD_Workflow
        //                                        //if (tableName.Equals("AD_WF_NodeNext"))
        //                                        //    ManageWorkflowTables(tableName, ref param, true); //for AD_Workflow


        //                                        //_ImportInfo.Add("Updated table " + tableName + ": Done");

        //                                        //Add item to the menu. (only for process, form and window)
        //                                        ProcessMenu(data, tableName, Export_ID);
        //                                    }


        //                                    if (update != 0)
        //                                    {
        //                                        sbMsg.Append("<br />").Append("Updated table " + tableName + ": Done");
        //                                        SendMessage("Updated table " + tableName + ": Done");
        //                                    }
        //                                    if (!multiplekeys)
        //                                        UpdateDependencies(PrimaryColumn, tableName, tables, false);
        //                                }
        //                                else
        //                                {
        //                                    //sbMsg.Clear();
        //                                    sbMsg.Append("<br />").Append("Error Updating " + tableName + ": " + error.GetName());
        //                                    SendMessage("Error Updating " + tableName + ": " + error.GetName());
        //                                    //_ImportInfo.Add("Error Updating " + tableName + ": " + error.GetName());

        //                                }

        //                                //update binay record
        //                                if (update > 0 && IsBinaryData)
        //                                {

        //                                    String paramName = "@" + BinaryColumnName;
        //                                    System.Data.SqlClient.SqlParameter[] prm = new System.Data.SqlClient.SqlParameter[1];
        //                                    prm[0] = new System.Data.SqlClient.SqlParameter(paramName, BinaryValue);

        //                                    _updateQuery = "UPDATE " + tableName + " SET " + BinaryColumnName + " = " + paramName + whereQuery;
        //                                    update = DB.ExecuteQuery(_updateQuery, prm, null);

        //                                    //if (update <= 0)
        //                                    //    log.Fine("BlobNotLoaded");

        //                                    IsBinaryData = false;
        //                                }







        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                sbMsg.Append("<br />").Append(ex.Message);
        //                                SendMessage(ex.Message);
        //                                //_ErroLog.Add("Error updating " + tableName + ": " + ex.Message);
        //                                //log.Severe(ex.Message);
        //                            }
        //                        }
        //                        else //Insert Record
        //                        {
        //                            sbMsg.Append("Processing table " + tableName + " <===> " + RecordID);
        //                            SendMessage("<br />Processing table " + tableName + " <===> " + RecordID);

        //                            try
        //                            {
        //                                sbMsg.Append("<br />").Append("Inserting into table " + tableName + " <===> " + NewPrimaryKey);
        //                                SendMessage("Inserting into table " + tableName + " <===> " + NewPrimaryKey);
        //                                //AD_Workflow contains an ID AD_WF_Node_ID which has a circular dependency
        //                                //AD_Workflow ==> AD_WF_Node_ID;
        //                                //AD_WF_Node  ==> AD_Workflow_ID
        //                                //We can not insert data into ad_workflow with wf_node_id as wf_node_id is not created at 
        //                                //the time of insertion. So we will nullify the wf_node_id and update it later at the time
        //                                //of insertion into AD_WF_Node table.
        //                                if (tableName.Equals("AD_Workflow"))
        //                                    ManageWorkflowTables(tableName, ref param, false); //for AD_Workflow
        //                                //Insert Mode

        //                                int insert = 0;

        //                                if (tableName.Equals("AD_Table", StringComparison.OrdinalIgnoreCase)
        //                                    || tableName.Equals("AD_Tab", StringComparison.OrdinalIgnoreCase))
        //                                {
        //                                    insert = DB.ExecuteQuery(_insertQuery, param);

        //                                    ReportEngine_N re = null;
        //                                    byte[] report = null;
        //                                    ProcessCtl process = new ProcessCtl();
        //                                    if (tableName.Equals("AD_Table", StringComparison.OrdinalIgnoreCase))
        //                                    {



        //                                        //callback.QueryExecuted(new CallBackDetail() { Status = "Standard columns will be created" });
        //                                        //ProcessInfo info = new ProcessInfo("CreateTable", 173, 100, NewPrimaryKey);
        //                                        //process.Process(info, GetCtx(), out report, out re);
        //                                    }
        //                                    else if (tableName.Equals("AD_Tab", StringComparison.OrdinalIgnoreCase))
        //                                    {
        //                                        SendMessage("Standard fields will be created");
        //                                        sbMsg.Append("<br />").Append("Standard fields will be created");

        //                                        ProcessInfo info = new ProcessInfo("CreateFields", 174, 106, NewPrimaryKey);
        //                                        process.Process(info, GetCtx(), out report, out re);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    insert = DB.ExecuteQuery(_insertQuery, param, null);
        //                                }

        //                                if (insert > 0)
        //                                {
        //                                    //Insert Into LastUpdation
        //                                    DB.ExecuteQuery(@"INSERT INTO AD_LastUpdatedExportData
        //                                                            (AD_CLIENT_ID,TABLE_ID,RecordExport_ID,RECORD_ID,UPDATEDDATE) 
        //                                                            VALUES(" + _AD_Client_ID + "," + mTable.GetAD_Table_ID() + @",
        //                                                                '" + Export_ID + @"'," + NewPrimaryKey + @",
        //                                                            to_date('" + Convert.ToDateTime(UpdatedDateInXMl).ToString("MM/dd/yyyy HH:mm:ss") + "','MM/dd/yyyy HH24:MI:SS'))");


        //                                    if (tableName.Equals("AD_ModuleMenuFolder", StringComparison.OrdinalIgnoreCase))
        //                                    {
        //                                        if (data[0]["IsParentFolder"].Equals("Y"))
        //                                        {
        //                                            ModuleParentFolder = data[0]["Name"].ToString();
        //                                            ModuleParentExportID = data[0]["Export_ID"].ToString();

        //                                            MMenu menu = new MMenu(ctx, 0, null);
        //                                            menu.SetName(ModuleParentFolder);
        //                                            menu.SetIsSummary(true);
        //                                            menu.SetExport_ID(ModuleParentExportID);
        //                                            bool b = menu.Save();

        //                                            if (b)
        //                                                ModuleMenuParentID = menu.Get_ID();
        //                                        }
        //                                    }
        //                                    sbMsg.Append("<br />").Append("Inserted into " + tableName);
        //                                    SendMessage("Inserted into " + tableName);

        //                                    if (!multiplekeys)
        //                                    {
        //                                        UpdateDependencies(PrimaryColumn, tableName, tables, true); //update the dependency on other tables

        //                                        if (IsBinaryData)
        //                                        {
        //                                            String paramName = "@" + BinaryColumnName;
        //                                            String whereQuery = " WHERE " + tableName + "_ID = " + NewPrimaryKey;
        //                                            System.Data.SqlClient.SqlParameter[] prm = new System.Data.SqlClient.SqlParameter[1];
        //                                            prm[0] = new System.Data.SqlClient.SqlParameter(paramName, BinaryValue);

        //                                            _updateQuery = "UPDATE " + tableName + " SET " + BinaryColumnName + " = " + paramName + whereQuery;
        //                                            DB.ExecuteQuery(_updateQuery, prm, null);

        //                                            //if (update <= 0)
        //                                            //    log.Fine("BlobNotLoaded");

        //                                            IsBinaryData = false;
        //                                        }

        //                                        //if (tableName.Equals("AD_Module_DBScript"))
        //                                        //{
        //                                        //    int excecuted = ExecuteScript(data[0]["Script"].ToString());
        //                                        //    callback.QueryExecuted(new CallBackDetail() { Status = "Script excecuted <===> " + excecuted });
        //                                        //}

        //                                        if (tableName.Equals("AD_Table"))
        //                                            MSequence.CreateTableSequence(GetCtx(), data[0]["TableName"].ToString(), null);

        //                                        if (tableName.Equals("AD_WF_Node"))
        //                                            ManageWorkflowTables(tableName, ref param, true); //for AD_Workflow

        //                                        //_ImportInfo.Add("Inserted new record in " + tableName);
        //                                        //SendMessage("Inserted new record in " + tableName);
        //                                        //sbMsg.Append("<br />").Append("Inserted new record in " + tableName);

        //                                        //Add item to the menu. (only for process, form and window)
        //                                        ProcessMenu(tempData, tableName, Export_ID);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    ValueNamePair error = VLogger.RetrieveError();
        //                                    String errorText = "";
        //                                    if (error != null)
        //                                        errorText = error.GetName();

        //                                    sbMsg.Append("<br />").Append("Error in table " + tableName + " <===> " + errorText);
        //                                    SendMessage("Error in table " + tableName + " <===> " + errorText);
        //                                    //_ImportInfo.Add("Error Inserting in " + tableName + ": " + errorText);
        //                                }
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                sbMsg.Append("<br />").Append(ex.Message);
        //                                SendMessage(ex.Message);
        //                                //_ErroLog.Add("Error inserting " + tableName + ": " + ex.Message);
        //                            }
        //                        }

        //                        // IDataReader dr = null;

        //                        if (tableName.Equals("AD_Process", StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            //Assing role to the new menu created
        //                            IDataReader dr = GetRoles();
        //                            bool saved = false;
        //                            while (dr.Read())
        //                            {
        //                                X_AD_Process_Access access = new X_AD_Process_Access(GetCtx(), 0, null);
        //                                access.SetAD_Process_ID(NewPrimaryKey);
        //                                access.SetAD_Role_ID(int.Parse(dr[0].ToString()));
        //                                access.SetIsReadWrite(true);
        //                                saved = access.Save();
        //                            }
        //                            dr.Close();
        //                        }


        //                        if (tableName.Equals("AD_Form", StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            //Assing role to the new menu created

        //                            IDataReader dr = GetRoles();
        //                            bool saved = false;
        //                            while (dr.Read())
        //                            {
        //                                X_AD_Form_Access access = new X_AD_Form_Access(GetCtx(), 0, null);
        //                                access.SetAD_Form_ID(NewPrimaryKey);
        //                                access.SetAD_Role_ID(int.Parse(dr[0].ToString()));
        //                                access.SetIsReadWrite(true);
        //                                saved = access.Save();

        //                            }
        //                            dr.Close();
        //                        }

        //                        //assign role acces to workflow
        //                        if (tableName.Equals("AD_Workflow", StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            //Assing role to the new menu created

        //                            IDataReader dr = GetRoles();
        //                            bool saved = false;
        //                            while (dr.Read())
        //                            {
        //                                X_AD_Workflow_Access access = new X_AD_Workflow_Access(GetCtx(), 0, null);
        //                                access.SetAD_Workflow_ID(NewPrimaryKey);
        //                                access.SetAD_Role_ID(int.Parse(dr[0].ToString()));
        //                                access.SetIsReadWrite(true);
        //                                saved = access.Save();
        //                            }
        //                            dr.Close();
        //                        }



        //                        if (tableName.Equals("AD_Column", StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            // Commented Code
        //                            // As Primary key was not unique  if we sync column here

        //                            // DoColumnSync(NewPrimaryKey);
        //                        }

        //                        //Reset the primary values
        //                        NewPrimaryKey = 0;
        //                        OldPrimaryKey = 0;
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    sbMsg.Append("<br />").Append(ex.Message);
        //                    SendMessage(ex.Message);
        //                    //_ErroLog.Add("Error: " + ex.Message);
        //                    //log.Severe(ex.Message);
        //                }
        //                SendMessage(sbMsg.ToString(), true, true);
        //            }

        //            //update ad_ref_table
        //            if (ds.Tables.Contains("AD_Ref_Table"))
        //                Add_AD_Ref_Tables(ds.Tables["AD_Ref_Table"].Select());


        //            if (ds.Tables.Contains("AD_Workflow"))
        //            {
        //                DataTable dt = ds.Tables["AD_Workflow"];
        //                for (int x = 0; x < dt.Rows.Count; x++)
        //                {
        //                    String updateworkflow = "UPDATE AD_Workflow SET AD_WF_Node_ID = @AD_WF_Node_ID WHERE AD_Workflow_ID = @AD_Workflow_ID";
        //                    Object wf_node_id = dt.Rows[x]["AD_WF_Node_ID"];
        //                    Object workflow_id = dt.Rows[x]["AD_Workflow_ID"];

        //                    if (wf_node_id != null && wf_node_id != DBNull.Value)
        //                    {
        //                        System.Data.SqlClient.SqlParameter[] updateparam = new System.Data.SqlClient.SqlParameter[2];
        //                        updateparam[0] = new System.Data.SqlClient.SqlParameter("@AD_WF_Node_ID", wf_node_id);
        //                        updateparam[1] = new System.Data.SqlClient.SqlParameter("@AD_Workflow_ID", workflow_id);
        //                        int updatenode = DB.ExecuteQuery(updateworkflow, updateparam, null);
        //                        if (updatenode > 0)
        //                        {
        //                            //_ImportInfo.Add("**Updated AD_WF_Node in AD_Workflow");
        //                        }
        //                        else
        //                        {

        //                        }
        //                    }
        //                }
        //            }

        //            if (ds.Tables.Contains("AD_Column") && _isLastClientRecordOfSchemaOrData)
        //            {

        //                DataTable dt = ds.Tables["AD_Column"];
        //                SendMessage("Column Sync Start " + dt.Rows.Count, true, false);
        //                /* Create MetaDataObject Once */
        //                mdObject = new MarketSvc.Classes.DatabaseMetaData();
        //                fkDataTable = new Dictionary<string, DataTable>(5);

        //                StringBuilder sb = new StringBuilder();

        //                for (int x = 0; x < dt.Rows.Count; x++)
        //                {
        //                    string msg = DoColumnSync(Util.GetValueOfInt(dt.Rows[x]["AD_Column_ID"]));
        //                    SendMessage(msg);
        //                    sb.Append(msg).Append("<br />");
        //                    if (x > 0 && (x % 50) == 0)
        //                    {
        //                        SendMessage(sb.ToString(), true, true);
        //                        sb.Clear();
        //                    }
        //                }

        //                if (sb.Length > 0)
        //                {
        //                    SendMessage(sb.ToString(), true, true);
        //                    sb.Clear();
        //                }

        //                mdObject.Dispose();
        //                mdObject = null;
        //                fkDataTable.Clear();
        //                fkDataTable = null;


        //                SendMessage("Column Sync END", true, false);

        //                System.Threading.Thread.Sleep(500); // 5 seconds sleep
        //            }



        //            //at last execute script 
        //            if (ds.Tables.Contains("AD_Module_DBScript") && _isLastClientRecordOfSchemaOrData)
        //            {
        //                SendMessage("Running Scripts", true, false);
        //                // // Changed Code to Execute Scripts based on the column IsPreExecuteScript
        //                // if UnChecked then execute unchecked Script here else execute all

        //                if (ds.Tables["AD_Module_DBScript"].Columns["IsPreExecuteScript"] != null)
        //                {
        //                    DataRow[] scripts = ds.Tables["AD_Module_DBScript"].Select("IsPreExecuteScript" + " = 'N' ");
        //                    int excecuted = ExecuteScript(scripts);
        //                }
        //                else
        //                {
        //                    DataRow[] scripts = ds.Tables["AD_Module_DBScript"].Select();
        //                    int excecuted = ExecuteScript(scripts);
        //                }
        //                SendMessage("End Scripts", true, false);
        //            }

        //            if (dsExportedData != null)
        //            {
        //                dsExportedData.Clear();
        //                dsExportedData = null;
        //            }
        //            return "ImportDone";
        //        }



        ///After Menu Work
        String ImportNow(DataSet ds, IMarketCallback callback)
        {
            __callback = callback;
            //no import process will be run if dataset is empty
            if (ds.Tables.Count <= 0)
                return "XmlNotLoaded";

            //_trx = Get_Trx().GetTrxName();   // work with transaction
            DataSet dsExportedData = DB.ExecuteDataset("SELECT * FROM AD_LastUpdatedExportData");
            String SeqTable = "ExportTableSequence";

            if (!ds.Tables.Contains(SeqTable))
            {
                _isDataMigration = false;
                SeqTable = "AD_Module_DB_Schema";
            }
            else
            {
                _isDataMigration = true;
            }

            _tableList = ds.Tables[SeqTable].Select();       //fetch all the tables from dataset having xml data



            // Changed Code to Execute Scripts based on the column IsPreExecuteScript
            // if Checked then execute checked Script before DB Schema
            if (ds.Tables.Contains("AD_Module_DBScript") && _isFirstClientRecordOfSchemaOrData) // run only one time in case of multiple clients are selected
            {
                if (ds.Tables["AD_Module_DBScript"].Columns["IsPreExecuteScript"] != null)
                {
                    DataRow[] scripts = ds.Tables["AD_Module_DBScript"].Select("IsPreExecuteScript" + " = 'Y' ");
                    int excecuted = ExecuteScript(scripts);
                }
            }

            //InsertOrUpdate function 
            List<int> m_ExecutedID = new List<int>();
            List<string> m_ExecutedTable = new List<string>();

            dsTemp = ds.Copy();

            Dictionary<String, String> __generatedInsertQueries = new Dictionary<string, string>();
            Dictionary<String, String> __generatedUpdateQueries = new Dictionary<string, string>();
            Dictionary<String, int> __ColumnCount = new Dictionary<String, int>();

            StringBuilder sbMsg = new StringBuilder("");
            StringBuilder sbEntityTypeCheck = new StringBuilder("");
            StringBuilder sbColumnConstaintCheck = new StringBuilder("");
            StringBuilder sbCheck = new StringBuilder("");
            List<DataRow> recentUpdated = null;
            bool skipUpdate = false;
            bool? updateRecentED = null; //if null then skip updateor insert in ad_RecentExportData
            //if fale then Insert 
            //if true then Update


            String PrimaryColumn = string.Empty;
            DataRow[] tempData = null;
            DataRow[] data = null;
            String QueryForMultipleParents = String.Empty;
            bool multiplekeys = false;
            int RecordID = -1;
            MTable mTable = null;
            string Export_ID = string.Empty;

            for (int tables = _tableList.Count() - 1; tables >= 0; tables--)
            {

                String tableName = _tableList[tables]["TableName"].ToString();  //extract the table name to be processed
                if (tableName.Equals("AD_Ref_Table", StringComparison.OrdinalIgnoreCase))
                    continue;

                NewPrimaryKey = 0;
                OldPrimaryKey = 0;
                sbMsg.Clear();
                sbEntityTypeCheck.Clear();
                sbColumnConstaintCheck.Clear();
                sbCheck.Clear();
                skipUpdate = false;
                updateRecentED = null;
                tempData = null;
                data = null;
                QueryForMultipleParents = String.Empty;
                multiplekeys = false;


                try
                {
                    mTable = MTable.Get(GetCtx(), tableName);
                    PrimaryColumn = tableName + "_ID";


                    RecordID = int.Parse(_tableList[tables]["Record_ID"].ToString());
                    if (!dsTemp.Tables[tableName].Columns.Contains(PrimaryColumn))
                    {
                        multiplekeys = true;
                        String[] pcolumns = MTable.Get(Env.GetCtx(), tableName).GetKeyColumns();
                        int AD_ColOne_ID = int.Parse(_tableList[tables]["AD_ColOne_ID"].ToString());

                        QueryForMultipleParents = pcolumns[0] + " = " + RecordID + " AND " + pcolumns[1] + " = " + AD_ColOne_ID;
                        tempData = dsTemp.Tables[tableName].Select(QueryForMultipleParents);
                    }
                    else
                    {
                        tempData = dsTemp.Tables[tableName].Select(PrimaryColumn + " = " + RecordID);
                    }
                    Export_ID = tempData[0]["Export_ID"].ToString();
                    UpdatedDateInXMl = Util.GetValueOfString(tempData[0]["Updated"]);

                    if (multiplekeys)
                    {
                        data = dsTemp.Tables[tableName].Select(QueryForMultipleParents + " AND Export_ID = " + DB.TO_STRING(Export_ID));
                    }
                    else
                    {
                        data = ds.Tables[tableName].Select(PrimaryColumn + " = " + RecordID + " AND Export_ID = " + DB.TO_STRING(Export_ID));
                    }
                    if (data.Count() > 0 && !IsModuleTable(tableName))
                    {
                        recentUpdated = (dsExportedData.Tables[0].Select("Table_ID=" + mTable.GetAD_Table_ID() + " AND RecordExport_ID='" + Export_ID + "' AND AD_Client_ID=" + _AD_Client_ID)).ToList();
                        if (recentUpdated.Count > 0) //Check Last Updated Date
                        {
                            if ((Convert.ToDateTime(UpdatedDateInXMl).ToString("MM/dd/yyyy HH:mm:ss")).Equals(Convert.ToDateTime(recentUpdated[0]["UPDATEDDATE"]).ToString("MM/dd/yyyy HH:mm:ss")))
                            {
                                SendMessage("Skip updation, Allready Updated, table " + tableName + " " + tempData[0][PrimaryColumn]);
                                skipUpdate = true;
                                updateRecentED = null;
                                NewPrimaryKey = Util.GetValueOfInt(recentUpdated[0]["Record_ID"]);
                                OldPrimaryKey = Util.GetValueOfInt(tempData[0][PrimaryColumn]);
                            }
                            else
                            {
                                updateRecentED = true;
                            }
                        }
                        else //insert in AD_RecentExportData
                        {
                            updateRecentED = false;
                        }
                        if (!multiplekeys)
                        {
                            if (tableName.Equals("AD_ModuleMenuFolder", StringComparison.OrdinalIgnoreCase))
                            {
                                if (data[0]["IsParentFolder"].Equals("Y"))
                                {
                                    ModuleParentFolder = data[0]["Name"].ToString();
                                    ModuleParentExportID = data[0]["Export_ID"].ToString();

                                    MMenu[] arrmenu = MMenu.Get(ctx, "Export_ID = " + DB.TO_STRING(ModuleParentExportID));
                                    MMenu menu = null;
                                    if (arrmenu.Length <= 0)
                                    {
                                        menu = new MMenu(ctx, 0, null);
                                    }
                                    else
                                    {
                                        menu = arrmenu[0];
                                    }
                                    menu.SetName(ModuleParentFolder);
                                    menu.SetIsSummary(true);
                                    menu.SetExport_ID(ModuleParentExportID);
                                    bool b = menu.Save();

                                    if (b)
                                        ModuleMenuParentID = menu.Get_ID();
                                }
                                //work done for menu 
                                // place folder inside folder
                                else
                                {
                                    ModuleParentFolder = data[0]["Name"].ToString();
                                    ModuleParentExportID = data[0]["Export_ID"].ToString();
                                    data = dsTemp.Tables[tableName].Select("Export_ID = '" + ModuleParentExportID + "'");
                                    int ParentID = 0;
                                    if (data[0].Table.Columns.Contains("AD_Ref_ModuleMenuFolder_ID"))
                                    {
                                        ParentID = Util.GetValueOfInt(data[0]["AD_Ref_ModuleMenuFolder_ID"]);
                                    }

                                    DataRow[] rData = null;
                                    MMenu[] arrmenu = null;
                                    MMenu menu = null;
                                    int parentNodeID = 0;
                                    int count = 0;
                                    if (ParentID > 0)
                                    {
                                        rData = dsTemp.Tables[tableName].Select(PrimaryColumn + " = " + ParentID);
                                        if (rData != null && rData.Count() > 0)
                                        {
                                            int seqNo = -1;
                                            if (rData[0].Table.Columns.Contains("SeqNo"))
                                            {
                                                seqNo = Util.GetValueOfInt(rData[0]["SeqNo"].ToString());
                                            }
                                            else
                                            {
                                                seqNo = 9999;
                                            }

                                            arrmenu = MMenu.Get(ctx, "Export_ID = " + DB.TO_STRING(rData[0]["Export_ID"].ToString()));
                                            menu = null;
                                            if (arrmenu.Length <= 0)
                                            {
                                                // Change by mohit...asked by mukesh sir...menu setting...same name folder issue...31/10/2017..PMS TaskID=4467
                                                arrmenu = MMenu.Get(ctx, "Name = " + DB.TO_STRING(rData[0]["Name"].ToString()) + " AND Export_ID = " + DB.TO_STRING(rData[0]["Export_ID"].ToString()));
                                                if (arrmenu.Length <= 0)
                                                {
                                                    menu = new MMenu(ctx, 0, null);
                                                }
                                                else
                                                {
                                                    menu = arrmenu[0];
                                                }
                                            }
                                            else
                                            {
                                                menu = arrmenu[0];
                                            }
                                            menu.SetName(rData[0]["Name"].ToString());
                                            menu.SetIsSummary(true);
                                            menu.SetIsActive(true);
                                            menu.SetExport_ID(rData[0]["Export_ID"].ToString());
                                            menu.Save();
                                            parentNodeID = menu.Get_ID();
                                            count = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM AD_TreeNodeMM WHERE Node_ID=" + menu.Get_ID()));
                                            if (count > 0)//update
                                            {
                                                int Parent_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Parent_ID FROM AD_TreeNodeMM WHERE Node_ID=" + menu.Get_ID()));
                                                if (Parent_ID > 0)
                                                {

                                                }
                                                else
                                                {
                                                    DB.ExecuteQuery("UPDATE AD_TreeNodeMM SET Parent_ID=0, SeqNo=" + seqNo + " WHERE Node_ID=" + menu.Get_ID());
                                                }
                                            }
                                            else //insert
                                            {
                                                DB.ExecuteQuery("INSERT INTO AD_TreeNodeMM (AD_CLIENT_ID ,AD_ORG_ID,AD_TREE_ID,CREATED,CREATEDBY,ISACTIVE,NODE_ID,PARENT_ID,SEQNO,UPDATED,UPDATEDBY,EXPORT_ID) VALUES (0,0,121," + GlobalVariable.TO_DATE(DateTime.Now, true) + ",100,'Y'," + menu.Get_ID() + ",0," + seqNo + "," + GlobalVariable.TO_DATE(DateTime.Now, true) + ",100,NULL)");
                                            }
                                        }
                                    }
                                    arrmenu = MMenu.Get(ctx, "Export_ID = " + DB.TO_STRING(ModuleParentExportID));
                                    menu = null;
                                    if (arrmenu.Length <= 0)
                                    {
                                        // Change by mohit...asked by mukesh sir...menu setting...same name folder issue...31/10/2017..PMS TaskID=4467
                                        arrmenu = MMenu.Get(ctx, "Name = " + DB.TO_STRING(ModuleParentFolder) + " AND Export_ID = " + DB.TO_STRING(ModuleParentExportID));
                                        if (arrmenu.Length <= 0)
                                        {
                                            menu = new MMenu(ctx, 0, null);
                                        }
                                        else
                                        {
                                            menu = arrmenu[0];
                                        }
                                    }
                                    else
                                    {
                                        menu = arrmenu[0];
                                    }
                                    menu.SetName(ModuleParentFolder);
                                    menu.SetIsSummary(true);
                                    menu.SetIsActive(true);
                                    menu.SetExport_ID(ModuleParentExportID);
                                    menu.Save();

                                    count = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM AD_TreeNodeMM WHERE Node_ID=" + menu.Get_ID()));
                                    if (count > 0)//update
                                    {
                                        DB.ExecuteQuery("UPDATE AD_TreeNodeMM SET Parent_ID=" + parentNodeID + ", SeqNo=" + Util.GetValueOfInt(data[0]["SeqNo"]) + " WHERE Node_ID=" + menu.Get_ID());
                                    }
                                    else //insert
                                    {
                                        DB.ExecuteQuery("INSERT INTO AD_TreeNodeMM (AD_CLIENT_ID ,AD_ORG_ID,AD_TREE_ID,CREATED,CREATEDBY,ISACTIVE,NODE_ID,PARENT_ID,SEQNO,UPDATED,UPDATEDBY,EXPORT_ID) VALUES (0,0,121," + GlobalVariable.TO_DATE(DateTime.Now, true) + ",100,'Y'," + menu.Get_ID() + "," + parentNodeID + "," + Util.GetValueOfInt(data[0]["SeqNo"]) + "," + GlobalVariable.TO_DATE(DateTime.Now, true) + ",100,NULL)");
                                    }
                                }
                            }

                            //Add item to the menu. (only for process, form and window)
                            ProcessMenu(data, tableName, Export_ID);
                            UpdateDependencies(PrimaryColumn, tableName, tables, false);

                        }
                        if (skipUpdate)
                        {
                            continue;
                        }


                    }


                    m_TableName.Add(tableName);

                    String _insertQuery = String.Empty;
                    String _updateQuery = String.Empty;

                    if (__generatedUpdateQueries.ContainsKey(tableName))
                    {
                        _insertQuery = __generatedInsertQueries[tableName];
                        _updateQuery = __generatedUpdateQueries[tableName];
                        m_CountColumns = __ColumnCount[tableName];
                    }
                    else
                    {
                        _insertQuery = CreateInsertQuery(tableName);      //create insert query for the table
                        _updateQuery = CreateUpdateQuery(tableName);      //create update query for the table

                    }

                    //String PrimaryColumn = tableName + "_ID";

                    //String Export_ID = "";
                    //try
                    //{
                    //    DataRow[] tempData = null;
                    //    DataRow[] data = null;
                    //    String QueryForMultipleParents = String.Empty;

                    /*CR_002*/
                    bool hasEntityTypeColumn = false;

                    //bool multiplekeys = false;
                    //if (!dsTemp.Tables[tableName].Columns.Contains(PrimaryColumn))
                    //{
                    //    multiplekeys = true;
                    //    String[] pcolumns = MTable.Get(Env.GetCtx(), tableName).GetKeyColumns();
                    //    int AD_ColOne_ID = int.Parse(_tableList[tables]["AD_ColOne_ID"].ToString());

                    //    QueryForMultipleParents = pcolumns[0] + " = " + RecordID + " AND " + pcolumns[1] + " = " + AD_ColOne_ID;
                    //    tempData = dsTemp.Tables[tableName].Select(QueryForMultipleParents);
                    //}
                    //else
                    //{
                    //    tempData = dsTemp.Tables[tableName].Select(PrimaryColumn + " = " + RecordID);
                    //}

                    //Export_ID = tempData[0]["Export_ID"].ToString();        //fetch export id from the data row available
                    //DataRow[] data = ds.Tables[tableName].Select(PrimaryColumn + " = " + RecordID);

                    hasEntityTypeColumn = dsTemp.Tables[tableName].Columns.Contains("EntityType");

                    //if (multiplekeys)
                    //{
                    //    data = dsTemp.Tables[tableName].Select(QueryForMultipleParents + " AND Export_ID = " + DB.TO_STRING(Export_ID));
                    //}
                    //else
                    //{
                    //    data = ds.Tables[tableName].Select(PrimaryColumn + " = " + RecordID + " AND Export_ID = " + DB.TO_STRING(Export_ID));
                    //}

                    if (data.Count() > 0)
                    {
                        Export_ID = data[0]["Export_ID"].ToString();        //fetch export id from the data row available
                        String TempAD_Client_ID = data[0]["AD_Client_ID"].ToString();

                        if (hasEntityTypeColumn)
                        {
                            sbEntityTypeCheck.Clear();
                            sbEntityTypeCheck.Append("SELECT ")
                                /*OR_2*///.Append(multiplekeys ? "Count(1) as PrimaryColumn" : PrimaryColumn) //Karan
                            .Append(" EntityType ") // select Id Column to pass [null or DbNull.value] check
                                        .Append(" FROM ")
                                        .Append(tableName)
                                        .Append(" WHERE Export_ID = ")
                                        .Append(DB.TO_STRING(Export_ID));
                        }

                        //check if the record with the extracted export id already exisits
                        sbCheck.Append("SELECT ")
                            /*OR_2*///.Append(multiplekeys ? "Count(1) as PrimaryColumn" : PrimaryColumn) //Karan
                           .Append(multiplekeys ? "AD_Client_ID as PrimaryColumn" : PrimaryColumn) // select Id Column to pass [null or DbNull.value] check
                                       .Append(" FROM ")
                                       .Append(tableName)
                                       .Append(" WHERE Export_ID = ")
                                       .Append(DB.TO_STRING(Export_ID));

                        if (SeqTable == "ExportTableSequence")
                        {
                            sbCheck.Append(" AND AD_Client_ID = ");

                            if (hasEntityTypeColumn)
                            {
                                sbEntityTypeCheck.Append(" AND AD_Client_ID = ");
                            }


                            //if current record client id is 0 then record insert in to client id 0
                            if (TempAD_Client_ID.Equals("0"))
                            {
                                sbCheck.Append("0");

                                if (hasEntityTypeColumn)
                                {
                                    sbEntityTypeCheck.Append(" 0 ");
                                }
                            }
                            else
                            {
                                if (!TempAD_Client_ID.Equals("0"))
                                {
                                    if (_AD_Client_ID.Equals(0))
                                    {
                                        continue;
                                    }
                                }
                                sbCheck.Append(_AD_Client_ID);

                                if (hasEntityTypeColumn)
                                {
                                    sbEntityTypeCheck.Append(_AD_Client_ID);
                                }

                            }
                        }
                        else
                        {

                            if (!TempAD_Client_ID.Equals("0")) //in case schema if record client is not system
                            {
                                if (_AD_Client_ID.Equals(0)) //current client is system then skip the record 
                                {
                                    continue;
                                }
                            }

                            sbCheck.Append(" AND (AD_Client_ID = 0 OR AD_Client_ID = ").Append(_AD_Client_ID).Append(")");

                            if (hasEntityTypeColumn)
                            {
                                sbEntityTypeCheck.Append(" AND (AD_Client_ID = 0 OR AD_Client_ID = ").Append(_AD_Client_ID).Append(")");
                            }
                        }



                        //For AD_Column, we need to check it with its name 
                        //as standard columns would have already been created by now
                        if (tableName.Equals("AD_Column", StringComparison.OrdinalIgnoreCase))
                        {
                            String __ColumnName = data[0]["ColumnName"].ToString();
                            String __TableID = data[0]["AD_Table_ID"].ToString();

                            Object r1 = DB.ExecuteScalar(sbCheck.ToString(), null, null);

                            if (r1 == null)
                            {
                                //sbCheck = new StringBuilder("SELECT ")
                                sbCheck.Clear();
                                sbCheck.Append("SELECT ")
                                            .Append(PrimaryColumn)
                                            .Append(" FROM ")
                                            .Append(tableName)
                                            .Append(" WHERE ColumnName = ")
                                            .Append(DB.TO_STRING(__ColumnName))
                                            .Append(" AND AD_Table_ID = ")
                                            .Append(__TableID);
                                //.Append(" AND Export_ID = ")
                                //.Append(DB.TO_STRING(Export_ID));

                            }


                            sbColumnConstaintCheck = new StringBuilder("SELECT AD_Column_ID, ConstraintType ").Append(sbCheck.ToString().Substring(sbCheck.ToString().IndexOf("FROM")));

                        }
                        else if (tableName.Equals("AD_Element", StringComparison.OrdinalIgnoreCase))
                        {
                            String __ColumnName = data[0]["ColumnName"].ToString();

                            Object r1 = DB.ExecuteScalar(sbCheck.ToString(), null, null);

                            if (r1 == null)
                            {
                                sbCheck.Clear();
                                sbCheck.Append("SELECT ")
                                            .Append(PrimaryColumn)
                                            .Append(" FROM ")
                                            .Append(tableName)
                                            .Append(" WHERE ColumnName = ")
                                            .Append(DB.TO_STRING(__ColumnName));
                            }
                        }
                        else if (tableName.Equals("AD_Field", StringComparison.OrdinalIgnoreCase))
                        {
                            String __ColumnID = data[0]["AD_Column_ID"].ToString();
                            String __TabID = data[0]["AD_Tab_ID"].ToString();

                            Object r1 = DB.ExecuteScalar(sbCheck.ToString(), null, null);

                            if (r1 == null)
                            {
                                //sbCheck = new StringBuilder("SELECT ")
                                sbCheck.Clear();
                                sbCheck.Append("SELECT ")
                                            .Append(PrimaryColumn)
                                            .Append(" FROM ")
                                            .Append(tableName)
                                            .Append(" WHERE AD_Column_ID = ")
                                            .Append(__ColumnID)
                                            .Append(" AND AD_Tab_ID = ")
                                            .Append(__TabID);
                            }
                        }
                        // Done to Check Whether table is AD_Sequence do not update
                        // Insert Based on Name
                        else if (tableName.Equals("AD_Sequence", StringComparison.OrdinalIgnoreCase))
                        {
                            //String __ColumnName = data[0]["ColumnName"].ToString();

                            Object r1 = null;

                            if (r1 == null)
                            {
                                sbCheck.Clear();
                                sbCheck.Append("SELECT ")
                                            .Append(PrimaryColumn)
                                            .Append(" FROM ")
                                            .Append(tableName)
                                            .Append(" WHERE Name = ")
                                            .Append(DB.TO_STRING(tempData[0]["Name"].ToString()));
                            }
                        }

                        else if (tableName.Equals("AD_ModuleInfo", StringComparison.OrdinalIgnoreCase) && !_isDataMigration)
                        {
                            data[0]["VersionNo"] = CurrentModuleVersion;
                            data[0].EndEdit();
                        }

                        Object result = DB.ExecuteScalar(sbCheck.ToString(), null, null);

                        //MTable mTable = MTable.Get(GetCtx(), tableName);    //Get table info
                        MColumn[] columns = mTable.GetColumns(true);    //Get all the columns of the selected table

                        //create sql parameters
                        //System.Data.SqlClient.SqlParameter[] param = GetSqlParameter(columns, m_CountColumns, tableName, RecordID, result, QueryForMultipleParents);
                        // Changed in case of AD_Sequence

                        System.Data.SqlClient.SqlParameter[] param = null;

                        if (tableName == "AD_Sequence")
                        {
                            if (result != null)
                            {
                                param = GetSqlParameter(columns, m_CountColumns, tableName, RecordID, result, QueryForMultipleParents);
                            }
                            else
                            {
                                param = GetSqlParameterSeq(columns, m_CountColumns, tableName, RecordID, tempData[0]["Name"], QueryForMultipleParents);
                            }
                        }
                        else
                        {
                            param = GetSqlParameter(columns, m_CountColumns, tableName, RecordID, result, QueryForMultipleParents);
                        }

                        if (result != null && result != DBNull.Value) //update
                        {

                            try
                            {

                                if (tableName.Equals("AD_Sequence", StringComparison.OrdinalIgnoreCase))
                                {
                                    skipUpdate = true;
                                }

                                // Get Old Constraint type setting 
                                if (tableName.Equals("AD_Column", StringComparison.OrdinalIgnoreCase))
                                {
                                    IDataReader dr = DB.ExecuteReader(sbColumnConstaintCheck.ToString(), null);
                                    if (dr.Read())
                                    {
                                        int ad_col_id = VAdvantage.Utility.Util.GetValueOfInt(dr[0]);
                                        if (!_dicColumnCostarintType.Keys.Contains(ad_col_id))
                                        {
                                            _dicColumnCostarintType[ad_col_id] = VAdvantage.Utility.Util.GetValueOfString(dr[1]);
                                        }
                                    }
                                    dr.Close();
                                }

                                //SendMessage("Updating table " + tableName);
                                //sbMsg.Append("\n").Append("Updating table " + tableName);
                                //Append where query for a particular record
                                String whereQuery = " WHERE " + tableName + "_ID = " + result;
                                if (multiplekeys)
                                {
                                    String[] pcolumns = MTable.Get(Env.GetCtx(), tableName).GetKeyColumns();
                                    String value_1 = param.First(s => s.ParameterName.Equals("@" + pcolumns[0])).Value.ToString();
                                    String value_2 = param.First(s => s.ParameterName.Equals("@" + pcolumns[1])).Value.ToString();
                                    whereQuery = " WHERE " + pcolumns[0].Replace("@", "") + " = " + value_1 + " AND " + pcolumns[1].Replace("@", "") + " = " + value_2;
                                }

                                _updateQuery += whereQuery;

                                if (tableName.Equals("AD_Workflow"))
                                    ManageWorkflowTables(tableName, ref param, false); //for AD_Workflow

                                if (hasEntityTypeColumn && !skipUpdate)
                                {
                                    Object eType = DB.ExecuteScalar(sbEntityTypeCheck.ToString(), null, null);
                                    if (eType != null && eType.ToString().Equals("CUST"))
                                    {
                                        SendMessage("Skip updation, Customized record, table " + tableName);
                                        skipUpdate = true;
                                    }
                                }


                                //int update = skipUpdate ? 0 : DB.ExecuteQuery(_updateQuery, param, null);
                                int update = 0;
                                if (!skipUpdate)
                                {
                                    sbMsg.Append("Processing table " + tableName + " <===> " + RecordID);
                                    SendMessage("<br />Processing table " + tableName + " <===> " + RecordID);

                                    sbMsg.Append("<br />").Append("Updating table " + tableName);
                                    SendMessage("Updating table " + tableName);
                                    update = DB.ExecuteQuery(_updateQuery, param, null);
                                }
                                //Insert Or Update AD_RecentExportDate
                                if (update > 0 && updateRecentED != null)
                                {
                                    if (updateRecentED == true)
                                    {
                                        DB.ExecuteQuery(@"UPDATE AD_LastUpdatedExportData SET 
                                                                    UPDATEDDATE=to_date('" + Convert.ToDateTime(UpdatedDateInXMl).ToString("MM/dd/yyyy HH:mm:ss") + @"','MM/dd/yyyy HH24:MI:SS')
                                                          WHERE AD_CLIENT_ID=" + _AD_Client_ID + @"
                                                                AND TABLE_ID=" + mTable.GetAD_Table_ID() + @"
                                                                AND RecordExport_ID ='" + Export_ID + @"'
                                                                AND Record_ID=" + result);

                                    }
                                    else if (updateRecentED == false)
                                    {
                                        DB.ExecuteQuery(@"INSERT INTO AD_LastUpdatedExportData
                                                            (AD_CLIENT_ID,TABLE_ID,RecordExport_ID,RECORD_ID,UPDATEDDATE) 
                                                            VALUES(" + _AD_Client_ID + "," + mTable.GetAD_Table_ID() + @",
                                                                '" + Export_ID + @"'," + result + @",
                                                            to_date('" + Convert.ToDateTime(UpdatedDateInXMl).ToString("MM/dd/yyyy HH:mm:ss") + "','MM/dd/yyyy HH24:MI:SS') )");
                                    }
                                }

                                ValueNamePair error = VLogger.RetrieveError();
                                if (error == null || update > 0 || skipUpdate)
                                {
                                    if (!multiplekeys)
                                    {
                                        if (tableName.Equals("AD_ModuleMenuFolder", StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (data[0]["IsParentFolder"].Equals("Y"))
                                            {
                                                ModuleParentFolder = data[0]["Name"].ToString();
                                                ModuleParentExportID = data[0]["Export_ID"].ToString();

                                                MMenu[] arrmenu = MMenu.Get(ctx, "Export_ID = " + DB.TO_STRING(ModuleParentExportID));
                                                MMenu menu = null;
                                                if (arrmenu.Length <= 0)
                                                {
                                                    menu = new MMenu(ctx, 0, null);
                                                }
                                                else
                                                {
                                                    menu = arrmenu[0];
                                                }
                                                menu.SetName(ModuleParentFolder);
                                                menu.SetIsSummary(true);
                                                menu.SetExport_ID(ModuleParentExportID);
                                                bool b = menu.Save();

                                                if (b)
                                                    ModuleMenuParentID = menu.Get_ID();
                                            }
                                        }

                                        if (tableName.Equals("AD_WF_Node"))
                                            ManageWorkflowTables(tableName, ref param, true); //for AD_Workflow
                                        //if (tableName.Equals("AD_WF_NodeNext"))
                                        //    ManageWorkflowTables(tableName, ref param, true); //for AD_Workflow


                                        //_ImportInfo.Add("Updated table " + tableName + ": Done");

                                        //Add item to the menu. (only for process, form and window)
                                        ProcessMenu(data, tableName, Export_ID);
                                    }


                                    if (update != 0)
                                    {
                                        sbMsg.Append("<br />").Append("Updated table " + tableName + ": Done");
                                        SendMessage("Updated table " + tableName + ": Done");
                                    }
                                    if (!multiplekeys)
                                        UpdateDependencies(PrimaryColumn, tableName, tables, false);
                                }
                                else
                                {
                                    //sbMsg.Clear();
                                    sbMsg.Append("<br />").Append("Error Updating " + tableName + ": " + error.GetName());
                                    SendMessage("Error Updating " + tableName + ": " + error.GetName());
                                    //_ImportInfo.Add("Error Updating " + tableName + ": " + error.GetName());

                                }

                                //update binay record
                                if (update > 0 && IsBinaryData)
                                {

                                    String paramName = "@" + BinaryColumnName;
                                    System.Data.SqlClient.SqlParameter[] prm = new System.Data.SqlClient.SqlParameter[1];
                                    prm[0] = new System.Data.SqlClient.SqlParameter(paramName, BinaryValue);

                                    _updateQuery = "UPDATE " + tableName + " SET " + BinaryColumnName + " = " + paramName + whereQuery;
                                    update = DB.ExecuteQuery(_updateQuery, prm, null);

                                    //if (update <= 0)
                                    //    log.Fine("BlobNotLoaded");

                                    IsBinaryData = false;
                                }







                            }
                            catch (Exception ex)
                            {
                                sbMsg.Append("<br />").Append(ex.Message);
                                SendMessage(ex.Message);
                                //_ErroLog.Add("Error updating " + tableName + ": " + ex.Message);
                                //log.Severe(ex.Message);
                            }
                        }
                        else //Insert Record
                        {
                            sbMsg.Append("Processing table " + tableName + " <===> " + RecordID);
                            SendMessage("<br />Processing table " + tableName + " <===> " + RecordID);

                            try
                            {
                                sbMsg.Append("<br />").Append("Inserting into table " + tableName + " <===> " + NewPrimaryKey);
                                SendMessage("Inserting into table " + tableName + " <===> " + NewPrimaryKey);
                                //AD_Workflow contains an ID AD_WF_Node_ID which has a circular dependency
                                //AD_Workflow ==> AD_WF_Node_ID;
                                //AD_WF_Node  ==> AD_Workflow_ID
                                //We can not insert data into ad_workflow with wf_node_id as wf_node_id is not created at 
                                //the time of insertion. So we will nullify the wf_node_id and update it later at the time
                                //of insertion into AD_WF_Node table.
                                if (tableName.Equals("AD_Workflow"))
                                    ManageWorkflowTables(tableName, ref param, false); //for AD_Workflow
                                //Insert Mode

                                int insert = 0;

                                if (tableName.Equals("AD_Table", StringComparison.OrdinalIgnoreCase)
                                    || tableName.Equals("AD_Tab", StringComparison.OrdinalIgnoreCase))
                                {
                                    insert = DB.ExecuteQuery(_insertQuery, param);

                                    ReportEngine_N re = null;
                                    byte[] report = null;
                                    ProcessCtl process = new ProcessCtl();
                                    if (tableName.Equals("AD_Table", StringComparison.OrdinalIgnoreCase))
                                    {



                                        //callback.QueryExecuted(new CallBackDetail() { Status = "Standard columns will be created" });
                                        //ProcessInfo info = new ProcessInfo("CreateTable", 173, 100, NewPrimaryKey);
                                        //process.Process(info, GetCtx(), out report, out re);
                                    }
                                    else if (tableName.Equals("AD_Tab", StringComparison.OrdinalIgnoreCase))
                                    {
                                        //SendMessage("Standard fields will be created");
                                        //sbMsg.Append("<br />").Append("Standard fields will be created");

                                        //ProcessInfo info = new ProcessInfo("CreateFields", 174, 106, NewPrimaryKey);
                                        //process.Process(info, GetCtx(), out report, out re);
                                    }
                                }
                                else
                                {
                                    insert = DB.ExecuteQuery(_insertQuery, param, null);
                                }

                                if (insert > 0)
                                {
                                    //Insert Into LastUpdation
                                    DB.ExecuteQuery(@"INSERT INTO AD_LastUpdatedExportData
                                                            (AD_CLIENT_ID,TABLE_ID,RecordExport_ID,RECORD_ID,UPDATEDDATE) 
                                                            VALUES(" + _AD_Client_ID + "," + mTable.GetAD_Table_ID() + @",
                                                                '" + Export_ID + @"'," + NewPrimaryKey + @",
                                                            to_date('" + Convert.ToDateTime(UpdatedDateInXMl).ToString("MM/dd/yyyy HH:mm:ss") + "','MM/dd/yyyy HH24:MI:SS'))");


                                    if (tableName.Equals("AD_ModuleMenuFolder", StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (data[0]["IsParentFolder"].Equals("Y"))
                                        {
                                            ModuleParentFolder = data[0]["Name"].ToString();
                                            ModuleParentExportID = data[0]["Export_ID"].ToString();

                                            MMenu menu = new MMenu(ctx, 0, null);
                                            menu.SetName(ModuleParentFolder);
                                            menu.SetIsSummary(true);
                                            menu.SetExport_ID(ModuleParentExportID);
                                            bool b = menu.Save();

                                            if (b)
                                                ModuleMenuParentID = menu.Get_ID();
                                        }
                                    }
                                    sbMsg.Append("<br />").Append("Inserted into " + tableName);
                                    SendMessage("Inserted into " + tableName);

                                    if (!multiplekeys)
                                    {
                                        UpdateDependencies(PrimaryColumn, tableName, tables, true); //update the dependency on other tables

                                        if (IsBinaryData)
                                        {
                                            String paramName = "@" + BinaryColumnName;
                                            String whereQuery = " WHERE " + tableName + "_ID = " + NewPrimaryKey;
                                            System.Data.SqlClient.SqlParameter[] prm = new System.Data.SqlClient.SqlParameter[1];
                                            prm[0] = new System.Data.SqlClient.SqlParameter(paramName, BinaryValue);

                                            _updateQuery = "UPDATE " + tableName + " SET " + BinaryColumnName + " = " + paramName + whereQuery;
                                            DB.ExecuteQuery(_updateQuery, prm, null);

                                            //if (update <= 0)
                                            //    log.Fine("BlobNotLoaded");

                                            IsBinaryData = false;
                                        }

                                        //if (tableName.Equals("AD_Module_DBScript"))
                                        //{
                                        //    int excecuted = ExecuteScript(data[0]["Script"].ToString());
                                        //    callback.QueryExecuted(new CallBackDetail() { Status = "Script excecuted <===> " + excecuted });
                                        //}

                                        if (tableName.Equals("AD_Table"))
                                            MSequence.CreateTableSequence(GetCtx(), data[0]["TableName"].ToString(), null);

                                        if (tableName.Equals("AD_WF_Node"))
                                            ManageWorkflowTables(tableName, ref param, true); //for AD_Workflow

                                        //_ImportInfo.Add("Inserted new record in " + tableName);
                                        //SendMessage("Inserted new record in " + tableName);
                                        //sbMsg.Append("<br />").Append("Inserted new record in " + tableName);

                                        //Add item to the menu. (only for process, form and window)
                                        ProcessMenu(tempData, tableName, Export_ID);
                                    }
                                }
                                else
                                {
                                    ValueNamePair error = VLogger.RetrieveError();
                                    String errorText = "";
                                    if (error != null)
                                        errorText = error.GetName();

                                    sbMsg.Append("<br />").Append("Error in table " + tableName + " <===> " + errorText);
                                    SendMessage("Error in table " + tableName + " <===> " + errorText);
                                    //_ImportInfo.Add("Error Inserting in " + tableName + ": " + errorText);
                                }
                            }
                            catch (Exception ex)
                            {
                                sbMsg.Append("<br />").Append(ex.Message);
                                SendMessage(ex.Message);
                                //_ErroLog.Add("Error inserting " + tableName + ": " + ex.Message);
                            }
                        }

                        // IDataReader dr = null;

                        if (tableName.Equals("AD_Process", StringComparison.OrdinalIgnoreCase))
                        {
                            //Assing role to the new menu created
                            IDataReader dr = GetRoles();
                            bool saved = false;
                            while (dr.Read())
                            {
                                X_AD_Process_Access access = new X_AD_Process_Access(GetCtx(), 0, null);
                                access.SetAD_Process_ID(NewPrimaryKey);
                                access.SetAD_Role_ID(int.Parse(dr[0].ToString()));
                                access.SetIsReadWrite(true);
                                saved = access.Save();
                            }
                            dr.Close();
                        }


                        if (tableName.Equals("AD_Form", StringComparison.OrdinalIgnoreCase))
                        {
                            //Assing role to the new menu created

                            IDataReader dr = GetRoles();
                            bool saved = false;
                            while (dr.Read())
                            {
                                X_AD_Form_Access access = new X_AD_Form_Access(GetCtx(), 0, null);
                                access.SetAD_Form_ID(NewPrimaryKey);
                                access.SetAD_Role_ID(int.Parse(dr[0].ToString()));
                                access.SetIsReadWrite(true);
                                saved = access.Save();

                            }
                            dr.Close();
                        }

                        //assign role acces to workflow
                        if (tableName.Equals("AD_Workflow", StringComparison.OrdinalIgnoreCase))
                        {
                            //Assing role to the new menu created

                            IDataReader dr = GetRoles();
                            bool saved = false;
                            while (dr.Read())
                            {
                                X_AD_Workflow_Access access = new X_AD_Workflow_Access(GetCtx(), 0, null);
                                access.SetAD_Workflow_ID(NewPrimaryKey);
                                access.SetAD_Role_ID(int.Parse(dr[0].ToString()));
                                access.SetIsReadWrite(true);
                                saved = access.Save();
                            }
                            dr.Close();
                        }



                        if (tableName.Equals("AD_Column", StringComparison.OrdinalIgnoreCase))
                        {
                            // Commented Code
                            // As Primary key was not unique  if we sync column here

                            // DoColumnSync(NewPrimaryKey);
                        }

                        //Reset the primary values
                        NewPrimaryKey = 0;
                        OldPrimaryKey = 0;
                    }
                }
                catch (Exception ex)
                {
                    sbMsg.Append("<br />").Append(ex.Message);
                    SendMessage(ex.Message);
                    //_ErroLog.Add("Error: " + ex.Message);
                    //log.Severe(ex.Message);
                }
                SendMessage(sbMsg.ToString(), true, true);
            }

            //update ad_ref_table
            if (ds.Tables.Contains("AD_Ref_Table"))
                Add_AD_Ref_Tables(ds.Tables["AD_Ref_Table"].Select());


            if (ds.Tables.Contains("AD_Workflow"))
            {
                DataTable dt = ds.Tables["AD_Workflow"];
                for (int x = 0; x < dt.Rows.Count; x++)
                {
                    String updateworkflow = "UPDATE AD_Workflow SET AD_WF_Node_ID = @AD_WF_Node_ID WHERE AD_Workflow_ID = @AD_Workflow_ID";
                    Object wf_node_id = dt.Rows[x]["AD_WF_Node_ID"];
                    Object workflow_id = dt.Rows[x]["AD_Workflow_ID"];

                    if (wf_node_id != null && wf_node_id != DBNull.Value)
                    {
                        System.Data.SqlClient.SqlParameter[] updateparam = new System.Data.SqlClient.SqlParameter[2];
                        updateparam[0] = new System.Data.SqlClient.SqlParameter("@AD_WF_Node_ID", wf_node_id);
                        updateparam[1] = new System.Data.SqlClient.SqlParameter("@AD_Workflow_ID", workflow_id);
                        int updatenode = DB.ExecuteQuery(updateworkflow, updateparam, null);
                        if (updatenode > 0)
                        {
                            //_ImportInfo.Add("**Updated AD_WF_Node in AD_Workflow");
                        }
                        else
                        {

                        }
                    }
                }
            }

            if (ds.Tables.Contains("AD_Column") && _isLastClientRecordOfSchemaOrData)
            {

                DataTable dt = ds.Tables["AD_Column"];
                SendMessage("Column Sync Start " + dt.Rows.Count, true, false);
                /* Create MetaDataObject Once */
                mdObject = new MarketSvc.Classes.DatabaseMetaData();
                fkDataTable = new Dictionary<string, DataTable>(5);

                StringBuilder sb = new StringBuilder();

                for (int x = 0; x < dt.Rows.Count; x++)
                {
                    string msg = DoColumnSync(Util.GetValueOfInt(dt.Rows[x]["AD_Column_ID"]));
                    SendMessage(msg);
                    sb.Append(msg).Append("<br />");
                    if (x > 0 && (x % 50) == 0)
                    {
                        SendMessage(sb.ToString(), true, true);
                        sb.Clear();
                    }
                }

                if (sb.Length > 0)
                {
                    SendMessage(sb.ToString(), true, true);
                    sb.Clear();
                }

                mdObject.Dispose();
                mdObject = null;
                fkDataTable.Clear();
                fkDataTable = null;


                SendMessage("Column Sync END", true, false);

                System.Threading.Thread.Sleep(500); // 5 seconds sleep
            }



            //at last execute script 
            if (ds.Tables.Contains("AD_Module_DBScript") && _isLastClientRecordOfSchemaOrData)
            {
                SendMessage("Running Scripts", true, false);
                // // Changed Code to Execute Scripts based on the column IsPreExecuteScript
                // if UnChecked then execute unchecked Script here else execute all

                if (ds.Tables["AD_Module_DBScript"].Columns["IsPreExecuteScript"] != null)
                {
                    DataRow[] scripts = ds.Tables["AD_Module_DBScript"].Select("IsPreExecuteScript" + " = 'N' ");
                    int excecuted = ExecuteScript(scripts);
                }
                else
                {
                    DataRow[] scripts = ds.Tables["AD_Module_DBScript"].Select();
                    int excecuted = ExecuteScript(scripts);
                }
                SendMessage("End Scripts", true, false);
            }

            if (dsExportedData != null)
            {
                dsExportedData.Clear();
                dsExportedData = null;
            }
            return "ImportDone";
        }
        #endregion
        /// <summary>
        /// Updates the dependencies in rest of the dataset
        /// </summary>
        /// <param name="PrimaryColumn">Primary column of the table to be updated</param>
        /// <param name="tableName">Name of the table</param>
        /// <param name="tables">Counter number of the table in the list</param>
        /// 
        /* BF = BugFix */
        /* [BF_10116_01]  refList records are not properly insertrd*/
        /* [BF_10116_02]  refTable records are not properly insertrd*/
        /* [BF_10116_03]  Zoom window Id against field is not imported properly.*/
        ///
        void UpdateDependencies(String PrimaryColumn, String tableName, int tables, bool isInsertMode)
        {
            try
            {
                String OriginalExport_ID = String.Empty;
                String tPrimaryColumn = String.Empty;
                String CurrentTable = String.Empty;
                bool skipcheck = false;

                List<String> ref_primary_Ids = new List<string>();
                /*OR_1*/
                //Original code 
                // for (int b = (tables - 1); b >= 0; b--) // iterate all the tables from the current position to zero
                for (int b = 0; b < uniqueTableName.Count; b++) // modified to reduce iteration 
                {
                    if (!String.IsNullOrEmpty(tPrimaryColumn))
                    {
                        PrimaryColumn = tPrimaryColumn;
                        tPrimaryColumn = String.Empty;
                    }
                    skipcheck = false;
                    /*OR_1*/
                    //CurrentTable = _tableList[b]["TableName"].ToString();
                    CurrentTable = uniqueTableName[b];


                    ref_primary_Ids.Clear();


                    ////only for ad_column and ad_ref_tabe which contains two fk tables against ad_column
                    //if ((tableName.Equals("AD_Column", StringComparison.OrdinalIgnoreCase) && CurrentTable.Equals("AD_Ref_Table", StringComparison.OrdinalIgnoreCase))
                    //    || (tableName.Equals("AD_Window", StringComparison.OrdinalIgnoreCase) && CurrentTable.Equals("AD_Field", StringComparison.OrdinalIgnoreCase))) //BF_10116_03
                    //{
                    //    skipcheck = true;
                    //}



                    if (_tableReferenceColumns.Keys.Contains(CurrentTable) && _tableReferenceColumns[CurrentTable].Keys.Contains(PrimaryColumn))
                    {
                        ref_primary_Ids.AddRange(_tableReferenceColumns[CurrentTable][PrimaryColumn]);
                        ref_primary_Ids.Insert(0, PrimaryColumn);
                    }


                    else if (ds.Tables[CurrentTable].Columns.Contains(PrimaryColumn) || skipcheck)
                    {
                        ref_primary_Ids.Insert(0, PrimaryColumn);
                    }


                    if (ref_primary_Ids.Count < 1)
                    {
                        continue;

                    }
                    if (CurrentTable.Equals("AD_Column", StringComparison.OrdinalIgnoreCase) || CurrentTable.Equals("AD_Process_Para", StringComparison.OrdinalIgnoreCase)
                        || CurrentTable.Equals("AD_CrystalReport_Para", StringComparison.OrdinalIgnoreCase))
                    {
                        if (PrimaryColumn.ToLower() == "ad_reference_id" && ref_primary_Ids.Contains(PrimaryColumn))
                        {
                            ref_primary_Ids.Remove(PrimaryColumn);
                        }
                    }

                    //if (ds.Tables[CurrentTable].Columns.Contains(PrimaryColumn) || skipcheck)
                    //{

                    //if (CurrentTable.Equals("AD_Column", StringComparison.OrdinalIgnoreCase) && PrimaryColumn.Equals("AD_Reference_ID", StringComparison.OrdinalIgnoreCase))
                    //{
                    //    tPrimaryColumn = PrimaryColumn;
                    //    PrimaryColumn = "AD_Reference_Value_ID";
                    //}
                    //if (CurrentTable.Equals("AD_Process_Para", StringComparison.OrdinalIgnoreCase) && PrimaryColumn.Equals("AD_Reference_ID", StringComparison.OrdinalIgnoreCase))
                    //{
                    //    tPrimaryColumn = PrimaryColumn;
                    //    PrimaryColumn = "AD_Reference_Value_ID";
                    //}

                    ////Update ad_reference_id in ad_crystal_para table 
                    ////ad_referebce _id as ad_reference_value_id
                    //if (CurrentTable.Equals("AD_CrystalReport_Para", StringComparison.OrdinalIgnoreCase) && PrimaryColumn.Equals("AD_Reference_ID", StringComparison.OrdinalIgnoreCase))
                    //{
                    //    tPrimaryColumn = PrimaryColumn;
                    //    PrimaryColumn = "AD_Reference_Value_ID";
                    //}


                    //ref_primary_Ids.Clear(); //Handle case: if same key referenced, multiple time in a table [AD_Sub_Group_ID_1]
                    //ref_primary_Ids.Add(PrimaryColumn); //Add Primary column

                    //if (string.IsNullOrEmpty(tPrimaryColumn))
                    //{
                    //    tPrimaryColumn = PrimaryColumn;
                    //}

                    //if (_isDataMigration)
                    //{

                    //    for (int ii = 1; ii < 4; ii++)
                    //    {
                    //        //if (ii == 0)
                    //        // {
                    //        ref_primary_Ids.Add(PrimaryColumn + "_" + ii);
                    //        // }
                    //        //else
                    //        // {
                    //        //   ref_primary_Ids.Add("Ref"+ii+"_" + PrimaryColumn);
                    //        //}
                    //    }
                    //}







                    //if (CurrentTable.Equals("AD_WF_NodeNext", StringComparison.OrdinalIgnoreCase) && PrimaryColumn.Equals("AD_WF_Node_ID", StringComparison.OrdinalIgnoreCase))
                    //{
                    //    tPrimaryColumn = PrimaryColumn;
                    //    ref_primary_Ids.Add("AD_WF_Next_ID");
                    //}

                    ////Update Pint Format Child Reference Also in AD_PrintFormatItem Table
                    //if (CurrentTable.Equals("AD_PrintFormatItem", StringComparison.OrdinalIgnoreCase) && PrimaryColumn.Equals("AD_PrintFormat_ID", StringComparison.OrdinalIgnoreCase))
                    //{
                    //    tPrimaryColumn = PrimaryColumn;
                    //    ref_primary_Ids.Add("AD_PrintFormatChild_ID");
                    //}


                    ////skipcheck work only ad_ref_table. this is a special table which has no primary key
                    ////and had two columns as fk to ad_column
                    ////[BF_10116_02]  
                    //if (skipcheck)
                    //{
                    //    tPrimaryColumn = PrimaryColumn;
                    //    ref_primary_Ids.Clear();
                    //    if (tableName.Equals("AD_Column", StringComparison.OrdinalIgnoreCase))
                    //    {
                    //        ref_primary_Ids.Add("Column_Display_ID");
                    //        ref_primary_Ids.Add("Column_Key_ID");
                    //    }
                    //    else //BF_10116_03
                    //    {
                    //        ref_primary_Ids.Add("ZoomWindow_ID");
                    //    }
                    //}



                    if (string.IsNullOrEmpty(tPrimaryColumn)) //store Original Primary Column
                    {
                        tPrimaryColumn = PrimaryColumn;
                    }


                    DataRow[] rows = null;
                    DataRow[] tempData = null;

                    for (int ids = 0; ids < ref_primary_Ids.Count; ids++)
                    {
                        rows = null;
                        tempData = null;
                        PrimaryColumn = ref_primary_Ids[ids];
                        //find export_id
                        if (dsTemp.Tables[CurrentTable].Columns.Contains(PrimaryColumn))
                        {
                            tempData = dsTemp.Tables[CurrentTable].Select(PrimaryColumn + " = " + OldPrimaryKey);

                            //[BF_10116_01]  // appned export id check 
                            // wrote this line outside from if statement 
                            OriginalExport_ID = " AND Export_ID IN (";

                            if (tempData.Length > 0)
                            {
                                // OriginalExport_ID = " AND Export_ID IN (";
                                foreach (DataRow trow in tempData)
                                {
                                    OriginalExport_ID += DB.TO_STRING(trow["Export_ID"].ToString());        //fetch export id from the data row available
                                    OriginalExport_ID += ",";
                                }
                            }
                            OriginalExport_ID += "'0')"; // pull out this line outside from if statement 
                        }

                        if (ds.Tables[CurrentTable].Columns.Contains(PrimaryColumn))
                            rows = ds.Tables[CurrentTable].Select(PrimaryColumn + " = " + OldPrimaryKey + OriginalExport_ID);


                        //skipcheck work only ad_ref_table. this is a special table which has no primary key
                        //and had two columns as fk to ad_column
                        //if (skipcheck)
                        //{
                        //String[] refTable_cols = new String[] { "Column_Display_ID", "Column_Key_ID" };
                        //for (int c = 0; c < 2; c++)
                        //{

                        //    DataRow[] refrows = ds.Tables[CurrentTable].Select(refTable_cols[c] + " = " + OldPrimaryKey);
                        //    foreach (DataRow row in refrows)
                        //    {
                        //        row.BeginEdit();
                        //        row[refTable_cols[c]] = NewPrimaryKey;
                        //        row.EndEdit();
                        //    }
                        //}
                        //}

                        if (rows != null)
                        {
                            foreach (DataRow row in rows)
                            {
                                row.BeginEdit();
                                row[PrimaryColumn] = NewPrimaryKey;
                                //_ImportInfo.Add("**Updated column " + PrimaryColumn + " in dependency table " + CurrentTable);
                                //if (CurrentTable.Equals("AD_WF_NodeNext", StringComparison.OrdinalIgnoreCase))
                                //{
                                //    row["AD_WF_Next_ID"] = NewPrimaryKey;
                                //    _ImportInfo.Add("**Updated column AD_WF_NodeNext_ID in dependency table " + CurrentTable);
                                //}
                                row.EndEdit();

                            }
                        }
                    }
                    //}
                }
            }
            catch (Exception ex)
            {
                SendMessage("Error in method UpdateDependencies: " + ex.Message);

                //_ErroLog.Add(" Error in method UpdateDependencies: " + ex.Message);
            }
        }


        void Add_AD_Ref_Tables(DataRow[] rows)
        {
            try
            {
                foreach (DataRow row in rows)
                {

                    string export_Id = row["Export_ID"].ToString();

                    DataRow[] oldrefRow = dsTemp.Tables["AD_Ref_Table"].Select("Export_ID = " + DB.TO_STRING(export_Id));
                    if (oldrefRow.Count() > 0)
                    {
                        SendMessage("<br />Processing table AD_Ref_Table  <===>[" + oldrefRow[0]["AD_Reference_ID"] + "]<->[" + row["AD_Reference_ID"] + "]");
                    }

                    String _sql = "Select Count(1) from AD_Ref_Table WHERE Export_ID = " + DB.TO_STRING(Convert.ToString(row["Export_ID"]));
                    Object result = DB.ExecuteScalar(_sql);

                    if (Convert.ToInt16(result) > 0)
                    {
                        DB.ExecuteQuery("DELETE FROM AD_Ref_Table WHERE Export_ID = " + DB.TO_STRING(Convert.ToString(row["Export_ID"])));
                    }


                    X_AD_Ref_Table refTable = new X_AD_Ref_Table(GetCtx(), 0, null);
                    refTable.SetAD_Reference_ID(Convert.ToInt32(row["AD_Reference_ID"]));
                    refTable.SetAD_Table_ID(Convert.ToInt32(row["AD_Table_ID"]));
                    refTable.SetColumn_Display_ID(Convert.ToInt32(row["Column_Display_ID"]));
                    refTable.SetColumn_Key_ID(Convert.ToInt32(row["Column_Key_ID"]));
                    refTable.SetEntityType(Convert.ToString(row["EntityType"]));
                    refTable.SetIsValueDisplayed(row["IsValueDisplayed"].ToString().Equals("Y"));
                    refTable.SetOrderByClause(Convert.ToString(row["OrderByClause"]));
                    refTable.SetWhereClause(Convert.ToString(row["WhereClause"]));
                    refTable.SetExport_ID(Convert.ToString(row["Export_ID"]));
                    refTable.Set_Value("IsDisplayIdentifiers", row["ISDISPLAYIDENTIFIERS"].ToString() == "Y");

                    bool saved = refTable.Save(null);

                    ValueNamePair v = VLogger.RetrieveError();

                    if (saved)
                    {
                        //_ImportInfo.Add("Inserted into AD_Ref_Table");
                        SendMessage("Inserted into AD_Ref_Table " + refTable.GetAD_Reference_ID());
                    }
                    else
                    {
                        //_ImportInfo.Add("Error ocurred in AD_Ref_Table: " + v.GetName());
                        SendMessage(" Error ocurred in AD_Ref_Table: " + v.GetName());
                    }
                }
            }
            catch (Exception ex)
            {
                //_ErroLog.Add(" Error in method Add_AD_Ref_Table: " + ex.Message);
                // _ImportInfo.Add(" Error in method Add_AD_Ref_Table: " + ex.Message);
                SendMessage(" Error in method Add_AD_Ref_Table: " + ex.Message);
            }
        }

        /// <summary>
        /// Manages the workflow tables while importing
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <param name="param">Parameters</param>
        void ManageWorkflowTables(String tableName, ref System.Data.SqlClient.SqlParameter[] param, bool is_wf_node)
        {
            try
            {
                if (tableName.Equals("AD_Workflow", StringComparison.OrdinalIgnoreCase))
                {
                    System.Data.SqlClient.SqlParameter sqlp = param.FirstOrDefault((d) => d.ParameterName.Equals("@AD_WF_Node_ID"));
                    sqlp.Value = DBNull.Value;
                }
                else if (tableName.Equals("AD_WF_Node", StringComparison.OrdinalIgnoreCase))
                {
                    if (!is_wf_node)
                        return;

                    //update workflow
                    //String updateworkflow = "UPDATE AD_Workflow SET AD_WF_Node_ID = @AD_WF_Node_ID WHERE AD_Workflow_ID = @AD_Workflow_ID";
                    //Object wf_node_id = param.FirstOrDefault((d) => d.ParameterName.Equals("@AD_WF_Node_ID")).Value;
                    //Object workflow_id = param.FirstOrDefault((d) => d.ParameterName.Equals("@AD_Workflow_ID")).Value;

                    //System.Data.SqlClient.SqlParameter[] updateparam = new System.Data.SqlClient.SqlParameter[2];
                    //updateparam[0] = new System.Data.SqlClient.SqlParameter("@AD_WF_Node_ID", wf_node_id);
                    //updateparam[1] = new System.Data.SqlClient.SqlParameter("@AD_Workflow_ID", workflow_id);

                    //int updatenode = DB.ExecuteQuery(updateworkflow, updateparam, null);

                    //if (updatenode > 0)
                    //    _ImportInfo.Add("**Updated AD_WF_Node in AD_Workflow");

                }
            }
            catch (Exception ex)
            {
                //_ErroLog.Add(" Error in method ManageWorkflowTables: " + ex.Message);
                // _ImportInfo.Add(" Error in method ManageWorkflowTables: " + ex.Message);
                SendMessage("Error in method ManageWorkflowTables: " + ex.Message);
            }
        }

        ///before Menu Settings

        //public void AddWindowToMenu(string tableName, int ID, String parentFolder, String subParentFolder, String menuName, string menuitem_exportid, String ParentExportID)
        //{
        //    try
        //    {
        //        String __query = "";
        //        String MenuAction = "W";
        //        //String _sql = "SELECT AD_Menu_ID FROM AD_Menu WHERE IsActive = 'Y' AND Upper(Name) = " + DB.TO_STRING(parentFolder.ToUpper());
        //        //Object ad_menu_id = DB.ExecuteScalar(_sql);
        //        //int menuid = 0;
        //        MMenu menu = null;

        //        bool saved = false;
        //        //if (ad_menu_id != null)
        //        //{
        //        //    menuid = Convert.ToInt32(ad_menu_id);
        //        //}
        //        //else
        //        //{
        //        //    menu = new MMenu(GetCtx(), 0, null);
        //        //    menu.SetName(parentFolder);
        //        //    menu.SetIsSummary(true);
        //        //    menu.Save();
        //        //    menuid = menu.Get_ID();



        //        //    //mtreenodemm = new MTreeNodeMM(MTree.Get(GetCtx(), 121, null), menu.Get_ID());
        //        //    //saved = mtreenodemm.Save();
        //        //}


        //        int menuid = ModuleMenuParentID;
        //        String _sql = String.Empty;
        //        MMenu submenu = null;

        //        if (!String.IsNullOrEmpty(subParentFolder))
        //        {
        //            _sql = "SELECT AD_Menu_ID FROM AD_Menu WHERE IsActive = 'Y' AND Upper(Export_ID) = " + DB.TO_STRING(ParentExportID.ToUpper());
        //            Object ad_menu_id = DB.ExecuteScalar(_sql);
        //            if (ad_menu_id != null)
        //            {
        //                submenu = new MMenu(GetCtx(), Convert.ToInt32(ad_menu_id), null);
        //            }
        //            else
        //            {

        //                submenu = new MMenu(GetCtx(), 0, null);
        //                submenu.SetName(subParentFolder);
        //                submenu.SetIsSummary(true);
        //                submenu.SetExport_ID(ParentExportID);
        //                submenu.Save();

        //            }
        //            //Update Parent Id to zero if parent menu and current record ids are same
        //            __query = "Update AD_TreeNodeMM SET Parent_ID = " + (submenu.Get_ID() == menuid ? 0 : menuid).ToString() + " WHERE Node_ID = " + submenu.Get_ID();
        //            DB.ExecuteQuery(__query);

        //            //mtreenodemm = new MTreeNodeMM(MTree.Get(GetCtx(), 121, null), submenu.Get_ID());
        //            //mtreenodemm.SetParent_ID(menuid);
        //            //saved = mtreenodemm.Save();

        //        }

        //        //menuid = menu.Get_ID();
        //        int subid = 0;

        //        if (submenu != null)
        //            subid = submenu.Get_ID();


        //        //check if menu already exists
        //        _sql = "SELECT AD_Menu_ID From AD_Menu WHERE Export_ID = " + DB.TO_STRING(menuitem_exportid);
        //        int oldmenuid = Convert.ToInt32(DB.ExecuteScalar(_sql));

        //        menu = new MMenu(GetCtx(), oldmenuid, null);
        //        menu.SetName(menuName);
        //        menu.SetExport_ID(menuitem_exportid);
        //        //menu.Set_Value(tableName + "_ID", ID);
        //        menu.SetIsSummary(false);

        //        if (tableName.Equals("AD_ModuleWindow", StringComparison.OrdinalIgnoreCase))
        //        {
        //            MenuAction = "W";
        //            menu.SetAction("W");
        //            menu.SetAD_Window_ID(ID);
        //        }
        //        else if (tableName.Equals("AD_ModuleForm", StringComparison.OrdinalIgnoreCase))
        //        {
        //            MenuAction = "X";
        //            menu.SetAction("X");
        //            menu.SetAD_Form_ID(ID);
        //        }
        //        else if (tableName.Equals("AD_ModuleProcess", StringComparison.OrdinalIgnoreCase))
        //        {
        //            MenuAction = "P";

        //            menu.SetAction("P");
        //            //BUG 002  Show Menu As Report menu If Process marked as Report
        //            if (DB.ExecuteScalar("SELECT IsReport FROM AD_Process WHERE AD_Process_ID =" + ID).ToString() == "Y")
        //            {
        //                menu.SetAction("R");
        //            }
        //            menu.SetAD_Process_ID(ID);
        //        }

        //        if (menu.Save())
        //        {
        //            //_ImportInfo.Add("Item added to menu");

        //            //Assing role to the new menu to be created
        //            saved = false;
        //            IDataReader dr = GetRoles();
        //            while (dr.Read())
        //            {
        //                int roleid = int.Parse(dr[0].ToString());
        //                if (MenuAction.Equals("W", StringComparison.OrdinalIgnoreCase))
        //                {
        //                    X_AD_Window_Access access = new X_AD_Window_Access(GetCtx(), 0, null);
        //                    access.SetAD_Window_ID(ID);
        //                    access.SetAD_Role_ID(roleid);
        //                    access.SetIsReadWrite(true);
        //                    saved = access.Save();
        //                }
        //                else if (MenuAction.Equals("P", StringComparison.OrdinalIgnoreCase))
        //                {
        //                    X_AD_Process_Access access = new X_AD_Process_Access(GetCtx(), 0, null);
        //                    access.SetAD_Process_ID(ID);
        //                    access.SetAD_Role_ID(roleid);
        //                    access.SetIsReadWrite(true);
        //                    saved = access.Save();
        //                }
        //                else if (MenuAction.Equals("X", StringComparison.OrdinalIgnoreCase))
        //                {
        //                    X_AD_Form_Access access = new X_AD_Form_Access(GetCtx(), 0, null);
        //                    access.SetAD_Form_ID(ID);
        //                    access.SetAD_Role_ID(roleid);
        //                    access.SetIsReadWrite(true);
        //                    saved = access.Save();
        //                }
        //            }
        //            dr.Close();
        //        }
        //        else
        //        {
        //            //_ImportInfo.Add("Item could not be added to menu");
        //        }

        //        __query = "Update AD_TreeNodeMM SET Parent_ID = " + (subid.Equals(0) ? menuid : subid) + " WHERE Node_ID = " + menu.Get_ID();


        //        SendMessage("added menu item ==>" + DB.ExecuteQuery(__query));

        //        //mtreenodemm = new MTreeNodeMM(MTree.Get(GetCtx(), 121, _trx), menu.Get_ID());
        //        //mtreenodemm.SetParent_ID(subid.Equals(0) ? menuid : subid);

        //        //mtreenodemm.Save();



        //    }
        //    catch (Exception ex)
        //    {
        //        //_ErroLog.Add(" Error in method AddWindowToMenu: " + ex.Message);
        //        SendMessage(" Error in method AddWindowToMenu: " + ex.Message);
        //    }
        //}

        /// After Menu Settings

        public void AddWindowToMenu(string tableName, int ID, String parentFolder, String subParentFolder, String menuName, string menuitem_exportid, String ParentExportID, int seqNo)
        {
            try
            {
                String __query = "";
                String MenuAction = "W";
                //String _sql = "SELECT AD_Menu_ID FROM AD_Menu WHERE IsActive = 'Y' AND Upper(Name) = " + DB.TO_STRING(parentFolder.ToUpper());
                //Object ad_menu_id = DB.ExecuteScalar(_sql);
                //int menuid = 0;
                MMenu menu = null;

                bool saved = false;
                //if (ad_menu_id != null)
                //{
                //    menuid = Convert.ToInt32(ad_menu_id);
                //}
                //else
                //{
                //    menu = new MMenu(GetCtx(), 0, null);
                //    menu.SetName(parentFolder);
                //    menu.SetIsSummary(true);
                //    menu.Save();
                //    menuid = menu.Get_ID();



                //    //mtreenodemm = new MTreeNodeMM(MTree.Get(GetCtx(), 121, null), menu.Get_ID());
                //    //saved = mtreenodemm.Save();
                //}


                int menuid = ModuleMenuParentID;
                String _sql = String.Empty;
                MMenu submenu = null;

                if (!String.IsNullOrEmpty(subParentFolder))
                {
                    //_sql = "SELECT AD_Menu_ID FROM AD_Menu WHERE IsActive = 'Y' AND Upper(Export_ID) = " + DB.TO_STRING(ParentExportID.ToUpper());
                    _sql = "SELECT AD_Menu_ID FROM AD_Menu WHERE Upper(Export_ID) = " + DB.TO_STRING(ParentExportID.ToUpper());
                    Object ad_menu_id = DB.ExecuteScalar(_sql);
                    if (ad_menu_id != null)
                    {
                        submenu = new MMenu(GetCtx(), Convert.ToInt32(ad_menu_id), null);
                    }
                    else
                    {

                        submenu = new MMenu(GetCtx(), 0, null);
                        submenu.SetName(subParentFolder);
                        submenu.SetIsSummary(true);
                        submenu.SetExport_ID(ParentExportID);
                        submenu.Save();

                    }
                    //Update Parent Id to zero if parent menu and current record ids are same
                    // __query = "Update AD_TreeNodeMM SET Parent_ID = " + (submenu.Get_ID() == menuid ? 0 : menuid).ToString() + " WHERE Node_ID = " + submenu.Get_ID();
                    // DB.ExecuteQuery(__query);

                    //mtreenodemm = new MTreeNodeMM(MTree.Get(GetCtx(), 121, null), submenu.Get_ID());
                    //mtreenodemm.SetParent_ID(menuid);
                    //saved = mtreenodemm.Save();
                    //
                }

                //menuid = menu.Get_ID();
                int subid = 0;

                if (submenu != null)
                    subid = submenu.Get_ID();


                //check if menu already exists
                // _sql = "SELECT AD_Menu_ID From AD_Menu WHERE IsActive = 'Y' AND Export_ID = " + DB.TO_STRING(menuitem_exportid);
                _sql = "SELECT AD_Menu_ID From AD_Menu WHERE Upper(Export_ID) = " + DB.TO_STRING(menuitem_exportid.ToString());
                int oldmenuid = Convert.ToInt32(DB.ExecuteScalar(_sql));

                menu = new MMenu(GetCtx(), oldmenuid, null);
                menu.SetName(menuName);
                menu.SetExport_ID(menuitem_exportid);
                //menu.Set_Value(tableName + "_ID", ID);
                menu.SetIsSummary(false);

                if (tableName.Equals("AD_ModuleWindow", StringComparison.OrdinalIgnoreCase))
                {
                    MenuAction = "W";
                    menu.SetAction("W");
                    menu.SetAD_Window_ID(ID);
                }
                else if (tableName.Equals("AD_ModuleForm", StringComparison.OrdinalIgnoreCase))
                {
                    MenuAction = "X";
                    menu.SetAction("X");
                    menu.SetAD_Form_ID(ID);
                }
                else if (tableName.Equals("AD_ModuleProcess", StringComparison.OrdinalIgnoreCase))
                {
                    MenuAction = "P";

                    menu.SetAction("P");
                    //BUG 002  Show Menu As Report menu If Process marked as Report
                    if (DB.ExecuteScalar("SELECT IsReport FROM AD_Process WHERE AD_Process_ID =" + ID).ToString() == "Y")
                    {
                        menu.SetAction("R");
                    }
                    menu.SetAD_Process_ID(ID);
                }

                if (menu.Save())
                {
                    //_ImportInfo.Add("Item added to menu");

                    //Assing role to the new menu to be created
                    saved = false;
                    IDataReader dr = GetRoles();
                    while (dr.Read())
                    {
                        int roleid = int.Parse(dr[0].ToString());
                        if (MenuAction.Equals("W", StringComparison.OrdinalIgnoreCase))
                        {
                            X_AD_Window_Access access = new X_AD_Window_Access(GetCtx(), 0, null);
                            access.SetAD_Window_ID(ID);
                            access.SetAD_Role_ID(roleid);
                            access.SetIsReadWrite(true);
                            saved = access.Save();
                        }
                        else if (MenuAction.Equals("P", StringComparison.OrdinalIgnoreCase))
                        {
                            X_AD_Process_Access access = new X_AD_Process_Access(GetCtx(), 0, null);
                            access.SetAD_Process_ID(ID);
                            access.SetAD_Role_ID(roleid);
                            access.SetIsReadWrite(true);
                            saved = access.Save();
                        }
                        else if (MenuAction.Equals("X", StringComparison.OrdinalIgnoreCase))
                        {
                            X_AD_Form_Access access = new X_AD_Form_Access(GetCtx(), 0, null);
                            access.SetAD_Form_ID(ID);
                            access.SetAD_Role_ID(roleid);
                            access.SetIsReadWrite(true);
                            saved = access.Save();
                        }
                    }
                    dr.Close();
                }
                else
                {
                    //_ImportInfo.Add("Item could not be added to menu");
                }

                __query = "Update AD_TreeNodeMM SET Parent_ID = " + (subid.Equals(0) ? menuid : subid) + ", SeqNo = " + seqNo + " WHERE Node_ID = " + menu.Get_ID();


                SendMessage("added menu item ==>" + DB.ExecuteQuery(__query));

                //mtreenodemm = new MTreeNodeMM(MTree.Get(GetCtx(), 121, _trx), menu.Get_ID());
                //mtreenodemm.SetParent_ID(subid.Equals(0) ? menuid : subid);

                //mtreenodemm.Save();



            }
            catch (Exception ex)
            {
                //_ErroLog.Add(" Error in method AddWindowToMenu: " + ex.Message);
                SendMessage(" Error in method AddWindowToMenu: " + ex.Message);
            }
        }

        private IDataReader GetRoles()
        {
            String __inclients = String.Empty;
            foreach (int i in clients)
            {
                if (!String.IsNullOrEmpty(__inclients))
                {
                    __inclients += ",";
                }
                __inclients += i;
            }
            //__inclients = __inclients.Substring(0, __inclients.Length - 1);
            String __rolequery = "SELECT AD_Role_ID From AD_Role Where IsAdministrator='Y' and IsActive = 'Y' and AD_Client_ID IN(" + __inclients + ")";
            IDataReader dr = DB.ExecuteReader(__rolequery);

            return dr;
        }





        /// <summary>
        /// cteare column in database
        /// </summary>
        /// <param name="p_AD_Column_ID">column id</param>
        /// <returns>error msg if any</returns>
        protected string DoColumnSync(int p_AD_Column_ID)
        {
            string tableName = "";
            try
            {
                string exception = "";
                //log.Info("C_Column_ID=" + p_AD_Column_ID);
                if (p_AD_Column_ID == 0)
                {
                    //    return "";
                    throw new Exception("@No@ @AD_Column_ID@");
                }
                //IDbTransaction trx = ExecuteQuery.GerServerTransaction();
                MColumn column = new MColumn(GetCtx(), p_AD_Column_ID, null);
                if (column.Get_ID() == 0)
                {

                    throw new Exception("@NotFound@ @AD_Column_ID@" + p_AD_Column_ID);
                }

                MTable table = MTable.Get(GetCtx(), column.GetAD_Table_ID());
                if (table.Get_ID() == 0)
                {

                    throw new Exception("@NotFound@ @AD_Table_ID@" + column.GetAD_Table_ID());
                }

                if (table.IsView()) //Do not create[Sync] column if table is view
                {
                    return "Skip View =>" + table.GetName();
                }


                //	Find Column in Database

                //DatabaseMetaData md = new DatabaseMetaData();
                String catalog = "";
                String schema = VAdvantage.DataBase.DB.GetSchema();

                //get table name
                tableName = table.GetTableName();

                if (DatabaseType.IsOracle)
                    tableName = tableName.ToUpper();

                int noColumns;
                string sql = null;
                //get columns of a table
                DataSet dt = null;
                // using (MarketSvc.DatabaseMetaData md = new MarketSvc.DatabaseMetaData())
                //{
                dt = mdObject.GetColumns(catalog, schema, tableName);
                //}

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
                        return "Skip " + column.GetColumnName() + "[VirtualColumn]";
                    }
                    sql = column.GetSQLAdd(table);
                }

                string retMsg = column.GetColumnName() + "[" + table.GetTableName() + "] ";

                int no = 0;
                if (sql.IndexOf(";") == -1)
                {
                    //no = 
                    //ExecuteQuery.ExecuteNonQuery(sql, false, get_TrxName());
                    try
                    {
                        no = VAdvantage.DataBase.DB.ExecuteQuery(sql, null, null);
                        //if (DatabaseType.IsPostgre)
                        //{
                        //    if (no == -1)
                        //        no = 1;
                        //}
                        //AddLog(0, DateTime.MinValue, Decimal.Parse(no.ToString()), sql);
                    }
                    catch (Exception ex)
                    {
                        SendMessage("Error in Column Sync for table " + tableName + "===>" + ex.Message);
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
                        int count = VAdvantage.DataBase.DB.ExecuteQuery(statements[i].ToString(), null, null);
                        //AddLog(0, DateTime.MinValue, Decimal.Parse(count.ToString()), statements[i]);
                        //if (DatabaseType.IsPostgre)
                        //{
                        //    if (no == -1)
                        //    {
                        //        no = 1;
                        //    }
                        //}
                        //no += count;
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
                    //throw new Exception(msg);
                    throw new Exception(retMsg + " ==> " + no);
                }
                string r = "";
                if (!_isDataMigration)
                {
                    string cType = Util.GetValueOfString(column.GetConstraintType());//Get Contaraint type
                    if (_dicColumnCostarintType.Keys.Contains(p_AD_Column_ID)) //If Conatin column id
                    {
                        if (cType != _dicColumnCostarintType[p_AD_Column_ID]) // contraint type not equel to old type
                        {
                            r = CreateFK(column, table); //createForeign key 
                        }
                    }
                    //If new Column
                    else if (!string.IsNullOrEmpty(cType) && cType != X_AD_Column.CONSTRAINTTYPE_DoNOTCreate) // Has Containt type
                    {
                        r = CreateFK(column, table);
                    }
                }
                //return sql + "; " + r;
                return retMsg + r + " ==> " + no;

            }
            catch (Exception ex)
            {
                //_ErroLog.Add("ColumnSync: " + ex.Message);
                SendMessage("Error in Column Sync for table " + tableName + "===>" + ex.Message);
                return ex.Message;
                //return retMsg + r + " ==> -1" ;
            }

            //return "";
        }	//	doIt

        Dictionary<String, DataTable> fkDataTable = null;

        private String CreateFK(MColumn column, MTable table)
        {
            String returnMessage = "";
            //if (p_AD_Column_ID == 0)
            //  throw new Exception("@No@ @AD_Column_ID@");
            //MColumn column = new MColumn(GetCtx(), p_AD_Column_ID, null);

            int p_AD_Column_ID = column.Get_ID();

            if (column.Get_ID() == 0)
                throw new Exception("@NotFound@ @AD_Column_ID@ " + p_AD_Column_ID);

            //MTable table = MTable.Get(GetCtx(), column.GetAD_Table_ID());
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
            DataSet ds = null;
            DataTable dTable = null;
            try
            {
                /*Find foreign key relation in Database
                 * */
                //Trx trx = Trx.Get("getDatabaseMetaData");
                //MarketSvc.DatabaseMetaData md = new MarketSvc.DatabaseMetaData();

                String catalog = "";// DB.getCatalog();
                String schema = DB.GetSchema();
                String tableName = table.GetTableName();

                //if (DatabaseType.IsOracle)
                //    tableName = tableName.ToUpper();


                String dropsql = null;
                int no = 0;

                String constraintNameDB = null;
                String PKTableNameDB = null;
                String PKColumnNameDB = null;
                int constraintTypeDB = 0;

                /* Get foreign key information from DatabaseMetadata
                 * */
                if (fkDataTable.Keys.Contains(tableName))
                {
                    dTable = fkDataTable[tableName];
                }
                else
                {
                    dTable = mdObject.GetForeignKeys(catalog, schema, tableName).Tables[0];
                    fkDataTable.Add(tableName, dTable);
                }
                //md.Dispose();

                //if (dTable.Rows.Count > 0)
                //{
                //    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                //    {
                //        string sql = "Select column_name from user_cons_columns where constraint_name='" + ds.Tables[0].Rows[i]["FOREIGN_KEY_CONSTRAINT_NAME"].ToString() + "'";
                //        string fkcolumnName = Util.GetValueOfString(DB.ExecuteScalar(sql));

                //        if (fkcolumnName.Equals(column.GetColumnName(), StringComparison.OrdinalIgnoreCase))
                //        {
                //            constraintNameDB = dTable.Rows[i]["FOREIGN_KEY_CONSTRAINT_NAME"].ToString();
                //            PKTableNameDB = dTable.Rows[i]["PRIMARY_KEY_TABLE_NAME"].ToString(); //rs.GetString("PKTABLE_NAME");
                //            PKColumnNameDB = dTable.Rows[i]["PRIMARY_KEY_TABLE_NAME"].ToString() + "_ID"; //rs.GetString("PKCOLUMN_NAME");
                //            constraintTypeDB = Util.GetValueOfInt(dTable.Rows[i]["DELETE_RULE"].ToString()); //rs.getShort("DELETE_RULE");
                //            break;
                //        }
                //    }
                //}


                if (dTable.Rows.Count > 0)
                {
                    for (int i = 0; i < dTable.Rows.Count; i++)
                    {
                        //string sql = "Select column_name from user_cons_columns where constraint_name='" + dTable.Rows[i]["FOREIGN_KEY_CONSTRAINT_NAME"].ToString() + "'";
                        string fkcolumn = mdObject.GetForeignColumnName(dTable.Rows[i]);


                        if (fkcolumn.Equals(column.GetColumnName(), StringComparison.OrdinalIgnoreCase))
                        {
                            Dictionary<string, string> fkcolumnName = mdObject.GetForeignColumnDetail(dTable.Rows[i]);
                            constraintNameDB = fkcolumnName["ConstraintNameDB"];
                            PKTableNameDB = fkcolumnName["PK_Table_Name"]; //rs.GetString("PKTABLE_NAME");
                            PKColumnNameDB = fkcolumnName["PK_Column_Name"]; //rs.GetString("PKCOLUMN_NAME");
                            constraintTypeDB = mdObject.GetConstraintTypeDB(fkcolumnName["Delete_Rule"]); //rs.getShort("DELETE_RULE");
                            break;
                        }
                    }
                }

                pstmt = new SqlParameter[1];
                pstmt[0] = new SqlParameter("@param", column.Get_ID());

                ds = DB.ExecuteDataset(fk, pstmt, null);

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
                                //no = DB.executeUpdate(Get_TrxName(), dropsql);

                                no = DB.ExecuteQuery(dropsql, null, null);
                                //AddLog(0, null, Decimal.Parse(no.ToString()), dropsql);
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
                                // no = DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                                no = ExecuteQuery(sql.ToString(), null, null, false, true);
                                //AddLog(0, null, Decimal.Parse(no.ToString()), sql.ToString());
                                if (no != -1)
                                {
                                    //log.Finer(ConstraintName + " - " + TableName + "." + ColumnName);
                                    returnMessage = " ADD FK " + ColumnName;// sql.ToString();
                                }
                                else
                                {
                                    // log.Info(ConstraintName + " - " + TableName + "." + ColumnName
                                    //    + " - ReturnCode=" + no);
                                    returnMessage = " FAILED FK:" + ColumnName;
                                }
                            } //if createfk

                        }
                        catch (Exception e)
                        {
                            SendMessage(" Error FK " + ColumnName + "[Error]" + e.Message);
                            //log.Log(Level.SEVERE, sql.ToString() + " - " + e.ToString());
                            returnMessage = " Error FK " + ColumnName + "[Error]" + e.Message;
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
                        no = DB.ExecuteQuery(dropsql, null, null);
                        //AddLog(0, null, Decimal.Parse(no.ToString()), dropsql);
                        returnMessage = "Drop FK for table" + table.GetTableName(); // ; dropsql.ToString();
                    }
                }
            }
            catch (Exception e)
            {
                SendMessage(" Error FK =>" + e.Message);
                returnMessage = " Error FK =>" + e.Message; //log.Log(Level.SEVERE, fk, e);
            }
            return returnMessage;
        }

        #endregion


        //Break dependencies [Market Or ModelLibrary
        public int ExecuteQuery(String sql, SqlParameter[] param, Trx trx, bool ignoreError, bool throwError = false)
        {

            try
            {
                if (trx != null)
                {
                    return trx.ExecuteNonQuery(sql, param, trx);
                }
                else
                {
                    return VAdvantage.SqlExec.ExecuteQuery.ExecuteNonQuery(sql, param);
                }
            }
            catch (System.Data.Common.DbException ex)
            {
                if (ignoreError)
                {
                    VLogger.Get().Log(Level.WARNING, "importFromServer", ex.Message);
                }
                else
                {
                    VLogger.Get().Log(Level.SEVERE, "importFromServer", ex);
                    // log.SaveError("DBExecuteError", ex);
                }
                if (throwError)
                {
                    throw new Exception(ex.Message);
                }
                return -1;
            }
            catch (Exception ex)
            {
                VLogger.Get().Severe(ex.ToString());
                //log.SaveError(ex.ToString());
                return -1;
            }
        }


        public void Clear()
        {
            if (dsOriginal != null)
            {
                dsOriginal.Clear();
                dsOriginal = null;
            }
            _sbLog = null;

            if (uniqueTableName != null)
            {
                uniqueTableName.Clear();
                uniqueTableName = null;
            }

            if (_tableReferenceColumns != null)
            {
                _tableReferenceColumns.Clear();
                _tableReferenceColumns = null;

            }

            if (_dicColumnCostarintType != null)
            {
                _dicColumnCostarintType.Clear();
                _dicColumnCostarintType = null;
            }
        }


        private void SendMessage(string msg, bool callBack = false, bool skipLog = false)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return;
            }
            if (__callback != null && callBack)
            {
                __callback.QueryExecuted(new CallBackDetail() { Status = msg });
            }

            if (string.IsNullOrEmpty(logfileName))
            {

                if (_sbLog != null && !skipLog)
                {
                    _sbLog.Append("<br />").Append(msg);
                }
            }
            else
            {
                MarketSvc.InstallModule.CommonFunctions.WriteLog(msg, logfileName);
            }
        }



        #region InstallModule

        public string logfileName = null;

        #endregion

    }




}
