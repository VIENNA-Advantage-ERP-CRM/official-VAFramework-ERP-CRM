﻿using System.Collections.Generic;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Utility;
using VIS.Filters;
using VIS.Models;

namespace VIS.Controllers
{
    public class InfoGeneralController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]       
        public JsonResult GetSearchColumns(string tableName, string ad_Language, bool isBaseLangage)
        {
            //Change by mohit-to handle translation in general info.
            //Added 2 new parametere- string ad_Language, bool isBaseLangage.
            //Asked by mukesh sir- 09/03/2018

            VIS.Models.InfoGeneralModel model = new Models.InfoGeneralModel();

            return Json(new { result = model.GetSchema(tableName, ad_Language, isBaseLangage) }, JsonRequestBehavior.AllowGet);
            //return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult GetDispalyColumns(int AD_Table_ID, string AD_Language, bool IsBaseLangage, string TableName)
        {
            //Change by mohit-to handle translation in general info.
            //Added 2 new parametere- string AD_Language, bool IsBaseLangage.
            //Asked by mukesh sir - 09/03/2018

            VIS.Models.InfoGeneralModel model = new Models.InfoGeneralModel();

            return Json(new { result = model.GetDisplayCol(AD_Table_ID, AD_Language,IsBaseLangage,TableName) }, JsonRequestBehavior.AllowGet);
            //return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult GetData(string tableName, int AD_Table_ID, int pageNo, Ctx ctx, string keyCol, string selectedIDs,
            bool requery, string srchCtrl, string validationCode)
        {
            VIS.Models.InfoGeneralModel model = new Models.InfoGeneralModel();

            List<InfoSearchCol> srchCtrls = JsonConvert.DeserializeObject<List<InfoSearchCol>>(srchCtrl);
            //model.GetSchema(Ad_InfoWindow_ID);
            return Json(JsonConvert.SerializeObject(model.GetData(tableName, AD_Table_ID, pageNo, Session["ctx"] as Ctx,  keyCol,  selectedIDs,
             requery,srchCtrls,  validationCode)), JsonRequestBehavior.AllowGet);
        }

    }
}