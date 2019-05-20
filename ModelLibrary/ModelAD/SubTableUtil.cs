/********************************************************
 * Module Name    : Application Dictionry
 * Purpose        : Fetch the column form database and insert into AD_Table 
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
            m_baseTable = MTable.Get(Context, tableName);
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
                m_derivedTableType = userDef ? X_AD_Table.SUBTABLETYPE_Delta_User : X_AD_Table.SUBTABLETYPE_Delta_System;
            m_vTableName = tableName + "_v";
            log.Info(tableName + " + " + m_dTableName + " = " + m_vTableName);
        }	//	SubTableUtil


        /** The base table		*/
        private Context m_ctx;
        /** The base table		*/
        private MTable m_baseTable;
        /** The derived table		*/
        private MTable m_derivedTable;
        /**	Sub Table Type			*/
        private String m_derivedTableType = X_AD_Table.SUBTABLETYPE_History_Each;
        /** The view table		*/
        private MTable m_viewTable;
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
            m_derivedTable = MTable.Get(m_ctx, m_dTableName);
            if (m_derivedTable == null)
                m_derivedTable = new MTable(m_ctx, 0, null);

            PO.CopyValues(m_baseTable, m_derivedTable);
            m_derivedTable.SetTableName(m_dTableName);
            m_derivedTable.SetName(m_derivedTable.GetName() + " SubTable");
            m_derivedTable.SetSubTableType(m_derivedTableType);
            m_derivedTable.SetBase_Table_ID(m_baseTable.GetAD_Table_ID());
            if (!m_derivedTable.Save())
                throw new Exception("Cannot save " + m_dTableName);

            MColumn[] dCols = SyncMColumns(true);

            //	Sync Columns in Database
            List<MColumn> list = new List<MColumn>(dCols.Length);
            foreach (MColumn element in dCols)
                list.Add(element);
            Trx trx = Trx.Get("getDatabaseMetaData");
            //
            
            String catalog = "";
            String schema = DataBase.DB.GetSchema();
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
                    MColumn dCol = list[i];
                    if (columnName.Equals(dCol.GetColumnName(), StringComparison.OrdinalIgnoreCase))
                    {
                        String sql = dCol.GetSQLModify(m_derivedTable, false);
                        DataBase.DB.ExecuteUpdateMultiple(sql, false, null);
                        found = true;
                        list.Remove(list[i]);
                        break;
                    }
                }
                if (!found)
                {
                    String sql = "ALTER TABLE " + m_dTableName + " DROP COLUMN " + columnName;
                    DataBase.DB.ExecuteQuery(sql, null);
                }
            }
            //rs.close();
            trx.Close();

            //	No Columns
            if (noColumns == 0)
            {
                String sql = m_derivedTable.GetSQLCreate();
                return DataBase.DB.ExecuteUpdateMultiple(sql, false, null) >= 0;
            }
            //	New Columns
            for (int i = 0; i < list.Count(); i++)
            {
                MColumn dCol = list[i];
                if (dCol.IsVirtualColumn())
                    continue;
                String sql = dCol.GetSQLAdd(m_derivedTable);
                DataBase.DB.ExecuteUpdateMultiple(sql, false, null);
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
            m_viewTable = MTable.Get(m_ctx, m_vTableName);
            if (m_viewTable == null)
                m_viewTable = new MTable(m_ctx, 0, null);

            PO.CopyValues(m_baseTable, m_viewTable);
            m_viewTable.SetTableName(m_vTableName);
            m_viewTable.SetName(m_baseTable.GetName() + " View");
            m_viewTable.SetIsView(true);
            if (!m_viewTable.Save())
                throw new Exception("Cannot save " + m_vTableName);

            MColumn[] vCols = SyncMColumns(false);

            //ColumnSync
            //	Create View
            StringBuilder sql = new StringBuilder("CREATE OR REPLACE VIEW ")
                .Append(m_vTableName)
                .Append(" AS SELECT ");
            for (int i = 0; i < vCols.Length; i++)
            {
                if (i > 0)
                    sql.Append(",");
                MColumn column = vCols[i];
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
        private MColumn[] SyncMColumns(bool derived)
        {
            MTable target = derived ? m_derivedTable : m_viewTable;
            MTable source = derived ? m_baseTable : m_derivedTable;
            MColumn[] sCols = source.GetColumns(false);
            MColumn[] tCols = target.GetColumns(false);
            //	Base Columns
            foreach (MColumn sCol in sCols)
            {
                MColumn tCol = null;
                foreach (MColumn column in tCols)
                {
                    if (sCol.GetColumnName().Equals(column.GetColumnName()))
                    {
                        tCol = column;
                        break;
                    }
                }
                if (tCol == null)
                    tCol = new MColumn(target);
                PO.CopyValues(sCol, tCol);
                tCol.SetAD_Table_ID(target.GetAD_Table_ID());	//	reset parent
                tCol.SetIsCallout(false);
                tCol.SetCallout(null);
                tCol.SetIsMandatory(false);
                tCol.SetIsMandatoryUI(false);
                tCol.SetIsTranslated(false);
                //	tCol.SetIsUpdateable(true);
                if (tCol.IsKey())
                {
                    tCol.SetIsKey(false);
                    tCol.SetAD_Reference_ID(DisplayType.TableDir);
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
                MColumn key = target.GetColumn(keyColumnName);
                if (key == null)
                {
                    key = new MColumn(target);
                    M_Element ele = M_Element.Get(m_ctx, keyColumnName, target.Get_TrxName());
                    if (ele == null)
                    {
                        ele = new M_Element(m_ctx, keyColumnName, target.GetEntityType(), null);
                        ele.Save();
                    }
                    key.SetAD_Element_ID(ele.GetAD_Element_ID());
                    key.SetAD_Reference_ID(DisplayType.ID);
                    key.Save();
                }
                addlColumns.Add(keyColumnName);
                //	Addl References
                if (m_userDef)
                {
                    String colName = "AD_Role_ID";
                    addlColumns.Add(colName);
                    if (target.GetColumn(colName) == null)
                    {
                        MColumn col = new MColumn(target);
                        col.SetColumnName(colName);
                        col.SetAD_Reference_ID(DisplayType.TableDir);
                        CreateColumn(col, target, false);
                        col.SetIsUpdateable(false);
                        col.SetIsMandatory(false);
                    }
                    colName = "AD_User_ID";
                    addlColumns.Add(colName);
                    if (target.GetColumn(colName) == null)
                    {
                        MColumn col = new MColumn(target);
                        col.SetColumnName(colName);
                        col.SetAD_Reference_ID(DisplayType.TableDir);
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
                        MColumn col = new MColumn(target);
                        col.SetColumnName(colName);
                        col.SetAD_Reference_ID(DisplayType.YesNo);
                        col.SetDefaultValue("N");
                        col.SetIsUpdateable(false);
                        col.SetIsMandatory(true);
                        CreateColumn(col, target, false);
                    }
                }
            }


            //	Delete
            foreach (MColumn tCol in tCols)
            {
                MColumn sCol = null;
                foreach (MColumn column in sCols)
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
            MTable table = MTable.Get(Context, tableName);
            CheckStandardColumns(table, null);
        }

        /// <summary>
        /// Check Existence of Std columns and create them in AD and DB
        /// </summary>
        /// <param name="table">name of the table</param>
        /// <param name="EntityType">Entity Type</param>
        public static void CheckStandardColumns(MTable table, String EntityType)
        {
            if (table == null)
                return;
            if (Utility.Util.IsEmpty(EntityType))
                EntityType = table.GetEntityType();
            table.GetColumns(true);		//	get new columns
            //	Key Column
            String colName = table.GetTableName() + "_ID";
            if (table.GetColumn(colName) == null)
            {
                MColumn col = new MColumn(table);
                col.SetColumnName(colName);
                col.SetAD_Reference_ID(DisplayType.ID);
                col.SetIsKey(true);
                col.SetIsUpdateable(false);
                col.SetIsMandatory(true);
                col.SetEntityType(EntityType);
                col.SetIsCopy(false);           // By Default isCopy check box should be False on this Column.
                CreateColumn(col, table, true);
            }
            colName = "AD_Client_ID";
            if (table.GetColumn(colName) == null)
            {
                MColumn col = new MColumn(table);
                col.SetColumnName(colName);
                col.SetAD_Reference_ID(DisplayType.TableDir);
                col.SetIsUpdateable(false);
                col.SetIsMandatory(true);
                col.SetAD_Val_Rule_ID(116);	//	Client Login
                col.SetDefaultValue("@#AD_Client_ID@");
                col.SetConstraintType(X_AD_Column.CONSTRAINTTYPE_Restrict);
                col.SetEntityType(EntityType);
                col.SetIsCopy(true);            // By Default isCopy check box should be true on this Column.
                CreateColumn(col, table, true);
            }
            colName = "AD_Org_ID";
            if (table.GetColumn(colName) == null)
            {
                MColumn col = new MColumn(table);
                col.SetColumnName(colName);
                col.SetAD_Reference_ID(DisplayType.TableDir);
                col.SetIsUpdateable(false);
                col.SetIsMandatory(true);
                col.SetIsMandatoryUI(true);
                col.SetAD_Val_Rule_ID(104);	//	Org Security
                col.SetDefaultValue("@#AD_Org_ID@");
                col.SetConstraintType(X_AD_Column.CONSTRAINTTYPE_Restrict);
                col.SetEntityType(EntityType);
                col.SetIsCopy(true);            // By Default isCopy check box should be true on this Column.
                CreateColumn(col, table, true);
            }
            colName = "Created";
            if (table.GetColumn(colName) == null)
            {
                MColumn col = new MColumn(table);
                col.SetColumnName(colName);
                col.SetAD_Reference_ID(DisplayType.DateTime);
                col.SetIsUpdateable(false);
                col.SetIsMandatory(true);
                col.SetEntityType(EntityType);
                col.SetIsCopy(false);           // By Default isCopy check box should be False on this Column.
                CreateColumn(col, table, true);
            }
            colName = "Updated";
            if (table.GetColumn(colName) == null)
            {
                MColumn col = new MColumn(table);
                col.SetColumnName(colName);
                col.SetAD_Reference_ID(DisplayType.DateTime);
                col.SetIsUpdateable(false);
                col.SetIsMandatory(true);
                col.SetEntityType(EntityType);
                col.SetIsCopy(false);           // By Default isCopy check box should be False on this Column.
                CreateColumn(col, table, true);
            }
            colName = "CreatedBy";
            if (table.GetColumn(colName) == null)
            {
                MColumn col = new MColumn(table);
                col.SetColumnName(colName);
                col.SetAD_Reference_ID(DisplayType.Table);
                col.SetAD_Reference_Value_ID(110);
                col.SetConstraintType(X_AD_Column.CONSTRAINTTYPE_DoNOTCreate);
                col.SetIsUpdateable(false);
                col.SetIsMandatory(true);
                col.SetEntityType(EntityType);
                col.SetIsCopy(false);           // By Default isCopy check box should be False on this Column.
                CreateColumn(col, table, true);
            }
            colName = "UpdatedBy";
            if (table.GetColumn(colName) == null)
            {
                MColumn col = new MColumn(table);
                col.SetColumnName(colName);
                col.SetAD_Reference_ID(DisplayType.Table);
                col.SetAD_Reference_Value_ID(110);
                col.SetConstraintType(X_AD_Column.CONSTRAINTTYPE_DoNOTCreate);
                col.SetIsUpdateable(false);
                col.SetIsMandatory(true);
                col.SetEntityType(EntityType);
                col.SetIsCopy(false);           // By Default isCopy check box should be False on this Column.
                CreateColumn(col, table, true);
            }
            colName = "IsActive";
            if (table.GetColumn(colName) == null)
            {
                MColumn col = new MColumn(table);
                col.SetColumnName(colName);
                col.SetAD_Reference_ID(DisplayType.YesNo);
                col.SetDefaultValue("Y");
                col.SetIsUpdateable(true);
                col.SetIsMandatory(true);
                col.SetEntityType(EntityType);
                col.SetIsCopy(false);           // By Default isCopy check box should be False on this Column.
                CreateColumn(col, table, true);
            }
            colName = "Export_ID";
            if (table.GetColumn(colName) == null)
            {
                MColumn col = new MColumn(table);
                col.SetColumnName(colName);
                col.SetAD_Reference_ID(DisplayType.String);
                col.SetIsUpdateable(true);
                col.SetIsMandatory(false);
                col.SetEntityType(EntityType);
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
        private static bool CreateColumn(MColumn col, MTable table, bool alsoInDB)
        {
            //	Element
            M_Element element = M_Element.Get(col.GetCtx(), col.GetColumnName(), col.Get_TrxName());
            if (element == null)
            {
                element = new M_Element(col.GetCtx(), col.GetColumnName(), col.GetEntityType(), null);
                if (!element.Save())
                    return false;
                log.Info("Created Element: " + element.GetColumnName());
            }
            //	Column Sync
            col.SetColumnName(element.GetColumnName());
            col.SetName(element.GetName());
            col.SetDescription(element.GetDescription());
            col.SetHelp(element.GetHelp());
            col.SetAD_Element_ID(element.GetAD_Element_ID());
            //

            if (!col.Save())
                return false;

            //	DB
            if (!alsoInDB)
                return true;
            //
            String sql = col.GetSQLAdd(table);
            bool success = DataBase.DB.ExecuteUpdateMultiple(sql, false, table.Get_TrxName()) >= 0;
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
