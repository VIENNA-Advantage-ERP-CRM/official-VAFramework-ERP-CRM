/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MAcctProcessor
 * Purpose        : Accounting Processor Model
 * Class Used     : X_C_AcctProcessor and ViennaProcessor
 * Chronological    Development
 * Raghunandan     07-Jan-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using System.Data.SqlClient;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.Process;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MAcctProcessor : X_C_AcctProcessor, ViennaProcessor
    {
        //Static Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MAcctProcessor).FullName);

        /// <summary>
        /// Get Active
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns>active processors</returns>
        public static MAcctProcessor[] GetActive(Ctx ctx)
        {
            List<MAcctProcessor> list = new List<MAcctProcessor>();
            String sql = "SELECT * FROM C_AcctProcessor WHERE IsActive='Y'";
            IDataReader idr = null;

            //Changed By Karan.....
            //DataSet pstmt = null;
            string scheduleIP = null;
            try
            {

                string machineIP = System.Net.Dns.GetHostEntry(Environment.MachineName).AddressList[0].ToString();


                idr = DataBase.DB.ExecuteReader(sql, null, null);
                while (idr.Read())
                {
                    scheduleIP = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT RunOnlyOnIP FROM AD_Schedule WHERE 
                                                       AD_Schedule_ID = (SELECT AD_Schedule_ID FROM C_AcctProcessor WHERE C_AcctProcessor_ID =" + idr["C_AcctProcessor_ID"] + " )"));

                    //list.Add(new MAcctProcessor(ctx, idr, null));

                    if (string.IsNullOrEmpty(scheduleIP) || machineIP.Contains(scheduleIP))
                    {
                        list.Add(new MAcctProcessor(new Ctx(), idr, null));
                    }

                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null) { idr.Close(); }
                _log.Log(Level.SEVERE, "GetActive", e);
            }

            MAcctProcessor[] retValue = new MAcctProcessor[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get Active Account Processors
        /// VIS0060 - 21-Oct-2021
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="ExecuteProcess"></param>
        /// <returns>active processors</returns>
        public static MAcctProcessor[] GetActive(Ctx ctx, string ExecuteProcess, string machineIP)
        {
            List<MAcctProcessor> list = new List<MAcctProcessor>();
            String sql = "SELECT * FROM C_AcctProcessor WHERE IsActive='Y'";
            IDataReader idr = null;
            string scheduleIP = null;
            try
            {
                // string machineIP = Classes.CommonFunctions.GetMachineIPPort();
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                while (idr.Read())
                {
                    scheduleIP = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT RunOnlyOnIP FROM AD_Schedule WHERE 
                                                       AD_Schedule_ID = (SELECT AD_Schedule_ID FROM C_AcctProcessor WHERE C_AcctProcessor_ID =" + idr["C_AcctProcessor_ID"] + " )"));

                    if (ExecuteProcess.Equals("2") && (string.IsNullOrEmpty(scheduleIP) || machineIP.Equals(scheduleIP)))
                    {
                        list.Add(new MAcctProcessor(new Ctx(), idr, null));
                    }
                    else if (!string.IsNullOrEmpty(scheduleIP) && machineIP.Equals(scheduleIP))
                    {
                        list.Add(new MAcctProcessor(new Ctx(), idr, null));
                    }
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null) { idr.Close(); }
                _log.Log(Level.SEVERE, "GetActive", e);
            }

            MAcctProcessor[] retValue = new MAcctProcessor[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Standard Construvtor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="C_AcctProcessor_ID"></param>
        /// <param name="trxName"></param>
        public MAcctProcessor(Ctx ctx, int C_AcctProcessor_ID, Trx trxName)
            : base(ctx, C_AcctProcessor_ID, trxName)
        {
            if (C_AcctProcessor_ID == 0)
            {
                //	setName (null);
                //	setSupervisor_ID (0);
                SetFrequencyType(FREQUENCYTYPE_Hour);
                SetFrequency(1);
                SetKeepLogDays(7);	// 7
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public MAcctProcessor(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        {

        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="client">parent</param>
        /// <param name="Supervisor_ID">admin</param>
        public MAcctProcessor(MClient client, int Supervisor_ID)
            : this(client.GetCtx(), 0, client.Get_TrxName())
        {
            SetClientOrg(client);
            SetName(client.GetName() + " - "
                + Msg.Translate(GetCtx(), "C_AcctProcessor_ID"));
            SetSupervisor_ID(Supervisor_ID);
        }



        /// <summary>
        /// Get Server ID
        /// </summary>
        /// <returns>id</returns>
        public String GetServerID()
        {
            return "AcctProcessor" + Get_ID();
        }

        /// <summary>
        /// Get Date Next Run
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>date next run</returns>
        public DateTime? GetDateNextRun(bool requery)
        {
            if (requery)
            {
                Load(Get_TrxName());
            }
            return GetDateNextRun();
        }

        /// <summary>
        /// Get Logs
        /// </summary>
        /// <returns>logs</returns>
        public ViennaProcessorLog[] GetLogs()
        {
            List<MAcctProcessorLog> list = new List<MAcctProcessorLog>();
            String sql = "SELECT * "
                + "FROM C_AcctProcessorLog "
                + "WHERE C_AcctProcessor_ID=@C_AcctProcessor_ID"
                + "ORDER BY Created DESC";
            IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@C_AcctProcessor_ID", GetC_AcctProcessor_ID());
                while (idr.Read())
                {
                    list.Add(new MAcctProcessorLog(GetCtx(), idr, Get_TrxName()));
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }

            MAcctProcessorLog[] retValue = new MAcctProcessorLog[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Delete old Request Log
        /// </summary>
        /// <returns>number of records</returns>
        public int DeleteLog()
        {
            if (GetKeepLogDays() < 1)
            {
                return 0;
            }
            String sql = "DELETE FROM C_AcctProcessorLog "
                + "WHERE C_AcctProcessor_ID=" + GetC_AcctProcessor_ID()
                //jz + " AND (Created+" + getKeepLogDays() + ") < SysDate";
                + " AND addDays(Created," + GetKeepLogDays() + ") < SysDate";
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            return no;
        }
    }
}
