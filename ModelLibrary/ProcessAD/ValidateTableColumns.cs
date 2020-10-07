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
        bool fromMenu = false;
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
                fromMenu = true;
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

                // Check on table whether it has single or multiple keys
                bool hasSingleKey = true;
                if (!table.IsSingleKey())
                    hasSingleKey = false;

                if (table == null || table.Get_ID() == 0)
                    return Msg.GetMsg(GetCtx(), "VIS_TableNotFound");

                // create HTML for tables
                sbHTML.Append("<div class='vis-val-tc-parCtr'> "
                                + "<div class='vis-val-tc-hdr'><label>" + Msg.Translate(GetCtx(), "AD_Table_ID") + ": </label>" + table.GetTableName() + "</div>"
                                + "<div class='vis-val-tc-colCtr'>"
                                    + "<div class='vis-val-tc-colHdrs'>" + Msg.GetMsg(GetCtx(), "VIS_ADCols") + "</div>"
                                    + "<div class='vis-val-tc-colHdrs'>" + Msg.GetMsg(GetCtx(), "VIS_DBCols") + "</div>"
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

                // check whether process is running from menu or from table and column Window
                // if process is executing from window then apply style
                string mainMenuWrap = "";
                if (!fromMenu)
                    mainMenuWrap = "style='max-height: 450px;'";

                sbHTML.Append("<div class='vis-val-tc-mainwrap' " + mainMenuWrap + "> ");

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
                        // Condition for Multikey columns and Single key to mark as Primary key columns
                        if ((hasSingleKey && col.GetAD_Reference_ID() == 13) || (!hasSingleKey && col.IsParent()))
                        {
                            keyCol = " * ";
                            style += " font-weight: bold; font-size: initial;";
                        }

                        sbHTML.Append("<div class='vis-val-tc-colWid'>"
                                       + "<div class='vis-val-tc-col-l' style='" + style + "'> " + keyCol + " " + col.GetColumnName() + " (" + drRef[0]["Name"] + " (" + col.GetFieldLength() + ")" + ") " + "</div>");
                        if (dr != null && dr.Length > 0)
                        {
                            // change done for PostgreSQL
                            if (DB.IsPostgreSQL())
                                sbHTML.Append("<div class='vis-val-tc-col-r' style='" + style + "'>" + Util.GetValueOfString(dr[0]["Column_Name"]) + " (" + Util.GetValueOfString(dr[0]["Data_Type"]) + ") " + "</div>");
                            else
                                sbHTML.Append("<div class='vis-val-tc-col-r' style='" + style + "'>" + Util.GetValueOfString(dr[0]["Column_Name"]) + " (" + Util.GetValueOfString(dr[0]["DataType"]) + " (" + Util.GetValueOfInt(dr[0]["LENGTH"]) + ") " + ") " + "</div>");
                        }
                        else
                        {
                            if (col.IsVirtualColumn())
                                sbHTML.Append("<div class='vis-val-tc-virCol' style='" + style + "'>" + Msg.GetMsg(GetCtx(), "VIS_VirtualCol") + "</div>");
                            else
                                sbHTML.Append("<div class='vis-val-tc-col-r' style='" + style + "'>" + Msg.GetMsg(GetCtx(), "VIS_DBNotFound") + "</div>");
                        }
                        sbHTML.Append("</div>");
                        ADCols.Add(adColName.ToString().ToLower());
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
                        if (ADCols.Contains(dbColName.ToString().ToLower()))
                            continue;
                        else
                        {
                            sbHTML.Append("<div class='vis-val-tc-colWid'>");

                            DataRow dr = null;
                            if (hasDBCols && dt.Tables[0].Rows.Count > 0)
                                dr = dt.Tables[0].Rows[d];

                            if (dr != null)
                            {
                                // change done for PostgreSQL.
                                if (DB.IsPostgreSQL())
                                    sbHTML.Append("<div class='vis-val-tc-col-red-r'>" + Util.GetValueOfString(dr["Column_Name"]) + " (" + Util.GetValueOfString(dr["Data_Type"]) + ")" + "</div>");
                                else
                                    sbHTML.Append("<div class='vis-val-tc-col-red-r'>" + Util.GetValueOfString(dr["Column_Name"]) + " (" + Util.GetValueOfString(dr["DATATYPE"]) + " (" + Util.GetValueOfInt(dr["LENGTH"]) + ") " + ") " + "</div>");
                            }
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

            return "";
        }

    }
}
