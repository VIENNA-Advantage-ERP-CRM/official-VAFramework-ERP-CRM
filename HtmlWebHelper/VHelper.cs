using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Data;
using System.Web;
using System.Xml;
using System.ServiceModel;


namespace VWebHelper
{
   public class VHelper
    {
       public static string GetLog(string path1, string id, out string name)
       {
           StringBuilder sb = new StringBuilder("");
           name = "";

           if (path1 != null)
           {
               try
               {
                   string path = "";
                   bool isHttps =false;
                   if (path1.Contains("$Y$"))
                   {
                       path = path1.Substring(0, path1.IndexOf("$Y$"));
                       isHttps = true;
                   }

                   else
                   {
                       path = path1;
                   }

                   string nameVersion = path.Substring(path.IndexOf('\\') + 1);
                   int indexSeprator = nameVersion.LastIndexOf('_');
                   //name = nameVersion.Substring(0, indexSeprator);
                   string newVersion = nameVersion.Substring(indexSeprator + 1); // new version [from Folder ]


                   string basePath = AppDomain.CurrentDomain.BaseDirectory;
                   string modulePathDir = basePath + path + "\\htmlbin";
                   string binDirectory = basePath + "bin";


                   string modulePathSvc = basePath + path + "\\Svc";
                  
                   // string SvcDirectory = basePath;



                   if (Directory.Exists(modulePathDir))
                   {
                       sb.Append("bin Directory found").Append("<br />");
                       string[] binModuleFiles = Directory.GetFiles(modulePathDir);
                       string[] binFiles = Directory.GetFiles(binDirectory);
                       sb.Append("--------Start-------").Append("<br />");
                       foreach (string fileMPath in binModuleFiles)
                       {
                           try
                           {
                               FileInfo fMInfo = new FileInfo(fileMPath);
                               FileInfo fBInfo = new FileInfo(fileMPath);
                               String binFile = binDirectory + "\\" + fMInfo.Name;

                               if (!BinConatinFile(binFiles, fMInfo.Name))
                               {
                                   sb.Append("file not found , inserting :").Append(fMInfo.Name).Append("<br />");
                                   File.Copy(fileMPath, binFile, true);
                                   sb.Append("copy File :").Append(fMInfo.Name).Append("<br />");
                               }
                               else
                               {
                                   sb.Append("Found File :").Append(fMInfo.Name).Append("<br />");
                                   // Decimal vImpID = 0, vInstalledID = 0;
                                   Version vImp = null, vInstall = null;

                                   FileVersionInfo fileImported = FileVersionInfo.GetVersionInfo(fMInfo.FullName);
                                   if (fileImported != null)
                                   {
                                       vImp = new Version(fileImported.FileMajorPart,fileImported.FileMinorPart,fileImported.FileBuildPart,fileImported.FilePrivatePart);
                                       //vImpID = Convert.ToDecimal(fileImported.FileVersion.Replace(".", ""));
                                   }

                                   //Get file Info
                                   FileVersionInfo fileInstalled = FileVersionInfo.GetVersionInfo(Path.Combine(binDirectory, fMInfo.Name));
                                   if (fileInstalled != null)
                                   {
                                       vInstall = new Version(fileInstalled.FileMajorPart, fileInstalled.FileMinorPart, fileInstalled.FileBuildPart, fileInstalled.FilePrivatePart); 
                                       //vInstalledID = Convert.ToDecimal(fileInstalled.FileVersion.Replace(".", ""));
                                   }

                                   if (vImp > vInstall)
                                   {
                                       sb.Append("updating File :").Append(fMInfo.Name).Append("<br />");
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
                                       sb.Append("newer or same version of file :").Append(fMInfo.Name).Append(" exist ").Append("<br />");
                                   }
                               }
                           }
                           catch (Exception exxx)
                           {
                               sb.Append("Error Coping File =>" + fileMPath + " Error=>" + exxx.Message).Append("<br />"); 
                           }
                       }
                       sb.Append("--------Done-------").Append("<br />");
                   }
                   else
                   {
                       sb.Append("[bin] directory not exists").Append("<br />");
                   }

                   //skip svc, need not to copy
                   if (Directory.Exists(modulePathSvc))
                   {
                       bool ignoreHttpsFolder = false;
                       if (Directory.Exists(modulePathSvc + "\\https"))
                       {
                           if (isHttps)
                           {
                               modulePathSvc += "\\https";
                           }
                           else
                           {
                               try
                               {
                                   Directory.Delete(modulePathSvc + "\\https", true);
                                   sb.Append("https folder deleted").Append("<br />");
                               }
                               catch (Exception ex)
                               {
                                   sb.Append("error deleting https folder =>" + ex.Message).Append("<br />");
                                   ignoreHttpsFolder = true;
                               }

                           }
                       }


                       if (Directory.GetDirectories(modulePathSvc).Count() > 0)
                       {

                           sb.Append("Svc Directory found").Append("<br />");

                           string baseSvc = basePath.Substring(0, basePath.Length - 1);
                           DirectoryInfo source = new DirectoryInfo(modulePathSvc);
                           DirectoryInfo destination = new DirectoryInfo(baseSvc);
                           sb.Append("--------Start-------").Append("<br />");
                           sb.Append("coping Svc files").Append("<br />");
                           try
                           {
                               CopyAll(source, destination, ignoreHttpsFolder);
                           }
                           catch (Exception ecopy)
                           {
                               sb.Append("ErrorOccured => " + ecopy.Message).Append("<br />");
                           }
                           sb.Append("-------Done-------").Append("<br />");
                       }
                       else
                       {
                           sb.Append("Svc directory contains no file").Append("<br />");
                       }
                   }
                   else
                   {
                       sb.Append("[Svc] directory not exits").Append("<br />");
                   }
               }
               catch (Exception e)
               {
                   sb.Append(e.Message);
               }
           }
           else
           {
               sb.Append("no updation found[bin,Svc]").Append("<br />");
           }

           if (id != null)
           {

               if (string.IsNullOrEmpty(id))
               {
                   sb.Append("Module ID is null").Append("<br />");

               }
               else
               {
                   try
                   {
                       string pkID = "";
                       string versionNo = "";
                       if (id.Contains("_"))
                       {
                           pkID = id.Substring(0, id.IndexOf("_"));
                           versionNo = id.Substring(id.IndexOf("_") + 1);

                           if (VAdvantage.DataBase.DB.ExecuteQuery("UPDATE AD_ModuleInfo SET VersionNo='" + versionNo + "'  WHERE AD_ModuleInfo_ID = " + pkID) != -1)
                           {
                               sb.Append("<br />").Append("Module Version Updated =>" + versionNo);
                           }
                           else
                           {
                               sb.Append("<br />").Append("Module Version Not Updated =>" + versionNo);
                           }
                       }
                       else
                       {
                           pkID = id;
                       }



                       IDataReader dr = VAdvantage.DataBase.DB.ExecuteReader("SELECT LogSummary,Name FROM AD_ModuleInfo WHERE AD_ModuleInfo_ID = " + pkID);
                       string log = "";
                       if (dr.Read())
                       {
                           name = dr[1].ToString();
                           log = dr[0].ToString();
                       }
                       dr.Close();
                       sb.Insert(0, log + "<br />");
                   }
                   catch (Exception ee)
                   {
                       sb.Append(ee.Message).Append("<br />");
                   }
               }
           }
           else
           {
               sb.Insert(0, "Module ID not found <br /> ");
           }

           //sb.Append(UpdateVesion()).Append("<br />");
           sb.Append("<br />").Append("*************<br />All Done<br />").Append("*************");
           return sb.ToString();
       }

       private static bool CopyAll(DirectoryInfo source, DirectoryInfo target,bool ignorehttpsFolder = false)
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

       public static string BaseURL
       {
           get
           {
               HttpContext context = HttpContext.Current;
               string baseUrl = context.Request.Url.ToString();
               baseUrl = baseUrl.Substring(0, baseUrl.ToLower().LastIndexOf("/default.aspx"));
               baseUrl += "/ViennaAdvantage.aspx";
               return baseUrl;
           }

       }

       //public static string SendMail(object logObject,string tokenKey)
       //{
       //    try
       //    {
       //        string log = Convert.ToString(logObject);

       //        EndpointAddress endPoint = new EndpointAddress("http://cloudservice.softwareonthecloud.com/service.asmx");
       //        BasicHttpBinding binding = new BasicHttpBinding()
       //        {
       //            MaxBufferSize = int.MaxValue,
       //            MaxReceivedMessageSize = int.MaxValue,
       //            OpenTimeout = TimeSpan.FromMinutes(5),
       //            SendTimeout = TimeSpan.FromMinutes(5),
       //            CloseTimeout = TimeSpan.FromMinutes(5),
       //            ReceiveTimeout = TimeSpan.FromMinutes(5),
       //        };
       //        MarketSvc.ServerInfo.ServiceSoapClient clo = new  MarketSvc.ServerInfo.ServiceSoapClient(binding, endPoint);

       //        System.Net.ServicePointManager.Expect100Continue = false;
       //        //var ret =  clo.GetUserInfoOfTokenKey(tokenKey,logObject.ToString(), SecureEngine.Encrypt(System.Web.Configuration.WebConfigurationManager.AppSettings["accesskey"]).ToString());
       //        var ret = clo.GetUserInfoOfTokenKey(tokenKey, logObject.ToString(), MarketSvc.Classes.Utility.GetAccesskey());
       //        clo.Close();
       //        return ret;
       //        //string Subject = String.Empty;
       //        //DataSet ds = new DataSet();
       //        //if (userinfo == null || userinfo == "" || userinfo == "NoDataFound")
       //        //{
       //        //    Subject = "userinfo not found";
       //        //}

       //        //else 
       //        //{
       //        //    byte[] buffer = Encoding.UTF8.GetBytes(userinfo);
       //        //    using (MemoryStream stream = new MemoryStream(buffer))
       //        //    {
       //        //        XmlReader reader = XmlReader.Create(stream);
       //        //        ds.ReadXml(reader);
       //        //    }
       //        //    if (ds == null || ds.Tables[0].Rows.Count == 0)
       //        //    {
       //        //        Subject = "userinfo not found";
       //        //    }
       //        //    else
       //        //    {
       //        //        Subject = "Module Migration Report of user having ID= " + ds.Tables[0].Rows[0]["AD_User_ID"].ToString() + " and UserName= " + ds.Tables[0].Rows[0]["Name"].ToString();
       //        //    }
       //        //    if (ds.Tables[0].Rows[0]["UserINfo"] != null && ds.Tables[0].Rows[0]["UserINfo"].ToString() != "")
       //        //    {
       //        //        Subject += " and FullName= " + ds.Tables[0].Rows[0]["UserINfo"].ToString();
       //        //    }

       //        //}



       //        //VAdvantageSvc.DataServicesBasic2 mail = new VAdvantageSvc.DataServicesBasic2();


       //        //string logs = log;

       //        //VAdvantageSvc.UserInformation user = new VAdvantageSvc.UserInformation();
       //        //user.Username = "viennami";
       //        //user.Password = "viennami";
       //        ////user.UseSSL = ReportSSL.ToString().Equals("Y") ? true : false;
       //        ////user.SmtpAuthentication = ReportSMTP.ToString().Equals("Y") ? true : false;
       //        ////user.Host = ReportHost;


       //        //return mail.SendMail("viennami", "viennami", "viennami", "viennami", Subject, logs, user);
       //    }
       //    catch(Exception ex)
       //    {
       //        return ex.Message;
       //    }
       //}
    }
}
