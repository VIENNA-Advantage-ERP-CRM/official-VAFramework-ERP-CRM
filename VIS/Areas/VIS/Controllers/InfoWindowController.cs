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
    public class InfoWindowController : Controller
    {
        //
        // GET: /VIS/InfoWindow/

        public ActionResult Index()
        {
            return View();
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult GetSearchColumn(int Ad_InfoWindow_ID)
        {
            Ctx ctx=Session["ctx"] as Ctx;
            InfoWindowModel model = new InfoWindowModel();
            //model.GetSchema(Ad_InfoWindow_ID);
            return Json(new { result =model.GetSchema(Ad_InfoWindow_ID,ctx )}, JsonRequestBehavior.AllowGet);
        }

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult GetData(string sql,string tableName,int pageNo)
        {
            InfoWindowModel model = new InfoWindowModel();
            //model.GetSchema(Ad_InfoWindow_ID);
            return Json(JsonConvert.SerializeObject(model.GetData(sql, tableName,pageNo, Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Mohit to get info window id on the basis of search key passed.
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult GetInfoWindowID(string InfoSearchKey)
        {            
            InfoWindowModel model = new InfoWindowModel();            
            return Json(new { result = model.GetInfoWindowID(InfoSearchKey) }, JsonRequestBehavior.AllowGet);
        }
    }
}
