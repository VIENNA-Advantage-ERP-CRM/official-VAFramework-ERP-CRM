/********************************************************
 * Module Name    : Process
 * Purpose        : check table and columns between database and AD
 * Chronological Development
 * Lokesh Chauhan     10-Dec-2019
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;

namespace VAdvantage.Process
{
    public class ValidateTableColumns : SvrProcess
    {
        #region Private Variables
        private string p_AD_Table_ID = "";
        List<string> standardColumns = new List<string>();
        private StringBuilder sbHTML = new StringBuilder("");
        #endregion Private Variables

        /// <summary>
        /// Process parameters
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {

                }
                else if (name == "AD_Table_ID")
                {
                    p_AD_Table_ID = para[i].GetParameter().ToString();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }

            if (p_AD_Table_ID.Trim().ToString() == "")
            {
                p_AD_Table_ID = GetRecord_ID().ToString();
            }

            standardColumns.Add("AD_CLIENT_ID");
            standardColumns.Add("AD_ORG_ID");
            standardColumns.Add("CREATED");
            standardColumns.Add("CREATEDBY");
            standardColumns.Add("EXPORT_ID");
            standardColumns.Add("ISACTIVE");
            standardColumns.Add("UPDATED");
            standardColumns.Add("UPDATEDBY");
        }

        /// <summary>
        /// Process logic to verify database columns and Columns in Application Dictionary
        /// </summary>
        /// <returns></returns>
        protected override string DoIt()
        {
            // if table not found, in case if run from menu 
            // or from table, then return with message
            if (p_AD_Table_ID.Trim() == "")
            {
                return Msg.GetMsg(GetCtx(), "VIS_TableNotSelected");
            }

            DataSet dsDT = DB.ExecuteDataset("SELECT AD_Reference_ID, Name FROM AD_Reference WHERE IsActive = 'Y' AND ValidationType = 'D'");

            string[] tableIDs = p_AD_Table_ID.Split(',');

            // loop through tables selected in parameter or 
            // if run from window then single table ID
            for (int i = 0; i < tableIDs.Length; i++)
            {
                int AD_Table_ID = Util.GetValueOfInt(tableIDs[i]);
                MTable table = MTable.Get(GetCtx(), AD_Table_ID);
                if (table == null || table.Get_ID() == 0)
                    return Msg.GetMsg(GetCtx(), "VIS_TableNotFound");

                // create HTML for tables
                sbHTML.Append("<div> "
                                + "<div style='width: 100%; width: 100%; font-size: x-large; font-weight: 700; font-family: sans-serif; margin-bottom: 10px;'>" + table.GetTableName() + "</div>"
                                + "<div style='width: 100%; height: 30px; border-bottom: 2px solid 2px solid #d4d4dc;'>"
                                    + "<div style='width: 50%; float:left; min-width: 300px; font-weight: bold;font-size: large;'>" + Msg.GetMsg(GetCtx(), "VIS_ADCols") + "</div>"
                                    + "<div style='width: 50%; float:right; min-width: 300px; font-weight: bold;font-size: large;'>" + Msg.GetMsg(GetCtx(), "VIS_DBCols") + "</div>"
                                + "</div>");

                //	Find Column in Database
                DatabaseMetaData md = new DatabaseMetaData();
                String catalog = "";
                String schema = DataBase.DB.GetSchema();

                //get table name
                string tableName = table.GetTableName();
                //get columns of a table
                DataSet dt = md.GetColumns(catalog, schema, tableName);
                bool hasDBCols = false;
                if (dt.Tables[0].Rows.Count > 0)
                    hasDBCols = true;

                // get all columns from table
                MColumn[] columnsAD = table.GetColumns(true);

                // variables to create HTML as string for both AD and in DB
                StringBuilder adColName = new StringBuilder("");
                StringBuilder dbColName = new StringBuilder("");

                List<string> ADCols = new List<string>();

                sbHTML.Append("<div class='vis-val-tc-mainwrap'> ");

                if (columnsAD.Length > 0)
                {
                    // loop through Columns present in Application dictionary
                    for (int c = 0; c < columnsAD.Length; c++)
                    {
                        var col = columnsAD[c];

                        adColName.Clear();
                        adColName.Append(col.GetColumnName().ToUpper());

                        DataRow[] dr = null;
                        if (hasDBCols && dt.Tables[0].Rows.Count > 0)
                            dr = dt.Tables[0].Select("COLUMN_NAME = '" + adColName.ToString() + "'");

                        DataRow[] drRef = dsDT.Tables[0].Select("AD_Reference_ID = " + col.GetAD_Reference_ID());

                        // if column is virtual, then add style
                        var style = " color: red; font-style: italic;";
                        if (col.IsVirtualColumn() || (dr != null && dr.Length > 0))
                            style = " color: green;";

                        // add different style for key column
                        string keyCol = "";
                        if (col.GetAD_Reference_ID() == 13)
                        {
                            keyCol = " * ";
                            style += " font-weight: bold; font-size: initial;";
                        }

                        sbHTML.Append("<div style='width: 100%;'>"
                                       + "<div class='vis-val-tc-col-l' style='" + style + "'> " + keyCol + " " + col.GetColumnName() + " (" + drRef[0]["Name"] + " (" + col.GetFieldLength() + ") " + " ) " + "</div>");
                        if (dr != null && dr.Length > 0)
                            sbHTML.Append("<div class='vis-val-tc-col-r' style='" + style + "'>" + Util.GetValueOfString(dr[0]["Column_Name"]) + " (" + Util.GetValueOfString(dr[0]["DATATYPE"]) + " (" + Util.GetValueOfInt(dr[0]["LENGTH"]) + ") " + " ) " + "</div>");
                        else
                        {
                            if (col.IsVirtualColumn())
                                sbHTML.Append("<div style='width: 50%; float:right; min-width: 300px;font-weight: 600; font-style: italic; text-overflow: ellipsis;  overflow: hidden;  white-space: nowrap;" + style + "'>" + Msg.GetMsg(GetCtx(), "VIS_VirtualCol") + "</div>");
                            else
                                sbHTML.Append("<div class='vis-val-tc-col-r' style='" + style + "'>" + Msg.GetMsg(GetCtx(), "VIS_DBNotFound") + "</div>");
                        }
                        sbHTML.Append("</div>");
                        ADCols.Add(adColName.ToString());
                    }
                }

                // if columns present in database
                if (hasDBCols)
                {
                    // loop through columns in DB
                    for (int d = 0; d < dt.Tables[0].Rows.Count; d++)
                    {
                        dbColName.Clear();
                        dbColName.Append(dt.Tables[0].Rows[d]["Column_Name"]);
                        if (ADCols.Contains(dbColName.ToString()))
                            continue;
                        else
                        {
                            sbHTML.Append("<div style='width: 100%;'>");

                            DataRow dr = null;
                            if (hasDBCols && dt.Tables[0].Rows.Count > 0)
                                dr = dt.Tables[0].Rows[d];

                            if (dr != null)
                                sbHTML.Append("<div class='vis-val-tc-col-red-r'>" + Util.GetValueOfString(dr["Column_Name"]) + " (" + Util.GetValueOfString(dr["DATATYPE"]) + " (" + Util.GetValueOfInt(dr["LENGTH"]) + ") " + " ) " + "</div>");
                            else
                                sbHTML.Append("<div class='vis-val-tc-col-red-r'>" + Msg.GetMsg(GetCtx(), "VIS_ADNotFound") + "</div>");

                            sbHTML.Append("<div class='vis-val-tc-col-red-l'>" + Msg.GetMsg(GetCtx(), "VIS_ADNotFound") + "</div>");

                            sbHTML.Append("</div>");
                        }
                    }
                }

                sbHTML.Append("</div>");

                sbHTML.Append("</div>");

                md.Dispose();

                ProcessInfo pi = GetProcessInfo();
                pi.SetCustomHTML(sbHTML.ToString());
            }

            return "All OK";
        }

    }
}
