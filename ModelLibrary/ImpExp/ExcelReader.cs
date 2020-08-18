using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Data;
using System.Data.OleDb;
using System.Text;

namespace VAdvantage.ImpExp
{
    public class ExcelReader : ExcelHelper
    {
        #region "  Constructors  "

        public ExcelReader()
        { }

        public ExcelReader(int startRow, int startCol)
            : base(startRow, startCol)
        { }

        #endregion


        private bool mReadAllAsText = false;

        public bool ReadAllAsText
        {
            get { return mReadAllAsText; }
            set { mReadAllAsText = value; }
        }

        private void ValidatePropertiesForExtract()
        {

            if (this.StartRow <= 0)
                throw new Exception("The StartRow Property is Invalid. Must be Greater or Equal Than 1.");

            //            if (this.StartRow > mDtExcel.Rows.Count)
            //                throw new BadUsageException("The StartRow Property is Invalid. Must be Less or Equal to Worksheet row's count.");

            if (this.StartColumn <= 0)
                throw new Exception("The StartColumn Property is Invalid. Must be Greater or Equal Than 1.");

            //            if (this.StartColumn > mDtExcel.Columns.Count)
            //                throw new BadUsageException("The StartColumn Property is Invalid. Must be Less or Equal To Worksheet Column's count.");

        }

        private string m_file;
        public String[] GetSheets()
        {
            OleDbConnection connExcel = null;
            DataTable dt = null;
            try
            {
                connExcel = new OleDbConnection(CreateConnectionString(m_file));
                connExcel.Open();
                dt = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (dt == null)
                {
                    return null;
                }

                String[] excelSheets = new String[dt.Rows.Count];
                int i = 0;

                // Add the sheet name to the string array.

                foreach (DataRow row in dt.Rows)
                {
                    excelSheets[i] = row["TABLE_NAME"].ToString();
                    i++;
                }

                // Loop through all of the sheets if you want too...

                for (int j = 0; j < excelSheets.Length; j++)
                {
                    // Query each excel sheet.

                }

                return excelSheets;
            }
            catch
            {
                return null;
            }
            finally
            {
                // Clean up.

                if (connExcel != null)
                {
                    connExcel.Close();
                    connExcel.Dispose();
                }
                if (dt != null)
                {
                    dt.Dispose();
                }
            }
        }


        public bool TestConnection(string file)
        {

            ValidatePropertiesForExtract();

            OleDbConnection connExcel;
            //private OleDbDataAdapter mDaExcel;

            connExcel = new OleDbConnection(CreateConnectionString(file));
            connExcel.Open();

            m_file = file;
            if (connExcel.State == ConnectionState.Open)
            {
                connExcel.Close();
                return true;
            }
            else
            {
                return false;
            }

        }

        public DataTable ExtractDataTable(string file, string sheetName)
        {
            m_SheetName = sheetName;
            return ExtractDataTable(file, StartRow, StartColumn);
        }

        private string m_SheetName = "";
        public DataTable ExtractDataTable(string file, int row, int col)
        {
            OleDbConnection connExcel = null;
            try
            {
                ValidatePropertiesForExtract();

               
                //private OleDbDataAdapter mDaExcel;

                connExcel = new OleDbConnection(CreateConnectionString(file));
                connExcel.Open();
                DataTable res = new DataTable();
                
                string sheetName = GetFirstSheet(connExcel);

                if (!sheetName.Equals(m_SheetName))
                {
                    m_SheetName = sheetName;
                }

                string sheet = sheetName + (sheetName.EndsWith("$") ? "" : "$");
                string command = String.Format("SELECT * FROM [" + m_SheetName + "]", sheet);

                OleDbCommand cm = new OleDbCommand(command, connExcel);
                OleDbDataAdapter da = new OleDbDataAdapter(cm);
                da.Fill(res);

                connExcel.Close();
                return res;
            }
            catch 
            {
                if(connExcel!=null)
                    connExcel.Close();
                //VAdvantage.Classes.ShowMessage.Error(ex.Message, false);
                return null;
            }

        }

        protected override string ExtraProps()
        {
            if (mReadAllAsText)
                return " IMEX=1;";

            return string.Empty;
        }
    }
}