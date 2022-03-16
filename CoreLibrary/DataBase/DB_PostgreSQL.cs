using CoreLibrary.DataBase;
using Npgsql;
/********************************************************
 * Module/Class Name  : Postgre Database Classes
 * Purpose            : Convert the oracle sql query to postgrey queries 
 *                  
 * Chronological Development
 * Mukesh Arora     8-Dec-2008
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using VAdvantage.DBPort;
using VAdvantage.Logging;

namespace VAdvantage.DataBase
{
    public class DB_PostgreSQL : ViennaDatabase
    {
        /// <summary>
        /// PostgreSQL Database
        /// </summary>
        public DB_PostgreSQL()
        {
        }

        /** Default Port            */
        public static int DEFAULT_PORT = 5432;

        /** Statement Converter     */
        private ConvertSQL m_convert = new ConvertSQL_PostgreSQL();

        /** Cached Database Name	*/
        private String m_dbName = null;

        private String m_userName = null;

        /**	Logger			*/
        private static VLogger log = VLogger.GetVLogger(typeof(DB_PostgreSQL).FullName);

        public static String NATIVE_MARKER = "NATIVE_" + DatabaseType.DB_POSTGRESQL + "_KEYWORK";

        private string connectionString = null;
        private System.Data.IDbConnection con = null;

        /// <summary>
        ///	   Convert an individual Oracle Style statements to target database statement syntax.
        /// </summary>
        /// <param name="oraStatement">oracle statement</param>
        /// <returns>converted Statement from oracle statement</returns>
        public String ConvertStatement1(String oraStatement)
        {
            String[] retValue = m_convert.Convert(oraStatement);

            if (retValue.Length == 0)
                return oraStatement;


            if (retValue.Length != 1)

            {
                log.Log(Level.SEVERE, ("DB_PostgreSQL.convertStatement - Convert Command Number=" + retValue.Length
                    + " (" + oraStatement + ") - " + m_convert.GetConversionError()));
                throw new ArgumentException
                    ("DB_PostgreSQL.convertStatement - Convert Command Number=" + retValue.Length
                        + " (" + oraStatement + ") - " + m_convert.GetConversionError());
            }


            return retValue[0];



        }   //  convertStatement

        public String ConvertStatement(string oraStatement)
        {
            if (oraStatement.Contains("\t"))
            {
                oraStatement = oraStatement.Replace("\t", " ").Trim();
            }

            if (oraStatement.StartsWith("ALTER TABLE") && (oraStatement.IndexOf(" MODIFY ") > 0))
            {
                String[] tokens = oraStatement.Split(' ');
                String sql = "ALTER TABLE " + tokens[2] + " ALTER " + tokens[4];
                int idef = oraStatement.IndexOf(" DEFAULT ");
                if ((idef > 0) || (oraStatement.IndexOf(" NULL") < 0))
                {
                    int i = sql.Length + 1; //alter v.s. modify
                    if (idef > 0)
                    {
                        if (!("DEFAULT".Equals(tokens[5]) || (tokens[5].Length == 0)))
                        {
                            sql += " TYPE " + oraStatement.Substring(i, (idef + 1) - i); //type stuff
                            sql += ", ALTER " + tokens[4];
                        }
                        sql += " SET DEFAULT " + oraStatement.Substring(idef + 9, (oraStatement.Length) - (idef + 9));
                    }
                    else
                    {
                        int rpDrop = 0;
                        if ((oraStatement.IndexOf(" MODIFY (") > 0) || (oraStatement.IndexOf(" MODIFY(") > 0))
                            rpDrop = 1;
                        sql += " TYPE " + oraStatement.Substring(i, oraStatement.Length - rpDrop);
                    }
                    oraStatement = sql;
                }
                else
                {
                    if (oraStatement.IndexOf(" NOT NULL") > 0)
                    {
                        sql += " SET NOT NULL";
                        return sql;
                    }
                    else if (oraStatement.IndexOf(" NULL") > 0)
                    {
                        sql += " DROP NOT NULL";
                        return sql;
                    }
                }
            }

            if (oraStatement.StartsWith("CREATE UNIQUE INDEX ") && (oraStatement.IndexOf("TO_NCHAR(AD_User_ID)") > 0)) //jz hack number pad
            {
                oraStatement = oraStatement.Replace("TO_NCHAR(AD_User_ID)", "TO_CHAR(AD_User_ID,'9999999')::VARCHAR");
            }

            if (oraStatement.StartsWith("ALTER TABLE") && (oraStatement.IndexOf(" ADD (") > 0)) //jz remove () for add
            {
                oraStatement = oraStatement.Replace(" ADD (", " ADD ");
                oraStatement = oraStatement.Substring(0, oraStatement.Length - 1);
            }

            if (oraStatement.StartsWith("UPDATE")) //jz use tablename to replace co-relation id
            {
                //    while (oraStatement.IndexOf('\t') > -1)
                //        oraStatement = oraStatement.Replace('\t', ' ');
                //    String[] tokens = oraStatement.Split(' ');
                //    if (!"SET".Equals(tokens[2].ToUpper()))
                //    {
                //        String[] sep = { " ", "=", ">", "<", "(", "," };
                //        String crid = tokens[2] + ".";
                //        String ncrid = tokens[1] + ".";

                //        oraStatement = oraStatement.Replace(" " + tokens[2] + " ", " ");
                //        foreach (String element in sep)
                //        {
                //            String crid1 = (element + crid).ToUpper();
                //            String ncrid1 = element + ncrid;
                //            int l = crid1.Length;
                //            int ix = oraStatement.ToUpper().IndexOf(crid1);
                //            while (ix > -1)
                //            {
                //                int sl = oraStatement.Length;
                //                string s = oraStatement;
                //                int x1 = ix + 1;
                //                int y1 = sl - x1;
                //                oraStatement = s.Substring(0, ix) + ncrid1 + s.Substring(x1, y1);
                //                ix = oraStatement.ToUpper().IndexOf(crid1);
                //                //oraStatement = oraStatement.replace(crid1, ncrid1);
                //            }
                //        }
                //    }

                oraStatement = DBUtils.UpdateSetSelectList(oraStatement);
            }

            if (oraStatement.StartsWith("DELETE FROM ")) //jz use tablename to replace co-relation id
            {
                String[] tokens = oraStatement.Split(' ');
                if ((tokens.Length > 3) && !"WHERE".Equals(tokens[3]))
                {
                    String[] sep = { " ", "=", ">", "<", "(", "," };
                    String crid = tokens[3] + ".";
                    String ncrid = tokens[2] + ".";

                    oraStatement = oraStatement.Replace(" " + tokens[3] + " ", " ");
                    foreach (String element in sep)
                    {
                        String crid1 = element + crid;
                        String ncrid1 = element + ncrid;
                        while (oraStatement.IndexOf(crid1) > -1)
                            oraStatement = oraStatement.Replace(crid1, ncrid1);
                    }
                }
            }

            StringBuilder sb = new StringBuilder(oraStatement.ToString());

            while (sb.ToString().IndexOf("NUMBER(10,0)") > -1)
                sb.Replace("NUMBER(10,0)", "INTEGER");
            while (sb.ToString().IndexOf("NUMBER(10)") > -1)
                sb.Replace("NUMBER(10)", "INTEGER");
            while (sb.ToString().IndexOf("NUMERIC(10,0)") > -1)
                sb.Replace("NUMERIC(10,0)", "INTEGER");
            while (sb.ToString().IndexOf("NUMERIC(10)") > -1)
                sb.Replace("NUMERIC(10)", "INTEGER");


            //sb.Replace("NVARCHAR2", "VARCHAR");
            //sb.Replace("SYSDATE", "CURRENT_TIMESTAMP");
            //sb.Replace(", New", ", "+ '"' + "New" + '"');
            //sb.Replace(", NEW", ", " + '"' + "NEW" + '"');

            //sb.Replace("END CASE", "END"); // added by veena on 11 Aug

            //Added By Karan for saving universal time in DB.
            sb.Replace("SYS_EXTRACT_UTC(SYSTIMESTAMP)", "CURRENT_TIMESTAMP at time zone 'UTC'");
            // Added By Karan for creating view in Postgray.
            sb.Replace("CREATE OR REPLACE FORCE VIEW", "CREATE OR REPLACE VIEW");

            String[] retValue = m_convert.Convert(sb.ToString());
            if (retValue == null)
            {
                log.Warning("Not Converted (" + oraStatement + ") - "
                        + m_convert.GetConversionError());
                return oraStatement;
            }
            if (retValue.Length != 1)
            {
                log.Warning("Convert error! Converted statement Number=" + retValue.Length
                        + " (" + oraStatement + ") - " + m_convert.GetConversionError());
                return oraStatement;
            }
            //  Diagnostics (show changed, but not if AD_Error

            return retValue[0];

        } //  convertStatement

        public int GetStandardPort()
        {
            return DEFAULT_PORT;
        }

        public bool SupportsBLOB()
        {
            return true;
        }


        /// <summary>
        /// Create SQL TO Date String from Timestamp
        /// </summary>
        /// <param name="time">time Date to be converted</param>
        /// <param name="dayOnly">dayOnly true if time set to 00:00:00</param>
        /// <returns>TO_DATE('2001-01-30 18:10:20',''YYYY-MM-DD HH24:MI:SS')</returns>
        public String TO_DATE(DateTime? time, bool dayOnly)
        {

            StringBuilder dateString = new StringBuilder("");
            string myDate = "";

            if (time == null)
            {
                if (dayOnly)
                    return "current_date";
                return "current_date";
            }

            dateString = new StringBuilder("TO_DATE('");
            //  YYYY-MM-DD HH24:MI:SS.mmmm  JDBC Timestamp format
            //String myDate = time.ToString("yyyy-mm-dd");
            //myDate = time.ToString("yyyy-MM-dd HH:mm:ss");
            myDate = time.Value.ToString();//"yyyy-MM-dd");
            if (dayOnly)
            {
                myDate = time.Value.ToString("yyyy-MM-dd");
                dateString.Append(myDate);
                dateString.Append("','YYYY-MM-DD')");
            }
            else
            {
                myDate = time.Value.ToString("yyyy-MM-dd HH:mm:ss");
                dateString.Append(myDate);	//	cut off miliseconds
                dateString.Append("','YYYY-MM-DD HH24:MI:SS')");
            }
            return dateString.ToString();
        }   //  TO_DATE

        public string TO_CHAR(string columnName, int displayType, string AD_Language)
        {
            StringBuilder retValue = new StringBuilder("CAST (");
            retValue.Append(columnName);
            retValue.Append(" AS Text)");

            //  Numbers
            /*
            if (DisplayType.isNumeric(displayType))
            {
                if (displayType == DisplayType.Amount)
                    retValue.append(" AS TEXT");
                else
                    retValue.append(" AS TEXT");			
                //if (!Language.isDecimalPoint(AD_Language))      //  reversed
                //retValue.append(",'NLS_NUMERIC_CHARACTERS='',.'''");
            }
            else if (DisplayType.isDate(displayType))
            {
                retValue.append(",'")
                    .append(Language.getLanguage(AD_Language).getDBdatePattern())
                    .append("'");
            }
            retValue.append(")");
            //*/
            return retValue.ToString();
        }

        public string TO_NUMBER(decimal? number, int displayType)
        {
            if (number == null)
                return "NULL";
            Decimal result = number.Value;
            //int scale = VAdvantage.Classes.DisplayType.GetDefaultPrecision(displayType);
            //if (scale > Decimal.  number.Value.())
            //{
            //    try
            //    {
            //        result = number.setScale(scale, BigDecimal.ROUND_HALF_UP);
            //    }
            //    catch (Exception e)
            //    {
            //        //  log.severe("Number=" + number + ", Scale=" + " - " + e.getMessage());
            //    }
            //}
            return result.ToString();
        }

        /// <summary>
        /// Get next ID fromm Sequence
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public int GetNextID(string Name)
        {
            int m_sequence_id = DB.GetSQLValue(null, "SELECT nextval('" + Name.ToUpper() + "')");
            return m_sequence_id;
        }

        /// <summary>
        /// Drop Existing if any and create new Sequence.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="increment"></param>
        /// <param name="minvalue"></param>
        /// <param name="maxvalue"></param>
        /// <param name="start"></param>
        /// <param name="trxName"></param>
        /// <returns></returns>
        public bool CreateSequence(string name, int increment, int minvalue, int maxvalue, int start, Trx trxName)
        {
            int no = DB.ExecuteQuery("DROP SEQUENCE " + name.ToUpper() + "_SEQ", null, trxName);
            string sql= "CREATE SEQUENCE " + name.ToUpper() + "_SEQ"
                                + " MINVALUE " + minvalue
                                + " MAXVALUE " + maxvalue
                                + " START WITH " + start
                                + " INCREMENT BY " + increment;

            no = DB.ExecuteQuery(sql, null, trxName);

            if (no == -1)
                return false;
            else
                return true;
        }



        public string GetName()
        {
            return DatabaseType.DB_POSTGRESQL;
        }




        public System.Data.IDbConnection GetCachedConnection(bool autoCommit, int transactionIsolation)
        {
            if (con == null)
            {
                con = new NpgsqlConnection(connectionString);

            }
            return con;
        }

        public void SetConnectionString(string conString)
        {
            connectionString = conString;
        }


        public System.Data.DataSet ExecuteDatasetPaging(string sql, int page, int pageSize, int increment)
        {
            DataSet ds = null;
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            try
            {


                connection.Open();
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter();
                adapter.SelectCommand = new NpgsqlCommand(sql + " limit " + pageSize + " offset " + ((page - 1) * pageSize));
                adapter.SelectCommand.CommandTimeout = 150;
                adapter.SelectCommand.Connection = (NpgsqlConnection)connection;
                ds = new DataSet();

                if (page < 1)// Set rowcount =PageNumber * PageSize for best performance
                {
                    page = 1;
                }
                //adapter.Fill(ds, ((page - 1) * pageSize) + increment, pageSize - increment, "Data");
                //adapter.Fill(ds, ((page - 1) * pageSize), pageSize, "Data");
                adapter.Fill(ds, "Data");
                //adapter.FillSchema(ds, SchemaType.Mapped, "DataSchema");

                //if (ds != null && ds.Tables.Count > 1)
                //{
                //    DataTable data = ds.Tables["Data"];
                //    DataTable schema = ds.Tables["DataSchema"];
                //    data.Merge(schema);
                //    ds.Tables.Remove(schema);
                //}
            }
            catch(Exception ex)
            {
                //
                ds = null;
            }
            finally
            {
                connection.Close();
            }
            return ds;
        }

        /// <summary>
        /// Execute Procedure
        /// </summary>
        /// <param name="sql">Procedure Name</param>
        /// <param name="arrParam">Sql Parameters</param>
        /// <returns>Sql Parameters containing result</returns>
        public SqlParameter[] ExecuteProcedure(IDbConnection _conn, string sql, DbParameter[] arrParam, DbTransaction transaction)
        {
            NpgsqlConnection conn;
            if (_conn != null)
            {
                conn = (NpgsqlConnection)_conn;
            }
            else
            {
                string dbConn = DB.GetConnectionString();
                conn = new NpgsqlConnection(dbConn);
            }

            NpgsqlCommand cmd = new NpgsqlCommand();
            int result;
            int countOut = 0;
            NpgsqlDataReader pgreader = null;
            SqlParameter[] ret = null;
            try
            {
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                if (transaction != null)
                {
                    cmd.Transaction = (NpgsqlTransaction)transaction;
                }

                if (arrParam != null)
                {
                    foreach (DbParameter p in arrParam)
                    {
                        if (p.Direction == ParameterDirection.Output)
                        {
                            countOut++;
                            continue;
                        }
                        cmd.Parameters.Add(p);
                    }
                }

                //Open connection and execute insert query.
                if (conn != null && conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                pgreader = cmd.ExecuteReader();

                ret = new SqlParameter[countOut];
                countOut = 0;

                while (pgreader.Read())
                {
                    if (arrParam != null && arrParam.Length > 0)
                    {
                        for (int i = 0; i < arrParam.Length; i++)
                        {
                            if (arrParam[i].Direction == ParameterDirection.Output)
                            {
                                ret[countOut] = new SqlParameter(arrParam[i].ParameterName, pgreader.GetValue(countOut));
                                countOut++;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (pgreader != null)
                {
                    pgreader.Close();
                }
                if (_conn == null)
                    conn.Close();
                cmd.Parameters.Clear();
                VAdvantage.Logging.VLogger.Get().Severe(e.Message + " [Procedure]" + sql);
            }
            finally
            {
                if (pgreader != null)
                {
                    pgreader.Close();
                }
                if (_conn == null)
                    conn.Close();
                cmd.Parameters.Clear();
            }
            return ret;
        }
    }
}
