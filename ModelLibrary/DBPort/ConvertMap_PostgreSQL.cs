using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.DBPort
{
    class ConvertMap_PostgreSQL
    {
        private static Dictionary<String, String> s_pg = new Dictionary<String, String>();

        /// <summary>
        /// Return Map for PostgreSQL
        /// </summary>
        /// <returns>with pattern as key and the replacement as value</returns>
        public static Dictionary<String, String> GetConvertMap()
        {

            if (s_pg.Count == 0)
                InitConvertMap();
            return s_pg;
        }   //  getConvertMap

        /// <summary>
        ///PostgreSQL Init
        /// </summary>
        static private void InitConvertMap()
        {
            //      Oracle Pattern                  Replacement

            //  Data Types
            s_pg.Add("\\bNUMBER\\b", "NUMERIC");
            s_pg.Add("\\bDATE\\b", "TIMESTAMP");
            s_pg.Add("\\bVARCHAR2\\b", "VARCHAR");
            s_pg.Add("\\bNVARCHAR2\\b", "VARCHAR");
            s_pg.Add("\\bNCHAR\\b", "CHAR");
            //begin vpj-cd e-evolution 03/11/2005 PostgreSQL
            s_pg.Add("\\bBLOB\\b", "BYTEA");                 //  BLOB not directly supported
            s_pg.Add("\\bCLOB\\b", "TEXT");                //  CLOB not directly supported
            //s_pg.Add("\\bLIMIT\\b", "\"limit\"");
           // s_pg.Add("\\bACTION\\b", "\"action\"");
            s_pg.Add("\\bold\\b", "\"old\"");
            s_pg.Add("\\bnew\\b", "\"new\"");
            //s_pg.Add("\\bBLOB\\b",                  "OID");                 //  BLOB not directly supported
            //s_pg.Add("\\bCLOB\\b",                  "OID");                //  CLOB not directly supported
            //end vpj-cd e-evolution 03/11/2005 PostgreSQL

            //  Storage
            s_pg.Add("\\bCACHE\\b", "");
            s_pg.Add("\\bUSING INDEX\\b", "");
            s_pg.Add("\\bTABLESPACE\\s\\w+\\b", "");
            s_pg.Add("\\bSTORAGE\\([\\w\\s]+\\)", "");
            //
            s_pg.Add("\\bBITMAP INDEX\\b", "INDEX");

            //  Functions
            s_pg.Add("\\bSYSDATE\\b", "CURRENT_TIMESTAMP");   //  alternative: NOW()
            //begin vpj-cd e-evolution 03/11/2005 PostgreSQL		                                     
            s_pg.Add("\\bDUMP\\b", "MD5");
            s_pg.Add("END CASE", "END");
            s_pg.Add("\\bgetDate\\b\\(\\)", "CURRENT_TIMESTAMP");   //  alternative: NOW()
            //end vpj-cd e-evolution 03/11/2005 PostgreSQL
            s_pg.Add("\\bNVL\\b", "COALESCE");
            s_pg.Add("\\bTO_DATE\\b", "TO_TIMESTAMP");
            //
            s_pg.Add("\\bDBMS_OUTPUT.PUT_LINE\\b", "RAISE NOTICE");
            s_pg.Add("\\bTO_NCHAR\\b", "");

            //  Temporary
            s_pg.Add("\\bGLOBAL TEMPORARY\\b", "TEMPORARY");
            s_pg.Add("\\bON COMMIT DELETE ROWS\\b", "");
            s_pg.Add("\\bON COMMIT PRESERVE ROWS\\b", "");

            //DDL

            // begin vpj-cd e-evolution 08/02/2005 PostgreSQL
            //s_pg.Add("\\bMODIFY\\b","ALTER COLUMN");						
            //s_pg.Add("\\bDEFAULT\\b","SET DEFAULT");
            // end vpj-cd e-evolution 08/02/2005 PostgreSQL

            //  DROP TABLE x CASCADE CONSTRAINTS
            s_pg.Add("\\bCASCADE CONSTRAINTS\\b", "");

            //  Select
            s_pg.Add("\\sFROM\\s+DUAL\\b", "");

            //  Statements
            s_pg.Add("\\bELSIF\\b", "ELSE IF");
            // begin vpj-cd e-evolution 03/11/2005 PostgreSQL
            s_pg.Add("\\bREC \\b", "AS REC ");
            // end vpj-cd e-evolution 03/11/2005 PostgreSQL

            //  Sequences
            s_pg.Add("\\bSTART WITH\\b", "START");
            s_pg.Add("\\bINCREMENT BY\\b", "INCREMENT");


        }
    }
}
