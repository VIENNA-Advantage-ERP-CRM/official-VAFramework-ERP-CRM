using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using System.Data;
using VAdvantage.Classes;

namespace VAdvantage.Server
{
    public class AlertProcessor : ViennaServer
    {
        /**
         * 	Alert Processor
         *	@param model model
         */
        public AlertProcessor(MAlertProcessor model)
            : base(model, 180)		//	3 minute delay 
        {
            m_model = model;
            m_client = MClient.Get(model.GetCtx(), model.GetAD_Client_ID());
        }	//	AlertProcessor

        /**	The Concrete Model			*/
        private MAlertProcessor m_model = null;
        /**	Last Summary				*/
        private StringBuilder m_summary = new StringBuilder();
        /**	Last Error Msg				*/
        private StringBuilder m_errors = new StringBuilder();
        /** Client info					*/
        private MClient m_client = null;
        /** Mail/Notice Recipients			*/
        private List<int> m_recipients = new List<int>();

        /**
         * 	Work
         */
        protected override void DoWork()
        {
            m_summary = new StringBuilder();
            m_errors = new StringBuilder();
            //
            int count = 0;
            int countError = 0;
            MAlert[] alerts = m_model.GetAlerts(true);
            for (int i = 0; i < alerts.Length; i++)
            {
                if (!ProcessAlert(alerts[i]))
                    countError++;
                count++;
            }
            //
            String summary = "Alerts=" + count;
            if (countError > 0)
                summary += ", Not processed=" + countError;
            summary += " - ";
            m_summary.Insert(0, summary);
            //
            int no = m_model.DeleteLog();
            m_summary.Append("Logs deleted=").Append(no);
            //
            MAlertProcessorLog pLog = new MAlertProcessorLog(m_model, m_summary.ToString());
            pLog.SetReference("#" + _runCount.ToString()
                + " - " + TimeUtil.FormatElapsed(CommonFunctions.CovertMilliToDate(_startWork)));
            pLog.SetTextMsg(m_errors.ToString());
            pLog.Save();
        }	//	doWork

        /**
         * 	Process Alert
         *	@param alert alert
         *	@return true if processed
         */
        private bool ProcessAlert(MAlert alert)
        {
            if (!alert.IsValid())
            {
                log.Info("Invalid: " + alert);
                return false;
            }
            log.Info("" + alert);
            m_recipients.Clear();

            StringBuilder message = new StringBuilder(alert.GetAlertMessage())
                .Append(Env.NL);
            //	Context
            Ctx ctx = alert.GetCtx();
            ctx.SetAD_Client_ID(alert.GetAD_Client_ID());
            ctx.SetAD_Org_ID(alert.GetAD_Org_ID());
            //
            bool valid = true;
            bool processed = false;
            MAlertRule[] rules = alert.GetRules(false);
            for (int i = 0; i < rules.Length; i++)
            {
                if (i > 0)
                    message.Append(Env.NL).Append("================================").Append(Env.NL);
                //Trx trx = null;		//	assume r/o

                MAlertRule rule = rules[i];
                if (!rule.IsValid())
                {
                    log.Config("Invalid: " + rule);
                    continue;
                }
                log.Fine("" + rule);

                //	Pre
                String sql = rule.GetPreProcessing();
                if (sql != null && sql.Length > 0)
                {
                    int no = DB.ExecuteQuery(sql);
                    if (no == -1)
                    {
                        ValueNamePair error = VLogger.RetrieveError();
                        rule.SetErrorMsg("Pre=" + error.GetName());
                        m_errors.Append("Pre=" + error.GetName());
                        rule.SetIsValid(false);
                        rule.Save();
                        valid = false;
                        break;
                    }
                }	//	Pre

                //	The processing
                ctx.SetAD_Role_ID(0);
                ctx.SetAD_User_ID(0);
                sql = rule.GetSql();
                if (alert.IsEnforceRoleSecurity()
                    || alert.IsEnforceClientSecurity())
                {
                    int AD_Role_ID = alert.GetFirstAD_Role_ID();
                    if (AD_Role_ID == -1)
                        AD_Role_ID = alert.GetFirstUserAD_Role_ID();
                    if (AD_Role_ID != -1)
                    {
                        String tableName = rule.GetTableName();
                        bool fullyQualified = MRole.SQL_FULLYQUALIFIED;
                        if (Util.IsEmpty(tableName))
                            fullyQualified = MRole.SQL_NOTQUALIFIED;
                        MRole role = MRole.Get(ctx, AD_Role_ID, 0, false);
                        sql = role.AddAccessSQL(sql, tableName,
                            fullyQualified, MRole.SQL_RO);
                        ctx.SetAD_Role_ID(AD_Role_ID);
                    }
                    if (alert.GetFirstAD_User_ID() != -1)
                        ctx.SetAD_User_ID(alert.GetFirstAD_User_ID());
                }

                try
                {
                    String text = ListSqlSelect(sql);
                    if (text != null && text.Length > 0)
                    {
                        message.Append(text);
                        processed = true;
                        int index = text.IndexOf(":");
                        if (index > 0 && index < 5)
                            m_summary.Append(text.Substring(0, index));
                    }
                }
                catch (Exception e)
                {
                    rule.SetErrorMsg("Select=" + e.Message);
                    m_errors.Append("Select=" + e.Message);
                    rule.SetIsValid(false);
                    rule.Save();
                    valid = false;
                    break;
                }

                //	Post
                sql = rule.GetPostProcessing();
                if (sql != null && sql.Length > 0)
                {
                    int no = DB.ExecuteQuery(sql);
                    if (no == -1)
                    {
                        ValueNamePair error = VLogger.RetrieveError();
                        rule.SetErrorMsg("Post=" + error.GetName());
                        m_errors.Append("Post=" + error.GetName());
                        rule.SetIsValid(false);
                        rule.Save();
                        valid = false;
                        break;
                    }
                }	//	Post
            }	//	 for all rules

            //	Update header if error
            if (!valid)
            {
                alert.SetIsValid(false);
                alert.Save();
                return false;
            }

            //	Nothing to report
            if (!processed)
            {
                m_summary.Append(alert.GetName()).Append("=No Result - ");
                return true;
            }

            //	Send Message
            int countRecipient = 0;
            MAlertRecipient[] recipients = alert.GetRecipients(false);
            for (int i = 0; i < recipients.Length; i++)
            {
                MAlertRecipient recipient = recipients[i];
                if (recipient.GetAD_User_ID() >= 0)		//	System == 0
                    if (SendInfo(recipient.GetAD_User_ID(), alert, message.ToString()))
                        countRecipient++;
                if (recipient.GetAD_Role_ID() >= 0)		//	SystemAdministrator == 0
                {
                    MUserRoles[] urs = MUserRoles.GetOfRole(GetCtx(), recipient.GetAD_Role_ID());
                    for (int j = 0; j < urs.Length; j++)
                    {
                        MUserRoles ur = urs[j];
                        if (!ur.IsActive())
                            continue;
                        if (SendInfo(ur.GetAD_User_ID(), alert, message.ToString()))
                            countRecipient++;
                    }
                }
            }

            m_summary.Append(alert.GetName()).Append(" (Recipients=").Append(countRecipient).Append(") - ");
            return valid;
        }	//	processAlert

        /**
         * 	Send Email / Notice
         * 	@param AD_User_ID user
         *	@param alert alert
         *	@param message message text
         *	@return true if sent (or previously sent)
         */
        private bool SendInfo(int AD_User_ID, MAlert alert, String message)
        {
            if (m_recipients.Contains(AD_User_ID))
                return false;
            m_recipients.Add(AD_User_ID);
            //
            bool success = false;
            MUser to = MUser.Get(alert.GetCtx(), AD_User_ID);
            String NotificationType = to.GetNotificationType();
            if (Util.IsEmpty(NotificationType))
                NotificationType = X_AD_User.NOTIFICATIONTYPE_EMail;
            //	Send Mail
            if (X_AD_User.NOTIFICATIONTYPE_EMail.Equals(NotificationType)
                || X_AD_User.NOTIFICATIONTYPE_EMailPlusNotice.Equals(NotificationType))
            {
                success = m_client.SendEMail(AD_User_ID, alert.GetAlertSubject(), message, null);
                if (!success)
                {
                    log.Warning("EMail failed: " + to);
                    NotificationType = X_AD_User.NOTIFICATIONTYPE_Notice;
                }
            }
            //	Send Note
            if (X_AD_User.NOTIFICATIONTYPE_Notice.Equals(NotificationType)
                || X_AD_User.NOTIFICATIONTYPE_EMailPlusNotice.Equals(NotificationType))
            {
                int AD_Message_ID = 1040;	//	AlertNotice
                MNote note = new MNote(alert.GetCtx(), AD_Message_ID, AD_User_ID,
                    X_AD_Alert.Table_ID, alert.GetAD_Alert_ID(),
                    alert.GetAlertSubject(), message, null);
                success = note.Save();
            }
            return success;
        }	//	sendInfo

        /**
         * 	List Sql Select
         *	@param sql sql select
         *	@param trx transaction
         *	@return list of rows & values
         *	@throws Exception
         */
        private String ListSqlSelect(String sql)
        {
            StringBuilder result = new StringBuilder();
            Exception error = null;
            int count = 0;

            IDataReader dr = null;

            try
            {
                dr = DB.ExecuteReader(sql);
                while (dr.Read())
                {
                    result.Append("------------------").Append(Env.NL);
                    for (int col = 0; col <= dr.FieldCount - 1; col++)
                    {
                        result.Append(dr.GetName(col)).Append(" = ");
                        result.Append(dr[col].ToString());
                        result.Append(Env.NL);
                    }	//	for all columns
                    count++;
                }
                dr.Close();
                if (result.Length == 0)
                    log.Fine("No rows selected");
            }
            catch (Exception e)
            {
                if (dr != null)
                    dr.Close();

                if (DB.IsOracle() || sql.IndexOf(" DBA_Free_Space") == -1)
                {
                    log.Log(Level.SEVERE, sql, e);
                    error = e;
                }
                else
                {
                    log.Log(Level.WARNING, sql, e);
                }
            }

            //	Error occured
            if (error != null)
                throw new Exception("(" + sql + ") " + Env.NL
                    + error.Message);

            if (count > 0)
                result.Insert(0, "#" + count + ": ");
            return result.ToString();
        }	//	listSqlSelect



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
