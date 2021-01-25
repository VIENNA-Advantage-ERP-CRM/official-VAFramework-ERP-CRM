using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MPriceListVersionModel
    {
        private string sql = "";

        public int GetM_PriceList_Version_ID(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int M_PriceList_ID;
            M_PriceList_ID = Util.GetValueOfInt(paramValue[0].ToString());
            //sql = "SELECT COUNT(*) FROM M_PriceList_Version WHERE IsActive='Y' AND M_PriceList_ID = " + M_PriceList_ID;
            //if (Util.GetValueOfInt(DB.ExecuteScalar(sql)) > 1)
            //{
            //    sql = "SELECT MAX(M_PriceList_Version_ID) FROM M_PriceList_Version WHERE IsActive='Y' AND M_PriceList_ID = " + M_PriceList_ID;
            //}
            //else
            //{
            //    sql = "SELECT (M_PriceList_Version_ID) FROM M_PriceList_Version WHERE IsActive='Y' AND M_PriceList_ID = " + M_PriceList_ID;
            //}
            sql = "SELECT M_PriceList_Version_ID FROM M_PriceList_Version WHERE IsActive = 'Y' AND M_PriceList_ID = " + M_PriceList_ID + @" AND VALIDFROM <= SYSDATE ORDER BY VALIDFROM DESC";
            return Util.GetValueOfInt(DB.ExecuteScalar(sql));
        }

        // Added by Bharat on 12/May/2017
        public Dictionary<string, int> GetPriceList(Ctx ctx, string fields)
        {
            int M_PriceListVersion_ID = Util.GetValueOfInt(fields);
            Dictionary<string, int> retDic = new Dictionary<string, int>();
            MPriceListVersion ver = new MPriceListVersion(ctx, M_PriceListVersion_ID, null);
            retDic["M_PriceList_ID"] = ver.GetM_PriceList_ID();
            MPriceList list = new MPriceList(ctx, ver.GetM_PriceList_ID(), null);
            retDic["VAB_Currency_ID"] = list.GetVAB_Currency_ID();
            return retDic;
        }
    }
}