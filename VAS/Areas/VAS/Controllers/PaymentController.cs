/********************************************************
 * Project Name   : VIS
 * Class Name     : PaymentController
 * Purpose        : Used for payment callout.......
 * Chronological    Development
 * Mohit            27/04/2017
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Utility;
using VIS.Models;
using VISLogic.Filters;

namespace VIS.Controllers
{
    public class PaymentController : Controller
    {
        //
        // GET: /VIS/Payment/
        public ActionResult Index()
        {
            return View();
        }


        //Payment callout-invoice selection
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult GetModuleCount(string prefix)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            CalloutPaymentModel model = new CalloutPaymentModel(ctx);
            bool _check = model.CheckedModuleInfo(prefix);
            return Json(JsonConvert.SerializeObject(_check), JsonRequestBehavior.AllowGet);
        }

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult GetLCNo(int Invoice_ID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            CalloutPaymentModel model = new CalloutPaymentModel(ctx);
            LcDetails LcDet = model.GetLcDetail(Invoice_ID, ctx);
            return Json(JsonConvert.SerializeObject(LcDet), JsonRequestBehavior.AllowGet);
        }
        //
	}
}