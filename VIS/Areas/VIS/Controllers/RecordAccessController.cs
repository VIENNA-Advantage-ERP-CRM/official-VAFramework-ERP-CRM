using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Utility;
using VIS.Areas.VIS.Models;
using VIS.Filters;

namespace VIS.Areas.VIS.Controllers
{
    public class RecordAccessController : Controller
    {
        //
        // GET: /VIS/RecordAccess/
        public ActionResult Index()
        {
            return View();
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult SaveAccess(int VAF_Role_ID, int VAF_TableView_ID, int Record_ID, bool isActive, bool isExclude, bool isReadOnly, bool isDependentEntities,bool isUpdate)
        {
            Ctx ctx=Session["ctx"] as Ctx;            
            RecordAccessModel model = new RecordAccessModel();            
            return Json(new { result = model.SaveAccess(ctx,VAF_Role_ID,VAF_TableView_ID,Record_ID,isActive,isExclude,isReadOnly,isDependentEntities,isUpdate) }, JsonRequestBehavior.AllowGet);
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]        
        public JsonResult DeleteRecord(int VAF_Role_ID, int VAF_TableView_ID, int Record_ID, bool isActive, bool isExclude, bool isReadOnly, bool isDependentEntities)
        {
            RecordAccessModel model = new RecordAccessModel();
            return Json(new { result = model.DeleteRecordAccess( VAF_Role_ID, VAF_TableView_ID, Record_ID, isActive, isExclude, isReadOnly, isDependentEntities) }, JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 06 June 2017
        public JsonResult GetRoles()
        {
            Ctx ctx = Session["ctx"] as Ctx;
            RecordAccessModel model = new RecordAccessModel();            
            return Json(JsonConvert.SerializeObject(model.GetRoles(ctx)), JsonRequestBehavior.AllowGet); ;
        }

        // Added by Bharat on 06 June 2017
        public JsonResult GetRecordAccess(int Table_ID, int Record_ID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            RecordAccessModel model = new RecordAccessModel();
            return Json(JsonConvert.SerializeObject(model.GetRecordAccess(Table_ID, Record_ID, ctx)), JsonRequestBehavior.AllowGet); ;
        }
	}
}