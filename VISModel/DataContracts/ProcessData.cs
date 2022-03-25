using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;


namespace VIS.Models
{

    /// <summary>
    /// return Initial detail of process when user click process menu
    /// </summary>
    public class ProcessDataOut
    {
        public String Name { get; set; }
        public String Description { get; set; }
        public String Help { get; set; }
        public int AD_Process_ID { get; set; }
        public int AD_CtxArea_ID { get; set; }
        public String IsSOTrx { get; set; }
        public String MessageText { get; set; }
        public bool IsBackground { get; set; }
        public bool AskUser { get; set; }
        public bool IsCrystal { get; set; }

        public bool IsError { get; set; }
        public string Message { get; set; }
        public bool IsReport { get; set; }
        public bool HasPara { get; set; }
        public string ImageUrl { get; set; }
        public string FontName { get; set; }
    }

    /// <summary>
    /// store process para info
    /// </summary>
    public class ProcessPara
    {
        public String Name { get; set; }
        public String Info { get; set; }
        public String Info_To { get; set; }
        public int DisplayType { get; set; }
        public object Result { get; set; }
        public object Result2 { get; set; }
        public bool LoadRecursiveData { get;set; }
        public bool ShowChildOfSelected { get; set; }
        public int AD_Column_ID { get; set; }
    }


    /// <summary>
    /// class object to communicate process info between server and client
    /// </summary>

    public class ProcessReportInfo
    {
        int _record_ID;
        string recIDs = "";
        private bool isReport = false;
        int _AD_Process_ID;             //2
        public int AD_PrintFormat_ID
        {
            get;
            set;
        }

        public int AD_ReportView_ID
        {
            get;
            set;
        }

        public int AD_PInstance_ID { get; set; }

        public Dictionary<string, object> ReportProcessInfo
        {
            get;
            set;
        }

        public byte[] Report
        {
            get;
            set;
        }

        public string ReportString
        {
            get;
            set;

        }

        public bool IsError { get; set; }
        public string Message { get; set; }

        public bool ShowParameter
        {
            get;
            set;
        }

        public String ReportFilePath { get; set; }

        public List<GridField> ProcessFields { get; set; }

        public string HTML
        {
            get;
            set;
        }
        public bool IsRCReport
        {
            get;
            set;
        }
        public string ErrorText
        {
            get;
            set;
        }
        public int AD_Table_ID
        {
            get;
            set;
        }
        public bool AskForNewTab
        {
            get;
            set;
        }

        public int TotalRecords { get; set; }

        public bool IsReportFormat { get; set; }

        public bool IsTelerikReport { get; set; }
        public bool IsJasperReport { get; set; }

        public string RecordIDs
        {
            get;
            set;
        }

        /// <summary>
        /// record id
        /// </summary>
        /// <returns></returns>
        public int RecordID
        {
            get;
            set;
        }

        /// <summary>
        /// get system will use crystal report viewer
        /// </summary>
        /// <returns></returns>
        public bool IsReport
        {
            get;
            set;
        }

        /// <summary>
        /// get process id
        /// </summary>
        /// <returns></returns>
        public int AD_Process_ID
        {
            get;
            set;
        }

        public string Result { get; set; }

        // Change Lokesh Chauhan
        public string CustomHTML { get; set; }

        public bool IsBiHTMlReport { get; set; }
    }

    public class GridReportInfo
    {
        public ProcessReportInfo repInfo
        {
            get;
            set;
        }
        public PageSetting pSetting
        {
            get;
            set;
        }
        public string Error
        {
            get;
            set;
        }
    }

}