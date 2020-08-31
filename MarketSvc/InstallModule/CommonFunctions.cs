using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Hosting;

namespace MarketSvc.InstallModule
{
    public class CommonFunctions
    {
        
        public static void WriteLog(string msg, string fileName)
        {
            File.AppendAllText(HostingEnvironment.ApplicationPhysicalPath + "log\\" + fileName, msg + Environment.NewLine);
        }


    }
}