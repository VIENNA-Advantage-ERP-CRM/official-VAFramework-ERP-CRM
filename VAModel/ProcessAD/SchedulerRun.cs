/********************************************************
 * Module Name    : Scheduler
 * Purpose        : Schedule the Events
 * Author         : Jagmohan Bhatt
 * Date           : 10-Nov-2009
 ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    /// <summary>
    /// 
    /// </summary>
    public class SchedulerRun : ProcessEngine.SvrProcess
    {
        /** Scheduler		*/
        private int p_AD_Scheduler_ID = 0;

        /// <summary>
        /// Prepare
        /// </summary>
        protected override void Prepare()
        {
            p_AD_Scheduler_ID = GetRecord_ID();
        }	//	prepare


        /// <summary>
        /// Do It
        /// </summary>
        /// <returns></returns>
        protected override string DoIt()
        {
            log.Info("AD_Scheduler_ID=" + p_AD_Scheduler_ID);
            MScheduler scheduler = new MScheduler(GetCtx(), p_AD_Scheduler_ID, Get_TrxName());
            return scheduler.Execute(Get_Trx());            
        }
    }
}
