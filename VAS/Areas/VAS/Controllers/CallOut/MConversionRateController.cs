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
    public class MConversionRateController:Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        // Get Rate 
        public JsonResult GetRate(string fields)
        {
            
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MConversionRateModel objConversionModel = new MConversionRateModel();
                retJSON = JsonConvert.SerializeObject(objConversionModel.GetRate(ctx,fields));
            }           
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
        
        //Get Convert
        public JsonResult Convert(string fields)
        {
            
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MConversionRateModel objConversionModel = new MConversionRateModel();
                retJSON = JsonConvert.SerializeObject(objConversionModel.Convert(ctx,fields));
            }
           
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        //Get Convert
        public JsonResult CurrencyConvert(string fields)
        {

            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MConversionRateModel objConversionModel = new MConversionRateModel();
                retJSON = JsonConvert.SerializeObject(objConversionModel.CurrencyConvert(ctx, fields));
            }

            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
    }
}