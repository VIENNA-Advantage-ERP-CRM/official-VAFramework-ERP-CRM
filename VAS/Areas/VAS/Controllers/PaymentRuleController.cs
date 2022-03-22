using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Utility;
using VIS.Filters;
using VIS.Models;

namespace VIS.Controllers
{
    public class PaymentRuleController : Controller
    {
        //
        // GET: /VIS/PaymentRule/
        public ActionResult Index()
        {
            return View();
        }

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult LoadPaymentMethodDetails(PaymentInputValues inputs)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            PaymentModel model = new PaymentModel(ctx, inputs);
            PaymentMetohdDetails details = model.DynInit();
            return Json(JsonConvert.SerializeObject(details), JsonRequestBehavior.AllowGet); ;
        }

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult SaveChanges(PaymentInputValues inputs)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            PaymentModel model = new PaymentModel(ctx, inputs);
            TabDetails details = model.SaveChanges(inputs);
            return Json(JsonConvert.SerializeObject(details), JsonRequestBehavior.AllowGet); ;
        }



        //public JsonResult ValidateCreditCardNumber(string KNumber, string CCType)
        //{
        //    Ctx ctx = Session["ctx"] as Ctx;
        //    PaymentModel model = new PaymentModel(ctx);
        //    PaymentMetohdDetails details = model.DynInit();
        //    return Json(JsonConvert.SerializeObject(details), JsonRequestBehavior.AllowGet); ;
        //}

        //public JsonResult ValidateCreditCardNumberAndExp(string KNumber, string CCType, string KExp)
        //{
        //    Ctx ctx = Session["ctx"] as Ctx;
        //    PaymentModel model = new PaymentModel(ctx);
        //    PaymentMetohdDetails details = model.DynInit();
        //    return Json(JsonConvert.SerializeObject(details), JsonRequestBehavior.AllowGet); ;
        //}

        //public JsonResult ValidateRoutingNo(string SRouting)
        //{
        //    Ctx ctx = Session["ctx"] as Ctx;
        //    PaymentModel model = new PaymentModel(ctx);
        //    PaymentMetohdDetails details = model.DynInit();
        //    return Json(JsonConvert.SerializeObject(details), JsonRequestBehavior.AllowGet); ;
        //}


        //public JsonResult ValidateAccountNo(string SNumber)
        //{
        //    Ctx ctx = Session["ctx"] as Ctx;
        //    PaymentModel model = new PaymentModel(ctx);
        //    PaymentMetohdDetails details = model.DynInit();
        //    return Json(JsonConvert.SerializeObject(details), JsonRequestBehavior.AllowGet); ;
        //}



        //public JsonResult ValidateCheckNo(string SCheck)
        //{
        //    Ctx ctx = Session["ctx"] as Ctx;
        //    PaymentModel model = new PaymentModel(ctx);
        //    PaymentMetohdDetails details = model.DynInit();
        //    return Json(JsonConvert.SerializeObject(details), JsonRequestBehavior.AllowGet); ;
        //}


    }
}