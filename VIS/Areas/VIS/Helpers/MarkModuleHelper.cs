using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Helpers
{
    public class MarkModuleHelper
    {
        public string SaveExportData(int[] moduleId, int[] _recordID, int _tableID, string _strRecordID, Ctx ctx)
        {
            string result = "OK";
            if (string.IsNullOrEmpty(_strRecordID))
            {
                return result;
            }

            string sql = "delete from VAF_ExportData where record_id in (" + _strRecordID + ") " +
                                   "and vaf_tableview_id=" + _tableID;

            try
            {
                int a = DB.ExecuteQuery(sql, null, null);
            }
            catch
            {

            }
            if (moduleId == null || _recordID == null)
            {
                return result;
            }
            for (int i = 0; i < moduleId.Length; i++)
            {
                for (int j = 0; j < _recordID.Length; j++)
                {
                    X_VAF_ExportData obj = new X_VAF_ExportData(ctx, 0, null);
                    obj.SetRecord_ID(int.Parse(_recordID[j].ToString()));
                    obj.SetVAF_TableView_ID(_tableID);
                    obj.SetVAF_ModuleInfo_ID(moduleId[i]);

                    if (!obj.Save())
                    {
                        if (result.Equals("OK"))
                        {
                            result = "";
                        }
                        result += ("RecordID->" + _recordID[j] + " for Module Name->" + moduleId[i] + ", is not marked");
                    }

                }
            }
            return result;

        }

        // Added by Bharat on 09 June 2017
        public List<Dictionary<string, object>> LoadModules(Ctx ctx)
        {
            List<Dictionary<string, object>> retDic = null;
            string sql = "SELECT VAF_ModuleInfo_ID, Name FROM VAF_ModuleInfo WHERE IsActive='Y' ORDER BY Upper(Name)";
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retDic = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["VAF_ModuleInfo_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_ModuleInfo_ID"]);
                    obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    retDic.Add(obj);
                }
            }
            return retDic;
        }

        // Added by Bharat on 09 June 2017
        public List<int> GetExportData(int _recordID, int _tableID, Ctx ctx)
        {
            List<int> recID = null;
            string sql = "SELECT VAF_ModuleInfo_ID FROM VAF_ExportData e WHERE e.Record_ID=" + _recordID + " AND e.VAF_TableView_ID = " + _tableID;
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                recID = new List<int>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    int id = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_ModuleInfo_ID"]);
                    recID.Add(id);
                }
            }
            return recID;
        }
    }
}