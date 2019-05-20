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
    public class MPriceListVersionController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetM_PriceList_Version_ID(string fields)
        {
            string retError = "";
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MPriceListVersionModel objPriceList = new MPriceListVersionModel();
                retJSON = JsonConvert.SerializeObject(objPriceList.GetM_PriceList_Version_ID(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
    }
}