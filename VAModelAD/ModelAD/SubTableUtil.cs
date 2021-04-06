/********************************************************
 * Module Name    : Application Dictionry
 * Purpose        : Fetch the column form database and insert into VAF_TableView 
 * Class Used     : context.cs,globalvariable.cs...
 * Chronological Development
 * Jagmohan Bhatt       03-Sept-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using System.Data;
using VAdvantage.Utility;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    /// <summary>
    /// Generate Delta or History Tables
    /// </summary>
    public class SubTableUtil
    {
        /// <summary>
        /// Generate Sub Table Infrastructure
        /// </summary>
        /// <param name="Context">context</param>
        /// <param name="tableName">name of the table</param>
        /// <param name="userDef">user defined</param>
        public SubTableUtil(Context Context, String tableName, bool userDef)
            : this(Context, tableName, userDef, false)
        {

        }	//	SubTableUtil

        /// <summary>
        /// Generate History Infrastructure
        /// </summary>
        /// <param name="Context">context</param>
        /// <param name="tableName">name of the table</param>
        public SubTableUtil(Context Context, String tableName)
            : this(Context, tableName, false, true)
        {

        }	//	SubTableUtil

        /// <summary>
        /// Generate Delta Infrastructure
        /// </summary>
        /// <param name="Context">context</param>
        /// <param name="tableName">table name</param>
        /// <param name="userDef">user defined</param>
        /// <param name="history">history</param>
        private SubTableUtil(Context Context, String tableName, bool userDef, bool history)
        {
            m_ctx = Context;
            m_baseTable = MVAFTableView.Get(Context, tableName);
            if (m_baseTable == null)
                throw new ArgumentException("Not found: " + tableName);
            m_userDef = userDef;
            m_history = history;
            int index = tableName.IndexOf("_");
            m_dTableName = tableName.Substring(0, index)
                + (m_userDef ? "_UserDef" : "_SysDef")
                + tableName.Substring(index);
            if (history)	//	Default=Each
                m_dTableName = tableName + "H";
            else
                m_derivedTableType = userDef ? X_VAF_TableView.SUBTABLETYPE_Delta_User : X_VAF_TableView.SUBTABLETYPE_Delta_System;
            m_vTableName = tableName + "_v";
            log.Info(tableName + " + " + m_dTableName + " = " + m_vTableName);
        }	//	SubTableUtil


        /** The base table		*/
        private Context m_ctx;
        /** The base table		*/
        private MVAFTableView m_baseTable;
        /** The derived table		*/
        private MVAFTableView m_derivedTable;
        /**	Sub Table Type			*/
        private String m_derivedTableType = X_VAF_TableView.SUBTABLETYPE_History_Each;
        /** The view table		*/
        private MVAFTableView m_viewTable;
        /** UserDef or SysDef Delta	*/
        private bool m_userDef;
        /** History				*/
        private bool m_history;
        /** The delta table name	*/
        private String m_dTableName;
        /** The view		 name	*/
        private String m_vTableName;

        /**	Logger	*/
        private static VLogger log = VLogger.GetVLogger(typeof(SubTableUtil).FullName);



        /// <summary>
        /// Generate Table and View
        /// </summary>
        /// <returns></returns>
        private bool Generate()
        {
            try
            {
                CheckStandardColumns(m_ctx, m_baseTable.GetTableName());
                GenerateTable();
                CheckStandardColumns(m_ctx, m_derivedTable.GetTableName());

                if (!m_history)
                    GenerateView();
            }
            catch 
            {
                //e.StackTrace;
                return false;
            }
            return true;
        }	//	generate


        /// <summary>
        /// Generate Delta and History Table
        /// </summary>
        /// <returns></returns>
        private bool GenerateTable()
        {
            //	Table
            m_derivedTable = MVAFTableView.Get(m_ctx, m_dTableName);
            if (m_derivedTable == null)
                m_derivedTable = new MVAFTableView(m_ctx, 0, null);

            PO.CopyValues(m_baseTable, m_derivedTable);
            m_derivedTable.SetTableName(m_dTableName);
            m_derivedTable.SetName(m_derivedTable.GetName() + " SubTable");
            m_derivedTable.SetSubTableType(m_derivedTableType);
            m_derivedTable.SetBase_Table_ID(m_baseTable.GetVAF_TableView_ID());
            if (!m_derivedTable.Save())
                throw new Exception("Cannot save " + m_dTableName);

            MVAFColumn[] dCols = SyncMColumns(true);

            //	Sync Columns in Database
            List<MVAFColumn> list = new List<MVAFColumn>(dCols.Length);
            foreach (MVAFColumn element in dCols)
                list.Add(element);
            Trx trx = Trx.Get("getDatabaseMetaData");
            //
            
            String catalog = "";
            String schema = CoreLibrary.DataBase.DB.GetSchema();
            String tableName = m_dTableName;
            tableName = tableName.ToUpper();
            int noColumns = 0;
            //
            DataSet rs = null;
            using (DatabaseMetaData md = new DatabaseMetaData())
            {
                 rs = md.GetColumns(catalog, schema, tableName);
            }
            for (int rscount = 0; rscount <= rs.Tables[0].Rows.Count - 1; rscount++)
            {
                noColumns++;
                String columnName = rs.Tables[0].Rows[rscount]["COLUMN_NAME"].ToString();
                bool found = false;
                for (int i = 0; i < list.Count; i++)
                {
                    MVAFColumn dCol = list[i];
                    if (columnName.Equals(dCol.GetColumnName(), StringComparison.OrdinalIgnoreCase))
                    {
                        String sql = dCol.GetSQLModify(m_derivedTable, false);
                        CoreLibrary.DataBase.DB.ExecuteUpdateMultiple(sql, false, null);
                        found = true;
                        list.Remove(list[i]);
                        break;
                    }
                }
                if (!found)
                {
                    String sql = "ALTER TABLE " + m_dTableName + " DROP COLUMN " + columnName;
                    CoreLibrary.DataBase.DB.ExecuteQuery(sql, null);
                }
            }
            //rs.close();
            trx.Close();

            //	No Columns
            if (noColumns == 0)
            {
                String sql = m_derivedTable.GetSQLCreate();
                return CoreLibrary.DataBase.DB.ExecuteUpdateMultiple(sql, false, null) >= 0;
            }
            //	New Columns
            for (int i = 0; i < list.Count(); i++)
            {
                MVAFColumn dCol = list[i];
                if (dCol.IsVirtualColumn())
                    continue;
                String sql = dCol.GetSQLAdd(m_derivedTable);
                CoreLibrary.DataBase.DB.ExecuteUpdateMultiple(sql, false, null);
            }
            return true;
        }	//	generateTable



        /// <summary>
        /// Generate Delta View
        /// </summary>
        /// <returns>true, if created</returns>
        private bool GenerateView()
        {
            //	View
            m_viewTable = MVAFTableView.Get(m_ctx, m_vTableName);
            if (m_viewTable == null)
                m_viewTable = new MVAFTableView(m_ctx, 0, null);

            PO.CopyValues(m_baseTable, m_viewTable);
            m_viewTable.SetTableName(m_vTableName);
            m_viewTable.SetName(m_baseTable.GetName() + " View");
            m_viewTable.SetIsView(true);
            if (!m_viewTable.Save())
                throw new Exception("Cannot save " + m_vTableName);

            MVAFColumn[] vCols = SyncMColumns(false);

            //ColumnSync
            //	Create View
            StringBuilder sql = new StringBuilder("CREATE OR REPLACE VIEW ")
                .Append(m_vTableName)
                .Append(" AS SELECT ");
            for (int i = 0; i < vCols.Length; i++)
            {
                if (i > 0)
                    sql.Append(",");
                MVAFColumn column = vCols[i];
                String columnName = column.GetColumnName();
                if (column.IsStandardColumn()
                    || column.IsKey())
                    sql.Append("b.").Append(columnName);
                else
                    sql.Append(",COALESCE(b.").Append(columnName)
                        .Append("d.").Append(columnName).Append(") AS ").Append(columnName);
            }
            //	From
            String keyColumnName = m_baseTable.GetTableName() + "_ID";
            sql.Append(" FROM ").Append(m_baseTable.GetTableName())
                .Append(" b LEFT OUTER JOIN ").Append(m_dTableName)
                .Append(" d ON (b.").Append(keyColumnName)
                .Append("=d.").Append(keyColumnName).Append(")");
            //
            log.Info(sql.ToString());
            if (DataBase.DB.ExecuteQuery(sql.ToString(), null) < 0)
            {
                return false;
            }
            return true;
        }	//	generateView


        /// <summary>
        /// Synchronize target table with base table in dictionary
        /// </summary>
        /// <param name="derived">is derived</param>
        /// <returns></returns>
        private MVAFColumn[] SyncMColumns(bool derived)
        {
            MVAFTableView target = derived ? m_derivedTable : m_viewTable;
            MVAFTableView source = derived ? m_baseTable : m_derivedTable;
            MVAFColumn[] sCols = source.GetColumns(false);
            MVAFColumn[] tCols = target.GetColumns(false);
            //	Base Columns
            foreach (MVAFColumn sCol in sCols)
            {
                MVAFColumn tCol = null;
                foreach (MVAFColumn column in tCols)
                {
                    if (sCol.GetColumnName().Equals(column.GetColumnName()))
                    {
                        tCol = column;
                        break;
                    }
                }
                if (tCol == null)
                    tCol = new MVAFColumn(target);
                PO.CopyValues(sCol, tCol);
                tCol.SetVAF_TableView_ID(target.GetVAF_TableView_ID());	//	reset parent
                tCol.SetIsCallout(false);
                tCol.SetCallout(null);
                tCol.SetIsMandatory(false);
                tCol.SetIsMandatoryUI(false);
                tCol.SetIsTranslated(false);
                //	tCol.SetIsUpdateable(true);
                if (tCol.IsKey())
                {
                    tCol.SetIsKey(false);
                    tCol.SetVAF_Control_Ref_ID(DisplayType.TableDir);
                }
                if (tCol.Save())
                    throw new Exception("Cannot save column " + sCol.GetColumnName());
            }
            //
            tCols = target.GetColumns(true);
            List<String> addlColumns = new List<String>();
            if (derived && !m_history)	//	delta only
            {
                //	KeyColumn
                String keyColumnName = target.GetTableName() + "_ID";
                MVAFColumn key = target.GetColumn(keyColumnName);
                if (key == null)
                {
                    key = new MVAFColumn(target);
                    M_VAFColumnDic ele = M_VAFColumnDic.Get(m_ctx, keyColumnName, target.Get_TrxName());
                    if (ele == null)
                    {
                        ele = new M_VAFColumnDic(m_ctx, keyColumnName, target.GetRecordType(), null);
                        ele.Save();
                    }
                    key.SetVAF_ColumnDic_ID(ele.GetVAF_ColumnDic_ID());
                    key.SetVAF_Control_Ref_ID(DisplayType.ID);
                    key.Save();
                }
                addlColumns.Add(keyColumnName);
                //	Addl References
                if (m_userDef)
                {
                    String colName = "VAF_Role_ID";
                    addlColumns.Add(colName);
                    if (target.GetColumn(colName) == null)
                    {
                        MVAFColumn col = new MVAFColumn(target);
                        col.SetColumnName(colName);
                        col.SetVAF_Control_Ref_ID(DisplayType.TableDir);
                        CreateColumn(col, target, false);
                        col.SetIsUpdateable(false);
                        col.SetIsMandatory(false);
                    }
                    colName = "VAF_UserContact_ID";
                    addlColumns.Add(colName);
                    if (target.GetColumn(colName) == null)
                    {
                        MVAFColumn col = new MVAFColumn(target);
                        col.SetColumnName(colName);
                        col.SetVAF_Control_Ref_ID(DisplayType.TableDir);
                        col.SetIsUpdateable(false);
                        col.SetIsMandatory(false);
                        CreateColumn(col, target, false);
                    }
                }
                else	//	System
                {
                    String colName = "IsSystemDefault";
                    addlColumns.Add(colName);
                    if (target.GetColumn(colName) == null)
                    {
                        MVAFColumn col = new MVAFColumn(target);
                        col.SetColumnName(colName);
                        col.SetVAF_Control_Ref_ID(DisplayType.YesNo);
                        col.SetDefaultValue("N");
                        col.SetIsUpdateable(false);
                        col.SetIsMandatory(true);
                        CreateColumn(col, target, false);
                    }
                }
            }


            //	Delete
            foreach (MVAFColumn tCol in tCols)
            {
                MVAFColumn sCol = null;
                foreach (MVAFColumn column in sCols)
                {
                    if (tCol.GetColumnName().Equals(column.GetColumnName()))
                    {
                        sCol = column;
                        break;
                    }
                }
                if (sCol == null)
                {
                    if (!addlColumns.Contains(tCol.GetColumnName()))
                    {
                        if (!tCol.Delete(true))
                            throw new Exception("Cannot delete column "
                                    + tCol.GetColumnName());
                    }
                }
            }
            return tCols;
        }	//	SyncMColumns

        
        /// <summary>
        /// Check Existence of Std columns and create them in AD and DB
        /// </summary>
        /// <param name="Context">context</param>
        /// <param name="tableName">name of the  table</param>
        static void CheckStandardColumns(Context Context, String tableName)
        {
            MVAFTableView table = MVAFTableView.Get(Context, tableName);
            CheckStandardColumns(table, null);
        }

        /// <summary>
        /// Check Existence of Std columns and create them in AD and DB
        /// </summary>
        /// <param name="table">name of the table</param>
        /// <param name="RecordType">Entity Type</param>
        public static void CheckStandardColumns(MVAFTableView table, String RecordType)
        {
            if (table == null)
                return;
            if (Utility.Util.IsEmpty(RecordType))
                RecordType = table.GetRecordType();
            table.GetColumns(true);		//	get new columns
            //	Key Column
            String colName = table.GetTableName() + "_ID";
            if (table.GetColumn(colName) == null)
            {
                MVAFColumn col = new MVAFColumn(table);
                col.SetColumnName(colName);
                col.SetVAF_Control_Ref_ID(DisplayType.ID);
                col.SetIsKey(true);
                col.SetIsUpdateable(false);
                col.SetIsMandatory(true);
                col.SetRecordType(RecordType);
                col.SetIsCopy(false);           // By Default isCopy check box should be False on this Column.
                CreateColumn(col, table, true);
            }
            colName = "VAF_Client_ID";
            if (table.GetColumn(colName) == null)
            {
                MVAFColumn col = new MVAFColumn(table);
                col.SetColumnName(colName);
                col.SetVAF_Control_Ref_ID(DisplayType.TableDir);
                col.SetIsUpdateable(false);
                col.SetIsMandatory(true);
                col.SetVAF_DataVal_Rule_ID(116);	//	Client Login
                col.SetDefaultValue("@#VAF_Client_ID@");
                col.SetConstraintType(X_VAF_Column.CONSTRAINTTYPE_Restrict);
                col.SetRecordType(RecordType);
                col.SetIsCopy(true);            // By Default isCopy check box should be true on this Column.
                CreateColumn(col, table, true);
            }
            colName = "VAF_Org_ID";
            if (table.GetColumn(colName) == null)
            {
                MVAFColumn col = new MVAFColumn(table);
                col.SetColumnName(colName);
                col.SetVAF_Control_Ref_ID(DisplayType.TableDir);
                col.SetIsUpdateable(false);
                col.SetIsMandatory(true);
                col.SetIsMandatoryUI(true);
                col.SetVAF_DataVal_Rule_ID(104);	//	Org Security
                col.SetDefaultValue("@#VAF_Org_ID@");
                col.SetConstraintType(X_VAF_Column.CONSTRAINTTYPE_Restrict);
                col.SetRecordType(RecordType);
                col.SetIsCopy(true);            // By Default isCopy check box should be true on this Column.
                CreateColumn(col, table, true);
            }
            colName = "Created";
            if (table.GetColumn(colName) == null)
            {
                MVAFColumn col = new MVAFColumn(table);
                col.SetColumnName(colName);
                col.SetVAF_Control_Ref_ID(DisplayType.DateTime);
                col.SetIsUpdateable(false);
                col.SetIsMandatory(true);
                col.SetRecordType(RecordType);
                col.SetIsCopy(false);           // By Default isCopy check box should be False on this Column.
                CreateColumn(col, table, true);
            }
            colName = "Updated";
            if (table.GetColumn(colName) == null)
            {
                MVAFColumn col = new MVAFColumn(table);
                col.SetColumnName(colName);
                col.SetVAF_Control_Ref_ID(DisplayType.DateTime);
                col.SetIsUpdateable(false);
                col.SetIsMandatory(true);
                col.SetRecordType(RecordType);
                col.SetIsCopy(false);           // By Default isCopy check box should be False on this Column.
                CreateColumn(col, table, true);
            }
            colName = "CreatedBy";
            if (table.GetColumn(colName) == null)
            {
                MVAFColumn col = new MVAFColumn(table);
                col.SetColumnName(colName);
                col.SetVAF_Control_Ref_ID(DisplayType.Table);
                col.SetVAF_Control_Ref_Value_ID(110);
                col.SetConstraintType(X_VAF_Column.CONSTRAINTTYPE_DoNOTCreate);
                col.SetIsUpdateable(false);
                col.SetIsMandatory(true);
                col.SetRecordType(RecordType);
                col.SetIsCopy(false);           // By Default isCopy check box should be False on this Column.
                CreateColumn(col, table, true);
            }
            colName = "UpdatedBy";
            if (table.GetColumn(colName) == null)
            {
                MVAFColumn col = new MVAFColumn(table);
                col.SetColumnName(colName);
                col.SetVAF_Control_Ref_ID(DisplayType.Table);
                col.SetVAF_Control_Ref_Value_ID(110);
                col.SetConstraintType(X_VAF_Column.CONSTRAINTTYPE_DoNOTCreate);
                col.SetIsUpdateable(false);
                col.SetIsMandatory(true);
                col.SetRecordType(RecordType);
                col.SetIsCopy(false);           // By Default isCopy check box should be False on this Column.
                CreateColumn(col, table, true);
            }
            colName = "IsActive";
            if (table.GetColumn(colName) == null)
            {
                MVAFColumn col = new MVAFColumn(table);
                col.SetColumnName(colName);
                col.SetVAF_Control_Ref_ID(DisplayType.YesNo);
                col.SetDefaultValue("Y");
                col.SetIsUpdateable(true);
                col.SetIsMandatory(true);
                col.SetRecordType(RecordType);
                col.SetIsCopy(false);           // By Default isCopy check box should be False on this Column.
                CreateColumn(col, table, true);
            }
            colName = "Export_ID";
            if (table.GetColumn(colName) == null)
            {
                MVAFColumn col = new MVAFColumn(table);
                col.SetColumnName(colName);
                col.SetVAF_Control_Ref_ID(DisplayType.String);
                col.SetIsUpdateable(true);
                col.SetIsMandatory(false);
                col.SetRecordType(RecordType);
                col.SetFieldLength(50);
                col.SetIsCopy(false);           // By Default isCopy check box should be False on this Column.
                CreateColumn(col, table, true);
            }
        }	//	checkStandardColumns

 
        /// <summary>
        /// Create Column in AD and DB
        /// </summary>
        /// <param name="col">Column Object</param>
        /// <param name="table">Table Object</param>
        /// <param name="alsoInDB">Also in DB</param>
        /// <returns></returns>
        private static bool CreateColumn(MVAFColumn col, MVAFTableView table, bool alsoInDB)
        {
            //	Element
            M_VAFColumnDic element = M_VAFColumnDic.Get(col.GetCtx(), col.GetColumnName(), col.Get_TrxName());
            if (element == null)
            {
                element = new M_VAFColumnDic(col.GetCtx(), col.GetColumnName(), col.GetRecordType(), null);
                if (!element.Save())
                    return false;
                log.Info("Created Element: " + element.GetColumnName());
            }
            //	Column Sync
            col.SetColumnName(element.GetColumnName());
            col.SetName(element.GetName());
            col.SetDescription(element.GetDescription());
            col.SetHelp(element.GetHelp());
            col.SetVAF_ColumnDic_ID(element.GetVAF_ColumnDic_ID());
            //

            if (!col.Save())
                return false;

            //	DB
            if (!alsoInDB)
                return true;
            //
            String sql = col.GetSQLAdd(table);
            bool success = CoreLibrary.DataBase.DB.ExecuteUpdateMultiple(sql, false, table.Get_TrxName()) >= 0;
            if (success)
                log.Info("Created: " + table.GetTableName()
                    + "." + col.GetColumnName());
            else
                log.Warning("NOT Created: " + table.GetTableName()
                    + "." + col.GetColumnName());
            return success;
        }	//	createColumn


    }
}
