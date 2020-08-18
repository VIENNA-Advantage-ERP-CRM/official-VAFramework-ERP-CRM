using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VAdvantage.Classes
{
    public class TableSQL
    {
        public string GetTableQuery(Ctx ctx, int AD_Tab_ID, int AD_Table_ID)
        {
            StringBuilder sqlTbl = new StringBuilder("");

            if (AD_Table_ID <= 0 && AD_Tab_ID > 0)
                AD_Table_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_Table_ID FROM AD_Tab WHERE AD_Tab_ID = " + AD_Tab_ID));

            if (AD_Table_ID <= 0)
                return sqlTbl.ToString();

            string sql = "SELECT * FROM AD_Column WHERE AD_Table_ID = " + AD_Table_ID;

            POInfo inf = POInfo.GetPOInfo(ctx, AD_Table_ID);


            return sqlTbl.ToString();
        }

        public string GetFieldV(Ctx ctx)
        {
            //	IsActive is part of View
            String sql = "SELECT * FROM AD_Field_v WHERE AD_Tab_ID=@tabID";
            if (!Env.IsBaseLanguage(ctx, "AD_Tab"))
                sql = "SELECT * FROM AD_Field_vt WHERE AD_Tab_ID=@tabID"
                    + " AND AD_Language='" + Env.GetAD_Language(ctx) + "'";
            //if (AD_UserDef_Win_ID != 0)
            //    sql += " AND AD_UserDef_Win_ID=" + AD_UserDef_Win_ID;
            sql += " ORDER BY IsDisplayed DESC, SeqNo";
            return sql;
        }
    }
}
