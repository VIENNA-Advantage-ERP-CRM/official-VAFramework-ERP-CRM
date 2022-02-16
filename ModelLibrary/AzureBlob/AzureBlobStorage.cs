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

        /// <summary>
        /// Upload the specified file to the specified Azure blob container
        /// </summary>
        /// <param name="containerUrl"></param>
        /// <param name="fullFilePath"></param>
        /// <returns>true if succeeded, false if failed</returns>
        public static string UploadFile(Ctx ctx, string containerUrl, string fullFilePath)
        {
            if (!Env.IsModuleInstalled("VA090_"))
            {
                _log.SaveError("AzureBlob", "Module VA090_AzureBlobStorage is not installed!");
                return Msg.GetMsg(ctx, "VIS_VA090NotInstalled");
            }

            Assembly assembly = Assembly.Load("VA090Svc");
            Type type = assembly.GetType("VA090Svc.Classes.AzureBlobStorage");

            object[] param = new object[3];
            param[0] = ctx;
            param[1] = containerUrl;
            param[2] = fullFilePath;
            var resultObj = type.GetMethod("UploadFile").Invoke(null, param);

            if (resultObj != null)
            {
                return resultObj.ToString();
            }
            return null;
        }

        /// <summary>
        /// Download the specified file to the specified Azure blob container
        /// </summary>
        /// <param name="containerUrl"></param>
        /// <param name="downloadFullPath"></param>
        /// <param name="fileName"></param>
        /// <returns>true if succeeded, false if failed</returns>
        public static string DownloadFile(Ctx ctx, string containerUrl, string downloadFullPath, string fileName)
        {

            if (!Env.IsModuleInstalled("VA090_"))
            {
                _log.SaveError("AzureBlob", "Module VA090_AzureBlobStorage is not installed!");
                return Msg.GetMsg(ctx, "VIS_VA090NotInstalled");
            }

            Assembly assembly = Assembly.Load("VA090Svc");
            Type type = assembly.GetType("VA090Svc.Classes.AzureBlobStorage");

            object[] param = new object[4];
            param[0] = ctx;
            param[1] = containerUrl;
            param[2] = downloadFullPath;
            param[3] = fileName;
            var resultObj = type.GetMethod("DownloadFile").Invoke(null, param);

            if (resultObj != null)
            {
                return resultObj.ToString();
            }
            return null;
        }
    }
}
