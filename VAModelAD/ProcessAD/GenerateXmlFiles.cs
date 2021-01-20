/* 
 * Create Xml Files Against Data In VAF_Module_DB_Schema Table Filer By Module
 * Used Table name as Xml File name 
 * Directory is fixed having name 'ModuleXMLFiles'
 * next Module name used as folder for xml files
 * e.g  [Application Folder]\\ModuleXMLFiles\\'Module_name'\\  'Table_Name'.xml
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;
using System.IO;

namespace VAdvantage.Process
{
    public class GenerateXmlFiles : SvrProcess
    {
        #region Declaration
        //Foreign ID 
        int VAF_ModuleInfo_ID = 0;
        //ID
        int VAF_Module_DB_Schema_ID = 0;
        //Replace Space with Underscore
        string Module_Name = "";
        string prefix = "";
        string versionId = "1.0.0.0";
        string versionNo = "1";
        string Database_Schema = "DataBaseSchema";


        //Physical Folder
        const string XML_FOLDER = "ModuleXMLFiles";
        //List Conating Distinct Table Name
        List<String> lstTables = new List<string>();



        String pathFolder = "";
        #endregion

        protected override void Prepare()
        {
            VAF_Module_DB_Schema_ID = GetRecord_ID();

            IDataReader dr = DB.ExecuteReader(@" SELECT VAF_ModuleInfo_ID,Name,Prefix,VersionId,versionNo FROM VAF_ModuleInfo 
                                                WHERE VAF_ModuleInfo_ID = 
                                                (SELECT VAF_ModuleInfo_ID FROM VAF_Module_DB_Schema 
                                                  WHERE VAF_Module_DB_Schema_ID =" + VAF_Module_DB_Schema_ID + ")");
            if (dr.Read())
            {
                VAF_ModuleInfo_ID = Util.GetValueOfInt(dr[0]);
                Module_Name = dr[1].ToString();//.Replace(' ', '_');
                prefix = dr[2].ToString();
                versionId = dr[3].ToString();
                if (string.IsNullOrEmpty(versionId))
                {
                    versionId = "1";
                }
                versionNo = dr[4].ToString();
                if (string.IsNullOrEmpty(versionNo))
                {
                    versionNo = "1.0.0.0";
                }
            }
            dr.Close();
            pathFolder = System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, prefix, Module_Name.Trim()+ "_" +versionNo , Database_Schema);
            //throw new NotImplementedException();
        }

        protected override string DoIt()
        {
            if (string.IsNullOrEmpty(Module_Name) || VAF_ModuleInfo_ID < 1)
            {
                throw new ArgumentNullException("Empty Module Name");
            }

            IDataReader dr = DB.ExecuteReader("SELECT DISTINCT TableName FROM VAF_Module_DB_Schema WHERE VAF_ModuleInfo_ID =" + VAF_ModuleInfo_ID);
            while (dr.Read())
            {
                lstTables.Add(dr[0].ToString());
            }
            dr.Close();

            CreateXMFiles();

            return "Files Generated in " + prefix + " folder";
        }

        /// <summary>
        /// Create XMl Files Physically in application folder
        /// </summary>
        private void CreateXMFiles()
        {

            if (!Directory.Exists(pathFolder))
            {
                Directory.CreateDirectory(pathFolder);
            }

            DataSet ds = null;
            StreamWriter sw = null;

            CreateModuleSchemaFiles();



            string tableName = "";

            for (int t = 0; t < lstTables.Count; t++)
            {
                tableName = lstTables[t];

                ds = DB.ExecuteDataset(" SELECT t.* FROM " + tableName + " t INNER JOIN VAF_Module_DB_Schema db "
                                    + " ON t." + GetKeyColumnName(tableName) + " = db.Record_ID WHERE db.TableName = '" + tableName + "' "
                                    + " AND db.VAF_ModuleInfo_ID = " + VAF_ModuleInfo_ID);


                sw = new StreamWriter(pathFolder + "\\" + tableName + ".xml");
                if (ds.Tables.Count > 0)
                {
                    ds.Tables[0].TableName = tableName;
                }
                ds.WriteXml(sw, XmlWriteMode.WriteSchema);
                sw.Close();
                ds = null;
                sw = null;
            }
        }

        private void CreateModuleSchemaFiles()
        {
            //1. Create DB_Schema xml
            DataSet ds = DB.ExecuteDataset(" SELECT VAF_TableView_ID,TableName,Record_ID FROM VAF_Module_DB_Schema WHERE VAF_ModuleInfo_ID =" + VAF_ModuleInfo_ID + " ORDER BY VAF_Module_DB_Schema_ID desc");
            StreamWriter sw = new StreamWriter(pathFolder + "\\" + "VAF_Module_DB_Schema.xml");
            if (ds.Tables.Count > 0)
            {
                ds.Tables[0].TableName = "VAF_Module_DB_Schema";
            }
            ds.WriteXml(sw, XmlWriteMode.WriteSchema);
            sw.Close();
            ds = null;

            //2. DB Script

            ds = DB.ExecuteDataset(" SELECT * FROM VAF_Module_DBScript WHERE IsActive='Y' AND  VAF_ModuleInfo_ID = " + VAF_ModuleInfo_ID +" ORDER BY VAF_Module_DBScript_ID");
            sw = new StreamWriter(pathFolder + "\\" + "VAF_Module_DBScript.xml");
            if (ds.Tables.Count > 0)
            {
                ds.Tables[0].TableName = "VAF_Module_DBScript";
            }
            ds.WriteXml(sw, XmlWriteMode.WriteSchema);
            sw.Close();
            ds = null;

        }
        private void CreateXML(string tableName)
        {
            DataSet ds = DB.ExecuteDataset(" SELECT * FROM " + tableName + " WHERE VAF_ModuleInfo_ID =" + VAF_ModuleInfo_ID);
            StreamWriter sw = new StreamWriter(pathFolder + "\\" + tableName + ".xml");
            if (ds.Tables.Count > 0)
            {
                ds.Tables[0].TableName = tableName;
            }
            ds.WriteXml(sw, XmlWriteMode.WriteSchema);
            sw.Close();
            ds = null;
        }


        private string GetKeyColumnName(string tableName)
        {
            /* SpecialCase For VAF_CtrlRef_Table*/
            if (tableName == "VAF_CtrlRef_Table")
                return "VAF_Control_Ref_ID";
            else
                return tableName + "_ID";
        }
    }
}
