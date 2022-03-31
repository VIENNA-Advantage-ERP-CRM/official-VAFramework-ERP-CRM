using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class LocatorModel
    {
        public int LocatorId { get; set; }
        public string LocatorValue { get; set; }
        public bool CreateNew { get; set; }
        public string WarehouseInfo { get; set; }
        public int WarehoseId { get; set; }
        public string Warehouse { get; set; }
        public string Xaxsis { get; set; }
        public string Yaxsis { get; set; }
        public string Zaxsis { get; set; }
        public string Value { get; set; }

        /// <summary>
        /// savelocator value into database
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="warehouseId"></param>
        /// <param name="tValue"></param>
        /// <param name="tX"></param>
        /// <param name="tY"></param>
        /// <param name="tZ"></param>
        /// <returns></returns>
        public int LocatorSave(Ctx ctx, string warehouseId, string tValue, string tX, string tY, string tZ)
        {
            var loc = MLocator.Get(ctx, Convert.ToInt32(warehouseId), tValue, tX, tY, tZ);
            return loc.GetM_Locator_ID();
        }

        // Added by Bharat on 09 June 2017
        public List<Dictionary<string, object>> GetWarehouse(int warehouse_id, Ctx ctx)
        {
            List<Dictionary<string, object>> retDic = null;            
            string sql = "SELECT M_Warehouse_ID, Name FROM M_Warehouse";
            if (warehouse_id != 0)
            {
                sql += " WHERE M_Warehouse_ID=" + warehouse_id;
            }            
            string finalSql = MRole.GetDefault(ctx).AddAccessSQL(sql, "M_Warehouse", MRole.SQL_NOTQUALIFIED, MRole.SQL_RO) + " ORDER BY 2";
            DataSet ds = DB.ExecuteDataset(finalSql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retDic = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["M_Warehouse_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Warehouse_ID"]);
                    obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    retDic.Add(obj);
                }
            }
            return retDic;
        }

        // Added by Bharat on 09 June 2017
        public Dictionary<string, object> GetWarehouseData(int locator_id, Ctx ctx)
        {
            Dictionary<string, object> obj = null;
            string sql = "SELECT w.Name, l.x, l.y, l.z, l.Value, w.M_Warehouse_ID, w.Value AS wValue, w.Separator FROM M_Warehouse w" +
                  " INNER JOIN M_Locator l on w.M_Warehouse_ID = l.M_Warehouse_ID and l.M_Locator_ID=" + locator_id;
            
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {                
                    obj = new Dictionary<string, object>();
                    obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["Name"]);
                    obj["x"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["x"]);
                    obj["y"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["y"]);
                    obj["z"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["z"]);
                    obj["Value"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["Value"]);
                    obj["M_Warehouse_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_Warehouse_ID"]);
                    obj["wValue"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["wValue"]);
                    obj["Separator"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["Separator"]);                    
            }
            return obj;
        }
  
    }
}