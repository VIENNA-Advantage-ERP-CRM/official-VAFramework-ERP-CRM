/********************************************************
 * Module Name    :     Log
 * Purpose        :     Maintain the Log (Stores the type of handler (Console, File or Buffer))
 * Author         :     Jagmohan Bhatt
 * Date           :     3-Aug-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace VAdvantage.Logging
{
    /// <summary>
    /// A <tt>Handler</tt> object takes log messages from a <tt>Logger</tt> and
    /// exports them.  It might for example, write them to a console
    /// or write them to a file, or send them to a network logging service,
    /// or forward them to an OS log, or whatever.
    /// <para>
    /// A <tt>Handler</tt> can be disabled by doing a <tt>setLevel(Level.OFF)</tt>
    /// and can  be re-enabled by doing a <tt>setLevel</tt> with an appropriate level.
    /// </para>
    /// <tt>Handler</tt> classes typically use <tt>LogManager</tt> properties to set
    /// default values for the <tt>Handler</tt>'s <tt>Filter</tt>, <tt>Formatter</tt>,
    /// and <tt>Level</tt>.  See the specific documentation for each concrete
    /// <tt>Handler</tt> class.
    /// </summary>
    public abstract class Handler
    {
        /// <summary>
        /// defualt off value
        /// </summary>
        private static int offValue = Level.OFF.IntValue();

        private Level logLevel = Level.ALL; //set the initial log level

        /// <summary>
        /// Publish a Log Record
        /// </summary>
        /// <param name="record">LogRecord which contains the msg</param>
        public abstract void Publish(LogRecord record);

        /// <summary>
        /// close the writer object
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// Flush any buffered output
        /// </summary>
        public abstract void Flush();

        /// <summary>
        /// The intention is to allow developers to turn on voluminous logging, but to limit the messages that are sent to certain Handlers
        /// </summary>
        /// <param name="newLevel">New Level to set</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetLevel(Level newLevel)
        {
            if (newLevel == null)
                throw new NullReferenceException("No Level");

            logLevel = newLevel;
        }

        /// <summary>
        /// Get the log level specifying which messages will be logged by this Handler.  Message levels lower than this level will be discarded.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Level GetLevel()
        {
            return logLevel;
        }

        private Filter filter;  //take an instance of interface filter to filter the log

        /// <summary>
        /// Gets the Filter object
        /// </summary>
        /// <returns></returns>
        public Filter GetFilter()
        {
            return filter;
        }

        /// <summary>
        /// Set Filter
        /// </summary>
        /// <param name="newFilter">Filter to be set</param>
        public void SetFilter(Filter newFilter)
        {
            filter = newFilter;
        }

        /// <summary>
        /// Check if this <tt>Handler</tt> would actually log a given <tt>LogRecord</tt>.
        /// <para>
        /// This method checks if the <tt>LogRecord</tt> has an appropriate
        /// <tt>Level</tt> and  whether it satisfies any <tt>Filter</tt>.  It also
        /// may make other <tt>Handler</tt> specific checks that might prevent a
        /// handler from logging the <tt>LogRecord</tt>. It will return false if
        /// the <tt>LogRecord</tt> is Null.
        /// </para>
        /// </summary>
        /// <param name="record">LogRecord</param>
        /// <returns>true, if the LogRecord would be logged</returns>
        public bool IsLoggable(LogRecord record)
        {
            int levelValue = GetLevel().IntValue();
            if (record.GetLevel().IntValue() < levelValue || levelValue == offValue)
            {
                return false;
            }
            Filter filter = GetFilter();
            if (filter == null)
            {
                return true;
            }
            return filter.IsLoggable(record);
        }

        private Formatter formatter;    //null object of Formatter claass

        /// <summary>
        /// Get the Formatter
        /// </summary>
        /// <returns>Formatter</returns>
        public Formatter GetFormatter()
        {
            return formatter;
        }

        /// <summary>
        /// Set the instantiated object to formatter 
        /// </summary>
        /// <param name="newFormatter">Formatter object</param>
        public void SetFormatter(Formatter newFormatter)
        {
            formatter = newFormatter;

        }
    }
}
