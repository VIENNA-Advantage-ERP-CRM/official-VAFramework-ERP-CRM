using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Utility;
using VIS.Filters;
namespace VIS.Controllers
{
    public class InfoMenuController : Controller
    {
        ////
        //// GET: /VIS/InfoMenu/
        //[HttpGet]
        //[AllowAnonymous]
        public ActionResult Index()
        {  
            return View();
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult LoadMenu()
        {
            VIS.Models.InfoMenuModel model = new Models.InfoMenuModel();
            Ctx ctx = Session["ctx"] as Ctx;
            model.FIllInfoWinMenu(ctx);
            return Json(new { result = model.InfoWinMenu }, JsonRequestBehavior.AllowGet);
            //return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }
    }
}