using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.Logging;

namespace VAdvantage.Classes
{
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