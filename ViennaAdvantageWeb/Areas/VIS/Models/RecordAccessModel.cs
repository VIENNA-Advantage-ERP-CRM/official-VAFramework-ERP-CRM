using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Areas.VIS.Models
{
    public class RecordAccessModel
    {
        public bool SaveAccess(Ctx ctx,int AD_Role_ID, int AD_Table_ID, int Record_ID, bool isActive, bool isExclude, bool isReadOnly, bool isDependentEntities,bool isUpdate)
        {
            if (isUpdate)
            {
                string sql = "UPDATE AD_Record_Access SET IsActive='" + (isActive ? 'Y' : 'N') + @"',
                              IsExclude='" + (isExclude ? 'Y' : 'N') + @"',
                              IsReadOnly='" + (isReadOnly ? 'Y' : 'N') + @"',
                              IsDependentEntities='" + (isDependentEntities ? 'Y' : 'N') + @"'
                              WHERE AD_Role_ID=" + AD_Role_ID + @"
                              AND Record_ID=" + Record_ID + @"
                              AND AD_Table_ID=" + AD_Table_ID ;
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
            bool success = recData.Save();
            return success;
        }
        public bool DeleteRecordAccess( int AD_Role_ID, int AD_Table_ID, int Record_ID, bool isActive, bool isExclude, bool isReadOnly, bool isDependentEntities)
        {
            string sql = "DELETE FROM  AD_Record_Access WHERE AD_Role_ID=" + AD_Role_ID + @"
                        AND Record_ID=" + Record_ID + @"
                        AND AD_Table_ID=" + AD_Table_ID + @"
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
    }
}