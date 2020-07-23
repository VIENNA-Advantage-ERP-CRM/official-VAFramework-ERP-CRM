using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.Utility;
using VAdvantage.Logging;
using System.IO;
using VAdvantage.Login;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Process;
using System.Xml;
namespace VAdvantage.Install
{
   public class Translation
   {
	/// <summary>
	/// Translation	 
	/// </summary>
	/// <param name="ctx">context</param>
	public Translation (Ctx ctx)
	{
		_ctx = ctx;
	}	//	Translation
	
	/**	DTD						*/
    public static String DTD = "<!DOCTYPE ViennaTrl PUBLIC \"-//Vienna, Inc.//DTD Vienna Translation 1.0//EN\" \"http://www.Viennasolutions.com\">";
	/**	XML Element Tag			*/
	public static  String	XML_TAG = "compiereTrl";

    public static String XML_TAG_Vienna = "ViennaTrl";

    public static String XML_TAG_Adam = "adempiereTrl";
	/**	XML Attribute Table			*/
	public static  String	XML_ATTRIBUTE_TABLE = "table";
	/** XML Attribute Language		*/
	public static  String	XML_ATTRIBUTE_LANGUAGE = "language";

	/**	XML Row Tag					*/
	public static  String	XML_ROW_TAG = "row";
	/** XML Row Attribute ID		*/
	public static  String	XML_ROW_ATTRIBUTE_ID = "id";
	/** XML Row Attribute Translated	*/
	public static String	XML_ROW_ATTRIBUTE_TRANSLATED = "trl";

	/**	XML Value Tag				*/
	public static  String	XML_VALUE_TAG = "value";
	/** XML Value Column			*/
	public static  String	XML_VALUE_ATTRIBUTE_COLUMN = "column";
	/** XML Value Original			*/
	public static  String	XML_VALUE_ATTRIBUTE_ORIGINAL = "original";

	/**	Table is centrally maintained	*/
	private bool		_IsCentrallyMaintained = false;
	/**	Logger						*/
	private VLogger			log = VLogger.GetVLogger(typeof(Translation).FullName);// getClass());
	/** Context						*/
	private Ctx				_ctx = null;

	/// <summary>
    /// 	Import Translation.
      	    //Uses TranslationHandler to update translation         
	/// </summary>
	/// <param name="directory">file Directory</param>
	/// <param name="AD_Client_ID">only certain client if id >= 0</param>
	/// <param name="AD_Language">language</param>
	/// <param name="Trl_Table">table</param>
	/// <returns>status Message</returns>
	public String ImportTrl (String directory, int AD_Client_ID, String AD_Language, String Trl_Table)
	{		
        String fileName = directory + "\\" + Trl_Table + "_" + AD_Language + ".xml";
		log.Info(fileName);	
        FileInfo inn=new FileInfo(fileName);
		if (!inn.Exists)
		{
			String msg = "File does not exist: " + fileName;
			log.Log(Level.SEVERE, msg);
			return msg;
		}
       
		try
		{
			TranslationHandler handler = new TranslationHandler(AD_Client_ID);
			//SAXParserFactory factory = SAXParserFactory.newInstance();
		//	factory.setValidating(true);
            XmlReaderSettings factory = new XmlReaderSettings();
		//SAXParser parser = factory.newSAXParser();                  
			//parser.parse(in, handler);
            factory.IgnoreComments = true;
            factory.IgnoreProcessingInstructions = true;
            factory.IgnoreWhitespace = true;
            XmlReader parser = XmlReader.Create(inn.FullName, factory);
            while (parser.Read())
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
                        handler.StartElement(strURI, strlocal, strName, att);
                        if (strName == "value")
                        {
                            if (att[1] != "")
                            {
                                char[] ch = att[1].ToCharArray();
                                handler.Characters(ch);
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


			log.Info("Updated=" + handler.GetUpdateCount());
			return Msg.GetMsg(_ctx, "Updated") + "=" + handler.GetUpdateCount();
		}
		catch (Exception e)
		{
			log.Log(Level.SEVERE, "importTrl", e);
			return e.ToString();
		}
	}	//	importTrl

	
	/// <summary>
	/// Import Translation
	/// </summary>
	/// <param name="directory">file directory</param>
	/// <param name="AD_Client_ID">only certain client if id >= 0</param>
	/// <param name="AD_Language">language</param>
	/// <param name="Trl_Table">table</param>
	/// <returns>stauts Message </returns>
    public String ExportTrl(String directory, int AD_Client_ID, String AD_Language, String Trl_Table)
    {
        String fileName = directory + "\\" + Trl_Table + "_" + AD_Language + ".xml";
        log.Info(fileName);
        FileInfo ut = new FileInfo(fileName);

        bool isBaseLanguage = Language.IsBaseLanguage(AD_Language);
        String tableName = Trl_Table;
        int pos = tableName.IndexOf("_Trl");
        String Base_Table = Trl_Table.Substring(0, pos);
        if (isBaseLanguage)
            tableName =  Base_Table;
        String keyColumn = Base_Table + "_ID";
        String[] trlColumns = GetTrlColumns(Base_Table);
        //
        StringBuilder sql = null;
        try
        {
            XmlDocument xmlDoc = new XmlDocument();

            XmlNode root = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", "no");// CreateNode(XmlNodeType.XmlDeclaration, "", "");// .DocumentElement;
            xmlDoc.AppendChild(root);
            XmlComment xmlcmnt = xmlDoc.CreateComment(DTD);
            xmlDoc.AppendChild(xmlcmnt);   
            XmlElement document = xmlDoc.CreateElement(XML_TAG_Vienna);
            document.SetAttribute(XML_ATTRIBUTE_LANGUAGE, AD_Language);
            document.SetAttribute(XML_ATTRIBUTE_TABLE, Base_Table);
            xmlDoc.AppendChild(document);
           // xmlDoc.CreateComment(DTD);
       
            //
            sql = new StringBuilder("SELECT ");
            if (isBaseLanguage)
                sql.Append("'Y',");							//	1
            else
                sql.Append("t.IsTranslated,");
            sql.Append("t.").Append(keyColumn);				//	2
            //
            for (int i = 0; i < trlColumns.Length; i++)
                sql.Append(", t.").Append(trlColumns[i])
                    .Append(",o.").Append(trlColumns[i]).Append(" AS ").Append(trlColumns[i]).Append("O");
            //
            sql.Append(" FROM ").Append(tableName).Append(" t")
                .Append(" INNER JOIN ").Append(Base_Table)
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
            if (AD_Client_ID >= 0)
                sql.Append(haveWhere ? " AND " : " WHERE ").Append("o.AD_Client_ID=").Append(AD_Client_ID);
            sql.Append(" ORDER BY t.").Append(keyColumn);

            SqlParameter[] param = null;
            IDataReader idr = null;
            if (!isBaseLanguage)
            {
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@param", AD_Language);
            }
            idr = DataBase.DB.ExecuteReader(sql.ToString(), param, null);
            int rows = 0;
            while (idr.Read())
            {
                XmlElement row = xmlDoc.CreateElement(XML_ROW_TAG);
                row.SetAttribute(XML_ROW_ATTRIBUTE_ID, Utility.Util.GetValueOfInt(idr[1]).ToString());
                row.SetAttribute(XML_ROW_ATTRIBUTE_TRANSLATED, Utility.Util.GetValueOfString(idr[0]));
                for (int i = 0; i < trlColumns.Length; i++)
                {
                    XmlElement value = xmlDoc.CreateElement(XML_VALUE_TAG);
                    value.SetAttribute(XML_VALUE_ATTRIBUTE_COLUMN, trlColumns[i]);
                    String origString = Utility.Util.GetValueOfString(idr[(trlColumns[i] + "O")]);			//	Original Value
                    if (origString == null)
                        origString = "";
                    String valueString = Utility.Util.GetValueOfString(idr[trlColumns[i]]);				//	Value
                    if (valueString == null)
                        valueString = "";
                    value.SetAttribute(XML_VALUE_ATTRIBUTE_ORIGINAL, origString);
                    value.AppendChild(xmlDoc.CreateTextNode(valueString));
                    row.AppendChild(value);
                }
                document.AppendChild(row);
                rows++;
            }
            idr.Close();
            log.Info("Records=" + rows
                + ", DTD=" + document.GetType()
                + " - " + Trl_Table);
            xmlDoc.Save(fileName);
        }
        catch (Exception e)
        {
            log.Log(Level.SEVERE, "", e);
        }

        return "";
    }	//	exportTrl

	
	/// <summary>
	///	Get Columns for Table
	 	/// </summary>
	/// <param name="Base_Table">table</param>
    /// <returns>array of translated columns</returns>
	private String[] GetTrlColumns (String Base_Table)
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
            idr.Close();
			log.Log(Level.SEVERE, sql, e);
		}

		sql = "SELECT ColumnName "
			+ "FROM AD_Column c"
			+ " INNER JOIN AD_Table t ON (c.AD_Table_ID=t.AD_Table_ID) "
			+ " WHERE t.TableName=@param"
			+ " AND c.AD_Reference_ID IN (10,14) "
            +" AND c.ColumnName <> 'Export_ID' "
			+ "ORDER BY IsMandatory DESC, ColumnName";
		List<String> list = new List<String>();
		try
		{			
            param[0] = new SqlParameter("@param", Base_Table + "_Trl");
            idr = DataBase.DB.ExecuteReader(sql, param, null);
			while (idr.Read())
			{
                String s = Utility.Util.GetValueOfString(idr[0]);// rs.getString(1);		
				list.Add(s);
			}
            idr.Close();
		}
		catch (Exception e)
		{
            idr.Close();
			log.Log(Level.SEVERE, sql, e);
		}

		//	Convert to Array
		String[] retValue = new String[list.Count];
        retValue=list.ToArray();
		return retValue;
	}	//	getTrlColumns

	
	/// <summary>
	/// Validate Language.	 * 	
	 //  - Check if AD_Language record exists
	 // - Check Trl table records	 
	/// </summary>
	/// <param name="AD_Language">language</param>
	/// <returns>if validated - or error message</returns>
	public String ValidateLanguage (String AD_Language)
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
				language = new MLanguage (_ctx,idr, null);
            idr.Close();
		}
		catch (Exception e)
		{
            idr.Close();
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

	
	/// <summary>
	///process
	 /// </summary>
	/// <param name="directory">directory</param>
	/// <param name="AD_Language">language</param>
	/// <param name="mode">mode</param>
	private void Process (String directory, String AD_Language, String mode)
	{
		DirectoryInfo dir=new DirectoryInfo(directory);	
        if(!dir.Exists)
        {			
            dir.Create();
        }
		dir = new DirectoryInfo(directory);
		if (!dir.Exists)
		{
		    return;
		}
		String 	sql = "SELECT Name, TableName "
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
            String table = (String)trlTables[i];
            if (mode.StartsWith("i"))
                ImportTrl(directory, -1, AD_Language, table);
            else
            { }
				//ExportTrl(directory, -1, AD_Language, table);
		}
	}		
	}	
}
