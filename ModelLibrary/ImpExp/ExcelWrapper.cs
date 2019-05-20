using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.ImpExp;
using System.IO;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Model;
using System.Data.SqlClient;
using VAdvantage.Utility;
using VAdvantage.Classes;
using VAdvantage.Login;
using System.ComponentModel;
using System.Web.Hosting;
using System.ServiceModel;

namespace VAdvantage.ImpExp
{
    public class ImportClientInfoModel
    {
        public List<int> m_MappedColumns;

        public List<int> m_MappedTables;

        public List<int> m_AllTables;

        public List<int> m_ExcelColIndex;

        public List<string> m_ExcelColName;

        public List<int> m_identifierColumn;

        public String FileName = "";

        public int MWindowID;

        public List<int> TabList;

        public Ctx ctx;

        public bool OnlySave;

    }
}
