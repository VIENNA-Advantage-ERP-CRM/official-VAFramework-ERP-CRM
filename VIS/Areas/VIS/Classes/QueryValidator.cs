using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VIS.Classes
{
    public class QueryValidator
    {
        private static List<string> Keyword = new List<string>()
            {
            "DROP","DELETE","V$SESSION","V$INSTANCE","UNION","SESSION","UPDATE","INSERT","TRUNCATE","--","/*",
            "ALL_TABLES","ALL_TAB_COLUMNS",
            };

        public static bool IsValid(string sql)
        {
            if (string.IsNullOrEmpty(sql))
                return true;
            foreach (string key in Keyword)
            {
                if (sql.ToUpper().IndexOf(key) > -1)
                    return false;
            }

            return true;
        }

    }
}