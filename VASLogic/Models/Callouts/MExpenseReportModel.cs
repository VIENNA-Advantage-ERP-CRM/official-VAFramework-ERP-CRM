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
            decimal PriceStd = 0;
            int _m_PriceList_ID = 0;
            int _priceListVersion_Id = 0;
            _m_PriceList_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_PriceList_ID FROM S_TimeExpense  WHERE S_TimeExpense_ID =" + _s_TimeExpense_ID, null, null));
            _priceListVersion_Id = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_PriceList_Version_ID,name FROM M_PriceList_Version WHERE IsActive='Y' AND M_PriceList_ID=" + _m_PriceList_ID + "ORDER BY ValidFrom DESC", null, null));
            sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                        + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                         + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                        + "  AND C_UOM_ID=" + _c_Uom_Id);
            PriceStd = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));
            sql.Clear();
            retDic["PriceStd"] = PriceStd;
            return retDic;
        }

        /// <summary>
        /// Get Standard Price
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="param">List of parameters</param>
        /// <returns>price</returns>
        public Dictionary<String, Object> GetStandardPrice(Ctx ctx, string param)
        {
            string[] paramValue = param.Split(',');

            Dictionary<String, Object> retDic = new Dictionary<String, Object>();

            //Assign parameter value
            int _m_Product_Id = Util.GetValueOfInt(paramValue[0].ToString());
            int _s_TimeExpense_ID = Util.GetValueOfInt(paramValue[1].ToString());
            int _c_Uom_Id = Util.GetValueOfInt(paramValue[2].ToString());
            DateTime? dateExpense = Util.GetValueOfDateTime(paramValue[3].ToString());
            StringBuilder sql = new StringBuilder();
            int _m_PriceList_ID = 0;
            DateTime? validFrom;
            decimal priceActual = 0;
            int currency = 0;
            bool noPrice = true;
            _m_PriceList_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_PriceList_ID FROM S_TimeExpense  WHERE S_TimeExpense_ID =" + _s_TimeExpense_ID, null, null));
            sql.Append("SELECT pp.PriceStd, pp.PriceList,pp.PriceLimit ,"
                + "pp.C_UOM_ID,pv.ValidFrom,pl.C_Currency_ID "
                + "FROM M_Product p, M_ProductPrice pp, M_PriceList pl, M_PriceList_Version pv "
                + "WHERE p.M_Product_ID=pp.M_Product_ID"
                + " AND pp.M_PriceList_Version_ID=pv.M_PriceList_Version_ID"
                + " AND pv.M_PriceList_ID=pl.M_PriceList_ID"
                + " AND pv.IsActive='Y'"
                + " AND p.M_Product_ID=" + _m_Product_Id
                + " AND pl.M_PriceList_ID=" + _m_PriceList_ID
                + " AND pp.C_UOM_ID=" + _c_Uom_Id
                + " ORDER BY pv.ValidFrom DESC");

            DataSet ds = DB.ExecuteDataset(sql.ToString());
            if (ds != null && ds.Tables.Count > 0 && noPrice)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    validFrom = Util.GetValueOfDateTime(ds.Tables[0].Rows[0]["ValidFrom"]);
                    if (validFrom == null || !(dateExpense < validFrom))
                    {
                        noPrice = false;
                        priceActual = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]);

                        if (priceActual == 0)
                        {
                            priceActual = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceList"]);//.GetDecimal("PriceList");
                        }
                        if (priceActual == 0)
                        {
                            priceActual = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceLimit"]);//.GetDecimal("PriceLimit");
                        }
                        //	Currency
                        currency = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_Currency_ID"]);

                    }
                }

            }
            if (noPrice)
            {
                sql.Clear();
                //	Find if via Base Pricelist
                sql.Append("SELECT  pp.PriceStd, pp.PriceList,pp.PriceLimit ,"
                    + "pp.C_UOM_ID,pv.ValidFrom,pl.C_Currency_ID "
                    + "FROM M_Product p, M_ProductPrice pp, M_PriceList pl, M_PriceList bpl, M_PriceList_Version pv "
                    + "WHERE p.M_Product_ID=pp.M_Product_ID"
                    + " AND pp.M_PriceList_Version_ID=pv.M_PriceList_Version_ID"
                    + " AND pv.M_PriceList_ID=bpl.M_PriceList_ID"
                    + " AND pv.IsActive='Y'"
                    + " AND bpl.M_PriceList_ID=pl.BasePriceList_ID"	//	Base
                    + " AND p.M_Product_ID=" + _m_Product_Id
                    + " AND pl.M_PriceList_ID=" + _m_PriceList_ID
                    + " ORDER BY pv.ValidFrom DESC");
                DataSet ds1 = DB.ExecuteDataset(sql.ToString());
                if (ds1 != null && ds1.Tables.Count > 0 && noPrice)
                {
                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        validFrom = Util.GetValueOfDateTime(ds1.Tables[0].Rows[0]["ValidFrom"]);
                        if (validFrom == null || !(dateExpense < validFrom))
                        {
                            noPrice = false;
                            priceActual = Util.GetValueOfDecimal(ds1.Tables[0].Rows[0]["PriceStd"]);

                            if (priceActual == 0)
                            {
                                priceActual = Util.GetValueOfDecimal(ds1.Tables[0].Rows[0]["PriceList"]);//.GetDecimal("PriceList");
                            }
                            if (priceActual == 0)
                            {
                                priceActual = Util.GetValueOfDecimal(ds1.Tables[0].Rows[0]["PriceLimit"]);//.GetDecimal("PriceLimit");
                            }
                            //	Currency
                            currency = Util.GetValueOfInt(ds1.Tables[0].Rows[0]["C_Currency_ID"]);

                        }
                    }

                }
            }
            sql.Clear();
            retDic["PriceActual"] = priceActual;
            retDic["C_Currency_ID"] = currency;
            return retDic;
        }

        /// <summary>
        /// Get Charge Amount
        /// </summary>
        /// <param name="ctx">Parameters</param>
        /// <param name="fields"></param>
        /// <returns>Charge Amount</returns>
        public int GetChargeAmount(Ctx ctx,string fields)
        {
            int chargeAmt = Util.GetValueOfInt(DB.ExecuteScalar("SELECT ChargeAmt FROM C_Charge WHERE C_Charge_ID = " + Util.GetValueOfInt(fields), null, null));
            return chargeAmt;
        }
    }
}





