using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.AzureBlob
{
    public static class AzureBlobStorage
    {
        private static VLogger _log = VLogger.GetVLogger(typeof(AzureBlobStorage).FullName);

        public static string UploadFile(string fullFilePath)
        {
            //if (!Env.IsModuleInstalled("VA090_"))
            //{
            //    _log.SaveError("AzureBlob", "Module VA090_AzureBlobStorage is not installed!");
            //    return "Module VA090_AzureBlobStorage is not installed!";
            //}

            //try
            //{
            Assembly assembly = Assembly.Load("VA090Svc");
            Type type = assembly.GetType("VA090Svc.Classes.AzureBlobStorage");

            //if (type != null)
            //{
            object[] param = new object[1];
            param[0] = fullFilePath;
            var resultObj = type.GetMethod("UploadFile").Invoke(null, param);
            Exception result = (Exception)resultObj;
            return "";
            //    }
            //    return "Type is null";
            //}
            //catch (Exception ex)
            //{
            //    _log.SaveError("AzureBlob", "Exception: " + ex.Message);
            //    return ex.Message;
            //}
        }
        public static string DownloadFile(string containerUrl, string downloadFullPath, string fileName)
        {
            //if (!Env.IsModuleInstalled("VA090_"))
            //{
            //    _log.SaveError("AzureBlob", "Module VA090_AzureBlobStorage is not installed!");
            //    return "Module VA090_AzureBlobStorage is not installed!";
            //}

            //try
            //{
            Assembly assembly = Assembly.Load("VA090Svc");
            Type type = assembly.GetType("VA090Svc.Classes.AzureBlobStorage");

            //if (type != null)
            //{
            object[] param = new object[3];
            param[0] = containerUrl;
            param[1] = downloadFullPath;
            param[2] = fileName;
            var resultObj = type.GetMethod("DownloadFile").Invoke(null, param);
            Exception result = (Exception)resultObj;
            return "";
            //    }
            //    return "Type is null";
            //}
            //catch (Exception ex)
            //{
            //    _log.SaveError("AzureBlob", "Exception: " + ex.Message);
            //    return ex.Message;
            //}
        }
    }
}
