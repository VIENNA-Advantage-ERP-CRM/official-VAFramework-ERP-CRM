﻿using System;
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
        public JsonResult GetData(string tableName,int pageNo, string SelectedIDs
            , bool Requery, string Infos, string ValidationCode, string SrchCtrls)
        {
            InfoWindowModel model = new InfoWindowModel();
            Info inf = JsonConvert.DeserializeObject<Info>(Infos);
            List<InfoSearchCol> SrchCtrl= JsonConvert.DeserializeObject<List<InfoSearchCol>>(SrchCtrls);
            //model.GetSchema(Ad_InfoWindow_ID);
            return Json(JsonConvert.SerializeObject(model.GetData(tableName,pageNo, Session["ctx"] as Ctx,
                SelectedIDs, Requery, inf, ValidationCode,SrchCtrl)), JsonRequestBehavior.AllowGet);
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
