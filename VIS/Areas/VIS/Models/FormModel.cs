using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
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

        public List<JTable> GetProcessedRequest(int VAF_TableView_ID, int Record_ID)
        {
            string m_where = "(VAF_TableView_ID=" + VAF_TableView_ID + " AND Record_ID=" + Record_ID + ")";

            if (VAF_TableView_ID == 114)
            {// MUser.Table_ID){
                m_where += " OR VAF_UserContact_ID=" + Record_ID + " OR SalesRep_ID=" + Record_ID;
            }
            else if (VAF_TableView_ID == 291)
            {//MBPartner.Table_ID){
                m_where += " OR VAB_BusinessPartner_ID=" + Record_ID;
            }
            else if (VAF_TableView_ID == 259)
            {// MOrder.Table_ID){
                m_where += " OR VAB_Order_ID=" + Record_ID;
            }
            else if (VAF_TableView_ID == 318)
            {//MInvoice.Table_ID){
                m_where += " OR VAB_Invoice_ID=" + Record_ID;
            }
            else if (VAF_TableView_ID == 335)
            {// MPayment.Table_ID){
                m_where += " OR VAB_Payment_ID=" + Record_ID;
            }
            else if (VAF_TableView_ID == 208)
            {//MProduct.Table_ID){
                m_where += " OR M_Product_ID=" + Record_ID;
            }
            else if (VAF_TableView_ID == 203)
            {//MProject.Table_ID){
                m_where += " OR VAB_Project_ID=" + Record_ID;
            }
            else if (VAF_TableView_ID == 539)
            {// MAsset.Table_ID){
                m_where += " OR VAA_Asset_ID=" + Record_ID;
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
            string sql = "SELECT DISTINCT t.VAF_TableView_ID, t.TableName "
              + "FROM VAF_TableView t "
              + "WHERE EXISTS (SELECT 1 FROM VAF_Tab tt "
                  + "WHERE tt.VAF_TableView_ID = t.VAF_TableView_ID AND tt.SeqNo=10) "
              + " AND t.VAF_TableView_ID IN "
                  + "(SELECT VAF_TableView_ID FROM VAF_Column "
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
        public int GetZoomVAF_Screen_ID(string targetTableName, int curWindow_ID, string targetWhereClause, bool isSOTrx)
        {
            int zoomWindow_ID = 0;
            int PO_zoomWindow_ID = 0;
            // Find windows where the first tab is based on the table
            string sql = "SELECT DISTINCT VAF_Screen_ID, PO_Window_ID "
                + "FROM VAF_TableView t "
                + "WHERE TableName ='" + targetTableName + "'";
            IDataReader dr = null;
            try
            {
                //DataSet ds = null;
                //ds = ExecuteQuery.ExecuteDataset(sql);

                dr = DB.ExecuteReader(sql);
                while (dr.Read())
                {
                    zoomWindow_ID = Util.GetValueOfInt(dr["VAF_Screen_ID"].ToString());
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
                //VAdvantage.Common.ErrorLog.FillErrorLog("ZoomTarget.GetZoomVAF_Screen_ID", GlobalVariable.LAST_EXECUTED_QUERY, e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
            }


            if (PO_zoomWindow_ID == 0)
                return zoomWindow_ID;

            int VAF_Screen_ID = 0;

            if (targetWhereClause != null && targetWhereClause.Length != 0)
            {
                List<KeyNamePair> zoomList = new List<KeyNamePair>();
                zoomList = ZoomTarget.GetZoomTargets(targetTableName, curWindow_ID, targetWhereClause, _ctx);
                if (zoomList != null && zoomList.Count > 0)
                    VAF_Screen_ID = zoomList[0].GetKey();
            }
            if (VAF_Screen_ID != 0)
            {
                return VAF_Screen_ID;
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
            string sql = "SELECT DISTINCT w.VAF_Screen_ID, w.Name, tt.WhereClause, t.TableName, " +
                    "wp.VAF_Screen_ID, wp.Name, ws.VAF_Screen_ID, ws.Name "
                + "FROM VAF_TableView t "
                + "INNER JOIN VAF_Tab tt ON (tt.VAF_TableView_ID = t.VAF_TableView_ID) ";

            bool baseLanguage = Env.IsBaseLanguage(ctx, "");// GlobalVariable.IsBaseLanguage();
            if (baseLanguage)
            {
                sql += "INNER JOIN VAF_Screen w ON (tt.VAF_Screen_ID=w.VAF_Screen_ID)";
                sql += " LEFT OUTER JOIN VAF_Screen ws ON (t.VAF_Screen_ID=ws.VAF_Screen_ID)"
                    + " LEFT OUTER JOIN VAF_Screen wp ON (t.PO_Window_ID=wp.VAF_Screen_ID)";
            }
            else
            {
                sql += "INNER JOIN VAF_Screen_TL w ON (tt.VAF_Screen_ID=w.VAF_Screen_ID AND w.VAF_Language='" + Env.GetVAF_Language(ctx) + "')";
                sql += " LEFT OUTER JOIN VAF_Screen_TL ws ON (t.VAF_Screen_ID=ws.VAF_Screen_ID AND ws.VAF_Language='" + Env.GetVAF_Language(ctx) + "')"
                    + " LEFT OUTER JOIN VAF_Screen_TL wp ON (t.PO_Window_ID=wp.VAF_Screen_ID AND wp.VAF_Language='" + Env.GetVAF_Language(ctx) + "')";
            }
            sql += "WHERE t.TableName ='" + targetTableName
                + "' AND w.VAF_Screen_ID <>" + curWindow_ID
                + " AND tt.SeqNo=10"
                + " AND (wp.VAF_Screen_ID IS NOT NULL "
                        + "OR EXISTS (SELECT 1 FROM VAF_Tab tt2 WHERE tt2.VAF_Screen_ID = ws.VAF_Screen_ID AND tt2.VAF_TableView_ID=t.VAF_TableView_ID AND tt2.SeqNo=10))"
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
                                KeyNamePair pp = new KeyNamePair(windowList[i].VAF_Screen_ID, windowList[i].windowName);
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


        public int GetWorkflowWindowID(int VAF_TableView_ID)
        {
            object windowID = DB.ExecuteScalar("SELECT VAF_Screen_ID FROM VAF_TableView WHERE VAF_TableView_ID=" + VAF_TableView_ID);
            if (windowID != null && windowID != DBNull.Value)
            {
                return Convert.ToInt32(windowID);
            }
            return 0;
        }

        public List<JTable> GetZoomWindowID(int VAF_TableView_ID)
        {
            SqlParamsIn sqlP = new SqlParamsIn();
            sqlP.sql = "SELECT TableName, VAF_Screen_ID, PO_Window_ID FROM VAF_TableView WHERE VAF_TableView_ID=" + VAF_TableView_ID;

            VIS.Helpers.SqlHelper help = new Helpers.SqlHelper();
            return help.ExecuteJDataSet(sqlP);
        }

        public List<JTable> GetZoomTargetClass(Ctx ctx, string targetTableName, int curWindow_ID)
        {
            string sql = "SELECT DISTINCT w.VAF_Screen_ID, w.Name, tt.WhereClause, t.TableName, " +
                              "wp.VAF_Screen_ID, wp.Name, ws.VAF_Screen_ID, ws.Name "
                          + "FROM VAF_TableView t "
                          + "INNER JOIN VAF_Tab tt ON (tt.VAF_TableView_ID = t.VAF_TableView_ID) ";
            var baseLanguage = Env.IsBaseLanguage(ctx, "VAF_Screen");
            if (baseLanguage)
            {
                sql += "INNER JOIN VAF_Screen w ON (tt.VAF_Screen_ID=w.VAF_Screen_ID)";
                sql += " LEFT OUTER JOIN VAF_Screen ws ON (t.VAF_Screen_ID=ws.VAF_Screen_ID)"
                    + " LEFT OUTER JOIN VAF_Screen wp ON (t.PO_Window_ID=wp.VAF_Screen_ID)";
            }
            else
            {
                sql += "INNER JOIN VAF_Screen_TL w ON (tt.VAF_Screen_ID=w.VAF_Screen_ID AND w.VAF_Language=@para1)";
                sql += " LEFT OUTER JOIN VAF_Screen_TL ws ON (t.VAF_Screen_ID=ws.VAF_Screen_ID AND ws.VAF_Language=@para2)"
                    + " LEFT OUTER JOIN VAF_Screen_TL wp ON (t.PO_Window_ID=wp.VAF_Screen_ID AND wp.VAF_Language=@para3)";
            }
            sql += "WHERE t.TableName = @para4"
                + " AND w.VAF_Screen_ID <> @para5 AND w.isActive='Y'"
                + " AND tt.SeqNo=10"
                + " AND (wp.VAF_Screen_ID IS NOT NULL "
                        + "OR EXISTS (SELECT 1 FROM VAF_Tab tt2 WHERE tt2.VAF_Screen_ID = ws.VAF_Screen_ID AND tt2.VAF_TableView_ID=t.VAF_TableView_ID AND tt2.SeqNo=10))"
                + " ORDER BY 2";

            List<SqlParams> param = new List<SqlParams>();
            if (!baseLanguage)
            {
                param.Add(new SqlParams() { name = "@para1", value = Env.GetVAF_Language(Env.GetCtx()) });
                param.Add(new SqlParams() { name = "@para2", value = Env.GetVAF_Language(Env.GetCtx()) });
                param.Add(new SqlParams() { name = "@para3", value = Env.GetVAF_Language(Env.GetCtx()) });
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
            else if (_columnName.Equals("VAB_BusinessPartner_ID"))
            {
                sql += "SELECT VAB_BusinessPartner_ID FROM VAB_BusinessPartner WHERE (UPPER(Value) LIKE ";
                sql += DB.TO_STRING(text) + " OR UPPER(Name) LIKE " + DB.TO_STRING(text) + ")";
            }
            else if (_columnName.Equals("VAB_Order_ID"))
            {
                sql += "SELECT VAB_Order_ID FROM VAB_Order WHERE UPPER(DocumentNo) LIKE ";
                sql += DB.TO_STRING(text);
            }
            else if (_columnName.Equals("VAB_Invoice_ID"))
            {
                sql += "SELECT VAB_Invoice_ID FROM VAB_Invoice WHERE UPPER(DocumentNo) LIKE ";
                sql += DB.TO_STRING(text);
            }
            else if (_columnName.Equals("M_InOut_ID"))
            {
                sql += "SELECT M_InOut_ID FROM M_InOut WHERE UPPER(DocumentNo) LIKE ";
                sql += DB.TO_STRING(text);
            }
            else if (_columnName.Equals("VAB_Payment_ID"))
            {
                sql += "SELECT VAB_Payment_ID FROM VAB_Payment WHERE UPPER(DocumentNo) LIKE ";
                sql += DB.TO_STRING(text);
            }
            else if (_columnName.Equals("VAGL_BatchJRNL_ID"))
            {
                sql += "SELECT VAGL_BatchJRNL_ID FROM VAGL_BatchJRNL WHERE UPPER(DocumentNo) LIKE ";
                sql += DB.TO_STRING(text);
            }
            else if (_columnName.Equals("SalesRep_ID"))
            {
                sql += "SELECT VAF_UserContact_ID FROM VAF_UserContact WHERE UPPER(Name) LIKE ";
                sql += DB.TO_STRING(text);
            }
            return sql;
        }

        public List<JTable> GetWareProWiseLocator(Ctx ctx, string colName, int orgId, int warehouseId, int productId, bool onlyIsSOTrx)
        {
            string sql = "SELECT M_Locator_ID," + colName + " FROM M_Locator WHERE IsActive='Y'";
            //JID_0932 In validation of locator need to consider organization  
            if (orgId != 0)
            {
                sql += " AND VAF_Org_ID=@org";
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


        public List<JTable> GetValidAccountCombination(int VAF_Client_ID, bool onlyActive)
        {
            string sql = "SELECT VAB_Acct_ValidParameter_ID, Combination, Description "
            + "FROM VAB_Acct_ValidParameter WHERE VAF_Client_ID=" + VAF_Client_ID;
            if (onlyActive)
                sql += " AND IsActive='Y'";
            sql += " ORDER BY 2";
            SqlParamsIn sqlP = new SqlParamsIn();
            sqlP.sql = sql;
            VIS.Helpers.SqlHelper help = new Helpers.SqlHelper();
            return help.ExecuteJDataSet(sqlP);
        }

        public List<JTable> GenAttributeSetInstance(int VAB_GenFeatureSet_ID, bool onlyActive)
        {
            var sql = "SELECT ASI.VAB_GenFeatureSetInstance_ID, ASI.Description " +
         " from VAB_GenFeatureSetInstance ASI, M_Product P WHERE ASI.VAB_GenFeatureSet_ID  = " + VAB_GenFeatureSet_ID;
            if (onlyActive)
                sql += " AND IsActive='Y'";
            sql += " ORDER BY 2";
            SqlParamsIn sqlP = new SqlParamsIn();
            sqlP.sql = sql;
            VIS.Helpers.SqlHelper help = new Helpers.SqlHelper();
            return help.ExecuteJDataSet(sqlP);
        }

        public string GetDocWhere(int VAF_UserContact_ID, string TableName)
        {
            string docAccess = "(EXISTS (SELECT 1 FROM VAB_BusinessPartner bp INNER JOIN VAF_UserContact u "
            + "ON (u.VAB_BusinessPartner_ID=bp.VAB_BusinessPartner_ID) "
            + " WHERE u.VAF_UserContact_ID="
            + VAF_UserContact_ID
            + " AND bp.VAB_BusinessPartner_ID="
            + TableName
            + ".VAB_BusinessPartner_ID)"
            + " OR EXISTS (SELECT 1 FROM VAB_BPart_Relation bpr INNER JOIN VAF_UserContact u "
            + "ON (u.VAB_BusinessPartner_ID=bpr.VAB_BusinessPartnerRelation_ID) "
            + " WHERE u.VAF_UserContact_ID="
            + VAF_UserContact_ID
            + " AND bpr.VAB_BusinessPartner_ID=" + TableName + ".VAB_BusinessPartner_ID)";

            var hasUserColumn = false;
            var sql1 = "SELECT count(*) FROM VAF_TableView t "
                + "INNER JOIN VAF_Column c ON (t.VAF_TableView_ID=c.VAF_TableView_ID) "
                + "WHERE t.tableName='" + TableName
                + "' AND c.ColumnName='VAF_UserContact_ID' ";

            int ret = Util.GetValueOfInt(DB.ExecuteScalar(docAccess));
            hasUserColumn = ret > 0 ? true : false;

            if (hasUserColumn)
                docAccess += " OR " + TableName + ".VAF_UserContact_ID =" + VAF_UserContact_ID;
            docAccess += ")";
            return docAccess;
        }


        public string UpdateTree(List<int> oldParentChildren, List<int> newParentChildren, int oldId, int newId, int VAF_TreeInfo_ID, string tableName)
        {
            string sql;
            for (var i = 0; i < oldParentChildren.Count; i++)
            {
                //StringBuilder sql = new StringBuilder("UPDATE ");
                sql = "UPDATE " + tableName + " SET Parent_ID=" + oldId + ", SeqNo=" + i + ", Updated=SysDate" +
                                  " WHERE VAF_TreeInfo_ID=" + VAF_TreeInfo_ID + " AND Node_ID=" + oldParentChildren[i];
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
                     " WHERE VAF_TreeInfo_ID=" + VAF_TreeInfo_ID +
                      " AND Node_ID=" + newParentChildren[i];
                    DB.ExecuteQuery(sql);
                }
            }

            return "";
        }


        #region AReport
        public List<JTable> GetPrintFormats(int VAF_TableView_ID, int VAF_Tab_ID)
        {
            string sql = "SELECT VAF_Print_Rpt_Layout_ID, Name, VAF_Client_ID "
                       + "FROM VAF_Print_Rpt_Layout "
                       + "WHERE VAF_TableView_ID='" + VAF_TableView_ID + "' AND IsTableBased='Y' ";
            if (VAF_Tab_ID > 0)
            {
                sql = sql + " AND VAF_Tab_ID='" + VAF_Tab_ID + "' ";
            }
            sql = sql + "ORDER BY VAF_Client_ID DESC, IsDefault DESC, Name";	//	Own First
            sql = MRole.GetDefault(_ctx).AddAccessSQL(sql,		//	Own First
                   "VAF_Print_Rpt_Layout", MRole.SQL_NOTQUALIFIED, MRole.SQL_RO);

            SqlParamsIn sqlP = new SqlParamsIn();
            sqlP.sql = sql;
            VIS.Helpers.SqlHelper help = new Helpers.SqlHelper();
            return help.ExecuteJDataSet(sqlP);

        }

        public List<JTable> GetShowReportDetails(int VAF_TableView_ID, int VAF_Tab_ID)
        {
            string sql = "SELECT VAF_Print_Rpt_Layout_ID, Name, Description,IsDefault "
                               + "FROM VAF_Print_Rpt_Layout "
                               + "WHERE VAF_TableView_ID=" + VAF_TableView_ID;
            if (VAF_Tab_ID > 0)
            {
                sql = sql + " AND VAF_Tab_ID=" + VAF_Tab_ID;
            }
            sql = sql + " ORDER BY Name";
            sql = MRole.GetDefault(_ctx).AddAccessSQL(sql, "VAF_Print_Rpt_Layout", MRole.SQL_NOTQUALIFIED, MRole.SQL_RO);

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
                        + "CURRENCYBASEWITHCONVERSIONTYPE(o.GrandTotal,o.VAB_Currency_ID,o.DateAcct, o.VAF_Client_ID,o.VAF_Org_ID, o.VAB_CurrencyType_ID) AS ConvAmt ");
            if (isOrder)
            {
                sql.Append("FROM VAB_Order o"
                        + " INNER JOIN VAB_Currency c ON (o.VAB_Currency_ID=c.VAB_Currency_ID)"
                        + " INNER JOIN VAB_OrderLine l ON (o.VAB_Order_ID=l.VAB_Order_ID) "
                        + "WHERE o.VAB_Order_ID=" + Record_ID + "");
            }
            else
            {
                sql.Append("FROM VAB_Invoice o"
                        + " INNER JOIN VAB_Currency c ON (o.VAB_Currency_ID=c.VAB_Currency_ID)"
                        + " INNER JOIN VAB_InvoiceLine l ON (o.VAB_Invoice_ID=l.VAB_Invoice_ID) "
                        + "WHERE o.VAB_Invoice_ID=" + Record_ID + "");
            }
            sql.Append("GROUP BY o.VAB_Currency_ID, c.ISO_Code, o.TotalLines, o.GrandTotal, o.DateAcct, o.VAF_Client_ID, o.VAF_Org_ID,o.VAB_CurrencyType_ID");

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
                sqlArray.Add("SELECT VAF_ExportData_ID,RECORD_ID, AD_ColOne_ID FROM VAF_ExportData WHERE VAF_TableView_ID=" + ServerValues.VAF_TableView_ID);
            }
            if (ServerValues.IsAttachment)
            {
                sqlArray.Add("SELECT distinct att.VAF_Attachment_ID, att.Record_ID FROM VAF_Attachment att  INNER JOIN VAF_AttachmentLine al ON (al.VAF_Attachment_id=att.VAF_Attachment_id)  WHERE att.VAF_TableView_ID=" + ServerValues.VAF_TableView_ID);
            }
            if (ServerValues.IsChat)
            {
                sqlArray.Add("SELECT VACM_Chat_ID, Record_ID FROM VACM_Chat WHERE VAF_TableView_ID=" + ServerValues.VAF_TableView_ID);
            }
            if (ServerValues.IsPLock)
            {
                sqlArray.Add("SELECT Record_ID FROM VAF_Private_Rights WHERE VAF_UserContact_ID=" + ServerValues.VAF_UserContact_ID + " AND VAF_TableView_ID=" + ServerValues.VAF_TableView_ID + " AND IsActive='Y' ORDER BY Record_ID");
            }
            if (ServerValues.IsSubscribeRecord)
            {
                sqlArray.Add("Select VACM_Subscribe_ID,Record_ID from VACM_Subscribe where VAF_UserContact_ID=" + ServerValues.VAF_UserContact_ID + " AND vaf_tableview_ID=" + ServerValues.VAF_TableView_ID);
            }
            if (ServerValues.IsViewDocument)
            {
                sqlArray.Add("SELECT vadms_windowdoclink_id, record_id FROM VADMS_WindowDocLink wdl JOIN vadms_document doc ON wdl.VADMS_Document_ID  =doc.VADMS_Document_ID WHERE doc.vadms_docstatus!='DD' AND VAF_TableView_ID =" + ServerValues.VAF_TableView_ID);
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

            sql.Append("SELECT VAF_Column_ID FROM VAF_Column WHERE VAF_TableView_ID=" + tableID + " AND ColumnName='" + columnSortName + "'");
            int colID = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString()));


            for (int i = 0; i < columnIDs.Count; i++)
            {
                if (noYes[i] == "N")
                {
                    sql.Clear();
                    sql.Append("UPDATE " + tableName + " SET " + columnSortName + "=0");
                    if (columnYesNoName != null)
                        sql.Append("," + columnYesNoName + "='N'");
                    sql.Append(", Updated=SYS_EXTRACT_UTC(SYSTIMESTAMP) ,UpdatedBy=" + _ctx.GetVAF_UserContact_ID() + " WHERE " + keyColumnName + "=" + columnIDs[i]);
                    DB.ExecuteQuery(sql.ToString());

                    MChangeLog log = new MChangeLog(_ctx, 0, null, _ctx.GetAD_Session_ID(), tableID, colID, keyColumnName, _ctx.GetVAF_Client_ID(),
                        _ctx.GetVAF_Org_ID(), oldValue[columnIDs[i]], 0);
                    log.SetRecord_ID(Convert.ToInt32(columnIDs[i]));
                    log.Save();

                }
                else
                {
                    sql.Clear();
                    sql.Append("UPDATE " + tableName + " SET " + columnSortName + "=" + (yesCount + 1) + "0");	//	10 steps
                    if (columnYesNoName != null)
                        sql.Append("," + columnYesNoName + "='Y'");
                    sql.Append(", Updated=SYS_EXTRACT_UTC(SYSTIMESTAMP) ,UpdatedBy=" + _ctx.GetVAF_UserContact_ID() + "  WHERE " + keyColumnName + "=" + columnIDs[i]);
                    DB.ExecuteQuery(sql.ToString());

                    MChangeLog log = new MChangeLog(_ctx, 0, null, _ctx.GetAD_Session_ID(), tableID, colID, keyColumnName, _ctx.GetVAF_Client_ID(),
                        _ctx.GetVAF_Org_ID(), oldValue[columnIDs[i]], (yesCount + 1) + "0");
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
            StringBuilder sql = new StringBuilder("SELECT VAB_Acct_ValidParameter_ID, Alias FROM VAB_Acct_ValidParameter WHERE ");
            for (var i = 0; i < Elements.Length; i++)
            {
                //var ase = Elements[i];
                var isMandatory = Elements[i].IsMandatory;
                var type = Elements[i].Type;
                //

                if (type.Equals(MAcctSchemaElement.ELEMENTTYPE_Organization))
                {
                    sql = sql.Append("VAF_Org_ID");
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
                    sql = sql.Append("VAB_SubAcct_ID");
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
                    sql = sql.Append("VAB_BusinessPartner_ID");
                    if (string.IsNullOrEmpty(value))
                        sql = sql.Append(" IS NULL AND ");
                    else
                        sql = sql.Append("=").Append(value).Append(" AND ");
                }
                else if (type.Equals(MAcctSchemaElement.ELEMENTTYPE_Campaign))
                {
                    sql = sql.Append("VAB_Promotion_ID");
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
                    sql = sql.Append("VAB_Project_ID");
                    if (string.IsNullOrEmpty(value))
                        sql = sql.Append(" IS NULL AND ");
                    else
                        sql = sql.Append("=").Append(value).Append(" AND ");
                }
                else if (type.Equals(MAcctSchemaElement.ELEMENTTYPE_SalesRegion))
                {
                    sql = sql.Append("VAB_SalesRegionState_ID");
                    if (string.IsNullOrEmpty(value))
                        sql = sql.Append(" IS NULL AND ");
                    else
                        sql = sql.Append("=").Append(value).Append(" AND ");
                }
                else if (type.Equals(MAcctSchemaElement.ELEMENTTYPE_OrgTrx))
                {
                    sql = sql.Append("VAF_OrgTrx_ID");
                    if (string.IsNullOrEmpty(value))
                        sql = sql.Append(" IS NULL AND ");
                    else
                        sql = sql.Append("=").Append(value).Append(" AND ");
                }
                else if (type.Equals(MAcctSchemaElement.ELEMENTTYPE_Activity))
                {
                    sql = sql.Append("VAB_BillingCode_ID");
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

        public List<JTable> GetBankAccountData(int VAB_Bank_Acct_ID, DateTime ts)
        {
            var sql = "SELECT p.DateTrx,p.VAB_Payment_ID,p.DocumentNo, ba.VAB_Currency_ID,c.ISO_Code,p.PayAmt,"
               + "currencyConvert(p.PayAmt,p.VAB_Currency_ID,ba.VAB_Currency_ID,@t,null,p.VAF_Client_ID,p.VAF_Org_ID),"   //  #1
               + " bp.Name,'P' AS Type "
               + "FROM VAB_Bank_Acct ba"
               + " INNER JOIN VAB_Payment_V p ON (p.VAB_Bank_Acct_ID=ba.VAB_Bank_Acct_ID)"
               + " INNER JOIN VAB_Currency c ON (p.VAB_Currency_ID=c.VAB_Currency_ID)"
               + " LEFT OUTER JOIN VAB_BusinessPartner bp ON (p.VAB_BusinessPartner_ID=bp.VAB_BusinessPartner_ID) "
               + "WHERE p.Processed='Y' AND p.IsReconciled='N'"
               + " AND p.DocStatus IN ('CO','CL','RE','VO') AND p.PayAmt<>0"
               + " AND p.VAB_Bank_Acct_ID=@VAB_Bank_Acct_ID"                              	//  #2
               + " AND NOT EXISTS (SELECT * FROM VAB_BankingJRNLLine l "
                   //	Voided Bank Statements have 0 StmtAmt
                   + "WHERE p.VAB_Payment_ID=l.VAB_Payment_ID AND l.StmtAmt <> 0)";
            var countVA012 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(VAF_ModuleInfo_ID) FROM VAF_ModuleInfo WHERE PREFIX='VA012_' AND IsActive = 'Y'"));
            if (countVA012 > 0)
            {

                sql += " UNION SELECT cs.DateAcct AS DateTrx,cl.VAB_CashJRNLLine_ID AS VAB_Payment_ID,cs.DocumentNo, ba.VAB_Currency_ID,c.ISO_Code,cl.Amount AS PayAmt,"
                + "currencyConvert(cl.Amount,cl.VAB_Currency_ID,ba.VAB_Currency_ID,@t,null,cs.VAF_Client_ID,cs.VAF_Org_ID),"   //  #1
                + " Null AS Name,'C' AS Type FROM VAB_Bank_Acct ba"
                + " INNER JOIN VAB_CashJRNLLine cl ON (cl.VAB_Bank_Acct_ID=ba.VAB_Bank_Acct_ID)"
                + " INNER JOIN VAB_CashJRNL cs ON (cl.VAB_CashJRNL_ID=cs.VAB_CashJRNL_ID)"
                + " INNER JOIN VAB_Charge chrg ON chrg.VAB_Charge_ID=cl.VAB_Charge_ID"
                + " INNER JOIN VAB_Currency c ON (cl.VAB_Currency_ID=c.VAB_Currency_ID)"
                + " WHERE cs.Processed='Y' AND cl.VA012_IsReconciled='N'"
                + " AND cl.CashType ='C' AND chrg.DTD001_ChargeType ='CON'"
                + " AND cs.DocStatus IN ('CO','CL','RE','VO') AND cl.Amount<>0"
                + " AND cl.VAB_Bank_Acct_ID=@VAB_Bank_Acct_ID"                              	//  #2
                + " AND NOT EXISTS (SELECT * FROM VAB_BankingJRNLLine l "
                    //	Voided Bank Statements have 0 StmtAmt
                    + "WHERE cl.VAB_CashJRNLLine_ID=l.VAB_CashJRNLLine_ID AND l.StmtAmt <> 0)";
            }


            SqlParamsIn sqlP = new SqlParamsIn();

            List<SqlParams> param = new List<SqlParams>();
            param.Add(new SqlParams() { name = "@t", value = ts });
            param.Add(new SqlParams() { name = "@VAB_Bank_Acct_ID", value = VAB_Bank_Acct_ID });

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
        /// <param name="VAF_Language"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetTranslatedText(Ctx _ctx, List<string> Cols, string VAF_Language)
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
                    if (Env.IsBaseLanguage(_ctx, "VAF_ColumnDic"))
                    {
                        sqlElements = @"SELECT e.ColumnName, NVL(e.Name, '') AS Name FROM VAF_ColumnDic e WHERE e.ColumnName IN (" + Columns + ")";

                        sqlMsgs = @"SELECT m.Value, COALESCE(m.MsgText, m.Value) AS Name  FROM VAF_Msg_Lable m WHERE m.Value NOT  IN
                                (SELECT ColumnName FROM VAF_ColumnDic WHERE ColumnName IN (" + Columns + @")) AND m.Value IN (" + Columns + ")";
                    }
                    else
                    {
                        param = new System.Data.SqlClient.SqlParameter[1];
                        param[0] = new System.Data.SqlClient.SqlParameter("@VAF_Language", VAF_Language);

                        sqlElements = @"SELECT e.ColumnName, COALESCE(t.Name, e.Name) AS Name FROM VAF_ColumnDic e INNER JOIN VAF_ColumnDic_TL t ON (e.VAF_ColumnDic_ID = t.VAF_ColumnDic_ID)
                        WHERE  t.VAF_Language = @VAF_Language AND e.ColumnName IN (" + Columns + ")";

                        sqlMsgs = @"SELECT m.Value, COALESCE(t.MsgText, m.MsgText) AS Name  FROM VAF_Msg_Lable m LEFT JOIN VAF_Msg_Lable_TL t ON (m.VAF_Msg_Lable_ID = t.VAF_Msg_Lable_ID) WHERE m.Value NOT  IN
                            (SELECT ColumnName FROM VAF_ColumnDic WHERE ColumnName IN (" + Columns + ")) AND m.Value IN (" + Columns + ") AND t.VAF_Language = @VAF_Language";
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

        /// <summary>
        /// Get Theme list
        /// </summary>
        /// <returns>dynamic list</returns>
        internal List<dynamic> GetThemes()
        {
            List<dynamic> retObj = new List<dynamic>();
            string qry = " SELECT PrimaryColor, OnPrimaryColor, SecondaryColor, OnSecondaryColor " +
                                " , IsDefault, VAF_Theme_ID  FROM VAF_Theme WHERE IsActive='Y'";
            DataSet ds = DB.ExecuteDataset(qry);

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dynamic obj = new ExpandoObject();
                    obj.Id = Util.GetValueOfString(dr["VAF_Theme_ID"]);
                    obj.PColor = Util.GetValueOfString(dr["PrimaryColor"]);
                    obj.OnPColor = Util.GetValueOfString(dr["OnPrimaryColor"]);
                    obj.SColor = Util.GetValueOfString(dr["SecondaryColor"]);
                    obj.OnSColor = Util.GetValueOfString(dr["OnSecondaryColor"]);
                    obj.IsDefault = Util.GetValueOfString(dr["IsDefault"]);
                    retObj.Add(obj);
                }

            }
            return retObj;
        }



        /// <summary>
        /// Get Version information for changed columns
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="od"></param>
        /// <returns></returns>
        public dynamic GetVerDetails(Ctx ctx, dynamic od)
        {
            List<string> colNames = new List<string>();
            List<string> dbColNames = new List<string>();
            List<object> oldValues = new List<object>();
            // create list of default columns for which Version change details not required
            List<string> defColNames = new List<string>(new string[] { "Created", "CreatedBy", "Updated", "UpdatedBy", "Export_ID" });
            dynamic data = new ExpandoObject();
            string TableName = od.TName.Value;
            // Get original table name by removing "_Ver" suffix from the end
            string origTableName = TableName.Substring(0, TableName.Length - 4);
            var RecID = od.RID.Value;
            // Get parent table ID
            int VAF_TableView_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAF_TableView_ID FROM VAF_TableView WHERE TableName = '" + origTableName + "'", null, null));
            MTable tbl = new MTable(ctx, VAF_TableView_ID, null);
            DataSet dsColumns = null;
            // check if Maintain Version is marked on table
            if (tbl.IsMaintainVersions())
                dsColumns = DB.ExecuteDataset("SELECT Name, ColumnName, VAF_Column_ID, VAF_Control_Ref_ID FROM VAF_Column WHERE VAF_TableView_ID = " + VAF_TableView_ID);
            // else get columns on which maintain version is marked
            else
                dsColumns = DB.ExecuteDataset("SELECT Name, ColumnName, VAF_Column_ID, VAF_Control_Ref_ID FROM VAF_Column WHERE VAF_TableView_ID = " + VAF_TableView_ID + " AND IsMaintainVersions = 'Y'");
            // return if maintain version not found either on table or column level
            if (!(dsColumns != null && dsColumns.Tables[0].Rows.Count > 0))
                return data;

            DataSet dsFields = DB.ExecuteDataset("SELECT Name, VAF_Column_ID FROM VAF_Field WHERE VAF_Tab_ID = " + Util.GetValueOfInt(od.TabID.Value));
            StringBuilder sbSQL = new StringBuilder("");
            // check if table has single key
            if (tbl.IsSingleKey())
                sbSQL.Append(origTableName + "_ID = " + od[(origTableName + "_ID").ToLower()].Value);
            else
            {
                string[] keyCols = tbl.GetKeyColumns();
                for (int w = 0; w < keyCols.Length; w++)
                {
                    if (w == 0)
                    {
                        if (keyCols[w] != null)
                            sbSQL.Append(keyCols[w] + " = " + od[(keyCols[w]).ToLower()]);
                        else
                            sbSQL.Append(" NVL(" + keyCols[w] + ",0) = 0");
                    }
                    else
                    {
                        if (keyCols[w] != null)
                            sbSQL.Append(" AND " + keyCols[w] + " = " + od[(keyCols[w]).ToLower()]);
                        else
                            sbSQL.Append(" AND NVL(" + keyCols[w] + ",0) = 0");
                    }
                }
            }

            POInfo inf = POInfo.GetPOInfo(ctx, Util.GetValueOfInt(od.TblID.Value));
            // Get SQL Query from PO Info for selected table
            string sqlCol = GetSQLQuery(ctx, Util.GetValueOfInt(od.TblID.Value), inf.GetPoInfoColumns());

            //DataSet dsRec = DB.ExecuteDataset("SELECT * FROM " + TableName + " WHERE " + sbSQL.ToString() + " AND RecordVersion < " + od["recordversion"].Value + " ORDER BY RecordVersion DESC");
            //DataSet dsRec = DB.ExecuteDataset("SELECT * FROM " + TableName + " WHERE " + sbSQL.ToString() + " AND RecordVersion = " + Util.GetValueOfInt(od["oldversion"].Value));
            // get data from Version table according to the Record Version 
            DataSet dsRec = DB.ExecuteDataset(sqlCol + " WHERE " + sbSQL.ToString() + " AND RecordVersion = " + Util.GetValueOfInt(od["oldversion"].Value));
            DataRow dr = null;
            if (dsRec != null && dsRec.Tables[0].Rows.Count > 0)
                dr = dsRec.Tables[0].Rows[0];

            StringBuilder sbColName = new StringBuilder("");
            StringBuilder sbColValue = new StringBuilder("");
            for (int i = 0; i < dsColumns.Tables[0].Rows.Count; i++)
            {
                sbColName.Clear();
                sbColValue.Clear();
                sbColName.Append(Util.GetValueOfString(dsColumns.Tables[0].Rows[i]["ColumnName"]));
                if (!dr.Table.Columns.Contains(sbColName.ToString()))
                    continue;

                if (defColNames.Contains(sbColName.ToString()) || (sbColName.ToString() == origTableName + "_ID") || (sbColName.ToString() == origTableName + "_Ver_ID"))
                    continue;

                if (Util.GetValueOfInt(dsColumns.Tables[0].Rows[i]["VAF_Control_Ref_ID"]) == 20)
                    dr[sbColName.ToString()] = (Util.GetValueOfString(dr[sbColName.ToString()]) == "Y") ? true : false;

                var val = od[sbColName.ToString().ToLower()];
                if (val != null)
                {
                    if (Util.GetValueOfString(dr[sbColName.ToString()]) != Util.GetValueOfString(od[sbColName.ToString().ToLower()].Value))
                    {
                        colNames.Add(Util.GetValueOfString(dsColumns.Tables[0].Rows[i]["Name"]));
                        dbColNames.Add(sbColName.ToString());
                        // get text for column based on different reference types
                        if (dr.Table.Columns.Contains(sbColName.ToString() + "_TXT"))
                            sbColValue.Append(Util.GetValueOfString(dr[sbColName.ToString() + "_TXT"]));
                        else if (dr.Table.Columns.Contains(sbColName.ToString() + "_LOC"))
                            sbColValue.Append(Util.GetValueOfString(dr[sbColName.ToString() + "_LOC"]));
                        else if (dr.Table.Columns.Contains(sbColName.ToString() + "_LTR"))
                            sbColValue.Append(Util.GetValueOfString(dr[sbColName.ToString() + "_LTR"]));
                        else if (dr.Table.Columns.Contains(sbColName.ToString() + "_ASI"))
                            sbColValue.Append(Util.GetValueOfString(dr[sbColName.ToString() + "_ASI"]));
                        else if (dr.Table.Columns.Contains(sbColName.ToString() + "_ACT"))
                            sbColValue.Append(Util.GetValueOfString(dr[sbColName.ToString() + "_ACT"]));
                        else if (dr.Table.Columns.Contains(sbColName.ToString() + "_CTR"))
                            sbColValue.Append(Util.GetValueOfString(dr[sbColName.ToString() + "_CTR"]));
                        else
                            sbColValue.Append(Util.GetValueOfString(dr[sbColName.ToString()]));

                        oldValues.Add(sbColValue.ToString());
                    }
                }
            }

            data.ColumnNames = colNames;
            data.OldVals = oldValues;
            data.DBColNames = dbColNames;

            return data;
        }

        public string GetSQLQuery(Ctx m_ctx, int _VAF_TableView_ID, POInfoColumn[] m_columns)
        {
            StringBuilder _querySQL = new StringBuilder("");
            if (m_columns.Length > 0)
            {
                _querySQL.Append("SELECT ");
                MTable tbl = new MTable(m_ctx, _VAF_TableView_ID, null);
                // append all columns from table and get comma separated string
                _querySQL.Append(tbl.GetSelectColumns());
                foreach (var column in m_columns)
                {
                    // check if column name length is less than 26, then only add this column in selection column
                    // else only ID will be displayed
                    // as limitation in oracle to restrict column name to 30 characters
                    if ((column.ColumnName.Length + 4) < 30)
                    {
                        // for Lookup type of columns
                        if (DisplayType.IsLookup(column.DisplayType))
                        {
                            VLookUpInfo lookupInfo = VLookUpFactory.GetLookUpInfo(m_ctx, 0, column.DisplayType,
                                column.VAF_Column_ID, Env.GetLanguage(m_ctx), column.ColumnName, column.VAF_Control_Ref_Value_ID,
                                column.IsParent, column.ValidationCode);

                            if (lookupInfo != null && lookupInfo.displayColSubQ != null && lookupInfo.displayColSubQ.Trim() != "")
                            {
                                if (lookupInfo.queryDirect.Length > 0)
                                {
                                    // create columnname as columnname_TXT for lookup type of columns
                                    lookupInfo.displayColSubQ = " (SELECT MAX(" + lookupInfo.displayColSubQ + ") " + lookupInfo.queryDirect.Substring(lookupInfo.queryDirect.LastIndexOf(" FROM " + lookupInfo.tableName + " "), lookupInfo.queryDirect.Length - (lookupInfo.queryDirect.LastIndexOf(" FROM " + lookupInfo.tableName + " "))) + ") AS " + column.ColumnName + "_TXT";

                                    lookupInfo.displayColSubQ = lookupInfo.displayColSubQ.Replace("@key", tbl.GetTableName() + "." + column.ColumnName);
                                }
                                _querySQL.Append(", " + lookupInfo.displayColSubQ);
                            }
                        }
                        // case for Location type of columns
                        else if (column.DisplayType == DisplayType.Location)
                        {
                            _querySQL.Append(", " + column.ColumnName + " AS " + column.ColumnName + "_LOC");
                        }
                        // case for Locator type of columns
                        else if (column.DisplayType == DisplayType.Locator)
                        {
                            _querySQL.Append(", " + column.ColumnName + " AS " + column.ColumnName + "_LTR");
                        }
                        // case for Attribute Set Instance & General Attribute columns
                        else if (column.DisplayType == DisplayType.PAttribute || column.DisplayType == DisplayType.GAttribute)
                        {
                            _querySQL.Append(", " + column.ColumnName + " AS " + column.ColumnName + "_ASI");
                        }
                        // case for Account type of columns
                        else if (column.DisplayType == DisplayType.Account)
                        {
                            _querySQL.Append(", " + column.ColumnName + " AS " + column.ColumnName + "_ACT");
                        }
                    }
                }
                // Append FROM table name to query
                _querySQL.Append(" FROM " + tbl.GetTableName());
            }
            return _querySQL.ToString();
        }

    }
}