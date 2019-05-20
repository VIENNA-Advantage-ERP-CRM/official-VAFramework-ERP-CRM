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
    public class MProductPricingController:Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetProductPricing(string fields)
        {
            
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MProductPricingModel objProductPricing = new MProductPricingModel();
                retJSON = JsonConvert.SerializeObject(objProductPricing.GetProductPricing(ctx,fields));
            }          
            // return Json(new { result = retJSON, error = retError }, JsonRequestBehavior.AllowGet);
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
    }
}