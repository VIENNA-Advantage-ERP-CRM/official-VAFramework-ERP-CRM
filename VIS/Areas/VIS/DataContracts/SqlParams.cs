using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;

/// <summary>
///   represent ADO Objects as class Propeties
/// </summary>
/// 

namespace VIS.DataContracts
{
   
    /// <summary>
    /// sql Parameter
    /// </summary>
   
    public class SqlParams
    {
        public string name { get; set; }
        public object value { get; set; }
        public bool isDate { get; set; }
        public bool isDateTime { get; set; }
        public bool isByteArray { get; set; }
    }

    /// <summary>
    /// sql parameter DataContact 
    /// --send from client
    /// </summary>
    public class SqlParamsIn
    {
        public int pageSize { get; set; }
        public int page { get; set; }
        public string sql { get; set; }
        public string increment { get; set; }

        public List<SqlParams> param { get; set; }
    }

    /// <summary>
    /// represent Json Data Table
    /// </summary>
    public class JTable
    {
        public int total = 0;
        public int page = 1;
        public int records = 0;
        public string name;
        public List<JColumn> columns = new List<JColumn>();
        public List<JRow> rows = new List<JRow>();
    }

    /// <summary>
    /// represent Json Data Row
    /// </summary>
    public class JRow
    {
        public object id;
        public Dictionary<string, object> cells = new Dictionary<string, object>();
    }

    /// <summary>
    /// represent Json Data Column
    /// </summary>
    public class JColumn
    {
        public string name;
        public string type;
        public int index;
    }
   
}