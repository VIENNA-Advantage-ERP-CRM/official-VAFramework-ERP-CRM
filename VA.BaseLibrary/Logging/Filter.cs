/********************************************************
 * Module Name    :     Log
 * Purpose        :     Filter the log
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
    /// A Filter can be used to provide fine grain control over
    /// what is logged, beyond the control provided by log levels.
    /// <para>
    /// Each Logger and each Handler can have a filter associated with it.
    /// The Logger or Handler will call the isLoggable method to check
    /// if a given LogRecord should be published.  If isLoggable returns
    /// false, the LogRecord will be discarde
    ///</para>
    /// </summary>
    public interface Filter
    {
        /// <summary>
        /// Check if a given log record should be published.
        /// </summary>
        /// <param name="record"></param>
        /// <returns>true, if loggable</returns>
        bool IsLoggable(LogRecord record);
    }
}
