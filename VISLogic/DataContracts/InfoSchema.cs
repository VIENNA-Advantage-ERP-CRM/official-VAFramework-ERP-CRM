using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.Model;

namespace VIS.DataContracts
{

    public class InfoSchema
    {

        public string Name
        {
            get;
            set;
        }
        public int AD_Reference_ID
        {
            get;
            set;
        }
        public int AD_Reference_Value_ID
        {
            get;
            set;
        }

        public bool IsQueryCriteria
        {
            get;
            set;
        }
        public string SelectClause
        {
            get;
            set;
        }
        public string SetValue
        {
            get;
            set;
        }
        public string Condition
        {
            get;
            set;
        }
        public string ColumnName
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public bool IsDisplayed
        {
            get;
            set;
        }
        public bool IsKey
        {
            get;
            set;
        }
        public bool IsRange
        {
            get;
            set;
        }
        public bool ISIDENTIFIER
        {
            get;
            set;
        }

        public List<InfoRefList> RefList
        {
            get;
            set;
        }
        public Lookup lookup
        {
            get;
            set;
        }

    }

    public class InfoRefList
    {
        public string Key
        {
            get;
            set;
        }
        public string Value
        {
            get;
            set;
        }
    }


    //public class DisplayContent
    //{

    //    //public DisplayContent(string columnName, string header, bool isDisplayed, bool isKeyColumn, int displayType)
    //    //{
    //    //    ColumnName = columnName;
    //    //    Header = header;
    //    //    IsDisplayed = isDisplayed;
    //    //    IsKeyColumn = isKeyColumn;
    //    //    DisplayType = displayType;
    //    //}
    //    public string ColumnName
    //    {
    //        get;
    //        set;
    //    }
    //    public bool IsDisplayed
    //    {
    //        get;
    //        set;
    //    }
    //    public bool IsKeyColumn
    //    {
    //        get;
    //        set;
    //    }
    //    public string Header
    //    {
    //        get;
    //        set;
    //    }
    //    public int DisplayType
    //    {
    //        get;
    //        set;
    //    }
    //}
    public class Info
    {
        public int ID
        {
            get;
            set;
        }

        public string WindowName
        {
            get;
            set;
        }
        public string TableName
        {
            get;
            set;
        }
        public string FromClause
        {
            get;
            set;
        }
        public string OTHERCLAUSE
        {
            get;
            set;
        }
        public List<InfoSchema> Schema
        {
            get;
            set;
        }

    }

    public class InfoData
    {
        public List<DataObject> data
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
    public class DataObject
    {
        public string ColumnName
        {
            get;
            set;
        }
        public string Header
        {
            get;
            set;
        }
        public int RowCount
        {
            get;
            set;
        }
        public List<object> Values
        {
            get;
            set;
        }
    }
    public class PageSetting
    {
        public int CurrentPage
        {
            get;
            set;
        }
        public int TotalPage
        {
            get;
            set;
        }
        public int TotalRecords
        {
            get;
            set;
        }
        public int PageSize
        {
            get;
            set;
        }
    }
}
