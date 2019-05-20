using System;
using System.Collections.Generic;
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
           
            string sql = "delete from ad_exportdata where record_id in (" + _strRecordID + ") " +
                                   "and ad_table_id=" + _tableID;

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
                    X_AD_ExportData obj = new X_AD_ExportData(ctx, 0, null);
                    obj.SetRecord_ID(int.Parse(_recordID[j].ToString()));
                    obj.SetAD_Table_ID(_tableID);
                    obj.SetAD_ModuleInfo_ID(moduleId[i]);

                    if (!obj.Save())
                    {
                        if (result.Equals("OK"))
                        {
                            result = "";
                        }
                        result+=("RecordID->" + _recordID[j] + " for Module Name->" + moduleId[i] + ", is not marked");
                    }

                }
            }
            return result;

        }
    }
}