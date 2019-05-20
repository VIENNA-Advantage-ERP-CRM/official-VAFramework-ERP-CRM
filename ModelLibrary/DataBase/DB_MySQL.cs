using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.DataBase
{
    class DB_MySQL :ViennaDatabase
    {

        private string connectionString = null;
        private System.Data.IDbConnection con = null;

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

        

        public string TO_CHAR(string columnName, int displayType, string AD_Language)
        {
            throw new NotImplementedException();
        }

        public string TO_NUMBER(decimal? number, int displayType)
        {
            throw new NotImplementedException();
        }

        public int GetNextID(string Name)
        {
            throw new NotImplementedException();
        }

        public bool CreateSequence(string name, int increment, int minvalue, int maxvalue, int start, Trx trxName)
        {
            throw new NotImplementedException();
        }



        public string GetName()
        {
            return DatabaseType.DB_MYSQL;
        }









        public System.Data.IDbConnection GetCachedConnection(bool autoCommit, int transactionIsolation)
        {
            if (con == null)
            {
                con = new MySqlConnection(connectionString);
            }
            return con;
        }

        public void SetConnectionString(string conString)
        {
            connectionString = conString;
        }


        public string TO_DATE(DateTime? time, bool dayOnly)
        {
            throw new NotImplementedException();
        }


        public System.Data.DataSet ExecuteDatasetPaging(string sql, int page, int pageSize, int increment)
        {
            throw new NotImplementedException();
        }
    }
}
