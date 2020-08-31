using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Xml.Linq;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace MarketSvc
{
    public class ImportMoule : IMarketCallback
    {

        private StringBuilder sbLog = new StringBuilder("");
        private List<CallBackDetail> lstCallback = new List<CallBackDetail>();
        private readonly object locklist = new object();

        public List<CallBackDetail> GetCallBackMsg()
        {
            List<CallBackDetail> lst = null;
            lock (locklist)
            {
                lst = new List<CallBackDetail>(lstCallback);

                lstCallback.Clear();
            }
            return lst;
        }

        public string GetLog()
        {
            var log = sbLog.ToString();
            sbLog.Clear();
            return log;
        }



        /// <summary>
        /// Import from Local 
        /// </summary>
        /// <param name="moduleName">Module Name</param>
        /// <param name="clientIds">client Id Array</param>
        /// <param name="langCode">Lang Codes</param>
        /// <param name="onlyTranslation">Only translate</param>
        /// <param name="ctxDic"> client context </param>
        public CallBackDetail ImportModuleFromLocalFile(string moduleName, int[] clientIds, string[] langCode, bool onlyTranslation, IDictionary<string, string> ctxDic)
        {
            //callback = OperationContext.Current.GetCallbackChannel<IMarketCallback>();
            sbError = new StringBuilder("");
            DateTime dtStart = DateTime.Now;

            string moduleID;
            string prefix, name, newVersion, version;
            try
            {
                //Get New Version and current Version of module
                prefix = moduleName.Substring(0, moduleName.IndexOf('\\'));
                string nameVersion = moduleName.Substring(moduleName.IndexOf('\\') + 1);
                int indexSeprator = nameVersion.LastIndexOf('_');
                name = nameVersion.Substring(0, indexSeprator);
                newVersion = nameVersion.Substring(indexSeprator + 1); // new version [from Folder ]

                object vrsn = DB.ExecuteScalar // from database
                        ("SELECT VersionNo FROM AD_ModuleInfo WHERE LOWER(Prefix) = '" + prefix.ToLower() + "'  AND LOWER(name) ='" + name.ToLower() + "'") ?? "0.0.0.0";

                version = vrsn.ToString(); // default

            }
            catch
            {
                //sbMsg.Append("Error parsing " + moduleName + " Or getting module ID" }));
                lstCallback.Add(new CallBackDetail() { Status = "Done", Action = "Error", ActionMsg = "Error parsing " + moduleName + " Or getting module ID" });
                return null;            }



            Ctx ctx = new Ctx(ctxDic);

            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            SendMsg("start at => " + dtStart.ToShortTimeString());
            SendPValue("12");
            SendMsg("Checking module folder");

            if (!Directory.Exists(basePath + moduleName))
            {
                SendMsg("Module folder not found", true);
                return null;
            }

            /*clientBinPath*/
            string clientBinPath = basePath + moduleName + "\\ClientBin";
            /*clientBinPath*/
            string clientAreaPath = basePath + moduleName + "\\Areas";

            /*bin Path*/
            string binPath = basePath + moduleName + "\\htmlbin";
            /* Databse Schema Path */
            string dbSchemaPath = basePath + moduleName + "\\DataBaseSchema";
            /* cReports Path */
            string cReportsPath = basePath + moduleName + "\\CReports";
            /* Svc Path */
            string svcPath = basePath + moduleName + "\\htmlSvc";
            /* Svc Path */
            string translationPath = basePath + moduleName + "\\Translations";
            /* End */

            bool redirectWithUrl = false;

            SendMsg("Module folder found.");

            SendMsg("Import start for module[prefix\\name_versionNo] <===> " + moduleName);


            if (!onlyTranslation)
            {
                SendPValue("15");
                #region Data Import

                /************************************** Data Import //**********************************/

                String datapath = Path.Combine(basePath, moduleName, "Data");
                if (Directory.Exists(dbSchemaPath) || Directory.Exists(datapath))
                {
                    SendPValue("16");
                    ImportModuleFromServer importmodule = new ImportModuleFromServer(clientIds);
                    if (Directory.Exists(dbSchemaPath))
                    {
                        SendMsg("----------------------------------<br />  Importing Schema <br /> ----------------------------------");
                        SendPValue("17", "Y");

                        importmodule.Init(0, dbSchemaPath, sbLog);
                        importmodule.CurrentModuleVersion = version;

                        for (int i = 0; i < clientIds.Length; i++)
                        {
                            SendMsg(" Start for  ===> " + clientIds[i]);
                            importmodule.Prepare(clientIds[i], i == 0, i == clientIds.Length - 1);
                            // Set Current Database Version 
                            importmodule.DoIt(ctx, this);
                            SendMsg(" Done for ====>" + clientIds[i]);
                        }
                        SendMsg("------- Schema Imported -------");
                        importmodule.Clear();
                    }
                    SendPValue("25");
                    if (Directory.Exists(datapath))
                    {
                        SendMsg("----------------------------------<br /> Importing Data <br /> ----------------------------------");
                        SendPValue("27", "Y");

                        importmodule.Init(0, datapath, sbLog);
                        //importmodule.CurrentModuleVersion = version;

                        for (int i = 0; i < clientIds.Length; i++)
                        {
                            SendMsg(" Start for  ===> " + clientIds[i]);
                            importmodule.Prepare(clientIds[i], i == 0, i == clientIds.Length - 1);
                            importmodule.DoIt(ctx, this);
                            SendMsg(" Done for ====>" + clientIds[i]);
                        }
                        SendMsg("------- Data Imported -------");
                        importmodule.Clear();
                    }
                    importmodule = null;
                }
                /**************************************** End ******************************************/

                #endregion
                SendPValue("45");
                #region "Silverlight"
                /**************************************** SILVERLIGHT **********************************\
             */

                //if (Directory.Exists(clientBinPath))
                //{

                //    QueryExecuted(new CallBackDetail()
                //    {
                //        Status = "-----------------------------------------------------------------------"
                //    });
                //    QueryExecuted(new CallBackDetail() { Status = "Silverlight Import Start<br />--------------------------------------------------------------------" });
                //    //Client BIN Silverlight
                //    /* Backup folder path */
                //    string backupPath = AppDomain.CurrentDomain.BaseDirectory + "XapBackupFile";
                //    /* Temp folder path */
                //    string tempPath = backupPath + "\\Temp";
                //    /* isChanged */
                //    bool isChanged = false;
                //    /* check Backup directory */
                //    if (!Directory.Exists(backupPath))
                //    {
                //        /* contain last before module update */
                //        Directory.CreateDirectory(backupPath);
                //        /* Temp directory for modification */
                //        Directory.CreateDirectory(tempPath);
                //        /* message */
                //        QueryExecuted(new CallBackDetail() { Status = "Backup directory created" });
                //    }
                //    //Message
                //    QueryExecuted(new CallBackDetail() { Status = "Coping xap file in backup and temp directory" });
                //    /* copy xap to Temp folder as zip */
                //    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "ClientBin\\ViennaAdvantage.xap", tempPath + "\\ViennaAdvantage.zip", true);
                //    //Message
                //    QueryExecuted(new CallBackDetail() { Status = "done" });
                //    QueryExecuted(new CallBackDetail() { Status = "Module integration start" });

                //    //read Zip file
                //    ZipFile zipFile = ZipFile.Read(tempPath + "\\ViennaAdvantage.zip");
                //    //find Appmanifest file
                //    foreach (ZipEntry ze in zipFile.Entries)
                //    {
                //        if (ze.FileName == "AppManifest.xaml")
                //        {
                //            //Extract to temp folder
                //            ze.Extract(tempPath, ExtractExistingFileAction.OverwriteSilently);
                //            break;
                //        }
                //    }

                //    /* read Appmanifest from temp folder */
                //    XElement apTempFile = XElement.Load(Path.Combine(tempPath, "AppManifest.xaml"));

                //    //Get all file names from clientbin Folder
                //    //  = Directory.GetFiles(clientBinPath);
                //    string[] clientBinFiles = Directory.GetFiles(clientBinPath, "*.*", SearchOption.AllDirectories);

                //    foreach (String strfile in clientBinFiles)
                //    {

                //        string clientBinFileName = strfile.Replace(clientBinPath, "");
                //        while (clientBinFileName[0] == '\\')
                //        {
                //            clientBinFileName = clientBinFileName.Substring(1);
                //        }

                //        clientBinFileName = clientBinFileName.Replace("\\", "/");

                //        FileInfo fi = new FileInfo(strfile);

                //        if (!zipFile.ContainsEntry(clientBinFileName))
                //        {
                //            SendMsg("Installing File : " + fi.Name);
                //            //Add entry to xap dlls
                //            zipFile.AddEntry(clientBinFileName, fi.OpenRead());

                //            //Read last element of appmenifest
                //            XElement xe = apTempFile.Elements().Last();

                //            //ad attributr to element
                //            XNamespace nameSpace = apTempFile.Attribute(XNamespace.Xmlns + "x").Value;
                //            XNamespace nameSpace2 = apTempFile.Attribute("xmlns").Value;
                //            XElement node = new XElement(nameSpace2 + "AssemblyPart");
                //            XAttribute a2 = new XAttribute(nameSpace + "Name", clientBinFileName.Substring(0, clientBinFileName.Length - 4));
                //            XAttribute a3 = new XAttribute("Source", clientBinFileName);
                //            node.Add(a2);
                //            node.Add(a3);
                //            xe.Add(node);

                //            //set changed flag
                //            isChanged = true;
                //        }
                //        else //OverWrite
                //        {
                //            //Check Version of Imported And installed dll
                //            int vImpID = 0, vInstalledID = 0;
                //            Version vImp = null, vInst = null;
                //            //Get File Version Info
                //            FileVersionInfo asmImported = FileVersionInfo.GetVersionInfo(fi.FullName);
                //            if (asmImported != null)
                //            {
                //                //vImpID = Convert.ToInt32(asmImported.FileVersion.Replace(".", ""));
                //                vImp = new Version(asmImported.FileVersion);
                //            }

                //            foreach (ZipEntry ze in zipFile.Entries)
                //            {
                //                if (ze.FileName == clientBinFileName)
                //                {
                //                    //Extract to temp folder
                //                    ze.Extract(tempPath, ExtractExistingFileAction.OverwriteSilently);
                //                    break;
                //                }
                //            }
                //            //Get file Info
                //            FileVersionInfo asmInstalled = FileVersionInfo.GetVersionInfo(Path.Combine(tempPath, clientBinFileName));
                //            if (asmInstalled != null)
                //            {
                //                // vInstalledID = Convert.ToInt32(asmInstalled.FileVersion.Replace(".", ""));
                //                vInst = new Version(asmInstalled.FileVersion);
                //            }


                //            if (vImp > vInst)
                //            {
                //                SendMsg("Updating file : " + fi.Name);
                //                zipFile.UpdateEntry(clientBinFileName, fi.OpenRead());
                //                isChanged = true;
                //            }
                //            else
                //            {
                //                SendMsg("File already Installed : " + fi.Name);
                //            }
                //            File.Delete(Path.Combine(tempPath, clientBinFileName));
                //        }
                //    }
                //    SendPValue("48");
                //    if (isChanged)
                //    {
                //        //Save XElement to temp folder 
                //        apTempFile.Save(tempPath + "\\AppManifest.xaml");
                //        //read app menifet and udate zip entry
                //        FileStream fs = File.OpenRead(tempPath + "\\AppManifest.xaml");
                //        zipFile.UpdateEntry("AppManifest.xaml", fs);
                //        //save zip entry
                //        zipFile.Save();
                //        //clean up resource
                //        zipFile.Dispose();
                //        //close file
                //        fs.Close();

                //        //create backUp File
                //        File.Copy(AppDomain.CurrentDomain.BaseDirectory + "ClientBin\\ViennaAdvantage.xap", backupPath + "\\ViennaAdvantage.xap", true);
                //        //copy altered xap to clientbin folder
                //        File.Copy(tempPath + "\\ViennaAdvantage.zip", AppDomain.CurrentDomain.BaseDirectory + "\\ClientBin\\ViennaAdvantage.xap", true);
                //        //message
                //        QueryExecuted(new CallBackDetail() { Status = "Module Import Done" });
                //    }
                //    else
                //    {
                //        //clean up resource 
                //        zipFile.Dispose();
                //        QueryExecuted(new CallBackDetail() { Status = "No Module change found" });
                //    }

                //    // message
                //    QueryExecuted(new CallBackDetail() { Status = "Cleaning up " });

                //    //delete temp folder
                //    File.Delete(tempPath + "\\AppManifest.xaml");
                //    File.Delete(tempPath + "\\ViennaAdvantage.zip");

                //    QueryExecuted(new CallBackDetail() { Status = "Silverlight Module Imported" });
                //    QueryExecuted(new CallBackDetail() { Status = "----------------------------Done---------------------------" });
                //    SendPValue("54");

                //}
                //else
                //{
                //    QueryExecuted(new CallBackDetail() { Status = "No updation found[Silverlight]" });
                //}


                /******************* Areas **************************************************/
                //Reports
                if (Directory.Exists(clientAreaPath))
                {
                    SendMsg("----------------------------------------------------------<br />Area Import Start");
                    SendMsg("-----------------------------------------------------------");
                    string baseAreas = basePath + "Areas";
                    DirectoryInfo source = new DirectoryInfo(clientAreaPath);
                    DirectoryInfo destination = new DirectoryInfo(baseAreas);
                    try
                    {
                        MarketSvc.Classes.Utility.CopyAll(source, destination);
                                               
                        //*****************BY SUKHWINDER ON 28-OCT-2016, FOR ENCRYPTING CURRENT CLIENT DEFAULT USERS *******************

                        if (moduleName.Substring(0, moduleName.IndexOf('\\')).ToLower().Equals("vis_"))
                        {
                            var frameworkExportID = Util.GetValueOfString(DB.ExecuteScalar("SELECT EXPORT_ID FROM AD_MODULEINFO WHERE UPPER(NAME) = UPPER('"+ name +"')"));

                            if (frameworkExportID == "VIS_1000011" || frameworkExportID == "VIS_1000021")   // For Beta and Sandbox live.                            
                                EncryptPasswordForDefaultUsers();
                        }                       
                        //*****************END************************************************************************


                        //*****************BY SUKHWINDER ON 28-OCT-2016, FOR COPYING ETL/JAR FILES *******************
                        //// Manish 28/4/2017
                        //CopyETLFiles(source, destination, (moduleName.Substring(0, moduleName.IndexOf('\\'))));

                        //*****************END************************************************************************


                    }
                    catch (Exception ecopy)
                    {
                        SendMsg("Error => " + ecopy.Message);
                        // kept exception for showing in the log after module is installed.
                        sbError.Append("<div class='market-error-message'> <b>Error=> " + ecopy.Message + "</b></div>").Append("<br />");
                    }
                    SendMsg("---------------Done---------------");
                    SendPValue("54");
                }

                else
                {
                    QueryExecuted(new CallBackDetail() { Status = "No updation found[Area]" });
                }

                // Manish 28/4/2017
                DirectoryInfo sourceETL = new DirectoryInfo(clientAreaPath);
                CopyETLFiles(sourceETL,  (moduleName.Substring(0, moduleName.IndexOf('\\'))));

                #endregion
                SendPValue("55");
                #region " CReports"
                /******************* CReports **************************************************/
                //Reports
                if (Directory.Exists(cReportsPath))
                {
                    SendMsg("----------------------------------------------------------<br />Crystal Reports Import Start");
                    SendMsg("-----------------------------------------------------------");
                    string baseReports = basePath + "CReports";
                    DirectoryInfo source = new DirectoryInfo(cReportsPath);
                    DirectoryInfo destination = new DirectoryInfo(baseReports);
                    try
                    {
                        MarketSvc.Classes.Utility.CopyAll(source, destination);
                    }
                    catch (Exception ecopy)
                    {
                        SendMsg("Error => " + ecopy.Message);
                    }
                    SendMsg("---------------Done---------------");
                }
                /******************** END  *******************************************************/
                #endregion
                SendPValue("57");
                //#region SVC
                //if (Directory.Exists(svcPath))
                //{
                //    if (Directory.GetDirectories(svcPath).Count() > 0)
                //    {
                //        SendMsg("----------------------------------------------------------<br /> SVC Import Start");
                //        SendMsg("-----------------------------------------------------------");
                //        string baseSvc = basePath.Substring(0, basePath.Length - 1);
                //        DirectoryInfo source = new DirectoryInfo(svcPath);
                //        DirectoryInfo destination = new DirectoryInfo(baseSvc);
                //        try
                //        {
                //            CopyAll(source, destination);
                //        }
                //        catch (Exception ecopy)
                //        {
                //            SendMsg("ErrorOccured => " + ecopy.Message);
                //        }
                //        SendMsg("---------------Done---------------");
                //    }
                //}


                //#endregion
                SendPValue("59");
            }
            SendPValue("60");
            #region Translation

            ///Check Langcode
            if (langCode == null || langCode.Count() < 1)
            {
                SendMsg("Language code not found");
            }
            else
            {
                if (!Directory.Exists(translationPath))
                {
                    SendMsg("Translation path not found");
                }
                else
                {
                    SendMsg("Translation path found, looking for lang code");
                    SendPValue("61", "Y");
                    string langFullPath = "";
                    foreach (string code in langCode)
                    {
                       // code = code.Replace("\"", "");
                        langFullPath = Path.Combine(translationPath, code.Replace("\"","")); //Full path till lang code [ ../Translation/en_US ]
                        if (!Directory.Exists(langFullPath))
                        {
                            SendMsg("Translation full path not found <==>" + code);
                        }
                        else
                        {
                            SendMsg("Translation start for <==>" + code);

                            TranslationImportModule exp = new TranslationImportModule();
                            exp.Prepare(code.Replace("\"", ""), 0, "I", langFullPath, ctx, this, sbLog);
                            string msg = exp.DoIt();
                            SendMsg(msg);
                            SendMsg("Translation Done for <==>" + code);
                            SendMsg("-------------X----------------------X----------------");
                        }
                    }
                    SendPValue("75");
                    SendMsg(" Syncronizing Start....");
                    VAdvantage.ProcessEngine.ProcessInfo pi = new VAdvantage.ProcessEngine.ProcessInfo();
                    SyncronizeTranslation st = new SyncronizeTranslation();

                    SendPValue("77", "Y");
                    st.StartProcess(ctx, pi, null);

                    SendMsg(pi.GetSummary());
                    SendMsg(" ----X--- Sync End --- X------");
                }
            }



            #endregion
            SendPValue("90");

            Env.Reset(false);	// not final
            SendMsg(" Reset Cache ");
            SendPValue("91");

            #region "Web bin"

            /************************** Web Bin ******************************************************/




            /** Get Module ID */

            string errorMsg = "Could not find Module ID";

            try
            {
                SendMsg("end at => " + DateTime.Now.ToShortTimeString() + " Total[" + (DateTime.Now - dtStart).TotalMinutes + " Mins]");


                moduleID = DB.ExecuteScalar
                    ("SELECT AD_ModuleInfo_ID FROM AD_ModuleInfo WHERE LOWER(Prefix) = '" + prefix.ToLower() + "' AND VersionNo = '" + version + "' AND LOWER(name) ='" + name.ToLower() + "'").ToString();

                //save log

                System.Data.SqlClient.SqlParameter[] para = new System.Data.SqlClient.SqlParameter[1];
                para[0] = new System.Data.SqlClient.SqlParameter("@log", sbLog.ToString());
                int count = VAdvantage.DataBase.DB.ExecuteQuery("UPDATE AD_ModuleInfo SET LogSummary=@log WHERE AD_ModuleInfo_ID = " + moduleID, para, null);
                sbLog.Clear();

            }
            catch (Exception e)
            {
                moduleID = "";
                errorMsg += "<br /> Error <======>" + e.Message + "<br />"
                         + " Reason <===> May not find connection to database or Erroneous Data in xml Files";
            }
            SendPValue("95");

            //callback.QueryExecuted(new CallBackDetail() { Status = "---> Done [ silverlight + data+schema +cReports  <---" });

            if (Directory.Exists(binPath) || Directory.Exists(svcPath))
            {
                //callback.QueryExecuted(new CallBackDetail() { Status = "--> Done  [bin pending] <--", Query = "Redirect", Error = moduleName + "\\bin" });
                //callback.QueryExecuted(new CallBackDetail() { Status = " Done "});
                //callback.QueryExecuted(new CallBackDetail() { Status = moduleID, Action = "Redirect", ActionMsg = moduleName + "\\bin" });
                redirectWithUrl = true;
                // return;
            }

            /************************* END ***********************************************************/

            //callback.QueryExecuted(new CallBackDetail() { Status = "******************<br />All Done" });
            //callback.QueryExecuted(new CallBackDetail() { Status = "******************" });



            SendPValue("98");

            if (onlyTranslation)
            {
                SendMsg("Translation Done", true);
                return null;
            }

            CallBackDetail cb = null;

            if (string.IsNullOrEmpty(moduleID))
            {
                cb = new CallBackDetail() { Status = moduleID, Action = "Error", ActionMsg = errorMsg };
                //SendMsg(errorMsg, true);
            }
            else if (redirectWithUrl)
            {
                cb = new CallBackDetail() { Status = moduleID + "_" + newVersion, Action = "Redirect", ActionMsg = moduleName };
            }
            else
            {
                 cb = new CallBackDetail() { Status = moduleID + "_" + newVersion, Action = "Redirect" };
            }

            //QueryExecuted(cb);
            return cb;
            #endregion
        }

        /// <summary>
        /// FOR ENCRYPTING CURRENT CLIENT DEFAULT USERS
        /// BY SUKHWINDER ON 28-OCT-2016,
        /// </summary>
        private void EncryptPasswordForDefaultUsers()
        {
            List<int> clientIdList = new List<int>();

            DataSet dsClients = DB.ExecuteDataset("SELECT AD_CLIENT_ID FROM AD_CLIENT WHERE ISACTIVE='Y'");

            if (dsClients != null)
            {                
                for (int i = 0; i < dsClients.Tables[0].Rows.Count; i++)
                {
                    if (dsClients.Tables[0].Rows[i]["AD_CLIENT_ID"] != null && dsClients.Tables[0].Rows[i]["AD_CLIENT_ID"] != DBNull.Value)                               
                    {
                        clientIdList.Add(Util.GetValueOfInt(dsClients.Tables[0].Rows[i]["AD_CLIENT_ID"]));                        
                    }                    
                }
            }

            var isPwdEncrypted = Util.GetValueOfString(DB.ExecuteScalar("SELECT ISENCRYPTED FROM AD_COLUMN WHERE  AD_TABLE_ID=" + MTable.Get_Table_ID("AD_User") + " AND ColumnName = 'Password' AND EXPORT_ID = 'VIS_417' AND ISACTIVE='Y'"));

            if (isPwdEncrypted == "Y")
            {
                foreach (int clientID in clientIdList)
                {                    
                   // DataSet dsClientUser = DB.ExecuteDataset("SELECT AD_USER_ID, PASSWORD FROM  (SELECT AD_USER_ID,PASSWORD FROM AD_USER WHERE AD_CLIENT_ID =  " + clientID + "  AND ISLOGINUSER = 'Y' ORDER BY AD_USER_ID  ) WHERE rownum <=3 ");
                    DataSet dsClientUser = DB.ExecuteDataset("SELECT AD_USER_ID, Password FROM (Select AD_USER_ID, PASSWORD, Row_Number() Over (Order By AD_USER_ID) as RowNumber FROM AD_USER WHERE AD_CLIENT_ID =  " + clientID + "  AND ISLOGINUSER = 'Y' AND ISACTIVE='Y' ORDER BY AD_USER_ID  ) temp WHERE RowNumber <=3 ");                    

                    if (dsClientUser != null)
                    {
                        string userId, pwd;
                        for (int j = 0; j < dsClientUser.Tables[0].Rows.Count; j++)
                        {
                            if (dsClientUser.Tables[0].Rows[j]["AD_USER_ID"] != null && dsClientUser.Tables[0].Rows[j]["AD_USER_ID"] != DBNull.Value
                                && dsClientUser.Tables[0].Rows[j]["PASSWORD"] != null && dsClientUser.Tables[0].Rows[j]["PASSWORD"] != DBNull.Value)
                            {
                                userId = dsClientUser.Tables[0].Rows[j]["AD_USER_ID"] != null ? dsClientUser.Tables[0].Rows[j]["AD_USER_ID"].ToString() : "0";
                                pwd = dsClientUser.Tables[0].Rows[j]["PASSWORD"] != null ? dsClientUser.Tables[0].Rows[j]["PASSWORD"].ToString() : "";

                                if ( !string.IsNullOrEmpty(pwd) && !VAdvantage.Utility.SecureEngine.IsEncrypted(pwd))
                                {
                                    int count = VAdvantage.DataBase.DB.ExecuteQuery("UPDATE AD_USER SET PASSWORD='" + VAdvantage.Utility.SecureEngine.Encrypt(pwd) + "' WHERE AD_USER_ID = " + userId);
                                }
                            }
                        }
                    }
                }
            }
        }

        //private void CopyETLFiles(DirectoryInfo src, DirectoryInfo dest, string ModuleName)
        private void CopyETLFiles(DirectoryInfo src, string ModuleName)
        {
            string ETLDestination = WebConfigurationManager.AppSettings["ETLDestinationDirectory"];
           
            try
            {
                if (Directory.Exists(Directory.GetParent(src.FullName).FullName + "\\EtlFiles"))
                {
                    string[] files = System.IO.Directory.GetFiles(Directory.GetParent(src.FullName).FullName + "\\EtlFiles");

                    if (files.Length != 0)
                    {
                        SendMsg("<br /> ETL Files Exists, Copy started");
                        if (!Directory.Exists(ETLDestination))
                        {
                            Directory.CreateDirectory(ETLDestination);
                        }
                        string[] filesAtDest = System.IO.Directory.GetFiles(ETLDestination);
                        if (filesAtDest.Length > 0)
                        {
                            string backUpintoBackup = ETLDestination.TrimEnd('\\') + "_BackUp";
                            if (!Directory.Exists(backUpintoBackup))
                            {
                                Directory.CreateDirectory(backUpintoBackup);
                            }

                            string BackUpFolderName = backUpintoBackup + "\\" + "EtlFiles";
                            if (!Directory.Exists(BackUpFolderName))
                            {
                                //Directory.Delete(BackUpFolderName, true);
                                Directory.CreateDirectory(BackUpFolderName);
                            }


                            foreach (var file in filesAtDest)
                            {
                                if (Path.GetFileName(file).Contains(ModuleName))
                                {
                                    string pathCombine = Path.Combine(BackUpFolderName, Path.GetFileName(file));
                                    if (File.Exists(pathCombine))
                                    {
                                        File.Delete(pathCombine);
                                    }
                                    File.Move(file, Path.Combine(BackUpFolderName, Path.GetFileName(file)));
                                }
                            }
                        }

                        foreach (var file in files)
                        {
                            File.Copy(file, Path.Combine(ETLDestination, Path.GetFileName(file)), true);
                            SendMsg("<br /> File " + Path.GetFileName(file) + "  Copied Successfully :");
                        }



                        SendMsg("<br /> ETL Files DONE ");
                    }
                    else
                    {
                        SendMsg("<br /> No file exists at source location"); ///"No file exists at source location";
                    }
                }
                else
                {
                    SendMsg("<br /> Source path of ETL Files does not exists"); //"-------------Source path of ETL Files does not exists -------------";
                }
            }
            catch (Exception ex)
            {
                SendMsg("Error => " + ex.Message);
            }
        }

        private void SendMsg(string msg)
        {
            SendMsg(msg, false);

        }

        private void SendPValue(string pValue)
        {
            SendPValue(pValue, "N");

        }

        private void SendPValue(string pValue, string show)
        {
            //if (callback != null)
            {
                QueryExecuted(new CallBackDetail() { Status = pValue, Action = "PValue", ActionMsg = show });
            }
        }

        private void SendMsg(string msg, bool isDone)
        {

            QueryExecuted(new CallBackDetail() { Status = msg, Action = isDone ? "Done" : "" });
            //lstCallback.Add(new CallBackDetail() { Status = msg, Action = isDone ? "Done" : "" });


            if (sbLog != null)
            {
                sbLog.Append("<br />").Append(msg);
            }
        }

        public void QueryExecuted(CallBackDetail cbd)
        {
            lock (locklist)
            {
                lstCallback.Add(cbd);
            }
        }

        public bool Done { get; set; }
        public static StringBuilder sbError { get; set; }
    }
}
