using CoreLibrary.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.DataBase
{
    public class DBConn
    {

        static DBConn()
        {
            //SetConnectionString();
        }

        private static string connectionString = null;


        public static void SetConnectionString()
        {
            CreateConnectionString();
            //DB.SetDBTarget(VConnection.Get());
        }
       

        public static void SetOracleConnectionString(string connString)
        {
            VConnection vconn = VConnection.Get();
            vconn.Db_Type = DatabaseType.DB_ORACLE;
            vconn.SetAttributes(connString);
            vconn.GetDatabase().SetConnectionString(connString);
            DB.SetDBTarget(VConnection.Get());
            connectionString = connString;
        }

        public static void SetPostgresConnectionString(string connString)
        {
            VConnection vconn = VConnection.Get();
            vconn.Db_Type = DatabaseType.DB_POSTGRESQL;
            vconn.SetAttributes(connString);
            vconn.GetDatabase().SetConnectionString(connString);
            DB.SetDBTarget(VConnection.Get());
            connectionString = connString;
        }

        public static string CreateConnectionString()
        {

            if (DB.UseMigratedConnection)
            {
                if (DB.MigrationConnection != null && DB.MigrationConnection != "")
                {
                    VConnection vconn = VConnection.Get();
                    vconn.SetAttributes(DB.MigrationConnection);

                    return DB.MigrationConnection;
                }


                return "";// Ini.CreateConnectionString(WindowMigration.MDialog.GetMConnection());
            }
            else
            {
                if (connectionString == null)
                {
                    //connectionString =  System.Configuration.ConfigurationSettings.AppSettings["oracleConnectionString"];
                    VConnection vconn = VConnection.Get();
                    connectionString = System.Configuration.ConfigurationManager.AppSettings["oracleConnectionString"];

                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        vconn.Db_Type = DatabaseType.DB_ORACLE;
                        connectionString = TrimUnicode(connectionString);
                        vconn.SetAttributes(connectionString);
                        vconn.GetDatabase().SetConnectionString(connectionString);
                        DB.SetDBTarget(VConnection.Get());
                        return connectionString;
                    }
                    connectionString = System.Configuration.ConfigurationManager.AppSettings["PostgreSQLConnectionString"];
                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        vconn.Db_Type = DatabaseType.DB_POSTGRESQL;
                        vconn.SetAttributes(connectionString);
                        vconn.GetDatabase().SetConnectionString(connectionString);
                        DB.SetDBTarget(VConnection.Get());
                        return connectionString;
                    }
                    connectionString = System.Configuration.ConfigurationManager.AppSettings["PostgreSQLPlusConnectionString"];
                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        vconn.Db_Type = DatabaseType.DB_POSTGRESQL;
                        vconn.SetAttributes(connectionString);
                        vconn.GetDatabase().SetConnectionString(connectionString);
                        DB.SetDBTarget(VConnection.Get());
                        return connectionString;
                    }
                    connectionString = System.Configuration.ConfigurationManager.AppSettings["MSSQLConnectionString"];
                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        vconn.Db_Type = DatabaseType.DB_MSSQL;
                        vconn.SetAttributes(connectionString);
                        vconn.GetDatabase().SetConnectionString(connectionString);
                        DB.SetDBTarget(VConnection.Get());
                        return connectionString;
                    }
                }

                return connectionString;

                //VConnection vconn = VConnection.Get();
                // return Ini.CreateConnectionString(vconn);
            }

            //s_conn.Add(1, constr);


            //}
            //return constr;   //return the connection string to the user
        }

        public static string CreateConnectionString(string host_name, string port_number, string user_id, string password, string database, string db_to_match)
        {
            string connection_string = "";
            if (db_to_match.Equals(VEnvironment.DBTYPE_PG))
            {
                connection_string = "Server=" + host_name + ";Port=" + port_number + ";User Id=" + user_id + ";Password=" + password + ";Database=" + database + ";";
                SetPostgresConnectionString(connection_string);

            }
            else if (db_to_match.Equals(VEnvironment.DBTYPE_ORACLE))
            {
                connection_string = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + host_name + ")(PORT=" + port_number + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + database + ")));User Id=" + user_id + ";Password=" + password + ";";
                SetOracleConnectionString(connection_string);
            }
            else if (db_to_match.Equals(VEnvironment.DBTYPE_MS))
            {
                connection_string = "Server=" + host_name + "," + port_number + ";uid=" + user_id + ";Password=" + password + ";Database=" + database;
            }
            else if (db_to_match.Equals(VEnvironment.DBTYPE_DB2))
            {
                connection_string = "Server=" + host_name + ";Port=" + port_number + ";User Id=" + user_id + ";Password=" + password + ";Database=" + database;
            }

            if (connectionString == null)
            {
                connectionString = System.Configuration.ConfigurationManager.AppSettings["oracleConnectionString"].ToString();
                if(connection_string != null)
                SetOracleConnectionString(connection_string);
            }
            if (connectionString == null)
            {
                connectionString = System.Configuration.ConfigurationManager.AppSettings["PostgreSQLConnectionString"].ToString();
                if (connection_string != null)
                SetPostgresConnectionString(connection_string);
            }
            return connection_string;   //return the connection string to the caller
        }

        private static string TrimUnicode(string connectionString)
        {
            int uIndex = connectionString.ToLower().IndexOf("unicode");
            if (uIndex > -1)//found
            {
                connectionString = connectionString.Substring(0, uIndex) + connectionString.Substring(connectionString.ToLower().IndexOf("true") + 5);
            }
            return connectionString;
        }
    }
}
