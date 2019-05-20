using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Models;

namespace VIS.Controllers
{
    public class MCashJournalController : Controller
    {
        //public ActionResult Index()
        //{
        //    return View();
        //}
        // Added by Arpit on 13/Dec/2017
        public JsonResult GetBPLocation(string fields)
        {
            string retJSON = "";
            Ctx ctx = Session["ctx"] as Ctx;
            MCashJournalModel objCJModel = new MCashJournalModel();
            retJSON = JsonConvert.SerializeObject(objCJModel.GetLocationData(ctx, fields));
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
    }
}