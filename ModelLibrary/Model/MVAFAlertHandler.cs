using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using System.Data.SqlClient;

namespace VAdvantage.Model
{
    public class MVAFAlertHandler : X_VAF_AlertHandler, ViennaProcessor
    {
        public static MVAFAlertHandler[] GetActive(Ctx ctx)
        {
            List<MVAFAlertHandler> list = new List<MVAFAlertHandler>();
            String sql = "SELECT * FROM VAF_AlertHandler WHERE IsActive='Y'";

            //Changed By Karan.....
            string scheduleIP = null;
            try
            {
                string machineIP = System.Net.Dns.GetHostEntry(Environment.MachineName).AddressList[0].ToString();

                DataSet ds = DB.ExecuteDataset(sql);

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    scheduleIP = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT RunOnlyOnIP FROM VAF_Plan WHERE 
                                                        VAF_Plan_ID = (SELECT VAF_Plan_ID FROM VAF_AlertHandler WHERE VAF_AlertHandler_ID =" + dr["VAF_AlertHandler_ID"] + " )"));

                    //if (string.IsNullOrEmpty(scheduleIP) || machineIP.Contains(scheduleIP) || machineIPPort.Contains(scheduleIP))
                    if (string.IsNullOrEmpty(scheduleIP) || machineIP.Contains(scheduleIP))
                    {
                        list.Add(new MVAFAlertHandler(new Ctx(), dr, null));
                    }
                }
                ds = null;

                //foreach(DataRow dr in ds.Tables[0].Rows)
                //    list.Add(new MAlertProcessor(ctx, dr, null));
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, sql, e);
            }

            MVAFAlertHandler[] retValue = new MVAFAlertHandler[list.Count()];
            retValue = list.ToArray();
            return retValue;
        }	//	getActive

        /**	Static Logger	*/
        private static VLogger s_log = VLogger.GetVLogger(typeof(MVAFAlertHandler).FullName);

        public MVAFAlertHandler(Ctx ctx, int VAF_AlertHandler_ID, Trx trx)
            : base(ctx, VAF_AlertHandler_ID, trx)
        {
        }	//	MAlertProcessor

        public MVAFAlertHandler(Ctx ctx, DataRow rs, Trx trx)
            : base(ctx, rs, trx)
        {
        }	//	MAlertProcessor

        /**	The Alerts						*/
        private MVAFAlert[] m_alerts = null;


        public ViennaProcessorLog[] GetLogs()
        {
            List<MVAFAlertHandlerLog> list = new List<MVAFAlertHandlerLog>();
            String sql = "SELECT * "
                + "FROM VAF_AlertHandlerLog "
                + "WHERE VAF_AlertHandler_ID=@VAF_AlertHandler_ID "
                + "ORDER BY Created DESC";
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@VAF_AlertHandler_ID", GetVAF_AlertHandler_ID());
                DataSet ds = DB.ExecuteDataset(sql, param);
                foreach(DataRow dr in ds.Tables[0].Rows)
                    list.Add(new MVAFAlertHandlerLog(GetCtx(), dr, null));
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            MVAFAlertHandlerLog[] retValue = new MVAFAlertHandlerLog[list.Count()];
            retValue = list.ToArray();
            return retValue;
        }	//	getLogs

        public String GetServerID()
        {
            return "AlertProcessor" + Get_ID();
        }	//	getServerID

        public DateTime? GetDateNextRun(bool requery)
        {
            if (requery)
                Load(Get_TrxName());
            return GetDateNextRun();
        }	//	getDateNextRun

        public int DeleteLog()
        {
            if (GetKeepLogDays() < 1)
                return 0;
            String sql = "DELETE FROM VAF_AlertHandlerLog "
                + "WHERE VAF_AlertHandler_ID=" + GetVAF_AlertHandler_ID()
                //jz + " AND (Created+" + getKeepLogDays() + ") < SysDate";
                + " AND addDays(Created," + GetKeepLogDays() + ") < SysDate";
            DB.ExecuteQuery(sql, null, Get_TrxName());
            return 0;
        }	//	deleteLog


        public MVAFAlert[] GetAlerts(bool reload)
        {
            if (m_alerts != null && !reload)
                return m_alerts;
            String sql = "SELECT * FROM VAF_Alert "
                + "WHERE VAF_AlertHandler_ID=@VAF_AlertHandler_ID AND IsActive='Y'";
            List<MVAFAlert> list = new List<MVAFAlert>();
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@VAF_AlertHandler_ID", GetVAF_AlertHandler_ID());
                DataSet ds = DB.ExecuteDataset(sql, param);
                foreach(DataRow dr in ds.Tables[0].Rows)
                    list.Add(new MVAFAlert(GetCtx(), dr, null));
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            //
            m_alerts = new MVAFAlert[list.Count()];
            m_alerts = list.ToArray();
            return m_alerts;
        }	//	getAlerts
    }
}
