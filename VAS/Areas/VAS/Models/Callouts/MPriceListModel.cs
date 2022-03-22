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
    public class MPriceListModel
    {
        /// <summary>
        /// GetPriceList
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<String, String> GetPriceList(Ctx ctx, string fields)
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
                MCurrency crncy = new MCurrency(ctx, prcLst.GetC_Currency_ID(), null);
                retDic["PriceListPrecision"] = prcLst.GetPricePrecision().ToString();
                //JID_1744  Precision should be as per currency percision 
                retDic["StdPrecision"] = crncy.GetStdPrecision().ToString();
                retDic["EnforcePriceLimit"] = prcLst.IsEnforcePriceLimit() ? "Y" : "N";
                retDic["IsTaxIncluded"] = prcLst.IsTaxIncluded() ? "Y" : "N";
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
            int M_PriceList_ID;
            M_PriceList_ID = Util.GetValueOfInt(fields);
            Dictionary<String, Object> retDic = null;
            string sql = "SELECT pl.IsTaxIncluded,pl.EnforcePriceLimit,pl.C_Currency_ID,c.StdPrecision,"
            + "plv.M_PriceList_Version_ID,plv.ValidFrom "
            + "FROM M_PriceList pl,C_Currency c,M_PriceList_Version plv "
            + "WHERE pl.C_Currency_ID=c.C_Currency_ID"
            + " AND pl.M_PriceList_ID=plv.M_PriceList_ID"
            + " AND pl.M_PriceList_ID=" + M_PriceList_ID                        //	1
            + "ORDER BY plv.ValidFrom DESC";
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retDic = new Dictionary<String, Object>();
                retDic["IsTaxIncluded"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["IsTaxIncluded"]);
                retDic["EnforcePriceLimit"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["EnforcePriceLimit"]);
                retDic["StdPrecision"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["StdPrecision"]);
                retDic["C_Currency_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_Currency_ID"]);
                retDic["M_PriceList_Version_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_PriceList_Version_ID"]);
                retDic["ValidFrom"] = Util.GetValueOfDateTime(ds.Tables[0].Rows[0]["ValidFrom"]);
            }
            return retDic;
        }

        /// <summary>
        /// Get Price List Data When select or change the PriceList on 
        /// Provisional Invoice window
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="fields">Parameters</param>
        /// <returns>List of Data</returns>
        public Dictionary<String, Object> GetPriceListDataForProvisionalInvoice(Ctx ctx, string fields)
        {
            int M_PriceList_ID = Util.GetValueOfInt(fields);
            Dictionary<String, Object> retDic = null;

            string sql = "SELECT pl.IsTaxIncluded,pl.EnforcePriceLimit,pl.C_Currency_ID,"
            + "plv.M_PriceList_Version_ID "
            + "FROM M_PriceList pl,C_Currency c,M_PriceList_Version plv "
            + "WHERE pl.C_Currency_ID=c.C_Currency_ID"
            + " AND pl.M_PriceList_ID=plv.M_PriceList_ID"
            + " AND pl.M_PriceList_ID=" + M_PriceList_ID
            + "ORDER BY plv.ValidFrom DESC";
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retDic = new Dictionary<String, Object>();
                retDic["IsTaxIncluded"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["IsTaxIncluded"]);
                retDic["EnforcePriceLimit"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["EnforcePriceLimit"]);
                retDic["C_Currency_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_Currency_ID"]);
                retDic["M_PriceList_Version_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_PriceList_Version_ID"]);
            }
            return retDic;
        }
    }
}