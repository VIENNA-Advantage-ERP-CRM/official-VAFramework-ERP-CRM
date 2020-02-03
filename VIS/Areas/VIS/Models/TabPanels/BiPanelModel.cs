using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Model
{

    public class BiPanelModel
    {
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
            Assembly assem = Assembly.Load("VA037");
            Type type = assem.GetType("VA037.Classes.Common");
            if (type != null)
            {
                var o = Activator.CreateInstance(type);
                MethodInfo methodInfo = type.GetMethod("GetLoginSession", new Type[] { typeof(Ctx), typeof(VLogger) });
                object[] param = new object[2];
                param[0] = ctx;
                param[1] = log;
                var result = methodInfo.Invoke(o, param);
                lstString.Add(result.ToString());
                PropertyInfo prop = type.GetProperty("BIUrl");
                if (null != prop)
                {
                    var propValue = prop.GetValue(type, null);
                    lstString.Add(propValue.ToString());
                }
            }
            return lstString;
        }
    }


}