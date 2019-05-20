/********************************************************
 * Module Name    : Framework
 * Purpose        : Generates x classes
 * Chronological Development
 * Jagmohan Bhatt : 11-feb-2009
 * Raghunandan      29-April-2009  change in CreateColumnMethods() line no-502 to handle null value 
 ******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Model;
using System.IO;
using VAdvantage.SqlExec;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;
using System.Text.RegularExpressions;

namespace VAdvantage.Tool
{
    /// <summary>
    /// Generates the x classes
    /// </summary>
    public class GenerateModel
    {
        private long _createdMS = CurrentTimeMillis();
        static private StringBuilder sbTextCopy1 = new StringBuilder();
        static private string fileName1 = string.Empty;
        public GenerateModel()
        {
        }
        /// <summary>
        /// return time in mili seconds
        /// </summary>
        /// <returns></returns>
        public static long CurrentTimeMillis()
        {
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            return (long)ts.TotalMilliseconds;
        }

        /**	Logger			*/
        private static VLogger log = VLogger.GetVLogger(typeof(GenerateModel).FullName);


        /// <summary>+
        /// GenerateModel
        /// </summary>
        /// <param name="AD_Table_ID">table id</param>
        /// <param name="directory">directory name</param>
        /// <param name="namespaceName">namespace name</param>

        public GenerateModel(int AD_Table_ID, String directory, String namespaceName)
        {
            StringBuilder mandatory = new StringBuilder("");
            StringBuilder sbText = CreateColumns(AD_Table_ID, mandatory);
            String tableName = CreateHeader(AD_Table_ID, sbText, mandatory, namespaceName);
            //WriteToFile(sbText, directory + "\\"+tableName + ".cs");
            sbTextCopy1 = sbText;
            fileName1 = tableName + ".cs";
        }

        /// <summary>
        /// Creates Column
        /// </summary>
        /// <param name="AD_Table_ID">table id</param>
        /// <param name="mandatory">mandatory string</param>
        /// <returns>created columns in string</returns>
        private StringBuilder CreateColumns(int AD_Table_ID, StringBuilder mandatory)
        {

            StringBuilder sb = new StringBuilder();
            String sql = "SELECT c.ColumnName, c.IsUpdateable, c.IsMandatory,"		//	1..3
                + " c.AD_Reference_ID, c.AD_Reference_Value_ID, DefaultValue, SeqNo, "	//	4..7
                + " c.FieldLength, c.valueMin, c.valueMax, c.vFormat, c.callOut, "	//	8..12
                + " c.name, c.description, c.columnSQL, c.isEncrypted, c.IsKey "
                + "FROM AD_Column c "
                + "WHERE c.AD_Table_ID=@tableid"
                + " AND c.IsActive='Y'"
                + " AND c.ColumnName <> 'AD_Client_ID'"
                + " AND c.ColumnName <> 'AD_Org_ID'"
                + " AND c.ColumnName <> 'IsActive'"
                + " AND c.ColumnName NOT LIKE 'Created%'"
                + " AND c.ColumnName NOT LIKE 'Updated%' "
                + "ORDER BY c.ColumnName";

            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@tableid", AD_Table_ID);
            IDataReader dr = null;
            try
            {
                bool ppCreated = false;

                dr = DataBase.DB.ExecuteReader(sql, param);
                while (dr.Read())
                {
                    String columnName = dr["ColumnName"].ToString();
                    bool isUpdateable = "Y".Equals(dr["IsUpdateable"].ToString());
                    bool isMandatory = "Y".Equals(dr["IsMandatory"].ToString());
                    int displayType = string.IsNullOrEmpty(dr["AD_Reference_ID"].ToString()) ? int.Parse("0") : int.Parse(dr["AD_Reference_ID"].ToString());
                    int AD_Reference_Value_ID = string.IsNullOrEmpty(dr["AD_Reference_Value_ID"].ToString()) ? int.Parse("0") : int.Parse(dr["AD_Reference_Value_ID"].ToString());
                    String defaultValue = dr["DefaultValue"].ToString();
                    int seqNo = string.IsNullOrEmpty(dr["SeqNo"].ToString()) ? int.Parse("0") : int.Parse(dr["SeqNo"].ToString());
                    int fieldLength = int.Parse(dr["FieldLength"].ToString());
                    String valueMin = dr["valueMin"].ToString();
                    String valueMax = dr["valueMax"].ToString();
                    String vFormat = dr["vFormat"].ToString();
                    String callOut = dr["callOut"].ToString();
                    String name = dr["name"].ToString();
                    String description = dr["description"].ToString();
                    String columnSQL = dr["columnSQL"].ToString();
                    bool virtualColumn = columnSQL != null && columnSQL.Length > 0;
                    bool isEncrypted = "Y".Equals(dr["isEncrypted"].ToString());
                    //		bool IsKey = "Y".equals(rs.getString(17));
                    //
                    sb.Append(CreateColumnMethods(mandatory,
                        columnName, isUpdateable, isMandatory,
                        displayType, AD_Reference_Value_ID, fieldLength,
                        defaultValue, valueMin, valueMax, vFormat,
                        callOut, name, description, virtualColumn, isEncrypted));
                    //	
                    if (seqNo == 1 && !ppCreated)
                    {
                        sb.Append(CreateKeyNamePair(columnName, displayType));
                        ppCreated = true;
                    }
                }

                dr.Close();
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
                if (dr != null)
                {
                    dr.Close();
                }
            }

            return sb;
        }

        /// <summary>
        /// CreateHeader
        /// </summary>
        /// <param name="AD_Table_ID">table id</param>
        /// <param name="sbText">generated text</param>
        /// <param name="mandatory">mandatory text</param>
        /// <param name="packageName">name of the package</param>
        /// <returns>created header in string</returns>
        private String CreateHeader(int AD_Table_ID, StringBuilder sb, StringBuilder mandatory, String packageName)
        {
            String tableName = "";
            int accessLevel = 0;
            long updatedMS = _createdMS;
            String sql = "SELECT TableName, AccessLevel, "
                + "(SELECT MAX(Updated) FROM AD_Column c WHERE t.AD_Table_ID=c.AD_Table_ID) as MaxUpdate, Updated "
                + "FROM AD_Table t WHERE AD_Table_ID=@tableid";
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@tableid", AD_Table_ID);
            IDataReader dr = null;
            try
            {
                dr = VAdvantage.DataBase.DB.ExecuteReader(sql, param);
                while (dr.Read())
                {

                    tableName = dr["TableName"].ToString();
                    accessLevel = int.Parse(dr["AccessLevel"].ToString());
                    //long msC = Convert.ToDateTime(dr["MaxUpdate"].ToString()).Millisecond;
                    //long msT = long.Parse(dr["Updated"].ToString());
                    //updatedMS = Math.Max(msC, msT);
                }
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                log.Log(Level.SEVERE, sql + " - " + AD_Table_ID, e);
            }

            if (tableName == null)
                throw new Exception("TableName not found for ID=" + AD_Table_ID);
            //
            String accessLevelInfo = accessLevel + " ";
            if (accessLevel >= 4)
                accessLevelInfo += "- System ";
            if (accessLevel == 2 || accessLevel == 3 || accessLevel == 6 || accessLevel == 7)
                accessLevelInfo += "- Client ";
            if (accessLevel == 1 || accessLevel == 3 || accessLevel == 5 || accessLevel == 7)
                accessLevelInfo += "- Org ";

            String keyColumn = tableName + "_ID";
            String className = "X_" + tableName;
            long serialVersionUID = 26282125316789L + updatedMS;
            //
            StringBuilder start = new StringBuilder()
                .Append("namespace " + packageName + "\n"
                    + "/** Generated Model - DO NOT CHANGE */\n");
            //if (!packageName.Equals("org.compiere.model"))
            //    start.Append("import org.compiere.model.*;");
            start.Append("using System;"
                + "using System.Text;"
                + "using VAdvantage.DataBase;"
                + "using VAdvantage.Common;"
                + "using VAdvantage.Classes;"
                + "using VAdvantage.Process;"
                + "using VAdvantage.Model;"
                + "using VAdvantage.Utility;"
                + "using System.Data;"
                //	Class
                + "/** Generated Model for ").Append(tableName).Append("\n"
                + " *  @author Raghu (Updated) \n"
                + " *  @version ").Append("Vienna Framework 1.1.1").Append(" - $Id$")
                //	.append(s_run)	//	Timestamp
                    .Append(" */\n"
                + "public class ").Append(className).Append(" : PO"
                + "{"
                //	Standard Constructor
                + "public ").Append(className).Append(" (Context ctx, int ").Append(keyColumn)
                    .Append(", Trx trxName) : base (ctx, ").Append(keyColumn).Append(", trxName)"
                + "{"
                + "/** if (").Append(keyColumn).Append(" == 0)"
                + "{").Append(mandatory).Append("} */\n"
                + "}"

                //	Standard Constructor
                + "public ").Append(className).Append(" (Ctx ctx, int ").Append(keyColumn)
                    .Append(", Trx trxName) : base (ctx, ").Append(keyColumn).Append(", trxName)"
                + "{"
                + "/** if (").Append(keyColumn).Append(" == 0)"
                + "{").Append(mandatory).Append("} */\n"
                + "}"


                //	Constructor End
                //	Short Constructor
                //			+ "/** Short Constructor */\n"
                //			+ "public ").append(className).append(" (Ctx ctx, int ").append(keyColumn).append(")"
                //			+ "{"
                //			+ "this (ctx, ").append(keyColumn).append(", null);"
                //			+ "}"	//	Constructor End

                //	Load Constructor
                + "/** Load Constructor \n@param ctx context\n@param rs result set \n@param trxName transaction\n*/\n"
                + "public ").Append(className).Append(" (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)"
                + "{"
                + "}"	//	Load Constructor End

                 + "/** Load Constructor \n@param ctx context\n@param rs result set \n@param trxName transaction\n*/\n"
                + "public ").Append(className).Append(" (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)"
                + "{"
                + "}"	//	Load Constructor End

                + "/** Load Constructor \n@param ctx context\n@param rs result set \n@param trxName transaction\n*/\n"
                + "public ").Append(className).Append(" (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)"
                + "{"
                + "}"	//	Load Constructor End

                + "/** Static Constructor \n Set Table ID By Table Name\n added by ->Harwinder */\n"
                + "static ").Append(className).Append("()"
                + "{"
                + " Table_ID = Get_Table_ID(Table_Name);"
                + " model = new KeyNamePair(Table_ID,Table_Name);"
                + "}"

                //
                + "/** Serial Version No */\n"
                + "static long serialVersionUID = " + serialVersionUID + "L;"
                + "/** Last Updated Timestamp " + DateTime.Now + " */\n"
                + "public static long updatedMS = " + updatedMS + "L;"
                + "/** AD_Table_ID=").Append(AD_Table_ID).Append(" */\n"
                + "public static int Table_ID; // =").Append(AD_Table_ID).Append(";\n"
                //	
                + "/** TableName=").Append(tableName).Append(" */\n"
                + "public static String Table_Name=\"").Append(tableName).Append("\";\n"
                + "protected static KeyNamePair model;"// = new KeyNamePair(Table_ID").Append(",\"").Append(tableName).Append("\");\n"   // ).Append(AD_Table_ID).Append(",\"").Append(tableName).Append("\");\n"
                //	
                + "protected Decimal accessLevel = new Decimal(").Append(accessLevel).Append(");"
                + "/** AccessLevel\n@return ").Append(accessLevelInfo).Append("\n*/\n"
                + "protected override int Get_AccessLevel()"
                + "{"
                + "return Convert.ToInt32(accessLevel.ToString());"
                + "}"
                //
                + "/** Load Meta Data\n@param ctx context\n@return PO Info\n*/\n"
                + "protected override POInfo InitPO (Context ctx)"
                + "{"
                + "POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);"
                + "return poi;"
                + "}"		//	initPO
                //
                //
                + "/** Load Meta Data\n@param ctx context\n@return PO Info\n*/\n"
                + "protected override POInfo InitPO (Ctx ctx)"
                + "{"
                + "POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);"
                + "return poi;"
                + "}"		//	initPO
                //

                + "/** Info\n@return info\n*/\n"
                + "public override String ToString()"
                + "{"
                + "StringBuilder sb = new StringBuilder (\"").Append(className).Append("[\")"
                + ".Append(Get_ID()).Append(\"]\");"
                + "return sb.ToString();"
                + "}");

            StringBuilder end = new StringBuilder("}\n}");
            //
            sb.Insert(0, start.ToString()); //By sarab
            sb.Append(end);

            return className;
        }

        /// <summary>
        /// WriteToFile
        /// </summary>
        /// <param name="sbText">generated text</param>
        /// <param name="fileName">name of the file</param>
        private void WriteToFile(StringBuilder sbText, String fileName)
        {
            TextWriter fw = new StreamWriter(fileName);
            for (int i = 0; i < sbText.Length; i++)
            {
                char c = sbText[i];
                //after
                if (c == ';' || c == '}')
                {
                    fw.Write(c);
                    if (sbText.ToString().Substring(i + 1).StartsWith("//"))
                        fw.Write('\t');
                    else
                        fw.Write('\n');

                }
                //	before & after
                else if (c == '{')
                {
                    fw.Write('\n');
                    fw.Write(c);
                    fw.Write('\n');
                }
                else
                    fw.Write(c);
            }
            fw.Flush();
            fw.Close();

        }

        /// <summary>
        /// CreateColumnMethods
        /// </summary>
        /// <param name="mandatory">mandatory text</param>
        /// <param name="columnName">columnName</param>
        /// <param name="isUpdateable">isUpdateable - true or false</param>
        /// <param name="isMandatory">isMandatory - true of false</param>
        /// <param name="displayType">displayType (int)</param>
        /// <param name="AD_Reference_ID">AD_Reference_ID</param>
        /// <param name="fieldLength">fieldLength</param>
        /// <param name="defaultValue">defaultValue</param>
        /// <param name="ValueMin">Min value</param>
        /// <param name="ValueMax">max value</param>
        /// <param name="VFormat">string format</param>
        /// <param name="Callout">callout string</param>
        /// <param name="Name">name</param>
        /// <param name="Description">description</param>
        /// <param name="virtualColumn">virtual column - true or false</param>
        /// <param name="IsEncrypted">IsEncrypted - true of false</param>
        /// <returns>String</returns>
        private String CreateColumnMethods(StringBuilder mandatory,
             String columnName, bool isUpdateable, bool isMandatory,
             int displayType, int AD_Reference_ID, int fieldLength,
             String defaultValue, String valueMin, String valueMax, String vFormat,
             String callOut, String name, String description,
             bool virtualColumn, bool isEncrypted)
        {
            //	Clazz
            Type clazz = DisplayType.GetClass(displayType, true);
            if (defaultValue == null)
                defaultValue = "";
            if (DisplayType.IsLOB(displayType))		//	No length check for LOBs
                fieldLength = 0;

            //	Handle Posted
            if (columnName.Equals("Posted", StringComparison.InvariantCultureIgnoreCase)
                || columnName.Equals("Processed", StringComparison.InvariantCultureIgnoreCase)
                || columnName.Equals("Processing", StringComparison.InvariantCultureIgnoreCase))
            {
                clazz = typeof(bool);
                AD_Reference_ID = 0;
            }
            //	Record_ID
            else if (columnName.Equals("Record_ID", StringComparison.InvariantCultureIgnoreCase))
            {
                clazz = typeof(int);
                AD_Reference_ID = 0;
            }
            //	String Key
            else if (columnName.Equals("AD_Language", StringComparison.InvariantCultureIgnoreCase)
                || columnName.Equals("EntityType", StringComparison.InvariantCultureIgnoreCase)
                || columnName.Equals("DocBaseType", StringComparison.InvariantCultureIgnoreCase))
            {
                clazz = typeof(String);
            }
            //	Data Type
            String dataType = clazz.Name;
            dataType = dataType.Substring(dataType.LastIndexOf('.') + 1);
            if (dataType.Equals("bool"))
                dataType = "bool";
            else if (dataType.Equals("Int32"))
                dataType = "int";
            //else if (displayType == displayType.Binary)
            //    dataType = "byte[]";


            StringBuilder sb = new StringBuilder("");
            //	****** Set Comment ******
            sb.Append("/** Set ").Append(name);
            sb.Append(".\n@param ").Append(columnName).Append(" ");
            if (description != null && description.Length > 0)
                sb.Append(description);
            else
                sb.Append(name);
            sb.Append(" */\n");

            //	Set	********
            String setValue = "Set_Value";
            if (isEncrypted)
                setValue = "Set_ValueE";
            //	public void setColumn (xxx variable)
            sb.Append("public ");
            if (!isUpdateable)
                setValue = "Set_ValueNoCheck";


            if (dataType == "DateTime" || dataType == "Decimal")
            {
                sb.Append("void Set").Append(columnName).Append(" (").Append(dataType + "?").Append(" ").Append(columnName).Append(")"
                    + "{");
            }
            else
            {
                sb.Append("void Set").Append(columnName).Append(" (").Append(dataType).Append(" ").Append(columnName).Append(")"
                    + "{");
            }
            //	List Validation
            if (AD_Reference_ID != 0)
            {
                String staticVar = addListValidation(sb, AD_Reference_ID, columnName, !isMandatory);
                sb.Insert(0, staticVar);	//	first check
            }
            //	setValue ("ColumnName", xx);
            if (virtualColumn)
            {
                sb.Append("throw new ArgumentException (\"").Append(columnName).Append(" Is virtual column\");");
            }
            //	Integer
            else if (clazz == typeof(int))
            {
                if (columnName.EndsWith("_ID"))
                {
                    if (isMandatory)	//	check mandatory ID
                    {
                        int firstOK = 1;	//	Valid ID 0
                        if (columnName.Equals("AD_Client_ID") || columnName.Equals("AD_Org_ID")
                            || columnName.Equals("Record_ID") || columnName.Equals("C_DocType_ID")
                            || columnName.Equals("Node_ID") || columnName.Equals("AD_Role_ID")
                            || columnName.Equals("M_AttributeSet_ID") || columnName.Equals("M_AttributeSetInstance_ID"))
                            firstOK = 0;
                        sb.Append("if (").Append(columnName)
                            .Append(" < ").Append(firstOK).Append(") throw new ArgumentException (\"")
                            .Append(columnName).Append(" is mandatory.\");");
                    }
                    else				//	set optional _ID to null if 0
                    {
                        sb.Append("if (").Append(columnName).Append(" <= 0) ")
                            .Append(setValue).Append(" (\"").Append(columnName).Append("\", null);")
                            .Append("else\n");
                    }
                }
                //	set_Value ("C_BPartner_ID", new Integer(C_BPartner_ID));
                sb.Append(setValue).Append(" (\"").Append(columnName).Append("\", ").Append(columnName).Append(");");
            }
            //	bool
            else if (clazz == typeof(bool))
            {
                sb.Append(setValue).Append(" (\"").Append(columnName)
                    .Append("\", ").Append(columnName).Append(");");
            }
            else
            {
                if (isMandatory && AD_Reference_ID == 0)	//	does not apply to int/bool
                {
                    sb.Append("if (")
                        .Append(columnName).Append(" == null)"
                            + " throw new ArgumentException (\"")
                        .Append(columnName).Append(" is mandatory.\");");
                }
                // String length check
                if ((clazz == typeof(String)) && fieldLength > 0)
                {
                    sb.Append("if (");
                    if (!isMandatory)
                        sb.Append(columnName).Append(" != null && ");
                    sb.Append(columnName).Append(".Length > ").Append(fieldLength)
                        .Append("){log.Warning(\"Length > ")
                        .Append(fieldLength).Append(" - truncated\");")
                        .Append(columnName).Append(" = ")
                        .Append(columnName).Append(".Substring(0,").Append(fieldLength).Append(");}");

                    sb.Append(setValue).Append(" (\"").Append(columnName).Append("\", ")
                        .Append(columnName).Append(");");
                }
                else
                {

                    if (dataType == "DateTime")
                    {
                        sb.Append(setValue).Append(" (\"").Append(columnName).Append("\", ")
                            .Append("(DateTime?)" + columnName).Append(");");
                    }
                    else if (dataType == "Decimal")
                    {
                        sb.Append(setValue).Append(" (\"").Append(columnName).Append("\", ")
                         .Append("(Decimal?)" + columnName).Append(");");
                    }
                    else
                    {
                        sb.Append(setValue).Append(" (\"").Append(columnName).Append("\", ")
                            .Append(columnName).Append(");");
                    }
                }
                //	set_Value ("C_BPartner_ID", new Integer(C_BPartner_ID));
            }
            //	set Method close
            sb.Append("}");

            //	Mandatory call in constructor
            if (isMandatory)
            {
                mandatory.Append("Set").Append(columnName).Append(" (");
                if (clazz.Equals(typeof(int)))
                    mandatory.Append("0");
                else if (clazz.Equals(typeof(bool)))
                {
                    if (defaultValue.IndexOf('Y') != -1)
                        mandatory.Append("true");
                    else
                        mandatory.Append("false");
                }
                else if (clazz.Equals(typeof(Decimal)))
                    mandatory.Append("0.0");
                else if (clazz.Equals(typeof(DateTime)))
                    mandatory.Append("DateTime.Now");
                else
                    mandatory.Append("null");
                mandatory.Append(");");
                if (defaultValue.Length > 0)
                    mandatory.Append("// ").Append(defaultValue).Append("\n");
            }

            //	****** Get Comment ****** 
            sb.Append("/** Get ").Append(name);
            if (description != null && description.Length > 0)
                sb.Append(".\n@return ").Append(description);
            else
                sb.Append(".\n@return ").Append(name);
            sb.Append(" */\n");

            //	Get	********
            String getValue = "Get_Value";


            if (dataType == "DateTime")
                sb.Append("public ").Append(dataType + "?");
            else
                sb.Append("public ").Append(dataType);

            if (clazz.Equals(typeof(bool)))
            {
                sb.Append(" Is");
                if (columnName.ToLower().StartsWith("is"))
                    sb.Append(columnName.Substring(2));
                else
                    sb.Append(columnName);
            }
            else
                sb.Append(" Get").Append(columnName);
            sb.Append("() {");
            if (clazz.Equals(typeof(int)))
                sb.Append("Object ii = ")//for null value from database change(int)-->(int?)
                    .Append(getValue).Append("(\"").Append(columnName).Append("\");"
                    + "if (ii == null)"
                    + " return 0;"
                    + "return Convert.ToInt32(ii);");
            else if (clazz.Equals(typeof(Decimal)))
            {
                sb.Append("Object bd =").Append(getValue)
                    .Append("(\"").Append(columnName).Append("\");"
                    + "if (bd == null)"
                    + " return Env.ZERO;"
                    + "return  Convert.ToDecimal(bd);");
            }
            else if (clazz.Equals(typeof(bool)))
                sb.Append("Object oo = ").Append(getValue)
                    .Append("(\"").Append(columnName).Append("\");"
                    + "if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return \"Y\".Equals(oo);}"
                    + "return false;");
            else if (dataType.Equals("Object"))
                sb.Append("return ").Append(getValue)
                    .Append("(\"").Append(columnName).Append("\");");
            else if (dataType.Equals("DateTime"))
                sb.Append("return (").Append(dataType).Append("?)").Append(getValue)
                    .Append("(\"").Append(columnName).Append("\");");
            else
                sb.Append("return (").Append(dataType).Append(")").Append(getValue)
                    .Append("(\"").Append(columnName).Append("\");");
            sb.Append("}");
            //
            return sb.ToString();
        }

        /// <summary>
        /// CreateKeyNamePair
        /// </summary>
        /// <param name="columnName"><columnName/param>
        /// <param name="displayType">displayType - int</param>
        /// <returns>column methods in string</returns>
        private StringBuilder CreateKeyNamePair(String columnName, int displayType)
        {
            String method = "Get" + columnName + "()";
            if (displayType != DisplayType.String)
                method = "" + method + ".ToString()";
            StringBuilder sb = new StringBuilder("/** Get Record ID/ColumnName\n@return ID/ColumnName pair */\n"
                + "public KeyNamePair GetKeyNamePair() "
                + "{return new KeyNamePair(Get_ID(), ").Append(method).Append(");}");
            return sb;
        }

        /// <summary>
        /// AddListValidation
        /// </summary>
        /// <param name="sbText">Generated text</param>
        /// <param name="AD_Reference_ID">AD_Reference_ID</param>
        /// <param name="columnName">columnName</param>
        /// <param name="nullable">nullable - true or false</param>
        /// <returns></returns>
        private String addListValidation(StringBuilder sb, int AD_Reference_ID, String columnName, bool nullable)
        {
            StringBuilder isValid = new StringBuilder("/** Is test a valid value.\n@param test testvalue\n@returns true if valid **/\npublic bool Is")
                .Append(columnName).Append("Valid (String test){return ");

            StringBuilder retValue = new StringBuilder();
            retValue.Append("\n/** ").Append(columnName).Append(" AD_Reference_ID=").Append(AD_Reference_ID).Append(" */\n")
                .Append("public static int ").Append(columnName.ToUpper())
                .Append("_AD_Reference_ID=").Append(AD_Reference_ID).Append(";");
            //
            bool found = false;
            StringBuilder values = new StringBuilder("Reference_ID=")
                .Append(AD_Reference_ID);
            StringBuilder statement = new StringBuilder("");
            if (nullable)
            {
                statement.Append("if (").Append(columnName).Append(" == null");
                isValid.Append("test == null");
            }
            //
            bool isRealList = false;
            String sql = "SELECT Value, name FROM AD_Ref_List WHERE AD_Reference_ID=@refid";
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@refid", AD_Reference_ID);
            IDataReader dr = null;
            try
            {
                dr = VAdvantage.DataBase.DB.ExecuteReader(sql, param);
                while (dr.Read())
                {
                    isRealList = true;
                    String value = dr["Value"].ToString();
                    values.Append(" - ").Append(value);
                    if (statement.Length == 0)
                    {
                        statement.Append("if (").Append(columnName)
                            .Append(".Equals(\"").Append(value).Append("\")");
                        isValid.Append("test.Equals(\"").Append(value).Append("\")");
                    }
                    else
                    {
                        statement.Append(" || ").Append(columnName)
                            .Append(".Equals(\"").Append(value).Append("\")");
                        isValid.Append(" || test.Equals(\"").Append(value).Append("\")");
                    }
                    //
                    if (!found)
                    {
                        found = true;
                        if (!nullable)
                            sb.Append("if (")
                                .Append(columnName).Append(" == null)"
                                + " throw new ArgumentException (\"")
                                .Append(columnName).Append(" is mandatory\");");
                    }



                    //	name (SmallTalkNotation)
                    String name = dr["name"].ToString();
                    char[] nameArray = name.ToCharArray();
                    StringBuilder nameClean = new StringBuilder();
                    bool initCap = true;
                    for (int i = 0; i < nameArray.Length; i++)
                    {

                        char c = nameArray[i];
                        Regex specialCharacter = new Regex(@"^[a-zA-Z0-9]*$");  //Code Changed By Sarab
                        if (specialCharacter.IsMatch(c.ToString()))
                        {
                            //if (java.lang.Character.isJavaIdentifierPart(c)) //Code Commented By Sarab
                            //{                      
                            if (initCap)
                                nameClean.Append(c.ToString().ToUpper());
                            else
                                nameClean.Append(c);
                            initCap = false;
                        }
                        else
                        {
                            if (c == '+')
                                nameClean.Append("Plus");
                            else if (c == '-')
                                nameClean.Append("_");
                            else if (c == '>')
                            {
                                if (name.IndexOf('<') == -1)	//	ignore <xx>
                                    nameClean.Append("Gt");
                            }
                            else if (c == '<')
                            {
                                if (name.IndexOf('>') == -1)	//	ignore <xx>
                                    nameClean.Append("Le");
                            }
                            else if (c == '!')
                                nameClean.Append("Not");
                            else if (c == '=')
                                nameClean.Append("Eq");
                            else if (c == '~')
                                nameClean.Append("Like");
                            initCap = true;
                        }
                    }
                    retValue.Append("/** ").Append(name).Append(" = ").Append(value).Append(" */\n");
                    retValue.Append("public static String ").Append(columnName.ToUpper())
                        .Append("_").Append(nameClean)
                        .Append(" = \"").Append(value).Append("\";");
                }
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
                found = false;
            }

            //
            statement = new StringBuilder("if (!Is").Append(columnName)
                .Append("Valid(").Append(columnName).Append(")");
            statement.Append(")\nthrow new ArgumentException (\"").Append(columnName)
                .Append(" Invalid value - \" + ").Append(columnName)
                .Append(" + \" - ").Append(values).Append("\");");
            //
            if (found && !columnName.Equals("EntityType"))
                sb.Append(statement);
            //
            if (isRealList)
            {
                isValid.Append(";}");
                retValue.Append(isValid);
            }
            return retValue.ToString();
        }

        /// <summary>
        /// Main entry method
        /// </summary>
        /// <returns>final execution status</returns>
        public static string StartProcess(string namespaceName, string directory, bool getChekboxStatus, string getTableId, string classType, out StringBuilder sbTextCopy, out string fileName)
        {
            //string directory = Directory;// "D:\\XClass\\";

            //if (string.IsNullOrEmpty(directory))
            //{
            //    sbTextCopy = sbTextCopy1;
            //    fileName = fileName1;
            //    return "No Directory";
            //}


            //second parameter
            //string namespaceName = "VAdvantage.Model";
            //string namespaceName = "VAdvantage.Model";
            namespaceName += "{";

            if (string.IsNullOrEmpty(namespaceName))
            {
                sbTextCopy = sbTextCopy1;
                fileName = fileName1;
                return "No Namespace";
            }

            //third parameter
            string entityType;// = "'U','A'";	//	User, Application

            if (getChekboxStatus == false)
            {
                entityType = "'D'";	//	User, Application
            }
            else
            {
                entityType = classType;// "'U','A'";	//	User, Application 
                // entityType = "'U'";
            }
            if (string.IsNullOrEmpty(entityType))
            {
                sbTextCopy = sbTextCopy1;
                fileName = fileName1;
                return "No Entity Type";
            }

            //StringBuilder sql = new StringBuilder("");
            StringBuilder sql = new StringBuilder("EntityType IN (").Append(entityType).Append(")");
            int count = 0;
            if (getChekboxStatus == false)
            {
                //MessageBox.Show(GlobalVariable.GetChekboxStatus.ToString());
                //MessageBox.Show(GlobalVariable.GetTableId.ToString());
                //sql.Insert(0, "SELECT AD_Table_ID "
                //    + "FROM AD_Table "
                //    + "WHERE (TableName IN ('RV_WarehousePrice','RV_BPartner')"	//	special views
                //        + " OR IsView='N')"
                //    + " AND Referenced_Table_ID IS NULL"
                //    + " AND TableName NOT LIKE '%_Trl' and AD_Table_ID =" + GlobalVariable.GetTableId); // TableName NOT LIKE 'T%' AND ");
                //sql.Append(" ORDER BY TableName");

                new GenerateModel(int.Parse(getTableId), directory, namespaceName);
                count++;
            }
            else
            {
                // MessageBox.Show(GlobalVariable.GetChekboxStatus.ToString());
                sql.Insert(0, "SELECT AD_Table_ID "
                    + "FROM AD_Table "
                    + "WHERE (TableName IN ('RV_WarehousePrice','RV_BPartner')"	//	special views
                        + " OR IsView='N')"
                    + " AND Referenced_Table_ID IS NULL"
                    + " AND TableName NOT LIKE '%_Trl' AND "); // TableName NOT LIKE 'T%' AND ");
                sql.Append(" ORDER BY TableName");
                IDataReader dr = null;
                try
                {

                    dr = VAdvantage.DataBase.DB.ExecuteReader(sql.ToString());

                    while (dr.Read())
                    {

                        new GenerateModel(int.Parse(dr["AD_Table_ID"].ToString()), directory, namespaceName);
                        count++;
                    }
                    dr.Close();
                }
                catch (Exception e)
                {
                    log.Severe(e.ToString());
                    if (dr != null)
                    {
                        dr.Close();
                    }
                }
            }



            //try
            //{

            //    IDataReader dr = VAdvantage.DataBase.DB.ExecuteReader(sql.ToString());

            //    while (dr.Read())
            //    {
            //        new GenerateModelcls(int.Parse(dr["AD_Table_ID"].ToString()), directory, namespaceName);
            //        count++;
            //    }
            //    dr.Close();
            //}
            //catch (SqlException sqlexc)
            //{

            //} 
            sbTextCopy = sbTextCopy1;
            fileName = fileName1;
            if (getChekboxStatus == false)
            {
                return "Generated " + count + " Class";
            }
            else
            {
                return "Generated " + count + " Classes";
            }
        }

    }
}