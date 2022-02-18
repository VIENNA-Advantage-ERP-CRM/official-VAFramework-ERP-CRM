using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Classes;

/// <summary>
/// window Data Contract To pass between Client -Server
/// </summary>
/// 
namespace VIS.DataContracts
{

    /// <summary>
    /// 
    /// </summary>
    public class SaveRecordIn
    {
        public int Record_ID { get; set; }
        public int AD_Table_ID { get; set; }
        public int AD_WIndow_ID { get; set; }

        public int AD_Client_ID { get; set; }
        public int AD_Org_ID { get; set; }

        public bool CompareDB { get; set; }
        public bool Inserting { get; set; }

        public string TableName { get; set; }

        public string WhereClause { get; set; }
        public String SelectSQL { get; set; }

        public Dictionary<String, Object> RowData { get; set; }
        public Dictionary<String, Object> OldRowData { get; set; }

        public bool ManualCmd { get; set; }

        public List<WindowField> Fields { get; set; }
        public int TreeID { get; set; }
        public int ParentNodeID { get; set; }
        public int TreeTableID { get; set; }
        public int SelectedTreeNodeID { get; set; }

        public bool MaintainVersions { get; set; }

        public bool ImmediateSave { get; set; }

        public DateTime? ValidFrom { get; set; }

        public int VerRecID { get; set; }

        public List<string> UnqFields { get; set; }
    }

    public class WindowField
    {
        public bool IsVirtualColumn { get; set; }
        public int DisplayType { get; set; }
        public string ColumnSQL { get; set; }
        public string ColumnName { get; set; }
        public bool IsKey { get; set; }
        public bool IsEncryptedColumn { get; set; }
        public bool IsParentColumn { get; set; }
        public string Name { get; set; }
        public bool IsObscure { get; set; }
    }

    public class SaveRecordOut
    {
        public bool IsError { get; set; }

        public bool IsWarning { get; set; }
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
        public CardViewData CardViewTpl { get; set; }
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


   

}