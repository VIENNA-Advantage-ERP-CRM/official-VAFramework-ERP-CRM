using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Data;
using VAdvantage.Utility;

namespace MarketSvc.Classes
{
    public class Utility
    {


        private static string _key = "caff4eb4fbd6273e37e8a325e19f0991";

        /// <summary>
        /// Get Migration Log and  update version in web config 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetLog(string path, string id, out string name)
        {
            StringBuilder sb = new StringBuilder("");
            name = "";
            try
            {
                if (path != null)
                {
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;
                    string modulePathDir = basePath + path;
                    string binDirectory = basePath + "bin";

                    if (Directory.Exists(modulePathDir))
                    {
                        sb.Append("bin Directory found").Append("\n");
                        string[] binModuleFiles = Directory.GetFiles(modulePathDir);
                        string[] binFiles = Directory.GetFiles(binDirectory);

                        foreach (string fileMPath in binModuleFiles)
                        {
                            FileInfo fMInfo = new FileInfo(fileMPath);
                            FileInfo fBInfo = new FileInfo(fileMPath);
                            String binFile = binDirectory + "\\" + fMInfo.Name;

                            if (!BinConatinFile(binFiles, fMInfo.Name))
                            {

                                File.Copy(fileMPath, binFile);
                                sb.Append("copy File :").Append(fMInfo.Name).Append("\n");
                            }
                            else
                            {
                                int vImpID = 0, vInstalledID = 0;
                                FileVersionInfo fileImported = FileVersionInfo.GetVersionInfo(fMInfo.FullName);
                                if (fileImported != null)
                                {
                                    vImpID = Convert.ToInt32(fileImported.FileVersion.Replace(".", ""));
                                }

                                //Get file Info
                                FileVersionInfo fileInstalled = FileVersionInfo.GetVersionInfo(Path.Combine(binDirectory, fMInfo.Name));
                                if (fileInstalled != null)
                                {
                                    vInstalledID = Convert.ToInt32(fileInstalled.FileVersion.Replace(".", ""));
                                }

                                if (vImpID > vInstalledID)
                                {
                                    sb.Append("update File :").Append(fMInfo.Name).Append("\n");
                                    try
                                    {
                                        File.Copy(fileMPath, binFile, true);
                                    }
                                    catch (Exception eex)
                                    {
                                        sb.Append(eex.Message);
                                    }
                                }
                                else
                                {
                                    sb.Append("newer or same version of file :").Append(fMInfo.Name).Append(" exist ").Append("\n");
                                }
                            }
                        }
                    }
                    else
                    {
                        sb.Append("Directory Not Exist").Append("\n");
                    }
                }

                else
                {
                    sb.Append("no updation found[web]").Append("\n");
                }


                if (id != null)
                {
                    if (string.IsNullOrEmpty(id))
                    {
                        sb.Append("Module ID is null").Append("\n");

                    }
                    else
                    {

                        IDataReader dr = VAdvantage.DataBase.DB.ExecuteReader("SELECT LogSummary,Name FROM AD_ModuleInfo WHERE AD_ModuleInfo_ID = " + id);
                        string log = "";
                        if (dr.Read())
                        {
                            name = dr[1].ToString();
                            log = dr[0].ToString();
                        }
                        dr.Close();
                        sb.Insert(0, log);
                    }
                }
                else
                {
                    sb.Insert(0, "Module ID not found");
                }
                sb.Append("******************\nAll Done\n").Append("******************");
            }
            catch (Exception e)
            {
                sb.Append(e.Message);
            }


            sb.Append(UpdateVesion());
            return sb.ToString();
        }

        /// <summary>
        /// Update Version in web Config
        /// </summary>
        private static string UpdateVesion()
        {
            try
            {
                System.Configuration.Configuration rootWebConfig1 =
                    System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                if (rootWebConfig1.AppSettings.Settings.Count > 0)
                {
                    System.Configuration.KeyValueConfigurationElement customSetting =
                        rootWebConfig1.AppSettings.Settings["UpdateVersion"];
                    if (customSetting != null)
                    {
                        int version = -1;
                        try
                        {
                            version = int.Parse(customSetting.Value);
                        }
                        catch
                        {
                        }
                        customSetting.Value = (version + 1).ToString();
                    }
                    else
                    {
                        rootWebConfig1.AppSettings.Settings.Add("UpdateVersion", "0");
                    }
                    rootWebConfig1.Save();
                }
                return "web config version updated";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private static bool BinConatinFile(string[] binFiles, string fileName)
        {
            FileInfo binFileInfo = null;
            foreach (string binFile in binFiles)
            {
                binFileInfo = new FileInfo(binFile);
                if (binFileInfo.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                binFileInfo = null;
            }
            binFileInfo = null;
            return false;
        }

        public static string GetAccesskey()
        {
            //string url = "";
            //try
            //{
            //    url = SecureEngine.Encrypt(System.Configuration.ConfigurationManager.AppSettings["accesskey"].ToString());

            //}
            //catch { }
            //return url;
            return _key;
        }
        
        public static bool ShowUnReleasedModule()
        {
            if (System.Web.Configuration.WebConfigurationManager.AppSettings["ShowUnReleasedModule"] != null && System.Web.Configuration.WebConfigurationManager.AppSettings["ShowUnReleasedModule"].ToString() != "")
            {
                if (System.Web.Configuration.WebConfigurationManager.AppSettings["ShowUnReleasedModule"].ToString() == "Y")
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool CopyAll(DirectoryInfo source, DirectoryInfo target, bool ignorehttpsFolder = false)
        {
            //try
            //{
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }
            foreach (FileInfo fi in source.GetFiles())
            {

                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            foreach (DirectoryInfo diSourceDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetDir;

                if (ignorehttpsFolder && diSourceDir.Name.Equals("https", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }


                if (!Directory.Exists(target.FullName + "\\" + diSourceDir.Name))
                {
                    nextTargetDir = target.CreateSubdirectory(diSourceDir.Name);
                }
                else
                {
                    nextTargetDir = new DirectoryInfo(target.FullName + "\\" + diSourceDir.Name);
                }
                CopyAll(diSourceDir, nextTargetDir);
            }
            return true;
            //}
            //catch (Exception ie)
            //{
            //    return false;
            //}
        }
    }
}
