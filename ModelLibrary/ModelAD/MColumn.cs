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

using VAdvantage.ProcessEngine;

namespace VAdvantage.Model
{
    public class MColumn : X_AD_Column
    {
        /**	Get Element			*/
        private M_Element _element = null;
        /**	Cache						*/
        private static CCache<int, MColumn> s_cache = new CCache<int, MColumn>("AD_Column", 20);
        private static Logging.VLogger s_log	= Logging.VLogger.GetVLogger(typeof(MColumn).FullName);

        public MColumn(MTable parent)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetAD_Table_ID(parent.GetAD_Table_ID());
            SetEntityType(parent.GetEntityType());
        }	//	M_Column

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Column_ID">column id</param>
        /// <param name="trxName">transaction</param>
        public MColumn(Ctx ctx, int AD_Column_ID, Trx trxName)
            : base(ctx, AD_Column_ID, trxName)
        {
            if (AD_Column_ID == 0)
            {
                //	setAD_Element_ID (0);
                //	setAD_Reference_ID (0);
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
        public MColumn(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        /// Get M_Column from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Column_ID">column id</param>
        /// <returns>MColumn</returns>
        public static MColumn Get(Ctx ctx, int AD_Column_ID)
        {
            int key = AD_Column_ID;
            MColumn retValue = null;
            retValue = s_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MColumn(ctx, AD_Column_ID, null);
            if (retValue.Get_ID() != 0)
                s_cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Get Column Name
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Column_ID">column id</param>
        /// <returns>Column Name or null</returns>
        public static string GetColumnName(Ctx ctx, int AD_Column_ID)
        {
            MColumn col = MColumn.Get(ctx, AD_Column_ID);
            if (col.Get_ID() == 0)
                return null;
            return col.GetColumnName();
        }
        
        /// <summary>
        ///Is Standard Column
        /// </summary>
        /// <returns>true for AD_Client_ID, etc.</returns>
        public Boolean IsStandardColumn()
        {
            String columnName = GetColumnName();
            if (columnName.Equals("AD_Client_ID")
                || columnName.Equals("AD_Org_ID")
                || columnName.Equals("IsActive")
                || columnName.StartsWith("Created")
                || columnName.StartsWith("Updated"))
                return true;
            return false;
        }

        public Boolean IsAD_Client_ID()
        {
            String columnName = GetColumnName();
            if (columnName.Equals("AD_Client_ID", StringComparison.OrdinalIgnoreCase))
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


        public Boolean IsAD_Org_ID()
        {
            String columnName = GetColumnName();
            if (columnName.Equals("AD_Org_ID", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }
 
        /// <summary>
        /// Is Virtual Column
        /// </summary>
        /// <param param name="table">MTable object</param>
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
        /// <param param name="table">MTable object</param>
        /// <param name="setNullOption"></param>
        /// Created By : Kiran Sangwan
        /// <returns>string modified sqlCommand</returns>
        public string GetSQLModify(MTable table, bool setNullOption)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlBase = new StringBuilder("ALTER TABLE ")
                .Append(table.GetTableName())
                .Append(" MODIFY ").Append(GetColumnName());

            //	Default
            StringBuilder sqlDefault = new StringBuilder(sqlBase.ToString())
                .Append(" ").Append(getSQLDataType())
                .Append(" DEFAULT ");
            string defaultValue = GetSQLDefaultValue();
            if (defaultValue.Length > 0)
                sqlDefault.Append(defaultValue);
            else
            {
                sqlDefault.Append(" NULL ");
                defaultValue = null;
            }
            sql.Append(sqlDefault);

            //	Constraint

            //	Null Values
            if (IsMandatory() && defaultValue != null && defaultValue.Length > 0)
            {
                StringBuilder sqlSet = new StringBuilder("UPDATE ")
                    .Append(table.GetTableName())
                    .Append(" SET ").Append(GetColumnName())
                    .Append("=").Append(defaultValue)
                    .Append(" WHERE ").Append(GetColumnName()).Append(" IS NULL");
                sql.Append("; ").Append(sqlSet);
            }

            //	Null
            if (setNullOption)
            {
                StringBuilder sqlNull = new StringBuilder(sqlBase.ToString());
                if (IsMandatory())
                    sqlNull.Append(" NOT NULL");
                else
                    sqlNull.Append(" NULL");
                sql.Append("; ").Append(sqlNull);
            }
            //
            return sql.ToString();
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
            int dt = GetAD_Reference_ID();
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
                            || columnName.Equals("AD_Language")
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
            int dt = GetAD_Reference_ID();
            return DisplayType.GetSQLDataType(dt, columnName, GetFieldLength());
        }

        /// <summary>
        /// Get SQL Add command
        /// </summary>
        /// Created By : Kiran Sangwan
        /// <returns>string sql add command</returns>
        public string GetSQLAdd(MTable table)
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
                    sql.Append(" DEFAULT ").Append( defaultValue );
            }
            //	Inline Constraint only for oracle now
            if (DatabaseType.IsOracle)
            {
                if (GetAD_Reference_ID() == DisplayType.YesNo)
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
                StringBuilder sql = new StringBuilder("UPDATE AD_Field SET Name=")
                    .Append(DataBase.DB.TO_STRING(GetName()))
                    .Append(", Description=").Append(DataBase.DB.TO_STRING(GetDescription()))
                    .Append(", Help=").Append(DataBase.DB.TO_STRING(GetHelp()))
                    .Append(" WHERE AD_Column_ID=").Append(Get_ID())
                    .Append(" AND IsCentrallyMaintained='Y'");

                int no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());

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
            int displayType = GetAD_Reference_ID();
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
            UPDATE AD_Column c
            SET IsUpdateable='N', IsAlwaysUpdateable='N'
            WHERE AD_Table_ID IN (SELECT AD_Table_ID FROM AD_Table WHERE IsView='Y')
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
                int dt = GetAD_Reference_ID();
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
            if ((newRecord || Is_ValueChanged("AD_Element_ID"))
                && GetAD_Element_ID() != 0)
            {
                _element = new M_Element(GetCtx(), GetAD_Element_ID(), Get_TrxName());
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
        public M_Element GetElement()
        {
            if (_element == null || _element.GetAD_Element_ID() != GetAD_Element_ID())
                _element = new M_Element(GetCtx(), GetAD_Element_ID(), Get_TrxName());
            return _element;
        }

        /// <summary>
        /// Get FK Name
        /// </summary>
        /// <returns>foreign key column name</returns>
        public String GetFKColumnName()
        {
            String keyColumnName = GetColumnName();
            int displayType = GetAD_Reference_ID();
            if (displayType == DisplayType.List)
                return "Value";
            if (displayType == DisplayType.Account)
                return "C_ValidCombination_ID";
            //
            if (displayType == DisplayType.Table)
            {
                int AD_Reference_ID = GetAD_Reference_Value_ID();
                MRefTable rt = MRefTable.Get(GetCtx(), AD_Reference_ID);
                return rt.GetKeyColumnName();
            }
            return keyColumnName;
        }

        /// <summary>
        /// Get FK Table - also for lists
        /// </summary>
        /// <returns>FK Table</returns>
        public MTable GetFKTable()
        {
            if (!IsFK())
                return null;

            int displayType = GetAD_Reference_ID();
            //	Lists
            if (displayType == DisplayType.List)
                return MTable.Get(GetCtx(), MRefList.Table_ID);
            //	Account
            //if (displayType == DisplayType.Account)
            //    return MTable.Get(GetCtx(), MAccount.Table_ID);	//	C_ValidCombination

            //	Table, TableDir, ...
            String FKTableName = GetFKColumnName();
            if (FKTableName.EndsWith("_ID"))
                FKTableName = FKTableName.Substring(0, FKTableName.Length - 3);

            return MTable.Get(GetCtx(), FKTableName);
        }

        /// <summary>
        /// Get Label Name
        /// </summary>
        /// <param name="AD_Language">language</param>
        /// <returns>name</returns>
        public String GetName(String AD_Language)
        {
            return Get_Translation(GetColumnName(), AD_Language);
        }

        /// <summary>
        /// Column has FK (lists not included)
        /// </summary>
        /// <returns>true if has FK</returns>
        public bool IsFK()
        {
            int dt = GetAD_Reference_ID();
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
            int dt = GetAD_Reference_ID();
            return dt == DisplayType.List;
        }


        /// <summary>
        /// Column is a String
        /// </summary>
        /// <returns>true if volumn is a list</returns>
        public bool IsStringOrText()
        {
            int dt = GetAD_Reference_ID();
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
            int dt = GetAD_Reference_ID();
            return dt == (DisplayType.Date);
        }

        /// <summary>
        /// Column is a DateTime
        /// </summary>
        /// <returns>true if volumn is a list</returns>
        public bool IsDateTime()
        {
            int dt = GetAD_Reference_ID();
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
            int dt = GetAD_Reference_ID();
            return dt == DisplayType.YesNo;
        }

        /// <summary>
        /// Selection Column
        /// </summary>
        /// <returns>true if Selection Column</returns>
        public new  bool IsSelectionColumn()
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


    }
}
