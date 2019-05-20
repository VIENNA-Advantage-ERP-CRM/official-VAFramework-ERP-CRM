using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Utility;
using VIS.Filters;
using VIS.Helpers;

namespace VIS.Areas.VIS.Controllers
{
    public class MarkModuleController : Controller
    {
        //
        // GET: /VIS/MarkModule/
        public ActionResult Index()
        {
            return View();
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult SaveExportData(int[] moduleId, int[] _recordID, int _tableID, string _strRecordID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            if (ctx != null)
            {
                MarkModuleHelper model = new MarkModuleHelper();
                return Json(new { result = model.SaveExportData(moduleId, _recordID, _tableID, _strRecordID, ctx) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { result = "SessionExpired" }, JsonRequestBehavior.AllowGet);
            }
            
        }
    }
}