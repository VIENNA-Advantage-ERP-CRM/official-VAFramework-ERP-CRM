using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.Classes;
using System.Data.SqlClient;
using VAdvantage.Process;
using VAdvantage.DataBase;
//using VAdvantage.Apps;

namespace VAdvantage.Model
{
    /// <summary>
    /// Series Model
    /// </summary>
    public class MSeries : X_D_Series
    {

        private static VLogger s_log = VLogger.GetVLogger(typeof(MSeries).FullName);

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="D_Series_ID">series ID</param>
        /// <param name="trxName">trx Name (Optional)</param>
        public MSeries(Ctx ctx, int D_Series_ID, Trx trxName)
            : base(ctx, D_Series_ID, trxName)
        {
            if (D_Series_ID == 0)
            {
                //save default records
            }
        }   //MSeries


        /// <summary>
        /// Action after save or update
        /// </summary>
        /// <param name="newRecord"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!newRecord)
            {
                bool isChanged = (this.Is_ValueChanged("AlertValue") || this.Is_ValueChanged("AlertValue_X") ||
                    this.Is_ValueChanged("WhereCondition") || this.Is_ValueChanged("DateTimeTypes") ||
                    this.Is_ValueChanged("AD_COLUMN_X_ID") || this.Is_ValueChanged("AD_COLUMN_Y_ID"));
                if (isChanged)
                {
                    string sql = "UPDATE D_SERIES SET ISSHOWN = 'N', STARTVALUE=0, ALERTLASTRUN=NULL "
                    + "WHERE D_SERIES_ID = @SeriesID";
                    SqlParameter[] param = new SqlParameter[1];
                    param[0] = new SqlParameter("@SeriesID", Get_ID());
                    int no = DataBase.DB.ExecuteQuery(sql, param);
                    log.Fine("MSeries Alert update  #" + no);

                }
            }

            return success;
        }


        protected override bool BeforeSave(bool newRecord)
        {

            if (IsView())
            {
                SetAD_Table_ID(-1);
            }
            else
            {
                SetTableView_ID(-1);
            }

            return true;
        }



        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">DataRow</param>
        /// <param name="trxName">Transaction</param>
        public MSeries(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }	//	MSeries

        
        /** Cached Fonts						*/
        private static CCache<int, MSeries> s_chart = new CCache<int, MSeries>("D_Series", 5);

        //Asix Names (X and Y)
        private static string m_colX;
        private static string m_colY;

        //Table Name
        private static string m_tableName;

        /* X Filter */
        private static string m_XFilter;

        /** String Constants for DataTypes **/
        private const string IS_DATETIME = "(15,16)      ";
        private const string IS_STRING = "(10,14)      ";
        private const string IS_INTEGER = "(12,11,22,29)";
        private const string IS_IDENTIFIER = "(19)         ";

        /** String Constants for DateTime Types **/
        private const string IS_YEARLY = "Y";
        private const string IS_DAILY = "D";
        private const string IS_MONTHLY = "M";
        //----------------------------------------
        private const string IS_LAST_N_DAYS = "A";
        private const string IS_LAST_N_MONTHS = "B";
        private const string IS_LAST_N_YEARS = "C";
        private const string IS_CURRENT_WEEK = "W";
        private const string IS_ALL = "E";


        /// <summary>
        /// Get all the series of selected chart
        /// </summary>
        /// <param name="D_Chart_ID"></param>
        /// <returns></returns>
        public MSeries[] Get(int D_Chart_ID)
        {
            int key = D_Chart_ID;
            List<MSeries> list = new List<MSeries>();
            String sql = "SELECT * FROM D_Series pfi "
                + "WHERE pfi.IsActive='Y' And pfi.D_Chart_ID=@D_ChartID";
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@D_ChartID", key);
                DataSet ds = SqlExec.ExecuteQuery.ExecuteDataset(sql, param);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    MSeries pfi = new MSeries(GetCtx(), dr, null);
                    //if (role.IsColumnAccess(GetAD_Table_ID(), pfi.GetAD_Column_ID(), true))
                    list.Add(pfi);

                }
            }
            catch (Exception e)
            {
                log.Severe(e.ToString());
                //log entry, if any
            }
            MSeries[] retValue = new MSeries[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get all the series of selected chart
        /// </summary>
        /// <param name="D_Chart_ID"></param>
        /// <returns></returns>
        static public MSeries GetByID(int D_Series_ID,Ctx ctx)
        {
            MSeries pfi = new MSeries(ctx, D_Series_ID, null);
            return pfi;
        }

        //Date to be processed
        private string m_date1;
        private string m_date2;

        //Date to be processed
        private DateTime m_date_1;
        private DateTime m_date_2;

        private const string SPACE = " ";   //SQL query space
        private const string IDENTIFIER_COL = "idtnfr.";

        /// <summary>
        /// Gets the dynamic SQL from database
        /// </summary>
        /// <returns></returns>
        public string GetSql(bool isFiltered, string specialWhere)
        {
            StringBuilder sb = new StringBuilder("SELECT ");    //start the SELECT QUERY
            MTable m_Table = null;// MTable.Get(GetCtx(), GetAD_Table_ID());
            //get the table name

            if (IsView())
            {
                m_Table = MTable.Get(GetCtx(), GetTableView_ID());
            }
            else
            {
                m_Table = MTable.Get(GetCtx(), GetAD_Table_ID());
            }


            string s_tableName = m_Table.GetTableName();

            //get X Column Name
            MColumn column = MColumn.Get(GetCtx(), this.GetAD_Column_X_ID());
            string s_colX = column.GetFKColumnName();

            column = MColumn.Get(GetCtx(), this.GetAD_Column_Y_ID());

            string s_colY = column.GetFKColumnName();
            m_XFilter = GetDateTimeTypes();
            //Assign values to the member vairables
            m_colX = s_colX;
            m_colY = s_colY;
            m_tableName = s_tableName;

            string s_groupby = "";
            //Append X Axis to SQL Query

            //if datatype = date and user want to see all the dates (including with no values)
            if (IsDate_X() && (!isFiltered))
            {
                StringBuilder sbGroupBy = new StringBuilder("");
                sbGroupBy.Append(ApplyDateFunction(s_colX));
                if (GetDateTimeTypes() == IS_DAILY || GetDateTimeTypes() == IS_LAST_N_DAYS)
                    sbGroupBy.Append(", ")
                        .Append(ApplyDateFunction(s_colX, "yyyy"))
                        .Append(", ")
                        .Append(ApplyDateFunction(s_colX, "mm"))
                        .Append(", ").Append(ApplyDateFunction(s_colX, "dd/mm/yy"));
                else if (GetDateTimeTypes() == IS_MONTHLY || GetDateTimeTypes() == IS_LAST_N_MONTHS)
                    sbGroupBy.Append(", ")
                        .Append(ApplyDateFunction(s_colX, "yyyy"))
                        .Append(", ").Append(ApplyDateFunction(s_colX, "Mon-RRRR"));

                s_groupby = sbGroupBy.ToString();   //set value into variable to be used later for group by clause (!important)
                sb.Append(sbGroupBy.ToString());

            }
            else
                sb.Append(s_colX);  //in case of other than date and (no filter)

            //give name to col X (can be fetched from db too)
            if (!IsIdentifier_X())
                sb.Append(" ColX, ");
            else
                sb.Append(" , ");


            if (!IsIdentifier_X())
                sb.Append(ApplyAggregateFunction(s_colY, false));
            else //in case of identifier column, do not aggregate it (we will do it in top level query)
                sb.Append("NVL(")
                    .Append(s_colY)
                    .Append(",0) ").Append(s_colY);
            //sb.Append(",'9,999.99')");

            //give name to col Y
            if (!IsIdentifier_X())
                sb.Append(" ColY");

            sb.Append(" From ").Append(s_tableName);    //append table name

            string val = "";
            if (!string.IsNullOrEmpty(GetSQLWhere(out val)))    //get the where clause (actually it fectches only date condition. However, it can be used for other where clause purposes)
            {
                sb.Append(val);
                string whereClause = GetWhereClause();
                if (!string.IsNullOrEmpty(whereClause))
                {
                    whereClause = Env.ParseContext(GetCtx(), 0, whereClause, false);
                    if (val.Equals(" WHERE "))
                        sb.Append("").Append(whereClause);
                    else
                        sb.Append("").Append(" AND " + whereClause);
                }
                else
                {
                    if (val.Equals(" WHERE "))
                        sb.Append("1=1");
                }

                //special where serves the purpose
                //for alerts where user needs alerts
                //for particular X value
                if (!string.IsNullOrEmpty(specialWhere))
                {
                    sb.Append(" ").Append(" AND " + specialWhere);
                }


                //Additional Filter
                MSeriesFilter[] filter = MSeriesFilter.GetFilters(this.Get_ID());
                StringBuilder sqlFilter = new StringBuilder();
                bool ifBetween = false;
                for (int i = 0; i <= filter.Length - 1; i++)
                {
                    if (!val.Equals(" WHERE "))
                        sqlFilter.Append(" AND ");
                    string colValue = MColumn.Get(GetCtx(), filter[i].GetAD_Column_ID()).GetColumnName();
                    sqlFilter.Append(colValue);

                    if (filter[i].GetWhereCondition() == "1")
                        sqlFilter.Append(" = ");
                    else if (filter[i].GetWhereCondition() == "2")
                        sqlFilter.Append(" != ");
                    else if (filter[i].GetWhereCondition() == "3")
                        sqlFilter.Append(" >= ");
                    else if (filter[i].GetWhereCondition() == "4")
                        sqlFilter.Append(" <= ");
                    else if (filter[i].GetWhereCondition() == "5")
                    {
                        sqlFilter.Append(" BETWEEN ");
                        ifBetween = true;
                    }

                    //put value
                    if (MColumn.Get(GetCtx(), filter[i].GetAD_Column_ID()).getSQLDataType() == "DATE")
                    {
                        sqlFilter.Append("TO_DATE('").Append(filter[0].GetWhereValue()).Append("','dd/mm/RRRR'").Append(")");
                    }
                    else
                        sqlFilter.Append("'").Append(Env.ParseContext(GetCtx(), 0, filter[i].GetWhereValue(), false)).Append("'");
                    if (ifBetween)
                    {
                        if (MColumn.Get(GetCtx(), filter[i].GetAD_Column_ID()).getSQLDataType() == "DATE")
                        {
                            //sqlFilter.Append("TO_DATE('").Append(colValue).Append("','dd/mm/yyyy'").Append(")");
                            sqlFilter.Append(" AND ");
                            sqlFilter.Append("TO_DATE('").Append(filter[i].GetValueTo()).Append("','dd/mm/RRRR'").Append(")");
                        }
                        else
                            sqlFilter.Append(" AND '").Append(filter[i].GetValueTo()).Append("'");
                    }

                    if (sb.ToString().IndexOf("WHERE") > 0)
                        sb.Append(" ").Append(sqlFilter);

                }
            }

            //string datatype_Y = GetDataType_Y();
            // if (datatype_Y.Equals(IS_STRING) || datatype_Y.Equals(IS_INTEGER) && !IsNone())
            //Changes made on 5jul2011 to query view as we do with tables (req. by amardeep/done by jagmohan)
            if ((!IsIdentifier_X() && isFiltered) || IsString_X())
                sb.Append(AddGroupByClause(s_colX));

            if (IsDate_X() && isFiltered)
            {
                StringBuilder unionTableQuery = new StringBuilder("SELECT ");
                if (this.GetDateTimeTypes() == IS_MONTHLY || this.GetDateTimeTypes() == IS_LAST_N_MONTHS)
                {
                    unionTableQuery.Append(ApplyDateFunction(m_colX)).Append(", ")
                        .Append("TRIM(TO_CHAR(" + m_colX + ",'yyyy')), ")
                        .Append(ApplyAggregateFunction(m_colY, true)).Append(SPACE + "ColY");
                    unionTableQuery.Append(", TRIM(TO_CHAR(" + m_colX + ",'Mon-RRRR')) as ColX");
                }
                else if (this.GetDateTimeTypes() == IS_DAILY || this.GetDateTimeTypes() == IS_LAST_N_DAYS || this.GetDateTimeTypes() == IS_CURRENT_WEEK)
                {
                    unionTableQuery.Append(ApplyDateFunction(m_colX)).Append(", ")
                        .Append("TRIM(TO_CHAR(" + m_colX + ",'yyyy')), ")
                        .Append("TRIM(TO_CHAR(" + m_colX + ",'mm')), ")
                        .Append(ApplyAggregateFunction(m_colY, true)).Append(SPACE + "ColY");
                    unionTableQuery.Append(", TRIM(TO_CHAR(" + m_colX + ",'dd/mm/yy')) as ColX");
                }
                else
                    unionTableQuery.Append(ApplyDateFunction(m_colX))
                        .Append(" as ColX,")
                        .Append(ApplyAggregateFunction(m_colY, true))
                        .Append(SPACE + "ColY");

                unionTableQuery.Append(" FROM(");
                unionTableQuery.Append(GetUnionQuery(m_date_1, m_date_2, m_colX, "0 " + m_colY));
                //UNION
                unionTableQuery.Append(" UNION ALL ");

                unionTableQuery.Append(sb.ToString());

                unionTableQuery.Append(")");

                //group by the whole query
                if (!IsNone())
                {
                    unionTableQuery.Append(" GROUP BY ");
                    unionTableQuery.Append(ApplyDateFunction(m_colX));

                    if (this.GetDateTimeTypes() == IS_MONTHLY || this.GetDateTimeTypes() == IS_LAST_N_MONTHS)
                    {
                        unionTableQuery.Append(", TRIM(TO_CHAR(" + m_colX + ",'Mon-RRRR'))")
                            .Append(", TRIM(TO_CHAR(" + m_colX + ",'yyyy'))");
                    }
                    else if (this.GetDateTimeTypes() == IS_DAILY || this.GetDateTimeTypes() == IS_LAST_N_DAYS || this.GetDateTimeTypes() == IS_CURRENT_WEEK)
                    {
                        unionTableQuery.Append(", TRIM(TO_CHAR(" + m_colX + ",'dd/mm/yy'))")
                            .Append(", TRIM(TO_CHAR(" + m_colX + ",'yyyy')),")
                            .Append("TRIM(TO_CHAR(" + m_colX + ",'mm')) ");
                    }
                }

                sb = unionTableQuery;

                //SET ORDER
            }
            else if (IsIdentifier_X())     //in case of the identifier column
            {
                StringBuilder unionTableQuery = new StringBuilder("SELECT ");
                string s_identifierCol = "";
                string fkTableName = "";
                if (s_colX.EndsWith("_ID"))
                {
                    MTable table = MColumn.Get(GetCtx(), this.GetAD_Column_X_ID()).GetFKTable();
                    fkTableName = table.GetTableName();
                    MColumn[] columns = table.GetColumns(false);
                    for (int i = 0; i <= columns.Length - 1; i++)
                    {
                        MColumn idn_column = MColumn.Get(GetCtx(), columns[i].Get_ID());
                        if (idn_column.IsIdentifier())
                        {
                            s_identifierCol = idn_column.GetName();
                            break;
                        }
                    }
                }

                //Building main query before union (if required)
                unionTableQuery.Append(IDENTIFIER_COL).Append(m_colX)
                    .Append(",");
                unionTableQuery.Append(IDENTIFIER_COL).Append(s_identifierCol)
                    .Append(" ColX,")
                    .Append(ApplyAggregateFunction(m_colY, true))
                    .Append(" ColY");

                unionTableQuery.Append(" FROM (");
                unionTableQuery.Append(sb.ToString());
                unionTableQuery.Append(") OUTT ");

                //JOIN WITH IDENTIFIER TABLE
                unionTableQuery.Append("INNER JOIN ")
                    .Append(fkTableName).Append(" idtnfr")
                    .Append(" ON ")
                    .Append(IDENTIFIER_COL).Append(m_colX)
                    .Append(" = OUTT.").Append(m_colX);

                //group by the whole query
                if (!IsNone())
                {
                    unionTableQuery.Append(" GROUP BY ")
                        .Append(IDENTIFIER_COL).Append(m_colX)
                        .Append(",")
                        .Append(IDENTIFIER_COL).Append(s_identifierCol);
                }

                sb = unionTableQuery;
            }

            if (IsDate_X() && (!isFiltered))
            {
                if (!IsNone())
                {
                    sb.Append(" GROUP BY ")
                        .Append(s_groupby);
                }
            }

            //add order by clause after group by clause
            string orderByColumn = GetOrderByColumn();
            //initially user had to select "asc". same field has now been set as ascending by default and
            //user can choose to make it descending. (for user friendly purpose ! you needed it :))
            string orderByMethod = IsOrderByAsc() ? "DESC" : "ASC";
            sb.Append(" ORDER BY ");

            if (orderByColumn.Equals("2"))
                sb.Append("ColY ").Append(orderByMethod);
            else
            {
                if (IsDate_X() && (this.GetDateTimeTypes() == IS_MONTHLY || this.GetDateTimeTypes() == IS_LAST_N_MONTHS))
                    sb.Append("2, " + orderByColumn).Append(" ").Append(orderByMethod);
                else if (IsDate_X() && (this.GetDateTimeTypes() == IS_DAILY || this.GetDateTimeTypes() == IS_LAST_N_DAYS || this.GetDateTimeTypes() == IS_CURRENT_WEEK))
                    sb.Append("2,3, " + orderByColumn).Append(" ").Append(orderByMethod);
                else
                    sb.Append(orderByColumn).Append(" ").Append(orderByMethod);
            }

            return sb.ToString();
        }




        public string GetSql(bool isFiltered, string specialWhere, Ctx _Ctx)
        {
            StringBuilder sb = new StringBuilder("SELECT ");    //start the SELECT QUERY
            MTable m_Table = null;// MTable.Get(GetCtx(), GetAD_Table_ID());
            //get the table name

            if (IsView())
            {
                m_Table = MTable.Get(GetCtx(), GetTableView_ID());
            }
            else
            {
                m_Table = MTable.Get(GetCtx(), GetAD_Table_ID());
            }
            //get the table name
            string s_tableName = m_Table.GetTableName();

            //get X Column Name
            MColumn column = MColumn.Get(_Ctx, this.GetAD_Column_X_ID());
            string s_colX = column.GetFKColumnName();

            column = MColumn.Get(_Ctx, this.GetAD_Column_Y_ID());

            string s_colY = column.GetFKColumnName();
            m_XFilter = GetDateTimeTypes();
            //Assign values to the member vairables
            m_colX = s_colX;
            m_colY = s_colY;
            m_tableName = s_tableName;

            string s_groupby = "";
            //Append X Axis to SQL Query

            //if datatype = date and user want to see all the dates (including with no values)
            if (IsDate_X() && (!isFiltered))
            {
                StringBuilder sbGroupBy = new StringBuilder("");
                sbGroupBy.Append(ApplyDateFunction(s_colX));
                if (GetDateTimeTypes() == IS_DAILY || GetDateTimeTypes() == IS_LAST_N_DAYS)
                    sbGroupBy.Append(", ")
                        .Append(ApplyDateFunction(s_colX, "yyyy"))
                        .Append(", ")
                        .Append(ApplyDateFunction(s_colX, "mm"))
                        .Append(", ").Append(ApplyFullDateFunction(s_colX, "dd/mm/yy"));
                else if (GetDateTimeTypes() == IS_MONTHLY || GetDateTimeTypes() == IS_LAST_N_MONTHS)
                    sbGroupBy.Append(", ")
                        .Append(ApplyDateFunction(s_colX, "yyyy"))
                        .Append(", ").Append(ApplyDateFunction(s_colX, "Mon-RRRR"));

                s_groupby = sbGroupBy.ToString();   //set value into variable to be used later for group by clause (!important)
                sb.Append(sbGroupBy.ToString());

            }
            else
                sb.Append(s_colX);  //in case of other than date and (no filter)

            //give name to col X (can be fetched from db too)
            if (!IsIdentifier_X())
                sb.Append(" ColX, ");
            else
                sb.Append(" , ");


            if (!IsIdentifier_X())
                sb.Append(ApplyAggregateFunction(s_colY, false));
            else //in case of identifier column, do not aggregate it (we will do it in top level query)
                sb.Append("NVL(")
                    .Append(s_colY)
                    .Append(",0) ").Append(s_colY);
            //sb.Append(",'9,999.99')");

            //give name to col Y
            if (!IsIdentifier_X())
                sb.Append(" ColY");

            sb.Append(" From ").Append(s_tableName);    //append table name

            string val = "";
            if (!string.IsNullOrEmpty(GetSQLWhere(out val)))    //get the where clause (actually it fectches only date condition. However, it can be used for other where clause purposes)
            {
                sb.Append(val);
                string whereClause = GetWhereClause();
                if (!string.IsNullOrEmpty(whereClause))
                {
                    whereClause = Env.ParseContext(_Ctx, 0, whereClause, false);
                    if (val.Equals(" WHERE "))
                        sb.Append("").Append(whereClause);
                    else
                        sb.Append("").Append(" AND " + whereClause);
                }
                else
                {
                    if (val.Equals(" WHERE "))
                        sb.Append("1=1");
                }

                //special where serves the purpose
                //for alerts where user needs alerts
                //for particular X value
                if (!string.IsNullOrEmpty(specialWhere))
                {
                    sb.Append(" ").Append(" AND " + specialWhere);
                }


                //Additional Filter
                MSeriesFilter[] filter = MSeriesFilter.GetFilters(this.Get_ID());
                StringBuilder sqlFilter = new StringBuilder();
                bool ifBetween = false;
                for (int i = 0; i <= filter.Length - 1; i++)
                {
                    if (filter[i] != null)
                    {
                        continue;
                    }


                    if (!val.Equals(" WHERE "))
                        sqlFilter.Append(" AND ");
                    string colValue = MColumn.Get(_Ctx, filter[i].GetAD_Column_ID()).GetColumnName();
                    sqlFilter.Append(colValue);

                    if (filter[i].GetWhereCondition() == "1")
                        sqlFilter.Append(" = ");
                    else if (filter[i].GetWhereCondition() == "2")
                        sqlFilter.Append(" != ");
                    else if (filter[i].GetWhereCondition() == "3")
                        sqlFilter.Append(" >= ");
                    else if (filter[i].GetWhereCondition() == "4")
                        sqlFilter.Append(" <= ");
                    else if (filter[i].GetWhereCondition() == "5")
                    {
                        sqlFilter.Append(" BETWEEN ");
                        ifBetween = true;
                    }

                    //put value
                    if (MColumn.Get(_Ctx, filter[i].GetAD_Column_ID()).getSQLDataType() == "DATE")
                    {
                        sqlFilter.Append("TO_DATE('").Append(filter[0].GetWhereValue()).Append("','dd/mm/RRRR'").Append(")");
                    }
                    else
                        sqlFilter.Append("'").Append(Env.ParseContext(_Ctx, 0, filter[i].GetWhereValue(), false)).Append("'");
                    if (ifBetween)
                    {
                        if (MColumn.Get(_Ctx, filter[i].GetAD_Column_ID()).getSQLDataType() == "DATE")
                        {
                            //sqlFilter.Append("TO_DATE('").Append(colValue).Append("','dd/mm/yyyy'").Append(")");
                            sqlFilter.Append(" AND ");
                            sqlFilter.Append("TO_DATE('").Append(filter[i].GetValueTo()).Append("','dd/mm/RRRR'").Append(")");
                        }
                        else
                            sqlFilter.Append(" AND '").Append(filter[i].GetValueTo()).Append("'");
                    }

                    if (sb.ToString().IndexOf("WHERE") > 0)
                        sb.Append(" ").Append(sqlFilter);

                }
            }

            //string datatype_Y = GetDataType_Y();
            // if (datatype_Y.Equals(IS_STRING) || datatype_Y.Equals(IS_INTEGER) && !IsNone())
            //Changes made on 5jul2011 to query view as we do with tables (req. by amardeep/done by jagmohan)
            if ((!IsIdentifier_X() && isFiltered) || IsString_X())
                sb.Append(AddGroupByClause(s_colX));

            if (IsDate_X() && isFiltered)
            {
                StringBuilder unionTableQuery = new StringBuilder("SELECT ");
                if (this.GetDateTimeTypes() == IS_MONTHLY || this.GetDateTimeTypes() == IS_LAST_N_MONTHS)
                {
                    unionTableQuery.Append(ApplyDateFunction(m_colX)).Append(", ")
                        .Append("TRIM(TO_CHAR(" + m_colX + ",'yyyy')), ")
                        .Append(ApplyAggregateFunction(m_colY, true)).Append(SPACE + "ColY");
                    unionTableQuery.Append(", TRIM(TO_CHAR(" + m_colX + ",'Mon-RRRR')) as ColX");
                }
                else if (this.GetDateTimeTypes() == IS_DAILY || this.GetDateTimeTypes() == IS_LAST_N_DAYS || this.GetDateTimeTypes() == IS_CURRENT_WEEK)
                {
                    unionTableQuery.Append(ApplyDateFunction(m_colX)).Append(", ")
                        .Append("TRIM(TO_CHAR(" + m_colX + ",'yyyy')), ")
                        .Append("TRIM(TO_CHAR(" + m_colX + ",'mm')), ")
                        .Append(ApplyAggregateFunction(m_colY, true)).Append(SPACE + "ColY");
                    unionTableQuery.Append(", TRIM(TO_CHAR(" + m_colX + ",'dd/mm/yy')) as ColX");
                }
                else
                    unionTableQuery.Append(ApplyDateFunction(m_colX))
                        .Append(" as ColX,")
                        .Append(ApplyAggregateFunction(m_colY, true))
                        .Append(SPACE + "ColY");

                unionTableQuery.Append(" FROM(");
                unionTableQuery.Append(GetUnionQuery(m_date_1, m_date_2, m_colX, "0 " + m_colY));
                //UNION
                unionTableQuery.Append(" UNION ALL ");

                unionTableQuery.Append(sb.ToString());

                unionTableQuery.Append(")");

                //group by the whole query
                if (!IsNone())
                {
                    unionTableQuery.Append(" GROUP BY ");
                    unionTableQuery.Append(ApplyDateFunction(m_colX));

                    if (this.GetDateTimeTypes() == IS_MONTHLY || this.GetDateTimeTypes() == IS_LAST_N_MONTHS)
                    {
                        unionTableQuery.Append(", TRIM(TO_CHAR(" + m_colX + ",'Mon-RRRR'))")
                            .Append(", TRIM(TO_CHAR(" + m_colX + ",'yyyy'))");
                    }
                    else if (this.GetDateTimeTypes() == IS_DAILY || this.GetDateTimeTypes() == IS_LAST_N_DAYS || this.GetDateTimeTypes() == IS_CURRENT_WEEK)
                    {
                        unionTableQuery.Append(", TRIM(TO_CHAR(" + m_colX + ",'dd/mm/yy'))")
                            .Append(", TRIM(TO_CHAR(" + m_colX + ",'yyyy')),")
                            .Append("TRIM(TO_CHAR(" + m_colX + ",'mm')) ");
                    }
                }

                sb = unionTableQuery;

                //SET ORDER
            }
            else if (IsIdentifier_X())     //in case of the identifier column
            {
                StringBuilder unionTableQuery = new StringBuilder("SELECT ");
                string s_identifierCol = "";
                string fkTableName = "";
                if (s_colX.EndsWith("_ID"))
                {
                    MTable table = MColumn.Get(_Ctx, this.GetAD_Column_X_ID()).GetFKTable();
                    fkTableName = table.GetTableName();
                    MColumn[] columns = table.GetColumns(false);
                    for (int i = 0; i <= columns.Length - 1; i++)
                    {
                        MColumn idn_column = MColumn.Get(_Ctx, columns[i].Get_ID());
                        if (idn_column.IsIdentifier())
                        {
                            s_identifierCol = idn_column.GetColumnName();
                            break;
                        }
                    }
                }

                //Building main query before union (if required)
                unionTableQuery.Append(IDENTIFIER_COL).Append(m_colX)
                    .Append(",");


                if (IsCount())
                {
                    unionTableQuery.Append(IDENTIFIER_COL).Append(s_identifierCol)
                     .Append(" ColX,")
                     .Append(ApplyAggregateFunction(m_colY, true))
                     .Append(" ColY");
                }
                else
                {

                    unionTableQuery.Append(IDENTIFIER_COL).Append(s_identifierCol)
                        .Append(" ColX,")
                        .Append(ApplyAggregateFunction("OUTT." + m_colY, true))
                        .Append(" ColY");
                }

                unionTableQuery.Append(" FROM (");
                unionTableQuery.Append(sb.ToString());
                unionTableQuery.Append(") OUTT ");

                //JOIN WITH IDENTIFIER TABLE
                unionTableQuery.Append("INNER JOIN ")
                    .Append(fkTableName).Append(" idtnfr")
                    .Append(" ON ")
                    .Append(IDENTIFIER_COL).Append(m_colX)
                    .Append(" = OUTT.").Append(m_colX);

                //group by the whole query
                if (!IsNone())
                {
                    unionTableQuery.Append(" GROUP BY ")
                        .Append(IDENTIFIER_COL).Append(m_colX)
                        .Append(",")
                        .Append(IDENTIFIER_COL).Append(s_identifierCol);
                }

                sb = unionTableQuery;
            }

            if (IsDate_X() && (!isFiltered))
            {
                if (!IsNone())
                {
                    sb.Append(" GROUP BY ")
                        .Append(s_groupby);
                }
            }

            //add order by clause after group by clause


            string orderByColumn = GetOrderByColumn();





            //initially user had to select "asc". same field has now been set as ascending by default and
            //user can choose to make it descending. (for user friendly purpose ! you needed it :))
            string orderByMethod = IsOrderByAsc() ? "DESC" : "ASC";
            sb.Append(" ORDER BY ");

            if (orderByColumn.Equals("2"))
                sb.Append("ColY ").Append(orderByMethod);
            else
            {
                if (IsDate_X() && (this.GetDateTimeTypes() == IS_MONTHLY || this.GetDateTimeTypes() == IS_LAST_N_MONTHS))
                    sb.Append("2 " + orderByMethod + "," + orderByColumn).Append(" ").Append(orderByMethod);
                else if (IsDate_X() && (this.GetDateTimeTypes() == IS_DAILY || this.GetDateTimeTypes() == IS_LAST_N_DAYS || this.GetDateTimeTypes() == IS_CURRENT_WEEK))
                    sb.Append("2,3, " + orderByColumn).Append(" ").Append(orderByMethod);
                else
                {
                    //if (IsDate_X())
                    //{
                    //    if (orderByColumn == "1")
                    //    {
                    //        orderByColumn = s_colX;
                    //    }
                    //}
                    sb.Append(orderByColumn).Append(" ").Append(orderByMethod);
                }
            }

            return sb.ToString();
        }


        /// <summary>
        /// Get Union Query for Gap filling
        /// </summary>
        /// <param name="date1">date from</param>
        /// <param name="date2">date to</param>
        /// <returns>Union Query</returns>
        private string GetUnionQuery(DateTime date1, DateTime date2, string colName, string function)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT dates." + colName + "," + function + " FROM");
            sb.Append("(SELECT TO_DATE('" + date1.ToString("MM/dd/yyyy") + "', 'MM/DD/RRRR') - 1 + rownum AS " + colName);

            sb.Append(" FROM all_objects");
            sb.Append(" WHERE TO_DATE('" + date1.ToString("MM/dd/yyyy") + "', 'MM/DD/RRRR') - 1 + rownum <= TO_DATE('" + date2.ToString("MM/dd/yyyy") + "', 'MM/DD/RRRR')) dates");
            sb.Append(" WHERE dates.").Append(colName).Append(">TO_DATE('01/01/0001', 'MM/DD/RRRR')");
            sb.Append(" GROUP BY dates." + colName);
            //sb.Append(" ORDER BY " + colName + " ASC");

            return sb.ToString();
        }


        /// <summary>
        /// Add group by clause in sql query
        /// </summary>
        /// <param name="colName">colname</param>
        /// <returns>group by query</returns>
        private string AddGroupByClause(string colName)
        {
            if (!IsNone())
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(" GROUP BY ").Append(colName);
                return sb.ToString();
            }
            return "";
        }

        /// <summary>
        /// Apply the datefunction in the query
        /// </summary>
        /// <param name="colName">Column Name</param>
        /// <returns>SQL Query with date function</returns>
        public string ApplyDateFunction(string colName)
        {
            //currently 3 types of date types are supported : daily, monthly, yearly
            string s_reportType = this.GetDateTimeTypes();
            StringBuilder sb = new StringBuilder("trim(to_char(").Append(colName);

            if (s_reportType.Equals("D") || s_reportType.Equals("A") || s_reportType.Equals("W"))
                sb.Append(",").Append("'dd'").Append("))");
            else if (s_reportType.Equals("M") || s_reportType.Equals("B"))
                sb.Append(",").Append("'mm'").Append("))");
            else if (s_reportType.Equals("Y") || s_reportType.Equals("C"))
                sb.Append(",").Append("'yyyy'").Append("))");
            else
            {
                sb = new StringBuilder("TO_Date(").Append(colName).Append(",'DD-mm-RRRR')");
            }

            return sb.ToString();

        }


        /// <summary>
        /// Apply the datefunction in the query
        /// </summary>
        /// <param name="colName">Column Name</param>
        /// <returns>SQL Query with date function</returns>
        public string ApplyFullDateFunction(string colName, string customFormat)
        {
            //currently 3 types of date types are supported : daily, monthly, yearly
            string s_reportType = this.GetDateTimeTypes();
            StringBuilder sb = new StringBuilder("TO_Date(").Append(colName);
            sb.Append(",").Append("'" + customFormat + "'");

            sb.Append(")");
            return sb.ToString();

        }


        /// <summary>
        /// Apply the datefunction in the query
        /// </summary>
        /// <param name="colName">Column Name</param>
        /// <returns>SQL Query with date function</returns>
        public string ApplyDateFunction(string colName, string customFormat)
        {
            //currently 3 types of date types are supported : daily, monthly, yearly
            string s_reportType = this.GetDateTimeTypes();
            StringBuilder sb = new StringBuilder("trim(to_char(").Append(colName);
            sb.Append(",").Append("'" + customFormat + "'");

            sb.Append("))");
            return sb.ToString();

        }

        /// <summary>
        /// Apply aggregate function
        /// </summary>
        /// <param name="colName">colName</param>
        /// <returns>SQL Query with aggregate function on X</returns>
        public string ApplyAggregateFunction(string colName, bool checkCountIssue)
        {
            StringBuilder sb = new StringBuilder();

            if (!this.IsNone())
                sb.Append("NVL(ROUND(");

            if (this.IsSum())
                sb.Append("SUM(").Append(colName).Append(")");
            else if (IsAvg())
                sb.Append("AVG(").Append(colName).Append(")");
            else if (IsCount())
            {
                if (IsDate_X() && checkCountIssue && (this.GetDateTimeTypes() == IS_YEARLY || this.GetDateTimeTypes() == IS_MONTHLY || this.GetDateTimeTypes() == IS_DAILY || this.GetDateTimeTypes() == IS_LAST_N_DAYS || this.GetDateTimeTypes() == IS_LAST_N_MONTHS || this.GetDateTimeTypes() == IS_LAST_N_YEARS))
                {
                    //sb.Append("CASE WHEN MOD(" + ApplyDateFunction(m_colX) + ",4) = 0 AND (MOD(" + ApplyDateFunction(m_colX) + ",400) = 0 OR MOD(" + ApplyDateFunction(m_colX) + ",100) <> 0) THEN COUNT(").Append(colName).Append(") - 366 ELSE COUNT(").Append(colName).Append(") - 365 END");
                    sb.Append("SUM(").Append(colName).Append(")");
                }
                else
                    sb.Append("COUNT(").Append("*").Append(")");
            }
            else
            {
                sb.Append(colName);
                return sb.ToString();
            }

            sb.Append(",3),0)");
            return sb.ToString();
        }

        /// <summary>
        /// Gets the Where clause of an SQL query
        /// </summary>
        /// <param name="sqlwhere">contains where clause (mind you! its an out variable)</param>
        private string GetSQLWhere(out string sqlwhere)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" WHERE ");
            if ((IsDate_X() || IsIdentifier_X() || IsString_X()) && GetDateTimeTypes() != "N")     //if colx is datetime
            {
                DateTime alternateDate = DateTime.Now;
                if (GetDateFrom() == null && (GetDateTimeTypes() == IS_YEARLY || GetDateTimeTypes() == IS_MONTHLY || GetDateTimeTypes() == IS_DAILY))
                {
                    IDataReader dr = DataBase.DB.ExecuteReader("SELECT MIN(" + m_colX + ") FROM " + m_tableName);
                    while (dr.Read())
                    {
                        try
                        {
                            alternateDate = DateTime.Parse(dr[0].ToString());
                        }
                        catch 
                        {
                            alternateDate = DateTime.MinValue;
                            continue;
                        }
                    }
                    dr.Close();
                }
                if (GetDateTimeTypes() == IS_MONTHLY)
                {
                    DateTime dt = DateTime.Now;
                    //set the datefrom and dateto for a search query
                    if (GetDateFrom() != null)
                    {
                        m_date_1 = (DateTime)GetDateFrom().Value;
                        //dt = GetDateFrom().Value.AddMonths(-1);
                        dt = GetDateFrom().Value;                        
                        //m_date1 = dt.Month + "/01/" + dt.Year;
                        m_date1 = dt.Month + "/"+dt.Day+"/" + dt.Year;
                    }
                    else
                    {
                        if (alternateDate.Year.Equals(DateTime.MinValue.Year))//if year is0001 and month is 1st then gives error. to resolve, this is done...
                        {
                            m_date_1 = (DateTime)alternateDate;
                            m_date1 = m_date_1.Month + "/01/" + m_date_1.Year;
                        }
                        else
                        {
                            m_date_1 = (DateTime)alternateDate;
                            dt = alternateDate.AddMonths(-1);
                            //m_date1 = dt.Month + "/01/" + dt.Year;
                            m_date1 = dt.Month + "/" + dt.Day + "/" + dt.Year;
                        }
                    }

                    if (GetDateTo() == null)
                    {
                        m_date_2 = DateTime.Now;
                        dt = DateTime.Now.AddMonths(1);
                        m_date2 = dt.Month + "/" + dt.Day + "/" + dt.Year;

                    }
                    else
                    {
                        m_date_2 = (DateTime)GetDateTo().Value;
                        //dt = GetDateTo().Value.AddMonths(1);
                        dt = GetDateTo().Value;

                         int tDays= DateTime.DaysInMonth(dt.Year, dt.Month);

                         if (tDays == dt.Day)
                         {
                             m_date2 = dt.Month + "/" + (dt.Day) + "/" + dt.Year;
                         }
                         else
                         {
                             m_date2 = dt.Month + "/" + (dt.Day + 1) + "/" + dt.Year;
                         }
                    }

                    //sb.Append(m_colX).Append(" BETWEEN ");
                    //sb.Append(GetFormattedDateColumn(m_date1, "mm/dd/yyyy")).Append(" AND ");
                    //sb.Append(GetFormattedDateColumn(m_date2, "mm/dd/yyyy"));
                }
                else if (GetDateTimeTypes() == IS_YEARLY)
                {
                    DateTime dt = DateTime.Now;

                    if (GetDateFrom() != null)
                    {
                        m_date_1 = (DateTime)GetDateFrom().Value;
                        //dt = GetDateFrom().Value.AddYears(-1);
                        //m_date1 = "01/01/" + dt.Year;
                        dt = GetDateFrom().Value;
                        m_date1 = dt.Month + "/" + dt.Day + "/" + dt.Year;
                    }
                    else
                    {
                        if (alternateDate.Year.Equals(DateTime.MinValue.Year))
                        {
                            m_date_1 = (DateTime)alternateDate;
                            m_date1 = "01/01/" + +m_date_1.Year;
                        }
                        else
                        {
                            m_date_1 = (DateTime)alternateDate;
                            dt = alternateDate.AddYears(-1);
                            m_date1 = "01/01/" + dt.Year;
                        }
                    }

                    if (GetDateTo() == null)
                    {
                        m_date_2 = DateTime.Now;
                        dt = DateTime.Now.AddYears(1);
                        m_date2 = "12/31/" + dt.Year;
                    }
                    else
                    {
                        m_date_2 = (DateTime)GetDateTo().Value;
//                        dt = GetDateTo().Value.AddYears(1);
                        //m_date2 = "12/31/" + dt.Year;
                        dt = GetDateTo().Value;

                        int tDays= DateTime.DaysInMonth(dt.Year, dt.Month);

                        if (tDays == dt.Day)
                        {
                            m_date2 = dt.Month + "/" + (dt.Day) + "/" + dt.Year;
                        }
                        else
                        {
                            m_date2 = dt.Month + "/" + (dt.Day + 1) + "/" + dt.Year;
                        }
                    }

                }
                else if (GetDateTimeTypes() == IS_DAILY)
                {
                    DateTime dt = DateTime.Now;
                    if (GetDateFrom() != null)
                    {
                        m_date_1 = (DateTime)GetDateFrom().Value;
                        //dt = GetDateFrom().Value.AddDays(-1);
                        dt = GetDateFrom().Value;
                        m_date1 = dt.Month + "/" + dt.Day + "/" + dt.Year;
                    }
                    else
                    {
                        if (alternateDate.Year.Equals(DateTime.MinValue.Year))
                        {
                            m_date_1 = (DateTime)alternateDate;
                            m_date1 = m_date_1.Month + "/" + m_date_1.Day + "/" + m_date_1.Year;
                        }
                        else
                        {
                            m_date_1 = (DateTime)alternateDate;
                            dt = alternateDate.AddDays(-1);
                            m_date1 = dt.Month + "/" + dt.Day + "/" + dt.Year;
                        }
                    }

                    if (GetDateTo() == null)
                    {
                        m_date_2 = DateTime.Now;
                        dt = DateTime.Now.AddDays(1);
                        m_date2 = dt.Month + "/" + dt.Day + "/" + dt.Year;
                    }
                    else
                    {
                        m_date_2 = (DateTime)GetDateTo().Value;
                        //dt = GetDateTo().Value.AddDays(1);                        
                        dt = GetDateTo().Value;

                         int tDays= DateTime.DaysInMonth(dt.Year, dt.Month);

                         if (tDays == dt.Day)
                         {
                             m_date2 = dt.Month + "/" + (dt.Day) + "/" + dt.Year;
                         }
                         else
                         {
                             m_date2 = dt.Month + "/" + (dt.Day + 1) + "/" + dt.Year;
                         }
                    }
                }
                else if (GetDateTimeTypes() == IS_LAST_N_DAYS)
                {
                    DateTime dt_from = DateTime.Now.AddDays(-1);
                    DateTime dt_to = DateTime.Now.AddDays(1);

                    DateTime lastNDate = dt_from.AddDays(-(this.GetLastNValue() - 1));
                    m_date_1 = (DateTime)lastNDate.AddDays(1);
                    m_date_2 = DateTime.Now;
                    //m_date1 = lastNDate.Month + "/" + (lastNDate.Day - 1) + "/" + lastNDate.Year;
                    
                    m_date1 = lastNDate.Month + "/" + (lastNDate.Day) + "/" + lastNDate.Year;
                    m_date2 = dt_to.Month + "/" + dt_to.Day + "/" + dt_to.Year;
                    
                }
                else if (GetDateTimeTypes() == IS_LAST_N_MONTHS)
                {
                    DateTime dt_from = DateTime.Now.AddMonths(-1);
                    DateTime dt_to = DateTime.Now.AddMonths(1);

                    DateTime lastNDate = dt_from.AddMonths(-(this.GetLastNValue() - 1));
                    m_date_1 = (DateTime)lastNDate.AddMonths(1);
                    m_date_2 = DateTime.Now;

                   // m_date1 = (lastNDate.Month - 1) + "/" + lastNDate.Day + "/" + lastNDate.Year; 
                    //if ((lastNDate.Month - 1) == 0)
                    //{
                    //    m_date1 = "12/" + lastNDate.Day + "/" + (lastNDate.Year - 1);
                    //}
                    //else
                    //{
                        //m_date1 = (lastNDate.Month - 1) + "/" + lastNDate.Day + "/" + lastNDate.Year;
                    m_date1 = (lastNDate.Month) + "/" + lastNDate.Day + "/" + lastNDate.Year;
                   // }                     
                    //m_date2 = dt_to.Month + "/" + dt_to.Day + "/" + dt_to.Year;
                    int tDays = DateTime.DaysInMonth(dt_to.Year, dt_to.Month);

                    if (tDays == dt_to.Day)
                     {
                         m_date2 = (dt_to.Month - 1) + "/" + (dt_to.Day) + "/" + dt_to.Year;
                     }
                     else
                     {
                         m_date2 = (dt_to.Month - 1) + "/" + (dt_to.Day + 1) + "/" + dt_to.Year;
                     }
                }
                else if (GetDateTimeTypes() == IS_LAST_N_YEARS)
                {
                    DateTime dt_from = DateTime.Now;
                    DateTime dt_to = DateTime.Now;
                    //DateTime dt_to = DateTime.Now.AddYears(1);

                    DateTime lastNDate = dt_from.AddYears(-(this.GetLastNValue() - 1));
                    m_date_1 = (DateTime)lastNDate;
                    m_date_2 = DateTime.Now;

                    m_date1 = lastNDate.Month + "/" + lastNDate.Day + "/" + (lastNDate.Year - 1);
                    int tDays = DateTime.DaysInMonth(dt_to.Year, dt_to.Month);

                    if (tDays == dt_to.Day)
                    {
                        m_date2 = dt_to.Month + "/" + (dt_to.Day) + "/" + dt_to.Year;
                    }
                    else
                    {
                        m_date2 = dt_to.Month + "/" + (dt_to.Day + 1) + "/" + dt_to.Year;
                    }
                }
                else if (GetDateTimeTypes() == IS_CURRENT_WEEK)
                {
                    //DateTime dt_from = GetDateFrom().Value.AddYears(-1);
                    //DateTime dt_to = GetDateFrom().Value.AddYears(1);

                    DayOfWeek today = DateTime.Now.DayOfWeek;
                    DateTime firstDate = DateTime.Now.AddDays(-(int)today);
                    m_date1 = firstDate.Month + "/" + firstDate.Day + "/" + firstDate.Year;
                    m_date2 = DateTime.Now.Month + "/" + DateTime.Now.Day + "/" + DateTime.Now.Year;
                }


                if (IsIdentifier_X() || IsString_X())
                {
                    MColumn dateCol = MColumn.Get(GetCtx(), GetAD_DateColumn_ID());
                    sb.Append(dateCol.GetFKColumnName()).Append(" BETWEEN ");
                }
                else
                    sb.Append(m_colX).Append(" BETWEEN ");

                sb.Append(GetFormattedDateColumn(m_date1, "mm/dd/RRRR")).Append(" AND ");
                sb.Append(GetFormattedDateColumn(m_date2, "mm/dd/RRRR"));

            }
            sqlwhere = sb.ToString();
            return sqlwhere;
        }



        /// <summary>
        /// Get formatted date column (dd, mm, yyyy)
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public string GetFormattedDateColumn(string colName, string format)
        {
            return " TRIM(TO_DATE('" + colName + "','" + format + "'))";
        }

        /// <summary>
        /// Get formatted date column (dd, mm, yyyy)
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public string GetFormattedDateDBColumn(string colName, string format)
        {
            return " TRIM(TO_DATE(" + colName + ",'" + format + "'))";
        }


        /// <summary>
        /// Get all the series from the database
        /// </summary>
        /// <returns>All the series available in database</returns>
        static public MSeries[] GetAll(Ctx ctx)
        {
            List<MSeries> list = new List<MSeries>();
            String sql = "SELECT * FROM D_Series pfi "
                + "WHERE pfi.IsActive='Y' AND ISSETALERT='Y' AND "
                + "AD_CLIENT_ID=" + ctx.GetAD_Client_ID();
            try
            {
                DataSet ds = SqlExec.ExecuteQuery.ExecuteDataset(sql);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    MSeries pfi = new MSeries(ctx, dr, null);
                    list.Add(pfi);
                }
            }
            catch (Exception e)
            {
                s_log.Severe(e.ToString());
            }
            MSeries[] retValue = new MSeries[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        private enum WhereCondition
        {
            EqualsTo = 1,
            NoEqualsTo = 2,
            GreaterThanEqualsTo = 3,
            LessThanEqualsTo = 4,
            Between = 5
        }


        private bool MatchCondition(int where, string original_x, string match_x, string orginal_y, string match_y, string matchto_y)
        {
            //bool returnValue = false;
            if (original_x.Equals(match_x) || string.IsNullOrEmpty(match_x))
            {
                if (where == (int)WhereCondition.GreaterThanEqualsTo)
                {
                    if (double.Parse(orginal_y) >= double.Parse(match_y))
                    {
                        return true;
                    }
                }
                else if (where == (int)WhereCondition.EqualsTo)
                {
                    if (double.Parse(orginal_y) == double.Parse(match_y))
                    {
                        return true;
                    }
                }
                else if (where == (int)WhereCondition.NoEqualsTo)
                {
                    if (double.Parse(orginal_y) != double.Parse(match_y))
                    {
                        return true;
                    }
                }
                else if (where == (int)WhereCondition.LessThanEqualsTo)
                {
                    if (double.Parse(orginal_y) <= double.Parse(match_y))
                    {
                        return true;
                    }
                }
                if (where == (int)WhereCondition.Between)
                {
                    if (double.Parse(orginal_y) >= double.Parse(match_y) && int.Parse(orginal_y) <= int.Parse(matchto_y))
                    {
                        return true;
                    }
                }
            }
            else
            {
            }
            return false;

        }

        private int m_totalMatch = 0;
        /// <summary>
        /// Count the number of match in an instance
        /// </summary>
        /// <returns></returns>
        public int CountMatch()
        {
            return m_totalMatch;
        }


        //static String AD_MessageValue = "DashBoardAlerts";


        /// <summary>
        /// Gets the list of all the series with qualified alerts
        /// </summary>
        /// <returns>Alerts in List</returns>
        static public List<MSeries> GetAlertList(Ctx ctx, out List<string> msg)
        {
            MSeries[] m_AlertSeries = GetAll(ctx);
            List<string> m_AlertMsg = new List<string>();
            List<MSeries> m_AlertList = new List<MSeries>();
            foreach (MSeries series in m_AlertSeries)
            {
                if (series.IsSetAlert())
                {
                    if (!string.IsNullOrEmpty(series.GetWhereCondition()) || !string.IsNullOrEmpty(series.GetAlertValue()))
                    {
                        string sql = "";

                        if (string.IsNullOrEmpty(series.GetSqlQuery()))
                        {
                            string specialWhere = "";
                            if (series.IsDate_X())
                            {

                                if (series.IsDateType_Daily() || series.IsDateType_LastNDays())
                                {
                                    if (series.GetAlertLastRun() != null)
                                    {
                                        if (DateTime.Parse(series.GetAlertLastRun().ToString()).ToString("dd/MM/RRRR") == DateTime.Now.ToString("dd/MM/RRRR"))
                                        {
                                            continue;
                                        }
                                    }
                                    specialWhere = series.GetFormattedDateDBColumn(series.GetColumnName(series.GetAD_Column_X_ID()), "dd/mm/RRRR") + "=" + series.GetFormattedDateDBColumn("SYSDATE", "dd/mm/RRRR");
                                    sql = series.GetSql(false, specialWhere);

                                }
                                else if (series.IsDateType_Monthly() || series.IsDateType_LastNMonths())
                                {
                                    if (series.GetAlertLastRun() != null)
                                    {
                                        if (DateTime.Parse(series.GetAlertLastRun().ToString()).ToString("MM/yyyy") == DateTime.Now.ToString("MM/yyyy"))
                                        {
                                            continue;
                                        }
                                    }
                                    specialWhere = series.ApplyDateFunction(series.GetColumnName(series.GetAD_Column_X_ID()), "mm") + "=" + DateTime.Now.Month + " AND "
                                        + series.ApplyDateFunction(series.GetColumnName(series.GetAD_Column_X_ID()), "yyyy") + "=" + DateTime.Now.Year;

                                    sql = series.GetSql(false, specialWhere);

                                }
                                else if (series.IsDateType_Yearly() || series.IsDateType_LastNYears())
                                {
                                    if (series.GetAlertLastRun() != null)
                                    {
                                        if (DateTime.Parse(series.GetAlertLastRun().ToString()).ToString("yyyy") == DateTime.Now.ToString("yyyy"))
                                        {
                                            continue;
                                        }
                                    }

                                    specialWhere = series.ApplyDateFunction(series.GetColumnName(series.GetAD_Column_X_ID()), "yyyy") + "=" + DateTime.Now.Year;
                                    sql = series.GetSql(false, specialWhere);

                                }

                            }
                            else
                                sql = series.GetSql(false, "");
                        }
                        else
                            sql = series.GetSqlQuery();

                        IDataReader dr = DataBase.DB.ExecuteReader(sql);
                        StringBuilder sb = new StringBuilder("");
                        int total_match = 0;

                        while (dr.Read())
                        {
                            bool match = false;

                            if (series.IsIdentifier_X())
                                match = series.MatchCondition(int.Parse(series.GetWhereCondition()), dr[0].ToString(), series.GetAlertValue_X(), dr["ColY"].ToString(), series.GetAlertValue(), series.GetValueTo());
                            else
                                match = series.MatchCondition(int.Parse(series.GetWhereCondition()), dr["ColX"].ToString(), series.GetAlertValue_X(), dr["ColY"].ToString(), series.GetAlertValue(), series.GetValueTo());
                            if (match)
                            {
                                //MNote notes = new MNote(GetCtx(), MMessage.GetAD_Message_ID(VAdvantage.Utility.GetCtx(), AD_MessageValue), Env.GetContext().GetAD_User_ID(), series.GetAD_Client_ID(), series.GetAD_Org_ID(), series.Get_Table_ID(), series.Get_ID(), dr[0].ToString(), null);
                                //if (series.IsIdentifier_X() || series.IsString_X())
                                //    notes.SetReference(dr[0].ToString());
                                //else
                                //    notes.SetReference(series.ToString());

                                //notes.SetRecord(series.Get_Table_ID(), series.Get_ID());
                                //notes.SetTextMsg("Value Matched");
                                //notes.Save();

                                s_log.Info("Alert Matched");
                                total_match++;
                                if (!m_AlertList.Contains(series))
                                    m_AlertList.Add(series);

                                sb.Append(dr["ColX"].ToString()).Append(",");
                            }
                        }
                        dr.Close();


                        //Delete 

                        if (series.IsString_X())
                        {
                            series.m_totalMatch = total_match;
                            if (series.GetStartValue() == total_match)
                            {
                                if (m_AlertList.Contains(series))
                                    m_AlertList.Remove(series);
                            }
                        }

                        if (m_AlertList.Contains(series))
                        {
                            if (!string.IsNullOrEmpty(sb.ToString()))
                            {

                                StringBuilder sbMsg = new StringBuilder();
                                sbMsg.Append("ALERT!");
                                if (int.Parse(series.GetWhereCondition()) == (int)WhereCondition.NoEqualsTo)
                                {
                                    sbMsg.Append(" Following X Values are not equal to " + series.GetAlertValue());
                                }
                                else if (int.Parse(series.GetWhereCondition()) == (int)WhereCondition.EqualsTo)
                                {
                                    sbMsg.Append(" Following X Values are equal to " + series.GetAlertValue());
                                }
                                else if (int.Parse(series.GetWhereCondition()) == (int)WhereCondition.Between)
                                {
                                    sbMsg.Append(" Following X Values are between " + series.GetAlertValue()).Append(" & ").Append(series.GetValueTo());
                                }
                                else if (int.Parse(series.GetWhereCondition()) == (int)WhereCondition.GreaterThanEqualsTo)
                                {
                                    sbMsg.Append(" Following X Values are greater than " + series.GetAlertValue());
                                }
                                else if (int.Parse(series.GetWhereCondition()) == (int)WhereCondition.LessThanEqualsTo)
                                {
                                    sbMsg.Append(" Following X Values are less than " + series.GetAlertValue());
                                }

                                sbMsg.Append("\n " + sb.ToString());
                                m_AlertMsg.Add(sbMsg.ToString().Substring(0, sbMsg.Length - 1));
                            }
                        }
                    }
                }
            }

            msg = m_AlertMsg;
            return m_AlertList;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("MSeries[");
            sb.Append(Get_ID()).Append(",AD_Table_ID=").Append(GetAD_Table_ID());
            sb.Append(",Name=").Append(GetName());
            sb.Append("]");

            return sb.ToString();
        }

        public bool IsIdentifier_X()
        {
            if (GetDataType_X() == IS_IDENTIFIER)
                return true;

            return false;
        }

        public bool IsDate_X()
        {
            if (GetDataType_X() == IS_DATETIME)
                return true;
            return false;
        }

        public bool IsString_X()
        {
            if (GetDataType_X() == IS_STRING)
                return true;

            return false;
        }

        public bool IsDateType_LastNDays()
        {
            if (GetDateTimeTypes() == IS_LAST_N_DAYS)
                return true;
            return false;
        }
        public bool IsDateType_LastNMonths()
        {
            if (GetDateTimeTypes() == IS_LAST_N_MONTHS)
                return true;
            return false;
        }

        public bool IsDateType_LastNYears()
        {
            if (GetDateTimeTypes() == IS_LAST_N_YEARS)
                return true;
            return false;
        }



        public bool IsDateType_Yearly()
        {
            if (GetDateTimeTypes() == IS_YEARLY)
                return true;
            return false;
        }

        public bool IsDateType_Monthly()
        {
            if (GetDateTimeTypes() == IS_MONTHLY)
                return true;
            return false;
        }
        public bool IsDateType_Daily()
        {
            if (GetDateTimeTypes() == IS_DAILY)
                return true;
            return false;
        }

        public string GetColumnName(int AD_Column_ID)
        {
            MColumn column = MColumn.Get(GetCtx(), AD_Column_ID);
            return column.GetFKColumnName();
        }

    }
}
