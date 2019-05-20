using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Data;
using System.Data.OleDb;
using System.Text;

namespace VAdvantage.ImpExp
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class ExcelHelper
    {
        public string CreateConnectionString(string file)
        {
            string extProps = string.Empty;

            extProps += ExtraProps();

            if (this.HasHeaders)
                extProps += " HDR=YES;";
            else
                extProps += " HDR=NO;";


            return String.Format(file.EndsWith("xlsx") ? xlsxconn : mConnectionString, file, extProps);
        }

        protected virtual string ExtraProps()
        {
            return string.Empty;
        }

        #region "  Constructors  "

        protected ExcelHelper()
        { }

        protected ExcelHelper(int startRow, int startCol)
        {
            mStartColumn = startCol;
            mStartRow = startRow;
        }

        #endregion

        #region "  Private Fields  "

        private string mSheetName = String.Empty;

        static string xlsxconn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;{1}\"";
 
        private static string mConnectionString = "Provider=Microsoft.Jet.OleDb.4.0;Data Source={0};Extended Properties=\"Excel 8.0;{1}\"";

        private int mStartRow = 1;
        private int mStartColumn = 1;

        private bool mHasHeaders = true;

        #endregion

        #region "  Public Properties  "

        /// <summary>The Start Row where is the data. Starting at 1.</summary>
        public int StartRow
        {
            get { return mStartRow; }
            set { mStartRow = value; }
        }

        /// <summary>The Start Column where is the data. Starting at 1.</summary>
        public int StartColumn
        {
            get { return mStartColumn; }
            set { mStartColumn = value; }
        }

        /// <summary>Indicates if the sheet has a header row.</summary>
        public bool HasHeaders
        {
            get { return mHasHeaders; }
            set { mHasHeaders = value; }
        }

        /// <summary>The Excel Sheet Name, if empty means the current worksheet in the file.</summary>
        public string SheetName
        {
            get { return mSheetName; }
            set { mSheetName = value; }
        }


        #endregion


        internal static string GetFirstSheet(OleDbConnection conn)
        {
#if NET_2_0
			    DataTable dt = conn.GetSchema("Tables");
#else
            DataTable dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
#endif

            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0]["TABLE_NAME"].ToString();
            else
                throw new Exception("A Valid sheet was not found in the workbook.");
        }


    }
}