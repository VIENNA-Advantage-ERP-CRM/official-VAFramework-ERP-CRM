using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;
using VAModelAD.Model;

namespace VAdvantage.BiReport
{
    public class BiReportEngine
    {
        private static CCache<string, Type> cache = new CCache<string, Type>("Common", 30, 60);

        public List<string> GetUserBILogin(Ctx ctx, VLogger log)
        {
            List<string> lstString = new List<string>();
            // add this to utiiy class


            Type type = (Type)cache.Get("VA037Svc");

            if (type == null)
            {
                Assembly assem = Assembly.Load("VA037Svc");
                type = assem.GetType("VA037Svc.Classes.Common");
                cache.Add("VA037Svc", type);
            }

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

        /// <summary>
        /// Load report parameter string from VA039 module.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="reportID"></param>
        /// <param name="AD_PInstance_ID"></param>
        /// <param name="pi"></param>
        /// <returns></returns>
        public string GetReportParameters(Ctx ctx, int reportID, int AD_PInstance_ID, VAdvantage.ProcessEngine.ProcessInfo pi)
        {
            object result = null;

            Assembly assem = Assembly.Load("VA039");
            Type type = assem.GetType("VA039.Classes.Common");

            if (type != null)
            {
                var o = Activator.CreateInstance(type);
                MethodInfo methodInfo = type.GetMethod("GetReportParameterString", new Type[] { typeof(Ctx), typeof(int), typeof(int), typeof(VAdvantage.ProcessEngine.ProcessInfo) });
                object[] param = new object[4];
                param[0] = ctx;
                param[1] = reportID;
                param[2] = AD_PInstance_ID;
                param[3] = pi;
                result = methodInfo.Invoke(o, param);
            }
            return result.ToString();
        }

        /// <summary>
        /// Get Report ID based on reportUUID
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="reportUUID"></param>
        /// <returns></returns>
        public int GetReportID(Ctx ctx, string reportUUID)
        {
            object result = null;

            Assembly assem = Assembly.Load("VA039");
            Type type = assem.GetType("VA039.Classes.Common");

            if (type != null)
            {
                var o = Activator.CreateInstance(type);
                MethodInfo methodInfo = type.GetMethod("GetReportID", new Type[] { typeof(Ctx), typeof(string) });
                object[] param = new object[2];
                param[0] = ctx;
                param[1] = reportUUID;
                result = methodInfo.Invoke(o, param);
            }
            if (result!=null)
                return Convert.ToInt32(result.ToString());

            return 0;
        }

        /// <summary>
        /// Generates a url of report with parameters.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="log"></param>
        /// <param name="pi"></param>
        /// <returns></returns>
        public string GetReportString(Ctx ctx, VLogger log, VAdvantage.ProcessEngine.ProcessInfo pi)
        {
            string reportFilePath = "";
            List<string> result = GetUserBILogin(ctx, log);
            int outvalue = 0;
            // check if return result returned by login is token or interger. If int value then show respective message.
            if (!int.TryParse(result[0], out outvalue))
            {
                object biRepID = DB.ExecuteScalar("SELECT VA039_BIREPORTID FROM AD_PRocess where AD_Process_ID=" + pi.GetAD_Process_ID());

                int rID = GetReportID(ctx, biRepID.ToString());

                string filters = GetReportParameters(ctx, rID, pi.GetAD_PInstance_ID(), pi);
                reportFilePath = result[1] + "JsAPI?clientOrg=" + MClient.Get(ctx).GetValue() + "&reportUUID=" + biRepID + "&token=" + result[0]+ "&showFilters=false&showInfo=false" + filters;
            }
            else
            {
                if (outvalue == 1)
                {
                    reportFilePath = Msg.GetMsg(ctx, "VA037_BIToolMembership");
                }
                else if (outvalue == 2)
                {
                    reportFilePath = Msg.GetMsg(ctx, "VA037_BIUrlNotFound");
                }
                else if (outvalue == 3)
                {
                    reportFilePath = Msg.GetMsg(ctx, "VA037_NotBIUser");
                }
                else if (outvalue == 4)
                {
                    reportFilePath = Msg.GetMsg(ctx, "VA037_BICallingError");
                }
            }
            return reportFilePath;
        }

    }
}

