﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.PushNotif
{
    /// <summary>
    /// Data Contract for SSE
    /// </summary>
    public class SSEData
    {
        /// <summary>
        /// Unique Or Valid Event/Action to handle
        /// </summary>
        public string Event { get; set; }

        /// <summary>
        /// Message Data Might be comma or json string
        /// </summary>
        public string Message { get; set; }


    }

   public class SSEManager
    {
        private static SSEManager _obj = null;
        private static object _lockObj = new object();
        private Task _task = null;
        private volatile Dictionary<int, List<SSEData>> _messageList = null;
        /// <summary>
        /// Get Single constructor
        /// </summary>
        /// <returns></returns>
        public static SSEManager Get()
        {
            if (_obj != null)
            {
                return _obj;
            }
            lock (_lockObj)
            {
                if (_obj == null)
                {
                    _obj = new SSEManager();
                }
            }
            return _obj;
        }

        private SSEManager()
        {
            _messageList = new Dictionary<int, List<SSEData>>(10);
        }

        public enum Cast
        {
            Unicast=0,
            BroadCast=1
        }

        public void AddMessage(int sessionid, string msg,string evt = "MSG", Cast type = Cast.Unicast)
        {
            var sData = new SSEData();
            sData.Event = evt;
            sData.Message = msg;
            AddMessage(sessionid, sData, type);
        }


        public  void  AddMessage(int sessionid, SSEData data, Cast type = Cast.Unicast)
        {
            if (type == Cast.BroadCast)
            {
                var sList = SessionManager.Get().GetSessionIds();
                lock (_lockObj)
                {
                    for (int i = 0, j = sList.Count; i < j; i++)
                    {
                        if (sessionid != sList[i])
                        {
                            if (!_messageList.ContainsKey(sList[i]))
                            {
                                _messageList[sList[i]] = new List<SSEData>();
                            }
                            _messageList[sList[i]].Add(data);
                        }
                    }
                }
            }
            else
            {
                lock (_lockObj)
                {
                    
                        if (!_messageList.ContainsKey(sessionid))
                        {
                            _messageList[sessionid] = new List<SSEData>();
                        }
                        _messageList[sessionid].Add(data);
                }
            }
        }

        //private void RemoveMessage(int sessionid)
        //{
        //    lock (_lockObj)
        //    {
        //        if (_messageList.ContainsKey(sessionid))
        //            _messageList.Remove(sessionid);
        //    }
        //}

        public  List<SSEData> GetMessages(int sessionid)
        {
            lock (_lockObj)
            {
                List<SSEData> lst = new List<SSEData>();
                if (_messageList.ContainsKey(sessionid))
                {
                    lst = _messageList[sessionid];
                    _messageList.Remove(sessionid);
                }
                return lst;
            }
        }
    }


    public class SessionData
    {
        public int UserId { get; set; }
         public string Name { get; set; }
    }
    public class SessionManager
    {
        private static SessionManager _obj = null;
        private static object _lockObj = new object();
        private Dictionary<int,SessionData> _sessionLst = null;

        /// <summary>
        /// Get Single constructor
        /// </summary>
        /// <returns></returns>
        public static SessionManager Get()
        {
            if (_obj != null)
            {
                return _obj;
            }
            lock (_lockObj)
            {
                if (_obj == null)
                {
                    _obj = new SessionManager();
                }
            }
            return _obj;
        }

        private SessionManager()
        {
            _sessionLst = new Dictionary<int, SessionData>();
        }

        public void AddSession(int sessionid, SessionData data)
        {
            lock (_lockObj)
            {
                if (!_sessionLst.ContainsKey(sessionid))
                {
                    _sessionLst.Add(sessionid, data);
                }
            }
            //Notify
            SSEManager.Get().AddMessage(sessionid, data.Name + " Logged in" , "LOGIN", SSEManager.Cast.BroadCast);
        }
        public void RemoveSession(int sessionid)
        {
            SessionData sData = null;
            lock (_lockObj)
            {
                if (_sessionLst.ContainsKey(sessionid))
                {
                    sData = _sessionLst[sessionid];
                    _sessionLst.Remove(sessionid);
                }
            }
            if (sData != null)
            {
                //Notify
                SSEManager.Get().AddMessage(sessionid, sData.Name +  " Logged off", "LOGOFF", SSEManager.Cast.BroadCast);
            }
        }

        public List<int> GetSessionIds()
        {
            return _sessionLst.Keys.ToList();
        }
    }
}
