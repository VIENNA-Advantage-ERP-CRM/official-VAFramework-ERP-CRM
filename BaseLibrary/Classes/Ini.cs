using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using VAdvantage.Classes;

using VAdvantage.Utility;



using System.IO;
using VAdvantage.Logging;
//using VAdvantage.Install;


namespace VAdvantage.DataBase
{
    public static class Ini
    {
        /** System environment prefix                                       */
        public static String ENV_PREFIX = "env.";

        public static String VIENNA_HOME = "VIENNA_HOME";
        // XML file name				*/
        public static String VIENNA_PROPERTY_FILE = "vienna.properties";
        public static String P_IMPORT_BATCH_SIZE = "ImportBatchSize";

        // XML file name				*/
        public static String VIENNA_ENV_FILE = "viennaEnv.properties";

        /** Apps User ID		*/
        public static String P_UID = "ApplicationUserID";
        private static String DEFAULT_UID = "GardenAdmin";
        /** Apps Password		*/
        public static String P_PWD = "ApplicationPassword";
        private static String DEFAULT_PWD = "GardenAdmin";
        /** Store Password		*/
        public static String P_STORE_PWD = "StorePassword";
        private static bool DEFAULT_STORE_PWD = true;
        /** Trace Level			*/
        public static String P_TRACELEVEL = "TraceLevel";
        private static String DEFAULT_TRACELEVEL = "WARNING";
        /** Trace to File		*/
        public static String P_TRACEFILE = "TraceFile";
        private static bool DEFAULT_TRACEFILE = false;
        /** Language			*/
        public static String P_LANGUAGE = "Language";
        private static String DEFAULT_LANGUAGE = VAdvantage.Login.Language.GetName("en-US");
        /** Ini File Name		*/
        public static String P_INI = "FileNameINI";
        private static String DEFAULT_INI = "";
        /** Connection Details	*/
        public static String P_CONNECTION = "Connection";
        private static String DEFAULT_CONNECTION = "";

        /** Connection Details	*/
        public static String P_DIRECTCONNECTION = "DirectConnection";
        private static String DEFAULT_DIRECTCONNECTION = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=202.164.37.2)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=xe)));User Id=vadvantage;Password=vsysadvant;";

        /** Auto Login			*/
        public static String P_A_LOGIN = "AutoLogin";
        private static bool DEFAULT_A_LOGIN = false;
        /** Auto New Record		*/
        public static String P_A_NEW = "AutoNew";
        private static bool DEFAULT_A_NEW = true;
        /** Dictonary Maintennace	*/
        public static String P_VIENNASYS = "ViennaSys";	//	Save system records
        private static bool DEFAULT_VIENNASYS = false;
        /** Cache Windows			*/
        public static String P_CACHE_WINDOW = "CacheWindow";
        private static bool DEFAULT_CACHE_WINDOW = false;
        /** Temp Directory			*/
        public static String P_TEMP_DIR = "TempDir";
        private static String DEFAULT_TEMP_DIR = "";
        /** Role					*/
        public static String P_ROLE = "Role";
        private static String DEFAULT_ROLE = "";
        /** Client Name				*/
        public static String P_CLIENT = "Client";
        private static String DEFAULT_CLIENT = "";
        /** Org Name				*/
        public static String P_ORG = "Organization";
        private static String DEFAULT_ORG = "";
        /** Warehouse Name			*/
        public static String P_WAREHOUSE = "Warehouse";
        private static String DEFAULT_WAREHOUSE = "";
        /** Current Date			*/
        public static String P_TODAY = "CDate264";
        private static DateTime? DEFAULT_TODAY = DateTime.Now;
        /** Print Preview			*/
        public static String P_PRINTPREVIEW = "PrintPreview";
        private static bool DEFAULT_PRINTPREVIEW = true;

        private static String P_WARNING = "Warning";
        private static String DEFAULT_WARNING = "Do_not_change_any_of_the_data_as_they_will_have_undocumented_side_effects.";

        private static string P_PRINTER = "Printer";
        private static string DEFAULT_PRINTER = "";

        //database setting
        public static String P_DBUSED = "dbused";
        private static string DEFAULT_DBUSED = "";

        public static String P_DB_NAME = "dbname";
        private static string DEFAULT_DB_NAME = "";

        public static String P_DBUSER_ID = "dbuserid";
        private static String DEFAULT_DBUSER_ID = "";

        public static String P_DBPWD = "dbpwd";
        private static String DEFAULT_DB_PWD = "";

        public static String P_DB_PORT = "dbport";
        private static String DEFAULT_DB_PORT = "";

        public static String P_DB_HOST = "dbhost";
        private static String DEFAULT_DB_HOST = "";
        /** Warehouse Name			*/

        /** Hide Client-Org 		* ** Harwinder */
        public static String P_Show_ClientOrg = "ShowClientORG";
        private static bool DEFAULT_ClientOrg_Status = true;

        public static String P_Show_Mini_Grid = "ShowMiniGrid";
        private static bool DEFAULT_MiniGrid_Status = false;

        public static String P_APP_TYPE = "AppType";
        private static String DEFAULT_APP_TYPE = "VServer";

        public static String P_APP_HOST = "AppHost";
        private static String DEFAULT_APP_HOST = "localhost";

        public static String P_APP_PORT = "AppPort";
        private static String DEFAULT_APP_PORT = "2090";

        //Dictonary Maintennace	
        public static String _VIENNASYS = "VFramworkSys";	//	Save system records
        /// <summary>
        /// Path of the XML file which contains server configuration
        /// </summary>
        private const string _XML_DOC_PATH = @"appconn.xml";  //path of the xml file which contains connection string
        /// <summary>
        /// Gets the XML Dcoument Path
        /// </summary>
        public static string XML_DOC_PATH
        {
            get { return _XML_DOC_PATH; }
        }

        /// <summary>
        /// Root node of the xml file which contains server configuration
        /// </summary>
        private const string _XML_ROOT = "//connectionstring";   //root node of connection string xml file
        /// <summary>
        /// Gets the Root node of XML Document
        /// </summary>
        public static string XML_ROOT
        {
            get { return _XML_ROOT; }
        }


        /**
         *  Set Client Mode
         *  @param client client
         */
        public static void SetClient(bool client)
        {
            _client = client;
        }   //  setClient


        /** Ini Properties		*/
        private static String[] PROPERTIES = {
		P_UID, P_PWD, P_TRACELEVEL, P_TRACEFILE, 
		P_LANGUAGE, P_INI,
		P_CONNECTION, P_DIRECTCONNECTION, P_STORE_PWD,
		P_A_LOGIN, P_A_NEW, 
		P_VIENNASYS, P_CACHE_WINDOW,
		P_TEMP_DIR,
		P_ROLE, P_CLIENT, P_ORG, P_WAREHOUSE, P_TODAY,
		P_PRINTER, P_PRINTPREVIEW, P_WARNING, P_DBUSED, P_DBUSER_ID , P_DBPWD, P_DB_PORT, P_DB_HOST, P_DB_NAME,
        P_Show_ClientOrg,P_Show_Mini_Grid,
        P_APP_TYPE, P_APP_HOST, P_APP_PORT

	};

        /** Ini Property Values	*/
        private static String[] VALUES = {
		DEFAULT_UID, DEFAULT_PWD, DEFAULT_TRACELEVEL, DEFAULT_TRACEFILE?"Y":"N",
		DEFAULT_LANGUAGE, DEFAULT_INI,
		DEFAULT_CONNECTION, DEFAULT_DIRECTCONNECTION, DEFAULT_STORE_PWD?"Y":"N",
		DEFAULT_A_LOGIN?"Y":"N", DEFAULT_A_NEW?"Y":"N",
		DEFAULT_VIENNASYS?"Y":"N", DEFAULT_CACHE_WINDOW?"Y":"N",
		DEFAULT_TEMP_DIR,
		DEFAULT_ROLE, DEFAULT_CLIENT, DEFAULT_ORG, DEFAULT_WAREHOUSE, DEFAULT_TODAY.ToString(),
		DEFAULT_PRINTER, DEFAULT_PRINTPREVIEW?"Y":"N", DEFAULT_WARNING, DEFAULT_DBUSED, DEFAULT_DBUSER_ID, DEFAULT_DB_PWD,
        DEFAULT_DB_PORT, DEFAULT_DB_HOST, DEFAULT_DB_NAME,DEFAULT_ClientOrg_Status?"Y":"N",DEFAULT_MiniGrid_Status?"Y":"N",
        DEFAULT_APP_TYPE, DEFAULT_APP_HOST, DEFAULT_APP_PORT
	};

        //Property
        public static VAdvantage.Utility.Properties s_prop = new VAdvantage.Utility.Properties();

        public static String GetFrameworkHome()
        {
            //String env = Environment.GetEnvironmentVariable(ENV_PREFIX + VIENNA_HOME);
            //if (string.IsNullOrEmpty(env))
            //    env = Environment.GetEnvironmentVariable(VIENNA_HOME);
            //if (string.IsNullOrEmpty(env))	//	Fallback
            //    env = Path.DirectorySeparatorChar + "framework";
            return "c:\\VIENNA_HOME";
        }   //  getViennaHome

        private static string xmlFileName = "";
        public static String GetFileName(bool tryUserHome)
        {
            String baseName = null;
            if (tryUserHome && _client)
                baseName = Environment.GetEnvironmentVariable("USERPROFILE");

            //  Server
            if (!_client || string.IsNullOrEmpty(baseName))
            {
                String home = GetFrameworkHome();
                if (!string.IsNullOrEmpty(home))
                    baseName = home;
            }

            if (baseName != null && !baseName.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
                baseName += System.IO.Path.DirectorySeparatorChar.ToString();
            if (baseName == null)
                baseName = "";
            //
            xmlFileName = baseName + VIENNA_PROPERTY_FILE;
            return baseName + VIENNA_PROPERTY_FILE;
        }	//	getFileName


        /// <summary>
        /// Are the properties loaded?
        /// </summary>
        /// <returns>true if properties loaded.</returns>
        public static bool IsLoaded()
        {
            return _loaded;
        }   //  isLoaded


        static private CCache<int, string> s_conn = new CCache<int, string>("Connection", 3);

        /// <summary>
        /// Creates connection string by picking values from XML file.
        /// </summary>
        /// <returns>The connection string for creating connection to the database</returns>



        public static string connectionString = null;// System.Configuration.ConfigurationSettings.AppSettings["oracleConnectionString"];


        public static string CreateConnectionString(VConnection vconn)
        {
            return CreateConnectionString(vconn.Db_host, vconn.Db_port.ToString(), vconn.Db_uid, vconn.Db_pwd, vconn.Db_name, vconn.Db_Type);
        }


        // /// <summary>
        ///// Static connection setting on remote side when get diffrent database for diffrent client
        ///// </summary>
        ///// <param name="host_name"></param>
        ///// <param name="port_number"></param>
        ///// <param name="user_id"></param>
        ///// <param name="password"></param>
        ///// <param name="database"></param>
        ///// <returns>connect String</returns>
        ///// <createby>Raghu</createby>
        ///// <date>17-Jan-2012</date>
        //public static string CreateConnectionString(string host_name, string port_number, string user_id, string password, string database)
        //{


        //    connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + host_name + ")(PORT=" + port_number + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + database + ")));User Id=" + user_id + ";Password=" + password + ";";
        //    VConnection vconn = VConnection.Get();
        //    vconn.SetAttributes(connectionString);
        //    return connectionString;
        //}


        /// <summary>
        /// Creates connection string from values passed in by the user
        /// </summary>
        /// <param name="host_name">Name of the Host or the IP Address</param>
        /// <param name="port_number">Port number of the database server</param>
        /// <param name="user_id">User ID of the database Server</param>
        /// <param name="password">Password of the Database Server</param>
        /// <param name="database">Database Name where the data is stored</param>
        /// <param name="db_to_match"></param>
        /// <returns>Name of the selected database for which connection string is to be created</returns>
        public static string CreateConnectionString(string host_name, string port_number, string user_id, string password, string database, string db_to_match)
        {
            string connection_string = "";
            //if (db_to_match.Equals(VEnvironment.DBTYPE_PG))
            //{
            //    connection_string = "Server=" + host_name + ";Port=" + port_number + ";User Id=" + user_id + ";Password=" + password + ";Database=" + database + ";";
            //}
            //else if (db_to_match.Equals(VEnvironment.DBTYPE_ORACLE))
            //{
            //    connection_string = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + host_name + ")(PORT=" + port_number + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + database + ")));User Id=" + user_id + ";Password=" + password + ";";
            //}
            //else if (db_to_match.Equals(VEnvironment.DBTYPE_MS))
            //{
            //    connection_string = "Server=" + host_name + "," + port_number + ";uid=" + user_id + ";Password=" + password + ";Database=" + database;
            //}
            //else if (db_to_match.Equals(VEnvironment.DBTYPE_DB2))
            //{
            //    connection_string = "Server=" + host_name + ";Port=" + port_number + ";User Id=" + user_id + ";Password=" + password + ";Database=" + database;
            //}

            if (connectionString == null)
            {
                //connectionString = System.Configuration.ConfigurationSettings.AppSettings["oracleConnectionString"].ToString();
                connectionString = System.Configuration.ConfigurationManager.AppSettings["oracleConnectionString"].ToString();


                VConnection vconn = VConnection.Get();
                vconn.SetAttributes(connectionString);

            }

            connection_string = connectionString;

            if (!string.IsNullOrEmpty(s_conn[1]))
                s_conn[1] = connection_string;
            else
                s_conn.Add(1, connection_string);

            return connection_string;   //return the connection string to the caller
        }

        [Obsolete]
        public static string CreateConnectionString()
        {
            string connection_string = "";
            //if (db_to_match.Equals(VEnvironment.DBTYPE_PG))
            //{
            //    connection_string = "Server=" + host_name + ";Port=" + port_number + ";User Id=" + user_id + ";Password=" + password + ";Database=" + database + ";";
            //}
            //else if (db_to_match.Equals(VEnvironment.DBTYPE_ORACLE))
            //{
            //    connection_string = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + host_name + ")(PORT=" + port_number + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + database + ")));User Id=" + user_id + ";Password=" + password + ";";
            //}
            //else if (db_to_match.Equals(VEnvironment.DBTYPE_MS))
            //{
            //    connection_string = "Server=" + host_name + "," + port_number + ";uid=" + user_id + ";Password=" + password + ";Database=" + database;
            //}
            //else if (db_to_match.Equals(VEnvironment.DBTYPE_DB2))
            //{
            //    connection_string = "Server=" + host_name + ";Port=" + port_number + ";User Id=" + user_id + ";Password=" + password + ";Database=" + database;
            //}

            if (connectionString == null)
            {
                //connectionString = System.Configuration.ConfigurationSettings.AppSettings["oracleConnectionString"].ToString();
                connectionString = System.Configuration.ConfigurationManager.AppSettings["oracleConnectionString"].ToString();


                VConnection vconn = VConnection.Get();
                vconn.SetAttributes(connectionString);

            }


            return connectionString;   //return the connection string to the caller
        }


        /// <summary>
        /// Converts the passed in SQL Query to the specific Database format
        /// </summary>
        /// <param name="sql">SQL Query which is to be converted</param>
        /// <returns>Converted SQL Query into other databse format</returns>
        //public static string ConvertSqlQuery(string sql)
        //{
        //    string final_query = sql;    //value which is to be returned back to the caller
        //    //string new_sql = Ini.RemoveInvalidSpaces(sql);    //use only if needed to avoid unnecessary calling overhead
        //    // final_query = new_sql.ToLower();
        //    final_query = sql;
        //    if (DatabaseType.IsPostgre)
        //    {
        //        string strDB = "vienna";
        //        if (!DataBase.DB.UseMigratedConnection)
        //        {
        //            strDB = VConnection.Get().Db_name;
        //        }
        //        else
        //        {
        //            strDB = "ss";// WindowMigration.MDialog.GetMConnection().Db_name;
        //        }
        //        string strSearchPath = "set search_path to " + strDB + ", public;";
        //        final_query = DB_PostgreSQL.ConvertStatement(final_query);
        //        final_query = final_query.Insert(0, strSearchPath);
        //    }
        //    GlobalVariable.LAST_EXECUTED_QUERY = final_query.ToString().Trim();
        //    return final_query.ToString(); //return the converted sql query
        //}



        /// <summary>
        /// Removes invalid spaces from the query
        /// </summary>
        /// <param name="sql">SQL query from where white spaces is to be removed</param>
        /// <returns>WhiteSpace less query returned back to the caller</returns>
        //public static string RemoveInvalidSpaces(string sql)
        //{
        //    string[] parse_sql = sql.Split(' ');    //split the query to avoid unnecesary white spaces
        //    StringBuilder final_query = new StringBuilder("");
        //    //loop through all the split words
        //    for (int i = 0; i <= parse_sql.Length - 1; i++)
        //    {
        //        //if words have no values ignore them
        //        if (!String.IsNullOrEmpty(parse_sql[i].Trim()))
        //        {
        //            if (parse_sql[i] == "(")
        //                final_query.Append(parse_sql[i].Trim()); //append the query without space
        //            else
        //                if (i < parse_sql.Length - 1)
        //                {
        //                    if ((parse_sql[i + 1] == ")") && i < parse_sql.Length - 1)
        //                        final_query.Append(parse_sql[i].Trim()); //append the query without space
        //                    else
        //                        final_query.Append(parse_sql[i].Trim() + " "); //append the query one by one
        //                }
        //                else if (i > 0)
        //                {
        //                    if ((parse_sql[i - 1] == ")") && i > 0)
        //                        final_query.Append(parse_sql[i].Trim()); //append the query without space
        //                    else
        //                        final_query.Append(parse_sql[i].Trim() + " "); //append the query one by one
        //                }
        //                else
        //                    final_query.Append(parse_sql[i].Trim()); //append the query one by one
        //        }
        //    }
        //    return final_query.ToString();
        //}

        /// <summary>
        ///Get next number for Key column = 0 is Error.
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="TableName">Table Name</param>
        /// <param name="trxName">optionl transaction name</param>
        /// <returns> Next Number</returns>
        //public static int GetNextID(Context ctx, String tableName, Trx trxName)
        //{
        //    if (ctx == null)
        //        throw new ArgumentException("Context missing");
        //    if (tableName == null || tableName.Length == 0)
        //        throw new ArgumentException("TableName missing");
        //    return GetNextID(ctx.GetAD_Client_ID(), tableName, trxName);
        //}	//	getNextID

        ///// <summary>
        /////Get next number for Key column = 0 is Error.
        ///// </summary>
        ///// <param name="ctx">Context</param>
        ///// <param name="TableName">Table Name</param>
        ///// <param name="trxName">optionl transaction name</param>
        ///// <returns> Next Number</returns>
        //public static int GetNextID(Ctx ctx, String tableName, Trx trxName)
        //{
        //    if (ctx == null)
        //        throw new ArgumentException("Context missing");
        //    if (tableName == null || tableName.Length == 0)
        //        throw new ArgumentException("TableName missing");
        //    return GetNextID(ctx.GetAD_Client_ID(), tableName, trxName);
        //}	//	getNextID

        ///// <summary>
        /////Get next number for Key column = 0 is Error.
        ///// </summary>
        ///// <param name="AD_Client_ID">client</param>
        ///// <param name="tableName">table name</param>
        ///// <param name="trxName">optional Transaction Name</param>
        ///// <returns>Next no</returns>
        //public static int GetNextID(int AD_Client_ID, string tableName, Trx trxName)
        //{
        //    int no = 0;
        //    try
        //    {
        //        if (DatabaseType.IsOracle)
        //            no = MSequence.GetNextIDOracle(AD_Client_ID, tableName, trxName);
        //        else if (DatabaseType.IsPostgre)
        //            no = MSequence.GetNextIDPostgre(AD_Client_ID, tableName, trxName);
        //        else if (DatabaseType.IsMySql)
        //            no = MSequence.GetNextIDMySql(AD_Client_ID, tableName);
        //        else if (DatabaseType.IsMSSql)
        //            no = MSequence.GetNextIDMSSql(AD_Client_ID, tableName);
        //    }
        //    catch
        //    {
        //    }
        //    return no;
        //}

        /*************************************************************************/



        /** IsClient Internal marker            */
        private static bool _client = true;
        /** IsClient Internal marker            */
        private static bool _loaded = false;

        /**
         *  Are we in Client Mode ?
         *  @return true if client
         */


        public static bool IsClient()
        {
            return _client;
        }   //  isClient


        //private int _postCount = 0;

        //public String PostImmediate(Context ctx,
        //    int AD_Client_ID, int AD_Table_ID, int Record_ID, bool force, Trx trxName)
        //{
        //    //log.info("[" + m_no + "] Table=" + AD_Table_ID + ", Record=" + Record_ID);
        //    _postCount++;
        //    MAcctSchema[] ass = MAcctSchema.GetClientAcctSchema(ctx, AD_Client_ID);
        //    return Doc.postImmediate(ass, AD_Table_ID, Record_ID, force, trxName);
        //}	//	postImmediate

        /**
        *	Get Propery as bool
        *  @param key  Key
        *  @return     Value
        */
        public static bool IsPropertyBool(String key)
        {
            return GetProperty(key).Equals("Y");
        }

        //public static IDbConnection GetConnection()
        //{
        //    VConnection vconn = null;
        //    if (DataBase.DB.UseMigratedConnection)
        //    {
        //        vconn = VConnection.Get(); // WindowMigration.MDialog.GetMConnection();
        //    }
        //    else
        //    {
        //        vconn = VConnection.Get();
        //    }

        //    return GetConnection(vconn);
        //}
        //#pragma warning disable 612, 618
        //public static IDbConnection GetConnection(VConnection vconn)
        //{
        //    if (vconn.IsOracle())
        //        return new OracleConnection(Ini.CreateConnectionString());
        //    else if (vconn.IsPostgreSQL())
        //        return new NpgsqlConnection(Ini.CreateConnectionString());
        //    else if (vconn.IsMSSQLServer())
        //        return new SqlConnection(Ini.CreateConnectionString());
        //    else if (vconn.IsMysql())
        //        return new MySqlConnection(Ini.CreateConnectionString());

        //    return null;
        //}
        //#pragma warning restore 612, 618

        public static bool IsCacheWindow()
        {
            return GetProperty(P_CACHE_WINDOW).Equals("Y");
        }	//	isCacheWindow

        public static bool IsShowClientOrg()
        {
            return !GetProperty(P_Show_ClientOrg).Equals("N");
        }

        public static bool IsShowMiniGrid()
        {
            return GetProperty(P_Show_Mini_Grid).Equals("Y");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static String FindHome()
        {
            String ch = GetFrameworkHome();
            if (!string.IsNullOrEmpty(ch))
                return ch;

            return ch;
            //return Environment.CurrentDirectory;
        }


        private static VLogger log = VLogger.GetVLogger(typeof(Ini).FullName);

        public static bool StartUp(bool isClient, bool isExternal)
        {
            bool OK = true;
            if (log != null)
                return true;

            VLogMgt.Initialize(isClient);
            Ini.SetClient(isClient);

            //log = VLogger.GetVLogger(typeof(Framework.Program).FullName);
            //if (VAdvantage.DataBase.Ini.LoadProperties(isClient))
            OK = true;

            VLogMgt.SetLevel(Ini.GetProperty(Ini.P_TRACELEVEL));

            if (isClient && Ini.IsPropertyBool(Ini.P_TRACEFILE)
                && VLogFile.Get(false, null, isClient) == null)
            {
                if (!isExternal)
                    VLogMgt.AddHandler(VLogFile.Get(true, Ini.FindHome(), isClient));
                else
                    VLogMgt.AddHandler(VLogFile.Get(true, Environment.GetEnvironmentVariable("USERPROFILE"), isClient));

            }

            return OK;
        }

        #region "Updated By jagmohan Bhatt"
        //purpose: change the xml file with the real property file which
        //is easy to access and faster than xml loader

        public static void LoadProperties(bool reload)
        {
            if (reload || s_prop.Count() == 0)
                LoadProperties(GetFileName(_client));
        }

        /// <summary>
        /// Load INI parameters from filename.
        /// Logger is on default level (INFO)
        /// </summary>
        /// <param name="filename">filename to load</param>
        /// <returns>true if first time</returns>
        public static bool LoadProperties(String filename)
        {
            bool loadOK = true;
            bool firstTime = false;



            FileStream fis = null;

            try
            {
                fis = new FileStream(filename, FileMode.Open, FileAccess.Read);
                s_prop.Load(fis);
                fis.Close();
            }
            catch (FileNotFoundException e)
            {
                log.Warning(filename + " not found <=>" + e.Message);
                loadOK = false;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, filename + " - " + e.ToString());
                loadOK = false;
            }


            if (!loadOK || s_prop.GetProperty(P_TODAY, "").Equals(""))
            {
                log.Config(filename);
                firstTime = true;

                //if (!VAdvantage.Framework.IniDialog.Accept())
                //    Environment.Exit(-1);

            }

            //	Check/set properties	defaults
            for (int i = 0; i < PROPERTIES.Length; i++)
            {
                if (VALUES[i].Length > 0)
                    CheckProperty(PROPERTIES[i], VALUES[i]);
            }

            //
            String tempDir = Environment.GetEnvironmentVariable("TEMP");
            if (tempDir == null || tempDir.Length == 1)
                tempDir = GetFrameworkHome();
            if (tempDir == null)
                tempDir = "";
            CheckProperty(P_TEMP_DIR, tempDir);

            //  Save if not exist or could not be read
            if (!loadOK || firstTime)
            {
                SaveProperties(true);
            }
            _loaded = true;

            return firstTime;
            //if (true)//EvaluationCheck.DemoCheck())
            //{
            //    return firstTime;
            //}
            //else
            //{
            // //   Environment.Exit(0);
            //}

            ///return false;
        }	//	loadProperties


        /// <summary>
        /// Save INI parameters to disk
        /// </summary>
        /// <param name="tryUserHome">get user home first</param>
        public static void SaveProperties(bool tryUserHome)
        {
            String fileName = GetFileName(_client);
            /**	File writer				*/
            StreamWriter _writer = null;
            //FileOutputStream fos = null;
            try
            {
                FileStream file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                _writer = new StreamWriter(file, Encoding.UTF8);
                s_prop.Store(_writer, "ViennaFramework (c) 2009-2011");
                _writer.Flush();
                _writer.Close();
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "Cannot save Properties to " + fileName + " - " + e.Message);
                return;
            }
            log.Finer(fileName);
        }	//	SaveProperties

        /// <summary>
        /// Set Property
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public static void SetProperty(string key, string value)
        {
            if (s_prop == null)
                s_prop = new VAdvantage.Utility.Properties();
            if (key.Equals(P_WARNING))
                s_prop.SetProperty(key, value);
            else if (!IsClient())
                s_prop.SetProperty(key, SecureEngineUtility.Secure.CLEARVALUE_START + value + SecureEngineUtility.Secure.CLEARVALUE_END);
            else
            {
                if (value == null)
                    s_prop.SetProperty(key, "");
                else
                {
                    String eValue = SecureEngine.Encrypt(value);
                    if (eValue == null)
                        s_prop.SetProperty(key, "");
                    else
                        s_prop.SetProperty(key, eValue);
                }
            }
        }

        /// Fetches the Node value from xml file of a specific node
        /// </summary>
        /// <param name="value">Name of the node whose values is to be fetched</param>
        /// <returns>Value of the node</returns>
        public static string GetProperty(string key)
        {
            if (key == null)
                return "";
            String retStr = s_prop.GetProperty(key, "");
            if (retStr == null || retStr.Length == 0)
                return "";
            //
            String value = "";
            if (retStr.Substring(0, 3) == "xyz")
                value = retStr.Substring(3);
            else
                value = SecureEngine.Decrypt(retStr);
            //	log.finer(key + "=" + value);
            if (value == null)
                return "";
            return value;
        }

        /// <summary>
        /// Get Properties
        /// </summary>
        /// <returns></returns>
        public static VAdvantage.Utility.Properties GetProperties()
        {
            return s_prop;
        }   //  getProperties


        /// <summary>
        /// Set Property
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public static void SetProperty(String key, bool value)
        {
            SetProperty(key, value ? "Y" : "N");
        }   //  setProperty

        /// <summary>
        /// Set Property
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public static void SetProperty(String key, int value)
        {
            SetProperty(key, value.ToString());
        }   //  setProperty


        /// <summary>
        /// Load property and set to default, if not existing
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Property</returns>
        private static String CheckProperty(String key, String defaultValue)
        {
            String result = null;
            if (key.Equals(P_WARNING))
                result = defaultValue;
            else if (!IsClient())
                result = s_prop.GetProperty(key, SecureEngineUtility.Secure.CLEARVALUE_START + defaultValue + SecureEngineUtility.Secure.CLEARVALUE_END);
            else
                result = s_prop.GetProperty(key, SecureEngine.Encrypt(defaultValue));
            s_prop.SetProperty(key, result);
            return result;
        }	//	checkProperty

        #endregion

        public static void SetViennaHome(String value)
        {
            if (!string.IsNullOrEmpty(value))
                Environment.SetEnvironmentVariable(VIENNA_HOME, value);
        }   //  setViennaHome




    }
}
