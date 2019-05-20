/********************************************************
 * Module Name    : Log
 * Purpose        : Format Log
 * Chronological Development
 * Jagmohan Bhatt   21-Aug-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.Logging
{
    /// <summary>
    /// Log Formatter
    /// </summary>
    public class VLogFormatter : Formatter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public VLogFormatter()
            : base()
        {
        }

        /// <summary>
        /// Get Formatter
        /// </summary>
        /// <returns>singleton</returns>
        public static VLogFormatter Get()
        {
            if (_formatter == null)
                _formatter = new VLogFormatter();
            return _formatter;
        }	//	get

        /**	Singleton				*/
        private static VLogFormatter _formatter = null;
        /**	New Line				*/
        public static String NL = Environment.NewLine;


        /** Short Format			*/
        private bool _shortFormat = false;


        /// <summary>
        /// Format
        /// </summary>
        /// <param name="record">LogRecord</param>
        /// <returns>formatted string</returns>
        public override String Format(LogRecord record)
        {
            StringBuilder sb = new StringBuilder();
            //time of execution
            string time = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ms");
            time = time + "00";

            /**	Time/Error		*/
            if (record.GetLevel() == Level.SEVERE)
            {
                //         12.12.12.123 
                sb.Append("===========> ");
                sb.Append("    (SEVERE)   ");
                //if (VAdvantage.DataBase.Ini.IsClient())
                //    System.Media.SystemSounds.Beep.Play();
            }
            else if (record.GetLevel() == Level.WARNING)
            {
                //         12.12.12.123
                sb.Append("-----------> ");
                sb.Append("    (WARNING)  ");
            }
            else
            {
                //123456789123456789
                sb.Append(time.Substring(11, 23 - 11));
                //int spaces = 11;
                if (record.GetLevel() == Level.INFO)
                    sb.Append("    (INFO)     ");
                else if (record.GetLevel() == Level.CONFIG)
                    sb.Append("    (CONFIG)   ");
                else if (record.GetLevel() == Level.FINE)
                    sb.Append("    (FINE)     ");
                else if (record.GetLevel() == Level.FINER)
                    sb.Append("    (FINER)    ");
                else if (record.GetLevel() == Level.FINEST)
                    sb.Append("    (FINEST)   ");
                //sb.Append("                          ".Substring(0, spaces));
            }

            /**	Class.method	**/
            if (!_shortFormat)
                sb.Append(GetClassMethod(record)).Append(": ");

            /**	Message			**/
            sb.Append(record.message);

            //sb.Append(record.sourceClassName + "." + record.sourceMethodName + " > " + record.lineNumber + " > " + record.message);
            return sb.ToString();
        }


        /// <summary>
        /// Gets the header for the log file.
        /// </summary>
        /// <returns>header string</returns>
        public override string GetHead()
        {
            StringBuilder sb = new StringBuilder()
                .Append("*** ")
                .Append(DateTime.Now.ToString())
                .Append(" VAdvantage Log ")
                .Append(" ***");

            return sb.ToString();
        }

        /// <summary>
        /// Gets the footer for the log file.
        /// </summary>
        /// <returns>footer stirng</returns>
        public override String GetTail()
        {
            StringBuilder sb = new StringBuilder()
                .Append("*** ")
                .Append(DateTime.Now.ToString())
                .Append(" VAdvantage Log")
                .Append(" ***");

            return sb.ToString();
        }	//	getTail


        public void SetFormat(bool shortFormat)
        {
            _shortFormat = shortFormat;
        }	//	setFormat


        /// <summary>
        /// Get Class Method from Log Record
        /// </summary>
        /// <param name="record">record</param>
        /// <returns>class.method</returns>
        public static String GetClassMethod(LogRecord record)
        {
            StringBuilder sb = new StringBuilder();
            String className = record.GetLoggerName();
            if (className == null
                || className.IndexOf("default") != -1	//	anonymous logger
                || className.IndexOf("global") != -1)	//	global logger
                className = record.sourceClassName;
            if (className != null)
            {
                int index = className.LastIndexOf('.');
                if (index != -1)
                    sb.Append(className.Substring(index + 1));
                else
                    sb.Append(className);
            }
            else
                sb.Append(record.GetLoggerName());
            if (record.sourceMethodName != null)
                sb.Append(".").Append(record.sourceMethodName);
            String retValue = sb.ToString();
            if (retValue.Equals("Trace.printStack"))
                return "";
            return retValue;
        }	//	getClassMethod
    }
}
