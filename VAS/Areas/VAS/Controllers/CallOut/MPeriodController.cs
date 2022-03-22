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
    public class MPeriodController : Controller
    {
        // GET: VIS/MPeriod
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get Period ID
        /// </summary>
        /// <param name="fields">Parameter</param>
        /// <returns>Period ID</returns>
        public JsonResult GetPeriod(string fields)
        {
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MPeriodModel objDocType = new MPeriodModel();
                retJSON = JsonConvert.SerializeObject(objDocType.GetPeriod(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Period Detail
        /// </summary>
        /// <param name="fields">Parameter</param>
        /// <returns>Period Detail</returns>
        public JsonResult GetPeriodDetail(string fields)
        {
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MPeriodModel objDocType = new MPeriodModel();
                retJSON = JsonConvert.SerializeObject(objDocType.GetPeriodDetail(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
    }
}