/********************************************************
 * Module Name    : Scheduler
 * Purpose        : Schedule the Events
 * Author         : Jagmohan Bhatt
 * Date           : 03-Nov-2009
 ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using VAdvantage.Process;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VAdvantage.Classes;
using VAdvantage.Logging;
using VAdvantage.Print;
using VAdvantage.ProcessEngine;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using System.Threading;
//using System.Data.OracleClient;
using Oracle.ManagedDataAccess.Client;
using System.Reflection;
using VAModelAD.Model;

namespace VAdvantage.Model
{
    public class MScheduler : X_AD_Scheduler, ViennaProcessor
    {
        /// <summary>
        /// Get Active
        /// </summary>
        /// <param name="ctx">context</param>
        /// <returns>active processors</returns>
        public static MScheduler[] GetActive(Ctx ctx)
        {

            //List<MScheduler> list = new List<MScheduler>();
            //String sql = "SELECT * FROM AD_Scheduler WHERE IsActive='Y'";
            //try
            //{
            //    DataSet ds = DataBase.DB.ExecuteDataset(sql);
            //    foreach (DataRow dr in ds.Tables[0].Rows)
            //    {
            //        list.Add(new MScheduler(ctx, dr, null));
            //    }
            //    ds = null;
            //}
            //catch (Exception e)
            //{

            //    s_log.Log(Level.SEVERE, sql, e);
            //}


            //MScheduler[] retValue = new MScheduler[list.Count()];
            //retValue = list.ToArray();
            //return retValue;


            // changed by lakhwinder
            // the scheduler entries having ip address on the respective schedule.
            // will not be run on the other system having different machine IP.

            List<MScheduler> list = new List<MScheduler>();
            String sql = "SELECT * FROM AD_Scheduler WHERE IsActive='Y'";
            string scheduleIP = null;
            try
            {
                //string machineIP = null;// System.Net.Dns.GetHostEntry(Environment.MachineName).AddressList[0].ToString();

                string machineIP = System.Net.Dns.GetHostEntry(Environment.MachineName).AddressList[0].ToString();
                //var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                //foreach (var ip in host.AddressList)
                //{
                //    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                //    {
                //        machineIP= ip.ToString();
                //        break;
                //    }
                //}

                //int port=System.Web.HttpContext.Current.Request.Url.Port;
                //string machineIPPort =machineIP+ ":" + port.ToString();
                DataSet ds = DataBase.DB.ExecuteDataset(sql);
                s_log.SaveError("Console VServer Machine IP : " + machineIP, "Console VServer Machine IP : " + machineIP);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    scheduleIP = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT RunOnlyOnIP FROM AD_Schedule WHERE 
                                                        AD_Schedule_ID = (SELECT AD_Schedule_ID FROM AD_Scheduler WHERE AD_Scheduler_ID =" + dr["AD_Scheduler_ID"] + " )"));
                    s_log.SaveError("Console VServer Schedule IP : " + scheduleIP, "Console VServer Schedule IP : " + scheduleIP);
                    //if (string.IsNullOrEmpty(scheduleIP) || machineIP.Contains(scheduleIP) || machineIPPort.Contains(scheduleIP))
                    if (string.IsNullOrEmpty(scheduleIP) || machineIP.Contains(scheduleIP))
                    {
                        list.Add(new MScheduler(new Ctx(), dr, null));
                    }
                }
                ds = null;
            }
            catch (Exception e)
            {

                s_log.Log(Level.SEVERE, sql, e);
            }


            MScheduler[] retValue = new MScheduler[list.Count()];
            retValue = list.ToArray();
            return retValue;
        }   //	getActive

        /// <summary>
        /// Get Active Schedulers
        /// VIS0060 - 21-Oct-2021
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name= ExecuteProcess"></param >
        /// <returns>active processors</returns>
        public static MScheduler[] GetActive(Ctx ctx, string ExecuteProcess, string machineIP)
        {
            List<MScheduler> list = new List<MScheduler>();
            String sql = "SELECT * FROM AD_Scheduler WHERE IsActive='Y'";
            string scheduleIP = null;
            try
            {
                // string machineIP = Classes.CommonFunctions.GetMachineIPPort();
                s_log.SaveError("Console VServer Machine IP : " + machineIP, "Console VServer Machine IP : " + machineIP);

                DataSet ds = DataBase.DB.ExecuteDataset(sql);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        scheduleIP = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT RunOnlyOnIP FROM AD_Schedule WHERE 
                                                        AD_Schedule_ID = (SELECT AD_Schedule_ID FROM AD_Scheduler WHERE AD_Scheduler_ID =" + dr["AD_Scheduler_ID"] + " )"));
                        s_log.SaveError("Console VServer Schedule IP : " + scheduleIP, "Console VServer Schedule IP : " + scheduleIP);

                        if (ExecuteProcess.Equals("2") && (string.IsNullOrEmpty(scheduleIP) || machineIP.Equals(scheduleIP)))
                        {
                            list.Add(new MScheduler(new Ctx(), dr, null));
                        }
                        else if (!string.IsNullOrEmpty(scheduleIP) && machineIP.Equals(scheduleIP))
                        {
                            list.Add(new MScheduler(new Ctx(), dr, null));
                        }
                    }
                }
                ds = null;
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, sql, e);
            }

            MScheduler[] retValue = new MScheduler[list.Count()];
            retValue = list.ToArray();
            return retValue;
        }	//	getActive

        /**	Static Logger	*/
        private static VLogger s_log = VLogger.GetVLogger(typeof(MScheduler).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Scheduler_ID">scheduler id</param>
        /// <param name="trxName">optional transaction name</param>
        public MScheduler(Ctx ctx, int AD_Scheduler_ID, Trx trxName)
            : base(ctx, AD_Scheduler_ID, trxName)
        {

            if (AD_Scheduler_ID == 0)
            {
                //	setAD_Process_ID (0);
                //	setName (null);
                SetFrequencyType(FREQUENCYTYPE_Day);
                SetFrequency(1);
                //
                SetKeepLogDays(7);
                //	setSupervisor_ID (0);
            }
        }	//	MScheduler


        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">optional transaction name</param>
        public MScheduler(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }	//	MScheduler

        /** Process to be executed		*/
        private MProcess m_process = null;
        /**	Scheduler Parameter			*/
        private MSchedulerPara[] m_parameter = null;
        /** Scheduler Recipients		*/
        private MSchedulerRecipient[] m_recipients = null;
        /** The Supervisor				*/
        private MUser m_supervisor = null;

        /// <summary>
        /// Get Server ID
        /// </summary>
        /// <returns>ID</returns>
        public String GetServerID()
        {
            return "Scheduler" + Get_ID();
        }	//	getServerID

        /// <summary>
        /// Get Date Next Run
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>date next run</returns>
        public DateTime? GetDateNextRun(bool requery)
        {
            if (requery)
                Load(Get_TrxName());
            return GetDateNextRun();
        }	//	getDateNextRun


        /// <summary>
        /// Get Logs
        /// </summary>
        /// <returns>logs</returns>
        public ViennaProcessorLog[] GetLogs()
        {
            List<MSchedulerLog> list = new List<MSchedulerLog>();
            String sql = "SELECT * "
                + "FROM AD_SchedulerLog "
                + "WHERE AD_Scheduler_ID=@scheduleid "
                + "ORDER BY Created DESC";

            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@scheduleid", GetAD_Scheduler_ID());
                DataSet ds = DataBase.DB.ExecuteDataset(sql, param);

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new MSchedulerLog(GetCtx(), dr, Get_TrxName()));
                }
                ds = null;
            }
            catch (Exception e)
            {

                log.Log(Level.SEVERE, sql, e);
            }


            MSchedulerLog[] retValue = new MSchedulerLog[list.Count()];
            retValue = list.ToArray();
            return retValue;
        }	//	getLogs


        /// <summary>
        /// Delete old Request Log
        /// </summary>
        /// <returns>number of records</returns>
        public int DeleteLog()
        {
            if (GetKeepLogDays() < 1)
                return 0;
            String sql = "DELETE FROM AD_SchedulerLog "
                + "WHERE AD_Scheduler_ID=" + GetAD_Scheduler_ID()
                + " AND adddays(Created, " + GetKeepLogDays() + ") < SysDate";
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            return no;
        }	//	deleteLog

        /// <summary>
        /// Get Process
        /// </summary>
        /// <returns>process</returns>
        public MProcess GetProcess()
        {


            if (m_process == null)
                m_process = new MProcess(GetCtx(), GetAD_Process_ID(), null);
            return m_process;
        }	//	getProcess


        /// <summary>
        /// Get Parameters
        /// </summary>
        /// <param name="reload">reload</param>
        /// <returns>parameter</returns>
        public MSchedulerPara[] GetParameters(bool reload)
        {
            if (!reload && m_parameter != null)
                return m_parameter;
            List<MSchedulerPara> list = new List<MSchedulerPara>();
            String sql = "SELECT * FROM AD_Scheduler_Para WHERE AD_Scheduler_ID=@scheduleid AND IsActive='Y'";
            DataSet ds = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@scheduleid", GetAD_Scheduler_ID());
                ds = new DataSet();
                ds = DataBase.DB.ExecuteDataset(sql, param);

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new MSchedulerPara(GetCtx(), dr, null));
                }
                ds = null;
            }
            catch (Exception e)
            {
                if (ds != null)
                {
                    ds = null;
                }
                log.Log(Level.SEVERE, sql, e);
            }

            m_parameter = new MSchedulerPara[list.Count()];
            m_parameter = list.ToArray();
            return m_parameter;
        }	//	getParameter

        /// <summary>
        /// Get Recipients
        /// </summary>
        /// <param name="reload">reload</param>
        /// <returns>recipients</returns>
        public MSchedulerRecipient[] GetRecipients(bool reload)
        {
            if (!reload && m_recipients != null)
                return m_recipients;
            List<MSchedulerRecipient> list = new List<MSchedulerRecipient>();
            String sql = "SELECT * FROM AD_SchedulerRecipient WHERE AD_Scheduler_ID=@scheduleid AND IsActive='Y'";
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@scheduleid", GetAD_Scheduler_ID());
                DataSet ds = DataBase.DB.ExecuteDataset(sql, param);

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new MSchedulerRecipient(GetCtx(), dr, null));
                }
                ds = null;

            }
            catch (Exception e)
            {

                log.Log(Level.SEVERE, sql, e);
            }

            m_recipients = new MSchedulerRecipient[list.Count()];
            m_recipients = list.ToArray();
            return m_recipients;
        }	//	getRecipients

        /// <summary>
        /// Get Recipient AD_User_IDs
        /// </summary>
        /// <returns>array of user IDs</returns>
        public int[] GetRecipientAD_User_IDs()
        {
            List<int> list = new List<int>();
            MSchedulerRecipient[] recipients = GetRecipients(false);
            for (int i = 0; i < recipients.Length; i++)
            {
                MSchedulerRecipient recipient = recipients[i];
                if (!recipient.IsActive())
                    continue;
                if (recipient.GetAD_User_ID() != 0)
                {
                    int ii = recipient.GetAD_User_ID();
                    if (!list.Contains(ii))
                        list.Add(ii);
                }
                if (recipient.GetAD_Role_ID() != 0)
                {
                    MUserRoles[] urs = MUserRoles.GetOfRole(GetCtx(), recipient.GetAD_Role_ID());
                    for (int j = 0; j < urs.Length; j++)
                    {
                        MUserRoles ur = urs[j];
                        if (!ur.IsActive())
                            continue;
                        int ii = ur.GetAD_User_ID();
                        if (!list.Contains(ii))
                            list.Add(ii);
                    }
                }
            }
            //	Add Updater
            if (list.Count() == 0)
            {
                int ii = GetUpdatedBy();
                list.Add(ii);
            }
            //
            int[] recipientIDs = new int[list.Count()];
            recipientIDs = list.ToArray();
            return recipientIDs;
        }	//	getRecipientAD_User_IDs


        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MScheduler[");
            sb.Append(Get_ID()).Append("-").Append(GetName())
                .Append("]");
            return sb.ToString();
        }	//	toString


        /// <summary>
        /// Run Scheduler
        /// </summary>
        /// <param name="trx">optional transaction</param>
        /// <returns>Summary</returns>
        public String Execute(Trx trx)
        {
            //if (GetAD_Form_ID() != 0)
            //{
            //    return RunCrystalReport();
            //}

            if (m_process == null)
                GetProcess();

            if (m_process.IsReport())
            {
                //if (m_process.GetClassname() != null)
                //{
                //    RunProcess(trx);
                //}

                return RunReport(trx);
                //return ""; // RunReport(trx); //not implemented yet
            }
            else
            {
                return RunProcess(trx);
            }
        }	//	execute

        public void ExecuteFromThread(Trx trx)
        {
            this.Execute(trx);
        }

        private List<String> _emails = new List<string>();
        bool isDocxFile = false;
        /// <summary>
        /// Run Report
        /// </summary>
        /// <param name="trx">optional transaction</param>
        /// <returns></returns>

        private String RunReport(Trx trx)
        {
            log.Info(m_process.ToString());
            if (!m_process.IsReport() || (m_process.GetAD_PrintFormat_ID() == 0
                                             && m_process.GetAD_ReportView_ID() == 0
                                             //&& !m_process.GetIsCrystalReport()
                                             && m_process.GetIsCrystalReport() == "N"
                                              && m_process.GetAD_ReportFormat_ID() == 0
                                              && !m_process.Get_Value("IsCrystalReport").Equals("B")
                                              && !m_process.Get_Value("IsCrystalReport").Equals("J")
                                              && m_process.GetAD_ReportMaster_ID() == 0))
                return "Not a Report AD_Process_ID=" + m_process.GetAD_Process_ID()
                    + " - " + m_process.GetName();
            //	Process
            int AD_Table_ID = 0;
            int Record_ID = 0;
            //
            MPInstance pInstance = new MPInstance(m_process, Record_ID);
            String error = FillParameter(pInstance);
            if (!String.IsNullOrEmpty(error))
            {
                NotifySupervisor(false, error, null);
                return error;
            }
            //
            ProcessInfo pi = new ProcessInfo(m_process.GetName(), m_process.GetAD_Process_ID(), AD_Table_ID, Record_ID);
            pi.SetAD_User_ID(GetUpdatedBy());
            pi.SetAD_Client_ID(GetAD_Client_ID());
            pi.SetAD_PInstance_ID(pInstance.GetAD_PInstance_ID());
            pi.SetAD_Org_ID(GetAD_Org_ID());
            if (!m_process.ProcessIt(pi, trx) && pi.GetClassName() != null)
            {
                string msg = "Process failed: (" + pi.GetClassName() + ") " + pi.GetSummary();
                NotifySupervisor(false, msg, null);
                return msg;
            }

            IReportEngine re = null;

            //Dynamic Report    
            if (m_process.GetAD_ReportMaster_ID() > 0)
            {
                String fqClassName = "", asmName = "";
                DataSet ds = DB.ExecuteDataset("SELECT ClassName,AssemblyName FROM AD_ReportMaster WHERE IsActive='Y' AND AD_ReportMaster_ID = " + m_process.GetAD_ReportMaster_ID());
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    fqClassName = ds.Tables[0].Rows[0]["ClassName"].ToString();
                    asmName = ds.Tables[0].Rows[0]["AssemblyName"].ToString();
                    re = VAdvanatge.Report.ReportEngine.GetReportEngine(p_ctx, pi, trx, asmName, fqClassName);
                }
                else
                {
                    log.Log(Level.WARNING, "Report Engine data not found Error -> InActive record");
                    re = null;
                }
            }

            else if (m_process.GetAD_ReportFormat_ID() > 0)
            {
                string lang = p_ctx.GetContext("#AD_Language");
                lang = lang.Replace("_", "-");

                if ((m_process.GetAD_ReportFormat_ID() > 0) && (lang == "ar-IQ"))
                {
                    isDocxFile = true;
                    //p_ctx.SetContext("ReportFromSchdular", true);
                    pi.IsArabicReportFromOutside = true;

                }
                else
                {
                    pi.IsArabicReportFromOutside = false;
                }
                pi.SetPrintAllPages(true);
                re = VAdvanatge.Report.ReportEngine.GetReportEngine(p_ctx, pi, trx, "VARCOMSvc", "ViennaAdvantage.Classes.ReportFromatWrapper");
            }

            else if (m_process.GetIsCrystalReport() == "Y")
            {
                re = new CrystalReport.CrystalReportEngine();
                re.StartReport(p_ctx, pi, null);
            }
            else if (m_process.GetIsCrystalReport() == "B")
            {
                re = VAdvanatge.Report.ReportEngine.GetReportEngine(p_ctx, pi, trx, "VA039", "VA039.Classes.BIReportEngine");

                //try
                //{
                //    log.Log(Level.INFO, "MWFActivity=>BI Report");
                //    X_AD_Process BIProcess = new X_AD_Process(p_ctx, pi.GetAD_Process_ID(), null);
                //    var Dll = Assembly.Load("VA039");
                //    var BIReportEngine = Dll.GetType("VA039.Classes.BIReportEngine");

                //    var ctor = BIReportEngine.GetConstructors()[0];
                //    if (ctor.GetParameters().Length > 2)
                //    {
                //        re = (IReportEngine)ctor.Invoke(new object[] { p_ctx, pi.GetAD_PInstance_ID(), pi });
                //    }
                //    else
                //    {
                //        re = (IReportEngine)ctor.Invoke(new object[] { p_ctx, pi.GetAD_PInstance_ID() });
                //    }

                //    //ConstructorInfo conInfo = BIReportEngine.GetConstructor(new[] { typeof(Ctx), typeof(int) });
                //    //log.Log(Level.INFO, "MWFActivity=>BI Report Cunstructor Call");
                //    //re = (IReportEngine)conInfo.Invoke(new object[] { p_ctx, pi.GetAD_PInstance_ID() });
                //    log.Log(Level.INFO, "MWFActivity=>BI Report Engine Reference");
                //}
                //catch (Exception e)
                //{
                //    log.Log(Level.INFO, "MWFActivity=>BI Report Error" + e.Message);
                //    re = null;
                //}
            }
            else if (m_process.GetIsCrystalReport() == "J")
            {
                re = VAdvanatge.Report.ReportEngine.GetReportEngine(p_ctx, pi, trx, "VA039", "VA039.Classes.JasperReportEngine");
                //try
                //{
                //    log.Log(Level.INFO, "MWFActivity=>Jasper Report");
                //    X_AD_Process BIProcess = new X_AD_Process(p_ctx, pi.GetAD_Process_ID(), null);
                //    var Dll = Assembly.Load("VA039");
                //    var JasperReportEngine = Dll.GetType("VA039.Classes.JasperReportEngine");
                //    ConstructorInfo conInfo = JasperReportEngine.GetConstructor(new[] { typeof(Ctx), typeof(int) });
                //    log.Log(Level.INFO, "MWFActivity=>Jasper Report Cunstructor Call");
                //    re = (IReportEngine)conInfo.Invoke(new object[] { p_ctx, pi.GetAD_PInstance_ID() });
                //    log.Log(Level.INFO, "MWFActivity=>Jasper Report Engine Reference");
                //}
                //catch (Exception e)
                //{
                //    log.Log(Level.INFO, "MWFActivity=>Jasper Report Error" + e.Message);
                //    re = null;
                //}
            }
            else
            {
                re = ReportEngine_N.Get(GetCtx(), pi);
            }

            //Report
            //re = ReportEngine_N.Get(GetCtx(), pi);
            if (re == null)
            {
                string msg = "Cannot create Report AD_Process_ID=" + m_process.GetAD_Process_ID()
                    + " - " + m_process.GetName();
                NotifySupervisor(false, msg, null);
                return msg;
            }

            //	Notice
            int AD_Message_ID = 884;		//	HARDCODED SchedulerResult
            int[] userIDs = GetRecipientAD_User_IDs();
            byte[] report = null;
            bool success = false;
            if (re != null)
            {
                //int reportTable_ID = re.GetPrintFormat().GetAD_Table_ID();
                if (re is IReportView)
                {
                    ((IReportView)re).GetView();
                }

                if (re != null)
                {
                    report = re.GetReportBytes();
                }

                _emails = new List<String>();
                for (int i = 0; i < userIDs.Length; i++)
                {
                    MNote note = new MNote(GetCtx(), AD_Message_ID, userIDs[i], trx);
                    // changes done by Bharat on 22 May 2018 to set Organization to * on Notification as discussed with Mukesh Sir.
                    //note.SetClientOrg(GetAD_Client_ID(), GetAD_Org_ID());
                    note.SetClientOrg(GetAD_Client_ID(), 0);
                    note.SetTextMsg(GetName());
                    note.SetDescription(GetDescription());
                    note.SetRecord(AD_Table_ID, Record_ID);
                    note.Save();

                    if (report != null)
                    {
                        MAttachment attachment = new MAttachment(GetCtx(), MNote.Table_ID, note.GetAD_Note_ID(), Get_TrxName());
                        attachment.SetClientOrg(GetAD_Client_ID(), GetAD_Org_ID());

                        if (isDocxFile)
                        {
                            attachment.AddEntry("Report_" + DateTime.Now.Ticks + ".Docx", report);
                        }
                        else
                        {
                            attachment.AddEntry("Report_" + DateTime.Now.Ticks + ".pdf", report);
                        }
                        attachment.SetTextMsg(GetName());
                        attachment.Save();
                    }
                    MClient client = MClient.Get(GetCtx(), GetAD_Client_ID());

                    success = SendEMail(client, userIDs[i], null, GetName(), GetDescription(), null, true, 0, 0, report);

                }
            }

            NotifySupervisor(success, pi.GetSummary(), report);
            //
            return pi.GetSummary();
        }	//	runReport

        /// <summary>
        /// Send actual EMail
        /// </summary>
        /// <param name="client"></param>
        /// <param name="AD_User_ID"></param>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="pdf"></param>
        /// <param name="isHTML"></param>
        private bool SendEMail(MClient client, int AD_User_ID, String email, String subject,
            String message, FileInfo pdf, bool isHTML, int AD_Table_ID, int record_ID, byte[] bArray = null)
        {
            if (AD_User_ID != 0)
            {
                MUser user = MUser.Get(GetCtx(), AD_User_ID);
                email = user.GetEMail();

                if (!user.IsActive() || email == null)
                {
                    return false;
                }

                // Check Added by Lokesh Chauhan 
                if (!user.IsEmail())
                {
                    if (!(MUser.NOTIFICATIONTYPE_EMail.Equals(user.GetNotificationType()) ||
                   MUser.NOTIFICATIONTYPE_EMailPlusFaxEMail.Equals(user.GetNotificationType())))
                    {
                        return false;
                    }
                }

                if (email.IndexOf(";") == -1)
                {
                    email = email.Trim();


                    if (!_emails.Contains(email))
                    {
                        if ((isDocxFile))
                        {
                            if (client.SendEMail(email, null, subject, message, pdf, isHTML, AD_Table_ID, record_ID, bArray, DateTime.Now.Millisecond.ToString() + bArray.Length + ".docx"))
                            {
                                _emails.Add(email);
                                return true;
                            }
                        }
                        else
                        {
                            if (client.SendEMail(email, null, subject, message, pdf, isHTML, AD_Table_ID, record_ID, bArray))
                            {
                                _emails.Add(email);
                                return true;
                            }
                        }

                    }
                    return false;
                }
                //	Multiple EMail
                StringTokenizer st = new StringTokenizer(email, ";");
                while (st.HasMoreTokens())
                {
                    String email1 = st.NextToken().Trim();
                    if (email1.Length == 0)
                        continue;
                    if (!_emails.Contains(email1))
                    {
                        if (isDocxFile)
                        {
                            //, DateTime.Now.Millisecond.ToString() + bArray.Length + ".docx"
                            if (client.SendEMail(email1, null, subject, message, pdf, isHTML, AD_Table_ID, record_ID, bArray, DateTime.Now.Millisecond.ToString() + bArray.Length + ".docx"))
                            {
                                _emails.Add(email1);
                            }
                        }
                        else
                        {
                            if (client.SendEMail(email1, null, subject, message, pdf, isHTML, AD_Table_ID, record_ID, bArray))
                            {
                                _emails.Add(email1);
                            }
                        }
                    }
                }

            }
            else if (email != null && email.Length > 0)
            {
                //	Just one
                if (email.IndexOf(";") == -1)
                {
                    email = email.Trim();
                    if (!_emails.Contains(email))
                    {
                        if (isDocxFile)
                        {
                            //  //, DateTime.Now.Millisecond.ToString() + bArray.Length + ".docx"
                            if (client.SendEMail(email, null, subject, message, pdf, isHTML, AD_Table_ID, record_ID, bArray, DateTime.Now.Millisecond.ToString() + bArray.Length + ".docx"))
                            {
                                _emails.Add(email);
                                return true;
                            }
                        }
                        else
                        {
                            if (client.SendEMail(email, null, subject, message, pdf, isHTML, AD_Table_ID, record_ID, bArray))
                            {
                                _emails.Add(email);
                                return true;
                            }
                        }

                    }
                    return false;
                }
                //	Multiple EMail
                StringTokenizer st = new StringTokenizer(email, ";");
                while (st.HasMoreTokens())
                {
                    String email1 = st.NextToken().Trim();
                    if (email1.Length == 0)
                        continue;
                    if (!_emails.Contains(email1))
                    {
                        if (isDocxFile)
                        {
                            if (client.SendEMail(email1, null, subject, message, pdf, isHTML, AD_Table_ID, record_ID, bArray, DateTime.Now.Millisecond.ToString() + bArray.Length + ".docx"))
                            {
                                _emails.Add(email1);
                            }
                        }
                        else
                        {
                            if (client.SendEMail(email1, null, subject, message, pdf, isHTML, AD_Table_ID, record_ID, bArray))
                            {
                                _emails.Add(email1);
                            }
                        }
                    }
                }

            }

            if (_emails != null)
            {
                if (_emails.Count > 0)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Run Process
        /// </summary>
        /// <param name="trx">optional transaction</param>
        /// <returns>Summary</returns>
        private String RunProcess(Trx trx)
        {
            log.Info(m_process.ToString());
            //	Process (see also MWFActivity.performWork
            int AD_Table_ID = 0;
            int Record_ID = 0;
            //
            MPInstance pInstance = new MPInstance(m_process, Record_ID);
            String error = FillParameter(pInstance);
            if (!String.IsNullOrEmpty(error))
            {
                NotifySupervisor(false, error, null);
                return error;
            }
            if (m_process.Get_ID() <= 0)
                return "error";
            //FillParameter(pInstance);
            //

            Ctx ctx = new Ctx();
            ctx.SetAD_Client_ID(GetAD_Client_ID());
            ctx.SetContext("AD_Client_ID", GetAD_Client_ID());
            ctx.SetAD_Org_ID(GetAD_Org_ID());
            ctx.SetContext("AD_Org_ID", GetAD_Org_ID());
            ctx.SetAD_User_ID(GetUpdatedBy());
            ctx.SetContext("AD_User_ID", GetUpdatedBy());
            ctx.SetContext("#SalesRep_ID", GetUpdatedBy());



            ProcessInfo pi = new ProcessInfo(m_process.GetName(), m_process.GetAD_Process_ID(), AD_Table_ID, Record_ID);
            pi.SetAD_User_ID(GetUpdatedBy());
            pi.SetAD_Client_ID(GetAD_Client_ID());
            pi.SetAD_PInstance_ID(pInstance.GetAD_PInstance_ID());

            pi.SetLocalCtx(ctx.GetMap());

            m_process.ProcessIt(pi, trx);
            NotifySupervisor(!pi.IsError(), pi.GetSummary(), null);
            return pi.GetSummary();
        }	//	runProcess


        /// <summary>
        /// Fill the parameter
        /// </summary>
        /// <param name="pInstance">instance detail</param>
        private string FillParameter(MPInstance pInstance)
        {
            StringBuilder sb = null;
            MSchedulerPara[] sParams = GetParameters(false);
            MPInstancePara[] iParams = pInstance.GetParameters();
            for (int pi = 0; pi < iParams.Length; pi++)
            {
                MPInstancePara iPara = iParams[pi];
                for (int np = 0; np < sParams.Length; np++)
                {
                    MSchedulerPara sPara = sParams[np];
                    if (iPara.GetParameterName().Equals(sPara.GetColumnName()))
                    {
                        String variable = sPara.GetParameterDefault();





                        log.Fine(sPara.GetColumnName() + " = " + variable);
                        //	Value - Constant/Variable
                        Object value = variable;
                        if (variable == null || (variable != null && variable.Length == 0))
                            value = null;
                        else if (variable.IndexOf("@") != -1)	//	we may have a variable
                        {
                            //	Strip
                            int index = variable.IndexOf("@");
                            String columnName = variable.Substring(index + 1);
                            index = columnName.IndexOf("@");
                            if (index != -1)
                            {
                                columnName = columnName.Substring(0, index);
                                //	try Env
                                String env = GetCtx().GetContext(columnName);
                                if (env.Length == 0)
                                {
                                    if (columnName == "sysdate")
                                    {
                                        value = DateTime.Now;
                                    }
                                    else
                                    {
                                        log.Warning(sPara.GetColumnName()
                                            + " - not in environment =" + columnName
                                            + "(" + variable + ") - ignored");
                                        break;
                                    }
                                }
                                else
                                    value = env;
                            }
                        }	//	@variable@

                        //	No Value
                        if (value == null)
                        {
                            log.Fine(sPara.GetColumnName() + " - empty");
                            break;
                        }

                        object colValue = "";

                        try
                        {
                            MProcessPara parass = new MProcessPara(GetCtx(), sPara.GetAD_Process_Para_ID(), null);
                            if (DisplayType.IsLookup(parass.GetAD_Reference_ID()))
                            {
                                if (sPara.GetColumnName().ToLower() == "ad_org_id")
                                {
                                    DataSet ds = DB.ExecuteDataset(@"SELECT
                                                                  (SELECT columnname FROM AD_Column WHERE isidentifier='Y'
                                                                  AND AD_Table_ID = (SELECT AD_Table_ID FROM AD_Table WHERE tableName='AD_Org' )
                                                                  ) AS name FROM AD_Org WHERE AD_Org_ID=" + value);
                                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                                    {
                                        for (int a = 0; a < ds.Tables[0].Rows.Count; a++)
                                        {
                                            if (a > 0)
                                            {
                                                colValue += "||'-'||";
                                            }
                                            colValue += " " + ds.Tables[0].Rows[a]["name"].ToString();
                                        }
                                        colValue = DB.ExecuteScalar("SELECT " + colValue + " FROM AD_Org WHERE AD_Org_ID=" + value);
                                    }

                                }
                                else if (sPara.GetColumnName().ToLower() == "ad_client_id")
                                {
                                    DataSet ds = DB.ExecuteDataset(@"SELECT
                                                                  (SELECT columnname FROM AD_Column WHERE isidentifier='Y'
                                                                  AND AD_Table_ID = (SELECT AD_Table_ID FROM AD_Table WHERE tableName='AD_Client' )
                                                                  ) AS name FROM AD_Client WHERE AD_Client_ID=" + value);
                                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                                    {
                                        for (int a = 0; a < ds.Tables[0].Rows.Count; a++)
                                        {
                                            if (a > 0)
                                            {
                                                colValue += "||'-'||";
                                            }
                                            colValue += " " + ds.Tables[0].Rows[a]["name"].ToString();
                                        }
                                        colValue = DB.ExecuteScalar("SELECT " + colValue + " FROM AD_Client WHERE AD_Client_ID=" + value);
                                    }
                                }
                                else if (sPara.GetColumnName().ToLower() == "ad_user_id")
                                {
                                    DataSet ds = DB.ExecuteDataset(@"SELECT
                                                                  (SELECT columnname FROM AD_Column WHERE isidentifier='Y'
                                                                  AND AD_Table_ID = (SELECT AD_Table_ID FROM AD_Table WHERE tableName='AD_User' )
                                                                  ) AS name FROM AD_User WHERE AD_User_ID=" + value);

                                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                                    {
                                        for (int a = 0; a < ds.Tables[0].Rows.Count; a++)
                                        {
                                            if (a > 0)
                                            {
                                                colValue += "||'-'||";
                                            }
                                            colValue += " " + ds.Tables[0].Rows[a]["name"].ToString();
                                        }
                                        colValue = DB.ExecuteScalar("SELECT " + colValue + " FROM AD_User WHERE AD_User_ID=" + value);
                                    }

                                }
                            }
                        }
                        catch
                        {

                        }

                        //	Convert to Type
                        try
                        {
                            if (DisplayType.IsNumeric(sPara.GetDisplayType())
                                || DisplayType.IsID(sPara.GetDisplayType()))
                            {
                                Decimal? bd = null;
                                if (value is Decimal)
                                    bd = (Decimal)value;
                                else if (value is int)
                                    bd = decimal.Parse(value.ToString());
                                else
                                    bd = decimal.Parse(value.ToString());
                                iPara.SetP_Number((decimal)bd);
                                if (!string.IsNullOrEmpty(colValue.ToString()))
                                {
                                    iPara.SetInfo(colValue.ToString());
                                }
                                else
                                {
                                    iPara.SetInfo(bd.ToString());
                                }
                                log.Fine(sPara.GetColumnName() + " = " + variable + " (=" + bd + "=)");
                            }
                            else if (DisplayType.IsDate(sPara.GetDisplayType()))
                            {
                                DateTime? ts = null;
                                if (value is DateTime)
                                    ts = (DateTime)value;
                                else
                                    ts = DateTime.Parse(value.ToString());
                                //if (DisplayType.Date == sPara.GetDisplayType())
                                //{
                                iPara.SetP_Date(ts);
                                //}
                                //else if (DisplayType.DateTime == sPara.GetDisplayType())
                                //{
                                //    iPara.SetP_Date_Time(ts);
                                //}
                                //else if (DisplayType.Time == sPara.GetDisplayType())
                                //{
                                //    iPara.SetP_Time(ts);

                                //}

                                iPara.SetInfo(value.ToString());
                                log.Fine(sPara.GetColumnName() + " = " + variable + " (=" + ts + "=)");
                            }
                            else
                            {
                                iPara.SetP_String(value.ToString());
                                iPara.SetInfo(value.ToString());
                                log.Fine(sPara.GetColumnName()
                                        + " = " + variable
                                        + " (=" + value + "=) " + value.GetType().FullName);
                            }


                            if (!iPara.Save())
                                log.Warning("Not Saved - " + sPara.GetColumnName());
                        }
                        catch (Exception e)
                        {
                            String msg = sPara.GetColumnName()
                                + " = " + variable + " (" + value
                                + ") " + value.GetType().FullName
                                + " - " + e.Message;
                            log.Warning(msg);
                            if (sb == null)
                                sb = new StringBuilder(msg);
                            else
                                sb.Append("; ").Append(msg);
                        }
                        break;
                    }	//	parameter match
                }	//	scheduler parameter loop
            }	//	instance parameter loop

            if (sb == null)
                return null;
            return sb.ToString();
        }	//	fillParameter

        public DateTime[] CheckProcessTime(int AD_SCHEDULE_ID, MScheduler scheduler)
        {
            MSchedule schedule = new MSchedule(GetCtx(), AD_SCHEDULE_ID, Get_TrxName());
            DateTime? dtNextRun;
            bool blNextDate = false;
            try
            {
                dtNextRun = scheduler.GetUpdated();
                if (schedule.GetScheduleHour() > 0)
                {
                    //blNextDate = true;
                    dtNextRun = dtNextRun.Value.Subtract(new TimeSpan(dtNextRun.Value.Hour, 0, 0));
                    dtNextRun = dtNextRun.Value.AddHours(schedule.GetScheduleHour());
                }
                if (schedule.GetScheduleMinute() > 0)
                {
                    //blNextDate = true;
                    dtNextRun = dtNextRun.Value.Subtract(new TimeSpan(0, dtNextRun.Value.Minute, 0));
                    dtNextRun = dtNextRun.Value.AddMinutes(schedule.GetScheduleMinute());
                }
            }
            catch
            {
                dtNextRun = DateTime.Now;

            }
            if ((scheduler.GetDateNextRun() != null) && (!blNextDate))
                dtNextRun = (DateTime)this.GetDateNextRun();

            DateTime[] dtSchedule = schedule.GetNext((DateTime)dtNextRun, 1);
            DateTime[] returnValue = new DateTime[2];
            if (DateTime.Now.ToString("dd-MM-yyyy hh:mm") == dtNextRun.Value.ToString("dd-MM-yyyy hh:mm"))
            {
                returnValue[0] = DateTime.Parse(dtNextRun.ToString());
                returnValue[1] = DateTime.Parse(dtSchedule[0].ToString());

                //scheduler.SetDateLastRun(DateTime.Parse(dtNextRun.ToString()));
                //dtSchedule = schedule.GetNext(DateTime.Parse(dtSchedule[0].ToString()), 1);
                //scheduler.SetDateNextRun(DateTime.Parse(dtSchedule[0].ToString()));
                //if (!scheduler.Save())
                //return false;
                return returnValue;
            }
            else
                return returnValue;
        }

        /** Scheduler Result		*/
        private static int AD_Message_ID = 884;		//	HARDCODED SchedulerResult

        private bool NotifySupervisor(bool success, String message, byte[] report)
        {
            if (m_supervisor == null)
                m_supervisor = MUser.Get(GetCtx(), GetSupervisor_ID());
            //	Send Mail
            // if (m_supervisor.IsNotificationEMail())
            //{
            MClient client = MClient.Get(GetCtx(), GetAD_Client_ID());
            String subject = client.GetName() + ": " + GetName();

            SendEMail(client, GetSupervisor_ID(), null, subject, message, null, false, 0, 0, report);
            //if (client.SendEMail(GetSupervisor_ID(), subject, message, attachmentFile))
            // return true;
            // }
            //	Create Notice
            MNote note = new MNote(GetCtx(), AD_Message_ID, GetSupervisor_ID(), null);
            // changes done by Bharat on 22 May 2018 to set Organization to * on Notification as discussed with Mukesh Sir.
            //note.SetClientOrg(GetAD_Client_ID(), GetAD_Org_ID());
            note.SetClientOrg(GetAD_Client_ID(), 0);
            note.SetTextMsg(GetName());
            note.SetDescription(message);
            note.SetRecord(Table_ID, Get_ID());		//	point to this
            bool ok = note.Save();
            //	Attachment
            if (ok && (report != null))
            {
                MAttachment attachment = new MAttachment(GetCtx(), X_AD_Note.Table_ID, note.GetAD_Note_ID(), null);
                attachment.SetClientOrg(GetAD_Client_ID(), GetAD_Org_ID());
                // attachment.AddEntry(attachmentFile.FullName);
                if (isDocxFile)
                {
                    attachment.AddEntry("Report_" + DateTime.Now.Ticks + ".Docx", report);
                }
                else
                {
                    attachment.AddEntry("Report_" + DateTime.Now.Ticks + ".pdf", report);
                }
                attachment.SetTextMsg(GetName());
                attachment.Save();

            }
            return ok;
        }	//	sendEMail


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="cParams"></param>
        ///// <returns></returns>
        //public string RunCrystalReport()
        //{

        //    string reportPath = System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "CReports\\Reports");
        //    string reportImagePath = System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "CReports\\Images");

        //    System.Globalization.CultureInfo systemCulture = Thread.CurrentThread.CurrentCulture;

        //    if (!string.IsNullOrEmpty(GetCtx().GetAD_Language().Replace('_', '-')))
        //    {
        //        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(GetCtx().GetAD_Language().Replace('_', '-'));
        //        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(GetCtx().GetAD_Language().Replace('_', '-'));
        //    }

        //    MForm form = new MForm(GetCtx(), GetAD_Form_ID(), Get_TrxName());

        //    //CrystalParameter para = new CrystalParameter(windowNo);
        //    int AD_CrystalInstance_ID = 0;
        //    MCrystalInstance instance = null;
        //    try
        //    {
        //        instance = new MCrystalInstance(Env.GetContext(), GetAD_Form_ID(), 0);
        //        if (!instance.Save())
        //        {
        //            return Msg.GetMsg(GetCtx(), "");
        //        }
        //        AD_CrystalInstance_ID = instance.GetAD_CrystalInstance_ID();
        //    }
        //    catch
        //    {
        //        return Msg.GetMsg(GetCtx(), "CrystalInstanceNotSaved");
        //    }

        //    if (!String.IsNullOrEmpty(form.GetProcedureName()))
        //    {
        //        //bool b = StartDBProcess(form.GetProcedureName(), cParams.ColNames, cParams.ColValues);
        //        bool b = StartDBProcess(form.GetProcedureName(), null, null);
        //        if (!b)
        //        {
        //            //serviceError = new CustomException(new Exception("CouldNotExecuteProcedure"));
        //            Thread.CurrentThread.CurrentCulture = systemCulture;
        //            Thread.CurrentThread.CurrentUICulture = systemCulture;

        //            return Msg.GetMsg(GetCtx(), "ProcedureNotExecuted");
        //        }
        //    }

        //    //serviceError = null;
        //    ProcessInfoParameter[] parameters = ProcessInfoUtil.SetCrystalParameterFromDB(AD_CrystalInstance_ID);
        //    string _ReportImagePath = "";
        //    string _ReportPath = "";

        //    if (form.IsReport())
        //    {
        //        string path = reportPath;
        //        _ReportImagePath = reportImagePath;
        //        if (String.IsNullOrEmpty(path))
        //        {

        //        }

        //        if (form.GetReportPath().IndexOf(":") < 0)
        //            _ReportPath = path + "\\" + form.GetReportPath();
        //        else
        //            _ReportPath = form.GetReportPath();
        //    }
        //    else
        //    {
        //        return Msg.GetMsg(GetCtx(), "FormIsNotReport");
        //    }

        //    ReportDocument rptBurndown = new ReportDocument();
        //    if (File.Exists(_ReportPath))   //Check if the crystal report file exists in a specified location.
        //    {
        //        try
        //        {
        //            rptBurndown.Load(_ReportPath);

        //            //Set Connection Info
        //            ConnectionInfo.Get().SetAttributes(connectionString);


        //            //Application will pick database info from the property file.
        //            CrystalDecisions.Shared.ConnectionInfo crDbConnection = new CrystalDecisions.Shared.ConnectionInfo();
        //            crDbConnection.IntegratedSecurity = false;
        //            crDbConnection.DatabaseName = ConnectionInfo.Get().Db_name;
        //            crDbConnection.UserID = ConnectionInfo.Get().Db_uid;
        //            crDbConnection.Password = ConnectionInfo.Get().Db_pwd;
        //            //crDbConnection.Type = ConnectionInfoType.Unknown;
        //            crDbConnection.ServerName = ConnectionInfo.Get().Db_host;
        //            CrystalDecisions.CrystalReports.Engine.Database crDatabase = rptBurndown.Database;
        //            CrystalDecisions.Shared.TableLogOnInfo oCrTableLoginInfo;
        //            foreach (CrystalDecisions.CrystalReports.Engine.Table oCrTable in crDatabase.Tables)
        //            {
        //                crDbConnection.IntegratedSecurity = false;
        //                crDbConnection.DatabaseName = ConnectionInfo.Get().Db_name;
        //                crDbConnection.UserID = ConnectionInfo.Get().Db_uid;
        //                crDbConnection.Password = ConnectionInfo.Get().Db_pwd;
        //                //crDbConnection.Type = ConnectionInfoType.Unknown;
        //                crDbConnection.ServerName = ConnectionInfo.Get().Db_host;

        //                oCrTableLoginInfo = oCrTable.LogOnInfo;
        //                oCrTableLoginInfo.ConnectionInfo = crDbConnection;
        //                oCrTable.ApplyLogOnInfo(oCrTableLoginInfo);
        //            }

        //            //Create Parameter query
        //            string sql = form.GetSqlQuery();
        //            StringBuilder sb = new StringBuilder(" Where ");
        //            if (parameters.Count() > 0)
        //            {
        //                int loopCount = 0;
        //                for (int para = 0; para <= parameters.Count() - 1; para++)
        //                {
        //                    string sInfo = parameters[para].GetInfo();
        //                    string sInfoTo = parameters[para].GetInfo_To();
        //                    if ((String.IsNullOrEmpty(sInfo) && String.IsNullOrEmpty(sInfoTo)) || sInfo == "NULL")
        //                    {
        //                        continue;
        //                    }

        //                    if (loopCount > 0)
        //                        sb.Append(" And ");
        //                    string paramName = parameters[para].GetParameterName();
        //                    object paramValue = parameters[para].GetParameter();
        //                    object paramValueTo = parameters[para].GetParameter_To();

        //                    if (paramValue is DateTime)
        //                    {
        //                        sb.Append(paramName).Append(" Between ").Append(GlobalVariable.TO_DATE((DateTime)paramValue, true));
        //                        if (paramValueTo != null && paramValueTo.ToString() != String.Empty)
        //                            sb.Append(" And ").Append(GlobalVariable.TO_DATE(((DateTime)paramValueTo).AddDays(1), true));
        //                        else
        //                            sb.Append(" And ").Append(GlobalVariable.TO_DATE(((DateTime)paramValue).AddDays(1), true));

        //                    }
        //                    else
        //                    {
        //                        sb.Append("Upper(").Append(paramName).Append(")").Append(" = Upper(")
        //                            .Append(GlobalVariable.TO_STRING(paramValue.ToString()) + ")");
        //                    }

        //                    loopCount++;
        //                }

        //                if (sb.Length > 7)
        //                    sql = sql + sb.ToString();
        //            }

        //            if (form.IsIncludeProcedure())
        //            {
        //                bool result = StartDBProcess(form.GetProcedureName(), parameters);
        //            }

        //            DataSet ds = DB.ExecuteDataset(sql);

        //            if (ds == null)
        //            {
        //                ValueNamePair error = VLogger.RetrieveError();
        //                return Msg.GetMsg(GetCtx(), "NoRecords");
        //            }

        //            bool imageError = false;
        //            if (form.IsIncludeImage())
        //            {
        //                for (int i_img = 0; i_img <= ds.Tables[0].Rows.Count - 1; i_img++)
        //                {
        //                    String ImagePath = "";
        //                    String ImageField = "";
        //                    if (ds.Tables[0].Rows[i_img][form.GetImagePathField()] != null)
        //                    {
        //                        ImagePath = ds.Tables[0].Rows[i_img][form.GetImagePathField()].ToString();
        //                        ImageField = form.GetImageField();

        //                        if (ds.Tables[0].Columns.Contains(ImageField))
        //                        {
        //                            if (File.Exists(_ReportImagePath + "\\" + ImagePath))
        //                            {
        //                                byte[] b = StreamFile(_ReportImagePath + "\\" + ImagePath);
        //                                ds.Tables[0].Rows[i_img][ImageField] = b;
        //                            }
        //                            else
        //                            {
        //                                imageError = true;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            imageError = true;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        imageError = true;
        //                    }
        //                }
        //            }

        //            if (imageError)
        //            {
        //                //   ShowMessage.Error("ErrorLoadingSomeImages", true);
        //            }

        //            System.IO.Stream oStream;
        //            byte[] report = null;

        //            rptBurndown.SetDataSource(ds.Tables[0]);                //By karan approveed by lokesh......
        //            //rptBurndown.PrintOptions.ApplyPageMargins(new CrystalDecisions.Shared.PageMargins(100, 360, 100, 360));
        //            oStream = rptBurndown.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        //            report = new byte[oStream.Length];
        //            oStream.Read(report, 0, Convert.ToInt32(oStream.Length));

        //            Thread.CurrentThread.CurrentCulture = systemCulture;
        //            Thread.CurrentThread.CurrentUICulture = systemCulture;

        //            int AD_Table_ID = 0;
        //            int Record_ID = 0;

        //            int AD_Message_ID = 884;		//	HARDCODED SchedulerResult
        //            int[] userIDs = GetRecipientAD_User_IDs();

        //            bool success = false;
        //            if (report != null)
        //            {
        //                _emails = new List<String>();
        //                for (int i = 0; i < userIDs.Length; i++)
        //                {
        //                    MNote note = new MNote(GetCtx(), AD_Message_ID, userIDs[i], Get_TrxName());
        //                    note.SetClientOrg(GetAD_Client_ID(), GetAD_Org_ID());
        //                    note.SetTextMsg(GetName());
        //                    note.SetDescription(GetDescription());
        //                    note.SetRecord(AD_Table_ID, Record_ID);
        //                    note.Save();

        //                    MAttachment attachment = new MAttachment(GetCtx(), MNote.Table_ID, note.GetAD_Note_ID(), Get_TrxName());
        //                    attachment.SetClientOrg(GetAD_Client_ID(), GetAD_Org_ID());
        //                    attachment.AddEntry("Report_" + DateTime.Now.Ticks + ".pdf", report);
        //                    attachment.SetTextMsg(GetName());
        //                    attachment.Save();

        //                    MClient client = MClient.Get(GetCtx(), GetAD_Client_ID());

        //                    success = SendEMail(client, userIDs[i], null, GetName(), GetDescription(), null, true, 0, 0, report);

        //                }
        //            }

        //            NotifySupervisor(success, GetDescription(), report);

        //            return Msg.GetMsg(GetCtx(), "ProcessCompleted");

        //        }
        //        catch (Exception ex)
        //        {
        //            Thread.CurrentThread.CurrentCulture = systemCulture;
        //            Thread.CurrentThread.CurrentUICulture = systemCulture;
        //            return Msg.GetMsg(GetCtx(), ex.Message);
        //        }
        //    }
        //    else
        //    {
        //        return Msg.GetMsg(GetCtx(), "ReportNotFound");
        //    }
        //}

        ///// <summary>
        ///// Start the DB Process
        ///// </summary>
        ///// <param name="procedureName">name of the process</param>
        ///// <returns></returns>
        //private bool StartDBProcess(String procedureName, ProcessInfoParameter[] list)
        //{
        //    if (DatabaseType.IsPostgre)  //jz Only DB2 not support stored procedure now
        //    {
        //        return false;
        //    }

        //    //  execute on this thread/connection
        //    //String sql = "{call " + procedureName + "(" + _pi.GetAD_PInstance_ID() + ")}";
        //    try
        //    {
        //        //only oracle procedure are supported
        //        OracleCommand comm = new OracleCommand();
        //        OracleConnection conn = (OracleConnection)VAdvantage.DataBase.DB.GetConnection();
        //        conn.Open();
        //        comm.Connection = conn;
        //        comm.CommandText = procedureName;
        //        comm.CommandType = CommandType.StoredProcedure;
        //        OracleCommandBuilder.DeriveParameters(comm);
        //        OracleParameter[] param = new OracleParameter[comm.Parameters.Count];
        //        int i = 0;
        //        StringBuilder orclParams = new StringBuilder();
        //        bool isDateTo = false;
        //        foreach (OracleParameter orp in comm.Parameters)
        //        {
        //            if (isDateTo)
        //            {
        //                isDateTo = false;
        //                continue;
        //            }
        //            Object paramvalue = list[i].GetParameter();
        //            if (paramvalue != null)
        //            {
        //                if (orp.DbType == System.Data.DbType.DateTime)
        //                {
        //                    if (paramvalue.ToString().Length > 0)
        //                    {
        //                        paramvalue = ((DateTime)paramvalue).ToString("dd-MMM-yyyy");
        //                    }
        //                    param[i] = new OracleParameter(orp.ParameterName, paramvalue);
        //                    if (list[i].GetParameter_To().ToString().Length > 0)
        //                    {
        //                        paramvalue = list[i].GetParameter_To();
        //                        paramvalue = ((DateTime)paramvalue).ToString("dd-MMM-yyyy");
        //                        param[i + 1] = new OracleParameter(comm.Parameters[i + 1].ParameterName, paramvalue);
        //                        i++;
        //                        isDateTo = true;
        //                        continue;
        //                    }
        //                    else
        //                    {
        //                        if (comm.Parameters.Count > (i + 1))
        //                        {
        //                            if (comm.Parameters[i + 1].ParameterName.Equals(comm.Parameters[i].ParameterName + "_TO", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                param[i + 1] = new OracleParameter(comm.Parameters[i + 1].ParameterName, paramvalue);
        //                                isDateTo = true;
        //                                continue;
        //                            }
        //                        }
        //                    }
        //                }
        //                else if (orp.DbType == System.Data.DbType.VarNumeric)
        //                {
        //                    if (paramvalue.ToString().Length > 0)
        //                    {
        //                        //continue;
        //                    }
        //                    else
        //                        paramvalue = 0;
        //                }
        //                else
        //                {
        //                    if (paramvalue.ToString().Length > 0)
        //                    {
        //                        paramvalue = GlobalVariable.TO_STRING(paramvalue.ToString());
        //                    }
        //                }

        //            }
        //            param[i] = new OracleParameter(orp.ParameterName, paramvalue);
        //            //orclParams.Append(orp.ParameterName).Append(": ").Append(_curTab.GetValue(list[i]));
        //            //if (i < comm.Parameters.Count - 1)
        //            //    orclParams.Append(", ");
        //            i++;
        //        }

        //        //log.Fine("Executing " + procedureName + "(" + _pi.GetAD_PInstance_ID() + ")");
        //        int res = VAdvantage.SqlExec.Oracle.OracleHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, procedureName, param);
        //        //DataBase.DB.ExecuteQuery(sql, null);
        //    }
        //    catch (Exception e)
        //    {
        //        VLogger.Get().SaveError(e.Message, e);
        //        //log.Log(Level.SEVERE, "Error executing procedure " + procedureName, e);
        //        return false;

        //    }
        //    //	log.fine(Log.l4_Data, "ProcessCtl.startProcess - done");
        //    return true;
        //}

        //private bool StartDBProcess(String procedureName, List<String> list, List<object> values)
        //{
        //    if (DatabaseType.IsPostgre)  //jz Only DB2 not support stored procedure now
        //    {
        //        return false;
        //    }

        //    //  execute on this thread/connection
        //    //String sql = "{call " + procedureName + "(" + _pi.GetAD_PInstance_ID() + ")}";
        //    try
        //    {
        //        //only oracle procedure are supported
        //        OracleCommand comm = new OracleCommand();
        //        OracleConnection conn = (OracleConnection)VAdvantage.DataBase.DB.GetConnection();
        //        conn.Open();
        //        comm.Connection = conn;
        //        comm.CommandText = procedureName;
        //        comm.CommandType = CommandType.StoredProcedure;
        //        OracleCommandBuilder.DeriveParameters(comm);
        //        OracleParameter[] param = new OracleParameter[list.Count];
        //        int i = 0;
        //        StringBuilder orclParams = new StringBuilder();
        //        foreach (OracleParameter orp in comm.Parameters)
        //        {
        //            param[i] = new OracleParameter(orp.ParameterName, values[i]);
        //            orclParams.Append(orp.ParameterName).Append(": ").Append(values[i]);
        //            if (i < comm.Parameters.Count - 1)
        //                orclParams.Append(", ");
        //            i++;
        //        }

        //        //log.Fine("Executing " + procedureName + "(" + _pi.GetAD_PInstance_ID() + ")");
        //        int res = VAdvantage.SqlExec.Oracle.OracleHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, procedureName, param);
        //        //DataBase.DB.ExecuteQuery(sql, null);
        //    }
        //    catch (Exception e)
        //    {
        //        VLogger.Get().SaveError(e.Message, e);
        //        //log.Log(Level.SEVERE, "Error executing procedure " + procedureName, e);
        //        return false;

        //    }
        //    //	log.fine(Log.l4_Data, "ProcessCtl.startProcess - done");
        //    return true;
        //}

        //private byte[] StreamFile(string filename)
        //{
        //    FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);

        //    // Create a byte array of file stream length
        //    byte[] ImageData = new byte[fs.Length];

        //    //Read block of bytes from stream into the byte array
        //    fs.Read(ImageData, 0, System.Convert.ToInt32(fs.Length));

        //    //Close the File Stream
        //    fs.Close();
        //    return ImageData; //return the byte data
        //}
    }
}
