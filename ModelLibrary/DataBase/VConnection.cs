using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Logging;
using VAdvantage.Model;
//using VAdvantage.Install;
using System.Net;

//using VAdvantage.Server;
using System.Runtime.Remoting.Channels;
//using System.Runtime.Remoting.Channels.Tcp;
using System.Data;
//using System.Data.OracleClient;
using Npgsql;
using System.Net.NetworkInformation;
using Oracle.ManagedDataAccess.Client;

namespace VAdvantage.DataBase
{

    [Serializable()]
    public class VConnection
    {
        /** Connection      */
        public static VConnection s_cc = null;
        /** Database            */
        private ViennaDatabase m_db = null;

        /* Application server type profiles */
        public static ValueNamePair[] appsTypeProfiles = new ValueNamePair[]
	    {
            //new ValueNamePair(VEnvironment.APPSTYPE_VSERVER, "VServer"),
            //new ValueNamePair(VEnvironment.APPSTYPE_IIS, "IIS")
	    };

        /** Logger			*/
        private static VLogger log = VLogger.GetVLogger(typeof(VConnection).FullName);

        /** Name of Connection  */
        private String m_name = "Standard";

        public String Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        /** Application Host    */
        private String m_apps_host = "MyAppsServer";

        public String Apps_host
        {
            get { return m_apps_host; }
            set
            {
                m_apps_host = value;
                m_name = ToString();
            }
        }
        /** Application Port    */
        private int m_apps_port = 2090;

        public int Apps_port
        {
            get { return m_apps_port; }
            set { m_apps_port = value; }
        }

        /** Application Type       */
        private String appsType = "aa";// VEnvironment.APPSTYPE_VSERVER;

        public String AppsType
        {
            get { return appsType; }
            set { appsType = value; }
        }


        /** DB User name        */
        private String m_db_uid = "vadvantage";

        public String Db_uid
        {
            get { return m_db_uid; }
            set { m_db_uid = value; }
        }
        /** DB User password    */
        private String m_db_pwd = "vadvantage";

        public String Db_pwd
        {
            get { return m_db_pwd; }
            set { m_db_pwd = value; }
        }

        /** Database Type       */
        private String m_type =    DatabaseType.DB_ORACLE;

        public String Db_Type
        {
            get { return m_type; }
            set { m_type = value; }
        }
        /** Database Host       */
        private String m_db_host = "MyDBServer";

        public String Db_host
        {
            get { return m_db_host; }
            set { m_db_host = value; }
        }

        private string m_db_port = "1521";

        public string Db_port
        {
            get { return m_db_port; }
            set { m_db_port = value; }
        }

        /** Database name       */
        private String m_db_name = "vadvantage";

        public String Db_name
        {
            get { return m_db_name; }
            set { m_db_name = value; }
        }

        public string m_db_searchPath = "public";
        public string Db_searchPath
        {
            get { return m_db_searchPath; }
            set { m_db_searchPath = value; }
        }

        //initialize connection string and other properties*/
        

        /// <summary>
        /// Is Oracle
        /// </summary>
        /// <returns>true if oracle</returns>
        public bool IsOracle()
        {
           return DatabaseType.DB_ORACLE.Equals(m_type);
        } 	//  isOracle

        /// <summary>
        /// Is MySql
        /// </summary>
        /// <returns>true if Mysql</returns>
        public bool IsMysql()
        {
            return DatabaseType.DB_MYSQL.Equals(m_type);
        } 	//  isDB2


        public bool IsMSSQLServer()
        {
            return DatabaseType.DB_MSSQL.Equals(m_type);
        } 	//  isMSSQLServer


        public bool IsPostgreSQL()
        {
            return DatabaseType.DB_POSTGRESQL.Equals(m_type);
        }
        public bool IsPostgreSQLPlus()
        {
            return DatabaseType.DB_POSTGRESQLPLUS.Equals(m_type);
        } 


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(m_apps_host);
            sb.Append("{").Append(m_db_host)
              .Append("-").Append(m_db_name)
              .Append("#").Append(m_db_uid)
              .Append("}");
            return sb.ToString();
        }

        public String ToStringLong()
        {
            StringBuilder sb = new StringBuilder("VConnection[");
            sb.Append("name=").Append(m_name)
                // Added WAS support
              .Append(",AppsType=").Append(appsType)
              .Append(",AppsHost=").Append(m_apps_host)
              .Append(",AppsPort=").Append(m_apps_port)
              .Append(",type=").Append(m_type)
              .Append(",DBhost=").Append(m_db_host)
              .Append(",DBport=").Append(m_db_port)
              .Append(",DBname=").Append(m_db_name)
              .Append(",UID=").Append(m_db_uid)
              .Append(",PWD=").Append(m_db_pwd)
              ;		//	the format is read by setAttributes
            sb.Append("]");
            return sb.ToString();
        }	//  toStringLong

        private string Substring(string var, int startindex, int endindex)
        {
            endindex = (endindex - startindex);
            return var.Substring(startindex, endindex);
        }

        public override bool Equals(Object o)
        {
            if (o is VConnection)
            {
                VConnection cc = (VConnection)o;
                if (cc.Apps_host.Equals(m_apps_host)
                  && (cc.Apps_port == m_apps_port)
                  && cc.Db_host.Equals(m_db_host)
                  && (cc.Db_port == m_db_port)
                  && cc.Db_name.Equals(m_db_name)
                  && cc.Db_Type.Equals(m_type)
                  && cc.Db_uid.Equals(m_db_uid)
                  && cc.Db_pwd.Equals(m_db_pwd))
                    return true;
            }
            return false;
        }	//  equals

        public bool EqualsDatabase(Object o)
        {
            if (o is VConnection)
            {
                VConnection cc = (VConnection)o;
                if ((cc.Apps_port == m_apps_port)
                  && cc.Db_host.Equals(m_db_host)
                  && (cc.Db_port == m_db_port)
                  && cc.Db_name.Equals(m_db_name)
                  && cc.Db_Type.Equals(m_type)
                  && cc.Db_uid.Equals(m_db_uid)
                  && cc.Db_pwd.Equals(m_db_pwd))
                    return true;
            }
            return false;
        }

        public void SetAttributes(String attributes)
        {
            try
            {
                if (IsOracle())
                {

                    //int index = attributes.IndexOf("name=");
                    //Name = Substring(attributes, index + 5, attributes.IndexOf(",", index));
                    //// Added WAS support
                    //index = attributes.IndexOf("AppsType=");
                    //if (index > 0)
                    //    AppsType = Substring(attributes, index + 9, attributes.IndexOf(",", index));
                    ////
                    //Apps_host = Substring(attributes, attributes.IndexOf("AppsHost=") + 9, attributes.IndexOf(",AppsPort="));
                    //index = attributes.IndexOf("AppsPort=");
                    //Apps_port = int.Parse(Substring(attributes, index + 9, attributes.IndexOf(",", index)));
                    ////
                    //Db_Type = Substring(attributes, attributes.IndexOf("type=") + 5, attributes.IndexOf(",DBhost="));
                    //Db_host = Substring(attributes, attributes.IndexOf("DBhost=") + 7, attributes.IndexOf(",DBport="));
                    //Db_port = Substring(attributes, attributes.IndexOf("DBport=") + 7, attributes.IndexOf(",DBname="));
                    //Db_name = Substring(attributes, attributes.IndexOf("DBname=") + 7, attributes.IndexOf(",UID="));
                    ////
                    //Db_uid = Substring(attributes, attributes.IndexOf("UID=") + 4, attributes.IndexOf(",PWD="));
                    //Db_pwd = Substring(attributes, attributes.IndexOf("PWD=") + 4, attributes.IndexOf("]"));

                    int index = attributes.IndexOf("HOST=", StringComparison.OrdinalIgnoreCase);
                    Db_host = Substring(attributes, index + 5, attributes.IndexOf(")", index));

                    index = attributes.IndexOf("PORT=", StringComparison.OrdinalIgnoreCase);
                    Db_port = Substring(attributes, attributes.IndexOf("PORT=", StringComparison.OrdinalIgnoreCase) + 5, attributes.IndexOf(")", index));
                    index = attributes.IndexOf("SERVICE_NAME=", StringComparison.OrdinalIgnoreCase);
                    Db_name = Substring(attributes, attributes.IndexOf("SERVICE_NAME=", StringComparison.OrdinalIgnoreCase) + 13, attributes.IndexOf(")", index));
                    index = attributes.IndexOf("User Id=", StringComparison.OrdinalIgnoreCase);
                    Db_uid = Substring(attributes, attributes.IndexOf("User Id=", StringComparison.OrdinalIgnoreCase) + 8, attributes.IndexOf(";", index));
                    Db_pwd = attributes.Substring(attributes.IndexOf("Password=", StringComparison.OrdinalIgnoreCase) + 9);
                    //
                }
                if (IsPostgreSQL())
                {
                    //Server=192.168.0.164;Port=5432;User Id=frameworkdb;Password=frameworkdb;Database=frameworkdb;"></add>
                    int index = attributes.IndexOf("Server=", StringComparison.OrdinalIgnoreCase);
                    Db_host = Substring(attributes, index + 7, attributes.IndexOf(";", index));
                    index = attributes.IndexOf("PORT=", StringComparison.OrdinalIgnoreCase);
                    Db_port = Substring(attributes, index + 5, attributes.IndexOf(";", index));
                    index = attributes.IndexOf("User Id=", StringComparison.OrdinalIgnoreCase);
                    Db_uid = Substring(attributes, index + 8, attributes.IndexOf(";", index));
                    index = attributes.IndexOf("Password=", StringComparison.OrdinalIgnoreCase);
                    Db_pwd = Substring(attributes, index + 9, attributes.IndexOf(";", index));
                    index = attributes.IndexOf("SearchPath=", StringComparison.OrdinalIgnoreCase);
                    if (index > 0)
                    {
                        Db_searchPath = Substring(attributes, index + 11, attributes.IndexOf(";", index));
                    }
                    Db_name = attributes.Substring(attributes.IndexOf("Database=", StringComparison.OrdinalIgnoreCase) + 9);
                }
            }
            catch (Exception e)
            {
                log.Severe(attributes + " - " + e.Message);
            }
        }	//  setAttributes


        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="db_host"></param>
        /// <param name="db_port"></param>
        /// <param name="db_name"></param>
        /// <param name="db_uid"></param>
        /// <param name="db_pwd"></param>
        /// <returns></returns>
        public static VConnection Get(String type, String db_host, String db_port, String db_name, String db_uid, String db_pwd)
        {
            VConnection cc = new VConnection();
            cc.Apps_host = db_host;
            cc.Db_Type = type;
            cc.Db_host = db_host;
            cc.Db_port = db_port;
            cc.Db_name = db_name;
            //
            if (db_uid != null)
                cc.Db_uid = db_uid;
            if (db_pwd != null)
                cc.Db_pwd = db_pwd;
            return cc;
        }	//  get


        public static VConnection Get()
        {

            if (s_cc == null)
            {

                ////String attributes = Ini.GetProperty(Ini.P_CONNECTION);
                // if ((attributes == null) || (attributes.Length == 0))
                //{
                //    VConnection cc = new VConnection();
                //    cc.SetAttributes();		//	initial environment
                //   // CConnectionDialog ccd = new CConnectionDialog(cc);
                //   // s_cc = ccd.GetConnection();
                //    //  set also in ALogin and Ctrl
                //   // Ini.SetProperty(Ini.P_CONNECTION, s_cc.ToStringLong());
                //   // Ini.SaveProperties(Ini.IsClient());
                //}
                //else	//	existing environment properties
                //{
                s_cc = new VConnection();

                //s_cc.SetAttributes(attributes);
                //}
                //log.Fine(s_cc.ToString());
            }

            return s_cc;
        } 	//  get


        public static VConnection Get(String type, String dbhost, String dbport, String dbname)
        {
            return Get(type, dbhost, dbport, dbname, null, null);
        } 	//  get


        /// <summary>
        /// Vienna Connection
        /// </summary>
        public VConnection()
        {
            String hostName = "localhost";
            try
            {

                // Db_Type = System.Configuration.ConfigurationSettings.AppSettings["DB_Type"].ToString();
                //  CreateDBConnectionString();


                //IPHostEntry ip = Dns.GetHostByName(hostName);
                // hostName = ip.HostName;
            }
            catch
            {
            }

            m_apps_host = hostName;
            m_db_host = hostName;
        }




        public Exception TestAppServer()
        {
            //First step is to check if the application host is pinging or not

            try
            {
                Ping ping = new Ping();
                if (IPStatus.Success != ping.Send(Apps_host).Status)
                {
                    return new Exception("Could not connect");
                }
            }
            catch (Exception e1)
            {
                return e1;
            }

            //TcpChannel tcp = new TcpChannel();  //create a new tcp channel
            IChannel[] reg_channels = ChannelServices.RegisteredChannels;

            //ServerBean m_server = null;
            //if (reg_channels.Length <= 0)
            //    ChannelServices.RegisterChannel(tcp, false); //register the created channel
            try
            {
                //right now, viennaFramework has only one way to connect i.e tcp protocols
                //we will soon be making it available with IIS > 6.0 as well.
                //// m_server = (ServerBean)Activator.GetObject(typeof(ServerBean), "tcp://" + Apps_host + ":" + Apps_port + "/ViennaFramework");
                // string sttr = "";
                // if (m_server.TryConnection(out sttr) == "OK")
                // {
                //     return null;
                // }
            }
            catch (Exception ex)
            {
                //ChannelServices.UnregisterChannel(tcp);     //unregister the channel in case error occurs. !Important
                return ex;
            }

            return null;

        }

        //public Exception Connect()
        //{

        //    return Connect(Db_host, Db_port, Db_uid, Db_pwd, Db_name);
        //}

//        public Exception Connect(string server, string port, string userid, string password, string database)
//        {
//            string connection_string = Ini.CreateConnectionString(this);
//            IDbConnection conn = null;
//            try
//            {
//                if (IsOracle())
//                {
//#pragma warning disable 612, 618
//                    conn = new OracleConnection(connection_string);
//#pragma warning restore 612, 618
//                    conn.Open();
//                }
//                else if (IsPostgreSQL())
//                {

//                    conn = new NpgsqlConnection(connection_string);
//                    conn.Open();
//                }
//                //else if (IsMSSQLServer())
//                //{
//                //    conn = new System.Data.SqlClient.SqlConnection(connection_string);
//                //    conn.Open();
//                //}

//            }
//            catch (Exception ex)
//            {
//                return ex;
//            }
//            finally
//            {
//                conn.Close();
//            }
//            return null;
//        }

        //public Exception TestDatabase()
        //{
        //    return Connect(Db_host, Db_port, Db_uid, Db_pwd, Db_name);
        //}

        // private Exception m_dbException = null;



        private void SetAttributes()
        {
            Name = ToString();
        }	//  setAttributes

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vc"></param>
        public VConnection(VConnection vc)
        {
            this.Db_Type = vc.Db_Type;
            Db_host = vc.Db_host;
            Db_port = vc.Db_port;
            Db_name = vc.Db_name;
            Db_uid = vc.Db_uid;
            Db_pwd = vc.Db_pwd;
        }

        public ViennaDatabase GetDatabase()
        {
            if (m_db != null && !m_db.GetName().Equals(m_type))
                m_db = null;

            if (m_db == null)
            {
                try
                {
                    //for (int i = 0; i < DatabaseType.DB_NAMES.Length; i++)
                    //{
                    //    if (DatabaseType.DB_NAMES[i].Equals(m_type))
                    //    {
                    //        m_db = (ViennaDatabase)DatabaseType.DB_CLASSES[i].
                    //                   newInstance();
                    //        break;
                    //    }
                    //}
                    m_db = DatabaseType.GetDatabase(m_type);
                   
                }
                
                catch (Exception e)
                {
                    log.Severe(e.Message);
                }
            }
            return m_db;
        }
    }

    public class VEnvironment
    {
        /**	AS Type JBoss (default)		*/
        public static String APPSTYPE_VSERVER = "vserver";
        /**	AS Type JBoss (default)		*/
        public static String APPSTYPE_IIS = "iis";

        /** DB Type PostgreSQL			*/
        public static String DBTYPE_PG = "P";
        /** DB Type Oracle Std			*/
        public static String DBTYPE_ORACLE = "O";
        /** DB Type DB/2				*/
        public static String DBTYPE_DB2 = "D";
        /** DB Type MS SQL Server		*/
        //public static final String		DBTYPE_MS = "<sqlServer>";
        public static String DBTYPE_MS = "S";


        public static string VIENNA_DB_SYSTEM = "VIENNA_DB_SYSTEM";
        public static string VIENNA_DB_SYSTEM_PWD = "VIENNA_DB_SYSTEM_PWD";
        public static string VIENNA_DB_NAME = "VIENNA_DB_NAME";
        public static string VIENNA_DB_PASSWORD = "VIENNA_DB_PASSWORD";
        public static string VIENNA_DB_USER = "VIENNA_DB_USER";
        public static string VIENNA_DB_PORT = "VIENNA_DB_PORT";
        public static string VIENNA_DB_PATH = "VIENNA_DB_PATH";
        public static string VIENNA_DB_TYPE = "VIENNA_DB_TYPE";
        public static string VIENNA_DB_SERVER = "VIENNA_DB_SERVER";


    }
}
