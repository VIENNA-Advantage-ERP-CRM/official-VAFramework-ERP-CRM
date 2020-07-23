/********************************************************
 * Module\Class Name    : EnvConstants
 * Purpose        : Declare the contanst of System Environment
 * Class Used     : 
 * Chronological Development
 * Veena Pandey     02-May-2009
 ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.Utility
{
    public class EnvConstants
    {
        /** WindowNo for Find           */
        public const int WINDOW_FIND = 1110;
        /** WinowNo for MLookup         */
        public const int WINDOW_MLOOKUP = 1111;
        /** WindowNo for PrintCustomize */
        public const int WINDOW_CUSTOMIZE = 1112;

        /** WindowNo for PrintCustomize */
        public const int WINDOW_INFO = 1113;
        /** Tab for Info                */
        public const int TAB_INFO = 1113;
        /** WindowNo for AccountEditor */
        public const int WINDOW_ACCOUNT = 1114;


        /** AS Host						*/
        public static String VIENNA_APPS_SERVER = "VIENNA_APPS_SERVER";
        /** AS Type						*/
        public static String VIENNA_APPS_TYPE = "VIENNA_APPS_TYPE";

        /**	AS Type JBoss (default)		*/
        public static String APPSTYPE_VSERVER = "vserver";
        /** AS RMI Port					*/
        public static String VIENNA_APP_PORT = "VIENNA_APP_PORT";

        /** DB Host						*/
        public static String VIENNA_DB_SERVER = "VIENNA_DB_SERVER";
        /** DB Type	e.g. oracleXE		*/
        public static String VIENNA_DB_TYPE = "VIENNA_DB_TYPE";
        /** DB Path	e.g. oracle			*/
        public static String VIENNA_DB_PATH = "VIENNA_DB_PATH";
        /** DB Type PostgreSQL			*/
        public static String DBTYPE_PG = "postgreSQL";
        /** DB Type Oracle Std			*/
        public static String DBTYPE_ORACLE = "oracle";
        /** DB Type Oracle XP			*/
        public static String DBTYPE_ORACLEXE = "oracleXE";
        /** DB Type DB/2				*/
        public static String DBTYPE_DB2 = "db2";
        //public static  String		DBTYPE_MS = "<sqlServer>";
        public static String DBTYPE_MS = "sqlServer";
        /** DB Name						*/
        public static String VIENNA_DB_NAME = "VIENNA_DB_NAME";
        /** DB Port						*/
        public static String VIENNA_DB_PORT = "VIENNA_DB_PORT";
        /** DB Vienna UID				*/
        public static String VIENNA_DB_USER = "VIENNA_DB_USER";
        /** DB Vienna User Exists	*/
        public static String VIENNA_DB_USER_EXISTS = "VIENNA_DB_USER_EXISTS";
        /** DB Vienna PWD				*/
        public static String VIENNA_DB_PASSWORD = "VIENNA_DB_PASSWORD";
        /** DB URL						*/
        public static String VIENNA_DB_URL = "VIENNA_DB_URL";
        /** DB System PWD 				*/
        public static String VIENNA_DB_SYSTEM = "VIENNA_DB_SYSTEM";
        /** File Name				*/
        //private static String FILENAME = "Environment.properties";

    }
}
