/********************************************************
 * Module Name    :     Log
 * Purpose        :     Maintain the Log (Manages the Logs)
 * Author         :     Jagmohan Bhatt
 * Date           :     4-Aug-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VAdvantage;


namespace VAdvantage.Logging
{
    /// <summary>
    /// Log Management.
    /// </summary>
    public class VLogMgt
    {
        /** Handlers			*/
        private static List<Handler> _handlers = null;

        /** Logger				*/
        private static Logger log = Logger.GetAnonymousLogger();

        /// <summary>
        /// Initializes 
        /// use Initialize(bool,string,bool)
        /// </summary>
        /// <param name="isClient">if client</param>
        public static void Initialize(bool isClient)
        {

            if (_handlers != null)
            {
                return;

            }


            //create handler list
            _handlers = new List<Handler>();
            try
            {
                //Logger rootLogger = Logger.GetLogger("");
                //Handler[] handlers = rootLogger.GetHandlers();

                //for (int i = 0; i < handlers.Length; i++)
                //{
                //    if (!_handlers.Contains(handlers[i]))
                //        _handlers.Add(handlers[i]);
                //}
            }
            catch
            {
            }

            //check loggers
            //if (VLogErrorBuffer.Get(false) == null)
            //    AddHandler(VLogErrorBuffer.Get(true));

            VLogFile fh = VLogFile.Get(false, null, isClient);
            if (fh == null && !isClient)
            {
                fh = VLogFile.Get(true, null, isClient);
                AddHandler(fh);
            }

            SetFormatter(VLogFormatter.Get());
            SetFilter(VLogFilter.Get());

        }


        //private static DateTime? _lastDate = null;





        /// <summary>
        /// Initilize log file 
        /// </summary>
        /// <param name="isClient"> client file (always set true)</param>
        /// <param name="Path"> file path </param>
        public static void Initialize(bool isClient, String Path)
        {
            //if (_lastDate == null || !Utility.Util.IsEqual(_lastDate.Value.Date, DateTime.Now.Date)) //Set Now Date
            //{
            //    _lastDate = DateTime.Now.Date; 
            //}
            //else
            //{
            //    //return if Last and current date is same // do not init log file
            //    return;
            //}


            if (_handlers != null)
            {
                return; //already initilize
            }
            //create handler list

            //try
            //{
            //    Logger rootLogger = Logger.GetLogger("");
            //    Handler[] handlers = rootLogger.GetHandlers();

            //    for (int i = 0; i < handlers.Length; i++)
            //    {
            //        if (!_handlers.Contains(handlers[i]))
            //            _handlers.Add(handlers[i]);
            //    }
            //}
            //catch
            //{
            //}

            //check loggers
            // if (VLogErrorBuffer.Get(false) == null)
            //    AddHandler(VLogErrorBuffer.Get(true));


            if (Path.Contains("*"))
            {
                Path = Path.Replace("*", "Star");
            }


            _handlers = new List<Handler>();

            VLogFile fh = VLogFile.Get(false, Path, isClient);
            if (fh == null)
            {
                fh = VLogFile.Get(true, Path, isClient);
            }

            AddHandler(fh);

            SetFormatter(VLogFormatter.Get());
            SetFilter(VLogFilter.Get());
        }

        /** Current Log Level	*/
        private static Level _currentLevel = Level.INFO;

        /** LOG Levels			*/
        public static Level[] LEVELS = new Level[] 
		{Level.OFF, Level.SEVERE, Level.WARNING, Level.INFO,
		Level.CONFIG, Level.FINE, Level.FINER, Level.FINEST, Level.ALL};

        /// <summary>
        /// Gets the current Level
        /// </summary>
        /// <returns>Level</returns>
        public static Level GetLevel()
        {
            return _currentLevel;
        }	//	getLevel

        /// <summary>
        /// Add the handler
        /// </summary>
        /// <param name="handler">handler</param>
        public static void AddHandler(Handler handler)
        {

            if (handler == null)
                return;
            Logger rootLogger = Logger.GetLogger("");
            rootLogger.AddHandler(handler);
            //
            _handlers.Add(handler);
            log.Log(Level.CONFIG, "Handler=" + handler);
            //log.Log(Level.ALL, "Handler=" + handler);



        }	//	addHandler


        public static void AddHandler(Handler handler, string level)
        {
            _handlers = new List<Handler>();


            if (handler == null)
                return;
            Logger rootLogger = Logger.GetLogger("");
            rootLogger.AddHandler(handler);
            //
            _handlers.Add(handler);
            // log.Log(Level.CONFIG, "Handler=" + handler);
            if (level.Equals("SEVERE"))
            {
                log.Log(Level.SEVERE, "Handler=" + handler);
            }
            else if (level.Equals("WARNING"))
            {
                log.Log(Level.WARNING, "Handler=" + handler);
            }
            else if (level.Equals("INFO"))
            {
                log.Log(Level.INFO, "Handler=" + handler);
            }
            else if (level.Equals("CONFIG"))
            {
                log.Log(Level.CONFIG, "Handler=" + handler);
            }
            else if (level.Equals("FINE"))
            {
                log.Log(Level.FINE, "Handler=" + handler);
            }
            else if (level.Equals("FINER"))
            {
                log.Log(Level.FINER, "Handler=" + handler);
            }
            else if (level.Equals("FINEST"))
            {
                log.Log(Level.FINEST, "Handler=" + handler);
            }
            else
            {
                log.Log(Level.ALL, "Handler=" + handler);
            }

        }	//	addHandler

        /// <summary>
        /// Is Logging Level FINEST logged
        /// </summary>
        /// <returns></returns>
        public static bool IsLevelAll()
        {
            return Level.ALL.IntValue() == _currentLevel.IntValue();
        }	//	isLevelFinest


        /// <summary>
        /// Is Logging level Finest
        /// </summary>
        /// <returns></returns>
        public static bool IsLevelFinest()
        {
            return Level.FINEST.IntValue() >= _currentLevel.IntValue();
        }	//	isLevelFinest

        /// <summary>
        /// Is Logging level finer
        /// </summary>
        /// <returns>true, if level is finer</returns>
        public static bool IsLevelFiner()
        {
            return Level.FINER.IntValue() >= _currentLevel.IntValue();
        }	//	isLevelFiner

        /// <summary>
        /// Is Logging level fine
        /// </summary>
        /// <returns>true if level is fine</returns>
        public static bool IsLevelFine()
        {
            return Level.FINE.IntValue() >= _currentLevel.IntValue();
        }	//	isLevelFine

        /// <summary>
        /// Is Logging level info
        /// </summary>
        /// <returns>true if level is info</returns>
        public static bool IsLevelInfo()
        {
            return Level.INFO.IntValue() >= _currentLevel.IntValue();
        }	//	isLevelFine


        /// <summary>
        /// Set Level for all handlers
        /// </summary>
        /// <param name="levelString">string representation of level</param>
        public static void SetLevel(String levelString)
        {
            if (levelString == null)
                return;
            //
            for (int i = 0; i < LEVELS.Length; i++)
            {
                if (LEVELS[i].GetName().Equals(levelString))
                {
                    SetLevel(LEVELS[i]);
                    return;
                }
            }
            log.Log(Level.CONFIG, "Ignored: " + levelString);
        }	//	setLevel

        /// <summary> 
        /// Set Level for handler
        /// </summary>
        /// <param name="level">level</param>
        public static void SetLevel(Level level)
        {
            if (level == null)
                return;

            if (_handlers == null)
                Initialize(true);

            for (int i = 0; i < _handlers.Count; i++)
            {
                Handler handler = (Handler)_handlers[i];
                handler.SetLevel(level);
            }

            if (level.IntValue() != _currentLevel.IntValue())
            {
                SetLoggerLevel(level, null);

                log.Config(level.ToString());
            }

            _currentLevel = level;
        }

        /// <summary>
        /// Set Level for all Loggers
        /// </summary>
        /// <param name="level">log level</param>
        /// <param name="loggerNamePart">optional partial class/logger name</param>
        public static void SetLoggerLevel(Level level, String loggerNamePart)
        {
            if (level == null)
                return;
            LogManager mgr = LogManager.GetLogManager();
            Dictionary<String, Logger> en = mgr.GetLoggerNames();
            foreach (KeyValuePair<string, Logger> list in en)
            {
                String name = list.Key;
                if (loggerNamePart == null || name.IndexOf(loggerNamePart) != -1)
                {
                    Logger lll = Logger.GetLogger(name);
                    lll.SetLevel(level);
                    if (log.GetParent() == lll)
                        log.SetLevel(level);
                }
            }
        }	//	setLoggerLevel


        /// <summary>
        /// Get Handlers
        /// </summary>
        /// <returns>hanlers</returns>
        public static Handler[] GetHandlers()
        {
            Handler[] handlers = new Handler[_handlers.Count];
            for (int i = 0; i < _handlers.Count; i++)
                handlers[i] = (Handler)_handlers[i];
            return handlers;
        }	//	getHandlers


        /// <summary>
        /// Get Level as Integer value
        /// </summary>
        /// <returns></returns>
        public static int GetLevelAsInt()
        {
            return _currentLevel.IntValue();
        }	//	getLevel

        /// <summary>
        /// Shutdown the logmanager
        /// </summary>
        public static void Shutdown()
        {

            LogManager mgr = LogManager.GetLogManager();
            mgr.Reset();
            _handlers = null;
        }

        /// <summary>
        /// Set Formatter
        /// </summary>
        /// <param name="formatter"></param>
        protected static void SetFormatter(Formatter formatter)
        {
            for (int i = 0; i < _handlers.Count; i++)
            {
                Handler handler = (Handler)_handlers[i];
                handler.SetFormatter(formatter);
            }
            log.Log(Level.CONFIG, "Formatter=" + formatter);
        }	//	setFormatter

        /// <summary>
        /// Set Filter
        /// </summary>
        /// <param name="filter"></param>
        protected static void SetFilter(Filter filter)
        {
            for (int i = 0; i < _handlers.Count; i++)
            {
                Handler handler = (Handler)_handlers[i];
                handler.SetFilter(filter);
            }
            log.Log(Level.CONFIG, "Filter=" + filter);
        }	//	setFilter

        /// <summary>
        /// Get Message
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        //private static String GetMsg(String msg)
        //{
        //    if (DataBase.DB.IsConnected())
        //        return Msg.Translate(Env.GetContext(), msg);
        //    return msg;
        //}   //  GetMsg

        /// <summary>
        /// Get Info detail
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="sb">existing info</param>
        /// <returns>Info in string</returns>
        //public static StringBuilder GetInfo(Ctx ctx, StringBuilder sb)
        //{
        //    if (sb == null)
        //        sb = new StringBuilder();
        //    //String eq = " = ";
        //    //sb.Append(GetMsg(Program.NAME))
        //    //    .Append(" ").Append(Program.MAIN_VERSION).Append("_").Append(Program.DATE_VERSION)
        //    //    .Append(" Implementation:").Append(Environment.NewLine);

        //    //sb.Append(GetMsg("Host")).Append(eq).Append(Ini.GetProperty("dbhost") + " : " + Ini.GetProperty("dbport")).Append(Environment.NewLine);
        //    //sb.Append(GetMsg("Database")).Append(eq).Append(Ini.GetProperty("dbhost") + " / " + Ini.GetProperty("dbname")).Append(Environment.NewLine);
        //    //sb.Append(GetMsg("VAF_UserContact_ID")).Append(eq).Append(ctx.GetContext("#VAF_UserContact_Name")).Append(Environment.NewLine);
        //    //sb.Append(GetMsg("VAF_Role_ID")).Append(eq).Append(ctx.GetContext("#VAF_Role_Name")).Append(Environment.NewLine);
        //    ////
        //    //sb.Append(GetMsg("VAF_Client_ID")).Append(eq).Append(ctx.GetContext("#VAF_Client_Name")).Append(Environment.NewLine);
        //    //sb.Append(GetMsg("VAF_Org_ID")).Append(eq).Append(ctx.GetContext("#VAF_Org_Name")).Append(Environment.NewLine);
        //    ////
        //    //sb.Append(GetMsg("Date")).Append(eq).Append(ctx.GetContext("#Date")).Append(Environment.NewLine);
        //    //sb.Append(GetMsg("Printer")).Append(eq).Append(ctx.GetPrinterName()).Append(Environment.NewLine);
        //    ////
        //    return sb;
        //}   //  getInfo


        /// <summary>
        /// Info Detail
        /// </summary>
        /// <param name="sb">existing info</param>
        /// <param name="ctx">context</param>
        /// <returns>info detail in string</returns>
        //public static StringBuilder GetInfoDetail(StringBuilder sb, Ctx ctx)
        //{
        //    if (sb == null)
        //        sb = new StringBuilder();
        //    if (ctx == null)
        //        ctx = Env.GetContext();
        //    //  Envoronment
        //    sb.Append(Environment.NewLine)
        //        .Append("=== Context ===").Append(Env.NL);
        //    String[] context = ctx.GetEntireContext();
        //    for (int i = 0; i < context.Length; i++)
        //        sb.Append(context[i]).Append(Env.NL);
        //    //  System
        //    sb.Append(Env.NL)
        //        .Append("=== System ===").Append(Env.NL);

        //    System.OperatingSystem osInfo = System.Environment.OSVersion;

        //    sb.Append("Operating System: ");
        //    switch (osInfo.Version.Major)
        //    {
        //        case 3:
        //            sb.Append("Windows NT 3.51").Append(Environment.NewLine + "Service Pack: ").Append(osInfo.ServicePack).Append(Environment.NewLine);
        //            break;
        //        case 4:
        //            sb.Append("Windows NT 4.0").Append(Environment.NewLine + "Service Pack: ").Append(osInfo.ServicePack).Append(Environment.NewLine);
        //            break;
        //        case 5:
        //            if (osInfo.Version.Minor == 0)
        //                sb.Append("Windows 2000").Append(Environment.NewLine + "Service Pack: ").Append(osInfo.ServicePack).Append(Environment.NewLine);
        //            else
        //                sb.Append("Windows XP").Append(Environment.NewLine + "Service Pack: ").Append(osInfo.ServicePack).Append(Environment.NewLine);
        //            break;
        //    }


        //    return sb;
        //}

        /// <summary>
        /// Enable/Disable logging (of handlers)
        /// </summary>
        /// <param name="enableLogging">enableLogging true if logging enabled</param>
        public static void Enable(bool enableLogging)
        {
            if (enableLogging)
            {
                SetLevel(Level.INFO);
            }
            else
            {
                Level level = _currentLevel;
                SetLevel(Level.OFF);
               // _currentLevel = level;
            }
        }

    }
}
