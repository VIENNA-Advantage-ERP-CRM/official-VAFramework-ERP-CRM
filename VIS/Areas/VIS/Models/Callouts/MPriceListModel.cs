using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MPriceListModel
    {
        /// <summary>
        /// GetPriceList
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<String, String> GetPriceList(Ctx ctx,string fields)
        {
            if (fields != null)
            {
                string[] paramValue = fields.ToString().Split(',');
                int M_PriceList_ID;

                //Assign parameter value
                M_PriceList_ID = Util.GetValueOfInt(paramValue[0].ToString());
                //End Assign parameter value

                MPriceList prcLst = new MPriceList(ctx, M_PriceList_ID, null);
                Dictionary<String, String> retDic = new Dictionary<string, string>();
                // Reset Orig Shipment
                retDic["StdPrecision"] = prcLst.GetPricePrecision().ToString();
                return retDic;
            }
            else
            {
                return null;
            }
        }

    }
}