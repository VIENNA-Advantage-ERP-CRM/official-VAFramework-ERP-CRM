using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VAdvantage.Common
{
    public class Common
    {
        public static string NONBUSINESSDAY = "@DateIsInNonBusinessDay@";


        /// <summary>
        /// Method to get byte array of report.
        /// </summary>
        /// <param name="ctx">Current Context.</param>
        /// <param name="AD_Process_ID">Process ID.</param>
        /// <param name="Name">Name of process.</param>
        /// <param name="AD_Table_ID">Table id for fetching report.</param>
        /// <param name="Record_ID">Record id.</param>
        /// <param name="WindowNo">Window Number if process fired from window.</param>
        /// <param name="recIDs">String of record id's/</param>
        /// <param name="fileType">Report type.</param>
        /// <param name="report">Out parameter to get byte array.</param>
        /// <returns>Returns Process Info list and report byte array.</returns>
        public Dictionary<string, object> GetReport(Ctx ctx, int AD_Process_ID, string Name, int AD_Table_ID, int Record_ID, int WindowNo, string recIDs, string fileType, out byte[] report, out string rptFilePath)
        {
            Dictionary<string, object> d = null;
            report = null;
            rptFilePath = null;

            // Create PInstance
            MPInstance instance = null;
            try
            {
                instance = new MPInstance(ctx, AD_Process_ID, Record_ID);
            }
            catch (Exception e)
            {
                VLogger.Get().SaveError("GetReport-Instance Save", e.Message);
                return d;
            }

            if (!instance.Save())
            {
                ValueNamePair vp = VLogger.RetrieveError();
                if (vp != null)
                {
                    VLogger.Get().SaveError("GetReport-Instance Save", vp.Name ?? vp.GetValue());
                }
                else
                {
                    VLogger.Get().SaveError("GetReport-Instance Save", "PInstanceNotSaved");
                }

                return d;
            }

            // Culture.
            string lang = ctx.GetAD_Language().Replace("_", "-");
            System.Globalization.CultureInfo original = System.Threading.Thread.CurrentThread.CurrentCulture;

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(lang);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);


            // Process info
            ProcessInfo pi = new ProcessInfo(Name, AD_Process_ID, AD_Table_ID, Record_ID, recIDs);
            pi.SetFileType(fileType);
            TryPrintFromDocType(pi);

            try
            {

                pi.SetAD_User_ID(ctx.GetAD_User_ID());
                pi.SetAD_Client_ID(ctx.GetAD_Client_ID());
                pi.SetAD_PInstance_ID(instance.GetAD_PInstance_ID());
                ProcessCtl ctl = new ProcessCtl();
                d = new Dictionary<string, object>();
                d = ctl.Process(pi, ctx, out report, out rptFilePath);
                ctl.ReportString = null;

            }
            catch (Exception e)
            {
                VLogger.Get().SaveError("GetReport", e.Message);
                report = null;
            }
            return d;
        }
    }
}
