using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.Classes;
using VAdvantage.Controller;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class ASearchModel
    {
        // Added by Bharat on 05 June 2017
        public List<Dictionary<string, object>> GetData(string valueColumnName, int AD_Tab_ID, int AD_Table_ID, Ctx ctx)
        {
            List<Dictionary<string, object>> retDic = null;
            string sql = "SELECT Name," + valueColumnName + ", AD_UserQuery_ID FROM AD_UserQuery WHERE"
                + " AD_Client_ID=" + ctx.GetAD_Client_ID() + " AND IsActive='Y'"
                + " AND (AD_Tab_ID=" + AD_Tab_ID + " OR AD_Table_ID=" + AD_Table_ID + ")"
                + " ORDER BY Upper(Name), AD_UserQuery_ID";

            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retDic = new List<Dictionary<string,object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    obj[valueColumnName] = Util.GetValueOfString(ds.Tables[0].Rows[i][valueColumnName]);
                    obj["AD_UserQuery_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_UserQuery_ID"]);
                    retDic.Add(obj);
                }
            }
            return retDic;
        }

        // Added by Bharat on 05 June 2017
        public List<Dictionary<string, object>> GetQueryLines(int AD_UserQuery_ID, Ctx ctx)
        {
            List<Dictionary<string, object>> retDic = null;
            string sql = "SELECT KEYNAME, KEYVALUE, OPERATOR AS OPERATORNAME,VALUE1NAME," +
                "VALUE1VALUE, VALUE2NAME, VALUE2VALUE, AD_USERQUERYLINE_ID, Isfullday FROM AD_UserQueryLine WHERE AD_UserQuery_ID=" +
                AD_UserQuery_ID + " ORDER BY SeqNo";

            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retDic = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["KEYNAME"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["KEYNAME"]);
                    obj["KEYVALUE"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["KEYVALUE"]);
                    obj["OPERATORNAME"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["OPERATORNAME"]);
                    obj["VALUE1NAME"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["VALUE1NAME"]);
                    obj["VALUE1VALUE"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["VALUE1VALUE"]);
                    obj["VALUE2NAME"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["VALUE2NAME"]);
                    obj["VALUE2VALUE"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["VALUE2VALUE"]);
                    obj["AD_USERQUERYLINE_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_USERQUERYLINE_ID"]);
                    obj["FULLDAY"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["ISFULLDAY"]);
                    retDic.Add(obj);
                }
            }
            return retDic;
        }

        // Added by Bharat on 05 June 2017
        public int GetQueryDefault(int AD_UserQuery_ID, Ctx ctx)
        {
            string sql = "SELECT Count(*) FROM AD_DefaultUserQuery WHERE AD_UserQuery_ID=" + AD_UserQuery_ID + " AND AD_User_ID!=" + ctx.GetAD_User_ID();
            int count = Util.GetValueOfInt(DB.ExecuteScalar(sql));
            return count;
        }

        // Added by Bharat on 05 June 2017
        public int GetNoOfRecrds(string sql, Ctx ctx)
        {            
            int count = Util.GetValueOfInt(DB.ExecuteScalar(sql));
            return count;
        }
    }
}