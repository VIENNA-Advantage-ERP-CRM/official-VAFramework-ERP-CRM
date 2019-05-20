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

namespace VAdvantage.Server
{
    public class RequestProcessor : ViennaServer
    {
        public RequestProcessor(MRequestProcessor model)
            : base(model, 60)	//	1 minute delay
        {
            m_model = model;
            m_client = MClient.Get(model.GetCtx(), model.GetAD_Client_ID());
        }	//	RequestProcessor

        /**	The Concrete Model			*/
        private MRequestProcessor m_model = null;
        /**	Last Summary				*/
        private StringBuilder m_summary = new StringBuilder();
        /** Client info					*/
        private MClient m_client = null;


        protected override void DoWork()
        {
            m_summary = new StringBuilder();
            //
            ProcessEMail();
            FindSalesRep();
            ProcessRequests();
            //processStatus();
            //processECR();
            //
            int no = m_model.DeleteLog();
            m_summary.Append("Logs deleted=").Append(no);
            //
            MRequestProcessorLog pLog = new MRequestProcessorLog(m_model, m_summary.ToString());
            pLog.SetReference("#" + _runCount.ToString()
                + " - " + TimeUtil.FormatElapsed(VAdvantage.Classes.CommonFunctions.CovertMilliToDate(_startWork)));
            pLog.Save();
        }

        private void ProcessRequests()
        {
            /**
             *  Due Requests (Scheduled -> Due)
             */
            String sql = "SELECT * FROM R_Request "
                + "WHERE DueType='" + X_R_Request.DUETYPE_Scheduled + "' AND Processed='N'"
                + " AND DateNextAction > SysDate"
                + " AND AD_Client_ID=" + m_model.GetAD_Client_ID();
            if (m_model.GetR_RequestType_ID() != 0)
                sql += " AND R_RequestType_ID=" + m_model.GetR_RequestType_ID();
            int count = 0;
            int countEMails = 0;
            try
            {
                DataSet ds = DB.ExecuteDataset(sql);
                foreach(DataRow dr in ds.Tables[0].Rows)
                {
                    MRequest request = new MRequest(GetCtx(), dr, null);
                    request.SetDueType();
                    if (request.IsDue())
                    {
                        if (request.GetRequestType().IsEMailWhenDue())
                        {
                            if (SendEmail(request, "RequestDue"))
                            {
                                request.SetDateLastAlert();
                                countEMails++;
                            }
                        }
                        request.Save();
                        count++;
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            m_summary.Append("New Due #").Append(count);
            if (countEMails > 0)
                m_summary.Append(" (").Append(countEMails).Append(" EMail)");
            m_summary.Append(" - ");

            /**
             *  Overdue Requests.
             *  Due Requests - are they overdue? (Send EMail)
             */
            sql = "SELECT * FROM R_Request r "
                + "WHERE r.DueType='" + X_R_Request.DUETYPE_Due + "' AND r.Processed='N'"
                + " AND AD_Client_ID=" + m_model.GetAD_Client_ID()
                + " AND EXISTS (SELECT * FROM R_RequestType rt "
                    + "WHERE r.R_RequestType_ID=rt.R_RequestType_ID"
                    + " AND addDays(r.DateNextAction,rt.DueDateTolerance) > SysDate)";
            //	+ " AND addDays(COALESCE(r.DateNextAction,Updated),rt.DueDateTolerance)>SysDate)";
            if (m_model.GetR_RequestType_ID() != 0)
                sql += " AND r.R_RequestType_ID=" + m_model.GetR_RequestType_ID();
            count = 0;
            countEMails = 0;
            try
            {

                DataSet ds = DB.ExecuteDataset(sql);
                foreach(DataRow dr in ds.Tables[0].Rows)
                {
                    MRequest request = new MRequest(GetCtx(), dr, null);
                    request.SetDueType();
                    if (request.IsOverdue())
                    {
                        if (request.GetRequestType().IsEMailWhenOverdue()
                            && !TimeUtil.IsSameDay(request.GetDateLastAlert(), null))
                        {
                            if (SendEmail(request, "RequestDue"))
                            {
                                request.SetDateLastAlert();
                                countEMails++;
                            }
                        }
                        request.Save();
                        count++;
                    }
                }

            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            m_summary.Append("New Overdue #").Append(count);
            if (countEMails > 0)
                m_summary.Append(" (").Append(countEMails).Append(" EMail)");
            m_summary.Append(" - ");

            /**
             *  Send (over)due alerts
             */
            if (m_model.GetOverdueAlertDays() > 0)
            {
                sql = "SELECT * FROM R_Request "
                    + "WHERE Processed='N'"
                    + " AND AD_Client_ID=" + m_model.GetAD_Client_ID()
                    //jz	+ " AND (DateNextAction+" + m_model.getOverdueAlertDays() + ") > SysDate"
                    + " AND addDays(DateNextAction," + m_model.GetOverdueAlertDays() + ") > SysDate "
                    + " AND (DateLastAlert IS NULL";
                if (m_model.GetRemindDays() > 0)
                    //jz	sql += " OR (DateLastAlert+" + m_model.getRemindDays()
                    sql += " OR addDays(DateLastAlert," + m_model.GetRemindDays()
                        + ") > SysDate ";
                sql += ")";
                if (m_model.GetR_RequestType_ID() != 0)
                    sql += " AND R_RequestType_ID=" + m_model.GetR_RequestType_ID();
                count = 0;
                countEMails = 0;
                try
                {

                    DataSet ds = DB.ExecuteDataset(sql);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        MRequest request = new MRequest(GetCtx(), dr, null);
                        request.SetDueType();
                        if (request.GetRequestType().IsEMailWhenOverdue()
                            && ((request.GetDateLastAlert() == null)
                                || !TimeUtil.IsSameDay(request.GetDateLastAlert(), null)))
                        {
                            if (SendEmail(request, "RequestAlert"))
                            {
                                request.SetDateLastAlert();
                                countEMails++;
                            }
                        }
                        request.Save();
                        count++;
                    }
                }
                catch (SqlException e)
                {
                    log.Log(Level.SEVERE, sql, e);
                }
                m_summary.Append("Alerts #").Append(count);
                if (countEMails > 0)
                    m_summary.Append(" (").Append(countEMails).Append(" EMail)");
                m_summary.Append(" - ");
            }	//	Overdue

            /**
             *  Escalate if Date Next Action + Overdue Assign Days > SysDate
             */
            if (m_model.GetOverdueAssignDays() > 0)
            {
                sql = "SELECT * FROM R_Request "
                    + "WHERE Processed='N'"
                    + " AND AD_Client_ID=" + m_model.GetAD_Client_ID()
                    + " AND IsEscalated='N'"
                    + " AND addDays(DateNextAction," + m_model.GetOverdueAssignDays()
                        + ") > SysDate";
                if (m_model.GetR_RequestType_ID() != 0)
                    sql += " AND R_RequestType_ID=" + m_model.GetR_RequestType_ID();
                count = 0;
                countEMails = 0;
                try
                {
                    DataSet ds = DB.ExecuteDataset(sql);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        MRequest request = new MRequest(GetCtx(), dr, null);
                        if (Escalate(request))
                            count++;
                    }
                }
                catch (SqlException e)
                {
                    log.Log(Level.SEVERE, sql, e);
                }
                m_summary.Append("Escalated #").Append(count).Append(" - ");
            }	//	Esacalate

            /**
             *  Send Inactivity alerts
             */
            if (m_model.GetInactivityAlertDays() > 0)
            {
                sql = "SELECT * FROM R_Request r "
                    + "WHERE r.Processed='N'"
                    + " AND r.AD_Client_ID=" + m_model.GetAD_Client_ID()
                    //	Nothing happening for x days
                    + " AND addDays(r.Updated," + m_model.GetInactivityAlertDays() + ") < SysDate "
                    + " AND (r.DateLastAlert IS NULL";
                if (m_model.GetRemindDays() > 0)
                    sql += " OR addDays(r.DateLastAlert," + m_model.GetRemindDays()
                        + ") < SysDate ";
                sql += ")";
                //	Next Date & Updated over due date tolerance
                sql += " AND EXISTS (SELECT * FROM R_RequestType rt "
                    + "WHERE r.R_RequestType_ID=rt.R_RequestType_ID"
                    + " AND addDays(COALESCE(r.DateNextAction,Updated),rt.DueDateTolerance) > SysDate)";
                if (m_model.GetR_RequestType_ID() != 0)
                    sql += " AND r.R_RequestType_ID=" + m_model.GetR_RequestType_ID();

                count = 0;
                countEMails = 0;
                try
                {
                    DataSet ds = DB.ExecuteDataset(sql);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        MRequest request = new MRequest(GetCtx(), dr, null);
                        request.SetDueType();
                        //	only once per day
                        if (!TimeUtil.IsSameDay(request.GetDateLastAlert(), null))
                        {
                            if (SendEmail(request, "RequestInactive"))
                            {
                                request.SetDateLastAlert();
                                countEMails++;
                            }
                            request.Save();
                            count++;
                        }
                    }
                }
                catch (SqlException e)
                {
                    log.Log(Level.SEVERE, sql, e);
                }
                m_summary.Append("Inactivity #").Append(count);
                if (countEMails > 0)
                    m_summary.Append(" (").Append(countEMails).Append(" EMail)");
                m_summary.Append(" - ");
            }	//	Inactivity

        }	//  processRequests

        private bool SendEmail(MRequest request, String AD_Message)
        {
            //  Alert: Request {0} overdue
            String subject = Msg.GetMsg(m_client.GetAD_Language(), AD_Message,
                new String[] { request.GetDocumentNo() });
            return m_client.SendEMail(request.GetSalesRep_ID(), subject, request.GetSummary(), request.CreatePDF());
        }   //  sendAlert


        private bool Escalate(MRequest request)
        {
            //  Get Supervisor
            MUser supervisor = request.GetSalesRep();	//	self
            int supervisor_ID = request.GetSalesRep().GetSupervisor_ID();
            if ((supervisor_ID == 0) && (m_model.GetSupervisor_ID() != 0))
                supervisor_ID = m_model.GetSupervisor_ID();
            if ((supervisor_ID != 0) && (supervisor_ID != request.GetAD_User_ID()))
                supervisor = MUser.Get(GetCtx(), supervisor_ID);

            //  Escalated: Request {0} to {1}
            String subject = Msg.GetMsg(m_client.GetAD_Language(), "RequestEscalate",
                new String[] { request.GetDocumentNo(), supervisor.GetName() });
            String to = request.GetSalesRep().GetEMail();
            if ((to == null) || (to.Length == 0))
                log.Warning("SalesRep has no EMail - " + request.GetSalesRep());
            else
                m_client.SendEMail(request.GetSalesRep_ID(), subject, request.GetSummary(), request.CreatePDF());

            //	Not the same - send mail to supervisor
            if (request.GetSalesRep_ID() != supervisor.GetAD_User_ID())
            {
                to = supervisor.GetEMail();
                if ((to == null) || (to.Length == 0))
                    log.Warning("Supervisor has no EMail - " + supervisor);
                else
                    m_client.SendEMail(supervisor.GetAD_User_ID(), subject, request.GetSummary(), request.CreatePDF());
            }

            //  ----------------
            request.SetDueType();
            request.SetIsEscalated(true);
            request.SetResult(subject);
            return request.Save();
        }   //  escalate


        private void ProcessEMail()
        {
            //	m_summary.Append("Mail #").Append(count)
            //		.Append(" - ");
        }   //  processEMail

        private void FindSalesRep()
        {
            int changed = 0;
            int notFound = 0;
            Ctx ctx = new Ctx();
            //
            String sql = "SELECT * FROM R_Request "
                + "WHERE AD_Client_ID=@AD_Client_ID"
                + " AND SalesRep_ID=0 AND Processed='N'";
            if (m_model.GetR_RequestType_ID() != 0)
                sql += " AND R_RequestType_ID=@R_RequestType_ID";

            try
            {
                SqlParameter[] param = null;

                if (m_model.GetR_RequestType_ID() != 0)
                    param = new SqlParameter[2];
                else
                    param = new SqlParameter[1];

                param[0] = new SqlParameter("@AD_Client_ID", m_model.GetAD_Client_ID());
                if (m_model.GetR_RequestType_ID() != 0)
                    param[1] = new SqlParameter("@AD_Client_ID", m_model.GetR_RequestType_ID());

                DataSet ds = DB.ExecuteDataset(sql, param);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    MRequest request = new MRequest(ctx, dr, null);
                    if (request.GetSalesRep_ID() != 0)
                        continue;
                    int SalesRep_ID = FindSalesRep(request);
                    if (SalesRep_ID != 0)
                    {
                        request.SetSalesRep_ID(SalesRep_ID);
                        request.Save();
                        changed++;
                    }
                    else
                        notFound++;
                }
            }
            catch (SqlException ex)
            {
                log.Log(Level.SEVERE, sql, ex);
            }
            //
            if ((changed == 0) && (notFound == 0))
                m_summary.Append("No unallocated Requests");
            else
                m_summary.Append("Allocated SalesRep=").Append(changed);
            if (notFound > 0)
                m_summary.Append(",Not=").Append(notFound);
            m_summary.Append(" - ");
        }	//	findSalesRep

        /**
         *  Find SalesRep/User based on Request Type and Question.
         *  @param request request
         *  @return SalesRep_ID user
         */
        private int FindSalesRep(MRequest request)
        {
            String QText = request.GetSummary();
            if (QText == null)
                QText = "";
            else
                QText = QText.ToUpper();
            //
            MRequestProcessorRoute[] routes = m_model.GetRoutes(false);
            for (int i = 0; i < routes.Length; i++)
            {
                MRequestProcessorRoute route = routes[i];

                //	Match first on Request Type
                if ((request.GetR_RequestType_ID() == route.GetR_RequestType_ID())
                    && (route.GetR_RequestType_ID() != 0))
                    return route.GetAD_User_ID();

                //	Match on element of keyword
                String keyword = route.GetKeyword();
                if (keyword != null)
                {
                    StringTokenizer st = new StringTokenizer(keyword.ToUpper(), " ,;\t\n\r\f");
                    while (st.HasMoreElements())
                    {
                        if (QText.IndexOf(st.NextToken()) != -1)
                            return route.GetAD_User_ID();
                    }
                }
            }	//	for all routes

            return m_model.GetSupervisor_ID();
        }   //  findSalesRep

        /**
         * 	Get Server Info
         *	@return info
         */
        public override String GetServerInfo()
        {
            return "#" + _runCount + " - Last=" + m_summary.ToString();
        }	//	getServerInfo
    }
}
