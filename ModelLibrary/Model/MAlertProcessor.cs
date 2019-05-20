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
    public class MAlertProcessor : X_AD_AlertProcessor, ViennaProcessor
    {
        public static MAlertProcessor[] GetActive(Ctx ctx)
        {
            List<MAlertProcessor> list = new List<MAlertProcessor>();
            String sql = "SELECT * FROM AD_AlertProcessor WHERE IsActive='Y'";

            try
            {
                DataSet ds = DB.ExecuteDataset(sql);
                foreach(DataRow dr in ds.Tables[0].Rows)
                    list.Add(new MAlertProcessor(ctx, dr, null));
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, sql, e);
            }

            MAlertProcessor[] retValue = new MAlertProcessor[list.Count()];
            retValue = list.ToArray();
            return retValue;
        }	//	getActive

        /**	Static Logger	*/
        private static VLogger s_log = VLogger.GetVLogger(typeof(MAlertProcessor).FullName);

        public MAlertProcessor(Ctx ctx, int AD_AlertProcessor_ID, Trx trx)
            : base(ctx, AD_AlertProcessor_ID, trx)
        {
        }	//	MAlertProcessor

        public MAlertProcessor(Ctx ctx, DataRow rs, Trx trx)
            : base(ctx, rs, trx)
        {
        }	//	MAlertProcessor

        /**	The Alerts						*/
        private MAlert[] m_alerts = null;


        public ViennaProcessorLog[] GetLogs()
        {
            List<MAlertProcessorLog> list = new List<MAlertProcessorLog>();
            String sql = "SELECT * "
                + "FROM AD_AlertProcessorLog "
                + "WHERE AD_AlertProcessor_ID=@AD_AlertProcessor_ID "
                + "ORDER BY Created DESC";
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@AD_AlertProcessor_ID", GetAD_AlertProcessor_ID());
                DataSet ds = DB.ExecuteDataset(sql, param);
                foreach(DataRow dr in ds.Tables[0].Rows)
                    list.Add(new MAlertProcessorLog(GetCtx(), dr, null));
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            MAlertProcessorLog[] retValue = new MAlertProcessorLog[list.Count()];
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
            String sql = "DELETE FROM AD_AlertProcessorLog "
                + "WHERE AD_AlertProcessor_ID=" + GetAD_AlertProcessor_ID()
                //jz + " AND (Created+" + getKeepLogDays() + ") < SysDate";
                + " AND addDays(Created," + GetKeepLogDays() + ") < SysDate";
            DB.ExecuteQuery(sql, null, Get_TrxName());
            return 0;
        }	//	deleteLog


        public MAlert[] GetAlerts(bool reload)
        {
            if (m_alerts != null && !reload)
                return m_alerts;
            String sql = "SELECT * FROM AD_Alert "
                + "WHERE AD_AlertProcessor_ID=@AD_AlertProcessor_ID AND IsActive='Y'";
            List<MAlert> list = new List<MAlert>();
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@AD_AlertProcessor_ID", GetAD_AlertProcessor_ID());
                DataSet ds = DB.ExecuteDataset(sql, param);
                foreach(DataRow dr in ds.Tables[0].Rows)
                    list.Add(new MAlert(GetCtx(), dr, null));
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            //
            m_alerts = new MAlert[list.Count()];
            m_alerts = list.ToArray();
            return m_alerts;
        }	//	getAlerts
    }
}
