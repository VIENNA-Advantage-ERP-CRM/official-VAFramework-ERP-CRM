/********************************************************
 * Module Name    :     Report
 * Purpose        :     Generate Reports
 * Author         :     Jagmohan Bhatt
 * Date           :     1-July-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.Classes;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VAdvantage.Login;
using VAdvantage.Logging;
using VAModelAD.DataBase;

namespace VAdvantage.Print
{
    /// <summary>
    /// Process the repoting data
    /// </summary>
    public class DataEngine
    {
        private static VLogger log = VLogger.GetVLogger(typeof(DataEngine).FullName);
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="language">current language</param>
        public DataEngine(Language language)
        {
            //_isParent = isParent;
            if (language != null)
                _language = language;
        }	//	DataEngine

        /**Is Parent    */
        //private bool _isParent;


        /**	Synonym							*/
        private String _synonym = "A";

        /**	Default Language				*/
        private Language _language = Language.GetLoginLanguage();

        /**	Start Time						*/
        private long _startTime = CommonFunctions.CurrentTimeMillis();
        /** Running Total after .. lines	*/
        private int _runningTotalLines = -1;
        /** Print String					*/
        private String _runningTotalString = null;

        /** Key Indicator in Report			*/
        public const String KEY = "*";

        private PrintInfo _printInfo = null;

        public void SetPInfo(PrintInfo info)
        {
            _printInfo = info;
        }

        /// <summary>
        /// Load Data
        /// </summary>
        /// <param name="ctx">current context</param>
        /// <param name="format">format</param>
        /// <param name="query">query</param>
        /// <returns></returns>
        public PrintData GetPrintData(Ctx ctx, MPrintFormat format, Query query)
        {
            if (format == null)
                throw new Exception("No print format");
            String tableName = null;
            String reportName = format.GetName();
            //    //
            if (format.GetAD_ReportView_ID() != 0)
            {
                String sql = "SELECT t.AD_Table_ID, t.TableName, rv.Name "
                    + "FROM AD_Table t"
                    + " INNER JOIN AD_ReportView rv ON (t.AD_Table_ID=rv.AD_Table_ID) "
                    + "WHERE rv.AD_ReportView_ID='" + format.GetAD_ReportView_ID() + "'";	//	1
                IDataReader dr = null;
                try
                {
                    dr = DataBase.DB.ExecuteReader(sql);
                    while (dr.Read())
                    {
                        tableName = dr[1].ToString();
                        reportName = dr[2].ToString();
                    }
                    dr.Close();
                }
                catch (Exception ex)
                {
                    log.Severe(ex.ToString());
                    if (dr != null)
                    {
                        dr.Close();
                    }
                    return null;
                }
                finally
                {
                    dr.Close();
                }
            }
            else
            {
                String sql = "SELECT TableName FROM AD_Table WHERE AD_Table_ID='" + format.GetAD_Table_ID() + "'";	//	#1
                IDataReader dr = null;
                try
                {
                    dr = DataBase.DB.ExecuteReader(sql);
                    while (dr.Read())
                    {
                        tableName = dr[0].ToString();
                    }
                    dr.Close();
                }
                catch (Exception e1)
                {
                    if (dr != null)
                    {
                        dr.Close();
                    }
                    log.Severe(e1.ToString());
                    return null;
                }
                finally
                {
                    dr.Close();
                }
            }

            if (tableName == null)
            {
                return null;
            }
            if (format.IsTranslationView() && tableName.ToLower().EndsWith("_v"))	//	_vt not just _v
                tableName += "t";
            format.SetTranslationViewQuery(query);

            PrintData pd = GetPrintDataInfo(ctx, format, query, reportName, tableName);
            if (pd == null)
                return null;
            LoadPrintData(ctx, pd, format);
            return pd;
            //return null;
        }	//	GetPrintData


        private PrintDataGroup _group = new PrintDataGroup();

        /// <summary>
        /// Get Print data information
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="format">format</param>
        /// <param name="query">query</param>
        /// <param name="reportName">name of the report</param>
        /// <param name="tableName">name of the table</param>
        /// <returns></returns>
        private PrintData GetPrintDataInfo(Ctx ctx, MPrintFormat format, Query query,
            String reportName, String tableName)
        {
            _startTime = CommonFunctions.CurrentTimeMillis();

            List<PrintDataColumn> columns = new List<PrintDataColumn>();
            _group = new PrintDataGroup();



            //    //	Order Columns (identifed by non zero/null SortNo)
            int[] orderAD_Column_IDs = format.GetOrderAD_Column_IDs();

            int AD_Tab_ID = 0;
            if (format.GetAD_Tab_ID() > 0)
            {
                AD_Tab_ID = format.GetAD_Tab_ID();
            }

            List<String> orderColumns = new List<String>(orderAD_Column_IDs.Length);
            for (int i = 0; i < orderAD_Column_IDs.Length; i++)
            {
                //log.finest("Order AD_Column_ID=" + orderAD_Column_IDs[i]);
                orderColumns.Add("");		//	initial value overwritten with fully qualified name
            }

            //    //	Direct SQL w/o Reference Info
            StringBuilder sqlSELECT = new StringBuilder("SELECT ");
            StringBuilder sqlFROM = new StringBuilder(" FROM ");
            sqlFROM.Append(tableName);
            StringBuilder sqlGROUP = new StringBuilder(" GROUP BY ");
            string sqlSelfTableRef = "";
            //    //
            bool IsGroupedBy = false;
            //    //
            String sql = "SELECT c.AD_Column_ID,c.ColumnName,"				//	1..2
                + "c.AD_Reference_ID,c.AD_Reference_Value_ID,"				//	3..4
                + "c.FieldLength,c.IsMandatory,c.IsKey,c.IsParent,"			//	5..8
                + "COALESCE(rvc.IsGroupFunction,'N'),rvc.FunctionColumn,"	//	9..10
                + "pfi.IsGroupBy,pfi.IsSummarized,pfi.IsAveraged,pfi.IsCounted, "	//	11..14
                + "pfi.IsPrinted,pfi.SortNo,pfi.IsPageBreak, "				//	15..17
                + "pfi.IsMinCalc,pfi.IsMaxCalc, "							//	18..19
                + "pfi.IsRunningTotal,pfi.RunningTotalLines, "				//	20..21
                + "pfi.IsVarianceCalc, pfi.IsDeviationCalc, "				//	22..23
                + "c.ColumnSQL,pfi.ISASCENDING "											//	24
                + "FROM AD_PrintFormat pf"
                + " INNER JOIN AD_PrintFormatItem pfi ON (pf.AD_PrintFormat_ID=pfi.AD_PrintFormat_ID)"
                + " INNER JOIN AD_Column c ON (pfi.AD_Column_ID=c.AD_Column_ID)"
                + " LEFT OUTER JOIN AD_ReportView_Col rvc ON (pf.AD_ReportView_ID=rvc.AD_ReportView_ID AND c.AD_Column_ID=rvc.AD_Column_ID) "
                + "WHERE pf.AD_PrintFormat_ID='" + format.Get_ID() + "'"					//	#1
                + " AND pfi.IsActive='Y' AND (pfi.IsPrinted='Y' OR c.IsKey='Y' OR pfi.SortNo > 0) "
                + "ORDER BY pfi.IsPrinted DESC, pfi.SeqNo";		//	Functions are put in first column
            IDataReader dr = null;
            try
            {

                dr = DataBase.DB.ExecuteReader(sql);

                _synonym = "A";		//	synonym
                while (dr.Read())
                {
                    //            //	Get Values from record
                    int AD_Column_ID = Utility.Util.GetValueOfInt(dr[0].ToString());
                    String ColumnName = dr[1].ToString();
                    String ColumnSQL = dr[23].ToString();
                    if (ColumnSQL == null)
                        ColumnSQL = "";
                    int AD_Reference_ID = Utility.Util.GetValueOfInt(dr[2].ToString());
                    int AD_Reference_Value_ID = Utility.Util.GetValueOfInt(dr[3].ToString());
                    //  ColumnInfo
                    int FieldLength = Utility.Util.GetValueOfInt(dr[4].ToString());
                    bool IsMandatory = "Y".Equals(dr[5].ToString());
                    bool IsKey = "Y".Equals(dr[6].ToString());
                    bool IsParent = "Y".Equals(dr[7].ToString());
                    //  SQL GroupBy
                    bool IsGroupFunction = "Y".Equals(dr[8].ToString());
                    if (IsGroupFunction)
                        IsGroupedBy = true;
                    String FunctionColumn = dr[9].ToString();
                    if (FunctionColumn == null)
                        FunctionColumn = "";
                    //	Breaks/Column Functions
                    if ("Y".Equals(dr[10].ToString()))
                        _group.AddGroupColumn(ColumnName);
                    if ("Y".Equals(dr[11].ToString()))
                        _group.AddFunction(ColumnName, PrintDataFunction.F_SUM);
                    if ("Y".Equals(dr[12].ToString()))
                        _group.AddFunction(ColumnName, PrintDataFunction.F_MEAN);
                    if ("Y".Equals(dr[13].ToString()))
                        _group.AddFunction(ColumnName, PrintDataFunction.F_COUNT);
                    if ("Y".Equals(dr[17].ToString()))	//	IsMinCalc
                        _group.AddFunction(ColumnName, PrintDataFunction.F_MIN);
                    if ("Y".Equals(dr[18].ToString()))	//	IsMaxCalc
                        _group.AddFunction(ColumnName, PrintDataFunction.F_MAX);
                    if ("Y".Equals(dr[21].ToString()))	//	IsVarianceCalc
                        _group.AddFunction(ColumnName, PrintDataFunction.F_VARIANCE);
                    if ("Y".Equals(dr[22].ToString()))	//	IsDeviationCalc
                        _group.AddFunction(ColumnName, PrintDataFunction.F_DEVIATION);
                    if ("Y".Equals(dr[19].ToString()))	//	isRunningTotal
                        //	RunningTotalLines only once - use max
                        _runningTotalLines = Math.Max(_runningTotalLines, Utility.Util.GetValueOfInt(dr[20].ToString()));

                    //	General Info
                    bool IsPrinted = "Y".Equals(dr[14].ToString());
                    int SortNo = Utility.Util.GetValueOfInt(dr[15].ToString());
                    bool isPageBreak = "Y".Equals(dr[16].ToString());
                    bool isAsc = "Y".Equals(dr["ISASCENDING"].ToString());

                    //	Fully qualified Table.Column for ordering
                    String orderName = tableName + "." + ColumnName;
                    PrintDataColumn pdc = null;

                    //  -- Key --
                    if (IsKey)
                    {
                        //	=>	Table.Column,
                        sqlSELECT.Append(tableName).Append(".").Append(ColumnName).Append(",");
                        sqlGROUP.Append(tableName).Append(".").Append(ColumnName).Append(",");
                        pdc = new PrintDataColumn(AD_Column_ID, ColumnName, AD_Reference_ID, FieldLength, KEY, isPageBreak);	//	KeyColumn
                    }
                    else if (!IsPrinted)	//	not printed Sort Columns
                    { }
                    //	-- Parent, TableDir (and unqualified Search) --
                    else if (IsParent
                            || AD_Reference_ID == DisplayType.TableDir
                            || (AD_Reference_ID == DisplayType.Search && AD_Reference_Value_ID == 0)
                        )
                    {
                        if (ColumnSQL.Length > 0)
                        {
                            //log.warning(ColumnName + " - virtual column not allowed with this Display type");
                            continue;
                        }
                        //  Creates Embedded SQL in the form
                        //  SELECT ColumnTable.Name FROM ColumnTable WHERE TableName.ColumnName=ColumnTable.ColumnName
                        String eSql = VLookUpFactory.GetLookup_TableDirEmbed(_language, ColumnName, tableName);

                        //	TableName
                        String table = ColumnName;
                        if (table.EndsWith("_ID"))
                            table = table.Substring(0, table.Length - 3);
                        //  DisplayColumn
                        String display = ColumnName;
                        //	=> (..) AS AName, Table.ID,
                        sqlSELECT.Append("(").Append(eSql).Append(") AS ").Append(_synonym).Append(display).Append(",")
                            .Append(tableName).Append(".").Append(ColumnName).Append(",");
                        sqlGROUP.Append(_synonym).Append(display).Append(",")
                            .Append(tableName).Append(".").Append(ColumnName).Append(",");
                        orderName = _synonym + display;
                        //
                        pdc = new PrintDataColumn(AD_Column_ID, ColumnName, AD_Reference_ID, FieldLength, orderName, isPageBreak);
                        SynonymNext();
                    }
                    //	-- Table --
                    else if (AD_Reference_ID == DisplayType.Table
                            || (AD_Reference_ID == DisplayType.Search && AD_Reference_Value_ID != 0)
                        )
                    {
                        if (ColumnSQL.Length > 0)
                        {
                            continue;
                        }
                        TableReference tr = GetTableReference(AD_Reference_Value_ID);
                        String display = tr.DisplayColumn;
                        //	=> A.Name AS AName, Table.ID,
                        if (tr.IsValueDisplayed)
                            sqlSELECT.Append(_synonym).Append(".Value||'-'||");
                        sqlSELECT.Append(_synonym).Append(".").Append(display);
                        sqlSELECT.Append(" AS ").Append(_synonym).Append(display).Append(",")
                            .Append(tableName).Append(".").Append(ColumnName).Append(",");
                        sqlGROUP.Append(_synonym).Append(".").Append(display).Append(",")
                            .Append(tableName).Append(".").Append(ColumnName).Append(",");
                        orderName = _synonym + display;

                        //	=> x JOIN table A ON (x.KeyColumn=A.Key)
                        if (IsMandatory)
                        {
                            sqlFROM.Append(" INNER JOIN ");
                        }
                        else
                        {
                            sqlFROM.Append(" LEFT OUTER JOIN ");
                        }

                        if (tr.TableName.Equals(tableName))
                        {
                            sqlSelfTableRef = _synonym + "." + tr.KeyColumn;
                        }
                        sqlFROM.Append(tr.TableName).Append(" ").Append(_synonym).Append(" ON (");
                        if (!ColumnName.EndsWith("_ID") && DatabaseType.IsPostgre && !ColumnName.ToUpper().Equals("AD_LANGUAGE") && !ColumnName.ToUpper().Equals("ENTITYTYPE"))
                        {
                            sqlFROM.Append("TO_NUMBER(").Append(tableName).Append(".").Append(ColumnName).Append(",'99G99')").Append("=");
                            //TO_NUMBER()
                        }
                        else
                        {
                            sqlFROM.Append(tableName).Append(".").Append(ColumnName).Append("=");
                        }
                        sqlFROM.Append(_synonym).Append(".").Append(tr.KeyColumn).Append(")");
                        //
                        pdc = new PrintDataColumn(AD_Column_ID, ColumnName, AD_Reference_ID, FieldLength, orderName, isPageBreak);
                        SynonymNext();
                    }

                    //	-- List or Button with ReferenceValue --
                    else if (AD_Reference_ID == DisplayType.List
                        || (AD_Reference_ID == DisplayType.Button && AD_Reference_Value_ID != 0))
                    {
                        if (ColumnSQL.Length > 0)
                        {
                            //log.warning(ColumnName + " - virtual column not allowed with this Display type");
                            continue;
                        }
                        if (Env.IsBaseLanguage(_language, "AD_Ref_List"))
                        {
                            //	=> A.Name AS AName,
                            sqlSELECT.Append(_synonym).Append(".Name AS ").Append(_synonym).Append("Name,");
                            sqlGROUP.Append(_synonym).Append(".Name,");
                            orderName = _synonym + "Name";
                            //	=> x JOIN AD_Ref_List A ON (x.KeyColumn=A.Value AND A.AD_Reference_ID=123)
                            if (IsMandatory)
                                sqlFROM.Append(" INNER JOIN ");
                            else
                                sqlFROM.Append(" LEFT OUTER JOIN ");
                            sqlFROM.Append("AD_Ref_List ").Append(_synonym).Append(" ON (")
                                .Append(tableName).Append(".").Append(ColumnName).Append("=").Append(_synonym).Append(".Value")
                                .Append(" AND ").Append(_synonym).Append(".AD_Reference_ID=").Append(AD_Reference_Value_ID).Append(")");
                        }
                        else
                        {
                            //	=> A.Name AS AName,
                            sqlSELECT.Append(_synonym).Append(".Name AS ").Append(_synonym).Append("Name,");
                            sqlGROUP.Append(_synonym).Append(".Name,");
                            orderName = _synonym + "Name";

                            //	LEFT OUTER JOIN AD_Ref_List XA ON (AD_Table.EntityType=XA.Value AND XA.AD_Reference_ID=245)
                            //	LEFT OUTER JOIN AD_Ref_List_Trl A ON (XA.AD_Ref_List_ID=A.AD_Ref_List_ID AND A.AD_Language='de_DE')
                            if (IsMandatory)
                                sqlFROM.Append(" INNER JOIN ");
                            else
                                sqlFROM.Append(" LEFT OUTER JOIN ");
                            sqlFROM.Append(" AD_Ref_List X").Append(_synonym).Append(" ON (")
                                .Append(tableName).Append(".").Append(ColumnName).Append("=X")
                                .Append(_synonym).Append(".Value AND X").Append(_synonym).Append(".AD_Reference_ID=").Append(AD_Reference_Value_ID)
                                .Append(")");
                            if (IsMandatory)
                                sqlFROM.Append(" INNER JOIN ");
                            else
                                sqlFROM.Append(" LEFT OUTER JOIN ");
                            sqlFROM.Append(" AD_Ref_List_Trl ").Append(_synonym).Append(" ON (X")
                                .Append(_synonym).Append(".AD_Ref_List_ID=").Append(_synonym).Append(".AD_Ref_List_ID")
                                .Append(" AND ").Append(_synonym).Append(".AD_Language='").Append(_language.GetAD_Language()).Append("')");
                        }
                        // 	TableName.ColumnName,
                        sqlSELECT.Append(tableName).Append(".").Append(ColumnName).Append(",");
                        pdc = new PrintDataColumn(AD_Column_ID, ColumnName, AD_Reference_ID, FieldLength, orderName, isPageBreak);
                        SynonymNext();
                    }

                    //  -- Special Lookups --
                    else if (AD_Reference_ID == DisplayType.Location
                        || AD_Reference_ID == DisplayType.Account
                        || AD_Reference_ID == DisplayType.Locator
                        || AD_Reference_ID == DisplayType.PAttribute
                    )
                    {
                        if (ColumnSQL.Length > 0)
                        {
                            //log.warning(ColumnName + " - virtual column not allowed with this Display type");
                            continue;
                        }
                        //	TableName, DisplayColumn
                        String table = "";
                        String key = "";
                        String display = "";
                        String synonym = null;
                        //
                        if (AD_Reference_ID == DisplayType.Location)
                        {
                            table = "C_Location";
                            key = "C_Location_ID";
                            display = "City||'.'";	//	in case City is empty
                            synonym = "Address";
                        }
                        else if (AD_Reference_ID == DisplayType.Account)
                        {
                            table = "C_ValidCombination";
                            key = "C_ValidCombination_ID";
                            display = "Combination";
                        }
                        else if (AD_Reference_ID == DisplayType.Locator)
                        {
                            table = "M_Locator";
                            key = "M_Locator_ID";
                            display = "Value";
                        }
                        else if (AD_Reference_ID == DisplayType.PAttribute)
                        {
                            table = "M_AttributeSetInstance";
                            key = "M_AttributeSetInstance_ID";
                            display = "Description";
                            //if (CLogMgt.IsLevelFinest())
                            //jz display += "||'{'||" + _synonym + "._AttributeSetInstance_ID||'}'";
                            //display += "||'{'||" + DataBase.DB.TO_CHAR(_synonym + "._AttributeSetInstance_ID", DisplayType.Number, Env.GetAD_Language(Env.GetContext())) + "||'}'";
                            synonym = "Description";
                        }
                        if (synonym == null)
                            synonym = display;

                        //	=> A.Name AS AName, table.ID,
                        sqlSELECT.Append(_synonym).Append(".").Append(display).Append(" AS ")
                            .Append(_synonym).Append(synonym).Append(",")
                            .Append(tableName).Append(".").Append(ColumnName).Append(",");
                        sqlGROUP.Append(_synonym).Append(".").Append(synonym).Append(",")
                            .Append(tableName).Append(".").Append(ColumnName).Append(",");
                        orderName = _synonym + synonym;
                        //	=> x JOIN table A ON (table.ID=A.Key)
                        if (IsMandatory)
                            sqlFROM.Append(" INNER JOIN ");
                        else
                            sqlFROM.Append(" LEFT OUTER JOIN ");
                        sqlFROM.Append(table).Append(" ").Append(_synonym).Append(" ON (")
                            .Append(tableName).Append(".").Append(ColumnName).Append("=")
                            .Append(_synonym).Append(".").Append(key).Append(")");
                        //
                        pdc = new PrintDataColumn(AD_Column_ID, ColumnName, AD_Reference_ID, FieldLength, orderName, isPageBreak);
                        SynonymNext();
                    }

                    //	-- Standard Column --
                    else
                    {
                        int index = FunctionColumn.IndexOf("@");
                        StringBuilder sb = new StringBuilder();
                        if (ColumnSQL != null && ColumnSQL.Length > 0)
                        {
                            //	=> ColumnSQL AS ColumnName
                            sb.Append(ColumnSQL);
                            sqlSELECT.Append(sb).Append(" AS ").Append(ColumnName).Append(",");
                            if (!IsGroupFunction)
                                sqlGROUP.Append(sb).Append(",");
                            orderName = ColumnName;		//	no prefix for synonym
                        }
                        else if (index == -1)
                        {
                            MColumn col = new MColumn(ctx, AD_Column_ID, null);
                            string obscureType = col.GetObscureType();
                            if (obscureType != null && obscureType.Length > 0 && !MRole.GetDefault(ctx).IsColumnAccess(col.GetAD_Table_ID(),AD_Column_ID,false))
                            {
                                sb.Append(DBFunctionCollections.GetObscureColumn(obscureType, tableName, ColumnName)).Append(",");
                            }
                            else
                            {
                                //	=> Table.Column,
                                sb.Append(tableName).Append(".").Append(ColumnName).Append(",");
                            }
                            sqlSELECT.Append(sb);
                            if (!IsGroupFunction)
                                sqlGROUP.Append(sb).Append(",");
                        }
                        else
                        {
                            //  => Function(Table.Column) AS Column   -- function has @ where column name goes
                            sb.Append(FunctionColumn.Substring(0, index))
                                //	If I eg entered sum(amount)  as function column in the report view the query would look like:
                                //	Tablename.amountsum(amount), after removing the line below I Get the wanted result. The original query column (tablename.column) is replaced by the function column entered in the report view window.
                                //	.Append(tableName).Append(".").Append(ColumnName)	// xxxxxx
                                .Append(FunctionColumn.Substring(index + 1));
                            sqlSELECT.Append(sb).Append(" AS ").Append(ColumnName).Append(",");
                            if (!IsGroupFunction)
                                sqlGROUP.Append(sb).Append(",");
                            orderName = ColumnName;		//	no prefix for synonym
                        }
                        pdc = new PrintDataColumn(AD_Column_ID, ColumnName,
                            AD_Reference_ID, FieldLength, ColumnName, isPageBreak);
                    }

                    //	Order Sequence - Overwrite order column name
                    for (int i = 0; i < orderAD_Column_IDs.Length; i++)
                    {
                        if (AD_Column_ID == orderAD_Column_IDs[i])
                        {
                            orderColumns.RemoveAt(i);
                            if (isAsc)
                            {
                                orderColumns.Insert(i, orderName);
                                break;
                            }
                            else
                            {
                                orderColumns.Insert(i, orderName + " DESC ");
                                break;
                            }
                        }
                    }

                    //
                    if (pdc == null || (!IsPrinted && !IsKey))
                        continue;

                    columns.Add(pdc);
                }	//	for all Fields in Tab
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }

                log.Log(Level.SEVERE, "SQL=" + sql + " - ID=" + format.Get_ID(), e);
            }
            finally
            {
                dr.Close();
            }

            if (columns.Count == 0)
            {
                //log.log(Level.SEVERE, "No Colums - Delete Report Format " + reportName + " and start again");
                //log.finest("No Colums - SQL=" + sql + " - ID=" + format.Get_ID());
                return null;
            }

            bool hasLevelNo = false;
            if (tableName.StartsWith("T_Report"))
            {
                hasLevelNo = true;
                if (sqlSELECT.ToString().IndexOf("LevelNo") == -1)
                    sqlSELECT.Append("LevelNo,");
            }

            /**
             *	Assemble final SQL - delete last SELECT ','
             */
            StringBuilder finalSQL = new StringBuilder();
            finalSQL.Append(sqlSELECT.ToString().Substring(0, sqlSELECT.Length - 1))
                .Append(sqlFROM);

            //	WHERE clause
            if (tableName.StartsWith("T_Report"))
            {
                finalSQL.Append(" WHERE ");
                for (int i = 0; i < query.GetRestrictionCount(); i++)
                {
                    String q = query.GetWhereClause(i);
                    if (q.IndexOf("AD_PInstance_ID") != -1)	//	ignore all other Parameters
                        finalSQL.Append(q);
                }	//	for all restrictions
            }
            else
            {
                //	User supplied Where Clause
                if (query != null && query.IsActive())
                {
                    finalSQL.Append(" WHERE ");
                    if (!query.GetTableName().Equals(tableName))
                        query.SetTableName(tableName);


                    string wherequery = query.GetWhereClause(true);

                    // wherequery = AppendTableNames(wherequery, tableName);

                    finalSQL.Append(wherequery);
                }
                //	Access Restriction
                MRole role = MRole.GetDefault(ctx, false);
                if (role.GetAD_Role_ID() == 0 && !Ini.IsClient())
                { }
                else
                    finalSQL = new StringBuilder(role.AddAccessSQL(finalSQL.ToString(),
                        tableName, MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO));
            }

            if (sqlSelfTableRef.Length > 0)
            {
                finalSQL = finalSQL.Replace(sqlSelfTableRef + " NOT IN", "nvl(" + sqlSelfTableRef + ",0) NOT IN");
            }

            if (AD_Tab_ID > 0)
            {
                MTab tab = new MTab(ctx, AD_Tab_ID, null);
                string where = tab.GetWhereClause();
                if (where != null && where.Length > 0)
                {
                    if (where.Contains("@"))
                    {
                        where = Env.ParseContext(ctx, Util.GetValueOfInt(ctx.GetContext("SelectedWindow")), where, false);
                    }

                    //  where = AppendTableNames(where, tableName);
                    finalSQL.Append(" AND " + where + " ");
                }
            }

            MTable tableNaam = MTable.Get(ctx, tableName);
            MColumn cols = tableNaam.GetColumn("AD_PInstance_ID");
            if (cols != null && _printInfo.GetAD_PInstance_ID() > 0)
            {
                if (finalSQL.ToString().ToUpper().Contains(" WHERE "))
                {
                    finalSQL.Append(" AND AD_PInstance_ID=" + _printInfo.GetAD_PInstance_ID() + " ");
                }
                else
                {
                    finalSQL.Append(" WHERE AD_PInstance_ID=" + _printInfo.GetAD_PInstance_ID() + " ");
                }
            }

            //  Group By
            if (IsGroupedBy)
                finalSQL.Append(sqlGROUP.ToString().Substring(0, sqlGROUP.Length - 1));    //  last ,

            //	Add ORDER BY clause
            if (orderColumns != null)
            {
                for (int i = 0; i < orderColumns.Count; i++)
                {
                    if (i == 0)
                        finalSQL.Append(" ORDER BY ");
                    else
                        finalSQL.Append(",");
                    String by = (String)orderColumns[i];
                    if (by == null || by.Length == 0)
                        by = (i + 1).ToString();
                    finalSQL.Append(by);
                }
            }	//	order by


            //	Print Data
            PrintData pd = new PrintData(ctx, reportName);
            PrintDataColumn[] info = new PrintDataColumn[columns.Count];
            info = columns.ToArray();		//	column order is is _synonymc with SELECT column position
            pd.SetColumnInfo(info);
            pd.SetTableName(tableName);
            pd.SetSQL(finalSQL.ToString());
            pd.SetHasLevelNo(hasLevelNo);

            return pd;
        }	//GetPrintDataInfo

        private string AppendTableNames(string originalquery, string tablename)
        {
            try
            {
                // StringBuilder finalQuery = new StringBuilder();
                bool isSplitted = false;
                string query = originalquery;
                if (query.Contains(" and ") || query.Contains(" AND "))
                {
                    isSplitted = true;
                    string[] and;
                    if (query.Contains(" and "))
                    {
                        and = new string[1] { " and " };
                    }
                    else
                    {
                        and = new string[1] { " AND " };
                    }

                    string[] columns = query.Split(and, StringSplitOptions.None);
                    for (int i = 0; i < columns.Count(); i++)
                    {
                        if (!columns.ElementAt(i).Contains('.') && !columns.ElementAt(i).ToLower().Contains("to_date(") && !columns.ElementAt(i).ToLower().Contains("between"))
                        {
                            // finalQuery.Append(columns.ElementAt(i));

                            string colAtEle = columns.ElementAt(i);
                            if (colAtEle.Contains("("))
                            {
                                string colName = (colAtEle.Substring(colAtEle.IndexOf("(") + 1, colAtEle.IndexOf(")") - (colAtEle.IndexOf("(") + 1))).Trim();

                                colName = colAtEle.Replace(colName, tablename + "." + colName);

                                originalquery = originalquery.Replace(colAtEle, colName);
                            }
                            else
                            {
                                originalquery = originalquery.Replace(colAtEle, tablename + "." + colAtEle);
                            }

                        }
                        else if (!columns.ElementAt(i).Contains('.') && columns.ElementAt(i).ToLower().Contains("to_date("))
                        {
                            string colAtEle = columns.ElementAt(i);
                            if (colAtEle.ToLower().Contains("between"))
                            {
                                originalquery = originalquery.Replace(colAtEle, tablename + "." + colAtEle);
                            }
                            // originalquery = originalquery.Replace(colAtEle, tablename + "." + colAtEle);
                        }
                    }





                }

                if (query.Contains(" or ") || query.Contains(" OR "))
                {
                    isSplitted = true;
                    string[] or;
                    if (query.Contains(" or "))
                    {
                        or = new string[1] { " or " };
                    }

                    else
                    {
                        or = new string[1] { " OR " };
                    }

                    string[] columns = query.Split(or, StringSplitOptions.None);
                    for (int i = 0; i < columns.Count(); i++)
                    {
                        if (!columns.ElementAt(i).Contains('.') && !columns.ElementAt(i).ToLower().Contains("to_date("))
                        {
                            string colAtEle = columns.ElementAt(i);
                            if (colAtEle.Contains("("))
                            {
                                string colName = (colAtEle.Substring(colAtEle.IndexOf("(") + 1, colAtEle.IndexOf(")") - (colAtEle.IndexOf("(") + 1))).Trim();

                                colName = colAtEle.Replace(colName, tablename + "." + colName);

                                originalquery = originalquery.Replace(colAtEle, colName);
                            }
                            else
                            {
                                originalquery = originalquery.Replace(colAtEle, tablename + "." + colAtEle);
                            }
                        }
                        else if (!columns.ElementAt(i).Contains('.') && columns.ElementAt(i).ToLower().Contains("to_date("))
                        {
                            string colAtEle = columns.ElementAt(i);
                            if (colAtEle.ToLower().Contains("between"))
                            {
                                originalquery = originalquery.Replace(colAtEle, tablename + "." + colAtEle);
                            }
                        }
                    }
                }

                if (!isSplitted)
                {
                    if (!query.Contains('.'))
                    {
                        if (query.Contains("("))
                        {
                            string colName = (query.Substring(query.IndexOf("(") + 1, query.IndexOf(")") - (query.IndexOf("(") + 1))).Trim();

                            colName = query.Replace(colName, tablename + "." + colName);

                            originalquery = originalquery.Replace(query, colName);
                        }
                        else
                        {
                            originalquery = tablename + "." + originalquery;//error resolved  to handle group by case to lower case in report view window 
                        }
                    }
                }
                return originalquery;
            }
            catch
            {
                return originalquery;
            }

        }


        /// <summary>
        /// Load Print data
        /// </summary>
        /// <param name="pd">print data</param>
        /// <param name="format">format</param>
        private void LoadPrintData(Ctx ctx, PrintData pd, MPrintFormat format)
        {
            //	Translate Spool Output
            bool translateSpool = pd.GetTableName().Equals("T_Spool");
            _runningTotalString = Utility.Msg.GetMsg(Env.GetContext(), "RunningTotal", true);
            int rowNo = 0;
            PrintDataColumn pdc = null;
            bool hasLevelNo = pd.HasLevelNo();
            int levelNo = 0;
            //
            //IDataReader dr = null;
            DataSet ds = null;
            try
            {
                //lakhwinder
                //Temporary Solution for large reports Hang Problem
                //try
                //{
                //    dr = DataBase.DB.ExecuteReader("SELECT * FROM ( " + pd.GetSQL() + " )  WHERE ROWNUM<1001");
                //}
                //catch
                //{
                //    dr = null;
                //}


                //lakhwinder
                //Implement Grid Report Paging
                try
                {
                    if (format.IsGridReport)
                    {
                        int pageSize = Util.GetValueOfInt(ctx.GetContext("#REPORT_PAGE_SIZE")); //500;

                        string sql = "SELECT COUNT(*) FROM ( " + pd.GetSQL() + " )";
                        if (DataBase.DatabaseType.IsPostgre)
                        {
                            sql += " as SQLQuery ";
                        }


                        int totalRec = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        format.TotalPage = (totalRec % pageSize) == 0 ? (totalRec / pageSize) : ((totalRec / pageSize) + 1);
                        ds = VAdvantage.DataBase.DB.GetDatabase().ExecuteDatasetPaging(pd.GetSQL(), format.PageNo, pageSize, 0);

                    }
                    //dr = DataBase.DB.ExecuteReader("SELECT * FROM ( " + pd.GetSQL() + " )  WHERE ROWNUM<1001");
                }
                catch
                {
                    ds = null;
                }

                if (ds == null)
                {
                    //dr = DataBase.DB.ExecuteReader(pd.GetSQL());
                    ds = DataBase.DB.ExecuteDataset(pd.GetSQL());
                }
                //DataTable dt = new DataTable();
                //dt.Load(dr);
                //	Row Loop
                for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                {
                    if (hasLevelNo)
                        levelNo = Utility.Util.GetValueOfInt(ds.Tables[0].Rows[x]["LevelNo"].ToString());
                    else
                        levelNo = 0;
                    //	Check Group Change ----------------------------------------
                    if (_group.GetGroupColumnCount() > 1)	//	one is GRANDTOTAL_
                    {
                        //	Check Columns for Function Columns
                        for (int i = pd.GetColumnInfo().Length - 1; i >= 0; i--)	//	backwards (leaset group first)
                        {
                            PrintDataColumn group_pdc = pd.GetColumnInfo()[i];
                            if (!_group.IsGroupColumn(group_pdc.GetColumnName()))
                                continue;

                            //	Group change
                            Object value = _group.GroupChange(group_pdc.GetColumnName(), ds.Tables[0].Rows[x][group_pdc.GetAlias()]);
                            if (value != null)	//	Group change
                            {
                                char[] functions = _group.GetFunctions(group_pdc.GetColumnName());
                                for (int f = 0; f < functions.Length; f++)
                                {
                                    PrintRunningTotal(pd, levelNo, rowNo++);
                                    pd.AddRow(true, levelNo);
                                    //	Get columns
                                    for (int c = 0; c < pd.GetColumnInfo().Length; c++)
                                    {
                                        pdc = pd.GetColumnInfo()[c];
                                        //	log.fine("loadPrintData - PageBreak = " + pdc.isPageBreak());

                                        if (group_pdc.GetColumnName().Equals(pdc.GetColumnName()))
                                        {
                                            String valueString = value.ToString();
                                            if (value.GetType() == typeof(DateTime))
                                            { }
                                            //        valueString = DisplayType.GetDateFormat(pdc.GetDisplayType(), _language).format(value);
                                            valueString += PrintDataFunction.GetFunctionSymbol(functions[f]);
                                            pd.AddNode(new PrintDataElement(pdc.GetColumnName(), valueString, DisplayType.String, false, pdc.IsPageBreak()));
                                        }
                                        else if (_group.IsFunctionColumn(pdc.GetColumnName(), functions[f]))
                                        {
                                            pd.AddNode(new PrintDataElement(pdc.GetColumnName(), NativeDigitConverter.ConvertToNativeNumerals(
                                                _group.GetValue(group_pdc.GetColumnName(), pdc.GetColumnName(), functions[f]), pd.GetCtx()),
                                                PrintDataFunction.GetFunctionDisplayType(functions[f]), false, pdc.IsPageBreak()));
                                        }
                                    }	//	 for all columns
                                }	//	for all functions
                                //	Reset Group Values
                                for (int c = 0; c < pd.GetColumnInfo().Length; c++)
                                {
                                    pdc = pd.GetColumnInfo()[c];
                                    _group.Reset(group_pdc.GetColumnName(), pdc.GetColumnName());
                                }
                            }	//	Group change
                        }	//	for all columns
                    }	//	group change

                    //	new row ---------------------------------------------------
                    PrintRunningTotal(pd, levelNo, rowNo++);
                    pd.AddRow(false, levelNo);
                    int counter = 0;
                    //	Get columns
                    for (int i = 0; i < pd.GetColumnInfo().Length; i++)
                    {
                        pdc = pd.GetColumnInfo()[i];
                        PrintDataElement pde = null;

                        //	Key Column - No DisplayColumn
                        if (pdc.GetAlias().Equals(KEY))
                        {
                            if (pdc.GetColumnName().EndsWith("_ID"))
                            {
                                //	int id = rs.GetInt(pdc.GetColumnIDName());
                                int id = Utility.Util.GetValueOfInt(ds.Tables[0].Rows[x][counter++]);
                                if (!string.IsNullOrEmpty(id.ToString()))
                                {
                                    KeyNamePair pp = new KeyNamePair(id, KEY);	//	Key
                                    pde = new PrintDataElement(pdc.GetColumnName(), pp, pdc.GetDisplayType(), true, pdc.IsPageBreak());
                                }
                            }
                            else
                            {
                                //	String id = rs.GetString(pdc.GetColumnIDName());
                                String id = ds.Tables[0].Rows[x][counter++].ToString();
                                if (!string.IsNullOrEmpty(id))
                                {
                                    ValueNamePair pp = new ValueNamePair(id, KEY);	//	Key
                                    pde = new PrintDataElement(pdc.GetColumnName(), pp, pdc.GetDisplayType(), true, pdc.IsPageBreak());
                                }
                            }
                        }
                        //	Non-Key Column
                        else
                        {
                            //	Display and Value Column
                            if (pdc.HasAlias())
                            {
                                //	DisplayColumn first
                                String display = ds.Tables[0].Rows[x][counter++].ToString();
                                if (pdc.GetColumnName().EndsWith("_ID"))
                                {
                                    string id = ds.Tables[0].Rows[x][counter++].ToString();
                                    if (display != null && !string.IsNullOrEmpty(id))
                                    {
                                        KeyNamePair pp = new KeyNamePair(int.Parse(id), display);
                                        pde = new PrintDataElement(pdc.GetColumnName(), pp, pdc.GetDisplayType());
                                    }
                                }
                                else
                                {
                                    string id = ds.Tables[0].Rows[x][counter++].ToString();
                                    if (display != null && !string.IsNullOrEmpty(id))
                                    {
                                        ValueNamePair pp = new ValueNamePair(id, display);
                                        pde = new PrintDataElement(pdc.GetColumnName(), pp, pdc.GetDisplayType());
                                    }
                                }
                            }
                            //	Display Value only
                            else
                            {
                                //	Transformation for bools
                                if (pdc.GetDisplayType() == DisplayType.YesNo)
                                {
                                    String s = ds.Tables[0].Rows[x][counter++].ToString();
                                    if (!string.IsNullOrEmpty(s))
                                    {
                                        bool b = s.Equals("Y");
                                        pde = new PrintDataElement(pdc.GetColumnName(), (bool)b, pdc.GetDisplayType());
                                    }
                                }
                                else if (pdc.GetDisplayType() == DisplayType.TextLong)
                                {

                                    string clob = ds.Tables[0].Rows[x][counter++].ToString();
                                    String value = "";
                                    if (clob != null)
                                    {
                                        long Length = clob.Length;
                                        value = clob.Substring(0, (int)Length);
                                    }
                                    pde = new PrintDataElement(pdc.GetColumnName(), value, pdc.GetDisplayType());
                                }

                                /* Modified by Deepak */
                                /* to resolve DateTime Issue*/

                                else if (DisplayType.IsDate(pdc.GetDisplayType()))
                                {
                                    //Modified by Jagmohan Bhatt
                                    //Purpose: There are some unkonwn errors which are not being traced (only on few systems).
                                    //In case any error comes (rare), we will show it blank so that report does not get interrupted. 
                                    DateTime? time = null;
                                    string sValue = ds.Tables[0].Rows[x][counter++].ToString();
                                    if (string.IsNullOrEmpty(sValue))
                                    {
                                        continue;
                                    }

                                    if (pdc.GetDisplayType() == DisplayType.DateTime)
                                    {
                                        try
                                        {
                                            time = Convert.ToDateTime(sValue);
                                            //DateTime finalValue = TimeZoneInfo.ConvertTime(DateTime.SpecifyKind((DateTime)time, DateTimeKind.Utc), TimeZoneInfo.FindSystemTimeZoneById(format.GetCtx().GetContext("#TimeZoneName")));

                                            DateTime finalValue = time.Value;//.ToUniversalTime();
                                            string offset = format.GetCtx().GetContext("#TimezoneOffset");
                                            if (!string.IsNullOrEmpty(offset))
                                            {
                                                finalValue = finalValue.AddMinutes(-int.Parse(offset));
                                            }

                                            pde = new PrintDataElement(pdc.GetColumnName(), finalValue, pdc.GetDisplayType());

                                            sValue = finalValue.ToString("G");
                                            //continue;
                                        }
                                        catch
                                        {
                                            continue;
                                        }
                                    }

                                    else if (pdc.GetDisplayType() == DisplayType.Date)
                                    {
                                        try
                                        {
                                            if (sValue.Length > 10)
                                            {
                                                time = Convert.ToDateTime(sValue);
                                                //DateTime finalValue = TimeZoneInfo.ConvertTime(DateTime.SpecifyKind((DateTime)time, DateTimeKind.Utc), TimeZoneInfo.FindSystemTimeZoneById(format.GetCtx().GetContext("#TimeZoneName")));
                                                DateTime finalValue = time.Value;//.ToUniversalTime();
                                                //string offset = format.GetCtx().GetContext("#TimezoneOffset");
                                                //if (!string.IsNullOrEmpty(offset))
                                                //{
                                                //    finalValue = finalValue.AddMinutes(- int.Parse(offset));
                                                //}

                                                sValue = finalValue.ToString("d"); // sValue.Substring(0, sValue.IndexOf(" "));
                                            }
                                            // time = Convert.ToDateTime(sValue);
                                            //pde = new PrintDataElement(pdc.GetColumnName(), sValue, pdc.GetDisplayType());

                                        }
                                        catch
                                        {
                                            continue;
                                        }
                                    }
                                    else  // only Time
                                    {
                                        try
                                        {
                                            if (sValue.Length > 10)
                                            {
                                                time = Convert.ToDateTime(sValue);
                                                //DateTime finalValue = TimeZoneInfo.ConvertTime(DateTime.SpecifyKind((DateTime)time, DateTimeKind.Utc), TimeZoneInfo.FindSystemTimeZoneById(format.GetCtx().GetContext("#TimeZoneName")));
                                                //sValue = sValue.Substring(10);

                                                DateTime finalValue = time.Value;//.ToUniversalTime();
                                                string offset = format.GetCtx().GetContext("#TimezoneOffset");
                                                if (!string.IsNullOrEmpty(offset))
                                                {
                                                    finalValue = finalValue.AddMinutes(-int.Parse(offset));
                                                }

                                                //sValue = finalValue.ToString("d"); // sValue.Substring(0, sValue.IndexOf(" "));

                                                sValue = finalValue.ToString("T"); // sValue.Substring(sValue.IndexOf(" ") + 1);
                                            }
                                            //pde = new PrintDataElement(pdc.GetColumnName(), sValue, pdc.GetDisplayType());
                                        }
                                        catch
                                        {
                                            continue;
                                        }
                                    }
                                    pde = new PrintDataElement(pdc.GetColumnName(), NativeDigitConverter.ConvertToNativeNumerals(sValue, pd.GetCtx()), pdc.GetDisplayType());
                                }

                                else
                                //	The general case
                                {
                                    Object obj = ds.Tables[0].Rows[x][counter++];
                                    if (obj != null && obj.GetType() == typeof(String))
                                    {
                                        obj = ((String)obj).Trim();
                                        if (((String)obj).Length == 0)
                                            obj = null;
                                    }
                                    if (obj != null)
                                    {
                                        //	Translate Spool Output
                                        if (translateSpool && obj.GetType() == typeof(String))
                                        {
                                            String s = (String)obj;
                                            s = Utility.Msg.ParseTranslation(pd.GetCtx(), s);
                                            pde = new PrintDataElement(pdc.GetColumnName(), s, pdc.GetDisplayType());
                                        }
                                        else
                                        {
                                            object toSting = obj;

                                            if (!Env.IsBaseLanguage(pd.GetCtx().GetContext("#AD_Language"), "") && DisplayType.IsNumeric(pdc.GetDisplayType()) && !pdc.GetColumnName().Equals("AmtInWords")) // Convert to native Digits
                                            {
                                                toSting = NativeDigitConverter.ConvertToNativeNumerals(toSting.ToString(), pd.GetCtx());
                                            }
                                            pde = new PrintDataElement(pdc.GetColumnName(), toSting, obj, pdc.GetDisplayType());
                                        }
                                    }
                                }
                            }	//	Value only
                        }	//	Non-Key Column
                        if (pde != null)
                        {
                            pd.AddNode(pde);
                            _group.AddValue(pde.GetColumnName(), pde.GetFunctionOriginalValue());
                        }
                    }	//	for all columns



                }


                //while (dr.Read())
                //{
                //    if (hasLevelNo)
                //        levelNo = Utility.Util.GetValueOfInt(dr["LevelNo"].ToString());
                //    else
                //        levelNo = 0;
                //    //	Check Group Change ----------------------------------------
                //    if (_group.GetGroupColumnCount() > 1)	//	one is GRANDTOTAL_
                //    {
                //        //	Check Columns for Function Columns
                //        for (int i = pd.GetColumnInfo().Length - 1; i >= 0; i--)	//	backwards (leaset group first)
                //        {
                //            PrintDataColumn group_pdc = pd.GetColumnInfo()[i];
                //            if (!_group.IsGroupColumn(group_pdc.GetColumnName()))
                //                continue;

                //            //	Group change
                //            Object value = _group.GroupChange(group_pdc.GetColumnName(), dr[group_pdc.GetAlias()]);
                //            if (value != null)	//	Group change
                //            {
                //                char[] functions = _group.GetFunctions(group_pdc.GetColumnName());
                //                for (int f = 0; f < functions.Length; f++)
                //                {
                //                    PrintRunningTotal(pd, levelNo, rowNo++);
                //                    pd.AddRow(true, levelNo);
                //                    //	Get columns
                //                    for (int c = 0; c < pd.GetColumnInfo().Length; c++)
                //                    {
                //                        pdc = pd.GetColumnInfo()[c];
                //                        //	log.fine("loadPrintData - PageBreak = " + pdc.isPageBreak());

                //                        if (group_pdc.GetColumnName().Equals(pdc.GetColumnName()))
                //                        {
                //                            String valueString = value.ToString();
                //                            if (value.GetType() == typeof(DateTime))
                //                            { }
                //                            //        valueString = DisplayType.GetDateFormat(pdc.GetDisplayType(), _language).format(value);
                //                            valueString += PrintDataFunction.GetFunctionSymbol(functions[f]);
                //                            pd.AddNode(new PrintDataElement(pdc.GetColumnName(), valueString, DisplayType.String, false, pdc.IsPageBreak()));
                //                        }
                //                        else if (_group.IsFunctionColumn(pdc.GetColumnName(), functions[f]))
                //                        {
                //                            pd.AddNode(new PrintDataElement(pdc.GetColumnName(), NativeDigitConverter.ConvertToNativeNumerals(
                //                                _group.GetValue(group_pdc.GetColumnName(), pdc.GetColumnName(), functions[f]), pd.GetCtx()),
                //                                PrintDataFunction.GetFunctionDisplayType(functions[f]), false, pdc.IsPageBreak()));
                //                        }
                //                    }	//	 for all columns
                //                }	//	for all functions
                //                //	Reset Group Values
                //                for (int c = 0; c < pd.GetColumnInfo().Length; c++)
                //                {
                //                    pdc = pd.GetColumnInfo()[c];
                //                    _group.Reset(group_pdc.GetColumnName(), pdc.GetColumnName());
                //                }
                //            }	//	Group change
                //        }	//	for all columns
                //    }	//	group change

                //    //	new row ---------------------------------------------------
                //    PrintRunningTotal(pd, levelNo, rowNo++);
                //    pd.AddRow(false, levelNo);
                //    int counter = 0;
                //    //	Get columns
                //    for (int i = 0; i < pd.GetColumnInfo().Length; i++)
                //    {
                //        pdc = pd.GetColumnInfo()[i];
                //        PrintDataElement pde = null;

                //        //	Key Column - No DisplayColumn
                //        if (pdc.GetAlias().Equals(KEY))
                //        {
                //            if (pdc.GetColumnName().EndsWith("_ID"))
                //            {
                //                //	int id = rs.GetInt(pdc.GetColumnIDName());
                //                int id = Utility.Util.GetValueOfInt(dr[counter++]);
                //                if (!string.IsNullOrEmpty(id.ToString()))
                //                {
                //                    KeyNamePair pp = new KeyNamePair(id, KEY);	//	Key
                //                    pde = new PrintDataElement(pdc.GetColumnName(), pp, pdc.GetDisplayType(), true, pdc.IsPageBreak());
                //                }
                //            }
                //            else
                //            {
                //                //	String id = rs.GetString(pdc.GetColumnIDName());
                //                String id = dr[counter++].ToString();
                //                if (!string.IsNullOrEmpty(id))
                //                {
                //                    ValueNamePair pp = new ValueNamePair(id, KEY);	//	Key
                //                    pde = new PrintDataElement(pdc.GetColumnName(), pp, pdc.GetDisplayType(), true, pdc.IsPageBreak());
                //                }
                //            }
                //        }
                //        //	Non-Key Column
                //        else
                //        {
                //            //	Display and Value Column
                //            if (pdc.HasAlias())
                //            {
                //                //	DisplayColumn first
                //                String display = dr[counter++].ToString();
                //                if (pdc.GetColumnName().EndsWith("_ID"))
                //                {
                //                    string id = dr[counter++].ToString();
                //                    if (display != null && !string.IsNullOrEmpty(id))
                //                    {
                //                        KeyNamePair pp = new KeyNamePair(int.Parse(id), display);
                //                        pde = new PrintDataElement(pdc.GetColumnName(), pp, pdc.GetDisplayType());
                //                    }
                //                }
                //                else
                //                {
                //                    string id = dr[counter++].ToString();
                //                    if (display != null && !string.IsNullOrEmpty(id))
                //                    {
                //                        ValueNamePair pp = new ValueNamePair(id, display);
                //                        pde = new PrintDataElement(pdc.GetColumnName(), pp, pdc.GetDisplayType());
                //                    }
                //                }
                //            }
                //            //	Display Value only
                //            else
                //            {
                //                //	Transformation for bools
                //                if (pdc.GetDisplayType() == DisplayType.YesNo)
                //                {
                //                    String s = dr[counter++].ToString();
                //                    if (!string.IsNullOrEmpty(s))
                //                    {
                //                        bool b = s.Equals("Y");
                //                        pde = new PrintDataElement(pdc.GetColumnName(), (bool)b, pdc.GetDisplayType());
                //                    }
                //                }
                //                else if (pdc.GetDisplayType() == DisplayType.TextLong)
                //                {

                //                    string clob = dr[counter++].ToString();
                //                    String value = "";
                //                    if (clob != null)
                //                    {
                //                        long Length = clob.Length;
                //                        value = clob.Substring(0, (int)Length);
                //                    }
                //                    pde = new PrintDataElement(pdc.GetColumnName(), value, pdc.GetDisplayType());
                //                }

                //                    /* Modified by Deepak */
                //                /* to resolve DateTime Issue*/

                //                else if (DisplayType.IsDate(pdc.GetDisplayType()))
                //                {
                //                    //Modified by Jagmohan Bhatt
                //                    //Purpose: There are some unkonwn errors which are not being traced (only on few systems).
                //                    //In case any error comes (rare), we will show it blank so that report does not get interrupted. 
                //                    DateTime? time = null;
                //                    string sValue = dr[counter++].ToString();
                //                    if (string.IsNullOrEmpty(sValue))
                //                    {
                //                        continue;
                //                    }

                //                    if (pdc.GetDisplayType() == DisplayType.DateTime)
                //                    {
                //                        try
                //                        {
                //                            time = Convert.ToDateTime(sValue);
                //                            //DateTime finalValue = TimeZoneInfo.ConvertTime(DateTime.SpecifyKind((DateTime)time, DateTimeKind.Utc), TimeZoneInfo.FindSystemTimeZoneById(format.GetCtx().GetContext("#TimeZoneName")));

                //                            DateTime finalValue = time.Value;//.ToUniversalTime();
                //                            string offset = format.GetCtx().GetContext("#TimezoneOffset");
                //                            if (!string.IsNullOrEmpty(offset))
                //                            {
                //                                finalValue = finalValue.AddMinutes(-int.Parse(offset));
                //                            }

                //                            pde = new PrintDataElement(pdc.GetColumnName(), finalValue, pdc.GetDisplayType());

                //                            sValue = finalValue.ToString("G");
                //                            //continue;
                //                        }
                //                        catch
                //                        {
                //                            continue;
                //                        }
                //                    }

                //                    else if (pdc.GetDisplayType() == DisplayType.Date)
                //                    {
                //                        try
                //                        {
                //                            if (sValue.Length > 10)
                //                            {
                //                                time = Convert.ToDateTime(sValue);
                //                                //DateTime finalValue = TimeZoneInfo.ConvertTime(DateTime.SpecifyKind((DateTime)time, DateTimeKind.Utc), TimeZoneInfo.FindSystemTimeZoneById(format.GetCtx().GetContext("#TimeZoneName")));
                //                                DateTime finalValue = time.Value;//.ToUniversalTime();
                //                                //string offset = format.GetCtx().GetContext("#TimezoneOffset");
                //                                //if (!string.IsNullOrEmpty(offset))
                //                                //{
                //                                //    finalValue = finalValue.AddMinutes(- int.Parse(offset));
                //                                //}

                //                                sValue = finalValue.ToString("d"); // sValue.Substring(0, sValue.IndexOf(" "));
                //                            }
                //                            // time = Convert.ToDateTime(sValue);
                //                            //pde = new PrintDataElement(pdc.GetColumnName(), sValue, pdc.GetDisplayType());

                //                        }
                //                        catch
                //                        {
                //                            continue;
                //                        }
                //                    }
                //                    else  // only Time
                //                    {
                //                        try
                //                        {
                //                            if (sValue.Length > 10)
                //                            {
                //                                time = Convert.ToDateTime(sValue);
                //                                //DateTime finalValue = TimeZoneInfo.ConvertTime(DateTime.SpecifyKind((DateTime)time, DateTimeKind.Utc), TimeZoneInfo.FindSystemTimeZoneById(format.GetCtx().GetContext("#TimeZoneName")));
                //                                //sValue = sValue.Substring(10);

                //                                DateTime finalValue = time.Value;//.ToUniversalTime();
                //                                string offset = format.GetCtx().GetContext("#TimezoneOffset");
                //                                if (!string.IsNullOrEmpty(offset))
                //                                {
                //                                    finalValue = finalValue.AddMinutes(-int.Parse(offset));
                //                                }

                //                                //sValue = finalValue.ToString("d"); // sValue.Substring(0, sValue.IndexOf(" "));

                //                                sValue = finalValue.ToString("T"); // sValue.Substring(sValue.IndexOf(" ") + 1);
                //                            }
                //                            //pde = new PrintDataElement(pdc.GetColumnName(), sValue, pdc.GetDisplayType());
                //                        }
                //                        catch
                //                        {
                //                            continue;
                //                        }
                //                    }
                //                    pde = new PrintDataElement(pdc.GetColumnName(), NativeDigitConverter.ConvertToNativeNumerals(sValue, pd.GetCtx()), pdc.GetDisplayType());
                //                }

                //                else
                //                //	The general case
                //                {
                //                    Object obj = dr[counter++];
                //                    if (obj != null && obj.GetType() == typeof(String))
                //                    {
                //                        obj = ((String)obj).Trim();
                //                        if (((String)obj).Length == 0)
                //                            obj = null;
                //                    }
                //                    if (obj != null)
                //                    {
                //                        //	Translate Spool Output
                //                        if (translateSpool && obj.GetType() == typeof(String))
                //                        {
                //                            String s = (String)obj;
                //                            s = Utility.Msg.ParseTranslation(pd.GetCtx(), s);
                //                            pde = new PrintDataElement(pdc.GetColumnName(), s, pdc.GetDisplayType());
                //                        }
                //                        else
                //                        {
                //                            object toSting = obj;

                //                            if (!Env.IsBaseLanguage(pd.GetCtx().GetContext("#AD_Language"), "") && DisplayType.IsNumeric(pdc.GetDisplayType()) && !pdc.GetColumnName().Equals("AmtInWords")) // Convert to native Digits
                //                            {
                //                                toSting = NativeDigitConverter.ConvertToNativeNumerals(toSting.ToString(), pd.GetCtx());
                //                            }
                //                            pde = new PrintDataElement(pdc.GetColumnName(), toSting, obj, pdc.GetDisplayType());
                //                        }
                //                    }
                //                }
                //            }	//	Value only
                //        }	//	Non-Key Column
                //        if (pde != null)
                //        {
                //            pd.AddNode(pde);
                //            _group.AddValue(pde.GetColumnName(), pde.GetFunctionOriginalValue());
                //        }
                //    }	//	for all columns

                //}	//	for all rows
                //dr.Close();
            }
            catch (Exception e)
            {
                //if (dr != null)
                //{
                //    dr.Close();
                //}
                log.Severe(e.ToString());
            }
            finally
            {
                //if (dr != null)
                //    dr.Close();
            }

            //	--	we have all rows - finish
            //	Check last Group Change
            if (_group.GetGroupColumnCount() > 1)	//	one is TOTAL
            {
                for (int i = pd.GetColumnInfo().Length - 1; i >= 0; i--)	//	backwards (leaset group first)
                {
                    PrintDataColumn group_pdc = pd.GetColumnInfo()[i];
                    if (!_group.IsGroupColumn(group_pdc.GetColumnName()))
                        continue;
                    Object value = _group.GroupChange(group_pdc.GetColumnName(), new Object());
                    if (value != null)	//	Group change
                    {
                        char[] functions = _group.GetFunctions(group_pdc.GetColumnName());
                        for (int f = 0; f < functions.Length; f++)
                        {
                            PrintRunningTotal(pd, levelNo, rowNo++);
                            pd.AddRow(true, levelNo);
                            //	Get columns
                            for (int c = 0; c < pd.GetColumnInfo().Length; c++)
                            {
                                pdc = pd.GetColumnInfo()[c];
                                if (group_pdc.GetColumnName().Equals(pdc.GetColumnName()))
                                {
                                    String valueString = value.ToString();
                                    //if (value.GetType() == typeof(DateTime))
                                    //    valueString = DisplayType.GetDateFormat(pdc.GetDisplayType(), _language).format(value);
                                    valueString += PrintDataFunction.GetFunctionSymbol(functions[f]);
                                    pd.AddNode(new PrintDataElement(pdc.GetColumnName(),
                                        valueString, DisplayType.String));
                                }
                                else if (_group.IsFunctionColumn(pdc.GetColumnName(), functions[f]))
                                {
                                    pd.AddNode(new PrintDataElement(pdc.GetColumnName(),
                                       NativeDigitConverter.ConvertToNativeNumerals(
                                        _group.GetValue(group_pdc.GetColumnName(),
                                            pdc.GetColumnName(), functions[f]), pd.GetCtx()),
                                        PrintDataFunction.GetFunctionDisplayType(functions[f])));
                                }
                            }
                        }	//	for all functions
                        //	No Need to Reset
                    }	//	Group change
                }
            }	//	last group change

            //	Add Total Lines
            if (_group.IsGroupColumn(PrintDataGroup.TOTAL))
            {
                char[] functions = _group.GetFunctions(PrintDataGroup.TOTAL);
                for (int f = 0; f < functions.Length; f++)
                {
                    PrintRunningTotal(pd, levelNo, rowNo++);
                    pd.AddRow(true, levelNo);
                    //	Get columns
                    for (int c = 0; c < pd.GetColumnInfo().Length; c++)
                    {
                        pdc = pd.GetColumnInfo()[c];
                        if (c == 0)		//	put Function in first Column
                        {
                            String name = "";
                            if (!format.GetTableFormat().IsPrintFunctionSymbols())		//	Translate Sum, etc.
                                name = Msg.GetMsg(Env.GetContext(), PrintDataFunction.GetFunctionName(functions[f]), true);
                            name += PrintDataFunction.GetFunctionSymbol(functions[f]);	//	Symbol
                            pd.AddNode(new PrintDataElement(pdc.GetColumnName(), name.Trim(), DisplayType.String));
                        }
                        else if (_group.IsFunctionColumn(pdc.GetColumnName(), functions[f]))
                        {
                            pd.AddNode(new PrintDataElement(pdc.GetColumnName(),

                                NativeDigitConverter.ConvertToNativeNumerals(_group.GetValue(PrintDataGroup.TOTAL, pdc.GetColumnName(), functions[f]), pd.GetCtx()),
                                PrintDataFunction.GetFunctionDisplayType(functions[f])));
                        }
                    }	//	for all columns
                }	//	for all functions
                //	No Need to Reset
            }	//	TotalLine

            if (pd.GetRowCount() == 0)
            {
                if (VLogMgt.IsLevelFinest())
                    log.Warning("NO Rows - ms=" + (CommonFunctions.CurrentTimeMillis() - _startTime)
                        + " - " + pd.GetSQL());
                else
                    log.Warning("NO Rows - ms=" + (CommonFunctions.CurrentTimeMillis() - _startTime));
            }
            else
                log.Info("Rows=" + pd.GetRowCount() + " - ms=" + (CommonFunctions.CurrentTimeMillis() - _startTime));
        }	//	loadPrintData

        /// <summary>
        /// Print Runnning total
        /// </summary>
        /// <param name="pd">print data</param>
        /// <param name="levelNo">level number</param>
        /// <param name="rowNo">row number</param>
        private void PrintRunningTotal(PrintData pd, int levelNo, int rowNo)
        {
            if (_runningTotalLines < 1)	//	-1 = none
                return;
            if (rowNo % _runningTotalLines != 0)
                return;

            PrintDataColumn pdc = null;
            int start = 0;
            if (rowNo == 0)	//	no page break on page 1
                start = 1;
            for (int rt = start; rt < 2; rt++)
            {
                pd.AddRow(true, levelNo);
                //	Get sum columns
                for (int c = 0; c < pd.GetColumnInfo().Length; c++)
                {
                    pdc = pd.GetColumnInfo()[c];
                    if (c == 0)
                    {
                        String title = "RunningTotal";
                        pd.AddNode(new PrintDataElement(pdc.GetColumnName(),
                            title, DisplayType.String, false, rt == 0));		//	page break
                    }
                    else if (_group.IsFunctionColumn(pdc.GetColumnName(), PrintDataFunction.F_SUM))
                    {
                        pd.AddNode(new PrintDataElement(pdc.GetColumnName(), NativeDigitConverter.ConvertToNativeNumerals(
                            _group.GetValue(PrintDataGroup.TOTAL, pdc.GetColumnName(), PrintDataFunction.F_SUM), pd.GetCtx()),
                            PrintDataFunction.GetFunctionDisplayType(PrintDataFunction.F_SUM), false, false));
                    }
                }	//	for all sum columns
            }	//	 two lines
        }	//	printRunningTotal

        /// <summary>
        /// Get the reference of the table
        /// </summary>
        /// <param name="AD_Reference_Value_ID">reference value id</param>
        /// <returns></returns>
        public static TableReference GetTableReference(int AD_Reference_Value_ID)
        {
            TableReference tr = new TableReference();
            //
            String SQL = "SELECT t.TableName, ck.ColumnName AS KeyColumn,"	//	1..2
                + " cd.ColumnName AS DisplayColumn, rt.IsValueDisplayed, cd.IsTranslated "
                + "FROM AD_Ref_Table rt"
                + " INNER JOIN AD_Table t ON (rt.AD_Table_ID = t.AD_Table_ID)"
                + " INNER JOIN AD_Column ck ON (rt.Column_Key_ID = ck.AD_Column_ID)"
                + " INNER JOIN AD_Column cd ON (rt.Column_Display_ID = cd.AD_Column_ID) "
                + "WHERE rt.AD_Reference_ID='" + AD_Reference_Value_ID + "'"			//	1
                + " AND rt.IsActive = 'Y' AND t.IsActive = 'Y'";
            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(SQL);
                while (dr.Read())
                {
                    tr.TableName = dr[0].ToString();
                    tr.KeyColumn = dr[1].ToString();
                    tr.DisplayColumn = dr[2].ToString();
                    tr.IsValueDisplayed = "Y".Equals(dr[3].ToString());
                    tr.IsTranslated = "Y".Equals(dr[4].ToString());
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                log.Severe(ex.ToString());
            }
            finally
            {
                dr.Close();
            }
            return tr;
        }	//  GetTableReference

        /// <summary>
        /// Get the next synonym
        /// </summary>
        private void SynonymNext()
        {
            int Length = _synonym.Length;
            char cc = _synonym[0];
            if (cc == 'Z')
            {
                cc = 'A';
                Length++;
            }
            else
                cc++;
            //
            _synonym = cc.ToString();
            if (Length == 1)
                return;
            _synonym += cc.ToString();
            if (Length == 2)
                return;
            _synonym += cc.ToString();
        }	//	synonymNext
    }

}
