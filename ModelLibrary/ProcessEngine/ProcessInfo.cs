
/********************************************************
 * Module Name    : Process
 * Purpose        : Execute the process
 * Author         : Jagmohan Bhatt
 * Date           : 13 jan 2009
  ******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using VAdvantage.Common;
using VAdvantage.DataBase;
using VAdvantage.Utility;

namespace VAdvantage.ProcessEngine
{
    /// <summary>
    /// Information of process (Get and Set)
    /// </summary>
    [Serializable]
    public class ProcessInfo
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strTitle">title to be displayed</param>
        /// <param name="AD_Process_ID">process id</param>
        /// <param name="Table_ID">table id</param>
        /// <param name="Record_ID">record id</param>
        public ProcessInfo(string strTitle, int AD_Process_ID, int Table_ID, int Record_ID)
        {
            SetTitle(strTitle);
            SetAD_Process_ID(AD_Process_ID);
            SetTable_ID(Table_ID);
            SetRecord_ID(Record_ID);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strTitle">title to be displayed</param>
        /// <param name="AD_Process_ID">process id</param>
        /// <param name="Table_ID">table id</param>
        /// <param name="Record_ID">record id</param>
        public ProcessInfo(string strTitle, int AD_Process_ID, int Table_ID, int Record_ID, string recIds)
        {
            SetTitle(strTitle);
            SetAD_Process_ID(AD_Process_ID);
            SetTable_ID(Table_ID);
            SetRecord_ID(Record_ID);
            SetRecIDs(recIds);
        }

        /// <summary>
        /// Overload constructor
        /// </summary>
        /// <param name="title">title to be displayed</param>
        /// <param name="AD_Process_ID">process id</param>
        public ProcessInfo(String title, int AD_Process_ID)
            : this(title, AD_Process_ID, 0, 0)
        {
        }

        public ProcessInfo()
        {

        }

        /* Variable Dec */
        String _title;                  //1
        int _AD_Process_ID;             //2
        int _table_ID;
        string _tableName;
        int _record_ID;
        int _AD_User_ID;
        int _AD_Client_ID;              //7    
        int? _AD_Org_ID;
        String _className = null;
        int _AD_PInstance_ID = 0;
        String _summary = "";
        bool _error = false;
        bool _batch = false;
        bool _timeout = false;
        int _AD_PrintFormat_Table_ID = 0; //15
        string _AD_PrintFormat_TableName = "";
        int _AD_PrintFormat_ID = 0;        //16 
        bool PrintAllPages = false;
        int _AD_ReportView_ID = 0;
        ProcessInfoParameter[] _parameter = null;
        bool _isCrystal = false;
        bool _isReportFormat = false;
        int _totalrecords = 0;              //21
        string recIDs = "";
        bool _isTelerikReport = false;
        bool _isJasperReport = false;
        bool _isSupportPaging = false;          //25
        Object _SerializableObject = null;

        [NonSerialized]
        Object _TransientObject = null;  //27

        // vinay bhatt for window id
        int _ad_window_ID = 0;    ///28
        string _dynamicAction;
        int _pageSize = 100;
        int _totalPage = 1;
        string _fileType = ProcessCtl.ReportType_PDF;
        int _pageNo = 0;
        int _windowNo = 0;

        private List<ProcessInfoLog> _logs = null;
        private int _AD_ReportFormat_ID;
        private int _AD_ReportMaster_ID;
        private bool useCrysalReportViewer = false;
        private bool isReport = false;
        private string ActionOrigin = "W";
        private string OriginName = "";

        private Dictionary<string, string> ctxLocal = new Dictionary<string, string>();

        // Change Lokesh Chauhan
        private string _CustomHTML = "";

        public Dictionary<String, Object> ToList()
        {
            Dictionary<String, Object> lst = new Dictionary<string, object>();

            lst.Add("Title", _title);                    //1
            lst.Add("Process_ID", _AD_Process_ID);
            lst.Add("AD_PInstance_ID", _AD_PInstance_ID);
            lst.Add("Record_ID", _record_ID);
            lst.Add("Error", IsError());
            lst.Add("Summary", GetSummary());
            lst.Add("ClassName", _className);
            lst.Add("AD_Table_ID", _table_ID);
            lst.Add("AD_TableName", _tableName);
            lst.Add("AD_User_ID", _AD_User_ID);             //10
            lst.Add("AD_Client_ID", _AD_Client_ID);
            lst.Add("Batch", _batch);
            lst.Add("TimeOut", _timeout);
            lst.Add("AD_PrintFormat_Table_ID", _AD_PrintFormat_Table_ID);     //14
            lst.Add("AD_PrintFormat_ID", _AD_PrintFormat_ID);
            lst.Add("IsCrystal", _isCrystal);
            lst.Add("IsReportFormat", _isReportFormat);
            lst.Add("TotalRecords", _totalrecords);
            lst.Add("IsTelerikReport", _isTelerikReport);
            lst.Add("IsJasperReport", _isJasperReport);
            lst.Add("SupportPaging", _isSupportPaging);                 //21

            lst.Add("DynamicAction", _dynamicAction);
            lst.Add("PageSize", _pageSize);
            lst.Add("TotalPage", _totalPage);                           //24
            lst.Add("FileType", _fileType);                           //25
            lst.Add("PageNo", _pageNo);
            lst.Add("AD_Window_ID", _ad_window_ID);
            lst.Add("WindowNo", _windowNo);
            lst.Add("PrintFormatTableName", _AD_PrintFormat_TableName);
            lst.Add("AD_ReportView_ID", _AD_ReportView_ID);
            lst.Add("UseCrystalReportViewer", useCrysalReportViewer);
            lst.Add("IsReport", isReport);
            lst.Add("ActionOrigin", ActionOrigin);
            lst.Add("OriginName", OriginName);
            return lst;
        }

        public ProcessInfo FromList(Dictionary<String, string> lst)
        {
            ProcessInfo info = new ProcessInfo(Util.GetValueOfString(lst["Title"]), Util.GetValueOfInt(lst["Process_ID"])); //2
            info._AD_PInstance_ID = Util.GetValueOfInt(lst["AD_PInstance_ID"]);
            info.useCrysalReportViewer = Util.GetValueOfBool(lst["UseCrystalReportViewer"]);
            info.isReport = Util.GetValueOfBool(lst["IsReport"]);
            info._record_ID = Util.GetValueOfInt(lst["Record_ID"]);
            info._error = Convert.ToBoolean(lst["Error"]);
            info._summary = Util.GetValueOfString(lst["Summary"]);
            info._className = Util.GetValueOfString(lst["ClassName"]);
            info._table_ID = Util.GetValueOfInt(lst["AD_Table_ID"]);
            info._tableName = Util.GetValueOfString(lst["AD_TableName"]);
            info._AD_User_ID = Util.GetValueOfInt(lst["AD_User_ID"]);
            info._AD_Client_ID = Util.GetValueOfInt(lst["AD_Client_ID"]); ;
            // info._batch = Convert.ToBoolean(lst["Batch"]);
            info._timeout = Convert.ToBoolean(lst["TimeOut"]);
            info._AD_PrintFormat_Table_ID = Util.GetValueOfInt(lst["AD_PrintFormat_Table_ID"]);  //14
            info._AD_PrintFormat_ID = Util.GetValueOfInt(lst["AD_PrintFormat_ID"]);
            info._isSupportPaging = Util.GetValueOfBool(lst["SupportPaging"]);
            info._dynamicAction = Util.GetValueOfString(lst["DynamicAction"]);       //17

            info._pageSize = Util.GetValueOfInt(lst["PageSize"]);
            if (info._pageSize < 1)
                info._pageSize = 10;

            info._totalPage = Util.GetValueOfInt(lst["TotalPage"]);                     //19
            if (info._totalPage < 1)
                info._totalPage = 1;

            info._fileType = Util.GetValueOfString(lst["FileType"]);
            if (info._fileType == "")
                info._fileType = ProcessCtl.ReportType_PDF;

            info._pageNo = Util.GetValueOfInt(lst["PageNo"]);

            info._ad_window_ID = Util.GetValueOfInt(lst["AD_Window_ID"]);
            info._windowNo = Util.GetValueOfInt(lst["WindowNo"]);
            info.ActionOrigin = Util.GetValueOfString(lst["ActionOrigin"]);
            info.OriginName = Util.GetValueOfString(lst["OriginName"]);
            return info;
        }

        public void SetValueInLocalCtx(string key, string value)
        {
            ctxLocal[key] = value;
        }

        public void Clear()
        {
            ctxLocal.Clear();
        }

        public Dictionary<string, string> GetLocalCtx()
        {
            return ctxLocal;
        }
        public void SetLocalCtx(IDictionary<string, string> ctx)
        {
            ctxLocal = new Dictionary<string, string>(ctx);
        }

        public int GetPageNo()
        {
            return _pageNo;
        }

        public void SetPageNo(int PageNo)
        {
            _pageNo = PageNo;
        }

        public int GetAD_Window_ID()
        {
            return _ad_window_ID;
        }

        public void SetAD_Window_ID(int AD_Window_ID)
        {
            _ad_window_ID = AD_Window_ID;
        }
        //
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("ProcessInfo[");
            sb.Append(_title)
                .Append(",Process_ID=").Append(_AD_Process_ID);
            if (_AD_PInstance_ID != 0)
                sb.Append(",AD_PInstance_ID=").Append(_AD_PInstance_ID);
            if (_record_ID != 0)
                sb.Append(",Record_ID=").Append(_record_ID);
            if (_className != null)
                sb.Append(",ClassName=").Append(_className);
            sb.Append(",Error=").Append(IsError());
            sb.Append(",Summary=").Append(GetSummary());
            //	.Append(GetLogInfo(false));
            sb.Append("]");
            return sb.ToString();
        }   //  toString

        /// <summary>
        /// Sets the summary
        /// </summary>
        /// <param name="summary"></param>
        public void SetSummary(String summary)
        {
            _summary = summary;
            if (summary == null)
            {
                _summary = "null";
            }
        }

        /// <summary>
        /// gets the summary
        /// </summary>
        /// <returns></returns>
        public String GetSummary()
        {
            return Utility.Util.CleanMnemonic(_summary);
        }

        /// <summary>
        /// set summar (overlaod)
        /// </summary>
        /// <param name="TranslatedSummary"></param>
        /// <param name="error"></param>
        public void SetSummary(String TranslatedSummary, bool error)
        {
            SetSummary(TranslatedSummary);
            SetError(error);
        }

        /// <summary>
        /// Method setTransientObject
        /// </summary>
        /// <param name="TransientObject">TransientObject Object</param>
        public void SetTransientObject(Object TransientObject)
        {
            _TransientObject = TransientObject;
        }

        /// <summary>
        /// getTransientObject
        /// </summary>
        /// <returns>Object</returns>
        public Object GetTransientObject()
        {
            return _TransientObject;
        }

        /// <summary>
        /// Get serilaized object
        /// </summary>
        /// <returns></returns>
        public Object GetSerializableObject()
        {
            return _SerializableObject;
        }

        /// <summary>
        /// Set Serializable object
        /// </summary>
        /// <param name="SerializableObject"></param>
        public void SetSerializableObject(Object SerializableObject)
        {
            _SerializableObject = SerializableObject;
        }

        /// <summary>
        /// add summary
        /// </summary>
        /// <param name="AdditionalSummary"></param>
        public void AddSummary(String AdditionalSummary)
        {
            _summary += AdditionalSummary;
        }

        /// <summary>
        /// set error
        /// </summary>
        /// <param name="error"></param>
        public void SetError(bool error)
        {
            _error = error;
        }

        /// <summary>
        /// check if error
        /// </summary>
        /// <returns></returns>
        public bool IsError()
        {
            return _error;
        }

        /// <summary>
        /// Batch - i.e. UI not blocked
        /// </summary>
        /// <returns>bool</returns>
        public bool IsBatch()
        {
            return _batch;
        }

        /// <summary>
        /// Batch
        /// </summary>
        /// <param name="batch">true if batch processing</param>
        public void SetIsBatch(bool batch)
        {
            _batch = batch;
        }

        /// <summary>
        /// get pinstance id
        /// </summary>
        /// <returns></returns>
        public int GetAD_PInstance_ID()
        {
            return _AD_PInstance_ID;
        }

        /// <summary>
        /// set pinstance id
        /// </summary>
        /// <param name="AD_PInstance_ID"></param>
        public void SetAD_PInstance_ID(int AD_PInstance_ID)
        {
            _AD_PInstance_ID = AD_PInstance_ID;
        }

        /// <summary>
        /// get system will use crystal report viewer
        /// </summary>
        /// <returns></returns>
        public bool GetUseCrystalReportViewer()
        {
            return useCrysalReportViewer;
        }

        /// <summary>
        /// Set system will use Report
        /// </summary>
        /// <param name="useCRV"></param>
        public void SetUseCrystalReportViewer(bool useCRV)
        {
            useCrysalReportViewer = useCRV;
        }

        /// <summary>
        /// Set Action Origin (Menu, Window, Form)
        /// </summary>
        /// <param name="ActionOrigin"></param>
        public void SetActionOrigin(string ActionOrigin)
        {
            this.ActionOrigin = ActionOrigin;
        }


        /// <summary>
        /// get Action Origin
        /// </summary>
        /// <returns></returns>
        public String GetActionOrigin()
        {
            return ActionOrigin;
        }

        /// <summary>
        /// Set Action(report or other action viewed or downloaded) origin Name
        /// </summary>
        /// <param name="OriginName"></param>
        public void SetOriginName(string OriginName)
        {
            this.OriginName = OriginName;
        }

        /// <summary>
        /// Get Action Origin Name
        /// </summary>
        /// <returns></returns>
        public String GetOriginName()
        {
            return OriginName;
        }

        /// <summary>
        /// get system will use crystal report viewer
        /// </summary>
        /// <returns></returns>
        public bool GetIsReport()
        {
            return isReport;
        }

        /// <summary>
        /// Set system will use Report
        /// </summary>
        /// <param name="useCRV"></param>
        public void SetIsReport(bool isRep)
        {
            isReport = isRep;
        }




        /// <summary>
        /// get process id
        /// </summary>
        /// <returns></returns>
        public int GetAD_Process_ID()
        {
            return _AD_Process_ID;
        }

        /// <summary>
        /// set process id
        /// </summary>
        /// <param name="AD_Process_ID"></param>
        public void SetAD_Process_ID(int AD_Process_ID)
        {
            _AD_Process_ID = AD_Process_ID;
        }

        /// <summary>
        /// get class name
        /// </summary>
        /// <returns></returns>
        public String GetClassName()
        {
            return _className;
        }

        /// <summary>
        /// set class name
        /// </summary>
        /// <param name="className"></param>
        public void SetClassName(String className)
        {
            _className = className;
            if (_className != null && _className.Length == 0)
                _className = null;
        }


        /// <summary>
        /// set class name
        /// </summary>
        /// <param name="className"></param>
        public string GetTableName()
        {
            return _tableName;

        }

        /// <summary>
        /// get table id
        /// </summary>
        /// <returns></returns>
        public int GetTable_ID()
        {
            return _table_ID;
        }

        /// <summary>
        /// set table id
        /// </summary>
        /// <param name="AD_Table_ID"></param>
        public void SetTable_ID(int AD_Table_ID)
        {
            String sql = "SELECT TableName FROM AD_Table WHERE AD_Table_ID=" + AD_Table_ID;
            object ob = DB.ExecuteScalar(sql);
            if (ob != null)
                _tableName = ob.ToString();
            _table_ID = AD_Table_ID;
        }

        public void SetRecIDs(string recIds)
        {
            this.recIDs = recIds;
        }

        public string GetRecIds()
        {
            return recIDs;
        }

        /// <summary>
        /// get record id
        /// </summary>
        /// <returns></returns>
        public int GetRecord_ID()
        {
            return _record_ID;
        }

        /// <summary>
        /// set reocrd id
        /// </summary>
        /// <param name="Record_ID"></param>
        public void SetRecord_ID(int Record_ID)
        {
            _record_ID = Record_ID;
        }

        /// <summary>
        /// get title
        /// </summary>
        /// <returns></returns>
        public String GetTitle()
        {
            return _title;
        }

        /// <summary>
        /// set title
        /// </summary>
        /// <param name="Title"></param>
        public void SetTitle(String title)
        {
            _title = title;
        }	//	SetTitle

        /// <summary>
        /// set client id
        /// </summary>
        /// <param name="AD_Client_ID"></param>
        public void SetAD_Client_ID(int AD_Client_ID)
        {
            _AD_Client_ID = AD_Client_ID;
        }

        public void SetAD_Org_ID(int AD_Org_ID)
        {
            _AD_Org_ID = AD_Org_ID;
        }

        /// <summary>
        /// get client id
        /// </summary>
        /// <returns></returns>
        public int? GetAD_Client_ID()
        {
            return _AD_Client_ID;
        }

        public int? GetAD_Org_ID()
        {
            return _AD_Org_ID;
        }

        /// <summary>
        /// set user id
        /// </summary>
        /// <param name="AD_User_ID"></param>
        public void SetAD_User_ID(int AD_User_ID)
        {
            _AD_User_ID = AD_User_ID;
        }

        /// <summary>
        /// get user id
        /// </summary>
        /// <returns></returns>
        public int? GetAD_User_ID()
        {
            return _AD_User_ID;
        }

        /// <summary>
        /// get parameter
        /// </summary>
        /// <returns>ProcessInfoParameter object</returns>
        public ProcessInfoParameter[] GetParameter()
        {
            return _parameter;
        }

        /// <summary>
        /// set parameter
        /// </summary>
        /// <param name="parameter">parameter array</param>
        public void SetParameter(ProcessInfoParameter[] parameter)
        {
            _parameter = parameter;
        }

        /// <summary>
        /// imeout
        /// </summary>
        /// <param name="timeout">timeout true still running</param>
        public void SetIsTimeout(bool timeout)
        {
            _timeout = timeout;
        }

        /// <summary>
        /// Timeout - i.e process did not complete
        /// </summary>
        /// <returns>boolean</returns>
        public bool IsTimeout()
        {
            return _timeout;
        }	//	isTimeout


        /// <summary>
        /// Add to Log
        /// </summary>
        /// <param name="Log_ID">Log ID</param>
        /// <param name="P_ID">Process ID</param>
        /// <param name="P_Date">Process Date</param>
        /// <param name="P_Number">Process Number</param>
        /// <param name="P_Msg">Process Message</param>
        public void AddLog(int Log_ID, int? P_ID, DateTime? P_Date, Decimal? P_Number, String P_Msg)
        {
            AddLog(new ProcessInfoLog(Log_ID, P_ID, P_Date, P_Number, P_Msg));
        }	//	addLog

        /// <summary>
        /// Add to Log
        /// </summary>
        /// <param name="P_ID">Process ID</param>
        /// <param name="P_Date">Process Date</param>
        /// <param name="P_Number">Process Number</param>
        /// <param name="P_Msg">Process Message</param>
        public void AddLog(int P_ID, DateTime? P_Date, Decimal? P_Number, String P_Msg)
        {
            AddLog(new ProcessInfoLog(P_ID, P_Date, P_Number, P_Msg));
        }	//	addLog

        /// <summary>
        /// Add to Log
        /// </summary>
        /// <param name="logEntry">log entry</param>
        public void AddLog(ProcessInfoLog logEntry)
        {
            if (logEntry == null)
                return;
            if (_logs == null)
                _logs = new List<ProcessInfoLog>();
            _logs.Add(logEntry);
        }	//	addLog

        /// <summary>
        /// Get Logs
        /// </summary>
        /// <returns>ProcessInfoLog[]</returns>
        public ProcessInfoLog[] GetLogs()
        {
            if (_logs == null)
                return null;
            ProcessInfoLog[] logs = new ProcessInfoLog[_logs.Count];
            logs = _logs.ToArray();
            return logs;
        }	//	getLogs

        /// <summary>
        /// <para>
        /// Set Log of Process.
        /// - Translated Process Message
        /// - List of log entries
        ///     Date - Number - Msg
        /// </para>
        /// </summary>
        /// <param name="html">if true with HTML markup</param>
        /// <returns>Log Info</returns>
        public String GetLogInfo(bool html)
        {
            //NOTE** : dateformat has not been implemented

            if (_logs == null)
                return "";
            //
            StringBuilder sb = new StringBuilder();
            //SimpleDateFormat dateFormat = DisplayType.getDateFormat(DisplayType.DateTime);
            if (html)
                sb.Append("<table width=\"100%\" border=\"1\" cellspacing=\"0\" cellpadding=\"2\">");
            //
            for (int i = 0; i < _logs.Count; i++)
            {
                if (html)
                    sb.Append("<tr>");
                else if (i > 0)
                    sb.Append("\n");
                //
                ProcessInfoLog log = (ProcessInfoLog)_logs[i];
                /**
                if (log.getP_ID() != 0)
                    sb.append(html ? "<td>" : "")
                        .append(log.getP_ID())
                        .append(html ? "</td>" : " \t");	**/
                //
                if (log.GetP_Date() != DateTime.MinValue)
                    sb.Append(html ? "<td>" : "")
                        .Append(log.GetP_Date())
                        .Append(html ? "</td>" : " \t");
                //
                if (log.GetP_Number() != null)
                    sb.Append(html ? "<td>" : "")
                        .Append(log.GetP_Number())
                        .Append(html ? "</td>" : " \t");
                //
                if (log.GetP_Msg() != null)
                    sb.Append(html ? "<td>" : "")
                        .Append(Utility.Msg.ParseTranslation(Utility.Env.GetContext(), log.GetP_Msg()))
                        .Append(html ? "</td>" : "");
                //
                if (html)
                    sb.Append("</tr>");
            }
            if (html)
                sb.Append("</table>");
            return sb.ToString();
        }	//	getLogInfo
        public void SetLogList(List<ProcessInfoLog> logs)
        {
            _logs = logs;
        }	//	setLogList

        /// <summary>
        /// Method getIDs
        /// </summary>
        /// <returns>int[]</returns>
        public int[] GetIDs()
        {
            if (_logs == null)
            {
                return null;
            }
            int[] ids = new int[_logs.Count];
            for (int i = 0; i < _logs.Count; i++)
            {
                ids[i] = Utility.Util.GetValueOfInt(((ProcessInfoLog)_logs[i]).GetP_ID());
            }
            return ids;
        }

        /// <summary>
        /// set user id
        /// </summary>
        /// <param name="AD_User_ID"></param>
        public void Set_AD_PrintFormat_Table_ID(int AD_PrintFormat_Table_ID)
        {

            String sql = "SELECT TableName FROM AD_Table WHERE AD_Table_ID=" + AD_PrintFormat_Table_ID;
            object ob = DB.ExecuteScalar(sql);
            if (ob != null)
                _AD_PrintFormat_TableName = ob.ToString();
            _AD_PrintFormat_Table_ID = AD_PrintFormat_Table_ID;
        }
        public int Get_AD_PrintFormat_Table_ID()
        {
            return _AD_PrintFormat_Table_ID;
        }

        /// <summary>
        /// set user id
        /// </summary>
        /// <param name="AD_User_ID"></param>
        public void Set_AD_PrintFormat_ID(int AD_PrintFormat_ID)
        {
            _AD_PrintFormat_ID = AD_PrintFormat_ID;
        }
        public int Get_AD_PrintFormat_ID()
        {
            return _AD_PrintFormat_ID;
        }

        /// <summary>
        /// set user id
        /// </summary>
        /// <param name="AD_User_ID"></param>
        public void SetPrintAllPages(bool all)
        {
            PrintAllPages = all;
        }
        public bool GetPrintAllPages()
        {
            return PrintAllPages;
        }


        public void SetIsCrystal(bool isCrystal)
        {
            _isCrystal = isCrystal;
        }
        public bool GetIsCrystal()
        {
            return _isCrystal;
        }

        public void SetIsTelerik(bool IsTelerikReport)
        {
            _isTelerikReport = IsTelerikReport;
        }
        public bool GetIsTelerik()
        {
            return _isTelerikReport;
        }

        public void SetIsReportFormat(bool isRF)
        {
            _isReportFormat = isRF;
        }
        public bool GetIsReportFormat()
        {
            return _isReportFormat;
        }

        public void SetTotalRecords(int tRecords)
        {
            _totalrecords = tRecords;

        }
        public int GetTotalRecords()
        {
            return _totalrecords;
        }

        /// <param name="AD_User_ID"></param>
        public void Set_AD_ReportView_ID(int AD_ReportView_ID)
        {
            _AD_ReportView_ID = AD_ReportView_ID;
        }
        public int Get_AD_ReportView_ID()
        {
            return _AD_ReportView_ID;
        }

        public void SetIsJasperReport(bool IsJasperReport)
        {
            _isJasperReport = IsJasperReport;
        }
        public bool GetIsJasperReport()
        {
            return _isJasperReport;
        }

        public bool GetIsSupportPaging()
        {
            return _isSupportPaging;
        }
        /// <summary>
        /// Report engine support paging 
        /// </summary>
        /// <param name="isSupportPaging"></param>
        public void SetIsSupportPaging(bool isSupportPaging)
        {
            _isSupportPaging = isSupportPaging;
        }

        public void SetTotalPage(int totalPage)
        {
            _totalPage = totalPage;

        }
        public int GetTotalPage()
        {
            return _totalPage;
        }

        public void SetPageSize(int pageSize)
        {
            _pageSize = pageSize;

        }
        public int GetPageSize()
        {
            return _pageSize;
        }

        public void SetDynamicAction(string da)
        {
            _dynamicAction = da;

        }
        public string GetDynamicAction()
        {
            return _dynamicAction;
        }

        public void SetFileType(string ft)
        {
            _fileType = ft;

        }
        public string GetFileType()
        {
            return _fileType;
        }

        /// <summary>
        /// Report Compser specific setting
        /// </summary>
        public bool IsArabicReportFromOutside
        {
            get;
            set;
        }

        public void SetAD_ReportFormat_ID(int AD_ReportFormat_ID)
        {
            _AD_ReportFormat_ID = AD_ReportFormat_ID;
        }
        public void SetAD_ReportMaster_ID(int AD_ReportMaster_ID)
        {
            _AD_ReportMaster_ID = AD_ReportMaster_ID;
        }

        public int GetAD_ReportFormat_ID()
        {
            return _AD_ReportFormat_ID;
        }
        public int GetAD_ReportMaster_ID()
        {
            return _AD_ReportMaster_ID;
        }

        // Change Lokesh Chauhan
        public string GetCustomHTML()
        {
            return _CustomHTML;
        }
        public void SetCustomHTML(string HTML)
        {
            _CustomHTML = HTML;
        }
    }
}
