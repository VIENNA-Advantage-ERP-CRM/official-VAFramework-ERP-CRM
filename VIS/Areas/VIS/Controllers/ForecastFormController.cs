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
    /// 
    /// </summary>
    /// <param name="Org_ID"></param>
    /// <param name="Period_ID"></param>
    /// <param name="IncludeSO"></param>
    /// <param name="DocType"></param>
    /// <param name="IncludeOpenSO"></param>
    /// <param name="OpenOrders"></param>
    /// <param name="IncludeOpportunity"></param>
    /// <param name="Opportunity"></param>
    /// <param name="ProductCategory"></param>
    /// <param name="BudgetQunatity"></param>
    /// <param name="DeleteAndGenerateLines"></param>
    /// <param name="Forecast_ID"></param>
    /// <param name="TeamForecast_ID"></param>
    /// <param name="Table_ID"></param>
    /// <returns></returns>
        public JsonResult CreateForecastLine(string Org_ID, string Period_ID, string IncludeSO, string DocType, string IncludeOpenSO, string OpenOrders,string IncludeOpportunity,
            string Opportunity,string ProductCategory,string BudgetQunatity,string DeleteAndGenerateLines, string Forecast_ID, string TeamForecast_ID, string Table_ID)
        {
            var ctx = Session["ctx"] as Ctx;
            ForecastFormModel obj = new ForecastFormModel();
            var value = obj.CreateForecastLine(ctx,Util.GetValueOfInt(Org_ID), Util.GetValueOfInt(Period_ID),Util.GetValueOfBool(IncludeSO), Util.GetValueOfInt(DocType),
                Util.GetValueOfBool(IncludeOpenSO),  OpenOrders, Util.GetValueOfBool(IncludeOpportunity),  Opportunity,  ProductCategory,Util.GetValueOfInt(BudgetQunatity),
                Util.GetValueOfBool(DeleteAndGenerateLines), Util.GetValueOfInt(Forecast_ID),Util.GetValueOfInt(TeamForecast_ID), Util.GetValueOfInt(Table_ID));             
            return Json(JsonConvert.SerializeObject(value), JsonRequestBehavior.AllowGet);
        }


    }
}

