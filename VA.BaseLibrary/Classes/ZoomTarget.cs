/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ZoomTarget
 * Purpose        : To get the list of table related with the current table for zoom window;
 * Class Used     : MLanguage,GlobalVariable
 * Chronological    Development
 * Raghunandan      9-April-2009 
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Windows.Forms;
using System.Collections;
using VAdvantage.Logging;

namespace VAdvantage.Classes
{
    public class ZoomTarget
    {

        /**	Static Logger	*/
        private static VLogger log = VLogger.GetVLogger(typeof(ZoomTarget).FullName);

        /// <summary>
        /// Get the Zoom Across Targets for a table.
        /// </summary>
        /// <param name="targetTableName">Target Table for zoom</param>
        /// <param name="curWindow_ID">Window from where zoom is invoked</param>
        /// <param name="targetWhereClause">Where Clause in the format "Record_ID=value"</param>
        /// <returns>Record list</returns>
        public static List<KeyNamePair> GetZoomTargets(String targetTableName, int curWindow_ID, String targetWhereClause,VAdvantage.Utility.Ctx ctx)
        {
            #region variables
            //The Option List					
            List<KeyNamePair> zoomList = new List<KeyNamePair>();
            List<WindowWhereClause> windowList = new List<WindowWhereClause>();
            ArrayList columns = new ArrayList();
            int zoom_Window_ID = 0;
            string PO_Window_ID;
            string zoom_WindowName = "";
            string whereClause = "";
            bool windowFound = false;
            //Context ctx = Utility.Env.GetContext();
            #endregion

            // Find windows where the first tab is based on the table
            string sql = "SELECT DISTINCT w.AD_Window_ID, w.Name, tt.WhereClause, t.TableName, " +
                    "wp.AD_Window_ID, wp.Name, ws.AD_Window_ID, ws.Name "
                + "FROM AD_Table t "
                + "INNER JOIN AD_Tab tt ON (tt.AD_Table_ID = t.AD_Table_ID) ";

            bool baseLanguage = Utility.Env.IsBaseLanguage(ctx, "");// GlobalVariable.IsBaseLanguage();
            if (baseLanguage)
            {
                sql += "INNER JOIN AD_Window w ON (tt.AD_Window_ID=w.AD_Window_ID)";
                sql += " LEFT OUTER JOIN AD_Window ws ON (t.AD_Window_ID=ws.AD_Window_ID)"
                    + " LEFT OUTER JOIN AD_Window wp ON (t.PO_Window_ID=wp.AD_Window_ID)";
            }
            else
            {
                sql += "INNER JOIN AD_Window_Trl w ON (tt.AD_Window_ID=w.AD_Window_ID AND w.AD_Language='" + Utility.Env.GetAD_Language(ctx) + "')";
                sql += " LEFT OUTER JOIN AD_Window_Trl ws ON (t.AD_Window_ID=ws.AD_Window_ID AND ws.AD_Language='" + Utility.Env.GetAD_Language(ctx) + "')"
                    + " LEFT OUTER JOIN AD_Window_Trl wp ON (t.PO_Window_ID=wp.AD_Window_ID AND wp.AD_Language='" + Utility.Env.GetAD_Language(ctx) + "')";
            }
            sql += "WHERE t.TableName ='" + targetTableName
                + "' AND w.AD_Window_ID <>" + curWindow_ID
                + " AND tt.SeqNo=10"
                + " AND (wp.AD_Window_ID IS NOT NULL "
                        + "OR EXISTS (SELECT 1 FROM AD_Tab tt2 WHERE tt2.AD_Window_ID = ws.AD_Window_ID AND tt2.AD_Table_ID=t.AD_Table_ID AND tt2.SeqNo=10))"
                + " ORDER BY 2";


            DataSet ds = null;
            try
            {
                ds = ExecuteQuery.ExecuteDataset(sql);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //rs = ds.Tables[0].Rows[i];
                    windowFound = true;
                    zoom_Window_ID = int.Parse(ds.Tables[0].Rows[i][6].ToString());
                    zoom_WindowName = ds.Tables[0].Rows[i][7].ToString();
                    PO_Window_ID = ds.Tables[0].Rows[i][4].ToString();
                    whereClause = ds.Tables[0].Rows[i][2].ToString();

                    // Multiple window support only for Order, Invoice, Shipment/Receipt which have PO windows
                    if (PO_Window_ID == null || PO_Window_ID.Length == 0)
                        break;

                    WindowWhereClause windowClause = new WindowWhereClause(int.Parse(ds.Tables[0].Rows[i][0].ToString()), ds.Tables[0].Rows[i][1].ToString(), whereClause);
                    windowList.Add(windowClause);
                }
                ds = null;
            }
            catch (Exception e)
            {
                // fill error log
                log.Log(Level.SEVERE, sql, e);
                //VAdvantage.//Common.////ErrorLog.FillErrorLog("ZoomTarget.GetZoomTargets", GlobalVariable.LAST_EXECUTED_QUERY, e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
            }
            string sql1 = "";

            if (!windowFound || (windowList.Count <= 1 && zoom_Window_ID == 0))
                return zoomList;
            //If there is a single window for the table, no parsing is neccessary
            if (windowList.Count <= 1)
            {
                //Check if record exists in target table
                sql1 = "SELECT count(*) FROM " + targetTableName + " WHERE " + targetWhereClause;
                if (whereClause != null && whereClause.Length != 0)
                {
                    sql1 += " AND " + Evaluator.ReplaceVariables(whereClause, ctx, null);
                }
            }
            else if (windowList.Count > 1)
            {
                // Get the columns used in the whereClause and stores in an arraylist
                for (int i = 0; i < windowList.Count; i++)
                {
                    ParseColumns(columns, windowList[i].whereClause);
                }

                // Get the distinct values of the columns from the table if record exists
                sql1 = "SELECT DISTINCT ";
                for (int i = 0; i < columns.Count; i++)
                {
                    if (i != 0)
                        sql1 += ",";
                    sql1 += columns[i].ToString();
                }

                if (columns.Count == 0)
                    sql1 += "count(*) ";
                sql1 += " FROM " + targetTableName + " WHERE " + targetWhereClause;
            }
            log.Fine(sql1);
            List<ValueNamePair> columnValues = new List<ValueNamePair>();

            try
            {
                ds = ExecuteQuery.ExecuteDataset(sql1);
                for (int cnt = 0; cnt < ds.Tables[0].Rows.Count; cnt++)
                {
                    if (columns.Count > 0)
                    {
                        columnValues.Clear();
                        // store column names with their values in the variable
                        for (int i = 0; i < columns.Count; i++)
                        {
                            String columnName = (String)columns[i].ToString();
                            String columnValue = (String)ds.Tables[0].Rows[cnt][columnName].ToString();
                            log.Fine(columnName + " = " + columnValue);
                            columnValues.Add(new ValueNamePair(columnValue, columnName));
                        }

                        // Find matching windows
                        for (int i = 0; i < windowList.Count; i++)
                        {
                            log.Fine("Window : " + windowList[i].windowName + " WhereClause : " + windowList[i].whereClause);
                            if (EvaluateWhereClause(columnValues, windowList[i].whereClause))
                            {
                                log.Fine("MatchFound : " + windowList[i].windowName);
                                KeyNamePair pp = new KeyNamePair(windowList[i].AD_Window_ID, windowList[i].windowName);
                                zoomList.Add(pp);
                                // Use first window found. Ideally there should be just one matching
                                break;
                            }
                        }
                    }
                    else
                    {
                        // get total number of records
                        int rowCount = int.Parse(ds.Tables[0].Rows[cnt][0].ToString());
                        if (rowCount != 0)
                        {
                            // make a key name pair
                            KeyNamePair pp = new KeyNamePair(zoom_Window_ID, zoom_WindowName);
                            zoomList.Add(pp);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // fill error log
                log.Log(Level.SEVERE, sql1, e);
                //VAdvantage.//Common.////ErrorLog.FillErrorLog("ZoomTarget.GetZoomTargets", GlobalVariable.LAST_EXECUTED_QUERY, e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
            }

            return zoomList;
        }

        /// <summary>
        /// Parse String and add columnNames to the list.
        /// String should be of the format ColumnName=<Value> AND ColumnName2=<Value2>
        /// </summary>
        /// <param name="list">list to be added to</param>
        /// <param name="parseString">string to parse for variables</param>
        public static void ParseColumns(ArrayList list, String parseString)
        {
            if (parseString == null || parseString.Length == 0)
                return;

            //	log.fine(parseString);
            String s = parseString;

            // Currently parsing algorithm does not handle parenthesis, IN clause or EXISTS clause
            if (s.Contains(" EXISTS ") || s.Contains(" IN ") || s.Contains("(") || s.Contains(")"))
                return;

            //  while we have columns
            while (s.IndexOf("=") != -1)
            {
                int endIndex = s.IndexOf("=");
                int beginIndex = s.LastIndexOf(' ', endIndex);
                int len = endIndex - (beginIndex + 1);

                //String variable = s.Substring(beginIndex + 1, endIndex);
                String variable = s.Substring(beginIndex + 1, len);

                if (variable.IndexOf(".") != -1)
                {
                    beginIndex = variable.IndexOf(".") + 1;
                    //variable = variable.Substring(beginIndex, endIndex);
                    len = endIndex - beginIndex;
                    variable = variable.Substring(beginIndex, len);
                }

                if (!list.Contains(variable))
                    list.Add(variable);

                s = s.Substring(endIndex + 1);
            }
        }

        /// <summary>
        /// Evaluate where clause
        /// </summary>
        /// <param name="columnValues">columns with the values</param>
        /// <param name="whereClause">where clause</param>
        /// <returns>bool type true if where clause evaluates to true</returns>
        public static bool EvaluateWhereClause(List<ValueNamePair> columnValues, String whereClause)
        {
            if (whereClause == null || whereClause.Length == 0)
                return true;

            string s = whereClause;
            bool result = true;

            // Currently parsing algorithm does not handle parenthesis, IN clause or EXISTS clause
            if (s.Contains(" EXISTS ") || s.Contains(" IN ") || s.Contains("(") || s.Contains(")"))
                return false;

            //  while we have variables
            while (s.IndexOf("=") != -1)
            {
                int endIndex = s.IndexOf("=");
                int beginIndex = s.LastIndexOf(' ', endIndex);

                string variable = s.Substring(beginIndex + 1, endIndex - (beginIndex + 1));
                string operand1 = "";
                string operand2 = "";
                string operator1 = "=";

                if (variable.IndexOf(".") != -1)
                {
                    beginIndex = variable.IndexOf(".")+1;
                    variable = variable.Substring(beginIndex);//, endIndex-beginIndex);
                }

                for (int i = 0; i < columnValues.Count; i++)
                {
                    if (variable.Equals(columnValues[i].GetName()))
                    {
                        operand1 = "'" + columnValues[i].GetValue() + "'";
                        break;
                    }
                }

                s = s.Substring(endIndex + 1);
                beginIndex = 0;
                endIndex = s.IndexOf(' ');
                if (endIndex == -1)
                    operand2 = s.Substring(beginIndex);
                else
                    operand2 = s.Substring(beginIndex, endIndex);

                /* log.fine("operand1:"+operand1+ 
                        " operator1:"+ operator1 +
                //        " operand2:"+operand2); */
                if (!Evaluator.EvaluateLogicTuple(operand1, operator1, operand2))
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Get the Zoom Across Targets for a table.
        /// </summary>
        /// <param name="targetTableName">for Target Table for zoom</param>
        /// <param name="curWindow_ID">Window from where zoom is invoked</param>
        /// <param name="targetWhereClause">Where Clause in the format "WHERE Record_ID=?"</param>
        /// <param name="paramsObj">paramsObj[] parameter to whereClause. Should be the Record_ID</param>
        /// <returns></returns>
        //public static List<KeyNamePair> GetZoomTargets(string targetTableName, int curWindow_ID, string targetWhereClause, object[] paramsObj)
        //{
        //    if (paramsObj.Length != 1)
        //    {
        //        return null;
        //    }
        //    int record_ID = (int)paramsObj[0];
        //    string whereClause = targetWhereClause.Replace("?", record_ID.ToString());
        //    whereClause = whereClause.Replace("WHERE ", " ");
        //    //MessageBox.Show("WhereClause : " + whereClause);
        //    return GetZoomTargets(targetTableName, curWindow_ID, whereClause,null);
        //}

        /// <summary>
        /// Get the Zoom Into Target for a table.
        /// </summary>
        /// <param name="targetTableName">for Target Table for zoom</param>
        /// <param name="curWindow_ID">Window from where zoom is invoked</param>
        /// <param name="targetWhereClause">Where Clause in the format "Record_ID=<value>"</param>
        /// <param name="isSOTrx">Sales contex of window from where zoom is invoked</param>
        /// <returns>PO_zoomWindow_ID</returns>
        public static int GetZoomAD_Window_ID(string targetTableName, int curWindow_ID, string targetWhereClause, bool isSOTrx,VAdvantage.Utility.Ctx ctx )
        {
            int zoomWindow_ID = 0;
            int PO_zoomWindow_ID = 0;
            // Find windows where the first tab is based on the table
            string sql = "SELECT DISTINCT AD_Window_ID, PO_Window_ID "
                + "FROM AD_Table t "
                + "WHERE TableName ='" + targetTableName + "'";
            IDataReader dr = null;
            try
            {
                //DataSet ds = null;
                //ds = ExecuteQuery.ExecuteDataset(sql);

                dr = ExecuteQuery.ExecuteReader(sql);
                while (dr.Read())
                {
                    zoomWindow_ID = Utility.Util.GetValueOfInt(dr["AD_Window_ID"].ToString());
                    if (dr["PO_Window_ID"] != null && dr["PO_Window_ID"].ToString().Length > 0)
                        PO_zoomWindow_ID = Utility.Util.GetValueOfInt(dr["PO_Window_ID"].ToString());
                }
                dr.Close();

            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                // fill error log
                log.Severe(e.ToString());
                //VAdvantage.//Common.////ErrorLog.FillErrorLog("ZoomTarget.GetZoomAD_Window_ID", GlobalVariable.LAST_EXECUTED_QUERY, e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
            }
            

            if (PO_zoomWindow_ID == 0)
                return zoomWindow_ID;

            int AD_Window_ID = 0;

            if (targetWhereClause != null && targetWhereClause.Length != 0)
            {
                List<KeyNamePair> zoomList = new List<KeyNamePair>();
                zoomList = ZoomTarget.GetZoomTargets(targetTableName, curWindow_ID, targetWhereClause,ctx);
                if (zoomList != null && zoomList.Count > 0)
                    AD_Window_ID = zoomList[0].GetKey();
            }
            if (AD_Window_ID != 0)
            {
                return AD_Window_ID;
            }

            if (isSOTrx)
            {
                return zoomWindow_ID;
            }

            return PO_zoomWindow_ID;
        }

    }

    /// <summary>
    /// Window WhereClause
    /// </summary>
    public class WindowWhereClause
    {

        //Context ctx = Utility.Env.GetContext();
        public int AD_Window_ID = 0;//Window	
        public String windowName = "";// Window Name
        public String whereClause = "";// Window Where Clause

        /// <summary>
        ///	Org Access constructor
        /// </summary>
        /// <param name="ad_Window_ID">window</param>
        /// <param name="name">Window Name</param>
        /// <param name="where">Where Clause on the first tab of the window </param>
        public WindowWhereClause(int ad_Window_ID, String name, String where)
        {
            this.AD_Window_ID = ad_Window_ID;
            this.windowName = name;
            this.whereClause = where;
        }

        /// <summary>
        /// Extended String Representation
        /// </summary>
        /// <returns>extended info</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            //sb.append(Msg.translate(GetCtx(), "AD_Window_ID")).append("=")
            //sb.Append(Msg.translate(ctx.GetContext(), "AD_Window_ID")).Append("=")
            //  .Append(windowName).Append(" - ")
            // .Append(whereClause);
            return sb.ToString();
        }

    }
}
