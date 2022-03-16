/********************************************************
 * Module Name    : Log
 * Purpose        : Buffer Log
 * Chronological Development
 * Jagmohan Bhatt   21-Aug-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;


namespace VAdvantage.Logging
{
    /// <summary>
    /// Client Error Buffer
    /// </summary>
    public class VLogErrorBuffer : Handler
    {
        public VLogErrorBuffer()
        {
            //ALogin
            if (s_handler == null)
                s_handler = this;
            Initialize();
        }	//	CLogErrorBuffer

        /**	Appender				*/
        private static VLogErrorBuffer s_handler;

        /// <summary>
        /// Get Client Log Handler
        /// </summary>
        /// <param name="create">create if not exists</param>
        /// <returns>handler</returns>
        public static VLogErrorBuffer Get(bool create)
        {
            if (s_handler == null && create)
                s_handler = new VLogErrorBuffer();
            return s_handler;
        }	//	get

        /** Error Buffer Size			*/
        private static int ERROR_SIZE = 20;
        /**	The Error Buffer			*/
        private LinkedList<LogRecord> m_errors = new LinkedList<LogRecord>();
        /**	The Error Buffer History	*/
        private LinkedList<LogRecord[]> m_history = new LinkedList<LogRecord[]>();

        /** Log Size					*/
        private static int LOG_SIZE = 100;
        /**	The Log Buffer				*/
        private LinkedList<LogRecord> m_logs = new LinkedList<LogRecord>();
        /**	Issue Error					*/
        private volatile bool m_issueError = true;

        /// <summary>
        /// Initialize
        /// </summary>
        private void Initialize()
        {
            //	System.out.println("CLogConsole.initialize");

            //	Foratting
            SetFormatter(VLogFormatter.Get());
            //	Default Level
            base.SetLevel(Level.INFO);
            //	Filter
            SetFilter(VLogFilter.Get());
        }	//	initialize


        /// <summary>
        /// Issue Error 
        /// </summary>
        /// <returns>true if issue error</returns>
        public bool IsIssueError()
        {
            return m_issueError;
        }	//	isIssueError


        /// <summary>
        /// Set IssueError
        /// </summary>
        /// <param name="issueError">true if error is to be issued</param>
        public void SetIssueError(bool issueError)
        {
            m_issueError = issueError;
        }	//	setIssueError

        /// <summary>
        /// Set Level.
        /// Ignore OFF - and higer then FINE
        /// </summary>
        /// <param name="newLevel">ignored</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public new void SetLevel(Level newLevel)
        {
            if (newLevel == null)
                return;
            if (newLevel == Level.OFF)
                base.SetLevel(Level.SEVERE);
            else if (newLevel == Level.ALL || newLevel == Level.FINEST || newLevel == Level.FINER)
                base.SetLevel(Level.FINE);
            else
                base.SetLevel(newLevel);
        }	//	SetLevel


        /// <summary>
        /// Publish
        /// Logging.Handler#Publish()
        /// </summary>
        /// <param name="record"></param>
        public override void Publish(LogRecord record)
        {
            if (!IsLoggable(record) || m_logs == null)
                return;

            //	Output
            lock (m_logs)
            {
                if (m_logs.Count >= LOG_SIZE)
                    m_logs.RemoveFirst();
                m_logs.AddLast(record);
            }

            //	We have an error
            if (record.GetLevel() == Level.SEVERE)
            {
                if (m_errors.Count >= ERROR_SIZE)
                {
                    m_errors.RemoveFirst();
                    m_history.RemoveFirst();
                }
                //	Add Error
                m_errors.AddLast(record);
                //record.sourceClassName;	//	forces Class Name eval

                //	Create History
                List<LogRecord> history = new List<LogRecord>();
                try
                {
                    foreach (LogRecord logr in m_logs)
                    {
                        LogRecord rec = (LogRecord)logr;
                        if (rec.GetLevel() == Level.SEVERE)
                        {
                            if (history.Count == 0)
                                history.Add(rec);
                            else
                                break;		//	don't incluse previous error
                        }
                        else
                        {
                            history.Add(rec);
                            if (history.Count > 10)
                                break;		//	no more then 10 history records
                        }

                    }
                }
                catch
                {

                }
                LogRecord[] historyArray = new LogRecord[history.Count];
                int no = 0;
                for (int i = history.Count - 1; i >= 0; i--)
                    historyArray[no++] = (LogRecord)history[i];
                m_history.AddLast(historyArray);
                //	Issue Reporting
                if (m_issueError)
                {
                    String loggerName = record.GetLoggerName();			//	class name	
                    //	String className = record.getSourceClassName();		//	physical class
                    String methodName = record.sourceMethodName;	//	
                    if (!methodName.Equals("SaveError")
                        && !methodName.Equals("Get_Value")
                        && !methodName.Equals("DataSave")
                        && loggerName.IndexOf("Issue") == -1
                        && loggerName.IndexOf("CConnection") == -1
                        // && VAdvantage.DataBase.DB.IsConnected()
                        )
                    {
                        m_issueError = false;
                        //MVAFIssue.create(record);
                        m_issueError = true;
                    }
                }
            }
        }

        /// <summary>
        /// Close
        /// <see>
        /// Logging.Handler#close()
        /// </see>
        /// </summary>
        public override void Close()
        {
            if (m_logs != null)
                m_logs.Clear();
            m_logs = null;
            if (m_errors != null)
                m_errors.Clear();
            m_errors = null;
            if (m_history != null)
                m_history.Clear();
            m_history = null;
        }

        public override void Flush()
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Get ColumnNames of Log Entries
        /// </summary>
        /// <param name="ctx">context (not used)</param>
        /// <returns>string List</returns>
        //public object[] GetColumnNames(VAdvantage.Classes.Context ctx)
        //{
        //    colName = new object[4];
        //    colName[0] = "Time";
        //    colName[1] = "Level";
        //    colName[2] = "Class.Method";
        //    colName[3] = "Message";
        //    //List<String> cn = new List<String>();
        //    //cn.Add("Time");
        //    //cn.Add("Level");
        //    ////
        //    //cn.Add("Class.Method");
        //    //cn.Add("Message");
        //    //2
        //    //
        //    return colName;
        //}	//	getColumnNames

        /**	Constains column name for Datagrindview binding				*/
       // object[] colName;
        /**	Constains Data for Datagrindview binding				*/
        object[,] errorData;


        /// <summary>
        /// Get Log Data
        /// </summary>
        /// <param name="errorsOnly">if true errors otherwise log</param>
        /// <returns> data array</returns>
        public object[,] GetLogData(bool errorsOnly)
        {
            LogRecord[] records = GetRecords(errorsOnly);
            //	System.out.println("getLogData - " + events.length);
            //List<List<Object>> rows = new List<List<Object>>(records.Length);

            errorData = new object[records.Length, 4];
            for (int i = 0; i < records.Length; i++)
            {
                LogRecord record = records[i];
                List<Object> cols = new List<Object>();
                //
                string time = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ms");
                time = time + "00";
                time = time.Substring(11, 23 - 11);
                errorData[i, 0] = time;
                errorData[i, 1] = record.GetLevel().GetName();
                errorData[i, 2] = VLogFormatter.GetClassMethod(record);
                errorData[i, 3] = record.message;

                //cols.Add(DateTime.Now.ToString());
                //cols.Add(record.GetLevel().GetName());
                //
                //cols.Add(VLogFormatter.GetClassMethod(record));
                //cols.Add(record.message);
                //
                //cols.Add(CLogFormatter.getParameters(record));
                //cols.Add(CLogFormatter.getExceptionTrace(record));
                //
                //rows.Add(cols);
            }
            return errorData;
        }	//	getData

        /// <summary>
        /// Get Array of events with most recent first
        /// </summary>
        /// <param name="errorsOnly">if true errors otherwise log</param>
        /// <returns>array of events </returns>
        public LogRecord[] GetRecords(bool errorsOnly)
        {
            LogRecord[] retValue = null;
            if (errorsOnly)
            {
                lock (m_errors)
                {
                    retValue = new LogRecord[m_errors.Count];
                    retValue = m_errors.ToArray();
                }
            }
            else
            {
                lock (m_logs)
                {
                    retValue = new LogRecord[m_logs.Count];
                    retValue = m_logs.ToArray();
                }
            }
            return retValue;
        }	//	getEvents


        /// <summary>
        /// Reset Error Buffer
        /// </summary>
        /// <param name="errorsOnly">if true errors otherwise log</param>
        public void ResetBuffer(bool errorsOnly)
        {
            lock (m_errors)
            {
                m_errors.Clear();
                m_history.Clear();
            }
            if (!errorsOnly)
            {
                lock (m_logs)
                {
                    m_logs.Clear();
                }
            }
        }	//	resetBuffer


        /// <summary>
        /// Get/Put Error Info in String
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="errorsOnly">if true errors otherwise log</param>
        /// <returns>info</returns>
        //public String GetErrorInfo(Context ctx, bool errorsOnly)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    //
        //    if (errorsOnly)
        //    {
        //        foreach(LogRecord[] logarr in m_history)
        //        {
        //            sb.Append("-------------------------------\n");
        //            LogRecord[] records = (LogRecord[])logarr;
        //            for (int j = 0; j < records.Length; j++)
        //            {
        //                LogRecord record = records[j];
        //                sb.Append(GetFormatter().Format(record));
        //            }
        //        }
        //    }
        //    else
        //    {
        //        foreach (LogRecord logs in m_logs)
        //        {
        //            LogRecord record = (LogRecord)logs;
        //            sb.Append(GetFormatter().Format(record)).Append(Environment.NewLine);
        //        }
        //    }
        //    VLogMgt.GetInfo(VAdvantage.Utility.Env.GetContext(), sb);
        //    VLogMgt.GetInfoDetail(sb, ctx);
        //    //
        //    return sb.ToString();
        //}	//	getErrorInfo


        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("CLogErrorBuffer[");
            sb.Append("Errors=").Append(m_errors.Count)
                .Append(",History=").Append(m_history.Count)
                .Append(",Logs=").Append(m_logs.Count)
                .Append(",Level=").Append(GetLevel())
                .Append("]");
            return sb.ToString();
        }	//	toString

        /// <summary>
        /// Hashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
