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
    public class InfoGeneralModel
    {

        public List<InfoGenral> GetSchema(string tableName, string VAF_Language, bool isBaseLangage)
        {
            try
            {
                

                //Change by mohit-to handle translation in general info.
                //Added 2 new parametere- string VAF_Language, bool isBaseLangage.
                //Asked by mukesh sir- 09/03/2018
                string sql = string.Empty;
                if (isBaseLangage)
                {
                    sql = @"SELECT c.ColumnName,
                            c.name,
                            c.IsIdentifier,
                            t.VAF_TableView_ID,
                            t.TableName,
                            C.IsTranslated 
                         FROM VAF_TableView t
                            INNER JOIN VAF_Column c ON (t.VAF_TableView_ID=c.VAF_TableView_ID)
                            WHERE c.VAF_Control_Ref_ID=10
                            AND t.TableName='" + tableName + @"'
                           AND EXISTS (SELECT * FROM VAF_Field f 
                     WHERE f.VAF_Column_ID=c.VAF_Column_ID
                     AND f.IsDisplayed='Y' AND f.IsEncrypted='N' AND f.ObscureType IS NULL)                    
                 ORDER BY c.IsIdentifier DESC, c.SeqNo";
                }
                else
                {
                    sql = @"SELECT c.ColumnName,  
                          trl.name,
                          c.IsIdentifier,
                          t.VAF_TableView_ID,
                          t.TableName,
                          C.IsTranslated
                        FROM VAF_TableView t
                        INNER JOIN VAF_Column c
                        ON (t.VAF_TableView_ID      =c.VAF_TableView_ID)
                        INNER JOIN VAF_Column_TL trl
                        ON (c.VAF_Column_ID=trl.VAF_Column_ID)
                        WHERE c.VAF_Control_Ref_ID=10
                        AND t.TableName        ='" + tableName + @"'
                        AND trl.VAF_Language='" + VAF_Language + @"'
                        AND EXISTS(SELECT * FROM VAF_Field f
                          WHERE f.VAF_Column_ID=c.VAF_Column_ID
                          AND f.IsDisplayed   ='Y' AND f.IsEncrypted   ='N' AND f.ObscureType  IS NULL)
                        ORDER BY c.IsIdentifier DESC,c.SeqNo";
                }

                DataSet ds = DB.ExecuteDataset(sql);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return null;
                }
                List<InfoGenral> lstInfoGen = new List<InfoGenral>();
                InfoGenral item = null;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (i == 3)
                    {
                        break;
                    }
                    item = new InfoGenral();
                    item.VAF_TableView_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_TableView_ID"]);
                    item.ColumnName = ds.Tables[0].Rows[i]["ColumnName"].ToString();
                    item.Name = ds.Tables[0].Rows[i]["Name"].ToString();
                    item.IsIdentifier = ds.Tables[0].Rows[i]["IsIdentifier"].ToString() == "Y" ? true : false;
                    // Change done by mohit asked by mukesh sir to show the data on info window from translated tab if logged in with langauge other than base language- 22/03/2018
                    item.IsTranslated = ds.Tables[0].Rows[i]["IsTranslated"].ToString() == "Y" ? true : false;
                    lstInfoGen.Add(item);

                }
                ds = null;
                return lstInfoGen;
            }
            catch
            {
                return null;
            }
        }

        public List<InfoColumn> GetDisplayCol(int VAF_TableView_ID, string VAF_Language, bool IsBaseLangage,string _tableName)
        {
            try
            {
                bool _trlTableExist = false;
                if (!IsBaseLangage)
                {
                    
                    VAdvantage.DataBase.VConnection con = new VAdvantage.DataBase.VConnection();
                    string owner = con.Db_uid;
                    if (Util.GetValueOfInt(DB.ExecuteQuery("SELECT count(*) FROM ALL_OBJECTS WHERE OBJECT_TYPE = 'TABLE' AND OWNER =upper( '" + owner + "') AND OBJECT_NAME =upper( '" + _tableName + "_TRL')", null, null)) > 0)
                    {
                        _trlTableExist = true;
                    }
                }

                //Change by mohit-to handle translation in general info.
                //Added 2 new parametere- string VAF_Language, bool IsBaseLangage.
                //Asked by mukesh sir - 09/03/2018

                string sql = string.Empty;
                if (IsBaseLangage)
                {
                    sql = @"SELECT c.ColumnName,c.Name,
                              c.VAF_Control_Ref_ID,
                              c.IsKey,
                              f.IsDisplayed,
                              c.VAF_Control_Ref_Value_ID,
                              c.ColumnSQL,
                              C.IsTranslated
                            FROM VAF_Column c
                            INNER JOIN VAF_TableView t
                            ON (c.VAF_TableView_ID=t.VAF_TableView_ID)                            
                            INNER JOIN VAF_Tab tab
                            ON (t.VAF_Screen_ID=tab.VAF_Screen_ID)
                            INNER JOIN VAF_Field f
                            ON (tab.VAF_Tab_ID  =f.VAF_Tab_ID
                            AND f.VAF_Column_ID =c.VAF_Column_ID)
                            WHERE t.VAF_TableView_ID=" + VAF_TableView_ID + @"
                            AND (c.IsKey       ='Y'
                            OR (f.IsEncrypted  ='N'
                            AND f.ObscureType IS NULL))                            
                            ORDER BY c.IsKey DESC,
                              f.SeqNo";
                }
                else
                {
                    sql = @"SELECT c.ColumnName,trl.Name,
                              c.VAF_Control_Ref_ID,
                              c.IsKey,
                              f.IsDisplayed,
                              c.VAF_Control_Ref_Value_ID,
                              c.ColumnSQL,
                              C.IsTranslated
                            FROM VAF_Column c
                            INNER JOIN VAF_TableView t
                            ON (c.VAF_TableView_ID=t.VAF_TableView_ID)
                            INNER JOIN VAF_Column_TL trl
                            ON (c.vaf_column_ID=trl.VAF_Column_ID)
                            INNER JOIN VAF_Tab tab
                            ON (t.VAF_Screen_ID=tab.VAF_Screen_ID)
                            INNER JOIN VAF_Field f
                            ON (tab.VAF_Tab_ID  =f.VAF_Tab_ID
                            AND f.VAF_Column_ID =c.VAF_Column_ID)
                            WHERE t.VAF_TableView_ID=" + VAF_TableView_ID + @"
                            AND trl.VAF_Language='" + VAF_Language + @"'
                            AND (c.IsKey       ='Y'
                            OR (f.IsEncrypted  ='N'
                            AND f.ObscureType IS NULL))                            
                            ORDER BY c.IsKey DESC,
                              f.SeqNo";
                }
                DataSet ds = DB.ExecuteDataset(sql);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return null;
                }
                List<InfoColumn> lstCols = new List<InfoColumn>();
                InfoColumn item = null;
                int displayType = 0;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    item = new InfoColumn();
                    item.IsKey = ds.Tables[0].Rows[i]["IsKey"].ToString() == "Y" ? true : false;
                    item.IsDisplayed = ds.Tables[0].Rows[i]["IsDisplayed"].ToString() == "Y" ? true : false;


                    displayType = Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_Control_Ref_ID"]);

                    if (item.IsKey)
                    {
                    }
                    else if (!item.IsDisplayed)
                    {
                        continue;
                    }
                    else if (!(displayType == DisplayType.YesNo
                        || displayType == DisplayType.Amount
                        || displayType == DisplayType.Number
                        || displayType == DisplayType.Quantity
                        || displayType == DisplayType.Integer
                        || displayType == DisplayType.String
                        || displayType == DisplayType.Text
                        || displayType == DisplayType.Memo
                        || DisplayType.IsDate(displayType)
                        || displayType == DisplayType.List))
                    {
                        continue;
                    }
                    else if (!(ds.Tables[0].Rows[i]["ColumnSQL"] == null || ds.Tables[0].Rows[i]["ColumnSQL"] == DBNull.Value))
                    {
                        continue;
                    }

                    item.ColumnName = ds.Tables[0].Rows[i]["ColumnName"].ToString();
                    item.Name = ds.Tables[0].Rows[i]["Name"].ToString();
                    item.VAF_Control_Ref_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_Control_Ref_ID"]);
                    item.IsKey = ds.Tables[0].Rows[i]["IsKey"].ToString() == "Y" ? true : false;
                    item.IsDisplayed = ds.Tables[0].Rows[i]["IsDisplayed"].ToString() == "Y" ? true : false;
                    if (!(ds.Tables[0].Rows[i]["VAF_Control_Ref_Value_ID"] == null || ds.Tables[0].Rows[i]["VAF_Control_Ref_Value_ID"] == DBNull.Value))
                    {
                        item.VAF_Control_Ref_Value_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_Control_Ref_Value_ID"]);
                        item.RefList = GetRefList(item.VAF_Control_Ref_Value_ID);
                    }
                    item.ColumnSQL = ds.Tables[0].Rows[i]["ColumnSQL"].ToString();
                    // Change done by mohit asked by mukesh sir to show the data on info window from translated tab if logged in with langauge other than base language- 22/03/2018
                    item.IsTranslated = ds.Tables[0].Rows[i]["IsTranslated"].ToString() == "Y" ? true : false;
                    item.trlTableExist = _trlTableExist;

                    lstCols.Add(item);

                }

                return lstCols;
            }
            catch
            {
                return null;
            }
        }
        private List<InfoRefList> GetRefList(int VAF_Control_Ref_ID)
        {
            try
            {
                String sql = "SELECT Value, Name FROM VAF_CtrlRef_List "
                    + "WHERE VAF_Control_Ref_ID=" + VAF_Control_Ref_ID + " AND IsActive='Y' ORDER BY 1";
                DataSet ds = null;
                List<InfoRefList> list = new List<InfoRefList>();
                try
                {
                    ds = DB.ExecuteDataset(sql, null, null);
                    InfoRefList itm = new InfoRefList();
                    itm.Key = "";
                    itm.Value = "";
                    list.Add(itm);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        itm = new InfoRefList();
                        itm.Key = ds.Tables[0].Rows[i]["Value"].ToString();
                        itm.Value = ds.Tables[0].Rows[i]["Name"].ToString();

                        list.Add(itm);
                    }
                    ds = null;
                }
                catch (Exception)
                {

                }

                return list;
            }
            catch
            {
                return null;
            }
        }


        public InfoData GetData(string sql, string tableName, int pageNo, VAdvantage.Utility.Ctx ctx)
        {
            InfoData _iData = new InfoData();
            try
            {
                sql = sql.Replace('●', '%');
                sql = MRole.GetDefault(ctx).AddAccessSQL(sql, tableName,
                                MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);


                int totalRec = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM ( " + sql + " ) t", null, null));
                int pageSize = 50;
                PageSetting pSetting = new PageSetting();
                pSetting.CurrentPage = pageNo;
                pSetting.TotalPage = (totalRec % pageSize) == 0 ? (totalRec / pageSize) : ((totalRec / pageSize) + 1);
                _iData.pSetting = pSetting;

                //DataSet data = DBase.DB.ExecuteDataset(sql);
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

    }
    public class InfoGenral
    {
        public int VAF_TableView_ID
        {
            get;
            set;
        }

        public string ColumnName
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public bool IsIdentifier
        {
            get;
            set;
        }
        public bool IsTranslated
        {
            get;
            set;
        }

    }

    public class InfoColumn
    {
        public InfoColumn()
        {

        }

        public InfoColumn(String colHeader, String columnName, bool readOnly, String colSQL, int colClass)
        {
            Name = colHeader;
            ColumnName = columnName;
            ColumnSQL = colSQL;
            VAF_Control_Ref_ID = colClass;
        }

        public string ColumnName
        {
            get;
            set;
        }
        public int VAF_Control_Ref_ID
        {
            get;
            set;
        }
        public bool IsKey
        {
            get;
            set;
        }
        public bool IsDisplayed
        {
            get;
            set;
        }
        public int VAF_Control_Ref_Value_ID
        {
            get;
            set;
        }
        public string ColumnSQL
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public int _sequence
        {
            get;
            set;
        }
        public List<InfoRefList> RefList
        {
            get;
            set;
        }
        public InfoColumn Seq(int sequence)
        {
            _sequence = sequence;
            return this;
        }
        public bool IsTranslated
        {
            get;
            set;
        }
        public bool trlTableExist
        {
            get;
            set;
        }
    }

}