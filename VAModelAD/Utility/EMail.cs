/********************************************************
 * Module Name    : 
 * Purpose        : To send email to a client/user
 * Class Used     : 
 * Chronological Development
 * Veena Pandey     1-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Net.Configuration;
using System.Configuration;
using System.Runtime.Serialization;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.Process;
using System.Net.Mime;
using System.IO;
using System.Security;
using VAdvantage.MailBox;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
//using InterIMAP.Synchronous;
//using InterIMAP.Common.Interfaces;
using VAdvantage.Logging;
using VAdvantage.DataBase;
using System.Data;

namespace VAdvantage.Utility
{
    /// <summary>
    /// Class to send email
    /// </summary>
    [Serializable]
    public class EMail
    {
        #region "Private Variables"

        /**	From Address				*/
        private MailAddress _from;
        /** To Address					*/
        private List<MailAddress> _to;
        /** CC Addresses				*/
        private List<MailAddress> _cc;
        /** BCC Addresses				*/
        private List<MailAddress> _bcc;
        /**	Reply To Address			*/
        private MailAddress _replyTo;
        /**	Mail Subject				*/
        private String _subject;
        /** Mail Plain Message			*/
        private String _messageText;
        /** Mail HTML Message			*/
        private String _messageHTML;
        /**	Mail SMTP Server			*/
        private String _smtpHost;
        /** SMPT Port					*/
        private int _smtpPort = 25;
        /** SMPT TLS					*/
        private bool _isSmtpTLS = false;
        /**	Attachments					*/
        private List<Object> _attachments;
        /**	UserName and Password		*/
        private NetworkCredential _auth = null;
        /**	Message						*/
        private MailMessage _msg = null;
        /** Ctx - may be null		*/
        private Ctx _ctx;

        /**	Info Valid					*/
        private bool _valid = false;
        /** Send result Message			*/
        private String _sentMsg = null;

        /**	Mail Sent OK Status				*/
        public const String SENT_OK = "OK";
        /** Smtpconfing object*/
        private SMTPConfig _smtpConfig;

        /**	Logger							*/
        //protected static CLogger		log = CLogger.getCLogger (EMail.class);
        protected static VLogger log = VLogger.GetVLogger(typeof(EMail).FullName);

        //list of attachment
        private List<byte[]> _listData;
        private List<string> _listFile;
        private bool IsSendFromClient = false;


        #endregion

        public bool ISHTML
        {
            get;
            set;
        }
        /// <summary>
        /// Constructor to Create email object with checked Smtp Authentication
        /// </summary>
        /// <param name="client"></param>
        /// <param name="fromEMail"></param>
        /// <param name="fromName"></param>
        /// <param name="toEMail"></param>
        /// <param name="toName"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        public EMail(X_AD_Client client, String fromEMail, String fromName, String toEMail, String toName,
            String subject, String message)
            : this(client.GetCtx(), client.GetSmtpHost(), client.GetSmtpPort(), client.IsSmtpTLS(),
                fromEMail, fromName, toEMail, toName, subject, message)
        {
            if (client.IsSmtpAuthorization())
            {
                CreateAuthenticator(client.GetRequestUser(), client.GetRequestUserPW());
            }
        }

        /// <summary>
        /// Constructor to Create email object with checked Smtp Authentication
        /// </summary>
        /// <param name="client"></param>
        /// <param name="fromEMail"></param>
        /// <param name="fromName"></param>
        /// <param name="toEMail"></param>
        /// <param name="toName"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="isHtml"></param>
        public EMail(X_AD_Client client, String fromEMail, String fromName, String toEMail, String toName,
            String subject, String message, bool? isHtml)
            : this(client.GetCtx(), client.GetSmtpHost(), client.GetSmtpPort(), client.IsSmtpTLS(),
                fromEMail, fromName, toEMail, toName, subject, message, isHtml)
        {
            if (client.IsSmtpAuthorization())
            {
                CreateAuthenticator(client.GetRequestUser(), client.GetRequestUserPW());
            }
        }


        /// <summary>
        /// Constructor Constructor to Create email object without check Smtp Authentication
        /// 
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="smtpHost">The mail server</param>
        /// <param name="smtpPort">port</param>
        /// <param name="isSmtpTLS">TLS protocol</param>
        /// <param name="fromEMail">Sender's EMail address</param>
        /// <param name="fromName">Sender's name</param>
        /// <param name="toEMail">Recipient EMail address</param>
        /// <param name="toName">Recipient's name</param>
        /// <param name="subject">Subject of message</param>
        /// <param name="message">message</param>
        public EMail(Ctx ctx, String smtpHost, int smtpPort, bool isSmtpTLS, String fromEMail,
            String fromName, String toEMail, String toName, String subject, String message)
        {
            SetSmtpHost(smtpHost);
            if (smtpPort != 0)
                _smtpPort = smtpPort;
            _isSmtpTLS = isSmtpTLS;
            //_smtpConfig = new SMTPConfig();
            //_smtpConfig.Host = smtpHost;
            //_smtpConfig.UseSSL =isSmtpTLS;


            if (!SetFrom(fromEMail, fromName))
                return;

            //Split ToEmail 
            string[] emailList;
            string[] names;
            emailList = toEMail.Split(';');

            if (toName != null)
            {
                names = toName.Split(';');
            }
            else
            {
                toName = "";
                names = toName.Split(';');
            }


            //names = toName.Split(';');
            if (names.Length == emailList.Length)
            {
                for (int i = 0; i < emailList.Length; i++)
                    AddTo(emailList[i].Trim(), names[i]);
            }
            else
            {
                for (int i = 0; i < emailList.Length; i++)
                    AddTo(emailList[i].Trim(), null);
            }


            _ctx = ctx;
            if (subject == null || subject.Length == 0)
                SetSubject(".");	//	pass validation
            else
                SetSubject(subject);
            if (message != null && message.Length > 0)
                SetMessageText(message);
            _valid = IsValid(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="smtpHost"></param>
        /// <param name="smtpPort"></param>
        /// <param name="isSmtpTLS"></param>
        /// <param name="fromEMail"></param>
        /// <param name="fromName"></param>
        /// <param name="toEMail"></param>
        /// <param name="toName"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="isHtml"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="IsSmtpAuthorization"></param>
        public EMail(Ctx ctx, String smtpHost, int smtpPort, bool isSmtpTLS, String fromEMail,
            String fromName, String toEMail, String toName, String subject, String message,
            bool? isHtml, string username, string password, bool IsSmtpAuthorization)
            : this(ctx, smtpHost, smtpPort, isSmtpTLS, fromEMail,
           fromName, toEMail, toName, subject, message, isHtml)
        {
            if (IsSmtpAuthorization)
            {
                CreateAuthenticator(username, password);
            }
        }





        /// <summary>
        /// Constructor to Create email object without check Smtp Authentication
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="smtpHost">The mail server</param>
        /// <param name="smtpPort">port</param>
        /// <param name="isSmtpTLS">TLS protocol</param>
        /// <param name="fromEMail">Sender's EMail address</param>
        /// <param name="fromName">Sender's name</param>
        /// <param name="toEMail">Recipient EMail address</param>
        /// <param name="toName">Recipient's name</param>
        /// <param name="subject">Subject of message</param>
        /// <param name="message">message</param>
        public EMail(Ctx ctx, String smtpHost, int smtpPort, bool isSmtpTLS, String fromEMail,
            String fromName, String toEMail, String toName, String subject, String message, bool? isHtml)
        {
            SetSmtpHost(smtpHost);
            if (smtpPort != 0)
                _smtpPort = smtpPort;
            _isSmtpTLS = isSmtpTLS;
            if (!SetFrom(fromEMail, fromName))
                return;
            log.Log(Level.WARNING, "Step 1");
            string[] emailList;
            string[] names;
            emailList = toEMail.Split(';');
            names = toName.Split(';');
            for (int i = 0; i < emailList.Length; i++)
                AddTo(emailList[i].Trim(), names[i]);

            _ctx = ctx;
            if (subject == null || subject.Length == 0)
                SetSubject(".");	//	pass validation
            else
                SetSubject(subject);
            if (isHtml == true)
            {
                if (message != null && message.Length > 0)
                    SetMessageHTML(message);
            }
            else
                if (message != null && message.Length > 0)
                    SetMessageText(message);
            _valid = IsValid(true);
        }

        /// <summary>
        /// Create Email object
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="smtpConfig"></param>
        /// <param name="fromEMail"></param>
        /// <param name="fromName"></param>
        /// <param name="toEMail"></param>
        /// <param name="toName"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="isHTML"></param>
        public EMail(Ctx ctx, SMTPConfig smtpConfig, string fromEMail,
            string fromName, string toEMail, string toName, string subject, string message, bool isHTML)
        {

            _smtpConfig = new SMTPConfig();
            _smtpConfig = smtpConfig;
            string[] emailList;

            string[] names;


            SetSmtpHost(smtpConfig.Host);

            //if (smtpPort != 0)
            _smtpPort = smtpConfig.ServerPort;

            _isSmtpTLS = smtpConfig.UseSSL;

            if (!SetFrom(fromEMail, fromName))
                return;

            emailList = toEMail.Split(';');
            names = toName.Split(';');
            for (int i = 0; i < emailList.Length; i++)
                AddTo(emailList[i].Trim(), names[i]);





            _ctx = ctx;
            if (subject == null || subject.Length == 0)
                SetSubject(".");	//	pass validation
            else
                SetSubject(subject);
            if (message != null && message.Length > 0)
                if (isHTML == true)
                {
                    SetMessageHTML(message);
                }
                else
                {
                    SetMessageText(message);
                }
            _valid = IsValid(true);
        }


        /// <summary>
        /// Create email object without check Smtp Authentication
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="smtpHost"></param>
        /// <param name="smtpPort"></param>
        /// <param name="isSmtpTLS"></param>
        /// <param name="mailMsg"></param>
        /// <param name="listData"></param>
        /// <param name="listFile"></param>
        public EMail(Ctx ctx, String smtpHost, int smtpPort, bool isSmtpTLS, MailMessage mailMsg, List<byte[]> listData, List<string> listFile)
        {
            SetSmtpHost(smtpHost);
            if (smtpPort != 0)
            {
                _smtpPort = smtpPort;
            }
            _isSmtpTLS = isSmtpTLS;
            if (!SetFrom(mailMsg.From.ToString(), ""))
            {
                return;
            }
            AddTo(mailMsg.To.ToString(), "");
            _ctx = ctx;
            if (mailMsg.Subject == null || mailMsg.Subject.Length == 0)
            {
                SetSubject(".");	//	pass validation
            }
            else
            {
                SetSubject(mailMsg.Subject);
            }
            if (mailMsg.Body != null && mailMsg.Body.Length > 0)
                SetMessageText(mailMsg.Body);
            _valid = IsValid(true);
            _listData = listData;
            _listFile = listFile;
        }


        /// <summary>
        /// Get Send Result Msg
        /// </summary>
        /// <returns>msg</returns>
        public String GetSentMsg()
        {
            return _sentMsg;
        }

        /// <summary>
        /// Was sending the Msg OK
        /// </summary>
        /// <returns>msg == OK</returns>
        public bool IsSentOK()
        {
            return _sentMsg != null && SENT_OK.Equals(_sentMsg);
        }

        /// <summary>
        /// Dump Message Info
        /// </summary>
        private void DumpMessage()
        {
            if (_msg == null)
                return;
            try
            {
                //Enumeration<?> e = _msg.getAllHeaderLines ();
                //while (e.hasMoreElements())
                //    log.fine("- " + e.nextElement());

                //IEnumerator<string> ie = _msg.Headers.GetEnumerator();

                System.Collections.IEnumerator ie = _msg.Headers.GetEnumerator();
                ie.Reset();
                while (ie.MoveNext())
                {
                    //log.fine("- " + ie.Current);
                }
            }
            catch (Exception ex)
            {
                log.Log(Level.WARNING, _msg.ToString(), ex);
            }
        }

        /// <summary>
        /// Get the message directly
        /// </summary>
        /// <returns>mail message</returns>
        protected MailMessage GetMimeMessage()
        {
            return _msg;
        }

        /// <summary>
        /// Get Message ID or null
        /// </summary>
        /// <returns>Message ID e.g. "20030130004739.veenapandey@mail.viennasolutions.com"</returns>
        public String GetMessageID()
        {
            try
            {
                if (_msg != null)
                {
                    //return _msg.getMessageID(); // <11104210.1244008563656.JavaMail.veena.pandey@mail.viennasolutions.com> fromemail@host
                    return CommonFunctions.GenerateRandomNo() + "." + _msg.From.User + "@" + _smtpHost;
                }
            }
            catch (Exception ex) //(MessagingException ex)
            {
                log.Log(Level.SEVERE, "", ex);
            }
            return null;
        }


        //public EMailAuthenticator CreateAuthenticator(String username, String password)
        //{
        //    if (username == null || password == null)
        //    {
        //        //log.warning("Ignored - " + username + "/" + password);
        //        _auth = null;
        //    }
        //    else
        //    {
        //        //	log.fine("setEMailUser: " + username + "/" + password);
        //        _auth = new EMailAuthenticator(username, password);
        //    }
        //    return _auth;
        //}

        /// <summary>
        /// Create NetworkCredential for User
        /// </summary>
        /// <param name="username">user name</param>
        /// <param name="password">user password</param>
        /// <returns>NetworkCredential or null</returns>
        public NetworkCredential CreateAuthenticator(String username, String password)
        {
            if (username == null || password == null)
            {
                //log.warning("Ignored - " + username + "/" + password);
                _auth = null;
            }
            else
            {
                //	log.fine("setEMailUser: " + username + "/" + password);
                _auth = new NetworkCredential(username, password);
            }
            return _auth;
        }

        /// <summary>
        /// Get Sender
        /// </summary>
        /// <returns>Sender's internet address</returns>
        public MailAddress GetFrom()
        {
            return _from;
        }

        /// <summary>
        /// Set Sender
        /// </summary>
        /// <param name="newFromEMail">Sender's email address</param>
        /// <param name="newFromName">Sender's name</param>
        /// <returns>true if valid</returns>
        public bool SetFrom(String newFromEMail, String newFromName)
        {
            if (newFromEMail == null)
            {
                _valid = false;
                return _valid;
            }
            try
            {
                if (newFromName == null)
                    _from = new MailAddress(newFromEMail, null);
                else
                    _from = new MailAddress(newFromEMail, newFromName);
                _valid = true;
            }
            catch (Exception e)
            {
                log.Log(Level.WARNING, newFromEMail + ": " + e.ToString());
                _valid = false;
            }
            return _valid;
        }

        /// <summary>
        /// Add To Recipient
        /// </summary>
        /// <param name="newToEMail">Recipient's email address</param>
        /// <param name="newToName">Recipient's name (optional)</param>
        /// <returns>true if valid</returns>
        public bool AddTo(String newToEMail, String newToName)
        {
            if (newToEMail == null || newToEMail.Length == 0)
            {
                _valid = false;
                return _valid;
            }
            MailAddress ia = null;
            try
            {
                if (newToName == null)
                    ia = new MailAddress(newToEMail, null);
                else
                    ia = new MailAddress(newToEMail, newToName);
            }
            catch (Exception e)
            {
                log.Log(Level.WARNING, newToEMail + ": " + e.ToString());
                _valid = false;
                return _valid;
            }
            if (_to == null)
                _to = new List<MailAddress>();
            _to.Add(ia);
            return _valid;
        }

        /// <summary>
        /// Get Recipient
        /// </summary>
        /// <returns>Recipient's internet address</returns>
        public MailAddress GetTo()
        {
            if (_to == null || _to.Count == 0)
                return null;
            MailAddress ia = _to[0];
            return ia;
        }

        /// <summary>
        /// Get TO Recipients
        /// </summary>
        /// <returns>Recipient's internet address</returns>
        public MailAddress[] GetTos()
        {
            if (_to == null || _to.Count == 0)
                return null;
            MailAddress[] ias = new MailAddress[_to.Count];
            ias = _to.ToArray();
            return ias;

            //MailAddressCollection collec = new MailAddressCollection();
            //for (int i = 0; i < _to.Count; i++)
            //{
            //    collec.Add(_to[i]);
            //}
            //return collec;
        }

        /// <summary>
        /// Add CC Recipient
        /// </summary>
        /// <param name="newCc">EMail cc Recipient</param>
        /// <returns>true if valid</returns>
        public bool AddCc(String newCc)
        {
            if (newCc == null || newCc.Length == 0)
                return false;
            MailAddress ia = null;
            try
            {
                ia = new MailAddress(newCc, null);
            }
            catch (Exception e)
            {
                log.Log(Level.WARNING, newCc + ": " + e.ToString());
                return false;
            }
            if (_cc == null)
                _cc = new List<MailAddress>();
            _cc.Add(ia);
            return true;


        }
        /// <summary>
        /// Add To Recipient
        /// </summary>
        /// <param name="newToEMail">Recipient's email address</param>
        /// <param name="newToName">Recipient's name (optional)</param>
        /// <returns>true if valid</returns>
        public bool AddCc(String newCcEMail, String newCcName)
        {
            if (newCcEMail == null || newCcEMail.Length == 0)
            {
                _valid = false;
                return _valid;
            }
            MailAddress ia = null;
            try
            {
                if (newCcName == null)
                    ia = new MailAddress(newCcEMail, null);
                else
                    ia = new MailAddress(newCcEMail, newCcName);
            }
            catch (Exception e)
            {
                log.Log(Level.WARNING, newCcName + ": " + e.ToString());
                _valid = false;
                return _valid;
            }
           if (_cc == null)
                _cc = new List<MailAddress>();
            _cc.Add(ia);
            return _valid;
        }
        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>Recipient's internet address</returns>
        public MailAddress[] GetCcs()
        {
            if (_cc == null || _cc.Count == 0)
                return null;
            MailAddress[] ias = new MailAddress[_cc.Count];
            ias = _cc.ToArray();
            return ias;

            //MailAddressCollection collec = new MailAddressCollection();
            //for (int i = 0; i < _cc.Count; i++)
            //{
            //    collec.Add(_cc[i]);
            //}
            //return collec;
        }

        /// <summary>
        /// Add BCC Recipient
        /// </summary>
        /// <param name="newBcc">EMail bcc Recipien</param>
        /// <returns>true if valid</returns>
        public bool AddBcc(String newBcc)
        {
            if (newBcc == null || newBcc.Length == 0)
                return false;
            MailAddress ia = null;
            try
            {
                ia = new MailAddress(newBcc, null);
            }
            catch (Exception e)
            {
                log.Log(Level.WARNING, newBcc + ": " + e.Message);
                return false;
            }
            if (_bcc == null)
                _bcc = new List<MailAddress>();
            _bcc.Add(ia);
            return true;
        }
        /// <summary>
        /// Add To Recipient
        /// </summary>
        /// <param name="newToEMail">Recipient's email address</param>
        /// <param name="newToName">Recipient's name (optional)</param>
        /// <returns>true if valid</returns>
        public bool AddBcc(String newBccEMail, String newBccName)
        {
            if (newBccEMail == null || newBccEMail.Length == 0)
            {
                _valid = false;
                return _valid;
            }
            MailAddress ia = null;
            try
            {
                if (newBccName == null)
                    ia = new MailAddress(newBccEMail, null);
                else
                    ia = new MailAddress(newBccEMail, newBccName);
            }
            catch (Exception e)
            {
                log.Log(Level.WARNING, newBccName + ": " + e.ToString());
                _valid = false;
                return _valid;
            }
            if (_bcc == null)
                _bcc = new List<MailAddress>();
            _bcc.Add(ia);
            return _valid;
        }
        /// <summary>
        /// Get BCC Recipients
        /// </summary>
        /// <returns>Recipient's internet address</returns>
        public MailAddress[] GetBccs()
        {
            if (_bcc == null || _bcc.Count == 0)
                return null;
            MailAddress[] ias = new MailAddress[_bcc.Count];
            ias = _bcc.ToArray();
            return ias;

            //MailAddressCollection collec = new MailAddressCollection();
            //for (int i = 0; i < _bcc.Count; i++)
            //{
            //    collec.Add(_bcc[i]);
            //}
            //return collec;
        }

        /// <summary>
        /// Set Reply to Address
        /// </summary>
        /// <param name="newTo">email address</param>
        /// <returns>true if valid</returns>
        public bool SetReplyTo(String newTo)
        {
            if (newTo == null || newTo.Length == 0)
                return false;
            MailAddress ia = null;
            try
            {
                ia = new MailAddress(newTo, null);
            }
            catch (Exception e)
            {
                log.Log(Level.WARNING, newTo + ": " + e.ToString());
                return false;
            }
            _replyTo = ia;
            return true;
        }

        /// <summary>
        /// Get Reply To
        /// </summary>
        /// <returns>Reply To internet address</returns>
        public MailAddress GetReplyTo()
        {
            return _replyTo;
        }

        /// <summary>
        /// Set Subject
        /// </summary>
        /// <param name="newSubject">Subject</param>
        public void SetSubject(String newSubject)
        {
            if (newSubject == null || newSubject.Length == 0)
                _valid = false;
            else
                _subject = newSubject;
        }

        /// <summary>
        /// Get Subject
        /// </summary>
        /// <returns>subject</returns>
        public String GetSubject()
        {
            return _subject;
        }

        /// <summary>
        /// Set Message
        /// </summary>
        /// <param name="newMessage">message</param>
        public void SetMessageText(String newMessage)
        {
            if (newMessage == null || newMessage.Length == 0)
                _valid = false;
            else
            {
                _messageText = newMessage;
                if (!_messageText.EndsWith("\n"))
                    _messageText += "\n";
            }
        }

        /// <summary>
        /// Get MIME String Message - line ending with CRLF.
        /// </summary>
        /// <returns>message</returns>
        public String GetMessageCRLF()
        {
            if (_messageText == null)
                return "";
            char[] chars = _messageText.ToCharArray();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];
                if (c == '\n')
                {
                    int previous = i - 1;
                    if (previous >= 0 && chars[previous] == '\r')
                        sb.Append(c);
                    else
                        sb.Append("\r\n");
                }
                else
                    sb.Append(c);
            }
            //	log.fine("IN  " + _messageText);
            //	log.fine("OUT " + sb);

            return sb.ToString();
        }

        /// <summary>
        /// Set HTML Message
        /// </summary>
        /// <param name="html">message</param>
        public void SetMessageHTML(String html)
        {
            if (html == null || html.Length == 0)
                _valid = false;
            else
            {
                _messageHTML = html;
                if (!_messageHTML.EndsWith("\n"))
                    _messageHTML += "\n";
            }
        }

        /// <summary>
        /// Set HTML Message
        /// </summary>
        /// <param name="subject">subject repeated in message as H2</param>
        /// <param name="message">message</param>
        public void SetMessageHTML(String subject, String message)
        {
            _subject = subject;
            StringBuilder sb = new StringBuilder("<HTML>\n")
                .Append("<HEAD>\n")
                .Append("<TITLE>\n")
                .Append(subject + "\n")
                .Append("</TITLE>\n")
                .Append("</HEAD>\n");
            sb.Append("<BODY>\n")
                .Append("<H2>" + subject + "</H2>" + "\n")
                .Append(message)
                .Append("\n")
                .Append("</BODY>\n");
            sb.Append("</HTML>\n");
            _messageHTML = sb.ToString();
        }

        /// <summary>
        /// Get HTML Message
        /// </summary>
        /// <returns>message</returns>
        public String GetMessageHTML()
        {
            return _messageHTML;
        }

        /// <summary>
        /// Add file Attachment
        /// </summary>
        /// <param name="file">file to attach</param>
        public void AddAttachment(FileInfo file)
        {
            if (file == null)
                return;
            if (_attachments == null)
                _attachments = new List<Object>();
            _attachments.Add(file);
        }

        /// <summary>
        /// Add file Attachment
        /// </summary>
        /// <param name="file">stream</param>
        public void AddAttachment(Stream file)
        {
            if (file == null)
                return;
            if (_attachments == null)
                _attachments = new List<Object>();
            _attachments.Add(file);
        }


        /// <summary>
        /// Add file Attachment
        /// </summary>
        /// <param name="file">stream</param>
        //public void AddAttachment(IMAPMessageContent file)
        //{
        //    if (file == null)
        //        return;
        //    if (_attachments == null)
        //        _attachments = new List<Object>();
        //    _attachments.Add(file);
        //}
        /// <summary>
        /// Add file Attachment
        /// </summary>
        /// <param name="file">stream</param>
        //public void AddAttachment(IMessageContent file)
        //{
        //    if (file == null)
        //        return;
        //    if (_attachments == null)
        //        _attachments = new List<Object>();
        //    _attachments.Add(file);
        //}
        /// <summary>
        /// Add file Attachment
        /// </summary>
        /// <param name="filePath">path of the file</param>
        public void AddAttachment(String filePath)
        {
            if (filePath == null)
                return;
            if (_attachments == null)
                _attachments = new List<Object>();
            _attachments.Add(filePath);
        }

        /// <summary>
        /// Set SMTP Host or address
        /// </summary>
        /// <param name="newSmtpHost">Mail server</param>
        public void SetSmtpHost(String newSmtpHost)
        {
            if (newSmtpHost == null || newSmtpHost.Length == 0)
                _valid = false;
            else
                _smtpHost = newSmtpHost;
        }

        /// <summary>
        /// Get Mail Server name or address
        /// </summary>
        /// <returns>Mail Server</returns>
        public String GetSmtpHost()
        {
            return _smtpHost;
        }

        /// <summary>
        /// Is Info valid to send EMail
        /// </summary>
        /// <returns>true if email is valid and can be sent</returns>
        public bool IsValid()
        {
            return _valid;
        }

        /// <summary>
        /// Is Info valid to send EMail
        /// </summary>
        /// <param name="recheck">if true check main variables</param>
        /// <returns>true if email is valid and can be sent</returns>
        public bool IsValid(bool recheck)
        {
            if (!recheck)
                return _valid;

            //  From
            if (_from == null
                || _from.Address.Length == 0
                || _from.Address.IndexOf(' ') != -1)
            {
                //log.warning("From is invalid=" + _from);
                return false;
            }
            //	To
            MailAddress[] ias = GetTos();
            //MailAddressCollection ias = GetTos();
            if (ias == null)
            {
                ias = GetCcs();
                if (ias == null)
                {
                    ias = GetBccs();
                    if (ias == null)
                    {
                        return false;
                    }
                }
                //log.warning("No To");

            }
            for (int i = 0; i < ias.Length; i++)
            {
                if (ias[i] == null
                    || ias[i].Address.Length == 0
                    || ias[i].Address.IndexOf(' ') != -1)
                {
                    //log.warning("To(" + i + ") is invalid=" + ias[i]);
                    return false;
                }
            }

            //	Host
            if (_smtpHost == null || _smtpHost.Length == 0)
            {
                //log.warning("SMTP Host is invalid" + _smtpHost);
                return false;
            }

            //	Subject
            if (_subject == null || _subject.Length == 0)
            {
                //log.warning("Subject is invalid=" + _subject);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Set the message content
        /// </summary>
        private void SetContent()
        {
            Encoding encod = System.Text.Encoding.UTF8;

            _msg.Subject = GetSubject();
            _msg.SubjectEncoding = encod;

            //	First Part - Message
            _msg.Body = "";
            string bodyMsg = "";
            if (_messageHTML == null || _messageHTML.Length == 0)
            {
                //mbp_1.setText(getMessageCRLF(), charSetName);
                bodyMsg = GetMessageCRLF();

            }
            else
            {
                //mbp_1.setDataHandler(new DataHandler
                //    (new ByteArrayDataSource(_messageHTML, charSetName, "text/html")));
                bodyMsg = _messageHTML;
                _msg.IsBodyHtml = true;
            }

            _msg.Body = bodyMsg;
            _msg.BodyEncoding = encod;

            //log.fine("(multi) " + GetSubject());

            //	Attachments
            if (_attachments != null && _attachments.Count != 0)
            {
                //	for all attachments
                for (int i = 0; i < _attachments.Count; i++)
                {
                    Object attachment = _attachments[i];
                    Attachment data = null;
                    if (attachment.GetType() == typeof(FileInfo))
                    {
                        FileInfo file = (FileInfo)attachment;

                        bool th = File.Exists(file.FullName);

                        if (File.Exists(file.FullName))//(file.Exists)
                        {
                            data = new Attachment(file.FullName, MediaTypeNames.Application.Octet);

                            data.Name = file.Name;
                            // Add time stamp information for the file.


                            System.Globalization.CultureInfo original = System.Threading.Thread.CurrentThread.CurrentCulture;

                            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");


                            ContentDisposition disposition = data.ContentDisposition;
                            disposition.CreationDate = file.CreationTime;  //File.GetCreationTime(file.FullName);
                            disposition.ModificationDate = file.LastWriteTime; //File.GetLastWriteTime(file.FullName);
                            disposition.ReadDate = file.LastAccessTime; //File.GetLastAccessTime(file.FullName);

                            System.Threading.Thread.CurrentThread.CurrentCulture = original;
                            System.Threading.Thread.CurrentThread.CurrentUICulture = original;
                        }
                        else
                        {
                            //log.log(Level.WARNING, "File does not exist: " + file);
                            continue;
                        }
                    }
                    else if (attachment.GetType() == typeof(MemoryStream))
                    {
                        //File file = (File)attachment;
                        //if (file.exists())
                        //    ds = new FileDataSource (file);
                        //else
                        //{
                        //    //log.log(Level.WARNING, "File does not exist: " + file);
                        //    continue;
                        //}

                        Stream file = (Stream)attachment;
                        data = new Attachment(file, MediaTypeNames.Application.Octet);

                    }
                    //else if (attachment.GetType() == typeof(IMAPMessageContent))
                    //{
                    //    //File file = (File)attachment;
                    //    //if (file.exists())
                    //    //    ds = new FileDataSource (file);
                    //    //else
                    //    //{
                    //    //    //log.log(Level.WARNING, "File does not exist: " + file);
                    //    //    continue;
                    //    //}

                    //    IMAPMessageContent file = (IMAPMessageContent)attachment;
                    //    Stream s = new MemoryStream(file.BinaryData);

                    //    data = new Attachment(s, file.ContentFilename, file.ContentType);

                    //}
                    else if (attachment.GetType() == typeof(string))
                    {


                        String filePath = (String)attachment;
                        data = new Attachment(filePath, MediaTypeNames.Application.Octet);
                        // Add time stamp information for the file.

                        System.Globalization.CultureInfo original = System.Threading.Thread.CurrentThread.CurrentCulture;

                        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                        System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");


                        ContentDisposition disposition = data.ContentDisposition;
                        disposition.CreationDate = File.GetCreationTime(filePath);
                        disposition.ModificationDate = File.GetLastWriteTime(filePath);
                        disposition.ReadDate = File.GetLastAccessTime(filePath);


                        System.Threading.Thread.CurrentThread.CurrentCulture = original;
                        System.Threading.Thread.CurrentThread.CurrentUICulture = original;

                    }
                    //else if (attachment instanceof URL)
                    //{
                    //    URL url = (URL)attachment;
                    //    ds = new URLDataSource (url);
                    //}
                    //else if (attachment instanceof DataSource)
                    //    ds = (DataSource)attachment;
                    else
                    {
                        //log.log(Level.WARNING, "Attachement type unknown: " + attachment);
                        continue;
                    }

                    // Add the file attachment to this e-mail message.
                    _msg.Attachments.Add(data);
                }
            }
            if (_listData != null)
            {
                for (int i = 0; i < _listData.Count; i++)
                {
                    byte[] fileData = _listData[i];
                    string fileName = _listFile[i];
                    MemoryStream mStream = new MemoryStream(fileData);
                    _msg.Attachments.Add(new System.Net.Mail.Attachment(mStream, fileName));
                    // _msg.IsBodyHtml = true;
                }
            }

        }



        public String EmailFrom()
        {
            if (_from != null)
                return _from.ToString();

            return null;
        }

        /// <summary>
        /// To Send Mail
        /// </summary>
        /// <returns>OK or error message</returns>
        public string Send()
        {

            log.Log(Level.INFO, "Start Executing send Mail function");

            string configMessage= IsConfigurationExist(_ctx);
            if (configMessage != "OK")
            {
                return configMessage;
            }

            SmtpClient smtpClient = new SmtpClient();
            //smtpClient.
            smtpClient.Host = _smtpHost;
            smtpClient.Port = _smtpPort;
            smtpClient.Timeout = 200000;
            //if(_isSmtpTLS)
            smtpClient.EnableSsl = _isSmtpTLS;
            try
            {

                if (_smtpConfig != null)
                {
                    if (_smtpConfig.SmtpAuthentication)
                    {
                        smtpClient.Credentials = new System.Net.NetworkCredential(_smtpConfig.UserName, _smtpConfig.Password);
                        // smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    }
                    else
                    {
                        smtpClient.UseDefaultCredentials = true;

                    }
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                }
                else
                {
                    if (_auth != null)		//	createAuthenticator was called
                    {
                        smtpClient.Credentials = _auth;
                    }
                    else
                    {

                        //  smtpClient.Credentials = new System.Net.NetworkCredential("veena.pandey@viennasolutions.com", "veena");
                        // smtpClient.DeliveryMethod = mailSettings.Smtp.DeliveryMethod;
                        smtpClient.UseDefaultCredentials = true;
                    }
                }


            }
            catch (SecurityException se)
            {
                log.Log(Level.WARNING, "Auth=" + _auth + " - " + se.ToString());
                _sentMsg = se.ToString();
                return se.ToString();
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "Auth=" + _auth, e);
                _sentMsg = e.ToString();
                return e.ToString();
            }

            log.Info("(" + _smtpHost + ") " + _from + " -> " + _to);
            _sentMsg = null;
            //
            if (!IsValid(true))
            {
                _sentMsg = "Invalid Data";
                return _sentMsg;
            }

            try
            {
                //	_msg = new MimeMessage(session);
                _msg = new MailMessage();
                //	Addresses
                _msg.From = _from;

                MailAddress[] rec = GetTos();
                if (rec != null && rec.Length > 0)
                {
                    //    //_msg.setRecipient(Message.RecipientType.TO, rec[0]);
                    //    _msg.To.Add(rec[0]);
                    //}
                    //else
                    //{
                    //_msg.setRecipients(Message.RecipientType.TO, rec);
                    //_msg.To = rec;
                    foreach (MailAddress ma in rec)
                    {
                        _msg.To.Add(ma);
                    }
                }
                rec = GetCcs();
                if (rec != null && rec.Length > 0)
                {
                    //_msg.setRecipients(Message.RecipientType.CC, rec);
                    //_msg.CC = rec;
                    foreach (MailAddress ma in rec)
                    {
                        _msg.CC.Add(ma);
                    }
                }
                rec = GetBccs();
                if (rec != null && rec.Length > 0)
                {
                    //_msg.setRecipients(Message.RecipientType.BCC, rec);
                    //_msg.Bcc = rec;
                    foreach (MailAddress ma in rec)
                    {
                        _msg.Bcc.Add(ma);
                    }
                }
                if (_replyTo != null)
                {
                    //_msg.setReplyTo(new Address[] { _replyTo });
                    //_msg.ReplyTo = _replyTo;
                    _msg.ReplyToList.Add(_replyTo);
                }
                //
                //_msg.setSentDate(new java.util.Date());

                _msg.Headers.Add("Comments", "FrameworkMail");

                //_msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure | DeliveryNotificationOptions.OnSuccess;
                _msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                SetContent();
                log.Fine("message =" + _msg);

                ///////////
                if (ISHTML)
                {
                    _msg.IsBodyHtml = true;
                }
                //////////

                smtpClient.Send(_msg);
                log.Fine("Success - MessageID=" + GetMessageID());

                log.Log(Level.INFO, "End Executing send Mail function");

            }
            catch (SmtpException me)
            {
                _sentMsg = me.Message;
                return me.Message;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "", e);
                _sentMsg = e.Message;
                return e.Message;
            }
            finally
            {
                if (smtpClient != null)
                {
                    smtpClient.Dispose();
                }
            }


            //if (CLogMgt.isLevelFinest())
            //    DumpMessage();
            _sentMsg = SENT_OK;
            return _sentMsg;
        }

        /// <summary>
        /// Get Email and Name from a string
        /// </summary>
        /// <param name="stringToParse">String to be parse</param>
        /// <param name="resultEmailList">ref string resultEmailList</param>
        /// <param name="resultNameList">ref string resultNameList</param>
        /// <returns>true if list parse string contain all mail address valid</returns>
        public static bool ParseMailAddress(string stringToParse, ref string resultEmailList, ref string resultNameList, bool isShowMsg)
        {

            String strPattern = @"([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})";
            Regex regx = new Regex(strPattern);
            Match mc;
            resultEmailList = "";
            resultNameList = "";
            //names = toName.Split(';');
            //string Name;
            int pos;

            bool isValid = true;

            string[] emailList = stringToParse.Split(';');

            for (int i = 0; i < emailList.Length; i++)
            {
                if (emailList[i].Trim() != "")
                {
                    if (regx.IsMatch(emailList[i].Trim()))
                    {
                        mc = (regx.Match(emailList[i]));

                        if (resultEmailList != "")
                        {
                            resultEmailList += ";";
                            resultNameList += ";";
                        }
                        resultEmailList += mc.Value;

                        pos = emailList[i].ToString().IndexOf('<');


                        if (pos != -1)
                            resultNameList += emailList[i].ToString().Substring(0, pos).Replace("\"", "");



                    }
                    else
                    {
                        // if (isShowMsg)
                        //    ShowMessage.Error("NotRecogniseEmail", true, emailList[i].ToString());
                        isValid = false;
                        break;
                    }
                }
            }
            return isValid;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("EMail[")
                .Append(_smtpHost)
                .Append(":").Append(_smtpPort);
            if (_isSmtpTLS)
                sb.Append("(TLS)");
            sb.Append(",From:").Append(_from)
                .Append(",To:").Append(GetTo())
                .Append(",Subject=").Append(GetSubject())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Email Attachment
        /// </summary>
        /// <param name="file">attached File in byte</param>
        /// <param name="name">file name</param>
        public void AddAttachment(byte[] file, string name)
        {
            if (_listData == null)
            {
                _listData = new List<byte[]>();
            }
            if (_listFile == null)
            {
                _listFile = new List<string>();
            }
            if (file == null)
                return;
            _listData.Add(file);
            if (name == null || name.Trim().Length < 1)
            {
                name = "file-" + file.Length;
            }
            _listFile.Add(name);
        }



        /// <summary>
        /// Lakhwinder
        /// Get All Credentials from client if sendfromClient=true
        /// Get username and password from User mail configration
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="smtpHost"></param>
        /// <param name="smtpPort"></param>
        /// <param name="isSmtpTLS"></param>
        /// <param name="fromEMail"></param>
        /// <param name="fromName"></param>
        /// <param name="toEMail"></param>
        /// <param name="toName"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="isHtml"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="IsSmtpAuthorization"></param>
        public EMail(Ctx ctx, String fromEMail,
            String fromName, String toEMail, String toName, String subject, String message,
            bool? isHtml, bool sendFromClient)
            : this(ctx, null, 0, false, fromEMail,
           fromName, toEMail, toName, subject, message, isHtml)
        {
            this.IsSendFromClient = sendFromClient;
            string username = null;
            string password = null;
            string uName = null;   //By Sukhwinder on 3rd Jan, 2018, for getting FromName.
            int smtpport = 0;
            bool IsSmtpAuthorization = false;
            _ctx = ctx;
            int mailConfigID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_UserMailConfigration_ID FROM AD_UserMailConfigration WHERE IsActive='Y' AND AD_User_ID=" + ctx.GetAD_User_ID()));
            MUserMailConfigration userConfig = new MUserMailConfigration(ctx, mailConfigID, null);
            username = userConfig.GetSmtpUsername();
            password = userConfig.GetSmtpPassword();


            //By Sukhwinder on 3rd Jan, 2018, for getting FromName.
            MUser user1 = new MUser(ctx, userConfig.GetAD_User_ID(), null);
            uName = user1.GetName();
            //

            SetSmtpHost(userConfig.GetSmtpHost());
            _isSmtpTLS = userConfig.IsSmtpIsSsl();
            if (_isSmtpTLS)
            {
                _smtpPort = 587;
            }
            else
            {
                _smtpPort = 25;
            }
            IsSmtpAuthorization = userConfig.IsSmtpAuthorization();
            if (string.IsNullOrEmpty(fromEMail))
            {
                fromEMail = username;
                fromName = uName;   //username to Uname
            }
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(GetSmtpHost()) || sendFromClient)
            {
                X_AD_Client client = new X_AD_Client(ctx, ctx.GetAD_Client_ID(), null);
                SetSmtpHost(client.GetSmtpHost());
                smtpport = client.GetSmtpPort();
                if (smtpport != 0)
                {
                    _smtpPort = smtpport;
                }
                _isSmtpTLS = client.IsSmtpTLS();
                IsSmtpAuthorization = client.IsSmtpAuthorization();
                if (!sendFromClient)
                {
                    MUser user = new MUser(ctx, ctx.GetAD_User_ID(), null);
                    username = user.GetEMailUser();
                    password = user.GetEMailUserPW();
                    fromEMail = username;
                    fromName = user.GetName();
                }
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || sendFromClient)
                {
                    username = client.GetRequestUser();
                    password = client.GetRequestUserPW();
                    fromEMail = username;
                    fromName = client.GetName();   //username to client.GetName()
                }
            }


            if (!SetFrom(fromEMail, fromName))
                return;
            log.Log(Level.WARNING,"Step 2");
            if (IsSmtpAuthorization)
            {
                CreateAuthenticator(username, password);
            }
        }
        public void SetSMTPPort(int port)
        {
            _smtpPort = port;
        }


        public string IsConfigurationExist(Ctx ctx)
        {
            int mailConfigID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_UserMailConfigration_ID FROM AD_UserMailConfigration WHERE IsActive='Y' AND AD_User_ID=" + ctx.GetAD_User_ID()));
            MUserMailConfigration userConfig = new MUserMailConfigration(ctx, mailConfigID, null);
            string username = userConfig.GetSmtpUsername();
            string password = userConfig.GetSmtpPassword();
            string host = userConfig.GetSmtpHost();
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(host) || IsSendFromClient)
            {
                X_AD_Client client = new X_AD_Client(ctx, ctx.GetAD_Client_ID(), null);
                host = client.GetSmtpHost();
                int smtpport = client.GetSmtpPort();
                if (!IsSendFromClient)
                {
                    MUser user = new MUser(ctx, ctx.GetAD_User_ID(), null);
                    username = user.GetEMailUser();
                    password = user.GetEMailUserPW();
                }
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || IsSendFromClient)
                {
                    username = client.GetRequestUser();
                    password = client.GetRequestUserPW();
                }
            }

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(host))
            {
                return "ConfigurationIncompleteOrNotFound";
            }
            return "OK";
        }





    }


    public class MailConfigMethod
    {
        //static bool chk = false;
        static SMTPConfig config = new SMTPConfig();
        // private static VLogger _log = VLogger.GetVLogger(typeof(MailConfigMethod).FullName);
        public static SMTPConfig GetUSmtpConfig(int AD_User_ID, Ctx ctx)
        {
            //IMAPConfig localConfig = new IMAPConfig();


            MUserMailConfigration _userMailConfigration = null;

            IDataReader idr = null;

            string sql = "SELECT * FROM AD_USERMAILConfigration WHERE AD_USER_ID=" + AD_User_ID + " AND IsActive='Y'";


            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "AD_USERMAILConfigration", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);

            idr = DB.ExecuteReader(sql, null, null);

            //DataTable dt = new DataTable();
            //dt.Load(idr);


            if (idr.Read())
            {
                _userMailConfigration = new MUserMailConfigration(ctx, idr, null);
            }
            if (idr != null)
                idr.Close();
            //config = new IMAPConfig(System.Windows.Forms.Application.StartupPath + "\\" + DataBase.GlobalVariable.IMAP_CONFIG_FILE);
            if (_userMailConfigration != null)
            {
                if (string.IsNullOrEmpty(_userMailConfigration.GetSmtpHost()) || string.IsNullOrEmpty(_userMailConfigration.GetSmtpUsername()) || string.IsNullOrEmpty(_userMailConfigration.GetSmtpPassword()))
                {
                    //log.Info("Config info not foundM1");
                    return null;
                }

                config = new SMTPConfig(_userMailConfigration.GetSmtpHost(),
                                    _userMailConfigration.GetSmtpUsername(), _userMailConfigration.GetSmtpPassword(),
                                    _userMailConfigration.IsSmtpIsSsl(), _userMailConfigration.IsSmtpAuthorization(), false, "", _userMailConfigration.GetEMail());
            }
            else
            {
                // Classes.ShowMessage.Error("ConfigNotFound", true);
                //_log.Info("Config info not foundM2");
                return null;
            }
            //chk = true;
            // _log.Info("Config info found");
            return config;

            //return config;
        }

        public static SMTPConfig GetUMConfig(int AD_User_ID, Ctx ctx)
        {
            //IMAPConfig localConfig = new IMAPConfig(); //used for any saved on local pc 
            //IMAPConfig config = new IMAPConfig(); // user for user configuration
            MUserMailConfigration _userMailConfigration = null;

            try
            {

                string sql = "Select * from AD_USERMAILConfigration where AD_USER_ID=" + AD_User_ID + "and IsActive='Y'";
                IDataReader idr = DB.ExecuteReader(sql, null, null);
                if (idr.Read())
                {
                    _userMailConfigration = new MUserMailConfigration(ctx, idr, null);

                }
                if (idr != null)
                    idr.Close();
                //config = new IMAPConfig(System.Windows.Forms.Application.StartupPath + "\\" + DataBase.GlobalVariable.IMAP_CONFIG_FILE);
                if (_userMailConfigration != null)
                {
                    //config = new IMAPConfig(_userMailConfigration.GetImapHost(),
                    //                    _userMailConfigration.GetImapUsername(), _userMailConfigration.GetImapPassword(), _userMailConfigration.GetEMail(),
                    //                    _userMailConfigration.IsImapIsSsl(), _userMailConfigration.IsAutoLogin(), _userMailConfigration.IsAutoAttach(), _userMailConfigration.GetTableAttach(), "");
                    config.Host = _userMailConfigration.GetImapHost();
                    config.UserName = _userMailConfigration.GetImapUsername();
                    config.Password = _userMailConfigration.GetImapPassword();
                    config.UseSSL = _userMailConfigration.IsImapIsSsl();
                    config.AutoLogon = _userMailConfigration.IsAutoLogin();
                    config.Email = _userMailConfigration.GetEMail();
                    //config.URL = _userMailConfigration.GetURL();

                }
                else
                {
                    // Classes.ShowMessage.Error("ConfigNotFound", true);
                    return null;
                }
                return config;
            }
            catch
            {
                return null;
            }

            // Get the local saved Config file if any
            //localConfig = new IMAPConfig(configFile);


        }

        public static String GetURL(int AD_User_ID)
        {

            string sql = "Select URL from AD_Client where AD_Client_ID=" + AD_User_ID + "and IsActive='Y'";

            try
            {
                IDataReader idr = DB.ExecuteReader(sql, null, null);
                string url = "";
                if (idr.Read())
                {
                    url = idr[0].ToString();
                }
                if (idr != null)
                    idr.Close();

                return url;
            }
            catch
            {
                return "";
            }

        }

    }


    public class UserInformation
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public string Email { get; set; }
        public int HostPort { get; set; }
        public bool UseSSL { get; set; }
        public bool SmtpAuthentication { get; set; }
    }



}
