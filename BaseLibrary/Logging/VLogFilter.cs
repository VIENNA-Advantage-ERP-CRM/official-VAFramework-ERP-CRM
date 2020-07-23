/********************************************************
 * Module Name    : Log
 * Purpose        : Filter Log
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
    /// Log Filter
    /// </summary>
    public class VLogFilter : Filter
    {
        /// <summary>
        /// Get Filter
        /// </summary>
        /// <returns>singleton</returns>
        public static VLogFilter Get()
        {
            if (s_filter == null)
                s_filter = new VLogFilter();
            return s_filter;
        }

        /**	Singleton			*/
        private static VLogFilter s_filter = null;

        public VLogFilter()
        {
        }	//	CLogFilter

        /// <summary>
        /// Check if a message of the given level would actually be logged
        /// </summary>
        /// <param name="record">a message logging level</param>
        /// <returns>true if the given message level is currently being logged.</returns>
        bool Filter.IsLoggable(LogRecord record)
        {
            if (record.GetLevel() == Level.SEVERE
                || record.GetLevel() == Level.WARNING)
                return true;
            //
            String loggerName = record.GetLoggerName();
            if (loggerName != null)
            {
                //	if (loggerName.toLowerCase().indexOf("focus") != -1)
                //		return true;
                if (loggerName.StartsWith("System."))
                    return false;
            }
            String className = record.sourceClassName;
            if (className != null)
            {
                if (className.StartsWith("System."))
                    return false;
            }
            return true;
        }	//	isLoggable
    }
}
