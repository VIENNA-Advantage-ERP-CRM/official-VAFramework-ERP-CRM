using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VIS.Models;

namespace VIS.DataContracts
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

        public bool IsError { get; set; }
        public string Message { get; set; }
        public bool IsReport { get; set; }
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