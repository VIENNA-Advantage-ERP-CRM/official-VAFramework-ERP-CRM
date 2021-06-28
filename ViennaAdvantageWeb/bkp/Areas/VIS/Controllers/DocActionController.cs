using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Utility;
using VIS.Filters;
using VIS.Models;
using VIS.Classes;

namespace VIS.Controllers
{
    public class DocActionController : Controller
    {
        //
        // GET: /VIS/DocAction/
        public ActionResult Index()
        {
            return View();
        }

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        [ValidateInput(false)]

        public JsonResult GetDocActions(int AD_Table_ID, int Record_ID, string docStatus, bool processing, string orderType, bool isSOTrx, string docAction, string tableName, string values, string names)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            DocActionModel model = new DocActionModel(ctx);
            List<string> lstValues = JsonConvert.DeserializeObject<List<string>>(values);
            List<string> lstNames = JsonConvert.DeserializeObject<List<string>>(names);

            DocAtions action = model.GetActions(AD_Table_ID, Record_ID, docStatus, Util.GetValueOfBool(processing), orderType, Util.GetValueOfBool(isSOTrx), docAction, tableName, lstValues, lstNames);
            return Json(JsonConvert.SerializeObject(action), JsonRequestBehavior.AllowGet); ;
        }

        // Added by Bharat on 06 June 2017
        public JsonResult GetReference(string RefQry)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            DocActionModel model = new DocActionModel(ctx);
            RefQry = SecureEngineBridge.DecryptByClientKey(RefQry, ctx.GetSecureKey());
            return Json(JsonConvert.SerializeObject(model.GetReference(RefQry)), JsonRequestBehavior.AllowGet); ;
        }
    }
}