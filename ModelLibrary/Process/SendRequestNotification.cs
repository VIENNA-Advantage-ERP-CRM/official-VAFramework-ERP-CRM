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
        public static String Table_Name = "R_Request";
        private int _success = 0;
        private int _failure = 0;
        private int _notices = 0;
        private StringBuilder _emailTo = new StringBuilder();
        private int mailText_ID = 0;
        private List<String> sendInfo = null;
        private String subject = "";
        protected override string DoIt()
        {
            _req = new MRequest(GetCtx(), GetRecord_ID(), Get_Trx());
            MRequestType reqType = MRequestType.Get(GetCtx(), _req.GetR_RequestType_ID());
            // check mail template if found on request or request type.
            mailText_ID = _req.GetR_MailText_ID();
            if (mailText_ID == 0)
            {

                if (reqType.GetR_MailText_ID() > 0)
                {
                    mailText_ID = reqType.GetR_MailText_ID();
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
                //if (_req.GetR_RequestType_ID() != _reqAction.GetR_RequestType_ID() && _reqAction.GetR_RequestType_ID() > 0)
                //{
                //    _changed = true;
                //    sendInfo.Add("R_RequestType_ID");
                //}
                //if (_req.GetR_Group_ID() != _reqAction.GetR_Group_ID() && _reqAction.GetR_Group_ID() > 0)
                //{
                //    _changed = true;
                //    sendInfo.Add("R_Group_ID");
                //}
                //if (_req.GetR_Category_ID() != _reqAction.GetR_Category_ID() && _reqAction.GetR_Category_ID() > 0)
                //{
                //    _changed = true;
                //    sendInfo.Add("R_Category_ID");
                //}
                //if (_req.GetR_Status_ID() != _reqAction.GetR_Status_ID() && _reqAction.GetR_Status_ID() > 0)
                //{
                //    _changed = true;
                //    sendInfo.Add("R_Status_ID");
                //}
                //if (_req.GetR_Resolution_ID() != _reqAction.GetR_Resolution_ID() && _reqAction.GetR_Resolution_ID() > 0)
                //{
                //    _changed = true;
                //    sendInfo.Add("R_Resolution_ID");
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
                if ((sendInfo.Count > 0 && _changed) || _req.GetResult() != null)
                {
                    prepareNotificMsg(sendInfo);

                    //	Update Request Result
                    if (mailText_ID == 0 && !reqType.IsR_AllowSaveNotify())
                    {
                        if (_reqAction != null)
                            _req.SetDateLastAction(_reqAction.GetCreated());
                        _req.SetLastResult(_req.GetResult());
                        _req.SetDueType();
                        //	ReSet Reqiuest  Values
                        _req.SetConfidentialTypeEntry(_req.GetConfidentialType());
                        _req.SetStartDate(null);
                        _req.SetEndTime(null);
                        _req.SetR_StandardResponse_ID(0);
                        _req.SetR_MailText_ID(0);
                        _req.SetResult(null);
                        if (!_req.Save())
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            if (pp != null)
                            {
                                return !string.IsNullOrEmpty(pp.GetName()) ? pp.GetName() : Msg.GetMsg(GetCtx(), pp.GetValue());
                            }
                            else
                            {
                                return Msg.GetMsg(GetCtx(), "R_NoReqChanges");
                            }
                        }
                    }

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
            finalMsg.Append(Msg.Translate(GetCtx(), "R_Request_ID") + ": " + _req.GetDocumentNo()).Append("\n").Append(Msg.Translate(GetCtx(), "R_NotificSent"));
            //	Subject
            if (mailText_ID == 0)
            {
                subject = Msg.Translate(GetCtx(), "R_Request_ID")
                   + " " + Msg.GetMsg(GetCtx(), "Updated", true) + ": " + _req.GetDocumentNo() + " (●" + MTable.Get_Table_ID(Table_Name) + "-" + _req.GetR_Request_ID() + "●) " + Msg.GetMsg(GetCtx(), "DoNotChange");
            }
            //	Message

            //		UpdatedBy: Joe
            int UpdatedBy = GetCtx().GetAD_User_ID();
            MUser from = MUser.Get(GetCtx(), UpdatedBy);

            FileInfo pdf = CreatePDF();
            log.Finer(message.ToString());

            //	Prepare sending Notice/Mail
            MClient client = MClient.Get(GetCtx(), GetAD_Client_ID());
            //	ReSet from if external
            if (from.GetEMailUser() == null || from.GetEMailUserPW() == null)
                from = null;
            _success = 0;
            _failure = 0;
            _notices = 0;

            /** List of users - aviod duplicates	*/
            List<int> userList = new List<int>();
            String sql = "SELECT u.AD_User_ID, u.NotificationType, u.EMail, u.Name, MAX(r.AD_Role_ID) "
                + "FROM RV_RequestUpdates_Only ru"
                + " INNER JOIN AD_User u ON (ru.AD_User_ID=u.AD_User_ID)"
                + " LEFT OUTER JOIN AD_User_Roles r ON (u.AD_User_ID=r.AD_User_ID) "
                + "WHERE ru.R_Request_ID= " + _req.GetR_Request_ID()
                + " GROUP BY u.AD_User_ID, u.NotificationType, u.EMail, u.Name";

            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                while (idr.Read())
                {
                    int AD_User_ID = Utility.Util.GetValueOfInt(idr[0]);
                    String NotificationType = Util.GetValueOfString(idr[1]); //idr.GetString(1);
                    if (NotificationType == null)
                        NotificationType = X_AD_User.NOTIFICATIONTYPE_EMail;
                    String email = Util.GetValueOfString(idr[2]);// idr.GetString(2);

                    if (String.IsNullOrEmpty(email))
                    {
                        continue;
                    }

                    String Name = Util.GetValueOfString(idr[3]);//idr.GetString(3);
                    //	Role
                    int AD_Role_ID = Utility.Util.GetValueOfInt(idr[4]);
                    if (idr == null)
                    {
                        AD_Role_ID = -1;
                    }

                    //	Don't send mail to oneself
                    //		if (AD_User_ID == UpdatedBy)
                    //			continue;

                    //	No confidential to externals
                    if (AD_Role_ID == -1
                        && (_req.GetConfidentialTypeEntry().Equals(X_R_Request.CONFIDENTIALTYPE_Internal)
                            || _req.GetConfidentialTypeEntry().Equals(X_R_Request.CONFIDENTIALTYPE_PrivateInformation)))
                        continue;

                    if (X_AD_User.NOTIFICATIONTYPE_None.Equals(NotificationType))
                    {
                        log.Config("Opt out: " + Name);
                        continue;
                    }
                    // JID_1858 IF User's Notification Type is Notice is allow to send notification to respective User
                    if ((X_AD_User.NOTIFICATIONTYPE_EMail.Equals(NotificationType)
                        || X_AD_User.NOTIFICATIONTYPE_EMailPlusNotice.Equals(NotificationType))
                        && (email == null || email.Length == 0))
                    {
                        if (AD_Role_ID >= 0)
                            NotificationType = X_AD_User.NOTIFICATIONTYPE_Notice;
                        else
                        {
                            log.Config("No EMail: " + Name);
                            continue;
                        }
                    }
                    if (X_AD_User.NOTIFICATIONTYPE_Notice.Equals(NotificationType)
                        && AD_Role_ID < 0)
                    {
                        log.Config("No internal User: " + Name);
                        continue;
                    }

                    //	Check duplicate receivers
                    int ii = AD_User_ID;
                    if (userList.Contains(ii))
                        continue;
                    userList.Add(ii);

                    // check the user roles for organization access.
                    MUser user = new MUser(GetCtx(), AD_User_ID, null);

                    // VIS0060: Commented after discussion with Mukesh and Mohit, as there will be no role of requested User.
                    //MRole[] role = user.GetRoles(GetAD_Org_ID());
                    //if (role.Length == 0)
                    //    continue;


                    //
                    SendNoticeNow(AD_User_ID, NotificationType,
                        client, from, subject, message.ToString(), pdf);
                    finalMsg.Append("\n").Append(user.GetName()).Append(".");
                    isEmailSent = true;
                }

                idr.Close();
                // Notification For Role
                List<int> _users = SendRoleNotice();
                for (int i = 0; i < _users.Count; i++)
                {
                    MUser user = new MUser(GetCtx(), _users[i], null);
                    int AD_User_ID = user.GetAD_User_ID();
                    String NotificationType = user.GetNotificationType(); //idr.GetString(1);
                    if (NotificationType == null)
                        NotificationType = X_AD_User.NOTIFICATIONTYPE_EMail;
                    String email = user.GetEMail();// idr.GetString(2);

                    if (String.IsNullOrEmpty(email))
                    {
                        continue;
                    }

                    String Name = user.GetName();//idr.GetString(3);
                                                 //	Role                  

                    if (X_AD_User.NOTIFICATIONTYPE_None.Equals(NotificationType))
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
                    finalMsg.Append(Msg.Translate(GetCtx(), "R_Request_ID") + ": " + _req.GetDocumentNo()).Append("\n").Append(Msg.Translate(GetCtx(), "R_NoNotificationSent"));
                }

                int AD_Message_ID = 834;
                MNote note = new MNote(GetCtx(), AD_Message_ID, GetCtx().GetAD_User_ID(),
                    X_R_Request.Table_ID, _req.GetR_Request_ID(),
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
            string sql = "SELECT R_RequestAction_ID FROM R_RequestAction WHERE R_Request_ID=" + _req.GetR_Request_ID() + " AND IsActive='Y'  ORDER BY R_RequestAction_ID DESC";
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
                int UpdatedBy = GetCtx().GetAD_User_ID();
                MUser from = MUser.Get(GetCtx(), UpdatedBy);
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
                    X_R_Request req = new X_R_Request(GetCtx(), 0, null);

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
        /// <param name="AD_User_ID">Id of user</param>
        /// <param name="NotificationType"> Notification type</param>
        /// <param name="client"> Tenant object</param>
        /// <param name="from"> From user notice</param>
        /// <param name="subject">Subject of notice.</param>
        /// <param name="message">Message to be sent to user</param>
        /// <param name="pdf"> Attachment</param>
        private void SendNoticeNow(int AD_User_ID, String NotificationType,
          MClient client, MUser from, String subject, String message, FileInfo pdf)
        {
            MUser to = MUser.Get(GetCtx(), AD_User_ID);
            if (NotificationType == null)
                NotificationType = to.GetNotificationType();
            //	Send Mail
            if (X_AD_User.NOTIFICATIONTYPE_EMail.Equals(NotificationType)
                || X_AD_User.NOTIFICATIONTYPE_EMailPlusNotice.Equals(NotificationType))
            {
                VAdvantage.Model.MMailAttachment1 _mAttachment = new VAdvantage.Model.MMailAttachment1(GetCtx(), 0, null);
                _mAttachment.SetAD_Client_ID(GetCtx().GetAD_Client_ID());
                _mAttachment.SetAD_Org_ID(GetCtx().GetAD_Org_ID());
                _mAttachment.SetAD_Table_ID(MTable.Get_Table_ID(Table_Name));
                _mAttachment.IsActive();
                _mAttachment.SetMailAddress("");
                _mAttachment.SetAttachmentType("M");
                _mAttachment.SetRecord_ID(_req.GetR_Request_ID());
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
                    NotificationType = X_AD_User.NOTIFICATIONTYPE_Notice;
                    _mAttachment.SetIsMailSent(false);
                }

                _mAttachment.Save();
            }

            //	Send Note
            if (X_AD_User.NOTIFICATIONTYPE_Notice.Equals(NotificationType)
                || X_AD_User.NOTIFICATIONTYPE_EMailPlusNotice.Equals(NotificationType))
            {
                int AD_Message_ID = 834;
                MNote note = new MNote(GetCtx(), AD_Message_ID, AD_User_ID,
                    X_R_Request.Table_ID, _req.GetR_Request_ID(),
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
            string sql = @"SELECT AD_User.ad_user_ID,
                         AD_User_Roles.AD_Role_ID
                        FROM AD_User_Roles
                        INNER JOIN ad_user
                        ON (AD_User_Roles.AD_User_ID    =AD_User.AD_User_ID)
                        WHERE AD_User_Roles.AD_Role_ID IN
                          (SELECT AD_Role_ID
                          FROM R_RequestTypeUpdates
                          WHERE AD_Role_ID   IS NOT NULL
                          AND R_RequestType_ID=" + _req.GetR_RequestType_ID() + @"
                          AND IsActive        ='Y'
                          )
                        AND AD_User_Roles.AD_User_ID NOT IN
                          (SELECT u.AD_User_ID
                          FROM RV_RequestUpdates_Only ru
                          INNER JOIN AD_User u
                          ON (ru.AD_User_ID=u.AD_User_ID)
                          LEFT OUTER JOIN AD_User_Roles r
                          ON (u.AD_User_ID     =r.AD_User_ID)
                          WHERE ru.R_Request_ID=" + _req.GetR_Request_ID() + @"
                          )
                        AND ad_user.email IS NOT NULL";

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
            MRole role = new MRole(GetCtx(), Util.GetValueOfInt(_ds.Tables[0].Rows[0]["AD_Role_ID"]), null);
            bool isAllUser = false;
            // if access all organization
            if (role.IsAccessAllOrgs())
            {
                isAllUser = true;
            }
            // if not access user organization access.
            if (!isAllUser && !role.IsUseUserOrgAccess())
            {
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_Org_ID) FROm AD_Role_OrgAccess WHERE IsActive='Y' AND  AD_Role_ID=" + role.GetAD_Role_ID() + " AND AD_Org_ID IN (" + _req.GetAD_Org_ID() + ",0)")) > 0)
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
                    users.Add(Util.GetValueOfInt(_ds.Tables[0].Rows[i]["AD_User_ID"]));
                }
                else
                {
                    if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_Org_ID) FROm AD_User_OrgAccess WHERE AD_User_ID=" + Util.GetValueOfInt(_ds.Tables[0].Rows[i]["AD_User_ID"]) + " AND  IsActive='Y' AND  AD_Org_ID IN (" + _req.GetAD_Org_ID() + ",0)")) > 0)
                    {
                        users.Add(Util.GetValueOfInt(_ds.Tables[0].Rows[i]["AD_User_ID"]));
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
