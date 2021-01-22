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
    public class SubscribeController : Controller
    {
        //
        // GET: /VIS/Subscribe/
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult Subscribe(int VAF_Screen_ID, int Record_ID, int VAF_TableView_ID)
        {
             Ctx ct = Session["ctx"] as Ctx;
             SubscribeModel model = new SubscribeModel(ct);
             var result= model.InsertSubscription(VAF_Screen_ID,Record_ID,VAF_TableView_ID);
             return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        }


        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult UnSubscribe(int VAF_Screen_ID, int Record_ID, int VAF_TableView_ID)
        {
            Ctx ct = Session["ctx"] as Ctx;
            SubscribeModel model = new SubscribeModel(ct);
            var result = model.DeleteSubscription(VAF_Screen_ID,Record_ID,VAF_TableView_ID);
            return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        }
       
	}
}