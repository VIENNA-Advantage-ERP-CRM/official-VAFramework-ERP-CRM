/********************************************************
// Module Name    : run time show window 
// Purpose        : create the dymanmic sql query for loookup Fields
// Class Used     : GlobalVariable.cs, CommonFunctions.cs,Ctx.cs
// Created By     : Harwinder 
// Date           : -----   
**********************************************************/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Login;
using VAdvantage.DataBase;
using System.Data.SqlClient;
using VAdvantage.Model;
using VAdvantage.Process;
using VAdvantage.Utility;
using VAdvantage.Logging;
using VAdvantage.Controller;

namespace VAdvantage.Classes
{
    /// <summary>
    /// create Lookup field
    /// </summary>
    public class VLookUpFactory
    {
        /*Table IsTranslated Cache */
        // private static CCache<string, bool> _sIsTranslated = new CCache<string, bool>("AD_Table_isTranslated", 10);
        /** Table Reference Cache				*/
        // private static CCache<string, VLookUpInfo> _sCacheRefTable = new CCache<string, VLookUpInfo>("AD_Ref_Table", 30, 60);	//	1h
        /**	Logging								*/
        private static VLogger s_log = VLogger.GetVLogger(typeof(VLookUpInfo).FullName);

        /// <summary>
        /// Create MLookup
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="windowNo">window no</param>
        /// <param name="Column_ID">column id</param>
        /// <param name="AD_Reference_ID">reference id</param>
        /// <returns>MLookup</returns>
        public static MLookup Get(Ctx ctx, int windowNo, int Column_ID, int AD_Reference_ID)
        {
            String columnName = "";
            int AD_Reference_Value_ID = 0;
            bool isParent = false;
            String validationCode = "";
            //
            String sql = "SELECT c.ColumnName, c.AD_Reference_Value_ID, c.IsParent, vr.Code "
                + "FROM AD_Column c"
                + " LEFT OUTER JOIN AD_Val_Rule vr ON (c.AD_Val_Rule_ID=vr.AD_Val_Rule_ID) "
                + "WHERE c.AD_Column_ID=" + Column_ID;
            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql);
                if (dr.Read())
                {
                    columnName = dr[0].ToString();
                    AD_Reference_Value_ID = Utility.Util.GetValueOfInt(dr[1]);
                    isParent = "Y".Equals(dr[2].ToString());
                    validationCode = dr[3].ToString();
                }
                else
                {
                    s_log.Log(Level.SEVERE, "Column Not Found - AD_Column_ID=" + Column_ID);
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
            MLookup lookup = new MLookup(ctx, windowNo, AD_Reference_ID);
            VLookUpInfo info = GetLookUpInfo(lookup, Column_ID,
                Env.GetLanguage(ctx), columnName, AD_Reference_Value_ID, isParent, validationCode);
            //VLookUpInfo info = GetLookUpInfo(lookup.GetCtx(), lookup.GetWindowNo(), lookup.GetDisplayType(),
            //  Column_ID,Env.GetLanguage(ctx), columnName, AD_Reference_Value_ID, isParent, validationCode);
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
        /// <param name="AD_Reference_ID">displaytype id </param>
        /// <param name="ColumnName">name of column</param>
        /// <param name="AD_Reference_Value_ID"> </param>
        /// <param name="IsParent">is parent column</param>
        /// <param name="ValidationCode">validate text</param>
        /// <returns>MLookup object</returns>
        public static MLookup Get(Ctx ctx, int windowNo, int Column_ID, int AD_Reference_ID,
                 String columnName, int AD_Reference_Value_ID,
                bool IsParent, String ValidationCode)
        {
            MLookup lookup = new MLookup(ctx, windowNo, AD_Reference_ID);
            VLookUpInfo info = GetLookUpInfo(lookup, Column_ID,
                Env.GetLanguage(ctx), columnName, AD_Reference_Value_ID, IsParent, ValidationCode);
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
        /// <param name="Column_ID">AD_Column_ID or AD_Process_Para_ID</param>
        /// <param name="language">report lang</param>
        /// <param name="ColumnName">key column name</param>
        /// <param name="AD_Reference_Value_ID"></param>
        /// <param name="IsParent">parent (prevents query to directly access value)</param>
        /// <param name="ValidationCode">ValidationCode optional SQL validation</param>
        /// <returns>lookup info structure</returns>
        public static VLookUpInfo GetLookUpInfo(Lookup lookup,
            int Column_ID, Language language, String columnName, int AD_Reference_Value_ID,
            bool IsParent, String ValidationCode)
        {
            return GetLookUpInfo(lookup.GetCtx(), lookup.GetWindowNo(), lookup.GetDisplayType(),
                Column_ID, language, columnName, AD_Reference_Value_ID,
                IsParent, ValidationCode);
        }

        /// <summary>
        /// Get Information for Lookups based on Column_ID for Table Columns or Process Parameters.
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Reference_ID">ref id</param>
        /// <param name="Column_ID">col id</param>
        /// <param name="ColumnName">name of col</param>
        /// <param name="AD_Reference_Value_ID">ref_val id</param>
        /// <param name="IsParent">is parent col</param>
        /// <param name="ValidationCode">validate text</param>
        /// <returns></returns>
        public static VLookUpInfo GetLookUpInfo(Ctx ctx, int windowNum, int AD_Reference_ID,
            int Column_ID, Language language, string columnName, int AD_Reference_Value_ID, bool IsParent, string ValidationCode)
        {
            VLookUpInfo info = null;
            bool needToAddSecurity = true;
            //	List
            if (AD_Reference_ID == DisplayType.List)	//	17
            {
                info = GetLookUp_List(language, AD_Reference_Value_ID);
                info.hasImageIdentifier = true;
                needToAddSecurity = false;
            }
            // TAble OR Search with Reference value
            else if ((AD_Reference_ID == DisplayType.Table || AD_Reference_ID == DisplayType.Search || DisplayType.MultiKey == AD_Reference_ID)
            && AD_Reference_Value_ID != 0)
            {
                info = GetLookup_Table(ctx, language, windowNum, AD_Reference_Value_ID);
            }
            //	Acct
            else if (AD_Reference_ID == DisplayType.Account)
            {
                info = GetLookup_Acct(ctx, Column_ID);
            }
            else if (AD_Reference_ID == DisplayType.ProductContainer)
            {
                info = GetLookup_PContainer(ctx, Column_ID);
            }
            else if (AD_Reference_ID == DisplayType.List)
            {
                info = GetLookup_List(ctx, language, windowNum, AD_Reference_Value_ID);
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
            info.AD_Reference_Value_ID = AD_Reference_Value_ID;
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
                    || info.validationCode.IndexOf("@AD_Org_ID@") != -1)	//	don't validate Org
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
        /// <param name="AD_Reference_Value_ID">ref_val id</param>
        /// <returns></returns>
        public static VLookUpInfo GetLookUp_List(Language language, int AD_Reference_Value_ID)
        {
            StringBuilder realSQL = new StringBuilder("SELECT NULL, AD_Ref_List.Value,");
            String displayCol = "AD_Ref_List.Name";
            if (Utility.Env.IsBaseLanguage(language, "AD_Ref_List"))
            {
                realSQL.Append(displayCol + ", AD_Ref_List.IsActive, (SELECT COALESCE(FontName,ImageURL)||'|'|| FontStyle FROM AD_Image WHERE AD_Image_ID=AD_Ref_List.AD_Image_ID) AS Image,(SELECT ListDisplayOption FROM AD_Reference WHERE AD_Reference_ID=AD_Ref_List.AD_Reference_ID) AS ListDisplayOption FROM AD_Ref_List AD_Ref_List");
            }
            else
            {
                displayCol = "trl.Name";
                realSQL.Append(displayCol + ", AD_Ref_List.IsActive, (SELECT COALESCE(FontName,ImageURL)||'|'|| FontStyle FROM AD_Image WHERE AD_Image_ID=AD_Ref_List.AD_Image_ID) AS Image, (SELECT ListDisplayOption FROM AD_Reference WHERE AD_Reference_ID=AD_Ref_List.AD_Reference_ID) AS ListDisplayOption "
                    + " FROM AD_Ref_List AD_Ref_List INNER JOIN AD_Ref_List_Trl trl "
                    + " ON (AD_Ref_List.AD_Ref_List_ID=trl.AD_Ref_List_ID AND trl.AD_Language='")
                        .Append(language.GetAD_Language()).Append("')");
            }
            //realSQL.Append(" INNER JOIN AD_Reference ref ON (ref.AD_Reference_ID=AD_Ref_List.AD_Reference_ID) ");
            //realSQL.Append(" LEFT OUTER JOIN AD_Image img ON (AD_Ref_List.AD_Image_ID=img.AD_Image_ID) ");

            realSQL.Append(" WHERE AD_Ref_List.AD_Reference_ID=").Append(AD_Reference_Value_ID);
            realSQL.Append(" ORDER BY 2");
            //
            VLookUpInfo lookupInfo = new VLookUpInfo(realSQL.ToString(), "AD_Ref_List", "AD_Ref_List.Value",
                101, 101, Query.GetEqualQuery("AD_Reference_ID", AD_Reference_Value_ID));	//	Zoom Window+Query
            // add extra property for display Column value
            lookupInfo.displayColSubQ = displayCol;
            return lookupInfo;
        }

        /// <summary>
        ///Get Lookup SQL for List
        /// </summary>
        /// <param name="language"></param>
        /// <param name="AD_Reference_Value_ID"></param>
        /// <param name="linkColumnName"></param>
        /// <returns>SELECT Name FROM AD_Ref_List WHERE AD_Reference_ID=x AND Value=linkColumn</returns>
        public static String GetLookup_ListEmbed(Language language,
            int AD_Reference_Value_ID, String linkColumnName)
        {
            StringBuilder realSQL = new StringBuilder("SELECT ");
            if (Utility.Env.IsBaseLanguage(language, "AD_Ref_List"))
                realSQL.Append("AD_Ref_List.Name FROM AD_Ref_List");
            else
                realSQL.Append("trl.Name "
                    + "FROM AD_Ref_List INNER JOIN AD_Ref_List_Trl trl "
                    + " ON (AD_Ref_List.AD_Ref_List_ID=trl.AD_Ref_List_ID AND trl.AD_Language='")
                        .Append(language.GetAD_Language()).Append("')");
            realSQL.Append(" WHERE AD_Ref_List.AD_Reference_ID=").Append(AD_Reference_Value_ID)
                .Append(" AND AD_Ref_List.Value=").Append(linkColumnName);
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
            if (columnName.Equals("AD_OrgBP_ID"))
                columnName = "AD_Org_ID";

            if (columnName.IndexOf("M_Locator") != -1)
                columnName = "M_Locator_ID";

            string tableName = columnName.Substring(0, columnName.Length - 3);
            //	boolean isSOTrx = !"N".Equals(ctx.getContext( WindowNo, "IsSOTrx"));
            int ZoomWindow = 0;
            int ZoomWindowPO = 0;
            bool isTranslated = false;
            string keyColumn = columnName;

            string sql = "SELECT t.AD_Window_ID,t.PO_Window_ID "
                + "FROM AD_Table t "
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
            bool hasImageIdentifier = false;
            StringBuilder displayColumn = GetLookup_DisplayColumn(language, tableName,out hasImageIdentifier);


            realSQL.Append((displayColumn == null) ? "NULL" : displayColumn.ToString());
            realSQL.Append(",").Append(tableName).Append(".IsActive");

            //  Translation
            if (isTranslated && !Env.IsBaseLanguage(language, tableName))//  GlobalVariable.IsBaseLanguage())
            {
                realSQL.Append(" FROM ").Append(tableName)
                    .Append(" INNER JOIN ").Append(tableName).Append("_TRL ON (")
                    .Append(tableName).Append(".").Append(keyColumn)
                    .Append("=").Append(tableName).Append("_Trl.").Append(keyColumn)
                    .Append(" AND ").Append(tableName).Append("_Trl.AD_Language='")
                    .Append(language.GetAD_Language()).Append("')");
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

            lInfo.hasImageIdentifier = hasImageIdentifier;

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
            String sql = "SELECT c.ColumnName,c.IsTranslated,c.AD_Reference_ID,c.AD_Reference_Value_ID "
                + "FROM AD_Table t INNER JOIN AD_Column c ON (t.AD_Table_ID=c.AD_Table_ID) "
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
                    if (dr[0].ToString().Equals("AD_Image_ID"))
                        continue;
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

                String sql1 = "SELECT c.ColumnName,c.IsTranslated,c.AD_Reference_ID,c.AD_Reference_Value_ID "
               + "FROM AD_Table t INNER JOIN AD_Column c ON (t.AD_Table_ID=c.AD_Table_ID) "
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
                        if (dr[0].ToString().Equals("AD_Image_ID"))
                            continue;
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
                    embedSQL.Append("NVL(" + DataBase.DB.TO_CHAR(tableName + "." + ldc.ColumnName, ldc.DisplayType, language.GetAD_Language()) + ",'')");
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
                + "FROM AD_Table t"
                + " INNER JOIN AD_Column c ON (t.AD_Table_ID=c.AD_Table_ID) "
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
            String sql0 = "SELECT c.ColumnName,c.IsTranslated,c.AD_Reference_ID,"
                + "c.AD_Reference_Value_ID,t.AD_Window_ID,t.PO_Window_ID "
                + "FROM AD_Table t"
                + " INNER JOIN AD_Column c ON (t.AD_Table_ID=c.AD_Table_ID) "
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
                LookupDisplayColumn ldc = list[i];

                if (i > 0)
                {
                    if (ldc.ColumnName.ToLower().Equals("ad_image_id"))
                    {
                        displayColumn.Append(" ||'^^'|| ");
                    }
                    else
                        if (!list[i - 1].ColumnName.ToLower().Equals("ad_image_id"))
                        displayColumn.Append(" ||'_'|| ");
                    else
                        displayColumn.Append(" ||' '|| ");
                }

                //jz EDB || problem
                //if (DatabaseType.IsPostgre)
                //    displayColumn.Append("COALESCE(TO_CHAR(");
                //else if (DatabaseType.IsMSSql)
                //    displayColumn.Append("COALESCE(CONVERT(VARCHAR,");
                displayColumn.Append("NVL(");
                //  translated
                if (ldc.ColumnName.ToLower().Equals("ad_image_id"))
                {
                    string embeddedSQL = "SELECT NVL(ImageURL,'') ||'^^' FROM AD_Image WHERE " + tableName + ".AD_Image_ID=AD_Image.AD_Image_ID";
                    displayColumn.Append("(").Append(embeddedSQL).Append(")");
                    
                }
                else if (ldc.IsTranslated && !Env.IsBaseLanguage(language, tableName))//  DataBase.GlobalVariable.IsBaseLanguage())
                    displayColumn.Append(tableName).Append("_Trl.").Append(ldc.ColumnName);
                //  date
                else if (DisplayType.IsDate(ldc.DisplayType))
                {
                    displayColumn.Append(DataBase.DB.TO_CHAR(tableName + "." + ldc.ColumnName, ldc.DisplayType, language.GetAD_Language()));
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
                else if (DisplayType.IsNumeric(ldc.DisplayType) || DisplayType.IsID(ldc.DisplayType))
                {
                    displayColumn.Append(DataBase.DB.TO_CHAR(tableName + "." + ldc.ColumnName, ldc.DisplayType, language.GetAD_Language()));
                }
                else if (ldc.DisplayType == DisplayType.List && ldc.AD_Ref_Val_ID != 0)
                {
                    // string embeddedSQL = GetLookup_ListEmbed(language, ldc.ColumnName, tableName, ldc.AD_Ref_Val_ID);
                    //if (embeddedSQL != null)
                    //    displayColumn.Append("(").Append(embeddedSQL).Append(")");
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
                if (ldc.ColumnName.ToLower().Equals("ad_image_id"))
                    displayColumn.Append(",'Images/nothing.png^^')");
                else
                    displayColumn.Append(",'')");

                //
            }
            return displayColumn;
        }


        /// <summary>
        /// Get Display Columns SQL for Table/Table Direct Lookup
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <param name="language">language object </param>
        /// <returns></returns>
        public static StringBuilder GetLookup_DisplayColumn(Language language, string tableName,out bool hasImagIdentifier)
        {
            //	get display column names
            String sql0 = "SELECT c.ColumnName,c.IsTranslated,c.AD_Reference_ID,"
                + "c.AD_Reference_Value_ID,t.AD_Window_ID,t.PO_Window_ID "
                + "FROM AD_Table t"
                + " INNER JOIN AD_Column c ON (t.AD_Table_ID=c.AD_Table_ID) "
                + "WHERE tableName=@tableName"
                + " AND c.IsIdentifier='Y' "
                + "ORDER BY c.SeqNo";
            hasImagIdentifier = false;
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
                LookupDisplayColumn ldc = list[i];

                if (i > 0)
                {
                    if (ldc.ColumnName.ToLower().Equals("ad_image_id") || ldc.DisplayType==DisplayType.Image)
                    {
                        displayColumn.Append(" ||'^^'|| ");
                    }
                    else
                        if (!list[i - 1].ColumnName.ToLower().Equals("ad_image_id") && list[i - 1].DisplayType != DisplayType.Image)
                        displayColumn.Append(" ||'_'|| ");
                    else
                        displayColumn.Append(" ||' '|| ");
                }

                //jz EDB || problem
                //if (DatabaseType.IsPostgre)
                //    displayColumn.Append("COALESCE(TO_CHAR(");
                //else if (DatabaseType.IsMSSql)
                //    displayColumn.Append("COALESCE(CONVERT(VARCHAR,");
                displayColumn.Append("NVL(");
                //  translated
                if (ldc.ColumnName.ToLower().Equals("ad_image_id") || ldc.DisplayType == DisplayType.Image)
                {
                    string embeddedSQL = "SELECT NVL(ImageURL,'') ||'^^' FROM AD_Image WHERE CAST(" + tableName + "."+ ldc.ColumnName+ " AS Integer)=AD_Image.AD_Image_ID";
                    displayColumn.Append("(").Append(embeddedSQL).Append(")");
                    hasImagIdentifier = true;

                }
                else if (ldc.IsTranslated && !Env.IsBaseLanguage(language, tableName))//  DataBase.GlobalVariable.IsBaseLanguage())
                    displayColumn.Append(tableName).Append("_Trl.").Append(ldc.ColumnName);
                //  date
                else if (DisplayType.IsDate(ldc.DisplayType))
                {
                    displayColumn.Append(DataBase.DB.TO_CHAR(tableName + "." + ldc.ColumnName, ldc.DisplayType, language.GetAD_Language()));
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
                else if (DisplayType.IsNumeric(ldc.DisplayType) || DisplayType.IsID(ldc.DisplayType))
                {
                    displayColumn.Append(DataBase.DB.TO_CHAR(tableName + "." + ldc.ColumnName, ldc.DisplayType, language.GetAD_Language()));
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
                if (ldc.ColumnName.ToLower().Equals("ad_image_id") || ldc.DisplayType == DisplayType.Image)
                    displayColumn.Append(",'Images/nothing.png^^')");
                else
                    displayColumn.Append(",'')");

                //
            }
            return displayColumn;
        }



        /// <summary>
        /// Get Lookup SQL for Table Lookup
        /// </summary>
        /// <param name="AD_Reference_Value_ID"></param>
        /// <returns></returns>
        private static VLookUpInfo GetLookup_Table(Ctx ctx, Language language, int windowNum, int AD_Reference_Value_ID)
        {
            string key = AD_Reference_Value_ID.ToString();
            VLookUpInfo retValue = null;
            //_sCacheRefTable.TryGetValue(key, out retValue);
            //if (retValue != null)
            //{
            //    s_log.Finest("Cache: " + retValue);
            //    return retValue.Clone();
            //}

            string sql0 = "SELECT t.TableName,ck.ColumnName AS KeyColumn,"				//	1..2
            + "cd.ColumnName AS DisplayColumn,rt.IsValueDisplayed,cd.IsTranslated,"	//	3..5
            + "rt.WhereClause,rt.OrderByClause,t.AD_Window_ID,t.PO_Window_ID, "		//	6..9
            + "t.AD_Table_ID , rt.IsDisplayIdentifiers "								//	10..11
            + "FROM AD_Ref_Table rt"
            + " INNER JOIN AD_Table t ON (rt.AD_Table_ID=t.AD_Table_ID)"
            + " INNER JOIN AD_Column ck ON (rt.Column_Key_ID=ck.AD_Column_ID)"
            + " INNER JOIN AD_Column cd ON (rt.Column_Display_ID=cd.AD_Column_ID) "
            + "WHERE rt.AD_Reference_ID = '" + key + "' "
            + " AND rt.IsActive='Y' AND t.IsActive='Y'";
            //
            string keyColumn = "", tableName = "", whereClause = "", orderByClause = "";
            string displayColumn = "";
            bool isTranslated = false, isValueDisplayed = false, isDisplayIdentifiers = false;
            int zoomWindow = 0;
            int zoomWindowPO = 0;
            //	int AD_Table_ID = 0;
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
                s_log.Log(Level.SEVERE, "No Table Reference Table ID=" + AD_Reference_Value_ID);
                return null;
            }

            isTranslated = isTranslated && IsTranslated(tableName);


            StringBuilder displayColumn1 = null;

            bool hasImageIdentifier = false;
            if (isDisplayIdentifiers)
            {
                displayColumn1 = GetLookup_DisplayColumn(language, tableName,out hasImageIdentifier) ;
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
                    .Append(" AND ").Append(tableName).Append("_Trl.AD_Language='")
                    .Append(language.GetAD_Language()).Append("')");
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

            s_log.Finest("AD_Reference_Value_ID=" + AD_Reference_Value_ID + " - " + sb.ToString());
            retValue = new VLookUpInfo(sb.ToString(), tableName,
                tableName + "." + keyColumn, zoomWindow, zoomWindowPO, zoomQuery);
            //if(!_sCacheRefTable.ContainsKey(key))
            // {
            //  _sCacheRefTable[key] = retValue.Clone();
            // }

            // display column  for Table type of references
            retValue.displayColSubQ = displayColumn;
            retValue.hasImageIdentifier = hasImageIdentifier;

            return retValue;
        }


        private static VLookUpInfo GetLookup_List(Ctx ctx,Language language, int windowNum, int AD_Reference_Value_ID)
        {
            string key = AD_Reference_Value_ID.ToString();
            VLookUpInfo retValue = null;

            string keyColumn = "", tableName = "", whereClause = "", orderByClause = "";
            string displayColumn = "";
            bool isTranslated = false, isValueDisplayed = false, isDisplayIdentifiers = false;
            int zoomWindow = 0;
            int zoomWindowPO = 0;
            //	int AD_Table_ID = 0;
            bool loaded = false;


            tableName = "AD_Ref_List";
            keyColumn = "AD_Ref_List_ID";
            displayColumn = "Name";
            isValueDisplayed = true;
            isTranslated = true;
            whereClause = "";
            orderByClause = "";
            zoomWindow = 0;
            zoomWindowPO = 0;
            isDisplayIdentifiers = false;
            loaded = true;


            if (!loaded)
            {
                s_log.Log(Level.SEVERE, "No Table Reference Table ID=" + AD_Reference_Value_ID);
                return null;
            }

            isTranslated = isTranslated && IsTranslated(tableName);


            //StringBuilder displayColumn1 = null;
            //if (isDisplayIdentifiers)
            //{
            //    displayColumn1 = GetLookup_DisplayColumn(language, tableName);
            //if (displayColumn1 == null)
            //{
            //    displayColumn1 = new StringBuilder("NULL");
            //}
            //displayColumn = displayColumn1.ToString();
            //}
            //if (displayColumn1 == null)
            //{
            //    displayColumn = tableName + ((isTranslated && !Env.IsBaseLanguage(language, tableName)) ? "_TRL." : ".") + displayColumn;
            //}
            //else
            //{
            //    displayColumn = displayColumn1.ToString();
            //}

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
                    .Append(" AND ").Append(tableName).Append("_Trl.AD_Language='")
                    .Append(language.GetAD_Language()).Append("')");
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


            //	add WHERE clause
            Query zoomQuery = null;
            if (whereClause != "")
            {
                string where = whereClause;
                //if (where.IndexOf("@") != -1)
                //    where = Utility.Env.ParseContext(ctx, windowNum, where, false);
                if (where.Length == 0 && whereClause.Length != 0)
                {
                    s_log.Severe("Could not resolve: " + whereClause);
                    ////Common.////ErrorLog.FillErrorLog("VlookupFactory","","Could not resolve: " + whereClause,VAdvantage.Framework.Message.MessageType.ERROR);
                }


                //	We have no context
                //if (where.Length != 0)
                //{
                //    if (isDisplayIdentifiers)
                //        sb.Append(" WHERE ");
                //    else
                //        sb.Append(" AND ");

                //    sb.Append(where);
                //    if (where.IndexOf(".") == -1)
                //    {
                //        s_log.Log(Level.SEVERE, "Table - " + tableName
                //        + ": WHERE should be fully qualified: " + whereClause);
                //    }
                //    zoomQuery = new Query(tableName);
                //    zoomQuery.AddRestriction(where);
                //}
            }

            //	Order By qualified term or by Name
            //if (orderByClause != "")
            //{
            //    sb.Append(" ORDER BY ").Append(orderByClause);
            //    if (orderByClause.IndexOf(".") == -1)
            //    {
            //        s_log.Log(Level.SEVERE, "getLookup_Table - " + tableName
            //            + ": ORDER BY must fully qualified: " + orderByClause);
            //    }
            //}
            //else
            sb.Append(" ORDER BY 3");

            s_log.Finest("AD_Reference_Value_ID=" + AD_Reference_Value_ID + " - " + sb.ToString());
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
        /// <param name="AD_Reference_Value_ID"></param>
        /// <returns></returns>
        static public string GetLookup_TableEmbed(Language language, string BaseColumn, string BaseTable, int AD_Reference_Value_ID)
        {
            string sql = "SELECT t.tableName,ck.ColumnName AS keyColumn,"
                + "cd.ColumnName AS DisplayColumn,rt.IsValueDisplayed,cd.IsTranslated "
                + "FROM AD_Ref_Table rt"
                + " INNER JOIN AD_Table t ON (rt.AD_Table_ID=t.AD_Table_ID)"
                + " INNER JOIN AD_Column ck ON (rt.Column_Key_ID=ck.AD_Column_ID)"
                + " INNER JOIN AD_Column cd ON (rt.Column_Display_ID=cd.AD_Column_ID) "
                + "WHERE rt.AD_Reference_ID=" + AD_Reference_Value_ID.ToString() + ""
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
                    s_log.Log(Level.SEVERE, "Cannot find Reference Table, ID=" + AD_Reference_Value_ID
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
                    .Append(" AND ").Append(tableName).Append("_Trl.AD_Language='")
                    .Append(language.GetAD_Language()).Append("')");
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
        }   //	getLookup_TableEmbed

        static public string GetLookup_ListEmbed(Language language, string BaseColumn,string BaseTable, int AD_Reference_Value_ID)
        {
            string sql = "SELECT t.tableName,ck.ColumnName AS keyColumn,"
                + "cd.ColumnName AS DisplayColumn,rt.IsValueDisplayed,cd.IsTranslated "
                + "FROM AD_Ref_Table rt"
                + " INNER JOIN AD_Table t ON (rt.AD_Table_ID=t.AD_Table_ID)"
                + " INNER JOIN AD_Column ck ON (rt.Column_Key_ID=ck.AD_Column_ID)"
                + " INNER JOIN AD_Column cd ON (rt.Column_Display_ID=cd.AD_Column_ID) "
                + "WHERE rt.AD_Reference_ID=" + AD_Reference_Value_ID.ToString() + ""
                + " AND rt.IsActive='Y' AND t.IsActive='Y'";
            //
            string keyColumn = "", DisplayColumn = "", tableName = "";
            bool isTranslated = false, isValueDisplayed = false;
           
                    tableName = "AD_Ref_List";
            keyColumn = "AD_Ref_List_ID";
                    DisplayColumn = "Name";
                    isTranslated = true;
               
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
                    .Append(" AND ").Append(tableName).Append("_Trl.AD_Language='")
                    .Append(language.GetAD_Language()).Append("')");
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
        /// <param name="AD_Column_ID"></param>
        /// <returns></returns>
        private static VLookUpInfo GetLookup_Acct(Ctx ctx, int AD_Column_ID)
        {
            //	Try cache - assume no language change
            string key = "Acct" + AD_Column_ID;
            VLookUpInfo retValue = null;
            //_sCacheRefTable.TryGetValue(key, out retValue);
            //if (retValue != null)
            //{
            //    s_log.Finest("Cache: " + retValue);
            //    return retValue.Clone();
            //}
            string displayColumn = "Combination";
            string sql = "SELECT C_ValidCombination_ID, NULL, " + displayColumn + ", IsActive FROM C_ValidCombination";
            int zoomWindow = 153;
            Query zoomQuery = new Query("C_ValidCombination");
            //
            retValue = new VLookUpInfo(sql, "C_ValidCombination",
                "C_ValidCombination.C_ValidCombination_ID",
                zoomWindow, zoomWindow, zoomQuery);
            // Display column for Account Control
            retValue.displayColSubQ = displayColumn;
            //_sCacheRefTable.Add(key, retValue.Clone());
            return retValue;
        }


        /// <summary>
        /// crate Lookup_Acct info
        /// </summary>
        /// <param name="AD_Column_ID"></param>
        /// <returns></returns>
        private static VLookUpInfo GetLookup_PContainer(Ctx ctx, int AD_Column_ID)
        {
            //	Try cache - assume no language change
            string key = "pContainer" + AD_Column_ID;
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

   

    /***********************************************************************************/

    //    LookupDisplayColumn
    //Lookup Display Column Value Object

    /**********************************************************************************/
    public class LookupDisplayColumn
    {
        /// <summary>
        ///lookup Column Value Object
        /// </summary>
        /// <param name="columnName">coumn name</param>
        /// <param name="isTranslated">translated</param>
        /// <param name="AD_Reference_ID">display type</param>
        /// <param name="AD_Reference_Value_ID">list/table ref id</param>
        public LookupDisplayColumn(string columnName, bool isTranslated,
            int AD_Reference_ID, int AD_Reference_Value_ID)
        {
            ColumnName = columnName;
            IsTranslated = isTranslated;
            DisplayType = AD_Reference_ID;
            AD_Ref_Val_ID = AD_Reference_Value_ID;
        }	//

        /** Column Name		*/
        public string ColumnName;
        /** Translated		*/
        public bool IsTranslated;
        /** Display Type	*/
        public int DisplayType;
        /** Value Reference	*/
        public int AD_Ref_Val_ID;

        /// <summary>
        ///String Representation
        /// </summary>
        /// <returns>class ifo text</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("LookupDisplayColumn[");
            sb.Append("ColumnName=").Append(ColumnName);
            if (IsTranslated)
                sb.Append(",IsTranslated");
            sb.Append(",DisplayType=").Append(DisplayType);
            if (AD_Ref_Val_ID != 0)
                sb.Append(",AD_Ref_val_ID=").Append(AD_Ref_Val_ID);
            sb.Append("]");
            return sb.ToString();
        }
    }

    /************************************************************************************/

    //  Access Sql Parser Class
    //Parse FROM in SQL WHERE clause

    /************************************************************************************/
    public class AccessSqlParser
    {

        #region declarartion

        /**	FROM String			*/
        private const string FROM = " FROM ";
        private int FROM_LENGTH = FROM.Length;
        private const string WHERE = " WHERE ";
        private const string ON = " ON ";

        ///**	Logger				*/
        private VLogger log = VLogger.GetVLogger(typeof(AccessSqlParser).FullName);

        /**	Original SQL			*/
        private string sqlOriginal;
        /**	SQL Selects			*/
        private string[] v_sql;
        /**	List of Arrays		*/
        //private ArrayList<TableInfo[]>	m_tableInfo = new ArrayList<TableInfo[]>(); 

        /**	List of Arrays		*/
        private List<TableInfo[]> m_tableInfo = new List<TableInfo[]>();

        #endregion

        public AccessSqlParser()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="qry">sql qry string</param>
        public AccessSqlParser(string qry)
        {
            SetSql(qry);
        }

        /// <summary>
        /// Set Sql and Parse it
        /// </summary>
        /// <param name="sql">sql text</param>
        public void SetSql(string sql)
        {
            if (sql == null)
                throw new ArgumentException("No SQL");
            sqlOriginal = sql;
            int index = sqlOriginal.IndexOf("\nFROM ");
            if (index != -1)
                sqlOriginal = sqlOriginal.Replace("\nFROM ", FROM);
            index = sqlOriginal.IndexOf("\nWHERE ");
            if (index != -1)
                sqlOriginal = sqlOriginal.Replace("\nWHERE ", WHERE);
            //
            Parse();
        }	//

        /// <summary>
        /// Parse Original SQL.
        ///	Called from setSql or Constructor.
        /// </summary>
        /// <returns>true if parsed</returns>
        public bool Parse()
        {
            if (sqlOriginal == null || sqlOriginal.Length == 0)
                throw new ArgumentException("No SQL");

            GetSelectStatements();

            //	analyse each select	
            for (int i = 0; i < v_sql.Length; i++)
            {
                TableInfo[] info = GetTableInfo(v_sql[i].Trim());
                m_tableInfo.Add(info);
            }
            //
            if (VLogMgt.IsLevelFinest())
                log.Fine(ToString());
            return m_tableInfo.Count > 0;
        }

        /// <summary>
        ///Parses 	m_sqlOriginal and creates Array of m_sql statements
        /// </summary>
        private void GetSelectStatements()
        {
            String[] sqlIn = new String[] { sqlOriginal };
            String[] sqlOut = null;
            try
            {
                sqlOut = GetSubSQL(sqlIn);
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sqlOriginal, e);
                throw new ArgumentException(sqlOriginal);
            }
            //	a sub-query was found
            while (sqlIn.Length != sqlOut.Length)
            {
                sqlIn = sqlOut;
                try
                {
                    sqlOut = GetSubSQL(sqlIn);
                }
                catch (Exception e)
                {
                    log.Log(Level.SEVERE, sqlOriginal, e);
                    throw new ArgumentException(sqlOut.Length + ": " + sqlOriginal);
                }
            }
            v_sql = sqlOut;
        }

        /// <summary>
        /// Get Sub SQL of sql statements
        /// </summary>
        /// <param name="sqlIn">array of input sql</param>
        /// <returns>array of resulting sql</returns>
        private String[] GetSubSQL(String[] sqlIn)
        {
            List<string> list = new List<string>();
            for (int sqlIndex = 0; sqlIndex < sqlIn.Length; sqlIndex++)
            {
                string sql = sqlIn[sqlIndex];
                int index = sql.IndexOf("(SELECT ", 7);
                while (index != -1)
                {
                    int endIndex = index + 1;
                    int parenthesisLevel = 0;
                    //	search for the end of the sql
                    while (endIndex++ < sql.Length)
                    {
                        char c;
                        char.TryParse(sql.Substring(endIndex, 1), out c);
                        if (c == ')')
                        {
                            if (parenthesisLevel == 0)
                                break;
                            else
                                parenthesisLevel--;
                        }
                        else if (c == '(')
                            parenthesisLevel++;
                    }
                    string subSQL = sql.Substring(index, (endIndex + 1) - index);
                    list.Insert(list.Count, subSQL);
                    //	remove inner SQL (##)
                    sql = sql.Substring(0, index + 1) + "##"
                        + sql.Substring(endIndex);
                    index = sql.IndexOf("(SELECT ", 7);
                }
                list.Insert(list.Count, sql);
                //	last SQL
            }
            string[] retValue = list.ToArray();
            //list.toArray(retValue);
            return retValue;
        }

        /// <summary>
        /// Get Main Sql
        /// </summary>
        /// <returns></returns>
        public String GetMainSql()
        {
            if (v_sql == null)
                return sqlOriginal;

            if (v_sql.Length == 1)
                return v_sql[0];
            for (int i = v_sql.Length - 1; i >= 0; i--)
            {
                if (v_sql[i].Substring(0, 1) != "(")
                    return v_sql[i];
            }
            return "";
        }	//	

        /// <summary>
        ///	Get Table Info.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TableInfo[] GetTableInfo(int index)
        {
            if (index < 0 || index > m_tableInfo.Count)
                return null;
            TableInfo[] retValue = m_tableInfo[index];
            return retValue;
        }	//	getTableInfo

        /// <summary>
        ///Get Table Info for SQL
        /// </summary>
        /// <param name="sql">sql string</param>
        /// <returns>Table info</returns>
        private TableInfo[] GetTableInfo(string sql)
        {
            List<TableInfo> list = new List<TableInfo>();
            if (sql.StartsWith("(") && sql.EndsWith(")"))
                sql = sql.Substring(1, sql.Length - 1);

            int fromIndex = sql.IndexOf(FROM);
            if (fromIndex != sql.LastIndexOf(FROM))
            {
                log.Log(Level.WARNING, "More than one FROM clause - " + sql);
            }
            while (fromIndex != -1)
            {
                string from = sql.Substring(fromIndex + FROM_LENGTH);
                //changed By Lakhwinder
                //int index = from.LastIndexOf(WHERE);	//	end at where
                int index = from.IndexOf(WHERE);
                if (index != -1)
                    from = from.Substring(0, index);
                from = from.Replace(" AS ", " ");
                from = from.Replace(" as ", " ");
                from = from.Replace(" INNER JOIN ", ", ");
                from = from.Replace(" LEFT OUTER JOIN ", ", ");
                from = from.Replace(" RIGHT OUTER JOIN ", ", ");
                from = from.Replace(" FULL JOIN ", ", ");
                //	Remove ON clause - assumes that there is no IN () in the clause
                index = from.IndexOf(ON);
                while (index != -1)
                {
                    int indexClose = from.IndexOf(')');		//	does not catch "IN (1,2)" in ON
                    int indexNextOn = from.IndexOf(ON, index + 4);
                    if (indexNextOn != -1)
                        indexClose = from.LastIndexOf(')', indexNextOn);
                    if (indexClose != -1)
                        from = from.Substring(0, index) + from.Substring(indexClose + 1);
                    else
                    {
                        log.Log(Level.WARNING, "Could not remove ON " + from);
                        break;
                    }
                    index = from.IndexOf(ON);
                }

                //  log.Fine("getTableInfo - " + from);

                string[] arr = from.Split(',');
                //StringTokenizer tableST = new StringTokenizer(from, ",");
                for (int k = 0; k < arr.Length; k++)
                {
                    string tableString = arr[k].Trim();
                    TableInfo tableInfo = null;
                    if (tableString.Contains(" "))
                    {
                        string[] arrsp = tableString.Split(' ');
                        tableInfo = new TableInfo(arrsp[0], arrsp[1]);
                    }
                    else
                    {
                        tableInfo = new TableInfo(tableString);
                    }
                    list.Add(tableInfo);
                }
                sql = sql.Substring(0, fromIndex);
                fromIndex = sql.LastIndexOf(FROM);
            }
            TableInfo[] retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        ///Get index of main Statements
        /// </summary>
        /// <returns>index of main statement or -1 if not found</returns>
        public int GetMainSqlIndex()
        {
            if (v_sql == null)
                return -1;
            else if (v_sql.Length == 1)
                return 0;
            for (int i = v_sql.Length - 1; i >= 0; i--)
            {
                if (!v_sql[i].StartsWith("("))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 	Table Info VO
        /// </summary>
        public class TableInfo
        {
            /// <summary>
            /// 	Constructor
            /// </summary>
            /// <param name="tableName"></param>
            /// <param name="synonym"></param>
            public TableInfo(string tableName, string synonym)
            {
                m_tableName = tableName;
                m_synonym = synonym;
            }	//	TableInfo

            /// <summary>
            ///	Short Constuctor - no syn
            /// </summary>
            /// <param name="tableName"></param>
            public TableInfo(string tableName)
                : this(tableName, null)
            {
            }	//	TableInfo

            private string m_tableName;
            private string m_synonym;

            /// <summary>
            ///Get Table Synonym
            /// </summary>
            /// <returns></returns>
            public string GetSynonym()
            {
                if (m_synonym == null)
                    return "";
                return m_synonym;
            }	//	getSynonym

            /// <summary>
            /// Get tableName
            /// </summary>
            /// <returns></returns>
            public string GetTableName()
            {
                return m_tableName;
            }	//	getTableName

            /// <summary>
            /// String Representation
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder(m_tableName);
                if (GetSynonym().Length > 0)
                    sb.Append("=").Append(m_synonym);
                return sb.ToString();
            }

        }	//	TableInfo
    }
}
