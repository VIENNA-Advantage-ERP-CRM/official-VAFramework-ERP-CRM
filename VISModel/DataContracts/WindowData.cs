using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIS.DataContracts
{
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
    }

}
