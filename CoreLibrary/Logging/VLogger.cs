/********************************************************
 * Module Name    :     Log
 * Purpose        :     Maintain the Log (Instantiate the Logger class)
 * Author         :     Jagmohan Bhatt
 * Date           :     4-Aug-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using VAdvantage.Model;

namespace VAdvantage.Logging
{
    /// <summary>
    /// Vienna Logger
    /// </summary>
    [Serializable()]
    public class VLogger : Logger
    {
        private static readonly object _lock = new object();

        /// <summary>
        /// Get Logger
        /// </summary>
        /// <param name="type">class type</param>
        /// <returns>VLogger</returns>
        public static VLogger GetVLogger(Type type)
        {
            return GetVLogger(type.Name);
        }

        /// <summary>
        /// Get Logger (Sync)
        /// </summary>
        /// <param name="className">class full name</param>
        /// <returns>VLogger object</returns>
       // [MethodImpl(MethodImplOptions.Synchronized)]
        public static VLogger GetVLogger(string className)
        {
            lock (_lock)
            {
                LogManager manager = LogManager.GetLogManager();    //get the current logmanager object
                if (string.IsNullOrEmpty(className))
                    className = "";

                Logger result = manager.GetLogger(className);   //find if the logger already exist
                if (result != null && result is VLogger)    //if yes, return the object
                    return (VLogger)result;

                //other, we will have to create new
                VLogger newLogger = new VLogger(className);
                newLogger.SetLevel(VLogMgt.GetLevel());
                manager.AddLogger(newLogger);

                return newLogger;
            }
        }

        /**	Default Logger			*/
        private static VLogger _logger = null;

        public static VLogger Get()
        {
            if (_logger == null)
                _logger = GetVLogger("VAdvantage.default");
            return _logger;
        }	//	get


        [ThreadStatic]
        private static ValueNamePair _lastError = null;
        [ThreadStatic]
        private static Exception _lastException = null;
        [ThreadStatic]
        private static ValueNamePair _lastWarning = null;
        [ThreadStatic]
        private static ValueNamePair _lastInfo = null;
        [ThreadStatic]
        private static ValueNamePair _lastAdvDocError = null;

        /// <summary>
        /// Save Error
        /// </summary>
        /// <param name="VAF_Msg_Lable">AD Message</param>
        /// <param name="message">message</param>
        /// <param name="issueError">true, if error is to be issued in log</param>
        /// <returns></returns>
        public bool SaveError(String VAF_Msg_Lable, String message, bool issueError, bool isAdvDocNoError)
        {
            _lastAdvDocError = new ValueNamePair(VAF_Msg_Lable, message);
            //  print it
            if (issueError)
                Warning(VAF_Msg_Lable + " - " + message);
            return true;
        }   //  saveError

        /// <summary>
        /// Retrieve Advance Document Error
        /// </summary>
        /// <returns>Error in ValueNamePair object</returns>
        public static ValueNamePair RetrieveAdvDocNoError()
        {
            ValueNamePair vp = _lastAdvDocError;
            _lastAdvDocError = null;
            return vp;
        }   //  retrieveError

        /// <summary>
        /// Save Error
        /// </summary>
        /// <param name="VAF_Msg_Lable">AD Message</param>
        /// <param name="message">message</param>
        /// <returns>true, if saved</returns>
        public bool SaveError(String VAF_Msg_Lable, String message)
        {
            return SaveError(VAF_Msg_Lable, message, true);
        }   //  saveError

        /// <summary>
        /// Save Error
        /// </summary>
        /// <param name="VAF_Msg_Lable">AD Message</param>
        /// <param name="ex">exception object</param>
        /// <returns></returns>
        public bool SaveError(String VAF_Msg_Lable, Exception ex)
        {
            _lastException = ex;
            return SaveError(VAF_Msg_Lable, ex.Message, true);
        }   //  saveError

        /// <summary>
        /// Save Error
        /// </summary>
        /// <param name="VAF_Msg_Lable">AD Message</param>
        /// <param name="message">message</param>
        /// <param name="issueError">true, if error is to be issued in log</param>
        /// <returns></returns>
        public bool SaveError(String VAF_Msg_Lable, String message, bool issueError)
        {
            _lastError = new ValueNamePair(VAF_Msg_Lable, message);
            //  print it
            if (issueError)
                Warning(VAF_Msg_Lable + " - " + message);
            return true;
        }   //  saveError

        /// <summary>
        /// Retrieve Error
        /// </summary>
        /// <returns>Error in ValueNamePair object</returns>
        public static ValueNamePair RetrieveError()
        {
            ValueNamePair vp = _lastError;
            _lastError = null;
            return vp;
        }   //  retrieveError


        /// <summary>
        /// Retrieve Exception
        /// </summary>
        /// <returns>Exception in ValueNamePair object</returns>
        public static Exception RetrieveException()
        {
            Exception ex = _lastException;
            _lastException = null;
            return ex;
        }   //  retrieveError

        /// <summary>
        /// Save Warning
        /// </summary>
        /// <param name="VAF_Msg_Lable">AD Message</param>
        /// <param name="message">message to be logged</param>
        /// <returns>true, if saved</returns>
        public bool SaveWarning(String VAF_Msg_Lable, String message)
        {
            _lastWarning = new ValueNamePair(VAF_Msg_Lable, message);
            //  print it
            if (true) //	issueError
                Warning(VAF_Msg_Lable + " - " + message);
            return true;
        }   //  saveWarning

        /// <summary>
        /// Retrieve Warning
        /// </summary>
        /// <returns>Warning in ValueNamePair object</returns>
        public static ValueNamePair RetrieveWarning()
        {
            ValueNamePair vp = _lastWarning;
            _lastWarning = null;
            return vp;
        }   //  retrieveWarning

        /// <summary>
        /// Save Info
        /// </summary>
        /// <param name="VAF_Msg_Lable">AD Message</param>
        /// <param name="message">message to be logged</param>
        /// <returns>true, if saved</returns>
        public bool SaveInfo(String VAF_Msg_Lable, String message)
        {
            _lastInfo = new ValueNamePair(VAF_Msg_Lable, message);
            return true;
        }   //  saveInfo

        /// <summary>
        /// Retrieve Info
        /// </summary>
        /// <returns>Info in ValueNamePair object</returns>
        public static ValueNamePair RetrieveInfo()
        {
            ValueNamePair vp = _lastInfo;
            _lastInfo = null;
            return vp;
        }   //  retrieveInfo


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="className">Name of the class</param>
        public VLogger(string className)
            : base(className)
        {
        }

        /// <summary>
        /// Reset the saved last errors
        /// </summary>
        public static void ResetLast()
        {
            _lastError = null;
            _lastException = null;
            _lastWarning = null;
            _lastInfo = null;
        }	//	resetLast

    }
}
