using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Logging;
using VAdvantage.Process;
using VAdvantage.Utility;
//using VAdvantage.Install;
using System.IO;
using System.Xml;
using VAdvantage.Login;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.ProcessEngine;
using VAdvantage.Model;
using VAdvantage.Install;

namespace VAdvantage.Process
{
    public class TranslationMgr
    {
        /// <summary>
        ///Translation	
        /// </summary>
        /// <param name="ctx">context</param>
        public TranslationMgr(Ctx ctx)
        {
            _ctx = ctx;
        }	//	Translation

        /**	DTD						*/
        public static String DTD = "<!DOCTYPE ViennaTrl PUBLIC \"-//Vienna, Inc.//DTD Vienna Translation 1.0//EN\" \"http://www.Viennasolutions.com\">";
        /**	XML Element Tag			*/
        public static String XML_TAG = "compiereTrl";
        public static String XML_TAG_Vienna = "ViennaTrl";
        /**	XML Attribute Table			*/
        public static String XML_ATTRIBUTE_TABLE = "table";
        /** XML Attribute Language		*/
        public static String XML_ATTRIBUTE_LANGUAGE = "language";

        /**	XML Row Tag					*/
        public static String XML_ROW_TAG = "row";
        /** XML Row Attribute ID		*/
        public static String XML_ROW_ATTRIBUTE_ID = "id";
        /** XML Row Attribute Translated	*/
        public static String XML_ROW_ATTRIBUTE_TRANSLATED = "trl";

        /**	XML Value Tag				*/
        public static String XML_VALUE_TAG = "value";
        /** XML Value Column			*/
        public static String XML_VALUE_ATTRIBUTE_COLUMN = "column";
        /** XML Value Original			*/
        public static String XML_VALUE_ATTRIBUTE_ORIGINAL = "original";

        /**	Table is centrally maintained	*/
        private bool _IsCentrallyMaintained = false;
        /**	Logger						*/
        private VLogger log = VLogger.GetVLogger(typeof(TranslationMgr).FullName);// getClass());
        /** Context						*/
        private Ctx _ctx = null;
        /** Number of Words				*/
        private int _wordCount = 0;

        private String _ExportScope = null;
        private int _AD_Client_ID = 0;
        private String _AD_Language = null;

        internal static List<String> lstTableHasDisplayCol = new List<string>() { "AD_WINDOW", "AD_FORM", "AD_SHORTCUT" };

        //XmlDeclaration xmlDec;

        /** XML Export ID Row Attribute ID		*/
        public static String XML_ROW_ATTRIBUTE_EID = "eid";

        //Module Specific
        string _prefix = "";

        //
        bool _InsertExportID = true;


        /// <summary>
        /// Set Export Scope	
        /// </summary>
        /// <param name="exportScope">exportScope 	</param>
        /// <param name="AD_Client_ID"> only certain client if id >= 0</param>
        public void SetExportScope(String exportScope, int AD_Client_ID)
        {
            _ExportScope = exportScope;
            _AD_Client_ID = AD_Client_ID;
        }	//	setExportScope

        /* Set Prefix */
        public void SetPrefix(string prefix)
        {
            _prefix = prefix;
        }

        public void SetByExportID(bool include)
        {
            _InsertExportID = include;
        }

        /// <summary>
        ///	Get Tenant	 
        /// </summary>
        /// <returns>tenant</returns>
        public int GetAD_Client_ID()
        {
            return _AD_Client_ID;
        }

        /// <summary>
        /// You should use method validateLanguage()    
        /// </summary>
        /// <param name="language">language</param>
        public void SetAD_Language(String language)
        {
            _AD_Language = language;
        }

        /// <summary>
        /// Get Language    
        /// </summary>
        /// <returns>language</returns>
        public String GetAD_Language()
        {
            return _AD_Language;
        }	//	getAD_Language

        /// <summary>
        ///	 	Import Translation.
        //	Uses TranslationHandler to update translation
        /// </summary>
        /// <param name="directory">file</param>
        /// <param name="Trl_Table">table</param>
        /// <returns>status message</returns>
        public String ImportTrl(String directory, String Trl_Table)
        {
            String fileName = directory + "\\" + Trl_Table + "_" + _AD_Language + ".xml";
            log.Info(fileName);
            FileInfo inn = new FileInfo(fileName);
            if (!inn.Exists)
            {
                String msg = "File does not exist: " + fileName;
                log.Log(Level.SEVERE, msg);
                return msg;
            }

            int words = _wordCount;
            XmlReader parser = null;
            try
            {
                bool _isBaseLanguage = false;
                string _tableName = "";

                TranslationHandler handler = new TranslationHandler(_AD_Client_ID);
                handler.SetByExportD(_InsertExportID);

                XmlReaderSettings factory = new XmlReaderSettings();

                factory.IgnoreComments = true;
                factory.IgnoreProcessingInstructions = true;
                factory.IgnoreWhitespace = true;
                parser = XmlReader.Create(inn.FullName, factory);			//                                  
                while (parser.Read())
                {
                    try
                    {
                        switch (parser.NodeType)
                        {
                            case XmlNodeType.Element:
                                bool isEmptyElement = false;
                                isEmptyElement = parser.IsEmptyElement;
                                string strURI = parser.NamespaceURI;
                                string strName = parser.Name;
                                string strlocal = parser.LocalName;
                                List<string> att = new List<string>();
                                if (parser.HasAttributes)
                                {
                                    for (int i = 0; i < parser.AttributeCount; i++)
                                    {
                                        att.Add(parser.GetAttribute(i));
                                    }
                                }

                                if (strURI.Equals(Translation.XML_TAG) || strURI.Equals(Translation.XML_TAG_Vienna) || strURI.Equals(Translation.XML_TAG_Adam))
                                {
                                    string lang = att[0]; //attributes[Translation.XML_ATTRIBUTE_LANGUAGE].Value;
                                    _isBaseLanguage = Language.IsBaseLanguage(lang);
                                    _tableName = att[1].ToUpper(); //attributes[Translation.XML_ATTRIBUTE_TABLE].Value;
                                }



                                handler.StartElement(strURI, strlocal, strName, att);
                                if (strName == "value")
                                {
                                    if (att[1] != "")
                                    {
                                        //preventive Check skip Name column updation for some tables
                                        if (_isBaseLanguage && lstTableHasDisplayCol.Contains(_tableName))
                                        {
                                            if (att[0].ToUpper() == "NAME")
                                            {
                                                break;
                                            }
                                        }

                                        if (parser.Read())
                                        {
                                            switch (parser.NodeType)
                                            {
                                                case XmlNodeType.Text:

                                                    string trlValue = parser.Value;
                                                    if (!String.IsNullOrEmpty(trlValue))
                                                    {
                                                        char[] ch = trlValue.ToCharArray();
                                                        handler.Characters(ch);
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                }
                                break;

                            case XmlNodeType.EndElement:
                                string URI = parser.NamespaceURI;
                                string Name = parser.Name;
                                handler.EndElement(URI, "", Name);
                                break;
                        }
                    }
                    catch (Exception ec)
                    {
                        log.Info("error in importing record...Invalid Record.. in table " + Trl_Table + " error=>"+ ec.Message);
                    }
                }
                _wordCount += handler.GetWordCount();
                log.Info("Updated=" + handler.GetUpdateCount() + ", Words=" + (_wordCount - words));
                if (parser != null)
                {
                    parser.Close();
                }
                return Msg.GetMsg(_ctx, "Updated") + "=" + handler.GetUpdateCount();
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "importTrl", e);
                if (parser != null)
                {
                    parser.Close();
                }
                return e.ToString();
            }
        }	//	importTrl

        //  /// <summary>
        //  ///	 	Import Translation.
        //  //	Uses TranslationHandler to update translation
        //  /// </summary>
        //  /// <param name="directory">file</param>
        //  /// <param name="Trl_Table">table</param>
        //  /// <returns>status message</returns>
        //  public String ImportTrl(String directory, String Trl_Table)
        //  {
        //      String fileName = directory + "\\" + Trl_Table + "_" + _AD_Language + ".xml";
        //      log.Info(fileName);
        //      FileInfo inn = new FileInfo(fileName);
        //      if (!inn.Exists)
        //      {
        //          String msg = "File does not exist: " + fileName;
        //          log.Log(Level.SEVERE, msg);
        //          return msg;
        //      }

        //      int words = _wordCount;
        //      try
        //      {
        //          bool _isBaseLanguage = false;
        //          string _tableName = "";

        //          TranslationHandler handler = new TranslationHandler(_AD_Client_ID);
        //          handler.SetByExportD(_InsertExportID);
        //          using (XmlReader reader = XmlReader.Create(fileName))
        //          {
        //              while (reader.Read())
        //              {
        //switch (reader.NodeType)
        //              {
        //                  case XmlNodeType.Element:
        //                      bool isEmptyElement = false;
        //                      isEmptyElement = reader.IsEmptyElement;
        //                      string strURI = reader.NamespaceURI;
        //                      string strName = reader.Name;
        //                      string strlocal = reader.LocalName;
        //                      List<string> att = new List<string>();
        //                  if (reader.HasAttributes)
        //                      {
        //                          for (int i = 0; i < reader.AttributeCount; i++)
        //                          {
        //                              att.Add(reader.GetAttribute(i));
        //                          }
        //                      }
        //        if (strURI.Equals(Translation.XML_TAG) || strURI.Equals(Translation.XML_TAG_Vienna) || strURI.Equals(Translation.XML_TAG_Adam))
        //        {
        //            string lang = att[0]; //attributes[Translation.XML_ATTRIBUTE_LANGUAGE].Value;
        //            _isBaseLanguage = Language.IsBaseLanguage(lang);
        //            _tableName = att[1].ToUpper(); //attributes[Translation.XML_ATTRIBUTE_TABLE].Value;
        //        }
        //        handler.StartElement(strURI, strlocal, strName, att);

        //              }
        //          }

        //          //XmlReaderSettings factory = new XmlReaderSettings();

        //          //factory.IgnoreComments = true;
        //          //factory.IgnoreProcessingInstructions = true;
        //          //factory.IgnoreWhitespace = true;
        //          //XmlReader parser = XmlReader.Create(inn.FullName, factory);			//                                  
        //          //while (parser.Read())
        //          //{
        //          //    switch (parser.NodeType)
        //          //    {
        //          //        case XmlNodeType.Element:
        //          //            bool isEmptyElement = false;
        //          //            isEmptyElement = parser.IsEmptyElement;
        //          //            string strURI = parser.NamespaceURI;
        //          //            string strName = parser.Name;
        //          //            string strlocal = parser.LocalName;
        //          //            List<string> att = new List<string>();
        //          //            if (parser.HasAttributes)
        //          //            {
        //          //                for (int i = 0; i < parser.AttributeCount; i++)
        //          //                {
        //          //                    att.Add(parser.GetAttribute(i));
        //          //                }
        //          //            }

        //          //            if (strURI.Equals(Translation.XML_TAG) || strURI.Equals(Translation.XML_TAG_Vienna) || strURI.Equals(Translation.XML_TAG_Adam))
        //          //            {
        //          //                string lang = att[0]; //attributes[Translation.XML_ATTRIBUTE_LANGUAGE].Value;
        //          //                _isBaseLanguage = Language.IsBaseLanguage(lang);
        //          //                _tableName = att[1].ToUpper(); //attributes[Translation.XML_ATTRIBUTE_TABLE].Value;
        //          //            }



        //          //            handler.StartElement(strURI, strlocal, strName, att);
        //          //            if (strName == "value")
        //          //            {
        //          //                if (att[1] != "")
        //          //                {
        //          //                    //preventive Check skip Name column updation for some tables
        //          //                    if (_isBaseLanguage && lstTableHasDisplayCol.Contains(_tableName))
        //          //                    {
        //          //                        if (att[0].ToUpper() == "NAME")
        //          //                        {
        //          //                            break;
        //          //                        }
        //          //                    }

        //          //                    if (parser.Read())
        //          //                    {
        //          //                        switch (parser.NodeType)
        //          //                        {
        //          //                            case XmlNodeType.Text:

        //          //                                string trlValue = parser.Value;
        //          //                                if (!String.IsNullOrEmpty(trlValue))
        //          //                                {
        //          //                                    char[] ch = trlValue.ToCharArray();
        //          //                                    handler.Characters(ch);
        //          //                                }
        //          //                                break;
        //          //                        }
        //          //                    }
        //          //                }
        //          //            }
        //          //            break;

        //          //        case XmlNodeType.EndElement:
        //          //            string URI = parser.NamespaceURI;
        //          //            string Name = parser.Name;
        //          //            handler.EndElement(URI, "", Name);
        //          //            break;
        //          //    }
        //          //}
        //          _wordCount += handler.GetWordCount();
        //          log.Info("Updated=" + handler.GetUpdateCount() + ", Words=" + (_wordCount - words));
        //          return Msg.GetMsg(_ctx, "Updated") + "=" + handler.GetUpdateCount();
        //      }
        //      catch (Exception e)
        //      {
        //          log.Log(Level.SEVERE, "importTrl", e);
        //          return e.ToString();
        //      }
        //  }	//	importTrl


        /// <summary>
        ///	Import Translation	
        /// </summary>
        /// <param name="directory">file directory</param>
        /// <param name="trlTableName">table trl</param>
        /// <param name="translationLevel">translation level TranslationImportExport.TranslationLevel</param>
        /// <returns>status message</returns>
        public String ExportTrl(String directory, String trlTableName,
            String translationLevel)
        {
            if (translationLevel == null)
                translationLevel = TranslationImportExport.TranslationLevel_All;

            String fileName = directory + "\\" + trlTableName + "_" + _AD_Language + ".xml";
            log.Info(fileName);
            FileInfo ut = new FileInfo(fileName);

            String info = "-";
            int words = _wordCount;
            bool isBaseLanguage = Language.IsBaseLanguage(_AD_Language);
            String tableName = trlTableName;
            int pos = tableName.IndexOf("_Trl");
            String baseTableName = trlTableName.Substring(0, pos);
            if (isBaseLanguage)
                tableName = baseTableName;
            String keyColumn = baseTableName + "_ID";
            String[] trlColumns = GetTrlColumns(baseTableName);
            //
            StringBuilder sql = null;
            IDataReader idr = null;
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlNode root = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", "no");
                xmlDoc.AppendChild(root);
                XmlComment xmlcmnt = xmlDoc.CreateComment(DTD);
                xmlDoc.AppendChild(xmlcmnt);
                XmlElement document = xmlDoc.CreateElement(XML_TAG_Vienna);
                document.SetAttribute(XML_ATTRIBUTE_LANGUAGE, _AD_Language);
                document.SetAttribute(XML_ATTRIBUTE_TABLE, baseTableName);// Base_Table);
                xmlDoc.AppendChild(document);
                sql = new StringBuilder("SELECT ");
                if (isBaseLanguage)
                    sql.Append("'Y',");							//	1
                else
                    sql.Append("t.IsTranslated,");
                sql.Append("t.").Append(keyColumn);				//	2	

                if (_InsertExportID)
                {
                    sql.Append(",o.Export_ID");                  //3
                }

                for (int i = 0; i < trlColumns.Length; i++)
                    sql.Append(", t.").Append(trlColumns[i])
                        .Append(",o.").Append(trlColumns[i]).Append(" AS ").Append(trlColumns[i]).Append("O");
                //
                sql.Append(" FROM ").Append(tableName).Append(" t")
                    .Append(" INNER JOIN ").Append(baseTableName)
                    .Append(" o ON (t.").Append(keyColumn).Append("=o.").Append(keyColumn).Append(")");
                bool haveWhere = false;
                if (!isBaseLanguage)
                {
                    sql.Append(" WHERE t.AD_Language=@param");
                    haveWhere = true;
                }
                if (_IsCentrallyMaintained)
                {
                    sql.Append(haveWhere ? " AND " : " WHERE ").Append("o.IsCentrallyMaintained='N'");
                    haveWhere = true;
                }

                if (!string.IsNullOrEmpty(_prefix))
                {
                    sql.Append(haveWhere ? " AND " : " WHERE ").Append("o.Export_ID LIKE '").Append(_prefix).Append("%'");
                    haveWhere = true;
                }

                if (_AD_Client_ID >= 0)
                {
                    sql.Append(haveWhere ? " AND " : " WHERE ").Append("o.AD_Client_ID=").Append(_AD_Client_ID);
                    haveWhere = true;
                }
                String scopeSQL = GetScopeSQL(baseTableName);
                if (!Utility.Util.IsEmpty(scopeSQL))
                    sql.Append(haveWhere ? " AND " : " WHERE ").Append(scopeSQL);
                sql.Append(" ORDER BY t.").Append(keyColumn);
              
                SqlParameter[] param = null;
                if (!isBaseLanguage)
                {
                    param = new SqlParameter[1];
                    param[0] = new SqlParameter("@param", _AD_Language);
                }
                idr = DataBase.DB.ExecuteReader(sql.ToString(), param, null);
                int rows = 0;
                while (idr.Read())
                {
                    XmlElement row = xmlDoc.CreateElement(XML_ROW_TAG);
                    row.SetAttribute(XML_ROW_ATTRIBUTE_ID, Utility.Util.GetValueOfInt(idr[1]).ToString());
                    row.SetAttribute(XML_ROW_ATTRIBUTE_TRANSLATED, Utility.Util.GetValueOfString(idr[0]));
                    if (_InsertExportID)
                    {
                        row.SetAttribute(XML_ROW_ATTRIBUTE_EID, Util.GetValueOfString(idr[2]));
                    }
                    foreach (String element in trlColumns)
                    {
                        if (!baseTableName.Equals("AD_PrintFormatItem"))
                        {
                            if (translationLevel.Equals(TranslationImportExport.TranslationLevel_LabelOnly)
                                && element.IndexOf("Name") == -1)
                                continue;
                            if (translationLevel.Equals(TranslationImportExport.TranslationLevel_LabelDescriptionOnly)
                                && !(element.Equals("Name") || element.Equals("Description")))
                                continue;
                        }


                        XmlElement value = xmlDoc.CreateElement(XML_VALUE_TAG);
                        value.SetAttribute(XML_VALUE_ATTRIBUTE_COLUMN, element);
                        String origString = Utility.Util.GetValueOfString(idr[element + "O"]);			//	Original Value
                        if (origString == null)
                            origString = "";
                        String valueString = Utility.Util.GetValueOfString(idr[element]);				//	Value
                        if (valueString == null)
                            valueString = "";
                        value.SetAttribute(XML_VALUE_ATTRIBUTE_ORIGINAL, origString);
                        value.AppendChild(xmlDoc.CreateTextNode(valueString));
                        row.AppendChild(value);
                        _wordCount += Utility.Util.CountWords(origString);
                    }
                    document.AppendChild(row);
                    rows++;
                }
                idr.Close();
                info = trlTableName + ": Records=" + rows + ", Words=" + (_wordCount - words);
                if (document.GetType() != null)
                    info += ", DTD=" + document.GetType();
                log.Info(info);
                xmlDoc.Save(fileName);
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql.ToString(), e);
                info = e.Message.ToString();
            }
            return info;
        }	//	exportTrl


        /// <summary>
        ///	Import Translation	
        /// </summary>
        /// <param name="directory">file directory</param>
        /// <param name="trlTableName">table trl</param>
        /// <param name="translationLevel">translation level TranslationImportExport.TranslationLevel</param>
        /// <returns>status message</returns>
        /// By Sukhwinder on 23 June, 2017 for creating translations on the basis of modules.
        public String ExportTrl(String directory, String trlTableName,
            String translationLevel, String prefix)
        {
            _prefix = prefix;

            if (translationLevel == null)
                translationLevel = TranslationImportExport.TranslationLevel_All;

            String fileName = directory + "\\" + trlTableName + "_" + _AD_Language + ".xml";
            log.Info(fileName);
            FileInfo ut = new FileInfo(fileName);

            String info = "-";
            int words = _wordCount;
            bool isBaseLanguage = Language.IsBaseLanguage(_AD_Language);
            String tableName = trlTableName;
            int pos = tableName.IndexOf("_Trl");
            String baseTableName = trlTableName.Substring(0, pos);
            if (isBaseLanguage)
                tableName = baseTableName;
            String keyColumn = baseTableName + "_ID";
            String[] trlColumns = GetTrlColumns(baseTableName);
            //
            StringBuilder sql = null;
            IDataReader idr = null;
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlNode root = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", "no");
                xmlDoc.AppendChild(root);
                XmlComment xmlcmnt = xmlDoc.CreateComment(DTD);
                xmlDoc.AppendChild(xmlcmnt);
                XmlElement document = xmlDoc.CreateElement(XML_TAG_Vienna);
                document.SetAttribute(XML_ATTRIBUTE_LANGUAGE, _AD_Language);
                document.SetAttribute(XML_ATTRIBUTE_TABLE, baseTableName);// Base_Table);
                xmlDoc.AppendChild(document);
                sql = new StringBuilder("SELECT ");
                if (isBaseLanguage)
                    sql.Append("'Y',");							//	1
                else
                    sql.Append("t.IsTranslated,");
                sql.Append("t.").Append(keyColumn);				//	2	

                if (_InsertExportID)
                {
                    sql.Append(",o.Export_ID");                  //3
                }

                for (int i = 0; i < trlColumns.Length; i++)
                    sql.Append(", t.").Append(trlColumns[i])
                        .Append(",o.").Append(trlColumns[i]).Append(" AS ").Append(trlColumns[i]).Append("O");
                //
                sql.Append(" FROM ").Append(tableName).Append(" t")
                    .Append(" INNER JOIN ").Append(baseTableName)
                    .Append(" o ON (t.").Append(keyColumn).Append("=o.").Append(keyColumn).Append(")");
                bool haveWhere = false;
                if (!isBaseLanguage)
                {
                    sql.Append(" WHERE t.AD_Language=@param");
                    haveWhere = true;
                }
                if (_IsCentrallyMaintained)
                {
                    sql.Append(haveWhere ? " AND " : " WHERE ").Append("o.IsCentrallyMaintained='N'");
                    haveWhere = true;
                }

                if (!string.IsNullOrEmpty(_prefix))
                {
                    sql.Append(haveWhere ? " AND " : " WHERE ").Append("o.Export_ID LIKE '").Append(_prefix).Append("%'");
                    haveWhere = true;
                }

                if (_AD_Client_ID >= 0)
                {
                    sql.Append(haveWhere ? " AND " : " WHERE ").Append("o.AD_Client_ID=").Append(_AD_Client_ID);
                    haveWhere = true;
                }
                String scopeSQL = GetScopeSQL(baseTableName);
                if (!Utility.Util.IsEmpty(scopeSQL))
                    sql.Append(haveWhere ? " AND " : " WHERE ").Append(scopeSQL);
                sql.Append(" ORDER BY t.").Append(keyColumn);

                SqlParameter[] param = null;
                if (!isBaseLanguage)
                {
                    param = new SqlParameter[1];
                    param[0] = new SqlParameter("@param", _AD_Language);
                }
                idr = DataBase.DB.ExecuteReader(sql.ToString(), param, null);
                int rows = 0;
                while (idr.Read())
                {
                    XmlElement row = xmlDoc.CreateElement(XML_ROW_TAG);
                    row.SetAttribute(XML_ROW_ATTRIBUTE_ID, Utility.Util.GetValueOfInt(idr[1]).ToString());
                    row.SetAttribute(XML_ROW_ATTRIBUTE_TRANSLATED, Utility.Util.GetValueOfString(idr[0]));
                    if (_InsertExportID)
                    {
                        row.SetAttribute(XML_ROW_ATTRIBUTE_EID, Util.GetValueOfString(idr[2]));
                    }
                    foreach (String element in trlColumns)
                    {
                        if (!baseTableName.Equals("AD_PrintFormatItem"))
                        {
                            if (translationLevel.Equals(TranslationImportExport.TranslationLevel_LabelOnly)
                                && element.IndexOf("Name") == -1)
                                continue;
                            if (translationLevel.Equals(TranslationImportExport.TranslationLevel_LabelDescriptionOnly)
                                && !(element.Equals("Name") || element.Equals("Description")))
                                continue;
                        }


                        XmlElement value = xmlDoc.CreateElement(XML_VALUE_TAG);
                        value.SetAttribute(XML_VALUE_ATTRIBUTE_COLUMN, element);
                        String origString = Utility.Util.GetValueOfString(idr[element + "O"]);			//	Original Value
                        if (origString == null)
                            origString = "";
                        String valueString = Utility.Util.GetValueOfString(idr[element]);				//	Value
                        if (valueString == null)
                            valueString = "";
                        value.SetAttribute(XML_VALUE_ATTRIBUTE_ORIGINAL, origString);
                        value.AppendChild(xmlDoc.CreateTextNode(valueString));
                        row.AppendChild(value);
                        _wordCount += Utility.Util.CountWords(origString);
                    }
                    document.AppendChild(row);
                    rows++;
                }
                idr.Close();
                info = trlTableName + ": Records=" + rows + ", Words=" + (_wordCount - words);
                if (document.GetType() != null)
                    info += ", DTD=" + document.GetType();
                log.Info(info);
                xmlDoc.Save(fileName);
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql.ToString(), e);
                info = e.Message.ToString();
            }
            return info;
        }	//	exportTrl


        /// <summary>
        /// Get Columns for Table	 
        /// </summary>
        /// <param name="Base_Table">table</param>
        /// <returns>array of translated columns</returns>
        private String[] GetTrlColumns(String Base_Table)
        {
            _IsCentrallyMaintained = false;
            String sql = "SELECT TableName FROM AD_Table t"
                + " INNER JOIN AD_Column c ON (c.AD_Table_ID=t.AD_Table_ID AND c.ColumnName='IsCentrallyMaintained') "
                + "WHERE t.TableName=@param AND c.IsActive='Y'";
            SqlParameter[] param = new SqlParameter[1];
            IDataReader idr = null;
            try
            {
                param[0] = new SqlParameter("@param", Base_Table);
                idr = DataBase.DB.ExecuteReader(sql, param, null);
                if (idr.Read())
                    _IsCentrallyMaintained = true;
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }


            bool isBaseLanguage = Language.IsBaseLanguage(_AD_Language);


            sql = "SELECT ColumnName "
                + "FROM AD_Column c"
                + " INNER JOIN AD_Table t ON (c.AD_Table_ID=t.AD_Table_ID) "
                + "WHERE t.TableName=@param"
                + " AND c.AD_Reference_ID IN (10,14) "
                  + " AND c.ColumnName <> 'Export_ID' "
                + "ORDER BY IsMandatory DESC, ColumnName";
            List<String> list = new List<String>();
            try
            {
                param[0] = new SqlParameter("@param", Base_Table + "_Trl");
                idr = DataBase.DB.ExecuteReader(sql, param, null);
                while (idr.Read())
                {
                    String s = Utility.Util.GetValueOfString(idr[0]);// rs.getString(1);		
                    if (isBaseLanguage && lstTableHasDisplayCol.Contains(Base_Table.ToUpper()))
                    {
                        if (s.Equals("Name", StringComparison.OrdinalIgnoreCase))
                        {
                            s = "DisplayName";
                        }
                    }
                    list.Add(s);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }

            //	Convert to Array
            String[] retValue = new String[list.Count];
            retValue = list.ToArray();
            return retValue;
        }	//	getTrlColumns

        /// <summary>
        /// Reduce Scope to tenant users	
        /// </summary>
        /// <param name="baseTableName">table name</param>
        /// <returns>string</returns>
        private String GetScopeSQL(String baseTableName)
        {
            if (Utility.Util.IsEmpty(baseTableName)
                || _ExportScope == null
                || !_ExportScope.Equals(TranslationImportExport.ExportScope_SystemUser))
                return null;
            //	Not translated
            if (baseTableName.Equals("AD_Table"))
                return "1=2";
            //	AccessLevel 4=System only
            if (baseTableName.Equals("AD_Window"))
                return "o.AD_Window_ID IN (SELECT t.AD_Window_ID FROM AD_Tab tab"
                    + " INNER JOIN AD_Table tt ON (tab.AD_Table_ID=tt.AD_Table_ID) "
                    + "WHERE tt.AccessLevel <> '4')";
            if (baseTableName.Equals("AD_Tab"))
                return "EXISTS (SELECT * FROM AD_Table tt "
                    + "WHERE o.AD_Table_ID=tt.AD_Table_ID AND tt.AccessLevel <> '4')";
            if (baseTableName.Equals("AD_Field"))
                return "o.AD_Tab_ID IN (SELECT AD_Tab_ID FROM AD_Tab tab"
                    + " INNER JOIN AD_Table tt ON (tab.AD_Table_ID=tt.AD_Table_ID) "
                    + "WHERE tt.AccessLevel <> '4')";
            if (baseTableName.Equals("AD_Element"))
                return "o.AD_Element_ID IN (SELECT AD_Element_ID FROM AD_Column c"
                    + " INNER JOIN AD_Table tt ON (c.AD_Table_ID=tt.AD_Table_ID) "
                    + "WHERE tt.AccessLevel <> '4')";
            if (baseTableName.Equals("AD_Process"))
                return "o.AccessLevel <> '4'";
            if (baseTableName.Equals("AD_Process_Para"))
                return "o.AD_Process_ID IN (SELECT AD_Process_ID FROM AD_Process WHERE AccessLevel<>'4')";

            return null;
        }	//	getScopeSQL

        /// <summary>
        ///	Validate Language.
        //Check if AD_Language record exists
        // Check Trl table records	 
        /// </summary>
        /// <param name="AD_Language">language</param>
        /// <returns> "" if validated - or error message</returns>
        public String ValidateLanguage(String AD_Language)
        {
            String sql = "SELECT * "
                + "FROM AD_Language "
                + "WHERE AD_Language=@param";
            MLanguage language = null;
            IDataReader idr = null;
            SqlParameter[] param = new SqlParameter[1];
            try
            {
                param[0] = new SqlParameter("@param", AD_Language);
                idr = DataBase.DB.ExecuteReader(sql, param, null);
                if (idr.Read())
                    language = new MLanguage(_ctx, idr, null);
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
                return e.ToString();
            }

            //	No AD_Language Record
            if (language == null)
            {
                log.Log(Level.SEVERE, "Language does not exist: " + AD_Language);
                return "Language does not exist: " + AD_Language;
            }
            //	Language exists
            SetAD_Language(AD_Language);
            if (language.IsActive())
            {
                if (language.IsBaseLanguage())
                    return "";
            }




            else
            {
                log.Log(Level.SEVERE, "Language not active or not system language: " + AD_Language);
                return "Language not active or not system language: " + AD_Language;
            }

            //	Validate Translation
            log.Info("Start Validating ... " + language);
            language.Maintain(true);
            return "";
        }	//	validateLanguage


        public String ValidateLanguage(String AD_Language, bool forceActiveSysLang) //Market special
        {
            String sql = "SELECT * "
                + "FROM AD_Language "
                + "WHERE AD_Language=@param";
            MLanguage language = null;
            IDataReader idr = null;
            SqlParameter[] param = new SqlParameter[1];
            try
            {
                param[0] = new SqlParameter("@param", AD_Language);
                idr = DataBase.DB.ExecuteReader(sql, param, null);
                if (idr.Read())
                    language = new MLanguage(_ctx, idr, null);
                idr.Close();
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
                return e.ToString();
            }

            //	No AD_Language Record
            if (language == null)
            {
                log.Log(Level.SEVERE, "Language does not exist: " + AD_Language);
                return "Language does not exist: " + AD_Language;
            }
            //	Language exists
            SetAD_Language(AD_Language);
            if (language.IsActive())
            {
                if (language.IsBaseLanguage())
                    return "";
            }

            if (forceActiveSysLang )
            {
                if ((!language.IsActive() || !language.IsSystemLanguage()))
                {
                    language.SetIsActive(true);
                    language.SetIsSystemLanguage(true);
                    if (!language.Save())
                    {
                        log.Log(Level.SEVERE, "Save[active-SysLang] Error  " + AD_Language);
                        return "Language not active or not system language[Save Error]: " + AD_Language;
                    }
                }
            }
            else 
            {
                if ((!language.IsActive() || !language.IsSystemLanguage()))
                {
                    log.Log(Level.SEVERE, "Language not active or not system language: " + AD_Language);
                    return "Language not active or not system language: " + AD_Language;
                }
            }
            //	Validate Translation
            log.Info("Start Validating ... " + language);
            language.Maintain(true);
            return "";
        }	//	validateLanguage



        /// <summary>
        ///	Get Number of Words	
        /// </summary>
        /// <returns>number of words exported</returns>
        public int GetWordCount()
        {
            return _wordCount;
        }	//	getWordCount

        /// <summary>
        /// Process	
        /// </summary>
        /// <param name="directory">directory</param>
        /// <param name="mode">mode</param>
        private void Process(String directory, String mode)
        {
            DirectoryInfo dir = new DirectoryInfo(directory);
            if (!dir.Exists)
                dir.Create();
            dir = new DirectoryInfo(directory);
            if (!dir.Exists)
            {
                return;
            }

            String sql = "SELECT Name, TableName "
                + "FROM AD_Table "
                + "WHERE TableName LIKE '%_Trl' "
                + "ORDER BY 1";
            List<String> trlTables = new List<String>();
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql);
                while (idr.Read())
                    trlTables.Add(Utility.Util.GetValueOfString(idr[1]));// rs.getString(2));
                idr.Close();
            }
            catch (Exception e)
            {
                idr.Close();
                log.Log(Level.SEVERE, sql, e);
            }

            for (int i = 0; i < trlTables.Count; i++)
            {
                String table = trlTables[i];//.get(i);
                if (mode.StartsWith("i"))
                    ImportTrl(directory, table);
                else
                    ExportTrl(directory, table, null);
            }
        }
    }
}
