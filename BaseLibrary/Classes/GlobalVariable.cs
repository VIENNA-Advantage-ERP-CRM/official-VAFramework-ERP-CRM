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


namespace VAdvantage.DataBase
{
    /// <summary>
    /// Contains all the Gloabal Variables to be used in Application
    /// </summary>
    public sealed class GlobalVariable

    {
        public static string AD_BASE_LANGUAGE = "en-US";

        public static string ACCESSKEY = "caff4eb4fbd6273e37e8a325e19f0991";
        private static bool _isEditor = false;

        public static string PhysicalPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
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
        public static String ImagePath = System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "Images");
        public static String TempDowloadPath = System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "TempDownload");
        public static String ApplicationPhysicalPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;




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

        private static string _VAF_MSG_LABLE;

        public static string VAF_MSG_LABLE
        {
            get { return _VAF_MSG_LABLE; }
            set { _VAF_MSG_LABLE = value; }
        }
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
            return "";
           // return Utility.Env.GetContext().GetVAF_UserContact_Name() + "@" + Utility.Env.GetContext().GetVAF_Org_Name() + '.' + Utility.Env.GetContext().GetVAF_Role_Name() + "[ " + GetHost + " ]";
        }

        private static string _LAST_EXECUTED_QUERY = "";

        public static string LAST_EXECUTED_QUERY
        {
            get { return _LAST_EXECUTED_QUERY; }
            set { _LAST_EXECUTED_QUERY = value; }
        }

        //---------------Created By - -------------//




        public const string PRODUCT_NAME = "ViennaAdvantageSvc";

        public static string[] PACKAGES = new string[]
        {
            "VAWorkflow","ModelLibrary","VAModelAD","XModel"
        };


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
            return DB.TO_STRING(txt, maxLength);
            //if (string.IsNullOrEmpty(txt))
            //    return "NULL";

            ////  Length
            //String text = txt;
            //if (maxLength != 0 && text.Length > maxLength)
            //    text = txt.Substring(0, maxLength);

            ////  copy characters		(we need to look through anyway)
            //StringBuilder outt = new StringBuilder();
            //outt.Append(QUOTE);		//	'
            //for (int i = 0; i < text.Length; i++)
            //{
            //    char c = text[i];
            //    if (c == QUOTE)
            //        outt.Append("''");
            //    else
            //        outt.Append(c);
            //}
            //outt.Append(QUOTE);		//	'
            ////
            //return outt.ToString();
        }	//	TO_STRING

       

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

        /// <summary>
        /// Adds specified number of value to current datetime
        /// </summary>
        /// <param name="duration">integer number from CommonFuctions.Calendar enum</param>
        /// <param name="time"></param>
        /// <returns>new date</returns>
        /// <author>Veena</author>
        

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

        public static String P_VIENNASYS = "ViennaSys"; //	Save system records



        //#region "Calendar Variables"
        //// used in workflow
        //public static int Minute = 12;
        //public static int Second = 13;
        //public static int Hour = 10;
        //public static int DayOfMonth = 5;
        //public static int DayOfYear = 6;
        //public static int Month = 2;
        //public static int Year = 1;

        //#endregion

        public static string GetLanguageCode()
        {
            return "en_US";
            //return Utility.Env.GetLoginLanguage(Utility.Env.GetContext()).GetVAF_Language();
            //if (VAF_LANGUAGE != "")
            //    return    VAF_LANGUAGE.Replace("-", "_");
            //else
            //    return "";
        }
    }
}