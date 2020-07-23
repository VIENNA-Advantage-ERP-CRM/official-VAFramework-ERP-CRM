/********************************************************
 * Module Name    : Application Dictionry
 * Purpose        : Fetch the column form database and insert into AD_Table 
 * Class Used     : context.cs,globalvariable.cs...
 * Chronological Development
 * Jagmohan Bhatt       02-Sept-2009
  ******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using VAdvantage.Logging;
using VAdvantage.DataBase;
using System.Data;
using VAdvantage.Model;
using VAdvantage.Classes;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    /// <summary>
    /// Create Columns of Table or View
    /// </summary>
    public class TableCreateColumns : ProcessEngine.SvrProcess
    {
        /** Entity Type			*/
        private String p_EntityType = "C";	//	ENTITYTYPE_Customization
        /** Table				*/
        private int p_AD_Table_ID = 0;
        /** CheckAllDBTables	*/
        private bool p_AllTables = false;

        /** Column Count	*/
        private int m_count = 0;


        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            foreach (ProcessInfoParameter element in para)
            {
                String name = element.GetParameterName();
                if (element.GetParameter() == null)
                { }
                else if (name.Equals("EntityType"))
                    p_EntityType = (String)element.GetParameter();
                else if (name.Equals("AllTables"))
                    p_AllTables = "Y".Equals(element.GetParameter());
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
            p_AD_Table_ID = GetRecord_ID();
        }	//	prepare


        /// <summary>
        /// Process
        /// </summary>
        /// <returns></returns>
        protected override String DoIt()
        {
           if (p_AD_Table_ID == 0)
                throw new Exception("@NotFound@ @AD_Table_ID@ " + p_AD_Table_ID);
            log.Info("EntityType=" + p_EntityType
                + ", AllTables=" + p_AllTables
                + ", AD_Table_ID=" + p_AD_Table_ID);

            Trx trx = Trx.Get("getDatabaseMetaData");
            DatabaseMetaData md = new DatabaseMetaData();

            string catalog = "";
            string schema = DataBase.DB.GetSchema();
            if (p_AllTables)
            {
                AddTable(md, catalog, schema);
            }
            else
            {

                MTable table = new MTable(GetCtx(), p_AD_Table_ID, Get_Trx());
                if ((table == null) || (table.Get_ID() == 0))
                    throw new Exception("@NotFound@ @AD_Table_ID@ " + p_AD_Table_ID);
                log.Info(table.GetTableName() + ", EntityType=" + p_EntityType);
                String tableName = table.GetTableName();
                
                //tableName = tableName.ToUpper();
                DataSet rs = md.GetColumns(catalog, schema, tableName.ToUpper());
                AddTableColumn(GetCtx(), rs, table, p_EntityType);
                SubTableUtil.CheckStandardColumns(table, p_EntityType);
            }
            md.Dispose();
            trx.Close();
            return "#" + m_count;

        }

        /// <summary>
        /// Add Table in DB
        /// </summary>
        /// <param name="md">DB Info</param>
        /// <param name="catalog">current catalog</param>
        /// <param name="schema">schema</param>
        private void AddTable(DatabaseMetaData md, String catalog, String schema)
        {
            DataSet ds = md.GetTables();
            for (int i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
            {
                String tableName = ds.Tables[0].Rows[i]["TABLE_NAME"].ToString();
                String tableType = ds.Tables[0].Rows[i]["TABLE_TYPE"].ToString();

                //	Try to find
                MTable table = MTable.Get(GetCtx(), tableName);
                //	Create new ?
                if (table == null)
                {
                    String tn = tableName.ToUpper();
                    if (tn.StartsWith("T_SELECTION")	//	temp table
                        || tn.EndsWith("_VT")			//	print trl views
                        || tn.EndsWith("_V")			//	views
                        || tn.EndsWith("_V1")			//	views
                        || tn.StartsWith("A_A")			//	asset tables not yet
                        || tn.StartsWith("A_D")			//	asset tables not yet
                        || (tn.IndexOf("$") != -1		//	oracle system tables
                        )
                        || (tn.IndexOf("EXPLAIN") != -1	//	explain plan
                        )
                        )
                    {
                        log.Fine("Ignored: " + tableName + " - " + tableType);
                        continue;
                    }
                    //
                    log.Info(tableName + " - " + tableType);

                    //	Create New
                    table = new MTable(GetCtx(), 0, Get_Trx());
                    table.SetEntityType(p_EntityType);
                    table.SetName(tableName);
                    table.SetTableName(tableName);
                    table.SetIsView("VIEW".Equals(tableType));
                    if (!table.Save())
                        continue;
                }
                //	Check Columns
                tableName = tableName.ToUpper();
                DataSet rsC = md.GetColumns(catalog, schema, tableName);
                //addTableColumn(GetCtx(), rsC, table, p_EntityType);
                //SubTableUtil.checkStandardColumns(table, p_EntityType);
            }
        }	//	addTable

        /// <summary>
        /// Add Table columns in DB
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="rs">Dataset</param>
        /// <param name="table">Table Object</param>
        /// <param name="entityType">Entity type</param>
        /// <returns></returns>
        protected List<String> AddTableColumn(Ctx ctx, DataSet rs, MTable table, String entityType)
        {
            //MClientShare
            List<String> colName = new List<String>();
            String tableName = table.GetTableName();
            if (DatabaseType.IsOracle)
            {
                tableName = tableName.ToUpper();
                //
                for (int i = 0; i <= rs.Tables[0].Rows.Count - 1; i++)
                {
                    String tn = rs.Tables[0].Rows[i]["TABLE_NAME"].ToString();
                    if (!tableName.Equals(tn, StringComparison.OrdinalIgnoreCase))
                        continue;
                    String columnName = rs.Tables[0].Rows[i]["COLUMN_NAME"].ToString();
                    colName.Add(columnName);
                    MColumn column = table.GetColumn(columnName);
                    if (column != null)
                        continue;
                    //int dataType = Utility.Util.GetValueOfInt(rs.Tables[0].Rows[i]["DATATYPE"].ToString());
                    String typeName = rs.Tables[0].Rows[i]["DATATYPE"].ToString();
                    String nullable = rs.Tables[0].Rows[i]["NULLABLE"].ToString();
                    int size = Utility.Util.GetValueOfInt(rs.Tables[0].Rows[i]["LENGTH"]);
                    int digits = Utility.Util.GetValueOfInt(rs.Tables[0].Rows[i]["PRECISION"]);
                    //
                    log.Config(columnName + " - DataType=" + " " + typeName
                        + ", Nullable=" + nullable + ", Size=" + size + ", Digits="
                        + digits);
                    //
                    column = new MColumn(table);
                    column.SetEntityType(entityType);
                    //	Element
                    M_Element element = M_Element.Get(ctx, columnName, Get_Trx());
                    if (element == null)
                    {
                        element = new M_Element(ctx, columnName, entityType, Get_Trx());
                        element.Save();
                    }
                    //	Column Sync
                    column.SetColumnName(element.GetColumnName());
                    column.SetName(element.GetName());
                    column.SetDescription(element.GetDescription());
                    column.SetHelp(element.GetHelp());
                    column.SetAD_Element_ID(element.GetAD_Element_ID());
                    //	Other
                    column.SetIsMandatory("NO".Equals(nullable));
                    column.SetIsMandatoryUI(column.IsMandatory());                    

                    // Key
                    if (columnName.Equals(tableName + "_ID", StringComparison.OrdinalIgnoreCase))
                    {
                        column.SetIsKey(true);
                        column.SetAD_Reference_ID(DisplayType.ID);
                        column.SetIsUpdateable(false);
                    }
                    // Account
                    else if ((columnName.ToUpper().IndexOf("ACCT") != -1)
                        && (size == 10))
                        column.SetAD_Reference_ID(DisplayType.Account);
                    // Location
                    else if (columnName.Equals("C_Location_ID", StringComparison.OrdinalIgnoreCase))
                        column.SetAD_Reference_ID(DisplayType.Location);
                    // Product Attribute
                    else if (columnName.Equals("M_AttributeSetInstance_ID"))
                        column.SetAD_Reference_ID(DisplayType.PAttribute);
                    // SalesRep_ID (=User)
                    else if (columnName.Equals("SalesRep_ID", StringComparison.OrdinalIgnoreCase))
                    {
                        column.SetAD_Reference_ID(DisplayType.Table);
                        column.SetAD_Reference_Value_ID(190);
                    }
                    // ID
                    else if (columnName.EndsWith("_ID"))
                        column.SetAD_Reference_ID(DisplayType.TableDir);
                    // Date
                    else if ((typeName == Types.DATE) || (typeName == Types.TIME)
                        || (typeName == Types.TIMESTAMP)
                        // || columnName.toUpperCase().indexOf("DATE") != -1
                        || columnName.Equals("Created", StringComparison.OrdinalIgnoreCase)
                        || columnName.Equals("Updated", StringComparison.OrdinalIgnoreCase))
                    {
                        column.SetAD_Reference_ID(DisplayType.DateTime);
                        column.SetIsUpdateable(false);
                    }
                    // CreatedBy/UpdatedBy (=User)
                    else if (columnName.Equals("CreatedBy", StringComparison.OrdinalIgnoreCase)
                        || columnName.Equals("UpdatedBy", StringComparison.OrdinalIgnoreCase))
                    {
                        column.SetAD_Reference_ID(DisplayType.Table);
                        column.SetAD_Reference_Value_ID(110);
                        column.SetConstraintType(X_AD_Column.CONSTRAINTTYPE_DoNOTCreate);
                        column.SetIsUpdateable(false);
                    }
                    // Export_ID
                    // By Default isCopy check box should be False on this Column.
                    else if (columnName.Equals("Export_ID", StringComparison.OrdinalIgnoreCase))
                    {
                        column.SetIsCopy(false);
                    }
                    //	Entity Type
                    else if (columnName.Equals("EntityType", StringComparison.OrdinalIgnoreCase))
                    {
                        column.SetAD_Reference_ID(DisplayType.Table);
                        column.SetAD_Reference_Value_ID(389);
                        column.SetDefaultValue("U");
                        column.SetConstraintType(X_AD_Column.CONSTRAINTTYPE_Restrict);
                        column.SetReadOnlyLogic("@EntityType@=D");
                    }
                    // CLOB
                    else if (typeName == Types.CLOB)
                        column.SetAD_Reference_ID(DisplayType.TextLong);
                    // BLOB
                    else if (typeName == Types.BLOB)
                        column.SetAD_Reference_ID(DisplayType.Binary);
                    // Amount
                    else if (columnName.ToUpper().IndexOf("AMT") != -1)
                        column.SetAD_Reference_ID(DisplayType.Amount);
                    // Qty
                    else if (columnName.ToUpper().IndexOf("QTY") != -1)
                        column.SetAD_Reference_ID(DisplayType.Quantity);
                    // Boolean
                    else if ((size == 1)
                        && (columnName.ToUpper().StartsWith("IS") || (typeName == Types.CHAR)))
                        column.SetAD_Reference_ID(DisplayType.YesNo);
                    // List
                    else if ((size < 4) && (typeName == Types.CHAR))
                        column.SetAD_Reference_ID(DisplayType.List);
                    // Name, DocumentNo
                    else if (columnName.Equals("Name", StringComparison.OrdinalIgnoreCase)
                        || columnName.Equals("DocumentNo", StringComparison.OrdinalIgnoreCase))
                    {
                        column.SetAD_Reference_ID(DisplayType.String);
                        column.SetIsIdentifier(true);
                        column.SetSeqNo(1);
                    }
                    // String, Text
                    else if ((typeName == Types.CHAR) || (typeName == Types.VARCHAR)
                        || typeName.StartsWith("NVAR")
                        || typeName.StartsWith("NCHAR"))
                    {
                        if (typeName.StartsWith("N"))	//	MultiByte
                            size /= 2;
                        if (size > 255)
                            column.SetAD_Reference_ID(DisplayType.Text);
                        else
                            column.SetAD_Reference_ID(DisplayType.String);
                    }

                    // Number
                    else if ((typeName == Types.INTEGER) || (typeName == Types.SMALLINT)
                        || (typeName == Types.DECIMAL) || (typeName == Types.NUMERIC))
                    {
                        if (size == 10)
                            column.SetAD_Reference_ID(DisplayType.Integer);
                        else
                            column.SetAD_Reference_ID(DisplayType.Number);
                    }
                    //	??
                    else
                        column.SetAD_Reference_ID(DisplayType.String);

                    //	General Defaults
                    if (columnName.EndsWith("_ID"))
                        column.SetConstraintType(X_AD_Column.CONSTRAINTTYPE_Restrict);
                    if (columnName.Equals("AD_Client_ID"))
                    {
                        column.SetAD_Val_Rule_ID(116);	//	Client Login
                        column.SetDefaultValue("@#AD_Client_ID@");
                        column.SetIsUpdateable(false);                        
                    }
                    else if (columnName.Equals("AD_Org_ID"))
                    {
                        column.SetAD_Val_Rule_ID(104);	//	Org Security
                        column.SetDefaultValue("@#AD_Org_ID@");
                        column.SetIsUpdateable(false);                        
                    }
                    else if (columnName.Equals("Processed"))
                    {
                        column.SetAD_Reference_ID(DisplayType.YesNo);
                        column.SetDefaultValue("N");
                        column.SetIsUpdateable(false);
                    }
                    else if (columnName.Equals("Posted"))
                    {
                        column.SetAD_Reference_ID(DisplayType.Button);
                        column.SetAD_Reference_Value_ID(234);	//	_PostedStatus
                        column.SetDefaultValue("N");
                        column.SetIsUpdateable(false);
                    }

                    //	General
                    column.SetFieldLength(size);
                    if (column.IsUpdateable() && table.IsView())
                        column.SetIsUpdateable(false);

                    //	Done
                    if (column.Save())
                    {
                        AddLog(0, DateTime.Now, null, table.GetTableName() + "." + column.GetColumnName());
                        m_count++;
                    }
                }	//	while columns
            }
            return colName;
        }	//	addTableColumn
    }


}
