using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MVAMPriceListVersionModel
    {
        private string sql = "";

        public int GetVAM_PriceListVersion_ID(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int VAM_PriceList_ID;
            VAM_PriceList_ID = Util.GetValueOfInt(paramValue[0].ToString());
            //sql = "SELECT COUNT(*) FROM VAM_PriceListVersion WHERE IsActive='Y' AND VAM_PriceList_ID = " + VAM_PriceList_ID;
            //if (Util.GetValueOfInt(DB.ExecuteScalar(sql)) > 1)
            //{
            //    sql = "SELECT MAX(VAM_PriceListVersion_ID) FROM VAM_PriceListVersion WHERE IsActive='Y' AND VAM_PriceList_ID = " + VAM_PriceList_ID;
            //}
            //else
            //{
            //    sql = "SELECT (VAM_PriceListVersion_ID) FROM VAM_PriceListVersion WHERE IsActive='Y' AND VAM_PriceList_ID = " + VAM_PriceList_ID;
            //}
            sql = "SELECT VAM_PriceListVersion_ID FROM VAM_PriceListVersion WHERE IsActive = 'Y' AND VAM_PriceList_ID = " + VAM_PriceList_ID + @" AND VALIDFROM <= SYSDATE ORDER BY VALIDFROM DESC";
            return Util.GetValueOfInt(DB.ExecuteScalar(sql));
        }

        // Added by Bharat on 12/May/2017
        public Dictionary<string, int> GetPriceList(Ctx ctx, string fields)
        {
            int VAM_PriceListVersion_ID = Util.GetValueOfInt(fields);
            Dictionary<string, int> retDic = new Dictionary<string, int>();
            MVAMPriceListVersion ver = new MVAMPriceListVersion(ctx, VAM_PriceListVersion_ID, null);
            retDic["VAM_PriceList_ID"] = ver.GetVAM_PriceList_ID();
            MVAMPriceList list = new MVAMPriceList(ctx, ver.GetVAM_PriceList_ID(), null);
            retDic["VAB_Currency_ID"] = list.GetVAB_Currency_ID();
            return retDic;
        }
    }
}