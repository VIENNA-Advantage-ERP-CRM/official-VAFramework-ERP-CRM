using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.DBase;

namespace VIS.Models
{
    public class InfoWindowModel
    {
        //public int ID
        //{
        //    get;
        //    set;
        //}
        //public string name
        //{
        //    get;
        //    set;
        //}
        //public bool openDialog
        //{
        //    get;
        //    set;
        //}
        //public string WindowName
        //{
        //    get;
        //    set;
        //}
        //public string FromClause
        //{
        //    get;
        //    set;
        //}
        //public string OTHERCLAUSE
        //{
        //    get;
        //    set;
        //}
        //public string SearchHtml
        //{
        //    get;
        //    set;
        //}

        //public List<InfoSchema> lstSchema
        //{
        //    get;
        //    set;
        //}
        //public List<DisplayContent> lstContent
        //{
        //    get;
        //    set;
        //}

        public Info GetSchema(int AD_InfoWindow_ID, Ctx ctx)
        {
            try
            {
                bool isBaseLanguage = VAdvantage.Utility.Env.IsBaseLanguage(ctx, "");
                string sql = string.Empty;
                if (isBaseLanguage)
                {
                    sql = @"SELECT 
                                            IW.Name AS WindowName,
                                            AD_InfoColumn.Name,
                                            AD_InfoColumn.AD_Reference_ID,
                                            AD_InfoColumn.AD_Reference_Value_ID,
                                            AD_InfoColumn.IsQueryCriteria,
                                            AD_InfoColumn.SelectClause,
                                            AD_InfoColumn.AD_SetValue,
                                            AD_InfoColumn.AD_Condition,
                                            IW.FromClause,
                                            IW.OTHERCLAUSE,
                                            AD_ELEMENT.ColumnName,
                                            AD_ELEMENT.Description,
                                            AD_InfoColumn.IsDisplayed,
                                            AD_InfoColumn.IsKey,                                           
                                            AD_InfoColumn.IsRange,
                                            AD_InfoColumn.ISIDENTIFIER,
                                            infotable.TableName
                                            FROM AD_InfoColumn
                                            INNER JOIN AD_ELEMENT
                                            ON (AD_ELEMENT.AD_ELEMENT_ID =AD_InfoColumn.AD_ELEMENT_ID)
                                            INNER JOIN AD_InfoWIndow IW
                                            ON(IW.AD_InfoWindow_ID            =AD_InfoColumn.AD_InfoWindow_ID)
                                            INNER JOIN AD_Table infotable
                                            ON(infotable.AD_Table_ID=IW.AD_Table_ID) 
                                            WHERE AD_InfoColumn.IsActive      ='Y'
                                            AND AD_InfoColumn.AD_InfoWindow_ID=" + AD_InfoWindow_ID + " ORDER BY AD_InfoColumn.seqno";
                }
                else  //get column header from translation
                {
                    sql = @"SELECT IWT.Name AS WindowName,
                                                              AD_InfoColumn_trl.Name,
                                                              AD_InfoColumn.AD_Reference_ID,
                                                              AD_InfoColumn.AD_Reference_Value_ID,
                                                              AD_InfoColumn.IsQueryCriteria,
                                                              AD_InfoColumn.SelectClause,
                                                              AD_InfoColumn.AD_SetValue,
                                                              AD_InfoColumn.AD_Condition,
                                                              IW.FromClause,
                                                              IW.OTHERCLAUSE,
                                                              AD_ELEMENT.ColumnName,
                                                              AD_ELEMENT.Description,
                                                              AD_InfoColumn.IsDisplayed,
                                                              AD_InfoColumn.IsKey,
                                                              AD_InfoColumn.AD_Reference_Value_ID,
                                                              AD_InfoColumn.IsRange,
                                                              AD_InfoColumn.ISIDENTIFIER,
                                                              infotable.TableName
                                                            FROM AD_InfoColumn
                                                            INNER JOIN AD_ELEMENT
                                                            ON (AD_ELEMENT.AD_ELEMENT_ID =AD_InfoColumn.AD_ELEMENT_ID)
                                                            INNER JOIN AD_InfoWIndow IW
                                                            ON(IW.AD_InfoWindow_ID =AD_InfoColumn.AD_InfoWindow_ID)
                                                            INNER JOIN AD_InfoColumn_trl
                                                            ON (AD_InfoColumn_trl.AD_InfoColumn_ID=AD_InfoColumn.AD_InfoColumn_ID
                                                            AND AD_InfoColumn_trl.AD_language     ='" + ctx.GetAD_Language() + @"')
                                                            INNER JOIN AD_InfoWindow_trl IWT
                                                            ON (IWT.AD_InfoWindow_ID          =IW.AD_InfoWindow_ID
                                                            AND IWT.AD_language               ='" + ctx.GetAD_Language() + @"')
                                                           INNER JOIN AD_Table infotable
                                                            ON(infotable.AD_Table_ID=IW.AD_Table_ID)                        
                                                            WHERE AD_InfoColumn.IsActive      ='Y'
                                                            AND AD_InfoColumn.AD_InfoWindow_ID=" + AD_InfoWindow_ID + " ORDER BY AD_InfoColumn.seqno";
                }
                DataSet ds = DBase.DB.ExecuteDataset(sql);
                return GetSchema(ds, ctx);
                //lstContent = GetDisplayColList(ds);
                //infoWinSelect = ds.Tables[0].Rows[0]["FromClause"].ToString();
                //otherSql = ds.Tables[0].Rows[0]["OTHERCLAUSE"].ToString();

            }
            catch
            {
                return null;
            }
        }

        private Info GetSchema(DataSet ds, Ctx ctx)
        {
            try
            {
                //throw new NotImplementedException();
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return null;
                }

                Info info = new Info();


                List<InfoSchema> lstSchema = new List<InfoSchema>();

                InfoSchema schema = null;
                string cName = string.Empty;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    schema = new InfoSchema();
                    cName = ds.Tables[0].Rows[i]["SELECTCLAUSE"].ToString();
                    cName = (cName.Contains('.')) ? cName.Substring(cName.LastIndexOf('.') + 1) : cName;
                    //dbcol.Add(cName);
                    if (ds.Tables[0].Rows[i]["IsQueryCriteria"].ToString() == "N")
                    {
                        //if (ds.Tables[0].Rows[i]["IsDisplayed"].ToString() == "Y")
                        //{
                        //    colName.Add(ds.Tables[0].Rows[i]["Name"].ToString());
                        //}

                        schema.IsQueryCriteria = false;
                    }
                    else
                    {
                        schema.IsQueryCriteria = true;
                    }
                    info.WindowName = ds.Tables[0].Rows[i]["WindowName"].ToString();
                    info.TableName = ds.Tables[0].Rows[i]["TableName"].ToString();
                    schema.Name = ds.Tables[0].Rows[i]["Name"].ToString();

                    if (ds.Tables[0].Rows[i]["AD_Reference_ID"] != null &&
                        ds.Tables[0].Rows[i]["AD_Reference_ID"] != DBNull.Value)
                    {
                        schema.AD_Reference_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Reference_ID"]);
                        if (schema.AD_Reference_ID == 17)//if reference is List
                        {
                            schema.RefList = GetRefList(Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Reference_Value_ID"]), ctx);
                        }
                    }
                    if (ds.Tables[0].Rows[i]["AD_Reference_Value_ID"] != null &&
                        ds.Tables[0].Rows[i]["AD_Reference_Value_ID"] != DBNull.Value)
                    {
                        schema.AD_Reference_Value_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Reference_Value_ID"]);

                    }
                    else
                    {
                        schema.AD_Reference_Value_ID = 0;
                    }
                    if (ds.Tables[0].Rows[i]["SelectClause"] != null &&
                       ds.Tables[0].Rows[i]["SelectClause"] != DBNull.Value)
                    {
                        schema.SelectClause = (ds.Tables[0].Rows[i]["SelectClause"]).ToString();
                    }
                    if (ds.Tables[0].Rows[i]["AD_SetValue"] != null &&
                      ds.Tables[0].Rows[i]["AD_SetValue"] != DBNull.Value)
                    {
                        schema.SetValue = (ds.Tables[0].Rows[i]["AD_SetValue"]).ToString();
                    }
                    if (ds.Tables[0].Rows[i]["AD_Condition"] != null &&
                    ds.Tables[0].Rows[i]["AD_Condition"] != DBNull.Value)
                    {
                        schema.Condition = (ds.Tables[0].Rows[i]["AD_Condition"]).ToString();
                    }
                    if (ds.Tables[0].Rows[i]["FromClause"] != null &&
                       ds.Tables[0].Rows[i]["FromClause"] != DBNull.Value)
                    {
                        info.FromClause = (ds.Tables[0].Rows[i]["FromClause"]).ToString();
                    }
                    if (ds.Tables[0].Rows[i]["OTHERCLAUSE"] != null &&
                       ds.Tables[0].Rows[i]["OTHERCLAUSE"] != DBNull.Value)
                    {
                        info.OTHERCLAUSE = (ds.Tables[0].Rows[i]["OTHERCLAUSE"]).ToString();
                    }
                    if (ds.Tables[0].Rows[i]["ColumnName"] != null &&
                       ds.Tables[0].Rows[i]["ColumnName"] != DBNull.Value)
                    {
                        schema.ColumnName = (ds.Tables[0].Rows[i]["ColumnName"]).ToString();
                    }
                    if (ds.Tables[0].Rows[i]["Description"] != null &&
                       ds.Tables[0].Rows[i]["Description"] != DBNull.Value)
                    {
                        schema.Description = (ds.Tables[0].Rows[i]["Description"]).ToString();
                    }

                    if (ds.Tables[0].Rows[i]["IsKey"].ToString() == "N")
                    {
                        schema.IsKey = false;
                    }
                    else
                    {
                        schema.IsKey = true;
                    }
                    if (ds.Tables[0].Rows[i]["IsRange"].ToString() == "N")
                    {
                        schema.IsRange = false;
                    }
                    else
                    {
                        schema.IsRange = true;
                    }
                    if (ds.Tables[0].Rows[i]["ISIDENTIFIER"].ToString() == "N")
                    {
                        schema.ISIDENTIFIER = false;
                    }
                    else
                    {
                        schema.ISIDENTIFIER = true;
                    }

                    if (ds.Tables[0].Rows[i]["IsDisplayed"].ToString() == "N")
                    {
                        schema.IsDisplayed = false;
                    }
                    else
                    {
                        schema.IsDisplayed = true;
                    }
                    //if (schema.AD_Reference_ID == DisplayType.Search
                    //    || schema.AD_Reference_ID == DisplayType.Table
                    //    || schema.AD_Reference_ID == DisplayType.TableDir)
                    //{
                    //    schema.lookup = VLookUpFactory.Get(ctx, 0, 0, schema.AD_Reference_ID, schema.ColumnName, schema.AD_Reference_Value_ID, false, null);
                    //}

                    lstSchema.Add(schema);
                }

                info.Schema = lstSchema;
                return info;
            }
            catch
            {
                return null;
            }
        }

        private List<InfoRefList> GetRefList(int AD_Reference_ID, Ctx ctx)
        {

            //String sql = "SELECT Value, Name FROM AD_Ref_List "
            //    + "WHERE AD_Reference_ID=" + AD_Reference_ID + " AND IsActive='Y' ORDER BY 1";
            //DataSet ds = null;

            List<InfoRefList> list = new List<InfoRefList>();
            try
            {
                ValueNamePair[] refList = MRefList.GetList(AD_Reference_ID, true, ctx);
                //ds = DB.ExecuteDataset(sql, null, null);
                InfoRefList itm = null;// new InfoRefList();
                                       // itm.Key = "";
                                       // itm.Value = "";
                                       // list.Add(itm);
                for (int i = 0; i < refList.Length; i++)
                {
                    itm = new InfoRefList();
                    itm.Key = refList[i].GetKeyID().ToString();//["Value"].ToString();
                    itm.Value = refList[i].GetValue();// ds.Tables[0].Rows[i]["Name"].ToString();

                    list.Add(itm);
                }
                refList = null;
            }
            catch (Exception)
            {

            }

            return list;


        }




        public InfoData GetData(string tableName, int pageNo, VAdvantage.Utility.Ctx ctx, string selectedIDs
            , bool requery, Info info, string validationCode, List<InfoSearchCol> srchCtrls)
        {

            string sql = "SELECT ";
            string colName = null;
            string tabname = null;
            string keyCol = null;
            string cName = null;
            int displayType = 0;
            List<InfoSchema> schema = info.Schema;
            if (schema != null)
            { 
            //get Qry from InfoColumns
            for (int item = 0; item < schema.Count; item++)
            {
                colName = schema[item].SelectClause;
                if (colName.IndexOf('.') > -1)
                {
                    cName = colName.Substring(colName.LastIndexOf('.') + 1);
                }
                else
                {
                    cName = colName;
                }

                if (schema[item].IsKey)
                {
                    keyCol = cName.ToUpper();
                    tabname = keyCol.Substring(0, keyCol.IndexOf("_ID"));
                }
                displayType = schema[item].AD_Reference_ID;
                if (displayType == DisplayType.YesNo)
                {
                    sql += " ( CASE " + colName + " WHEN 'Y' THEN  'True' ELSE 'False'  END ) AS " + (cName);
                }
                else if (displayType == DisplayType.List)
                {
                    //    ValueNamePair[] values = MRefList.GetList(Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_REFERENCE_VALUE_ID"]), false);
                    //    sql.Append(" CASE ");
                    //    for (int j = 0; j < values.Length; j++)
                    //    {
                    //        sql.Append(" WHEN " + colName + "='" + values[j].Key.ToString() + "' THEN '" + values[j].Name.ToString() + "'");
                    //    }
                    //    sql.Append(" END AS " + cName);
                    var refList = schema[item].RefList;
                    sql += (" CASE ");
                    foreach (var refListItem in refList)
                    {
                        sql += " WHEN " + colName + "='" + refListItem.Key + "' THEN '" + refListItem.Value + "'";
                    }
                    sql += " END AS " + cName;
                    //sql += colName + " ";
                }
                else
                {
                    sql += colName + " ";
                }
                if (!((schema.Count - 1) == item))
                {
                    sql += ", ";
                }

            }
        }

            if (selectedIDs != null && selectedIDs.Length > 0)
            {
                sql += ", 'N' AS ordcol";
            }


            // sql=sql.substring(0,sql.Length-2)
            sql += " FROM " + info.FromClause;
            string sqlOrderby = "";
            if (requery == true)
            {
                //Get Where Clause From SearchControls
                //  debugger;
                var whereClause = " ";
                string srchValue = null;
                var appendAND = false;
                for (var i = 0; i < srchCtrls.Count; i++)
                {
                    srchValue = Convert.ToString(srchCtrls[i].Value);

                    //JID_0905:  In Case of Date Range, if From Date is not selected then check if To Date is selected
                    if (srchCtrls[i].AD_Reference_ID == DisplayType.Date && srchCtrls[i].IsRange)
                    {
                        if (srchValue == null)
                        {
                            srchValue = Convert.ToString(srchCtrls[i].Value);
                        }
                    }

                    // Consider checkbox value only in case of true value
                    if (srchValue == null || srchValue.Length == 0 || (srchValue == "0" && srchCtrls[i].AD_Reference_ID != DisplayType.YesNo) || srchValue == "-1")
                    {
                        continue;
                    }

                    {
                        if (appendAND == true)
                        {
                            whereClause += " AND ";
                        }
                        if (srchCtrls[i].AD_Reference_ID == DisplayType.String
                            || srchCtrls[i].AD_Reference_ID == DisplayType.Text
                            || srchCtrls[i].AD_Reference_ID == DisplayType.TextLong)
                        {


                            if (!((srchValue).IndexOf("%") == 0))
                            {
                                srchValue = "●" + srchValue;
                            }
                            else
                            {
                                srchValue = (srchValue).Replace("%", "●");
                            }
                            if (!(((srchValue).LastIndexOf("●")) == ((srchValue).Length)))
                            {
                                srchValue = srchValue + "●";
                            }
                            srchValue = GlobalVariable.TO_STRING(srchValue);
                            whereClause += "  UPPER(" + srchCtrls[i].SearchColumnName + ") LIKE " + srchValue.ToUpper();
                        }
                        else if (srchCtrls[i].AD_Reference_ID == DisplayType.Date)
                        {
                            string fromValue = null;
                            string toValue = null;
                            DateTime date = Convert.ToDateTime((srchCtrls[i].Value));
                            fromValue = "TO_DATE( '" + ((date.Month) + 1) + "-" + date.Day + "-" + date.Year + "', 'MM-DD-YYYY')";// GlobalVariable.TO_DATE(Util.GetValueOfDateTime(srchCtrls[i].Ctrl.getValue()), true);

                            if (srchCtrls[i].IsRange)
                            {
                                // JID_0905: If To Date is empty, then select system date
                                if (srchCtrls[i].Value == null)
                                {
                                    date = DateTime.Now;
                                }
                                else
                                {
                                    date = Convert.ToDateTime((srchCtrls[i].ValueTo));
                                }
                                toValue = "TO_DATE( '" + ((date.Month) + 1) + "-" + date.Day + "-" + date.Year + "', 'MM-DD-YYYY')";// GlobalVariable.TO_DATE(Util.GetValueOfDateTime(srchCtrls[i].Ctrl.getValue()), true);
                                whereClause += " ( " + srchCtrls[i].SearchColumnName + " BETWEEN " + fromValue + " AND " + toValue + ")";
                            }
                            else
                            {
                                whereClause += srchCtrls[i].SearchColumnName + " =" + fromValue;
                            }
                        }
                        else if (srchCtrls[i].AD_Reference_ID == DisplayType.DateTime)
                        {
                            string fromValue = null;
                            string toValue = null;
                            var date = Convert.ToDateTime((srchCtrls[i].Value));
                            fromValue = "TO_DATE( '" + ((date.Month) + 1) + "-" + date.Day + "-" + date.Year + " " + date.Hour + ":" + date.Minute + ":" + date.Second + "', 'MM-DD-YYYY HH24:MI:SS')";// GlobalVariable.TO_DATE(Util.GetValueOfDateTime(srchCtrls[i].Ctrl.getValue()), true);

                            if (srchCtrls[i].IsRange)
                            {
                                date = Convert.ToDateTime((srchCtrls[i].ValueTo));
                                toValue = "TO_DATE( '" + ((date.Month) + 1) + "-" + date.Day + "-" + date.Year + " " + date.Hour + ":" + date.Minute + ":" + date.Second + "', 'MM-DD-YYYY HH24:MI:SS')";// GlobalVariable.TO_DATE(Util.GetValueOfDateTime(srchCtrls[i].Ctrl.getValue()), true);
                                whereClause += " ( " + srchCtrls[i].SearchColumnName + " BETWEEN " + fromValue + " AND " + toValue + ")";
                            }
                            else
                            {
                                whereClause += srchCtrls[i].SearchColumnName + " =" + fromValue;
                            }
                        }

                        else if (srchCtrls[i].AD_Reference_ID == DisplayType.YesNo)
                        {


                            srchValue = Convert.ToBoolean(srchCtrls[i].Value) == true ? "Y" : "N";
                            whereClause += srchCtrls[i].SearchColumnName + " = '" + srchValue + "'";

                        }
                        else
                        {
                            string fromValue = null;
                            string toValue = null;
                            fromValue = Convert.ToString(srchCtrls[i].Value);
                            if (srchCtrls[i].IsRange)
                            {
                                //if (srchCtrls[i].Value)
                                //{     // Consider checkbox value only in case of true value
                                    srchValue = Convert.ToBoolean(srchCtrls[i].Value) == true ? "Y" : "";
                                    whereClause += srchCtrls[i].SearchColumnName + " = '" + srchValue + "'";
                                //}
                            }
                            else
                            {
                                whereClause += " " + srchCtrls[i].SearchColumnName + " ='" + fromValue + "'";
                            }


                        }




                        appendAND = true;



                    }

                }

                // VIS0060: Handled case of alise name on Info window.
                sql = MRole.GetDefault(ctx).AddAccessSQL(sql, info.TableName, true, false);

                if (whereClause.Length > 1)
                {
                    if (info.FromClause.ToUpper().IndexOf("WHERE") > -1 || sql.ToUpper().IndexOf("WHERE") > -1)
                    {
                        sql += " AND " + whereClause;
                    }
                    else
                    {
                        sql += " WHERE " + whereClause;
                    }
                    if (validationCode != null && validationCode.Length > 0)
                    {
                        sql += " AND " + validationCode;
                    }
                }
                else if (validationCode != null && validationCode.Length > 0)
                {
                    if (info.FromClause != null)
                    {
                        if (info.FromClause.ToUpper().IndexOf("WHERE") > -1 || sql.ToUpper().IndexOf("WHERE") > -1)
                        {
                            sql += " AND " + validationCode;
                        }
                        else
                        {
                            sql += " WHERE " + validationCode;
                        }
                    }
                }







                if (info.OTHERCLAUSE != null)
                {
                    if (selectedIDs != null && selectedIDs.Length > 0)
                    {
                        var otherClause = "";
                        if (info.OTHERCLAUSE.IndexOf("ORDER BY") > -1)
                        {
                            otherClause = info.OTHERCLAUSE.Substring(0, info.OTHERCLAUSE.IndexOf("ORDER BY")); ;
                            sqlOrderby = info.OTHERCLAUSE.Substring(info.OTHERCLAUSE.IndexOf("ORDER BY"));
                        }
                        if (otherClause != null && otherClause.Length > 0)

                            if ((otherClause).ToUpper().IndexOf("WHERE") > -1)
                            {
                                if (info.FromClause.ToUpper().IndexOf("WHERE") > -1
                                    || whereClause.Length > 1
                                    || (validationCode != null && validationCode.Length > 1))
                                {
                                    otherClause = (otherClause).Replace("WHERE", "AND");
                                }

                            }
                        sql += " " + otherClause;
                    }
                    else
                    {
                        if ((info.OTHERCLAUSE).ToUpper().IndexOf("WHERE") > -1)
                        {
                            if (info.FromClause.ToUpper().IndexOf("WHERE") > -1
                                || whereClause.Length > 1
                                || (validationCode != null && validationCode.Length > 1))
                            {
                                info.OTHERCLAUSE = (info.OTHERCLAUSE).Replace("WHERE", "AND");
                            }

                        }
                        sql += " " + info.OTHERCLAUSE;
                    }
                }
            }
            else
            {
                if (info.FromClause.ToUpper().IndexOf("WHERE") > -1)
                {
                    sql += " AND rownum=-1";
                }
                else
                {
                    sql += " WHERE rownum=-1";
                }

            }



            if (selectedIDs != null && selectedIDs.Length > 0)
            {
                var sqlUnion = " UNION " + sql;
                sqlUnion = sqlUnion.Replace("'N' AS ordcol", "'Y' AS ordcol");

                if (sql.ToUpper().IndexOf("WHERE") > -1)
                {
                    sql += " AND " + tabname + "." + keyCol + " IN(" + selectedIDs + ")";
                    sql += sqlUnion;
                    sql += " AND " + tabname + "." + keyCol + " NOT IN(" + selectedIDs + ")";
                }
                //else {
                //    sql += " WHERE " + tabname + "." + keyCol + " IN(" + selectedIDs + ")";
                //    sql += sqlUnion;
                //    sql += " AND " + tabname + "." + keyCol + " NOT IN(" + selectedIDs + ")";
                //}
                sql = "SELECT * FROM (" + sql + ") tblquery";
            }


            if (selectedIDs != null && selectedIDs.Length > 0)
            {
                if (sqlOrderby.ToUpper().IndexOf("ORDER BY") > -1)
                {
                    sqlOrderby = sqlOrderby.ToUpper().Replace("ORDER BY", "ORDER BY ordcol ASC,");


                    //sql = sql.substr(0, sql.IndexOf("ORDER BY"));
                    string[] sqlOrderbyArr = sqlOrderby.Split(',');
                    var finalorderBy = " ORDER BY ";
                    for (var l = 0; l < sqlOrderbyArr.Length; l++)
                    {
                        sqlOrderbyArr[l] = sqlOrderbyArr[l].Replace("ORDER BY ", "");
                        if (sqlOrderbyArr[l].IndexOf(".") > -1)
                        {
                            sqlOrderbyArr[l] = sqlOrderbyArr[l].Replace(sqlOrderbyArr[l].Substring(0, sqlOrderbyArr[l].IndexOf(".") + 1), "tblquery.");
                        }
                        else
                        {
                            sqlOrderbyArr[l] = "tblquery." + sqlOrderbyArr[l];
                        }
                            
                            if (l > 0)
                            finalorderBy += ",";
                        finalorderBy += sqlOrderbyArr[l];
                    }
                    sql += finalorderBy;
                }
                else
                {
                    sql += " ORDER BY ordcol ASC";
                }



            }



            InfoData _iData = new InfoData();
            try
            {
                sql = sql.Replace('●', '%');
                sql = ParseContext(sql, ctx);
                //sql = MRole.GetDefault(ctx).AddAccessSQL(sql, tableName,
                //               MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);


                int totalRec = Util.GetValueOfInt(VAdvantage.DataBase.DB.ExecuteScalar("SELECT COUNT(*) FROM ( " + sql + " ) t", null, null));
                int pageSize = 50;
                PageSetting pSetting = new PageSetting();
                pSetting.CurrentPage = pageNo;
                pSetting.TotalRecords = totalRec;
                pSetting.PageSize = pageSize;
                pSetting.TotalPage = (totalRec % pageSize) == 0 ? (totalRec / pageSize) : ((totalRec / pageSize) + 1);
                _iData.pSetting = pSetting;

                DataSet data = DBase.DB.ExecuteDatasetPaging(sql, pageNo, pageSize);
                if (data == null)
                {
                    return null;
                }
                List<DataObject> dyndata = new List<DataObject>();
                DataObject item = null;
                List<object> values = null;
                for (int i = 0; i < data.Tables[0].Columns.Count; i++)  //columns
                {
                    item = new DataObject();

                    item.ColumnName = data.Tables[0].Columns[i].ColumnName;
                    values = new List<object>();
                    for (int j = 0; j < data.Tables[0].Rows.Count; j++)  //rows
                    {

                        values.Add(data.Tables[0].Rows[j][data.Tables[0].Columns[i].ColumnName]);
                    }
                    item.Values = values;
                    item.RowCount = data.Tables[0].Rows.Count;
                    dyndata.Add(item);
                }
                _iData.data = dyndata;
                return _iData;
            }
            catch (Exception ex)
            {
                _iData.Error = ex.Message;
                return _iData;
            }
        }

        private string ParseContext(string res, Ctx ctx)
        {
            string token = string.Empty;
            string value = string.Empty;
            while (res.Contains('@'))
            {
                token = res.Substring(res.IndexOf('@') + 1);
                token = token.Substring(0, token.IndexOf('@'));
                if (token.StartsWith("#"))
                {
                    value = ctx.GetContext(token);
                }
                else
                {
                    value = ctx.GetContext("#" + token);
                }
                res = res.Replace("@" + token + "@", value);

            }
            return res;
        }

        /// <summary>
        /// Method to get the info window id from search key passed.- Added by mohit - 13 Feb 2019.
        /// </summary>
        /// <param name="InfoSearchKey"></param> search key of info window.
        /// <returns></returns> info window id.
        public int GetInfoWindowID(string InfoSearchKey)
        {
            return Util.GetValueOfInt(VAdvantage.DataBase.DB.ExecuteScalar("SELECT AD_InfoWindow_ID FROM AD_InfoWindow WHERE IsActive='Y' AND Value='" + InfoSearchKey + "'", null, null));
        }

    }


    public class InfoSchema
    {

        public string Name
        {
            get;
            set;
        }
        public int AD_Reference_ID
        {
            get;
            set;
        }
        public int AD_Reference_Value_ID
        {
            get;
            set;
        }

        public bool IsQueryCriteria
        {
            get;
            set;
        }
        public string SelectClause
        {
            get;
            set;
        }
        public string SetValue
        {
            get;
            set;
        }
        public string Condition
        {
            get;
            set;
        }
        public string ColumnName
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public bool IsDisplayed
        {
            get;
            set;
        }
        public bool IsKey
        {
            get;
            set;
        }
        public bool IsRange
        {
            get;
            set;
        }
        public bool ISIDENTIFIER
        {
            get;
            set;
        }

        public List<InfoRefList> RefList
        {
            get;
            set;
        }
        public Lookup lookup
        {
            get;
            set;
        }

    }

    public class InfoRefList
    {
        public string Key
        {
            get;
            set;
        }
        public string Value
        {
            get;
            set;
        }
    }

    public class InfoSearchCol
    {
        public int AD_Reference_ID
        {
            get;
            set;
        }

        public string ColumnName
        {
            get;
            set;
        }

        public bool IsRange
        {
            get;
            set;
        }

        public string SearchColumnName
        {
            get;
            set;
        }

        public object Value
        {

            get; set;
        }

        public object ValueTo
        {

            get; set;
        }
    }
    //public class DisplayContent
    //{

    //    //public DisplayContent(string columnName, string header, bool isDisplayed, bool isKeyColumn, int displayType)
    //    //{
    //    //    ColumnName = columnName;
    //    //    Header = header;
    //    //    IsDisplayed = isDisplayed;
    //    //    IsKeyColumn = isKeyColumn;
    //    //    DisplayType = displayType;
    //    //}
    //    public string ColumnName
    //    {
    //        get;
    //        set;
    //    }
    //    public bool IsDisplayed
    //    {
    //        get;
    //        set;
    //    }
    //    public bool IsKeyColumn
    //    {
    //        get;
    //        set;
    //    }
    //    public string Header
    //    {
    //        get;
    //        set;
    //    }
    //    public int DisplayType
    //    {
    //        get;
    //        set;
    //    }
    //}
    public class Info
    {
        public int ID
        {
            get;
            set;
        }

        public string WindowName
        {
            get;
            set;
        }
        public string TableName
        {
            get;
            set;
        }
        public string FromClause
        {
            get;
            set;
        }
        public string OTHERCLAUSE
        {
            get;
            set;
        }
        public List<InfoSchema> Schema
        {
            get;
            set;
        }

    }

    public class InfoData
    {
        public List<DataObject> data
        {
            get;
            set;
        }
        public PageSetting pSetting
        {
            get;
            set;
        }
        public string Error
        {
            get;
            set;
        }
    }
    public class DataObject
    {
        public string ColumnName
        {
            get;
            set;
        }
        public string Header
        {
            get;
            set;
        }
        public int RowCount
        {
            get;
            set;
        }
        public List<object> Values
        {
            get;
            set;
        }
    }
    public class PageSetting
    {
        public int CurrentPage
        {
            get;
            set;
        }
        public int TotalPage
        {
            get;
            set;
        }
        public int TotalRecords
        {
            get;
            set;
        }
        public int PageSize
        {
            get;
            set;
        }
    }
}