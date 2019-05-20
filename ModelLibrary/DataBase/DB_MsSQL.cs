using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.DataBase
{
    class DB_MSSQL:ViennaDatabase
    {
        public string GetName()
        {
            return DatabaseType.DB_MSSQL;
        }

        public int GetStandardPort()
        {
            throw new NotImplementedException();
        }

        public bool SupportsBLOB()
        {
            throw new NotImplementedException();
        }

        public string ConvertStatement(string oraStatement)
        {
            throw new NotImplementedException();
        }

        public string TO_DATE(DateTime? time, bool dayOnly)
        {
            //if (time == null)
            //{
            //    if (dayOnly)
            //        return "CAST(STR(YEAR(Getdate()))+'-'+STR(Month(Getdate()))+'-'+STR(Day(Getdate())) AS DATETIME)";
            //    return "getdate()";
            //}

            //StringBuilder dateString = new StringBuilder("CAST('");
            ////  YYYY-MM-DD HH24:MI:SS.mmmm  JDBC Timestamp format
            //myDate = time.ToString();
            //if (dayOnly)
            //{
            //    dateString.Append(myDate.Substring(0, 10));
            //    dateString.Append("' AS DATETIME)");
            //}
            //else
            //{
            //    dateString.Append(myDate.Substring(0, myDate.IndexOf(".")));	//	cut off miliseconds
            //    dateString.Append("' AS DATETIME)");
            //}
            return "";
        }

        public string TO_CHAR(string columnName, int displayType, string AD_Language)
        {
            throw new NotImplementedException();
        }

        public string TO_NUMBER(decimal? number, int displayType)
        {
            throw new NotImplementedException();
        }

        public bool CreateSequence(string name, int increment, int minvalue, int maxvalue, int start, Trx trxName)
        {
            throw new NotImplementedException();
        }

        public System.Data.IDbConnection GetCachedConnection(bool autoCommit, int transactionIsolation)
        {
            throw new NotImplementedException();
        }

        public void SetConnectionString(string conString)
        {
            throw new NotImplementedException();
        }


        public System.Data.DataSet ExecuteDatasetPaging(string sql, int page, int pageSize, int increment)
        {
            throw new NotImplementedException();
        }
    }
}
