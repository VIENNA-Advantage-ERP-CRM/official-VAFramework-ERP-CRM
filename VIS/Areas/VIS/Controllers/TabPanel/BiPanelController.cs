using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Utility;
using VIS.Model;

namespace VIS.Controllers
{
    public class BiPanelController : Controller
    {
        // GET: VIS/BiPanel
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetUserBILogin()
        {
            Ctx ctx = Session["ctx"] as Ctx;
            BiPanelModel model = new BiPanelModel(ctx);
            List<string> result=  model.GetUserBILogin();
            return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        }
    }
}