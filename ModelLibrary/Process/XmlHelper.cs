using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Utility;
using System.IO;

namespace System.Runtime.CompilerServices
{
    public class ExtensionAttribute : Attribute { }
}


namespace VAdvantage.Process
{

    public static class XmlHelper
    {



        /// <summary>
        /// Generates Xml files from a supplied dataset
        /// </summary>
        /// <param name="ds">DataSource of xml file</param>
        /// <param name="path">Path where Xml files are to be saved</param>
        /// <returns>Status of the process</returns>
        public static string ToXml(this DataSet ds, String path)
        {
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                DataTable table = ds.Tables[i];
                table.WriteXml(Path.Combine(path, table.TableName + ".xml"), XmlWriteMode.WriteSchema);
            }

            if (ds.Tables.Count <= 0)
                return Msg.GetMsg(Env.GetContext(), "NoRecordAvailable");

            return Msg.GetMsg(Env.GetContext(), "XmlGenerated");
        }

        /// <summary>
        /// Adds or Copies new reocrd in dataset
        /// </summary>
        /// <param name="ds">Base dataset</param>
        /// <param name="tmpDs">Dataset whose records are to be saved in Base dataset</param>
        /// <param name="tableName">Name of the table which is to be imported</param>
        /// <param name="recordid">Primary key value of the row</param>
        public static void AddOrCopy(this DataSet ds, DataSet tmpDs, string tableName, int recordid, int AD_colone_ID, string[] dss, int rowNum)
        {
            if (ds.Tables.Contains(tableName))
            {
                String s = tableName;
                if (s.Equals("AD_Ref_Table"))
                    s = "AD_Reference";

                DataRow[] copyRows = tmpDs.Tables[0].Select();
                int Pid = 0;
                int AColID = 0;

                foreach (DataRow copyRow in copyRows)
                {
                    if (AD_colone_ID <= 0)
                    {
                        Pid = Convert.ToInt32(copyRow[s + "_ID"].ToString());

                        DataRow[] find = ds.Tables[s].Select(s + "_ID = " + Pid);

                        if (find.Count() <= 0)
                            ds.Tables[tableName].ImportRow(copyRow);
                    }
                    else
                    {
                        Pid = Convert.ToInt32(copyRow[dss[0]].ToString());
                        AColID = Convert.ToInt32(copyRow[dss[1]].ToString());
                        DataRow[] find = ds.Tables[s].Select(dss[0] + " =" + Pid + " and " + dss[1] + " =" + AColID);
                        if (find.Count() <= 0)
                            ds.Tables[tableName].ImportRow(copyRow);
                    }
                }

            }
            else
            {
                tmpDs.Tables[0].TableName = tableName;
                ds.Tables.Add(tmpDs.Tables[0].Copy());
            }

            DataRow newrow = ds.Tables["ExportTableSequence"].NewRow();
            newrow[0] = rowNum;
            newrow[1] = tableName;
            newrow[2] = recordid;
            if (AD_colone_ID > 0)
            {
                newrow[3] = AD_colone_ID;
            }

            //DataRow[] rows = ds.Tables["ExportTableSequence"].Select("TableName = '" + tableName + "' AND Record_ID = " + recordid);

            //foreach (DataRow dr in rows)
            //{

            //    dr.Delete();
            //}

            ds.Tables["ExportTableSequence"].Rows.Add(newrow);
        }

        /// <summary>
        /// Gets the value of the column of a particular row filtered by primary key
        /// </summary>
        /// <param name="ds">DataSet from where value is to be selected</param>
        /// <param name="TableName">Name of the table in a dataset</param>
        /// <param name="ColumnName">Name of the column whose value is to be picked</param>
        /// <param name="Key">Primary Key of the table</param>
        /// <returns>Column value as Object</returns>
        public static Object Get(this DataSet ds, String TableName, String ColumnName, int Key)
        {
            DataRow[] rows = ds.Tables[TableName].Select(TableName + "_ID = " + Key);
            if (rows.Count() > 0)
            {
                return rows[0][ColumnName];
            }

            return null;
        }
    }
}
