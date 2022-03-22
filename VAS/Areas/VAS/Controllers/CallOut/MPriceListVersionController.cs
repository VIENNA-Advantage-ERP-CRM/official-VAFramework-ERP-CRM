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

        // Added by Bharat on 12/May/2017
        public JsonResult GetPriceList(string fields)
        {            
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MPriceListVersionModel objPriceList = new MPriceListVersionModel();
                retJSON = JsonConvert.SerializeObject(objPriceList.GetPriceList(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get M_PriceList_Version_ID for Contract
        /// </summary>
        /// <param name="fields">fields</param>
        /// <returns></returns>
        public JsonResult GetM_PriceList_Version_ID_On_Transaction_Date(string fields)
        {           
            string retJSON = string.Empty;

            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MPriceListVersionModel objPriceList = new MPriceListVersionModel();
                retJSON = JsonConvert.SerializeObject(objPriceList.GetM_PriceList_Version_ID_On_Transaction_Date(ctx, fields));
            }

            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
    }
}