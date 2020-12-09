using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// window Data Contract To pass between Client -Server
/// </summary>
/// 
namespace VIS.DataContracts
{

    /// <summary>
    /// 
    /// </summary>
   

    

    public class SaveRecordOut
    {
        public bool IsError { get; set; }
        public string ErrorMsg { get; set; }
        public string InfoMsg { get; set; }
        public bool FireEEvent { get; set; }
        public bool FireIEvent { get; set; }

        public Dictionary<String, Object> RowData { get; set; }
        public char Status { get; set; }

        public bool LatestVersion { get; set; }

        public EventParamOut EventParam;
    }

    public class EventParamOut
    {
        public String Msg { get; set; }
        public String Info { get; set; }
        public bool IsError
        {
            get;
            set;
        }



    }

    public class DeleteRecordIn
    {
        public List<int> RecIds { get; set; }
        public List<int> SingleKeyWhere { get; set; }
        public List<String> MultiKeyWhere { get; set; }
        public bool HasKeyColumn { get; set; }
        public int AD_Table_ID { get; set; }
        public String TableName { get; set; }
    }

    public class DeleteRecordOut
    {
        public bool IsError { get; set; }
        public string ErrorMsg { get; set; }
        public string InfoMsg { get; set; }
        public bool FireEEvent { get; set; }
        public bool FireIEvent { get; set; }
        public List<int> DeletedRecIds { get; set; }
        public List<int> RecIds { get; set; }
        public List<int> UnDeletedRecIds { get; set; }
        public EventParamOut EventParam;
    }

    public class WindowRecordOut
    {
        public List<JTable> Tables { get; set; }
        public Dictionary<string, Dictionary<object, string>> LookupDirect  { get;set; }
    }

    public class RecordInfoIn
    {
        public int CreatedBy
        { get; set; }

        public DateTime? Created
        { get; set; }

        public DateTime? Updated { get; set; }

        public int UpdatedBy { get; set; }

        public string Info { get; set; }

        public int AD_Table_ID { get; set; }

        public object Record_ID { get; set; }
    }

    public class RecordInfoOut
    {
        public bool ShowGrid { get; set; }
        public String Info { get; set; }
        public List<RecordRow> Rows { get; set; }
        public List<string> Headers { get; set; }
    }

    public class RecordRow
    {
        public string AD_Column_ID { get; set; }
        //public string ColumnName { get; set; }
        public string NewValue { get; set; }
        public string OldValue { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime Updated { get; set; }
    }


    public class CardViewData
    {
        public int FieldGroupID { get; set; }
        public List<int> IncludedCols { get; set; }
        public List<CardViewCondition> Conditions { get; set; }
        public int AD_CardView_ID { get; set; }
    }

    public class CardViewCondition
    {
        public string Color {get;set;}
        public string ConditionValue
        {
            get;
            set;
        }
        public string FColor { get; set; }
    }

}