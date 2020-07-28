/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MStore
 * Purpose        : Web Store.
 * Class Used     : X_W_Store
 * Chronological    Development
 * Deepak           09-Nov-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MStore : X_W_Store
    {
        //	Logger						
        // private static VLogger _log = VLogger.GetVLogger(typeof(MAccount).FullName);
        private static VLogger _log = VLogger.GetVLogger(typeof(MStore).FullName);
        /// <summary>
        /// Get WStore from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="W_Store_ID">Id</param>
        /// <returns>Wstore</returns>
        public static MStore Get(Ctx ctx, int W_Store_ID)
        {
            int key = W_Store_ID;
            //Integer key = new Integer(W_Store_ID);
            //MStore retValue = (MStore) s_cache.get (key);
            MStore retValue = (MStore)s_cache[key];
            if (retValue != null)
            {
                return retValue;
            }
            retValue = new MStore(ctx, W_Store_ID, null);
            if (retValue.Get_ID() != 0)
            {
                //s_cache.put(key, retValue);
                s_cache.Add(key, retValue);
            }

            return retValue;
        }	//	get

        /// <summary>
        ///	Get WStore from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="contextPath">web server context path</param>
        /// <returns>WStore</returns>
        public static MStore Get(Ctx ctx, String contextPath)
        {
            MStore wstore = null;
            //Iterator<MStore> it = s_cache.Values().iterator();
            IEnumerator<MStore> it = s_cache.Values.GetEnumerator();
            while (it.MoveNext())
            {
                wstore = (MStore)it.Current;
                if (wstore.GetWebContext().Equals(contextPath))
                {
                    return wstore;
                }

            }

            //	Search by context
            //PreparedStatement pstmt = null;
            String sql = "SELECT * FROM W_Store WHERE WebContext=@Param1";
            SqlParameter[] Param = new SqlParameter[1];
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                Param[0] = new SqlParameter("@Param1", contextPath);
                idr = DataBase.DB.ExecuteReader(sql, Param, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {

                    wstore = new MStore(ctx, dr, null);
                }
                //pstmt = DataBase.prepareStatement (sql, null);
                //pstmt.setString(1, contextPath);
                //ResultSet rs = pstmt.executeQuery ();
                //if (rs.next ())
                //    wstore = new MStore (ctx, rs, null);
                //rs.close ();
                //pstmt.close ();
                //pstmt = null;
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);

            }
            finally
            {
                dt = null;
                if (idr != null)
                {
                    idr.Close();
                }
            }

            //	Try client
            if (wstore == null)
            {
                sql = "SELECT * FROM W_Store WHERE AD_Client_ID=@Param1 AND IsActive='Y' ORDER BY W_Store_ID";

                SqlParameter[] Param1 = new SqlParameter[1];
                DataTable dt1 = null;
                IDataReader idr1 = null;
                try
                {
                    Param[0] = new SqlParameter("@Param1", ctx.GetAD_Client_ID());
                    idr = DataBase.DB.ExecuteReader(sql, Param1, null);
                    dt = new DataTable();
                    dt.Load(idr1);
                    idr1.Close();
                    foreach (DataRow dr in dt1.Rows)
                    {
                        wstore = new MStore(ctx, dr, null);
                        _log.Warning("Ctx " + contextPath
                            + " Not found - Found via AD_Client_ID=" + ctx.GetAD_Client_ID());


                    }

                    //pstmt = DataBase.prepareStatement (sql, null);
                    //pstmt.setInt (1, ctx.getAD_Client_ID());
                    //ResultSet rs = pstmt.executeQuery ();
                    //if (rs.next ())
                    //{
                    //    wstore = new MStore (ctx, rs, null);
                    //    s_log.warning("Ctx " + contextPath 
                    //        + " Not found - Found via AD_Client_ID=" + ctx.getAD_Client_ID());
                    //}
                    //rs.close ();
                    //pstmt.close ();
                    //pstmt = null;
                }
                catch (Exception e)
                {
                    if (idr1 != null)
                    {
                        idr1.Close();
                    }
                    _log.Log(Level.SEVERE, sql, e);
                }
                finally
                {
                    dt = null;
                    if (idr1 != null)
                    {
                        idr1.Close();
                    }
                }
            }
            //	Nothing
            if (wstore == null)
            {
                return null;
            }
            //	Save
            //Integer key = new Integer(wstore.GetW_Store_ID());
            int key = wstore.GetW_Store_ID();
            //s_cache.put (key, wstore);

            s_cache.Add(key, wstore);

            return wstore;
        }	//	get

        /// <summary>
        ///	Get active Web Stores of Clieny
        /// </summary>
        /// <param name="client">client</param>
        /// <returns>array of web stores</returns>
        public static MStore[] GetOfClient(MClient client)
        {
            //ArrayList<MStore> list = new ArrayList<MStore>();
            List<MStore> list = new List<MStore>();
            String sql = "SELECT * FROM W_Store WHERE AD_Client_ID=@Param1 AND IsActive='Y'";

            SqlParameter[] Param = new SqlParameter[1];
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                Param[0] = new SqlParameter("@Param1", client.GetAD_Client_ID());
                idr = DataBase.DB.ExecuteReader(sql, Param, client.Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MStore(client.GetCtx(), dr, client.Get_TrxName()));
                }
            }
            //PreparedStatement pstmt = null;
            //try
            //{
            //    pstmt = DataBase.prepareStatement (sql, client.get_TrxName());
            //    pstmt.setInt (1, client.getAD_Client_ID());
            //    ResultSet rs = pstmt.executeQuery ();
            //    while (rs.next ())
            //        list.add (new MStore (client.getCtx(), rs, client.get_TrxName()));
            //    rs.close ();
            //    pstmt.close ();
            //    pstmt = null;
            //}
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
                if (idr != null)
                {
                    idr.Close();
                }
            }
            //
            MStore[] retValue = new MStore[list.Count];
            //list.toArray (retValue);
            retValue = list.ToArray();
            return retValue;
        }	//	getOfClient

        /// <summary>
        ///	Get Active Web Stores
        /// </summary>
        /// <returns>cached web stores</returns>
        public static MStore[] GetActive()
        {
            _log.Info("");
            //    try
            //{
            //    Collection<MStore> cc = s_cache.values();
            //    Object[] oo = cc.toArray();
            //    for (int i = 0; i < oo.length; i++)
            //        s_log.info(i + ": " + oo[i]);
            //    MStore[] retValue = new MStore[oo.length];
            //    for (int i = 0; i < oo.length; i++)
            //        retValue[i] = (MStore)oo[i];
            //    return retValue;
            //}
            try
            {

                IEnumerator<MStore> cc = s_cache.Values.GetEnumerator();
                List<MStore> listMStore = new List<MStore>();
                while (cc.MoveNext())
                {
                    listMStore.Add(cc.Current);
                }
                MStore[] retValue = listMStore.ToArray();
                for (int i = 0; i < retValue.Length; i++)
                {
                    _log.Info(i + ": " + retValue[i]);
                }
                return retValue;
            }
            catch (Exception e)
            {
                _log.Severe(e.ToString());
            }
            return new MStore[] { };
        }

        //	Cache						
        private static CCache<int, MStore> s_cache
            = new CCache<int, MStore>("W_Store", 2);
        /**	Logger	*/
        //private static CLogger	s_log	= CLogger.getCLogger (MStore.class);
        /**	Logger						*/
        // private static VLogger _log = VLogger.GetVLogger(typeof(MAccount).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="W_Store_ID">id</param>
        /// <param name="trxName">trx</param>
        public MStore(Ctx ctx, int W_Store_ID, Trx trxName)
            : base(ctx, W_Store_ID, trxName)
        {

            if (W_Store_ID == 0)
            {
                SetIsDefault(false);
                SetIsMenuAssets(true);	// Y
                SetIsMenuContact(true);	// Y
                SetIsMenuInterests(true);	// Y
                SetIsMenuInvoices(true);	// Y
                SetIsMenuOrders(true);	// Y
                SetIsMenuPayments(true);	// Y
                SetIsMenuRegistrations(true);	// Y
                SetIsMenuRequests(true);	// Y
                SetIsMenuRfQs(true);	// Y
                SetIsMenuShipments(true);	// Y

                //	setC_PaymentTerm_ID (0);
                //	setM_PriceList_ID (0);
                //	setM_Warehouse_ID (0);
                //	setName (null);
                //	setSalesRep_ID (0);
                //	setURL (null);
                //	setWebContext (null);
            }
        }	//	MWStore



        /// <summary>
        ///  Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">trx</param>
        public MStore(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

            //super (ctx, rs, trxName);
        }	//	MWStore

        //	The Messages						
        private MMailMsg[] m_msgs = null;
        /// <summary>
        ///	Get Web Ctx  
        /// </summary>
        /// <param name="full">if true fully qualified</param>
        /// <returns>web context</returns>
        public String GetWebContext(Boolean full)
        {
            if (!full)
            {
                //return super.getURL();
                return base.GetURL();
            }
            //String url = super.getURL();
            String url = base.GetURL();
            if (url == null || url.Length == 0)
                url = "http://localhost";
            if (url.EndsWith("/"))
                url += url.Substring(0, url.Length - 1);
            return url + GetWebContext();	//	starts with /
        }	//	getWebContext

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public String toString()
        {
            StringBuilder sb = new StringBuilder("WStore[");
            sb.Append(GetWebContext(true))
                .Append("]");
            return sb.ToString();
        }	//	toString


        /// <summary>
        ///	Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true if can be saved</returns>
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            //	Ctx to start with /
            if (!GetWebContext().StartsWith("/"))
            {
                SetWebContext("/" + GetWebContext());
            }
            //	Org to Warehouse
            if (newRecord || Is_ValueChanged("M_Warehouse_ID") || GetAD_Org_ID() == 0)
            {
                MWarehouse wh = new MWarehouse(GetCtx(), GetM_Warehouse_ID(), Get_TrxName());
                SetAD_Org_ID(wh.GetAD_Org_ID());
            }

            String url = GetURL();
            if (url == null)
            {
                url = "";
            }
            Boolean urlOK = url.StartsWith("http://") || url.StartsWith("https://");
            if (!urlOK) // || url.indexOf("localhost") != -1)
            {
                log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "URL")
                    + " - e.g. http://www.ViennaAdvantage.com");
                return false;
            }

            return true;
        }	//	beforeSave


        /// <summary>
        ///	Create EMail from Request User
        /// </summary>
        /// <param name="toEMail">recipient</param>
        /// <param name="toName">tomail</param>
        /// <param name="subject">subject</param>
        /// <param name="message">message</param>
        /// <returns>Email</returns>
        public EMail CreateEMail(String toEMail, String toName,
            String subject, String message)
        {
            if (toEMail == null || toEMail.Length == 0)
            {
                _log.Warning("No To");
                return null;
            }

            //
            EMail email = null;
            MClient client = MClient.Get(GetCtx(), GetAD_Client_ID());
            if (client.IsServerEMail() && Ini.IsClient())
            {
                //MessageBox.Show("Get Connection Problem");
                //Server server = CConnection.get().getServer();

                try
                {
                    //if (server != null)
                    if (!DataBase.DB.IsConnected())
                    {
                        email = CreateEMail(toEMail, toName, subject, message);
                    }

                    //{	//	See ServerBean
                    //    email = server.CreateEMail(GetCtx(), GetAD_Client_ID(),
                    //        toEMail, toName, subject, message);

                    //}
                    else
                    {
                        log.Log(Level.WARNING, "No AppsServer");
                    }
                }
                catch (Exception ex)
                {
                    log.Log(Level.SEVERE, GetName() + " - AppsServer error", ex);
                }
            }
            String from = GetWStoreEMail();
            if (from == null || from.Length == 0)
                from = client.GetRequestEMail();
            if (email == null)
                email = new EMail(client,
                       from, client.GetName(), toEMail, toName,
                       subject, message);
            //	Authorizetion
            if (client.IsSmtpAuthorization())
            {
                if (GetWStoreEMail() != null && GetWStoreUser() != null && GetWStoreUserPW() != null)
                {
                    email.CreateAuthenticator(GetWStoreUser(), GetWStoreUserPW());
                }
                else
                {
                    email.CreateAuthenticator(client.GetRequestUser(), client.GetRequestUserPW());
                }
            }
            //	Bcc
            email.AddBcc(from);
            //
            return email;
        }

        /// <summary>
        ///	Send EMail from WebStore User
        /// </summary>
        /// <param name="toEMail">recipient email address</param>
        /// <param name="toName">tomail</param>
        /// <param name="subject">subject</param>
        /// <param name="message">message</param>
        /// <returns>true if sent</returns>
        public Boolean SendEMail(String toEMail, String toName,
            String subject, String message)
        {
            if (message == null || message.Length == 0)
            {
                log.Warning("No Message");
                return false;
            }
            StringBuilder msgText = new StringBuilder();
            if (GetEMailHeader() != null)
            {
                msgText.Append(GetEMailHeader());
            }
            msgText.Append(message);
            if (GetEMailFooter() != null)
            {
                msgText.Append(GetEMailFooter());
            }
            //
            EMail email = CreateEMail(toEMail, toName, subject, msgText.ToString());
            if (email == null)
            {
                return false;
            }
            try
            {
                String msg = email.Send();
                if (EMail.SENT_OK.Equals(email.Send()))
                {
                    log.Info("Sent EMail " + subject + " to " + toEMail);
                    return true;
                }
                else
                {
                    log.Warning("Could NOT Send Email: " + subject
                        + " to " + toEMail + ": " + msg
                        + " (" + GetName() + ")");
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.Severe(GetName() + " - " + ex.Message);
                return false;
            }
        }	//	sendEMail

        /// <summary>
        ///	Test WebStore EMail
        /// </summary>
        /// <returns>OK or error</returns>
        public String TestEMail()
        {
            if (GetWStoreEMail() == null || GetWStoreEMail().Length == 0)
            {
                return "No Web Store EMail for " + GetName();
            }
            //
            EMail email = CreateEMail(GetWStoreEMail(), "WebStore",
                "Vienna WebStore EMail Test",
                "Vienna WebStore EMail Test: " + toString());
            if (email == null)
            {
                return "Could not create Web Store EMail: " + GetName();
            }
            try
            {
                String msg = email.Send();
                if (EMail.SENT_OK.Equals(email.Send()))
                {
                    log.Info("Sent Test EMail to " + GetWStoreEMail());
                    return "OK";
                }
                else
                {
                    log.Warning("Could NOT send Test Email to "
                        + GetWStoreEMail() + ": " + msg);
                    return msg;
                }
            }
            catch (Exception ex)
            {
                log.Severe(GetName() + " - " + ex.Message);
                return ex.Message;
            }
        }	//	testEMail

        /// <summary>
        ///	Get Messages
        /// </summary>
        /// <param name="reload">reload data</param>
        /// <returns>array of messages</returns>
        public MMailMsg[] GetMailMsgs(Boolean reload)
        {
            if (m_msgs != null && !reload)
            {
                return m_msgs;
            }
            //ArrayList<MMailMsg> list = new ArrayList<MMailMsg>();
            List<MMailMsg> list = new List<MMailMsg>();
            //
            String sql = "SELECT * FROM W_MailMsg WHERE W_Store_ID=@Param ORDER BY MailMsgType";

            SqlParameter[] Param = new SqlParameter[1];
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                Param[0] = new SqlParameter("@Param1", GetW_Store_ID());
                idr = DataBase.DB.ExecuteReader(sql, Param, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MMailMsg(GetCtx(), dr, Get_TrxName()));
                }
            }
            //PreparedStatement pstmt = null;
            //try
            //{
            //    pstmt = DataBase.prepareStatement (sql, get_TrxName());
            //    pstmt.setInt (1, getW_Store_ID());
            //    ResultSet rs = pstmt.executeQuery ();
            //    while (rs.next ())
            //        list.add (new MMailMsg (getCtx(), rs, get_TrxName()));
            //    rs.close ();
            //    pstmt.close ();
            //    pstmt = null;
            //}
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
                if (idr != null)
                {
                    idr.Close();
                }
            }
            //try
            //{
            //    if (pstmt != null)
            //        pstmt.close ();
            //    pstmt = null;
            //}
            //catch (Exception e)
            //{
            //    pstmt = null;
            //}
            //
            m_msgs = new MMailMsg[list.Count];
            //list.toArray (m_msgs);
            m_msgs = list.ToArray();
            return m_msgs;
        }	//	getMailMsgs

        /// <summary>
        /// 	Get Mail Msg and if not found create it
        /// </summary>
        /// <param name="MailMsgType">mail message type</param>
        /// <returns>Message</returns>
        public MMailMsg GetMailMsg(String MailMsgType)
        {
            if (m_msgs == null)
            {
                GetMailMsgs(false);
            }
            //	existing msg
            for (int i = 0; i < m_msgs.Length; i++)
            {
                if (m_msgs[i].GetMailMsgType().Equals(MailMsgType))
                {
                    return m_msgs[i];
                }
            }

            //	create missing
            if (CreateMessages() == 0)
            {
                log.Severe("Not created/found: " + MailMsgType);
                return null;
            }
            GetMailMsgs(true);
            //	try again
            for (int i = 0; i < m_msgs.Length; i++)
            {
                if (m_msgs[i].GetMailMsgType().Equals(MailMsgType))
                {
                    return m_msgs[i];
                }
            }

            //	nothing found
            log.Severe("Not found: " + MailMsgType);
            return null;
        }	//	getMailMsg


        /// <summary>
        /// Create (missing) Messages
        /// </summary>
        /// <returns>number of messages created</returns>
        public int CreateMessages()
        {
            String[][] initMsgs = new String[][]
		{
			new String[]{MMailMsg.MAILMSGTYPE_UserVerification,
				"EMail Verification", 
				"EMail Verification ",
				"Dear ", 
				"\nYou requested the Verification Code: ",
				"\nPlease enter the verification code to get access."},
			new String[]{MMailMsg.MAILMSGTYPE_UserPassword,
				"Password Request", 
				"Password Request ",
				"Dear ", 
				"\nWe received a 'Send Password' request from: ",
				"\nYour password is: "},
			new String[]{MMailMsg.MAILMSGTYPE_Subscribe,
				"Subscription New", 
				"New Subscription ",
				"Dear ", 
				"\nYou requested to be added to the list: ",
				"\nThanks for your interest."},
			new String[]{MMailMsg.MAILMSGTYPE_UnSubscribe,
				"Subscription Removed", 
				"Remove Subscription ",
				"Dear ", 
				"\nYou requested to be removed from the list: ",
				"\nSorry to see you go.  This is effictive immediately."},
			new String[]{MMailMsg.MAILMSGTYPE_OrderAcknowledgement,
				"Order Acknowledgement", 
				"Vienna Web - Order ",
				"Dear ", 
				"\nThank you for your purchase: ",
				"\nYou can view your Orders, Invoices, Payments in the Web Store."
				+ "\nFrom there, you also download your Assets (Documentation, etc.)"},
			new String[]{MMailMsg.MAILMSGTYPE_PaymentAcknowledgement,
				"Payment Success", 
				"Vienna Web - Payment ",
				"Dear ", 
				"\nThank you for your payment of ",
				"\nYou can view your Orders, Invoices, Payments in the Web Store."
				+ "\nFrom there you also download your Assets (Documentation, etc.)"},
			new String[]{MMailMsg.MAILMSGTYPE_PaymentError,
				"Payment Error", 
				"Vienna Web - Declined Payment ",
				"Dear ",
				"\nUnfortunately your payment was declined: ",
				"\nPlease check and try again. You can pay later by going to 'My Orders' or 'My Invoices' - or by directly creating a payment in 'My Payments'"},
			new String[]{MMailMsg.MAILMSGTYPE_Request,
				"Request", 
				"Request ",
				"Dear ",
				"\nThank you for your request: " + MRequest.SEPARATOR,
				MRequest.SEPARATOR + "\nPlease check back for updates."},
				
			new String[]{MMailMsg.MAILMSGTYPE_UserAccount,
				"Welcome Message", 
				"Welcome",
				"Welcome to our Web Store",
				"This is the Validation Code to access information:",
				""},
		};

            if (m_msgs == null)
            {
                GetMailMsgs(false);
            }
            if (m_msgs.Length == initMsgs.Length)	//	may create a problem if user defined own ones - unlikely
            {
                return 0;		//	nothing to do
            }
            int counter = 0;
            for (int i = 0; i < initMsgs.Length; i++)
            {
                Boolean found = false;
                for (int m = 0; m < m_msgs.Length; m++)
                {
                    if (initMsgs[i][0].Equals(m_msgs[m].GetMailMsgType()))
                    {
                        found = true;
                        break;
                    }
                }	//	for all existing msgs
                if (found)
                {
                    continue;
                }
                MMailMsg msg = new MMailMsg(this, initMsgs[i][0], initMsgs[i][1],
                    initMsgs[i][2], initMsgs[i][3], initMsgs[i][4], initMsgs[i][5]);
                if (msg.Save())
                {
                    counter++;
                }
                else
                {
                    log.Severe("Not created MailMsgType=" + initMsgs[i][0]);
                }
            }	//	for all initMsgs

            log.Info("#" + counter);
            m_msgs = null;		//	reset
            return counter;
        }	//	createMessages

    }

}
