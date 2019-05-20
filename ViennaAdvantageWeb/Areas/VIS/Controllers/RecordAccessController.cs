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
        public JsonResult SaveAccess(int AD_Role_ID, int AD_Table_ID, int Record_ID, bool isActive, bool isExclude, bool isReadOnly, bool isDependentEntities,bool isUpdate)
        {
            Ctx ctx=Session["ctx"] as Ctx;            
            RecordAccessModel model = new RecordAccessModel();            
            return Json(new { result = model.SaveAccess(ctx,AD_Role_ID,AD_Table_ID,Record_ID,isActive,isExclude,isReadOnly,isDependentEntities,isUpdate) }, JsonRequestBehavior.AllowGet);
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]        
        public JsonResult DeleteRecord(int AD_Role_ID, int AD_Table_ID, int Record_ID, bool isActive, bool isExclude, bool isReadOnly, bool isDependentEntities)
        {
            RecordAccessModel model = new RecordAccessModel();
            return Json(new { result = model.DeleteRecordAccess( AD_Role_ID, AD_Table_ID, Record_ID, isActive, isExclude, isReadOnly, isDependentEntities) }, JsonRequestBehavior.AllowGet);
        }
	}
}