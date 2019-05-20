/* 
 * Create Xml Files Against Data In AD_Module_DB_Schema Table Filer By Module
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
        int AD_ModuleInfo_ID = 0;
        //ID
        int AD_Module_DB_Schema_ID = 0;
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
            AD_Module_DB_Schema_ID = GetRecord_ID();

            IDataReader dr = DB.ExecuteReader(@" SELECT AD_ModuleInfo_ID,Name,Prefix,VersionId,versionNo FROM AD_ModuleInfo 
                                                WHERE AD_ModuleInfo_ID = 
                                                (SELECT AD_ModuleInfo_ID FROM AD_Module_DB_Schema 
                                                  WHERE AD_Module_DB_Schema_ID =" + AD_Module_DB_Schema_ID + ")");
            if (dr.Read())
            {
                AD_ModuleInfo_ID = Util.GetValueOfInt(dr[0]);
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
            if (string.IsNullOrEmpty(Module_Name) || AD_ModuleInfo_ID < 1)
            {
                throw new ArgumentNullException("Empty Module Name");
            }

            IDataReader dr = DB.ExecuteReader("SELECT DISTINCT TableName FROM AD_Module_DB_Schema WHERE AD_ModuleInfo_ID =" + AD_ModuleInfo_ID);
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

                ds = DB.ExecuteDataset(" SELECT t.* FROM " + tableName + " t INNER JOIN AD_Module_DB_Schema db "
                                    + " ON t." + GetKeyColumnName(tableName) + " = db.Record_ID WHERE db.TableName = '" + tableName + "' "
                                    + " AND db.AD_ModuleInfo_ID = " + AD_ModuleInfo_ID);


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
            DataSet ds = DB.ExecuteDataset(" SELECT AD_Table_ID,TableName,Record_ID FROM AD_Module_DB_Schema WHERE AD_ModuleInfo_ID =" + AD_ModuleInfo_ID + " ORDER BY AD_Module_DB_Schema_ID desc");
            StreamWriter sw = new StreamWriter(pathFolder + "\\" + "AD_Module_DB_Schema.xml");
            if (ds.Tables.Count > 0)
            {
                ds.Tables[0].TableName = "AD_Module_DB_Schema";
            }
            ds.WriteXml(sw, XmlWriteMode.WriteSchema);
            sw.Close();
            ds = null;

            //2. DB Script

            ds = DB.ExecuteDataset(" SELECT * FROM AD_Module_DBScript WHERE IsActive='Y' AND  AD_ModuleInfo_ID = " + AD_ModuleInfo_ID +" ORDER BY AD_Module_DBScript_ID");
            sw = new StreamWriter(pathFolder + "\\" + "AD_Module_DBScript.xml");
            if (ds.Tables.Count > 0)
            {
                ds.Tables[0].TableName = "AD_Module_DBScript";
            }
            ds.WriteXml(sw, XmlWriteMode.WriteSchema);
            sw.Close();
            ds = null;

        }
        private void CreateXML(string tableName)
        {
            DataSet ds = DB.ExecuteDataset(" SELECT * FROM " + tableName + " WHERE AD_ModuleInfo_ID =" + AD_ModuleInfo_ID);
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
            /* SpecialCase For AD_Ref_Table*/
            if (tableName == "AD_Ref_Table")
                return "AD_Reference_ID";
            else
                return tableName + "_ID";
        }
    }
}
