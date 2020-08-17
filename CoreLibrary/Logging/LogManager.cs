/********************************************************
 * Module Name    :     Log
 * Purpose        :     Maintain the Log (Manages the Logs)
 * Author         :     Jagmohan Bhatt
 * Date           :     6-Aug-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.CompilerServices;

namespace VAdvantage.Logging
{
    /// <summary>
    /// Manages the Logs and process them
    /// </summary>
    public class LogManager
    {
        // The global LogManager object
        private static LogManager manager;
        private static Handler[] emptyHandlers = { };

        // Table of known loggers.  Maps names to Loggers.
        private Dictionary<String, Logger> loggers = new Dictionary<String, Logger>();
        private static Level defaultLevel = Level.INFO;
        private Logger rootLogger;
        // Tree of known loggers
        private LogNode root = new LogNode(null);

        /// <summary>
        /// Add a named logger.  This does nothing and returns false if a logger
        /// with the same name is already registered.
        /// </summary>
        /// <param name="logger">the new logger.</param>
        /// <returns>true if the argument logger was registered successfully, false if a logger of that name already exists.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool AddLogger(Logger logger)
        {
            string name = logger.GetName();
            //if (string.IsNullOrEmpty(name))
            //    return false;

            Logger old = null;
            if (loggers.ContainsKey(name))
                old = loggers[name];

            if (old != null)
                return false;


            // We're adding a new logger.
            // Note that we are creating a strong reference here that will
            // keep the Logger in existence indefinitely.
            if (loggers.ContainsKey(name))
                loggers[name] = logger;
            else
                loggers.Add(name, logger);

            //Level level = Level.INFO;
            //logger.SetLevel(level);

            // Find the new node and its parent.
            LogNode node = FindNode(name);
            node.logger = logger;
            Logger parent = null;
            LogNode nodep = node.parent;
            while (nodep != null)
            {
                if (nodep.logger != null)
                {
                    parent = nodep.logger;
                    break;
                }
                nodep = nodep.parent;
            }


            if (parent != null)
                DoSetParent(logger, parent);

            node.WalkAndSetParent(logger);
            return true;
            //Level level = getLevelProperty(name + ".level", null);
        }

        /// <summary>
        /// Reset the logging configuration.
        /// <para>
        /// For all named loggers, the reset operation removes and closes
        /// all Handlers and (except for the root logger) sets the level
        /// to null.  The root logger's level is set to Level.INFO.
        /// </para>
        /// </summary>
        /// <param name="name"></param>
        public void Reset()
        {
            //ArrayList list = loggers.ToArray();
            foreach (KeyValuePair<string, Logger> list in loggers)
            {
                string name = list.Key;
                ResetLogger(name);
            }
        }

        /// <summary>
        ///  Private method to reset an individual target logger.
        /// </summary>
        /// <param name="name">name of the logger</param>
        private void ResetLogger(String name)
        {
            Logger logger = GetLogger(name);
            if (logger == null)
            {
                return;
            }
            // Close all the Logger's handlers.
            Handler[] targets = logger.GetHandlers();
            for (int i = 0; i < targets.Length; i++)
            {
                Handler h = targets[i];
                logger.RemoveHandler(h);
                try
                {
                    h.Close();
                }
                catch
                {
                    //Problems closing a handler?  Keep going...
                }
            }

            if (name != null && name.Equals(""))
            {
                // This is the root logger.
                logger.SetLevel(defaultLevel);
            }
            else
            {
                logger.SetLevel(null);
            }
        }


        /// <summary>
        /// Find the available node
        /// </summary>
        /// <param name="name">name of the node</param>
        /// <returns>LogNode</returns>
        private LogNode FindNode(String name)
        {
            if (name == null || name.Equals(""))
            {
                return root;
            }
            LogNode node = root;
            while (name.Length > 0)
            {
                int ix = name.IndexOf(".");
                String head;
                if (ix > 0)
                {
                    head = name.Substring(0, ix);
                    name = name.Substring(ix + 1);
                }
                else
                {
                    head = name;
                    name = "";
                }
                if (node.children == null)
                {
                    node.children = new Dictionary<Object, Object>();
                }

                LogNode child = null;

                if (node.children.ContainsKey(head))
                    child = (LogNode)node.children[head];

                if (child == null)
                {
                    child = new LogNode(node);
                    if (node.children.ContainsKey(head))
                        node.children[head] = child;
                    else
                        node.children.Add(head, child);
                }
                node = child;
            }
            return node;
        }

        /// <summary>
        /// Set the parent for logger object
        /// </summary>
        /// <param name="logger">logger whose parent is to be set</param>
        /// <param name="parent">parent logger</param>
        private static void DoSetParent(Logger logger, Logger parent)
        {
            logger.SetParent(parent);
        }

        /// <summary>
        /// Get the looger by name
        /// </summary>
        /// <param name="name">name of the logger</param>
        /// <returns>Logger Object</returns>
        public Logger GetLogger(string name)
        {
            if (loggers.ContainsKey(name))
                return loggers[name];
            else
                return null;
        }

        /// <summary>
        /// Get an enumeration of known logger names.
        /// </summary>
        /// <returns>enumeration of logger name strings</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Dictionary<String, Logger> GetLoggerNames()
        {
            return loggers;
        }

        /// <summary>
        /// First function which gets executed
        /// </summary>
        public static void Run()
        {
            if (manager == null)
            {
                manager = new LogManager();
            }
        }


        /// <summary>
        /// Blank constructor
        /// </summary>
        public LogManager()
        {
            //does nothing here
        }

        /// <summary>
        /// Get Log Manager
        /// </summary>
        /// <returns>LogManager</returns>
        public static LogManager GetLogManager()
        {
            if (manager == null)
            {
                manager = new LogManager();
                manager.rootLogger = new RootLogger();
                manager.AddLogger(manager.rootLogger);
            }
            return manager;
        }

        /// <summary>
        /// Inner class which maintains the node for logger
        /// </summary>
        public class LogNode
        {
            public Dictionary<object, object> children;
            public Logger logger;
            public LogNode parent;

            public LogNode(LogNode parent)
            {
                this.parent = parent;
            }

            /// <summary>
            /// set the parent for logger node
            /// </summary>
            /// <param name="parent">logger</param>
            public void WalkAndSetParent(Logger parent)
            {
                if (children == null)
                    return;

                for (int i = 0; i <= children.Count - 1; i++)
                {
                    try
                    {
                        LogNode node = (LogNode)children[i];
                        if (node.logger == null)
                            node.WalkAndSetParent(parent);
                        else
                            DoSetParent(node.logger, parent);
                    }
                    catch
                    {
                    }
                }
            }
        }

        /// <summary>
        /// We use a subclass of Logger for the root logger, so
        /// that we only instantiate the global handlers when they
        /// are first needed.
        /// </summary>
        private class RootLogger : Logger
        {
            /// <summary>
            /// Root Logger
            /// </summary>
            public RootLogger()
                : base("")
            {
                SetLevel(defaultLevel);
            }

            /// <summary>
            /// Log
            /// </summary>
            /// <param name="record">LogRecord</param>
            public new void Log(LogRecord record)
            {
                base.Log(record);
            }

            /// <summary>
            /// Add Hanlder
            /// </summary>
            /// <param name="h">Handler</param>
            public new void AddHandler(Handler h)
            {
                base.AddHandler(h);
            }

            /// <summary>
            /// Get Handler
            /// </summary>
            /// <returns>Handler Array</returns>
            public new Handler[] GetHandlers()
            {
                return base.GetHandlers();
            }

        }
    }
}
