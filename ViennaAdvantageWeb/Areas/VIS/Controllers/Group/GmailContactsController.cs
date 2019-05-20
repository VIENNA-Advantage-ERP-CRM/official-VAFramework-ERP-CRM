using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Utility;
using VIS.Models;

namespace VIS.Controllers
{
    public class GmailContactsController : Controller
    {
        //
        // GET: /VIS/GmailContacts/
        public ActionResult Index()
        {
            return View();
        }

        //[HttpGet]
        //public JsonResult GetRole()
        //{
        //    //ViewBag.WindowNumber = WinNo;
        //    VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
        //    GmailContactsSettings objContactSettingModel = new GmailContactsSettings();
        //    List<RolesInfo> lstRole = new List<RolesInfo>();
        //    lstRole = objContactSettingModel.FillRole(ctx);
        //    return Json(JsonConvert.SerializeObject(lstRole), JsonRequestBehavior.AllowGet);
        //}


        //[HttpGet]
        //public JsonResult GetSavedDetail()
        //{
        //    //ViewBag.WindowNumber = WinNo;
        //    VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
        //    GmailContactsSettings objContactSettingModel = new GmailContactsSettings();
        //    return Json(JsonConvert.SerializeObject(objContactSettingModel.GetSavedGmailCredential(ctx)), JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public JsonResult GetContacts(string username, string password, int role_ID, bool isUpdateExisting)
        //{
        //    //ViewBag.WindowNumber = WinNo;
        //    VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
        //    GmailContactsSettings objContactSettingModel = new GmailContactsSettings();
        //    string message = objContactSettingModel.GetContacts(ctx, username, password, role_ID, isUpdateExisting);
        //    if (objContactSettingModel.objMyContact.Count == 0)
        //    {
        //        return Json(JsonConvert.SerializeObject(message), JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(JsonConvert.SerializeObject(objContactSettingModel.objMyContact), JsonRequestBehavior.AllowGet);
        //}


	}
}