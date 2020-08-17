using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
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

        /// <summary>
        /// if Doctype or targetDocType column exist in window, then check print format attached to that Doc type. and open that one.
        /// </summary>
        /// <param name="_pi">Process Info.</param>
        private static void TryPrintFromDocType(ProcessInfo _pi)
        {
            try
            {
                string colName = "C_DocTypeTarget_ID";
                string sql = "SELECT COUNT(*) FROM AD_Column WHERE AD_Table_ID=" + _pi.GetTable_ID() + " AND ColumnName   ='C_DocTypeTarget_ID'";
                int id = Util.GetValueOfInt(DB.ExecuteScalar(sql));
                if (id < 1)
                {
                    colName = "C_DocType_ID";
                    sql = "SELECT COUNT(*) FROM AD_Column WHERE AD_Table_ID=" + _pi.GetTable_ID() + " AND ColumnName   ='C_DocType_ID'";
                    id = Util.GetValueOfInt(DB.ExecuteScalar(sql));
                    if (id < 1)
                    {
                        return;
                    }
                }
                string tableName = Util.GetValueOfString(DB.ExecuteScalar("SELECT TableName FROM AD_Table WHERE AD_Table_ID=" + _pi.GetTable_ID()));
                sql = "SELECT " + colName + " FROM " + tableName + " WHERE " + tableName + "_ID =" + _pi.GetRecord_ID();
                id = Util.GetValueOfInt(DB.ExecuteScalar(sql));
                if (id < 1)
                {
                    return;
                }
                sql = "SELECT AD_ReportFormat_ID FROM C_DocType WHERE C_DocType_ID=" + id;
                id = Util.GetValueOfInt(DB.ExecuteScalar(sql));
                if (id > 0)
                {
                    _pi.SetAD_ReportFormat_ID(id);
                }

            }
            catch
            {
                return;
            }
        }
    }
}
