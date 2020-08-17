using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace VAdvantage.DataBase
{
    public static class IniPreference
    {
        private const string _PREF_DOC_PATH = @"C:\VAdvantage\preference.xml";  //path of the xml file which contains connection string
        /// <summary>
        /// Gets the XML Dcoument Path
        /// </summary>
        public static string PREF_DOC_PATH
        {
            get { return _PREF_DOC_PATH; }
        }

        /// <summary>
        /// Root node of the xml file which contains server configuration
        /// </summary>
        private const string _PREF_ROOT = "//preferences";   //root node of connection string xml file
        /// <summary>
        /// Gets the Root node of XML Document
        /// </summary>
        public static string PREF_ROOT
        {
            get { return _PREF_ROOT; }
        }

        public static string AUTOCOMMIT_NODE
        {
            get
            {
                return "autocommit";
            }
        }

        public static string SHOWACCTTAB_NODE
        {
            get
            {
                return "showaccttab";
            }
        }

        public static string SHOWADVTAB_NODE
        {
            get
            {
                return "showadvtab";
            }
        }

        public static string SHOWTRLTAB_NODE
        {
            get
            {
                return "showtrltab";
            }
        }

        public static string AUTOLOGIN_NODE
        {
            get
            {
                return "autologin";
            }
        }

        public static string STOREPWD_NODE
        {
            get
            {
                return "storepwd";
            }
        }

        public static string AUTONEWRECORD_NODE
        {
            get
            {
                return "autonewrecord";
            }
        }

        public static string CACHEWINDOWS_NODE
        {
            get
            {
                return "cachewindows";
            }
        }

        public static string PREVIEWPRINTER_NODE
        {
            get
            {
                return "previewprint";
            }
        }

        public static string PRINTER_NODE
        {
            get
            {
                return "printer";
            }
        }

        public static string TRACEFILE_NODE
        {
            get
            {
                return "tracefile";
            }
        }

        public static string DICTIONARY_NODE
        {
            get
            {
                return "dictionary";
            }
        }
        /// <summary>
        /// Fetches the Node value from xml file of a specific node
        /// </summary>
        /// <param name="value">Name of the node whose values is to be fetched</param>
        /// <returns>Value of the node</returns>
        public static string GetProperty(string value)
        {
            string return_value = "";   //final output to be returned to caller.
            XmlDocument doc = new XmlDocument();    //Initialize the XMLDocument
            doc.Load(PREF_DOC_PATH); //load the file into document

            XmlNodeList nodes = doc.SelectNodes(PREF_ROOT);  //select the top node of the xml
            //loop through all the nodes in xml file
            foreach (XmlNode node in nodes)
            {
                return_value = node.SelectSingleNode(value).InnerXml;   //set the selected value into return variable
            }
            return return_value;    //finally return the value to the caller.
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        public static void SetProperty(string key, string value)
        {
            XmlDocument doc = new XmlDocument();    //Initialize the XMLDocument
            doc.Load(PREF_DOC_PATH); //load the file into document

            XmlNodeList nodes = doc.SelectNodes(PREF_ROOT);  //select the top node of the xml
            //loop through all the nodes in xml file
            foreach (XmlNode node in nodes)
            {
                //set the selected value into return variable
                node.SelectSingleNode(value).InnerText = key;
            }
            doc.Save(PREF_DOC_PATH);  //finally return the value to the caller.

        }
    }
}
