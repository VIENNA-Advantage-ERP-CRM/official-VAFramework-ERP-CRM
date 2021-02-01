using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Model;
using VAdvantage.Utility;



namespace VAdvantage.Model
{
    public class MVAFColumn : X_VAF_Column
    {
        /**	Get Element			*/
        private M_VAFColumnDic _element = null;
        /**	Cache						*/
        private static CCache<int, MVAFColumn> s_cache = new CCache<int, MVAFColumn>("VAF_Column", 20);
        private static Logging.VLogger s_log = Logging.VLogger.GetVLogger(typeof(MVAFColumn).FullName);

        public MVAFColumn(MVAFTableView parent)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetVAF_TableView_ID(parent.GetVAF_TableView_ID());
            SetEntityType(parent.GetEntityType());
        }	//	M_Column

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Column_ID">column id</param>
        /// <param name="trxName">transaction</param>
        public MVAFColumn(Ctx ctx, int VAF_Column_ID, Trx trxName)
            : base(ctx, VAF_Column_ID, trxName)
        {
            if (VAF_Column_ID == 0)
            {
                //	setVAF_ColumnDic_ID (0);
                //	setVAF_Control_Ref_ID (0);
                //	setColumnName (null);
                //	setName (null);
                //	setEntityType (null);	// U
                SetIsAlwaysUpdateable(false);	// N
                SetIsEncrypted("N");
                SetIsIdentifier(false);
                SetIsKey(false);
                SetIsMandatory(false);
                SetIsMandatoryUI(false);
                SetIsParent(false);
                SetIsSelectionColumn(false);
                SetIsTranslated(false);
                SetIsUpdateable(true);	// Y
                //SetVersion(0.0);
                SetIsSelectionColumn(false);
                SetIsSummaryColumn(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MVAFColumn(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        /// Get M_Column from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Column_ID">column id</param>
        /// <returns>MColumn</returns>
        public static MVAFColumn Get(Ctx ctx, int VAF_Column_ID)
        {
            int key = VAF_Column_ID;
            MVAFColumn retValue = null;
            retValue = s_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MVAFColumn(ctx, VAF_Column_ID, null);
            if (retValue.Get_ID() != 0)
                s_cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Get Column Name
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Column_ID">column id</param>
        /// <returns>Column Name or null</returns>
        public static string GetColumnName(Ctx ctx, int VAF_Column_ID)
        {
            MVAFColumn col = MVAFColumn.Get(ctx, VAF_Column_ID);
            if (col.Get_ID() == 0)
                return null;
            return col.GetColumnName();
        }

        /// <summary>
        ///Is Standard Column
        /// </summary>
        /// <returns>true for VAF_Client_ID, etc.</returns>
        public Boolean IsStandardColumn()
        {
            String columnName = GetColumnName();
            if (columnName.Equals("VAF_Client_ID")
                || columnName.Equals("VAF_Org_ID")
                || columnName.Equals("IsActive")
                || columnName.StartsWith("Created")
                || columnName.StartsWith("Updated"))
                return true;
            return false;
        }

        public Boolean IsVAF_Client_ID()
        {
            String columnName = GetColumnName();
            if (columnName.Equals("VAF_Client_ID", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        public Boolean IsCreatedBy()
        {
            String columnName = GetColumnName();
            if (columnName.Equals("CreatedBy", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        public Boolean IsUpdatedBy()
        {
            String columnName = GetColumnName();
            if (columnName.Equals("UpdatedBy", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }


        public Boolean IsVAF_Org_ID()
        {
            String columnName = GetColumnName();
            if (columnName.Equals("VAF_Org_ID", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Is Virtual Column
        /// </summary>
        /// <param param name="table">MVAFTableView object</param>
        /// <param name="setNullOption"></param>
        /// Created By : Kiran Sangwan
        /// <returns>string if virtual column</returns>
        public bool IsVirtualColumn()
        {
            string s = GetColumnSQL();
            return s != null && s.Length > 0;
        }

        /// <summary>
        /// Get SQL Modify command
        /// </summary>
        /// <param param name="table">MVAFTableView object</param>
        /// <param name="setNullOption"></param>
        /// Created By : Kiran Sangwan
        /// <returns>string modified sqlCommand</returns>
        public string GetSQLModify(MVAFTableView table, bool setNullOption)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlBase = new StringBuilder("ALTER TABLE ")
                .Append(table.GetTableName());

            if (DatabaseType.IsOracle)
            {
                sqlBase.Append(" MODIFY ");
            }
            else if (DatabaseType.IsPostgre)
            {
                sqlBase.Append(" ALTER COLUMN ");
            }


            ////	Default
            //StringBuilder sqlDefault = new StringBuilder(sqlBase.ToString())
            //    .Append(" ").Append(getSQLDataType())
            //    .Append(" DEFAULT ");
            //string defaultValue = GetSQLDefaultValue();
            //if (defaultValue.Length > 0)
            //    sqlDefault.Append(defaultValue);
            //else
            //{
            //    sqlDefault.Append(" NULL ");
            //    defaultValue = null;
            //}
            //sql.Append(sqlDefault);

            ////	Constraint

            ////	Null Values
            //if (IsMandatory() && defaultValue != null && defaultValue.Length > 0)
            //{
            //    StringBuilder sqlSet = new StringBuilder("UPDATE ")
            //        .Append(table.GetTableName())
            //        .Append(" SET ").Append(GetColumnName())
            //        .Append("=").Append(defaultValue)
            //        .Append(" WHERE ").Append(GetColumnName()).Append(" IS NULL");
            //    sql.Append("; ").Append(sqlSet);
            //}

            ////	Null
            //if (setNullOption)
            //{
            //    StringBuilder sqlNull = new StringBuilder(sqlBase.ToString());
            //    if (IsMandatory())
            //        sqlNull.Append(" NOT NULL");
            //    else
            //        sqlNull.Append(" NULL");
            //    sql.Append("; ").Append(sqlNull);
            //}
            //
            sql.Append(sqlBase.ToString()).Append(GetSQLModifyPerColumn(table, GetColumnName(), setNullOption));

            string defaultVal = SetDefaultValue(table, GetColumnName());
            if (!string.IsNullOrEmpty(defaultVal))
            {
                sql.Append(defaultVal);
            }



            string setNull = SetNullOption(setNullOption, table, GetColumnName());
            if (setNull.Length > 0)
            {
                sql.Append(";").Append(setNull);
            }

            string updateQL = UpdateDefaultValue(table);
            if (updateQL.Length > 0)
            {
                sql.Append(";").Append(updateQL);
            }

            return sql.ToString();
        }



        public string GetSQLModifyPerColumn(MVAFTableView table, string columnName, bool setNullOption)
        {
            //StringBuilder sql = new StringBuilder();
            //sql.Append(" " + columnName + " ");

            StringBuilder sqlBase = new StringBuilder(" " + columnName + " ");
            if (DatabaseType.IsPostgre)
            {
                sqlBase.Append(" TYPE ");
            }

            //	Default
            StringBuilder sqlDefault = new StringBuilder(sqlBase.ToString())
                .Append(" ").Append(getSQLDataType());


            //    .Append(" DEFAULT ");
            //string defaultValue = GetSQLDefaultValue();
            //if (defaultValue.Length > 0)
            //    sqlDefault.Append(defaultValue);
            //else
            //{
            //    sqlDefault.Append(" NULL ");
            //    defaultValue = null;
            //}

            //sql.Append(sqlDefault);


            //SetNullOption(setNullOption, table, columnName);


            //	Constraint
            return sqlDefault.ToString();
        }

        public string SetDefaultValue(MVAFTableView table, string columnName)
        {
            StringBuilder sqlDefault = new StringBuilder("");

            if (DatabaseType.IsOracle)
            {
                sqlDefault.Append(" DEFAULT ");
                string defaultValue = GetSQLDefaultValue();
                if (defaultValue.Length > 0)
                    sqlDefault.Append(defaultValue);
                else
                {
                    sqlDefault.Append(" NULL ");
                    defaultValue = null;
                }
            }
            else if (DatabaseType.IsPostgre)
            {
                //ALTER TABLE Test ALTER COLUMN  Description SET DEFAULT  NULL 
                sqlDefault.Append("; ALTER TABLE " + table.GetTableName())
                    .Append(" ALTER COLUMN " + columnName + " SET DEFAULT ");
                string defaultValue = GetSQLDefaultValue();
                if (defaultValue.Length > 0)
                    sqlDefault.Append(defaultValue);
                else
                {
                    sqlDefault.Append(" NULL ");
                    defaultValue = null;
                }

            }
            return sqlDefault.ToString();
        }

        public string SetNullOption(bool setNullOption, MVAFTableView table, string columnName)
        {
            if (setNullOption)
            {
                StringBuilder sqlBase1 = new StringBuilder("ALTER TABLE ")
                .Append(table.GetTableName());

                if (DatabaseType.IsOracle)
                {
                    sqlBase1.Append(" MODIFY " + columnName + " ");

                    StringBuilder sqlNull = new StringBuilder(sqlBase1.ToString());
                    if (IsMandatory())
                        sqlNull.Append(" NOT NULL");
                    else
                        sqlNull.Append(" NULL");

                    return sqlNull.ToString();
                }
                else if (DatabaseType.IsPostgre)
                {
                    //sqlBase1.Append(" ALTER " + columnName + " SET ");
                    sqlBase1.Append(" ALTER COLUMN " + columnName + " " + (IsMandatory() ? " SET " : " DROP ") + " NOT NULL");
                    return sqlBase1.ToString();
                }
            }
            return "";
        }


        public string UpdateDefaultValue(MVAFTableView table)
        {
            string defaultValue = GetDefaultValue();
            //	Null Values
            if (IsMandatory() && defaultValue != null && defaultValue.Length > 0)
            {
                StringBuilder sqlSet = new StringBuilder("UPDATE ")
                    .Append(table.GetTableName())
                    .Append(" SET ").Append(GetColumnName())
                    .Append("=").Append(defaultValue)
                    .Append(" WHERE ").Append(GetColumnName()).Append(" IS NULL");
                return sqlSet.ToString();
            }
            return "";
        }


        /// <summary>
        /// Get SQL Default Value
        /// </summary>
        /// Created By : Kiran Sangwan
        /// <returns>string default clause</returns>
        public string GetSQLDefaultValue()
        {
            string defaultValue = GetDefaultValue();
            string columnName = GetColumnName();
            int dt = GetVAF_Control_Ref_ID();
            //
            string sql = "";
            if (columnName.Equals("CreatedBy") || columnName.Equals("UpdatedBy"))
                sql = "0";
            else if (columnName.Equals("Created") || columnName.Equals("Updated"))
            {
                if (DatabaseType.IsOracle || DatabaseType.IsPostgre)
                    sql = "SYSDATE";
                else
                    sql = "CURRENT_TIMESTAMP";

            }
            else if (defaultValue != null && defaultValue.Length > 0)
            {
                defaultValue = defaultValue.Trim();
                if (defaultValue.Equals("NULL"))
                    sql = "NULL";
                else if (defaultValue.StartsWith("@SQL="))
                {
                    s_log.Warning("Get sql default: " + defaultValue);	//	warning & ignore default
                }
                else
                {
                    int first = defaultValue.IndexOf("@");
                    if (first >= 0 && defaultValue.IndexOf("@", first + 1) >= 0)
                    {
                        s_log.Finer("Get default: (Unresolved Variable) " + defaultValue);
                    }
                    else if (defaultValue.Equals("-1"))
                    {
                        s_log.Finer("Get default: (invalid value) " + defaultValue);
                    }
                    else
                    {
                        if (DisplayType.IsText(dt)
                            || DisplayType.List == dt
                            || DisplayType.YesNo == dt
                            || (DisplayType.Button == dt && !columnName.EndsWith("_ID"))
                            || columnName.Equals("EntityType")
                            || columnName.Equals("VAF_Language")
                            || columnName.Equals("DocBaseType")
                            )
                        {
                            //sql = DatabaseType.TO_STRING(defaultValue);
                            sql = GlobalVariable.TO_STRING(defaultValue);
                        }
                        else
                        {
                            sql = defaultValue;
                        }
                    }
                }
            }
            else if (columnName.Equals("IsActive"))
                sql = "'Y'";
            //	NO default value - set Data Type
            else
            {
                if (dt == DisplayType.YesNo)
                {
                    sql = "'N'";
                }
                else if (DisplayType.IsNumeric(dt)
                    && (IsMandatory() || IsMandatoryUI()))
                {
                    sql = "0";
                }
            }
            return sql;
        }

        /// <summary>
        /// Get SQL Data Type
        /// </summary>
        /// Created By : Kiran Sangwan
        /// <returns>string sql data type</returns>
        public string getSQLDataType()
        {
            string columnName = GetColumnName();
            int dt = GetVAF_Control_Ref_ID();
            return DisplayType.GetSQLDataType(dt, columnName, GetFieldLength());
        }

        /// <summary>
        /// Get SQL Add command
        /// </summary>
        /// Created By : Kiran Sangwan
        /// <returns>string sql add command</returns>
        public string GetSQLAdd(MVAFTableView table)
        {
            if (IsVirtualColumn())
                return null;
            StringBuilder sql = new StringBuilder("ALTER TABLE ")
                .Append(table.GetTableName())
                .Append(" ADD ").Append(GetSQLDDL());
            return sql.ToString();
        }

        /// <summary>
        /// Get SQL DDL
        /// </summary>
        /// Created By : Kiran Sangwan
        /// <returns>string columnName datataype .. or null if virtual column</returns>
        public string GetSQLDDL()
        {
            if (IsVirtualColumn())
                return null;
            //
            string columnName = GetColumnName();
            StringBuilder sql = new StringBuilder(columnName)
                .Append(" ").Append(getSQLDataType());
            string defaultValue = GetSQLDefaultValue();
            if (defaultValue != null && defaultValue.Length > 0)
            {
                //if (defaultValue == "Y" || defaultValue == "N")
                //   sql.Append(" DEFAULT ").Append("'" + defaultValue + "'");
                //else
                sql.Append(" DEFAULT ").Append(defaultValue);
            }
            //	Inline Constraint only for oracle now
            if (DatabaseType.IsOracle)
            {
                if (GetVAF_Control_Ref_ID() == DisplayType.YesNo)
                {
                    sql.Append(" CHECK (").Append(columnName).Append(" IN ('Y','N'))");
                }
            }

            //	Null
            if (IsMandatory())
            {
                sql.Append(" NOT NULL");
            }
            return sql.ToString();
        }

        /// <summary>
        /// Set Encrypted
        /// </summary>
        /// <param name="isEncrypted">encrypted</param>
        public void SetIsEncrypted(bool isEncrypted)
        {
            SetIsEncrypted(isEncrypted ? "Y" : "N");
        }

        /// <summary>
        /// Is the Column Encrypted?
        /// </summary>
        /// <returns>true if encrypted</returns>
        public bool IsEncrypted()
        {
            String s = GetIsEncrypted();
            return "Y".Equals(s);
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            //	Update Fields
            if (!newRecord)
            {
                StringBuilder sql = new StringBuilder("UPDATE VAF_Field SET Name=")
                    .Append(DataBase.DB.TO_STRING(GetName()))
                    .Append(", Description=").Append(DataBase.DB.TO_STRING(GetDescription()))
                    .Append(", Help=").Append(DataBase.DB.TO_STRING(GetHelp()))
                    .Append(" WHERE VAF_Column_ID=").Append(Get_ID())
                    .Append(" AND IsCentrallyMaintained='Y'");

                int no = CoreLibrary.DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());

                //int no = DataBase.executeUpdate(sql.ToString(), Get_TrxName());
                log.Fine("afterSave - Fields updated #" + no);
            }
            return success;
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns></returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (!CheckVersions(false))
                return false;

            int displayType = GetVAF_Control_Ref_ID();
            //	Length
            if (DisplayType.IsLOB(displayType))	//	LOBs are 0
            {
                if (GetFieldLength() != 0)
                    SetFieldLength(0);
            }
            else if (GetFieldLength() == 0)
            {
                if (DisplayType.IsID(displayType))
                    SetFieldLength(10);
                else if (DisplayType.IsNumeric(displayType))
                    SetFieldLength(14);
                else if (DisplayType.IsDate(displayType))
                    SetFieldLength(7);
                else if (DisplayType.YesNo == displayType)
                    SetFieldLength(1);
                else
                {
                    log.SaveError("FillMandatory", Utility.Msg.GetElement(GetCtx(), "FieldLength"));
                    return false;
                }
            }

            /** Views are not updateable
            UPDATE VAF_Column c
            SET IsUpdateable='N', IsAlwaysUpdateable='N'
            WHERE VAF_TableView_ID IN (SELECT VAF_TableView_ID FROM VAF_TableView WHERE IsView='Y')
            **/

            //	Virtual Column
            if (IsVirtualColumn())
            {
                if (IsMandatory())
                    SetIsMandatory(false);
                if (IsMandatoryUI())
                    SetIsMandatoryUI(false);
                if (IsUpdateable())
                    SetIsUpdateable(false);
            }
            //	Updateable/Mandatory
            if (IsParent() || IsKey())
            {
                SetIsUpdateable(false);
                SetIsMandatory(true);
            }
            if (IsAlwaysUpdateable() && !IsUpdateable())
                SetIsAlwaysUpdateable(false);
            //	Encrypted
            if (IsEncrypted())
            {
                int dt = GetVAF_Control_Ref_ID();
                if (IsKey() || IsParent() || IsStandardColumn()
                    || IsVirtualColumn() || IsIdentifier() || IsTranslated()
                    || DisplayType.IsLookup(dt) || DisplayType.IsLOB(dt)
                    || "DocumentNo".ToLower().Equals(GetColumnName().ToLower())
                    || "Value".ToLower().Equals(GetColumnName().ToLower())
                    || "Name".ToLower().Equals(GetColumnName().ToLower()))
                {
                    log.Warning("Encryption not sensible - " + GetColumnName());
                    SetIsEncrypted(false);
                }
            }

            //	Sync Terminology
            if ((newRecord || Is_ValueChanged("VAF_ColumnDic_ID"))
                && GetVAF_ColumnDic_ID() != 0)
            {
                _element = new M_VAFColumnDic(GetCtx(), GetVAF_ColumnDic_ID(), Get_TrxName());
                SetColumnName(_element.GetColumnName());
                SetName(_element.GetName());
                SetDescription(_element.GetDescription());
                SetHelp(_element.GetHelp());
            }

            if (IsKey() && (newRecord || Is_ValueChanged("IsKey")))
                SetConstraintType(null);

            return true;
        }

        /// <summary>
        /// Get Element
        /// </summary>
        /// <returns>element</returns>
        public M_VAFColumnDic GetElement()
        {
            if (_element == null || _element.GetVAF_ColumnDic_ID() != GetVAF_ColumnDic_ID())
                _element = new M_VAFColumnDic(GetCtx(), GetVAF_ColumnDic_ID(), Get_TrxName());
            return _element;
        }

        /// <summary>
        /// Get FK Name
        /// </summary>
        /// <returns>foreign key column name</returns>
        public String GetFKColumnName()
        {
            String keyColumnName = GetColumnName();
            int displayType = GetVAF_Control_Ref_ID();
            if (displayType == DisplayType.List)
                return "Value";
            if (displayType == DisplayType.Account)
                return "VAB_Acct_ValidParameter_ID";
            //
            if (displayType == DisplayType.Table)
            {
                int VAF_Control_Ref_ID = GetVAF_Control_Ref_Value_ID();
                MVAFCtrlRefTable rt = MVAFCtrlRefTable.Get(GetCtx(), VAF_Control_Ref_ID);
                return rt.GetKeyColumnName();
            }
            return keyColumnName;
        }

        /// <summary>
        /// Get FK Table - also for lists
        /// </summary>
        /// <returns>FK Table</returns>
        public MVAFTableView GetFKTable()
        {
            if (!IsFK())
                return null;

            int displayType = GetVAF_Control_Ref_ID();
            //	Lists
            if (displayType == DisplayType.List)
                return MVAFTableView.Get(GetCtx(), MVAFCtrlRefList.Table_ID);
            //	Account
            //if (displayType == DisplayType.Account)
            //    return MVAFTableView.Get(GetCtx(), MAccount.Table_ID);	//	VAB_Acct_ValidParameter

            //	Table, TableDir, ...
            String FKTableName = GetFKColumnName();
            if (FKTableName.EndsWith("_ID"))
                FKTableName = FKTableName.Substring(0, FKTableName.Length - 3);

            return MVAFTableView.Get(GetCtx(), FKTableName);
        }

        /// <summary>
        /// Get Label Name
        /// </summary>
        /// <param name="VAF_Language">language</param>
        /// <returns>name</returns>
        public String GetName(String VAF_Language)
        {
            return Get_Translation(GetColumnName(), VAF_Language);
        }

        /// <summary>
        /// Column has FK (lists not included)
        /// </summary>
        /// <returns>true if has FK</returns>
        public bool IsFK()
        {
            int dt = GetVAF_Control_Ref_ID();
            if (DisplayType.IsID(dt)
                && dt != DisplayType.ID
                && !IsKey()
                && !IsVirtualColumn())
                return true;
            if (dt == DisplayType.Button && GetColumnName().EndsWith("_ID"))
                return true;
            return false;
        }

        /// <summary>
        /// Column is a List
        /// </summary>
        /// <returns>true if volumn is a list</returns>
        public bool IsList()
        {
            int dt = GetVAF_Control_Ref_ID();
            return dt == DisplayType.List;
        }


        /// <summary>
        /// Column is a String
        /// </summary>
        /// <returns>true if volumn is a list</returns>
        public bool IsStringOrText()
        {
            int dt = GetVAF_Control_Ref_ID();
            if (dt == DisplayType.String || dt == DisplayType.TextLong || dt == DisplayType.Text)
                return true;
            return false;
            //return dt == (DisplayType.String || DisplayType.Text || DisplayType.TextLong);
        }

        /// <summary>
        /// Column is a Date
        /// </summary>
        /// <returns>true if volumn is a list</returns>
        public bool IsDate()
        {
            int dt = GetVAF_Control_Ref_ID();
            return dt == (DisplayType.Date);
        }

        /// <summary>
        /// Column is a DateTime
        /// </summary>
        /// <returns>true if volumn is a list</returns>
        public bool IsDateTime()
        {
            int dt = GetVAF_Control_Ref_ID();
            if (dt == DisplayType.DateTime || dt == DisplayType.Date)
                return true;
            return false;
        }

        /// <summary>
        /// Column is a Boolena (yes-no)
        /// </summary>
        /// <returns>true if volumn is a list</returns>
        public bool IsYesNo()
        {
            int dt = GetVAF_Control_Ref_ID();
            return dt == DisplayType.YesNo;
        }

        /// <summary>
        /// Selection Column
        /// </summary>
        /// <returns>true if Selection Column</returns>
        public new bool IsSelectionColumn()
        {
            if (base.IsSelectionColumn())
                return true;
            String cn = GetColumnName();
            return "Value".Equals(cn)
                || "Name".Equals(cn)
                || "Description".Equals(cn)
                || "DocumentNo".Equals(cn);
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MColumn[");
            sb.Append(Get_ID()).Append("-").Append(GetColumnName()).Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool BeforeDelete()
        {
            if (!CheckVersions(true))
                return false;
            return true;
        }

        /// <summary>
        /// function to check if user try to either delete or remove 
        /// last column which is marked as Maintain Version to false
        /// then return false, if there is data in respective Version Table
        /// </summary>
        /// <param name="delete"></param>
        /// <returns>bool (true/false)</returns>
        public bool CheckVersions(bool delete)
        {
            bool check = true;
            StringBuilder sb = new StringBuilder("SELECT COUNT(VAF_Column_ID) FROM VAF_Column WHERE VAF_TableView_ID = " + GetVAF_TableView_ID() + " AND IsMaintainVersions = 'Y' AND VAF_Column_ID != " + GetVAF_Column_ID());
            if (!delete)
            {
                if (!(Is_ValueChanged("IsMaintainVersions") && (Util.GetValueOfBool(Get_ValueOld("IsMaintainVersions")) && !Util.GetValueOfBool(Get_Value("IsMaintainVersions")))))
                    check = false;
            }
            else
            {
                if (!(Util.GetValueOfBool(Get_ValueOld("IsMaintainVersions"))))
                    check = false;
            }
            if (check)
            {
                int countVerCols = Util.GetValueOfInt(DB.ExecuteScalar(sb.ToString(), null, Get_Trx()));
                if (countVerCols == 0)
                {
                    sb.Clear();
                    sb.Append("SELECT TableName FROM VAF_TableView WHERE VAF_TableView_ID = " + GetVAF_TableView_ID());
                    string tableName = Util.GetValueOfString(DB.ExecuteScalar(sb.ToString(), null, Get_Trx()));
                    sb.Clear();

                    DatabaseMetaData md = new DatabaseMetaData();

                    //get columns of a table
                    DataSet dt = md.GetColumns("", CoreLibrary.DataBase.DB.GetSchema(), tableName + "_Ver");
                    md.Dispose();

                    // check whether version table exists in database
                    if (dt != null && dt.Tables[0] != null && dt.Tables[0].Rows.Count > 0)
                    {
                        MVAFTableView tblVer = MVAFTableView.Get(GetCtx(), tableName + "_Ver");
                        if (tblVer.HasVersionData(tableName + "_Ver"))
                        {
                            // if Maintain Version is marked on Table Level then allow to remove Maintain Version from Column
                            if (Util.GetValueOfString(DB.ExecuteScalar("SELECT IsMaintainVersions FROM VAF_TableView WHERE VAF_TableView_ID = " + GetVAF_TableView_ID(), null, Get_Trx())) == "Y")
                                return true;
                            else
                            {
                                log.SaveError("VersionDataExists", Utility.Msg.GetElement(GetCtx(), "VersionDataExists"));
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}
