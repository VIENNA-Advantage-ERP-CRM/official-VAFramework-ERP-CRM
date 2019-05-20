/********************************************************
 * Module/Class Name    : Database Metadata
 * Purpose              : Fetches the database table and columns
 * Chronological Development
 * Jagmohan Bhatt   3-Sep-2009
 ******************************************************/
using System;
using System.Data;
using System.Data.OleDb;
using VAdvantage.DataBase;
using System.Collections.Generic;
using System.Collections;
using System.Data.Common;
using Npgsql;

namespace VAdvantage.Utility
{
/// <summary>
/// Summary description for DatabaseMetaData.
/// </summary>
    public class DatabaseMetaData : IDisposable
    {
        /// <summary>
        /// The Connection Object
        /// </summary>
        //OleDbConnection connection_1;
        DbConnection connection_;

        // Field descriptor #8 I
        public static int importedKeyCascade = 0;

        // Field descriptor #8 I
        public static int importedKeyRestrict = 1;

        // Field descriptor #8 I
        public static int importedKeySetNull = 2;

        public static int importedKeyNoAction = 3;

        /// <summary>
        /// Default No-Argument constructor that
        /// Creates a connection to the database
        /// </summary>
        public DatabaseMetaData()
        {

            //DbProviderFactory dbProviderFactory = null;
            //string CONNECTION_STRING = DB.GetConnectionString();

            //if (DatabaseType.IsOracle)
            //{
            //    dbProviderFactory = DbProviderFactories.GetFactory("System.Data.OracleClient");
            //}
            //else if (DatabaseType.IsPostgre)
            //{
            //    dbProviderFactory = DbProviderFactories.GetFactory("Npgsql");
            //}
            try
            {
               
                //connection_ = dbProviderFactory.CreateConnection();
               // connection_.ConnectionString = DB.GetConnectionString();
                connection_ =(DbConnection)DB.GetConnection();
                connection_.Open();
                
            }
            catch
            {
            }
        }

        /// <summary>
        /// Closes the Connection to the Database
        /// </summary>
        public void Dispose()
        {
            this.connection_.Close();
        }

        /// <summary>
        /// Retrieves Database Metadata information about Tables
        /// of the specific database exposed to this user
        /// </summary>
        public DataSet GetTables()
        {
            DataSet ds = new DataSet();
            //DataTable tables = this.connection_1.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            //DataTable tables = this.connection_.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            DataTable tables1 = this.connection_.GetSchema("Tables", null);
            ds.Tables.Add(tables1);
            return ds;
        }


                /// <summary>
        /// Retrieves Database Metadata information about Columns
        /// of the specific database exposed to this user
        /// </summary>
        public DataSet GetIndexInfo(string catalog, string schema, string indexName)
        {
            DataSet ds = null;
            try
            {

                ds = new DataSet();
                //DataTable tables1 = this.connection_1.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, schema, "VIENNA_2", null });
                DataTable tables = null;
                if (DatabaseType.IsPostgre)
                    tables = this.connection_.GetSchema("IndexColumns", new string[] { schema, VConnection.Get().Db_searchPath, indexName.ToLower(), null });
                else
                    tables = this.connection_.GetSchema("IndexColumns", new string[] { null, indexName.ToUpper(), schema });

                ds.Tables.Add(tables);
            }
            catch
            {

            }
            return ds;
        }

        /// <summary>
        /// Retrieves Database Metadata information about Columns
        /// of the specific database exposed to this user
        /// </summary>
        public DataSet GetColumns(string catalog, string schema, string tableName)
        {
            DataSet ds = null;
            try
            {

                ds = new DataSet();
                //DataTable tables1 = this.connection_1.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, schema, "VIENNA_2", null });
                DataTable tables = null;
                if (DatabaseType.IsPostgre)
                    tables = this.connection_.GetSchema("Columns", new string[] {schema, VConnection.Get().Db_searchPath,tableName.ToLower(), null });
                else if(DatabaseType.IsOracle)
                    tables = this.connection_.GetSchema("Columns", new string[] { schema, tableName.ToUpper() });

                ds.Tables.Add(tables);
            }
            catch
            {

            }
            return ds;
        }

        /// <summary>
        /// Retrieves Database Metadata information about Foreign Keys
        /// of the specific database exposed to this user
        /// </summary>
        public DataSet GetForeignKeys(string catalog, string schema, string tableName)
        {
            DataSet ds = null;
            try
            {

                ds = new DataSet();
                //DataTable tables1 = this.connection_1.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, schema, "VIENNA_2", null });
                DataTable tables = null;
                if (DatabaseType.IsPostgre)
                    tables = this.connection_.GetSchema("ForeignKeys", new string[] { schema, VConnection.Get().Db_searchPath, tableName.ToLower(), null });
                else if (DatabaseType.IsOracle)
                    tables = this.connection_.GetSchema("ForeignKeys", new string[] { schema, tableName.ToUpper() });

                ds.Tables.Add(tables);
            }
            catch
            {

            }
            return ds;
        }


        public  Dictionary<string,string> GetForeignColumnDetail(DataRow dr)
        {
            Dictionary<string, string> retValue = new Dictionary<string, string>();
            
            if(DatabaseType.IsOracle)
            {
                retValue["FK_Column_Name"] = DB.ExecuteScalar("SELECT column_name FROM user_cons_columns WHERE constraint_name='" + dr["FOREIGN_KEY_CONSTRAINT_NAME"].ToString() + "'").ToString();
                retValue["PK_Table_Name"] = dr["PRIMARY_KEY_TABLE_NAME"].ToString();
                retValue["PK_Column_Name"] = DB.ExecuteScalar("SELECT column_name FROM user_cons_columns WHERE constraint_name='" + dr["PRIMARY_KEY_CONSTRAINT_NAME"].ToString() + "'").ToString();
                retValue["Delete_Rule"] = dr["DELETE_RULE"].ToString();
                retValue["ConstraintNameDB"] = dr["FOREIGN_KEY_CONSTRAINT_NAME"].ToString();
            }
            else if(DatabaseType.IsPostgre)
            {
                DataSet ds = DB.ExecuteDataset("SELECT tc.table_name FK_Table_Name,kcu.column_name FK_Column_Name,rc.delete_rule,ccu.table_name PK_Table_Name," +
                                               "ccu.column_name PK_Column_Name FROM information_schema.table_constraints tc LEFT JOIN information_schema.key_column_usage kcu" +
                                               " ON tc.constraint_name = kcu.constraint_name LEFT JOIN information_schema.referential_constraints rc ON tc.constraint_name = rc.constraint_name"+
                                               " LEFT JOIN information_schema.constraint_column_usage ccu ON rc.unique_constraint_name = ccu.constraint_name "+
                                               " WHERE tc.constraint_name = '"+dr["CONSTRAINT_NAME"]+"'");

                retValue["FK_Column_Name"] = ds.Tables[0].Rows[0]["FK_Column_Name"].ToString();
                retValue["PK_Table_Name"] = ds.Tables[0].Rows[0]["PK_Table_Name"].ToString();
                retValue["PK_Column_Name"] = ds.Tables[0].Rows[0]["PK_Column_Name"].ToString();
                retValue["Delete_Rule"] = ds.Tables[0].Rows[0]["Delete_Rule"].ToString();
                retValue["ConstraintNameDB"] = dr["CONSTRAINT_NAME"].ToString();
            }
            return retValue;
        }


        internal int GetConstraintTypeDB(string type)
        {
            type = type.ToUpper();
            if (type == "CASCADE")
            {
                return importedKeyCascade;
            }
            else if (type == "RESTRICT")
            {
                return importedKeyRestrict;
            }
            else if (type == "SET NULL")
            {
                return importedKeySetNull;
            }
            return importedKeyNoAction;
        }
    }

    public class Types
    {

        public const string DATE = "DATE";
        public const string TIME = "TIME";
        public const string TIMESTAMP = "TIMESTAMP";
        public const string NUMBER = "NUMBER";
        public const string INTEGER = "INTEGER";
        public const string NUMERIC = "NUMERIC";

        public const string BLOB = "BLOB";
        public const string CLOB = "CLOB";

        public const string CHAR = "CHAR";
        public const string VARCHAR = "NVARCHAR"; 
        public const string DECIMAL = "NUMBER";
        public const string SMALLINT = "NUMBER";

    }
}