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
        public string GetTableQuery(Ctx ctx, int VAF_Tab_ID, int VAF_TableView_ID)
        {
            StringBuilder sqlTbl = new StringBuilder("");

            if (VAF_TableView_ID <= 0 && VAF_Tab_ID > 0)
                VAF_TableView_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAF_TableView_ID FROM VAF_Tab WHERE VAF_Tab_ID = " + VAF_Tab_ID));

            if (VAF_TableView_ID <= 0)
                return sqlTbl.ToString();

            string sql = "SELECT * FROM VAF_Column WHERE VAF_TableView_ID = " + VAF_TableView_ID;

            POInfo inf = POInfo.GetPOInfo(ctx, VAF_TableView_ID);


            return sqlTbl.ToString();
        }

        public string GetFieldV(Ctx ctx)
        {
            //	IsActive is part of View
            String sql = "SELECT * FROM VAF_Field_v WHERE VAF_Tab_ID=@tabID";
            if (!Env.IsBaseLanguage(ctx, "VAF_Tab"))
                sql = "SELECT * FROM VAF_Field_vt WHERE VAF_Tab_ID=@tabID"
                    + " AND VAF_Language='" + Env.GetVAF_Language(ctx) + "'";
            //if (AD_UserDef_Win_ID != 0)
            //    sql += " AND AD_UserDef_Win_ID=" + AD_UserDef_Win_ID;
            sql += " ORDER BY IsDisplayed DESC, SeqNo";
            return sql;
        }
    }
}
