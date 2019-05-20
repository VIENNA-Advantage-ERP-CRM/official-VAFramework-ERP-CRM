using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.Utility;
using System.Data.SqlClient;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using VAdvantage.Logging;
using VAdvantage.WF;
using VAdvantage.Process;
using System.IO;

namespace VAdvantage.Server
{
    public class WorkflowProcessor : ViennaServer
    {
        public WorkflowProcessor(MWorkflowProcessor model)
            : base(model, 120)		//	2 minute dalay
        {
            m_model = model;
            m_client = MClient.Get(model.GetCtx(), model.GetAD_Client_ID());
        }	//	WorkflowProcessor

        /**	The Concrete Model			*/
        private MWorkflowProcessor m_model = null;
        /**	Last Summary				*/
        private StringBuilder m_summary = new StringBuilder();
        /** Client onfo					*/
        private MClient m_client = null;



        protected override void DoWork()
        {
            m_summary = new StringBuilder();
            //
            Wakeup();
            DynamicPriority();
            SendAlerts();
            //
            int no = m_model.DeleteLog();
            m_summary.Append("Logs deleted=").Append(no);
            //
            MWorkflowProcessorLog pLog = new MWorkflowProcessorLog(m_model, m_summary.ToString());
            pLog.SetReference("#" + _runCount.ToString()
                + " - " + TimeUtil.FormatElapsed(CommonFunctions.CovertMilliToDate(_startWork)));
            pLog.Save();
        }

        private void Wakeup()
        {
            String sql = "SELECT * "
                + "FROM AD_WF_Activity a "
                + "WHERE Processed='N' AND WFState='OS'"	//	suspended
                + " AND EndWaitTime > SysDate"
                + " AND AD_Client_ID=@AD_Client_ID"
                + " AND EXISTS (SELECT * FROM AD_Workflow wf "
                    + " INNER JOIN AD_WF_Node wfn ON (wf.AD_Workflow_ID=wfn.AD_Workflow_ID) "
                    + "WHERE a.AD_WF_Node_ID=wfn.AD_WF_Node_ID"
                    + " AND wfn.Action='Z'"		//	sleeping
                    + " AND wf.AD_WorkflowProcessor_ID IS NULL OR wf.AD_WorkflowProcessor_ID=@AD_WorkflowProcessor_ID)";

            int count = 0;
            try
            {
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@AD_Client_ID", m_model.GetAD_Client_ID());
                param[1] = new SqlParameter("@AD_WorkflowProcessor_ID", m_model.GetAD_WorkflowProcessor_ID());
                DataSet ds = DB.ExecuteDataset(sql, param);
                foreach(DataRow dr in ds.Tables[0].Rows)
                {
                    MWFActivity activity = new MWFActivity(GetCtx(), dr, null);
                    activity.SetWFState(StateEngine.STATE_COMPLETED);
                    // saves and calls MWFProcess.checkActivities();
                    count++;
                }

            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "wakeup", e);
            }
            m_summary.Append("Wakeup #").Append(count).Append(" - ");
        }	//	wakeup


        private void DynamicPriority()
        {
            //	suspened activities with dynamic priority node
            String sql = "SELECT * "
                + "FROM AD_WF_Activity a "
                + "WHERE Processed='N' AND WFState='OS'"	//	suspended
                + " AND EXISTS (SELECT * FROM AD_Workflow wf"
                    + " INNER JOIN AD_WF_Node wfn ON (wf.AD_Workflow_ID=wfn.AD_Workflow_ID) "
                    + "WHERE a.AD_WF_Node_ID=wfn.AD_WF_Node_ID AND wf.AD_WorkflowProcessor_ID=@AD_WorkflowProcessor_ID"
                    + " AND wfn.DynPriorityUnit IS NOT NULL AND wfn.DynPriorityChange IS NOT NULL)";
            int count = 0;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@AD_WorkflowProcessor_ID", m_model.GetAD_WorkflowProcessor_ID());
                DataSet ds = DB.ExecuteDataset(sql, param);

                foreach(DataRow dr in ds.Tables[0].Rows)
                {
                    MWFActivity activity = new MWFActivity(GetCtx(), dr, null);
                    if (activity.GetDynPriorityStart() == 0)
                        activity.SetDynPriorityStart(activity.GetPriority());
                    long ms = CommonFunctions.CurrentTimeMillis() - CommonFunctions.CurrentTimeMillis(activity.GetCreated());
                    MWFNode node = activity.GetNode();
                    int prioDiff = node.CalculateDynamicPriority((int)(ms / 1000));
                    activity.SetPriority(activity.GetDynPriorityStart() + prioDiff);
                    activity.Save();
                    count++;
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            m_summary.Append("DynPriority #").Append(count).Append(" - ");

        }	//	setPriority


        private void SendAlerts()
        {
            int count = 0;
            int countEMails = 0;
            String sql = "";
            //	Alert over Priority
            if (m_model.GetAlertOverPriority() > 0)
            {
                sql = "SELECT * "
                    + "FROM AD_WF_Activity a "
                    + "WHERE Processed='N' AND WFState='OS'"	//	suspended
                    + " AND Priority >= @Priority"				//	##1
                    + " AND (DateLastAlert IS NULL";
                if (m_model.GetRemindDays() > 0)
                    sql += " OR (DateLastAlert+" + m_model.GetRemindDays()
                        + ") < SysDate";
                sql += ") AND EXISTS (SELECT * FROM AD_Workflow wf "
                        + " INNER JOIN AD_WF_Node wfn ON (wf.AD_Workflow_ID=wfn.AD_Workflow_ID) "
                        + "WHERE a.AD_WF_Node_ID=wfn.AD_WF_Node_ID"
                        + " AND wf.AD_WorkflowProcessor_ID IS NULL OR wf.AD_WorkflowProcessor_ID=@AD_WorkflowProcessor_ID)";



                try
                {
                    SqlParameter[] param = new SqlParameter[2];
                    param[0] = new SqlParameter("@Priority", m_model.GetAlertOverPriority());
                    param[1] = new SqlParameter("@AD_WorkflowProcessor_ID", m_model.GetAD_WorkflowProcessor_ID());

                    DataSet ds = DB.ExecuteDataset(sql, param);
                    foreach(DataRow dr in  ds.Tables[0].Rows)
                    {
                        MWFActivity activity = new MWFActivity(GetCtx(), dr, null);
                        bool escalate = activity.GetDateLastAlert() != null;
                        countEMails += SendEmail(activity, "ActivityOverPriority", escalate, true);
                        activity.SetDateLastAlert(CommonFunctions.CovertMilliToDate(CommonFunctions.CurrentTimeMillis()));
                        activity.Save();
                        count++;
                    }
                }
                catch (SqlException e)
                {
                    log.Log(Level.SEVERE, "(Priority) - " + sql, e);
                }
                m_summary.Append("OverPriority #").Append(count);
                if (countEMails > 0)
                    m_summary.Append(" (").Append(countEMails).Append(" EMail)");
                m_summary.Append(" - ");
            }	//	Alert over Priority

            /**
             * 	Over End Wait
             */
            sql = "SELECT * "
                + "FROM AD_WF_Activity a "
                + "WHERE Processed='N' AND WFState='OS'"	//	suspended
                + " AND EndWaitTime > SysDate"
                + " AND (DateLastAlert IS NULL";
            if (m_model.GetRemindDays() > 0)
                sql += " OR (DateLastAlert+" + m_model.GetRemindDays()
                    + ") < SysDate";
            sql += ") AND EXISTS (SELECT * FROM AD_Workflow wf "
                    + " INNER JOIN AD_WF_Node wfn ON (wf.AD_Workflow_ID=wfn.AD_Workflow_ID) "
                    + "WHERE a.AD_WF_Node_ID=wfn.AD_WF_Node_ID"
                    + " AND wfn.Action<>'Z'"	//	not sleeping
                    + " AND wf.AD_WorkflowProcessor_ID IS NULL OR wf.AD_WorkflowProcessor_ID=@AD_WorkflowProcessor_ID)";

            count = 0;
            countEMails = 0;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@AD_WorkflowProcessor_ID", m_model.GetAD_WorkflowProcessor_ID());
                DataSet ds = DB.ExecuteDataset(sql, param);
                foreach(DataRow dr in ds.Tables[0].Rows)
                {
                    MWFActivity activity = new MWFActivity(GetCtx(), dr, null);
                    bool escalate = activity.GetDateLastAlert() != null;
                    countEMails += SendEmail(activity, "ActivityEndWaitTime", escalate, false);
                    activity.SetDateLastAlert(CommonFunctions.CovertMilliToDate(CommonFunctions.CurrentTimeMillis()));
                    activity.Save();
                    count++;
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "(EndWaitTime) - " + sql, e);
            }
            m_summary.Append("EndWaitTime #").Append(count);
            if (countEMails > 0)
                m_summary.Append(" (").Append(countEMails).Append(" EMail)");
            m_summary.Append(" - ");

            /**
             *  Send inactivity alerts
             */
            if (m_model.GetInactivityAlertDays() > 0)
            {
                sql = "SELECT * "
                    + "FROM AD_WF_Activity a "
                    + "WHERE Processed='N' AND WFState='OS'"	//	suspended
                    + " AND (Updated+" + m_model.GetInactivityAlertDays() + ") < SysDate"
                    + " AND (DateLastAlert IS NULL";
                if (m_model.GetRemindDays() > 0)
                    sql += " OR (DateLastAlert+" + m_model.GetRemindDays()
                        + ") < SysDate";
                sql += ") AND EXISTS (SELECT * FROM AD_Workflow wf "
                        + " INNER JOIN AD_WF_Node wfn ON (wf.AD_Workflow_ID=wfn.AD_Workflow_ID) "
                        + "WHERE a.AD_WF_Node_ID=wfn.AD_WF_Node_ID"
                        + " AND wf.AD_WorkflowProcessor_ID IS NULL OR wf.AD_WorkflowProcessor_ID=@AD_WorkflowProcessor_ID)";

                count = 0;
                countEMails = 0;
                try
                {
                    SqlParameter[] param = new SqlParameter[1];
                    param[0] = new SqlParameter("@AD_WorkflowProcessor_ID", m_model.GetAD_WorkflowProcessor_ID());
                    DataSet ds = DB.ExecuteDataset(sql, param);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {

                        MWFActivity activity = new MWFActivity(GetCtx(), dr, null);
                        bool escalate = activity.GetDateLastAlert() != null;
                        countEMails += SendEmail(activity, "ActivityInactivity",
                            escalate, false);
                        activity.SetDateLastAlert(CommonFunctions.CovertMilliToDate(CommonFunctions.CurrentTimeMillis()));
                        activity.Save();
                        count++;
                    }

                }
                catch (SqlException e)
                {
                    log.Log(Level.SEVERE, "(Inactivity): " + sql, e);
                }
                m_summary.Append("Inactivity #").Append(count);
                if (countEMails > 0)
                    m_summary.Append(" (").Append(countEMails).Append(" EMail)");
                m_summary.Append(" - ");
            }	//	Inactivity

        }	//	sendAlerts


        private int SendEmail(MWFActivity activity, String AD_Message, bool toProcess, bool toSupervisor)
        {
            if (m_client == null || m_client.GetAD_Client_ID() != activity.GetAD_Client_ID())
                m_client = MClient.Get(GetCtx(), activity.GetAD_Client_ID());

            MWFProcess process = new MWFProcess(GetCtx(), activity.GetAD_WF_Process_ID(), null);

            String subjectVar = activity.GetNode().GetName();
            String message = activity.GetTextMsg();
            if (message == null || message.Length == 0)
                message = process.GetTextMsg();
            FileInfo pdf = null;
            PO po = activity.GetPO();
            if (po is DocAction)
            {
                message = ((DocAction)po).GetDocumentInfo() + "\n" + message;
                pdf = ((DocAction)po).CreatePDF();
            }

            //  Inactivity Alert: Workflow Activity {0}
            String subject = Msg.GetMsg(m_client.GetAD_Language(), AD_Message,
                new Object[] { subjectVar });

            //	Prevent duplicates
            List<int> list = new List<int>();
            int counter = 0;

            //	To Activity Owner
            if (m_client.SendEMail(activity.GetAD_User_ID(), subject, message, pdf))
                counter++;
            list.Add(activity.GetAD_User_ID());

            //	To Process Owner
            if (toProcess
                && process.GetAD_User_ID() != activity.GetAD_User_ID())
            {
                if (m_client.SendEMail(process.GetAD_User_ID(), subject, message, pdf))
                    counter++;
                list.Add(process.GetAD_User_ID());
            }

            //	To Activity Responsible
            MWFResponsible responsible = MWFResponsible.Get(GetCtx(), activity.GetAD_WF_Responsible_ID());
            counter += sendAlertToResponsible(responsible, list, process, subject, message, pdf);

            //	To Process Responsible
            if (toProcess
                && process.GetAD_WF_Responsible_ID() != activity.GetAD_WF_Responsible_ID())
            {
                responsible = MWFResponsible.Get(GetCtx(), process.GetAD_WF_Responsible_ID());
                counter += sendAlertToResponsible(responsible, list, process, subject, message, pdf);
            }

            //	Processor SuperVisor
            if (toSupervisor && m_model.GetSupervisor_ID() != 0
                && !list.Contains(m_model.GetSupervisor_ID()))
            {
                if (m_client.SendEMail(m_model.GetSupervisor_ID(), subject, message, pdf))
                    counter++;
                list.Add(m_model.GetSupervisor_ID());
            }

            return counter;
        }   //  sendAlert


        private int sendAlertToResponsible(MWFResponsible responsible, List<int> list, MWFProcess process, String subject, String message, FileInfo pdf)
        {
            int counter = 0;
            if (responsible.IsInvoker())
            {
                ;
            }
            //	Human
            else if (X_AD_WF_Responsible.RESPONSIBLETYPE_Human.Equals(responsible.GetResponsibleType())
                && responsible.GetAD_User_ID() != 0
                && !list.Contains(responsible.GetAD_User_ID()))
            {
                if (m_client.SendEMail(responsible.GetAD_User_ID(), subject, message, pdf))
                    counter++;
                list.Add(responsible.GetAD_User_ID());
            }
            //	Org of the Document
            else if (X_AD_WF_Responsible.RESPONSIBLETYPE_Organization.Equals(responsible.GetResponsibleType()))
            {
                PO document = process.GetPO();
                if (document != null)
                {
                    MOrgInfo org = MOrgInfo.Get(GetCtx(), document.GetAD_Org_ID(), null);
                    if (org.GetSupervisor_ID() != 0
                        && !list.Contains(org.GetSupervisor_ID()))
                    {
                        if (m_client.SendEMail(org.GetSupervisor_ID(), subject, message, pdf))
                            counter++;
                        list.Add(org.GetSupervisor_ID());
                    }
                }
            }
            //	Role
            else if (X_AD_WF_Responsible.RESPONSIBLETYPE_Role.Equals(responsible.GetResponsibleType())
                && responsible.GetAD_Role_ID() != 0)
            {
                MUserRoles[] userRoles = MUserRoles.GetOfRole(GetCtx(), responsible.GetAD_Role_ID());
                for (int i = 0; i < userRoles.Length; i++)
                {
                    MUserRoles roles = userRoles[i];
                    if (!roles.IsActive())
                        continue;
                    int AD_User_ID = roles.GetAD_User_ID();
                    if (!list.Contains(AD_User_ID))
                    {
                        if (m_client.SendEMail(AD_User_ID, subject, message, pdf))
                            counter++;
                        list.Add(AD_User_ID);
                    }
                }
            }
            return counter;
        }	//	sendAlertToResponsible

        public override string GetServerInfo()
        {
            return "#" + _runCount + " - Last=" + m_summary.ToString();
        }
    }
}
