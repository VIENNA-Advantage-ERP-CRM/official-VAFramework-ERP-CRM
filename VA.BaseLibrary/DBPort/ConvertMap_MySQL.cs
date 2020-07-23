using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.DBPort
{
    class ConvertMap_MySQL
    {
        private static Dictionary<String, String> s_mysql = new Dictionary<String, String>();

        /// <summary>
        /// Return Map for MySql
        /// </summary>
        /// <returns>with pattern as key and the replacement as value</returns>
        public static Dictionary<String, String> GetConvertMap()
        {
            if (s_mysql.Count  == 0)
                InitConvertMap();
            return s_mysql;
        }   //  getConvertMap
        /// <summary>
        /// 
        ///  MySQL Init
        /// </summary>
        static private void InitConvertMap()
        {
            //      Oracle Pattern                     MySQL Replacement

            //  Data Types
            s_mysql.Add("\\bNUMBER[^\\(]\\b", "DECIMAL(22, 10) ");
            s_mysql.Add("\\bNUMBER[\\s]?\\(", "DECIMAL(");
            s_mysql.Add("\\bVARCHAR2\\b", "VARCHAR");
            s_mysql.Add("\\bNVARCHAR2\\b", "VARCHAR");
            s_mysql.Add("\\bNCHAR\\b", "CHAR");
            s_mysql.Add("\\bCLOB\\b", "TEXT");     // CLOB not directly supported

            // Reserved words
            s_mysql.Add("\\bLIMIT\\b", "`limit`");
            s_mysql.Add("\\bSEPARATOR\\b", "`separator`");
            s_mysql.Add("\\bUNDO\\b", "`undo`");
            s_mysql.Add("\\bLINES\\b", "`lines`");
            //s_mysql.put("\\bACTION\\b",                "`action`");

            //  Storage
            s_mysql.Add("\\bCACHE\\b", "");
            s_mysql.Add("\\bUSING INDEX\\b", "");
            s_mysql.Add("\\bTABLESPACE\\s\\w+\\b", "");
            s_mysql.Add("\\bSTORAGE\\([\\w\\s]+\\)", "");
            //
            s_mysql.Add("\\bBITMAP INDEX\\b", "INDEX");

            //  Functions
            s_mysql.Add("\\bSYSDATE\\b", "SysDate()");   //  alternative: NOW()
            s_mysql.Add("\\bUSER\\b", "user()");
            s_mysql.Add("\\bDUMP\\b", "MD5");
            s_mysql.Add("END CASE", "END");
            s_mysql.Add("\\bgetDate\\b\\(\\)", "CURRENT_TIMESTAMP");   //  alternative: NOW()
            s_mysql.Add("\\bNVL\\b", "COALESCE");
            s_mysql.Add("\\bTO_DATE\\b", "TO_DATE"); // POSTGRES has "TO_TIMESTAMP", MYSQL has "STR_TO_DATE". Trifon: Created to_date function for MySQL!
            //
            s_mysql.Add("\\bDBMS_OUTPUT.PUT_LINE\\b", ""); // There is no RAISE NOTICE
            s_mysql.Add("\\bTO_NCHAR\\b", "");

            //  Temporary
            s_mysql.Add("\\bGLOBAL TEMPORARY\\b", "TEMPORARY");
            s_mysql.Add("\\bON COMMIT DELETE ROWS\\b", "");
            s_mysql.Add("\\bON COMMIT PRESERVE ROWS\\b", "");

            //DDL

            s_mysql.Add("\\bCASCADE CONSTRAINTS\\b", "");

            //  Select
            s_mysql.Add("\\sFROM\\s+DUAL\\b", "");

            //  Statements
            s_mysql.Add("\\bELSIF\\b", "ELSEIF");
            s_mysql.Add("\\bREC \\b", "AS REC ");

            //  Sequences
            s_mysql.Add("\\bSTART WITH\\b", "START");
            s_mysql.Add("\\bINCREMENT BY\\b", "INCREMENT");
        }	
    }
}
