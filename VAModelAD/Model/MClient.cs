/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_AD_Client
 * Chronological Development
 * ------
 * Veena Pandey     01-June-2009 Added functions of mail
 ******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.IO;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;
using VAdvantage.Logging;

namespace VAModelAD.Model
{
    public class MClient : X_AD_Client
    {
       
        /**	Cache						*/
        private static CCache<int, MClient> s_cache = new CCache<int, MClient>("AD_Client", 3);

        private static VLogger s_log = VLogger.GetVLogger(typeof(MClient).FullName);



        private bool m_createNew = false;

        public MClient(Ctx ctx, int AD_Client_ID, Trx trxName)
           : base(ctx, AD_Client_ID, trxName)
        {

        }

        public MClient(Ctx ctx, DataRow dr, Trx trx) : base(ctx, dr, trx)
        {
        }

        internal static MClient Get(Ctx ctx)
        {
            return Get(ctx, ctx.GetAD_Client_ID());
        }

        internal static MClient Get(Ctx ctx, int AD_Client_ID)
        {
            int key = AD_Client_ID;
            MClient client = (MClient)s_cache[key];
            if (client != null)
                return client;
            client = new MClient(ctx, AD_Client_ID, null);
            if (AD_Client_ID == 0)
                client.Load((Trx)null);
            s_cache.Add(key, client);
            return client;
        }



        public VAdvantage.Login.Language GetLanguage()
        {
            if (_language == null)
            {
                _language = VAdvantage.Login.Language.GetLanguage(GetAD_Language());
                _language = Env.VerifyLanguage(GetCtx(), _language);
            }
            return _language;
        }	//	getLanguage

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
        }	//	


       

        /// <summary>
        /// Get all clients
        /// </summary>
        /// <param name="ctx">context</param>
        /// <returns>clients</returns>
        internal static MClient[] GetAll(Ctx ctx)
        {
            List<MClient> list = new List<MClient>();
            String sql = "SELECT * FROM AD_Client";
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
        }	//	getAll

       
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


        private VAdvantage.Login.Language _language = null;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public VAdvantage.Login.Language GetLanguage()
        //{
        //    if (_language == null)
        //    {
        //        _language = VAdvantage.Login.Language.GetLanguages(GetAD_Language());
        //        _language = Env.VerifyLanguage(GetCtx(), _language);
        //    }
        //    return _language;
        //}	//	getLanguage

        //public void SetAD_Language(String AD_Language)
        //{
        //    _language = null;
        //    base.SetAD_Language(AD_Language);
        //}	//	setAD_Language




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
            string toEMail = VAdvantage.Utility.Util.GetValueOfString(to.Get_Value("EMail"));
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
            return CreateEMail(from, VAdvantage.Utility.Util.GetValueOfString(to.Get_Value("GetEMail")), VAdvantage.Utility.Util.GetValueOfString(to.Get_Value("Name")), subject, message);
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
                //        email = server.createEMail(GetCtx(), GetAD_Client_ID(),
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
                //        email = server.createEMail(GetCtx(), GetAD_Client_ID(),
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
                email = new EMail(this, VAdvantage.Utility.Util.GetValueOfString(from.Get_Value("EMail")), VAdvantage.Utility.Util.GetValueOfString(from.Get_Value("Name")), toEMail, toName,
                        subject, message);
            if (!email.IsValid())
                return null;
            if (IsSmtpAuthorization())
                email.CreateAuthenticator(VAdvantage.Utility.Util.GetValueOfString(from.Get_Value("EMailUser")), VAdvantage.Utility.Util.GetValueOfString(from.Get_Value("EMailUserPW")));
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
                //        email = server.createEMail(GetCtx(), GetAD_Client_ID(),
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
        }	//	testEMail


      


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
            string toEMail = VAdvantage.Utility.Util.GetValueOfString(to.Get_Value("EMail"));
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
            return CreateEMail(from, VAdvantage.Utility.Util.GetValueOfString(to.Get_Value("GetEMail")), VAdvantage.Utility.Util.GetValueOfString(to.Get_Value("Name")), subject, message, isHTML);
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
                //        email = server.createEMail(GetCtx(), GetAD_Client_ID(),
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
                //        email = server.createEMail(GetCtx(), GetAD_Client_ID(),
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
                email = new EMail(this, VAdvantage.Utility.Util.GetValueOfString(from.Get_Value("EMail")), VAdvantage.Utility.Util.GetValueOfString(from.Get_Value("Name")), toEMail, toName,
                        subject, message);
                email.ISHTML = isHTML;
            }
            if (!email.IsValid())
                return null;
            if (IsSmtpAuthorization())
                email.CreateAuthenticator(VAdvantage.Utility.Util.GetValueOfString(from.Get_Value("EMailUser")), VAdvantage.Utility.Util.GetValueOfString(from.Get_Value("EMailUserPW")));
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
                //        email = server.createEMail(GetCtx(), GetAD_Client_ID(),
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

        public bool SendEMail(String toEMail, String toName, String subject, String message, FileInfo attachment, bool isHtml, int AD_Table_ID, int Record_ID, byte[] array = null, String fileName = "Rpt.pdf")
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

            FetchRecordsAttachment(email, AD_Table_ID, Record_ID);
            try
            {
                String msg = email.Send();
                if (EMail.SENT_OK.Equals(msg))
                {
                    SaveMailAttachment(true, AD_Table_ID, Record_ID, message, subject, toEMail, GetRequestEMail());
                    //log.info("Sent EMail " + subject + " to " + toEMail);
                    return true;
                }
                else
                {
                    SaveMailAttachment(false, AD_Table_ID, Record_ID, message, subject, toEMail, GetRequestEMail());
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

        // EmpCode : VIS0008
        // Change done to save Mail history
        /// <summary>
        /// Save mail history against the record ID
        /// </summary>
        /// <param name="success"></param>
        /// <param name="AD_Table_ID"></param>
        /// <param name="Record_ID"></param>
        /// <param name="message"></param>
        /// <param name="subject"></param>
        /// <param name="toEMail"></param>
        /// <param name="fromEMail"></param>
        private void SaveMailAttachment(bool success, int AD_Table_ID, int Record_ID, string message, string subject, string toEMail, string fromEMail)
        {
            if (!(AD_Table_ID > 0 && Record_ID > 0))
            {
                log.SaveError("VISPRO No Table  :: " + AD_Table_ID + ", " + Record_ID, "No Table No Record :: " + AD_Table_ID + ", " + Record_ID);
                return;
            }
            //written to send attachment details into mailattachment table
            MMailAttachment1 _mAttachment = new VAdvantage.Model.MMailAttachment1(GetCtx(), 0, null);
            _mAttachment.SetIsMailSent(success);
            _mAttachment.SetAD_Client_ID(GetCtx().GetAD_Client_ID());
            _mAttachment.SetAD_Org_ID(GetCtx().GetAD_Org_ID());
            _mAttachment.SetAD_Table_ID(AD_Table_ID);
            _mAttachment.IsActive();
            _mAttachment.SetAttachmentType("M");
            _mAttachment.SetRecord_ID(Util.GetValueOfInt(Record_ID));
            _mAttachment.SetTextMsg(message);
            _mAttachment.SetTitle(subject);
            _mAttachment.SetMailAddress(toEMail);
            _mAttachment.SetMailAddressFrom(fromEMail);
            if (_mAttachment.GetEntries().Length > 0)
            {
                _mAttachment.SetIsAttachment(true);
            }
            else
            {
                _mAttachment.SetIsAttachment(false);
            }
            if (!_mAttachment.Save())
            {
                ValueNamePair vnp = VLogger.RetrieveError();
                string msg = "";
                if (vnp != null)
                {
                    msg = vnp.GetName();
                    if (msg.Trim() == "")
                        msg = vnp.GetValue();
                }

                if (msg.Trim() == "")
                    msg = "Error in saving attachment History :  " + subject + " - " + message;

                log.SaveError(msg, subject + " - " + message);
            }
        }

        private void FetchRecordsAttachment(EMail email, int AD_Table_ID, int Record_ID)
        {
            MAttachment mAttach = MAttachment.Get(p_ctx, AD_Table_ID, Record_ID);
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

        /////////////////////////////////////


    }
}
