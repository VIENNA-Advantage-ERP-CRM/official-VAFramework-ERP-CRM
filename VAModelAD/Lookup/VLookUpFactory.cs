/********************************************************
// Module Name    : run time show window 
// Purpose        : create the dymanmic sql query for loookup Fields
// Class Used     : GlobalVariable.cs, CommonFunctions.cs,Ctx.cs
// Created By     : Harwinder 
// Date           : -----   
**********************************************************/



using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using VAdvantage.Login;
using VAdvantage.DataBase;
using System.Data.SqlClient;
using VAdvantage.Model;

using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Classes
{
    /// <summary>
    /// create Lookup field
    /// </summary>
    public class VLookUpFactory
    {
        /*Table IsTranslated Cache */
        // private static CCache<string, bool> _sIsTranslated = new CCache<string, bool>("VAF_TableView_isTranslated", 10);
        /** Table Reference Cache				*/
        // private static CCache<string, VLookUpInfo> _sCacheRefTable = new CCache<string, VLookUpInfo>("VAF_CtrlRef_Table", 30, 60);	//	1h
        /**	Logging								*/
        private static VLogger s_log = VLogger.GetVLogger(typeof(VLookUpInfo).FullName);

        /// <summary>
        /// Create MLookup
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="windowNo">window no</param>
        /// <param name="Column_ID">column id</param>
        /// <param name="VAF_Control_Ref_ID">reference id</param>
        /// <returns>MLookup</returns>
        public static MLookup Get(Ctx ctx, int windowNo, int Column_ID, int VAF_Control_Ref_ID)
        {
            String columnName = "";
            int VAF_Control_Ref_Value_ID = 0;
            bool isParent = false;
            String validationCode = "";
            //
            String sql = "SELECT c.ColumnName, c.VAF_Control_Ref_Value_ID, c.IsParent, vr.Code "
                + "FROM VAF_Column c"
                + " LEFT OUTER JOIN VAF_DataVal_Rule vr ON (c.VAF_DataVal_Rule_ID=vr.VAF_DataVal_Rule_ID) "
                + "WHERE c.VAF_Column_ID=" + Column_ID;
            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql);
                if (dr.Read())
                {
                    columnName = dr[0].ToString();
                    VAF_Control_Ref_Value_ID = Utility.Util.GetValueOfInt(dr[1]);
                    isParent = "Y".Equals(dr[2].ToString());
                    validationCode = dr[3].ToString();
                }
                else
                {
                    s_log.Log(Level.SEVERE, "Column Not Found - VAF_Column_ID=" + Column_ID);
                }
                dr.Close();
                dr = null;

            }
            catch (System.Exception ex)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                s_log.Log(Level.SEVERE, sql, ex);
            }
            //
            MLookup lookup = new MLookup(ctx, windowNo, VAF_Control_Ref_ID);
            VLookUpInfo info = GetLookUpInfo(lookup, Column_ID,
                Env.GetLanguage(ctx), columnName, VAF_Control_Ref_Value_ID, isParent, validationCode);
            //VLookUpInfo info = GetLookUpInfo(lookup.GetCtx(), lookup.GetWindowNo(), lookup.GetDisplayType(),
            //  Column_ID,Env.GetLanguage(ctx), columnName, VAF_Control_Ref_Value_ID, isParent, validationCode);
            if (info == null)
                throw new ArgumentException("MLookup.create - no LookupInfo");
            return lookup.Initialize(info);
        }

        /// <summary>
        /// Create MLookup
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="WindowNo">window number</param>
        /// <param name="Column_ID">key id of column</param>
        /// <param name="VAF_Control_Ref_ID">displaytype id </param>
        /// <param name="ColumnName">name of column</param>
        /// <param name="VAF_Control_Ref_Value_ID"> </param>
        /// <param name="IsParent">is parent column</param>
        /// <param name="ValidationCode">validate text</param>
        /// <returns>MLookup object</returns>
        public static MLookup Get(Ctx ctx, int windowNo, int Column_ID, int VAF_Control_Ref_ID,
                 String columnName, int VAF_Control_Ref_Value_ID,
                bool IsParent, String ValidationCode)
        {
            MLookup lookup = new MLookup(ctx, windowNo, VAF_Control_Ref_ID);
            VLookUpInfo info = GetLookUpInfo(lookup, Column_ID,
                Env.GetLanguage(ctx), columnName, VAF_Control_Ref_Value_ID, IsParent, ValidationCode);
            if (info == null)
                throw new ArgumentException("MLookup.create - no LookupInfo");
            return lookup.Initialize(info);
        }

        /// <summary>
        /// Get Information for Lookups based on Column_ID for Table Columns or Process Parameters.
        ///
        ///	The SQL returns three columns:
        /// <pre>
        ///Key, Value, Name, IsActive	(where either key or value is null)
        /// </pre>
        /// </summary>
        /// <param name="lookup">ctx context for access</param>
        /// <param name="Column_ID">VAF_Column_ID or VAF_Job_Para_ID</param>
        /// <param name="language">report lang</param>
        /// <param name="ColumnName">key column name</param>
        /// <param name="VAF_Control_Ref_Value_ID"></param>
        /// <param name="IsParent">parent (prevents query to directly access value)</param>
        /// <param name="ValidationCode">ValidationCode optional SQL validation</param>
        /// <returns>lookup info structure</returns>
        public static VLookUpInfo GetLookUpInfo(Lookup lookup,
            int Column_ID, Language language, String columnName, int VAF_Control_Ref_Value_ID,
            bool IsParent, String ValidationCode)
        {
            return GetLookUpInfo(lookup.GetCtx(), lookup.GetWindowNo(), lookup.GetDisplayType(),
                Column_ID, language, columnName, VAF_Control_Ref_Value_ID,
                IsParent, ValidationCode);
        }

        /// <summary>
        /// Get Information for Lookups based on Column_ID for Table Columns or Process Parameters.
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Control_Ref_ID">ref id</param>
        /// <param name="Column_ID">col id</param>
        /// <param name="ColumnName">name of col</param>
        /// <param name="VAF_Control_Ref_Value_ID">ref_val id</param>
        /// <param name="IsParent">is parent col</param>
        /// <param name="ValidationCode">validate text</param>
        /// <returns></returns>
        public static VLookUpInfo GetLookUpInfo(Ctx ctx, int windowNum, int VAF_Control_Ref_ID,
            int Column_ID, Language language, string columnName, int VAF_Control_Ref_Value_ID, bool IsParent, string ValidationCode)
        {
            VLookUpInfo info = null;
            bool needToAddSecurity = true;
            //	List
            if (VAF_Control_Ref_ID == DisplayType.List)	//	17
            {
                info = GetLookUp_List(language, VAF_Control_Ref_Value_ID);
                needToAddSecurity = false;
            }
            // TAble OR Search with Reference value
            else if ((VAF_Control_Ref_ID == DisplayType.Table || VAF_Control_Ref_ID == DisplayType.Search || DisplayType.MultiKey == VAF_Control_Ref_ID)
            && VAF_Control_Ref_Value_ID != 0)
            {
                info = GetLookup_Table(ctx, language, windowNum, VAF_Control_Ref_Value_ID);
            }
            //	Acct
            else if (VAF_Control_Ref_ID == DisplayType.Account)
            {
                info = GetLookup_Acct(ctx, Column_ID);
            }
            else if (VAF_Control_Ref_ID == DisplayType.ProductContainer)
            {
                info = GetLookup_PContainer(ctx, Column_ID);
            }
            //	TableDir, Search, ID, ...
            else
            {
                info = GetLookUp_TableDir(ctx, language, windowNum, columnName);
            }

            if (info == null)
            {
                s_log.Severe("No SQL - " + columnName);
                return null;
            }
            //	remaining values
            info.column_ID = Column_ID;
            info.VAF_Control_Ref_Value_ID = VAF_Control_Ref_Value_ID;
            info.isParent = IsParent;
            info.validationCode = ValidationCode;

            //variable in sql where 
            if (info.query.IndexOf("@") != -1)
            {
                string newSQL = Utility.Env.ParseContext(ctx, windowNum, info.query, false);	//	only global
                if (newSQL.Length == 0)
                {
                    s_log.Severe("SQL parse error: " + info.query);
                    return null;
                }
                info.query = newSQL;



                s_log.Fine("SQL =" + newSQL); //jz
            }

            //	Direct Query - NO Validation/Security
            int posOrder = info.query.LastIndexOf(" ORDER BY ");
            bool hasWhere = info.query.LastIndexOf(" WHERE ") != -1;
            if (hasWhere)	//	might be for a select sub-query
            {
                //	SELECT (SELECT .. FROM .. WHERE ..) FROM ..
                //	SELECT .. FROM .. WHERE EXISTS (SELECT .. FROM .. WHERE ..)
                AccessSqlParser asp = new AccessSqlParser(info.query);
                string mainQuery = asp.GetMainSql();
                hasWhere = mainQuery.IndexOf(" WHERE ") != -1;
            }


            if (posOrder == -1)
            {
                info.queryDirect = info.query
                    + (hasWhere ? " AND " : " WHERE ") + info.keyColumn + "=@key";
                info.queryAll = VAdvantage.Model.MRole.GetDefault(ctx, false).AddAccessSQL(info.query,
                   info.tableName, VAdvantage.Model.MRole.SQL_FULLYQUALIFIED, VAdvantage.Model.MRole.SQL_RO);
                // }
            }
            else
            {
                info.queryDirect = info.query.Substring(0, posOrder);
                info.queryAll = VAdvantage.Model.MRole.GetDefault(ctx, false).AddAccessSQL(info.queryDirect,
                   info.tableName, VAdvantage.Model.MRole.SQL_FULLYQUALIFIED, VAdvantage.Model.MRole.SQL_RO);

                info.queryDirect += (hasWhere ? " AND " : " WHERE ") + info.keyColumn + "=@key";
            }
            //	Validation
            string local_validationCode = "";
            if (info.validationCode == null || info.validationCode.Length == 0) { info.isValidated = true; }
            else
            {
                local_validationCode = Utility.Env.ParseContext(ctx, windowNum, info.validationCode, true);
                //  returns "" if not all variables were parsed
                if (local_validationCode.Length == 0
                    || info.validationCode.IndexOf("@VAF_Org_ID@") != -1)	//	don't validate Org
                {
                    info.isValidated = false;
                    local_validationCode = "";
                }
                else
                    info.isValidated = true;
            }

            //	Add Local Validation
            if (local_validationCode.Length != 0)
            {
                //jz handle no posOrder case
                if (posOrder > 0)
                    info.query = info.query.Substring(0, posOrder)
                        + (hasWhere ? " AND " : " WHERE ") + local_validationCode
                        + info.query.Substring(posOrder);
                else
                    info.query = info.query
                    + (hasWhere ? " AND " : " WHERE ") + local_validationCode;

                info.queryAll = VAdvantage.Model.MRole.GetDefault(ctx, false).AddAccessSQL(info.query,
                   info.tableName, VAdvantage.Model.MRole.SQL_FULLYQUALIFIED, VAdvantage.Model.MRole.SQL_RO);
                // }
            }


            // if (info.isValidated)
            // {

            // else
            //  {

            // }


            //	Add Security
            if (needToAddSecurity)
                info.query = VAdvantage.Model.MRole.GetDefault(ctx, false).AddAccessSQL(info.query,
                    info.tableName, VAdvantage.Model.MRole.SQL_FULLYQUALIFIED, VAdvantage.Model.MRole.SQL_RO);

            return info;
        }

        /// <summary>
        ///Get Lookup SQL for Lists
        /// </summary>
        /// <param name="VAF_Control_Ref_Value_ID">ref_val id</param>
        /// <returns></returns>
        public static VLookUpInfo GetLookUp_List(Language language, int VAF_Control_Ref_Value_ID)
        {
            StringBuilder realSQL = new StringBuilder("SELECT NULL, VAF_CtrlRef_List.Value,");
            String displayCol = "VAF_CtrlRef_List.Name";
            if (Utility.Env.IsBaseLanguage(language, "VAF_CtrlRef_List"))
            {
                realSQL.Append(displayCol + ", VAF_CtrlRef_List.IsActive FROM VAF_CtrlRef_List");
            }
            else
            {
                displayCol = "trl.Name";
                realSQL.Append(displayCol + ", VAF_CtrlRef_List.IsActive "
                    + "FROM VAF_CtrlRef_List INNER JOIN VAF_CtrlRef_TL trl "
                    + " ON (VAF_CtrlRef_List.VAF_CtrlRef_List_ID=trl.VAF_CtrlRef_List_ID AND trl.VAF_Language='")
                        .Append(language.GetVAF_Language()).Append("')");
            }
            realSQL.Append(" WHERE VAF_CtrlRef_List.VAF_Control_Ref_ID=").Append(VAF_Control_Ref_Value_ID);
            realSQL.Append(" ORDER BY 2");
            //
            VLookUpInfo lookupInfo = new VLookUpInfo(realSQL.ToString(), "VAF_CtrlRef_List", "VAF_CtrlRef_List.Value",
                101, 101, Query.GetEqualQuery("VAF_Control_Ref_ID", VAF_Control_Ref_Value_ID));	//	Zoom Window+Query
            // add extra property for display Column value
            lookupInfo.displayColSubQ = displayCol;
            return lookupInfo;
        }

        /// <summary>
        ///Get Lookup SQL for List
        /// </summary>
        /// <param name="language"></param>
        /// <param name="VAF_Control_Ref_Value_ID"></param>
        /// <param name="linkColumnName"></param>
        /// <returns>SELECT Name FROM VAF_CtrlRef_List WHERE VAF_Control_Ref_ID=x AND Value=linkColumn</returns>
        public static String GetLookup_ListEmbed(Language language,
            int VAF_Control_Ref_Value_ID, String linkColumnName)
        {
            StringBuilder realSQL = new StringBuilder("SELECT ");
            if (Utility.Env.IsBaseLanguage(language, "VAF_CtrlRef_List"))
                realSQL.Append("VAF_CtrlRef_List.Name FROM VAF_CtrlRef_List");
            else
                realSQL.Append("trl.Name "
                    + "FROM VAF_CtrlRef_List INNER JOIN VAF_CtrlRef_TL trl "
                    + " ON (VAF_CtrlRef_List.VAF_CtrlRef_List_ID=trl.VAF_CtrlRef_List_ID AND trl.VAF_Language='")
                        .Append(language.GetVAF_Language()).Append("')");
            realSQL.Append(" WHERE VAF_CtrlRef_List.VAF_Control_Ref_ID=").Append(VAF_Control_Ref_Value_ID)
                .Append(" AND VAF_CtrlRef_List.Value=").Append(linkColumnName);
            //
            return realSQL.ToString();
        }


        /// <summary>
        ///	Get Lookup SQL for direct Table Lookup
        /// </summary>
        /// <param name="ColumnName">column name</param>
        /// <returns></returns>
        public static VLookUpInfo GetLookUp_TableDir(Ctx ctx, Language language, int winNum, string columnName)
        {
            if (!columnName.ToUpper().EndsWith("_ID"))
            {
                ////Common.////ErrorLog.FillErrorLog("VLookupInfo", "", "Key does not end with '_ID': " + ColumnName, VAdvantage.Framework.Message.MessageType.ERROR);
                s_log.Log(Level.SEVERE, "Key does not end with '_ID': " + columnName);
                return null;
            }
            //	Hardcoded BPartner Org
            if (columnName.Equals("VAF_OrgBP_ID"))
                columnName = "VAF_Org_ID";

            if (columnName.IndexOf("M_Locator") != -1)
                columnName = "M_Locator_ID";

            string tableName = columnName.Substring(0, columnName.Length - 3);
            //	boolean isSOTrx = !"N".Equals(ctx.getContext( WindowNo, "IsSOTrx"));
            int ZoomWindow = 0;
            int ZoomWindowPO = 0;
            bool isTranslated = false;
            string keyColumn = columnName;

            string sql = "SELECT t.VAF_Screen_ID,t.PO_Window_ID "
                + "FROM VAF_TableView t "
                + "WHERE tableName=@tableName ";

            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@tableName", tableName);

                dr = DataBase.DB.ExecuteReader(sql, param);
                while (dr.Read())
                {
                    ZoomWindow = Utility.Util.GetValueOfInt(dr[0]);
                    ZoomWindowPO = Utility.Util.GetValueOfInt(dr[1]);
                }
                dr.Close();
                dr = null;
                param = null;

            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                s_log.Log(Level.SEVERE, sql, e);
                return null;
            }

            isTranslated = IsTranslated(tableName);

            StringBuilder realSQL = new StringBuilder("SELECT ");
            realSQL.Append(tableName).Append(".").Append(keyColumn).Append(",NULL,");

            StringBuilder displayColumn = GetLookup_DisplayColumn(language, tableName);

            realSQL.Append((displayColumn == null) ? "NULL" : displayColumn.ToString());
            realSQL.Append(",").Append(tableName).Append(".IsActive");

            //  Translation
            if (isTranslated && !Env.IsBaseLanguage(language, tableName))//  GlobalVariable.IsBaseLanguage())
            {
                realSQL.Append(" FROM ").Append(tableName)
                    .Append(" INNER JOIN ").Append(tableName).Append("_TRL ON (")
                    .Append(tableName).Append(".").Append(keyColumn)
                    .Append("=").Append(tableName).Append("_Trl.").Append(keyColumn)
                    .Append(" AND ").Append(tableName).Append("_Trl.VAF_Language='")
                    .Append(language.GetVAF_Language()).Append("')");
            }
            else	//	no translation
            {
                realSQL.Append(" FROM ").Append(tableName);
            }

            //	Order by Display    
            realSQL.Append(" ORDER BY 3");
            //	((LookupDisplayColumn)list.get(3)).ColumnName);
            Query zoomQuery = null;	//	corrected in VLookup

            if (VLogMgt.IsLevelFinest())
                s_log.Fine("ColumnName=" + columnName + " - " + realSQL);
            VLookUpInfo lInfo = new VLookUpInfo(realSQL.ToString(), tableName,
                tableName + "." + keyColumn, ZoomWindow, ZoomWindowPO, zoomQuery);

            // display column for Table Direct columns
            if (displayColumn != null)
                lInfo.displayColSubQ = displayColumn.ToString();
            else
                lInfo.displayColSubQ = "";

            return lInfo;
        }


        /// <summary>
        ///Get embedded SQL for TableDir Lookup (no translation)
        /// </summary>
        /// <param name="language">language</param>
        /// <param name="ColumnName">col name</param>
        /// <param name="BaseTable">base table name</param>
        /// <returns>ELECT Column FROM TableName WHERE BaseTable.ColumnName=TableName.ColumnName</returns>
        static public String GetLookup_TableDirEmbed(Language language, String columnName, String baseTable)
        {
            return GetLookup_TableDirEmbed(language, columnName, baseTable, columnName);
        }

        /// <summary>
        /// Get embedded SQL for TableDir Lookup (no translation)
        /// </summary>
        /// <param name="language"></param>
        /// <param name="ColumnName"></param>
        /// <param name="BaseTable"></param>
        /// <param name="BaseColumn"></param>
        /// <returns>SELECT Column FROM TableName WHERE BaseTable.BaseColumn=TableName.ColumnName</returns>
        public static String GetLookup_TableDirEmbed(Language language,
            String columnName, String baseTable, String baseColumn)
        {
            String tableName = columnName.Substring(0, columnName.Length - 3);

            //	get display column name (first identifier column)
            String sql = "SELECT c.ColumnName,c.IsTranslated,c.VAF_Control_Ref_ID,c.VAF_Control_Ref_Value_ID "
                + "FROM VAF_TableView t INNER JOIN VAF_Column c ON (t.VAF_TableView_ID=c.VAF_TableView_ID) "
                + "WHERE TableName='" + tableName + "'"
                + " AND c.IsIdentifier='Y' "
                + "ORDER BY c.SeqNo";
            //
            List<LookupDisplayColumn> list = new List<LookupDisplayColumn>();
            //
            IDataReader dr = null;
            try
            {
                //PreparedStatement pstmt = DataBase.prepareStatement(sql, null);
                dr = DataBase.DB.ExecuteReader(sql);
                while (dr.Read())
                {
                    LookupDisplayColumn ldc = new LookupDisplayColumn(dr[0].ToString(),
                        "Y".Equals(dr[1].ToString()), Utility.Util.GetValueOfInt(dr[2]), Utility.Util.GetValueOfInt(dr[3]));
                    list.Add(ldc);
                    //	s_log.fine("getLookup_TableDirEmbed: " + ColumnName + " - " + ldc);
                }
                dr.Close();
                dr = null;
            }
            catch (System.Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                ////Common.////ErrorLog.FillErrorLog("VlookupFactory", sql, e.Message , VAdvantage.Framework.Message.MessageType.ERROR);
                s_log.Log(Level.SEVERE, sql, e);
                return "";
            }
            //  Do we have columns ?
            if (list.Count == 0)
            {
                // Change By Lokesh Chauhan In Case Primary Key Column differs from table name in case

                String sql1 = "SELECT c.ColumnName,c.IsTranslated,c.VAF_Control_Ref_ID,c.VAF_Control_Ref_Value_ID "
               + "FROM VAF_TableView t INNER JOIN VAF_Column c ON (t.VAF_TableView_ID=c.VAF_TableView_ID) "
               + "WHERE UPPER(TableName)=UPPER('" + tableName + "')"
               + " AND c.IsIdentifier='Y' "
               + "ORDER BY c.SeqNo";
                int count = 0;
                try
                {

                    //PreparedStatement pstmt = DataBase.prepareStatement(sql, null);
                    dr = DataBase.DB.ExecuteReader(sql1);
                    while (dr.Read())
                    {
                        count++;
                        LookupDisplayColumn ldc = new LookupDisplayColumn(dr[0].ToString(),
                            "Y".Equals(dr[1].ToString()), Utility.Util.GetValueOfInt(dr[2]), Utility.Util.GetValueOfInt(dr[3]));
                        list.Add(ldc);
                        //	s_log.fine("getLookup_TableDirEmbed: " + ColumnName + " - " + ldc);
                    }
                    dr.Close();
                    dr = null;
                }
                catch (System.Exception e)
                {
                    if (dr != null)
                    {
                        dr.Close();
                        dr = null;
                    }
                    ////Common.////ErrorLog.FillErrorLog("VlookupFactory", sql, e.Message , VAdvantage.Framework.Message.MessageType.ERROR);
                    s_log.Log(Level.SEVERE, sql, e);
                    return "";
                }

                if (count < 1)
                {

                    ////Common.////ErrorLog.FillErrorLog("VlookupFactory", "", "No Identifier records found: " + columnName, VAdvantage.Framework.Message.MessageType.ERROR);
                    s_log.Log(Level.SEVERE, "No Identifier records found: " + columnName);
                    return "";
                }
            }


            //
            StringBuilder embedSQL = new StringBuilder("SELECT ");

            int size = list.Count;
            for (int i = 0; i < size; i++)
            {
                if (i > 0)
                    embedSQL.Append("||' - '||");
                LookupDisplayColumn ldc = list[i];

                //  date, number
                if (DisplayType.IsDate(ldc.DisplayType) || DisplayType.IsNumeric(ldc.DisplayType))
                {
                    embedSQL.Append("NVL(" + DataBase.DB.TO_CHAR(tableName + "." + ldc.ColumnName, ldc.DisplayType, language.GetVAF_Language()) + ",'')");
                }
                //  TableDir
                else if ((ldc.DisplayType == DisplayType.TableDir || ldc.DisplayType == DisplayType.Search)
                  && ldc.ColumnName.EndsWith("_ID"))
                {
                    String embeddedSQL = GetLookup_TableDirEmbed(language, ldc.ColumnName, tableName);
                    embedSQL.Append("NVL((").Append(embeddedSQL).Append("),'')");
                }
                //  String
                else
                {
                    ////jz EDB || problem
                    //if (DatabaseType.IsPostgre)
                    //    embedSQL.Append("COALESCE(TO_CHAR(").Append(tableName).Append(".").Append(ldc.ColumnName).Append("),'')");
                    //else
                    embedSQL.Append("NVL(").Append(tableName).Append(".").Append(ldc.ColumnName).Append(",'')");
                }
            }

            embedSQL.Append(" FROM ").Append(tableName);
            embedSQL.Append(" WHERE ").Append(baseTable).Append(".").Append(baseColumn);
            embedSQL.Append("=").Append(tableName).Append(".").Append(columnName);
            //
            return embedSQL.ToString();
        }


        /// <summary>
        /// Check Table Is Translatation type or not
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <returns>true if translated</returns>
        private static bool IsTranslated(string tableName)
        {
            bool isTranslated = false;

            //if (_sIsTranslated.ContainsKey(tableName))
            //{
            //    return _sIsTranslated[tableName];
            //}

            string sql1 = "SELECT count(*) "
                + "FROM VAF_TableView t"
                + " INNER JOIN VAF_Column c ON (t.VAF_TableView_ID=c.VAF_TableView_ID) "
                + "WHERE tableName=@tableName"
                + " AND c.IsIdentifier='Y' "
                + " AND c.IsTranslated = 'Y' ";

            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@tableName", tableName);

                int count = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteScalar(sql1, param, null));
                isTranslated = !(count == 0);
                param = null;
            }
            catch (System.Data.Common.DbException e)
            {
                s_log.Log(Level.SEVERE, sql1, e);
            }
            //if (!_sCacheRefTable.ContainsKey(tableName))
            //{
            //    _sIsTranslated.Add(tableName, isTranslated);
            //}
            return isTranslated;
        }

        /// <summary>
        /// Get Display Columns SQL for Table/Table Direct Lookup
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <param name="language">language object </param>
        /// <returns></returns>
        public static StringBuilder GetLookup_DisplayColumn(Language language, string tableName)
        {
            //	get display column names
            String sql0 = "SELECT c.ColumnName,c.IsTranslated,c.VAF_Control_Ref_ID,"
                + "c.VAF_Control_Ref_Value_ID,t.VAF_Screen_ID,t.PO_Window_ID "
                + "FROM VAF_TableView t"
                + " INNER JOIN VAF_Column c ON (t.VAF_TableView_ID=c.VAF_TableView_ID) "
                + "WHERE tableName=@tableName"
                + " AND c.IsIdentifier='Y' "
                + "ORDER BY c.SeqNo";

            List<LookupDisplayColumn> list = new List<LookupDisplayColumn>();
            bool isTranslated = false;
            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@tableName", tableName);
                dr = DataBase.DB.ExecuteReader(sql0, param);
                while (dr.Read())
                {
                    LookupDisplayColumn ldc = new LookupDisplayColumn(dr[0].ToString(),
                        "Y".Equals(dr[1].ToString()), Utility.Util.GetValueOfInt(dr[2]), Utility.Util.GetValueOfInt(dr[3]));
                    list.Add(ldc);

                    if (!isTranslated && ldc.IsTranslated)
                        isTranslated = true;
                }
                dr.Close();
                dr = null;
                param = null;
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                s_log.Log(Level.SEVERE, sql0, e);
                return null;
            }

            //  Do we have columns ?
            if (list.Count == 0)
            {
                s_log.Log(Level.SEVERE, "No Identifier records found: " + tableName);
                ////Common.////ErrorLog.FillErrorLog("VlookupFactory", "", "No Identifier records found: " + tableName, VAdvantage.Framework.Message.MessageType.ERROR);
                return null;
            }

            StringBuilder displayColumn = new StringBuilder("");
            int size = list.Count;
            //  Get Display Column
            for (int i = 0; i < size; i++)
            {
                if (i > 0)
                    displayColumn.Append(" ||'_'|| ");
                LookupDisplayColumn ldc = list[i];
                //jz EDB || problem
                //if (DatabaseType.IsPostgre)
                //    displayColumn.Append("COALESCE(TO_CHAR(");
                //else if (DatabaseType.IsMSSql)
                //    displayColumn.Append("COALESCE(CONVERT(VARCHAR,");
                displayColumn.Append("NVL(");
                //  translated
                if (ldc.IsTranslated && !Env.IsBaseLanguage(language, tableName))//  DataBase.GlobalVariable.IsBaseLanguage())
                    displayColumn.Append(tableName).Append("_Trl.").Append(ldc.ColumnName);
                //  date
                else if (DisplayType.IsDate(ldc.DisplayType))
                {
                    displayColumn.Append(DataBase.DB.TO_CHAR(tableName + "." + ldc.ColumnName, ldc.DisplayType, language.GetVAF_Language()));
                }
                //Search with ref key
                else if (ldc.DisplayType == DisplayType.Search && ldc.AD_Ref_Val_ID > 0)
                {
                    string embeddedSQL = GetLookup_TableEmbed(language, ldc.ColumnName, tableName, ldc.AD_Ref_Val_ID);
                    if (embeddedSQL != null)
                        displayColumn.Append("(").Append(embeddedSQL).Append(")");
                }

                //  TableDir // Search 
                else if ((ldc.DisplayType == DisplayType.TableDir || ldc.DisplayType == DisplayType.Search)
                    && ldc.ColumnName.EndsWith("_ID"))
                {
                    string embeddedSQL = GetLookup_TableDirEmbed(language, ldc.ColumnName, tableName);
                    if (embeddedSQL != null)
                        displayColumn.Append("(").Append(embeddedSQL).Append(")");
                }

                //	Table
                else if (ldc.DisplayType == DisplayType.Table && ldc.AD_Ref_Val_ID != 0)
                {
                    string embeddedSQL = GetLookup_TableEmbed(language, ldc.ColumnName, tableName, ldc.AD_Ref_Val_ID);
                    if (embeddedSQL != null)
                        displayColumn.Append("(").Append(embeddedSQL).Append(")");
                }
                //  number
                else if (DisplayType.IsNumeric(ldc.DisplayType)|| DisplayType.IsID(ldc.DisplayType))
                {
                    displayColumn.Append(DataBase.DB.TO_CHAR(tableName + "." + ldc.ColumnName, ldc.DisplayType, language.GetVAF_Language()));
                }
                //  String
                else
                {
                    //jz EDB || null issue
                    if (DatabaseType.IsPostgre)
                        displayColumn.Append("NVL(").Append(tableName).Append(".").Append(ldc.ColumnName).Append(",'')");
                    else if (DatabaseType.IsMSSql)
                        displayColumn.Append("COALESCE(CONVERT(VARCHAR,").Append(tableName).Append(".").Append(ldc.ColumnName).Append("),'')");
                    else
                        displayColumn.Append(tableName).Append(".").Append(ldc.ColumnName);
                }

                //jz EDB || problem
                //if (DatabaseType.IsPostgre || DatabaseType.IsMSSql)
                displayColumn.Append(",'')");
            }
            return displayColumn;
        }



        /// <summary>
        /// Get Lookup SQL for Table Lookup
        /// </summary>
        /// <param name="VAF_Control_Ref_Value_ID"></param>
        /// <returns></returns>
        private static VLookUpInfo GetLookup_Table(Ctx ctx, Language language, int windowNum, int VAF_Control_Ref_Value_ID)
        {
            string key = VAF_Control_Ref_Value_ID.ToString();
            VLookUpInfo retValue = null;
            //_sCacheRefTable.TryGetValue(key, out retValue);
            //if (retValue != null)
            //{
            //    s_log.Finest("Cache: " + retValue);
            //    return retValue.Clone();
            //}

            string sql0 = "SELECT t.TableName,ck.ColumnName AS KeyColumn,"				//	1..2
            + "cd.ColumnName AS DisplayColumn,rt.IsValueDisplayed,cd.IsTranslated,"	//	3..5
            + "rt.WhereClause,rt.OrderByClause,t.VAF_Screen_ID,t.PO_Window_ID, "		//	6..9
            + "t.VAF_TableView_ID , rt.IsDisplayIdentifiers "								//	10..11
            + "FROM VAF_CtrlRef_Table rt"
            + " INNER JOIN VAF_TableView t ON (rt.VAF_TableView_ID=t.VAF_TableView_ID)"
            + " INNER JOIN VAF_Column ck ON (rt.Column_Key_ID=ck.VAF_Column_ID)"
            + " INNER JOIN VAF_Column cd ON (rt.Column_Display_ID=cd.VAF_Column_ID) "
            + "WHERE rt.VAF_Control_Ref_ID = '" + key + "' "
            + " AND rt.IsActive='Y' AND t.IsActive='Y'";
            //
            string keyColumn = "", tableName = "", whereClause = "", orderByClause = "";
            string displayColumn = "";
            bool isTranslated = false, isValueDisplayed = false, isDisplayIdentifiers = false;
            int zoomWindow = 0;
            int zoomWindowPO = 0;
            //	int VAF_TableView_ID = 0;
            bool loaded = false;

            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql0);
                while (dr.Read())
                {
                    tableName = dr[0].ToString();
                    keyColumn = dr[1].ToString();
                    displayColumn = dr[2].ToString();
                    isValueDisplayed = "Y".Equals(dr[3].ToString());
                    isTranslated = "Y".Equals(dr[4].ToString());
                    whereClause = dr[5].ToString();
                    orderByClause = dr[6].ToString();
                    zoomWindow = Utility.Util.GetValueOfInt(dr[7]);
                    zoomWindowPO = Utility.Util.GetValueOfInt(dr[8]);
                    isDisplayIdentifiers = "Y".Equals(dr[10].ToString());
                    loaded = true;
                }
                dr.Close();
                dr = null;

            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                s_log.Log(Level.SEVERE, sql0, e);
                return null;
            }

            if (!loaded)
            {
                s_log.Log(Level.SEVERE, "No Table Reference Table ID=" + VAF_Control_Ref_Value_ID);
                return null;
            }

            isTranslated = isTranslated && IsTranslated(tableName);


            StringBuilder displayColumn1 = null;
            if (isDisplayIdentifiers)
            {
                displayColumn1 = GetLookup_DisplayColumn(language, tableName);
                //if (displayColumn1 == null)
                //{
                //    displayColumn1 = new StringBuilder("NULL");
                //}
                //displayColumn = displayColumn1.ToString();
            }
            if (displayColumn1 == null)
            {
                displayColumn = tableName + ((isTranslated && !Env.IsBaseLanguage(language, tableName)) ? "_TRL." : ".") + displayColumn;
            }
            else
            {
                displayColumn = displayColumn1.ToString();
            }

            StringBuilder sb = new StringBuilder("SELECT ");
            if (!keyColumn.EndsWith("_ID")) { sb.Append("NULL,"); }


            //Translated
            if (isTranslated && !Env.IsBaseLanguage(language, tableName))//  GlobalVariable.IsBaseLanguage())
            {
                sb.Append(tableName).Append(".").Append(keyColumn).Append(",");
                if (keyColumn.EndsWith("_ID"))
                    sb.Append("NULL,");
                if (isValueDisplayed)
                    sb.Append("NVL(").Append(tableName).Append(".Value,'-1') || '-' || ");

                sb.Append(displayColumn.ToString());

                sb.Append(",").Append(tableName).Append(".IsActive");
                sb.Append(" FROM ").Append(tableName)
                    .Append(" INNER JOIN ").Append(tableName).Append("_TRL ON (")
                    .Append(tableName).Append(".").Append(keyColumn)
                    .Append("=").Append(tableName).Append("_Trl.").Append(keyColumn)
                    .Append(" AND ").Append(tableName).Append("_Trl.VAF_Language='")
                    .Append(language.GetVAF_Language()).Append("')");
            }
            //	Not Translated
            else
            {
                sb.Append(tableName).Append(".").Append(keyColumn).Append(",");
                if (keyColumn.EndsWith("_ID"))
                    sb.Append("NULL,");
                if (isValueDisplayed)
                    sb.Append("NVL(").Append(tableName).Append(".Value,'-1') || '-' || ");
                //jz EDB || problem
                //if (DatabaseType.IsPostgre)
                //    sb.Append("COALESCE(TO_CHAR(").Append(displayColumn).Append("),'')");
                //else if (DatabaseType.IsMSSql)
                //   sb.Append("COALESCE(CONVERT(VARCHAR,").Append(displayColumn).Append("),'')");
                //else
                sb.Append("NVL(").Append(displayColumn).Append(",'-1')");
                sb.Append(",").Append(tableName).Append(".IsActive");
                sb.Append(" FROM ").Append(tableName);
            }
            if (!isDisplayIdentifiers)
                sb.Append(" WHERE " + displayColumn + " IS NOT NULL ");

            //	add WHERE clause
            Query zoomQuery = null;
            if (whereClause != "")
            {
                string where = whereClause;
                if (where.IndexOf("@") != -1)
                    where = Utility.Env.ParseContext(ctx, windowNum, where, false);
                if (where.Length == 0 && whereClause.Length != 0)
                {
                    s_log.Severe("Could not resolve: " + whereClause);
                    ////Common.////ErrorLog.FillErrorLog("VlookupFactory","","Could not resolve: " + whereClause,VAdvantage.Framework.Message.MessageType.ERROR);
                }


                //	We have no context
                if (where.Length != 0)
                {
                    if (isDisplayIdentifiers)
                        sb.Append(" WHERE ");
                    else
                        sb.Append(" AND ");

                    sb.Append(where);
                    if (where.IndexOf(".") == -1)
                    {
                        s_log.Log(Level.SEVERE, "Table - " + tableName
                        + ": WHERE should be fully qualified: " + whereClause);
                    }
                    zoomQuery = new Query(tableName);
                    zoomQuery.AddRestriction(where);
                }
            }

            //	Order By qualified term or by Name
            if (orderByClause != "")
            {
                sb.Append(" ORDER BY ").Append(orderByClause);
                if (orderByClause.IndexOf(".") == -1)
                {
                    s_log.Log(Level.SEVERE, "getLookup_Table - " + tableName
                        + ": ORDER BY must fully qualified: " + orderByClause);
                }
            }
            else
                sb.Append(" ORDER BY 3");

            s_log.Finest("VAF_Control_Ref_Value_ID=" + VAF_Control_Ref_Value_ID + " - " + sb.ToString());
            retValue = new VLookUpInfo(sb.ToString(), tableName,
                tableName + "." + keyColumn, zoomWindow, zoomWindowPO, zoomQuery);
            //if(!_sCacheRefTable.ContainsKey(key))
            // {
            //  _sCacheRefTable[key] = retValue.Clone();
            // }
            
            // display column  for Table type of references
            retValue.displayColSubQ = displayColumn;

            return retValue;
        }

        /// <summary>
        ///Get Embedded Lookup SQL for Table Lookup
        /// </summary>
        /// <param name="BaseColumn"></param>
        /// <param name="BaseTable"></param>
        /// <param name="VAF_Control_Ref_Value_ID"></param>
        /// <returns></returns>
        static public string GetLookup_TableEmbed(Language language, string BaseColumn, string BaseTable, int VAF_Control_Ref_Value_ID)
        {
            string sql = "SELECT t.tableName,ck.ColumnName AS keyColumn,"
                + "cd.ColumnName AS DisplayColumn,rt.IsValueDisplayed,cd.IsTranslated "
                + "FROM VAF_CtrlRef_Table rt"
                + " INNER JOIN VAF_TableView t ON (rt.VAF_TableView_ID=t.VAF_TableView_ID)"
                + " INNER JOIN VAF_Column ck ON (rt.Column_Key_ID=ck.VAF_Column_ID)"
                + " INNER JOIN VAF_Column cd ON (rt.Column_Display_ID=cd.VAF_Column_ID) "
                + "WHERE rt.VAF_Control_Ref_ID=" + VAF_Control_Ref_Value_ID.ToString() + ""
                + " AND rt.IsActive='Y' AND t.IsActive='Y'";
            //
            string keyColumn = "", DisplayColumn = "", tableName = "";
            bool isTranslated = false, isValueDisplayed = false;
            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql);
                bool success = false;
                while (dr.Read())
                {
                    tableName = Utility.Util.GetValueOfString(dr[0]);
                    keyColumn = Utility.Util.GetValueOfString(dr[1]);
                    DisplayColumn = Utility.Util.GetValueOfString(dr[2]);
                    isValueDisplayed = dr[3].ToString().Equals("Y");
                    isTranslated = dr[4].ToString().Equals("Y");
                    success = true;
                }
                dr.Close();
                dr = null;
                if (!success)
                {
                    s_log.Log(Level.SEVERE, "Cannot find Reference Table, ID=" + VAF_Control_Ref_Value_ID
                    + ", Base=" + BaseTable + "." + BaseColumn);
                    return null;
                }
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                s_log.Log(Level.SEVERE, sql, e);
                return null;
            }

            StringBuilder embedSQL = new StringBuilder("SELECT ");
            //	Translated
            if (isTranslated && !Env.IsBaseLanguage(language, tableName))// GlobalVariable.IsBaseLanguage())
            {
                if (isValueDisplayed)
                    embedSQL.Append(tableName).Append(".Value||'-'||");
                embedSQL.Append(tableName).Append("_Trl.").Append(DisplayColumn);
                //
                embedSQL.Append(" FROM ").Append(tableName)
                    .Append(" INNER JOIN ").Append(tableName).Append("_TRL ON (")
                    .Append(tableName).Append(".").Append(keyColumn)
                    .Append("=").Append(tableName).Append("_Trl.").Append(keyColumn)
                    .Append(" AND ").Append(tableName).Append("_Trl.VAF_Language='")
                    .Append(language.GetVAF_Language()).Append("')");
            }
            //	Not Translated
            else
            {
                if (isValueDisplayed)
                    embedSQL.Append(tableName).Append(".Value||'-'||");
                embedSQL.Append(tableName).Append(".").Append(DisplayColumn);
                //
                embedSQL.Append(" FROM ").Append(tableName);
            }

            embedSQL.Append(" WHERE ").Append(BaseTable).Append(".").Append(BaseColumn);
            embedSQL.Append("=").Append(tableName).Append(".").Append(keyColumn);

            return embedSQL.ToString();
        }	//	getLookup_TableEmbed

        /// <summary>
        /// crate Lookup_Acct info
        /// </summary>
        /// <param name="VAF_Column_ID"></param>
        /// <returns></returns>
        private static VLookUpInfo GetLookup_Acct(Ctx ctx, int VAF_Column_ID)
        {
            //	Try cache - assume no language change
            string key = "Acct" + VAF_Column_ID;
            VLookUpInfo retValue = null;
            //_sCacheRefTable.TryGetValue(key, out retValue);
            //if (retValue != null)
            //{
            //    s_log.Finest("Cache: " + retValue);
            //    return retValue.Clone();
            //}
            string displayColumn = "Combination";
            string sql = "SELECT VAB_Acct_ValidParameter_ID, NULL, " + displayColumn + ", IsActive FROM VAB_Acct_ValidParameter";
            int zoomWindow = 153;
            Query zoomQuery = new Query("VAB_Acct_ValidParameter");
            //
            retValue = new VLookUpInfo(sql, "VAB_Acct_ValidParameter",
                "VAB_Acct_ValidParameter.VAB_Acct_ValidParameter_ID",
                zoomWindow, zoomWindow, zoomQuery);
            // Display column for Account Control
            retValue.displayColSubQ = displayColumn;
            //_sCacheRefTable.Add(key, retValue.Clone());
            return retValue;
        }


        /// <summary>
        /// crate Lookup_Acct info
        /// </summary>
        /// <param name="VAF_Column_ID"></param>
        /// <returns></returns>
        private static VLookUpInfo GetLookup_PContainer(Ctx ctx, int VAF_Column_ID)
        {
            //	Try cache - assume no language change
            string key = "pContainer" + VAF_Column_ID;
            VLookUpInfo retValue = null;
            //_sCacheRefTable.TryGetValue(key, out retValue);
            //if (retValue != null)
            //{
            //    s_log.Finest("Cache: " + retValue);
            //    return retValue.Clone();
            //}
            string displayColumn = "Name";
            string sql = "SELECT M_ProductContainer_ID, NULL, " + displayColumn + ", IsActive FROM M_ProductContainer";
            int zoomWindow = 0;
            Query zoomQuery = new Query("M_ProductContainer");
            //
            retValue = new VLookUpInfo(sql, "M_ProductContainer",
                "M_ProductContainer.M_ProductContainer_ID",
                zoomWindow, zoomWindow, zoomQuery);
            //_sCacheRefTable.Add(key, retValue.Clone());

            // display column name for Product Container control
            retValue.displayColSubQ = displayColumn;

            return retValue;
        }


    }

    

    //  Access Sql Parser Class
    //Parse FROM in SQL WHERE clause

  
}
