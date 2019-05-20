/******************************************************
 * Project Name   : VAdvantage
 * Class Name     : ViennaServer
 * Purpose        : Vienna Server Base
 * Class Used     : Threding
 * Chronological    Development
 * Raghunandan      12-Jan-2010
  *****************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Threading;
using System.Data;
using VAdvantage.Logging;
using System.Data.SqlClient;
using VAdvantage.WF;

namespace VAdvantage.Server
{
    public abstract class ViennaServer
    {
        public ViennaServer()
        {
            log = VLogger.GetVLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Create New Server Thead
        /// </summary>
        /// <param name="model"></param>
        /// <returns>server tread or null</returns>
        public static ViennaServer Create(ViennaProcessor model)
        {
            if (model is MRequestProcessor)
                return new RequestProcessor((MRequestProcessor)model);
            if (model is MWorkflowProcessor)
                return new WorkflowProcessor((MWorkflowProcessor)model);
            if (model is MAcctProcessor)
                return new AcctProcessor((MAcctProcessor)model);
            if (model is MAlertProcessor)
                return new AlertProcessor((MAlertProcessor)model);
            if (model is MScheduler)
                return new Scheduler((MScheduler)model);

            //if shedular is not working
            throw new ArgumentException("Unknown Processor");

            //return null;
        }


        #region PrivateVariables

        public Thread objThread = null;
        //	The Processor Model						
        protected ViennaProcessor _model;

        // Initial nap is seconds		
        private int _initialNap = 0;

        //	Miliseconds to sleep - 10 Min default	
        private long _sleepMS = 0;
        // Sleeping					
        private volatile bool _sleeping = false;

        // Server start time					
        private long _start = 0;

        // Number of Work executions	
        protected int _runCount = 0;

        // Tine start of work				
        protected long _startWork = 0;

        // Number MS of last Run		
        private long _runLastMS = 0;

        // Number of MS total			
        private long _runTotalMS = 0;
        // When to run next			
        private long _nextWork = 0;
        //	Logger						
        protected VLogger log = null;//Logging.VLogger.GetVLogger(getClass());
        //	Context						
        private Utility.Ctx _ctx = null;
        // System						
        protected static MSystem _system = null;
        // Client						
        protected MClient _client = null;

        #endregion



        /// <summary>
        /// Get Server Context
        /// </summary>
        /// <returns>context</returns>
        public Ctx GetCtx()
        {
            return _ctx;
        }

        /// <summary>
        /// Server Base Class
        /// </summary>
        /// <param name="model"></param>
        /// <param name="initialNap">delay time running in sec</param>
        protected ViennaServer(ViennaProcessor model, int initialNap)
      
        {

            log = VLogger.GetVLogger(this.GetType().FullName);
            _model = model;
            _ctx = new Ctx(model.GetCtx().GetMap());
            if (_system == null)
            {
                _system = MSystem.Get(_ctx);
            }
            _client = MClient.Get(_ctx);
            _ctx.SetContext("#AD_Client_ID", _client.GetAD_Client_ID());
            _initialNap = initialNap;
        }

        /// <summary>
        /// Returns the sleepMS.
        /// </summary>
        /// <returns></returns>
        public long GetSleepMS()
        {
            return _sleepMS;
        }

        /// <summary>
        /// Sleep for set time
        /// </summary>
        /// <returns>true if not interrupted</returns>
        public bool Sleep()
        {
            if (this.IsInterrupted())
            {
                log.Info(this.GetType().FullName + ": Interrupted");
                return false;
            }
            if (_sleepMS < 10)
            {
                return true;
            }
            //
            log.Fine(this.GetType().FullName + ": Sleeping " + TimeUtil.FormatElapsed(_sleepMS));
            _sleeping = true;
            try
            {
                Thread.Sleep(Convert.ToInt32(_sleepMS));
            }
            catch (ThreadInterruptedException e)
            {
                log.Info(this.GetType().FullName + ": Interrupted [Error] "+e.Message);
                _sleeping = false;
                return false;
            }
            _sleeping = false;
            return true;
        }

        /// <summary>
        /// 	Run Now
        /// </summary>
        public void RunNow()
        {
            log.Info(objThread.Name);
            _startWork = CommonFunctions.CurrentTimeMillis(DateTime.Now);
            DoWork();
            long now = CommonFunctions.CurrentTimeMillis(DateTime.Now);

            _runCount++;
            _runLastMS = now - _startWork;
            _runTotalMS += _runLastMS;
            //
            _model.SetDateLastRun(new DateTime(now));
            _model.Save();

            log.Fine(objThread.Name + ": " + GetStatistics());
        }


        public void RunInvokeIISService()
        {
            _start = CommonFunctions.CurrentTimeMillis(DateTime.Now);
            while (true)
            {
                long now = CommonFunctions.CurrentTimeMillis(DateTime.Now);

                //	---------------
                _startWork = CommonFunctions.CurrentTimeMillis(DateTime.Now);
                DoWork();
                now = CommonFunctions.CurrentTimeMillis(DateTime.Now);

                _sleepMS = 900000; // 15 min

                log.Fine(objThread.Name + ": " + GetStatistics());
                if (!Sleep())
                {
                    break;
                }
            }
            _start = 0;
        }

        /// <summary>
        /// Run async
        /// </summary>
        public void Run()
        {
            int AD_Schedule_ID = _model.GetAD_Schedule_ID();
            MSchedule schedule = null;
            if (AD_Schedule_ID != 0)
            {
                schedule = MSchedule.Get(GetCtx(), AD_Schedule_ID);
                if (!schedule.IsOKtoRunOnIP())
                {
                    log.Warning(objThread.Name + ": Stopped - IP Restriction " + schedule);
                    return;		//	done
                }
            }

            try
            {
                log.Fine(objThread.Name + ": Pre-Nap - " + _initialNap);
                Thread.Sleep(_initialNap * 1000);
            }
            catch (ThreadInterruptedException e)
            {
                log.Log(Level.SEVERE, objThread.Name + ": Pre-Nap Interrupted", e);
                return;
            }

            _start = CommonFunctions.CurrentTimeMillis(DateTime.Now);
            while (true)
            {
                long now = CommonFunctions.CurrentTimeMillis(DateTime.Now);
                if (_sleepMS == 0)
                    _sleepMS = CalculateSleep(now);
                DateTime scheduled = CommonFunctions.CovertMilliToDate(now + _sleepMS);
                DateTime? dateNextRun = null;
                if (_nextWork == 0)
                {
                    _nextWork = now + _sleepMS;
                    dateNextRun = GetDateNextRun(true);
                    _model.SetDateNextRun(dateNextRun);
                    _model.Save();
                }
                else
                    dateNextRun = CommonFunctions.CovertMilliToDate(_nextWork);

                log.Config(this.objThread.Name + ": NextWork=" + dateNextRun + " - Scheduled=" + scheduled);

                //	---------------
                _startWork = CommonFunctions.CurrentTimeMillis(DateTime.Now);
                DoWork();
                now = CommonFunctions.CurrentTimeMillis(DateTime.Now);
                //	---------------

                _runCount++;
                _runLastMS = now - _startWork;
                _runTotalMS += _runLastMS;
                //
                _sleepMS = CalculateSleep(now);
                _nextWork = now + _sleepMS;
                //
                _model.SetDateLastRun(CommonFunctions.CovertMilliToDate(now));
                _model.SetDateNextRun(CommonFunctions.CovertMilliToDate(_nextWork));
                _model.Save();

                log.Fine(objThread.Name + ": " + GetStatistics());
                if (!Sleep())
                {
                    break;
                }
            }
            _start = 0;
        }

        /// <summary>
        /// Get Run Statistics
        /// </summary>
        /// <returns>Statistic info</returns>
        public String GetStatistics()
        {
            return "Run #" + _runCount
                + " - Last=" + TimeUtil.FormatElapsed(_runLastMS)
                + " - Total=" + TimeUtil.FormatElapsed(_runTotalMS)
                + " - Next " + TimeUtil.FormatElapsed(_nextWork - CommonFunctions.CurrentTimeMillis(DateTime.Now));
        }

        /// <summary>
        /// Do the actual Work
        /// </summary>
        protected abstract void DoWork();

        /// <summary>
        /// Get Server Info
        /// </summary>
        /// <returns>info</returns>
        public abstract String GetServerInfo();

        /// <summary>
        /// Get Unique ID
        /// </summary>
        /// <returns>Unique ID</returns>
        public String GetServerID()
        {
            return _model.GetServerID();
        }

        /// <summary>
        /// Get the date Next run
        /// </summary>
        /// <param name="requery">requery database</param>
        /// <returns>date next run</returns>
        public DateTime? GetDateNextRun(bool requery)
        {
            return _model.GetDateNextRun(requery);
        }

        /// <summary>
        /// Get the date Last run
        /// </summary>
        /// <returns>date lest run</returns>
        public DateTime? GetDateLastRun()
        {
            return _model.GetDateLastRun();
        }

        /// <summary>
        /// Get Description
        /// </summary>
        /// <returns>Description</returns>
        public String GetDescription()
        {
            return _model.GetDescription();
        }

        /// <summary>
        /// Get Model
        /// </summary>
        /// <returns>Model</returns>
        public ViennaProcessor GetModel()
        {
            return _model;
        }

        /// <summary>
        /// Calculate Sleep ms
        /// </summary>
        /// <param name="now"></param>
        /// <returns>miliseconds</returns>
        private long CalculateSleep(long now)
        {
            String frequencyType = _model.GetFrequencyType();
            int frequency = _model.GetFrequency();
            if (frequency < 1)
            {
                frequency = 1;
            }

            long typeSec = 600;			//	10 minutes
            if (frequencyType == null)
            {
                typeSec = 300;			//	5 minutes
            }
            else if (X_R_RequestProcessor.FREQUENCYTYPE_Minute.Equals(frequencyType))
            {
                typeSec = 60;
            }
            else if (X_R_RequestProcessor.FREQUENCYTYPE_Hour.Equals(frequencyType))
            {
                typeSec = 3600;
            }
            else if (X_R_RequestProcessor.FREQUENCYTYPE_Day.Equals(frequencyType))
            {
                typeSec = 86400;
            }

            long sleep = typeSec * 1000 * frequency;		//	ms
            if (_model.GetAD_Schedule_ID() == 0)
            {
                return sleep;
            }

            //	Calculate Schedule
            MSchedule schedule = MSchedule.Get(GetCtx(), _model.GetAD_Schedule_ID());
            long next = schedule.GetNextRunMS(now);
            long delta = next - now;
            if (delta < 0)
            {
                log.Warning("Negative Delta=" + delta + " - set to " + sleep);
                delta = sleep;
            }
            return delta;
        }

        /// <summary>
        /// 	Is Sleeping
        /// </summary>
        /// <returns>sleeping</returns>
        public bool IsSleeping()
        {
            return _sleeping;
        }


        /// <summary>
        /// Get Seconds Alive
        /// </summary>
        /// <returns>seconds alive</returns>
        public int GetSecondsAlive()
        {
            if (_start == 0)
            {
                return 0;
            }
            long now = CommonFunctions.CurrentTimeMillis(DateTime.Now);
            long ms = (now - _start) / 1000;
            return (int)ms;
        }

        /// <summary>
        /// Get Start Time
        /// </summary>
        /// <returns>start time</returns>
        public DateTime? GetStartTime()
        {
            if (_start == 0)
            {
                return null;
            }
            return (DateTime?)CommonFunctions.CovertMilliToDate(_start);
        }

        /// <summary>
        /// Get Processor Logs
        /// </summary>
        /// <returns>logs</returns>
        public ViennaProcessorLog[] GetLogs()
        {
            return _model.GetLogs();
        }

        public void Start()
        {
            objThread.Start();
        }

        public void InitThread(int seq)
        {
            objThread = new Thread(new ThreadStart(Run));
            objThread.Name = this.GetType().Name + "_" + seq;
        }

        public void InitThread(int seq, string name)
        {
            if (name.Equals("InvokeIISService", StringComparison.OrdinalIgnoreCase))
            {
                objThread = new Thread(new ThreadStart(RunInvokeIISService));
                objThread.Name = name + "_" + seq;
            }
            else
            {
                objThread = new Thread(new ThreadStart(Run));
                objThread.Name = this.GetType().Name + "_" + seq;
            }
        }

        public Thread GetServerThread()
        {
            return objThread;
        }

        public void SetPriority(ThreadPriority priority)
        {
            objThread.Priority = priority;
        }

        bool Interrupted = false;   // to check if the thread is interrupted

        public bool IsInterrupted()
        {
            return (Interrupted == true);
        }

        public void Interrupt()
        {
            Interrupted = true;
            objThread.Interrupt();
        }


        public override bool Equals(object obj)
        {
            if ((obj == null) || !(obj is ViennaServer))
                return false;
            //	Different class
            if (!obj.GetType().FullName.Equals(GetType().FullName))
                return false;
            return GetServerID().Equals(((ViennaServer)obj).GetServerID());

        }

        /// <summary>
        /// new Function override to remove warning
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
