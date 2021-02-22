using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.DBase;

namespace VIS.Models
{
    public class MVAMPriceListModel
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
                int VAM_PriceList_ID;

                //Assign parameter value
                VAM_PriceList_ID = Util.GetValueOfInt(paramValue[0].ToString());
                //End Assign parameter value

                MVAMPriceList prcLst = new MVAMPriceList(ctx, VAM_PriceList_ID, null);
                Dictionary<String, String> retDic = new Dictionary<string, string>();
                // Reset Orig Shipment
                MVABCurrency crncy = new MVABCurrency(ctx, prcLst.GetVAB_Currency_ID(), null);
                //retDic["StdPrecision"] = prcLst.GetPricePrecision().ToString();
                //JID_1744  Precision should be as per currency percision 
                retDic["StdPrecision"] = crncy.GetStdPrecision().ToString();
                return retDic;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get Price List Data
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="fields">Parameters</param>
        /// <returns>List of Data</returns>
        public Dictionary<String, Object> GetPriceListData(Ctx ctx, string fields)
        {            
                int VAM_PriceList_ID;                
                VAM_PriceList_ID = Util.GetValueOfInt(fields);
                Dictionary<String, Object> retDic = null;
                string sql = "SELECT pl.IsTaxIncluded,pl.EnforcePriceLimit,pl.VAB_Currency_ID,c.StdPrecision,"
                + "plv.VAM_PriceListVersion_ID,plv.ValidFrom "
                + "FROM VAM_PriceList pl,VAB_Currency c,VAM_PriceListVersion plv "
                + "WHERE pl.VAB_Currency_ID=c.VAB_Currency_ID"
                + " AND pl.VAM_PriceList_ID=plv.VAM_PriceList_ID"
                + " AND pl.VAM_PriceList_ID=" + VAM_PriceList_ID						//	1
                + "ORDER BY plv.ValidFrom DESC";
                DataSet ds = DB.ExecuteDataset(sql, null, null);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    retDic = new Dictionary<String, Object>();
                    retDic["IsTaxIncluded"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["IsTaxIncluded"]);
                    retDic["EnforcePriceLimit"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["EnforcePriceLimit"]);
                    retDic["StdPrecision"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["StdPrecision"]);
                    retDic["VAB_Currency_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAB_Currency_ID"]);
                    retDic["VAM_PriceListVersion_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAM_PriceListVersion_ID"]);                    
                    retDic["ValidFrom"] = Util.GetValueOfDateTime(ds.Tables[0].Rows[0]["ValidFrom"]);
                }
                return retDic;
        }
    }
}