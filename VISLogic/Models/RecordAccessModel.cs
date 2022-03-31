using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Areas.VIS.Models
{
    public class RecordAccessModel
    {
        public bool SaveAccess(Ctx ctx, int AD_Role_ID, int AD_Table_ID, int Record_ID, bool isActive,
            bool isExclude, bool isReadOnly, bool isIncludeNull, bool isDependentEntities, bool isUpdate)
        {
            if (isUpdate)
            {
                string sql = "UPDATE AD_Record_Access SET IsActive='" + (isActive ? 'Y' : 'N') + @"',
                              IsExclude='" + (isExclude ? 'Y' : 'N') + @"',
                              IsReadOnly='" + (isReadOnly ? 'Y' : 'N') + @"',
                              IsIncludeNull='" + (isIncludeNull ? 'Y' : 'N') + @"',
                              IsDependentEntities='" + (isDependentEntities ? 'Y' : 'N') + @"'
                              WHERE AD_Role_ID=" + AD_Role_ID + @"
                              AND Record_ID=" + Record_ID + @"
                              AND AD_Table_ID=" + AD_Table_ID;
                int res = VAdvantage.DataBase.DB.ExecuteQuery(sql);
                if (res > -1)
                {
                    return true;
                }
                return false;
            }

            MRecordAccess recData = new MRecordAccess(ctx, AD_Role_ID, AD_Table_ID, Record_ID, null);
            recData.SetIsActive(isActive);
            recData.SetIsExclude(isExclude);
            recData.SetIsReadOnly(isReadOnly);
            recData.SetIsDependentEntities(isDependentEntities);
            recData.SetIsIncludeNull(isIncludeNull);
            bool success = recData.Save();
            return success;
        }
        public bool DeleteRecordAccess(int AD_Role_ID, int AD_Table_ID, int Record_ID, bool isActive,
            bool isExclude, bool isReadOnly, bool isIncludeNull, bool isDependentEntities)
        {
            string sql = "DELETE FROM  AD_Record_Access WHERE AD_Role_ID=" + AD_Role_ID + @"
                        AND Record_ID=" + Record_ID + @"
                        AND AD_Table_ID=" + AD_Table_ID + @"
                        AND IsActive='" + (isActive ? 'Y' : 'N') + @"'
                        AND IsExclude='" + (isExclude ? 'Y' : 'N') + @"'
                        AND IsReadOnly='" + (isReadOnly ? 'Y' : 'N') + @"'
                        AND IsIncludeNull='" + (isIncludeNull ? 'Y' : 'N') + @"'
                        AND IsDependentEntities='" + (isDependentEntities ? 'Y' : 'N') + "'";
            int res = VAdvantage.DataBase.DB.ExecuteQuery(sql);
            if (res > -1)
            {
                return true;
            }
            return false;
        }

        // Added by Bharat on 06 June 2017
        public List<Dictionary<string, object>> GetRoles(Ctx ctx)
        {
            List<Dictionary<string, object>> retDic = null;
            string sql = MRole.GetDefault(ctx).AddAccessSQL("SELECT AD_Role_ID, Name FROM AD_Role ORDER BY 2", "AD_Role", MRole.SQL_NOTQUALIFIED, MRole.SQL_RW);
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retDic = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["AD_Role_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Role_ID"]);
                    obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    retDic.Add(obj);
                }
            }
            return retDic;
        }

        // Added by Bharat on 06 June 2017
        public List<Dictionary<string, object>> GetRecordAccess(int _AD_Table_ID, int _Record_ID, Ctx ctx)
        {
            List<Dictionary<string, object>> retDic = null;
            string sql = @"SELECT AD_ROLE_ID,ISACTIVE,ISDEPENDENTENTITIES,ISEXCLUDE,ISREADONLY,ISINCLUDENULL FROM AD_Record_Access WHERE AD_Table_ID=" + _AD_Table_ID
                + " AND Record_ID=" + _Record_ID + " AND AD_Client_ID=" + ctx.GetAD_Client_ID();
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retDic = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["AD_ROLE_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_ROLE_ID"]);
                    obj["ISACTIVE"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["ISACTIVE"]);
                    obj["ISDEPENDENTENTITIES"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["ISDEPENDENTENTITIES"]);
                    obj["ISEXCLUDE"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["ISEXCLUDE"]);
                    obj["ISREADONLY"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["ISREADONLY"]);
                    obj["ISINCLUDENULL"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["ISINCLUDENULL"]);
                    retDic.Add(obj);
                }
            }
            return retDic;
        }
    }
}