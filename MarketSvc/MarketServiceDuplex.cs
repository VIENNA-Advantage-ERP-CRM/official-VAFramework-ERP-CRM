using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.IO;
using VAdvantage.Utility;
using Ionic.Zip;
using System.Xml.Linq;
using System.Diagnostics;
using VAdvantage.DataBase;
using System.Data;
using MarketSvc.MService;


namespace MarketSvc
{

    [ServiceContract(Namespace = "", CallbackContract = typeof(IMarketCallback))]
    public interface IMarketServiceDuplex
    {
        [OperationContract(IsOneWay = true)]
        void ImportModuleFromMarket(int AD_ModuleInfo_ID, int[] clientIds, string[] langCode, bool onlyTranslation, IDictionary<string, string> ctxDic);

        [OperationContract(IsOneWay = true)]
        void ImportModuleFromLocalFile(String moduleName, int[] clientIds, string[] langCode, bool onlyTranslation, IDictionary<string, string> ctxDic);

        [OperationContract(IsOneWay = true)]
        void CheckModuleFolder(String moduleName);

    }

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    //[ServiceBehavior(InstanceContextMode =  InstanceContextMode.Single,ConcurrencyMode = ConcurrencyMode.Single)]   
    public class MarketServiceDuplex : IMarketServiceDuplex
    {
        private string connectionString = System.Configuration.ConfigurationManager.AppSettings["oracleConnectionString"];

        IMarketCallback callback = null;
        StringBuilder sbLog = new StringBuilder(""); 


        #region Module

        /// <summary>
        /// Import Module data and dll's
        /// </summary>
        /// <param name="AD_ModuleInfo_ID">Module ID</param>
        public void ImportModuleFromMarket(int AD_ModuleInfo_ID, int[] AD_Client_Ids, string[] langCode, bool onlyTranslation, IDictionary<string, string> ctxDic)
        {
             sbLog = new StringBuilder(""); 

            try
            {
                /* Callback Contract */

                //OperationContext.Current.OutgoingMessageProperties.AllowOutputBatching = true;
                // OperationContext.Current.Channel.OperationTimeout = new TimeSpan(5, 0, 0);

                callback = OperationContext.Current.GetCallbackChannel<IMarketCallback>();
                
                SendMsg("* Creating Client Proxy *" );
                

                /* Market Service */
                var client = ServiceEndPoint.GetMServiceClient();
                /* Message */
                //callback.QueryExecuted(new CallBackDetail() { Status = "Proxy Created, Getting data" });
                SendMsg("Proxy Created, Getting data");
                //sbLog.Append("\n").Append(" Proxy Created, Getting data");



                SendPValue("2","Y");
                /* Call market service to get info data against module */
                //MarketSvc.MService.ModuleFolderInfo data = client.GetModuleData(AD_ModuleInfo_ID);
                System.Net.ServicePointManager.Expect100Continue = false;
                MarketSvc.MService.ModuleFolderInfo data = client.GetModuleData(AD_ModuleInfo_ID, MarketSvc.Classes.Utility.GetAccesskey());
                client.Close();
                SendPValue("4");
                if (data == null)
                {
                    SendMsg("Error getting Data, Operation aborted, Please try after some time", true);
                    return;
                }
                else if (data.ISError)
                {
                    SendMsg(data.ErrorMessage, true);
                    return;
                }

                /*Message */
                //callback.QueryExecuted(new CallBackDetail() { Status = "Data Fetched" });
                SendMsg("Data Fetched");

                //callback.QueryExecuted(new CallBackDetail() { Status = "Extracting file in application folder" });
                SendMsg("Extracting file in application folder");


                SendPValue("8");
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string fileTempPath = basePath + data.Prefix + DateTime.Now.Ticks;
                string filePath = basePath + data.Prefix;
                try
                {
                    File.WriteAllBytes(fileTempPath, data.Data);
                    ICSharpCode.SharpZipLib.Zip.FastZip z = new ICSharpCode.SharpZipLib.Zip.FastZip();
                    z.ExtractZip(fileTempPath, basePath + data.Prefix, "");
                    File.Delete(fileTempPath);
                    SendPValue("10");
                }
                catch
                {
                    //callback.QueryExecuted(new CallBackDetail() { Status = "Error while extracting folder/data", Action = "Done" });
                    SendMsg("Error while extracting folder/data", true);

                    return;
                }

                //callback.QueryExecuted(new CallBackDetail() { Status = "Folder extracted at " + filePath });

                SendMsg("Folder extracted at " + filePath); 

                SendMsg("Module Integration Start");
                string moduleLocalPath = data.Prefix + "\\" + data.Name.Trim() + "_" + data.VersionNo;

                //Call Local Import Procedure
                ImportModuleFromLocalFile(moduleLocalPath, AD_Client_Ids, langCode, onlyTranslation, ctxDic);

                #region Comment

                //callback.QueryExecuted(new CallBackDetail() { Status = "Data Fetched", Action = "Done" });

                //**************************************Data Import //**********************************/

                //callback.QueryExecuted(new CallBackDetail() { Status = "--> Data Export Pending <--" });

                //callback.QueryExecuted(new CallBackDetail() { Status = "Done", Query="Done" });
                //**************************************** end ******************************************/

                ///**************************************** SILVERLIGHT **********************************\
                ////Client BIN Silverlight

                ///* Backup folder path */
                //string backupPath = AppDomain.CurrentDomain.BaseDirectory + "Backup";
                ///* Temp folder path */
                //string tempPath = backupPath + "\\Temp";
                ///* isChanged */
                //bool isChanged = false;

                ///* if module has dll's */
                //if (data.ClientBinDll != null && data.ClientBinDll.Count > 0)
                //{
                //    /* check Backup directory */
                //    if (!Directory.Exists(backupPath))
                //    {
                //        /* contain last before module update */
                //        Directory.CreateDirectory(backupPath);
                //        /* Temp directory for modification */
                //        Directory.CreateDirectory(tempPath);
                //        /* message */
                //        callback.QueryExecuted(new CallBackDetail() { Status = "backup directory created" });
                //    }
                //    //Message
                //    callback.QueryExecuted(new CallBackDetail() { Status = "coping xap file in Backup and temp directory" });
                //    /* copy xap to Temp folder as zip */
                //    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "ClientBin\\ViennaAdvantage.xap", tempPath + "\\ViennaAdvantage.zip", true);
                //    //Message
                //    callback.QueryExecuted(new CallBackDetail() { Status = "done" });
                //    callback.QueryExecuted(new CallBackDetail() { Status = "module integration start" });

                //    //read Zip file
                //    ZipFile file = ZipFile.Read(tempPath + "\\ViennaAdvantage.zip");
                //    //find Appmanifest file
                //    foreach (ZipEntry ze in file.Entries)
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

                //    foreach (KeyValuePair<string, byte[]> pair in data.ClientBinDll)
                //    {
                //        if (!file.ContainsEntry(pair.Key))
                //        {
                //            //Add entry to xap dlls
                //            file.AddEntry(pair.Key, pair.Value);

                //            //Read last element of appmenifest
                //            XElement xe = apTempFile.Elements().Last();

                //            //ad attributr to element
                //            XNamespace nameSpace = apTempFile.Attribute(XNamespace.Xmlns + "x").Value;
                //            XNamespace nameSpace2 = apTempFile.Attribute("xmlns").Value;
                //            XElement node = new XElement(nameSpace2 + "AssemblyPart");
                //            XAttribute a2 = new XAttribute(nameSpace + "Name", pair.Key.Substring(0, pair.Key.Length - 4));
                //            XAttribute a3 = new XAttribute("Source", pair.Key);
                //            node.Add(a2);
                //            node.Add(a3);
                //            xe.Add(node);

                //            //set the hanged flag
                //            isChanged = true;
                //        }
                //    }

                //    if (isChanged)
                //    {
                //        //Save XElement to temp folder 
                //        apTempFile.Save(tempPath + "\\AppManifest.xaml");
                //        //read app menifet and udate zip entry
                //        FileStream fs = File.OpenRead(tempPath + "\\AppManifest.xaml");
                //        file.UpdateEntry("AppManifest.xaml", fs);
                //        //save zip entry
                //        file.Save();
                //        //clean up resource
                //        file.Dispose();
                //        //close file
                //        fs.Close();

                //        //create backUp File
                //        File.Copy(AppDomain.CurrentDomain.BaseDirectory + "ClientBin\\ViennaAdvantage.xap", backupPath + "\\ViennaAdvantage.xap", true);
                //        //copy altered xap to clientbin folder
                //        File.Copy(tempPath + "\\ViennaAdvantage.zip", AppDomain.CurrentDomain.BaseDirectory + "\\ClientBin\\ViennaAdvantage.xap", true);
                //        //message
                //        callback.QueryExecuted(new CallBackDetail() { Status = "Module Import Done" });
                //    }
                //    else
                //    {
                //        //clean up resource 
                //        file.Dispose();
                //        callback.QueryExecuted(new CallBackDetail() { Status = " No Module Change Found" });
                //    }

                //    // message
                //    callback.QueryExecuted(new CallBackDetail() { Status = "Cleaning up " });

                //    //delete temp folder
                //    File.Delete(tempPath + "\\AppManifest.xaml");
                //    File.Delete(tempPath + "\\ViennaAdvantage.zip");

                //    callback.QueryExecuted(new CallBackDetail() { Status = "==> silverlight Module Imported <==" });
                //}

                ///**************************** END SILVERLIGHT *********************************/


                ///************************* BIN WEB *******************************************/
                //isChanged = false;
                //if (data.BinDll.Count > 0)
                //{
                //    callback.QueryExecuted(new CallBackDetail() { Status = "Helper Dll import start " });

                //    string webBinpathFolder = AppDomain.CurrentDomain.BaseDirectory + "bin";
                //    string[] filesNames = Directory.GetFiles(webBinpathFolder);

                //    callback.QueryExecuted(new CallBackDetail() { Status = "Getting Bin Directory" });
                //    foreach (KeyValuePair<String, Byte[]> bPair in data.BinDll)
                //    {
                //        if (!filesNames.Contains(webBinpathFolder + "\\" + bPair.Key))
                //        {
                //            isChanged = true;
                //            FileStream fs = new FileStream(webBinpathFolder + "\\" + bPair.Key, FileMode.OpenOrCreate);
                //            MemoryStream ms = new MemoryStream(bPair.Value);
                //            ms.WriteTo(fs);
                //            ms.Close();
                //            fs.Close();
                //        }
                //    }
                //    if (isChanged)
                //    {
                //        callback.QueryExecuted(new CallBackDetail() { Status = "Helper Dll import Done " });
                //    }
                //    callback.QueryExecuted(new CallBackDetail() { Status = "No change found in  HelperDll " });
                //}

                //callback.QueryExecuted(new CallBackDetail() { Status = "Module Imported", Query = "Done" });
                ///************************* END BIN ******************************************/

                //return "Done";
                #endregion
            }
            catch (Exception e)
            {
                try
                {
                    callback.QueryExecuted(new CallBackDetail() { Status = e.Message, Action = "Done" });
                   
                }
                catch
                {
                    
                }
            }
        }

        #region Import From Local File

        /// <summary>
        /// Import from Local 
        /// </summary>
        /// <param name="moduleName">Module Name</param>
        /// <param name="clientIds">client Id Array</param>
        /// <param name="langCode">Lang Codes</param>
        /// <param name="onlyTranslation">Only translate</param>
        /// <param name="ctxDic"> client context </param>
        public void ImportModuleFromLocalFile(string moduleName, int[] clientIds, string[] langCode, bool onlyTranslation, IDictionary<string, string> ctxDic)
        {
            callback = OperationContext.Current.GetCallbackChannel<IMarketCallback>();            
            string moduleID;
            string prefix,  name, newVersion,version;
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
                callback.QueryExecuted(new CallBackDetail() { Status = "Done", Action = "Error", ActionMsg = "Error parsing "+ moduleName +" Or getting module ID" });
                return;
            }

            

            Ctx ctx = new Ctx(ctxDic);

            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            SendPValue("12");
            SendMsg("Checking module folder");

            if (!Directory.Exists(basePath + moduleName))
            {
                SendMsg("Module folder not found", true);
                return;
            }

            /*clientBinPath*/
            string clientBinPath = basePath + moduleName + "\\ClientBin";
            /*bin Path*/
            string binPath = basePath + moduleName + "\\bin";
            /* Databse Schema Path */
            string dbSchemaPath = basePath + moduleName + "\\DataBaseSchema";
            /* cReports Path */
            string cReportsPath = basePath + moduleName + "\\CReports";
            /* Svc Path */
            string svcPath = basePath + moduleName + "\\Svc";
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
                        SendMsg("----------------------------------\n  Importing Schema \n ----------------------------------");
                        SendPValue("17", "Y");

                        importmodule.Init(0, dbSchemaPath, sbLog);
                        importmodule.CurrentModuleVersion = version;

                        for (int i = 0; i < clientIds.Length; i++)
                        {
                            SendMsg(" Start for  ===> " +clientIds[i]);
                            importmodule.Prepare(clientIds[i], i==0,i==clientIds.Length-1);
                             // Set Current Database Version 
                            importmodule.DoIt(ctx, callback);
                            SendMsg(" Done for ====>" + clientIds[i]);
                        }
                        SendMsg("------- Schema Imported -------");
                        importmodule.Clear();
                    }
                    SendPValue("25");
                    if (Directory.Exists(datapath))
                    {
                        SendMsg("----------------------------------\n Importing Data \n ----------------------------------");
                        SendPValue("27", "Y");

                        importmodule.Init(0, datapath, sbLog);
                        //importmodule.CurrentModuleVersion = version;

                        for (int i = 0; i < clientIds.Length; i++)
                        {
                            SendMsg(" Start for  ===> " + clientIds[i]);
                            importmodule.Prepare(clientIds[i], i == 0, i == clientIds.Length - 1);
                            importmodule.DoIt(ctx, callback);
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

                if (Directory.Exists(clientBinPath))
                {

                    callback.QueryExecuted(new CallBackDetail()
                    {
                        Status = "-----------------------------------------------------------------------"
                    });
                    callback.QueryExecuted(new CallBackDetail() { Status = "Silverlight Import Start\n--------------------------------------------------------------------" });
                    //Client BIN Silverlight
                    /* Backup folder path */
                    string backupPath = AppDomain.CurrentDomain.BaseDirectory + "XapBackupFile";
                    /* Temp folder path */
                    string tempPath = backupPath + "\\Temp";
                    /* isChanged */
                    bool isChanged = false;
                    /* check Backup directory */
                    if (!Directory.Exists(backupPath))
                    {
                        /* contain last before module update */
                        Directory.CreateDirectory(backupPath);
                        /* Temp directory for modification */
                        Directory.CreateDirectory(tempPath);
                        /* message */
                        callback.QueryExecuted(new CallBackDetail() { Status = "Backup directory created" });
                    }
                    //Message
                    callback.QueryExecuted(new CallBackDetail() { Status = "Coping xap file in backup and temp directory" });
                    /* copy xap to Temp folder as zip */
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "ClientBin\\ViennaAdvantage.xap", tempPath + "\\ViennaAdvantage.zip", true);
                    //Message
                    callback.QueryExecuted(new CallBackDetail() { Status = "done" });
                    callback.QueryExecuted(new CallBackDetail() { Status = "Module integration start" });

                    //read Zip file
                    ZipFile zipFile = ZipFile.Read(tempPath + "\\ViennaAdvantage.zip");
                    //find Appmanifest file
                    foreach (ZipEntry ze in zipFile.Entries)
                    {
                        if (ze.FileName == "AppManifest.xaml")
                        {
                            //Extract to temp folder
                            ze.Extract(tempPath, ExtractExistingFileAction.OverwriteSilently);
                            break;
                        }
                    }

                    /* read Appmanifest from temp folder */
                    XElement apTempFile = XElement.Load(Path.Combine(tempPath, "AppManifest.xaml"));

                    //Get all file names from clientbin Folder
                   //  = Directory.GetFiles(clientBinPath);
                   string[] clientBinFiles = Directory.GetFiles(clientBinPath, "*.*", SearchOption.AllDirectories);

                    foreach (String strfile in clientBinFiles)
                    {

                        string clientBinFileName = strfile.Replace(clientBinPath, "");
                        while (clientBinFileName[0] == '\\')
                        {
                            clientBinFileName = clientBinFileName.Substring(1);
                        }

                        clientBinFileName = clientBinFileName.Replace("\\", "/");

                        FileInfo fi = new FileInfo(strfile);

                        if (!zipFile.ContainsEntry(clientBinFileName)) 
                        {
                            SendMsg("Installing File : " + fi.Name);
                            //Add entry to xap dlls
                            zipFile.AddEntry(clientBinFileName, fi.OpenRead());

                            //Read last element of appmenifest
                            XElement xe = apTempFile.Elements().Last();

                            //ad attributr to element
                            XNamespace nameSpace = apTempFile.Attribute(XNamespace.Xmlns + "x").Value;
                            XNamespace nameSpace2 = apTempFile.Attribute("xmlns").Value;
                            XElement node = new XElement(nameSpace2 + "AssemblyPart");
                            XAttribute a2 = new XAttribute(nameSpace + "Name", clientBinFileName.Substring(0, clientBinFileName.Length - 4));
                            XAttribute a3 = new XAttribute("Source", clientBinFileName);
                            node.Add(a2);
                            node.Add(a3);
                            xe.Add(node);

                            //set changed flag
                            isChanged = true;
                        }
                        else //OverWrite
                        {
                            //Check Version of Imported And installed dll
                           // int vImpID = 0, vInstalledID = 0;
                            Version vImp = null, vInst=null;
                            //Get File Version Info
                            FileVersionInfo asmImported = FileVersionInfo.GetVersionInfo(fi.FullName);
                            if (asmImported != null)
                            {
                                //vImpID = Convert.ToInt32(asmImported.FileVersion.Replace(".", ""));
                                vImp = new Version(asmImported.FileVersion);
                            }

                            foreach (ZipEntry ze in zipFile.Entries)
                            {
                                if (ze.FileName == clientBinFileName)
                                {
                                    //Extract to temp folder
                                    ze.Extract(tempPath, ExtractExistingFileAction.OverwriteSilently);
                                    break;
                                }
                            }
                            //Get file Info
                            FileVersionInfo asmInstalled = FileVersionInfo.GetVersionInfo(Path.Combine(tempPath, clientBinFileName));
                            if (asmInstalled != null)
                            {
                               // vInstalledID = Convert.ToInt32(asmInstalled.FileVersion.Replace(".", ""));
                                vInst = new Version(asmInstalled.FileVersion);
                            }


                            if (vImp > vInst)
                            {
                                SendMsg("Updating file : " + fi.Name);
                                zipFile.UpdateEntry(clientBinFileName, fi.OpenRead());
                                isChanged = true;
                            }
                            else
                            {
                                SendMsg("File already Installed : " + fi.Name);
                            }
                            File.Delete(Path.Combine(tempPath, clientBinFileName));
                        }
                    }
                    SendPValue("48");
                    if (isChanged)
                    {
                        //Save XElement to temp folder 
                        apTempFile.Save(tempPath + "\\AppManifest.xaml");
                        //read app menifet and udate zip entry
                        FileStream fs = File.OpenRead(tempPath + "\\AppManifest.xaml");
                        zipFile.UpdateEntry("AppManifest.xaml", fs);
                        //save zip entry
                        zipFile.Save();
                        //clean up resource
                        zipFile.Dispose();
                        //close file
                        fs.Close();

                        //create backUp File
                        File.Copy(AppDomain.CurrentDomain.BaseDirectory + "ClientBin\\ViennaAdvantage.xap", backupPath + "\\ViennaAdvantage.xap", true);
                        //copy altered xap to clientbin folder
                        File.Copy(tempPath + "\\ViennaAdvantage.zip", AppDomain.CurrentDomain.BaseDirectory + "\\ClientBin\\ViennaAdvantage.xap", true);
                        //message
                        callback.QueryExecuted(new CallBackDetail() { Status = "Module Import Done" });
                    }
                    else
                    {
                        //clean up resource 
                        zipFile.Dispose();
                        callback.QueryExecuted(new CallBackDetail() { Status = "No Module change found" });
                    }

                    // message
                    callback.QueryExecuted(new CallBackDetail() { Status = "Cleaning up " });

                    //delete temp folder
                    File.Delete(tempPath + "\\AppManifest.xaml");
                    File.Delete(tempPath + "\\ViennaAdvantage.zip");

                    callback.QueryExecuted(new CallBackDetail() { Status = "Silverlight Module Imported" });
                    callback.QueryExecuted(new CallBackDetail() { Status = "----------------------------Done---------------------------" });
                    SendPValue("54");

                }
                else
                {
                    callback.QueryExecuted(new CallBackDetail() { Status = "No updation found[Silverlight]" });
                }

                #endregion
                SendPValue("55");
                #region " CReports"
                /******************* CReports **************************************************/
                //Reports
                if (Directory.Exists(cReportsPath))
                {
                    SendMsg("----------------------------------------------------------\nCrystal Reports Import Start");
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
                        SendMsg("Error => "+ecopy.Message);
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
                //        SendMsg("----------------------------------------------------------\n SVC Import Start");
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
                        langFullPath = Path.Combine(translationPath, code); //Full path till lang code [ ../Translation/en_US ]
                        if (!Directory.Exists(langFullPath))
                        {
                            SendMsg("Translation full path not found <==>" + code);
                        }
                        else
                        {
                            SendMsg("Translation start for <==>" + code);

                            TranslationImportModule exp = new TranslationImportModule();
                            exp.Prepare(code, 0, "I", langFullPath, ctx, callback,sbLog);
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

                    SendPValue("77","Y");
                    st.StartProcess(ctx, pi, null);

                    SendMsg(pi.GetSummary());
                    SendMsg(" ----X--- Sync End --- X------");
                }
            }



            #endregion
            SendPValue("90");

            #region "Web bin"

            /************************** Web Bin ******************************************************/

            /** Get Module ID */
            
           string  errorMsg = "Could not find Module ID";

            try
            {
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
                errorMsg += "\n Error <======>" + e.Message + "\n"
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

            //callback.QueryExecuted(new CallBackDetail() { Status = "******************\nAll Done" });
            //callback.QueryExecuted(new CallBackDetail() { Status = "******************" });


            SendPValue("98");

            if (onlyTranslation)
            {
                SendMsg("Translation Done", true);
            }

            else if (string.IsNullOrEmpty(moduleID))
            {
                callback.QueryExecuted(new CallBackDetail() { Status = moduleID, Action = "Error", ActionMsg = errorMsg });
            }
            else if (redirectWithUrl)
            {
                callback.QueryExecuted(new CallBackDetail() { Status = moduleID + "_" + newVersion, Action = "Redirect", ActionMsg = moduleName });
            }
            else
            {
                callback.QueryExecuted(new CallBackDetail() { Status = moduleID + "_" + newVersion, Action = "Redirect" });
            }


            #endregion
        }

        //private void Export(Ctx ctx)
        //{
        //    callback = OperationContext.Current.GetCallbackChannel<IMarketCallback>();
        //    callback.QueryExecuted(new CallBackDetail(){ Status ="stating"});
        //    TranslationImportExport exp = new TranslationImportExport();
        //    exp.Prepare("de_DE",0,"E",0,"D:\\Exp",ctx,"INV_");
        //    string msg = exp.DoIt();
        //    callback.QueryExecuted(new CallBackDetail() { Status = "Done" });
        //    return;
        //}

        #endregion

        private void SendMsg(string msg)
        {
            SendMsg(msg, false);

        }

        private void SendPValue(string pValue)
        {
            SendPValue(pValue, "N");

        }

        private void SendPValue(string pValue,string show)
        {
            if (callback != null)
            {
                callback.QueryExecuted(new CallBackDetail() { Status = pValue, Action = "PValue",ActionMsg =show });
            }
        }


        private void SendMsg(string msg, bool isDone)
        {
            if (callback != null)
            {
                callback.QueryExecuted(new CallBackDetail() { Status = msg, Action = isDone ? "Done" : "" });
            }

            if (sbLog != null)
            {
                sbLog.Append("\n").Append(msg);
            }
        }

        //private bool CopyAll(DirectoryInfo source, DirectoryInfo target)
        //{
        //    //try
        //    //{
        //        if (Directory.Exists(target.FullName) == false)
        //        {
        //            Directory.CreateDirectory(target.FullName);
        //        }
        //        foreach (FileInfo fi in source.GetFiles())
        //        {

        //            fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
        //        }

        //        foreach (DirectoryInfo diSourceDir in source.GetDirectories())
        //        {
        //            DirectoryInfo nextTargetDir;
        //            if (!Directory.Exists(target.FullName + "\\" + diSourceDir.Name))
        //            {
        //                nextTargetDir = target.CreateSubdirectory(diSourceDir.Name);
        //            }
        //            else
        //            {
        //                nextTargetDir = new DirectoryInfo(target.FullName + "\\" + diSourceDir.Name);
        //            }
        //            CopyAll(diSourceDir, nextTargetDir);
        //        }
        //        return true;
        //    //}
        //    //catch (Exception ie)
        //    //{
        //    //    return false;
        //    //}
        //} 

        #endregion



        public void CheckModuleFolder(string modulePath)
        {
            try
            {
                callback = OperationContext.Current.GetCallbackChannel<IMarketCallback>();
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                SendMsg("Checking Module folder");
                string fullpath = modulePath;

                if (Directory.Exists(modulePath))
                {
                    SendMsg("Module Folder found at <==>" + modulePath);
                    //return;
                }
                else if (Directory.Exists(basePath + modulePath))
                {
                    SendMsg("Module folder found in Application folder <====>" + modulePath);
                    fullpath = basePath + modulePath;
                }

                else
                {
                    callback.QueryExecuted(new CallBackDetail() { Action = "Error",  Status = "Module folder found not found",ActionMsg= "Module folder found not found" });
                    //SendMsg("Module folder found not found",true);
                    return;
                }

                string tPath = fullpath + "\\Translations";

                string lang = null;
                if (Directory.Exists(tPath))
                {

                    string[] dir = Directory.GetDirectories(tPath);
                    foreach (String str1 in dir)
                    {
                        string str = str1.Substring(str1.LastIndexOf('\\') + 1);

                        if (string.IsNullOrEmpty(lang))
                        {
                            lang += "'" + str + "'";
                        }
                        else
                        {
                            lang += ",'" + str + "'";
                        }
                    }
                    callback.QueryExecuted(new CallBackDetail() { Action = "Done", ActionMsg = lang, Status = "Language Tarnslation Found" });
                }
                else
                {
                    callback.QueryExecuted(new CallBackDetail() { Action = "Done", ActionMsg = "",Status = "No Translation available for module" });
                }
            }
            catch (Exception e)
            {
                callback.QueryExecuted(new CallBackDetail() { Action = "Error", ActionMsg = "Error", Status = e.Message });
            }
        }
    }
            
   
    
    public interface IMarketCallback
    {
        [OperationContract(IsOneWay = true)]
        void QueryExecuted(CallBackDetail cbd);
    }

    public class CallBackDetail
    {
        public String Status { get; set; }
        public String Action { get; set; }
        public String ActionMsg { get; set; }
    }

}
