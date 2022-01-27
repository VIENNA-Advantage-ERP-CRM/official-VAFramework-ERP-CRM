/********************************************************
 * Module Name    : VAdvantage
 * Purpose        : Background thread initiated at login
 * Chronological Development
 * VIS0008   20-Dec-2021
 ******************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Utility;

namespace VAdvantage.Classes
{
    public class ThreadInstance : IDisposable
    {
        #region Varibales
        private static ThreadInstance _tInstance = null;
        // private static bool isThreadRunning = false;
        Thread t = null;
        PeriodicJob pj = null;
        #endregion Varibales

        /// <summary>
        /// Singleton object of ThreadInstance
        /// </summary>
        /// <returns>Object of ThreadInstance</returns>
        public static ThreadInstance Get()
        {
            if (_tInstance == null)
            {
                _tInstance = new ThreadInstance();
            }
            return _tInstance;
        }

        /// <summary>
        /// Constructor to initialize background thread
        /// </summary>
        private ThreadInstance()
        {
            InitThread();
        }

        /// <summary>
        /// Initialize periodic job and start background thread
        /// </summary>
        private void InitThread()
        {
            t = new Thread(new ThreadStart(DoWork));
            pj = PeriodicJob.Get();
            t.IsBackground = true;
            t.Name = "RunningThread";
            t.Start();
        }

        /// <summary>
        /// Start background thread if not alive or null
        /// </summary>
        public void Start()
        {
            if (t == null || !t.IsAlive)
            {
                InitThread();
            }
        }

        /// <summary>
        /// Thread wait for 2 minutes and notify all registered observers
        /// </summary>
        private void DoWork()
        {            
            pj.Notify();
            Thread.Sleep(1000 * 60 * 2);
            DoWork();
        }

        /// <summary>
        /// Dispose off thread
        /// </summary>
        public void Dispose()
        {
            if (t != null)
            {
                try
                {
                    t.Interrupt();
                    t = null;
                }
                catch
                {
                }
            }
        }
    }
}
