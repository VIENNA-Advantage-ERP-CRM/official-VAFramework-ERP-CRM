/********************************************************
 * Module Name    :    VA Framework
 * Purpose        :    Implement Generate Lines functionality for Team amd Master Forecast
 * Employee Code  :    209
 * Date           :    26-April-2021
  ******************************************************/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Utility;
using VIS.Models;

namespace VIS.Controllers
{
    public class ForecastFormController : Controller
    {
        // GET: VIS/Forecast
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        ///  Load org ID and check window (Either team forecast and master Forecast) and Team Forecast reference 
        /// </summary>
        /// <param name="AD_Table_ID">Table ID</param>
        /// <param name="AD_Record_ID">Record ID</param>
        /// <returns>dictionary object</returns>
        public JsonResult GetTableAndRecordInfo(string AD_Table_ID, string AD_Record_ID)
        {
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                ForecastFormModel objForecast = new ForecastFormModel();
                retJSON = JsonConvert.SerializeObject(objForecast.GetTableAndRecordInfo(ctx, AD_Table_ID, AD_Record_ID));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Generate Lines for master or team forecast
        /// </summary>
        /// <param name="Org_ID">Organization</param>
        /// <param name="Period_ID">Period</param>
        /// <param name="IncludeSO">Include SlesOrder</param>
        /// <param name="DocType">Document Type</param>
        /// <param name="IncludeOpenSO">Include OpenSlesOrder</param>
        /// <param name="OpenOrders">Open SalesOrder</param>
        /// <param name="IncludeOpportunity">Include Opportunit</param>
        /// <param name="Opportunity">Opportunity</param>
        /// <param name="ProductCategory">Product Category</param>
        /// <param name="BudgetQunatity">Budget Quantity</param>
        /// <param name="DeleteAndGenerateLines">DeleteAndGenerateLines</param>
        /// <param name="Forecast_ID">Team/master Forecast</param>
        /// <param name="TeamForecast_IDS">Teamforecast</param>
        /// <param name="Table_ID">Table</param>
        /// <param name="IsMasterForecast">IsmasterForecast</param>
        /// <param name="SalesPriceList_ID">Sales PriceList</param>
        /// <returns>info</returns>
        public JsonResult CreateForecastLine(string Org_ID, string Period_ID, string IncludeSO, string DocType, string IncludeOpenSO, string OpenOrders, string IncludeOpportunity,
            string Opportunity, string ProductCategory, string BudgetQunatity, string DeleteAndGenerateLines, string Forecast_ID, string TeamForecast_IDS, string Table_ID,
            string IsMasterForecast,string IsBudgetForecast,string MasterForecast_IDs, string SalesPriceList_ID)
        {
            var ctx = Session["ctx"] as Ctx;
            ForecastFormModel obj = new ForecastFormModel();
            var value = obj.CreateForecastLine(ctx, Util.GetValueOfInt(Org_ID), Util.GetValueOfInt(Period_ID), Util.GetValueOfBool(IncludeSO), Util.GetValueOfInt(DocType),
                Util.GetValueOfBool(IncludeOpenSO), OpenOrders, Util.GetValueOfBool(IncludeOpportunity), Opportunity, ProductCategory, Util.GetValueOfInt(BudgetQunatity),
                Util.GetValueOfBool(DeleteAndGenerateLines), Util.GetValueOfInt(Forecast_ID), Util.GetValueOfString(TeamForecast_IDS), Util.GetValueOfInt(Table_ID), 
                Util.GetValueOfBool(IsMasterForecast), Util.GetValueOfBool(IsBudgetForecast), Util.GetValueOfString(MasterForecast_IDs), Util.GetValueOfInt(SalesPriceList_ID));
            return Json(JsonConvert.SerializeObject(value), JsonRequestBehavior.AllowGet);
        }


    }
}

