/********************************************************
 * Module Name    :     General
 * Purpose        :     Contains GlobalVairables and function
 * Author         :     Jagmohan Bhatt
 * Date           :     12-Nov-2008
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Process;

namespace VAdvantage.DataBase
{
    /// <summary>
    /// Contains all the Gloabal Variables to be used in Application
    /// </summary>
    public sealed class GlobalVariable
    
    {


        public  static  string ACCESSKEY = "caff4eb4fbd6273e37e8a325e19f0991";
        private static bool _isEditor = false;
        /// <summary>
        /// Property used in Visual Editor From
        /// </summary>
        public static bool IsVisualEditor
        {
            get
            {
                return _isEditor;
            }
            set
            {
                _isEditor = value;
            }
        }

        private static bool _isVserver = false;
        /// <summary>
        /// Property used in Visual Editor From
        /// </summary>
        public static bool IsVserver
        {
            get
            {
                return _isVserver;
            }
            set
            {
                _isVserver = value;
            }
        }

        public const string SMTP_CONFIG_FILE = "SMTPConfig.cfg";
        public const string IMAP_CONFIG_FILE = "IMAPConfig_";
        public const string EMAIL_FOLDER = "EMail";
        public const string EMAIL_TEMP_FOLDER = "EMail\\Temp";
        
        #region Miscellaneous Properties
        /// <summary>
        /// _DEFT_COMBO_VALUE contains default DataRowView value in combobox
        /// </summary>
        private const string _DEFT_COMBO_VALUE = "System.Data.DataRowView";
        /// <summary>
        /// DEFT_COMBO_VALUE Property to get the value of _DEFT_COMBO_VALUE
        /// </summary>
        public static string DEFT_COMBO_VALUE
        {
            get { return _DEFT_COMBO_VALUE; }
        }

        private const string _FILE_SEPRATOR = "\\";

        public string FILE_SEPRATOR
        {
            get
            {
                return _FILE_SEPRATOR;
            }
        }

        #endregion


        public static String AttachmentPath = System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "Attachments");

        #region GenrateClass Path
        /// <summary>
        /// set the class,to genrate the class 
        /// </summary>
        private static String _strPath;
        /// <summary>
        /// Set the Class path
        /// </summary>
        public static string GenClassPath
        {
            get { return GlobalVariable._strPath; }
            set { GlobalVariable._strPath = value; }
        }
        #endregion

        #region GetALLTableName
        /// <summary>
        /// get set Tableid from AD_TABLE
        /// </summary>
        private static string _AD_TABLE_ID;
        /// <summary>
        /// get Tableid from AD_TABLE
        /// Set Tableid from AD_TABLE
        /// </summary>
        public static string GetTableId
        {
            get { return GlobalVariable._AD_TABLE_ID; }
            set { GlobalVariable._AD_TABLE_ID = value; }
        }
        #endregion

        #region CheckBox Status
        /// <summary>
        /// get set Tableid from AD_TABLE
        /// </summary>
        private static bool _chkStatus;
        /// <summary>
        /// get Check box status
        /// Set Check box status
        /// </summary>
        public static bool GetChekboxStatus
        {
            get { return _chkStatus; }
            set { _chkStatus = value; }
        }
        #endregion

        #region Class Type
        /// <summary>
        /// set the class,to genrate the class 
        /// </summary>
        private static string _clsType;
        /// <summary>
        /// Set the Class path
        /// </summary>
        public static string ClassType
        {
            get { return GlobalVariable._clsType; }
            set { GlobalVariable._clsType = value; }
        }
        #endregion

        #region Login Properties

        /// <summary>
        /// _AD_LANGUAGE variable contains the currently selected Language
        /// </summary>
        private static string _AD_LANGUAGE;
        /// <summary>
        /// Gets or Sets get the value of static variable _AD_LANGUAGE
        /// </summary>
        public static string AD_LANGUAGE
        {
            get { return GlobalVariable._AD_LANGUAGE; }
            set { GlobalVariable._AD_LANGUAGE = value; }
        }
        /// <summary>
        /// _AD_BASE_LANGUAGE variable contains the default Language english(en-EN)
        /// </summary>
        private static string _AD_BASE_LANGUAGE = "en_US";
        /// <summary>
        /// Gets or Sets get the value of static variable _AD_BASE_LANGUAGE
        /// </summary>
        public static string AD_BASE_LANGUAGE
        {
            get
            {
                return GlobalVariable._AD_BASE_LANGUAGE;
            }
            //set
            //{
            //    GlobalVariable._AD_BASE_LANGUAGE ="en-US";
            //}
        }


        /// <summary>
        /// _AD_SESSION_ID variable contains the current session ID of the user
        /// </summary>
        private static string _AD_SESSION_ID;

        /// <summary>
        /// Gets or Sets get the value of static variable _AD_SESSION_ID
        /// </summary>
        public static string AD_SESSION_ID
        {
            get { return GlobalVariable._AD_SESSION_ID; }
            set { GlobalVariable._AD_SESSION_ID = value; }
        }
        #endregion

        //#region Database Properties
        ///// <summary>
        ///// Contains the Value Name of the Postgre SQL Database
        ///// </summary>
        //private const string _PGSQL_VALUE_NAME = "postgreSQL";

        ///// <summary>
        ///// Gets the Value Name of PostgreSQL for Combobox
        ///// </summary>
        //public static string PGSQL_VALUE_NAME
        //{
        //    get { return _PGSQL_VALUE_NAME; }
        //}

        ///// <summary>
        ///// Contains the Value Name of the MS SQL Server Database
        ///// </summary>
        //private const string _MSSQL_VALUE_NAME = "sqlServer";

        ///// <summary>
        ///// Gets the Value Name of MS Sql Server for Combobox
        ///// </summary>
        //public static string MSSQL_VALUE_NAME
        //{
        //    get { return _MSSQL_VALUE_NAME; }
        //}

        ///// <summary>
        ///// Contains the Value Name of the MySQL Database
        ///// </summary>
        //private const string _MYSQL_VALUE_NAME = "mySql";

        ///// <summary>
        ///// Gets the Value Name of MySQL for Combobox
        ///// </summary>
        //public static string MYSQL_VALUE_NAME
        //{
        //    get { return _MYSQL_VALUE_NAME; }
        //}

        ///// <summary>
        ///// Contains the Value Name of the Oracle Database
        ///// </summary>
        //private const string _ORCL_VALUE_NAME = "oracle";

        ///// <summary>
        ///// Gets the Value Name of Oracle for Combobox
        ///// </summary>
        //public static string ORCL_VALUE_NAME
        //{
        //    get { return _ORCL_VALUE_NAME; }
        //}

        ///// <summary>
        ///// Contains the display Name of the Postgre SQL Database
        ///// </summary>
        //private const string _PGSQL_DISPLAY_NAME = "postgreSQL";

        ///// <summary>
        ///// Gets the Display Name of PostgreSQL for Combobox
        ///// </summary>
        //public static string PGSQL_DISPLAY_NAME
        //{
        //    get { return _PGSQL_DISPLAY_NAME; }
        //}

        ///// <summary>
        ///// Contains the Display Name of the MS SQL Server Database
        ///// </summary>
        //private const string _MSSQL_DISPLAY_NAME = "sqlServer";

        ///// <summary>
        ///// Gets the Display Name of MS Sql Server for Combobox
        ///// </summary>
        //public static string MSSQL_DISPLAY_NAME
        //{
        //    get { return _MSSQL_DISPLAY_NAME; }
        //}

        ///// <summary>
        ///// Contains the display Name of the MySQL Database
        ///// </summary>
        //private const string _MYSQL_DISPLAY_NAME = "mySql";

        ///// <summary>
        ///// Gets the Display Name of MySQL for Combobox
        ///// </summary>
        //public static string MYSQL_DISPLAY_NAME
        //{
        //    get { return _MYSQL_DISPLAY_NAME; }
        //}

        ///// <summary>
        ///// Contains the display Name of the Oracle Database
        ///// </summary>
        //private const string _ORCL_DISPLAY_NAME = "oracle";

        ///// <summary>
        ///// Gets the Display Name of Oracle for Combobox
        ///// </summary>
        //public static string ORCL_DISPLAY_NAME
        //{
        //    get { return _ORCL_DISPLAY_NAME; }
        //}

        ///// <summary>
        ///// Contains the Default Port Number of Postgre SQL
        ///// </summary>
        //private const string _PGSQL_DEFAULT_PORT = "5444";

        ///// <summary>
        ///// Gets the Default Port number of PostgreSQL
        ///// </summary>
        //public static string PGSQL_DEFAULT_PORT
        //{
        //    get { return _PGSQL_DEFAULT_PORT; }
        //}

        ///// <summary>
        ///// Contains the Default Port Number of MS SQL Server
        ///// </summary>
        //private const string _MSSQL_DEFAULT_PORT = "1433";

        ///// <summary>
        ///// Gets the Default Port number of MS SQL SERVER
        ///// </summary>
        //public static string MSSQL_DEFAULT_PORT
        //{
        //    get { return _MSSQL_DEFAULT_PORT; }
        //}

        ///// <summary>
        ///// Contains the Default Port Number of MySQL
        ///// </summary>
        //private const string _MYSQL_DEFAULT_PORT = "3306";

        ///// <summary>
        ///// Gets the Default Port number of MySQL
        ///// </summary>
        //public static string MYSQL_DEFAULT_PORT
        //{
        //    get { return _MYSQL_DEFAULT_PORT; }
        //}

        ///// <summary>
        ///// Contains the Default Port Number of Oracle 10g
        ///// </summary>
        //private const string _ORCL_DEFAULT_PORT = "1521";

        ///// <summary>
        ///// Gets the Default Port number of Oracle10g
        ///// </summary>
        //public static string ORCL_DEFAULT_PORT
        //{
        //    get { return _ORCL_DEFAULT_PORT; }
        //}

        //#endregion

        #region XML Properties
        /// <summary>
        /// Contains the XML Node Name of the Host hosting the application
        /// </summary>
        private const string _HOST_NODE = "dbhost";

        /// <summary>
        /// Gets the Name of the Host from XML File
        /// </summary>
        public static string HOST_NODE
        {
            get { return _HOST_NODE; }
        }

        private const string _USERID_NODE = "dbuserid";

        /// <summary>
        /// Gets the Name of the UserID Node from XML file
        /// </summary>
        public static string USERID_NODE
        {
            get { return _USERID_NODE; }
        }

        private const string _PWD_NODE = "dbpwd";

        /// <summary>
        /// Gets the Password Node Name from XML file
        /// </summary>
        public static string PWD_NODE
        {
            get { return _PWD_NODE; }
        }

        private const string _PORT_NODE = "dbport";

        /// <summary>
        /// Gets the PortNumber Node from XML file
        /// </summary>
        public static string PORT_NODE
        {
            get { return _PORT_NODE; }
        }

        /// <summary>
        /// _DBNAME_NODE variable contains the Name of the database application is connected with
        /// </summary>
        private const string _DBNAME_NODE = "dbname";

        /// <summary>
        /// Gets the Name of the DB Name Node from XML file
        /// </summary>
        public static string DBNAME_NODE
        {
            get { return _DBNAME_NODE; }
        }

        /// <summary>
        /// _DBTYPE_NODE variable contains the XML Node of the Database Type
        /// </summary>
        private const string _DBTYPE_NODE = "dbused";

        /// <summary>
        /// Gets the Name of the DB Type Node from XML file
        /// </summary>
        public static string DBTYPE_NODE
        {
            get { return _DBTYPE_NODE; }
        }

        private static string _AD_MESSAGE;

        public static string AD_MESSAGE
        {
            get { return _AD_MESSAGE; }
            set { _AD_MESSAGE = value; }
        }
        #endregion

        #region Functions to resolve Display to Value format and Vice-Verca
        /// <summary>
        /// Accepts dbtype in Display format and return the value format for ComboBox
        /// e.g: MS SQL Server =  mssql;
        /// </summary>
        /// <param name="dbtype">Database Type name in Display format</param>
        /// <returns>Database Type name in Value format</returns>
        //public static string ResolveDBTypeForValue(string dbtype)
        //{
        //    if (dbtype == GlobalVariable._MSSQL_DISPLAY_NAME)
        //        return GlobalVariable._MSSQL_VALUE_NAME;
        //    else if (dbtype == GlobalVariable._PGSQL_DISPLAY_NAME)
        //        return GlobalVariable._PGSQL_VALUE_NAME;
        //    else if (dbtype == GlobalVariable._ORCL_DISPLAY_NAME)
        //        return GlobalVariable._ORCL_VALUE_NAME;
        //    else if (dbtype == GlobalVariable._MYSQL_DISPLAY_NAME)
        //        return GlobalVariable._MYSQL_VALUE_NAME;

        //    return GlobalVariable._PGSQL_VALUE_NAME;
        //}

        /// <summary>
        /// Accepts dbtype in Value format and return the Display format for ComboBox
        /// e.g: mssql =  MS SQL Server;
        /// </summary>
        /// <param name="dbtype">Database Type name in Value format</param>
        /// <returns>Database Type name in Display format</returns>
        //public static string ResolveDBTypeForDisplay(string dbtype)
        //{
        //    if (dbtype == GlobalVariable._MSSQL_VALUE_NAME)
        //        return GlobalVariable._MSSQL_DISPLAY_NAME;
        //    else if (dbtype == GlobalVariable._PGSQL_VALUE_NAME)
        //        return GlobalVariable._PGSQL_DISPLAY_NAME;
        //    else if (dbtype == GlobalVariable._ORCL_VALUE_NAME)
        //        return GlobalVariable._ORCL_DISPLAY_NAME;
        //    else if (dbtype == GlobalVariable._MYSQL_VALUE_NAME)
        //        return GlobalVariable._MYSQL_DISPLAY_NAME;

        //    return GlobalVariable._PGSQL_DISPLAY_NAME;
        //}
        #endregion

        #region Extract DB Properties
        /// <summary>
        /// Gets the Database configured with application
        /// </summary>
        public static string WhichDatabase
        {
            get
            {
                return Ini.GetProperty(GlobalVariable.DBTYPE_NODE);
            }
        }

        /// <summary>
        /// Gets the Host Name configured and saved with application
        /// </summary>
        public static string GetHost
        {
            get { return Ini.GetProperty(GlobalVariable.HOST_NODE); }
        }

        /// <summary>
        /// Gets the Port Number of the database configured with Application
        /// </summary>
        public static string GetPort
        {
            get { return Ini.GetProperty(GlobalVariable.PORT_NODE); }
        }

        /// <summary>
        /// Gets the UserID of the database configured
        /// </summary>
        public static string GetUserID
        {
            get { return Ini.GetProperty(GlobalVariable.USERID_NODE); }
        }

        /// <summary>
        /// Gets the Password of the databaes configured
        /// </summary>
        public static string GetPassword
        {
            get { return Ini.GetProperty(GlobalVariable.PWD_NODE); }
        }

        /// <summary>
        /// Gets the Name of the databae configured with application
        /// </summary>
        public static string GetDBName
        {
            get { return Ini.GetProperty(GlobalVariable.DBNAME_NODE); }
        }
        #endregion

        /// <summary>
        /// Get Title Of Page
        /// </summary>
        /// <returns></returns>
        public static string GetTitle()
        {
            return Utility.Env.GetContext().GetAD_User_Name() + "@" + Utility.Env.GetContext().GetAD_Org_Name() + '.' + Utility.Env.GetContext().GetAD_Role_Name() + "[ " + GetHost + " ]";
        }

        private static string _LAST_EXECUTED_QUERY = "";

        public static string LAST_EXECUTED_QUERY
        {
            get { return _LAST_EXECUTED_QUERY; }
            set { _LAST_EXECUTED_QUERY = value; }
        }

        //---------------Created By - Harwinder-------------//

        #region BaseLanguage

        /// <summary>
        /// is Base Language (en-US)
        /// </summary>
        /// <returns>true if base language</returns>
        public static bool IsBaseLanguage()
        {
            return true;
            //return AD_BASE_LANGUAGE.Equals(Utility.Env.GetLoginLanguage(Utility.Env.GetContext()).GetAD_Language());
        }

        /// <summary>
        /// Get DataBase Code of Language
        /// </summary>
        /// <returns></returns>
        public static string GetLanguageCode()
        {
            return "en_US";
            //return Utility.Env.GetLoginLanguage(Utility.Env.GetContext()).GetAD_Language();
            //if (AD_LANGUAGE != "")
            //    return    AD_LANGUAGE.Replace("-", "_");
            //else
            //    return "";
        }

        /// <summary>
        /// Table is in Base Translation (AD)
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static bool IsBaseTranslation(string tableName)
        {
            if (tableName.StartsWith("AD")
                || tableName.Equals("C_Country_Trl"))
                return true;
            return false;
        }	//	

        #endregion

        #region Context
        // private static VAdvantage.Classes.Context ctx = new VAdvantage.Classes.Context();

        // /// <summary>
        // /// Get Context Object
        // /// </summary>
        // /// <returns></returns>
        // public static VAdvantage.Classes.Context GetContext()
        // {
        //     return ctx;
        // }

        // /// <summary>
        /////Parse Context replaces global or Window context @tag@ with actual value.
        ///// </summary>
        ///// <param name="ctx">ctx context</param>
        ///// <param name="WindowNo">Number of Window</param>
        ///// <param name="value">Message to be parsed</param>
        ///// <param name="onlyWindow">onlyWindow  if true, no defaults are used</param>
        ///// <returns>parsed String or "" if not successful</returns>
        // public static string ParseContext(Context ctx, int WindowNo, string value,
        //  bool onlyWindow)
        // {
        //     return ParseContext(ctx, WindowNo, value, onlyWindow, false);
        // }

        // /// <summary>
        // ///	Parse Context replaces global or Window context @tag@ with actual value.
        // ///  @tag@ are ignored otherwise "" is returned
        // /// </summary>
        // /// <param name="ctx">context</param>
        // /// <param name="WindowNo">Number of Window</param>
        // /// <param name="value">value Message to be parsed</param>
        // /// <param name="onlyWindow">if true, no defaults are used</param>
        // /// <param name="ignoreUnparsable">if true, unsuccessful @return parsed String or "" if not successful and ignoreUnparsable</param>
        // /// <returns>parsed context </returns>
        // public static string ParseContext(Context ctx, int WindowNo, string value,
        // bool onlyWindow, bool ignoreUnparsable)
        // {
        //     if (value == null || value.Length == 0)
        //         return "";

        //     string token;

        //     StringBuilder outStr = new StringBuilder("");

        //     int i = value.IndexOf('@');
        //     // Check whether the @ is not the last in line (i.e. in EMailAdress or with wrong entries)
        //     while (i != -1 && i != value.LastIndexOf("@"))
        //     {
        //         outStr.Append(value.Substring(0, i));			// up to @
        //         value = value.Substring(i + 1, value.Length - i - 1);	// from first @

        //         int j = value.IndexOf('@');						// next @
        //         if (j < 0)
        //         {
        //             //s_log.log(Level.SEVERE, "No second tag: " + inStr);
        //             return "";						//	no second tag
        //         }

        //         token = value.Substring(0, j);

        //         string ctxInfo = ctx.GetContext(WindowNo, token, onlyWindow);	// get context
        //         if (ctxInfo.Length == 0 && (token.StartsWith("#") || token.StartsWith("$")))
        //             ctxInfo = ctx.GetContext(token);	// get global context
        //         if (ctxInfo.Length == 0)
        //         {
        //             //s_log.config("No Context Win=" + WindowNo + " for: " + token);
        //             if (!ignoreUnparsable)
        //                 return "";						//	token not found
        //         }
        //         else
        //             outStr.Append(ctxInfo);				// replace context with Context

        //         value = value.Substring(j + 1, value.Length - j - 1);	// from second @
        //         i = value.IndexOf('@');
        //     }
        //     outStr.Append(value);						// add the rest of the string

        //     return outStr.ToString();
        // }	//	parseContext

        // /// <summary>
        // /// Get Preference.
        // ///  <pre>
        // ///		0)	Current Setting
        // /// 		1) 	Window Preference
        // ///		2) 	Global Preference
        // ///		3)	Login settings
        // ///		4)	Accounting settings
        // ///  </pre>
        // /// </summary>
        // /// <param name="ctx">context</param>
        // /// <param name="AD_Window_ID">window no</param>
        // /// <param name="context">Entity to search</param>
        // /// <param name="system">System level preferences (vs. user defined)</param>
        // /// <returns>preference value</returns>
        // public static String GetPreference(Context ctx, int AD_Window_ID, String context, bool system)
        //{
        //    if (ctx == null || context == null)
        //        throw new ArgumentException("Require Context");
        //    String retValue = null;
        //    //
        //    if (!system)	//	User Preferences
        //    {
        //        retValue = ctx.GetContext("P" + AD_Window_ID + "|" + context);//	Window Pref
        //        if (retValue.Length == 0)
        //            retValue = ctx.GetContext("P|" + context);  			//	Global Pref
        //    }
        //    else			//	System Preferences
        //    {
        //        retValue = ctx.GetContext("#" + context);   				//	Login setting
        //        if (retValue.Length == 0)
        //            retValue = ctx.GetContext("$" + context);   			//	Accounting setting
        //    }
        //    return (retValue == null ? "" : retValue);
        //}	//	getPreference

        #endregion

        #region "WindowNumber"
        //	Array of active Windows
        //private static List<object> _sWindows = new List<object>(20);

        ///// <summary>
        ///// 	Add Container and return WindowNo.
        ///// </summary>
        ///// <param name="win"></param>
        ///// <returns>WindowNo used for context</returns>
        //public static int CreateWindowNo(object win)
        //{
        //    int retValue = _sWindows.Count;
        //    _sWindows.Insert(_sWindows.Count, win);
        //    return retValue;
        //}	//

        #endregion

        //------------ End -----------------------//

        public const string PRODUCT_NAME = "ViennaAdvantageSvc";  
        public const string ASSEMBLY_NAME = "ModelLibrary";
        //public const string ASSEMBLY_NAME = "VAdvantage.exe";
         
        public static String TO_STRING(String txt)
        {
            return TO_STRING(txt, 0);
        }   //  TO_STRING

        /**
         *	Package Strings for SQL command in quotes.
         *  <pre>
         *		-	include in ' (single quotes)
         *		-	replace ' with ''
         *  </pre>
         *  @param txt  String with text
         *  @param maxLength    Maximum Length of content or 0 to ignore
         *  @return escaped string for insert statement (NULL if null)
         */
        public static String TO_STRING(String txt, int maxLength)
        {
            if (string.IsNullOrEmpty(txt))
                return "NULL";

            //  Length
            String text = txt;
            if (maxLength != 0 && text.Length > maxLength)
                text = txt.Substring(0, maxLength);

            //  copy characters		(we need to look through anyway)
            StringBuilder outt = new StringBuilder();
            outt.Append(QUOTE);		//	'
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == QUOTE)
                    outt.Append("''");
                else
                    outt.Append(c);
            }
            outt.Append(QUOTE);		//	'
            //
            return outt.ToString();
        }	//	TO_STRING

        public static DateTime? SetDateTimeUTC(DateTime? dateTime)
        {
            if (dateTime != null)
            {
                if (dateTime.Value.Kind == DateTimeKind.Unspecified)
                {
                    dateTime = new DateTime(dateTime.Value.Ticks, DateTimeKind.Local);// .ToUniversalTime().ToLocalTime().ToUniversalTime();
                    dateTime = dateTime.Value.ToUniversalTime();
                }
                else if (dateTime.Value.Kind == DateTimeKind.Local)
                {
                    dateTime = dateTime.Value.ToUniversalTime();
                }

            }
            return dateTime;
        }

        /// <summary>
        /// Retrun Date time string 
        /// </summary>
        /// <param name="time"></param>
        /// <param name="dayOnly"></param>
        /// <returns></returns>
        public static String TO_DATE(DateTime? time, bool dayOnly)
        {
            return DB.TO_DATE(time, dayOnly);
        }

        public static String SetDateFormat(DateTime time, bool dayOnly)
        {
            string myDate = "";
            if (DatabaseType.IsOracle)
            {
                if (time == null)
                {
                    if (dayOnly)
                        return DateTime.Now.ToString("yyyy-MM-dd");
                    return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }

                //  YYYY-MM-DD HH24:MI:SS.mmmm  JDBC Timestamp format
                //String myDate = time.ToString("yyyy-mm-dd");
                if (dayOnly)
                {
                    myDate = time.ToString("yyyy-MM-dd");
                }
                else
                {
                    myDate = time.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            else if (DatabaseType.IsPostgre)
            {
                if (time == null)
                {
                    if (dayOnly)
                        return "TRUNC(SysDate)";
                    return "SysDate";
                }

                //  YYYY-MM-DD HH24:MI:SS.mmmm  JDBC Timestamp format
                myDate = time.ToString("yyyy-MM-dd HH:mm:ss");
                if (dayOnly)
                {
                    myDate = time.ToString("yyyy-MM-dd");
                }
                else
                {
                    myDate = time.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            //else if (DatabaseType.IsMSSql)
            //{
            //    if (time == null)
            //    {
            //        if (dayOnly)
            //            return "CAST(STR(YEAR(Getdate()))+'-'+STR(Month(Getdate()))+'-'+STR(Day(Getdate())) AS DATETIME)";
            //        return "getdate()";
            //    }

            //    //  YYYY-MM-DD HH24:MI:SS.mmmm  JDBC Timestamp format
            //    myDate = time.ToString();
            //    if (dayOnly)
            //    {
            //        dateString.Append(myDate.Substring(0, 10));
            //        dateString.Append("' AS DATETIME)");
            //    }
            //    else
            //    {
            //        dateString.Append(myDate.Substring(0, myDate.IndexOf(".")));	//	cut off miliseconds
            //        dateString.Append("' AS DATETIME)");
            //    }
            //}
            return myDate;
        }

        /** Quote			*/
        private static char QUOTE = '\'';

        public static String P_VIENNASYS = "ViennaSys";	//	Save system records

        #region "New Design"


        private static System.Drawing.Color _TAB_HEADER_PANEL_BACKCOLOR = System.Drawing.ColorTranslator.FromHtml("#7F7F7F");

        public static System.Drawing.Color TAB_HEADER_PANEL_BACKCOLOR
        {
            get { return GlobalVariable._TAB_HEADER_PANEL_BACKCOLOR; }
            set { GlobalVariable._TAB_HEADER_PANEL_BACKCOLOR = value; }
        }

        private static System.Drawing.Color _TAB_HEADER_LABEL_FORECOLOR = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");

        public static System.Drawing.Color TAB_HEADER_LABEL_FORECOLOR
        {
            get { return GlobalVariable._TAB_HEADER_LABEL_FORECOLOR; }
            set { GlobalVariable._TAB_HEADER_LABEL_FORECOLOR = value; }
        }



        #endregion

        #region "Calendar Variables"
        // used in workflow
        public static int Minute = 12;
        public static int Second = 13;
        public static int Hour = 10;
        public static int DayOfMonth = 5;
        public static int DayOfYear = 6;
        public static int Month = 2;
        public static int Year = 1;

        #endregion
    }
}
