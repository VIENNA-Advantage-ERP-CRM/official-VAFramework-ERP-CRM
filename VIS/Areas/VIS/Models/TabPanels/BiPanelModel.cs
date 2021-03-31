using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using VAdvantage.BiReport;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Model
{

    public class BiPanelModel
    {
        private static CCache<string, Type> cache = new CCache<string, Type>("BiPanelModelType", 30, 60);
        VLogger log = VLogger.GetVLogger(typeof(BiPanelModel).FullName);
        Ctx ctx = null;
        public BiPanelModel(Ctx ctx)
        {
            this.ctx = ctx;
        }

        public List<string> GetUserBILogin()
        {
            List<string> lstString = new List<string>();
            // add this to utiiy class

            BiReportEngine com = new BiReportEngine();
            lstString = com.GetUserBILogin(ctx, log);


            //Type type = (Type)cache.Get("VA037Svc");

            //if (type == null)
            //{
            //    Assembly assem = Assembly.Load("VA037Svc");
            //    type = assem.GetType("VA037Svc.Classes.Common");
            //    cache.Add("VA037Svc", type);
            //}

            //if (type != null)
            //{
            //    var o = Activator.CreateInstance(type);
            //    MethodInfo methodInfo = type.GetMethod("GetLoginSession", new Type[] { typeof(Ctx), typeof(VLogger) });
            //    object[] param = new object[2];
            //    param[0] = ctx;
            //    param[1] = log;
            //    var result = methodInfo.Invoke(o, param);
            //    lstString.Add(result.ToString());
            //    PropertyInfo prop = type.GetProperty("BIUrl");
            //    if (null != prop)
            //    {
            //        var propValue = prop.GetValue(type, null);
            //        lstString.Add(propValue.ToString());
            //    }
            //}
            return lstString;
        }
    }


}