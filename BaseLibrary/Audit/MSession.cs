using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using System.Data;
using System.Net;
using VAdvantage.Utility;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MSession : X_AD_Session
    {
        //Sessions			
        private static CCache<int, MSession> s_sessions = new CCache<int, MSession>("AD_Session_ID", 30);   //	no
        /**	Logger	*/
        private static VLogger s_log = VLogger.GetVLogger(typeof(MSession).FullName);

        //	get
        /**	Sessions					*/
        private static CCache<int, MSession> cache = Ini.IsClient()
            ? new CCache<int, MSession>("AD_Session_ID", 1, 0)		//	one client session 
            : new CCache<int, MSession>("AD_Session_ID", 30, 0);    //	no time-out	

        private static CCache<int, bool> roleChangeLog = new CCache<int, bool>("AD_Session_RoleLog", 10, 0);

        /* Do-not use CCache class :: cacahe list get clear at time cache reset process*/
        //private static Dictionary<int, MSession> cache = new Dictionary<int, MSession>(10);

        /// <summary>
        /// Get existing or create local session
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="createNew">create if not found</param>
        /// <returns>session</returns>
        public static MSession Get(Ctx ctx, Boolean createNew)
        {
            //int AD_Session_ID = ctx.GetContextAsInt("#AD_Session_ID");
            //MSession session = null;
            //if (AD_Session_ID > 0)
            //    session = cache[AD_Session_ID];

            //if (session == null && createNew)
            //{
            //    session = new MSession(ctx, null);	//	local session
            //    session.Save();
            //    AD_Session_ID = session.GetAD_Session_ID();
            //    ctx.SetContext("#AD_Session_ID", AD_Session_ID.ToString());
            //    cache.Add(AD_Session_ID, session);
            //}
            //return session;
            return Get(ctx, createNew, "");
        }	//	get

        /// <summary>
        /// Get existing or create local session
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="createNew">create if not found</param>
        /// <param name="requestAddr">Request address</param>
        /// <returns>session</returns>
        public static MSession Get(Ctx ctx, Boolean createNew, String requestAddr)
        {
            int AD_Session_ID = ctx.GetContextAsInt("#AD_Session_ID");
            MSession session = null;
            if (AD_Session_ID > 0)
                session = cache[AD_Session_ID];

            if (session == null && AD_Session_ID > 0)
            {
                // check from DB
                session = new MSession(ctx, AD_Session_ID, null);
                if (session.Get_ID() != AD_Session_ID)
                    session = null;
                else
                    cache.Add(AD_Session_ID, session);
            }

            if (session != null && session.IsProcessed())
            {
                s_log.Log(Level.WARNING, "Session Processed=" + session);

                cache.Remove(AD_Session_ID);
                session = null;
            }


            if (session == null && createNew)
            {
                session = new MSession(ctx, null);	//	local session
                if (!string.IsNullOrEmpty(requestAddr))
                {
                    session.SetRequest_Addr(requestAddr);
                }
                session.Save();
                AD_Session_ID = session.GetAD_Session_ID();
                ctx.SetContext("#AD_Session_ID", AD_Session_ID.ToString());
                cache.Add(AD_Session_ID, session);
            }

            if (session == null)
            {
                s_log.Fine("No Session");
            }

            return session;


        }


        /// <summary>
        /// Get existing or create remote session
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="Remote_Addr">remote address</param>
        /// <param name="Remote_Host">remote host</param>
        /// <param name="WebSession">web session</param>
        /// <returns>session</returns>
        public static MSession Get(Ctx ctx, String remoteAddr, String remoteHost, String WebSession)
        {
            int AD_Session_ID = ctx.GetContextAsInt("#AD_Session_ID");
            MSession session = null;
            if (AD_Session_ID > 0)
                session = cache[AD_Session_ID];
            if (session == null)
            {
                session = new MSession(ctx, remoteAddr, remoteHost, WebSession, null);	//	remote session
                session.Save();
                AD_Session_ID = session.GetAD_Session_ID();
                ctx.SetContext("#AD_Session_ID", AD_Session_ID.ToString());
                cache.Add(AD_Session_ID, session);
            }
            return session;
        }


        public static MSession Get(Ctx ctx, String SessionType, bool createNew, String Remote_Addr, String Remote_Host, String WebSession)
        {
            int AD_Session_ID = ctx.GetContextAsInt("#AD_Session_ID");
            MSession session = null;
            if (AD_Session_ID > 0)
            {
                session = s_sessions.Get(ctx, AD_Session_ID);
                if (session == null)
                    session = new MSession(ctx, AD_Session_ID, null);
                if (session.IsProcessed())
                {
                    s_log.Log(Level.WARNING, "Processed=" + session, new ArgumentException("Processed"));
                    s_sessions.Remove(AD_Session_ID);
                    return null;
                }
            }
            if (session == null)
            {
                if (createNew)
                {
                    session = new MSession(ctx, SessionType, Remote_Addr, Remote_Host, null);
                    session.Save();
                    AD_Session_ID = session.GetAD_Session_ID();
                    ctx.SetContext("#AD_Session_ID", AD_Session_ID);
                    cache.Add(AD_Session_ID, session);
                    if (s_sessions.ContainsKey(AD_Session_ID))
                        s_sessions[AD_Session_ID] = session;
                    else
                        s_sessions.Add(AD_Session_ID, session);
                }
                else
                    s_log.Warning("No Session!");
            }
            return session;
        }   //	get



        /// <summary>
        /// 	 * 	Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Session_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MSession(Ctx ctx, int AD_Session_ID, Trx trxName)
            : base(ctx, AD_Session_ID, trxName)
        {

            if (AD_Session_ID == 0)
            {
                SetProcessed(false);
                int AD_Role_ID = ctx.GetAD_Role_ID();
                SetAD_Role_ID(AD_Role_ID);
            }
        }	//	MSess

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MSession(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }	//	MSession

        /// <summary>
        /// New (remote) Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="Remote_Addr">remote address</param>
        /// <param name="Remote_Host">remote host</param>
        /// <param name="WebSession">web session</param>
        /// <param name="trxName">transaction</param>
        public MSession(Ctx ctx, String remoteAddr, String remoteHost, String webSession, Trx trxName)
            : this(ctx, 0, trxName)
        {

            if (remoteAddr != null)
                SetRemote_Addr(remoteAddr);
            if (remoteHost != null)
                SetRemote_Host(remoteHost);
            if (webSession != null)
                SetWebSession(webSession);
        }


        /// <summary>
        /// New (local) Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="trxName">transaction</param>
        public MSession(Ctx ctx, Trx trxName)
            : this(ctx, 0, trxName)
        {

            try
            {
                string hostName = "";

                hostName = ctx.GetContext("HostName");
                if (string.IsNullOrEmpty(hostName))
                {
                    hostName = System.Net.Dns.GetHostName();
                }
                SetRemote_Host(hostName);




                string cmpIP = "";

                cmpIP = ctx.GetContext("IPAddress");

                if (string.IsNullOrEmpty(cmpIP))
                {
                    System.Net.IPAddress[] ipA = System.Net.Dns.GetHostAddresses(hostName);
                    for (int i = 0; i < ipA.Length; i++)
                    {
                        cmpIP = ipA[i].ToString();
                    }

                }

                SetRemote_Addr(cmpIP);

                //InetAddress lh = InetAddress.getLocalHost();
                SetAD_Role_ID(ctx.GetAD_Role_ID());




            }
            catch (ApplicationException ex)
            {
                log.Log(Level.SEVERE, "No Local Host", ex);
            }
        }	//	MSession

        private Boolean _webStoreSession = false;

        public Boolean IsWebStoreSession()
        {
            return _webStoreSession;

        }

        public void SetWebStoreSession(Boolean webStoreSession)
        {
            _webStoreSession = webStoreSession;
        }	//	setWebStoreSession


        public void Logout()
        {
            SetProcessed(true);
            Save();
            cache.Remove(GetAD_Session_ID());
            //(GetAD_Session_ID());
            log.Info(TimeUtil.FormatElapsed(GetCreated(), GetUpdated()));
        }

        /// <summary>
        /// 	Create Change Log only if table is logged
        /// </summary>
        /// <param name="TrxName">transaction name</param>
        /// <param name="AD_ChangeLog_ID">0 for new change log</param>
        /// <param name="AD_Table_ID">table</param>
        /// <param name="AD_Column_ID">column</param>
        /// <param name="keyInfo">key value(s)</param>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="AD_Org_ID">org</param>
        /// <param name="OldValue">old</param>
        /// <param name="NewValue">new</param>
        /// <param name="tableName"></param>
        /// <param name="type"></param>
        /// <returns>change log or null</returns>
        public MChangeLog ChangeLog(Trx trx, int AD_ChangeLog_ID,
        int AD_Table_ID, int AD_Column_ID, Object keyInfo,
        int AD_Client_ID, int AD_Org_ID,
        Object oldValue, Object newValue,
        String tableName, String type)
        {
            //	Null handling
            if (oldValue == null && newValue == null)
                return null;
            //	Equal Value
            if (oldValue != null && newValue != null && oldValue.Equals(newValue))
                return null;

            //	No Log
            if (MChangeLog.IsNotLogged(AD_Table_ID, tableName, AD_Column_ID, type))
                return null;

            //	Role Logging
            //MRole role = MRole.GetDefault(GetCtx(), false);
            //	Do we need to log
            if (_webStoreSession						//	log if WebStore
                || MChangeLog.IsLogged(AD_Table_ID, type)		//	im/explicit log
                || (IsRoleChangeLog(GetCtx()))) //	Role Logging
            {; }
            else
            {
                return null;
            }
            //
            log.Finest("AD_ChangeLog_ID=" + AD_ChangeLog_ID
                    + ", AD_Session_ID=" + GetAD_Session_ID()
                    + ", AD_Table_ID=" + AD_Table_ID + ", AD_Column_ID=" + AD_Column_ID
                   + ": " + oldValue + " -> " + newValue);
            //Boolean success = false;

            try
            {
                String trxName = null;
                if (trx != null)
                    trxName = trx.GetTrxName();
                MChangeLog cl = new MChangeLog(GetCtx(),
                    AD_ChangeLog_ID, trxName, GetAD_Session_ID(),
                    AD_Table_ID, AD_Column_ID, keyInfo, AD_Client_ID, AD_Org_ID,
                    oldValue, newValue);
                if (cl.Save())
                    return cl;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "AD_ChangeLog_ID=" + AD_ChangeLog_ID
                    + ", AD_Session_ID=" + GetAD_Session_ID()
                    + ", AD_Table_ID=" + AD_Table_ID + ", AD_Column_ID=" + AD_Column_ID, e);
                return null;
            }
            log.Log(Level.SEVERE, "AD_ChangeLog_ID=" + AD_ChangeLog_ID
               + ", AD_Session_ID=" + GetAD_Session_ID()
               + ", AD_Table_ID=" + AD_Table_ID + ", AD_Column_ID=" + AD_Column_ID);
            return null;
        }

        private bool IsRoleChangeLog(Ctx ctx)
        {
            int AD_Role_ID = ctx.GetAD_Role_ID();
            bool isChangeLog = false;
            if (roleChangeLog.ContainsKey(AD_Role_ID))
                isChangeLog = roleChangeLog[AD_Role_ID];
            else
            {
                isChangeLog = Utility.Util.GetValueOfBool(
                    DB.ExecuteScalar("SELECT IsChangeLog FROM AD_Role WHERE AD_Role_ID = " + AD_Role_ID) == "Y");
                roleChangeLog[AD_Role_ID] = isChangeLog;
            }
            return isChangeLog;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AD_Client_ID"></param>
        /// <param name="AD_Org_ID"></param>
        /// <param name="AD_Table_ID"></param>
        /// <param name="whereClause"></param>
        /// <param name="recordCount"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public MQueryLog QueryLog(int AD_Client_ID, int AD_Org_ID,
        int AD_Table_ID, String whereClause, int recordCount, String parameter)
        {
            MQueryLog qlog = null;
            try
            {
                qlog = new MQueryLog(GetCtx(), GetAD_Session_ID(),
                    AD_Client_ID, AD_Org_ID,
                    AD_Table_ID, whereClause, recordCount, parameter);
                qlog.Save();
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "AD_Session_ID=" + GetAD_Session_ID()
                    + ", AD_Table_ID=" + AD_Table_ID + ", Where=" + whereClause
                   , e);
            }
            return qlog;
        }

        /// <summary>
        /// Log view/download action events
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Session_ID">sessioni id</param>
        /// <param name="AD_Client_ID"client id></param>
        /// <param name="AD_Org_ID">org id</param>
        /// <param name="action">menu action (window.proces etc)</param>
        /// <param name="actionType"type of action></param>
        /// <param name="actionOrigin">origin of action</param>
        /// <param name="desc">additional info</param>
        /// <param name="AD_Table_ID">table id</param>
        /// <param name="Record_ID">record id</param>
        /// <returns></returns>
        public MActionLog ActionLog(Ctx ctx, int AD_Session_ID,
    int AD_Client_ID, int AD_Org_ID,
    String actionOrigin, string actionType, String OriginName, string desc, int AD_Table_ID, int Record_ID = 0)
        {
           

            MActionLog alog = null;
            try
            {
                alog = new MActionLog(GetCtx(), GetAD_Session_ID(),
                    AD_Client_ID, AD_Org_ID, actionOrigin, actionType, OriginName, desc, AD_Table_ID, Record_ID);

                alog.Save();
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "AD_Session_ID=" + GetAD_Session_ID()
                    + ", AD_Table_ID=" + AD_Table_ID + ", actionOrigin=" + OriginName
                   , e);
            }
            return alog;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="AD_Client_ID"></param>
        /// <param name="AD_Org_ID"></param>
        /// <param name="AD_Table_ID"></param>
        /// <param name="whereClause"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public MQueryLog QueryLog(int AD_Client_ID, int AD_Org_ID,
        int AD_Table_ID, String whereClause, int recordCount)
        {
            return QueryLog(AD_Client_ID, AD_Org_ID, AD_Table_ID,
                whereClause, recordCount, (String)null);
        }	//

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AD_Client_ID"></param>
        /// <param name="AD_Org_ID"></param>
        /// <param name="AD_Table_ID"></param>
        /// <param name="whereClause"></param>
        /// <param name="recordCount"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public MQueryLog QueryLog(int AD_Client_ID, int AD_Org_ID,
        int AD_Table_ID, String whereClause, int recordCount, Object parameter)
        {
            String para = null;
            if (parameter != null)
                para = parameter.ToString();
            return QueryLog(AD_Client_ID, AD_Org_ID, AD_Table_ID,
                whereClause, recordCount, para);
        }	//

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AD_Client_ID"></param>
        /// <param name="AD_Org_ID"></param>
        /// <param name="AD_Table_ID"></param>
        /// <param name="whereClause"></param>
        /// <param name="recordCount"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public MQueryLog QueryLog(int AD_Client_ID, int AD_Org_ID,
        int AD_Table_ID, String whereClause, int recordCount, Object[] parameters)
        {
            String para = null;
            if (parameters != null && parameters.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (i > 0)
                        sb.Append(", ");
                    if (parameters[i] == null)
                        sb.Append("NULL");
                    else
                        sb.Append(parameters[i].ToString());
                }
                para = sb.ToString();
            }
            return QueryLog(AD_Client_ID, AD_Org_ID, AD_Table_ID,
                whereClause, recordCount, para);
        }	//	queryLog

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AD_Client_ID"></param>
        /// <param name="AD_Org_ID"></param>
        /// <param name="AD_Window_ID"></param>
        /// <param name="AD_Form_ID"></param>
        /// <returns></returns>
        public MWindowLog WindowLog(int AD_Client_ID, int AD_Org_ID,
        int AD_Window_ID, int AD_Form_ID)
        {
            MWindowLog wlog = null;
            try
            {
                wlog = new MWindowLog(GetCtx(), GetAD_Session_ID(),
                    AD_Client_ID, AD_Org_ID,
                    AD_Window_ID, AD_Form_ID);
                wlog.Save();
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "AD_Session_ID=" + GetAD_Session_ID()
                    + ", AD_Window_ID=" + AD_Window_ID + ", AD_Form_ID=" + AD_Form_ID
                    , e);
            }
            return wlog;
        }


        /// <summary>
        /// Retrieve existing session
        /// createNew create if not found
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns>session or null if session was ended or does not exist</returns>
        /// <witer>Raghu</witer>
        /// <date>08-March-2011</date>
        public static MSession Get(Ctx ctx)
        {

            return Get(ctx, false, "");


        }

        /// <summary>
        /// Is the information logged?
        /// </summary>
        /// <param name="AD_Table_ID"> table id</param>
        /// <param name="tableName">table name</param>
        /// <param name="type">change type</param>
        /// <returns> true if table is logged</returns>
        public Boolean IsLogged(int AD_Table_ID, String tableName, String type)
        {
            //	No Log
            if (MChangeLog.IsNotLogged(AD_Table_ID, tableName, 0, type))
                return false;
            //	Role Logging
           // MRole role = MRole.GetDefault(GetCtx(), false);
            //	Do we need to log
            if (IsWebStoreSession()						//	log if WebStore
                || MChangeLog.IsLogged(AD_Table_ID, type)		//	im/explicit log
                || IsRoleChangeLog(GetCtx()))	//	Role Logging
                return true;
            //
            return false;
        }


    }
}
