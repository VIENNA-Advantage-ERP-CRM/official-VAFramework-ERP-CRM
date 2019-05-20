using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VAdvantage.Logging;

namespace VAdvantage.Server
{
    /// <summary>
    /// The Scheduler Process
    /// </summary>
    public class Scheduler : ViennaServer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public Scheduler(MScheduler model)
            : base(model, 240)		//	nap
        {
            m_model = model;
            //	m_client = MClient.get(model.getCtx(), model.getAD_Client_ID());
        }	//	Scheduler

        /**	The Concrete Model			*/
        private MScheduler m_model = null;
        /**	Last Summary				*/
        private StringBuilder m_summary = new StringBuilder();
        /** Transaction					*/
        private Trx m_trx = null;

        protected override void DoWork()
        {
            m_summary = new StringBuilder(m_model.ToString())
            .Append(" - ");
            MProcess process = m_model.GetProcess();
            //
            try
            {
                //	Explicitly set Environment
                Ctx ctx = m_model.GetCtx();
                ctx.SetAD_Client_ID(m_model.GetAD_Client_ID());
                ctx.SetContext("AD_Client_ID", m_model.GetAD_Client_ID());
                ctx.SetAD_Org_ID(m_model.GetAD_Org_ID());
                ctx.SetContext("AD_Org_ID", m_model.GetAD_Org_ID());
                ctx.SetAD_User_ID(m_model.GetUpdatedBy());
                ctx.SetContext("AD_User_ID", m_model.GetUpdatedBy());
                ctx.SetContext("#SalesRep_ID", m_model.GetUpdatedBy());
                //
                m_trx = Trx.Get("Scheduler");
                String result = m_model.Execute(m_trx);
                m_summary.Append(result);
                m_trx.Commit();
            }
            catch (Exception e)
            {
                if (m_trx != null)
                    m_trx.Rollback();
                log.Log(Level.WARNING, process.ToString(), e);
                m_summary.Append(e.ToString());
            }
            if (m_trx != null)
                m_trx.Close();
            //
            int no = m_model.DeleteLog();
            m_summary.Append("Logs deleted=").Append(no);
            //
            MSchedulerLog pLog = new MSchedulerLog(m_model, m_summary.ToString());
            pLog.SetReference("#" + (_runCount.ToString())
                + " - " + TimeUtil.FormatElapsed(VAdvantage.Classes.CommonFunctions.CovertMilliToDate(_startWork)));
            pLog.Save();
        }


        public override String GetServerInfo()
        {
            return "#" + _runCount + " - Last=" + m_summary.ToString();
        }	//	getServerInfo


      
    }
}
