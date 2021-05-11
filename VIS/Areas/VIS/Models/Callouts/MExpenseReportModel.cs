using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.DBase;


namespace VIS.Models
{
    public class MExpenseReportModel
    {
        /// <summary>
        /// Get Price of product
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="param">List of parameters</param>
        /// <returns>Price</returns>
        public Dictionary<String, Object> GetPrices(Ctx ctx, string param)
        {
            string[] paramValue = param.Split(',');

            Dictionary<String, Object> retDic = new Dictionary<String, Object>();

            //Assign parameter value
            int _m_Product_Id = Util.GetValueOfInt(paramValue[0].ToString());
            int _s_TimeExpense_ID = Util.GetValueOfInt(paramValue[1].ToString());
            int _c_Uom_Id = Util.GetValueOfInt(paramValue[2].ToString());
            StringBuilder sql = new StringBuilder();
            decimal PriceList = 0;
            int _m_PriceList_ID = 0;
            int _priceListVersion_Id = 0;
            _m_PriceList_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_PriceList_ID FROM S_TimeExpense  WHERE S_TimeExpense_ID =" + _s_TimeExpense_ID, null, null));
            _priceListVersion_Id = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_PriceList_Version_ID,name FROM M_PriceList_Version WHERE IsActive='Y' AND M_PriceList_ID=" + _m_PriceList_ID, null, null));
            sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                        + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                        + "  AND C_UOM_ID=" + _c_Uom_Id);

            DataSet ds = DB.ExecuteDataset(sql.ToString());
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    PriceList = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceList"]);
                }
            }
            sql.Clear();
            retDic["PriceList"] = PriceList;
            return retDic;
        }

    }
}

