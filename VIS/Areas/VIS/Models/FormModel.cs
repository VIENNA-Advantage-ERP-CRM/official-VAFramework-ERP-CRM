using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.DataContracts;
using VIS.Helpers;

namespace VIS.Models
{
    public class FormModel
    {
        Ctx _ctx = new Ctx();
        public FormModel(Ctx ctx)
        {
            _ctx = ctx;
        }

        public List<JTable> GetProcessedRequest(int AD_Table_ID, int Record_ID)
        {
            string m_where = "(AD_Table_ID=" + AD_Table_ID + " AND Record_ID=" + Record_ID + ")";

            if (AD_Table_ID == 114)
            {// MUser.Table_ID){
                m_where += " OR AD_User_ID=" + Record_ID + " OR SalesRep_ID=" + Record_ID;
            }
            else if (AD_Table_ID == 291)
            {//MBPartner.Table_ID){
                m_where += " OR C_BPartner_ID=" + Record_ID;
            }
            else if (AD_Table_ID == 259)
            {// MOrder.Table_ID){
                m_where += " OR C_Order_ID=" + Record_ID;
            }
            else if (AD_Table_ID == 318)
            {//MInvoice.Table_ID){
                m_where += " OR C_Invoice_ID=" + Record_ID;
            }
            else if (AD_Table_ID == 335)
            {// MPayment.Table_ID){
                m_where += " OR C_Payment_ID=" + Record_ID;
            }
            else if (AD_Table_ID == 208)
            {//MProduct.Table_ID){
                m_where += " OR M_Product_ID=" + Record_ID;
            }
            else if (AD_Table_ID == 203)
            {//MProject.Table_ID){
                m_where += " OR C_Project_ID=" + Record_ID;
            }
            else if (AD_Table_ID == 539)
            {// MAsset.Table_ID){
                m_where += " OR A_Asset_ID=" + Record_ID;
            }
            string sql = "SELECT Processed, COUNT(*) "
                  + "FROM R_Request WHERE " + m_where
                  + " GROUP BY Processed "
                  + "ORDER BY Processed DESC";

            SqlParamsIn sqlP = new SqlParamsIn();
            sqlP.sql = sql;

            VIS.Helpers.SqlHelper help = new Helpers.SqlHelper();
            return help.ExecuteJDataSet(sqlP);
        }

        public List<string> GetZoomTargets(string tableName)
        {
            List<string> zoomTargets = new List<string>();
            string sql = "SELECT DISTINCT t.AD_Table_ID, t.TableName "
              + "FROM AD_Table t "
              + "WHERE EXISTS (SELECT 1 FROM AD_Tab tt "
                  + "WHERE tt.AD_Table_ID = t.AD_Table_ID AND tt.SeqNo=10) "
              + " AND t.AD_Table_ID IN "
                  + "(SELECT AD_Table_ID FROM AD_Column "
                  + "WHERE ColumnName='" + tableName + "_ID') "
              + "AND TableName NOT LIKE 'I\\_%'"
              + "AND TableName NOT LIKE '" + tableName + "' "
              + "ORDER BY 1";

            var dr = DB.ExecuteReader(sql);
            if (dr != null)
            {
                while (dr.Read())
                {
                    zoomTargets.Add(dr["TableName"].ToString());
                }

                dr.Close();
                dr = null;
            }
            return zoomTargets;

        }

        /// <summary>
        /// Parse String and add columnNames to the list.
        /// String should be of the format ColumnName=<Value> AND ColumnName2=<Value2>
        /// </summary>
        /// <param name="list">list to be added to</param>
        /// <param name="parseString">string to parse for variables</param>
        public static void ParseColumns(List<Object> list, String parseString)
        {
            if (parseString == null || parseString.Length == 0)
                return;

            //	//log.fine(parseString);
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
                    len = variable.Length - beginIndex;
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
            // while (s.IndexOf("=") != -1)
            string[] varibles = s.Split(' ');

            for(int o=0;o< varibles.Length;o++)
            {
                s = varibles[o];
                if (s.IndexOf("=") == -1)
                    continue;
                int endIndex = s.IndexOf("=");
                int beginIndex = s.LastIndexOf(' ', endIndex);

                string variable = s.Substring(beginIndex + 1, endIndex - (beginIndex + 1));
                string operand1 = "";
                string operand2 = "";
                string operator1 = "=";

                if (variable.IndexOf(".") != -1)
                {
                    beginIndex = variable.IndexOf(".") + 1;
                    endIndex = variable.Length - beginIndex;
                    variable = variable.Substring(beginIndex, endIndex);
                }

                for (int i = 0; i < columnValues.Count; i++)
                {
                    if (variable.Equals(columnValues[i].GetName()))
                    {
                        operand1 = "'" + columnValues[i].GetValue() + "'";
                        break;
                    }
                }
               
                beginIndex = 0;
                

                beginIndex = s.IndexOf('=') + 1;
                endIndex = s.Length - beginIndex;
                //endIndex = endIndex - beginIndex;
                if (endIndex == -1)
                    operand2 = s.Substring(beginIndex);
                else
                    operand2 = s.Substring(beginIndex, endIndex);


                /* //log.fine("operand1:"+operand1+ 
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
        /// Get the Zoom Into Target for a table.
        /// </summary>
        /// <param name="targetTableName">for Target Table for zoom</param>
        /// <param name="curWindow_ID">Window from where zoom is invoked</param>
        /// <param name="targetWhereClause">Where Clause in the format "Record_ID=<value>"</param>
        /// <param name="isSOTrx">Sales contex of window from where zoom is invoked</param>
        /// <returns>PO_zoomWindow_ID</returns>
        public int GetZoomAD_Window_ID(string targetTableName, int curWindow_ID, string targetWhereClause, bool isSOTrx)
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

                dr = DB.ExecuteReader(sql);
                while (dr.Read())
                {
                    zoomWindow_ID = Util.GetValueOfInt(dr["AD_Window_ID"].ToString());
                    if (dr["PO_Window_ID"] != null && dr["PO_Window_ID"].ToString().Length > 0)
                        PO_zoomWindow_ID = Util.GetValueOfInt(dr["PO_Window_ID"].ToString());
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
                //VAdvantage.Common.ErrorLog.FillErrorLog("ZoomTarget.GetZoomAD_Window_ID", GlobalVariable.LAST_EXECUTED_QUERY, e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
            }


            if (PO_zoomWindow_ID == 0)
                return zoomWindow_ID;

            int AD_Window_ID = 0;

            if (targetWhereClause != null && targetWhereClause.Length != 0)
            {
                List<KeyNamePair> zoomList = new List<KeyNamePair>();
                zoomList = ZoomTarget.GetZoomTargets(targetTableName, curWindow_ID, targetWhereClause, _ctx);
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

        /// <summary>
        /// Get the Zoom Across Targets for a table.
        /// </summary>
        /// <param name="targetTableName">Target Table for zoom</param>
        /// <param name="curWindow_ID">Window from where zoom is invoked</param>
        /// <param name="targetWhereClause">Where Clause in the format "Record_ID=value"</param>
        /// <returns>Record list</returns>
        public List<KeyNamePair> GetZoomTargets(String targetTableName, int curWindow_ID, String targetWhereClause)
        {
            #region variables
            //The Option List					
            List<KeyNamePair> zoomList = new List<KeyNamePair>();
            List<WindowWhereClause> windowList = new List<WindowWhereClause>();
            List<Object> columns = new List<Object>();
            int ZoomWindow_ID = 0;
            string PO_Window_ID;
            string zoom_WindowName = "";
            string whereClause = "";
            bool windowFound = false;
            Ctx ctx = Env.GetContext();
            #endregion

            // Find windows where the first tab is based on the table
            string sql = "SELECT DISTINCT w.AD_Window_ID, w.Name, tt.WhereClause, t.TableName, " +
                    "wp.AD_Window_ID, wp.Name, ws.AD_Window_ID, ws.Name "
                + "FROM AD_Table t "
                + "INNER JOIN AD_Tab tt ON (tt.AD_Table_ID = t.AD_Table_ID) ";

            bool baseLanguage = Env.IsBaseLanguage(ctx, "");// GlobalVariable.IsBaseLanguage();
            if (baseLanguage)
            {
                sql += "INNER JOIN AD_Window w ON (tt.AD_Window_ID=w.AD_Window_ID)";
                sql += " LEFT OUTER JOIN AD_Window ws ON (t.AD_Window_ID=ws.AD_Window_ID)"
                    + " LEFT OUTER JOIN AD_Window wp ON (t.PO_Window_ID=wp.AD_Window_ID)";
            }
            else
            {
                sql += "INNER JOIN AD_Window_Trl w ON (tt.AD_Window_ID=w.AD_Window_ID AND w.AD_Language='" + Env.GetAD_Language(ctx) + "')";
                sql += " LEFT OUTER JOIN AD_Window_Trl ws ON (t.AD_Window_ID=ws.AD_Window_ID AND ws.AD_Language='" + Env.GetAD_Language(ctx) + "')"
                    + " LEFT OUTER JOIN AD_Window_Trl wp ON (t.PO_Window_ID=wp.AD_Window_ID AND wp.AD_Language='" + Env.GetAD_Language(ctx) + "')";
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
                ds = DB.ExecuteDataset(sql, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //rs = ds.Tables[0].Rows[i];
                    windowFound = true;
                    ZoomWindow_ID = int.Parse(ds.Tables[0].Rows[i][6].ToString());
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
                //log.Log(Level.SEVERE, sql, e);
                //VAdvantage.Common.ErrorLog.FillErrorLog("ZoomTarget.GetZoomTargets", GlobalVariable.LAST_EXECUTED_QUERY, e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
            }
            string sql1 = "";

            if (!windowFound || (windowList.Count <= 1 && ZoomWindow_ID == 0))
                return zoomList;
            //If there is a single window for the table, no parsing is neccessary
            if (windowList.Count <= 1)
            {
                //Check if record exists in target table
                sql1 = "SELECT count(*) FROM " + targetTableName + " WHERE " + targetWhereClause;
                if (whereClause != null && whereClause.Length != 0)
                {
                    sql1 += " AND " + Evaluator.ReplaceVariables(whereClause, Env.GetContext(), null);
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
            //log.Fine(sql1);
            List<ValueNamePair> columnValues = new List<ValueNamePair>();

            try
            {
                ds = DB.ExecuteDataset(sql1, null);
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
                            //log.Fine(columnName + " = " + columnValue);
                            columnValues.Add(new ValueNamePair(columnValue, columnName));
                        }

                        // Find matching windows
                        for (int i = 0; i < windowList.Count; i++)
                        {
                            //log.Fine("Window : " + windowList[i].windowName + " WhereClause : " + windowList[i].whereClause);
                            if (EvaluateWhereClause(columnValues, windowList[i].whereClause))
                            {
                                //log.Fine("MatchFound : " + windowList[i].windowName);
                                KeyNamePair pp = new KeyNamePair(windowList[i].AD_Window_ID, windowList[i].windowName);
                                zoomList.Add(pp);
                                // Use first window found. Ideally there should be just one matching

                                //this break is remove by karan on 18 jan 2021, to show a record which can exist in more than one window.
                               //break;
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
                            KeyNamePair pp = new KeyNamePair(ZoomWindow_ID, zoom_WindowName);
                            zoomList.Add(pp);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // fill error log
                //log.Log(Level.SEVERE, sql1, e);
                //VAdvantage.Common.ErrorLog.FillErrorLog("ZoomTarget.GetZoomTargets", GlobalVariable.LAST_EXECUTED_QUERY, e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
            }

            return zoomList;
        }


        public int GetWorkflowWindowID(int AD_Table_ID)
        {
            object windowID = DB.ExecuteScalar("SELECT AD_Window_ID FROM AD_Table WHERE AD_Table_ID=" + AD_Table_ID);
            if (windowID != null && windowID != DBNull.Value)
            {
                return Convert.ToInt32(windowID);
            }
            return 0;
        }

        public List<JTable> GetZoomWindowID(int AD_Table_ID)
        {
            SqlParamsIn sqlP = new SqlParamsIn();
            sqlP.sql = "SELECT TableName, AD_Window_ID, PO_Window_ID FROM AD_Table WHERE AD_Table_ID=" + AD_Table_ID;

            VIS.Helpers.SqlHelper help = new Helpers.SqlHelper();
            return help.ExecuteJDataSet(sqlP);
        }

        public List<JTable> GetZoomTargetClass(Ctx ctx, string targetTableName, int curWindow_ID)
        {
            string sql = "SELECT DISTINCT w.AD_Window_ID, w.Name, tt.WhereClause, t.TableName, " +
                              "wp.AD_Window_ID, wp.Name, ws.AD_Window_ID, ws.Name "
                          + "FROM AD_Table t "
                          + "INNER JOIN AD_Tab tt ON (tt.AD_Table_ID = t.AD_Table_ID) ";
            var baseLanguage = Env.IsBaseLanguage(ctx, "AD_Window");
            if (baseLanguage)
            {
                sql += "INNER JOIN AD_Window w ON (tt.AD_Window_ID=w.AD_Window_ID)";
                sql += " LEFT OUTER JOIN AD_Window ws ON (t.AD_Window_ID=ws.AD_Window_ID)"
                    + " LEFT OUTER JOIN AD_Window wp ON (t.PO_Window_ID=wp.AD_Window_ID)";
            }
            else
            {
                sql += "INNER JOIN AD_Window_Trl w ON (tt.AD_Window_ID=w.AD_Window_ID AND w.AD_Language=@para1)";
                sql += " LEFT OUTER JOIN AD_Window_Trl ws ON (t.AD_Window_ID=ws.AD_Window_ID AND ws.AD_Language=@para2)"
                    + " LEFT OUTER JOIN AD_Window_Trl wp ON (t.PO_Window_ID=wp.AD_Window_ID AND wp.AD_Language=@para3)";
            }
            sql += "WHERE t.TableName = @para4"
                + " AND w.AD_Window_ID <> @para5 AND w.isActive='Y'"
                + " AND tt.SeqNo=10"
                + " AND (wp.AD_Window_ID IS NOT NULL "
                        + "OR EXISTS (SELECT 1 FROM AD_Tab tt2 WHERE tt2.AD_Window_ID = ws.AD_Window_ID AND tt2.AD_Table_ID=t.AD_Table_ID AND tt2.SeqNo=10))"
                + " ORDER BY 2";

            List<SqlParams> param = new List<SqlParams>();
            if (!baseLanguage)
            {
                param.Add(new SqlParams() { name = "@para1", value = Env.GetAD_Language(Env.GetCtx()) });
                param.Add(new SqlParams() { name = "@para2", value = Env.GetAD_Language(Env.GetCtx()) });
                param.Add(new SqlParams() { name = "@para3", value = Env.GetAD_Language(Env.GetCtx()) });
            }
            param.Add(new SqlParams() { name = "@para4", value = targetTableName });
            param.Add(new SqlParams() { name = "@para5", value = curWindow_ID });

            SqlParamsIn sqlP = new SqlParamsIn();
            sqlP.sql = sql;
            sqlP.param = param;
            VIS.Helpers.SqlHelper help = new Helpers.SqlHelper();
            return help.ExecuteJDataSet(sqlP);
        }

        public string GetAccessSql(string _columnName, string text)
        {
            string sql = "";
            if (_columnName.Equals("M_Product_ID"))
            {
                sql += "SELECT DISTINCT p.M_Product_ID FROM M_Product p LEFT OUTER JOIN M_Manufacturer mr ON (p.M_Product_ID=mr.M_Product_ID)" +
                    " LEFT OUTER JOIN M_ProductAttributes patr ON (p.M_Product_ID=patr.M_Product_ID) WHERE (UPPER(p.Value) LIKE " + DB.TO_STRING(text) +
                    " OR UPPER(p.Name) LIKE " + DB.TO_STRING(text) + " OR UPPER(mr.UPC) LIKE " + DB.TO_STRING(text) +
                    " OR UPPER(patr.UPC) LIKE " + DB.TO_STRING(text) + " OR UPPER(p.UPC) LIKE " + DB.TO_STRING(text) + ")";
            }
            else if (_columnName.Equals("C_BPartner_ID"))
            {
                sql += "SELECT C_BPartner_ID FROM C_BPartner WHERE (UPPER(Value) LIKE ";
                sql += DB.TO_STRING(text) + " OR UPPER(Name) LIKE " + DB.TO_STRING(text) + ")";
            }
            else if (_columnName.Equals("C_Order_ID"))
            {
                sql += "SELECT C_Order_ID FROM C_Order WHERE UPPER(DocumentNo) LIKE ";
                sql += DB.TO_STRING(text);
            }
            else if (_columnName.Equals("C_Invoice_ID"))
            {
                sql += "SELECT C_Invoice_ID FROM C_Invoice WHERE UPPER(DocumentNo) LIKE ";
                sql += DB.TO_STRING(text);
            }
            else if (_columnName.Equals("M_InOut_ID"))
            {
                sql += "SELECT M_InOut_ID FROM M_InOut WHERE UPPER(DocumentNo) LIKE ";
                sql += DB.TO_STRING(text);
            }
            else if (_columnName.Equals("C_Payment_ID"))
            {
                sql += "SELECT C_Payment_ID FROM C_Payment WHERE UPPER(DocumentNo) LIKE ";
                sql += DB.TO_STRING(text);
            }
            else if (_columnName.Equals("GL_JournalBatch_ID"))
            {
                sql += "SELECT GL_JournalBatch_ID FROM GL_JournalBatch WHERE UPPER(DocumentNo) LIKE ";
                sql += DB.TO_STRING(text);
            }
            else if (_columnName.Equals("SalesRep_ID"))
            {
                sql += "SELECT AD_User_ID FROM AD_User WHERE UPPER(Name) LIKE ";
                sql += DB.TO_STRING(text);
            }
            return sql;
        }
        /// <summary>
        /// autocomplete search 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="_columnName"></param>
        /// <param name="text"></param>
        /// <param name="sql"></param>
        /// <returns>DataSet</returns>
        /// Mandeep Singh(VIS0028) 13-sep-2021
        public DataSet GetAccessSqlAutoComplete(Ctx ctx, string _columnName, string text,string sql)
        {
            sql = VIS.Classes.SecureEngineBridge.DecryptByClientKey(sql, ctx.GetSecureKey());
            int idx = sql.IndexOf("finalValue");
            string lastPart = "";
            if (idx != -1) {
                lastPart = sql.Substring(idx, sql.Length - idx);
                int newIndex = lastPart.IndexOf("WHERE");
                newIndex = newIndex + 5;
                lastPart = lastPart.Substring(newIndex, lastPart.Length - newIndex);
                sql = sql.Replace(lastPart,"");

            }
            bool isColumnMatch = false;
            if (_columnName.Equals("M_Product_ID"))
            {
                isColumnMatch = true;
                sql += " (UPPER(M_Product.Value) LIKE " + DB.TO_STRING(text) +
                    " OR UPPER(M_Product.Name) LIKE " + DB.TO_STRING(text) + ")";
                sql += " AND ";
            }
            else if (_columnName.Equals("C_BPartner_ID"))
            {
                isColumnMatch = true;
                sql += " (UPPER(Value) LIKE ";
                sql += DB.TO_STRING(text) + " OR UPPER(Name) LIKE " + DB.TO_STRING(text) + ")";
                sql += " AND ";
            }
            else if (_columnName.Equals("C_Order_ID"))
            {
                isColumnMatch = true;
                sql += " UPPER(DocumentNo) LIKE ";
                sql += DB.TO_STRING(text);
                sql += " AND ";
            }
            else if (_columnName.Equals("C_Invoice_ID"))
            {
                isColumnMatch = true;
                sql += " UPPER(DocumentNo) LIKE ";
                sql += DB.TO_STRING(text);
                sql += " AND ";
            }
            else if (_columnName.Equals("M_InOut_ID"))
            {
                isColumnMatch = true;
                sql += " UPPER(DocumentNo) LIKE ";
                sql += DB.TO_STRING(text);
                sql += " AND ";
            }
            else if (_columnName.Equals("C_Payment_ID"))
            {
                isColumnMatch = true;
                sql += " UPPER(DocumentNo) LIKE ";
                sql += DB.TO_STRING(text);
                sql += " AND ";
            }
            else if (_columnName.Equals("GL_JournalBatch_ID"))
            {
                isColumnMatch = true;
                sql += " UPPER(DocumentNo) LIKE ";
                sql += DB.TO_STRING(text);
                sql += " AND ";
            }
            else if (_columnName.Equals("SalesRep_ID"))
            {
                isColumnMatch = true;
                sql += " UPPER(Name) LIKE ";
                sql += DB.TO_STRING(text);
                sql += " AND ";
            }
            if (isColumnMatch) {
                sql += lastPart;
            }
            else {
                sql += lastPart;
                sql = DBFunctionCollection.convertToSubQuery(sql, "*") + "WHERE UPPER(finalvalue) LIKE " + DB.TO_STRING(text);
            }
            DataSet ds = VIS.DBase.DB.ExecuteDatasetPaging(sql, 1, 1000);
            if (ds != null)
            {
                ds.Tables[0].TableName = "Table";
            }
            return ds;

        }


        public List<JTable> GetWareProWiseLocator(Ctx ctx, string colName, int orgId, int warehouseId, int productId, bool onlyIsSOTrx)
        {
            string sql = "SELECT M_Locator_ID," + colName + " FROM M_Locator WHERE IsActive='Y'";
            //JID_0932 In validation of locator need to consider organization  
            if (orgId != 0)
            {
                sql += " AND AD_Org_ID=@org";
            }
            if (warehouseId != 0)
            {
                sql += " AND M_Warehouse_ID=@w";
            }
            if (productId != 0)
            {
                sql += " AND (IsDefault='Y' OR EXISTS (SELECT * FROM M_Storage s WHERE s.M_Locator_ID=M_Locator.M_Locator_ID AND s.M_Product_ID=@p)";
                if (onlyIsSOTrx)
                {
                    //	Default Product
                    sql += "OR EXISTS (SELECT * FROM M_Product p WHERE p.M_Locator_ID=M_Locator.M_Locator_ID AND p.M_Product_ID=@p)OR EXISTS (SELECT * FROM M_ProductLocator pl " +
                        " WHERE pl.M_Locator_ID=M_Locator.M_Locator_ID AND pl.M_Product_ID=@p) OR 0 = (SELECT COUNT(*) FROM M_ProductLocator pl " +
                        " INNER JOIN M_Locator l2 ON (pl.M_Locator_ID=l2.M_Locator_ID) WHERE pl.M_Product_ID=@p AND l2.M_Warehouse_ID=M_Locator.M_Warehouse_ID )";
                }
                sql += ")";
            }

            var finalSql = MRole.GetDefault(ctx).AddAccessSQL(sql, "M_Locator", MRole.SQL_NOTQUALIFIED, MRole.SQL_RO);
            List<SqlParams> param = new List<SqlParams>();
            if (orgId != 0)
            {
                param.Add(new SqlParams() { name = "@org", value = orgId });
            }
            if (warehouseId != 0)
            {
                param.Add(new SqlParams() { name = "@w", value = warehouseId });
            }
            if (productId != 0)
            {
                param.Add(new SqlParams() { name = "@p", value = productId });
            }



            SqlParamsIn sqlP = new SqlParamsIn();
            sqlP.sql = sql;
            sqlP.param = param;
            VIS.Helpers.SqlHelper help = new Helpers.SqlHelper();
            return help.ExecuteJDataSet(sqlP);
        }


        public List<JTable> GetValidAccountCombination(int AD_Client_ID, bool onlyActive)
        {
            string sql = "SELECT C_ValidCombination_ID, Combination, Description "
            + "FROM C_ValidCombination WHERE AD_Client_ID=" + AD_Client_ID;
            if (onlyActive)
                sql += " AND IsActive='Y'";
            sql += " ORDER BY 2";
            SqlParamsIn sqlP = new SqlParamsIn();
            sqlP.sql = sql;
            VIS.Helpers.SqlHelper help = new Helpers.SqlHelper();
            return help.ExecuteJDataSet(sqlP);
        }

        public List<JTable> GenAttributeSetInstance(int C_GenAttributeSet_ID, bool onlyActive)
        {
            var sql = "SELECT ASI.C_GenAttributeSetInstance_ID, ASI.Description " +
         " from C_GenAttributeSetInstance ASI, M_Product P WHERE ASI.C_GenAttributeSet_ID  = " + C_GenAttributeSet_ID;
            if (onlyActive)
                sql += " AND IsActive='Y'";
            sql += " ORDER BY 2";
            SqlParamsIn sqlP = new SqlParamsIn();
            sqlP.sql = sql;
            VIS.Helpers.SqlHelper help = new Helpers.SqlHelper();
            return help.ExecuteJDataSet(sqlP);
        }

        public string GetDocWhere(int AD_User_ID, string TableName)
        {
            string docAccess = "(EXISTS (SELECT 1 FROM C_BPartner bp INNER JOIN AD_User u "
            + "ON (u.C_BPartner_ID=bp.C_BPartner_ID) "
            + " WHERE u.AD_User_ID="
            + AD_User_ID
            + " AND bp.C_BPartner_ID="
            + TableName
            + ".C_BPartner_ID)"
            + " OR EXISTS (SELECT 1 FROM C_BP_Relation bpr INNER JOIN AD_User u "
            + "ON (u.C_BPartner_ID=bpr.C_BPartnerRelation_ID) "
            + " WHERE u.AD_User_ID="
            + AD_User_ID
            + " AND bpr.C_BPartner_ID=" + TableName + ".C_BPartner_ID)";

            var hasUserColumn = false;
            var sql1 = "SELECT count(*) FROM AD_Table t "
                + "INNER JOIN AD_Column c ON (t.AD_Table_ID=c.AD_Table_ID) "
                + "WHERE t.tableName='" + TableName
                + "' AND c.ColumnName='AD_User_ID' ";

            int ret = Util.GetValueOfInt(DB.ExecuteScalar(docAccess));
            hasUserColumn = ret > 0 ? true : false;

            if (hasUserColumn)
                docAccess += " OR " + TableName + ".AD_User_ID =" + AD_User_ID;
            docAccess += ")";
            return docAccess;
        }


        public string UpdateTree(List<int> oldParentChildren, List<int> newParentChildren, int oldId, int newId, int AD_Tree_ID, string tableName)
        {
            string sql;
            for (var i = 0; i < oldParentChildren.Count; i++)
            {
                //StringBuilder sql = new StringBuilder("UPDATE ");
                sql = "UPDATE " + tableName + " SET Parent_ID=" + oldId + ", SeqNo=" + i + ", Updated=SysDate" +
                                  " WHERE AD_Tree_ID=" + AD_Tree_ID + " AND Node_ID=" + oldParentChildren[i];
                //log.Fine(sql.ToString());
                DB.ExecuteQuery(sql);
            }

            if (oldId != newId)
            {
                // childs = newParent.children();
                /// i = 0;
                for (var i = 0; i < newParentChildren.Count; i++)
                {
                    sql = "UPDATE " + tableName + " SET Parent_ID=" + newId +
                    ", SeqNo=" + i + ", Updated=SysDate" +
                     " WHERE AD_Tree_ID=" + AD_Tree_ID +
                      " AND Node_ID=" + newParentChildren[i];
                    DB.ExecuteQuery(sql);
                }
            }

            return "";
        }


        #region AReport
        public List<JTable> GetPrintFormats(int AD_Table_ID, int AD_Tab_ID)
        {
            string sql = "SELECT AD_PrintFormat_ID, Name, AD_Client_ID "
                       + "FROM AD_PrintFormat "
                       + "WHERE AD_Table_ID='" + AD_Table_ID + "' AND IsTableBased='Y' ";
            if (AD_Tab_ID > 0)
            {
                sql = sql + " AND AD_Tab_ID='" + AD_Tab_ID + "' ";
            }
            sql = sql + "ORDER BY AD_Client_ID DESC, IsDefault DESC, Name";	//	Own First
            sql = MRole.GetDefault(_ctx).AddAccessSQL(sql,		//	Own First
                   "AD_PrintFormat", MRole.SQL_NOTQUALIFIED, MRole.SQL_RO);

            SqlParamsIn sqlP = new SqlParamsIn();
            sqlP.sql = sql;
            VIS.Helpers.SqlHelper help = new Helpers.SqlHelper();
            return help.ExecuteJDataSet(sqlP);

        }

        public List<JTable> GetShowReportDetails(int AD_Table_ID, int AD_Tab_ID)
        {
            string sql = "SELECT AD_PrintFormat_ID, Name, Description,IsDefault "
                               + "FROM AD_PrintFormat "
                               + "WHERE AD_Table_ID=" + AD_Table_ID;
            if (AD_Tab_ID > 0)
            {
                sql = sql + " AND AD_Tab_ID=" + AD_Tab_ID;
            }
            sql = sql + " ORDER BY Name";
            sql = MRole.GetDefault(_ctx).AddAccessSQL(sql, "AD_PrintFormat", MRole.SQL_NOTQUALIFIED, MRole.SQL_RO);

            SqlParamsIn sqlP = new SqlParamsIn();
            sqlP.sql = sql;
            VIS.Helpers.SqlHelper help = new Helpers.SqlHelper();
            return help.ExecuteJDataSet(sqlP);
        }
        #endregion

        #region Controller

        public List<JTable> GetTrxInfo(int Record_ID, bool isOrder)
        {
            StringBuilder sql = new StringBuilder("SELECT COUNT(*) AS Lines,c.ISO_Code,o.TotalLines,o.GrandTotal,"
                        + "CURRENCYBASEWITHCONVERSIONTYPE(o.GrandTotal,o.C_Currency_ID,o.DateAcct, o.AD_Client_ID,o.AD_Org_ID, o.C_CONVERSIONTYPE_ID) AS ConvAmt ");
            if (isOrder)
            {
                sql.Append("FROM C_Order o"
                        + " INNER JOIN C_Currency c ON (o.C_Currency_ID=c.C_Currency_ID)"
                        + " INNER JOIN C_OrderLine l ON (o.C_Order_ID=l.C_Order_ID) "
                        + "WHERE o.C_Order_ID=" + Record_ID + "");
            }
            else
            {
                sql.Append("FROM C_Invoice o"
                        + " INNER JOIN C_Currency c ON (o.C_Currency_ID=c.C_Currency_ID)"
                        + " INNER JOIN C_InvoiceLine l ON (o.C_Invoice_ID=l.C_Invoice_ID) "
                        + "WHERE o.C_Invoice_ID=" + Record_ID + "");
            }
            sql.Append("GROUP BY o.C_Currency_ID, c.ISO_Code, o.TotalLines, o.GrandTotal, o.DateAcct, o.AD_Client_ID, o.AD_Org_ID,o.C_CONVERSIONTYPE_ID");

            SqlParamsIn sqlP = new SqlParamsIn();
            sqlP.sql = sql.ToString();
            VIS.Helpers.SqlHelper help = new Helpers.SqlHelper();
            return help.ExecuteJDataSet(sqlP);

        }

        public List<JTable> LoadData(dynamic ServerValues)
        {
            List<string> sqlArray = new List<string>();

            if (ServerValues.IsExportData)
            {
                sqlArray.Add("SELECT AD_EXPORTDATA_ID,RECORD_ID, AD_ColOne_ID FROM AD_ExportData WHERE AD_Table_ID=" + ServerValues.AD_Table_ID);
            }
            if (ServerValues.IsAttachment)
            {
                sqlArray.Add("SELECT distinct att.AD_Attachment_ID, att.Record_ID FROM AD_Attachment att  INNER JOIN AD_AttachmentLine al ON (al.AD_Attachment_id=att.AD_Attachment_id)  WHERE att.AD_Table_ID=" + ServerValues.AD_Table_ID);
            }
            if (ServerValues.IsChat)
            {
                sqlArray.Add("SELECT CM_Chat_ID, Record_ID FROM CM_Chat WHERE AD_Table_ID=" + ServerValues.AD_Table_ID);
            }
            if (ServerValues.IsPLock)
            {
                sqlArray.Add("SELECT Record_ID FROM AD_Private_Access WHERE AD_User_ID=" + ServerValues.AD_User_ID + " AND AD_Table_ID=" + ServerValues.AD_Table_ID + " AND IsActive='Y' ORDER BY Record_ID");
            }
            if (ServerValues.IsSubscribeRecord)
            {
                sqlArray.Add("Select cm_Subscribe_ID,Record_ID from CM_Subscribe where AD_User_ID=" + ServerValues.AD_User_ID + " AND ad_Table_ID=" + ServerValues.AD_Table_ID);
            }
            if (ServerValues.IsViewDocument)
            {
                sqlArray.Add("SELECT vadms_windowdoclink_id, record_id FROM VADMS_WindowDocLink wdl JOIN vadms_document doc ON wdl.VADMS_Document_ID  =doc.VADMS_Document_ID WHERE doc.vadms_docstatus!='DD' AND AD_Table_ID =" + ServerValues.AD_Table_ID);
            }

            string sql = string.Join("~", sqlArray);

            SqlParamsIn sqlP = new SqlParamsIn();
            sqlP.sql = sql;
            VIS.Helpers.SqlHelper help = new Helpers.SqlHelper();
            return help.ExecuteJDataSet(sqlP);
        }

        #endregion

        #region windowFrame

        /// <summary>
        /// Used to set sequencing for sequence tabs
        /// </summary>
        /// <param name="columnIDs">List of IDs of columns</param>
        /// <param name="noYes">List, if yes, set display=Y Else N</param>
        /// <param name="tableName">Name of table</param>
        /// <param name="keyColumnName">Name of KeyColumn</param>
        /// <param name="columnSortName"> Name of column what will contain seqno </param>
        /// <param name="columnYesNoName"> Name of column what will contain info for if field is displayed OR not</param>
        /// <returns></returns>
        public string SetFieldsSorting(List<string> columnIDs, List<string> noYes, string tableName, string keyColumnName, string columnSortName, string columnYesNoName, Dictionary<string, string> oldValue)
        {
            StringBuilder sql = new StringBuilder();
            int yesCount = 0;
            int tableID = MTable.Get_Table_ID(tableName);

            sql.Append("SELECT AD_Column_ID FROM AD_Column WHERE AD_Table_ID=" + tableID + " AND ColumnName='" + columnSortName + "'");
            int colID = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString()));


            for (int i = 0; i < columnIDs.Count; i++)
            {
                if (noYes[i] == "N")
                {
                    sql.Clear();
                    sql.Append("UPDATE " + tableName + " SET " + columnSortName + "=0");
                    if (columnYesNoName != null)
                        sql.Append("," + columnYesNoName + "='N'");
                    sql.Append(", Updated=SYS_EXTRACT_UTC(SYSTIMESTAMP) ,UpdatedBy=" + _ctx.GetAD_User_ID() + " WHERE " + keyColumnName + "=" + columnIDs[i]);
                    DB.ExecuteQuery(sql.ToString());

                    MChangeLog log = new MChangeLog(_ctx, 0, null, _ctx.GetAD_Session_ID(), tableID, colID, keyColumnName, _ctx.GetAD_Client_ID(),
                        _ctx.GetAD_Org_ID(), oldValue[columnIDs[i]], 0);
                    log.SetRecord_ID(Convert.ToInt32(columnIDs[i]));
                    log.Save();

                }
                else
                {
                    sql.Clear();
                    sql.Append("UPDATE " + tableName + " SET " + columnSortName + "=" + (yesCount + 1) + "0");	//	10 steps
                    if (columnYesNoName != null)
                        sql.Append("," + columnYesNoName + "='Y'");
                    sql.Append(", Updated=SYS_EXTRACT_UTC(SYSTIMESTAMP) ,UpdatedBy=" + _ctx.GetAD_User_ID() + "  WHERE " + keyColumnName + "=" + columnIDs[i]);
                    DB.ExecuteQuery(sql.ToString());

                    MChangeLog log = new MChangeLog(_ctx, 0, null, _ctx.GetAD_Session_ID(), tableID, colID, keyColumnName, _ctx.GetAD_Client_ID(),
                        _ctx.GetAD_Org_ID(), oldValue[columnIDs[i]], (yesCount + 1) + "0");
                    log.SetRecord_ID(Convert.ToInt32(columnIDs[i]));
                    log.Save();

                    yesCount++;
                }
            }


            return "";
        }

        #endregion

        #region AccountForm

        public string GetValidCombination(dynamic accountSchemaElements, dynamic Elements, dynamic aseList, string value, string sb)
        {
            StringBuilder sql = new StringBuilder("SELECT C_ValidCombination_ID, Alias FROM C_ValidCombination WHERE ");
            for (var i = 0; i < Elements.Length; i++)
            {
                //var ase = Elements[i];
                var isMandatory = Elements[i].IsMandatory;
                var type = Elements[i].Type;
                //

                if (type.Equals(MAcctSchemaElement.ELEMENTTYPE_Organization))
                {
                    sql = sql.Append("AD_Org_ID");
                    if (value != null)
                    {
                        sql = sql.Append("=").Append(value).Append(" AND ");
                    }
                    //if (string.IsNullOrEmpty(value))
                    //    sql = sql.Append(" IS NULL AND ");
                    //else
                    //    sql = sql.Append("=").Append(value).Append(" AND ");
                }
                else if (type.Equals(MAcctSchemaElement.ELEMENTTYPE_Account))
                {
                    sql = sql.Append("Account_ID");
                    if (string.IsNullOrEmpty(value))
                        sql = sql.Append(" IS NULL AND ");
                    else
                        sql = sql.Append("=").Append(value).Append(" AND ");
                }
                else if (type.Equals(MAcctSchemaElement.ELEMENTTYPE_SubAccount))
                {
                    sql = sql.Append("C_SubAcct_ID");
                    if (string.IsNullOrEmpty(value))
                        sql = sql.Append(" IS NULL AND ");
                    else
                        sql = sql.Append("=").Append(value).Append(" AND ");
                }
                else if (type.Equals(MAcctSchemaElement.ELEMENTTYPE_Product))
                {
                    sql = sql.Append("M_Product_ID");
                    if (string.IsNullOrEmpty(value))
                        sql = sql.Append(" IS NULL AND ");
                    else
                        sql = sql.Append("=").Append(value).Append(" AND ");
                }
                else if (type.Equals(MAcctSchemaElement.ELEMENTTYPE_BPartner))
                {
                    sql = sql.Append("C_BPartner_ID");
                    if (string.IsNullOrEmpty(value))
                        sql = sql.Append(" IS NULL AND ");
                    else
                        sql = sql.Append("=").Append(value).Append(" AND ");
                }
                else if (type.Equals(MAcctSchemaElement.ELEMENTTYPE_Campaign))
                {
                    sql = sql.Append("C_Campaign_ID");
                    if (string.IsNullOrEmpty(value))
                        sql = sql.Append(" IS NULL AND ");
                    else
                        sql = sql.Append("=").Append(value).Append(" AND ");
                }
                else if (type.Equals(MAcctSchemaElement.ELEMENTTYPE_LocationFrom))
                {
                    sql = sql.Append("C_LocFrom_ID");
                    if (string.IsNullOrEmpty(value))
                        sql = sql.Append(" IS NULL AND ");
                    else
                        sql = sql.Append("=").Append(value).Append(" AND ");
                }
                else if (type.Equals(MAcctSchemaElement.ELEMENTTYPE_LocationTo))
                {
                    sql = sql.Append("C_LocTo_ID");
                    if (string.IsNullOrEmpty(value))
                        sql = sql.Append(" IS NULL AND ");
                    else
                        sql = sql.Append("=").Append(value).Append(" AND ");
                }
                else if (type.Equals(MAcctSchemaElement.ELEMENTTYPE_Project))
                {
                    sql = sql.Append("C_Project_ID");
                    if (string.IsNullOrEmpty(value))
                        sql = sql.Append(" IS NULL AND ");
                    else
                        sql = sql.Append("=").Append(value).Append(" AND ");
                }
                else if (type.Equals(MAcctSchemaElement.ELEMENTTYPE_SalesRegion))
                {
                    sql = sql.Append("C_SalesRegion_ID");
                    if (string.IsNullOrEmpty(value))
                        sql = sql.Append(" IS NULL AND ");
                    else
                        sql = sql.Append("=").Append(value).Append(" AND ");
                }
                else if (type.Equals(MAcctSchemaElement.ELEMENTTYPE_OrgTrx))
                {
                    sql = sql.Append("AD_OrgTrx_ID");
                    if (string.IsNullOrEmpty(value))
                        sql = sql.Append(" IS NULL AND ");
                    else
                        sql = sql.Append("=").Append(value).Append(" AND ");
                }
                else if (type.Equals(MAcctSchemaElement.ELEMENTTYPE_Activity))
                {
                    sql = sql.Append("C_Activity_ID");
                    if (string.IsNullOrEmpty(value))
                        sql = sql.Append(" IS NULL AND ");
                    else
                        sql = sql.Append("=").Append(value).Append(" AND ");
                }
                else if (type.Equals(MAcctSchemaElement.ELEMENTTYPE_UserList1))
                {
                    sql = sql.Append("User1_ID");
                    if (string.IsNullOrEmpty(value))
                        sql = sql.Append(" IS NULL AND ");
                    else
                        sql = sql.Append("=").Append(value).Append(" AND ");
                }
                else if (type.Equals(MAcctSchemaElement.ELEMENTTYPE_UserList2))
                {
                    sql = sql.Append("User2_ID");
                    if (string.IsNullOrEmpty(value))
                        sql = sql.Append(" IS NULL AND ");
                    else
                        sql = sql.Append("=").Append(value).Append(" AND ");
                }
                else if (type.Equals(MAcctSchemaElement.ELEMENTTYPE_UserElement1))
                {
                    sql = sql.Append("UserElement1_ID");
                    if (string.IsNullOrEmpty(value))
                        sql = sql.Append(" IS NULL AND ");
                    else
                        sql = sql.Append("=").Append(value).Append(" AND ");
                }
                else if (type.Equals(MAcctSchemaElement.ELEMENTTYPE_UserElement2))
                {
                    sql = sql.Append("UserElement2_ID");
                    if (string.IsNullOrEmpty(value))
                        sql = sql.Append(" IS NULL AND ");
                    else
                        sql = sql.Append("=").Append(value).Append(" AND ");
                }
                else if (type.Equals(MAcctSchemaElement.ELEMENTTYPE_UserElement3))
                {
                    sql = sql.Append("UserElement3_ID");
                    if (string.IsNullOrEmpty(value))
                        sql = sql.Append(" IS NULL AND ");
                    else
                        sql = sql.Append("=").Append(value).Append(" AND ");
                }
                else if (type.Equals(MAcctSchemaElement.ELEMENTTYPE_UserElement4))
                {
                    sql = sql.Append("UserElement4_ID");
                    if (string.IsNullOrEmpty(value))
                        sql = sql.Append(" IS NULL AND ");
                    else
                        sql = sql.Append("=").Append(value).Append(" AND ");
                }
                else if (type.Equals(MAcctSchemaElement.ELEMENTTYPE_UserElement5))
                {
                    sql = sql.Append("UserElement5_ID");
                    if (string.IsNullOrEmpty(value))
                        sql = sql.Append(" IS NULL AND ");
                    else
                        sql = sql.Append("=").Append(value).Append(" AND ");
                }
                else if (type.Equals(MAcctSchemaElement.ELEMENTTYPE_UserElement6))
                {
                    sql = sql.Append("UserElement6_ID");
                    if (string.IsNullOrEmpty(value))
                        sql = sql.Append(" IS NULL AND ");
                    else
                        sql = sql.Append("=").Append(value).Append(" AND ");
                }
                else if (type.Equals(MAcctSchemaElement.ELEMENTTYPE_UserElement7))
                {
                    sql = sql.Append("UserElement7_ID");
                    if (string.IsNullOrEmpty(value))
                        sql = sql.Append(" IS NULL AND ");
                    else
                        sql = sql.Append("=").Append(value).Append(" AND ");
                }
                else if (type.Equals(MAcctSchemaElement.ELEMENTTYPE_UserElement8))
                {
                    sql = sql.Append("UserElement8_ID");
                    if (string.IsNullOrEmpty(value))
                        sql = sql.Append(" IS NULL AND ");
                    else
                        sql = sql.Append("=").Append(value).Append(" AND ");
                }
                else if (type.Equals(MAcctSchemaElement.ELEMENTTYPE_UserElement9))
                {
                    sql = sql.Append("UserElement9_ID");
                    if (string.IsNullOrEmpty(value))
                        sql = sql.Append(" IS NULL AND ");
                    else
                        sql = sql.Append("=").Append(value).Append(" AND ");
                }
                //  
                if (isMandatory && (value == null) && sb != null)
                {
                    sb = sb + aseList[i] + ", ";
                }
            }
            return sql.ToString();
        }

        #endregion

        #region CreateFromStatement

        public List<JTable> GetBankAccountData(int C_BankAccount_ID, DateTime ts)
        {
            var sql = "SELECT p.DateTrx,p.C_Payment_ID,p.DocumentNo, ba.C_Currency_ID,c.ISO_Code,p.PayAmt,"
               + "currencyConvert(p.PayAmt,p.C_Currency_ID,ba.C_Currency_ID,@t,null,p.AD_Client_ID,p.AD_Org_ID),"   //  #1
               + " bp.Name,'P' AS Type "
               + "FROM C_BankAccount ba"
               + " INNER JOIN C_Payment_v p ON (p.C_BankAccount_ID=ba.C_BankAccount_ID)"
               + " INNER JOIN C_Currency c ON (p.C_Currency_ID=c.C_Currency_ID)"
               + " LEFT OUTER JOIN C_BPartner bp ON (p.C_BPartner_ID=bp.C_BPartner_ID) "
               + "WHERE p.Processed='Y' AND p.IsReconciled='N'"
               + " AND p.DocStatus IN ('CO','CL','RE','VO') AND p.PayAmt<>0"
               + " AND p.C_BankAccount_ID=@C_BankAccount_ID"                              	//  #2
               + " AND NOT EXISTS (SELECT * FROM C_BankStatementLine l "
                   //	Voided Bank Statements have 0 StmtAmt
                   + "WHERE p.C_Payment_ID=l.C_Payment_ID AND l.StmtAmt <> 0)";
            var countVA012 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(AD_ModuleInfo_ID) FROM AD_ModuleInfo WHERE PREFIX='VA012_' AND IsActive = 'Y'"));
            if (countVA012 > 0)
            {

                sql += " UNION SELECT cs.DateAcct AS DateTrx,cl.C_CashLine_ID AS C_Payment_ID,cs.DocumentNo, ba.C_Currency_ID,c.ISO_Code,cl.Amount AS PayAmt,"
                + "currencyConvert(cl.Amount,cl.C_Currency_ID,ba.C_Currency_ID,@t,null,cs.AD_Client_ID,cs.AD_Org_ID),"   //  #1
                + " Null AS Name,'C' AS Type FROM C_BankAccount ba"
                + " INNER JOIN C_CashLine cl ON (cl.C_BankAccount_ID=ba.C_BankAccount_ID)"
                + " INNER JOIN C_Cash cs ON (cl.C_Cash_ID=cs.C_Cash_ID)"
                + " INNER JOIN C_Charge chrg ON chrg.C_Charge_ID=cl.C_Charge_ID"
                + " INNER JOIN C_Currency c ON (cl.C_Currency_ID=c.C_Currency_ID)"
                + " WHERE cs.Processed='Y' AND cl.VA012_IsReconciled='N'"
                + " AND cl.CashType ='C' AND chrg.DTD001_ChargeType ='CON'"
                + " AND cs.DocStatus IN ('CO','CL','RE','VO') AND cl.Amount<>0"
                + " AND cl.C_BankAccount_ID=@C_BankAccount_ID"                              	//  #2
                + " AND NOT EXISTS (SELECT * FROM C_BankStatementLine l "
                    //	Voided Bank Statements have 0 StmtAmt
                    + "WHERE cl.C_CashLine_ID=l.C_CashLine_ID AND l.StmtAmt <> 0)";
            }


            SqlParamsIn sqlP = new SqlParamsIn();

            List<SqlParams> param = new List<SqlParams>();
            param.Add(new SqlParams() { name = "@t", value = ts });
            param.Add(new SqlParams() { name = "@C_BankAccount_ID", value = C_BankAccount_ID });

            sqlP.sql = sql;
            sqlP.param = param;
            VIS.Helpers.SqlHelper help = new Helpers.SqlHelper();
            return help.ExecuteJDataSet(sqlP);


        }

        #endregion

        #region Common
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_ctx"></param>
        /// <param name="Columns"></param>
        /// <param name="AD_Language"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetTranslatedText(Ctx _ctx, List<string> Cols, string AD_Language)
        {
            Dictionary<string, object> translations = new Dictionary<string, object>();
            if (Cols.Count > 0)
            {
                DataSet dsTrl = null;
                DataTable dt = null;
                try
                {
                    // create comma separated string from list of elements provided
                    string Columns = string.Join(",", string.Join(",", Cols).Replace(" ", "").Split(',').Select(x => string.Format("'{0}'", x)));

                    string sqlElements = "";
                    string sqlMsgs = "";
                    System.Data.SqlClient.SqlParameter[] param = null;

                    // check if we have to check base Language (i.e. English) else other language to fetch data from translations
                    if (Env.IsBaseLanguage(_ctx, "AD_Element"))
                    {
                        sqlElements = @"SELECT e.ColumnName, NVL(e.Name, '') AS Name FROM AD_Element e WHERE e.ColumnName IN (" + Columns + ")";

                        sqlMsgs = @"SELECT m.Value, COALESCE(m.MsgText, m.Value) AS Name  FROM AD_Message m WHERE m.Value NOT  IN
                                (SELECT ColumnName FROM AD_Element WHERE ColumnName IN (" + Columns + @")) AND m.Value IN (" + Columns + ")";
                    }
                    else
                    {
                        param = new System.Data.SqlClient.SqlParameter[1];
                        param[0] = new System.Data.SqlClient.SqlParameter("@AD_Language", AD_Language);

                        sqlElements = @"SELECT e.ColumnName, COALESCE(t.Name, e.Name) AS Name FROM AD_Element e INNER JOIN AD_Element_Trl t ON (e.AD_Element_ID = t.AD_Element_ID)
                        WHERE  t.AD_Language = @AD_Language AND e.ColumnName IN (" + Columns + ")";

                        sqlMsgs = @"SELECT m.Value, COALESCE(t.MsgText, m.MsgText) AS Name  FROM AD_Message m LEFT JOIN AD_Message_Trl t ON (m.AD_Message_ID = t.AD_Message_ID) WHERE m.Value NOT  IN
                            (SELECT ColumnName FROM AD_Element WHERE ColumnName IN (" + Columns + ")) AND m.Value IN (" + Columns + ") AND t.AD_Language = @AD_Language";
                    }

                    dsTrl = DB.ExecuteDataset(sqlElements, param, null);
                    if (dsTrl != null && dsTrl.Tables[0].Rows.Count > 0)
                    {
                        dt = dsTrl.Tables[0];
                        // convert result data to Dictionary
                        translations = dt.AsEnumerable().ToDictionary<DataRow, string, object>(row => row.Field<string>(0), row => row.Field<object>(1));
                    }
                    dsTrl = null;
                    dt = null;

                    dsTrl = DB.ExecuteDataset(sqlMsgs, param, null);
                    if (dsTrl != null && dsTrl.Tables[0].Rows.Count > 0)
                    {
                        dt = dsTrl.Tables[0];
                        // convert result data to Dictionary and concat to existing dictionary result
                        translations = translations.Concat(dt.AsEnumerable().ToDictionary<DataRow, string, object>(row => row.Field<string>(0), row => row.Field<object>(1))).ToDictionary(x => x.Key, x => x.Value);
                    }
                }
                catch (Exception ex)
                {
                    dsTrl = null;
                    dt = null;
                }
            }
            return translations;
        }
        #endregion

    }
}