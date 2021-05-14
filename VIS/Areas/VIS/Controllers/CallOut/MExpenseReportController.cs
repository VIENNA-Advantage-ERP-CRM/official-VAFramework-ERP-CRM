using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Models;

namespace VIS.Controllers
{
    public class MExpenseReportController:Controller
    {
        /// <summary>
        /// Get the curency of the pricelist
        /// </summary>
        /// <param name="fields">Time expense ID</param>
        /// <returns>Currency ID</returns>
        public JsonResult GetPriceListCurrency(string fields)
        {

            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MTimeExpense timeExpense = new MTimeExpense(ctx, Util.GetValueOfInt(fields), null);
                MPriceList priceList = new MPriceList(ctx, timeExpense.GetM_PriceList_ID(), null);
                retJSON = JsonConvert.SerializeObject(priceList.GetC_Currency_ID());
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Get price of product
        /// </summary>
        /// <param name="fields">List of Parameters</param>
        /// <returns>Data in JSON Format</returns>
        public JsonResult GetPrices(string fields)
        {
            String retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MExpenseReportModel objExpense = new MExpenseReportModel();
                retJSON = JsonConvert.SerializeObject(objExpense.GetPrices(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Get standard price of product
        /// </summary>
        /// <param name="fields">List of Parameters</param>
        /// <returns>Data in JSON format</returns>
        public JsonResult GetStandardPrice(string fields)
        {
            String retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MExpenseReportModel objExpense = new MExpenseReportModel();
                retJSON = JsonConvert.SerializeObject(objExpense.GetStandardPrice(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get ChargeAmount
        /// </summary>
        /// <param name="fields">Parameters</param>
        /// <returns>Data in JSON Format</returns>
        public JsonResult GetChargeAmount(string fields)
        {
            String retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MExpenseReportModel objExpense = new MExpenseReportModel();
                retJSON = JsonConvert.SerializeObject(objExpense.GetChargeAmount(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

    }
}