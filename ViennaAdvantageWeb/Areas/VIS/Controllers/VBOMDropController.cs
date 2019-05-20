/********************************************************
 * Module Name    : VIS
 * Purpose        : Controller class for VBOMDrop Form
 * Class Used     : 
 * Chronological Development
 * Sarbjit Kaur     27 May 2015
 ******************************************************/
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
    public class VBOMDropController : Controller
    {
        //
        // GET: /VIS/VBOMDrop/
        public ActionResult Index()
        {
            return View();
        }

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult GetDetail()
        {
            Ctx ctx = Session["ctx"] as Ctx;
            VBOMDropModel objVBOMDrop = new VBOMDropModel();
            return Json(JsonConvert.SerializeObject(objVBOMDrop.GetDetail(ctx)), JsonRequestBehavior.AllowGet);
        }

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult GetBOMLines(int productID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            VBOMDropModel objVBOMDrop = new VBOMDropModel();
            return Json(JsonConvert.SerializeObject(objVBOMDrop.CreateMainPanel(ctx,productID)), JsonRequestBehavior.AllowGet);
        }

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]  
        public JsonResult SaveRecord(string[] param)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            List<BOMLines> lstBOMLines= JsonConvert.DeserializeObject<List<BOMLines>>(param[4]);
            VBOMDropModel objVBOMDrop = new VBOMDropModel();
            return Json(JsonConvert.SerializeObject(objVBOMDrop.Cmd_Save(ctx,param,lstBOMLines)), JsonRequestBehavior.AllowGet);
        }
	}
}