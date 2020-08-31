using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using VAdvantage.DataBase;

namespace MarketSvc.InstallModule
{
    /// <summary>
    /// Summary description for DBConnection
    /// </summary>
    public class DBConnection
    {
        public DBConnection()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public static string CreateConnectionString(string dbHostIP, string dbUserName, string dbUserPswd, string dbPortNo, string dbServiceName)
        {
            if (dbPortNo == null || dbPortNo.Equals(string.Empty))
            {
                dbPortNo = "1521";
            }
            if (dbServiceName == null || dbServiceName.Equals(string.Empty))
            {
                dbServiceName = "XE";
            }
            if (dbHostIP != null && dbUserName != null && dbUserPswd != null && dbServiceName != null && dbPortNo != null)
            {
                //Ini.connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + dbHostIP + ")(PORT=" + dbPortNo + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + dbServiceName + ")));User Id=" + dbUserName + ";Password=" + dbUserPswd;
                Ini.connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + dbHostIP + ")(PORT=" +
                    dbPortNo + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + dbServiceName + ")));User Id=" + dbUserName +
                    ";Password=" + dbUserPswd + "; Min Pool Size = 3; Max Pool Size = 5";
            }
            //else
            //{
            //    Ini.connectionString = WebConfigurationManager.AppSettings["oracleConnectionString"];
            //}
            VConnection conn = VConnection.Get();
            conn.Db_uid = dbUserName;
            conn.Db_pwd = dbUserPswd;


            return Ini.connectionString;
        }
    }
}
