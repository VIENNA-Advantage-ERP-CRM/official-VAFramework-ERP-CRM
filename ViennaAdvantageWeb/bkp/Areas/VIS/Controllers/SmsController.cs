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
    public class SmsController : Controller
    {
        //
        // GET: /VIS/Sms/
        public ActionResult Index()
        {
            return View();
        }

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        public JsonResult SendSms(string sms, string format)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            List<SmsHelper> smss = JsonConvert.DeserializeObject<List<SmsHelper>>(sms);
            SmsModel model = new SmsModel(ctx);
            SmsResponse result = model.Send(smss, format);
            return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 09 June 2017
        public JsonResult GetUser(int BPartner_ID)
        {
            Ctx ct = Session["ctx"] as Ctx;
            SmsModel model = new SmsModel(ct);
            return Json(JsonConvert.SerializeObject(model.GetUser(BPartner_ID)), JsonRequestBehavior.AllowGet);
        }
    }
}