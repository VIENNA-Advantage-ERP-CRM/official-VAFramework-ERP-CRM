/********************************************************
 * Module Name    :     Log
 * Purpose        :     Maintain the Log (Creates and Edit the file)
 * Author         :     Jagmohan Bhatt
 * Date           :     5-Aug-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace VAdvantage.Logging
{
    /// <summary>
    /// Vienna Log File Handler
    /// </summary>
    public class VLogFile : Handler
    {
        //[DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        //private static extern int AllocConsole();

        private static VLogFile s_logFile = null;

        /**	Output file				*/
        private FileStream _file = null;
        /**	File writer				*/
        private StreamWriter _writer = null;

        /** Current File Name Date	*/
        private String _fileNameDate = "";

        /**	Printed header			*/
        private bool _doneHeader = false;

        private DateTime? _lastFileDate = null;


        private static readonly object _lock = new object();

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="viennaHome">log file base directory name</param>
        /// <param name="createLogDir">create log directory</param>
        /// <param name="isClient">client</param>
        /// 

      //  [MethodImpl(MethodImplOptions.Synchronized)]
        private void Initialize(String viennaHome, bool createLogDir, bool isClient)
        {
            lock (_lock)
            {
                Close();//close existing

                _doneHeader = false;

                //	New File Name
                if (!CreateFile(viennaHome, createLogDir, isClient))
                    return;

                _lastFileDate = DateTime.Now;


                try
                {
                    _writer = new StreamWriter(_file);
                    m_records = 0;
                    //Publish(null);
                }
                catch
                {
                }

                //	Foratting
                SetFormatter(VLogFormatter.Get());
                //	Level
                SetLevel(Level.FINE);
                //	Filter
                SetFilter(VLogFilter.Get());
            }
        }

        /// <summary>
        /// Get File Logger
        /// </summary>
        /// <param name="create">create if not exists</param>
        /// <param name="viennaHome">vienna home</param>
        /// <param name="isClient">client or not</param>
        /// <returns>File Logger</returns>
        public static VLogFile Get(bool create, String viennaHome, bool isClient)
        {
            //if (s_logFile!=null && !s_logFile._viennaHome.Equals(viennaHome))
            //{
            //    s_logFile = null;
            //    create = true;
            //}
            //if (s_logFile == null && create)
            s_logFile = new VLogFile(viennaHome, true, isClient);
            return s_logFile;
        }	//	get


        /**	Vienna Home			*/
        private String _viennaHome = "";


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="viennaHome">home directory</param>
        /// <param name="createLogDir">create log directory or not</param>
        /// <param name="isClient">client or not</param>
        public VLogFile(String viennaHome, bool createLogDir, bool isClient)
        {
            //if (s_logFile == null)
            //    s_logFile = this;
            //else
            //    new Exception("Error in Handlers");
            //
            if (viennaHome != null && viennaHome.Length > 0)
                _viennaHome = viennaHome;
            else
                _viennaHome = Environment.CurrentDirectory + "\\Vienna\\home";// ini.GetFrameworkHome();
            Initialize(_viennaHome, createLogDir, isClient);



        }	//	CLogFile



        /// <summary>
        /// Creates the Log file within application directory.
        /// </summary>
        /// <param name="baseDirName">directory name</param>
        /// <param name="createLogDir">create directory or not</param>
        /// <param name="isClient">client or not</param>
        /// <returns>true if file created</returns>
        private bool CreateFile(String baseDirName, bool createLogDir, bool isClient)
        {
            String fileName = baseDirName;
            int index = fileName.LastIndexOf('\\');
            String Sufix = fileName.Substring(index + 1, fileName.Length - (index + 1));
            fileName = fileName.Substring(0, index);
            try
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    DirectoryInfo dir = new DirectoryInfo(fileName);
                    if (!dir.Exists)
                    {
                        fileName = "";
                    }
                }
                if (!string.IsNullOrEmpty(fileName) && createLogDir)
                {
                    fileName += Path.DirectorySeparatorChar + "log";
                    DirectoryInfo dir = new DirectoryInfo(fileName);

                    if (!dir.Exists)
                        dir.Create();

                    //if (!dir.Exists)
                    //    fileName = "";

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        fileName += Path.DirectorySeparatorChar;
                        if (isClient)
                            //fileName += "client";
                            fileName += Sufix;

                        _fileNameDate = GetFileNameDate();
                        fileName += _fileNameDate + "_";
                        for (int i = 0; i < 100; i++)
                        {
                            String finalName = fileName + i + ".log";
                            if (!File.Exists(finalName))
                            {
                                FileStream file = new FileStream(finalName, FileMode.OpenOrCreate, FileAccess.Write);
                                _file = file;
                                break;
                            }
                        }
                        if (_file == null)		//	Fallback create temp file
                        {
                            _file = new FileStream(Environment.GetEnvironmentVariable("TEMP") + "\\" + "vienna.log", FileMode.OpenOrCreate, FileAccess.Write);
                        }
                    }
                }
            }
            catch
            {
                _file = null;
                return false;
            }
            return true;


        }

        /// <summary>
        /// Gets the name of the file according to the date it is created on
        /// </summary>
        /// <returns>Gets the name of the file</returns>
        public static String GetFileNameDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd") + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute;
        }	//	getFileNameDate


        /** Record Counter			*/
        private int m_records = 0;

        /// <summary>
        /// Publishes the file
        /// </summary>
        /// <param name="record">stores the values to be printed</param>
        /// 
       // [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Publish(LogRecord record)
        {
            lock (_lock)
            {
                //if (DateTime.Now.Hour != Convert.ToDateTime(_lastFileDate).Hour) //Set Now Date
                //{
                //    Initialize(_viennaHome, true, true);
                //}

                if (DateTime.Now > Convert.ToDateTime(_lastFileDate).AddHours(1)) //Set Now Date
                {
                    Initialize(_viennaHome, true, true);
                }

                if (!IsLoggable(record) || _writer == null)
                    return;

                //	Format
                String msg = null;
                try
                {
                    msg = GetFormatter().Format(record);
                }
                catch
                {
                    return;
                }

                try
                {
                    if (!_doneHeader)
                    {
                        _writer.WriteLine(GetFormatter().GetHead());
                        //AllocConsole();
                        //Console.WriteLine(GetFormatter().GetHead()); // outputs to console window
                        _doneHeader = true;
                    }
                    _writer.WriteLine(msg);
                    //AllocConsole();
                    //Console.WriteLine(msg); // outputs to console window
                    m_records++;

                    //if (string.IsNullOrEmpty(record.message))
                    //    record.message = "Executed";
                    if (record.GetLevel() == Level.SEVERE
                        || record.GetLevel() == Level.WARNING
                        || m_records % 10 == 0)	//	flush every 10 records
                        Flush();
                    //print into file
                    //_writer.WriteLine(this.Format(record));
                    //_writer.Flush();   //finally flush the print
                }
                catch
                {

                    Close();
                    _lastFileDate = null; //try next time when publish is called 
                    //try again to initlize file 
                }
            }
        }

        /// <summary>
        /// Close the writer object
        /// </summary>
        public override void Close()
        {
            if (_writer == null)
                return;

            try
            {
                if (!_doneHeader)
                    _writer.WriteLine(GetFormatter().GetHead());

                _writer.WriteLine(GetFormatter().GetTail());
            }
            catch
            {
            }

            Flush();

            try
            {
                _writer.Close();
                _file.Close();
            }
            catch
            {
            }
            _writer = null;
            _file = null;
            s_logFile = null;
            _lastFileDate = null;

        }

        /// <summary>
        /// Flush the buffered output
        /// </summary>
        public override void Flush()
        {
            try
            {
                if (_writer != null)
                    _writer.Flush();
            }
            catch
            {
                //errror occured
            }
        }
    }
}
