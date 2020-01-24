﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Helpers;
using System.Globalization;
using VAdvantage.Process;
using VAdvantage.Logging;
using VIS.DataContracts;
using VIS.Classes;


namespace VIS.Helpers
{

    /// <summary>
    /// help to achieve [AD] Window 's core functionality
    ///  -- get window particulars
    ///  -- insert or upfate window record
    ///  -- delete or refresh window record
    /// </summary>

    public class WindowHelper : IDisposable
    {
        public WindowHelper()
        {
        }

        private List<String> _createSqlColumn = new List<String>();
        private List<String> _createSqlValue = new List<String>();
        List<VAdvantage.Process.PO_LOB> _lobInfo = null;
        string key;

        //format record info

        /** Date Time Format		*/
        private SimpleDateFormat _dateTimeFormat = DisplayType.GetDateFormat
            (DisplayType.DateTime);
        /** Date Format			*/
        private SimpleDateFormat _dateFormat = DisplayType.GetDateFormat
            (DisplayType.DateTime);
        /** Number Format		*/
        private Format _numberFormat = DisplayType.GetNumberFormat(DisplayType.Number);
        /** Amount Format		*/
        private Format _amtFormat = DisplayType.GetNumberFormat(DisplayType.Amount);
        /** Number Format		*/
        private Format _intFormat = DisplayType.GetNumberFormat(DisplayType.Integer);




        /// <summary>
        /// Insert or update window record 
        /// </summary>
        /// <param name="gTableIn">data record send from client </param>
        /// <param name="ctx">session context </param>
        /// <returns>Data contract to client</returns>
        public SaveRecordOut InsertOrUpdateRecord(SaveRecordIn gTableIn, Ctx ctx)
        {
            SaveRecordOut outData = new SaveRecordOut();

            if (String.IsNullOrEmpty(gTableIn.TableName))
            {
                outData.IsError = true;
                outData.ErrorMsg = "Table Name is Null";
                outData.Status = 'G';
                return outData;
            }

            int Record_ID = gTableIn.Record_ID;
            string tableName = gTableIn.TableName;
            int AD_Client_ID = gTableIn.AD_Client_ID;
            int AD_Org_ID = gTableIn.AD_Org_ID;

            key = ctx.GetSecureKey();

            try
            {
                if (!tableName.EndsWith("_Trl"))	//	translation tables have no model
                {
                    DataSavePO(Record_ID, gTableIn, outData, ctx);
                    return outData;
                }
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(NullReferenceException) && e.Message.Contains("No Persistent "))
                {
                    //log.Warning(_tableName + " - " + e.Message);

                }
                else
                {
                    outData.IsError = true;
                    outData.ErrorMsg = "Persistency Issue - " + tableName + ": " + e.Message;
                    outData.Status = GridTable.SAVE_ERROR;
                    return outData;
                }
            }



            bool error = false;
            //LobReset();
            //
            String message = null;
            const String ERROR = "ERROR: ";
            const String INFO = "Info: ";

            var m_fields = gTableIn.Fields;
            bool inserting = gTableIn.Inserting;
            var dataRow = gTableIn.RowData;
            var dataRowOld = gTableIn.OldRowData;


            //	Update SQL with specific where clause
            StringBuilder select = new StringBuilder("SELECT ");
            for (int i = 0; i < m_fields.Count; i++)
            {
                WindowField field = m_fields[i];
                if (inserting && field.IsVirtualColumn)
                    continue;

                if (i > 0)
                    select.Append(",");

                //if (field.GetColumnSQL(true).IndexOf("@") != -1)
                //{
                //    //****Utility.Env.ParseContext used to parse @,,@ and get value from context from column sql
                //    select.Append(Utility.Envs.ParseContext(_ctx, _windowNo, field.GetColumnSQL(true), false));
                //    continue;
                //}
                select.Append(field.ColumnSQL);	//	ColumnName or Virtual Column
            }

            select.Append(" FROM ").Append(tableName);
            StringBuilder singleRowWHERE = new StringBuilder();
            StringBuilder multiRowWHERE = new StringBuilder();
            //	Create SQL	& RowID
            if (inserting)
            {
                select.Append(" WHERE 1=2");
            }
            else	//  FOR UPDATE causes  -  ORA-01002 fetch out of sequence
            {
                select.Append(" WHERE ").Append(gTableIn.WhereClause);
            }



            CultureInfo currentCultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;

            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");// Utility.Envs.GetLanguage(Utility.Envs.GetContext()).GetCulture(Utility.Envs.GetBaseAD_Language());
            //System.Threading.Thread.CurrentThread.CurrentUICulture = Utility.Envs.GetLanguage(Utility.Envs.GetContext()).GetCulture(Utility.Envs.GetBaseAD_Language());




            IDataReader drRef = null;
            IDataReader dr = null;
            try
            {
                dr = DB.ExecuteReader(select.ToString());
                //	only one row
                if (!(inserting || ((System.Data.Common.DbDataReader)dr).HasRows))
                {
                    //  dt.Dispose();
                    dr.Close();
                    //ShowInfoMessage("SaveErrorRowNotFound", "");
                    outData.IsError = true;
                    outData.FireEEvent = true;
                    outData.EventParam = new EventParamOut() { Msg = "SaveErrorRowNotFound", Info = "", IsError = true };

                    //FireDataStatusEEvent("SaveErrorRowNotFound", "", true);
                    outData.Status = GridTable.SAVE_ERROR;

                    return outData;
                }

                bool manualUpdate = true;
                //if (DB.isRemoteObjects())
                //    manualUpdate = true;

                Dictionary<string, object> rowDataDB = null;
                //Prepare

                if (((System.Data.Common.DbDataReader)dr).HasRows)
                {
                    while (dr.Read())
                    {
                        rowDataDB = ReadData(dr, m_fields, true);
                        break;
                    }
                }
                else
                {
                    int size = m_fields.Count;
                    rowDataDB = new Dictionary<string, object>(size);
                }
                dr.Close();
                dr = null;

                /**	Data:
             *		_rowData	= original Data
             *		 drv 	    = updated Data
             *		rowDataDB	= current Data in DB
             *	1) Difference between original & updated Data?	N:next
             *	2) Difference between original & current Data?	Y:don't update
             *	3) Update current Data
             *	4) Refresh to get last Data (changed by trigger, ...)
             */
                //	Constants for Created/Updated(By)
                DateTime now = DateTime.Now;
                int user = ctx.GetAD_User_ID();

                /**
                 *	for every column
                 */
                int size1 = m_fields.Count;

                int colRs = 1;



                for (int col = 0; col < size1; col++)
                {
                    WindowField field = m_fields[col];
                    if (field.IsVirtualColumn)
                    {
                        if (!inserting)
                            colRs++;
                        continue;
                    }
                    String columnName = field.ColumnName;
                    string colNameLower = columnName.ToLower();

                    if (field.DisplayType == DisplayType.Binary)
                    {
                        object oNew = dataRow[colNameLower];
                        object oOld = dataRowOld[colNameLower];
                        if (oNew != null && !oNew.Equals(""))
                        {
                            oNew = Convert.FromBase64String(oNew.ToString());
                        }
                        if (oOld != null && !oOld.Equals(""))
                        {
                            oOld = Convert.FromBase64String(oOld.ToString());
                        }
                        dataRow[colNameLower] = oNew;
                        dataRowOld[colNameLower] = oOld;
                    }


                    if (field.DisplayType == DisplayType.RowID
                    || field.IsVirtualColumn)
                    {
                        ; //	ignore
                    }
                    //	New Key
                    else if (field.IsKey && inserting)
                    {
                        if (columnName.EndsWith("_ID") || columnName.ToUpper().EndsWith("_ID"))
                        {
                            int insertID = DB.GetNextID(ctx, tableName, null);	//	no trx
                            if (manualUpdate)
                            {
                                CreateUpdateSql(columnName, insertID.ToString());
                            }
                            else
                            {
                                //rs.updateInt (colRs, insertID); 						// ***
                            }
                            singleRowWHERE.Append(columnName).Append("=").Append(insertID);
                            //
                            message = INFO + columnName + " -> " + insertID.ToString() + " (Key)";
                        }

                        else //	Key with String value
                        {
                            String str = (String)dataRow[columnName.ToLower()];// drv.GetFieldValue(field.GetColumnName().ToUpper()).ToString();
                            if (manualUpdate)
                                CreateUpdateSql(columnName, GlobalVariable.TO_STRING(str));
                            else
                            {
                                //	rs.updateString (colRs, str); 						// ***
                            }
                            singleRowWHERE = new StringBuilder();	//	overwrite
                            singleRowWHERE.Append(columnName).Append("=").Append(GlobalVariable.TO_STRING(str));
                            //
                            message = INFO + columnName + " -> " + str + " (StringKey)";
                        }

                        //log.Fine(message);
                    }

                    // CAse For Documnet Number
                    //  case New Value(key) 
                    //	New DocumentNo
                    else if (columnName.Equals("DocumentNo"))
                    {
                        bool newDocNo = false;
                        String docNo = (String)dataRow[columnName.ToLower()];// drv[col].ToString();
                        //  we need to have a doc number
                        if (docNo == null || docNo.Length == 0)
                            newDocNo = true;
                        //  Preliminary ID from CalloutSystem
                        else if (docNo.StartsWith("<") && docNo.EndsWith(">"))
                            newDocNo = true;

                        if (newDocNo || inserting)
                        {
                            String insertDoc = null;
                            //  always overwrite if insering with mandatory DocType DocNo
                            /****************************************Check******************************************************/
                            if (inserting)
                                insertDoc = null; // DataBase.getDocumentNo (m_ctx, m_WindowNo, 
                            //m_tableName, true, null);	//	only doc type - no trx
                            //log.Fine("DocumentNo entered=" + docNo + ", DocTypeInsert=" + insertDoc + ", newDocNo=" + newDocNo);
                            // can we use entered DocNo?
                            if (insertDoc == null || insertDoc.Length == 0)
                            {
                                if (!newDocNo && docNo != null && docNo.Length > 0)
                                    insertDoc = docNo;
                                else //  get a number from DocType or Table
                                    insertDoc = MSequence.GetDocumentNo(AD_Client_ID, tableName, null, ctx);// DataBase.getDocumentNo(m_ctx, m_WindowNo, 
                                //m_tableName, false, null);	//	no trx
                                /****************************************Check********************************************************/
                            }
                            //	There might not be an automatic document no for this document
                            if (insertDoc == null || insertDoc.Length == 0)
                            {
                                //  in case DB function did not return a value
                                if (docNo != null && docNo.Length != 0)
                                    insertDoc = (String)dataRow[columnName.ToLower()];//drv[col].ToString();
                                else
                                {
                                    error = true;
                                    message = ERROR + field.ColumnName + "= " + (String)dataRow[columnName.ToLower()] + " NO DocumentNo";
                                    //log.Fine(message);
                                    //Common.ErrorLog.FillErrorLog("SaveWithutPO","",message,VAdvantage.Framework.Message.MessageType.ERROR);
                                    break;
                                }
                            }
                            //
                            if (manualUpdate)
                                CreateUpdateSql(columnName, DB.TO_STRING(insertDoc));
                            else
                            {
                                //rs.updateString (colRs, insertDoc);					//	***
                                //
                                message = INFO + columnName + " -> " + insertDoc + " (DocNo)";

                            }
                            // Common.ErrorLog.FillErrorLog("SaveWithutPO","DocumnetNo.",message,VAdvantage.Framework.Message.MessageType.ERROR);
                            //log.Fine(message);
                        }
                    }   //	New DocumentNo

                    //    New Value(key)
                    else if (columnName.Equals("Value") && inserting)
                    {
                        String value = (String)dataRow[columnName.ToLower()];// .ToString();
                        //  Get from Sequence, if not entered
                        if (value == null || value.Length == 0)
                        {
                            /***************************************Check**************************************************/
                            value = MSequence.GetDocumentNo(AD_Client_ID, tableName, null, ctx);// null;// DataBase.getDocumentNo(_ctx, _windowNo, _tableName, false, null);
                            //  No Value
                            if (value == null || value.Length == 0)
                            {
                                error = true;
                                message = ERROR + field.ColumnName + "= " + (String)dataRow[columnName.ToLower()]
                                     + " No Value";
                                //Common.ErrorLog.FillErrorLog("SaveWithutPO", "Value", message, VAdvantage.Framework.Message.MessageType.ERROR);
                                //log.Fine(message);
                                break;
                            }
                        }
                        if (manualUpdate)
                            CreateUpdateSql(columnName, DB.TO_STRING(value));
                        else
                        {
                            //rs.updateString (colRs, value); 							//	***
                        }
                        //
                        message = INFO + columnName + " -> " + value + " (Value)";
                        //log.Fine(message);
                    }	//	New Value(key)

                    else if (columnName.Equals("Updated"))
                    {
                        var d = (dataRowOld != null && dataRowOld[columnName.ToLower()] != null) ? Convert.ToDateTime(dataRowOld[columnName.ToLower()]) : DateTime.Now;
                        DateTime oldDate = d.ToUniversalTime();

                        if (gTableIn.CompareDB && !inserting && !Util.IsEqual(oldDate, rowDataDB[columnName.ToLower()]))	//	changed
                        {
                            error = true;
                            message = ERROR + field.ColumnName + "= " + oldDate
                                 + " != DB: " + rowDataDB[columnName.ToLower()];
                            //log.Fine(message);
                            //ErrorLog.FillErrorLog("GridTable", "Save direct", message, VAdvantage.Framework.Message.MessageType.ERROR);
                            break;
                        }
                        if (manualUpdate)
                        {
                            CreateUpdateSql(columnName, GlobalVariable.TO_DATE(DateTime.Now.ToUniversalTime(), false));
                        }
                        else
                        {
                            //	rs.updateTimestamp (colRs, now); 							//	***
                        }
                        //
                        message = INFO + "Updated/By -> " + now + " - " + user;
                        //log.Fine(message);
                        //ErrorLog.FillErrorLog("GridTable", "Save direct", message, VAdvantage.Framework.Message.MessageType.INFORMATION);
                    } //	Updated

                    //	UpdatedBy	- update
                    else if (columnName.Equals("UpdatedBy"))
                    {
                        if (manualUpdate)
                            CreateUpdateSql(columnName, user.ToString());
                        else
                        {
                            //	rs.updateInt (colRs, user); 								//	***
                        }
                    } //	UpdatedBy

                    //	Created
                    else if (inserting && columnName.Equals("Created"))
                    {
                        if (manualUpdate)
                            CreateUpdateSql(columnName, GlobalVariable.TO_DATE(DateTime.Now.ToUniversalTime(), false));
                        else
                        {
                            //	rs.updateTimestamp (colRs, now); 							//	***
                        }
                    } //	Created

                    //	CreatedBy
                    else if (inserting && columnName.Equals("CreatedBy"))
                    {
                        if (manualUpdate)
                            CreateUpdateSql(columnName, user.ToString());
                        else
                        {
                            //rs.updateInt (colRs, user); 								//	***
                        }
                    } //	CreatedBy





                    //	Nothing changed & null
                    else if (dataRow[columnName.ToLower()] == null && (dataRowOld == null || dataRowOld[columnName.ToLower()] == null))
                    {
                        if (inserting)
                        {
                            if (manualUpdate)
                                CreateUpdateSql(columnName, "NULL");
                            else
                            {
                                //rs.updateNull (colRs); 								//	***
                            }
                            message = INFO + columnName + "= NULL";
                        }
                    }


                    else if (inserting
                        || !Util.IsEqual(dataRow[columnName.ToLower()], dataRowOld[columnName.ToLower()])) 			//	changed
                    {


                        //	Original == DB
                        if (inserting || !gTableIn.CompareDB
                                || Util.IsEqual(dataRowOld[colNameLower], rowDataDB[colNameLower]))
                        {
                            //if (CLogMgt.isLevelFinest())
                            //log.fine(columnName + "=" + rowData[col]
                            //                                  + " " + (rowData[col]==null ? "" : rowData[col].getClass().getName()));
                            //
                            bool encrypted = field.IsEncryptedColumn;
                            Object newVal = dataRow[colNameLower];

                            //
                            String type = "String";
                            if (dataRow[colNameLower] == null)
                            {
                                if (manualUpdate)
                                    CreateUpdateSql(columnName, "NULL");
                                else
                                {
                                    //	rs.updateNull (colRs); 							//	***
                                }
                            }

                            //	ID - int
                            else if (FieldType.IsID(field.DisplayType)
                                    || field.DisplayType == DisplayType.Integer)
                            {

                                try
                                {
                                    int val = 0;
                                    if (newVal.GetType().Equals(typeof(int)))
                                        val = (int)newVal;
                                    else
                                        val = Convert.ToInt32(newVal.ToString());
                                    if (encrypted)
                                        val = (int)Encrypt(val);
                                    if (manualUpdate)
                                        CreateUpdateSql(columnName, newVal.ToString());
                                    else
                                    {
                                        //	rs.updateInt (colRs, iii.intValue()); 		// 	***
                                    }
                                }
                                catch (Exception) //  could also be a String (AD_Language, AD_Message)
                                {
                                    if (manualUpdate)
                                        CreateUpdateSql(columnName, GlobalVariable.TO_STRING((string)newVal));
                                    else
                                    {
                                        //	rs.updateString (colRs, rowData[col].toString ()); //	***
                                    }
                                }
                                type = "Int";
                            }
                            //	Numeric - BigDecimal
                            else if (FieldType.IsNumeric(field.DisplayType))
                            {
                                decimal bd = Convert.ToDecimal(newVal);
                                if (encrypted)
                                    bd = (decimal)Encrypt(bd);
                                if (manualUpdate)
                                    CreateUpdateSql(columnName, bd.ToString());
                                else
                                {
                                    //	rs.updateBigDecimal (colRs, bd); 				//	***
                                }
                                type = "Number";
                            }
                            //	Date - Timestamp
                            else if (FieldType.IsDate(field.DisplayType))
                            {
                                DateTime ts = Convert.ToDateTime(newVal);
                                DateTime tsOld = Convert.ToDateTime(dataRowOld[columnName.ToLower()]);

                                if (Util.IsEqual(ts, tsOld))
                                {
                                    continue;
                                }
                                if (encrypted)
                                    ts = (DateTime)Encrypt(ts);
                                if (manualUpdate)
                                    CreateUpdateSql(columnName, GlobalVariable.TO_DATE(ts, false));
                                else
                                {
                                    //	rs.updateTimestamp (colRs, ts); 				//	***
                                }
                                type = "Date";
                            }
                            //	LOB
                            else if (field.DisplayType == DisplayType.TextLong)
                            {
                                PO_LOB lob = new PO_LOB(tableName, columnName,
                                        null, field.DisplayType, newVal);
                                LobAdd(lob);
                                type = "CLOB";
                            }
                            //	BLOB
                            else if (field.DisplayType == DisplayType.Binary
                                    || field.DisplayType == DisplayType.Image)
                            {
                                PO_LOB lob = new PO_LOB(tableName, columnName,
                                        null, field.DisplayType, newVal);
                                LobAdd(lob);
                                type = "BLOB";
                            }
                            //	Boolean
                            else if (field.DisplayType == DisplayType.YesNo)
                            {
                                String yn = null;
                                if (newVal.GetType().Equals(typeof(bool)))
                                {
                                    Boolean bb = (Boolean)newVal;
                                    yn = bb ? "Y" : "N";
                                }
                                else
                                    yn = "Y".Equals(newVal) ? "Y" : "N";
                                if (encrypted)
                                    yn = (String)Encrypt(yn);
                                if (manualUpdate)
                                    CreateUpdateSql(columnName, GlobalVariable.TO_STRING(yn));
                                else
                                {
                                    //	rs.updateString (colRs, yn); 					//	***
                                }
                            }
                            //	String and others
                            else
                            {
                                String str = newVal.ToString();
                                if (encrypted)
                                    str = (String)Encrypt(str);
                                if (manualUpdate)
                                    CreateUpdateSql(columnName, DB.TO_STRING(str));
                                else
                                {
                                    //rs.updateString (colRs, str); 					//	***
                                }
                            }
                            //
                            message = INFO + columnName + "= ";
                            if (dataRowOld != null && dataRowOld[colNameLower] != null)
                            {
                                message += (dataRowOld[colNameLower].ToString() == "" ? null : dataRowOld[colNameLower].ToString()).ToString();
                            }
                            else
                            {
                                message += "null";
                            }

                            message += " -> " + dataRow[colNameLower] + " (" + type + ")";
                            if (encrypted)
                                message += " encrypted";
                        }
                        //	Original != DB
                        else
                        {
                            error = true;
                            message = ERROR + field.ColumnName + "= " + ((dataRowOld != null && dataRowOld[colNameLower] != null) ? dataRowOld[colNameLower] : "null").ToString()
                                    + " != DB: " + rowDataDB[colNameLower] + " -> " + dataRow[colNameLower];
                            //log.fine(is);
                            //	Object o1 = m_rowData[col];
                            //	Object o2 = rowDataDB[col];
                            //boolean eq = o1.equals(o2);
                            //	log.fine((o1 == o2) + "  " + eq);
                        }
                    }	//	DataChanged

                    if (field.IsKey && !inserting)
                    {
                        if (dataRow[colNameLower] == null)
                        {
                            throw new Exception("Key is NULL - " + columnName);
                        }
                        if (columnName.EndsWith("_ID"))
                            singleRowWHERE.Append(columnName).Append("=").Append(dataRow[colNameLower]);
                        else
                        {
                            singleRowWHERE = new StringBuilder();	//	overwrite
                            singleRowWHERE.Append(columnName).Append("=").Append(GlobalVariable.TO_STRING(dataRow[colNameLower].ToString()));
                        }
                    }
                    //	MultiKey Inserting - retrieval sql
                    if (field.IsParentColumn)
                    {
                        if (dataRow[colNameLower] == null)
                            throw new Exception("MultiKey Parent is NULL - " + columnName);
                        if (multiRowWHERE.Length != 0)
                            multiRowWHERE.Append(" AND ");
                        if (columnName.EndsWith("_ID"))
                            multiRowWHERE.Append(columnName).Append("=").Append(dataRow[colNameLower]);
                        else
                            multiRowWHERE.Append(columnName).Append("=").Append(GlobalVariable.TO_STRING(dataRow[colNameLower].ToString()));
                    }
                    //
                    colRs++;


                }

                if (error)
                {
                    if (manualUpdate)
                        CreateUpdateSqlReset();
                    //else
                    //  rs.cancelRowUpdates();
                    //rs.close();
                    // ShowInfoMessage("SaveErrorDataChanged", "");

                    outData.IsError = true;
                    outData.FireEEvent = true;
                    outData.EventParam = new EventParamOut() { Msg = "SaveErrorDataChanged", Info = "", IsError = true };
                    outData.Status = GridTable.SAVE_ERROR;
                    return outData;

                    //FireDataStatusEEvent("SaveErrorDataChanged", "", true);
                    //dataRefresh(m_rowChanged);
                    //return SAVE_ERROR;
                }

                String whereClause = singleRowWHERE.ToString();
                if (whereClause.Length == 0)
                    whereClause = multiRowWHERE.ToString();
                if (inserting)
                {
                    //log.fine("Inserting ...");
                    if (manualUpdate)
                    {
                        String sql = CreateUpdateSql(true, null, tableName);
                        int no = DB.ExecuteQuery(sql, null, null);	//	no Trx
                        if (no != 1)
                        {
                            //ErrorLog.FillErrorLog("GridTable", sql, " ---Update #=" + no + " - ", VAdvantage.Framework.Message.MessageType.ERROR);
                            //log.Log(Level.SEVERE, "Insert #=" + no + " - " + sql);
                            outData.ErrorMsg = "Insert #=" + no + " - " + sql;
                        }
                    }
                    else
                    {
                        //	rs.insertRow();
                    }
                }
                else
                {
                    //g.Fine("Updating ... " + whereClause);
                    if (manualUpdate)
                    {
                        String sql = CreateUpdateSql(false, whereClause, tableName);
                        int no = DB.ExecuteQuery(sql);	//	no Trx
                        if (no != 1)
                        {
                            //ErrorLog.FillErrorLog("GridTable", sql, "Update #=" + no + " - ", VAdvantage.Framework.Message.MessageType.ERROR);
                            //log.Log(Level.SEVERE, "Update #=" + no + " - " + sql);
                            outData.ErrorMsg = "Update #=" + no + " - " + sql;
                        }
                    }
                    else
                    {
                        //rs.updateRow();
                    }
                }
                LobSave(whereClause);
                StringBuilder refreshSQL = new StringBuilder(gTableIn.SelectSQL)
                    .Append(" WHERE ").Append(whereClause);
                drRef = DB.ExecuteReader(refreshSQL.ToString());

                if (((System.Data.Common.DbDataReader)drRef).HasRows)
                {
                    while (drRef.Read())
                    {
                        rowDataDB = ReadData(drRef, m_fields);
                        //RefreshData(drv, rowDataDB);
                        outData.RowData = rowDataDB;
                        //	update buffer
                        break;
                    }
                }
                else
                {
                    //ErrorLog.FillErrorLog("GridTable", refreshSQL.ToString(), "Error", VAdvantage.Framework.Message.MessageType.ERROR);
                    //log.Log(Level.SEVERE, "Inserted row not found");
                    outData.ErrorMsg = "Inserted row not found";
                }
                drRef.Close();
                drRef = null;

            }
            catch (System.Data.Common.DbException e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                if (drRef != null)
                {
                    drRef.Close();
                    drRef = null;
                }


                String msg = "SaveError";
                if (e.ErrorCode == 1)		//	Unique Constraint
                {
                    outData.ErrorMsg = "Key Not Unique";
                    msg = "SaveErrorNotUnique";
                }
                else
                    outData.ErrorMsg = select.ToString() + "==>" + e.Message;
                //log.Log(Level.SEVERE, select.ToString(), e);

                outData.IsError = true;
                outData.FireEEvent = true;
                outData.EventParam = new EventParamOut() { Msg = msg, Info = e.Message, IsError = true };
                outData.Status = GridTable.SAVE_ERROR;
                return outData;
            }

            catch (Exception e)
            {
                outData.IsError = true;
                outData.FireEEvent = true;
                outData.EventParam = new EventParamOut() { Msg = "SaveError", Info = e.Message, IsError = true };
                outData.Status = GridTable.SAVE_ERROR;
                return outData;
            }

            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                if (drRef != null)
                {
                    drRef.Close();
                    drRef = null;
                }
                System.Threading.Thread.CurrentThread.CurrentCulture = currentCultureInfo;

            }

            outData.FireIEvent = true;
            outData.EventParam = new EventParamOut() { Msg = "Saved", Info = "", IsError = true };
            //FireDataStatusIEvent("Saved", "");

            return outData;
        }

        /// <summary>
        /// Save data through window persistent object
        /// </summary>
        /// <param name="Record_ID">record id of window record</param>
        /// <param name="inn">in data contrxt from client</param>
        /// <param name="outt">data contract  to client</param>
        /// <param name="ctx">session context</param>
        private void DataSavePO(int Record_ID, SaveRecordIn inn, SaveRecordOut outt, Ctx ctx)
        {
            var rowData = inn.RowData; // new 
            var _rowData = inn.OldRowData;
            var m_fields = inn.Fields;
            int AD_Table_ID = inn.AD_Table_ID;
            string whereClause = inn.WhereClause;
            string SQL_Select = inn.SelectSQL;
            bool inserting = inn.Inserting;
            bool compareDB = inn.CompareDB;

            // Table ID of the table where record need to be Inserted/Updated
            int InsAD_Table_ID = AD_Table_ID;
            // Record ID from table where record need to be Inserted/Updated
            int InsRecord_ID = Record_ID;

            dynamic versionInfo = new System.Dynamic.ExpandoObject();
            Trx trx = null;
            bool hasDocValWF = false;

            // check if Maintain versions property is true / else skip
            if (inn.MaintainVersions)
            {
                // trx = Trx.Get("VISHistory"+System.DateTime.Now.Ticks);
                // Get Version table ID from AD_Table
                InsAD_Table_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_Table_ID FROM AD_Table WHERE TableName = '" + inn.TableName + "_Ver'"));
                if (InsAD_Table_ID <= 0)
                {
                    outt.IsError = true;
                    outt.FireEEvent = true;
                    outt.EventParam = new EventParamOut() { Msg = "Version Table Not Found", Info = inn.TableName + "_Ver", IsError = true };
                    outt.Status = GridTable.SAVE_ERROR;
                    return;
                }

                InsRecord_ID = Util.GetValueOfInt(inn.VerRecID);
                versionInfo.AD_Table_ID = AD_Table_ID;
                versionInfo.Record_ID = Record_ID;
                versionInfo.AD_Window_ID = inn.AD_WIndow_ID;
                versionInfo.ImmediateSave = inn.ImmediateSave;
                // check whether any Document Value type workflow is attached with Version table
                hasDocValWF = GetDocValueWF(ctx, ctx.GetAD_Client_ID(), InsAD_Table_ID, trx);
                versionInfo.HasDocValWF = hasDocValWF;
            }

            int Ver_Window_ID = 0;
            PO po = GetPO(ctx, InsAD_Table_ID, InsRecord_ID, whereClause, trx, out Ver_Window_ID);
            //	No Persistent Object
            if (po == null)
            {
                throw new NullReferenceException("No Persistent Obj");
            }

            // vinay bhatt window id
            po.SetAD_Window_ID(inn.AD_WIndow_ID);
            //      
            // check and set field values based on Master Versions 
            // else execute normally
            if (inn.MaintainVersions)
            {
                po.SetMasterDetails(versionInfo);
                po.SetAD_Window_ID(Ver_Window_ID);
                SetFields(ctx, po, m_fields, inn, outt, Record_ID, hasDocValWF, true);
            }
            else
                SetFields(ctx, po, m_fields, inn, outt, Record_ID, hasDocValWF, false);

            if (!po.Save())
            {
                String msg = "SaveError";
                String info = "";
                ValueNamePair ppE = VAdvantage.Logging.VLogger.RetrieveError();
                if (ppE == null)
                    ppE = VAdvantage.Logging.VLogger.RetrieveWarning();
                if (ppE != null)
                {
                    msg = ppE.GetValue();
                    info = ppE.GetName();
                    //	Unique Constraint
                    Exception ex = VAdvantage.Logging.VLogger.RetrieveException();
                    if (ex != null)
                        msg = "SaveErrorNotUnique";
                }

                //Added By amit 14-07-2015 Advance Documnet Control
                ValueNamePair ppE1 = VAdvantage.Logging.VLogger.RetrieveAdvDocNoError();
                if (ppE1 != null)
                {
                    //msg = ppE1.GetValue();
                    info = ppE1.GetName();
                    //	Unique Constraint
                    Exception ex = VAdvantage.Logging.VLogger.RetrieveException();
                    if (ex != null)
                        msg = "SaveErrorNotUnique";
                }
                //End

                outt.IsError = true;
                outt.FireEEvent = true;
                outt.EventParam = new EventParamOut() { Msg = msg, Info = info, IsError = true };
                outt.Status = GridTable.SAVE_ERROR;
                return;
            }
            //	Refresh - update buffer
            String whereC = po.Get_WhereClause(true);

            // if Maintain version is true then create PO object of original table
            if (inn.MaintainVersions)
            {
                var masDet = po.GetMasterDetails();
                po = GetPO(ctx, AD_Table_ID, masDet.Record_ID, whereClause, trx, out Ver_Window_ID);
                //	No Persistent Object
                if (po == null)
                {
                    throw new NullReferenceException("No Persistent Obj");
                }
                whereC = po.Get_WhereClause(true);
            }

            if (inn.ParentNodeID > 0 && po.Get_ID() != inn.ParentNodeID)
            {
                MTree tre = new MTree(ctx, inn.TreeID, null);

                string sql = "Update " + tre.GetNodeTableName() + " SET SeqNo=Seqno+1, updated=SYSDATE WHERE AD_Tree_ID=" + inn.TreeID + " AND Parent_ID=" + inn.ParentNodeID;
                DB.ExecuteQuery(sql, null, trx);

                DB.ExecuteQuery("UPDATE " + tre.GetNodeTableName() + " SET Parent_ID=" + inn.ParentNodeID + ", seqNo=0, updated=SYSDATE WHERE AD_Tree_ID=" + inn.TreeID + " AND Node_ID=" + po.Get_ID(), null, trx);
            }

            //ErrorLog.FillErrorLog("Table Object", whereClause, "information", VAdvantage.Framework.Message.MessageType.INFORMATION);

            String refreshSQL = SQL_Select + " WHERE " + whereC;

            IDataReader dr = null;
            outt.RowData = rowData;
            try
            {
                if (inn.MaintainVersions && (!inn.ImmediateSave || hasDocValWF))
                {
                    outt.RowData = inn.OldRowData;
                    // if table has Workflow then return status as has WF (W)
                    if (hasDocValWF)
                        outt.Status = GridTable.SAVE_WFAPPROVAL;
                    // if record is not Immediate Save then return Save in Future (F)
                    if (!inn.ImmediateSave)
                        outt.Status = GridTable.SAVE_FUTURE;
                }
                else
                {
                    dr = DB.ExecuteReader(refreshSQL, null, trx);
                    while (dr.Read())
                    {
                        outt.RowData = ReadData(dr, m_fields);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                outt.ErrorMsg = refreshSQL + e.Message;


                String msg = "SaveError";
                outt.IsError = true;
                outt.FireEEvent = true;
                outt.EventParam = new EventParamOut() { Msg = msg, Info = e.Message, IsError = true };
                outt.Status = GridTable.SAVE_ERROR;
                return;
            }

            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
            }

            ValueNamePair pp = VAdvantage.Logging.VLogger.RetrieveWarning();
            if (pp != null)
            {
                String msg = pp.GetValue();
                String info = pp.GetName();
                //fireDataStatusEEvent(msg, info, false);
                outt.FireEEvent = true;
                outt.EventParam = new EventParamOut() { Msg = msg, Info = info, IsError = true };

            }
            else
            {
                pp = VAdvantage.Logging.VLogger.RetrieveInfo();
                String msg = "Saved";
                String info = "";
                if (pp != null)
                {
                    msg = pp.GetValue();
                    info = pp.GetName();
                }
                outt.FireIEvent = true;
                outt.EventParam = new EventParamOut() { Msg = msg, Info = info };
            }
            if (!inn.MaintainVersions)
                outt.Status = GridTable.SAVE_OK;
        }

        /// <summary>
        /// function to check whether there is any Document Value 
        /// type workflow linked with table
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_Client_ID"></param>
        /// <param name="AD_Table_ID"></param>
        /// <param name="_trx"></param>
        /// <returns>true/false</returns>
        public bool GetDocValueWF(Ctx ctx, int AD_Client_ID, int AD_Table_ID, Trx _trx)
        {
            String sql = "SELECT COUNT(AD_Workflow_ID) FROM AD_Workflow "
                + " WHERE WorkflowType='V' AND IsActive='Y' AND IsValid='Y' AND AD_Table_ID = " + AD_Table_ID + " AND AD_Client_ID = " + AD_Client_ID
                + " ORDER BY AD_Client_ID, AD_Table_ID";

            return Util.GetValueOfInt(DB.ExecuteScalar(sql, null, _trx)) > 0;
        }

        private PO GetPO(Ctx ctx, int AD_Table_ID, int Record_ID, string whereClause, Trx trx, out int AD_Window_ID)
        {
            MTable table = MTable.Get(ctx, AD_Table_ID);
            PO po = null;
            if (table.IsSingleKey() || Record_ID == 0)
            {
                po = table.GetPO(ctx, Record_ID, trx);
            }
            else	//	Multi - Key
            {
                po = table.GetPO(ctx, whereClause, trx);
            }
            AD_Window_ID = table.GetAD_Window_ID();
            return po;
        }

        /// <summary>
        /// function to set fields  in PO based on Version Reocrd or Simple Cases
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="po"></param>
        /// <param name="m_fields"></param>
        /// <param name="inn"></param>
        /// <param name="outt"></param>
        /// <param name="Record_ID"></param>
        /// <param name="HasDocValWF"></param>
        /// <param name="VersionRecord"></param>
        private void SetFields(Ctx ctx, PO po, List<WindowField> m_fields, SaveRecordIn inn, SaveRecordOut outt, int Record_ID, bool HasDocValWF, bool VersionRecord)
        {
            var rowData = inn.RowData; // new 
            var _rowData = inn.OldRowData;
            bool inserting = inn.Inserting;
            bool compareDB = inn.CompareDB;

            int size = m_fields.Count;

            bool isEmpty = _rowData == null || _rowData.Count == 0;
            List<string> parentLinkCols = new List<string>();
            List<Object> parentLinkColValue = new List<Object>();
            for (int col = 0; col < size; col++)
            {
                WindowField field = m_fields[col];
                if (field.IsVirtualColumn)
                    continue;
                String columnName = field.ColumnName;

                //bool isClientOrgId = columnName == "AD_Client_ID" || columnName == "AD_Org_ID";

                Object value = rowData[columnName.ToLower()];// GetValueAccordingPO(rowData[col], field.GetDisplayType(), isClientOrgId);
                Object oldValue = isEmpty ? null : _rowData[columnName.ToLower()];// GetValueAccordingPO(_rowData[col], field.GetDisplayType(), isClientOrgId);

                if (field.IsEncryptedColumn)
                {
                    value = SecureEngineBridge.DecryptByClientKey((string)rowData[columnName.ToLower()], key);// GetValueAccordingPO(rowData[col], field.GetDisplayType(), isClientOrgId);
                    oldValue = isEmpty ? null : SecureEngineBridge.DecryptByClientKey((string)_rowData[columnName.ToLower()], key);// GetValueAccordingPO(_rowData[col], field.GetDisplayType(), isClientOrgId);
                }

                //	RowID
                if (DisplayType.IsDate(field.DisplayType))
                {
                    if (value != null && !value.Equals(""))
                    {
                        value = Convert.ToDateTime(value).ToUniversalTime();
                    }
                    if (oldValue != null && !oldValue.Equals(""))
                    {
                        oldValue = Convert.ToDateTime(oldValue).ToUniversalTime();
                    }
                }
                if (field.DisplayType == DisplayType.Binary)
                {
                    if (value != null && !value.Equals(""))
                    {
                        value = Convert.FromBase64String(value.ToString());
                    }
                    if (oldValue != null && !oldValue.Equals(""))
                    {
                        oldValue = Convert.FromBase64String(oldValue.ToString());
                    }
                }

                // In case of Version record
                if (VersionRecord)
                {
                    if (columnName.ToLower() == "created" || columnName.ToLower() == "updated")
                        value = System.DateTime.Now;
                    if (columnName.ToLower() == "createdby" || columnName.ToLower() == "updatedby")
                        value = ctx.GetAD_User_ID();
                    if (!po.Set_ValueNoCheck(columnName, value))
                    {
                        outt.IsError = true;
                        outt.FireEEvent = true;
                        outt.EventParam = new EventParamOut() { Msg = "ValidationError", Info = columnName, IsError = true };
                        outt.Status = GridTable.SAVE_ERROR;
                        return;
                    }
                    // check in case of version record, if there are multiple Parent Link Columns
                    if (VersionRecord && field.IsParentColumn && !parentLinkCols.Contains(columnName))
                    {
                        parentLinkCols.Add(columnName);
                        parentLinkColValue.Add(value);
                    }
                    continue;
                }

                if (field.DisplayType == DisplayType.RowID)
                {
                    ;   //ignore
                }

                //	Nothing changed & null
                else if (oldValue == null && value == null)
                {
                    ;	//	ignore
                }
                /*data changed*/
                else if (inserting || !Util.IsEqual(value, oldValue))
                {
                    //	Check existence
                    int poIndex = po.Get_ColumnIndex(columnName);
                    if (poIndex < 0)
                    {
                        //	Custom Fields not in PO
                        po.Set_CustomColumn(columnName, value);
                        continue;
                    }

                    Object dbValue = po.Get_Value(poIndex);

                    if (inserting
                        || !compareDB
                        //	Original == DB
                        || oldValue == dbValue
                        || Util.IsEqual(oldValue, dbValue)
                        //	Target == DB (changed by trigger to new value already)
                        || Util.IsEqual(value, dbValue))
                    {
                        if (!po.Set_ValueNoCheck(columnName, value))
                        {
                            outt.IsError = true;
                            outt.FireEEvent = true;
                            outt.EventParam = new EventParamOut() { Msg = "ValidationError", Info = columnName, IsError = true };
                            outt.Status = GridTable.SAVE_ERROR;
                            return;
                        }
                    }
                    //	Original != DB
                    else
                    {
                        String msg = columnName
                            + "= " + oldValue
                                + (oldValue == null ? "" : "(" + oldValue.GetType().FullName + ")")
                            + " != DB: " + dbValue
                                + (dbValue == null ? "" : "(" + dbValue.GetType().FullName + ")")
                            + " -> New: " + value
                                + (value == null ? "" : "(" + value.GetType().FullName + ")");
                        //	CLogMgt.setLevel(Level.FINEST);

                        outt.IsError = true;
                        outt.FireEEvent = true;
                        outt.EventParam = new EventParamOut() { Msg = "SaveErrorDataChanged", Info = columnName, IsError = true };
                        outt.Status = GridTable.SAVE_ERROR;
                        return;
                    }
                }
            }

            if (inn.SelectedTreeNodeID > 0)
            {
                po.SetNodeParentID(inn.SelectedTreeNodeID);
            }
            else if (inn.ParentNodeID > 0 && po.Get_ID() != inn.ParentNodeID)
            {
                po.SetNodeParentID(inn.ParentNodeID);
            }

            // Lokesh Chauhan // Master Data Versioning
            // set fields for Master Data Versions
            if (VersionRecord)
            {
                if (inn.ValidFrom != null)
                    po.Set_Value("VersionValidFrom", inn.ValidFrom.Value);

                po.Set_Value("IsVersionApproved", true);
                if (inn.ImmediateSave)
                {
                    po.Set_Value("ProcessedVersion", true);
                    po.Set_Value("VersionValidFrom", System.DateTime.Now);
                }
                // Only increase record version if Version do not exist for same date
                if (po.Get_ID() <= 0)
                {
                    int VerRec = 1;
                    int curMaxVer = 0;
                    // Get Max Record version saved in Version Record field of Version table
                    if (parentLinkCols.Count <= 0)
                        curMaxVer = Util.GetValueOfInt(DB.ExecuteScalar("SELECT  MAX(NVL(RecordVersion,0)) + 1 FROM " + inn.TableName + "_Ver WHERE " + inn.TableName + "_ID = " + Record_ID));
                    else
                    {
                        StringBuilder whClause = new StringBuilder("");
                        for (int i = 0; i < parentLinkCols.Count; i++)
                        {
                            if (i == 0)
                            {
                                if (parentLinkColValue[i] != null)
                                    whClause.Append(parentLinkCols[i] + " = " + parentLinkColValue[i]);
                                else
                                    whClause.Append(" NVL(" + parentLinkCols[i] + ",0) = 0");
                            }
                            else
                            {
                                if (parentLinkColValue[i] != null)
                                    whClause.Append(" AND " + parentLinkCols[i] + " = " + parentLinkColValue[i]);
                                else
                                    whClause.Append(" AND NVL(" + parentLinkCols[i] + ",0) = 0");
                            }
                        }
                        curMaxVer = Util.GetValueOfInt(DB.ExecuteScalar("SELECT  MAX(NVL(RecordVersion,0)) + 1 FROM " + inn.TableName + "_Ver WHERE " + whClause));
                    }
                    if (curMaxVer > 0)
                        VerRec = curMaxVer;
                    po.Set_Value("RecordVersion", VerRec);
                }
                if (!inn.ImmediateSave)
                {
                    if (inn.ValidFrom == null)
                        inn.ImmediateSave = true;
                }
                // if Document value type Workflow is attached with the table,
                // then set "ProcessedVersion" & "IsVersionApproved" as false for Version
                if (HasDocValWF)
                {
                    po.Set_Value("ProcessedVersion", false);
                    po.Set_Value("IsVersionApproved", false);
                }
            }
            // Lokesh Chauhan // Master Data Versioning
        }

        /// <summary>
        /// Read data of row 
        /// -- encrypt or decrypt on basic of column info
        /// </summary>
        /// <param name="dr">data reader row</param>
        /// <param name="m_fields">columns info list</param>
        /// <returns>modified row</returns>
        private Dictionary<string, Object> ReadData(IDataReader dr, List<WindowField> m_fields, bool converToBoolValue = false)
        {
            int size = m_fields.Count;
            Dictionary<string, Object> rowData = new Dictionary<string, Object>(size);
            String columnName = null;
            int displayType = 0;

            //	Types see also MField.createDefault
            try
            {
                //	get row data
                for (int j = 0; j < size; j++)
                {
                    WindowField field = m_fields[j];
                    columnName = field.ColumnName;
                    string colLower = columnName.ToLower();
                    //	NULL
                    if (dr.IsDBNull(j))
                    {
                        rowData[colLower] = null;
                    }
                    else
                    {
                        //	Column Info

                        displayType = field.DisplayType;
                        //	Integer, ID, Lookup (UpdatedBy is a numeric column)
                        if ((DisplayType.IsID(displayType) // JJ: don't touch!
                                && (columnName.EndsWith("_ID") || columnName.EndsWith("_Acct")))
                            || columnName.EndsWith("atedBy") || columnName.EndsWith("_ID_1") || columnName.EndsWith("_ID_2") || columnName.EndsWith("_ID_3")
                            || displayType == DisplayType.Integer)
                        {
                            rowData[colLower] = Util.GetValueOfInt(dr[j].ToString());	//	Integer
                        }
                        //	Number
                        else if (DisplayType.IsNumeric(displayType))
                        {
                            rowData[colLower] = Util.GetValueOfDecimal(dr[j].ToString());			//	BigDecimal
                        }
                        //	Date
                        else if (DisplayType.IsDate(displayType))
                        {
                            DateTime time = new DateTime();

                            // time.Kind = DateTimeKind.Utc;
                            DateTime.TryParse(dr[j].ToString(), out time);

                            time = DateTime.SpecifyKind(time, DateTimeKind.Utc);

                            //Remove Time Offset In case of DataOnly Field
                            //if (displayType == DisplayType.Date)
                            {
                                //time = time.ToUniversalTime();
                            }
                            rowData[colLower] = time;
                        }
                        //	RowID or Key (and Selection)
                        else if (displayType == DisplayType.RowID)
                            rowData[colLower] = null;
                        //	YesNo
                        else if (displayType == DisplayType.YesNo)
                        {
                            String str = dr[j].ToString();
                            if (field.IsEncryptedColumn)
                                str = (String)Decrypt(str);
                            if (converToBoolValue)
                            {
                                rowData[colLower] = "Y".Equals(str);
                            }
                            else
                            {
                                rowData[colLower] = str;	//	Boolean
                            }
                        }
                        //	LOB
                        else if (DisplayType.IsLOB(displayType))
                        {
                            Object value = dr[j];
                            if (dr[j] == null)
                                rowData[colLower] = null;
                            //else if (value instanceof Clob) 
                            //{
                            //    Clob lob = (Clob)value;
                            //    long length = lob.length();
                            //    rowData[j] = lob.getSubString(1, (int)length);
                            //}
                            //else if (value instanceof Blob)
                            //{
                            //    Blob lob = (Blob)value;
                            //    long length = lob.length();
                            //    rowData[j] = lob.getBytes(1, (int)length);
                            //}
                            //// For EnterpriseDB (Vienna Type Long Text is stored as Text in EDB)
                            //else if (value instanceof java.lang.String) {
                            //    rowData[j] = value.toString();
                            //}
                            if (value.GetType().Equals(typeof(string)))
                            {
                                rowData[colLower] = value.ToString();
                            }
                            else
                            {
                                rowData[colLower] = value;
                            }
                        }
                        //	String
                        else
                            rowData[colLower] = dr[j].ToString();//string
                        //	Encrypted
                        if (field.IsEncryptedColumn && displayType != DisplayType.YesNo)
                        {

                            rowData[colLower] = Decrypt(rowData[colLower]);
                        }
                    }
                }
            }
            catch (Exception)
            {
                //log.Log(Level.SEVERE, columnName + ", DT=" + displayType, e);
                //ErrorLog.FillErrorLog("GridTable", "columnName  DT=" + displayType, e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
            }
            return rowData;

        }

        private Object Encrypt(Object xx)
        {
            if (xx == null)
                return null;
            return SecureEngine.Encrypt(xx);
        }	//	encrypt

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="yy"></param>
        /// <returns></returns>
        private Object Decrypt(Object yy)
        {
            if (yy == null)
                return null;
            if (SecureEngine.IsEncrypted((String)yy))
                return SecureEngineBridge.EncryptFromSeverToClient((string)yy, key);
            return yy;
        }	//	d

        /// <summary>
        ///	Prepare SQL creation
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        private void CreateUpdateSql(String columnName, String value)
        {
            _createSqlColumn.Add(columnName);
            _createSqlValue.Add(value);
        }

        /// <summary>
        ///Create update/insert SQL
        /// </summary>
        /// <param name="insert">true if insert - update otherwise</param>
        /// <param name="whereClause">where clause for update</param>
        /// <returns></returns>
        private String CreateUpdateSql(bool insert, String whereClause, string tableName)
        {
            StringBuilder sb = new StringBuilder();
            if (insert)
            {
                sb.Append("INSERT INTO ").Append(tableName).Append(" (");
                for (int i = 0; i < _createSqlColumn.Count; i++)
                {
                    if (i != 0)
                        sb.Append(",");
                    sb.Append(_createSqlColumn[i]);
                }
                sb.Append(") VALUES ( ");
                for (int i = 0; i < _createSqlValue.Count; i++)
                {
                    if (i != 0)
                        sb.Append(",");
                    sb.Append(_createSqlValue[i]);
                }
                sb.Append(")");
            }
            else
            {
                sb.Append("UPDATE ").Append(tableName).Append(" SET ");
                for (int i = 0; i < _createSqlColumn.Count; i++)
                {
                    if (i != 0)
                        sb.Append(",");
                    sb.Append(_createSqlColumn[i]).Append("=").Append(_createSqlValue[i]);
                }
                sb.Append(" WHERE ").Append(whereClause);
            }
            CreateUpdateSqlReset();
            return sb.ToString();
        }

        /// <summary>
        ///Reset Update -Insert generic lists
        /// </summary>
        private void CreateUpdateSqlReset()
        {
            _createSqlColumn = new List<String>();
            _createSqlValue = new List<String>();
        }	//	createUpdateSqlReset

        private void LobAdd(VAdvantage.Process.PO_LOB lob)
        {
            //		log.fine("LOB=" + lob);
            if (_lobInfo == null)
                _lobInfo = new List<VAdvantage.Process.PO_LOB>();
            _lobInfo.Add(lob);
        }	//

        private void LobSave(String whereClause)
        {
            if (_lobInfo == null)
                return;
            for (int i = 0; i < _lobInfo.Count; i++)
            {
                VAdvantage.Process.PO_LOB lob = (VAdvantage.Process.PO_LOB)_lobInfo[i];
                lob.Save(whereClause, null);		//	no trx
            }	//	for all LOBs
            LobReset();
        }	//

        private void LobReset()
        {
            _lobInfo = null;
        }

        /// <summary>
        /// delete record of window
        /// </summary>
        /// <param name="dInn">row info</param>
        /// <param name="ctx"></param>
        /// <returns>dleted row info to client</returns>
        internal DeleteRecordOut DeleteRecord(DeleteRecordIn dInn, Ctx ctx)
        {
            DeleteRecordOut outt = new DeleteRecordOut();

            MTable table = MTable.Get(ctx, dInn.AD_Table_ID);
            PO po = null;

            List<int> singleKeyWhere = dInn.SingleKeyWhere;
            List<String> multiKeyWhere = dInn.MultiKeyWhere;
            List<int> deletedRecordIds = new List<int>();
            string tableName = dInn.TableName;
            bool haskeyColumn = dInn.HasKeyColumn;

            List<int> unDeletedRecIds = new List<int>(dInn.RecIds);

            Trx p_trx = Trx.Get("GridDel" + DateTime.Now.Ticks);

            try
            {
                for (int i = 0; i < dInn.RecIds.Count; i++)
                {

                    if (haskeyColumn)
                        po = table.GetPO(ctx, singleKeyWhere[i], p_trx);
                    else	//	Multi - Key
                        po = table.GetPO(ctx, multiKeyWhere[i], p_trx);

                    //	Delete via PO
                    if (po != null)
                    {
                        bool ok = false;
                        try
                        {
                            ok = po.Delete(false);
                        }
                        catch (Exception t)
                        {
                            outt.ErrorMsg = "Delete" + t.Message;
                            //log.log(Level.SEVERE, "Delete", t);
                        }
                        if (!ok)
                        {
                            p_trx.Rollback();

                            outt.IsError = true;
                            outt.FireEEvent = true;
                            ValueNamePair vp = VLogger.RetrieveError();
                            if (vp != null)
                            {
                                outt.EventParam = new EventParamOut() { Msg = vp.GetValue(), Info = vp.GetName(), IsError = true };
                            }
                            else
                            {
                                outt.EventParam = new EventParamOut() { Msg = "DeleteError", Info = "", IsError = true };
                            }
                            break;
                        }
                        else
                        {
                            p_trx.Commit();
                            deletedRecordIds.Add(dInn.RecIds[i]);
                            unDeletedRecIds.Remove(dInn.RecIds[i]);
                            if (haskeyColumn)
                            {
                                if (outt.RecIds == null)
                                    outt.RecIds = new List<int>();
                                outt.RecIds.Add(singleKeyWhere[i]);
                            }
                        }
                    }
                    else	//	Delete via SQL
                    {
                        StringBuilder sql = new StringBuilder("DELETE FROM ");
                        sql.Append(tableName).Append(" WHERE ").Append(multiKeyWhere[i]);
                        int no = 0;
                        try
                        {
                            //pstmt = DB.prepareStatement(sql.toString(), (Trx)null);
                            no = DB.ExecuteQuery(sql.ToString());
                            deletedRecordIds.Add(dInn.RecIds[i]);
                            unDeletedRecIds.Remove(dInn.RecIds[i]);
                        }

                        catch (System.Data.Common.DbException e)
                        {

                            outt.ErrorMsg = "Delete" + sql.ToString() + "-" + e.Message;
                            String msg = "DeleteError";
                            if (e.ErrorCode == -2146232008)	//	Child Record Found
                                msg = "DeleteErrorDependent";

                            outt.IsError = true;
                            outt.FireEEvent = true;
                            outt.EventParam = new EventParamOut() { Msg = msg, Info = e.Message, IsError = true };
                            break;

                        }


                        catch (Exception e)
                        {
                            outt.ErrorMsg = "Delete" + sql.ToString() + "-" + e.Message;
                            String msg = "DeleteError";
                            //     if (e.ErrorCode == -2146232008)	//	Child Record Found
                            //msg = "DeleteErrorDependent";
                            outt.IsError = true;
                            outt.FireEEvent = true;
                            outt.EventParam = new EventParamOut() { Msg = msg, Info = e.Message, IsError = true };
                            break;

                        }

                        //	Check Result
                        if (no != 1)
                        {
                            outt.IsError = true;
                            outt.ErrorMsg = "Number of deleted rows = " + no;
                            break;
                        }

                    }
                }
            }
            catch (Exception e)
            {
                outt.IsError = true;
                outt.FireEEvent = true;
                outt.EventParam = new EventParamOut() { Msg = "DeleteError", Info = e.Message, IsError = true };
            }
            finally
            {
                p_trx.Close();
                p_trx = null;
            }

            outt.DeletedRecIds = deletedRecordIds;
            outt.UnDeletedRecIds = unDeletedRecIds;
            return outt;
        }

        /// <summary>
        /// Get Recoprds of window
        /// </summary>
        /// <param name="sqlIn">sqlparameter</param>
        /// <param name="encryptedColnames">lsit encrypted column of window</param>
        /// <param name="ctx">context</param>
        /// <returns>Json cutom equalvalent to dataset</returns>
        internal object GetWindowRecords(SqlParamsIn sqlIn, List<string> encryptedColNames, Ctx ctx, int rowCount, string sqlCount, int AD_Table_ID)
        {
            List<JTable> outO = new List<JTable>();
            JTable obj = null;


            MSession session = MSession.Get(ctx, true);
            session.QueryLog(ctx.GetAD_Client_ID(), ctx.GetAD_Org_ID(), AD_Table_ID,
                sqlCount, rowCount);



            DataSet ds = new SqlHelper().ExecuteDataSet(sqlIn);
            if (ds == null)
                return null;

            bool checkEncrypted = encryptedColNames != null && encryptedColNames.Count > 0;
            key = ctx.GetSecureKey();


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

                    JRow r = new JRow();
                    r.id = row;
                    for (int column = 0; column < dt.Columns.Count; column++)
                    {
                        //var c = new Dictionary<string,object>();
                        //c[dt.Columns[column].ColumnName.ToLower()] = dt.Rows[row][column];

                        if (checkEncrypted && encryptedColNames.Contains(dt.Columns[column].ColumnName.ToLower()))
                        {
                            r.cells[dt.Columns[column].ColumnName.ToLower()] = SecureEngineBridge.EncryptFromSeverToClient(dt.Rows[row][column].ToString(), key);
                            continue;
                        }
                        r.cells[dt.Columns[column].ColumnName.ToLower()] = dt.Rows[row][column];
                        //rows.Append(dt.Columns[column].ColumnName).Append(":").Append(dt.Rows[row][column]);
                    }
                    obj.rows.Add(r);
                }
                outO.Add(obj);

            }
            return outO;
        }

        /// <summary>
        /// Get a record of window
        /// </summary>
        /// <param name="sql">sql string</param>
        /// <param name="encryptedColNames">list of encrypted columna names</param>
        /// <param name="ctx"></param>
        /// <returns>JTable object</returns>
        internal object GetWindowRecord(string sql, List<string> encryptedColNames, Ctx ctx)
        {

            var h = new SqlHelper();
            DataSet ds = h.ExecuteDataSet(sql);
            h = null;

            JTable obj = null;

            if (ds == null)
                return null;

            key = ctx.GetSecureKey();

            bool checkEncrypted = encryptedColNames != null && encryptedColNames.Count > 0;

            obj = new JTable();

            var dt = ds.Tables[0];

            obj.name = dt.TableName;
            obj.records = ds.Tables[0].Rows.Count;
            obj.page = 1;
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

                JRow r = new JRow();
                r.id = row;
                for (int column = 0; column < dt.Columns.Count; column++)
                {
                    //var c = new Dictionary<string,object>();
                    //c[dt.Columns[column].ColumnName.ToLower()] = dt.Rows[row][column];

                    if (checkEncrypted && encryptedColNames.Contains(dt.Columns[column].ColumnName.ToLower()))
                    {
                        r.cells[dt.Columns[column].ColumnName.ToLower()] = SecureEngineBridge.EncryptFromSeverToClient(dt.Rows[row][column].ToString(), key);
                        continue;
                    }
                    r.cells[dt.Columns[column].ColumnName.ToLower()] = dt.Rows[row][column];
                    //rows.Append(dt.Columns[column].ColumnName).Append(":").Append(dt.Rows[row][column]);
                }
                obj.rows.Add(r);
            }
            return obj;
        }




        /// <summary>
        /// Get Records of window
        /// </summary>
        /// <param name="sqlIn">sqlparameter</param>
        /// <param name="encryptedColnames">lsit encrypted column of window</param>
        /// <param name="ctx">context</param>
        /// <returns>Json cutom equalvalent to dataset</returns>
        internal object GetWindowRecordsForTreeNode(SqlParamsIn sqlIn, List<string> encryptedColNames, Ctx ctx, int rowCount, string sqlCount, int AD_Table_ID, int treeID, int treeNodeID)
        {
            List<JTable> outO = new List<JTable>();
            JTable obj = null;


            MSession session = MSession.Get(ctx, true);
            session.QueryLog(ctx.GetAD_Client_ID(), ctx.GetAD_Org_ID(), AD_Table_ID,
                sqlCount, rowCount);


            string tableName = MTable.GetTableName(ctx, AD_Table_ID);

            MTree tree = new MTree(ctx, treeID, null);

            if (treeNodeID > 0)
            {

                GetChildNodesID(treeNodeID, tree.GetNodeTableName(), treeID, tableName);

                string sqlWhere = "SELECT Node_ID FROM " + tree.GetNodeTableName() + " WHERE (Parent_ID IN (" + parentIDs + ")  OR Node_ID IN (" + parentIDs + ")) AND AD_Tree_ID=" + treeID;

                string sqlQuery = sqlIn.sql;

                if (sqlQuery.ToUpper().LastIndexOf("WHERE") > -1)
                {
                    if (sqlQuery.ToUpper().LastIndexOf("ORDER BY") > -1)
                    {
                        sqlIn.sql = sqlQuery.Insert(sqlQuery.ToUpper().LastIndexOf("ORDER BY"), " AND " + tableName + "_ID IN (" + sqlWhere + ") ");
                    }
                    else
                    {
                        sqlIn.sql = sqlQuery + " AND " + tableName + "_ID IN (" + sqlWhere + ") ";
                    }
                }
            }
            else
            {
                string sql = "SELECT node_ID FROM " + tree.GetNodeTableName() + " WHERE AD_Tree_ID=" + treeID + " AND (Parent_ID = " + treeNodeID + " AND NODE_ID IN (SELECT " + tableName + "_ID FROM " + tableName + " WHERE ISActive='Y' AND IsSummary='N'))";

                string sqlQuery = sqlIn.sql;

                if (sqlIn.sql.ToUpper().LastIndexOf("WHERE") > -1)
                {

                    if (sqlQuery.ToUpper().LastIndexOf("ORDER BY") > -1)
                    {
                        sqlIn.sql = sqlQuery.Insert(sqlQuery.ToUpper().LastIndexOf("ORDER BY"), " AND " + tableName + "_ID IN (" + sql + ") ");
                    }
                    else
                    {
                        sqlIn.sql = sqlQuery + " AND " + tableName + "_ID IN (" + sql + ") ";
                    }

                    //sqlIn.sql = sqlIn.sql.Insert(sqlIn.sql.ToUpper().LastIndexOf("WHERE") + 5, " " + tableName + "_ID IN (" + sql + ") AND ");
                }
            }

            DataSet ds = new SqlHelper().ExecuteDataSet(sqlIn);

            if (ds == null)
                return null;

            bool checkEncrypted = encryptedColNames != null && encryptedColNames.Count > 0;
            key = ctx.GetSecureKey();


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

                    JRow r = new JRow();
                    r.id = row;
                    for (int column = 0; column < dt.Columns.Count; column++)
                    {
                        //var c = new Dictionary<string,object>();
                        //c[dt.Columns[column].ColumnName.ToLower()] = dt.Rows[row][column];

                        if (checkEncrypted && encryptedColNames.Contains(dt.Columns[column].ColumnName.ToLower()))
                        {
                            r.cells[dt.Columns[column].ColumnName.ToLower()] = SecureEngineBridge.EncryptFromSeverToClient(dt.Rows[row][column].ToString(), key);
                            continue;
                        }
                        r.cells[dt.Columns[column].ColumnName.ToLower()] = dt.Rows[row][column];
                        //rows.Append(dt.Columns[column].ColumnName).Append(":").Append(dt.Rows[row][column]);
                    }
                    obj.rows.Add(r);
                }
                outO.Add(obj);

            }
            return outO;
        }

        StringBuilder parentIDs = new StringBuilder();
        private void GetChildNodesID(int currentnode, string tableName, int treeID, string adtableName)
        {
            if (parentIDs.Length == 0)
            {
                parentIDs.Append(currentnode);
            }
            else
            {
                parentIDs.Append(",").Append(currentnode);
            }


            //  string sql = "SELECT node_ID FROM " + tableName + " WHERE AD_Tree_ID=" + treeID + " AND Parent_ID = " + currentnode + " AND NODE_ID IN (SELECT " + adtableName + "_ID FROM " + adtableName + " WHERE ISActive='Y' AND IsSummary='Y')";


            string sql = "SELECT pr.node_ID FROM " + tableName + "   pr JOIN " + adtableName + " mp on pr.Node_ID=mp." + adtableName + "_id  WHERE pr.AD_Tree_ID=" + treeID + " AND pr.Parent_ID = " + currentnode + " AND mp.ISActive='Y' AND mp.IsSummary='Y'";

            DataSet ds = DB.ExecuteDataset(sql);
            if (ds == null || ds.Tables[0].Rows.Count > 0)
            {
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    GetChildNodesID(Convert.ToInt32(ds.Tables[0].Rows[j]["node_ID"]), tableName, treeID, adtableName);
                }
            }
        }


        int strtreeNodesCount = 1;
        private void GetChildNodesCount(int currentnode, string tableName, int treeID, string adtableName, string whereClause, bool ShowSummaryNodes)
        {
            //string sql = "SELECT count(*) FROM " + tableName + " WHERE Parent_ID=" + currentnode + " AND AD_Tree_ID=" + treeID;
            //var count = DB.ExecuteScalar(sql);
            //if (count != null && count != DBNull.Value)
            //{
            //    strtreeNodesCount += Util.GetValueOfInt(count);
            //}

            //sql = "SELECT node_ID FROM " + tableName + "  pr JOIN " + adtableName + " mp on pr.Node_ID=mp." + adtableName + "_id WHERE pr.AD_Tree_ID=" + treeID + " AND pr.Parent_ID = " + currentnode + " AND  mp.ISActive='Y' AND mp.IsSummary='Y'";

            //DataSet ds = DB.ExecuteDataset(sql);
            //if (ds == null || ds.Tables[0].Rows.Count > 0)
            //{
            //    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
            //    {
            //        GetChildNodesCount(Convert.ToInt32(ds.Tables[0].Rows[j]["node_ID"]), tableName, treeID, adtableName);
            //    }
            //}


            //string sql = "SELECT Node_ID, IsSummary FROM " + tableName + " WHERE Parent_ID=" + currentnode + " AND AD_Tree_ID=" + treeID;

            string sqls = "SELECT node_ID, " + adtableName + ".IsSummary FROM " + tableName + "  pr JOIN " + adtableName + " " + adtableName + " on pr.Node_ID=" + adtableName + "." + adtableName + "_id WHERE pr.AD_Tree_ID=" + treeID + " AND pr.Parent_ID = " + currentnode;
            if (whereClause.Length > 0)
            {
                sqls = sqls + " AND " + whereClause;
            }

            DataSet countDs = DB.ExecuteDataset(sqls);
            if (countDs != null && countDs.Tables[0].Rows.Count > 0)
            {
                strtreeNodesCount += countDs.Tables[0].Rows.Count;
            }

            //sql = "SELECT node_ID FROM " + tableName + "  pr JOIN " + adtableName + " mp on pr.Node_ID=mp." + adtableName + "_id WHERE pr.AD_Tree_ID=" + treeID + " AND pr.Parent_ID = " + currentnode + " AND  mp.ISActive='Y' AND mp.IsSummary='Y'";

            //DataSet ds = DB.ExecuteDataset(sql);

            sqls = "SELECT node_ID, mp.IsSummary FROM " + tableName + "  pr JOIN " + adtableName + " mp on pr.Node_ID=mp." + adtableName + "_id WHERE pr.AD_Tree_ID=" + treeID + " AND pr.Parent_ID = " + currentnode + " ";
            countDs = DB.ExecuteDataset(sqls);

            DataRow[] rows = countDs.Tables[0].Select("IsSummary='Y'");

            if (rows != null && rows.Count() > 0)
            {
                for (int j = 0; j < rows.Count(); j++)
                {
                    GetChildNodesCount(Convert.ToInt32(rows[j]["node_ID"]), tableName, treeID, adtableName, whereClause, ShowSummaryNodes);
                }
            }

        }


        /// <summary>
        /// Get Records of window
        /// </summary>
        /// <param name="sqlIn">sqlparameter</param>
        /// <param name="encryptedColnames">lsit encrypted column of window</param>
        /// <param name="ctx">context</param>
        /// <returns>Json cutom equalvalent to dataset</returns>
        internal int GetRecordCountForTreeNode(string whereClause, Ctx ctx, int AD_Table_ID, int treeID, int treeNodeID, int windowNo, bool ShowSummaryNodes)
        {
            MSession session = MSession.Get(ctx, true);
            session.QueryLog(ctx.GetAD_Client_ID(), ctx.GetAD_Org_ID(), AD_Table_ID,
                "", 0);

            MTree tree = new MTree(ctx, treeID, null);

            if (whereClause.Length > 0)
            {
                whereClause = Env.ParseContext(ctx, windowNo, whereClause, false);
            }
            if (!ShowSummaryNodes)
            {
                strtreeNodesCount = 0;
            }

            if (tree != null && tree.GetAD_Tree_ID() > 0)
            {
                if (treeNodeID > 0)
                {
                    GetChildNodesCount(treeNodeID, tree.GetNodeTableName(), treeID, MTable.GetTableName(ctx, AD_Table_ID), whereClause, ShowSummaryNodes);
                }
                else
                {
                    string sql = "SELECT Count(*) FROM " + tree.GetNodeTableName() + " WHERE AD_Tree_ID=" + treeID + " AND Parent_ID = " + 0 + " AND NODE_ID IN (SELECT " + MTable.GetTableName(ctx, AD_Table_ID) + "_ID FROM " + MTable.GetTableName(ctx, AD_Table_ID) + " WHERE  IsSummary='N' ";

                    if (whereClause.Length > 0)
                    {
                        if (sql.ToLower().LastIndexOf("where") > -1)
                        {
                            sql += " AND " + whereClause;
                        }
                        else
                        {
                            sql += " WHERE " + whereClause;
                        }
                    }
                    sql += ")";
                    strtreeNodesCount = Util.GetValueOfInt(DB.ExecuteScalar(sql));
                }
            }

            return strtreeNodesCount;
        }




        internal RecordInfoOut GetRecordInfo(RecordInfoIn dse, Ctx ctx)
        {
            RecordInfoOut outt = new RecordInfoOut();

            var _info = new StringBuilder("");
            if (dse.CreatedBy == -1)

                return outt;
            //  Info
            MUser user = MUser.Get(ctx, dse.CreatedBy);
            _info.Append(" ")
                .Append(Msg.Translate(ctx, "CreatedBy"))
                .Append(": ").Append(user.GetName())
                .Append(" - ").Append(String.Format("{0:D}", dse.Created)).Append("<br/>");

            if (!dse.Created.Equals(dse.Updated)
                || !dse.CreatedBy.Equals(dse.UpdatedBy))
            {
                if (!dse.CreatedBy.Equals(dse.UpdatedBy))
                    user = MUser.Get(ctx, dse.UpdatedBy);
                _info.Append(" ")
                    .Append(Msg.Translate(ctx, "UpdatedBy"))
                    .Append(": ").Append(user.GetName())
                    .Append(" - ").Append(String.Format("{0:D}", dse.Updated)).Append("<br/>");
            }
            if (dse.Info != null && dse.Info.Length > 0)
                _info.Append("<br/> (").Append(dse.Info).Append(")");

            outt.Info = _info.ToString();
            //	Only Client Preference can view Change Log
            if (!MRole.PREFERENCETYPE_Client.Equals(MRole.GetDefault(ctx).GetPreferenceType()))
                return outt;

            int Record_ID = 0;
            if (dse.Record_ID is int)
                Record_ID = (int)dse.Record_ID;
            //else
            //    log.Info("dynInit - Invalid Record_ID=" + dse.Record_ID);
            if (Record_ID == 0)
                return outt;

            //Data
            String sql = "SELECT AD_Column_ID, Updated, UpdatedBy, OldValue, NewValue "
                + " FROM AD_ChangeLog "
                + " WHERE AD_Table_ID=" + dse.AD_Table_ID + " AND Record_ID=" + Record_ID
                + " ORDER BY Updated DESC";
            try
            {
                DataSet ds = DB.ExecuteDataset(sql, null, null);



                outt.Rows = new List<RecordRow>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    var r = AddLine(Util.GetValueOfInt(dr[0]), Util.GetValueOfDateTime(dr[1]), Util.GetValueOfInt(dr[2]),
                             dr[3].ToString(), dr[4].ToString(), ctx);

                    outt.Rows.Add(r);
                    outt.ShowGrid = true;
                }
            }
            catch (Exception)
            {
                //log.Log(Level.SEVERE, sql, e);
            }

            List<String> columnNames = new List<String>();
            outt.Headers = new List<string>();
            outt.Headers.Add(Msg.Translate(ctx, "AD_Column_ID"));
            outt.Headers.Add(Msg.Translate(ctx, "NewValue"));
            outt.Headers.Add(Msg.Translate(ctx, "OldValue"));
            outt.Headers.Add(Msg.Translate(ctx, "UpdatedBy"));
            outt.Headers.Add(Msg.Translate(ctx, "Updated") + " (" + Msg.GetMsg(ctx, "UTC") + ")");

            return outt;
        }

        private RecordRow AddLine(int AD_Column_ID, DateTime? Updated, int UpdatedBy,
         String OldValue, String NewValue, Ctx ctx)
        {
            var line = new RecordRow();
            //	Column
            MColumn column = MColumn.Get(ctx, AD_Column_ID);
            line.AD_Column_ID = column.GetName();
            //
            if (OldValue != null && OldValue.Equals(MChangeLog.NULL))
                OldValue = null;
            String showOldValue = OldValue;
            if (NewValue != null && NewValue.Equals(MChangeLog.NULL))
                NewValue = null;
            String showNewValue = NewValue;
            //
            try
            {
                if (DisplayType.IsText(column.GetAD_Reference_ID()))
                {
                    ;
                }
                else if (column.GetAD_Reference_ID() == DisplayType.YesNo)
                {
                    if (OldValue != null)
                    {
                        bool yes = OldValue.Equals("True") || OldValue.Equals("Y");
                        showOldValue = Msg.GetMsg(ctx, yes ? "Y" : "N");
                    }
                    if (NewValue != null)
                    {
                        bool yes = NewValue.Equals("True") || NewValue.Equals("Y");
                        showNewValue = Msg.GetMsg(ctx, yes ? "Y" : "N");
                    }
                }
                else if (column.GetAD_Reference_ID() == DisplayType.Amount)
                {
                    if (OldValue != null)
                        showOldValue = String.Format(_amtFormat.GetFormat(), OldValue);//.fo
                    //.format (new BigDecimal (OldValue));
                    if (NewValue != null)
                        showNewValue = String.Format(_amtFormat.GetFormat(), NewValue);//.ToString(_amtFormat.GetFormat());//  m_amtFormat
                    //.format (new BigDecimal (NewValue));
                }
                else if (column.GetAD_Reference_ID() == DisplayType.Integer)
                {
                    if (OldValue != null)
                        showOldValue = String.Format(_intFormat.GetFormat(), OldValue);//.ToString(_intFormat.GetFormat());// m_intFormat.format (new Integer (OldValue));
                    if (NewValue != null)
                        showNewValue = String.Format(_intFormat.GetFormat(), NewValue);//.ToString(_intFormat.GetFormat());//m_intFormat.format (new Integer (NewValue));
                }
                else if (DisplayType.IsNumeric(column.GetAD_Reference_ID()))
                {
                    if (OldValue != null)
                        showOldValue = String.Format(_numberFormat.GetFormat(), OldValue);//.ToString(_numberFormat.GetFormat());//m_numberFormat.format (new BigDecimal (OldValue));
                    if (NewValue != null)
                        showNewValue = String.Format(_numberFormat.GetFormat(), NewValue);//.ToString(_numberFormat.GetFormat()); //m_numberFormat.format (new BigDecimal (NewValue));
                }
                else if (column.GetAD_Reference_ID() == DisplayType.Date)
                {
                    if (OldValue != null)
                        showOldValue = _dateFormat.Format(OldValue);// m_dateFormat.format (Timestamp.valueOf (OldValue));
                    if (NewValue != null)
                        showNewValue = _dateFormat.Format(NewValue);// m_dateFormat.format (Timestamp.valueOf (NewValue));
                }
                else if (column.GetAD_Reference_ID() == DisplayType.DateTime)
                {
                    if (OldValue != null)
                        showOldValue = _dateTimeFormat.Format(OldValue);// (Timestamp.valueOf (OldValue));
                    if (NewValue != null)
                        showNewValue = _dateTimeFormat.Format(NewValue);// (Timestamp.valueOf (NewValue));
                }
                else if (DisplayType.IsLookup(column.GetAD_Reference_ID()))
                {
                    MLookup lookup = VAdvantage.Classes.VLookUpFactory.Get(ctx, 0,
                        AD_Column_ID, column.GetAD_Reference_ID(),
                         column.GetColumnName(),
                        column.GetAD_Reference_Value_ID(),
                        column.IsParent(), null);

                    if (OldValue != null)
                    {
                        Object key = OldValue;
                        if (column.GetAD_Reference_ID() != DisplayType.List
                            && column.GetColumnName().EndsWith("_ID"))
                        {
                            try
                            {
                                key = Convert.ToInt32(OldValue);
                            }
                            catch (Exception e)
                            {
                            }
                        }
                        NamePair pp = lookup.Get(key);
                        if (pp != null)
                            showOldValue = pp.GetName();
                    }
                    if (NewValue != null)
                    {
                        Object key = NewValue;
                        if (column.GetAD_Reference_ID() != DisplayType.List
                            && column.GetColumnName().EndsWith("_ID"))
                        {
                            try
                            {
                                key = Convert.ToInt32(NewValue);
                            }
                            catch (Exception e)
                            {
                            }
                        }
                        NamePair pp = lookup.Get(key);
                        if (pp != null)
                            showNewValue = pp.GetName();
                    }
                }
                else if (DisplayType.IsLOB(column.GetAD_Reference_ID()))
                {
                }

            }
            catch (Exception e)
            {
                //log.Log(Level.WARNING, OldValue + "->" + NewValue, e);
            }
            //
            line.NewValue = showNewValue;
            line.OldValue = showOldValue;
            //	UpdatedBy
            MUser user = MUser.Get(ctx, UpdatedBy);
            line.UpdatedBy = user.GetName();
            //	Updated
            line.Updated = Convert.ToDateTime(Updated);
            return line;
        }



        /// <summary>
        /// clean up
        /// </summary>

        internal static CardViewData GetCardViewDetail(int AD_Window_ID, int AD_Tab_ID, Ctx ctx)
        {
            CardViewData cv = new CardViewData();
            cv.IncludedCols = new List<int>();
            cv.Conditions = new List<CardViewCondition>();

            int AD_CV_ID = -1;

            string sql = "SELECT AD_CardView_ID,AD_Field_ID FROM AD_CardView WHERE AD_Window_ID = " + AD_Window_ID + " AND AD_Tab_ID = " + AD_Tab_ID + " AND AD_USER_ID=" + ctx.GetAD_User_ID()
                            + " AND AD_Client_ID = " + ctx.GetAD_Client_ID() + " ORDER BY AD_USER_ID  ";
            IDataReader dr = null;
            try
            {
                dr = DB.ExecuteReader(sql);
                if (dr.Read())
                {
                    AD_CV_ID = Convert.ToInt32(dr[0]);
                    cv.FieldGroupID = VAdvantage.Utility.Util.GetValueOfInt(dr[1]);
                    cv.AD_CardView_ID = AD_CV_ID;
                }
                else
                {
                    dr.Close();
                    sql = "SELECT c.AD_CardView_ID, c.AD_Field_ID  FROM AD_CardView c INNER JOIN AD_CardView_Role r ON r.AD_Cardview_ID = r.AD_CardView_ID WHERE c.AD_Window_ID=" + AD_Window_ID + " AND "
                                 + " c.AD_Tab_ID=" + AD_Tab_ID + " AND r.AD_Role_ID = " + ctx.GetAD_Role_ID() + " AND c.AD_User_ID IS NULL";
                    dr = DB.ExecuteReader(sql);
                    if (dr.Read())
                    {
                        AD_CV_ID = Convert.ToInt32(dr[0]);
                        cv.FieldGroupID = VAdvantage.Utility.Util.GetValueOfInt(dr[1]);
                        cv.AD_CardView_ID = AD_CV_ID;
                    }
                }
                dr.Close();

                if (AD_CV_ID > 0)
                {
                    sql = "SELECT AD_Field_ID FROM AD_CardView_Column WHERE IsActive='Y' AND AD_CardView_ID = " + AD_CV_ID + " ORDER BY SeqNo";
                    dr = DB.ExecuteReader(sql);
                    while (dr.Read())
                    {
                        cv.IncludedCols.Add(VAdvantage.Utility.Util.GetValueOfInt(dr[0]));
                    }
                    dr.Close();
                }
                if (AD_CV_ID > 0)
                {
                    sql = "SELECT ConditionValue,Color  FROM AD_CardView_Condition WHERE IsActive='Y' AND AD_CardView_ID = " + AD_CV_ID + " ORDER BY AD_CardView_Condition_ID ";
                    dr = DB.ExecuteReader(sql);
                    while (dr.Read())
                    {
                        var cdc = new CardViewCondition();
                        cdc.Color = dr[1].ToString();
                        cdc.ConditionValue = dr[0].ToString();
                        cv.Conditions.Add(cdc);
                    }
                    dr.Close();
                }
            }
            catch
            {
                if (dr != null)
                    dr.Close();
            }
            return cv;
        }

        public void Dispose()
        {
            _createSqlColumn.Clear();
            _createSqlColumn = null;
            _createSqlValue.Clear();
            _createSqlValue = null;
            _lobInfo = null;
            key = null;
        }

        //public string GetTreeNodePath(Ctx ctx, int node_ID, int TreeID)
        //{
        //    MTree tree = new MTree(ctx, TreeID, null);

        //    OracleConnection conn = new OracleConnection(DBConn.CreateConnectionString());
        //    OracleCommand cmd = new OracleCommand("GetTreeNodePath");
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    OracleParameter pNodeID = new OracleParameter("nodeID", node_ID);
        //    cmd.Parameters.Add(pNodeID);

        //    OracleParameter pAdLanguage = new OracleParameter("adlanguage", ctx.GetAD_Language());
        //    cmd.Parameters.Add(pAdLanguage);

        //    OracleParameter pNodeTableName = new OracleParameter("nodeTableName", tree.GetNodeTableName());
        //    cmd.Parameters.Add(pNodeTableName);

        //    string tableName = MTable.GetTableName(ctx, tree.GetAD_Table_ID());

        //    OracleParameter pRecordTableName = new OracleParameter("recordTableName", tableName);
        //    cmd.Parameters.Add(pRecordTableName);

        //    OracleParameter pRecordKeyCol = new OracleParameter("recordKeyCol", tableName + "_ID");
        //    cmd.Parameters.Add(pRecordKeyCol);

        //    OracleParameter pretValue = new OracleParameter();
        //    pretValue.ParameterName = "retValue";
        //    pretValue.Direction = ParameterDirection.Output;
        //    pretValue.OracleType = OracleType.VarChar;
        //    pretValue.Size = 2000;
        //    cmd.Parameters.Add(pretValue);



        //    cmd.Connection = conn;
        //    object otput = "";
        //    try
        //    {
        //        if (conn.State == ConnectionState.Closed)
        //        {
        //            conn.Open();
        //        }


        //        cmd.ExecuteScalar();

        //        otput = pretValue.Value;

        //        conn.Close();
        //    }
        //    catch
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //        }

        //    }

        //    if (otput.ToString().IndexOf("\\") > 0)
        //    {
        //        otput = otput.ToString().Substring(0, otput.ToString().LastIndexOf("\\"));
        //    }

        //    if (otput.ToString().IndexOf("\\") > 0)
        //    {
        //        otput = otput.ToString().Substring(0, otput.ToString().LastIndexOf("\\"));
        //        otput = tree.GetName() + "\\ " + otput;
        //    }
        //    else
        //    {
        //        otput = tree.GetName();
        //    }


        //    return otput.ToString();

        //}


        public string GetTreeNodePath(Ctx ctx, int node_ID, int TreeID)
        {
            MTree tree = new MTree(ctx, TreeID, null);

            object otput = "";

            string tableName = MTable.GetTableName(ctx, tree.GetAD_Table_ID());
            string sql = "select gettreenodepaths(" + node_ID + ",'" + ctx.GetAD_Language() + "','" + tree.GetNodeTableName() + "','" + tableName + "','" + tableName + "_ID', " + TreeID + ") from dual";

            otput = DB.ExecuteScalar(sql, null, null);


            if (otput.ToString().IndexOf("\\") > 0)
            {
                otput = otput.ToString().Substring(0, otput.ToString().LastIndexOf("\\"));
            }

            if (otput.ToString().IndexOf("\\") > 0)
            {
                otput = otput.ToString().Substring(0, otput.ToString().LastIndexOf("\\"));
                otput = tree.GetName() + "\\ " + otput;
            }
            else
            {
                otput = tree.GetName();
            }

            return otput.ToString();

        }

        public void InsertUpdateDefaultSearch(Ctx _ctx, int AD_Tab_ID, int AD_Table_ID, int AD_User_ID, int? AD_UserQuery_ID)
        {

            if (AD_UserQuery_ID == 0 || AD_UserQuery_ID == null)
            {
                DB.ExecuteQuery("DELETE FROM AD_DefaultUserQuery WHERE AD_Tab_ID=" + AD_Tab_ID + " AND AD_Table_ID=" + AD_Table_ID + " AND AD_User_ID=" + AD_User_ID);
                return;
            }


            string sql = "SELECT AD_DefaultUserQuery_ID FROM AD_DefaultUserQuery WHERE AD_Tab_ID=" + AD_Tab_ID + " AND AD_Table_ID=" + AD_Table_ID + " AND AD_User_ID=" + AD_User_ID;
            object id = DB.ExecuteScalar(sql);
            int AD_DefaultUserQuery_ID = 0;
            if (id != null && id != DBNull.Value)
            {
                AD_DefaultUserQuery_ID = Convert.ToInt32(id);
            }


            X_AD_DefaultUserQuery userQuery = new X_AD_DefaultUserQuery(_ctx, AD_DefaultUserQuery_ID, null);
            userQuery.SetAD_Tab_ID(AD_Tab_ID);
            userQuery.SetAD_Table_ID(AD_Table_ID);
            userQuery.SetAD_User_ID(AD_User_ID);
            userQuery.SetAD_UserQuery_ID(Convert.ToInt32(AD_UserQuery_ID));
            userQuery.Save();



        }
        /// <summary>
        /// Method to get parent tab records ID.
        /// </summary>
        /// <param name="SelectColumn">Column  to be selected</param>
        /// <param name="SelectTable">From table</param>
        /// <param name="WhereColumn">Where column</param>
        /// <param name="WhereValue">ID of child column</param>
        /// <returns></returns>
        public int GetZoomParentRecord(string SelectColumn, string SelectTable, string WhereColumn, string WhereValue)
        {
            int recordID = 0;
            string sql = "SELECT " + SelectColumn + " FROM " + SelectTable + " WHERE " + WhereColumn + "=" + Util.GetValueOfInt(WhereValue);
            recordID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            return recordID;
        }
    }
}