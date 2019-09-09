/********************************************************
 * Module Name    :     Log
 * Purpose        :     Maintain the Log (Maintains the Log REcord)
 * Author         :     Jagmohan Bhatt
 * Date           :     7-Aug-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace VAdvantage.Logging
{
    /// <summary>
    /// LogRecord objects are used to pass logging requests between
    /// the logging framework and individual log Handlers.
    /// </summary>
    [Serializable()]
    public class LogRecord
    {
        private Level _level;
        //private long _sequenceNumber;
        private string _sourceClassName;
        private string _sourceMethodName;
        private string _message;
        private string _lineNumber;
        private string _parentClassNameOfSourceClass;
        /// <summary>
        /// Gets and Sets the log message
        /// </summary>
        public string message
        {
            set
            {
                _message = value;
            }
            get
            {
                return _message;
            }

        }

        /// <summary>
        /// Gets and Sets the log class name
        /// </summary>
        public string sourceClassName
        {
            set
            {
                _sourceClassName = value;
            }
            get
            {
                return _sourceClassName;
            }
        }

        /// <summary>
        /// Gets and Sets the log method name
        /// </summary>
        public string sourceMethodName
        {
            set
            {
                _sourceMethodName = value;

            }
            get
            {
                return _sourceMethodName;
            }
        }

        /// <summary>
        /// Gets and Sets the log line number
        /// </summary>
        public string lineNumber
        {
            set
            {
                _lineNumber = value;

            }
            get
            {
                return _lineNumber;
            }
        }
        public string ParentClassNameOfSourceClass
        {
            set
            {
                _parentClassNameOfSourceClass = value;
            }
            get
            {
                return _parentClassNameOfSourceClass;
            }
        }      
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="level">Level of the Log</param>
        /// <param name="msg">msg</param>
        public LogRecord(Level level, string msg)
        {

            this._level = level;
            this._message = msg;

            //****************Trace the class name and method**************//
            StackTrace st = new StackTrace(true);
            StackFrame[] sf = st.GetFrames();
            // First, search back to a method in the Logger class.
            int i = 0;
            while (i < sf.Length)
            {
                StackFrame frame = st.GetFrame(i);
                System.Reflection.MethodBase bas = frame.GetMethod();
                if (bas != null && bas.ReflectedType!=null)
                {
                    Type ty = bas.ReflectedType;
                    string name = ty.FullName;

                    string className = frame.GetMethod().ReflectedType.FullName;
                    if (className.Equals("VAdvantage.Logging.Logger") || className.Equals("VAdvantage.Logging.VLogger"))
                    {
                        break;
                    }
                }
                i++;
            }
            // Now search for the first frame before the "Logger" class.
            while (i < sf.Length)
            {
                StackFrame frame = st.GetFrame(i);
                if (frame.GetMethod().ReflectedType == null)
                {
                    i++;
                    continue;
                }
                string className = frame.GetMethod().ReflectedType.FullName;
                if (!className.Equals("VAdvantage.Logging.Logger") && !className.Equals("VAdvantage.Logging.VLogger"))
                {
                    if (className.Equals("VAdvantage.Model.PO"))
                    {
                        i++;
                        continue;
                    }
                    if (st.GetFrame(i).GetMethod().IsConstructor)   //in case of contstr, set the name as Load
                        this.sourceMethodName = "Load";
                    else
                        this.sourceMethodName = st.GetFrame(i).GetMethod().Name;
                    this.lineNumber = "Line at: " + st.GetFrame(i).GetFileLineNumber();
                    this.sourceClassName = st.GetFrame(i).GetMethod().ReflectedType.FullName;
                    this.ParentClassNameOfSourceClass = st.GetFrame(i).GetMethod().ReflectedType.BaseType.Name;
                    return;
                }
                i++;

            }
        }

        /// <summary>
        /// Get Level
        /// </summary>
        /// <returns></returns>
        public Level GetLevel()
        {
            return _level;
        }

        private String loggerName;  //logger name

        /// <summary>
        /// Get the Logger name
        /// </summary>
        /// <returns></returns>
        public String GetLoggerName()
        {
            return loggerName;
        }

        /// <summary>
        /// Set the Logger Name
        /// </summary>
        /// <param name="name"></param>
        public void SetLoggerName(String name)
        {
            loggerName = name;
        }

    }
}
