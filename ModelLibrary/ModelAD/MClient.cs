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

namespace VAdvantage.Model
{
    public class MClient : X_AD_Client
    {
        //Client Info					
        private MClientInfo _info = null;
        /**	Cache						*/
        private static CCache<int, MClient> s_cache = new CCache<int, MClient>("AD_Client", 3);

        private static VLogger s_log = VLogger.GetVLogger(typeof(MClient).FullName);

        public MClient(Ctx ctx, int AD_Client_ID, Trx trxName)
            : this(ctx, AD_Client_ID, false, trxName)
        {

        }



        public MClient(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        private bool m_createNew = false;

        public MClient(Ctx ctx, int AD_Client_ID, bool createNew, Trx trxName)
            : base(ctx, AD_Client_ID, trxName)
        {
            m_createNew = createNew;
            if (AD_Client_ID == 0)
            {
                if (m_createNew)
                {
                    //	setValue (null);
                    //	setName (null);
                    SetAD_Org_ID(0);
                    SetIsMultiLingualDocument(false);
                    SetIsSmtpAuthorization(false);
                    SetIsUseBetaFunctions(false);
                    SetIsServerEMail(false);
                    SetAD_Language(GlobalVariable.GetLanguageCode());
                    SetAutoArchive(AUTOARCHIVE_None);
                    SetMMPolicy(MMPOLICY_FiFo);	// F
                    SetIsPostImmediate(false);
                    SetIsCostImmediate(false);
                    SetSmtpPort(25);
                    SetIsSmtpTLS(false);
                }
                else
                    Load(Get_TrxName());
            }
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
        /// Get SMTP Host
        /// </summary>
        /// <returns>SMTP or loaclhost</returns>
        public new String GetSmtpHost()
        {
            //String s = super.getSmtpHost();
            String s = base.GetSmtpHost();
            if (s == null)
            {
                s = "localhost";
            }
            return s;
        }	//	getSMTPHost

        /**
	 *	Get SMTP Port
	 *	@return port (default 25)
	 */
        public new int GetSmtpPort()
        {
            int p = base.GetSmtpPort();
            if (p == 0)
                SetSmtpPort(25);
            return base.GetSmtpPort();
        }

        /// <summary>
        /// Get Primary Accounting Schema
        /// </summary>
        /// <returns>Acct Schema or null</returns>
        internal MAcctSchema GetAcctSchema()
        {
            if (_info == null)
                _info = MClientInfo.Get(GetCtx(), GetAD_Client_ID(), Get_TrxName());
            if (_info != null)
            {
                int C_AcctSchema_ID = _info.GetC_AcctSchema1_ID();
                if (C_AcctSchema_ID != 0)
                    return MAcctSchema.Get(GetCtx(), C_AcctSchema_ID);
            }
            return null;
        }

        /// <summary>
        /// Get Primary Accounting Schema ID
        /// </summary>
        /// <returns></returns>
        public int GetAcctSchemaID()
        {
            if (_info == null)
                _info = MClientInfo.Get(GetCtx(), GetAD_Client_ID(), Get_TrxName());
            if (_info != null)
            {
                int C_AcctSchema_ID = _info.GetC_AcctSchema1_ID();
                if (C_AcctSchema_ID != 0)
                    return C_AcctSchema_ID;
            }
            return 0;
        }

        /// <summary>
        /// Get all clients
        /// </summary>
        /// <param name="ctx">context</param>
        /// <returns>clients</returns>
        public static MClient[] GetAll(Ctx ctx)
        {
            List<MClient> list = new List<MClient>();
            String sql = "SELECT * FROM AD_Client";
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);

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
        /// Get Default Accounting Currency
        /// </summary>
        /// <returns>currency or 0</returns>
        public int GetC_Currency_ID()
        {
            if (_info == null)
                GetInfo();
            if (_info != null)
                return _info.GetC_Currency_ID();
            return 0;
        }	//	getC_Currency_ID
        /// <summary>
        ///Get optionally cached client
        /// </summary>
        /// <param name="ctx">context</param>
        /// <returns>client</returns>
        public static MClient Get(Ctx ctx)
        {
            return Get(ctx, ctx.GetAD_Client_ID());
        }
        /// <summary>
        ///Get client
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Client_ID">id</param>
        /// <returns>client</returns>
        public static MClient Get(Ctx ctx, int AD_Client_ID)
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
        /// <summary>
        ///Get Client Info
        /// </summary>
        /// <returns>Client Info</returns>
        public MClientInfo GetInfo()
        {
            if (_info == null)
                _info = MClientInfo.Get(GetCtx(), GetAD_Client_ID(), Get_Trx());
            return _info;
        }

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


        public new String GetAD_Language()
        {
            String s = base.GetAD_Language();
            if (s == null)
                return VAdvantage.Login.Language.GetBaseAD_Language();
            return s;
        }	//	getAD_Language


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
            string toEMail = Utility.Util.GetValueOfString(to.Get_Value("EMail"));
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
            return CreateEMail(from, Utility.Util.GetValueOfString(to.Get_Value("GetEMail")), Utility.Util.GetValueOfString(to.Get_Value("Name")), subject, message);
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
                email = new EMail(this, Utility.Util.GetValueOfString(from.Get_Value("EMail")), Utility.Util.GetValueOfString(from.Get_Value("Name")), toEMail, toName,
                        subject, message);
            if (!email.IsValid())
                return null;
            if (IsSmtpAuthorization())
                email.CreateAuthenticator(Utility.Util.GetValueOfString(from.Get_Value("EMailUser")), Utility.Util.GetValueOfString(from.Get_Value("EMailUserPW")));
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


        public System.Globalization.CultureInfo GetLocale()
        {
            VAdvantage.Login.Language lang = GetLanguage();
            if (lang != null)
                return lang.GetCulture();
            return lang.GetCulture("en_US");
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

        public bool SetupClientInfo(String language)
        {
            //	Create Trees
            String sql = null;
            if (Env.IsBaseLanguage(language, "AD_Ref_List"))	//	Get TreeTypes & Name
                sql = "SELECT Value, Name FROM AD_Ref_List WHERE AD_Reference_ID=120 AND IsActive='Y'";
            else
                sql = "SELECT l.Value, t.Name FROM AD_Ref_List l, AD_Ref_List_Trl t "
                    + "WHERE l.AD_Reference_ID=120 AND l.AD_Ref_List_ID=t.AD_Ref_List_ID AND l.IsActive='Y'";

            //  Tree IDs
            int AD_Tree_Org_ID = 0, AD_Tree_BPartner_ID = 0, AD_Tree_Project_ID = 0,
                AD_Tree_SalesRegion_ID = 0, AD_Tree_Product_ID = 0,
                AD_Tree_Campaign_ID = 0, AD_Tree_Activity_ID = 0;

            bool success = false;
            try
            {
                //IDataReader dr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                DataSet dr = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                MTree tree = null;
                for (int i = 0; i <= dr.Tables[0].Rows.Count - 1; i++)
                {

                    try
                    {
                        String treeType = dr.Tables[0].Rows[i]["Value"].ToString();
                        if (treeType.Equals(X_AD_Tree.TREETYPE_Other)
                            || treeType.StartsWith("U"))
                            continue;
                        String name = GetName() + " " + dr.Tables[0].Rows[i]["Name"].ToString();
                        //
                        if (treeType.Equals(X_AD_Tree.TREETYPE_Organization))
                        {
                            tree = new MTree(this, name, treeType);
                            success = tree.Save();
                            AD_Tree_Org_ID = tree.GetAD_Tree_ID();
                        }
                        else if (treeType.Equals(X_AD_Tree.TREETYPE_BPartner))
                        {
                            tree = new MTree(this, name, treeType);
                            success = tree.Save();
                            AD_Tree_BPartner_ID = tree.GetAD_Tree_ID();
                        }
                        else if (treeType.Equals(X_AD_Tree.TREETYPE_Project))
                        {
                            tree = new MTree(this, name, treeType);
                            success = tree.Save();
                            AD_Tree_Project_ID = tree.GetAD_Tree_ID();
                        }
                        else if (treeType.Equals(X_AD_Tree.TREETYPE_SalesRegion))
                        {
                            tree = new MTree(this, name, treeType);
                            success = tree.Save();
                            AD_Tree_SalesRegion_ID = tree.GetAD_Tree_ID();
                        }
                        else if (treeType.Equals(X_AD_Tree.TREETYPE_Product))
                        {
                            tree = new MTree(this, name, treeType);
                            success = tree.Save();
                            AD_Tree_Product_ID = tree.GetAD_Tree_ID();
                        }
                        else if (treeType.Equals(X_AD_Tree.TREETYPE_ElementValue))
                        {
                            //commented by lakhwinder 
                            //do not crete Account Tree While tenant Creation
                            //tree = new MTree(this, name, treeType);
                            //success = tree.Save();
                            //m_AD_Tree_Account_ID = tree.GetAD_Tree_ID();
                        }
                        else if (treeType.Equals(X_AD_Tree.TREETYPE_Campaign))
                        {
                            tree = new MTree(this, name, treeType);
                            success = tree.Save();
                            AD_Tree_Campaign_ID = tree.GetAD_Tree_ID();
                        }
                        else if (treeType.Equals(X_AD_Tree.TREETYPE_Activity))
                        {
                            tree = new MTree(this, name, treeType);
                            success = tree.Save();
                            AD_Tree_Activity_ID = tree.GetAD_Tree_ID();
                        }
                        else if (treeType.Equals(X_AD_Tree.TREETYPE_Menu))	//	No Menu
                            success = true;
                        else	//	PC (Product Category), BB (BOM)
                        {
                            tree = new MTree(this, name, treeType);
                            success = tree.Save();
                        }
                        if (!success)
                        {
                            log.Log(VAdvantage.Logging.Level.SEVERE, "Tree NOT created: " + name);
                            break;
                        }
                    }
                    catch { }
                }

                dr.Dispose();
            }
            catch (Exception e1)
            {
                log.Log(VAdvantage.Logging.Level.SEVERE, "Trees", e1);
                success = false;
            }

            if (!success)
                return false;

            //	Create ClientInfo
            MClientInfo clientInfo = new MClientInfo(this,
                AD_Tree_Org_ID, AD_Tree_BPartner_ID, AD_Tree_Project_ID,
                AD_Tree_SalesRegion_ID, AD_Tree_Product_ID,
                AD_Tree_Campaign_ID, AD_Tree_Activity_ID, Get_TrxName());
            success = clientInfo.Save();
            return success;
        }	//	createTrees

        /** Client Info Setup Tree for Account	*/
        private int m_AD_Tree_Account_ID;

        public int GetSetup_AD_Tree_Account_ID()
        {
            return m_AD_Tree_Account_ID;
        }	//	getSetup_AD_Tree_Account_ID


        /**
     //* 	Save
     //*	@return true if saved
     //*/
        public override bool Save()
        {
            if (Get_ID() == 0 && !m_createNew)
                return SaveUpdate();
            // check costing calculation, if costing not calculate then not able to change record
            if (Get_ID() != 0 && Is_ValueChanged("IsCostImmediate"))
            {
                bool result = checkAllCostCalculated(Get_ID());
                if (!result)
                {
                    log.SaveError("Warning", Msg.GetMsg(GetCtx(), "CostNotCalculateForAllTransaction"));
                    return result;
                }
            }
            return base.Save();
        }

        // check how many records in system whose costing not calculated based on client
        public bool checkAllCostCalculated(int client_ID)
        {
            bool result = true;
            string sql = null;
            int count = 0;
            try
            {
                // check columnname exist in table or not
                if (Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT count(*) FROM ad_column WHERE 
                    ad_table_id = ( SELECT ad_table_id FROM ad_table WHERE tablename LIKE 'M_Inventory' ) AND columnname LIKE 'IsCostCalculated'", null, null)) > 0)
                {
                    // check module exist or not
                    count = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VAMFG_' AND Isactive = 'Y' "));

                    // check how many records in system whose costing not calculated based on client
                    sql = @"SELECT SUM(record) FROM (
                            SELECT COUNT(*) AS record FROM m_Inventory WHERE AD_Client_ID = " + client_ID + @" AND IsActive = 'Y'
                            AND ((docstatus IN ('CO' , 'CL')  AND iscostcalculated = 'N')  OR (docstatus IN ('RE')  AND iscostcalculated = 'Y'
                            AND ISREVERSEDCOSTCALCULATED= 'N'  AND description LIKE '%{->%'))
                            UNION
                            SELECT COUNT(*) AS record FROM m_movement WHERE AD_Client_ID  = " + client_ID + @" AND isactive = 'Y'
                            AND ((docstatus IN ('CO' , 'CL') AND iscostcalculated = 'N') OR (docstatus IN ('RE') AND iscostcalculated = 'Y' 
                            AND ISREVERSEDCOSTCALCULATED= 'N' AND description LIKE '%{->%'))
                            UNION
                            SELECT COUNT(*) AS record FROM C_Invoice WHERE AD_Client_ID = " + client_ID + @" AND isactive = 'Y'
                            AND issotrx = 'N' AND isreturntrx = 'N'
                            AND ((docstatus IN ('CO' , 'CL') AND iscostcalculated = 'N') OR (docstatus IN ('RE') AND iscostcalculated = 'Y'
                            AND ISREVERSEDCOSTCALCULATED= 'N' AND description LIKE '%{->%'))
                            UNION
                            SELECT COUNT(*) AS record FROM m_inout  WHERE AD_Client_ID = " + client_ID + @" AND isactive = 'Y'
                            AND ((docstatus IN ('CO' , 'CL') AND iscostcalculated = 'N') OR (docstatus IN ('RE') AND iscostcalculated = 'Y'
                            AND ISREVERSEDCOSTCALCULATED= 'N' AND description LIKE '%{->%'))";
                    if (count > 0)
                    {
                        sql += @" UNION 
                                 SELECT COUNT(*) AS record FROM VAMFG_M_WrkOdrTransaction WHERE AD_Client_ID = " + client_ID + @" 
                                 AND VAMFG_WorkOrderTxnType IN ('CI', 'CR') AND  isactive = 'Y' AND
                                 ((docstatus IN ('CO' , 'CL') AND iscostcalculated = 'N') OR (docstatus IN ('RE') AND iscostcalculated = 'Y' 
                                 AND ISREVERSEDCOSTCALCULATED= 'N' AND VAMFG_description like '%{->%'))";
                    }
                    sql += ")";
                    count = 0;
                    count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
                    if (count == 0)
                        result = true;
                    else
                        result = false;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /**
     * 	String Representation
     *	@return info
     */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MClient[")
                .Append(Get_ID()).Append("-").Append(GetValue())
                .Append("]");
            return sb.ToString();
        }	//	toString



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
            string toEMail = Utility.Util.GetValueOfString(to.Get_Value("EMail"));
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
            return CreateEMail(from, Utility.Util.GetValueOfString(to.Get_Value("GetEMail")), Utility.Util.GetValueOfString(to.Get_Value("Name")), subject, message, isHTML);
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
                email = new EMail(this, Utility.Util.GetValueOfString(from.Get_Value("EMail")), Utility.Util.GetValueOfString(from.Get_Value("Name")), toEMail, toName,
                        subject, message);
                email.ISHTML = isHTML;
            }
            if (!email.IsValid())
                return null;
            if (IsSmtpAuthorization())
                email.CreateAuthenticator(Utility.Util.GetValueOfString(from.Get_Value("EMailUser")), Utility.Util.GetValueOfString(from.Get_Value("EMailUserPW")));
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
