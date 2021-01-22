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
        public bool SaveAccess(Ctx ctx,int VAF_Role_ID, int VAF_TableView_ID, int Record_ID, bool isActive, bool isExclude, bool isReadOnly, bool isDependentEntities,bool isUpdate)
        {
            if (isUpdate)
            {
                string sql = "UPDATE VAF_Record_Rights SET IsActive='" + (isActive ? 'Y' : 'N') + @"',
                              IsExclude='" + (isExclude ? 'Y' : 'N') + @"',
                              IsReadOnly='" + (isReadOnly ? 'Y' : 'N') + @"',
                              IsDependentEntities='" + (isDependentEntities ? 'Y' : 'N') + @"'
                              WHERE VAF_Role_ID=" + VAF_Role_ID + @"
                              AND Record_ID=" + Record_ID + @"
                              AND VAF_TableView_ID=" + VAF_TableView_ID ;
                int res = VAdvantage.DataBase.DB.ExecuteQuery(sql);
                if (res > -1)
                {
                    return true;
                }
                return false;
            }

            MRecordAccess recData = new MRecordAccess(ctx, VAF_Role_ID, VAF_TableView_ID, Record_ID, null);
            recData.SetIsActive(isActive);
            recData.SetIsExclude(isExclude);
            recData.SetIsReadOnly(isReadOnly);
            recData.SetIsDependentEntities(isDependentEntities);
            bool success = recData.Save();
            return success;
        }
        public bool DeleteRecordAccess( int VAF_Role_ID, int VAF_TableView_ID, int Record_ID, bool isActive, bool isExclude, bool isReadOnly, bool isDependentEntities)
        {
            string sql = "DELETE FROM  VAF_Record_Rights WHERE VAF_Role_ID=" + VAF_Role_ID + @"
                        AND Record_ID=" + Record_ID + @"
                        AND VAF_TableView_ID=" + VAF_TableView_ID + @"
                        AND IsActive='" + (isActive ? 'Y' : 'N') + @"'
                        AND IsExclude='" + (isExclude ? 'Y' : 'N') + @"'
                        AND IsReadOnly='" + (isReadOnly ? 'Y' : 'N') + @"'
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
            string sql = MRole.GetDefault(ctx).AddAccessSQL("SELECT VAF_Role_ID, Name FROM VAF_Role ORDER BY 2", "VAF_Role", MRole.SQL_NOTQUALIFIED, MRole.SQL_RW);
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retDic = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["VAF_Role_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_Role_ID"]);
                    obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);                    
                    retDic.Add(obj);
                }
            }
            return retDic;
        }

        // Added by Bharat on 06 June 2017
        public List<Dictionary<string, object>> GetRecordAccess(int _VAF_TableView_ID, int _Record_ID, Ctx ctx)
        {
            List<Dictionary<string, object>> retDic = null;
            string sql = @"SELECT VAF_ROLE_ID,ISACTIVE,ISDEPENDENTENTITIES,ISEXCLUDE,ISREADONLY FROM VAF_Record_Rights WHERE VAF_TableView_ID=" + _VAF_TableView_ID 
                + " AND Record_ID=" + _Record_ID + " AND VAF_Client_ID=" + ctx.GetVAF_Client_ID();
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retDic = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["VAF_ROLE_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_ROLE_ID"]);
                    obj["ISACTIVE"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["ISACTIVE"]);
                    obj["ISDEPENDENTENTITIES"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["ISDEPENDENTENTITIES"]);
                    obj["ISEXCLUDE"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["ISEXCLUDE"]);
                    obj["ISREADONLY"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["ISREADONLY"]);                    
                    retDic.Add(obj);
                }
            }
            return retDic;
        }
    }
}