using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace VAdvantage.DataBase
{


    /// <summary>
    /// *  Interface for Vienna Databases
    /// </summary>
    public interface  ViennaDatabase
    {
        ///// <summary>
        ///// Get Database Name
        ///// </summary>
        ///// <returns>database short name</returns>
        //public String GetName();

        ///// <summary>
        ///// Get Database Description
        ///// </summary>
        ///// <returns>database long name and version</returns>
        //public String GetDescription();

        string GetName();

        /// <summary>
        /// Get Standard JDBC Port
        /// </summary>
        /// <returns>standard port</returns>
        int GetStandardPort();
        

        ///// <summary>
        /////	Get  Catalog
        ///// </summary>
        ///// <returns></returns>
        //public String GetCatalog();

        ///// <summary>
        /////Get JDBC Schema
        ///// </summary>
        ///// <returns>Schema</returns>
        //public String GetSchema();

        /// <summary>
        /// Supports BLOB
        /// </summary>
        /// <returns>true if BLOB is supported</returns>
         bool SupportsBLOB();

        ///// <summary>
        ///// String Representation
        ///// </summary>
        ///// <returns>info</returns>
        //String ToString();


        /// <summary>
        /// Convert an individual Oracle Style statements to target database statement syntax
        /// </summary>
        /// <param name="oraStatement">oracle statement</param>
        /// <returns>converted Statement</returns>
        String ConvertStatement(String oraStatement);

        ///// <summary>
        ///// Check if DBMS support the sql statement
        ///// </summary>
        ///// <param name="sql">SQL statement</param>
        ///// <returns>true: yes</returns>
        //public bool Isupported(String sql);

        ///// <summary>
        ///// Get constraint type associated with the index
        ///// </summary>
        ///// <param name="conn">connection</param>
        ///// <param name="tableName"> table name</param>
        ///// <param name="IXName">Index name</param>
        ///// <returns>String[0] = 0: do not know, 1: Primary Key  2: Foreign Key
        ////  		String[1] - String[n] = Constraint Name</returns>
        //public String GetConstraintType(System.Data.IDbConnection conn, String tableName, String IXName);


        ///// <summary>
        ///// Check and generate an alternative SQL
        ///// </summary>
        ///// <param name="reExNo">number of re-execution</param>
        ///// <param name="msg">previous execution error message</param>
        ///// <param name="sql">previous executed SQL</param>
        ///// <returns>the alternative SQL, null if no alternative</returns>
        //public String GetAlternativeSQL(int reExNo, String msg, String sql);

        ///// <summary>
        ///// Get Name of System User
        ///// </summary>
        ///// <returns>e.g. sa, system</returns>
        //public String GetSystemUser();

        ///// <summary>
        ///// Get Name of System Database
        ///// </summary>
        ///// <param name="databaseName">database Name</param>
        ///// <returns>master or database Name</returns>
        //public String GetSystemDatabase(String databaseName);

        /// <summary>
        ///  Create SQL TO Date String from Timestamp
        /// </summary>
        /// <param name="time">time Date to be converted</param>
        /// <param name="dayOnly">dayOnly true if time set to 00:00:00</param>
        /// <returns>date function</returns>
        String TO_DATE(DateTime? time, bool dayOnly);

        /// <summary>
        /// Create SQL for formatted Date, Number
        /// </summary>
        /// <param name="columnName">columnName  the column name in the SQL</param>
        /// <param name="displayType">displayType Display Type</param>
        /// <param name="AD_Language">AD_Language 6 character language setting (from Env.LANG_*)</param>
        /// <returns>TRIM(TO_CHAR(columnName,'999G999G999G990D00','NLS_NUMERIC_CHARACTERS='',.'''))
        /// or TRIM(TO_CHAR(columnName,'TM9')) depending on DisplayType and Language</returns>
        String TO_CHAR(String columnName, int displayType, String AD_Language);


        /// <summary>
        ///	Return number as string for INSERT statements with correct precision
        /// </summary>
        /// <param name="number">number</param>
        /// <param name="displayType">display Type</param>
        /// <returns>number as string</returns>
         String TO_NUMBER(Decimal? number, int displayType);


        /// <summary>
        /// Return next sequence this Sequence
        /// </summary>
        /// <param name="Name">Sequence Name</param>
        /// <returns></returns>
        int GetNextID(String Name);

        /// <summary>
        /// Create Native Sequence
        /// </summary>
        /// <param name="name"></param>
        /// <param name="increment"></param>
        /// <param name="minvalue"></param>
        /// <param name="maxvalue"></param>
        /// <param name="start"></param>
        /// <param name="trxName"></param>
        /// <returns></returns>
        bool CreateSequence(String name, int increment, int minvalue, int maxvalue, int start, Trx trxName);

        /// <summary>
        /// Get Database Connection
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="autoCommit"></param>
        /// <param name="transactionIsolation"></param>
        /// <returns></returns>
         System.Data.IDbConnection GetCachedConnection(
        bool autoCommit, int transactionIsolation);

        /// <summary>
        /// Connection String 
        /// </summary>
        void SetConnectionString(string conString);

        DataSet ExecuteDatasetPaging(string sql, int page, int pageSize, int increment);

        SqlParameter[] ExecuteProcedure(IDbConnection conn, string sql, DbParameter[] param, DbTransaction trx);

        ///** Create User commands					*/
        // static int CMD_CREATE_USER = 0;
        ///** Create Database/Schema Commands			*/
        // static int CMD_CREATE_DATABASE = 1;
        ///** Drop Database/Schema Commands			*/
        //static int CMD_DROP_DATABASE = 2;

        /// <summary>
        /// Get SQL Commands.
        ///The following variables are resolved:
        ///	@SystemPassword@, @SystemUser@, @SystemPassword@
        ///	@SystemPassword@, @DatabaseName@, @DatabaseDevice@
        ///  </code>
        /// @param cmdType CMD_*
        /// </summary>
        /// <param name="cmdType"></param>
        ///// <returns>array of commands to be executed</returns>
        // String[] GetCommands(int cmdType);


        ///// <summary>
        ///// Close
        ///// </summary>
        // void Close();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public ConvertSql GetConvert();

        ///**
        // * @return true if support statement timeout
        // */
        //public bool IsQueryTimeoutSupported();

        /// <summary>
        ///Default sql use to test whether a connection is still valid
        /// </summary>
        // static String DEFAULT_CONN_TEST_SQL = "SELECT Version FROM AD_System";

        /// <summary>
        ///// Is the database have sql extension that return a subset of the query result
        ///// </summary>
        ///// <returns></returns>
        //public bool IsPagingSupported();

        ///// <summary>
        ///// modify sql to return a subset of the query result
        ///// </summary>
        ///// <param name="sql"></param>
        ///// <param name="start"></param>
        ///// <param name="end"></param>
        ///// <returns></returns>
        //public String AddPagingSQL(String sql, int start, int end);

    }   //  ViennaDatabase

}