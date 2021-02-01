using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using VAdvantage.Classes;
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

        public Info GetSchema(int VAF_QuickSearchWindow_ID,Ctx ctx)
        {
            try
            {
                bool isBaseLanguage = VAdvantage.Utility.Env.IsBaseLanguage(ctx, "");
                string sql = string.Empty;
                if (isBaseLanguage)
                {
                    sql = @"SELECT 
                                            IW.Name AS WindowName,
                                            VAF_QuickSearchColumn.Name,
                                            VAF_QuickSearchColumn.VAF_Control_Ref_ID,
                                            VAF_QuickSearchColumn.VAF_Control_Ref_Value_ID,
                                            VAF_QuickSearchColumn.IsQueryCriteria,
                                            VAF_QuickSearchColumn.SelectClause,
                                            VAF_QuickSearchColumn.AD_SetValue,
                                            VAF_QuickSearchColumn.AD_Condition,
                                            IW.FromClause,
                                            IW.OTHERCLAUSE,
                                            VAF_COLUMNDIC.ColumnName,
                                            VAF_COLUMNDIC.Description,
                                            VAF_QuickSearchColumn.IsDisplayed,
                                            VAF_QuickSearchColumn.IsKey,                                           
                                            VAF_QuickSearchColumn.IsRange,
                                            VAF_QuickSearchColumn.ISIDENTIFIER,
                                            infotable.TableName
                                            FROM VAF_QuickSearchColumn
                                            INNER JOIN VAF_COLUMNDIC
                                            ON (VAF_COLUMNDIC.VAF_COLUMNDIC_ID =VAF_QuickSearchColumn.VAF_COLUMNDIC_ID)
                                            INNER JOIN VAF_QuickSearchWindow IW
                                            ON(IW.VAF_QuickSearchWindow_ID            =VAF_QuickSearchColumn.VAF_QuickSearchWindow_ID)
                                            INNER JOIN VAF_TableView infotable
                                            ON(infotable.VAF_TableView_ID=IW.VAF_TableView_ID) 
                                            WHERE VAF_QuickSearchColumn.IsActive      ='Y'
                                            AND VAF_QuickSearchColumn.VAF_QuickSearchWindow_ID=" + VAF_QuickSearchWindow_ID + " ORDER BY VAF_QuickSearchColumn.seqno";
                }
                else  //get column header from translation
                {
                    sql = @"SELECT IWT.Name AS WindowName,
                                                              VAF_QuickSearchColumn_TL.Name,
                                                              VAF_QuickSearchColumn.VAF_Control_Ref_ID,
                                                              VAF_QuickSearchColumn.VAF_Control_Ref_Value_ID,
                                                              VAF_QuickSearchColumn.IsQueryCriteria,
                                                              VAF_QuickSearchColumn.SelectClause,
                                                              VAF_QuickSearchColumn.AD_SetValue,
                                                              VAF_QuickSearchColumn.AD_Condition,
                                                              IW.FromClause,
                                                              IW.OTHERCLAUSE,
                                                              VAF_COLUMNDIC.ColumnName,
                                                              VAF_COLUMNDIC.Description,
                                                              VAF_QuickSearchColumn.IsDisplayed,
                                                              VAF_QuickSearchColumn.IsKey,
                                                              VAF_QuickSearchColumn.VAF_Control_Ref_Value_ID,
                                                              VAF_QuickSearchColumn.IsRange,
                                                              VAF_QuickSearchColumn.ISIDENTIFIER,
                                                              infotable.TableName
                                                            FROM VAF_QuickSearchColumn
                                                            INNER JOIN VAF_COLUMNDIC
                                                            ON (VAF_COLUMNDIC.VAF_COLUMNDIC_ID =VAF_QuickSearchColumn.VAF_COLUMNDIC_ID)
                                                            INNER JOIN VAF_QuickSearchWindow IW
                                                            ON(IW.VAF_QuickSearchWindow_ID =VAF_QuickSearchColumn.VAF_QuickSearchWindow_ID)
                                                            INNER JOIN VAF_QuickSearchColumn_TL
                                                            ON (VAF_QuickSearchColumn_TL.VAF_QuickSearchColumn_ID=VAF_QuickSearchColumn.VAF_QuickSearchColumn_ID
                                                            AND VAF_QuickSearchColumn_TL.VAF_Language     ='" + ctx.GetVAF_Language() + @"')
                                                            INNER JOIN VAF_QuickSearchWindow_TL IWT
                                                            ON (IWT.VAF_QuickSearchWindow_ID          =IW.VAF_QuickSearchWindow_ID
                                                            AND IWT.VAF_Language               ='" + ctx.GetVAF_Language() + @"')
                                                           INNER JOIN VAF_TableView infotable
                                                            ON(infotable.VAF_TableView_ID=IW.VAF_TableView_ID)                        
                                                            WHERE VAF_QuickSearchColumn.IsActive      ='Y'
                                                            AND VAF_QuickSearchColumn.VAF_QuickSearchWindow_ID=" + VAF_QuickSearchWindow_ID + " ORDER BY VAF_QuickSearchColumn.seqno";
                }
                DataSet ds = DBase.DB.ExecuteDataset(sql);
                return GetSchema(ds,ctx);
                //lstContent = GetDisplayColList(ds);
                //infoWinSelect = ds.Tables[0].Rows[0]["FromClause"].ToString();
                //otherSql = ds.Tables[0].Rows[0]["OTHERCLAUSE"].ToString();

            }
            catch
            {
                return null;
            }
        }

        private Info GetSchema(DataSet ds,Ctx ctx)
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
                    
                    if (ds.Tables[0].Rows[i]["VAF_Control_Ref_ID"] != null &&
                        ds.Tables[0].Rows[i]["VAF_Control_Ref_ID"] != DBNull.Value)
                    {
                        schema.VAF_Control_Ref_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_Control_Ref_ID"]);
                        if (schema.VAF_Control_Ref_ID == 17)//if reference is List
                        {
                            schema.RefList = GetRefList(Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_Control_Ref_Value_ID"]),ctx);
                        }
                    }
                    if (ds.Tables[0].Rows[i]["VAF_Control_Ref_Value_ID"] != null &&
                        ds.Tables[0].Rows[i]["VAF_Control_Ref_Value_ID"] != DBNull.Value)
                    {
                        schema.VAF_Control_Ref_Value_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_Control_Ref_Value_ID"]);

                    }
                    else
                    {
                        schema.VAF_Control_Ref_Value_ID = 0;
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
                    //if (schema.VAF_Control_Ref_ID == DisplayType.Search
                    //    || schema.VAF_Control_Ref_ID == DisplayType.Table
                    //    || schema.VAF_Control_Ref_ID == DisplayType.TableDir)
                    //{
                    //    schema.lookup = VLookUpFactory.Get(ctx, 0, 0, schema.VAF_Control_Ref_ID, schema.ColumnName, schema.VAF_Control_Ref_Value_ID, false, null);
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

        private List<InfoRefList> GetRefList(int VAF_Control_Ref_ID,Ctx ctx)
        {
          
                //String sql = "SELECT Value, Name FROM VAF_CtrlRef_List "
                //    + "WHERE VAF_Control_Ref_ID=" + VAF_Control_Ref_ID + " AND IsActive='Y' ORDER BY 1";
                //DataSet ds = null;
               
                List<InfoRefList> list = new List<InfoRefList>();
                try
                {
                    ValueNamePair[] refList = MVAFCtrlRefList.GetList(VAF_Control_Ref_ID, true, ctx);
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




        public InfoData GetData(string sql, string tableName, int pageNo,VAdvantage.Utility.Ctx ctx)
        {

            InfoData _iData = new InfoData();
            try
            {
                sql = sql.Replace('●', '%');
                sql = ParseContext(sql, ctx);
                sql = MVAFRole.GetDefault(ctx).AddAccessSQL(sql, tableName,
                               MVAFRole.SQL_FULLYQUALIFIED, MVAFRole.SQL_RO);

                
                int totalRec = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM ( "+sql+" ) t",null,null));
                int pageSize = 50;
                PageSetting pSetting = new PageSetting();
                pSetting.CurrentPage = pageNo;
                pSetting.TotalPage = (totalRec % pageSize) == 0 ? (totalRec / pageSize) : ((totalRec / pageSize) + 1);
                _iData.pSetting = pSetting;

                DataSet data = DBase.DB.ExecuteDatasetPaging(sql, pageNo, pageSize);
                if (data == null )
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
            catch(Exception ex)
            {
                _iData.Error = ex.Message;
                return _iData;
            }
        }

        private string ParseContext(string res,Ctx ctx)
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
            return Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAF_QuickSearchWindow_ID FROM VAF_QuickSearchWindow WHERE IsActive='Y' AND Value='" + InfoSearchKey + "'", null, null));
        }

    }


    public class InfoSchema
    {

        public string Name
        {
            get;
            set;
        }
        public int VAF_Control_Ref_ID
        {
            get;
            set;
        }
        public int VAF_Control_Ref_Value_ID
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
        
    }
}