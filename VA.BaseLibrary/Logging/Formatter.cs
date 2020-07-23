/********************************************************
 * Module Name    :     Log
 * Purpose        :     Format the log msg
 * Author         :     Jagmohan Bhatt
 * Date           :     21-Aug-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.Logging
{
    /// <summary>
    /// Contains the function which are used to format the log message
    /// </summary>
    public abstract class Formatter
    {
        /// <summary>
        /// Blank Contstructor
        /// </summary>
        internal Formatter()
        {
            //do nothing here
        }

        /// <summary>
        /// Format the record stored in LogRecord
        /// </summary>
        /// <param name="record">LogRecord</param>
        /// <returns>formatted string</returns>
        public abstract String Format(LogRecord record);

        /// <summary>
        /// Create the Header for log file
        /// </summary>
        /// <returns>header string</returns>
        public abstract String GetHead();

        /// <summary>
        /// Create the footer for log file
        /// </summary>
        /// <returns>footer string</returns>
        public abstract String GetTail();
    }
}
