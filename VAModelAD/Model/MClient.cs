using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VAModelAD.Model
{
    public class MClient: VAdvantage.Model.X_VAF_Client
    {

        private static CCache<int, MClient> s_cache = new CCache<int, MClient>("VAF_Client_AD", 3);

        private static VLogger s_log = VLogger.GetVLogger(typeof(MClient).FullName);

        private VAdvantage.Login.Language _language = null;
        public MClient(Ctx ctx, int VAF_Client_ID, Trx trxName)
            : base(ctx, VAF_Client_ID, trxName)
        {

        }

        public MClient(Ctx ctx, DataRow dr, Trx trx):base(ctx,dr,trx)
        {
        }

        internal static MClient Get(Ctx ctx)
        {
            return Get(ctx, ctx.GetVAF_Client_ID());
        }

        internal static MClient Get(Ctx ctx, int VAF_Client_ID)
        {
            int key = VAF_Client_ID;
            MClient client = (MClient)s_cache[key];
            if (client != null)
                return client;
            client = new MClient(ctx, VAF_Client_ID, null);
            if (VAF_Client_ID == 0)
                client.Load((Trx)null);
            s_cache.Add(key, client);
            return client;
        }

        public VAdvantage.Login.Language GetLanguage()
        {
            if (_language == null)
            {
                _language = VAdvantage.Login.Language.GetLanguage(GetVAF_Language());
                _language = Env.VerifyLanguage(GetCtx(), _language);
            }
            return _language;
        }   //	getLanguage

        public bool IsAutoUpdateTrl(String strTableName)
        {
            if (base.IsMultiLingualDocument())
                return false;
            if (strTableName == null)
                return false;
            //	Not Multi-Lingual Documents - only Doc Related
            if (strTableName.StartsWith("AD"))
                return false;
            return true;
        }

        /**
	 * 	Is Auto Archive on
	 *	@return true if auto archive
	 */
        public bool IsAutoArchive()
        {
            String aa = GetAutoArchive();
            return aa != null && !aa.Equals(AUTOARCHIVE_None);
        }   //	

        internal static MClient[] GetAll(Ctx ctx)
        {
            List<MClient> list = new List<MClient>();
            String sql = "SELECT * FROM VAF_Client";
            try
            {
                DataSet ds = DB.ExecuteDataset(sql, null, null);

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    MClient client = new MClient(ctx, dr, null);
                    list.Add(client);
                }
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, sql, e);
            }

            MClient[] RetValue = new MClient[list.Count()];
            RetValue = list.ToArray();
            return RetValue;
        }   //	getAll


        
        ///////////////////HTMLBODY//////////
        #region htmlbody
        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public EMail CreateEMail(MUser from, MUser to, String subject, String message, bool isHTML)
        {
            if (to == null)
            {
                //log.warning("No To user");
                return null;
            }
            if (to.GetEMail() == null || to.GetEMail().Length == 0)
            {
                //log.warning("No To address: " + to);
                return null;
            }
            if (to.IsEMailBounced())
            {
                //log.warning("EMail bounced: " + to.GetBouncedInfo() + " - " + to.GetEMail());
                return null;
            }
            return CreateEMail(from, to.GetEMail(), to.GetName(), subject, message, isHTML);
        }


        public EMail CreateEMail(PO from, PO to, String subject, String message, bool isHTML)
        {
            if (to == null)
            {
                //log.warning("No To user");
                return null;
            }
            string toEMail = Util.GetValueOfString(to.Get_Value("EMail"));
            if (toEMail == null || toEMail.Length == 0)
            {
                //log.warning("No To address: " + to);
                return null;
            }
            if ((bool)to.Get_Value("IsMailBounced"))
            {
                //log.warning("EMail bounced: " + to.GetBouncedInfo() + " - " + to.GetEMail());
                return null;
            }
            return CreateEMail(from, Util.GetValueOfString(to.Get_Value("GetEMail")), Util.GetValueOfString(to.Get_Value("Name")), subject, message, isHTML);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="toEMail"></param>
        /// <param name="toName"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public EMail CreateEMail(MUser from, String toEMail, String toName, String subject, String message, bool isHTML)
        {
            if (toEMail == null || toEMail.Length == 0)
            {
                //log.warning("No To address");
                return null;
            }
            //	No From - send from Request
            if (from == null)
                return CreateEMail(toEMail, toName, subject, message, isHTML);
            //	No From details - Error
            if (from.GetEMail() == null
                || from.GetEMailUser() == null || from.GetEMailUserPW() == null)
            {
                //log.warning("From EMail incomplete: " + from + " (" + GetName() + ")");
                return null;
            }
            //
            EMail email = null;
            if (IsServerEMail() && Ini.IsClient())
            {
                //Server server = CConnection.get().getServer();
                //try
                //{
                //    if (server != null)
                //    {	//	See ServerBean
                //        email = server.createEMail(GetCtx(), GetVAF_Client_ID(),
                //            from.GetAD_User_ID(),
                //            toEMail, toName,
                //            subject, message);
                //    }
                //    else
                //        log.log(Level.WARNING, "No AppsServer");
                //}
                //catch (RemoteException ex)
                //{
                //    log.log(Level.SEVERE, GetName() + " - AppsServer error", ex);
                //}
            }
            if (email == null)
            {
                email = new EMail(this, from.GetEMail(), from.GetName(), toEMail, toName,
                        subject, message);
                email.ISHTML = isHTML;
            }
            if (!email.IsValid())
                return null;
            if (IsSmtpAuthorization())
                email.CreateAuthenticator(from.GetEMailUser(), from.GetEMailUserPW());
            return email;
        }

        public EMail CreateEMail(PO from, String toEMail, String toName, String subject, String message, bool isHTML)
        {
            if (toEMail == null || toEMail.Length == 0)
            {
                //log.warning("No To address");
                return null;
            }
            //	No From - send from Request
            if (from == null)
                return CreateEMail(toEMail, toName, subject, message, isHTML);
            //	No From details - Error
            if (from.Get_Value("EMail") == null
                || from.Get_Value("EMailUser") == null || from.Get_Value("EMailUserPW") == null)
            {
                //log.warning("From EMail incomplete: " + from + " (" + GetName() + ")");
                return null;
            }
            //
            EMail email = null;
            if (IsServerEMail() && Ini.IsClient())
            {
                //Server server = CConnection.get().getServer();
                //try
                //{
                //    if (server != null)
                //    {	//	See ServerBean
                //        email = server.createEMail(GetCtx(), GetVAF_Client_ID(),
                //            from.GetAD_User_ID(),
                //            toEMail, toName,
                //            subject, message);
                //    }
                //    else
                //        log.log(Level.WARNING, "No AppsServer");
                //}
                //catch (RemoteException ex)
                //{
                //    log.log(Level.SEVERE, GetName() + " - AppsServer error", ex);
                //}
            }
            if (email == null)
            {
                email = new EMail(this, Util.GetValueOfString(from.Get_Value("EMail")), Util.GetValueOfString(from.Get_Value("Name")), toEMail, toName,
                        subject, message);
                email.ISHTML = isHTML;
            }
            if (!email.IsValid())
                return null;
            if (IsSmtpAuthorization())
                email.CreateAuthenticator(Util.GetValueOfString(from.Get_Value("EMailUser")), Util.GetValueOfString(from.Get_Value("EMailUserPW")));
            return email;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="toEMail"></param>
        /// <param name="toName"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public EMail CreateEMail(String toEMail, String toName, String subject, String message, bool isHTML)
        {
            if (toEMail == null || toEMail.Length == 0)
            {
                //log.warning("No To");
                return null;
            }
            //
            EMail email = null;
            if (IsServerEMail() && Ini.IsClient())
            {
                //Server server = CConnection.get().getServer();
                //try
                //{
                //    if (server != null)
                //    {	//	See ServerBean
                //        email = server.createEMail(GetCtx(), GetVAF_Client_ID(),
                //            toEMail, toName, subject, message);
                //    }
                //    else
                //        log.log(Level.WARNING, "No AppsServer");
                //}
                //catch (RemoteException ex)
                //{
                //    log.log(Level.SEVERE, GetName() + " - AppsServer error", ex);
                //}
            }
            if (email == null)
            {
                email = new EMail(this, GetRequestEMail(), GetName(), toEMail, toName,
                            subject, message);
                email.ISHTML = isHTML;
            }
            if (email == null || !email.IsValid())
                return null;
            if (IsSmtpAuthorization())
                email.CreateAuthenticator(GetRequestUser(), GetRequestUserPW());
            return email;
        }


        public bool SendEMail(MUser from, MUser to, String subject, String message, FileInfo attachment, bool isHTML)
        {
            EMail email = CreateEMail(from, to, subject, message, isHTML);
            if (email == null)
                return false;
            if (attachment != null)
                email.AddAttachment(attachment);
            MailAddress emailFrom = email.GetFrom();
            try
            {
                return SendEmailNow(from, to, email);
            }
            catch (Exception ex)
            {
                log.Severe(GetName() + " - from " + emailFrom + " to " + to + ": " + ex.Message);
                return false;
            }
        }

        public bool SendEMail(String toEMail, String toName, String subject, String message, FileInfo attachment, bool isHtml)
        {
            EMail email = CreateEMail(toEMail, toName, subject, message, isHtml);
            if (email == null)
                return false;
            if (attachment != null)
                email.AddAttachment(attachment);
            try
            {
                String msg = email.Send();
                if (EMail.SENT_OK.Equals(msg))
                {
                    log.Info("Sent EMail " + subject + " to " + toEMail);
                    return true;
                }
                else
                {
                    log.Warning("Could NOT Send Email: " + subject + " to " + toEMail + ": " + msg + " (" + GetName() + ")");
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.Severe(GetName() + " - " + ex.Message);
                return false;
            }
        }

        public bool SendEMail(String toEMail, String toName, String subject, String message, FileInfo attachment, bool isHtml, int VAF_TableView_ID, int Record_ID, byte[] array = null, String fileName = "Rpt.pdf")
        {
            EMail email = CreateEMail(toEMail, toName, subject, message, isHtml);
            if (email == null)
                return false;
            if (attachment != null)
                email.AddAttachment(attachment);
            if (array != null)
            {
                email.AddAttachment(array, fileName);
            }

            FetchRecordsAttachment(email, VAF_TableView_ID, Record_ID);
            try
            {
                String msg = email.Send();
                if (EMail.SENT_OK.Equals(msg))
                {
                    //log.info("Sent EMail " + subject + " to " + toEMail);
                    return true;
                }
                else
                {
                    //log.warning("Could NOT Send Email: " + subject + " to " + toEMail + ": " + msg + " (" + GetName() + ")");
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.Severe(GetName() + " - " + ex.Message);
                return false;
            }
        }

        private void FetchRecordsAttachment(EMail email, int VAF_TableView_ID, int Record_ID)
        {
            MAttachment mAttach = MAttachment.Get(p_ctx, VAF_TableView_ID, Record_ID);
            string filePath = ""; //System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "TempDownload");
            if (mAttach == null)
                return;
            if (mAttach.IsFromHTML())
            {
                for (int i = 0; i < mAttach._lines.Count; i++)
                {

                    filePath = System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "TempDownload");
                    filePath = System.IO.Path.Combine(filePath, mAttach.GetFile(mAttach._lines[i].Line_ID));
                    if (filePath.IndexOf("ERROR") > -1)
                    {
                        continue;
                    }
                    filePath = System.IO.Path.Combine(filePath, mAttach._lines[i].FileName);
                    email.AddAttachment(new FileInfo(filePath));

                }
            }
            else
            {
                foreach (MAttachmentEntry entry in mAttach.GetEntries())
                {
                    email.AddAttachment(entry.GetData(), entry.GetName());
                }
            }
        }


        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public EMail CreateEMail(MUser from, MUser to, String subject, String message)
        {
            if (to == null)
            {
                //log.warning("No To user");
                return null;
            }
            if (to.GetEMail() == null || to.GetEMail().Length == 0)
            {
                //log.warning("No To address: " + to);
                return null;
            }
            if (to.IsEMailBounced())
            {
                //log.warning("EMail bounced: " + to.GetBouncedInfo() + " - " + to.GetEMail());
                return null;
            }
            return CreateEMail(from, to.GetEMail(), to.GetName(), subject, message);
        }


        public EMail CreateEMail(PO from, PO to, String subject, String message)
        {
            if (to == null)
            {
                //log.warning("No To user");
                return null;
            }
            string toEMail = Util.GetValueOfString(to.Get_Value("EMail"));
            if (toEMail == null || toEMail.Length == 0)
            {
                //log.warning("No To address: " + to);
                return null;
            }
            if ((bool)to.Get_Value("IsMailBounced"))
            {
                //log.warning("EMail bounced: " + to.GetBouncedInfo() + " - " + to.GetEMail());
                return null;
            }
            return CreateEMail(from, Util.GetValueOfString(to.Get_Value("GetEMail")), Util.GetValueOfString(to.Get_Value("Name")), subject, message);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="toEMail"></param>
        /// <param name="toName"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public EMail CreateEMail(MUser from, String toEMail, String toName, String subject, String message)
        {
            if (toEMail == null || toEMail.Length == 0)
            {
                //log.warning("No To address");
                return null;
            }
            //	No From - send from Request
            if (from == null)
                return CreateEMail(toEMail, toName, subject, message);
            //	No From details - Error
            if (from.GetEMail() == null
                || from.GetEMailUser() == null || from.GetEMailUserPW() == null)
            {
                //log.warning("From EMail incomplete: " + from + " (" + GetName() + ")");
                return null;
            }
            //
            EMail email = null;
            if (IsServerEMail() && Ini.IsClient())
            {
                //Server server = CConnection.get().getServer();
                //try
                //{
                //    if (server != null)
                //    {	//	See ServerBean
                //        email = server.createEMail(GetCtx(), GetVAF_Client_ID(),
                //            from.GetAD_User_ID(),
                //            toEMail, toName,
                //            subject, message);
                //    }
                //    else
                //        log.log(Level.WARNING, "No AppsServer");
                //}
                //catch (RemoteException ex)
                //{
                //    log.log(Level.SEVERE, GetName() + " - AppsServer error", ex);
                //}
            }
            if (email == null)
                email = new EMail(this, from.GetEMail(), from.GetName(), toEMail, toName,
                        subject, message);
            if (!email.IsValid())
                return null;
            if (IsSmtpAuthorization())
                email.CreateAuthenticator(from.GetEMailUser(), from.GetEMailUserPW());
            return email;
        }

        public EMail CreateEMail(PO from, String toEMail, String toName, String subject, String message)
        {
            if (toEMail == null || toEMail.Length == 0)
            {
                //log.warning("No To address");
                return null;
            }
            //	No From - send from Request
            if (from == null)
                return CreateEMail(toEMail, toName, subject, message);
            //	No From details - Error
            if (from.Get_Value("EMail") == null
                || from.Get_Value("EMailUser") == null || from.Get_Value("EMailUserPW") == null)
            {
                //log.warning("From EMail incomplete: " + from + " (" + GetName() + ")");
                return null;
            }
            //
            EMail email = null;
            if (IsServerEMail() && Ini.IsClient())
            {
                //Server server = CConnection.get().getServer();
                //try
                //{
                //    if (server != null)
                //    {	//	See ServerBean
                //        email = server.createEMail(GetCtx(), GetVAF_Client_ID(),
                //            from.GetAD_User_ID(),
                //            toEMail, toName,
                //            subject, message);
                //    }
                //    else
                //        log.log(Level.WARNING, "No AppsServer");
                //}
                //catch (RemoteException ex)
                //{
                //    log.log(Level.SEVERE, GetName() + " - AppsServer error", ex);
                //}
            }
            if (email == null)
                email = new EMail(this, Util.GetValueOfString(from.Get_Value("EMail")), Util.GetValueOfString(from.Get_Value("Name")), toEMail, toName,
                        subject, message);
            if (!email.IsValid())
                return null;
            if (IsSmtpAuthorization())
                email.CreateAuthenticator(Util.GetValueOfString(from.Get_Value("EMailUser")), Util.GetValueOfString(from.Get_Value("EMailUserPW")));
            return email;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="toEMail"></param>
        /// <param name="toName"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public EMail CreateEMail(String toEMail, String toName, String subject, String message)
        {
            if (toEMail == null || toEMail.Length == 0)
            {
                //log.warning("No To");
                return null;
            }
            //
            EMail email = null;
            if (IsServerEMail() && Ini.IsClient())
            {
                //Server server = CConnection.get().getServer();
                //try
                //{
                //    if (server != null)
                //    {	//	See ServerBean
                //        email = server.createEMail(GetCtx(), GetVAF_Client_ID(),
                //            toEMail, toName, subject, message);
                //    }
                //    else
                //        log.log(Level.WARNING, "No AppsServer");
                //}
                //catch (RemoteException ex)
                //{
                //    log.log(Level.SEVERE, GetName() + " - AppsServer error", ex);
                //}
            }
            if (email == null)
                email = new EMail(this, GetRequestEMail(), GetName(), toEMail, toName,
                            subject, message);
            if (email == null || !email.IsValid())
                return null;
            if (IsSmtpAuthorization())
                email.CreateAuthenticator(GetRequestUser(), GetRequestUserPW());
            return email;
        }



        /// <summary>
        ///Test EMail
        /// </summary>
        /// <returns> OK or error</returns>
        public String TestEMail()
        {
            if (GetRequestEMail() == null || GetRequestEMail().Length == 0)
            {
                return "No Request EMail for " + GetName();
            }
            //
            EMail email = CreateEMail(GetRequestEMail(), GetName(),
                "Vienna Advantage EMail Test",
                "Vienna Advantage EMail Test: " + ToString());
            if (email == null)
                return "Could not create EMail: " + GetName();
            try
            {
                String msg = email.Send();
                if (EMail.SENT_OK.Equals(msg))
                {
                    log.Info("Sent Test EMail to " + GetRequestEMail());
                    return "OK";
                }
                else
                {
                    log.Warning("Could NOT send Test EMail from "
                        + GetSmtpHost() + ": " + GetRequestEMail()
                        + " (" + GetRequestUser()
                        + ") to " + GetRequestEMail() + ": " + msg);
                    return msg;
                }
            }
            catch (Exception ex)
            {
                log.Severe(GetName() + " - " + ex.Message);
                return ex.Message;
            }
        }   //	testEMail


        /// <summary>
        /// 
        /// </summary>
        /// <param name="AD_User_ID"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="attachment"></param>
        /// <returns></returns>
        public bool SendEMail(int AD_User_ID, String subject, String message, FileInfo attachment)
        {
            MUser to = MUser.Get(GetCtx(), AD_User_ID);
            String toEMail = to.GetEMail();
            if (toEMail == null || toEMail.Length == 0)
            {
                //log.warning("No EMail for recipient: " + to);
                return false;
            }
            if (to.IsEMailBounced())
            {
                //log.warning("EMail bounced for recipient: " + to);
                return false;
            }
            EMail email = CreateEMail(null, to, subject, message);
            if (email == null)
                return false;
            if (attachment != null)
                email.AddAttachment(attachment);
            try
            {
                return SendEmailNow(null, to, email);
            }
            catch (Exception ex)
            {
                log.Severe(GetName() + " - " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="attachment"></param>
        /// <returns></returns>
        public bool SendEMail(MUser from, MUser to, String subject, String message, FileInfo attachment)
        {
            EMail email = CreateEMail(from, to, subject, message);
            if (email == null)
                return false;
            if (attachment != null)
                email.AddAttachment(attachment);
            MailAddress emailFrom = email.GetFrom();
            try
            {
                return SendEmailNow(from, to, email);
            }
            catch (Exception ex)
            {
                log.Severe(GetName() + " - from " + emailFrom + " to " + to + ": " + ex.Message);
                return false;
            }
        }

        public bool SendEMail(PO from, PO to, String subject, String message, FileInfo attachment)
        {
            EMail email = CreateEMail(from, to, subject, message);
            if (email == null)
                return false;
            if (attachment != null)
                email.AddAttachment(attachment);
            MailAddress emailFrom = email.GetFrom();
            try
            {
                return SendEmailNow(from, to, email);
            }
            catch
            {
                //log.severe(GetName() + " - from " + emailFrom + " to " + to + ": " + ex.Message);
                return false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="toEMail"></param>
        /// <param name="toName"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="attachment"></param>
        /// <returns></returns>
        public bool SendEMail(String toEMail, String toName, String subject, String message, FileInfo attachment)
        {
            EMail email = CreateEMail(toEMail, toName, subject, message);
            if (email == null)
                return false;
            if (attachment != null)
                email.AddAttachment(attachment);
            try
            {
                String msg = email.Send();
                if (EMail.SENT_OK.Equals(msg))
                {
                    //log.info("Sent EMail " + subject + " to " + toEMail);
                    return true;
                }
                else
                {
                    //log.warning("Could NOT Send Email: " + subject + " to " + toEMail + ": " + msg + " (" + GetName() + ")");
                    return false;
                }
            }
            catch
            {
                //log.severe(GetName() + " - " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Send Email Now
        /// </summary>
        /// <param name="from">optional from user</param>
        /// <param name="to">to user</param>
        /// <param name="email">email</param>
        /// <returns>true if sent</returns>
        private bool SendEmailNow(MUser from, MUser to, EMail email)
        {
            String msg = email.Send();
            //
            X_AD_UserMail um = new X_AD_UserMail(GetCtx(), 0, null);
            um.SetClientOrg(this);
            um.SetAD_User_ID(to.GetAD_User_ID());
            um.SetSubject(email.GetSubject());
            um.SetMailText(email.GetMessageCRLF());
            if (email.IsSentOK())
                um.SetMessageID(email.GetMessageID());
            else
            {
                um.SetMessageID(email.GetSentMsg());
                um.SetIsDelivered(X_AD_UserMail.ISDELIVERED_No);
            }
            um.Save();

            //
            if (email.IsSentOK())
            {
                //if (from != null)
                //    log.info("Sent Email: " + email.GetSubject() + " from " + from.GetEMail() + " to " + to.GetEMail());
                //else
                //    log.info("Sent Email: " + email.GetSubject() + " to " + to.GetEMail());
                return true;
            }
            else
            {
                //if (from != null)
                //    log.warning("Could NOT Send Email: " + email.GetSubject()
                //        + " from " + from.GetEMail()
                //        + " to " + to.GetEMail() + ": " + msg
                //        + " (" + GetName() + ")");
                //else
                //    log.warning("Could NOT Send Email: " + email.GetSubject()
                //        + " to " + to.GetEMail() + ": " + msg
                //        + " (" + GetName() + ")");
                return false;
            }
        }

        private bool SendEmailNow(PO from, PO to, EMail email)
        {
            String msg = email.Send();
            //
            X_AD_UserMail um = new X_AD_UserMail(GetCtx(), 0, null);
            um.SetClientOrg(this);
            um.SetAD_User_ID((int)to.Get_Value("AD_User_ID"));
            um.SetSubject(email.GetSubject());
            um.SetMailText(email.GetMessageCRLF());
            if (email.IsSentOK())
                um.SetMessageID(email.GetMessageID());
            else
            {
                um.SetMessageID(email.GetSentMsg());
                um.SetIsDelivered(X_AD_UserMail.ISDELIVERED_No);
            }
            um.Save();

            //
            if (email.IsSentOK())
            {
                //if (from != null)
                //    log.info("Sent Email: " + email.GetSubject() + " from " + from.GetEMail() + " to " + to.GetEMail());
                //else
                //    log.info("Sent Email: " + email.GetSubject() + " to " + to.GetEMail());
                return true;
            }
            else
            {
                //if (from != null)
                //    log.warning("Could NOT Send Email: " + email.GetSubject()
                //        + " from " + from.GetEMail()
                //        + " to " + to.GetEMail() + ": " + msg
                //        + " (" + GetName() + ")");
                //else
                //    log.warning("Could NOT Send Email: " + email.GetSubject()
                //        + " to " + to.GetEMail() + ": " + msg
                //        + " (" + GetName() + ")");
                return false;
            }
        }

    }
}
