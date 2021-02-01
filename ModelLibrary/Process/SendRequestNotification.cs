using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.ProcessEngine;
using VAdvantage.Model;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using System.Threading;
using System.IO;
using VAdvantage.Classes;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Process
{
    class SendRequestNotification : SvrProcess
    {
        private MRequest _req = null;
        private MRequestAction _reqAction = null;
        StringBuilder message = null;
        public const String SEPARATOR =
          "\n---------.----------.----------.----------.----------.----------\n";
        public static String Table_Name = "VAR_Request";
        private int _success = 0;
        private int _failure = 0;
        private int _notices = 0;
        private StringBuilder _emailTo = new StringBuilder();
        private int mailText_ID = 0;
        private List<String> sendInfo = null;
        private String subject = "";
        protected override string DoIt()
        {
            _req = new MRequest(GetCtx(), GetRecord_ID(), null);

            // check mail template if found on request or request type.
            mailText_ID = _req.GetVAR_MailTemplate_ID();
            if (mailText_ID == 0)
            {
                MRequestType reqType = new MRequestType(GetCtx(), _req.GetVAR_Req_Type_ID(), null);
                if (reqType.GetVAR_MailTemplate_ID() > 0)
                {
                    mailText_ID = reqType.GetVAR_MailTemplate_ID();
                }

            }
            if (mailText_ID == 0)
            {
                GetReqHistory();
                if (_reqAction == null)
                {
                    return Msg.GetMsg(GetCtx(), "R_NoReqChanges");
                }
                string changedValues = _reqAction.GetChangedValues();
                bool _changed = false;
                sendInfo = new List<String>();

                if (!string.IsNullOrEmpty(changedValues))
                {
                    string[] strValues = changedValues.Split(',');
                    if (strValues.Length > 0)
                    {
                        for (int i = 0; i < strValues.Length; i++)
                        {
                            _changed = true;
                            sendInfo.Add(strValues[i]);
                        }
                    }
                }

                #region commented
                //
                //if (_req.GetVAR_Req_Type_ID() != _reqAction.GetVAR_Req_Type_ID() && _reqAction.GetVAR_Req_Type_ID() > 0)
                //{
                //    _changed = true;
                //    sendInfo.Add("VAR_Req_Type_ID");
                //}
                //if (_req.GetR_Group_ID() != _reqAction.GetR_Group_ID() && _reqAction.GetR_Group_ID() > 0)
                //{
                //    _changed = true;
                //    sendInfo.Add("VAR_Group_ID");
                //}
                //if (_req.GetVAR_Category_ID() != _reqAction.GetVAR_Category_ID() && _reqAction.GetVAR_Category_ID() > 0)
                //{
                //    _changed = true;
                //    sendInfo.Add("VAR_Category_ID");
                //}
                //if (_req.GetVAR_Req_Status_ID() != _reqAction.GetVAR_Req_Status_ID() && _reqAction.GetVAR_Req_Status_ID() > 0)
                //{
                //    _changed = true;
                //    sendInfo.Add("VAR_Req_Status_ID");
                //}
                //if (_req.GetR_Resolution_ID() != _reqAction.GetR_Resolution_ID() && _reqAction.GetR_Resolution_ID() > 0)
                //{
                //    _changed = true;
                //    sendInfo.Add("VAR_Resolution_ID");
                //}
                ////
                //if (_req.GetSalesRep_ID() != _reqAction.GetSalesRep_ID() && _reqAction.GetSalesRep_ID() > 0)
                //{
                //    _changed = true;
                //    sendInfo.Add("SalesRep_ID");
                //}
                ////
                //if (_req.GetPriority() != _reqAction.GetPriority() && !string.IsNullOrEmpty(_reqAction.GetPriority()))
                //{
                //    _changed = true;
                //    sendInfo.Add("Priority");
                //}
                //if (_req.GetPriorityUser() != _reqAction.GetPriorityUser() && !string.IsNullOrEmpty(_reqAction.GetPriorityUser()))
                //{
                //    _changed = true;
                //    sendInfo.Add("PriorityUser");
                //}
                //if (_req.GetSummary() != _reqAction.GetSummary() && !string.IsNullOrEmpty(_reqAction.GetSummary()))
                //{
                //    _changed = true;
                //    sendInfo.Add("Summary");
                //}
                #endregion
                if (sendInfo.Count > 0 && _changed)
                {
                    prepareNotificMsg(sendInfo);
                    // For Role Changes
                    Thread thread = new Thread(new ThreadStart(() => SendNotices(sendInfo)));
                    thread.Start();
                    return Msg.GetMsg(GetCtx(), "R_EmailBackgrndRun");
                }
                else
                {
                    return Msg.GetMsg(GetCtx(), "R_NoReqChanges");
                }
            }
            else
            {
                prepareNotificMsg(sendInfo);
                // For Role Changes
                Thread thread = new Thread(new ThreadStart(() => SendNotices(sendInfo)));
                thread.Start();
                return Msg.GetMsg(GetCtx(), "R_EmailBackgrndRun");
            }

            return "";
        }

        /// <summary>
        /// Send notice to users.
        /// </summary>
        /// <param name="list"> List of columns changed.</param>
        protected void SendNotices(List<String> list)
        {
            bool isEmailSent = false;
            StringBuilder finalMsg = new StringBuilder();
            finalMsg.Append(Msg.Translate(GetCtx(), "VAR_Request_ID") + ": " + _req.GetDocumentNo()).Append("\n").Append(Msg.Translate(GetCtx(), "R_NotificSent"));
            //	Subject
            if (mailText_ID == 0)
            {
                subject = Msg.Translate(GetCtx(), "VAR_Request_ID")
                   + " " + Msg.GetMsg(GetCtx(), "Updated", true) + ": " + _req.GetDocumentNo() + " (●" + MVAFTableView.Get_Table_ID(Table_Name) + "-" + _req.GetVAR_Request_ID() + "●) " + Msg.GetMsg(GetCtx(), "DoNotChange");
            }
            //	Message

            //		UpdatedBy: Joe
            int UpdatedBy = GetCtx().GetVAF_UserContact_ID();
            MVAFUserContact from = MVAFUserContact.Get(GetCtx(), UpdatedBy);

            FileInfo pdf = CreatePDF();
            log.Finer(message.ToString());

            //	Prepare sending Notice/Mail
            MVAFClient client = MVAFClient.Get(GetCtx(), GetVAF_Client_ID());
            //	ReSet from if external
            if (from.GetEMailUser() == null || from.GetEMailUserPW() == null)
                from = null;
            _success = 0;
            _failure = 0;
            _notices = 0;

            /** List of users - aviod duplicates	*/
            List<int> userList = new List<int>();
            String sql = "SELECT u.VAF_UserContact_ID, u.NotificationType, u.EMail, u.Name, MAX(r.VAF_Role_ID) "
                + "FROM RV_RequestUpdates_Only ru"
                + " INNER JOIN VAF_UserContact u ON (ru.VAF_UserContact_ID=u.VAF_UserContact_ID)"
                + " LEFT OUTER JOIN VAF_UserContact_Roles r ON (u.VAF_UserContact_ID=r.VAF_UserContact_ID) "
                + "WHERE ru.VAR_Request_ID= " + _req.GetVAR_Request_ID()
                + " GROUP BY u.VAF_UserContact_ID, u.NotificationType, u.EMail, u.Name";

            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                while (idr.Read())
                {
                    int VAF_UserContact_ID = Utility.Util.GetValueOfInt(idr[0]);
                    String NotificationType = Util.GetValueOfString(idr[1]); //idr.GetString(1);
                    if (NotificationType == null)
                        NotificationType = X_VAF_UserContact.NOTIFICATIONTYPE_EMail;
                    String email = Util.GetValueOfString(idr[2]);// idr.GetString(2);

                    if (String.IsNullOrEmpty(email))
                    {
                        continue;
                    }

                    String Name = Util.GetValueOfString(idr[3]);//idr.GetString(3);
                    //	Role
                    int VAF_Role_ID = Utility.Util.GetValueOfInt(idr[4]);
                    if (idr == null)
                    {
                        VAF_Role_ID = -1;
                    }

                    //	Don't send mail to oneself
                    //		if (VAF_UserContact_ID == UpdatedBy)
                    //			continue;

                    //	No confidential to externals
                    if (VAF_Role_ID == -1
                        && (_req.GetConfidentialTypeEntry().Equals(X_VAR_Request.CONFIDENTIALTYPE_Internal)
                            || _req.GetConfidentialTypeEntry().Equals(X_VAR_Request.CONFIDENTIALTYPE_PrivateInformation)))
                        continue;

                    if (X_VAF_UserContact.NOTIFICATIONTYPE_None.Equals(NotificationType))
                    {
                        log.Config("Opt out: " + Name);
                        continue;
                    }
                    // JID_1858 IF User's Notification Type is Notice is allow to send notification to respective User
                    if ((X_VAF_UserContact.NOTIFICATIONTYPE_EMail.Equals(NotificationType)
                        || X_VAF_UserContact.NOTIFICATIONTYPE_EMailPlusNotice.Equals(NotificationType))
                        && (email == null || email.Length == 0))
                    {
                        if (VAF_Role_ID >= 0)
                            NotificationType = X_VAF_UserContact.NOTIFICATIONTYPE_Notice;
                        else
                        {
                            log.Config("No EMail: " + Name);
                            continue;
                        }
                    }
                    if (X_VAF_UserContact.NOTIFICATIONTYPE_Notice.Equals(NotificationType)
                        && VAF_Role_ID < 0)
                    {
                        log.Config("No internal User: " + Name);
                        continue;
                    }

                    //	Check duplicate receivers
                    int ii = VAF_UserContact_ID;
                    if (userList.Contains(ii))
                        continue;
                    userList.Add(ii);

                    // check the user roles for organization access.
                    MVAFUserContact user = new MVAFUserContact(GetCtx(), VAF_UserContact_ID, null);
                    MVAFRole[] role = user.GetRoles(GetVAF_Org_ID());
                    if (role.Length == 0)
                        continue;


                    //
                    SendNoticeNow(VAF_UserContact_ID, NotificationType,
                        client, from, subject, message.ToString(), pdf);
                    finalMsg.Append("\n").Append(user.GetName()).Append(".");
                    isEmailSent = true;
                }

                idr.Close();
                // Notification For Role
                List<int> _users = SendRoleNotice();
                for (int i = 0; i < _users.Count; i++)
                {
                    MVAFUserContact user = new MVAFUserContact(GetCtx(), _users[i], null);
                    int VAF_UserContact_ID = user.GetVAF_UserContact_ID();
                    String NotificationType = user.GetNotificationType(); //idr.GetString(1);
                    if (NotificationType == null)
                        NotificationType = X_VAF_UserContact.NOTIFICATIONTYPE_EMail;
                    String email = user.GetEMail();// idr.GetString(2);

                    if (String.IsNullOrEmpty(email))
                    {
                        continue;
                    }

                    String Name = user.GetName();//idr.GetString(3);
                                                 //	Role                  

                    if (X_VAF_UserContact.NOTIFICATIONTYPE_None.Equals(NotificationType))
                    {
                        log.Config("Opt out: " + Name);
                        continue;
                    }

                    //
                    SendNoticeNow(_users[i], NotificationType,
                        client, from, subject, message.ToString(), pdf);
                    finalMsg.Append("\n").Append(user.GetName()).Append(".");
                    isEmailSent = true;
                }

                if (!isEmailSent)
                {
                    finalMsg.Clear();
                    finalMsg.Append(Msg.Translate(GetCtx(), "VAR_Request_ID") + ": " + _req.GetDocumentNo()).Append("\n").Append(Msg.Translate(GetCtx(), "R_NoNotificationSent"));
                }

                int VAF_Msg_Lable_ID = 834;
                MVAFNotice note = new MVAFNotice(GetCtx(), VAF_Msg_Lable_ID, GetCtx().GetVAF_UserContact_ID(),
                    X_VAR_Request.Table_ID, _req.GetVAR_Request_ID(),
                    subject, finalMsg.ToString(), Get_TrxName());
                if (note.Save())
                    log.Log(Level.INFO, "ProcessFinished", "");
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }


            //	New Sales Rep (may happen if sent from beforeSave
            if (!userList.Contains(_req.GetSalesRep_ID()))
                SendNoticeNow(_req.GetSalesRep_ID(), null,
                    client, from, subject, message.ToString(), pdf);


        }

        /// <summary>
        /// get history record of request.
        /// </summary>
        private void GetReqHistory()
        {
            int _reqAction_ID = 0;
            string sql = "SELECT VAR_Req_History_ID FROM VAR_Req_History WHERE VAR_Request_ID=" + _req.GetVAR_Request_ID() + " AND IsActive='Y'  ORDER BY VAR_Req_History_ID DESC";
            _reqAction_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql));
            if (_reqAction_ID > 0)
            {
                _reqAction = new MRequestAction(GetCtx(), _reqAction_ID, null);
            }

        }

        /// <summary>
        /// Prepare notice message.
        /// </summary>
        /// <param name="list">list of the values changed.</param>
        private void prepareNotificMsg(List<String> list)
        {
            if (mailText_ID == 0)
            {
                message = new StringBuilder();
                //		UpdatedBy: Joe
                int UpdatedBy = GetCtx().GetVAF_UserContact_ID();
                MVAFUserContact from = MVAFUserContact.Get(GetCtx(), UpdatedBy);
                if (from != null)
                    message.Append(Msg.Translate(GetCtx(), "UpdatedBy")).Append(": ")
                        .Append(from.GetName());
                //		LastAction/Created: ...	
                if (_req.GetDateLastAction() != null)
                    message.Append("\n").Append(Msg.Translate(GetCtx(), "DateLastAction"))
                        .Append(": ").Append(_req.GetDateLastAction());
                else
                    message.Append("\n").Append(Msg.Translate(GetCtx(), "Created"))
                        .Append(": ").Append(_req.GetCreated());
                //	Changes
                for (int i = 0; i < list.Count; i++)
                {
                    X_VAR_Request req = new X_VAR_Request(GetCtx(), 0, null);

                    String columnName = (String)list[i];
                    message.Append("\n").Append(Msg.GetElement(GetCtx(), columnName))
                        .Append(": ").Append(_reqAction.getColumnValue(columnName))
                        .Append(" -> ").Append(_req.getColumnValue(columnName));
                }
                //	NextAction
                if (_req.GetDateNextAction() != null)
                    message.Append("\n").Append(Msg.Translate(GetCtx(), "DateNextAction"))
                        .Append(": ").Append(_req.GetDateNextAction());
                message.Append(SEPARATOR)
                    .Append(_req.GetSummary());
                if (_req.GetResult() != null)
                    message.Append("\n----------\n").Append(_req.GetResult());
                message.Append(_req.GetMailTrailer(null));
            }
            else
            {
                message = new StringBuilder();

                MMailText text = new MMailText(GetCtx(), mailText_ID, null);
                text.SetPO(_req, true); //Set _Po Current value
                subject += _req.GetDocumentNo() + ": " + text.GetMailHeader();

                message.Append(text.GetMailText(true));
                if (_req.GetDateNextAction() != null)
                    message.Append("\n").Append(Msg.Translate(GetCtx(), "DateNextAction"))
                        .Append(": ").Append(_req.GetDateNextAction());

                // message.Append(GetMailTrailer(null));
            }

        }

        /// <summary>
        /// Create pdf
        /// </summary>
        /// <returns> returns fileinfo.</returns>
        public FileInfo CreatePDF()
        {
            try
            {
                string fileName = _req.Get_TableName() + _req.Get_ID() + "_" + CommonFunctions.GenerateRandomNo()
                                    + ".txt"; //.pdf
                string filePath = Path.GetTempPath() + fileName;

                FileInfo temp = new FileInfo(filePath);
                if (!temp.Exists)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                log.Severe("Could not create PDF - " + e.Message);
            }
            return null;
        }

        /// <summary>
        /// Send notice to user
        /// </summary>
        /// <param name="VAF_UserContact_ID">Id of user</param>
        /// <param name="NotificationType"> Notification type</param>
        /// <param name="client"> Tenant object</param>
        /// <param name="from"> From user notice</param>
        /// <param name="subject">Subject of notice.</param>
        /// <param name="message">Message to be sent to user</param>
        /// <param name="pdf"> Attachment</param>
        private void SendNoticeNow(int VAF_UserContact_ID, String NotificationType,
          MVAFClient client, MVAFUserContact from, String subject, String message, FileInfo pdf)
        {
            MVAFUserContact to = MVAFUserContact.Get(GetCtx(), VAF_UserContact_ID);
            if (NotificationType == null)
                NotificationType = to.GetNotificationType();
            //	Send Mail
            if (X_VAF_UserContact.NOTIFICATIONTYPE_EMail.Equals(NotificationType)
                || X_VAF_UserContact.NOTIFICATIONTYPE_EMailPlusNotice.Equals(NotificationType))
            {
                VAdvantage.Model.MMailAttachment1 _mAttachment = new VAdvantage.Model.MMailAttachment1(GetCtx(), 0, null);
                _mAttachment.SetVAF_Client_ID(GetCtx().GetVAF_Client_ID());
                _mAttachment.SetVAF_Org_ID(GetCtx().GetVAF_Org_ID());
                _mAttachment.SetVAF_TableView_ID(MVAFTableView.Get_Table_ID(Table_Name));
                _mAttachment.IsActive();
                _mAttachment.SetMailAddress("");
                _mAttachment.SetAttachmentType("M");
                _mAttachment.SetRecord_ID(_req.GetVAR_Request_ID());
                _mAttachment.SetTextMsg(message);
                _mAttachment.SetTitle(subject);
                _mAttachment.SetMailAddress(to.GetEMail());

                if (from != null && !string.IsNullOrEmpty(from.GetEMail()))
                {
                    _mAttachment.SetMailAddressFrom(from.GetEMail());
                }
                else
                {
                    _mAttachment.SetMailAddressFrom(client.GetRequestEMail());
                }

                _mAttachment.NewRecord();

                if (client.SendEMail(from, to, subject, message.ToString(), pdf))
                {
                    _success++;
                    if (_emailTo.Length > 0)
                        _emailTo.Append(", ");
                    _emailTo.Append(to.GetEMail());
                    _mAttachment.SetIsMailSent(true);
                }
                else
                {
                    log.Warning("Failed: " + to);
                    _failure++;
                    NotificationType = X_VAF_UserContact.NOTIFICATIONTYPE_Notice;
                    _mAttachment.SetIsMailSent(false);
                }

                _mAttachment.Save();
            }

            //	Send Note
            if (X_VAF_UserContact.NOTIFICATIONTYPE_Notice.Equals(NotificationType)
                || X_VAF_UserContact.NOTIFICATIONTYPE_EMailPlusNotice.Equals(NotificationType))
            {
                int VAF_Msg_Lable_ID = 834;
                MVAFNotice note = new MVAFNotice(GetCtx(), VAF_Msg_Lable_ID, VAF_UserContact_ID,
                    X_VAR_Request.Table_ID, _req.GetVAR_Request_ID(),
                    subject, message.ToString(), Get_TrxName());
                if (note.Save())
                    _notices++;
            }

        }

        /// <summary>
        /// Send notice to role
        /// </summary>
        /// <param name="list">change information</param>
        private List<int> SendRoleNotice()
        {
            List<int> _users = new List<int>();
            string sql = @"SELECT VAF_UserContact.VAF_UserContact_ID,
                         VAF_UserContact_Roles.VAF_Role_ID
                        FROM VAF_UserContact_Roles
                        INNER JOIN VAF_UserContact
                        ON (VAF_UserContact_Roles.VAF_UserContact_ID    =VAF_UserContact.VAF_UserContact_ID)
                        WHERE VAF_UserContact_Roles.VAF_Role_ID IN
                          (SELECT VAF_Role_ID
                          FROM VAR_Rtype_UpdatesAlert
                          WHERE VAF_Role_ID   IS NOT NULL
                          AND VAR_Req_Type_ID=" + _req.GetVAR_Req_Type_ID() + @"
                          AND IsActive        ='Y'
                          )
                        AND VAF_UserContact_Roles.VAF_UserContact_ID NOT IN
                          (SELECT u.VAF_UserContact_ID
                          FROM RV_RequestUpdates_Only ru
                          INNER JOIN VAF_UserContact u
                          ON (ru.VAF_UserContact_ID=u.VAF_UserContact_ID)
                          LEFT OUTER JOIN VAF_UserContact_Roles r
                          ON (u.VAF_UserContact_ID     =r.VAF_UserContact_ID)
                          WHERE ru.VAR_Request_ID=" + _req.GetVAR_Request_ID() + @"
                          )
                        AND VAF_UserContact.email IS NOT NULL";

            DataSet _ds = DB.ExecuteDataset(sql, null, null);
            if (_ds != null && _ds.Tables[0].Rows.Count > 0)
            {
                _users = validateUsers(_ds);
            }
            return _users;

        }

        /// <summary>
        /// Validate the organization access of users according to the role.
        /// </summary>
        /// <param name="_ds"></param>
        /// <returns></returns>
        private List<int> validateUsers(DataSet _ds)
        {
            List<int> users = new List<int>();
            MVAFRole role = new MVAFRole(GetCtx(), Util.GetValueOfInt(_ds.Tables[0].Rows[0]["VAF_Role_ID"]), null);
            bool isAllUser = false;
            // if access all organization
            if (role.IsAccessAllOrgs())
            {
                isAllUser = true;
            }
            // if not access user organization access.
            if (!isAllUser && !role.IsUseUserOrgAccess())
            {
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_Org_ID) FROm VAF_Role_OrgRights WHERE IsActive='Y' AND  VAF_Role_ID=" + role.GetVAF_Role_ID() + " AND VAF_Org_ID IN (" + _req.GetVAF_Org_ID() + ",0)")) > 0)
                {
                    isAllUser = true;
                }
                else
                {
                    return users;
                }
            }
            for (int i = 0; i < _ds.Tables[0].Rows.Count; i++)
            {
                if (isAllUser)
                {
                    users.Add(Util.GetValueOfInt(_ds.Tables[0].Rows[i]["VAF_UserContact_ID"]));
                }
                else
                {
                    if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_Org_ID) FROm VAF_UserContact_OrgRights WHERE VAF_UserContact_ID=" + Util.GetValueOfInt(_ds.Tables[0].Rows[i]["VAF_UserContact_ID"]) + " AND  IsActive='Y' AND  VAF_Org_ID IN (" + _req.GetVAF_Org_ID() + ",0)")) > 0)
                    {
                        users.Add(Util.GetValueOfInt(_ds.Tables[0].Rows[i]["VAF_UserContact_ID"]));
                    }
                }
            }
            return users;
        }
        protected override void Prepare()
        {

        }
    }
}
