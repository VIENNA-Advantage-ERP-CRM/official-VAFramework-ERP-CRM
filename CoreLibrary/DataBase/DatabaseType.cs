using CoreLibrary.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Logging;
//using VAdvantage.Install;
//using VAdvantage.WindowMigration;

namespace VAdvantage.DataBase
{
    /// <summary>
    /// Gets the Currently configured database with the current application
    /// </summary>
    public static class DatabaseType
    {

        /**	Logger							*/
	private static VLogger			log = VLogger.GetVLogger (typeof(DatabaseType).FullName);

        /** Oracle ID       */
        public static String DB_ORACLE = "Oracle";
        /** PostgreSQL ID   */
        public static String DB_POSTGRESQL = "PostgreSQL";

        /** PostgreSQL ID   */
        public static String DB_POSTGRESQLPLUS = "PostgreSQL+";

        public static String DB_MYSQL = "MySQL";

        public static String DB_MSSQL = "MSSQL";

        /** Supported Databases     */
        public static String[] DB_NAMES = new String[] {
		 DB_ORACLE
		,DB_POSTGRESQL
        ,DB_POSTGRESQLPLUS
        ,DB_MSSQL
		,DB_MYSQL
	};

        /** Connection Timeout in seconds   */
	public static int           CONNECTION_TIMEOUT = 10;
	
	/// <summary>
	/// Get Database by database Id.
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	public static ViennaDatabase GetDatabase (String type)
	{
		//ViennaDatabase db = null;
        return GetDatabaseFromURL(type);
        //for (int i = 0; i < DatabaseType.DB_NAMES.Length; i++)
        //{
        //    if (DatabaseType.DB_NAMES[i].Equals (type))
        //    {
        //        db = (ViennaDatabase)DatabaseType.DB_CLASSES[i].
        //               newInstance ();
        //        break;
        //    }
        //}
		//return db;
	}
	
	/**
	 *  Get Database Driver by url string.
	 *  Access to database specific functionality.
	 *  @param URL JDBC connection url
	 *  @return Vienna Database Driver
	 */
	public static ViennaDatabase GetDatabaseFromURL(String url)
	{

		if (url == null)
		{
			log.Severe("No Database URL");
			return null;
		}
        url = url.ToLower();
		if (url.IndexOf("oracle") != -1)
			return new DB_Oracle();
        if (url.IndexOf("postgresql") != -1)
			return new DB_PostgreSQL();
		if (url.IndexOf("mysql") != -1)
			return new DB_MySQL();

		log.Severe("No Database for " + url);
		return null;
	}



        #region Used Database Properties
        /// <summary>
        /// To check if Used Databae is MySQL
        /// </summary>
        public static bool IsMySql
        {
            get
            {
                if (!DB.UseMigratedConnection)
                {
                    if (VConnection.Get().IsMysql())
                        return true;
                    return false;

                }
                else
                {
                    //if (false)
                    //{
                    //    return true;
                    //}
                    return false;
                }
            }
        }

        /// <summary>
        /// To check if Used Databae is MSSQL
        /// </summary>
        public static bool IsMSSql
        {
            get
            {
                if (!DB.UseMigratedConnection)
                {
                    if (VConnection.Get().IsMSSQLServer())
                        return true;
                    return false;
                }
                else
                {
                    //if (false)//  MDialog.GetMConnection().IsMSSQLServer())
                    //{
                    //    return true;
                    //}
                    return false;
                }
            }
        }

        /// <summary>
        /// To check if Used Databae is Postgre SQL
        /// </summary>
        public static bool IsPostgre
        {
            get
            {
                if (!DB.UseMigratedConnection)
                {
                    if (VConnection.Get().IsPostgreSQL())
                        return true;
                    return false;

                }
                else
                {
                    //if (false)//  MDialog.GetMConnection().IsPostgreSQL())
                    //{
                    //    return true;
                    //}
                    return false;
                }
            }
        }
        /// <summary>
        /// To check if Used Databae is Postgre SQL
        /// </summary>
        public static bool IsPostgrePlus
        {
            get
            {
                if (!DB.UseMigratedConnection)
                {
                    if (VConnection.Get().IsPostgreSQLPlus())
                        return true;
                    return false;

                }
                else
                {
                    //if (false)//  MDialog.GetMConnection().IsPostgreSQL())
                    //{
                    //    return true;
                    //}
                    return false;
                }
            }
        }

        /// <summary>
        /// To check if Used Databae is Oracle
        /// </summary>
        public static bool IsOracle
        {
            get
            {
                if (!DB.UseMigratedConnection)
                {
                    if (VConnection.Get().IsOracle())
                        return true;
                    return false;

                }
                else
                {

                    // manish 8 May, 2017
                    if (true) //MDialog.GetMConnection().IsOracle())
                    {
                        return true;
                    }
                    //End
                    return false;
                }
            }
        }



        #endregion
    }
}
