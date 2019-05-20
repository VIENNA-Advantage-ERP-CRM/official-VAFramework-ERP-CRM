using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Logging;
using System.Runtime.CompilerServices;
using VAdvantage.Utility;
using VAdvantage.Model;
using VAdvantage.WF;
using System.Threading;
using System.Net;
using VAdvantage.Print;
using VAdvantage.Classes;
using System.IO;
using System.Reflection;
using VAdvantage.DataBase;


namespace VAdvantage.Server
{
    public class ViennaServerMgr
    {
        static object lockThread = new object();

        public static string invokeIISUrl = "";

        //MHDF1065436570
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static ViennaServerMgr Get()
        {
            if (s_serverMgr == null)
            {
                //	for faster subsequent calls
                s_serverMgr = new ViennaServerMgr();
                s_serverMgr.StartAll(true);
                s_serverMgr.log.Info(s_serverMgr.ToString());
            }
            return s_serverMgr;
        }	//	get


        public static bool Restart()
        {
            if (s_serverMgr == null)
            {
                Get();
                return true;
            }
            s_serverMgr.StopAll();
            return true;
        }

        /**	Singleton					*/
        private static ViennaServerMgr s_serverMgr = null;
        /**	Logger						*/
        protected  VLogger log = null;

        /// <summary>
        /// for outside calling. Normally singleton object is allowed 
        /// </summary>
        /// <param name="doTrace"></param>
        public ViennaServerMgr(bool doTrace)
            : base()
        {
            //create log console
            log = VLogger.GetVLogger(this.GetType().FullName);
            StartEnvironment(doTrace);  //start the environment
        }

        public ViennaServerMgr()
            : base()
        {
            log = VLogger.GetVLogger(this.GetType().FullName);
            StartEnvironment(true);
        }



        /**	The Servers				*/
        private List<ViennaServer> m_servers = new List<ViennaServer>();
        /** Context					*/
        private Context m_ctx = Env.GetContext();
        /** Start					*/
        //private Timestamp		m_start = new Timestamp(System.currentTimeMillis());

        public Ctx GetCtx()
        {
            return m_ctx;
        }	//	getCtx

        private bool StartEnvironment(bool doTrace)
        {
            //Program.StartUp(false, "ServerMgr");    //start up the ServerMgr class (Loads the initial value to run the application)
            if (doTrace)
                VLogMgt.SetLevel(Level.ALL);    //in case of server side, we will always open the trace to ALL
            else
                VLogMgt.SetLevel(Level.OFF);
            log.Info("");

            String Remote_Addr = "Unknown";
            String Remote_Host = null;
            try
            {
                IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
                if (ips.Length > 0)
                    Remote_Addr = ips[0].ToString();    //pick the one tht comes first (in case of multiple IP's)
                Remote_Host = Dns.GetHostName();
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "No Local Host", e);
            }


            //	Set Session
            MSession session = MSession.Get(GetCtx(), "Z", true, Remote_Addr, Remote_Host, "Server");
            //
            return session != null;
        }	//	startEnvironment


        private bool doStartAcctProcessor = false;

        public bool DoStartAcctProcessor
        {
            get { return doStartAcctProcessor; }
            set { doStartAcctProcessor = value; }
        }
        private bool doStartRequestProcessor = false;

        public bool DoStartRequestProcessor
        {
            get { return doStartRequestProcessor; }
            set { doStartRequestProcessor = value; }
        }
        private bool doWorkflowProcessor = false;

        public bool DoWorkflowProcessor
        {
            get { return doWorkflowProcessor; }
            set { doWorkflowProcessor = value; }
        }
        private bool doStartAlertProcessor = false;

        public bool DoStartAlertProcessor
        {
            get { return doStartAlertProcessor; }
            set { doStartAlertProcessor = value; }
        }
        private bool doStartScheduler = false;

        public bool DoStartScheduler
        {
            get { return doStartScheduler; }
            set { doStartScheduler = value; }
        }

        private bool doStartMSMQ = false;
        public bool DoStartMSMQ
        {
            get { return doStartMSMQ; }
            set { doStartMSMQ = value; }
        }


        private bool doInvokeService = false;
        public bool DoInvokeService
        {
            get { return doInvokeService; }
            set { doInvokeService = value; }
        }

        private void RequeryAll()
        {
            if (m_servers.Count() > 0)
                log.Config("Current #" + m_servers.Count());

            //	Accounting
            if (doStartAcctProcessor)
            {
                MAcctProcessor[] acctModels = MAcctProcessor.GetActive(m_ctx);
                for (int i = 0; i < acctModels.Length; i++)
                {
                    MAcctProcessor pModel = acctModels[i];
                    ViennaServer server = ViennaServer.Create(pModel);
                    AddServer(server, i);
                }
            }
            ////	Request

            if (doStartRequestProcessor)
            {
                MRequestProcessor[] requestModels = MRequestProcessor.GetActive(m_ctx);
                for (int i = 0; i < requestModels.Length; i++)
                {
                    MRequestProcessor pModel = requestModels[i];
                    ViennaServer server = ViennaServer.Create(pModel);
                    AddServer(server, i);
                }
            }
            ////	Workflow

            if (doWorkflowProcessor)
            {
                MWorkflowProcessor[] workflowModels = MWorkflowProcessor.GetActive(m_ctx);
                for (int i = 0; i < workflowModels.Length; i++)
                {
                    MWorkflowProcessor pModel = workflowModels[i];
                    ViennaServer server = ViennaServer.Create(pModel);
                    AddServer(server, i);
                }
            }

            ////	Alert Process (Notice and send mail) This is a test after exclusion
            if (doStartAlertProcessor)
            {
                MAlertProcessor[] alertModels = MAlertProcessor.GetActive(m_ctx);
                for (int i = 0; i < alertModels.Length; i++)
                {
                    //All Alert processor server to the server list
                    MAlertProcessor pModel = alertModels[i];
                    ViennaServer server = ViennaServer.Create(pModel);
                    AddServer(server, i);
                }
            }

            ////	Scheduler
            if (doStartScheduler)
            {
                MScheduler[] schedulerModels = MScheduler.GetActive(m_ctx);
                for (int i = 0; i < schedulerModels.Length; i++)
                {
                    MScheduler pModel = schedulerModels[i];
                    ViennaServer server = ViennaServer.Create(pModel);
                    AddServer(server, i);
                }
            }

            if (doInvokeService)
            {
                AddServer(new InvokeIISServiceProcessor(), 0);
            }

            log.Config("#" + m_servers.Count());
        }	//	requeryAll


        private bool AddServer(ViennaServer server, int seq)
        {
            if (m_servers.Contains(server))
                return false;

            m_servers.Add(server);

            if (server is InvokeIISServiceProcessor)
                server.InitThread(seq, "InvokeIISService");
            else
                server.InitThread(seq);    //initializes the thread object

            server.GetServerThread().Priority = ThreadPriority.Normal;
            server.Start();
            return true;
        }	//	addServer


        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool StartAll(bool requery)
        {
            log.Info("Start New log================================>" + DateTime.Now.Millisecond.ToString());
            if (requery)
                RequeryAll();
            foreach (ViennaServer vserver in m_servers)
            {
                try
                {
                    Thread server = vserver.GetServerThread();
                    if (server.IsAlive)
                        continue;
                    //	Wait until dead
                    if (vserver.IsInterrupted())
                    {
                        int maxWait = 10;	//	10 iterations = 1 sec
                        while (server.IsAlive)
                        {
                            if (maxWait-- == 0)
                            {
                                log.Severe("Wait timeout for interruped " + server);
                                break;
                            }
                            try
                            {
                                Thread.Sleep(100);		//	1/10 sec
                            }
                            catch (ThreadInterruptedException e)
                            {
                                log.Log(Level.SEVERE, "While sleeping", e);
                            }
                        }
                    }
                    //	Do start
                    if (!server.IsAlive)
                    {
                        //	replace
                        //	server = ViennaServer.create (server.getModel());
                        vserver.Start();
                        server.Priority = ThreadPriority.Normal - 2; // (Thread.NORM_PRIORITY-2);
                    }
                }
                catch (Exception e)
                {
                    log.Log(Level.WARNING, "Server: " + vserver, e);
                }
            }	//	for all servers

            //	Final Check
            int noRunning = 0;
            int noStopped = 0;
            foreach (ViennaServer vserver in m_servers)
            {
                try
                {
                    Thread server = vserver.GetServerThread();
                    if (server.IsAlive)
                    {
                        log.Info("Alive: " + server);
                        noRunning++;
                    }
                    else
                    {
                        log.Warning("Dead: " + server);
                        noStopped++;
                    }
                }
                catch (Exception e)
                {
                    log.Log(Level.SEVERE, "(checking) - " + vserver, e);
                    noStopped++;
                }
            }

            log.Info("Running=" + noRunning + ", Stopped=" + noStopped);
            //ViennaServerGroup.Get().Dump();
            return noStopped == 0;
        }

        /// <summary>
        /// new funtion From remote server side
        /// </summary>
        /// <param name="processOption"></param>
        /// <param name="traceFile"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool StartAll(string processOption, bool traceFile)
        {
            log.Info("Start New log================================>" + DateTime.Now.Millisecond.ToString());
            try
            {
                s_serverMgr = new ViennaServerMgr(traceFile);

            }
            catch (Exception ex)
            {
                log.SaveError("Restart the Vienna Server", ex);
                return false;
            }
            string[] arry = processOption.Substring(0, processOption.Length - 1).Split(',');

            //Now  get Sever info from VServer form
            for (int i = 0; i < arry.Length; i++)
            {
                if (arry[i].Equals("SP"))
                {
                    s_serverMgr.DoStartScheduler = true;
                }
                else if (arry[i].Equals("AP"))
                {
                    s_serverMgr.DoStartAcctProcessor = true;
                }
                else if (arry[i].Equals("AL"))
                {
                    s_serverMgr.DoStartAlertProcessor = true;
                }
                else if (arry[i].Equals("RP"))
                {
                    s_serverMgr.DoStartRequestProcessor = true;
                }
                else if (arry[i].Equals("WP"))
                {
                    s_serverMgr.DoWorkflowProcessor = true;
                }
                else if (arry[i].Equals("MS"))
                {
                    s_serverMgr.DoStartMSMQ = true;
                }
            }
            try
            {
                bool b = s_serverMgr.StartAll(true);
                if (b)
                {
                    if (s_serverMgr.DoStartMSMQ)
                    {
                        //MSMQServer sQ = new MSMQServer(Ini.s_prop.GetProperty("VIENNA_MAIL_SERVER"), Ini.s_prop.GetProperty("VIENNA_MAIL_USER"),  
                        //Ini.s_prop.GetProperty("VIENNA_MAIL_PASSWORD"), Ini.s_prop.GetProperty("VIENNA_ADMIN_EMAIL"));
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Modal library load connection
        /// </summary>
        /// <param name="dbUserName"></param>
        /// <param name="dbPws"></param>
        /// <param name="dbPortNo"></param>
        /// <param name="dbService"></param>
        /// <param name="dbIP"></param>
        /// <param name="ProcessFilePath"></param>
        /// <returns></returns>
        public static  string ReflectionIni(string dbUserName, string dbPws, string dbPortNo, string dbService, string dbIP, string ProcessFilePath)
        {
            string connection = "";
            try
            {
                VLogMgt.Initialize(true, ProcessFilePath);
                //log = VLogger.GetVLogger("VAdvantage.Server.ViennaServerMgr");
               // log.Info("Client Log***************************************************************");
                VLogMgt.SetLevel(Level.ALL); 
                connection = Ini.CreateConnectionString(dbIP, dbPortNo, dbUserName, dbPws, dbService);
               

                //Assembly asmShedular = Assembly.Load("ModelLibrary");
                //Type objClass = asmShedular.GetType("VAdvantage.DataBase.Ini");
                //Type[] parameterTypes = new Type[] { typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) };
                //Object[] args = new Object[] { dbIP, dbPortNo, dbUserName, dbPws, dbService };
                //MethodInfo m = objClass.GetMethod("CreateConnectionString", parameterTypes);
                //connection = (string)m.Invoke(null, args);

                //Type logClass = asmShedular.GetType("VAdvantage.Logging.VLogMgt");
                //Type[] logPara = new Type[] { typeof(bool), typeof(string) };
                //Object[] logArgs = new Object[] { true, ProcessFilePath };
                //MethodInfo mlog = logClass.GetMethod("Initialize", logPara);
                //mlog.Invoke(null, logArgs);
               
                //log.Info("Connection String=" + connection);
            }
            catch (Exception ev)
            {
               VLogger.Get().SaveError("Connection Error", ev);
            }
            VLogger.Get().Info("================================>"+ connection);
            return connection;
        }


        public bool Start(String serverID)
        {
            ViennaServer server = GetServer(serverID);
            if (server == null)
                return false;
            if (server.GetServerThread().IsAlive)
                return true;

            try
            {
                //	replace
                int index = m_servers.IndexOf(server);
                server = ViennaServer.Create(server.GetModel());
                if (server == null)
                    m_servers.RemoveAt(index);
                else
                    m_servers[index] = server;
                server.Start();
                server.SetPriority(ThreadPriority.Normal);

                Thread.Sleep(0);    //yeild other theads of lower priority to run
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "Server=" + serverID, e);
                return false;
            }
            log.Info(server.ToString());
            //ViennaServerGroup.Get().Dump();
            if (server == null)
                return false;
            return server.GetServerThread().IsAlive;
        }	//	startIt


        public ViennaServer GetServer(String serverID)
        {
            if (serverID == null)
                return null;
            for (int i = 0; i < m_servers.Count(); i++)
            {
                ViennaServer server = m_servers[i];
                if (serverID.Equals(server.GetServerID()))
                    return server;
            }
            return null;
        }	//	getServer


        protected ViennaServer[] GetActive()
        {
            List<ViennaServer> list = new List<ViennaServer>();
            for (int i = 0; i < m_servers.Count(); i++)
            {
                ViennaServer vserver = m_servers[i];
                Thread server = vserver.GetServerThread();
                if (server != null && server.IsAlive)
                    list.Add(vserver);
            }
            ViennaServer[] retValue = new ViennaServer[list.Count()];
            retValue = list.ToArray();
            return retValue;
        }	//	getActive

        public bool StopAll()
        {
            log.Info("Stop All log================================>" + DateTime.Now.Millisecond.ToString());
            ViennaServer[] servers = GetActive();
            //	Interrupt
            for (int i = 0; i < servers.Length; i++)
            {
                ViennaServer server = servers[i];
                try
                {
                    if (server.GetServerThread().IsAlive)
                    {
                        server.SetPriority(ThreadPriority.Highest);
                        server.Interrupt();
                    }
                }
                catch (Exception e)
                {
                    log.Log(Level.SEVERE, "(Interrupting) - " + server, e);
                }
            }	//	for all servers
            Thread.Sleep(0);    //yielding 

            //	Wait for death
            for (int i = 0; i < servers.Length; i++)
            {
                ViennaServer server = servers[i];
                try
                {
                    int maxWait = 10;	//	10 iterations = 1 sec
                    while (server.GetServerThread().IsAlive)
                    {
                        if (maxWait-- == 0)
                        {
                            log.Severe("Wait timeout for interruped " + server);
                            break;
                        }
                        Thread.Sleep(100);		//	1/10
                    }
                }
                catch (Exception e)
                {
                    log.Log(Level.SEVERE, "(Waiting) - " + server, e);
                }
            }	//	for all servers

            //	Final Check
            int noRunning = 0;
            int noStopped = 0;
            for (int i = 0; i < servers.Length; i++)
            {
                ViennaServer server = servers[i];
                try
                {
                    if (server.GetServerThread().IsAlive)
                    {
                        log.Warning("Alive: " + server);
                        noRunning++;
                    }
                    else
                    {
                        log.Info("Stopped: " + server);
                        noStopped++;
                    }
                }
                catch (Exception e)
                {
                    log.Log(Level.SEVERE, "(Checking) - " + server, e);
                    noRunning++;
                }
            }

            //	End Session
            MSession session = MSession.Get(Env.GetContext(), false);	//	finish
            if (session != null)
                session.Logout();
            m_servers.Clear();
            log.Fine("Running=" + noRunning + ", Stopped=" + noStopped);
            //ViennaServerGroup.Get().Dump();
            return noRunning == 0;
        }	//	stopAll


        public bool Stop(String serverID)
        {
            ViennaServer server = GetServer(serverID);
            if (server == null)
                return false;
            if (!server.GetServerThread().IsAlive)
                return true;

            try
            {
                server.GetServerThread().Interrupt();
                Thread.Sleep(10);	//	1/100 sec
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "Stop", e);
                return false;
            }
            log.Info(server.ToString());
            //ViennaServerGroup.Get().Dump();
            return !server.GetServerThread().IsAlive;
        }	//	stop

    }
}
