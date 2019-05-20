; VIS.Logging = VIS.Logging || {};
(function (VIS, $) {
    //1. Level
    function Level(name, value) {
        this.name = name;
        this.value = value;
    };
    Level.prototype.getIntValue = function () {
        return this.value;
    };
    Level.prototype.getName = function () {
        return this.name;
    };

    Level.prototype.getLevel = function (val) {

        if (Level.OFF.getIntValue() == val)
            return Level.OFF;
        else if (Level.SEVERE.getIntValue() == val)
            return Level.SEVERE;
        else if (Level.WARNING.getIntValue() == val)
            return Level.WARNING;
        else if (Level.INFO.getIntValue() == val)
            return Level.INFO;
        else if (Level.CONFIG.getIntValue() == val)
            return Level.CONFIG;
        else if (Level.FINE.getIntValue() == val)
            return Level.FINE;
        else if (Level.FINER.getIntValue() == val)
            return Level.FINER;
        else if (Level.FINEST.getIntValue() == val)
            return Level.FINEST;
        else if (Level.ALL.getIntValue() == val)
            return Level.ALL;
    };


    Level.OFF = new Level("OFF", 9999);
    Level.SEVERE = new Level("SEVERE", 1000);
    Level.WARNING = new Level("WARNING", 900);
    Level.INFO = new Level("INFO", 800);
    Level.CONFIG = new Level("CONFIG", 700);
    Level.FINE = new Level("FINE", 600);
    Level.FINER = new Level("FINER", 500);
    Level.FINEST = new Level("FINEST", 400);
    Level.ALL = new Level("ALL", 0);

    //2. Log Record
    function LogRecord(level, msg) {
        this._level = level;
        this._sequenceNumber;
        this._sourceClassName;
        this._sourceMethodName;
        this._message = msg;
        this._lineNumber;
        this._loggerName;

    }
    LogRecord.prototype.getMessage = function () {
        return this._message;
    }
    LogRecord.prototype.setMessage = function (msg) {
        this._message = msg;
    }
    LogRecord.prototype.getSourceClassName = function () {
        return this._sourceClassName;
    }
    LogRecord.prototype.setSourceClassName = function (msg) {
        this._sourceClassName = msg;
    }
    LogRecord.prototype.getSourceMethodName = function () {
        return this._sourceMethodName;
    }
    LogRecord.prototype.setSourceMethodName = function (msg) {
        this._sourceMethodName = msg;
    }
    LogRecord.prototype.getLineNumber = function () {
        return this._lineNumber;
    }
    LogRecord.prototype.setLineNumber = function (msg) {
        this._lineNumber = msg;
    }
    LogRecord.prototype.getLoggerName = function () {
        return this._loggerName;
    }
    LogRecord.prototype.setLoggerName = function (msg) {
        this._loggerName = msg;
    }
    LogRecord.prototype.getLevel = function () {
        return this._level;
    }



    //3.VLogFilter
    function VLogFilter() {
        if (VLogFilter.prototype.singleInstance) {
            return VLogFilter.prototype.singleInstance;
        }
        VLogFilter.prototype.singleInstance = this;
    };
    VLogFilter.prototype.singleInstance = null;
    VLogFilter.get = function () {

        if (VLogFilter.prototype.singleInstance) {
            return VLogFilter.prototype.singleInstance;
        }
        return new VLogFilter();
    }
    VLogFilter.prototype.getIsLoggable = function (record) {
        if (record.getLevel() === Level.SEVERE
            || record.getLevel() === Level.WARNING)
            return true;
        //
        var loggerName = record.getLoggerName();
        if (loggerName) {
            //	if (loggerName.toLowerCase().indexOf("focus") != -1)
            //		return true;
            if (loggerName.startsWith("System."))
                return false;
        }
        var className = record.sourceClassName;
        if (className) {
            if (className.startsWith("System."))
                return false;
        }
        return true;
    }



    //4.VLogFormatter
    function VLogFormatter() {
        if (VLogFormatter.prototype.singletonInstance) {
            return VLogFormatter.prototype.singletonInstance;
        }
        VLogFormatter.prototype.singletonInstance = this;
    }
    VLogFormatter.prototype.singletonInstance = null;
    /**	New Line				*/
    VLogFormatter.prototype.NL = '/n';
    VLogFormatter.get = function () {
        if (VLogFormatter.prototype.singletonInstance) {
            return VLogFormatter.prototype.singletonInstance;
        }
        return new VLogFormatter();
    };
    VLogFormatter.prototype.format = function (record) {
        var sb = new StringBuilder();
        //time of execution
        var time = Date.now.toString("yyyy-MM-dd hh:mm:ss.ms");
        time = time + "00";

        /**	Time/Error		*/
        if (record.getLevel() == Level.prototype.SEVERE) {
            //         12.12.12.123 
            sb.append("===========> ");
            sb.append("    (SEVERE)   ");
            //if (VAdvantage.DataBase.Ini.IsClient())
            //System.Media.SystemSounds.Beep.Play();
        }
        else if (record.getLevel() == Level.WARNING) {
            //         12.12.12.123
            sb.append("-----------> ");
            sb.append("    (WARNING)  ");
        }
        else {
            //123456789123456789
            sb.append(time.Substring(11, 23 - 11));
            var spaces = 11;
            if (record.getLevel() == Level.INFO)
                sb.append("    (INFO)     ");
            else if (record.getLevel() == Level.CONFIG)
                sb.append("    (CONFIG)   ");
            else if (record.getLevel() == Level.FINE)
                sb.append("    (FINE)     ");
            else if (record.getLevel() == Level.FINER)
                sb.append("    (FINER)    ");
            else if (record.getLevel() == Level.FINEST)
                sb.append("    (FINEST)   ");
            //sb.Append("                          ".Substring(0, spaces));
        }

        /**	Class.method	**/
        //if (!_shortFormat)
        //   sb.append(GetClassMethod(record)).Append(": ");

        /**	Message			**/
        sb.append(record.message);

        //sb.Append(record.sourceClassName + "." + record.sourceMethodName + " > " + record.lineNumber + " > " + record.message);
        return sb.toString();
    };
    VLogFormatter.prototype.getHead = function () {
        //Assembly asm = Assembly.GetExecutingAssembly();
        var asmName = "ViennaAdvantage";
        var sb = new StringBuilder()
            .append("*** ")
            .append(DateTime.Now.ToString())
            .append(" VAdvantage Log ")
            .append(" ***")
            .append("\n").append(asmName);

        return sb.toString();
    };

    VLogFormatter.prototype.getTail = function () {
        var sb = new StringBuilder()
            .append("*** ")
            //.append(DateTime.Now.ToString())
            .append(" VAdvantage Log")
            .append(" ***");

        return sb.toString();
    };
    /**************************************************************************
     * 	Get Class Method from Log Record
     *	@param record record
     *	@return class.method
     */
    VLogFormatter.prototype.getClassMethod = function (record) {
        var sb = "";
        var className = record.getLoggerName();
        if (className == null
            || className.indexOf("default") != -1	//	anonymous logger
            || className.indexOf("global") != -1)	//	global logger
            className = record.getSourceClassName();
        if (className != null) {
            var index = className.lastIndexOf('.');
            if (index != -1)

                sb += className.substring(index + 1);
            else
                sb += className;
        }
        else
            sb += (record.getLoggerName());
        if (record.getSourceMethodName() != null)
            sb += (".") + record.getSourceMethodName();
        var retValue = sb;
        if (retValue.equals("Trace.printStack"))
            return "";
        return retValue;
    };	//	getCl

    VLogFormatter.prototype.getParameters = function (record) {
        var sb = "";
        var parameters = record.getParameters();
        if (parameters != null && parameters.length > 0) {
            for (var i = 0; i < parameters.length; i++) {
                if (i > 0)
                    sb += ", ";
                sb += parameters[i];
            }
        }
        return sb;
    };	//	getParameters

    /**
     * 	Get Log Exception
     *	@param record log record
     *	@return null if exists or string
     */
    VLogFormatter.prototype.getExceptionTrace = function (record) {
        try {
        }
        catch (ex) {

        }
        return "";
    }	//	getException


    //5. Handler
    function Handler() {
        this.offValue = Level.OFF.getIntValue();
        this.logLevel = Level.ALL;
        this.formatter;
        this.filter;
    }
    Handler.prototype.publish = function (record) {
    }
    Handler.prototype.close = function () {
    }
    Handler.prototype.flush = function () {
    }
    Handler.prototype.setLevel = function (newLevel) {
        if (newLevel == null) {
            throw new NullReferenceException("No Level");
        }
        this.logLevel = newLevel;
    }
    Handler.prototype.getLevel = function () {
        return this.logLevel;
    }
    Handler.prototype.getFilter = function () {
        return this.filter;
    }
    Handler.prototype.setFilter = function (newFilter) {
        this.filter = newFilter;
    }
    Handler.prototype.getIsLoggable = function (record) {
        var levelValue = this.getLevel().getIntValue();
        if (record.getLevel().getIntValue() < levelValue || levelValue == this.offValue) {
            return false;
        }
        var filter = this.getFilter();
        if (filter == null) {
            return true;
        }
        return filter.getIsLoggable(record);
    }
    Handler.prototype.getFormatter = function () {
        return this.formatter;
    }
    Handler.prototype.setFormatter = function (newFormatter) {
        this.formatter = newFormatter;
    }


    //6 Log Buffer
    function VLogErrorBuffer() {
        if (VLogErrorBuffer.prototype.singleInstance) {
            return VLogErrorBuffer.prototype.singleInstance;
        }

        //	Foratting
        this.setFormatter(VLogFormatter.get());
        //	Default Level
        this.setLevel(Level.INFO);
        //	Filter
        this.setFilter(VLogFilter.get());

        /** Error Buffer Size			*/
        this.ERROR_SIZE = 20;
        /**	The Error Buffer			*/
        this.m_errors = [];
        /**	The Error Buffer History	*/
        this.m_history = [];

        /** Log Size					*/
        this.LOG_SIZE = 100;
        /**	The Log Buffer				*/
        this.m_logs = [];
        /**	Issue Error					*/
        this.m_issueError = true;

        VLogErrorBuffer.prototype.singleInstance = this;
    };
    VIS.Utility.inheritPrototype(VLogErrorBuffer, Handler);
    VLogErrorBuffer.prototype.singleInstance = null;
    VLogErrorBuffer.get = function (create) {

        if (VLogErrorBuffer.prototype.singleInstance)
            return VLogErrorBuffer.prototype.singleInstance;
        else if (create) {
            return new VLogErrorBuffer;
        }
        return null;
    }
    VLogErrorBuffer.prototype.setLevel = function (newLevel) {
        if (newLevel == null)
            return;
        if (newLevel == Level.OFF)
            this.$super.setLevel.call(this, Level.SEVERE);
        else if (newLevel == Level.ALL || newLevel == Level.FINEST || newLevel == Level.FINER)
            this.$super.setLevel.call(this, Level.FINE);
        else
            this.$super.setLevel.call(this, newLevel);
    }

    VLogErrorBuffer.prototype.publish = function (record) {

        if (!this.getIsLoggable(record) || this.m_logs == null)
            return;

        //	Output
        if (this.m_logs.length >= this.LOG_SIZE)
            this.m_logs.shift();
        this.m_logs.push(record);

        //	We have an error
        if (record.getLevel() == Level.SEVERE) {
            if (this.m_errors.length >= this.ERROR_SIZE) {
                this.m_errors.shift();
                this.m_history.shift();
            }
            //	Add Error
            this.m_errors.push(record);

            var history = [];
            for (var i = 0; i < this.m_logs.length; i++) {
                var rec = this.m_logs[i];
                if (rec.getLevel() == Level.SEVERE) {
                    if (history.length == 0)
                        history.push(rec);
                    else
                        break;		//	don't incluse previous error
                }
                else {
                    history.push(rec);
                    if (history.length > 10)
                        break;		//	no more then 10 history records
                }

            }

            this.m_history.push(history);
            //	Issue Reporting
            if (this.m_issueError) {
                var loggerName = record.getLoggerName();			//	class name	
                //	String className = record.getSourceClassName();		//	physical class
                var methodName = record.getSourceMethodName();	//	
                //if (methodName && !methodName.equals("SaveError")
                //    && !methodName.equals("Get_Value")
                //    && !methodName.equals("DataSave")
                //    && loggerName.indexOf("Issue") == -1
                //    && loggerName.indexOf("CConnection") == -1
                //    ) {
                //    //m_issueError = false;
                //    //MIssue.create(record);
                //    // m_issueError = true;
                //}
            }
        }
        console.log(record);
    }
    VLogErrorBuffer.prototype.close = function () {
        if (this.m_logs != null) {
            this.m_logs.length = 0;
            this.m_logs = null;
        }

        if (this.m_errors != null) {
            this.m_errors.length = 0;
            this.m_errors = null;
        }
        if (this.m_history != null) {
            this.m_history.length = 0;
            this.m_history = null;
        }
    }

    /**************************************************************************
	 * 	Get ColumnNames of Log Entries
	 * 	@param ctx context (not used)
	 * 	@return string vector
	 */
    VLogErrorBuffer.prototype.getColumnNames = function (ctx) {
        var colName = [4];
        colName[0] = "Time";
        colName[1] = "Level";
        colName[2] = "Class.Method";
        colName[3] = "Message";
        return colName;
    };	//	getColumnNames

    /**
	 * 	Get Array of events with most recent first
	 * 	@param errorsOnly if true errors otherwise log
	 * 	@return array of events 
	 */
    VLogErrorBuffer.prototype.getRecords = function (errorsOnly) {
        var retValue = null;
        if (errorsOnly) {
            retValue = this.m_errors.slice();
        }
        else {
            retValue = this.m_logs.slice();
        }
        return retValue;
    };	//	getEvents

    /**
	 * 	Get Log Data
	 * 	@param errorsOnly if true errors otherwise log
	 * 	@return data array
	 */
    VLogErrorBuffer.prototype.getLogData = function (errorsOnly) {
        var records = this.getRecords(errorsOnly);
        //	System.out.println("getLogData - " + events.length);
        var rows = [];

        for (var i = 0, len = records.length; i < len; i++) {
            var record = records[i];
            var cols = [];
            //
            cols.push(new Date().toString());//.getMillis()));
            cols.push(record.getLevel().getName());
            //
            cols.push(VLogFormatter.get().getClassMethod(record));
            cols.push(record.getMessage());
            //
            //cols.push(VLogFormatter.get().getParameters(record));
            //cols.push(VLogFormatter.get().getExceptionTrace(record));
            //
            rows.push(cols);
        }
        return rows;
    };	//	getData

    /**
	 * 	Reset Error Buffer
	 * 	@param errorsOnly if true errors otherwise log
	 */
    VLogErrorBuffer.prototype.resetBuffer = function (errorsOnly) {
        {
            this.m_errors.length = 0;
            this.m_history.length = 0;
        }
        if (!errorsOnly) {
            this.m_logs.length = 0;
        }
    };
	//	resetBuffer




//7 Logger 

function Logger(name) {
    this.levelValue = Level.INFO.getIntValue();
    this.name = name;    //name of the logger
    this.levelObject;  //level object
    this.handlers = []; //diffrent handlers (VLogFile)
    this.kids = [];
    this.manager = LogManager.getLogManager();    //initiate the logmanager class
    this.parent;    // our nearest parent.
    this.offValue = Level.OFF.getIntValue();
    this.filter;
}
Logger.prototype.setFilter = function (newFilter) {
    this.filter = newFilter;
}
Logger.prototype.getFilter = function () {
    return this.filter;
}
Logger.prototype.log = function (level, msg, e) {

    var msg = msg;
    if (e) {
        if (e.message) {
            msg += " => " + e.message;
        }
        if (e.stack) {
            msg += " ===> " + e.stack;
        }
    }

    //check if msg is eligible to be logged or not
    if (level.getIntValue() < this.levelValue || this.levelValue == Level.OFF.getIntValue())
        return;

    var lr = new LogRecord(level, msg);   //set the values into logrecord class

    lr.setLoggerName(this.name);

    //if (lr.getLevel().getIntValue() < this.levelValue || levelValue == offValue)
    //    return;

    if (this.filter != null && !this.filter.getIsLoggable(lr)) {
        return;
    }

    // Post the LogRecord to all our Handlers, and then to
    // our parents' handlers, all the way up the tree.

    var logger = this;
    while (logger != null) {
        var targets = logger.getHandlers();
        if (targets != null)
            for (var i = 0; i < targets.length; i++) {
                //VLogFile.Publish (abstract method)
                targets[i].publish(lr);
            }

        logger = logger.getParent();
    }
}
Logger.prototype.getParent = function () {
    return this.parent;
}
Logger.prototype.getHandlers = function () {
    if (this.handlers == null) {
        //if handler is null, set the empty handler
        return emptyHandlers;
    }
    return this.handlers;
}
Logger.prototype.getName = function () {
    return this.name;
}
Logger.prototype.setLevel = function (level) {
    this.levelObject = level;
    this.levelValue = level.getIntValue();
    //UpdateEffectiveLevel();
}
Logger.prototype.getLevel = function () {
    return this.levelObject;
}
Logger.prototype.addHandler = function (handler) {

    if (this.handlers == null)
        handlers = [];

    this.handlers.push(handler);
}
Logger.prototype.removeHandler = function (handler) {
    if (handler == null) {
        return;
    }
    if (this.handlers == null) {
        return;
    }
    this.handlers.remove(handler);
}

Logger.getLogger = function (name) {
    var logm = LogManager.getLogManager();
    var result = logm.getLogger(name);
    if (result == null) {
        //if not found, add logger to the log manager
        result = new Logger(name);
        logm.addLogger(result);
        result = logm.getLogger(name);
    }
    return result;

}
Logger.prototype.setParent = function (logger) {
    //if (parent == null)
    //    return;
    this.parent = logger;
    //    DoSetParent(logger);
}
Logger.prototype.getIsLoggable = function (level) {
    if (level.getIntValue() < levelValue || levelValue == offValue) {
        return false;
    }
    return true;
}
Logger.prototype.fine = function (msg) {
    if (Level.FINE.getIntValue() < this.levelValue) {
        return;
    }
    this.log(Level.FINE, msg);
}
Logger.prototype.info = function (msg) {
    if (Level.INFO.getIntValue() < this.levelValue) {
        return;
    }
    this.log(Level.INFO, msg);
}
Logger.prototype.config = function (msg) {
    if (Level.CONFIG.getIntValue() < this.levelValue) {
        return;
    }
    this.log(Level.CONFIG, msg);
}
Logger.prototype.finer = function (msg) {
    if (Level.FINER.getIntValue() < this.levelValue) {
        return;
    }
    this.log(Level.FINER, msg);
}
Logger.prototype.finest = function (msg) {
    if (Level.FINEST.getIntValue() < this.levelValue) {
        return;
    }

    this.log(Level.FINEST, msg);
}
Logger.prototype.severe = function (msg) {
    if (Level.SEVERE.getIntValue() < this.levelValue) {
        return;
    }
    this.log(Level.SEVERE, msg);

}
Logger.prototype.warning = function (msg) {
    if (Level.WARNING.getIntValue() < this.levelValue) {
        return;
    }

    this.log(Level.WARNING, msg);

}
    //Added By Sarab
Logger.prototype.saveError = function (msg,err) {
    //_lastException = err;
    var issueError=true;
    return this.saveError(msg, err.message, issueError);

}
Logger.prototype.saveError = function (msg, err, issueError)
{
   // _lastError = new ValueNamePair(AD_Message, message);
    //  print it
if (issueError)
    this.warning(msg+" "+err.message);
return true;
}   //  saveError



//-----------

Logger.prototype.getIsLevelFinest = function () {
    return VLogMgt.getIsLevelFinest();
}

//8/// RootLogger
function RootLogger() {
    Logger.call(this, "");
    this.setLevel(Level.INFO);
}
VIS.Utility.inheritPrototype(RootLogger, Logger);


//9.Log Manager
function LogManager() {

    if (LogManager.prototype.singleInstance) {
        return LogManager.prototype.singleInstance
    }
    // The global LogManager object
    this.emptyHandlers = [];
    // Table of known loggers.  Maps names to Loggers.
    this.loggers = {};;
    this.defaultLevel = Level.INFO;
    this.rootLogger;

    LogManager.prototype.singleInstance = this;
    // Tree of known loggers
    //private LogNode root = new LogNode(null);
}
LogManager.prototype.singleInstance = null;
LogManager.prototype.addLogger = function (logger) {
    var name = logger.getName();

    var old = null;
    if (name in this.loggers)
        old = loggers[name];

    if (old != null)
        return false;


    // We're adding a new logger.
    // Note that we are creating a strong reference here that will
    // keep the Logger in existence indefinitely.
    // if (loggers.ContainsKey(name))
    this.loggers[name] = logger;
    if (logger instanceof RootLogger) {
    }
    else
        logger.setParent(this.rootLogger);
    // else
    //   loggers.Add(name, logger);

    //Level level = Level.INFO;
    //logger.SetLevel(level);

    // Find the new node and its parent.
    //LogNode node = FindNode(name);
    //node.logger = logger;
    //Logger parent = null;
    //LogNode nodep = node.parent;
    //while (nodep != null)
    //{
    //    if (nodep.logger != null)
    //    {
    //        parent = nodep.logger;
    //        break;
    //    }
    //    nodep = nodep.parent;
    //}


    //if(parent != null)
    //    DoSetParent(logger, parent);

    //node.WalkAndSetParent(logger);
    return true;
    //Level level = getLevelProperty(name + ".level", null);
}
LogManager.prototype.reset = function () {
    //ArrayList list = loggers.ToArray();
    for (var prop in this.loggers) {
        ResetLogger(prop);
    }
}
LogManager.prototype.resetLogger = function (name) {
    var logger = this.getLogger(name);
    if (logger == null) {
        return;
    }
    // Close all the Logger's handlers.
    var targets = logger.getHandlers();
    for (var i = 0; i < targets.length; i++) {
        var h = targets[i];
        logger.removeHandler(h);
        try {
            h.close();
        }
        catch (ex) {
            //Problems closing a handler?  Keep going...
        }
    }

    if (name != null && name.equals("")) {
        // This is the root logger.
        logger.setLevel(this.defaultLevel);
    }
    else {
        logger.setLevel(null);
    }
}
LogManager.prototype.getLogger = function (name) {
    if (name in this.loggers)
        return this.loggers[name];
    else
        return null;
}
LogManager.getLogManager = function () {
    if (!LogManager.prototype.singleInstance) {
        new LogManager();
        var mgr = LogManager.prototype.singleInstance
        mgr.rootLogger = new RootLogger();
        mgr.addLogger(mgr.rootLogger);
    }
    return LogManager.prototype.singleInstance;
}
LogManager.prototype.getLoggerNames = function () {
    return this.loggers;
}


//10. VLogger
function VLogger(name) {
    Logger.call(this, name);
}
VIS.Utility.inheritPrototype(VLogger, Logger);
VLogger.getVLogger = function (className) {
    var manager = LogManager.getLogManager();    //get the current logmanager object
    if (!className)
        className = "";

    var result = manager.getLogger(className);   //find if the logger already exist
    if (result != null && result instanceof VLogger)    //if yes, return the object
        return result;

    //other, we will have to create new
    var newLogger = new VLogger(className);

    newLogger.setLevel(VIS.Logging.VLogMgt.getLevel());

    manager.addLogger(newLogger);
    return newLogger;
}




//11. VLogMgt
function VLogMgt() {
    var _handlers = null;
    /** Current Log Level	*/
    var _currentLevel = Level.INFO;
    /** LOG Levels			*/
    var LEVELS = [Level.OFF, Level.SEVERE, Level.WARNING, Level.INFO,
    Level.CONFIG, Level.FINE, Level.FINER, Level.FINEST, Level.ALL];

    var addHandler = function (handler) {
        if (handler == null)
            return;
        var rootLogger = Logger.getLogger("");
        rootLogger.addHandler(handler);
        //
        _handlers.push(handler);
    }	//	addHandler
    var getIsLevelAll = function () {
        return Level.ALL.getIntValue() == _currentLevel.getIntValue();
    }	//	isLevelFinest
    var initialize = function (isClient) {

        if (_handlers != null)
            return;

        //create handler list
        _handlers = [];
        try {
            var rootLogger = Logger.getLogger("");
            var handlers = rootLogger.getHandlers();

            for (var i = 0; i < handlers.length; i++) {
                if (!_handlers.contains(handlers[i]))
                    _handlers.Add(handlers[i]);
            }
        }
        catch (e) {
        }

        //check loggers

        if (VLogErrorBuffer.get(false) == null)
            addHandler(VLogErrorBuffer.get(true));

        setFormatter(VLogFormatter.get());
        setFilter(VLogFilter.get());
    }
    var setLevel = function (level) {
        if (level == null)
            return;

        if (_handlers == null)
            initialize(true);

        for (var i = 0; i < _handlers.length; i++) {
            var handler = _handlers[i];
            handler.setLevel(level);
        }

        if (level.getIntValue() != _currentLevel.getIntValue()) {
            setLoggerLevel(level, null);
        }
        _currentLevel = level;
    }
    var setLoggerLevel = function (level, loggerNamePart) {
        if (level == null)
            return;
        var mgr = LogManager.getLogManager();
        var en = mgr.getLoggerNames();
        for (var prop in en) {
            var name = prop;
            if (loggerNamePart == null || name.indexOf(loggerNamePart) != -1) {
                var lll = Logger.getLogger(name);
                lll.setLevel(level);
                //if (log.getParent() == lll)
                //    log.SetLevel(level);
            }
        }
    }
    var setFormatter = function (formatter) {
        for (var i = 0; i < _handlers.length; i++) {
            var handler = _handlers[i];
            handler.setFormatter(formatter);
        }
    }
    var setFilter = function (filter) {
        for (var i = 0; i < _handlers.length; i++) {
            var handler = _handlers[i];
            handler.setFilter(filter);
        }
    }	//	setFilter
    var getLevel = function () {
        return _currentLevel;
    }	//	getLevel
    var setLevelByString = function (levelString) {
        if (!levelString)
            return;
        //
        for (var i = 0; i < LEVELS.length; i++) {
            if (LEVELS[i].getName().equals(levelString)) {
                setLevel(LEVELS[i]);
                return;
            }
        }
    }	//	
    var getLevelAsInt = function () {
        return _currentLevel.getIntValue();
    }	//	getLevel
    var shutdown = function () {
        var mgr = LogManager.getLogManager();
        mgr.reset();
    }
    var enable = function (enableLogging) {
        if (enableLogging) {
            setLevel(_currentLevel);
        }
        else {
            var level = _currentLevel;
            setLevel(Level.OFF);
            _currentLevel = level;
        }
    }

    return {

        initialize: initialize,
        addHandler: addHandler,
        setLevel: setLevel,
        setLevelByString: setLevelByString,
        getLevel: getLevel,
        shutdown: shutdown,
        enable: enable,

        getIsLevelAll: function () {
            return Level.ALL.getIntValue() == _currentLevel.getIntValue();
        },
        getIsLevelFinest: function () {
            return Level.FINEST.getIntValue() >= _currentLevel.getIntValue();
        },
        getIsLevelFiner: function () {
            return Level.FINER.getIntValue() >= _currentLevel.getIntValue();
        },
        getIsLevelFine: function () {
            return Level.FINE.getIntValue() >= _currentLevel.getIntValue();
        },
        getIsLevelInfo: function () {
            return Level.INFO.IntValue() >= _currentLevel.IntValue();
        }
    }
};



VIS.Logging.Level = Level;
VIS.Logging.LogRecord = LogRecord;
VIS.Logging.VLogFilter = VLogFilter;
VIS.Logging.VLogFormatter = VLogFormatter;
VIS.Logging.VLogErrorBuffer = VLogErrorBuffer;
VIS.Logging.Logger = Logger;
VIS.Logging.VLogger = VLogger;
VIS.Logging.LogManager = LogManager;
VIS.Logging.VLogMgt = VLogMgt();
}(VIS, jQuery));