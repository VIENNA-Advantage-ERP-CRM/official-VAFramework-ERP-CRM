using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Utility;
using VIS.Models;

namespace VIS.Controllers
{
    public class MVAMPriceListVersionController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetVAM_PriceListVersion_ID(string fields)
        {
            string retError = "";
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MVAMPriceListVersionModel objPriceList = new MVAMPriceListVersionModel();
                retJSON = JsonConvert.SerializeObject(objPriceList.GetVAM_PriceListVersion_ID(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 12/May/2017
        public JsonResult GetPriceList(string fields)
        {            
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MVAMPriceListVersionModel objPriceList = new MVAMPriceListVersionModel();
                retJSON = JsonConvert.SerializeObject(objPriceList.GetPriceList(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
    }
}