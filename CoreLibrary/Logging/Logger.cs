/********************************************************
 * Module Name    :     Log
 * Purpose        :     Maintain the Log (Helps creating instance of log class)
 * Author         :     Jagmohan Bhatt
 * Date           :     5-Aug-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace VAdvantage.Logging
{
    /// <summary>
    /// A Logger object is used to log messages for a specific system or application component.
    /// </summary>
    public class Logger
    {
        private volatile int levelValue; //initial level value
        private string name;    //name of the logger
        private Level levelObject;  //level object
        private List<Handler> handlers; //diffrent handlers (VLogFile)
        // private ArrayList kids;
        private LogManager manager = LogManager.GetLogManager();    //initiate the logmanager class
        private Logger parent;    // our nearest parent.
        private static int offValue = Level.OFF.IntValue();
        private Filter filter;

        private static readonly object _lock = new object();

        /// <summary>
        /// Set Filter
        /// </summary>
        /// <param name="newFilter"></param>
        public void SetFilter(Filter newFilter)
        {
            filter = newFilter;
        }

        /// <summary>
        /// Get Filter
        /// </summary>
        /// <returns></returns>
        public Filter GetFilter()
        {
            return filter;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">name of the logger</param>
        internal Logger(string name)
        {
            this.name = name;   //set the name
            this.levelValue = Level.INFO.IntValue();    //set the level value
        }

        /// <summary>
        /// Log a LogRecord.
        /// <para>
        /// All the other logging methods in this class call through
        /// this method to actually perform any Logging.  Subclasses can
        /// override this single method to capture all log activity.
        /// </para>
        /// </summary>
        /// <param name="record"></param>
        /// 
        //[MethodImpl(MethodImplOptions.Synchronized)]
        public void Log(LogRecord record)
        {
            lock (_lock)
            {
                if (record.GetLevel().IntValue() < levelValue || levelValue == offValue)
                    return;

                //lock (this)
                // {
                if (filter != null && !filter.IsLoggable(record))
                {
                    return;
                }
                //}

                // Post the LogRecord to all our Handlers, and then to
                // our parents' handlers, all the way up the tree.

                Logger logger = this;
                while (logger != null)
                {
                    Handler[] targets = logger.GetHandlers();
                    if (targets != null)
                        for (int i = 0; i < targets.Length; i++)
                        {
                            //VLogFile.Publish (abstract method)
                            targets[i].Publish(record);
                        }

                    logger = logger.GetParent();
                }
            }
        }

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="level">level to be logged</param>
        /// <param name="msg">msg to be logged</param>
        public void Log(Level level, string msg)
        {
            ////check if msg is eligible to be logged or not
            //if (level.IntValue() < levelValue || levelValue == Level.OFF.IntValue())
            //    return;

            //LogRecord lr = new LogRecord(level, msg);   //set the values into logrecord class
            //DoLog(lr);  //do log

            //check if msg is eligible to be logged or not
            if (level.IntValue() < levelValue || levelValue == Level.OFF.IntValue())
                return;

            LogRecord lr = new LogRecord(level, msg);   //set the values into logrecord class
            DoLog(lr);  //do log

        }

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="level">level to be logged</param>
        /// <param name="msg">msg to be logged</param>
        /// <param name="obj">Not IMplemented</param>
        public void Log(Level level, string msg, object obj)
        {
            Log(level, msg);
        }

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="level">level to be logged</param>
        /// <param name="msg">msg to be logged</param>
        /// <param name="e">Exception</param>
        public void Log(Level level, string msg, Exception e)
        {
            Log(level, msg + " => " + e.Message);
        }

        /// <summary>
        /// Get the parent object of the current logger
        /// </summary>
        /// <returns>Logger</returns>
        public Logger GetParent()
        {
            return parent;
        }

        /// <summary>
        /// empty handler in case of null
        /// </summary>
        private static Handler[] emptyHandlers = new Handler[0];
        private bool anonymous; //not in use as of now

        /// <summary>
        /// Get anonymous user if userid is not found
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static Logger GetAnonymousLogger()
        {
            //create the instance of log manager
            LogManager manager = LogManager.GetLogManager();
            Logger result = new Logger(null);   //instantiate the logger with null to make a parent object
            result.SetAnonymous(true);
            Logger root = manager.GetLogger("");    //blank parent logger
            result.DoSetParent(root);
            return result;
        }

        public void SetAnonymous(bool val)
        {
            anonymous = val;
        }

        /// <summary>
        /// Gets the handler
        /// </summary>
        /// <returns>Handler</returns>
        //[MethodImpl(MethodImplOptions.Synchronized)]
        public Handler[] GetHandlers()
        {
            lock (_lock)
            {
                if (handlers == null)
                {
                    //if handler is null, set the empty handler
                    return emptyHandlers;
                }
                //process the handler
                Handler[] result = new Handler[handlers.Count];
                result = (Handler[])handlers.ToArray();
                return result;
            }
        }

        /// <summary>
        /// Final Logging (Main Method)
        /// </summary>
        /// <param name="lr">LogRecord</param>
        private void DoLog(LogRecord lr)
        {
            lr.SetLoggerName(name);
            Log(lr);
        }

        /// <summary>
        /// Get the name of the VLogger
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return name;
        }

        /// <summary>
        /// Set the log level specifying which message levels will be logged by this logger.
        /// </summary>
        /// <param name="level"></param>
        public void SetLevel(Level level)
        {
            levelObject = level;
            UpdateEffectiveLevel();
        }

        /// <summary>
        /// Get Level
        /// </summary>
        /// <returns></returns>
        public Level GetLevel()
        {
            return levelObject;
        }

        /// <summary>
        /// Add the output handler
        /// </summary>
        /// <param name="handler">name of the handler</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddHandler(Handler handler)
        {

            if (handlers == null)
                handlers = new List<Handler>();

            handlers.Add(handler);
        }

        /// <summary>
        /// Removes the handler
        /// </summary>
        /// <param name="handler">Handler</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RemoveHandler(Handler handler)
        {
            if (handler == null)
            {
                return;
            }
            if (handlers == null)
            {
                return;
            }
            handlers.Remove(handler);
        }

        #region "Level Functions"

        /// <summary>
        /// Log a Fine message.
        /// <para>
        /// If the logger is currently enabled for the Fine message
        /// level then the given message is forwarded to all the
        /// registered output Handler objects.  
        /// </para>
        /// </summary>
        /// <param name="msg">The string message (or a key in the message catalog)</param>
        public void Fine(string msg)
        {
            if (Level.FINE.IntValue() < levelValue)
            {
                return;
            }

            Log(Level.FINE, msg);

        }

        /// <summary>
        /// Log a CONFIG message.
        /// <para>
        /// If the logger is currently enabled for the CONFIG message
        /// level then the given message is forwarded to all the
        /// registered output Handler objects.  
        /// </para>
        /// </summary>
        /// <param name="msg">The string message (or a key in the message catalog)</param>
        public void Config(String msg)
        {
            if (Level.CONFIG.IntValue() < levelValue)
            {
                return;
            }
            Log(Level.CONFIG, msg);
        }

        /// <summary>
        /// Log a FINER message.
        /// <para>
        /// If the logger is currently enabled for the FINER message
        /// level then the given message is forwarded to all the
        /// registered output Handler objects.  
        /// </para>
        /// </summary>
        /// <param name="msg">The string message (or a key in the message catalog)</param>
        public void Finer(string msg)
        {
            if (Level.FINER.IntValue() < levelValue)
            {
                return;
            }

            Log(Level.FINER, msg);

        }

        /// <summary>
        /// Log a Finest message.
        /// <para>
        /// If the logger is currently enabled for the Finest message
        /// level then the given message is forwarded to all the
        /// registered output Handler objects.  
        /// </para>
        /// </summary>
        /// <param name="msg">The string message (or a key in the message catalog)</param>
        public void Finest(string msg)
        {
            if (Level.FINEST.IntValue() < levelValue)
            {
                return;
            }

            Log(Level.FINEST, msg);

        }

        /// <summary>
        /// Log a INFO message.
        /// <para>
        /// If the logger is currently enabled for the INFO message
        /// level then the given message is forwarded to all the
        /// registered output Handler objects.  
        /// </para>
        /// </summary>
        /// <param name="msg">The string message (or a key in the message catalog)</param>
        public void Info(string msg)
        {
            if (Level.INFO.IntValue() < levelValue)
            {
                return;
            }

            Log(Level.INFO, msg);

        }

        /// <summary>
        /// Is finest level
        /// </summary>
        /// <returns>true if finest or more</returns>
        public static bool IsLevelFinest()
        {
            return VLogMgt.IsLevelFinest();
        }


        /// <summary>
        /// Log a SEVERE message.
        /// <para>
        /// If the logger is currently enabled for the SEVERE message
        /// level then the given message is forwarded to all the
        /// registered output Handler objects.  
        /// </para>
        /// </summary>
        /// <param name="msg">The string message (or a key in the message catalog)</param>
        public void Severe(string msg)
        {
            if (Level.SEVERE.IntValue() < levelValue)
            {
                return;
            }

            Log(Level.SEVERE, msg);

        }


        /// <summary>
        /// Log a WARNING message.
        /// <para>
        /// If the logger is currently enabled for the WARNING message
        /// level then the given message is forwarded to all the
        /// registered output Handler objects.  
        /// </para>
        /// </summary>
        /// <param name="msg">The string message (or a key in the message catalog)</param>
        public void Warning(string msg)
        {
            if (Level.WARNING.IntValue() < levelValue)
            {
                return;
            }

            Log(Level.WARNING, msg);

        }


        public static Logger global = GetLogger("global");  //default logger

        /// <summary>
        /// Get the Logger Object
        /// </summary>
        /// <param name="name">Name of the logger</param>
        /// <returns>Logger Obejct</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static Logger GetLogger(string name)
        {
            LogManager logm = LogManager.GetLogManager();
            Logger result = logm.GetLogger(name);
            if (result == null)
            {
                //if not found, add logger to the log manager
                result = new Logger(name);
                logm.AddLogger(result);
                result = logm.GetLogger(name);
            }
            return result;

        }

        /// <summary>
        /// Set Parent
        /// </summary>
        /// <param name="logger">name of the logger whose parent is to bet set</param>
        public void SetParent(Logger logger)
        {
            //if (parent == null)
            //    return;

            DoSetParent(logger);
        }

        /// <summary>
        /// Finally set the parent
        /// </summary>
        /// <param name="newParent">name of the new parent logger object</param>
        private void DoSetParent(Logger newParent)
        {
            parent = newParent;

            UpdateEffectiveLevel();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateEffectiveLevel()
        {
            // Figure out our current effective level.
            int newLevelValue;

            if (levelObject != null)
            {
                newLevelValue = levelObject.IntValue();
            }
            else
            {
                if (parent != null)
                {
                    newLevelValue = parent.levelValue;
                }
                else
                {
                    // This may happen during initialization.
                    newLevelValue = Level.INFO.IntValue();
                }

            }
            if (levelValue == newLevelValue)
            {
                return;
            }
            levelValue = newLevelValue;

        }


        /// <summary>
        /// Check if a message of the given level would actually be logged
        /// </summary>
        /// <param name="level">level</param>
        /// <returns>true or false</returns>
        public bool IsLoggable(Level level)
        {
            if (level.IntValue() < levelValue || levelValue == offValue)
            {
                return false;
            }
            return true;
        }


        #endregion
    }
}
